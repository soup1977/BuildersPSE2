Option Strict On
Imports BuildersPSE2.BuildersPSE.Models

Namespace Services
    ''' <summary>
    ''' Centralized service for calculating project, building, and level rollup data.
    ''' Eliminates duplication across LoadProjectRollupData, LoadBuildingRollupData, LoadLevelRollupData.
    ''' </summary>
    Public Class RollupCalculationService

        ''' <summary>
        ''' Calculates rollup totals for a project
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
        ''' Calculates rollup totals for a building
        ''' </summary>
        Public Function CalculateBuildingRollup(bldg As BuildingModel) As RollupResult
            Dim result As New RollupResult With {.Type = RollupType.Building}

            If bldg.Levels Is Nothing OrElse bldg.Levels.Count = 0 Then
                result.HasData = False
                result.StatusMessage = "No levels defined yet. Add levels and assign units for rollup data."
                Return result
            End If

            ' Store BldgQty for display
            result.BldgQty = bldg.BldgQty

            ' Per-building totals from levels (BASE values)
            result.BaseSqft = bldg.Levels.Sum(Function(l) If(l.OverallSQFT, 0D))
            result.BaseBdft = bldg.Levels.Sum(Function(l) If(l.OverallBDFT, 0D))
            result.BaseCost = bldg.Levels.Sum(Function(l) If(l.OverallCost, 0D))
            result.BaseDelivery = bldg.Levels.Sum(Function(l) If(l.DeliveryCost, 0D))
            result.BaseSell = bldg.Levels.Sum(Function(l) If(l.OverallPrice, 0D))

            ' Extended by building quantity
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
        ''' Calculates rollup totals for a level (includes BldgQty for extended values)
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
        Private Sub CalculateMargins(result As RollupResult)
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
        Private Sub CalculatePerUnitPrices(result As RollupResult)
            result.PricePerBdft = If(result.ExtendedBdft = 0, 0D, result.ExtendedSell / result.ExtendedBdft)
            result.PricePerSqft = If(result.ExtendedSqft = 0, 0D, result.ExtendedSell / result.ExtendedSqft)
        End Sub

        ''' <summary>
        ''' Checks if user-entered gross SQFT matches calculated rollup (Project only)
        ''' </summary>
        Private Sub CheckGrossSqftMismatch(result As RollupResult)
            Const grossThreshold As Double = 0.01

            If result.TotalGrossSqft = 0 Then
                result.HasGrossSqftMismatch = False
                Return
            End If

            Dim variance As Double = Math.Abs(result.CalculatedGrossSqft - result.TotalGrossSqft) / result.TotalGrossSqft
            result.HasGrossSqftMismatch = variance > grossThreshold
        End Sub

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