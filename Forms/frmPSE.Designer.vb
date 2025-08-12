<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FrmPSE
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
        Me.TreeViewLevels = New System.Windows.Forms.TreeView()
        Me.ContextMenuLevels = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.mnuCopyUnits = New System.Windows.Forms.ToolStripMenuItem()
        Me.mnuPasteUnits = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabControlData = New System.Windows.Forms.TabControl()
        Me.TabUnitBased = New System.Windows.Forms.TabPage()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtTotMargin = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtTotalDeliveryCost = New System.Windows.Forms.TextBox()
        Me.ChkDetailedView = New System.Windows.Forms.CheckBox()
        Me.btnReuseActualUnit = New System.Windows.Forms.Button()
        Me.ListboxExistingActualUnits = New System.Windows.Forms.ListBox()
        Me.ContextMenuActualUnits = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteActualUnitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BtnImportLevelData = New System.Windows.Forms.Button()
        Me.LblTotalMargin = New System.Windows.Forms.Label()
        Me.TxtTotalMargin = New System.Windows.Forms.TextBox()
        Me.LblTotalSellPrice = New System.Windows.Forms.Label()
        Me.BtnRecalculate = New System.Windows.Forms.Button()
        Me.BtnFinish = New System.Windows.Forms.Button()
        Me.TxtTotalSellPrice = New System.Windows.Forms.TextBox()
        Me.LblTotalOverallCost = New System.Windows.Forms.Label()
        Me.TxtTotalOverallCost = New System.Windows.Forms.TextBox()
        Me.LblTotalItemCost = New System.Windows.Forms.Label()
        Me.TxtTotalItemCost = New System.Windows.Forms.TextBox()
        Me.LblTotalManHours = New System.Windows.Forms.Label()
        Me.TxtTotalManHours = New System.Windows.Forms.TextBox()
        Me.LblTotalJobSuppliesCost = New System.Windows.Forms.Label()
        Me.TxtTotalJobSuppliesCost = New System.Windows.Forms.TextBox()
        Me.LblTotalMGMTLabor = New System.Windows.Forms.Label()
        Me.TxtTotalMGMTLabor = New System.Windows.Forms.TextBox()
        Me.LblTotalDesignLabor = New System.Windows.Forms.Label()
        Me.TxtTotalDesignLabor = New System.Windows.Forms.TextBox()
        Me.LblTotalManufLaborCost = New System.Windows.Forms.Label()
        Me.TxtTotalManufLaborCost = New System.Windows.Forms.TextBox()
        Me.LblTotalPlateCost = New System.Windows.Forms.Label()
        Me.TxtTotalPlateCost = New System.Windows.Forms.TextBox()
        Me.LblTotalLumberCost = New System.Windows.Forms.Label()
        Me.TxtTotalLumberCost = New System.Windows.Forms.TextBox()
        Me.LblTotalBDFT = New System.Windows.Forms.Label()
        Me.TxtTotalBDFT = New System.Windows.Forms.TextBox()
        Me.LblTotalLF = New System.Windows.Forms.Label()
        Me.TxtTotalLF = New System.Windows.Forms.TextBox()
        Me.LblTotalQuantity = New System.Windows.Forms.Label()
        Me.TxtTotalQuantity = New System.Windows.Forms.TextBox()
        Me.LblTotalPlanSQFT = New System.Windows.Forms.Label()
        Me.TxtTotalPlanSQFT = New System.Windows.Forms.TextBox()
        Me.ListBoxAvailableUnits = New System.Windows.Forms.ListBox()
        Me.BtnConvertToActualUnit = New System.Windows.Forms.Button()
        Me.DataGridViewAssigned = New System.Windows.Forms.DataGridView()
        Me.tabRawUnit = New System.Windows.Forms.TabPage()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.PanelDetails = New System.Windows.Forms.Panel()
        Me.btnDeleteUnit = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.LblActualUnitQuantity = New System.Windows.Forms.Label()
        Me.TxtActualUnitQuantity = New System.Windows.Forms.TextBox()
        Me.LblTotalSellPricePerSQFT = New System.Windows.Forms.Label()
        Me.TxtTotalSellPricePerSQFT = New System.Windows.Forms.TextBox()
        Me.LblDeliveryCostPerSQFT = New System.Windows.Forms.Label()
        Me.TxtDeliveryCostPerSQFT = New System.Windows.Forms.TextBox()
        Me.LblOverallCostPerSQFT = New System.Windows.Forms.Label()
        Me.TxtOverallCostPerSQFT = New System.Windows.Forms.TextBox()
        Me.LblItemCostPerSQFT = New System.Windows.Forms.Label()
        Me.TxtItemCostPerSQFT = New System.Windows.Forms.TextBox()
        Me.LblManHoursPerSQFT = New System.Windows.Forms.Label()
        Me.TxtManHoursPerSQFT = New System.Windows.Forms.TextBox()
        Me.LblJobSuppliesPerSQFT = New System.Windows.Forms.Label()
        Me.TxtJobSuppliesPerSQFT = New System.Windows.Forms.TextBox()
        Me.LblMGMTLaborPerSQFT = New System.Windows.Forms.Label()
        Me.TxtMGMTLaborPerSQFT = New System.Windows.Forms.TextBox()
        Me.LblDesignLaborPerSQFT = New System.Windows.Forms.Label()
        Me.TxtDesignLaborPerSQFT = New System.Windows.Forms.TextBox()
        Me.LblManufLaborPerSQFT = New System.Windows.Forms.Label()
        Me.TxtManufLaborPerSQFT = New System.Windows.Forms.TextBox()
        Me.LblBDFTPerSQFT = New System.Windows.Forms.Label()
        Me.TxtBDFTPerSQFT = New System.Windows.Forms.TextBox()
        Me.LblPlatePerSQFT = New System.Windows.Forms.Label()
        Me.TxtPlatePerSQFT = New System.Windows.Forms.TextBox()
        Me.LblLumberPerSQFT = New System.Windows.Forms.Label()
        Me.TxtLumberPerSQFT = New System.Windows.Forms.TextBox()
        Me.BtnToggleDetails = New System.Windows.Forms.Button()
        Me.LblUnitName = New System.Windows.Forms.Label()
        Me.TxtUnitName = New System.Windows.Forms.TextBox()
        Me.LblPlanSQFT = New System.Windows.Forms.Label()
        Me.TxtPlanSQFT = New System.Windows.Forms.TextBox()
        Me.LblOptionalAdder = New System.Windows.Forms.Label()
        Me.TxtOptionalAdder = New System.Windows.Forms.TextBox()
        Me.LblUnitType = New System.Windows.Forms.Label()
        Me.CmbUnitType = New System.Windows.Forms.ComboBox()
        Me.BtnCancel = New System.Windows.Forms.Button()
        Me.LblReferencedRawUnit = New System.Windows.Forms.Label()
        Me.TxtReferencedRawUnit = New System.Windows.Forms.TextBox()
        Me.chkAttachToLevel = New System.Windows.Forms.CheckBox()
        Me.ContextMenuLevels.SuspendLayout()
        Me.TabControlData.SuspendLayout()
        Me.TabUnitBased.SuspendLayout()
        Me.ContextMenuActualUnits.SuspendLayout()
        CType(Me.DataGridViewAssigned, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tabRawUnit.SuspendLayout()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.PanelDetails.SuspendLayout()
        Me.SuspendLayout()
        '
        'TreeViewLevels
        '
        Me.TreeViewLevels.ContextMenuStrip = Me.ContextMenuLevels
        Me.TreeViewLevels.Dock = System.Windows.Forms.DockStyle.Left
        Me.TreeViewLevels.HideSelection = False
        Me.TreeViewLevels.Location = New System.Drawing.Point(0, 0)
        Me.TreeViewLevels.Name = "TreeViewLevels"
        Me.TreeViewLevels.Size = New System.Drawing.Size(230, 647)
        Me.TreeViewLevels.TabIndex = 0
        '
        'ContextMenuLevels
        '
        Me.ContextMenuLevels.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.mnuCopyUnits, Me.mnuPasteUnits})
        Me.ContextMenuLevels.Name = "ContextMenuLevels"
        Me.ContextMenuLevels.Size = New System.Drawing.Size(133, 48)
        '
        'mnuCopyUnits
        '
        Me.mnuCopyUnits.Name = "mnuCopyUnits"
        Me.mnuCopyUnits.Size = New System.Drawing.Size(132, 22)
        Me.mnuCopyUnits.Text = "Copy Units"
        '
        'mnuPasteUnits
        '
        Me.mnuPasteUnits.Name = "mnuPasteUnits"
        Me.mnuPasteUnits.Size = New System.Drawing.Size(132, 22)
        Me.mnuPasteUnits.Text = "Paste Units"
        '
        'TabControlData
        '
        Me.TabControlData.Controls.Add(Me.TabUnitBased)
        Me.TabControlData.Controls.Add(Me.tabRawUnit)
        Me.TabControlData.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControlData.Location = New System.Drawing.Point(230, 0)
        Me.TabControlData.Name = "TabControlData"
        Me.TabControlData.SelectedIndex = 0
        Me.TabControlData.Size = New System.Drawing.Size(887, 647)
        Me.TabControlData.TabIndex = 1
        '
        'TabUnitBased
        '
        Me.TabUnitBased.Controls.Add(Me.Label2)
        Me.TabUnitBased.Controls.Add(Me.txtTotMargin)
        Me.TabUnitBased.Controls.Add(Me.Label1)
        Me.TabUnitBased.Controls.Add(Me.txtTotalDeliveryCost)
        Me.TabUnitBased.Controls.Add(Me.ChkDetailedView)
        Me.TabUnitBased.Controls.Add(Me.btnReuseActualUnit)
        Me.TabUnitBased.Controls.Add(Me.ListboxExistingActualUnits)
        Me.TabUnitBased.Controls.Add(Me.BtnImportLevelData)
        Me.TabUnitBased.Controls.Add(Me.LblTotalMargin)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalMargin)
        Me.TabUnitBased.Controls.Add(Me.LblTotalSellPrice)
        Me.TabUnitBased.Controls.Add(Me.BtnRecalculate)
        Me.TabUnitBased.Controls.Add(Me.BtnFinish)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalSellPrice)
        Me.TabUnitBased.Controls.Add(Me.LblTotalOverallCost)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalOverallCost)
        Me.TabUnitBased.Controls.Add(Me.LblTotalItemCost)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalItemCost)
        Me.TabUnitBased.Controls.Add(Me.LblTotalManHours)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalManHours)
        Me.TabUnitBased.Controls.Add(Me.LblTotalJobSuppliesCost)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalJobSuppliesCost)
        Me.TabUnitBased.Controls.Add(Me.LblTotalMGMTLabor)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalMGMTLabor)
        Me.TabUnitBased.Controls.Add(Me.LblTotalDesignLabor)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalDesignLabor)
        Me.TabUnitBased.Controls.Add(Me.LblTotalManufLaborCost)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalManufLaborCost)
        Me.TabUnitBased.Controls.Add(Me.LblTotalPlateCost)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalPlateCost)
        Me.TabUnitBased.Controls.Add(Me.LblTotalLumberCost)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalLumberCost)
        Me.TabUnitBased.Controls.Add(Me.LblTotalBDFT)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalBDFT)
        Me.TabUnitBased.Controls.Add(Me.LblTotalLF)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalLF)
        Me.TabUnitBased.Controls.Add(Me.LblTotalQuantity)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalQuantity)
        Me.TabUnitBased.Controls.Add(Me.LblTotalPlanSQFT)
        Me.TabUnitBased.Controls.Add(Me.TxtTotalPlanSQFT)
        Me.TabUnitBased.Controls.Add(Me.ListBoxAvailableUnits)
        Me.TabUnitBased.Controls.Add(Me.BtnConvertToActualUnit)
        Me.TabUnitBased.Controls.Add(Me.DataGridViewAssigned)
        Me.TabUnitBased.Location = New System.Drawing.Point(4, 22)
        Me.TabUnitBased.Name = "TabUnitBased"
        Me.TabUnitBased.Size = New System.Drawing.Size(879, 621)
        Me.TabUnitBased.TabIndex = 0
        Me.TabUnitBased.Text = "Units"
        '
        'Label2
        '
        Me.Label2.Location = New System.Drawing.Point(325, 195)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(82, 20)
        Me.Label2.TabIndex = 58
        Me.Label2.Text = "Margin w/ Del:"
        '
        'txtTotMargin
        '
        Me.txtTotMargin.Location = New System.Drawing.Point(413, 192)
        Me.txtTotMargin.Name = "txtTotMargin"
        Me.txtTotMargin.ReadOnly = True
        Me.txtTotMargin.Size = New System.Drawing.Size(100, 20)
        Me.txtTotMargin.TabIndex = 57
        '
        'Label1
        '
        Me.Label1.Location = New System.Drawing.Point(520, 221)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(67, 20)
        Me.Label1.TabIndex = 56
        Me.Label1.Text = "Del. $$:"
        '
        'txtTotalDeliveryCost
        '
        Me.txtTotalDeliveryCost.Location = New System.Drawing.Point(612, 218)
        Me.txtTotalDeliveryCost.Name = "txtTotalDeliveryCost"
        Me.txtTotalDeliveryCost.ReadOnly = True
        Me.txtTotalDeliveryCost.Size = New System.Drawing.Size(100, 20)
        Me.txtTotalDeliveryCost.TabIndex = 55
        '
        'ChkDetailedView
        '
        Me.ChkDetailedView.AutoSize = True
        Me.ChkDetailedView.Location = New System.Drawing.Point(10, 250)
        Me.ChkDetailedView.Name = "ChkDetailedView"
        Me.ChkDetailedView.Size = New System.Drawing.Size(91, 17)
        Me.ChkDetailedView.TabIndex = 54
        Me.ChkDetailedView.Text = "Detailed View"
        Me.ChkDetailedView.UseVisualStyleBackColor = True
        '
        'btnReuseActualUnit
        '
        Me.btnReuseActualUnit.Location = New System.Drawing.Point(156, 215)
        Me.btnReuseActualUnit.Name = "btnReuseActualUnit"
        Me.btnReuseActualUnit.Size = New System.Drawing.Size(135, 29)
        Me.btnReuseActualUnit.TabIndex = 53
        Me.btnReuseActualUnit.Text = "Add Actual Unit to Level"
        Me.btnReuseActualUnit.UseVisualStyleBackColor = True
        '
        'ListboxExistingActualUnits
        '
        Me.ListboxExistingActualUnits.ContextMenuStrip = Me.ContextMenuActualUnits
        Me.ListboxExistingActualUnits.FormattingEnabled = True
        Me.ListboxExistingActualUnits.Location = New System.Drawing.Point(156, 10)
        Me.ListboxExistingActualUnits.Name = "ListboxExistingActualUnits"
        Me.ListboxExistingActualUnits.Size = New System.Drawing.Size(163, 199)
        Me.ListboxExistingActualUnits.TabIndex = 52
        '
        'ContextMenuActualUnits
        '
        Me.ContextMenuActualUnits.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EditToolStripMenuItem, Me.DeleteActualUnitToolStripMenuItem})
        Me.ContextMenuActualUnits.Name = "ContextMenuActualUnits"
        Me.ContextMenuActualUnits.Size = New System.Drawing.Size(170, 48)
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.EditToolStripMenuItem.Text = "Edit Actual Unit"
        '
        'DeleteActualUnitToolStripMenuItem
        '
        Me.DeleteActualUnitToolStripMenuItem.Name = "DeleteActualUnitToolStripMenuItem"
        Me.DeleteActualUnitToolStripMenuItem.Size = New System.Drawing.Size(169, 22)
        Me.DeleteActualUnitToolStripMenuItem.Text = "Delete Actual Unit"
        '
        'BtnImportLevelData
        '
        Me.BtnImportLevelData.Location = New System.Drawing.Point(340, 215)
        Me.BtnImportLevelData.Name = "BtnImportLevelData"
        Me.BtnImportLevelData.Size = New System.Drawing.Size(150, 30)
        Me.BtnImportLevelData.TabIndex = 0
        Me.BtnImportLevelData.Text = "Import Level Data (CSV)"
        '
        'LblTotalMargin
        '
        Me.LblTotalMargin.Location = New System.Drawing.Point(325, 65)
        Me.LblTotalMargin.Name = "LblTotalMargin"
        Me.LblTotalMargin.Size = New System.Drawing.Size(67, 20)
        Me.LblTotalMargin.TabIndex = 51
        Me.LblTotalMargin.Text = "Margin $$:"
        '
        'TxtTotalMargin
        '
        Me.TxtTotalMargin.Location = New System.Drawing.Point(413, 62)
        Me.TxtTotalMargin.Name = "TxtTotalMargin"
        Me.TxtTotalMargin.ReadOnly = True
        Me.TxtTotalMargin.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalMargin.TabIndex = 50
        '
        'LblTotalSellPrice
        '
        Me.LblTotalSellPrice.Location = New System.Drawing.Point(325, 13)
        Me.LblTotalSellPrice.Name = "LblTotalSellPrice"
        Me.LblTotalSellPrice.Size = New System.Drawing.Size(67, 20)
        Me.LblTotalSellPrice.TabIndex = 49
        Me.LblTotalSellPrice.Text = "Sell Price:"
        '
        'BtnRecalculate
        '
        Me.BtnRecalculate.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnRecalculate.Location = New System.Drawing.Point(661, 566)
        Me.BtnRecalculate.Name = "BtnRecalculate"
        Me.BtnRecalculate.Size = New System.Drawing.Size(100, 30)
        Me.BtnRecalculate.TabIndex = 3
        Me.BtnRecalculate.Text = "Recalculate"
        '
        'BtnFinish
        '
        Me.BtnFinish.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnFinish.Location = New System.Drawing.Point(767, 566)
        Me.BtnFinish.Name = "BtnFinish"
        Me.BtnFinish.Size = New System.Drawing.Size(100, 30)
        Me.BtnFinish.TabIndex = 4
        Me.BtnFinish.Text = "Close"
        '
        'TxtTotalSellPrice
        '
        Me.TxtTotalSellPrice.Location = New System.Drawing.Point(413, 10)
        Me.TxtTotalSellPrice.Name = "TxtTotalSellPrice"
        Me.TxtTotalSellPrice.ReadOnly = True
        Me.TxtTotalSellPrice.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalSellPrice.TabIndex = 48
        '
        'LblTotalOverallCost
        '
        Me.LblTotalOverallCost.Location = New System.Drawing.Point(325, 39)
        Me.LblTotalOverallCost.Name = "LblTotalOverallCost"
        Me.LblTotalOverallCost.Size = New System.Drawing.Size(67, 20)
        Me.LblTotalOverallCost.TabIndex = 47
        Me.LblTotalOverallCost.Text = "Overall Cost:"
        '
        'TxtTotalOverallCost
        '
        Me.TxtTotalOverallCost.Location = New System.Drawing.Point(413, 36)
        Me.TxtTotalOverallCost.Name = "TxtTotalOverallCost"
        Me.TxtTotalOverallCost.ReadOnly = True
        Me.TxtTotalOverallCost.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalOverallCost.TabIndex = 46
        '
        'LblTotalItemCost
        '
        Me.LblTotalItemCost.Location = New System.Drawing.Point(520, 195)
        Me.LblTotalItemCost.Name = "LblTotalItemCost"
        Me.LblTotalItemCost.Size = New System.Drawing.Size(86, 20)
        Me.LblTotalItemCost.TabIndex = 45
        Me.LblTotalItemCost.Text = "Item $$:"
        '
        'TxtTotalItemCost
        '
        Me.TxtTotalItemCost.Location = New System.Drawing.Point(612, 192)
        Me.TxtTotalItemCost.Name = "TxtTotalItemCost"
        Me.TxtTotalItemCost.ReadOnly = True
        Me.TxtTotalItemCost.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalItemCost.TabIndex = 44
        '
        'LblTotalManHours
        '
        Me.LblTotalManHours.Location = New System.Drawing.Point(520, 91)
        Me.LblTotalManHours.Name = "LblTotalManHours"
        Me.LblTotalManHours.Size = New System.Drawing.Size(86, 20)
        Me.LblTotalManHours.TabIndex = 43
        Me.LblTotalManHours.Text = "Man Hours:"
        '
        'TxtTotalManHours
        '
        Me.TxtTotalManHours.Location = New System.Drawing.Point(612, 88)
        Me.TxtTotalManHours.Name = "TxtTotalManHours"
        Me.TxtTotalManHours.ReadOnly = True
        Me.TxtTotalManHours.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalManHours.TabIndex = 42
        '
        'LblTotalJobSuppliesCost
        '
        Me.LblTotalJobSuppliesCost.Location = New System.Drawing.Point(520, 169)
        Me.LblTotalJobSuppliesCost.Name = "LblTotalJobSuppliesCost"
        Me.LblTotalJobSuppliesCost.Size = New System.Drawing.Size(86, 20)
        Me.LblTotalJobSuppliesCost.TabIndex = 41
        Me.LblTotalJobSuppliesCost.Text = "Job Supplies $$:"
        '
        'TxtTotalJobSuppliesCost
        '
        Me.TxtTotalJobSuppliesCost.Location = New System.Drawing.Point(612, 166)
        Me.TxtTotalJobSuppliesCost.Name = "TxtTotalJobSuppliesCost"
        Me.TxtTotalJobSuppliesCost.ReadOnly = True
        Me.TxtTotalJobSuppliesCost.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalJobSuppliesCost.TabIndex = 40
        '
        'LblTotalMGMTLabor
        '
        Me.LblTotalMGMTLabor.Location = New System.Drawing.Point(520, 143)
        Me.LblTotalMGMTLabor.Name = "LblTotalMGMTLabor"
        Me.LblTotalMGMTLabor.Size = New System.Drawing.Size(86, 20)
        Me.LblTotalMGMTLabor.TabIndex = 39
        Me.LblTotalMGMTLabor.Text = "MGMT Labor $$:"
        '
        'TxtTotalMGMTLabor
        '
        Me.TxtTotalMGMTLabor.Location = New System.Drawing.Point(612, 140)
        Me.TxtTotalMGMTLabor.Name = "TxtTotalMGMTLabor"
        Me.TxtTotalMGMTLabor.ReadOnly = True
        Me.TxtTotalMGMTLabor.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalMGMTLabor.TabIndex = 38
        '
        'LblTotalDesignLabor
        '
        Me.LblTotalDesignLabor.Location = New System.Drawing.Point(520, 117)
        Me.LblTotalDesignLabor.Name = "LblTotalDesignLabor"
        Me.LblTotalDesignLabor.Size = New System.Drawing.Size(86, 20)
        Me.LblTotalDesignLabor.TabIndex = 37
        Me.LblTotalDesignLabor.Text = "Design Labor $$:"
        '
        'TxtTotalDesignLabor
        '
        Me.TxtTotalDesignLabor.Location = New System.Drawing.Point(612, 114)
        Me.TxtTotalDesignLabor.Name = "TxtTotalDesignLabor"
        Me.TxtTotalDesignLabor.ReadOnly = True
        Me.TxtTotalDesignLabor.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalDesignLabor.TabIndex = 36
        '
        'LblTotalManufLaborCost
        '
        Me.LblTotalManufLaborCost.Location = New System.Drawing.Point(519, 62)
        Me.LblTotalManufLaborCost.Name = "LblTotalManufLaborCost"
        Me.LblTotalManufLaborCost.Size = New System.Drawing.Size(86, 20)
        Me.LblTotalManufLaborCost.TabIndex = 35
        Me.LblTotalManufLaborCost.Text = "Manuf Labor $$:"
        '
        'TxtTotalManufLaborCost
        '
        Me.TxtTotalManufLaborCost.Location = New System.Drawing.Point(612, 62)
        Me.TxtTotalManufLaborCost.Name = "TxtTotalManufLaborCost"
        Me.TxtTotalManufLaborCost.ReadOnly = True
        Me.TxtTotalManufLaborCost.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalManufLaborCost.TabIndex = 34
        '
        'LblTotalPlateCost
        '
        Me.LblTotalPlateCost.Location = New System.Drawing.Point(519, 36)
        Me.LblTotalPlateCost.Name = "LblTotalPlateCost"
        Me.LblTotalPlateCost.Size = New System.Drawing.Size(86, 20)
        Me.LblTotalPlateCost.TabIndex = 33
        Me.LblTotalPlateCost.Text = "Plate $$:"
        '
        'TxtTotalPlateCost
        '
        Me.TxtTotalPlateCost.Location = New System.Drawing.Point(612, 36)
        Me.TxtTotalPlateCost.Name = "TxtTotalPlateCost"
        Me.TxtTotalPlateCost.ReadOnly = True
        Me.TxtTotalPlateCost.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalPlateCost.TabIndex = 32
        '
        'LblTotalLumberCost
        '
        Me.LblTotalLumberCost.Location = New System.Drawing.Point(519, 10)
        Me.LblTotalLumberCost.Name = "LblTotalLumberCost"
        Me.LblTotalLumberCost.Size = New System.Drawing.Size(86, 20)
        Me.LblTotalLumberCost.TabIndex = 31
        Me.LblTotalLumberCost.Text = "Lumber $$:"
        '
        'TxtTotalLumberCost
        '
        Me.TxtTotalLumberCost.Location = New System.Drawing.Point(612, 10)
        Me.TxtTotalLumberCost.Name = "TxtTotalLumberCost"
        Me.TxtTotalLumberCost.ReadOnly = True
        Me.TxtTotalLumberCost.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalLumberCost.TabIndex = 30
        '
        'LblTotalBDFT
        '
        Me.LblTotalBDFT.Location = New System.Drawing.Point(325, 91)
        Me.LblTotalBDFT.Name = "LblTotalBDFT"
        Me.LblTotalBDFT.Size = New System.Drawing.Size(67, 20)
        Me.LblTotalBDFT.TabIndex = 29
        Me.LblTotalBDFT.Text = "BDFT:"
        '
        'TxtTotalBDFT
        '
        Me.TxtTotalBDFT.Location = New System.Drawing.Point(413, 88)
        Me.TxtTotalBDFT.Name = "TxtTotalBDFT"
        Me.TxtTotalBDFT.ReadOnly = True
        Me.TxtTotalBDFT.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalBDFT.TabIndex = 28
        '
        'LblTotalLF
        '
        Me.LblTotalLF.Location = New System.Drawing.Point(325, 117)
        Me.LblTotalLF.Name = "LblTotalLF"
        Me.LblTotalLF.Size = New System.Drawing.Size(67, 20)
        Me.LblTotalLF.TabIndex = 27
        Me.LblTotalLF.Text = "LF:"
        '
        'TxtTotalLF
        '
        Me.TxtTotalLF.Location = New System.Drawing.Point(413, 114)
        Me.TxtTotalLF.Name = "TxtTotalLF"
        Me.TxtTotalLF.ReadOnly = True
        Me.TxtTotalLF.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalLF.TabIndex = 26
        '
        'LblTotalQuantity
        '
        Me.LblTotalQuantity.Location = New System.Drawing.Point(325, 143)
        Me.LblTotalQuantity.Name = "LblTotalQuantity"
        Me.LblTotalQuantity.Size = New System.Drawing.Size(67, 20)
        Me.LblTotalQuantity.TabIndex = 25
        Me.LblTotalQuantity.Text = "Unit Quantity:"
        '
        'TxtTotalQuantity
        '
        Me.TxtTotalQuantity.Location = New System.Drawing.Point(413, 140)
        Me.TxtTotalQuantity.Name = "TxtTotalQuantity"
        Me.TxtTotalQuantity.ReadOnly = True
        Me.TxtTotalQuantity.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalQuantity.TabIndex = 24
        '
        'LblTotalPlanSQFT
        '
        Me.LblTotalPlanSQFT.Location = New System.Drawing.Point(325, 169)
        Me.LblTotalPlanSQFT.Name = "LblTotalPlanSQFT"
        Me.LblTotalPlanSQFT.Size = New System.Drawing.Size(67, 20)
        Me.LblTotalPlanSQFT.TabIndex = 23
        Me.LblTotalPlanSQFT.Text = "Plan SQFT:"
        '
        'TxtTotalPlanSQFT
        '
        Me.TxtTotalPlanSQFT.Location = New System.Drawing.Point(413, 166)
        Me.TxtTotalPlanSQFT.Name = "TxtTotalPlanSQFT"
        Me.TxtTotalPlanSQFT.ReadOnly = True
        Me.TxtTotalPlanSQFT.Size = New System.Drawing.Size(100, 20)
        Me.TxtTotalPlanSQFT.TabIndex = 22
        '
        'ListBoxAvailableUnits
        '
        Me.ListBoxAvailableUnits.FormattingEnabled = True
        Me.ListBoxAvailableUnits.Location = New System.Drawing.Point(10, 10)
        Me.ListBoxAvailableUnits.Name = "ListBoxAvailableUnits"
        Me.ListBoxAvailableUnits.Size = New System.Drawing.Size(140, 199)
        Me.ListBoxAvailableUnits.TabIndex = 1
        '
        'BtnConvertToActualUnit
        '
        Me.BtnConvertToActualUnit.Location = New System.Drawing.Point(10, 214)
        Me.BtnConvertToActualUnit.Name = "BtnConvertToActualUnit"
        Me.BtnConvertToActualUnit.Size = New System.Drawing.Size(140, 30)
        Me.BtnConvertToActualUnit.TabIndex = 2
        Me.BtnConvertToActualUnit.Text = "Convert to Actual Unit"
        '
        'DataGridViewAssigned
        '
        Me.DataGridViewAssigned.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridViewAssigned.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DataGridViewAssigned.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewAssigned.Location = New System.Drawing.Point(10, 273)
        Me.DataGridViewAssigned.Name = "DataGridViewAssigned"
        Me.DataGridViewAssigned.Size = New System.Drawing.Size(857, 287)
        Me.DataGridViewAssigned.TabIndex = 2
        '
        'tabRawUnit
        '
        Me.tabRawUnit.Controls.Add(Me.DataGridView1)
        Me.tabRawUnit.Location = New System.Drawing.Point(4, 22)
        Me.tabRawUnit.Name = "tabRawUnit"
        Me.tabRawUnit.Padding = New System.Windows.Forms.Padding(3)
        Me.tabRawUnit.Size = New System.Drawing.Size(879, 621)
        Me.tabRawUnit.TabIndex = 1
        Me.tabRawUnit.Text = "Raw Units Data"
        Me.tabRawUnit.UseVisualStyleBackColor = True
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Top
        Me.DataGridView1.Location = New System.Drawing.Point(3, 3)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.ReadOnly = True
        Me.DataGridView1.Size = New System.Drawing.Size(873, 518)
        Me.DataGridView1.TabIndex = 0
        '
        'PanelDetails
        '
        Me.PanelDetails.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PanelDetails.Controls.Add(Me.chkAttachToLevel)
        Me.PanelDetails.Controls.Add(Me.btnDeleteUnit)
        Me.PanelDetails.Controls.Add(Me.btnSave)
        Me.PanelDetails.Controls.Add(Me.LblActualUnitQuantity)
        Me.PanelDetails.Controls.Add(Me.TxtActualUnitQuantity)
        Me.PanelDetails.Controls.Add(Me.LblTotalSellPricePerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtTotalSellPricePerSQFT)
        Me.PanelDetails.Controls.Add(Me.LblDeliveryCostPerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtDeliveryCostPerSQFT)
        Me.PanelDetails.Controls.Add(Me.LblOverallCostPerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtOverallCostPerSQFT)
        Me.PanelDetails.Controls.Add(Me.LblItemCostPerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtItemCostPerSQFT)
        Me.PanelDetails.Controls.Add(Me.LblManHoursPerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtManHoursPerSQFT)
        Me.PanelDetails.Controls.Add(Me.LblJobSuppliesPerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtJobSuppliesPerSQFT)
        Me.PanelDetails.Controls.Add(Me.LblMGMTLaborPerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtMGMTLaborPerSQFT)
        Me.PanelDetails.Controls.Add(Me.LblDesignLaborPerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtDesignLaborPerSQFT)
        Me.PanelDetails.Controls.Add(Me.LblManufLaborPerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtManufLaborPerSQFT)
        Me.PanelDetails.Controls.Add(Me.LblBDFTPerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtBDFTPerSQFT)
        Me.PanelDetails.Controls.Add(Me.LblPlatePerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtPlatePerSQFT)
        Me.PanelDetails.Controls.Add(Me.LblLumberPerSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtLumberPerSQFT)
        Me.PanelDetails.Controls.Add(Me.BtnToggleDetails)
        Me.PanelDetails.Controls.Add(Me.LblUnitName)
        Me.PanelDetails.Controls.Add(Me.TxtUnitName)
        Me.PanelDetails.Controls.Add(Me.LblPlanSQFT)
        Me.PanelDetails.Controls.Add(Me.TxtPlanSQFT)
        Me.PanelDetails.Controls.Add(Me.LblOptionalAdder)
        Me.PanelDetails.Controls.Add(Me.TxtOptionalAdder)
        Me.PanelDetails.Controls.Add(Me.LblUnitType)
        Me.PanelDetails.Controls.Add(Me.CmbUnitType)
        Me.PanelDetails.Controls.Add(Me.BtnCancel)
        Me.PanelDetails.Controls.Add(Me.LblReferencedRawUnit)
        Me.PanelDetails.Controls.Add(Me.TxtReferencedRawUnit)
        Me.PanelDetails.Dock = System.Windows.Forms.DockStyle.Right
        Me.PanelDetails.Location = New System.Drawing.Point(1117, 0)
        Me.PanelDetails.Name = "PanelDetails"
        Me.PanelDetails.Size = New System.Drawing.Size(253, 647)
        Me.PanelDetails.TabIndex = 2
        Me.PanelDetails.Visible = False
        '
        'btnDeleteUnit
        '
        Me.btnDeleteUnit.Location = New System.Drawing.Point(22, 575)
        Me.btnDeleteUnit.Name = "btnDeleteUnit"
        Me.btnDeleteUnit.Size = New System.Drawing.Size(110, 31)
        Me.btnDeleteUnit.TabIndex = 48
        Me.btnDeleteUnit.Text = "Delete Unit"
        Me.btnDeleteUnit.UseVisualStyleBackColor = False
        '
        'btnSave
        '
        Me.btnSave.Location = New System.Drawing.Point(138, 575)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(110, 31)
        Me.btnSave.TabIndex = 47
        Me.btnSave.Text = "Save"
        Me.btnSave.UseVisualStyleBackColor = False
        '
        'LblActualUnitQuantity
        '
        Me.LblActualUnitQuantity.Location = New System.Drawing.Point(5, 171)
        Me.LblActualUnitQuantity.Name = "LblActualUnitQuantity"
        Me.LblActualUnitQuantity.Size = New System.Drawing.Size(109, 23)
        Me.LblActualUnitQuantity.TabIndex = 46
        Me.LblActualUnitQuantity.Text = "Unit Plan Qty:"
        '
        'TxtActualUnitQuantity
        '
        Me.TxtActualUnitQuantity.Location = New System.Drawing.Point(120, 171)
        Me.TxtActualUnitQuantity.Name = "TxtActualUnitQuantity"
        Me.TxtActualUnitQuantity.Size = New System.Drawing.Size(119, 20)
        Me.TxtActualUnitQuantity.TabIndex = 7
        '
        'LblTotalSellPricePerSQFT
        '
        Me.LblTotalSellPricePerSQFT.Location = New System.Drawing.Point(5, 523)
        Me.LblTotalSellPricePerSQFT.Name = "LblTotalSellPricePerSQFT"
        Me.LblTotalSellPricePerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblTotalSellPricePerSQFT.TabIndex = 22
        Me.LblTotalSellPricePerSQFT.Text = "Total Sell Price/SQFT:"
        '
        'TxtTotalSellPricePerSQFT
        '
        Me.TxtTotalSellPricePerSQFT.Location = New System.Drawing.Point(120, 520)
        Me.TxtTotalSellPricePerSQFT.Name = "TxtTotalSellPricePerSQFT"
        Me.TxtTotalSellPricePerSQFT.ReadOnly = True
        Me.TxtTotalSellPricePerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtTotalSellPricePerSQFT.TabIndex = 23
        '
        'LblDeliveryCostPerSQFT
        '
        Me.LblDeliveryCostPerSQFT.Location = New System.Drawing.Point(5, 497)
        Me.LblDeliveryCostPerSQFT.Name = "LblDeliveryCostPerSQFT"
        Me.LblDeliveryCostPerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblDeliveryCostPerSQFT.TabIndex = 24
        Me.LblDeliveryCostPerSQFT.Text = "Delivery Cost/SQFT:"
        '
        'TxtDeliveryCostPerSQFT
        '
        Me.TxtDeliveryCostPerSQFT.Location = New System.Drawing.Point(120, 494)
        Me.TxtDeliveryCostPerSQFT.Name = "TxtDeliveryCostPerSQFT"
        Me.TxtDeliveryCostPerSQFT.ReadOnly = True
        Me.TxtDeliveryCostPerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtDeliveryCostPerSQFT.TabIndex = 25
        '
        'LblOverallCostPerSQFT
        '
        Me.LblOverallCostPerSQFT.Location = New System.Drawing.Point(5, 471)
        Me.LblOverallCostPerSQFT.Name = "LblOverallCostPerSQFT"
        Me.LblOverallCostPerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblOverallCostPerSQFT.TabIndex = 26
        Me.LblOverallCostPerSQFT.Text = "Overall Cost/SQFT:"
        '
        'TxtOverallCostPerSQFT
        '
        Me.TxtOverallCostPerSQFT.Location = New System.Drawing.Point(120, 468)
        Me.TxtOverallCostPerSQFT.Name = "TxtOverallCostPerSQFT"
        Me.TxtOverallCostPerSQFT.ReadOnly = True
        Me.TxtOverallCostPerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtOverallCostPerSQFT.TabIndex = 27
        '
        'LblItemCostPerSQFT
        '
        Me.LblItemCostPerSQFT.Location = New System.Drawing.Point(5, 445)
        Me.LblItemCostPerSQFT.Name = "LblItemCostPerSQFT"
        Me.LblItemCostPerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblItemCostPerSQFT.TabIndex = 28
        Me.LblItemCostPerSQFT.Text = "Item Cost/SQFT:"
        '
        'TxtItemCostPerSQFT
        '
        Me.TxtItemCostPerSQFT.Location = New System.Drawing.Point(120, 442)
        Me.TxtItemCostPerSQFT.Name = "TxtItemCostPerSQFT"
        Me.TxtItemCostPerSQFT.ReadOnly = True
        Me.TxtItemCostPerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtItemCostPerSQFT.TabIndex = 29
        '
        'LblManHoursPerSQFT
        '
        Me.LblManHoursPerSQFT.Location = New System.Drawing.Point(5, 419)
        Me.LblManHoursPerSQFT.Name = "LblManHoursPerSQFT"
        Me.LblManHoursPerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblManHoursPerSQFT.TabIndex = 30
        Me.LblManHoursPerSQFT.Text = "Man Hours/SQFT:"
        '
        'TxtManHoursPerSQFT
        '
        Me.TxtManHoursPerSQFT.Location = New System.Drawing.Point(120, 416)
        Me.TxtManHoursPerSQFT.Name = "TxtManHoursPerSQFT"
        Me.TxtManHoursPerSQFT.ReadOnly = True
        Me.TxtManHoursPerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtManHoursPerSQFT.TabIndex = 31
        '
        'LblJobSuppliesPerSQFT
        '
        Me.LblJobSuppliesPerSQFT.Location = New System.Drawing.Point(5, 393)
        Me.LblJobSuppliesPerSQFT.Name = "LblJobSuppliesPerSQFT"
        Me.LblJobSuppliesPerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblJobSuppliesPerSQFT.TabIndex = 32
        Me.LblJobSuppliesPerSQFT.Text = "Job Supplies/SQFT:"
        '
        'TxtJobSuppliesPerSQFT
        '
        Me.TxtJobSuppliesPerSQFT.Location = New System.Drawing.Point(120, 390)
        Me.TxtJobSuppliesPerSQFT.Name = "TxtJobSuppliesPerSQFT"
        Me.TxtJobSuppliesPerSQFT.ReadOnly = True
        Me.TxtJobSuppliesPerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtJobSuppliesPerSQFT.TabIndex = 33
        '
        'LblMGMTLaborPerSQFT
        '
        Me.LblMGMTLaborPerSQFT.Location = New System.Drawing.Point(5, 367)
        Me.LblMGMTLaborPerSQFT.Name = "LblMGMTLaborPerSQFT"
        Me.LblMGMTLaborPerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblMGMTLaborPerSQFT.TabIndex = 34
        Me.LblMGMTLaborPerSQFT.Text = "MGMT Labor/SQFT:"
        '
        'TxtMGMTLaborPerSQFT
        '
        Me.TxtMGMTLaborPerSQFT.Location = New System.Drawing.Point(120, 364)
        Me.TxtMGMTLaborPerSQFT.Name = "TxtMGMTLaborPerSQFT"
        Me.TxtMGMTLaborPerSQFT.ReadOnly = True
        Me.TxtMGMTLaborPerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtMGMTLaborPerSQFT.TabIndex = 35
        '
        'LblDesignLaborPerSQFT
        '
        Me.LblDesignLaborPerSQFT.Location = New System.Drawing.Point(5, 341)
        Me.LblDesignLaborPerSQFT.Name = "LblDesignLaborPerSQFT"
        Me.LblDesignLaborPerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblDesignLaborPerSQFT.TabIndex = 36
        Me.LblDesignLaborPerSQFT.Text = "Design Labor/SQFT:"
        '
        'TxtDesignLaborPerSQFT
        '
        Me.TxtDesignLaborPerSQFT.Location = New System.Drawing.Point(120, 338)
        Me.TxtDesignLaborPerSQFT.Name = "TxtDesignLaborPerSQFT"
        Me.TxtDesignLaborPerSQFT.ReadOnly = True
        Me.TxtDesignLaborPerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtDesignLaborPerSQFT.TabIndex = 37
        '
        'LblManufLaborPerSQFT
        '
        Me.LblManufLaborPerSQFT.Location = New System.Drawing.Point(5, 315)
        Me.LblManufLaborPerSQFT.Name = "LblManufLaborPerSQFT"
        Me.LblManufLaborPerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblManufLaborPerSQFT.TabIndex = 38
        Me.LblManufLaborPerSQFT.Text = "Manuf Labor/SQFT:"
        '
        'TxtManufLaborPerSQFT
        '
        Me.TxtManufLaborPerSQFT.Location = New System.Drawing.Point(120, 312)
        Me.TxtManufLaborPerSQFT.Name = "TxtManufLaborPerSQFT"
        Me.TxtManufLaborPerSQFT.ReadOnly = True
        Me.TxtManufLaborPerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtManufLaborPerSQFT.TabIndex = 39
        '
        'LblBDFTPerSQFT
        '
        Me.LblBDFTPerSQFT.Location = New System.Drawing.Point(5, 289)
        Me.LblBDFTPerSQFT.Name = "LblBDFTPerSQFT"
        Me.LblBDFTPerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblBDFTPerSQFT.TabIndex = 40
        Me.LblBDFTPerSQFT.Text = "BDFT/SQFT:"
        '
        'TxtBDFTPerSQFT
        '
        Me.TxtBDFTPerSQFT.Location = New System.Drawing.Point(120, 286)
        Me.TxtBDFTPerSQFT.Name = "TxtBDFTPerSQFT"
        Me.TxtBDFTPerSQFT.ReadOnly = True
        Me.TxtBDFTPerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtBDFTPerSQFT.TabIndex = 41
        '
        'LblPlatePerSQFT
        '
        Me.LblPlatePerSQFT.Location = New System.Drawing.Point(5, 263)
        Me.LblPlatePerSQFT.Name = "LblPlatePerSQFT"
        Me.LblPlatePerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblPlatePerSQFT.TabIndex = 42
        Me.LblPlatePerSQFT.Text = "Plate$/SQFT:"
        '
        'TxtPlatePerSQFT
        '
        Me.TxtPlatePerSQFT.Location = New System.Drawing.Point(120, 260)
        Me.TxtPlatePerSQFT.Name = "TxtPlatePerSQFT"
        Me.TxtPlatePerSQFT.ReadOnly = True
        Me.TxtPlatePerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtPlatePerSQFT.TabIndex = 43
        '
        'LblLumberPerSQFT
        '
        Me.LblLumberPerSQFT.Location = New System.Drawing.Point(5, 237)
        Me.LblLumberPerSQFT.Name = "LblLumberPerSQFT"
        Me.LblLumberPerSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblLumberPerSQFT.TabIndex = 44
        Me.LblLumberPerSQFT.Text = "Lumber$/SQFT:"
        '
        'TxtLumberPerSQFT
        '
        Me.TxtLumberPerSQFT.Location = New System.Drawing.Point(120, 234)
        Me.TxtLumberPerSQFT.Name = "TxtLumberPerSQFT"
        Me.TxtLumberPerSQFT.ReadOnly = True
        Me.TxtLumberPerSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtLumberPerSQFT.TabIndex = 45
        '
        'BtnToggleDetails
        '
        Me.BtnToggleDetails.Location = New System.Drawing.Point(10, 14)
        Me.BtnToggleDetails.Name = "BtnToggleDetails"
        Me.BtnToggleDetails.Size = New System.Drawing.Size(100, 30)
        Me.BtnToggleDetails.TabIndex = 0
        Me.BtnToggleDetails.Text = "Hide Details"
        '
        'LblUnitName
        '
        Me.LblUnitName.Location = New System.Drawing.Point(5, 54)
        Me.LblUnitName.Name = "LblUnitName"
        Me.LblUnitName.Size = New System.Drawing.Size(109, 23)
        Me.LblUnitName.TabIndex = 1
        Me.LblUnitName.Text = "Actual Unit Name:"
        '
        'TxtUnitName
        '
        Me.TxtUnitName.Location = New System.Drawing.Point(120, 54)
        Me.TxtUnitName.Name = "TxtUnitName"
        Me.TxtUnitName.Size = New System.Drawing.Size(119, 20)
        Me.TxtUnitName.TabIndex = 3
        '
        'LblPlanSQFT
        '
        Me.LblPlanSQFT.Location = New System.Drawing.Point(5, 84)
        Me.LblPlanSQFT.Name = "LblPlanSQFT"
        Me.LblPlanSQFT.Size = New System.Drawing.Size(109, 23)
        Me.LblPlanSQFT.TabIndex = 3
        Me.LblPlanSQFT.Text = "Plan SQFT:"
        '
        'TxtPlanSQFT
        '
        Me.TxtPlanSQFT.Location = New System.Drawing.Point(120, 84)
        Me.TxtPlanSQFT.Name = "TxtPlanSQFT"
        Me.TxtPlanSQFT.Size = New System.Drawing.Size(119, 20)
        Me.TxtPlanSQFT.TabIndex = 4
        '
        'LblOptionalAdder
        '
        Me.LblOptionalAdder.Location = New System.Drawing.Point(5, 114)
        Me.LblOptionalAdder.Name = "LblOptionalAdder"
        Me.LblOptionalAdder.Size = New System.Drawing.Size(109, 23)
        Me.LblOptionalAdder.TabIndex = 5
        Me.LblOptionalAdder.Text = "Optional Adder:"
        '
        'TxtOptionalAdder
        '
        Me.TxtOptionalAdder.Location = New System.Drawing.Point(120, 114)
        Me.TxtOptionalAdder.Name = "TxtOptionalAdder"
        Me.TxtOptionalAdder.Size = New System.Drawing.Size(119, 20)
        Me.TxtOptionalAdder.TabIndex = 5
        '
        'LblUnitType
        '
        Me.LblUnitType.Location = New System.Drawing.Point(5, 144)
        Me.LblUnitType.Name = "LblUnitType"
        Me.LblUnitType.Size = New System.Drawing.Size(109, 23)
        Me.LblUnitType.TabIndex = 7
        Me.LblUnitType.Text = "Unit Type:"
        '
        'CmbUnitType
        '
        Me.CmbUnitType.Items.AddRange(New Object() {"Res", "NonRes"})
        Me.CmbUnitType.Location = New System.Drawing.Point(120, 144)
        Me.CmbUnitType.Name = "CmbUnitType"
        Me.CmbUnitType.Size = New System.Drawing.Size(119, 21)
        Me.CmbUnitType.TabIndex = 6
        '
        'BtnCancel
        '
        Me.BtnCancel.Location = New System.Drawing.Point(138, 611)
        Me.BtnCancel.Name = "BtnCancel"
        Me.BtnCancel.Size = New System.Drawing.Size(110, 31)
        Me.BtnCancel.TabIndex = 13
        Me.BtnCancel.Text = "Cancel"
        '
        'LblReferencedRawUnit
        '
        Me.LblReferencedRawUnit.Location = New System.Drawing.Point(5, 211)
        Me.LblReferencedRawUnit.Name = "LblReferencedRawUnit"
        Me.LblReferencedRawUnit.Size = New System.Drawing.Size(109, 23)
        Me.LblReferencedRawUnit.TabIndex = 14
        Me.LblReferencedRawUnit.Text = "Referenced Raw Unit:"
        '
        'TxtReferencedRawUnit
        '
        Me.TxtReferencedRawUnit.Location = New System.Drawing.Point(120, 208)
        Me.TxtReferencedRawUnit.Name = "TxtReferencedRawUnit"
        Me.TxtReferencedRawUnit.ReadOnly = True
        Me.TxtReferencedRawUnit.Size = New System.Drawing.Size(119, 20)
        Me.TxtReferencedRawUnit.TabIndex = 15
        '
        'chkAttachToLevel
        '
        Me.chkAttachToLevel.AutoSize = True
        Me.chkAttachToLevel.Location = New System.Drawing.Point(98, 552)
        Me.chkAttachToLevel.Name = "chkAttachToLevel"
        Me.chkAttachToLevel.Size = New System.Drawing.Size(141, 17)
        Me.chkAttachToLevel.TabIndex = 49
        Me.chkAttachToLevel.Text = "Attach to Current Level?"
        Me.chkAttachToLevel.UseVisualStyleBackColor = True
        '
        'FrmPSE
        '
        Me.BackColor = System.Drawing.Color.LightGray
        Me.ClientSize = New System.Drawing.Size(1370, 647)
        Me.Controls.Add(Me.TabControlData)
        Me.Controls.Add(Me.TreeViewLevels)
        Me.Controls.Add(Me.PanelDetails)
        Me.Name = "FrmPSE"
        Me.Text = "Builders PSE Management"
        Me.ContextMenuLevels.ResumeLayout(False)
        Me.TabControlData.ResumeLayout(False)
        Me.TabUnitBased.ResumeLayout(False)
        Me.TabUnitBased.PerformLayout()
        Me.ContextMenuActualUnits.ResumeLayout(False)
        CType(Me.DataGridViewAssigned, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tabRawUnit.ResumeLayout(False)
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.PanelDetails.ResumeLayout(False)
        Me.PanelDetails.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Private WithEvents LblActualUnitQuantity As Label
    Private WithEvents TxtActualUnitQuantity As TextBox
    Private WithEvents TreeViewLevels As TreeView
    Private WithEvents TabControlData As TabControl
    Private WithEvents TabUnitBased As TabPage
    Private WithEvents ListBoxAvailableUnits As ListBox
    Private WithEvents BtnConvertToActualUnit As Button
    Private WithEvents DataGridViewAssigned As DataGridView
    Private WithEvents PanelDetails As Panel
    Private WithEvents BtnToggleDetails As Button
    Private WithEvents LblUnitName As Label
    Private WithEvents TxtUnitName As TextBox
    Private WithEvents LblPlanSQFT As Label
    Private WithEvents TxtPlanSQFT As TextBox
    Private WithEvents LblOptionalAdder As Label
    Private WithEvents TxtOptionalAdder As TextBox
    Private WithEvents LblUnitType As Label
    Private WithEvents CmbUnitType As ComboBox
    Private WithEvents BtnCancel As Button
    Private WithEvents BtnRecalculate As Button
    Private WithEvents BtnFinish As Button
    Private WithEvents LblReferencedRawUnit As Label
    Private WithEvents TxtReferencedRawUnit As TextBox
    Private WithEvents LblLumberPerSQFT As Label
    Private WithEvents TxtLumberPerSQFT As TextBox
    Private WithEvents LblPlatePerSQFT As Label
    Private WithEvents TxtPlatePerSQFT As TextBox
    Private WithEvents LblBDFTPerSQFT As Label
    Private WithEvents TxtBDFTPerSQFT As TextBox
    Private WithEvents LblManufLaborPerSQFT As Label
    Private WithEvents TxtManufLaborPerSQFT As TextBox
    Private WithEvents LblDesignLaborPerSQFT As Label
    Private WithEvents TxtDesignLaborPerSQFT As TextBox
    Private WithEvents LblMGMTLaborPerSQFT As Label
    Private WithEvents TxtMGMTLaborPerSQFT As TextBox
    Private WithEvents LblJobSuppliesPerSQFT As Label
    Private WithEvents TxtJobSuppliesPerSQFT As TextBox
    Private WithEvents LblManHoursPerSQFT As Label
    Private WithEvents TxtManHoursPerSQFT As TextBox
    Private WithEvents LblItemCostPerSQFT As Label
    Private WithEvents TxtItemCostPerSQFT As TextBox
    Private WithEvents LblOverallCostPerSQFT As Label
    Private WithEvents TxtOverallCostPerSQFT As TextBox
    Private WithEvents LblDeliveryCostPerSQFT As Label
    Private WithEvents TxtDeliveryCostPerSQFT As TextBox
    Private WithEvents LblTotalSellPricePerSQFT As Label
    Private WithEvents TxtTotalSellPricePerSQFT As TextBox
    Private WithEvents LblTotalPlanSQFT As Label
    Private WithEvents TxtTotalPlanSQFT As TextBox
    Private WithEvents LblTotalQuantity As Label
    Private WithEvents TxtTotalQuantity As TextBox
    Private WithEvents LblTotalLF As Label
    Private WithEvents TxtTotalLF As TextBox
    Private WithEvents LblTotalBDFT As Label
    Private WithEvents TxtTotalBDFT As TextBox
    Private WithEvents LblTotalLumberCost As Label
    Private WithEvents TxtTotalLumberCost As TextBox
    Private WithEvents LblTotalPlateCost As Label
    Private WithEvents TxtTotalPlateCost As TextBox
    Private WithEvents LblTotalManufLaborCost As Label
    Private WithEvents TxtTotalManufLaborCost As TextBox
    Private WithEvents LblTotalDesignLabor As Label
    Private WithEvents TxtTotalDesignLabor As TextBox
    Private WithEvents LblTotalMGMTLabor As Label
    Private WithEvents TxtTotalMGMTLabor As TextBox
    Private WithEvents LblTotalJobSuppliesCost As Label
    Private WithEvents TxtTotalJobSuppliesCost As TextBox
    Private WithEvents LblTotalManHours As Label
    Private WithEvents TxtTotalManHours As TextBox
    Private WithEvents LblTotalItemCost As Label
    Private WithEvents TxtTotalItemCost As TextBox
    Private WithEvents LblTotalOverallCost As Label
    Private WithEvents TxtTotalOverallCost As TextBox
    Private WithEvents LblTotalSellPrice As Label
    Private WithEvents TxtTotalSellPrice As TextBox
    Private WithEvents LblTotalMargin As Label
    Private WithEvents TxtTotalMargin As TextBox
    Private WithEvents BtnImportLevelData As Button
    Friend WithEvents ListboxExistingActualUnits As ListBox
    Friend WithEvents btnReuseActualUnit As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents btnDeleteUnit As Button
    Friend WithEvents ChkDetailedView As CheckBox
    Private WithEvents Label1 As Label
    Private WithEvents txtTotalDeliveryCost As TextBox
    Friend WithEvents ContextMenuLevels As ContextMenuStrip
    Friend WithEvents mnuCopyUnits As ToolStripMenuItem
    Friend WithEvents mnuPasteUnits As ToolStripMenuItem
    Friend WithEvents ContextMenuActualUnits As ContextMenuStrip
    Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents DeleteActualUnitToolStripMenuItem As ToolStripMenuItem
    Private WithEvents Label2 As Label
    Private WithEvents txtTotMargin As TextBox
    Friend WithEvents tabRawUnit As TabPage
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents chkAttachToLevel As CheckBox
End Class