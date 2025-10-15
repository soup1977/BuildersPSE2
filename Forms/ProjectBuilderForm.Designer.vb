<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ProjectBuilderForm
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
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.colBuildingName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colBldgQty = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colResUnits = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colNumFloors = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colNumRoofs = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.colNumWalls = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BtnSaveProjectBuilder = New System.Windows.Forms.Button()
        Me.btnClose = New System.Windows.Forms.Button()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridView1
        '
        Me.DataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.colBuildingName, Me.colBldgQty, Me.colResUnits, Me.colNumFloors, Me.colNumRoofs, Me.colNumWalls})
        Me.DataGridView1.Dock = System.Windows.Forms.DockStyle.Top
        Me.DataGridView1.Location = New System.Drawing.Point(0, 0)
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.Size = New System.Drawing.Size(800, 270)
        Me.DataGridView1.TabIndex = 0
        '
        'colBuildingName
        '
        Me.colBuildingName.HeaderText = "Building Name"
        Me.colBuildingName.Name = "colBuildingName"
        '
        'colBldgQty
        '
        Me.colBldgQty.HeaderText = "Building Qty"
        Me.colBldgQty.Name = "colBldgQty"
        '
        'colResUnits
        '
        Me.colResUnits.HeaderText = "Building Plan Units"
        Me.colResUnits.Name = "colResUnits"
        '
        'colNumFloors
        '
        Me.colNumFloors.HeaderText = "Number of Floor Levels"
        Me.colNumFloors.Name = "colNumFloors"
        '
        'colNumRoofs
        '
        Me.colNumRoofs.HeaderText = "Number of Roof Levels"
        Me.colNumRoofs.Name = "colNumRoofs"
        '
        'colNumWalls
        '
        Me.colNumWalls.HeaderText = "Number of Wall Levels"
        Me.colNumWalls.Name = "colNumWalls"
        '
        'BtnSaveProjectBuilder
        '
        Me.BtnSaveProjectBuilder.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.BtnSaveProjectBuilder.Location = New System.Drawing.Point(570, 379)
        Me.BtnSaveProjectBuilder.Name = "BtnSaveProjectBuilder"
        Me.BtnSaveProjectBuilder.Size = New System.Drawing.Size(106, 31)
        Me.BtnSaveProjectBuilder.TabIndex = 1
        Me.BtnSaveProjectBuilder.Text = "Apply"
        Me.BtnSaveProjectBuilder.UseVisualStyleBackColor = True
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(682, 379)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(106, 31)
        Me.btnClose.TabIndex = 2
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'ProjectBuilderForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.btnClose)
        Me.Controls.Add(Me.BtnSaveProjectBuilder)
        Me.Controls.Add(Me.DataGridView1)
        Me.Name = "ProjectBuilderForm"
        Me.Text = "ProjectBuilderForm"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents colBuildingName As DataGridViewTextBoxColumn
    Friend WithEvents colBldgQty As DataGridViewTextBoxColumn
    Friend WithEvents colResUnits As DataGridViewTextBoxColumn
    Friend WithEvents colNumFloors As DataGridViewTextBoxColumn
    Friend WithEvents colNumRoofs As DataGridViewTextBoxColumn
    Friend WithEvents colNumWalls As DataGridViewTextBoxColumn
    Friend WithEvents BtnSaveProjectBuilder As Button
    Friend WithEvents btnClose As Button
End Class
