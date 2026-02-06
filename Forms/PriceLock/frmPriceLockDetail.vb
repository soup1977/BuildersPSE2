' =====================================================
' frmPriceLockDetail.vb
' Main Price Lock editing form with Component and Material tabs
' ENHANCED: Margin propagation and price update prompts
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Linq
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Public Class frmPriceLockDetail

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _priceLock As PLPriceLock
    Private _componentPricing As List(Of PLComponentPricing)
    Private _materialPricing As List(Of PLMaterialPricing)
    Private _isDirty As Boolean = False
    Private _rlImports As List(Of PLRandomLengthsImport)
    ' Track original margin values for change detection
    Private _originalAdjustedMargin As Decimal
    Private _originalOptionMargin As Decimal

#End Region

#Region "Constructor"

    Public Sub New(priceLock As PLPriceLock, dataAccess As PriceLockDataAccess)
        _priceLock = priceLock
        _dataAccess = dataAccess
        InitializeComponent()

        ' Setup grids before loading data
        SetupComponentGrid()
        SetupMaterialGrid()

        ' Populate status combo
        cboStatus.Items.AddRange(New String() {
            PLPriceLockStatus.Draft,
            PLPriceLockStatus.Pending,
            PLPriceLockStatus.Approved,
            PLPriceLockStatus.Sent
        })

        ' Wire up event handlers for value changes
        AddHandler dtpLockDate.ValueChanged, AddressOf Control_ValueChanged
        AddHandler txtLockName.TextChanged, AddressOf Control_ValueChanged
        AddHandler nudBaseMgmtMargin.ValueChanged, AddressOf Control_ValueChanged
        ' ENHANCED: Special handlers for margin changes
        AddHandler nudAdjustedMargin.ValueChanged, AddressOf AdjustedMargin_ValueChanged
        AddHandler nudOptionMargin.ValueChanged, AddressOf OptionMargin_ValueChanged
        ' Wire up RL selection handlers
        AddHandler cboBaselineRL.SelectedIndexChanged, AddressOf RLSelection_Changed
        AddHandler cboCurrentRL.SelectedIndexChanged, AddressOf RLSelection_Changed

    End Sub

#End Region

