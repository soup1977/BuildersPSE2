' =====================================================
' frmRandomLengthsImport.vb
' Import/Update Random Lengths pricing for materials
' UPDATED: Support for RL-based percent change calculations
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Linq
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Public Class frmRandomLengthsImport

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _priceLock As PLPriceLock
    Private _materialPricing As List(Of PLMaterialPricing)
    Private _categories As List(Of PLMaterialCategory)

#End Region

#Region "Constructor"

    Public Sub New(priceLock As PLPriceLock, dataAccess As PriceLockDataAccess)
        _priceLock = priceLock
        _dataAccess = dataAccess

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        Me.Text = $"Material Pricing - {_priceLock.SubdivisionName}"
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmRandomLengthsImport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadCategories()
        SetupGrid()
        LoadMaterialPricing()
    End Sub

    Private Sub btnLoadCurrent_Click(sender As Object, e As EventArgs) Handles btnLoadCurrent.Click
        LoadMaterialPricing()
    End Sub

    Private Sub btnApplyToAll_Click(sender As Object, e As EventArgs) Handles btnApplyToAll.Click
        ApplyDateToAll()
    End Sub

    Private Sub btnCalculate_Click(sender As Object, e As EventArgs) Handles btnCalculate.Click
        CalculateAllPrices()
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        SaveAllPricing()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

#End Region

#Region "Grid Setup"

    Private Sub SetupGrid()
        ' Add OriginalSellPrice column if not present
        If Not dgvPricing.Columns.Contains("OriginalSellPrice") Then
            Dim col As New DataGridViewTextBoxColumn() With {
                .Name = "OriginalSellPrice",
                .HeaderText = "Original Sell",
                .Width = 100
            }
            col.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            col.DefaultCellStyle.Format = "N2"

            ' Insert after CategoryName (index 2, after the hidden columns)
            Dim insertIndex = dgvPricing.Columns("RLDate").Index
            dgvPricing.Columns.Insert(insertIndex, col)
        End If

        ' Add BaselineRL and CurrentRL columns for display
        If Not dgvPricing.Columns.Contains("BaselineRLPrice") Then
            Dim colBaseline As New DataGridViewTextBoxColumn() With {
                .Name = "BaselineRLPrice",
                .HeaderText = "Baseline RL",
                .Width = 85,
                .ReadOnly = True
            }
            colBaseline.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            colBaseline.DefaultCellStyle.Format = "N2"
            colBaseline.DefaultCellStyle.ForeColor = Color.Gray

            Dim insertIndex = dgvPricing.Columns("RLPrice").Index
            dgvPricing.Columns.Insert(insertIndex, colBaseline)
        End If

        If Not dgvPricing.Columns.Contains("CurrentRLPrice") Then
            Dim colCurrent As New DataGridViewTextBoxColumn() With {
                .Name = "CurrentRLPrice",
                .HeaderText = "Current RL",
                .Width = 85,
                .ReadOnly = True
            }
            colCurrent.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            colCurrent.DefaultCellStyle.Format = "N2"
            colCurrent.DefaultCellStyle.ForeColor = Color.Gray

            Dim insertIndex = dgvPricing.Columns("RLPrice").Index + 1
            dgvPricing.Columns.Insert(insertIndex, colCurrent)
        End If

        If Not dgvPricing.Columns.Contains("RLPctChange") Then
            Dim colPct As New DataGridViewTextBoxColumn() With {
                .Name = "RLPctChange",
                .HeaderText = "RL % Chg",
                .Width = 75,
                .ReadOnly = True
            }
            colPct.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            colPct.DefaultCellStyle.Format = "P2"

            Dim insertIndex = dgvPricing.Columns("CalculatedPrice").Index
            dgvPricing.Columns.Insert(insertIndex, colPct)
        End If
    End Sub

#End Region

