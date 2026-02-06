Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.Utilities

Namespace DataAccess
    ''' <summary>
    ''' Data access layer for Project-related operations including Projects, Buildings, Levels,
    ''' ActualUnits, RawUnits, and their related entities.
    ''' Uses TableOperations for all insert/update operations to ensure single source of truth.
    ''' </summary>
    Public Class ProjectDataAccess

#Region "Project Operations"

        ''' <summary>
        ''' Saves a project (insert or update). Validates ArchitectID and EngineerID CustomerTypes.
        ''' </summary>
        ''' <param name="proj">The project model to save.</param>
        ''' <exception cref="ArgumentException">Thrown if ArchitectID or EngineerID reference invalid customer types.</exception>
        Public Sub SaveProject(proj As ProjectModel)
            ' Validate CustomerType for ArchitectID (2) and EngineerID (3)
            If proj.ArchitectID.HasValue AndAlso Not HelperDataAccess.ValidateCustomerType(proj.ArchitectID.Value, 2) Then
                Throw New ArgumentException("ArchitectID must reference a customer with CustomerType=2 (Architect).")
            End If
            If proj.EngineerID.HasValue AndAlso Not HelperDataAccess.ValidateCustomerType(proj.EngineerID.Value, 3) Then
                Throw New ArgumentException("EngineerID must reference a customer with CustomerType=3 (Engineer).")
            End If

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   proj.ProjectID = TableOperations.UpsertProject(proj, conn, tran)
                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error saving project " & If(proj.ProjectID = 0, "(new)", proj.ProjectID.ToString()))
        End Sub

        ''' <summary>
        ''' Gets all projects with optional detail loading (buildings and settings).
        ''' </summary>
        ''' <param name="includeDetails">If true, loads buildings and settings for latest version.</param>
        ''' <returns>List of ProjectModel objects.</returns>
        Public Function GetProjects(Optional includeDetails As Boolean = True) As List(Of ProjectModel)
            Dim projects As New List(Of ProjectModel)

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjects)
                                                                           While reader.Read()
                                                                               Dim proj As ProjectModel = MapProjectFromReader(reader)

                                                                               If includeDetails Then
                                                                                   Dim versions = ProjVersionDataAccess.GetProjectVersions(proj.ProjectID)
                                                                                   If versions.Any() Then
                                                                                       Dim latestVersionID = versions.OrderByDescending(Function(v) v.VersionDate).First().VersionID
                                                                                       proj.Buildings = GetBuildingsByVersionID(latestVersionID)
                                                                                       proj.Settings = GetProjectProductSettings(latestVersionID)
                                                                                   End If
                                                                               End If

                                                                               projects.Add(proj)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading projects")

            Return projects
        End Function

        ''' <summary>
        ''' Gets a single project by ID with buildings and settings.
        ''' </summary>
        ''' <param name="projectID">The project ID to retrieve.</param>
        ''' <returns>ProjectModel or Nothing if not found.</returns>
        Public Function GetProjectByID(projectID As Integer) As ProjectModel
            Dim proj As ProjectModel = Nothing
            Dim param As New SqlParameter("@ProjectID", projectID)

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectByID, {param})
                                                                           If reader.Read() Then
                                                                               proj = MapProjectFromReader(reader)

                                                                               Dim versions = ProjVersionDataAccess.GetProjectVersions(proj.ProjectID)
                                                                               If versions.Any() Then
                                                                                   Dim latestVersionID = versions.OrderByDescending(Function(v) v.VersionDate).First().VersionID
                                                                                   proj.Buildings = GetBuildingsByVersionID(latestVersionID)
                                                                                   proj.Settings = GetProjectProductSettings(latestVersionID)
                                                                               End If
                                                                           End If
                                                                       End Using
                                                                   End Sub, "Error loading project by ID " & projectID)

            Return proj
        End Function

        ''' <summary>
        ''' Deletes a project and all related data (versions, buildings, levels, units, etc.).
        ''' </summary>
        ''' <param name="projectID">The project ID to delete.</param>
        ''' <param name="notificationMessage">Output message indicating success or failure.</param>
        Public Sub DeleteProject(projectID As Integer, ByRef notificationMessage As String)
            Dim localNotification As String = String.Empty
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using transaction As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   ' Fetch all VersionIDs for the project
                                                                                   Dim versionIDs As New List(Of Integer)
                                                                                   Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReaderTransactional(
                                                                               Queries.SelectProjectVersions, {New SqlParameter("@ProjectID", projectID)}, conn, transaction)
                                                                                       While reader.Read()
                                                                                           versionIDs.Add(reader.GetInt32(reader.GetOrdinal("VersionID")))
                                                                                       End While
                                                                                   End Using

                                                                                   ' Delete version-related data in FK-safe order
                                                                                   For Each vid As Integer In versionIDs
                                                                                       DeleteVersionRelatedData(vid, conn, transaction)
                                                                                   Next

                                                                                   ' Delete project versions - NEW PARAMETER INSTANCE
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                               Queries.DeleteProjectVersionsByProject, {New SqlParameter("@ProjectID", projectID)}, conn, transaction)

                                                                                   ' Delete direct project-related data - NEW PARAMETER INSTANCES FOR EACH
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                               Queries.DeleteProjectDesignInfoByProject, {New SqlParameter("@ProjectID", projectID)}, conn, transaction)
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                               Queries.DeleteProjectLoadsByProject, {New SqlParameter("@ProjectID", projectID)}, conn, transaction)
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                               Queries.DeleteProjectBearingStylesByProject, {New SqlParameter("@ProjectID", projectID)}, conn, transaction)
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                               Queries.DeleteProjectGeneralNotesByProject, {New SqlParameter("@ProjectID", projectID)}, conn, transaction)
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                               Queries.DeleteProjectItemsByProject, {New SqlParameter("@ProjectID", projectID)}, conn, transaction)

                                                                                   ' Finally, delete the project - NEW PARAMETER INSTANCE
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                               Queries.DeleteProjectByID, {New SqlParameter("@ProjectID", projectID)}, conn, transaction)

                                                                                   transaction.Commit()
                                                                                   localNotification = $"Project {projectID} was successfully deleted."
                                                                               Catch ex As Exception
                                                                                   transaction.Rollback()
                                                                                   localNotification = String.Empty
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error deleting project " & projectID)
            notificationMessage = localNotification
        End Sub

        ''' <summary>Maps a SqlDataReader row to a ProjectModel.</summary>
        Private Function MapProjectFromReader(reader As SqlDataReader) As ProjectModel
            Return New ProjectModel With {
                .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                .JBID = If(reader.IsDBNull(reader.GetOrdinal("JBID")), String.Empty, reader.GetString(reader.GetOrdinal("JBID"))),
                .ProjectType = New ProjectTypeModel With {
                    .ProjectTypeID = reader.GetInt32(reader.GetOrdinal("ProjectTypeID")),
                    .ProjectTypeName = If(reader.IsDBNull(reader.GetOrdinal("ProjectType")), String.Empty, reader.GetString(reader.GetOrdinal("ProjectType")))
                },
                .ProjectName = If(reader.IsDBNull(reader.GetOrdinal("ProjectName")), String.Empty, reader.GetString(reader.GetOrdinal("ProjectName"))),
                .Estimator = New EstimatorModel With {
                    .EstimatorID = reader.GetInt32(reader.GetOrdinal("EstimatorID")),
                    .EstimatorName = If(reader.IsDBNull(reader.GetOrdinal("Estimator")), String.Empty, reader.GetString(reader.GetOrdinal("Estimator")))
                },
                .Address = If(reader.IsDBNull(reader.GetOrdinal("Address")), String.Empty, reader.GetString(reader.GetOrdinal("Address"))),
                .City = If(reader.IsDBNull(reader.GetOrdinal("City")), String.Empty, reader.GetString(reader.GetOrdinal("City"))),
                .State = If(reader.IsDBNull(reader.GetOrdinal("State")), String.Empty, reader.GetString(reader.GetOrdinal("State"))),
                .Zip = If(reader.IsDBNull(reader.GetOrdinal("Zip")), String.Empty, reader.GetString(reader.GetOrdinal("Zip"))),
                .BidDate = If(reader.IsDBNull(reader.GetOrdinal("BidDate")), CType(Nothing, Date?), reader.GetDateTime(reader.GetOrdinal("BidDate"))),
                .ArchPlansDated = If(reader.IsDBNull(reader.GetOrdinal("ArchPlansDated")), CType(Nothing, Date?), reader.GetDateTime(reader.GetOrdinal("ArchPlansDated"))),
                .EngPlansDated = If(reader.IsDBNull(reader.GetOrdinal("EngPlansDated")), CType(Nothing, Date?), reader.GetDateTime(reader.GetOrdinal("EngPlansDated"))),
                .MilesToJobSite = If(reader.IsDBNull(reader.GetOrdinal("MilesToJobSite")), 0, reader.GetInt32(reader.GetOrdinal("MilesToJobSite"))),
                .TotalNetSqft = If(reader.IsDBNull(reader.GetOrdinal("TotalNetSqft")), CType(Nothing, Integer?), reader.GetInt32(reader.GetOrdinal("TotalNetSqft"))),
                .TotalGrossSqft = If(reader.IsDBNull(reader.GetOrdinal("TotalGrossSqft")), 0, reader.GetInt32(reader.GetOrdinal("TotalGrossSqft"))),
                .ArchitectID = If(reader.IsDBNull(reader.GetOrdinal("ArchitectID")), CType(Nothing, Integer?), reader.GetInt32(reader.GetOrdinal("ArchitectID"))),
                .EngineerID = If(reader.IsDBNull(reader.GetOrdinal("EngineerID")), CType(Nothing, Integer?), reader.GetInt32(reader.GetOrdinal("EngineerID"))),
                .ProjectNotes = If(reader.IsDBNull(reader.GetOrdinal("ProjectNotes")), String.Empty, reader.GetString(reader.GetOrdinal("ProjectNotes"))),
                .LastModifiedDate = If(reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")), Date.MinValue, reader.GetDateTime(reader.GetOrdinal("LastModifiedDate"))),
                .CreatedDate = If(reader.IsDBNull(reader.GetOrdinal("createddate")), Date.MinValue, reader.GetDateTime(reader.GetOrdinal("createddate"))),
                .ArchitectName = If(reader.IsDBNull(reader.GetOrdinal("ArchitectName")), String.Empty, reader.GetString(reader.GetOrdinal("ArchitectName"))),
                .EngineerName = If(reader.IsDBNull(reader.GetOrdinal("EngineerName")), String.Empty, reader.GetString(reader.GetOrdinal("EngineerName")))
            }
        End Function

        ''' <summary>Helper to delete all version-related data in FK-safe order.</summary>
        Private Sub DeleteVersionRelatedData(versionID As Integer, conn As SqlConnection, transaction As SqlTransaction)
            ' Create NEW parameter instance for each query to avoid SqlParameterCollection error
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
        Queries.DeleteActualToLevelMappingsByVersion, {New SqlParameter("@VersionID", versionID)}, conn, transaction)
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
        Queries.DeleteCalculatedComponentsByVersion, {New SqlParameter("@VersionID", versionID)}, conn, transaction)
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
        Queries.DeleteActualUnitsByVersion, {New SqlParameter("@VersionID", versionID)}, conn, transaction)
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
        Queries.DeleteRawUnitsByVersion, {New SqlParameter("@VersionID", versionID)}, conn, transaction)
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
        Queries.DeleteLevelsByVersion, {New SqlParameter("@VersionID", versionID)}, conn, transaction)
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
        Queries.DeleteBuildingsByVersion, {New SqlParameter("@VersionID", versionID)}, conn, transaction)
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
        Queries.DeleteProjectProductSettingsByVersion, {New SqlParameter("@VersionID", versionID)}, conn, transaction)
        End Sub

#End Region

#Region "Building Operations"

        ''' <summary>
        ''' Gets all buildings for a specific version.
        ''' </summary>
        ''' <param name="versionID">The version ID to query.</param>
        ''' <returns>List of BuildingModel objects with their levels.</returns>
        Public Shared Function GetBuildingsByVersionID(versionID As Integer) As List(Of BuildingModel)
            Dim buildings As New List(Of BuildingModel)
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID)}

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectBuildingsByVersionID, params)
                                                                           While reader.Read()
                                                                               Dim bldg As New BuildingModel With {
                                                                                   .BuildingID = reader.GetInt32(reader.GetOrdinal("BuildingID")),
                                                                                   .BuildingName = If(Not reader.IsDBNull(reader.GetOrdinal("BuildingName")), reader.GetString(reader.GetOrdinal("BuildingName")), String.Empty),
                                                                                   .BuildingType = If(Not reader.IsDBNull(reader.GetOrdinal("BuildingType")), reader.GetInt32(reader.GetOrdinal("BuildingType")), Nothing),
                                                                                   .ResUnits = If(Not reader.IsDBNull(reader.GetOrdinal("ResUnits")), reader.GetInt32(reader.GetOrdinal("ResUnits")), Nothing),
                                                                                   .BldgQty = reader.GetInt32(reader.GetOrdinal("BldgQty")),
                                                                                   .FloorCostPerBldg = If(Not reader.IsDBNull(reader.GetOrdinal("FloorCostPerBldg")), reader.GetDecimal(reader.GetOrdinal("FloorCostPerBldg")), Nothing),
                                                                                   .RoofCostPerBldg = If(Not reader.IsDBNull(reader.GetOrdinal("RoofCostPerBldg")), reader.GetDecimal(reader.GetOrdinal("RoofCostPerBldg")), Nothing),
                                                                                   .WallCostPerBldg = If(Not reader.IsDBNull(reader.GetOrdinal("WallCostPerBldg")), reader.GetDecimal(reader.GetOrdinal("WallCostPerBldg")), Nothing),
                                                                                   .ExtendedFloorCost = If(Not reader.IsDBNull(reader.GetOrdinal("ExtendedFloorCost")), reader.GetDecimal(reader.GetOrdinal("ExtendedFloorCost")), Nothing),
                                                                                   .ExtendedRoofCost = If(Not reader.IsDBNull(reader.GetOrdinal("ExtendedRoofCost")), reader.GetDecimal(reader.GetOrdinal("ExtendedRoofCost")), Nothing),
                                                                                   .ExtendedWallCost = If(Not reader.IsDBNull(reader.GetOrdinal("ExtendedWallCost")), reader.GetDecimal(reader.GetOrdinal("ExtendedWallCost")), Nothing),
                                                                                   .OverallPrice = If(Not reader.IsDBNull(reader.GetOrdinal("OverallPrice")), reader.GetDecimal(reader.GetOrdinal("OverallPrice")), Nothing),
                                                                                   .OverallCost = If(Not reader.IsDBNull(reader.GetOrdinal("OverallCost")), reader.GetDecimal(reader.GetOrdinal("OverallCost")), Nothing)
                                                                               }
                                                                               bldg.Levels = GetLevelsByBuildingID(bldg.BuildingID)
                                                                               buildings.Add(bldg)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading buildings for version " & versionID)

            Return buildings
        End Function

        ''' <summary>
        ''' Saves a building (insert or update) using TableOperations, then saves its levels.
        ''' </summary>
        ''' <param name="bldg">The building model to save.</param>
        ''' <param name="versionID">The version ID this building belongs to.</param>
        Public Shared Sub SaveBuilding(bldg As BuildingModel, versionID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   bldg.BuildingID = TableOperations.UpsertBuilding(bldg, versionID, conn, tran)

                                                                                   For Each level In bldg.Levels
                                                                                       level.LevelID = TableOperations.UpsertLevel(level, bldg.BuildingID, versionID, conn, tran)
                                                                                   Next

                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error saving building")
        End Sub

        ''' <summary>
        ''' Deletes a building and all its levels (cascading delete).
        ''' </summary>
        ''' <param name="buildingID">The building ID to delete.</param>
        Public Sub DeleteBuilding(buildingID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim levels As List(Of LevelModel) = GetLevelsByBuildingID(buildingID)
                                                                       For Each level In levels
                                                                           DeleteLevel(level.LevelID)
                                                                       Next
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteBuilding, {New SqlParameter("@BuildingID", buildingID)})
                                                                   End Sub, "Error deleting building " & buildingID)
        End Sub

        ''' <summary>Gets the BuildingID for a given LevelID.</summary>
        Public Function GetBuildingIDByLevelID(levelID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
            Dim buildingIDObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       buildingIDObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.GetBuildingIDByLevelID, params)
                                                                   End Sub, "Error fetching BuildingID for LevelID " & levelID)
            Return If(buildingIDObj Is DBNull.Value OrElse buildingIDObj Is Nothing, 0, CInt(buildingIDObj))
        End Function

