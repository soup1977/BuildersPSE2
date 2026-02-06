Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Services
Imports BuildersPSE2.Utilities
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

    Private sourceBuildingStructure As New List(Of Integer)   ' ProductTypeID sequence, sorted by LevelNumber
    Private copiedBuildingMappings As New List(Of List(Of Tuple(Of Integer, Integer))) '

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
    Private colColorCode As DataGridViewColumn
    Private colActions As DataGridViewColumn

    Private copiedMappings As New List(Of Tuple(Of Integer, Integer)) ' (ActualUnitID, Quantity)
    Private sourceProductTypeID As Integer = -1
    ' Add these class-level fields near the other Private fields (around line 30)
    Private dragStartIndex As Integer = -1
    Private isDragging As Boolean = False

    Private ReadOnly _mainForm As frmMain = CType(Application.OpenForms.OfType(Of frmMain)().FirstOrDefault(), frmMain)


    Private unitNameColumnIndex As Integer = -1
    Private colorCodeColumnIndex As Integer = -1


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

        ' Enable double buffering to reduce flicker
        EnableDoubleBuffering(DataGridViewAssigned)

        DataGridViewAssigned.AutoGenerateColumns = False

        ' Don't pre-set DataSource type - it will be set when data loads
        ' bindingSource.DataSource = GetType(DisplayUnitData)  ' REMOVE THIS LINE

        DataGridViewAssigned.DataSource = bindingSource
        AddCalculatedColumns()
        ChkDetailedView.Checked = False
        ToggleGridView()
    End Sub

    Private Sub DataGridViewAssigned_DataError(sender As Object, e As DataGridViewDataErrorEventArgs) _
    Handles DataGridViewAssigned.DataError
        ' Suppress data binding errors (especially during design-time or before data loads)
        ' Log if needed for debugging:
        ' Debug.WriteLine($"DataGridView Error: {e.Exception.Message} at row {e.RowIndex}, col {e.ColumnIndex}")
        e.ThrowException = False
    End Sub

    ''' <summary>
    ''' Enables double buffering on a DataGridView to reduce flicker during updates.
    ''' </summary>
    Private Shared Sub EnableDoubleBuffering(dgv As DataGridView)
        Dim dgvType As Type = GetType(DataGridView)
        Dim pi As System.Reflection.PropertyInfo = dgvType.GetProperty(
        "DoubleBuffered",
        System.Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.NonPublic)
        pi?.SetValue(dgv, True, Nothing)
    End Sub

    Private Sub AddCalculatedColumns()
        ' Reset cache when rebuilding columns
        unitNameColumnIndex = -1
        colorCodeColumnIndex = -1

        DataGridViewAssigned.Columns.Clear()

        ' Hidden ID columns
        colActualUnitID = CreateTextColumn("ActualUnitID", "Actual Unit ID")
        colMappingID = CreateTextColumn("MappingID", "Mapping ID")

        ' Visible base columns (always shown)
        colUnitName = CreateTextColumn("UnitName", "Actual Unit Name", True, True)
        colReferencedRawUnitName = CreateTextColumn("ReferencedRawUnitName", "Referenced Raw Unit Name", True, True)
        colPlanSQFT = CreateTextColumn("PlanSQFT", "Plan SQFT", True)
        colActualUnitQuantity = CreateTextColumn("ActualUnitQuantity", "Actual Unit Quantity", True)

        ' Detail columns (hidden by default, toggled by ChkDetailedView)
        colLF = CreateTextColumn("LF", "LF")
        colBDFT = CreateTextColumn("BDFT", "BDFT")
        colLumberCost = CreateTextColumn("LumberCost", "Lumber $$")
        colPlateCost = CreateTextColumn("PlateCost", "Plate $$")
        colManufLaborCost = CreateTextColumn("ManufLaborCost", "Manuf Labor $$")
        colDesignLabor = CreateTextColumn("DesignLabor", "Design Labor $$")
        colMGMTLabor = CreateTextColumn("MGMTLabor", "MGMT Labor $$")
        colJobSuppliesCost = CreateTextColumn("JobSuppliesCost", "Job Supplies $$")
        colManHours = CreateTextColumn("ManHours", "Man Hours")
        colItemCost = CreateTextColumn("ItemCost", "Item $$")
        colDeliveryCost = CreateTextColumn("DeliveryCost", "Delivery $$")
        colOverallCost = CreateTextColumn("OverallCost", "Overall Cost")
        colSellPrice = CreateTextColumn("SellPrice", "Sell Price")
        colMargin = CreateTextColumn("Margin", "Margin $$")
        colColorCode = CreateTextColumn("ColorCode", "ColorCode", False, False, "colColorCode")

        ' Add all text columns
        DataGridViewAssigned.Columns.AddRange({
        colActualUnitID, colMappingID, colUnitName, colReferencedRawUnitName,
        colPlanSQFT, colActualUnitQuantity, colLF, colBDFT, colLumberCost,
        colPlateCost, colManufLaborCost, colDesignLabor, colMGMTLabor,
        colJobSuppliesCost, colManHours, colItemCost, colDeliveryCost,
        colOverallCost, colSellPrice, colMargin, colColorCode
    })

        ' Button column (special case - not a text column)
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
        colColorCode.Visible = isDetailedView
        DataGridViewAssigned.Refresh()
        UIHelper.Add($"Grid view switched to {(If(isDetailedView, "Detailed", "Base"))} mode.")
    End Sub

    Private Sub ChkDetailedView_CheckedChanged(sender As Object, e As EventArgs) Handles ChkDetailedView.CheckedChanged
        ToggleGridView()
    End Sub

    Private Sub LoadLevelHierarchy()
        TreeViewLevels.Nodes.Clear()
        Try
            Dim project = dataAccess.GetProjectByID(selectedProjectID)
            If project Is Nothing Then
                UIHelper.Add("Status: Project not found.")
                Return
            End If
            Dim versions As List(Of ProjectVersionModel) = ProjVersionDataAccess.GetProjectVersions(selectedProjectID)
            Dim selectedVersion As ProjectVersionModel = versions.FirstOrDefault(Function(v) v.VersionID = selectedVersionID)
            Dim projectNodeText As String = project.ProjectName & If(selectedVersion IsNot Nothing, " - " & selectedVersion.VersionName, "")
            Dim projectNode As TreeNode = TreeViewLevels.Nodes.Add(projectNodeText)
            projectNode.Tag = New Dictionary(Of String, Object) From {{"Type", "Project"}, {"ID", selectedProjectID}}
            Dim buildings = ProjectDataAccess.GetBuildingsByVersionID(selectedVersionID)
            If Not buildings.Any() Then
                UIHelper.Add("Status: No buildings found for this version.")
                Return
            End If
            For Each bldg In buildings
                Dim bldgNode As New TreeNode(String.Format("{0} ({1})", bldg.BuildingName, "x" & bldg.BldgQty)) With {
                    .Tag = New Dictionary(Of String, Object) From {{"Type", "Building"}, {"ID", bldg.BuildingID}}
                }
                projectNode.Nodes.Add(bldgNode)

                Dim levels = ProjectDataAccess.GetLevelsByBuildingID(bldg.BuildingID)
                For Each lvl In levels
                    Dim levelNode As New TreeNode(String.Format("{0} ({1})", lvl.LevelName, lvl.ProductTypeName)) With {
                        .Tag = New Dictionary(Of String, Object) From {{"Type", "Level"}, {"ID", lvl.LevelID}, {"ProductTypeID", lvl.ProductTypeID}}
                    }
                    bldgNode.Nodes.Add(levelNode)
                Next
            Next
            projectNode.Expand()
            UIHelper.Add("Status: Project hierarchy loaded for version " & selectedVersionID & ".")
        Catch ex As Exception
            UIHelper.Add("Status: Error loading hierarchy: " & ex.Message)
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
                UIHelper.Add("Status: No raw units found for this version and product type.")
                Return
            End If
            For Each rawUnit In rawUnits
                ListBoxAvailableUnits.Items.Add(rawUnit.RawUnitName)
            Next
            UIHelper.Add("Status: Raw units loaded for version " & selectedVersionID & " and product type " & productTypeID & ".")
            UpdateSelectedRawPreview()
        Catch ex As Exception
            UIHelper.Add("Status: Error loading raw units: " & ex.Message)
            MessageBox.Show("Error loading raw units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Formats an ActualUnit for display in the listbox using consistent naming.
    ''' Returns just the unit name portion (column 1).
    ''' </summary>
    Private Function FormatActualUnitName(unit As ActualUnitModel) As String
        Return $"{unit.UnitName} ({unit.UnitType}) - ({unit.PlanSQFT:F0})"
    End Function

    ''' <summary>
    ''' Gets the sell price per SQFT display string for an ActualUnit (column 2).
    ''' </summary>
    Private Function FormatActualUnitPrice(unit As ActualUnitModel) As String
        Dim sellPricePerSQFT As Decimal = 0D
        Dim sellPriceComponent = unit.CalculatedComponents?.FirstOrDefault(Function(c) c.ComponentType = "SellPrice/SQFT")
        If sellPriceComponent IsNot Nothing Then
            sellPricePerSQFT = sellPriceComponent.Value
        End If
        Return $"{sellPricePerSQFT:C2}/SQFT"
    End Function

    ''' <summary>
    ''' Refreshes the ListboxExistingActualUnits with current filter applied.
    ''' </summary>
    Private Sub RefreshExistingActualUnitsList()
        ListboxExistingActualUnits.Items.Clear()
        For Each unit In existingActualUnits
            ' Store simple identifier - actual display handled by DrawItem
            ListboxExistingActualUnits.Items.Add(unit.UnitName)
        Next
    End Sub

    Private Sub FilterActualUnits(productTypeID As Integer)
        ListboxExistingActualUnits.Items.Clear()
        Try
            existingActualUnits = ProjectDataAccess.GetActualUnitsByVersion(selectedVersionID).Where(Function(u) u.ProductTypeID = productTypeID).ToList()
            If Not existingActualUnits.Any() Then
                UIHelper.Add("Status: No actual units found for this version and product type.")
                Return
            End If
            RefreshExistingActualUnitsList()
            UIHelper.Add("Status: Actual units loaded for version " & selectedVersionID & " and product type " & productTypeID & ".")
        Catch ex As Exception
            UIHelper.Add("Status: Error loading actual units: " & ex.Message)
            MessageBox.Show("Error loading actual units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ListboxExistingActualUnits_DrawItem(sender As Object, e As DrawItemEventArgs) Handles ListboxExistingActualUnits.DrawItem
        If e.Index < 0 OrElse e.Index >= existingActualUnits.Count Then Exit Sub

        Dim unit As ActualUnitModel = existingActualUnits(e.Index)

        ' Determine background color from ColorCode
        Dim backColor As Color = Color.White
        If Not String.IsNullOrEmpty(unit.ColorCode) Then
            Try
                backColor = ColorTranslator.FromHtml("#" & unit.ColorCode)
            Catch
                ' Fallback to white on invalid hex
            End Try
        End If

        ' Draw the background
        Using backBrush As New SolidBrush(backColor)
            e.Graphics.FillRectangle(backBrush, e.Bounds)
        End Using

        ' Draw selection highlight if selected
        If (e.State And DrawItemState.Selected) = DrawItemState.Selected Then
            Dim highlightColor As Color = Color.FromArgb(128, SystemColors.Highlight)
            Using highlightBrush As New SolidBrush(highlightColor)
                e.Graphics.FillRectangle(highlightBrush, e.Bounds)
            End Using
            Using pen As New Pen(Color.Black, 1)
                Dim borderRect As New Rectangle(e.Bounds.X + 1, e.Bounds.Y + 1, e.Bounds.Width - 2, e.Bounds.Height - 2)
                e.Graphics.DrawRectangle(pen, borderRect)
            End Using
        End If

        ' Calculate text color based on luminance
        Dim luminance As Double = (0.299 * backColor.R + 0.587 * backColor.G + 0.114 * backColor.B) / 255
        Dim textColor As Color = If(luminance < 0.5, Color.White, Color.Black)

        ' Define column widths
        Dim col1Width As Integer = CInt(e.Bounds.Width * 0.65) ' 65% for name
        Dim col2Start As Integer = e.Bounds.X + col1Width

        ' Get display text for each column
        Dim col1Text As String = FormatActualUnitName(unit)
        Dim col2Text As String = FormatActualUnitPrice(unit)

        ' Draw column 1 (left-aligned)
        Dim col1Rect As New Rectangle(e.Bounds.X + 2, e.Bounds.Y, col1Width - 4, e.Bounds.Height)
        Using textBrush As New SolidBrush(textColor)
            Using sf As New StringFormat With {.Trimming = StringTrimming.EllipsisCharacter, .FormatFlags = StringFormatFlags.NoWrap}
                e.Graphics.DrawString(col1Text, e.Font, textBrush, col1Rect, sf)
            End Using
        End Using

        ' Draw column 2 (right-aligned)
        Dim col2Rect As New Rectangle(col2Start, e.Bounds.Y, e.Bounds.Width - col1Width - 4, e.Bounds.Height)
        Using textBrush As New SolidBrush(textColor)
            Using sf As New StringFormat With {.Alignment = StringAlignment.Far, .Trimming = StringTrimming.EllipsisCharacter}
                e.Graphics.DrawString(col2Text, e.Font, textBrush, col2Rect, sf)
            End Using
        End Using

        ' Draw vertical separator line
        Using pen As New Pen(Color.FromArgb(64, textColor), 1)
            e.Graphics.DrawLine(pen, col2Start - 2, e.Bounds.Y + 2, col2Start - 2, e.Bounds.Bottom - 2)
        End Using
    End Sub
    Private Sub LoadAssignedUnits()
        Try
            ' Suspend UI updates
            DataGridViewAssigned.SuspendLayout()
            bindingSource.RaiseListChangedEvents = False

            Dim dt As DataTable = ProjectDataAccess.ComputeLevelUnitsDataTable(selectedLevelID)

            ' Clear and rebind - this ensures columns reconnect to new DataTable
            bindingSource.DataSource = Nothing
            bindingSource.DataSource = dt

            displayUnits = dt.AsEnumerable() _
                 .Select(Function(row) New DisplayUnitData With {
                     .ActualUnitID = row.Field(Of Integer)("ActualUnitID"),
                     .MappingID = If(row.IsNull("MappingID"), -1, row.Field(Of Integer)("MappingID")),
                     .UnitName = row.Field(Of String)("UnitName"),
                     .ReferencedRawUnitName = row.Field(Of String)("ReferencedRawUnitName"),
                     .PlanSQFT = row.Field(Of Decimal)("PlanSQFT"),
                     .ActualUnitQuantity = row.Field(Of Integer)("ActualUnitQuantity"),
                     .LF = row.Field(Of Decimal)("LF"),
                     .BDFT = row.Field(Of Decimal)("BDFT"),
                     .LumberCost = row.Field(Of Decimal)("LumberCost"),
                     .PlateCost = row.Field(Of Decimal)("PlateCost"),
                     .ManufLaborCost = row.Field(Of Decimal)("ManufLaborCost"),
                     .DesignLabor = row.Field(Of Decimal)("DesignLabor"),
                     .MGMTLabor = row.Field(Of Decimal)("MGMTLabor"),
                     .JobSuppliesCost = row.Field(Of Decimal)("JobSuppliesCost"),
                     .ManHours = row.Field(Of Decimal)("ManHours"),
                     .ItemCost = row.Field(Of Decimal)("ItemCost"),
                     .OverallCost = row.Field(Of Decimal)("OverallCost"),
                     .DeliveryCost = row.Field(Of Decimal)("DeliveryCost"),
                     .SellPrice = row.Field(Of Decimal)("SellPrice"),
                     .Margin = row.Field(Of Decimal)("Margin"),
                     .ColorCode = If(row.IsNull("ColorCode"), Nothing, row.Field(Of String)("ColorCode"))
                 }).ToList()

            UpdateLevelTotals()
        Catch ex As Exception
            MessageBox.Show("Error loading assigned units: " & ex.Message, "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            ' Always resume UI updates
            bindingSource.RaiseListChangedEvents = True
            bindingSource.ResetBindings(True)  ' Changed to True to force full refresh
            DataGridViewAssigned.ResumeLayout(True)  ' Force layout
            DataGridViewAssigned.Refresh()
        End Try
    End Sub

    Private Sub DataGridViewAssigned_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) _
Handles DataGridViewAssigned.CellFormatting

        ' Cache column indices on first call
        If unitNameColumnIndex < 0 Then
            unitNameColumnIndex = If(colUnitName IsNot Nothing, colUnitName.Index, -1)
            colorCodeColumnIndex = If(colColorCode IsNot Nothing, colColorCode.Index, -1)
        End If

        ' Quick exit for non-target columns
        If e.ColumnIndex <> unitNameColumnIndex OrElse e.RowIndex < 0 Then Return
        If colorCodeColumnIndex < 0 Then Return

        Dim row As DataGridViewRow = DataGridViewAssigned.Rows(e.RowIndex)
        If row.IsNewRow Then Return

        Dim colorValue As Object = row.Cells(colorCodeColumnIndex).Value
        If colorValue Is Nothing OrElse IsDBNull(colorValue) Then Return

        Dim colorCode As String = colorValue.ToString().Trim()
        If String.IsNullOrEmpty(colorCode) OrElse colorCode = "N/A" Then Return

        Try
            Dim backColor As Color = ColorTranslator.FromHtml("#" & colorCode)
            e.CellStyle.BackColor = backColor
            e.CellStyle.SelectionBackColor = backColor

            ' Calculate luminance and set text color: white for dark backgrounds, black for light
            Dim luminance As Double = (0.299 * backColor.R + 0.587 * backColor.G + 0.114 * backColor.B) / 255
            Dim textColor As Color = If(luminance < 0.5, Color.White, Color.Black)
            e.CellStyle.ForeColor = textColor
            e.CellStyle.SelectionForeColor = textColor
        Catch
            ' Ignore invalid color
        End Try
    End Sub

    Private Sub UpdateLevelTotals()
        ' Single pass aggregation
        Dim totals As New DisplayUnitTotals()

        For Each u In displayUnits
            Dim extQty As Integer = u.ActualUnitQuantity
            totals.PlanSQFT += u.PlanSQFT * extQty
            totals.Quantity += extQty
            totals.LF += u.LF
            totals.BDFT += u.BDFT
            totals.LumberCost += u.LumberCost
            totals.PlateCost += u.PlateCost
            totals.ManufLaborCost += u.ManufLaborCost
            totals.DesignLabor += u.DesignLabor
            totals.MGMTLabor += u.MGMTLabor
            totals.JobSuppliesCost += u.JobSuppliesCost
            totals.ManHours += u.ManHours
            totals.ItemCost += u.ItemCost
            totals.OverallCost += u.OverallCost
            totals.SellPrice += u.SellPrice
            totals.Margin += u.Margin
            totals.DeliveryCost += u.DeliveryCost
        Next

        TxtTotalPlanSQFT.Text = totals.PlanSQFT.ToString("F2")
        TxtTotalQuantity.Text = totals.Quantity.ToString("F0")
        TxtTotalLF.Text = totals.LF.ToString("F2")
        TxtTotalBDFT.Text = totals.BDFT.ToString("F2")
        TxtTotalLumberCost.Text = totals.LumberCost.ToString("C2")
        TxtTotalPlateCost.Text = totals.PlateCost.ToString("C2")
        TxtTotalManufLaborCost.Text = totals.ManufLaborCost.ToString("C2")
        TxtTotalDesignLabor.Text = totals.DesignLabor.ToString("C2")
        TxtTotalMGMTLabor.Text = totals.MGMTLabor.ToString("C2")
        TxtTotalJobSuppliesCost.Text = totals.JobSuppliesCost.ToString("C2")
        TxtTotalManHours.Text = totals.ManHours.ToString("F2")
        TxtTotalItemCost.Text = totals.ItemCost.ToString("C2")
        TxtTotalOverallCost.Text = totals.OverallCost.ToString("C2")
        TxtTotalSellPrice.Text = totals.SellPrice.ToString("C2")
        TxtTotalMargin.Text = totals.Margin.ToString("C2")
        txtTotMargin.Text = If(totals.SellPrice > 0, (totals.Margin / totals.SellPrice).ToString("P2"), "0.00%")
        txtTotalDeliveryCost.Text = totals.DeliveryCost.ToString("C2")

        UIHelper.Add($"Total Adjusted SQFT: {totals.PlanSQFT} - Recalc complete.")
    End Sub

    ' Add helper structure
    Private Structure DisplayUnitTotals
        Public PlanSQFT As Decimal
        Public Quantity As Integer
        Public LF As Decimal
        Public BDFT As Decimal
        Public LumberCost As Decimal
        Public PlateCost As Decimal
        Public ManufLaborCost As Decimal
        Public DesignLabor As Decimal
        Public MGMTLabor As Decimal
        Public JobSuppliesCost As Decimal
        Public ManHours As Decimal
        Public ItemCost As Decimal
        Public OverallCost As Decimal
        Public SellPrice As Decimal
        Public Margin As Decimal
        Public DeliveryCost As Decimal
    End Structure

    Private Sub UpdateSelectedRawPreview()
        Try
            Dim currentRawUnit = If(ListBoxAvailableUnits.SelectedIndex >= 0, rawUnits.FirstOrDefault(Function(r) r.RawUnitName = ListBoxAvailableUnits.Items(ListBoxAvailableUnits.SelectedIndex).ToString()), Nothing)
            If currentRawUnit Is Nothing OrElse currentRawUnit.SqFt.GetValueOrDefault() <= 0 Then
                ClearPerSQFTTextBoxes()
                UIHelper.Add($"Status: No valid raw unit or zero SQFT for version {selectedVersionID}.")
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
            UIHelper.Add($"Status: Preview updated for raw unit {currentRawUnit.RawUnitName} in version {selectedVersionID}.")
        Catch ex As Exception
            UIHelper.Add($"Status: Error updating raw unit preview for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error updating raw unit preview: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub FrmPSE_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim productTypes = dataAccess.GetProductTypes()
            If Not productTypes.Any() Then
                UIHelper.Add("Status: No product types found for version " & selectedVersionID & ".")
                Return
            End If
            If TreeViewLevels.Nodes.Count > 0 AndAlso TreeViewLevels.Nodes(0).Nodes.Count > 0 AndAlso TreeViewLevels.Nodes(0).Nodes(0).Nodes.Count > 0 Then
                TreeViewLevels.SelectedNode = TreeViewLevels.Nodes(0).Nodes(0).Nodes(0) ' Assume first level
            Else
                UIHelper.Add("Status: No levels available for version " & selectedVersionID & ".")
            End If

            populaterawunits()

            UpdateLevelTotals()
            UpdateSelectedRawPreview()
            TreeViewLevels.ExpandAll()
            UIHelper.Add("Status: Form loaded for version " & selectedVersionID & ".")
        Catch ex As Exception
            UIHelper.Add("Status: Error initializing form for version " & selectedVersionID & ": " & ex.Message)
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
            UIHelper.Add("Status: No raw units found for version " & selectedVersionID & ".")
        End If
    End Sub
    Private Function CreateAndSaveActualUnit() As ActualUnitModel
        Try
            Dim sqft As Decimal
            Dim adder As Decimal
            If String.IsNullOrEmpty(TxtUnitName.Text) OrElse Not Decimal.TryParse(TxtPlanSQFT.Text, sqft) OrElse sqft <= 0 OrElse
       Not Decimal.TryParse(TxtOptionalAdder.Text, adder) OrElse adder < 1 Then
                UIHelper.Add("Status: Invalid input—Name, SQFT (>0), and Adder (≥1) must be valid.")
                MessageBox.Show("Invalid input—Name, SQFT (>0), and Adder (≥1) must be valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End If

            Dim rawUnit As RawUnitModel = rawUnits.FirstOrDefault(Function(r) r.RawUnitID = selectedRawUnitID)
            If rawUnit Is Nothing Then
                UIHelper.Add("Status: Referenced Raw Unit not found.")
                MessageBox.Show("Referenced Raw Unit not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return Nothing
            End If

            ' Calculate next sort order (append to end)
            Dim nextSortOrder As Integer = If(existingActualUnits.Any(), existingActualUnits.Max(Function(u) u.SortOrder) + 1, 1)

            Dim actualUnit As New ActualUnitModel With {
            .VersionID = selectedVersionID,
            .RawUnitID = rawUnit.RawUnitID,
            .ProductTypeID = rawUnit.ProductTypeID,
            .UnitName = TxtUnitName.Text,
            .PlanSQFT = sqft,
            .ColorCode = txtColorCode.Text,
            .UnitType = If(CmbUnitType.SelectedItem?.ToString(), "Res"),
            .OptionalAdder = adder,
            .MarginPercent = ProjectDataAccess.GetEffectiveMargin(Me.selectedVersionID, rawUnit.ProductTypeID, rawUnit.RawUnitID),
            .SortOrder = nextSortOrder
        }

            dataAccess.SaveActualUnit(actualUnit)
            actualUnit.CalculatedComponents = CalculateComponentsFromRaw(rawUnit, Me.selectedVersionID)
            dataAccess.SaveCalculatedComponents(actualUnit)

            UIHelper.Add("Status: Actual unit created for version " & selectedVersionID & ".")
            Return actualUnit
        Catch ex As Exception
            UIHelper.Add("Status: Error creating actual unit for version " & selectedVersionID & ": " & ex.Message)
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
            UIHelper.Add("Status: Error calculating components for version " & versionID & ": " & ex.Message)
            MessageBox.Show("Error calculating components: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return components
        End Try
    End Function



    Private Sub BtnImportLevelData_Click(sender As Object, e As EventArgs) Handles BtnImportLevelData.Click
        UIHelper.ShowBusy(frmMain)
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
                    UIHelper.Add("Status: Raw units imported successfully for version " & selectedVersionID & ".")
                    If selectedLevelID <> -1 Then
                        populaterawunits()
                        FilterRawUnits(productTypeID)
                        RecalculateAllActualUnits(selectedVersionID)
                        FilterActualUnits(productTypeID)
                        LoadAssignedUnits()
                    End If
                Else
                    UIHelper.Add("Status: Import cancelled for version " & selectedVersionID & ".")
                End If
            Catch ex As Exception
                UIHelper.Add("Status: Import failed for version " & selectedVersionID & ": " & ex.Message)
                MessageBox.Show("Import failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                isImporting = False
                UIHelper.HideBusy(frmMain)
            End Try
        End If
    End Sub


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
                                                         ' Single call replaces ~50 lines of repetitive parsing
                                                         PopulateRawUnitFromCsv(model, rowData)
                                                         Try
                                                             If mapTo = "Create New" Then
                                                                 ' Insert new RawUnit
                                                                 Dim newRawUnitID As Integer = dataAccess.InsertRawUnit(model)
                                                                 model.RawUnitID = newRawUnitID
                                                                 ' Fetch the CostEffectiveDateID based on AvgSPFNo2
                                                                 ' Instead of the 15-line block, use:
                                                                 InsertRawUnitLumberHistory(model, conn, transaction)
                                                             Else
                                                                 Dim existing As RawUnitModel = allRawUnits.FirstOrDefault(Function(ru) ru.RawUnitName.Trim().ToUpper() = mapTo.Trim().ToUpper())
                                                                 If existing Is Nothing Then
                                                                     MessageBox.Show("Selected existing unit '" & mapTo & "' not found. Creating new unit.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                                                                     Dim newRawUnitID As Integer = dataAccess.InsertRawUnit(model)
                                                                     model.RawUnitID = newRawUnitID
                                                                     ' Fetch the CostEffectiveDateID based on AvgSPFNo2
                                                                     ' Instead of the 15-line block, use:
                                                                     InsertRawUnitLumberHistory(model, conn, transaction)
                                                                 Else
                                                                     model.RawUnitID = existing.RawUnitID
                                                                     dataAccess.UpdateRawUnit(model)
                                                                     ' Fetch the CostEffectiveDateID based on AvgSPFNo2
                                                                     ' Instead of the 15-line block, use:
                                                                     InsertRawUnitLumberHistory(model, conn, transaction)
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
                UIHelper.Add($"Status: No raw unit selected for conversion in version {selectedVersionID}.")
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
            UIHelper.Add($"Status: Converting raw unit {rawUnit.RawUnitName} to actual unit for version {selectedVersionID}.")
        Catch ex As Exception
            UIHelper.Add($"Status: Error converting raw unit for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error converting raw unit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnReuseActualUnit_Click(sender As Object, e As EventArgs) Handles btnReuseActualUnit.Click
        Try
            Dim selectedActual = GetSelectedActualUnit()
            If selectedActual Is Nothing Then Return

            selectedActualUnitID = selectedActual.ActualUnitID
            currentdetailsMode = DetailsMode.ReuseUnit
            PopulateDetailsPane(selectedActual, False, 1)
            UIHelper.Add($"Status: Reusing actual unit {selectedActual.UnitName} for version {selectedVersionID}.")
        Catch ex As Exception
            UIHelper.Add($"Status: Error reusing unit for version {selectedVersionID}: {ex.Message}")
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
            'TxtReferencedRawUnit.Text = actual.ReferencedRawUnitName
            TxtActualUnitQuantity.Text = quantity.ToString()

            ' Populate RawUnit ComboBox for edit modes
            PopulateRawUnitComboBox(actual.ProductTypeID, actual.RawUnitID)

            ' Enable/disable fields based on mode
            TxtUnitName.Enabled = currentdetailsMode = DetailsMode.NewUnit OrElse currentdetailsMode = DetailsMode.EditGlobalUnit
            TxtPlanSQFT.Enabled = currentdetailsMode = DetailsMode.NewUnit OrElse currentdetailsMode = DetailsMode.EditGlobalUnit
            TxtOptionalAdder.Enabled = currentdetailsMode = DetailsMode.NewUnit OrElse currentdetailsMode = DetailsMode.EditGlobalUnit
            txtColorCode.Enabled = currentdetailsMode = DetailsMode.NewUnit OrElse currentdetailsMode = DetailsMode.EditGlobalUnit
            CmbUnitType.Enabled = currentdetailsMode = DetailsMode.NewUnit OrElse currentdetailsMode = DetailsMode.EditGlobalUnit
            'TxtReferencedRawUnit.Enabled = False
            TxtActualUnitQuantity.Enabled = currentdetailsMode = DetailsMode.ReuseUnit OrElse currentdetailsMode = DetailsMode.EditMappingQuantity

            ' Enable RawUnit selection in EditGlobalUnit and EditMappingQuantity modes
            CmbRawUnitSelection.Enabled = currentdetailsMode = DetailsMode.EditGlobalUnit OrElse currentdetailsMode = DetailsMode.EditMappingQuantity
            CmbRawUnitSelection.Visible = currentdetailsMode = DetailsMode.EditGlobalUnit OrElse currentdetailsMode = DetailsMode.EditMappingQuantity

            ' Explicit reset for delete button
            btnDeleteUnit.Visible = False
            btnDeleteUnit.Text = "Delete"

            chkAttachToLevel.Visible = False
            chkAttachToLevel.Checked = False

            ' Set button text
            Select Case currentdetailsMode
                Case DetailsMode.NewUnit
                    btnSave.Text = "Save New Unit"
                    chkAttachToLevel.Visible = True
                Case DetailsMode.EditGlobalUnit
                    btnSave.Text = "Save Unit Changes"
                    btnDeleteUnit.Text = "Delete Unit"
                    btnDeleteUnit.Visible = True
                Case DetailsMode.ReuseUnit
                    btnSave.Text = "Save and Attach"
                Case DetailsMode.EditMappingQuantity
                    btnSave.Text = "Save Changes"
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
            UIHelper.Add($"Status: Details pane opened in {currentdetailsMode} mode for version {selectedVersionID}.")
        Catch ex As Exception
            UIHelper.Add($"Status: Error opening details pane for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error opening details pane: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ''' <summary>
    ''' Populates the RawUnit selection ComboBox with matching ProductType units.
    ''' </summary>
    Private Sub PopulateRawUnitComboBox(productTypeID As Integer, currentRawUnitID As Integer)
        ' IMPORTANT: Clear DataSource BEFORE clearing Items
        CmbRawUnitSelection.DataSource = Nothing

        ' Now set up the new binding
        CmbRawUnitSelection.DisplayMember = "RawUnitName"
        CmbRawUnitSelection.ValueMember = "RawUnitID"

        Dim availableRawUnits = ProjectDataAccess.GetRawUnitsByVersionID(selectedVersionID) _
        .Where(Function(r) r.ProductTypeID = productTypeID).ToList()

        If availableRawUnits.Any() Then
            CmbRawUnitSelection.DataSource = availableRawUnits

            ' Select current RawUnit
            Dim currentIndex = availableRawUnits.FindIndex(Function(r) r.RawUnitID = currentRawUnitID)
            If currentIndex >= 0 Then
                CmbRawUnitSelection.SelectedIndex = currentIndex
            End If
        End If
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
                UIHelper.Add($"Status: Invalid unit selection for version {selectedVersionID}.")
                MessageBox.Show("Invalid unit selection for edit.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim sqft As Decimal
            Dim adder As Decimal
            If Not Decimal.TryParse(TxtPlanSQFT.Text, sqft) OrElse sqft <= 0 OrElse
           Not Decimal.TryParse(TxtOptionalAdder.Text, adder) OrElse adder < 1 Then
                UIHelper.Add($"Status: Invalid input for version {selectedVersionID}—SQFT (>0) and Adder (≥1) must be valid.")
                MessageBox.Show("Invalid input—SQFT (>0) and Adder (≥1) must be valid.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim updatedUnit As ActualUnitModel = ProjectDataAccess.GetActualUnitByID(selectedActualUnitID)
            If updatedUnit Is Nothing Then
                UIHelper.Add($"Status: Actual unit not found for version {selectedVersionID}.")
                MessageBox.Show("Actual unit not found for update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Check if RawUnit was changed via ComboBox
            Dim selectedRawUnit As RawUnitModel = Nothing
            Dim rawUnitChanged As Boolean = False
            If CmbRawUnitSelection.Visible AndAlso CmbRawUnitSelection.SelectedItem IsNot Nothing Then
                selectedRawUnit = TryCast(CmbRawUnitSelection.SelectedItem, RawUnitModel)
                If selectedRawUnit IsNot Nothing AndAlso selectedRawUnit.RawUnitID <> updatedUnit.RawUnitID Then
                    rawUnitChanged = True
                    updatedUnit.RawUnitID = selectedRawUnit.RawUnitID
                    UIHelper.Add($"Status: RawUnit changing from ID {updatedUnit.RawUnitID} to '{selectedRawUnit.RawUnitName}' (ID {selectedRawUnit.RawUnitID}).")
                End If
            End If

            ' Update standard fields
            updatedUnit.VersionID = selectedVersionID
            updatedUnit.UnitName = TxtUnitName.Text
            updatedUnit.PlanSQFT = sqft
            updatedUnit.OptionalAdder = adder
            updatedUnit.ColorCode = txtColorCode.Text
            updatedUnit.UnitType = If(CmbUnitType.SelectedItem?.ToString(), "Res")

            ' Save the ActualUnit (now includes RawUnitID if changed)
            dataAccess.SaveActualUnit(updatedUnit)

            ' Recalculate components - use new RawUnit if changed, otherwise existing
            Dim rawUnitForCalc As RawUnitModel = If(rawUnitChanged AndAlso selectedRawUnit IsNot Nothing,
                                             selectedRawUnit,
                                             ProjectDataAccess.GetRawUnitByID(updatedUnit.RawUnitID))
            If rawUnitForCalc IsNot Nothing Then
                updatedUnit.CalculatedComponents = CalculateComponentsFromRaw(rawUnitForCalc, selectedVersionID)
                dataAccess.SaveCalculatedComponents(updatedUnit)
            End If

            ' Update the unit in the existing list and refresh display
            Dim index As Integer = existingActualUnits.FindIndex(Function(u) u.ActualUnitID = selectedActualUnitID)
            If index >= 0 Then
                existingActualUnits(index) = updatedUnit
                RefreshExistingActualUnitsList()
            End If

            If selectedLevelID > 0 Then
                LoadAssignedUnits()
                UpdateRollups()
            End If

            UIHelper.Add($"Status: Actual unit '{updatedUnit.UnitName}' updated successfully for version {selectedVersionID}." &
                 If(rawUnitChanged, $" RawUnit changed to '{selectedRawUnit?.RawUnitName}'.", ""))
        Catch ex As Exception
            UIHelper.Add($"Status: Error updating actual unit for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error updating actual unit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub HandleEditMappingQuantity(quantity As Integer)
        Try
            If selectedMappingID <= 0 Then
                UIHelper.Add($"Status: Invalid mapping ID for version {selectedVersionID}.")
                MessageBox.Show("Invalid mapping selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim mapping As ActualToLevelMappingModel = ProjectDataAccess.GetActualToLevelMappingsByLevelID(selectedLevelID) _
            .FirstOrDefault(Function(m) m.MappingID = selectedMappingID)
            If mapping Is Nothing Then
                UIHelper.Add($"Status: Mapping not found for ID {selectedMappingID} in version {selectedVersionID}.")
                MessageBox.Show("Mapping not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            ' Update quantity
            mapping.Quantity = quantity
            dataAccess.SaveActualToLevelMapping(mapping)

            ' Check if RawUnit was changed
            Dim selectedRawUnit As RawUnitModel = TryCast(CmbRawUnitSelection.SelectedItem, RawUnitModel)
            If selectedRawUnit IsNot Nothing AndAlso selectedRawUnit.RawUnitID <> mapping.ActualUnit.RawUnitID Then
                ' Update the ActualUnit's RawUnitID
                Dim actualUnit = ProjectDataAccess.GetActualUnitByID(mapping.ActualUnitID)
                If actualUnit IsNot Nothing Then
                    actualUnit.RawUnitID = selectedRawUnit.RawUnitID

                    ' Save the updated ActualUnit
                    dataAccess.SaveActualUnit(actualUnit)

                    ' Recalculate components based on new RawUnit
                    actualUnit.CalculatedComponents = CalculateComponentsFromRaw(selectedRawUnit, selectedVersionID)
                    dataAccess.SaveCalculatedComponents(actualUnit)

                    UIHelper.Add($"Status: RawUnit changed to '{selectedRawUnit.RawUnitName}' for ActualUnit '{actualUnit.UnitName}'.")
                End If
            End If

            LoadAssignedUnits()
            UpdateRollups()
            UIHelper.Add($"Status: Mapping updated for unit {mapping.ActualUnit.UnitName} in version {selectedVersionID}.")
        Catch ex As Exception
            UIHelper.Add($"Status: Error updating mapping for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error updating mapping: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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
        UIHelper.Add("Unit quantity updated successfully for version " & selectedVersionID & ".")
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
        UIHelper.Add("Unit saved and attached successfully for version " & selectedVersionID & ".")
    End Sub

    Private Sub HandleReuseMapping(qty As Integer)
        Dim mapping As New ActualToLevelMappingModel With {
            .VersionID = selectedVersionID,
            .ActualUnitID = selectedActualUnitID,
            .LevelID = selectedLevelID,
            .Quantity = qty
        }
        dataAccess.SaveActualToLevelMapping(mapping)
        UIHelper.Add("Existing unit reused and mapped successfully for version " & selectedVersionID & ".")
    End Sub

    Private Sub btnDeleteUnit_Click(sender As Object, e As EventArgs) Handles btnDeleteUnit.Click
        Try
            Select Case currentdetailsMode
                Case DetailsMode.EditGlobalUnit
                    Dim selectedActual As ActualUnitModel = ProjectDataAccess.GetActualUnitByID(selectedActualUnitID)
                    If selectedActual Is Nothing Then Return
                    Dim mappings = dataAccess.GetActualToLevelMappingsByActualUnitID(selectedActual.ActualUnitID)
                    If mappings.Any() Then
                        Dim result = MessageBox.Show($"The unit '{selectedActual.UnitName}' is used in {mappings.Count} level(s). Deleting it will remove it from all levels. Continue?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                        If result <> DialogResult.Yes Then Return
                    End If
                    dataAccess.DeleteActualUnit(selectedActual.ActualUnitID)
                    existingActualUnits.RemoveAll(Function(u) u.ActualUnitID = selectedActualUnitID)
                    RefreshExistingActualUnitsList()
                    If selectedLevelID > 0 Then LoadAssignedUnits()
                    UIHelper.Add($"Status: Actual unit deleted for version {selectedVersionID}.")

                Case DetailsMode.EditMappingQuantity
                    If selectedMappingID <= 0 Then
                        MessageBox.Show("Invalid mapping selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return
                    End If
                    Dim result = MessageBox.Show("Delete this unit mapping from the level?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                    If result <> DialogResult.Yes Then Return
                    dataAccess.DeleteActualToLevelMapping(selectedMappingID)
                    LoadAssignedUnits()
                    UpdateRollups()
                    ResetDetailsPane()
                    UIHelper.Add($"Status: Mapping deleted for version {selectedVersionID}.")

                Case Else
                    MessageBox.Show("Delete not supported in this mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Select
        Catch ex As Exception
            UIHelper.Add($"Status: Error deleting for version {selectedVersionID}: {ex.Message}")
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
            'TxtReferencedRawUnit.Text = String.Empty
            TxtActualUnitQuantity.Text = String.Empty
            btnDeleteUnit.Text = "Delete"
            btnDeleteUnit.Visible = False
            selectedActualUnitID = -1
            selectedMappingID = -1
            currentdetailsMode = DetailsMode.None
            btnSave.Text = "Save"
            PanelDetails.Visible = False
            BtnToggleDetails.Text = "Show Details"
            UIHelper.Add($"Status: Details pane reset for version {selectedVersionID}.")
        Catch ex As Exception
            UIHelper.Add($"Status: Error resetting details pane for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error resetting details pane: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnRecalculate_Click(sender As Object, e As EventArgs) Handles BtnRecalculate.Click
        Try
            UIHelper.ShowBusy(frmMain)
            If selectedLevelID <= 0 Then
                UIHelper.Add("Status: Please select a level to recalculate for version " & selectedVersionID & ".")
                MessageBox.Show("Please select a level before recalculating.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If
            LoadAssignedUnits()
            UpdateRollups()
            If MsgBox("Recalculate entire project for version " & selectedVersionID & "?", MsgBoxStyle.YesNo, "Confirm") = MsgBoxResult.Yes Then
                RecalculateAllActualUnits(selectedVersionID)
                RecalculateProject()
            End If
            UIHelper.Add("Status: Recalculated totals successfully for version " & selectedVersionID & ".")
            MessageBox.Show("Recalculated totals successfully for version " & selectedVersionID & ".", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            UIHelper.Add("Status: Error recalculating for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error recalculating: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            UIHelper.HideBusy(frmMain)
        End Try
    End Sub
    Private Sub RecalculateProject()
        Try
            RollupCalculationService.RecalculateVersion(selectedVersionID)
            LoadAssignedUnits()
            UIHelper.Add("Status: Project recalculated successfully for version " & selectedVersionID & ".")
        Catch ex As Exception
            UIHelper.Add("Status: Error recalculating project for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error recalculating project: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub DataGridViewAssigned_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewAssigned.CellContentClick
        Try
            If e.ColumnIndex <> colActions.Index OrElse e.RowIndex < 0 Then Return

            Dim drv As DataRowView = TryCast(bindingSource.Current, DataRowView)
            If drv Is Nothing Then
                MessageBox.Show("No unit selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Dim row As DataRow = drv.Row

            ' ← ALL VALUES COME STRAIGHT FROM THE DATATABLE — NO LOOKUPS!
            selectedMappingID = Convert.ToInt32(row("MappingID"))
            selectedActualUnitID = Convert.ToInt32(row("ActualUnitID"))   ' ← YOU HAVE THIS!
            Dim quantity = Convert.ToInt32(row("ActualUnitQuantity"))
            Dim unitName = row("UnitName").ToString()

            ' Load the full ActualUnit object only once (still needed for PopulateDetailsPane)
            Dim actualUnit As ActualUnitModel = ProjectDataAccess.GetActualUnitByID(selectedActualUnitID)
            If actualUnit Is Nothing Then
                MessageBox.Show("Unit not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            currentdetailsMode = DetailsMode.EditMappingQuantity
            PopulateDetailsPane(actualUnit, False, quantity)

            UIHelper.Add($"Status: Editing mapping quantity for unit {unitName} in version {selectedVersionID}.")

        Catch ex As Exception
            UIHelper.Add($"Status: Error in cell click: {ex.Message}")
            MessageBox.Show("Error: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadExistingActualUnits()
        ListboxExistingActualUnits.Items.Clear()
        Try
            existingActualUnits = ProjectDataAccess.GetActualUnitsByVersion(selectedVersionID)
            If Not existingActualUnits.Any() Then
                UIHelper.Add("Status: No actual units found for version " & selectedVersionID & ".")
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
            UIHelper.Add("Status: Actual units loaded for version " & selectedVersionID & ".")
        Catch ex As Exception
            UIHelper.Add("Status: Error loading actual units for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error loading actual units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ListBoxAvailableUnits_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBoxAvailableUnits.SelectedIndexChanged
        Try
            If ListBoxAvailableUnits.SelectedIndex < 0 Then
                UIHelper.Add($"Status: No raw unit selected for version {selectedVersionID}.")
                UpdateSelectedRawPreview()
                Return
            End If
            UpdateSelectedRawPreview()
        Catch ex As Exception
            UIHelper.Add($"Status: Error updating raw unit preview for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error updating raw unit preview: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuCopyUnits_Click(sender As Object, e As EventArgs) Handles mnuCopyUnits.Click
        If selectedLevelID <= 0 Then
            UIHelper.Add("Status: Please select a level to copy units for version " & selectedVersionID & ".")
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
            UIHelper.Add($"Status: Copied {copiedMappings.Count} units from level for version {selectedVersionID}. Ready to paste.")
        Catch ex As Exception
            UIHelper.Add("Status: Error copying units for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error copying units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuPasteUnits_Click(sender As Object, e As EventArgs) Handles mnuPasteUnits.Click
        If selectedLevelID <= 0 Then
            UIHelper.Add("Status: Please select a target level for version " & selectedVersionID & ".")
            MessageBox.Show("Please select a target level to paste units.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        If copiedMappings.Count = 0 Then
            UIHelper.Add("Status: No units copied for version " & selectedVersionID & ".")
            MessageBox.Show("No units copied to paste.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Try
            Dim levelTag = DirectCast(TreeViewLevels.SelectedNode.Tag, Dictionary(Of String, Object))
            Dim targetProductTypeID As Integer = CInt(levelTag("ProductTypeID"))
            If targetProductTypeID <> sourceProductTypeID Then
                UIHelper.Add("Status: Source and target levels must have the same Product Type for version " & selectedVersionID & ".")
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
            UpdateRollups()
            'copiedMappings.Clear()
            UIHelper.Add("Status: Units pasted successfully for version " & selectedVersionID & ".")
        Catch ex As Exception
            UIHelper.Add("Status: Error pasting units for version " & selectedVersionID & ": " & ex.Message)
            MessageBox.Show("Error pasting units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnToggleDetails_Click(sender As Object, e As EventArgs) Handles BtnToggleDetails.Click
        Try
            PanelDetails.Visible = Not PanelDetails.Visible
            BtnToggleDetails.Text = If(PanelDetails.Visible, "Hide Details", "Show Details")
            UIHelper.Add($"Status: Details panel {(If(PanelDetails.Visible, "shown", "hidden"))} for version {selectedVersionID}.")
        Catch ex As Exception
            UIHelper.Add($"Status: Error toggling details panel for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error toggling details panel: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnCancel_Click(sender As Object, e As EventArgs) Handles BtnCancel.Click
        Try
            ResetDetailsPane()
            btnDeleteUnit.Visible = False
            UIHelper.Add($"Status: Details pane reset for version {selectedVersionID}.")
        Catch ex As Exception
            UIHelper.Add($"Status: Error resetting details pane for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error resetting details pane: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub BtnFinish_Click(sender As Object, e As EventArgs) Handles BtnFinish.Click
        Try
            UIHelper.ShowBusy(_mainForm)
            UIHelper.Add($"Status: Session complete for version {selectedVersionID} at {DateTime.Now:HH:mm:ss}.")
            RollupCalculationService.RecalculateVersion(selectedVersionID)
            _mainForm.RemoveTabFromTabControl($"PSE_{selectedProjectID}_{selectedVersionID}")
        Catch ex As Exception
            UIHelper.Add($"Status: Error closing form for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error closing form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            UIHelper.HideBusy(_mainForm)
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
                    ' Only add to list if it matches current filter (ProductTypeID)
                    Dim currentProductTypeID As Integer = -1
                    If TreeViewLevels.SelectedNode IsNot Nothing AndAlso TreeViewLevels.SelectedNode.Level = 2 Then
                        Dim levelTag = DirectCast(TreeViewLevels.SelectedNode.Tag, Dictionary(Of String, Object))
                        currentProductTypeID = CInt(levelTag("ProductTypeID"))
                    End If
                    If currentProductTypeID = -1 OrElse actualUnit.ProductTypeID = currentProductTypeID Then
                        existingActualUnits.Add(actualUnit)
                        ' DrawItem handles display - just add placeholder
                        ListboxExistingActualUnits.Items.Add(actualUnit.UnitName)
                    End If
                    UIHelper.Add("New unit saved successfully—available for reuse.")
                    ' Optional attach based on user choice
                    If chkAttachToLevel.Checked AndAlso selectedLevelID > 0 AndAlso CInt(TxtActualUnitQuantity.Text) > 0 Then
                        Dim newMapping As New ActualToLevelMappingModel With {
                            .VersionID = selectedVersionID,
                            .ActualUnitID = actualUnit.ActualUnitID,
                            .LevelID = selectedLevelID,
                            .Quantity = CInt(TxtActualUnitQuantity.Text)
                        }
                        dataAccess.SaveActualToLevelMapping(newMapping)
                        LoadAssignedUnits()
                        UpdateRollups()
                    End If
                Case Else
                    UIHelper.Add($"Status: Invalid save mode for version {selectedVersionID}.")
                    MessageBox.Show("Invalid operation mode.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
            End Select
            ResetDetailsPane()
            currentdetailsMode = DetailsMode.None
        Catch ex As Exception
            UIHelper.Add($"Status: Error saving for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error saving: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ListboxExistingActualUnits_MouseDown(sender As Object, e As MouseEventArgs) Handles ListboxExistingActualUnits.MouseDown
        Try
            ' Handle right-click context menu (existing logic)
            If e.Button = MouseButtons.Right AndAlso ListboxExistingActualUnits.SelectedIndex >= 0 Then
                If ListboxExistingActualUnits.ContextMenuStrip Is Nothing Then
                    UIHelper.Add($"Status: Context menu not configured for version {selectedVersionID}.")
                    MessageBox.Show("Context menu not configured.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                ListboxExistingActualUnits.ContextMenuStrip.Show(ListboxExistingActualUnits, e.Location)
                UIHelper.Add($"Status: Context menu opened for version {selectedVersionID}.")
            ElseIf e.Button = MouseButtons.Left Then
                ' Start drag operation
                Dim index As Integer = ListboxExistingActualUnits.IndexFromPoint(e.Location)
                If index >= 0 Then
                    dragStartIndex = index
                    isDragging = False ' Will be set true on MouseMove if threshold met
                End If
            End If
        Catch ex As Exception
            UIHelper.Add($"Status: Error in mouse down for version {selectedVersionID}: {ex.Message}")
        End Try
    End Sub
    Private Sub ListboxExistingActualUnits_MouseMove(sender As Object, e As MouseEventArgs) Handles ListboxExistingActualUnits.MouseMove
        If e.Button = MouseButtons.Left AndAlso dragStartIndex >= 0 AndAlso Not isDragging Then
            ' Start drag if moved enough (prevents accidental drags)
            If Math.Abs(e.Y - ListboxExistingActualUnits.GetItemRectangle(dragStartIndex).Y) > 5 Then
                isDragging = True
                ListboxExistingActualUnits.DoDragDrop(dragStartIndex, DragDropEffects.Move)
            End If
        End If
    End Sub

    Private Sub ListboxExistingActualUnits_MouseUp(sender As Object, e As MouseEventArgs) Handles ListboxExistingActualUnits.MouseUp
        dragStartIndex = -1
        isDragging = False
    End Sub

    Private Sub ListboxExistingActualUnits_DragOver(sender As Object, e As DragEventArgs) Handles ListboxExistingActualUnits.DragOver
        e.Effect = DragDropEffects.Move
    End Sub

    Private Sub ListboxExistingActualUnits_DragDrop(sender As Object, e As DragEventArgs) Handles ListboxExistingActualUnits.DragDrop
        Try
            Dim targetPoint As Point = ListboxExistingActualUnits.PointToClient(New Point(e.X, e.Y))
            Dim targetIndex As Integer = ListboxExistingActualUnits.IndexFromPoint(targetPoint)

            ' If dropped below all items, target is last position
            If targetIndex < 0 Then
                targetIndex = ListboxExistingActualUnits.Items.Count - 1
            End If

            Dim sourceIndex As Integer = CInt(e.Data.GetData(GetType(Integer)))

            If sourceIndex <> targetIndex AndAlso sourceIndex >= 0 Then
                ' Reorder the backing list
                Dim movedUnit As ActualUnitModel = existingActualUnits(sourceIndex)
                existingActualUnits.RemoveAt(sourceIndex)
                existingActualUnits.Insert(targetIndex, movedUnit)

                ' Reorder the listbox items
                Dim movedItem As Object = ListboxExistingActualUnits.Items(sourceIndex)
                ListboxExistingActualUnits.Items.RemoveAt(sourceIndex)
                ListboxExistingActualUnits.Items.Insert(targetIndex, movedItem)

                ' Select the moved item
                ListboxExistingActualUnits.SelectedIndex = targetIndex

                ' Persist the new sort order to database
                SaveActualUnitsSortOrder()

                UIHelper.Add($"Status: Reordered '{movedUnit.UnitName}' from position {sourceIndex + 1} to {targetIndex + 1}.")
            End If
        Catch ex As Exception
            UIHelper.Add($"Status: Error during drag-drop reorder: {ex.Message}")
            MessageBox.Show("Error reordering items: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            dragStartIndex = -1
            isDragging = False
        End Try
    End Sub

    ''' <summary>
    ''' Persists the current sort order of actual units to the database.
    ''' </summary>
    Private Sub SaveActualUnitsSortOrder()
        Try
            Using conn As SqlConnection = SqlConnectionManager.Instance.GetConnection()
                Using tran As SqlTransaction = conn.BeginTransaction()
                    Try
                        For i As Integer = 0 To existingActualUnits.Count - 1
                            Dim unit As ActualUnitModel = existingActualUnits(i)
                            unit.SortOrder = i + 1 ' 1-based sort order

                            Using cmd As New SqlCommand(Queries.UpdateActualUnitsSortOrderBatch, conn, tran)
                                cmd.Parameters.AddWithValue("@SortOrder", unit.SortOrder)
                                cmd.Parameters.AddWithValue("@ActualUnitID", unit.ActualUnitID)
                                cmd.Parameters.AddWithValue("@VersionID", selectedVersionID)
                                cmd.ExecuteNonQuery()
                            End Using
                        Next
                        tran.Commit()
                        UIHelper.Add($"Status: Sort order saved for {existingActualUnits.Count} units.")
                    Catch ex As Exception
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using
        Catch ex As Exception
            UIHelper.Add($"Status: Error saving sort order: {ex.Message}")
            MessageBox.Show("Error saving sort order: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub EditToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EditToolStripMenuItem.Click, ListboxExistingActualUnits.DoubleClick
        Try
            Dim selectedActual = GetSelectedActualUnit()
            If selectedActual Is Nothing Then Return

            selectedActualUnitID = selectedActual.ActualUnitID
            currentdetailsMode = DetailsMode.EditGlobalUnit
            PopulateDetailsPane(selectedActual, True, 0)
            UIHelper.Add($"Status: Editing actual unit {selectedActual.UnitName} for version {selectedVersionID}.")
        Catch ex As Exception
            UIHelper.Add($"Status: Error editing actual unit for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error editing actual unit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DeleteActualUnitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteActualUnitToolStripMenuItem.Click
        Try
            Dim selectedActual = GetSelectedActualUnit()
            If selectedActual Is Nothing Then Return

            Dim mappings = dataAccess.GetActualToLevelMappingsByActualUnitID(selectedActual.ActualUnitID)
            If mappings.Any() Then
                Dim result = MessageBox.Show($"The unit '{selectedActual.UnitName}' is used in {mappings.Count} level(s). Deleting will remove it from all levels. Continue?",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                If result <> DialogResult.Yes Then Return
            End If

            dataAccess.DeleteActualUnit(selectedActual.ActualUnitID)
            existingActualUnits.RemoveAt(ListboxExistingActualUnits.SelectedIndex)
            ListboxExistingActualUnits.Items.RemoveAt(ListboxExistingActualUnits.SelectedIndex)
            If selectedLevelID > 0 Then LoadAssignedUnits()
            UIHelper.Add($"Status: Actual unit '{selectedActual.UnitName}' deleted for version {selectedVersionID}.")
        Catch ex As ApplicationException
            MessageBox.Show(ex.Message, "Deletion Blocked", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Catch ex As Exception
            UIHelper.Add($"Status: Error deleting unit for version {selectedVersionID}: {ex.Message}")
            MessageBox.Show("Error deleting unit: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
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

    Private Sub TreeViewLevels_MouseDown(sender As Object, e As MouseEventArgs) Handles TreeViewLevels.MouseDown
        If e.Button <> MouseButtons.Right Then Return

        Dim clickedNode As TreeNode = TreeViewLevels.GetNodeAt(e.X, e.Y)
        If clickedNode Is Nothing Then Return

        TreeViewLevels.SelectedNode = clickedNode
        Dim tag As Dictionary(Of String, Object) = DirectCast(clickedNode.Tag, Dictionary(Of String, Object))
        Dim nodeType As String = CStr(tag("Type"))

        ' Hide all menu items first
        mnuCopyUnits.Visible = False
        mnuPasteUnits.Visible = False
        mnuCopyBuildingUnits.Visible = False
        mnuPasteBuildingUnits.Visible = False
        mnuClearBuildingCopy.Enabled = False

        ' Show only the relevant ones
        If nodeType = "Level" Then
            mnuCopyUnits.Visible = True
            mnuPasteUnits.Visible = True
            mnuPasteUnits.Enabled = (copiedMappings.Count > 0)
        ElseIf nodeType = "Building" Then
            mnuCopyBuildingUnits.Visible = True
            mnuPasteBuildingUnits.Visible = True
            mnuPasteBuildingUnits.Enabled = (copiedBuildingMappings.Count > 0)
        End If

        mnuClearBuildingCopy.Enabled = (copiedMappings.Count > 0) Or (copiedBuildingMappings.Count > 0)

        ' Show the menu
        ContextMenuLevels.Show(TreeViewLevels, e.Location)
    End Sub

    Private Sub mnuCopyBuildingUnits_Click(sender As Object, e As EventArgs) Handles mnuCopyBuildingUnits.Click
        Dim selectedNode As TreeNode = TreeViewLevels.SelectedNode
        If selectedNode Is Nothing Then
            MessageBox.Show("No building selected.", "Copy Units", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim tag As Dictionary(Of String, Object) = DirectCast(selectedNode.Tag, Dictionary(Of String, Object))
        If CStr(tag("Type")) <> "Building" Then
            MessageBox.Show("Please right-click on a Building node to copy from.", "Copy Units", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim buildingID As Integer = CInt(tag("ID"))

        Try
            ' Clear any previous copy
            sourceBuildingStructure.Clear()
            copiedBuildingMappings.Clear()

            ' Fetch and sort levels by LevelNumber (LevelNumber is non-nullable Integer)
            Dim levels As List(Of LevelModel) = ProjectDataAccess.GetLevelsByBuildingID(buildingID) _
                .OrderBy(Function(l) l.LevelNumber) _
                .ToList()

            If levels.Count = 0 Then
                MessageBox.Show("This building has no levels to copy.", "Copy Units", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim totalMappingsCopied As Integer = 0

            For Each lvl As LevelModel In levels
                ' Store the structure for paste-time validation
                sourceBuildingStructure.Add(lvl.ProductTypeID)

                ' Get mappings for this level
                Dim mappings As List(Of ActualToLevelMappingModel) = ProjectDataAccess.GetActualToLevelMappingsByLevelID(lvl.LevelID)

                Dim thisLevelList As New List(Of Tuple(Of Integer, Integer))
                For Each m As ActualToLevelMappingModel In mappings
                    thisLevelList.Add(New Tuple(Of Integer, Integer)(m.ActualUnitID, m.Quantity))
                    totalMappingsCopied += 1
                Next

                copiedBuildingMappings.Add(thisLevelList)
            Next

            ' Success feedback
            UIHelper.Add($"Status: Copied {totalMappingsCopied} unit mappings from {levels.Count} levels in Building ID {buildingID} (Version {selectedVersionID}). Ready to paste to a matching building.")
            MessageBox.Show($"Copied {totalMappingsCopied} unit assignments from {levels.Count} levels.{vbCrLf}You can now paste to another building with the identical level structure.",
                        "Building Units Copied", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            UIHelper.Add("Status: Error copying building units: " & ex.Message)
            MessageBox.Show("Error copying building units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub mnuPasteBuildingUnits_Click(sender As Object, e As EventArgs) Handles mnuPasteBuildingUnits.Click
        Dim selectedNode As TreeNode = TreeViewLevels.SelectedNode
        If selectedNode Is Nothing Then
            MessageBox.Show("No building selected.", "Paste Units", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim tag As Dictionary(Of String, Object) = DirectCast(selectedNode.Tag, Dictionary(Of String, Object))
        If CStr(tag("Type")) <> "Building" Then
            MessageBox.Show("Please right-click on a Building node to paste to.", "Paste Units", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim targetBuildingID As Integer = CInt(tag("ID"))

        ' Safety: Must have copied building data
        If sourceBuildingStructure.Count = 0 OrElse copiedBuildingMappings.Count = 0 Then
            MessageBox.Show("No building units have been copied yet.", "Paste Units", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Safety: Must be same version
        If selectedVersionID <= 0 Then
            MessageBox.Show("Invalid version selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Try
            ' Get target levels — same ordering as source
            Dim targetLevels As List(Of LevelModel) = ProjectDataAccess.GetLevelsByBuildingID(targetBuildingID) _
                .OrderBy(Function(l) l.LevelNumber) _
                .ToList()

            ' CRITICAL: Structure must match exactly
            If targetLevels.Count <> sourceBuildingStructure.Count Then
                MessageBox.Show($"Cannot paste: Target building has {targetLevels.Count} levels, but source has {sourceBuildingStructure.Count}.{vbCrLf}Level count must match exactly.",
                                "Structure Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                Return
            End If

            For i As Integer = 0 To targetLevels.Count - 1
                If targetLevels(i).ProductTypeID <> sourceBuildingStructure(i) Then
                    MessageBox.Show($"Cannot paste: Level {i + 1} Product Type mismatch.{vbCrLf}" &
                                    $"Source: {GetProductTypeName(sourceBuildingStructure(i))} ({sourceBuildingStructure(i)}){vbCrLf}" &
                                    $"Target: {GetProductTypeName(targetLevels(i).ProductTypeID)} ({targetLevels(i).ProductTypeID})",
                                    "Structure Mismatch", MessageBoxButtons.OK, MessageBoxIcon.Stop)
                    Return
                End If
            Next

            ' Confirmation
            Dim result = MessageBox.Show($"Paste {copiedBuildingMappings.Sum(Function(l) l.Count)} unit assignments to {targetLevels.Count} levels in this building?{vbCrLf}{vbCrLf}This will overwrite existing assignments on matching levels.",
                                        "Confirm Paste to Building", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result <> DialogResult.Yes Then Return

            ' Begin transaction
            Using conn = SqlConnectionManager.Instance.GetConnection()
                Using tran = conn.BeginTransaction()
                    Try
                        For i As Integer = 0 To targetLevels.Count - 1
                            Dim targetLevelID As Integer = targetLevels(i).LevelID
                            Dim sourceMappings As List(Of Tuple(Of Integer, Integer)) = copiedBuildingMappings(i)

                            ' Get current mappings for this level
                            Dim currentMappings = ProjectDataAccess.GetActualToLevelMappingsByLevelID(targetLevelID)

                            For Each src In sourceMappings
                                Dim actualUnitID = src.Item1
                                Dim quantity = src.Item2

                                Dim existing = currentMappings.FirstOrDefault(Function(m) m.ActualUnitID = actualUnitID)
                                If existing IsNot Nothing Then
                                    ' Update quantity
                                    existing.Quantity = quantity
                                    dataAccess.SaveActualToLevelMapping(existing)
                                Else
                                    ' Insert new
                                    Dim newMap As New ActualToLevelMappingModel With {
                                        .VersionID = selectedVersionID,
                                        .ActualUnitID = actualUnitID,
                                        .LevelID = targetLevelID,
                                        .Quantity = quantity
                                    }
                                    dataAccess.SaveActualToLevelMapping(newMap)
                                End If
                            Next

                            ' Optional: Remove any mappings that exist in target but not in source?
                            ' (Current behavior: leave them — safer)
                            ' If you want full replace, uncomment below:
                            ' Dim toDelete = currentMappings.Where(Function(m) Not sourceMappings.Any(Function(s) s.Item1 = m.ActualUnitID))
                            ' For Each del In toDelete
                            '     dataAccess.DeleteActualToLevelMapping(del.MappingID, conn, tran)
                            ' Next
                        Next

                        tran.Commit()
                    Catch ex As Exception
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            ' Refresh UI and rollups
            LoadAssignedUnits()
            For Each lvl In targetLevels
                RollupCalculationService.UpdateLevelRollups(lvl.LevelID)
            Next
            If targetBuildingID > 0 Then
                RollupCalculationService.UpdateBuildingRollups(targetBuildingID)
            End If

            ' Clear copy buffer
            'sourceBuildingStructure.Clear()
            'copiedBuildingMappings.Clear()

            UIHelper.Add($"Status: Successfully pasted unit assignments to Building ID {targetBuildingID} (Version {selectedVersionID}).")
            MessageBox.Show("Building unit assignments pasted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            UIHelper.Add("Status: Error pasting building units: " & ex.Message)
            MessageBox.Show("Error pasting building units: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Function GetProductTypeName(id As Integer) As String
        Dim types = dataAccess.GetProductTypes()
        Dim pt = types.FirstOrDefault(Function(t) t.ProductTypeID = id)
        Return If(pt IsNot Nothing, pt.ProductTypeName, "Unknown")
    End Function

    Private Sub mnuClearBuildingCopy_Click(sender As Object, e As EventArgs) Handles mnuClearBuildingCopy.Click
        copiedMappings.Clear()
        sourceBuildingStructure.Clear()
        copiedBuildingMappings.Clear()
        UIHelper.Add("Status: copy buffer cleared.")
    End Sub

    ''' <summary>
    ''' Inserts a RawUnitLumberHistory record for the given model within the provided transaction.
    ''' </summary>
    Private Sub InsertRawUnitLumberHistory(model As RawUnitModel, conn As SqlConnection, transaction As SqlTransaction)
        ' Fetch the CostEffectiveDateID based on AvgSPFNo2
        Dim costEffectiveID As Object = DBNull.Value
        If model.AvgSPFNo2.HasValue Then
            Dim fetchParams As SqlParameter() = {New SqlParameter("@SPFLumberCost", model.AvgSPFNo2.Value)}
            costEffectiveID = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectCostEffectiveDateIDByCost, fetchParams)
        End If

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

        Using cmdHistory As New SqlCommand(Queries.InsertRawUnitLumberHistory, conn, transaction) With {.CommandTimeout = 0}
            cmdHistory.Parameters.AddRange(HelperDataAccess.BuildParameters(historyParams))
            Dim historyID As Integer = CInt(cmdHistory.ExecuteScalar())
            Debug.WriteLine($"Created RawUnitLumberHistory for RawUnitID: {model.RawUnitID}, HistoryID: {historyID}")
        End Using
    End Sub
    ''' <summary>
    ''' Parses a nullable decimal value from CSV row data (case-insensitive key lookup).
    ''' </summary>
    Private Shared Function ParseCsvDecimal(rowData As Dictionary(Of String, String), key As String) As Decimal?
        Dim decVal As Decimal
        Dim upperKey As String = key.ToUpper()
        If rowData.ContainsKey(upperKey) AndAlso Decimal.TryParse(rowData(upperKey), decVal) Then
            Return decVal
        End If
        Return Nothing
    End Function

    ''' <summary>
    ''' Populates a RawUnitModel from CSV row data using standardized field mapping.
    ''' </summary>
    Private Shared Sub PopulateRawUnitFromCsv(model As RawUnitModel, rowData As Dictionary(Of String, String))
        ' Required fields
        model.BF = ParseCsvDecimal(rowData, "BF")
        model.LF = ParseCsvDecimal(rowData, "LF")
        model.EWPLF = ParseCsvDecimal(rowData, "EWPLF")
        model.SqFt = ParseCsvDecimal(rowData, "SQFT")
        model.FCArea = ParseCsvDecimal(rowData, "FCAREA")
        model.LumberCost = ParseCsvDecimal(rowData, "LUMBERCOST")
        model.PlateCost = ParseCsvDecimal(rowData, "PLATECOST")
        model.ManufLaborCost = ParseCsvDecimal(rowData, "MANUFLABORCOST")
        model.DesignLabor = ParseCsvDecimal(rowData, "DESIGNLABOR")
        model.MGMTLabor = ParseCsvDecimal(rowData, "MGMTLABOR")
        model.JobSuppliesCost = ParseCsvDecimal(rowData, "JOBSUPPLIESCOST")
        model.ManHours = ParseCsvDecimal(rowData, "MANHOURS")
        model.ItemCost = ParseCsvDecimal(rowData, "ITEMCOST")
        model.OverallCost = ParseCsvDecimal(rowData, "OVERALLCOST")
        model.DeliveryCost = ParseCsvDecimal(rowData, "DELIVERYCOST")
        model.TotalSellPrice = ParseCsvDecimal(rowData, "TOTALSELLPRICE")
        model.AvgSPFNo2 = ParseCsvDecimal(rowData, "AVGSPFNO2")

        ' Optional fields for Component Export
        model.SPFNo2BDFT = ParseCsvDecimal(rowData, "SPFNO2BDFT")
        model.Avg241800 = ParseCsvDecimal(rowData, "AVG2X4-1800")
        model.MSR241800BDFT = ParseCsvDecimal(rowData, "2X4-1800BDFT")
        model.Avg242400 = ParseCsvDecimal(rowData, "AVG2X4-2400")
        model.MSR242400BDFT = ParseCsvDecimal(rowData, "2X4-2400BDFT")
        model.Avg261800 = ParseCsvDecimal(rowData, "AVG2X6-1800")
        model.MSR261800BDFT = ParseCsvDecimal(rowData, "2X6-1800BDFT")
        model.Avg262400 = ParseCsvDecimal(rowData, "AVG2X6-2400")
        model.MSR262400BDFT = ParseCsvDecimal(rowData, "2X6-2400BDFT")
    End Sub

    ''' <summary>
    ''' Creates a standard read-only text column for the DataGridView.
    ''' </summary>
    Private Function CreateTextColumn(dataProperty As String, header As String,
                                       Optional visible As Boolean = False,
                                       Optional sortable As Boolean = False,
                                       Optional name As String = Nothing) As DataGridViewTextBoxColumn
        Return New DataGridViewTextBoxColumn With {
            .Name = If(name, dataProperty),
            .DataPropertyName = dataProperty,
            .HeaderText = header,
            .ReadOnly = True,
            .Visible = visible,
            .SortMode = If(sortable, DataGridViewColumnSortMode.Automatic, DataGridViewColumnSortMode.NotSortable)
        }
    End Function

    ''' <summary>
    ''' Updates level and building rollups for the current selection.
    ''' </summary>
    Private Sub UpdateRollups()
        If selectedLevelID > 0 Then RollupCalculationService.UpdateLevelRollups(selectedLevelID)
        If selectedBuildingID > 0 Then RollupCalculationService.UpdateBuildingRollups(selectedBuildingID)
    End Sub
    ''' <summary>
    ''' Resets all per-SQFT preview textboxes to zero.
    ''' </summary>
    Private Sub ClearPerSQFTTextBoxes()
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
    End Sub

    ''' <summary>
    ''' Gets the selected actual unit from the listbox, returning Nothing if invalid.
    ''' </summary>
    Private Function GetSelectedActualUnit(Optional showError As Boolean = True) As ActualUnitModel
        If ListboxExistingActualUnits.SelectedIndex < 0 Then
            If showError Then MessageBox.Show("Please select an existing actual unit from the list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return Nothing
        End If
        Return existingActualUnits(ListboxExistingActualUnits.SelectedIndex)
    End Function
End Class