' =====================================================
' frmRandomLengthsImportEdit.Designer.vb
' Designer file for Random Lengths Import Edit dialog
' BuildersPSE2 - PriceLock Module
' =====================================================

<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmRandomLengthsImportEdit
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
        Me.lblReportDate = New System.Windows.Forms.Label()
        Me.dtpReportDate = New System.Windows.Forms.DateTimePicker()
        Me.lblReportName = New System.Windows.Forms.Label()
        Me.txtReportName = New System.Windows.Forms.TextBox()
        Me.lblImportMethod = New System.Windows.Forms.Label()
        Me.cboImportMethod = New System.Windows.Forms.ComboBox()
        Me.lblNotes = New System.Windows.Forms.Label()
        Me.txtNotes = New System.Windows.Forms.TextBox()
        Me.lblSourceFile = New System.Windows.Forms.Label()
        Me.grpExcelImport = New System.Windows.Forms.GroupBox()
        Me.btnImportExcel = New System.Windows.Forms.Button()
        Me.btnBrowseExcel = New System.Windows.Forms.Button()
        Me.txtExcelPath = New System.Windows.Forms.TextBox()
        Me.lblExcelPath = New System.Windows.Forms.Label()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.btnOK = New System.Windows.Forms.Button()
        Me.grpExcelImport.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'lblReportDate
        '
        Me.lblReportDate.AutoSize = True
        Me.lblReportDate.Location = New System.Drawing.Point(20, 25)
        Me.lblReportDate.Name = "lblReportDate"
        Me.lblReportDate.Size = New System.Drawing.Size(76, 15)
        Me.lblReportDate.TabIndex = 0
        Me.lblReportDate.Text = "Report Date:"
        '
        'dtpReportDate
        '
        Me.dtpReportDate.Format = System.Windows.Forms.DateTimePickerFormat.Short
        Me.dtpReportDate.Location = New System.Drawing.Point(120, 22)
        Me.dtpReportDate.Name = "dtpReportDate"
        Me.dtpReportDate.Size = New System.Drawing.Size(120, 23)
        Me.dtpReportDate.TabIndex = 1
        '
        'lblReportName
        '
        Me.lblReportName.AutoSize = True
        Me.lblReportName.Location = New System.Drawing.Point(20, 60)
        Me.lblReportName.Name = "lblReportName"
        Me.lblReportName.Size = New System.Drawing.Size(83, 15)
        Me.lblReportName.TabIndex = 2
        Me.lblReportName.Text = "Report Name:"
        '
        'txtReportName
        '
        Me.txtReportName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtReportName.Location = New System.Drawing.Point(120, 57)
        Me.txtReportName.MaxLength = 100
        Me.txtReportName.Name = "txtReportName"
        Me.txtReportName.Size = New System.Drawing.Size(440, 23)
        Me.txtReportName.TabIndex = 3
        '
        'lblImportMethod
        '
        Me.lblImportMethod.AutoSize = True
        Me.lblImportMethod.Location = New System.Drawing.Point(20, 95)
        Me.lblImportMethod.Name = "lblImportMethod"
        Me.lblImportMethod.Size = New System.Drawing.Size(93, 15)
        Me.lblImportMethod.TabIndex = 4
        Me.lblImportMethod.Text = "Import Method:"
        '
        'cboImportMethod
        '
        Me.cboImportMethod.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboImportMethod.FormattingEnabled = True
        Me.cboImportMethod.Location = New System.Drawing.Point(120, 92)
        Me.cboImportMethod.Name = "cboImportMethod"
        Me.cboImportMethod.Size = New System.Drawing.Size(120, 23)
        Me.cboImportMethod.TabIndex = 5
        '
        'lblNotes
        '
        Me.lblNotes.AutoSize = True
        Me.lblNotes.Location = New System.Drawing.Point(20, 130)
        Me.lblNotes.Name = "lblNotes"
        Me.lblNotes.Size = New System.Drawing.Size(41, 15)
        Me.lblNotes.TabIndex = 6
        Me.lblNotes.Text = "Notes:"
        '
        'txtNotes
        '
        Me.txtNotes.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtNotes.Location = New System.Drawing.Point(120, 127)
        Me.txtNotes.MaxLength = 500
        Me.txtNotes.Multiline = True
        Me.txtNotes.Name = "txtNotes"
        Me.txtNotes.ScrollBars = System.Windows.Forms.ScrollBars.Vertical
        Me.txtNotes.Size = New System.Drawing.Size(440, 60)
        Me.txtNotes.TabIndex = 7
        '
        'lblSourceFile
        '
        Me.lblSourceFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblSourceFile.ForeColor = System.Drawing.Color.Gray
        Me.lblSourceFile.Location = New System.Drawing.Point(20, 195)
        Me.lblSourceFile.Name = "lblSourceFile"
        Me.lblSourceFile.Size = New System.Drawing.Size(540, 20)
        Me.lblSourceFile.TabIndex = 8
        Me.lblSourceFile.Text = "Source: (none)"
        Me.lblSourceFile.Visible = False
        '
        'grpExcelImport
        '
        Me.grpExcelImport.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpExcelImport.Controls.Add(Me.btnImportExcel)
        Me.grpExcelImport.Controls.Add(Me.btnBrowseExcel)
        Me.grpExcelImport.Controls.Add(Me.txtExcelPath)
        Me.grpExcelImport.Controls.Add(Me.lblExcelPath)
        Me.grpExcelImport.Location = New System.Drawing.Point(20, 220)
        Me.grpExcelImport.Name = "grpExcelImport"
        Me.grpExcelImport.Size = New System.Drawing.Size(540, 90)
        Me.grpExcelImport.TabIndex = 9
        Me.grpExcelImport.TabStop = False
        Me.grpExcelImport.Text = "Excel Import"
        '
        'btnImportExcel
        '
        Me.btnImportExcel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnImportExcel.Enabled = False
        Me.btnImportExcel.Location = New System.Drawing.Point(400, 50)
        Me.btnImportExcel.Name = "btnImportExcel"
        Me.btnImportExcel.Size = New System.Drawing.Size(120, 28)
        Me.btnImportExcel.TabIndex = 3
        Me.btnImportExcel.Text = "Import from Excel"
        Me.btnImportExcel.UseVisualStyleBackColor = True
        '
        'btnBrowseExcel
        '
        Me.btnBrowseExcel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBrowseExcel.Location = New System.Drawing.Point(470, 22)
        Me.btnBrowseExcel.Name = "btnBrowseExcel"
        Me.btnBrowseExcel.Size = New System.Drawing.Size(50, 23)
        Me.btnBrowseExcel.TabIndex = 2
        Me.btnBrowseExcel.Text = "..."
        Me.btnBrowseExcel.UseVisualStyleBackColor = True
        '
        'txtExcelPath
        '
        Me.txtExcelPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtExcelPath.Location = New System.Drawing.Point(80, 22)
        Me.txtExcelPath.Name = "txtExcelPath"
        Me.txtExcelPath.ReadOnly = True
        Me.txtExcelPath.Size = New System.Drawing.Size(380, 23)
        Me.txtExcelPath.TabIndex = 1
        '
        'lblExcelPath
        '
        Me.lblExcelPath.AutoSize = True
        Me.lblExcelPath.Location = New System.Drawing.Point(15, 25)
        Me.lblExcelPath.Name = "lblExcelPath"
        Me.lblExcelPath.Size = New System.Drawing.Size(59, 15)
        Me.lblExcelPath.TabIndex = 0
        Me.lblExcelPath.Text = "Excel File:"
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Controls.Add(Me.btnOK)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Location = New System.Drawing.Point(0, 320)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlButtons.Size = New System.Drawing.Size(580, 50)
        Me.pnlButtons.TabIndex = 10
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(480, 11)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(85, 28)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOK.Location = New System.Drawing.Point(385, 11)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(85, 28)
        Me.btnOK.TabIndex = 0
        Me.btnOK.Text = "OK"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'frmRandomLengthsImportEdit
        '
        Me.AcceptButton = Me.btnOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 15.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(580, 370)
        Me.Controls.Add(Me.pnlButtons)
        Me.Controls.Add(Me.grpExcelImport)
        Me.Controls.Add(Me.lblSourceFile)
        Me.Controls.Add(Me.txtNotes)
        Me.Controls.Add(Me.lblNotes)
        Me.Controls.Add(Me.cboImportMethod)
        Me.Controls.Add(Me.lblImportMethod)
        Me.Controls.Add(Me.txtReportName)
        Me.Controls.Add(Me.lblReportName)
        Me.Controls.Add(Me.dtpReportDate)
        Me.Controls.Add(Me.lblReportDate)
        Me.Font = New System.Drawing.Font("Segoe UI", 9.0!)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "frmRandomLengthsImportEdit"
        Me.ShowInTaskbar = False
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Random Lengths Import"
        Me.grpExcelImport.ResumeLayout(False)
        Me.grpExcelImport.PerformLayout()
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents lblReportDate As Label
    Friend WithEvents dtpReportDate As DateTimePicker
    Friend WithEvents lblReportName As Label
    Friend WithEvents txtReportName As TextBox
    Friend WithEvents lblImportMethod As Label
    Friend WithEvents cboImportMethod As ComboBox
    Friend WithEvents lblNotes As Label
    Friend WithEvents txtNotes As TextBox
    Friend WithEvents lblSourceFile As Label
    Friend WithEvents grpExcelImport As GroupBox
    Friend WithEvents btnImportExcel As Button
    Friend WithEvents btnBrowseExcel As Button
    Friend WithEvents txtExcelPath As TextBox
    Friend WithEvents lblExcelPath As Label
    Friend WithEvents pnlButtons As Panel
    Friend WithEvents btnCancel As Button
    Friend WithEvents btnOK As Button

End Class