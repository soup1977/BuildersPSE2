' =====================================================
' frmPriceLockCopy.Designer.vb
' Designer file for Price Lock Copy Dialog
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Partial Public Class frmPriceLockCopy

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
    Friend WithEvents lblSource As Label
    Friend WithEvents lblSourceInfo As Label
    Friend WithEvents lblTarget As Label
    Friend WithEvents lblLockDate As Label
    Friend WithEvents lblLockName As Label

    ' Input Controls
    Friend WithEvents chkSameSubdivision As CheckBox
    Friend WithEvents cboTargetSubdivision As ComboBox
    Friend WithEvents dtpLockDate As DateTimePicker
    Friend WithEvents txtLockName As TextBox

    ' Buttons
    Friend WithEvents btnOK As Button
    Friend WithEvents btnCancel As Button

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()

        Me.lblSource = New Label()
        Me.lblSourceInfo = New Label()
        Me.chkSameSubdivision = New CheckBox()
        Me.lblTarget = New Label()
        Me.cboTargetSubdivision = New ComboBox()
        Me.lblLockDate = New Label()
        Me.dtpLockDate = New DateTimePicker()
        Me.lblLockName = New Label()
        Me.txtLockName = New TextBox()
        Me.btnOK = New Button()
        Me.btnCancel = New Button()

        Me.SuspendLayout()

        '
        ' lblSource
        '
        Me.lblSource.AutoSize = True
        Me.lblSource.Font = New Font(Me.Font, FontStyle.Bold)
        Me.lblSource.Location = New Point(20, 20)
        Me.lblSource.Name = "lblSource"
        Me.lblSource.Text = "Copying from:"

        '
        ' lblSourceInfo
        '
        Me.lblSourceInfo.AutoSize = True
        Me.lblSourceInfo.Location = New Point(120, 20)
        Me.lblSourceInfo.Name = "lblSourceInfo"
        Me.lblSourceInfo.Text = ""

        '
        ' chkSameSubdivision
        '
        Me.chkSameSubdivision.AutoSize = True
        Me.chkSameSubdivision.Checked = True
        Me.chkSameSubdivision.CheckState = CheckState.Checked
        Me.chkSameSubdivision.Location = New Point(20, 55)
        Me.chkSameSubdivision.Name = "chkSameSubdivision"
        Me.chkSameSubdivision.Text = "Copy to same subdivision (new date)"

        '
        ' lblTarget
        '
        Me.lblTarget.AutoSize = True
        Me.lblTarget.Location = New Point(20, 88)
        Me.lblTarget.Name = "lblTarget"
        Me.lblTarget.Text = "Target Subdivision:"

        '
        ' cboTargetSubdivision
        '
        Me.cboTargetSubdivision.DropDownStyle = ComboBoxStyle.DropDownList
        Me.cboTargetSubdivision.Enabled = False
        Me.cboTargetSubdivision.Location = New Point(140, 85)
        Me.cboTargetSubdivision.Name = "cboTargetSubdivision"
        Me.cboTargetSubdivision.Width = 270

        '
        ' lblLockDate
        '
        Me.lblLockDate.AutoSize = True
        Me.lblLockDate.Location = New Point(20, 123)
        Me.lblLockDate.Name = "lblLockDate"
        Me.lblLockDate.Text = "New Lock Date:"

        '
        ' dtpLockDate
        '
        Me.dtpLockDate.Format = DateTimePickerFormat.Short
        Me.dtpLockDate.Location = New Point(140, 120)
        Me.dtpLockDate.Name = "dtpLockDate"
        Me.dtpLockDate.Width = 150

        '
        ' lblLockName
        '
        Me.lblLockName.AutoSize = True
        Me.lblLockName.Location = New Point(20, 158)
        Me.lblLockName.Name = "lblLockName"
        Me.lblLockName.Text = "New Lock Name:"

        '
        ' txtLockName
        '
        Me.txtLockName.Location = New Point(140, 155)
        Me.txtLockName.Name = "txtLockName"
        Me.txtLockName.Width = 270

        '
        ' btnOK
        '
        Me.btnOK.DialogResult = DialogResult.None
        Me.btnOK.Height = 30
        Me.btnOK.Location = New Point(230, 200)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Text = "Copy"
        Me.btnOK.Width = 90

        '
        ' btnCancel
        '
        Me.btnCancel.DialogResult = DialogResult.Cancel
        Me.btnCancel.Height = 30
        Me.btnCancel.Location = New Point(330, 200)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.Width = 80

        '
        ' frmPriceLockCopy
        '
        Me.AcceptButton = Me.btnOK
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New Size(450, 280)
        Me.Controls.Add(Me.lblSource)
        Me.Controls.Add(Me.lblSourceInfo)
        Me.Controls.Add(Me.chkSameSubdivision)
        Me.Controls.Add(Me.lblTarget)
        Me.Controls.Add(Me.cboTargetSubdivision)
        Me.Controls.Add(Me.lblLockDate)
        Me.Controls.Add(Me.dtpLockDate)
        Me.Controls.Add(Me.lblLockName)
        Me.Controls.Add(Me.txtLockName)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.btnCancel)
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmPriceLockCopy"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Copy Price Lock"

        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

End Class