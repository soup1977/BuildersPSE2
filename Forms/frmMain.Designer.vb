<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMain
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(frmMain))
        Me.MenuStrip = New System.Windows.Forms.MenuStrip()
        Me.FileMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.CreateProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportPSEProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportMGMTProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditLumberToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditConfigurationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditIEPulldownsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ViewToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToggleActivityLogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearActivityLogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.ttslUserName = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.cmsStatusHistory = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.btnPriceLockAdmin = New System.Windows.Forms.Button()
        Me.btnPriceLock = New System.Windows.Forms.Button()
        Me.btnToggleLog = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnImportCSV = New System.Windows.Forms.Button()
        Me.btnEditLumber = New System.Windows.Forms.Button()
        Me.btnMondayList = New System.Windows.Forms.Button()
        Me.btnImportPSE = New System.Windows.Forms.Button()
        Me.btnCreateProject = New System.Windows.Forms.Button()
        Me.SplitContainerMain = New System.Windows.Forms.SplitContainer()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.lstActivityLog = New System.Windows.Forms.ListBox()
        Me.PanelLogHeader = New System.Windows.Forms.Panel()
        Me.btnClearLog = New System.Windows.Forms.Button()
        Me.lblLogHeader = New System.Windows.Forms.Label()
        Me.MenuStrip.SuspendLayout()
        Me.StatusStrip.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.SplitContainerMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainerMain.Panel1.SuspendLayout()
        Me.SplitContainerMain.Panel2.SuspendLayout()
        Me.SplitContainerMain.SuspendLayout()
        Me.PanelLogHeader.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip
        '
        Me.MenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileMenu, Me.ToolsToolStripMenuItem, Me.ViewToolStripMenuItem})
        Me.MenuStrip.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip.Name = "MenuStrip"
        Me.MenuStrip.Size = New System.Drawing.Size(1384, 24)
        Me.MenuStrip.TabIndex = 5
        '
        'FileMenu
        '
        Me.FileMenu.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CreateProjectToolStripMenuItem, Me.ImportPSEProjectToolStripMenuItem, Me.ImportMGMTProjectToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.FileMenu.Name = "FileMenu"
        Me.FileMenu.Size = New System.Drawing.Size(37, 20)
        Me.FileMenu.Text = "&File"
        '
        'CreateProjectToolStripMenuItem
        '
        Me.CreateProjectToolStripMenuItem.Name = "CreateProjectToolStripMenuItem"
        Me.CreateProjectToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
        Me.CreateProjectToolStripMenuItem.Text = "Create Project"
        '
        'ImportPSEProjectToolStripMenuItem
        '
        Me.ImportPSEProjectToolStripMenuItem.Name = "ImportPSEProjectToolStripMenuItem"
        Me.ImportPSEProjectToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
        Me.ImportPSEProjectToolStripMenuItem.Text = "Import PSE Project"
        '
        'ImportMGMTProjectToolStripMenuItem
        '
        Me.ImportMGMTProjectToolStripMenuItem.Name = "ImportMGMTProjectToolStripMenuItem"
        Me.ImportMGMTProjectToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
        Me.ImportMGMTProjectToolStripMenuItem.Text = "Import MGMT Project"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'ToolsToolStripMenuItem
        '
        Me.ToolsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EditLumberToolStripMenuItem, Me.EditConfigurationToolStripMenuItem, Me.EditIEPulldownsToolStripMenuItem})
        Me.ToolsToolStripMenuItem.Name = "ToolsToolStripMenuItem"
        Me.ToolsToolStripMenuItem.Size = New System.Drawing.Size(47, 20)
        Me.ToolsToolStripMenuItem.Text = "Tools"
        '
        'EditLumberToolStripMenuItem
        '
        Me.EditLumberToolStripMenuItem.Name = "EditLumberToolStripMenuItem"
        Me.EditLumberToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.EditLumberToolStripMenuItem.Text = "Edit Lumber"
        '
        'EditConfigurationToolStripMenuItem
        '
        Me.EditConfigurationToolStripMenuItem.Name = "EditConfigurationToolStripMenuItem"
        Me.EditConfigurationToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.EditConfigurationToolStripMenuItem.Text = "Edit Configuration"
        '
        'EditIEPulldownsToolStripMenuItem
        '
        Me.EditIEPulldownsToolStripMenuItem.Name = "EditIEPulldownsToolStripMenuItem"
        Me.EditIEPulldownsToolStripMenuItem.Size = New System.Drawing.Size(171, 22)
        Me.EditIEPulldownsToolStripMenuItem.Text = "Edit IE Pulldowns"
        '
        'ViewToolStripMenuItem
        '
        Me.ViewToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToggleActivityLogToolStripMenuItem, Me.ClearActivityLogToolStripMenuItem})
        Me.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem"
        Me.ViewToolStripMenuItem.Size = New System.Drawing.Size(44, 20)
        Me.ViewToolStripMenuItem.Text = "&View"
        '
        'ToggleActivityLogToolStripMenuItem
        '
        Me.ToggleActivityLogToolStripMenuItem.Name = "ToggleActivityLogToolStripMenuItem"
        Me.ToggleActivityLogToolStripMenuItem.ShortcutKeys = CType((System.Windows.Forms.Keys.Control Or System.Windows.Forms.Keys.L), System.Windows.Forms.Keys)
        Me.ToggleActivityLogToolStripMenuItem.Size = New System.Drawing.Size(216, 22)
        Me.ToggleActivityLogToolStripMenuItem.Text = "Toggle Activity Log"
        '
        'ClearActivityLogToolStripMenuItem
        '
        Me.ClearActivityLogToolStripMenuItem.Name = "ClearActivityLogToolStripMenuItem"
        Me.ClearActivityLogToolStripMenuItem.Size = New System.Drawing.Size(216, 22)
        Me.ClearActivityLogToolStripMenuItem.Text = "Clear Activity Log"
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ttslUserName, Me.ToolStripStatusLabel})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 687)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(1384, 24)
        Me.StatusStrip.TabIndex = 7
        '
        'ttslUserName
        '
        Me.ttslUserName.BorderSides = CType((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) _
            Or System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom), System.Windows.Forms.ToolStripStatusLabelBorderSides)
        Me.ttslUserName.Name = "ttslUserName"
        Me.ttslUserName.Size = New System.Drawing.Size(20, 19)
        Me.ttslUserName.Text = "..."
        '
        'ToolStripStatusLabel
        '
        Me.ToolStripStatusLabel.Name = "ToolStripStatusLabel"
        Me.ToolStripStatusLabel.Size = New System.Drawing.Size(1349, 19)
        Me.ToolStripStatusLabel.Spring = True
        Me.ToolStripStatusLabel.Text = "Ready"
        Me.ToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.ToolStripStatusLabel.Visible = False
        '
        'cmsStatusHistory
        '
        Me.cmsStatusHistory.Name = "cmsStatusHistory"
        Me.cmsStatusHistory.Size = New System.Drawing.Size(61, 4)
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 24)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnPriceLockAdmin)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnPriceLock)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnToggleLog)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnClose)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnImportCSV)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnEditLumber)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnMondayList)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnImportPSE)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnCreateProject)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.SplitContainerMain)
        Me.SplitContainer1.Size = New System.Drawing.Size(1384, 663)
        Me.SplitContainer1.SplitterDistance = 147
        Me.SplitContainer1.TabIndex = 8
        '
        'btnPriceLockAdmin
        '
        Me.btnPriceLockAdmin.Location = New System.Drawing.Point(12, 463)
        Me.btnPriceLockAdmin.Name = "btnPriceLockAdmin"
        Me.btnPriceLockAdmin.Size = New System.Drawing.Size(127, 30)
        Me.btnPriceLockAdmin.TabIndex = 10
        Me.btnPriceLockAdmin.Text = "Price Lock Admin"
        Me.btnPriceLockAdmin.UseVisualStyleBackColor = True
        '
        'btnPriceLock
        '
        Me.btnPriceLock.Location = New System.Drawing.Point(12, 427)
        Me.btnPriceLock.Name = "btnPriceLock"
        Me.btnPriceLock.Size = New System.Drawing.Size(127, 30)
        Me.btnPriceLock.TabIndex = 9
        Me.btnPriceLock.Text = "Price Locks"
        Me.btnPriceLock.UseVisualStyleBackColor = True
        '
        'btnToggleLog
        '
        Me.btnToggleLog.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnToggleLog.Location = New System.Drawing.Point(12, 596)
        Me.btnToggleLog.Name = "btnToggleLog"
        Me.btnToggleLog.Size = New System.Drawing.Size(127, 27)
        Me.btnToggleLog.TabIndex = 8
        Me.btnToggleLog.Text = "Show Log"
        Me.btnToggleLog.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(12, 317)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(127, 27)
        Me.btnClose.TabIndex = 7
        Me.btnClose.Text = "Exit Program"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnImportCSV
        '
        Me.btnImportCSV.Location = New System.Drawing.Point(12, 176)
        Me.btnImportCSV.Name = "btnImportCSV"
        Me.btnImportCSV.Size = New System.Drawing.Size(127, 31)
        Me.btnImportCSV.TabIndex = 0
        Me.btnImportCSV.Text = "Import MGMT Project"
        '
        'btnEditLumber
        '
        Me.btnEditLumber.Location = New System.Drawing.Point(12, 261)
        Me.btnEditLumber.Name = "btnEditLumber"
        Me.btnEditLumber.Size = New System.Drawing.Size(127, 31)
        Me.btnEditLumber.TabIndex = 1
        Me.btnEditLumber.Text = "Edit Lumber"
        '
        'btnMondayList
        '
        Me.btnMondayList.Location = New System.Drawing.Point(12, 499)
        Me.btnMondayList.Name = "btnMondayList"
        Me.btnMondayList.Size = New System.Drawing.Size(127, 31)
        Me.btnMondayList.TabIndex = 2
        Me.btnMondayList.Text = "Monday List"
        Me.btnMondayList.Visible = False
        '
        'btnImportPSE
        '
        Me.btnImportPSE.Location = New System.Drawing.Point(12, 139)
        Me.btnImportPSE.Name = "btnImportPSE"
        Me.btnImportPSE.Size = New System.Drawing.Size(127, 31)
        Me.btnImportPSE.TabIndex = 3
        Me.btnImportPSE.Text = "Import PSE"
        '
        'btnCreateProject
        '
        Me.btnCreateProject.Location = New System.Drawing.Point(12, 102)
        Me.btnCreateProject.Name = "btnCreateProject"
        Me.btnCreateProject.Size = New System.Drawing.Size(127, 31)
        Me.btnCreateProject.TabIndex = 4
        Me.btnCreateProject.Text = "Create Project"
        '
        'SplitContainerMain
        '
        Me.SplitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainerMain.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainerMain.Name = "SplitContainerMain"
        Me.SplitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainerMain.Panel1
        '
        Me.SplitContainerMain.Panel1.Controls.Add(Me.TabControl1)
        Me.SplitContainerMain.Panel1MinSize = 200
        '
        'SplitContainerMain.Panel2
        '
        Me.SplitContainerMain.Panel2.Controls.Add(Me.lstActivityLog)
        Me.SplitContainerMain.Panel2.Controls.Add(Me.PanelLogHeader)
        Me.SplitContainerMain.Panel2Collapsed = True
        Me.SplitContainerMain.Panel2MinSize = 100
        Me.SplitContainerMain.Size = New System.Drawing.Size(1233, 663)
        Me.SplitContainerMain.SplitterDistance = 200
        Me.SplitContainerMain.TabIndex = 1
        '
        'TabControl1
        '
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1233, 663)
        Me.TabControl1.TabIndex = 0
        '
        'lstActivityLog
        '
        Me.lstActivityLog.BackColor = System.Drawing.Color.FromArgb(CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer), CType(CType(30, Byte), Integer))
        Me.lstActivityLog.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.lstActivityLog.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lstActivityLog.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lstActivityLog.ForeColor = System.Drawing.Color.LightGray
        Me.lstActivityLog.FormattingEnabled = True
        Me.lstActivityLog.ItemHeight = 14
        Me.lstActivityLog.Location = New System.Drawing.Point(0, 24)
        Me.lstActivityLog.Name = "lstActivityLog"
        Me.lstActivityLog.Size = New System.Drawing.Size(150, 22)
        Me.lstActivityLog.TabIndex = 0
        '
        'PanelLogHeader
        '
        Me.PanelLogHeader.BackColor = System.Drawing.Color.FromArgb(CType(CType(45, Byte), Integer), CType(CType(45, Byte), Integer), CType(CType(48, Byte), Integer))
        Me.PanelLogHeader.Controls.Add(Me.btnClearLog)
        Me.PanelLogHeader.Controls.Add(Me.lblLogHeader)
        Me.PanelLogHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.PanelLogHeader.Location = New System.Drawing.Point(0, 0)
        Me.PanelLogHeader.Name = "PanelLogHeader"
        Me.PanelLogHeader.Size = New System.Drawing.Size(150, 24)
        Me.PanelLogHeader.TabIndex = 1
        '
        'btnClearLog
        '
        Me.btnClearLog.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClearLog.FlatAppearance.BorderSize = 0
        Me.btnClearLog.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.btnClearLog.ForeColor = System.Drawing.Color.White
        Me.btnClearLog.Location = New System.Drawing.Point(100, 1)
        Me.btnClearLog.Name = "btnClearLog"
        Me.btnClearLog.Size = New System.Drawing.Size(47, 22)
        Me.btnClearLog.TabIndex = 1
        Me.btnClearLog.Text = "Clear"
        Me.btnClearLog.UseVisualStyleBackColor = True
        '
        'lblLogHeader
        '
        Me.lblLogHeader.AutoSize = True
        Me.lblLogHeader.ForeColor = System.Drawing.Color.White
        Me.lblLogHeader.Location = New System.Drawing.Point(6, 5)
        Me.lblLogHeader.Name = "lblLogHeader"
        Me.lblLogHeader.Size = New System.Drawing.Size(62, 13)
        Me.lblLogHeader.TabIndex = 0
        Me.lblLogHeader.Text = "Activity Log"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1384, 711)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.MenuStrip)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip
        Me.Name = "frmMain"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Builders PSE"
        Me.MenuStrip.ResumeLayout(False)
        Me.MenuStrip.PerformLayout()
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.SplitContainerMain.Panel1.ResumeLayout(False)
        Me.SplitContainerMain.Panel2.ResumeLayout(False)
        CType(Me.SplitContainerMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainerMain.ResumeLayout(False)
        Me.PanelLogHeader.ResumeLayout(False)
        Me.PanelLogHeader.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip As MenuStrip
    Friend WithEvents FileMenu As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StatusStrip As StatusStrip
    Friend WithEvents ToolStripStatusLabel As ToolStripStatusLabel
    Friend WithEvents cmsStatusHistory As ContextMenuStrip
    Friend WithEvents StatusHistoryListBox As ListBox
    Friend WithEvents ToolTip As ToolTip
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents btnImportCSV As Button
    Friend WithEvents btnEditLumber As Button
    Friend WithEvents btnMondayList As Button
    Friend WithEvents btnImportPSE As Button
    Friend WithEvents btnCreateProject As Button
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents CreateProjectToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ImportPSEProjectToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ImportMGMTProjectToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditLumberToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditConfigurationToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditIEPulldownsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ttslUserName As ToolStripStatusLabel
    Friend WithEvents btnClose As Button
    Friend WithEvents SplitContainerMain As SplitContainer
    Friend WithEvents lstActivityLog As ListBox
    Friend WithEvents PanelLogHeader As Panel
    Friend WithEvents lblLogHeader As Label
    Friend WithEvents btnClearLog As Button
    Friend WithEvents btnToggleLog As Button
    Friend WithEvents ViewToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToggleActivityLogToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ClearActivityLogToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents btnPriceLock As Button
    Friend WithEvents btnPriceLockAdmin As Button
End Class