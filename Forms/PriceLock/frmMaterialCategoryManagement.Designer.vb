' =====================================================
' frmMaterialCategoryManagement.Designer.vb
' Designer file for Material Category Management Form
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Partial Public Class frmMaterialCategoryManagement

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
    Friend WithEvents dgvCategories As DataGridView

    ' Edit Panel
    Friend WithEvents pnlEdit As Panel
    Friend WithEvents lblDetailsHeader As Label
    Friend WithEvents lblName As Label
    Friend WithEvents txtCategoryName As TextBox
    Friend WithEvents lblCode As Label
    Friend WithEvents txtCategoryCode As TextBox
    Friend WithEvents lblDisplayOrder As Label
    Friend WithEvents nudDisplayOrder As NumericUpDown
    Friend WithEvents chkIsActive As CheckBox
    Friend WithEvents lblReorder As Label
    Friend WithEvents lblStatus As Label

    ' Buttons
    Friend WithEvents btnAdd As Button
    Friend WithEvents btnUpdate As Button
    Friend WithEvents btnMoveUp As Button
    Friend WithEvents btnMoveDown As Button
    Friend WithEvents pnlButtons As Panel
    Friend WithEvents btnClose As Button

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.splitContainer = New System.Windows.Forms.SplitContainer()
        Me.dgvCategories = New System.Windows.Forms.DataGridView()
        Me.pnlEdit = New System.Windows.Forms.Panel()
        Me.lblDetailsHeader = New System.Windows.Forms.Label()
        Me.lblName = New System.Windows.Forms.Label()
        Me.txtCategoryName = New System.Windows.Forms.TextBox()
        Me.lblCode = New System.Windows.Forms.Label()
        Me.txtCategoryCode = New System.Windows.Forms.TextBox()
        Me.lblDisplayOrder = New System.Windows.Forms.Label()
        Me.nudDisplayOrder = New System.Windows.Forms.NumericUpDown()
        Me.chkIsActive = New System.Windows.Forms.CheckBox()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.lblReorder = New System.Windows.Forms.Label()
        Me.btnMoveUp = New System.Windows.Forms.Button()
        Me.btnMoveDown = New System.Windows.Forms.Button()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnClose = New System.Windows.Forms.Button()
        CType(Me.splitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer.Panel1.SuspendLayout()
        Me.splitContainer.Panel2.SuspendLayout()
        Me.splitContainer.SuspendLayout()
        CType(Me.dgvCategories, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlEdit.SuspendLayout()
        CType(Me.nudDisplayOrder, System.ComponentModel.ISupportInitialize).BeginInit()
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
        Me.splitContainer.Panel1.Controls.Add(Me.dgvCategories)
        '
        'splitContainer.Panel2
        '
        Me.splitContainer.Panel2.Controls.Add(Me.pnlEdit)
        Me.splitContainer.Size = New System.Drawing.Size(700, 455)
        Me.splitContainer.SplitterDistance = 425
        Me.splitContainer.TabIndex = 0
        '
        'dgvCategories
        '
        Me.dgvCategories.AllowUserToAddRows = False
        Me.dgvCategories.AllowUserToDeleteRows = False
        Me.dgvCategories.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvCategories.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvCategories.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvCategories.Location = New System.Drawing.Point(0, 0)
        Me.dgvCategories.MultiSelect = False
        Me.dgvCategories.Name = "dgvCategories"
        Me.dgvCategories.ReadOnly = True
        Me.dgvCategories.RowHeadersVisible = False
        Me.dgvCategories.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvCategories.Size = New System.Drawing.Size(425, 455)
        Me.dgvCategories.TabIndex = 0
        '
        'pnlEdit
        '
        Me.pnlEdit.Controls.Add(Me.lblDetailsHeader)
        Me.pnlEdit.Controls.Add(Me.lblName)
        Me.pnlEdit.Controls.Add(Me.txtCategoryName)
        Me.pnlEdit.Controls.Add(Me.lblCode)
        Me.pnlEdit.Controls.Add(Me.txtCategoryCode)
        Me.pnlEdit.Controls.Add(Me.lblDisplayOrder)
        Me.pnlEdit.Controls.Add(Me.nudDisplayOrder)
        Me.pnlEdit.Controls.Add(Me.chkIsActive)
        Me.pnlEdit.Controls.Add(Me.btnAdd)
        Me.pnlEdit.Controls.Add(Me.btnUpdate)
        Me.pnlEdit.Controls.Add(Me.lblReorder)
        Me.pnlEdit.Controls.Add(Me.btnMoveUp)
        Me.pnlEdit.Controls.Add(Me.btnMoveDown)
        Me.pnlEdit.Controls.Add(Me.lblStatus)
        Me.pnlEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlEdit.Location = New System.Drawing.Point(0, 0)
        Me.pnlEdit.Name = "pnlEdit"
        Me.pnlEdit.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlEdit.Size = New System.Drawing.Size(271, 455)
        Me.pnlEdit.TabIndex = 0
        '
        'lblDetailsHeader
        '
        Me.lblDetailsHeader.AutoSize = True
        Me.lblDetailsHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.lblDetailsHeader.Location = New System.Drawing.Point(10, 10)
        Me.lblDetailsHeader.Name = "lblDetailsHeader"
        Me.lblDetailsHeader.Size = New System.Drawing.Size(100, 13)
        Me.lblDetailsHeader.TabIndex = 0
        Me.lblDetailsHeader.Text = "Category Details"
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
        'txtCategoryName
        '
        Me.txtCategoryName.Location = New System.Drawing.Point(100, 40)
        Me.txtCategoryName.Name = "txtCategoryName"
        Me.txtCategoryName.Size = New System.Drawing.Size(150, 20)
        Me.txtCategoryName.TabIndex = 2
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
        'txtCategoryCode
        '
        Me.txtCategoryCode.Location = New System.Drawing.Point(100, 70)
        Me.txtCategoryCode.Name = "txtCategoryCode"
        Me.txtCategoryCode.Size = New System.Drawing.Size(80, 20)
        Me.txtCategoryCode.TabIndex = 4
        '
        'lblDisplayOrder
        '
        Me.lblDisplayOrder.AutoSize = True
        Me.lblDisplayOrder.Location = New System.Drawing.Point(10, 103)
        Me.lblDisplayOrder.Name = "lblDisplayOrder"
        Me.lblDisplayOrder.Size = New System.Drawing.Size(73, 13)
        Me.lblDisplayOrder.TabIndex = 5
        Me.lblDisplayOrder.Text = "Display Order:"
        '
        'nudDisplayOrder
        '
        Me.nudDisplayOrder.Location = New System.Drawing.Point(100, 100)
        Me.nudDisplayOrder.Maximum = New Decimal(New Integer() {999, 0, 0, 0})
        Me.nudDisplayOrder.Name = "nudDisplayOrder"
        Me.nudDisplayOrder.Size = New System.Drawing.Size(60, 20)
        Me.nudDisplayOrder.TabIndex = 6
        '
        'chkIsActive
        '
        Me.chkIsActive.AutoSize = True
        Me.chkIsActive.Checked = True
        Me.chkIsActive.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkIsActive.Location = New System.Drawing.Point(100, 130)
        Me.chkIsActive.Name = "chkIsActive"
        Me.chkIsActive.Size = New System.Drawing.Size(56, 17)
        Me.chkIsActive.TabIndex = 7
        Me.chkIsActive.Text = "Active"
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(10, 165)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(80, 23)
        Me.btnAdd.TabIndex = 8
        Me.btnAdd.Text = "Add New"
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(100, 165)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(80, 23)
        Me.btnUpdate.TabIndex = 9
        Me.btnUpdate.Text = "Update"
        '
        'lblReorder
        '
        Me.lblReorder.AutoSize = True
        Me.lblReorder.Location = New System.Drawing.Point(10, 208)
        Me.lblReorder.Name = "lblReorder"
        Me.lblReorder.Size = New System.Drawing.Size(48, 13)
        Me.lblReorder.TabIndex = 10
        Me.lblReorder.Text = "Reorder:"
        '
        'btnMoveUp
        '
        Me.btnMoveUp.Location = New System.Drawing.Point(100, 205)
        Me.btnMoveUp.Name = "btnMoveUp"
        Me.btnMoveUp.Size = New System.Drawing.Size(60, 23)
        Me.btnMoveUp.TabIndex = 11
        Me.btnMoveUp.Text = "▲ Up"
        '
        'btnMoveDown
        '
        Me.btnMoveDown.Location = New System.Drawing.Point(165, 205)
        Me.btnMoveDown.Name = "btnMoveDown"
        Me.btnMoveDown.Size = New System.Drawing.Size(60, 23)
        Me.btnMoveDown.TabIndex = 12
        Me.btnMoveDown.Text = "▼ Down"
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.ForeColor = System.Drawing.Color.Green
        Me.lblStatus.Location = New System.Drawing.Point(10, 245)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 13)
        Me.lblStatus.TabIndex = 13
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnClose)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Location = New System.Drawing.Point(0, 455)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(700, 45)
        Me.pnlButtons.TabIndex = 1
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(608, 6)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(80, 23)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        '
        'frmMaterialCategoryManagement
        '
        Me.ClientSize = New System.Drawing.Size(700, 500)
        Me.Controls.Add(Me.splitContainer)
        Me.Controls.Add(Me.pnlButtons)
        Me.MinimumSize = New System.Drawing.Size(600, 400)
        Me.Name = "frmMaterialCategoryManagement"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Material Category Management"
        Me.splitContainer.Panel1.ResumeLayout(False)
        Me.splitContainer.Panel2.ResumeLayout(False)
        CType(Me.splitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer.ResumeLayout(False)
        CType(Me.dgvCategories, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlEdit.ResumeLayout(False)
        Me.pnlEdit.PerformLayout()
        CType(Me.nudDisplayOrder, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class
