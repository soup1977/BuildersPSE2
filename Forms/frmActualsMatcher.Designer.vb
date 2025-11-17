<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmActualsMatcher
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
        Me.dgvMitek = New System.Windows.Forms.DataGridView()
        Me.dgvProject = New System.Windows.Forms.DataGridView()
        Me.chkMatchAllSameName = New System.Windows.Forms.CheckBox()
        Me.btnMatchSelected = New System.Windows.Forms.Button()
        Me.btnAutoMatchByName = New System.Windows.Forms.Button()
        Me.btnClearMatches = New System.Windows.Forms.Button()
        Me.btnImportMatched = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        CType(Me.dgvMitek, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvProject, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvMitek
        '
        Me.dgvMitek.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvMitek.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMitek.Location = New System.Drawing.Point(12, 35)
        Me.dgvMitek.Name = "dgvMitek"
        Me.dgvMitek.ReadOnly = True
        Me.dgvMitek.Size = New System.Drawing.Size(475, 475)
        Me.dgvMitek.TabIndex = 0
        '
        'dgvProject
        '
        Me.dgvProject.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvProject.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvProject.Location = New System.Drawing.Point(500, 35)
        Me.dgvProject.Name = "dgvProject"
        Me.dgvProject.ReadOnly = True
        Me.dgvProject.Size = New System.Drawing.Size(475, 475)
        Me.dgvProject.TabIndex = 1
        '
        'chkMatchAllSameName
        '
        Me.chkMatchAllSameName.AutoSize = True
        Me.chkMatchAllSameName.Location = New System.Drawing.Point(12, 12)
        Me.chkMatchAllSameName.Name = "chkMatchAllSameName"
        Me.chkMatchAllSameName.Size = New System.Drawing.Size(131, 17)
        Me.chkMatchAllSameName.TabIndex = 2
        Me.chkMatchAllSameName.Text = "Match All Same Name"
        Me.chkMatchAllSameName.UseVisualStyleBackColor = True
        '
        'btnMatchSelected
        '
        Me.btnMatchSelected.Location = New System.Drawing.Point(410, 525)
        Me.btnMatchSelected.Name = "btnMatchSelected"
        Me.btnMatchSelected.Size = New System.Drawing.Size(105, 29)
        Me.btnMatchSelected.TabIndex = 3
        Me.btnMatchSelected.Text = "Match Selected"
        Me.btnMatchSelected.UseVisualStyleBackColor = True
        '
        'btnAutoMatchByName
        '
        Me.btnAutoMatchByName.Location = New System.Drawing.Point(521, 525)
        Me.btnAutoMatchByName.Name = "btnAutoMatchByName"
        Me.btnAutoMatchByName.Size = New System.Drawing.Size(118, 29)
        Me.btnAutoMatchByName.TabIndex = 4
        Me.btnAutoMatchByName.Text = "Auto Match by Name"
        Me.btnAutoMatchByName.UseVisualStyleBackColor = True
        '
        'btnClearMatches
        '
        Me.btnClearMatches.Location = New System.Drawing.Point(645, 525)
        Me.btnClearMatches.Name = "btnClearMatches"
        Me.btnClearMatches.Size = New System.Drawing.Size(105, 29)
        Me.btnClearMatches.TabIndex = 5
        Me.btnClearMatches.Text = "Clear All"
        Me.btnClearMatches.UseVisualStyleBackColor = True
        '
        'btnImportMatched
        '
        Me.btnImportMatched.Location = New System.Drawing.Point(756, 525)
        Me.btnImportMatched.Name = "btnImportMatched"
        Me.btnImportMatched.Size = New System.Drawing.Size(105, 29)
        Me.btnImportMatched.TabIndex = 6
        Me.btnImportMatched.Text = "Import Matched"
        Me.btnImportMatched.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Location = New System.Drawing.Point(867, 525)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(105, 29)
        Me.btnCancel.TabIndex = 7
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'frmActualsMatcher
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(984, 661)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.btnImportMatched)
        Me.Controls.Add(Me.btnClearMatches)
        Me.Controls.Add(Me.btnAutoMatchByName)
        Me.Controls.Add(Me.btnMatchSelected)
        Me.Controls.Add(Me.chkMatchAllSameName)
        Me.Controls.Add(Me.dgvProject)
        Me.Controls.Add(Me.dgvMitek)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog
        Me.Name = "frmActualsMatcher"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Import Actuals - Match Levls"
        CType(Me.dgvMitek, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvProject, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents dgvMitek As DataGridView
    Friend WithEvents dgvProject As DataGridView
    Friend WithEvents chkMatchAllSameName As CheckBox
    Friend WithEvents btnMatchSelected As Button
    Friend WithEvents btnAutoMatchByName As Button
    Friend WithEvents btnClearMatches As Button
    Friend WithEvents btnImportMatched As Button
    Friend WithEvents btnCancel As Button
End Class
