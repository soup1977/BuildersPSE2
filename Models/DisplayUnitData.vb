' Add to Models (new file or existing Models.vb)
Namespace BuildersPSE.Models
    Public Class DisplayUnitData
        Public Property ActualUnit As ActualUnitModel
        Public Property MappingID As Integer
        Public Property ReferencedRawUnitName As String
        Public Property ActualUnitQuantity As Integer
        Public Property LF As Decimal
        Public Property BDFT As Decimal
        Public Property LumberCost As Decimal
        Public Property PlateCost As Decimal
        Public Property ManufLaborCost As Decimal
        Public Property DesignLabor As Decimal
        Public Property MGMTLabor As Decimal
        Public Property JobSuppliesCost As Decimal
        Public Property ManHours As Decimal
        Public Property ItemCost As Decimal
        Public Property OverallCost As Decimal
        Public Property DeliveryCost As Decimal
        Public Property SellPrice As Decimal
        Public Property Margin As Decimal
        Public Property ColorCode As String

        Public Property ActualUnitID As Integer


        Public Property UnitName As String


        Public Property PlanSQFT As Decimal


        Public ReadOnly Property OptionalAdder As Decimal
            Get
                Return If(ActualUnit IsNot Nothing, ActualUnit.OptionalAdder, 1D)
            End Get
        End Property
    End Class
End Namespace
