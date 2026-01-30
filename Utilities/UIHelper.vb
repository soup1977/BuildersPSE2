' ==== Put this in a Module or a small static class (e.g. UIHelper.vb) ====
Option Strict On
Imports System.Windows.Forms

Public Module UIHelper

    Private _originalCursor As Cursor = Cursors.Default
    Private _busyCount As Integer = 0           ' supports nested calls

    ''' <summary>
    ''' Show "working" feedback – safe to call multiple times (nested)
    ''' </summary>
    ''' <param name="form">The form that owns the status label (usually MainForm)</param>
    ''' <param name="message">Optional text to show in status bar (default "Please wait...")</param>
    Public Sub ShowBusy(form As Form, Optional message As String = "Please wait...")
        If _busyCount = 0 Then
            _originalCursor = form.Cursor
            form.Cursor = Cursors.WaitCursor
            SetStatus(message)
            Add(message) ' Log to activity panel
        End If
        _busyCount += 1
    End Sub

    ''' <summary>
    ''' Hide "working" feedback – call once for every ShowBusy
    ''' </summary>
    Public Sub HideBusy(form As Form)
        _busyCount -= 1
        If _busyCount <= 0 Then
            _busyCount = 0
            form.Cursor = _originalCursor
            ClearStatus()
        End If
    End Sub

    ' Tiny wrappers so you don't have to know the exact ToolStripStatusLabel names
    Private Sub SetStatus(text As String)
        Dim f As frmMain = TryCast(Application.OpenForms("frmMain"), frmMain)
        If f IsNot Nothing AndAlso f.StatusStrip IsNot Nothing Then
            f.ToolStripStatusLabel.Text = text & "  "

        End If
    End Sub

    Private Sub ClearStatus()
        Dim f As frmMain = TryCast(Application.OpenForms("frmMain"), frmMain)
        If f IsNot Nothing AndAlso f.StatusStrip IsNot Nothing Then
            f.ToolStripStatusLabel.Text = "Ready"

        End If
    End Sub

    ''' <summary>
    ''' Adds a timestamped message to the Activity Log panel in frmMain.
    ''' Safe to call from any thread (invokes if needed).
    ''' Also writes to Debug output for development.
    ''' </summary>
    ''' <param name="text">Text to display in the activity log</param>


    Public Sub Add(text As String)

        ' Always write to debug output
        Debug.WriteLine($"[{DateTime.Now:HH:mm:ss}] {text}")

        ' Get main form and add to activity log
        Dim main As frmMain = TryCast(Application.OpenForms("frmMain"), frmMain)
        If main Is Nothing OrElse main.IsDisposed Then Return

        ' Use the new public method on frmMain (handles thread safety internally)
        main.AddToActivityLog(text)
    End Sub

    ''' <summary>
    ''' Toggles the Activity Log panel visibility from anywhere in the application.
    ''' </summary>
    Public Sub ToggleActivityLog()
        Dim main As frmMain = TryCast(Application.OpenForms("frmMain"), frmMain)
        If main Is Nothing OrElse main.IsDisposed Then Return

        If main.InvokeRequired Then
            main.Invoke(Sub() main.ToggleActivityLog())
        Else
            main.ToggleActivityLog()
        End If
    End Sub

    ''' <summary>
    ''' Clears all entries from the Activity Log.
    ''' </summary>
    Public Sub ClearActivityLog()
        Dim main As frmMain = TryCast(Application.OpenForms("frmMain"), frmMain)
        If main Is Nothing OrElse main.IsDisposed Then Return

        If main.InvokeRequired Then
            main.Invoke(Sub() main.ClearActivityLog())
        Else
            main.ClearActivityLog()
        End If
    End Sub

End Module