Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.Utilities

Namespace DataAccess
    ''' <summary>
    ''' Data access for ProjVersionPriceHistory operations
    ''' </summary>
    Public Class PriceHistoryDataAccess

#Region "Duplicate Check"

        ''' <summary>
        ''' Checks if a price history record exists for the same project/version within the last X minutes
        ''' </summary>
        Public Shared Function GetRecentPriceHistory(projectID As Integer, versionID As Integer,
                                                      withinMinutes As Integer) As PriceHistoryModel
            Dim result As PriceHistoryModel = Nothing

            Dim sql As String = "
                SELECT TOP 1 *
                FROM ProjVersionPriceHistory
                WHERE ProjectID = @ProjectID 
                  AND VersionID = @VersionID
                  AND CreatedDate >= DATEADD(MINUTE, -@Minutes, GETDATE())
                ORDER BY CreatedDate DESC"

            Dim params As SqlParameter() = {
                New SqlParameter("@ProjectID", projectID),
                New SqlParameter("@VersionID", versionID),
                New SqlParameter("@Minutes", withinMinutes)
            }

            Using reader = SqlConnectionManager.Instance.ExecuteReader(sql, params)
                If reader.Read() Then
                    result = MapFromReader(reader)
                End If
            End Using

            Return result
        End Function

#End Region

