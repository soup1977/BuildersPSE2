Option Strict On
Imports System.Windows.Forms
Imports BuildersPSE2.BuildersPSE.DataAccess
Imports BuildersPSE2.BuildersPSE.Models



Public Class frmInclusionsExclusions
    Private ProjectID As Integer ' Set from caller, e.g., main form.
    Private ReadOnly da As New DataAccess()

    Public Sub New(projID As Integer)
        InitializeComponent() ' Call designer-generated setup
        ProjectID = projID
        LoadData()
    End Sub

    Private Sub UpdateStatus(message As String)
        Dim mdiParent As frmMain = TryCast(Me.MdiParent, frmMain)
        If mdiParent IsNot Nothing Then
            mdiParent.StatusLabel.Text = $"{message} at {DateTime.Now:HH:mm:ss}"
        Else
            Debug.WriteLine($"{message} at {DateTime.Now:HH:mm:ss}")
        End If
    End Sub
    Private Sub frmInclusionsExclusions_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.dgvLoads.Rows.Add("Roof")
        Me.dgvLoads.Rows.Add("Floor")
        Me.dgvLoads.Rows.Add("Corridor")

        Me.dgvRoofItems.Rows.Add(1, "Wood Roof Trusses", False, False, False)
        Me.dgvRoofItems.Rows.Add(2, "Valleys", False, False, False)
        Me.dgvRoofItems.Rows.Add(3, "End Gables", False, False, False)
        Me.dgvRoofItems.Rows.Add(4, "Party Wall Gables", False, False, False)
        Me.dgvRoofItems.Rows.Add(5, "Draftstop Gables", False, False, False)
        Me.dgvRoofItems.Rows.Add(6, "Shear Gables", False, False, False)
        Me.dgvRoofItems.Rows.Add(7, "Lay-in Gables", False, False, False)
        Me.dgvRoofItems.Rows.Add(8, "Drag Trusses", False, False, False)
        Me.dgvRoofItems.Rows.Add(9, "Integrated Truss Parapets", False, False, False)
        Me.dgvRoofItems.Rows.Add(10, "Exterior Blocking Panels", False, False, False)
        Me.dgvRoofItems.Rows.Add(11, "Interior Blocking Panels", False, False, False)
        Me.dgvRoofItems.Rows.Add(12, "Drag Blocking Panels", False, False, False)
        Me.dgvRoofItems.Rows.Add(13, "", False, False, False)
        Me.dgvRoofItems.Rows.Add(14, "Truss-to-Truss Connectors", False, False, False)
        Me.dgvRoofItems.Rows.Add(15, "Truss-to-Beam Connectors", False, False, False)
        Me.dgvRoofItems.Rows.Add(16, "Truss-to-Wall Connectors", False, False, False)
        Me.dgvRoofItems.Rows.Add(17, "Fire Treated Truss Materials", False, False, False)
        Me.dgvRoofItems.Rows.Add(18, "Treated Truss Materials", False, False, False)
        Me.dgvRoofItems.Rows.Add(19, "Stainless Steel Plates", False, False, False)
        Me.dgvRoofItems.Rows.Add(20, "Snow Drifts", False, False, False)
        Me.dgvRoofItems.Rows.Add(21, "Sprinkler Loads", False, False, False)
        Me.dgvRoofItems.Rows.Add(22, "Mechanical Loads", False, False, False)
        Me.dgvRoofItems.Rows.Add(23, "Solar Loads", False, False, False)
        Me.dgvRoofItems.Rows.Add(24, "Ceiling Vault/Tray/Other", False, False, False)

        Me.dgvFloorItems.Rows.Add(27, "Wood Floor Trusses", False, False, False)
        Me.dgvFloorItems.Rows.Add(28, "End Gables", False, False, False)
        Me.dgvFloorItems.Rows.Add(29, "Party Wall Gables", False, False, False)
        Me.dgvFloorItems.Rows.Add(30, "Draftstop Gables", False, False, False)
        Me.dgvFloorItems.Rows.Add(31, "Shear Gables", False, False, False)
        Me.dgvFloorItems.Rows.Add(32, "Exterior Blocking Panels", False, False, False)
        Me.dgvFloorItems.Rows.Add(33, "Interior Blocking Panels", False, False, False)
        Me.dgvFloorItems.Rows.Add(34, "Drag Blocking Panels", False, False, False)
        Me.dgvFloorItems.Rows.Add(35, "Corridor Trusses", False, False, False)
        Me.dgvFloorItems.Rows.Add(36, "Balcony Trusses", False, False, False)
        Me.dgvFloorItems.Rows.Add(37, "", False, False, False)
        Me.dgvFloorItems.Rows.Add(38, "", False, False, False)
        Me.dgvFloorItems.Rows.Add(39, "", False, False, False)
        Me.dgvFloorItems.Rows.Add(40, "Truss-to-Truss Connectors", False, False, False)
        Me.dgvFloorItems.Rows.Add(41, "Truss-to-Beam Connectors", False, False, False)
        Me.dgvFloorItems.Rows.Add(42, "Truss-to-Wall Connectors", False, False, False)
        Me.dgvFloorItems.Rows.Add(43, "Fire Treated Truss Materials", False, False, False)
        Me.dgvFloorItems.Rows.Add(44, "Firewall Hangers", False, False, False)
        Me.dgvFloorItems.Rows.Add(45, "Treated Truss Materials", False, False, False)
        Me.dgvFloorItems.Rows.Add(46, "Stainless Steel Plates", False, False, False)
        Me.dgvFloorItems.Rows.Add(47, "Snow Drifts", False, False, False)
        Me.dgvFloorItems.Rows.Add(48, "Sprinkler Loads", False, False, False)
        Me.dgvFloorItems.Rows.Add(49, "Mechanical Loads", False, False, False)
        Me.dgvFloorItems.Rows.Add(50, "On Edge Floors (Roof Style)", False, False, False)
        Me.dgvFloorItems.Rows.Add(51, "Stacked TC/BC Trusses", False, False, False)

        Me.dgvWallItems.Rows.Add(53, "Wood Wall Panels", False, False, False)
        Me.dgvWallItems.Rows.Add(54, "Wood Panels Below Grade", False, False, False)
        Me.dgvWallItems.Rows.Add(55, "Metal/Steel Framed Walls", False, False, False)
        Me.dgvWallItems.Rows.Add(56, "CMU Walls", False, False, False)
        Me.dgvWallItems.Rows.Add(57, "Round/Radius Walls", False, False, False)
        Me.dgvWallItems.Rows.Add(58, "Raked Walls", False, False, False)
        Me.dgvWallItems.Rows.Add(59, "Wood Sheathing", False, False, False)
        Me.dgvWallItems.Rows.Add(60, "Non-wood Sheathing", False, False, False)
        Me.dgvWallItems.Rows.Add(61, "Horizontal Seam Blocking", False, False, False)
        Me.dgvWallItems.Rows.Add(62, "Any other Blocking", False, False, False)
        Me.dgvWallItems.Rows.Add(63, "Loose VTP - Install on Site", False, False, False)
        Me.dgvWallItems.Rows.Add(64, "Loose VBP - Install on Site", False, False, False)
        Me.dgvWallItems.Rows.Add(65, "", False, False, False)
        Me.dgvWallItems.Rows.Add(66, "Treated Bottom Plate", False, False, False)
        Me.dgvWallItems.Rows.Add(67, "Anchor Hardware", False, False, False)
        Me.dgvWallItems.Rows.Add(68, "Panels over 10' Tall", False, False, False)
        Me.dgvWallItems.Rows.Add(69, "LSL Wall Material", False, False, False)
        Me.dgvWallItems.Rows.Add(70, "Wood Headers within Wall", False, False, False)
        Me.dgvWallItems.Rows.Add(71, "Wood Columns within Wall", False, False, False)
        Me.dgvWallItems.Rows.Add(72, "Walls outside Ext. Envelope", False, False, False)

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
            End If

            ' Load Loads
            Dim loads = da.GetProjectLoads(ProjectID)
            Dim rowIdx As Integer = 0
            For Each loadItem In loads
                If rowIdx < dgvLoads.Rows.Count Then
                    With dgvLoads.Rows(rowIdx)
                        .Cells(0).Value = loadItem.Category
                        .Cells(1).Value = loadItem.TCLL
                        .Cells(2).Value = loadItem.TCDL
                        .Cells(3).Value = loadItem.BCLL
                        .Cells(4).Value = loadItem.BCDL
                        .Cells(5).Value = loadItem.OCSpacing
                        .Cells(6).Value = loadItem.LiveLoadDeflection
                        .Cells(7).Value = loadItem.TotalLoadDeflection
                        .Cells(8).Value = loadItem.Absolute
                        .Cells(9).Value = loadItem.Deflection
                    End With
                    rowIdx += 1
                End If
            Next

            ' Load Bearing Styles
            Dim bearing = da.GetProjectBearingStyles(ProjectID)
            If bearing IsNot Nothing Then
                cmbExtWall.SelectedItem = bearing.ExtWallStyle
                txtExtRim.Text = bearing.ExtRimRibbon
                cmbIntWall.SelectedItem = bearing.IntWallStyle
                txtIntRim.Text = bearing.IntRimRibbon
                cmbCorridorWall.SelectedItem = bearing.CorridorWallStyle
                txtCorridorRim.Text = bearing.CorridorRimRibbon
            End If

            ' Load Notes
            Dim note = da.GetProjectGeneralNotes(ProjectID)
            If note IsNot Nothing Then
                txtGeneralNotes.Text = note.Notes
            End If

            ' Load Items (Roof, Floor, Wall)
            Dim items = da.GetProjectItems(ProjectID)
            For Each item In items
                Dim dgv As DataGridView = If(item.Section = "Roof", dgvRoofItems, If(item.Section = "Floor", dgvFloorItems, dgvWallItems))
                Dim row = dgv.Rows.Cast(Of DataGridViewRow).FirstOrDefault(Function(r) CInt(r.Cells("KN").Value) = item.KN)
                If row IsNot Nothing Then
                    row.Cells("Included").Value = item.Status = "y"
                    row.Cells("Excluded").Value = item.Status = "x"
                    row.Cells("Optional").Value = item.Status = "o"
                End If
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
            .WallHeights = txtWallHeights.Text
        }
            Dim existingInfo = da.GetProjectDesignInfo(ProjectID)
            If existingInfo IsNot Nothing Then info.InfoID = existingInfo.InfoID
            da.SaveProjectDesignInfo(info)

            ' Save Loads
            Dim loads As New List(Of ProjectLoadModel)
            For Each row As DataGridViewRow In dgvLoads.Rows
                loads.Add(New ProjectLoadModel With {
                .ProjectID = ProjectID,
                .Category = If(row.Cells("Category").Value Is Nothing, String.Empty, CStr(row.Cells("Category").Value)),
                .TCLL = If(row.Cells("TCLL").Value Is Nothing, String.Empty, CStr(row.Cells("TCLL").Value)),
                .TCDL = If(row.Cells("TCDL").Value Is Nothing, String.Empty, CStr(row.Cells("TCDL").Value)),
                .BCLL = If(row.Cells("BCLL").Value Is Nothing, String.Empty, CStr(row.Cells("BCLL").Value)),
                .BCDL = If(row.Cells("BCDL").Value Is Nothing, String.Empty, CStr(row.Cells("BCDL").Value)),
                .OCSpacing = If(row.Cells("OCSpacing").Value Is Nothing, String.Empty, CStr(row.Cells("OCSpacing").Value)),
                .LiveLoadDeflection = If(row.Cells("LiveDefl").Value Is Nothing, String.Empty, CStr(row.Cells("LiveDefl").Value)),
                .TotalLoadDeflection = If(row.Cells("TotalDefl").Value Is Nothing, String.Empty, CStr(row.Cells("TotalDefl").Value)),
                .Absolute = If(row.Cells("Absolute").Value Is Nothing, String.Empty, CStr(row.Cells("Absolute").Value)),
                .Deflection = If(row.Cells("Deflection").Value Is Nothing, String.Empty, CStr(row.Cells("Deflection").Value))
            })
            Next
            da.SaveProjectLoads(ProjectID, loads)

            ' Save Bearing Styles
            Dim bearing As New ProjectBearingStylesModel With {
            .ProjectID = ProjectID,
            .ExtWallStyle = If(cmbExtWall.SelectedItem Is Nothing, String.Empty, CStr(cmbExtWall.SelectedItem)),
            .ExtRimRibbon = txtExtRim.Text,
            .IntWallStyle = If(cmbIntWall.SelectedItem Is Nothing, String.Empty, CStr(cmbIntWall.SelectedItem)),
            .IntRimRibbon = txtIntRim.Text,
            .CorridorWallStyle = If(cmbCorridorWall.SelectedItem Is Nothing, String.Empty, CStr(cmbCorridorWall.SelectedItem)),
            .CorridorRimRibbon = txtCorridorRim.Text
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
            Dim items As New List(Of ProjectItemModel)
            For Each dgv In {dgvRoofItems, dgvFloorItems, dgvWallItems}
                Dim section As String = If(dgv Is dgvRoofItems, "Roof", If(dgv Is dgvFloorItems, "Floor", "Wall"))
                For Each row As DataGridViewRow In dgv.Rows
                    Dim status As String = If(CBool(row.Cells("Included").Value), "y", If(CBool(row.Cells("Excluded").Value), "x", If(CBool(row.Cells("Optional").Value), "o", "")))
                    items.Add(New ProjectItemModel With {
                    .ProjectID = ProjectID,
                    .Section = section,
                    .KN = CInt(row.Cells("KN").Value),
                    .Description = CStr(row.Cells("Desc").Value),
                    .Status = status
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


End Class