
Module modDialogs
    Public Sub AddFormToTabControl(formType As Type, tagValue As String, Optional constructorArgs As Object() = Nothing)
        Try
            ' Get the main form instance
            Dim mainForm As frmMain = CType(Application.OpenForms.OfType(Of frmMain)().FirstOrDefault(), frmMain)
            If mainForm Is Nothing Then
                MessageBox.Show("Main application window not found. Please open from the main window.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            ' Check if a tab with the same tagValue exists
            For Each tabPage As TabPage In mainForm.TabControl1.TabPages
                If tabPage.Tag IsNot Nothing AndAlso CStr(tabPage.Tag) = tagValue Then
                    mainForm.IsRemovingTab = True ' Suppress event
                    Try
                        mainForm.SuspendLayout()
                        mainForm.TabControl1.SelectedTab = tabPage
                        mainForm.ToolStripStatusLabel.Text = $"Activated existing tab {tagValue} at {DateTime.Now:HH:mm:ss}"
                    Finally
                        mainForm.ResumeLayout()
                        mainForm.IsRemovingTab = False
                    End Try
                    Exit Sub
                End If
            Next

            ' Instantiate the form
            Dim mdiForm As Form
            If constructorArgs Is Nothing Then
                mdiForm = CType(Activator.CreateInstance(formType), Form)
            Else
                mdiForm = CType(Activator.CreateInstance(formType, constructorArgs), Form)
            End If

            ' Configure the form
            mdiForm.Tag = tagValue
            mdiForm.TopLevel = False
            mdiForm.FormBorderStyle = FormBorderStyle.None
            mdiForm.Dock = DockStyle.Fill

            ' Create and add new TabPage
            Dim newTab As New TabPage(mdiForm.Text) With {
                .Tag = tagValue
            }
            newTab.Controls.Add(mdiForm)

            ' Add and select tab, suppressing redraw
            mainForm.IsRemovingTab = True
            Try
                mainForm.SuspendLayout()
                mainForm.TabControl1.TabPages.Add(newTab)
                mainForm.TabControl1.SelectedTab = newTab
                mdiForm.Show()
                mainForm.ToolStripStatusLabel.Text = $"Opened {mdiForm.Text} in tab at {DateTime.Now:HH:mm:ss}"
            Finally
                mainForm.ResumeLayout()
                mainForm.IsRemovingTab = False
            End Try
        Catch ex As Exception
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
                Dim wasSelected As Boolean = (mainForm.TabControl1.SelectedTab Is tabToRemove)

                ' Dispose of the form in the tab
                If tabToRemove.Controls.Count > 0 AndAlso TypeOf tabToRemove.Controls(0) Is Form Then
                    CType(tabToRemove.Controls(0), Form).Dispose()
                End If

                ' Remove tab, suppressing redraw
                mainForm.IsRemovingTab = True
                Try
                    mainForm.SuspendLayout()
                    mainForm.TabControl1.TabPages.Remove(tabToRemove)
                    tabToRemove.Dispose()

                    If wasSelected Then
                        ' Select previous tab if available and still open
                        If mainForm.PreviousTab IsNot Nothing AndAlso mainForm.TabControl1.TabPages.Contains(mainForm.PreviousTab) Then
                            mainForm.TabControl1.SelectedTab = mainForm.PreviousTab
                        ElseIf mainForm.TabControl1.TabPages.Count > 0 Then
                            ' Fallback to last tab
                            Dim lastTabIndex As Integer = mainForm.TabControl1.TabPages.Count - 1
                            mainForm.TabControl1.SelectedIndex = lastTabIndex
                        End If
                    End If

                    mainForm.ToolStripStatusLabel.Text = $"Closed tab {tagValue} at {DateTime.Now:HH:mm:ss}"
                Finally
                    mainForm.ResumeLayout()
                    mainForm.IsRemovingTab = False
                End Try
            End If
        Catch ex As Exception
            Dim mainForm As frmMain = CType(Application.OpenForms.OfType(Of frmMain)().FirstOrDefault(), frmMain)
            If mainForm IsNot Nothing Then
                mainForm.ToolStripStatusLabel.Text = $"Error closing tab: {ex.Message} at {DateTime.Now:HH:mm:ss}"
                mainForm.IsRemovingTab = False
            End If
        End Try
    End Sub
End Module
