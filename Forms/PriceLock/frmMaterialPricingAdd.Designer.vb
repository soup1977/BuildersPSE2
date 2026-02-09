<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMaterialPricingAdd
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
        Me.Label1 = New System.Windows.Forms.Label()
        Me.cboCategory = New System.Windows.Forms.ComboBox()
        Me.btnAddCategory = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.dtpRandomLengthsDate = New System.Windows.Forms.DateTimePicker()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.nudRandomLengthsPrice = New System.Windows.Forms.NumericUpDown()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.nudCalculatedPrice = New System.Windows.Forms.NumericUpDown()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.nudPriceSentToBuilder = New System.Windows.Forms.NumericUpDown()
        Me.lblPriceDiffWarning = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtPriceNote = New System.Windows.Forms.TextBox()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.lblPreviousApprovedLabel = New System.Windows.Forms.Label()
        Me.lblPreviousApproved = New System.Windows.Forms.Label()
        CType(Me.nudRandomLengthsPrice, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudCalculatedPrice, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nudPriceSentToBuilder, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(20, 23)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(52, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Category:"
        '
        'cboCategory
        '
        Me.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboCategory.FormattingEnabled = True
        Me.cboCategory.Location = New System.Drawing.Point(150, 20)
        Me.cboCategory.Name = "cboCategory"
        Me.cboCategory.Size = New System.Drawing.Size(200, 21)
        Me.cboCategory.TabIndex = 1
        '
        'btnAddCategory
        '
        Me.btnAddCategory.Location = New System.Drawing.Point(355, 19)
        Me.btnAddCategory.Name = "btnAddCategory"
        Me.btnAddCategory.Size = New System.Drawing.Size(50, 23)
        Me.btnAddCategory.TabIndex = 2
        Me.btnAddCategory.Text = "Add..."
        Me.btnAddCategory.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.ForeColor = System.Drawing.Color.Gray
        Me.Label2.Location = New System.Drawing.Point(20, 55)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(168, 13)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "─── Random Lengths Data ───"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(22, 107)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(85, 13)
        Me.Label3.TabIndex = 4
        Me.Label3.Text = "RL Report Date:"
        '
        'dtpRandomLengthsDate
        '
        Me.dtpRandomLengthsDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpRandomLengthsDate.Location = New System.Drawing.Point(150, 106)
        Me.dtpRandomLengthsDate.Name = "dtpRandomLengthsDate"
        Me.dtpRandomLengthsDate.Size = New System.Drawing.Size(120, 20)
        Me.dtpRandomLengthsDate.TabIndex = 5
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(20, 135)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(82, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "RL Price (MBF):"
        '
        'nudRandomLengthsPrice
        '
        Me.nudRandomLengthsPrice.DecimalPlaces = 2
        Me.nudRandomLengthsPrice.Location = New System.Drawing.Point(150, 132)
        Me.nudRandomLengthsPrice.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.nudRandomLengthsPrice.Name = "nudRandomLengthsPrice"
        Me.nudRandomLengthsPrice.Size = New System.Drawing.Size(100, 20)
        Me.nudRandomLengthsPrice.TabIndex = 7
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.ForeColor = System.Drawing.Color.Gray
        Me.Label5.Location = New System.Drawing.Point(20, 170)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(130, 13)
        Me.Label5.TabIndex = 8
        Me.Label5.Text = "─── Price Tracking ───"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(20, 198)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(87, 13)
        Me.Label6.TabIndex = 9
        Me.Label6.Text = "Calculated Price:"
        '
        'nudCalculatedPrice
        '
        Me.nudCalculatedPrice.DecimalPlaces = 4
        Me.nudCalculatedPrice.Location = New System.Drawing.Point(150, 195)
        Me.nudCalculatedPrice.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.nudCalculatedPrice.Minimum = New Decimal(New Integer() {99999, 0, 0, -2147483648})
        Me.nudCalculatedPrice.Name = "nudCalculatedPrice"
        Me.nudCalculatedPrice.Size = New System.Drawing.Size(100, 20)
        Me.nudCalculatedPrice.TabIndex = 10
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.ForeColor = System.Drawing.Color.Gray
        Me.Label7.Location = New System.Drawing.Point(260, 198)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(85, 13)
        Me.Label7.TabIndex = 11
        Me.Label7.Text = "(= Sent to Sales)"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(20, 228)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(81, 13)
        Me.Label8.TabIndex = 12
        Me.Label8.Text = "Price to Builder:"
        '
        'nudPriceSentToBuilder
        '
        Me.nudPriceSentToBuilder.DecimalPlaces = 4
        Me.nudPriceSentToBuilder.Location = New System.Drawing.Point(150, 225)
        Me.nudPriceSentToBuilder.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.nudPriceSentToBuilder.Minimum = New Decimal(New Integer() {99999, 0, 0, -2147483648})
        Me.nudPriceSentToBuilder.Name = "nudPriceSentToBuilder"
        Me.nudPriceSentToBuilder.Size = New System.Drawing.Size(100, 20)
        Me.nudPriceSentToBuilder.TabIndex = 13
        '
        'lblPriceDiffWarning
        '
        Me.lblPriceDiffWarning.AutoSize = True
        Me.lblPriceDiffWarning.ForeColor = System.Drawing.Color.DarkOrange
        Me.lblPriceDiffWarning.Location = New System.Drawing.Point(150, 251)
        Me.lblPriceDiffWarning.Name = "lblPriceDiffWarning"
        Me.lblPriceDiffWarning.Size = New System.Drawing.Size(213, 13)
        Me.lblPriceDiffWarning.TabIndex = 14
        Me.lblPriceDiffWarning.Text = "⚠ Builder price differs from Calculated price"
        Me.lblPriceDiffWarning.Visible = False
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(20, 279)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(33, 13)
        Me.Label9.TabIndex = 15
        Me.Label9.Text = "Note:"
        '
        'txtPriceNote
        '
        Me.txtPriceNote.Location = New System.Drawing.Point(150, 279)
        Me.txtPriceNote.Multiline = True
        Me.txtPriceNote.Name = "txtPriceNote"
        Me.txtPriceNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtPriceNote.Size = New System.Drawing.Size(250, 50)
        Me.txtPriceNote.TabIndex = 16
        '
        'btnOK
        '
        Me.btnOK.Location = New System.Drawing.Point(230, 344)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(80, 30)
        Me.btnOK.TabIndex = 17
        Me.btnOK.Text = "Add"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(320, 344)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(80, 30)
        Me.btnCancel.TabIndex = 18
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'lblPreviousApprovedLabel
        '
        Me.lblPreviousApprovedLabel.AutoSize = True
        Me.lblPreviousApprovedLabel.Location = New System.Drawing.Point(12, 80)
        Me.lblPreviousApprovedLabel.Name = "lblPreviousApprovedLabel"
        Me.lblPreviousApprovedLabel.Size = New System.Drawing.Size(127, 13)
        Me.lblPreviousApprovedLabel.TabIndex = 10
        Me.lblPreviousApprovedLabel.Text = "Previous Approved Price:"
        '
        'lblPreviousApproved
        '
        Me.lblPreviousApproved.AutoSize = True
        Me.lblPreviousApproved.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblPreviousApproved.ForeColor = System.Drawing.Color.Gray
        Me.lblPreviousApproved.Location = New System.Drawing.Point(160, 80)
        Me.lblPreviousApproved.Name = "lblPreviousApproved"
        Me.lblPreviousApproved.Size = New System.Drawing.Size(17, 15)
        Me.lblPreviousApproved.TabIndex = 11
        Me.lblPreviousApproved.Text = "--"
        '
        'frmMaterialPricingAdd
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(434, 395)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.txtPriceNote)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.lblPriceDiffWarning)
        Me.Controls.Add(Me.nudPriceSentToBuilder)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.nudCalculatedPrice)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.nudRandomLengthsPrice)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.dtpRandomLengthsDate)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnAddCategory)
        Me.Controls.Add(Me.cboCategory)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.lblPreviousApprovedLabel)
        Me.Controls.Add(Me.lblPreviousApproved)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmMaterialPricingAdd"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Add Material Pricing"
        CType(Me.nudRandomLengthsPrice, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudCalculatedPrice, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nudPriceSentToBuilder, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents Label1 As Label
    Friend WithEvents cboCategory As ComboBox
    Friend WithEvents btnAddCategory As Button
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents dtpRandomLengthsDate As DateTimePicker
    Friend WithEvents Label4 As Label
    Friend WithEvents nudRandomLengthsPrice As NumericUpDown
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents nudCalculatedPrice As NumericUpDown
    Friend WithEvents Label7 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents nudPriceSentToBuilder As NumericUpDown
    Friend WithEvents lblPriceDiffWarning As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents txtPriceNote As TextBox
    Friend WithEvents btnOK As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents lblPreviousApprovedLabel As Label
    Friend WithEvents lblPreviousApproved As Label
End Class
