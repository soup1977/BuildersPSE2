<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmPriceLockDetail
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle8 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.grpMargins = New System.Windows.Forms.GroupBox()
        Me.nudOptionMargin = New System.Windows.Forms.NumericUpDown()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.nudAdjustedMargin = New System.Windows.Forms.NumericUpDown()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.nudBaseMgmtMargin = New System.Windows.Forms.NumericUpDown()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.grpRandomLengths = New System.Windows.Forms.GroupBox()
        Me.lblRLStatus = New System.Windows.Forms.Label()
        Me.btnCalculateFromRL = New System.Windows.Forms.Button()
        Me.btnManageRLImports = New System.Windows.Forms.Button()
        Me.cboCurrentRL = New System.Windows.Forms.ComboBox()
        Me.lblCurrentRL = New System.Windows.Forms.Label()
        Me.cboBaselineRL = New System.Windows.Forms.ComboBox()
        Me.lblBaselineRL = New System.Windows.Forms.Label()
        Me.btnChangeStatus = New System.Windows.Forms.Button()
        Me.cboStatus = New System.Windows.Forms.ComboBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.txtLockName = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.dtpLockDate = New System.Windows.Forms.DateTimePicker()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.lblCodeValue = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.lblSubdivisionValue = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.lblBuilderValue = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.tabControl = New System.Windows.Forms.TabControl()
        Me.tabComponents = New System.Windows.Forms.TabPage()
        Me.dgvComponents = New System.Windows.Forms.DataGridView()
        Me.ComponentPricingID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.FullDescription = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ProductTypeName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.IsAdder = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Cost = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CalculatedPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.FinalPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PriceSentToBuilder = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HasDiff = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PriceNote = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlCompButtons = New System.Windows.Forms.Panel()
        Me.btnImportMitek = New System.Windows.Forms.Button()
        Me.btnDeleteComponent = New System.Windows.Forms.Button()
        Me.btnEditComponent = New System.Windows.Forms.Button()
        Me.btnAddComponent = New System.Windows.Forms.Button()
        Me.tabMaterials = New System.Windows.Forms.TabPage()
        Me.dgvMaterials = New System.Windows.Forms.DataGridView()
        Me.MaterialPricingID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CategoryName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.RandomLengthsPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CalculatedPriceMat = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PriceSentToBuilderMat = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PctChangeFromPrevious = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.HasDiffMat = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PriceNoteMat = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlMatButtons = New System.Windows.Forms.Panel()
        Me.btnAddMaterial = New System.Windows.Forms.Button()
        Me.btnEditMaterial = New System.Windows.Forms.Button()
        Me.tabHistory = New System.Windows.Forms.TabPage()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.pnlHeader.SuspendLayout()
        Me.grpMargins.SuspendLayout()
        CType(Me.nudOptionMargin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudAdjustedMargin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudBaseMgmtMargin, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpRandomLengths.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.tabControl.SuspendLayout()
        Me.tabComponents.SuspendLayout()
        CType(Me.dgvComponents, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlCompButtons.SuspendLayout()
        Me.tabMaterials.SuspendLayout()
        CType(Me.dgvMaterials, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlMatButtons.SuspendLayout()
        Me.tabHistory.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlHeader
        '
        Me.pnlHeader.Controls.Add(Me.grpMargins)
        Me.pnlHeader.Controls.Add(Me.grpRandomLengths)
        Me.pnlHeader.Controls.Add(Me.btnChangeStatus)
        Me.pnlHeader.Controls.Add(Me.cboStatus)
        Me.pnlHeader.Controls.Add(Me.Label11)
        Me.pnlHeader.Controls.Add(Me.txtLockName)
        Me.pnlHeader.Controls.Add(Me.Label10)
        Me.pnlHeader.Controls.Add(Me.dtpLockDate)
        Me.pnlHeader.Controls.Add(Me.Label9)
        Me.pnlHeader.Controls.Add(Me.lblCodeValue)
        Me.pnlHeader.Controls.Add(Me.Label8)
        Me.pnlHeader.Controls.Add(Me.lblSubdivisionValue)
        Me.pnlHeader.Controls.Add(Me.Label7)
        Me.pnlHeader.Controls.Add(Me.lblBuilderValue)
        Me.pnlHeader.Controls.Add(Me.Label1)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlHeader.Size = New System.Drawing.Size(1084, 140)
        Me.pnlHeader.TabIndex = 0
        '
        'grpMargins
        '
        Me.grpMargins.Controls.Add(Me.nudOptionMargin)
        Me.grpMargins.Controls.Add(Me.Label14)
        Me.grpMargins.Controls.Add(Me.nudAdjustedMargin)
        Me.grpMargins.Controls.Add(Me.Label13)
        Me.grpMargins.Controls.Add(Me.nudBaseMgmtMargin)
        Me.grpMargins.Controls.Add(Me.Label12)
        Me.grpMargins.Location = New System.Drawing.Point(10, 70)
        Me.grpMargins.Name = "grpMargins"
        Me.grpMargins.Size = New System.Drawing.Size(500, 55)
        Me.grpMargins.TabIndex = 13
        Me.grpMargins.TabStop = False
        Me.grpMargins.Text = "Margins"
        '
        'nudOptionMargin
        '
        Me.nudOptionMargin.DecimalPlaces = 2
        Me.nudOptionMargin.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.nudOptionMargin.Location = New System.Drawing.Point(345, 20)
        Me.nudOptionMargin.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudOptionMargin.Name = "nudOptionMargin"
        Me.nudOptionMargin.Size = New System.Drawing.Size(60, 20)
        Me.nudOptionMargin.TabIndex = 5
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(295, 22)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(41, 13)
        Me.Label14.TabIndex = 4
        Me.Label14.Text = "Option:"
        '
        'nudAdjustedMargin
        '
        Me.nudAdjustedMargin.DecimalPlaces = 2
        Me.nudAdjustedMargin.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.nudAdjustedMargin.Location = New System.Drawing.Point(220, 20)
        Me.nudAdjustedMargin.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudAdjustedMargin.Name = "nudAdjustedMargin"
        Me.nudAdjustedMargin.Size = New System.Drawing.Size(60, 20)
        Me.nudAdjustedMargin.TabIndex = 3
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(160, 22)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(51, 13)
        Me.Label13.TabIndex = 2
        Me.Label13.Text = "Adjusted:"
        '
        'nudBaseMgmtMargin
        '
        Me.nudBaseMgmtMargin.DecimalPlaces = 2
        Me.nudBaseMgmtMargin.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.nudBaseMgmtMargin.Location = New System.Drawing.Point(85, 20)
        Me.nudBaseMgmtMargin.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudBaseMgmtMargin.Name = "nudBaseMgmtMargin"
        Me.nudBaseMgmtMargin.Size = New System.Drawing.Size(60, 20)
        Me.nudBaseMgmtMargin.TabIndex = 1
        Me.nudBaseMgmtMargin.Visible = False
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(10, 22)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(70, 13)
        Me.Label12.TabIndex = 0
        Me.Label12.Text = "Base MGMT:"
        Me.Label12.Visible = False
        '
        'grpRandomLengths
        '
        Me.grpRandomLengths.Controls.Add(Me.lblRLStatus)
        Me.grpRandomLengths.Controls.Add(Me.btnCalculateFromRL)
        Me.grpRandomLengths.Controls.Add(Me.btnManageRLImports)
        Me.grpRandomLengths.Controls.Add(Me.cboCurrentRL)
        Me.grpRandomLengths.Controls.Add(Me.lblCurrentRL)
        Me.grpRandomLengths.Controls.Add(Me.cboBaselineRL)
        Me.grpRandomLengths.Controls.Add(Me.lblBaselineRL)
        Me.grpRandomLengths.Location = New System.Drawing.Point(520, 70)
        Me.grpRandomLengths.Name = "grpRandomLengths"
        Me.grpRandomLengths.Size = New System.Drawing.Size(550, 55)
        Me.grpRandomLengths.TabIndex = 14
        Me.grpRandomLengths.TabStop = False
        Me.grpRandomLengths.Text = "Random Lengths Pricing"
        '
        'lblRLStatus
        '
        Me.lblRLStatus.AutoSize = True
        Me.lblRLStatus.ForeColor = System.Drawing.Color.Gray
        Me.lblRLStatus.Location = New System.Drawing.Point(65, 43)
        Me.lblRLStatus.Name = "lblRLStatus"
        Me.lblRLStatus.Size = New System.Drawing.Size(0, 13)
        Me.lblRLStatus.TabIndex = 6
        '
        'btnCalculateFromRL
        '
        Me.btnCalculateFromRL.Location = New System.Drawing.Point(455, 17)
        Me.btnCalculateFromRL.Name = "btnCalculateFromRL"
        Me.btnCalculateFromRL.Size = New System.Drawing.Size(85, 23)
        Me.btnCalculateFromRL.TabIndex = 5
        Me.btnCalculateFromRL.Text = "Calculate"
        Me.btnCalculateFromRL.UseVisualStyleBackColor = True
        '
        'btnManageRLImports
        '
        Me.btnManageRLImports.Font = New System.Drawing.Font("Microsoft Sans Serif", 7.0!)
        Me.btnManageRLImports.Location = New System.Drawing.Point(425, 17)
        Me.btnManageRLImports.Name = "btnManageRLImports"
        Me.btnManageRLImports.Size = New System.Drawing.Size(24, 23)
        Me.btnManageRLImports.TabIndex = 4
        Me.btnManageRLImports.Text = "..."
        Me.btnManageRLImports.UseVisualStyleBackColor = True
        '
        'cboCurrentRL
        '
        Me.cboCurrentRL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCurrentRL.FormattingEnabled = True
        Me.cboCurrentRL.Location = New System.Drawing.Point(270, 19)
        Me.cboCurrentRL.Name = "cboCurrentRL"
        Me.cboCurrentRL.Size = New System.Drawing.Size(150, 21)
        Me.cboCurrentRL.TabIndex = 3
        '
        'lblCurrentRL
        '
        Me.lblCurrentRL.AutoSize = True
        Me.lblCurrentRL.Location = New System.Drawing.Point(220, 22)
        Me.lblCurrentRL.Name = "lblCurrentRL"
        Me.lblCurrentRL.Size = New System.Drawing.Size(44, 13)
        Me.lblCurrentRL.TabIndex = 2
        Me.lblCurrentRL.Text = "Current:"
        '
        'cboBaselineRL
        '
        Me.cboBaselineRL.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboBaselineRL.FormattingEnabled = True
        Me.cboBaselineRL.Location = New System.Drawing.Point(65, 19)
        Me.cboBaselineRL.Name = "cboBaselineRL"
        Me.cboBaselineRL.Size = New System.Drawing.Size(150, 21)
        Me.cboBaselineRL.TabIndex = 1
        '
        'lblBaselineRL
        '
        Me.lblBaselineRL.AutoSize = True
        Me.lblBaselineRL.Location = New System.Drawing.Point(10, 22)
        Me.lblBaselineRL.Name = "lblBaselineRL"
        Me.lblBaselineRL.Size = New System.Drawing.Size(50, 13)
        Me.lblBaselineRL.TabIndex = 0
        Me.lblBaselineRL.Text = "Baseline:"
        '
        'btnChangeStatus
        '
        Me.btnChangeStatus.Location = New System.Drawing.Point(680, 38)
        Me.btnChangeStatus.Name = "btnChangeStatus"
        Me.btnChangeStatus.Size = New System.Drawing.Size(75, 23)
        Me.btnChangeStatus.TabIndex = 12
        Me.btnChangeStatus.Text = "Change..."
        Me.btnChangeStatus.UseVisualStyleBackColor = True
        '
        'cboStatus
        '
        Me.cboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboStatus.Enabled = False
        Me.cboStatus.FormattingEnabled = True
        Me.cboStatus.Location = New System.Drawing.Point(570, 39)
        Me.cboStatus.Name = "cboStatus"
        Me.cboStatus.Size = New System.Drawing.Size(100, 21)
        Me.cboStatus.TabIndex = 11
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(520, 42)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(40, 13)
        Me.Label11.TabIndex = 10
        Me.Label11.Text = "Status:"
        '
        'txtLockName
        '
        Me.txtLockName.Location = New System.Drawing.Point(295, 39)
        Me.txtLockName.Name = "txtLockName"
        Me.txtLockName.Size = New System.Drawing.Size(200, 20)
        Me.txtLockName.TabIndex = 9
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(220, 42)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(65, 13)
        Me.Label10.TabIndex = 8
        Me.Label10.Text = "Lock Name:"
        '
        'dtpLockDate
        '
        Me.dtpLockDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpLockDate.Location = New System.Drawing.Point(80, 39)
        Me.dtpLockDate.Name = "dtpLockDate"
        Me.dtpLockDate.Size = New System.Drawing.Size(120, 20)
        Me.dtpLockDate.TabIndex = 7
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(10, 42)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(60, 13)
        Me.Label9.TabIndex = 6
        Me.Label9.Text = "Lock Date:"
        '
        'lblCodeValue
        '
        Me.lblCodeValue.AutoSize = True
        Me.lblCodeValue.Location = New System.Drawing.Point(595, 12)
        Me.lblCodeValue.Name = "lblCodeValue"
        Me.lblCodeValue.Size = New System.Drawing.Size(0, 13)
        Me.lblCodeValue.TabIndex = 5
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(550, 12)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(40, 13)
        Me.Label8.TabIndex = 4
        Me.Label8.Text = "Code:"
        '
        'lblSubdivisionValue
        '
        Me.lblSubdivisionValue.AutoSize = True
        Me.lblSubdivisionValue.Location = New System.Drawing.Point(330, 12)
        Me.lblSubdivisionValue.Name = "lblSubdivisionValue"
        Me.lblSubdivisionValue.Size = New System.Drawing.Size(0, 13)
        Me.lblSubdivisionValue.TabIndex = 3
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(250, 12)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(76, 13)
        Me.Label7.TabIndex = 2
        Me.Label7.Text = "Subdivision:"
        '
        'lblBuilderValue
        '
        Me.lblBuilderValue.AutoSize = True
        Me.lblBuilderValue.Location = New System.Drawing.Point(70, 12)
        Me.lblBuilderValue.Name = "lblBuilderValue"
        Me.lblBuilderValue.Size = New System.Drawing.Size(0, 13)
        Me.lblBuilderValue.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(10, 12)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(50, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Builder:"
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Controls.Add(Me.btnSave)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Location = New System.Drawing.Point(0, 650)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlButtons.Size = New System.Drawing.Size(1084, 50)
        Me.pnlButtons.TabIndex = 1
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.Location = New System.Drawing.Point(994, 10)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(80, 30)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Close"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSave.Location = New System.Drawing.Point(884, 10)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(100, 30)
        Me.btnSave.TabIndex = 0
        Me.btnSave.Text = "Save Changes"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'tabControl
        '
        Me.tabControl.Controls.Add(Me.tabComponents)
        Me.tabControl.Controls.Add(Me.tabMaterials)
        Me.tabControl.Controls.Add(Me.tabHistory)
        Me.tabControl.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tabControl.Location = New System.Drawing.Point(0, 140)
        Me.tabControl.Name = "tabControl"
        Me.tabControl.SelectedIndex = 0
        Me.tabControl.Size = New System.Drawing.Size(1084, 510)
        Me.tabControl.TabIndex = 2
        '
        'tabComponents
        '
        Me.tabComponents.Controls.Add(Me.dgvComponents)
        Me.tabComponents.Controls.Add(Me.pnlCompButtons)
        Me.tabComponents.Location = New System.Drawing.Point(4, 22)
        Me.tabComponents.Name = "tabComponents"
        Me.tabComponents.Padding = New System.Windows.Forms.Padding(3)
        Me.tabComponents.Size = New System.Drawing.Size(1076, 484)
        Me.tabComponents.TabIndex = 0
        Me.tabComponents.Text = "Component Pricing"
        Me.tabComponents.UseVisualStyleBackColor = True
        '
        'dgvComponents
        '
        Me.dgvComponents.AllowUserToAddRows = False
        Me.dgvComponents.AllowUserToDeleteRows = False
        Me.dgvComponents.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvComponents.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvComponents.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvComponents.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.ComponentPricingID, Me.FullDescription, Me.ProductTypeName, Me.IsAdder, Me.Cost, Me.CalculatedPrice, Me.FinalPrice, Me.PriceSentToBuilder, Me.HasDiff, Me.PriceNote})
        Me.dgvComponents.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvComponents.Location = New System.Drawing.Point(3, 43)
        Me.dgvComponents.MultiSelect = False
        Me.dgvComponents.Name = "dgvComponents"
        Me.dgvComponents.ReadOnly = True
        Me.dgvComponents.RowHeadersVisible = False
        Me.dgvComponents.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvComponents.Size = New System.Drawing.Size(1070, 438)
        Me.dgvComponents.TabIndex = 1
        '
        'ComponentPricingID
        '
        Me.ComponentPricingID.DataPropertyName = "ComponentPricingID"
        Me.ComponentPricingID.HeaderText = "ComponentPricingID"
        Me.ComponentPricingID.Name = "ComponentPricingID"
        Me.ComponentPricingID.ReadOnly = True
        Me.ComponentPricingID.Visible = False
        '
        'FullDescription
        '
        Me.FullDescription.DataPropertyName = "FullDescription"
        Me.FullDescription.HeaderText = "Plan / Elevation / Option"
        Me.FullDescription.MinimumWidth = 200
        Me.FullDescription.Name = "FullDescription"
        Me.FullDescription.ReadOnly = True
        '
        'ProductTypeName
        '
        Me.ProductTypeName.DataPropertyName = "ProductTypeName"
        Me.ProductTypeName.HeaderText = "Type"
        Me.ProductTypeName.Name = "ProductTypeName"
        Me.ProductTypeName.ReadOnly = True
        '
        'IsAdder
        '
        Me.IsAdder.DataPropertyName = "IsAdder"
        Me.IsAdder.HeaderText = "Adder"
        Me.IsAdder.Name = "IsAdder"
        Me.IsAdder.ReadOnly = True
        '
        'Cost
        '
        Me.Cost.DataPropertyName = "Cost"
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle1.Format = "C2"
        Me.Cost.DefaultCellStyle = DataGridViewCellStyle1
        Me.Cost.HeaderText = "Cost"
        Me.Cost.Name = "Cost"
        Me.Cost.ReadOnly = True
        '
        'CalculatedPrice
        '
        Me.CalculatedPrice.DataPropertyName = "CalculatedPrice"
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle2.Format = "C2"
        Me.CalculatedPrice.DefaultCellStyle = DataGridViewCellStyle2
        Me.CalculatedPrice.HeaderText = "Calculated"
        Me.CalculatedPrice.Name = "CalculatedPrice"
        Me.CalculatedPrice.ReadOnly = True
        '
        'FinalPrice
        '
        Me.FinalPrice.DataPropertyName = "FinalPrice"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle3.Format = "C2"
        Me.FinalPrice.DefaultCellStyle = DataGridViewCellStyle3
        Me.FinalPrice.HeaderText = "Final"
        Me.FinalPrice.Name = "FinalPrice"
        Me.FinalPrice.ReadOnly = True
        '
        'PriceSentToBuilder
        '
        Me.PriceSentToBuilder.DataPropertyName = "PriceSentToBuilder"
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle4.Format = "C2"
        Me.PriceSentToBuilder.DefaultCellStyle = DataGridViewCellStyle4
        Me.PriceSentToBuilder.HeaderText = "Builder"
        Me.PriceSentToBuilder.Name = "PriceSentToBuilder"
        Me.PriceSentToBuilder.ReadOnly = True
        '
        'HasDiff
        '
        Me.HasDiff.HeaderText = ""
        Me.HasDiff.Name = "HasDiff"
        Me.HasDiff.ReadOnly = True
        '
        'PriceNote
        '
        Me.PriceNote.DataPropertyName = "PriceNote"
        Me.PriceNote.HeaderText = "Note"
        Me.PriceNote.Name = "PriceNote"
        Me.PriceNote.ReadOnly = True
        '
        'pnlCompButtons
        '
        Me.pnlCompButtons.Controls.Add(Me.btnImportMitek)
        Me.pnlCompButtons.Controls.Add(Me.btnDeleteComponent)
        Me.pnlCompButtons.Controls.Add(Me.btnEditComponent)
        Me.pnlCompButtons.Controls.Add(Me.btnAddComponent)
        Me.pnlCompButtons.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlCompButtons.Location = New System.Drawing.Point(3, 3)
        Me.pnlCompButtons.Name = "pnlCompButtons"
        Me.pnlCompButtons.Size = New System.Drawing.Size(1070, 40)
        Me.pnlCompButtons.TabIndex = 0
        '
        'btnImportMitek
        '
        Me.btnImportMitek.Location = New System.Drawing.Point(300, 8)
        Me.btnImportMitek.Name = "btnImportMitek"
        Me.btnImportMitek.Size = New System.Drawing.Size(130, 23)
        Me.btnImportMitek.TabIndex = 3
        Me.btnImportMitek.Text = "Import MiTek CSV..."
        Me.btnImportMitek.UseVisualStyleBackColor = True
        '
        'btnDeleteComponent
        '
        Me.btnDeleteComponent.Location = New System.Drawing.Point(210, 8)
        Me.btnDeleteComponent.Name = "btnDeleteComponent"
        Me.btnDeleteComponent.Size = New System.Drawing.Size(70, 23)
        Me.btnDeleteComponent.TabIndex = 2
        Me.btnDeleteComponent.Text = "Delete"
        Me.btnDeleteComponent.UseVisualStyleBackColor = True
        '
        'btnEditComponent
        '
        Me.btnEditComponent.Location = New System.Drawing.Point(130, 8)
        Me.btnEditComponent.Name = "btnEditComponent"
        Me.btnEditComponent.Size = New System.Drawing.Size(70, 23)
        Me.btnEditComponent.TabIndex = 1
        Me.btnEditComponent.Text = "Edit"
        Me.btnEditComponent.UseVisualStyleBackColor = True
        '
        'btnAddComponent
        '
        Me.btnAddComponent.Location = New System.Drawing.Point(10, 8)
        Me.btnAddComponent.Name = "btnAddComponent"
        Me.btnAddComponent.Size = New System.Drawing.Size(110, 23)
        Me.btnAddComponent.TabIndex = 0
        Me.btnAddComponent.Text = "Add Component"
        Me.btnAddComponent.UseVisualStyleBackColor = True
        '
        'tabMaterials
        '
        Me.tabMaterials.Controls.Add(Me.dgvMaterials)
        Me.tabMaterials.Controls.Add(Me.pnlMatButtons)
        Me.tabMaterials.Location = New System.Drawing.Point(4, 22)
        Me.tabMaterials.Name = "tabMaterials"
        Me.tabMaterials.Padding = New System.Windows.Forms.Padding(3)
        Me.tabMaterials.Size = New System.Drawing.Size(1076, 484)
        Me.tabMaterials.TabIndex = 1
        Me.tabMaterials.Text = "Material Pricing"
        Me.tabMaterials.UseVisualStyleBackColor = True
        '
        'dgvMaterials
        '
        Me.dgvMaterials.AllowUserToAddRows = False
        Me.dgvMaterials.AllowUserToDeleteRows = False
        Me.dgvMaterials.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvMaterials.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvMaterials.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMaterials.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.MaterialPricingID, Me.CategoryName, Me.RandomLengthsPrice, Me.CalculatedPriceMat, Me.PriceSentToBuilderMat, Me.PctChangeFromPrevious, Me.HasDiffMat, Me.PriceNoteMat})
        Me.dgvMaterials.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvMaterials.Location = New System.Drawing.Point(3, 43)
        Me.dgvMaterials.MultiSelect = False
        Me.dgvMaterials.Name = "dgvMaterials"
        Me.dgvMaterials.ReadOnly = True
        Me.dgvMaterials.RowHeadersVisible = False
        Me.dgvMaterials.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvMaterials.Size = New System.Drawing.Size(1070, 438)
        Me.dgvMaterials.TabIndex = 1
        '
        'MaterialPricingID
        '
        Me.MaterialPricingID.DataPropertyName = "MaterialPricingID"
        Me.MaterialPricingID.HeaderText = "MaterialPricingID"
        Me.MaterialPricingID.Name = "MaterialPricingID"
        Me.MaterialPricingID.ReadOnly = True
        Me.MaterialPricingID.Visible = False
        '
        'CategoryName
        '
        Me.CategoryName.DataPropertyName = "CategoryName"
        Me.CategoryName.HeaderText = "Category"
        Me.CategoryName.Name = "CategoryName"
        Me.CategoryName.ReadOnly = True
        '
        'RandomLengthsPrice
        '
        Me.RandomLengthsPrice.DataPropertyName = "RandomLengthsPrice"
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle5.Format = "N2"
        Me.RandomLengthsPrice.DefaultCellStyle = DataGridViewCellStyle5
        Me.RandomLengthsPrice.HeaderText = "RL Price"
        Me.RandomLengthsPrice.Name = "RandomLengthsPrice"
        Me.RandomLengthsPrice.ReadOnly = True
        '
        'CalculatedPriceMat
        '
        Me.CalculatedPriceMat.DataPropertyName = "CalculatedPrice"
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle6.Format = "N4"
        Me.CalculatedPriceMat.DefaultCellStyle = DataGridViewCellStyle6
        Me.CalculatedPriceMat.HeaderText = "Calculated"
        Me.CalculatedPriceMat.Name = "CalculatedPriceMat"
        Me.CalculatedPriceMat.ReadOnly = True
        '
        'PriceSentToBuilderMat
        '
        Me.PriceSentToBuilderMat.DataPropertyName = "PriceSentToBuilder"
        DataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle7.Format = "N4"
        Me.PriceSentToBuilderMat.DefaultCellStyle = DataGridViewCellStyle7
        Me.PriceSentToBuilderMat.HeaderText = "Builder"
        Me.PriceSentToBuilderMat.Name = "PriceSentToBuilderMat"
        Me.PriceSentToBuilderMat.ReadOnly = True
        '
        'PctChangeFromPrevious
        '
        Me.PctChangeFromPrevious.DataPropertyName = "PctChangeFromPrevious"
        DataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle8.Format = "P1"
        Me.PctChangeFromPrevious.DefaultCellStyle = DataGridViewCellStyle8
        Me.PctChangeFromPrevious.HeaderText = "% Change"
        Me.PctChangeFromPrevious.Name = "PctChangeFromPrevious"
        Me.PctChangeFromPrevious.ReadOnly = True
        '
        'HasDiffMat
        '
        Me.HasDiffMat.HeaderText = ""
        Me.HasDiffMat.Name = "HasDiffMat"
        Me.HasDiffMat.ReadOnly = True
        '
        'PriceNoteMat
        '
        Me.PriceNoteMat.DataPropertyName = "PriceNote"
        Me.PriceNoteMat.HeaderText = "Note"
        Me.PriceNoteMat.Name = "PriceNoteMat"
        Me.PriceNoteMat.ReadOnly = True
        '
        'pnlMatButtons
        '
        Me.pnlMatButtons.Controls.Add(Me.btnAddMaterial)
        Me.pnlMatButtons.Controls.Add(Me.btnEditMaterial)
        Me.pnlMatButtons.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlMatButtons.Location = New System.Drawing.Point(3, 3)
        Me.pnlMatButtons.Name = "pnlMatButtons"
        Me.pnlMatButtons.Size = New System.Drawing.Size(1070, 40)
        Me.pnlMatButtons.TabIndex = 0
        '
        'btnAddMaterial
        '
        Me.btnAddMaterial.Location = New System.Drawing.Point(12, 11)
        Me.btnAddMaterial.Name = "btnAddMaterial"
        Me.btnAddMaterial.Size = New System.Drawing.Size(76, 22)
        Me.btnAddMaterial.TabIndex = 1
        Me.btnAddMaterial.Text = "Add Material"
        Me.btnAddMaterial.UseVisualStyleBackColor = True
        '
        'btnEditMaterial
        '
        Me.btnEditMaterial.Location = New System.Drawing.Point(114, 11)
        Me.btnEditMaterial.Name = "btnEditMaterial"
        Me.btnEditMaterial.Size = New System.Drawing.Size(100, 23)
        Me.btnEditMaterial.TabIndex = 0
        Me.btnEditMaterial.Text = "Edit Selected"
        Me.btnEditMaterial.UseVisualStyleBackColor = True
        '
        'tabHistory
        '
        Me.tabHistory.Controls.Add(Me.Label2)
        Me.tabHistory.Location = New System.Drawing.Point(4, 22)
        Me.tabHistory.Name = "tabHistory"
        Me.tabHistory.Size = New System.Drawing.Size(1076, 484)
        Me.tabHistory.TabIndex = 2
        Me.tabHistory.Text = "History"
        Me.tabHistory.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(20, 20)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(209, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Price change history will be displayed here."
        '
        'frmPriceLockDetail
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1084, 700)
        Me.Controls.Add(Me.tabControl)
        Me.Controls.Add(Me.pnlButtons)
        Me.Controls.Add(Me.pnlHeader)
        Me.MinimumSize = New System.Drawing.Size(900, 600)
        Me.Name = "frmPriceLockDetail"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Price Lock Details"
        Me.pnlHeader.ResumeLayout(False)
        Me.pnlHeader.PerformLayout()
        Me.grpMargins.ResumeLayout(False)
        Me.grpMargins.PerformLayout()
        CType(Me.nudOptionMargin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudAdjustedMargin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudBaseMgmtMargin, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpRandomLengths.ResumeLayout(False)
        Me.grpRandomLengths.PerformLayout()
        Me.pnlButtons.ResumeLayout(False)
        Me.tabControl.ResumeLayout(False)
        Me.tabComponents.ResumeLayout(False)
        CType(Me.dgvComponents, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlCompButtons.ResumeLayout(False)
        Me.tabMaterials.ResumeLayout(False)
        CType(Me.dgvMaterials, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlMatButtons.ResumeLayout(False)
        Me.tabHistory.ResumeLayout(False)
        Me.tabHistory.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlHeader As Panel
    Friend WithEvents grpMargins As GroupBox
    Friend WithEvents nudOptionMargin As NumericUpDown
    Friend WithEvents Label14 As Label
    Friend WithEvents nudAdjustedMargin As NumericUpDown
    Friend WithEvents Label13 As Label
    Friend WithEvents nudBaseMgmtMargin As NumericUpDown
    Friend WithEvents Label12 As Label
    Friend WithEvents btnChangeStatus As Button
    Friend WithEvents cboStatus As ComboBox
    Friend WithEvents Label11 As Label
    Friend WithEvents txtLockName As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents dtpLockDate As DateTimePicker
    Friend WithEvents Label9 As Label
    Friend WithEvents lblCodeValue As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents lblSubdivisionValue As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents lblBuilderValue As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents pnlButtons As Panel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents tabControl As TabControl
    Friend WithEvents tabComponents As TabPage
    Friend WithEvents dgvComponents As DataGridView
    Friend WithEvents pnlCompButtons As Panel
    Friend WithEvents btnImportMitek As Button
    Friend WithEvents btnDeleteComponent As Button
    Friend WithEvents btnEditComponent As Button
    Friend WithEvents btnAddComponent As Button
    Friend WithEvents tabMaterials As TabPage
    Friend WithEvents dgvMaterials As DataGridView
    Friend WithEvents pnlMatButtons As Panel
    Friend WithEvents btnEditMaterial As Button
    Friend WithEvents tabHistory As TabPage
    Friend WithEvents Label2 As Label
    Friend WithEvents ComponentPricingID As DataGridViewTextBoxColumn
    Friend WithEvents FullDescription As DataGridViewTextBoxColumn
    Friend WithEvents ProductTypeName As DataGridViewTextBoxColumn
    Friend WithEvents IsAdder As DataGridViewTextBoxColumn
    Friend WithEvents Cost As DataGridViewTextBoxColumn
    Friend WithEvents CalculatedPrice As DataGridViewTextBoxColumn
    Friend WithEvents FinalPrice As DataGridViewTextBoxColumn
    Friend WithEvents PriceSentToBuilder As DataGridViewTextBoxColumn
    Friend WithEvents HasDiff As DataGridViewTextBoxColumn
    Friend WithEvents PriceNote As DataGridViewTextBoxColumn
    Friend WithEvents MaterialPricingID As DataGridViewTextBoxColumn
    Friend WithEvents CategoryName As DataGridViewTextBoxColumn
    Friend WithEvents RandomLengthsPrice As DataGridViewTextBoxColumn
    Friend WithEvents CalculatedPriceMat As DataGridViewTextBoxColumn
    Friend WithEvents PriceSentToBuilderMat As DataGridViewTextBoxColumn
    Friend WithEvents PctChangeFromPrevious As DataGridViewTextBoxColumn
    Friend WithEvents HasDiffMat As DataGridViewTextBoxColumn
    Friend WithEvents PriceNoteMat As DataGridViewTextBoxColumn
    Friend WithEvents btnAddMaterial As Button
    Friend WithEvents grpRandomLengths As GroupBox
    Friend WithEvents lblBaselineRL As Label
    Friend WithEvents cboBaselineRL As ComboBox
    Friend WithEvents lblCurrentRL As Label
    Friend WithEvents cboCurrentRL As ComboBox
    Friend WithEvents btnManageRLImports As Button
    Friend WithEvents btnCalculateFromRL As Button
    Friend WithEvents lblRLStatus As Label
End Class