#End Region

#Region "Level Operations"

        ''' <summary>
        ''' Gets all levels for a specific building.
        ''' </summary>
        ''' <param name="buildingID">The building ID to query.</param>
        ''' <returns>List of LevelModel objects.</returns>
        Public Shared Function GetLevelsByBuildingID(buildingID As Integer) As List(Of LevelModel)
            Dim levels As New List(Of LevelModel)
            Dim params As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectLevelsByBuilding, params)
                                                                           While reader.Read()
                                                                               levels.Add(New LevelModel With {
                                                                                   .LevelID = reader.GetInt32(reader.GetOrdinal("LevelID")),
                                                                                   .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID")),
                                                                                   .BuildingID = buildingID,
                                                                                   .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                                                                   .ProductTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("ProductTypeName")), reader.GetString(reader.GetOrdinal("ProductTypeName")), String.Empty),
                                                                                   .LevelNumber = reader.GetInt32(reader.GetOrdinal("LevelNumber")),
                                                                                   .LevelName = reader.GetString(reader.GetOrdinal("LevelName")),
                                                                                   .OverallSQFT = If(Not reader.IsDBNull(reader.GetOrdinal("OverallSQFT")), reader.GetDecimal(reader.GetOrdinal("OverallSQFT")), Nothing),
                                                                                   .OverallLF = If(Not reader.IsDBNull(reader.GetOrdinal("OverallLF")), reader.GetDecimal(reader.GetOrdinal("OverallLF")), Nothing),
                                                                                   .OverallBDFT = If(Not reader.IsDBNull(reader.GetOrdinal("OverallBDFT")), reader.GetDecimal(reader.GetOrdinal("OverallBDFT")), Nothing),
                                                                                   .LumberCost = If(Not reader.IsDBNull(reader.GetOrdinal("LumberCost")), reader.GetDecimal(reader.GetOrdinal("LumberCost")), Nothing),
                                                                                   .PlateCost = If(Not reader.IsDBNull(reader.GetOrdinal("PlateCost")), reader.GetDecimal(reader.GetOrdinal("PlateCost")), Nothing),
                                                                                   .LaborCost = If(Not reader.IsDBNull(reader.GetOrdinal("LaborCost")), reader.GetDecimal(reader.GetOrdinal("LaborCost")), Nothing),
                                                                                   .LaborMH = If(Not reader.IsDBNull(reader.GetOrdinal("LaborMH")), reader.GetDecimal(reader.GetOrdinal("LaborMH")), Nothing),
                                                                                   .DesignCost = If(Not reader.IsDBNull(reader.GetOrdinal("DesignCost")), reader.GetDecimal(reader.GetOrdinal("DesignCost")), Nothing),
                                                                                   .MGMTCost = If(Not reader.IsDBNull(reader.GetOrdinal("MGMTCost")), reader.GetDecimal(reader.GetOrdinal("MGMTCost")), Nothing),
                                                                                   .JobSuppliesCost = If(Not reader.IsDBNull(reader.GetOrdinal("JobSuppliesCost")), reader.GetDecimal(reader.GetOrdinal("JobSuppliesCost")), Nothing),
                                                                                   .ItemsCost = If(Not reader.IsDBNull(reader.GetOrdinal("ItemsCost")), reader.GetDecimal(reader.GetOrdinal("ItemsCost")), Nothing),
                                                                                   .DeliveryCost = If(Not reader.IsDBNull(reader.GetOrdinal("DeliveryCost")), reader.GetDecimal(reader.GetOrdinal("DeliveryCost")), Nothing),
                                                                                   .OverallCost = If(Not reader.IsDBNull(reader.GetOrdinal("OverallCost")), reader.GetDecimal(reader.GetOrdinal("OverallCost")), Nothing),
                                                                                   .OverallPrice = If(Not reader.IsDBNull(reader.GetOrdinal("OverallPrice")), reader.GetDecimal(reader.GetOrdinal("OverallPrice")), Nothing),
                                                                                   .TotalSQFT = If(Not reader.IsDBNull(reader.GetOrdinal("TotalSQFT")), reader.GetDecimal(reader.GetOrdinal("TotalSQFT")), Nothing),
                                                                                   .AvgPricePerSQFT = If(Not reader.IsDBNull(reader.GetOrdinal("AvgPricePerSQFT")), reader.GetDecimal(reader.GetOrdinal("AvgPricePerSQFT")), Nothing),
                                                                                   .CommonSQFT = If(Not reader.IsDBNull(reader.GetOrdinal("CommonSQFT")), reader.GetDecimal(reader.GetOrdinal("CommonSQFT")), Nothing)
                                                                               })
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading levels for building " & buildingID)

            Return levels
        End Function

        ''' <summary>
        ''' Saves a level (insert or update) using TableOperations.
        ''' </summary>
        ''' <param name="level">The level model to save.</param>
        ''' <param name="buildingID">The building ID this level belongs to.</param>
        ''' <param name="versionID">The version ID this level belongs to.</param>
        Public Shared Sub SaveLevel(level As LevelModel, buildingID As Integer, versionID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   level.LevelID = TableOperations.UpsertLevel(level, buildingID, versionID, conn, tran)
                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error saving level")
        End Sub

        ''' <summary>
        ''' Deletes a level and orphaned actual units (with no remaining mappings).
        ''' </summary>
        ''' <param name="levelID">The level ID to delete.</param>
        Public Sub DeleteLevel(levelID As Integer)
            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                               ' Get actual unit IDs linked to this level
                                                                               Dim actualUnitIDs As New List(Of Integer)
                                                                               Using cmd As New SqlCommand(Queries.SelectActualUnitIDsByLevelID, conn, transaction)
                                                                                   cmd.Parameters.AddWithValue("@LevelID", levelID)
                                                                                   Using reader As SqlDataReader = cmd.ExecuteReader()
                                                                                       While reader.Read()
                                                                                           actualUnitIDs.Add(reader.GetInt32(0))
                                                                                       End While
                                                                                   End Using
                                                                               End Using

                                                                               ' Delete mappings for this level
                                                                               Using cmd As New SqlCommand(Queries.DeleteActualToLevelMappingByLevelID, conn, transaction)
                                                                                   cmd.Parameters.AddWithValue("@LevelID", levelID)
                                                                                   cmd.ExecuteNonQuery()
                                                                               End Using

                                                                               ' Delete orphaned actual units (no remaining mappings)
                                                                               For Each auID In actualUnitIDs
                                                                                   Dim count As Integer
                                                                                   Using cmd As New SqlCommand(Queries.CountMappingsByActualUnitID, conn, transaction)
                                                                                       cmd.Parameters.AddWithValue("@ActualUnitID", auID)
                                                                                       count = CInt(cmd.ExecuteScalar())
                                                                                   End Using
                                                                                   If count = 0 Then
                                                                                       Using cmd As New SqlCommand(Queries.DeleteCalculatedComponentsByActualUnitID, conn, transaction)
                                                                                           cmd.Parameters.AddWithValue("@ActualUnitID", auID)
                                                                                           cmd.ExecuteNonQuery()
                                                                                       End Using
                                                                                       Using cmd As New SqlCommand(Queries.DeleteActualUnit, conn, transaction)
                                                                                           cmd.Parameters.AddWithValue("@ActualUnitID", auID)
                                                                                           cmd.ExecuteNonQuery()
                                                                                       End Using
                                                                                   End If
                                                                               Next

                                                                               ' Delete the level
                                                                               Using cmd As New SqlCommand(Queries.DeleteLevel, conn, transaction)
                                                                                   cmd.Parameters.AddWithValue("@LevelID", levelID)
                                                                                   cmd.ExecuteNonQuery()
                                                                               End Using

                                                                               transaction.Commit()
                                                                           End Sub, "Error deleting level " & levelID)
                End Using
            End Using
        End Sub

        ''' <summary>Gets level info (VersionID, ProductTypeID, CommonSQFT) for a level.</summary>
        Public Shared Function GetLevelInfo(levelID As Integer) As Tuple(Of Integer, Integer, Decimal)
            Dim params As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
            Dim versionID, productTypeID As Integer
            Dim commonSQFT As Decimal = 0D

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(
                                                                           "SELECT VersionID, ProductTypeID, ISNULL(CommonSQFT, 0) FROM Levels WHERE LevelID = @LevelID", params)
                                                                           If reader.Read() Then
                                                                               versionID = reader.GetInt32(0)
                                                                               productTypeID = reader.GetInt32(1)
                                                                               commonSQFT = reader.GetDecimal(2)
                                                                           Else
                                                                               Throw New Exception("Level not found for ID " & levelID)
                                                                           End If
                                                                       End Using
                                                                   End Sub, "Error fetching level info for " & levelID)

            Return Tuple.Create(versionID, productTypeID, commonSQFT)
        End Function

