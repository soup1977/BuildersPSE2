Option Strict On
Imports System.Collections.Generic

Namespace BuildersPSE.Models
    Public Class ActualUnitModel
        Public Property ActualUnitID As Integer ' PK, auto-incremented
        Public Property UnitName As String ' Descriptive name
        Public Property PlanSQFT As Decimal ' User-entered square footage
        Public Property UnitType As String ' e.g., "Res", "NonRes"
        Public Property OptionalAdder As Decimal = 1D ' Default to 1.0, multiplier for calculations
        Public Property RawUnitID As Integer ' FK to RawUnits
        Public Property VersionID As Integer ' FK to Projects
        Public Property ProductTypeID As Integer ' FK to ProductType (e.g., 1 for Floor)
        Public Property ColorCode As String = String.Empty

        Public Property MarginPercent As Decimal = 0D ' Default to 0%, used in calculations
        Public Property ReferencedRawUnitName As String ' Joined from RawUnits for display/reference
        ' Collections
        Public Property CalculatedComponents As New List(Of CalculatedComponentModel) ' Runtime calculated values like Lumber/SQFT
    End Class
End Namespace
