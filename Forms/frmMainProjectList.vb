Option Strict On
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Utilities
Imports System.Runtime.InteropServices

Public Class frmMainProjectList
    Private da As New ProjectDataAccess()
    Private dvProjects As DataView
    Private ReadOnly _mainForm As frmMain = CType(Application.OpenForms.OfType(Of frmMain)().FirstOrDefault(), frmMain)

    ' Timer for search debouncing
    Private WithEvents searchTimer As New Timer() With {.Interval = 300}

    ' Win32 API for flicker-free updates
    <DllImport("user32.dll")>
    Private Shared Function SendMessage(hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr
    End Function
    Private Const WM_SETREDRAW As Integer = &HB

    Private Sub FrmMainProjectList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        OptimizeGridSettings()
        RefreshProjectList()
        UIHelper.Add("Project list loaded")
    End Sub

    ''' <summary>
    ''' One-time grid optimizations applied at load
    ''' </summary>
    Private Sub OptimizeGridSettings()
        With DataGridViewProjects
            ' Double buffering eliminates flicker (use reflection since property is protected)
            Dim dgvType As Type = GetType(DataGridView)
            Dim pi = dgvType.GetProperty("DoubleBuffered", Reflection.BindingFlags.Instance Or Reflection.BindingFlags.NonPublic)
            pi?.SetValue(DataGridViewProjects, True, Nothing)

            ' Disable auto-sizing during data operations for speed
            .AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
            .RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing

            ' Set fixed row height instead of auto-calculating
            .RowTemplate.Height = 22
        End With
    End Sub

    Public Sub RefreshProjectList()
        BeginGridUpdate()
        Try
            Me.ProjectListTableAdapter.Fill(Me.ProjectSummaryDataSet.ProjectList)
            dvProjects = Me.ProjectSummaryDataSet.ProjectList.DefaultView
        Finally
            EndGridUpdate()
        End Try
    End Sub

    ''' <summary>
    ''' Suspends all painting for maximum speed
    ''' </summary>
    Private Sub BeginGridUpdate()
        SendMessage(DataGridViewProjects.Handle, WM_SETREDRAW, IntPtr.Zero, IntPtr.Zero)
        DataGridViewProjects.SuspendLayout()
    End Sub

    ''' <summary>
    ''' Resumes painting and forces refresh
    ''' </summary>
    Private Sub EndGridUpdate()
        DataGridViewProjects.ResumeLayout()
        SendMessage(DataGridViewProjects.Handle, WM_SETREDRAW, New IntPtr(1), IntPtr.Zero)
        DataGridViewProjects.Invalidate()
    End Sub

    Private Sub dgvProjects_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridViewProjects.CellContentClick
        Const LINK_COLUMN_NAME As String = "JBID"

        If e.RowIndex < 0 Then Exit Sub
        If DataGridViewProjects.Columns(e.ColumnIndex).Name <> LINK_COLUMN_NAME Then Exit Sub

        Dim projectIdObj As Object = DataGridViewProjects.Rows(e.RowIndex).Cells("ProjectID").Value

        If projectIdObj Is Nothing OrElse IsDBNull(projectIdObj) Then Exit Sub

        OpenProjectDetails(Convert.ToInt32(projectIdObj))
    End Sub

    Private Sub OpenProjectDetails(projectId As Integer)
        Try
            Dim row As DataGridViewRow = DataGridViewProjects.CurrentRow

            ' Validate current row matches, use direct index if not
            If row Is Nothing OrElse row.IsNewRow OrElse
               row.Cells("ProjectID").Value Is Nothing OrElse
               Convert.ToInt32(row.Cells("ProjectID").Value) <> projectId Then

                ' Use DataView.Find for O(1) lookup if sorted, otherwise linear search
                row = DataGridViewProjects.Rows _
                    .Cast(Of DataGridViewRow)() _
                    .FirstOrDefault(Function(r) Not r.IsNewRow AndAlso
                                              r.Cells("ProjectID").Value IsNot Nothing AndAlso
                                              Convert.ToInt32(r.Cells("ProjectID").Value) = projectId)
            End If

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
        If DataGridViewProjects.CurrentRow IsNot Nothing AndAlso Not DataGridViewProjects.CurrentRow.IsNewRow Then
            OpenProjectDetails(CInt(DataGridViewProjects.CurrentRow.Cells("ProjectID").Value))
        Else
            UIHelper.Add("No project selected for editing")
            MessageBox.Show("Select a project row to edit.", "Truss Tip", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub TxtSearch_TextChanged(sender As Object, e As EventArgs) Handles txtSearch.TextChanged
        searchTimer.Stop()
        searchTimer.Start()
    End Sub

    Private Sub searchTimer_Tick(sender As Object, e As EventArgs) Handles searchTimer.Tick
        searchTimer.Stop()
        ApplySearchFilter()
    End Sub

    Private Sub ApplySearchFilter()
        Dim searchText As String = txtSearch.Text.Trim()

        BeginGridUpdate()
        Try
            If String.IsNullOrEmpty(searchText) Then
                ProjectListBindingSource.Filter = String.Empty
            Else
                searchText = searchText.Replace("'", "''")
                ProjectListBindingSource.Filter = $"JBID LIKE '%{searchText}%' OR ProjectName LIKE '%{searchText}%' OR CustomerName LIKE '%{searchText}%' OR SalesName LIKE '%{searchText}%' OR Address LIKE '%{searchText}%'"
            End If
        Finally
            EndGridUpdate()
        End Try
    End Sub

    Private Sub btnRefreshGrid_Click(sender As Object, e As EventArgs) Handles btnRefreshGrid.Click
        RefreshProjectList()
    End Sub

    ''' <summary>
    ''' Cleanup timer on form close
    ''' </summary>
    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        searchTimer?.Dispose()
        MyBase.OnFormClosing(e)
    End Sub

End Class