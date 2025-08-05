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
        Me.btnSave = New System.Windows.Forms.Button()
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
        Me.tabLoads = New System.Windows.Forms.TabPage()
        Me.dgvLoads = New System.Windows.Forms.DataGridView()
        Me.tabBearing = New System.Windows.Forms.TabPage()
        Me.txtCorridorRim = New System.Windows.Forms.TextBox()
        Me.lblCorridorRim = New System.Windows.Forms.Label()
        Me.cmbCorridorWall = New System.Windows.Forms.ComboBox()
        Me.lblCorridorWall = New System.Windows.Forms.Label()
        Me.txtIntRim = New System.Windows.Forms.TextBox()
        Me.lblIntRim = New System.Windows.Forms.Label()
        Me.cmbIntWall = New System.Windows.Forms.ComboBox()
        Me.lblIntWall = New System.Windows.Forms.Label()
        Me.txtExtRim = New System.Windows.Forms.TextBox()
        Me.lblExtRim = New System.Windows.Forms.Label()
        Me.cmbExtWall = New System.Windows.Forms.ComboBox()
        Me.lblExtWall = New System.Windows.Forms.Label()
        Me.tabNotes = New System.Windows.Forms.TabPage()
        Me.txtGeneralNotes = New System.Windows.Forms.TextBox()
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
        Me.colCategory = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTCLL = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colTCDL = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colBCLL = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colBCDL = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colOCSpacing = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colLiveDefl = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colTotalDefl = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colAbsolute = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.colDeflection = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.tabControl.SuspendLayout()
        Me.tabDesignInfo.SuspendLayout()
        Me.tabLoads.SuspendLayout()
        CType(Me.dgvLoads, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabBearing.SuspendLayout()
        Me.tabNotes.SuspendLayout()
        Me.tabRoofItems.SuspendLayout()
        CType(Me.dgvRoofItems, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabFloorItems.SuspendLayout()
        CType(Me.dgvFloorItems, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabWallItems.SuspendLayout()
        CType(Me.dgvWallItems, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'tabControl
        '
        Me.tabControl.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tabControl.Controls.Add(Me.tabDesignInfo)
        Me.tabControl.Controls.Add(Me.tabLoads)
        Me.tabControl.Controls.Add(Me.tabBearing)
        Me.tabControl.Controls.Add(Me.tabRoofItems)
        Me.tabControl.Controls.Add(Me.tabFloorItems)
        Me.tabControl.Controls.Add(Me.tabWallItems)
        Me.tabControl.Controls.Add(Me.tabNotes)
        Me.tabControl.Location = New System.Drawing.Point(0, 0)
        Me.tabControl.Name = "tabControl"
        Me.tabControl.SelectedIndex = 0
        Me.tabControl.Size = New System.Drawing.Size(900, 559)
        Me.tabControl.TabIndex = 0
        '
        'tabDesignInfo
        '
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
        Me.tabDesignInfo.Size = New System.Drawing.Size(892, 533)
        Me.tabDesignInfo.TabIndex = 0
        Me.tabDesignInfo.Text = "Design Info"
        Me.tabDesignInfo.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSave.Location = New System.Drawing.Point(726, 565)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 1
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'lblWallHeights
        '
        Me.lblWallHeights.AutoSize = True
        Me.lblWallHeights.Location = New System.Drawing.Point(10, 253)
        Me.lblWallHeights.Name = "lblWallHeights"
        Me.lblWallHeights.Size = New System.Drawing.Size(76, 13)
        Me.lblWallHeights.TabIndex = 16
        Me.lblWallHeights.Text = "Wall Height(s):"
        '
        'txtWallHeights
        '
        Me.txtWallHeights.Location = New System.Drawing.Point(150, 250)
        Me.txtWallHeights.Name = "txtWallHeights"
        Me.txtWallHeights.Size = New System.Drawing.Size(200, 20)
        Me.txtWallHeights.TabIndex = 17
        '
        'lblFloorDepths
        '
        Me.lblFloorDepths.AutoSize = True
        Me.lblFloorDepths.Location = New System.Drawing.Point(10, 223)
        Me.lblFloorDepths.Name = "lblFloorDepths"
        Me.lblFloorDepths.Size = New System.Drawing.Size(76, 13)
        Me.lblFloorDepths.TabIndex = 14
        Me.lblFloorDepths.Text = "Floor Depth(s):"
        '
        'txtFloorDepths
        '
        Me.txtFloorDepths.Location = New System.Drawing.Point(150, 220)
        Me.txtFloorDepths.Name = "txtFloorDepths"
        Me.txtFloorDepths.Size = New System.Drawing.Size(200, 20)
        Me.txtFloorDepths.TabIndex = 15
        '
        'lblRoofPitches
        '
        Me.lblRoofPitches.AutoSize = True
        Me.lblRoofPitches.Location = New System.Drawing.Point(10, 193)
        Me.lblRoofPitches.Name = "lblRoofPitches"
        Me.lblRoofPitches.Size = New System.Drawing.Size(77, 13)
        Me.lblRoofPitches.TabIndex = 12
        Me.lblRoofPitches.Text = "Roof Pitch(es):"
        '
        'txtRoofPitches
        '
        Me.txtRoofPitches.Location = New System.Drawing.Point(150, 190)
        Me.txtRoofPitches.Name = "txtRoofPitches"
        Me.txtRoofPitches.Size = New System.Drawing.Size(200, 20)
        Me.txtRoofPitches.TabIndex = 13
        '
        'lblOccupancy
        '
        Me.lblOccupancy.AutoSize = True
        Me.lblOccupancy.Location = New System.Drawing.Point(10, 163)
        Me.lblOccupancy.Name = "lblOccupancy"
        Me.lblOccupancy.Size = New System.Drawing.Size(110, 13)
        Me.lblOccupancy.TabIndex = 10
        Me.lblOccupancy.Text = "Occupancy Category:"
        '
        'cmbOccupancy
        '
        Me.cmbOccupancy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbOccupancy.FormattingEnabled = True
        Me.cmbOccupancy.Items.AddRange(New Object() {"II", "III", "IV"})
        Me.cmbOccupancy.Location = New System.Drawing.Point(150, 160)
        Me.cmbOccupancy.Name = "cmbOccupancy"
        Me.cmbOccupancy.Size = New System.Drawing.Size(121, 21)
        Me.cmbOccupancy.TabIndex = 11
        '
        'lblSnowLoad
        '
        Me.lblSnowLoad.AutoSize = True
        Me.lblSnowLoad.Location = New System.Drawing.Point(10, 133)
        Me.lblSnowLoad.Name = "lblSnowLoad"
        Me.lblSnowLoad.Size = New System.Drawing.Size(91, 13)
        Me.lblSnowLoad.TabIndex = 8
        Me.lblSnowLoad.Text = "Snow Load Type:"
        '
        'cmbSnowLoad
        '
        Me.cmbSnowLoad.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbSnowLoad.FormattingEnabled = True
        Me.cmbSnowLoad.Items.AddRange(New Object() {"Pf - Roof", "Pg - Ground"})
        Me.cmbSnowLoad.Location = New System.Drawing.Point(150, 130)
        Me.cmbSnowLoad.Name = "cmbSnowLoad"
        Me.cmbSnowLoad.Size = New System.Drawing.Size(121, 21)
        Me.cmbSnowLoad.TabIndex = 9
        '
        'lblWindSpeed
        '
        Me.lblWindSpeed.AutoSize = True
        Me.lblWindSpeed.Location = New System.Drawing.Point(10, 103)
        Me.lblWindSpeed.Name = "lblWindSpeed"
        Me.lblWindSpeed.Size = New System.Drawing.Size(69, 13)
        Me.lblWindSpeed.TabIndex = 6
        Me.lblWindSpeed.Text = "Wind Speed:"
        '
        'cmbWindSpeed
        '
        Me.cmbWindSpeed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbWindSpeed.FormattingEnabled = True
        Me.cmbWindSpeed.Items.AddRange(New Object() {"115", "120", "125", "130", "135", "140"})
        Me.cmbWindSpeed.Location = New System.Drawing.Point(150, 100)
        Me.cmbWindSpeed.Name = "cmbWindSpeed"
        Me.cmbWindSpeed.Size = New System.Drawing.Size(121, 21)
        Me.cmbWindSpeed.TabIndex = 7
        '
        'lblExposure
        '
        Me.lblExposure.AutoSize = True
        Me.lblExposure.Location = New System.Drawing.Point(10, 73)
        Me.lblExposure.Name = "lblExposure"
        Me.lblExposure.Size = New System.Drawing.Size(99, 13)
        Me.lblExposure.TabIndex = 4
        Me.lblExposure.Text = "Exposure Category:"
        '
        'cmbExposure
        '
        Me.cmbExposure.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbExposure.FormattingEnabled = True
        Me.cmbExposure.Items.AddRange(New Object() {"B", "C", "D"})
        Me.cmbExposure.Location = New System.Drawing.Point(150, 70)
        Me.cmbExposure.Name = "cmbExposure"
        Me.cmbExposure.Size = New System.Drawing.Size(121, 21)
        Me.cmbExposure.TabIndex = 5
        '
        'lblImportance
        '
        Me.lblImportance.AutoSize = True
        Me.lblImportance.Location = New System.Drawing.Point(10, 43)
        Me.lblImportance.Name = "lblImportance"
        Me.lblImportance.Size = New System.Drawing.Size(63, 13)
        Me.lblImportance.TabIndex = 2
        Me.lblImportance.Text = "Importance:"
        '
        'cmbImportance
        '
        Me.cmbImportance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbImportance.FormattingEnabled = True
        Me.cmbImportance.Items.AddRange(New Object() {"Residential", "Commercial", "Agricultural", "Essential"})
        Me.cmbImportance.Location = New System.Drawing.Point(150, 40)
        Me.cmbImportance.Name = "cmbImportance"
        Me.cmbImportance.Size = New System.Drawing.Size(121, 21)
        Me.cmbImportance.TabIndex = 3
        '
        'lblBuildingCode
        '
        Me.lblBuildingCode.AutoSize = True
        Me.lblBuildingCode.Location = New System.Drawing.Point(10, 13)
        Me.lblBuildingCode.Name = "lblBuildingCode"
        Me.lblBuildingCode.Size = New System.Drawing.Size(75, 13)
        Me.lblBuildingCode.TabIndex = 0
        Me.lblBuildingCode.Text = "Building Code:"
        '
        'cmbBuildingCode
        '
        Me.cmbBuildingCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBuildingCode.FormattingEnabled = True
        Me.cmbBuildingCode.Items.AddRange(New Object() {"2021 - IBC", "2021 - IRC", "2018 - IBC", "2018 - IRC", "2012 - IBC", "2012 - IRC"})
        Me.cmbBuildingCode.Location = New System.Drawing.Point(150, 10)
        Me.cmbBuildingCode.Name = "cmbBuildingCode"
        Me.cmbBuildingCode.Size = New System.Drawing.Size(121, 21)
        Me.cmbBuildingCode.TabIndex = 1
        '
        'tabLoads
        '
        Me.tabLoads.Controls.Add(Me.dgvLoads)
        Me.tabLoads.Location = New System.Drawing.Point(4, 22)
        Me.tabLoads.Name = "tabLoads"
        Me.tabLoads.Padding = New System.Windows.Forms.Padding(3)
        Me.tabLoads.Size = New System.Drawing.Size(892, 574)
        Me.tabLoads.TabIndex = 1
        Me.tabLoads.Text = "Loads"
        Me.tabLoads.UseVisualStyleBackColor = True
        '
        'dgvLoads
        '
        Me.dgvLoads.AllowUserToAddRows = False
        Me.dgvLoads.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvLoads.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvLoads.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colCategory, Me.colTCLL, Me.colTCDL, Me.colBCLL, Me.colBCDL, Me.colOCSpacing, Me.colLiveDefl, Me.colTotalDefl, Me.colAbsolute, Me.colDeflection})
        Me.dgvLoads.Location = New System.Drawing.Point(10, 10)
        Me.dgvLoads.Name = "dgvLoads"
        Me.dgvLoads.Size = New System.Drawing.Size(874, 150)
        Me.dgvLoads.TabIndex = 0
        '
        'tabBearing
        '
        Me.tabBearing.Controls.Add(Me.txtCorridorRim)
        Me.tabBearing.Controls.Add(Me.lblCorridorRim)
        Me.tabBearing.Controls.Add(Me.cmbCorridorWall)
        Me.tabBearing.Controls.Add(Me.lblCorridorWall)
        Me.tabBearing.Controls.Add(Me.txtIntRim)
        Me.tabBearing.Controls.Add(Me.lblIntRim)
        Me.tabBearing.Controls.Add(Me.cmbIntWall)
        Me.tabBearing.Controls.Add(Me.lblIntWall)
        Me.tabBearing.Controls.Add(Me.txtExtRim)
        Me.tabBearing.Controls.Add(Me.lblExtRim)
        Me.tabBearing.Controls.Add(Me.cmbExtWall)
        Me.tabBearing.Controls.Add(Me.lblExtWall)
        Me.tabBearing.Location = New System.Drawing.Point(4, 22)
        Me.tabBearing.Name = "tabBearing"
        Me.tabBearing.Padding = New System.Windows.Forms.Padding(3)
        Me.tabBearing.Size = New System.Drawing.Size(892, 574)
        Me.tabBearing.TabIndex = 2
        Me.tabBearing.Text = "Bearing Styles"
        Me.tabBearing.UseVisualStyleBackColor = True
        '
        'txtCorridorRim
        '
        Me.txtCorridorRim.Location = New System.Drawing.Point(200, 160)
        Me.txtCorridorRim.Name = "txtCorridorRim"
        Me.txtCorridorRim.Size = New System.Drawing.Size(200, 20)
        Me.txtCorridorRim.TabIndex = 11
        '
        'lblCorridorRim
        '
        Me.lblCorridorRim.AutoSize = True
        Me.lblCorridorRim.Location = New System.Drawing.Point(10, 163)
        Me.lblCorridorRim.Name = "lblCorridorRim"
        Me.lblCorridorRim.Size = New System.Drawing.Size(67, 13)
        Me.lblCorridorRim.TabIndex = 10
        Me.lblCorridorRim.Text = "Rim/Ribbon:"
        '
        'cmbCorridorWall
        '
        Me.cmbCorridorWall.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbCorridorWall.FormattingEnabled = True
        Me.cmbCorridorWall.Items.AddRange(New Object() {"Top Chord", "Bottom Chord", "Mix of TC/BC", "See Notes"})
        Me.cmbCorridorWall.Location = New System.Drawing.Point(200, 130)
        Me.cmbCorridorWall.Name = "cmbCorridorWall"
        Me.cmbCorridorWall.Size = New System.Drawing.Size(121, 21)
        Me.cmbCorridorWall.TabIndex = 9
        '
        'lblCorridorWall
        '
        Me.lblCorridorWall.AutoSize = True
        Me.lblCorridorWall.Location = New System.Drawing.Point(10, 133)
        Me.lblCorridorWall.Name = "lblCorridorWall"
        Me.lblCorridorWall.Size = New System.Drawing.Size(161, 13)
        Me.lblCorridorWall.TabIndex = 8
        Me.lblCorridorWall.Text = "Corridor Wall Floor Bearing Style:"
        '
        'txtIntRim
        '
        Me.txtIntRim.Location = New System.Drawing.Point(200, 100)
        Me.txtIntRim.Name = "txtIntRim"
        Me.txtIntRim.Size = New System.Drawing.Size(200, 20)
        Me.txtIntRim.TabIndex = 7
        '
        'lblIntRim
        '
        Me.lblIntRim.AutoSize = True
        Me.lblIntRim.Location = New System.Drawing.Point(10, 103)
        Me.lblIntRim.Name = "lblIntRim"
        Me.lblIntRim.Size = New System.Drawing.Size(67, 13)
        Me.lblIntRim.TabIndex = 6
        Me.lblIntRim.Text = "Rim/Ribbon:"
        '
        'cmbIntWall
        '
        Me.cmbIntWall.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbIntWall.FormattingEnabled = True
        Me.cmbIntWall.Items.AddRange(New Object() {"Top Chord", "Bottom Chord", "Mix of TC/BC", "See Notes"})
        Me.cmbIntWall.Location = New System.Drawing.Point(200, 70)
        Me.cmbIntWall.Name = "cmbIntWall"
        Me.cmbIntWall.Size = New System.Drawing.Size(121, 21)
        Me.cmbIntWall.TabIndex = 5
        '
        'lblIntWall
        '
        Me.lblIntWall.AutoSize = True
        Me.lblIntWall.Location = New System.Drawing.Point(10, 73)
        Me.lblIntWall.Name = "lblIntWall"
        Me.lblIntWall.Size = New System.Drawing.Size(137, 13)
        Me.lblIntWall.TabIndex = 4
        Me.lblIntWall.Text = "Int Wall Floor Bearing Style:"
        '
        'txtExtRim
        '
        Me.txtExtRim.Location = New System.Drawing.Point(200, 40)
        Me.txtExtRim.Name = "txtExtRim"
        Me.txtExtRim.Size = New System.Drawing.Size(200, 20)
        Me.txtExtRim.TabIndex = 3
        '
        'lblExtRim
        '
        Me.lblExtRim.AutoSize = True
        Me.lblExtRim.Location = New System.Drawing.Point(10, 43)
        Me.lblExtRim.Name = "lblExtRim"
        Me.lblExtRim.Size = New System.Drawing.Size(67, 13)
        Me.lblExtRim.TabIndex = 2
        Me.lblExtRim.Text = "Rim/Ribbon:"
        '
        'cmbExtWall
        '
        Me.cmbExtWall.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbExtWall.FormattingEnabled = True
        Me.cmbExtWall.Items.AddRange(New Object() {"Top Chord", "Bottom Chord", "Mix of TC/BC", "See Notes"})
        Me.cmbExtWall.Location = New System.Drawing.Point(200, 10)
        Me.cmbExtWall.Name = "cmbExtWall"
        Me.cmbExtWall.Size = New System.Drawing.Size(121, 21)
        Me.cmbExtWall.TabIndex = 1
        '
        'lblExtWall
        '
        Me.lblExtWall.AutoSize = True
        Me.lblExtWall.Location = New System.Drawing.Point(10, 13)
        Me.lblExtWall.Name = "lblExtWall"
        Me.lblExtWall.Size = New System.Drawing.Size(140, 13)
        Me.lblExtWall.TabIndex = 0
        Me.lblExtWall.Text = "Ext Wall Floor Bearing Style:"
        '
        'tabNotes
        '
        Me.tabNotes.Controls.Add(Me.txtGeneralNotes)
        Me.tabNotes.Location = New System.Drawing.Point(4, 22)
        Me.tabNotes.Name = "tabNotes"
        Me.tabNotes.Padding = New System.Windows.Forms.Padding(3)
        Me.tabNotes.Size = New System.Drawing.Size(892, 574)
        Me.tabNotes.TabIndex = 3
        Me.tabNotes.Text = "General Notes"
        Me.tabNotes.UseVisualStyleBackColor = True
        '
        'txtGeneralNotes
        '
        Me.txtGeneralNotes.Location = New System.Drawing.Point(10, 10)
        Me.txtGeneralNotes.Multiline = True
        Me.txtGeneralNotes.Name = "txtGeneralNotes"
        Me.txtGeneralNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtGeneralNotes.Size = New System.Drawing.Size(800, 300)
        Me.txtGeneralNotes.TabIndex = 0
        '
        'tabRoofItems
        '
        Me.tabRoofItems.Controls.Add(Me.dgvRoofItems)
        Me.tabRoofItems.Location = New System.Drawing.Point(4, 22)
        Me.tabRoofItems.Name = "tabRoofItems"
        Me.tabRoofItems.Padding = New System.Windows.Forms.Padding(3)
        Me.tabRoofItems.Size = New System.Drawing.Size(892, 574)
        Me.tabRoofItems.TabIndex = 4
        Me.tabRoofItems.Text = "Roof Truss Items"
        Me.tabRoofItems.UseVisualStyleBackColor = True
        '
        'dgvRoofItems
        '
        Me.dgvRoofItems.AllowUserToAddRows = False
        Me.dgvRoofItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvRoofItems.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colRoofKN, Me.colRoofDesc, Me.colRoofIncluded, Me.colRoofExcluded, Me.colRoofOptional})
        Me.dgvRoofItems.Location = New System.Drawing.Point(10, 10)
        Me.dgvRoofItems.Name = "dgvRoofItems"
        Me.dgvRoofItems.Size = New System.Drawing.Size(771, 556)
        Me.dgvRoofItems.TabIndex = 0
        '
        'colRoofKN
        '
        Me.colRoofKN.Name = "colRoofKN"
        '
        'colRoofDesc
        '
        Me.colRoofDesc.Name = "colRoofDesc"
        Me.colRoofDesc.Width = 200
        '
        'colRoofIncluded
        '
        Me.colRoofIncluded.Name = "colRoofIncluded"
        '
        'colRoofExcluded
        '
        Me.colRoofExcluded.Name = "colRoofExcluded"
        '
        'colRoofOptional
        '
        Me.colRoofOptional.Name = "colRoofOptional"
        '
        'tabFloorItems
        '
        Me.tabFloorItems.Controls.Add(Me.dgvFloorItems)
        Me.tabFloorItems.Location = New System.Drawing.Point(4, 22)
        Me.tabFloorItems.Name = "tabFloorItems"
        Me.tabFloorItems.Padding = New System.Windows.Forms.Padding(3)
        Me.tabFloorItems.Size = New System.Drawing.Size(892, 574)
        Me.tabFloorItems.TabIndex = 5
        Me.tabFloorItems.Text = "Floor Truss Items"
        Me.tabFloorItems.UseVisualStyleBackColor = True
        '
        'dgvFloorItems
        '
        Me.dgvFloorItems.AllowUserToAddRows = False
        Me.dgvFloorItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvFloorItems.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colFloorKN, Me.colFloorDesc, Me.colFloorIncluded, Me.colFloorExcluded, Me.colFloorOptional})
        Me.dgvFloorItems.Location = New System.Drawing.Point(10, 10)
        Me.dgvFloorItems.Name = "dgvFloorItems"
        Me.dgvFloorItems.Size = New System.Drawing.Size(697, 556)
        Me.dgvFloorItems.TabIndex = 0
        '
        'colFloorKN
        '
        Me.colFloorKN.Name = "colFloorKN"
        '
        'colFloorDesc
        '
        Me.colFloorDesc.Name = "colFloorDesc"
        Me.colFloorDesc.Width = 200
        '
        'colFloorIncluded
        '
        Me.colFloorIncluded.Name = "colFloorIncluded"
        '
        'colFloorExcluded
        '
        Me.colFloorExcluded.Name = "colFloorExcluded"
        '
        'colFloorOptional
        '
        Me.colFloorOptional.Name = "colFloorOptional"
        '
        'tabWallItems
        '
        Me.tabWallItems.Controls.Add(Me.dgvWallItems)
        Me.tabWallItems.Location = New System.Drawing.Point(4, 22)
        Me.tabWallItems.Name = "tabWallItems"
        Me.tabWallItems.Padding = New System.Windows.Forms.Padding(3)
        Me.tabWallItems.Size = New System.Drawing.Size(892, 574)
        Me.tabWallItems.TabIndex = 6
        Me.tabWallItems.Text = "Wall Panel Items"
        Me.tabWallItems.UseVisualStyleBackColor = True
        '
        'dgvWallItems
        '
        Me.dgvWallItems.AllowUserToAddRows = False
        Me.dgvWallItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvWallItems.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colWallKN, Me.colWallDesc, Me.colWallIncluded, Me.colWallExcluded, Me.colWallOptional})
        Me.dgvWallItems.Location = New System.Drawing.Point(10, 10)
        Me.dgvWallItems.Name = "dgvWallItems"
        Me.dgvWallItems.Size = New System.Drawing.Size(600, 400)
        Me.dgvWallItems.TabIndex = 0
        '
        'colWallKN
        '
        Me.colWallKN.Name = "colWallKN"
        '
        'colWallDesc
        '
        Me.colWallDesc.Name = "colWallDesc"
        Me.colWallDesc.Width = 200
        '
        'colWallIncluded
        '
        Me.colWallIncluded.Name = "colWallIncluded"
        '
        'colWallExcluded
        '
        Me.colWallExcluded.Name = "colWallExcluded"
        '
        'colWallOptional
        '
        Me.colWallOptional.Name = "colWallOptional"
        '
        'colCategory
        '
        Me.colCategory.HeaderText = "Category"
        Me.colCategory.Name = "colCategory"
        '
        'colTCLL
        '
        Me.colTCLL.HeaderText = "TCLL"
        Me.colTCLL.Items.AddRange(New Object() {"30", "20"})
        Me.colTCLL.Name = "colTCLL"
        Me.colTCLL.Width = 65
        '
        'colTCDL
        '
        Me.colTCDL.HeaderText = "TCDL"
        Me.colTCDL.Items.AddRange(New Object() {"5"})
        Me.colTCDL.Name = "colTCDL"
        Me.colTCDL.Width = 65
        '
        'colBCLL
        '
        Me.colBCLL.HeaderText = "BCLL"
        Me.colBCLL.Items.AddRange(New Object() {"0"})
        Me.colBCLL.Name = "colBCLL"
        Me.colBCLL.Width = 65
        '
        'colBCDL
        '
        Me.colBCDL.HeaderText = "BCDL"
        Me.colBCDL.Items.AddRange(New Object() {"0"})
        Me.colBCDL.Name = "colBCDL"
        Me.colBCDL.Width = 65
        '
        'colOCSpacing
        '
        Me.colOCSpacing.HeaderText = "OC Spacing"
        Me.colOCSpacing.Items.AddRange(New Object() {"12"""})
        Me.colOCSpacing.Name = "colOCSpacing"
        Me.colOCSpacing.Width = 75
        '
        'colLiveDefl
        '
        Me.colLiveDefl.HeaderText = "Live Load Deflection"
        Me.colLiveDefl.Items.AddRange(New Object() {"180"})
        Me.colLiveDefl.Name = "colLiveDefl"
        Me.colLiveDefl.Width = 75
        '
        'colTotalDefl
        '
        Me.colTotalDefl.HeaderText = "Total Load Deflection"
        Me.colTotalDefl.Items.AddRange(New Object() {"180"})
        Me.colTotalDefl.Name = "colTotalDefl"
        Me.colTotalDefl.Width = 75
        '
        'colAbsolute
        '
        Me.colAbsolute.HeaderText = "Absolute"
        Me.colAbsolute.Items.AddRange(New Object() {".25"""})
        Me.colAbsolute.Name = "colAbsolute"
        Me.colAbsolute.Width = 75
        '
        'colDeflection
        '
        Me.colDeflection.HeaderText = "Deflection"
        Me.colDeflection.Items.AddRange(New Object() {"LL"})
        Me.colDeflection.Name = "colDeflection"
        Me.colDeflection.Width = 75
        '
        'frmInclusionsExclusions
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 600)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.tabControl)
        Me.Name = "frmInclusionsExclusions"
        Me.Text = "Inclusions / Exclusions"
        Me.tabControl.ResumeLayout(False)
        Me.tabDesignInfo.ResumeLayout(False)
        Me.tabDesignInfo.PerformLayout()
        Me.tabLoads.ResumeLayout(False)
        CType(Me.dgvLoads, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabBearing.ResumeLayout(False)
        Me.tabBearing.PerformLayout()
        Me.tabNotes.ResumeLayout(False)
        Me.tabNotes.PerformLayout()
        Me.tabRoofItems.ResumeLayout(False)
        CType(Me.dgvRoofItems, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabFloorItems.ResumeLayout(False)
        CType(Me.dgvFloorItems, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabWallItems.ResumeLayout(False)
        CType(Me.dgvWallItems, System.ComponentModel.ISupportInitialize).EndInit()
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
    Friend WithEvents tabLoads As TabPage
    Friend WithEvents dgvLoads As DataGridView
    Friend WithEvents tabBearing As TabPage
    Friend WithEvents lblCorridorRim As Label
    Friend WithEvents txtCorridorRim As TextBox
    Friend WithEvents lblCorridorWall As Label
    Friend WithEvents cmbCorridorWall As ComboBox
    Friend WithEvents lblIntRim As Label
    Friend WithEvents txtIntRim As TextBox
    Friend WithEvents lblIntWall As Label
    Friend WithEvents cmbIntWall As ComboBox
    Friend WithEvents lblExtRim As Label
    Friend WithEvents txtExtRim As TextBox
    Friend WithEvents lblExtWall As Label
    Friend WithEvents cmbExtWall As ComboBox
    Friend WithEvents tabNotes As TabPage
    Friend WithEvents txtGeneralNotes As TextBox
    Friend WithEvents tabRoofItems As TabPage
    Friend WithEvents dgvRoofItems As DataGridView
    Friend WithEvents tabFloorItems As TabPage
    Friend WithEvents dgvFloorItems As DataGridView
    Friend WithEvents tabWallItems As TabPage
    Friend WithEvents dgvWallItems As DataGridView
    Friend WithEvents btnSave As Button
    Friend WithEvents colFloorKN As DataGridViewTextBoxColumn
    Friend WithEvents colFloorDesc As DataGridViewTextBoxColumn
    Friend WithEvents colFloorIncluded As DataGridViewCheckBoxColumn
    Friend WithEvents colFloorExcluded As DataGridViewCheckBoxColumn
    Friend WithEvents colFloorOptional As DataGridViewCheckBoxColumn
    Friend WithEvents colWallKN As DataGridViewTextBoxColumn
    Friend WithEvents colWallDesc As DataGridViewTextBoxColumn
    Friend WithEvents colWallIncluded As DataGridViewCheckBoxColumn
    Friend WithEvents colWallExcluded As DataGridViewCheckBoxColumn
    Friend WithEvents colWallOptional As DataGridViewCheckBoxColumn
    Friend WithEvents TabPage1 As TabPage
    Friend WithEvents colRoofKN As DataGridViewTextBoxColumn
    Friend WithEvents colRoofDesc As DataGridViewTextBoxColumn
    Friend WithEvents colRoofIncluded As DataGridViewCheckBoxColumn
    Friend WithEvents colRoofExcluded As DataGridViewCheckBoxColumn
    Friend WithEvents colRoofOptional As DataGridViewCheckBoxColumn
    Friend WithEvents colCategory As DataGridViewTextBoxColumn
    Friend WithEvents colTCLL As DataGridViewComboBoxColumn
    Friend WithEvents colTCDL As DataGridViewComboBoxColumn
    Friend WithEvents colBCLL As DataGridViewComboBoxColumn
    Friend WithEvents colBCDL As DataGridViewComboBoxColumn
    Friend WithEvents colOCSpacing As DataGridViewComboBoxColumn
    Friend WithEvents colLiveDefl As DataGridViewComboBoxColumn
    Friend WithEvents colTotalDefl As DataGridViewComboBoxColumn
    Friend WithEvents colAbsolute As DataGridViewComboBoxColumn
    Friend WithEvents colDeflection As DataGridViewComboBoxColumn
End Class