Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.Utilities

Namespace DataAccess


    Public Class RollupDataAccess
        ' Updated: UpdateBuildingRollups (use VersionID)
        Public Shared Sub UpdateBuildingRollups(buildingID As Integer)
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
                                                                       Dim wallPricePerBldgParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                                                                       Dim wallPricePerBldgObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateWallPricePerBldg, wallPricePerBldgParams)
                                                                       Dim wallPricePerBldg As Decimal = If(wallPricePerBldgObj Is DBNull.Value OrElse wallPricePerBldgObj Is Nothing, 0D, CDec(wallPricePerBldgObj))
                                                                       Dim floorBaseCostParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                                                                       Dim floorBaseCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateFloorBaseCost, floorBaseCostParams)
                                                                       Dim floorBaseCost As Decimal = If(floorBaseCostObj Is DBNull.Value OrElse floorBaseCostObj Is Nothing, 0D, CDec(floorBaseCostObj))
                                                                       Dim roofBaseCostParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                                                                       Dim roofBaseCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateRoofBaseCost, roofBaseCostParams)
                                                                       Dim roofBaseCost As Decimal = If(roofBaseCostObj Is DBNull.Value OrElse roofBaseCostObj Is Nothing, 0D, CDec(roofBaseCostObj))
                                                                       Dim wallBaseCostParams As SqlParameter() = {New SqlParameter("@BuildingID", buildingID)}
                                                                       Dim wallBaseCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.CalculateWallBaseCost, wallBaseCostParams)
                                                                       Dim wallBaseCost As Decimal = If(wallBaseCostObj Is DBNull.Value OrElse wallBaseCostObj Is Nothing, 0D, CDec(wallBaseCostObj))
                                                                       Dim extendedFloorCost As Decimal = floorBaseCost * bldgQty
                                                                       Dim extendedRoofCost As Decimal = roofBaseCost * bldgQty
                                                                       Dim extendedWallCost As Decimal = wallBaseCost * bldgQty
                                                                       Dim overallCost As Decimal = floorBaseCost + roofBaseCost + wallBaseCost
                                                                       Dim overallPrice As Decimal = floorPricePerBldg + roofPricePerBldg + wallPricePerBldg
                                                                       Dim updateParams As New Dictionary(Of String, Object) From {
                                                                            {"@BuildingID", buildingID},
                                                                            {"@FloorCostPerBldg", floorBaseCost},
                                                                            {"@RoofCostPerBldg", roofBaseCost},
                                                                            {"@WallCostPerBldg", wallBaseCost},
                                                                            {"@ExtendedFloorCost", extendedFloorCost},
                                                                            {"@ExtendedRoofCost", extendedRoofCost},
                                                                            {"@ExtendedWallCost", extendedWallCost},
                                                                            {"@OverallPrice", overallPrice},
                                                                            {"@OverallCost", overallCost}
                                                                        }
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateBuildingRollupsSql, HelperDataAccess.BuildParameters(updateParams))
                                                                   End Sub, "Error updating building rollups for ID " & buildingID)
        End Sub

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
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLevelRollupsSql, HelperDataAccess.BuildParameters(paramsDict))
                                                                   End Sub, "Error updating level rollups")
        End Sub
        Public Shared Sub RecalculateVersion(versionID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim buildings As List(Of BuildingModel) = ProjectDataAccess.GetBuildingsByVersionID(versionID)
                                                                       For Each bldg As BuildingModel In buildings
                                                                           Dim levels As List(Of LevelModel) = ProjectDataAccess.GetLevelsByBuildingID(bldg.BuildingID)
                                                                           For Each lvl As LevelModel In levels
                                                                               UpdateLevelRollups(lvl.LevelID)
                                                                           Next
                                                                           UpdateBuildingRollups(bldg.BuildingID)
                                                                       Next
                                                                   End Sub, "Error recalculating version " & versionID)
        End Sub

        ' Replace existing UpdateLevelRollups with this
        Public Shared Sub UpdateLevelRollups(levelID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim dt As DataTable = ProjectDataAccess.ComputeLevelUnitsDataTable(levelID)

                                                                       ' All sums are now 1-liners — no risk of mismatch
                                                                       Dim overallSQFT As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("PlanSQFT") * r.Field(Of Integer)("ActualUnitQuantity"))
                                                                       Dim overallLF As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("LF"))
                                                                       Dim overallBDFT As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("BDFT"))
                                                                       Dim lumberCost As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("LumberCost"))
                                                                       Dim plateCost As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("PlateCost"))
                                                                       Dim laborCost As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("ManufLaborCost"))
                                                                       Dim laborMH As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("ManHours"))
                                                                       Dim designCost As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("DesignLabor"))
                                                                       Dim mgmtCost As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("MGMTLabor"))
                                                                       Dim jobSuppliesCost As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("JobSuppliesCost"))
                                                                       Dim itemsCost As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("ItemCost"))
                                                                       Dim deliveryCost As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("DeliveryCost"))
                                                                       Dim overallCost As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("OverallCost"))
                                                                       Dim overallPrice As Decimal = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("SellPrice"))

                                                                       Dim levelInfo = ProjectDataAccess.GetLevelInfo(levelID)
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

                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLevelRollupsSql, HelperDataAccess.BuildParameters(paramsDict))
                                                                   End Sub, "Error updating level rollups for " & levelID)
        End Sub

    End Class
End Namespace
