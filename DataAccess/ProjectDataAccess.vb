Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.Utilities




Namespace DataAccess
    Public Class ProjectDataAccess

        'Refactored: SaveProject
        Public Sub SaveProject(proj As ProjectModel)
            ' Validate CustomerType for ArchitectID (2) and EngineerID (3)
            If proj.ArchitectID.HasValue AndAlso Not HelperDataAccess.ValidateCustomerType(proj.ArchitectID.Value, 2) Then
                Throw New ArgumentException("ArchitectID must reference a customer with CustomerType=2 (Architect).")
            End If
            If proj.EngineerID.HasValue AndAlso Not HelperDataAccess.ValidateCustomerType(proj.EngineerID.Value, 3) Then
                Throw New ArgumentException("EngineerID must reference a customer with CustomerType=3 (Engineer).")
            End If

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim paramsDict As Dictionary(Of String, Object) = ModelParams.ForProject(proj)

                                                                       If proj.ProjectID = 0 Then
                                                                           Dim newID As Integer = SqlConnectionManager.Instance.ExecuteScalar(Of Integer)(
                                                                            Queries.InsertProject,
                                                                            HelperDataAccess.BuildParameters(paramsDict))
                                                                           proj.ProjectID = newID
                                                                       Else
                                                                           paramsDict.Add("@ProjectID", proj.ProjectID)   ' safe – key does not exist yet
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(
                                                                            Queries.UpdateProject,
                                                                            HelperDataAccess.BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving project " & If(proj.ProjectID = 0, "(new)", proj.ProjectID.ToString()))
        End Sub

        ' === PRIVATE SHARED MAPPER (single source of truth) ===
        Private Function MapProjectFromReader(reader As SqlDataReader) As ProjectModel
            ' Cache ordinals once – fast and Option Strict safe
            Dim ordProjectID As Integer = reader.GetOrdinal("ProjectID")
            Dim ordJBID As Integer = reader.GetOrdinal("JBID")
            Dim ordProjectTypeID As Integer = reader.GetOrdinal("ProjectTypeID")
            Dim ordProjectType As Integer = reader.GetOrdinal("ProjectType")
            Dim ordProjectName As Integer = reader.GetOrdinal("ProjectName")
            Dim ordEstimatorID As Integer = reader.GetOrdinal("EstimatorID")
            Dim ordEstimator As Integer = reader.GetOrdinal("Estimator")
            Dim ordAddress As Integer = reader.GetOrdinal("Address")
            Dim ordCity As Integer = reader.GetOrdinal("City")
            Dim ordState As Integer = reader.GetOrdinal("State")
            Dim ordZip As Integer = reader.GetOrdinal("Zip")
            Dim ordBidDate As Integer = reader.GetOrdinal("BidDate")
            Dim ordArchPlansDated As Integer = reader.GetOrdinal("ArchPlansDated")
            Dim ordEngPlansDated As Integer = reader.GetOrdinal("EngPlansDated")
            Dim ordMilesToJobSite As Integer = reader.GetOrdinal("MilesToJobSite")
            Dim ordTotalNetSqft As Integer = reader.GetOrdinal("TotalNetSqft")
            Dim ordTotalGrossSqft As Integer = reader.GetOrdinal("TotalGrossSqft")
            Dim ordArchitectID As Integer = reader.GetOrdinal("ArchitectID")
            Dim ordEngineerID As Integer = reader.GetOrdinal("EngineerID")
            Dim ordProjectNotes As Integer = reader.GetOrdinal("ProjectNotes")
            Dim ordLastModifiedDate As Integer = reader.GetOrdinal("LastModifiedDate")
            Dim ordCreatedDate As Integer = reader.GetOrdinal("createddate")
            Dim ordArchitectName As Integer = reader.GetOrdinal("ArchitectName")
            Dim ordEngineerName As Integer = reader.GetOrdinal("EngineerName")

            Return New ProjectModel With {
        .ProjectID = reader.GetInt32(ordProjectID),
        .JBID = If(reader.IsDBNull(ordJBID), String.Empty, reader.GetString(ordJBID)),
        .ProjectType = New ProjectTypeModel With {
            .ProjectTypeID = reader.GetInt32(ordProjectTypeID),
            .ProjectTypeName = If(reader.IsDBNull(ordProjectType), String.Empty, reader.GetString(ordProjectType))
        },
        .ProjectName = If(reader.IsDBNull(ordProjectName), String.Empty, reader.GetString(ordProjectName)),
        .Estimator = New EstimatorModel With {
            .EstimatorID = reader.GetInt32(ordEstimatorID),
            .EstimatorName = If(reader.IsDBNull(ordEstimator), String.Empty, reader.GetString(ordEstimator))
        },
        .Address = If(reader.IsDBNull(ordAddress), String.Empty, reader.GetString(ordAddress)),
        .City = If(reader.IsDBNull(ordCity), String.Empty, reader.GetString(ordCity)),
        .State = If(reader.IsDBNull(ordState), String.Empty, reader.GetString(ordState)),
        .Zip = If(reader.IsDBNull(ordZip), String.Empty, reader.GetString(ordZip)),
        .BidDate = If(reader.IsDBNull(ordBidDate), CType(Nothing, Date?), reader.GetDateTime(ordBidDate)),
        .ArchPlansDated = If(reader.IsDBNull(ordArchPlansDated), CType(Nothing, Date?), reader.GetDateTime(ordArchPlansDated)),
        .EngPlansDated = If(reader.IsDBNull(ordEngPlansDated), CType(Nothing, Date?), reader.GetDateTime(ordEngPlansDated)),
        .MilesToJobSite = If(reader.IsDBNull(ordMilesToJobSite), 0, reader.GetInt32(ordMilesToJobSite)),
        .TotalNetSqft = If(reader.IsDBNull(ordTotalNetSqft), CType(Nothing, Integer?), reader.GetInt32(ordTotalNetSqft)),
        .TotalGrossSqft = If(reader.IsDBNull(ordTotalGrossSqft), 0, reader.GetInt32(ordTotalGrossSqft)),
        .ArchitectID = If(reader.IsDBNull(ordArchitectID), CType(Nothing, Integer?), reader.GetInt32(ordArchitectID)),
        .EngineerID = If(reader.IsDBNull(ordEngineerID), CType(Nothing, Integer?), reader.GetInt32(ordEngineerID)),
        .ProjectNotes = If(reader.IsDBNull(ordProjectNotes), String.Empty, reader.GetString(ordProjectNotes)),
        .LastModifiedDate = If(reader.IsDBNull(ordLastModifiedDate), Date.MinValue, reader.GetDateTime(ordLastModifiedDate)),
        .CreatedDate = If(reader.IsDBNull(ordCreatedDate), Date.MinValue, reader.GetDateTime(ordCreatedDate)),
        .ArchitectName = If(reader.IsDBNull(ordArchitectName), String.Empty, reader.GetString(ordArchitectName)),
        .EngineerName = If(reader.IsDBNull(ordEngineerName), String.Empty, reader.GetString(ordEngineerName))
    }
        End Function

        ' === REFACTORED: GetProjects ===
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

        ' === REFACTORED: GetProjectByID ===
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


        ' Updated: GetProjects (use VersionID for details)
        'Public Function GetProjects(Optional includeDetails As Boolean = True) As List(Of ProjectModel)
        '    Dim projects As New List(Of ProjectModel)
        '    SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
        '                                                               Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjects)
        '                                                                   While reader.Read()
        '                                                                       Dim proj As New ProjectModel With {
        '                                                                  .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
        '                                                                  .JBID = If(Not reader.IsDBNull(reader.GetOrdinal("JBID")), reader.GetString(reader.GetOrdinal("JBID")), String.Empty),
        '                                                                  .ProjectType = New ProjectTypeModel With {
        '                                                                      .ProjectTypeID = reader.GetInt32(reader.GetOrdinal("ProjectTypeID")),
        '                                                                      .ProjectTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectType")), reader.GetString(reader.GetOrdinal("ProjectType")), String.Empty)
        '                                                                  },
        '                                                                  .ProjectName = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectName")), reader.GetString(reader.GetOrdinal("ProjectName")), String.Empty),
        '                                                                  .Estimator = New EstimatorModel With {
        '                                                                      .EstimatorID = reader.GetInt32(reader.GetOrdinal("EstimatorID")),
        '                                                                      .EstimatorName = If(Not reader.IsDBNull(reader.GetOrdinal("Estimator")), reader.GetString(reader.GetOrdinal("Estimator")), String.Empty)
        '                                                                  },
        '                                                                  .Address = If(Not reader.IsDBNull(reader.GetOrdinal("Address")), reader.GetString(reader.GetOrdinal("Address")), String.Empty),
        '                                                                  .City = If(Not reader.IsDBNull(reader.GetOrdinal("City")), reader.GetString(reader.GetOrdinal("City")), String.Empty),
        '                                                                  .State = If(Not reader.IsDBNull(reader.GetOrdinal("State")), reader.GetString(reader.GetOrdinal("State")), String.Empty),
        '                                                                  .Zip = If(Not reader.IsDBNull(reader.GetOrdinal("Zip")), reader.GetString(reader.GetOrdinal("Zip")), String.Empty),
        '                                                                  .BidDate = If(Not reader.IsDBNull(reader.GetOrdinal("BidDate")), reader.GetDateTime(reader.GetOrdinal("BidDate")), Nothing),
        '                                                                  .ArchPlansDated = If(Not reader.IsDBNull(reader.GetOrdinal("ArchPlansDated")), reader.GetDateTime(reader.GetOrdinal("ArchPlansDated")), Nothing),
        '                                                                  .EngPlansDated = If(Not reader.IsDBNull(reader.GetOrdinal("EngPlansDated")), reader.GetDateTime(reader.GetOrdinal("EngPlansDated")), Nothing),
        '                                                                  .MilesToJobSite = If(Not reader.IsDBNull(reader.GetOrdinal("MilesToJobSite")), reader.GetInt32(reader.GetOrdinal("MilesToJobSite")), 0),
        '                                                                  .TotalNetSqft = If(Not reader.IsDBNull(reader.GetOrdinal("TotalNetSqft")), reader.GetInt32(reader.GetOrdinal("TotalNetSqft")), Nothing),
        '                                                                  .TotalGrossSqft = If(Not reader.IsDBNull(reader.GetOrdinal("TotalGrossSqft")), reader.GetInt32(reader.GetOrdinal("TotalGrossSqft")), 0),
        '                                                                  .ArchitectID = If(Not reader.IsDBNull(reader.GetOrdinal("ArchitectID")), reader.GetInt32(reader.GetOrdinal("ArchitectID")), Nothing),
        '                                                                  .EngineerID = If(Not reader.IsDBNull(reader.GetOrdinal("EngineerID")), reader.GetInt32(reader.GetOrdinal("EngineerID")), Nothing),
        '                                                                  .ProjectNotes = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectNotes")), reader.GetString(reader.GetOrdinal("ProjectNotes")), String.Empty),
        '                                                                  .LastModifiedDate = If(Not reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")), reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")), Nothing),
        '                                                                  .CreatedDate = If(Not reader.IsDBNull(reader.GetOrdinal("createddate")), reader.GetDateTime(reader.GetOrdinal("createddate")), Nothing),
        '                                                                  .ArchitectName = If(Not reader.IsDBNull(reader.GetOrdinal("ArchitectName")), reader.GetString(reader.GetOrdinal("ArchitectName")), String.Empty),
        '                                                                  .EngineerName = If(Not reader.IsDBNull(reader.GetOrdinal("EngineerName")), reader.GetString(reader.GetOrdinal("EngineerName")), String.Empty)
        '                                                              }

        '                                                                       If includeDetails Then
        '                                                                           Dim versions As List(Of ProjectVersionModel) = ProjVersionDataAccess.GetProjectVersions(proj.ProjectID)
        '                                                                           If versions.Any() Then
        '                                                                               Dim latestVersionID As Integer = versions.First().VersionID
        '                                                                               proj.Buildings = GetBuildingsByVersionID(latestVersionID)
        '                                                                               proj.Settings = GetProjectProductSettings(latestVersionID)
        '                                                                           End If
        '                                                                       End If

        '                                                                       projects.Add(proj)
        '                                                                   End While
        '                                                               End Using
        '                                                           End Sub, "Error loading Project")
        '    Return projects
        'End Function

        '' Updated: GetProjectByID (use VersionID for details)
        'Public Function GetProjectByID(projectID As Integer) As ProjectModel
        '    Dim proj As ProjectModel = Nothing
        '    Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
        '    SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
        '                                                               Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectByID, params)
        '                                                                   If reader.Read() Then
        '                                                                       proj = New ProjectModel With {
        '                                                                  .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
        '                                                                  .JBID = If(Not reader.IsDBNull(reader.GetOrdinal("JBID")), reader.GetString(reader.GetOrdinal("JBID")), String.Empty),
        '                                                                  .ProjectType = New ProjectTypeModel With {
        '                                                                      .ProjectTypeID = reader.GetInt32(reader.GetOrdinal("ProjectTypeID")),
        '                                                                      .ProjectTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectType")), reader.GetString(reader.GetOrdinal("ProjectType")), String.Empty)
        '                                                                  },
        '                                                                  .ProjectName = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectName")), reader.GetString(reader.GetOrdinal("ProjectName")), String.Empty),
        '                                                                  .Estimator = New EstimatorModel With {
        '                                                                      .EstimatorID = reader.GetInt32(reader.GetOrdinal("EstimatorID")),
        '                                                                      .EstimatorName = If(Not reader.IsDBNull(reader.GetOrdinal("Estimator")), reader.GetString(reader.GetOrdinal("Estimator")), String.Empty)
        '                                                                  },
        '                                                                  .Address = If(Not reader.IsDBNull(reader.GetOrdinal("Address")), reader.GetString(reader.GetOrdinal("Address")), String.Empty),
        '                                                                  .City = If(Not reader.IsDBNull(reader.GetOrdinal("City")), reader.GetString(reader.GetOrdinal("City")), String.Empty),
        '                                                                  .State = If(Not reader.IsDBNull(reader.GetOrdinal("State")), reader.GetString(reader.GetOrdinal("State")), String.Empty),
        '                                                                  .Zip = If(Not reader.IsDBNull(reader.GetOrdinal("Zip")), reader.GetString(reader.GetOrdinal("Zip")), String.Empty),
        '                                                                  .BidDate = If(Not reader.IsDBNull(reader.GetOrdinal("BidDate")), reader.GetDateTime(reader.GetOrdinal("BidDate")), Nothing),
        '                                                                  .ArchPlansDated = If(Not reader.IsDBNull(reader.GetOrdinal("ArchPlansDated")), reader.GetDateTime(reader.GetOrdinal("ArchPlansDated")), Nothing),
        '                                                                  .EngPlansDated = If(Not reader.IsDBNull(reader.GetOrdinal("EngPlansDated")), reader.GetDateTime(reader.GetOrdinal("EngPlansDated")), Nothing),
        '                                                                  .MilesToJobSite = If(Not reader.IsDBNull(reader.GetOrdinal("MilesToJobSite")), reader.GetInt32(reader.GetOrdinal("MilesToJobSite")), 0),
        '                                                                  .TotalNetSqft = If(Not reader.IsDBNull(reader.GetOrdinal("TotalNetSqft")), reader.GetInt32(reader.GetOrdinal("TotalNetSqft")), Nothing),
        '                                                                  .TotalGrossSqft = If(Not reader.IsDBNull(reader.GetOrdinal("TotalGrossSqft")), reader.GetInt32(reader.GetOrdinal("TotalGrossSqft")), 0),
        '                                                                  .ArchitectID = If(Not reader.IsDBNull(reader.GetOrdinal("ArchitectID")), reader.GetInt32(reader.GetOrdinal("ArchitectID")), Nothing),
        '                                                                  .EngineerID = If(Not reader.IsDBNull(reader.GetOrdinal("EngineerID")), reader.GetInt32(reader.GetOrdinal("EngineerID")), Nothing),
        '                                                                  .ProjectNotes = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectNotes")), reader.GetString(reader.GetOrdinal("ProjectNotes")), String.Empty),
        '                                                                  .LastModifiedDate = If(Not reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")), reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")), Nothing),
        '                                                                  .CreatedDate = If(Not reader.IsDBNull(reader.GetOrdinal("createddate")), reader.GetDateTime(reader.GetOrdinal("createddate")), Nothing),
        '                                                                  .ArchitectName = If(Not reader.IsDBNull(reader.GetOrdinal("ArchitectName")), reader.GetString(reader.GetOrdinal("ArchitectName")), String.Empty),
        '                                                                  .EngineerName = If(Not reader.IsDBNull(reader.GetOrdinal("EngineerName")), reader.GetString(reader.GetOrdinal("EngineerName")), String.Empty)
        '                                                              }
        '                                                                       Dim versions As List(Of ProjectVersionModel) = ProjVersionDataAccess.GetProjectVersions(proj.ProjectID)
        '                                                                       If versions.Any() Then
        '                                                                           Dim latestVersionID As Integer = versions.First().VersionID
        '                                                                           proj.Buildings = GetBuildingsByVersionID(latestVersionID)
        '                                                                           proj.Settings = GetProjectProductSettings(latestVersionID)
        '                                                                       End If
        '                                                                   End If
        '                                                               End Using
        '                                                           End Sub, "Error loading project by ID " & projectID)
        '    Return proj
        'End Function
        ' Added: GetBuildingsByVersionID (use VersionID)
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

        Public Function InsertRawUnit(model As RawUnitModel) As Integer
            Dim newRawUnitID As Integer

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim paramsDict As Dictionary(Of String, Object) = ModelParams.ForRawUnit(model, model.VersionID)
                                                                       newRawUnitID = SqlConnectionManager.Instance.ExecuteScalar(Of Integer)(Queries.InsertRawUnit, HelperDataAccess.BuildParameters(paramsDict))
                                                                   End Sub, "Error inserting RawUnit " & model.RawUnitName)

            Return newRawUnitID
        End Function

        Public Sub UpdateRawUnit(model As RawUnitModel)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim paramsDict As Dictionary(Of String, Object) = ModelParams.ForRawUnit(model, model.VersionID) ' VersionID is ignored by UPDATE but harmless
                                                                       paramsDict.Remove("@VersionID")   ' clean – not required for UPDATE
                                                                       paramsDict.Remove("@ProductTypeID") ' clean – not required for UPDATE
                                                                       paramsDict.Add("@RawUnitID", model.RawUnitID)

                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateRawUnit, HelperDataAccess.BuildParameters(paramsDict))
                                                                   End Sub, "Error updating RawUnit " & model.RawUnitName)
        End Sub



        ' GetLevelsByBuildingID (unchanged, uses BuildingID)
        Public Shared Function GetLevelsByBuildingID(buildingID As Integer) As List(Of LevelModel)
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
        Public Shared Function GetProjectProductSettings(versionID As Integer) As List(Of ProjectProductSettingsModel)
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
        Public Shared Sub SaveBuilding(bldg As BuildingModel, versionID As Integer)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@BuildingName", If(String.IsNullOrEmpty(bldg.BuildingName), DBNull.Value, CType(bldg.BuildingName, Object))},
                {"@BuildingType", If(bldg.BuildingType.HasValue, CType(bldg.BuildingType.Value, Object), DBNull.Value)},
                {"@ResUnits", If(bldg.ResUnits.HasValue, CType(bldg.ResUnits.Value, Object), DBNull.Value)},
                {"@BldgQty", bldg.BldgQty},
                {"@LastModifiedDate", Now},
                {"@VersionID", versionID}
            }

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If bldg.BuildingID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertBuilding, HelperDataAccess.BuildParameters(paramsDict))
                                                                           bldg.BuildingID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@BuildingID", bldg.BuildingID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateBuilding, HelperDataAccess.BuildParameters(paramsDict))
                                                                       End If

                                                                       For Each level In bldg.Levels
                                                                           SaveLevel(level, bldg.BuildingID, versionID)
                                                                       Next
                                                                   End Sub, "Error saving building")
        End Sub

        ' Updated: SaveLevel (use VersionID)
        Public Shared Sub SaveLevel(level As LevelModel, buildingID As Integer, versionID As Integer)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@VersionID", versionID},
                {"@BuildingID", buildingID},
                {"@ProductTypeID", level.ProductTypeID},
                {"@LevelNumber", level.LevelNumber},
                {"@LastModifiedDate", Now},
                {"@LevelName", level.LevelName}
            }

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If level.LevelID = 0 Then
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertLevel, HelperDataAccess.BuildParameters(paramsDict))
                                                                           level.LevelID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@LevelID", level.LevelID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLevel, HelperDataAccess.BuildParameters(paramsDict))
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
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectProductSetting, HelperDataAccess.BuildParameters(paramsDict))
                                                                           If newIDObj IsNot Nothing Then setting.SettingID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@SettingID", setting.SettingID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectProductSetting, HelperDataAccess.BuildParameters(paramsDict))
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

        Public Function GetProjectVersionStatus() As List(Of ProjectVersionStatus)
            Dim types As New List(Of ProjectVersionStatus)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectStatus)
                                                                           While reader.Read()
                                                                               Dim pt As New ProjectVersionStatus With {
                                                    .ProjVersionStatusID = reader.GetInt32(reader.GetOrdinal("ProjVersionStatusID")),
                                                    .ProjVersionStatus = If(Not reader.IsDBNull(reader.GetOrdinal("ProjVersionStatus")), reader.GetString(reader.GetOrdinal("ProjVersionStatus")), String.Empty)
                                                }
                                                                               types.Add(pt)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading project Version Status")
            Return types
        End Function


        ' Updated: SaveActualUnit (use VersionID)
        ' Update the SaveActualUnit method to include SortOrder parameter
        Public Sub SaveActualUnit(unit As ActualUnitModel)
            Dim parameters As New Dictionary(Of String, Object) From {
        {"@VersionID", unit.VersionID},
        {"@RawUnitID", unit.RawUnitID},
        {"@ProductTypeID", unit.ProductTypeID},
        {"@UnitName", unit.UnitName},
        {"@PlanSQFT", unit.PlanSQFT},
        {"@UnitType", unit.UnitType},
        {"@OptionalAdder", unit.OptionalAdder},
        {"@ColorCode", If(String.IsNullOrEmpty(unit.ColorCode), DBNull.Value, CObj(unit.ColorCode))},
        {"@MarginPercent", unit.MarginPercent},
        {"@SortOrder", unit.SortOrder}
    }

            If unit.ActualUnitID > 0 Then
                ' Update existing
                parameters.Add("@ActualUnitID", unit.ActualUnitID)
                SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateActualUnit, HelperDataAccess.BuildParameters(parameters))
            Else
                ' Insert new
                unit.ActualUnitID = SqlConnectionManager.Instance.ExecuteScalar(Of Integer)(Queries.InsertActualUnit, HelperDataAccess.BuildParameters(parameters))
            End If
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
                                                                           Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertActualToLevelMapping, HelperDataAccess.BuildParameters(paramsDict))
                                                                           If newIDObj IsNot Nothing Then mapping.MappingID = CInt(newIDObj)
                                                                       Else
                                                                           paramsDict.Add("@MappingID", mapping.MappingID)
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateActualToLevelMapping, HelperDataAccess.BuildParameters(paramsDict))
                                                                       End If
                                                                   End Sub, "Error saving actual to level mapping")
        End Sub

        ' GetActualToLevelMappingsByLevelID (unchanged query, uses LevelID)
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

        ' GetActualUnitByID (unchanged)
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

        ' GetCalculatedComponentsByActualUnitID (unchanged)
        Public Shared Function GetCalculatedComponentsByActualUnitID(actualUnitID As Integer) As List(Of CalculatedComponentModel)
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
                                                                           SqlConnectionManager.Instance.ExecuteNonQuery(Queries.InsertCalculatedComponent, HelperDataAccess.BuildParameters(insertParams))
                                                                       Next
                                                                   End Sub, "Error saving calculated components")
        End Sub

        ' Updated: GetRawUnitsByVersionID (use VersionID)
        Public Shared Function GetRawUnitsByVersionID(versionID As Integer) As List(Of RawUnitModel)
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
                                                                               rawUnits.Add(rawUnit)
                                                                           End While
                                                                       End Using
                                                                   End Sub, "Error loading raw units for version " & versionID)
            Return rawUnits
        End Function


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
        Public Shared Function GetConfigValue(configKey As String) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@ConfigKey", configKey)}
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectConfigValue, params)
                                                                   End Sub, "Error fetching config value for " & configKey)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        ' GetMilesToJobSite (unchanged, uses ProjectID)
        Public Shared Function GetMilesToJobSite(projectID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectMilesToJobSite, params)
                                                                   End Sub, "Error fetching miles for project " & projectID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0, CInt(valObj))
        End Function




        ' GetRawUnitByID (unchanged)
        ' GetRawUnitByID (updated to retrieve all fields)
        Public Shared Function GetRawUnitByID(rawUnitID As Integer) As RawUnitModel
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



        ' In DataAccess.vb: Add these methods

        Public Shared Function GetProjectIDByVersionID(versionID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID)}
            Dim idObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)("SELECT ProjectID FROM ProjectVersions WHERE VersionID = @VersionID", params)
            Return If(idObj Is DBNull.Value OrElse idObj Is Nothing, 0, CInt(idObj))
        End Function

        Public Shared Function GetLevelInfo(levelID As Integer) As Tuple(Of Integer, Integer, Decimal)
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

        Public Shared Function GetEffectiveMargin(versionID As Integer, productTypeID As Integer, rawUnitID As Integer) As Decimal
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
        ' Updated: GetMarginPercent (use VersionID)
        Public Shared Function GetMarginPercent(versionID As Integer, productTypeID As Integer) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID), New SqlParameter("@ProductTypeID", productTypeID)}
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectMarginPercent, params)
                                                                   End Sub, "Error fetching margin percent for version " & versionID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        Public Shared Function ComputeLevelUnitsDataTable(levelID As Integer) As DataTable
            Dim dt As New DataTable()

            ' Define columns once — matches your DisplayUnitData properties
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

            For Each mapping In mappings
                Dim actual = mapping.ActualUnit
                Dim raw = GetRawUnitByID(actual.RawUnitID)

                If raw Is Nothing Then Continue For

                ' For walls (ProductTypeID = 3), use LF as the basis instead of SqFt
                Dim isWallUnit As Boolean = (raw.ProductTypeID = 3)
                Dim divisor As Decimal = If(isWallUnit, 1, raw.SqFt.GetValueOrDefault())

                ' Skip if no valid divisor (prevents division by zero)
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

            ' Second pass: apply delivery cost
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

            ' Add all rows at once
            For Each row In tempRows
                dt.Rows.Add(row)
            Next

            Return dt
        End Function

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