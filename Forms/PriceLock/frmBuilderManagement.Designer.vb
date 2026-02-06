<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmBuilderManagement
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
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.dgvBuilders = New System.Windows.Forms.DataGridView()
        Me.BuilderID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BuilderName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.BuilderCode = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.IsActive = New System.Windows.Forms.DataGridViewCheckBoxColumn()
        Me.pnlEdit = New System.Windows.Forms.Panel()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.btnManageSubdivisions = New System.Windows.Forms.Button()
        Me.btnUpdate = New System.Windows.Forms.Button()
        Me.btnAdd = New System.Windows.Forms.Button()
        Me.chkIsActive = New System.Windows.Forms.CheckBox()
        Me.txtBuilderCode = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtBuilderName = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.pnlButtons = New System.Windows.Forms.Panel()
        Me.btnClose = New System.Windows.Forms.Button()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        CType(Me.dgvBuilders, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlEdit.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 0)
        Me.SplitContainer1.Name = "SplitContainer1"
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.dgvBuilders)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlEdit)
        Me.SplitContainer1.Size = New System.Drawing.Size(700, 410)
        Me.SplitContainer1.SplitterDistance = 400
        Me.SplitContainer1.TabIndex = 0
        '
        'dgvBuilders
        '
        Me.dgvBuilders.AllowUserToAddRows = False
        Me.dgvBuilders.AllowUserToDeleteRows = False
        Me.dgvBuilders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvBuilders.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvBuilders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvBuilders.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.BuilderID, Me.BuilderName, Me.BuilderCode, Me.IsActive})
        Me.dgvBuilders.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvBuilders.Location = New System.Drawing.Point(0, 0)
        Me.dgvBuilders.MultiSelect = False
        Me.dgvBuilders.Name = "dgvBuilders"
        Me.dgvBuilders.ReadOnly = True
        Me.dgvBuilders.RowHeadersVisible = False
        Me.dgvBuilders.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvBuilders.Size = New System.Drawing.Size(400, 410)
        Me.dgvBuilders.TabIndex = 0
        '
        'BuilderID
        '
        Me.BuilderID.DataPropertyName = "BuilderID"
        Me.BuilderID.HeaderText = "BuilderID"
        Me.BuilderID.Name = "BuilderID"
        Me.BuilderID.ReadOnly = True
        Me.BuilderID.Visible = False
        '
        'BuilderName
        '
        Me.BuilderName.DataPropertyName = "BuilderName"
        Me.BuilderName.HeaderText = "Builder Name"
        Me.BuilderName.Name = "BuilderName"
        Me.BuilderName.ReadOnly = True
        Me.BuilderName.Width = 200
        '
        'BuilderCode
        '
        Me.BuilderCode.DataPropertyName = "BuilderCode"
        Me.BuilderCode.HeaderText = "Code"
        Me.BuilderCode.Name = "BuilderCode"
        Me.BuilderCode.ReadOnly = True
        Me.BuilderCode.Width = 80
        '
        'IsActive
        '
        Me.IsActive.DataPropertyName = "IsActive"
        Me.IsActive.HeaderText = "Active"
        Me.IsActive.Name = "IsActive"
        Me.IsActive.ReadOnly = True
        Me.IsActive.Width = 60
        '
        'pnlEdit
        '
        Me.pnlEdit.Controls.Add(Me.lblStatus)
        Me.pnlEdit.Controls.Add(Me.btnManageSubdivisions)
        Me.pnlEdit.Controls.Add(Me.btnUpdate)
        Me.pnlEdit.Controls.Add(Me.btnAdd)
        Me.pnlEdit.Controls.Add(Me.chkIsActive)
        Me.pnlEdit.Controls.Add(Me.txtBuilderCode)
        Me.pnlEdit.Controls.Add(Me.Label3)
        Me.pnlEdit.Controls.Add(Me.txtBuilderName)
        Me.pnlEdit.Controls.Add(Me.Label2)
        Me.pnlEdit.Controls.Add(Me.Label1)
        Me.pnlEdit.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlEdit.Location = New System.Drawing.Point(0, 0)
        Me.pnlEdit.Name = "pnlEdit"
        Me.pnlEdit.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlEdit.Size = New System.Drawing.Size(296, 410)
        Me.pnlEdit.TabIndex = 0
        '
        'lblStatus
        '
        Me.lblStatus.AutoSize = True
        Me.lblStatus.ForeColor = System.Drawing.Color.Green
        Me.lblStatus.Location = New System.Drawing.Point(10, 195)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(0, 13)
        Me.lblStatus.TabIndex = 9
        '
        'btnManageSubdivisions
        '
        Me.btnManageSubdivisions.Location = New System.Drawing.Point(10, 155)
        Me.btnManageSubdivisions.Name = "btnManageSubdivisions"
        Me.btnManageSubdivisions.Size = New System.Drawing.Size(150, 23)
        Me.btnManageSubdivisions.TabIndex = 8
        Me.btnManageSubdivisions.Text = "Manage Subdivisions..."
        Me.btnManageSubdivisions.UseVisualStyleBackColor = True
        '
        'btnUpdate
        '
        Me.btnUpdate.Location = New System.Drawing.Point(100, 110)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New System.Drawing.Size(80, 23)
        Me.btnUpdate.TabIndex = 7
        Me.btnUpdate.Text = "Update"
        Me.btnUpdate.UseVisualStyleBackColor = True
        '
        'btnAdd
        '
        Me.btnAdd.Location = New System.Drawing.Point(10, 110)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New System.Drawing.Size(80, 23)
        Me.btnAdd.TabIndex = 6
        Me.btnAdd.Text = "Add New"
        Me.btnAdd.UseVisualStyleBackColor = True
        '
        'chkIsActive
        '
        Me.chkIsActive.AutoSize = True
        Me.chkIsActive.Checked = True
        Me.chkIsActive.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkIsActive.Location = New System.Drawing.Point(80, 79)
        Me.chkIsActive.Name = "chkIsActive"
        Me.chkIsActive.Size = New System.Drawing.Size(56, 17)
        Me.chkIsActive.TabIndex = 5
        Me.chkIsActive.Text = "Active"
        Me.chkIsActive.UseVisualStyleBackColor = True
        '
        'txtBuilderCode
        '
        Me.txtBuilderCode.Location = New System.Drawing.Point(80, 53)
        Me.txtBuilderCode.Name = "txtBuilderCode"
        Me.txtBuilderCode.Size = New System.Drawing.Size(80, 20)
        Me.txtBuilderCode.TabIndex = 4
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(10, 56)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(35, 13)
        Me.Label3.TabIndex = 3
        Me.Label3.Text = "Code:"
        '
        'txtBuilderName
        '
        Me.txtBuilderName.Location = New System.Drawing.Point(80, 27)
        Me.txtBuilderName.Name = "txtBuilderName"
        Me.txtBuilderName.Size = New System.Drawing.Size(170, 20)
        Me.txtBuilderName.TabIndex = 2
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(10, 30)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(38, 13)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Name:"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(10, 10)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(86, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Builder Details"
        '
        'pnlButtons
        '
        Me.pnlButtons.Controls.Add(Me.btnClose)
        Me.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlButtons.Location = New System.Drawing.Point(0, 410)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New System.Drawing.Size(700, 45)
        Me.pnlButtons.TabIndex = 1
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(610, 10)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(80, 23)
        Me.btnClose.TabIndex = 0
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'frmBuilderManagement
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(700, 455)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.pnlButtons)
        Me.MinimumSize = New System.Drawing.Size(600, 400)
        Me.Name = "frmBuilderManagement"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Builder Management"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        CType(Me.dgvBuilders, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlEdit.ResumeLayout(False)
        Me.pnlEdit.PerformLayout()
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents dgvBuilders As DataGridView
    Friend WithEvents BuilderID As DataGridViewTextBoxColumn
    Friend WithEvents BuilderName As DataGridViewTextBoxColumn
    Friend WithEvents BuilderCode As DataGridViewTextBoxColumn
    Friend WithEvents IsActive As DataGridViewCheckBoxColumn
    Friend WithEvents pnlEdit As Panel
    Friend WithEvents lblStatus As Label
    Friend WithEvents btnManageSubdivisions As Button
    Friend WithEvents btnUpdate As Button
    Friend WithEvents btnAdd As Button
    Friend WithEvents chkIsActive As CheckBox
    Friend WithEvents txtBuilderCode As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtBuilderName As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents Label1 As Label
    Friend WithEvents pnlButtons As Panel
    Friend WithEvents btnClose As Button
End Class
