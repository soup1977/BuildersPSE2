''' <summary>
''' Model representing LevelActuals data for variance comparison.
''' </summary>
Public Class LevelActualModel

    Public Property ActualID As Integer = 0
    Public Property BisTrackSalesOrder As String = String.Empty
    Public Property MiTekJobNumber As String = String.Empty
    Public Property SellPrice As Decimal = 0D
    Public Property TotalCost As Decimal = 0D
    Public Property BDFT As Decimal = 0D
    Public Property LumberCost As Decimal = 0D
    Public Property PlateCost As Decimal = 0D
    Public Property ManufLaborCost As Decimal = 0D
    Public Property ManufLaborMH As Decimal = 0D
    Public Property ItemCost As Decimal = 0D
    Public Property DeliveryCost As Decimal = 0D
    Public Property MiscLaborCost As Decimal = 0D
    Public Property AvgSPF2 As Decimal = 0D

    ''' <summary>
    ''' Calculates margin percentage: (SellPrice - TotalCost) / SellPrice
    ''' </summary>
    Public ReadOnly Property MarginPercent As Decimal
        Get
            If SellPrice = 0 Then Return 0D
            Return (SellPrice - TotalCost) / SellPrice
        End Get
    End Property

    ''' <summary>
    ''' Converts the model to a dictionary for grid population.
    ''' </summary>
    Public Function ToDictionary() As Dictionary(Of String, Decimal)
        Return New Dictionary(Of String, Decimal) From {
            {"Sell Price", SellPrice},
            {"BDFT", BDFT},
            {"AvgSPF2", AvgSPF2},
            {"LumberCost", LumberCost},
            {"PlateCost", PlateCost},
            {"ManufLaborCost", ManufLaborCost},
            {"ManufLaborMH", ManufLaborMH},
            {"ItemCost", ItemCost},
            {"DeliveryCost", DeliveryCost},
            {"MiscLaborCost", MiscLaborCost},
            {"Total Cost", TotalCost}
        }
    End Function

End Class