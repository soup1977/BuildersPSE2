' Replace the entire frmMain.vb with the following updated code
Option Strict On

Imports System.Windows.Forms
Imports BuildersPSE2.BuildersPSE.DataAccess

Public Class frmMain
    Private m_ChildFormNumber As Integer

    ' In frmMain.vb: Fix StatusLabel property to use correct name
    Public ReadOnly Property StatusLabel As ToolStripStatusLabel
        Get
            Return CType(StatusStrip.Items("ToolStripStatusLabel"), ToolStripStatusLabel)
        End Get
    End Property

    Private Sub FrmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialize status label
        Dim lbl As New ToolStripStatusLabel("Ready") With {
            .Name = "ToolStripStatusLabel",
            .Spring = True,
            .TextAlign = ContentAlignment.MiddleLeft
        }
        StatusStrip.Items.Clear()
        StatusStrip.Items.Add(lbl)
        StatusStrip.BringToFront()

        ' Open frmMainProjectList as a tab on load
        AddFormToTabControl(GetType(frmMainProjectList), "ProjectList")
    End Sub

    Private Sub frmCreateProject_Click(sender As Object, e As EventArgs) Handles frmCreateProject.Click
        Try
            Dim tagValue As String = $"NewProject_{m_ChildFormNumber + 1}"
            AddFormToTabControl(GetType(frmCreateEditProject), tagValue, New Object() {Nothing, 0})
            ToolStripStatusLabel.Text = $"Opening new project form (Tab: {tagValue}) at {DateTime.Now:HH:mm:ss}"
        Catch ex As Exception
            ToolStripStatusLabel.Text = $"Error opening new project form: {ex.Message} at {DateTime.Now:HH:mm:ss}"
        MessageBox.Show($"Error opening new project form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Using dialog As New OpenFileDialog()
            dialog.Filter = "Excel Files (*.xlsx;*.xls;*.xlsm)|*.xlsx;*.xls;*.xlsm|All Files (*.*)|*.*"
            dialog.Title = "Select a Spreadsheet File"
            If dialog.ShowDialog() = DialogResult.OK Then
                ' Parse spreadsheet and show preview
                Dim spreadsheetData As Dictionary(Of String, DataTable) = SpreadsheetParser.ParseSpreadsheet(dialog.FileName)

                If Not spreadsheetData.ContainsKey("Summary") Then
                    MessageBox.Show("Summary sheet is missing.", "Invalid Spreadsheet", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                If spreadsheetData("Summary").Columns.Count <= 6 Then
                    MessageBox.Show("Summary sheet has too few columns; expected at least 7.", "Invalid Spreadsheet", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If
                Dim summaryG1 As String = spreadsheetData("Summary").Columns(6).ColumnName.Trim()
                If summaryG1 <> "v0.2.1" Then
                    MessageBox.Show($"Invalid spreadsheet version. Expected 'V2.1' in Summary column G header, found '{summaryG1}'.", "Invalid Spreadsheet", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End If


                Dim testForm As New Form With {
                .Text = "Spreadsheet Data Preview",
                .Width = 800,
                .Height = 600
            }
                Dim tabControl As New TabControl With {
                .Dock = DockStyle.Fill
            }
                For Each kvp As KeyValuePair(Of String, DataTable) In spreadsheetData
                    Dim dg As New DataGridView With {
                    .Dock = DockStyle.Fill,
                    .DataSource = kvp.Value,
                    .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
                }
                    Dim tabPage As New TabPage(kvp.Key)
                    tabPage.Controls.Add(dg)
                    tabControl.TabPages.Add(tabPage)
                Next
                ' Add OK/Cancel buttons
                Dim buttonPanel As New FlowLayoutPanel With {
                .Dock = DockStyle.Bottom,
                .Height = 40,
                .FlowDirection = FlowDirection.RightToLeft
            }
                Dim btnCancel As New Button With {
                .Text = "Cancel",
                .DialogResult = DialogResult.Cancel
            }
                Dim btnOK As New Button With {
                .Text = "OK",
                .DialogResult = DialogResult.OK
            }
                buttonPanel.Controls.Add(btnCancel)
                buttonPanel.Controls.Add(btnOK)
                testForm.Controls.Add(tabControl)
                testForm.Controls.Add(buttonPanel)
                testForm.AcceptButton = btnOK
                testForm.CancelButton = btnCancel
                If testForm.ShowDialog() = DialogResult.OK Then
                    Dim importer As New SpreadsheetImportData()
                    Try
                        importer.ImportSpreadsheetAsNewProject(dialog.FileName)
                        MessageBox.Show("Import successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Catch ex As Exception
                        MessageBox.Show($"Import failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    End Try
                Else
                    MessageBox.Show("Import cancelled.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            Else
                MessageBox.Show("No file selected.", "Import Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information)
            End If
        End Using
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        'frmMondayList.Show()
        Try
            Dim tagValue As String = $"MondayList_{m_ChildFormNumber + 1}"
            AddFormToTabControl(GetType(frmMondayList), tagValue)
            ToolStripStatusLabel.Text = $"Opening Monday List form (Tab: {tagValue}) at {DateTime.Now:HH:mm:ss}"
        Catch ex As Exception
            ToolStripStatusLabel.Text = $"Error opening Monday List form: {ex.Message} at {DateTime.Now:HH:mm:ss}"
            MessageBox.Show($"Error opening Monday List form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnEditLumber_Click(sender As Object, e As EventArgs) Handles btnEditLumber.Click
        Try
            Dim tagValue As String = $"LumberManagement_{m_ChildFormNumber + 1}"
            AddFormToTabControl(GetType(frmLumberManagement), tagValue)
            ToolStripStatusLabel.Text = $"Opening Lumber Management form (Tab: {tagValue}) at {DateTime.Now:HH:mm:ss}"
        Catch ex As Exception
            ToolStripStatusLabel.Text = $"Error opening Lumber Management form: {ex.Message} at {DateTime.Now:HH:mm:ss}"
            MessageBox.Show($"Error opening Lumber Management form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class