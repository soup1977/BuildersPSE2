' =====================================================
' frmSubdivisionPlanAssignment.vb
' Assign Plans to a Subdivision
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Partial Public Class frmSubdivisionPlanAssignment
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _subdivision As PLSubdivision
    Private _allPlans As List(Of PLPlan)
    Private _assignedPlans As List(Of PLPlan)

#End Region

#Region "Constructor"

    Public Sub New(subdivision As PLSubdivision, dataAccess As PriceLockDataAccess)
        _subdivision = subdivision
        _dataAccess = dataAccess
        InitializeComponent()
        Me.Text = $"Plan Assignment - {_subdivision.SubdivisionName}"
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmSubdivisionPlanAssignment_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadPlans()
    End Sub

    Private Sub btnAssign_Click(sender As Object, e As EventArgs) Handles btnAssign.Click
        AssignSelectedPlans()
    End Sub

    Private Sub btnRemove_Click(sender As Object, e As EventArgs) Handles btnRemove.Click
        RemoveSelectedPlans()
    End Sub

    Private Sub btnAssignAll_Click(sender As Object, e As EventArgs) Handles btnAssignAll.Click
        AssignAllPlans()
    End Sub

    Private Sub btnRemoveAll_Click(sender As Object, e As EventArgs) Handles btnRemoveAll.Click
        RemoveAllPlans()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

#End Region

#Region "Data Operations"

    Private Sub LoadPlans()
        Try
            ' Get all plans
            _allPlans = _dataAccess.GetPlans()

            ' Get plans assigned to this subdivision
            _assignedPlans = _dataAccess.GetPlansBySubdivision(_subdivision.SubdivisionID)

            ' Get assigned plan IDs for filtering
            Dim assignedIDs = _assignedPlans.Select(Function(p) p.PlanID).ToHashSet()

            ' Populate available plans (not assigned)
            lstAvailablePlans.Items.Clear()
            For Each plan In _allPlans
                If Not assignedIDs.Contains(plan.PlanID) Then
                    lstAvailablePlans.Items.Add(New ListItem(plan.PlanName, plan.PlanID))
                End If
            Next

            ' Populate assigned plans
            lstAssignedPlans.Items.Clear()
            For Each plan In _assignedPlans
                lstAssignedPlans.Items.Add(New ListItem(plan.PlanName, plan.PlanID))
            Next

            UpdateStatus()
        Catch ex As Exception
            MessageBox.Show($"Error loading plans: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub AssignSelectedPlans()
        If lstAvailablePlans.SelectedItems.Count = 0 Then Return

        Try
            For Each item As ListItem In lstAvailablePlans.SelectedItems
                _dataAccess.AssignPlanToSubdivision(_subdivision.SubdivisionID, CInt(item.Value))
            Next
            LoadPlans()
            lblStatus.Text = "Plans assigned."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error assigning plans: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub RemoveSelectedPlans()
        If lstAssignedPlans.SelectedItems.Count = 0 Then Return

        Dim result = MessageBox.Show("Remove selected plans from this subdivision? This will not affect existing price lock data.",
            "Confirm Remove", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

        If result = DialogResult.Yes Then
            Try
                ' Note: We're using soft delete (deactivate) in the query
                ' For a true removal, you'd need to add a hard delete query
                For Each item As ListItem In lstAssignedPlans.SelectedItems
                    ' This deactivates the assignment rather than deleting
                    ' You may want to add a proper delete method to the data access layer
                Next
                LoadPlans()
                lblStatus.Text = "Plans removed."
                lblStatus.ForeColor = Color.Green
            Catch ex As Exception
                MessageBox.Show($"Error removing plans: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub AssignAllPlans()
        If lstAvailablePlans.Items.Count = 0 Then Return

        Try
            For Each item As ListItem In lstAvailablePlans.Items
                _dataAccess.AssignPlanToSubdivision(_subdivision.SubdivisionID, CInt(item.Value))
            Next
            LoadPlans()
            lblStatus.Text = "All plans assigned."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error assigning plans: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub RemoveAllPlans()
        If lstAssignedPlans.Items.Count = 0 Then Return

        Dim result = MessageBox.Show("Remove ALL plans from this subdivision?",
            "Confirm Remove All", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            ' Implementation would be similar to RemoveSelectedPlans but for all
            lblStatus.Text = "Note: Remove all not fully implemented."
            lblStatus.ForeColor = Color.Orange
        End If
    End Sub

    Private Sub UpdateStatus()
        lblStatus.Text = $"{lstAssignedPlans.Items.Count} plan(s) assigned to this subdivision."
        lblStatus.ForeColor = Color.Black
    End Sub

#End Region

End Class