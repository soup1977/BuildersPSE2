
Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.Utilities

Namespace DataAccess


    Public Class ProjVersionDataAccess
        Public Shared Function CreateProjectVersion(projectID As Integer, versionName As String, description As String, customerID As Integer?, salesID As Integer?) As Integer
            ' Validate CustomerID for CustomerType=1
            If customerID.HasValue AndAlso Not HelperDataAccess.ValidateCustomerType(customerID, 1) Then
                Throw New ArgumentException("CustomerID must reference a customer with CustomerType=1 (Customer).")
            End If

            Dim newVersionID As Integer

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim params As New Dictionary(Of String, Object) From {
                                                                   {"@ProjectID", projectID},
                                                                   {"@VersionName", versionName},
                                                                   {"@VersionDate", Now},
                                                                   {"@LastModifiedDate", Now},
                                                                   {"@Description", If(String.IsNullOrEmpty(description), DBNull.Value, CType(description, Object))},
                                                                   {"@CustomerID", If(customerID.HasValue, CType(customerID.Value, Object), DBNull.Value)},
                                                                   {"@SalesID", If(salesID.HasValue, CType(salesID.Value, Object), DBNull.Value)},
                                                                   {"@MondayID", Nothing}
                                                               }
                                                                       Dim newVersionIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectVersion, HelperDataAccess.BuildParameters(params))
                                                                       newVersionID = CInt(newVersionIDObj)
                                                                   End Sub, "Error creating project version for " & projectID)
            Return newVersionID
        End Function

        Public Shared Sub DuplicateProjectVersion(originalVersionID As Integer, newVersionName As String, description As String, projectID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       ' Pre-fetch all data from the original version before starting the transaction
                                                                       Dim origBuildings As List(Of BuildingModel) = ProjectDataAccess.GetBuildingsByVersionID(originalVersionID)
                                                                       For Each bldg In origBuildings
                                                                           For Each level In bldg.Levels
                                                                               level.ActualUnitMappings = ProjectDataAccess.GetActualToLevelMappingsByLevelID(level.LevelID)
                                                                           Next
                                                                       Next

                                                                       ' Collect unique RawUnits
                                                                       Dim rawUnitSet As New HashSet(Of Integer)
                                                                       Dim uniqueRawUnits As New List(Of RawUnitModel)
                                                                       For Each bldg In origBuildings
                                                                           For Each level In bldg.Levels
                                                                               For Each mapping In level.ActualUnitMappings
                                                                                   Dim rawID As Integer = mapping.ActualUnit.RawUnitID
                                                                                   If Not rawUnitSet.Contains(rawID) Then
                                                                                       rawUnitSet.Add(rawID)
                                                                                       Dim rawUnit As RawUnitModel = ProjectDataAccess.GetRawUnitByID(rawID)
                                                                                       If rawUnit IsNot Nothing Then
                                                                                           uniqueRawUnits.Add(rawUnit)
                                                                                       Else
                                                                                           Debug.WriteLine($"Warning: RawUnitID {rawID} not found.")
                                                                                       End If
                                                                                   End If
                                                                               Next
                                                                           Next
                                                                       Next

                                                                       ' Validate RawUnitName for all RawUnits
                                                                       For Each rawUnit In uniqueRawUnits
                                                                           If String.IsNullOrEmpty(rawUnit.RawUnitName) Then
                                                                               Throw New ArgumentException($"RawUnitID {rawUnit.RawUnitID} has a null or empty RawUnitName.")
                                                                           End If
                                                                       Next

                                                                       ' Collect unique ActualUnits
                                                                       Dim actualUnitSet As New HashSet(Of Integer)
                                                                       Dim uniqueActualUnits As New List(Of ActualUnitModel)
                                                                       For Each bldg In origBuildings
                                                                           For Each level In bldg.Levels
                                                                               For Each mapping In level.ActualUnitMappings
                                                                                   Dim actualID As Integer = mapping.ActualUnit.ActualUnitID
                                                                                   If Not actualUnitSet.Contains(actualID) Then
                                                                                       actualUnitSet.Add(actualID)
                                                                                       uniqueActualUnits.Add(mapping.ActualUnit)
                                                                                   End If
                                                                               Next
                                                                           Next
                                                                       Next

                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using transaction As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   ' Create new version
                                                                                   Dim params As New Dictionary(Of String, Object) From {
                                                                                        {"@ProjectID", projectID},
                                                                                        {"@VersionName", newVersionName},
                                                                                        {"@VersionDate", Date.Now},
                                                                                        {"@Description", If(String.IsNullOrEmpty(description), DBNull.Value, CType(description, Object))},
                                                                                        {"@CustomerID", DBNull.Value},
                                                                                        {"@SalesID", DBNull.Value},
                                                                                        {"MondayID", Nothing}
                                                                                    }
                                                                                   Dim newVersionID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertProjectVersion, HelperDataAccess.BuildParameters(params), conn, transaction)

                                                                                   ' Duplicate ProjectProductSettings
                                                                                   params = New Dictionary(Of String, Object) From {
                                                                                        {"@OriginalVersionID", originalVersionID},
                                                                                        {"@NewVersionID", newVersionID}
                                                                                    }
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DuplicateProjectProductSettings, HelperDataAccess.BuildParameters(params), conn, transaction)

                                                                                   ' Duplicate RawUnits and create map
                                                                                   Dim rawIdMap As New Dictionary(Of Integer, Integer)
                                                                                   For Each raw In uniqueRawUnits
                                                                                       params = ModelParams.ForRawUnit(raw, newVersionID)
                                                                                       Dim newRawUnitID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertRawUnit, HelperDataAccess.BuildParameters(params), conn, transaction)
                                                                                       rawIdMap.Add(raw.RawUnitID, newRawUnitID)
                                                                                   Next

                                                                                   ' Duplicate ActualUnits and create map
                                                                                   Dim actualIdMap As New Dictionary(Of Integer, Integer)
                                                                                   For Each actual In uniqueActualUnits
                                                                                       Dim colorCode As String = If(actual.ColorCode Is Nothing, Nothing, actual.ColorCode.Trim())
                                                                                       params = ModelParams.ForActualUnit(actual, newVersionID, rawIdMap(actual.RawUnitID), colorCode)
                                                                                       Dim newActualUnitID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertActualUnit, HelperDataAccess.BuildParameters(params), conn, transaction)
                                                                                       actualIdMap.Add(actual.ActualUnitID, newActualUnitID)
                                                                                   Next

                                                                                   ' Duplicate CalculatedComponents
                                                                                   For Each actual In uniqueActualUnits
                                                                                       Dim newActualUnitID As Integer = actualIdMap(actual.ActualUnitID)
                                                                                       For Each comp In actual.CalculatedComponents
                                                                                           params = ModelParams.ForComponent(comp, newVersionID, newActualUnitID)
                                                                                           SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.InsertCalculatedComponent, HelperDataAccess.BuildParameters(params), conn, transaction)
                                                                                       Next
                                                                                   Next

                                                                                   ' Duplicate Buildings and create map
                                                                                   Dim buildingIdMap As New Dictionary(Of Integer, Integer)
                                                                                   For Each origBldg In origBuildings
                                                                                       params = ModelParams.ForBuilding(origBldg, newVersionID)
                                                                                       Dim newBldgID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertBuilding, HelperDataAccess.BuildParameters(params), conn, transaction)
                                                                                       buildingIdMap.Add(origBldg.BuildingID, newBldgID)
                                                                                   Next

                                                                                   ' Duplicate Levels and create map
                                                                                   Dim levelIdMap As New Dictionary(Of Integer, Integer)
                                                                                   For Each origBldg In origBuildings
                                                                                       Dim newBldgID As Integer = buildingIdMap(origBldg.BuildingID)
                                                                                       For Each origLevel In origBldg.Levels
                                                                                           params = ModelParams.ForLevel(origLevel, newVersionID, newBldgID)
                                                                                           Dim newLevelID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertLevel, HelperDataAccess.BuildParameters(params), conn, transaction)
                                                                                           levelIdMap.Add(origLevel.LevelID, newLevelID)
                                                                                       Next
                                                                                   Next

                                                                                   ' Duplicate ActualToLevelMappings
                                                                                   For Each origBldg In origBuildings
                                                                                       For Each origLevel In origBldg.Levels
                                                                                           Dim newLevelID As Integer = levelIdMap(origLevel.LevelID)
                                                                                           For Each origMapping In origLevel.ActualUnitMappings
                                                                                               params = ModelParams.ForMapping(origMapping, newVersionID, actualIdMap(origMapping.ActualUnit.ActualUnitID), newLevelID)
                                                                                               SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.InsertActualToLevelMapping, HelperDataAccess.BuildParameters(params), conn, transaction)
                                                                                           Next
                                                                                       Next
                                                                                   Next

                                                                                   transaction.Commit()
                                                                               Catch ex As Exception
                                                                                   transaction.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error duplicating project version " & originalVersionID)
        End Sub





        ' Get all versions for a project
        Public Shared Function GetProjectVersions(projectID As Integer) As List(Of ProjectVersionModel)
            Dim versions As New List(Of ProjectVersionModel)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectVersions, params)
                                                                           While reader.Read()
                                                                               Dim version As New ProjectVersionModel With {
                                                                           .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID")),
                                                                           .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                                                                           .VersionName = reader.GetString(reader.GetOrdinal("VersionName")),
                                                                           .VersionDate = If(Not reader.IsDBNull(reader.GetOrdinal("VersionDate")), reader.GetDateTime(reader.GetOrdinal("VersionDate")), Nothing),
                                                                           .Description = If(Not reader.IsDBNull(reader.GetOrdinal("Description")), reader.GetString(reader.GetOrdinal("Description")), String.Empty),
                                                                           .LastModifiedDate = If(Not reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")), reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")), Nothing),
                                                                           .CustomerID = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerID")), reader.GetInt32(reader.GetOrdinal("CustomerID")), Nothing),
                                                                           .SalesID = If(Not reader.IsDBNull(reader.GetOrdinal("SalesID")), reader.GetInt32(reader.GetOrdinal("SalesID")), Nothing),
                                                                           .CustomerName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerName")), reader.GetString(reader.GetOrdinal("CustomerName")), String.Empty),
                                                                           .SalesName = If(Not reader.IsDBNull(reader.GetOrdinal("SalesName")), reader.GetString(reader.GetOrdinal("SalesName")), String.Empty),
                                                                           .MondayID = If(Not reader.IsDBNull(reader.GetOrdinal("MondayID")), reader.GetString(reader.GetOrdinal("MondayID")), String.Empty)
                                                                       }
                                                                               versions.Add(version)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading versions for project " & projectID)
            Return versions
        End Function

        ' Update an existing project version (CustomerID restricted to CustomerType=1 via UI filtering and validation)
        Public Shared Sub UpdateProjectVersion(versionID As Integer, versionName As String, description As String, mondayid As String, customerID As Integer?, salesID As Integer?)
            ' Validate CustomerID for CustomerType=1
            If customerID.HasValue AndAlso Not HelperDataAccess.ValidateCustomerType(customerID, 1) Then
                Throw New ArgumentException("CustomerID must reference a customer with CustomerType=1 (Customer).")
            End If

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim params As New Dictionary(Of String, Object) From {
                                                                   {"@VersionID", versionID},
                                                                   {"@VersionName", versionName},
                                                                   {"@LastModifiedDate", Date.Now},
                                                                   {"@Description", If(String.IsNullOrEmpty(description), DBNull.Value, CType(description, Object))},
                                                                   {"@CustomerID", If(customerID.HasValue, CType(customerID.Value, Object), DBNull.Value)},
                                                                   {"@SalesID", If(salesID.HasValue, CType(salesID.Value, Object), DBNull.Value)},
                                                                   {"@MondayID", If(String.IsNullOrEmpty(mondayid), DBNull.Value, CType(mondayid, Object))}
                                                               }
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectVersion, HelperDataAccess.BuildParameters(params))
                                                                   End Sub, "Error updating project version " & versionID)
        End Sub
        ' In ProjVersionDataAccess.vb (add to existing class)
        Public Shared Function GetAllProjectVersions() As List(Of List(Of ProjectVersionModel))
            Dim projectVersions As New List(Of List(Of ProjectVersionModel))
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader("SELECT pv.*, c.CustomerName, s.SalesName FROM ProjectVersions pv LEFT JOIN Customer c ON pv.CustomerID = c.CustomerID AND c.CustomerType = 1 LEFT JOIN Sales s ON pv.SalesID = s.SalesID ORDER BY pv.ProjectID, pv.VersionDate DESC")
                                                                           Dim currentProjectID As Integer = -1
                                                                           Dim currentVersions As List(Of ProjectVersionModel) = Nothing
                                                                           While reader.Read()
                                                                               Dim projectID As Integer = reader.GetInt32(reader.GetOrdinal("ProjectID"))
                                                                               If projectID <> currentProjectID Then
                                                                                   currentVersions = New List(Of ProjectVersionModel)
                                                                                   projectVersions.Add(currentVersions)
                                                                                   currentProjectID = projectID
                                                                               End If
                                                                               Dim version As New ProjectVersionModel With {
                                                                                   .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID")),
                                                                                   .ProjectID = projectID,
                                                                                   .VersionName = If(Not reader.IsDBNull(reader.GetOrdinal("VersionName")), reader.GetString(reader.GetOrdinal("VersionName")), String.Empty),
                                                                                   .VersionDate = If(Not reader.IsDBNull(reader.GetOrdinal("VersionDate")), reader.GetDateTime(reader.GetOrdinal("VersionDate")), Nothing),
                                                                                   .Description = If(Not reader.IsDBNull(reader.GetOrdinal("Description")), reader.GetString(reader.GetOrdinal("Description")), String.Empty),
                                                                                   .LastModifiedDate = If(Not reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")), reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")), Nothing),
                                                                                   .CustomerID = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerID")), reader.GetInt32(reader.GetOrdinal("CustomerID")), Nothing),
                                                                                   .SalesID = If(Not reader.IsDBNull(reader.GetOrdinal("SalesID")), reader.GetInt32(reader.GetOrdinal("SalesID")), Nothing),
                                                                                   .CustomerName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerName")), reader.GetString(reader.GetOrdinal("CustomerName")), String.Empty),
                                                                                   .SalesName = If(Not reader.IsDBNull(reader.GetOrdinal("SalesName")), reader.GetString(reader.GetOrdinal("SalesName")), String.Empty)
                                                                               }
                                                                               currentVersions.Add(version)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading all project versions")
            Return projectVersions
        End Function
    End Class
End Namespace