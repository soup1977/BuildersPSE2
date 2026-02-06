' =====================================================
' frmPlanManagementNew.vb
' Manage Plans and their Elevations
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Public Class frmPlanManagement

    Private _dataAccess As PriceLockDataAccess
    Private _plans As List(Of PLPlan)
    Private _elevations As List(Of PLElevation)

    Public Sub New()
        InitializeComponent()
        _dataAccess = New PriceLockDataAccess()
    End Sub

    Public Sub New(dataAccess As PriceLockDataAccess)
        InitializeComponent()
        _dataAccess = dataAccess
    End Sub

    Private Sub frmPlanManagementNew_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrids()
        LoadPlans()
        ClearPlanFields()
        ClearElevationFields()
    End Sub

    Private Sub SetupGrids()
        ' Plans grid
        dgvPlans.Columns.Clear()
        dgvPlans.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "PlanID", .DataPropertyName = "PlanID", .Visible = False})
        dgvPlans.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "PlanName", .HeaderText = "Plan", .DataPropertyName = "PlanName", .FillWeight = 50})
        dgvPlans.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "SquareFootage", .HeaderText = "Sq Ft", .DataPropertyName = "SquareFootage", .FillWeight = 25})
        dgvPlans.Columns.Add(New DataGridViewCheckBoxColumn() With {.Name = "IsActive", .HeaderText = "Active", .DataPropertyName = "IsActive", .FillWeight = 25})

        ' Elevations grid
        dgvElevations.Columns.Clear()
        dgvElevations.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ElevationID", .DataPropertyName = "ElevationID", .Visible = False})
        dgvElevations.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ElevationName", .HeaderText = "Elevation", .DataPropertyName = "ElevationName", .FillWeight = 70})
        dgvElevations.Columns.Add(New DataGridViewCheckBoxColumn() With {.Name = "IsActive", .HeaderText = "Active", .DataPropertyName = "IsActive", .FillWeight = 30})
    End Sub

    Private Sub dgvPlans_SelectionChanged(sender As Object, e As EventArgs) Handles dgvPlans.SelectionChanged
        LoadSelectedPlan()
        LoadElevations()
    End Sub

    Private Sub dgvElevations_SelectionChanged(sender As Object, e As EventArgs) Handles dgvElevations.SelectionChanged
        LoadSelectedElevation()
    End Sub

    Private Sub btnAddPlan_Click(sender As Object, e As EventArgs) Handles btnAddPlan.Click
        AddPlan()
    End Sub

    Private Sub btnUpdatePlan_Click(sender As Object, e As EventArgs) Handles btnUpdatePlan.Click
        UpdatePlan()
    End Sub

    Private Sub btnAddElevation_Click(sender As Object, e As EventArgs) Handles btnAddElevation.Click
        AddElevation()
    End Sub

    Private Sub btnUpdateElevation_Click(sender As Object, e As EventArgs) Handles btnUpdateElevation.Click
        UpdateElevation()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

