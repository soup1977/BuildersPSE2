Option Strict On

Namespace BuildersPSE.Models
    Public Class ProjectProductSettingsModel
        Public Property SettingID As Integer
        Public Property VersionID As Integer ' FK to Projects
        Public Property ProductTypeID As Integer ' FK to ProductType
        Public Property MarginPercent As Decimal? ' Override, apply if set
        Public Property LumberAdder As Decimal? ' Override, apply only if user-entered (>0)
    End Class
End Namespace
