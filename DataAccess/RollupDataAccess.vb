Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.BuildersPSE.Utilities

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
                                                                       Dim units As List(Of DisplayUnitData) = ProjectDataAccess.ComputeLevelUnits(levelID)
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

                                                                       Dim levelInfo As Tuple(Of Integer, Integer, Decimal) = ProjectDataAccess.GetLevelInfo(levelID)
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

