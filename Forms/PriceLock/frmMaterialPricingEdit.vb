' =====================================================
' frmMaterialPricingEdit.vb
' Dialog for editing a material pricing record
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Partial Public Class frmMaterialPricingEdit
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _priceLock As PLPriceLock
    Private _materialPricing As PLMaterialPricing

#End Region

#Region "Constructor"

    Public Sub New(materialPricing As PLMaterialPricing, priceLock As PLPriceLock, dataAccess As PriceLockDataAccess)
        _materialPricing = materialPricing
        _priceLock = priceLock
        _dataAccess = dataAccess
        InitializeComponent()
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmMaterialPricingEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadMaterialData()
        LoadPreviousPrice()
    End Sub

    Private Sub nudPriceSentToBuilder_ValueChanged(sender As Object, e As EventArgs) Handles nudPriceSentToBuilder.ValueChanged
        lblPriceDiffWarning.Visible = (nudPriceSentToBuilder.Value <> nudCalculatedPrice.Value)
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If ValidateInput() Then SaveMaterialPricing()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadMaterialData()
        lblCategoryName.Text = _materialPricing.CategoryName

        ' RL fields become READ-ONLY reference data
        dtpRandomLengthsDate.Enabled = False
        dtpRandomLengthsDate.Value = If(_materialPricing.RandomLengthsDate.HasValue, _materialPricing.RandomLengthsDate.Value, DateTime.Today)

        nudRandomLengthsPrice.Enabled = False
        nudRandomLengthsPrice.Value = If(_materialPricing.RandomLengthsPrice, 0D)

        ' Show RL-based calculation fields (NEW)
        lblBaselineRLValue.Text = If(_materialPricing.BaselineRLPrice.HasValue, _materialPricing.BaselineRLPrice.Value.ToString("C2"), "--")
        lblCurrentRLValue.Text = If(_materialPricing.CurrentRLPrice.HasValue, _materialPricing.CurrentRLPrice.Value.ToString("C2"), "--")
        lblRLPctChangeValue.Text = If(_materialPricing.RLPercentChange.HasValue, _materialPricing.RLPercentChange.Value.ToString("P2"), "--")

        ' Editable fields
        nudCalculatedPrice.Value = If(_materialPricing.CalculatedPrice, 0D)
        nudPriceSentToBuilder.Value = If(_materialPricing.PriceSentToBuilder, 0D)
        txtPriceNote.Text = _materialPricing.PriceNote

        lblPriceDiffWarning.Visible = _materialPricing.HasPriceDifference

        ' Display percent change
        If _materialPricing.PctChangeFromPrevious.HasValue Then
            Dim pct = _materialPricing.PctChangeFromPrevious.Value
            lblPctChange.Text = pct.ToString("P1")
            If pct > 0.05D Then
                lblPctChange.ForeColor = Color.Red
            ElseIf pct < -0.05D Then
                lblPctChange.ForeColor = Color.Green
            Else
                lblPctChange.ForeColor = Color.Black
            End If
        Else
            lblPctChange.Text = "--"
        End If
    End Sub

    Private Sub LoadPreviousPrice()
        ' Get previous price for this material category
        Dim previousPrice = _dataAccess.GetPreviousMaterialPrice(
            _priceLock.SubdivisionID,
            _materialPricing.MaterialCategoryID,
            _priceLock.PriceLockDate)

        If previousPrice.HasValue Then
            lblPreviousPrice.Text = previousPrice.Value.ToString("N4")
        Else
            lblPreviousPrice.Text = "(No previous lock)"
        End If
    End Sub

#End Region

#Region "Validation and Save"

    Private Function ValidateInput() As Boolean
        ' Require note if prices differ
        If nudPriceSentToBuilder.Value <> nudCalculatedPrice.Value AndAlso String.IsNullOrWhiteSpace(txtPriceNote.Text) Then
            MessageBox.Show("Please enter a note explaining why the Builder price differs from the Calculated price.",
                "Note Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPriceNote.Focus()
            Return False
        End If
        Return True
    End Function

    Private Sub SaveMaterialPricing()
        Try
            Cursor = Cursors.WaitCursor

            _materialPricing.RandomLengthsDate = dtpRandomLengthsDate.Value.Date
            _materialPricing.RandomLengthsPrice = nudRandomLengthsPrice.Value
            _materialPricing.CalculatedPrice = nudCalculatedPrice.Value
            _materialPricing.PriceSentToSales = nudCalculatedPrice.Value  ' Always same as Calculated
            _materialPricing.PriceSentToBuilder = nudPriceSentToBuilder.Value
            _materialPricing.PriceNote = txtPriceNote.Text.Trim()
            _materialPricing.ModifiedBy = Environment.UserName

            ' Calculate percent change from previous
            Dim previousPrice = _dataAccess.GetPreviousMaterialPrice(
                _priceLock.SubdivisionID,
                _materialPricing.MaterialCategoryID,
                _priceLock.PriceLockDate)

            If previousPrice.HasValue AndAlso previousPrice.Value <> 0 Then
                _materialPricing.PctChangeFromPrevious = (nudCalculatedPrice.Value - previousPrice.Value) / previousPrice.Value
            Else
                _materialPricing.PctChangeFromPrevious = Nothing
            End If

            _dataAccess.UpdateMaterialPricing(_materialPricing)

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