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
        Me.lblSales.Location = New System.Drawing.Point(59, 115)
        Me.lblSales.Name = "lblSales"
        Me.lblSales.Size = New System.Drawing.Size(67, 13)
        Me.lblSales.TabIndex = 6
        Me.lblSales.Text = "Sales Name:"
        '
        'btnOK
        '
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOK.Location = New System.Drawing.Point(232, 160)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(69, 29)
        Me.btnOK.TabIndex = 8
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(313, 160)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(72, 28)
        Me.btnCancel.TabIndex = 9
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmImportProjectDialog
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(446, 230)
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
End Class
