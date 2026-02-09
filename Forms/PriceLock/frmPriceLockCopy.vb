' =====================================================
' frmPriceLockCopy.vb
' Dialog for copying an existing Price Lock
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Partial Public Class frmPriceLockCopy
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _sourcePriceLock As PLPriceLock
    Private _newPriceLockID As Integer

#End Region

#Region "Properties"

    Public ReadOnly Property NewPriceLockID As Integer
        Get
            Return _newPriceLockID
        End Get
    End Property

#End Region

#Region "Constructor"

    Public Sub New(sourcePriceLock As PLPriceLock, dataAccess As PriceLockDataAccess)
        _sourcePriceLock = sourcePriceLock
        _dataAccess = dataAccess

        InitializeComponent()
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmPriceLockCopy_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Display source info
        lblSourceInfo.Text = $"{_sourcePriceLock.BuilderName} - {_sourcePriceLock.SubdivisionName} ({_sourcePriceLock.PriceLockDate:d})"

        ' Load subdivisions
        LoadSubdivisions()

        ' Set default date to today
        dtpLockDate.Value = DateTime.Today

        ' Generate default name
        GenerateLockName()
    End Sub

    Private Sub chkSameSubdivision_CheckedChanged(sender As Object, e As EventArgs) Handles chkSameSubdivision.CheckedChanged
        cboTargetSubdivision.Enabled = Not chkSameSubdivision.Checked

        If chkSameSubdivision.Checked Then
            ' Select the source subdivision
            For i = 0 To cboTargetSubdivision.Items.Count - 1
                Dim item = TryCast(cboTargetSubdivision.Items(i), ListItem)
                If item IsNot Nothing AndAlso CInt(item.Value) = _sourcePriceLock.SubdivisionID Then
                    cboTargetSubdivision.SelectedIndex = i
                    Exit For
                End If
            Next
        End If
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If ValidateInput() Then
            CopyPriceLock()
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadSubdivisions()
        cboTargetSubdivision.Items.Clear()

        For Each sub_ In _dataAccess.GetAllActiveSubdivisions()
            cboTargetSubdivision.Items.Add(New ListItem($"{sub_.BuilderName} - {sub_.DisplayName}", sub_.SubdivisionID))

            ' Select the source subdivision by default
            If sub_.SubdivisionID = _sourcePriceLock.SubdivisionID Then
                cboTargetSubdivision.SelectedIndex = cboTargetSubdivision.Items.Count - 1
            End If
        Next
    End Sub

    Private Sub GenerateLockName()
        ' Generate a default lock name like "Jan 2026 Pricelock"
        Dim lockDate = dtpLockDate.Value
        txtLockName.Text = $"{lockDate:MMM yyyy} Pricelock"
    End Sub

#End Region

#Region "Validation and Copy"

    Private Function ValidateInput() As Boolean
        If cboTargetSubdivision.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a target subdivision.", "Validation",
                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cboTargetSubdivision.Focus()
            Return False
        End If

        Dim selectedSubdivision = TryCast(cboTargetSubdivision.SelectedItem, ListItem)
        Dim targetSubdivisionID = CInt(selectedSubdivision.Value)

        ' Check if a price lock already exists for this subdivision/date
        Dim existingLocks = _dataAccess.GetPriceLocksBySubdivision(targetSubdivisionID)
        Dim duplicateLock = existingLocks.FirstOrDefault(Function(pl) pl.PriceLockDate.Date = dtpLockDate.Value.Date)

        If duplicateLock IsNot Nothing Then
            MessageBox.Show($"A price lock already exists for this subdivision on {dtpLockDate.Value:d}.",
                "Duplicate Lock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            dtpLockDate.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub CopyPriceLock()
        Try
            Cursor = Cursors.WaitCursor

            Dim selectedSubdivision = TryCast(cboTargetSubdivision.SelectedItem, ListItem)
            Dim targetSubdivisionID = CInt(selectedSubdivision.Value)

            ' Copy the price lock
            _newPriceLockID = _dataAccess.CopyPriceLock(
                _sourcePriceLock.PriceLockID,
                targetSubdivisionID,
                dtpLockDate.Value.Date,
                Environment.UserName)

            ' Update the name
            Dim newLock = _dataAccess.GetPriceLockByID(_newPriceLockID)
            If newLock IsNot Nothing Then
                newLock.PriceLockName = txtLockName.Text.Trim()
                _dataAccess.UpdatePriceLock(newLock)
            End If

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show($"Error copying price lock: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

#End Region

End Class