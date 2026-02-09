' =====================================================
' frmPriceLockNew.Designer.vb
' Designer file for New Price Lock Dialog
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Partial Public Class frmPriceLockNew

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
    Friend WithEvents lblBuilder As Label
    Friend WithEvents lblSubdivision As Label
    Friend WithEvents lblLockDate As Label
    Friend WithEvents lblLockName As Label

    ' Input Controls
    Friend WithEvents cboBuilder As ComboBox
    Friend WithEvents cboSubdivision As ComboBox
    Friend WithEvents dtpLockDate As DateTimePicker
    Friend WithEvents txtLockName As TextBox

    ' Margin GroupBox and Controls
    Friend WithEvents grpMargins As GroupBox
    Friend WithEvents lblBaseMgmt As Label
    Friend WithEvents nudBaseMgmtMargin As NumericUpDown
    Friend WithEvents lblBaseMgmtPct As Label
    Friend WithEvents lblAdjusted As Label
    Friend WithEvents nudAdjustedMargin As NumericUpDown
    Friend WithEvents lblOption As Label
    Friend WithEvents nudOptionMargin As NumericUpDown

    ' Checkbox
    Friend WithEvents chkCopyFromPrevious As CheckBox

    ' Buttons
    Friend WithEvents btnOK As Button
    Friend WithEvents btnCancel As Button

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()

        Me.lblBuilder = New Label()
        Me.cboBuilder = New ComboBox()
        Me.lblSubdivision = New Label()
        Me.cboSubdivision = New ComboBox()
        Me.lblLockDate = New Label()
        Me.dtpLockDate = New DateTimePicker()
        Me.lblLockName = New Label()
        Me.txtLockName = New TextBox()

        Me.grpMargins = New GroupBox()
        Me.lblBaseMgmt = New Label()
        Me.nudBaseMgmtMargin = New NumericUpDown()
        Me.lblBaseMgmtPct = New Label()
        Me.lblAdjusted = New Label()
        Me.nudAdjustedMargin = New NumericUpDown()
        Me.lblOption = New Label()
        Me.nudOptionMargin = New NumericUpDown()

        Me.chkCopyFromPrevious = New CheckBox()
        Me.btnOK = New Button()
        Me.btnCancel = New Button()

        Me.grpMargins.SuspendLayout()
        CType(Me.nudBaseMgmtMargin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudAdjustedMargin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudOptionMargin, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()

        '
        ' lblBuilder
        '
        Me.lblBuilder.AutoSize = True
        Me.lblBuilder.Location = New Point(20, 23)
        Me.lblBuilder.Name = "lblBuilder"
        Me.lblBuilder.Text = "Builder:"

        '
        ' cboBuilder
        '
        Me.cboBuilder.DropDownStyle = ComboBoxStyle.DropDownList
        Me.cboBuilder.Location = New Point(150, 20)
        Me.cboBuilder.Name = "cboBuilder"
        Me.cboBuilder.Width = 250

        '
        ' lblSubdivision
        '
        Me.lblSubdivision.AutoSize = True
        Me.lblSubdivision.Location = New Point(20, 58)
        Me.lblSubdivision.Name = "lblSubdivision"
        Me.lblSubdivision.Text = "Subdivision:"

        '
        ' cboSubdivision
        '
        Me.cboSubdivision.DropDownStyle = ComboBoxStyle.DropDownList
        Me.cboSubdivision.Location = New Point(150, 55)
        Me.cboSubdivision.Name = "cboSubdivision"
        Me.cboSubdivision.Width = 250

        '
        ' lblLockDate
        '
        Me.lblLockDate.AutoSize = True
        Me.lblLockDate.Location = New Point(20, 93)
        Me.lblLockDate.Name = "lblLockDate"
        Me.lblLockDate.Text = "Price Lock Date:"

        '
        ' dtpLockDate
        '
        Me.dtpLockDate.Format = DateTimePickerFormat.Short
        Me.dtpLockDate.Location = New Point(150, 90)
        Me.dtpLockDate.Name = "dtpLockDate"
        Me.dtpLockDate.Width = 150

        '
        ' lblLockName
        '
        Me.lblLockName.AutoSize = True
        Me.lblLockName.Location = New Point(20, 128)
        Me.lblLockName.Name = "lblLockName"
        Me.lblLockName.Text = "Lock Name:"

        '
        ' txtLockName
        '
        Me.txtLockName.Location = New Point(150, 125)
        Me.txtLockName.Name = "txtLockName"
        Me.txtLockName.Width = 250

        '
        ' grpMargins
        '
        Me.grpMargins.Controls.Add(Me.lblBaseMgmt)
        Me.grpMargins.Controls.Add(Me.nudBaseMgmtMargin)
        Me.grpMargins.Controls.Add(Me.lblBaseMgmtPct)
        Me.grpMargins.Controls.Add(Me.lblAdjusted)
        Me.grpMargins.Controls.Add(Me.nudAdjustedMargin)
        Me.grpMargins.Controls.Add(Me.lblOption)
        Me.grpMargins.Controls.Add(Me.nudOptionMargin)
        Me.grpMargins.Location = New Point(20, 165)
        Me.grpMargins.Name = "grpMargins"
        Me.grpMargins.Size = New Size(390, 110)
        Me.grpMargins.Text = "Default Margins"

        '
        ' lblBaseMgmt
        '
        Me.lblBaseMgmt.AutoSize = True
        Me.lblBaseMgmt.Location = New Point(15, 25)
        Me.lblBaseMgmt.Name = "lblBaseMgmt"
        Me.lblBaseMgmt.Text = "Base MGMT Margin:"

        '
        ' nudBaseMgmtMargin
        '
        Me.nudBaseMgmtMargin.DecimalPlaces = 2
        Me.nudBaseMgmtMargin.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.nudBaseMgmtMargin.Location = New Point(160, 23)
        Me.nudBaseMgmtMargin.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudBaseMgmtMargin.Minimum = New Decimal(New Integer() {0, 0, 0, 0})
        Me.nudBaseMgmtMargin.Name = "nudBaseMgmtMargin"
        Me.nudBaseMgmtMargin.Value = New Decimal(New Integer() {15, 0, 0, 131072})
        Me.nudBaseMgmtMargin.Width = 80

        '
        ' lblBaseMgmtPct
        '
        Me.lblBaseMgmtPct.AutoSize = True
        Me.lblBaseMgmtPct.ForeColor = Color.Gray
        Me.lblBaseMgmtPct.Location = New Point(250, 25)
        Me.lblBaseMgmtPct.Name = "lblBaseMgmtPct"
        Me.lblBaseMgmtPct.Text = "(e.g., 0.15 = 15%)"

        '
        ' lblAdjusted
        '
        Me.lblAdjusted.AutoSize = True
        Me.lblAdjusted.Location = New Point(15, 55)
        Me.lblAdjusted.Name = "lblAdjusted"
        Me.lblAdjusted.Text = "Adjusted Base Margin:"

        '
        ' nudAdjustedMargin
        '
        Me.nudAdjustedMargin.DecimalPlaces = 2
        Me.nudAdjustedMargin.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.nudAdjustedMargin.Location = New Point(160, 53)
        Me.nudAdjustedMargin.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudAdjustedMargin.Minimum = New Decimal(New Integer() {0, 0, 0, 0})
        Me.nudAdjustedMargin.Name = "nudAdjustedMargin"
        Me.nudAdjustedMargin.Value = New Decimal(New Integer() {15, 0, 0, 131072})
        Me.nudAdjustedMargin.Width = 80

        '
        ' lblOption
        '
        Me.lblOption.AutoSize = True
        Me.lblOption.Location = New Point(15, 85)
        Me.lblOption.Name = "lblOption"
        Me.lblOption.Text = "Option Margin:"

        '
        ' nudOptionMargin
        '
        Me.nudOptionMargin.DecimalPlaces = 2
        Me.nudOptionMargin.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.nudOptionMargin.Location = New Point(160, 83)
        Me.nudOptionMargin.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudOptionMargin.Minimum = New Decimal(New Integer() {0, 0, 0, 0})
        Me.nudOptionMargin.Name = "nudOptionMargin"
        Me.nudOptionMargin.Value = New Decimal(New Integer() {3, 0, 0, 65536})
        Me.nudOptionMargin.Width = 80

        '
        ' chkCopyFromPrevious
        '
        Me.chkCopyFromPrevious.AutoSize = True
        Me.chkCopyFromPrevious.Checked = True
        Me.chkCopyFromPrevious.CheckState = CheckState.Checked
        Me.chkCopyFromPrevious.Location = New Point(20, 285)
        Me.chkCopyFromPrevious.Name = "chkCopyFromPrevious"
        Me.chkCopyFromPrevious.Text = "Copy pricing data from previous price lock (if exists)"

        '
        ' btnOK
        '
        Me.btnOK.DialogResult = DialogResult.None
        Me.btnOK.Height = 30
        Me.btnOK.Location = New Point(230, 320)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Text = "Create"
        Me.btnOK.Width = 90

        '
        ' btnCancel
        '
        Me.btnCancel.DialogResult = DialogResult.Cancel
        Me.btnCancel.Height = 30
        Me.btnCancel.Location = New Point(330, 320)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.Width = 80

        '
        ' frmPriceLockNew
        '
        Me.AcceptButton = Me.btnOK
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New Size(450, 380)
        Me.Controls.Add(Me.lblBuilder)
        Me.Controls.Add(Me.cboBuilder)
        Me.Controls.Add(Me.lblSubdivision)
        Me.Controls.Add(Me.cboSubdivision)
        Me.Controls.Add(Me.lblLockDate)
        Me.Controls.Add(Me.dtpLockDate)
        Me.Controls.Add(Me.lblLockName)
        Me.Controls.Add(Me.txtLockName)
        Me.Controls.Add(Me.grpMargins)
        Me.Controls.Add(Me.chkCopyFromPrevious)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmPriceLockNew"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Create New Price Lock"

        Me.grpMargins.ResumeLayout(False)
        Me.grpMargins.PerformLayout()
        CType(Me.nudBaseMgmtMargin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudAdjustedMargin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudOptionMargin, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

End Class