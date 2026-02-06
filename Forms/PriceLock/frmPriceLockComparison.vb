' =====================================================
' frmPriceLockComparison.vb
' Compare two price locks side-by-side
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Partial Public Class frmPriceLockComparison
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _componentChanges As List(Of PLComponentPriceComparison)
    Private _materialChanges As List(Of PLMaterialPriceComparison)

#End Region

#Region "Constructor"

    Public Sub New()
        _dataAccess = New PriceLockDataAccess()
        InitializeComponent()
    End Sub

    Public Sub New(dataAccess As PriceLockDataAccess)
        _dataAccess = dataAccess
        InitializeComponent()
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmPriceLockComparison_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupComponentGrid()
        SetupMaterialGrid()
        LoadBuilders()
    End Sub

    Private Sub cboBuilder_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBuilder.SelectedIndexChanged
        LoadSubdivisions()
    End Sub

    Private Sub cboSubdivision_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSubdivision.SelectedIndexChanged
        LoadPriceLocks()
    End Sub

    Private Sub btnRunReport_Click(sender As Object, e As EventArgs) Handles btnRunReport.Click
        RunComparison()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        ExportToExcel()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

#End Region

#Region "Grid Setup"

    Private Sub SetupComponentGrid()
        dgvComponents.Columns.Clear()

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Description",
            .HeaderText = "Plan / Elevation / Option",
            .Width = 220
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "ProductType",
            .HeaderText = "Type",
            .Width = 50
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PreviousPrice",
            .HeaderText = "Previous",
            .Width = 80,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "CurrentPrice",
            .HeaderText = "Current",
            .Width = 80,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Change",
            .HeaderText = "Change",
            .Width = 80,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PctChange",
            .HeaderText = "% Change",
            .Width = 70,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "P1", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Status",
            .HeaderText = "Status",
            .Width = 80
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Note",
            .HeaderText = "Note",
            .Width = 150
        })
    End Sub

    Private Sub SetupMaterialGrid()
        dgvMaterials.Columns.Clear()

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Category",
            .HeaderText = "Category",
            .Width = 150
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PreviousPrice",
            .HeaderText = "Previous",
            .Width = 90,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "N4", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "CurrentPrice",
            .HeaderText = "Current",
            .Width = 90,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "N4", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Change",
            .HeaderText = "Change",
            .Width = 90,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "N4", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PctChange",
            .HeaderText = "% Change",
            .Width = 80,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "P1", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Status",
            .HeaderText = "Status",
            .Width = 80
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Note",
            .HeaderText = "Note",
            .Width = 150
        })
    End Sub

#End Region

