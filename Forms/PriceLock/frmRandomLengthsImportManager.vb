' =====================================================
' frmRandomLengthsImportManager.vb
' Manage Random Lengths pricing imports
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Linq
Imports System.IO
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Public Class frmRandomLengthsImportManager

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _imports As List(Of PLRandomLengthsImport)
    Private _pricingTypes As List(Of PLRandomLengthsPricingType)
    Private _currentImport As PLRandomLengthsImport
    Private _currentPricing As List(Of PLRandomLengthsPricing)
    Private _isDirty As Boolean = False

#End Region

#Region "Constructor"

    Public Sub New()
        _dataAccess = New PriceLockDataAccess()
        InitializeComponent()
        SetupGrids()
    End Sub

    Public Sub New(dataAccess As PriceLockDataAccess)
        _dataAccess = dataAccess
        InitializeComponent()
        SetupGrids()
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmRandomLengthsImportManager_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadPricingTypes()
        LoadImports()
        UpdateButtonStates()
    End Sub

    Private Sub frmRandomLengthsImportManager_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If _isDirty Then
            Dim result = MessageBox.Show("You have unsaved changes. Do you want to save before closing?",
                "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                SaveCurrentPricing()
            ElseIf result = DialogResult.Cancel Then
                e.Cancel = True
            End If
        End If
    End Sub

#End Region

#Region "Grid Setup"

    Private Sub SetupGrids()
        ' Setup Imports Grid
        dgvImports.AutoGenerateColumns = False
        dgvImports.Columns.Clear()
        dgvImports.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvImports.MultiSelect = False
        dgvImports.AllowUserToAddRows = False
        dgvImports.AllowUserToDeleteRows = False
        dgvImports.ReadOnly = True

        dgvImports.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "RandomLengthsImportID",
            .HeaderText = "ID",
            .Width = 50,
            .Visible = False
        })

        dgvImports.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "ReportDate",
            .HeaderText = "Report Date",
            .Width = 100
        })

        dgvImports.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "ReportName",
            .HeaderText = "Name",
            .Width = 150
        })

        dgvImports.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PriceCount",
            .HeaderText = "Prices",
            .Width = 60
        })

        dgvImports.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "ImportMethod",
            .HeaderText = "Method",
            .Width = 70
        })

        dgvImports.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "ImportedBy",
            .HeaderText = "Imported By",
            .Width = 100
        })

        dgvImports.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "ImportedDate",
            .HeaderText = "Imported",
            .Width = 120
        })

        ' Setup Pricing Grid
        dgvPricing.AutoGenerateColumns = False
        dgvPricing.Columns.Clear()
        dgvPricing.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvPricing.AllowUserToAddRows = False
        dgvPricing.AllowUserToDeleteRows = False

        dgvPricing.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "RLPricingTypeID",
            .HeaderText = "ID",
            .Visible = False
        })

        dgvPricing.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "CategoryGroup",
            .HeaderText = "Category",
            .Width = 100,
            .ReadOnly = True
        })

        dgvPricing.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "TypeCode",
            .HeaderText = "Code",
            .Width = 120,
            .ReadOnly = True
        })

        dgvPricing.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "TypeName",
            .HeaderText = "Description",
            .Width = 180,
            .ReadOnly = True
        })

        dgvPricing.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Price",
            .HeaderText = "Price (MBF)",
            .Width = 100,
            .DefaultCellStyle = New DataGridViewCellStyle() With {
                .Format = "N2",
                .Alignment = DataGridViewContentAlignment.MiddleRight
            }
        })

        dgvPricing.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Notes",
            .HeaderText = "Notes",
            .Width = 150
        })
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadPricingTypes()
        Try
            _pricingTypes = _dataAccess.GetRLPricingTypes()
        Catch ex As Exception
            MessageBox.Show($"Error loading pricing types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            _pricingTypes = New List(Of PLRandomLengthsPricingType)()
        End Try
    End Sub

    Private Sub LoadImports()
        Try
            Cursor = Cursors.WaitCursor
            _imports = _dataAccess.GetRandomLengthsImports()

            dgvImports.Rows.Clear()
            For Each imp In _imports
                Dim rowIndex = dgvImports.Rows.Add()
                Dim row = dgvImports.Rows(rowIndex)
                row.Cells("RandomLengthsImportID").Value = imp.RandomLengthsImportID
                row.Cells("ReportDate").Value = imp.ReportDate.ToString("MM/dd/yyyy")
                row.Cells("ReportName").Value = imp.ReportName
                row.Cells("PriceCount").Value = imp.PriceCount
                row.Cells("ImportMethod").Value = imp.ImportMethod
                row.Cells("ImportedBy").Value = imp.ImportedBy
                row.Cells("ImportedDate").Value = imp.ImportedDate.ToString("MM/dd/yyyy HH:mm")
            Next

            lblImportCount.Text = $"{_imports.Count} import(s)"

        Catch ex As Exception
            MessageBox.Show($"Error loading imports: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub LoadPricingForImport(importID As Integer)
        Try
            Cursor = Cursors.WaitCursor

            _currentImport = _imports.FirstOrDefault(Function(i) i.RandomLengthsImportID = importID)
            If _currentImport Is Nothing Then
                ClearPricingGrid()
                Return
            End If

            ' Load existing pricing for this import
            _currentPricing = _dataAccess.GetRandomLengthsPricingByImport(importID)

            ' Build grid with all pricing types, showing existing values where available
            dgvPricing.Rows.Clear()

            Dim currentGroup As String = ""
            For Each pType In _pricingTypes.OrderBy(Function(t) t.DisplayOrder).ThenBy(Function(t) t.CategoryGroup)
                ' Add group header row if category changed
                If pType.CategoryGroup <> currentGroup Then
                    currentGroup = pType.CategoryGroup
                    ' Optionally add a visual separator - skip for now
                End If

                Dim existing = _currentPricing.FirstOrDefault(Function(p) p.RLPricingTypeID = pType.RLPricingTypeID)

                Dim rowIndex = dgvPricing.Rows.Add()
                Dim row = dgvPricing.Rows(rowIndex)
                row.Cells("RLPricingTypeID").Value = pType.RLPricingTypeID
                row.Cells("CategoryGroup").Value = pType.CategoryGroup
                row.Cells("TypeCode").Value = pType.TypeCode
                row.Cells("TypeName").Value = pType.TypeName
                row.Cells("Price").Value = If(existing IsNot Nothing, CObj(existing.Price), DBNull.Value)
                row.Cells("Notes").Value = If(existing IsNot Nothing, existing.Notes, Nothing)

                ' Color code by category group
                row.DefaultCellStyle.BackColor = GetCategoryColor(pType.CategoryGroup)
            Next

            ' Update header info
            grpPricingDetail.Text = $"Pricing Detail - {_currentImport.ReportDate:MM/dd/yyyy} {_currentImport.ReportName}"
            _isDirty = False

        Catch ex As Exception
            MessageBox.Show($"Error loading pricing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub ClearPricingGrid()
        dgvPricing.Rows.Clear()
        _currentImport = Nothing
        _currentPricing = Nothing
        grpPricingDetail.Text = "Pricing Detail"
        _isDirty = False
    End Sub

    Private Function GetCategoryColor(categoryGroup As String) As Color
        Select Case categoryGroup.ToUpper()
            Case "FRAMING LUMBER"
                Return Color.FromArgb(255, 255, 240) ' Light yellow
            Case "STUDS"
                Return Color.FromArgb(240, 255, 240) ' Light green
            Case "OSB"
                Return Color.FromArgb(240, 248, 255) ' Alice blue
            Case "SPF"
                Return Color.FromArgb(255, 240, 245) ' Lavender blush
            Case "MSR"
                Return Color.FromArgb(245, 245, 220) ' Beige
            Case "TREATED"
                Return Color.FromArgb(240, 255, 255) ' Azure
            Case "EWP"
                Return Color.FromArgb(255, 250, 240) ' Floral white
            Case "WIDES"
                Return Color.FromArgb(248, 248, 255) ' Ghost white
            Case Else
                Return Color.White
        End Select
    End Function

#End Region

#Region "Button Events"

    Private Sub btnNewImport_Click(sender As Object, e As EventArgs) Handles btnNewImport.Click
        CreateNewImport()
    End Sub

    Private Sub btnEditImport_Click(sender As Object, e As EventArgs) Handles btnEditImport.Click
        EditSelectedImport()
    End Sub

    Private Sub btnDeleteImport_Click(sender As Object, e As EventArgs) Handles btnDeleteImport.Click
        DeleteSelectedImport()
    End Sub

    Private Sub btnSavePricing_Click(sender As Object, e As EventArgs) Handles btnSavePricing.Click
        SaveCurrentPricing()
    End Sub

    Private Sub btnCopyFromPrevious_Click(sender As Object, e As EventArgs) Handles btnCopyFromPrevious.Click
        CopyFromPreviousImport()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub dgvImports_SelectionChanged(sender As Object, e As EventArgs) Handles dgvImports.SelectionChanged
        If dgvImports.SelectedRows.Count > 0 Then
            ' Check for unsaved changes
            If _isDirty Then
                Dim result = MessageBox.Show("You have unsaved changes. Do you want to save before switching?",
                    "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    SaveCurrentPricing()
                ElseIf result = DialogResult.Cancel Then
                    Return
                End If
            End If

            Dim importID = CInt(dgvImports.SelectedRows(0).Cells("RandomLengthsImportID").Value)
            LoadPricingForImport(importID)
        End If
        UpdateButtonStates()
    End Sub

    Private Sub dgvPricing_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPricing.CellValueChanged
        If e.RowIndex >= 0 Then
            _isDirty = True
        End If
    End Sub

    Private Sub dgvPricing_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvPricing.CellFormatting
        ' Highlight empty price cells
        If e.RowIndex >= 0 AndAlso dgvPricing.Columns(e.ColumnIndex).Name = "Price" Then
            If e.Value Is Nothing OrElse e.Value Is DBNull.Value Then
                e.CellStyle.BackColor = Color.MistyRose
            End If
        End If
    End Sub

#End Region

#Region "CRUD Operations"

    Private Sub CreateNewImport()
        Using frm As New frmRandomLengthsImportEdit(Nothing, _dataAccess)
            If frm.ShowDialog() = DialogResult.OK Then
                LoadImports()

                ' Select the new import
                For i = 0 To dgvImports.Rows.Count - 1
                    If CInt(dgvImports.Rows(i).Cells("RandomLengthsImportID").Value) = frm.SavedImportID Then
                        dgvImports.ClearSelection()
                        dgvImports.Rows(i).Selected = True
                        dgvImports.FirstDisplayedScrollingRowIndex = i
                        Exit For
                    End If
                Next
            End If
        End Using
    End Sub

    Private Sub EditSelectedImport()
        If dgvImports.SelectedRows.Count = 0 Then Return

        Dim importID = CInt(dgvImports.SelectedRows(0).Cells("RandomLengthsImportID").Value)
        Dim imp = _imports.FirstOrDefault(Function(i) i.RandomLengthsImportID = importID)
        If imp Is Nothing Then Return

        Using frm As New frmRandomLengthsImportEdit(imp, _dataAccess)
            If frm.ShowDialog() = DialogResult.OK Then
                LoadImports()
                LoadPricingForImport(importID)
            End If
        End Using
    End Sub

    Private Sub DeleteSelectedImport()
        If dgvImports.SelectedRows.Count = 0 Then Return

        Dim importID = CInt(dgvImports.SelectedRows(0).Cells("RandomLengthsImportID").Value)
        Dim imp = _imports.FirstOrDefault(Function(i) i.RandomLengthsImportID = importID)
        If imp Is Nothing Then Return

        ' Check if in use by any price locks
        ' TODO: Add query to check if import is referenced by any price locks

        Dim result = MessageBox.Show(
            $"Are you sure you want to delete the import from {imp.ReportDate:MM/dd/yyyy}?" & vbCrLf & vbCrLf &
            "This will also delete all pricing data for this import.",
            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Try
                ' Delete pricing first, then import
                _dataAccess.DeleteRandomLengthsPricingByImport(importID)
                _dataAccess.DeactivateRandomLengthsImport(importID, Environment.UserName)

                ClearPricingGrid()
                LoadImports()

                MessageBox.Show("Import deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show($"Error deleting import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub SaveCurrentPricing()
        If _currentImport Is Nothing Then
            MessageBox.Show("No import selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor
            Dim savedCount = 0
            Dim skippedCount = 0

            For Each row As DataGridViewRow In dgvPricing.Rows
                Dim rlPricingTypeID = CInt(row.Cells("RLPricingTypeID").Value)
                Dim priceCell = row.Cells("Price")
                Dim notesCell = row.Cells("Notes")

                ' Skip if no price entered
                If priceCell.Value Is Nothing OrElse priceCell.Value Is DBNull.Value Then
                    skippedCount += 1
                    Continue For
                End If

                Dim price As Decimal
                If Not Decimal.TryParse(priceCell.Value.ToString(), price) Then
                    skippedCount += 1
                    Continue For
                End If

                Dim pricing As New PLRandomLengthsPricing() With {
                    .RandomLengthsImportID = _currentImport.RandomLengthsImportID,
                    .RLPricingTypeID = rlPricingTypeID,
                    .Price = price,
                    .Notes = If(notesCell.Value IsNot Nothing, notesCell.Value.ToString(), Nothing)
                }

                ' Upsert the pricing
                _dataAccess.UpsertRandomLengthsPricing(pricing)
                savedCount += 1
            Next

            _isDirty = False
            LoadImports() ' Refresh to update price counts

            MessageBox.Show($"Saved {savedCount} price(s). Skipped {skippedCount} empty/invalid entries.",
                "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show($"Error saving pricing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub CopyFromPreviousImport()
        If _currentImport Is Nothing Then
            MessageBox.Show("Please select an import first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Find the previous import by date
        Dim previousImport = _imports _
            .Where(Function(i) i.ReportDate < _currentImport.ReportDate AndAlso i.PriceCount > 0) _
            .OrderByDescending(Function(i) i.ReportDate) _
            .FirstOrDefault()

        If previousImport Is Nothing Then
            MessageBox.Show("No previous import with pricing found.", "Not Found", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim result = MessageBox.Show(
            $"Copy pricing from {previousImport.ReportDate:MM/dd/yyyy} ({previousImport.PriceCount} prices)?" & vbCrLf & vbCrLf &
            "This will overwrite any existing values in the grid (but won't save until you click Save).",
            "Copy Pricing", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Try
                Cursor = Cursors.WaitCursor

                Dim previousPricing = _dataAccess.GetRandomLengthsPricingByImport(previousImport.RandomLengthsImportID)
                Dim copiedCount = 0

                For Each row As DataGridViewRow In dgvPricing.Rows
                    Dim rlPricingTypeID = CInt(row.Cells("RLPricingTypeID").Value)
                    Dim prevPrice = previousPricing.FirstOrDefault(Function(p) p.RLPricingTypeID = rlPricingTypeID)

                    If prevPrice IsNot Nothing Then
                        row.Cells("Price").Value = prevPrice.Price
                        copiedCount += 1
                    End If
                Next

                _isDirty = True
                MessageBox.Show($"Copied {copiedCount} price(s) from previous import.", "Copy Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Catch ex As Exception
                MessageBox.Show($"Error copying prices: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Finally
                Cursor = Cursors.Default
            End Try
        End If
    End Sub

#End Region

#Region "UI State"

    Private Sub UpdateButtonStates()
        Dim hasSelection = dgvImports.SelectedRows.Count > 0
        btnEditImport.Enabled = hasSelection
        btnDeleteImport.Enabled = hasSelection
        btnSavePricing.Enabled = hasSelection
        btnCopyFromPrevious.Enabled = hasSelection
        dgvPricing.Enabled = hasSelection
    End Sub

    ' [Keep all existing code, only add this new button handler after line 156]

    Private Sub btnImportFromExcel_Click(sender As Object, e As EventArgs) Handles btnImportfromExcel.Click
        ImportFromExcelDirect()
    End Sub

    ' [Add this new method after the CopyFromPreviousImport method, around line 290]

    Private Sub ImportFromExcelDirect()
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*"
            ofd.Title = "Select Random Lengths Pricing Excel File"
            ofd.CheckFileExists = True

            If ofd.ShowDialog() = DialogResult.OK Then
                Using frm As New frmRandomLengthsImportEdit(Nothing, _dataAccess)
                    ' Set the Excel path BEFORE showing the dialog
                    frm.SetupForExcelImport(ofd.FileName)

                    If frm.ShowDialog() = DialogResult.OK Then
                        LoadImports()

                        ' Select the new import
                        For i = 0 To dgvImports.Rows.Count - 1
                            If CInt(dgvImports.Rows(i).Cells("RandomLengthsImportID").Value) = frm.SavedImportID Then
                                dgvImports.ClearSelection()
                                dgvImports.Rows(i).Selected = True
                                dgvImports.FirstDisplayedScrollingRowIndex = i
                                Exit For
                            End If
                        Next
                    End If
                End Using
            End If
        End Using
    End Sub

#End Region

End Class
