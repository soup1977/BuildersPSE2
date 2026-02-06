' =====================================================
' frmOptionManagement.Designer.vb
' Designer file for Option Management Form
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Partial Public Class frmOptionManagement

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

    ' Split Container
    Friend WithEvents splitContainer As SplitContainer

    ' Grid
    Friend WithEvents dgvOptions As DataGridView

    ' Edit Panel
    Friend WithEvents pnlEdit As Panel
    Friend WithEvents lblDetailsHeader As Label
    Friend WithEvents lblName As Label
    Friend WithEvents txtOptionName As TextBox
    Friend WithEvents lblDescription As Label
    Friend WithEvents txtOptionDescription As TextBox
    Friend WithEvents chkIsActive As CheckBox
    Friend WithEvents lblStatus As Label

    ' Buttons
    Friend WithEvents btnAdd As Button
    Friend WithEvents btnUpdate As Button
    Friend WithEvents pnlButtons As Panel
    Friend WithEvents btnClose As Button

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.splitContainer = New System.Windows.Forms.SplitContainer()
        Me.dgvOptions = New System.Windows.Forms.DataGridView()
        Me.pnlEdit = New System.Windows.Forms.Panel()
        Me.lblDetailsHeader = New System.Windows.Forms.Label()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtOptionName = New System.Windows.Forms.TextBox()
        Me.lblDescription = New System.Windows.Forms.Label()
        Me.txtOptionDescription = New System.Windows.Forms.TextBox()
        Me.chkIsActive = New System.Windows.Forms.CheckBox()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnClose = New System.Windows.Forms.Button()
        CType(Me.splitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer.Panel1.SuspendLayout()
        Me.splitContainer.Panel2.SuspendLayout()
        Me.splitContainer.SuspendLayout()
        CType(Me.dgvOptions, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlEdit.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'splitContainer
        '
        Me.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitContainer.Location = New System.Drawing.Point(0, 0)
        Me.splitContainer.Name = "splitContainer"
        '
        'splitContainer.Panel1
        '
        Me.splitContainer.Panel1.Controls.Add(Me.dgvOptions)
        '
        'splitContainer.Panel2
        '
        Me.splitContainer.Panel2.Controls.Add(Me.pnlEdit)
        Me.splitContainer.Size = New System.Drawing.Size(735, 419)
        Me.splitContainer.SplitterDistance = 476
        Me.splitContainer.TabIndex = 0
        '
        'dgvOptions
        '
        Me.dgvOptions.AllowUserToAddRows = False
        Me.dgvOptions.AllowUserToDeleteRows = False
        Me.dgvOptions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvOptions.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvOptions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvOptions.Location = New System.Drawing.Point(0, 0)
        Me.dgvOptions.MultiSelect = False
        Me.dgvOptions.Name = "dgvOptions"
        Me.dgvOptions.ReadOnly = True
        Me.dgvOptions.RowHeadersVisible = False
        Me.dgvOptions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvOptions.Size = New System.Drawing.Size(476, 419)
        Me.dgvOptions.TabIndex = 0
        '
        'pnlEdit
        '
        Me.pnlEdit.Controls.Add(Me.lblDetailsHeader)
        Me.pnlEdit.Controls.Add(Me.lblName)
        Me.pnlEdit.Controls.Add(Me.txtOptionName)
        Me.pnlEdit.Controls.Add(Me.lblDescription)
        Me.pnlEdit.Controls.Add(Me.txtOptionDescription)
        Me.pnlEdit.Controls.Add(Me.chkIsActive)
        Me.pnlEdit.Controls.Add(Me.btnAdd)
        Me.pnlEdit.Controls.Add(Me.btnUpdate)
        Me.pnlEdit.Controls.Add(Me.lblStatus)
        Me.pnlEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlEdit.Location = New System.Drawing.Point(0, 0)
        Me.pnlEdit.Name = "pnlEdit"
        Me.pnlEdit.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlEdit.Size = New System.Drawing.Size(255, 419)
        Me.pnlEdit.TabIndex = 0
        '
        'lblDetailsHeader
        '
        Me.lblDetailsHeader.AutoSize = True
        Me.lblDetailsHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblDetailsHeader.Location = New System.Drawing.Point(10, 10)
        Me.lblDetailsHeader.Name = "lblDetailsHeader"
        Me.lblDetailsHeader.Size = New System.Drawing.Size(87, 13)
        Me.lblDetailsHeader.TabIndex = 0
        Me.lblDetailsHeader.Text = "Option Details"
        '
        'lblName
        '
        Me.lblName.AutoSize = True
        Me.lblName.Location = New System.Drawing.Point(10, 43)
        Me.lblName.Name = "lblName"
        Me.lblName.Size = New System.Drawing.Size(38, 13)
        Me.lblName.TabIndex = 1
        Me.lblName.Text = "Name:"
        '
        'txtOptionName
        '
        Me.txtOptionName.Location = New System.Drawing.Point(90, 40)
        Me.txtOptionName.Name = "txtOptionName"
        Me.txtOptionName.Size = New System.Drawing.Size(150, 20)
        Me.txtOptionName.TabIndex = 2
        '
        'lblDescription
        '
        Me.lblDescription.AutoSize = True
        Me.lblDescription.Location = New System.Drawing.Point(10, 73)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Size = New System.Drawing.Size(63, 13)
        Me.lblDescription.TabIndex = 3
        Me.lblDescription.Text = "Description:"
        '
        'txtOptionDescription
        '
        Me.txtOptionDescription.Location = New System.Drawing.Point(90, 70)
        Me.txtOptionDescription.Multiline = True
        Me.txtOptionDescription.Name = "txtOptionDescription"
        Me.txtOptionDescription.Size = New System.Drawing.Size(150, 60)
        Me.txtOptionDescription.TabIndex = 4
        '
        'chkIsActive
        '
        Me.chkIsActive.AutoSize = True
        Me.chkIsActive.Checked = True
        Me.chkIsActive.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkIsActive.Location = New System.Drawing.Point(90, 140)
        Me.chkIsActive.Name = "chkIsActive"
        Me.chkIsActive.Size = New System.Drawing.Size(56, 17)
        Me.chkIsActive.TabIndex = 5
        Me.chkIsActive.Text = "Active"
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(10, 175)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(80, 23)
        Me.btnAdd.TabIndex = 6
        Me.btnAdd.Text = "Add New"
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(100, 175)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(80, 23)
        Me.btnUpdate.TabIndex = 7
        Me.btnUpdate.Text = "Update"
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.ForeColor = System.Drawing.Color.Green
        Me.lblStatus.Location = New System.Drawing.Point(10, 215)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 13)
        Me.lblStatus.TabIndex = 8
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnClose)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Location = New System.Drawing.Point(0, 419)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(735, 48)
        Me.pnlButtons.TabIndex = 1
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(643, 6)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(80, 23)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        '
        'frmOptionManagement
        '
        Me.ClientSize = New System.Drawing.Size(735, 467)
        Me.Controls.Add(Me.splitContainer)
        Me.Controls.Add(Me.pnlButtons)
        Me.MinimumSize = New System.Drawing.Size(550, 350)
        Me.Name = "frmOptionManagement"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Option Management"
        Me.splitContainer.Panel1.ResumeLayout(False)
        Me.splitContainer.Panel2.ResumeLayout(False)
        CType(Me.splitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer.ResumeLayout(False)
        CType(Me.dgvOptions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlEdit.ResumeLayout(False)
        Me.pnlEdit.PerformLayout()
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class
