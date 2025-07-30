Option Strict On
Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.DataAccess

Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.BuildersPSE.Utilities
Imports Microsoft.VisualBasic.FileIO

Public Class FrmPSE
    Inherits Form

    Private marginCache As New Dictionary(Of Integer, Decimal)

    ' Data Tracking
    Private selectedLevelID As Integer = -1
    Private selectedProjectID As Integer = -1
    Private currentUnitIndex As Integer = -1
    Private selectedRawUnitID As Integer = -1 ' Track selected RawUnit for database
    Private assignedUnits As New List(Of ActualUnitModel) ' Use ActualUnitModel
    Private displayUnits As New List(Of DisplayUnitData) ' For UI binding
    Private bindingSource As New BindingSource
    Private rawUnits As New List(Of RawUnitModel) ' Use RawUnitModel
    Private dataAccess As New DataAccess
    Private isImporting As Boolean = False ' Flag to prevent dialog looping
    Private isReusing As Boolean = False  ' Flag: True for reuse, False for new
    Private selectedActualUnitID As Integer = -1  ' Track selected ActualUnit for reuse
    Private existingActualUnits As New List(Of ActualUnitModel)  ' Cache for project

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
    Private colSellPrice As DataGridViewColumn
    Private colMargin As DataGridViewColumn
    Private colActions As DataGridViewColumn
    Private colDeliveryCost As DataGridViewColumn

    Private copiedMappings As New List(Of Tuple(Of Integer, Integer)) ' (ActualUnitID, Quantity)
    Private sourceProductTypeID As Integer = -1 ' For compatibility check on paste


    ' Temporary class for UI binding
    Private Class DisplayUnitData
        Public Property ActualUnitID As Integer
        Public Property MappingID As Integer
        Public Property UnitName As String
        Public Property ReferencedRawUnitName As String
        Public Property PlanSQFT As Decimal
        Public Property ActualUnitQuantity As Integer
        Public Property OptionalAdder As Decimal
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
        Public Property SellPrice As Decimal
        Public Property Margin As Decimal
        Public Property DeliveryCost As Decimal

    End Class

    Public Sub New(projectID As Integer)
        InitializeComponent()
        selectedProjectID = projectID
        LoadLevelHierarchy() ' Project-specific tree
        SetupUI()
        LoadExistingActualUnits()
    End Sub

    Private Sub SetupUI()
        ' Populate CmbUnitType
        CmbUnitType.Items.Clear()
        CmbUnitType.Items.AddRange(New Object() {"Res", "NonRes"})
        ' Bind DataGridView
        DataGridViewAssigned.AutoGenerateColumns = False
        bindingSource.DataSource = GetType(DisplayUnitData)
        DataGridViewAssigned.DataSource = bindingSource
        AddCalculatedColumns()
        UpdateTotals()
        ' Ensure base view is default
        ChkDetailedView.Checked = False ' Assume CheckBox added in designer
        ToggleGridView()
    End Sub

    Private Sub AddCalculatedColumns()
        DataGridViewAssigned.Columns.Clear()

        ' Define columns with references
        colActualUnitID = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "ActualUnitID",
        .HeaderText = "Actual Unit ID",
        .Visible = False ' Always hidden
    }
        DataGridViewAssigned.Columns.Add(colActualUnitID)

        colMappingID = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "MappingID",
        .HeaderText = "Mapping ID",
        .Visible = False ' Always hidden
    }
        DataGridViewAssigned.Columns.Add(colMappingID)

        colUnitName = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "UnitName",
        .HeaderText = "Actual Unit Name",
        .ReadOnly = True,
        .Visible = True ' Base view
    }
        DataGridViewAssigned.Columns.Add(colUnitName)

        colReferencedRawUnitName = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "ReferencedRawUnitName",
        .HeaderText = "Referenced Raw Unit Name",
        .ReadOnly = True,
        .Visible = True ' Base view
    }
        DataGridViewAssigned.Columns.Add(colReferencedRawUnitName)

        colPlanSQFT = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "PlanSQFT",
        .HeaderText = "Plan SQFT",
        .ReadOnly = True,
        .Visible = True ' Base view
    }
        DataGridViewAssigned.Columns.Add(colPlanSQFT)

        colActualUnitQuantity = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "ActualUnitQuantity",
        .HeaderText = "Actual Unit Quantity",
        .ReadOnly = True,
        .Visible = True ' Base view
    }
        DataGridViewAssigned.Columns.Add(colActualUnitQuantity)

        colLF = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "LF",
        .HeaderText = "LF",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colLF)

        colBDFT = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "BDFT",
        .HeaderText = "BDFT",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colBDFT)

        colLumberCost = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "LumberCost",
        .HeaderText = "Lumber $$",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colLumberCost)

        colPlateCost = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "PlateCost",
        .HeaderText = "Plate $$",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colPlateCost)

        colManufLaborCost = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "ManufLaborCost",
        .HeaderText = "Manuf Labor $$",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colManufLaborCost)

        colDesignLabor = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "DesignLabor",
        .HeaderText = "Design Labor $$",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colDesignLabor)

        colMGMTLabor = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "MGMTLabor",
        .HeaderText = "MGMT Labor $$",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colMGMTLabor)

        colJobSuppliesCost = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "JobSuppliesCost",
        .HeaderText = "Job Supplies $$",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colJobSuppliesCost)

        colManHours = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "ManHours",
        .HeaderText = "Man Hours",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colManHours)

        colItemCost = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "ItemCost",
        .HeaderText = "Item $$",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colItemCost)

        colDeliveryCost = New DataGridViewTextBoxColumn With {
            .DataPropertyName = "DeliveryCost",
            .HeaderText = "Delivery $$",
            .ReadOnly = True,
            .Visible = False  ' Detailed view only
}
        DataGridViewAssigned.Columns.Add(colDeliveryCost)

        colOverallCost = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "OverallCost",
        .HeaderText = "Overall Cost",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colOverallCost)

        colSellPrice = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "SellPrice",
        .HeaderText = "Sell Price",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colSellPrice)

        colMargin = New DataGridViewTextBoxColumn With {
        .DataPropertyName = "Margin",
        .HeaderText = "Margin $$",
        .ReadOnly = True,
        .Visible = False ' Detailed view only
    }
        DataGridViewAssigned.Columns.Add(colMargin)

        colActions = New DataGridViewButtonColumn With {
        .Name = "ActionsColumn",
        .HeaderText = "Actions",
        .Text = "Edit",
        .UseColumnTextForButtonValue = True,
        .Visible = True ' Base view
    }
        DataGridViewAssigned.Columns.Add(colActions)
    End Sub
    Private Sub ToggleGridView()
        Dim isDetailedView As Boolean = ChkDetailedView.Checked
        ' Base view: only UnitName, ReferencedRawUnitName, PlanSQFT, ActualUnitQuantity, Actions
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
    Private Sub chkDetailedView_CheckedChanged(sender As Object, e As EventArgs) Handles ChkDetailedView.CheckedChanged
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
                    Dim levelNode As TreeNode = bldgNode.Nodes.Add(lvl.ProductTypeName & " Level " & lvl.LevelNumber) ' Use ProductTypeName
                    levelNode.Tag = New Dictionary(Of String, Object) From {{"Type", "Level"}, {"ID", lvl.LevelID}, {"ProductTypeID", lvl.ProductTypeID}}
                Next
            Next
            projectNode.Expand()
        Else
            LblStatus.Text = "Status: Project not found."
        End If
    End Sub
    Private Function CreateAndSaveActualUnit() As ActualUnitModel
        Dim sqft As Decimal
        Dim adder As Decimal
        If String.IsNullOrEmpty(TxtUnitName.Text) OrElse Not Decimal.TryParse(TxtPlanSQFT.Text, sqft) OrElse sqft <= 0 OrElse
       Not Decimal.TryParse(TxtOptionalAdder.Text, adder) OrElse adder < 1 Then
            MessageBox.Show("Invalid input—Name, SQFT (>0), and Adder (≥1) must be valid.")
            Return Nothing
        End If

        Dim rawUnit As RawUnitModel = rawUnits.Find(Function(r) r.RawUnitID = selectedRawUnitID)
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
        .OptionalAdder = adder
    }

        ' Calculate margin for storage
        Dim marginPercent As Decimal = dataAccess.GetMarginPercent(selectedProjectID, rawUnit.ProductTypeID)
        If marginPercent = 0D Then
            If rawUnit.TotalSellPrice.GetValueOrDefault() > 0D AndAlso rawUnit.OverallCost.GetValueOrDefault() > 0D Then
                marginPercent = (rawUnit.TotalSellPrice.Value - rawUnit.OverallCost.Value) / rawUnit.TotalSellPrice.Value
            End If
        End If
        actualUnit.MarginPercent = marginPercent

        dataAccess.SaveActualUnit(actualUnit)  ' Saves with MarginPercent

        actualUnit.CalculatedComponents = CalculateComponentsFromRaw(rawUnit)
        dataAccess.SaveCalculatedComponents(actualUnit)

        Return actualUnit
    End Function
    Private Sub TreeViewLevels_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeViewLevels.AfterSelect
        If e.Node.Level = 2 Then ' Level node
            Dim levelTag = DirectCast(e.Node.Tag, Dictionary(Of String, Object))
            selectedLevelID = CInt(levelTag("ID"))
            Dim productTypeID As Integer = CInt(levelTag("ProductTypeID"))
            FilterRawUnits(productTypeID)
            LoadAssignedUnits()
            UpdateTotals()
        Else
            ListBoxAvailableUnits.Items.Clear()
            assignedUnits.Clear()
            displayUnits.Clear()
            bindingSource.DataSource = Nothing
            UpdateTotals()
        End If
    End Sub

    Private Sub FilterRawUnits(productTypeID As Integer)
        ListBoxAvailableUnits.Items.Clear()
        rawUnits = dataAccess.GetRawUnitsByProjectID(selectedProjectID).Where(Function(r) r.ProductTypeID = productTypeID).ToList()
        For Each rawUnit In rawUnits
            ListBoxAvailableUnits.Items.Add(rawUnit.RawUnitName)
        Next
    End Sub

    Private Sub UpdateTotals()
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
        TxtTotalOverallCost.Text = displayUnits.Sum(Function(u) u.OverallCost).ToString("F2")  ' Excludes DeliveryCost
        TxtTotalSellPrice.Text = displayUnits.Sum(Function(u) u.SellPrice).ToString("F2")  ' Includes DeliveryCost
        TxtTotalMargin.Text = displayUnits.Sum(Function(u) u.Margin).ToString("F2")
        txtTotalDeliveryCost.Text = displayUnits.Sum(Function(u) u.DeliveryCost).ToString("F2")
        Dim currentRawUnit = If(ListBoxAvailableUnits.SelectedIndex >= 0, rawUnits.Find(Function(r) r.RawUnitName = ListBoxAvailableUnits.Items(ListBoxAvailableUnits.SelectedIndex).ToString()), Nothing)
        If currentRawUnit IsNot Nothing Then
            TxtLumberPerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.LumberCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
            TxtPlatePerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.PlateCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
            TxtBDFTPerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.BF.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
            TxtManufLaborPerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.ManufLaborCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
            TxtDesignLaborPerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.DesignLabor.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
            TxtMGMTLaborPerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.MGMTLabor.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
            TxtJobSuppliesPerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.JobSuppliesCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
            TxtManHoursPerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.ManHours.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
            TxtItemCostPerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.ItemCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
            TxtOverallCostPerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.OverallCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
            TxtDeliveryCostPerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.DeliveryCost.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
            TxtTotalSellPricePerSQFT.Text = If(currentRawUnit.SqFt.HasValue AndAlso currentRawUnit.SqFt.Value > 0, (currentRawUnit.TotalSellPrice.GetValueOrDefault() / currentRawUnit.SqFt.Value).ToString("F2"), "0.00")
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
        LblStatus.Text = $"Total Adjusted SQFT: {totalSQFT} - Recalc complete. No variations detected... yet."
    End Sub

    Private Sub FrmPSE_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        marginCache.Clear()
        Dim productTypes = dataAccess.GetProductTypes()  ' Assume method added to fetch distinct types for project
        For Each pt In productTypes
            Dim margin = dataAccess.GetMarginPercent(selectedProjectID, pt.ProductTypeID)
            If margin = 0D Then
                ' Project-wide raw fallback avg (query RawUnits for project)
                Dim avgRawMarginQuery As String = "SELECT AVG( (TotalSellPrice - OverallCost) / TotalSellPrice ) FROM RawUnits WHERE ProjectID = @ProjectID AND ProductTypeID = @ProductTypeID AND TotalSellPrice > 0"
                Dim params As SqlParameter() = {New SqlParameter("@ProjectID", selectedProjectID), New SqlParameter("@ProductTypeID", pt.ProductTypeID)}
                Dim avgObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(avgRawMarginQuery, params)
                margin = If(avgObj Is DBNull.Value, 0D, CDec(avgObj))
            End If
            marginCache(pt.ProductTypeID) = margin
        Next

        ' Ensure fresh load with no data except project details
        rawUnits.Clear()
        assignedUnits.Clear()
        displayUnits.Clear()
        ListBoxAvailableUnits.Items.Clear()
        bindingSource.DataSource = Nothing
        UpdateTotals()
    End Sub

    Private Sub LoadAssignedUnits()
        Dim mappings = dataAccess.GetActualToLevelMappingsByLevelID(selectedLevelID)
        displayUnits.Clear()
        assignedUnits.Clear()
        For Each mapping In mappings
            assignedUnits.Add(mapping.ActualUnit)
            Dim unit As New DisplayUnitData With {
            .MappingID = mapping.MappingID,
            .ActualUnitID = mapping.ActualUnitID,
            .UnitName = mapping.ActualUnit.UnitName,
            .ReferencedRawUnitName = mapping.ActualUnit.ReferencedRawUnitName,
            .PlanSQFT = mapping.ActualUnit.PlanSQFT,
            .ActualUnitQuantity = mapping.Quantity,
            .OptionalAdder = mapping.ActualUnit.OptionalAdder
        }

            Dim components As List(Of CalculatedComponentModel) = mapping.ActualUnit.CalculatedComponents
            If components Is Nothing Then
                Debug.WriteLine("Missing components for ActualUnitID: " & mapping.ActualUnitID)
                Continue For
            End If

            CalculateDerivedFields(unit, components)

            ' Apply LumberAdder as difference between ProjectProductSettings.LumberAdder and RawUnit.AvgSPFNo2
            Dim productTypeID As Integer = mapping.ActualUnit.ProductTypeID
            Dim rawUnit = rawUnits.Find(Function(r) r.RawUnitID = mapping.ActualUnit.RawUnitID)
            If rawUnit Is Nothing Then
                Debug.WriteLine("Missing raw unit for ActualUnitID: " & mapping.ActualUnitID)
                Continue For
            End If
            Dim projectLumberAdder As Decimal = dataAccess.GetLumberAdder(selectedProjectID, productTypeID)
            Dim avgSPFNo2 As Decimal = rawUnit.AvgSPFNo2.GetValueOrDefault()
            Dim effectiveLumberAdder As Decimal = If(projectLumberAdder > 0, Math.Max(0, projectLumberAdder - avgSPFNo2), 0D)
            If effectiveLumberAdder > 0 Then
                Dim adderAmount As Decimal = (unit.BDFT / 1000D) * effectiveLumberAdder
                unit.LumberCost += adderAmount
                unit.OverallCost += adderAmount
            End If

            Dim marginPercent As Decimal = dataAccess.GetMarginPercent(selectedProjectID, productTypeID)
            If marginPercent = 0D Then
                ' Fallback to raw margin (per unit's RawUnit)
                Dim rawMarginUnit = rawUnits.Find(Function(r) r.RawUnitID = mapping.ActualUnit.RawUnitID)
                If rawMarginUnit IsNot Nothing Then
                    Dim rawCost As Decimal = rawMarginUnit.OverallCost.GetValueOrDefault()
                    Dim rawSell As Decimal = rawMarginUnit.TotalSellPrice.GetValueOrDefault()
                    If rawSell > 0D Then marginPercent = (rawSell - rawCost) / rawSell
                End If
            End If

            ' Sum costs from components
            Dim unitCost As Decimal = (GetComponentValue(components, "Lumber/SQFT") + GetComponentValue(components, "Plate/SQFT") +
                               GetComponentValue(components, "ManufLabor/SQFT") + GetComponentValue(components, "DesignLabor/SQFT") +
                               GetComponentValue(components, "MGMTLabor/SQFT") + GetComponentValue(components, "JobSupplies/SQFT") +
                               GetComponentValue(components, "ItemCost/SQFT")) *
                               unit.PlanSQFT * unit.ActualUnitQuantity * unit.OptionalAdder

            unit.SellPrice = If(marginPercent >= 1D, unit.OverallCost, unit.OverallCost / (1D - marginPercent))
            unit.Margin = unit.SellPrice - unit.OverallCost

            ' Calculate derived fields from CalculatedComponents (for display/reference only)
            Dim lfPerSqft = GetComponentValue(components, "LF/SQFT")
            unit.LF = lfPerSqft * unit.PlanSQFT * unit.ActualUnitQuantity * unit.OptionalAdder

            Dim bdftPerSqft = GetComponentValue(components, "BDFT/SQFT")
            unit.BDFT = bdftPerSqft * unit.PlanSQFT * unit.ActualUnitQuantity * unit.OptionalAdder

            Dim lumberPerSqft = GetComponentValue(components, "Lumber/SQFT")
            unit.LumberCost = lumberPerSqft * unit.PlanSQFT * unit.ActualUnitQuantity * unit.OptionalAdder

            Dim platePerSqft = GetComponentValue(components, "Plate/SQFT")
            unit.PlateCost = platePerSqft * unit.PlanSQFT * unit.ActualUnitQuantity * unit.OptionalAdder

            Dim manufLaborPerSqft = GetComponentValue(components, "ManufLabor/SQFT")
            unit.ManufLaborCost = manufLaborPerSqft * unit.PlanSQFT * unit.ActualUnitQuantity * unit.OptionalAdder

            Dim designLaborPerSqft = GetComponentValue(components, "DesignLabor/SQFT")
            unit.DesignLabor = designLaborPerSqft * unit.PlanSQFT * unit.ActualUnitQuantity * unit.OptionalAdder

            Dim mgmtLaborPerSqft = GetComponentValue(components, "MGMTLabor/SQFT")
            unit.MGMTLabor = mgmtLaborPerSqft * unit.PlanSQFT * unit.ActualUnitQuantity * unit.OptionalAdder

            Dim jobSuppliesPerSqft = GetComponentValue(components, "JobSupplies/SQFT")
            unit.JobSuppliesCost = jobSuppliesPerSqft * unit.PlanSQFT * unit.ActualUnitQuantity * unit.OptionalAdder

            Dim manHoursPerSqft = GetComponentValue(components, "ManHours/SQFT")
            unit.ManHours = manHoursPerSqft * unit.PlanSQFT * unit.ActualUnitQuantity * unit.OptionalAdder

            Dim itemCostPerSqft = GetComponentValue(components, "ItemCost/SQFT")
            unit.ItemCost = itemCostPerSqft * unit.PlanSQFT * unit.ActualUnitQuantity * unit.OptionalAdder

            displayUnits.Add(unit)
        Next
        ' Calculate level totals for proration
        Dim totalBDFT As Decimal = displayUnits.Sum(Function(u) u.BDFT)

        ' Fetch required values
        Dim mileageRate As Decimal = dataAccess.GetConfigValue("MileageRate")
        Dim milesToJobSite As Integer = dataAccess.GetMilesToJobSite(selectedProjectID)

        ' Calculate level delivery (matches Queries.CalculateDeliveryCost logic)
        Dim deliveryTotal As Decimal = 0D
        If totalBDFT > 0 Then
            Dim numLoads As Decimal = Math.Ceiling(totalBDFT / 10000D)
            deliveryTotal = numLoads * mileageRate * milesToJobSite
            deliveryTotal = Math.Round(deliveryTotal, 0)
            If deliveryTotal < 150 Then deliveryTotal = 150
        End If

        ' Prorate delivery and apply margin
        For Each unit In displayUnits
            Dim productTypeID As Integer = assignedUnits.First(Function(a) a.ActualUnitID = unit.ActualUnitID).ProductTypeID
            Dim marginPercent As Decimal = If(marginCache.ContainsKey(productTypeID), marginCache(productTypeID), dataAccess.GetMarginPercent(selectedProjectID, productTypeID))
            If marginPercent = 0D Then
                Dim rawUnit = rawUnits.Find(Function(r) r.RawUnitID = assignedUnits.First(Function(a) a.ActualUnitID = unit.ActualUnitID).RawUnitID)
                If rawUnit IsNot Nothing Then
                    Dim rawCost As Decimal = rawUnit.OverallCost.GetValueOrDefault()
                    Dim rawSell As Decimal = rawUnit.TotalSellPrice.GetValueOrDefault()
                    If rawSell > 0D Then marginPercent = (rawSell - rawCost) / rawSell
                End If
            End If

            If totalBDFT > 0 Then
                unit.DeliveryCost = (unit.BDFT / totalBDFT) * deliveryTotal
            Else
                unit.DeliveryCost = 0D
            End If
            unit.SellPrice += unit.DeliveryCost  ' Add to SellPrice post-margin
            unit.Margin = unit.SellPrice - unit.OverallCost  ' Recalculate Margin
        Next

        bindingSource.DataSource = displayUnits
        bindingSource.ResetBindings(False)
        DataGridViewAssigned.Refresh()
        UpdateTotals()
    End Sub

    Private Function GetComponentValue(components As List(Of CalculatedComponentModel), componentType As String) As Decimal
        Dim comp As CalculatedComponentModel = components.FirstOrDefault(Function(c) c.ComponentType = componentType)
        If comp IsNot Nothing Then
            Return comp.Value
        Else
            Return 0D
        End If
    End Function


    Private Sub BtnImportLevelData_Click(sender As Object, e As EventArgs) Handles BtnImportLevelData.Click
        Dim ofd As New OpenFileDialog With {
        .Filter = "CSV Files|*.csv"
    }
        If ofd.ShowDialog() = DialogResult.OK Then
            isImporting = True
            Try
                ' Use a default productTypeID (e.g., 1 for Floor), as CSV will override per row
                Dim productTypeID As Integer = 1  ' Default fallback; CSV "Product" column takes precedence

                dataAccess.ImportRawUnits(selectedProjectID, ofd.FileName, productTypeID)
                LblStatus.Text = "Import successful. Refreshing units..."
                ' Optional: If a level is selected, refresh its units; otherwise, skip or refresh globally
                'If selectedLevelID <> -1 Then
                '    Dim levelTag = DirectCast(TreeViewLevels.SelectedNode.Tag, Dictionary(Of String, Object))
                '    productTypeID = CInt(levelTag("ProductTypeID"))
                '    FilterRawUnits(productTypeID)
                'End If
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
            Dim selectedRawUnit As RawUnitModel = rawUnits.Find(Function(r) r.RawUnitName = selectedRawUnitName)
            If selectedRawUnit IsNot Nothing Then
                ' Assuming PanelDetails exists in the form for entering ActualUnit details
                PanelDetails.Visible = True
                BtnToggleDetails.Text = "Hide Details"
                currentUnitIndex = -1 ' New unit, not editing
                selectedRawUnitID = selectedRawUnit.RawUnitID

                ' Pre-fill fields for new ActualUnit
                TxtUnitName.Text = selectedRawUnit.RawUnitName & " - Actual"
                TxtReferencedRawUnit.Text = selectedRawUnit.RawUnitName
                TxtPlanSQFT.Text = selectedRawUnit.SqFt.GetValueOrDefault().ToString("F2")
                TxtActualUnitQuantity.Text = "1"
                TxtOptionalAdder.Text = "1.0"
                CmbUnitType.SelectedIndex = CmbUnitType.Items.IndexOf("Res")

                ' Display per-square-foot calculations for UI reference (not stored in ActualUnit)
                If selectedRawUnit.SqFt.GetValueOrDefault() > 0 Then
                    TxtLumberPerSQFT.Text = (selectedRawUnit.LumberCost.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                    TxtPlatePerSQFT.Text = (selectedRawUnit.PlateCost.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                    TxtBDFTPerSQFT.Text = (selectedRawUnit.BF.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                    TxtManufLaborPerSQFT.Text = (selectedRawUnit.ManufLaborCost.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                    TxtDesignLaborPerSQFT.Text = (selectedRawUnit.DesignLabor.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                    TxtMGMTLaborPerSQFT.Text = (selectedRawUnit.MGMTLabor.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                    TxtJobSuppliesPerSQFT.Text = (selectedRawUnit.JobSuppliesCost.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                    TxtManHoursPerSQFT.Text = (selectedRawUnit.ManHours.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                    TxtItemCostPerSQFT.Text = (selectedRawUnit.ItemCost.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                    TxtOverallCostPerSQFT.Text = (selectedRawUnit.OverallCost.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                    TxtDeliveryCostPerSQFT.Text = (selectedRawUnit.DeliveryCost.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                    TxtTotalSellPricePerSQFT.Text = (selectedRawUnit.TotalSellPrice.GetValueOrDefault() / selectedRawUnit.SqFt.GetValueOrDefault()).ToString("F2")
                Else
                    ' Reset to 0.00 if SqFt is 0 or null
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

                LblStatus.Text = $"Status: Ready to create new actual unit from {selectedRawUnit.RawUnitName}."
            Else
                MessageBox.Show("Selected raw unit not found.")
            End If
        Else
            MessageBox.Show("Please select a raw unit from the list.")
        End If
    End Sub

    Private Sub BtnSaveAndAttach_Click(sender As Object, e As EventArgs) Handles BtnSaveAndAttach.Click
        ' Validate UnitName (required in all modes)
        If String.IsNullOrEmpty(TxtUnitName.Text) Then
            MessageBox.Show("Actual Unit Name must be valid. Please check your entries.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If isReusing Then
            ' Reuse mode: Only quantity is editable; create/update mapping only
            Dim qty As Integer
            If Not Integer.TryParse(TxtActualUnitQuantity.Text, qty) OrElse qty <= 0 Then
                MessageBox.Show("Invalid quantity—must be a positive integer.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Create/update mapping
            Dim mapping As New ActualToLevelMappingModel With {
            .ProjectID = selectedProjectID,
            .ActualUnitID = selectedActualUnitID,
            .LevelID = selectedLevelID,
            .Quantity = qty
        }
            dataAccess.SaveActualToLevelMapping(mapping)  ' Inserts or updates

            ' Refresh UI and rollups
            LoadAssignedUnits()
            UpdateTotals()
            dataAccess.UpdateLevelRollups(selectedLevelID)
            Dim buildingID As Integer = dataAccess.GetBuildingIDByLevelID(selectedLevelID)
            If buildingID > 0 Then
                dataAccess.UpdateBuildingRollups(buildingID)
            Else
                MessageBox.Show("No building found for the selected level. Building rollup skipped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            UpdateTotals()  ' Ensure UI sync

            ' Reset pane and flag
            ResetDetailsPane()
            isReusing = False
            LblStatus.Text = "Existing unit reused and mapped successfully—no variations introduced."

        Else
            ' New or edit mode: Parse all fields
            Dim sqft As Decimal
            Dim adder As Decimal
            Dim qty As Integer
            If Not Decimal.TryParse(TxtPlanSQFT.Text, sqft) OrElse sqft <= 0 OrElse
           Not Decimal.TryParse(TxtOptionalAdder.Text, adder) OrElse adder < 1 OrElse
           Not Integer.TryParse(TxtActualUnitQuantity.Text, qty) OrElse qty <= 0 Then
                MessageBox.Show("Invalid input—SQFT (>0), Adder (≥1), and Quantity (>0) must be valid numbers.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Create or update ActualUnit
            Dim actualUnit As ActualUnitModel = CreateAndSaveActualUnit()
            If actualUnit Is Nothing Then Return  ' Validation failed in helper

            If currentUnitIndex = -1 Then
                ' New ActualUnit: Check for duplicates
                If Not assignedUnits.Any(Function(u) u.UnitName = actualUnit.UnitName AndAlso u.RawUnitID = actualUnit.RawUnitID) Then
                    ' Create mapping
                    Dim mapping As New ActualToLevelMappingModel With {
                    .ProjectID = selectedProjectID,
                    .ActualUnitID = actualUnit.ActualUnitID,
                    .LevelID = selectedLevelID,
                    .Quantity = qty
                }
                    dataAccess.SaveActualToLevelMapping(mapping)

                    ' Update UI
                    assignedUnits.Add(actualUnit)
                    Dim displayUnit As New DisplayUnitData With {
                    .ActualUnitID = actualUnit.ActualUnitID,
                    .MappingID = mapping.MappingID,
                    .UnitName = actualUnit.UnitName,
                    .ReferencedRawUnitName = rawUnits.Find(Function(r) r.RawUnitID = actualUnit.RawUnitID)?.RawUnitName,
                    .PlanSQFT = actualUnit.PlanSQFT,
                    .ActualUnitQuantity = qty,
                    .OptionalAdder = actualUnit.OptionalAdder
                }
                    CalculateDerivedFields(displayUnit, actualUnit.CalculatedComponents)
                    displayUnits.Add(displayUnit)
                    bindingSource.ResetBindings(False)
                Else
                    MessageBox.Show("Duplicate unit name and raw unit combination detected—preventing variation in data.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
            Else
                ' Edit existing ActualUnit
                actualUnit.ActualUnitID = assignedUnits(currentUnitIndex).ActualUnitID
                dataAccess.SaveActualUnit(actualUnit)
                Dim mappings = dataAccess.GetActualToLevelMappingsByLevelID(selectedLevelID)
                Dim mapping = mappings.FirstOrDefault(Function(m) m.ActualUnitID = actualUnit.ActualUnitID)
                If mapping IsNot Nothing Then
                    mapping.Quantity = qty
                    Dim updateParams As SqlParameter() = {
                    New SqlParameter("@Quantity", qty),
                    New SqlParameter("@MappingID", mapping.MappingID)
                }
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateActualToLevelMapping, updateParams)
                Else
                    ' Rare case: Mapping missing; create new
                    Dim newMapping As New ActualToLevelMappingModel With {
                    .ProjectID = selectedProjectID,
                    .ActualUnitID = actualUnit.ActualUnitID,
                    .LevelID = selectedLevelID,
                    .Quantity = qty
                }
                    dataAccess.SaveActualToLevelMapping(newMapping)
                End If
                actualUnit.CalculatedComponents = CalculateComponentsFromRaw(rawUnits.Find(Function(r) r.RawUnitID = actualUnit.RawUnitID))
                dataAccess.SaveCalculatedComponents(actualUnit)
                assignedUnits(currentUnitIndex) = actualUnit
                displayUnits(currentUnitIndex).UnitName = actualUnit.UnitName
                displayUnits(currentUnitIndex).ReferencedRawUnitName = rawUnits.Find(Function(r) r.RawUnitID = actualUnit.RawUnitID)?.RawUnitName
                displayUnits(currentUnitIndex).PlanSQFT = sqft
                displayUnits(currentUnitIndex).ActualUnitQuantity = qty
                displayUnits(currentUnitIndex).OptionalAdder = adder
                CalculateDerivedFields(displayUnits(currentUnitIndex), actualUnit.CalculatedComponents)
                bindingSource.ResetBindings(False)
                currentUnitIndex = -1
            End If

            ' Common rollups for new/edit
            dataAccess.UpdateLevelRollups(selectedLevelID)
            Dim buildingID As Integer = dataAccess.GetBuildingIDByLevelID(selectedLevelID)
            If buildingID > 0 Then
                dataAccess.UpdateBuildingRollups(buildingID)
            Else
                MessageBox.Show("No building found for the selected level. Building rollup skipped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            End If
            UpdateTotals()

            ' Reset pane
            ResetDetailsPane()
            LblStatus.Text = "Unit saved and attached successfully—consistent data maintained across levels."
        End If
    End Sub
    ' New helper to reset details pane (reduce repetition, Deming: streamline processes)
    Private Sub ResetDetailsPane()
        TxtUnitName.Text = ""
        TxtPlanSQFT.Text = ""
        TxtActualUnitQuantity.Text = "1"
        TxtOptionalAdder.Text = "1.0"
        CmbUnitType.SelectedIndex = -1
        PanelDetails.Visible = False
    End Sub

    ' Helper to compute CalculatedComponents from RawUnit (per /SQFT values)
    Private Function CalculateComponentsFromRaw(rawUnit As RawUnitModel) As List(Of CalculatedComponentModel)
        Dim components As New List(Of CalculatedComponentModel)
        Dim sqFt As Decimal = rawUnit.SqFt.GetValueOrDefault()
        If sqFt <= 0 Then Return components

        ' Fetch project-specific values
        Dim projectID As Integer = selectedProjectID ' From form
        Dim productTypeID As Integer = rawUnit.ProductTypeID
        Dim lumberAdder As Decimal = dataAccess.GetLumberAdder(projectID, productTypeID)
        Dim overrideMargin As Decimal = dataAccess.GetMarginPercent(projectID, productTypeID)

        ' Determine margin: override or raw fallback
        Dim marginPercent As Decimal = overrideMargin
        If marginPercent = 0D Then
            Dim rawCost As Decimal = rawUnit.OverallCost.GetValueOrDefault()
            Dim rawSell As Decimal = rawUnit.TotalSellPrice.GetValueOrDefault()
            If rawSell > 0D Then
                marginPercent = (rawSell - rawCost) / rawSell
            End If
        End If

        ' Calculate per-SQFT costs
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

        ' For reference/checks only: sell and margin per-SQFT
        Dim sellPerSqft As Decimal = If(marginPercent >= 1D, 0D, overallCostPerSqft / (1D - marginPercent))
        Dim marginPerSqft As Decimal = sellPerSqft - overallCostPerSqft

        ' Add to components (costs primary, sell/margin as reference)
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

        ' Reference only
        components.Add(New CalculatedComponentModel With {.ComponentType = "SellPrice/SQFT", .Value = sellPerSqft})
        components.Add(New CalculatedComponentModel With {.ComponentType = "Margin/SQFT", .Value = marginPerSqft})

        Return components
    End Function

    ' Helper to calculate derived fields for DisplayUnitData
    Private Sub CalculateDerivedFields(displayUnit As DisplayUnitData, components As List(Of CalculatedComponentModel))
        If components Is Nothing Then
            Debug.WriteLine("Missing components for DisplayUnitData")
            Return
        End If

        Dim lfPerSqft As Decimal = GetComponentValue(components, "LF/SQFT")
        displayUnit.LF = lfPerSqft * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder

        Dim bdftPerSqft As Decimal = GetComponentValue(components, "BDFT/SQFT")
        displayUnit.BDFT = bdftPerSqft * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder

        Dim lumberPerSqft As Decimal = GetComponentValue(components, "Lumber/SQFT")
        displayUnit.LumberCost = lumberPerSqft * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder

        Dim platePerSqft As Decimal = GetComponentValue(components, "Plate/SQFT")
        displayUnit.PlateCost = platePerSqft * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder

        Dim manufLaborPerSqft As Decimal = GetComponentValue(components, "ManufLabor/SQFT")
        displayUnit.ManufLaborCost = manufLaborPerSqft * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder

        Dim designLaborPerSqft As Decimal = GetComponentValue(components, "DesignLabor/SQFT")
        displayUnit.DesignLabor = designLaborPerSqft * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder

        Dim mgmtLaborPerSqft As Decimal = GetComponentValue(components, "MGMTLabor/SQFT")
        displayUnit.MGMTLabor = mgmtLaborPerSqft * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder

        Dim jobSuppliesPerSqft As Decimal = GetComponentValue(components, "JobSupplies/SQFT")
        displayUnit.JobSuppliesCost = jobSuppliesPerSqft * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder

        Dim manHoursPerSqft As Decimal = GetComponentValue(components, "ManHours/SQFT")
        displayUnit.ManHours = manHoursPerSqft * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder

        Dim itemCostPerSqft As Decimal = GetComponentValue(components, "ItemCost/SQFT")
        displayUnit.ItemCost = itemCostPerSqft * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder

        Dim overallCostPerSqft As Decimal = GetComponentValue(components, "OverallCost/SQFT")
        displayUnit.OverallCost = overallCostPerSqft * displayUnit.PlanSQFT * displayUnit.ActualUnitQuantity * displayUnit.OptionalAdder
    End Sub


    Private Sub BtnRecalculate_Click(sender As Object, e As EventArgs) Handles BtnRecalculate.Click
        displayUnits.Clear()
        Dim mappings = dataAccess.GetActualToLevelMappingsByLevelID(selectedLevelID)
        For Each mapping In mappings
            Dim actualUnit As ActualUnitModel = mapping.ActualUnit  ' Assuming populated
            Dim displayUnit As New DisplayUnitData With {
            .ActualUnitID = mapping.ActualUnitID,
            .MappingID = mapping.MappingID,
            .UnitName = actualUnit.UnitName,
            .ReferencedRawUnitName = actualUnit.ReferencedRawUnitName,
            .PlanSQFT = actualUnit.PlanSQFT,
            .ActualUnitQuantity = mapping.Quantity,
            .OptionalAdder = actualUnit.OptionalAdder
        }

            ' Get components and calculate cost fields
            Dim components As List(Of CalculatedComponentModel) = actualUnit.CalculatedComponents
            If components Is Nothing Then
                Debug.WriteLine("Missing components for ActualUnitID: " & mapping.ActualUnitID)
                Continue For
            End If
            CalculateDerivedFields(displayUnit, components)  ' Set LF, BDFT, costs, etc.

            ' Get margin
            Dim productTypeID As Integer = actualUnit.ProductTypeID
            Dim marginPercent As Decimal = If(marginCache.ContainsKey(productTypeID), marginCache(productTypeID), dataAccess.GetMarginPercent(selectedProjectID, productTypeID))
            If marginPercent = 0D Then
                Dim rawmarginUnit = rawUnits.Find(Function(r) r.RawUnitID = actualUnit.RawUnitID)
                If rawmarginUnit IsNot Nothing Then
                    Dim rawCost As Decimal = rawmarginUnit.OverallCost.GetValueOrDefault()
                    Dim rawSell As Decimal = rawmarginUnit.TotalSellPrice.GetValueOrDefault()
                    If rawSell > 0D Then marginPercent = (rawSell - rawCost) / rawSell
                End If
            End If
            ' Reapply LumberAdder as difference between ProjectProductSettings.LumberAdder and RawUnit.AvgSPFNo2

            Dim rawUnit = rawUnits.Find(Function(r) r.RawUnitID = actualUnit.RawUnitID)
            If rawUnit Is Nothing Then
                Debug.WriteLine("Missing raw unit for ActualUnitID: " & mapping.ActualUnitID)
                Continue For
            End If
            Dim projectLumberAdder As Decimal = dataAccess.GetLumberAdder(selectedProjectID, productTypeID)
            Dim avgSPFNo2 As Decimal = rawUnit.AvgSPFNo2.GetValueOrDefault()
            Dim effectiveLumberAdder As Decimal = If(projectLumberAdder > 0, projectLumberAdder - avgSPFNo2, 0D)
            If effectiveLumberAdder > 0 Then
                Dim adderAmount As Decimal = (displayUnit.BDFT / 1000D) * effectiveLumberAdder
                displayUnit.LumberCost += adderAmount
                displayUnit.OverallCost += adderAmount
            End If

            displayUnits.Add(displayUnit)
        Next

        ' Calculate level totals for proration
        Dim totalBDFT As Decimal = displayUnits.Sum(Function(u) u.BDFT)

        ' Fetch required values
        Dim mileageRate As Decimal = dataAccess.GetConfigValue("MileageRate")
        Dim milesToJobSite As Integer = dataAccess.GetMilesToJobSite(selectedProjectID)

        ' Calculate level delivery (matches Queries.CalculateDeliveryCost logic)
        Dim deliveryTotal As Decimal = 0D
        If totalBDFT > 0 Then
            Dim numLoads As Decimal = Math.Ceiling(totalBDFT / 10000D)
            deliveryTotal = numLoads * mileageRate * milesToJobSite
            deliveryTotal = Math.Round(deliveryTotal, 0)
            If deliveryTotal < 150 Then deliveryTotal = 150
        End If

        ' Prorate delivery and apply margin
        For Each unit In displayUnits
            Dim productTypeID As Integer = assignedUnits.First(Function(a) a.ActualUnitID = unit.ActualUnitID).ProductTypeID
            Dim marginPercent As Decimal = If(marginCache.ContainsKey(productTypeID), marginCache(productTypeID), dataAccess.GetMarginPercent(selectedProjectID, productTypeID))
            If marginPercent = 0D Then
                Dim rawUnit = rawUnits.Find(Function(r) r.RawUnitID = assignedUnits.First(Function(a) a.ActualUnitID = unit.ActualUnitID).RawUnitID)
                If rawUnit IsNot Nothing Then
                    Dim rawCost As Decimal = rawUnit.OverallCost.GetValueOrDefault()
                    Dim rawSell As Decimal = rawUnit.TotalSellPrice.GetValueOrDefault()
                    If rawSell > 0D Then marginPercent = (rawSell - rawCost) / rawSell
                End If
            End If

            If totalBDFT > 0 Then
                unit.DeliveryCost = (unit.BDFT / totalBDFT) * deliveryTotal
            Else
                unit.DeliveryCost = 0D
            End If

            unit.SellPrice = If(marginPercent >= 1D, unit.OverallCost, unit.OverallCost / (1D - marginPercent)) + unit.DeliveryCost  ' Add delivery cost to SellPrice
            unit.Margin = unit.SellPrice - unit.OverallCost
        Next

        bindingSource.DataSource = displayUnits
        bindingSource.ResetBindings(False)
        DataGridViewAssigned.Refresh()
        UpdateTotals()
        dataAccess.UpdateLevelRollups(selectedLevelID)

        ' Fetch BuildingID for the selected LevelID
        Dim buildingQuery As String = "SELECT BuildingID FROM Levels WHERE LevelID = @LevelID"
        Dim buildingParams As SqlParameter() = {New SqlParameter("@LevelID", selectedLevelID)}
        Dim buildingIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(buildingQuery, buildingParams)
        Dim buildingID As Integer = If(buildingIDObj Is DBNull.Value OrElse buildingIDObj Is Nothing, 0, CInt(buildingIDObj))

        If buildingID > 0 Then
            dataAccess.UpdateBuildingRollups(buildingID)
        Else
            MessageBox.Show("No building found for the selected level. Rollup skipped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
        UpdateTotals()

        Dim totalSQFT As Decimal = displayUnits.Sum(Function(u) u.PlanSQFT * u.ActualUnitQuantity)
        LblStatus.Text = $"Total Adjusted SQFT: {totalSQFT} - Recalc complete. No variations detected."
        MessageBox.Show("Recalculated totals successfully.")
    End Sub

    Private Sub BtnToggleDetails_Click(sender As Object, e As EventArgs) Handles BtnToggleDetails.Click
        PanelDetails.Visible = Not PanelDetails.Visible
        BtnToggleDetails.Text = If(PanelDetails.Visible, "Hide Details", "Show Details")
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        PanelDetails.Visible = False
    End Sub

    Private Sub BtnFinish_Click(sender As Object, e As EventArgs) Handles BtnFinish.Click
        LblStatus.Text = $"Last Saved: {DateTime.Now} - Sandbox session complete."
        Me.Close()
    End Sub
    Private Sub DataGridViewAssigned_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewAssigned.CellContentClick
        If e.ColumnIndex = DataGridViewAssigned.Columns("ActionsColumn").Index AndAlso e.RowIndex >= 0 Then
            currentUnitIndex = e.RowIndex
            LoadUnitDetails()
            PanelDetails.Visible = True
            ' MessageBox.Show($"Editing row {e.RowIndex}, Column {e.ColumnIndex}")
        End If
    End Sub

    Private Sub LoadUnitDetails()
        If currentUnitIndex < 0 OrElse currentUnitIndex >= displayUnits.Count Then
            MessageBox.Show("Invalid selection - no unit data available.")
            Return
        End If
        Dim displayUnit = displayUnits(currentUnitIndex)
        Dim actualUnit = assignedUnits(currentUnitIndex)  ' Synced per prior fix
        If actualUnit Is Nothing Then
            MessageBox.Show("Actual unit data not loaded.")
            Return
        End If
        ' Basic fields
        TxtUnitName.Text = displayUnit.UnitName
        TxtReferencedRawUnit.Text = displayUnit.ReferencedRawUnitName
        TxtPlanSQFT.Text = displayUnit.PlanSQFT.ToString("F2")
        TxtActualUnitQuantity.Text = displayUnit.ActualUnitQuantity.ToString("F0")
        TxtOptionalAdder.Text = displayUnit.OptionalAdder.ToString("F2")
        CmbUnitType.SelectedItem = actualUnit.UnitType
        selectedRawUnitID = actualUnit.RawUnitID

        ' CalculatedComponents per-SQFT (fetch from components or recompute from RawUnit)
        Dim rawUnit = rawUnits.Find(Function(r) r.RawUnitID = selectedRawUnitID)
        If rawUnit IsNot Nothing AndAlso rawUnit.SqFt.GetValueOrDefault() > 0 Then
            TxtLumberPerSQFT.Text = (rawUnit.LumberCost.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
            TxtPlatePerSQFT.Text = (rawUnit.PlateCost.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
            TxtBDFTPerSQFT.Text = (rawUnit.BF.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
            TxtManufLaborPerSQFT.Text = (rawUnit.ManufLaborCost.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
            TxtDesignLaborPerSQFT.Text = (rawUnit.DesignLabor.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
            TxtMGMTLaborPerSQFT.Text = (rawUnit.MGMTLabor.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
            TxtJobSuppliesPerSQFT.Text = (rawUnit.JobSuppliesCost.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
            TxtManHoursPerSQFT.Text = (rawUnit.ManHours.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
            TxtItemCostPerSQFT.Text = (rawUnit.ItemCost.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
            TxtOverallCostPerSQFT.Text = (rawUnit.OverallCost.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
            TxtDeliveryCostPerSQFT.Text = (rawUnit.DeliveryCost.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
            TxtTotalSellPricePerSQFT.Text = (rawUnit.TotalSellPrice.GetValueOrDefault() / rawUnit.SqFt.Value).ToString("F2")
        Else
            ' Reset to 0 if no raw data
            TxtLumberPerSQFT.Text = "0.00"
            ' Repeat for all Txt...PerSQFT fields
        End If
        PanelDetails.Visible = True
        BtnToggleDetails.Text = "Hide Details"
        isReusing = False  ' Edit mode for new ActualUnits
        ' Make fields editable
        TxtUnitName.ReadOnly = False
        TxtPlanSQFT.ReadOnly = False
        TxtOptionalAdder.ReadOnly = False
        CmbUnitType.Enabled = True
        btnSaveNewOnly.Visible = False
        btnDeleteUnit.Visible = True
    End Sub
    ' New method to load existing ActualUnits
    Private Sub LoadExistingActualUnits()
        ListboxExistingActualUnits.Items.Clear()
        existingActualUnits = dataAccess.GetActualUnitsByProject(selectedProjectID)
        For Each actual In existingActualUnits
            ListboxExistingActualUnits.Items.Add(actual.UnitName & " (" & actual.UnitType & ")")  ' Display with type for clarity
        Next
    End Sub

    ' Handle selection in ListBoxExistingActualUnits (optional preview)
    Private Sub ListBoxExistingActualUnits_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListboxExistingActualUnits.SelectedIndexChanged
        If ListboxExistingActualUnits.SelectedIndex >= 0 Then
            selectedActualUnitID = existingActualUnits(ListboxExistingActualUnits.SelectedIndex).ActualUnitID
            LblStatus.Text = $"Selected existing unit: {existingActualUnits(ListboxExistingActualUnits.SelectedIndex).UnitName}"
        End If
    End Sub

    ' New button handler for Reuse
    Private Sub BtnReuseActualUnit_Click(sender As Object, e As EventArgs) Handles btnReuseActualUnit.Click
        btnSaveNewOnly.Visible = False
        If ListboxExistingActualUnits.SelectedIndex >= 0 Then
            isReusing = True
            currentUnitIndex = -1  ' Not editing assigned; creating new mapping
            Dim selectedActual As ActualUnitModel = existingActualUnits(ListboxExistingActualUnits.SelectedIndex)
            If selectedActual IsNot Nothing Then
                PanelDetails.Visible = True
                BtnToggleDetails.Text = "Hide Details"

                ' Load for reuse: read-only except Quantity
                TxtUnitName.Text = selectedActual.UnitName
                TxtUnitName.ReadOnly = True
                TxtReferencedRawUnit.Text = selectedActual.ReferencedRawUnitName
                TxtPlanSQFT.Text = selectedActual.PlanSQFT.ToString("F2")
                TxtPlanSQFT.ReadOnly = True
                TxtOptionalAdder.Text = selectedActual.OptionalAdder.ToString("F2")
                TxtOptionalAdder.ReadOnly = True
                CmbUnitType.SelectedItem = selectedActual.UnitType
                CmbUnitType.Enabled = False
                TxtActualUnitQuantity.Text = "1"  ' Default qty for new mapping
                TxtActualUnitQuantity.ReadOnly = False  ' Editable

                ' Display per-SQFT (read-only, from components)
                UpdatePerSQFTFields(selectedActual.CalculatedComponents)

                LblStatus.Text = $"Status: Ready to reuse {selectedActual.UnitName}. Update quantity and save."
            Else
                MessageBox.Show("Selected actual unit not found.")
            End If
        Else
            MessageBox.Show("Please select an existing actual unit from the list.")
        End If
    End Sub

    ' Helper to update per-SQFT fields from components
    Private Sub UpdatePerSQFTFields(components As List(Of CalculatedComponentModel))
        TxtLumberPerSQFT.Text = GetComponentValue(components, "Lumber/SQFT").ToString("F2")
        TxtPlatePerSQFT.Text = GetComponentValue(components, "Plate/SQFT").ToString("F2")
        TxtBDFTPerSQFT.Text = GetComponentValue(components, "BDFT/SQFT").ToString("F2")
        TxtManufLaborPerSQFT.Text = GetComponentValue(components, "ManufLabor/SQFT").ToString("F2")
        TxtDesignLaborPerSQFT.Text = GetComponentValue(components, "DesignLabor/SQFT").ToString("F2")
        TxtMGMTLaborPerSQFT.Text = GetComponentValue(components, "MGMTLabor/SQFT").ToString("F2")
        TxtJobSuppliesPerSQFT.Text = GetComponentValue(components, "JobSupplies/SQFT").ToString("F2")
        TxtManHoursPerSQFT.Text = GetComponentValue(components, "ManHours/SQFT").ToString("F2")
        TxtItemCostPerSQFT.Text = GetComponentValue(components, "ItemCost/SQFT").ToString("F2")
        TxtOverallCostPerSQFT.Text = GetComponentValue(components, "OverallCost/SQFT").ToString("F2")
        TxtDeliveryCostPerSQFT.Text = GetComponentValue(components, "DeliveryCost/SQFT").ToString("F2")
        TxtTotalSellPricePerSQFT.Text = GetComponentValue(components, "SellPrice/SQFT").ToString("F2")

    End Sub

    Private Sub btnSaveNewOnly_Click(sender As Object, e As EventArgs) Handles btnSaveNewOnly.Click
        Dim actualUnit As ActualUnitModel = CreateAndSaveActualUnit()
        If actualUnit Is Nothing Then Return  ' Validation failed

        ' No mapping or grid add—empowers reuse later
        ' Optionally: Refresh existing actual units list if you have one
        LoadExistingActualUnits()  ' Assuming you add this method per prior convos

        ResetDetailsPane()
        LblStatus.Text = "New ActualUnit created successfully—ready for reuse across levels without assignment."
    End Sub

    Private Sub BtnDeleteUnit_Click(sender As Object, e As EventArgs) Handles btnDeleteUnit.Click
        If currentUnitIndex < 0 OrElse currentUnitIndex >= displayUnits.Count Then
            MessageBox.Show("No unit selected for deletion.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If MessageBox.Show("Remove this unit from the level? The ActualUnit will remain available for reuse.", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Try
                Dim mappingID As Integer = displayUnits(currentUnitIndex).MappingID
                System.Diagnostics.Debug.WriteLine("Attempting to delete MappingID: " & mappingID)
                dataAccess.DeleteActualToLevelMapping(mappingID)

                ' Remove from local lists
                displayUnits.RemoveAt(currentUnitIndex)
                assignedUnits.RemoveAt(currentUnitIndex)

                ' Refresh UI
                bindingSource.ResetBindings(False)
                DataGridViewAssigned.Refresh()
                UpdateTotals()

                ' Update rollups
                dataAccess.UpdateLevelRollups(selectedLevelID)
                Dim buildingID As Integer = dataAccess.GetBuildingIDByLevelID(selectedLevelID)
                If buildingID > 0 Then
                    dataAccess.UpdateBuildingRollups(buildingID)
                Else
                    MessageBox.Show("No building found for the selected level. Building rollup skipped.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
                UpdateTotals() ' Ensure UI sync

                ' Reset pane and state
                ResetDetailsPane()
                currentUnitIndex = -1
                btnDeleteUnit.Visible = False
                LblStatus.Text = "Unit mapping removed successfully—ActualUnit preserved for reuse."
                System.Diagnostics.Debug.WriteLine("MappingID " & mappingID & " deleted successfully.")
            Catch ex As Exception
                System.Diagnostics.Debug.WriteLine("Delete error: " & ex.ToString()) ' Log full stack
                MessageBox.Show("Error removing unit mapping: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub TreeViewLevels_MouseDown(sender As Object, e As MouseEventArgs) Handles TreeViewLevels.MouseDown
        If e.Button = MouseButtons.Right Then
            Dim hit As TreeNode = TreeViewLevels.GetNodeAt(e.X, e.Y)
            If hit IsNot Nothing Then
                TreeViewLevels.SelectedNode = hit
                Dim nodeTag = DirectCast(hit.Tag, Dictionary(Of String, Object))
                Dim isLevel As Boolean = CStr(nodeTag("Type")) = "Level"
                mnuCopyUnits.Enabled = isLevel
                mnuPasteUnits.Enabled = isLevel AndAlso copiedMappings.Count > 0
                ContextMenuLevels.Show(TreeViewLevels, e.Location)
            End If
        End If
    End Sub
    Private Sub mnuCopyUnits_Click(sender As Object, e As EventArgs) Handles mnuCopyUnits.Click
        If selectedLevelID <= 0 Then
            MessageBox.Show("Invalid level selected for copy.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

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
        If selectedLevelID <= 0 OrElse copiedMappings.Count = 0 Then
            MessageBox.Show("No data to paste or invalid target level.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim levelTag = DirectCast(TreeViewLevels.SelectedNode.Tag, Dictionary(Of String, Object))
        Dim targetProductTypeID As Integer = CInt(levelTag("ProductTypeID"))
        If targetProductTypeID <> sourceProductTypeID Then
            MessageBox.Show("Cannot paste: Source and target levels must have the same Product Type (e.g., Floor vs. Roof).", "Compatibility Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim existingMappings = dataAccess.GetActualToLevelMappingsByLevelID(selectedLevelID)
        For Each copied In copiedMappings
            Dim actualUnitID As Integer = copied.Item1
            Dim quantity As Integer = copied.Item2

            Dim existing = existingMappings.FirstOrDefault(Function(m) m.ActualUnitID = actualUnitID)
            If existing IsNot Nothing Then
                ' Update quantity (additive)
                existing.Quantity += quantity
                dataAccess.SaveActualToLevelMapping(existing)
            Else
                ' Create new mapping
                Dim newMapping As New ActualToLevelMappingModel With {
                    .ProjectID = selectedProjectID,
                    .ActualUnitID = actualUnitID,
                    .LevelID = selectedLevelID,
                    .Quantity = quantity
                }
                dataAccess.SaveActualToLevelMapping(newMapping)
            End If
        Next

        ' Refresh if target is selected
        LoadAssignedUnits()
        UpdateTotals()
        dataAccess.UpdateLevelRollups(selectedLevelID)
        Dim buildingID As Integer = dataAccess.GetBuildingIDByLevelID(selectedLevelID)
        If buildingID > 0 Then dataAccess.UpdateBuildingRollups(buildingID)

        copiedMappings.Clear() ' Clear after successful paste
        LblStatus.Text = "Units pasted successfully. Rollups updated."
    End Sub

    Private Sub FrmPSE_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        copiedMappings.Clear()
    End Sub

End Class