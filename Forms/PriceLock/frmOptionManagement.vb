' =====================================================
' frmOptionManagement.vb
' Manage Structural Options (Oversized Garage, Outdoor Room, etc.)
' ENHANCED: Added Merge Options functionality
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.Windows.Forms
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Public Class frmOptionManagement
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _options As List(Of PLOption)
    Private _allOptions As List(Of PLOption)

    ' Controls
    Private WithEvents splitContainer As SplitContainer
    Private WithEvents dgvOptions As DataGridView
    Private pnlEdit As Panel
    Private lblDetailsHeader As Label
    Private lblName As Label
    Private txtOptionName As TextBox
    Private lblDescription As Label
    Private txtOptionDescription As TextBox
    Private chkIsActive As CheckBox
    Private WithEvents btnAdd As Button
    Private WithEvents btnUpdate As Button
    Private lblStatus As Label
    Private lblUsageInfo As Label
    Private grpMerge As GroupBox
    Private lblMergeInstructions As Label
    Private lblMergeTarget As Label
    Private WithEvents cboMergeTarget As ComboBox
    Private lblMergeInfo As Label
    Private WithEvents btnMerge As Button
    Private pnlButtons As Panel
    Private WithEvents btnClose As Button

#End Region

#Region "Constructor"

    Public Sub New()
        _dataAccess = New PriceLockDataAccess()
        InitializeFormComponents()
    End Sub

    Public Sub New(dataAccess As PriceLockDataAccess)
        _dataAccess = dataAccess
        InitializeFormComponents()
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
        UpdateMergeControls()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        AddOption()
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        UpdateOption()
    End Sub

    Private Sub btnMerge_Click(sender As Object, e As EventArgs) Handles btnMerge.Click
        MergeSelectedOption()
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

    Private Sub cboMergeTarget_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboMergeTarget.SelectedIndexChanged
        UpdateMergeButtonState()
    End Sub

#End Region

#Region "Grid Setup"

    Private Sub SetupGrid()
        dgvOptions.Columns.Clear()
        dgvOptions.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "OptionID", .DataPropertyName = "OptionID", .Visible = False})
        dgvOptions.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "OptionName", .HeaderText = "Option Name", .DataPropertyName = "OptionName", .Width = 180})
        dgvOptions.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "OptionDescription", .HeaderText = "Description", .DataPropertyName = "OptionDescription", .Width = 150})
        dgvOptions.Columns.Add(New DataGridViewCheckBoxColumn() With {.Name = "IsActive", .HeaderText = "Active", .DataPropertyName = "IsActive", .Width = 50})
        dgvOptions.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "UsageCount",
            .HeaderText = "Usage",
            .Width = 60,
            .ReadOnly = True
        })
    End Sub

#End Region

