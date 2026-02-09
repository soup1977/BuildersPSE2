Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Utilities

Namespace Services
    ''' <summary>
    ''' Service for building and saving price history snapshots
    ''' </summary>
    Public Class PriceHistoryService

        Private ReadOnly _rollupService As New RollupCalculationService()

        ''' <summary>
        ''' Captures a complete price history snapshot for the given project/version
        ''' </summary>
        Public Function CapturePriceHistory(project As ProjectModel,
                                             version As ProjectVersionModel,
                                             reportType As String) As PriceHistoryModel

            ' Calculate project rollup
            Dim rollup As RollupResult = _rollupService.CalculateProjectRollup(project, version)

            ' Get active futures contract
            Dim activeFutures As LumberFutures = GetActiveFuturesContract(version.VersionID)

            ' Get active lumber cost effective info
            Dim activeLumberInfo = GetActiveLumberInfo(version.VersionID)

            ' Get product settings
            Dim settings = ProjectDataAccess.GetProjectProductSettings(version.VersionID)

            ' Build the model
            Dim model As New PriceHistoryModel With {
                .ProjectID = project.ProjectID,
                .VersionID = version.VersionID,
                .VersionName = version.VersionName,
                .CreatedBy = CurrentUser.WindowsLogon,
                .ReportType = reportType,
            .JBID = project.JBID,
                .ProjectName = project.ProjectName,
                .Address = project.Address,
                .City = project.City,
                .State = project.State,
                .Zip = project.Zip,
                .BidDate = project.BidDate,
                .MilesToJobSite = project.MilesToJobSite,
                .TotalGrossSqftEntered = project.TotalGrossSqft,
                .TotalNetSqftEntered = project.TotalNetSqft,
                .ExtendedSell = rollup.ExtendedSell,
                .ExtendedCost = rollup.ExtendedCost,
                .ExtendedDelivery = rollup.ExtendedDelivery,
                .ExtendedSqft = rollup.ExtendedSqft,
                .ExtendedBdft = rollup.ExtendedBdft,
                .CalculatedGrossSqft = rollup.CalculatedGrossSqft,
                .Margin = rollup.Margin,
                .MarginWithDelivery = rollup.MarginWithDelivery,
                .PricePerBdft = rollup.PricePerBdft,
                .PricePerSqft = rollup.PricePerSqft,
                .FuturesAdderAmt = version.FuturesAdderAmt,
                .FuturesAdderProjTotal = version.FuturesAdderProjTotal,
                .FuturesContractMonth = activeFutures?.ContractMonth,
                .FuturesPriorSettle = activeFutures?.PriorSettle,
                .FuturesPullDate = activeFutures?.PullDate,
                .LumberFutureID = activeFutures?.LumberFutureID,
                .ActiveFloorSPFNo2 = LumberDataAccess.GetActiveSPFNo2ByProductType(version.VersionID, "Floor"),
                .ActiveRoofSPFNo2 = LumberDataAccess.GetActiveSPFNo2ByProductType(version.VersionID, "Roof"),
                .ActiveCostEffectiveID = activeLumberInfo.CostEffectiveID,
                .ActiveCostEffectiveDate = activeLumberInfo.CostEffectiveDate,
                .CustomerID = version.CustomerID,
                .CustomerName = version.CustomerName,
                .SalesID = version.SalesID,
                .SalesName = version.SalesName,
                .EstimatorID = project.Estimator?.EstimatorID,
                .EstimatorName = project.Estimator?.EstimatorName
            }

            ' Product settings
            For Each setting In settings
                Select Case setting.ProductTypeID
                    Case 1 ' Floor
                        model.FloorMarginPercent = setting.MarginPercent
                        model.FloorLumberAdder = setting.LumberAdder
                    Case 2 ' Roof
                        model.RoofMarginPercent = setting.MarginPercent
                        model.RoofLumberAdder = setting.LumberAdder
                    Case 3 ' Wall
                        model.WallMarginPercent = setting.MarginPercent
                        model.WallLumberAdder = setting.LumberAdder
                End Select
            Next

            ' Building/Level detail
            Dim totalBldgQty As Integer = 0
            Dim totalLevelCount As Integer = 0
            Dim totalUnitCount As Integer = 0

            For Each bldg In project.Buildings
                Dim bldgRollup = _rollupService.CalculateBuildingRollup(bldg)

                Dim bldgModel As New PriceHistoryBuildingModel With {
                    .BuildingID = bldg.BuildingID,
                    .BuildingName = bldg.BuildingName,
                    .BldgQty = bldg.BldgQty,
                    .BaseSell = bldgRollup.BaseSell,
                    .BaseCost = bldgRollup.BaseCost,
                    .BaseDelivery = bldgRollup.BaseDelivery,
                    .BaseSqft = bldgRollup.BaseSqft,
                    .BaseBdft = bldgRollup.BaseBdft,
                    .ExtendedSell = bldgRollup.ExtendedSell,
                    .ExtendedCost = bldgRollup.ExtendedCost,
                    .ExtendedDelivery = bldgRollup.ExtendedDelivery,
                    .ExtendedSqft = bldgRollup.ExtendedSqft,
                    .ExtendedBdft = bldgRollup.ExtendedBdft,
                    .Margin = bldgRollup.Margin
                }

                totalBldgQty += bldg.BldgQty

                For Each level In bldg.Levels
                    Dim levelRollup = _rollupService.CalculateLevelRollup(level, 1)
                    Dim unitCount = level.ActualUnitMappings?.Sum(Function(m) m.Quantity)

                    Dim levelModel As New PriceHistoryLevelModel With {
                        .LevelID = level.LevelID,
                        .LevelName = level.LevelName,
                        .LevelNumber = level.LevelNumber,
                        .ProductTypeID = level.ProductTypeID,
                        .ProductTypeName = level.ProductTypeName,
                        .OverallSqft = levelRollup.BaseSqft,
                        .OverallLf = levelRollup.OverallLf,
                        .OverallBdft = levelRollup.BaseBdft,
                        .LumberCost = levelRollup.LumberCost,
                        .PlateCost = levelRollup.PlateCost,
                        .LaborCost = levelRollup.LaborCost,
                        .LaborMH = 0, ' Add if available
                        .DesignCost = levelRollup.DesignCost,
                        .MgmtCost = levelRollup.MgmtCost,
                        .JobSuppliesCost = levelRollup.JobSuppliesCost,
                        .ItemsCost = levelRollup.ItemsCost,
                        .DeliveryCost = levelRollup.BaseDelivery,
                        .OverallCost = levelRollup.BaseCost,
                        .OverallPrice = levelRollup.BaseSell,
                        .Margin = levelRollup.Margin,
                        .ActualUnitCount = If(unitCount, 0)
                    }

                    bldgModel.Levels.Add(levelModel)
                    totalLevelCount += 1
                    totalUnitCount += If(unitCount, 0)
                Next

                model.Buildings.Add(bldgModel)
            Next

            model.TotalBuildingCount = project.Buildings.Count
            model.TotalBldgQty = totalBldgQty
            model.TotalLevelCount = totalLevelCount
            model.TotalActualUnitCount = totalUnitCount

            Return model
        End Function

        Private Function GetActiveFuturesContract(versionID As Integer) As LumberFutures
            Dim futures = LumberDataAccess.GetFuturesForVersion(versionID)
            Return futures.FirstOrDefault(Function(f) f.Active)
        End Function

        Private Function GetActiveLumberInfo(versionID As Integer) As (CostEffectiveID As Integer?, CostEffectiveDate As Date?)
            ' Query for active lumber history record
            Dim sql As String = "
                SELECT TOP 1 h.CostEffectiveDateID, c.CosteffectiveDate
                FROM RawUnitLumberHistory h
                JOIN LumberCostEffective c ON h.CostEffectiveDateID = c.CostEffectiveID
                WHERE h.VersionID = @VersionID AND h.IsActive = 1"

            Using reader = SqlConnectionManager.Instance.ExecuteReader(sql,
                {New SqlParameter("@VersionID", versionID)})
                If reader.Read() Then
                    Return (
                        If(reader.IsDBNull(0), Nothing, CType(reader.GetInt32(0), Integer?)),
                        If(reader.IsDBNull(1), Nothing, CType(reader.GetDateTime(1), Date?))
                    )
                End If
            End Using

            Return (Nothing, Nothing)
        End Function

    End Class
End Namespace