#Region "Grid Formatting"

    Private Sub dgvComponents_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvComponents.CellFormatting
        If e.RowIndex < 0 Then Return

        ' Color code percent change
        If dgvComponents.Columns(e.ColumnIndex).Name = "PctChange" OrElse
           dgvComponents.Columns(e.ColumnIndex).Name = "Change" Then
            Dim pctCell = dgvComponents.Rows(e.RowIndex).Cells("PctChange")
            If pctCell.Value IsNot Nothing AndAlso TypeOf pctCell.Value Is Decimal Then
                Dim pct = CDec(pctCell.Value)
                If pct > 0.05D Then
                    e.CellStyle.ForeColor = Color.Red
                ElseIf pct < -0.05D Then
                    e.CellStyle.ForeColor = Color.Green
                ElseIf pct > 0 Then
                    e.CellStyle.ForeColor = Color.DarkOrange
                ElseIf pct < 0 Then
                    e.CellStyle.ForeColor = Color.DarkGreen
                End If
            End If
        End If

        ' Color code status
        If dgvComponents.Columns(e.ColumnIndex).Name = "Status" AndAlso e.Value IsNot Nothing Then
            Select Case e.Value.ToString()
                Case "New"
                    e.CellStyle.ForeColor = Color.Blue
                Case "Removed"
                    e.CellStyle.ForeColor = Color.Gray
                Case "Changed"
                    e.CellStyle.ForeColor = Color.DarkOrange
            End Select
        End If
    End Sub

    Private Sub dgvMaterials_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvMaterials.CellFormatting
        If e.RowIndex < 0 Then Return

        ' Color code percent change
        If dgvMaterials.Columns(e.ColumnIndex).Name = "PctChange" OrElse
           dgvMaterials.Columns(e.ColumnIndex).Name = "Change" Then
            Dim pctCell = dgvMaterials.Rows(e.RowIndex).Cells("PctChange")
            If pctCell.Value IsNot Nothing AndAlso TypeOf pctCell.Value Is Decimal Then
                Dim pct = CDec(pctCell.Value)
                If pct > 0.05D Then
                    e.CellStyle.ForeColor = Color.Red
                ElseIf pct < -0.05D Then
                    e.CellStyle.ForeColor = Color.Green
                ElseIf pct > 0 Then
                    e.CellStyle.ForeColor = Color.DarkOrange
                ElseIf pct < 0 Then
                    e.CellStyle.ForeColor = Color.DarkGreen
                End If
            End If
        End If

        ' Color code status
        If dgvMaterials.Columns(e.ColumnIndex).Name = "Status" AndAlso e.Value IsNot Nothing Then
            Select Case e.Value.ToString()
                Case "New"
                    e.CellStyle.ForeColor = Color.Blue
                Case "Removed"
                    e.CellStyle.ForeColor = Color.Gray
                Case "Changed"
                    e.CellStyle.ForeColor = Color.DarkOrange
            End Select
        End If
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadBuilders()
        cboBuilder.Items.Clear()
        For Each builder In _dataAccess.GetBuilders()
            cboBuilder.Items.Add(New ListItem(builder.BuilderName, builder.BuilderID))
        Next

        If cboBuilder.Items.Count > 0 Then
            cboBuilder.SelectedIndex = 0
        End If
    End Sub

    Private Sub LoadSubdivisions()
        cboSubdivision.Items.Clear()
        cboLockFrom.Items.Clear()
        cboLockTo.Items.Clear()

        Dim selectedBuilder = TryCast(cboBuilder.SelectedItem, ListItem)
        If selectedBuilder Is Nothing Then Return

        For Each sub_ In _dataAccess.GetSubdivisionsByBuilder(CInt(selectedBuilder.Value))
            cboSubdivision.Items.Add(New ListItem(sub_.SubdivisionName, sub_.SubdivisionID))
        Next

        If cboSubdivision.Items.Count > 0 Then
            cboSubdivision.SelectedIndex = 0
        End If
    End Sub

    Private Sub LoadPriceLocks()
        cboLockFrom.Items.Clear()
        cboLockTo.Items.Clear()

        Dim selectedSubdivision = TryCast(cboSubdivision.SelectedItem, ListItem)
        If selectedSubdivision Is Nothing Then Return

        Dim locks = _dataAccess.GetPriceLocksBySubdivision(CInt(selectedSubdivision.Value))

        For Each lock_ In locks.OrderByDescending(Function(l) l.PriceLockDate)
            Dim displayText = $"{lock_.PriceLockDate:d} - {lock_.PriceLockName} ({lock_.Status})"
            cboLockFrom.Items.Add(New ListItem(displayText, lock_.PriceLockID))
            cboLockTo.Items.Add(New ListItem(displayText, lock_.PriceLockID))
        Next

        ' Default: compare second-to-last vs last (if we have at least 2)
        If cboLockFrom.Items.Count >= 2 Then
            cboLockFrom.SelectedIndex = 1  ' Previous
            cboLockTo.SelectedIndex = 0    ' Current
        ElseIf cboLockFrom.Items.Count = 1 Then
            cboLockFrom.SelectedIndex = 0
            cboLockTo.SelectedIndex = 0
        End If
    End Sub

#End Region

