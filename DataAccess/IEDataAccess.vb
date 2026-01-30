Imports System.Data.SqlClient
Imports BuildersPSE2.Utilities

Namespace DataAccess


    Public Class IEDataAccess
        Public Shared Function GetProjectBearingStylesData(projectID As Integer) As DataTable
            Dim dt As New DataTable()
            dt.Columns.Add("BearingID", GetType(Integer))
            dt.Columns.Add("ProjectID", GetType(Integer))
            dt.Columns.Add("ExtWallStyle", GetType(String))
            dt.Columns.Add("ExtRimRibbon", GetType(String))
            dt.Columns.Add("IntWallStyle", GetType(String))
            dt.Columns.Add("IntRimRibbon", GetType(String))
            dt.Columns.Add("CorridorWallStyle", GetType(String))
            dt.Columns.Add("CorridorRimRibbon", GetType(String))

            Dim bearing As ProjectBearingStylesModel = GetProjectBearingStyles(projectID)
            If bearing IsNot Nothing Then
                dt.Rows.Add(bearing.BearingID, bearing.ProjectID, bearing.ExtWallStyle, bearing.ExtRimRibbon,
                            bearing.IntWallStyle, bearing.IntRimRibbon, bearing.CorridorWallStyle, bearing.CorridorRimRibbon)
            End If

            Return dt
        End Function

        Public Shared Function GetProjectLoadsData(projectID As Integer) As DataTable
            Dim dt As New DataTable()
            dt.Columns.Add("Category", GetType(String))
            dt.Columns.Add("TCLL", GetType(String))
            dt.Columns.Add("TCDL", GetType(String))
            dt.Columns.Add("BCLL", GetType(String))
            dt.Columns.Add("BCDL", GetType(String))
            dt.Columns.Add("OCSpacing", GetType(String))
            dt.Columns.Add("LiveLoadDeflection", GetType(String))
            dt.Columns.Add("TotalLoadDeflection", GetType(String))
            dt.Columns.Add("Absolute", GetType(String))
            dt.Columns.Add("Deflection", GetType(String))

            Dim loads As List(Of ProjectLoadModel) = GetProjectLoads(projectID) ' Reuse existing method
            For Each load As ProjectLoadModel In loads
                dt.Rows.Add(load.Category, load.TCLL, load.TCDL, load.BCLL, load.BCDL,
                            load.OCSpacing, load.LiveLoadDeflection, load.TotalLoadDeflection,
                            load.AbsoluteLL, load.AbsoluteTL)
            Next

            Return dt
        End Function
        Public Function GetProjectDesignInfo(projectID As Integer) As ProjectDesignInfoModel
            Dim info As ProjectDesignInfoModel = Nothing
            Dim params As SqlParameter() = {
                New SqlParameter("@ProjectID", projectID)
            }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectDesignInfo, params)
                                                                           If reader.Read() Then
                                                                               info = New ProjectDesignInfoModel With {
                                                                                   .InfoID = reader.GetInt32(reader.GetOrdinal("InfoID")),
                                                                                   .ProjectID = projectID,
                                                                                   .BuildingCode = If(Not reader.IsDBNull(reader.GetOrdinal("BuildingCode")), reader.GetString(reader.GetOrdinal("BuildingCode")), String.Empty),
                                                                                   .Importance = If(Not reader.IsDBNull(reader.GetOrdinal("Importance")), reader.GetString(reader.GetOrdinal("Importance")), String.Empty),
                                                                                   .ExposureCategory = If(Not reader.IsDBNull(reader.GetOrdinal("ExposureCategory")), reader.GetString(reader.GetOrdinal("ExposureCategory")), String.Empty),
                                                                                   .WindSpeed = If(Not reader.IsDBNull(reader.GetOrdinal("WindSpeed")), reader.GetString(reader.GetOrdinal("WindSpeed")), String.Empty),
                                                                                   .SnowLoadType = If(Not reader.IsDBNull(reader.GetOrdinal("SnowLoadType")), reader.GetString(reader.GetOrdinal("SnowLoadType")), String.Empty),
                                                                                   .OccupancyCategory = If(Not reader.IsDBNull(reader.GetOrdinal("OccupancyCategory")), reader.GetString(reader.GetOrdinal("OccupancyCategory")), String.Empty),
                                                                                   .RoofPitches = If(Not reader.IsDBNull(reader.GetOrdinal("RoofPitches")), reader.GetString(reader.GetOrdinal("RoofPitches")), String.Empty),
                                                                                   .FloorDepths = If(Not reader.IsDBNull(reader.GetOrdinal("FloorDepths")), reader.GetString(reader.GetOrdinal("FloorDepths")), String.Empty),
                                                                                   .WallHeights = If(Not reader.IsDBNull(reader.GetOrdinal("WallHeights")), reader.GetString(reader.GetOrdinal("WallHeights")), String.Empty),
                                                                                   .HeelHeights = If(Not reader.IsDBNull(reader.GetOrdinal("HeelHeights")), reader.GetString(reader.GetOrdinal("HeelHeights")), String.Empty)
                                                                               }
                                                                           End If
                                                                       End Using
                                                                   End Sub, "Error loading ProjectDesignInfo for ProjectID " & projectID)
            Return info
        End Function

        Public Sub SaveProjectDesignInfo(info As ProjectDesignInfoModel)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@ProjectID", info.ProjectID},
                {"@BuildingCode", If(String.IsNullOrEmpty(info.BuildingCode), DBNull.Value, CObj(info.BuildingCode))},
                {"@Importance", If(String.IsNullOrEmpty(info.Importance), DBNull.Value, CObj(info.Importance))},
                {"@ExposureCategory", If(String.IsNullOrEmpty(info.ExposureCategory), DBNull.Value, CObj(info.ExposureCategory))},
                {"@WindSpeed", If(String.IsNullOrEmpty(info.WindSpeed), DBNull.Value, CObj(info.WindSpeed))},
                {"@SnowLoadType", If(String.IsNullOrEmpty(info.SnowLoadType), DBNull.Value, CObj(info.SnowLoadType))},
                {"@OccupancyCategory", If(String.IsNullOrEmpty(info.OccupancyCategory), DBNull.Value, CObj(info.OccupancyCategory))},
                {"@RoofPitches", If(String.IsNullOrEmpty(info.RoofPitches), DBNull.Value, CObj(info.RoofPitches))},
                {"@FloorDepths", If(String.IsNullOrEmpty(info.FloorDepths), DBNull.Value, CObj(info.FloorDepths))},
                {"@WallHeights", If(String.IsNullOrEmpty(info.WallHeights), DBNull.Value, CObj(info.WallHeights))},
                {"@HeelHeights", If(String.IsNullOrEmpty(info.HeelHeights), DBNull.Value, CObj(info.HeelHeights))}
            }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If info.InfoID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectDesignInfo, HelperDataAccess.BuildParameters(paramsDict))
                                                                           info.InfoID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@InfoID", info.InfoID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectDesignInfo, HelperDataAccess.BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving ProjectDesignInfo for ProjectID " & info.ProjectID)
        End Sub

        Public Shared Function GetProjectLoads(projectID As Integer) As List(Of ProjectLoadModel)
            Dim loads As New List(Of ProjectLoadModel)
            Dim params As SqlParameter() = {
                New SqlParameter("@ProjectID", projectID)
            }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectLoads, params)
                                                                           While reader.Read()
                                                                               Dim load As New ProjectLoadModel With {
                                                                                   .LoadID = reader.GetInt32(reader.GetOrdinal("LoadID")),
                                                                                   .ProjectID = projectID,
                                                                                   .Category = If(Not reader.IsDBNull(reader.GetOrdinal("Category")), reader.GetString(reader.GetOrdinal("Category")), String.Empty),
                                                                                   .TCLL = If(Not reader.IsDBNull(reader.GetOrdinal("TCLL")), reader.GetString(reader.GetOrdinal("TCLL")), String.Empty),
                                                                                   .TCDL = If(Not reader.IsDBNull(reader.GetOrdinal("TCDL")), reader.GetString(reader.GetOrdinal("TCDL")), String.Empty),
                                                                                   .BCLL = If(Not reader.IsDBNull(reader.GetOrdinal("BCLL")), reader.GetString(reader.GetOrdinal("BCLL")), String.Empty),
                                                                                   .BCDL = If(Not reader.IsDBNull(reader.GetOrdinal("BCDL")), reader.GetString(reader.GetOrdinal("BCDL")), String.Empty),
                                                                                   .OCSpacing = If(Not reader.IsDBNull(reader.GetOrdinal("OCSpacing")), reader.GetString(reader.GetOrdinal("OCSpacing")), String.Empty),
                                                                                   .LiveLoadDeflection = If(Not reader.IsDBNull(reader.GetOrdinal("LiveLoadDeflection")), reader.GetString(reader.GetOrdinal("LiveLoadDeflection")), String.Empty),
                                                                                   .TotalLoadDeflection = If(Not reader.IsDBNull(reader.GetOrdinal("TotalLoadDeflection")), reader.GetString(reader.GetOrdinal("TotalLoadDeflection")), String.Empty),
                                                                                   .AbsoluteLL = If(Not reader.IsDBNull(reader.GetOrdinal("AbsoluteLL")), reader.GetString(reader.GetOrdinal("AbsoluteLL")), String.Empty),
                                                                                   .AbsoluteTL = If(Not reader.IsDBNull(reader.GetOrdinal("AbsoluteTL")), reader.GetString(reader.GetOrdinal("AbsoluteTL")), String.Empty)
                                                                               }
                                                                               loads.Add(load)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading ProjectLoads for ProjectID " & projectID)
            Return loads
        End Function

        Public Sub SaveProjectLoads(projectID As Integer, loads As List(Of ProjectLoadModel))
            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                               ' Delete existing loads
                                                                               Dim deleteParams As SqlParameter() = {
                                                                                   New SqlParameter("@ProjectID", projectID)
                                                                               }
                                                                               SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteProjectLoads, deleteParams, conn, transaction)

                                                                               ' Insert new loads
                                                                               For Each load In loads
                                                                                   Dim paramsDict As New Dictionary(Of String, Object) From {
                                                                                       {"@ProjectID", projectID},
                                                                                       {"@Category", If(String.IsNullOrEmpty(load.Category), DBNull.Value, CObj(load.Category))},
                                                                                       {"@TCLL", If(String.IsNullOrEmpty(load.TCLL), DBNull.Value, CObj(load.TCLL))},
                                                                                       {"@TCDL", If(String.IsNullOrEmpty(load.TCDL), DBNull.Value, CObj(load.TCDL))},
                                                                                       {"@BCLL", If(String.IsNullOrEmpty(load.BCLL), DBNull.Value, CObj(load.BCLL))},
                                                                                       {"@BCDL", If(String.IsNullOrEmpty(load.BCDL), DBNull.Value, CObj(load.BCDL))},
                                                                                       {"@OCSpacing", If(String.IsNullOrEmpty(load.OCSpacing), DBNull.Value, CObj(load.OCSpacing))},
                                                                                       {"@LiveLoadDeflection", If(String.IsNullOrEmpty(load.LiveLoadDeflection), DBNull.Value, CObj(load.LiveLoadDeflection))},
                                                                                       {"@TotalLoadDeflection", If(String.IsNullOrEmpty(load.TotalLoadDeflection), DBNull.Value, CObj(load.TotalLoadDeflection))},
                                                                                       {"@AbsoluteLL", If(String.IsNullOrEmpty(load.AbsoluteLL), DBNull.Value, CObj(load.AbsoluteLL))},
                                                                                       {"@AbsoluteTL", If(String.IsNullOrEmpty(load.AbsoluteTL), DBNull.Value, CObj(load.AbsoluteTL))}
                                                                                   }
                                                                                   Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(Queries.InsertProjectLoad, HelperDataAccess.BuildParameters(paramsDict), conn, transaction)
                                                                                   load.LoadID = CInt(newIDObj)
                                                                               Next
                                                                               transaction.Commit()
                                                                           End Sub, "Error saving ProjectLoads for ProjectID " & projectID)
                End Using
            End Using
        End Sub

        Public Shared Function GetProjectBearingStyles(projectID As Integer) As ProjectBearingStylesModel
            Dim bearing As ProjectBearingStylesModel = Nothing
            Dim params As SqlParameter() = {
                New SqlParameter("@ProjectID", projectID)
            }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectBearingStyles, params)
                                                                           If reader.Read() Then
                                                                               bearing = New ProjectBearingStylesModel With {
                                                                                   .BearingID = reader.GetInt32(reader.GetOrdinal("BearingID")),
                                                                                   .ProjectID = projectID,
                                                                                   .ExtWallStyle = If(Not reader.IsDBNull(reader.GetOrdinal("ExtWallStyle")), reader.GetString(reader.GetOrdinal("ExtWallStyle")), String.Empty),
                                                                                   .ExtRimRibbon = If(Not reader.IsDBNull(reader.GetOrdinal("ExtRimRibbon")), reader.GetString(reader.GetOrdinal("ExtRimRibbon")), String.Empty),
                                                                                   .IntWallStyle = If(Not reader.IsDBNull(reader.GetOrdinal("IntWallStyle")), reader.GetString(reader.GetOrdinal("IntWallStyle")), String.Empty),
                                                                                   .IntRimRibbon = If(Not reader.IsDBNull(reader.GetOrdinal("IntRimRibbon")), reader.GetString(reader.GetOrdinal("IntRimRibbon")), String.Empty),
                                                                                   .CorridorWallStyle = If(Not reader.IsDBNull(reader.GetOrdinal("CorridorWallStyle")), reader.GetString(reader.GetOrdinal("CorridorWallStyle")), String.Empty),
                                                                                   .CorridorRimRibbon = If(Not reader.IsDBNull(reader.GetOrdinal("CorridorRimRibbon")), reader.GetString(reader.GetOrdinal("CorridorRimRibbon")), String.Empty)
                                                                               }
                                                                           End If
                                                                       End Using
                                                                   End Sub, "Error loading ProjectBearingStyles for ProjectID " & projectID)
            Return bearing
        End Function

        Public Sub SaveProjectBearingStyles(bearing As ProjectBearingStylesModel)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@ProjectID", bearing.ProjectID},
                {"@ExtWallStyle", If(String.IsNullOrEmpty(bearing.ExtWallStyle), DBNull.Value, CObj(bearing.ExtWallStyle))},
                {"@ExtRimRibbon", If(String.IsNullOrEmpty(bearing.ExtRimRibbon), DBNull.Value, CObj(bearing.ExtRimRibbon))},
                {"@IntWallStyle", If(String.IsNullOrEmpty(bearing.IntWallStyle), DBNull.Value, CObj(bearing.IntWallStyle))},
                {"@IntRimRibbon", If(String.IsNullOrEmpty(bearing.IntRimRibbon), DBNull.Value, CObj(bearing.IntRimRibbon))},
                {"@CorridorWallStyle", If(String.IsNullOrEmpty(bearing.CorridorWallStyle), DBNull.Value, CObj(bearing.CorridorWallStyle))},
                {"@CorridorRimRibbon", If(String.IsNullOrEmpty(bearing.CorridorRimRibbon), DBNull.Value, CObj(bearing.CorridorRimRibbon))}
            }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If bearing.BearingID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectBearingStyles, HelperDataAccess.BuildParameters(paramsDict))
                                                                           bearing.BearingID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@BearingID", bearing.BearingID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectBearingStyles, HelperDataAccess.BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving ProjectBearingStyles for ProjectID " & bearing.ProjectID)
        End Sub

        Public Function GetProjectGeneralNotes(projectID As Integer) As ProjectGeneralNotesModel
            Dim note As ProjectGeneralNotesModel = Nothing
            Dim params As SqlParameter() = {
                New SqlParameter("@ProjectID", projectID)
            }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectGeneralNotes, params)
                                                                           If reader.Read() Then
                                                                               note = New ProjectGeneralNotesModel With {
                                                                                   .NoteID = reader.GetInt32(reader.GetOrdinal("NoteID")),
                                                                                   .ProjectID = projectID,
                                                                                   .Notes = If(Not reader.IsDBNull(reader.GetOrdinal("Notes")), reader.GetString(reader.GetOrdinal("Notes")), String.Empty)
                                                                               }
                                                                           End If
                                                                       End Using
                                                                   End Sub, "Error loading ProjectGeneralNotes for ProjectID " & projectID)
            Return note
        End Function

        Public Sub SaveProjectGeneralNotes(note As ProjectGeneralNotesModel)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@ProjectID", note.ProjectID},
                {"@Notes", If(String.IsNullOrEmpty(note.Notes), DBNull.Value, CObj(note.Notes))}
            }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If note.NoteID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectGeneralNotes, HelperDataAccess.BuildParameters(paramsDict))
                                                                           note.NoteID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@NoteID", note.NoteID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectGeneralNotes, HelperDataAccess.BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving ProjectGeneralNotes for ProjectID " & note.ProjectID)
        End Sub

        Public Shared Function GetProjectItems(projectID As Integer) As List(Of ProjectItemModel)
            Dim items As New List(Of ProjectItemModel)
            Dim params As SqlParameter() = {
                New SqlParameter("@ProjectID", projectID)
            }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectItems, params)
                                                                           While reader.Read()
                                                                               Dim item As New ProjectItemModel With {
                                                                                   .ItemID = reader.GetInt32(reader.GetOrdinal("ItemID")),
                                                                                   .ProjectID = projectID,
                                                                                   .Section = If(Not reader.IsDBNull(reader.GetOrdinal("Section")), reader.GetString(reader.GetOrdinal("Section")), String.Empty),
                                                                                   .KN = reader.GetInt32(reader.GetOrdinal("KN")),
                                                                                   .Description = If(Not reader.IsDBNull(reader.GetOrdinal("Description")), reader.GetString(reader.GetOrdinal("Description")), String.Empty),
                                                                                   .Status = If(Not reader.IsDBNull(reader.GetOrdinal("Status")), reader.GetString(reader.GetOrdinal("Status")), String.Empty),
                                                                                   .Note = If(Not reader.IsDBNull(reader.GetOrdinal("Note")), reader.GetString(reader.GetOrdinal("Note")), String.Empty)
                                                                               }
                                                                               items.Add(item)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading ProjectItems for ProjectID " & projectID)
            Return items
        End Function

        Public Sub SaveProjectItems(projectID As Integer, items As List(Of ProjectItemModel))
            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                               ' Delete existing items
                                                                               Dim deleteParams As SqlParameter() = {
                                                                   New SqlParameter("@ProjectID", projectID)
                                                               }
                                                                               SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteProjectItems, deleteParams, conn, transaction)
                                                                               ' Insert new items
                                                                               For Each item In items
                                                                                   Dim paramsDict As New Dictionary(Of String, Object) From {
                                                                       {"@ProjectID", projectID},
                                                                       {"@Section", If(String.IsNullOrEmpty(item.Section), DBNull.Value, CObj(item.Section))},
                                                                       {"@KN", item.KN},
                                                                       {"@Description", If(String.IsNullOrEmpty(item.Description), DBNull.Value, CObj(item.Description))},
                                                                       {"@Status", If(String.IsNullOrEmpty(item.Status), DBNull.Value, CObj(item.Status))},
                                                                       {"@Note", If(String.IsNullOrEmpty(item.Note), DBNull.Value, CObj(item.Note))}
                                                                   }
                                                                                   Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(Queries.InsertProjectItem, HelperDataAccess.BuildParameters(paramsDict), conn, transaction)
                                                                                   item.ItemID = CInt(newIDObj)
                                                                               Next
                                                                               transaction.Commit()
                                                                           End Sub, "Error saving ProjectItems for ProjectID " & projectID)
                End Using
            End Using
        End Sub
        Public Function GetComboOptions(category As String) As List(Of String)
            Dim options As New List(Of String)
            Dim params As SqlParameter() = {New SqlParameter("@Category", category)}
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectComboOptions, params)
                                                                           While reader.Read()
                                                                               options.Add(reader.GetString(reader.GetOrdinal("Value")))
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading combo options for category " & category)
            Return options
        End Function

        Public Function GetItemOptions(section As String) As List(Of Tuple(Of Integer, String))
            Dim options As New List(Of Tuple(Of Integer, String))
            Dim params As SqlParameter() = {New SqlParameter("@Section", section)}
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectItemOptions, params)
                                                                           While reader.Read()
                                                                               options.Add(Tuple.Create(reader.GetInt32(reader.GetOrdinal("KN")), reader.GetString(reader.GetOrdinal("Description"))))
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading item options for section " & section)
            Return options
        End Function


    End Class
End Namespace