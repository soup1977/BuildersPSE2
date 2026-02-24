Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.Utilities

Namespace DataAccess
    ''' <summary>
    ''' Data access layer for Project Version operations including create, update, and duplication.
    ''' Uses TableOperations.ToDb() for consistent null handling.
    ''' </summary>
    Public Class ProjVersionDataAccess

#Region "Create Operations"

        ''' <summary>
        ''' Creates a new project version.
        ''' </summary>
        Public Shared Function CreateProjectVersion(projectID As Integer, versionName As String, description As String,
                                                    customerID As Integer?, salesID As Integer?,
                                                    Optional futuresAdderAmt As Decimal? = Nothing,
                                                    Optional futuresAdderProjTotal As Decimal? = Nothing) As Integer
            ' Validate CustomerID for CustomerType=1
            If customerID.HasValue AndAlso Not HelperDataAccess.ValidateCustomerType(customerID, 1) Then
                Throw New ArgumentException("CustomerID must reference a customer with CustomerType=1 (Customer).")
            End If

            Dim newVersionID As Integer = 0

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   Dim model As New ProjectVersionModel With {
                                                                                       .ProjectID = projectID,
                                                                                       .VersionName = versionName,
                                                                                       .VersionDate = Now,
                                                                                       .Description = description,
                                                                                       .CustomerID = customerID,
                                                                                       .SalesID = salesID,
                                                                                       .ProjVersionStatusID = 1,
                                                                                       .FuturesAdderAmt = futuresAdderAmt,
                                                                                       .FuturesAdderProjTotal = futuresAdderProjTotal
                                                                                   }
                                                                                   newVersionID = TableOperations.InsertProjectVersion(model, conn, tran)
                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, $"Error creating project version for project {projectID}")

            Return newVersionID
        End Function

#End Region

#Region "Read Operations"

        ''' <summary>
        ''' Gets all versions for a specific project.
        ''' </summary>
        Public Shared Function GetProjectVersions(projectID As Integer) As List(Of ProjectVersionModel)
            Dim versions As New List(Of ProjectVersionModel)

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(
                                                                           Queries.SelectProjectVersions, {New SqlParameter("@ProjectID", projectID)})
                                                                           While reader.Read()
                                                                               versions.Add(MapVersionFromReader(reader))
                                                                           End While
                                                                       End Using
                                                                   End Sub, $"Error loading versions for project {projectID}")

            Return versions
        End Function

        ''' <summary>
        ''' Gets all project versions across all projects, grouped by project.
        ''' </summary>
        Public Shared Function GetAllProjectVersions() As List(Of List(Of ProjectVersionModel))
            Dim projectVersions As New List(Of List(Of ProjectVersionModel))

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectAllProjectVersions)
                                                                           Dim currentProjectID As Integer = -1
                                                                           Dim currentVersions As List(Of ProjectVersionModel) = Nothing

                                                                           While reader.Read()
                                                                               Dim projectID As Integer = reader.GetInt32(reader.GetOrdinal("ProjectID"))

                                                                               If projectID <> currentProjectID Then
                                                                                   currentVersions = New List(Of ProjectVersionModel)
                                                                                   projectVersions.Add(currentVersions)
                                                                                   currentProjectID = projectID
                                                                               End If

                                                                               currentVersions.Add(MapVersionFromReader(reader))
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading all project versions")

            Return projectVersions
        End Function

        ''' <summary>Maps a SqlDataReader to ProjectVersionModel.</summary>
        Private Shared Function MapVersionFromReader(reader As SqlDataReader) As ProjectVersionModel
            Return New ProjectVersionModel With {
                .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID")),
                .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                .VersionName = reader.GetString(reader.GetOrdinal("VersionName")),
                .VersionDate = If(Not reader.IsDBNull(reader.GetOrdinal("VersionDate")),
                    reader.GetDateTime(reader.GetOrdinal("VersionDate")), Nothing),
                .Description = If(Not reader.IsDBNull(reader.GetOrdinal("Description")),
                    reader.GetString(reader.GetOrdinal("Description")), String.Empty),
                .LastModifiedDate = If(Not reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")),
                    reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")), Nothing),
                .CustomerID = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerID")),
                    reader.GetInt32(reader.GetOrdinal("CustomerID")), Nothing),
                .SalesID = If(Not reader.IsDBNull(reader.GetOrdinal("SalesID")),
                    reader.GetInt32(reader.GetOrdinal("SalesID")), Nothing),
                .CustomerName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerName")),
                    reader.GetString(reader.GetOrdinal("CustomerName")), String.Empty),
                .SalesName = If(Not reader.IsDBNull(reader.GetOrdinal("SalesName")),
                    reader.GetString(reader.GetOrdinal("SalesName")), String.Empty),
                .MondayID = If(Not reader.IsDBNull(reader.GetOrdinal("MondayID")),
                    reader.GetString(reader.GetOrdinal("MondayID")), String.Empty),
                .ProjVersionStatusID = If(Not reader.IsDBNull(reader.GetOrdinal("ProjVersionStatusID")),
                    reader.GetInt32(reader.GetOrdinal("ProjVersionStatusID")), Nothing),
                .FuturesAdderAmt = If(Not reader.IsDBNull(reader.GetOrdinal("FuturesAdderAmt")),
                    reader.GetDecimal(reader.GetOrdinal("FuturesAdderAmt")), Nothing),
                .FuturesAdderProjTotal = If(Not reader.IsDBNull(reader.GetOrdinal("FuturesAdderProjTotal")),
                    reader.GetDecimal(reader.GetOrdinal("FuturesAdderProjTotal")), Nothing),
                .IsLocked = If(Not reader.IsDBNull(reader.GetOrdinal("IsLocked")),
                    reader.GetBoolean(reader.GetOrdinal("IsLocked")), False),
                .LockedDate = If(Not reader.IsDBNull(reader.GetOrdinal("LockedDate")),
                    reader.GetDateTime(reader.GetOrdinal("LockedDate")), Nothing),
                .LockedBy = If(Not reader.IsDBNull(reader.GetOrdinal("LockedBy")),
                    reader.GetString(reader.GetOrdinal("LockedBy")), String.Empty)
            }
        End Function

