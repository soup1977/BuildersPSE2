' =====================================================
' frmPriceLockNew.vb
' Dialog for creating a new Price Lock
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Partial Public Class frmPriceLockNew
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
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

    Public Sub New(dataAccess As PriceLockDataAccess)
        _dataAccess = dataAccess
        InitializeComponent()
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmPriceLockNew_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadBuilders()
        GenerateLockName()
    End Sub

    Private Sub cboBuilder_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBuilder.SelectedIndexChanged
        LoadSubdivisions()
    End Sub

    Private Sub cboSubdivision_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboSubdivision.SelectedIndexChanged
        LoadPreviousMargins()
        GenerateLockName()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If ValidateInput() Then
            CreatePriceLock()
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadBuilders()
        cboBuilder.Items.Clear()
        For Each builder In _dataAccess.GetBuilders()
            cboBuilder.Items.Add(New ListItem(builder.BuilderName, builder.BuilderID))
        Next

        If cboBuilder.Items.Count > 0 Then
            cboBuilder.SelectedIndex = 0
        End If
    End Sub

    Private Sub LoadSubdivisions()
        cboSubdivision.Items.Clear()

        Dim selectedBuilder = TryCast(cboBuilder.SelectedItem, ListItem)
        If selectedBuilder Is Nothing Then Return

        For Each sub_ In _dataAccess.GetSubdivisionsByBuilder(CInt(selectedBuilder.Value))
            cboSubdivision.Items.Add(New ListItem(sub_.DisplayName, sub_.SubdivisionID))
        Next

        If cboSubdivision.Items.Count > 0 Then
            cboSubdivision.SelectedIndex = 0
        End If
    End Sub

    Private Sub LoadPreviousMargins()
        ' Try to load margins from the most recent price lock for this subdivision
        Dim selectedSubdivision = TryCast(cboSubdivision.SelectedItem, ListItem)
        If selectedSubdivision Is Nothing Then Return

        Dim previousLock = _dataAccess.GetLatestPriceLock(CInt(selectedSubdivision.Value))
        If previousLock IsNot Nothing Then
            If previousLock.BaseMgmtMargin.HasValue Then
                nudBaseMgmtMargin.Value = previousLock.BaseMgmtMargin.Value
            End If
            If previousLock.AdjustedMarginBaseModels.HasValue Then
                nudAdjustedMargin.Value = previousLock.AdjustedMarginBaseModels.Value
            End If
            If previousLock.OptionMargin.HasValue Then
                nudOptionMargin.Value = previousLock.OptionMargin.Value
            End If
        End If
    End Sub

    Private Sub GenerateLockName()
        ' Generate a default lock name like "Jan 2026 Pricelock"
        Dim lockDate = dtpLockDate.Value
        txtLockName.Text = $"{lockDate:MMM yyyy} Pricelock"
    End Sub

#End Region

#Region "Validation and Creation"

    Private Function ValidateInput() As Boolean
        If cboBuilder.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a builder.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cboBuilder.Focus()
            Return False
        End If

        If cboSubdivision.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a subdivision.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cboSubdivision.Focus()
            Return False
        End If

        ' Check if a price lock already exists for this subdivision/date
        Dim selectedSubdivision = TryCast(cboSubdivision.SelectedItem, ListItem)
        Dim existingLocks = _dataAccess.GetPriceLocksBySubdivision(CInt(selectedSubdivision.Value))
        Dim duplicateLock = existingLocks.FirstOrDefault(Function(pl) pl.PriceLockDate.Date = dtpLockDate.Value.Date)

        If duplicateLock IsNot Nothing Then
            MessageBox.Show($"A price lock already exists for this subdivision on {dtpLockDate.Value:d}.",
                "Duplicate Lock", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            dtpLockDate.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub CreatePriceLock()
        Try
            Cursor = Cursors.WaitCursor

            Dim selectedSubdivision = TryCast(cboSubdivision.SelectedItem, ListItem)
            Dim subdivisionID = CInt(selectedSubdivision.Value)

            ' Check if we should copy from previous
            Dim previousLock = _dataAccess.GetLatestPriceLock(subdivisionID)

            If chkCopyFromPrevious.Checked AndAlso previousLock IsNot Nothing Then
                ' Use the copy function
                _newPriceLockID = _dataAccess.CopyPriceLock(
                    previousLock.PriceLockID,
                    subdivisionID,
                    dtpLockDate.Value.Date,
                    Environment.UserName)

                ' Update the copied lock with new settings
                Dim newLock = _dataAccess.GetPriceLockByID(_newPriceLockID)
                If newLock IsNot Nothing Then
                    newLock.PriceLockName = txtLockName.Text.Trim()
                    newLock.BaseMgmtMargin = nudBaseMgmtMargin.Value
                    newLock.AdjustedMarginBaseModels = nudAdjustedMargin.Value
                    newLock.OptionMargin = nudOptionMargin.Value
                    _dataAccess.UpdatePriceLock(newLock)
                End If
            Else
                ' Create new empty price lock
                Dim newLock As New PLPriceLock() With {
                    .SubdivisionID = subdivisionID,
                    .PriceLockDate = dtpLockDate.Value.Date,
                    .PriceLockName = txtLockName.Text.Trim(),
                    .BaseMgmtMargin = nudBaseMgmtMargin.Value,
                    .AdjustedMarginBaseModels = nudAdjustedMargin.Value,
                    .OptionMargin = nudOptionMargin.Value,
                    .Status = PLPriceLockStatus.Draft,
                    .CreatedBy = Environment.UserName
                }

                _newPriceLockID = _dataAccess.InsertPriceLock(newLock)
            End If

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show($"Error creating price lock: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

#End Region

End Class