<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmRandomLengthsImport
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
        Me.pnlTop = New System.Windows.Forms.Panel()
        Me.lblInstructions = New System.Windows.Forms.Label()
        Me.btnApplyToAll = New System.Windows.Forms.Button()
        Me.btnCalculate = New System.Windows.Forms.Button()
        Me.nudDefaultMargin = New System.Windows.Forms.NumericUpDown()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.btnLoadCurrent = New System.Windows.Forms.Button()
        Me.dtpReportDate = New System.Windows.Forms.DateTimePicker()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.dgvPricing = New System.Windows.Forms.DataGridView()
        Me.MaterialPricingID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MaterialCategoryID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CategoryName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.RLDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.RLPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PreviousPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CalculatedPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.FinalPrice = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PctChange = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Note = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.pnlBottom = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.pnlTop.SuspendLayout()
        CType(Me.nudDefaultMargin, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvPricing, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlBottom.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnlTop
        '
        Me.pnlTop.Controls.Add(Me.lblInstructions)
        Me.pnlTop.Controls.Add(Me.btnApplyToAll)
        Me.pnlTop.Controls.Add(Me.btnCalculate)
        Me.pnlTop.Controls.Add(Me.nudDefaultMargin)
        Me.pnlTop.Controls.Add(Me.Label3)
        Me.pnlTop.Controls.Add(Me.btnLoadCurrent)
        Me.pnlTop.Controls.Add(Me.dtpReportDate)
        Me.pnlTop.Controls.Add(Me.Label1)
        Me.pnlTop.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlTop.Location = New System.Drawing.Point(0, 0)
        Me.pnlTop.Name = "pnlTop"
        Me.pnlTop.Size = New System.Drawing.Size(834, 80)
        Me.pnlTop.TabIndex = 0
        '
        'lblInstructions
        '
        Me.lblInstructions.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblInstructions.ForeColor = System.Drawing.Color.Gray
        Me.lblInstructions.Location = New System.Drawing.Point(500, 15)
        Me.lblInstructions.Name = "lblInstructions"
        Me.lblInstructions.Size = New System.Drawing.Size(320, 45)
        Me.lblInstructions.TabIndex = 7
        Me.lblInstructions.Text = "Enter Random Lengths prices (MBF) for each category. Click 'Calculate Prices' to" &
    " compute sell prices based on margin."
        '
        'btnApplyToAll
        '
        Me.btnApplyToAll.Location = New System.Drawing.Point(330, 48)
        Me.btnApplyToAll.Name = "btnApplyToAll"
        Me.btnApplyToAll.Size = New System.Drawing.Size(130, 25)
        Me.btnApplyToAll.TabIndex = 6
        Me.btnApplyToAll.Text = "Apply RL Date to All"
        Me.btnApplyToAll.UseVisualStyleBackColor = True
        '
        'btnCalculate
        '
        Me.btnCalculate.Location = New System.Drawing.Point(200, 48)
        Me.btnCalculate.Name = "btnCalculate"
        Me.btnCalculate.Size = New System.Drawing.Size(110, 25)
        Me.btnCalculate.TabIndex = 5
        Me.btnCalculate.Text = "Calculate Prices"
        Me.btnCalculate.UseVisualStyleBackColor = True
        '
        'nudDefaultMargin
        '
        Me.nudDefaultMargin.DecimalPlaces = 2
        Me.nudDefaultMargin.Increment = New Decimal(New Integer() {1, 0, 0, 131072})
        Me.nudDefaultMargin.Location = New System.Drawing.Point(110, 50)
        Me.nudDefaultMargin.Maximum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nudDefaultMargin.Name = "nudDefaultMargin"
        Me.nudDefaultMargin.Size = New System.Drawing.Size(70, 20)
        Me.nudDefaultMargin.TabIndex = 4
        Me.nudDefaultMargin.Value = New Decimal(New Integer() {15, 0, 0, 131072})
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(15, 53)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(78, 13)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Default Margin:"
        '
        'btnLoadCurrent
        '
        Me.btnLoadCurrent.Location = New System.Drawing.Point(330, 13)
        Me.btnLoadCurrent.Name = "btnLoadCurrent"
        Me.btnLoadCurrent.Size = New System.Drawing.Size(130, 25)
        Me.btnLoadCurrent.TabIndex = 2
        Me.btnLoadCurrent.Text = "Load Current Pricing"
        Me.btnLoadCurrent.UseVisualStyleBackColor = True
        '
        'dtpReportDate
        '
        Me.dtpReportDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpReportDate.Location = New System.Drawing.Point(190, 15)
        Me.dtpReportDate.Name = "dtpReportDate"
        Me.dtpReportDate.Size = New System.Drawing.Size(120, 20)
        Me.dtpReportDate.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(15, 18)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(156, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Random Lengths Report Date:"
        '
        'dgvPricing
        '
        Me.dgvPricing.AllowUserToAddRows = False
        Me.dgvPricing.AllowUserToDeleteRows = False
        Me.dgvPricing.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvPricing.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvPricing.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPricing.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.MaterialPricingID, Me.MaterialCategoryID, Me.CategoryName, Me.RLDate, Me.RLPrice, Me.PreviousPrice, Me.CalculatedPrice, Me.FinalPrice, Me.PctChange, Me.Note})
        Me.dgvPricing.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvPricing.Location = New System.Drawing.Point(0, 80)
        Me.dgvPricing.MultiSelect = False
        Me.dgvPricing.Name = "dgvPricing"
        Me.dgvPricing.RowHeadersVisible = False
        Me.dgvPricing.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.dgvPricing.Size = New System.Drawing.Size(834, 470)
        Me.dgvPricing.TabIndex = 1
        '
        'MaterialPricingID
        '
        Me.MaterialPricingID.HeaderText = "MaterialPricingID"
        Me.MaterialPricingID.Name = "MaterialPricingID"
        Me.MaterialPricingID.Visible = False
        '
        'MaterialCategoryID
        '
        Me.MaterialCategoryID.HeaderText = "MaterialCategoryID"
        Me.MaterialCategoryID.Name = "MaterialCategoryID"
        Me.MaterialCategoryID.Visible = False
        '
        'CategoryName
        '
        Me.CategoryName.HeaderText = "Category"
        Me.CategoryName.Name = "CategoryName"
        Me.CategoryName.ReadOnly = True
        '
        'RLDate
        '
        Me.RLDate.HeaderText = "RL Date"
        Me.RLDate.Name = "RLDate"
        '
        'RLPrice
        '
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle1.Format = "N2"
        Me.RLPrice.DefaultCellStyle = DataGridViewCellStyle1
        Me.RLPrice.HeaderText = "RL Price (MBF)"
        Me.RLPrice.Name = "RLPrice"
        '
        'PreviousPrice
        '
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle2.ForeColor = System.Drawing.Color.Gray
        DataGridViewCellStyle2.Format = "N4"
        Me.PreviousPrice.DefaultCellStyle = DataGridViewCellStyle2
        Me.PreviousPrice.HeaderText = "Previous"
        Me.PreviousPrice.Name = "PreviousPrice"
        Me.PreviousPrice.ReadOnly = True
        '
        'CalculatedPrice
        '
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle3.Format = "N4"
        Me.CalculatedPrice.DefaultCellStyle = DataGridViewCellStyle3
        Me.CalculatedPrice.HeaderText = "Calculated"
        Me.CalculatedPrice.Name = "CalculatedPrice"
        Me.CalculatedPrice.ReadOnly = True
        '
        'FinalPrice
        '
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle4.Format = "N4"
        Me.FinalPrice.DefaultCellStyle = DataGridViewCellStyle4
        Me.FinalPrice.HeaderText = "Final Price"
        Me.FinalPrice.Name = "FinalPrice"
        '
        'PctChange
        '
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight
        DataGridViewCellStyle5.Format = "P1"
        Me.PctChange.DefaultCellStyle = DataGridViewCellStyle5
        Me.PctChange.HeaderText = "% Chg"
        Me.PctChange.Name = "PctChange"
        Me.PctChange.ReadOnly = True
        '
        'Note
        '
        Me.Note.HeaderText = "Note"
        Me.Note.Name = "Note"
        '
        'pnlBottom
        '
        Me.pnlBottom.Controls.Add(Me.btnCancel)
        Me.pnlBottom.Controls.Add(Me.btnSave)
        Me.pnlBottom.Controls.Add(Me.lblStatus)
        Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlBottom.Location = New System.Drawing.Point(0, 550)
        Me.pnlBottom.Name = "pnlBottom"
        Me.pnlBottom.Size = New System.Drawing.Size(834, 50)
        Me.pnlBottom.TabIndex = 2
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.Location = New System.Drawing.Point(744, 10)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(80, 30)
        Me.btnCancel.TabIndex = 2
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSave.Location = New System.Drawing.Point(654, 10)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(80, 30)
        Me.btnSave.TabIndex = 1
        Me.btnSave.Text = "Save All"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.ForeColor = System.Drawing.Color.Green
        Me.lblStatus.Location = New System.Drawing.Point(15, 17)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 13)
        Me.lblStatus.TabIndex = 0
        '
        'frmRandomLengthsImport
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(834, 600)
        Me.Controls.Add(Me.dgvPricing)
        Me.Controls.Add(Me.pnlBottom)
        Me.Controls.Add(Me.pnlTop)
        Me.MinimumSize = New System.Drawing.Size(700, 450)
        Me.Name = "frmRandomLengthsImport"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Random Lengths Pricing Import"
        Me.pnlTop.ResumeLayout(False)
        Me.pnlTop.PerformLayout()
        CType(Me.nudDefaultMargin, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvPricing, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlBottom.ResumeLayout(False)
        Me.pnlBottom.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnlTop As Panel
    Friend WithEvents lblInstructions As Label
    Friend WithEvents btnApplyToAll As Button
    Friend WithEvents btnCalculate As Button
    Friend WithEvents nudDefaultMargin As NumericUpDown
    Friend WithEvents Label3 As Label
    Friend WithEvents btnLoadCurrent As Button
    Friend WithEvents dtpReportDate As DateTimePicker
    Friend WithEvents Label1 As Label
    Friend WithEvents dgvPricing As DataGridView
    Friend WithEvents pnlBottom As Panel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnSave As Button
    Friend WithEvents lblStatus As Label
    Friend WithEvents MaterialPricingID As DataGridViewTextBoxColumn
    Friend WithEvents MaterialCategoryID As DataGridViewTextBoxColumn
    Friend WithEvents CategoryName As DataGridViewTextBoxColumn
    Friend WithEvents RLDate As DataGridViewTextBoxColumn
    Friend WithEvents RLPrice As DataGridViewTextBoxColumn
    Friend WithEvents PreviousPrice As DataGridViewTextBoxColumn
    Friend WithEvents CalculatedPrice As DataGridViewTextBoxColumn
    Friend WithEvents FinalPrice As DataGridViewTextBoxColumn
    Friend WithEvents PctChange As DataGridViewTextBoxColumn
    Friend WithEvents Note As DataGridViewTextBoxColumn
End Class
