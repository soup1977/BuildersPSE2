Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.DataAccess
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.BuildersPSE.Utilities
Imports Microsoft.VisualBasic.FileIO

Public Class FrmPSE
    Inherits Form

    ' Data Tracking
    Private selectedMappingID As Integer = -1
    Private selectedLevelID As Integer = -1
    Private selectedProjectID As Integer = -1
    Private selectedBuildingID As Integer = -1 ' Cached for efficiency
    Private selectedRawUnitID As Integer = -1 ' Track selected RawUnit for database
    Private displayUnits As New List(Of DisplayUnitData) ' Merged with ActualUnit reference
    Private bindingSource As New BindingSource
    Private rawUnits As New List(Of RawUnitModel)
    Private dataAccess As New DataAccess
    Private isImporting As Boolean = False
    Private isReusing As Boolean = False
    Private selectedActualUnitID As Integer = -1
    Private existingActualUnits As New List(Of ActualUnitModel)
    Private marginCache As New Dictionary(Of Integer, Decimal)

    ' Class-level fields to store column references for toggling
    Private colActualUnitID As DataGridViewColumn
    Private colMappingID As DataGridViewColumn
    Private colUnitName As DataGridViewColumn
    Private colReferencedRawUnitName As DataGridViewColumn
    Private colPlanSQFT As DataGridViewColumn
    Private colActualUnitQuantity As DataGridViewColumn
    Private colLF As DataGridViewColumn
    Private colBDFT As DataGridViewColumn
    Private colLumberCost As DataGridViewColumn
    Private colPlateCost As DataGridViewColumn
    Private colManufLaborCost As DataGridViewColumn
    Private colDesignLabor As DataGridViewColumn
    Private colMGMTLabor As DataGridViewColumn
    Private colJobSuppliesCost As DataGridViewColumn
    Private colManHours As DataGridViewColumn
    Private colItemCost As DataGridViewColumn
    Private colOverallCost As DataGridViewColumn
    Private colDeliveryCost As DataGridViewColumn
    Private colSellPrice As DataGridViewColumn
    Private colMargin As DataGridViewColumn
    Private colActions As DataGridViewColumn

    Private copiedMappings As New List(Of Tuple(Of Integer, Integer)) ' (ActualUnitID, Quantity)
    Private sourceProductTypeID As Integer = -1

    ' Updated DisplayUnitData to include ActualUnit reference
    Private Class DisplayUnitData
        Public Property ActualUnit As ActualUnitModel ' Reference to full model
        Public Property MappingID As Integer
        Public Property ReferencedRawUnitName As String
        Public Property ActualUnitQuantity As Integer
        Public Property LF As Decimal
        Public Property BDFT As Decimal
        Public Property LumberCost As Decimal
        Public Property PlateCost As Decimal
        Public Property ManufLaborCost As Decimal
        Public Property DesignLabor As Decimal
        Public Property MGMTLabor As Decimal
        Public Property JobSuppliesCost As Decimal
        Public Property ManHours As Decimal
        Public Property ItemCost As Decimal
        Public Property OverallCost As Decimal
        Public Property DeliveryCost As Decimal
        Public Property SellPrice As Decimal
        Public Property Margin As Decimal

        ' Properties now proxy to ActualUnit where possible
        Public ReadOnly Property ActualUnitID As Integer
            Get
                Return If(ActualUnit IsNot Nothing, ActualUnit.ActualUnitID, 0)
            End Get
        End Property

        Public ReadOnly Property UnitName As String
            Get
                Return If(ActualUnit IsNot Nothing, ActualUnit.UnitName, String.Empty)
            End Get
        End Property

        Public ReadOnly Property PlanSQFT As Decimal
            Get
                Return If(ActualUnit IsNot Nothing, ActualUnit.PlanSQFT, 0D)
            End Get
        End Property

        Public ReadOnly Property OptionalAdder As Decimal
            Get
                Return If(ActualUnit IsNot Nothing, ActualUnit.OptionalAdder, 1D)
            End Get
        End Property
    End Class

    Public Sub New(projectID As Integer)
        InitializeComponent()
        selectedProjectID = projectID
        LoadLevelHierarchy()
        SetupUI()
        LoadExistingActualUnits()
    End Sub

    Private Sub SetupUI()
        CmbUnitType.Items.Clear()
        CmbUnitType.Items.AddRange(New Object() {"Res", "NonRes"})
        DataGridViewAssigned.AutoGenerateColumns = False
        bindingSource.DataSource = GetType(DisplayUnitData)
        DataGridViewAssigned.DataSource = bindingSource
        AddCalculatedColumns()
        ChkDetailedView.Checked = False
        ToggleGridView()
    End Sub

    Private Sub AddCalculatedColumns()
        DataGridViewAssigned.Columns.Clear()

        colActualUnitID = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "ActualUnitID",
            .HeaderText = "Actual Unit ID",
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colActualUnitID)

        colMappingID = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "MappingID",
            .HeaderText = "Mapping ID",
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colMappingID)

        colUnitName = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "UnitName",
            .HeaderText = "Actual Unit Name",
            .ReadOnly = True,
            .Visible = True
        }
        DataGridViewAssigned.Columns.Add(colUnitName)

        colReferencedRawUnitName = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "ReferencedRawUnitName",
            .HeaderText = "Referenced Raw Unit Name",
            .ReadOnly = True,
            .Visible = True
        }
        DataGridViewAssigned.Columns.Add(colReferencedRawUnitName)

        colPlanSQFT = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "PlanSQFT",
            .HeaderText = "Plan SQFT",
            .ReadOnly = True,
            .Visible = True
        }
        DataGridViewAssigned.Columns.Add(colPlanSQFT)

        colActualUnitQuantity = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "ActualUnitQuantity",
            .HeaderText = "Actual Unit Quantity",
            .ReadOnly = True,
            .Visible = True
        }
        DataGridViewAssigned.Columns.Add(colActualUnitQuantity)

        colLF = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "LF",
            .HeaderText = "LF",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colLF)

        colBDFT = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "BDFT",
            .HeaderText = "BDFT",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colBDFT)

        colLumberCost = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "LumberCost",
            .HeaderText = "Lumber $$",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colLumberCost)

        colPlateCost = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "PlateCost",
            .HeaderText = "Plate $$",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colPlateCost)

        colManufLaborCost = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "ManufLaborCost",
            .HeaderText = "Manuf Labor $$",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colManufLaborCost)

        colDesignLabor = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "DesignLabor",
            .HeaderText = "Design Labor $$",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colDesignLabor)

        colMGMTLabor = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "MGMTLabor",
            .HeaderText = "MGMT Labor $$",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colMGMTLabor)

        colJobSuppliesCost = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "JobSuppliesCost",
            .HeaderText = "Job Supplies $$",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colJobSuppliesCost)

        colManHours = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "ManHours",
            .HeaderText = "Man Hours",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colManHours)

        colItemCost = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "ItemCost",
            .HeaderText = "Item $$",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colItemCost)

        colDeliveryCost = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "DeliveryCost",
            .HeaderText = "Delivery $$",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colDeliveryCost)

        colOverallCost = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "OverallCost",
            .HeaderText = "Overall Cost",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colOverallCost)

        colSellPrice = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "SellPrice",
            .HeaderText = "Sell Price",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colSellPrice)

        colMargin = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "Margin",
            .HeaderText = "Margin $$",
            .ReadOnly = True,
            .Visible = False
        }
        DataGridViewAssigned.Columns.Add(colMargin)

        colActions = New DataGridViewButtonColumn With {
            .Name = "ActionsColumn",
            .HeaderText = "Actions",
            .Text = "Edit",
            .UseColumnTextForButtonValue = True,
            .Visible = True
        }
        DataGridViewAssigned.Columns.Add(colActions)
    End Sub

    Private Sub ToggleGridView()
        Dim isDetailedView As Boolean = ChkDetailedView.Checked
        colLF.Visible = isDetailedView
        colBDFT.Visible = isDetailedView
        colLumberCost.Visible = isDetailedView
        colPlateCost.Visible = isDetailedView
        colManufLaborCost.Visible = isDetailedView
        colDesignLabor.Visible = isDetailedView
        colMGMTLabor.Visible = isDetailedView
        colJobSuppliesCost.Visible = isDetailedView
        colManHours.Visible = isDetailedView
        colItemCost.Visible = isDetailedView
        colOverallCost.Visible = isDetailedView
        colSellPrice.Visible = isDetailedView
        colMargin.Visible = isDetailedView
        colDeliveryCost.Visible = isDetailedView
        DataGridViewAssigned.Refresh()
        LblStatus.Text = $"Grid view switched to {(If(isDetailedView, "Detailed", "Base"))} mode."
    End Sub

    Private Sub ChkDetailedView_CheckedChanged(sender As Object, e As EventArgs) Handles ChkDetailedView.CheckedChanged
        ToggleGridView()
    End Sub

    Private Sub LoadLevelHierarchy()
        TreeViewLevels.Nodes.Clear()
        Dim project = dataAccess.GetProjectByID(selectedProjectID)
        If project IsNot Nothing Then
            Dim projectNode As TreeNode = TreeViewLevels.Nodes.Add(project.ProjectName)
            projectNode.Tag = New Dictionary(Of String, Object) From {{"Type", "Project"}, {"ID", selectedProjectID}}
            Dim buildings = dataAccess.GetBuildingsByProjectID(selectedProjectID)
            For Each bldg In buildings
                Dim bldgNode As TreeNode = projectNode.Nodes.Add(bldg.BuildingName)
                bldgNode.Tag = New Dictionary(Of String, Object) From {{"Type", "Building"}, {"ID", bldg.BuildingID}}
                Dim levels = dataAccess.GetLevelsByBuildingID(bldg.BuildingID)
                For Each lvl In levels
                    Dim levelNode As TreeNode = bldgNode.Nodes.Add(lvl.ProductTypeName & " Level " & lvl.LevelNumber)
                    levelNode.Tag = New Dictionary(Of String, Object) From {{"Type", "Level"}, {"ID", lvl.LevelID}, {"ProductTypeID", lvl.ProductTypeID}}
                Next
            Next
            projectNode.Expand()
        Else
            LblStatus.Text = "Status: Project not found."
        End If
    End Sub

    Private Sub TreeViewLevels_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeViewLevels.AfterSelect
        If e.Node.Level = 2 Then ' Level node
            Dim levelTag = DirectCast(e.Node.Tag, Dictionary(Of String, Object))
            selectedLevelID = CInt(levelTag("ID"))
            Dim productTypeID As Integer = CInt(levelTag("ProductTypeID"))
            FilterRawUnits(productTypeID)
            selectedBuildingID = dataAccess.GetBuildingIDByLevelID(selectedLevelID) ' Cache
            LoadAssignedUnits()
        Else
            ListBoxAvailableUnits.Items.Clear()
            displayUnits.Clear()
            bindingSource.DataSource = Nothing
            selectedBuildingID = -1
            UpdateLevelTotals()
            UpdateSelectedRawPreview()
        End If
    End Sub

    Private Sub FilterRawUnits(productTypeID As Integer)
        ListBoxAvailableUnits.Items.Clear()
        rawUnits = dataAccess.GetRawUnitsByProjectID(selectedProjectID).Where(Function(r) r.ProductTypeID = productTypeID).ToList()
        For Each rawUnit In rawUnits
            ListBoxAvailableUnits.Items.Add(rawUnit.RawUnitName)
        Next
        UpdateSelectedRawPreview() ' Update preview on filter
    End Sub

    Private Sub LoadAssignedUnits()
        Try
            displayUnits.Clear()
            Dim mappings = dataAccess.GetActualToLevelMappingsByLevelID(selectedLevelID)
            For Each mapping In mappings
                Dim actualUnit = mapping.ActualUnit
                Dim displayUnit As New DisplayUnitData With {
                    .ActualUnit = actualUnit,
                    .MappingID = mapping.MappingID,
                    .ReferencedRawUnitName = actualUnit.ReferencedRawUnitName,
                    .ActualUnitQuantity = mapping.Quantity
                }

                Dim components As List(Of CalculatedComponentModel) = actualUnit.CalculatedComponents
                If components Is Nothing Then Continue For

                CalculateDerivedFields(displayUnit, components)

                Dim productTypeID As Integer = actualUnit.ProductTypeID
                Dim rawUnit = rawUnits.FirstOrDefault(Function(r) r.RawUnitID = actualUnit.RawUnitID)
                If rawUnit Is Nothing Then Continue For

                Dim projectLumberAdder As Decimal = dataAccess.GetLumberAdder(selectedProjectID, productTypeID)
                If projectLumberAdder > 0 Then
                    Dim avgSPFNo2 As Decimal = rawUnit.AvgSPFNo2.GetValueOrDefault()
                    Dim effectiveLumberAdder As Decimal = projectLumberAdder - avgSPFNo2
                    If effectiveLumberAdder > 0 Then
                        Dim adderAmount As Decimal = (displayUnit.BDFT / 1000D) * effectiveLumberAdder
                        displayUnit.LumberCost += adderAmount
                        displayUnit.OverallCost += adderAmount
                    End If
                End If

                Dim marginPercent As Decimal = GetEffectiveMargin(productTypeID, actualUnit.RawUnitID)
                displayUnit.SellPrice = If(marginPercent >= 1D, displayUnit.OverallCost, displayUnit.OverallCost / (1D - marginPercent))
                displayUnit.Margin = displayUnit.SellPrice - displayUnit.OverallCost

                displayUnits.Add(displayUnit)
            Next

            ProrateDeliveryCosts(selectedLevelID, displayUnits)

            bindingSource.DataSource = displayUnits
            bindingSource.ResetBindings(False)
            DataGridViewAssigned.Refresh()
            UpdateLevelTotals()
        Catch ex As Exception
            MessageBox.Show("Error loading assigned units: " & ex.Message)
        End Try
    End Sub

    Private Sub ProrateDeliveryCosts(levelID As Integer, ByRef displayUnitsList As List(Of DisplayUnitData))
        Dim totalBDFT As Decimal = displayUnitsList.Sum(Function(u) u.BDFT)
        Dim mileageRate As Decimal = dataAccess.GetConfigValue("MileageRate")
        Dim milesToJobSite As Integer = dataAccess.GetMilesToJobSite(selectedProjectID)
        Dim deliveryTotal As Decimal = 0D

        If totalBDFT > 0 Then
            Dim numLoads As Decimal = Math.Ceiling(totalBDFT / 10000D)
            deliveryTotal = numLoads * mileageRate * milesToJobSite
            deliveryTotal = Math.Round(deliveryTotal, 0)
            If deliveryTotal < 150 Then deliveryTotal = 150
        End If

        For Each unit In displayUnitsList
            Dim productTypeID As Integer = unit.ActualUnit.ProductTypeID
            Dim marginPercent As Decimal = GetEffectiveMargin(productTypeID, unit.ActualUnit.RawUnitID)

            If totalBDFT > 0 Then
                unit.DeliveryCost = (unit.BDFT / totalBDFT) * deliveryTotal
            Else
                unit.DeliveryCost = 0D
            End If

            unit.SellPrice = If(marginPercent >= 1D, unit.OverallCost, unit.OverallCost / (1D - marginPercent)) + unit.DeliveryCost
            unit.Margin = unit.SellPrice - unit.OverallCost
        Next
    End Sub

    Private Function GetEffectiveMargin(productTypeID As Integer, rawUnitID As Integer) As Decimal
        If marginCache.ContainsKey(productTypeID) Then Return marginCache(productTypeID)

        Dim margin As Decimal = dataAccess.GetMarginPercent(selectedProjectID, productTypeID)
        If margin = 0D Then
            Dim rawUnit = dataAccess.GetRawUnitByID(rawUnitID)
            If rawUnit IsNot Nothing AndAlso rawUnit.TotalSellPrice.GetValueOrDefault() > 0D Then
                margin = (rawUnit.TotalSellPrice.Value - rawUnit.OverallCost.GetValueOrDefault()) / rawUnit.TotalSellPrice.Value
            End If
        End If

        marginCache(productTypeID) = margin
        Return margin
    End Function

    Private Sub UpdateLevelTotals()
        Dim totalSQFT As Decimal = displayUnits.Sum(Function(u) u.PlanSQFT * u.ActualUnitQuantity)
        TxtTotalPlanSQFT.Text = totalSQFT.ToString("F2")
        TxtTotalQuantity.Text = displayUnits.Sum(Function(u) u.ActualUnitQuantity).ToString("F0")
        TxtTotalLF.Text = displayUnits.Sum(Function(u) u.LF).ToString("F2")
        TxtTotalBDFT.Text = displayUnits.Sum(Function(u) u.BDFT).ToString("F2")
        TxtTotalLumberCost.Text = displayUnits.Sum(Function(u) u.LumberCost).ToString("F2")
        TxtTotalPlateCost.Text = displayUnits.Sum(Function(u) u.PlateCost).ToString("F2")
        TxtTotalManufLaborCost.Text = displayUnits.Sum(Function(u) u.ManufLaborCost).ToString("F2")
        TxtTotalDesignLabor.Text = displayUnits.Sum(Function(u) u.DesignLabor).ToString("F2")
        TxtTotalMGMTLabor.Text = displayUnits.Sum(Function(u) u.MGMTLabor).ToString("F2")
        TxtTotalJobSuppliesCost.Text = displayUnits.Sum(Function(u) u.JobSuppliesCost).ToString("F2")
        TxtTotalManHours.Text = displayUnits.Sum(Function(u) u.ManHours).ToString("F2")
        TxtTotalItemCost.Text = displayUnits.Sum(Function(u) u.ItemCost).ToString("F2")
        TxtTotalOverallCost.Text = displayUnits.Sum(Function(u) u.OverallCost).ToString("F2")
        TxtTotalSellPrice.Text = displayUnits.Sum(Function(u) u.SellPrice).ToString("F2")
        TxtTotalMargin.Text = displayUnits.Sum(Function(u) u.Margin).ToString("F2")
        txtTotalDeliveryCost.Text = displayUnits.Sum(Function(u) u.DeliveryCost).ToString("F2")
        LblStatus.Text = $"Total Adjusted SQFT: {totalSQFT} - Recalc complete. No variations detected... yet."
    End Sub

    Private Sub UpdateSelectedRawPreview()
        Dim currentRawUnit = If(ListBoxAvailableUnits.SelectedIndex >= 0, rawUnits.FirstOrDefault(Function(r) r.RawUnitName = ListBoxAvailableUnits.Items(ListBoxAvailableUnits.SelectedIndex).ToString()), Nothing)
        If currentRawUnit IsNot Nothing AndAlso currentRawUnit.SqFt.GetValueOrDefault() > 0 Then
            TxtLumberPerSQFT.Text = (currentRawUnit.LumberCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
            TxtPlatePerSQFT.Text = (currentRawUnit.PlateCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
            TxtBDFTPerSQFT.Text = (currentRawUnit.BF.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
            TxtManufLaborPerSQFT.Text = (currentRawUnit.ManufLaborCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
            TxtDesignLaborPerSQFT.Text = (currentRawUnit.DesignLabor.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
            TxtMGMTLaborPerSQFT.Text = (currentRawUnit.MGMTLabor.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
            TxtJobSuppliesPerSQFT.Text = (currentRawUnit.JobSuppliesCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
            TxtManHoursPerSQFT.Text = (currentRawUnit.ManHours.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
            TxtItemCostPerSQFT.Text = (currentRawUnit.ItemCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
            TxtOverallCostPerSQFT.Text = (currentRawUnit.OverallCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
            TxtDeliveryCostPerSQFT.Text = (currentRawUnit.DeliveryCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
            TxtTotalSellPricePerSQFT.Text = (currentRawUnit.TotalSellPrice.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2")
        Else
            TxtLumberPerSQFT.Text = "0.00"
            TxtPlatePerSQFT.Text = "0.00"
            TxtBDFTPerSQFT.Text = "0.00"
            TxtManufLaborPerSQFT.Text = "0.00"
            TxtDesignLaborPerSQFT.Text = "0.00"
            TxtMGMTLaborPerSQFT.Text = "0.00"
            TxtJobSuppliesPerSQFT.Text = "0.00"
            TxtManHoursPerSQFT.Text = "0.00"
            TxtItemCostPerSQFT.Text = "0.00"
            TxtOverallCostPerSQFT.Text = "0.00"
            TxtDeliveryCostPerSQFT.Text = "0.00"
            TxtTotalSellPricePerSQFT.Text = "0.00"
        End If
    End Sub

    Private Sub FrmPSE_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            marginCache.Clear()
            Dim productTypes = dataAccess.GetProductTypes()
            For Each pt In productTypes
                marginCache(pt.ProductTypeID) = GetEffectiveMargin(pt.ProductTypeID, -1) ' Pre-cache
            Next
            ' Initial filter if default level, else clear
            If TreeViewLevels.Nodes.Count > 0 Then
                TreeViewLevels.SelectedNode = TreeViewLevels.Nodes(0).Nodes(0).Nodes(0) ' Assume first level
            End If
            UpdateLevelTotals()
            UpdateSelectedRawPreview()
        Catch ex As Exception
            MessageBox.Show("Error initializing form: " & ex.Message)
        End Try
    End Sub

    Private Function CreateAndSaveActualUnit() As ActualUnitModel
        Dim sqft As Decimal
        Dim adder As Decimal
        If String.IsNullOrEmpty(TxtUnitName.Text) OrElse Not Decimal.TryParse(TxtPlanSQFT.Text, sqft) OrElse sqft <= 0 OrElse
           Not Decimal.TryParse(TxtOptionalAdder.Text, adder) OrElse adder < 1 Then
            MessageBox.Show("Invalid input—Name, SQFT (>0), and Adder (≥1) must be valid.")
            Return Nothing
        End If

        Dim rawUnit As RawUnitModel = rawUnits.FirstOrDefault(Function(r) r.RawUnitID = selectedRawUnitID)
        If rawUnit Is Nothing Then
            MessageBox.Show("Referenced Raw Unit not found.")
            Return Nothing
        End If

        Dim actualUnit As New ActualUnitModel With {
            .ProjectID = selectedProjectID,
            .RawUnitID = rawUnit.RawUnitID,
            .ProductTypeID = rawUnit.ProductTypeID,
            .UnitName = TxtUnitName.Text,
            .PlanSQFT = sqft,
            .UnitType = If(CmbUnitType.SelectedItem?.ToString(), "Res"),
            .OptionalAdder = adder,
            .MarginPercent = GetEffectiveMargin(rawUnit.ProductTypeID, rawUnit.RawUnitID)
        }

        dataAccess.SaveActualUnit(actualUnit)
        actualUnit.CalculatedComponents = CalculateComponentsFromRaw(rawUnit)
        dataAccess.SaveCalculatedComponents(actualUnit)

        Return actualUnit
    End Function

    Private Function CalculateComponentsFromRaw(rawUnit As RawUnitModel) As List(Of CalculatedComponentModel)
        Dim components As New List(Of CalculatedComponentModel)
        Dim sqFt As Decimal = rawUnit.SqFt.GetValueOrDefault()
        If sqFt <= 0 Then Return components

        Dim productTypeID As Integer = rawUnit.ProductTypeID
        Dim lumberAdder As Decimal = dataAccess.GetLumberAdder(selectedProjectID, productTypeID)
        Dim marginPercent As Decimal = GetEffectiveMargin(productTypeID, rawUnit.RawUnitID)

        Dim lfPerSqft As Decimal = rawUnit.LF.GetValueOrDefault() / sqFt
        Dim bdftPerSqft As Decimal = rawUnit.BF.GetValueOrDefault() / sqFt
        Dim lumberPerSqft As Decimal = rawUnit.LumberCost.GetValueOrDefault() / sqFt
        Dim platePerSqft As Decimal = rawUnit.PlateCost.GetValueOrDefault() / sqFt
        Dim manufLaborPerSqft As Decimal = rawUnit.ManufLaborCost.GetValueOrDefault() / sqFt
        Dim designLaborPerSqft As Decimal = rawUnit.DesignLabor.GetValueOrDefault() / sqFt
        Dim mgmtLaborPerSqft As Decimal = rawUnit.MGMTLabor.GetValueOrDefault() / sqFt
        Dim jobSuppliesPerSqft As Decimal = rawUnit.JobSuppliesCost.GetValueOrDefault() / sqFt
        Dim manHoursPerSqft As Decimal = rawUnit.ManHours.GetValueOrDefault() / sqFt
        Dim itemCostPerSqft As Decimal = rawUnit.ItemCost.GetValueOrDefault() / sqFt

        Dim overallCostPerSqft As Decimal = lumberPerSqft + platePerSqft + manufLaborPerSqft + designLaborPerSqft + mgmtLaborPerSqft + jobSuppliesPerSqft + itemCostPerSqft
        Dim sellPerSqft As Decimal = If(marginPercent >= 1D, 0D, overallCostPerSqft / (1D - marginPercent))
        Dim marginPerSqft As Decimal = sellPerSqft - overallCostPerSqft

        components.Add(New CalculatedComponentModel With {.ComponentType = "LF/SQFT", .Value = lfPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "BDFT/SQFT", .Value = bdftPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "Lumber/SQFT", .Value = lumberPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "Plate/SQFT", .Value = platePerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "ManufLabor/SQFT", .Value = manufLaborPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "DesignLabor/SQFT", .Value = designLaborPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "MGMTLabor/SQFT", .Value = mgmtLaborPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "JobSupplies/SQFT", .Value = jobSuppliesPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "ManHours/SQFT", .Value = manHoursPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "ItemCost/SQFT", .Value = itemCostPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "OverallCost/SQFT", .Value = overallCostPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "SellPrice/SQFT", .Value = sellPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "Margin/SQFT", .Value = marginPerSqft})

        Return components
    End Function

    Private Sub CalculateDerivedFields(displayUnit As DisplayUnitData, components As List(Of CalculatedComponentModel))
        If components Is Nothing Then Return

        Dim componentDict As New Dictionary(Of String, Decimal)
        For Each comp In components
            componentDict(comp.ComponentType) = comp.Value
        Next

        Dim value As Decimal
        displayUnit.LF = If(componentDict.TryGetValue("LF/SQFT", value), value, 0D) * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
        displayUnit.BDFT = If(componentDict.TryGetValue("BDFT/SQFT", value), value, 0D) * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
        displayUnit.LumberCost = If(componentDict.TryGetValue("Lumber/SQFT", value), value, 0D) * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
        displayUnit.PlateCost = If(componentDict.TryGetValue("Plate/SQFT", value), value, 0D) * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
        displayUnit.ManufLaborCost = If(componentDict.TryGetValue("ManufLabor/SQFT", value), value, 0D) * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
        displayUnit.DesignLabor = If(componentDict.TryGetValue("DesignLabor/SQFT", value), value, 0D) * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
        displayUnit.MGMTLabor = If(componentDict.TryGetValue("MGMTLabor/SQFT", value), value, 0D) * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
        displayUnit.JobSuppliesCost = If(componentDict.TryGetValue("JobSupplies/SQFT", value), value, 0D) * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
        displayUnit.ManHours = If(componentDict.TryGetValue("ManHours/SQFT", value), value, 0D) * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
        displayUnit.ItemCost = If(componentDict.TryGetValue("ItemCost/SQFT", value), value, 0D) * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
        displayUnit.OverallCost = If(componentDict.TryGetValue("OverallCost/SQFT", value), value, 0D) * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
    End Sub

    Private Sub BtnImportLevelData_Click(sender As Object, e As EventArgs) Handles BtnImportLevelData.Click
        Dim ofd As New OpenFileDialog With {
            .Filter = "CSV Files|*.csv"
        }
        If ofd.ShowDialog() = DialogResult.OK Then
            isImporting = True
            Try
                Dim productTypeID As Integer = 1 ' Default; CSV overrides
                dataAccess.ImportRawUnits(selectedProjectID, ofd.FileName, productTypeID)
                LblStatus.Text = "Import successful. Refreshing units..."
                If selectedLevelID <> -1 Then
                    Dim levelTag = DirectCast(TreeViewLevels.SelectedNode.Tag, Dictionary(Of String, Object))
                    productTypeID = CInt(levelTag("ProductTypeID"))
                    FilterRawUnits(productTypeID)
                End If
            Catch ex As Exception
                MessageBox.Show("Import failed: " & ex.Message)
            Finally
                isImporting = False
            End Try
        End If
    End Sub

    Private Sub BtnConvertToActualUnit_Click(sender As Object, e As EventArgs) Handles BtnConvertToActualUnit.Click
        isReusing = False
        btnSaveNewOnly.Visible = True
        If ListBoxAvailableUnits.SelectedIndex >= 0 Then
            Dim selectedRawUnitName As String = ListBoxAvailableUnits.SelectedItem.ToString()
            Dim selectedRawUnit As RawUnitModel = rawUnits.FirstOrDefault(Function(r) r.RawUnitName = selectedRawUnitName)
            If selectedRawUnit IsNot Nothing Then
                selectedRawUnitID = selectedRawUnit.RawUnitID
                PopulateDetailsPane(New ActualUnitModel With {.ReferencedRawUnitName = selectedRawUnit.RawUnitName, .PlanSQFT = selectedRawUnit.SqFt.GetValueOrDefault(), .OptionalAdder = 1D, .UnitType = "Res"}, False, False)
                LblStatus.Text = $"Status: Ready to create new actual unit from {selectedRawUnit.RawUnitName}."
            End If
        Else
            MessageBox.Show("Please select a raw unit from the list.")
        End If
    End Sub

    Private Sub BtnReuseActualUnit_Click(sender As Object, e As EventArgs) Handles btnReuseActualUnit.Click
        btnSaveNewOnly.Visible = False
        If ListboxExistingActualUnits.SelectedIndex >= 0 Then
            isReusing = True
            Dim selectedActual As ActualUnitModel = existingActualUnits(ListboxExistingActualUnits.SelectedIndex)
            PopulateDetailsPane(selectedActual, True, True)
            LblStatus.Text = $"Status: Ready to reuse {selectedActual.UnitName}. Update quantity and save."
        Else
            MessageBox.Show("Please select an existing actual unit from the list.")
        End If
    End Sub

    Private Sub PopulateDetailsPane(actualUnit As ActualUnitModel, isEdit As Boolean, isReuse As Boolean, Optional quantity As Integer = 1, Optional mappingID As Integer = -1)
        PanelDetails.Visible = True
        BtnToggleDetails.Text = "Hide Details"

        TxtUnitName.Text = actualUnit.UnitName
        TxtReferencedRawUnit.Text = actualUnit.ReferencedRawUnitName
        TxtPlanSQFT.Text = actualUnit.PlanSQFT.ToString("F2")
        TxtOptionalAdder.Text = actualUnit.OptionalAdder.ToString("F2")
        CmbUnitType.SelectedItem = actualUnit.UnitType
        ' Set quantity based on mode: edit uses provided quantity, reuse defaults to 1, new uses 1
        TxtActualUnitQuantity.Text = If(isEdit AndAlso Not isReuse, quantity.ToString(), "1")

        ' Store IDs for save operations
        selectedActualUnitID = actualUnit.ActualUnitID
        selectedMappingID = mappingID

        ' Read-only controls based on mode
        TxtUnitName.ReadOnly = isReuse OrElse isEdit
        TxtPlanSQFT.ReadOnly = isReuse OrElse isEdit
        TxtOptionalAdder.ReadOnly = isReuse OrElse isEdit
        CmbUnitType.Enabled = Not (isReuse OrElse isEdit)

        ' Dynamically set save button text based on mode
        BtnSaveAndAttach.Text = If(isEdit AndAlso Not isReuse, "Save Unit", "Save and Attach")

        ' Update per-SQFT from components or raw
        If actualUnit.CalculatedComponents IsNot Nothing AndAlso actualUnit.CalculatedComponents.Any() Then
            UpdatePerSQFTFields(actualUnit.CalculatedComponents)
        ElseIf actualUnit.RawUnitID > 0 Then
            Dim rawUnit = dataAccess.GetRawUnitByID(actualUnit.RawUnitID)
            If rawUnit IsNot Nothing Then UpdatePerSQFTFields(CalculateComponentsFromRaw(rawUnit))
        End If

        btnDeleteUnit.Visible = isEdit
    End Sub

    Private Sub UpdatePerSQFTFields(components As List(Of CalculatedComponentModel))
        Dim componentDict As New Dictionary(Of String, Decimal)
        For Each comp In components
            componentDict(comp.ComponentType) = comp.Value
        Next

        Dim value As Decimal
        TxtLumberPerSQFT.Text = If(componentDict.TryGetValue("Lumber/SQFT", value), value, 0D).ToString("F2")
        TxtPlatePerSQFT.Text = If(componentDict.TryGetValue("Plate/SQFT", value), value, 0D).ToString("F2")
        TxtBDFTPerSQFT.Text = If(componentDict.TryGetValue("BDFT/SQFT", value), value, 0D).ToString("F2")
        TxtManufLaborPerSQFT.Text = If(componentDict.TryGetValue("ManufLabor/SQFT", value), value, 0D).ToString("F2")
        TxtDesignLaborPerSQFT.Text = If(componentDict.TryGetValue("DesignLabor/SQFT", value), value, 0D).ToString("F2")
        TxtMGMTLaborPerSQFT.Text = If(componentDict.TryGetValue("MGMTLabor/SQFT", value), value, 0D).ToString("F2")
        TxtJobSuppliesPerSQFT.Text = If(componentDict.TryGetValue("JobSupplies/SQFT", value), value, 0D).ToString("F2")
        TxtManHoursPerSQFT.Text = If(componentDict.TryGetValue("ManHours/SQFT", value), value, 0D).ToString("F2")
        TxtItemCostPerSQFT.Text = If(componentDict.TryGetValue("ItemCost/SQFT", value), value, 0D).ToString("F2")
        TxtOverallCostPerSQFT.Text = If(componentDict.TryGetValue("OverallCost/SQFT", value), value, 0D).ToString("F2")
        TxtDeliveryCostPerSQFT.Text = If(componentDict.TryGetValue("DeliveryCost/SQFT", value), value, 0D).ToString("F2")
        TxtTotalSellPricePerSQFT.Text = If(componentDict.TryGetValue("SellPrice/SQFT", value), value, 0D).ToString("F2")
    End Sub

    Private Sub BtnSaveAndAttach_Click(sender As Object, e As EventArgs) Handles BtnSaveAndAttach.Click
        If String.IsNullOrEmpty(TxtUnitName.Text) Then
            MessageBox.Show("Actual Unit Name must be valid.")
            Return
        End If

        Dim qty As Integer
        If Not Integer.TryParse(TxtActualUnitQuantity.Text, qty) OrElse qty <= 0 Then
            MessageBox.Show("Invalid quantity—must be a positive integer.")
            Return
        End If

        Try
            If isReusing Then
                HandleReuseMapping(qty)
            ElseIf btnDeleteUnit.Visible Then ' isEdit mode (button visible only on edit)
                HandleEditMapping(qty)
            Else
                HandleNewUnit(qty)
            End If
            LoadAssignedUnits()
            dataAccess.UpdateLevelRollups(selectedLevelID)
            If selectedBuildingID > 0 Then dataAccess.UpdateBuildingRollups(selectedBuildingID)
            ResetDetailsPane()
        Catch ex As Exception
            MessageBox.Show("Error saving unit: " & ex.Message)
        End Try
    End Sub

    Private Sub HandleEditMapping(qty As Integer)
        If selectedMappingID <= 0 Then
            MessageBox.Show("Error: No valid mapping selected for update.")
            Return
        End If

        Dim mapping As New ActualToLevelMappingModel With {
        .MappingID = selectedMappingID,
        .ProjectID = selectedProjectID,
        .ActualUnitID = selectedActualUnitID,
        .LevelID = selectedLevelID,
        .Quantity = qty
    }
        dataAccess.SaveActualToLevelMapping(mapping)
        LblStatus.Text = "Unit quantity updated successfully—no variations introduced."
    End Sub

    Private Sub HandleNewUnit(qty As Integer)
        Dim actualUnit As ActualUnitModel = CreateAndSaveActualUnit()
        If actualUnit Is Nothing Then Return

        Dim mapping As New ActualToLevelMappingModel With {
        .ProjectID = selectedProjectID,
        .ActualUnitID = actualUnit.ActualUnitID,
        .LevelID = selectedLevelID,
        .Quantity = qty
    }
        dataAccess.SaveActualToLevelMapping(mapping)

        LblStatus.Text = "Unit saved and attached successfully—consistent data maintained across levels."
    End Sub

    Private Sub HandleReuseMapping(qty As Integer)
        Dim mapping As New ActualToLevelMappingModel With {
        .ProjectID = selectedProjectID,
        .ActualUnitID = selectedActualUnitID,
        .LevelID = selectedLevelID,
        .Quantity = qty
    }
        dataAccess.SaveActualToLevelMapping(mapping)
        LblStatus.Text = "Existing unit reused and mapped successfully—no variations introduced."
    End Sub

    Private Sub BtnDeleteUnit_Click(sender As Object, e As EventArgs) Handles btnDeleteUnit.Click
        If DataGridViewAssigned.CurrentRow Is Nothing Then Return

        Dim index As Integer = DataGridViewAssigned.CurrentRow.Index
        If index < 0 OrElse index >= displayUnits.Count Then Return

        If MessageBox.Show("Remove this unit from the level?", "Confirm Delete", MessageBoxButtons.YesNo) = DialogResult.Yes Then
            Try
                Dim mappingID As Integer = displayUnits(index).MappingID
                dataAccess.DeleteActualToLevelMapping(mappingID)
                LoadAssignedUnits()
                dataAccess.UpdateLevelRollups(selectedLevelID)
                If selectedBuildingID > 0 Then dataAccess.UpdateBuildingRollups(selectedBuildingID)
                ResetDetailsPane()
                LblStatus.Text = "Unit mapping removed successfully—ActualUnit preserved for reuse."
            Catch ex As Exception
                MessageBox.Show("Error removing unit mapping: " & ex.Message)
            End Try
        End If
    End Sub

    Private Sub ResetDetailsPane()
        TxtUnitName.Text = ""
        TxtPlanSQFT.Text = ""
        TxtActualUnitQuantity.Text = "1"
        TxtOptionalAdder.Text = "1.0"
        CmbUnitType.SelectedIndex = -1
        PanelDetails.Visible = False
        btnDeleteUnit.Visible = False
    End Sub

    Private Sub BtnRecalculate_Click(sender As Object, e As EventArgs) Handles BtnRecalculate.Click
        Try
            LoadAssignedUnits()
            dataAccess.UpdateLevelRollups(selectedLevelID)
            If selectedBuildingID > 0 Then dataAccess.UpdateBuildingRollups(selectedBuildingID)
            MessageBox.Show("Recalculated totals successfully.")
        Catch ex As Exception
            MessageBox.Show("Error recalculating: " & ex.Message)
        End Try
    End Sub

    Private Sub DataGridViewAssigned_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewAssigned.CellContentClick
        If e.ColumnIndex = colActions.Index AndAlso e.RowIndex >= 0 Then
            Dim displayUnit = displayUnits(e.RowIndex)
            PopulateDetailsPane(displayUnit.ActualUnit, True, False, displayUnit.ActualUnitQuantity, displayUnit.MappingID)
        End If
    End Sub

    Private Sub LoadExistingActualUnits()
        ListboxExistingActualUnits.Items.Clear()
        existingActualUnits = dataAccess.GetActualUnitsByProject(selectedProjectID)
        For Each actual In existingActualUnits
            ListboxExistingActualUnits.Items.Add(actual.UnitName & " (" & actual.UnitType & ")")
        Next
    End Sub

    Private Sub ListBoxAvailableUnits_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBoxAvailableUnits.SelectedIndexChanged
        UpdateSelectedRawPreview()
    End Sub

    Private Sub mnuCopyUnits_Click(sender As Object, e As EventArgs) Handles mnuCopyUnits.Click
        If selectedLevelID <= 0 Then Return

        Dim mappings = dataAccess.GetActualToLevelMappingsByLevelID(selectedLevelID)
        copiedMappings.Clear()
        For Each mapping In mappings
            copiedMappings.Add(New Tuple(Of Integer, Integer)(mapping.ActualUnitID, mapping.Quantity))
        Next

        Dim levelTag = DirectCast(TreeViewLevels.SelectedNode.Tag, Dictionary(Of String, Object))
        sourceProductTypeID = CInt(levelTag("ProductTypeID"))

        LblStatus.Text = $"Copied {copiedMappings.Count} units from level. Ready to paste."
    End Sub

    Private Sub mnuPasteUnits_Click(sender As Object, e As EventArgs) Handles mnuPasteUnits.Click
        If selectedLevelID <= 0 OrElse copiedMappings.Count = 0 Then Return

        Dim levelTag = DirectCast(TreeViewLevels.SelectedNode.Tag, Dictionary(Of String, Object))
        Dim targetProductTypeID As Integer = CInt(levelTag("ProductTypeID"))
        If targetProductTypeID <> sourceProductTypeID Then
            MessageBox.Show("Cannot paste: Source and target levels must have the same Product Type.")
            Return
        End If

        Dim existingMappings = dataAccess.GetActualToLevelMappingsByLevelID(selectedLevelID)
        For Each copied In copiedMappings
            Dim actualUnitID As Integer = copied.Item1
            Dim quantity As Integer = copied.Item2

            Dim existing = existingMappings.FirstOrDefault(Function(m) m.ActualUnitID = actualUnitID)
            If existing IsNot Nothing Then
                existing.Quantity += quantity
                dataAccess.SaveActualToLevelMapping(existing)
            Else
                Dim newMapping As New ActualToLevelMappingModel With {
                    .ProjectID = selectedProjectID,
                    .actualUnitID = actualUnitID,
                    .LevelID = selectedLevelID,
                    .quantity = quantity
                }
                dataAccess.SaveActualToLevelMapping(newMapping)
            End If
        Next

        LoadAssignedUnits()
        dataAccess.UpdateLevelRollups(selectedLevelID)
        If selectedBuildingID > 0 Then dataAccess.UpdateBuildingRollups(selectedBuildingID)
        copiedMappings.Clear()
        LblStatus.Text = "Units pasted successfully. Rollups updated."
    End Sub

    Private Sub BtnToggleDetails_Click(sender As Object, e As EventArgs) Handles BtnToggleDetails.Click
        PanelDetails.Visible = Not PanelDetails.Visible
        BtnToggleDetails.Text = If(PanelDetails.Visible, "Hide Details", "Show Details")
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        ResetDetailsPane()
    End Sub

    Private Sub BtnFinish_Click(sender As Object, e As EventArgs) Handles BtnFinish.Click
        LblStatus.Text = $"Last Saved: {DateTime.Now} - Sandbox session complete."
        Me.Close()
    End Sub

    Private Sub FrmPSE_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        copiedMappings.Clear()
    End Sub

End Class