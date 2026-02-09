' =====================================================
' frmMaterialPricingEdit.Designer.vb
' Designer file for Material Pricing Edit Dialog
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Partial Public Class frmMaterialPricingEdit

#Region "Windows Form Designer generated code"

    ''' <summary>
    ''' Required designer variable.
    ''' </summary>
    Private components As System.ComponentModel.IContainer = Nothing

    ''' <summary>
    ''' Clean up any resources being used.
    ''' </summary>
    Protected Overrides Sub Dispose(disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    ' Labels
    Friend WithEvents lblCategoryLabel As Label
    Friend WithEvents lblCategoryName As Label
    Friend WithEvents lblRLSection As Label
    Friend WithEvents lblRLDate As Label
    Friend WithEvents lblRLPrice As Label

    ' *** NEW: RL Calculation Display Labels ***
    Friend WithEvents lblBaselineRLLabel As Label
    Friend WithEvents lblBaselineRLValue As Label
    Friend WithEvents lblCurrentRLLabel As Label
    Friend WithEvents lblCurrentRLValue As Label
    Friend WithEvents lblRLPctChangeLabel As Label
    Friend WithEvents lblRLPctChangeValue As Label
    ' *** END NEW ***

    Friend WithEvents lblPriceSection As Label
    Friend WithEvents lblCalculatedPrice As Label
    Friend WithEvents lblCalculatedNote As Label
    Friend WithEvents lblBuilderPrice As Label
    Friend WithEvents lblPriceDiffWarning As Label
    Friend WithEvents lblComparisonSection As Label
    Friend WithEvents lblPreviousPriceLabel As Label
    Friend WithEvents lblPreviousPrice As Label
    Friend WithEvents lblPctChangeLabel As Label
    Friend WithEvents lblPctChange As Label
    Friend WithEvents lblNote As Label

    ' Input Controls
    Friend WithEvents dtpRandomLengthsDate As DateTimePicker
    Friend WithEvents nudRandomLengthsPrice As NumericUpDown
    Friend WithEvents nudCalculatedPrice As NumericUpDown
    Friend WithEvents nudPriceSentToBuilder As NumericUpDown
    Friend WithEvents txtPriceNote As TextBox

    ' Buttons
    Friend WithEvents btnOK As Button
    Friend WithEvents btnCancel As Button

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()

        Me.lblCategoryLabel = New Label()
        Me.lblCategoryName = New Label()
        Me.lblRLSection = New Label()
        Me.lblRLDate = New Label()
        Me.dtpRandomLengthsDate = New DateTimePicker()
        Me.lblRLPrice = New Label()
        Me.nudRandomLengthsPrice = New NumericUpDown()

        ' *** NEW: Initialize RL Calculation Labels ***
        Me.lblBaselineRLLabel = New Label()
        Me.lblBaselineRLValue = New Label()
        Me.lblCurrentRLLabel = New Label()
        Me.lblCurrentRLValue = New Label()
        Me.lblRLPctChangeLabel = New Label()
        Me.lblRLPctChangeValue = New Label()
        ' *** END NEW ***

        Me.lblPriceSection = New Label()
        Me.lblCalculatedPrice = New Label()
        Me.nudCalculatedPrice = New NumericUpDown()
        Me.lblCalculatedNote = New Label()
        Me.lblBuilderPrice = New Label()
        Me.nudPriceSentToBuilder = New NumericUpDown()
        Me.lblPriceDiffWarning = New Label()
        Me.lblComparisonSection = New Label()
        Me.lblPreviousPriceLabel = New Label()
        Me.lblPreviousPrice = New Label()
        Me.lblPctChangeLabel = New Label()
        Me.lblPctChange = New Label()
        Me.lblNote = New Label()
        Me.txtPriceNote = New TextBox()
        Me.btnOK = New Button()
        Me.btnCancel = New Button()

        CType(Me.nudRandomLengthsPrice, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudCalculatedPrice, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudPriceSentToBuilder, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()

        '
        ' lblCategoryLabel
        '
        Me.lblCategoryLabel.AutoSize = True
        Me.lblCategoryLabel.Font = New Font(Me.Font, FontStyle.Bold)
        Me.lblCategoryLabel.Location = New Point(20, 23)
        Me.lblCategoryLabel.Name = "lblCategoryLabel"
        Me.lblCategoryLabel.Text = "Category:"

        '
        ' lblCategoryName
        '
        Me.lblCategoryName.AutoSize = True
        Me.lblCategoryName.Font = New Font(Me.Font, FontStyle.Bold)
        Me.lblCategoryName.ForeColor = Color.DarkBlue
        Me.lblCategoryName.Location = New Point(100, 23)
        Me.lblCategoryName.Name = "lblCategoryName"
        Me.lblCategoryName.Text = ""

        '
        ' lblRLSection
        '
        Me.lblRLSection.AutoSize = True
        Me.lblRLSection.ForeColor = Color.Gray
        Me.lblRLSection.Location = New Point(20, 55)
        Me.lblRLSection.Name = "lblRLSection"
        Me.lblRLSection.Text = "─── Random Lengths Data (Read-Only) ───"

        '
        ' lblRLDate
        '
        Me.lblRLDate.AutoSize = True
        Me.lblRLDate.Location = New Point(20, 83)
        Me.lblRLDate.Name = "lblRLDate"
        Me.lblRLDate.Text = "RL Report Date:"

        '
        ' dtpRandomLengthsDate
        '
        Me.dtpRandomLengthsDate.Format = DateTimePickerFormat.Short
        Me.dtpRandomLengthsDate.Location = New Point(150, 80)
        Me.dtpRandomLengthsDate.Name = "dtpRandomLengthsDate"
        Me.dtpRandomLengthsDate.Width = 120

        '
        ' lblRLPrice
        '
        Me.lblRLPrice.AutoSize = True
        Me.lblRLPrice.Location = New Point(20, 115)
        Me.lblRLPrice.Name = "lblRLPrice"
        Me.lblRLPrice.Text = "RL Price (MBF):"

        '
        ' nudRandomLengthsPrice
        '
        Me.nudRandomLengthsPrice.DecimalPlaces = 2
        Me.nudRandomLengthsPrice.Location = New Point(150, 112)
        Me.nudRandomLengthsPrice.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.nudRandomLengthsPrice.Minimum = New Decimal(New Integer() {0, 0, 0, 0})
        Me.nudRandomLengthsPrice.Name = "nudRandomLengthsPrice"
        Me.nudRandomLengthsPrice.Width = 100

        ' *** NEW: RL Calculation Display Controls ***

        '
        ' lblBaselineRLLabel
        '
        Me.lblBaselineRLLabel.AutoSize = True
        Me.lblBaselineRLLabel.ForeColor = Color.Gray
        Me.lblBaselineRLLabel.Location = New Point(20, 147)
        Me.lblBaselineRLLabel.Name = "lblBaselineRLLabel"
        Me.lblBaselineRLLabel.Text = "Baseline RL:"

        '
        ' lblBaselineRLValue
        '
        Me.lblBaselineRLValue.AutoSize = True
        Me.lblBaselineRLValue.ForeColor = Color.Gray
        Me.lblBaselineRLValue.Font = New Font(Me.Font, FontStyle.Bold)
        Me.lblBaselineRLValue.Location = New Point(150, 147)
        Me.lblBaselineRLValue.Name = "lblBaselineRLValue"
        Me.lblBaselineRLValue.Text = "--"

        '
        ' lblCurrentRLLabel
        '
        Me.lblCurrentRLLabel.AutoSize = True
        Me.lblCurrentRLLabel.ForeColor = Color.Gray
        Me.lblCurrentRLLabel.Location = New Point(20, 172)
        Me.lblCurrentRLLabel.Name = "lblCurrentRLLabel"
        Me.lblCurrentRLLabel.Text = "Current RL:"

        '
        ' lblCurrentRLValue
        '
        Me.lblCurrentRLValue.AutoSize = True
        Me.lblCurrentRLValue.ForeColor = Color.Gray
        Me.lblCurrentRLValue.Font = New Font(Me.Font, FontStyle.Bold)
        Me.lblCurrentRLValue.Location = New Point(150, 172)
        Me.lblCurrentRLValue.Name = "lblCurrentRLValue"
        Me.lblCurrentRLValue.Text = "--"

        '
        ' lblRLPctChangeLabel
        '
        Me.lblRLPctChangeLabel.AutoSize = True
        Me.lblRLPctChangeLabel.ForeColor = Color.Gray
        Me.lblRLPctChangeLabel.Location = New Point(20, 197)
        Me.lblRLPctChangeLabel.Name = "lblRLPctChangeLabel"
        Me.lblRLPctChangeLabel.Text = "RL % Change:"

        '
        ' lblRLPctChangeValue
        '
        Me.lblRLPctChangeValue.AutoSize = True
        Me.lblRLPctChangeValue.ForeColor = Color.DarkBlue
        Me.lblRLPctChangeValue.Font = New Font(Me.Font, FontStyle.Bold)
        Me.lblRLPctChangeValue.Location = New Point(150, 197)
        Me.lblRLPctChangeValue.Name = "lblRLPctChangeValue"
        Me.lblRLPctChangeValue.Text = "--"

        ' *** END NEW ***

        '
        ' lblPriceSection
        '
        Me.lblPriceSection.AutoSize = True
        Me.lblPriceSection.ForeColor = Color.Gray
        Me.lblPriceSection.Location = New Point(20, 232)
        Me.lblPriceSection.Name = "lblPriceSection"
        Me.lblPriceSection.Text = "─── Price Tracking ───"

        '
        ' lblCalculatedPrice
        '
        Me.lblCalculatedPrice.AutoSize = True
        Me.lblCalculatedPrice.Location = New Point(20, 260)
        Me.lblCalculatedPrice.Name = "lblCalculatedPrice"
        Me.lblCalculatedPrice.Text = "Calculated Price:"

        '
        ' nudCalculatedPrice
        '
        Me.nudCalculatedPrice.DecimalPlaces = 4
        Me.nudCalculatedPrice.Location = New Point(150, 257)
        Me.nudCalculatedPrice.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.nudCalculatedPrice.Minimum = New Decimal(New Integer() {-99999, 0, 0, -2147483648})
        Me.nudCalculatedPrice.Name = "nudCalculatedPrice"
        Me.nudCalculatedPrice.Width = 100

        '
        ' lblCalculatedNote
        '
        Me.lblCalculatedNote.AutoSize = True
        Me.lblCalculatedNote.ForeColor = Color.Gray
        Me.lblCalculatedNote.Location = New Point(260, 260)
        Me.lblCalculatedNote.Name = "lblCalculatedNote"
        Me.lblCalculatedNote.Text = "(= Sent to Sales)"

        '
        ' lblBuilderPrice
        '
        Me.lblBuilderPrice.AutoSize = True
        Me.lblBuilderPrice.Location = New Point(20, 292)
        Me.lblBuilderPrice.Name = "lblBuilderPrice"
        Me.lblBuilderPrice.Text = "Price to Builder:"

        '
        ' nudPriceSentToBuilder
        '
        Me.nudPriceSentToBuilder.DecimalPlaces = 4
        Me.nudPriceSentToBuilder.Location = New Point(150, 289)
        Me.nudPriceSentToBuilder.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.nudPriceSentToBuilder.Minimum = New Decimal(New Integer() {-99999, 0, 0, -2147483648})
        Me.nudPriceSentToBuilder.Name = "nudPriceSentToBuilder"
        Me.nudPriceSentToBuilder.Width = 100

        '
        ' lblPriceDiffWarning
        '
        Me.lblPriceDiffWarning.AutoSize = True
        Me.lblPriceDiffWarning.ForeColor = Color.DarkOrange
        Me.lblPriceDiffWarning.Location = New Point(150, 317)
        Me.lblPriceDiffWarning.Name = "lblPriceDiffWarning"
        Me.lblPriceDiffWarning.Text = "⚠ Builder price differs from Calculated price"
        Me.lblPriceDiffWarning.Visible = False

        '
        ' lblComparisonSection
        '
        Me.lblComparisonSection.AutoSize = True
        Me.lblComparisonSection.ForeColor = Color.Gray
        Me.lblComparisonSection.Location = New Point(20, 347)
        Me.lblComparisonSection.Name = "lblComparisonSection"
        Me.lblComparisonSection.Text = "─── Comparison to Previous ───"

        '
        ' lblPreviousPriceLabel
        '
        Me.lblPreviousPriceLabel.AutoSize = True
        Me.lblPreviousPriceLabel.Location = New Point(20, 375)
        Me.lblPreviousPriceLabel.Name = "lblPreviousPriceLabel"
        Me.lblPreviousPriceLabel.Text = "Previous Price:"

        '
        ' lblPreviousPrice
        '
        Me.lblPreviousPrice.AutoSize = True
        Me.lblPreviousPrice.Location = New Point(150, 375)
        Me.lblPreviousPrice.Name = "lblPreviousPrice"
        Me.lblPreviousPrice.Text = ""

        '
        ' lblPctChangeLabel
        '
        Me.lblPctChangeLabel.AutoSize = True
        Me.lblPctChangeLabel.Location = New Point(20, 400)
        Me.lblPctChangeLabel.Name = "lblPctChangeLabel"
        Me.lblPctChangeLabel.Text = "% Change:"

        '
        ' lblPctChange
        '
        Me.lblPctChange.AutoSize = True
        Me.lblPctChange.Font = New Font(Me.Font, FontStyle.Bold)
        Me.lblPctChange.Location = New Point(150, 400)
        Me.lblPctChange.Name = "lblPctChange"
        Me.lblPctChange.Text = ""

        '
        ' lblNote
        '
        Me.lblNote.AutoSize = True
        Me.lblNote.Location = New Point(20, 435)
        Me.lblNote.Name = "lblNote"
        Me.lblNote.Text = "Note:"

        '
        ' txtPriceNote
        '
        Me.txtPriceNote.Height = 60
        Me.txtPriceNote.Location = New Point(150, 435)
        Me.txtPriceNote.Multiline = True
        Me.txtPriceNote.Name = "txtPriceNote"
        Me.txtPriceNote.ScrollBars = ScrollBars.Vertical
        Me.txtPriceNote.Width = 250

        '
        ' btnOK
        '
        Me.btnOK.Height = 30
        Me.btnOK.Location = New Point(230, 510)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Text = "Save"
        Me.btnOK.Width = 80

        '
        ' btnCancel
        '
        Me.btnCancel.DialogResult = DialogResult.Cancel
        Me.btnCancel.Height = 30
        Me.btnCancel.Location = New Point(320, 510)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.Width = 80

        '
        ' frmMaterialPricingEdit
        '
        Me.AcceptButton = Me.btnOK
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New Size(450, 560)
        Me.Controls.Add(Me.lblCategoryLabel)
        Me.Controls.Add(Me.lblCategoryName)
        Me.Controls.Add(Me.lblRLSection)
        Me.Controls.Add(Me.lblRLDate)
        Me.Controls.Add(Me.dtpRandomLengthsDate)
        Me.Controls.Add(Me.lblRLPrice)
        Me.Controls.Add(Me.nudRandomLengthsPrice)

        ' *** NEW: Add RL Calculation Labels to Controls ***
        Me.Controls.Add(Me.lblBaselineRLLabel)
        Me.Controls.Add(Me.lblBaselineRLValue)
        Me.Controls.Add(Me.lblCurrentRLLabel)
        Me.Controls.Add(Me.lblCurrentRLValue)
        Me.Controls.Add(Me.lblRLPctChangeLabel)
        Me.Controls.Add(Me.lblRLPctChangeValue)
        ' *** END NEW ***

        Me.Controls.Add(Me.lblPriceSection)
        Me.Controls.Add(Me.lblCalculatedPrice)
        Me.Controls.Add(Me.nudCalculatedPrice)
        Me.Controls.Add(Me.lblCalculatedNote)
        Me.Controls.Add(Me.lblBuilderPrice)
        Me.Controls.Add(Me.nudPriceSentToBuilder)
        Me.Controls.Add(Me.lblPriceDiffWarning)
        Me.Controls.Add(Me.lblComparisonSection)
        Me.Controls.Add(Me.lblPreviousPriceLabel)
        Me.Controls.Add(Me.lblPreviousPrice)
        Me.Controls.Add(Me.lblPctChangeLabel)
        Me.Controls.Add(Me.lblPctChange)
        Me.Controls.Add(Me.lblNote)
        Me.Controls.Add(Me.txtPriceNote)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMaterialPricingEdit"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Edit Material Pricing"

        CType(Me.nudRandomLengthsPrice, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudCalculatedPrice, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudPriceSentToBuilder, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

End Class