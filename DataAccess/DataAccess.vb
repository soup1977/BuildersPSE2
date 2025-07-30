Option Strict On
Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.BuildersPSE.Utilities

Namespace BuildersPSE.DataAccess
    Public Class DataAccess
        Public Sub ImportRawUnits(projectID As Integer, csvPath As String, productTypeID As Integer)
            Try
                Using parser As New Microsoft.VisualBasic.FileIO.TextFieldParser(csvPath)
                    parser.TextFieldType = Microsoft.VisualBasic.FileIO.FieldType.Delimited
                    parser.Delimiters = New String() {","}
                    parser.HasFieldsEnclosedInQuotes = True
                    parser.TrimWhiteSpace = True

                    If Not parser.EndOfData Then
                        Dim headers As String() = parser.ReadFields() ' First line as headers
                        Dim skippedHeaders As New HashSet(Of String) From {"JOBNUMBER", "PROJECT", "CUSTOMER", "JOBNAME", "STRUCTURENAME", "PLAN"}  ' Uppercase for comparison

                        While Not parser.EndOfData
                            Dim fields As String() = parser.ReadFields()
                            If fields.Length <> headers.Length Then
                                Continue While
                            End If

                            Dim rawUnit As New RawUnitModel With {
                        .ProjectID = projectID,  ' Set from parameter
                        .ProductTypeID = productTypeID  ' Default from parameter; override below if CSV has "Product"
                    }

                            For i As Integer = 0 To headers.Length - 1
                                Dim header As String = headers(i).Trim().ToUpper()  ' Normalize for matching
                                Dim valueStr As String = fields(i).Trim()

                                If skippedHeaders.Contains(header) Then
                                    Continue For  ' Skip processing this column entirely
                                End If

                                If header = "ELEVATION" Then
                                    rawUnit.RawUnitName = valueStr
                                ElseIf header = "PRODUCT" Then
                                    ' Determine ProductTypeID from CSV's Product column
                                    If String.Equals(valueStr, "Floor", StringComparison.OrdinalIgnoreCase) Then
                                        rawUnit.ProductTypeID = 1
                                    ElseIf String.Equals(valueStr, "Roof", StringComparison.OrdinalIgnoreCase) Then
                                        rawUnit.ProductTypeID = 2
                                    End If  ' Else, keep default from parameter
                                Else
                                    Dim tempVal As Decimal
                                    If Decimal.TryParse(valueStr, tempVal) Then
                                        Dim val As Decimal? = tempVal  ' Assign parsed value to nullable
                                        Select Case header
                                            Case "BF"
                                                rawUnit.BF = val
                                            Case "LF"
                                                rawUnit.LF = val
                                            Case "EWPLF"
                                                rawUnit.EWPLF = val
                                            Case "SQFT"
                                                rawUnit.SqFt = val
                                            Case "FCAREA"
                                                rawUnit.FCArea = val
                                            Case "LUMBERCOST"
                                                rawUnit.LumberCost = val
                                            Case "PLATECOST"
                                                rawUnit.PlateCost = val
                                            Case "MANUFLABORCOST"
                                                rawUnit.ManufLaborCost = val
                                            Case "DESIGNLABOR"
                                                rawUnit.DesignLabor = val
                                            Case "MGMTLABOR"
                                                rawUnit.MGMTLabor = val
                                            Case "JOBSUPPLIESCOST"
                                                rawUnit.JobSuppliesCost = val
                                            Case "MANHOURS"
                                                rawUnit.ManHours = val
                                            Case "ITEMCOST"
                                                rawUnit.ItemCost = val
                                            Case "OVERALLCOST"
                                                rawUnit.OverallCost = val
                                            Case "DELIVERYCOST"
                                                rawUnit.DeliveryCost = val
                                            Case "TOTALSELLPRICE"
                                                rawUnit.TotalSellPrice = val
                                            Case "AVGSPFNO2"
                                                rawUnit.AvgSPFNo2 = val
                                            Case Else
                                                rawUnit.Fields.Add(header, tempVal)  ' For unknown headers; extensibility
                                        End Select
                                    End If
                                End If
                            Next

                            ' Insert RawUnit base, get ID (now with all fields)
                            Dim insertParams As SqlParameter() = {
                        New SqlParameter("@RawUnitName", If(String.IsNullOrEmpty(rawUnit.RawUnitName), DBNull.Value, CType(rawUnit.RawUnitName, Object))),
                        New SqlParameter("@ProjectID", rawUnit.ProjectID),
                        New SqlParameter("@ProductTypeID", rawUnit.ProductTypeID),
                        New SqlParameter("@BF", If(rawUnit.BF.HasValue, CType(rawUnit.BF.Value, Object), DBNull.Value)),
                        New SqlParameter("@LF", If(rawUnit.LF.HasValue, CType(rawUnit.LF.Value, Object), DBNull.Value)),
                        New SqlParameter("@EWPLF", If(rawUnit.EWPLF.HasValue, CType(rawUnit.EWPLF.Value, Object), DBNull.Value)),
                        New SqlParameter("@SqFt", If(rawUnit.SqFt.HasValue, CType(rawUnit.SqFt.Value, Object), DBNull.Value)),
                        New SqlParameter("@FCArea", If(rawUnit.FCArea.HasValue, CType(rawUnit.FCArea.Value, Object), DBNull.Value)),
                        New SqlParameter("@LumberCost", If(rawUnit.LumberCost.HasValue, CType(rawUnit.LumberCost.Value, Object), DBNull.Value)),
                        New SqlParameter("@PlateCost", If(rawUnit.PlateCost.HasValue, CType(rawUnit.PlateCost.Value, Object), DBNull.Value)),
                        New SqlParameter("@ManufLaborCost", If(rawUnit.ManufLaborCost.HasValue, CType(rawUnit.ManufLaborCost.Value, Object), DBNull.Value)),
                        New SqlParameter("@DesignLabor", If(rawUnit.DesignLabor.HasValue, CType(rawUnit.DesignLabor.Value, Object), DBNull.Value)),
                        New SqlParameter("@MGMTLabor", If(rawUnit.MGMTLabor.HasValue, CType(rawUnit.MGMTLabor.Value, Object), DBNull.Value)),
                        New SqlParameter("@JobSuppliesCost", If(rawUnit.JobSuppliesCost.HasValue, CType(rawUnit.JobSuppliesCost.Value, Object), DBNull.Value)),
                        New SqlParameter("@ManHours", If(rawUnit.ManHours.HasValue, CType(rawUnit.ManHours.Value, Object), DBNull.Value)),
                        New SqlParameter("@ItemCost", If(rawUnit.ItemCost.HasValue, CType(rawUnit.ItemCost.Value, Object), DBNull.Value)),
                        New SqlParameter("@OverallCost", If(rawUnit.OverallCost.HasValue, CType(rawUnit.OverallCost.Value, Object), DBNull.Value)),
                        New SqlParameter("@DeliveryCost", If(rawUnit.DeliveryCost.HasValue, CType(rawUnit.DeliveryCost.Value, Object), DBNull.Value)),
                        New SqlParameter("@TotalSellPrice", If(rawUnit.TotalSellPrice.HasValue, CType(rawUnit.TotalSellPrice.Value, Object), DBNull.Value)),
                        New SqlParameter("@AvgSPFNo2", If(rawUnit.AvgSPFNo2.HasValue, CType(rawUnit.AvgSPFNo2.Value, Object), DBNull.Value))
                    }
                            Dim rawIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertRawUnit, insertParams)
                            If rawIDObj Is DBNull.Value OrElse rawIDObj Is Nothing Then
                                Throw New Exception("Failed to insert RawUnit for " & rawUnit.RawUnitName)
                            End If
                            rawUnit.RawUnitID = CInt(rawIDObj)

                            ' Optional: Persist Fields dictionary if extra columns exist (e.g., to a child table RawUnitFields)
                        End While
                    End If
                End Using
            Catch ex As Exception
                Throw New ApplicationException("RawUnits import failed for project " & projectID & ": " & ex.Message)
            End Try
        End Sub

        ' Updated GetProjectList in DataAccess.vb
        Public Function GetProjectList() As List(Of ProjectModel)
            Dim projects As New List(Of ProjectModel)
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectList)
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
                                .BidDate = If(Not reader.IsDBNull(reader.GetOrdinal("BidDate")), reader.GetDateTime(reader.GetOrdinal("BidDate")), Nothing)
                                }
                        ' Load only primary for list view
                        proj.Customers = GetCustomersByProject(proj.ProjectID)
                        proj.Salespeople = GetSalesByProject(proj.ProjectID)
                        proj.LastModifiedDate = If(Not reader.IsDBNull(reader.GetOrdinal("LastModifiedDate")), reader.GetDateTime(reader.GetOrdinal("LastModifiedDate")), Nothing)
                        projects.Add(proj)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading project list: " & ex.Message)
            End Try
            Return projects
        End Function
        ' Updated GetProjectByID to load Customers and Salespeople
        Public Function GetProjectByID(projectID As Integer) As ProjectModel
            Dim proj As ProjectModel = Nothing
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Try
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
            Catch ex As Exception
                Throw New ApplicationException("Error loading project by ID " & projectID & ": " & ex.Message)
            End Try
            Return proj
        End Function

        ' Updated GetProjects in DataAccess.vb
        Public Function GetProjects() As List(Of ProjectModel)
            Dim projects As New List(Of ProjectModel)
            Try
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
                        proj.Buildings = GetBuildingsByProjectID(proj.ProjectID)
                        proj.Settings = GetProjectProductSettings(proj.ProjectID)
                        proj.Customers = GetCustomersByProject(proj.ProjectID)
                        proj.Salespeople = GetSalesByProject(proj.ProjectID)
                        projects.Add(proj)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading projects: " & ex.Message)
            End Try
            Return projects
        End Function

        Public Function GetLevelsByBuildingID(buildingID As Integer) As List(Of LevelModel)
            Dim levels As New List(Of LevelModel)
            Dim params As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectLevelsByBuilding, params)
                    While reader.Read()
                        Dim level As New LevelModel With {
                    .LevelID = reader.GetInt32(reader.GetOrdinal("LevelID")),
                    .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
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
                    .CommonSQFT = If(Not reader.IsDBNull(reader.GetOrdinal("CommonSQFT")), reader.GetDecimal(reader.GetOrdinal("CommonSQFT")), Nothing)  ' If exists
                }
                        ' Load mappings if not already (e.g., level.ActualUnitMappings = GetActualToLevelMappingsByLevelID(level.LevelID))
                        levels.Add(level)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading levels for building " & buildingID & ": " & ex.Message)
            End Try
            Return levels
        End Function
        Public Function GetBuildingIDByLevelID(levelID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
            Try
                Dim buildingIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.GetBuildingIDByLevelID, params)
                Return If(buildingIDObj Is DBNull.Value OrElse buildingIDObj Is Nothing, 0, CInt(buildingIDObj))
            Catch ex As Exception
                Throw New ApplicationException("Error fetching BuildingID for LevelID " & levelID & ": " & ex.Message)
            End Try
        End Function
        Public Function GetBuildingsByProjectID(projectID As Integer) As List(Of BuildingModel)
            Dim buildings As New List(Of BuildingModel)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectBuildingsByProject, params)
                    While reader.Read()
                        Dim bldg As New BuildingModel With {
                    .BuildingID = reader.GetInt32(reader.GetOrdinal("BuildingID")),
                    .BuildingName = If(Not reader.IsDBNull(reader.GetOrdinal("BuildingName")), reader.GetString(reader.GetOrdinal("BuildingName")), String.Empty),
                    .BuildingType = If(Not reader.IsDBNull(reader.GetOrdinal("BuildingType")), reader.GetInt32(reader.GetOrdinal("BuildingType")), Nothing),
                    .ResUnits = If(Not reader.IsDBNull(reader.GetOrdinal("ResUnits")), reader.GetInt32(reader.GetOrdinal("ResUnits")), Nothing),
                    .BldgQty = reader.GetInt32(reader.GetOrdinal("BldgQty")),
                    .ProjectID = projectID,
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
            Catch ex As Exception
                Throw New ApplicationException("Error loading buildings for project " & projectID & ": " & ex.Message)
            End Try
            Return buildings
        End Function

        Public Function GetProductTypes() As List(Of ProductTypeModel)
            Dim types As New List(Of ProductTypeModel)
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProductTypes)
                    While reader.Read()
                        Dim pt As New ProductTypeModel With {
                        .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                        .ProductTypeName = If(Not reader.IsDBNull(reader.GetOrdinal("ProductTypeName")), reader.GetString(reader.GetOrdinal("ProductTypeName")), String.Empty)
                        }
                        types.Add(pt)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading product types: " & ex.Message)
            End Try
            Return types
        End Function

        Public Function GetProjectProductSettings(projectID As Integer) As List(Of ProjectProductSettingsModel)
            Dim settings As New List(Of ProjectProductSettingsModel)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectProjectProductSettings, params)
                    While reader.Read()
                        Dim setting As New ProjectProductSettingsModel With {
                        .SettingID = reader.GetInt32(reader.GetOrdinal("SettingID")),
                        .ProjectID = projectID,
                        .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                        .MarginPercent = If(Not reader.IsDBNull(reader.GetOrdinal("MarginPercent")), reader.GetDecimal(reader.GetOrdinal("MarginPercent")), Nothing),
                        .LumberAdder = If(Not reader.IsDBNull(reader.GetOrdinal("LumberAdder")), reader.GetDecimal(reader.GetOrdinal("LumberAdder")), Nothing)
                        }
                        settings.Add(setting)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading settings for project " & projectID & ": " & ex.Message)
            End Try
            Return settings
        End Function

        ' New helper methods
        Public Function GetCustomersByProject(projectID As Integer) As List(Of CustomerModel)
            Dim customers As New List(Of CustomerModel)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectCustomersByProject, params)
                    While reader.Read()
                        Dim customer As New CustomerModel With {
                            .CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                            .CustomerName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerName")), reader.GetString(reader.GetOrdinal("CustomerName")), String.Empty)
                        }
                        customers.Add(customer)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading customers for project " & projectID & ": " & ex.Message)
            End Try
            Return customers
        End Function
        Public Function GetSalesByProject(projectID As Integer) As List(Of SalesModel)
            Dim sales As New List(Of SalesModel)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectSalesByProject, params)
                    While reader.Read()
                        Dim sale As New SalesModel With {
                            .SalesID = reader.GetInt32(reader.GetOrdinal("SalesID")),
                            .SalesName = If(Not reader.IsDBNull(reader.GetOrdinal("SalesName")), reader.GetString(reader.GetOrdinal("SalesName")), String.Empty)
                        }
                        sales.Add(sale)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading sales for project " & projectID & ": " & ex.Message)
            End Try
            Return sales
        End Function

        ' Updated SaveProject to handle Customers and Salespeople mappings
        Public Sub SaveProject(proj As ProjectModel)
            Try
                Dim isNew As Boolean = proj.ProjectID = 0
                Dim parameters As SqlParameter() = {
                    New SqlParameter("@JBID", If(String.IsNullOrEmpty(proj.JBID), DBNull.Value, CType(proj.JBID, Object))),
                    New SqlParameter("@ProjectTypeID", proj.ProjectType.ProjectTypeID),
                    New SqlParameter("@ProjectName", If(String.IsNullOrEmpty(proj.ProjectName), DBNull.Value, CType(proj.ProjectName, Object))),
                    New SqlParameter("@EstimatorID", proj.Estimator.EstimatorID),
                    New SqlParameter("@Address", If(String.IsNullOrEmpty(proj.Address), DBNull.Value, CType(proj.Address, Object))),
                    New SqlParameter("@City", If(String.IsNullOrEmpty(proj.City), DBNull.Value, CType(proj.City, Object))),
                    New SqlParameter("@State", If(String.IsNullOrEmpty(proj.State), DBNull.Value, CType(proj.State, Object))),
                    New SqlParameter("@Zip", If(String.IsNullOrEmpty(proj.Zip), DBNull.Value, CType(proj.Zip, Object))),
                    New SqlParameter("@BidDate", If(proj.BidDate.HasValue, CType(proj.BidDate.Value, Object), DBNull.Value)),
                    New SqlParameter("@ArchPlansDated", If(proj.ArchPlansDated.HasValue, CType(proj.ArchPlansDated.Value, Object), DBNull.Value)),
                    New SqlParameter("@EngPlansDated", If(proj.EngPlansDated.HasValue, CType(proj.EngPlansDated.Value, Object), DBNull.Value)),
                    New SqlParameter("@MilesToJobSite", proj.MilesToJobSite),
                    New SqlParameter("@TotalNetSqft", proj.TotalNetSqft),
                    New SqlParameter("@TotalGrossSqft", proj.TotalGrossSqft),
                    New SqlParameter("@ProjectArchitect", If(String.IsNullOrEmpty(proj.ProjectArchitect), DBNull.Value, CType(proj.ProjectArchitect, Object))),
                    New SqlParameter("@ProjectEngineer", If(String.IsNullOrEmpty(proj.ProjectEngineer), DBNull.Value, CType(proj.ProjectEngineer, Object))),
                    New SqlParameter("@ProjectNotes", If(String.IsNullOrEmpty(proj.ProjectNotes), DBNull.Value, CType(proj.ProjectNotes, Object))),
                    New SqlParameter("@LastModifiedDate", Now())
                }

                If isNew Then
                    Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProject, parameters)
                    If newIDObj IsNot Nothing Then
                        proj.ProjectID = CInt(newIDObj)
                    End If
                Else
                    Dim updateParams As SqlParameter() = parameters.Concat({New SqlParameter("@ProjectID", proj.ProjectID)}).ToArray()
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProject, updateParams)
                End If

                ' Save Settings
                For Each setting In proj.Settings
                    SaveProjectProductSetting(setting, proj.ProjectID)
                Next

                ' Save Customers Mappings
                ' First, delete existing mappings
                Dim deleteCustParams As New SqlParameter("@ProjectID", proj.ProjectID)
                SqlConnectionManager.Instance.ExecuteNonQuery("DELETE FROM CustomertoProjectMapping WHERE ProjectID = @ProjectID", {deleteCustParams})
                ' Then insert new
                For Each customer In proj.Customers
                    Dim insertCustParams As SqlParameter() = {
                        New SqlParameter("@ProjectID", proj.ProjectID),
                        New SqlParameter("@CustomerID", customer.CustomerID)
                    }
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.InsertCustomerToProject, insertCustParams)
                Next

                ' Save Salespeople Mappings
                ' First, delete existing mappings
                Dim deleteSalesParams As New SqlParameter("@ProjectID", proj.ProjectID)
                SqlConnectionManager.Instance.ExecuteNonQuery("DELETE FROM SalestoProjectMapping WHERE ProjectID = @ProjectID", {deleteSalesParams})
                ' Then insert new
                For Each sale In proj.Salespeople
                    Dim insertSalesParams As SqlParameter() = {
                        New SqlParameter("@ProjectID", proj.ProjectID),
                        New SqlParameter("@SalesID", sale.SalesID)
                    }
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.InsertSalesToProject, insertSalesParams)
                Next

                ' Save Buildings
                For Each bldg In proj.Buildings
                    SaveBuilding(bldg, proj.ProjectID)
                Next
            Catch ex As Exception
                Throw New ApplicationException("Error saving project: " & ex.Message)
            End Try
        End Sub

        Public Sub SaveBuilding(bldg As BuildingModel, projectID As Integer)
            Dim params As SqlParameter() = {
            New SqlParameter("@BuildingName", If(String.IsNullOrEmpty(bldg.BuildingName), DBNull.Value, CType(bldg.BuildingName, Object))),
            New SqlParameter("@BuildingType", If(bldg.BuildingType.HasValue, CType(bldg.BuildingType.Value, Object), DBNull.Value)),
            New SqlParameter("@ResUnits", If(bldg.ResUnits.HasValue, CType(bldg.ResUnits.Value, Object), DBNull.Value)),
            New SqlParameter("@BldgQty", bldg.BldgQty),
            New SqlParameter("@ProjectID", projectID)
            }
            Try
                If bldg.BuildingID = 0 Then
                    bldg.BuildingID = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertBuilding, params))
                Else
                    Dim updateParams As SqlParameter() = params.Concat({New SqlParameter("@BuildingID", bldg.BuildingID)}).ToArray()
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateBuilding, updateParams)
                End If

                For Each level In bldg.Levels
                    SaveLevel(level, bldg.BuildingID, projectID)
                Next
            Catch ex As Exception
                Throw New ApplicationException("Error saving building: " & ex.Message)
            End Try
        End Sub

        Public Sub SaveLevel(level As LevelModel, buildingID As Integer, projectID As Integer)
            Dim params As SqlParameter() = {
                New SqlParameter("@ProjectID", projectID),
                New SqlParameter("@BuildingID", buildingID),
                New SqlParameter("@ProductTypeID", level.ProductTypeID),  ' Updated: Direct pass (non-nullable Integer)
                New SqlParameter("@LevelNumber", level.LevelNumber),
                New SqlParameter("@LevelName", level.LevelName)
            }
            Try
                If level.LevelID = 0 Then
                    level.LevelID = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertLevel, params))
                Else
                    Dim updateParams As SqlParameter() = params.Concat({New SqlParameter("@LevelID", level.LevelID)}).ToArray()
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLevel, updateParams)
                End If
            Catch ex As Exception
                Throw New ApplicationException("Error saving level: " & ex.Message)
            End Try
        End Sub

        Public Sub SaveProjectProductSetting(setting As ProjectProductSettingsModel, projectID As Integer)
            Try
                Dim params As SqlParameter() = {
                New SqlParameter("@ProjectID", projectID),
                New SqlParameter("@ProductTypeID", setting.ProductTypeID),
                New SqlParameter("@MarginPercent", If(setting.MarginPercent.HasValue, CType(setting.MarginPercent.Value, Object), DBNull.Value)),
                New SqlParameter("@LumberAdder", If(setting.LumberAdder.HasValue AndAlso setting.LumberAdder.Value > 0, CType(setting.LumberAdder.Value, Object), DBNull.Value))
                }

                If setting.SettingID = 0 Then
                    Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectProductSetting, params)
                    If newIDObj IsNot Nothing Then
                        setting.SettingID = CInt(newIDObj)
                    End If
                Else
                    Dim updateParams As SqlParameter() = params.Concat({New SqlParameter("@SettingID", setting.SettingID)}).ToArray()
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectProductSetting, updateParams)
                End If
            Catch ex As Exception
                Throw New ApplicationException("Error saving project setting: " & ex.Message)
            End Try
        End Sub

        Public Function GetProjectTypes() As List(Of ProjectTypeModel)
            Dim types As New List(Of ProjectTypeModel)
            Try
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
            Catch ex As Exception
                Throw New ApplicationException("Error loading project types: " & ex.Message)
            End Try
            Return types
        End Function
        Public Function SaveProjectType(projectType As ProjectTypeModel) As Integer
            Dim params As SqlParameter() = {
                New SqlParameter("@ProjectTypeName", If(String.IsNullOrEmpty(projectType.ProjectTypeName), DBNull.Value, CType(projectType.ProjectTypeName, Object))),
                New SqlParameter("@Description", If(String.IsNullOrEmpty(projectType.Description), DBNull.Value, CType(projectType.Description, Object)))
            }
            Try
                If projectType.ProjectTypeID = 0 Then
                    Return CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertProjectType, params))
                Else
                    Dim updateParams As SqlParameter() = params.Concat({New SqlParameter("@ProjectTypeID", projectType.ProjectTypeID)}).ToArray()
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateProjectType, updateParams)
                    Return projectType.ProjectTypeID
                End If
            Catch ex As Exception
                Throw New ApplicationException("Error saving project type: " & ex.Message)
            End Try
        End Function

        ' New methods for Estimator
        Public Function GetEstimators() As List(Of EstimatorModel)
            Dim estimators As New List(Of EstimatorModel)
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectEstimators)
                    While reader.Read()
                        Dim estimator As New EstimatorModel With {
                            .EstimatorID = reader.GetInt32(reader.GetOrdinal("EstimatorID")),
                            .EstimatorName = If(Not reader.IsDBNull(reader.GetOrdinal("EstimatorName")), reader.GetString(reader.GetOrdinal("EstimatorName")), String.Empty)
                        }
                        estimators.Add(estimator)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading estimators: " & ex.Message)
            End Try
            Return estimators
        End Function
        Public Function SaveEstimator(estimator As EstimatorModel) As Integer
            Dim params As SqlParameter() = {
                New SqlParameter("@EstimatorName", If(String.IsNullOrEmpty(estimator.EstimatorName), DBNull.Value, CType(estimator.EstimatorName, Object)))
            }
            Try
                If estimator.EstimatorID = 0 Then
                    Return CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertEstimator, params))
                Else
                    Dim updateParams As SqlParameter() = params.Concat({New SqlParameter("@EstimatorID", estimator.EstimatorID)}).ToArray()
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateEstimator, updateParams)
                    Return estimator.EstimatorID
                End If
            Catch ex As Exception
                Throw New ApplicationException("Error saving estimator: " & ex.Message)
            End Try
        End Function

        ' New methods for Customers
        Public Function GetCustomers() As List(Of CustomerModel)
            Dim customers As New List(Of CustomerModel)
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectCustomers)
                    While reader.Read()
                        Dim customer As New CustomerModel With {
                            .CustomerID = reader.GetInt32(reader.GetOrdinal("CustomerID")),
                            .CustomerName = If(Not reader.IsDBNull(reader.GetOrdinal("CustomerName")), reader.GetString(reader.GetOrdinal("CustomerName")), String.Empty)
                        }
                        customers.Add(customer)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading customers: " & ex.Message)
            End Try
            Return customers
        End Function
        Public Function SaveCustomer(customer As CustomerModel) As Integer
            Dim params As SqlParameter() = {
                New SqlParameter("@CustomerName", If(String.IsNullOrEmpty(customer.CustomerName), DBNull.Value, CType(customer.CustomerName, Object)))
            }
            Try
                If customer.CustomerID = 0 Then
                    Return CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertCustomer, params))
                Else
                    Dim updateParams As SqlParameter() = params.Concat({New SqlParameter("@CustomerID", customer.CustomerID)}).ToArray()
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateCustomer, updateParams)
                    Return customer.CustomerID
                End If
            Catch ex As Exception
                Throw New ApplicationException("Error saving customer: " & ex.Message)
            End Try
        End Function

        ' New methods for Sales
        Public Function GetSales() As List(Of SalesModel)
            Dim sales As New List(Of SalesModel)
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectSales)
                    While reader.Read()
                        Dim sale As New SalesModel With {
                            .SalesID = reader.GetInt32(reader.GetOrdinal("SalesID")),
                            .SalesName = If(Not reader.IsDBNull(reader.GetOrdinal("SalesName")), reader.GetString(reader.GetOrdinal("SalesName")), String.Empty)
                        }
                        sales.Add(sale)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading sales: " & ex.Message)
            End Try
            Return sales
        End Function
        Public Function SaveSales(sale As SalesModel) As Integer
            Dim params As SqlParameter() = {
                New SqlParameter("@SalesName", If(String.IsNullOrEmpty(sale.SalesName), DBNull.Value, CType(sale.SalesName, Object)))
            }
            Try
                If sale.SalesID = 0 Then
                    Return CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertSales, params))
                Else
                    Dim updateParams As SqlParameter() = params.Concat({New SqlParameter("@SalesID", sale.SalesID)}).ToArray()
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateSales, updateParams)
                    Return sale.SalesID
                End If
            Catch ex As Exception
                Throw New ApplicationException("Error saving sales: " & ex.Message)
            End Try
        End Function

        ' Add SaveActualUnit
        Public Sub SaveActualUnit(actual As ActualUnitModel)
            Dim params As SqlParameter() = {
                New SqlParameter("@ProjectID", actual.ProjectID),
                New SqlParameter("@RawUnitID", actual.RawUnitID),
                New SqlParameter("@ProductTypeID", actual.ProductTypeID),
                New SqlParameter("@UnitName", actual.UnitName),
                New SqlParameter("@PlanSQFT", actual.PlanSQFT),
                New SqlParameter("@UnitType", actual.UnitType),
                New SqlParameter("@OptionalAdder", actual.OptionalAdder)
            }
            If actual.ActualUnitID = 0 Then
                actual.ActualUnitID = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertActualUnit, params))
            Else
                params = params.Concat({New SqlParameter("@ActualUnitID", actual.ActualUnitID)}).ToArray()
                SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateActualUnit, params)
            End If
        End Sub
        Public Sub SaveActualToLevelMapping(mapping As ActualToLevelMappingModel)
            Dim params As SqlParameter() = {
                New SqlParameter("@ProjectID", mapping.ProjectID),
                New SqlParameter("@ActualUnitID", mapping.ActualUnitID),
                New SqlParameter("@LevelID", mapping.LevelID),
                New SqlParameter("@Quantity", mapping.Quantity)
}
            Try
                If mapping.MappingID = 0 Then
                    Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertActualToLevelMapping, params)
                    If newIDObj IsNot Nothing Then
                        mapping.MappingID = CInt(newIDObj)
                    End If
                Else
                    Dim updateParams As SqlParameter() = params.Concat({New SqlParameter("@MappingID", mapping.MappingID)}).ToArray()
                    SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateActualToLevelMapping, updateParams)
                End If
            Catch ex As Exception
                Throw New ApplicationException("Error saving actual to level mapping: " & ex.Message)
            End Try
        End Sub

        Public Function GetActualToLevelMappingsByLevelID(levelID As Integer) As List(Of ActualToLevelMappingModel)
            Dim mappings As New List(Of ActualToLevelMappingModel)
            Dim params As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectActualToLevelMappingsByLevelID, params)
                    While reader.Read()
                        Dim mapping As New ActualToLevelMappingModel With {
                            .MappingID = reader.GetInt32(reader.GetOrdinal("MappingID")),
                            .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
                            .ActualUnitID = reader.GetInt32(reader.GetOrdinal("ActualUnitID")),
                            .LevelID = levelID,
                            .Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                        }
                        ' Fetch associated ActualUnit (or map here if joined)
                        mapping.ActualUnit = GetActualUnitByID(mapping.ActualUnitID)
                        mappings.Add(mapping)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading mappings for level " & levelID & ": " & ex.Message)
            End Try
            Return mappings
        End Function
        ' Add GetActualUnitsByProject
        Public Function GetActualUnitsByProject(projectID As Integer) As List(Of ActualUnitModel)
            Dim actualUnits As New List(Of ActualUnitModel)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectActualUnitsByProject, params)
                    While reader.Read()
                        Dim actual As New ActualUnitModel With {
                            .ActualUnitID = reader.GetInt32(reader.GetOrdinal("ActualUnitID")),
                            .UnitName = reader.GetString(reader.GetOrdinal("UnitName")),
                            .PlanSQFT = reader.GetDecimal(reader.GetOrdinal("PlanSQFT")),
                            .UnitType = reader.GetString(reader.GetOrdinal("UnitType")),
                            .OptionalAdder = reader.GetDecimal(reader.GetOrdinal("OptionalAdder")),
                            .RawUnitID = reader.GetInt32(reader.GetOrdinal("RawUnitID")),
                            .ProjectID = projectID,
                            .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID"))
                        }
                        actual.CalculatedComponents = GetCalculatedComponentsByActualUnitID(actual.ActualUnitID)
                        actualUnits.Add(actual)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading actual units for project " & projectID & ": " & ex.Message)
            End Try
            Return actualUnits
        End Function
        Public Function GetActualUnitByID(actualUnitID As Integer) As ActualUnitModel
            Dim unit As ActualUnitModel = Nothing
            Dim params As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectActualUnitByID, params)
                    If reader.Read() Then
                        unit = New ActualUnitModel With {
                            .ActualUnitID = actualUnitID,
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
            Catch ex As Exception
                Throw New ApplicationException("Error loading actual unit " & actualUnitID & ": " & ex.Message)
            End Try
            Return unit
        End Function

        ' Implement GetCalculatedComponentsByActualUnitID (stubbed)
        Private Function GetCalculatedComponentsByActualUnitID(actualUnitID As Integer) As List(Of CalculatedComponentModel)
            Dim components As New List(Of CalculatedComponentModel)
            ' Implement query to select from CalculatedComponents
            Dim params As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnitID)}
            ' Assume query: "SELECT * FROM CalculatedComponents WHERE ActualUnitID = @ActualUnitID"
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader("SELECT * FROM CalculatedComponents WHERE ActualUnitID = @ActualUnitID", params)
                    While reader.Read()
                        Dim comp As New CalculatedComponentModel With {
                            .ComponentID = reader.GetInt32(reader.GetOrdinal("ComponentID")),
                            .ComponentType = reader.GetString(reader.GetOrdinal("ComponentType")),
                            .Value = reader.GetDecimal(reader.GetOrdinal("Value"))
                        }
                        components.Add(comp)
                    End While
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error loading calculated components for actual unit " & actualUnitID & ": " & ex.Message)
            End Try
            Return components
        End Function

        Public Sub SaveCalculatedComponents(actualUnit As ActualUnitModel)
            Dim deleteParams As SqlParameter() = {New SqlParameter("@ActualUnitID", actualUnit.ActualUnitID)}
            SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteCalculatedComponentsByActualUnitID, deleteParams)

            For Each comp In actualUnit.CalculatedComponents
                Dim insertParams As SqlParameter() = {
            New SqlParameter("@ProjectID", actualUnit.ProjectID),
            New SqlParameter("@ActualUnitID", actualUnit.ActualUnitID),
            New SqlParameter("@ComponentType", comp.ComponentType),
            New SqlParameter("@Value", comp.Value)
        }
                SqlConnectionManager.Instance.ExecuteNonQuery(Queries.InsertCalculatedComponent, insertParams)
            Next
        End Sub

        ' Add GetRawUnitsByProjectID
        Public Function GetRawUnitsByProjectID(projectID As Integer) As List(Of RawUnitModel)
            Dim rawUnits As New List(Of RawUnitModel)
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Try
                Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectRawUnitsByProject, params)
                    While reader.Read()
                        Dim rawUnit As New RawUnitModel With {
                    .RawUnitID = reader.GetInt32(reader.GetOrdinal("RawUnitID")),
                    .ProjectID = projectID,
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
            Catch ex As Exception
                Throw New ApplicationException("Error loading raw units for project " & projectID & ": " & ex.Message)
            End Try
            Return rawUnits
        End Function
        ' Update rollups for a specific level (e.g., after unit assignment in frmPSE)
        Public Sub UpdateLevelRollups(levelID As Integer)
            Try
                ' Fetch ProjectID and ProductTypeID for the level
                Dim levelInfoQuery As String = "SELECT ProjectID, ProductTypeID FROM Levels WHERE LevelID = @LevelID"
                Dim levelParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                Using levelReader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(levelInfoQuery, levelParams)
                    If levelReader.Read() Then
                        Dim projectID As Integer = levelReader.GetInt32(0)
                        Dim productTypeID As Integer = levelReader.GetInt32(1)

                        ' OverallSQFT: SUM(au.PlanSQFT * alm.Quantity)
                        Dim sqftQuery As String = "
                    SELECT SUM(au.PlanSQFT * alm.Quantity) AS OverallSQFT
                    FROM ActualUnits au JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID
                    WHERE alm.LevelID = @LevelID"
                        Dim sqftParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim overallSqftObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(sqftQuery, sqftParams)
                        Dim overallSqft As Decimal = If(overallSqftObj Is DBNull.Value, 0D, CDec(overallSqftObj))

                        ' OverallLF: SUM('LF/SQFT' * au.OptionalAdder * alm.Quantity * au.PlanSQFT)
                        Dim lfQuery As String = "
                    SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS OverallLF
                    FROM CalculatedComponents cc
                    JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID
                    JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID
                    WHERE cc.ComponentType = 'LF/SQFT' AND alm.LevelID = @LevelID"
                        Dim lfParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim overallLFObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(lfQuery, lfParams)
                        Dim overallLF As Decimal = If(overallLFObj Is DBNull.Value, 0D, CDec(overallLFObj))

                        ' OverallBDFT: SUM('BDFT/SQFT' * au.OptionalAdder * alm.Quantity * au.PlanSQFT)
                        Dim bdftQuery As String = Replace(lfQuery, "'LF/SQFT'", "'BDFT/SQFT'")
                        Dim bdftParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim overallBDFTObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(bdftQuery, bdftParams)
                        Dim overallBDFT As Decimal = If(overallBDFTObj Is DBNull.Value, 0D, CDec(overallBDFTObj))

                        ' LumberCost: Base + Adder
                        Dim lumberQuery As String = Replace(lfQuery, "'LF/SQFT'", "'Lumber/SQFT'")
                        Dim lumberParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim lumberBaseObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(lumberQuery, lumberParams)
                        Dim lumberBase As Decimal = If(lumberBaseObj Is DBNull.Value, 0D, CDec(lumberBaseObj))

                        ' Fetch LumberAdder from ProjectProductSettings
                        Dim adderParams As SqlParameter() = {
                    New SqlParameter("@ProjectID", projectID),
                    New SqlParameter("@ProductTypeID", productTypeID)
                }
                        Dim adderQuery As String = "SELECT LumberAdder FROM ProjectProductSettings WHERE ProjectID = @ProjectID AND ProductTypeID = @ProductTypeID"
                        Dim lumberAdderObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(adderQuery, adderParams)
                        Dim lumberAdder As Decimal = If(lumberAdderObj Is DBNull.Value OrElse CDec(lumberAdderObj) = 0D, 0D, CDec(lumberAdderObj))

                        Dim lumberCost As Decimal = lumberBase + (overallBDFT / 1000D) * lumberAdder

                        ' PlateCost: SUM('Plate/SQFT' * ...)
                        Dim plateQuery As String = Replace(lfQuery, "'LF/SQFT'", "'Plate/SQFT'")
                        Dim plateParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim plateCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(plateQuery, plateParams)
                        Dim plateCost As Decimal = If(plateCostObj Is DBNull.Value, 0D, CDec(plateCostObj))

                        ' LaborCost (ManufLaborCost): SUM('ManufLabor/SQFT' * ...)
                        Dim laborQuery As String = Replace(lfQuery, "'LF/SQFT'", "'ManufLabor/SQFT'")
                        Dim laborParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim laborCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(laborQuery, laborParams)
                        Dim laborCost As Decimal = If(laborCostObj Is DBNull.Value, 0D, CDec(laborCostObj))

                        ' LaborMH: SUM('ManHours/SQFT' * ...)
                        Dim laborMHQuery As String = Replace(lfQuery, "'LF/SQFT'", "'ManHours/SQFT'")
                        Dim laborMHParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim laborMHObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(laborMHQuery, laborMHParams)
                        Dim laborMH As Decimal = If(laborMHObj Is DBNull.Value, 0D, CDec(laborMHObj))

                        ' DesignCost: SUM('DesignLabor/SQFT' * ...)
                        Dim designQuery As String = Replace(lfQuery, "'LF/SQFT'", "'DesignLabor/SQFT'")
                        Dim designParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim designCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(designQuery, designParams)
                        Dim designCost As Decimal = If(designCostObj Is DBNull.Value, 0D, CDec(designCostObj))

                        ' MGMTCost: SUM('MGMTLabor/SQFT' * ...)
                        Dim mgmtQuery As String = Replace(lfQuery, "'LF/SQFT'", "'MGMTLabor/SQFT'")
                        Dim mgmtParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim mgmtCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(mgmtQuery, mgmtParams)
                        Dim mgmtCost As Decimal = If(mgmtCostObj Is DBNull.Value, 0D, CDec(mgmtCostObj))

                        ' JobSuppliesCost: SUM('JobSupplies/SQFT' * ...)
                        Dim suppliesQuery As String = Replace(lfQuery, "'LF/SQFT'", "'JobSupplies/SQFT'")
                        Dim suppliesParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim suppliesCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(suppliesQuery, suppliesParams)
                        Dim suppliesCost As Decimal = If(suppliesCostObj Is DBNull.Value, 0D, CDec(suppliesCostObj))

                        ' ItemsCost: SUM('ItemCost/SQFT' * ...)
                        Dim itemsQuery As String = Replace(lfQuery, "'LF/SQFT'", "'ItemCost/SQFT'")
                        Dim itemsParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim itemsCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(itemsQuery, itemsParams)
                        Dim itemsCost As Decimal = If(itemsCostObj Is DBNull.Value, 0D, CDec(itemsCostObj))

                        ' DeliveryCost: Using existing query
                        Dim deliveryParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim deliveryCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateDeliveryCost, deliveryParams)
                        Dim deliveryCost As Decimal = If(deliveryCostObj Is DBNull.Value, 0D, CDec(deliveryCostObj))

                        ' OverallCost: Sum of all costs
                        Dim overallCost As Decimal = lumberCost + plateCost + laborCost + designCost + mgmtCost + suppliesCost + itemsCost

                        ' OverallPrice: overallCost / (1 - MarginPercent), with margin from ProjectProductSettings
                        Dim marginParams As SqlParameter() = {
                    New SqlParameter("@ProjectID", projectID),
                    New SqlParameter("@ProductTypeID", productTypeID)
                }
                        Dim marginQuery As String = "SELECT MarginPercent FROM ProjectProductSettings WHERE ProjectID = @ProjectID AND ProductTypeID = @ProductTypeID"
                        Dim marginObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(marginQuery, marginParams)
                        Dim marginPercent As Decimal = If(marginObj Is DBNull.Value, 0D, CDec(marginObj))
                        Dim overallPrice As Decimal = If(marginPercent >= 1D, overallCost, overallCost / (1D - marginPercent)) + deliveryCost

                        ' Fetch CommonSQFT (user-entered, assume stored in Levels)
                        Dim commonSqftQuery As String = "SELECT CommonSQFT FROM Levels WHERE LevelID = @LevelID"
                        Dim commonParams As SqlParameter() = {New SqlParameter("@LevelID", levelID)}
                        Dim commonSqftObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(commonSqftQuery, commonParams)
                        Dim commonSqft As Decimal = If(commonSqftObj Is DBNull.Value, 0D, CDec(commonSqftObj))

                        ' TotalSQFT: OverallSQFT + CommonSQFT
                        Dim totalSqft As Decimal = overallSqft

                        ' AvgPricePerSQFT: OverallPrice / TotalSQFT if >0 else 0
                        Dim avgPricePerSqft As Decimal = If(totalSqft > 0D, overallPrice / totalSqft, 0D)

                        ' Update Levels table with all rollups
                        Dim updateQuery As String = "
                    UPDATE Levels SET
                    OverallSQFT = @OverallSQFT,
                    OverallLF = @OverallLF,
                    OverallBDFT = @OverallBDFT,
                    LumberCost = @LumberCost,
                    PlateCost = @PlateCost,
                    LaborCost = @LaborCost,
                    LaborMH = @LaborMH,
                    DesignCost = @DesignCost,
                    MGMTCost = @MGMTCost,
                    JobSuppliesCost = @JobSuppliesCost,
                    ItemsCost = @ItemsCost,
                    DeliveryCost = @DeliveryCost,
                    OverallCost = @OverallCost,
                    OverallPrice = @OverallPrice,
                    TotalSQFT = @TotalSQFT,
                    AvgPricePerSQFT = @AvgPricePerSQFT
                    WHERE LevelID = @LevelID"
                        Dim updateParams As SqlParameter() = {
                    New SqlParameter("@LevelID", levelID),
                    New SqlParameter("@OverallSQFT", overallSqft),
                    New SqlParameter("@OverallLF", overallLF),
                    New SqlParameter("@OverallBDFT", overallBDFT),
                    New SqlParameter("@LumberCost", lumberCost),
                    New SqlParameter("@PlateCost", plateCost),
                    New SqlParameter("@LaborCost", laborCost),
                    New SqlParameter("@LaborMH", laborMH),
                    New SqlParameter("@DesignCost", designCost),
                    New SqlParameter("@MGMTCost", mgmtCost),
                    New SqlParameter("@JobSuppliesCost", suppliesCost),
                    New SqlParameter("@ItemsCost", itemsCost),
                    New SqlParameter("@DeliveryCost", deliveryCost),
                    New SqlParameter("@OverallCost", overallCost),
                    New SqlParameter("@OverallPrice", overallPrice),
                    New SqlParameter("@TotalSQFT", totalSqft),
                    New SqlParameter("@AvgPricePerSQFT", avgPricePerSqft)
                }
                        SqlConnectionManager.Instance.ExecuteNonQuery(updateQuery, updateParams)
                    Else
                        Throw New ApplicationException("Level not found for ID " & levelID)
                    End If
                End Using
            Catch ex As Exception
                Throw New ApplicationException("Error updating level rollups for ID " & levelID & ": " & ex.Message)
            End Try
        End Sub

        ' Update rollups for a building (after level updates)
        ' In DataAccess.vb
        Public Sub UpdateBuildingRollups(buildingID As Integer)

            Try
                ' Fetch BldgQty
                Dim bldgQtyQuery As String = "SELECT BldgQty FROM Buildings WHERE BuildingID = @BuildingID"
                Dim bldgQtyParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                Dim bldgQtyObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(bldgQtyQuery, bldgQtyParams)
                Dim bldgQty As Integer = If(bldgQtyObj Is DBNull.Value, 1, CInt(bldgQtyObj))

                ' Fetch ProjectID and ProductTypeIDs for margin
                Dim levelInfoQuery As String = "SELECT ProjectID, ProductTypeID FROM Levels WHERE BuildingID = @BuildingID"
                Dim levelParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                Dim projectID As Integer = 0
                Dim productTypeIDs As New List(Of Integer)
                Using levelReader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(levelInfoQuery, levelParams)
                    While levelReader.Read()
                        projectID = levelReader.GetInt32(0)
                        productTypeIDs.Add(levelReader.GetInt32(1))
                    End While
                End Using

                ' FloorCostPerBldg: SUM(OverallPrice from levels with ProductTypeID=1, excluding DeliveryCost)
                Dim floorCostQuery As String = "
            SELECT SUM(l.OverallCost) AS FloorCostPerBldg
            FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"
                Dim floorCostParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                Dim floorCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(floorCostQuery, floorCostParams)
                Dim floorCostPerBldg As Decimal = If(floorCostObj Is DBNull.Value, 0D, CDec(floorCostObj))

                ' Floor DeliveryCost
                Dim floorDeliveryQuery As String = "
            SELECT SUM(l.DeliveryCost) AS FloorDeliveryCost
            FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"
                Dim floorDeliveryParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                Dim floorDeliveryObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(floorDeliveryQuery, floorDeliveryParams)
                Dim floorDeliveryCost As Decimal = If(floorDeliveryObj Is DBNull.Value, 0D, CDec(floorDeliveryObj))

                ' RoofCostPerBldg: SUM(OverallCost from levels with ProductTypeID=2)
                Dim roofCostQuery As String = Replace(floorCostQuery, "1", "2")
                Dim roofCostParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                Dim roofCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(roofCostQuery, roofCostParams)
                Dim roofCostPerBldg As Decimal = If(roofCostObj Is DBNull.Value, 0D, CDec(roofCostObj))

                ' Roof DeliveryCost
                Dim roofDeliveryQuery As String = Replace(floorDeliveryQuery, "1", "2")
                Dim roofDeliveryParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                Dim roofDeliveryObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(roofDeliveryQuery, roofDeliveryParams)
                Dim roofDeliveryCost As Decimal = If(roofDeliveryObj Is DBNull.Value, 0D, CDec(roofDeliveryObj))

                ' WallCostPerBldg: Stub as 0 (implement if ProductTypeID=3 exists)
                Dim wallCostPerBldg As Decimal = 0D
                Dim wallDeliveryCost As Decimal = 0D ' Stub

                ' Apply margin per ProductTypeID
                Dim floorMargin As Decimal = GetMarginPercent(projectID, 1)
                Dim avgRawMarginFloorQuery As String = ""
                If floorMargin = 0D Then
                    avgRawMarginFloorQuery = "
                SELECT AVG( (ru.TotalSellPrice - ru.OverallCost) / ru.TotalSellPrice )
                FROM RawUnits ru JOIN ActualUnits au ON ru.RawUnitID = au.RawUnitID
                JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID
                JOIN Levels l ON alm.LevelID = l.LevelID
                WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1 AND ru.TotalSellPrice > 0"
                    Dim avgObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(avgRawMarginFloorQuery, New SqlParameter() {New SqlParameter("@BuildingID", buildingID)})
                    floorMargin = If(avgObj Is DBNull.Value, 0D, CDec(avgObj))
                End If
                Dim floorPricePerBldg As Decimal = If(floorMargin >= 1D, floorCostPerBldg, floorCostPerBldg / (1D - floorMargin)) + floorDeliveryCost

                Dim roofMargin As Decimal = GetMarginPercent(projectID, 2)
                If roofMargin = 0D Then
                    Dim avgRawMarginRoofQuery As String = Replace(avgRawMarginFloorQuery, "1", "2")
                    Dim avgObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(avgRawMarginRoofQuery, New SqlParameter() {New SqlParameter("@BuildingID", buildingID)})
                    roofMargin = If(avgObj Is DBNull.Value, 0D, CDec(avgObj))
                End If
                Dim roofPricePerBldg As Decimal = If(roofMargin >= 1D, roofCostPerBldg, roofCostPerBldg / (1D - roofMargin)) + roofDeliveryCost

                Dim wallMargin As Decimal = 0D ' Stub
                Dim wallPricePerBldg As Decimal = wallCostPerBldg + wallDeliveryCost ' Stub

                ' Extended costs
                Dim extendedFloorCost As Decimal = floorPricePerBldg * bldgQty
                Dim extendedRoofCost As Decimal = roofPricePerBldg * bldgQty
                Dim extendedWallCost As Decimal = wallPricePerBldg * bldgQty

                ' OverallCost: Sum of costs EXCLUDING DeliveryCost
                Dim overallCost As Decimal = floorCostPerBldg + roofCostPerBldg + wallCostPerBldg
                Dim overallPrice As Decimal = floorPricePerBldg + roofPricePerBldg + wallPricePerBldg

                ' Update Buildings table
                Dim updateQuery As String = "
            UPDATE Buildings SET
            FloorCostPerBldg = @FloorCostPerBldg,
            RoofCostPerBldg = @RoofCostPerBldg,
            WallCostPerBldg = @WallCostPerBldg,
            ExtendedFloorCost = @ExtendedFloorCost,
            ExtendedRoofCost = @ExtendedRoofCost,
            ExtendedWallCost = @ExtendedWallCost,
            OverallPrice = @OverallPrice,
            OverallCost = @OverallCost
            WHERE BuildingID = @BuildingID"
                Dim updateParams As SqlParameter() = {
            New SqlParameter("@BuildingID", buildingID),
            New SqlParameter("@FloorCostPerBldg", floorPricePerBldg),
            New SqlParameter("@RoofCostPerBldg", roofPricePerBldg),
            New SqlParameter("@WallCostPerBldg", wallPricePerBldg),
            New SqlParameter("@ExtendedFloorCost", extendedFloorCost),
            New SqlParameter("@ExtendedRoofCost", extendedRoofCost),
            New SqlParameter("@ExtendedWallCost", extendedWallCost),
            New SqlParameter("@OverallPrice", overallPrice),
            New SqlParameter("@OverallCost", overallCost)
        }
                SqlConnectionManager.Instance.ExecuteNonQuery(updateQuery, updateParams)
            Catch ex As Exception
                Throw New ApplicationException("Error updating building rollups for ID " & buildingID & ": " & ex.Message)
            End Try
        End Sub
        Public Sub DeleteLevel(levelID As Integer)
            Dim transaction As SqlTransaction = Nothing
            Dim conn As SqlConnection = Nothing
            Try
                conn = New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                transaction = conn.BeginTransaction()

                ' Step 1: Get ActualUnitIDs for the level
                Dim actualUnitIDs As New List(Of Integer)
                Using cmd As New SqlCommand(Queries.SelectActualUnitIDsByLevelID, conn, transaction)
                    cmd.Parameters.AddWithValue("@LevelID", levelID)  ' Fresh parameter
                    Using reader As SqlDataReader = cmd.ExecuteReader()
                        While reader.Read()
                            actualUnitIDs.Add(reader.GetInt32(0))
                        End While
                    End Using
                End Using

                ' Step 2: Delete ActualToLevelMapping
                Using cmd As New SqlCommand(Queries.DeleteActualToLevelMappingByLevelID, conn, transaction)
                    cmd.Parameters.AddWithValue("@LevelID", levelID)  ' Fresh parameter
                    cmd.ExecuteNonQuery()
                End Using

                ' Step 3: Check and delete orphaned ActualUnits
                For Each auID In actualUnitIDs
                    Dim count As Integer
                    Using cmd As New SqlCommand(Queries.CountMappingsByActualUnitID, conn, transaction)
                        cmd.Parameters.AddWithValue("@ActualUnitID", auID)  ' Fresh parameter
                        count = CInt(cmd.ExecuteScalar())
                    End Using
                    If count = 0 Then
                        Using cmd As New SqlCommand(Queries.DeleteCalculatedComponentsByActualUnitID, conn, transaction)
                            cmd.Parameters.AddWithValue("@ActualUnitID", auID)  ' Fresh parameter
                            cmd.ExecuteNonQuery()
                        End Using
                        Using cmd As New SqlCommand(Queries.DeleteActualUnit, conn, transaction)
                            cmd.Parameters.AddWithValue("@ActualUnitID", auID)  ' Fresh parameter
                            cmd.ExecuteNonQuery()
                        End Using
                    End If
                Next

                ' Step 4: Delete the level
                Using cmd As New SqlCommand(Queries.DeleteLevel, conn, transaction)
                    cmd.Parameters.AddWithValue("@LevelID", levelID)  ' Fresh parameter
                    cmd.ExecuteNonQuery()
                End Using

                transaction.Commit()
            Catch ex As Exception
                If transaction IsNot Nothing Then transaction.Rollback()
                Throw New ApplicationException("Error deleting level " & levelID & ": " & ex.Message)
            Finally
                If conn IsNot Nothing AndAlso conn.State = ConnectionState.Open Then conn.Close()
            End Try
        End Sub
        Public Sub DeleteBuilding(buildingID As Integer)
            ' Fetch and delete all levels for this building first
            Dim levels As List(Of LevelModel) = GetLevelsByBuildingID(buildingID)
            For Each level In levels
                DeleteLevel(level.LevelID)
            Next
            ' Then delete the building
            Dim params As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
            SqlConnectionManager.Instance.ExecuteNonQuery(Queries.DeleteBuilding, params)
        End Sub

        Public Sub DeleteActualToLevelMapping(mappingID As Integer)
            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    Try
                        ' Delete the specific mapping using the transactional overload
                        Dim deleteMappingParams As SqlParameter() = {New SqlParameter("@MappingID", mappingID)}
                        SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeleteActualToLevelMappingByMappingID, deleteMappingParams, conn, transaction)

                        transaction.Commit()
                    Catch ex As Exception
                        transaction.Rollback()
                        Throw New ApplicationException("Error deleting mapping: " & ex.Message)
                    End Try
                End Using
            End Using
        End Sub
        Public Function GetConfigValue(configKey As String) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@ConfigKey", configKey)}
            Dim query As String = "SELECT Value FROM Configuration WHERE ConfigKey = @ConfigKey"
            Dim valObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(query, params)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        Public Function GetMilesToJobSite(projectID As Integer) As Integer
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID)}
            Dim query As String = "SELECT MilesToJobSite FROM Projects WHERE ProjectID = @ProjectID"
            Dim valObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(query, params)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0, CInt(valObj))
        End Function
        Public Function GetLumberAdder(projectID As Integer, productTypeID As Integer) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID), New SqlParameter("@ProductTypeID", productTypeID)}
            Dim query As String = "SELECT LumberAdder FROM ProjectProductSettings WHERE ProjectID = @ProjectID AND ProductTypeID = @ProductTypeID"
            Dim valObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(query, params)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        Public Function GetMarginPercent(projectID As Integer, productTypeID As Integer) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@ProjectID", projectID), New SqlParameter("@ProductTypeID", productTypeID)}
            Dim query As String = "SELECT MarginPercent FROM ProjectProductSettings WHERE ProjectID = @ProjectID AND ProductTypeID = @ProductTypeID"
            Dim valObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(query, params)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        Public Function GetRawUnitByID(rawUnitID As Integer) As RawUnitModel
            ' Implement similar to GetActualUnitByID, selecting from RawUnits
            Dim raw As RawUnitModel = Nothing
            Dim params As SqlParameter() = {New SqlParameter("@RawUnitID", rawUnitID)}
            Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectRawUnitByID, params) ' Add query to Queries
                If reader.Read() Then
                    raw = New RawUnitModel With {
                        .RawUnitID = rawUnitID,
                        .OverallCost = If(Not reader.IsDBNull(reader.GetOrdinal("OverallCost")), reader.GetDecimal(reader.GetOrdinal("OverallCost")), 0D),
                        .TotalSellPrice = If(Not reader.IsDBNull(reader.GetOrdinal("TotalSellPrice")), reader.GetDecimal(reader.GetOrdinal("TotalSellPrice")), 0D)
            }
                End If
            End Using
            Return raw
        End Function

        Public Sub SetLevelRollups(levelID As Integer, overallSqft As Decimal, overallLf As Decimal, overallBdft As Decimal, lumberCost As Decimal, plateCost As Decimal, laborCost As Decimal, laborMh As Decimal, designCost As Decimal, mgmtCost As Decimal, jobSuppliesCost As Decimal, itemsCost As Decimal, deliveryCost As Decimal, overallCost As Decimal, overallPrice As Decimal, totalSqft As Decimal, avgPricePerSqft As Decimal)
            Dim query As String = "
        UPDATE Levels SET
        OverallSQFT = @OverallSQFT,
        OverallLF = @OverallLF,
        OverallBDFT = @OverallBDFT,
        LumberCost = @LumberCost,
        PlateCost = @PlateCost,
        LaborCost = @LaborCost,
        LaborMH = @LaborMH,
        DesignCost = @DesignCost,
        MGMTCost = @MGMTCost,
        JobSuppliesCost = @JobSuppliesCost,
        ItemsCost = @ItemsCost,
        DeliveryCost = @DeliveryCost,
        OverallCost = @OverallCost,
        OverallPrice = @OverallPrice,
        TotalSQFT = @TotalSQFT,
        AvgPricePerSQFT = @AvgPricePerSQFT
        WHERE LevelID = @LevelID"
            Dim params As SqlParameter() = {
                New SqlParameter("@LevelID", levelID),
                New SqlParameter("@OverallSQFT", overallSqft),
                New SqlParameter("@OverallLF", overallLf),
                New SqlParameter("@OverallBDFT", overallBdft),
                New SqlParameter("@LumberCost", lumberCost),
                New SqlParameter("@PlateCost", plateCost),
                New SqlParameter("@LaborCost", laborCost),
                New SqlParameter("@LaborMH", laborMh),
                New SqlParameter("@DesignCost", designCost),
                New SqlParameter("@MGMTCost", mgmtCost),
                New SqlParameter("@JobSuppliesCost", jobSuppliesCost),
                New SqlParameter("@ItemsCost", itemsCost),
                New SqlParameter("@DeliveryCost", deliveryCost),
                New SqlParameter("@OverallCost", overallCost),
                New SqlParameter("@OverallPrice", overallPrice),
                New SqlParameter("@TotalSQFT", totalSqft),
                New SqlParameter("@AvgPricePerSQFT", avgPricePerSqft)
            }
            SqlConnectionManager.Instance.ExecuteNonQuery(query, params)
        End Sub


    End Class
End Namespace