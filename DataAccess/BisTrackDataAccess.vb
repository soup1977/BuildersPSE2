Imports System.Data.SqlClient
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Utilities

''' <summary>
''' Data access methods for BisTrack-related operations.
''' </summary>
Public Class BisTrackDataAccess

    ''' <summary>
    ''' Gets the MFGLaborAVOCost value from Configuration table.
    ''' Used to calculate ManufLaborMH = TaskActLabCost / MFGLaborAVOCost
    ''' </summary>
    ''' <returns>The MFGLaborAVOCost value, or 1 if not found (to prevent division by zero).</returns>
    Public Shared Function GetMFGLaborAVOCost() As Decimal
        Try
            Using reader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectMFGLaborAVOCost, Nothing)
                If reader.Read() AndAlso Not reader.IsDBNull(0) Then
                    Return CDec(reader("Value"))
                End If
            End Using
        Catch ex As Exception
            UIHelper.Add($"Error getting MFGLaborAVOCost: {ex.Message}")
        End Try
        Return 1D ' Default to 1 to prevent division by zero
    End Function

    ''' <summary>
    ''' Gets Bistrack actuals data for a specific level.
    ''' </summary>
    ''' <param name="levelID">The level ID.</param>
    ''' <param name="versionID">The version ID.</param>
    ''' <returns>List of BisTrack actual records for the level.</returns>
    Public Shared Function GetBistrackActualsForLevel(levelID As Integer, versionID As Integer) As List(Of BisTrackActualModel)
        Dim results As New List(Of BisTrackActualModel)()
        Dim mfgLaborRate As Decimal = GetMFGLaborAVOCost()

        Try
            Using reader = SqlConnectionManager.Instance.ExecuteReader(
                Queries.SelectBistrackActualsForLevel,
                {New SqlParameter("@LevelID", levelID), New SqlParameter("@VersionID", versionID)})

                While reader.Read()
                    Dim model As New BisTrackActualModel()
                    model.SalesOrderNumber = SafeString(reader, "SalesOrderNumber")
                    model.SellPrice = SafeDecimal(reader, "TotalSellPrice")
                    model.TotalCost = SafeDecimal(reader, "TotalCostPrice")
                    model.BDFT = SafeDecimal(reader, "TaskActInvBDFT")
                    model.LumberCost = SafeDecimal(reader, "TaskActLumCost")
                    model.PlateCost = SafeDecimal(reader, "TaskActPltCost")
                    model.ManufLaborCost = SafeDecimal(reader, "TaskActLabCost")
                    model.ManufLaborMH = If(mfgLaborRate > 0, model.ManufLaborCost / mfgLaborRate, 0D)
                    model.ItemCost = SafeDecimal(reader, "TaskActItemCost")
                    model.DeliveryCost = SafeDecimal(reader, "TaskActDelCost")
                    model.MiscLaborCost = SafeDecimal(reader, "TaskActHubCost") +
                                          SafeDecimal(reader, "TaskActJobSupplies") +
                                          SafeDecimal(reader, "TaskActManageCost")
                    model.AvgSPF2 = 0D ' Future use
                    results.Add(model)
                End While
            End Using
        Catch ex As Exception
            UIHelper.Add($"Error getting Bistrack actuals for level {levelID}: {ex.Message}")
        End Try

        Return results
    End Function

    ''' <summary>
    ''' Gets aggregated Bistrack actuals data for a building.
    ''' </summary>
    Public Shared Function GetBistrackActualsForBuilding(buildingID As Integer, versionID As Integer) As BisTrackActualModel
        Dim result As New BisTrackActualModel()
        Dim mfgLaborRate As Decimal = GetMFGLaborAVOCost()
        Dim recordCount As Integer = 0

        Try
            Using reader = SqlConnectionManager.Instance.ExecuteReader(
                Queries.SelectBistrackActualsForBuilding,
                {New SqlParameter("@BuildingID", buildingID), New SqlParameter("@VersionID", versionID)})

                While reader.Read()
                    recordCount += 1
                    result.SellPrice += SafeDecimal(reader, "TotalSellPrice")
                    result.TotalCost += SafeDecimal(reader, "TotalCostPrice")
                    result.BDFT += SafeDecimal(reader, "TaskActInvBDFT")
                    result.LumberCost += SafeDecimal(reader, "TaskActLumCost")
                    result.PlateCost += SafeDecimal(reader, "TaskActPltCost")
                    result.ManufLaborCost += SafeDecimal(reader, "TaskActLabCost")
                    result.ItemCost += SafeDecimal(reader, "TaskActItemCost")
                    result.DeliveryCost += SafeDecimal(reader, "TaskActDelCost")
                    result.MiscLaborCost += SafeDecimal(reader, "TaskActHubCost") +
                                            SafeDecimal(reader, "TaskActJobSupplies") +
                                            SafeDecimal(reader, "TaskActManageCost")
                End While
            End Using

            ' Calculate ManufLaborMH from total ManufLaborCost
            result.ManufLaborMH = If(mfgLaborRate > 0, result.ManufLaborCost / mfgLaborRate, 0D)
            result.LevelCount = recordCount

        Catch ex As Exception
            UIHelper.Add($"Error getting Bistrack actuals for building {buildingID}: {ex.Message}")
        End Try

        Return result
    End Function

    Private Shared Function SafeDecimal(reader As IDataReader, columnName As String) As Decimal
        If reader(columnName) Is DBNull.Value Then Return 0D
        Return CDec(reader(columnName))
    End Function

    Private Shared Function SafeString(reader As IDataReader, columnName As String) As String
        If reader(columnName) Is DBNull.Value Then Return String.Empty
        Return reader(columnName).ToString().Trim()
    End Function


    ''' <summary>
    ''' Validates whether building variance data is complete and can be displayed.
    ''' Returns True only if: ExpectedCount = LevelActualsCount = BistrackMatchedCount
    ''' </summary>
    ''' <param name="buildingID">The building ID to validate.</param>
    ''' <param name="versionID">The version ID.</param>
    ''' <returns>True if all counts match and variance can be shown; otherwise False.</returns>
    Public Shared Function CanShowBuildingVariance(buildingID As Integer, versionID As Integer) As Boolean
        If buildingID <= 0 OrElse versionID <= 0 Then Return False

        Try
            Using reader = SqlConnectionManager.Instance.ExecuteReader(
                Queries.SelectBuildingVarianceValidation,
                {New SqlParameter("@BuildingID", buildingID), New SqlParameter("@VersionID", versionID)})

                If reader.Read() Then
                    Dim bldgQty As Integer = If(reader.IsDBNull(reader.GetOrdinal("BldgQty")), 0, CInt(reader("BldgQty")))
                    Dim distinctLevelCount As Integer = If(reader.IsDBNull(reader.GetOrdinal("DistinctLevelCount")), 0, CInt(reader("DistinctLevelCount")))
                    Dim levelActualsCount As Integer = If(reader.IsDBNull(reader.GetOrdinal("LevelActualsCount")), 0, CInt(reader("LevelActualsCount")))
                    Dim bistrackMatchedCount As Integer = If(reader.IsDBNull(reader.GetOrdinal("BistrackMatchedCount")), 0, CInt(reader("BistrackMatchedCount")))

                    Dim expectedCount As Integer = distinctLevelCount * bldgQty

                    ' All three counts must match and be greater than zero
                    Dim isValid As Boolean = (expectedCount > 0) AndAlso
                                             (expectedCount = levelActualsCount) AndAlso
                                             (levelActualsCount = bistrackMatchedCount)

                    UIHelper.Add($"Building {buildingID} variance validation: Expected={expectedCount}, LevelActuals={levelActualsCount}, BistrackMatched={bistrackMatchedCount}, CanShow={isValid}")

                    Return isValid
                End If
            End Using
        Catch ex As Exception
            UIHelper.Add($"Error validating building variance for BuildingID {buildingID}: {ex.Message}")
        End Try

        Return False
    End Function

    ''' <summary>
    ''' Gets detailed variance validation info for debugging/display purposes.
    ''' </summary>
    Public Shared Function GetBuildingVarianceValidationInfo(buildingID As Integer, versionID As Integer) As BuildingVarianceValidationResult
        Dim result As New BuildingVarianceValidationResult()

        If buildingID <= 0 OrElse versionID <= 0 Then Return result

        Try
            Using reader = SqlConnectionManager.Instance.ExecuteReader(
                Queries.SelectBuildingVarianceValidation,
                {New SqlParameter("@BuildingID", buildingID), New SqlParameter("@VersionID", versionID)})

                If reader.Read() Then
                    result.BldgQty = If(reader.IsDBNull(reader.GetOrdinal("BldgQty")), 0, CInt(reader("BldgQty")))
                    result.DistinctLevelCount = If(reader.IsDBNull(reader.GetOrdinal("DistinctLevelCount")), 0, CInt(reader("DistinctLevelCount")))
                    result.LevelActualsCount = If(reader.IsDBNull(reader.GetOrdinal("LevelActualsCount")), 0, CInt(reader("LevelActualsCount")))
                    result.BistrackMatchedCount = If(reader.IsDBNull(reader.GetOrdinal("BistrackMatchedCount")), 0, CInt(reader("BistrackMatchedCount")))
                    result.ExpectedCount = result.DistinctLevelCount * result.BldgQty
                    result.IsValid = (result.ExpectedCount > 0) AndAlso
                                     (result.ExpectedCount = result.LevelActualsCount) AndAlso
                                     (result.LevelActualsCount = result.BistrackMatchedCount)
                End If
            End Using
        Catch ex As Exception
            UIHelper.Add($"Error getting building variance validation info: {ex.Message}")
        End Try

        Return result
    End Function


End Class
