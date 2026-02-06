Option Strict On
Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Utilities

Namespace Services
    ''' <summary>
    ''' Centralized service for calculating AND persisting project, building, and level rollup data.
    ''' Single source of truth: database operations use the same calculation logic as in-memory display.
    ''' </summary>
    Public Class RollupCalculationService

#Region "In-Memory Calculations (Primary Logic)"

        ''' <summary>
        ''' Calculates rollup totals for a project (in-memory, for display)
        ''' </summary>
        Public Function CalculateProjectRollup(proj As ProjectModel, Optional currentVersion As ProjectVersionModel = Nothing) As RollupResult
            Dim result As New RollupResult With {.Type = RollupType.Project}

            If proj.Buildings Is Nothing OrElse proj.Buildings.Count = 0 Then
                result.HasData = False
                result.StatusMessage = "No buildings defined yet. Add buildings and levels for rollup data."
                Return result
            End If

            ' Calculate aggregates from buildings
            result.CalculatedGrossSqft = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) If(l.OverallSQFT, 0D)) * b.BldgQty)
            result.ExtendedSqft = result.CalculatedGrossSqft
            result.ExtendedBdft = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) If(l.OverallBDFT, 0D)) * b.BldgQty)
            result.ExtendedCost = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) If(l.OverallCost, 0D)) * b.BldgQty)
            result.ExtendedDelivery = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) If(l.DeliveryCost, 0D)) * b.BldgQty)
            result.ExtendedSell = proj.Buildings.Sum(Function(b) b.Levels.Sum(Function(l) If(l.OverallPrice, 0D)) * b.BldgQty)

            ' User-entered values
            result.TotalGrossSqft = If(proj.TotalGrossSqft.HasValue, CInt(proj.TotalGrossSqft.Value), 0)
            result.TotalNetSqft = If(proj.TotalNetSqft.HasValue, CInt(proj.TotalNetSqft.Value), 0)

            ' Futures Adder values from version
            If currentVersion IsNot Nothing Then
                result.FuturesAdderAmt = currentVersion.FuturesAdderAmt.GetValueOrDefault(0D)
                result.FuturesAdderProjTotal = currentVersion.FuturesAdderProjTotal.GetValueOrDefault(0D)
            Else
                result.FuturesAdderAmt = 0D
                result.FuturesAdderProjTotal = 0D
            End If

            If result.CalculatedGrossSqft > 0 Then
                result.HasData = True
                CalculateMargins(result)
                CalculatePerUnitPrices(result)
                CheckGrossSqftMismatch(result)
            Else
                result.HasData = False
                result.StatusMessage = "No rollup data available yet. Assign units to levels for calculations."
            End If

            Return result
        End Function

        ''' <summary>
        ''' Calculates rollup totals for a building (in-memory, for display)
        ''' </summary>
        Public Function CalculateBuildingRollup(bldg As BuildingModel) As RollupResult
            Dim result As New RollupResult With {.Type = RollupType.Building}

            If bldg.Levels Is Nothing OrElse bldg.Levels.Count = 0 Then
                result.HasData = False
                result.StatusMessage = "No levels defined yet. Add levels and assign units for rollup data."
                Return result
            End If

            result.BldgQty = bldg.BldgQty

            ' Sum from levels
            result.BaseSqft = bldg.Levels.Sum(Function(l) If(l.OverallSQFT, 0D))
            result.BaseBdft = bldg.Levels.Sum(Function(l) If(l.OverallBDFT, 0D))
            result.BaseCost = bldg.Levels.Sum(Function(l) If(l.OverallCost, 0D))
            result.BaseDelivery = bldg.Levels.Sum(Function(l) If(l.DeliveryCost, 0D))
            result.BaseSell = bldg.Levels.Sum(Function(l) If(l.OverallPrice, 0D))

            ' Multiply by building quantity
            result.ExtendedSqft = result.BaseSqft * bldg.BldgQty
            result.ExtendedBdft = result.BaseBdft * bldg.BldgQty
            result.ExtendedCost = result.BaseCost * bldg.BldgQty
            result.ExtendedDelivery = result.BaseDelivery * bldg.BldgQty
            result.ExtendedSell = result.BaseSell * bldg.BldgQty
            result.CalculatedGrossSqft = result.BaseSqft

            If result.BaseSqft > 0 OrElse result.BaseBdft > 0 OrElse result.BaseCost > 0 OrElse result.BaseSell > 0 Then
                result.HasData = True
                CalculateMargins(result)
                CalculatePerUnitPrices(result)
            Else
                result.HasData = False
                result.StatusMessage = "No rollup data available yet. Assign units to levels for calculations."
            End If

            Return result
        End Function

        ''' <summary>
        ''' Calculates rollup totals for a level from unit-level data (ComputeLevelUnitsDataTable).
        ''' This is THE authoritative calculation method that respects current margins.
        ''' </summary>
        Public Shared Function CalculateLevelRollupFromUnits(levelID As Integer, Optional bldgQty As Integer = 1) As RollupResult
            Dim result As New RollupResult With {
                .Type = RollupType.Level,
                .BldgQty = bldgQty
            }

            ' Get dynamically calculated unit data (with current margins applied)
            Dim dt As DataTable = ProjectDataAccess.ComputeLevelUnitsDataTable(levelID)

            If dt.Rows.Count = 0 Then
                result.HasData = False
                result.StatusMessage = "No units assigned to this level."
                Return result
            End If

            ' Aggregate BASE values (per level, before BldgQty multiplication)
            result.BaseSqft = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("PlanSQFT") * r.Field(Of Integer)("ActualUnitQuantity"))
            result.BaseBdft = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("BDFT"))
            result.BaseDelivery = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("DeliveryCost"))
            result.BaseCost = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("OverallCost"))
            result.BaseSell = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("SellPrice"))

            ' Detail costs (base)
            result.LumberCost = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("LumberCost"))
            result.PlateCost = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("PlateCost"))
            result.LaborCost = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("ManufLaborCost"))
            result.DesignCost = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("DesignLabor"))
            result.MgmtCost = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("MGMTLabor"))
            result.JobSuppliesCost = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("JobSuppliesCost"))
            result.ItemsCost = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("ItemCost"))
            result.OverallLf = dt.AsEnumerable().Sum(Function(r) r.Field(Of Decimal)("LF"))

            ' Extended values (multiplied by BldgQty)
            result.ExtendedSqft = result.BaseSqft * bldgQty
            result.ExtendedBdft = result.BaseBdft * bldgQty
            result.ExtendedCost = result.BaseCost * bldgQty
            result.ExtendedDelivery = result.BaseDelivery * bldgQty
            result.ExtendedSell = result.BaseSell * bldgQty

            result.ExtLumberCost = result.LumberCost * bldgQty
            result.ExtPlateCost = result.PlateCost * bldgQty
            result.ExtLaborCost = result.LaborCost * bldgQty
            result.ExtDesignCost = result.DesignCost * bldgQty
            result.ExtMgmtCost = result.MgmtCost * bldgQty
            result.ExtJobSuppliesCost = result.JobSuppliesCost * bldgQty
            result.ExtItemsCost = result.ItemsCost * bldgQty
            result.ExtOverallLf = result.OverallLf * bldgQty

            result.HasData = True
            CalculateMargins(result)
            CalculatePerUnitPrices(result)

            Return result
        End Function

        ''' <summary>
        ''' Legacy method: Calculates level rollup from LevelModel properties (reads from database).
        ''' DEPRECATED: Prefer CalculateLevelRollupFromUnits which recalculates with current margins.
        ''' </summary>
        Public Function CalculateLevelRollup(level As LevelModel, Optional bldgQty As Integer = 1) As RollupResult
            Dim result As New RollupResult With {
                .Type = RollupType.Level,
                .HasData = True,
                .BldgQty = bldgQty,
                .BaseSell = CDec(If(level.OverallPrice, 0D)),
                .BaseCost = CDec(If(level.OverallCost, 0D)),
                .BaseSqft = CDec(If(level.OverallSQFT, 0D)),
                .BaseBdft = CDec(If(level.OverallBDFT, 0D)),
                .BaseDelivery = CDec(If(level.DeliveryCost, 0D)),
                .ExtendedSell = CDec(If(level.OverallPrice, 0D)) * bldgQty,
                .ExtendedCost = CDec(If(level.OverallCost, 0D)) * bldgQty,
                .ExtendedSqft = CDec(If(level.OverallSQFT, 0D)) * bldgQty,
                .ExtendedBdft = CDec(If(level.OverallBDFT, 0D)) * bldgQty,
                .ExtendedDelivery = CDec(If(level.DeliveryCost, 0D)) * bldgQty,
                .LumberCost = CDec(If(level.LumberCost, 0D)),
                .PlateCost = CDec(If(level.PlateCost, 0D)),
                .LaborCost = CDec(If(level.LaborCost, 0D)),
                .DesignCost = CDec(If(level.DesignCost, 0D)),
                .MgmtCost = CDec(If(level.MGMTCost, 0D)),
                .JobSuppliesCost = CDec(If(level.JobSuppliesCost, 0D)),
                .ItemsCost = CDec(If(level.ItemsCost, 0D)),
                .OverallLf = CDec(If(level.OverallLF, 0D)),
                .ExtLumberCost = CDec(If(level.LumberCost, 0D)) * bldgQty,
                .ExtPlateCost = CDec(If(level.PlateCost, 0D)) * bldgQty,
                .ExtLaborCost = CDec(If(level.LaborCost, 0D)) * bldgQty,
                .ExtDesignCost = CDec(If(level.DesignCost, 0D)) * bldgQty,
                .ExtMgmtCost = CDec(If(level.MGMTCost, 0D)) * bldgQty,
                .ExtJobSuppliesCost = CDec(If(level.JobSuppliesCost, 0D)) * bldgQty,
                .ExtItemsCost = CDec(If(level.ItemsCost, 0D)) * bldgQty,
                .ExtOverallLf = CDec(If(level.OverallLF, 0D)) * bldgQty
            }

            CalculateMargins(result)
            CalculatePerUnitPrices(result)

            Return result
        End Function

        ''' <summary>
        ''' Calculates margin and margin with delivery
        ''' </summary>
        Private Shared Sub CalculateMargins(result As RollupResult)
            ' Margin (excluding delivery from denominator)
            If result.ExtendedSell - result.ExtendedDelivery <> 0 Then
                result.Margin = ((result.ExtendedSell - result.ExtendedDelivery) - result.ExtendedCost) /
                                (result.ExtendedSell - result.ExtendedDelivery)
            Else
                result.Margin = 0D
            End If

            ' Margin with delivery (including delivery in cost)
            If result.ExtendedSell <> 0 Then
                result.MarginWithDelivery = (result.ExtendedSell - (result.ExtendedCost + result.ExtendedDelivery)) /
                                            result.ExtendedSell
            Else
                result.MarginWithDelivery = 0D
            End If
        End Sub

        ''' <summary>
        ''' Calculates per-unit pricing
        ''' </summary>
        Private Shared Sub CalculatePerUnitPrices(result As RollupResult)
            result.PricePerBdft = If(result.ExtendedBdft = 0, 0D, result.ExtendedSell / result.ExtendedBdft)
            result.PricePerSqft = If(result.ExtendedSqft = 0, 0D, result.ExtendedSell / result.ExtendedSqft)
        End Sub

        ''' <summary>
        ''' Checks if user-entered gross SQFT matches calculated rollup (Project only)
        ''' </summary>
        Private Sub CheckGrossSqftMismatch(result As RollupResult)
            Const grossThreshold As Double = 0.05

            If result.TotalGrossSqft = 0 Then
                result.HasGrossSqftMismatch = False
                Return
            End If

            Dim variance As Double = Math.Abs(result.CalculatedGrossSqft - result.TotalGrossSqft) / result.TotalGrossSqft
            result.HasGrossSqftMismatch = variance > grossThreshold
        End Sub

#End Region

#Region "Database Persistence Operations"

        ''' <summary>
        ''' Recalculates and persists rollups for an entire version (all buildings and levels).
        ''' This is the main entry point for recalculating everything after margin changes.
        ''' </summary>
        Public Shared Sub RecalculateVersion(versionID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim buildings As List(Of BuildingModel) = ProjectDataAccess.GetBuildingsByVersionID(versionID)
                                                                       For Each bldg As BuildingModel In buildings
                                                                           For Each lvl As LevelModel In bldg.Levels
                                                                               ' Update each level (this recalculates with current margins)
                                                                               UpdateLevelRollups(lvl.LevelID)
                                                                           Next
                                                                           ' Update building (aggregates from freshly updated levels)
                                                                           UpdateBuildingRollups(bldg.BuildingID)
                                                                       Next
                                                                   End Sub, "Error recalculating version " & versionID)
        End Sub

        ''' <summary>
        ''' Recalculates and persists level rollups to the Levels table.
        ''' Uses CalculateLevelRollupFromUnits which dynamically applies current margins from ProjectProductSettings.
        ''' </summary>
        Public Shared Sub UpdateLevelRollups(levelID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       ' Calculate using the authoritative method (with current margins)
                                                                       Dim result As RollupResult = CalculateLevelRollupFromUnits(levelID, bldgQty:=1)

                                                                       If Not result.HasData Then
                                                                           Return ' No units assigned, nothing to update
                                                                       End If

                                                                       ' Get level-specific data (common SQFT)
                                                                       Dim levelInfo = ProjectDataAccess.GetLevelInfo(levelID)
                                                                       Dim commonSQFT As Decimal = levelInfo.Item3
                                                                       Dim totalSQFT As Decimal = result.BaseSqft + commonSQFT
                                                                       Dim avgPricePerSqft As Decimal = If(totalSQFT > 0D, result.BaseSell / totalSQFT, 0D)

                                                                       ' Persist to Levels table
                                                                       Dim paramsDict As New Dictionary(Of String, Object) From {
                                                                           {"@LevelID", levelID},
                                                                           {"@OverallSQFT", result.BaseSqft},
                                                                           {"@OverallLF", result.OverallLf},
                                                                           {"@OverallBDFT", result.BaseBdft},
                                                                           {"@LumberCost", result.LumberCost},
                                                                           {"@PlateCost", result.PlateCost},
                                                                           {"@LaborCost", result.LaborCost},
                                                                           {"@LaborMH", 0D},
                                                                           {"@DesignCost", result.DesignCost},
                                                                           {"@MGMTCost", result.MgmtCost},
                                                                           {"@JobSuppliesCost", result.JobSuppliesCost},
                                                                           {"@ItemsCost", result.ItemsCost},
                                                                           {"@DeliveryCost", result.BaseDelivery},
                                                                           {"@OverallCost", result.BaseCost},
                                                                           {"@OverallPrice", result.BaseSell},
                                                                           {"@TotalSQFT", totalSQFT},
                                                                           {"@AvgPricePerSQFT", avgPricePerSqft}
                                                                       }

                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLevelRollupsSql, HelperDataAccess.BuildParameters(paramsDict))
                                                                   End Sub, "Error updating level rollups for " & levelID)
        End Sub

        ''' <summary>
        ''' Recalculates and persists building rollups to the Buildings table.
        ''' Uses CalculateBuildingRollup which aggregates from freshly updated level data.
        ''' </summary>
        Public Shared Sub UpdateBuildingRollups(buildingID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       ' Load building with its levels (which were just updated)
                                                                       Dim levels As List(Of LevelModel) = ProjectDataAccess.GetLevelsByBuildingID(buildingID)

                                                                       If Not levels.Any() Then
                                                                           Return ' No levels, nothing to update
                                                                       End If

                                                                       ' Get building quantity
                                                                       Dim bldgQty As Integer = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                                                                           Queries.SelectBldgQty, {New SqlParameter("@BuildingID", buildingID)}))

                                                                       ' Create a temporary BuildingModel to use the calculation logic
                                                                       Dim bldg As New BuildingModel With {
                                                                           .BuildingID = buildingID,
                                                                           .BldgQty = bldgQty,
                                                                           .Levels = levels
                                                                       }

                                                                       ' Use the standard calculation method
                                                                       Dim result As RollupResult = New RollupCalculationService().CalculateBuildingRollup(bldg)

                                                                       ' Persist to Buildings table
                                                                       Dim updateParams As New Dictionary(Of String, Object) From {
                                                                           {"@BuildingID", buildingID},
                                                                           {"@FloorCostPerBldg", GetCostByProductType(levels, 1)},
                                                                           {"@RoofCostPerBldg", GetCostByProductType(levels, 2)},
                                                                           {"@WallCostPerBldg", GetCostByProductType(levels, 3)},
                                                                           {"@ExtendedFloorCost", GetCostByProductType(levels, 1) * bldgQty},
                                                                           {"@ExtendedRoofCost", GetCostByProductType(levels, 2) * bldgQty},
                                                                           {"@ExtendedWallCost", GetCostByProductType(levels, 3) * bldgQty},
                                                                           {"@OverallPrice", result.BaseSell},
                                                                           {"@OverallCost", result.BaseCost}
                                                                       }
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateBuildingRollupsSql, HelperDataAccess.BuildParameters(updateParams))
                                                                   End Sub, "Error updating building rollups for ID " & buildingID)
        End Sub

        ''' <summary>
        ''' Helper to get total cost for a specific product type across levels
        ''' </summary>
        Private Shared Function GetCostByProductType(levels As List(Of LevelModel), productTypeID As Integer) As Decimal
            Return levels.Where(Function(l) l.ProductTypeID = productTypeID).Sum(Function(l) If(l.OverallCost, 0D))
        End Function

        ''' <summary>
        ''' Helper to get total price for a specific product type across levels
        ''' </summary>
        Private Shared Function GetPriceByProductType(levels As List(Of LevelModel), productTypeID As Integer) As Decimal
            Return levels.Where(Function(l) l.ProductTypeID = productTypeID).Sum(Function(l) If(l.OverallPrice, 0D))
        End Function

        ''' <summary>
        ''' Legacy method for setting level rollups with explicit values (used by older code).
        ''' DEPRECATED: Prefer UpdateLevelRollups which recalculates from ComputeLevelUnitsDataTable.
        ''' </summary>
        Public Sub SetLevelRollups(levelID As Integer, overallSqft As Decimal, overallLf As Decimal, overallBdft As Decimal,
                                   lumberCost As Decimal, plateCost As Decimal, laborCost As Decimal, laborMh As Decimal,
                                   designCost As Decimal, mgmtCost As Decimal, jobSuppliesCost As Decimal, itemsCost As Decimal,
                                   deliveryCost As Decimal, overallCost As Decimal, overallPrice As Decimal, totalSqft As Decimal,
                                   avgPricePerSqft As Decimal)
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

#End Region

    End Class

    ''' <summary>
    ''' Result container for rollup calculations
    ''' </summary>
    Public Class RollupResult
        Public Property Type As RollupType
        Public Property HasData As Boolean
        Public Property StatusMessage As String = String.Empty

        ' Base values (per unit/per building - NOT multiplied by BldgQty)
        Public Property BaseSell As Decimal
        Public Property BaseCost As Decimal
        Public Property BaseDelivery As Decimal
        Public Property BaseSqft As Decimal
        Public Property BaseBdft As Decimal

        ' Extended values (multiplied by BldgQty)
        Public Property ExtendedSell As Decimal
        Public Property ExtendedCost As Decimal
        Public Property ExtendedDelivery As Decimal
        Public Property ExtendedSqft As Decimal
        Public Property ExtendedBdft As Decimal

        ' Building quantity for display
        Public Property BldgQty As Integer = 1

        ' Calculated values (based on Extended for consistency with Project rollup)
        Public Property Margin As Decimal
        Public Property MarginWithDelivery As Decimal
        Public Property PricePerBdft As Decimal
        Public Property PricePerSqft As Decimal

        ' Project-specific
        Public Property TotalGrossSqft As Integer
        Public Property TotalNetSqft As Integer
        Public Property CalculatedGrossSqft As Decimal
        Public Property HasGrossSqftMismatch As Boolean

        ' Futures Adder values (Project-specific)
        Public Property FuturesAdderAmt As Decimal
        Public Property FuturesAdderProjTotal As Decimal

        ' Level-specific detail costs (Base values)
        Public Property LumberCost As Decimal
        Public Property PlateCost As Decimal
        Public Property LaborCost As Decimal
        Public Property DesignCost As Decimal
        Public Property MgmtCost As Decimal
        Public Property JobSuppliesCost As Decimal
        Public Property ItemsCost As Decimal
        Public Property OverallLf As Decimal

        ' Level-specific detail costs (Extended values)
        Public Property ExtLumberCost As Decimal
        Public Property ExtPlateCost As Decimal
        Public Property ExtLaborCost As Decimal
        Public Property ExtDesignCost As Decimal
        Public Property ExtMgmtCost As Decimal
        Public Property ExtJobSuppliesCost As Decimal
        Public Property ExtItemsCost As Decimal
        Public Property ExtOverallLf As Decimal
    End Class

    Public Enum RollupType
        Project
        Building
        Level
    End Enum

End Namespace