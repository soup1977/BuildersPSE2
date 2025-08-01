Option Strict On
Imports System.Windows.Forms

Public Class frmVersionDialog

    Public Property VersionName As String
    Public Property Description As String

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        If String.IsNullOrWhiteSpace(txtVersionName.Text) Then
            MessageBox.Show("Version name is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        VersionName = txtVersionName.Text
        Description = txtDescription.Text
        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

End Class