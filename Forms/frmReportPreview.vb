Option Strict On
Imports System.Data
Imports System.Text
Imports System.IO
Imports Microsoft.Reporting.WinForms

Public Class frmReportPreview
    Private ReadOnly reportPath As String

    Private ReadOnly dataSources As List(Of ReportDataSource)

    Public Sub New(reportPath As String, dataSources As List(Of ReportDataSource))
        InitializeComponent()
        Me.reportPath = reportPath
        Me.dataSources = dataSources

        Me.Text = "Report Preview - " & IO.Path.GetFileNameWithoutExtension(reportPath)
    End Sub

    Private Sub frmReportPreview_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            rptViewer.ProcessingMode = ProcessingMode.Local
            rptViewer.ShowToolBar = True
            rptViewer.LocalReport.ReportEmbeddedResource = reportPath
            rptViewer.LocalReport.DataSources.Clear()
            For Each ds As ReportDataSource In dataSources
                rptViewer.LocalReport.DataSources.Add(ds)
            Next
            rptViewer.RefreshReport()
        Catch ex As Exception
            MessageBox.Show("Error loading preview: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub rptViewer_ReportExport(sender As Object, e As ReportExportEventArgs) Handles rptViewer.ReportExport
        e.Cancel = True                     'take full control of the export

        Dim format As String = e.Extension.Name.ToUpperInvariant()
        Dim fileExt As String
        Dim filter As String

        Select Case format
            Case "PDF"
                fileExt = "pdf"
                filter = "PDF files (*.pdf)|*.pdf"
            Case "EXCEL", "EXCELOPENXML"
                fileExt = "xlsx"
                filter = "Excel Workbook (*.xlsx)|*.xlsx"
            Case "WORD", "WORDOPENXML"
                fileExt = "docx"
                filter = "Word Document (*.docx)|*.docx"
            Case Else
                MessageBox.Show($"Export to {format} is not supported in this custom handler.",
                                "Unsupported Format", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Exit Sub
        End Select

        '--- Build a sensible default file name from the data source -----------------
        Dim suggestedName As String = $"ProjectSummary_{DateTime.Now:yyyyMMdd}.{fileExt}"

        Try
            Dim ds = rptViewer.LocalReport.DataSources("ProjectSummaryDataSet")
            If ds IsNot Nothing AndAlso ds.Value IsNot Nothing AndAlso TypeOf ds.Value Is DataTable Then
                Dim table As DataTable = CType(ds.Value, DataTable)
                If table.Rows.Count > 0 Then
                    Dim row As DataRow = table.Rows(0)

                    Dim projectID As Object = If(row.Table.Columns.Contains("JBID"), row("JBID"), DBNull.Value)

                    Dim projectName As Object = If(row.Table.Columns.Contains("ProjectName"), row("ProjectName"), DBNull.Value)
                    Dim customerName As Object = If(row.Table.Columns.Contains("CustomerName"), row("CustomerName"), DBNull.Value)
                    Dim versionName As Object = If(row.Table.Columns.Contains("VersionName"), row("VersionName"), DBNull.Value)

                    Dim parts As New List(Of String)

                    If Not IsDBNull(projectID) Then
                        parts.Add(projectID.ToString().Trim())
                    End If

                    If Not IsDBNull(projectName) Then
                        parts.Add(SanitizeFileName(Convert.ToString(projectName)))
                    End If

                    If Not IsDBNull(customerName) AndAlso Not String.IsNullOrWhiteSpace(customerName.ToString()) Then
                        parts.Add(SanitizeFileName(Convert.ToString(customerName)))
                    End If

                    If Not IsDBNull(versionName) AndAlso Not String.IsNullOrWhiteSpace(versionName.ToString()) Then
                        parts.Add(SanitizeFileName(Convert.ToString(versionName)))
                    End If

                    parts.Add(Date.Today.ToString("yyyy-MM-dd"))

                    suggestedName = String.Join(" - ", parts) & "." & fileExt
                End If
            End If
        Catch ex As Exception
            'any problem reading the data → just use a safe fallback name
            suggestedName = $"ProjectSummary_{DateTime.Now:yyyyMMdd}.{fileExt}"
        End Try

        '--- Show SaveFileDialog with the suggested name -----------------------------
        Dim saveDlg As New SaveFileDialog With {
            .FileName = suggestedName,
            .Filter = filter & "|All files (*.*)|*.*",
            .DefaultExt = fileExt,
            .AddExtension = True
        }

        If saveDlg.ShowDialog() = DialogResult.OK Then
            Try
                Dim warnings As Warning() = Nothing
                Dim streamIds As String() = Nothing
                Dim mimeType As String = Nothing
                Dim encoding As String = Nothing
                Dim extension As String = Nothing

                Dim bytes As Byte() = rptViewer.LocalReport.Render(
                    format, Nothing, mimeType, encoding, extension, streamIds, warnings)

                File.WriteAllBytes(saveDlg.FileName, bytes)

                MessageBox.Show("Report exported successfully!", "Success",
                                MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Export failed: " & ex.Message, "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    '--- Remove invalid filename characters and clean up spaces --------------------
    Private Function SanitizeFileName(input As String) As String
        If String.IsNullOrEmpty(input) Then Return String.Empty

        Dim invalidChars = Path.GetInvalidFileNameChars()
        Dim sb As New StringBuilder(input.Length)

        For Each c As Char In input
            If Not invalidChars.Contains(c) Then
                sb.Append(c)
            End If
        Next

        'Replace multiple spaces with a single space and trim
        Dim result As String = sb.ToString()
        Do While result.Contains("  ")
            result = result.Replace("  ", " ")
        Loop

        Return result.Trim()
    End Function
End Class