#End Region

#Region "RawUnit Operations"

        ''' <summary>
        ''' Inserts a new RawUnit using TableOperations.
        ''' </summary>
        ''' <param name="model">The RawUnit model to insert.</param>
        ''' <returns>The new RawUnitID.</returns>
        Public Function InsertRawUnit(model As RawUnitModel) As Integer
            Dim newRawUnitID As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   newRawUnitID = TableOperations.InsertRawUnit(model, model.VersionID, conn, tran)
                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error inserting RawUnit " & model.RawUnitName)
            Return newRawUnitID
        End Function

        ''' <summary>
        ''' Updates an existing RawUnit.
        ''' </summary>
        ''' <param name="model">The RawUnit model to update.</param>
        Public Sub UpdateRawUnit(model As RawUnitModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim paramsDict As New Dictionary(Of String, Object) From {
                                                                           {"@RawUnitID", model.RawUnitID},
                                                                           {"@RawUnitName", TableOperations.ToDb(model.RawUnitName)},
                                                                           {"@BF", TableOperations.ToDb(model.BF)},
                                                                           {"@LF", TableOperations.ToDb(model.LF)},
                                                                           {"@EWPLF", TableOperations.ToDb(model.EWPLF)},
                                                                           {"@SqFt", TableOperations.ToDb(model.SqFt)},
                                                                           {"@FCArea", TableOperations.ToDb(model.FCArea)},
                                                                           {"@LumberCost", TableOperations.ToDb(model.LumberCost)},
                                                                           {"@PlateCost", TableOperations.ToDb(model.PlateCost)},
                                                                           {"@ManufLaborCost", TableOperations.ToDb(model.ManufLaborCost)},
                                                                           {"@DesignLabor", TableOperations.ToDb(model.DesignLabor)},
                                                                           {"@MGMTLabor", TableOperations.ToDb(model.MGMTLabor)},
                                                                           {"@JobSuppliesCost", TableOperations.ToDb(model.JobSuppliesCost)},
                                                                           {"@ManHours", TableOperations.ToDb(model.ManHours)},
                                                                           {"@ItemCost", TableOperations.ToDb(model.ItemCost)},
                                                                           {"@OverallCost", TableOperations.ToDb(model.OverallCost)},
                                                                           {"@DeliveryCost", TableOperations.ToDb(model.DeliveryCost)},
                                                                           {"@TotalSellPrice", TableOperations.ToDb(model.TotalSellPrice)},
                                                                           {"@AvgSPFNo2", TableOperations.ToDb(model.AvgSPFNo2)},
                                                                           {"@SPFNo2BDFT", TableOperations.ToDb(model.SPFNo2BDFT)},
                                                                           {"@Avg241800", TableOperations.ToDb(model.Avg241800)},
                                                                           {"@MSR241800BDFT", TableOperations.ToDb(model.MSR241800BDFT)},
                                                                           {"@Avg242400", TableOperations.ToDb(model.Avg242400)},
                                                                           {"@MSR242400BDFT", TableOperations.ToDb(model.MSR242400BDFT)},
                                                                           {"@Avg261800", TableOperations.ToDb(model.Avg261800)},
                                                                           {"@MSR261800BDFT", TableOperations.ToDb(model.MSR261800BDFT)},
                                                                           {"@Avg262400", TableOperations.ToDb(model.Avg262400)},
                                                                           {"@MSR262400BDFT", TableOperations.ToDb(model.MSR262400BDFT)}
                                                                       }
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateRawUnit, HelperDataAccess.BuildParameters(paramsDict))
                                                                   End Sub, "Error updating RawUnit " & model.RawUnitName)
        End Sub

        ''' <summary>Gets all RawUnits for a version.</summary>
        Public Shared Function GetRawUnitsByVersionID(versionID As Integer) As List(Of RawUnitModel)
            Dim rawUnits As New List(Of RawUnitModel)
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID)}

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectRawUnitsByVersion, params)
                                                                           While reader.Read()
                                                                               rawUnits.Add(MapRawUnitFromReader(reader, versionID))
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading raw units for version " & versionID)

            Return rawUnits
        End Function

        ''' <summary>Gets a single RawUnit by ID.</summary>
        Public Shared Function GetRawUnitByID(rawUnitID As Integer) As RawUnitModel
            Dim raw As RawUnitModel = Nothing
            Dim params As SqlParameter() = {New SqlParameter("@RawUnitID", rawUnitID)}

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectRawUnitByID, params)
                                                                           If reader.Read() Then
                                                                               raw = MapRawUnitFromReader(reader, reader.GetInt32(reader.GetOrdinal("VersionID")))
                                                                               raw.RawUnitID = rawUnitID
                                                                           End If
                                                                       End Using
                                                                   End Sub, "Error loading raw unit " & rawUnitID)

            Return raw
        End Function

        ''' <summary>Maps a SqlDataReader to RawUnitModel.</summary>
        Private Shared Function MapRawUnitFromReader(reader As SqlDataReader, versionID As Integer) As RawUnitModel
            Return New RawUnitModel With {
                .RawUnitID = reader.GetInt32(reader.GetOrdinal("RawUnitID")),
                .VersionID = versionID,
                .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                .RawUnitName = reader.GetString(reader.GetOrdinal("RawUnitName")),
                .BF = If(reader.IsDBNull(reader.GetOrdinal("BF")), Nothing, reader.GetDecimal(reader.GetOrdinal("BF"))),
                .LF = If(reader.IsDBNull(reader.GetOrdinal("LF")), Nothing, reader.GetDecimal(reader.GetOrdinal("LF"))),
                .EWPLF = If(reader.IsDBNull(reader.GetOrdinal("EWPLF")), Nothing, reader.GetDecimal(reader.GetOrdinal("EWPLF"))),
                .SqFt = If(reader.IsDBNull(reader.GetOrdinal("SqFt")), Nothing, reader.GetDecimal(reader.GetOrdinal("SqFt"))),
                .FCArea = If(reader.IsDBNull(reader.GetOrdinal("FCArea")), Nothing, reader.GetDecimal(reader.GetOrdinal("FCArea"))),
                .LumberCost = If(reader.IsDBNull(reader.GetOrdinal("LumberCost")), Nothing, reader.GetDecimal(reader.GetOrdinal("LumberCost"))),
                .PlateCost = If(reader.IsDBNull(reader.GetOrdinal("PlateCost")), Nothing, reader.GetDecimal(reader.GetOrdinal("PlateCost"))),
                .ManufLaborCost = If(reader.IsDBNull(reader.GetOrdinal("ManufLaborCost")), Nothing, reader.GetDecimal(reader.GetOrdinal("ManufLaborCost"))),
                .DesignLabor = If(reader.IsDBNull(reader.GetOrdinal("DesignLabor")), Nothing, reader.GetDecimal(reader.GetOrdinal("DesignLabor"))),
                .MGMTLabor = If(reader.IsDBNull(reader.GetOrdinal("MGMTLabor")), Nothing, reader.GetDecimal(reader.GetOrdinal("MGMTLabor"))),
                .JobSuppliesCost = If(reader.IsDBNull(reader.GetOrdinal("JobSuppliesCost")), Nothing, reader.GetDecimal(reader.GetOrdinal("JobSuppliesCost"))),
                .ManHours = If(reader.IsDBNull(reader.GetOrdinal("ManHours")), Nothing, reader.GetDecimal(reader.GetOrdinal("ManHours"))),
                .ItemCost = If(reader.IsDBNull(reader.GetOrdinal("ItemCost")), Nothing, reader.GetDecimal(reader.GetOrdinal("ItemCost"))),
                .OverallCost = If(reader.IsDBNull(reader.GetOrdinal("OverallCost")), Nothing, reader.GetDecimal(reader.GetOrdinal("OverallCost"))),
                .DeliveryCost = If(reader.IsDBNull(reader.GetOrdinal("DeliveryCost")), Nothing, reader.GetDecimal(reader.GetOrdinal("DeliveryCost"))),
                .TotalSellPrice = If(reader.IsDBNull(reader.GetOrdinal("TotalSellPrice")), Nothing, reader.GetDecimal(reader.GetOrdinal("TotalSellPrice"))),
                .AvgSPFNo2 = If(reader.IsDBNull(reader.GetOrdinal("AvgSPFNo2")), Nothing, reader.GetDecimal(reader.GetOrdinal("AvgSPFNo2"))),
                .SPFNo2BDFT = If(reader.IsDBNull(reader.GetOrdinal("SPFNo2BDFT")), Nothing, reader.GetDecimal(reader.GetOrdinal("SPFNo2BDFT"))),
                .Avg241800 = If(reader.IsDBNull(reader.GetOrdinal("Avg241800")), Nothing, reader.GetDecimal(reader.GetOrdinal("Avg241800"))),
                .MSR241800BDFT = If(reader.IsDBNull(reader.GetOrdinal("MSR241800BDFT")), Nothing, reader.GetDecimal(reader.GetOrdinal("MSR241800BDFT"))),
                .Avg242400 = If(reader.IsDBNull(reader.GetOrdinal("Avg242400")), Nothing, reader.GetDecimal(reader.GetOrdinal("Avg242400"))),
                .MSR242400BDFT = If(reader.IsDBNull(reader.GetOrdinal("MSR242400BDFT")), Nothing, reader.GetDecimal(reader.GetOrdinal("MSR242400BDFT"))),
                .Avg261800 = If(reader.IsDBNull(reader.GetOrdinal("Avg261800")), Nothing, reader.GetDecimal(reader.GetOrdinal("Avg261800"))),
                .MSR261800BDFT = If(reader.IsDBNull(reader.GetOrdinal("MSR261800BDFT")), Nothing, reader.GetDecimal(reader.GetOrdinal("MSR261800BDFT"))),
                .Avg262400 = If(reader.IsDBNull(reader.GetOrdinal("Avg262400")), Nothing, reader.GetDecimal(reader.GetOrdinal("Avg262400"))),
                .MSR262400BDFT = If(reader.IsDBNull(reader.GetOrdinal("MSR262400BDFT")), Nothing, reader.GetDecimal(reader.GetOrdinal("MSR262400BDFT")))
            }
        End Function

