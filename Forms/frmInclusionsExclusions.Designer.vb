<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmInclusionsExclusions
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
        Me.tabControl = New System.Windows.Forms.TabControl()
        Me.tabDesignInfo = New System.Windows.Forms.TabPage()
        Me.cboCorridorRimRibbon = New System.Windows.Forms.ComboBox()
        Me.cboIntRimRibbon = New System.Windows.Forms.ComboBox()
        Me.cboExtRimRibbon = New System.Windows.Forms.ComboBox()
        Me.cmbCorridorWall = New System.Windows.Forms.ComboBox()
        Me.lblCorridorWall = New System.Windows.Forms.Label()
        Me.cmbIntWall = New System.Windows.Forms.ComboBox()
        Me.lblIntWall = New System.Windows.Forms.Label()
        Me.lblExtRim = New System.Windows.Forms.Label()
        Me.cmbExtWall = New System.Windows.Forms.ComboBox()
        Me.lblExtWall = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.lblCorridor = New System.Windows.Forms.Label()
        Me.lblFloor = New System.Windows.Forms.Label()
        Me.lblCategory = New System.Windows.Forms.Label()
        Me.lblTCLL = New System.Windows.Forms.Label()
        Me.lblTCDL = New System.Windows.Forms.Label()
        Me.lblBCLL = New System.Windows.Forms.Label()
        Me.lblBCDL = New System.Windows.Forms.Label()
        Me.lblOCSpacing = New System.Windows.Forms.Label()
        Me.lblLiveLoadDeflection = New System.Windows.Forms.Label()
        Me.lblTotalLoadDeflection = New System.Windows.Forms.Label()
        Me.lblAbsolute = New System.Windows.Forms.Label()
        Me.lblDeflection = New System.Windows.Forms.Label()
        Me.lblRoof = New System.Windows.Forms.Label()
        Me.cmbTCLL_Roof = New System.Windows.Forms.ComboBox()
        Me.cmbTCDL_Roof = New System.Windows.Forms.ComboBox()
        Me.cmbBCLL_Roof = New System.Windows.Forms.ComboBox()
        Me.cmbBCDL_Roof = New System.Windows.Forms.ComboBox()
        Me.cmbOCSpacing_Roof = New System.Windows.Forms.ComboBox()
        Me.cmbLiveLoadDeflection_Roof = New System.Windows.Forms.ComboBox()
        Me.cmbTotalLoadDeflection_Roof = New System.Windows.Forms.ComboBox()
        Me.cmbAbsolute_Roof = New System.Windows.Forms.ComboBox()
        Me.cmbDeflection_Roof = New System.Windows.Forms.ComboBox()
        Me.cmbTCLL_Floor = New System.Windows.Forms.ComboBox()
        Me.cmbTCDL_Floor = New System.Windows.Forms.ComboBox()
        Me.cmbBCLL_Floor = New System.Windows.Forms.ComboBox()
        Me.cmbBCDL_Floor = New System.Windows.Forms.ComboBox()
        Me.cmbOCSpacing_Floor = New System.Windows.Forms.ComboBox()
        Me.cmbLiveLoadDeflection_Floor = New System.Windows.Forms.ComboBox()
        Me.cmbTotalLoadDeflection_Floor = New System.Windows.Forms.ComboBox()
        Me.cmbAbsolute_Floor = New System.Windows.Forms.ComboBox()
        Me.cmbDeflection_Floor = New System.Windows.Forms.ComboBox()
        Me.cmbTCLL_Corridor = New System.Windows.Forms.ComboBox()
        Me.cmbTCDL_Corridor = New System.Windows.Forms.ComboBox()
        Me.cmbBCLL_Corridor = New System.Windows.Forms.ComboBox()
        Me.cmbBCDL_Corridor = New System.Windows.Forms.ComboBox()
        Me.cmbOCSpacing_Corridor = New System.Windows.Forms.ComboBox()
        Me.cmbLiveLoadDeflection_Corridor = New System.Windows.Forms.ComboBox()
        Me.cmbTotalLoadDeflection_Corridor = New System.Windows.Forms.ComboBox()
        Me.cmbAbsolute_Corridor = New System.Windows.Forms.ComboBox()
        Me.cmbDeflection_Corridor = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtHeelHeights = New System.Windows.Forms.TextBox()
        Me.lblWallHeights = New System.Windows.Forms.Label()
        Me.txtWallHeights = New System.Windows.Forms.TextBox()
        Me.lblFloorDepths = New System.Windows.Forms.Label()
        Me.txtFloorDepths = New System.Windows.Forms.TextBox()
        Me.lblRoofPitches = New System.Windows.Forms.Label()
        Me.txtRoofPitches = New System.Windows.Forms.TextBox()
        Me.lblOccupancy = New System.Windows.Forms.Label()
        Me.cmbOccupancy = New System.Windows.Forms.ComboBox()
        Me.lblSnowLoad = New System.Windows.Forms.Label()
        Me.cmbSnowLoad = New System.Windows.Forms.ComboBox()
        Me.lblWindSpeed = New System.Windows.Forms.Label()
        Me.cmbWindSpeed = New System.Windows.Forms.ComboBox()
        Me.lblExposure = New System.Windows.Forms.Label()
        Me.cmbExposure = New System.Windows.Forms.ComboBox()
        Me.lblImportance = New System.Windows.Forms.Label()
        Me.cmbImportance = New System.Windows.Forms.ComboBox()
        Me.lblBuildingCode = New System.Windows.Forms.Label()
        Me.cmbBuildingCode = New System.Windows.Forms.ComboBox()
        Me.tabRoofItems = New System.Windows.Forms.TabPage()
        Me.dgvRoofItems = New System.Windows.Forms.DataGridView()
        Me.colRoofKN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colRoofDesc = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colRoofIncluded = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.colRoofExcluded = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.colRoofOptional = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.tabFloorItems = New System.Windows.Forms.TabPage()
        Me.dgvFloorItems = New System.Windows.Forms.DataGridView()
        Me.colFloorKN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colFloorDesc = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colFloorIncluded = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.colFloorExcluded = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.colFloorOptional = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.tabWallItems = New System.Windows.Forms.TabPage()
        Me.dgvWallItems = New System.Windows.Forms.DataGridView()
        Me.colWallKN = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colWallDesc = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colWallIncluded = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.colWallExcluded = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.colWallOptional = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.tabNotes = New System.Windows.Forms.TabPage()
        Me.txtGeneralNotes = New System.Windows.Forms.TextBox()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.tabControl.SuspendLayout()
        Me.tabDesignInfo.SuspendLayout()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.tabRoofItems.SuspendLayout()
        CType(Me.dgvRoofItems, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabFloorItems.SuspendLayout()
        CType(Me.dgvFloorItems, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabWallItems.SuspendLayout()
        CType(Me.dgvWallItems, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabNotes.SuspendLayout()
        Me.SuspendLayout()
        '
        'tabControl
        '
        Me.tabControl.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tabControl.Controls.Add(Me.tabDesignInfo)
        Me.tabControl.Controls.Add(Me.tabRoofItems)
        Me.tabControl.Controls.Add(Me.tabFloorItems)
        Me.tabControl.Controls.Add(Me.tabWallItems)
        Me.tabControl.Controls.Add(Me.tabNotes)
        Me.tabControl.Location = New System.Drawing.Point(0, 0)
        Me.tabControl.Name = "tabControl"
        Me.tabControl.SelectedIndex = 0
        Me.tabControl.Size = New System.Drawing.Size(900, 540)
        Me.tabControl.TabIndex = 0
        '
        'tabDesignInfo
        '
        Me.tabDesignInfo.Controls.Add(Me.cboCorridorRimRibbon)
        Me.tabDesignInfo.Controls.Add(Me.cboIntRimRibbon)
        Me.tabDesignInfo.Controls.Add(Me.cboExtRimRibbon)
        Me.tabDesignInfo.Controls.Add(Me.cmbCorridorWall)
        Me.tabDesignInfo.Controls.Add(Me.lblCorridorWall)
        Me.tabDesignInfo.Controls.Add(Me.cmbIntWall)
        Me.tabDesignInfo.Controls.Add(Me.lblIntWall)
        Me.tabDesignInfo.Controls.Add(Me.lblExtRim)
        Me.tabDesignInfo.Controls.Add(Me.cmbExtWall)
        Me.tabDesignInfo.Controls.Add(Me.lblExtWall)
        Me.tabDesignInfo.Controls.Add(Me.TableLayoutPanel1)
        Me.tabDesignInfo.Controls.Add(Me.Label1)
        Me.tabDesignInfo.Controls.Add(Me.txtHeelHeights)
        Me.tabDesignInfo.Controls.Add(Me.lblWallHeights)
        Me.tabDesignInfo.Controls.Add(Me.txtWallHeights)
        Me.tabDesignInfo.Controls.Add(Me.lblFloorDepths)
        Me.tabDesignInfo.Controls.Add(Me.txtFloorDepths)
        Me.tabDesignInfo.Controls.Add(Me.lblRoofPitches)
        Me.tabDesignInfo.Controls.Add(Me.txtRoofPitches)
        Me.tabDesignInfo.Controls.Add(Me.lblOccupancy)
        Me.tabDesignInfo.Controls.Add(Me.cmbOccupancy)
        Me.tabDesignInfo.Controls.Add(Me.lblSnowLoad)
        Me.tabDesignInfo.Controls.Add(Me.cmbSnowLoad)
        Me.tabDesignInfo.Controls.Add(Me.lblWindSpeed)
        Me.tabDesignInfo.Controls.Add(Me.cmbWindSpeed)
        Me.tabDesignInfo.Controls.Add(Me.lblExposure)
        Me.tabDesignInfo.Controls.Add(Me.cmbExposure)
        Me.tabDesignInfo.Controls.Add(Me.lblImportance)
        Me.tabDesignInfo.Controls.Add(Me.cmbImportance)
        Me.tabDesignInfo.Controls.Add(Me.lblBuildingCode)
        Me.tabDesignInfo.Controls.Add(Me.cmbBuildingCode)
        Me.tabDesignInfo.Location = New System.Drawing.Point(4, 22)
        Me.tabDesignInfo.Name = "tabDesignInfo"
        Me.tabDesignInfo.Padding = New System.Windows.Forms.Padding(3)
        Me.tabDesignInfo.Size = New System.Drawing.Size(892, 514)
        Me.tabDesignInfo.TabIndex = 0
        Me.tabDesignInfo.Text = "Design Info"
        Me.tabDesignInfo.UseVisualStyleBackColor = True
        '
        'cboCorridorRimRibbon
        '
        Me.cboCorridorRimRibbon.FormattingEnabled = True
        Me.cboCorridorRimRibbon.Items.AddRange(New Object() {"Rim", "Ribbon"})
        Me.cboCorridorRimRibbon.Location = New System.Drawing.Point(287, 387)
        Me.cboCorridorRimRibbon.Name = "cboCorridorRimRibbon"
        Me.cboCorridorRimRibbon.Size = New System.Drawing.Size(121, 21)
        Me.cboCorridorRimRibbon.TabIndex = 43
        '
        'cboIntRimRibbon
        '
        Me.cboIntRimRibbon.FormattingEnabled = True
        Me.cboIntRimRibbon.Items.AddRange(New Object() {"Rim", "Ribbon"})
        Me.cboIntRimRibbon.Location = New System.Drawing.Point(287, 360)
        Me.cboIntRimRibbon.Name = "cboIntRimRibbon"
        Me.cboIntRimRibbon.Size = New System.Drawing.Size(121, 21)
        Me.cboIntRimRibbon.TabIndex = 41
        '
        'cboExtRimRibbon
        '
        Me.cboExtRimRibbon.FormattingEnabled = True
        Me.cboExtRimRibbon.Items.AddRange(New Object() {"Rim", "Ribbon"})
        Me.cboExtRimRibbon.Location = New System.Drawing.Point(287, 333)
        Me.cboExtRimRibbon.Name = "cboExtRimRibbon"
        Me.cboExtRimRibbon.Size = New System.Drawing.Size(121, 21)
        Me.cboExtRimRibbon.TabIndex = 39
        '
        'cmbCorridorWall
        '
        Me.cmbCorridorWall.FormattingEnabled = True
        Me.cmbCorridorWall.Location = New System.Drawing.Point(103, 387)
        Me.cmbCorridorWall.Name = "cmbCorridorWall"
        Me.cmbCorridorWall.Size = New System.Drawing.Size(121, 21)
        Me.cmbCorridorWall.TabIndex = 42
        '
        'lblCorridorWall
        '
        Me.lblCorridorWall.AutoSize = True
        Me.lblCorridorWall.Location = New System.Drawing.Point(29, 390)
        Me.lblCorridorWall.Name = "lblCorridorWall"
        Me.lblCorridorWall.Size = New System.Drawing.Size(70, 13)
        Me.lblCorridorWall.TabIndex = 26
        Me.lblCorridorWall.Text = "Corridor Wall:"
        '
        'cmbIntWall
        '
        Me.cmbIntWall.FormattingEnabled = True
        Me.cmbIntWall.Location = New System.Drawing.Point(103, 360)
        Me.cmbIntWall.Name = "cmbIntWall"
        Me.cmbIntWall.Size = New System.Drawing.Size(121, 21)
        Me.cmbIntWall.TabIndex = 40
        '
        'lblIntWall
        '
        Me.lblIntWall.AutoSize = True
        Me.lblIntWall.Location = New System.Drawing.Point(29, 363)
        Me.lblIntWall.Name = "lblIntWall"
        Me.lblIntWall.Size = New System.Drawing.Size(46, 13)
        Me.lblIntWall.TabIndex = 24
        Me.lblIntWall.Text = "Int Wall:"
        '
        'lblExtRim
        '
        Me.lblExtRim.AutoSize = True
        Me.lblExtRim.Location = New System.Drawing.Point(284, 317)
        Me.lblExtRim.Name = "lblExtRim"
        Me.lblExtRim.Size = New System.Drawing.Size(67, 13)
        Me.lblExtRim.TabIndex = 23
        Me.lblExtRim.Text = "Rim/Ribbon:"
        '
        'cmbExtWall
        '
        Me.cmbExtWall.FormattingEnabled = True
        Me.cmbExtWall.Location = New System.Drawing.Point(103, 333)
        Me.cmbExtWall.Name = "cmbExtWall"
        Me.cmbExtWall.Size = New System.Drawing.Size(121, 21)
        Me.cmbExtWall.TabIndex = 38
        '
        'lblExtWall
        '
        Me.lblExtWall.AutoSize = True
        Me.lblExtWall.Location = New System.Drawing.Point(29, 336)
        Me.lblExtWall.Name = "lblExtWall"
        Me.lblExtWall.Size = New System.Drawing.Size(49, 13)
        Me.lblExtWall.TabIndex = 21
        Me.lblExtWall.Text = "Ext Wall:"
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 10
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 8.465012!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.04515!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.41903!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.41903!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.30578!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 10.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.lblCorridor, 0, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.lblFloor, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.lblCategory, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblTCLL, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblTCDL, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblBCLL, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblBCDL, 4, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblOCSpacing, 5, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblLiveLoadDeflection, 6, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblTotalLoadDeflection, 7, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblAbsolute, 8, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblDeflection, 9, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.lblRoof, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbTCLL_Roof, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbTCDL_Roof, 2, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbBCLL_Roof, 3, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbBCDL_Roof, 4, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbOCSpacing_Roof, 5, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbLiveLoadDeflection_Roof, 6, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbTotalLoadDeflection_Roof, 7, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbAbsolute_Roof, 8, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbDeflection_Roof, 9, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbTCLL_Floor, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbTCDL_Floor, 2, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbBCLL_Floor, 3, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbBCDL_Floor, 4, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbOCSpacing_Floor, 5, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbLiveLoadDeflection_Floor, 6, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbTotalLoadDeflection_Floor, 7, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbAbsolute_Floor, 8, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbDeflection_Floor, 9, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbTCLL_Corridor, 1, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbTCDL_Corridor, 2, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbBCLL_Corridor, 3, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbBCDL_Corridor, 4, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbOCSpacing_Corridor, 5, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbLiveLoadDeflection_Corridor, 6, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbTotalLoadDeflection_Corridor, 7, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbAbsolute_Corridor, 8, 3)
        Me.TableLayoutPanel1.Controls.Add(Me.cmbDeflection_Corridor, 9, 3)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(6, 116)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 4
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(883, 131)
        Me.TableLayoutPanel1.TabIndex = 20
        '
        'lblCorridor
        '
        Me.lblCorridor.AutoSize = True
        Me.lblCorridor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblCorridor.Location = New System.Drawing.Point(3, 96)
        Me.lblCorridor.Name = "lblCorridor"
        Me.lblCorridor.Size = New System.Drawing.Size(69, 35)
        Me.lblCorridor.TabIndex = 41
        Me.lblCorridor.Text = "Corridor"
        Me.lblCorridor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblFloor
        '
        Me.lblFloor.AutoSize = True
        Me.lblFloor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblFloor.Location = New System.Drawing.Point(3, 64)
        Me.lblFloor.Name = "lblFloor"
        Me.lblFloor.Size = New System.Drawing.Size(69, 32)
        Me.lblFloor.TabIndex = 40
        Me.lblFloor.Text = "Floor"
        Me.lblFloor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblCategory
        '
        Me.lblCategory.AutoSize = True
        Me.lblCategory.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblCategory.Location = New System.Drawing.Point(3, 0)
        Me.lblCategory.Name = "lblCategory"
        Me.lblCategory.Size = New System.Drawing.Size(69, 32)
        Me.lblCategory.TabIndex = 0
        Me.lblCategory.Text = "Category"
        Me.lblCategory.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblTCLL
        '
        Me.lblTCLL.AutoSize = True
        Me.lblTCLL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTCLL.Location = New System.Drawing.Point(78, 0)
        Me.lblTCLL.Name = "lblTCLL"
        Me.lblTCLL.Size = New System.Drawing.Size(83, 32)
        Me.lblTCLL.TabIndex = 1
        Me.lblTCLL.Text = "TCLL"
        Me.lblTCLL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblTCDL
        '
        Me.lblTCDL.AutoSize = True
        Me.lblTCDL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTCDL.Location = New System.Drawing.Point(167, 0)
        Me.lblTCDL.Name = "lblTCDL"
        Me.lblTCDL.Size = New System.Drawing.Size(86, 32)
        Me.lblTCDL.TabIndex = 2
        Me.lblTCDL.Text = "TCDL"
        Me.lblTCDL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBCLL
        '
        Me.lblBCLL.AutoSize = True
        Me.lblBCLL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblBCLL.Location = New System.Drawing.Point(259, 0)
        Me.lblBCLL.Name = "lblBCLL"
        Me.lblBCLL.Size = New System.Drawing.Size(86, 32)
        Me.lblBCLL.TabIndex = 3
        Me.lblBCLL.Text = "BCLL"
        Me.lblBCLL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblBCDL
        '
        Me.lblBCDL.AutoSize = True
        Me.lblBCDL.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblBCDL.Location = New System.Drawing.Point(351, 0)
        Me.lblBCDL.Name = "lblBCDL"
        Me.lblBCDL.Size = New System.Drawing.Size(85, 32)
        Me.lblBCDL.TabIndex = 4
        Me.lblBCDL.Text = "BCDL"
        Me.lblBCDL.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblOCSpacing
        '
        Me.lblOCSpacing.AutoSize = True
        Me.lblOCSpacing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblOCSpacing.Location = New System.Drawing.Point(442, 0)
        Me.lblOCSpacing.Name = "lblOCSpacing"
        Me.lblOCSpacing.Size = New System.Drawing.Size(82, 32)
        Me.lblOCSpacing.TabIndex = 5
        Me.lblOCSpacing.Text = "OC Spacing"
        Me.lblOCSpacing.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblLiveLoadDeflection
        '
        Me.lblLiveLoadDeflection.AutoSize = True
        Me.lblLiveLoadDeflection.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblLiveLoadDeflection.Location = New System.Drawing.Point(530, 0)
        Me.lblLiveLoadDeflection.Name = "lblLiveLoadDeflection"
        Me.lblLiveLoadDeflection.Size = New System.Drawing.Size(82, 32)
        Me.lblLiveLoadDeflection.TabIndex = 6
        Me.lblLiveLoadDeflection.Text = "Live Load Deflection"
        Me.lblLiveLoadDeflection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblTotalLoadDeflection
        '
        Me.lblTotalLoadDeflection.AutoSize = True
        Me.lblTotalLoadDeflection.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblTotalLoadDeflection.Location = New System.Drawing.Point(618, 0)
        Me.lblTotalLoadDeflection.Name = "lblTotalLoadDeflection"
        Me.lblTotalLoadDeflection.Size = New System.Drawing.Size(82, 32)
        Me.lblTotalLoadDeflection.TabIndex = 7
        Me.lblTotalLoadDeflection.Text = "Total Load Deflection"
        Me.lblTotalLoadDeflection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblAbsolute
        '
        Me.lblAbsolute.AutoSize = True
        Me.lblAbsolute.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblAbsolute.Location = New System.Drawing.Point(706, 0)
        Me.lblAbsolute.Name = "lblAbsolute"
        Me.lblAbsolute.Size = New System.Drawing.Size(82, 32)
        Me.lblAbsolute.TabIndex = 8
        Me.lblAbsolute.Text = "Absolute"
        Me.lblAbsolute.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblDeflection
        '
        Me.lblDeflection.AutoSize = True
        Me.lblDeflection.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblDeflection.Location = New System.Drawing.Point(794, 0)
        Me.lblDeflection.Name = "lblDeflection"
        Me.lblDeflection.Size = New System.Drawing.Size(86, 32)
        Me.lblDeflection.TabIndex = 9
        Me.lblDeflection.Text = "Deflection"
        Me.lblDeflection.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblRoof
        '
        Me.lblRoof.AutoSize = True
        Me.lblRoof.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblRoof.Location = New System.Drawing.Point(3, 32)
        Me.lblRoof.Name = "lblRoof"
        Me.lblRoof.Size = New System.Drawing.Size(69, 32)
        Me.lblRoof.TabIndex = 10
        Me.lblRoof.Text = "Roof"
        Me.lblRoof.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'cmbTCLL_Roof
        '
        Me.cmbTCLL_Roof.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend
        Me.cmbTCLL_Roof.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems
        Me.cmbTCLL_Roof.Dock = System.Windows.Forms.DockStyle.Fill
        Me.cmbTCLL_Roof.FormattingEnabled = True
        Me.cmbTCLL_Roof.Location = New System.Drawing.Point(78, 35)
        Me.cmbTCLL_Roof.Name = "cmbTCLL_Roof"
        Me.cmbTCLL_Roof.Size = New System.Drawing.Size(83, 21)
        Me.cmbTCLL_Roof.TabIndex = 7
        '
        'cmbTCDL_Roof
        '
        Me.cmbTCDL_Roof.Location = New System.Drawing.Point(167, 35)
        Me.cmbTCDL_Roof.Name = "cmbTCDL_Roof"
        Me.cmbTCDL_Roof.Size = New System.Drawing.Size(82, 21)
        Me.cmbTCDL_Roof.TabIndex = 8
        '
        'cmbBCLL_Roof
        '
        Me.cmbBCLL_Roof.Location = New System.Drawing.Point(259, 35)
        Me.cmbBCLL_Roof.Name = "cmbBCLL_Roof"
        Me.cmbBCLL_Roof.Size = New System.Drawing.Size(82, 21)
        Me.cmbBCLL_Roof.TabIndex = 9
        '
        'cmbBCDL_Roof
        '
        Me.cmbBCDL_Roof.Location = New System.Drawing.Point(351, 35)
        Me.cmbBCDL_Roof.Name = "cmbBCDL_Roof"
        Me.cmbBCDL_Roof.Size = New System.Drawing.Size(82, 21)
        Me.cmbBCDL_Roof.TabIndex = 10
        '
        'cmbOCSpacing_Roof
        '
        Me.cmbOCSpacing_Roof.Location = New System.Drawing.Point(442, 35)
        Me.cmbOCSpacing_Roof.Name = "cmbOCSpacing_Roof"
        Me.cmbOCSpacing_Roof.Size = New System.Drawing.Size(82, 21)
        Me.cmbOCSpacing_Roof.TabIndex = 11
        '
        'cmbLiveLoadDeflection_Roof
        '
        Me.cmbLiveLoadDeflection_Roof.Location = New System.Drawing.Point(530, 35)
        Me.cmbLiveLoadDeflection_Roof.Name = "cmbLiveLoadDeflection_Roof"
        Me.cmbLiveLoadDeflection_Roof.Size = New System.Drawing.Size(82, 21)
        Me.cmbLiveLoadDeflection_Roof.TabIndex = 12
        '
        'cmbTotalLoadDeflection_Roof
        '
        Me.cmbTotalLoadDeflection_Roof.Location = New System.Drawing.Point(618, 35)
        Me.cmbTotalLoadDeflection_Roof.Name = "cmbTotalLoadDeflection_Roof"
        Me.cmbTotalLoadDeflection_Roof.Size = New System.Drawing.Size(82, 21)
        Me.cmbTotalLoadDeflection_Roof.TabIndex = 13
        '
        'cmbAbsolute_Roof
        '
        Me.cmbAbsolute_Roof.Location = New System.Drawing.Point(706, 35)
        Me.cmbAbsolute_Roof.Name = "cmbAbsolute_Roof"
        Me.cmbAbsolute_Roof.Size = New System.Drawing.Size(82, 21)
        Me.cmbAbsolute_Roof.TabIndex = 14
        '
        'cmbDeflection_Roof
        '
        Me.cmbDeflection_Roof.Location = New System.Drawing.Point(794, 35)
        Me.cmbDeflection_Roof.Name = "cmbDeflection_Roof"
        Me.cmbDeflection_Roof.Size = New System.Drawing.Size(86, 21)
        Me.cmbDeflection_Roof.TabIndex = 15
        '
        'cmbTCLL_Floor
        '
        Me.cmbTCLL_Floor.Location = New System.Drawing.Point(78, 67)
        Me.cmbTCLL_Floor.Name = "cmbTCLL_Floor"
        Me.cmbTCLL_Floor.Size = New System.Drawing.Size(82, 21)
        Me.cmbTCLL_Floor.TabIndex = 16
        '
        'cmbTCDL_Floor
        '
        Me.cmbTCDL_Floor.Location = New System.Drawing.Point(167, 67)
        Me.cmbTCDL_Floor.Name = "cmbTCDL_Floor"
        Me.cmbTCDL_Floor.Size = New System.Drawing.Size(82, 21)
        Me.cmbTCDL_Floor.TabIndex = 17
        '
        'cmbBCLL_Floor
        '
        Me.cmbBCLL_Floor.Location = New System.Drawing.Point(259, 67)
        Me.cmbBCLL_Floor.Name = "cmbBCLL_Floor"
        Me.cmbBCLL_Floor.Size = New System.Drawing.Size(82, 21)
        Me.cmbBCLL_Floor.TabIndex = 18
        '
        'cmbBCDL_Floor
        '
        Me.cmbBCDL_Floor.Location = New System.Drawing.Point(351, 67)
        Me.cmbBCDL_Floor.Name = "cmbBCDL_Floor"
        Me.cmbBCDL_Floor.Size = New System.Drawing.Size(82, 21)
        Me.cmbBCDL_Floor.TabIndex = 19
        '
        'cmbOCSpacing_Floor
        '
        Me.cmbOCSpacing_Floor.Location = New System.Drawing.Point(442, 67)
        Me.cmbOCSpacing_Floor.Name = "cmbOCSpacing_Floor"
        Me.cmbOCSpacing_Floor.Size = New System.Drawing.Size(82, 21)
        Me.cmbOCSpacing_Floor.TabIndex = 20
        '
        'cmbLiveLoadDeflection_Floor
        '
        Me.cmbLiveLoadDeflection_Floor.Location = New System.Drawing.Point(530, 67)
        Me.cmbLiveLoadDeflection_Floor.Name = "cmbLiveLoadDeflection_Floor"
        Me.cmbLiveLoadDeflection_Floor.Size = New System.Drawing.Size(82, 21)
        Me.cmbLiveLoadDeflection_Floor.TabIndex = 21
        '
        'cmbTotalLoadDeflection_Floor
        '
        Me.cmbTotalLoadDeflection_Floor.Location = New System.Drawing.Point(618, 67)
        Me.cmbTotalLoadDeflection_Floor.Name = "cmbTotalLoadDeflection_Floor"
        Me.cmbTotalLoadDeflection_Floor.Size = New System.Drawing.Size(82, 21)
        Me.cmbTotalLoadDeflection_Floor.TabIndex = 22
        '
        'cmbAbsolute_Floor
        '
        Me.cmbAbsolute_Floor.Location = New System.Drawing.Point(706, 67)
        Me.cmbAbsolute_Floor.Name = "cmbAbsolute_Floor"
        Me.cmbAbsolute_Floor.Size = New System.Drawing.Size(82, 21)
        Me.cmbAbsolute_Floor.TabIndex = 23
        '
        'cmbDeflection_Floor
        '
        Me.cmbDeflection_Floor.Location = New System.Drawing.Point(794, 67)
        Me.cmbDeflection_Floor.Name = "cmbDeflection_Floor"
        Me.cmbDeflection_Floor.Size = New System.Drawing.Size(86, 21)
        Me.cmbDeflection_Floor.TabIndex = 24
        '
        'cmbTCLL_Corridor
        '
        Me.cmbTCLL_Corridor.Location = New System.Drawing.Point(78, 99)
        Me.cmbTCLL_Corridor.Name = "cmbTCLL_Corridor"
        Me.cmbTCLL_Corridor.Size = New System.Drawing.Size(82, 21)
        Me.cmbTCLL_Corridor.TabIndex = 25
        '
        'cmbTCDL_Corridor
        '
        Me.cmbTCDL_Corridor.Location = New System.Drawing.Point(167, 99)
        Me.cmbTCDL_Corridor.Name = "cmbTCDL_Corridor"
        Me.cmbTCDL_Corridor.Size = New System.Drawing.Size(82, 21)
        Me.cmbTCDL_Corridor.TabIndex = 26
        '
        'cmbBCLL_Corridor
        '
        Me.cmbBCLL_Corridor.Location = New System.Drawing.Point(259, 99)
        Me.cmbBCLL_Corridor.Name = "cmbBCLL_Corridor"
        Me.cmbBCLL_Corridor.Size = New System.Drawing.Size(82, 21)
        Me.cmbBCLL_Corridor.TabIndex = 27
        '
        'cmbBCDL_Corridor
        '
        Me.cmbBCDL_Corridor.Location = New System.Drawing.Point(351, 99)
        Me.cmbBCDL_Corridor.Name = "cmbBCDL_Corridor"
        Me.cmbBCDL_Corridor.Size = New System.Drawing.Size(82, 21)
        Me.cmbBCDL_Corridor.TabIndex = 28
        '
        'cmbOCSpacing_Corridor
        '
        Me.cmbOCSpacing_Corridor.Location = New System.Drawing.Point(442, 99)
        Me.cmbOCSpacing_Corridor.Name = "cmbOCSpacing_Corridor"
        Me.cmbOCSpacing_Corridor.Size = New System.Drawing.Size(82, 21)
        Me.cmbOCSpacing_Corridor.TabIndex = 29
        '
        'cmbLiveLoadDeflection_Corridor
        '
        Me.cmbLiveLoadDeflection_Corridor.Location = New System.Drawing.Point(530, 99)
        Me.cmbLiveLoadDeflection_Corridor.Name = "cmbLiveLoadDeflection_Corridor"
        Me.cmbLiveLoadDeflection_Corridor.Size = New System.Drawing.Size(82, 21)
        Me.cmbLiveLoadDeflection_Corridor.TabIndex = 30
        '
        'cmbTotalLoadDeflection_Corridor
        '
        Me.cmbTotalLoadDeflection_Corridor.Location = New System.Drawing.Point(618, 99)
        Me.cmbTotalLoadDeflection_Corridor.Name = "cmbTotalLoadDeflection_Corridor"
        Me.cmbTotalLoadDeflection_Corridor.Size = New System.Drawing.Size(82, 21)
        Me.cmbTotalLoadDeflection_Corridor.TabIndex = 31
        '
        'cmbAbsolute_Corridor
        '
        Me.cmbAbsolute_Corridor.Location = New System.Drawing.Point(706, 99)
        Me.cmbAbsolute_Corridor.Name = "cmbAbsolute_Corridor"
        Me.cmbAbsolute_Corridor.Size = New System.Drawing.Size(82, 21)
        Me.cmbAbsolute_Corridor.TabIndex = 32
        '
        'cmbDeflection_Corridor
        '
        Me.cmbDeflection_Corridor.Location = New System.Drawing.Point(794, 99)
        Me.cmbDeflection_Corridor.Name = "cmbDeflection_Corridor"
        Me.cmbDeflection_Corridor.Size = New System.Drawing.Size(86, 21)
        Me.cmbDeflection_Corridor.TabIndex = 33
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(210, 282)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(71, 13)
        Me.Label1.TabIndex = 19
        Me.Label1.Text = "Heel Heights:"
        '
        'txtHeelHeights
        '
        Me.txtHeelHeights.Location = New System.Drawing.Point(287, 279)
        Me.txtHeelHeights.Name = "txtHeelHeights"
        Me.txtHeelHeights.Size = New System.Drawing.Size(100, 20)
        Me.txtHeelHeights.TabIndex = 37
        '
        'lblWallHeights
        '
        Me.lblWallHeights.AutoSize = True
        Me.lblWallHeights.Location = New System.Drawing.Point(27, 256)
        Me.lblWallHeights.Name = "lblWallHeights"
        Me.lblWallHeights.Size = New System.Drawing.Size(70, 13)
        Me.lblWallHeights.TabIndex = 0
        Me.lblWallHeights.Text = "Wall Heights:"
        '
        'txtWallHeights
        '
        Me.txtWallHeights.Location = New System.Drawing.Point(103, 253)
        Me.txtWallHeights.Name = "txtWallHeights"
        Me.txtWallHeights.Size = New System.Drawing.Size(100, 20)
        Me.txtWallHeights.TabIndex = 34
        '
        'lblFloorDepths
        '
        Me.lblFloorDepths.AutoSize = True
        Me.lblFloorDepths.Location = New System.Drawing.Point(211, 256)
        Me.lblFloorDepths.Name = "lblFloorDepths"
        Me.lblFloorDepths.Size = New System.Drawing.Size(70, 13)
        Me.lblFloorDepths.TabIndex = 2
        Me.lblFloorDepths.Text = "Floor Depths:"
        '
        'txtFloorDepths
        '
        Me.txtFloorDepths.Location = New System.Drawing.Point(287, 253)
        Me.txtFloorDepths.Name = "txtFloorDepths"
        Me.txtFloorDepths.Size = New System.Drawing.Size(100, 20)
        Me.txtFloorDepths.TabIndex = 35
        '
        'lblRoofPitches
        '
        Me.lblRoofPitches.AutoSize = True
        Me.lblRoofPitches.Location = New System.Drawing.Point(27, 282)
        Me.lblRoofPitches.Name = "lblRoofPitches"
        Me.lblRoofPitches.Size = New System.Drawing.Size(71, 13)
        Me.lblRoofPitches.TabIndex = 4
        Me.lblRoofPitches.Text = "Roof Pitches:"
        '
        'txtRoofPitches
        '
        Me.txtRoofPitches.Location = New System.Drawing.Point(103, 279)
        Me.txtRoofPitches.Name = "txtRoofPitches"
        Me.txtRoofPitches.Size = New System.Drawing.Size(100, 20)
        Me.txtRoofPitches.TabIndex = 36
        '
        'lblOccupancy
        '
        Me.lblOccupancy.AutoSize = True
        Me.lblOccupancy.Location = New System.Drawing.Point(216, 92)
        Me.lblOccupancy.Name = "lblOccupancy"
        Me.lblOccupancy.Size = New System.Drawing.Size(65, 13)
        Me.lblOccupancy.TabIndex = 6
        Me.lblOccupancy.Text = "Occupancy:"
        '
        'cmbOccupancy
        '
        Me.cmbOccupancy.FormattingEnabled = True
        Me.cmbOccupancy.Location = New System.Drawing.Point(287, 89)
        Me.cmbOccupancy.Name = "cmbOccupancy"
        Me.cmbOccupancy.Size = New System.Drawing.Size(100, 21)
        Me.cmbOccupancy.TabIndex = 6
        '
        'lblSnowLoad
        '
        Me.lblSnowLoad.AutoSize = True
        Me.lblSnowLoad.Location = New System.Drawing.Point(6, 65)
        Me.lblSnowLoad.Name = "lblSnowLoad"
        Me.lblSnowLoad.Size = New System.Drawing.Size(91, 13)
        Me.lblSnowLoad.TabIndex = 8
        Me.lblSnowLoad.Text = "Snow Load Type:"
        '
        'cmbSnowLoad
        '
        Me.cmbSnowLoad.FormattingEnabled = True
        Me.cmbSnowLoad.Location = New System.Drawing.Point(103, 62)
        Me.cmbSnowLoad.Name = "cmbSnowLoad"
        Me.cmbSnowLoad.Size = New System.Drawing.Size(100, 21)
        Me.cmbSnowLoad.TabIndex = 3
        '
        'lblWindSpeed
        '
        Me.lblWindSpeed.AutoSize = True
        Me.lblWindSpeed.Location = New System.Drawing.Point(28, 92)
        Me.lblWindSpeed.Name = "lblWindSpeed"
        Me.lblWindSpeed.Size = New System.Drawing.Size(69, 13)
        Me.lblWindSpeed.TabIndex = 10
        Me.lblWindSpeed.Text = "Wind Speed:"
        '
        'cmbWindSpeed
        '
        Me.cmbWindSpeed.FormattingEnabled = True
        Me.cmbWindSpeed.Location = New System.Drawing.Point(103, 89)
        Me.cmbWindSpeed.Name = "cmbWindSpeed"
        Me.cmbWindSpeed.Size = New System.Drawing.Size(100, 21)
        Me.cmbWindSpeed.TabIndex = 5
        '
        'lblExposure
        '
        Me.lblExposure.AutoSize = True
        Me.lblExposure.Location = New System.Drawing.Point(227, 65)
        Me.lblExposure.Name = "lblExposure"
        Me.lblExposure.Size = New System.Drawing.Size(54, 13)
        Me.lblExposure.TabIndex = 12
        Me.lblExposure.Text = "Exposure:"
        '
        'cmbExposure
        '
        Me.cmbExposure.FormattingEnabled = True
        Me.cmbExposure.Location = New System.Drawing.Point(287, 62)
        Me.cmbExposure.Name = "cmbExposure"
        Me.cmbExposure.Size = New System.Drawing.Size(100, 21)
        Me.cmbExposure.TabIndex = 4
        '
        'lblImportance
        '
        Me.lblImportance.AutoSize = True
        Me.lblImportance.Location = New System.Drawing.Point(218, 38)
        Me.lblImportance.Name = "lblImportance"
        Me.lblImportance.Size = New System.Drawing.Size(63, 13)
        Me.lblImportance.TabIndex = 14
        Me.lblImportance.Text = "Importance:"
        '
        'cmbImportance
        '
        Me.cmbImportance.FormattingEnabled = True
        Me.cmbImportance.Location = New System.Drawing.Point(287, 35)
        Me.cmbImportance.Name = "cmbImportance"
        Me.cmbImportance.Size = New System.Drawing.Size(100, 21)
        Me.cmbImportance.TabIndex = 2
        '
        'lblBuildingCode
        '
        Me.lblBuildingCode.AutoSize = True
        Me.lblBuildingCode.Location = New System.Drawing.Point(22, 38)
        Me.lblBuildingCode.Name = "lblBuildingCode"
        Me.lblBuildingCode.Size = New System.Drawing.Size(75, 13)
        Me.lblBuildingCode.TabIndex = 16
        Me.lblBuildingCode.Text = "Building Code:"
        '
        'cmbBuildingCode
        '
        Me.cmbBuildingCode.FormattingEnabled = True
        Me.cmbBuildingCode.Location = New System.Drawing.Point(103, 35)
        Me.cmbBuildingCode.Name = "cmbBuildingCode"
        Me.cmbBuildingCode.Size = New System.Drawing.Size(100, 21)
        Me.cmbBuildingCode.TabIndex = 1
        '
        'tabRoofItems
        '
        Me.tabRoofItems.Controls.Add(Me.dgvRoofItems)
        Me.tabRoofItems.Location = New System.Drawing.Point(4, 22)
        Me.tabRoofItems.Name = "tabRoofItems"
        Me.tabRoofItems.Padding = New System.Windows.Forms.Padding(3)
        Me.tabRoofItems.Size = New System.Drawing.Size(892, 514)
        Me.tabRoofItems.TabIndex = 3
        Me.tabRoofItems.Text = "Roof Items"
        Me.tabRoofItems.UseVisualStyleBackColor = True
        '
        'dgvRoofItems
        '
        Me.dgvRoofItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvRoofItems.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colRoofKN, Me.colRoofDesc, Me.colRoofIncluded, Me.colRoofExcluded, Me.colRoofOptional})
        Me.dgvRoofItems.Location = New System.Drawing.Point(6, 6)
        Me.dgvRoofItems.Name = "dgvRoofItems"
        Me.dgvRoofItems.RowHeadersWidth = 4
        Me.dgvRoofItems.Size = New System.Drawing.Size(880, 504)
        Me.dgvRoofItems.TabIndex = 0
        '
        'colRoofKN
        '
        Me.colRoofKN.HeaderText = "KN"
        Me.colRoofKN.Name = "colRoofKN"
        Me.colRoofKN.Width = 50
        '
        'colRoofDesc
        '
        Me.colRoofDesc.HeaderText = "Desc"
        Me.colRoofDesc.Name = "colRoofDesc"
        Me.colRoofDesc.Width = 200
        '
        'colRoofIncluded
        '
        Me.colRoofIncluded.HeaderText = "Included"
        Me.colRoofIncluded.Name = "colRoofIncluded"
        Me.colRoofIncluded.Width = 75
        '
        'colRoofExcluded
        '
        Me.colRoofExcluded.HeaderText = "Excluded"
        Me.colRoofExcluded.Name = "colRoofExcluded"
        Me.colRoofExcluded.Width = 75
        '
        'colRoofOptional
        '
        Me.colRoofOptional.HeaderText = "Optional"
        Me.colRoofOptional.Name = "colRoofOptional"
        Me.colRoofOptional.Width = 75
        '
        'tabFloorItems
        '
        Me.tabFloorItems.Controls.Add(Me.dgvFloorItems)
        Me.tabFloorItems.Location = New System.Drawing.Point(4, 22)
        Me.tabFloorItems.Name = "tabFloorItems"
        Me.tabFloorItems.Padding = New System.Windows.Forms.Padding(3)
        Me.tabFloorItems.Size = New System.Drawing.Size(892, 514)
        Me.tabFloorItems.TabIndex = 4
        Me.tabFloorItems.Text = "Floor Items"
        Me.tabFloorItems.UseVisualStyleBackColor = True
        '
        'dgvFloorItems
        '
        Me.dgvFloorItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvFloorItems.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colFloorKN, Me.colFloorDesc, Me.colFloorIncluded, Me.colFloorExcluded, Me.colFloorOptional})
        Me.dgvFloorItems.Location = New System.Drawing.Point(6, 6)
        Me.dgvFloorItems.Name = "dgvFloorItems"
        Me.dgvFloorItems.RowHeadersWidth = 4
        Me.dgvFloorItems.Size = New System.Drawing.Size(880, 504)
        Me.dgvFloorItems.TabIndex = 0
        '
        'colFloorKN
        '
        Me.colFloorKN.HeaderText = "KN"
        Me.colFloorKN.Name = "colFloorKN"
        Me.colFloorKN.Width = 50
        '
        'colFloorDesc
        '
        Me.colFloorDesc.HeaderText = "Desc"
        Me.colFloorDesc.Name = "colFloorDesc"
        Me.colFloorDesc.Width = 200
        '
        'colFloorIncluded
        '
        Me.colFloorIncluded.HeaderText = "Included"
        Me.colFloorIncluded.Name = "colFloorIncluded"
        Me.colFloorIncluded.Width = 75
        '
        'colFloorExcluded
        '
        Me.colFloorExcluded.HeaderText = "Excluded"
        Me.colFloorExcluded.Name = "colFloorExcluded"
        Me.colFloorExcluded.Width = 75
        '
        'colFloorOptional
        '
        Me.colFloorOptional.HeaderText = "Optional"
        Me.colFloorOptional.Name = "colFloorOptional"
        Me.colFloorOptional.Width = 75
        '
        'tabWallItems
        '
        Me.tabWallItems.Controls.Add(Me.dgvWallItems)
        Me.tabWallItems.Location = New System.Drawing.Point(4, 22)
        Me.tabWallItems.Name = "tabWallItems"
        Me.tabWallItems.Padding = New System.Windows.Forms.Padding(3)
        Me.tabWallItems.Size = New System.Drawing.Size(892, 514)
        Me.tabWallItems.TabIndex = 5
        Me.tabWallItems.Text = "Wall Items"
        Me.tabWallItems.UseVisualStyleBackColor = True
        '
        'dgvWallItems
        '
        Me.dgvWallItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvWallItems.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colWallKN, Me.colWallDesc, Me.colWallIncluded, Me.colWallExcluded, Me.colWallOptional})
        Me.dgvWallItems.Location = New System.Drawing.Point(6, 6)
        Me.dgvWallItems.Name = "dgvWallItems"
        Me.dgvWallItems.RowHeadersWidth = 4
        Me.dgvWallItems.Size = New System.Drawing.Size(880, 504)
        Me.dgvWallItems.TabIndex = 0
        '
        'colWallKN
        '
        Me.colWallKN.HeaderText = "KN"
        Me.colWallKN.Name = "colWallKN"
        Me.colWallKN.Width = 50
        '
        'colWallDesc
        '
        Me.colWallDesc.HeaderText = "Desc"
        Me.colWallDesc.Name = "colWallDesc"
        Me.colWallDesc.Width = 200
        '
        'colWallIncluded
        '
        Me.colWallIncluded.HeaderText = "Included"
        Me.colWallIncluded.Name = "colWallIncluded"
        Me.colWallIncluded.Width = 75
        '
        'colWallExcluded
        '
        Me.colWallExcluded.HeaderText = "Excluded"
        Me.colWallExcluded.Name = "colWallExcluded"
        Me.colWallExcluded.Width = 75
        '
        'colWallOptional
        '
        Me.colWallOptional.HeaderText = "Optional"
        Me.colWallOptional.Name = "colWallOptional"
        Me.colWallOptional.Width = 75
        '
        'tabNotes
        '
        Me.tabNotes.Controls.Add(Me.txtGeneralNotes)
        Me.tabNotes.Location = New System.Drawing.Point(4, 22)
        Me.tabNotes.Name = "tabNotes"
        Me.tabNotes.Padding = New System.Windows.Forms.Padding(3)
        Me.tabNotes.Size = New System.Drawing.Size(892, 514)
        Me.tabNotes.TabIndex = 6
        Me.tabNotes.Text = "General Notes"
        Me.tabNotes.UseVisualStyleBackColor = True
        '
        'txtGeneralNotes
        '
        Me.txtGeneralNotes.Location = New System.Drawing.Point(6, 6)
        Me.txtGeneralNotes.Multiline = True
        Me.txtGeneralNotes.Name = "txtGeneralNotes"
        Me.txtGeneralNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtGeneralNotes.Size = New System.Drawing.Size(880, 504)
        Me.txtGeneralNotes.TabIndex = 0
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSave.Location = New System.Drawing.Point(721, 546)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 1
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(802, 546)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.TabIndex = 2
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'frmInclusionsExclusions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 600)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.tabControl)
        Me.Name = "frmInclusionsExclusions"
        Me.Text = "Inclusions / Exclusions"
        Me.tabControl.ResumeLayout(False)
        Me.tabDesignInfo.ResumeLayout(False)
        Me.tabDesignInfo.PerformLayout()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.TableLayoutPanel1.PerformLayout()
        Me.tabRoofItems.ResumeLayout(False)
        CType(Me.dgvRoofItems, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabFloorItems.ResumeLayout(False)
        CType(Me.dgvFloorItems, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabWallItems.ResumeLayout(False)
        CType(Me.dgvWallItems, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabNotes.ResumeLayout(False)
        Me.tabNotes.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents tabControl As TabControl
    Friend WithEvents tabDesignInfo As TabPage
    Friend WithEvents lblWallHeights As Label
    Friend WithEvents txtWallHeights As TextBox
    Friend WithEvents lblFloorDepths As Label
    Friend WithEvents txtFloorDepths As TextBox
    Friend WithEvents lblRoofPitches As Label
    Friend WithEvents txtRoofPitches As TextBox
    Friend WithEvents lblOccupancy As Label
    Friend WithEvents cmbOccupancy As ComboBox
    Friend WithEvents lblSnowLoad As Label
    Friend WithEvents cmbSnowLoad As ComboBox
    Friend WithEvents lblWindSpeed As Label
    Friend WithEvents cmbWindSpeed As ComboBox
    Friend WithEvents lblExposure As Label
    Friend WithEvents cmbExposure As ComboBox
    Friend WithEvents lblImportance As Label
    Friend WithEvents cmbImportance As ComboBox
    Friend WithEvents lblBuildingCode As Label
    Friend WithEvents cmbBuildingCode As ComboBox
    Friend WithEvents tabRoofItems As TabPage
    Friend WithEvents dgvRoofItems As DataGridView
    Friend WithEvents colRoofKN As DataGridViewTextBoxColumn
    Friend WithEvents colRoofDesc As DataGridViewTextBoxColumn
    Friend WithEvents colRoofIncluded As DataGridViewCheckBoxColumn
    Friend WithEvents colRoofExcluded As DataGridViewCheckBoxColumn
    Friend WithEvents colRoofOptional As DataGridViewCheckBoxColumn
    Friend WithEvents tabFloorItems As TabPage
    Friend WithEvents dgvFloorItems As DataGridView
    Friend WithEvents colFloorKN As DataGridViewTextBoxColumn
    Friend WithEvents colFloorDesc As DataGridViewTextBoxColumn
    Friend WithEvents colFloorIncluded As DataGridViewCheckBoxColumn
    Friend WithEvents colFloorExcluded As DataGridViewCheckBoxColumn
    Friend WithEvents colFloorOptional As DataGridViewCheckBoxColumn
    Friend WithEvents tabWallItems As TabPage
    Friend WithEvents dgvWallItems As DataGridView
    Friend WithEvents colWallKN As DataGridViewTextBoxColumn
    Friend WithEvents colWallDesc As DataGridViewTextBoxColumn
    Friend WithEvents colWallIncluded As DataGridViewCheckBoxColumn
    Friend WithEvents colWallExcluded As DataGridViewCheckBoxColumn
    Friend WithEvents colWallOptional As DataGridViewCheckBoxColumn
    Friend WithEvents tabNotes As TabPage
    Friend WithEvents txtGeneralNotes As TextBox
    Friend WithEvents btnSave As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents txtHeelHeights As TextBox
    Friend WithEvents cboCorridorRimRibbon As ComboBox
    Friend WithEvents cboIntRimRibbon As ComboBox
    Friend WithEvents cboExtRimRibbon As ComboBox
    Friend WithEvents cmbCorridorWall As ComboBox
    Friend WithEvents lblCorridorWall As Label
    Friend WithEvents cmbIntWall As ComboBox
    Friend WithEvents lblIntWall As Label
    Friend WithEvents lblExtRim As Label
    Friend WithEvents cmbExtWall As ComboBox
    Friend WithEvents lblExtWall As Label
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents lblCorridor As Label
    Friend WithEvents lblFloor As Label
    Friend WithEvents lblCategory As Label
    Friend WithEvents lblTCLL As Label
    Friend WithEvents lblTCDL As Label
    Friend WithEvents lblBCLL As Label
    Friend WithEvents lblBCDL As Label
    Friend WithEvents lblOCSpacing As Label
    Friend WithEvents lblLiveLoadDeflection As Label
    Friend WithEvents lblTotalLoadDeflection As Label
    Friend WithEvents lblAbsolute As Label
    Friend WithEvents lblDeflection As Label
    Friend WithEvents lblRoof As Label
    Friend WithEvents cmbTCLL_Roof As ComboBox
    Friend WithEvents cmbTCDL_Roof As ComboBox
    Friend WithEvents cmbBCLL_Roof As ComboBox
    Friend WithEvents cmbBCDL_Roof As ComboBox
    Friend WithEvents cmbOCSpacing_Roof As ComboBox
    Friend WithEvents cmbLiveLoadDeflection_Roof As ComboBox
    Friend WithEvents cmbTotalLoadDeflection_Roof As ComboBox
    Friend WithEvents cmbAbsolute_Roof As ComboBox
    Friend WithEvents cmbDeflection_Roof As ComboBox
    Friend WithEvents cmbTCLL_Floor As ComboBox
    Friend WithEvents cmbTCDL_Floor As ComboBox
    Friend WithEvents cmbBCLL_Floor As ComboBox
    Friend WithEvents cmbBCDL_Floor As ComboBox
    Friend WithEvents cmbOCSpacing_Floor As ComboBox
    Friend WithEvents cmbLiveLoadDeflection_Floor As ComboBox
    Friend WithEvents cmbTotalLoadDeflection_Floor As ComboBox
    Friend WithEvents cmbAbsolute_Floor As ComboBox
    Friend WithEvents cmbDeflection_Floor As ComboBox
    Friend WithEvents cmbTCLL_Corridor As ComboBox
    Friend WithEvents cmbTCDL_Corridor As ComboBox
    Friend WithEvents cmbBCLL_Corridor As ComboBox
    Friend WithEvents cmbBCDL_Corridor As ComboBox
    Friend WithEvents cmbOCSpacing_Corridor As ComboBox
    Friend WithEvents cmbLiveLoadDeflection_Corridor As ComboBox
    Friend WithEvents cmbTotalLoadDeflection_Corridor As ComboBox
    Friend WithEvents cmbAbsolute_Corridor As ComboBox
    Friend WithEvents cmbDeflection_Corridor As ComboBox
End Class