#Region "Report Generation"

    Private Sub RunComparison()
        Dim lockFrom = TryCast(cboLockFrom.SelectedItem, ListItem)
        Dim lockTo = TryCast(cboLockTo.SelectedItem, ListItem)

        If lockFrom Is Nothing OrElse lockTo Is Nothing Then
            MessageBox.Show("Please select two price locks to compare.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If CInt(lockFrom.Value) = CInt(lockTo.Value) Then
            MessageBox.Show("Please select two different price locks to compare.", "Same Lock Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor

            ' Get comparison data
            _componentChanges = _dataAccess.CompareComponentPricing(CInt(lockFrom.Value), CInt(lockTo.Value))
            _materialChanges = _dataAccess.CompareMaterialPricing(CInt(lockFrom.Value), CInt(lockTo.Value))

            ' Display results
            DisplayComponentChanges()
            DisplayMaterialChanges()
            UpdateSummary()

            btnExport.Enabled = True

        Catch ex As Exception
            MessageBox.Show($"Error running comparison: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub DisplayComponentChanges()
        dgvComponents.Rows.Clear()

        For Each change In _componentChanges
            Dim status = GetChangeStatus(change.PreviousFinalPrice, change.CurrentFinalPrice)
            dgvComponents.Rows.Add(
                change.FullDescription,
                change.ProductTypeName,
                If(change.PreviousFinalPrice.HasValue, CObj(change.PreviousFinalPrice.Value), DBNull.Value),
                If(change.CurrentFinalPrice.HasValue, CObj(change.CurrentFinalPrice.Value), DBNull.Value),
                If(change.FinalPriceDiff.HasValue, CObj(change.FinalPriceDiff.Value), DBNull.Value),
                If(change.FinalPricePctChange.HasValue, CObj(change.FinalPricePctChange.Value), DBNull.Value),
                status,
                change.PriceNote
            )
        Next
    End Sub

    Private Sub DisplayMaterialChanges()
        dgvMaterials.Rows.Clear()

        For Each change In _materialChanges
            Dim status = GetChangeStatus(change.PreviousPrice, change.CurrentPrice)
            dgvMaterials.Rows.Add(
                change.CategoryName,
                If(change.PreviousPrice.HasValue, CObj(change.PreviousPrice.Value), DBNull.Value),
                If(change.CurrentPrice.HasValue, CObj(change.CurrentPrice.Value), DBNull.Value),
                If(change.PriceDiff.HasValue, CObj(change.PriceDiff.Value), DBNull.Value),
                If(change.PctChangeFromPrevious.HasValue, CObj(change.PctChangeFromPrevious.Value), DBNull.Value),
                status,
                change.PriceNote
            )
        Next
    End Sub

    Private Function GetChangeStatus(previousPrice As Decimal?, currentPrice As Decimal?) As String
        If Not previousPrice.HasValue AndAlso currentPrice.HasValue Then
            Return "New"
        ElseIf previousPrice.HasValue AndAlso Not currentPrice.HasValue Then
            Return "Removed"
        ElseIf previousPrice.HasValue AndAlso currentPrice.HasValue AndAlso previousPrice.Value <> currentPrice.Value Then
            Return "Changed"
        Else
            Return "Unchanged"
        End If
    End Function

    Private Sub UpdateSummary()
        ' Component summary
        Dim compChanged = _componentChanges.Where(Function(c) c.FinalPriceDiff.HasValue AndAlso c.FinalPriceDiff.Value <> 0).Count()
        Dim compNew = _componentChanges.Where(Function(c) Not c.PreviousFinalPrice.HasValue AndAlso c.CurrentFinalPrice.HasValue).Count()
        Dim compRemoved = _componentChanges.Where(Function(c) c.PreviousFinalPrice.HasValue AndAlso Not c.CurrentFinalPrice.HasValue).Count()
        Dim compAvgPct = If(_componentChanges.Where(Function(c) c.FinalPricePctChange.HasValue).Any(),
            _componentChanges.Where(Function(c) c.FinalPricePctChange.HasValue).Average(Function(c) c.FinalPricePctChange.Value), 0D)

        lblComponentSummary.Text = $"Component Changes: {compChanged} changed, {compNew} new, {compRemoved} removed (Avg: {compAvgPct:P1})"
        lblComponentSummary.ForeColor = If(compAvgPct > 0, Color.Red, If(compAvgPct < 0, Color.Green, Color.Black))

        ' Material summary
        Dim matChanged = _materialChanges.Where(Function(m) m.PriceDiff.HasValue AndAlso m.PriceDiff.Value <> 0).Count()
        Dim matNew = _materialChanges.Where(Function(m) Not m.PreviousPrice.HasValue AndAlso m.CurrentPrice.HasValue).Count()
        Dim matRemoved = _materialChanges.Where(Function(m) m.PreviousPrice.HasValue AndAlso Not m.CurrentPrice.HasValue).Count()
        Dim matAvgPct = If(_materialChanges.Where(Function(m) m.PctChangeFromPrevious.HasValue).Any(),
            _materialChanges.Where(Function(m) m.PctChangeFromPrevious.HasValue).Average(Function(m) m.PctChangeFromPrevious.Value), 0D)

        lblMaterialSummary.Text = $"Material Changes: {matChanged} changed, {matNew} new, {matRemoved} removed (Avg: {matAvgPct:P1})"
        lblMaterialSummary.ForeColor = If(matAvgPct > 0, Color.Red, If(matAvgPct < 0, Color.Green, Color.Black))
    End Sub

#End Region

#Region "Export"

    Private Sub ExportToExcel()
        ' TODO: Implement Excel export using skill
        MessageBox.Show("Excel export will be implemented using the xlsx skill.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

#End Region

End Class