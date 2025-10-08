<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmCreateEditProject
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
        Me.components = New System.ComponentModel.Container()
        Me.tvProjectTree = New System.Windows.Forms.TreeView()
        Me.cmsTreeMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuAddBuilding = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuAddLevel = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditPSEToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuCopyLevels = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuPasteLevels = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuCopyBuilding = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuPasteBuilding = New System.Windows.Forms.ToolStripMenuItem()
        Me.tabControlRight = New System.Windows.Forms.TabControl()
        Me.tabProjectInfo = New System.Windows.Forms.TabPage()
        Me.btnGenerateProjectReport = New System.Windows.Forms.Button()
        Me.btnDeleteProject = New System.Windows.Forms.Button()
        Me.btnIEOpen = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.btnOpenPSE = New System.Windows.Forms.Button()
        Me.pnlProjectInfo = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnEditEngineer = New System.Windows.Forms.Button()
        Me.btnAddEngineer = New System.Windows.Forms.Button()
        Me.btnEditArchitect = New System.Windows.Forms.Button()
        Me.btnAddArchitect = New System.Windows.Forms.Button()
        Me.btnEditCustomer = New System.Windows.Forms.Button()
        Me.btnAddCustomer = New System.Windows.Forms.Button()
        Me.cboProjectEngineer = New System.Windows.Forms.ComboBox()
        Me.cboProjectArchitect = New System.Windows.Forms.ComboBox()
        Me.btnDuplicateVersion = New System.Windows.Forms.Button()
        Me.btnCreateVersion = New System.Windows.Forms.Button()
        Me.cboVersion = New System.Windows.Forms.ComboBox()
        Me.cboState = New System.Windows.Forms.ComboBox()
        Me.cboEstimator = New System.Windows.Forms.ComboBox()
        Me.cboProjectType = New System.Windows.Forms.ComboBox()
        Me.cboSalesman = New System.Windows.Forms.ComboBox()
        Me.cboCustomer = New System.Windows.Forms.ComboBox()
        Me.dtpLastModified = New System.Windows.Forms.DateTimePicker()
        Me.lblJBID = New System.Windows.Forms.Label()
        Me.txtJBID = New System.Windows.Forms.TextBox()
        Me.lblProjectType = New System.Windows.Forms.Label()
        Me.lblSalesman = New System.Windows.Forms.Label()
        Me.lblProjectName = New System.Windows.Forms.Label()
        Me.txtProjectName = New System.Windows.Forms.TextBox()
        Me.lblEstimator = New System.Windows.Forms.Label()
        Me.lblAddress = New System.Windows.Forms.Label()
        Me.txtAddress = New System.Windows.Forms.TextBox()
        Me.lblCity = New System.Windows.Forms.Label()
        Me.txtCity = New System.Windows.Forms.TextBox()
        Me.lblState = New System.Windows.Forms.Label()
        Me.lblZip = New System.Windows.Forms.Label()
        Me.txtZip = New System.Windows.Forms.TextBox()
        Me.lblBidDate = New System.Windows.Forms.Label()
        Me.dtpBidDate = New System.Windows.Forms.DateTimePicker()
        Me.lblArchPlansDated = New System.Windows.Forms.Label()
        Me.dtpArchPlansDated = New System.Windows.Forms.DateTimePicker()
        Me.lblEngPlansDated = New System.Windows.Forms.Label()
        Me.dtpEngPlansDated = New System.Windows.Forms.DateTimePicker()
        Me.lblCustomerName = New System.Windows.Forms.Label()
        Me.lblMilesToJobSite = New System.Windows.Forms.Label()
        Me.nudMilesToJobSite = New System.Windows.Forms.NumericUpDown()
        Me.lblTotalNetSqft = New System.Windows.Forms.Label()
        Me.nudTotalNetSqft = New System.Windows.Forms.NumericUpDown()
        Me.lblTotalGrossSqft = New System.Windows.Forms.Label()
        Me.nudTotalGrossSqft = New System.Windows.Forms.NumericUpDown()
        Me.lblProjectArchitect = New System.Windows.Forms.Label()
        Me.lblProjectEngineer = New System.Windows.Forms.Label()
        Me.lblProjectNotes = New System.Windows.Forms.Label()
        Me.txtProjectNotes = New System.Windows.Forms.TextBox()
        Me.lblVersion = New System.Windows.Forms.Label()
        Me.lblVersionDate = New System.Windows.Forms.Label()
        Me.dtpCreatedDate = New System.Windows.Forms.DateTimePicker()
        Me.btnSaveProjectInfo = New System.Windows.Forms.Button()
        Me.tabOverrides = New System.Windows.Forms.TabPage()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtRawRoofSPFPrice = New System.Windows.Forms.TextBox()
        Me.txtRawFloorSPFPrice = New System.Windows.Forms.TextBox()
        Me.dgvOverrides = New System.Windows.Forms.DataGridView()
        Me.btnSaveOverrides = New System.Windows.Forms.Button()
        Me.tabRollup = New System.Windows.Forms.TabPage()
        Me.dgvRollup = New System.Windows.Forms.DataGridView()
        Me.btnRecalcRollup = New System.Windows.Forms.Button()
        Me.tabBuildingInfo = New System.Windows.Forms.TabPage()
        Me.pnlBuildingInfo = New System.Windows.Forms.Panel()
        Me.lblPlanUnits = New System.Windows.Forms.Label()
        Me.txtResUnits = New System.Windows.Forms.TextBox()
        Me.lblBuildingName = New System.Windows.Forms.Label()
        Me.txtBuildingName = New System.Windows.Forms.TextBox()
        Me.lblBldgQty = New System.Windows.Forms.Label()
        Me.nudBldgQty = New System.Windows.Forms.NumericUpDown()
        Me.lblBuildingType = New System.Windows.Forms.Label()
        Me.txtBuildingType = New System.Windows.Forms.TextBox()
        Me.lblNbrUnits = New System.Windows.Forms.Label()
        Me.nudNbrUnits = New System.Windows.Forms.NumericUpDown()
        Me.btnSaveBuildingInfo = New System.Windows.Forms.Button()
        Me.tabLevelInfo = New System.Windows.Forms.TabPage()
        Me.pnlLevelInfo = New System.Windows.Forms.Panel()
        Me.nudLevelNumber = New System.Windows.Forms.NumericUpDown()
        Me.lblLevelName = New System.Windows.Forms.Label()
        Me.txtLevelName = New System.Windows.Forms.TextBox()
        Me.lblLevelType = New System.Windows.Forms.Label()
        Me.cmbLevelType = New System.Windows.Forms.ComboBox()
        Me.lblLevelNumber = New System.Windows.Forms.Label()
        Me.btnSaveLevelInfo = New System.Windows.Forms.Button()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.btnPreviewIncExc = New System.Windows.Forms.Button()
        Me.cmsTreeMenu.SuspendLayout()
        Me.tabControlRight.SuspendLayout()
        Me.tabProjectInfo.SuspendLayout()
        Me.pnlProjectInfo.SuspendLayout()
        CType(Me.nudMilesToJobSite, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudTotalNetSqft, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudTotalGrossSqft, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabOverrides.SuspendLayout()
        CType(Me.dgvOverrides, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabRollup.SuspendLayout()
        CType(Me.dgvRollup, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabBuildingInfo.SuspendLayout()
        Me.pnlBuildingInfo.SuspendLayout()
        CType(Me.nudBldgQty, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudNbrUnits, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabLevelInfo.SuspendLayout()
        Me.pnlLevelInfo.SuspendLayout()
        CType(Me.nudLevelNumber, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.SuspendLayout()
        '
        'tvProjectTree
        '
        Me.tvProjectTree.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.tvProjectTree.ContextMenuStrip = Me.cmsTreeMenu
        Me.tvProjectTree.HideSelection = False
        Me.tvProjectTree.Location = New System.Drawing.Point(3, 3)
        Me.tvProjectTree.Name = "tvProjectTree"
        Me.tvProjectTree.Size = New System.Drawing.Size(194, 531)
        Me.tvProjectTree.TabIndex = 0
        '
        'cmsTreeMenu
        '
        Me.cmsTreeMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuAddBuilding, Me.mnuAddLevel, Me.mnuDelete, Me.EditPSEToolStripMenuItem, Me.mnuCopyLevels, Me.mnuPasteLevels, Me.mnuCopyBuilding, Me.mnuPasteBuilding})
        Me.cmsTreeMenu.Name = "cmsTreeMenu"
        Me.cmsTreeMenu.Size = New System.Drawing.Size(150, 180)
        '
        'mnuAddBuilding
        '
        Me.mnuAddBuilding.Name = "mnuAddBuilding"
        Me.mnuAddBuilding.Size = New System.Drawing.Size(149, 22)
        Me.mnuAddBuilding.Text = "Add Building"
        '
        'mnuAddLevel
        '
        Me.mnuAddLevel.Name = "mnuAddLevel"
        Me.mnuAddLevel.Size = New System.Drawing.Size(149, 22)
        Me.mnuAddLevel.Text = "Add Level"
        '
        'mnuDelete
        '
        Me.mnuDelete.Name = "mnuDelete"
        Me.mnuDelete.Size = New System.Drawing.Size(149, 22)
        Me.mnuDelete.Text = "Delete"
        '
        'EditPSEToolStripMenuItem
        '
        Me.EditPSEToolStripMenuItem.Name = "EditPSEToolStripMenuItem"
        Me.EditPSEToolStripMenuItem.Size = New System.Drawing.Size(149, 22)
        Me.EditPSEToolStripMenuItem.Text = "Edit PSE"
        '
        'mnuCopyLevels
        '
        Me.mnuCopyLevels.Name = "mnuCopyLevels"
        Me.mnuCopyLevels.Size = New System.Drawing.Size(149, 22)
        Me.mnuCopyLevels.Text = "Copy Levels"
        '
        'mnuPasteLevels
        '
        Me.mnuPasteLevels.Name = "mnuPasteLevels"
        Me.mnuPasteLevels.Size = New System.Drawing.Size(149, 22)
        Me.mnuPasteLevels.Text = "Paste Levels"
        '
        'mnuCopyBuilding
        '
        Me.mnuCopyBuilding.Name = "mnuCopyBuilding"
        Me.mnuCopyBuilding.Size = New System.Drawing.Size(149, 22)
        Me.mnuCopyBuilding.Text = "Copy Building"
        '
        'mnuPasteBuilding
        '
        Me.mnuPasteBuilding.Name = "mnuPasteBuilding"
        Me.mnuPasteBuilding.Size = New System.Drawing.Size(149, 22)
        Me.mnuPasteBuilding.Text = "Paste Building"
        '
        'tabControlRight
        '
        Me.tabControlRight.Controls.Add(Me.tabProjectInfo)
        Me.tabControlRight.Controls.Add(Me.tabOverrides)
        Me.tabControlRight.Controls.Add(Me.tabRollup)
        Me.tabControlRight.Controls.Add(Me.tabBuildingInfo)
        Me.tabControlRight.Controls.Add(Me.tabLevelInfo)
        Me.tabControlRight.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabControlRight.Location = New System.Drawing.Point(0, 0)
        Me.tabControlRight.Name = "tabControlRight"
        Me.tabControlRight.SelectedIndex = 0
        Me.tabControlRight.Size = New System.Drawing.Size(719, 577)
        Me.tabControlRight.TabIndex = 1
        '
        'tabProjectInfo
        '
        Me.tabProjectInfo.Controls.Add(Me.btnPreviewIncExc)
        Me.tabProjectInfo.Controls.Add(Me.btnGenerateProjectReport)
        Me.tabProjectInfo.Controls.Add(Me.btnDeleteProject)
        Me.tabProjectInfo.Controls.Add(Me.btnIEOpen)
        Me.tabProjectInfo.Controls.Add(Me.btnClose)
        Me.tabProjectInfo.Controls.Add(Me.btnOpenPSE)
        Me.tabProjectInfo.Controls.Add(Me.pnlProjectInfo)
        Me.tabProjectInfo.Controls.Add(Me.btnSaveProjectInfo)
        Me.tabProjectInfo.Location = New System.Drawing.Point(4, 22)
        Me.tabProjectInfo.Name = "tabProjectInfo"
        Me.tabProjectInfo.Size = New System.Drawing.Size(711, 551)
        Me.tabProjectInfo.TabIndex = 0
        Me.tabProjectInfo.Text = "Project Info"
        '
        'btnGenerateProjectReport
        '
        Me.btnGenerateProjectReport.Location = New System.Drawing.Point(606, 519)
        Me.btnGenerateProjectReport.Name = "btnGenerateProjectReport"
        Me.btnGenerateProjectReport.Size = New System.Drawing.Size(94, 24)
        Me.btnGenerateProjectReport.TabIndex = 25
        Me.btnGenerateProjectReport.Text = "Project Report"
        Me.btnGenerateProjectReport.UseVisualStyleBackColor = True
        '
        'btnDeleteProject
        '
        Me.btnDeleteProject.Location = New System.Drawing.Point(39, 519)
        Me.btnDeleteProject.Name = "btnDeleteProject"
        Me.btnDeleteProject.Size = New System.Drawing.Size(89, 25)
        Me.btnDeleteProject.TabIndex = 24
        Me.btnDeleteProject.Text = "Delete Project"
        Me.btnDeleteProject.UseVisualStyleBackColor = True
        '
        'btnIEOpen
        '
        Me.btnIEOpen.Location = New System.Drawing.Point(255, 518)
        Me.btnIEOpen.Name = "btnIEOpen"
        Me.btnIEOpen.Size = New System.Drawing.Size(93, 27)
        Me.btnIEOpen.TabIndex = 23
        Me.btnIEOpen.Text = "Open IE"
        Me.btnIEOpen.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Location = New System.Drawing.Point(487, 518)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(101, 27)
        Me.btnClose.TabIndex = 22
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'btnOpenPSE
        '
        Me.btnOpenPSE.Location = New System.Drawing.Point(146, 518)
        Me.btnOpenPSE.Name = "btnOpenPSE"
        Me.btnOpenPSE.Size = New System.Drawing.Size(100, 27)
        Me.btnOpenPSE.TabIndex = 21
        Me.btnOpenPSE.Text = "Open PSE"
        '
        'pnlProjectInfo
        '
        Me.pnlProjectInfo.AutoScroll = True
        Me.pnlProjectInfo.Controls.Add(Me.Label1)
        Me.pnlProjectInfo.Controls.Add(Me.btnEditEngineer)
        Me.pnlProjectInfo.Controls.Add(Me.btnAddEngineer)
        Me.pnlProjectInfo.Controls.Add(Me.btnEditArchitect)
        Me.pnlProjectInfo.Controls.Add(Me.btnAddArchitect)
        Me.pnlProjectInfo.Controls.Add(Me.btnEditCustomer)
        Me.pnlProjectInfo.Controls.Add(Me.btnAddCustomer)
        Me.pnlProjectInfo.Controls.Add(Me.cboProjectEngineer)
        Me.pnlProjectInfo.Controls.Add(Me.cboProjectArchitect)
        Me.pnlProjectInfo.Controls.Add(Me.btnDuplicateVersion)
        Me.pnlProjectInfo.Controls.Add(Me.btnCreateVersion)
        Me.pnlProjectInfo.Controls.Add(Me.cboVersion)
        Me.pnlProjectInfo.Controls.Add(Me.cboState)
        Me.pnlProjectInfo.Controls.Add(Me.cboEstimator)
        Me.pnlProjectInfo.Controls.Add(Me.cboProjectType)
        Me.pnlProjectInfo.Controls.Add(Me.cboSalesman)
        Me.pnlProjectInfo.Controls.Add(Me.cboCustomer)
        Me.pnlProjectInfo.Controls.Add(Me.dtpLastModified)
        Me.pnlProjectInfo.Controls.Add(Me.lblJBID)
        Me.pnlProjectInfo.Controls.Add(Me.txtJBID)
        Me.pnlProjectInfo.Controls.Add(Me.lblProjectType)
        Me.pnlProjectInfo.Controls.Add(Me.lblSalesman)
        Me.pnlProjectInfo.Controls.Add(Me.lblProjectName)
        Me.pnlProjectInfo.Controls.Add(Me.txtProjectName)
        Me.pnlProjectInfo.Controls.Add(Me.lblEstimator)
        Me.pnlProjectInfo.Controls.Add(Me.lblAddress)
        Me.pnlProjectInfo.Controls.Add(Me.txtAddress)
        Me.pnlProjectInfo.Controls.Add(Me.lblCity)
        Me.pnlProjectInfo.Controls.Add(Me.txtCity)
        Me.pnlProjectInfo.Controls.Add(Me.lblState)
        Me.pnlProjectInfo.Controls.Add(Me.lblZip)
        Me.pnlProjectInfo.Controls.Add(Me.txtZip)
        Me.pnlProjectInfo.Controls.Add(Me.lblBidDate)
        Me.pnlProjectInfo.Controls.Add(Me.dtpBidDate)
        Me.pnlProjectInfo.Controls.Add(Me.lblArchPlansDated)
        Me.pnlProjectInfo.Controls.Add(Me.dtpArchPlansDated)
        Me.pnlProjectInfo.Controls.Add(Me.lblEngPlansDated)
        Me.pnlProjectInfo.Controls.Add(Me.dtpEngPlansDated)
        Me.pnlProjectInfo.Controls.Add(Me.lblCustomerName)
        Me.pnlProjectInfo.Controls.Add(Me.lblMilesToJobSite)
        Me.pnlProjectInfo.Controls.Add(Me.nudMilesToJobSite)
        Me.pnlProjectInfo.Controls.Add(Me.lblTotalNetSqft)
        Me.pnlProjectInfo.Controls.Add(Me.nudTotalNetSqft)
        Me.pnlProjectInfo.Controls.Add(Me.lblTotalGrossSqft)
        Me.pnlProjectInfo.Controls.Add(Me.nudTotalGrossSqft)
        Me.pnlProjectInfo.Controls.Add(Me.lblProjectArchitect)
        Me.pnlProjectInfo.Controls.Add(Me.lblProjectEngineer)
        Me.pnlProjectInfo.Controls.Add(Me.lblProjectNotes)
        Me.pnlProjectInfo.Controls.Add(Me.txtProjectNotes)
        Me.pnlProjectInfo.Controls.Add(Me.lblVersion)
        Me.pnlProjectInfo.Controls.Add(Me.lblVersionDate)
        Me.pnlProjectInfo.Controls.Add(Me.dtpCreatedDate)
        Me.pnlProjectInfo.Location = New System.Drawing.Point(3, 3)
        Me.pnlProjectInfo.Name = "pnlProjectInfo"
        Me.pnlProjectInfo.Size = New System.Drawing.Size(593, 509)
        Me.pnlProjectInfo.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 19)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(81, 13)
        Me.Label1.TabIndex = 55
        Me.Label1.Text = "Project Version:"
        '
        'btnEditEngineer
        '
        Me.btnEditEngineer.Location = New System.Drawing.Point(503, 304)
        Me.btnEditEngineer.Name = "btnEditEngineer"
        Me.btnEditEngineer.Size = New System.Drawing.Size(34, 20)
        Me.btnEditEngineer.TabIndex = 54
        Me.btnEditEngineer.Text = "Edit"
        Me.btnEditEngineer.UseVisualStyleBackColor = True
        '
        'btnAddEngineer
        '
        Me.btnAddEngineer.Location = New System.Drawing.Point(463, 304)
        Me.btnAddEngineer.Name = "btnAddEngineer"
        Me.btnAddEngineer.Size = New System.Drawing.Size(34, 20)
        Me.btnAddEngineer.TabIndex = 53
        Me.btnAddEngineer.Text = "Add"
        Me.btnAddEngineer.UseVisualStyleBackColor = True
        '
        'btnEditArchitect
        '
        Me.btnEditArchitect.Location = New System.Drawing.Point(503, 278)
        Me.btnEditArchitect.Name = "btnEditArchitect"
        Me.btnEditArchitect.Size = New System.Drawing.Size(34, 20)
        Me.btnEditArchitect.TabIndex = 52
        Me.btnEditArchitect.Text = "Edit"
        Me.btnEditArchitect.UseVisualStyleBackColor = True
        '
        'btnAddArchitect
        '
        Me.btnAddArchitect.Location = New System.Drawing.Point(463, 278)
        Me.btnAddArchitect.Name = "btnAddArchitect"
        Me.btnAddArchitect.Size = New System.Drawing.Size(34, 20)
        Me.btnAddArchitect.TabIndex = 51
        Me.btnAddArchitect.Text = "Add"
        Me.btnAddArchitect.UseVisualStyleBackColor = True
        '
        'btnEditCustomer
        '
        Me.btnEditCustomer.Location = New System.Drawing.Point(503, 250)
        Me.btnEditCustomer.Name = "btnEditCustomer"
        Me.btnEditCustomer.Size = New System.Drawing.Size(34, 20)
        Me.btnEditCustomer.TabIndex = 50
        Me.btnEditCustomer.Text = "Edit"
        Me.btnEditCustomer.UseVisualStyleBackColor = True
        '
        'btnAddCustomer
        '
        Me.btnAddCustomer.Location = New System.Drawing.Point(463, 251)
        Me.btnAddCustomer.Name = "btnAddCustomer"
        Me.btnAddCustomer.Size = New System.Drawing.Size(34, 20)
        Me.btnAddCustomer.TabIndex = 49
        Me.btnAddCustomer.Text = "Add"
        Me.btnAddCustomer.UseVisualStyleBackColor = True
        '
        'cboProjectEngineer
        '
        Me.cboProjectEngineer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboProjectEngineer.FormattingEnabled = True
        Me.cboProjectEngineer.Location = New System.Drawing.Point(98, 302)
        Me.cboProjectEngineer.Name = "cboProjectEngineer"
        Me.cboProjectEngineer.Size = New System.Drawing.Size(359, 21)
        Me.cboProjectEngineer.TabIndex = 18
        '
        'cboProjectArchitect
        '
        Me.cboProjectArchitect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboProjectArchitect.FormattingEnabled = True
        Me.cboProjectArchitect.Location = New System.Drawing.Point(98, 277)
        Me.cboProjectArchitect.Name = "cboProjectArchitect"
        Me.cboProjectArchitect.Size = New System.Drawing.Size(359, 21)
        Me.cboProjectArchitect.TabIndex = 17
        '
        'btnDuplicateVersion
        '
        Me.btnDuplicateVersion.Location = New System.Drawing.Point(440, 12)
        Me.btnDuplicateVersion.Name = "btnDuplicateVersion"
        Me.btnDuplicateVersion.Size = New System.Drawing.Size(98, 21)
        Me.btnDuplicateVersion.TabIndex = 45
        Me.btnDuplicateVersion.Text = "Duplicate Version"
        Me.btnDuplicateVersion.UseVisualStyleBackColor = True
        '
        'btnCreateVersion
        '
        Me.btnCreateVersion.Location = New System.Drawing.Point(336, 12)
        Me.btnCreateVersion.Name = "btnCreateVersion"
        Me.btnCreateVersion.Size = New System.Drawing.Size(98, 20)
        Me.btnCreateVersion.TabIndex = 44
        Me.btnCreateVersion.Text = "Create Version"
        Me.btnCreateVersion.UseVisualStyleBackColor = True
        '
        'cboVersion
        '
        Me.cboVersion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboVersion.FormattingEnabled = True
        Me.cboVersion.Location = New System.Drawing.Point(98, 13)
        Me.cboVersion.Name = "cboVersion"
        Me.cboVersion.Size = New System.Drawing.Size(180, 21)
        Me.cboVersion.TabIndex = 43
        '
        'cboState
        '
        Me.cboState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboState.FormattingEnabled = True
        Me.cboState.Items.AddRange(New Object() {"AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DE", "FL", "GA", "HI", "IA", "ID", "IL", "IN", "KS", "KY", "LA", "ME", "MD", "MA", "MI", "MN", "MO", "MS", "MT", "NC", "ND", "NE", "NH", "NJ", "NM", "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VA", "VT", "WA", "WI", "WV", "WY"})
        Me.cboState.Location = New System.Drawing.Point(373, 118)
        Me.cboState.Name = "cboState"
        Me.cboState.Size = New System.Drawing.Size(51, 21)
        Me.cboState.TabIndex = 6
        '
        'cboEstimator
        '
        Me.cboEstimator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEstimator.FormattingEnabled = True
        Me.cboEstimator.Location = New System.Drawing.Point(98, 196)
        Me.cboEstimator.Name = "cboEstimator"
        Me.cboEstimator.Size = New System.Drawing.Size(180, 21)
        Me.cboEstimator.TabIndex = 12
        '
        'cboProjectType
        '
        Me.cboProjectType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboProjectType.FormattingEnabled = True
        Me.cboProjectType.Location = New System.Drawing.Point(373, 39)
        Me.cboProjectType.Name = "cboProjectType"
        Me.cboProjectType.Size = New System.Drawing.Size(164, 21)
        Me.cboProjectType.TabIndex = 2
        '
        'cboSalesman
        '
        Me.cboSalesman.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSalesman.FormattingEnabled = True
        Me.cboSalesman.Location = New System.Drawing.Point(98, 223)
        Me.cboSalesman.Name = "cboSalesman"
        Me.cboSalesman.Size = New System.Drawing.Size(180, 21)
        Me.cboSalesman.TabIndex = 13
        '
        'cboCustomer
        '
        Me.cboCustomer.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCustomer.FormattingEnabled = True
        Me.cboCustomer.Location = New System.Drawing.Point(98, 250)
        Me.cboCustomer.Name = "cboCustomer"
        Me.cboCustomer.Size = New System.Drawing.Size(359, 21)
        Me.cboCustomer.TabIndex = 16
        '
        'dtpLastModified
        '
        Me.dtpLastModified.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpLastModified.Location = New System.Drawing.Point(98, 483)
        Me.dtpLastModified.Name = "dtpLastModified"
        Me.dtpLastModified.Size = New System.Drawing.Size(160, 20)
        Me.dtpLastModified.TabIndex = 42
        Me.dtpLastModified.TabStop = False
        '
        'lblJBID
        '
        Me.lblJBID.AutoSize = True
        Me.lblJBID.Location = New System.Drawing.Point(9, 43)
        Me.lblJBID.Name = "lblJBID"
        Me.lblJBID.Size = New System.Drawing.Size(83, 13)
        Me.lblJBID.TabIndex = 0
        Me.lblJBID.Text = "Project Number:"
        '
        'txtJBID
        '
        Me.txtJBID.Location = New System.Drawing.Point(98, 40)
        Me.txtJBID.Name = "txtJBID"
        Me.txtJBID.Size = New System.Drawing.Size(180, 20)
        Me.txtJBID.TabIndex = 1
        '
        'lblProjectType
        '
        Me.lblProjectType.AutoSize = True
        Me.lblProjectType.Location = New System.Drawing.Point(297, 42)
        Me.lblProjectType.Name = "lblProjectType"
        Me.lblProjectType.Size = New System.Drawing.Size(70, 13)
        Me.lblProjectType.TabIndex = 2
        Me.lblProjectType.Text = "Project Type:"
        '
        'lblSalesman
        '
        Me.lblSalesman.AutoSize = True
        Me.lblSalesman.Location = New System.Drawing.Point(36, 226)
        Me.lblSalesman.Name = "lblSalesman"
        Me.lblSalesman.Size = New System.Drawing.Size(56, 13)
        Me.lblSalesman.TabIndex = 4
        Me.lblSalesman.Text = "Salesman:"
        '
        'lblProjectName
        '
        Me.lblProjectName.AutoSize = True
        Me.lblProjectName.Location = New System.Drawing.Point(18, 69)
        Me.lblProjectName.Name = "lblProjectName"
        Me.lblProjectName.Size = New System.Drawing.Size(74, 13)
        Me.lblProjectName.TabIndex = 6
        Me.lblProjectName.Text = "Project Name:"
        '
        'txtProjectName
        '
        Me.txtProjectName.Location = New System.Drawing.Point(98, 66)
        Me.txtProjectName.Name = "txtProjectName"
        Me.txtProjectName.Size = New System.Drawing.Size(439, 20)
        Me.txtProjectName.TabIndex = 3
        '
        'lblEstimator
        '
        Me.lblEstimator.AutoSize = True
        Me.lblEstimator.Location = New System.Drawing.Point(39, 199)
        Me.lblEstimator.Name = "lblEstimator"
        Me.lblEstimator.Size = New System.Drawing.Size(53, 13)
        Me.lblEstimator.TabIndex = 8
        Me.lblEstimator.Text = "Estimator:"
        '
        'lblAddress
        '
        Me.lblAddress.AutoSize = True
        Me.lblAddress.Location = New System.Drawing.Point(44, 95)
        Me.lblAddress.Name = "lblAddress"
        Me.lblAddress.Size = New System.Drawing.Size(48, 13)
        Me.lblAddress.TabIndex = 10
        Me.lblAddress.Text = "Address:"
        '
        'txtAddress
        '
        Me.txtAddress.Location = New System.Drawing.Point(98, 92)
        Me.txtAddress.Name = "txtAddress"
        Me.txtAddress.Size = New System.Drawing.Size(439, 20)
        Me.txtAddress.TabIndex = 4
        '
        'lblCity
        '
        Me.lblCity.AutoSize = True
        Me.lblCity.Location = New System.Drawing.Point(65, 121)
        Me.lblCity.Name = "lblCity"
        Me.lblCity.Size = New System.Drawing.Size(27, 13)
        Me.lblCity.TabIndex = 12
        Me.lblCity.Text = "City:"
        '
        'txtCity
        '
        Me.txtCity.Location = New System.Drawing.Point(98, 118)
        Me.txtCity.Name = "txtCity"
        Me.txtCity.Size = New System.Drawing.Size(228, 20)
        Me.txtCity.TabIndex = 5
        '
        'lblState
        '
        Me.lblState.AutoSize = True
        Me.lblState.Location = New System.Drawing.Point(332, 123)
        Me.lblState.Name = "lblState"
        Me.lblState.Size = New System.Drawing.Size(35, 13)
        Me.lblState.TabIndex = 14
        Me.lblState.Text = "State:"
        '
        'lblZip
        '
        Me.lblZip.AutoSize = True
        Me.lblZip.Location = New System.Drawing.Point(432, 123)
        Me.lblZip.Name = "lblZip"
        Me.lblZip.Size = New System.Drawing.Size(25, 13)
        Me.lblZip.TabIndex = 16
        Me.lblZip.Text = "Zip:"
        '
        'txtZip
        '
        Me.txtZip.Location = New System.Drawing.Point(463, 118)
        Me.txtZip.Name = "txtZip"
        Me.txtZip.Size = New System.Drawing.Size(74, 20)
        Me.txtZip.TabIndex = 7
        '
        'lblBidDate
        '
        Me.lblBidDate.AutoSize = True
        Me.lblBidDate.Location = New System.Drawing.Point(41, 150)
        Me.lblBidDate.Name = "lblBidDate"
        Me.lblBidDate.Size = New System.Drawing.Size(51, 13)
        Me.lblBidDate.TabIndex = 18
        Me.lblBidDate.Text = "Bid Date:"
        '
        'dtpBidDate
        '
        Me.dtpBidDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpBidDate.Location = New System.Drawing.Point(98, 144)
        Me.dtpBidDate.Name = "dtpBidDate"
        Me.dtpBidDate.Size = New System.Drawing.Size(100, 20)
        Me.dtpBidDate.TabIndex = 8
        '
        'lblArchPlansDated
        '
        Me.lblArchPlansDated.AutoSize = True
        Me.lblArchPlansDated.Location = New System.Drawing.Point(338, 150)
        Me.lblArchPlansDated.Name = "lblArchPlansDated"
        Me.lblArchPlansDated.Size = New System.Drawing.Size(93, 13)
        Me.lblArchPlansDated.TabIndex = 20
        Me.lblArchPlansDated.Text = "Arch Plans Dated:"
        '
        'dtpArchPlansDated
        '
        Me.dtpArchPlansDated.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpArchPlansDated.Location = New System.Drawing.Point(437, 144)
        Me.dtpArchPlansDated.Name = "dtpArchPlansDated"
        Me.dtpArchPlansDated.Size = New System.Drawing.Size(100, 20)
        Me.dtpArchPlansDated.TabIndex = 10
        '
        'lblEngPlansDated
        '
        Me.lblEngPlansDated.AutoSize = True
        Me.lblEngPlansDated.Location = New System.Drawing.Point(341, 175)
        Me.lblEngPlansDated.Name = "lblEngPlansDated"
        Me.lblEngPlansDated.Size = New System.Drawing.Size(90, 13)
        Me.lblEngPlansDated.TabIndex = 22
        Me.lblEngPlansDated.Text = "Eng Plans Dated:"
        '
        'dtpEngPlansDated
        '
        Me.dtpEngPlansDated.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpEngPlansDated.Location = New System.Drawing.Point(437, 169)
        Me.dtpEngPlansDated.Name = "dtpEngPlansDated"
        Me.dtpEngPlansDated.Size = New System.Drawing.Size(100, 20)
        Me.dtpEngPlansDated.TabIndex = 11
        '
        'lblCustomerName
        '
        Me.lblCustomerName.AutoSize = True
        Me.lblCustomerName.Location = New System.Drawing.Point(38, 254)
        Me.lblCustomerName.Name = "lblCustomerName"
        Me.lblCustomerName.Size = New System.Drawing.Size(54, 13)
        Me.lblCustomerName.TabIndex = 24
        Me.lblCustomerName.Text = "Customer:"
        '
        'lblMilesToJobSite
        '
        Me.lblMilesToJobSite.AutoSize = True
        Me.lblMilesToJobSite.Location = New System.Drawing.Point(4, 176)
        Me.lblMilesToJobSite.Name = "lblMilesToJobSite"
        Me.lblMilesToJobSite.Size = New System.Drawing.Size(88, 13)
        Me.lblMilesToJobSite.TabIndex = 26
        Me.lblMilesToJobSite.Text = "Miles To JobSite:"
        '
        'nudMilesToJobSite
        '
        Me.nudMilesToJobSite.Location = New System.Drawing.Point(98, 170)
        Me.nudMilesToJobSite.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.nudMilesToJobSite.Name = "nudMilesToJobSite"
        Me.nudMilesToJobSite.Size = New System.Drawing.Size(100, 20)
        Me.nudMilesToJobSite.TabIndex = 9
        '
        'lblTotalNetSqft
        '
        Me.lblTotalNetSqft.AutoSize = True
        Me.lblTotalNetSqft.Location = New System.Drawing.Point(295, 199)
        Me.lblTotalNetSqft.Name = "lblTotalNetSqft"
        Me.lblTotalNetSqft.Size = New System.Drawing.Size(76, 13)
        Me.lblTotalNetSqft.TabIndex = 28
        Me.lblTotalNetSqft.Text = "Total Net Sqft:"
        '
        'nudTotalNetSqft
        '
        Me.nudTotalNetSqft.Location = New System.Drawing.Point(377, 197)
        Me.nudTotalNetSqft.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.nudTotalNetSqft.Name = "nudTotalNetSqft"
        Me.nudTotalNetSqft.Size = New System.Drawing.Size(160, 20)
        Me.nudTotalNetSqft.TabIndex = 14
        '
        'lblTotalGrossSqft
        '
        Me.lblTotalGrossSqft.AutoSize = True
        Me.lblTotalGrossSqft.Location = New System.Drawing.Point(285, 226)
        Me.lblTotalGrossSqft.Name = "lblTotalGrossSqft"
        Me.lblTotalGrossSqft.Size = New System.Drawing.Size(86, 13)
        Me.lblTotalGrossSqft.TabIndex = 30
        Me.lblTotalGrossSqft.Text = "Total Gross Sqft:"
        '
        'nudTotalGrossSqft
        '
        Me.nudTotalGrossSqft.Location = New System.Drawing.Point(377, 223)
        Me.nudTotalGrossSqft.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.nudTotalGrossSqft.Name = "nudTotalGrossSqft"
        Me.nudTotalGrossSqft.Size = New System.Drawing.Size(160, 20)
        Me.nudTotalGrossSqft.TabIndex = 15
        '
        'lblProjectArchitect
        '
        Me.lblProjectArchitect.AutoSize = True
        Me.lblProjectArchitect.Location = New System.Drawing.Point(4, 282)
        Me.lblProjectArchitect.Name = "lblProjectArchitect"
        Me.lblProjectArchitect.Size = New System.Drawing.Size(88, 13)
        Me.lblProjectArchitect.TabIndex = 32
        Me.lblProjectArchitect.Text = "Project Architect:"
        '
        'lblProjectEngineer
        '
        Me.lblProjectEngineer.AutoSize = True
        Me.lblProjectEngineer.Location = New System.Drawing.Point(4, 308)
        Me.lblProjectEngineer.Name = "lblProjectEngineer"
        Me.lblProjectEngineer.Size = New System.Drawing.Size(88, 13)
        Me.lblProjectEngineer.TabIndex = 34
        Me.lblProjectEngineer.Text = "Project Engineer:"
        '
        'lblProjectNotes
        '
        Me.lblProjectNotes.AutoSize = True
        Me.lblProjectNotes.Location = New System.Drawing.Point(18, 333)
        Me.lblProjectNotes.Name = "lblProjectNotes"
        Me.lblProjectNotes.Size = New System.Drawing.Size(74, 13)
        Me.lblProjectNotes.TabIndex = 36
        Me.lblProjectNotes.Text = "Project Notes:"
        '
        'txtProjectNotes
        '
        Me.txtProjectNotes.Location = New System.Drawing.Point(98, 330)
        Me.txtProjectNotes.Multiline = True
        Me.txtProjectNotes.Name = "txtProjectNotes"
        Me.txtProjectNotes.Size = New System.Drawing.Size(440, 147)
        Me.txtProjectNotes.TabIndex = 19
        '
        'lblVersion
        '
        Me.lblVersion.AutoSize = True
        Me.lblVersion.Location = New System.Drawing.Point(19, 489)
        Me.lblVersion.Name = "lblVersion"
        Me.lblVersion.Size = New System.Drawing.Size(73, 13)
        Me.lblVersion.TabIndex = 38
        Me.lblVersion.Text = "Last Modified:"
        '
        'lblVersionDate
        '
        Me.lblVersionDate.AutoSize = True
        Me.lblVersionDate.Location = New System.Drawing.Point(298, 489)
        Me.lblVersionDate.Name = "lblVersionDate"
        Me.lblVersionDate.Size = New System.Drawing.Size(73, 13)
        Me.lblVersionDate.TabIndex = 40
        Me.lblVersionDate.Text = "Created Date:"
        '
        'dtpCreatedDate
        '
        Me.dtpCreatedDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpCreatedDate.Location = New System.Drawing.Point(377, 483)
        Me.dtpCreatedDate.Name = "dtpCreatedDate"
        Me.dtpCreatedDate.Size = New System.Drawing.Size(160, 20)
        Me.dtpCreatedDate.TabIndex = 41
        Me.dtpCreatedDate.TabStop = False
        '
        'btnSaveProjectInfo
        '
        Me.btnSaveProjectInfo.Location = New System.Drawing.Point(354, 518)
        Me.btnSaveProjectInfo.Name = "btnSaveProjectInfo"
        Me.btnSaveProjectInfo.Size = New System.Drawing.Size(127, 27)
        Me.btnSaveProjectInfo.TabIndex = 20
        Me.btnSaveProjectInfo.Text = "Save Project Info"
        '
        'tabOverrides
        '
        Me.tabOverrides.Controls.Add(Me.Label3)
        Me.tabOverrides.Controls.Add(Me.Label2)
        Me.tabOverrides.Controls.Add(Me.txtRawRoofSPFPrice)
        Me.tabOverrides.Controls.Add(Me.txtRawFloorSPFPrice)
        Me.tabOverrides.Controls.Add(Me.dgvOverrides)
        Me.tabOverrides.Controls.Add(Me.btnSaveOverrides)
        Me.tabOverrides.Location = New System.Drawing.Point(4, 22)
        Me.tabOverrides.Name = "tabOverrides"
        Me.tabOverrides.Size = New System.Drawing.Size(711, 551)
        Me.tabOverrides.TabIndex = 0
        Me.tabOverrides.Text = "Overrides"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(30, 206)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(149, 13)
        Me.Label3.TabIndex = 48
        Me.Label3.Text = "Original SPF2 Roof Avg Price:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(30, 177)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(149, 13)
        Me.Label2.TabIndex = 47
        Me.Label2.Text = "Original SPF2 Floor Avg Price:"
        '
        'txtRawRoofSPFPrice
        '
        Me.txtRawRoofSPFPrice.Location = New System.Drawing.Point(185, 203)
        Me.txtRawRoofSPFPrice.Name = "txtRawRoofSPFPrice"
        Me.txtRawRoofSPFPrice.ReadOnly = True
        Me.txtRawRoofSPFPrice.Size = New System.Drawing.Size(68, 20)
        Me.txtRawRoofSPFPrice.TabIndex = 46
        '
        'txtRawFloorSPFPrice
        '
        Me.txtRawFloorSPFPrice.Location = New System.Drawing.Point(185, 174)
        Me.txtRawFloorSPFPrice.Name = "txtRawFloorSPFPrice"
        Me.txtRawFloorSPFPrice.ReadOnly = True
        Me.txtRawFloorSPFPrice.Size = New System.Drawing.Size(68, 20)
        Me.txtRawFloorSPFPrice.TabIndex = 45
        '
        'dgvOverrides
        '
        Me.dgvOverrides.Dock = System.Windows.Forms.DockStyle.Top
        Me.dgvOverrides.Location = New System.Drawing.Point(0, 0)
        Me.dgvOverrides.Name = "dgvOverrides"
        Me.dgvOverrides.Size = New System.Drawing.Size(711, 145)
        Me.dgvOverrides.TabIndex = 43
        '
        'btnSaveOverrides
        '
        Me.btnSaveOverrides.Location = New System.Drawing.Point(0, 483)
        Me.btnSaveOverrides.Name = "btnSaveOverrides"
        Me.btnSaveOverrides.Size = New System.Drawing.Size(562, 30)
        Me.btnSaveOverrides.TabIndex = 44
        Me.btnSaveOverrides.Text = "Save Overrides"
        '
        'tabRollup
        '
        Me.tabRollup.Controls.Add(Me.dgvRollup)
        Me.tabRollup.Controls.Add(Me.btnRecalcRollup)
        Me.tabRollup.Location = New System.Drawing.Point(4, 22)
        Me.tabRollup.Name = "tabRollup"
        Me.tabRollup.Size = New System.Drawing.Size(711, 551)
        Me.tabRollup.TabIndex = 0
        Me.tabRollup.Text = "Rollup Data"
        '
        'dgvRollup
        '
        Me.dgvRollup.Dock = System.Windows.Forms.DockStyle.Top
        Me.dgvRollup.Location = New System.Drawing.Point(0, 0)
        Me.dgvRollup.Name = "dgvRollup"
        Me.dgvRollup.Size = New System.Drawing.Size(711, 490)
        Me.dgvRollup.TabIndex = 45
        '
        'btnRecalcRollup
        '
        Me.btnRecalcRollup.Location = New System.Drawing.Point(0, 496)
        Me.btnRecalcRollup.Name = "btnRecalcRollup"
        Me.btnRecalcRollup.Size = New System.Drawing.Size(562, 30)
        Me.btnRecalcRollup.TabIndex = 46
        Me.btnRecalcRollup.Text = "Recalculate Rollup"
        '
        'tabBuildingInfo
        '
        Me.tabBuildingInfo.Controls.Add(Me.pnlBuildingInfo)
        Me.tabBuildingInfo.Controls.Add(Me.btnSaveBuildingInfo)
        Me.tabBuildingInfo.Location = New System.Drawing.Point(4, 22)
        Me.tabBuildingInfo.Name = "tabBuildingInfo"
        Me.tabBuildingInfo.Size = New System.Drawing.Size(711, 551)
        Me.tabBuildingInfo.TabIndex = 0
        Me.tabBuildingInfo.Text = "Building Info"
        '
        'pnlBuildingInfo
        '
        Me.pnlBuildingInfo.AutoScroll = True
        Me.pnlBuildingInfo.Controls.Add(Me.lblPlanUnits)
        Me.pnlBuildingInfo.Controls.Add(Me.txtResUnits)
        Me.pnlBuildingInfo.Controls.Add(Me.lblBuildingName)
        Me.pnlBuildingInfo.Controls.Add(Me.txtBuildingName)
        Me.pnlBuildingInfo.Controls.Add(Me.lblBldgQty)
        Me.pnlBuildingInfo.Controls.Add(Me.nudBldgQty)
        Me.pnlBuildingInfo.Controls.Add(Me.lblBuildingType)
        Me.pnlBuildingInfo.Controls.Add(Me.txtBuildingType)
        Me.pnlBuildingInfo.Controls.Add(Me.lblNbrUnits)
        Me.pnlBuildingInfo.Controls.Add(Me.nudNbrUnits)
        Me.pnlBuildingInfo.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlBuildingInfo.Location = New System.Drawing.Point(0, 0)
        Me.pnlBuildingInfo.Name = "pnlBuildingInfo"
        Me.pnlBuildingInfo.Size = New System.Drawing.Size(711, 489)
        Me.pnlBuildingInfo.TabIndex = 0
        '
        'lblPlanUnits
        '
        Me.lblPlanUnits.AutoSize = True
        Me.lblPlanUnits.Location = New System.Drawing.Point(36, 99)
        Me.lblPlanUnits.Name = "lblPlanUnits"
        Me.lblPlanUnits.Size = New System.Drawing.Size(58, 13)
        Me.lblPlanUnits.TabIndex = 9
        Me.lblPlanUnits.Text = "Plan Units:"
        '
        'txtResUnits
        '
        Me.txtResUnits.Location = New System.Drawing.Point(100, 96)
        Me.txtResUnits.Name = "txtResUnits"
        Me.txtResUnits.Size = New System.Drawing.Size(91, 20)
        Me.txtResUnits.TabIndex = 4
        '
        'lblBuildingName
        '
        Me.lblBuildingName.AutoSize = True
        Me.lblBuildingName.Location = New System.Drawing.Point(16, 13)
        Me.lblBuildingName.Name = "lblBuildingName"
        Me.lblBuildingName.Size = New System.Drawing.Size(78, 13)
        Me.lblBuildingName.TabIndex = 0
        Me.lblBuildingName.Text = "Building Name:"
        '
        'txtBuildingName
        '
        Me.txtBuildingName.Location = New System.Drawing.Point(100, 10)
        Me.txtBuildingName.Name = "txtBuildingName"
        Me.txtBuildingName.Size = New System.Drawing.Size(440, 20)
        Me.txtBuildingName.TabIndex = 1
        '
        'lblBldgQty
        '
        Me.lblBldgQty.AutoSize = True
        Me.lblBldgQty.Location = New System.Drawing.Point(44, 42)
        Me.lblBldgQty.Name = "lblBldgQty"
        Me.lblBldgQty.Size = New System.Drawing.Size(50, 13)
        Me.lblBldgQty.TabIndex = 2
        Me.lblBldgQty.Text = "Bldg Qty:"
        '
        'nudBldgQty
        '
        Me.nudBldgQty.Location = New System.Drawing.Point(100, 40)
        Me.nudBldgQty.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.nudBldgQty.Name = "nudBldgQty"
        Me.nudBldgQty.Size = New System.Drawing.Size(91, 20)
        Me.nudBldgQty.TabIndex = 2
        '
        'lblBuildingType
        '
        Me.lblBuildingType.AutoSize = True
        Me.lblBuildingType.Location = New System.Drawing.Point(20, 73)
        Me.lblBuildingType.Name = "lblBuildingType"
        Me.lblBuildingType.Size = New System.Drawing.Size(74, 13)
        Me.lblBuildingType.TabIndex = 4
        Me.lblBuildingType.Text = "Building Type:"
        Me.lblBuildingType.Visible = False
        '
        'txtBuildingType
        '
        Me.txtBuildingType.Location = New System.Drawing.Point(100, 70)
        Me.txtBuildingType.Name = "txtBuildingType"
        Me.txtBuildingType.Size = New System.Drawing.Size(440, 20)
        Me.txtBuildingType.TabIndex = 3
        Me.txtBuildingType.Visible = False
        '
        'lblNbrUnits
        '
        Me.lblNbrUnits.AutoSize = True
        Me.lblNbrUnits.Location = New System.Drawing.Point(211, 101)
        Me.lblNbrUnits.Name = "lblNbrUnits"
        Me.lblNbrUnits.Size = New System.Drawing.Size(67, 13)
        Me.lblNbrUnits.TabIndex = 6
        Me.lblNbrUnits.Text = "Actual Units:"
        '
        'nudNbrUnits
        '
        Me.nudNbrUnits.Location = New System.Drawing.Point(284, 99)
        Me.nudNbrUnits.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.nudNbrUnits.Name = "nudNbrUnits"
        Me.nudNbrUnits.Size = New System.Drawing.Size(91, 20)
        Me.nudNbrUnits.TabIndex = 7
        Me.nudNbrUnits.TabStop = False
        '
        'btnSaveBuildingInfo
        '
        Me.btnSaveBuildingInfo.Location = New System.Drawing.Point(0, 495)
        Me.btnSaveBuildingInfo.Name = "btnSaveBuildingInfo"
        Me.btnSaveBuildingInfo.Size = New System.Drawing.Size(562, 30)
        Me.btnSaveBuildingInfo.TabIndex = 8
        Me.btnSaveBuildingInfo.Text = "Save Building Info"
        '
        'tabLevelInfo
        '
        Me.tabLevelInfo.Controls.Add(Me.pnlLevelInfo)
        Me.tabLevelInfo.Controls.Add(Me.btnSaveLevelInfo)
        Me.tabLevelInfo.Location = New System.Drawing.Point(4, 22)
        Me.tabLevelInfo.Name = "tabLevelInfo"
        Me.tabLevelInfo.Size = New System.Drawing.Size(711, 551)
        Me.tabLevelInfo.TabIndex = 0
        Me.tabLevelInfo.Text = "Level Info"
        '
        'pnlLevelInfo
        '
        Me.pnlLevelInfo.AutoScroll = True
        Me.pnlLevelInfo.Controls.Add(Me.nudLevelNumber)
        Me.pnlLevelInfo.Controls.Add(Me.lblLevelName)
        Me.pnlLevelInfo.Controls.Add(Me.txtLevelName)
        Me.pnlLevelInfo.Controls.Add(Me.lblLevelType)
        Me.pnlLevelInfo.Controls.Add(Me.cmbLevelType)
        Me.pnlLevelInfo.Controls.Add(Me.lblLevelNumber)
        Me.pnlLevelInfo.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlLevelInfo.Location = New System.Drawing.Point(0, 0)
        Me.pnlLevelInfo.Name = "pnlLevelInfo"
        Me.pnlLevelInfo.Size = New System.Drawing.Size(711, 489)
        Me.pnlLevelInfo.TabIndex = 0
        '
        'nudLevelNumber
        '
        Me.nudLevelNumber.Location = New System.Drawing.Point(100, 70)
        Me.nudLevelNumber.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.nudLevelNumber.Name = "nudLevelNumber"
        Me.nudLevelNumber.Size = New System.Drawing.Size(79, 20)
        Me.nudLevelNumber.TabIndex = 6
        '
        'lblLevelName
        '
        Me.lblLevelName.AutoSize = True
        Me.lblLevelName.Location = New System.Drawing.Point(27, 13)
        Me.lblLevelName.Name = "lblLevelName"
        Me.lblLevelName.Size = New System.Drawing.Size(67, 13)
        Me.lblLevelName.TabIndex = 0
        Me.lblLevelName.Text = "Level Name:"
        '
        'txtLevelName
        '
        Me.txtLevelName.Location = New System.Drawing.Point(100, 10)
        Me.txtLevelName.Name = "txtLevelName"
        Me.txtLevelName.Size = New System.Drawing.Size(126, 20)
        Me.txtLevelName.TabIndex = 1
        '
        'lblLevelType
        '
        Me.lblLevelType.AutoSize = True
        Me.lblLevelType.Location = New System.Drawing.Point(31, 43)
        Me.lblLevelType.Name = "lblLevelType"
        Me.lblLevelType.Size = New System.Drawing.Size(63, 13)
        Me.lblLevelType.TabIndex = 2
        Me.lblLevelType.Text = "Level Type:"
        '
        'cmbLevelType
        '
        Me.cmbLevelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbLevelType.Location = New System.Drawing.Point(100, 40)
        Me.cmbLevelType.Name = "cmbLevelType"
        Me.cmbLevelType.Size = New System.Drawing.Size(126, 21)
        Me.cmbLevelType.TabIndex = 3
        '
        'lblLevelNumber
        '
        Me.lblLevelNumber.AutoSize = True
        Me.lblLevelNumber.Location = New System.Drawing.Point(18, 72)
        Me.lblLevelNumber.Name = "lblLevelNumber"
        Me.lblLevelNumber.Size = New System.Drawing.Size(76, 13)
        Me.lblLevelNumber.TabIndex = 4
        Me.lblLevelNumber.Text = "Level Number:"
        '
        'btnSaveLevelInfo
        '
        Me.btnSaveLevelInfo.Location = New System.Drawing.Point(0, 495)
        Me.btnSaveLevelInfo.Name = "btnSaveLevelInfo"
        Me.btnSaveLevelInfo.Size = New System.Drawing.Size(562, 30)
        Me.btnSaveLevelInfo.TabIndex = 6
        Me.btnSaveLevelInfo.Text = "Save Level Info"
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.tvProjectTree)
        Me.SplitContainer1.Panel1MinSize = 200
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.tabControlRight)
        Me.SplitContainer1.Size = New System.Drawing.Size(923, 577)
        Me.SplitContainer1.SplitterDistance = 200
        Me.SplitContainer1.TabIndex = 2
        '
        'btnPreviewIncExc
        '
        Me.btnPreviewIncExc.Location = New System.Drawing.Point(606, 492)
        Me.btnPreviewIncExc.Name = "btnPreviewIncExc"
        Me.btnPreviewIncExc.Size = New System.Drawing.Size(94, 24)
        Me.btnPreviewIncExc.TabIndex = 26
        Me.btnPreviewIncExc.Text = "I/E Report"
        Me.btnPreviewIncExc.UseVisualStyleBackColor = True
        '
        'frmCreateEditProject
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(923, 577)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Name = "frmCreateEditProject"
        Me.Text = "Create/Edit Project"
        Me.cmsTreeMenu.ResumeLayout(False)
        Me.tabControlRight.ResumeLayout(False)
        Me.tabProjectInfo.ResumeLayout(False)
        Me.pnlProjectInfo.ResumeLayout(False)
        Me.pnlProjectInfo.PerformLayout()
        CType(Me.nudMilesToJobSite, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudTotalNetSqft, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudTotalGrossSqft, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabOverrides.ResumeLayout(False)
        Me.tabOverrides.PerformLayout()
        CType(Me.dgvOverrides, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabRollup.ResumeLayout(False)
        CType(Me.dgvRollup, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabBuildingInfo.ResumeLayout(False)
        Me.pnlBuildingInfo.ResumeLayout(False)
        Me.pnlBuildingInfo.PerformLayout()
        CType(Me.nudBldgQty, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudNbrUnits, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabLevelInfo.ResumeLayout(False)
        Me.pnlLevelInfo.ResumeLayout(False)
        Me.pnlLevelInfo.PerformLayout()
        CType(Me.nudLevelNumber, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents tvProjectTree As TreeView
    Friend WithEvents tabControlRight As TabControl
    Friend WithEvents tabProjectInfo As TabPage
    Friend WithEvents pnlProjectInfo As Panel
    Friend WithEvents lblJBID As Label
    Friend WithEvents txtJBID As TextBox
    Friend WithEvents lblProjectType As Label
    Friend WithEvents lblSalesman As Label
    Friend WithEvents lblProjectName As Label
    Friend WithEvents txtProjectName As TextBox
    Friend WithEvents lblEstimator As Label
    Friend WithEvents lblAddress As Label
    Friend WithEvents txtAddress As TextBox
    Friend WithEvents lblCity As Label
    Friend WithEvents txtCity As TextBox
    Friend WithEvents lblState As Label
    Friend WithEvents lblZip As Label
    Friend WithEvents txtZip As TextBox
    Friend WithEvents lblBidDate As Label
    Friend WithEvents dtpBidDate As DateTimePicker
    Friend WithEvents lblArchPlansDated As Label
    Friend WithEvents dtpArchPlansDated As DateTimePicker
    Friend WithEvents lblEngPlansDated As Label
    Friend WithEvents dtpEngPlansDated As DateTimePicker
    Friend WithEvents lblCustomerName As Label
    Friend WithEvents lblMilesToJobSite As Label
    Friend WithEvents nudMilesToJobSite As NumericUpDown
    Friend WithEvents lblTotalNetSqft As Label
    Friend WithEvents nudTotalNetSqft As NumericUpDown
    Friend WithEvents lblTotalGrossSqft As Label
    Friend WithEvents nudTotalGrossSqft As NumericUpDown
    Friend WithEvents lblProjectArchitect As Label
    Friend WithEvents lblProjectEngineer As Label
    Friend WithEvents lblProjectNotes As Label
    Friend WithEvents txtProjectNotes As TextBox
    Friend WithEvents lblVersion As Label
    Friend WithEvents lblVersionDate As Label
    Friend WithEvents dtpCreatedDate As DateTimePicker
    Friend WithEvents btnSaveProjectInfo As Button
    Friend WithEvents tabOverrides As TabPage
    Friend WithEvents dgvOverrides As DataGridView
    Friend WithEvents btnSaveOverrides As Button
    Friend WithEvents tabRollup As TabPage
    Friend WithEvents dgvRollup As DataGridView
    Friend WithEvents btnRecalcRollup As Button
    Friend WithEvents tabBuildingInfo As TabPage
    Friend WithEvents pnlBuildingInfo As Panel
    Friend WithEvents lblBuildingName As Label
    Friend WithEvents txtBuildingName As TextBox
    Friend WithEvents lblBldgQty As Label
    Friend WithEvents nudBldgQty As NumericUpDown
    Friend WithEvents lblBuildingType As Label
    Friend WithEvents txtBuildingType As TextBox
    Friend WithEvents lblNbrUnits As Label
    Friend WithEvents nudNbrUnits As NumericUpDown
    Friend WithEvents btnSaveBuildingInfo As Button
    Friend WithEvents tabLevelInfo As TabPage
    Friend WithEvents pnlLevelInfo As Panel
    Friend WithEvents lblLevelName As Label
    Friend WithEvents txtLevelName As TextBox
    Friend WithEvents lblLevelType As Label
    Friend WithEvents cmbLevelType As ComboBox
    Friend WithEvents lblLevelNumber As Label
    Friend WithEvents btnSaveLevelInfo As Button
    Friend WithEvents cmsTreeMenu As ContextMenuStrip
    Friend WithEvents mnuAddBuilding As ToolStripMenuItem
    Friend WithEvents mnuAddLevel As ToolStripMenuItem
    Friend WithEvents mnuDelete As ToolStripMenuItem
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents nudLevelNumber As NumericUpDown
    Friend WithEvents lblPlanUnits As Label
    Friend WithEvents txtResUnits As TextBox
    Friend WithEvents dtpLastModified As DateTimePicker
    Friend WithEvents cboSalesman As ComboBox
    Friend WithEvents cboCustomer As ComboBox
    Friend WithEvents EditPSEToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents cboEstimator As ComboBox
    Friend WithEvents cboProjectType As ComboBox
    Friend WithEvents cboState As ComboBox
    Friend WithEvents mnuCopyLevels As ToolStripMenuItem
    Friend WithEvents mnuPasteLevels As ToolStripMenuItem
    Friend WithEvents btnDuplicateVersion As Button
    Friend WithEvents btnCreateVersion As Button
    Friend WithEvents cboVersion As ComboBox
    Friend WithEvents cboProjectEngineer As ComboBox
    Friend WithEvents cboProjectArchitect As ComboBox
    Friend WithEvents btnEditArchitect As Button
    Friend WithEvents btnAddArchitect As Button
    Friend WithEvents btnEditCustomer As Button
    Friend WithEvents btnAddCustomer As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents btnEditEngineer As Button
    Friend WithEvents btnAddEngineer As Button
    Friend WithEvents btnOpenPSE As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents btnIEOpen As Button
    Friend WithEvents mnuCopyBuilding As ToolStripMenuItem
    Friend WithEvents mnuPasteBuilding As ToolStripMenuItem
    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents txtRawRoofSPFPrice As TextBox
    Friend WithEvents txtRawFloorSPFPrice As TextBox
    Friend WithEvents btnDeleteProject As Button
    Friend WithEvents btnGenerateProjectReport As Button
    Friend WithEvents btnPreviewIncExc As Button
End Class