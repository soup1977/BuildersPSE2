Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.BuildersPSE.Utilities

Public Class frmImportProjectDialog
    Private Sub frmImportProjectDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Populate Customer ComboBox (CustomerType=1)
        Dim customers As New DataTable()
        Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
            conn.Open()
            Dim params As New Dictionary(Of String, Object) From {{"@CustomerType", 1}}
            Dim cmd As New SqlCommand(Queries.SelectCustomers, conn)
            cmd.Parameters.Add(New SqlParameter("@CustomerType", 1))
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(customers)
        End Using
        With cboCustomerName
            .DisplayMember = "CustomerName"
            .ValueMember = "CustomerID"
            .DataSource = customers
            .SelectedIndex = -1 ' Optional
        End With

        ' Populate Estimator ComboBox
        Dim estimators As New DataTable()
        Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
            conn.Open()
            Dim cmd As New SqlCommand(Queries.SelectEstimators, conn)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(estimators)
        End Using
        With cboEstimator
            .DisplayMember = "EstimatorName"
            .ValueMember = "EstimatorID"
            .DataSource = estimators
            .SelectedIndex = -1 ' Optional
        End With

        ' Populate Sales ComboBox
        Dim sales As New DataTable()
        Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
            conn.Open()
            Dim cmd As New SqlCommand(Queries.SelectSales, conn)
            Dim da As New SqlDataAdapter(cmd)
            da.Fill(sales)
        End Using
        With cboSales
            .DisplayMember = "SalesName"
            .ValueMember = "SalesID"
            .DataSource = sales
            .SelectedIndex = -1 ' Optional
        End With
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If String.IsNullOrWhiteSpace(txtProjectName.Text) Then
            MessageBox.Show("Project Name is required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If
        If String.IsNullOrWhiteSpace(cboCustomerName.Text) Then
            MessageBox.Show("Customer Name is required.", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        DialogResult = DialogResult.OK
        Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub
End Class