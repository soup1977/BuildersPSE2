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
            ' Use light load without details for list view optimization
            projects = da.GetProjects(includeDetails:=False)
            DataGridViewProjects.DataSource = projects
            ConfigureGridColumns()
        Catch ex As Exception
            ' Improved error handling with specific message and logging placeholder
            MessageBox.Show("Error loading projects: " & ex.Message, "Truss Alert", MessageBoxButtons.OK, MessageBoxIcon.Error)
            ' Log error here (e.g., to file or service) for profound knowledge analysis
        End Try
    End Sub

    Private Sub ConfigureGridColumns()
        With DataGridViewProjects
            .Columns("ProjectType").Visible = False
            .Columns("ProjectID").Visible = False
            .Columns("Estimator").Visible = True
            .Columns("Address").Visible = False
            .Columns("City").Visible = False
            .Columns("State").Visible = False
            .Columns("Zip").Visible = False
            .Columns("ArchPlansDated").Visible = False
            .Columns("EngPlansDated").Visible = False
            .Columns("MilesToJobSite").Visible = False
            .Columns("TotalNetSqft").Visible = False
            .Columns("TotalGrossSqft").Visible = False
            .Columns("ProjectArchitect").Visible = False
            .Columns("ProjectEngineer").Visible = False
            .Columns("ProjectNotes").Visible = False
            .Columns("CreatedDate").Visible = False
            .Columns("JBID").HeaderText = "Proj Nbr"
            .Columns("ProjectName").HeaderText = "Name"
            .Columns("BidDate").HeaderText = "Bid Date"
            .Columns("PrimaryCustomer").HeaderText = "Customer"
            .Columns("PrimarySalesman").HeaderText = "Salesman"
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        End With
    End Sub

    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        Dim searchText As String = txtSearch.Text.ToLower()
        If projects Is Nothing Then Exit Sub
        Dim filtered As New List(Of ProjectModel)
        For Each p As ProjectModel In projects
            If (p.ProjectName?.ToLower().Contains(searchText) = True) OrElse
                (p.BidDate.HasValue AndAlso p.BidDate.Value.ToString().ToLower().Contains(searchText)) OrElse
                (p.PrimaryCustomer?.ToLower().Contains(searchText) = True) OrElse
                (p.PrimarySalesman?.ToLower().Contains(searchText) = True) Then
                filtered.Add(p)
            End If
        Next
        DataGridViewProjects.DataSource = filtered
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
            Dim selectedProj As ProjectModel = CType(DataGridViewProjects.CurrentRow.DataBoundItem, ProjectModel)
            Dim fullProj As ProjectModel = da.GetProjectByID(selectedProj.ProjectID)
            Using frm As New frmCreateEditProject(fullProj) ' Pass full project
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
            Dim selectedProj As ProjectModel = CType(DataGridViewProjects.CurrentRow.DataBoundItem, ProjectModel)
            Using frm As New FrmPSE(selectedProj.ProjectID) ' Pass ID as Integer
                frm.ShowDialog() ' Modal for focus - servant to workflow
            End Using
        Else
            MessageBox.Show("Select a project to open PSE.", "Truss Tip", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub
End Class