' =====================================================
' frmSubdivisionManagement.vb
' Manage Subdivisions for a Builder
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Partial Public Class frmSubdivisionManagement
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _builder As PLBuilder
    Private _subdivisions As List(Of PLSubdivision)

#End Region

#Region "Constructor"

    Public Sub New(builder As PLBuilder, dataAccess As PriceLockDataAccess)
        _builder = builder
        _dataAccess = dataAccess
        InitializeComponent()
        Me.Text = $"Subdivision Management - {_builder.BuilderName}"
        lblBuilderName.Text = $"Builder: {_builder.BuilderName}"
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmSubdivisionManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()
        LoadSubdivisions()
        ClearEditFields()
    End Sub

    Private Sub dgvSubdivisions_SelectionChanged(sender As Object, e As EventArgs) Handles dgvSubdivisions.SelectionChanged
        LoadSelectedSubdivision()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        AddSubdivision()
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        UpdateSubdivision()
    End Sub

    Private Sub btnManagePlans_Click(sender As Object, e As EventArgs) Handles btnManagePlans.Click
        ManagePlans()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

#End Region

#Region "Grid Setup"

    Private Sub SetupGrid()
        dgvSubdivisions.Columns.Clear()
        dgvSubdivisions.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "SubdivisionID", .DataPropertyName = "SubdivisionID", .Visible = False})
        dgvSubdivisions.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "SubdivisionName", .HeaderText = "Subdivision Name", .DataPropertyName = "SubdivisionName", .Width = 200})
        dgvSubdivisions.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "SubdivisionCode", .HeaderText = "Code", .DataPropertyName = "SubdivisionCode", .Width = 100})
        dgvSubdivisions.Columns.Add(New DataGridViewCheckBoxColumn() With {.Name = "IsActive", .HeaderText = "Active", .DataPropertyName = "IsActive", .Width = 60})
    End Sub

#End Region

#Region "Data Operations"

    Private Sub LoadSubdivisions()
        Try
            _subdivisions = _dataAccess.GetSubdivisionsByBuilder(_builder.BuilderID)
            dgvSubdivisions.DataSource = Nothing
            dgvSubdivisions.DataSource = _subdivisions
        Catch ex As Exception
            MessageBox.Show($"Error loading subdivisions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadSelectedSubdivision()
        If dgvSubdivisions.SelectedRows.Count = 0 OrElse dgvSubdivisions.SelectedRows(0).Index >= _subdivisions.Count Then
            ClearEditFields()
            Return
        End If

        Dim subdivision = _subdivisions(dgvSubdivisions.SelectedRows(0).Index)
        txtSubdivisionName.Text = subdivision.SubdivisionName
        txtSubdivisionCode.Text = subdivision.SubdivisionCode
        chkIsActive.Checked = subdivision.IsActive
        btnManagePlans.Enabled = True
    End Sub

    Private Sub ClearEditFields()
        txtSubdivisionName.Text = ""
        txtSubdivisionCode.Text = ""
        chkIsActive.Checked = True
        btnManagePlans.Enabled = False
        lblStatus.Text = ""
    End Sub

    Private Sub AddSubdivision()
        If String.IsNullOrWhiteSpace(txtSubdivisionName.Text) Then
            MessageBox.Show("Please enter a subdivision name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSubdivisionName.Focus()
            Return
        End If

        Try
            Dim subdivision As New PLSubdivision() With {
                .BuilderID = _builder.BuilderID,
                .SubdivisionName = txtSubdivisionName.Text.Trim(),
                .SubdivisionCode = txtSubdivisionCode.Text.Trim(),
                .IsActive = chkIsActive.Checked
            }
            subdivision.SubdivisionID = _dataAccess.InsertSubdivision(subdivision)

            LoadSubdivisions()
            SelectSubdivisionInGrid(subdivision.SubdivisionID)
            lblStatus.Text = "Subdivision added successfully."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error adding subdivision: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateSubdivision()
        If dgvSubdivisions.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a subdivision to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(txtSubdivisionName.Text) Then
            MessageBox.Show("Please enter a subdivision name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtSubdivisionName.Focus()
            Return
        End If

        Try
            Dim subdivision = _subdivisions(dgvSubdivisions.SelectedRows(0).Index)
            subdivision.SubdivisionName = txtSubdivisionName.Text.Trim()
            subdivision.SubdivisionCode = txtSubdivisionCode.Text.Trim()
            subdivision.IsActive = chkIsActive.Checked

            _dataAccess.UpdateSubdivision(subdivision)

            LoadSubdivisions()
            SelectSubdivisionInGrid(subdivision.SubdivisionID)
            lblStatus.Text = "Subdivision updated successfully."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error updating subdivision: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SelectSubdivisionInGrid(subdivisionID As Integer)
        For i = 0 To dgvSubdivisions.Rows.Count - 1
            If CInt(dgvSubdivisions.Rows(i).Cells("SubdivisionID").Value) = subdivisionID Then
                dgvSubdivisions.Rows(i).Selected = True
                dgvSubdivisions.FirstDisplayedScrollingRowIndex = i
                Exit For
            End If
        Next
    End Sub

    Private Sub ManagePlans()
        If dgvSubdivisions.SelectedRows.Count = 0 Then Return

        Dim subdivision = _subdivisions(dgvSubdivisions.SelectedRows(0).Index)
        Using frm As New frmSubdivisionPlanAssignment(subdivision, _dataAccess)
            frm.ShowDialog()
        End Using
    End Sub

#End Region

End Class