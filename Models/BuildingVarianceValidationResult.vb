''' <summary>
''' Contains the results of building variance validation checks.
''' Used to determine if building variance data is complete enough to display.
''' </summary>
Public Class BuildingVarianceValidationResult

    ''' <summary>Building quantity from the Buildings table.</summary>
    Public Property BldgQty As Integer = 0

    ''' <summary>Count of distinct levels defined for this building.</summary>
    Public Property DistinctLevelCount As Integer = 0

    ''' <summary>Expected total: DistinctLevelCount × BldgQty.</summary>
    Public Property ExpectedCount As Integer = 0

    ''' <summary>Actual count of LevelActuals records for this building.</summary>
    Public Property LevelActualsCount As Integer = 0

    ''' <summary>Count of LevelActuals that have matching LevelActualsBistrack records.</summary>
    Public Property BistrackMatchedCount As Integer = 0

    ''' <summary>True if all counts match and variance can be displayed.</summary>
    Public Property IsValid As Boolean = False

    ''' <summary>
    ''' Gets a human-readable status message for display.
    ''' </summary>
    Public Function GetStatusMessage() As String
        If IsValid Then
            Return $"Complete: {ExpectedCount} of {ExpectedCount} levels matched"
        End If

        If ExpectedCount = 0 Then
            Return "No levels defined for this building"
        End If

        If LevelActualsCount = 0 Then
            Return $"No actuals imported (expecting {ExpectedCount})"
        End If

        If LevelActualsCount < ExpectedCount Then
            Return $"Incomplete actuals: {LevelActualsCount} of {ExpectedCount} imported"
        End If

        If BistrackMatchedCount < LevelActualsCount Then
            Return $"Incomplete Bistrack data: {BistrackMatchedCount} of {LevelActualsCount} matched"
        End If

        Return $"Data mismatch: Expected={ExpectedCount}, Actuals={LevelActualsCount}, Bistrack={BistrackMatchedCount}"
    End Function

End Class
