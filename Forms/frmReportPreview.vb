Option Strict On
Imports Microsoft.Reporting.WinForms
Imports System.Data

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
End Class