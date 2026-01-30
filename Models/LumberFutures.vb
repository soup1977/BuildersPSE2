Namespace BuildersPSE.Models
    Public Class LumberFutures
        Public Property ContractMonth As String
        Public Property LumberFutureID As Integer
        Public Property VersionID As Integer

        Public Property PriorSettle As Decimal?
        Public Property PullDate As DateTime
        Public Property Active As Boolean

        Public Overrides Function ToString() As String
            Dim settleStr = If(PriorSettle.HasValue, PriorSettle.Value.ToString("N2"), "—")
            Dim pullDateStr = PullDate.ToString("MM/dd/yyyy")  ' Date only, no time
            Return $"{ContractMonth}    ${settleStr}    [{pullDateStr}]"
        End Function
    End Class
End Namespace