#End Region

#Region "ActualUnit Operations"

        ''' <summary>
        ''' Saves an ActualUnit (insert or update) using TableOperations.
        ''' </summary>
        ''' <param name="unit">The ActualUnit model to save.</param>
        Public Sub SaveActualUnit(unit As ActualUnitModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   If unit.ActualUnitID > 0 Then
                                                                                       TableOperations.UpdateActualUnit(unit, conn, tran)
                                                                                   Else
                                                                                       unit.ActualUnitID = TableOperations.InsertActualUnit(unit, unit.RawUnitID, unit.VersionID, conn, tran, unit.ColorCode)
                                                                                   End If
                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error saving actual unit")
        End Sub

        ''' <summary>Gets all ActualUnits for a version.</summary>
        Public Shared Function GetActualUnitsByVersion(versionID As Integer) As List(Of ActualUnitModel)
            Dim actualUnits As New List(Of ActualUnitModel)
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID)}

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectActualUnitsByVersion, params)
                                                                           While reader.Read()
                                                                               Dim actual As New ActualUnitModel With {
                                                                                   .ActualUnitID = reader.GetInt32(reader.GetOrdinal("ActualUnitID")),
                                                                                   .UnitName = reader.GetString(reader.GetOrdinal("UnitName")),
                                                                                   .PlanSQFT = reader.GetDecimal(reader.GetOrdinal("PlanSQFT")),
                                                                                   .UnitType = reader.GetString(reader.GetOrdinal("UnitType")),
                                                                                   .OptionalAdder = reader.GetDecimal(reader.GetOrdinal("OptionalAdder")),
                                                                                   .RawUnitID = reader.GetInt32(reader.GetOrdinal("RawUnitID")),
                                                                                   .ColorCode = If(Not reader.IsDBNull(reader.GetOrdinal("ColorCode")), reader.GetString(reader.GetOrdinal("ColorCode")), String.Empty),
                                                                                   .VersionID = versionID,
                                                                                   .ReferencedRawUnitName = If(Not reader.IsDBNull(reader.GetOrdinal("RawUnitName")), reader.GetString(reader.GetOrdinal("RawUnitName")), String.Empty),
                                                                                   .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID"))
                                                                               }
                                                                               actual.CalculatedComponents = GetCalculatedComponentsByActualUnitID(actual.ActualUnitID)
                                                                               actualUnits.Add(actual)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading actual units for version " & versionID)

            Return actualUnits
        End Function

        ''' <summary>Gets a single ActualUnit by ID.</summary>
        Public Shared Function GetActualUnitByID(actualUnitID As Integer) As ActualUnitModel
            Dim unit As ActualUnitModel = Nothing
            Dim params As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectActualUnitByID, params)
                                                                           If reader.Read() Then
                                                                               unit = New ActualUnitModel With {
                                                                                   .ActualUnitID = actualUnitID,
                                                                                   .UnitName = If(Not reader.IsDBNull(reader.GetOrdinal("UnitName")), reader.GetString(reader.GetOrdinal("UnitName")), String.Empty),
                                                                                   .PlanSQFT = reader.GetDecimal(reader.GetOrdinal("PlanSQFT")),
                                                                                   .UnitType = If(Not reader.IsDBNull(reader.GetOrdinal("UnitType")), reader.GetString(reader.GetOrdinal("UnitType")), String.Empty),
                                                                                   .OptionalAdder = reader.GetDecimal(reader.GetOrdinal("OptionalAdder")),
                                                                                   .RawUnitID = reader.GetInt32(reader.GetOrdinal("RawUnitID")),
                                                                                   .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID")),
                                                                                   .ColorCode = If(Not reader.IsDBNull(reader.GetOrdinal("ColorCode")), reader.GetString(reader.GetOrdinal("ColorCode")), String.Empty),
                                                                                   .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                                                                   .ReferencedRawUnitName = If(Not reader.IsDBNull(reader.GetOrdinal("ReferencedRawUnitName")), reader.GetString(reader.GetOrdinal("ReferencedRawUnitName")), String.Empty),
                                                                                   .CalculatedComponents = GetCalculatedComponentsByActualUnitID(actualUnitID)
                                                                               }
                                                                           End If
                                                                       End Using
                                                                   End Sub, "Error loading actual unit " & actualUnitID)

            Return unit
        End Function

        ''' <summary>Deletes an ActualUnit (checks for existing mappings first).</summary>
        Public Sub DeleteActualUnit(actualUnitID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim mappingParams As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}
                                                                       Dim mappingCount As Integer = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                                                                           "SELECT COUNT(*) FROM ActualToLevelMapping WHERE ActualUnitID = @ActualUnitID", mappingParams))

                                                                       If mappingCount > 0 Then
                                                                           Throw New ApplicationException($"Cannot delete ActualUnit with {mappingCount} existing level mapping(s).")
                                                                       End If

                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteCalculatedComponentsByActualUnitID, mappingParams)
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteActualUnit, mappingParams)
                                                                   End Sub, "Error deleting actual unit " & actualUnitID)
        End Sub

#End Region

#Region "Mapping Operations"

        ''' <summary>
        ''' Saves an ActualToLevelMapping (insert or update) using TableOperations.
        ''' </summary>
        ''' <param name="mapping">The mapping model to save.</param>
        Public Sub SaveActualToLevelMapping(mapping As ActualToLevelMappingModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   If mapping.MappingID = 0 Then
                                                                                       mapping.MappingID = TableOperations.InsertActualToLevelMapping(
                                                                                           mapping.ActualUnitID, mapping.LevelID, mapping.Quantity, mapping.VersionID, conn, tran)
                                                                                   Else
                                                                                       TableOperations.UpdateActualToLevelMapping(mapping.MappingID, mapping.Quantity, conn, tran)
                                                                                   End If
                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error saving actual to level mapping")
        End Sub

        ''' <summary>Gets all mappings for a level.</summary>
        Public Shared Function GetActualToLevelMappingsByLevelID(levelID As Integer) As List(Of ActualToLevelMappingModel)
            Dim mappings As New List(Of ActualToLevelMappingModel)
            Dim params As SqlParameter() = {New SqlParameter("@LevelID", levelID)}

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectActualToLevelMappingsByLevelID, params)
                                                                           While reader.Read()
                                                                               Dim mapping As New ActualToLevelMappingModel With {
                                                                                   .MappingID = reader.GetInt32(reader.GetOrdinal("MappingID")),
                                                                                   .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID")),
                                                                                   .ActualUnitID = reader.GetInt32(reader.GetOrdinal("ActualUnitID")),
                                                                                   .LevelID = levelID,
                                                                                   .Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                                                               }
                                                                               mapping.ActualUnit = GetActualUnitByID(mapping.ActualUnitID)
                                                                               mappings.Add(mapping)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading mappings for level " & levelID)

            Return mappings
        End Function

        ''' <summary>Gets all mappings for an actual unit.</summary>
        Public Function GetActualToLevelMappingsByActualUnitID(actualUnitID As Integer) As List(Of ActualToLevelMappingModel)
            Dim mappings As New List(Of ActualToLevelMappingModel)
            Dim params As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectActualToLevelMappingsByActualUnitID, params)
                                                                           While reader.Read()
                                                                               mappings.Add(New ActualToLevelMappingModel With {
                                                                                   .MappingID = reader.GetInt32(reader.GetOrdinal("MappingID")),
                                                                                   .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID")),
                                                                                   .ActualUnitID = actualUnitID,
                                                                                   .LevelID = reader.GetInt32(reader.GetOrdinal("LevelID")),
                                                                                   .Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                                                               })
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading mappings for actual unit " & actualUnitID)

            Return mappings
        End Function

        ''' <summary>Deletes an ActualToLevelMapping.</summary>
        Public Sub DeleteActualToLevelMapping(mappingID As Integer)
            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                               SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                                   Queries.DeleteActualToLevelMappingByMappingID, {New SqlParameter("@MappingID", mappingID)}, conn, transaction)
                                                                               transaction.Commit()
                                                                           End Sub, "Error deleting mapping")
                End Using
            End Using
        End Sub

