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

        ' Helper method for executing queries with error handling
        Private Sub ExecuteWithErrorHandling(action As Action, ByVal errorMessage As String)
            Try
                action()
            Catch ex As Exception
                Throw New ApplicationException(errorMessage & ": " & ex.Message)
            End Try
        End Sub

        ' Helper for executing non-query with transaction
        Private Sub ExecuteNonQueryTransactional(ByVal query As String, ByVal params As SqlParameter(), ByVal conn As SqlConnection, ByVal transaction As SqlTransaction)
            Using cmd As New SqlCommand(query, conn, transaction)
                cmd.Parameters.AddRange(params)
                cmd.ExecuteNonQuery()
            End Using
        End Sub

        Public Sub ImportRawUnits(projectID As Integer, csvPath As String, productTypeID As Integer)
            ExecuteWithErrorHandling(Sub()
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
                                                         .ProjectID = projectID,
                                                         .ProductTypeID = productTypeID
                                                     }

                                                     For i As Integer = 0 To headers.Length - 1
                                                         Dim header As String = headers(i).Trim().ToUpper()
                                                         Dim valueStr As String = fields(i).Trim()

                                                         If skippedHeaders.Contains(header) Then Continue For

                                                         If header = "ELEVATION" Then
                                                             rawUnit.RawUnitName = valueStr
                                                         ElseIf header = "PRODUCT" Then
                                                             If String.Equals(valueStr, "Floor", StringComparison.OrdinalIgnoreCase) Then rawUnit.ProductTypeID = 1
                                                             If String.Equals(valueStr, "Roof", StringComparison.OrdinalIgnoreCase) Then rawUnit.ProductTypeID = 2
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

                                                     Dim insertParams As New Dictionary(Of String, Object) From {
                                                         {"@RawUnitName", If(String.IsNullOrEmpty(rawUnit.RawUnitName), DBNull.Value, CType(rawUnit.RawUnitName, Object))},
                                                         {"@ProjectID", rawUnit.ProjectID},
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
                                     End Sub, "RawUnits import failed for project " & projectID)
        End Sub

        ' Merged GetProjectList and GetProjects into one method with optional parameter for full load
        Public Function GetProjects(Optional includeDetails As Boolean = True) As List(Of ProjectModel)
            Dim projects As New List(Of ProjectModel)
            ExecuteWithErrorHandling(Sub()
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
                                             .ProjectArchitect = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectArchitect")), reader.GetString(reader.GetOrdinal("ProjectArchitect")), String.Empty),
                                             .ProjectEngineer = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectEngineer")), reader.GetString(reader.GetOrdinal("ProjectEngineer")), String.Empty),
                                             .ProjectNotes = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectNotes")), reader.GetString(reader.GetOrdinal("ProjectNotes")), String.Empty),
                                             .LastModifiedDate = If(Not reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")), reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")), Nothing),
                                             .CreatedDate = If(Not reader.IsDBNull(reader.GetOrdinal("createddate")), reader.GetDateTime(reader.GetOrdinal("createddate")), Nothing)
                                         }

                                                 If Not includeDetails Then
                                                     Dim primaryCustomerName As String = If(Not reader.IsDBNull(reader.GetOrdinal("PrimaryCustomer")), reader.GetString(reader.GetOrdinal("PrimaryCustomer")), String.Empty)
                                                     If Not String.IsNullOrEmpty(primaryCustomerName) Then
                                                         proj.Customers.Add(New CustomerModel With {.CustomerName = primaryCustomerName})
                                                     End If

                                                     Dim primarySalesmanName As String = If(Not reader.IsDBNull(reader.GetOrdinal("PrimarySalesman")), reader.GetString(reader.GetOrdinal("PrimarySalesman")), String.Empty)
                                                     If Not String.IsNullOrEmpty(primarySalesmanName) Then
                                                         proj.Salespeople.Add(New SalesModel With {.SalesName = primarySalesmanName})
                                                     End If
                                                 Else
                                                     proj.Buildings = GetBuildingsByProjectID(proj.ProjectID)
                                                     proj.Settings = GetProjectProductSettings(proj.ProjectID)
                                                     proj.Customers = GetCustomersByProject(proj.ProjectID)
                                                     proj.Salespeople = GetSalesByProject(proj.ProjectID)
                                                 End If

                                                 projects.Add(proj)
                                             End While
                                         End Using
                                     End Sub, "Error loading Project")
            Return projects
        End Function

        Public Function GetProjectByID(projectID As Integer) As ProjectModel
            Dim proj As ProjectModel = Nothing
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectByID, params)
                                             If reader.Read() Then
                                                 proj = New ProjectModel With {
                                                     .projectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
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
                                                     .ProjectArchitect = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectArchitect")), reader.GetString(reader.GetOrdinal("ProjectArchitect")), String.Empty),
                                                     .ProjectEngineer = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectEngineer")), reader.GetString(reader.GetOrdinal("ProjectEngineer")), String.Empty),
                                                     .ProjectNotes = If(Not reader.IsDBNull(reader.GetOrdinal("ProjectNotes")), reader.GetString(reader.GetOrdinal("ProjectNotes")), String.Empty),
                                                     .LastModifiedDate = If(Not reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")), reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")), Nothing),
                                                     .CreatedDate = If(Not reader.IsDBNull(reader.GetOrdinal("createddate")), reader.GetDateTime(reader.GetOrdinal("createddate")), Nothing)
                                                 }
                                                 proj.Buildings = GetBuildingsByProjectID(proj.ProjectID)
                                                 proj.Settings = GetProjectProductSettings(proj.ProjectID)
                                                 proj.Customers = GetCustomersByProject(proj.ProjectID)
                                                 proj.Salespeople = GetSalesByProject(proj.ProjectID)
                                             End If
                                         End Using
                                     End Sub, "Error loading project by ID " & projectID)
            Return proj
        End Function

        Public Function GetLevelsByBuildingID(buildingID As Integer) As List(Of LevelModel)
            Dim levels As New List(Of LevelModel)
            Dim params As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectLevelsByBuilding, params)
                                             While reader.Read()
                                                 Dim level As New LevelModel With {
                                                     .LevelID = reader.GetInt32(reader.GetOrdinal("LevelID")),
                                                     .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                                                     .buildingID = buildingID,
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

        Public Function GetBuildingIDByLevelID(levelID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
            Dim buildingIDObj As Object = Nothing
            ExecuteWithErrorHandling(Sub()
                                         buildingIDObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.GetBuildingIDByLevelID, params)
                                     End Sub, "Error fetching BuildingID for LevelID " & levelID)
            Return If(buildingIDObj Is DBNull.Value OrElse buildingIDObj Is Nothing, 0, CInt(buildingIDObj))
        End Function

        Public Function GetBuildingsByProjectID(projectID As Integer) As List(Of BuildingModel)
            Dim buildings As New List(Of BuildingModel)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectBuildingsByProject, params)
                                             While reader.Read()
                                                 Dim bldg As New BuildingModel With {
                                                     .BuildingID = reader.GetInt32(reader.GetOrdinal("BuildingID")),
                                                     .BuildingName = If(Not reader.IsDBNull(reader.GetOrdinal("BuildingName")), reader.GetString(reader.GetOrdinal("BuildingName")), String.Empty),
                                                     .BuildingType = If(Not reader.IsDBNull(reader.GetOrdinal("BuildingType")), reader.GetInt32(reader.GetOrdinal("BuildingType")), Nothing),
                                                     .ResUnits = If(Not reader.IsDBNull(reader.GetOrdinal("ResUnits")), reader.GetInt32(reader.GetOrdinal("ResUnits")), Nothing),
                                                     .BldgQty = reader.GetInt32(reader.GetOrdinal("BldgQty")),
                                                     .projectID = projectID,
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
                                     End Sub, "Error loading buildings for project " & projectID)
            Return buildings
        End Function

        Public Function GetProductTypes() As List(Of ProductTypeModel)
            Dim types As New List(Of ProductTypeModel)
            ExecuteWithErrorHandling(Sub()
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

        Public Function GetProjectProductSettings(projectID As Integer) As List(Of ProjectProductSettingsModel)
            Dim settings As New List(Of ProjectProductSettingsModel)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectProductSettings, params)
                                             While reader.Read()
                                                 Dim setting As New ProjectProductSettingsModel With {
                                                     .SettingID = reader.GetInt32(reader.GetOrdinal("SettingID")),
                                                     .projectID = projectID,
                                                     .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                                     .MarginPercent = If(Not reader.IsDBNull(reader.GetOrdinal("MarginPercent")), reader.GetDecimal(reader.GetOrdinal("MarginPercent")), Nothing),
                                                     .LumberAdder = If(Not reader.IsDBNull(reader.GetOrdinal("LumberAdder")), reader.GetDecimal(reader.GetOrdinal("LumberAdder")), Nothing)
                                                 }
                                                 settings.Add(setting)
                                             End While
                                         End Using
                                     End Sub, "Error loading settings for project " & projectID)
            Return settings
        End Function

        ' Generic method for mapped entities (customers and sales)
        Private Function GetMappedEntitiesByProject(Of T As New)(projectID As Integer, query As String, mapFunction As Func(Of SqlDataReader, T)) As List(Of T)
            Dim entities As New List(Of T)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(query, params)
                                             While reader.Read()
                                                 entities.Add(mapFunction(reader))
                                             End While
                                         End Using
                                     End Sub, "Error loading mapped entities for project " & projectID)
            Return entities
        End Function

        Public Function GetCustomersByProject(projectID As Integer) As List(Of CustomerModel)
            Return GetMappedEntitiesByProject(Of CustomerModel)(projectID, Queries.SelectCustomersByProject, Function(reader) New CustomerModel With {
                .CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                .CustomerName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerName")), reader.GetString(reader.GetOrdinal("CustomerName")), String.Empty)
            })
        End Function

        Public Function GetSalesByProject(projectID As Integer) As List(Of SalesModel)
            Return GetMappedEntitiesByProject(Of SalesModel)(projectID, Queries.SelectSalesByProject, Function(reader) New SalesModel With {
                .SalesID = reader.GetInt32(reader.GetOrdinal("SalesID")),
                .SalesName = If(Not reader.IsDBNull(reader.GetOrdinal("SalesName")), reader.GetString(reader.GetOrdinal("SalesName")), String.Empty)
            })
        End Function

        Public Sub SaveProject(proj As ProjectModel)
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
                {"@ProjectArchitect", If(String.IsNullOrEmpty(proj.ProjectArchitect), DBNull.Value, CType(proj.ProjectArchitect, Object))},
                {"@ProjectEngineer", If(String.IsNullOrEmpty(proj.ProjectEngineer), DBNull.Value, CType(proj.ProjectEngineer, Object))},
                {"@ProjectNotes", If(String.IsNullOrEmpty(proj.ProjectNotes), DBNull.Value, CType(proj.ProjectNotes, Object))},
                {"@LastModifiedDate", Now()}
            }

            ExecuteWithErrorHandling(Sub()
                                         Dim isNew As Boolean = proj.ProjectID = 0
                                         If isNew Then
                                             Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProject, BuildParameters(paramsDict))
                                             If newIDObj IsNot Nothing Then proj.ProjectID = CInt(newIDObj)
                                         Else
                                             paramsDict.Add("@ProjectID", proj.ProjectID)
                                             SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProject, BuildParameters(paramsDict))
                                         End If

                                         ' Save Settings
                                         For Each setting In proj.Settings
                                             SaveProjectProductSetting(setting, proj.ProjectID)
                                         Next

                                         ' Save Customers Mappings
                                         SqlConnectionManager.Instance.ExecuteNonQuery("DELETE FROM CustomertoProjectMapping WHERE ProjectID = @ProjectID", {New SqlParameter("@ProjectID", proj.ProjectID)})
                                         For Each customer In proj.Customers
                                             SqlConnectionManager.Instance.ExecuteNonQuery(Queries.InsertCustomerToProject, {New SqlParameter("@ProjectID", proj.ProjectID), New SqlParameter("@CustomerID", customer.CustomerID)})
                                         Next

                                         ' Save Salespeople Mappings
                                         SqlConnectionManager.Instance.ExecuteNonQuery("DELETE FROM SalestoProjectMapping WHERE ProjectID = @ProjectID", {New SqlParameter("@ProjectID", proj.ProjectID)})
                                         For Each sale In proj.Salespeople
                                             SqlConnectionManager.Instance.ExecuteNonQuery(Queries.InsertSalesToProject, {New SqlParameter("@ProjectID", proj.ProjectID), New SqlParameter("@SalesID", sale.SalesID)})
                                         Next

                                         ' Save Buildings
                                         For Each bldg In proj.Buildings
                                             SaveBuilding(bldg, proj.ProjectID)
                                         Next
                                     End Sub, "Error saving project")
        End Sub

        Public Sub SaveBuilding(bldg As BuildingModel, projectID As Integer)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@BuildingName", If(String.IsNullOrEmpty(bldg.BuildingName), DBNull.Value, CType(bldg.BuildingName, Object))},
                {"@BuildingType", If(bldg.BuildingType.HasValue, CType(bldg.BuildingType.Value, Object), DBNull.Value)},
                {"@ResUnits", If(bldg.ResUnits.HasValue, CType(bldg.ResUnits.Value, Object), DBNull.Value)},
                {"@BldgQty", bldg.BldgQty},
                {"@ProjectID", projectID}
            }

            ExecuteWithErrorHandling(Sub()
                                         If bldg.BuildingID = 0 Then
                                             Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertBuilding, BuildParameters(paramsDict))
                                             bldg.BuildingID = CInt(newIDObj)
                                         Else
                                             paramsDict.Add("@BuildingID", bldg.BuildingID)
                                             SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateBuilding, BuildParameters(paramsDict))
                                         End If

                                         For Each level In bldg.Levels
                                             SaveLevel(level, bldg.BuildingID, projectID)
                                         Next
                                     End Sub, "Error saving building")
        End Sub

        Public Sub SaveLevel(level As LevelModel, buildingID As Integer, projectID As Integer)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@ProjectID", projectID},
                {"@BuildingID", buildingID},
                {"@ProductTypeID", level.ProductTypeID},
                {"@LevelNumber", level.LevelNumber},
                {"@LevelName", level.LevelName}
            }

            ExecuteWithErrorHandling(Sub()
                                         If level.LevelID = 0 Then
                                             Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertLevel, BuildParameters(paramsDict))
                                             level.LevelID = CInt(newIDObj)
                                         Else
                                             paramsDict.Add("@LevelID", level.LevelID)
                                             SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLevel, BuildParameters(paramsDict))
                                         End If
                                     End Sub, "Error saving level")
        End Sub

        Public Sub SaveProjectProductSetting(setting As ProjectProductSettingsModel, projectID As Integer)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@ProjectID", projectID},
                {"@ProductTypeID", setting.ProductTypeID},
                {"@MarginPercent", If(setting.MarginPercent.HasValue, CType(setting.MarginPercent.Value, Object), DBNull.Value)},
                {"@LumberAdder", If(setting.LumberAdder.HasValue AndAlso setting.LumberAdder.Value > 0, CType(setting.LumberAdder.Value, Object), DBNull.Value)}
            }

            ExecuteWithErrorHandling(Sub()
                                         If setting.SettingID = 0 Then
                                             Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectProductSetting, BuildParameters(paramsDict))
                                             If newIDObj IsNot Nothing Then setting.SettingID = CInt(newIDObj)
                                         Else
                                             paramsDict.Add("@SettingID", setting.SettingID)
                                             SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectProductSetting, BuildParameters(paramsDict))
                                         End If
                                     End Sub, "Error saving project setting")
        End Sub

        Public Function GetProjectTypes() As List(Of ProjectTypeModel)
            Dim types As New List(Of ProjectTypeModel)
            ExecuteWithErrorHandling(Sub()
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

        Public Function SaveProjectType(projectType As ProjectTypeModel) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@ProjectTypeName", If(String.IsNullOrEmpty(projectType.ProjectTypeName), DBNull.Value, CType(projectType.ProjectTypeName, Object))},
                {"@Description", If(String.IsNullOrEmpty(projectType.Description), DBNull.Value, CType(projectType.Description, Object))}
            }

            Dim result As Integer = 0
            ExecuteWithErrorHandling(Sub()
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

        Public Function GetEstimators() As List(Of EstimatorModel)
            Dim estimators As New List(Of EstimatorModel)
            ExecuteWithErrorHandling(Sub()
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

        Public Function SaveEstimator(estimator As EstimatorModel) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@EstimatorName", If(String.IsNullOrEmpty(estimator.EstimatorName), DBNull.Value, CType(estimator.EstimatorName, Object))}
            }

            Dim result As Integer = 0
            ExecuteWithErrorHandling(Sub()
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

        Public Function GetCustomers() As List(Of CustomerModel)
            Dim customers As New List(Of CustomerModel)
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectCustomers)
                                             While reader.Read()
                                                 Dim customer As New CustomerModel With {
                                                     .CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                                                     .CustomerName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerName")), reader.GetString(reader.GetOrdinal("CustomerName")), String.Empty)
                                                 }
                                                 customers.Add(customer)
                                             End While
                                         End Using
                                     End Sub, "Error loading customers")
            Return customers
        End Function

        Public Function SaveCustomer(customer As CustomerModel) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@CustomerName", If(String.IsNullOrEmpty(customer.CustomerName), DBNull.Value, CType(customer.CustomerName, Object))}
            }

            Dim result As Integer = 0
            ExecuteWithErrorHandling(Sub()
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

        Public Function GetSales() As List(Of SalesModel)
            Dim sales As New List(Of SalesModel)
            ExecuteWithErrorHandling(Sub()
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

        Public Function SaveSales(sale As SalesModel) As Integer
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@SalesName", If(String.IsNullOrEmpty(sale.SalesName), DBNull.Value, CType(sale.SalesName, Object))}
            }

            Dim result As Integer = 0
            ExecuteWithErrorHandling(Sub()
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

        Public Sub SaveActualUnit(actual As ActualUnitModel)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@ProjectID", actual.ProjectID},
                {"@RawUnitID", actual.RawUnitID},
                {"@ProductTypeID", actual.ProductTypeID},
                {"@UnitName", actual.UnitName},
                {"@PlanSQFT", actual.PlanSQFT},
                {"@UnitType", actual.UnitType},
                {"@OptionalAdder", actual.OptionalAdder}
            }

            ExecuteWithErrorHandling(Sub()
                                         If actual.ActualUnitID = 0 Then
                                             Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertActualUnit, BuildParameters(paramsDict))
                                             actual.ActualUnitID = CInt(newIDObj)
                                         Else
                                             paramsDict.Add("@ActualUnitID", actual.ActualUnitID)
                                             SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateActualUnit, BuildParameters(paramsDict))
                                         End If
                                     End Sub, "Error saving actual unit")
        End Sub

        Public Sub SaveActualToLevelMapping(mapping As ActualToLevelMappingModel)
            Dim paramsDict As New Dictionary(Of String, Object) From {
                {"@ProjectID", mapping.ProjectID},
                {"@ActualUnitID", mapping.ActualUnitID},
                {"@LevelID", mapping.LevelID},
                {"@Quantity", mapping.Quantity}
            }

            ExecuteWithErrorHandling(Sub()
                                         If mapping.MappingID = 0 Then
                                             Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertActualToLevelMapping, BuildParameters(paramsDict))
                                             If newIDObj IsNot Nothing Then mapping.MappingID = CInt(newIDObj)
                                         Else
                                             paramsDict.Add("@MappingID", mapping.MappingID)
                                             SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateActualToLevelMapping, BuildParameters(paramsDict))
                                         End If
                                     End Sub, "Error saving actual to level mapping")
        End Sub

        Public Function GetActualToLevelMappingsByLevelID(levelID As Integer) As List(Of ActualToLevelMappingModel)
            Dim mappings As New List(Of ActualToLevelMappingModel)
            Dim params As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectActualToLevelMappingsByLevelID, params)
                                             While reader.Read()
                                                 Dim mapping As New ActualToLevelMappingModel With {
                                                     .MappingID = reader.GetInt32(reader.GetOrdinal("MappingID")),
                                                     .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                                                     .ActualUnitID = reader.GetInt32(reader.GetOrdinal("ActualUnitID")),
                                                     .levelID = levelID,
                                                     .Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                                 }
                                                 mapping.ActualUnit = GetActualUnitByID(mapping.ActualUnitID)
                                                 mappings.Add(mapping)
                                             End While
                                         End Using
                                     End Sub, "Error loading mappings for level " & levelID)
            Return mappings
        End Function

        Public Function GetActualUnitsByProject(projectID As Integer) As List(Of ActualUnitModel)
            Dim actualUnits As New List(Of ActualUnitModel)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectActualUnitsByProject, params)
                                             While reader.Read()
                                                 Dim actual As New ActualUnitModel With {
                                                     .ActualUnitID = reader.GetInt32(reader.GetOrdinal("ActualUnitID")),
                                                     .UnitName = reader.GetString(reader.GetOrdinal("UnitName")),
                                                     .PlanSQFT = reader.GetDecimal(reader.GetOrdinal("PlanSQFT")),
                                                     .UnitType = reader.GetString(reader.GetOrdinal("UnitType")),
                                                     .OptionalAdder = reader.GetDecimal(reader.GetOrdinal("OptionalAdder")),
                                                     .RawUnitID = reader.GetInt32(reader.GetOrdinal("RawUnitID")),
                                                     .projectID = projectID,
                                                     .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID"))
                                                 }
                                                 actual.CalculatedComponents = GetCalculatedComponentsByActualUnitID(actual.ActualUnitID)
                                                 actualUnits.Add(actual)
                                             End While
                                         End Using
                                     End Sub, "Error loading actual units for project " & projectID)
            Return actualUnits
        End Function

        Public Function GetActualUnitByID(actualUnitID As Integer) As ActualUnitModel
            Dim unit As ActualUnitModel = Nothing
            Dim params As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectActualUnitByID, params)
                                             If reader.Read() Then
                                                 unit = New ActualUnitModel With {
                                                     .actualUnitID = actualUnitID,
                                                     .UnitName = If(Not reader.IsDBNull(reader.GetOrdinal("UnitName")), reader.GetString(reader.GetOrdinal("UnitName")), String.Empty),
                                                     .PlanSQFT = reader.GetDecimal(reader.GetOrdinal("PlanSQFT")),
                                                     .UnitType = If(Not reader.IsDBNull(reader.GetOrdinal("UnitType")), reader.GetString(reader.GetOrdinal("UnitType")), String.Empty),
                                                     .OptionalAdder = reader.GetDecimal(reader.GetOrdinal("OptionalAdder")),
                                                     .RawUnitID = reader.GetInt32(reader.GetOrdinal("RawUnitID")),
                                                     .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                                                     .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                                                     .ReferencedRawUnitName = If(Not reader.IsDBNull(reader.GetOrdinal("ReferencedRawUnitName")), reader.GetString(reader.GetOrdinal("ReferencedRawUnitName")), String.Empty),
                                                     .CalculatedComponents = GetCalculatedComponentsByActualUnitID(actualUnitID)
                                                 }
                                             End If
                                         End Using
                                     End Sub, "Error loading actual unit " & actualUnitID)
            Return unit
        End Function

        Private Function GetCalculatedComponentsByActualUnitID(actualUnitID As Integer) As List(Of CalculatedComponentModel)
            Dim components As New List(Of CalculatedComponentModel)
            Dim params As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}
            ExecuteWithErrorHandling(Sub()
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

        Public Sub SaveCalculatedComponents(actualUnit As ActualUnitModel)
            ExecuteWithErrorHandling(Sub()
                                         SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteCalculatedComponentsByActualUnitID, {New SqlParameter("@ActualUnitID", actualUnit.ActualUnitID)})

                                         For Each comp In actualUnit.CalculatedComponents
                                             Dim insertParams As New Dictionary(Of String, Object) From {
                                                 {"@ProjectID", actualUnit.ProjectID},
                                                 {"@ActualUnitID", actualUnit.ActualUnitID},
                                                 {"@ComponentType", comp.ComponentType},
                                                 {"@Value", comp.Value}
                                             }
                                             SqlConnectionManager.Instance.ExecuteNonQuery(Queries.InsertCalculatedComponent, BuildParameters(insertParams))
                                         Next
                                     End Sub, "Error saving calculated components")
        End Sub

        Public Function GetRawUnitsByProjectID(projectID As Integer) As List(Of RawUnitModel)
            Dim rawUnits As New List(Of RawUnitModel)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectRawUnitsByProject, params)
                                             While reader.Read()
                                                 Dim rawUnit As New RawUnitModel With {
                                                     .RawUnitID = reader.GetInt32(reader.GetOrdinal("RawUnitID")),
                                                     .projectID = projectID,
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
                                     End Sub, "Error loading raw units for project " & projectID)
            Return rawUnits
        End Function

        Public Sub UpdateLevelRollups(levelID As Integer)
            Dim projectID As Integer = 0
            Dim productTypeID As Integer = 0
            Dim params As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader("SELECT ProjectID, ProductTypeID FROM Levels WHERE LevelID = @LevelID", params)
                                             If reader.Read() Then
                                                 projectID = reader.GetInt32(0)
                                                 productTypeID = reader.GetInt32(1)
                                             Else
                                                 Throw New ApplicationException("Level not found for ID " & levelID)
                                             End If
                                         End Using

                                         Dim overallSqft As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateOverallSQFT, params))
                                         Dim overallLF As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateOverallLF, params))
                                         Dim overallBDFT As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateOverallBDFT, params))
                                         Dim lumberCost As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateLumberCost, params))
                                         Dim plateCost As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculatePlateCost, params))
                                         Dim laborCost As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateLaborCost, params))
                                         Dim laborMH As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateLaborMH, params))
                                         Dim designCost As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateDesignCost, params))
                                         Dim mgmtCost As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateMGMTCost, params))
                                         Dim suppliesCost As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateJobSuppliesCost, params))
                                         Dim itemsCost As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateItemsCost, params))
                                         Dim deliveryCost As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateDeliveryCost, params))
                                         Dim overallCost As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateOverallCost, params))
                                         Dim marginPercent As Decimal = GetMarginPercent(projectID, productTypeID)
                                         Dim overallPrice As Decimal = If(marginPercent >= 1D, overallCost, overallCost / (1D - marginPercent)) + deliveryCost
                                         Dim commonSqft As Decimal = 0 ' Retrieve from Levels if applicable
                                         Dim totalSqft As Decimal = overallSqft + commonSqft
                                         Dim avgPricePerSqft As Decimal = If(totalSqft > 0D, overallPrice / totalSqft, 0D)

                                         Dim updateParams As New Dictionary(Of String, Object) From {
                                             {"@LevelID", levelID},
                                             {"@OverallSQFT", overallSqft},
                                             {"@OverallLF", overallLF},
                                             {"@OverallBDFT", overallBDFT},
                                             {"@LumberCost", lumberCost},
                                             {"@PlateCost", plateCost},
                                             {"@LaborCost", laborCost},
                                             {"@LaborMH", laborMH},
                                             {"@DesignCost", designCost},
                                             {"@MGMTCost", mgmtCost},
                                             {"@JobSuppliesCost", suppliesCost},
                                             {"@ItemsCost", itemsCost},
                                             {"@DeliveryCost", deliveryCost},
                                             {"@OverallCost", overallCost},
                                             {"@OverallPrice", overallPrice},
                                             {"@TotalSQFT", totalSqft},
                                             {"@AvgPricePerSQFT", avgPricePerSqft}
                                         }
                                         SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLevelRollupsSql, BuildParameters(updateParams))
                                     End Sub, "Error updating level rollups for ID " & levelID)
        End Sub

        Public Sub UpdateBuildingRollups(buildingID As Integer)
            Dim bldgQty As Integer = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectBldgQty, {New SqlParameter("@BuildingID", buildingID)}))
            Dim projectID As Integer = 0
            ExecuteWithErrorHandling(Sub()
                                         Dim levelInfoParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader("SELECT DISTINCT ProjectID FROM Levels WHERE BuildingID = @BuildingID", levelInfoParams)
                                             If reader.Read() Then projectID = reader.GetInt32(0)
                                         End Using

                                         Dim floorCostPerBldg As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateFloorCostPerBldg, levelInfoParams))
                                         Dim floorDeliveryCost As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateFloorDeliveryCost, levelInfoParams))
                                         Dim roofCostPerBldg As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateRoofCostPerBldg, levelInfoParams))
                                         Dim roofDeliveryCost As Decimal = CDec(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateRoofDeliveryCost, levelInfoParams))
                                         Dim wallCostPerBldg As Decimal = 0D ' Stub for future
                                         Dim wallDeliveryCost As Decimal = 0D ' Stub

                                         Dim floorMargin As Decimal = GetMarginPercent(projectID, 1)
                                         Dim floorPricePerBldg As Decimal = If(floorMargin >= 1D, floorCostPerBldg, floorCostPerBldg / (1D - floorMargin)) + floorDeliveryCost
                                         Dim roofMargin As Decimal = GetMarginPercent(projectID, 2)
                                         Dim roofPricePerBldg As Decimal = If(roofMargin >= 1D, roofCostPerBldg, roofCostPerBldg / (1D - roofMargin)) + roofDeliveryCost
                                         Dim wallPricePerBldg As Decimal = wallCostPerBldg + wallDeliveryCost ' Stub

                                         Dim extendedFloorCost As Decimal = floorPricePerBldg * bldgQty
                                         Dim extendedRoofCost As Decimal = roofPricePerBldg * bldgQty
                                         Dim extendedWallCost As Decimal = wallPricePerBldg * bldgQty

                                         Dim overallCost As Decimal = floorCostPerBldg + roofCostPerBldg + wallCostPerBldg
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

        Public Sub DeleteLevel(levelID As Integer)
            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    ExecuteWithErrorHandling(Sub()
                                                 ' Step 1: Get ActualUnitIDs for the level
                                                 Dim actualUnitIDs As New List(Of Integer)
                                                 Using cmd As New SqlCommand(Queries.SelectActualUnitIDsByLevelID, conn, transaction)
                                                     cmd.Parameters.AddWithValue("@LevelID", levelID)
                                                     Using reader As SqlDataReader = cmd.ExecuteReader()
                                                         While reader.Read()
                                                             actualUnitIDs.Add(reader.GetInt32(0))
                                                         End While
                                                     End Using
                                                 End Using

                                                 ' Step 2: Delete ActualToLevelMapping
                                                 Using cmd As New SqlCommand(Queries.DeleteActualToLevelMappingByLevelID, conn, transaction)
                                                     cmd.Parameters.AddWithValue("@LevelID", levelID)
                                                     cmd.ExecuteNonQuery()
                                                 End Using

                                                 ' Step 3: Check and delete orphaned ActualUnits
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

                                                 ' Step 4: Delete the level
                                                 Using cmd As New SqlCommand(Queries.DeleteLevel, conn, transaction)
                                                     cmd.Parameters.AddWithValue("@LevelID", levelID)
                                                     cmd.ExecuteNonQuery()
                                                 End Using

                                                 transaction.Commit()
                                             End Sub, "Error deleting level " & levelID)
                End Using
            End Using
        End Sub

        Public Sub DeleteBuilding(buildingID As Integer)
            ExecuteWithErrorHandling(Sub()
                                         Dim levels As List(Of LevelModel) = GetLevelsByBuildingID(buildingID)
                                         For Each level In levels
                                             DeleteLevel(level.LevelID)
                                         Next
                                         SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteBuilding, {New SqlParameter("@BuildingID", buildingID)})
                                     End Sub, "Error deleting building " & buildingID)
        End Sub

        Public Sub DeleteActualToLevelMapping(mappingID As Integer)
            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    ExecuteWithErrorHandling(Sub()
                                                 ExecuteNonQueryTransactional(Queries.DeleteActualToLevelMappingByMappingID, {New SqlParameter("@MappingID", mappingID)}, conn, transaction)
                                                 transaction.Commit()
                                             End Sub, "Error deleting mapping")
                End Using
            End Using
        End Sub

        Public Function GetConfigValue(configKey As String) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@ConfigKey", configKey)}
            Dim valObj As Object = Nothing
            ExecuteWithErrorHandling(Sub()
                                         valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectConfigValue, params)
                                     End Sub, "Error fetching config value for " & configKey)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        Public Function GetMilesToJobSite(projectID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Dim valObj As Object = Nothing
            ExecuteWithErrorHandling(Sub()
                                         valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectMilesToJobSite, params)
                                     End Sub, "Error fetching miles for project " & projectID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0, CInt(valObj))
        End Function

        Public Function GetLumberAdder(projectID As Integer, productTypeID As Integer) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID), New SqlParameter("@ProductTypeID", productTypeID)}
            Dim valObj As Object = Nothing
            ExecuteWithErrorHandling(Sub()
                                         valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectLumberAdder, params)
                                     End Sub, "Error fetching lumber adder for project " & projectID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        Public Function GetMarginPercent(projectID As Integer, productTypeID As Integer) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID), New SqlParameter("@ProductTypeID", productTypeID)}
            Dim valObj As Object = Nothing
            ExecuteWithErrorHandling(Sub()
                                         valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectMarginPercent, params)
                                     End Sub, "Error fetching margin percent for project " & projectID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        Public Function GetRawUnitByID(rawUnitID As Integer) As RawUnitModel
            Dim raw As RawUnitModel = Nothing
            Dim params As SqlParameter() = {New SqlParameter("@RawUnitID", rawUnitID)}
            ExecuteWithErrorHandling(Sub()
                                         Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectRawUnitByID, params)
                                             If reader.Read() Then
                                                 raw = New RawUnitModel With {
                                                     .rawUnitID = rawUnitID,
                                                     .OverallCost = If(Not reader.IsDBNull(reader.GetOrdinal("OverallCost")), reader.GetDecimal(reader.GetOrdinal("OverallCost")), 0D),
                                                     .TotalSellPrice = If(Not reader.IsDBNull(reader.GetOrdinal("TotalSellPrice")), reader.GetDecimal(reader.GetOrdinal("TotalSellPrice")), 0D)
                                                 }
                                             End If
                                         End Using
                                     End Sub, "Error loading raw unit " & rawUnitID)
            Return raw
        End Function

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
            ExecuteWithErrorHandling(Sub()
                                         SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLevelRollupsSql, BuildParameters(paramsDict))
                                     End Sub, "Error updating level rollups")
        End Sub
    End Class
End Namespace