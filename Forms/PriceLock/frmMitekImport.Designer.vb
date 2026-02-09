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
    ' Add this declaration with other controls:
    Friend WithEvents btnResolveOptions As Button

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.lblFileLabel = New System.Windows.Forms.Label()
        Me.txtFilePath = New System.Windows.Forms.TextBox()
        Me.btnBrowse = New System.Windows.Forms.Button()
        Me.grpOptions = New System.Windows.Forms.GroupBox()
        Me.chkCreateMissingPlans = New System.Windows.Forms.CheckBox()
        Me.chkCreateMissingElevations = New System.Windows.Forms.CheckBox()
        Me.chkCreateMissingOptions = New System.Windows.Forms.CheckBox()
        Me.chkUpdateExisting = New System.Windows.Forms.CheckBox()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.lblRecordCount = New System.Windows.Forms.Label()
        Me.lblPreview = New System.Windows.Forms.Label()
        Me.dgvPreview = New System.Windows.Forms.DataGridView()
        Me.prgImport = New System.Windows.Forms.ProgressBar()
        Me.btnImport = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnResolveOptions = New System.Windows.Forms.Button()
        Me.grpOptions.SuspendLayout()
        CType(Me.dgvPreview, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblFileLabel
        '
        Me.lblFileLabel.AutoSize = True
        Me.lblFileLabel.Location = New System.Drawing.Point(15, 18)
        Me.lblFileLabel.Name = "lblFileLabel"
        Me.lblFileLabel.Size = New System.Drawing.Size(50, 13)
        Me.lblFileLabel.TabIndex = 1
        Me.lblFileLabel.Text = "CSV File:"
        '
        'txtFilePath
        '
        Me.txtFilePath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtFilePath.Location = New System.Drawing.Point(80, 15)
        Me.txtFilePath.Name = "txtFilePath"
        Me.txtFilePath.Size = New System.Drawing.Size(650, 20)
        Me.txtFilePath.TabIndex = 2
        '
        'btnBrowse
        '
        Me.btnBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBrowse.Location = New System.Drawing.Point(740, 13)
        Me.btnBrowse.Name = "btnBrowse"
        Me.btnBrowse.Size = New System.Drawing.Size(80, 23)
        Me.btnBrowse.TabIndex = 3
        Me.btnBrowse.Text = "Browse..."
        '
        'grpOptions
        '
        Me.grpOptions.Controls.Add(Me.chkCreateMissingPlans)
        Me.grpOptions.Controls.Add(Me.chkCreateMissingElevations)
        Me.grpOptions.Controls.Add(Me.chkCreateMissingOptions)
        Me.grpOptions.Controls.Add(Me.chkUpdateExisting)
        Me.grpOptions.Location = New System.Drawing.Point(15, 50)
        Me.grpOptions.Name = "grpOptions"
        Me.grpOptions.Size = New System.Drawing.Size(400, 100)
        Me.grpOptions.TabIndex = 4
        Me.grpOptions.TabStop = False
        Me.grpOptions.Text = "Import Options"
        '
        'chkCreateMissingPlans
        '
        Me.chkCreateMissingPlans.AutoSize = True
        Me.chkCreateMissingPlans.Checked = True
        Me.chkCreateMissingPlans.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCreateMissingPlans.Location = New System.Drawing.Point(15, 22)
        Me.chkCreateMissingPlans.Name = "chkCreateMissingPlans"
        Me.chkCreateMissingPlans.Size = New System.Drawing.Size(123, 17)
        Me.chkCreateMissingPlans.TabIndex = 0
        Me.chkCreateMissingPlans.Text = "Create missing Plans"
        '
        'chkCreateMissingElevations
        '
        Me.chkCreateMissingElevations.AutoSize = True
        Me.chkCreateMissingElevations.Checked = True
        Me.chkCreateMissingElevations.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCreateMissingElevations.Location = New System.Drawing.Point(15, 44)
        Me.chkCreateMissingElevations.Name = "chkCreateMissingElevations"
        Me.chkCreateMissingElevations.Size = New System.Drawing.Size(146, 17)
        Me.chkCreateMissingElevations.TabIndex = 1
        Me.chkCreateMissingElevations.Text = "Create missing Elevations"
        '
        'chkCreateMissingOptions
        '
        Me.chkCreateMissingOptions.AutoSize = True
        Me.chkCreateMissingOptions.Checked = True
        Me.chkCreateMissingOptions.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCreateMissingOptions.Location = New System.Drawing.Point(15, 66)
        Me.chkCreateMissingOptions.Name = "chkCreateMissingOptions"
        Me.chkCreateMissingOptions.Size = New System.Drawing.Size(133, 17)
        Me.chkCreateMissingOptions.TabIndex = 2
        Me.chkCreateMissingOptions.Text = "Create missing Options"
        '
        'chkUpdateExisting
        '
        Me.chkUpdateExisting.AutoSize = True
        Me.chkUpdateExisting.Checked = True
        Me.chkUpdateExisting.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkUpdateExisting.Location = New System.Drawing.Point(200, 22)
        Me.chkUpdateExisting.Name = "chkUpdateExisting"
        Me.chkUpdateExisting.Size = New System.Drawing.Size(171, 17)
        Me.chkUpdateExisting.TabIndex = 3
        Me.chkUpdateExisting.Text = "Update existing pricing records"
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(430, 70)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 13)
        Me.lblStatus.TabIndex = 5
        '
        'lblRecordCount
        '
        Me.lblRecordCount.AutoSize = True
        Me.lblRecordCount.Location = New System.Drawing.Point(430, 95)
        Me.lblRecordCount.Name = "lblRecordCount"
        Me.lblRecordCount.Size = New System.Drawing.Size(0, 13)
        Me.lblRecordCount.TabIndex = 6
        '
        'lblPreview
        '
        Me.lblPreview.AutoSize = True
        Me.lblPreview.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblPreview.Location = New System.Drawing.Point(15, 160)
        Me.lblPreview.Name = "lblPreview"
        Me.lblPreview.Size = New System.Drawing.Size(56, 13)
        Me.lblPreview.TabIndex = 7
        Me.lblPreview.Text = "Preview:"
        '
        'dgvPreview
        '
        Me.dgvPreview.AllowUserToAddRows = False
        Me.dgvPreview.AllowUserToDeleteRows = False
        Me.dgvPreview.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvPreview.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvPreview.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvPreview.Location = New System.Drawing.Point(15, 180)
        Me.dgvPreview.Name = "dgvPreview"
        Me.dgvPreview.ReadOnly = True
        Me.dgvPreview.RowHeadersVisible = False
        Me.dgvPreview.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvPreview.Size = New System.Drawing.Size(850, 350)
        Me.dgvPreview.TabIndex = 8
        '
        'prgImport
        '
        Me.prgImport.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.prgImport.Location = New System.Drawing.Point(15, 540)
        Me.prgImport.Name = "prgImport"
        Me.prgImport.Size = New System.Drawing.Size(600, 23)
        Me.prgImport.TabIndex = 9
        Me.prgImport.Visible = False
        '
        'btnImport
        '
        Me.btnImport.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnImport.Enabled = False
        Me.btnImport.Location = New System.Drawing.Point(710, 575)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New System.Drawing.Size(80, 30)
        Me.btnImport.TabIndex = 10
        Me.btnImport.Text = "Import"
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.Location = New System.Drawing.Point(800, 575)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(80, 30)
        Me.btnCancel.TabIndex = 11
        Me.btnCancel.Text = "Cancel"
        '
        'btnResolveOptions
        '
        Me.btnResolveOptions.Enabled = False
        Me.btnResolveOptions.Location = New System.Drawing.Point(15, 575)
        Me.btnResolveOptions.Name = "btnResolveOptions"
        Me.btnResolveOptions.Size = New System.Drawing.Size(150, 28)
        Me.btnResolveOptions.TabIndex = 0
        Me.btnResolveOptions.Text = "Resolve Options..."
        '
        'frmMitekImport
        '
        Me.ClientSize = New System.Drawing.Size(900, 650)
        Me.Controls.Add(Me.btnResolveOptions)
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
        Me.MinimumSize = New System.Drawing.Size(800, 500)
        Me.Name = "frmMitekImport"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Import MiTek CSV"
        Me.grpOptions.ResumeLayout(False)
        Me.grpOptions.PerformLayout()
        CType(Me.dgvPreview, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

#End Region

End Class