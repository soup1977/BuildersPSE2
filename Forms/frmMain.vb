' Replace the entire frmMain.vb with the following updated code
Option Strict On

Imports System.Windows.Forms
Imports BuildersPSE2.DataAccess
Imports Microsoft.VisualBasic.FileIO

Public Class frmMain
    Private ReadOnly eida As New ExternalImportDataAccess
    Private m_ChildFormNumber As Integer
    Private m_previousTab As TabPage ' Track the previously selected tab
    Private m_isRemovingTab As Boolean = False ' Suppress events during removal
    Public ReadOnly Property PreviousTab As TabPage
        Get
            Return m_previousTab
        End Get
    End Property
    Public Property IsRemovingTab As Boolean
        Get
            Return m_isRemovingTab
        End Get
        Set(value As Boolean)
            m_isRemovingTab = value
        End Set
    End Property

    ' In frmMain.vb: Fix StatusLabel property to use correct name
    Public ReadOnly Property StatusLabel As ToolStripStatusLabel
        Get
            Return CType(StatusStrip.Items("ToolStripStatusLabel"), ToolStripStatusLabel)
        End Get
    End Property

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        If m_isRemovingTab Then
            Exit Sub
        End If
        If TabControl1.SelectedTab IsNot Nothing AndAlso TabControl1.SelectedTab.Tag IsNot Nothing Then
            ' Update previous tab only if switching to a different tab
            If m_previousTab IsNot TabControl1.SelectedTab Then
                m_previousTab = If(TabControl1.SelectedIndex >= 0, TabControl1.TabPages(TabControl1.SelectedIndex), Nothing)
            End If
        End If
    End Sub

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

    Private Sub frmCreateProject_Click(sender As Object, e As EventArgs) Handles btnCreateProject.Click
        Try
            Dim tagValue As String = $"NewProject_{m_ChildFormNumber + 1}"
            AddFormToTabControl(GetType(frmCreateEditProject), tagValue, New Object() {Nothing, 0})
            ToolStripStatusLabel.Text = $"Opening new project form (Tab: {tagValue}) at {DateTime.Now:HH:mm:ss}"
        Catch ex As Exception
            ToolStripStatusLabel.Text = $"Error opening new project form: {ex.Message} at {DateTime.Now:HH:mm:ss}"
            MessageBox.Show($"Error opening new project form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnImportPSE.Click
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
                    Try
                        ExternalImportDataAccess.ImportSpreadsheetAsNewProject(dialog.FileName)
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

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnMondayList.Click
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

    Private Sub btnImportCSV_Click(sender As Object, e As EventArgs) Handles btnImportCSV.Click
        Try
            Using openFileDialog As New OpenFileDialog()
                openFileDialog.Filter = "CSV Files (*.csv)|*.csv"
                openFileDialog.Title = "Select Project CSV File"
                If openFileDialog.ShowDialog() = DialogResult.OK Then
                    ' Prompt user to choose import type. Need to fix CSV export from MGMT to include project structure. Maybe add a new CSV only for Project structure?
                    'Dim importTypeResult = MessageBox.Show("Import full Model Project? (Yes = Full, No = Project Info Only)", "Import Type", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    'Dim isFullImport As Boolean = (importTypeResult = DialogResult.Yes)

                    Dim isfullimport As Boolean = True ' For now, always do full import

                    Using importDialog As New frmImportProjectDialog()
                        ' Pre-fill ProjectName from CSV's Project field
                        Using parser As New TextFieldParser(openFileDialog.FileName)
                            parser.TextFieldType = FieldType.Delimited
                            parser.SetDelimiters(",")
                            If Not parser.EndOfData Then
                                parser.ReadLine() ' Skip header
                                If Not parser.EndOfData Then
                                    Dim fields As String() = parser.ReadFields()
                                    If fields.Length >= 3 Then
                                        importDialog.txtProjectName.Text = fields(2).Trim() ' Project field
                                    End If
                                End If
                            End If
                        End Using

                        If importDialog.ShowDialog() = DialogResult.OK Then
                            Dim csvPath As String = openFileDialog.FileName
                            Dim projName As String = importDialog.txtProjectName.Text.Trim()
                            Dim custName As String = importDialog.cboCustomerName.Text.Trim()
                            Dim estID As Integer? = If(importDialog.cboEstimator.SelectedIndex >= 0, CInt(importDialog.cboEstimator.SelectedValue), Nothing)
                            Dim salID As Integer? = If(importDialog.cboSales.SelectedIndex >= 0, CInt(importDialog.cboSales.SelectedValue), Nothing)

                            Dim address As String = importDialog.txtAddress.Text.Trim()
                            Dim city As String = importDialog.txtCity.Text.Trim()
                            Dim state As String = importDialog.cboState.Text.Trim()
                            Dim zip As String = importDialog.txtZip.Text.Trim()
                            Dim biddate As Date? = importDialog.dtpBidDate.Value.Date
                            Dim archdate As Date? = importDialog.dtpArchPlansDated.Value.Date
                            Dim engdate As Date? = importDialog.dtpEngPlansDated.Value.Date
                            Dim miles As Integer? = CInt(importDialog.nudMilesToJobSite.Text.Trim())


                            Task.Run(Sub()
                                         Try
                                             Dim dataAccess As New ProjectDataAccess()
                                             Dim projectID As Integer
                                             If isfullimport Then
                                                 projectID = ExternalImportDataAccess.ImportProjectFromCSV(csvPath, projName, custName, estID, salID, address, city, state, zip, biddate, archdate, engdate, miles)
                                             Else
                                                 projectID = ExternalImportDataAccess.ImportProjectInfoFromCSV(csvPath, projName, custName, estID, salID, address, city, state, zip, biddate, archdate, engdate, miles)
                                             End If
                                             Me.Invoke(Sub()
                                                           Debug.WriteLine($"Import completed successfully for ProjectID: {projectID}")
                                                           MessageBox.Show($"Project {projectID} imported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                           AddFormToTabControl(GetType(frmMainProjectList), "ProjectList")
                                                           Debug.WriteLine("Refreshed project list tab for ProjectID: " & projectID)
                                                       End Sub)
                                         Catch ex As Exception
                                             Me.Invoke(Sub()
                                                           Debug.WriteLine($"Import failed: {ex.Message}")
                                                           MessageBox.Show($"Error importing project: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                                       End Sub)
                                         End Try
                                     End Sub)
                        Else
                            MessageBox.Show("Import cancelled.", "Import Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information)
                        End If
                    End Using
                Else
                    MessageBox.Show("No file selected.", "Import Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        Catch ex As Exception
            Debug.WriteLine($"Import error in btnImportCSV_Click: {ex.Message}")
            MessageBox.Show($"Error importing project: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class