#Region "Form Events"

    Private Sub frmPriceLockDetail_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadPriceLockData()

        ' Store original margin values
        _originalAdjustedMargin = nudAdjustedMargin.Value
        _originalOptionMargin = nudOptionMargin.Value

        LoadComponentPricing()
        LoadMaterialPricing()
        LoadRandomLengthsImports()
        UpdateControlStates()
        _isDirty = False
    End Sub

    Private Sub frmPriceLockDetail_FormClosing(sender As Object, e As FormClosingEventArgs) Handles MyBase.FormClosing
        If _isDirty Then
            Dim result = MessageBox.Show("You have unsaved changes. Do you want to save before closing?",
                "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                SavePriceLock()
            ElseIf result = DialogResult.Cancel Then
                e.Cancel = True
            End If
        End If
    End Sub

    Private Sub Control_ValueChanged(sender As Object, e As EventArgs)
        _isDirty = True
    End Sub
    ' ENHANCED: Handle adjusted margin changes with propagation
    Private Sub AdjustedMargin_ValueChanged(sender As Object, e As EventArgs)
        _isDirty = True

        ' Check if this is a meaningful change (not just loading)
        If _componentPricing Is Nothing OrElse _componentPricing.Count = 0 Then Return
        If nudAdjustedMargin.Value = _originalAdjustedMargin Then Return

        ' Prompt user to update related records
        Dim affected = Enumerable.Count(_componentPricing, Function(c) Not c.UsesOptionMargin)
        If affected > 0 Then
            Dim msg = $"The Adjusted Margin has changed from {_originalAdjustedMargin:P1} to {nudAdjustedMargin.Value:P1}." & vbCrLf & vbCrLf &
                      $"This affects {affected} component pricing record(s) (base models and non-option elevations)." & vbCrLf & vbCrLf &
                      "Do you want to recalculate prices for these records now?"

            Dim result = MessageBox.Show(msg, "Update Prices?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                UpdatePricesForMarginChange("Adjusted", nudAdjustedMargin.Value)
                _originalAdjustedMargin = nudAdjustedMargin.Value
            End If
        End If
    End Sub

    ' ENHANCED: Handle option margin changes with propagation
    Private Sub OptionMargin_ValueChanged(sender As Object, e As EventArgs)
        _isDirty = True

        ' Check if this is a meaningful change (not just loading)
        If _componentPricing Is Nothing OrElse _componentPricing.Count = 0 Then Return
        If nudOptionMargin.Value = _originalOptionMargin Then Return

        ' Prompt user to update related records
        Dim affected = Enumerable.Count(_componentPricing, Function(c) c.UsesOptionMargin)
        If affected > 0 Then
            Dim msg = $"The Option Margin has changed from {_originalOptionMargin:P1} to {nudOptionMargin.Value:P1}." & vbCrLf & vbCrLf &
                      $"This affects {affected} component pricing record(s) (options and option-based elevations)." & vbCrLf & vbCrLf &
                      "Do you want to recalculate prices for these records now?"

            Dim result = MessageBox.Show(msg, "Update Prices?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                UpdatePricesForMarginChange("Option", nudOptionMargin.Value)
                _originalOptionMargin = nudOptionMargin.Value
            End If
        End If
    End Sub

#End Region

#Region "Grid Setup"

    Private Sub SetupComponentGrid()
        dgvComponents.AutoGenerateColumns = False
        dgvComponents.Columns.Clear()

        ' Add columns explicitly
        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "ComponentPricingID",
            .HeaderText = "ID",
            .DataPropertyName = "ComponentPricingID",
            .Visible = False
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PlanName",
            .HeaderText = "Plan",
            .DataPropertyName = "PlanName",
            .Width = 100,
            .ReadOnly = True
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "ElevationName",
            .HeaderText = "Elevation",
            .DataPropertyName = "ElevationName",
            .Width = 100,
            .ReadOnly = True
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "OptionName",
            .HeaderText = "Option",
            .DataPropertyName = "OptionName",
            .Width = 150,
            .ReadOnly = True
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "ProductTypeName",
            .HeaderText = "Type",
            .DataPropertyName = "ProductTypeName",
            .Width = 80,
            .ReadOnly = True
        })

        ' ENHANCED: Add Margin Source column
        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "MarginName",
            .HeaderText = "Margin",
            .DataPropertyName = "MarginName",
            .Width = 75,
            .ReadOnly = True
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "Cost",
            .HeaderText = "Cost",
            .DataPropertyName = "Cost",
            .Width = 100,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {
                .Format = "C2",
                .Alignment = DataGridViewContentAlignment.MiddleRight
            }
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "MgmtSellPrice",
            .HeaderText = "Mgmt Sell",
            .DataPropertyName = "MgmtSellPrice",
            .Width = 100,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {
                .Format = "C2",
                .Alignment = DataGridViewContentAlignment.MiddleRight
            }
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "FinalPrice",
            .HeaderText = "Final Price",
            .DataPropertyName = "FinalPrice",
            .Width = 100,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {
                .Format = "C2",
                .Alignment = DataGridViewContentAlignment.MiddleRight
            }
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PriceSentToBuilder",
            .HeaderText = "Builder Price",
            .DataPropertyName = "PriceSentToBuilder",
            .Width = 100,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {
                .Format = "C2",
                .Alignment = DataGridViewContentAlignment.MiddleRight
            }
        })
        ' Add these columns after the "PriceSentToBuilder" column (after line 169)
        ' Change the margin columns to be data-bound:
        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
    .Name = "FinalMargin",
    .HeaderText = "Final Margin%",
    .DataPropertyName = "FinalMarginPct",
    .Width = 100,
    .ReadOnly = True,
    .DefaultCellStyle = New DataGridViewCellStyle() With {
        .Format = "P2",
        .Alignment = DataGridViewContentAlignment.MiddleRight
    }
})

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
    .Name = "BuilderMargin",
    .HeaderText = "Builder Margin%",
    .DataPropertyName = "BuilderMarginPct",
    .Width = 100,
    .ReadOnly = True,
    .DefaultCellStyle = New DataGridViewCellStyle() With {
        .Format = "P2",
        .Alignment = DataGridViewContentAlignment.MiddleRight
    }
})
        ' Use a TextBox column for IsAdder (will be formatted in CellFormatting)
        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "IsAdder",
            .HeaderText = "Adder",
            .DataPropertyName = "IsAdder",
            .Width = 60,
            .ReadOnly = True
        })

        ' Warning icon column
        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "HasDiff",
            .HeaderText = "⚠",
            .Width = 30,
            .ReadOnly = True
        })

        dgvComponents.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PriceNote",
            .HeaderText = "Notes",
            .DataPropertyName = "PriceNote",
            .Width = 150,
            .ReadOnly = True
        })

        dgvComponents.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvComponents.MultiSelect = False
        dgvComponents.AllowUserToAddRows = False
        dgvComponents.AllowUserToDeleteRows = False

        ' Remove the selection highlight color so conditional formatting shows through
        dgvComponents.DefaultCellStyle.SelectionBackColor = dgvComponents.DefaultCellStyle.BackColor
        dgvComponents.DefaultCellStyle.SelectionForeColor = dgvComponents.DefaultCellStyle.ForeColor
    End Sub

    Private Sub SetupMaterialGrid()
        dgvMaterials.AutoGenerateColumns = False
        dgvMaterials.Columns.Clear()

        ' Add columns explicitly
        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "MaterialPricingID",
            .HeaderText = "ID",
            .DataPropertyName = "MaterialPricingID",
            .Visible = False
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "CategoryName",
            .HeaderText = "Category",
            .DataPropertyName = "CategoryName",
            .Width = 150,
            .ReadOnly = True
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "CategoryCode",
            .HeaderText = "Code",
            .DataPropertyName = "CategoryCode",
            .Width = 80,
            .ReadOnly = True
        })
        ' Add after line 264 (after CategoryCode column)
        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
    .Name = "PreviousApprovedPrice",
    .HeaderText = "Prev Approved",
    .Width = 100,
    .ReadOnly = True,
    .DefaultCellStyle = New DataGridViewCellStyle() With {
        .Format = "C2",
        .Alignment = DataGridViewContentAlignment.MiddleRight,
        .ForeColor = Color.Gray  ' Gray text to show it's reference data
    }
})

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "RandomLengthsPrice",
            .HeaderText = "RL Price",
            .DataPropertyName = "RandomLengthsPrice",
            .Width = 100,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {
                .Format = "C2",
                .Alignment = DataGridViewContentAlignment.MiddleRight
            }
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "CalculatedPrice",
            .HeaderText = "Calc Price",
            .DataPropertyName = "CalculatedPrice",
            .Width = 100,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {
                .Format = "C2",
                .Alignment = DataGridViewContentAlignment.MiddleRight
            }
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PriceSentToBuilder",
            .HeaderText = "Builder Price",
            .DataPropertyName = "PriceSentToBuilder",
            .Width = 100,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {
                .Format = "C2",
                .Alignment = DataGridViewContentAlignment.MiddleRight
            }
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PctChangeFromPrevious",
            .HeaderText = "% Change",
            .DataPropertyName = "PctChangeFromPrevious",
            .Width = 80,
            .ReadOnly = True,
            .DefaultCellStyle = New DataGridViewCellStyle() With {
                .Format = "P1",
                .Alignment = DataGridViewContentAlignment.MiddleRight
            }
        })

        ' Warning icon column
        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "HasDiffMat",
            .HeaderText = "⚠",
            .Width = 30,
            .ReadOnly = True
        })

        dgvMaterials.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "PriceNote",
            .HeaderText = "Notes",
            .DataPropertyName = "PriceNote",
            .Width = 150,
            .ReadOnly = True
        })

        dgvMaterials.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvMaterials.MultiSelect = False
        dgvMaterials.AllowUserToAddRows = False
        dgvMaterials.AllowUserToDeleteRows = False
    End Sub