#Region "Insert Operations"

        ''' <summary>
        ''' Saves a complete price history snapshot including buildings and levels
        ''' </summary>
        Public Shared Function SavePriceHistory(model As PriceHistoryModel) As Integer
            Dim newID As Integer = 0

            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using tran As SqlTransaction = conn.BeginTransaction()
                    Try
                        ' Insert main record
                        newID = InsertPriceHistory(model, conn, tran)
                        model.PriceHistoryID = newID

                        ' Insert buildings and levels
                        For Each bldg In model.Buildings
                            bldg.PriceHistoryID = newID
                            Dim bldgID = InsertPriceHistoryBuilding(bldg, conn, tran)
                            bldg.PriceHistoryBuildingID = bldgID

                            For Each level In bldg.Levels
                                level.PriceHistoryBuildingID = bldgID
                                InsertPriceHistoryLevel(level, conn, tran)
                            Next
                        Next

                        tran.Commit()
                    Catch
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using

            Return newID
        End Function

        ''' <summary>
        ''' Overwrites an existing price history record
        ''' </summary>
        Public Shared Sub OverwritePriceHistory(existingID As Integer, model As PriceHistoryModel)
            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using tran As SqlTransaction = conn.BeginTransaction()
                    Try
                        ' Delete existing (cascades to children)
                        Dim deleteCmd As New SqlCommand(
                            "DELETE FROM ProjVersionPriceHistory WHERE PriceHistoryID = @ID", conn, tran)
                        deleteCmd.Parameters.AddWithValue("@ID", existingID)
                        deleteCmd.ExecuteNonQuery()

                        ' Insert new
                        Dim newID = InsertPriceHistory(model, conn, tran)
                        model.PriceHistoryID = newID

                        For Each bldg In model.Buildings
                            bldg.PriceHistoryID = newID
                            Dim bldgID = InsertPriceHistoryBuilding(bldg, conn, tran)
                            bldg.PriceHistoryBuildingID = bldgID

                            For Each level In bldg.Levels
                                level.PriceHistoryBuildingID = bldgID
                                InsertPriceHistoryLevel(level, conn, tran)
                            Next
                        Next

                        tran.Commit()
                    Catch
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using
        End Sub

        Private Shared Function InsertPriceHistory(model As PriceHistoryModel,
                                                    conn As SqlConnection,
                                                    tran As SqlTransaction) As Integer
            Dim sql As String = "
                INSERT INTO ProjVersionPriceHistory (
                    ProjectID, VersionID, VersionName, CreatedDate, CreatedBy, ReportType,
                    JBID, ProjectName, Address, City, State, Zip, BidDate, MilesToJobSite,
                    TotalGrossSqftEntered, TotalNetSqftEntered,
                    ExtendedSell, ExtendedCost, ExtendedDelivery, ExtendedSqft, ExtendedBdft,
                    CalculatedGrossSqft, Margin, MarginWithDelivery, PricePerBdft, PricePerSqft,
                    FuturesAdderAmt, FuturesAdderProjTotal, FuturesContractMonth, 
                    FuturesPriorSettle, FuturesPullDate, LumberFutureID,
                    ActiveFloorSPFNo2, ActiveRoofSPFNo2, ActiveCostEffectiveID, ActiveCostEffectiveDate,
                    FloorMarginPercent, FloorLumberAdder, RoofMarginPercent, RoofLumberAdder,
                    WallMarginPercent, WallLumberAdder,
                    CustomerID, CustomerName, SalesID, SalesName, EstimatorID, EstimatorName,
                    TotalBuildingCount, TotalBldgQty, TotalLevelCount, TotalActualUnitCount, Notes
                )
                OUTPUT INSERTED.PriceHistoryID
                VALUES (
                    @ProjectID, @VersionID, @VersionName, GETDATE(), @CreatedBy, @ReportType,
                    @JBID, @ProjectName, @Address, @City, @State, @Zip, @BidDate, @MilesToJobSite,
                    @TotalGrossSqftEntered, @TotalNetSqftEntered,
                    @ExtendedSell, @ExtendedCost, @ExtendedDelivery, @ExtendedSqft, @ExtendedBdft,
                    @CalculatedGrossSqft, @Margin, @MarginWithDelivery, @PricePerBdft, @PricePerSqft,
                    @FuturesAdderAmt, @FuturesAdderProjTotal, @FuturesContractMonth,
                    @FuturesPriorSettle, @FuturesPullDate, @LumberFutureID,
                    @ActiveFloorSPFNo2, @ActiveRoofSPFNo2, @ActiveCostEffectiveID, @ActiveCostEffectiveDate,
                    @FloorMarginPercent, @FloorLumberAdder, @RoofMarginPercent, @RoofLumberAdder,
                    @WallMarginPercent, @WallLumberAdder,
                    @CustomerID, @CustomerName, @SalesID, @SalesName, @EstimatorID, @EstimatorName,
                    @TotalBuildingCount, @TotalBldgQty, @TotalLevelCount, @TotalActualUnitCount, @Notes
                )"

            Using cmd As New SqlCommand(sql, conn, tran)
                cmd.Parameters.AddWithValue("@ProjectID", model.ProjectID)
                cmd.Parameters.AddWithValue("@VersionID", model.VersionID)
                cmd.Parameters.AddWithValue("@VersionName", TableOperations.ToDb(model.VersionName))
                cmd.Parameters.AddWithValue("@CreatedBy", TableOperations.ToDb(model.CreatedBy))
                cmd.Parameters.AddWithValue("@ReportType", TableOperations.ToDb(model.ReportType))
                cmd.Parameters.AddWithValue("@JBID", TableOperations.ToDb(model.JBID))
                cmd.Parameters.AddWithValue("@ProjectName", TableOperations.ToDb(model.ProjectName))
                cmd.Parameters.AddWithValue("@Address", TableOperations.ToDb(model.Address))
                cmd.Parameters.AddWithValue("@City", TableOperations.ToDb(model.City))
                cmd.Parameters.AddWithValue("@State", TableOperations.ToDb(model.State))
                cmd.Parameters.AddWithValue("@Zip", TableOperations.ToDb(model.Zip))
                cmd.Parameters.AddWithValue("@BidDate", TableOperations.ToDb(model.BidDate))
                cmd.Parameters.AddWithValue("@MilesToJobSite", model.MilesToJobSite)
                cmd.Parameters.AddWithValue("@TotalGrossSqftEntered", TableOperations.ToDb(model.TotalGrossSqftEntered))
                cmd.Parameters.AddWithValue("@TotalNetSqftEntered", TableOperations.ToDb(model.TotalNetSqftEntered))
                cmd.Parameters.AddWithValue("@ExtendedSell", model.ExtendedSell)
                cmd.Parameters.AddWithValue("@ExtendedCost", model.ExtendedCost)
                cmd.Parameters.AddWithValue("@ExtendedDelivery", model.ExtendedDelivery)
                cmd.Parameters.AddWithValue("@ExtendedSqft", model.ExtendedSqft)
                cmd.Parameters.AddWithValue("@ExtendedBdft", model.ExtendedBdft)
                cmd.Parameters.AddWithValue("@CalculatedGrossSqft", model.CalculatedGrossSqft)
                cmd.Parameters.AddWithValue("@Margin", model.Margin)
                cmd.Parameters.AddWithValue("@MarginWithDelivery", model.MarginWithDelivery)
                cmd.Parameters.AddWithValue("@PricePerBdft", model.PricePerBdft)
                cmd.Parameters.AddWithValue("@PricePerSqft", model.PricePerSqft)
                cmd.Parameters.AddWithValue("@FuturesAdderAmt", TableOperations.ToDb(model.FuturesAdderAmt))
                cmd.Parameters.AddWithValue("@FuturesAdderProjTotal", TableOperations.ToDb(model.FuturesAdderProjTotal))
                cmd.Parameters.AddWithValue("@FuturesContractMonth", TableOperations.ToDb(model.FuturesContractMonth))
                cmd.Parameters.AddWithValue("@FuturesPriorSettle", TableOperations.ToDb(model.FuturesPriorSettle))
                cmd.Parameters.AddWithValue("@FuturesPullDate", TableOperations.ToDb(model.FuturesPullDate))
                cmd.Parameters.AddWithValue("@LumberFutureID", TableOperations.ToDb(model.LumberFutureID))
                cmd.Parameters.AddWithValue("@ActiveFloorSPFNo2", model.ActiveFloorSPFNo2)
                cmd.Parameters.AddWithValue("@ActiveRoofSPFNo2", model.ActiveRoofSPFNo2)
                cmd.Parameters.AddWithValue("@ActiveCostEffectiveID", TableOperations.ToDb(model.ActiveCostEffectiveID))
                cmd.Parameters.AddWithValue("@ActiveCostEffectiveDate", TableOperations.ToDb(model.ActiveCostEffectiveDate))
                cmd.Parameters.AddWithValue("@FloorMarginPercent", TableOperations.ToDb(model.FloorMarginPercent))
                cmd.Parameters.AddWithValue("@FloorLumberAdder", TableOperations.ToDb(model.FloorLumberAdder))
                cmd.Parameters.AddWithValue("@RoofMarginPercent", TableOperations.ToDb(model.RoofMarginPercent))
                cmd.Parameters.AddWithValue("@RoofLumberAdder", TableOperations.ToDb(model.RoofLumberAdder))
                cmd.Parameters.AddWithValue("@WallMarginPercent", TableOperations.ToDb(model.WallMarginPercent))
                cmd.Parameters.AddWithValue("@WallLumberAdder", TableOperations.ToDb(model.WallLumberAdder))
                cmd.Parameters.AddWithValue("@CustomerID", TableOperations.ToDb(model.CustomerID))
                cmd.Parameters.AddWithValue("@CustomerName", TableOperations.ToDb(model.CustomerName))
                cmd.Parameters.AddWithValue("@SalesID", TableOperations.ToDb(model.SalesID))
                cmd.Parameters.AddWithValue("@SalesName", TableOperations.ToDb(model.SalesName))
                cmd.Parameters.AddWithValue("@EstimatorID", TableOperations.ToDb(model.EstimatorID))
                cmd.Parameters.AddWithValue("@EstimatorName", TableOperations.ToDb(model.EstimatorName))
                cmd.Parameters.AddWithValue("@TotalBuildingCount", model.TotalBuildingCount)
                cmd.Parameters.AddWithValue("@TotalBldgQty", model.TotalBldgQty)
                cmd.Parameters.AddWithValue("@TotalLevelCount", model.TotalLevelCount)
                cmd.Parameters.AddWithValue("@TotalActualUnitCount", model.TotalActualUnitCount)
                cmd.Parameters.AddWithValue("@Notes", TableOperations.ToDb(model.Notes))

                Return CInt(cmd.ExecuteScalar())
            End Using
        End Function

        Private Shared Function InsertPriceHistoryBuilding(model As PriceHistoryBuildingModel,
                                                            conn As SqlConnection,
                                                            tran As SqlTransaction) As Integer
            Dim sql As String = "
                INSERT INTO ProjVersionPriceHistoryBuildings (
                    PriceHistoryID, BuildingID, BuildingName, BldgQty,
                    BaseSell, BaseCost, BaseDelivery, BaseSqft, BaseBdft,
                    ExtendedSell, ExtendedCost, ExtendedDelivery, ExtendedSqft, ExtendedBdft, Margin
                )
                OUTPUT INSERTED.PriceHistoryBuildingID
                VALUES (
                    @PriceHistoryID, @BuildingID, @BuildingName, @BldgQty,
                    @BaseSell, @BaseCost, @BaseDelivery, @BaseSqft, @BaseBdft,
                    @ExtendedSell, @ExtendedCost, @ExtendedDelivery, @ExtendedSqft, @ExtendedBdft, @Margin
                )"

            Using cmd As New SqlCommand(sql, conn, tran)
                cmd.Parameters.AddWithValue("@PriceHistoryID", model.PriceHistoryID)
                cmd.Parameters.AddWithValue("@BuildingID", model.BuildingID)
                cmd.Parameters.AddWithValue("@BuildingName", TableOperations.ToDb(model.BuildingName))
                cmd.Parameters.AddWithValue("@BldgQty", model.BldgQty)
                cmd.Parameters.AddWithValue("@BaseSell", model.BaseSell)
                cmd.Parameters.AddWithValue("@BaseCost", model.BaseCost)
                cmd.Parameters.AddWithValue("@BaseDelivery", model.BaseDelivery)
                cmd.Parameters.AddWithValue("@BaseSqft", model.BaseSqft)
                cmd.Parameters.AddWithValue("@BaseBdft", model.BaseBdft)
                cmd.Parameters.AddWithValue("@ExtendedSell", model.ExtendedSell)
                cmd.Parameters.AddWithValue("@ExtendedCost", model.ExtendedCost)
                cmd.Parameters.AddWithValue("@ExtendedDelivery", model.ExtendedDelivery)
                cmd.Parameters.AddWithValue("@ExtendedSqft", model.ExtendedSqft)
                cmd.Parameters.AddWithValue("@ExtendedBdft", model.ExtendedBdft)
                cmd.Parameters.AddWithValue("@Margin", model.Margin)

                Return CInt(cmd.ExecuteScalar())
            End Using
        End Function

        Private Shared Sub InsertPriceHistoryLevel(model As PriceHistoryLevelModel,
                                                    conn As SqlConnection,
                                                    tran As SqlTransaction)
            Dim sql As String = "
                INSERT INTO ProjVersionPriceHistoryLevels (
                    PriceHistoryBuildingID, LevelID, LevelName, LevelNumber, ProductTypeID, ProductTypeName,
                    OverallSqft, OverallLf, OverallBdft, LumberCost, PlateCost, LaborCost, LaborMH,
                    DesignCost, MgmtCost, JobSuppliesCost, ItemsCost, DeliveryCost,
                    OverallCost, OverallPrice, Margin, ActualUnitCount
                )
                VALUES (
                    @PriceHistoryBuildingID, @LevelID, @LevelName, @LevelNumber, @ProductTypeID, @ProductTypeName,
                    @OverallSqft, @OverallLf, @OverallBdft, @LumberCost, @PlateCost, @LaborCost, @LaborMH,
                    @DesignCost, @MgmtCost, @JobSuppliesCost, @ItemsCost, @DeliveryCost,
                    @OverallCost, @OverallPrice, @Margin, @ActualUnitCount
                )"

            Using cmd As New SqlCommand(sql, conn, tran)
                cmd.Parameters.AddWithValue("@PriceHistoryBuildingID", model.PriceHistoryBuildingID)
                cmd.Parameters.AddWithValue("@LevelID", model.LevelID)
                cmd.Parameters.AddWithValue("@LevelName", TableOperations.ToDb(model.LevelName))
                cmd.Parameters.AddWithValue("@LevelNumber", model.LevelNumber)
                cmd.Parameters.AddWithValue("@ProductTypeID", model.ProductTypeID)
                cmd.Parameters.AddWithValue("@ProductTypeName", TableOperations.ToDb(model.ProductTypeName))
                cmd.Parameters.AddWithValue("@OverallSqft", model.OverallSqft)
                cmd.Parameters.AddWithValue("@OverallLf", model.OverallLf)
                cmd.Parameters.AddWithValue("@OverallBdft", model.OverallBdft)
                cmd.Parameters.AddWithValue("@LumberCost", model.LumberCost)
                cmd.Parameters.AddWithValue("@PlateCost", model.PlateCost)
                cmd.Parameters.AddWithValue("@LaborCost", model.LaborCost)
                cmd.Parameters.AddWithValue("@LaborMH", model.LaborMH)
                cmd.Parameters.AddWithValue("@DesignCost", model.DesignCost)
                cmd.Parameters.AddWithValue("@MgmtCost", model.MgmtCost)
                cmd.Parameters.AddWithValue("@JobSuppliesCost", model.JobSuppliesCost)
                cmd.Parameters.AddWithValue("@ItemsCost", model.ItemsCost)
                cmd.Parameters.AddWithValue("@DeliveryCost", model.DeliveryCost)
                cmd.Parameters.AddWithValue("@OverallCost", model.OverallCost)
                cmd.Parameters.AddWithValue("@OverallPrice", model.OverallPrice)
                cmd.Parameters.AddWithValue("@Margin", model.Margin)
                cmd.Parameters.AddWithValue("@ActualUnitCount", model.ActualUnitCount)

                cmd.ExecuteNonQuery()
            End Using
        End Sub

