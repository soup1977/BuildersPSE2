Imports System.Configuration
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.BuildersPSE.Utilities


Public Class frmMondayList

    Public Sub New()
        InitializeComponent()
    End Sub

    Private Sub frmMondayList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            Dim encryptedToken As String = ConfigurationManager.AppSettings("MondayApiTokenEncrypted")
            If String.IsNullOrEmpty(encryptedToken) Then
                Throw New Exception("Monday.com API token not found in configuration.")
            End If
            Dim apiToken As String = SecurityHelper.DecryptApiKey(encryptedToken)
            If String.IsNullOrEmpty(apiToken) Then
                Throw New Exception("Failed to decrypt Monday.com API token.")
            End If

            Dim accessor As New MondaycomAccess(apiToken)
            Dim items As List(Of MondayBoardItem) = accessor.GetTop50BoardItems("6930311385") ' Replace with your board ID


            ' Configure DataGridView columns
            dgvMondayItems.Columns.Clear()
            dgvMondayItems.Columns.Add("ItemID", "Item ID")
            dgvMondayItems.Columns.Add("Name", "Item Name")

            ' Dynamically add columns for unique column IDs
            Dim columnIds As New HashSet(Of String)
            For Each item In items
                For Each col In item.ColumnValues
                    If Not columnIds.Contains(col.Key) Then
                        columnIds.Add(col.Key)
                        dgvMondayItems.Columns.Add(col.Key, col.Key)
                    End If
                Next
            Next

            ' Populate rows
            dgvMondayItems.Rows.Clear()
            For Each item As MondayBoardItem In items
                Dim row As New List(Of String) From {item.ItemID, item.Name}
                For Each colId In columnIds
                    ' Fixed: Replace GetValueOrDefault with ContainsKey check
                    row.Add(If(item.ColumnValues.ContainsKey(colId), item.ColumnValues(colId), String.Empty))
                Next
                dgvMondayItems.Rows.Add(row.ToArray())
            Next

        Catch ex As Exception
            MessageBox.Show("Error loading monday.com items: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
End Class