#End Region

#Region "Button Events"

    Private Sub btnSave_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        SavePriceLock()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.Close()
    End Sub

    Private Sub btnChangeStatus_Click(sender As Object, e As EventArgs) Handles btnChangeStatus.Click
        ChangeStatus()
    End Sub

    Private Sub btnAddComponent_Click(sender As Object, e As EventArgs) Handles btnAddComponent.Click
        AddComponent()
    End Sub

    Private Sub btnEditComponent_Click(sender As Object, e As EventArgs) Handles btnEditComponent.Click
        EditSelectedComponent()
    End Sub

    Private Sub btnDeleteComponent_Click(sender As Object, e As EventArgs) Handles btnDeleteComponent.Click
        DeleteSelectedComponent()
    End Sub

    Private Sub btnImportMitek_Click(sender As Object, e As EventArgs) Handles btnImportMitek.Click
        ImportMitekCsv()
    End Sub

    Private Sub btnEditMaterial_Click(sender As Object, e As EventArgs) Handles btnEditMaterial.Click
        EditSelectedMaterial()
    End Sub

    ' ADD this button event handler in the "Button Events" region (around line 433)

    Private Sub btnAddMaterial_Click(sender As Object, e As EventArgs) Handles btnAddMaterial.Click
        AddMaterial()
    End Sub


    Private Sub btnManageRLImports_Click(sender As Object, e As EventArgs) Handles btnManageRLImports.Click
        OpenRLImportManager()
    End Sub

    Private Sub btnCalculateFromRL_Click(sender As Object, e As EventArgs) Handles btnCalculateFromRL.Click
        CalculateMaterialPricingFromRL()
    End Sub

#End Region

