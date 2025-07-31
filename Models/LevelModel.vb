Option Strict On
Imports System.Collections.Generic

Namespace BuildersPSE.Models
    Public Class LevelModel
        Public Property LevelID As Integer
        Public Property VersionID As Integer ' FK to Projects
        Public Property BuildingID As Integer ' FK to Buildings
        Public Property ProductTypeID As Integer
        Public Property ProductTypeName As String  ' New: Friendly name from ProductType (populated via join)
        Public Property LevelNumber As Integer
        Public Property LevelName As String

        ' Calculated/aggregated fields (populated via DataAccess)
        Public Property OverallPrice As Decimal?
        Public Property OverallCost As Decimal?
        Public Property OverallSQFT As Decimal?
        Public Property OverallLF As Decimal?
        Public Property OverallBDFT As Decimal?
        Public Property LumberCost As Decimal?
        Public Property PlateCost As Decimal?
        Public Property LaborCost As Decimal?
        Public Property LaborMH As Decimal?
        Public Property DesignCost As Decimal?
        Public Property MGMTCost As Decimal?
        Public Property JobSuppliesCost As Decimal?
        Public Property ItemsCost As Decimal?
        Public Property DeliveryCost As Decimal?
        Public Property UnitOverallCost As Decimal?
        Public Property CommonSQFT As Decimal?
        Public Property TotalSQFT As Decimal?
        Public Property AvgPricePerSQFT As Decimal?

        ' Collections
        Public Property ActualUnitMappings As New List(Of ActualToLevelMappingModel) ' Quantities of units in this level
    End Class
End Namespace
