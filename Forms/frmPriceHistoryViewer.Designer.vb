<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmPriceHistoryViewer
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.SplitContainerMain = New System.Windows.Forms.SplitContainer()
        Me.SplitContainerLeft = New System.Windows.Forms.SplitContainer()
        Me.GroupBoxHistory = New System.Windows.Forms.GroupBox()
        Me.dgvHistory = New System.Windows.Forms.DataGridView()
        Me.PanelHistoryFilter = New System.Windows.Forms.Panel()
        Me.btnRefresh = New System.Windows.Forms.Button()
        Me.cboVersionFilter = New System.Windows.Forms.ComboBox()
        Me.lblVersionFilter = New System.Windows.Forms.Label()
        Me.GroupBoxSummary = New System.Windows.Forms.GroupBox()
        Me.dgvSummary = New System.Windows.Forms.DataGridView()
        Me.SplitContainerRight = New System.Windows.Forms.SplitContainer()
        Me.GroupBoxBuildings = New System.Windows.Forms.GroupBox()
        Me.dgvBuildings = New System.Windows.Forms.DataGridView()
        Me.GroupBoxLevels = New System.Windows.Forms.GroupBox()
        Me.dgvLevels = New System.Windows.Forms.DataGridView()
        Me.PanelBottom = New System.Windows.Forms.Panel()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.btnExport = New System.Windows.Forms.Button()
        Me.btnCompare = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        CType(Me.SplitContainerMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerMain.Panel1.SuspendLayout()
        Me.SplitContainerMain.Panel2.SuspendLayout()
        Me.SplitContainerMain.SuspendLayout()
        CType(Me.SplitContainerLeft, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerLeft.Panel1.SuspendLayout()
        Me.SplitContainerLeft.Panel2.SuspendLayout()
        Me.SplitContainerLeft.SuspendLayout()
        Me.GroupBoxHistory.SuspendLayout()
        CType(Me.dgvHistory, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelHistoryFilter.SuspendLayout()
        Me.GroupBoxSummary.SuspendLayout()
        CType(Me.dgvSummary, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainerRight, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerRight.Panel1.SuspendLayout()
        Me.SplitContainerRight.Panel2.SuspendLayout()
        Me.SplitContainerRight.SuspendLayout()
        Me.GroupBoxBuildings.SuspendLayout()
        CType(Me.dgvBuildings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBoxLevels.SuspendLayout()
        CType(Me.dgvLevels, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainerMain
        '
        Me.SplitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerMain.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainerMain.Name = "SplitContainerMain"
        '
        'SplitContainerMain.Panel1
        '
        Me.SplitContainerMain.Panel1.Controls.Add(Me.SplitContainerLeft)
        '
        'SplitContainerMain.Panel2
        '
        Me.SplitContainerMain.Panel2.Controls.Add(Me.SplitContainerRight)
        Me.SplitContainerMain.Size = New System.Drawing.Size(1200, 650)
        Me.SplitContainerMain.SplitterDistance = 500
        Me.SplitContainerMain.TabIndex = 0
        '
        'SplitContainerLeft
        '
        Me.SplitContainerLeft.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerLeft.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainerLeft.Name = "SplitContainerLeft"
        Me.SplitContainerLeft.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainerLeft.Panel1
        '
        Me.SplitContainerLeft.Panel1.Controls.Add(Me.GroupBoxHistory)
        '
        'SplitContainerLeft.Panel2
        '
        Me.SplitContainerLeft.Panel2.Controls.Add(Me.GroupBoxSummary)
        Me.SplitContainerLeft.Size = New System.Drawing.Size(500, 650)
        Me.SplitContainerLeft.SplitterDistance = 350
        Me.SplitContainerLeft.TabIndex = 0
        '
        'GroupBoxHistory
        '
        Me.GroupBoxHistory.Controls.Add(Me.dgvHistory)
        Me.GroupBoxHistory.Controls.Add(Me.PanelHistoryFilter)
        Me.GroupBoxHistory.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBoxHistory.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.GroupBoxHistory.Location = New System.Drawing.Point(0, 0)
        Me.GroupBoxHistory.Name = "GroupBoxHistory"
        Me.GroupBoxHistory.Size = New System.Drawing.Size(500, 350)
        Me.GroupBoxHistory.TabIndex = 0
        Me.GroupBoxHistory.TabStop = False
        Me.GroupBoxHistory.Text = "Price History Snapshots"
        '
        'dgvHistory
        '
        Me.dgvHistory.AllowUserToAddRows = False
        Me.dgvHistory.AllowUserToDeleteRows = False
        Me.dgvHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvHistory.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvHistory.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvHistory.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.dgvHistory.Location = New System.Drawing.Point(3, 58)
        Me.dgvHistory.MultiSelect = False
        Me.dgvHistory.Name = "dgvHistory"
        Me.dgvHistory.ReadOnly = True
        Me.dgvHistory.RowHeadersVisible = False
        Me.dgvHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvHistory.Size = New System.Drawing.Size(494, 289)
        Me.dgvHistory.TabIndex = 1
        '
        'PanelHistoryFilter
        '
        Me.PanelHistoryFilter.Controls.Add(Me.btnRefresh)
        Me.PanelHistoryFilter.Controls.Add(Me.cboVersionFilter)
        Me.PanelHistoryFilter.Controls.Add(Me.lblVersionFilter)
        Me.PanelHistoryFilter.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelHistoryFilter.Location = New System.Drawing.Point(3, 19)
        Me.PanelHistoryFilter.Name = "PanelHistoryFilter"
        Me.PanelHistoryFilter.Size = New System.Drawing.Size(494, 39)
        Me.PanelHistoryFilter.TabIndex = 0
        '
        'btnRefresh
        '
        Me.btnRefresh.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRefresh.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.btnRefresh.Location = New System.Drawing.Point(411, 6)
        Me.btnRefresh.Name = "btnRefresh"
        Me.btnRefresh.Size = New System.Drawing.Size(75, 27)
        Me.btnRefresh.TabIndex = 2
        Me.btnRefresh.Text = "Refresh"
        Me.btnRefresh.UseVisualStyleBackColor = True
        '
        'cboVersionFilter
        '
        Me.cboVersionFilter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboVersionFilter.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.cboVersionFilter.FormattingEnabled = True
        Me.cboVersionFilter.Location = New System.Drawing.Point(60, 8)
        Me.cboVersionFilter.Name = "cboVersionFilter"
        Me.cboVersionFilter.Size = New System.Drawing.Size(200, 23)
        Me.cboVersionFilter.TabIndex = 1
        '
        'lblVersionFilter
        '
        Me.lblVersionFilter.AutoSize = True
        Me.lblVersionFilter.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.lblVersionFilter.Location = New System.Drawing.Point(8, 11)
        Me.lblVersionFilter.Name = "lblVersionFilter"
        Me.lblVersionFilter.Size = New System.Drawing.Size(48, 15)
        Me.lblVersionFilter.TabIndex = 0
        Me.lblVersionFilter.Text = "Version:"
        '
        'GroupBoxSummary
        '
        Me.GroupBoxSummary.Controls.Add(Me.dgvSummary)
        Me.GroupBoxSummary.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBoxSummary.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.GroupBoxSummary.Location = New System.Drawing.Point(0, 0)
        Me.GroupBoxSummary.Name = "GroupBoxSummary"
        Me.GroupBoxSummary.Size = New System.Drawing.Size(500, 296)
        Me.GroupBoxSummary.TabIndex = 0
        Me.GroupBoxSummary.TabStop = False
        Me.GroupBoxSummary.Text = "Snapshot Summary"
        '
        'dgvSummary
        '
        Me.dgvSummary.AllowUserToAddRows = False
        Me.dgvSummary.AllowUserToDeleteRows = False
        Me.dgvSummary.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvSummary.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvSummary.ColumnHeadersVisible = False
        Me.dgvSummary.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvSummary.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.dgvSummary.Location = New System.Drawing.Point(3, 19)
        Me.dgvSummary.Name = "dgvSummary"
        Me.dgvSummary.ReadOnly = True
        Me.dgvSummary.RowHeadersVisible = False
        Me.dgvSummary.Size = New System.Drawing.Size(494, 274)
        Me.dgvSummary.TabIndex = 0
        '
        'SplitContainerRight
        '
        Me.SplitContainerRight.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerRight.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainerRight.Name = "SplitContainerRight"
        Me.SplitContainerRight.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainerRight.Panel1
        '
        Me.SplitContainerRight.Panel1.Controls.Add(Me.GroupBoxBuildings)
        '
        'SplitContainerRight.Panel2
        '
        Me.SplitContainerRight.Panel2.Controls.Add(Me.GroupBoxLevels)
        Me.SplitContainerRight.Size = New System.Drawing.Size(696, 650)
        Me.SplitContainerRight.SplitterDistance = 300
        Me.SplitContainerRight.TabIndex = 0
        '
        'GroupBoxBuildings
        '
        Me.GroupBoxBuildings.Controls.Add(Me.dgvBuildings)
        Me.GroupBoxBuildings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBoxBuildings.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.GroupBoxBuildings.Location = New System.Drawing.Point(0, 0)
        Me.GroupBoxBuildings.Name = "GroupBoxBuildings"
        Me.GroupBoxBuildings.Size = New System.Drawing.Size(696, 300)
        Me.GroupBoxBuildings.TabIndex = 0
        Me.GroupBoxBuildings.TabStop = False
        Me.GroupBoxBuildings.Text = "Building Detail"
        '
        'dgvBuildings
        '
        Me.dgvBuildings.AllowUserToAddRows = False
        Me.dgvBuildings.AllowUserToDeleteRows = False
        Me.dgvBuildings.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvBuildings.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvBuildings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvBuildings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvBuildings.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.dgvBuildings.Location = New System.Drawing.Point(3, 19)
        Me.dgvBuildings.MultiSelect = False
        Me.dgvBuildings.Name = "dgvBuildings"
        Me.dgvBuildings.ReadOnly = True
        Me.dgvBuildings.RowHeadersVisible = False
        Me.dgvBuildings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvBuildings.Size = New System.Drawing.Size(690, 278)
        Me.dgvBuildings.TabIndex = 0
        '
        'GroupBoxLevels
        '
        Me.GroupBoxLevels.Controls.Add(Me.dgvLevels)
        Me.GroupBoxLevels.Dock = System.Windows.Forms.DockStyle.Fill
        Me.GroupBoxLevels.Font = New System.Drawing.Font("Segoe UI Semibold", 9.0!, System.Drawing.FontStyle.Bold)
        Me.GroupBoxLevels.Location = New System.Drawing.Point(0, 0)
        Me.GroupBoxLevels.Name = "GroupBoxLevels"
        Me.GroupBoxLevels.Size = New System.Drawing.Size(696, 346)
        Me.GroupBoxLevels.TabIndex = 0
        Me.GroupBoxLevels.TabStop = False
        Me.GroupBoxLevels.Text = "Level Detail"
        '
        'dgvLevels
        '
        Me.dgvLevels.AllowUserToAddRows = False
        Me.dgvLevels.AllowUserToDeleteRows = False
        Me.dgvLevels.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvLevels.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvLevels.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvLevels.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvLevels.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.dgvLevels.Location = New System.Drawing.Point(3, 19)
        Me.dgvLevels.Name = "dgvLevels"
        Me.dgvLevels.ReadOnly = True
        Me.dgvLevels.RowHeadersVisible = False
        Me.dgvLevels.Size = New System.Drawing.Size(690, 324)
        Me.dgvLevels.TabIndex = 0
        '
        'PanelBottom
        '
        Me.PanelBottom.Controls.Add(Me.lblStatus)
        Me.PanelBottom.Controls.Add(Me.btnExport)
        Me.PanelBottom.Controls.Add(Me.btnCompare)
        Me.PanelBottom.Controls.Add(Me.btnDelete)
        Me.PanelBottom.Controls.Add(Me.btnClose)
        Me.PanelBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.PanelBottom.Location = New System.Drawing.Point(0, 650)
        Me.PanelBottom.Name = "PanelBottom"
        Me.PanelBottom.Size = New System.Drawing.Size(1200, 50)
        Me.PanelBottom.TabIndex = 1
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.Location = New System.Drawing.Point(112, 17)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 15)
        Me.lblStatus.TabIndex = 4
        '
        'btnExport
        '
        Me.btnExport.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExport.Location = New System.Drawing.Point(912, 12)
        Me.btnExport.Name = "btnExport"
        Me.btnExport.Size = New System.Drawing.Size(85, 27)
        Me.btnExport.TabIndex = 3
        Me.btnExport.Text = "Export..."
        Me.btnExport.UseVisualStyleBackColor = True
        '
        'btnCompare
        '
        Me.btnCompare.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCompare.Location = New System.Drawing.Point(1003, 12)
        Me.btnCompare.Name = "btnCompare"
        Me.btnCompare.Size = New System.Drawing.Size(85, 27)
        Me.btnCompare.TabIndex = 2
        Me.btnCompare.Text = "Compare..."
        Me.btnCompare.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.Location = New System.Drawing.Point(12, 12)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(85, 27)
        Me.btnDelete.TabIndex = 1
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(1094, 12)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(85, 27)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'frmPriceHistoryViewer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1200, 700)
        Me.Controls.Add(Me.SplitContainerMain)
        Me.Controls.Add(Me.PanelBottom)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.MinimumSize = New System.Drawing.Size(900, 600)
        Me.Name = "frmPriceHistoryViewer"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Price History Viewer"
        Me.SplitContainerMain.Panel1.ResumeLayout(False)
        Me.SplitContainerMain.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerMain.ResumeLayout(False)
        Me.SplitContainerLeft.Panel1.ResumeLayout(False)
        Me.SplitContainerLeft.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerLeft, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerLeft.ResumeLayout(False)
        Me.GroupBoxHistory.ResumeLayout(False)
        CType(Me.dgvHistory, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelHistoryFilter.ResumeLayout(False)
        Me.PanelHistoryFilter.PerformLayout()
        Me.GroupBoxSummary.ResumeLayout(False)
        CType(Me.dgvSummary, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerRight.Panel1.ResumeLayout(False)
        Me.SplitContainerRight.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerRight, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerRight.ResumeLayout(False)
        Me.GroupBoxBuildings.ResumeLayout(False)
        CType(Me.dgvBuildings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBoxLevels.ResumeLayout(False)
        CType(Me.dgvLevels, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelBottom.ResumeLayout(False)
        Me.PanelBottom.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainerMain As SplitContainer
    Friend WithEvents SplitContainerLeft As SplitContainer
    Friend WithEvents GroupBoxHistory As GroupBox
    Friend WithEvents dgvHistory As DataGridView
    Friend WithEvents PanelHistoryFilter As Panel
    Friend WithEvents btnRefresh As Button
    Friend WithEvents cboVersionFilter As ComboBox
    Friend WithEvents lblVersionFilter As Label
    Friend WithEvents GroupBoxSummary As GroupBox
    Friend WithEvents dgvSummary As DataGridView
    Friend WithEvents SplitContainerRight As SplitContainer
    Friend WithEvents GroupBoxBuildings As GroupBox
    Friend WithEvents dgvBuildings As DataGridView
    Friend WithEvents GroupBoxLevels As GroupBox
    Friend WithEvents dgvLevels As DataGridView
    Friend WithEvents PanelBottom As Panel
    Friend WithEvents btnExport As Button
    Friend WithEvents btnCompare As Button
    Friend WithEvents btnDelete As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents lblStatus As Label

End Class