#End Region

#Region "Calculated Component Operations"

        ''' <summary>Gets all calculated components for an actual unit.</summary>
        Public Shared Function GetCalculatedComponentsByActualUnitID(actualUnitID As Integer) As List(Of CalculatedComponentModel)
            Dim components As New List(Of CalculatedComponentModel)
            Dim params As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectCalculatedComponentsByActualUnitID, params)
                                                                           While reader.Read()
                                                                               components.Add(New CalculatedComponentModel With {
                                                                                   .ComponentID = reader.GetInt32(reader.GetOrdinal("ComponentID")),
                                                                                   .ComponentType = reader.GetString(reader.GetOrdinal("ComponentType")),
                                                                                   .Value = reader.GetDecimal(reader.GetOrdinal("Value"))
                                                                               })
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading calculated components for actual unit " & actualUnitID)

            Return components
        End Function

        ''' <summary>Saves (replaces) all calculated components for an actual unit using TableOperations.</summary>
        Public Sub SaveCalculatedComponents(actualUnit As ActualUnitModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   ' Delete existing components
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                                       Queries.DeleteCalculatedComponentsByActualUnitID,
                                                                                       {New SqlParameter("@ActualUnitID", actualUnit.ActualUnitID)}, conn, tran)

                                                                                   ' Insert new components
                                                                                   For Each comp In actualUnit.CalculatedComponents
                                                                                       TableOperations.InsertCalculatedComponent(comp, actualUnit.ActualUnitID, actualUnit.VersionID, conn, tran)
                                                                                   Next

                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error saving calculated components")
        End Sub

