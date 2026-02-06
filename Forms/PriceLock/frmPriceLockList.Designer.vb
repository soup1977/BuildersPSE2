' =====================================================
' frmPriceLockList.Designer.vb
' Designer file for Price Lock List Form
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing

Partial Public Class frmPriceLockList

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
    Friend WithEvents pnlFilters As Panel
    Friend WithEvents pnlButtons As Panel

    ' Filter Controls
    Friend WithEvents lblBuilder As Label
    Friend WithEvents cboBuilder As ComboBox
    Friend WithEvents lblSubdivision As Label
    Friend WithEvents cboSubdivision As ComboBox
    Friend WithEvents lblStatus As Label
    Friend WithEvents cboStatus As ComboBox
    Friend WithEvents chkDateFilter As CheckBox
    Friend WithEvents lblFrom As Label
    Friend WithEvents dtpStartDate As DateTimePicker
    Friend WithEvents lblTo As Label
    Friend WithEvents dtpEndDate As DateTimePicker
    Friend WithEvents btnSearch As Button
    Friend WithEvents btnClearFilters As Button

    ' Data Grid
    Friend WithEvents dgvPriceLocks As DataGridView

    ' Action Buttons
    Friend WithEvents btnNewPriceLock As Button
    Friend WithEvents btnEditPriceLock As Button
    Friend WithEvents btnCopyPriceLock As Button
    Friend WithEvents btnDeletePriceLock As Button
    Friend WithEvents btnRefresh As Button
    Friend WithEvents lblRecordCount As Label
    Friend WithEvents btnAdmin As Button

    ''' <summary>
    ''' Required method for Designer support - do not modify
    ''' the contents of this method with the code editor.
    ''' </summary>
    Private Sub InitializeComponent()
        Me.pnlFilters = New System.Windows.Forms.Panel()
        Me.lblBuilder = New System.Windows.Forms.Label()
        Me.cboBuilder = New System.Windows.Forms.ComboBox()
        Me.lblSubdivision = New System.Windows.Forms.Label()
        Me.cboSubdivision = New System.Windows.Forms.ComboBox()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.cboStatus = New System.Windows.Forms.ComboBox()
        Me.chkDateFilter = New System.Windows.Forms.CheckBox()
        Me.lblFrom = New System.Windows.Forms.Label()
        Me.dtpStartDate = New System.Windows.Forms.DateTimePicker()
        Me.lblTo = New System.Windows.Forms.Label()
        Me.dtpEndDate = New System.Windows.Forms.DateTimePicker()
        Me.btnSearch = New System.Windows.Forms.Button()
        Me.btnClearFilters = New System.Windows.Forms.Button()
        Me.dgvPriceLocks = New System.Windows.Forms.DataGridView()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnNewPriceLock = New System.Windows.Forms.Button()
        Me.btnEditPriceLock = New System.Windows.Forms.Button()
        Me.btnCopyPriceLock = New System.Windows.Forms.Button()
        Me.btnDeletePriceLock = New System.Windows.Forms.Button()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.lblRecordCount = New System.Windows.Forms.Label()
        Me.btnAdmin = New System.Windows.Forms.Button()
        Me.pnlFilters.SuspendLayout()
        CType(Me.dgvPriceLocks, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlFilters
        '
        Me.pnlFilters.Controls.Add(Me.lblBuilder)
        Me.pnlFilters.Controls.Add(Me.cboBuilder)
        Me.pnlFilters.Controls.Add(Me.lblSubdivision)
        Me.pnlFilters.Controls.Add(Me.cboSubdivision)
        Me.pnlFilters.Controls.Add(Me.lblStatus)
        Me.pnlFilters.Controls.Add(Me.cboStatus)
        Me.pnlFilters.Controls.Add(Me.chkDateFilter)
        Me.pnlFilters.Controls.Add(Me.lblFrom)
        Me.pnlFilters.Controls.Add(Me.dtpStartDate)
        Me.pnlFilters.Controls.Add(Me.lblTo)
        Me.pnlFilters.Controls.Add(Me.dtpEndDate)
        Me.pnlFilters.Controls.Add(Me.btnSearch)
        Me.pnlFilters.Controls.Add(Me.btnClearFilters)
        Me.pnlFilters.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlFilters.Location = New System.Drawing.Point(0, 0)
        Me.pnlFilters.Name = "pnlFilters"
        Me.pnlFilters.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlFilters.Size = New System.Drawing.Size(1200, 100)
        Me.pnlFilters.TabIndex = 2
        '
        'lblBuilder
        '
        Me.lblBuilder.AutoSize = True
        Me.lblBuilder.Location = New System.Drawing.Point(10, 15)
        Me.lblBuilder.Name = "lblBuilder"
        Me.lblBuilder.Size = New System.Drawing.Size(42, 13)
        Me.lblBuilder.TabIndex = 0
        Me.lblBuilder.Text = "Builder:"
        '
        'cboBuilder
        '
        Me.cboBuilder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboBuilder.Location = New System.Drawing.Point(70, 12)
        Me.cboBuilder.Name = "cboBuilder"
        Me.cboBuilder.Size = New System.Drawing.Size(180, 21)
        Me.cboBuilder.TabIndex = 1
        '
        'lblSubdivision
        '
        Me.lblSubdivision.AutoSize = True
        Me.lblSubdivision.Location = New System.Drawing.Point(270, 15)
        Me.lblSubdivision.Name = "lblSubdivision"
        Me.lblSubdivision.Size = New System.Drawing.Size(64, 13)
        Me.lblSubdivision.TabIndex = 2
        Me.lblSubdivision.Text = "Subdivision:"
        '
        'cboSubdivision
        '
        Me.cboSubdivision.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSubdivision.Location = New System.Drawing.Point(350, 12)
        Me.cboSubdivision.Name = "cboSubdivision"
        Me.cboSubdivision.Size = New System.Drawing.Size(200, 21)
        Me.cboSubdivision.TabIndex = 3
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(570, 15)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(40, 13)
        Me.lblStatus.TabIndex = 4
        Me.lblStatus.Text = "Status:"
        '
        'cboStatus
        '
        Me.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStatus.Location = New System.Drawing.Point(620, 12)
        Me.cboStatus.Name = "cboStatus"
        Me.cboStatus.Size = New System.Drawing.Size(120, 21)
        Me.cboStatus.TabIndex = 5
        '
        'chkDateFilter
        '
        Me.chkDateFilter.AutoSize = True
        Me.chkDateFilter.Location = New System.Drawing.Point(10, 50)
        Me.chkDateFilter.Name = "chkDateFilter"
        Me.chkDateFilter.Size = New System.Drawing.Size(91, 17)
        Me.chkDateFilter.TabIndex = 6
        Me.chkDateFilter.Text = "Filter by Date:"
        '
        'lblFrom
        '
        Me.lblFrom.AutoSize = True
        Me.lblFrom.Location = New System.Drawing.Point(120, 52)
        Me.lblFrom.Name = "lblFrom"
        Me.lblFrom.Size = New System.Drawing.Size(33, 13)
        Me.lblFrom.TabIndex = 7
        Me.lblFrom.Text = "From:"
        '
        'dtpStartDate
        '
        Me.dtpStartDate.Enabled = False
        Me.dtpStartDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpStartDate.Location = New System.Drawing.Point(160, 48)
        Me.dtpStartDate.Name = "dtpStartDate"
        Me.dtpStartDate.Size = New System.Drawing.Size(120, 20)
        Me.dtpStartDate.TabIndex = 8
        '
        'lblTo
        '
        Me.lblTo.AutoSize = True
        Me.lblTo.Location = New System.Drawing.Point(290, 52)
        Me.lblTo.Name = "lblTo"
        Me.lblTo.Size = New System.Drawing.Size(23, 13)
        Me.lblTo.TabIndex = 9
        Me.lblTo.Text = "To:"
        '
        'dtpEndDate
        '
        Me.dtpEndDate.Enabled = False
        Me.dtpEndDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpEndDate.Location = New System.Drawing.Point(320, 48)
        Me.dtpEndDate.Name = "dtpEndDate"
        Me.dtpEndDate.Size = New System.Drawing.Size(120, 20)
        Me.dtpEndDate.TabIndex = 10
        '
        'btnSearch
        '
        Me.btnSearch.Location = New System.Drawing.Point(480, 46)
        Me.btnSearch.Name = "btnSearch"
        Me.btnSearch.Size = New System.Drawing.Size(80, 23)
        Me.btnSearch.TabIndex = 11
        Me.btnSearch.Text = "Search"
        '
        'btnClearFilters
        '
        Me.btnClearFilters.Location = New System.Drawing.Point(570, 46)
        Me.btnClearFilters.Name = "btnClearFilters"
        Me.btnClearFilters.Size = New System.Drawing.Size(90, 23)
        Me.btnClearFilters.TabIndex = 12
        Me.btnClearFilters.Text = "Clear Filters"
        '
        'dgvPriceLocks
        '
        Me.dgvPriceLocks.AllowUserToAddRows = False
        Me.dgvPriceLocks.AllowUserToDeleteRows = False
        Me.dgvPriceLocks.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvPriceLocks.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvPriceLocks.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.dgvPriceLocks.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvPriceLocks.Location = New System.Drawing.Point(0, 100)
        Me.dgvPriceLocks.MultiSelect = False
        Me.dgvPriceLocks.Name = "dgvPriceLocks"
        Me.dgvPriceLocks.ReadOnly = True
        Me.dgvPriceLocks.RowHeadersVisible = False
        Me.dgvPriceLocks.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvPriceLocks.Size = New System.Drawing.Size(1200, 550)
        Me.dgvPriceLocks.TabIndex = 0
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnNewPriceLock)
        Me.pnlButtons.Controls.Add(Me.btnEditPriceLock)
        Me.pnlButtons.Controls.Add(Me.btnCopyPriceLock)
        Me.pnlButtons.Controls.Add(Me.btnDeletePriceLock)
        Me.pnlButtons.Controls.Add(Me.btnRefresh)
        Me.pnlButtons.Controls.Add(Me.lblRecordCount)
        Me.pnlButtons.Controls.Add(Me.btnAdmin)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Location = New System.Drawing.Point(0, 650)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlButtons.Size = New System.Drawing.Size(1200, 50)
        Me.pnlButtons.TabIndex = 1
        '
        'btnNewPriceLock
        '
        Me.btnNewPriceLock.Location = New System.Drawing.Point(10, 10)
        Me.btnNewPriceLock.Name = "btnNewPriceLock"
        Me.btnNewPriceLock.Size = New System.Drawing.Size(110, 30)
        Me.btnNewPriceLock.TabIndex = 0
        Me.btnNewPriceLock.Text = "New Price Lock"
        '
        'btnEditPriceLock
        '
        Me.btnEditPriceLock.Location = New System.Drawing.Point(130, 10)
        Me.btnEditPriceLock.Name = "btnEditPriceLock"
        Me.btnEditPriceLock.Size = New System.Drawing.Size(80, 30)
        Me.btnEditPriceLock.TabIndex = 1
        Me.btnEditPriceLock.Text = "Edit"
        '
        'btnCopyPriceLock
        '
        Me.btnCopyPriceLock.Location = New System.Drawing.Point(220, 10)
        Me.btnCopyPriceLock.Name = "btnCopyPriceLock"
        Me.btnCopyPriceLock.Size = New System.Drawing.Size(80, 30)
        Me.btnCopyPriceLock.TabIndex = 2
        Me.btnCopyPriceLock.Text = "Copy"
        '
        'btnDeletePriceLock
        '
        Me.btnDeletePriceLock.Location = New System.Drawing.Point(310, 10)
        Me.btnDeletePriceLock.Name = "btnDeletePriceLock"
        Me.btnDeletePriceLock.Size = New System.Drawing.Size(80, 30)
        Me.btnDeletePriceLock.TabIndex = 3
        Me.btnDeletePriceLock.Text = "Delete"
        '
        'btnRefresh
        '
        Me.btnRefresh.Location = New System.Drawing.Point(410, 10)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(80, 30)
        Me.btnRefresh.TabIndex = 4
        Me.btnRefresh.Text = "Refresh"
        '
        'lblRecordCount
        '
        Me.lblRecordCount.AutoSize = True
        Me.lblRecordCount.Location = New System.Drawing.Point(520, 17)
        Me.lblRecordCount.Name = "lblRecordCount"
        Me.lblRecordCount.Size = New System.Drawing.Size(51, 13)
        Me.lblRecordCount.TabIndex = 5
        Me.lblRecordCount.Text = "0 records"
        '
        'btnAdmin
        '
        Me.btnAdmin.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAdmin.Location = New System.Drawing.Point(1090, 10)
        Me.btnAdmin.Name = "btnAdmin"
        Me.btnAdmin.Size = New System.Drawing.Size(90, 30)
        Me.btnAdmin.TabIndex = 6
        Me.btnAdmin.Text = "⚙ Setup..."
        '
        'frmPriceLockList
        '
        Me.ClientSize = New System.Drawing.Size(1200, 700)
        Me.Controls.Add(Me.dgvPriceLocks)
        Me.Controls.Add(Me.pnlButtons)
        Me.Controls.Add(Me.pnlFilters)
        Me.MinimumSize = New System.Drawing.Size(900, 500)
        Me.Name = "frmPriceLockList"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Price Lock Management"
        Me.pnlFilters.ResumeLayout(False)
        Me.pnlFilters.PerformLayout()
        CType(Me.dgvPriceLocks, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlButtons.ResumeLayout(False)
        Me.pnlButtons.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

#End Region

End Class