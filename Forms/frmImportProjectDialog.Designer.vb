<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmImportProjectDialog
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblProjectName = New System.Windows.Forms.Label()
        Me.txtProjectName = New System.Windows.Forms.TextBox()
        Me.lblCustomerName = New System.Windows.Forms.Label()
        Me.cboCustomerName = New System.Windows.Forms.ComboBox()
        Me.cboEstimator = New System.Windows.Forms.ComboBox()
        Me.lblEstimator = New System.Windows.Forms.Label()
        Me.cboSales = New System.Windows.Forms.ComboBox()
        Me.lblSales = New System.Windows.Forms.Label()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.cboState = New System.Windows.Forms.ComboBox()
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
        Me.lblMilesToJobSite = New System.Windows.Forms.Label()
        Me.nudMilesToJobSite = New System.Windows.Forms.NumericUpDown()
        CType(Me.nudMilesToJobSite, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lblProjectName
        '
        Me.lblProjectName.AutoSize = True
        Me.lblProjectName.Location = New System.Drawing.Point(67, 35)
        Me.lblProjectName.Name = "lblProjectName"
        Me.lblProjectName.Size = New System.Drawing.Size(74, 13)
        Me.lblProjectName.TabIndex = 0
        Me.lblProjectName.Text = "Project Name:"
        '
        'txtProjectName
        '
        Me.txtProjectName.Location = New System.Drawing.Point(147, 32)
        Me.txtProjectName.Name = "txtProjectName"
        Me.txtProjectName.Size = New System.Drawing.Size(242, 20)
        Me.txtProjectName.TabIndex = 1
        '
        'lblCustomerName
        '
        Me.lblCustomerName.AutoSize = True
        Me.lblCustomerName.Location = New System.Drawing.Point(56, 61)
        Me.lblCustomerName.Name = "lblCustomerName"
        Me.lblCustomerName.Size = New System.Drawing.Size(85, 13)
        Me.lblCustomerName.TabIndex = 2
        Me.lblCustomerName.Text = "Customer Name:"
        '
        'cboCustomerName
        '
        Me.cboCustomerName.FormattingEnabled = True
        Me.cboCustomerName.Location = New System.Drawing.Point(147, 58)
        Me.cboCustomerName.Name = "cboCustomerName"
        Me.cboCustomerName.Size = New System.Drawing.Size(239, 21)
        Me.cboCustomerName.TabIndex = 3
        '
        'cboEstimator
        '
        Me.cboEstimator.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboEstimator.FormattingEnabled = True
        Me.cboEstimator.Location = New System.Drawing.Point(147, 85)
        Me.cboEstimator.Name = "cboEstimator"
        Me.cboEstimator.Size = New System.Drawing.Size(239, 21)
        Me.cboEstimator.TabIndex = 5
        '
        'lblEstimator
        '
        Me.lblEstimator.AutoSize = True
        Me.lblEstimator.Location = New System.Drawing.Point(56, 88)
        Me.lblEstimator.Name = "lblEstimator"
        Me.lblEstimator.Size = New System.Drawing.Size(84, 13)
        Me.lblEstimator.TabIndex = 4
        Me.lblEstimator.Text = "Estimator Name:"
        '
        'cboSales
        '
        Me.cboSales.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboSales.FormattingEnabled = True
        Me.cboSales.Location = New System.Drawing.Point(147, 112)
        Me.cboSales.Name = "cboSales"
        Me.cboSales.Size = New System.Drawing.Size(239, 21)
        Me.cboSales.TabIndex = 7
        '
        'lblSales
        '
        Me.lblSales.AutoSize = True
        Me.lblSales.Location = New System.Drawing.Point(73, 115)
        Me.lblSales.Name = "lblSales"
        Me.lblSales.Size = New System.Drawing.Size(67, 13)
        Me.lblSales.TabIndex = 6
        Me.lblSales.Text = "Sales Name:"
        '
        'btnOK
        '
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOK.Location = New System.Drawing.Point(468, 360)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(69, 29)
        Me.btnOK.TabIndex = 8
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(549, 360)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(72, 28)
        Me.btnCancel.TabIndex = 9
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'cboState
        '
        Me.cboState.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboState.FormattingEnabled = True
        Me.cboState.Items.AddRange(New Object() {"AK", "AL", "AR", "AZ", "CA", "CO", "CT", "DE", "FL", "GA", "HI", "IA", "ID", "IL", "IN", "KS", "KY", "LA", "ME", "MD", "MA", "MI", "MN", "MO", "MS", "MT", "NC", "ND", "NE", "NH", "NJ", "NM", "NV", "NY", "OH", "OK", "OR", "PA", "RI", "SC", "SD", "TN", "TX", "UT", "VA", "VT", "WA", "WI", "WV", "WY"})
        Me.cboState.Location = New System.Drawing.Point(422, 165)
        Me.cboState.Name = "cboState"
        Me.cboState.Size = New System.Drawing.Size(51, 21)
        Me.cboState.TabIndex = 29
        '
        'lblAddress
        '
        Me.lblAddress.AutoSize = True
        Me.lblAddress.Location = New System.Drawing.Point(93, 142)
        Me.lblAddress.Name = "lblAddress"
        Me.lblAddress.Size = New System.Drawing.Size(48, 13)
        Me.lblAddress.TabIndex = 33
        Me.lblAddress.Text = "Address:"
        '
        'txtAddress
        '
        Me.txtAddress.Location = New System.Drawing.Point(147, 139)
        Me.txtAddress.Name = "txtAddress"
        Me.txtAddress.Size = New System.Drawing.Size(439, 20)
        Me.txtAddress.TabIndex = 27
        '
        'lblCity
        '
        Me.lblCity.AutoSize = True
        Me.lblCity.Location = New System.Drawing.Point(114, 168)
        Me.lblCity.Name = "lblCity"
        Me.lblCity.Size = New System.Drawing.Size(27, 13)
        Me.lblCity.TabIndex = 36
        Me.lblCity.Text = "City:"
        '
        'txtCity
        '
        Me.txtCity.Location = New System.Drawing.Point(147, 165)
        Me.txtCity.Name = "txtCity"
        Me.txtCity.Size = New System.Drawing.Size(228, 20)
        Me.txtCity.TabIndex = 28
        '
        'lblState
        '
        Me.lblState.AutoSize = True
        Me.lblState.Location = New System.Drawing.Point(381, 170)
        Me.lblState.Name = "lblState"
        Me.lblState.Size = New System.Drawing.Size(35, 13)
        Me.lblState.TabIndex = 37
        Me.lblState.Text = "State:"
        '
        'lblZip
        '
        Me.lblZip.AutoSize = True
        Me.lblZip.Location = New System.Drawing.Point(481, 170)
        Me.lblZip.Name = "lblZip"
        Me.lblZip.Size = New System.Drawing.Size(25, 13)
        Me.lblZip.TabIndex = 38
        Me.lblZip.Text = "Zip:"
        '
        'txtZip
        '
        Me.txtZip.Location = New System.Drawing.Point(512, 165)
        Me.txtZip.Name = "txtZip"
        Me.txtZip.Size = New System.Drawing.Size(74, 20)
        Me.txtZip.TabIndex = 30
        '
        'lblBidDate
        '
        Me.lblBidDate.AutoSize = True
        Me.lblBidDate.Location = New System.Drawing.Point(90, 197)
        Me.lblBidDate.Name = "lblBidDate"
        Me.lblBidDate.Size = New System.Drawing.Size(51, 13)
        Me.lblBidDate.TabIndex = 39
        Me.lblBidDate.Text = "Bid Date:"
        '
        'dtpBidDate
        '
        Me.dtpBidDate.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpBidDate.Location = New System.Drawing.Point(147, 191)
        Me.dtpBidDate.Name = "dtpBidDate"
        Me.dtpBidDate.Size = New System.Drawing.Size(100, 20)
        Me.dtpBidDate.TabIndex = 31
        '
        'lblArchPlansDated
        '
        Me.lblArchPlansDated.AutoSize = True
        Me.lblArchPlansDated.Location = New System.Drawing.Point(387, 197)
        Me.lblArchPlansDated.Name = "lblArchPlansDated"
        Me.lblArchPlansDated.Size = New System.Drawing.Size(93, 13)
        Me.lblArchPlansDated.TabIndex = 40
        Me.lblArchPlansDated.Text = "Arch Plans Dated:"
        '
        'dtpArchPlansDated
        '
        Me.dtpArchPlansDated.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpArchPlansDated.Location = New System.Drawing.Point(486, 191)
        Me.dtpArchPlansDated.Name = "dtpArchPlansDated"
        Me.dtpArchPlansDated.Size = New System.Drawing.Size(100, 20)
        Me.dtpArchPlansDated.TabIndex = 34
        '
        'lblEngPlansDated
        '
        Me.lblEngPlansDated.AutoSize = True
        Me.lblEngPlansDated.Location = New System.Drawing.Point(390, 222)
        Me.lblEngPlansDated.Name = "lblEngPlansDated"
        Me.lblEngPlansDated.Size = New System.Drawing.Size(90, 13)
        Me.lblEngPlansDated.TabIndex = 41
        Me.lblEngPlansDated.Text = "Eng Plans Dated:"
        '
        'dtpEngPlansDated
        '
        Me.dtpEngPlansDated.Format = System.Windows.Forms.DateTimePickerFormat.[Short]
        Me.dtpEngPlansDated.Location = New System.Drawing.Point(486, 216)
        Me.dtpEngPlansDated.Name = "dtpEngPlansDated"
        Me.dtpEngPlansDated.Size = New System.Drawing.Size(100, 20)
        Me.dtpEngPlansDated.TabIndex = 35
        '
        'lblMilesToJobSite
        '
        Me.lblMilesToJobSite.AutoSize = True
        Me.lblMilesToJobSite.Location = New System.Drawing.Point(53, 223)
        Me.lblMilesToJobSite.Name = "lblMilesToJobSite"
        Me.lblMilesToJobSite.Size = New System.Drawing.Size(88, 13)
        Me.lblMilesToJobSite.TabIndex = 42
        Me.lblMilesToJobSite.Text = "Miles To JobSite:"
        '
        'nudMilesToJobSite
        '
        Me.nudMilesToJobSite.Location = New System.Drawing.Point(147, 217)
        Me.nudMilesToJobSite.Maximum = New Decimal(New Integer() {999999, 0, 0, 0})
        Me.nudMilesToJobSite.Name = "nudMilesToJobSite"
        Me.nudMilesToJobSite.Size = New System.Drawing.Size(100, 20)
        Me.nudMilesToJobSite.TabIndex = 32
        '
        'frmImportProjectDialog
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(633, 400)
        Me.Controls.Add(Me.cboState)
        Me.Controls.Add(Me.lblAddress)
        Me.Controls.Add(Me.txtAddress)
        Me.Controls.Add(Me.lblCity)
        Me.Controls.Add(Me.txtCity)
        Me.Controls.Add(Me.lblState)
        Me.Controls.Add(Me.lblZip)
        Me.Controls.Add(Me.txtZip)
        Me.Controls.Add(Me.lblBidDate)
        Me.Controls.Add(Me.dtpBidDate)
        Me.Controls.Add(Me.lblArchPlansDated)
        Me.Controls.Add(Me.dtpArchPlansDated)
        Me.Controls.Add(Me.lblEngPlansDated)
        Me.Controls.Add(Me.dtpEngPlansDated)
        Me.Controls.Add(Me.lblMilesToJobSite)
        Me.Controls.Add(Me.nudMilesToJobSite)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnOK)
        Me.Controls.Add(Me.cboSales)
        Me.Controls.Add(Me.lblSales)
        Me.Controls.Add(Me.cboEstimator)
        Me.Controls.Add(Me.lblEstimator)
        Me.Controls.Add(Me.cboCustomerName)
        Me.Controls.Add(Me.lblCustomerName)
        Me.Controls.Add(Me.txtProjectName)
        Me.Controls.Add(Me.lblProjectName)
        Me.Name = "frmImportProjectDialog"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Import Project from CSV"
        CType(Me.nudMilesToJobSite, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblProjectName As Label
    Friend WithEvents txtProjectName As TextBox
    Friend WithEvents lblCustomerName As Label
    Friend WithEvents cboCustomerName As ComboBox
    Friend WithEvents cboEstimator As ComboBox
    Friend WithEvents lblEstimator As Label
    Friend WithEvents cboSales As ComboBox
    Friend WithEvents lblSales As Label
    Friend WithEvents btnOK As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents cboState As ComboBox
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
    Friend WithEvents lblMilesToJobSite As Label
    Friend WithEvents nudMilesToJobSite As NumericUpDown
End Class
