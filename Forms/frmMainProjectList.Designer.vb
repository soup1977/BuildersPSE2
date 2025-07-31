<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class frmMainProjectList
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
        Me.DataGridViewProjects = New System.Windows.Forms.DataGridView()
        Me.txtSearch = New System.Windows.Forms.TextBox()
        Me.btnNewProject = New System.Windows.Forms.Button()
        Me.btnEditProject = New System.Windows.Forms.Button()
        Me.btnOpenPSE = New System.Windows.Forms.Button()
        Me.btnRefreshGrid = New System.Windows.Forms.Button()
        CType(Me.DataGridViewProjects, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridViewProjects
        '
        Me.DataGridViewProjects.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridViewProjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewProjects.Location = New System.Drawing.Point(12, 40)
        Me.DataGridViewProjects.Name = "DataGridViewProjects"
        Me.DataGridViewProjects.Size = New System.Drawing.Size(776, 337)
        Me.DataGridViewProjects.TabIndex = 0
        '
        'txtSearch
        '
        Me.txtSearch.Location = New System.Drawing.Point(17, 8)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New System.Drawing.Size(300, 20)
        Me.txtSearch.TabIndex = 1
        '
        'btnNewProject
        '
        Me.btnNewProject.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnNewProject.Location = New System.Drawing.Point(549, 417)
        Me.btnNewProject.Name = "btnNewProject"
        Me.btnNewProject.Size = New System.Drawing.Size(75, 23)
        Me.btnNewProject.TabIndex = 2
        Me.btnNewProject.Text = "New Project"
        Me.btnNewProject.UseVisualStyleBackColor = True
        '
        'btnEditProject
        '
        Me.btnEditProject.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnEditProject.Location = New System.Drawing.Point(631, 416)
        Me.btnEditProject.Name = "btnEditProject"
        Me.btnEditProject.Size = New System.Drawing.Size(75, 23)
        Me.btnEditProject.TabIndex = 3
        Me.btnEditProject.Text = "Edit Project"
        Me.btnEditProject.UseVisualStyleBackColor = True
        '
        'btnOpenPSE
        '
        Me.btnOpenPSE.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnOpenPSE.Location = New System.Drawing.Point(713, 415)
        Me.btnOpenPSE.Name = "btnOpenPSE"
        Me.btnOpenPSE.Size = New System.Drawing.Size(75, 23)
        Me.btnOpenPSE.TabIndex = 4
        Me.btnOpenPSE.Text = "Open PSE"
        Me.btnOpenPSE.UseVisualStyleBackColor = True
        '
        'btnRefreshGrid
        '
        Me.btnRefreshGrid.Location = New System.Drawing.Point(439, 418)
        Me.btnRefreshGrid.Name = "btnRefreshGrid"
        Me.btnRefreshGrid.Size = New System.Drawing.Size(99, 21)
        Me.btnRefreshGrid.TabIndex = 5
        Me.btnRefreshGrid.Text = "Refresh Projects"
        Me.btnRefreshGrid.UseVisualStyleBackColor = True
        '
        'frmMainProjectList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.btnRefreshGrid)
        Me.Controls.Add(Me.btnOpenPSE)
        Me.Controls.Add(Me.btnEditProject)
        Me.Controls.Add(Me.btnNewProject)
        Me.Controls.Add(Me.txtSearch)
        Me.Controls.Add(Me.DataGridViewProjects)
        Me.Name = "frmMainProjectList"
        Me.Text = "Project List"
        CType(Me.DataGridViewProjects, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents DataGridViewProjects As DataGridView
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents btnNewProject As Button
    Friend WithEvents btnEditProject As Button
    Friend WithEvents btnOpenPSE As Button
    Friend WithEvents btnRefreshGrid As Button
End Class
