Option Strict On

Namespace BuildersPSE.Models
    Public Class CustomerModel
        Public Property CustomerID As Integer
        Public Property CustomerName As String
        Public Property CustomerTypeID As Integer?
        Public Property CustomerType As CustomerTypeModel
    End Class
End Namespace

