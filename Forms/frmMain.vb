' Replace the entire frmMain.vb with the following updated code
Option Strict On

Imports System.Windows.Forms

Public Class frmMain
    Private m_ChildFormNumber As Integer

    Public ReadOnly Property StatusLabel As ToolStripStatusLabel
        Get
            Return CType(StatusStrip.Items("StatusLabel"), ToolStripStatusLabel)
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
End Class