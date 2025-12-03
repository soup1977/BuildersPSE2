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
        Me.MenuStrip = New System.Windows.Forms.MenuStrip()
        Me.FileMenu = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.StatusStrip = New System.Windows.Forms.StatusStrip()
        Me.ToolStripStatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tsbStatusHistory = New System.Windows.Forms.ToolStripDropDownButton()
        Me.cmsStatusHistory = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ToolTip = New System.Windows.Forms.ToolTip(Me.components)
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.btnImportCSV = New System.Windows.Forms.Button()
        Me.btnEditLumber = New System.Windows.Forms.Button()
        Me.btnMondayList = New System.Windows.Forms.Button()
        Me.btnImportPSE = New System.Windows.Forms.Button()
        Me.btnCreateProject = New System.Windows.Forms.Button()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.ToolsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditLumberToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditConfigurationToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditIEPulldownsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CreateProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportMGMTProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ImportPSEProjectToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.MenuStrip.SuspendLayout()
        Me.StatusStrip.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'MenuStrip
        '
        Me.MenuStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.FileMenu, Me.ToolsToolStripMenuItem})
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
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
        Me.ExitToolStripMenuItem.Text = "E&xit"
        '
        'StatusStrip
        '
        Me.StatusStrip.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabel, Me.tsbStatusHistory})
        Me.StatusStrip.Location = New System.Drawing.Point(0, 689)
        Me.StatusStrip.Name = "StatusStrip"
        Me.StatusStrip.Size = New System.Drawing.Size(1384, 22)
        Me.StatusStrip.TabIndex = 7
        '
        'ToolStripStatusLabel
        '
        Me.ToolStripStatusLabel.Name = "ToolStripStatusLabel"
        Me.ToolStripStatusLabel.Size = New System.Drawing.Size(1311, 17)
        Me.ToolStripStatusLabel.Spring = True
        Me.ToolStripStatusLabel.Text = "Ready"
        Me.ToolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tsbStatusHistory
        '
        Me.tsbStatusHistory.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text
        Me.tsbStatusHistory.DropDown = Me.cmsStatusHistory
        Me.tsbStatusHistory.Name = "tsbStatusHistory"
        Me.tsbStatusHistory.Size = New System.Drawing.Size(58, 20)
        Me.tsbStatusHistory.Text = "History"
        '
        'cmsStatusHistory
        '
        Me.cmsStatusHistory.Name = "cmsStatusHistory"
        Me.cmsStatusHistory.OwnerItem = Me.tsbStatusHistory
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
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnImportCSV)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnEditLumber)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnMondayList)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnImportPSE)
        Me.SplitContainer1.Panel1.Controls.Add(Me.btnCreateProject)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.TabControl1)
        Me.SplitContainer1.Size = New System.Drawing.Size(1384, 665)
        Me.SplitContainer1.SplitterDistance = 157
        Me.SplitContainer1.TabIndex = 8
        '
        'btnImportCSV
        '
        Me.btnImportCSV.Location = New System.Drawing.Point(12, 139)
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
        Me.btnMondayList.Location = New System.Drawing.Point(12, 224)
        Me.btnMondayList.Name = "btnMondayList"
        Me.btnMondayList.Size = New System.Drawing.Size(127, 31)
        Me.btnMondayList.TabIndex = 2
        Me.btnMondayList.Text = "Monday List"
        '
        'btnImportPSE
        '
        Me.btnImportPSE.Location = New System.Drawing.Point(12, 176)
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
        'TabControl1
        '
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(0, 0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1223, 665)
        Me.TabControl1.TabIndex = 0
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
        Me.EditLumberToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.EditLumberToolStripMenuItem.Text = "Edit Lumber"
        '
        'EditConfigurationToolStripMenuItem
        '
        Me.EditConfigurationToolStripMenuItem.Name = "EditConfigurationToolStripMenuItem"
        Me.EditConfigurationToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.EditConfigurationToolStripMenuItem.Text = "Edit Configuration"
        '
        'EditIEPulldownsToolStripMenuItem
        '
        Me.EditIEPulldownsToolStripMenuItem.Name = "EditIEPulldownsToolStripMenuItem"
        Me.EditIEPulldownsToolStripMenuItem.Size = New System.Drawing.Size(180, 22)
        Me.EditIEPulldownsToolStripMenuItem.Text = "Edit IE Pulldowns"
        '
        'CreateProjectToolStripMenuItem
        '
        Me.CreateProjectToolStripMenuItem.Name = "CreateProjectToolStripMenuItem"
        Me.CreateProjectToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
        Me.CreateProjectToolStripMenuItem.Text = "Create Project"
        '
        'ImportMGMTProjectToolStripMenuItem
        '
        Me.ImportMGMTProjectToolStripMenuItem.Name = "ImportMGMTProjectToolStripMenuItem"
        Me.ImportMGMTProjectToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
        Me.ImportMGMTProjectToolStripMenuItem.Text = "Import MGMT Project"
        '
        'ImportPSEProjectToolStripMenuItem
        '
        Me.ImportPSEProjectToolStripMenuItem.Name = "ImportPSEProjectToolStripMenuItem"
        Me.ImportPSEProjectToolStripMenuItem.Size = New System.Drawing.Size(190, 22)
        Me.ImportPSEProjectToolStripMenuItem.Text = "Import PSE Project"
        '
        'frmMain
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1384, 711)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusStrip)
        Me.Controls.Add(Me.MenuStrip)
        Me.MainMenuStrip = Me.MenuStrip
        Me.Name = "frmMain"
        Me.Text = "Builders PSE"
        Me.MenuStrip.ResumeLayout(False)
        Me.MenuStrip.PerformLayout()
        Me.StatusStrip.ResumeLayout(False)
        Me.StatusStrip.PerformLayout()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents MenuStrip As MenuStrip
    Friend WithEvents FileMenu As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents StatusStrip As StatusStrip
    Friend WithEvents ToolStripStatusLabel As ToolStripStatusLabel
    Friend WithEvents tsbStatusHistory As ToolStripDropDownButton
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
End Class
