Option Strict On

Imports ClosedXML.Excel
Imports System.Data

Public Module SpreadsheetParser
    Public Function ParseSpreadsheet(filePath As String) As Dictionary(Of String, DataTable)
        Dim result As New Dictionary(Of String, DataTable)
        Using wb As New XLWorkbook(filePath)
            For Each ws As IXLWorksheet In wb.Worksheets
                Dim dt As New DataTable(ws.Name)
                Dim firstRow As IXLRow = ws.FirstRowUsed()
                If firstRow Is Nothing Then Continue For
                ' Add columns sequentially up to the last used column to handle sparse data
                Dim maxCol As Integer = ws.LastColumnUsed().ColumnNumber()
                For col As Integer = 1 To maxCol
                    Dim cell As IXLCell = firstRow.Cell(col)
                    Dim columnName As String = If(cell.IsEmpty() OrElse String.IsNullOrEmpty(cell.Value.ToString()), "Column" & col, cell.Value.ToString())
                    ' Handle potential duplicate names
                    Dim originalName As String = columnName
                    Dim suffix As Integer = 1
                    While dt.Columns.Contains(columnName)
                        columnName = originalName & "_" & suffix
                        suffix += 1
                    End While
                    dt.Columns.Add(columnName)
                Next
                ' Add rows, handling nulls
                For Each row As IXLRow In ws.RowsUsed().Skip(1)
                    Dim newRow As DataRow = dt.NewRow()
                    For col As Integer = 1 To dt.Columns.Count
                        Dim cellValue As Object = row.Cell(col).Value
                        newRow(col - 1) = If(cellValue Is Nothing OrElse cellValue.ToString() = String.Empty, DBNull.Value, cellValue)
                    Next
                    dt.Rows.Add(newRow)
                Next
                result.Add(ws.Name, dt)
            Next
        End Using
        Return result
    End Function
End Module