#Region "Grid Events"

    Private Sub dgvComponents_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvComponents.CellDoubleClick
        If e.RowIndex >= 0 Then
            EditSelectedComponent()
        End If
    End Sub

    Private Sub dgvComponents_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvComponents.CellFormatting
        If e.RowIndex < 0 OrElse e.RowIndex >= _componentPricing.Count Then Return

        Dim comp = _componentPricing(e.RowIndex)
        Dim columnName = dgvComponents.Columns(e.ColumnIndex).Name

        ' Show warning icon if prices differ
        If columnName = "HasDiff" Then
            e.Value = If(comp.HasPriceDifference, "⚠", "")
            e.FormattingApplied = True
        End If

        ' Format IsAdder column as Yes/No instead of True/False
        If columnName = "IsAdder" AndAlso e.Value IsNot Nothing Then
            If TypeOf e.Value Is Boolean Then
                e.Value = If(CBool(e.Value), "Yes", "No")
                e.FormattingApplied = True
            End If
        End If

        ' ENHANCED: Color-code margin source
        If columnName = "MarginName" Then
            If comp.UsesOptionMargin Then
                e.CellStyle.ForeColor = Color.DarkBlue
                e.CellStyle.Font = New Font(e.CellStyle.Font, FontStyle.Bold)
            End If
        End If

        ' NEW: Color-code Final Margin when it doesn't match expected price lock margin
        If columnName = "FinalMargin" Then
            ' Determine the expected margin based on whether this uses Option margin or Adjusted margin
            Dim expectedMargin As Decimal = If(comp.UsesOptionMargin,
            _priceLock.OptionMargin.GetValueOrDefault(0D),
            _priceLock.AdjustedMarginBaseModels.GetValueOrDefault(0D))

            ' Calculate the difference between actual and expected margin
            Dim marginDiff As Decimal = Math.Abs(comp.FinalMarginPct - expectedMargin)

            ' If Final Margin differs from expected margin by more than 0.5% (0.005), highlight in red
            If marginDiff > 0.01D Then
                e.CellStyle.BackColor = Color.LightCoral
                e.CellStyle.ForeColor = Color.DarkRed
                e.CellStyle.Font = New Font(e.CellStyle.Font, FontStyle.Bold)
            End If
        End If

        ' ENHANCED: Highlight adder rows in the grid for easy identification
        If comp.IsAdder Then
            ' Light blue tint for adder rows
            If Not comp.HasPriceDifference Then
                dgvComponents.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.FromArgb(245, 250, 255)
            End If
        End If

        ' Color-code Builder Margin when it differs from Final Margin
        If columnName = "BuilderMargin" Then
            Dim marginDiff As Decimal = Math.Abs(comp.FinalMarginPct - comp.BuilderMarginPct)

            ' If Builder Margin differs from Final Margin by more than 0.5% (0.005), highlight it
            If marginDiff > 0.005D Then
                e.CellStyle.BackColor = Color.LightCoral
                e.CellStyle.ForeColor = Color.DarkRed
                e.CellStyle.Font = New Font(e.CellStyle.Font, FontStyle.Bold)
            End If
        End If

        ' Highlight rows with price differences (takes precedence over adder color)
        If comp.HasPriceDifference Then
            dgvComponents.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.LightYellow
        End If
    End Sub

    Private Sub dgvMaterials_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvMaterials.CellDoubleClick
        If e.RowIndex >= 0 Then
            EditSelectedMaterial()
        End If
    End Sub

    Private Sub dgvMaterials_CellFormatting(sender As Object, e As DataGridViewCellFormattingEventArgs) Handles dgvMaterials.CellFormatting
        If e.RowIndex < 0 OrElse e.RowIndex >= _materialPricing.Count Then Return

        Dim mat = _materialPricing(e.RowIndex)

        ' Show warning icon if prices differ
        If dgvMaterials.Columns(e.ColumnIndex).Name = "HasDiffMat" Then
            e.Value = If(mat.HasPriceDifference, "⚠", "")
            e.FormattingApplied = True
        End If

        ' Highlight rows with price differences
        If mat.HasPriceDifference Then
            dgvMaterials.Rows(e.RowIndex).DefaultCellStyle.BackColor = Color.LightYellow
        End If

        ' Color code percent change
        If dgvMaterials.Columns(e.ColumnIndex).Name = "PctChangeFromPrevious" AndAlso mat.PctChangeFromPrevious.HasValue Then
            If mat.PctChangeFromPrevious.Value > 0.05D Then
                e.CellStyle.ForeColor = Color.Red
            ElseIf mat.PctChangeFromPrevious.Value < -0.05D Then
                e.CellStyle.ForeColor = Color.Green
            End If
        End If
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadPriceLockData()
        lblBuilderValue.Text = _priceLock.BuilderName
        lblSubdivisionValue.Text = _priceLock.SubdivisionName
        lblCodeValue.Text = If(_priceLock.SubdivisionCode, "--")
        dtpLockDate.Value = _priceLock.PriceLockDate
        txtLockName.Text = _priceLock.PriceLockName
        cboStatus.Text = _priceLock.Status
        nudBaseMgmtMargin.Value = If(_priceLock.BaseMgmtMargin, CDec(0.15))
        nudAdjustedMargin.Value = If(_priceLock.AdjustedMarginBaseModels, CDec(0.15))
        nudOptionMargin.Value = If(_priceLock.OptionMargin, CDec(0.3))
    End Sub

    ' Update the LoadComponentPricing method
    Private Sub LoadComponentPricing()
        _componentPricing = _dataAccess.GetComponentPricingByLock(_priceLock.PriceLockID)

        ' Calculate margin columns for each row
        For Each comp In _componentPricing
            Dim cost As Decimal = If(comp.Cost, 0D)
            Dim finalPrice As Decimal = If(comp.FinalPrice, 0D)
            Dim builderPrice As Decimal = If(comp.PriceSentToBuilder, 0D)

            If comp.IsAdder AndAlso comp.BaseComponentCost.HasValue AndAlso comp.AdderCost.HasValue Then
                ' FOR ADDERS: Calculate margin using incremental cost formula
                ' Margin = (AdderPrice - (AdderMgmtCost - BaseRefMgmtCost)) / AdderPrice

                Dim incrementalCost As Decimal = comp.AdderCost.Value

                ' Calculate FinalMargin% for adder
                ' AdderPrice is stored in FinalPrice (the incremental price charged)
                If finalPrice <> 0 Then
                    comp.FinalMarginPct = (finalPrice - incrementalCost) / finalPrice
                Else
                    comp.FinalMarginPct = 0D
                End If

                ' Calculate BuilderMargin% for adder
                ' AdderPrice is stored in PriceSentToBuilder
                If builderPrice <> 0 Then
                    comp.BuilderMarginPct = (builderPrice - incrementalCost) / builderPrice
                Else
                    comp.BuilderMarginPct = 0D
                End If
            Else
                ' FOR NON-ADDERS: Standard margin calculation
                If finalPrice > 0 Then
                    comp.FinalMarginPct = (finalPrice - cost) / finalPrice
                Else
                    comp.FinalMarginPct = 0D
                End If

                If builderPrice > 0 Then
                    comp.BuilderMarginPct = (builderPrice - cost) / builderPrice
                Else
                    comp.BuilderMarginPct = 0D
                End If
            End If
        Next

        dgvComponents.DataSource = Nothing
        dgvComponents.DataSource = _componentPricing
    End Sub

    Private Sub LoadMaterialPricing()
        _materialPricing = _dataAccess.GetMaterialPricingByLock(_priceLock.PriceLockID)
        dgvMaterials.DataSource = Nothing
        dgvMaterials.DataSource = _materialPricing
    End Sub

