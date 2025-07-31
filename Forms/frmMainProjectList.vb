Option Strict On

Imports BuildersPSE2.BuildersPSE.DataAccess
Imports BuildersPSE2.BuildersPSE.Models

Public Class frmMainProjectList
    Private da As New DataAccess() ' DAL instance - servant-simple dependency
    Private projects As List(Of ProjectModel) ' Cache for filtering

    Private Sub FrmMainProjectList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadProjects()
    End Sub

    Private Sub LoadProjects()
        Try
            ' Fetch parent projects without details
            projects = da.GetProjects(includeDetails:=False)
            ' Join with latest version data
            Dim displayProjects = projects.Select(Function(p)
                                                      Dim latestVersion As ProjectVersionModel = da.GetProjectVersions(p.ProjectID).FirstOrDefault()
                                                      Return New With {
                                                          p.ProjectID,
                                                          p.JBID,
                                                          p.ProjectName,
                                                          p.BidDate,
                                                          p.Estimator.EstimatorName,
                                                          .VersionName = If(latestVersion IsNot Nothing, latestVersion.VersionName, String.Empty),
                                                          .CustomerName = If(latestVersion IsNot Nothing, latestVersion.CustomerName, String.Empty),
                                                          .SalesName = If(latestVersion IsNot Nothing, latestVersion.SalesName, String.Empty),
                                                          .VersionID = If(latestVersion IsNot Nothing, latestVersion.VersionID, 0)
                                                      }
                                                  End Function).ToList()
            DataGridViewProjects.DataSource = displayProjects
            ConfigureGridColumns()
        Catch ex As Exception
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

    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        'stub for later date
    End Sub

    Private Sub BtnNewProject_Click(sender As Object, e As EventArgs) Handles btnNewProject.Click
        Using frm As New frmCreateEditProject() ' Open blank for new
            If frm.ShowDialog() = DialogResult.OK Then
                LoadProjects() ' Refresh post-save
            End If
        End Using
    End Sub

    Private Sub BtnEditProject_Click(sender As Object, e As EventArgs) Handles btnEditProject.Click
        If DataGridViewProjects.CurrentRow IsNot Nothing Then
            Dim projectID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("ProjectID").Value)
            Dim latestVersionID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("VersionID").Value)
            Dim selectedProj As ProjectModel = da.GetProjectByID(projectID)
            Using frm As New frmCreateEditProject(selectedProj, latestVersionID) ' Pass project and latest VersionID
                If frm.ShowDialog() = DialogResult.OK Then
                    LoadProjects() ' Refresh
                End If
            End Using
        Else
            MessageBox.Show("Focus on a project row to edit.", "Truss Tip", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub BtnOpenPSE_Click(sender As Object, e As EventArgs) Handles btnOpenPSE.Click
        If DataGridViewProjects.CurrentRow IsNot Nothing Then
            Dim projectID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("ProjectID").Value)
            Using frm As New FrmPSE(projectID) ' Pass ID as Integer
                frm.ShowDialog() ' Modal for focus
            End Using
        Else
            MessageBox.Show("Select a project to open PSE.", "Truss Tip", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub btnRefreshGrid_Click(sender As Object, e As EventArgs) Handles btnRefreshGrid.Click
        LoadProjects()
    End Sub
End Class