#Region "Grid Formatting"

    Private Sub dgvPricing_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvPricing.CellFormatting
        If e.RowIndex < 0 Then Return

        Dim columnName = dgvPricing.Columns(e.ColumnIndex).Name

        ' Color code percent change columns
        If columnName = "PctChange" OrElse columnName = "RLPctChange" Then
            Dim cell = dgvPricing.Rows(e.RowIndex).Cells(e.ColumnIndex)
            If cell.Value IsNot Nothing AndAlso cell.Value IsNot DBNull.Value Then
                Dim pct = CDec(cell.Value)
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

        ' Highlight OriginalSellPrice if empty (needs to be set for RL calculations)
        If columnName = "OriginalSellPrice" Then
            If e.Value Is Nothing OrElse e.Value Is DBNull.Value Then
                e.CellStyle.BackColor = Color.LightYellow
            End If
        End If
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadCategories()
        _categories = _dataAccess.GetMaterialCategories()
    End Sub

    Private Sub LoadMaterialPricing()
        Try
            Cursor = Cursors.WaitCursor
            dgvPricing.Rows.Clear()

            ' Get existing material pricing for this lock
            _materialPricing = _dataAccess.GetMaterialPricingByLock(_priceLock.PriceLockID)

            ' Get previous lock for comparison
            Dim previousLock = _dataAccess.GetPreviousPriceLock(_priceLock.SubdivisionID, _priceLock.PriceLockDate)

            ' Create a row for each category
            For Each cat In _categories.Where(Function(c) c.IsActive).OrderBy(Function(c) c.DisplayOrder)
                Dim existing = _materialPricing.FirstOrDefault(Function(m) m.MaterialCategoryID = cat.MaterialCategoryID)

                Dim previousPrice As Decimal? = Nothing
                If previousLock IsNot Nothing Then
                    previousPrice = _dataAccess.GetPreviousMaterialPrice(_priceLock.SubdivisionID, cat.MaterialCategoryID, _priceLock.PriceLockDate)
                End If

                Dim rowIndex = dgvPricing.Rows.Add()
                Dim row = dgvPricing.Rows(rowIndex)

                row.Cells("MaterialPricingID").Value = If(existing IsNot Nothing, existing.MaterialPricingID, 0)
                row.Cells("MaterialCategoryID").Value = cat.MaterialCategoryID
                row.Cells("CategoryName").Value = cat.CategoryName

                If existing IsNot Nothing Then
                    ' Original Sell Price (used for RL-based calculations)
                    row.Cells("OriginalSellPrice").Value = If(existing.OriginalSellPrice.HasValue, CObj(existing.OriginalSellPrice.Value), DBNull.Value)

                    ' RL-based calculation fields
                    row.Cells("BaselineRLPrice").Value = If(existing.BaselineRLPrice.HasValue, CObj(existing.BaselineRLPrice.Value), DBNull.Value)
                    row.Cells("CurrentRLPrice").Value = If(existing.CurrentRLPrice.HasValue, CObj(existing.CurrentRLPrice.Value), DBNull.Value)
                    row.Cells("RLPctChange").Value = If(existing.RLPercentChange.HasValue, CObj(existing.RLPercentChange.Value), DBNull.Value)

                    ' Legacy fields (still editable for manual entry)
                    row.Cells("RLDate").Value = If(existing.RandomLengthsDate.HasValue, existing.RandomLengthsDate.Value.ToShortDateString(), "")
                    row.Cells("RLPrice").Value = existing.RandomLengthsPrice
                    row.Cells("CalculatedPrice").Value = existing.CalculatedPrice
                    row.Cells("FinalPrice").Value = If(existing.PriceSentToSales, existing.CalculatedPrice)
                    row.Cells("Note").Value = existing.PriceNote
                Else
                    row.Cells("RLDate").Value = dtpReportDate.Value.ToShortDateString()
                End If

                row.Cells("PreviousPrice").Value = If(previousPrice.HasValue, CObj(previousPrice.Value), DBNull.Value)

                ' Calculate percent change from previous
                If previousPrice.HasValue AndAlso existing IsNot Nothing AndAlso existing.PriceSentToSales.HasValue Then
                    If previousPrice.Value <> 0 Then
                        Dim pctChange = (existing.PriceSentToSales.Value - previousPrice.Value) / previousPrice.Value
                        row.Cells("PctChange").Value = pctChange
                    End If
                End If
            Next

            UpdateStatusLabel()

        Catch ex As Exception
            MessageBox.Show($"Error loading pricing: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub UpdateStatusLabel()
        Dim total = dgvPricing.Rows.Count
        Dim withOriginal = 0
        Dim withFinal = 0

        For Each row As DataGridViewRow In dgvPricing.Rows
            If row.Cells("OriginalSellPrice").Value IsNot Nothing AndAlso row.Cells("OriginalSellPrice").Value IsNot DBNull.Value Then
                withOriginal += 1
            End If
            If row.Cells("FinalPrice").Value IsNot Nothing AndAlso row.Cells("FinalPrice").Value IsNot DBNull.Value Then
                withFinal += 1
            End If
        Next

        lblStatus.Text = $"{total} categories | {withOriginal} with Original Sell Price | {withFinal} with Final Price"
        lblStatus.ForeColor = Color.Green
    End Sub

#End Region

#Region "Calculations"

    Private Sub ApplyDateToAll()
        Dim dateStr = dtpReportDate.Value.ToShortDateString()
        For Each row As DataGridViewRow In dgvPricing.Rows
            row.Cells("RLDate").Value = dateStr
        Next
        lblStatus.Text = $"Applied date {dateStr} to all rows"
        lblStatus.ForeColor = Color.Blue
    End Sub

    Private Sub CalculateAllPrices()
        Dim margin = nudDefaultMargin.Value

        If margin >= 1 Then
            MessageBox.Show("Margin must be less than 1.", "Invalid Margin", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        For Each row As DataGridViewRow In dgvPricing.Rows
            Dim rlPriceCell = row.Cells("RLPrice")
            If rlPriceCell.Value IsNot Nothing AndAlso rlPriceCell.Value IsNot DBNull.Value Then
                Dim rlPrice As Decimal
                If Decimal.TryParse(rlPriceCell.Value.ToString(), rlPrice) AndAlso rlPrice > 0 Then
                    ' Calculate: RL Price / (1 - margin)
                    Dim calculated = rlPrice / (1 - margin)
                    row.Cells("CalculatedPrice").Value = calculated

                    ' Auto-fill Original Sell Price if empty (first time setup)
                    Dim originalCell = row.Cells("OriginalSellPrice")
                    If originalCell.Value Is Nothing OrElse originalCell.Value Is DBNull.Value OrElse CDec(originalCell.Value) = 0 Then
                        originalCell.Value = calculated
                    End If

                    ' Auto-fill final price if empty or zero
                    Dim finalCell = row.Cells("FinalPrice")
                    If finalCell.Value Is Nothing OrElse finalCell.Value Is DBNull.Value OrElse CDec(finalCell.Value) = 0 Then
                        finalCell.Value = calculated
                    End If

                    ' Update percent change from previous
                    Dim previousCell = row.Cells("PreviousPrice")
                    If previousCell.Value IsNot Nothing AndAlso previousCell.Value IsNot DBNull.Value Then
                        Dim prev = CDec(previousCell.Value)
                        If prev <> 0 Then
                            row.Cells("PctChange").Value = (calculated - prev) / prev
                        End If
                    End If
                End If
            End If
        Next

        dgvPricing.Refresh()
        UpdateStatusLabel()
        lblStatus.Text &= $" | Calculated with {margin:P0} margin"
    End Sub

#End Region

#Region "Save"

    Private Sub SaveAllPricing()
        Try
            Cursor = Cursors.WaitCursor
            Dim saved = 0
            Dim created = 0

            For Each row As DataGridViewRow In dgvPricing.Rows
                Dim materialPricingID = CInt(row.Cells("MaterialPricingID").Value)
                Dim categoryID = CInt(row.Cells("MaterialCategoryID").Value)

                ' Parse Original Sell Price
                Dim originalSellPrice As Decimal? = Nothing
                If row.Cells("OriginalSellPrice").Value IsNot Nothing AndAlso row.Cells("OriginalSellPrice").Value IsNot DBNull.Value Then
                    originalSellPrice = CDec(row.Cells("OriginalSellPrice").Value)
                End If

                ' Parse RL Date
                Dim rlDate As DateTime? = Nothing
                Dim rlDateStr = row.Cells("RLDate").Value?.ToString()
                If Not String.IsNullOrEmpty(rlDateStr) Then
                    Dim tempDate As DateTime
                    If DateTime.TryParse(rlDateStr, tempDate) Then
                        rlDate = tempDate
                    End If
                End If

                ' Parse RL Price
                Dim rlPrice As Decimal? = Nothing
                If row.Cells("RLPrice").Value IsNot Nothing AndAlso row.Cells("RLPrice").Value IsNot DBNull.Value Then
                    rlPrice = CDec(row.Cells("RLPrice").Value)
                End If

                ' Parse Calculated Price
                Dim calculatedPrice As Decimal? = Nothing
                If row.Cells("CalculatedPrice").Value IsNot Nothing AndAlso row.Cells("CalculatedPrice").Value IsNot DBNull.Value Then
                    calculatedPrice = CDec(row.Cells("CalculatedPrice").Value)
                End If

                ' Parse Final Price
                Dim finalPrice As Decimal? = Nothing
                If row.Cells("FinalPrice").Value IsNot Nothing AndAlso row.Cells("FinalPrice").Value IsNot DBNull.Value Then
                    finalPrice = CDec(row.Cells("FinalPrice").Value)
                End If

                Dim note = row.Cells("Note").Value?.ToString()

                ' Skip if no meaningful data entered
                If Not originalSellPrice.HasValue AndAlso Not rlPrice.HasValue AndAlso Not finalPrice.HasValue Then
                    Continue For
                End If

                If materialPricingID > 0 Then
                    ' Update existing
                    Dim pricing = _materialPricing.FirstOrDefault(Function(m) m.MaterialPricingID = materialPricingID)
                    If pricing IsNot Nothing Then
                        pricing.OriginalSellPrice = originalSellPrice
                        pricing.RandomLengthsDate = rlDate
                        pricing.RandomLengthsPrice = rlPrice
                        pricing.CalculatedPrice = calculatedPrice
                        pricing.PriceSentToSales = finalPrice
                        pricing.PriceSentToBuilder = finalPrice ' Default to same
                        pricing.PriceNote = note
                        pricing.ModifiedBy = Environment.UserName
                        _dataAccess.UpdateMaterialPricing(pricing)
                        saved += 1
                    End If
                Else
                    ' Create new
                    Dim newPricing As New PLMaterialPricing() With {
                        .PriceLockID = _priceLock.PriceLockID,
                        .MaterialCategoryID = categoryID,
                        .OriginalSellPrice = originalSellPrice,
                        .RandomLengthsDate = rlDate,
                        .RandomLengthsPrice = rlPrice,
                        .CalculatedPrice = calculatedPrice,
                        .PriceSentToSales = finalPrice,
                        .PriceSentToBuilder = finalPrice,
                        .PriceNote = note,
                        .ModifiedBy = Environment.UserName
                    }
                    _dataAccess.InsertMaterialPricing(newPricing)
                    created += 1
                End If
            Next

            MessageBox.Show($"Saved successfully!" & vbCrLf & vbCrLf &
                $"Updated: {saved}" & vbCrLf &
                $"Created: {created}",
                "Save Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show($"Error saving: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

#End Region

End Class