Option Strict On

Namespace Utilities
    Public NotInheritable Class StatusLogger
        Private Sub New()
        End Sub

        Private Const MaxHistory As Integer = 25
        Private Shared ReadOnly _history As New List(Of String)()
        Private Shared ReadOnly _sync As New Object()

        Public Shared Sub Add(message As String)
            Dim msg As String = $"{message} at {DateTime.Now:HH:mm:ss}"

            SyncLock _sync
                _history.Insert(0, msg)
                If _history.Count > MaxHistory Then
                    _history.RemoveAt(_history.Count - 1)
                End If
            End SyncLock

            Dim main As frmMain = Application.OpenForms.OfType(Of frmMain)().FirstOrDefault()
            If main Is Nothing OrElse main.IsDisposed OrElse main.StatusHistoryListBox Is Nothing Then
                Return
            End If

            ' --- THIS IS THE CRITICAL FIX ---
            Dim action As Action = Sub()
                                       main.ToolStripStatusLabel.Text = msg

                                       ' Clear and repopulate directly — no DataSource binding issues
                                       main.StatusHistoryListBox.BeginUpdate()
                                       main.StatusHistoryListBox.Items.Clear()
                                       For Each item In _history
                                           main.StatusHistoryListBox.Items.Add(item)
                                       Next
                                       main.StatusHistoryListBox.EndUpdate()

                                       ' Scroll to top when dropdown is open
                                       If main.cmsStatusHistory IsNot Nothing AndAlso main.cmsStatusHistory.Visible Then
                                           main.StatusHistoryListBox.TopIndex = 0
                                       End If
                                   End Sub

            If main.InvokeRequired Then
                Try : main.Invoke(action) : Catch : End Try
            Else
                action()
            End If
        End Sub
    End Class
End Namespace
