Option Strict On
Imports System.Collections.Generic

Namespace BuildersPSE.Models
    Public Class RawUnitModel
        Public Property RawUnitID As Integer ' PK, auto-incremented
        Public Property VersionID As Integer ' FK to Projects
        Public Property ProductTypeID As Integer ' FK to ProductType (e.g., 1 for Floor)
        Public Property RawUnitName As String ' Descriptive name, used in pull-down selection

        ' Schema-specific properties (nullable for optional/imported values)
        Public Property BF As Decimal?
        Public Property LF As Decimal?
        Public Property EWPLF As Decimal?
        Public Property SqFt As Decimal?
        Public Property FCArea As Decimal?
        Public Property LumberCost As Decimal?
        Public Property PlateCost As Decimal?
        Public Property ManufLaborCost As Decimal?
        Public Property DesignLabor As Decimal?
        Public Property MGMTLabor As Decimal?
        Public Property JobSuppliesCost As Decimal?
        Public Property ManHours As Decimal?
        Public Property ItemCost As Decimal?
        Public Property OverallCost As Decimal?
        Public Property DeliveryCost As Decimal?
        Public Property TotalSellPrice As Decimal?
        Public Property AvgSPFNo2 As Decimal?

        ' Optional: Keep Fields for extensibility during import (e.g., from CSV)
        Public Property Fields As New Dictionary(Of String, Decimal)
    End Class
End Namespace
