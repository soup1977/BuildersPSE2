' =====================================================
' frmOptionManagement.vb
' Manage Structural Options (Oversized Garage, Outdoor Room, etc.)
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models
Imports OpenQA.Selenium.Edge

Partial Public Class frmOptionManagement
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _options As List(Of PLOption)

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

    Private Sub frmOptionManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()
        LoadOptions()
        ClearEditFields()
    End Sub

    Private Sub dgvOptions_SelectionChanged(sender As Object, e As EventArgs) Handles dgvOptions.SelectionChanged
        LoadSelectedOption()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        AddOption()
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        UpdateOption()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

#End Region

#Region "Grid Setup"

    Private Sub SetupGrid()
        dgvOptions.Columns.Clear()
        dgvOptions.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "OptionID", .DataPropertyName = "OptionID", .Visible = False})
        dgvOptions.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "OptionName", .HeaderText = "Option Name", .DataPropertyName = "OptionName", .Width = 180})
        dgvOptions.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "OptionDescription", .HeaderText = "Description", .DataPropertyName = "OptionDescription", .Width = 150})
        dgvOptions.Columns.Add(New DataGridViewCheckBoxColumn() With {.Name = "IsActive", .HeaderText = "Active", .DataPropertyName = "IsActive", .Width = 50})
    End Sub

#End Region

#Region "Data Operations"

    Private Sub LoadOptions()
        Try
            _options = _dataAccess.GetOptions()
            dgvOptions.DataSource = Nothing
            dgvOptions.DataSource = _options
        Catch ex As Exception
            MessageBox.Show($"Error loading options: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadSelectedOption()
        If dgvOptions.SelectedRows.Count = 0 OrElse dgvOptions.SelectedRows(0).Index >= _options.Count Then
            ClearEditFields()
            Return
        End If

        Dim opt = _options(dgvOptions.SelectedRows(0).Index)
        txtOptionName.Text = opt.OptionName
        txtOptionDescription.Text = opt.OptionDescription
        chkIsActive.Checked = opt.IsActive
    End Sub

    Private Sub ClearEditFields()
        txtOptionName.Text = ""
        txtOptionDescription.Text = ""
        chkIsActive.Checked = True
        lblStatus.Text = ""
    End Sub

    Private Sub AddOption()
        If String.IsNullOrWhiteSpace(txtOptionName.Text) Then
            MessageBox.Show("Please enter an option name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtOptionName.Focus()
            Return
        End If

        Try
            Dim opt As New PLOption() With {
                .OptionName = txtOptionName.Text.Trim(),
                .OptionDescription = txtOptionDescription.Text.Trim(),
                .IsActive = chkIsActive.Checked
            }
            opt.OptionID = _dataAccess.InsertOption(opt)

            LoadOptions()
            SelectOptionInGrid(opt.OptionID)
            lblStatus.Text = "Option added."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error adding option: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateOption()
        If dgvOptions.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an option to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(txtOptionName.Text) Then
            MessageBox.Show("Please enter an option name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtOptionName.Focus()
            Return
        End If

        Try
            Dim opt = _options(dgvOptions.SelectedRows(0).Index)
            opt.OptionName = txtOptionName.Text.Trim()
            opt.OptionDescription = txtOptionDescription.Text.Trim()
            opt.IsActive = chkIsActive.Checked

            _dataAccess.UpdateOption(opt)

            LoadOptions()
            SelectOptionInGrid(opt.OptionID)
            lblStatus.Text = "Option updated."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error updating option: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SelectOptionInGrid(optionID As Integer)
        For i = 0 To dgvOptions.Rows.Count - 1
            If CInt(dgvOptions.Rows(i).Cells("OptionID").Value) = optionID Then
                dgvOptions.Rows(i).Selected = True
                dgvOptions.FirstDisplayedScrollingRowIndex = i
                Exit For
            End If
        Next
    End Sub

#End Region

End Class