#End Region

#Region "Settings Operations"

        ''' <summary>Gets all product settings for a version.</summary>
        Public Shared Function GetProjectProductSettings(versionID As Integer) As List(Of ProjectProductSettingsModel)
            Dim settings As New List(Of ProjectProductSettingsModel)
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID)}

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectProductSettings, params)
                                                                           While reader.Read()
                                                                               settings.Add(New ProjectProductSettingsModel With {
                                                                                   .SettingID = reader.GetInt32(reader.GetOrdinal("SettingID")),
                                                                                   .VersionID = versionID,
                                                                                   .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                                                                   .MarginPercent = If(Not reader.IsDBNull(reader.GetOrdinal("MarginPercent")), reader.GetDecimal(reader.GetOrdinal("MarginPercent")), Nothing),
                                                                                   .LumberAdder = If(Not reader.IsDBNull(reader.GetOrdinal("LumberAdder")), reader.GetDecimal(reader.GetOrdinal("LumberAdder")), Nothing)
                                                                               })
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading settings for version " & versionID)

            Return settings
        End Function

        ''' <summary>Saves a product setting (insert or update).</summary>
        Public Sub SaveProjectProductSetting(setting As ProjectProductSettingsModel, versionID As Integer)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@VersionID", versionID},
                {"@ProductTypeID", setting.ProductTypeID},
                {"@MarginPercent", TableOperations.ToDb(setting.MarginPercent)},
                {"@LumberAdder", If(setting.LumberAdder.HasValue AndAlso setting.LumberAdder.Value > 0, CObj(setting.LumberAdder.Value), DBNull.Value)}
            }

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If setting.SettingID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                                                                               Queries.InsertProjectProductSetting, HelperDataAccess.BuildParameters(paramsDict))
                                                                           If newIDObj IsNot Nothing Then setting.SettingID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@SettingID", setting.SettingID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectProductSetting, HelperDataAccess.BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving project setting")
        End Sub

        ''' <summary>Gets the margin percent for a product type in a version.</summary>
        Public Shared Function GetMarginPercent(versionID As Integer, productTypeID As Integer) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID), New SqlParameter("@ProductTypeID", productTypeID)}
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectMarginPercent, params)
                                                                   End Sub, "Error fetching margin percent for version " & versionID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        ''' <summary>Gets effective margin (from settings, raw unit, or config default).</summary>
        Public Shared Function GetEffectiveMargin(versionID As Integer, productTypeID As Integer, rawUnitID As Integer) As Decimal
            Dim margin As Decimal = GetMarginPercent(versionID, productTypeID)
            If margin <= 0D Then
                Dim rawUnit As RawUnitModel = GetRawUnitByID(rawUnitID)
                If rawUnit IsNot Nothing AndAlso rawUnit.TotalSellPrice.HasValue AndAlso rawUnit.TotalSellPrice > 0D AndAlso
                   rawUnit.OverallCost.HasValue AndAlso rawUnit.OverallCost < rawUnit.TotalSellPrice.Value Then
                    margin = (rawUnit.TotalSellPrice.Value - rawUnit.OverallCost.Value) / rawUnit.TotalSellPrice.Value
                End If
            End If
            If margin <= 0D Then
                margin = GetConfigValue("DefaultMarginPercent")
            End If
            Return margin
        End Function

