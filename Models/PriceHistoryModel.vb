''' <summary>
''' Model for capturing a complete price history snapshot
''' </summary>
Public Class PriceHistoryModel
    ' Core Identification
    Public Property PriceHistoryID As Integer
    Public Property ProjectID As Integer
    Public Property VersionID As Integer
    Public Property VersionName As String

    ' Audit
    Public Property CreatedDate As DateTime
    Public Property CreatedBy As String
    Public Property ReportType As String

    ' Project Header
    Public Property JBID As String
    Public Property ProjectName As String
    Public Property Address As String
    Public Property City As String
    Public Property State As String
    Public Property Zip As String
    Public Property BidDate As Date?
    Public Property MilesToJobSite As Integer
    Public Property TotalGrossSqftEntered As Integer?
    Public Property TotalNetSqftEntered As Integer?

    ' Rollup Values
    Public Property ExtendedSell As Decimal
    Public Property ExtendedCost As Decimal
    Public Property ExtendedDelivery As Decimal
    Public Property ExtendedSqft As Decimal
    Public Property ExtendedBdft As Decimal
    Public Property CalculatedGrossSqft As Decimal
    Public Property Margin As Decimal
    Public Property MarginWithDelivery As Decimal
    Public Property PricePerBdft As Decimal
    Public Property PricePerSqft As Decimal

    ' Futures Adder
    Public Property FuturesAdderAmt As Decimal?
    Public Property FuturesAdderProjTotal As Decimal?
    Public Property FuturesContractMonth As String
    Public Property FuturesPriorSettle As Decimal?
    Public Property FuturesPullDate As DateTime?
    Public Property LumberFutureID As Integer?

    ' Active Lumber Pricing
    Public Property ActiveFloorSPFNo2 As Decimal
    Public Property ActiveRoofSPFNo2 As Decimal
    Public Property ActiveCostEffectiveID As Integer?
    Public Property ActiveCostEffectiveDate As Date?

    ' Project Product Settings
    Public Property FloorMarginPercent As Decimal?
    Public Property FloorLumberAdder As Decimal?
    Public Property RoofMarginPercent As Decimal?
    Public Property RoofLumberAdder As Decimal?
    Public Property WallMarginPercent As Decimal?
    Public Property WallLumberAdder As Decimal?

    ' Customer/Sales Context
    Public Property CustomerID As Integer?
    Public Property CustomerName As String
    Public Property SalesID As Integer?
    Public Property SalesName As String
    Public Property EstimatorID As Integer?
    Public Property EstimatorName As String

    ' Building Summary
    Public Property TotalBuildingCount As Integer
    Public Property TotalBldgQty As Integer
    Public Property TotalLevelCount As Integer
    Public Property TotalActualUnitCount As Integer

    ' Notes
    Public Property Notes As String

    ' Child collections
    Public Property Buildings As New List(Of PriceHistoryBuildingModel)

    ' Add this property to the PriceHistoryModel class
    Public ReadOnly Property DisplayText As String
        Get
            Return $"{CreatedDate:g} - {VersionName} ({ExtendedSell:C0}, {Margin:P1})"
        End Get
    End Property

End Class

Public Class PriceHistoryBuildingModel
    Public Property PriceHistoryBuildingID As Integer
    Public Property PriceHistoryID As Integer
    Public Property BuildingID As Integer
    Public Property BuildingName As String
    Public Property BldgQty As Integer

    ' Building Rollup
    Public Property BaseSell As Decimal
    Public Property BaseCost As Decimal
    Public Property BaseDelivery As Decimal
    Public Property BaseSqft As Decimal
    Public Property BaseBdft As Decimal
    Public Property ExtendedSell As Decimal
    Public Property ExtendedCost As Decimal
    Public Property ExtendedDelivery As Decimal
    Public Property ExtendedSqft As Decimal
    Public Property ExtendedBdft As Decimal
    Public Property Margin As Decimal

    ' Child levels
    Public Property Levels As New List(Of PriceHistoryLevelModel)
End Class

Public Class PriceHistoryLevelModel
    Public Property PriceHistoryLevelID As Integer
    Public Property PriceHistoryBuildingID As Integer
    Public Property LevelID As Integer
    Public Property LevelName As String
    Public Property LevelNumber As Integer
    Public Property ProductTypeID As Integer
    Public Property ProductTypeName As String

    ' Level Rollup
    Public Property OverallSqft As Decimal
    Public Property OverallLf As Decimal
    Public Property OverallBdft As Decimal
    Public Property LumberCost As Decimal
    Public Property PlateCost As Decimal
    Public Property LaborCost As Decimal
    Public Property LaborMH As Decimal
    Public Property DesignCost As Decimal
    Public Property MgmtCost As Decimal
    Public Property JobSuppliesCost As Decimal
    Public Property ItemsCost As Decimal
    Public Property DeliveryCost As Decimal
    Public Property OverallCost As Decimal
    Public Property OverallPrice As Decimal
    Public Property Margin As Decimal
    Public Property ActualUnitCount As Integer


End Class
