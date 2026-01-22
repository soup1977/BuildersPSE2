Option Strict On
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Utilities

Public Class frmMainProjectList
    Private da As New ProjectDataAccess() ' DAL instance - servant-simple dependency

    Private dvProjects As DataView

    Private ReadOnly _mainForm As frmMain = CType(Application.OpenForms.OfType(Of frmMain)().FirstOrDefault(), frmMain)

    Private Sub FrmMainProjectList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'TODO: This line of code loads data into the 'ProjectSummaryDataSet.ProjectList' table. You can move, or remove it, as needed.
        RefreshProjectList()
        UIHelper.Add("Project list loaded")
    End Sub

    Public Sub RefreshProjectList()
        Me.ProjectListTableAdapter.Fill(Me.ProjectSummaryDataSet.ProjectList)
        dvProjects = Me.ProjectSummaryDataSet.ProjectList.DefaultView
    End Sub

    Private Sub dgvProjects_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewProjects.CellContentClick
        Const LINK_COLUMN_NAME As String = "JBID"   ' the column that looks like a link

        If e.RowIndex < 0 Then Exit Sub                                 ' header click
        If DataGridViewProjects.Columns(e.ColumnIndex).Name <> LINK_COLUMN_NAME Then Exit Sub

        ' Get the ProjectID from the current row – safest way under Option Strict
        Dim projectIdObj As Object = DataGridViewProjects.Rows(e.RowIndex).Cells("ProjectID").Value

        If projectIdObj Is Nothing OrElse IsDBNull(projectIdObj) Then Exit Sub

        Dim projectId As Integer = Convert.ToInt32(projectIdObj)

        ' Call your existing method
        OpenProjectDetails(projectId)
    End Sub

    Private Sub OpenProjectDetails(projectId As Integer)
        Try
            ' Get the row directly by searching the DataSource (fast, reliable, no looping)
            Dim row As DataGridViewRow = DataGridViewProjects.Rows _
            .Cast(Of DataGridViewRow)() _
            .FirstOrDefault(Function(r) r.Cells("ProjectID").Value IsNot Nothing AndAlso
                                      Convert.ToInt32(r.Cells("ProjectID").Value) = projectId)

            If row Is Nothing OrElse row.IsNewRow Then
                UIHelper.Add($"No row found for ProjectID {projectId}")
                MessageBox.Show("Selected project not found in the grid.", "Truss Tip", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
            End If

            Dim latestVersionID As Integer = Convert.ToInt32(row.Cells("VersionID").Value)
            Dim selectedProj As ProjectModel = da.GetProjectByID(projectId)

            _mainForm.AddFormToTabControl(GetType(frmCreateEditProject), $"EditProject_{projectId}", New Object() {selectedProj, latestVersionID})
            UIHelper.Add($"Opened edit project form for ProjectID {projectId}")

        Catch ex As Exception
            UIHelper.Add($"Error opening edit project form: {ex.Message}")
            MessageBox.Show($"Error opening edit project form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub DataGridViewProjects_DoubleClick(sender As Object, e As EventArgs) Handles DataGridViewProjects.DoubleClick
        If DataGridViewProjects.CurrentRow IsNot Nothing Then
            Dim projectID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("ProjectID").Value)
            OpenProjectDetails(projectID)
        Else
            UIHelper.Add("No project selected for editing")
            MessageBox.Show("Select a project row to edit.", "Truss Tip", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        Dim searchText As String = txtSearch.Text.Trim()
        If String.IsNullOrEmpty(searchText) Then
            ProjectListBindingSource.Filter = String.Empty
        Else
            searchText = searchText.Replace("'", "''") ' Escape single quotes for SQL-like filter syntax
            ProjectListBindingSource.Filter = $"JBID LIKE '%{searchText}%' OR ProjectName LIKE '%{searchText}%' OR CustomerName LIKE '%{searchText}%' OR SalesName LIKE '%{searchText}%' OR Address LIKE '%{searchText}%'"
        End If
    End Sub

    Private Sub btnRefreshGrid_Click(sender As Object, e As EventArgs) Handles btnRefreshGrid.Click
        RefreshProjectList()
    End Sub

End Class