#End Region

#Region "Read Operations"

        ''' <summary>
        ''' Gets price history records for a project
        ''' </summary>
        Public Shared Function GetPriceHistoryByProject(projectID As Integer) As List(Of PriceHistoryModel)
            Dim results As New List(Of PriceHistoryModel)

            Dim sql As String = "
                SELECT * FROM ProjVersionPriceHistory 
                WHERE ProjectID = @ProjectID 
                ORDER BY CreatedDate DESC"

            Using reader = SqlConnectionManager.Instance.ExecuteReader(sql,
                {New SqlParameter("@ProjectID", projectID)})
                While reader.Read()
                    results.Add(MapFromReader(reader))
                End While
            End Using

            Return results
        End Function

        Private Shared Function MapFromReader(reader As SqlDataReader) As PriceHistoryModel
            Return New PriceHistoryModel With {
        .PriceHistoryID = reader.GetInt32(reader.GetOrdinal("PriceHistoryID")),
        .ProjectID = reader.GetInt32(reader.GetOrdinal("ProjectID")),
        .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID")),
        .VersionName = If(reader.IsDBNull(reader.GetOrdinal("VersionName")), "", reader.GetString(reader.GetOrdinal("VersionName"))),
        .CreatedDate = reader.GetDateTime(reader.GetOrdinal("CreatedDate")),
        .CreatedBy = If(reader.IsDBNull(reader.GetOrdinal("CreatedBy")), "", reader.GetString(reader.GetOrdinal("CreatedBy"))),
        .ReportType = If(reader.IsDBNull(reader.GetOrdinal("ReportType")), "", reader.GetString(reader.GetOrdinal("ReportType"))),
            .JBID = If(reader.IsDBNull(reader.GetOrdinal("JBID")), "", reader.GetString(reader.GetOrdinal("JBID"))),
        .ProjectName = If(reader.IsDBNull(reader.GetOrdinal("ProjectName")), "", reader.GetString(reader.GetOrdinal("ProjectName"))),
        .Address = If(reader.IsDBNull(reader.GetOrdinal("Address")), "", reader.GetString(reader.GetOrdinal("Address"))),
        .City = If(reader.IsDBNull(reader.GetOrdinal("City")), "", reader.GetString(reader.GetOrdinal("City"))),
        .State = If(reader.IsDBNull(reader.GetOrdinal("State")), "", reader.GetString(reader.GetOrdinal("State"))),
        .Zip = If(reader.IsDBNull(reader.GetOrdinal("Zip")), "", reader.GetString(reader.GetOrdinal("Zip"))),
        .BidDate = If(reader.IsDBNull(reader.GetOrdinal("BidDate")), Nothing, CType(reader.GetDateTime(reader.GetOrdinal("BidDate")), DateTime?)),
        .MilesToJobSite = If(reader.IsDBNull(reader.GetOrdinal("MilesToJobSite")), 0, reader.GetInt32(reader.GetOrdinal("MilesToJobSite"))),
        .TotalGrossSqftEntered = If(reader.IsDBNull(reader.GetOrdinal("TotalGrossSqftEntered")), Nothing, CType(reader.GetInt32(reader.GetOrdinal("TotalGrossSqftEntered")), Integer?)),
        .TotalNetSqftEntered = If(reader.IsDBNull(reader.GetOrdinal("TotalNetSqftEntered")), Nothing, CType(reader.GetInt32(reader.GetOrdinal("TotalNetSqftEntered")), Integer?)),
            .ExtendedSell = If(reader.IsDBNull(reader.GetOrdinal("ExtendedSell")), 0D, reader.GetDecimal(reader.GetOrdinal("ExtendedSell"))),
        .ExtendedCost = If(reader.IsDBNull(reader.GetOrdinal("ExtendedCost")), 0D, reader.GetDecimal(reader.GetOrdinal("ExtendedCost"))),
        .ExtendedDelivery = If(reader.IsDBNull(reader.GetOrdinal("ExtendedDelivery")), 0D, reader.GetDecimal(reader.GetOrdinal("ExtendedDelivery"))),
        .ExtendedSqft = If(reader.IsDBNull(reader.GetOrdinal("ExtendedSqft")), 0D, reader.GetDecimal(reader.GetOrdinal("ExtendedSqft"))),
        .ExtendedBdft = If(reader.IsDBNull(reader.GetOrdinal("ExtendedBdft")), 0D, reader.GetDecimal(reader.GetOrdinal("ExtendedBdft"))),
        .CalculatedGrossSqft = If(reader.IsDBNull(reader.GetOrdinal("CalculatedGrossSqft")), 0D, reader.GetDecimal(reader.GetOrdinal("CalculatedGrossSqft"))),
        .Margin = If(reader.IsDBNull(reader.GetOrdinal("Margin")), 0D, reader.GetDecimal(reader.GetOrdinal("Margin"))),
        .MarginWithDelivery = If(reader.IsDBNull(reader.GetOrdinal("MarginWithDelivery")), 0D, reader.GetDecimal(reader.GetOrdinal("MarginWithDelivery"))),
        .PricePerBdft = If(reader.IsDBNull(reader.GetOrdinal("PricePerBdft")), 0D, reader.GetDecimal(reader.GetOrdinal("PricePerBdft"))),
        .PricePerSqft = If(reader.IsDBNull(reader.GetOrdinal("PricePerSqft")), 0D, reader.GetDecimal(reader.GetOrdinal("PricePerSqft"))),
            .FuturesAdderAmt = If(reader.IsDBNull(reader.GetOrdinal("FuturesAdderAmt")), Nothing, CType(reader.GetDecimal(reader.GetOrdinal("FuturesAdderAmt")), Decimal?)),
        .FuturesAdderProjTotal = If(reader.IsDBNull(reader.GetOrdinal("FuturesAdderProjTotal")), Nothing, CType(reader.GetDecimal(reader.GetOrdinal("FuturesAdderProjTotal")), Decimal?)),
        .FuturesContractMonth = If(reader.IsDBNull(reader.GetOrdinal("FuturesContractMonth")), "", reader.GetString(reader.GetOrdinal("FuturesContractMonth"))),
        .FuturesPriorSettle = If(reader.IsDBNull(reader.GetOrdinal("FuturesPriorSettle")), Nothing, CType(reader.GetDecimal(reader.GetOrdinal("FuturesPriorSettle")), Decimal?)),
        .FuturesPullDate = If(reader.IsDBNull(reader.GetOrdinal("FuturesPullDate")), Nothing, CType(reader.GetDateTime(reader.GetOrdinal("FuturesPullDate")), DateTime?)),
        .LumberFutureID = If(reader.IsDBNull(reader.GetOrdinal("LumberFutureID")), Nothing, CType(reader.GetInt32(reader.GetOrdinal("LumberFutureID")), Integer?)),
            .ActiveFloorSPFNo2 = If(reader.IsDBNull(reader.GetOrdinal("ActiveFloorSPFNo2")), 0D, reader.GetDecimal(reader.GetOrdinal("ActiveFloorSPFNo2"))),
        .ActiveRoofSPFNo2 = If(reader.IsDBNull(reader.GetOrdinal("ActiveRoofSPFNo2")), 0D, reader.GetDecimal(reader.GetOrdinal("ActiveRoofSPFNo2"))),
        .ActiveCostEffectiveID = If(reader.IsDBNull(reader.GetOrdinal("ActiveCostEffectiveID")), Nothing, CType(reader.GetInt32(reader.GetOrdinal("ActiveCostEffectiveID")), Integer?)),
        .ActiveCostEffectiveDate = If(reader.IsDBNull(reader.GetOrdinal("ActiveCostEffectiveDate")), Nothing, CType(reader.GetDateTime(reader.GetOrdinal("ActiveCostEffectiveDate")), DateTime?)),
            .FloorMarginPercent = If(reader.IsDBNull(reader.GetOrdinal("FloorMarginPercent")), Nothing, CType(reader.GetDecimal(reader.GetOrdinal("FloorMarginPercent")), Decimal?)),
        .FloorLumberAdder = If(reader.IsDBNull(reader.GetOrdinal("FloorLumberAdder")), Nothing, CType(reader.GetDecimal(reader.GetOrdinal("FloorLumberAdder")), Decimal?)),
        .RoofMarginPercent = If(reader.IsDBNull(reader.GetOrdinal("RoofMarginPercent")), Nothing, CType(reader.GetDecimal(reader.GetOrdinal("RoofMarginPercent")), Decimal?)),
        .RoofLumberAdder = If(reader.IsDBNull(reader.GetOrdinal("RoofLumberAdder")), Nothing, CType(reader.GetDecimal(reader.GetOrdinal("RoofLumberAdder")), Decimal?)),
        .WallMarginPercent = If(reader.IsDBNull(reader.GetOrdinal("WallMarginPercent")), Nothing, CType(reader.GetDecimal(reader.GetOrdinal("WallMarginPercent")), Decimal?)),
        .WallLumberAdder = If(reader.IsDBNull(reader.GetOrdinal("WallLumberAdder")), Nothing, CType(reader.GetDecimal(reader.GetOrdinal("WallLumberAdder")), Decimal?)),
            .CustomerID = If(reader.IsDBNull(reader.GetOrdinal("CustomerID")), Nothing, CType(reader.GetInt32(reader.GetOrdinal("CustomerID")), Integer?)),
        .CustomerName = If(reader.IsDBNull(reader.GetOrdinal("CustomerName")), "", reader.GetString(reader.GetOrdinal("CustomerName"))),
        .SalesID = If(reader.IsDBNull(reader.GetOrdinal("SalesID")), Nothing, CType(reader.GetInt32(reader.GetOrdinal("SalesID")), Integer?)),
        .SalesName = If(reader.IsDBNull(reader.GetOrdinal("SalesName")), "", reader.GetString(reader.GetOrdinal("SalesName"))),
        .EstimatorID = If(reader.IsDBNull(reader.GetOrdinal("EstimatorID")), Nothing, CType(reader.GetInt32(reader.GetOrdinal("EstimatorID")), Integer?)),
        .EstimatorName = If(reader.IsDBNull(reader.GetOrdinal("EstimatorName")), "", reader.GetString(reader.GetOrdinal("EstimatorName"))),
            .TotalBuildingCount = If(reader.IsDBNull(reader.GetOrdinal("TotalBuildingCount")), 0, reader.GetInt32(reader.GetOrdinal("TotalBuildingCount"))),
        .TotalBldgQty = If(reader.IsDBNull(reader.GetOrdinal("TotalBldgQty")), 0, reader.GetInt32(reader.GetOrdinal("TotalBldgQty"))),
        .TotalLevelCount = If(reader.IsDBNull(reader.GetOrdinal("TotalLevelCount")), 0, reader.GetInt32(reader.GetOrdinal("TotalLevelCount"))),
        .TotalActualUnitCount = If(reader.IsDBNull(reader.GetOrdinal("TotalActualUnitCount")), 0, reader.GetInt32(reader.GetOrdinal("TotalActualUnitCount"))),
        .Notes = If(reader.IsDBNull(reader.GetOrdinal("Notes")), "", reader.GetString(reader.GetOrdinal("Notes")))
    }
        End Function

#End Region
#Region "Read Operations - Detail"

        ''' <summary>
        ''' Gets building history records for a price history snapshot
        ''' </summary>
        Public Shared Function GetBuildingsByHistoryID(priceHistoryID As Integer) As List(Of PriceHistoryBuildingModel)
            Dim results As New List(Of PriceHistoryBuildingModel)

            Dim sql As String = "
                SELECT * FROM ProjVersionPriceHistoryBuildings
                WHERE PriceHistoryID = @PriceHistoryID
                ORDER BY BuildingName"

            Using reader = SqlConnectionManager.Instance.ExecuteReader(sql,
                {New SqlParameter("@PriceHistoryID", priceHistoryID)})
                While reader.Read()
                    results.Add(New PriceHistoryBuildingModel With {
                        .PriceHistoryBuildingID = reader.GetInt32(reader.GetOrdinal("PriceHistoryBuildingID")),
                        .PriceHistoryID = reader.GetInt32(reader.GetOrdinal("PriceHistoryID")),
                        .BuildingID = reader.GetInt32(reader.GetOrdinal("BuildingID")),
                        .BuildingName = If(reader.IsDBNull(reader.GetOrdinal("BuildingName")), "", reader.GetString(reader.GetOrdinal("BuildingName"))),
                        .BldgQty = reader.GetInt32(reader.GetOrdinal("BldgQty")),
                        .BaseSell = If(reader.IsDBNull(reader.GetOrdinal("BaseSell")), 0D, reader.GetDecimal(reader.GetOrdinal("BaseSell"))),
                        .BaseCost = If(reader.IsDBNull(reader.GetOrdinal("BaseCost")), 0D, reader.GetDecimal(reader.GetOrdinal("BaseCost"))),
                        .BaseDelivery = If(reader.IsDBNull(reader.GetOrdinal("BaseDelivery")), 0D, reader.GetDecimal(reader.GetOrdinal("BaseDelivery"))),
                        .BaseSqft = If(reader.IsDBNull(reader.GetOrdinal("BaseSqft")), 0D, reader.GetDecimal(reader.GetOrdinal("BaseSqft"))),
                        .BaseBdft = If(reader.IsDBNull(reader.GetOrdinal("BaseBdft")), 0D, reader.GetDecimal(reader.GetOrdinal("BaseBdft"))),
                        .ExtendedSell = If(reader.IsDBNull(reader.GetOrdinal("ExtendedSell")), 0D, reader.GetDecimal(reader.GetOrdinal("ExtendedSell"))),
                        .ExtendedCost = If(reader.IsDBNull(reader.GetOrdinal("ExtendedCost")), 0D, reader.GetDecimal(reader.GetOrdinal("ExtendedCost"))),
                        .ExtendedDelivery = If(reader.IsDBNull(reader.GetOrdinal("ExtendedDelivery")), 0D, reader.GetDecimal(reader.GetOrdinal("ExtendedDelivery"))),
                        .ExtendedSqft = If(reader.IsDBNull(reader.GetOrdinal("ExtendedSqft")), 0D, reader.GetDecimal(reader.GetOrdinal("ExtendedSqft"))),
                        .ExtendedBdft = If(reader.IsDBNull(reader.GetOrdinal("ExtendedBdft")), 0D, reader.GetDecimal(reader.GetOrdinal("ExtendedBdft"))),
                        .Margin = If(reader.IsDBNull(reader.GetOrdinal("Margin")), 0D, reader.GetDecimal(reader.GetOrdinal("Margin")))
                    })
                End While
            End Using

            Return results
        End Function

        ''' <summary>
        ''' Gets level history records for a building history record
        ''' </summary>
        Public Shared Function GetLevelsByBuildingHistoryID(priceHistoryBuildingID As Integer) As List(Of PriceHistoryLevelModel)
            Dim results As New List(Of PriceHistoryLevelModel)

            Dim sql As String = "
                SELECT * FROM ProjVersionPriceHistoryLevels
                WHERE PriceHistoryBuildingID = @PriceHistoryBuildingID
                ORDER BY LevelNumber"

            Using reader = SqlConnectionManager.Instance.ExecuteReader(sql,
                {New SqlParameter("@PriceHistoryBuildingID", priceHistoryBuildingID)})
                While reader.Read()
                    results.Add(New PriceHistoryLevelModel With {
                        .PriceHistoryLevelID = reader.GetInt32(reader.GetOrdinal("PriceHistoryLevelID")),
                        .PriceHistoryBuildingID = reader.GetInt32(reader.GetOrdinal("PriceHistoryBuildingID")),
                        .LevelID = reader.GetInt32(reader.GetOrdinal("LevelID")),
                        .LevelName = If(reader.IsDBNull(reader.GetOrdinal("LevelName")), "", reader.GetString(reader.GetOrdinal("LevelName"))),
                        .LevelNumber = reader.GetInt32(reader.GetOrdinal("LevelNumber")),
                        .ProductTypeID = reader.GetInt32(reader.GetOrdinal("ProductTypeID")),
                        .ProductTypeName = If(reader.IsDBNull(reader.GetOrdinal("ProductTypeName")), "", reader.GetString(reader.GetOrdinal("ProductTypeName"))),
                        .OverallSqft = If(reader.IsDBNull(reader.GetOrdinal("OverallSqft")), 0D, reader.GetDecimal(reader.GetOrdinal("OverallSqft"))),
                        .OverallLf = If(reader.IsDBNull(reader.GetOrdinal("OverallLf")), 0D, reader.GetDecimal(reader.GetOrdinal("OverallLf"))),
                        .OverallBdft = If(reader.IsDBNull(reader.GetOrdinal("OverallBdft")), 0D, reader.GetDecimal(reader.GetOrdinal("OverallBdft"))),
                        .LumberCost = If(reader.IsDBNull(reader.GetOrdinal("LumberCost")), 0D, reader.GetDecimal(reader.GetOrdinal("LumberCost"))),
                        .PlateCost = If(reader.IsDBNull(reader.GetOrdinal("PlateCost")), 0D, reader.GetDecimal(reader.GetOrdinal("PlateCost"))),
                        .LaborCost = If(reader.IsDBNull(reader.GetOrdinal("LaborCost")), 0D, reader.GetDecimal(reader.GetOrdinal("LaborCost"))),
                        .LaborMH = If(reader.IsDBNull(reader.GetOrdinal("LaborMH")), 0D, reader.GetDecimal(reader.GetOrdinal("LaborMH"))),
                        .DesignCost = If(reader.IsDBNull(reader.GetOrdinal("DesignCost")), 0D, reader.GetDecimal(reader.GetOrdinal("DesignCost"))),
                        .MgmtCost = If(reader.IsDBNull(reader.GetOrdinal("MgmtCost")), 0D, reader.GetDecimal(reader.GetOrdinal("MgmtCost"))),
                        .JobSuppliesCost = If(reader.IsDBNull(reader.GetOrdinal("JobSuppliesCost")), 0D, reader.GetDecimal(reader.GetOrdinal("JobSuppliesCost"))),
                        .ItemsCost = If(reader.IsDBNull(reader.GetOrdinal("ItemsCost")), 0D, reader.GetDecimal(reader.GetOrdinal("ItemsCost"))),
                        .DeliveryCost = If(reader.IsDBNull(reader.GetOrdinal("DeliveryCost")), 0D, reader.GetDecimal(reader.GetOrdinal("DeliveryCost"))),
                        .OverallCost = If(reader.IsDBNull(reader.GetOrdinal("OverallCost")), 0D, reader.GetDecimal(reader.GetOrdinal("OverallCost"))),
                        .OverallPrice = If(reader.IsDBNull(reader.GetOrdinal("OverallPrice")), 0D, reader.GetDecimal(reader.GetOrdinal("OverallPrice"))),
                        .Margin = If(reader.IsDBNull(reader.GetOrdinal("Margin")), 0D, reader.GetDecimal(reader.GetOrdinal("Margin"))),
                        .ActualUnitCount = If(reader.IsDBNull(reader.GetOrdinal("ActualUnitCount")), 0, reader.GetInt32(reader.GetOrdinal("ActualUnitCount")))
                    })
                End While
            End Using

            Return results
        End Function

        ''' <summary>
        ''' Deletes a price history record (cascades to buildings and levels)
        ''' </summary>
        Public Shared Sub DeletePriceHistory(priceHistoryID As Integer)
            Dim sql As String = "DELETE FROM ProjVersionPriceHistory WHERE PriceHistoryID = @PriceHistoryID"
            SqlConnectionManager.Instance.ExecuteNonQuery(sql, {New SqlParameter("@PriceHistoryID", priceHistoryID)})
        End Sub

#End Region
    End Class
End Namespace
