''' <summary>
''' Formats variance DataGridViews with consistent styling and color coding.
''' </summary>
Public Class VarianceGridFormatter
    Private Const VARIANCE_THRESHOLD As Decimal = 0.05D

    ''' <summary>
    ''' Applies standard formatting to a variance grid.
    ''' </summary>
    Public Sub FormatVarianceGrid(grid As DataGridView, Optional freezeEstimateColumn As Boolean = True)
        If grid.DataSource Is Nothing OrElse grid.Columns.Count = 0 Then Exit Sub

        ApplyCellFormats(grid)
        ApplyColumnStyles(grid)
        ApplyColorCoding(grid)

        If freezeEstimateColumn AndAlso grid.Columns.Count > 1 Then
            grid.Columns(1).Frozen = True
        End If
    End Sub

    Private Sub ApplyCellFormats(grid As DataGridView)
        For Each row As DataGridViewRow In grid.Rows
            If row.Cells("Metric").Value Is Nothing Then Continue For
            Dim metric = row.Cells("Metric").Value.ToString()

            For i = 1 To grid.Columns.Count - 1
                Dim cell = row.Cells(i)
                If cell.Value Is Nothing OrElse cell.Value Is DBNull.Value Then
                    cell.Value = 0
                End If
                cell.Style.Format = GetFormatForMetric(metric)
            Next
        Next
    End Sub

    Private Function GetFormatForMetric(metric As String) As String
        Select Case metric
            Case "Margin %" : Return "P2"
            Case "Sell Price", "LumberCost", "PlateCost", "ManufLaborCost",
                 "ItemCost", "DeliveryCost", "MiscLaborCost", "Total Cost"
                Return "C2"
            Case "BDFT", "Level Count" : Return "N0"
            Case Else : Return "N2"
        End Select
    End Function

    Private Sub ApplyColumnStyles(grid As DataGridView)
        ' Right-align numeric columns
        For i = 1 To grid.Columns.Count - 1
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
            For i = 1 To grid.Columns.Count - 1
                grid.Columns(i).Width = Math.Max(idealWidth, 100)
            Next
        End If
    End Sub

    Private Sub ApplyColorCoding(grid As DataGridView)
        For Each row As DataGridViewRow In grid.Rows
            If row.Cells("Metric").Value Is Nothing Then Continue For
            Dim metric = row.Cells("Metric").Value.ToString()
            Dim estimateValue = GetDecimalValue(row.Cells(1))
            If estimateValue = 0 Then Continue For

            For colIndex = 2 To grid.Columns.Count - 1
                Dim cell = row.Cells(colIndex)
                Dim actualValue = GetDecimalValue(cell)
                Dim pctDiff = (actualValue - estimateValue) / estimateValue

                cell.Style.BackColor = GetVarianceColor(metric, pctDiff)
            Next
        Next
    End Sub

    Private Function GetDecimalValue(cell As DataGridViewCell) As Decimal
        If cell.Value Is Nothing OrElse cell.Value Is DBNull.Value Then Return 0D
        Return CDec(cell.Value)
    End Function

    Private Function GetVarianceColor(metric As String, pctDiff As Decimal) As Color
        ' Higher is better for Margin %, Sell Price, Level Count
        Dim isHigherBetter = (metric = "Margin %" OrElse metric = "Sell Price" OrElse metric = "Level Count")

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