#End Region

#Region "CRUD Operations"

    Private Sub SavePriceLock()
        Try
            Cursor = Cursors.WaitCursor

            _priceLock.PriceLockDate = dtpLockDate.Value.Date
            _priceLock.PriceLockName = txtLockName.Text.Trim()
            _priceLock.BaseMgmtMargin = nudBaseMgmtMargin.Value
            _priceLock.AdjustedMarginBaseModels = nudAdjustedMargin.Value
            _priceLock.OptionMargin = nudOptionMargin.Value

            ' Set RL import IDs from combo boxes
            _priceLock.BaselineRLImportID = GetSelectedBaselineRLImportID()
            _priceLock.CurrentRLImportID = GetSelectedCurrentRLImportID()

            ' Save everything in one call
            _dataAccess.UpdatePriceLock(_priceLock)

            ' Update original values after save
            _originalAdjustedMargin = nudAdjustedMargin.Value
            _originalOptionMargin = nudOptionMargin.Value

            _isDirty = False
            MessageBox.Show("Price lock saved successfully.", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show($"Error saving price lock: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ' ENHANCED: Update prices when margin changes
    Private Sub UpdatePricesForMarginChange(marginType As String, newMargin As Decimal)
        Try
            Cursor = Cursors.WaitCursor

            _dataAccess.RecalculatePricesForMarginChange(_priceLock.PriceLockID, marginType, newMargin, Environment.UserName)

            ' Reload the grid to show updated prices
            LoadComponentPricing()

            MessageBox.Show($"Prices updated for {marginType} margin records.", "Updated", MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show($"Error updating prices: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Sub ChangeStatus()
        Dim currentStatus = _priceLock.Status
        Dim newStatus As String = Nothing

        ' Determine next status
        Select Case currentStatus
            Case PLPriceLockStatus.Draft
                newStatus = PLPriceLockStatus.Pending
            Case PLPriceLockStatus.Pending
                If MessageBox.Show("Approve this price lock?", "Confirm", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    newStatus = PLPriceLockStatus.Approved
                End If
            Case PLPriceLockStatus.Approved
                If MessageBox.Show("Mark as sent to builder?", "Confirm", MessageBoxButtons.YesNo) = DialogResult.Yes Then
                    newStatus = PLPriceLockStatus.Sent
                End If
            Case PLPriceLockStatus.Sent
                MessageBox.Show("This price lock has already been sent.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Return
        End Select

        If newStatus IsNot Nothing Then
            Try
                _dataAccess.UpdatePriceLockStatus(_priceLock.PriceLockID, newStatus, Environment.UserName)
                _priceLock.Status = newStatus
                cboStatus.Text = newStatus
                UpdateControlStates()
            Catch ex As Exception
                MessageBox.Show($"Error updating status: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub AddComponent()
        Using frm As New frmComponentPricingEdit(Nothing, _priceLock, _dataAccess)
            If frm.ShowDialog() = DialogResult.OK Then
                LoadComponentPricing()
            End If
        End Using
    End Sub

    Private Sub EditSelectedComponent()
        If dgvComponents.SelectedRows.Count = 0 Then Return
        Dim index = dgvComponents.SelectedRows(0).Index
        If index < 0 OrElse index >= _componentPricing.Count Then Return

        Dim comp = _componentPricing(index)
        Dim componentPricingID = comp.ComponentPricingID ' Store the ID for re-selection

        Using frm As New frmComponentPricingEdit(comp, _priceLock, _dataAccess)
            If frm.ShowDialog() = DialogResult.OK Then
                LoadComponentPricing() ' This reloads from database

                ' ENHANCED: Reselect the edited row
                For i = 0 To dgvComponents.Rows.Count - 1
                    If dgvComponents.Rows(i).Cells("ComponentPricingID").Value IsNot Nothing AndAlso
                   CInt(dgvComponents.Rows(i).Cells("ComponentPricingID").Value) = componentPricingID Then
                        dgvComponents.ClearSelection()
                        dgvComponents.Rows(i).Selected = True
                        dgvComponents.FirstDisplayedScrollingRowIndex = i
                        Exit For
                    End If
                Next
            End If
        End Using
    End Sub

    Private Sub DeleteSelectedComponent()
        If dgvComponents.SelectedRows.Count = 0 Then Return
        Dim index = dgvComponents.SelectedRows(0).Index
        If index < 0 OrElse index >= _componentPricing.Count Then Return

        Dim comp = _componentPricing(index)

        If MessageBox.Show($"Delete component pricing for {comp.FullDescription}?",
            "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) = DialogResult.Yes Then
            Try
                _dataAccess.DeleteComponentPricing(comp.ComponentPricingID)
                LoadComponentPricing()
            Catch ex As Exception
                MessageBox.Show($"Error deleting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub EditSelectedMaterial()
        If dgvMaterials.SelectedRows.Count = 0 Then Return
        Dim index = dgvMaterials.SelectedRows(0).Index
        If index < 0 OrElse index >= _materialPricing.Count Then Return

        Dim mat = _materialPricing(index)

        Using frm As New frmMaterialPricingEdit(mat, _priceLock, _dataAccess)
            If frm.ShowDialog() = DialogResult.OK Then
                LoadMaterialPricing()
            End If
        End Using
    End Sub

    Private Sub ImportMitekCsv()
        Using frm As New frmMitekImport(_priceLock, _dataAccess)
            If frm.ShowDialog() = DialogResult.OK Then
                LoadComponentPricing()
            End If
        End Using
    End Sub

    Private Sub UpdateControlStates()
        Dim canEdit = _priceLock.CanEdit

        dtpLockDate.Enabled = canEdit
        txtLockName.Enabled = canEdit
        nudBaseMgmtMargin.Enabled = canEdit
        nudAdjustedMargin.Enabled = canEdit
        nudOptionMargin.Enabled = canEdit
        btnAddComponent.Enabled = canEdit
        btnEditComponent.Enabled = canEdit
        btnDeleteComponent.Enabled = canEdit
        btnImportMitek.Enabled = canEdit
        btnEditMaterial.Enabled = canEdit
        btnSave.Enabled = canEdit
        btnAddMaterial.Enabled = canEdit


        ' Update status button text
        Select Case _priceLock.Status
            Case PLPriceLockStatus.Draft
                btnChangeStatus.Text = "Submit"
            Case PLPriceLockStatus.Pending
                btnChangeStatus.Text = "Approve"
            Case PLPriceLockStatus.Approved
                btnChangeStatus.Text = "Mark Sent"
            Case PLPriceLockStatus.Sent
                btnChangeStatus.Text = "Sent"
                btnChangeStatus.Enabled = False
        End Select
        ' RL import controls
        cboBaselineRL.Enabled = canEdit
        cboCurrentRL.Enabled = canEdit
        btnManageRLImports.Enabled = True  ' Always allow viewing RL imports
        UpdateRLStatus()  ' This will set btnCalculateFromRL.Enabled
    End Sub

    ' ADD these methods in the "CRUD Operations" region (after EditSelectedMaterial, around line 670)

    Private Sub AddMaterial()
        Using frm As New frmMaterialPricingAdd(_priceLock, _dataAccess)
            If frm.ShowDialog() = DialogResult.OK Then
                LoadMaterialPricing()
            End If
        End Using
    End Sub



#End Region

#Region "Random Lengths Import Selection"

    ''' <summary>
    ''' Load available Random Lengths imports into the combo boxes
    ''' </summary>
    Private Sub LoadRandomLengthsImports()
        Try
            _rlImports = _dataAccess.GetRandomLengthsImports()

            ' Add a blank/none option at the start
            cboBaselineRL.Items.Clear()
            cboCurrentRL.Items.Clear()

            cboBaselineRL.Items.Add("(None)")
            cboCurrentRL.Items.Add("(None)")

            For Each rlImport In _rlImports
                cboBaselineRL.Items.Add(rlImport)
                cboCurrentRL.Items.Add(rlImport)
            Next

            ' Select current values if set on the price lock
            SelectRLImportInCombo(cboBaselineRL, _priceLock.BaselineRLImportID)
            SelectRLImportInCombo(cboCurrentRL, _priceLock.CurrentRLImportID)

            UpdateRLStatus()

        Catch ex As Exception
            ' Don't fail the form load, just log it
            System.Diagnostics.Debug.WriteLine($"Error loading RL imports: {ex.Message}")
            lblRLStatus.Text = "Error loading RL imports"
            lblRLStatus.ForeColor = Color.Red
        End Try
    End Sub

    ''' <summary>
    ''' Select the appropriate item in the combo box based on import ID
    ''' </summary>
    Private Sub SelectRLImportInCombo(combo As ComboBox, importID As Integer?)
        If Not importID.HasValue OrElse importID.Value = 0 Then
            combo.SelectedIndex = 0 ' Select "(None)"
            Return
        End If

        For i = 1 To combo.Items.Count - 1
            Dim item = TryCast(combo.Items(i), PLRandomLengthsImport)
            If item IsNot Nothing AndAlso item.RandomLengthsImportID = importID.Value Then
                combo.SelectedIndex = i
                Return
            End If
        Next

        ' Not found, select none
        combo.SelectedIndex = 0
    End Sub

    ''' <summary>
    ''' Handle changes to RL import selection
    ''' </summary>
    Private Sub RLSelection_Changed(sender As Object, e As EventArgs)
        _isDirty = True
        UpdateRLStatus()
    End Sub

    ''' <summary>
    ''' Update the status label showing RL selection state
    ''' </summary>
    Private Sub UpdateRLStatus()
        Dim baselineSelected = cboBaselineRL.SelectedIndex > 0
        Dim currentSelected = cboCurrentRL.SelectedIndex > 0

        If baselineSelected AndAlso currentSelected Then
            ' Both selected - show percent change preview
            Dim baselineImport = TryCast(cboBaselineRL.SelectedItem, PLRandomLengthsImport)
            Dim currentImport = TryCast(cboCurrentRL.SelectedItem, PLRandomLengthsImport)

            If baselineImport IsNot Nothing AndAlso currentImport IsNot Nothing Then
                lblRLStatus.Text = $"Baseline: {baselineImport.ReportDate:MM/dd/yy} → Current: {currentImport.ReportDate:MM/dd/yy}"
                lblRLStatus.ForeColor = Color.Green
                btnCalculateFromRL.Enabled = _priceLock.CanEdit
            End If
        ElseIf baselineSelected OrElse currentSelected Then
            lblRLStatus.Text = "Select both Baseline and Current RL imports"
            lblRLStatus.ForeColor = Color.Orange
            btnCalculateFromRL.Enabled = False
        Else
            lblRLStatus.Text = "No RL imports selected"
            lblRLStatus.ForeColor = Color.Gray
            btnCalculateFromRL.Enabled = False
        End If
    End Sub

    ''' <summary>
    ''' Open the RL Import Manager dialog
    ''' </summary>
    Private Sub OpenRLImportManager()
        Using frm As New frmRandomLengthsImportManager(_dataAccess)
            frm.ShowDialog()
        End Using

        ' Reload imports in case any were added/changed
        LoadRandomLengthsImports()
    End Sub

    ''' <summary>
    ''' Calculate material pricing based on selected RL imports
    ''' </summary>
    Private Sub CalculateMaterialPricingFromRL()
        ' Validate selections
        If cboBaselineRL.SelectedIndex <= 0 OrElse cboCurrentRL.SelectedIndex <= 0 Then
            MessageBox.Show("Please select both a Baseline and Current Random Lengths import.",
                "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim baselineImport = TryCast(cboBaselineRL.SelectedItem, PLRandomLengthsImport)
        Dim currentImport = TryCast(cboCurrentRL.SelectedItem, PLRandomLengthsImport)

        If baselineImport Is Nothing OrElse currentImport Is Nothing Then Return

        ' Check if material pricing exists
        If _materialPricing Is Nothing OrElse _materialPricing.Count = 0 Then
            MessageBox.Show("No material pricing records found. Please add material categories first.",
                "No Materials", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Check if Original Sell Prices are set
        Dim missingOriginal = _materialPricing.Where(Function(m) Not m.OriginalSellPrice.HasValue OrElse m.OriginalSellPrice.Value = 0).ToList()
        If missingOriginal.Count > 0 Then
            Dim msg = $"{missingOriginal.Count} material(s) don't have an Original Sell Price set." & vbCrLf & vbCrLf &
                      "The Original Sell Price is the base price that gets adjusted by the RL percent change." & vbCrLf & vbCrLf &
                      "Would you like to continue? (Materials without Original Sell Price will be skipped)"

            If MessageBox.Show(msg, "Missing Original Prices", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
                Return
            End If
        End If

        ' Confirm calculation
        Dim confirmMsg = $"This will calculate new material prices based on:" & vbCrLf & vbCrLf &
                         $"  Baseline RL: {baselineImport.ReportDate:MM/dd/yyyy} {baselineImport.ReportName}" & vbCrLf &
                         $"  Current RL:  {currentImport.ReportDate:MM/dd/yyyy} {currentImport.ReportName}" & vbCrLf & vbCrLf &
                         "Formula: New Price = Original Sell Price × (1 + RL % Change)" & vbCrLf & vbCrLf &
                         "Do you want to continue?"

        If MessageBox.Show(confirmMsg, "Confirm Calculation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = DialogResult.No Then
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor

            ' First, save the RL import selections to the price lock
            _dataAccess.UpdatePriceLockRLImports(
                _priceLock.PriceLockID,
                baselineImport.RandomLengthsImportID,
                currentImport.RandomLengthsImportID)

            ' Update local object
            _priceLock.BaselineRLImportID = baselineImport.RandomLengthsImportID
            _priceLock.CurrentRLImportID = currentImport.RandomLengthsImportID

            ' Calculate and update material pricing
            _dataAccess.CalculateMaterialPricingFromRL(_priceLock.PriceLockID, Environment.UserName)

            ' Reload material pricing to show results
            LoadMaterialPricing()

            MessageBox.Show("Material prices calculated successfully.", "Calculation Complete",
                MessageBoxButtons.OK, MessageBoxIcon.Information)

        Catch ex As Exception
            MessageBox.Show($"Error calculating prices: {ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    ''' <summary>
    ''' Get the currently selected baseline RL import ID (for saving)
    ''' </summary>
    Private Function GetSelectedBaselineRLImportID() As Integer?
        If cboBaselineRL.SelectedIndex <= 0 Then Return Nothing
        Dim item = TryCast(cboBaselineRL.SelectedItem, PLRandomLengthsImport)
        Return If(item IsNot Nothing, item.RandomLengthsImportID, CType(Nothing, Integer?))
    End Function

    ''' <summary>
    ''' Get the currently selected current RL import ID (for saving)
    ''' </summary>
    Private Function GetSelectedCurrentRLImportID() As Integer?
        If cboCurrentRL.SelectedIndex <= 0 Then Return Nothing
        Dim item = TryCast(cboCurrentRL.SelectedItem, PLRandomLengthsImport)
        Return If(item IsNot Nothing, item.RandomLengthsImportID, CType(Nothing, Integer?))
    End Function

#End Region

End Class