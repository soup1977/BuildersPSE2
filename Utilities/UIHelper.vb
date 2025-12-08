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
            SetStatus(message & "  ", marquee:=True)
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

    ' Tiny wrappers so you don’t have to know the exact ToolStripStatusLabel names
    Private Sub SetStatus(text As String, marquee As Boolean)
        Dim f As frmMain = TryCast(Application.OpenForms("frmMain"), frmMain)
        If f IsNot Nothing AndAlso f.StatusStrip IsNot Nothing Then
            f.ToolStripStatusLabel1.Text = text
            f.ToolStripProgressBar1.Style = If(marquee, ProgressBarStyle.Marquee, ProgressBarStyle.Blocks)
            f.ToolStripProgressBar1.Visible = True
        End If
    End Sub

    Private Sub ClearStatus()
        Dim f As frmMain = TryCast(Application.OpenForms("frmMain"), frmMain)
        If f IsNot Nothing AndAlso f.StatusStrip IsNot Nothing Then
            f.ToolStripStatusLabel1.Text = "Ready"
            f.ToolStripProgressBar1.Visible = False
        End If
    End Sub

    ''' <summary>
    ''' Adds (or sets) a short message to the MainForm status strip label.
    ''' Safe to call from any thread (invokes if needed).
    ''' </summary>
    ''' <param name="text">Text to display</param>
    ''' <param name="append">True = add to existing text (with space), False = replace</param>
    ''' <param name="clearAfterMs">Optional auto-clear after X milliseconds (0 = never)</param>
    Public Sub Add(text As String,
                         Optional append As Boolean = True,
                         Optional clearAfterMs As Integer = 0)

        Dim main As frmMain = TryCast(Application.OpenForms("frmMain"), frmMain)
        If main Is Nothing OrElse main.IsDisposed Then Return

        Dim label As ToolStripStatusLabel = main.ToolStripStatusLabel1
        If label Is Nothing Then Return
        append = False
        Dim finalText As String = If(append AndAlso Not String.IsNullOrWhiteSpace(label.Text) AndAlso label.Text.TrimEnd() <> "Ready",
                                    label.Text.TrimEnd() & "  •  " & text,
                                    text)

        ' Thread-safe invoke
        If main.InvokeRequired Then
            main.Invoke(Sub()
                            label.Text = finalText
                            If clearAfterMs > 0 Then
                                Dim timer As New Timer With {.Interval = clearAfterMs, .Tag = label}
                                AddHandler timer.Tick, Sub(s, e)
                                                           Dim t As Timer = DirectCast(s, Timer)
                                                           Dim lbl As ToolStripStatusLabel = DirectCast(t.Tag, ToolStripStatusLabel)
                                                           lbl.Text = "Ready"
                                                           t.Stop()
                                                           t.Dispose()
                                                       End Sub
                                timer.Start()
                            End If
                        End Sub)
        Else
            label.Text = finalText
            If clearAfterMs > 0 Then
                Dim timer As New Timer With {.Interval = clearAfterMs}
                AddHandler timer.Tick, Sub()
                                           label.Text = "Ready"
                                           timer.Stop()
                                           timer.Dispose()
                                       End Sub
                timer.Start()
            End If
        End If
    End Sub

End Module
