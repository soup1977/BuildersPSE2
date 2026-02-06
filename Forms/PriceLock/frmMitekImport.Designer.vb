' =====================================================
' frmMitekImport.Designer.vb
' Designer file for MiTek Import Form
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Partial Public Class frmMitekImport

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
    Friend WithEvents lblFileLabel As Label
    Friend WithEvents lblStatus As Label
    Friend WithEvents lblRecordCount As Label
    Friend WithEvents lblPreview As Label

    ' Input Controls
    Friend WithEvents txtFilePath As TextBox
    Friend WithEvents btnBrowse As Button

    ' Options
    Friend WithEvents grpOptions As GroupBox
    Friend WithEvents chkCreateMissingPlans As CheckBox
    Friend WithEvents chkCreateMissingElevations As CheckBox
    Friend WithEvents chkCreateMissingOptions As CheckBox
    Friend WithEvents chkUpdateExisting As CheckBox

    ' Data Grid
    Friend WithEvents dgvPreview As DataGridView

    ' Progress
    Friend WithEvents prgImport As ProgressBar

    ' Buttons
    Friend WithEvents btnImport As Button
    Friend WithEvents btnCancel As Button

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()

        Me.lblFileLabel = New Label()
        Me.txtFilePath = New TextBox()
        Me.btnBrowse = New Button()

        Me.grpOptions = New GroupBox()
        Me.chkCreateMissingPlans = New CheckBox()
        Me.chkCreateMissingElevations = New CheckBox()
        Me.chkCreateMissingOptions = New CheckBox()
        Me.chkUpdateExisting = New CheckBox()

        Me.lblStatus = New Label()
        Me.lblRecordCount = New Label()
        Me.lblPreview = New Label()
        Me.dgvPreview = New DataGridView()
        Me.prgImport = New ProgressBar()
        Me.btnImport = New Button()
        Me.btnCancel = New Button()

        Me.grpOptions.SuspendLayout()
        CType(Me.dgvPreview, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()

        '
        ' lblFileLabel
        '
        Me.lblFileLabel.AutoSize = True
        Me.lblFileLabel.Location = New Point(15, 18)
        Me.lblFileLabel.Name = "lblFileLabel"
        Me.lblFileLabel.Text = "CSV File:"

        '
        ' txtFilePath
        '
        Me.txtFilePath.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.txtFilePath.Location = New Point(80, 15)
        Me.txtFilePath.Name = "txtFilePath"
        Me.txtFilePath.Width = 650

        '
        ' btnBrowse
        '
        Me.btnBrowse.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnBrowse.Location = New Point(740, 13)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Text = "Browse..."
        Me.btnBrowse.Width = 80

        '
        ' grpOptions
        '
        Me.grpOptions.Controls.Add(Me.chkCreateMissingPlans)
        Me.grpOptions.Controls.Add(Me.chkCreateMissingElevations)
        Me.grpOptions.Controls.Add(Me.chkCreateMissingOptions)
        Me.grpOptions.Controls.Add(Me.chkUpdateExisting)
        Me.grpOptions.Location = New Point(15, 50)
        Me.grpOptions.Name = "grpOptions"
        Me.grpOptions.Size = New Size(400, 100)
        Me.grpOptions.Text = "Import Options"

        '
        ' chkCreateMissingPlans
        '
        Me.chkCreateMissingPlans.AutoSize = True
        Me.chkCreateMissingPlans.Checked = True
        Me.chkCreateMissingPlans.CheckState = CheckState.Checked
        Me.chkCreateMissingPlans.Location = New Point(15, 22)
        Me.chkCreateMissingPlans.Name = "chkCreateMissingPlans"
        Me.chkCreateMissingPlans.Text = "Create missing Plans"

        '
        ' chkCreateMissingElevations
        '
        Me.chkCreateMissingElevations.AutoSize = True
        Me.chkCreateMissingElevations.Checked = True
        Me.chkCreateMissingElevations.CheckState = CheckState.Checked
        Me.chkCreateMissingElevations.Location = New Point(15, 44)
        Me.chkCreateMissingElevations.Name = "chkCreateMissingElevations"
        Me.chkCreateMissingElevations.Text = "Create missing Elevations"

        '
        ' chkCreateMissingOptions
        '
        Me.chkCreateMissingOptions.AutoSize = True
        Me.chkCreateMissingOptions.Checked = True
        Me.chkCreateMissingOptions.CheckState = CheckState.Checked
        Me.chkCreateMissingOptions.Location = New Point(15, 66)
        Me.chkCreateMissingOptions.Name = "chkCreateMissingOptions"
        Me.chkCreateMissingOptions.Text = "Create missing Options"

        '
        ' chkUpdateExisting
        '
        Me.chkUpdateExisting.AutoSize = True
        Me.chkUpdateExisting.Checked = True
        Me.chkUpdateExisting.CheckState = CheckState.Checked
        Me.chkUpdateExisting.Location = New Point(200, 22)
        Me.chkUpdateExisting.Name = "chkUpdateExisting"
        Me.chkUpdateExisting.Text = "Update existing pricing records"

        '
        ' lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New Point(430, 70)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Text = ""

        '
        ' lblRecordCount
        '
        Me.lblRecordCount.AutoSize = True
        Me.lblRecordCount.Location = New Point(430, 95)
        Me.lblRecordCount.Name = "lblRecordCount"
        Me.lblRecordCount.Text = ""

        '
        ' lblPreview
        '
        Me.lblPreview.AutoSize = True
        Me.lblPreview.Font = New Font(Me.Font, FontStyle.Bold)
        Me.lblPreview.Location = New Point(15, 160)
        Me.lblPreview.Name = "lblPreview"
        Me.lblPreview.Text = "Preview:"

        '
        ' dgvPreview
        '
        Me.dgvPreview.AllowUserToAddRows = False
        Me.dgvPreview.AllowUserToDeleteRows = False
        Me.dgvPreview.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right Or AnchorStyles.Bottom
        Me.dgvPreview.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvPreview.BackgroundColor = SystemColors.Window
        Me.dgvPreview.Location = New Point(15, 180)
        Me.dgvPreview.Name = "dgvPreview"
        Me.dgvPreview.ReadOnly = True
        Me.dgvPreview.RowHeadersVisible = False
        Me.dgvPreview.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.dgvPreview.Size = New Size(850, 350)

        '
        ' prgImport
        '
        Me.prgImport.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.prgImport.Location = New Point(15, 540)
        Me.prgImport.Name = "prgImport"
        Me.prgImport.Size = New Size(600, 23)
        Me.prgImport.Visible = False

        '
        ' btnImport
        '
        Me.btnImport.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Me.btnImport.Enabled = False
        Me.btnImport.Location = New Point(710, 575)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New Size(80, 30)
        Me.btnImport.Text = "Import"

        '
        ' btnCancel
        '
        Me.btnCancel.Anchor = AnchorStyles.Bottom Or AnchorStyles.Right
        Me.btnCancel.Location = New Point(800, 575)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New Size(80, 30)
        Me.btnCancel.Text = "Cancel"

        '
        ' frmMitekImport
        '
        Me.ClientSize = New Size(900, 650)
        Me.Controls.Add(Me.lblFileLabel)
        Me.Controls.Add(Me.txtFilePath)
        Me.Controls.Add(Me.btnBrowse)
        Me.Controls.Add(Me.grpOptions)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.lblRecordCount)
        Me.Controls.Add(Me.lblPreview)
        Me.Controls.Add(Me.dgvPreview)
        Me.Controls.Add(Me.prgImport)
        Me.Controls.Add(Me.btnImport)
        Me.Controls.Add(Me.btnCancel)
        Me.MinimumSize = New Size(800, 500)
        Me.Name = "frmMitekImport"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Import MiTek CSV"

        Me.grpOptions.ResumeLayout(False)
        Me.grpOptions.PerformLayout()
        CType(Me.dgvPreview, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

End Class