#End Region

#Region "Lookup Operations"

        ''' <summary>Gets all product types for dropdowns.</summary>
        Public Function GetProductTypes() As List(Of ProductTypeModel)
            Dim types As New List(Of ProductTypeModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProductTypes)
                                                                           While reader.Read()
                                                                               types.Add(New ProductTypeModel With {
                                                                                   .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                                                                   .ProductTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("ProductTypeName")), reader.GetString(reader.GetOrdinal("ProductTypeName")), String.Empty)
                                                                               })
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading product types")
            Return types
        End Function

        ''' <summary>Gets all project types for dropdowns.</summary>
        Public Function GetProjectTypes() As List(Of ProjectTypeModel)
            Dim types As New List(Of ProjectTypeModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectTypes)
                                                                           While reader.Read()
                                                                               types.Add(New ProjectTypeModel With {
                                                                                   .ProjectTypeID = reader.GetInt32(reader.GetOrdinal("ProjectTypeID")),
                                                                                   .ProjectTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectTypeName")), reader.GetString(reader.GetOrdinal("ProjectTypeName")), String.Empty),
                                                                                   .Description = If(Not reader.IsDBNull(reader.GetOrdinal("Description")), reader.GetString(reader.GetOrdinal("Description")), String.Empty)
                                                                               })
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading project types")
            Return types
        End Function

        ''' <summary>Gets all project version statuses for dropdowns.</summary>
        Public Function GetProjectVersionStatus() As List(Of ProjectVersionStatus)
            Dim types As New List(Of ProjectVersionStatus)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectStatus)
                                                                           While reader.Read()
                                                                               types.Add(New ProjectVersionStatus With {
                                                                                   .ProjVersionStatusID = reader.GetInt32(reader.GetOrdinal("ProjVersionStatusID")),
                                                                                   .ProjVersionStatus = If(Not reader.IsDBNull(reader.GetOrdinal("ProjVersionStatus")), reader.GetString(reader.GetOrdinal("ProjVersionStatus")), String.Empty)
                                                                               })
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading project Version Status")
            Return types
        End Function

        ''' <summary>Gets a configuration value by key.</summary>
        Public Shared Function GetConfigValue(configKey As String) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@ConfigKey", configKey)}
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectConfigValue, params)
                                                                   End Sub, "Error fetching config value for " & configKey)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        ''' <summary>Gets miles to job site for a project.</summary>
        Public Shared Function GetMilesToJobSite(projectID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectMilesToJobSite, params)
                                                                   End Sub, "Error fetching miles for project " & projectID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0, CInt(valObj))
        End Function

        ''' <summary>Gets the ProjectID for a given VersionID.</summary>
        Public Shared Function GetProjectIDByVersionID(versionID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID)}
            Dim idObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                "SELECT ProjectID FROM ProjectVersions WHERE VersionID = @VersionID", params)
            Return If(idObj Is DBNull.Value OrElse idObj Is Nothing, 0, CInt(idObj))
        End Function

#End Region

