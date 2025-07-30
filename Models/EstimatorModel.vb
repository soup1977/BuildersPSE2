Option Strict On

Namespace BuildersPSE.Models
    Public Class EstimatorModel
        Public Property EstimatorID As Integer
        Public Property EstimatorName As String
        Public Overrides Function ToString() As String
            Return EstimatorName
        End Function
    End Class
End Namespace