#Region "Plan Operations"

    Private Sub LoadPlans()
        Try
            _plans = _dataAccess.GetPlans()
            dgvPlans.DataSource = Nothing
            dgvPlans.DataSource = _plans
        Catch ex As Exception
            MessageBox.Show($"Error loading plans: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadSelectedPlan()
        If dgvPlans.SelectedRows.Count = 0 OrElse dgvPlans.SelectedRows(0).Index >= _plans.Count Then
            ClearPlanFields()
            Return
        End If

        Dim plan = _plans(dgvPlans.SelectedRows(0).Index)
        txtPlanName.Text = plan.PlanName
        txtPlanDescription.Text = plan.PlanDescription
        nudSquareFootage.Value = If(plan.SquareFootage, 0)
        chkPlanIsActive.Checked = plan.IsActive
    End Sub

    Private Sub ClearPlanFields()
        txtPlanName.Text = ""
        txtPlanDescription.Text = ""
        nudSquareFootage.Value = 0
        chkPlanIsActive.Checked = True
    End Sub

    Private Sub AddPlan()
        If String.IsNullOrWhiteSpace(txtPlanName.Text) Then
            MessageBox.Show("Please enter a plan name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPlanName.Focus()
            Return
        End If

        Try
            Dim plan As New PLPlan() With {
                .PlanName = txtPlanName.Text.Trim(),
                .PlanDescription = txtPlanDescription.Text.Trim(),
                .SquareFootage = If(nudSquareFootage.Value > 0, CType(CInt(nudSquareFootage.Value), Integer?), Nothing),
                .IsActive = chkPlanIsActive.Checked
            }
            plan.PlanID = _dataAccess.InsertPlan(plan)

            LoadPlans()
            SelectPlanInGrid(plan.PlanID)
            lblStatus.Text = "Plan added."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error adding plan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdatePlan()
        If dgvPlans.SelectedRows.Count = 0 Then Return

        If String.IsNullOrWhiteSpace(txtPlanName.Text) Then
            MessageBox.Show("Please enter a plan name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPlanName.Focus()
            Return
        End If

        Try
            Dim plan = _plans(dgvPlans.SelectedRows(0).Index)
            plan.PlanName = txtPlanName.Text.Trim()
            plan.PlanDescription = txtPlanDescription.Text.Trim()
            plan.SquareFootage = If(nudSquareFootage.Value > 0, CType(CInt(nudSquareFootage.Value), Integer?), Nothing)
            plan.IsActive = chkPlanIsActive.Checked

            _dataAccess.UpdatePlan(plan)

            LoadPlans()
            SelectPlanInGrid(plan.PlanID)
            lblStatus.Text = "Plan updated."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error updating plan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SelectPlanInGrid(planID As Integer)
        For i = 0 To dgvPlans.Rows.Count - 1
            If CInt(dgvPlans.Rows(i).Cells("PlanID").Value) = planID Then
                dgvPlans.Rows(i).Selected = True
                dgvPlans.FirstDisplayedScrollingRowIndex = i
                Exit For
            End If
        Next
    End Sub

#End Region

#Region "Elevation Operations"

    Private Sub LoadElevations()
        _elevations = New List(Of PLElevation)()
        dgvElevations.DataSource = Nothing

        If dgvPlans.SelectedRows.Count = 0 OrElse dgvPlans.SelectedRows(0).Index >= _plans.Count Then
            Return
        End If

        Try
            Dim plan = _plans(dgvPlans.SelectedRows(0).Index)
            _elevations = _dataAccess.GetElevationsByPlan(plan.PlanID)
            dgvElevations.DataSource = _elevations
        Catch ex As Exception
            MessageBox.Show($"Error loading elevations: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try

        ClearElevationFields()
    End Sub

    Private Sub LoadSelectedElevation()
        If dgvElevations.SelectedRows.Count = 0 OrElse dgvElevations.SelectedRows(0).Index >= _elevations.Count Then
            ClearElevationFields()
            Return
        End If

        Dim elevation = _elevations(dgvElevations.SelectedRows(0).Index)
        txtElevationName.Text = elevation.ElevationName
        chkElevationIsActive.Checked = elevation.IsActive
    End Sub

    Private Sub ClearElevationFields()
        txtElevationName.Text = ""
        chkElevationIsActive.Checked = True
    End Sub

    Private Sub AddElevation()
        If dgvPlans.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a plan first.", "No Plan Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(txtElevationName.Text) Then
            MessageBox.Show("Please enter an elevation name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtElevationName.Focus()
            Return
        End If

        Try
            Dim plan = _plans(dgvPlans.SelectedRows(0).Index)
            Dim elevation As New PLElevation() With {
                .PlanID = plan.PlanID,
                .ElevationName = txtElevationName.Text.Trim(),
                .IsActive = chkElevationIsActive.Checked
            }
            elevation.ElevationID = _dataAccess.InsertElevation(elevation)

            LoadElevations()
            lblStatus.Text = "Elevation added."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error adding elevation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateElevation()
        If dgvElevations.SelectedRows.Count = 0 Then Return

        If String.IsNullOrWhiteSpace(txtElevationName.Text) Then
            MessageBox.Show("Please enter an elevation name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtElevationName.Focus()
            Return
        End If

        Try
            Dim elevation = _elevations(dgvElevations.SelectedRows(0).Index)
            elevation.ElevationName = txtElevationName.Text.Trim()
            elevation.IsActive = chkElevationIsActive.Checked

            _dataAccess.UpdateElevation(elevation)

            LoadElevations()
            lblStatus.Text = "Elevation updated."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error updating elevation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

End Class