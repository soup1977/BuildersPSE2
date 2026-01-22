Option Strict On

Namespace BuildersPSE.Models
    Public Class ProjectVersionStatus
        Public Property ProjVersionStatusID As Integer
        Public Property ProjVersionStatus As String
        Public Overrides Function ToString() As String
            Return ProjVersionStatus
        End Function
    End Class
End Namespace
