''' <summary>
''' Formats variance DataGridViews with consistent styling and color coding.
''' </summary>
Public Class VarianceGridFormatter
    Private Const VARIANCE_THRESHOLD As Decimal = 0.05D

    ' Column header prefixes for styling
    Private Const DESIGN_PREFIX As String = "Des-"
    Private Const BISTRACK_PREFIX As String = "BT-"

    ''' <summary>
    ''' Applies standard formatting to a variance grid.
    ''' </summary>
    Public Sub FormatVarianceGrid(grid As DataGridView, Optional freezeEstimateColumn As Boolean = True)
        If grid Is Nothing Then Exit Sub
        If grid.DataSource Is Nothing Then Exit Sub
        If grid.Columns.Count = 0 Then Exit Sub
        If Not grid.Columns.Contains("Metric") Then Exit Sub

        ' CRITICAL: Enable custom header styling (required for header colors to show)
        grid.EnableHeadersVisualStyles = False
        grid.ColumnHeadersDefaultCellStyle.BackColor = SystemColors.Control
        grid.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText
        grid.ColumnHeadersDefaultCellStyle.Font = New Font(grid.Font, FontStyle.Bold)

        ' Configure grid appearance
        grid.AllowUserToAddRows = False
        grid.AllowUserToDeleteRows = False
        grid.ReadOnly = True
        grid.RowHeadersVisible = False
        grid.SelectionMode = DataGridViewSelectionMode.CellSelect
        grid.BackgroundColor = SystemColors.Window
        grid.BorderStyle = BorderStyle.Fixed3D
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal
        grid.GridColor = SystemColors.ControlLight

        ApplyCellFormats(grid)
        ApplyColumnStyles(grid)
        ApplyColumnHeaderStyles(grid)
        ApplyColorCoding(grid)

        If freezeEstimateColumn AndAlso grid.Columns.Count > 1 Then
            grid.Columns(1).Frozen = True
        End If

        grid.Refresh()
    End Sub

    Private Sub ApplyCellFormats(grid As DataGridView)
        If Not grid.Columns.Contains("Metric") Then Exit Sub

        For Each row As DataGridViewRow In grid.Rows
            If row.IsNewRow Then Continue For

            Dim metricCell = row.Cells("Metric")
            If metricCell Is Nothing OrElse metricCell.Value Is Nothing Then Continue For

            Dim metric As String = metricCell.Value.ToString().Trim()
            Dim formatString As String = GetFormatForMetric(metric)

            ' Apply format to all value columns (skip Metric column at index 0)
            For i As Integer = 1 To grid.Columns.Count - 1
                Dim cell = row.Cells(i)

                ' Handle null/DBNull values
                If cell.Value Is Nothing OrElse cell.Value Is DBNull.Value Then
                    cell.Value = 0D
                End If

                ' Apply the format string to the cell style
                cell.Style.Format = formatString
            Next
        Next
    End Sub

    Private Function GetFormatForMetric(metric As String) As String
        If String.IsNullOrEmpty(metric) Then Return "N2"

        ' Use case-insensitive comparison for robustness
        Select Case metric.Trim()
            Case "Margin %"
                Return "P2"
            Case "Sell Price", "LumberCost", "PlateCost", "ManufLaborCost",
                 "ItemCost", "DeliveryCost", "MiscLaborCost", "Total Cost"
                Return "C2"
            Case "BDFT", "Level Count"
                Return "N0"
            Case "AvgSPF2", "ManufLaborMH"
                Return "N2"
            Case Else
                Return "N2"
        End Select
    End Function

    Private Sub ApplyColumnStyles(grid As DataGridView)
        ' Right-align numeric columns
        For i As Integer = 1 To grid.Columns.Count - 1
            grid.Columns(i).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
            grid.Columns(i).MinimumWidth = 100
        Next

        ' Bold and left-align Metric column
        If grid.Columns.Contains("Metric") Then
            With grid.Columns("Metric")
                .DefaultCellStyle.Font = New Font(grid.Font, FontStyle.Bold)
                .DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
                .MinimumWidth = 140
                .DefaultCellStyle.Padding = New Padding(8, 0, 8, 0)
            End With
        End If

        grid.AutoResizeColumns()

        ' Distribute remaining width evenly
        Dim totalWidth As Integer = grid.ClientSize.Width
        Dim metricWidth As Integer = If(grid.Columns.Contains("Metric"), grid.Columns("Metric").Width, 140)
        Dim remainingWidth As Integer = totalWidth - metricWidth - SystemInformation.VerticalScrollBarWidth
        Dim valueColumnCount As Integer = grid.Columns.Count - 1

        If valueColumnCount > 0 Then
            Dim idealWidth As Integer = remainingWidth \ valueColumnCount
            For i As Integer = 1 To grid.Columns.Count - 1
                grid.Columns(i).Width = Math.Max(idealWidth, 100)
            Next
        End If
    End Sub

    ''' <summary>
    ''' Applies distinct header styles to differentiate Estimate, Design, and Bistrack columns.
    ''' </summary>
    Private Sub ApplyColumnHeaderStyles(grid As DataGridView)
        For Each col As DataGridViewColumn In grid.Columns
            Select Case True
                Case col.Name = "Metric"
                    col.HeaderCell.Style.BackColor = SystemColors.Control
                    col.HeaderCell.Style.ForeColor = Color.Black
                    col.HeaderCell.Style.Font = New Font(grid.Font, FontStyle.Bold)

                Case col.Name = "Estimate"
                    col.HeaderCell.Style.BackColor = Color.LightBlue
                    col.HeaderCell.Style.ForeColor = Color.Black
                    col.HeaderCell.Style.Font = New Font(grid.Font, FontStyle.Bold)

                Case col.Name.StartsWith(DESIGN_PREFIX)
                    col.HeaderCell.Style.BackColor = Color.LightGreen
                    col.HeaderCell.Style.ForeColor = Color.DarkGreen

                Case col.Name.StartsWith(BISTRACK_PREFIX) OrElse col.Name.StartsWith("BT ")
                    col.HeaderCell.Style.BackColor = Color.LightGoldenrodYellow
                    col.HeaderCell.Style.ForeColor = Color.DarkGoldenrod

                Case col.Name = "Design"
                    col.HeaderCell.Style.BackColor = Color.LightGreen
                    col.HeaderCell.Style.ForeColor = Color.Black

                Case col.Name = "BT Invoice"
                    col.HeaderCell.Style.BackColor = Color.LightGoldenrodYellow
                    col.HeaderCell.Style.ForeColor = Color.Black
            End Select
        Next
    End Sub

    Private Sub ApplyColorCoding(grid As DataGridView)
        If Not grid.Columns.Contains("Metric") Then Exit Sub
        If grid.Columns.Count < 3 Then Exit Sub ' Need at least Metric, Estimate, and one comparison column

        For Each row As DataGridViewRow In grid.Rows
            If row.IsNewRow Then Continue For

            Dim metricCell = row.Cells("Metric")
            If metricCell Is Nothing OrElse metricCell.Value Is Nothing Then Continue For

            Dim metric As String = metricCell.Value.ToString().Trim()
            Dim estimateValue As Decimal = GetDecimalValue(row.Cells(1))

            ' Skip color coding if estimate is zero (can't calculate percentage difference)
            If estimateValue = 0 Then Continue For

            ' Apply color coding to all columns after Estimate (index 2+)
            For colIndex As Integer = 2 To grid.Columns.Count - 1
                Dim cell = row.Cells(colIndex)
                Dim actualValue As Decimal = GetDecimalValue(cell)
                Dim pctDiff As Decimal = (actualValue - estimateValue) / estimateValue

                cell.Style.BackColor = GetVarianceColor(metric, pctDiff)
            Next
        Next
    End Sub

    Private Function GetDecimalValue(cell As DataGridViewCell) As Decimal
        If cell Is Nothing Then Return 0D
        If cell.Value Is Nothing OrElse cell.Value Is DBNull.Value Then Return 0D

        Dim result As Decimal = 0D
        If Decimal.TryParse(cell.Value.ToString(), result) Then
            Return result
        End If

        Return 0D
    End Function

    Private Function GetVarianceColor(metric As String, pctDiff As Decimal) As Color
        ' Higher is better for Margin %, Sell Price, Level Count
        Dim isHigherBetter As Boolean = (metric = "Margin %" OrElse metric = "Sell Price" OrElse metric = "Level Count")

        If isHigherBetter Then
            If pctDiff > VARIANCE_THRESHOLD Then Return Color.LightGreen
            If pctDiff < -VARIANCE_THRESHOLD Then Return Color.LightCoral
        Else
            ' For costs, lower is better
            If pctDiff > VARIANCE_THRESHOLD Then Return Color.LightCoral
            If pctDiff < -VARIANCE_THRESHOLD Then Return Color.LightGreen
        End If

        Return Color.White
    End Function
End Class