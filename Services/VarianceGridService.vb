Imports System.Data.SqlClient
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Utilities

''' <summary>
''' Service for building variance comparison DataTables for levels and buildings.
''' </summary>
Public Class VarianceGridService
    Private ReadOnly _versionID As Integer

    ''' <summary>
    ''' Standard metrics used in variance comparisons.
    ''' </summary>
    Private Shared ReadOnly MetricKeys As String() = {
        "Sell Price", "BDFT", "AvgSPF2", "LumberCost", "PlateCost",
        "ManufLaborCost", "ManufLaborMH", "ItemCost", "DeliveryCost",
        "MiscLaborCost", "Total Cost"
    }

    Public Sub New(versionID As Integer)
        _versionID = versionID
    End Sub

    ''' <summary>
    ''' Builds a variance DataTable for a specific level with estimate vs actuals columns.
    ''' </summary>
    Public Function BuildLevelVarianceTable(levelID As Integer) As DataTable
        If levelID <= 0 OrElse _versionID <= 0 Then Return Nothing

        Dim dt As New DataTable()
        Dim estimateData = GetLevelEstimateData(levelID)
        If estimateData Is Nothing Then Return Nothing

        InitializeVarianceColumns(dt, estimateData)
        AddLevelActualsColumns(dt, levelID)
        AddMarginRow(dt)

        Return dt
    End Function

    ''' <summary>
    ''' Builds a variance DataTable for a specific building with estimate vs actual columns.
    ''' </summary>
    Public Function BuildBuildingVarianceTable(buildingID As Integer) As DataTable
        If buildingID <= 0 OrElse _versionID <= 0 Then Return Nothing

        Dim dt As New DataTable()
        Dim estimateData = GetBuildingEstimateData(buildingID)
        If estimateData Is Nothing Then Return Nothing

        ' Initialize with Metric, Estimate, and Actual columns
        dt.Columns.Add("Metric", GetType(String))
        dt.Columns.Add("Estimate", GetType(Decimal))
        dt.Columns.Add("Actual", GetType(Decimal))

        Dim actualData = GetBuildingActualData(buildingID)

        For Each kvp In estimateData
            Dim row = dt.NewRow()
            row("Metric") = kvp.Key
            row("Estimate") = kvp.Value
            row("Actual") = If(actualData.ContainsKey(kvp.Key), actualData(kvp.Key), 0D)
            dt.Rows.Add(row)
        Next

        AddMarginRow(dt)
        Return dt
    End Function

    Private Function GetLevelEstimateData(levelID As Integer) As Dictionary(Of String, Decimal)
        Using reader = SqlConnectionManager.Instance.ExecuteReader(
            Queries.SelectLevelEstimateWithProductType,
            {New SqlParameter("@LevelID", levelID), New SqlParameter("@VersionID", _versionID)})

            Dim estTable As New DataTable()
            estTable.Load(reader)
            If estTable.Rows.Count = 0 Then Return Nothing

            Dim row = estTable.Rows(0)
            Dim productTypeName = row("ProductTypeName").ToString()
            Dim avgSPF = LumberDataAccess.GetActiveSPFNo2ByProductType(_versionID, productTypeName)

            Return New Dictionary(Of String, Decimal) From {
                {"Sell Price", CDec(row("OverallPrice"))},
                {"BDFT", CDec(row("OverallBDFT"))},
                {"AvgSPF2", avgSPF},
                {"LumberCost", CDec(row("LumberCost"))},
                {"PlateCost", CDec(row("PlateCost"))},
                {"ManufLaborCost", CDec(row("LaborCost"))},
                {"ManufLaborMH", CDec(row("LaborMH"))},
                {"ItemCost", CDec(row("ItemsCost"))},
                {"DeliveryCost", CDec(row("DeliveryCost"))},
                {"MiscLaborCost", CDec(row("DesignCost")) + CDec(row("MGMTCost")) + CDec(row("JobSuppliesCost"))},
                {"Total Cost", CDec(row("OverallCost"))}
            }
        End Using
    End Function

    Private Function GetBuildingEstimateData(buildingID As Integer) As Dictionary(Of String, Decimal)
        Dim bldgQty As Integer = 1
        Using r = SqlConnectionManager.Instance.ExecuteReader(
            "SELECT BldgQty FROM Buildings WHERE BuildingID = @BuildingID AND VersionID = @VersionID",
            {New SqlParameter("@BuildingID", buildingID), New SqlParameter("@VersionID", _versionID)})
            If r.Read() Then bldgQty = CInt(r("BldgQty"))
        End Using

        Dim est As New Dictionary(Of String, Decimal) From {
            {"Sell Price", 0D}, {"BDFT", 0D}, {"Level Count", 0D}, {"AvgSPF2", 0D},
            {"LumberCost", 0D}, {"PlateCost", 0D}, {"ManufLaborCost", 0D},
            {"ManufLaborMH", 0D}, {"ItemCost", 0D}, {"DeliveryCost", 0D},
            {"MiscLaborCost", 0D}, {"Total Cost", 0D}
        }

        Dim levelCount As Integer = 0
        Dim weightedEstSPF As Decimal = 0D, totalEstBDFT As Decimal = 0D

        Using r = SqlConnectionManager.Instance.ExecuteReader(
            Queries.SelectLevelsByBuilding & " AND l.VersionID = @VersionID",
            {New SqlParameter("@BuildingID", buildingID), New SqlParameter("@VersionID", _versionID)})

            While r.Read()
                levelCount += 1
                Dim bdft As Decimal = SafeDecimal(r, "OverallBDFT")
                Dim spf As Decimal = LumberDataAccess.GetActiveSPFNo2ByProductType(_versionID, r("ProductTypeName").ToString())

                weightedEstSPF += spf * bdft
                totalEstBDFT += bdft

                est("Sell Price") += SafeDecimal(r, "OverallPrice")
                est("BDFT") += bdft
                est("LumberCost") += SafeDecimal(r, "LumberCost")
                est("PlateCost") += SafeDecimal(r, "PlateCost")
                est("ManufLaborCost") += SafeDecimal(r, "LaborCost")
                est("ManufLaborMH") += SafeDecimal(r, "LaborMH")
                est("ItemCost") += SafeDecimal(r, "ItemsCost")
                est("DeliveryCost") += SafeDecimal(r, "DeliveryCost")
                est("MiscLaborCost") += SafeDecimal(r, "DesignCost") + SafeDecimal(r, "MGMTCost") + SafeDecimal(r, "JobSuppliesCost")
                est("Total Cost") += SafeDecimal(r, "OverallCost")
            End While
        End Using

        If levelCount = 0 Then Return Nothing
        If totalEstBDFT > 0 Then est("AvgSPF2") = weightedEstSPF / totalEstBDFT

        ' Multiply by building quantity (except AvgSPF2)
        For Each k In est.Keys.Where(Function(x) x <> "AvgSPF2").ToList()
            est(k) *= bldgQty
        Next
        est("Level Count") = levelCount * bldgQty

        Return est
    End Function

    Private Function GetBuildingActualData(buildingID As Integer) As Dictionary(Of String, Decimal)
        Dim act As New Dictionary(Of String, Decimal) From {
            {"Sell Price", 0D}, {"BDFT", 0D}, {"Level Count", 0D}, {"AvgSPF2", 0D},
            {"LumberCost", 0D}, {"PlateCost", 0D}, {"ManufLaborCost", 0D},
            {"ManufLaborMH", 0D}, {"ItemCost", 0D}, {"DeliveryCost", 0D},
            {"MiscLaborCost", 0D}, {"Total Cost", 0D}
        }

        Dim actualLevelCount As Integer = 0
        Dim weightedActSPF As Decimal = 0D, totalActBDFT As Decimal = 0D

        Dim sql As String = "SELECT la.* FROM LevelActuals la WHERE la.BuildingID = @BuildingID AND la.VersionID = @VersionID AND la.StageType = 1"

        Using r = SqlConnectionManager.Instance.ExecuteReader(sql,
            {New SqlParameter("@BuildingID", buildingID), New SqlParameter("@VersionID", _versionID)})

            While r.Read()
                actualLevelCount += 1
                Dim bdft As Decimal = SafeDecimal(r, "ActualBDFT")
                Dim spf As Decimal = SafeDecimal(r, "AvgSPFNo2Actual")

                totalActBDFT += bdft
                weightedActSPF += spf * bdft

                act("Sell Price") += SafeDecimal(r, "ActualSoldAmount")
                act("BDFT") += bdft
                act("LumberCost") += SafeDecimal(r, "ActualLumberCost")
                act("PlateCost") += SafeDecimal(r, "ActualPlateCost")
                act("ManufLaborCost") += SafeDecimal(r, "ActualManufLaborCost")
                act("ManufLaborMH") += SafeDecimal(r, "ActualManufMH")
                act("ItemCost") += SafeDecimal(r, "ActualItemCost")
                act("DeliveryCost") += SafeDecimal(r, "ActualDeliveryCost")
                act("MiscLaborCost") += SafeDecimal(r, "ActualMiscLaborCost")
                act("Total Cost") += SafeDecimal(r, "ActualTotalCost")
            End While
        End Using

        If totalActBDFT > 0 Then act("AvgSPF2") = weightedActSPF / totalActBDFT
        act("Level Count") = actualLevelCount

        Return act
    End Function

    Private Sub InitializeVarianceColumns(dt As DataTable, estimateData As Dictionary(Of String, Decimal))
        dt.Columns.Add("Metric", GetType(String))
        dt.Columns.Add("Estimate", GetType(Decimal))

        For Each kvp In estimateData
            Dim row = dt.NewRow()
            row("Metric") = kvp.Key
            row("Estimate") = kvp.Value
            dt.Rows.Add(row)
        Next
    End Sub

    Private Sub AddLevelActualsColumns(dt As DataTable, levelID As Integer)
        Using reader = SqlConnectionManager.Instance.ExecuteReader(Queries.SelectAllActualsForLevel,
            {New SqlParameter("@LevelID", levelID), New SqlParameter("@VersionID", _versionID)})

            Dim indexDesign As Integer = 1
            Dim indexInvoice As Integer = 1

            While reader.Read()
                Dim stageType As Integer = CInt(reader("StageType"))
                Dim miTekJob As String = SafeString(reader, "MiTekJobNumber")
                Dim bisTrackSO As String = SafeString(reader, "BisTrackSalesOrder")

                ' Build column header
                Dim header As String
                If stageType = 1 Then
                    header = If(String.IsNullOrEmpty(miTekJob), $"Design{indexDesign}", $"Des - {miTekJob}")
                    indexDesign += 1
                Else
                    header = If(String.IsNullOrEmpty(bisTrackSO), $"Invoice{indexInvoice}", $"BT - {bisTrackSO}")
                    indexInvoice += 1
                End If

                ' Add column if not exists
                If Not dt.Columns.Contains(header) Then
                    dt.Columns.Add(header, GetType(Decimal))
                End If

                ' Populate data
                Dim data As New Dictionary(Of String, Decimal) From {
                    {"Sell Price", SafeDecimal(reader, "ActualSoldAmount")},
                    {"BDFT", SafeDecimal(reader, "ActualBDFT")},
                    {"AvgSPF2", SafeDecimal(reader, "AvgSPFNo2Actual")},
                    {"LumberCost", SafeDecimal(reader, "ActualLumberCost")},
                    {"PlateCost", SafeDecimal(reader, "ActualPlateCost")},
                    {"ManufLaborCost", SafeDecimal(reader, "ActualManufLaborCost")},
                    {"ManufLaborMH", SafeDecimal(reader, "ActualManufMH")},
                    {"ItemCost", SafeDecimal(reader, "ActualItemCost")},
                    {"DeliveryCost", SafeDecimal(reader, "ActualDeliveryCost")},
                    {"MiscLaborCost", SafeDecimal(reader, "ActualMiscLaborCost")},
                    {"Total Cost", SafeDecimal(reader, "ActualTotalCost")}
                }

                For Each row As DataRow In dt.Rows
                    Dim metric = row("Metric").ToString()
                    If data.ContainsKey(metric) Then
                        row(header) = data(metric)
                    End If
                Next
            End While
        End Using
    End Sub

    Private Sub AddMarginRow(dt As DataTable)
        Dim marginRow = dt.NewRow()
        marginRow("Metric") = "Margin %"
        dt.Rows.InsertAt(marginRow, dt.Rows.Count)

        For i = 1 To dt.Columns.Count - 1
            Dim colName = dt.Columns(i).ColumnName
            Dim sellPrice = GetMetricValue(dt, "Sell Price", colName)
            Dim totalCost = GetMetricValue(dt, "Total Cost", colName)
            marginRow(colName) = If(sellPrice = 0, 0D, (sellPrice - totalCost) / sellPrice)
        Next
    End Sub

    Private Function GetMetricValue(dt As DataTable, metric As String, colName As String) As Decimal
        Dim row = dt.AsEnumerable().FirstOrDefault(Function(r) r.Field(Of String)("Metric") = metric)
        If row IsNot Nothing AndAlso Not row.IsNull(colName) Then
            Return CDec(row(colName))
        End If
        Return 0D
    End Function

    Private Shared Function SafeDecimal(reader As IDataReader, columnName As String) As Decimal
        If reader(columnName) Is DBNull.Value Then Return 0D
        Return CDec(reader(columnName))
    End Function

    Private Shared Function SafeString(reader As IDataReader, columnName As String) As String
        If reader(columnName) Is DBNull.Value Then Return String.Empty
        Return reader(columnName).ToString().Trim()
    End Function
End Class