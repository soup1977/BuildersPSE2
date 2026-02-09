' =====================================================
' frmOptionResolver.vb
' Dialog to resolve/confirm option mappings before import
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Linq
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models
Imports BuildersPSE2.Forms.PriceLock

Public Class frmOptionResolver
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _existingOptions As List(Of PLOption)
    Private _csvOptionNames As List(Of String)
    Private _mappings As List(Of OptionMapping)
    Private _currentIndex As Integer = 0

    Private Const SIMILARITY_THRESHOLD As Double = 0.7

#End Region

#Region "Properties"

    ''' <summary>
    ''' Returns the resolved option mappings after user confirmation
    ''' </summary>
    Public ReadOnly Property ResolvedMappings As List(Of OptionMapping)
        Get
            Return _mappings
        End Get
    End Property

#End Region

#Region "Constructor"

    Public Sub New(csvOptionNames As List(Of String), dataAccess As PriceLockDataAccess)
        _dataAccess = dataAccess
        _csvOptionNames = csvOptionNames.Distinct().ToList()
        _mappings = New List(Of OptionMapping)()

        InitializeComponents()
        Me.Text = "Resolve Option Names"
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmOptionResolver_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadExistingOptions()
        AnalyzeOptionsAndBuildMappings()
        SetupGrid()
        DisplayMappings()
    End Sub

    Private Sub btnAcceptAll_Click(sender As Object, e As EventArgs) Handles btnAcceptAll.Click
        ' Mark all mappings as confirmed
        For Each mapping In _mappings
            mapping.UserConfirmed = True
        Next
        Me.DialogResult = DialogResult.OK
        Me.Close()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub dgvMappings_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvMappings.CellClick
        If e.RowIndex < 0 Then Return

        ' Check if clicked on the "Change" button column
        If dgvMappings.Columns(e.ColumnIndex).Name = "colChange" Then
            ShowOptionSelectionDialog(e.RowIndex)
        End If
    End Sub

#End Region

#Region "Initialization"

    Private Sub LoadExistingOptions()
        _existingOptions = _dataAccess.GetOptions()
    End Sub

    Private Sub AnalyzeOptionsAndBuildMappings()
        Dim existingOptionNames = _existingOptions.Select(Function(o) o.OptionName).ToList()

        For Each csvOption In _csvOptionNames
            Dim mapping As New OptionMapping() With {
                .CsvOptionName = csvOption,
                .OccurrenceCount = 1,
                .UserConfirmed = False
            }

            ' Find similar existing options
            Dim matches = FuzzyMatcher.FindSimilarOptions(csvOption, existingOptionNames, SIMILARITY_THRESHOLD)

            If matches.Any(Function(m) m.IsExactMatch) Then
                ' Exact match found - auto-map but still show for review
                Dim exactMatch = matches.First(Function(m) m.IsExactMatch)
                Dim existingOpt = _existingOptions.First(Function(o) o.OptionName.Equals(exactMatch.OriginalName, StringComparison.OrdinalIgnoreCase))
                mapping.ResolvedOptionName = existingOpt.OptionName
                mapping.ResolvedOptionID = existingOpt.OptionID
                mapping.IsNewOption = False
                mapping.UserConfirmed = True
            ElseIf matches.Any() Then
                ' Fuzzy matches found - suggest best match but require confirmation
                Dim bestMatch = matches.First()
                Dim existingOpt = _existingOptions.First(Function(o) o.OptionName.Equals(bestMatch.OriginalName, StringComparison.OrdinalIgnoreCase))
                mapping.ResolvedOptionName = existingOpt.OptionName
                mapping.ResolvedOptionID = existingOpt.OptionID
                mapping.IsNewOption = False
                mapping.UserConfirmed = False
            Else
                ' No match - will create new
                mapping.ResolvedOptionName = csvOption
                mapping.ResolvedOptionID = Nothing
                mapping.IsNewOption = True
                mapping.UserConfirmed = False
            End If

            _mappings.Add(mapping)
        Next
    End Sub

#End Region

#Region "Grid Setup and Display"

    Private Sub SetupGrid()
        dgvMappings.Columns.Clear()
        dgvMappings.AutoGenerateColumns = False

        dgvMappings.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "colCsvName",
            .HeaderText = "Option Name in CSV",
            .Width = 200,
            .ReadOnly = True
        })

        dgvMappings.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "colResolvedName",
            .HeaderText = "Will Import As",
            .Width = 200,
            .ReadOnly = True
        })

        dgvMappings.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "colStatus",
            .HeaderText = "Status",
            .Width = 120,
            .ReadOnly = True
        })

        dgvMappings.Columns.Add(New DataGridViewCheckBoxColumn() With {
            .Name = "colConfirmed",
            .HeaderText = "Confirmed",
            .Width = 80
        })

        dgvMappings.Columns.Add(New DataGridViewButtonColumn() With {
            .Name = "colChange",
            .HeaderText = "",
            .Text = "Change...",
            .UseColumnTextForButtonValue = True,
            .Width = 80
        })
    End Sub

    Private Sub DisplayMappings()
        dgvMappings.Rows.Clear()

        For Each mapping In _mappings
            Dim rowIdx = dgvMappings.Rows.Add()
            Dim row = dgvMappings.Rows(rowIdx)

            row.Cells("colCsvName").Value = mapping.CsvOptionName
            row.Cells("colResolvedName").Value = mapping.ResolvedOptionName
            row.Cells("colConfirmed").Value = mapping.UserConfirmed

            If mapping.IsNewOption Then
                row.Cells("colStatus").Value = "CREATE NEW"
                row.DefaultCellStyle.BackColor = Color.LightYellow
            ElseIf mapping.CsvOptionName.Equals(mapping.ResolvedOptionName, StringComparison.OrdinalIgnoreCase) Then
                row.Cells("colStatus").Value = "Exact Match"
                row.DefaultCellStyle.BackColor = Color.LightGreen
            Else
                row.Cells("colStatus").Value = "Fuzzy Match"
                row.DefaultCellStyle.BackColor = Color.LightCyan
            End If

            row.Tag = mapping
        Next

        UpdateSummary()
    End Sub

    Private Sub UpdateSummary()
        ' Use Where().Count() instead of Count(predicate) for VB.NET compatibility
        Dim exactCount = _mappings.Where(Function(m) Not m.IsNewOption AndAlso m.CsvOptionName.Equals(m.ResolvedOptionName, StringComparison.OrdinalIgnoreCase)).Count()
        Dim fuzzyCount = _mappings.Where(Function(m) Not m.IsNewOption AndAlso Not m.CsvOptionName.Equals(m.ResolvedOptionName, StringComparison.OrdinalIgnoreCase)).Count()
        Dim newCount = _mappings.Where(Function(m) m.IsNewOption).Count()
        Dim confirmedCount = _mappings.Where(Function(m) m.UserConfirmed).Count()

        lblSummary.Text = $"Exact matches: {exactCount}  |  Fuzzy matches: {fuzzyCount}  |  New options: {newCount}  |  Confirmed: {confirmedCount}/{_mappings.Count}"

        ' Enable Accept All only if all are confirmed
        btnAcceptAll.Enabled = _mappings.All(Function(m) m.UserConfirmed)
        If Not btnAcceptAll.Enabled Then
            btnAcceptAll.Text = "Confirm All && Continue"
        Else
            btnAcceptAll.Text = "Continue with Import"
        End If
    End Sub

#End Region

#Region "Option Selection"

    Private Sub ShowOptionSelectionDialog(rowIndex As Integer)
        Dim mapping = DirectCast(dgvMappings.Rows(rowIndex).Tag, OptionMapping)

        Using frm As New frmSelectOption(mapping.CsvOptionName, _existingOptions, mapping.ResolvedOptionID)
            If frm.ShowDialog(Me) = DialogResult.OK Then
                If frm.CreateNew Then
                    mapping.ResolvedOptionName = frm.NewOptionName
                    mapping.ResolvedOptionID = Nothing
                    mapping.IsNewOption = True
                Else
                    mapping.ResolvedOptionName = frm.SelectedOption.OptionName
                    mapping.ResolvedOptionID = frm.SelectedOption.OptionID
                    mapping.IsNewOption = False
                End If
                mapping.UserConfirmed = True

                DisplayMappings()
            End If
        End Using
    End Sub

#End Region

#Region "Form Initialization"

    ''' <summary>
    ''' Initializes form components (renamed to avoid conflict with Designer)
    ''' </summary>
    Private Sub InitializeComponents()
        Me.dgvMappings = New DataGridView()
        Me.lblInstructions = New Label()
        Me.lblSummary = New Label()
        Me.btnAcceptAll = New Button()
        Me.btnCancel = New Button()
        Me.pnlButtons = New Panel()

        CType(Me.dgvMappings, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.pnlButtons.SuspendLayout()
        Me.SuspendLayout()

        ' lblInstructions
        Me.lblInstructions.Dock = DockStyle.Top
        Me.lblInstructions.Location = New Point(0, 0)
        Me.lblInstructions.Name = "lblInstructions"
        Me.lblInstructions.Padding = New Padding(10)
        Me.lblInstructions.Size = New Size(800, 60)
        Me.lblInstructions.Text = "Review the option mappings below. Options from the CSV file will be matched to existing options or created as new." & vbCrLf &
                                   "Click 'Change...' to modify a mapping. Check 'Confirmed' to approve each mapping, or click 'Confirm All' when ready."

        ' dgvMappings
        Me.dgvMappings.AllowUserToAddRows = False
        Me.dgvMappings.AllowUserToDeleteRows = False
        Me.dgvMappings.Anchor = AnchorStyles.Top Or AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.dgvMappings.BackgroundColor = SystemColors.Window
        Me.dgvMappings.Location = New Point(10, 70)
        Me.dgvMappings.Name = "dgvMappings"
        Me.dgvMappings.RowHeadersVisible = False
        Me.dgvMappings.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        Me.dgvMappings.Size = New Size(780, 350)

        ' lblSummary
        Me.lblSummary.Anchor = AnchorStyles.Bottom Or AnchorStyles.Left Or AnchorStyles.Right
        Me.lblSummary.Location = New Point(10, 430)
        Me.lblSummary.Name = "lblSummary"
        Me.lblSummary.Size = New Size(780, 20)

        ' pnlButtons
        Me.pnlButtons.Controls.Add(Me.btnAcceptAll)
        Me.pnlButtons.Controls.Add(Me.btnCancel)
        Me.pnlButtons.Dock = DockStyle.Bottom
        Me.pnlButtons.Location = New Point(0, 460)
        Me.pnlButtons.Name = "pnlButtons"
        Me.pnlButtons.Size = New Size(800, 50)

        ' btnAcceptAll
        Me.btnAcceptAll.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnAcceptAll.Location = New Point(560, 12)
        Me.btnAcceptAll.Name = "btnAcceptAll"
        Me.btnAcceptAll.Size = New Size(130, 28)
        Me.btnAcceptAll.Text = "Confirm All && Continue"

        ' btnCancel
        Me.btnCancel.Anchor = AnchorStyles.Top Or AnchorStyles.Right
        Me.btnCancel.Location = New Point(700, 12)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New Size(80, 28)
        Me.btnCancel.Text = "Cancel"

        ' frmOptionResolver
        Me.ClientSize = New Size(800, 510)
        Me.Controls.Add(Me.dgvMappings)
        Me.Controls.Add(Me.lblInstructions)
        Me.Controls.Add(Me.lblSummary)
        Me.Controls.Add(Me.pnlButtons)
        Me.MinimumSize = New Size(600, 400)
        Me.Name = "frmOptionResolver"
        Me.StartPosition = FormStartPosition.CenterParent
        Me.Text = "Resolve Option Names"

        CType(Me.dgvMappings, System.ComponentModel.ISupportInitialize).EndInit()
        Me.pnlButtons.ResumeLayout(False)
        Me.ResumeLayout(False)
    End Sub

    Private WithEvents dgvMappings As DataGridView
    Private lblInstructions As Label
    Private lblSummary As Label
    Private WithEvents btnAcceptAll As Button
    Private WithEvents btnCancel As Button
    Private pnlButtons As Panel

#End Region

End Class