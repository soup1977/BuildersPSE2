Option Strict On
Imports System.Windows.Forms

Public Class frmAdminUnlockDialog

    Public Property UnlockReason As String

    Public Sub New(versionName As String, lockedBy As String, lockedDate As DateTime?)
        InitializeComponent()

        lblWarning.Text = $"⚠️ ADMIN OVERRIDE WARNING ⚠️" & vbCrLf & vbCrLf &
                         $"You are about to UNLOCK version '{versionName}'" & vbCrLf & vbCrLf &
                         $"Originally locked by: {lockedBy}" & vbCrLf &
                         $"Locked on: {If(lockedDate.HasValue, lockedDate.Value.ToString("g"), "Unknown")}" & vbCrLf & vbCrLf &
                         "This action will be logged for audit purposes."

        lblWarning.ForeColor = Color.DarkRed
        lblWarning.Font = New Font(lblWarning.Font.FontFamily, 10, FontStyle.Bold)
    End Sub

    Private Sub btnUnlock_Click(sender As Object, e As EventArgs) Handles btnUnlock.Click
        If String.IsNullOrWhiteSpace(txtReason.Text) Then
            MessageBox.Show("You must provide a reason for unlocking.", "Reason Required",
                           MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtReason.Focus()
            Return
        End If

        UnlockReason = txtReason.Text.Trim()
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

End Class