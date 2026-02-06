<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmLumberManagement
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
        Me.lstLumberType = New System.Windows.Forms.ListBox()
        Me.lstCostEffective = New System.Windows.Forms.ListBox()
        Me.btnAddLumber = New System.Windows.Forms.Button()
        Me.btnAddCostEffective = New System.Windows.Forms.Button()
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.btnSaveLumberCosts = New System.Windows.Forms.Button()
        Me.btnExitForm = New System.Windows.Forms.Button()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'lstLumberType
        '
        Me.lstLumberType.FormattingEnabled = True
        Me.lstLumberType.Location = New System.Drawing.Point(58, 18)
        Me.lstLumberType.Name = "lstLumberType"
        Me.lstLumberType.Size = New System.Drawing.Size(163, 173)
        Me.lstLumberType.TabIndex = 0
        '
        'lstCostEffective
        '
        Me.lstCostEffective.FormattingEnabled = True
        Me.lstCostEffective.Location = New System.Drawing.Point(227, 18)
        Me.lstCostEffective.Name = "lstCostEffective"
        Me.lstCostEffective.Size = New System.Drawing.Size(108, 173)
        Me.lstCostEffective.TabIndex = 1
        '
        'btnAddLumber
        '
        Me.btnAddLumber.Location = New System.Drawing.Point(61, 197)
        Me.btnAddLumber.Name = "btnAddLumber"
        Me.btnAddLumber.Size = New System.Drawing.Size(52, 27)
        Me.btnAddLumber.TabIndex = 2
        Me.btnAddLumber.Text = "Add"
        Me.btnAddLumber.UseVisualStyleBackColor = True
        '
        'btnAddCostEffective
        '
        Me.btnAddCostEffective.Location = New System.Drawing.Point(227, 197)
        Me.btnAddCostEffective.Name = "btnAddCostEffective"
        Me.btnAddCostEffective.Size = New System.Drawing.Size(52, 27)
        Me.btnAddCostEffective.TabIndex = 3
        Me.btnAddCostEffective.Text = "Add"
        Me.btnAddCostEffective.UseVisualStyleBackColor = True
        '
        'DataGridView1
        '
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Location = New System.Drawing.Point(58, 230)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(709, 211)
        Me.DataGridView1.TabIndex = 4
        '
        'btnSaveLumberCosts
        '
        Me.btnSaveLumberCosts.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSaveLumberCosts.Location = New System.Drawing.Point(595, 454)
        Me.btnSaveLumberCosts.Name = "btnSaveLumberCosts"
        Me.btnSaveLumberCosts.Size = New System.Drawing.Size(74, 29)
        Me.btnSaveLumberCosts.TabIndex = 5
        Me.btnSaveLumberCosts.Text = "Save Costs"
        Me.btnSaveLumberCosts.UseVisualStyleBackColor = True
        '
        'btnExitForm
        '
        Me.btnExitForm.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnExitForm.Location = New System.Drawing.Point(684, 454)
        Me.btnExitForm.Name = "btnExitForm"
        Me.btnExitForm.Size = New System.Drawing.Size(65, 28)
        Me.btnExitForm.TabIndex = 6
        Me.btnExitForm.Text = "Exit"
        Me.btnExitForm.UseVisualStyleBackColor = True
        '
        'frmLumberManagement
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 547)
        Me.Controls.Add(Me.btnExitForm)
        Me.Controls.Add(Me.btnSaveLumberCosts)
        Me.Controls.Add(Me.DataGridView1)
        Me.Controls.Add(Me.btnAddCostEffective)
        Me.Controls.Add(Me.btnAddLumber)
        Me.Controls.Add(Me.lstCostEffective)
        Me.Controls.Add(Me.lstLumberType)
        Me.Name = "frmLumberManagement"
        Me.Text = "Lumber Management"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents lstLumberType As ListBox
    Friend WithEvents lstCostEffective As ListBox
    Friend WithEvents btnAddLumber As Button
    Friend WithEvents btnAddCostEffective As Button
    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents btnSaveLumberCosts As Button
    Friend WithEvents btnExitForm As Button
End Class
