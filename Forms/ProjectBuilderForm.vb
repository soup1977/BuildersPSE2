' I am creating a new form called ProjectBuilderForm that allows users to input building configurations via a DataGridView.
' The form is initialized with a VersionID, assuming the project and version are already created elsewhere.
' On Apply, it parses the grid rows, creates BuildingModel instances, populates their Levels based on the specified counts and naming conventions,
' and uses the existing SaveBuilding and SaveLevel functions to persist the data.
' Minimal changes: Only adding this new form class; no modifications to existing code.

Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess

Public Class ProjectBuilderForm
    Inherits Form

    Private ReadOnly pda As New ProjectDataAccess()
    Private m_VersionID As Integer

    Public Sub New(versionID As Integer)
        m_VersionID = versionID
        InitializeComponent()
    End Sub
    Private ReadOnly _mainForm As frmMain = CType(Application.OpenForms.OfType(Of frmMain)().FirstOrDefault(), frmMain)

    Private Sub btnApply_Click(sender As Object, e As EventArgs) Handles BtnSaveProjectBuilder.Click
        For Each row As DataGridViewRow In dgvProjBlder.Rows
            If row.IsNewRow Then Continue For

            Dim buildingName As String = If(row.Cells("colBuildingName").Value?.ToString(), String.Empty)
            If String.IsNullOrWhiteSpace(buildingName) Then Continue For

            Dim bldgQtyStr As String = If(row.Cells("colBldgQty").Value?.ToString(), "0")
            Dim bldgQty As Integer
            If Not Integer.TryParse(bldgQtyStr, bldgQty) OrElse bldgQty <= 0 Then
                MessageBox.Show("Invalid Building Qty for " & buildingName)
                Continue For
            End If

            Dim resUnitsStr As String = If(row.Cells("colResUnits").Value?.ToString(), String.Empty)
            Dim resUnits As Integer? = Nothing
            Dim resTemp As Integer
            If Integer.TryParse(resUnitsStr, resTemp) Then resUnits = resTemp

            Dim numFloorsStr As String = If(row.Cells("colNumFloors").Value?.ToString(), "0")
            Dim numFloors As Integer
            If Not Integer.TryParse(numFloorsStr, numFloors) OrElse numFloors < 0 Then
                MessageBox.Show("Invalid Number of Floor Levels for " & buildingName)
                Continue For
            End If

            Dim numRoofsStr As String = If(row.Cells("colNumRoofs").Value?.ToString(), "0")
            Dim numRoofs As Integer
            If Not Integer.TryParse(numRoofsStr, numRoofs) OrElse numRoofs < 0 Then
                MessageBox.Show("Invalid Number of Roof Levels for " & buildingName)
                Continue For
            End If

            Dim numWallsStr As String = If(row.Cells("colNumWalls").Value?.ToString(), "0")
            Dim numWalls As Integer
            If Not Integer.TryParse(numWallsStr, numWalls) OrElse numWalls < 0 Then
                MessageBox.Show("Invalid Number of Wall Levels for " & buildingName)
                Continue For
            End If

            Dim bldg As New BuildingModel With {
                .BuildingName = buildingName,
                .BldgQty = bldgQty,
                .ResUnits = resUnits,
                .BuildingType = Nothing, ' Assuming not required; set if needed
                .Levels = New List(Of LevelModel)
            }

            ' Add floor levels starting from 2nd
            Dim floorStart As Integer = 2
            For i As Integer = 1 To numFloors
                Dim levelNum As Integer = floorStart + i - 1
                Dim suffix As String = GetOrdinalSuffix(levelNum)
                Dim levelName As String = levelNum.ToString() & suffix & " Floor"
                Dim level As New LevelModel With {
                    .ProductTypeID = 1, ' Assuming 1 = Floor
                    .LevelNumber = levelNum,
                    .LevelName = levelName
                }
                bldg.Levels.Add(level)
            Next

            ' Add roof levels
            For i As Integer = 1 To numRoofs
                Dim levelName As String = If(numRoofs = 1, "Roof", "Roof " & i.ToString())
                Dim level As New LevelModel With {
                    .ProductTypeID = 2, ' Assuming 2 = Roof
                    .LevelNumber = i,
                    .LevelName = levelName
                }
                bldg.Levels.Add(level)
            Next

            ' Add wall levels starting from 1st
            Dim wallStart As Integer = 1
            For i As Integer = 1 To numWalls
                Dim levelNum As Integer = wallStart + i - 1
                Dim suffix As String = GetOrdinalSuffix(levelNum)
                Dim levelName As String = levelNum.ToString() & suffix & " Level"
                Dim level As New LevelModel With {
                    .ProductTypeID = 3, ' Assuming 3 = Wall
                    .LevelNumber = levelNum,
                    .LevelName = levelName
                }
                bldg.Levels.Add(level)
            Next

            ' Save using existing function
            ProjectDataAccess.SaveBuilding(bldg, m_VersionID)
        Next

        MessageBox.Show("Buildings and levels created successfully.")


    End Sub

    Private Function GetOrdinalSuffix(num As Integer) As String
        Dim teen As Boolean = (num Mod 100) >= 11 AndAlso (num Mod 100) <= 13
        Dim lastDigit As Integer = num Mod 10
        If teen Then Return "th"
        Select Case lastDigit
            Case 1
                Return "st"
            Case 2
                Return "nd"
            Case 3
                Return "rd"
            Case Else
                Return "th"
        End Select
    End Function

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Try

            _mainform.RemoveTabFromTabControl($"ProjectBuilder_{m_VersionID}")
        Catch ex As Exception

            MessageBox.Show("Error closing form: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class