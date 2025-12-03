Option Strict On

Imports System.Configuration
Imports BuildersPSE2.Utilities          ' For SecurityHelper
Imports BuildersPSE2.BuildersPSE.Models


Public Class frmMondaySearch

    ' This will be returned to the caller
    Public Property SelectedMondayItemId As String = String.Empty
    Public Property InitialSearchText As String = String.Empty



    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        Dim searchText = txtSearch.Text.Trim()
        If searchText.Length < 3 Then
            MessageBox.Show("Please enter at least 3 characters to search.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        btnSearch.Enabled = False
        btnSearch.Text = "Searching..."
        dgvResults.DataSource = Nothing
        Cursor = Cursors.WaitCursor

        Try
            Dim encryptedToken As String = ConfigurationManager.AppSettings("MondayApiTokenEncrypted")
            If String.IsNullOrEmpty(encryptedToken) Then Throw New Exception("Monday.com API token not found in configuration.")
            Dim apiToken As String = SecurityHelper.DecryptApiKey(encryptedToken)
            If String.IsNullOrEmpty(apiToken) Then Throw New Exception("Failed to decrypt Monday.com API token.")

            Dim accessor As New MondaycomAccess(apiToken)
            Dim items As List(Of MondayBoardItem) = accessor.SearchMondayItems("6930311385", searchText)

            ' FIXED: Only show the 4 columns you want
            dgvResults.Columns.Clear()
            dgvResults.Columns.Add("ItemID", "Item ID")
            dgvResults.Columns.Add("Name", "Project Name")
            dgvResults.Columns.Add("Customer", "Customer")   ' column ID will be looked up by key below
            dgvResults.Columns.Add("Address", "Address")     ' column ID will be looked up by key below

            dgvResults.Rows.Clear()
            For Each item As MondayBoardItem In items
                Dim customerText As String = If(item.ColumnValues.ContainsKey("text9__1"), item.ColumnValues("text9__1"), String.Empty)  ' <<< REPLACE "customer_column_id" WITH YOUR ACTUAL COLUMN ID
                Dim addressText As String = If(item.ColumnValues.ContainsKey("text2__1"), item.ColumnValues("text2__1"), String.Empty)   ' <<< REPLACE "address_column_id" WITH YOUR ACTUAL COLUMN ID

                dgvResults.Rows.Add(item.ItemID, item.Name, customerText, addressText)
            Next

            lblResultCount.Text = $"{items.Count} result(s) found"
            btnSelect.Enabled = items.Count > 0

        Catch ex As Exception
            MessageBox.Show("Error searching monday.com: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            btnSearch.Enabled = True
            btnSearch.Text = "Search"
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub btnSelect_Click(sender As Object, e As EventArgs) Handles btnSelect.Click
        If dgvResults.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a row first.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        Dim selectedRow = dgvResults.SelectedRows(0)
        SelectedMondayItemId = selectedRow.Cells("ItemID").Value.ToString()

        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub frmMondaySearch_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If Not String.IsNullOrWhiteSpace(InitialSearchText) Then
            txtSearch.Text = InitialSearchText
            txtSearch.SelectAll()           ' optional: highlight it so user can type over
            ' Optional: auto-run search
            'btnSearch.PerformClick()
        End If
        btnSelect.Enabled = False
        lblResultCount.Text = ""
    End Sub
End Class