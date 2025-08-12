Imports System.Reflection
Module modDialogs

    Public Sub AddFormToTabControl(formType As Type, tagValue As String, Optional constructorArgs As Object() = Nothing)
        Try
            ' Get the main form instance
            Dim mainForm As frmMain = CType(Application.OpenForms.OfType(Of frmMain)().FirstOrDefault(), frmMain)
            If mainForm Is Nothing Then
                MessageBox.Show("Main application window not found. Please open from the main window.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' Check if a tab with the same tagValue already exists
            For Each tabPage As TabPage In mainForm.TabControl1.TabPages
                If tabPage.Tag IsNot Nothing AndAlso CStr(tabPage.Tag) = tagValue Then
                    mainForm.TabControl1.SelectedTab = tabPage
                    mainForm.StatusLabel.Text = $"Activated existing tab {tagValue} at {DateTime.Now:HH:mm:ss}"
                    Exit Sub
                End If
            Next

            ' Instantiate the form dynamically
            Dim mdiForm As Form
            If constructorArgs Is Nothing Then
                mdiForm = CType(Activator.CreateInstance(formType), Form)
            Else
                mdiForm = CType(Activator.CreateInstance(formType, constructorArgs), Form)
            End If

            ' Configure the form for embedding
            mdiForm.Tag = tagValue
            mdiForm.TopLevel = False
            mdiForm.FormBorderStyle = FormBorderStyle.None
            mdiForm.Dock = DockStyle.Fill

            ' Create a new TabPage
            Dim newTab As New TabPage(mdiForm.Text) With {
                .Tag = tagValue
            }
            newTab.Controls.Add(mdiForm)

            ' Add to TabControl
            mainForm.TabControl1.TabPages.Add(newTab)
            mainForm.TabControl1.SelectedTab = newTab
            mdiForm.Show()

            ' Update status
            mainForm.ToolStripStatusLabel.Text = $"Opened {mdiForm.Text} in tab at {DateTime.Now:HH:mm:ss}"
        Catch ex As Exception
            ' Log error and show message
            Debug.WriteLine($"Error in AddFormToTabControl: {ex.Message} at {DateTime.Now:HH:mm:ss}")
            Dim mainForm As frmMain = CType(Application.OpenForms.OfType(Of frmMain)().FirstOrDefault(), frmMain)
            If mainForm IsNot Nothing Then
                mainForm.ToolStripStatusLabel.Text = $"Error opening form: {ex.Message} at {DateTime.Now:HH:mm:ss}"
            End If
            MessageBox.Show($"Error opening form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Public Sub RemoveTabFromTabControl(tagValue As String)
        Try
            ' Get the main form instance
            Dim mainForm As frmMain = CType(Application.OpenForms.OfType(Of frmMain)().FirstOrDefault(), frmMain)
            If mainForm Is Nothing Then
                Exit Sub
            End If

            ' Find the TabPage with the matching tagValue
            Dim tabToRemove As TabPage = Nothing
            For Each tabPage As TabPage In mainForm.TabControl1.TabPages
                If tabPage.Tag IsNot Nothing AndAlso CStr(tabPage.Tag) = tagValue Then
                    tabToRemove = tabPage
                    Exit For
                End If
            Next

            If tabToRemove IsNot Nothing Then
                Dim index As Integer = mainForm.TabControl1.TabPages.IndexOf(tabToRemove)
                ' Dispose of the form in the tab if it exists
                If tabToRemove.Controls.Count > 0 AndAlso TypeOf tabToRemove.Controls(0) Is Form Then
                    CType(tabToRemove.Controls(0), Form).Dispose()
                End If

                ' Remove the tab
                mainForm.TabControl1.TabPages.Remove(tabToRemove)
                tabToRemove.Dispose()

                ' Select the next tab if exists, else the last
                If mainForm.TabControl1.TabPages.Count > 0 Then
                    If index < mainForm.TabControl1.TabPages.Count Then
                        mainForm.TabControl1.SelectedIndex = index
                    Else
                        mainForm.TabControl1.SelectedIndex = mainForm.TabControl1.TabPages.Count - 1
                    End If
                End If

                ' Update status
                mainForm.ToolStripStatusLabel.Text = $"Closed tab {tagValue} at {DateTime.Now:HH:mm:ss}"
            End If
        Catch ex As Exception
            ' Log error
            Debug.WriteLine($"Error in RemoveTabFromTabControl: {ex.Message} at {DateTime.Now:HH:mm:ss}")
        End Try
    End Sub
End Module
