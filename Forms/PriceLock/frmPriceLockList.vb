' =====================================================
' frmPriceLockList.vb
' Main Price Lock listing form with filtering
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Partial Public Class frmPriceLockList
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _priceLocks As List(Of PLPriceLock)

#End Region

#Region "Constructor"

    Public Sub New()
        InitializeComponent()
        _dataAccess = New PriceLockDataAccess()
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmPriceLockList_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            SetupGridColumns()
            LoadFilterDropdowns()
            LoadPriceLocks()
            UpdateButtonStates()
        Catch ex As Exception
            MessageBox.Show($"Error loading form: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub dgvPriceLocks_SelectionChanged(sender As Object, e As EventArgs) Handles dgvPriceLocks.SelectionChanged
        UpdateButtonStates()
    End Sub

    Private Sub dgvPriceLocks_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvPriceLocks.CellDoubleClick
        If e.RowIndex >= 0 Then
            EditSelectedPriceLock()
        End If
    End Sub

    Private Sub dgvPriceLocks_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvPriceLocks.CellFormatting
        ' Color code status column
        If dgvPriceLocks.Columns(e.ColumnIndex).Name = "Status" AndAlso e.Value IsNot Nothing Then
            Select Case e.Value.ToString()
                Case "Draft"
                    e.CellStyle.ForeColor = Color.Gray
                Case "Pending"
                    e.CellStyle.ForeColor = Color.DarkOrange
                Case "Approved"
                    e.CellStyle.ForeColor = Color.Blue
                Case "Sent"
                    e.CellStyle.ForeColor = Color.Green
            End Select
        End If
    End Sub

#End Region

#Region "Filter Events"

    Private Sub cboBuilder_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboBuilder.SelectedIndexChanged
        LoadSubdivisionFilter()
    End Sub

    Private Sub chkDateFilter_CheckedChanged(sender As Object, e As EventArgs) Handles chkDateFilter.CheckedChanged
        dtpStartDate.Enabled = chkDateFilter.Checked
        dtpEndDate.Enabled = chkDateFilter.Checked
    End Sub

    Private Sub btnSearch_Click(sender As Object, e As EventArgs) Handles btnSearch.Click
        LoadPriceLocks()
    End Sub

    Private Sub btnClearFilters_Click(sender As Object, e As EventArgs) Handles btnClearFilters.Click
        cboBuilder.SelectedIndex = 0
        cboSubdivision.SelectedIndex = 0
        cboStatus.SelectedIndex = 0
        chkDateFilter.Checked = False
        dtpStartDate.Value = DateTime.Today.AddMonths(-3)
        dtpEndDate.Value = DateTime.Today
        LoadPriceLocks()
    End Sub

#End Region

#Region "Button Events"

    Private Sub btnNewPriceLock_Click(sender As Object, e As EventArgs) Handles btnNewPriceLock.Click
        CreateNewPriceLock()
    End Sub

    Private Sub btnEditPriceLock_Click(sender As Object, e As EventArgs) Handles btnEditPriceLock.Click
        EditSelectedPriceLock()
    End Sub

    Private Sub btnCopyPriceLock_Click(sender As Object, e As EventArgs) Handles btnCopyPriceLock.Click
        CopySelectedPriceLock()
    End Sub

    Private Sub btnDeletePriceLock_Click(sender As Object, e As EventArgs) Handles btnDeletePriceLock.Click
        DeleteSelectedPriceLock()
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadPriceLocks()
    End Sub

    Private Sub btnAdmin_Click(sender As Object, e As EventArgs) Handles btnAdmin.Click
        OpenAdminSetup()
    End Sub

#End Region

#Region "Grid Setup"

    Private Sub SetupGridColumns()
        dgvPriceLocks.Columns.Clear()

        ' CRITICAL: Disable auto-generation of columns
        dgvPriceLocks.AutoGenerateColumns = False

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "PriceLockID",
        .HeaderText = "ID",
        .DataPropertyName = "PriceLockID",
        .Width = 50,
        .Visible = False
    })

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "PriceLockDate",
        .HeaderText = "Lock Date",
        .DataPropertyName = "PriceLockDate",
        .Width = 100,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "d"}
    })
        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "PriceLockName",
        .HeaderText = "Lock Name",
        .DataPropertyName = "PriceLockName",
        .Width = 150
    })
        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "BuilderID",
        .HeaderText = "Builder ID",
        .DataPropertyName = "BuilderID",
        .Visible = False
    })

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "BuilderName",
        .HeaderText = "Builder",
        .DataPropertyName = "BuilderName",
        .Width = 120
    })



        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "SubdivisionName",
        .HeaderText = "Subdivision",
        .DataPropertyName = "SubdivisionName",
        .Width = 180
    })

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "SubdivisionCode",
        .HeaderText = "Code",
        .DataPropertyName = "SubdivisionCode",
        .Width = 80
    })

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "SubdivisionId",
        .HeaderText = "Subdivision ID",
        .DataPropertyName = "SubdivisionId",
        .Visible = False
    })

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "BaseMgmtMargin",
        .HeaderText = "Base Margin",
        .DataPropertyName = "BaseMgmtMargin",
        .Width = 100,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "P2"},
        .Visible = False
    })

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "AdjustedMarginBaseModels",
        .HeaderText = "Adjusted Margin",
        .DataPropertyName = "AdjustedMarginBaseModels",
        .Width = 100,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "P2"}
    })

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "OptionMargin",
        .HeaderText = "Option Margin",
        .DataPropertyName = "OptionMargin",
        .Width = 100,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "P2"}
    })

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "Status",
        .HeaderText = "Status",
        .DataPropertyName = "Status",
        .Width = 80
    })

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "CreatedBy",
        .HeaderText = "Created By",
        .DataPropertyName = "CreatedBy",
        .Width = 100
    })

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "CreatedDate",
        .HeaderText = "Created",
        .DataPropertyName = "CreatedDate",
        .Width = 100,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "g"}
    })

        dgvPriceLocks.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "SentToBuilderDate",
        .HeaderText = "Sent to Builder",
        .DataPropertyName = "SentToBuilderDate",
        .Width = 100,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "d"}
    })
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadFilterDropdowns()
        ' Load Builders
        cboBuilder.Items.Clear()
        cboBuilder.Items.Add(New ListItem("(All Builders)", 0))
        For Each builder In _dataAccess.GetBuilders()
            cboBuilder.Items.Add(New ListItem(builder.BuilderName, builder.BuilderID))
        Next
        cboBuilder.SelectedIndex = 0

        ' Load Subdivisions (will be filtered when builder changes)
        LoadSubdivisionFilter()

        ' Load Status
        cboStatus.Items.Clear()
        cboStatus.Items.Add(New ListItem("(All Statuses)", ""))
        cboStatus.Items.Add(New ListItem("Draft", PLPriceLockStatus.Draft))
        cboStatus.Items.Add(New ListItem("Pending", PLPriceLockStatus.Pending))
        cboStatus.Items.Add(New ListItem("Approved", PLPriceLockStatus.Approved))
        cboStatus.Items.Add(New ListItem("Sent", PLPriceLockStatus.Sent))
        cboStatus.SelectedIndex = 0

        ' Set default date range
        dtpStartDate.Value = DateTime.Today.AddMonths(-3)
        dtpEndDate.Value = DateTime.Today
    End Sub

    Private Sub LoadSubdivisionFilter()
        cboSubdivision.Items.Clear()
        cboSubdivision.Items.Add(New ListItem("(All Subdivisions)", 0))

        Dim selectedBuilder = TryCast(cboBuilder.SelectedItem, ListItem)
        If selectedBuilder IsNot Nothing AndAlso CInt(selectedBuilder.Value) > 0 Then
            For Each sub_ In _dataAccess.GetSubdivisionsByBuilder(CInt(selectedBuilder.Value))
                cboSubdivision.Items.Add(New ListItem(sub_.DisplayName, sub_.SubdivisionID))
            Next
        Else
            For Each sub_ In _dataAccess.GetAllActiveSubdivisions()
                cboSubdivision.Items.Add(New ListItem($"{sub_.BuilderName} - {sub_.DisplayName}", sub_.SubdivisionID))
            Next
        End If

        cboSubdivision.SelectedIndex = 0
    End Sub

    Private Sub LoadPriceLocks()
        Try
            Cursor = Cursors.WaitCursor

            ' Get filter values
            Dim builderID As Integer? = Nothing
            Dim subdivisionID As Integer? = Nothing
            Dim status As String = Nothing
            Dim startDate As Date? = Nothing
            Dim endDate As Date? = Nothing

            Dim selectedBuilder = TryCast(cboBuilder.SelectedItem, ListItem)
            If selectedBuilder IsNot Nothing AndAlso CInt(selectedBuilder.Value) > 0 Then
                builderID = CInt(selectedBuilder.Value)
            End If

            Dim selectedSubdivision = TryCast(cboSubdivision.SelectedItem, ListItem)
            If selectedSubdivision IsNot Nothing AndAlso CInt(selectedSubdivision.Value) > 0 Then
                subdivisionID = CInt(selectedSubdivision.Value)
            End If

            Dim selectedStatus = TryCast(cboStatus.SelectedItem, ListItem)
            If selectedStatus IsNot Nothing AndAlso Not String.IsNullOrEmpty(CStr(selectedStatus.Value)) Then
                status = CStr(selectedStatus.Value)
            End If

            If chkDateFilter.Checked Then
                startDate = dtpStartDate.Value.Date
                endDate = dtpEndDate.Value.Date
            End If

            ' Load data
            _priceLocks = _dataAccess.GetPriceLocksFiltered(builderID, subdivisionID, status, startDate, endDate)

            ' Bind to grid
            dgvPriceLocks.DataSource = Nothing
            dgvPriceLocks.DataSource = _priceLocks

            ' Update record count
            lblRecordCount.Text = $"{_priceLocks.Count} record(s)"

        Catch ex As Exception
            MessageBox.Show($"Error loading price locks: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

#End Region

#Region "CRUD Operations"

    Private Sub CreateNewPriceLock()
        Using frm As New frmPriceLockNew(_dataAccess)
            If frm.ShowDialog() = DialogResult.OK Then
                ' Open the new price lock for editing
                Dim newLock = _dataAccess.GetPriceLockByID(frm.NewPriceLockID)
                If newLock IsNot Nothing Then
                    Using editFrm As New frmPriceLockDetail(newLock, _dataAccess)
                        editFrm.ShowDialog()
                    End Using
                End If
                LoadPriceLocks()
            End If
        End Using
    End Sub

    Private Sub EditSelectedPriceLock()
        Dim priceLock = GetSelectedPriceLock()
        If priceLock Is Nothing Then Return

        Using frm As New frmPriceLockDetail(priceLock, _dataAccess)
            frm.ShowDialog()
            LoadPriceLocks()
        End Using
    End Sub

    Private Sub CopySelectedPriceLock()
        Dim priceLock = GetSelectedPriceLock()
        If priceLock Is Nothing Then Return

        Using frm As New frmPriceLockCopy(priceLock, _dataAccess)
            If frm.ShowDialog() = DialogResult.OK Then
                ' Open the copied price lock for editing
                Dim copiedLock = _dataAccess.GetPriceLockByID(frm.NewPriceLockID)
                If copiedLock IsNot Nothing Then
                    Using editFrm As New frmPriceLockDetail(copiedLock, _dataAccess)
                        editFrm.ShowDialog()
                    End Using
                End If
                LoadPriceLocks()
            End If
        End Using
    End Sub

    Private Sub DeleteSelectedPriceLock()
        Dim priceLock = GetSelectedPriceLock()
        If priceLock Is Nothing Then Return

        If Not priceLock.CanDelete Then
            MessageBox.Show("Only Draft price locks can be deleted.", "Cannot Delete",
                MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim result = MessageBox.Show(
            $"Are you sure you want to delete the price lock for {priceLock.SubdivisionName} dated {priceLock.PriceLockDate:d}?" &
            vbCrLf & vbCrLf & "This will delete all associated pricing data and cannot be undone.",
            "Confirm Delete",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2)

        If result = DialogResult.Yes Then
            Try
                If _dataAccess.DeletePriceLock(priceLock.PriceLockID) Then
                    LoadPriceLocks()
                Else
                    MessageBox.Show("Could not delete the price lock. It may have already been modified.",
                        "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                End If
            Catch ex As Exception
                MessageBox.Show($"Error deleting price lock: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Function GetSelectedPriceLock() As PLPriceLock
        If dgvPriceLocks.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a price lock.", "No Selection",
                MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return Nothing
        End If

        Dim index = dgvPriceLocks.SelectedRows(0).Index
        If index >= 0 AndAlso index < _priceLocks.Count Then
            Return _priceLocks(index)
        End If

        Return Nothing
    End Function

    Private Sub UpdateButtonStates()
        Dim priceLock = If(dgvPriceLocks.SelectedRows.Count > 0 AndAlso
                           dgvPriceLocks.SelectedRows(0).Index < _priceLocks?.Count,
                           _priceLocks(dgvPriceLocks.SelectedRows(0).Index), Nothing)

        btnEditPriceLock.Enabled = priceLock IsNot Nothing
        btnCopyPriceLock.Enabled = priceLock IsNot Nothing
        btnDeletePriceLock.Enabled = priceLock IsNot Nothing AndAlso priceLock.CanDelete
    End Sub

    Private Sub OpenAdminSetup()
        ' Show a quick menu to choose which admin form to open
        Dim menu As New ContextMenuStrip()

        Dim mnuBuilders = menu.Items.Add("Builders && Subdivisions...")
        AddHandler mnuBuilders.Click, Sub()
                                          Using frm As New frmBuilderManagement(_dataAccess)
                                              frm.ShowDialog()
                                              LoadFilterDropdowns() ' Refresh in case builders/subdivisions changed
                                          End Using
                                      End Sub

        Dim mnuPlans = menu.Items.Add("Plans && Elevations...")
        AddHandler mnuPlans.Click, Sub()
                                       Using frm As New frmPlanManagement(_dataAccess)
                                           frm.ShowDialog()
                                       End Using
                                   End Sub

        Dim mnuOptions = menu.Items.Add("Structural Options...")
        AddHandler mnuOptions.Click, Sub()
                                         Using frm As New frmOptionManagement(_dataAccess)
                                             frm.ShowDialog()
                                         End Using
                                     End Sub

        Dim mnuMaterials = menu.Items.Add("Material Categories...")
        AddHandler mnuMaterials.Click, Sub()
                                           Using frm As New frmMaterialCategoryManagement(_dataAccess)
                                               frm.ShowDialog()
                                           End Using
                                       End Sub

        ' Show menu below the button
        menu.Show(btnAdmin, New Point(0, btnAdmin.Height))
    End Sub


#End Region

End Class

''' <summary>
''' Helper class for ComboBox items with display text and value
''' </summary>
Public Class ListItem
    Public Property Text As String
    Public Property Value As Object

    Public Sub New(text As String, value As Object)
        Me.Text = text
        Me.Value = value
    End Sub

    Public Overrides Function ToString() As String
        Return Text
    End Function
End Class