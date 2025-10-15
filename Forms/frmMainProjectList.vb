Option Strict On
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess
Imports System.Data

Public Class frmMainProjectList
    Private da As New ProjectDataAccess() ' DAL instance - servant-simple dependency
    Private projects As List(Of ProjectModel) ' Cache for filtering
    Private dtProjects As DataTable
    Private dvProjects As DataView

    Private Sub FrmMainProjectList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.SuspendLayout()
        LoadProjects()
        UpdateStatus("Project list loaded")
        Me.ResumeLayout()
    End Sub

    Private Sub UpdateStatus(message As String)
        Dim mdiParent As frmMain = TryCast(Me.MdiParent, frmMain)
        If mdiParent IsNot Nothing Then
            mdiParent.StatusLabel.Text = $"{message} at {DateTime.Now:HH:mm:ss}"
        End If
    End Sub

    Private Sub LoadProjects()
        Try
            ' Fetch projects and versions, taking the latest version (first in sorted list)
            projects = da.GetProjects(includeDetails:=False)
            Dim versionDict As Dictionary(Of Integer, ProjectVersionModel) = ProjVersionDataAccess.GetAllProjectVersions().Where(Function(list) list.Any()).ToDictionary(Function(list) list(0).ProjectID, Function(list) list(0))

            ' Initialize DataTable with optimized settings
            dtProjects = New DataTable()
            With dtProjects.Columns
                .Add("ProjectID", GetType(Integer))
                .Add("JBID", GetType(String))
                .Add("ProjectName", GetType(String))
                .Add("BidDate", GetType(DateTime))
                .Add("EstimatorName", GetType(String))
                .Add("VersionName", GetType(String))
                .Add("CustomerName", GetType(String))
                .Add("SalesName", GetType(String))
                .Add("VersionID", GetType(Integer))
            End With

            ' Disable constraints/notifications for faster loading
            dtProjects.BeginLoadData()
            Try
                For Each p In projects
                    Dim row As DataRow = dtProjects.NewRow()
                    Dim latestVersion As ProjectVersionModel = Nothing
                    versionDict.TryGetValue(p.ProjectID, latestVersion)
                    row("ProjectID") = p.ProjectID
                    row("JBID") = p.JBID
                    row("ProjectName") = p.ProjectName
                    row("BidDate") = If(p.BidDate.HasValue, CType(p.BidDate.Value, Object), DBNull.Value)
                    row("EstimatorName") = p.Estimator.EstimatorName
                    row("VersionName") = If(latestVersion IsNot Nothing, latestVersion.VersionName, String.Empty)
                    row("CustomerName") = If(latestVersion IsNot Nothing, latestVersion.CustomerName, String.Empty)
                    row("SalesName") = If(latestVersion IsNot Nothing, latestVersion.SalesName, String.Empty)
                    row("VersionID") = If(latestVersion IsNot Nothing, latestVersion.VersionID, 0)
                    dtProjects.Rows.Add(row)
                Next
            Finally
                dtProjects.EndLoadData()
            End Try

            dvProjects = New DataView(dtProjects)
            DataGridViewProjects.DataSource = dvProjects
            ConfigureGridColumns()
            UpdateStatus($"Loaded {dtProjects.Rows.Count} projects")
        Catch ex As Exception
            UpdateStatus($"Error loading projects: {ex.Message}")
            MessageBox.Show("Error loading projects: " & ex.Message, "Truss Alert", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub ConfigureGridColumns()
        With DataGridViewProjects
            .Columns("ProjectID").Visible = False
            .Columns("VersionID").Visible = False
            .Columns("JBID").HeaderText = "Proj Nbr"
            .Columns("ProjectName").HeaderText = "Name"
            .Columns("BidDate").HeaderText = "Bid Date"
            .Columns("EstimatorName").HeaderText = "Estimator"
            .Columns("VersionName").HeaderText = "Latest Version"
            .Columns("CustomerName").HeaderText = "Customer"
            .Columns("SalesName").HeaderText = "Salesman"
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        End With
    End Sub

    Private Sub DataGridViewProjects_DoubleClick(sender As Object, e As EventArgs) Handles DataGridViewProjects.DoubleClick
        Try
            If DataGridViewProjects.CurrentRow IsNot Nothing Then
                Dim projectID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("ProjectID").Value)
                Dim latestVersionID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("VersionID").Value)
                Dim selectedProj As ProjectModel = da.GetProjectByID(projectID)
                AddFormToTabControl(GetType(frmCreateEditProject), $"EditProject_{projectID}", New Object() {selectedProj, latestVersionID})
                UpdateStatus($"Opened edit project form for ProjectID {projectID}")
            Else
                UpdateStatus("No project selected for editing")
                MessageBox.Show("Select a project row to edit.", "Truss Tip", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        Catch ex As Exception
            UpdateStatus($"Error opening edit project form: {ex.Message}")
            MessageBox.Show($"Error opening edit project form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        If MessageBox.Show("Are you sure you want to exit?", "Confirm Exit", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.Yes Then
            Application.Exit()
        End If
    End Sub

    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        If dvProjects IsNot Nothing Then
            Dim searchText As String = txtSearch.Text.Trim()
            If String.IsNullOrEmpty(searchText) Then
                dvProjects.RowFilter = String.Empty
            Else
                searchText = searchText.Replace("'", "''")
                dvProjects.RowFilter = $"JBID LIKE '%{searchText}%' OR ProjectName LIKE '%{searchText}%' OR EstimatorName LIKE '%{searchText}%'"
            End If
        End If
    End Sub

    Private Sub btnRefreshGrid_Click(sender As Object, e As EventArgs) Handles btnRefreshGrid.Click
        LoadProjects()
    End Sub

    Public Sub RefreshProjects()
        LoadProjects()
    End Sub
End Class