Option Strict On


Imports System.Data.SqlClient
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Utilities
Imports Microsoft.VisualBasic.FileIO

Public Class frmMain

    ' === Tab Management (clean, fast, no flicker, singleton support) ===
    Private _openTabs As New Dictionary(Of String, TabPage)(StringComparer.OrdinalIgnoreCase)
    Private _previousTab As TabPage = Nothing
    Private _isRemovingTab As Boolean = False

    ' === Activity Log Settings ===
    Private Const MAX_LOG_ENTRIES As Integer = 500

    ' === Counters for multi-instance forms ===
    Private m_NewProjectCounter As Integer = 0

    Public Sub New()
        InitializeComponent()

    End Sub

    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Event wiring for rock-solid tab tracking & refresh
        AddHandler TabControl1.Selecting, AddressOf TabControl1_Selecting
        AddHandler TabControl1.SelectedIndexChanged, AddressOf TabControl1_SelectedIndexChanged

        ' Open the project list as the default tab (singleton)
        AddFormToTabControl(GetType(frmMainProjectList), "ProjectList")
        ttslUserName.Text = CurrentUser.DisplayName

        LogStatus("Application started")
    End Sub

    ' ================================================================
    '  STATUS LOGGING (centralised, clean, auto-trims history)
    ' ================================================================
    Private Sub LogStatus(message As String)
        Dim fullMessage As String = $"{message}  @{DateTime.Now:HH:mm:ss}"
        ToolStripStatusLabel.Text = fullMessage
    End Sub

    ' ================================================================
    '  ACTIVITY LOG PANEL (public for UIHelper access)
    ' ================================================================
    ''' <summary>
    ''' Adds a timestamped message to the Activity Log panel.
    ''' Thread-safe - can be called from any thread.
    ''' </summary>
    Public Sub AddToActivityLog(message As String)
        If Me.InvokeRequired Then
            Me.Invoke(Sub() AddToActivityLog(message))
            Return
        End If

        Dim logEntry As String = $"[{DateTime.Now:HH:mm:ss}] {message}"

        ' Insert at top (newest first)
        lstActivityLog.Items.Insert(0, logEntry)

        ' Trim old entries to prevent memory bloat
        While lstActivityLog.Items.Count > MAX_LOG_ENTRIES
            lstActivityLog.Items.RemoveAt(lstActivityLog.Items.Count - 1)
        End While

        ' Also write to debug output for development
        Debug.WriteLine(logEntry)
    End Sub

    ''' <summary>
    ''' Toggles the Activity Log panel visibility.
    ''' </summary>
    Public Sub ToggleActivityLog()
        SplitContainerMain.Panel2Collapsed = Not SplitContainerMain.Panel2Collapsed
        btnToggleLog.Text = If(SplitContainerMain.Panel2Collapsed, "Show Log", "Hide Log")

        ' Set a reasonable default height when showing
        If Not SplitContainerMain.Panel2Collapsed Then
            SplitContainerMain.SplitterDistance = CInt(SplitContainerMain.Height * 0.75)
        End If
    End Sub

    ''' <summary>
    ''' Clears all entries from the Activity Log.
    ''' </summary>
    Public Sub ClearActivityLog()
        lstActivityLog.Items.Clear()
        AddToActivityLog("Log cleared")
    End Sub

    ' === Activity Log Event Handlers ===
    Private Sub btnToggleLog_Click(sender As Object, e As EventArgs) Handles btnToggleLog.Click, ToggleActivityLogToolStripMenuItem.Click
        ToggleActivityLog()
    End Sub

    Private Sub btnClearLog_Click(sender As Object, e As EventArgs) Handles btnClearLog.Click, ClearActivityLogToolStripMenuItem.Click
        ClearActivityLog()
    End Sub

    ' ================================================================
    '  TAB CONTROL EVENTS (previous tab tracking + auto-refresh)
    ' ================================================================
    Private Sub TabControl1_Selecting(sender As Object, e As TabControlCancelEventArgs) Handles TabControl1.Selecting
        If Not _isRemovingTab Then
            _previousTab = TabControl1.SelectedTab
        End If
    End Sub

    Private Sub TabControl1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles TabControl1.SelectedIndexChanged
        If _isRemovingTab OrElse TabControl1.SelectedTab Is Nothing Then Return

        Dim activeForm As Form = CType(TabControl1.SelectedTab.Controls(0), Form)

        LogStatus($"Activated tab '{TabControl1.SelectedTab.Tag}' ({activeForm.Text})")

        ' === Auto-refresh logic (add more TypeOf checks as needed) ===
        Select Case True

            Case TypeOf activeForm Is frmCreateEditProject
                CType(activeForm, frmCreateEditProject).LoadProjectBuildings()
                CType(activeForm, frmCreateEditProject).RefreshProjectTree()

            Case TypeOf activeForm Is frmMainProjectList
                CType(activeForm, frmMainProjectList).RefreshProjectList()

        End Select
    End Sub

    ' ================================================================
    '  ADD FORM TO TAB (singleton support via tag, no flicker)
    ' ================================================================
    Public Sub AddFormToTabControl(formType As Type, tagValue As String, Optional constructorArgs As Object() = Nothing)
        Try
            ' === Singleton behaviour – reuse existing tab if tag exists ===
            If _openTabs.ContainsKey(tagValue) Then
                _isRemovingTab = True
                Try
                    SuspendLayout()
                    TabControl1.SelectedTab = _openTabs(tagValue)
                Finally
                    ResumeLayout()
                    _isRemovingTab = False
                End Try
                LogStatus($"Re-activated existing tab: {tagValue}")
                Return
            End If

            ' === Create the form instance ===
            Dim newForm As Form = If(constructorArgs Is Nothing,
                CType(Activator.CreateInstance(formType), Form),
                CType(Activator.CreateInstance(formType, constructorArgs), Form))

            newForm.TopLevel = False
            newForm.FormBorderStyle = FormBorderStyle.None
            newForm.Dock = DockStyle.Fill
            newForm.Tag = tagValue

            ' === Create tab page ===
            Dim newTab As New TabPage(newForm.Text) With {.Tag = tagValue}
            newTab.Controls.Add(newForm)

            ' === Add to TabControl safely ===
            _isRemovingTab = True
            Try
                SuspendLayout()
                TabControl1.TabPages.Add(newTab)
                TabControl1.SelectedTab = newTab
                _openTabs.Add(tagValue, newTab)
                newForm.Show()
                LogStatus($"Opened new tab: {tagValue} ({newForm.Text})")
            Finally
                ResumeLayout()
                _isRemovingTab = False
            End Try

        Catch ex As Exception
            LogStatus($"Error opening tab {tagValue}: {ex.Message}")
            MessageBox.Show(ex.Message, "Error opening form", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    ' ================================================================
    '  FINAL CHROME/FIREFOX-STYLE TAB CLOSE – CLEANEST POSSIBLE (VB.NET FIX)
    ' ================================================================
    Public Sub RemoveTabFromTabControl(tagValue As String)
        Dim tabToRemove As TabPage = Nothing                     ' ← DECLARED FIRST (fixes BC30451)

        If Not _openTabs.TryGetValue(tagValue, tabToRemove) Then Return

        Dim closingCurrentTab As Boolean = (TabControl1.SelectedTab Is tabToRemove)

        ' === 1. Instantly pre-select the perfect next tab (zero flicker) ===
        If closingCurrentTab Then
            Dim nextTab As TabPage = Nothing

            ' Primary: Chrome/Firefox MRU behaviour
            If _previousTab IsNot Nothing AndAlso
           _previousTab IsNot tabToRemove AndAlso
           TabControl1.TabPages.Contains(_previousTab) Then

                nextTab = _previousTab

            Else
                ' Fallback: left tab if exists, otherwise the tab that will become the new first/last
                Dim index As Integer = TabControl1.TabPages.IndexOf(tabToRemove)
                If index > 0 Then
                    nextTab = TabControl1.TabPages(index - 1)
                ElseIf TabControl1.TabPages.Count > 1 Then
                    nextTab = TabControl1.TabPages(1)   ' after removal this becomes index 0
                End If
            End If

            If nextTab IsNot Nothing Then
                TabControl1.SelectedTab = nextTab   ' ← instant, no flash
            End If
        End If

        ' === 2. Clean removal ===
        _isRemovingTab = True
        Try
            If tabToRemove.Controls.Count > 0 AndAlso TypeOf tabToRemove.Controls(0) Is Form Then
                CType(tabToRemove.Controls(0), Form).Dispose()
            End If

            TabControl1.TabPages.Remove(tabToRemove)
            tabToRemove.Dispose()
            _openTabs.Remove(tagValue)

            LogStatus($"Closed tab: {tagValue}")
        Finally
            _isRemovingTab = False
        End Try
    End Sub


    ' ================================================================
    '  MENU / BUTTON HANDLERS (now super clean – singletons + counters)
    ' ================================================================
    Private Sub frmCreateProject_Click(sender As Object, e As EventArgs) Handles btnCreateProject.Click, CreateProjectToolStripMenuItem.Click
        m_NewProjectCounter += 1
        Dim tag As String = $"NewProject_{m_NewProjectCounter}"
        AddFormToTabControl(GetType(frmCreateEditProject), tag, New Object() {Nothing, 0})
    End Sub

    Private Sub btnImportPSE_Click(sender As Object, e As EventArgs) Handles btnImportPSE.Click, ImportPSEProjectToolStripMenuItem.Click
        Using ofd As New OpenFileDialog With {
            .Filter = "Excel Files|*.xlsx;*.xls;*.xlsm",
            .Title = "Select PSE Spreadsheet"
        }
            If ofd.ShowDialog() <> DialogResult.OK Then
                LogStatus("PSE import cancelled")
                Return
            End If

            Try
                ExternalImportDataAccess.ImportSpreadsheetAsNewProject(ofd.FileName)
                MessageBox.Show("Import successful.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                ' Refresh project list tab (singleton tag forces refresh via SelectedIndexChanged)
                AddFormToTabControl(GetType(frmMainProjectList), "ProjectList")
                LogStatus("PSE project imported successfully")
            Catch ex As Exception
                MessageBox.Show($"Import failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                LogStatus($"PSE import failed: {ex.Message}")
            End Try
        End Using
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnMondayList.Click
        AddFormToTabControl(GetType(frmMondayList), "MondayList") ' singleton
    End Sub

    Private Sub btnEditLumber_Click(sender As Object, e As EventArgs) Handles btnEditLumber.Click, EditLumberToolStripMenuItem.Click
        AddFormToTabControl(GetType(frmLumberManagement), "LumberManagement") ' singleton
    End Sub

    Private Sub btnImportCSV_Click(sender As Object, e As EventArgs) Handles btnImportCSV.Click, ImportMGMTProjectToolStripMenuItem.Click

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
                                             Dim projectID As Integer = ExternalImportDataAccess.ImportProjectFromCSV(csvPath, projName, custName, estID, salID, address, city, state, zip, biddate, archdate, engdate, miles)


                                             Me.Invoke(Sub()
                                                           Debug.WriteLine($"Import completed successfully for ProjectID: {projectID}")
                                                           'MessageBox.Show($"Project {projectID} imported successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
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

    Private Sub frmMain_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If CurrentUser.UserID > 0 Then
            Try
                Using conn As SqlConnection = SqlConnectionManager.Instance.GetConnection()
                    Using cmd As New SqlCommand(Queries.UpdateUserLoginStatus, conn)
                        cmd.Parameters.AddWithValue("@UserID", CurrentUser.UserID)
                        cmd.Parameters.AddWithValue("@IsLoggedIn", False)
                        cmd.Parameters.AddWithValue("@LastLogin", DBNull.Value)
                        cmd.Parameters.AddWithValue("@LastLogout", DateTime.Now)
                        cmd.ExecuteNonQuery()
                    End Using
                End Using
                CurrentUser.Clear()
            Catch
                ' silent fail on logout
            End Try
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click, ExitToolStripMenuItem.Click
        Me.Close()
    End Sub

    Private Sub btnPriceLock_Click(sender As Object, e As EventArgs) Handles btnPriceLock.Click
        Using frm As New frmPriceLockList()
            frm.ShowDialog()
        End Using
    End Sub

    Private Sub btnPriceLockAdmin_Click(sender As Object, e As EventArgs) Handles btnPriceLockAdmin.Click
        Using frm As New frmPriceLockAdmin()
            frm.ShowDialog()
        End Using
    End Sub
End Class