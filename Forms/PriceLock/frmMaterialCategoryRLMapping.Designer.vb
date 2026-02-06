<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMaterialCategoryRLMapping
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
        Me.grpCurrentMappings = New System.Windows.Forms.GroupBox()
        Me.dgvMappings = New System.Windows.Forms.DataGridView()
        Me.grpAddMapping = New System.Windows.Forms.GroupBox()
        Me.lblMaterialCategory = New System.Windows.Forms.Label()
        Me.cboMaterialCategory = New System.Windows.Forms.ComboBox()
        Me.lblRLPricingType = New System.Windows.Forms.Label()
        Me.cboRLPricingType = New System.Windows.Forms.ComboBox()
        Me.chkSetAsPrimary = New System.Windows.Forms.CheckBox()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.btnDelete = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.lblInstructions = New System.Windows.Forms.Label()
        CType(Me.dgvMappings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpCurrentMappings.SuspendLayout()
        Me.grpAddMapping.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpCurrentMappings
        '
        Me.grpCurrentMappings.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpCurrentMappings.Controls.Add(Me.dgvMappings)
        Me.grpCurrentMappings.Location = New System.Drawing.Point(12, 80)
        Me.grpCurrentMappings.Name = "grpCurrentMappings"
        Me.grpCurrentMappings.Size = New System.Drawing.Size(760, 300)
        Me.grpCurrentMappings.TabIndex = 2
        Me.grpCurrentMappings.TabStop = False
        Me.grpCurrentMappings.Text = "Current Mappings"
        '
        'dgvMappings
        '
        Me.dgvMappings.AllowUserToAddRows = False
        Me.dgvMappings.AllowUserToDeleteRows = False
        Me.dgvMappings.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvMappings.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvMappings.Location = New System.Drawing.Point(3, 16)
        Me.dgvMappings.MultiSelect = False
        Me.dgvMappings.Name = "dgvMappings"
        Me.dgvMappings.ReadOnly = True
        Me.dgvMappings.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvMappings.Size = New System.Drawing.Size(754, 281)
        Me.dgvMappings.TabIndex = 0
        '
        'grpAddMapping
        '
        Me.grpAddMapping.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.grpAddMapping.Controls.Add(Me.lblMaterialCategory)
        Me.grpAddMapping.Controls.Add(Me.cboMaterialCategory)
        Me.grpAddMapping.Controls.Add(Me.lblRLPricingType)
        Me.grpAddMapping.Controls.Add(Me.cboRLPricingType)
        Me.grpAddMapping.Controls.Add(Me.chkSetAsPrimary)
        Me.grpAddMapping.Controls.Add(Me.btnAdd)
        Me.grpAddMapping.Location = New System.Drawing.Point(12, 386)
        Me.grpAddMapping.Name = "grpAddMapping"
        Me.grpAddMapping.Size = New System.Drawing.Size(760, 110)
        Me.grpAddMapping.TabIndex = 3
        Me.grpAddMapping.TabStop = False
        Me.grpAddMapping.Text = "Add New Mapping"
        '
        'lblMaterialCategory
        '
        Me.lblMaterialCategory.AutoSize = True
        Me.lblMaterialCategory.Location = New System.Drawing.Point(15, 28)
        Me.lblMaterialCategory.Name = "lblMaterialCategory"
        Me.lblMaterialCategory.Size = New System.Drawing.Size(94, 13)
        Me.lblMaterialCategory.TabIndex = 0
        Me.lblMaterialCategory.Text = "Material Category:"
        '
        'cboMaterialCategory
        '
        Me.cboMaterialCategory.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboMaterialCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboMaterialCategory.FormattingEnabled = True
        Me.cboMaterialCategory.Location = New System.Drawing.Point(120, 25)
        Me.cboMaterialCategory.Name = "cboMaterialCategory"
        Me.cboMaterialCategory.Size = New System.Drawing.Size(620, 21)
        Me.cboMaterialCategory.TabIndex = 1
        '
        'lblRLPricingType
        '
        Me.lblRLPricingType.AutoSize = True
        Me.lblRLPricingType.Location = New System.Drawing.Point(15, 55)
        Me.lblRLPricingType.Name = "lblRLPricingType"
        Me.lblRLPricingType.Size = New System.Drawing.Size(91, 13)
        Me.lblRLPricingType.TabIndex = 2
        Me.lblRLPricingType.Text = "RL Pricing Type:"
        '
        'cboRLPricingType
        '
        Me.cboRLPricingType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cboRLPricingType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboRLPricingType.FormattingEnabled = True
        Me.cboRLPricingType.Location = New System.Drawing.Point(120, 52)
        Me.cboRLPricingType.Name = "cboRLPricingType"
        Me.cboRLPricingType.Size = New System.Drawing.Size(620, 21)
        Me.cboRLPricingType.TabIndex = 3
        '
        'chkSetAsPrimary
        '
        Me.chkSetAsPrimary.AutoSize = True
        Me.chkSetAsPrimary.Checked = True
        Me.chkSetAsPrimary.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkSetAsPrimary.Location = New System.Drawing.Point(120, 79)
        Me.chkSetAsPrimary.Name = "chkSetAsPrimary"
        Me.chkSetAsPrimary.Size = New System.Drawing.Size(213, 17)
        Me.chkSetAsPrimary.TabIndex = 4
        Me.chkSetAsPrimary.Text = "Set as Primary (used for calculations)"
        Me.chkSetAsPrimary.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnAdd.Location = New System.Drawing.Point(665, 75)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(75, 23)
        Me.btnAdd.TabIndex = 5
        Me.btnAdd.Text = "Add"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'btnDelete
        '
        Me.btnDelete.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnDelete.Location = New System.Drawing.Point(616, 502)
        Me.btnDelete.Name = "btnDelete"
        Me.btnDelete.Size = New System.Drawing.Size(75, 23)
        Me.btnDelete.TabIndex = 4
        Me.btnDelete.Text = "Delete"
        Me.btnDelete.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(697, 502)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(75, 23)
        Me.btnClose.TabIndex = 5
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'lblInstructions
        '
        Me.lblInstructions.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblInstructions.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblInstructions.ForeColor = System.Drawing.SystemColors.ControlDarkDark
        Me.lblInstructions.Location = New System.Drawing.Point(12, 9)
        Me.lblInstructions.Name = "lblInstructions"
        Me.lblInstructions.Size = New System.Drawing.Size(760, 60)
        Me.lblInstructions.TabIndex = 1
        Me.lblInstructions.Text = "This form links Material Categories (STUDS, OSB, etc.) to Random Lengths pricing" &
    " types (e.g., ""2x4 SPF Studs - Denver"")." & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "Each Material Category should ha" &
    "ve ONE primary RL pricing type. This primary mapping is used to calculate perce" &
    "nt changes in material costs based on Random Lengths report data."
        '
        'frmMaterialCategoryRLMapping
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(784, 537)
        Me.Controls.Add(Me.lblInstructions)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.btnDelete)
        Me.Controls.Add(Me.grpAddMapping)
        Me.Controls.Add(Me.grpCurrentMappings)
        Me.MinimumSize = New System.Drawing.Size(800, 576)
        Me.Name = "frmMaterialCategoryRLMapping"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Material Category → Random Lengths Mapping"
        CType(Me.dgvMappings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpCurrentMappings.ResumeLayout(False)
        Me.grpAddMapping.ResumeLayout(False)
        Me.grpAddMapping.PerformLayout()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents grpCurrentMappings As GroupBox
    Friend WithEvents dgvMappings As DataGridView
    Friend WithEvents grpAddMapping As GroupBox
    Friend WithEvents lblMaterialCategory As Label
    Friend WithEvents cboMaterialCategory As ComboBox
    Friend WithEvents lblRLPricingType As Label
    Friend WithEvents cboRLPricingType As ComboBox
    Friend WithEvents chkSetAsPrimary As CheckBox
    Friend WithEvents btnAdd As Button
    Friend WithEvents btnDelete As Button
    Friend WithEvents btnClose As Button
    Friend WithEvents lblInstructions As Label
End Class
