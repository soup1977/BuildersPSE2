<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmPlanManagement
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
        Me.pnlPlans = New System.Windows.Forms.Panel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.btnUpdatePlan = New System.Windows.Forms.Button()
        Me.btnAddPlan = New System.Windows.Forms.Button()
        Me.chkPlanIsActive = New System.Windows.Forms.CheckBox()
        Me.nudSquareFootage = New System.Windows.Forms.NumericUpDown()
        Me.txtPlanDescription = New System.Windows.Forms.TextBox()
        Me.txtPlanName = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.dgvPlans = New System.Windows.Forms.DataGridView()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.pnlElevations = New System.Windows.Forms.Panel()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.btnUpdateElevation = New System.Windows.Forms.Button()
        Me.btnAddElevation = New System.Windows.Forms.Button()
        Me.chkElevationIsActive = New System.Windows.Forms.CheckBox()
        Me.txtElevationName = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.dgvElevations = New System.Windows.Forms.DataGridView()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.pnlBottom = New System.Windows.Forms.Panel()
        Me.btnClose = New System.Windows.Forms.Button()
        Me.lblStatus = New System.Windows.Forms.Label()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.pnlPlans.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        CType(Me.nudSquareFootage, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.dgvPlans, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlElevations.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        CType(Me.dgvElevations, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlBottom.SuspendLayout()
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
        Me.SplitContainer1.Panel1.Controls.Add(Me.pnlPlans)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.pnlElevations)
        Me.SplitContainer1.Size = New System.Drawing.Size(900, 590)
        Me.SplitContainer1.SplitterDistance = 445
        Me.SplitContainer1.TabIndex = 0
        '
        'pnlPlans
        '
        Me.pnlPlans.Controls.Add(Me.GroupBox1)
        Me.pnlPlans.Controls.Add(Me.dgvPlans)
        Me.pnlPlans.Controls.Add(Me.Label1)
        Me.pnlPlans.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlPlans.Location = New System.Drawing.Point(0, 0)
        Me.pnlPlans.Name = "pnlPlans"
        Me.pnlPlans.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlPlans.Size = New System.Drawing.Size(445, 590)
        Me.pnlPlans.TabIndex = 0
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.btnUpdatePlan)
        Me.GroupBox1.Controls.Add(Me.btnAddPlan)
        Me.GroupBox1.Controls.Add(Me.chkPlanIsActive)
        Me.GroupBox1.Controls.Add(Me.nudSquareFootage)
        Me.GroupBox1.Controls.Add(Me.txtPlanDescription)
        Me.GroupBox1.Controls.Add(Me.txtPlanName)
        Me.GroupBox1.Controls.Add(Me.Label4)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.Label2)
        Me.GroupBox1.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.GroupBox1.Location = New System.Drawing.Point(10, 390)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(425, 190)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Plan Details"
        '
        'btnUpdatePlan
        '
        Me.btnUpdatePlan.Location = New System.Drawing.Point(120, 150)
        Me.btnUpdatePlan.Name = "btnUpdatePlan"
        Me.btnUpdatePlan.Size = New System.Drawing.Size(100, 28)
        Me.btnUpdatePlan.TabIndex = 8
        Me.btnUpdatePlan.Text = "Update Plan"
        Me.btnUpdatePlan.UseVisualStyleBackColor = True
        '
        'btnAddPlan
        '
        Me.btnAddPlan.Location = New System.Drawing.Point(15, 150)
        Me.btnAddPlan.Name = "btnAddPlan"
        Me.btnAddPlan.Size = New System.Drawing.Size(90, 28)
        Me.btnAddPlan.TabIndex = 7
        Me.btnAddPlan.Text = "Add Plan"
        Me.btnAddPlan.UseVisualStyleBackColor = True
        '
        'chkPlanIsActive
        '
        Me.chkPlanIsActive.AutoSize = True
        Me.chkPlanIsActive.Checked = True
        Me.chkPlanIsActive.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkPlanIsActive.Location = New System.Drawing.Point(105, 120)
        Me.chkPlanIsActive.Name = "chkPlanIsActive"
        Me.chkPlanIsActive.Size = New System.Drawing.Size(56, 17)
        Me.chkPlanIsActive.TabIndex = 6
        Me.chkPlanIsActive.Text = "Active"
        Me.chkPlanIsActive.UseVisualStyleBackColor = True
        '
        'nudSquareFootage
        '
        Me.nudSquareFootage.Location = New System.Drawing.Point(105, 90)
        Me.nudSquareFootage.Maximum = New Decimal(New Integer() {99999, 0, 0, 0})
        Me.nudSquareFootage.Name = "nudSquareFootage"
        Me.nudSquareFootage.Size = New System.Drawing.Size(100, 20)
        Me.nudSquareFootage.TabIndex = 5
        '
        'txtPlanDescription
        '
        Me.txtPlanDescription.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPlanDescription.Location = New System.Drawing.Point(105, 57)
        Me.txtPlanDescription.Name = "txtPlanDescription"
        Me.txtPlanDescription.Size = New System.Drawing.Size(305, 20)
        Me.txtPlanDescription.TabIndex = 4
        '
        'txtPlanName
        '
        Me.txtPlanName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtPlanName.Location = New System.Drawing.Point(105, 25)
        Me.txtPlanName.Name = "txtPlanName"
        Me.txtPlanName.Size = New System.Drawing.Size(305, 20)
        Me.txtPlanName.TabIndex = 3
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(15, 92)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(35, 13)
        Me.Label4.TabIndex = 2
        Me.Label4.Text = "Sq Ft:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(15, 60)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(63, 13)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Description:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(15, 28)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(62, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Plan Name:"
        '
        'dgvPlans
        '
        Me.dgvPlans.AllowUserToAddRows = False
        Me.dgvPlans.AllowUserToDeleteRows = False
        Me.dgvPlans.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvPlans.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvPlans.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvPlans.Location = New System.Drawing.Point(10, 35)
        Me.dgvPlans.MultiSelect = False
        Me.dgvPlans.Name = "dgvPlans"
        Me.dgvPlans.ReadOnly = True
        Me.dgvPlans.RowHeadersVisible = False
        Me.dgvPlans.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvPlans.Size = New System.Drawing.Size(425, 340)
        Me.dgvPlans.TabIndex = 1
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(10, 10)
        Me.Label1.Name = "Label1"
        Me.Label1.Padding = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.Label1.Size = New System.Drawing.Size(38, 23)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Plans"
        '
        'pnlElevations
        '
        Me.pnlElevations.Controls.Add(Me.GroupBox2)
        Me.pnlElevations.Controls.Add(Me.dgvElevations)
        Me.pnlElevations.Controls.Add(Me.Label5)
        Me.pnlElevations.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlElevations.Location = New System.Drawing.Point(0, 0)
        Me.pnlElevations.Name = "pnlElevations"
        Me.pnlElevations.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlElevations.Size = New System.Drawing.Size(451, 590)
        Me.pnlElevations.TabIndex = 0
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.btnUpdateElevation)
        Me.GroupBox2.Controls.Add(Me.btnAddElevation)
        Me.GroupBox2.Controls.Add(Me.chkElevationIsActive)
        Me.GroupBox2.Controls.Add(Me.txtElevationName)
        Me.GroupBox2.Controls.Add(Me.Label6)
        Me.GroupBox2.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.GroupBox2.Location = New System.Drawing.Point(10, 390)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(431, 190)
        Me.GroupBox2.TabIndex = 2
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Elevation Details"
        '
        'btnUpdateElevation
        '
        Me.btnUpdateElevation.Location = New System.Drawing.Point(125, 110)
        Me.btnUpdateElevation.Name = "btnUpdateElevation"
        Me.btnUpdateElevation.Size = New System.Drawing.Size(100, 28)
        Me.btnUpdateElevation.TabIndex = 4
        Me.btnUpdateElevation.Text = "Update Elev"
        Me.btnUpdateElevation.UseVisualStyleBackColor = True
        '
        'btnAddElevation
        '
        Me.btnAddElevation.Location = New System.Drawing.Point(15, 110)
        Me.btnAddElevation.Name = "btnAddElevation"
        Me.btnAddElevation.Size = New System.Drawing.Size(100, 28)
        Me.btnAddElevation.TabIndex = 3
        Me.btnAddElevation.Text = "Add Elevation"
        Me.btnAddElevation.UseVisualStyleBackColor = True
        '
        'chkElevationIsActive
        '
        Me.chkElevationIsActive.AutoSize = True
        Me.chkElevationIsActive.Checked = True
        Me.chkElevationIsActive.CheckState = System.Windows.Forms.CheckState.Checked
        Me.chkElevationIsActive.Location = New System.Drawing.Point(105, 60)
        Me.chkElevationIsActive.Name = "chkElevationIsActive"
        Me.chkElevationIsActive.Size = New System.Drawing.Size(56, 17)
        Me.chkElevationIsActive.TabIndex = 2
        Me.chkElevationIsActive.Text = "Active"
        Me.chkElevationIsActive.UseVisualStyleBackColor = True
        '
        'txtElevationName
        '
        Me.txtElevationName.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.txtElevationName.Location = New System.Drawing.Point(105, 28)
        Me.txtElevationName.Name = "txtElevationName"
        Me.txtElevationName.Size = New System.Drawing.Size(305, 20)
        Me.txtElevationName.TabIndex = 1
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(15, 31)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(54, 13)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Elevation:"
        '
        'dgvElevations
        '
        Me.dgvElevations.AllowUserToAddRows = False
        Me.dgvElevations.AllowUserToDeleteRows = False
        Me.dgvElevations.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgvElevations.BackgroundColor = System.Drawing.SystemColors.Window
        Me.dgvElevations.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvElevations.Location = New System.Drawing.Point(10, 35)
        Me.dgvElevations.MultiSelect = False
        Me.dgvElevations.Name = "dgvElevations"
        Me.dgvElevations.ReadOnly = True
        Me.dgvElevations.RowHeadersVisible = False
        Me.dgvElevations.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvElevations.Size = New System.Drawing.Size(431, 340)
        Me.dgvElevations.TabIndex = 1
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Dock = System.Windows.Forms.DockStyle.Top
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(10, 10)
        Me.Label5.Name = "Label5"
        Me.Label5.Padding = New System.Windows.Forms.Padding(0, 5, 0, 5)
        Me.Label5.Size = New System.Drawing.Size(173, 23)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "Elevations (for selected plan)"
        '
        'pnlBottom
        '
        Me.pnlBottom.Controls.Add(Me.btnClose)
        Me.pnlBottom.Controls.Add(Me.lblStatus)
        Me.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnlBottom.Location = New System.Drawing.Point(0, 590)
        Me.pnlBottom.Name = "pnlBottom"
        Me.pnlBottom.Padding = New System.Windows.Forms.Padding(10)
        Me.pnlBottom.Size = New System.Drawing.Size(900, 60)
        Me.pnlBottom.TabIndex = 1
        '
        'btnClose
        '
        Me.btnClose.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnClose.Location = New System.Drawing.Point(805, 15)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New System.Drawing.Size(80, 28)
        Me.btnClose.TabIndex = 1
        Me.btnClose.Text = "Close"
        Me.btnClose.UseVisualStyleBackColor = True
        '
        'lblStatus
        '
        Me.lblStatus.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lblStatus.ForeColor = System.Drawing.Color.Green
        Me.lblStatus.Location = New System.Drawing.Point(10, 20)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(780, 20)
        Me.lblStatus.TabIndex = 0
        '
        'frmPlanManagement
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(900, 650)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.pnlBottom)
        Me.MinimumSize = New System.Drawing.Size(800, 550)
        Me.Name = "frmPlanManagement"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Plan && Elevation Management"
        Me.SplitContainer1.Panel1.ResumeLayout(False)
        Me.SplitContainer1.Panel2.ResumeLayout(False)
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.SplitContainer1.ResumeLayout(False)
        Me.pnlPlans.ResumeLayout(False)
        Me.pnlPlans.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        CType(Me.nudSquareFootage, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.dgvPlans, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlElevations.ResumeLayout(False)
        Me.pnlElevations.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        CType(Me.dgvElevations, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlBottom.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents SplitContainer1 As SplitContainer
    Friend WithEvents pnlPlans As Panel
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents btnUpdatePlan As Button
    Friend WithEvents btnAddPlan As Button
    Friend WithEvents chkPlanIsActive As CheckBox
    Friend WithEvents nudSquareFootage As NumericUpDown
    Friend WithEvents txtPlanDescription As TextBox
    Friend WithEvents txtPlanName As TextBox
    Friend WithEvents Label4 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents dgvPlans As DataGridView
    Friend WithEvents Label1 As Label
    Friend WithEvents pnlElevations As Panel
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents btnUpdateElevation As Button
    Friend WithEvents btnAddElevation As Button
    Friend WithEvents chkElevationIsActive As CheckBox
    Friend WithEvents txtElevationName As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents dgvElevations As DataGridView
    Friend WithEvents Label5 As Label
    Friend WithEvents pnlBottom As Panel
    Friend WithEvents btnClose As Button
    Friend WithEvents lblStatus As Label
End Class