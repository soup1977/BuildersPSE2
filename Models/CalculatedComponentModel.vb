Option Strict On

Namespace BuildersPSE.Models
    Public Class CalculatedComponentModel
        Public Property ComponentID As Integer ' PK, auto-incremented
        Public Property ProjectID As Integer ' FK to Projects
        Public Property ActualUnitID As Integer ' FK to ActualUnits
        Public Property ComponentType As String ' e.g., "Lumber/SQFT"
        Public Property Value As Decimal ' Calculated value
    End Class
End Namespace
