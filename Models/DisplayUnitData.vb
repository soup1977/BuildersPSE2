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

        Public ReadOnly Property ActualUnitID As Integer
            Get
                Return If(ActualUnit IsNot Nothing, ActualUnit.ActualUnitID, 0)
            End Get
        End Property

        Public ReadOnly Property UnitName As String
            Get
                Return If(ActualUnit IsNot Nothing, ActualUnit.UnitName, String.Empty)
            End Get
        End Property

        Public ReadOnly Property PlanSQFT As Decimal
            Get
                Return If(ActualUnit IsNot Nothing, ActualUnit.PlanSQFT, 0D)
            End Get
        End Property

        Public ReadOnly Property OptionalAdder As Decimal
            Get
                Return If(ActualUnit IsNot Nothing, ActualUnit.OptionalAdder, 1D)
            End Get
        End Property
    End Class
End Namespace
