<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class WallImportMappingForm
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
        Me.btnImport = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.lblHeader = New System.Windows.Forms.Label()
        Me.dgvMapping = New System.Windows.Forms.DataGridView()
        Me.colCSVKey = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colArrow = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colTarget = New System.Windows.Forms.DataGridViewComboBoxColumn()
        Me.chkCreateLevels = New System.Windows.Forms.CheckBox()
        CType(Me.dgvMapping, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'btnImport
        '
        Me.btnImport.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnImport.Location = New System.Drawing.Point(639, 418)
        Me.btnImport.Name = "btnImport"
        Me.btnImport.Size = New System.Drawing.Size(70, 26)
        Me.btnImport.TabIndex = 0
        Me.btnImport.Text = "Import"
        Me.btnImport.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(715, 419)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(73, 24)
        Me.btnCancel.TabIndex = 1
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'lblHeader
        '
        Me.lblHeader.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblHeader.AutoSize = True
        Me.lblHeader.Location = New System.Drawing.Point(32, 12)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New System.Drawing.Size(10, 13)
        Me.lblHeader.TabIndex = 2
        Me.lblHeader.Text = "."
        '
        'dgvMapping
        '
        Me.dgvMapping.AllowUserToAddRows = False
        Me.dgvMapping.AllowUserToDeleteRows = False
        Me.dgvMapping.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvMapping.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvMapping.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMapping.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colCSVKey, Me.colArrow, Me.colTarget})
        Me.dgvMapping.Location = New System.Drawing.Point(12, 35)
        Me.dgvMapping.Name = "dgvMapping"
        Me.dgvMapping.RowHeadersVisible = False
        Me.dgvMapping.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvMapping.Size = New System.Drawing.Size(776, 377)
        Me.dgvMapping.TabIndex = 3
        '
        'colCSVKey
        '
        Me.colCSVKey.DataPropertyName = "CsvKey"
        Me.colCSVKey.HeaderText = "CSV Building"
        Me.colCSVKey.Name = "colCSVKey"
        '
        'colArrow
        '
        Me.colArrow.HeaderText = "Maps To ->"
        Me.colArrow.Name = "colArrow"
        '
        'colTarget
        '
        Me.colTarget.DataPropertyName = "TargetBuildingID"
        Me.colTarget.HeaderText = "Target Building"
        Me.colTarget.Name = "colTarget"
        '
        'chkCreateLevels
        '
        Me.chkCreateLevels.AutoSize = True
        Me.chkCreateLevels.Checked = True
        Me.chkCreateLevels.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkCreateLevels.Location = New System.Drawing.Point(16, 421)
        Me.chkCreateLevels.Name = "chkCreateLevels"
        Me.chkCreateLevels.Size = New System.Drawing.Size(194, 17)
        Me.chkCreateLevels.TabIndex = 4
        Me.chkCreateLevels.Text = "Create missing levels automatically?"
        Me.chkCreateLevels.UseVisualStyleBackColor = True
        Me.chkCreateLevels.Visible = False
        '
        'WallImportMappingForm
        '
        Me.AcceptButton = Me.btnImport
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.btnCancel
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.chkCreateLevels)
        Me.Controls.Add(Me.dgvMapping)
        Me.Controls.Add(Me.lblHeader)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnImport)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "WallImportMappingForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Import Wall Data - Building Mapping"
        CType(Me.dgvMapping, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents btnImport As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents lblHeader As Label
    Friend WithEvents dgvMapping As DataGridView
    Friend WithEvents colCSVKey As DataGridViewTextBoxColumn
    Friend WithEvents colArrow As DataGridViewTextBoxColumn
    Friend WithEvents colTarget As DataGridViewComboBoxColumn
    Friend WithEvents chkCreateLevels As CheckBox
End Class