#Region "Data Operations"

    Private Sub LoadOptions()
        Try
            _options = _dataAccess.GetOptions()
            dgvOptions.DataSource = Nothing
            dgvOptions.DataSource = _options

            For Each row As DataGridViewRow In dgvOptions.Rows
                If row.IsNewRow Then Continue For
                Dim optionID = CInt(row.Cells("OptionID").Value)
                Dim usageCount = _dataAccess.GetOptionUsageCount(optionID)
                row.Cells("UsageCount").Value = usageCount
            Next

            LoadMergeTargets()
        Catch ex As Exception
            MessageBox.Show($"Error loading options: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadMergeTargets()
        cboMergeTarget.Items.Clear()
        cboMergeTarget.Items.Add(New ListItem("(Select target option...)", 0))

        For Each opt In _options.Where(Function(o) o.IsActive)
            cboMergeTarget.Items.Add(New ListItem($"{opt.OptionName} (ID: {opt.OptionID})", opt.OptionID))
        Next

        cboMergeTarget.SelectedIndex = 0
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

        Dim usageCount = _dataAccess.GetOptionUsageCount(opt.OptionID)
        lblUsageInfo.Text = $"Used in {usageCount} component pricing record(s)"
        lblUsageInfo.Visible = True
    End Sub

    Private Sub ClearEditFields()
        txtOptionName.Text = ""
        txtOptionDescription.Text = ""
        chkIsActive.Checked = True
        lblStatus.Text = ""
        lblUsageInfo.Visible = False
    End Sub

    Private Sub AddOption()
        If String.IsNullOrWhiteSpace(txtOptionName.Text) Then
            MessageBox.Show("Please enter an option name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtOptionName.Focus()
            Return
        End If

        Dim newName = txtOptionName.Text.Trim()
        Dim existing = _options.FirstOrDefault(Function(o) o.OptionName.Equals(newName, StringComparison.OrdinalIgnoreCase))
        If existing IsNot Nothing Then
            MessageBox.Show("An option with name '" & newName & "' already exists (ID: " & existing.OptionID.ToString() & ")." & vbCrLf & vbCrLf &
                           "Consider using the Merge feature instead of creating a duplicate.",
                           "Duplicate Name", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Dim opt As New PLOption() With {
                .OptionName = newName,
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

#Region "Merge Operations"

    Private Sub UpdateMergeControls()
        If dgvOptions.SelectedRows.Count = 0 OrElse dgvOptions.SelectedRows(0).Index >= _options.Count Then
            grpMerge.Enabled = False
            Return
        End If

        grpMerge.Enabled = True
        Dim selectedOpt = _options(dgvOptions.SelectedRows(0).Index)

        cboMergeTarget.Items.Clear()
        cboMergeTarget.Items.Add(New ListItem("(Select target option...)", 0))

        For Each opt In _options.Where(Function(o) o.IsActive AndAlso o.OptionID <> selectedOpt.OptionID)
            cboMergeTarget.Items.Add(New ListItem($"{opt.OptionName} (ID: {opt.OptionID})", opt.OptionID))
        Next

        cboMergeTarget.SelectedIndex = 0

        Dim usageCount = _dataAccess.GetOptionUsageCount(selectedOpt.OptionID)
        lblMergeInfo.Text = "This will reassign " & usageCount.ToString() & " component pricing record(s) to the target option and deactivate '" & selectedOpt.OptionName & "'."
    End Sub

    Private Sub UpdateMergeButtonState()
        Dim hasValidTarget = cboMergeTarget.SelectedItem IsNot Nothing AndAlso
                            TypeOf cboMergeTarget.SelectedItem Is ListItem AndAlso
                            CInt(DirectCast(cboMergeTarget.SelectedItem, ListItem).Value) > 0

        btnMerge.Enabled = hasValidTarget AndAlso dgvOptions.SelectedRows.Count > 0
    End Sub

    Private Sub MergeSelectedOption()
        If dgvOptions.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select an option to merge.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim selectedItem = TryCast(cboMergeTarget.SelectedItem, ListItem)
        If selectedItem Is Nothing OrElse CInt(selectedItem.Value) = 0 Then
            MessageBox.Show("Please select a target option.", "No Target", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim sourceOpt = _options(dgvOptions.SelectedRows(0).Index)
        Dim targetOptionID = CInt(selectedItem.Value)
        Dim targetOpt = _options.FirstOrDefault(Function(o) o.OptionID = targetOptionID)

        If targetOpt Is Nothing Then
            MessageBox.Show("Target option not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        Dim usageCount = _dataAccess.GetOptionUsageCount(sourceOpt.OptionID)

        Dim msg = "Are you sure you want to merge:" & vbCrLf & vbCrLf &
                  "  SOURCE (will be deactivated):" & vbCrLf &
                  "    '" & sourceOpt.OptionName & "' (ID: " & sourceOpt.OptionID.ToString() & ")" & vbCrLf & vbCrLf &
                  "  INTO TARGET (will be kept):" & vbCrLf &
                  "    '" & targetOpt.OptionName & "' (ID: " & targetOpt.OptionID.ToString() & ")" & vbCrLf & vbCrLf &
                  "This will reassign " & usageCount.ToString() & " component pricing record(s)." & vbCrLf & vbCrLf &
                  "This action cannot be easily undone!"

        Dim result = MessageBox.Show(msg, "Confirm Merge", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)

        If result <> DialogResult.Yes Then Return

        Try
            Cursor = Cursors.WaitCursor

            Dim recordsUpdated = _dataAccess.MergeOptions(sourceOpt.OptionID, targetOptionID, Environment.UserName)

            LoadOptions()
            SelectOptionInGrid(targetOptionID)

            lblStatus.Text = "Merged! " & recordsUpdated.ToString() & " record(s) reassigned."
            lblStatus.ForeColor = Color.Green

            MessageBox.Show("Option '" & sourceOpt.OptionName & "' has been merged into '" & targetOpt.OptionName & "'." & vbCrLf &
                           recordsUpdated.ToString() & " component pricing record(s) were reassigned.",
                           "Merge Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show($"Error merging options: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

#End Region

#Region "Form Initialization"

    Private Sub InitializeFormComponents()
        Me.splitContainer = New SplitContainer()
        Me.dgvOptions = New DataGridView()
        Me.pnlEdit = New Panel()
        Me.lblDetailsHeader = New Label()
        Me.lblName = New Label()
        Me.txtOptionName = New TextBox()
        Me.lblDescription = New Label()
        Me.txtOptionDescription = New TextBox()
        Me.chkIsActive = New CheckBox()
        Me.btnAdd = New Button()
        Me.btnUpdate = New Button()
        Me.lblStatus = New Label()
        Me.lblUsageInfo = New Label()
        Me.grpMerge = New GroupBox()
        Me.lblMergeInstructions = New Label()
        Me.lblMergeTarget = New Label()
        Me.cboMergeTarget = New ComboBox()
        Me.lblMergeInfo = New Label()
        Me.btnMerge = New Button()
        Me.pnlButtons = New Panel()
        Me.btnClose = New Button()

        CType(Me.splitContainer, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitContainer.Panel1.SuspendLayout()
        Me.splitContainer.Panel2.SuspendLayout()
        Me.splitContainer.SuspendLayout()
        CType(Me.dgvOptions, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlEdit.SuspendLayout()
        Me.grpMerge.SuspendLayout()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()

        ' splitContainer
        Me.splitContainer.Dock = DockStyle.Fill
        Me.splitContainer.Location = New Point(0, 0)
        Me.splitContainer.Name = "splitContainer"
        Me.splitContainer.Panel1.Controls.Add(Me.dgvOptions)
        Me.splitContainer.Panel2.Controls.Add(Me.pnlEdit)
        Me.splitContainer.Size = New Size(884, 461)
        Me.splitContainer.SplitterDistance = 450
        Me.splitContainer.TabIndex = 0

        ' dgvOptions
        Me.dgvOptions.AllowUserToAddRows = False
        Me.dgvOptions.AllowUserToDeleteRows = False
        Me.dgvOptions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvOptions.BackgroundColor = SystemColors.Window
        Me.dgvOptions.Dock = DockStyle.Fill
        Me.dgvOptions.Location = New Point(0, 0)
        Me.dgvOptions.MultiSelect = False
        Me.dgvOptions.Name = "dgvOptions"
        Me.dgvOptions.ReadOnly = True
        Me.dgvOptions.RowHeadersVisible = False
        Me.dgvOptions.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.dgvOptions.Size = New Size(450, 461)
        Me.dgvOptions.TabIndex = 0

        ' pnlEdit
        Me.pnlEdit.Controls.Add(Me.lblDetailsHeader)
        Me.pnlEdit.Controls.Add(Me.lblName)
        Me.pnlEdit.Controls.Add(Me.txtOptionName)
        Me.pnlEdit.Controls.Add(Me.lblDescription)
        Me.pnlEdit.Controls.Add(Me.txtOptionDescription)
        Me.pnlEdit.Controls.Add(Me.chkIsActive)
        Me.pnlEdit.Controls.Add(Me.btnAdd)
        Me.pnlEdit.Controls.Add(Me.btnUpdate)
        Me.pnlEdit.Controls.Add(Me.lblStatus)
        Me.pnlEdit.Controls.Add(Me.lblUsageInfo)
        Me.pnlEdit.Controls.Add(Me.grpMerge)
        Me.pnlEdit.Dock = DockStyle.Fill
        Me.pnlEdit.Location = New Point(0, 0)
        Me.pnlEdit.Name = "pnlEdit"
        Me.pnlEdit.Padding = New Padding(10)
        Me.pnlEdit.Size = New Size(430, 461)
        Me.pnlEdit.TabIndex = 0

        ' lblDetailsHeader
        Me.lblDetailsHeader.AutoSize = True
        Me.lblDetailsHeader.Font = New Font("Microsoft Sans Serif", 8.25!, FontStyle.Bold)
        Me.lblDetailsHeader.Location = New Point(10, 10)
        Me.lblDetailsHeader.Name = "lblDetailsHeader"
        Me.lblDetailsHeader.Size = New Size(87, 13)
        Me.lblDetailsHeader.Text = "Option Details"

        ' lblName
        Me.lblName.AutoSize = True
        Me.lblName.Location = New Point(10, 38)
        Me.lblName.Name = "lblName"
        Me.lblName.Text = "Name:"

        ' txtOptionName
        Me.txtOptionName.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.txtOptionName.Location = New Point(90, 35)
        Me.txtOptionName.Name = "txtOptionName"
        Me.txtOptionName.Size = New Size(320, 20)
        Me.txtOptionName.TabIndex = 2

        ' lblDescription
        Me.lblDescription.AutoSize = True
        Me.lblDescription.Location = New Point(10, 68)
        Me.lblDescription.Name = "lblDescription"
        Me.lblDescription.Text = "Description:"

        ' txtOptionDescription
        Me.txtOptionDescription.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.txtOptionDescription.Location = New Point(90, 65)
        Me.txtOptionDescription.Multiline = True
        Me.txtOptionDescription.Name = "txtOptionDescription"
        Me.txtOptionDescription.Size = New Size(320, 50)
        Me.txtOptionDescription.TabIndex = 4

        ' chkIsActive
        Me.chkIsActive.AutoSize = True
        Me.chkIsActive.Checked = True
        Me.chkIsActive.CheckState = CheckState.Checked
        Me.chkIsActive.Location = New Point(90, 125)
        Me.chkIsActive.Name = "chkIsActive"
        Me.chkIsActive.Size = New Size(56, 17)
        Me.chkIsActive.Text = "Active"

        ' btnAdd
        Me.btnAdd.Location = New Point(90, 155)
        Me.btnAdd.Name = "btnAdd"
        Me.btnAdd.Size = New Size(90, 25)
        Me.btnAdd.TabIndex = 6
        Me.btnAdd.Text = "Add New"

        ' btnUpdate
        Me.btnUpdate.Location = New Point(190, 155)
        Me.btnUpdate.Name = "btnUpdate"
        Me.btnUpdate.Size = New Size(90, 25)
        Me.btnUpdate.TabIndex = 7
        Me.btnUpdate.Text = "Update"

        ' lblStatus
        Me.lblStatus.AutoSize = True
        Me.lblStatus.ForeColor = Color.Green
        Me.lblStatus.Location = New Point(90, 190)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New Size(0, 13)

        ' lblUsageInfo
        Me.lblUsageInfo.AutoSize = True
        Me.lblUsageInfo.ForeColor = Color.Gray
        Me.lblUsageInfo.Location = New Point(90, 210)
        Me.lblUsageInfo.Name = "lblUsageInfo"
        Me.lblUsageInfo.Size = New Size(0, 13)
        Me.lblUsageInfo.Visible = False

        ' grpMerge
        Me.grpMerge.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.grpMerge.Controls.Add(Me.lblMergeInstructions)
        Me.grpMerge.Controls.Add(Me.lblMergeTarget)
        Me.grpMerge.Controls.Add(Me.cboMergeTarget)
        Me.grpMerge.Controls.Add(Me.lblMergeInfo)
        Me.grpMerge.Controls.Add(Me.btnMerge)
        Me.grpMerge.Enabled = False
        Me.grpMerge.Location = New Point(10, 240)
        Me.grpMerge.Name = "grpMerge"
        Me.grpMerge.Size = New Size(400, 200)
        Me.grpMerge.TabStop = False
        Me.grpMerge.Text = "Merge Duplicate Options"

        ' lblMergeInstructions
        Me.lblMergeInstructions.Location = New Point(10, 25)
        Me.lblMergeInstructions.Name = "lblMergeInstructions"
        Me.lblMergeInstructions.Size = New Size(380, 35)
        Me.lblMergeInstructions.Text = "Select an option from the grid, then choose a target option to merge it into. The selected option will be deactivated."

        ' lblMergeTarget
        Me.lblMergeTarget.AutoSize = True
        Me.lblMergeTarget.Location = New Point(10, 70)
        Me.lblMergeTarget.Name = "lblMergeTarget"
        Me.lblMergeTarget.Text = "Merge selected option INTO:"

        ' cboMergeTarget
        Me.cboMergeTarget.Anchor = AnchorStyles.Top Or AnchorStyles.Left Or AnchorStyles.Right
        Me.cboMergeTarget.DropDownStyle = ComboBoxStyle.DropDownList
        Me.cboMergeTarget.Location = New Point(10, 90)
        Me.cboMergeTarget.Name = "cboMergeTarget"
        Me.cboMergeTarget.Size = New Size(380, 21)

        ' lblMergeInfo
        Me.lblMergeInfo.ForeColor = Color.DarkBlue
        Me.lblMergeInfo.Location = New Point(10, 120)
        Me.lblMergeInfo.Name = "lblMergeInfo"
        Me.lblMergeInfo.Size = New Size(380, 35)

        ' btnMerge
        Me.btnMerge.Enabled = False
        Me.btnMerge.Location = New Point(10, 160)
        Me.btnMerge.Name = "btnMerge"
        Me.btnMerge.Size = New Size(120, 28)
        Me.btnMerge.Text = "Merge Options"

        ' pnlButtons
        Me.pnlButtons.Controls.Add(Me.btnClose)
        Me.pnlButtons.Dock = DockStyle.Bottom
        Me.pnlButtons.Location = New Point(0, 461)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New Size(884, 50)

        ' btnClose
        Me.btnClose.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnClose.Location = New Point(792, 12)
        Me.btnClose.Name = "btnClose"
        Me.btnClose.Size = New Size(80, 25)
        Me.btnClose.Text = "Close"

        ' frmOptionManagement
        Me.ClientSize = New Size(884, 511)
        Me.Controls.Add(Me.splitContainer)
        Me.Controls.Add(Me.pnlButtons)
        Me.MinimumSize = New Size(700, 450)
        Me.Name = "frmOptionManagement"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Option Management"

        Me.splitContainer.Panel1.ResumeLayout(False)
        Me.splitContainer.Panel2.ResumeLayout(False)
        CType(Me.splitContainer, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitContainer.ResumeLayout(False)
        CType(Me.dgvOptions, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlEdit.ResumeLayout(False)
        Me.pnlEdit.PerformLayout()
        Me.grpMerge.ResumeLayout(False)
        Me.grpMerge.PerformLayout()
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)
    End Sub

#End Region

#Region "Helper Classes"

    Private Class ListItem
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

#End Region

End Class