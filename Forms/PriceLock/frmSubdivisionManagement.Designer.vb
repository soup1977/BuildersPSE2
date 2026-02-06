' =====================================================
' frmSubdivisionManagement.Designer.vb
' Designer file for Subdivision Management Form
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Partial Public Class frmSubdivisionManagement

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

    ' Panels
    Friend WithEvents pnlHeader As Panel
    Friend WithEvents splitContainer As SplitContainer
    Friend WithEvents pnlEdit As Panel
    Friend WithEvents pnlButtons As Panel

    ' Labels
    Friend WithEvents lblBuilderName As Label
    Friend WithEvents lblDetailsHeader As Label
    Friend WithEvents lblName As Label
    Friend WithEvents lblCode As Label
    Friend WithEvents lblCodeHint As Label
    Friend WithEvents lblStatus As Label

    ' Grid
    Friend WithEvents dgvSubdivisions As DataGridView

    ' Input Controls
    Friend WithEvents txtSubdivisionName As TextBox
    Friend WithEvents txtSubdivisionCode As TextBox
    Friend WithEvents chkIsActive As CheckBox

    ' Buttons
    Friend WithEvents btnAdd As Button
    Friend WithEvents btnUpdate As Button
    Friend WithEvents btnManagePlans As Button
    Friend WithEvents btnClose As Button

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.lblBuilderName = New System.Windows.Forms.Label()
        Me.splitContainer = New System.Windows.Forms.SplitContainer()
        Me.dgvSubdivisions = New System.Windows.Forms.DataGridView()
        Me.pnlEdit = New System.Windows.Forms.Panel()
        Me.lblDetailsHeader = New System.Windows.Forms.Label()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtSubdivisionName = New System.Windows.Forms.TextBox()
        Me.lblCode = New System.Windows.Forms.Label()
        Me.txtSubdivisionCode = New System.Windows.Forms.TextBox()
        Me.lblCodeHint = New System.Windows.Forms.Label()
        Me.chkIsActive = New System.Windows.Forms.CheckBox()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnManagePlans = New System.Windows.Forms.Button()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.pnlHeader.SuspendLayout()
        CType(Me.splitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer.Panel1.SuspendLayout()
        Me.splitContainer.Panel2.SuspendLayout()
        Me.splitContainer.SuspendLayout()
        CType(Me.dgvSubdivisions, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlEdit.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlHeader
        '
        Me.pnlHeader.Controls.Add(Me.lblBuilderName)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(875, 35)
        Me.pnlHeader.TabIndex = 2
        '
        'lblBuilderName
        '
        Me.lblBuilderName.AutoSize = True
        Me.lblBuilderName.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblBuilderName.Location = New System.Drawing.Point(10, 10)
        Me.lblBuilderName.Name = "lblBuilderName"
        Me.lblBuilderName.Size = New System.Drawing.Size(50, 13)
        Me.lblBuilderName.TabIndex = 0
        Me.lblBuilderName.Text = "Builder:"
        '
        'splitContainer
        '
        Me.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitContainer.Location = New System.Drawing.Point(0, 35)
        Me.splitContainer.Name = "splitContainer"
        '
        'splitContainer.Panel1
        '
        Me.splitContainer.Panel1.Controls.Add(Me.dgvSubdivisions)
        '
        'splitContainer.Panel2
        '
        Me.splitContainer.Panel2.Controls.Add(Me.pnlEdit)
        Me.splitContainer.Size = New System.Drawing.Size(875, 438)
        Me.splitContainer.SplitterDistance = 579
        Me.splitContainer.TabIndex = 0
        '
        'dgvSubdivisions
        '
        Me.dgvSubdivisions.AllowUserToAddRows = False
        Me.dgvSubdivisions.AllowUserToDeleteRows = False
        Me.dgvSubdivisions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSubdivisions.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvSubdivisions.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSubdivisions.Location = New System.Drawing.Point(0, 0)
        Me.dgvSubdivisions.MultiSelect = False
        Me.dgvSubdivisions.Name = "dgvSubdivisions"
        Me.dgvSubdivisions.ReadOnly = True
        Me.dgvSubdivisions.RowHeadersVisible = False
        Me.dgvSubdivisions.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvSubdivisions.Size = New System.Drawing.Size(579, 438)
        Me.dgvSubdivisions.TabIndex = 0
        '
        'pnlEdit
        '
        Me.pnlEdit.Controls.Add(Me.lblDetailsHeader)
        Me.pnlEdit.Controls.Add(Me.lblName)
        Me.pnlEdit.Controls.Add(Me.txtSubdivisionName)
        Me.pnlEdit.Controls.Add(Me.lblCode)
        Me.pnlEdit.Controls.Add(Me.txtSubdivisionCode)
        Me.pnlEdit.Controls.Add(Me.lblCodeHint)
        Me.pnlEdit.Controls.Add(Me.chkIsActive)
        Me.pnlEdit.Controls.Add(Me.btnAdd)
        Me.pnlEdit.Controls.Add(Me.btnUpdate)
        Me.pnlEdit.Controls.Add(Me.btnManagePlans)
        Me.pnlEdit.Controls.Add(Me.lblStatus)
        Me.pnlEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlEdit.Location = New System.Drawing.Point(0, 0)
        Me.pnlEdit.Name = "pnlEdit"
        Me.pnlEdit.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlEdit.Size = New System.Drawing.Size(292, 438)
        Me.pnlEdit.TabIndex = 0
        '
        'lblDetailsHeader
        '
        Me.lblDetailsHeader.AutoSize = True
        Me.lblDetailsHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblDetailsHeader.Location = New System.Drawing.Point(10, 10)
        Me.lblDetailsHeader.Name = "lblDetailsHeader"
        Me.lblDetailsHeader.Size = New System.Drawing.Size(115, 13)
        Me.lblDetailsHeader.TabIndex = 0
        Me.lblDetailsHeader.Text = "Subdivision Details"
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
        'txtSubdivisionName
        '
        Me.txtSubdivisionName.Location = New System.Drawing.Point(80, 40)
        Me.txtSubdivisionName.Name = "txtSubdivisionName"
        Me.txtSubdivisionName.Size = New System.Drawing.Size(170, 20)
        Me.txtSubdivisionName.TabIndex = 2
        '
        'lblCode
        '
        Me.lblCode.AutoSize = True
        Me.lblCode.Location = New System.Drawing.Point(10, 73)
        Me.lblCode.Name = "lblCode"
        Me.lblCode.Size = New System.Drawing.Size(35, 13)
        Me.lblCode.TabIndex = 3
        Me.lblCode.Text = "Code:"
        '
        'txtSubdivisionCode
        '
        Me.txtSubdivisionCode.Location = New System.Drawing.Point(80, 70)
        Me.txtSubdivisionCode.Name = "txtSubdivisionCode"
        Me.txtSubdivisionCode.Size = New System.Drawing.Size(100, 20)
        Me.txtSubdivisionCode.TabIndex = 4
        '
        'lblCodeHint
        '
        Me.lblCodeHint.AutoSize = True
        Me.lblCodeHint.ForeColor = System.Drawing.Color.Gray
        Me.lblCodeHint.Location = New System.Drawing.Point(185, 73)
        Me.lblCodeHint.Name = "lblCodeHint"
        Me.lblCodeHint.Size = New System.Drawing.Size(80, 13)
        Me.lblCodeHint.TabIndex = 5
        Me.lblCodeHint.Text = "(e.g., P210476)"
        '
        'chkIsActive
        '
        Me.chkIsActive.AutoSize = True
        Me.chkIsActive.Checked = True
        Me.chkIsActive.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkIsActive.Location = New System.Drawing.Point(80, 100)
        Me.chkIsActive.Name = "chkIsActive"
        Me.chkIsActive.Size = New System.Drawing.Size(56, 17)
        Me.chkIsActive.TabIndex = 6
        Me.chkIsActive.Text = "Active"
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(10, 135)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(80, 23)
        Me.btnAdd.TabIndex = 7
        Me.btnAdd.Text = "Add New"
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(100, 135)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(80, 23)
        Me.btnUpdate.TabIndex = 8
        Me.btnUpdate.Text = "Update"
        '
        'btnManagePlans
        '
        Me.btnManagePlans.Location = New System.Drawing.Point(10, 180)
        Me.btnManagePlans.Name = "btnManagePlans"
        Me.btnManagePlans.Size = New System.Drawing.Size(120, 23)
        Me.btnManagePlans.TabIndex = 9
        Me.btnManagePlans.Text = "Manage Plans..."
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.ForeColor = System.Drawing.Color.Green
        Me.lblStatus.Location = New System.Drawing.Point(10, 220)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 13)
        Me.lblStatus.TabIndex = 10
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnClose)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Location = New System.Drawing.Point(0, 473)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(875, 64)
        Me.pnlButtons.TabIndex = 1
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(783, 6)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(80, 23)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        '
        'frmSubdivisionManagement
        '
        Me.ClientSize = New System.Drawing.Size(875, 537)
        Me.Controls.Add(Me.splitContainer)
        Me.Controls.Add(Me.pnlButtons)
        Me.Controls.Add(Me.pnlHeader)
        Me.MinimumSize = New System.Drawing.Size(650, 400)
        Me.Name = "frmSubdivisionManagement"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Subdivision Management"
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        Me.splitContainer.Panel1.ResumeLayout(False)
        Me.splitContainer.Panel2.ResumeLayout(False)
        CType(Me.splitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer.ResumeLayout(False)
        CType(Me.dgvSubdivisions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlEdit.ResumeLayout(False)
        Me.pnlEdit.PerformLayout()
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class