#End Region

#Region "Update Operations"

        ''' <summary>
        ''' Updates an existing project version.
        ''' </summary>
        Public Shared Sub UpdateProjectVersion(versionID As Integer, versionName As String, mondayID As String,
                                               projStatusID As Integer?, customerID As Integer?, salesID As Integer?,
                                               Optional futuresAdderAmt As Decimal? = Nothing,
                                               Optional futuresAdderProjTotal As Decimal? = Nothing)
            ' Validate CustomerID for CustomerType=1
            If customerID.HasValue AndAlso Not HelperDataAccess.ValidateCustomerType(customerID, 1) Then
                Throw New ArgumentException("CustomerID must reference a customer with CustomerType=1 (Customer).")
            End If

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   Dim model As New ProjectVersionModel With {
                                                                                       .VersionID = versionID,
                                                                                       .VersionName = versionName,
                                                                                       .MondayID = mondayID,
                                                                                       .ProjVersionStatusID = projStatusID,
                                                                                       .CustomerID = customerID,
                                                                                       .SalesID = salesID,
                                                                                       .FuturesAdderAmt = futuresAdderAmt,
                                                                                       .FuturesAdderProjTotal = futuresAdderProjTotal
                                                                                   }
                                                                                   TableOperations.UpdateProjectVersion(model, conn, tran)
                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, $"Error updating project version {versionID}")
        End Sub

#End Region

#Region "Duplication Operations"

        ''' <summary>
        ''' Duplicates an entire project version including all related data:
        ''' Buildings, Levels, RawUnits, ActualUnits, CalculatedComponents, Mappings, Settings, and Lumber History.
        ''' </summary>
        Public Shared Sub DuplicateProjectVersion(originalVersionID As Integer, newVersionName As String,
                                                  description As String, projectID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       ' Pre-fetch all data before transaction
                                                                       Dim sourceData As VersionDuplicationData = LoadSourceVersionData(originalVersionID)

                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   ' Create new version
                                                                                   Dim newVersionID As Integer = CreateNewVersion(projectID, newVersionName, description, conn, tran)

                                                                                   ' Duplicate in dependency order
                                                                                   DuplicateProductSettings(originalVersionID, newVersionID, conn, tran)
                                                                                   Dim rawIdMap As Dictionary(Of Integer, Integer) = DuplicateRawUnits(sourceData.UniqueRawUnits, newVersionID, conn, tran)
                                                                                   DuplicateLumberHistory(sourceData.LumberHistories, rawIdMap, newVersionID, conn, tran)
                                                                                   Dim actualIdMap As Dictionary(Of Integer, Integer) = DuplicateActualUnits(sourceData.UniqueActualUnits, rawIdMap, newVersionID, conn, tran)
                                                                                   DuplicateCalculatedComponents(sourceData.UniqueActualUnits, actualIdMap, newVersionID, conn, tran)
                                                                                   Dim buildingIdMap As Dictionary(Of Integer, Integer) = DuplicateBuildings(sourceData.Buildings, newVersionID, conn, tran)
                                                                                   Dim levelIdMap As Dictionary(Of Integer, Integer) = DuplicateLevels(sourceData.Buildings, buildingIdMap, newVersionID, conn, tran)
                                                                                   DuplicateMappings(sourceData.Buildings, actualIdMap, levelIdMap, newVersionID, conn, tran)

                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, $"Error duplicating project version {originalVersionID}")
        End Sub

        ''' <summary>Container for pre-loaded version data.</summary>
        Private Class VersionDuplicationData
            Public Property Buildings As List(Of BuildingModel)
            Public Property UniqueRawUnits As List(Of RawUnitModel)
            Public Property UniqueActualUnits As List(Of ActualUnitModel)
            Public Property LumberHistories As Dictionary(Of Integer, List(Of RawUnitLumberHistoryModel))
        End Class

        ''' <summary>Pre-loads all data needed for version duplication.</summary>
        Private Shared Function LoadSourceVersionData(versionID As Integer) As VersionDuplicationData
            Dim data As New VersionDuplicationData With {
                .Buildings = ProjectDataAccess.GetBuildingsByVersionID(versionID),
                .UniqueRawUnits = New List(Of RawUnitModel),
                .UniqueActualUnits = New List(Of ActualUnitModel),
                .LumberHistories = New Dictionary(Of Integer, List(Of RawUnitLumberHistoryModel))
            }

            ' Load mappings for each level
            For Each bldg In data.Buildings
                For Each level In bldg.Levels
                    level.ActualUnitMappings = ProjectDataAccess.GetActualToLevelMappingsByLevelID(level.LevelID)
                Next
            Next

            ' Collect unique RawUnits and ActualUnits
            Dim rawUnitSet As New HashSet(Of Integer)
            Dim actualUnitSet As New HashSet(Of Integer)

            For Each bldg In data.Buildings
                For Each level In bldg.Levels
                    For Each mapping In level.ActualUnitMappings
                        ' RawUnits
                        If Not rawUnitSet.Contains(mapping.ActualUnit.RawUnitID) Then
                            rawUnitSet.Add(mapping.ActualUnit.RawUnitID)
                            Dim rawUnit = ProjectDataAccess.GetRawUnitByID(mapping.ActualUnit.RawUnitID)
                            If rawUnit IsNot Nothing Then
                                If String.IsNullOrEmpty(rawUnit.RawUnitName) Then
                                    Throw New ArgumentException($"RawUnitID {rawUnit.RawUnitID} has null or empty RawUnitName.")
                                End If
                                data.UniqueRawUnits.Add(rawUnit)
                            End If
                        End If

                        ' ActualUnits
                        If Not actualUnitSet.Contains(mapping.ActualUnit.ActualUnitID) Then
                            actualUnitSet.Add(mapping.ActualUnit.ActualUnitID)
                            data.UniqueActualUnits.Add(mapping.ActualUnit)
                        End If
                    Next
                Next
            Next

            ' Load lumber history
            data.LumberHistories = LoadLumberHistories(versionID)

            Return data
        End Function

        ''' <summary>Loads all lumber history records for a version.</summary>
        Private Shared Function LoadLumberHistories(versionID As Integer) As Dictionary(Of Integer, List(Of RawUnitLumberHistoryModel))
            Dim histories As New Dictionary(Of Integer, List(Of RawUnitLumberHistoryModel))

            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()

                ' Force ARITHABORT ON for consistent query plan
                Using setCmd As New SqlCommand("SET ARITHABORT ON", conn)
                    setCmd.ExecuteNonQuery()
                End Using

                Using cmd As New SqlCommand(Queries.SelectLumberHistoryByVersion, conn)
                    cmd.CommandTimeout = 120
                    cmd.Parameters.AddWithValue("@VersionID", versionID)

                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            Dim rawID As Integer = reader.GetInt32(reader.GetOrdinal("RawUnitID"))

                            If Not histories.ContainsKey(rawID) Then
                                histories(rawID) = New List(Of RawUnitLumberHistoryModel)
                            End If

                            histories(rawID).Add(New RawUnitLumberHistoryModel With {
                                .RawUnitID = rawID,
                                .CostEffectiveDateID = If(reader.IsDBNull(reader.GetOrdinal("CostEffectiveDateID")),
                                    Nothing, CType(reader.GetInt32(reader.GetOrdinal("CostEffectiveDateID")), Integer?)),
                                .LumberCost = reader.GetDecimal(reader.GetOrdinal("LumberCost")),
                                .AvgSPFNo2 = If(reader.IsDBNull(reader.GetOrdinal("AvgSPFNo2")),
                                    Nothing, CType(reader.GetDecimal(reader.GetOrdinal("AvgSPFNo2")), Decimal?)),
                                .Avg241800 = If(reader.IsDBNull(reader.GetOrdinal("Avg241800")),
                                    Nothing, CType(reader.GetDecimal(reader.GetOrdinal("Avg241800")), Decimal?)),
                                .Avg242400 = If(reader.IsDBNull(reader.GetOrdinal("Avg242400")),
                                    Nothing, CType(reader.GetDecimal(reader.GetOrdinal("Avg242400")), Decimal?)),
                                .Avg261800 = If(reader.IsDBNull(reader.GetOrdinal("Avg261800")),
                                    Nothing, CType(reader.GetDecimal(reader.GetOrdinal("Avg261800")), Decimal?)),
                                .Avg262400 = If(reader.IsDBNull(reader.GetOrdinal("Avg262400")),
                                    Nothing, CType(reader.GetDecimal(reader.GetOrdinal("Avg262400")), Decimal?))
                            })
                        End While
                    End Using
                End Using
            End Using

            Return histories
        End Function

        ''' <summary>Creates the new version record using TableOperations.</summary>
        Private Shared Function CreateNewVersion(projectID As Integer, versionName As String, description As String,
                                                 conn As SqlConnection, tran As SqlTransaction) As Integer
            Dim model As New ProjectVersionModel With {
                .ProjectID = projectID,
                .VersionName = versionName,
                .VersionDate = Date.Now,
                .Description = description,
                .ProjVersionStatusID = 1
            }
            Return TableOperations.InsertProjectVersion(model, conn, tran)
        End Function

        ''' <summary>Duplicates product settings.</summary>
        Private Shared Sub DuplicateProductSettings(originalVersionID As Integer, newVersionID As Integer,
                                                    conn As SqlConnection, tran As SqlTransaction)
            Dim params As New Dictionary(Of String, Object) From {
                {"@OriginalVersionID", originalVersionID},
                {"@NewVersionID", newVersionID}
            }
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                Queries.DuplicateProjectProductSettings, HelperDataAccess.BuildParameters(params), conn, tran)
        End Sub

        ''' <summary>Duplicates raw units using TableOperations and returns old-to-new ID mapping.</summary>
        Private Shared Function DuplicateRawUnits(rawUnits As List(Of RawUnitModel), newVersionID As Integer,
                                                  conn As SqlConnection, tran As SqlTransaction) As Dictionary(Of Integer, Integer)
            Dim idMap As New Dictionary(Of Integer, Integer)

            For Each raw In rawUnits
                Dim newID As Integer = TableOperations.InsertRawUnit(raw, newVersionID, conn, tran)
                idMap.Add(raw.RawUnitID, newID)
            Next

            Return idMap
        End Function


        ''' <summary>Duplicates lumber history records using TableOperations.</summary>
        Private Shared Sub DuplicateLumberHistory(histories As Dictionary(Of Integer, List(Of RawUnitLumberHistoryModel)),
                                                  rawIdMap As Dictionary(Of Integer, Integer), newVersionID As Integer,
                                                  conn As SqlConnection, tran As SqlTransaction)
            For Each kvp In histories
                If Not rawIdMap.ContainsKey(kvp.Key) Then Continue For

                Dim newRawID As Integer = rawIdMap(kvp.Key)
                Dim lastHistoryID As Integer = 0

                For Each hist In kvp.Value
                    lastHistoryID = TableOperations.InsertLumberHistoryFromModel(hist, newRawID, newVersionID, conn, tran)
                Next

                ' Activate the most recent history record
                If lastHistoryID > 0 Then
                    TableOperations.SetLumberHistoryActive(newRawID, newVersionID, lastHistoryID, conn, tran)
                End If
            Next
        End Sub

        ''' <summary>Duplicates actual units using TableOperations and returns old-to-new ID mapping.</summary>
        Private Shared Function DuplicateActualUnits(actualUnits As List(Of ActualUnitModel),
                                                     rawIdMap As Dictionary(Of Integer, Integer), newVersionID As Integer,
                                                     conn As SqlConnection, tran As SqlTransaction) As Dictionary(Of Integer, Integer)
            Dim idMap As New Dictionary(Of Integer, Integer)

            For Each actual In actualUnits
                Dim newRawID As Integer = rawIdMap(actual.RawUnitID)
                Dim newID As Integer = TableOperations.InsertActualUnit(actual, newRawID, newVersionID, conn, tran, actual.ColorCode?.Trim())
                idMap.Add(actual.ActualUnitID, newID)
            Next

            Return idMap
        End Function

        ''' <summary>Duplicates calculated components using TableOperations.</summary>
        Private Shared Sub DuplicateCalculatedComponents(actualUnits As List(Of ActualUnitModel),
                                                         actualIdMap As Dictionary(Of Integer, Integer), newVersionID As Integer,
                                                         conn As SqlConnection, tran As SqlTransaction)
            For Each actual In actualUnits
                Dim newActualID As Integer = actualIdMap(actual.ActualUnitID)
                For Each comp In actual.CalculatedComponents
                    TableOperations.InsertCalculatedComponent(comp, newActualID, newVersionID, conn, tran)
                Next
            Next
        End Sub

        ''' <summary>Duplicates buildings using TableOperations and returns old-to-new ID mapping.</summary>
        Private Shared Function DuplicateBuildings(buildings As List(Of BuildingModel), newVersionID As Integer,
                                                   conn As SqlConnection, tran As SqlTransaction) As Dictionary(Of Integer, Integer)
            Dim idMap As New Dictionary(Of Integer, Integer)

            For Each bldg In buildings
                Dim newID As Integer = TableOperations.InsertBuilding(bldg, newVersionID, conn, tran)
                idMap.Add(bldg.BuildingID, newID)
            Next

            Return idMap
        End Function

        ''' <summary>Duplicates levels using TableOperations and returns old-to-new ID mapping.</summary>
        Private Shared Function DuplicateLevels(buildings As List(Of BuildingModel),
                                                buildingIdMap As Dictionary(Of Integer, Integer), newVersionID As Integer,
                                                conn As SqlConnection, tran As SqlTransaction) As Dictionary(Of Integer, Integer)
            Dim idMap As New Dictionary(Of Integer, Integer)

            For Each bldg In buildings
                Dim newBldgID As Integer = buildingIdMap(bldg.BuildingID)
                For Each level In bldg.Levels
                    Dim newID As Integer = TableOperations.InsertLevel(level, newBldgID, newVersionID, conn, tran)
                    idMap.Add(level.LevelID, newID)
                Next
            Next

            Return idMap
        End Function

        ''' <summary>Duplicates actual-to-level mappings using TableOperations.</summary>
        Private Shared Sub DuplicateMappings(buildings As List(Of BuildingModel),
                                             actualIdMap As Dictionary(Of Integer, Integer),
                                             levelIdMap As Dictionary(Of Integer, Integer), newVersionID As Integer,
                                             conn As SqlConnection, tran As SqlTransaction)
            For Each bldg In buildings
                For Each level In bldg.Levels
                    Dim newLevelID As Integer = levelIdMap(level.LevelID)
                    For Each mapping In level.ActualUnitMappings
                        Dim newActualID As Integer = actualIdMap(mapping.ActualUnit.ActualUnitID)
                        TableOperations.InsertActualToLevelMapping(newActualID, newLevelID, mapping.Quantity, newVersionID, conn, tran)
                    Next
                Next
            Next
        End Sub

#End Region

#Region "Version Locking Operations"

        ''' <summary>
        ''' Checks if a project version is locked.
        ''' </summary>
        ''' <param name="versionID">The version ID to check.</param>
        ''' <returns>True if locked, False otherwise.</returns>
        Public Shared Function IsVersionLocked(versionID As Integer) As Boolean
            Dim isLocked As Boolean = False

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(
                                                                           Queries.SelectVersionIsLocked, {New SqlParameter("@VersionID", versionID)})
                                                                           If reader.Read() Then
                                                                               isLocked = reader.GetBoolean(0)
                                                                           End If
                                                                       End Using
                                                                   End Sub, $"Error checking lock status for version {versionID}")

            Return isLocked
        End Function

        ''' <summary>
        ''' Locks a project version and sets status to Submitted.
        ''' </summary>
        ''' <param name="versionID">The version ID to lock.</param>
        ''' <param name="submittedStatusID">The status ID for Submitted (typically 2).</param>
        ''' <param name="lockedBy">Username of person locking the version.</param>
        Public Shared Sub LockVersion(versionID As Integer, submittedStatusID As Integer, lockedBy As String)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   Dim params As New Dictionary(Of String, Object) From {
                                                                                       {"@VersionID", versionID},
                                                                                       {"@StatusID", submittedStatusID},
                                                                                       {"@LockedBy", lockedBy}
                                                                                   }

                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                                       Queries.LockProjectVersion,
                                                                                       HelperDataAccess.BuildParameters(params),
                                                                                       conn, tran)

                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, $"Error locking version {versionID}")
        End Sub

        ''' <summary>
        ''' Unlocks a project version (admin override only).
        ''' </summary>
        ''' <param name="versionID">The version ID to unlock.</param>
        Public Shared Sub UnlockVersion(versionID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   Dim params As New Dictionary(Of String, Object) From {
                                                                                       {"@VersionID", versionID}
                                                                                   }

                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                                       Queries.UnlockProjectVersion,
                                                                                       HelperDataAccess.BuildParameters(params),
                                                                                       conn, tran)

                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, $"Error unlocking version {versionID}")
        End Sub

        ''' <summary>
        ''' Unlocks a project version (admin override only) with audit logging.
        ''' </summary>
        ''' <param name="versionID">The version ID to unlock.</param>
        ''' <param name="unlockedBy">Admin username performing the override.</param>
        ''' <param name="reason">Reason for unlocking (required for audit).</param>
        ''' <param name="originalLockedDate">Original lock date (for audit trail).</param>
        ''' <param name="originalLockedBy">Original person who locked it (for audit trail).</param>
        Public Shared Sub UnlockVersionWithAudit(versionID As Integer, unlockedBy As String, reason As String,
                                                 originalLockedDate As DateTime?, originalLockedBy As String)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   ' 1. Unlock the version
                                                                                   Dim unlockParams As New Dictionary(Of String, Object) From {
                                                                                       {"@VersionID", versionID}
                                                                                   }
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                                       Queries.UnlockProjectVersion,
                                                                                       HelperDataAccess.BuildParameters(unlockParams),
                                                                                       conn, tran)

                                                                                   ' 2. Log the admin override
                                                                                   Dim auditParams As New Dictionary(Of String, Object) From {
                                                                                        {"@VersionID", versionID},
                                                                                        {"@UnlockedBy", unlockedBy},
                                                                                        {"@Reason", reason},
                                                                                        {"@OriginalLockedDate", If(originalLockedDate.HasValue, CType(originalLockedDate.Value, Object), DBNull.Value)},
                                                                                        {"@OriginalLockedBy", If(String.IsNullOrEmpty(originalLockedBy), DBNull.Value, CType(originalLockedBy, Object))}
                                                                                    }
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                                       Queries.LogVersionUnlock,
                                                                                       HelperDataAccess.BuildParameters(auditParams),
                                                                                       conn, tran)

                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, $"Error unlocking version {versionID} with audit")
        End Sub

#End Region

    End Class
End Namespace