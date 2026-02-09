<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmComponentPricingEdit
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
        Me.lblMarginSource = New System.Windows.Forms.Label()
        Me.cboMarginSource = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cboPlan = New System.Windows.Forms.ComboBox()
        Me.btnAddPlan = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cboElevation = New System.Windows.Forms.ComboBox()
        Me.btnAddElevation = New System.Windows.Forms.Button()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.cboOption = New System.Windows.Forms.ComboBox()
        Me.btnAddOption = New System.Windows.Forms.Button()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cboProductType = New System.Windows.Forms.ComboBox()
        Me.chkIsAdder = New System.Windows.Forms.CheckBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.nudMgmtSellPrice = New System.Windows.Forms.NumericUpDown()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.nudCost = New System.Windows.Forms.NumericUpDown()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.nudAppliedMargin = New System.Windows.Forms.NumericUpDown()
        Me.btnRecalculate = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.lblCalculatedPrice = New System.Windows.Forms.Label()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.nudFinalPrice = New System.Windows.Forms.NumericUpDown()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.nudPriceSentToBuilder = New System.Windows.Forms.NumericUpDown()
        Me.lblPriceDiffWarning = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtPriceNote = New System.Windows.Forms.TextBox()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.pnlAdderSettings = New System.Windows.Forms.Panel()
        Me.lblReferenceUnit = New System.Windows.Forms.Label()
        Me.cboReferenceUnit = New System.Windows.Forms.ComboBox()
        Me.lblAdderCostLabel = New System.Windows.Forms.Label()
        Me.lblAdderCost = New System.Windows.Forms.Label()
        Me.grpPreviousPricing = New System.Windows.Forms.GroupBox()
        Me.lblPrevSalesCaption = New System.Windows.Forms.Label()
        Me.lblPrevSalesValue = New System.Windows.Forms.Label()
        Me.lblPctChgSalesCaption = New System.Windows.Forms.Label()
        Me.lblPctChgSalesValue = New System.Windows.Forms.Label()
        Me.lblPrevBuilderCaption = New System.Windows.Forms.Label()
        Me.lblPrevBuilderValue = New System.Windows.Forms.Label()
        Me.lblPctChgBuilderCaption = New System.Windows.Forms.Label()
        Me.lblPctChgBuilderValue = New System.Windows.Forms.Label()
        Me.btnUnlockKeyFields = New System.Windows.Forms.Button()
        Me.lblUnlockWarning = New System.Windows.Forms.Label()
        CType(Me.nudMgmtSellPrice, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudCost, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudAppliedMargin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudFinalPrice, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudPriceSentToBuilder, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlAdderSettings.SuspendLayout()
        Me.grpPreviousPricing.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblMarginSource
        '
        Me.lblMarginSource.AutoSize = True
        Me.lblMarginSource.Location = New System.Drawing.Point(20, 262)
        Me.lblMarginSource.Name = "lblMarginSource"
        Me.lblMarginSource.Size = New System.Drawing.Size(79, 13)
        Me.lblMarginSource.TabIndex = 10
        Me.lblMarginSource.Text = "Margin Source:"
        '
        'cboMarginSource
        '
        Me.cboMarginSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboMarginSource.FormattingEnabled = True
        Me.cboMarginSource.Location = New System.Drawing.Point(150, 259)
        Me.cboMarginSource.Name = "cboMarginSource"
        Me.cboMarginSource.Size = New System.Drawing.Size(150, 21)
        Me.cboMarginSource.TabIndex = 11
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(20, 34)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(31, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Plan:"
        '
        'cboPlan
        '
        Me.cboPlan.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboPlan.FormattingEnabled = True
        Me.cboPlan.Location = New System.Drawing.Point(120, 31)
        Me.cboPlan.Name = "cboPlan"
        Me.cboPlan.Size = New System.Drawing.Size(260, 21)
        Me.cboPlan.TabIndex = 1
        '
        'btnAddPlan
        '
        Me.btnAddPlan.Location = New System.Drawing.Point(385, 30)
        Me.btnAddPlan.Name = "btnAddPlan"
        Me.btnAddPlan.Size = New System.Drawing.Size(50, 23)
        Me.btnAddPlan.TabIndex = 2
        Me.btnAddPlan.Text = "Add..."
        Me.btnAddPlan.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(20, 64)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(54, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "Elevation:"
        '
        'cboElevation
        '
        Me.cboElevation.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboElevation.FormattingEnabled = True
        Me.cboElevation.Location = New System.Drawing.Point(120, 61)
        Me.cboElevation.Name = "cboElevation"
        Me.cboElevation.Size = New System.Drawing.Size(260, 21)
        Me.cboElevation.TabIndex = 4
        '
        'btnAddElevation
        '
        Me.btnAddElevation.Location = New System.Drawing.Point(385, 60)
        Me.btnAddElevation.Name = "btnAddElevation"
        Me.btnAddElevation.Size = New System.Drawing.Size(50, 23)
        Me.btnAddElevation.TabIndex = 5
        Me.btnAddElevation.Text = "Add..."
        Me.btnAddElevation.UseVisualStyleBackColor = True
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(20, 94)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(41, 13)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "Option:"
        '
        'cboOption
        '
        Me.cboOption.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboOption.FormattingEnabled = True
        Me.cboOption.Location = New System.Drawing.Point(120, 91)
        Me.cboOption.Name = "cboOption"
        Me.cboOption.Size = New System.Drawing.Size(260, 21)
        Me.cboOption.TabIndex = 7
        '
        'btnAddOption
        '
        Me.btnAddOption.Location = New System.Drawing.Point(385, 90)
        Me.btnAddOption.Name = "btnAddOption"
        Me.btnAddOption.Size = New System.Drawing.Size(50, 23)
        Me.btnAddOption.TabIndex = 8
        Me.btnAddOption.Text = "Add..."
        Me.btnAddOption.UseVisualStyleBackColor = True
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(20, 124)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(74, 13)
        Me.Label4.TabIndex = 9
        Me.Label4.Text = "Product Type:"
        '
        'cboProductType
        '
        Me.cboProductType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboProductType.FormattingEnabled = True
        Me.cboProductType.Location = New System.Drawing.Point(120, 121)
        Me.cboProductType.Name = "cboProductType"
        Me.cboProductType.Size = New System.Drawing.Size(150, 21)
        Me.cboProductType.TabIndex = 10
        '
        'chkIsAdder
        '
        Me.chkIsAdder.AutoSize = True
        Me.chkIsAdder.Location = New System.Drawing.Point(120, 151)
        Me.chkIsAdder.Name = "chkIsAdder"
        Me.chkIsAdder.Size = New System.Drawing.Size(178, 17)
        Me.chkIsAdder.TabIndex = 11
        Me.chkIsAdder.Text = "This is an ADDER (not full price)"
        Me.chkIsAdder.UseVisualStyleBackColor = True
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.ForeColor = System.Drawing.Color.Gray
        Me.Label5.Location = New System.Drawing.Point(20, 242)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(121, 13)
        Me.Label5.TabIndex = 12
        Me.Label5.Text = "─── Source Data ───"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(20, 289)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(90, 13)
        Me.Label6.TabIndex = 13
        Me.Label6.Text = "MGMT Sell Price:"
        '
        'nudMgmtSellPrice
        '
        Me.nudMgmtSellPrice.DecimalPlaces = 2
        Me.nudMgmtSellPrice.Location = New System.Drawing.Point(150, 287)
        Me.nudMgmtSellPrice.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.nudMgmtSellPrice.Name = "nudMgmtSellPrice"
        Me.nudMgmtSellPrice.Size = New System.Drawing.Size(100, 20)
        Me.nudMgmtSellPrice.TabIndex = 14
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(20, 315)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(31, 13)
        Me.Label7.TabIndex = 15
        Me.Label7.Text = "Cost:"
        '
        'nudCost
        '
        Me.nudCost.DecimalPlaces = 2
        Me.nudCost.Location = New System.Drawing.Point(150, 313)
        Me.nudCost.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.nudCost.Name = "nudCost"
        Me.nudCost.Size = New System.Drawing.Size(100, 20)
        Me.nudCost.TabIndex = 16
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.ForeColor = System.Drawing.Color.Gray
        Me.Label8.Location = New System.Drawing.Point(20, 336)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(146, 13)
        Me.Label8.TabIndex = 17
        Me.Label8.Text = "─── Calculated Pricing ───"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(20, 364)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(80, 13)
        Me.Label9.TabIndex = 18
        Me.Label9.Text = "Applied Margin:"
        '
        'nudAppliedMargin
        '
        Me.nudAppliedMargin.DecimalPlaces = 2
        Me.nudAppliedMargin.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.nudAppliedMargin.Location = New System.Drawing.Point(150, 361)
        Me.nudAppliedMargin.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudAppliedMargin.Name = "nudAppliedMargin"
        Me.nudAppliedMargin.Size = New System.Drawing.Size(80, 20)
        Me.nudAppliedMargin.TabIndex = 19
        '
        'btnRecalculate
        '
        Me.btnRecalculate.Location = New System.Drawing.Point(240, 359)
        Me.btnRecalculate.Name = "btnRecalculate"
        Me.btnRecalculate.Size = New System.Drawing.Size(85, 23)
        Me.btnRecalculate.TabIndex = 20
        Me.btnRecalculate.Text = "Recalculate"
        Me.btnRecalculate.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(20, 394)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(87, 13)
        Me.Label10.TabIndex = 21
        Me.Label10.Text = "Calculated Price:"
        '
        'lblCalculatedPrice
        '
        Me.lblCalculatedPrice.AutoSize = True
        Me.lblCalculatedPrice.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCalculatedPrice.Location = New System.Drawing.Point(150, 394)
        Me.lblCalculatedPrice.Name = "lblCalculatedPrice"
        Me.lblCalculatedPrice.Size = New System.Drawing.Size(0, 13)
        Me.lblCalculatedPrice.TabIndex = 22
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.ForeColor = System.Drawing.Color.Gray
        Me.Label11.Location = New System.Drawing.Point(20, 421)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(130, 13)
        Me.Label11.TabIndex = 23
        Me.Label11.Text = "─── Price Tracking ───"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(20, 449)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(59, 13)
        Me.Label12.TabIndex = 24
        Me.Label12.Text = "Final Price:"
        '
        'nudFinalPrice
        '
        Me.nudFinalPrice.DecimalPlaces = 2
        Me.nudFinalPrice.Location = New System.Drawing.Point(150, 446)
        Me.nudFinalPrice.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.nudFinalPrice.Minimum = New Decimal(New Integer() {999999, 0, 0, -2147483648})
        Me.nudFinalPrice.Name = "nudFinalPrice"
        Me.nudFinalPrice.Size = New System.Drawing.Size(100, 20)
        Me.nudFinalPrice.TabIndex = 25
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.ForeColor = System.Drawing.Color.Gray
        Me.Label13.Location = New System.Drawing.Point(260, 449)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(85, 13)
        Me.Label13.TabIndex = 26
        Me.Label13.Text = "(= Sent to Sales)"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(20, 479)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(81, 13)
        Me.Label14.TabIndex = 27
        Me.Label14.Text = "Price to Builder:"
        '
        'nudPriceSentToBuilder
        '
        Me.nudPriceSentToBuilder.DecimalPlaces = 2
        Me.nudPriceSentToBuilder.Location = New System.Drawing.Point(150, 476)
        Me.nudPriceSentToBuilder.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.nudPriceSentToBuilder.Minimum = New Decimal(New Integer() {999999, 0, 0, -2147483648})
        Me.nudPriceSentToBuilder.Name = "nudPriceSentToBuilder"
        Me.nudPriceSentToBuilder.Size = New System.Drawing.Size(100, 20)
        Me.nudPriceSentToBuilder.TabIndex = 28
        '
        'lblPriceDiffWarning
        '
        Me.lblPriceDiffWarning.AutoSize = True
        Me.lblPriceDiffWarning.ForeColor = System.Drawing.Color.DarkOrange
        Me.lblPriceDiffWarning.Location = New System.Drawing.Point(150, 502)
        Me.lblPriceDiffWarning.Name = "lblPriceDiffWarning"
        Me.lblPriceDiffWarning.Size = New System.Drawing.Size(185, 13)
        Me.lblPriceDiffWarning.TabIndex = 29
        Me.lblPriceDiffWarning.Text = "⚠ Builder price differs from Final price"
        Me.lblPriceDiffWarning.Visible = False
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(20, 530)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(33, 13)
        Me.Label15.TabIndex = 30
        Me.Label15.Text = "Note:"
        '
        'txtPriceNote
        '
        Me.txtPriceNote.Location = New System.Drawing.Point(150, 530)
        Me.txtPriceNote.Multiline = True
        Me.txtPriceNote.Name = "txtPriceNote"
        Me.txtPriceNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtPriceNote.Size = New System.Drawing.Size(300, 60)
        Me.txtPriceNote.TabIndex = 31
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(300, 692)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(80, 30)
        Me.btnOK.TabIndex = 32
        Me.btnOK.Text = "Save"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(390, 692)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(80, 30)
        Me.btnCancel.TabIndex = 33
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'pnlAdderSettings
        '
        Me.pnlAdderSettings.Controls.Add(Me.lblReferenceUnit)
        Me.pnlAdderSettings.Controls.Add(Me.cboReferenceUnit)
        Me.pnlAdderSettings.Controls.Add(Me.lblAdderCostLabel)
        Me.pnlAdderSettings.Controls.Add(Me.lblAdderCost)
        Me.pnlAdderSettings.Location = New System.Drawing.Point(23, 174)
        Me.pnlAdderSettings.Name = "pnlAdderSettings"
        Me.pnlAdderSettings.Size = New System.Drawing.Size(460, 55)
        Me.pnlAdderSettings.TabIndex = 12
        Me.pnlAdderSettings.Visible = False
        '
        'lblReferenceUnit
        '
        Me.lblReferenceUnit.AutoSize = True
        Me.lblReferenceUnit.Location = New System.Drawing.Point(3, 5)
        Me.lblReferenceUnit.Name = "lblReferenceUnit"
        Me.lblReferenceUnit.Size = New System.Drawing.Size(82, 13)
        Me.lblReferenceUnit.TabIndex = 0
        Me.lblReferenceUnit.Text = "Reference Unit:"
        '
        'cboReferenceUnit
        '
        Me.cboReferenceUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboReferenceUnit.FormattingEnabled = True
        Me.cboReferenceUnit.Location = New System.Drawing.Point(130, 2)
        Me.cboReferenceUnit.Name = "cboReferenceUnit"
        Me.cboReferenceUnit.Size = New System.Drawing.Size(320, 21)
        Me.cboReferenceUnit.TabIndex = 1
        '
        'lblAdderCostLabel
        '
        Me.lblAdderCostLabel.AutoSize = True
        Me.lblAdderCostLabel.Location = New System.Drawing.Point(3, 32)
        Me.lblAdderCostLabel.Name = "lblAdderCostLabel"
        Me.lblAdderCostLabel.Size = New System.Drawing.Size(120, 13)
        Me.lblAdderCostLabel.TabIndex = 2
        Me.lblAdderCostLabel.Text = "Adder Cost (Difference):"
        '
        'lblAdderCost
        '
        Me.lblAdderCost.AutoSize = True
        Me.lblAdderCost.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblAdderCost.ForeColor = System.Drawing.Color.DarkGreen
        Me.lblAdderCost.Location = New System.Drawing.Point(130, 32)
        Me.lblAdderCost.Name = "lblAdderCost"
        Me.lblAdderCost.Size = New System.Drawing.Size(0, 13)
        Me.lblAdderCost.TabIndex = 3
        '
        'grpPreviousPricing
        '
        Me.grpPreviousPricing.Controls.Add(Me.lblPrevSalesCaption)
        Me.grpPreviousPricing.Controls.Add(Me.lblPrevSalesValue)
        Me.grpPreviousPricing.Controls.Add(Me.lblPctChgSalesCaption)
        Me.grpPreviousPricing.Controls.Add(Me.lblPctChgSalesValue)
        Me.grpPreviousPricing.Controls.Add(Me.lblPrevBuilderCaption)
        Me.grpPreviousPricing.Controls.Add(Me.lblPrevBuilderValue)
        Me.grpPreviousPricing.Controls.Add(Me.lblPctChgBuilderCaption)
        Me.grpPreviousPricing.Controls.Add(Me.lblPctChgBuilderValue)
        Me.grpPreviousPricing.Location = New System.Drawing.Point(12, 596)
        Me.grpPreviousPricing.Name = "grpPreviousPricing"
        Me.grpPreviousPricing.Size = New System.Drawing.Size(480, 85)
        Me.grpPreviousPricing.TabIndex = 34
        Me.grpPreviousPricing.TabStop = False
        Me.grpPreviousPricing.Text = "Previous Price Lock Comparison"
        '
        'lblPrevSalesCaption
        '
        Me.lblPrevSalesCaption.Location = New System.Drawing.Point(10, 22)
        Me.lblPrevSalesCaption.Name = "lblPrevSalesCaption"
        Me.lblPrevSalesCaption.Size = New System.Drawing.Size(110, 20)
        Me.lblPrevSalesCaption.TabIndex = 0
        Me.lblPrevSalesCaption.Text = "Prev Sent to Sales:"
        Me.lblPrevSalesCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPrevSalesValue
        '
        Me.lblPrevSalesValue.BackColor = System.Drawing.Color.WhiteSmoke
        Me.lblPrevSalesValue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblPrevSalesValue.Location = New System.Drawing.Point(125, 22)
        Me.lblPrevSalesValue.Name = "lblPrevSalesValue"
        Me.lblPrevSalesValue.Size = New System.Drawing.Size(90, 20)
        Me.lblPrevSalesValue.TabIndex = 1
        Me.lblPrevSalesValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPctChgSalesCaption
        '
        Me.lblPctChgSalesCaption.Location = New System.Drawing.Point(220, 22)
        Me.lblPctChgSalesCaption.Name = "lblPctChgSalesCaption"
        Me.lblPctChgSalesCaption.Size = New System.Drawing.Size(70, 20)
        Me.lblPctChgSalesCaption.TabIndex = 2
        Me.lblPctChgSalesCaption.Text = "% Change:"
        Me.lblPctChgSalesCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPctChgSalesValue
        '
        Me.lblPctChgSalesValue.BackColor = System.Drawing.Color.WhiteSmoke
        Me.lblPctChgSalesValue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblPctChgSalesValue.Location = New System.Drawing.Point(295, 22)
        Me.lblPctChgSalesValue.Name = "lblPctChgSalesValue"
        Me.lblPctChgSalesValue.Size = New System.Drawing.Size(70, 20)
        Me.lblPctChgSalesValue.TabIndex = 3
        Me.lblPctChgSalesValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'lblPrevBuilderCaption
        '
        Me.lblPrevBuilderCaption.Location = New System.Drawing.Point(10, 50)
        Me.lblPrevBuilderCaption.Name = "lblPrevBuilderCaption"
        Me.lblPrevBuilderCaption.Size = New System.Drawing.Size(110, 20)
        Me.lblPrevBuilderCaption.TabIndex = 4
        Me.lblPrevBuilderCaption.Text = "Prev Sent to Builder:"
        Me.lblPrevBuilderCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPrevBuilderValue
        '
        Me.lblPrevBuilderValue.BackColor = System.Drawing.Color.WhiteSmoke
        Me.lblPrevBuilderValue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblPrevBuilderValue.Location = New System.Drawing.Point(125, 50)
        Me.lblPrevBuilderValue.Name = "lblPrevBuilderValue"
        Me.lblPrevBuilderValue.Size = New System.Drawing.Size(90, 20)
        Me.lblPrevBuilderValue.TabIndex = 5
        Me.lblPrevBuilderValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPctChgBuilderCaption
        '
        Me.lblPctChgBuilderCaption.Location = New System.Drawing.Point(220, 50)
        Me.lblPctChgBuilderCaption.Name = "lblPctChgBuilderCaption"
        Me.lblPctChgBuilderCaption.Size = New System.Drawing.Size(70, 20)
        Me.lblPctChgBuilderCaption.TabIndex = 6
        Me.lblPctChgBuilderCaption.Text = "% Change:"
        Me.lblPctChgBuilderCaption.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'lblPctChgBuilderValue
        '
        Me.lblPctChgBuilderValue.BackColor = System.Drawing.Color.WhiteSmoke
        Me.lblPctChgBuilderValue.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.lblPctChgBuilderValue.Location = New System.Drawing.Point(295, 50)
        Me.lblPctChgBuilderValue.Name = "lblPctChgBuilderValue"
        Me.lblPctChgBuilderValue.Size = New System.Drawing.Size(70, 20)
        Me.lblPctChgBuilderValue.TabIndex = 7
        Me.lblPctChgBuilderValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'btnUnlockKeyFields
        '
        Me.btnUnlockKeyFields.Location = New System.Drawing.Point(307, 119)
        Me.btnUnlockKeyFields.Name = "btnUnlockKeyFields"
        Me.btnUnlockKeyFields.Size = New System.Drawing.Size(180, 25)
        Me.btnUnlockKeyFields.TabIndex = 35
        Me.btnUnlockKeyFields.Text = "🔓 Unlock Plan/Elev/Option"
        Me.btnUnlockKeyFields.UseVisualStyleBackColor = True
        Me.btnUnlockKeyFields.Visible = False
        '
        'lblUnlockWarning
        '
        Me.lblUnlockWarning.AutoSize = True
        Me.lblUnlockWarning.ForeColor = System.Drawing.Color.DarkOrange
        Me.lblUnlockWarning.Location = New System.Drawing.Point(117, 9)
        Me.lblUnlockWarning.Name = "lblUnlockWarning"
        Me.lblUnlockWarning.Size = New System.Drawing.Size(0, 13)
        Me.lblUnlockWarning.TabIndex = 36
        Me.lblUnlockWarning.Visible = False
        '
        'frmComponentPricingEdit
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(504, 728)
        Me.Controls.Add(Me.grpPreviousPricing)
        Me.Controls.Add(Me.pnlAdderSettings)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.txtPriceNote)
        Me.Controls.Add(Me.Label15)
        Me.Controls.Add(Me.lblPriceDiffWarning)
        Me.Controls.Add(Me.nudPriceSentToBuilder)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.nudFinalPrice)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.lblCalculatedPrice)
        Me.Controls.Add(Me.Label10)
        Me.Controls.Add(Me.btnRecalculate)
        Me.Controls.Add(Me.nudAppliedMargin)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.nudCost)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.nudMgmtSellPrice)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.chkIsAdder)
        Me.Controls.Add(Me.cboProductType)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.btnAddOption)
        Me.Controls.Add(Me.cboOption)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.btnAddElevation)
        Me.Controls.Add(Me.cboElevation)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnAddPlan)
        Me.Controls.Add(Me.cboPlan)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.cboMarginSource)
        Me.Controls.Add(Me.lblMarginSource)
        Me.Controls.Add(Me.btnUnlockKeyFields)
        Me.Controls.Add(Me.lblUnlockWarning)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmComponentPricingEdit"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Component Pricing"
        CType(Me.nudMgmtSellPrice, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudCost, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudAppliedMargin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudFinalPrice, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudPriceSentToBuilder, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlAdderSettings.ResumeLayout(False)
        Me.pnlAdderSettings.PerformLayout()
        Me.grpPreviousPricing.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents cboPlan As ComboBox
    Friend WithEvents btnAddPlan As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents cboElevation As ComboBox
    Friend WithEvents btnAddElevation As Button
    Friend WithEvents Label3 As Label
    Friend WithEvents cboOption As ComboBox
    Friend WithEvents btnAddOption As Button
    Friend WithEvents Label4 As Label
    Friend WithEvents cboProductType As ComboBox
    Friend WithEvents chkIsAdder As CheckBox
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents nudMgmtSellPrice As NumericUpDown
    Friend WithEvents Label7 As Label
    Friend WithEvents nudCost As NumericUpDown
    Friend WithEvents Label8 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents nudAppliedMargin As NumericUpDown
    Friend WithEvents btnRecalculate As Button
    Friend WithEvents Label10 As Label
    Friend WithEvents lblCalculatedPrice As Label
    Friend WithEvents Label11 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents nudFinalPrice As NumericUpDown
    Friend WithEvents Label13 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents nudPriceSentToBuilder As NumericUpDown
    Friend WithEvents lblPriceDiffWarning As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents txtPriceNote As TextBox
    Friend WithEvents btnOK As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents cboMarginSource As ComboBox
    Friend WithEvents lblMarginSource As Label

    ' Adder settings controls
    Friend WithEvents lblReferenceUnit As Label
    Friend WithEvents cboReferenceUnit As ComboBox
    Friend WithEvents lblAdderCostLabel As Label
    Friend WithEvents lblAdderCost As Label
    Friend WithEvents pnlAdderSettings As Panel

    ' Previous Price Lock Comparison controls
    Friend WithEvents grpPreviousPricing As GroupBox
    Friend WithEvents lblPrevSalesCaption As Label
    Friend WithEvents lblPrevSalesValue As Label
    Friend WithEvents lblPctChgSalesCaption As Label
    Friend WithEvents lblPctChgSalesValue As Label
    Friend WithEvents lblPrevBuilderCaption As Label
    Friend WithEvents lblPrevBuilderValue As Label
    Friend WithEvents lblPctChgBuilderCaption As Label
    Friend WithEvents lblPctChgBuilderValue As Label

    Friend WithEvents btnUnlockKeyFields As Button
    Friend WithEvents lblUnlockWarning As Label

End Class