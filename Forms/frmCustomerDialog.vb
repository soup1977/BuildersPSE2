Option Strict On
Imports System.Windows.Forms
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.BuildersPSE.Models

Public Class FrmCustomerDialog
    Private ReadOnly da As New ProjectDataAccess()
    Private ReadOnly customer As CustomerModel

    Public ReadOnly Property CustomerName As String
        Get
            Return txtCustomerName.Text
        End Get
    End Property

    Public ReadOnly Property CustomerTypeID As Integer?
        Get
            If cboCustomerType.SelectedValue IsNot Nothing AndAlso cboCustomerType.SelectedValue IsNot DBNull.Value Then
                Return CInt(cboCustomerType.SelectedValue)
            End If
            Return Nothing
        End Get
    End Property

    Public Sub New(Optional selectedCustomer As CustomerModel = Nothing, Optional defaultTypeID As Integer? = Nothing)
        InitializeComponent()
        cboCustomerType.DataSource = HelperDataAccess.GetCustomerTypes()
        cboCustomerType.DisplayMember = "CustomerTypeName"
        cboCustomerType.ValueMember = "CustomerTypeID"

        If selectedCustomer IsNot Nothing Then
            customer = selectedCustomer
            txtCustomerName.Text = customer.CustomerName
            cboCustomerType.SelectedValue = If(customer.CustomerTypeID, 0)
            Me.Text = "Edit Customer"
        Else
            customer = New CustomerModel()
            Me.Text = "Add Customer"
            If defaultTypeID.HasValue Then
                cboCustomerType.SelectedValue = defaultTypeID.Value
                cboCustomerType.Enabled = False ' Lock type for context-specific Add
            End If
        End If
    End Sub

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        Try
            If String.IsNullOrWhiteSpace(txtCustomerName.Text) Then
                MessageBox.Show("Customer name is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                txtCustomerName.Focus()
                Return
            End If
            If cboCustomerType.SelectedValue Is Nothing OrElse cboCustomerType.SelectedValue Is DBNull.Value Then
                MessageBox.Show("Customer type is required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cboCustomerType.Focus()
                Return
            End If

            customer.CustomerName = txtCustomerName.Text
            customer.CustomerTypeID = CInt(cboCustomerType.SelectedValue)
            customer.CustomerID = HelperDataAccess.SaveCustomer(customer, customer.CustomerTypeID)

            DialogResult = DialogResult.OK
            Close()
        Catch ex As Exception
            MessageBox.Show("Error saving customer: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub
End Class