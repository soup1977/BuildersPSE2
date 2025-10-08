Option Strict On

Imports BuildersPSE2.BuildersPSE.DataAccess




Public Class frmInclusionsExclusions
    Private ProjectID As Integer ' Set from caller, e.g., main form.
    Private ReadOnly da As New DataAccess()

    Public Sub New(projID As Integer)
        InitializeComponent() ' Call designer-generated setup
        ProjectID = projID
        Dim project = da.GetProjectByID(ProjectID)
        Me.Text = $"Inclusions/Exclusions - Project {Project.jbid}"
    End Sub

    ' In frmCreateEditProject.vb: Update UpdateStatus for robustness
    Private Sub UpdateStatus(message As String)
        Try
            Dim parentForm As frmMain = TryCast(Me.ParentForm, frmMain)
            If parentForm IsNot Nothing AndAlso parentForm.StatusLabel IsNot Nothing Then
                parentForm.StatusLabel.Text = $"{message} at {DateTime.Now:HH:mm:ss}"
            Else
                Debug.WriteLine($"Status update skipped: Parent form or StatusLabel is null. Message: {message}")
            End If
        Catch ex As Exception
            Debug.WriteLine($"Error updating status: {ex.Message}")
        End Try
    End Sub
    Private Sub frmInclusionsExclusions_Load(sender As Object, e As EventArgs) Handles MyBase.Load


        ' Load combo options from DB
        cmbBuildingCode.Items.AddRange(da.GetComboOptions("BuildingCode").ToArray())
        cmbImportance.Items.AddRange(da.GetComboOptions("Importance").ToArray())
        cmbExposure.Items.AddRange(da.GetComboOptions("Exposure").ToArray())
        cmbWindSpeed.Items.AddRange(da.GetComboOptions("WindSpeed").ToArray())
        cmbSnowLoad.Items.AddRange(da.GetComboOptions("SnowLoad").ToArray())
        cmbOccupancy.Items.AddRange(da.GetComboOptions("Occupancy").ToArray())
        cmbExtWall.Items.AddRange(da.GetComboOptions("BearingStyle").ToArray())
        cmbIntWall.Items.AddRange(da.GetComboOptions("BearingStyle").ToArray())
        cmbCorridorWall.Items.AddRange(da.GetComboOptions("BearingStyle").ToArray())



        ' Load loads combo options from DB to individual ComboBoxes
        Dim tcllOptions = da.GetComboOptions("TCLL").ToArray()
        cmbTCLL_Roof.Items.AddRange(tcllOptions)
        cmbTCLL_Floor.Items.AddRange(tcllOptions)
        cmbTCLL_Corridor.Items.AddRange(tcllOptions)

        Dim tcdlOptions = da.GetComboOptions("TCDL").ToArray()
        cmbTCDL_Roof.Items.AddRange(tcdlOptions)
        cmbTCDL_Floor.Items.AddRange(tcdlOptions)
        cmbTCDL_Corridor.Items.AddRange(tcdlOptions)

        Dim bcllOptions = da.GetComboOptions("BCLL").ToArray()
        cmbBCLL_Roof.Items.AddRange(bcllOptions)
        cmbBCLL_Floor.Items.AddRange(bcllOptions)
        cmbBCLL_Corridor.Items.AddRange(bcllOptions)

        Dim bcdlOptions = da.GetComboOptions("BCDL").ToArray()
        cmbBCDL_Roof.Items.AddRange(bcdlOptions)
        cmbBCDL_Floor.Items.AddRange(bcdlOptions)
        cmbBCDL_Corridor.Items.AddRange(bcdlOptions)

        Dim ocSpacingOptions = da.GetComboOptions("OCSpacing").ToArray()
        cmbOCSpacing_Roof.Items.AddRange(ocSpacingOptions)
        cmbOCSpacing_Floor.Items.AddRange(ocSpacingOptions)
        cmbOCSpacing_Corridor.Items.AddRange(ocSpacingOptions)

        Dim liveDeflOptions = da.GetComboOptions("LiveDefl").ToArray()
        cmbLiveLoadDeflection_Roof.Items.AddRange(liveDeflOptions)
        cmbLiveLoadDeflection_Floor.Items.AddRange(liveDeflOptions)
        cmbLiveLoadDeflection_Corridor.Items.AddRange(liveDeflOptions)

        Dim totalDeflOptions = da.GetComboOptions("TotalDefl").ToArray()
        cmbTotalLoadDeflection_Roof.Items.AddRange(totalDeflOptions)
        cmbTotalLoadDeflection_Floor.Items.AddRange(totalDeflOptions)
        cmbTotalLoadDeflection_Corridor.Items.AddRange(totalDeflOptions)

        Dim absoluteOptions = da.GetComboOptions("Absolute").ToArray()
        cmbAbsolute_Roof.Items.AddRange(absoluteOptions)
        cmbAbsolute_Floor.Items.AddRange(absoluteOptions)
        cmbAbsolute_Corridor.Items.AddRange(absoluteOptions)

        Dim deflectionOptions = da.GetComboOptions("Deflection").ToArray()
        cmbDeflection_Roof.Items.AddRange(deflectionOptions)
        cmbDeflection_Floor.Items.AddRange(deflectionOptions)
        cmbDeflection_Corridor.Items.AddRange(deflectionOptions)



        ' Add Note columns to DGVs
        For Each dgv In {dgvRoofItems, dgvFloorItems, dgvWallItems}
            Dim section As String = If(dgv Is dgvRoofItems, "Roof", If(dgv Is dgvFloorItems, "Floor", "Wall"))
            Dim colNote As New DataGridViewTextBoxColumn With {
            .Name = "col" & section & "Note",
            .HeaderText = "Note",
            .AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        }
            dgv.Columns.Add(colNote)
        Next

        ' Load item options from DB
        For Each item In da.GetItemOptions("Roof")
            Me.dgvRoofItems.Rows.Add(item.Item1, item.Item2, False, False, False, String.Empty)
        Next
        For Each item In da.GetItemOptions("Floor")
            Me.dgvFloorItems.Rows.Add(item.Item1, item.Item2, False, False, False, String.Empty)
        Next
        For Each item In da.GetItemOptions("Wall")
            Me.dgvWallItems.Rows.Add(item.Item1, item.Item2, False, False, False, String.Empty)
        Next

        ' Set default Excluded on all item rows
        For Each dgv In {dgvRoofItems, dgvFloorItems, dgvWallItems}
            Dim section As String = If(dgv Is dgvRoofItems, "Roof", If(dgv Is dgvFloorItems, "Floor", "Wall"))
            Dim colExcluded As String = "col" & section & "Excluded"
            For Each row As DataGridViewRow In dgv.Rows
                row.Cells(colExcluded).Value = True
            Next
        Next


        LoadData()

    End Sub

    Private Sub dgvItems_CellValueChanged(sender As Object, e As DataGridViewCellEventArgs) Handles dgvRoofItems.CellValueChanged, dgvFloorItems.CellValueChanged, dgvWallItems.CellValueChanged
        If e.RowIndex >= 0 AndAlso e.ColumnIndex >= 2 AndAlso e.ColumnIndex <= 4 Then ' Columns 2=Included, 3=Excluded, 4=Optional
            Dim dgv As DataGridView = DirectCast(sender, DataGridView)
            Dim row As DataGridViewRow = dgv.Rows(e.RowIndex)
            If CBool(row.Cells(e.ColumnIndex).Value) Then
                For col As Integer = 2 To 4
                    If col <> e.ColumnIndex Then
                        row.Cells(col).Value = False
                    End If
                Next
            End If
        End If
    End Sub
    Private Sub dgvItems_CurrentCellDirtyStateChanged(sender As Object, e As EventArgs) Handles dgvRoofItems.CurrentCellDirtyStateChanged, dgvFloorItems.CurrentCellDirtyStateChanged, dgvWallItems.CurrentCellDirtyStateChanged
        Dim dgv As DataGridView = DirectCast(sender, DataGridView)
        If dgv.CurrentCell IsNot Nothing AndAlso TypeOf dgv.CurrentCell Is DataGridViewCheckBoxCell Then
            dgv.CommitEdit(DataGridViewDataErrorContexts.Commit)
        End If
    End Sub

    Private Sub LoadData()
        Try

            UpdateStatus("Loading Inclusions/Exclusions data...")
            ' Load Design Info
            Dim info = da.GetProjectDesignInfo(ProjectID)
            If info IsNot Nothing Then
                cmbBuildingCode.SelectedItem = info.BuildingCode
                cmbImportance.SelectedItem = info.Importance
                cmbExposure.SelectedItem = info.ExposureCategory
                cmbWindSpeed.SelectedItem = info.WindSpeed
                cmbSnowLoad.SelectedItem = info.SnowLoadType
                cmbOccupancy.SelectedItem = info.OccupancyCategory
                txtRoofPitches.Text = info.RoofPitches
                txtFloorDepths.Text = info.FloorDepths
                txtWallHeights.Text = info.WallHeights
                txtHeelHeights.Text = info.HeelHeights
            End If



            ' Load Bearing Styles
            Dim bearing = da.GetProjectBearingStyles(ProjectID)
            If bearing IsNot Nothing Then
                cmbExtWall.SelectedItem = bearing.ExtWallStyle
                cboExtRimRibbon.Text = bearing.ExtRimRibbon
                cmbIntWall.SelectedItem = bearing.IntWallStyle
                cboIntRimRibbon.Text = bearing.IntRimRibbon
                cmbCorridorWall.SelectedItem = bearing.CorridorWallStyle
                cboCorridorRimRibbon.Text = bearing.CorridorRimRibbon
            End If

            ' Load Loads
            Dim loads = da.GetProjectLoads(ProjectID)
            If loads IsNot Nothing AndAlso loads.Count = 3 Then
                ' Assume order: Roof (0), Floor (1), Corridor (2) from ORDER BY Category in query
                With loads(0) ' Roof
                    cmbTCLL_Roof.Text = .TCLL
                    cmbTCDL_Roof.Text = .TCDL
                    cmbBCLL_Roof.Text = .BCLL
                    cmbBCDL_Roof.Text = .BCDL
                    cmbOCSpacing_Roof.Text = .OCSpacing
                    cmbLiveLoadDeflection_Roof.Text = .LiveLoadDeflection
                    cmbTotalLoadDeflection_Roof.Text = .TotalLoadDeflection
                    cmbAbsolute_Roof.Text = .Absolute
                    cmbDeflection_Roof.Text = .Deflection
                End With
                With loads(1) ' Floor
                    cmbTCLL_Floor.Text = .TCLL
                    cmbTCDL_Floor.Text = .TCDL
                    cmbBCLL_Floor.Text = .BCLL
                    cmbBCDL_Floor.Text = .BCDL
                    cmbOCSpacing_Floor.Text = .OCSpacing
                    cmbLiveLoadDeflection_Floor.Text = .LiveLoadDeflection
                    cmbTotalLoadDeflection_Floor.Text = .TotalLoadDeflection
                    cmbAbsolute_Floor.Text = .Absolute
                    cmbDeflection_Floor.Text = .Deflection
                End With
                With loads(2) ' Corridor
                    cmbTCLL_Corridor.Text = .TCLL
                    cmbTCDL_Corridor.Text = .TCDL
                    cmbBCLL_Corridor.Text = .BCLL
                    cmbBCDL_Corridor.Text = .BCDL
                    cmbOCSpacing_Corridor.Text = .OCSpacing
                    cmbLiveLoadDeflection_Corridor.Text = .LiveLoadDeflection
                    cmbTotalLoadDeflection_Corridor.Text = .TotalLoadDeflection
                    cmbAbsolute_Corridor.Text = .Absolute
                    cmbDeflection_Corridor.Text = .Deflection
                End With
            End If


            ' Load Notes
            Dim note = da.GetProjectGeneralNotes(ProjectID)
            If note IsNot Nothing Then
                txtGeneralNotes.Text = note.Notes
            End If

            ' Load Items (Roof, Floor, Wall)
            Dim items = da.GetProjectItems(ProjectID)
            For Each dgv In {dgvRoofItems, dgvFloorItems, dgvWallItems}
                Dim section As String = If(dgv Is dgvRoofItems, "Roof", If(dgv Is dgvFloorItems, "Floor", "Wall"))
                Dim colKN As String = "col" & section & "KN"
                Dim colIncluded As String = "col" & section & "Included"
                Dim colExcluded As String = "col" & section & "Excluded"
                Dim colOptional As String = "col" & section & "Optional"
                Dim colNote As String = "col" & section & "Note"
                For Each item In items.Where(Function(i) i.Section = section)
                    Dim row = dgv.Rows.Cast(Of DataGridViewRow).FirstOrDefault(Function(r) CInt(r.Cells(colKN).Value) = item.KN)
                    If row IsNot Nothing Then
                        row.Cells(colIncluded).Value = item.Status = "y"
                        row.Cells(colExcluded).Value = item.Status = "x"
                        row.Cells(colOptional).Value = item.Status = "o"
                        row.Cells(colNote).Value = item.Note
                    End If
                Next
            Next
            UpdateStatus("Inclusions/Exclusions data loaded successfully.")
        Catch ex As Exception
            UpdateStatus("Error loading Inclusions/Exclusions data: " & ex.Message)
            MessageBox.Show("Error loading data: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SaveData(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            UpdateStatus("Saving Inclusions/Exclusions data...")
            ' Save Design Info
            Dim info As New ProjectDesignInfoModel With {
            .ProjectID = ProjectID,
            .BuildingCode = If(cmbBuildingCode.SelectedItem Is Nothing, String.Empty, CStr(cmbBuildingCode.SelectedItem)),
            .Importance = If(cmbImportance.SelectedItem Is Nothing, String.Empty, CStr(cmbImportance.SelectedItem)),
            .ExposureCategory = If(cmbExposure.SelectedItem Is Nothing, String.Empty, CStr(cmbExposure.SelectedItem)),
            .WindSpeed = If(cmbWindSpeed.SelectedItem Is Nothing, String.Empty, CStr(cmbWindSpeed.SelectedItem)),
            .SnowLoadType = If(cmbSnowLoad.SelectedItem Is Nothing, String.Empty, CStr(cmbSnowLoad.SelectedItem)),
            .OccupancyCategory = If(cmbOccupancy.SelectedItem Is Nothing, String.Empty, CStr(cmbOccupancy.SelectedItem)),
            .RoofPitches = txtRoofPitches.Text,
            .FloorDepths = txtFloorDepths.Text,
            .WallHeights = txtWallHeights.Text,
            .HeelHeights = txtHeelHeights.Text
        }
            Dim existingInfo = da.GetProjectDesignInfo(ProjectID)
            If existingInfo IsNot Nothing Then info.InfoID = existingInfo.InfoID
            da.SaveProjectDesignInfo(info)

            ' Save Loads
            Dim loads As New List(Of ProjectLoadModel) From {
                New ProjectLoadModel With {
    .ProjectID = ProjectID,
    .Category = "Roof",
    .TCLL = cmbTCLL_Roof.Text,
    .TCDL = cmbTCDL_Roof.Text,
    .BCLL = cmbBCLL_Roof.Text,
    .BCDL = cmbBCDL_Roof.Text,
    .OCSpacing = cmbOCSpacing_Roof.Text,
    .LiveLoadDeflection = cmbLiveLoadDeflection_Roof.Text,
    .TotalLoadDeflection = cmbTotalLoadDeflection_Roof.Text,
    .Absolute = cmbAbsolute_Roof.Text,
    .Deflection = cmbDeflection_Roof.Text
},
                New ProjectLoadModel With {
    .ProjectID = ProjectID,
    .Category = "Floor",
    .TCLL = cmbTCLL_Floor.Text,
    .TCDL = cmbTCDL_Floor.Text,
    .BCLL = cmbBCLL_Floor.Text,
    .BCDL = cmbBCDL_Floor.Text,
    .OCSpacing = cmbOCSpacing_Floor.Text,
    .LiveLoadDeflection = cmbLiveLoadDeflection_Floor.Text,
    .TotalLoadDeflection = cmbTotalLoadDeflection_Floor.Text,
    .Absolute = cmbAbsolute_Floor.Text,
    .Deflection = cmbDeflection_Floor.Text
},
                New ProjectLoadModel With {
    .ProjectID = ProjectID,
    .Category = "Corridor",
    .TCLL = cmbTCLL_Corridor.Text,
    .TCDL = cmbTCDL_Corridor.Text,
    .BCLL = cmbBCLL_Corridor.Text,
    .BCDL = cmbBCDL_Corridor.Text,
    .OCSpacing = cmbOCSpacing_Corridor.Text,
    .LiveLoadDeflection = cmbLiveLoadDeflection_Corridor.Text,
    .TotalLoadDeflection = cmbTotalLoadDeflection_Corridor.Text,
    .Absolute = cmbAbsolute_Corridor.Text,
    .Deflection = cmbDeflection_Corridor.Text
}
            }
            da.SaveProjectLoads(ProjectID, loads)


            ' Save Bearing Styles
            Dim bearing As New ProjectBearingStylesModel With {
            .ProjectID = ProjectID,
            .ExtWallStyle = If(cmbExtWall.SelectedItem Is Nothing, String.Empty, CStr(cmbExtWall.SelectedItem)),
            .ExtRimRibbon = cboExtRimRibbon.Text,
            .IntWallStyle = If(cmbIntWall.SelectedItem Is Nothing, String.Empty, CStr(cmbIntWall.SelectedItem)),
            .IntRimRibbon = cboIntRimRibbon.Text,
            .CorridorWallStyle = If(cmbCorridorWall.SelectedItem Is Nothing, String.Empty, CStr(cmbCorridorWall.SelectedItem)),
            .CorridorRimRibbon = cboCorridorRimRibbon.Text
        }
            Dim existingBearing = da.GetProjectBearingStyles(ProjectID)
            If existingBearing IsNot Nothing Then bearing.BearingID = existingBearing.BearingID
            da.SaveProjectBearingStyles(bearing)

            ' Save Notes
            Dim note As New ProjectGeneralNotesModel With {
            .ProjectID = ProjectID,
            .Notes = txtGeneralNotes.Text
        }
            Dim existingNote = da.GetProjectGeneralNotes(ProjectID)
            If existingNote IsNot Nothing Then note.NoteID = existingNote.NoteID
            da.SaveProjectGeneralNotes(note)

            ' Save Items

            ' Save Items
            Dim items As New List(Of ProjectItemModel)
            For Each dgv In {dgvRoofItems, dgvFloorItems, dgvWallItems}
                Dim section As String = If(dgv Is dgvRoofItems, "Roof", If(dgv Is dgvFloorItems, "Floor", "Wall"))
                Dim colIncluded As String = "col" & section & "Included"
                Dim colExcluded As String = "col" & section & "Excluded"
                Dim colOptional As String = "col" & section & "Optional"
                Dim colKN As String = "col" & section & "KN"
                Dim colDesc As String = "col" & section & "Desc"
                Dim colNote As String = "col" & section & "Note"
                For Each row As DataGridViewRow In dgv.Rows
                    Dim status As String = If(CBool(row.Cells(colIncluded).Value), "y", If(CBool(row.Cells(colExcluded).Value), "x", If(CBool(row.Cells(colOptional).Value), "o", "")))
                    items.Add(New ProjectItemModel With {
                .ProjectID = ProjectID,
                .Section = section,
                .KN = CInt(row.Cells(colKN).Value),
                .Description = CStr(row.Cells(colDesc).Value),
                .Status = status,
                .Note = CStr(row.Cells(colNote).Value)
            })
                Next
            Next
            da.SaveProjectItems(ProjectID, items)

            UpdateStatus("Inclusions/Exclusions data saved successfully.")
            MessageBox.Show("Data saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            UpdateStatus("Error saving Inclusions/Exclusions data: " & ex.Message)
            MessageBox.Show("Error saving: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        RemoveTabFromTabControl(CStr(Me.Tag))
    End Sub
End Class