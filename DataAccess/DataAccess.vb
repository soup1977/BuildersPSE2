Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.BuildersPSE.Utilities



Namespace BuildersPSE.DataAccess
    Public Class DataAccess
        ' Helper method to build parameters from a dictionary
        Private Function BuildParameters(params As IDictionary(Of String, Object)) As SqlParameter()
            Dim sqlParams As New List(Of SqlParameter)
            For Each kvp As KeyValuePair(Of String, Object) In params
                sqlParams.Add(New SqlParameter(kvp.Key, If(kvp.Value, DBNull.Value)))
            Next
            Return sqlParams.ToArray()
        End Function



        ' Create a new project version (CustomerID restricted to CustomerType=1 via UI filtering and validation)
        Public Function CreateProjectVersion(projectID As Integer, versionName As String, description As String, customerID As Integer?, salesID As Integer?) As Integer
            ' Validate CustomerID for CustomerType=1
            If customerID.HasValue AndAlso Not ValidateCustomerType(customerID, 1) Then
                Throw New ArgumentException("CustomerID must reference a customer with CustomerType=1 (Customer).")
            End If

            Dim newVersionID As Integer
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim params As New Dictionary(Of String, Object) From {
                                                                   {"@ProjectID", projectID},
                                                                   {"@VersionName", versionName},
                                                                   {"@VersionDate", Date.Now},
                                                                   {"@Description", If(String.IsNullOrEmpty(description), DBNull.Value, CType(description, Object))},
                                                                   {"@CustomerID", If(customerID.HasValue, CType(customerID.Value, Object), DBNull.Value)},
                                                                   {"@SalesID", If(salesID.HasValue, CType(salesID.Value, Object), DBNull.Value)}
                                                               }
                                                                       Dim newVersionIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectVersion, BuildParameters(params))
                                                                       newVersionID = CInt(newVersionIDObj)
                                                                   End Sub, "Error creating project version for " & projectID)
            Return newVersionID
        End Function

        ' Duplicate an existing project version
        ' Duplicate an existing project version
        ' Duplicate an existing project version
        ' Duplicate an existing project version
        Public Sub DuplicateProjectVersion(originalVersionID As Integer, newVersionName As String, description As String, projectID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       ' Pre-fetch all data from the original version before starting the transaction
                                                                       Dim origBuildings As List(Of BuildingModel) = GetBuildingsByVersionID(originalVersionID)
                                                                       For Each bldg In origBuildings
                                                                           For Each level In bldg.Levels
                                                                               level.ActualUnitMappings = GetActualToLevelMappingsByLevelID(level.LevelID)
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
                                                                                       Dim rawUnit As RawUnitModel = GetRawUnitByID(rawID)
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
                        {"@SalesID", DBNull.Value}
                    }
                                                                                   Dim newVersionID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertProjectVersion, BuildParameters(params), conn, transaction)

                                                                                   ' Duplicate ProjectProductSettings
                                                                                   params = New Dictionary(Of String, Object) From {
                        {"@OriginalVersionID", originalVersionID},
                        {"@NewVersionID", newVersionID}
                    }
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DuplicateProjectProductSettings, BuildParameters(params), conn, transaction)

                                                                                   ' Duplicate RawUnits and create map
                                                                                   Dim rawIdMap As New Dictionary(Of Integer, Integer)
                                                                                   For Each raw In uniqueRawUnits
                                                                                       params = New Dictionary(Of String, Object) From {
                            {"@RawUnitName", raw.RawUnitName},
                            {"@VersionID", newVersionID},
                            {"@ProductTypeID", raw.ProductTypeID},
                            {"@BF", If(raw.BF.HasValue, CType(raw.BF.Value, Object), DBNull.Value)},
                            {"@LF", If(raw.LF.HasValue, CType(raw.LF.Value, Object), DBNull.Value)},
                            {"@EWPLF", If(raw.EWPLF.HasValue, CType(raw.EWPLF.Value, Object), DBNull.Value)},
                            {"@SqFt", If(raw.SqFt.HasValue, CType(raw.SqFt.Value, Object), DBNull.Value)},
                            {"@FCArea", If(raw.FCArea.HasValue, CType(raw.FCArea.Value, Object), DBNull.Value)},
                            {"@LumberCost", If(raw.LumberCost.HasValue, CType(raw.LumberCost.Value, Object), DBNull.Value)},
                            {"@PlateCost", If(raw.PlateCost.HasValue, CType(raw.PlateCost.Value, Object), DBNull.Value)},
                            {"@ManufLaborCost", If(raw.ManufLaborCost.HasValue, CType(raw.ManufLaborCost.Value, Object), DBNull.Value)},
                            {"@DesignLabor", If(raw.DesignLabor.HasValue, CType(raw.DesignLabor.Value, Object), DBNull.Value)},
                            {"@MGMTLabor", If(raw.MGMTLabor.HasValue, CType(raw.MGMTLabor.Value, Object), DBNull.Value)},
                            {"@JobSuppliesCost", If(raw.JobSuppliesCost.HasValue, CType(raw.JobSuppliesCost.Value, Object), DBNull.Value)},
                            {"@ManHours", If(raw.ManHours.HasValue, CType(raw.ManHours.Value, Object), DBNull.Value)},
                            {"@ItemCost", If(raw.ItemCost.HasValue, CType(raw.ItemCost.Value, Object), DBNull.Value)},
                            {"@OverallCost", If(raw.OverallCost.HasValue, CType(raw.OverallCost.Value, Object), DBNull.Value)},
                            {"@DeliveryCost", If(raw.DeliveryCost.HasValue, CType(raw.DeliveryCost.Value, Object), DBNull.Value)},
                            {"@TotalSellPrice", If(raw.TotalSellPrice.HasValue, CType(raw.TotalSellPrice.Value, Object), DBNull.Value)},
                            {"@AvgSPFNo2", If(raw.AvgSPFNo2.HasValue, CType(raw.AvgSPFNo2.Value, Object), DBNull.Value)}
                        }
                                                                                       Dim newRawUnitID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertRawUnit, BuildParameters(params), conn, transaction)
                                                                                       rawIdMap.Add(raw.RawUnitID, newRawUnitID)
                                                                                   Next

                                                                                   ' Duplicate ActualUnits and create map
                                                                                   Dim actualIdMap As New Dictionary(Of Integer, Integer)
                                                                                   For Each actual In uniqueActualUnits
                                                                                       params = New Dictionary(Of String, Object) From {
                            {"@VersionID", newVersionID},
                            {"@RawUnitID", rawIdMap(actual.RawUnitID)},
                            {"@ProductTypeID", actual.ProductTypeID},
                            {"@UnitName", actual.UnitName},
                            {"@PlanSQFT", actual.PlanSQFT},
                            {"@UnitType", actual.UnitType},
                            {"@OptionalAdder", actual.OptionalAdder}
                        }
                                                                                       Dim newActualUnitID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertActualUnit, BuildParameters(params), conn, transaction)
                                                                                       actualIdMap.Add(actual.ActualUnitID, newActualUnitID)
                                                                                   Next

                                                                                   ' Duplicate CalculatedComponents
                                                                                   For Each actual In uniqueActualUnits
                                                                                       Dim newActualUnitID As Integer = actualIdMap(actual.ActualUnitID)
                                                                                       For Each comp In actual.CalculatedComponents
                                                                                           params = New Dictionary(Of String, Object) From {
                                {"@VersionID", newVersionID},
                                {"@ActualUnitID", newActualUnitID},
                                {"@ComponentType", comp.ComponentType},
                                {"@Value", comp.Value}
                            }
                                                                                           SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.InsertCalculatedComponent, BuildParameters(params), conn, transaction)
                                                                                       Next
                                                                                   Next

                                                                                   ' Duplicate Buildings and create map
                                                                                   Dim buildingIdMap As New Dictionary(Of Integer, Integer)
                                                                                   For Each origBldg In origBuildings
                                                                                       params = New Dictionary(Of String, Object) From {
                            {"@BuildingName", If(String.IsNullOrEmpty(origBldg.BuildingName), DBNull.Value, CType(origBldg.BuildingName, Object))},
                            {"@BuildingType", If(origBldg.BuildingType.HasValue, CType(origBldg.BuildingType.Value, Object), DBNull.Value)},
                            {"@ResUnits", If(origBldg.ResUnits.HasValue, CType(origBldg.ResUnits.Value, Object), DBNull.Value)},
                            {"@BldgQty", origBldg.BldgQty},
                            {"@VersionID", newVersionID}
                        }
                                                                                       Dim newBldgID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertBuilding, BuildParameters(params), conn, transaction)
                                                                                       buildingIdMap.Add(origBldg.BuildingID, newBldgID)
                                                                                   Next

                                                                                   ' Duplicate Levels and create map
                                                                                   Dim levelIdMap As New Dictionary(Of Integer, Integer)
                                                                                   For Each origBldg In origBuildings
                                                                                       Dim newBldgID As Integer = buildingIdMap(origBldg.BuildingID)
                                                                                       For Each origLevel In origBldg.Levels
                                                                                           params = New Dictionary(Of String, Object) From {
                                {"@VersionID", newVersionID},
                                {"@BuildingID", newBldgID},
                                {"@ProductTypeID", origLevel.ProductTypeID},
                                {"@LevelNumber", origLevel.LevelNumber},
                                {"@LevelName", origLevel.LevelName}
                            }
                                                                                           Dim newLevelID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertLevel, BuildParameters(params), conn, transaction)
                                                                                           levelIdMap.Add(origLevel.LevelID, newLevelID)
                                                                                       Next
                                                                                   Next

                                                                                   ' Duplicate ActualToLevelMappings
                                                                                   For Each origBldg In origBuildings
                                                                                       For Each origLevel In origBldg.Levels
                                                                                           Dim newLevelID As Integer = levelIdMap(origLevel.LevelID)
                                                                                           For Each origMapping In origLevel.ActualUnitMappings
                                                                                               params = New Dictionary(Of String, Object) From {
                                    {"@VersionID", newVersionID},
                                    {"@ActualUnitID", actualIdMap(origMapping.ActualUnitID)},
                                    {"@LevelID", newLevelID},
                                    {"@Quantity", origMapping.Quantity}
                                }
                                                                                               SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.InsertActualToLevelMapping, BuildParameters(params), conn, transaction)
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
        Public Function GetProjectVersions(projectID As Integer) As List(Of ProjectVersionModel)
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
                                                                           .SalesName = If(Not reader.IsDBNull(reader.GetOrdinal("SalesName")), reader.GetString(reader.GetOrdinal("SalesName")), String.Empty)
                                                                       }
                                                                               versions.Add(version)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading versions for project " & projectID)
            Return versions
        End Function

        ' Update an existing project version (CustomerID restricted to CustomerType=1 via UI filtering and validation)
        Public Sub UpdateProjectVersion(versionID As Integer, versionName As String, description As String, customerID As Integer?, salesID As Integer?)
            ' Validate CustomerID for CustomerType=1
            If customerID.HasValue AndAlso Not ValidateCustomerType(customerID, 1) Then
                Throw New ArgumentException("CustomerID must reference a customer with CustomerType=1 (Customer).")
            End If

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim params As New Dictionary(Of String, Object) From {
                                                                   {"@VersionID", versionID},
                                                                   {"@VersionName", versionName},
                                                                   {"@Description", If(String.IsNullOrEmpty(description), DBNull.Value, CType(description, Object))},
                                                                   {"@CustomerID", If(customerID.HasValue, CType(customerID.Value, Object), DBNull.Value)},
                                                                   {"@SalesID", If(salesID.HasValue, CType(salesID.Value, Object), DBNull.Value)}
                                                               }
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectVersion, BuildParameters(params))
                                                                   End Sub, "Error updating project version " & versionID)
        End Sub
        ' Helper method to validate CustomerType for a given CustomerID
        Private Function ValidateCustomerType(customerID As Integer?, requiredType As Integer) As Boolean
            If Not customerID.HasValue Then Return True ' Allow NULL as valid (nullable field)
            Dim params As SqlParameter() = {New SqlParameter("@CustomerID", customerID.Value)}
            Dim typeObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)("SELECT CustomerType FROM Customer WHERE CustomerID = @CustomerID", params)
            If typeObj Is DBNull.Value OrElse typeObj Is Nothing Then
                Return False ' CustomerID not found
            End If
            Return CInt(typeObj) = requiredType
        End Function

        ' Updated: SaveProject (validates ArchitectID and EngineerID for CustomerType=2 and 3)
        Public Sub SaveProject(proj As ProjectModel)
            ' Validate CustomerType for ArchitectID (2) and EngineerID (3)
            If proj.ArchitectID.HasValue AndAlso Not ValidateCustomerType(proj.ArchitectID, 2) Then
                Throw New ArgumentException("ArchitectID must reference a customer with CustomerType=2 (Architect).")
            End If
            If proj.EngineerID.HasValue AndAlso Not ValidateCustomerType(proj.EngineerID, 3) Then
                Throw New ArgumentException("EngineerID must reference a customer with CustomerType=3 (Engineer).")
            End If

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim paramsDict As New Dictionary(Of String, Object) From {
                                                          {"@JBID", If(String.IsNullOrEmpty(proj.JBID), DBNull.Value, CType(proj.JBID, Object))},
                                                          {"@ProjectTypeID", proj.ProjectType.ProjectTypeID},
                                                          {"@ProjectName", If(String.IsNullOrEmpty(proj.ProjectName), DBNull.Value, CType(proj.ProjectName, Object))},
                                                          {"@EstimatorID", proj.Estimator.EstimatorID},
                                                          {"@Address", If(String.IsNullOrEmpty(proj.Address), DBNull.Value, CType(proj.Address, Object))},
                                                          {"@City", If(String.IsNullOrEmpty(proj.City), DBNull.Value, CType(proj.City, Object))},
                                                          {"@State", If(String.IsNullOrEmpty(proj.State), DBNull.Value, CType(proj.State, Object))},
                                                          {"@Zip", If(String.IsNullOrEmpty(proj.Zip), DBNull.Value, CType(proj.Zip, Object))},
                                                          {"@BidDate", If(proj.BidDate.HasValue, CType(proj.BidDate.Value, Object), DBNull.Value)},
                                                          {"@ArchPlansDated", If(proj.ArchPlansDated.HasValue, CType(proj.ArchPlansDated.Value, Object), DBNull.Value)},
                                                          {"@EngPlansDated", If(proj.EngPlansDated.HasValue, CType(proj.EngPlansDated.Value, Object), DBNull.Value)},
                                                          {"@MilesToJobSite", proj.MilesToJobSite},
                                                          {"@TotalNetSqft", proj.TotalNetSqft},
                                                          {"@TotalGrossSqft", proj.TotalGrossSqft},
                                                          {"@ArchitectID", If(proj.ArchitectID.HasValue, CType(proj.ArchitectID.Value, Object), DBNull.Value)},
                                                          {"@EngineerID", If(proj.EngineerID.HasValue, CType(proj.EngineerID.Value, Object), DBNull.Value)},
                                                          {"@ProjectNotes", If(String.IsNullOrEmpty(proj.ProjectNotes), DBNull.Value, CType(proj.ProjectNotes, Object))}
                                                      }
                                                                       If proj.ProjectID = 0 Then
                                                                           Dim newID As Integer = SqlConnectionManager.Instance.ExecuteScalar(Of Integer)(Queries.InsertProject, BuildParameters(paramsDict))
                                                                           proj.ProjectID = newID
                                                                       Else
                                                                           paramsDict.Add("@ProjectID", proj.ProjectID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProject, BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving project " & proj.ProjectID)
        End Sub

        ' Updated: GetProjects (use VersionID for details)
        Public Function GetProjects(Optional includeDetails As Boolean = True) As List(Of ProjectModel)
            Dim projects As New List(Of ProjectModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjects)
                                                                           While reader.Read()
                                                                               Dim proj As New ProjectModel With {
                                                                          .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                                                                          .JBID = If(Not reader.IsDBNull(reader.GetOrdinal("JBID")), reader.GetString(reader.GetOrdinal("JBID")), String.Empty),
                                                                          .ProjectType = New ProjectTypeModel With {
                                                                              .ProjectTypeID = reader.GetInt32(reader.GetOrdinal("ProjectTypeID")),
                                                                              .ProjectTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectType")), reader.GetString(reader.GetOrdinal("ProjectType")), String.Empty)
                                                                          },
                                                                          .ProjectName = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectName")), reader.GetString(reader.GetOrdinal("ProjectName")), String.Empty),
                                                                          .Estimator = New EstimatorModel With {
                                                                              .EstimatorID = reader.GetInt32(reader.GetOrdinal("EstimatorID")),
                                                                              .EstimatorName = If(Not reader.IsDBNull(reader.GetOrdinal("Estimator")), reader.GetString(reader.GetOrdinal("Estimator")), String.Empty)
                                                                          },
                                                                          .Address = If(Not reader.IsDBNull(reader.GetOrdinal("Address")), reader.GetString(reader.GetOrdinal("Address")), String.Empty),
                                                                          .City = If(Not reader.IsDBNull(reader.GetOrdinal("City")), reader.GetString(reader.GetOrdinal("City")), String.Empty),
                                                                          .State = If(Not reader.IsDBNull(reader.GetOrdinal("State")), reader.GetString(reader.GetOrdinal("State")), String.Empty),
                                                                          .Zip = If(Not reader.IsDBNull(reader.GetOrdinal("Zip")), reader.GetString(reader.GetOrdinal("Zip")), String.Empty),
                                                                          .BidDate = If(Not reader.IsDBNull(reader.GetOrdinal("BidDate")), reader.GetDateTime(reader.GetOrdinal("BidDate")), Nothing),
                                                                          .ArchPlansDated = If(Not reader.IsDBNull(reader.GetOrdinal("ArchPlansDated")), reader.GetDateTime(reader.GetOrdinal("ArchPlansDated")), Nothing),
                                                                          .EngPlansDated = If(Not reader.IsDBNull(reader.GetOrdinal("EngPlansDated")), reader.GetDateTime(reader.GetOrdinal("EngPlansDated")), Nothing),
                                                                          .MilesToJobSite = If(Not reader.IsDBNull(reader.GetOrdinal("MilesToJobSite")), reader.GetInt32(reader.GetOrdinal("MilesToJobSite")), 0),
                                                                          .TotalNetSqft = If(Not reader.IsDBNull(reader.GetOrdinal("TotalNetSqft")), reader.GetInt32(reader.GetOrdinal("TotalNetSqft")), Nothing),
                                                                          .TotalGrossSqft = If(Not reader.IsDBNull(reader.GetOrdinal("TotalGrossSqft")), reader.GetInt32(reader.GetOrdinal("TotalGrossSqft")), 0),
                                                                          .ArchitectID = If(Not reader.IsDBNull(reader.GetOrdinal("ArchitectID")), reader.GetInt32(reader.GetOrdinal("ArchitectID")), Nothing),
                                                                          .EngineerID = If(Not reader.IsDBNull(reader.GetOrdinal("EngineerID")), reader.GetInt32(reader.GetOrdinal("EngineerID")), Nothing),
                                                                          .ProjectNotes = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectNotes")), reader.GetString(reader.GetOrdinal("ProjectNotes")), String.Empty),
                                                                          .LastModifiedDate = If(Not reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")), reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")), Nothing),
                                                                          .CreatedDate = If(Not reader.IsDBNull(reader.GetOrdinal("createddate")), reader.GetDateTime(reader.GetOrdinal("createddate")), Nothing),
                                                                          .ArchitectName = If(Not reader.IsDBNull(reader.GetOrdinal("ArchitectName")), reader.GetString(reader.GetOrdinal("ArchitectName")), String.Empty),
                                                                          .EngineerName = If(Not reader.IsDBNull(reader.GetOrdinal("EngineerName")), reader.GetString(reader.GetOrdinal("EngineerName")), String.Empty)
                                                                      }

                                                                               If includeDetails Then
                                                                                   Dim versions As List(Of ProjectVersionModel) = GetProjectVersions(proj.ProjectID)
                                                                                   If versions.Any() Then
                                                                                       Dim latestVersionID As Integer = versions.First().VersionID
                                                                                       proj.Buildings = GetBuildingsByVersionID(latestVersionID)
                                                                                       proj.Settings = GetProjectProductSettings(latestVersionID)
                                                                                   End If
                                                                               End If

                                                                               projects.Add(proj)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading Project")
            Return projects
        End Function

        ' Updated: GetProjectByID (use VersionID for details)
        Public Function GetProjectByID(projectID As Integer) As ProjectModel
            Dim proj As ProjectModel = Nothing
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectByID, params)
                                                                           If reader.Read() Then
                                                                               proj = New ProjectModel With {
                                                                          .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                                                                          .JBID = If(Not reader.IsDBNull(reader.GetOrdinal("JBID")), reader.GetString(reader.GetOrdinal("JBID")), String.Empty),
                                                                          .ProjectType = New ProjectTypeModel With {
                                                                              .ProjectTypeID = reader.GetInt32(reader.GetOrdinal("ProjectTypeID")),
                                                                              .ProjectTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectType")), reader.GetString(reader.GetOrdinal("ProjectType")), String.Empty)
                                                                          },
                                                                          .ProjectName = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectName")), reader.GetString(reader.GetOrdinal("ProjectName")), String.Empty),
                                                                          .Estimator = New EstimatorModel With {
                                                                              .EstimatorID = reader.GetInt32(reader.GetOrdinal("EstimatorID")),
                                                                              .EstimatorName = If(Not reader.IsDBNull(reader.GetOrdinal("Estimator")), reader.GetString(reader.GetOrdinal("Estimator")), String.Empty)
                                                                          },
                                                                          .Address = If(Not reader.IsDBNull(reader.GetOrdinal("Address")), reader.GetString(reader.GetOrdinal("Address")), String.Empty),
                                                                          .City = If(Not reader.IsDBNull(reader.GetOrdinal("City")), reader.GetString(reader.GetOrdinal("City")), String.Empty),
                                                                          .State = If(Not reader.IsDBNull(reader.GetOrdinal("State")), reader.GetString(reader.GetOrdinal("State")), String.Empty),
                                                                          .Zip = If(Not reader.IsDBNull(reader.GetOrdinal("Zip")), reader.GetString(reader.GetOrdinal("Zip")), String.Empty),
                                                                          .BidDate = If(Not reader.IsDBNull(reader.GetOrdinal("BidDate")), reader.GetDateTime(reader.GetOrdinal("BidDate")), Nothing),
                                                                          .ArchPlansDated = If(Not reader.IsDBNull(reader.GetOrdinal("ArchPlansDated")), reader.GetDateTime(reader.GetOrdinal("ArchPlansDated")), Nothing),
                                                                          .EngPlansDated = If(Not reader.IsDBNull(reader.GetOrdinal("EngPlansDated")), reader.GetDateTime(reader.GetOrdinal("EngPlansDated")), Nothing),
                                                                          .MilesToJobSite = If(Not reader.IsDBNull(reader.GetOrdinal("MilesToJobSite")), reader.GetInt32(reader.GetOrdinal("MilesToJobSite")), 0),
                                                                          .TotalNetSqft = If(Not reader.IsDBNull(reader.GetOrdinal("TotalNetSqft")), reader.GetInt32(reader.GetOrdinal("TotalNetSqft")), Nothing),
                                                                          .TotalGrossSqft = If(Not reader.IsDBNull(reader.GetOrdinal("TotalGrossSqft")), reader.GetInt32(reader.GetOrdinal("TotalGrossSqft")), 0),
                                                                          .ArchitectID = If(Not reader.IsDBNull(reader.GetOrdinal("ArchitectID")), reader.GetInt32(reader.GetOrdinal("ArchitectID")), Nothing),
                                                                          .EngineerID = If(Not reader.IsDBNull(reader.GetOrdinal("EngineerID")), reader.GetInt32(reader.GetOrdinal("EngineerID")), Nothing),
                                                                          .ProjectNotes = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectNotes")), reader.GetString(reader.GetOrdinal("ProjectNotes")), String.Empty),
                                                                          .LastModifiedDate = If(Not reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")), reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")), Nothing),
                                                                          .CreatedDate = If(Not reader.IsDBNull(reader.GetOrdinal("createddate")), reader.GetDateTime(reader.GetOrdinal("createddate")), Nothing),
                                                                          .ArchitectName = If(Not reader.IsDBNull(reader.GetOrdinal("ArchitectName")), reader.GetString(reader.GetOrdinal("ArchitectName")), String.Empty),
                                                                          .EngineerName = If(Not reader.IsDBNull(reader.GetOrdinal("EngineerName")), reader.GetString(reader.GetOrdinal("EngineerName")), String.Empty)
                                                                      }
                                                                               Dim versions As List(Of ProjectVersionModel) = GetProjectVersions(proj.ProjectID)
                                                                               If versions.Any() Then
                                                                                   Dim latestVersionID As Integer = versions.First().VersionID
                                                                                   proj.Buildings = GetBuildingsByVersionID(latestVersionID)
                                                                                   proj.Settings = GetProjectProductSettings(latestVersionID)
                                                                               End If
                                                                           End If
                                                                       End Using
                                                                   End Sub, "Error loading project by ID " & projectID)
            Return proj
        End Function
        ' Added: GetBuildingsByVersionID (use VersionID)
        Public Function GetBuildingsByVersionID(versionID As Integer) As List(Of BuildingModel)
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
        ' Updated: ImportRawUnits (use VersionID)
        Public Sub ImportRawUnits(versionID As Integer, csvPath As String, productTypeID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using parser As New Microsoft.VisualBasic.FileIO.TextFieldParser(csvPath)
                                                                           parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited
                                                                           parser.Delimiters = New String() {","}
                                                                           parser.HasFieldsEnclosedInQuotes = True
                                                                           parser.TrimWhiteSpace = True
                                                                           If Not parser.EndOfData Then
                                                                               Dim headers As String() = parser.ReadFields()
                                                                               Dim skippedHeaders As New HashSet(Of String) From {"JOBNUMBER", "PROJECT", "CUSTOMER", "JOBNAME", "STRUCTURENAME", "PLAN"}
                                                                               While Not parser.EndOfData
                                                                                   Dim fields As String() = parser.ReadFields()
                                                                                   If fields.Length <> headers.Length Then Continue While
                                                                                   Dim rawUnit As New RawUnitModel With {
                                                                              .VersionID = versionID,
                                                                              .ProductTypeID = productTypeID
                                                                          }
                                                                                   Dim elevation As String = String.Empty
                                                                                   Dim productStr As String = String.Empty
                                                                                   For i As Integer = 0 To headers.Length - 1
                                                                                       Dim header As String = headers(i).Trim().ToUpper()
                                                                                       Dim valueStr As String = fields(i).Trim()
                                                                                       If skippedHeaders.Contains(header) Then Continue For
                                                                                       If header = "ELEVATION" Then
                                                                                           elevation = valueStr
                                                                                       ElseIf header = "PRODUCT" Then
                                                                                           productStr = valueStr
                                                                                           If String.Equals(productStr, "Floor", StringComparison.OrdinalIgnoreCase) Then rawUnit.ProductTypeID = 1
                                                                                           If String.Equals(productStr, "Roof", StringComparison.OrdinalIgnoreCase) Then rawUnit.ProductTypeID = 2
                                                                                       Else
                                                                                           Dim tempVal As Decimal
                                                                                           If Decimal.TryParse(valueStr, tempVal) Then
                                                                                               Dim val As Decimal? = tempVal
                                                                                               Select Case header
                                                                                                   Case "BF" : rawUnit.BF = val
                                                                                                   Case "LF" : rawUnit.LF = val
                                                                                                   Case "EWPLF" : rawUnit.EWPLF = val
                                                                                                   Case "SQFT" : rawUnit.SqFt = val
                                                                                                   Case "FCAREA" : rawUnit.FCArea = val
                                                                                                   Case "LUMBERCOST" : rawUnit.LumberCost = val
                                                                                                   Case "PLATECOST" : rawUnit.PlateCost = val
                                                                                                   Case "MANUFLABORCOST" : rawUnit.ManufLaborCost = val
                                                                                                   Case "DESIGNLABOR" : rawUnit.DesignLabor = val
                                                                                                   Case "MGMTLABOR" : rawUnit.MGMTLabor = val
                                                                                                   Case "JOBSUPPLIESCOST" : rawUnit.JobSuppliesCost = val
                                                                                                   Case "MANHOURS" : rawUnit.ManHours = val
                                                                                                   Case "ITEMCOST" : rawUnit.ItemCost = val
                                                                                                   Case "OVERALLCOST" : rawUnit.OverallCost = val
                                                                                                   Case "DELIVERYCOST" : rawUnit.DeliveryCost = val
                                                                                                   Case "TOTALSELLPRICE" : rawUnit.TotalSellPrice = val
                                                                                                   Case "AVGSPFNO2" : rawUnit.AvgSPFNo2 = val
                                                                                               End Select
                                                                                           End If
                                                                                       End If
                                                                                   Next
                                                                                   If Not String.IsNullOrEmpty(elevation) Then
                                                                                       rawUnit.RawUnitName = If(Not String.IsNullOrEmpty(productStr), elevation & " " & productStr, elevation)
                                                                                   End If
                                                                                   Dim insertParams As New Dictionary(Of String, Object) From {
                                                                              {"@RawUnitName", If(String.IsNullOrEmpty(rawUnit.RawUnitName), DBNull.Value, CType(rawUnit.RawUnitName, Object))},
                                                                              {"@VersionID", rawUnit.VersionID},
                                                                              {"@ProductTypeID", rawUnit.ProductTypeID},
                                                                              {"@BF", If(rawUnit.BF.HasValue, CType(rawUnit.BF.Value, Object), DBNull.Value)},
                                                                              {"@LF", If(rawUnit.LF.HasValue, CType(rawUnit.LF.Value, Object), DBNull.Value)},
                                                                              {"@EWPLF", If(rawUnit.EWPLF.HasValue, CType(rawUnit.EWPLF.Value, Object), DBNull.Value)},
                                                                              {"@SqFt", If(rawUnit.SqFt.HasValue, CType(rawUnit.SqFt.Value, Object), DBNull.Value)},
                                                                              {"@FCArea", If(rawUnit.FCArea.HasValue, CType(rawUnit.FCArea.Value, Object), DBNull.Value)},
                                                                              {"@LumberCost", If(rawUnit.LumberCost.HasValue, CType(rawUnit.LumberCost.Value, Object), DBNull.Value)},
                                                                              {"@PlateCost", If(rawUnit.PlateCost.HasValue, CType(rawUnit.PlateCost.Value, Object), DBNull.Value)},
                                                                              {"@ManufLaborCost", If(rawUnit.ManufLaborCost.HasValue, CType(rawUnit.ManufLaborCost.Value, Object), DBNull.Value)},
                                                                              {"@DesignLabor", If(rawUnit.DesignLabor.HasValue, CType(rawUnit.DesignLabor.Value, Object), DBNull.Value)},
                                                                              {"@MGMTLabor", If(rawUnit.MGMTLabor.HasValue, CType(rawUnit.MGMTLabor.Value, Object), DBNull.Value)},
                                                                              {"@JobSuppliesCost", If(rawUnit.JobSuppliesCost.HasValue, CType(rawUnit.JobSuppliesCost.Value, Object), DBNull.Value)},
                                                                              {"@ManHours", If(rawUnit.ManHours.HasValue, CType(rawUnit.ManHours.Value, Object), DBNull.Value)},
                                                                              {"@ItemCost", If(rawUnit.ItemCost.HasValue, CType(rawUnit.ItemCost.Value, Object), DBNull.Value)},
                                                                              {"@OverallCost", If(rawUnit.OverallCost.HasValue, CType(rawUnit.OverallCost.Value, Object), DBNull.Value)},
                                                                              {"@DeliveryCost", If(rawUnit.DeliveryCost.HasValue, CType(rawUnit.DeliveryCost.Value, Object), DBNull.Value)},
                                                                              {"@TotalSellPrice", If(rawUnit.TotalSellPrice.HasValue, CType(rawUnit.TotalSellPrice.Value, Object), DBNull.Value)},
                                                                              {"@AvgSPFNo2", If(rawUnit.AvgSPFNo2.HasValue, CType(rawUnit.AvgSPFNo2.Value, Object), DBNull.Value)}
                                                                          }
                                                                                   Dim rawIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertRawUnit, BuildParameters(insertParams))
                                                                                   If rawIDObj Is DBNull.Value OrElse rawIDObj Is Nothing Then
                                                                                       Throw New Exception("Failed to insert RawUnit for " & rawUnit.RawUnitName)
                                                                                   End If
                                                                                   rawUnit.RawUnitID = CInt(rawIDObj)
                                                                               End While
                                                                           End If
                                                                       End Using
                                                                   End Sub, "RawUnits import failed for version " & versionID)
        End Sub
        ' Add to DataAccess.vb
        Public Function InsertRawUnit(rawUnit As RawUnitModel) As Integer
            Dim params As New Dictionary(Of String, Object) From {
        {"@RawUnitName", If(String.IsNullOrEmpty(rawUnit.RawUnitName), DBNull.Value, CType(rawUnit.RawUnitName, Object))},
        {"@VersionID", rawUnit.VersionID},
        {"@ProductTypeID", rawUnit.ProductTypeID},
        {"@BF", If(rawUnit.BF.HasValue, CType(rawUnit.BF.Value, Object), DBNull.Value)},
        {"@LF", If(rawUnit.LF.HasValue, CType(rawUnit.LF.Value, Object), DBNull.Value)},
        {"@EWPLF", If(rawUnit.EWPLF.HasValue, CType(rawUnit.EWPLF.Value, Object), DBNull.Value)},
        {"@SqFt", If(rawUnit.SqFt.HasValue, CType(rawUnit.SqFt.Value, Object), DBNull.Value)},
        {"@FCArea", If(rawUnit.FCArea.HasValue, CType(rawUnit.FCArea.Value, Object), DBNull.Value)},
        {"@LumberCost", If(rawUnit.LumberCost.HasValue, CType(rawUnit.LumberCost.Value, Object), DBNull.Value)},
        {"@PlateCost", If(rawUnit.PlateCost.HasValue, CType(rawUnit.PlateCost.Value, Object), DBNull.Value)},
        {"@ManufLaborCost", If(rawUnit.ManufLaborCost.HasValue, CType(rawUnit.ManufLaborCost.Value, Object), DBNull.Value)},
        {"@DesignLabor", If(rawUnit.DesignLabor.HasValue, CType(rawUnit.DesignLabor.Value, Object), DBNull.Value)},
        {"@MGMTLabor", If(rawUnit.MGMTLabor.HasValue, CType(rawUnit.MGMTLabor.Value, Object), DBNull.Value)},
        {"@JobSuppliesCost", If(rawUnit.JobSuppliesCost.HasValue, CType(rawUnit.JobSuppliesCost.Value, Object), DBNull.Value)},
        {"@ManHours", If(rawUnit.ManHours.HasValue, CType(rawUnit.ManHours.Value, Object), DBNull.Value)},
        {"@ItemCost", If(rawUnit.ItemCost.HasValue, CType(rawUnit.ItemCost.Value, Object), DBNull.Value)},
        {"@OverallCost", If(rawUnit.OverallCost.HasValue, CType(rawUnit.OverallCost.Value, Object), DBNull.Value)},
        {"@DeliveryCost", If(rawUnit.DeliveryCost.HasValue, CType(rawUnit.DeliveryCost.Value, Object), DBNull.Value)},
        {"@TotalSellPrice", If(rawUnit.TotalSellPrice.HasValue, CType(rawUnit.TotalSellPrice.Value, Object), DBNull.Value)},
        {"@AvgSPFNo2", If(rawUnit.AvgSPFNo2.HasValue, CType(rawUnit.AvgSPFNo2.Value, Object), DBNull.Value)}
    }
            Dim newID As Integer
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertRawUnit, BuildParameters(params))
                                                                       If newIDObj Is DBNull.Value OrElse newIDObj Is Nothing Then
                                                                           Throw New Exception("Failed to insert RawUnit for " & rawUnit.RawUnitName)
                                                                       End If
                                                                       newID = CInt(newIDObj)
                                                                   End Sub, "Error inserting raw unit for version " & rawUnit.VersionID)
            Return newID
        End Function

        Public Sub UpdateRawUnit(rawUnit As RawUnitModel)
            Dim params As New Dictionary(Of String, Object) From {
        {"@RawUnitID", rawUnit.RawUnitID},
        {"@RawUnitName", If(String.IsNullOrEmpty(rawUnit.RawUnitName), DBNull.Value, CType(rawUnit.RawUnitName, Object))},
        {"@BF", If(rawUnit.BF.HasValue, CType(rawUnit.BF.Value, Object), DBNull.Value)},
        {"@LF", If(rawUnit.LF.HasValue, CType(rawUnit.LF.Value, Object), DBNull.Value)},
        {"@EWPLF", If(rawUnit.EWPLF.HasValue, CType(rawUnit.EWPLF.Value, Object), DBNull.Value)},
        {"@SqFt", If(rawUnit.SqFt.HasValue, CType(rawUnit.SqFt.Value, Object), DBNull.Value)},
        {"@FCArea", If(rawUnit.FCArea.HasValue, CType(rawUnit.FCArea.Value, Object), DBNull.Value)},
        {"@LumberCost", If(rawUnit.LumberCost.HasValue, CType(rawUnit.LumberCost.Value, Object), DBNull.Value)},
        {"@PlateCost", If(rawUnit.PlateCost.HasValue, CType(rawUnit.PlateCost.Value, Object), DBNull.Value)},
        {"@ManufLaborCost", If(rawUnit.ManufLaborCost.HasValue, CType(rawUnit.ManufLaborCost.Value, Object), DBNull.Value)},
        {"@DesignLabor", If(rawUnit.DesignLabor.HasValue, CType(rawUnit.DesignLabor.Value, Object), DBNull.Value)},
        {"@MGMTLabor", If(rawUnit.MGMTLabor.HasValue, CType(rawUnit.MGMTLabor.Value, Object), DBNull.Value)},
        {"@JobSuppliesCost", If(rawUnit.JobSuppliesCost.HasValue, CType(rawUnit.JobSuppliesCost.Value, Object), DBNull.Value)},
        {"@ManHours", If(rawUnit.ManHours.HasValue, CType(rawUnit.ManHours.Value, Object), DBNull.Value)},
        {"@ItemCost", If(rawUnit.ItemCost.HasValue, CType(rawUnit.ItemCost.Value, Object), DBNull.Value)},
        {"@OverallCost", If(rawUnit.OverallCost.HasValue, CType(rawUnit.OverallCost.Value, Object), DBNull.Value)},
        {"@DeliveryCost", If(rawUnit.DeliveryCost.HasValue, CType(rawUnit.DeliveryCost.Value, Object), DBNull.Value)},
        {"@TotalSellPrice", If(rawUnit.TotalSellPrice.HasValue, CType(rawUnit.TotalSellPrice.Value, Object), DBNull.Value)},
        {"@AvgSPFNo2", If(rawUnit.AvgSPFNo2.HasValue, CType(rawUnit.AvgSPFNo2.Value, Object), DBNull.Value)}
    }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateRawUnit, BuildParameters(params))
                                                                   End Sub, "Error updating raw unit " & rawUnit.RawUnitID)
        End Sub

        ' GetLevelsByBuildingID (unchanged, uses BuildingID)
        Public Function GetLevelsByBuildingID(buildingID As Integer) As List(Of LevelModel)
            Dim levels As New List(Of LevelModel)
            Dim params As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectLevelsByBuilding, params)
                                                                           While reader.Read()
                                                                               Dim level As New LevelModel With {
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
                                                 }
                                                                               levels.Add(level)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading levels for building " & buildingID)
            Return levels
        End Function

        ' GetBuildingIDByLevelID (unchanged)
        Public Function GetBuildingIDByLevelID(levelID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
            Dim buildingIDObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       buildingIDObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.GetBuildingIDByLevelID, params)
                                                                   End Sub, "Error fetching BuildingID for LevelID " & levelID)
            Return If(buildingIDObj Is DBNull.Value OrElse buildingIDObj Is Nothing, 0, CInt(buildingIDObj))
        End Function

        ' GetProductTypes (unchanged)
        Public Function GetProductTypes() As List(Of ProductTypeModel)
            Dim types As New List(Of ProductTypeModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProductTypes)
                                                                           While reader.Read()
                                                                               Dim pt As New ProductTypeModel With {
                                                     .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                                     .ProductTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("ProductTypeName")), reader.GetString(reader.GetOrdinal("ProductTypeName")), String.Empty)
                                                 }
                                                                               types.Add(pt)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading product types")
            Return types
        End Function

        ' Updated: GetProjectProductSettings (use VersionID)
        Public Function GetProjectProductSettings(versionID As Integer) As List(Of ProjectProductSettingsModel)
            Dim settings As New List(Of ProjectProductSettingsModel)
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID)}
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectProductSettings, params)
                                                                           While reader.Read()
                                                                               Dim setting As New ProjectProductSettingsModel With {
                                                     .SettingID = reader.GetInt32(reader.GetOrdinal("SettingID")),
                                                     .VersionID = versionID,
                                                     .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                                     .MarginPercent = If(Not reader.IsDBNull(reader.GetOrdinal("MarginPercent")), reader.GetDecimal(reader.GetOrdinal("MarginPercent")), Nothing),
                                                     .LumberAdder = If(Not reader.IsDBNull(reader.GetOrdinal("LumberAdder")), reader.GetDecimal(reader.GetOrdinal("LumberAdder")), Nothing)
                                                 }
                                                                               settings.Add(setting)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading settings for version " & versionID)
            Return settings
        End Function

        ' Updated: SaveBuilding (use VersionID)
        Public Sub SaveBuilding(bldg As BuildingModel, versionID As Integer)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@BuildingName", If(String.IsNullOrEmpty(bldg.BuildingName), DBNull.Value, CType(bldg.BuildingName, Object))},
                {"@BuildingType", If(bldg.BuildingType.HasValue, CType(bldg.BuildingType.Value, Object), DBNull.Value)},
                {"@ResUnits", If(bldg.ResUnits.HasValue, CType(bldg.ResUnits.Value, Object), DBNull.Value)},
                {"@BldgQty", bldg.BldgQty},
                {"@VersionID", versionID}
            }

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If bldg.BuildingID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertBuilding, BuildParameters(paramsDict))
                                                                           bldg.BuildingID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@BuildingID", bldg.BuildingID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateBuilding, BuildParameters(paramsDict))
                                                                       End If

                                                                       For Each level In bldg.Levels
                                                                           SaveLevel(level, bldg.BuildingID, versionID)
                                                                       Next
                                                                   End Sub, "Error saving building")
        End Sub

        ' Updated: SaveLevel (use VersionID)
        Public Sub SaveLevel(level As LevelModel, buildingID As Integer, versionID As Integer)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@VersionID", versionID},
                {"@BuildingID", buildingID},
                {"@ProductTypeID", level.ProductTypeID},
                {"@LevelNumber", level.LevelNumber},
                {"@LevelName", level.LevelName}
            }

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If level.LevelID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertLevel, BuildParameters(paramsDict))
                                                                           level.LevelID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@LevelID", level.LevelID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLevel, BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving level")
        End Sub

        ' Updated: SaveProjectProductSetting (use VersionID)
        Public Sub SaveProjectProductSetting(setting As ProjectProductSettingsModel, versionID As Integer)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@VersionID", versionID},
                {"@ProductTypeID", setting.ProductTypeID},
                {"@MarginPercent", If(setting.MarginPercent.HasValue, CType(setting.MarginPercent.Value, Object), DBNull.Value)},
                {"@LumberAdder", If(setting.LumberAdder.HasValue AndAlso setting.LumberAdder.Value > 0, CType(setting.LumberAdder.Value, Object), DBNull.Value)}
            }

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If setting.SettingID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectProductSetting, BuildParameters(paramsDict))
                                                                           If newIDObj IsNot Nothing Then setting.SettingID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@SettingID", setting.SettingID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectProductSetting, BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving project setting")
        End Sub

        ' GetProjectTypes (unchanged)
        Public Function GetProjectTypes() As List(Of ProjectTypeModel)
            Dim types As New List(Of ProjectTypeModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectTypes)
                                                                           While reader.Read()
                                                                               Dim pt As New ProjectTypeModel With {
                                                    .ProjectTypeID = reader.GetInt32(reader.GetOrdinal("ProjectTypeID")),
                                                    .ProjectTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectTypeName")), reader.GetString(reader.GetOrdinal("ProjectTypeName")), String.Empty),
                                                    .Description = If(Not reader.IsDBNull(reader.GetOrdinal("Description")), reader.GetString(reader.GetOrdinal("Description")), String.Empty)
                                                }
                                                                               types.Add(pt)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading project types")
            Return types
        End Function

        ' SaveProjectType (unchanged)
        Public Function SaveProjectType(projectType As ProjectTypeModel) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@ProjectTypeName", If(String.IsNullOrEmpty(projectType.ProjectTypeName), DBNull.Value, CType(projectType.ProjectTypeName, Object))},
                {"@Description", If(String.IsNullOrEmpty(projectType.Description), DBNull.Value, CType(projectType.Description, Object))}
            }

            Dim result As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If projectType.ProjectTypeID = 0 Then
                                                                           result = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectType, BuildParameters(paramsDict)))
                                                                       Else
                                                                           paramsDict.Add("@ProjectTypeID", projectType.ProjectTypeID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectType, BuildParameters(paramsDict))
                                                                           result = projectType.ProjectTypeID
                                                                       End If
                                                                   End Sub, "Error saving project type")
            Return result
        End Function

        ' GetEstimators (unchanged)
        Public Function GetEstimators() As List(Of EstimatorModel)
            Dim estimators As New List(Of EstimatorModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectEstimators)
                                                                           While reader.Read()
                                                                               Dim estimator As New EstimatorModel With {
                                                    .EstimatorID = reader.GetInt32(reader.GetOrdinal("EstimatorID")),
                                                    .EstimatorName = If(Not reader.IsDBNull(reader.GetOrdinal("EstimatorName")), reader.GetString(reader.GetOrdinal("EstimatorName")), String.Empty)
                                                }
                                                                               estimators.Add(estimator)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading estimators")
            Return estimators
        End Function

        ' SaveEstimator (unchanged)
        Public Function SaveEstimator(estimator As EstimatorModel) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@EstimatorName", If(String.IsNullOrEmpty(estimator.EstimatorName), DBNull.Value, CType(estimator.EstimatorName, Object))}
            }

            Dim result As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If estimator.EstimatorID = 0 Then
                                                                           result = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertEstimator, BuildParameters(paramsDict)))
                                                                       Else
                                                                           paramsDict.Add("@EstimatorID", estimator.EstimatorID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateEstimator, BuildParameters(paramsDict))
                                                                           result = estimator.EstimatorID
                                                                       End If
                                                                   End Sub, "Error saving estimator")
            Return result
        End Function

        ' GetCustomers (unchanged)
        Public Function GetCustomers(Optional customerType As Integer? = Nothing) As List(Of CustomerModel)
            Dim customers As New List(Of CustomerModel)
            Dim params As SqlParameter() = If(customerType.HasValue, {New SqlParameter("@CustomerType", customerType.Value)}, {New SqlParameter("@CustomerType", DBNull.Value)})
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectCustomers, params)
                                                                           While reader.Read()
                                                                               Dim customer As New CustomerModel With {
                                                                           .CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                                                           .CustomerName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerName")), reader.GetString(reader.GetOrdinal("CustomerName")), String.Empty),
                                                                           .CustomerTypeID = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerType")), reader.GetInt32(reader.GetOrdinal("CustomerType")), Nothing),
                                                                           .CustomerType = New CustomerTypeModel With {
                                                                               .CustomerTypeID = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerType")), reader.GetInt32(reader.GetOrdinal("CustomerType")), 0),
                                                                               .CustomerTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerTypeName")), reader.GetString(reader.GetOrdinal("CustomerTypeName")), String.Empty)
                                                                           }
                                                                       }
                                                                               customers.Add(customer)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading customers")
            Return customers
        End Function

        ' SaveCustomer (unchanged)
        Public Function SaveCustomer(customer As CustomerModel, customerTypeID As Integer?) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
        {"@CustomerName", If(String.IsNullOrEmpty(customer.CustomerName), DBNull.Value, CType(customer.CustomerName, Object))},
        {"@CustomerType", If(customerTypeID.HasValue, CType(customerTypeID.Value, Object), DBNull.Value)}
    }

            Dim result As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If customer.CustomerID = 0 Then
                                                                           result = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertCustomer, BuildParameters(paramsDict)))
                                                                       Else
                                                                           paramsDict.Add("@CustomerID", customer.CustomerID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateCustomer, BuildParameters(paramsDict))
                                                                           result = customer.CustomerID
                                                                       End If
                                                                   End Sub, "Error saving customer")
            Return result
        End Function
        ' Get all customer types for dropdowns
        Public Function GetCustomerTypes() As List(Of CustomerTypeModel)
            Dim types As New List(Of CustomerTypeModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectCustomerTypes)
                                                                           While reader.Read()
                                                                               Dim type As New CustomerTypeModel With {
                                                                           .CustomerTypeID = reader.GetInt32(reader.GetOrdinal("CustomerTypeID")),
                                                                           .CustomerTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerTypeName")), reader.GetString(reader.GetOrdinal("CustomerTypeName")), String.Empty)
                                                                       }
                                                                               types.Add(type)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading customer types")
            Return types
        End Function

        ' GetSales (unchanged)
        Public Function GetSales() As List(Of SalesModel)
            Dim sales As New List(Of SalesModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectSales)
                                                                           While reader.Read()
                                                                               Dim sale As New SalesModel With {
                                                    .SalesID = reader.GetInt32(reader.GetOrdinal("SalesID")),
                                                    .SalesName = If(Not reader.IsDBNull(reader.GetOrdinal("SalesName")), reader.GetString(reader.GetOrdinal("SalesName")), String.Empty)
                                                }
                                                                               sales.Add(sale)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading sales")
            Return sales
        End Function

        ' SaveSales (unchanged)
        Public Function SaveSales(sale As SalesModel) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@SalesName", If(String.IsNullOrEmpty(sale.SalesName), DBNull.Value, CType(sale.SalesName, Object))}
            }

            Dim result As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If sale.SalesID = 0 Then
                                                                           result = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertSales, BuildParameters(paramsDict)))
                                                                       Else
                                                                           paramsDict.Add("@SalesID", sale.SalesID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateSales, BuildParameters(paramsDict))
                                                                           result = sale.SalesID
                                                                       End If
                                                                   End Sub, "Error saving sales")
            Return result
        End Function

        ' Updated: SaveActualUnit (use VersionID)
        Public Sub SaveActualUnit(actual As ActualUnitModel)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@VersionID", actual.VersionID},
                {"@RawUnitID", actual.RawUnitID},
                {"@ProductTypeID", actual.ProductTypeID},
                {"@UnitName", actual.UnitName},
                {"@PlanSQFT", actual.PlanSQFT},
                {"@UnitType", actual.UnitType},
                {"@OptionalAdder", actual.OptionalAdder}
            }

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If actual.ActualUnitID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertActualUnit, BuildParameters(paramsDict))
                                                                           actual.ActualUnitID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@ActualUnitID", actual.ActualUnitID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateActualUnit, BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving actual unit")
        End Sub

        ' Updated: SaveActualToLevelMapping (use VersionID)
        Public Sub SaveActualToLevelMapping(mapping As ActualToLevelMappingModel)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@VersionID", mapping.VersionID},
                {"@ActualUnitID", mapping.ActualUnitID},
                {"@LevelID", mapping.LevelID},
                {"@Quantity", mapping.Quantity}
            }

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If mapping.MappingID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertActualToLevelMapping, BuildParameters(paramsDict))
                                                                           If newIDObj IsNot Nothing Then mapping.MappingID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@MappingID", mapping.MappingID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateActualToLevelMapping, BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving actual to level mapping")
        End Sub

        ' GetActualToLevelMappingsByLevelID (unchanged query, uses LevelID)
        Public Function GetActualToLevelMappingsByLevelID(levelID As Integer) As List(Of ActualToLevelMappingModel)
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

        ' GetActualToLevelMappingsByActualUnitID (unchanged query, uses ActualUnitID)
        Public Function GetActualToLevelMappingsByActualUnitID(actualUnitID As Integer) As List(Of ActualToLevelMappingModel)
            Dim mappings As New List(Of ActualToLevelMappingModel)
            Dim params As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectActualToLevelMappingsByActualUnitID, params)
                                                                           While reader.Read()
                                                                               Dim mapping As New ActualToLevelMappingModel With {
                                                    .MappingID = reader.GetInt32(reader.GetOrdinal("MappingID")),
                                                    .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID")),
                                                    .ActualUnitID = actualUnitID,
                                                    .LevelID = reader.GetInt32(reader.GetOrdinal("LevelID")),
                                                    .Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                                }
                                                                               mappings.Add(mapping)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading mappings for actual unit " & actualUnitID)
            Return mappings
        End Function

        ' Updated: GetActualUnitsByVersion (use VersionID)
        Public Function GetActualUnitsByVersion(versionID As Integer) As List(Of ActualUnitModel)
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
                                                    .VersionID = versionID,
                                                    .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID"))
                                                }
                                                                               actual.CalculatedComponents = GetCalculatedComponentsByActualUnitID(actual.ActualUnitID)
                                                                               actualUnits.Add(actual)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading actual units for version " & versionID)
            Return actualUnits
        End Function

        ' GetActualUnitByID (unchanged)
        Public Function GetActualUnitByID(actualUnitID As Integer) As ActualUnitModel
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
                                                    .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                                    .ReferencedRawUnitName = If(Not reader.IsDBNull(reader.GetOrdinal("ReferencedRawUnitName")), reader.GetString(reader.GetOrdinal("ReferencedRawUnitName")), String.Empty),
                                                    .CalculatedComponents = GetCalculatedComponentsByActualUnitID(actualUnitID)
                                                }
                                                                           End If
                                                                       End Using
                                                                   End Sub, "Error loading actual unit " & actualUnitID)
            Return unit
        End Function

        ' GetCalculatedComponentsByActualUnitID (unchanged)
        Private Function GetCalculatedComponentsByActualUnitID(actualUnitID As Integer) As List(Of CalculatedComponentModel)
            Dim components As New List(Of CalculatedComponentModel)
            Dim params As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectCalculatedComponentsByActualUnitID, params)
                                                                           While reader.Read()
                                                                               Dim comp As New CalculatedComponentModel With {
                                                    .ComponentID = reader.GetInt32(reader.GetOrdinal("ComponentID")),
                                                    .ComponentType = reader.GetString(reader.GetOrdinal("ComponentType")),
                                                    .Value = reader.GetDecimal(reader.GetOrdinal("Value"))
                                                }
                                                                               components.Add(comp)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading calculated components for actual unit " & actualUnitID)
            Return components
        End Function

        ' Updated: SaveCalculatedComponents (use VersionID)
        Public Sub SaveCalculatedComponents(actualUnit As ActualUnitModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteCalculatedComponentsByActualUnitID, {New SqlParameter("@ActualUnitID", actualUnit.ActualUnitID)})

                                                                       For Each comp In actualUnit.CalculatedComponents
                                                                           Dim insertParams As New Dictionary(Of String, Object) From {
                                                {"@VersionID", actualUnit.VersionID},
                                                {"@ActualUnitID", actualUnit.ActualUnitID},
                                                {"@ComponentType", comp.ComponentType},
                                                {"@Value", comp.Value}
                                            }
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.InsertCalculatedComponent, BuildParameters(insertParams))
                                                                       Next
                                                                   End Sub, "Error saving calculated components")
        End Sub

        ' Updated: GetRawUnitsByVersionID (use VersionID)
        Public Function GetRawUnitsByVersionID(versionID As Integer) As List(Of RawUnitModel)
            Dim rawUnits As New List(Of RawUnitModel)
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID)}
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectRawUnitsByVersion, params)
                                                                           While reader.Read()
                                                                               Dim rawUnit As New RawUnitModel With {
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
                                                    .AvgSPFNo2 = If(reader.IsDBNull(reader.GetOrdinal("AvgSPFNo2")), Nothing, reader.GetDecimal(reader.GetOrdinal("AvgSPFNo2")))
                                                }
                                                                               rawUnits.Add(rawUnit)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading raw units for version " & versionID)
            Return rawUnits
        End Function



        ' Updated: UpdateBuildingRollups (use VersionID)
        Public Sub UpdateBuildingRollups(buildingID As Integer)
            Dim bldgQty As Integer = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectBldgQty, {New SqlParameter("@BuildingID", buildingID)}))
            Dim versionID As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim levelInfoParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader("SELECT DISTINCT VersionID FROM Levels WHERE BuildingID = @BuildingID", levelInfoParams)
                                                                           If reader.Read() Then versionID = reader.GetInt32(0)
                                                                       End Using

                                                                       Dim floorPricePerBldgParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                                                                       Dim floorPricePerBldgObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateFloorPricePerBldg, floorPricePerBldgParams)
                                                                       Dim floorPricePerBldg As Decimal = If(floorPricePerBldgObj Is DBNull.Value OrElse floorPricePerBldgObj Is Nothing, 0D, CDec(floorPricePerBldgObj))

                                                                       Dim roofPricePerBldgParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                                                                       Dim roofPricePerBldgObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateRoofPricePerBldg, roofPricePerBldgParams)
                                                                       Dim roofPricePerBldg As Decimal = If(roofPricePerBldgObj Is DBNull.Value OrElse roofPricePerBldgObj Is Nothing, 0D, CDec(roofPricePerBldgObj))

                                                                       Dim wallPricePerBldg As Decimal = 0D ' Stub for future

                                                                       Dim floorBaseCostParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                                                                       Dim floorBaseCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateFloorBaseCost, floorBaseCostParams)
                                                                       Dim floorBaseCost As Decimal = If(floorBaseCostObj Is DBNull.Value OrElse floorBaseCostObj Is Nothing, 0D, CDec(floorBaseCostObj))

                                                                       Dim roofBaseCostParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                                                                       Dim roofBaseCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateRoofBaseCost, roofBaseCostParams)
                                                                       Dim roofBaseCost As Decimal = If(roofBaseCostObj Is DBNull.Value OrElse roofBaseCostObj Is Nothing, 0D, CDec(roofBaseCostObj))

                                                                       Dim wallBaseCost As Decimal = 0D ' Stub

                                                                       Dim extendedFloorCost As Decimal = floorPricePerBldg * bldgQty
                                                                       Dim extendedRoofCost As Decimal = roofPricePerBldg * bldgQty
                                                                       Dim extendedWallCost As Decimal = wallPricePerBldg * bldgQty

                                                                       Dim overallCost As Decimal = floorBaseCost + roofBaseCost + wallBaseCost
                                                                       Dim overallPrice As Decimal = floorPricePerBldg + roofPricePerBldg + wallPricePerBldg

                                                                       Dim updateParams As New Dictionary(Of String, Object) From {
                                            {"@BuildingID", buildingID},
                                            {"@FloorCostPerBldg", floorPricePerBldg},
                                            {"@RoofCostPerBldg", roofPricePerBldg},
                                            {"@WallCostPerBldg", wallPricePerBldg},
                                            {"@ExtendedFloorCost", extendedFloorCost},
                                            {"@ExtendedRoofCost", extendedRoofCost},
                                            {"@ExtendedWallCost", extendedWallCost},
                                            {"@OverallPrice", overallPrice},
                                            {"@OverallCost", overallCost}
                                        }
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateBuildingRollupsSql, BuildParameters(updateParams))
                                                                   End Sub, "Error updating building rollups for ID " & buildingID)
        End Sub

        ' DeleteLevel (unchanged, uses transactions)
        Public Sub DeleteLevel(levelID As Integer)
            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                               Dim actualUnitIDs As New List(Of Integer)
                                                                               Using cmd As New SqlCommand(Queries.SelectActualUnitIDsByLevelID, conn, transaction)
                                                                                   cmd.Parameters.AddWithValue("@LevelID", levelID)
                                                                                   Using reader As SqlDataReader = cmd.ExecuteReader()
                                                                                       While reader.Read()
                                                                                           actualUnitIDs.Add(reader.GetInt32(0))
                                                                                       End While
                                                                                   End Using
                                                                               End Using

                                                                               Using cmd As New SqlCommand(Queries.DeleteActualToLevelMappingByLevelID, conn, transaction)
                                                                                   cmd.Parameters.AddWithValue("@LevelID", levelID)
                                                                                   cmd.ExecuteNonQuery()
                                                                               End Using

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

                                                                               Using cmd As New SqlCommand(Queries.DeleteLevel, conn, transaction)
                                                                                   cmd.Parameters.AddWithValue("@LevelID", levelID)
                                                                                   cmd.ExecuteNonQuery()
                                                                               End Using

                                                                               transaction.Commit()
                                                                           End Sub, "Error deleting level " & levelID)
                End Using
            End Using
        End Sub

        ' DeleteBuilding (unchanged, cascades to levels)
        Public Sub DeleteBuilding(buildingID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim levels As List(Of LevelModel) = GetLevelsByBuildingID(buildingID)
                                                                       For Each level In levels
                                                                           DeleteLevel(level.LevelID)
                                                                       Next
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteBuilding, {New SqlParameter("@BuildingID", buildingID)})
                                                                   End Sub, "Error deleting building " & buildingID)
        End Sub

        ' DeleteActualToLevelMapping (unchanged, uses MappingID)
        Public Sub DeleteActualToLevelMapping(mappingID As Integer)
            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                               SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteActualToLevelMappingByMappingID, {New SqlParameter("@MappingID", mappingID)}, conn, transaction)
                                                                               transaction.Commit()
                                                                           End Sub, "Error deleting mapping")
                End Using
            End Using
        End Sub

        ' DeleteActualUnit (unchanged, checks mappings)
        Public Sub DeleteActualUnit(actualUnitID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim mappingParams As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}
                                                                       Dim mappingCount As Integer = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)("SELECT COUNT(*) FROM ActualToLevelMapping WHERE ActualUnitID = @ActualUnitID", mappingParams))
                                                                       If mappingCount > 0 Then
                                                                           Throw New ApplicationException($"Cannot delete ActualUnit with {mappingCount} existing level mapping(s).")
                                                                       End If

                                                                       Dim componentParams As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteCalculatedComponentsByActualUnitID, componentParams)

                                                                       Dim unitParams As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteActualUnit, unitParams)
                                                                   End Sub, "Error deleting actual unit " & actualUnitID)
        End Sub

        ' GetConfigValue (unchanged)
        Public Function GetConfigValue(configKey As String) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@ConfigKey", configKey)}
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectConfigValue, params)
                                                                   End Sub, "Error fetching config value for " & configKey)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        ' GetMilesToJobSite (unchanged, uses ProjectID)
        Public Function GetMilesToJobSite(projectID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectMilesToJobSite, params)
                                                                   End Sub, "Error fetching miles for project " & projectID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0, CInt(valObj))
        End Function

        ' Updated: GetLumberAdder (use VersionID)
        Public Function GetLumberAdder(versionID As Integer, productTypeID As Integer) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID), New SqlParameter("@ProductTypeID", productTypeID)}
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectLumberAdder, params)
                                                                   End Sub, "Error fetching lumber adder for version " & versionID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        ' Updated: GetMarginPercent (use VersionID)
        Public Function GetMarginPercent(versionID As Integer, productTypeID As Integer) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID), New SqlParameter("@ProductTypeID", productTypeID)}
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectMarginPercent, params)
                                                                   End Sub, "Error fetching margin percent for version " & versionID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        ' GetRawUnitByID (unchanged)
        ' GetRawUnitByID (updated to retrieve all fields)
        Public Function GetRawUnitByID(rawUnitID As Integer) As RawUnitModel
            Dim raw As RawUnitModel = Nothing
            Dim params As SqlParameter() = {New SqlParameter("@RawUnitID", rawUnitID)}

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectRawUnitByID, params)
                                                                           If reader.Read() Then
                                                                               raw = New RawUnitModel With {
                                                                                        .RawUnitID = rawUnitID,
                                                                                        .RawUnitName = If(Not reader.IsDBNull(reader.GetOrdinal("RawUnitName")), reader.GetString(reader.GetOrdinal("RawUnitName")), String.Empty),
                                                                                        .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                                                                        .BF = If(Not reader.IsDBNull(reader.GetOrdinal("BF")), reader.GetDecimal(reader.GetOrdinal("BF")), Nothing),
                                                                                        .LF = If(Not reader.IsDBNull(reader.GetOrdinal("LF")), reader.GetDecimal(reader.GetOrdinal("LF")), Nothing),
                                                                                        .EWPLF = If(Not reader.IsDBNull(reader.GetOrdinal("EWPLF")), reader.GetDecimal(reader.GetOrdinal("EWPLF")), Nothing),
                                                                                        .SqFt = If(Not reader.IsDBNull(reader.GetOrdinal("SqFt")), reader.GetDecimal(reader.GetOrdinal("SqFt")), Nothing),
                                                                                        .FCArea = If(Not reader.IsDBNull(reader.GetOrdinal("FCArea")), reader.GetDecimal(reader.GetOrdinal("FCArea")), Nothing),
                                                                                        .LumberCost = If(Not reader.IsDBNull(reader.GetOrdinal("LumberCost")), reader.GetDecimal(reader.GetOrdinal("LumberCost")), Nothing),
                                                                                        .PlateCost = If(Not reader.IsDBNull(reader.GetOrdinal("PlateCost")), reader.GetDecimal(reader.GetOrdinal("PlateCost")), Nothing),
                                                                                        .ManufLaborCost = If(Not reader.IsDBNull(reader.GetOrdinal("ManufLaborCost")), reader.GetDecimal(reader.GetOrdinal("ManufLaborCost")), Nothing),
                                                                                        .DesignLabor = If(Not reader.IsDBNull(reader.GetOrdinal("DesignLabor")), reader.GetDecimal(reader.GetOrdinal("DesignLabor")), Nothing),
                                                                                        .MGMTLabor = If(Not reader.IsDBNull(reader.GetOrdinal("MGMTLabor")), reader.GetDecimal(reader.GetOrdinal("MGMTLabor")), Nothing),
                                                                                        .JobSuppliesCost = If(Not reader.IsDBNull(reader.GetOrdinal("JobSuppliesCost")), reader.GetDecimal(reader.GetOrdinal("JobSuppliesCost")), Nothing),
                                                                                        .ManHours = If(Not reader.IsDBNull(reader.GetOrdinal("ManHours")), reader.GetDecimal(reader.GetOrdinal("ManHours")), Nothing),
                                                                                        .ItemCost = If(Not reader.IsDBNull(reader.GetOrdinal("ItemCost")), reader.GetDecimal(reader.GetOrdinal("ItemCost")), Nothing),
                                                                                        .OverallCost = If(Not reader.IsDBNull(reader.GetOrdinal("OverallCost")), reader.GetDecimal(reader.GetOrdinal("OverallCost")), Nothing),
                                                                                        .DeliveryCost = If(Not reader.IsDBNull(reader.GetOrdinal("DeliveryCost")), reader.GetDecimal(reader.GetOrdinal("DeliveryCost")), Nothing),
                                                                                        .TotalSellPrice = If(Not reader.IsDBNull(reader.GetOrdinal("TotalSellPrice")), reader.GetDecimal(reader.GetOrdinal("TotalSellPrice")), Nothing),
                                                                                        .AvgSPFNo2 = If(Not reader.IsDBNull(reader.GetOrdinal("AvgSPFNo2")), reader.GetDecimal(reader.GetOrdinal("AvgSPFNo2")), Nothing),
                                                                                        .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID"))
                                                                                    }
                                                                           End If
                                                                       End Using
                                                                   End Sub, "Error loading raw unit " & rawUnitID)

            Return raw
        End Function

        ' SetLevelRollups (unchanged, uses LevelID)
        Public Sub SetLevelRollups(levelID As Integer, overallSqft As Decimal, overallLf As Decimal, overallBdft As Decimal, lumberCost As Decimal, plateCost As Decimal, laborCost As Decimal, laborMh As Decimal, designCost As Decimal, mgmtCost As Decimal, jobSuppliesCost As Decimal, itemsCost As Decimal, deliveryCost As Decimal, overallCost As Decimal, overallPrice As Decimal, totalSqft As Decimal, avgPricePerSqft As Decimal)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@LevelID", levelID},
                {"@OverallSQFT", overallSqft},
                {"@OverallLF", overallLf},
                {"@OverallBDFT", overallBdft},
                {"@LumberCost", lumberCost},
                {"@PlateCost", plateCost},
                {"@LaborCost", laborCost},
                {"@LaborMH", laborMh},
                {"@DesignCost", designCost},
                {"@MGMTCost", mgmtCost},
                {"@JobSuppliesCost", jobSuppliesCost},
                {"@ItemsCost", itemsCost},
                {"@DeliveryCost", deliveryCost},
                {"@OverallCost", overallCost},
                {"@OverallPrice", overallPrice},
                {"@TotalSQFT", totalSqft},
                {"@AvgPricePerSQFT", avgPricePerSqft}
            }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLevelRollupsSql, BuildParameters(paramsDict))
                                                                   End Sub, "Error updating level rollups")
        End Sub

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
                                                                                   .WallHeights = If(Not reader.IsDBNull(reader.GetOrdinal("WallHeights")), reader.GetString(reader.GetOrdinal("WallHeights")), String.Empty)
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
                {"@WallHeights", If(String.IsNullOrEmpty(info.WallHeights), DBNull.Value, CObj(info.WallHeights))}
            }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If info.InfoID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectDesignInfo, BuildParameters(paramsDict))
                                                                           info.InfoID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@InfoID", info.InfoID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectDesignInfo, BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving ProjectDesignInfo for ProjectID " & info.ProjectID)
        End Sub

        Public Function GetProjectLoads(projectID As Integer) As List(Of ProjectLoadModel)
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
                                                                                   .Absolute = If(Not reader.IsDBNull(reader.GetOrdinal("Absolute")), reader.GetString(reader.GetOrdinal("Absolute")), String.Empty),
                                                                                   .Deflection = If(Not reader.IsDBNull(reader.GetOrdinal("Deflection")), reader.GetString(reader.GetOrdinal("Deflection")), String.Empty)
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
                                                                                       {"@Absolute", If(String.IsNullOrEmpty(load.Absolute), DBNull.Value, CObj(load.Absolute))},
                                                                                       {"@Deflection", If(String.IsNullOrEmpty(load.Deflection), DBNull.Value, CObj(load.Deflection))}
                                                                                   }
                                                                                   Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(Queries.InsertProjectLoad, BuildParameters(paramsDict), conn, transaction)
                                                                                   load.LoadID = CInt(newIDObj)
                                                                               Next
                                                                               transaction.Commit()
                                                                           End Sub, "Error saving ProjectLoads for ProjectID " & projectID)
                End Using
            End Using
        End Sub

        Public Function GetProjectBearingStyles(projectID As Integer) As ProjectBearingStylesModel
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
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectBearingStyles, BuildParameters(paramsDict))
                                                                           bearing.BearingID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@BearingID", bearing.BearingID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectBearingStyles, BuildParameters(paramsDict))
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
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectGeneralNotes, BuildParameters(paramsDict))
                                                                           note.NoteID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@NoteID", note.NoteID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectGeneralNotes, BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving ProjectGeneralNotes for ProjectID " & note.ProjectID)
        End Sub

        Public Function GetProjectItems(projectID As Integer) As List(Of ProjectItemModel)
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
                                                                                   Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(Queries.InsertProjectItem, BuildParameters(paramsDict), conn, transaction)
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

        ' In DataAccess.vb: Add these methods

        Public Function GetProjectIDByVersionID(versionID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID)}
            Dim idObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)("SELECT ProjectID FROM ProjectVersions WHERE VersionID = @VersionID", params)
            Return If(idObj Is DBNull.Value OrElse idObj Is Nothing, 0, CInt(idObj))
        End Function

        Private Function GetLevelInfo(levelID As Integer) As Tuple(Of Integer, Integer, Decimal)
            Dim params As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
            Dim versionID As Integer = 0
            Dim productTypeID As Integer = 0
            Dim commonSQFT As Decimal = 0D
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader("SELECT VersionID, ProductTypeID, ISNULL(CommonSQFT, 0) FROM Levels WHERE LevelID = @LevelID", params)
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

        Public Function GetEffectiveMargin(versionID As Integer, productTypeID As Integer, rawUnitID As Integer) As Decimal
            Dim margin As Decimal = GetMarginPercent(versionID, productTypeID)
            If margin <= 0D Then
                Dim rawUnit As RawUnitModel = GetRawUnitByID(rawUnitID)
                If rawUnit IsNot Nothing AndAlso rawUnit.TotalSellPrice.HasValue AndAlso rawUnit.TotalSellPrice > 0D AndAlso rawUnit.OverallCost.HasValue AndAlso rawUnit.OverallCost < rawUnit.TotalSellPrice.Value Then
                    margin = (rawUnit.TotalSellPrice.Value - rawUnit.OverallCost.Value) / rawUnit.TotalSellPrice.Value
                End If
            End If
            If margin <= 0D Then
                margin = GetConfigValue("DefaultMarginPercent")
            End If
            Return margin
        End Function

        Public Function ComputeLevelUnits(levelID As Integer) As List(Of DisplayUnitData)
            Dim units As New List(Of DisplayUnitData)
            Dim mappings As List(Of ActualToLevelMappingModel) = GetActualToLevelMappingsByLevelID(levelID)
            If Not mappings.Any() Then Return units

            Dim levelInfo As Tuple(Of Integer, Integer, Decimal) = GetLevelInfo(levelID)
            Dim versionID As Integer = levelInfo.Item1
            Dim productTypeID As Integer = levelInfo.Item2
            ' commonSQFT = levelInfo.Item3 (not used here)

            Dim projectID As Integer = GetProjectIDByVersionID(versionID)
            Dim miles As Integer = GetMilesToJobSite(projectID)
            Dim mileageRate As Decimal = GetConfigValue("MileageRate")

            Dim totalBDFT As Decimal = 0D
            Dim unitBDFTs As New List(Of Decimal)

            For Each mapping As ActualToLevelMappingModel In mappings
                Dim actual As ActualUnitModel = mapping.ActualUnit
                Dim raw As RawUnitModel = GetRawUnitByID(actual.RawUnitID)
                If raw Is Nothing OrElse raw.SqFt.GetValueOrDefault() <= 0D Then Continue For

                Dim display As New DisplayUnitData With {
                    .ActualUnit = actual,
                    .MappingID = mapping.MappingID,
                    .ReferencedRawUnitName = raw.RawUnitName,
                    .ActualUnitQuantity = mapping.Quantity
                }

                Dim planSqft As Decimal = actual.PlanSQFT
                Dim opt As Decimal = actual.OptionalAdder
                Dim qty As Integer = mapping.Quantity
                Dim extSqft As Decimal = planSqft * qty * opt

                Dim lfPer As Decimal = raw.LF.GetValueOrDefault() / raw.SqFt.Value
                display.LF = lfPer * extSqft

                Dim bdftPer As Decimal = raw.BF.GetValueOrDefault() / raw.SqFt.Value
                display.BDFT = bdftPer * extSqft

                Dim lumberPer As Decimal = raw.LumberCost.GetValueOrDefault() / raw.SqFt.Value
                display.LumberCost = lumberPer * extSqft

                Dim effective As Decimal = GetLumberAdder(versionID, productTypeID)
                If effective > 0D Then
                    display.LumberCost += (display.BDFT / 1000D) * effective
                End If

                display.PlateCost = (raw.PlateCost.GetValueOrDefault() / raw.SqFt.Value) * extSqft
                display.ManufLaborCost = (raw.ManufLaborCost.GetValueOrDefault() / raw.SqFt.Value) * extSqft
                display.DesignLabor = (raw.DesignLabor.GetValueOrDefault() / raw.SqFt.Value) * extSqft
                display.MGMTLabor = (raw.MGMTLabor.GetValueOrDefault() / raw.SqFt.Value) * extSqft
                display.JobSuppliesCost = (raw.JobSuppliesCost.GetValueOrDefault() / raw.SqFt.Value) * extSqft
                display.ManHours = (raw.ManHours.GetValueOrDefault() / raw.SqFt.Value) * extSqft
                display.ItemCost = (raw.ItemCost.GetValueOrDefault() / raw.SqFt.Value) * extSqft

                display.OverallCost = display.LumberCost + display.PlateCost + display.ManufLaborCost + display.DesignLabor + display.MGMTLabor + display.JobSuppliesCost + display.ItemCost

                Dim margin As Decimal = GetEffectiveMargin(versionID, productTypeID, raw.RawUnitID)
                display.SellPrice = If(margin >= 1D, display.OverallCost, display.OverallCost / (1D - margin))
                display.Margin = display.SellPrice - display.OverallCost

                unitBDFTs.Add(display.BDFT)
                totalBDFT += display.BDFT

                units.Add(display)
            Next

            Dim deliveryTotal As Decimal = 0D
            If totalBDFT > 0D Then
                Dim numLoads As Decimal = Math.Ceiling(totalBDFT / 10000D)
                deliveryTotal = numLoads * mileageRate * miles
                If deliveryTotal < 150D Then deliveryTotal = 150D
            End If

            If totalBDFT > 0D Then
                For i As Integer = 0 To units.Count - 1
                    units(i).DeliveryCost = (unitBDFTs(i) / totalBDFT) * deliveryTotal
                    Dim margin As Decimal = GetEffectiveMargin(versionID, units(i).ActualUnit.ProductTypeID, units(i).ActualUnit.RawUnitID)
                    units(i).SellPrice = If(margin >= 1D, units(i).OverallCost + units(i).DeliveryCost, units(i).OverallCost / (1D - margin) + units(i).DeliveryCost)
                    units(i).Margin = units(i).SellPrice - units(i).OverallCost - units(i).DeliveryCost
                Next
            End If

            Return units
        End Function

        Public Sub RecalculateVersion(versionID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim buildings As List(Of BuildingModel) = GetBuildingsByVersionID(versionID)
                                                                       For Each bldg As BuildingModel In buildings
                                                                           Dim levels As List(Of LevelModel) = GetLevelsByBuildingID(bldg.BuildingID)
                                                                           For Each lvl As LevelModel In levels
                                                                               UpdateLevelRollups(lvl.LevelID)
                                                                           Next
                                                                           UpdateBuildingRollups(bldg.BuildingID)
                                                                       Next
                                                                   End Sub, "Error recalculating version " & versionID)
        End Sub

        ' Replace existing UpdateLevelRollups with this
        Public Sub UpdateLevelRollups(levelID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim units As List(Of DisplayUnitData) = ComputeLevelUnits(levelID)
                                                                       Dim overallSQFT As Decimal = units.Sum(Function(u) u.PlanSQFT * u.ActualUnitQuantity * u.OptionalAdder)
                                                                       Dim overallLF As Decimal = units.Sum(Function(u) u.LF)
                                                                       Dim overallBDFT As Decimal = units.Sum(Function(u) u.BDFT)
                                                                       Dim lumberCost As Decimal = units.Sum(Function(u) u.LumberCost)
                                                                       Dim plateCost As Decimal = units.Sum(Function(u) u.PlateCost)
                                                                       Dim laborCost As Decimal = units.Sum(Function(u) u.ManufLaborCost)
                                                                       Dim laborMH As Decimal = units.Sum(Function(u) u.ManHours)
                                                                       Dim designCost As Decimal = units.Sum(Function(u) u.DesignLabor)
                                                                       Dim mgmtCost As Decimal = units.Sum(Function(u) u.MGMTLabor)
                                                                       Dim jobSuppliesCost As Decimal = units.Sum(Function(u) u.JobSuppliesCost)
                                                                       Dim itemsCost As Decimal = units.Sum(Function(u) u.ItemCost)
                                                                       Dim deliveryCost As Decimal = units.Sum(Function(u) u.DeliveryCost)
                                                                       Dim overallCost As Decimal = units.Sum(Function(u) u.OverallCost)
                                                                       Dim overallPrice As Decimal = units.Sum(Function(u) u.SellPrice)

                                                                       Dim levelInfo As Tuple(Of Integer, Integer, Decimal) = GetLevelInfo(levelID)
                                                                       Dim commonSQFT As Decimal = levelInfo.Item3
                                                                       Dim totalSQFT As Decimal = overallSQFT + commonSQFT
                                                                       Dim avgPricePerSqft As Decimal = If(totalSQFT > 0D, overallPrice / totalSQFT, 0D)

                                                                       Dim paramsDict As New Dictionary(Of String, Object) From {
                                                                           {"@LevelID", levelID},
                                                                           {"@OverallSQFT", overallSQFT},
                                                                           {"@OverallLF", overallLF},
                                                                           {"@OverallBDFT", overallBDFT},
                                                                           {"@LumberCost", lumberCost},
                                                                           {"@PlateCost", plateCost},
                                                                           {"@LaborCost", laborCost},
                                                                           {"@LaborMH", laborMH},
                                                                           {"@DesignCost", designCost},
                                                                           {"@MGMTCost", mgmtCost},
                                                                           {"@JobSuppliesCost", jobSuppliesCost},
                                                                           {"@ItemsCost", itemsCost},
                                                                           {"@DeliveryCost", deliveryCost},
                                                                           {"@OverallCost", overallCost},
                                                                           {"@OverallPrice", overallPrice},
                                                                           {"@TotalSQFT", totalSQFT},
                                                                           {"@AvgPricePerSQFT", avgPricePerSqft}
                                                                       }
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLevelRollupsSql, BuildParameters(paramsDict))
                                                                   End Sub, "Error updating level rollups for " & levelID)
        End Sub
        ' In DataAccess.vb: Add method to get average AvgSPFNo2 for floors and roofs
        Public Function GetAverageSPFNo2ByProductType(versionID As Integer, productTypeName As String) As Decimal
            Dim avgSPFNo2 As Decimal = 0D
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim params As New Dictionary(Of String, Object) From {
                                                                           {"@VersionID", versionID},
                                                                           {"@ProductTypeName", productTypeName}
                                                                       }
                                                                       Dim query As String = "SELECT AVG(ru.AvgSPFNo2) FROM RawUnits ru INNER JOIN ProductType pt ON ru.ProductTypeID = pt.ProductTypeID WHERE ru.VersionID = @VersionID AND pt.ProductTypeName = @ProductTypeName AND ru.AvgSPFNo2 IS NOT NULL"
                                                                       Dim result As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(query, BuildParameters(params))
                                                                       If result IsNot DBNull.Value Then
                                                                           avgSPFNo2 = CDec(result)
                                                                       End If
                                                                   End Sub, $"Error calculating average SPFNo2 for {productTypeName} in version {versionID}")
            Return avgSPFNo2
        End Function
        ' Delete an entire project and all related data, respecting FK constraints
        ' Delete an entire project and all related data, respecting FK constraints
        Public Sub DeleteProject(projectID As Integer, ByRef notificationMessage As String)
            Dim localNotification As String = String.Empty
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using transaction As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   ' Fetch all VersionIDs for the project
                                                                                   Dim versionIDs As New List(Of Integer)
                                                                                   Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReaderTransactional(Queries.SelectProjectVersions, New SqlParameter() {New SqlParameter("@ProjectID", projectID)}, conn, transaction)
                                                                                       While reader.Read()
                                                                                           versionIDs.Add(reader.GetInt32(reader.GetOrdinal("VersionID")))
                                                                                       End While
                                                                                   End Using

                                                                                   ' Delete version-related data in FK-safe order
                                                                                   For Each vid As Integer In versionIDs
                                                                                       SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteActualToLevelMappingsByVersion, New SqlParameter() {New SqlParameter("@VersionID", vid)}, conn, transaction)
                                                                                       SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteCalculatedComponentsByVersion, New SqlParameter() {New SqlParameter("@VersionID", vid)}, conn, transaction)
                                                                                       SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteActualUnitsByVersion, New SqlParameter() {New SqlParameter("@VersionID", vid)}, conn, transaction)
                                                                                       SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteRawUnitsByVersion, New SqlParameter() {New SqlParameter("@VersionID", vid)}, conn, transaction)
                                                                                       SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteLevelsByVersion, New SqlParameter() {New SqlParameter("@VersionID", vid)}, conn, transaction)
                                                                                       SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteBuildingsByVersion, New SqlParameter() {New SqlParameter("@VersionID", vid)}, conn, transaction)
                                                                                       SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteProjectProductSettingsByVersion, New SqlParameter() {New SqlParameter("@VersionID", vid)}, conn, transaction)
                                                                                   Next

                                                                                   ' Delete project versions
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteProjectVersionsByProject, New SqlParameter() {New SqlParameter("@ProjectID", projectID)}, conn, transaction)

                                                                                   ' Delete direct project-related data
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteProjectDesignInfoByProject, New SqlParameter() {New SqlParameter("@ProjectID", projectID)}, conn, transaction)
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteProjectLoadsByProject, New SqlParameter() {New SqlParameter("@ProjectID", projectID)}, conn, transaction)
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteProjectBearingStylesByProject, New SqlParameter() {New SqlParameter("@ProjectID", projectID)}, conn, transaction)
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteProjectGeneralNotesByProject, New SqlParameter() {New SqlParameter("@ProjectID", projectID)}, conn, transaction)
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteProjectItemsByProject, New SqlParameter() {New SqlParameter("@ProjectID", projectID)}, conn, transaction)

                                                                                   ' Finally, delete the project
                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteProjectByID, New SqlParameter() {New SqlParameter("@ProjectID", projectID)}, conn, transaction)

                                                                                   transaction.Commit()
                                                                                   localNotification = $"Project {projectID} was successfully deleted."
                                                                               Catch ex As Exception
                                                                                   transaction.Rollback()
                                                                                   localNotification = String.Empty ' Clear message on failure
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error deleting project " & projectID)
            notificationMessage = localNotification
        End Sub

    End Class
End Namespace