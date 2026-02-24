Option Strict On

''' <summary>
''' Dialog for selecting two snapshots to compare
''' </summary>
Public Class frmSelectSnapshotsToCompare

    Private _snapshots As List(Of PriceHistoryModel)
    Public Property SelectedSnapshot1 As PriceHistoryModel
    Public Property SelectedSnapshot2 As PriceHistoryModel

    Public Sub New(snapshots As List(Of PriceHistoryModel))
        InitializeComponent()
        _snapshots = snapshots
    End Sub

    Private Sub frmSelectSnapshotsToCompare_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Populate combo boxes
        cboSnapshot1.DataSource = _snapshots.ToList()
        cboSnapshot1.DisplayMember = "DisplayText"
        cboSnapshot1.ValueMember = "PriceHistoryID"

        cboSnapshot2.DataSource = _snapshots.ToList()
        cboSnapshot2.DisplayMember = "DisplayText"
        cboSnapshot2.ValueMember = "PriceHistoryID"

        ' Default: select first two different snapshots
        If _snapshots.Count >= 2 Then
            cboSnapshot1.SelectedIndex = 0
            cboSnapshot2.SelectedIndex = 1
        End If
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If cboSnapshot1.SelectedItem Is Nothing OrElse cboSnapshot2.SelectedItem Is Nothing Then
            MessageBox.Show("Please select two snapshots to compare.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        SelectedSnapshot1 = CType(cboSnapshot1.SelectedItem, PriceHistoryModel)
        SelectedSnapshot2 = CType(cboSnapshot2.SelectedItem, PriceHistoryModel)

        If SelectedSnapshot1.PriceHistoryID = SelectedSnapshot2.PriceHistoryID Then
            MessageBox.Show("Please select two different snapshots to compare.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

End Class