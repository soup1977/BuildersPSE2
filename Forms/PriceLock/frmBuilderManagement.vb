' =====================================================
' frmBuilderManagement.vb
' Manage Builders (DR Horton, Trumark, etc.)
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Public Class frmBuilderManagement

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _builders As List(Of PLBuilder)

#End Region

#Region "Constructor"

    Public Sub New()
        _dataAccess = New PriceLockDataAccess()
        InitializeComponent()
    End Sub

    Public Sub New(dataAccess As PriceLockDataAccess)
        _dataAccess = dataAccess
        InitializeComponent()
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmBuilderManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()
        LoadBuilders()
        ClearEditFields()
    End Sub

    Private Sub dgvBuilders_SelectionChanged(sender As Object, e As EventArgs) Handles dgvBuilders.SelectionChanged
        LoadSelectedBuilder()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        AddBuilder()
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        UpdateBuilder()
    End Sub

    Private Sub btnManageSubdivisions_Click(sender As Object, e As EventArgs) Handles btnManageSubdivisions.Click
        ManageSubdivisions()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

#End Region

#Region "Helper Methods"

    Private Sub SetupGrid()
        ' Grid columns are now defined in the designer
        ' This method can be removed or kept for additional runtime configuration
    End Sub

#End Region

#Region "Data Operations"

    Private Sub LoadBuilders()
        Try
            _builders = _dataAccess.GetBuilders()
            dgvBuilders.DataSource = Nothing
            dgvBuilders.DataSource = _builders
        Catch ex As Exception
            MessageBox.Show($"Error loading builders: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadSelectedBuilder()
        If dgvBuilders.SelectedRows.Count = 0 OrElse dgvBuilders.SelectedRows(0).Index >= _builders.Count Then
            ClearEditFields()
            Return
        End If

        Dim builder = _builders(dgvBuilders.SelectedRows(0).Index)
        txtBuilderName.Text = builder.BuilderName
        txtBuilderCode.Text = builder.BuilderCode
        chkIsActive.Checked = builder.IsActive
        btnManageSubdivisions.Enabled = True
    End Sub

    Private Sub ClearEditFields()
        txtBuilderName.Text = ""
        txtBuilderCode.Text = ""
        chkIsActive.Checked = True
        btnManageSubdivisions.Enabled = False
        lblStatus.Text = ""
    End Sub

    Private Sub AddBuilder()
        If String.IsNullOrWhiteSpace(txtBuilderName.Text) Then
            MessageBox.Show("Please enter a builder name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtBuilderName.Focus()
            Return
        End If

        Try
            Dim builder As New PLBuilder() With {
                .BuilderName = txtBuilderName.Text.Trim(),
                .BuilderCode = txtBuilderCode.Text.Trim(),
                .IsActive = chkIsActive.Checked
            }
            builder.BuilderID = _dataAccess.InsertBuilder(builder)

            LoadBuilders()
            SelectBuilderInGrid(builder.BuilderID)
            lblStatus.Text = "Builder added successfully."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error adding builder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateBuilder()
        If dgvBuilders.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a builder to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(txtBuilderName.Text) Then
            MessageBox.Show("Please enter a builder name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtBuilderName.Focus()
            Return
        End If

        Try
            Dim builder = _builders(dgvBuilders.SelectedRows(0).Index)
            builder.BuilderName = txtBuilderName.Text.Trim()
            builder.BuilderCode = txtBuilderCode.Text.Trim()
            builder.IsActive = chkIsActive.Checked

            _dataAccess.UpdateBuilder(builder)

            LoadBuilders()
            SelectBuilderInGrid(builder.BuilderID)
            lblStatus.Text = "Builder updated successfully."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error updating builder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SelectBuilderInGrid(builderID As Integer)
        For i = 0 To dgvBuilders.Rows.Count - 1
            If CInt(dgvBuilders.Rows(i).Cells("BuilderID").Value) = builderID Then
                dgvBuilders.Rows(i).Selected = True
                dgvBuilders.FirstDisplayedScrollingRowIndex = i
                Exit For
            End If
        Next
    End Sub

    Private Sub ManageSubdivisions()
        If dgvBuilders.SelectedRows.Count = 0 Then Return

        Dim builder = _builders(dgvBuilders.SelectedRows(0).Index)
        Using frm As New frmSubdivisionManagement(builder, _dataAccess)
            frm.ShowDialog()
        End Using
    End Sub

#End Region

End Class