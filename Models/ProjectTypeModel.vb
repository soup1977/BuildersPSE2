Option Strict On

Namespace BuildersPSE.Models
    Public Class ProjectTypeModel
        Public Property ProjectTypeID As Integer
        Public Property ProjectTypeName As String
        Public Property Description As String
        Public Overrides Function ToString() As String
            Return ProjectTypeName
        End Function
    End Class
End Namespace
