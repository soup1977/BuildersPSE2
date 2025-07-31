Option Strict On

Namespace BuildersPSE.Models
    Public Class ActualToLevelMappingModel
        Public Property MappingID As Integer ' PK, auto-incremented
        Public Property LevelID As Integer ' FK to Levels
        Public Property VersionID As Integer ' FK to Projects for context
        Public Property Quantity As Integer ' Number of this unit in the level
        Public Property ActualUnitID As Integer ' FK to ActualUnits
        Public Property ActualUnit As ActualUnitModel ' Associated ActualUnit details (populated in DataAccess)

    End Class
End Namespace
