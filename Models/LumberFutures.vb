Namespace BuildersPSE.Models
    Public Class LumberFutures
        Public Property ContractMonth As String
        Public Property ID As Integer
        Public Property PriorSettle As Decimal?
        Public Overrides Function ToString() As String
            Return $"{ContractMonth} – ${PriorSettle.GetValueOrDefault():F2}"
        End Function
    End Class
End Namespace
