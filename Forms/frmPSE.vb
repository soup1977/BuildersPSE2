Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.BuildersPSE.Utilities
Imports BuildersPSE2.DataAccess
Imports Microsoft.VisualBasic.FileIO

Public Class FrmPSE
    Inherits Form
    ' Enum for details pane modes
    Private Enum DetailsMode
        None
        NewUnit
        EditGlobalUnit
        ReuseUnit
        EditMappingQuantity
    End Enum
    Private currentdetailsMode As DetailsMode = DetailsMode.None

    ' Data Tracking
    Private selectedMappingID As Integer = -1
    Private selectedLevelID As Integer = -1
    Private selectedProjectID As Integer = -1
    Private selectedBuildingID As Integer = -1 ' Cached for efficiency
    Private selectedRawUnitID As Integer = -1 ' Track selected RawUnit for database
    Private displayUnits As New List(Of DisplayUnitData) ' Merged with ActualUnit reference
    Private bindingSource As New BindingSource
    Private rawUnits As New List(Of RawUnitModel)
    Private dataAccess As New ProjectDataAccess
    Private isImporting As Boolean = False
    Private isReusing As Boolean = False
    Private selectedActualUnitID As Integer = -1
    Private existingActualUnits As New List(Of ActualUnitModel)
    Private marginCache As New Dictionary(Of Integer, Decimal)
    Private isGlobalEdit As Boolean = False
    Private selectedVersionID As Integer

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


    Public Sub New(projectID As Integer, Optional versionID As Integer = 0)
        ' Check for versions before initializing
        Dim versions As List(Of ProjectVersionModel) = ProjVersionDataAccess.GetProjectVersions(projectID)
        If Not versions.Any() Then
            MessageBox.Show("No versions exist for this project. Create a version in the project editor.", "No Versions", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Throw New InvalidOperationException("Cannot open PSE form: No project versions available.")
        End If

        InitializeComponent()
        selectedProjectID = projectID
        If versionID = 0 Then
            selectedVersionID = versions.First().VersionID
        Else
            selectedVersionID = If(versions.Any(Function(v) v.VersionID = versionID), versionID, versions.First().VersionID)
        End If
        Dim project = dataAccess.GetProjectByID(projectID)
        Dim selectedVersion As ProjectVersionModel = versions.FirstOrDefault(Function(v) v.VersionID = selectedVersionID)
        Me.Text = $"Edit PSE - {project.JBID} - {selectedVersion.VersionName}"
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
        UpdateStatus($"Grid view switched to {(If(isDetailedView, "Detailed", "Base"))} mode.")
    End Sub

    Private Sub ChkDetailedView_CheckedChanged(sender As Object, e As EventArgs) Handles ChkDetailedView.CheckedChanged
        ToggleGridView()
    End Sub

    Private Sub LoadLevelHierarchy()
        TreeViewLevels.Nodes.Clear()
        Try
            Dim project = dataAccess.GetProjectByID(selectedProjectID)
            If project Is Nothing Then
                UpdateStatus("Status: Project not found.")
                Return
            End If
            Dim versions As List(Of ProjectVersionModel) = ProjVersionDataAccess.GetProjectVersions(selectedProjectID)
            Dim selectedVersion As ProjectVersionModel = versions.FirstOrDefault(Function(v) v.VersionID = selectedVersionID)
            Dim projectNodeText As String = project.ProjectName & If(selectedVersion IsNot Nothing, " - " & selectedVersion.VersionName, "")
            Dim projectNode As TreeNode = TreeViewLevels.Nodes.Add(projectNodeText)
            projectNode.Tag = New Dictionary(Of String, Object) From {{"Type", "Project"}, {"ID", selectedProjectID}}
            Dim buildings = ProjectDataAccess.GetBuildingsByVersionID(selectedVersionID)
            If Not buildings.Any() Then
                UpdateStatus("Status: No buildings found for this version.")
                Return
            End If
            For Each bldg In buildings
                Dim bldgNode As TreeNode = projectNode.Nodes.Add(bldg.BuildingName)
                bldgNode.Tag = New Dictionary(Of String, Object) From {{"Type", "Building"}, {"ID", bldg.BuildingID}}
                Dim levels = ProjectDataAccess.GetLevelsByBuildingID(bldg.BuildingID)
                For Each lvl In levels
                    Dim levelNode As TreeNode = bldgNode.Nodes.Add(lvl.ProductTypeName & " Level " & lvl.LevelNumber)
                    levelNode.Tag = New Dictionary(Of String, Object) From {{"Type", "Level"}, {"ID", lvl.LevelID}, {"ProductTypeID", lvl.ProductTypeID}}
                Next
            Next
            projectNode.Expand()
            UpdateStatus("Status: Project hierarchy loaded for version " & selectedVersionID & ".")
        Catch ex As Exception
            UpdateStatus("Status: Error loading hierarchy: " & ex.Message)
            MessageBox.Show("Error loading project hierarchy: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub TreeViewLevels_AfterSelect(sender As Object, e As TreeViewEventArgs) Handles TreeViewLevels.AfterSelect
        If e.Node.Level = 2 Then ' Level node
            Dim levelTag = DirectCast(e.Node.Tag, Dictionary(Of String, Object))
            selectedLevelID = CInt(levelTag("ID"))
            Dim productTypeID As Integer = CInt(levelTag("ProductTypeID"))
            FilterRawUnits(productTypeID)
            FilterActualUnits(productTypeID)
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
        Try
            rawUnits = ProjectDataAccess.GetRawUnitsByVersionID(selectedVersionID).Where(Function(r) r.ProductTypeID = productTypeID).ToList()
            If Not rawUnits.Any() Then
                UpdateStatus("Status: No raw units found for this version and product type.")
                Return
            End If
            For Each rawUnit In rawUnits
                ListBoxAvailableUnits.Items.Add(rawUnit.RawUnitName)
            Next
            UpdateStatus("Status: Raw units loaded for version " & selectedVersionID & " and product type " & productTypeID & ".")
            UpdateSelectedRawPreview()
        Catch ex As Exception
            UpdateStatus("Status: Error loading raw units: " & ex.Message)
            MessageBox.Show("Error loading raw units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub FilterActualUnits(productTypeID As Integer)
        ListboxExistingActualUnits.Items.Clear()
        Try
            existingActualUnits = ProjectDataAccess.GetActualUnitsByVersion(selectedVersionID).Where(Function(u) u.ProductTypeID = productTypeID).ToList()
            If Not existingActualUnits.Any() Then
                UpdateStatus("Status: No actual units found for this version and product type.")
                Return
            End If
            For Each unit In existingActualUnits
                Dim sellPricePerSQFT As Decimal = 0D
                Dim sellPriceComponent = unit.CalculatedComponents?.FirstOrDefault(Function(c) c.ComponentType = "SellPrice/SQFT")
                If sellPriceComponent IsNot Nothing Then
                    sellPricePerSQFT = sellPriceComponent.Value
                End If
                ListboxExistingActualUnits.Items.Add($"{unit.UnitName} ({unit.UnitType}) - {sellPricePerSQFT:C2}/SQFT")
            Next
            UpdateStatus("Status: Actual units loaded for version " & selectedVersionID & " and product type " & productTypeID & ".")
        Catch ex As Exception
            UpdateStatus("Status: Error loading actual units: " & ex.Message)
            MessageBox.Show("Error loading actual units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ListboxExistingActualUnits_DrawItem(sender As Object, e As DrawItemEventArgs) Handles ListboxExistingActualUnits.DrawItem
        If e.Index < 0 Then Exit Sub
        Dim itemText As String = ListboxExistingActualUnits.Items(e.Index).ToString()
        Dim unit As ActualUnitModel = existingActualUnits(e.Index)
        Dim backColor As Color = Color.White
        If Not String.IsNullOrEmpty(unit.ColorCode) Then
            Try
                backColor = ColorTranslator.FromHtml("#" & unit.ColorCode)
            Catch
                ' Fallback to white on invalid hex
            End Try
        End If

        ' Draw the background (ColorCode-based) to cover entire bounds
        e.Graphics.FillRectangle(New SolidBrush(backColor), e.Bounds)

        ' Draw selection highlight if selected
        If (e.State And DrawItemState.Selected) = DrawItemState.Selected Then
            ' Use semi-transparent light blue highlight
            Dim highlightColor As Color = Color.FromArgb(128, SystemColors.Highlight) ' 50% opacity
            e.Graphics.FillRectangle(New SolidBrush(highlightColor), e.Bounds)
            ' Draw a 1-pixel border inside bounds to avoid overlap
            Using pen As New Pen(Color.Black, 1)
                Dim borderRect As New Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2)
                e.Graphics.DrawRectangle(pen, borderRect)
            End Using
        End If

        ' Set text color: white for black background, black for all others
        Dim luminance As Double = (0.299 * backColor.R + 0.587 * backColor.G + 0.114 * backColor.B) / 255
        Dim textColor As Color = If(luminance < 0.5, Color.White, Color.Black)

        ' Draw the text
        e.Graphics.DrawString(itemText, e.Font, New SolidBrush(textColor), e.Bounds.X, e.Bounds.Y)

    End Sub
    Private Sub LoadAssignedUnits()
        Try
            displayUnits = ProjectDataAccess.ComputeLevelUnits(selectedLevelID)
            bindingSource.DataSource = displayUnits
            bindingSource.ResetBindings(False)
            DataGridViewAssigned.Refresh()
            UpdateLevelTotals()
        Catch ex As Exception
            MessageBox.Show("Error loading assigned units: " & ex.Message)
        End Try
    End Sub



    Private Sub UpdateLevelTotals()
        Dim totalSQFT As Decimal = displayUnits.Sum(Function(u) u.PlanSQFT * u.ActualUnitQuantity)
        TxtTotalPlanSQFT.Text = totalSQFT.ToString("F2")
        TxtTotalQuantity.Text = displayUnits.Sum(Function(u) u.ActualUnitQuantity).ToString("F0")
        TxtTotalLF.Text = displayUnits.Sum(Function(u) u.LF).ToString("F2")
        TxtTotalBDFT.Text = displayUnits.Sum(Function(u) u.BDFT).ToString("F2")
        TxtTotalLumberCost.Text = displayUnits.Sum(Function(u) u.LumberCost).ToString("C2")
        TxtTotalPlateCost.Text = displayUnits.Sum(Function(u) u.PlateCost).ToString("C2")
        TxtTotalManufLaborCost.Text = displayUnits.Sum(Function(u) u.ManufLaborCost).ToString("C2")
        TxtTotalDesignLabor.Text = displayUnits.Sum(Function(u) u.DesignLabor).ToString("C2")
        TxtTotalMGMTLabor.Text = displayUnits.Sum(Function(u) u.MGMTLabor).ToString("C2")
        TxtTotalJobSuppliesCost.Text = displayUnits.Sum(Function(u) u.JobSuppliesCost).ToString("C2")
        TxtTotalManHours.Text = displayUnits.Sum(Function(u) u.ManHours).ToString("C2")
        TxtTotalItemCost.Text = displayUnits.Sum(Function(u) u.ItemCost).ToString("C2")
        TxtTotalOverallCost.Text = displayUnits.Sum(Function(u) u.OverallCost).ToString("C2")
        TxtTotalSellPrice.Text = displayUnits.Sum(Function(u) u.SellPrice).ToString("C2")
        TxtTotalMargin.Text = displayUnits.Sum(Function(u) u.Margin).ToString("C2")
        If CInt(TxtTotalSellPrice.Text) > 0 Then
            txtTotMargin.Text = (CDec(TxtTotalMargin.Text) / CDec(TxtTotalSellPrice.Text)).ToString("P2")
        Else
            txtTotMargin.Text = "0.00"
        End If
        txtTotalDeliveryCost.Text = displayUnits.Sum(Function(u) u.DeliveryCost).ToString("C2")
        UpdateStatus($"Total Adjusted SQFT: {totalSQFT} - Recalc complete. No variations detected... yet.")
    End Sub

    Private Sub UpdateSelectedRawPreview()
        Try
            Dim currentRawUnit = If(ListBoxAvailableUnits.SelectedIndex >= 0, rawUnits.FirstOrDefault(Function(r) r.RawUnitName = ListBoxAvailableUnits.Items(ListBoxAvailableUnits.SelectedIndex).ToString()), Nothing)
            If currentRawUnit Is Nothing OrElse currentRawUnit.SqFt.GetValueOrDefault() <= 0 Then
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
                UpdateStatus($"Status: No valid raw unit or zero SQFT for version {selectedVersionID}.")
                Return
            End If
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
            UpdateStatus($"Status: Preview updated for raw unit {currentRawUnit.RawUnitName} in version {selectedVersionID}.")
        Catch ex As Exception
            UpdateStatus($"Status: Error updating raw unit preview for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error updating raw unit preview: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub FrmPSE_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim productTypes = dataAccess.GetProductTypes()
            If Not productTypes.Any() Then
                UpdateStatus("Status: No product types found for version " & selectedVersionID & ".")
                Return
            End If
            If TreeViewLevels.Nodes.Count > 0 AndAlso TreeViewLevels.Nodes(0).Nodes.Count > 0 AndAlso TreeViewLevels.Nodes(0).Nodes(0).Nodes.Count > 0 Then
                TreeViewLevels.SelectedNode = TreeViewLevels.Nodes(0).Nodes(0).Nodes(0) ' Assume first level
            Else
                UpdateStatus("Status: No levels available for version " & selectedVersionID & ".")
            End If

            populaterawunits()

            UpdateLevelTotals()
            UpdateSelectedRawPreview()
            TreeViewLevels.ExpandAll()
            UpdateStatus("Status: Form loaded for version " & selectedVersionID & ".")
        Catch ex As Exception
            UpdateStatus("Status: Error initializing form for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error initializing form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub populaterawunits()
        ' Populate DataGridView1 with RawUnits for the selected VersionID
        Dim rawUnits As List(Of RawUnitModel) = ProjectDataAccess.GetRawUnitsByVersionID(selectedVersionID)
        If rawUnits.Any() Then
            dgvRawUnitsData.DataSource = rawUnits
        Else
            dgvRawUnitsData.DataSource = Nothing
            UpdateStatus("Status: No raw units found for version " & selectedVersionID & ".")
        End If
    End Sub
    Private Function CreateAndSaveActualUnit() As ActualUnitModel
        Try
            Dim sqft As Decimal
            Dim adder As Decimal
            If String.IsNullOrEmpty(TxtUnitName.Text) OrElse Not Decimal.TryParse(TxtPlanSQFT.Text, sqft) OrElse sqft <= 0 OrElse
           Not Decimal.TryParse(TxtOptionalAdder.Text, adder) OrElse adder < 1 Then
                UpdateStatus("Status: Invalid input—Name, SQFT (>0), and Adder (≥1) must be valid.")
                MessageBox.Show("Invalid input—Name, SQFT (>0), and Adder (≥1) must be valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End If

            Dim rawUnit As RawUnitModel = rawUnits.FirstOrDefault(Function(r) r.RawUnitID = selectedRawUnitID)
            If rawUnit Is Nothing Then
                UpdateStatus("Status: Referenced Raw Unit not found.")
                MessageBox.Show("Referenced Raw Unit not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End If

            Dim actualUnit As New ActualUnitModel With {
            .VersionID = selectedVersionID,
            .RawUnitID = rawUnit.RawUnitID,
            .ProductTypeID = rawUnit.ProductTypeID,
            .UnitName = TxtUnitName.Text,
            .PlanSQFT = sqft,
            .ColorCode = txtColorCode.Text,
            .UnitType = If(CmbUnitType.SelectedItem?.ToString(), "Res"),
            .OptionalAdder = adder,
            .MarginPercent = ProjectDataAccess.GetEffectiveMargin(Me.selectedVersionID, rawUnit.ProductTypeID, rawUnit.RawUnitID)
        }

            dataAccess.SaveActualUnit(actualUnit)
            actualUnit.CalculatedComponents = CalculateComponentsFromRaw(rawUnit, Me.selectedVersionID)
            dataAccess.SaveCalculatedComponents(actualUnit)

            UpdateStatus("Status: Actual unit created for version " & selectedVersionID & ".")
            Return actualUnit
        Catch ex As Exception
            UpdateStatus("Status: Error creating actual unit for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error creating actual unit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End Try
    End Function

    Private Function CalculateComponentsFromRaw(rawUnit As RawUnitModel, versionID As Integer) As List(Of CalculatedComponentModel)
        Dim components As New List(Of CalculatedComponentModel)
        Dim sqFt As Decimal = rawUnit.SqFt.GetValueOrDefault()
        If sqFt <= 0 Then Return components

        Try
            Dim productTypeID As Integer = rawUnit.ProductTypeID
            Dim lumberAdder As Decimal = LumberDataAccess.GetLumberAdder(versionID, productTypeID)
            Dim marginPercent As Decimal = ProjectDataAccess.GetEffectiveMargin(versionID, productTypeID, rawUnit.RawUnitID)

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

            Dim overallCostPerSqft As Decimal = rawUnit.OverallCost.GetValueOrDefault() / sqFt

            Dim sellPerSqft As Decimal = rawUnit.TotalSellPrice.GetValueOrDefault() / sqFt
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
        Catch ex As Exception
            UpdateStatus("Status: Error calculating components for version " & versionID & ": " & ex.Message)
            MessageBox.Show("Error calculating components: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return components
        End Try
    End Function



    Private Sub BtnImportLevelData_Click(sender As Object, e As EventArgs) Handles BtnImportLevelData.Click
        Dim ofd As New OpenFileDialog With {
        .Filter = "CSV Files|*.csv"
    }
        If ofd.ShowDialog() = DialogResult.OK Then
            isImporting = True
            Dim productTypeID As Integer = -1 ' Default value
            If selectedLevelID <> -1 Then
                Dim levelTag = DirectCast(TreeViewLevels.SelectedNode.Tag, Dictionary(Of String, Object))
                productTypeID = CInt(levelTag("ProductTypeID")) ' Initialize from selected level
            End If
            Try
                If ImportRawUnitsWithMapping(ofd.FileName, productTypeID) Then
                    UpdateStatus("Status: Raw units imported successfully for version " & selectedVersionID & ".")
                    If selectedLevelID <> -1 Then
                        populaterawunits()
                        FilterRawUnits(productTypeID)
                        RecalculateAllActualUnits(selectedVersionID)
                        FilterActualUnits(productTypeID)
                        LoadAssignedUnits()
                    End If
                Else
                    UpdateStatus("Status: Import cancelled for version " & selectedVersionID & ".")
                End If
            Catch ex As Exception
                UpdateStatus("Status: Import failed for version " & selectedVersionID & ": " & ex.Message)
                MessageBox.Show("Import failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                isImporting = False
            End Try
        End If
    End Sub

    ' Add in frmPSE.vb: New subroutine to handle CSV parsing and user-driven mapping, empowering users (servant leadership) while standardizing updates to prevent defects (Deming).
    ' Revised ImportRawUnitsWithMapping in frmPSE.vb: Use case-insensitive key access by storing keys in upper case, change raw unit name key to "Elevation", and validate required headers accordingly. This resolves the "key not present" error by handling the actual CSV structure.
    ' In frmPSE.vb
    ' Modified to insert initial RawUnitLumberHistory record after each RawUnit insert or update
    ' Explanation: After calling dataAccess.InsertRawUnit or dataAccess.UpdateRawUnit in the Confirm Mapping handler,
    ' insert a RawUnitLumberHistory record with CostEffectiveDateID = NULL, copying LumberCost and Avg* fields from the model.
    ' This ensures original history for form-based imports, keeping RawUnits static.
    ' Minimal change: Added code inside the btnConfirm.Click handler after InsertRawUnit/UpdateRawUnit.
    ' Streamlines truss design imports (Deming's quality focus) and provides auditable data (Servant leadership).

    Private Function ImportRawUnitsWithMapping(filePath As String, productTypeID As Integer) As Boolean
        Dim newRawData As New List(Of Dictionary(Of String, String))
        Const NAME_KEY As String = "ELEVATION" ' Use "Elevation" as the key for RawUnitName based on CSV structure
        Dim requiredHeaders As String() = {"ELEVATION", "PRODUCT", "BF", "LF", "EWPLF", "SQFT", "FCAREA", "LUMBERCOST", "PLATECOST", "MANUFLABORCOST", "DESIGNLABOR", "MGMTLABOR", "JOBSUPPLIESCOST", "MANHOURS", "ITEMCOST", "OVERALLCOST", "DELIVERYCOST", "TOTALSELLPRICE", "AVGSPFNO2"}
        Dim optionalHeaders As String() = {"SPFNO2BDFT", "AVG2X4-1800", "2X4-1800BDFT", "AVG2X4-2400", "2X4-2400BDFT", "AVG2X6-1800", "2X6-1800BDFT", "AVG2X6-2400", "2X6-2400BDFT"}
        Dim skippedHeaders As New HashSet(Of String) From {"JOBNUMBER", "PROJECT", "CUSTOMERNAME", "JOBNAME", "STRUCTURENAME", "PLAN"}

        ' Parse CSV and validate headers
        Using parser As New TextFieldParser(filePath)
            parser.Delimiters = New String() {","}
            parser.HasFieldsEnclosedInQuotes = True
            parser.TrimWhiteSpace = True
            If parser.EndOfData Then
                Throw New Exception("CSV file is empty.")
            End If
            Dim headers() As String = parser.ReadFields()
            Dim headerDict As New Dictionary(Of String, String)
            For Each h In headers
                Dim trimmedHeader = h.Trim()
                headerDict(trimmedHeader.ToUpper()) = trimmedHeader
            Next
            ' Validate required headers (case-insensitive)
            Dim missingHeaders As New List(Of String)
            For Each reqHeader In requiredHeaders
                If Not headerDict.ContainsKey(reqHeader.ToUpper()) Then
                    missingHeaders.Add(reqHeader)
                End If
            Next
            If missingHeaders.Any() Then
                Throw New Exception("Missing required CSV headers: " & String.Join(", ", missingHeaders) & ". Please ensure the CSV includes columns like 'Elevation' for unit names and 'Product' for type.")
            End If
            ' Parse rows with uppercased keys
            While Not parser.EndOfData
                Dim fields() As String = parser.ReadFields()
                If fields.Length < headers.Length Then Continue While ' Skip malformed rows
                Dim rowDict As New Dictionary(Of String, String)
                For i As Integer = 0 To Math.Min(fields.Length - 1, headers.Length - 1)
                    rowDict(headers(i).Trim().ToUpper()) = fields(i).Trim()
                Next
                If String.IsNullOrEmpty(rowDict(NAME_KEY.ToUpper())) Then Continue While ' Skip rows with empty unit name
                newRawData.Add(rowDict)
            End While
        End Using

        ' Fetch all RawUnits for the version
        Dim allRawUnits As List(Of RawUnitModel) = ProjectDataAccess.GetRawUnitsByVersionID(selectedVersionID)

        ' Mapping dialog
        Dim mappingForm As New Form With {
        .Text = "Map Imported Raw Units",
        .Size = New Size(600, 400)
    }
        Dim dgv As New DataGridView With {
        .Dock = DockStyle.Fill,
        .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
    }
        dgv.Columns.Add("NewName", "New RawUnit Name")
        Dim cmbCol As New DataGridViewComboBoxColumn With {
        .Name = "MapTo",
        .HeaderText = "Map To Existing or Create New"
    }
        cmbCol.Items.Add("Create New")
        For Each ex In allRawUnits
            cmbCol.Items.Add(ex.RawUnitName)
        Next
        dgv.Columns.Add(cmbCol)
        For Each nr In newRawData
            Dim rowIdx As Integer = dgv.Rows.Add()
            Dim csvUnitName As String
            If nr.ContainsKey("PRODUCT") AndAlso nr("PRODUCT").ToUpper().Contains("WALL") Then
                Dim planVal As String = If(nr.ContainsKey("PLAN"), nr("PLAN").Trim(), "")
                Dim elevVal As String = If(nr.ContainsKey("ELEVATION"), nr("ELEVATION").Trim(), "")
                csvUnitName = planVal & If(String.IsNullOrEmpty(planVal) OrElse String.IsNullOrEmpty(elevVal), "", " - ") & elevVal
                If String.IsNullOrEmpty(csvUnitName) Then csvUnitName = "Unnamed Wall Unit"
            Else
                csvUnitName = If(nr.ContainsKey("ELEVATION"), nr("ELEVATION").Trim(), "")
            End If
            dgv.Rows(rowIdx).Cells("NewName").Value = csvUnitName ' Display trimmed CSV value (PLAN - ELEVATION for walls)
            dgv.Rows(rowIdx).Tag = nr("ELEVATION").Trim() ' Store original ELEVATION for lookup
            Dim matchingUnit As RawUnitModel = allRawUnits.FirstOrDefault(Function(r) r.RawUnitName.Trim().ToUpper() = csvUnitName.ToUpper())
            If matchingUnit Is Nothing Then
                Dim duplicates As List(Of RawUnitModel) = allRawUnits.Where(Function(r) r.RawUnitName.Trim().ToUpper() = csvUnitName.ToUpper()).ToList()
                If duplicates.Count > 1 Then
                    MessageBox.Show("Multiple existing units found with name '" & csvUnitName & "'. Selecting first match.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                    matchingUnit = duplicates.First()
                End If
            End If
            Dim matchName As String = If(matchingUnit IsNot Nothing, matchingUnit.RawUnitName, "Create New")
            dgv.Rows(rowIdx).Cells("MapTo").Value = matchName
        Next

        Dim wasConfirmed As Boolean = False
        Dim buttonPanel As New Panel With {
        .Dock = DockStyle.Bottom,
        .Height = 40,
        .BackColor = Color.LightGray
    }
        Dim btnConfirm As New Button With {
        .Text = "Confirm Mapping",
        .Width = 100,
        .Location = New Point(10, 5)
    }
        AddHandler btnConfirm.Click, Sub(s, args)
                                         Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                             conn.Open()
                                             Using transaction As SqlTransaction = conn.BeginTransaction()
                                                 Try
                                                     For Each r As DataGridViewRow In dgv.Rows
                                                         If r.IsNewRow Then Continue For
                                                         Dim newName As String = CStr(r.Cells("NewName").Value)
                                                         Dim mapTo As String = CStr(r.Cells("MapTo").Value)
                                                         Dim lookupElev As String = CStr(r.Tag) ' Use stored ELEVATION for lookup
                                                         Dim rowData As Dictionary(Of String, String) = newRawData.FirstOrDefault(Function(d) d(NAME_KEY.ToUpper()).Trim() = lookupElev)
                                                         If rowData Is Nothing Then
                                                             MessageBox.Show("No matching row data found for " & newName & " (ELEVATION: " & lookupElev & ")", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                             Continue For
                                                         End If
                                                         Dim productType As String = rowData("PRODUCT".ToUpper())
                                                         Dim mappedProductTypeID As Integer
                                                         Select Case productType.ToUpper()
                                                             Case "FLOOR"
                                                                 mappedProductTypeID = 1
                                                             Case "ROOF"
                                                                 mappedProductTypeID = 2
                                                             Case "WALL"
                                                                 mappedProductTypeID = 3
                                                             Case Else
                                                                 MessageBox.Show("Invalid Product type '" & productType & "' for unit " & newName & ". Defaulting to ProductTypeID 1 (Floor).", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                                 mappedProductTypeID = 1
                                                         End Select
                                                         Dim model As New RawUnitModel With {
                                                         .VersionID = selectedVersionID,
                                                         .ProductTypeID = mappedProductTypeID,
                                                         .RawUnitName = newName
                                                     }
                                                         Dim decVal As Decimal
                                                         Dim upperKey As String
                                                         ' Required fields
                                                         upperKey = "BF".ToUpper()
                                                         model.BF = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "LF".ToUpper()
                                                         model.LF = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "EWPLF".ToUpper()
                                                         model.EWPLF = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "SQFT".ToUpper()
                                                         model.SqFt = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "FCAREA".ToUpper()
                                                         model.FCArea = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "LUMBERCOST".ToUpper()
                                                         model.LumberCost = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "PLATECOST".ToUpper()
                                                         model.PlateCost = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "MANUFLABORCOST".ToUpper()
                                                         model.ManufLaborCost = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "DESIGNLABOR".ToUpper()
                                                         model.DesignLabor = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "MGMTLABOR".ToUpper()
                                                         model.MGMTLabor = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "JOBSUPPLIESCOST".ToUpper()
                                                         model.JobSuppliesCost = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "MANHOURS".ToUpper()
                                                         model.ManHours = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "ITEMCOST".ToUpper()
                                                         model.ItemCost = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "OVERALLCOST".ToUpper()
                                                         model.OverallCost = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "DELIVERYCOST".ToUpper()
                                                         model.DeliveryCost = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "TOTALSELLPRICE".ToUpper()
                                                         model.TotalSellPrice = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "AVGSPFNO2".ToUpper()
                                                         model.AvgSPFNo2 = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         ' Optional fields for Component Export
                                                         upperKey = "SPFNO2BDFT".ToUpper()
                                                         model.SPFNo2BDFT = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "AVG2X4-1800".ToUpper()
                                                         model.Avg241800 = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "2X4-1800BDFT".ToUpper()
                                                         model.MSR241800BDFT = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "AVG2X4-2400".ToUpper()
                                                         model.Avg242400 = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "2X4-2400BDFT".ToUpper()
                                                         model.MSR242400BDFT = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "AVG2X6-1800".ToUpper()
                                                         model.Avg261800 = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "2X6-1800BDFT".ToUpper()
                                                         model.MSR261800BDFT = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "AVG2X6-2400".ToUpper()
                                                         model.Avg262400 = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)
                                                         upperKey = "2X6-2400BDFT".ToUpper()
                                                         model.MSR262400BDFT = If(rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal), CType(decVal, Decimal?), Nothing)

                                                         Try
                                                             If mapTo = "Create New" Then
                                                                 ' Insert new RawUnit
                                                                 Dim newRawUnitID As Integer = dataAccess.InsertRawUnit(model)
                                                                 model.RawUnitID = newRawUnitID
                                                                 ' Fetch the CostEffectiveDateID based on AvgSPFNo2
                                                                 Dim costEffectiveID As Object = DBNull.Value
                                                                 If model.AvgSPFNo2.HasValue Then
                                                                     Dim fetchParams As SqlParameter() = {New SqlParameter("@SPFLumberCost", model.AvgSPFNo2.Value)}
                                                                     costEffectiveID = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectCostEffectiveDateIDByCost, fetchParams)
                                                                 End If

                                                                 ' Insert initial RawUnitLumberHistory record
                                                                 Dim historyParams As New Dictionary(Of String, Object) From {
                                                                 {"@RawUnitID", model.RawUnitID},
                                                                 {"@VersionID", selectedVersionID},
                                                                 {"@CostEffectiveDateID", costEffectiveID},
                                                                 {"@LumberCost", If(model.LumberCost.HasValue, CType(model.LumberCost.Value, Object), DBNull.Value)},
                                                                 {"@AvgSPFNo2", If(model.AvgSPFNo2.HasValue, CType(model.AvgSPFNo2.Value, Object), DBNull.Value)},
                                                                 {"@Avg241800", If(model.Avg241800.HasValue, CType(model.Avg241800.Value, Object), DBNull.Value)},
                                                                 {"@Avg242400", If(model.Avg242400.HasValue, CType(model.Avg242400.Value, Object), DBNull.Value)},
                                                                 {"@Avg261800", If(model.Avg261800.HasValue, CType(model.Avg261800.Value, Object), DBNull.Value)},
                                                                 {"@Avg262400", If(model.Avg262400.HasValue, CType(model.Avg262400.Value, Object), DBNull.Value)}
                                                             }
                                                                 Dim cmdHistory As New SqlCommand(Queries.InsertRawUnitLumberHistory, conn, transaction) With {
                                                                 .CommandTimeout = 0
                                                             }
                                                                 cmdHistory.Parameters.AddRange(HelperDataAccess.BuildParameters(historyParams))
                                                                 Dim historyID As Integer = CInt(cmdHistory.ExecuteScalar())
                                                                 Debug.WriteLine("Created initial RawUnitLumberHistory for RawUnitID: " & model.RawUnitID & ", HistoryID: " & historyID)
                                                             Else
                                                                 Dim existing As RawUnitModel = allRawUnits.FirstOrDefault(Function(ru) ru.RawUnitName.Trim().ToUpper() = mapTo.Trim().ToUpper())
                                                                 If existing Is Nothing Then
                                                                     MessageBox.Show("Selected existing unit '" & mapTo & "' not found. Creating new unit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                                     Dim newRawUnitID As Integer = dataAccess.InsertRawUnit(model)
                                                                     model.RawUnitID = newRawUnitID
                                                                     ' Fetch the CostEffectiveDateID based on AvgSPFNo2
                                                                     Dim costEffectiveID As Object = DBNull.Value
                                                                     If model.AvgSPFNo2.HasValue Then
                                                                         Dim fetchParams As SqlParameter() = {New SqlParameter("@SPFLumberCost", model.AvgSPFNo2.Value)}
                                                                         costEffectiveID = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectCostEffectiveDateIDByCost, fetchParams)
                                                                     End If

                                                                     ' Insert initial RawUnitLumberHistory record
                                                                     Dim historyParams As New Dictionary(Of String, Object) From {
                                                                     {"@RawUnitID", model.RawUnitID},
                                                                     {"@VersionID", selectedVersionID},
                                                                     {"@CostEffectiveDateID", costEffectiveID},
                                                                     {"@LumberCost", If(model.LumberCost.HasValue, CType(model.LumberCost.Value, Object), DBNull.Value)},
                                                                     {"@AvgSPFNo2", If(model.AvgSPFNo2.HasValue, CType(model.AvgSPFNo2.Value, Object), DBNull.Value)},
                                                                     {"@Avg241800", If(model.Avg241800.HasValue, CType(model.Avg241800.Value, Object), DBNull.Value)},
                                                                     {"@Avg242400", If(model.Avg242400.HasValue, CType(model.Avg242400.Value, Object), DBNull.Value)},
                                                                     {"@Avg261800", If(model.Avg261800.HasValue, CType(model.Avg261800.Value, Object), DBNull.Value)},
                                                                     {"@Avg262400", If(model.Avg241800.HasValue, CType(model.Avg241800.Value, Object), DBNull.Value)}
                                                                 }
                                                                     Dim cmdHistory As New SqlCommand(Queries.InsertRawUnitLumberHistory, conn, transaction) With {
                                                                     .CommandTimeout = 0
                                                                 }
                                                                     cmdHistory.Parameters.AddRange(HelperDataAccess.BuildParameters(historyParams))
                                                                     Dim historyID As Integer = CInt(cmdHistory.ExecuteScalar())
                                                                     Debug.WriteLine("Created initial RawUnitLumberHistory for RawUnitID: " & model.RawUnitID & ", HistoryID: " & historyID)
                                                                 Else
                                                                     model.RawUnitID = existing.RawUnitID
                                                                     dataAccess.UpdateRawUnit(model)
                                                                     ' Fetch the CostEffectiveDateID based on AvgSPFNo2
                                                                     Dim costEffectiveID As Object = DBNull.Value
                                                                     If model.AvgSPFNo2.HasValue Then
                                                                         Dim fetchParams As SqlParameter() = {New SqlParameter("@SPFLumberCost", model.AvgSPFNo2.Value)}
                                                                         costEffectiveID = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectCostEffectiveDateIDByCost, fetchParams)
                                                                     End If

                                                                     ' Insert initial RawUnitLumberHistory record
                                                                     Dim historyParams As New Dictionary(Of String, Object) From {
                                                                     {"@RawUnitID", model.RawUnitID},
                                                                     {"@VersionID", selectedVersionID},
                                                                     {"@CostEffectiveDateID", costEffectiveID},
                                                                     {"@LumberCost", If(model.LumberCost.HasValue, CType(model.LumberCost.Value, Object), DBNull.Value)},
                                                                     {"@AvgSPFNo2", If(model.AvgSPFNo2.HasValue, CType(model.AvgSPFNo2.Value, Object), DBNull.Value)},
                                                                     {"@Avg241800", If(model.Avg241800.HasValue, CType(model.Avg241800.Value, Object), DBNull.Value)},
                                                                     {"@Avg242400", If(model.Avg242400.HasValue, CType(model.Avg242400.Value, Object), DBNull.Value)},
                                                                     {"@Avg261800", If(model.Avg261800.HasValue, CType(model.Avg261800.Value, Object), DBNull.Value)},
                                                                     {"@Avg262400", If(model.Avg241800.HasValue, CType(model.Avg241800.Value, Object), DBNull.Value)}
                                                                 }
                                                                     Dim cmdHistory As New SqlCommand(Queries.InsertRawUnitLumberHistory, conn, transaction) With {
                                                                     .CommandTimeout = 0
                                                                 }
                                                                     cmdHistory.Parameters.AddRange(HelperDataAccess.BuildParameters(historyParams))
                                                                     Dim historyID As Integer = CInt(cmdHistory.ExecuteScalar())
                                                                     Debug.WriteLine("Created initial RawUnitLumberHistory for RawUnitID: " & model.RawUnitID & ", HistoryID: " & historyID)
                                                                 End If
                                                             End If
                                                         Catch ex As Exception
                                                             MessageBox.Show("Error processing unit '" & newName & "': " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                         End Try
                                                     Next
                                                     transaction.Commit()
                                                     wasConfirmed = True
                                                     mappingForm.Close()
                                                 Catch ex As Exception
                                                     transaction.Rollback()
                                                     MessageBox.Show("Error saving mapped units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                 End Try
                                             End Using
                                         End Using
                                     End Sub
        Dim btnCancel As New Button With {
        .Text = "Cancel",
        .Width = 100
    }
        btnCancel.Location = New Point(buttonPanel.Width - btnCancel.Width + 50, 5)
        AddHandler btnCancel.Click, Sub(s, args)
                                        wasConfirmed = False
                                        mappingForm.Close()
                                    End Sub
        buttonPanel.Controls.Add(btnConfirm)
        buttonPanel.Controls.Add(btnCancel)
        mappingForm.Controls.Add(dgv)
        mappingForm.Controls.Add(buttonPanel)
        mappingForm.ShowDialog()
        Return wasConfirmed
    End Function

    ' Add in frmPSE.vb: New subroutine to recalculate components for all affected ActualUnits post-import, ensuring data consistency across versions.
    Private Sub RecalculateAllActualUnits(versionID As Integer)
        Dim actualUnits As List(Of ActualUnitModel) = ProjectDataAccess.GetActualUnitsByVersion(versionID).ToList()
        For Each actual In actualUnits
            Dim rawUnit As RawUnitModel = ProjectDataAccess.GetRawUnitByID(actual.RawUnitID)
            If rawUnit IsNot Nothing Then
                actual.CalculatedComponents = CalculateComponentsFromRaw(rawUnit, Me.selectedVersionID)
                dataAccess.SaveCalculatedComponents(actual)
            End If
        Next
    End Sub

    Private Sub BtnConvertToActualUnit_Click(sender As Object, e As EventArgs) Handles BtnConvertToActualUnit.Click
        Try
            If ListBoxAvailableUnits.SelectedIndex < 0 Then
                UpdateStatus($"Status: No raw unit selected for conversion in version {selectedVersionID}.")
                MessageBox.Show("Please select a raw unit to convert.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            selectedRawUnitID = rawUnits(ListBoxAvailableUnits.SelectedIndex).RawUnitID
            Dim rawUnit As RawUnitModel = rawUnits(ListBoxAvailableUnits.SelectedIndex)
            Dim newActualUnit As New ActualUnitModel With {
            .RawUnitID = rawUnit.RawUnitID,
            .ReferencedRawUnitName = rawUnit.RawUnitName,
            .UnitName = rawUnit.RawUnitName,
            .PlanSQFT = If(rawUnit.SqFt, 0D),
            .UnitType = "Res",
            .OptionalAdder = 1D,
            .ProductTypeID = rawUnit.ProductTypeID
        }
            currentdetailsMode = DetailsMode.NewUnit
            PopulateDetailsPane(newActualUnit, False, 1)
            TxtUnitName.Text = String.Empty  ' Clear for user to enter actual unit name
            TxtUnitName.Focus()  ' Set focus for immediate typing
            UpdateStatus($"Status: Converting raw unit {rawUnit.RawUnitName} to actual unit for version {selectedVersionID}.")
        Catch ex As Exception
            UpdateStatus($"Status: Error converting raw unit for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error converting raw unit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnReuseActualUnit_Click(sender As Object, e As EventArgs) Handles btnReuseActualUnit.Click
        Try
            If ListboxExistingActualUnits.SelectedIndex < 0 Then
                UpdateStatus($"Status: Please select an actual unit to reuse for version {selectedVersionID}.")
                MessageBox.Show("Please select an existing actual unit from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            selectedActualUnitID = existingActualUnits(ListboxExistingActualUnits.SelectedIndex).ActualUnitID
            Dim selectedActual As ActualUnitModel = existingActualUnits(ListboxExistingActualUnits.SelectedIndex)
            If selectedActual Is Nothing Then
                UpdateStatus($"Status: Selected actual unit not found for version {selectedVersionID}.")
                MessageBox.Show("Selected actual unit not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            currentdetailsMode = DetailsMode.ReuseUnit
            PopulateDetailsPane(selectedActual, False, 1)
            UpdateStatus($"Status: Reusing actual unit {selectedActual.UnitName} for version {selectedVersionID}.")
        Catch ex As Exception
            UpdateStatus($"Status: Error reusing unit for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error reusing unit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PopulateDetailsPane(actual As ActualUnitModel, isEdit As Boolean, quantity As Integer)
        Try
            ' Set mode based on entry point
            If isEdit Then
                currentdetailsMode = DetailsMode.EditGlobalUnit
            ElseIf currentdetailsMode = DetailsMode.ReuseUnit Then
                ' Already set in caller
            ElseIf currentdetailsMode = DetailsMode.EditMappingQuantity Then
                ' Already set in caller
            Else
                currentdetailsMode = DetailsMode.NewUnit
            End If

            ' Configure UI based on mode
            TxtUnitName.Text = actual.UnitName
            TxtPlanSQFT.Text = actual.PlanSQFT.ToString()
            TxtOptionalAdder.Text = actual.OptionalAdder.ToString()
            CmbUnitType.SelectedItem = actual.UnitType
            txtColorCode.Text = actual.ColorCode
            TxtReferencedRawUnit.Text = actual.ReferencedRawUnitName
            TxtActualUnitQuantity.Text = quantity.ToString()

            ' Enable/disable fields based on mode
            TxtUnitName.Enabled = currentdetailsMode = DetailsMode.NewUnit OrElse currentdetailsMode = DetailsMode.EditGlobalUnit
            TxtPlanSQFT.Enabled = currentdetailsMode = DetailsMode.NewUnit OrElse currentdetailsMode = DetailsMode.EditGlobalUnit
            TxtOptionalAdder.Enabled = currentdetailsMode = DetailsMode.NewUnit OrElse currentdetailsMode = DetailsMode.EditGlobalUnit
            txtColorCode.Enabled = currentdetailsMode = DetailsMode.NewUnit OrElse currentdetailsMode = DetailsMode.EditGlobalUnit
            CmbUnitType.Enabled = currentdetailsMode = DetailsMode.NewUnit OrElse currentdetailsMode = DetailsMode.EditGlobalUnit
            TxtReferencedRawUnit.Enabled = False
            TxtActualUnitQuantity.Enabled = currentdetailsMode = DetailsMode.ReuseUnit OrElse currentdetailsMode = DetailsMode.EditMappingQuantity

            ' Explicit reset for delete button
            btnDeleteUnit.Visible = False
            btnDeleteUnit.Text = "Delete"

            chkAttachToLevel.Visible = False
            chkAttachToLevel.Checked = False

            ' Set button text
            Select Case currentdetailsMode
                Case DetailsMode.NewUnit
                    btnSave.Text = "Save New Unit"
                    chkAttachToLevel.Visible = True  ' Show option in NewUnit mode
                Case DetailsMode.EditGlobalUnit
                    btnSave.Text = "Save Unit Changes"
                    btnDeleteUnit.Text = "Delete Unit"
                    btnDeleteUnit.Visible = True
                Case DetailsMode.ReuseUnit
                    btnSave.Text = "Save and Attach"
                Case DetailsMode.EditMappingQuantity
                    btnSave.Text = "Save Quantity"
                    btnDeleteUnit.Text = "Delete Mapping"
                    btnDeleteUnit.Visible = True
            End Select

            ' Update per-SQFT from components or raw
            If actual.CalculatedComponents IsNot Nothing AndAlso actual.CalculatedComponents.Any() Then
                UpdatePerSQFTFields(actual.CalculatedComponents)
            ElseIf actual.RawUnitID > 0 Then
                Dim rawUnit = ProjectDataAccess.GetRawUnitByID(actual.RawUnitID)
                If rawUnit IsNot Nothing Then UpdatePerSQFTFields(CalculateComponentsFromRaw(rawUnit, Me.selectedVersionID))
            End If

            PanelDetails.Visible = True
            BtnToggleDetails.Text = "Hide Details"
            UpdateStatus($"Status: Details pane opened in {currentdetailsMode} mode for version {selectedVersionID}.")
        Catch ex As Exception
            UpdateStatus($"Status: Error opening details pane for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error opening details pane: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
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


    Private Sub HandleGlobalUnitEdit()
        Try
            If selectedActualUnitID <= 0 Then
                UpdateStatus($"Status: Invalid unit selection for version {selectedVersionID}.")
                MessageBox.Show("Invalid unit selection for edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim sqft As Decimal
            Dim adder As Decimal
            If Not Decimal.TryParse(TxtPlanSQFT.Text, sqft) OrElse sqft <= 0 OrElse
               Not Decimal.TryParse(TxtOptionalAdder.Text, adder) OrElse adder < 1 Then
                UpdateStatus($"Status: Invalid input for version {selectedVersionID}—SQFT (>0) and Adder (≥1) must be valid.")
                MessageBox.Show("Invalid input—SQFT (>0) and Adder (≥1) must be valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim updatedUnit As ActualUnitModel = ProjectDataAccess.GetActualUnitByID(selectedActualUnitID)
            If updatedUnit Is Nothing Then
                UpdateStatus($"Status: Actual unit not found for version {selectedVersionID}.")
                MessageBox.Show("Actual unit not found for update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            updatedUnit.VersionID = selectedVersionID
            updatedUnit.UnitName = TxtUnitName.Text
            updatedUnit.PlanSQFT = sqft
            updatedUnit.OptionalAdder = adder
            updatedUnit.ColorCode = txtColorCode.Text
            updatedUnit.UnitType = If(CmbUnitType.SelectedItem?.ToString(), "Res")

            dataAccess.SaveActualUnit(updatedUnit)
            Dim rawUnit = ProjectDataAccess.GetRawUnitByID(updatedUnit.RawUnitID)
            If rawUnit IsNot Nothing Then
                updatedUnit.CalculatedComponents = CalculateComponentsFromRaw(rawUnit, Me.selectedVersionID)
                dataAccess.SaveCalculatedComponents(updatedUnit)
            End If

            LoadExistingActualUnits()
            UpdateStatus($"Status: Actual unit {updatedUnit.UnitName} updated successfully for version {selectedVersionID}.")
        Catch ex As Exception
            UpdateStatus($"Status: Error updating actual unit for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error updating actual unit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub HandleEditMappingQuantity(quantity As Integer)
        Try
            If selectedMappingID <= 0 Then
                UpdateStatus($"Status: Invalid mapping ID for version {selectedVersionID}.")
                MessageBox.Show("Invalid mapping selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            Dim mapping As ActualToLevelMappingModel = ProjectDataAccess.GetActualToLevelMappingsByLevelID(selectedLevelID).FirstOrDefault(Function(m) m.MappingID = selectedMappingID)
            If mapping Is Nothing Then
                UpdateStatus($"Status: Mapping not found for ID {selectedMappingID} in version {selectedVersionID}.")
                MessageBox.Show("Mapping not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            mapping.Quantity = quantity
            dataAccess.SaveActualToLevelMapping(mapping)
            LoadAssignedUnits()
            RollupDataAccess.UpdateLevelRollups(selectedLevelID)
            If selectedBuildingID > 0 Then RollupDataAccess.UpdateBuildingRollups(selectedBuildingID)
            UpdateStatus($"Status: Mapping quantity updated for unit {mapping.ActualUnit.UnitName} in version {selectedVersionID}.")
        Catch ex As Exception
            UpdateStatus($"Status: Error updating mapping quantity for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error updating mapping quantity: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub HandleEditMapping(qty As Integer)
        If selectedMappingID <= 0 Then
            MessageBox.Show("Error: No valid mapping selected for update.")
            Return
        End If

        Dim mapping As New ActualToLevelMappingModel With {
            .MappingID = selectedMappingID,
            .VersionID = selectedVersionID,
            .ActualUnitID = selectedActualUnitID,
            .LevelID = selectedLevelID,
            .Quantity = qty
        }
        dataAccess.SaveActualToLevelMapping(mapping)
        UpdateStatus("Unit quantity updated successfully for version " & selectedVersionID & ".")
    End Sub

    Private Sub HandleNewUnit(qty As Integer)
        Dim actualUnit As ActualUnitModel = CreateAndSaveActualUnit()
        If actualUnit Is Nothing Then Return

        Dim mapping As New ActualToLevelMappingModel With {
            .VersionID = selectedVersionID,
            .ActualUnitID = actualUnit.ActualUnitID,
            .LevelID = selectedLevelID,
            .Quantity = qty
        }
        dataAccess.SaveActualToLevelMapping(mapping)
        UpdateStatus("Unit saved and attached successfully for version " & selectedVersionID & ".")
    End Sub

    Private Sub HandleReuseMapping(qty As Integer)
        Dim mapping As New ActualToLevelMappingModel With {
            .VersionID = selectedVersionID,
            .ActualUnitID = selectedActualUnitID,
            .LevelID = selectedLevelID,
            .Quantity = qty
        }
        dataAccess.SaveActualToLevelMapping(mapping)
        UpdateStatus("Existing unit reused and mapped successfully for version " & selectedVersionID & ".")
    End Sub

    Private Sub btnDeleteUnit_Click(sender As Object, e As EventArgs) Handles btnDeleteUnit.Click
        Try
            Select Case currentdetailsMode
                Case DetailsMode.EditGlobalUnit
                    ' Existing global delete logic (assume from prior: dataAccess.DeleteActualUnit(selectedActualUnitID), remove from lists, refresh)
                    Dim selectedActual As ActualUnitModel = ProjectDataAccess.GetActualUnitByID(selectedActualUnitID)
                    If selectedActual Is Nothing Then Return
                    Dim mappings = dataAccess.GetActualToLevelMappingsByActualUnitID(selectedActual.ActualUnitID)
                    If mappings.Any() Then
                        Dim result = MessageBox.Show($"The unit '{selectedActual.UnitName}' is used in {mappings.Count} level(s). Deleting it will remove it from all levels. Continue?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        If result <> DialogResult.Yes Then Return
                    End If
                    dataAccess.DeleteActualUnit(selectedActual.ActualUnitID)
                    existingActualUnits.RemoveAll(Function(u) u.ActualUnitID = selectedActualUnitID)
                    ListboxExistingActualUnits.Items.Clear()
                    For Each unit In existingActualUnits
                        ListboxExistingActualUnits.Items.Add(unit.UnitName & " (" & unit.UnitType & ")")
                    Next
                    If selectedLevelID > 0 Then LoadAssignedUnits()
                    UpdateStatus($"Status: Actual unit deleted for version {selectedVersionID}.")

                Case DetailsMode.EditMappingQuantity
                    If selectedMappingID <= 0 Then
                        MessageBox.Show("Invalid mapping selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                    Dim result = MessageBox.Show("Delete this unit mapping from the level?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    If result <> DialogResult.Yes Then Return
                    dataAccess.DeleteActualToLevelMapping(selectedMappingID)
                    LoadAssignedUnits()
                    RollupDataAccess.UpdateLevelRollups(selectedLevelID)
                    If selectedBuildingID > 0 Then RollupDataAccess.UpdateBuildingRollups(selectedBuildingID)
                    ResetDetailsPane()
                    UpdateStatus($"Status: Mapping deleted for version {selectedVersionID}.")

                Case Else
                    MessageBox.Show("Delete not supported in this mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select
        Catch ex As Exception
            UpdateStatus($"Status: Error deleting for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error deleting: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ResetDetailsPane()
        Try
            chkAttachToLevel.Checked = False
            chkAttachToLevel.Visible = False
            TxtUnitName.Text = String.Empty
            TxtPlanSQFT.Text = String.Empty
            TxtOptionalAdder.Text = String.Empty
            CmbUnitType.SelectedIndex = -1
            TxtReferencedRawUnit.Text = String.Empty
            TxtActualUnitQuantity.Text = String.Empty
            btnDeleteUnit.Text = "Delete"
            btnDeleteUnit.Visible = False
            selectedActualUnitID = -1
            selectedMappingID = -1
            currentdetailsMode = DetailsMode.None
            btnSave.Text = "Save"
            PanelDetails.Visible = False
            BtnToggleDetails.Text = "Show Details"
            UpdateStatus($"Status: Details pane reset for version {selectedVersionID}.")
        Catch ex As Exception
            UpdateStatus($"Status: Error resetting details pane for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error resetting details pane: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnRecalculate_Click(sender As Object, e As EventArgs) Handles BtnRecalculate.Click
        Try
            If selectedLevelID <= 0 Then
                UpdateStatus("Status: Please select a level to recalculate for version " & selectedVersionID & ".")
                MessageBox.Show("Please select a level before recalculating.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            LoadAssignedUnits()
            RollupDataAccess.UpdateLevelRollups(selectedLevelID)
            If selectedBuildingID > 0 Then RollupDataAccess.UpdateBuildingRollups(selectedBuildingID)
            If MsgBox("Recalculate entire project for version " & selectedVersionID & "?", MsgBoxStyle.YesNo, "Confirm") = MsgBoxResult.Yes Then
                RecalculateAllActualUnits(selectedVersionID)
                RecalculateProject()
            End If
            UpdateStatus("Status: Recalculated totals successfully for version " & selectedVersionID & ".")
            MessageBox.Show("Recalculated totals successfully for version " & selectedVersionID & ".", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            UpdateStatus("Status: Error recalculating for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error recalculating: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub RecalculateProject()
        Try
            RollupDataAccess.RecalculateVersion(selectedVersionID)
            LoadAssignedUnits()
            UpdateStatus("Status: Project recalculated successfully for version " & selectedVersionID & ".")
        Catch ex As Exception
            UpdateStatus("Status: Error recalculating project for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error recalculating project: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub DataGridViewAssigned_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewAssigned.CellContentClick
        Try
            If e.ColumnIndex = colActions.Index AndAlso e.RowIndex >= 0 Then
                Dim selectedUnit As DisplayUnitData = TryCast(bindingSource.Current, DisplayUnitData)
                If selectedUnit Is Nothing Then
                    UpdateStatus($"Status: No unit selected for editing in version {selectedVersionID}.")
                    MessageBox.Show("No unit selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                selectedMappingID = selectedUnit.MappingID
                selectedActualUnitID = selectedUnit.ActualUnit.ActualUnitID
                currentdetailsMode = DetailsMode.EditMappingQuantity
                PopulateDetailsPane(selectedUnit.ActualUnit, False, selectedUnit.ActualUnitQuantity)
                UpdateStatus($"Status: Editing mapping quantity for unit {selectedUnit.UnitName} in version {selectedVersionID}.")
            End If
        Catch ex As Exception
            UpdateStatus($"Status: Error in cell click for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error in cell click: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadExistingActualUnits()
        ListboxExistingActualUnits.Items.Clear()
        Try
            existingActualUnits = ProjectDataAccess.GetActualUnitsByVersion(selectedVersionID)
            If Not existingActualUnits.Any() Then
                UpdateStatus("Status: No actual units found for version " & selectedVersionID & ".")
                Return
            End If
            For Each actual In existingActualUnits
                Dim sellPricePerSQFT As Decimal = 0D
                Dim sellPriceComponent = actual.CalculatedComponents?.FirstOrDefault(Function(c) c.ComponentType = "SellPrice/SQFT")
                If sellPriceComponent IsNot Nothing Then
                    sellPricePerSQFT = sellPriceComponent.Value
                End If
                ListboxExistingActualUnits.Items.Add($"{actual.UnitName} ({actual.UnitType}) - {sellPricePerSQFT:C2}/SQFT")
            Next
            UpdateStatus("Status: Actual units loaded for version " & selectedVersionID & ".")
        Catch ex As Exception
            UpdateStatus("Status: Error loading actual units for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error loading actual units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ListBoxAvailableUnits_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBoxAvailableUnits.SelectedIndexChanged
        Try
            If ListBoxAvailableUnits.SelectedIndex < 0 Then
                UpdateStatus($"Status: No raw unit selected for version {selectedVersionID}.")
                UpdateSelectedRawPreview()
                Return
            End If
            UpdateSelectedRawPreview()
        Catch ex As Exception
            UpdateStatus($"Status: Error updating raw unit preview for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error updating raw unit preview: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuCopyUnits_Click(sender As Object, e As EventArgs) Handles mnuCopyUnits.Click
        If selectedLevelID <= 0 Then
            UpdateStatus("Status: Please select a level to copy units for version " & selectedVersionID & ".")
            MessageBox.Show("Please select a level to copy units.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Try
            Dim mappings = ProjectDataAccess.GetActualToLevelMappingsByLevelID(selectedLevelID)
            copiedMappings.Clear()
            For Each mapping In mappings
                copiedMappings.Add(New Tuple(Of Integer, Integer)(mapping.ActualUnitID, mapping.Quantity))
            Next
            Dim levelTag = DirectCast(TreeViewLevels.SelectedNode.Tag, Dictionary(Of String, Object))
            sourceProductTypeID = CInt(levelTag("ProductTypeID"))
            UpdateStatus($"Status: Copied {copiedMappings.Count} units from level for version {selectedVersionID}. Ready to paste.")
        Catch ex As Exception
            UpdateStatus("Status: Error copying units for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error copying units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuPasteUnits_Click(sender As Object, e As EventArgs) Handles mnuPasteUnits.Click
        If selectedLevelID <= 0 Then
            UpdateStatus("Status: Please select a target level for version " & selectedVersionID & ".")
            MessageBox.Show("Please select a target level to paste units.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        If copiedMappings.Count = 0 Then
            UpdateStatus("Status: No units copied for version " & selectedVersionID & ".")
            MessageBox.Show("No units copied to paste.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Try
            Dim levelTag = DirectCast(TreeViewLevels.SelectedNode.Tag, Dictionary(Of String, Object))
            Dim targetProductTypeID As Integer = CInt(levelTag("ProductTypeID"))
            If targetProductTypeID <> sourceProductTypeID Then
                UpdateStatus("Status: Source and target levels must have the same Product Type for version " & selectedVersionID & ".")
                MessageBox.Show("Cannot paste: Source and target levels must have the same Product Type.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim existingMappings = ProjectDataAccess.GetActualToLevelMappingsByLevelID(selectedLevelID)
            For Each copied In copiedMappings
                Dim actualUnitID As Integer = copied.Item1
                Dim quantity As Integer = copied.Item2

                Dim existing = existingMappings.FirstOrDefault(Function(m) m.ActualUnitID = actualUnitID)
                If existing IsNot Nothing Then
                    existing.Quantity = quantity  ' Changed from += to = for consistent paste behavior
                    dataAccess.SaveActualToLevelMapping(existing)
                Else
                    Dim newMapping As New ActualToLevelMappingModel With {
            .VersionID = selectedVersionID,
            .ActualUnitID = actualUnitID,
            .LevelID = selectedLevelID,
            .Quantity = quantity
        }
                    dataAccess.SaveActualToLevelMapping(newMapping)
                End If
            Next

            LoadAssignedUnits()
            RollupDataAccess.UpdateLevelRollups(selectedLevelID)
            If selectedBuildingID > 0 Then RollupDataAccess.UpdateBuildingRollups(selectedBuildingID)
            copiedMappings.Clear()
            UpdateStatus("Status: Units pasted successfully for version " & selectedVersionID & ".")
        Catch ex As Exception
            UpdateStatus("Status: Error pasting units for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error pasting units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnToggleDetails_Click(sender As Object, e As EventArgs) Handles BtnToggleDetails.Click
        Try
            PanelDetails.Visible = Not PanelDetails.Visible
            BtnToggleDetails.Text = If(PanelDetails.Visible, "Hide Details", "Show Details")
            UpdateStatus($"Status: Details panel {(If(PanelDetails.Visible, "shown", "hidden"))} for version {selectedVersionID}.")
        Catch ex As Exception
            UpdateStatus($"Status: Error toggling details panel for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error toggling details panel: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        Try
            ResetDetailsPane()
            btnDeleteUnit.Visible = False
            UpdateStatus($"Status: Details pane reset for version {selectedVersionID}.")
        Catch ex As Exception
            UpdateStatus($"Status: Error resetting details pane for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error resetting details pane: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnFinish_Click(sender As Object, e As EventArgs) Handles BtnFinish.Click
        Try
            UpdateStatus($"Status: Session complete for version {selectedVersionID} at {DateTime.Now:HH:mm:ss}.")
            RemoveTabFromTabControl($"PSE_{selectedProjectID}_{selectedVersionID}")
        Catch ex As Exception
            UpdateStatus($"Status: Error closing form for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error closing form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub FrmPSE_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        copiedMappings.Clear()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        If String.IsNullOrEmpty(TxtUnitName.Text) AndAlso currentdetailsMode <> DetailsMode.EditMappingQuantity Then
            MessageBox.Show("Actual Unit Name must be valid.")
            Return
        End If
        Try
            Select Case currentdetailsMode
                Case DetailsMode.EditGlobalUnit
                    HandleGlobalUnitEdit()
                Case DetailsMode.ReuseUnit
                    HandleReuseMapping(CInt(TxtActualUnitQuantity.Text))
                    LoadAssignedUnits()  ' Refresh grid after attaching to level
                Case DetailsMode.EditMappingQuantity
                    HandleEditMappingQuantity(CInt(TxtActualUnitQuantity.Text))
                Case DetailsMode.NewUnit
                    Dim actualUnit As ActualUnitModel = CreateAndSaveActualUnit()
                    If actualUnit Is Nothing Then Return
                    existingActualUnits.Add(actualUnit)
                    ListboxExistingActualUnits.Items.Add(actualUnit.UnitName & " (" & actualUnit.UnitType & ")")
                    UpdateStatus("New unit saved successfully—available for reuse.")
                    ' Optional attach based on user choice
                    If chkAttachToLevel.Checked AndAlso selectedLevelID > 0 AndAlso CInt(TxtActualUnitQuantity.Text) > 0 Then
                        Dim newMapping As New ActualToLevelMappingModel With {
                        .VersionID = selectedVersionID,
                        .ActualUnitID = actualUnit.ActualUnitID,
                        .LevelID = selectedLevelID,
                        .Quantity = CInt(TxtActualUnitQuantity.Text)
                    }
                        dataAccess.SaveActualToLevelMapping(newMapping)
                        LoadAssignedUnits()  ' Refresh grid if attached
                        RollupDataAccess.UpdateLevelRollups(selectedLevelID)
                        If selectedBuildingID > 0 Then RollupDataAccess.UpdateBuildingRollups(selectedBuildingID)
                    End If
                Case Else
                    UpdateStatus($"Status: Invalid save mode for version {selectedVersionID}.")
                    MessageBox.Show("Invalid operation mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
            End Select
            ResetDetailsPane()
            currentdetailsMode = DetailsMode.None
        Catch ex As Exception
            UpdateStatus($"Status: Error saving for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error saving: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ListboxExistingActualUnits_MouseDown(sender As Object, e As MouseEventArgs) Handles ListboxExistingActualUnits.MouseDown
        Try
            If e.Button = MouseButtons.Right AndAlso ListboxExistingActualUnits.SelectedIndex >= 0 Then
                If ListboxExistingActualUnits.ContextMenuStrip Is Nothing Then
                    UpdateStatus($"Status: Context menu not configured for version {selectedVersionID}.")
                    MessageBox.Show("Context menu not configured.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                ListboxExistingActualUnits.ContextMenuStrip.Show(ListboxExistingActualUnits, e.Location)
                UpdateStatus($"Status: Context menu opened for version {selectedVersionID}.")
            ElseIf e.Button = MouseButtons.Right Then
                UpdateStatus($"Status: Please select an actual unit for version {selectedVersionID} to show context menu.")
                MessageBox.Show("Please select an actual unit to show the context menu.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        Catch ex As Exception
            UpdateStatus($"Status: Error opening context menu for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error opening context menu: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub EditToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditToolStripMenuItem.Click, ListboxExistingActualUnits.DoubleClick
        Try
            If ListboxExistingActualUnits.SelectedIndex < 0 Then
                UpdateStatus("Status: Please select an actual unit to edit for version " & selectedVersionID & ".")
                MessageBox.Show("Please select an existing actual unit from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            selectedActualUnitID = existingActualUnits(ListboxExistingActualUnits.SelectedIndex).ActualUnitID
            Dim selectedActual As ActualUnitModel = existingActualUnits(ListboxExistingActualUnits.SelectedIndex)
            If selectedActual Is Nothing Then
                UpdateStatus("Status: Selected actual unit not found for version " & selectedVersionID & ".")
                MessageBox.Show("Selected actual unit not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            currentdetailsMode = DetailsMode.EditGlobalUnit
            PopulateDetailsPane(selectedActual, True, 0)
            UpdateStatus("Status: Editing actual unit " & selectedActual.UnitName & " for version " & selectedVersionID & ".")
        Catch ex As Exception
            UpdateStatus("Status: Error editing actual unit for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error editing actual unit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DeleteActualUnitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteActualUnitToolStripMenuItem.Click
        Try
            If ListboxExistingActualUnits.SelectedIndex < 0 Then
                UpdateStatus("Status: Please select an actual unit to delete for version " & selectedVersionID & ".")
                MessageBox.Show("Please select an existing actual unit from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim selectedActual As ActualUnitModel = existingActualUnits(ListboxExistingActualUnits.SelectedIndex)
            If selectedActual Is Nothing Then
                UpdateStatus("Status: Selected actual unit not found for version " & selectedVersionID & ".")
                MessageBox.Show("Selected actual unit not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim mappings = dataAccess.GetActualToLevelMappingsByActualUnitID(selectedActual.ActualUnitID)
            If mappings.Any() Then
                Dim result = MessageBox.Show($"The unit '{selectedActual.UnitName}' is used in {mappings.Count} level(s) for version {selectedVersionID}. Deleting it will remove it from all levels. Continue?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                If result <> DialogResult.Yes Then Return
            End If

            dataAccess.DeleteActualUnit(selectedActual.ActualUnitID)
            existingActualUnits.RemoveAt(ListboxExistingActualUnits.SelectedIndex)
            ListboxExistingActualUnits.Items.RemoveAt(ListboxExistingActualUnits.SelectedIndex)
            If selectedLevelID > 0 Then LoadAssignedUnits() ' Refresh grid if unit was mapped
            UpdateStatus($"Status: Actual unit '{selectedActual.UnitName}' deleted successfully for version {selectedVersionID}.")
        Catch ex As ApplicationException
            UpdateStatus($"Status: Deletion blocked for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show(ex.Message, "Deletion Blocked", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            UpdateStatus($"Status: Error deleting unit for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error deleting unit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' In frmCreateEditProject.vb: Update UpdateStatus for robustness
    Private Sub UpdateStatus(message As String)
        Try
            Dim parentForm As frmMain = TryCast(Me.ParentForm, frmMain)
            If parentForm IsNot Nothing AndAlso parentForm.StatusLabel IsNot Nothing Then
                parentForm.StatusLabel.Text = $"{message} at {DateTime.Now:HH:mm:ss}"
            Else
                Debug.WriteLine($"Status update skipped: Parent form or StatusLabel is null. Message: {message}")
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error updating status: {ex.Message}")
        End Try
    End Sub

    Private Sub btnPickColor_Click(sender As Object, e As EventArgs) Handles btnPickColor.Click
        Try
            Using colorDialog As New ColorDialog
                ' Configure ColorDialog
                colorDialog.AnyColor = True
                colorDialog.AllowFullOpen = True
                colorDialog.SolidColorOnly = False

                If colorDialog.ShowDialog() = DialogResult.OK Then
                    Dim selectedColor As Color = colorDialog.Color
                    Dim hexColor As String = String.Format("{0:X2}{1:X2}{2:X2}", selectedColor.R, selectedColor.G, selectedColor.B)
                    txtColorCode.Text = hexColor
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show("Error selecting color: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

    End Sub


End Class