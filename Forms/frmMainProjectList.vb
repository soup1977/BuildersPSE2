Option Strict On
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess

Public Class frmMainProjectList
    Private da As New ProjectDataAccess() ' DAL instance - servant-simple dependency
    Private projects As List(Of ProjectModel) ' Cache for filtering

    Private Sub FrmMainProjectList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadProjects()
        UpdateStatus("Project list loaded")
    End Sub
    Private Sub UpdateStatus(message As String)
        Dim mdiParent As frmMain = TryCast(Me.MdiParent, frmMain)
        If mdiParent IsNot Nothing Then
            mdiParent.StatusLabel.Text = $"{message} at {DateTime.Now:HH:mm:ss}"
        End If
    End Sub
    Private Sub LoadProjects()
        Try
            projects = da.GetProjects(includeDetails:=False)
            Dim displayProjects = projects.Select(Function(p)
                                                      Dim latestVersion As ProjectVersionModel = ProjVersionDataAccess.GetProjectVersions(p.ProjectID).FirstOrDefault()
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
            UpdateStatus($"Loaded {displayProjects.Count} projects")
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
        'stub for later date
    End Sub

    'Private Sub BtnNewProject_Click(sender As Object, e As EventArgs) Handles btnNewProject.Click
    '    Try
    '        OpenMDIForm(GetType(frmCreateEditProject), "NewProject")
    '        UpdateStatus("Opened new project form")
    '        LoadProjects()
    '    Catch ex As Exception
    '        UpdateStatus($"Error opening new project form: {ex.Message}")
    '        MessageBox.Show($"Error opening new project form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '    End Try
    'End Sub

    'Private Sub BtnEditProject_Click(sender As Object, e As EventArgs) Handles btnEditProject.Click
    '    If DataGridViewProjects.CurrentRow IsNot Nothing Then
    '        Try
    '            Dim projectID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("ProjectID").Value)
    '            Dim latestVersionID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("VersionID").Value)
    '            Dim selectedProj As ProjectModel = da.GetProjectByID(projectID)
    '            OpenMDIForm(GetType(frmCreateEditProject), $"EditProject_{projectID}", New Object() {selectedProj, latestVersionID})
    '            UpdateStatus($"Opened edit project form for ProjectID {projectID}")
    '            LoadProjects()
    '        Catch ex As Exception
    '            UpdateStatus($"Error opening edit project form: {ex.Message}")
    '            MessageBox.Show($"Error opening edit project form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        End Try
    '    Else
    '        UpdateStatus("No project selected for editing")
    '        MessageBox.Show("Focus on a project row to edit.", "Truss Tip", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '    End If
    'End Sub

    'Private Sub BtnOpenPSE_Click(sender As Object, e As EventArgs) Handles btnOpenPSE.Click
    '    If DataGridViewProjects.CurrentRow IsNot Nothing Then
    '        Try
    '            Dim projectID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("ProjectID").Value)
    '            Dim versionID As Integer = CInt(DataGridViewProjects.CurrentRow.Cells("VersionID").Value)
    '            Dim versions As List(Of ProjectVersionModel) = da.GetProjectVersions(projectID)
    '            If versionID <= 0 Then
    '                If Not versions.Any() Then
    '                    UpdateStatus($"No versions exist for ProjectID {projectID}")
    '                    MessageBox.Show("No versions exist for this project. Create a version in the project editor.", "No Versions", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '                    Exit Sub
    '                End If
    '                versionID = versions.OrderByDescending(Function(v) v.VersionDate).First().VersionID
    '            End If
    '            OpenMDIForm(GetType(FrmPSE), $"PSE_{projectID}_{versionID}", New Object() {projectID, versionID})
    '            UpdateStatus($"Opened PSE form for ProjectID {projectID}, VersionID {versionID}")
    '        Catch ex As Exception
    '            UpdateStatus($"Error opening PSE form: {ex.Message}")
    '            MessageBox.Show("Error opening PSE form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
    '        End Try
    '    Else
    '        UpdateStatus("No project selected for PSE")
    '        MessageBox.Show("Select a project to open PSE.", "Truss Tip", MessageBoxButtons.OK, MessageBoxIcon.Information)
    '    End If
    'End Sub

    Private Sub btnRefreshGrid_Click(sender As Object, e As EventArgs) Handles btnRefreshGrid.Click
        LoadProjects()
    End Sub
End Class