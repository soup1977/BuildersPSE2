Option Strict On
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Utilities

Public Class frmMainProjectList
    Private da As New ProjectDataAccess() ' DAL instance - servant-simple dependency
    Private projects As List(Of ProjectModel) ' Cache for filtering
    Private dtProjects As DataTable
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


    Private Sub DataGridViewProjects_DoubleClick(sender As Object, e As EventArgs) Handles DataGridViewProjects.DoubleClick
        Try
            If DataGridViewProjects.CurrentRow IsNot Nothing Then
                Dim projectID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("ProjectID").Value)
                Dim latestVersionID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("VersionID").Value)
                Dim selectedProj As ProjectModel = da.GetProjectByID(projectID)
                _mainForm.AddFormToTabControl(GetType(frmCreateEditProject), $"EditProject_{projectID}", New Object() {selectedProj, latestVersionID})
                UIHelper.Add($"Opened edit project form for ProjectID {projectID}")
            Else
                UIHelper.Add("No project selected for editing")
                MessageBox.Show("Select a project row to edit.", "Truss Tip", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            UIHelper.Add($"Error opening edit project form: {ex.Message}")
            MessageBox.Show($"Error opening edit project form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs)
        If MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Me.Close()
        End If
    End Sub

    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        If dvProjects IsNot Nothing Then
            Dim searchText As String = txtSearch.Text.Trim()
            If String.IsNullOrEmpty(searchText) Then
                dvProjects.RowFilter = String.Empty
            Else
                searchText = searchText.Replace("'", "''")
                dvProjects.RowFilter = $"JBID LIKE '%{searchText}%' OR ProjectName LIKE '%{searchText}%' OR CustomerName LIKE '%{searchText}%' OR SalesName LIKE '%{searchText}%' OR Address LIKE '%{searchText}%'"
            End If
        End If
    End Sub

    Private Sub btnRefreshGrid_Click(sender As Object, e As EventArgs) Handles btnRefreshGrid.Click
        RefreshProjectList()
    End Sub
End Class