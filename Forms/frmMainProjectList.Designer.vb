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
        Me.components = New System.ComponentModel.Container()
        Me.DataGridViewProjects = New System.Windows.Forms.DataGridView()
        Me.ProjectListBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.ProjectSummaryDataSet = New BuildersPSE2.ProjectSummaryDataSet()
        Me.txtSearch = New System.Windows.Forms.TextBox()
        Me.btnRefreshGrid = New System.Windows.Forms.Button()
        Me.ProjectListTableAdapter = New BuildersPSE2.ProjectSummaryDataSetTableAdapters.ProjectListTableAdapter()
        Me.JBID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ProjectName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Address = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.biddate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CreatedDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.estimatorname = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.CustomerName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SalesName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.VersionID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.ProjectID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.VersionName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.VersionDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.Description = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.LastModifiedDate = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MondayID = New System.Windows.Forms.DataGridViewTextBoxColumn()
        CType(Me.DataGridViewProjects, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ProjectListBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.ProjectSummaryDataSet, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridViewProjects
        '
        Me.DataGridViewProjects.AllowUserToAddRows = False
        Me.DataGridViewProjects.AllowUserToDeleteRows = False
        Me.DataGridViewProjects.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridViewProjects.AutoGenerateColumns = False
        Me.DataGridViewProjects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.DataGridViewProjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridViewProjects.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.JBID, Me.ProjectName, Me.Address, Me.biddate, Me.CreatedDate, Me.estimatorname, Me.CustomerName, Me.SalesName, Me.VersionID, Me.ProjectID, Me.VersionName, Me.VersionDate, Me.Description, Me.LastModifiedDate, Me.MondayID})
        Me.DataGridViewProjects.DataSource = Me.ProjectListBindingSource
        Me.DataGridViewProjects.Location = New System.Drawing.Point(12, 40)
        Me.DataGridViewProjects.Name = "DataGridViewProjects"
        Me.DataGridViewProjects.ReadOnly = True
        Me.DataGridViewProjects.Size = New System.Drawing.Size(1021, 359)
        Me.DataGridViewProjects.TabIndex = 0
        '
        'ProjectListBindingSource
        '
        Me.ProjectListBindingSource.DataMember = "ProjectList"
        Me.ProjectListBindingSource.DataSource = Me.ProjectSummaryDataSet
        '
        'ProjectSummaryDataSet
        '
        Me.ProjectSummaryDataSet.DataSetName = "ProjectSummaryDataSet"
        Me.ProjectSummaryDataSet.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema
        '
        'txtSearch
        '
        Me.txtSearch.Location = New System.Drawing.Point(17, 8)
        Me.txtSearch.Name = "txtSearch"
        Me.txtSearch.Size = New System.Drawing.Size(300, 20)
        Me.txtSearch.TabIndex = 1
        '
        'btnRefreshGrid
        '
        Me.btnRefreshGrid.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnRefreshGrid.Location = New System.Drawing.Point(934, 405)
        Me.btnRefreshGrid.Name = "btnRefreshGrid"
        Me.btnRefreshGrid.Size = New System.Drawing.Size(99, 27)
        Me.btnRefreshGrid.TabIndex = 5
        Me.btnRefreshGrid.Text = "Refresh Projects"
        Me.btnRefreshGrid.UseVisualStyleBackColor = True
        '
        'ProjectListTableAdapter
        '
        Me.ProjectListTableAdapter.ClearBeforeFill = True
        '
        'JBID
        '
        Me.JBID.DataPropertyName = "JBID"
        Me.JBID.HeaderText = "Project Nbr"
        Me.JBID.Name = "JBID"
        Me.JBID.ReadOnly = True
        '
        'ProjectName
        '
        Me.ProjectName.DataPropertyName = "ProjectName"
        Me.ProjectName.HeaderText = "ProjectName"
        Me.ProjectName.Name = "ProjectName"
        Me.ProjectName.ReadOnly = True
        '
        'Address
        '
        Me.Address.DataPropertyName = "Address"
        Me.Address.HeaderText = "Address"
        Me.Address.Name = "Address"
        Me.Address.ReadOnly = True
        '
        'biddate
        '
        Me.biddate.DataPropertyName = "biddate"
        Me.biddate.HeaderText = "Bid Date"
        Me.biddate.Name = "biddate"
        Me.biddate.ReadOnly = True
        '
        'CreatedDate
        '
        Me.CreatedDate.DataPropertyName = "CreatedDate"
        Me.CreatedDate.HeaderText = "CreatedDate"
        Me.CreatedDate.Name = "CreatedDate"
        Me.CreatedDate.ReadOnly = True
        Me.CreatedDate.Visible = False
        '
        'estimatorname
        '
        Me.estimatorname.DataPropertyName = "estimatorname"
        Me.estimatorname.HeaderText = "Estimator"
        Me.estimatorname.Name = "estimatorname"
        Me.estimatorname.ReadOnly = True
        '
        'CustomerName
        '
        Me.CustomerName.DataPropertyName = "CustomerName"
        Me.CustomerName.HeaderText = "CustomerName"
        Me.CustomerName.Name = "CustomerName"
        Me.CustomerName.ReadOnly = True
        '
        'SalesName
        '
        Me.SalesName.DataPropertyName = "SalesName"
        Me.SalesName.HeaderText = "SalesName"
        Me.SalesName.Name = "SalesName"
        Me.SalesName.ReadOnly = True
        '
        'VersionID
        '
        Me.VersionID.DataPropertyName = "VersionID"
        Me.VersionID.HeaderText = "VersionID"
        Me.VersionID.Name = "VersionID"
        Me.VersionID.ReadOnly = True
        Me.VersionID.Visible = False
        '
        'ProjectID
        '
        Me.ProjectID.DataPropertyName = "ProjectID"
        Me.ProjectID.HeaderText = "ProjectID"
        Me.ProjectID.Name = "ProjectID"
        Me.ProjectID.ReadOnly = True
        Me.ProjectID.Visible = False
        '
        'VersionName
        '
        Me.VersionName.DataPropertyName = "VersionName"
        Me.VersionName.HeaderText = "VersionName"
        Me.VersionName.Name = "VersionName"
        Me.VersionName.ReadOnly = True
        '
        'VersionDate
        '
        Me.VersionDate.DataPropertyName = "VersionDate"
        Me.VersionDate.HeaderText = "VersionDate"
        Me.VersionDate.Name = "VersionDate"
        Me.VersionDate.ReadOnly = True
        Me.VersionDate.Visible = False
        '
        'Description
        '
        Me.Description.DataPropertyName = "Description"
        Me.Description.HeaderText = "Description"
        Me.Description.Name = "Description"
        Me.Description.ReadOnly = True
        Me.Description.Visible = False
        '
        'LastModifiedDate
        '
        Me.LastModifiedDate.DataPropertyName = "LastModifiedDate"
        Me.LastModifiedDate.HeaderText = "LastModifiedDate"
        Me.LastModifiedDate.Name = "LastModifiedDate"
        Me.LastModifiedDate.ReadOnly = True
        Me.LastModifiedDate.Visible = False
        '
        'MondayID
        '
        Me.MondayID.DataPropertyName = "MondayID"
        Me.MondayID.HeaderText = "MondayID"
        Me.MondayID.Name = "MondayID"
        Me.MondayID.ReadOnly = True
        Me.MondayID.Visible = False
        '
        'frmMainProjectList
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1045, 461)
        Me.Controls.Add(Me.btnRefreshGrid)
        Me.Controls.Add(Me.txtSearch)
        Me.Controls.Add(Me.DataGridViewProjects)
        Me.Name = "frmMainProjectList"
        Me.Text = "Project List"
        CType(Me.DataGridViewProjects, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ProjectListBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.ProjectSummaryDataSet, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents DataGridViewProjects As DataGridView
    Friend WithEvents txtSearch As TextBox
    Friend WithEvents btnRefreshGrid As Button
    Friend WithEvents ProjectSummaryDataSet As ProjectSummaryDataSet
    Friend WithEvents ProjectListBindingSource As BindingSource
    Friend WithEvents ProjectListTableAdapter As ProjectSummaryDataSetTableAdapters.ProjectListTableAdapter
    Friend WithEvents JBID As DataGridViewTextBoxColumn
    Friend WithEvents ProjectName As DataGridViewTextBoxColumn
    Friend WithEvents Address As DataGridViewTextBoxColumn
    Friend WithEvents biddate As DataGridViewTextBoxColumn
    Friend WithEvents CreatedDate As DataGridViewTextBoxColumn
    Friend WithEvents estimatorname As DataGridViewTextBoxColumn
    Friend WithEvents CustomerName As DataGridViewTextBoxColumn
    Friend WithEvents SalesName As DataGridViewTextBoxColumn
    Friend WithEvents VersionID As DataGridViewTextBoxColumn
    Friend WithEvents ProjectID As DataGridViewTextBoxColumn
    Friend WithEvents VersionName As DataGridViewTextBoxColumn
    Friend WithEvents VersionDate As DataGridViewTextBoxColumn
    Friend WithEvents Description As DataGridViewTextBoxColumn
    Friend WithEvents LastModifiedDate As DataGridViewTextBoxColumn
    Friend WithEvents MondayID As DataGridViewTextBoxColumn
End Class