#Region "Computed Data Operations"

        ''' <summary>
        ''' Computes a DataTable of unit data for a level, including all cost calculations and delivery allocation.
        ''' </summary>
        ''' <param name="levelID">The level ID to compute data for.</param>
        ''' <returns>DataTable with all computed unit data.</returns>
        Public Shared Function ComputeLevelUnitsDataTable(levelID As Integer) As DataTable
            Dim dt As New DataTable()

            ' Define columns
            dt.Columns.Add("ActualUnitID", GetType(Integer))
            dt.Columns.Add("MappingID", GetType(Integer))
            dt.Columns.Add("UnitName", GetType(String))
            dt.Columns.Add("ReferencedRawUnitName", GetType(String))
            dt.Columns.Add("ColorCode", GetType(String))
            dt.Columns.Add("ActualUnitQuantity", GetType(Integer))
            dt.Columns.Add("PlanSQFT", GetType(Decimal))
            dt.Columns.Add("LF", GetType(Decimal))
            dt.Columns.Add("BDFT", GetType(Decimal))
            dt.Columns.Add("LumberCost", GetType(Decimal))
            dt.Columns.Add("PlateCost", GetType(Decimal))
            dt.Columns.Add("ManufLaborCost", GetType(Decimal))
            dt.Columns.Add("DesignLabor", GetType(Decimal))
            dt.Columns.Add("MGMTLabor", GetType(Decimal))
            dt.Columns.Add("JobSuppliesCost", GetType(Decimal))
            dt.Columns.Add("ManHours", GetType(Decimal))
            dt.Columns.Add("ItemCost", GetType(Decimal))
            dt.Columns.Add("DeliveryCost", GetType(Decimal))
            dt.Columns.Add("OverallCost", GetType(Decimal))
            dt.Columns.Add("SellPrice", GetType(Decimal))
            dt.Columns.Add("Margin", GetType(Decimal))
            dt.Columns.Add("ProductTypeID", GetType(Integer))
            dt.Columns.Add("RawUnitID", GetType(Integer))

            Dim mappings As List(Of ActualToLevelMappingModel) = GetActualToLevelMappingsByLevelID(levelID)
            If Not mappings.Any() Then Return dt

            Dim levelInfo As Tuple(Of Integer, Integer, Decimal) = GetLevelInfo(levelID)
            Dim versionID As Integer = levelInfo.Item1
            Dim productTypeID As Integer = levelInfo.Item2
            Dim projectID As Integer = GetProjectIDByVersionID(versionID)
            Dim miles As Integer = GetMilesToJobSite(projectID)
            Dim mileageRate As Decimal = GetConfigValue("MileageRate")
            Dim totalBDFT As Decimal = 0D
            Dim unitBDFTs As New List(Of Decimal)()
            Dim tempRows As New List(Of DataRow)()

            ' First pass: calculate costs
            For Each mapping In mappings
                Dim actual = mapping.ActualUnit
                Dim raw = GetRawUnitByID(actual.RawUnitID)

                If raw Is Nothing Then Continue For

                Dim isWallUnit As Boolean = (raw.ProductTypeID = 3)
                Dim divisor As Decimal = If(isWallUnit, 1, raw.SqFt.GetValueOrDefault())
                If divisor <= 0D Then Continue For

                Dim planSqft As Decimal = actual.PlanSQFT
                Dim opt As Decimal = actual.OptionalAdder
                Dim qty As Integer = mapping.Quantity
                Dim extSqft As Decimal = planSqft * qty * opt

                Dim lfPer As Decimal = raw.LF.GetValueOrDefault() / divisor
                Dim bdftPer As Decimal = raw.BF.GetValueOrDefault() / divisor
                Dim history = LumberDataAccess.GetLatestLumberHistory(actual.RawUnitID, versionID)
                Dim lumberPer As Decimal = history.LumberCost / divisor

                Dim lumberCost As Decimal = lumberPer * extSqft
                Dim lumberadder As Decimal = LumberDataAccess.GetLumberAdder(versionID, productTypeID)
                If lumberadder > 0D Then lumberCost += (bdftPer * extSqft / 1000D) * lumberadder

                Dim row As DataRow = dt.NewRow()
                row("ActualUnitID") = actual.ActualUnitID
                row("MappingID") = mapping.MappingID
                row("UnitName") = actual.UnitName
                row("ReferencedRawUnitName") = raw.RawUnitName
                row("ColorCode") = If(String.IsNullOrWhiteSpace(actual.ColorCode), "N/A", actual.ColorCode)
                row("ActualUnitQuantity") = qty
                row("PlanSQFT") = planSqft
                row("LF") = lfPer * extSqft
                row("BDFT") = bdftPer * extSqft
                row("LumberCost") = lumberCost
                row("PlateCost") = (raw.PlateCost.GetValueOrDefault() / divisor) * extSqft
                row("ManufLaborCost") = (raw.ManufLaborCost.GetValueOrDefault() / divisor) * extSqft
                row("DesignLabor") = (raw.DesignLabor.GetValueOrDefault() / divisor) * extSqft
                row("MGMTLabor") = (raw.MGMTLabor.GetValueOrDefault() / divisor) * extSqft
                row("JobSuppliesCost") = (raw.JobSuppliesCost.GetValueOrDefault() / divisor) * extSqft
                row("ManHours") = (raw.ManHours.GetValueOrDefault() / divisor) * extSqft
                row("ItemCost") = (raw.ItemCost.GetValueOrDefault() / divisor) * extSqft

                Dim overallCost As Decimal = row.Field(Of Decimal)("LumberCost") + row.Field(Of Decimal)("PlateCost") +
                    row.Field(Of Decimal)("ManufLaborCost") + row.Field(Of Decimal)("DesignLabor") +
                    row.Field(Of Decimal)("MGMTLabor") + row.Field(Of Decimal)("JobSuppliesCost") +
                    row.Field(Of Decimal)("ItemCost")
                row("OverallCost") = overallCost

                Dim margin As Decimal = GetEffectiveMargin(versionID, productTypeID, raw.RawUnitID)
                Dim sellPrice As Decimal = If(margin >= 1D, overallCost, overallCost / (1D - margin))
                row("SellPrice") = sellPrice
                row("Margin") = sellPrice - overallCost
                row("ProductTypeID") = actual.ProductTypeID
                row("RawUnitID") = raw.RawUnitID

                unitBDFTs.Add(row.Field(Of Decimal)("BDFT"))
                totalBDFT += row.Field(Of Decimal)("BDFT")
                tempRows.Add(row)
            Next

            ' Second pass: apply delivery cost allocation
            Dim deliveryTotal As Decimal = 0D
            If totalBDFT > 0D Then
                Dim numLoads As Decimal = Math.Ceiling(totalBDFT / 10000D)
                deliveryTotal = numLoads * mileageRate * miles
                If deliveryTotal < 150D Then deliveryTotal = 150D
            End If

            If totalBDFT > 0D Then
                For i As Integer = 0 To tempRows.Count - 1
                    Dim row = tempRows(i)
                    Dim unitBDFT = unitBDFTs(i)
                    row("DeliveryCost") = (unitBDFT / totalBDFT) * deliveryTotal

                    Dim overallCost = row.Field(Of Decimal)("OverallCost")
                    Dim margin = GetEffectiveMargin(versionID, CInt(row.Field(Of Integer?)("ProductTypeID")), row.Field(Of Integer)("RawUnitID"))
                    Dim sellPrice = If(margin >= 1D,
                        overallCost + row.Field(Of Decimal)("DeliveryCost"),
                        overallCost / (1D - margin) + row.Field(Of Decimal)("DeliveryCost"))
                    row("SellPrice") = sellPrice
                    row("Margin") = sellPrice - overallCost - row.Field(Of Decimal)("DeliveryCost")
                Next
            End If

            For Each row In tempRows
                dt.Rows.Add(row)
            Next

            Return dt
        End Function

#End Region

    End Class
End Namespace