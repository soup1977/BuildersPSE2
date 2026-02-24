Option Strict On

Imports BuildersPSE2.DataAccess

''' <summary>
''' Form for side-by-side comparison of two price history snapshots
''' </summary>
Public Class frmPriceHistoryCompare

    Private _snapshot1 As PriceHistoryModel
    Private _snapshot2 As PriceHistoryModel
    Private _buildings1 As List(Of PriceHistoryBuildingModel)
    Private _buildings2 As List(Of PriceHistoryBuildingModel)

    Public Sub New(snapshot1 As PriceHistoryModel, snapshot2 As PriceHistoryModel)
        InitializeComponent()
        _snapshot1 = snapshot1
        _snapshot2 = snapshot2
    End Sub

    Private Sub frmPriceHistoryCompare_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = "Price History Comparison"
        LoadComparisonData()
        SetupComparisonGrid()
        DisplayComparison()
    End Sub

    Private Sub LoadComparisonData()
        _buildings1 = PriceHistoryDataAccess.GetBuildingsByHistoryID(_snapshot1.PriceHistoryID)
        _buildings2 = PriceHistoryDataAccess.GetBuildingsByHistoryID(_snapshot2.PriceHistoryID)
    End Sub

    Private Sub SetupComparisonGrid()
        dgvComparison.AutoGenerateColumns = False
        dgvComparison.Columns.Clear()

        dgvComparison.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Metric",
            .HeaderText = "Metric",
            .Width = 180,
            .ReadOnly = True
        })
        dgvComparison.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Snapshot1",
            .HeaderText = $"{_snapshot1.VersionName} ({_snapshot1.CreatedDate:g})",
            .Width = 150,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Alignment = DataGridViewContentAlignment.MiddleRight}
        })
        dgvComparison.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Snapshot2",
            .HeaderText = $"{_snapshot2.VersionName} ({_snapshot2.CreatedDate:g})",
            .Width = 150,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Alignment = DataGridViewContentAlignment.MiddleRight}
        })
        dgvComparison.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Difference",
            .HeaderText = "Difference",
            .Width = 120,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Alignment = DataGridViewContentAlignment.MiddleRight}
        })
        dgvComparison.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PctChange",
            .HeaderText = "% Change",
            .Width = 90,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Alignment = DataGridViewContentAlignment.MiddleRight}
        })
    End Sub

    Private Sub DisplayComparison()
        dgvComparison.Rows.Clear()

        ' Header row
        AddSectionHeader("--- Snapshot Info ---")
        AddTextRow("Version", _snapshot1.VersionName, _snapshot2.VersionName)
        AddTextRow("Date", _snapshot1.CreatedDate.ToString("g"), _snapshot2.CreatedDate.ToString("g"))
        AddTextRow("Created By", _snapshot1.CreatedBy, _snapshot2.CreatedBy)
        AddTextRow("Report Type", _snapshot1.ReportType, _snapshot2.ReportType)

        ' Rollup Values
        AddSectionHeader("--- Rollup Values ---")
        AddCurrencyRow("Extended Sell", _snapshot1.ExtendedSell, _snapshot2.ExtendedSell)
        AddCurrencyRow("Extended Cost", _snapshot1.ExtendedCost, _snapshot2.ExtendedCost)
        AddCurrencyRow("Extended Delivery", _snapshot1.ExtendedDelivery, _snapshot2.ExtendedDelivery)
        AddNumericRow("Extended SQFT", _snapshot1.ExtendedSqft, _snapshot2.ExtendedSqft, "N0")
        AddNumericRow("Extended BDFT", _snapshot1.ExtendedBdft, _snapshot2.ExtendedBdft, "N0")
        AddPercentRow("Margin", _snapshot1.Margin, _snapshot2.Margin)
        AddPercentRow("Margin w/ Delivery", _snapshot1.MarginWithDelivery, _snapshot2.MarginWithDelivery)
        AddCurrencyRow("Price/BDFT", _snapshot1.PricePerBdft, _snapshot2.PricePerBdft, "C4")
        AddCurrencyRow("Price/SQFT", _snapshot1.PricePerSqft, _snapshot2.PricePerSqft)

        ' Futures Adder
        AddSectionHeader("--- Futures Adder ---")
        AddNullableCurrencyRow("Futures Adder/MBF", _snapshot1.FuturesAdderAmt, _snapshot2.FuturesAdderAmt)
        AddNullableCurrencyRow("Futures Adder Total", _snapshot1.FuturesAdderProjTotal, _snapshot2.FuturesAdderProjTotal)
        AddTextRow("Futures Contract", If(_snapshot1.FuturesContractMonth, "—"), If(_snapshot2.FuturesContractMonth, "—"))

        ' Active Lumber
        AddSectionHeader("--- Active Lumber ---")
        AddNumericRow("Floor SPF#2", _snapshot1.ActiveFloorSPFNo2, _snapshot2.ActiveFloorSPFNo2, "N2")
        AddNumericRow("Roof SPF#2", _snapshot1.ActiveRoofSPFNo2, _snapshot2.ActiveRoofSPFNo2, "N2")

        ' Structure
        AddSectionHeader("--- Structure ---")
        AddIntegerRow("Buildings", _snapshot1.TotalBuildingCount, _snapshot2.TotalBuildingCount)
        AddIntegerRow("Total Bldg Qty", _snapshot1.TotalBldgQty, _snapshot2.TotalBldgQty)
        AddIntegerRow("Total Levels", _snapshot1.TotalLevelCount, _snapshot2.TotalLevelCount)
        AddIntegerRow("Total Units", _snapshot1.TotalActualUnitCount, _snapshot2.TotalActualUnitCount)

        ' Building comparison
        If _buildings1.Count > 0 OrElse _buildings2.Count > 0 Then
            AddSectionHeader("--- Building Details ---")
            CompareBuildingDetails()
        End If

        ' Apply color coding
        ApplyColorCoding()
    End Sub

    Private Sub CompareBuildingDetails()
        ' Get all unique building names from both snapshots
        Dim allBuildingNames = _buildings1.Select(Function(b) b.BuildingName).
            Union(_buildings2.Select(Function(b) b.BuildingName)).
            Distinct().OrderBy(Function(n) n).ToList()

        For Each bldgName In allBuildingNames
            Dim bldg1 = _buildings1.FirstOrDefault(Function(b) b.BuildingName = bldgName)
            Dim bldg2 = _buildings2.FirstOrDefault(Function(b) b.BuildingName = bldgName)

            AddTextRow($"  {bldgName}", "", "")

            Dim sell1 = If(bldg1?.ExtendedSell, 0D)
            Dim sell2 = If(bldg2?.ExtendedSell, 0D)
            AddCurrencyRow($"    Ext Sell", sell1, sell2)

            Dim margin1 = If(bldg1?.Margin, 0D)
            Dim margin2 = If(bldg2?.Margin, 0D)
            AddPercentRow($"    Margin", margin1, margin2)
        Next
    End Sub

#Region "Row Helper Methods"

    Private Sub AddSectionHeader(text As String)
        Dim rowIndex = dgvComparison.Rows.Add(text, "", "", "", "")
        dgvComparison.Rows(rowIndex).DefaultCellStyle.BackColor = Color.LightGray
        dgvComparison.Rows(rowIndex).DefaultCellStyle.Font = New Font(dgvComparison.Font, FontStyle.Bold)
    End Sub

    Private Sub AddTextRow(metric As String, value1 As String, value2 As String)
        dgvComparison.Rows.Add(metric, value1, value2, "", "")
    End Sub

    Private Sub AddCurrencyRow(metric As String, value1 As Decimal, value2 As Decimal, Optional format As String = "C2")
        Dim diff = value2 - value1
        Dim pctChange = If(value1 <> 0, diff / value1, 0D)
        dgvComparison.Rows.Add(metric, value1.ToString(format), value2.ToString(format), diff.ToString(format), pctChange.ToString("P2"))
    End Sub

    Private Sub AddNullableCurrencyRow(metric As String, value1 As Decimal?, value2 As Decimal?, Optional format As String = "C2")
        Dim v1 = If(value1.HasValue, value1.Value.ToString(format), "—")
        Dim v2 = If(value2.HasValue, value2.Value.ToString(format), "—")
        Dim diff As String = ""
        Dim pct As String = ""

        If value1.HasValue AndAlso value2.HasValue Then
            Dim d = value2.Value - value1.Value
            diff = d.ToString(format)
            pct = If(value1.Value <> 0, (d / value1.Value).ToString("P2"), "—")
        End If

        dgvComparison.Rows.Add(metric, v1, v2, diff, pct)
    End Sub

    Private Sub AddNumericRow(metric As String, value1 As Decimal, value2 As Decimal, format As String)
        Dim diff = value2 - value1
        Dim pctChange = If(value1 <> 0, diff / value1, 0D)
        dgvComparison.Rows.Add(metric, value1.ToString(format), value2.ToString(format), diff.ToString(format), pctChange.ToString("P2"))
    End Sub

    Private Sub AddPercentRow(metric As String, value1 As Decimal, value2 As Decimal)
        Dim diff = value2 - value1
        ' For percentages, show absolute difference in percentage points
        dgvComparison.Rows.Add(metric, value1.ToString("P2"), value2.ToString("P2"), diff.ToString("P2"), "")
    End Sub

    Private Sub AddIntegerRow(metric As String, value1 As Integer, value2 As Integer)
        Dim diff = value2 - value1
        Dim pctChange = If(value1 <> 0, CDec(diff) / value1, 0D)
        dgvComparison.Rows.Add(metric, value1.ToString(), value2.ToString(), diff.ToString(), pctChange.ToString("P2"))
    End Sub

    Private Sub ApplyColorCoding()
        For Each row As DataGridViewRow In dgvComparison.Rows
            ' Skip header rows
            Dim metric = row.Cells("Metric").Value?.ToString()
            If String.IsNullOrEmpty(metric) OrElse metric.StartsWith("---") Then Continue For

            ' Color the % Change column based on direction
            Dim pctCell = row.Cells("PctChange")
            Dim diffCell = row.Cells("Difference")

            If pctCell.Value IsNot Nothing AndAlso Not String.IsNullOrEmpty(pctCell.Value.ToString()) Then
                Dim pctText = pctCell.Value.ToString().Replace("%", "").Trim()
                Dim pctValue As Decimal
                If Decimal.TryParse(pctText, pctValue) Then
                    ' For costs, increases are bad (red), decreases are good (green)
                    ' For margin/sell, increases are good (green), decreases are bad (red)
                    Dim isPositiveGood = metric.Contains("Margin") OrElse metric.Contains("Sell") OrElse metric.Contains("Price")

                    If Math.Abs(pctValue) > 0.01D Then ' More than 1% change
                        If pctValue > 0 Then
                            pctCell.Style.ForeColor = If(isPositiveGood, Color.DarkGreen, Color.DarkRed)
                            diffCell.Style.ForeColor = If(isPositiveGood, Color.DarkGreen, Color.DarkRed)
                        Else
                            pctCell.Style.ForeColor = If(isPositiveGood, Color.DarkRed, Color.DarkGreen)
                            diffCell.Style.ForeColor = If(isPositiveGood, Color.DarkRed, Color.DarkGreen)
                        End If
                    End If
                End If
            End If
        Next
    End Sub

#End Region

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        MessageBox.Show("Export to Excel coming soon.", "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

End Class