' =====================================================
' frmComponentPricingEdit.vb
' Dialog for editing a component pricing record
' ENHANCED: Margin source selection and price update prompts
' ENHANCED: Allow editing Plan/Elevation/Option on existing records
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Public Class frmComponentPricingEdit

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _priceLock As PLPriceLock
    Private _componentPricing As PLComponentPricing
    Private _isNew As Boolean
    Private _previousCalculatedPrice As Decimal?

    ' Track reference units for adder
    Private _referenceUnits As List(Of PLReferenceUnit)

    ' Previous Price Lock reference data (for display only)
    Private _hasPreviousPricing As Boolean = False

    ' Track if key fields are unlocked for editing
    Private _keyFieldsUnlocked As Boolean = False

    ' Store original values for change detection
    Private _originalPlanID As Integer
    Private _originalElevationID As Integer
    Private _originalOptionID As Integer?
    Private _originalProductTypeID As Integer

#End Region

#Region "Constructor"

    Public Sub New(componentPricing As PLComponentPricing, priceLock As PLPriceLock, dataAccess As PriceLockDataAccess)
        _componentPricing = componentPricing
        _priceLock = priceLock
        _dataAccess = dataAccess
        _isNew = (componentPricing Is Nothing)

        If _isNew Then
            _componentPricing = New PLComponentPricing() With {
                .PriceLockID = priceLock.PriceLockID,
                .MarginSource = "Adjusted"
            }
        Else
            ' Store original values for existing records
            _originalPlanID = componentPricing.PlanID
            _originalElevationID = componentPricing.ElevationID
            _originalOptionID = componentPricing.OptionID
            _originalProductTypeID = componentPricing.ProductTypeID
        End If

        InitializeComponent()

        Me.Text = If(_isNew, "Add Component Pricing", "Edit Component Pricing")
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmComponentPricingEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        ' Initialize margin source combo
        cboMarginSource.Items.AddRange(New String() {"Adjusted", "Option"})
        LoadDropdowns()
        LoadComponentData()

        ' Setup unlock button for existing records
        SetupUnlockButton()
    End Sub

    Private Sub cboPlan_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboPlan.SelectedIndexChanged
        LoadElevations()
        btnAddElevation.Enabled = (cboPlan.SelectedItem IsNot Nothing) AndAlso (_isNew OrElse _keyFieldsUnlocked)

        ' Reload reference units if this is an adder and we're in unlock mode
        If chkIsAdder.Checked AndAlso _keyFieldsUnlocked Then
            LoadReferenceUnits()
        End If
    End Sub

    Private Sub cboOption_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboOption.SelectedIndexChanged
        If _isNew AndAlso cboOption.SelectedItem IsNot Nothing Then
            Dim optionID = CInt(DirectCast(cboOption.SelectedItem, ListItem).Value)
            If optionID > 0 Then
                cboMarginSource.SelectedItem = "Option"
                nudAppliedMargin.Value = _priceLock.OptionMargin.GetValueOrDefault(0.3D)
            Else
                cboMarginSource.SelectedItem = "Adjusted"
                nudAppliedMargin.Value = _priceLock.AdjustedMarginBaseModels.GetValueOrDefault(0.15D)
            End If
        End If
    End Sub

    Private Sub cboMarginSource_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboMarginSource.SelectedIndexChanged
        If cboMarginSource.SelectedItem Is Nothing Then Return

        Dim marginSource = cboMarginSource.SelectedItem.ToString()
        If marginSource = "Option" Then
            nudAppliedMargin.Value = _priceLock.OptionMargin.GetValueOrDefault(0.3D)
        Else
            nudAppliedMargin.Value = _priceLock.AdjustedMarginBaseModels.GetValueOrDefault(0.15D)
        End If

        CalculatePrice()
    End Sub

    Private Sub nudCost_ValueChanged(sender As Object, e As EventArgs) Handles nudCost.ValueChanged
        CalculatePrice()
    End Sub

    Private Sub nudAppliedMargin_ValueChanged(sender As Object, e As EventArgs) Handles nudAppliedMargin.ValueChanged
        CalculatePrice()
    End Sub

    Private Sub nudPriceSentToBuilder_ValueChanged(sender As Object, e As EventArgs) Handles nudPriceSentToBuilder.ValueChanged
        lblPriceDiffWarning.Visible = (nudPriceSentToBuilder.Value <> nudFinalPrice.Value)
    End Sub

    Private Sub btnRecalculate_Click(sender As Object, e As EventArgs) Handles btnRecalculate.Click
        CalculatePrice()
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If ValidateInput() Then SaveComponentPricing()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub chkIsAdder_CheckedChanged(sender As Object, e As EventArgs) Handles chkIsAdder.CheckedChanged
        UpdateAdderControlsVisibility()
        If chkIsAdder.Checked Then
            LoadReferenceUnits()
        End If
        CalculatePrice()
    End Sub

    Private Sub cboReferenceUnit_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboReferenceUnit.SelectedIndexChanged
        UpdateAdderCostDisplay()
        CalculatePrice()
    End Sub

    ''' <summary>
    ''' Handles the unlock button click to enable editing of key fields
    ''' </summary>
    Private Sub btnUnlockKeyFields_Click(sender As Object, e As EventArgs) Handles btnUnlockKeyFields.Click
        UnlockKeyFieldsForEditing()
    End Sub

#End Region

#Region "Unlock Key Fields"

    ''' <summary>
    ''' Setup the unlock button visibility and state
    ''' </summary>
    Private Sub SetupUnlockButton()
        ' Only show unlock button for existing records
        btnUnlockKeyFields.Visible = Not _isNew
        lblUnlockWarning.Visible = False

        If Not _isNew Then
            btnUnlockKeyFields.Text = "🔓 Unlock Plan/Elev/Option"
            btnUnlockKeyFields.Enabled = True
        End If
    End Sub

    ''' <summary>
    ''' Unlocks the key identifier fields for editing after user confirmation
    ''' </summary>
    Private Sub UnlockKeyFieldsForEditing()
        Dim msg = "WARNING: You are about to enable editing of the Plan, Elevation, Option, and Product Type fields." & vbCrLf & vbCrLf &
                  "This should only be done to CORRECT data entry errors, not to repurpose a pricing record." & vbCrLf & vbCrLf &
                  "Changes to these fields will:" & vbCrLf &
                  "  • Update the existing record's classification" & vbCrLf &
                  "  • NOT create a new pricing record" & vbCrLf &
                  "  • May affect historical comparisons" & vbCrLf & vbCrLf &
                  "Are you sure you want to unlock these fields?"

        Dim result = MessageBox.Show(msg, "Confirm Unlock", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2)

        If result = DialogResult.Yes Then
            _keyFieldsUnlocked = True

            ' Enable the key fields
            cboPlan.Enabled = True
            cboElevation.Enabled = True
            cboOption.Enabled = True
            cboProductType.Enabled = True

            ' Enable add buttons
            btnAddPlan.Enabled = True
            btnAddElevation.Enabled = (cboPlan.SelectedItem IsNot Nothing)
            btnAddOption.Enabled = True

            ' Update UI to show unlocked state
            btnUnlockKeyFields.Text = "🔓 Fields Unlocked"
            btnUnlockKeyFields.Enabled = False
            btnUnlockKeyFields.BackColor = Color.LightGreen

            ' Show warning label
            lblUnlockWarning.Text = "⚠️ Key fields are unlocked for editing. Save will update this record's classification."
            lblUnlockWarning.ForeColor = Color.DarkOrange
            lblUnlockWarning.Visible = True

            ' Highlight the unlocked fields
            cboPlan.BackColor = Color.LightYellow
            cboElevation.BackColor = Color.LightYellow
            cboOption.BackColor = Color.LightYellow
            cboProductType.BackColor = Color.LightYellow
        End If
    End Sub

#End Region

#Region "Add New Buttons"

    Private Sub btnAddPlan_Click(sender As Object, e As EventArgs) Handles btnAddPlan.Click
        Dim planName = InputBox("Enter new Plan name:", "Add Plan")
        If String.IsNullOrWhiteSpace(planName) Then Return

        Try
            Dim newPlan As New PLPlan() With {
                .PlanName = planName.Trim(),
                .IsActive = True
            }
            newPlan.PlanID = _dataAccess.InsertPlan(newPlan)
            _dataAccess.AssignPlanToSubdivision(_priceLock.SubdivisionID, newPlan.PlanID)

            LoadPlans()
            SelectComboItem(cboPlan, newPlan.PlanID)

            MessageBox.Show($"Plan '{planName}' created and assigned to subdivision.", "Plan Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"Error adding plan: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnAddElevation_Click(sender As Object, e As EventArgs) Handles btnAddElevation.Click
        If cboPlan.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a Plan first.", "No Plan Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim elevName = InputBox("Enter new Elevation name (e.g., A, B, C):", "Add Elevation")
        If String.IsNullOrWhiteSpace(elevName) Then Return

        Try
            Dim planID = CInt(DirectCast(cboPlan.SelectedItem, ListItem).Value)

            Dim newElev As New PLElevation() With {
                .PlanID = planID,
                .ElevationName = elevName.Trim(),
                .IsActive = True
            }
            newElev.ElevationID = _dataAccess.InsertElevation(newElev)

            LoadElevations()
            SelectComboItem(cboElevation, newElev.ElevationID)

            MessageBox.Show($"Elevation '{elevName}' added.", "Elevation Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"Error adding elevation: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnAddOption_Click(sender As Object, e As EventArgs) Handles btnAddOption.Click
        Dim optionName = InputBox("Enter new Option name (e.g., Outdoor Room, Oversized Garage):", "Add Option")
        If String.IsNullOrWhiteSpace(optionName) Then Return

        Try
            Dim newOption As New PLOption() With {
                .OptionName = optionName.Trim(),
                .IsActive = True
            }
            newOption.OptionID = _dataAccess.InsertOption(newOption)

            LoadOptions()
            SelectComboItem(cboOption, newOption.OptionID)

            MessageBox.Show($"Option '{optionName}' added.", "Option Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"Error adding option: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadDropdowns()
        LoadPlans()
        LoadOptions()
        LoadProductTypes()
    End Sub

    Private Sub LoadPlans()
        Dim selectedID As Integer? = Nothing
        If cboPlan.SelectedItem IsNot Nothing Then
            selectedID = CInt(DirectCast(cboPlan.SelectedItem, ListItem).Value)
        End If

        cboPlan.Items.Clear()
        For Each plan In _dataAccess.GetPlansBySubdivision(_priceLock.SubdivisionID)
            cboPlan.Items.Add(New ListItem(plan.PlanName, plan.PlanID))
        Next

        If selectedID.HasValue Then
            SelectComboItem(cboPlan, selectedID.Value)
        End If
    End Sub

    Private Sub LoadElevations()
        Dim selectedID As Integer? = Nothing
        If cboElevation.SelectedItem IsNot Nothing Then
            selectedID = CInt(DirectCast(cboElevation.SelectedItem, ListItem).Value)
        End If

        cboElevation.Items.Clear()
        Dim selectedPlan = TryCast(cboPlan.SelectedItem, ListItem)
        If selectedPlan Is Nothing Then Return

        For Each elev In _dataAccess.GetElevationsByPlan(CInt(selectedPlan.Value))
            cboElevation.Items.Add(New ListItem(elev.ElevationName, elev.ElevationID))
        Next

        If selectedID.HasValue Then
            SelectComboItem(cboElevation, selectedID.Value)
        ElseIf cboElevation.Items.Count > 0 Then
            cboElevation.SelectedIndex = 0
        End If
    End Sub

    Private Sub LoadOptions()
        Dim selectedID As Integer? = Nothing
        If cboOption.SelectedItem IsNot Nothing Then
            selectedID = CInt(DirectCast(cboOption.SelectedItem, ListItem).Value)
        End If

        cboOption.Items.Clear()
        cboOption.Items.Add(New ListItem("(None - Base Elevation)", 0))
        For Each opt In _dataAccess.GetOptions()
            cboOption.Items.Add(New ListItem(opt.OptionName, opt.OptionID))
        Next

        If selectedID.HasValue Then
            SelectComboItem(cboOption, selectedID.Value)
        Else
            cboOption.SelectedIndex = 0
        End If
    End Sub

    Private Sub LoadProductTypes()
        cboProductType.Items.Clear()
        For Each pt In _dataAccess.GetProductTypes()
            cboProductType.Items.Add(New ListItem(pt.ProductTypeName, pt.ProductTypeID))
        Next
    End Sub

    Private Sub LoadComponentData()
        If _isNew Then
            cboMarginSource.SelectedItem = "Adjusted"
            nudAppliedMargin.Value = _priceLock.AdjustedMarginBaseModels.GetValueOrDefault(0.15D)
            btnAddElevation.Enabled = False
            pnlAdderSettings.Visible = False
            Return
        End If

        SelectComboItem(cboPlan, _componentPricing.PlanID)
        LoadElevations()
        SelectComboItem(cboElevation, _componentPricing.ElevationID)
        SelectComboItem(cboOption, If(_componentPricing.OptionID, 0))
        SelectComboItem(cboProductType, _componentPricing.ProductTypeID)

        cboMarginSource.SelectedItem = If(String.IsNullOrEmpty(_componentPricing.MarginSource), "Adjusted", _componentPricing.MarginSource)

        chkIsAdder.Checked = _componentPricing.IsAdder

        If _componentPricing.IsAdder Then
            UpdateAdderControlsVisibility()
            LoadReferenceUnits()
        End If

        nudMgmtSellPrice.Value = If(_componentPricing.MgmtSellPrice, 0D)
        nudCost.Value = If(_componentPricing.Cost, 0D)
        nudAppliedMargin.Value = If(_componentPricing.AppliedMargin, 0.15D)
        nudFinalPrice.Value = If(_componentPricing.FinalPrice, 0D)
        nudPriceSentToBuilder.Value = If(_componentPricing.PriceSentToBuilder, 0D)
        txtPriceNote.Text = _componentPricing.PriceNote

        _previousCalculatedPrice = _componentPricing.CalculatedPrice

        CalculatePrice()
        lblPriceDiffWarning.Visible = _componentPricing.HasPriceDifference

        PopulatePreviousPricingPanel()

        ' Disable key fields for existing records (until unlocked)
        cboPlan.Enabled = False
        cboElevation.Enabled = False
        cboOption.Enabled = False
        cboProductType.Enabled = False
        btnAddPlan.Enabled = False
        btnAddElevation.Enabled = False
        btnAddOption.Enabled = False
    End Sub

    Private Sub PopulatePreviousPricingPanel()
        If _isNew Then
            grpPreviousPricing.Visible = False
            Return
        End If

        grpPreviousPricing.Visible = True
        _hasPreviousPricing = _componentPricing.HasPreviousPricing

        If Not _hasPreviousPricing Then
            lblPrevSalesValue.Text = "N/A"
            lblPrevSalesValue.ForeColor = Color.Gray
            lblPctChgSalesValue.Text = "N/A"
            lblPctChgSalesValue.ForeColor = Color.Gray
            lblPrevBuilderValue.Text = "N/A"
            lblPrevBuilderValue.ForeColor = Color.Gray
            lblPctChgBuilderValue.Text = "N/A"
            lblPctChgBuilderValue.ForeColor = Color.Gray
            Return
        End If

        If _componentPricing.PreviousPriceSentToSales.HasValue Then
            lblPrevSalesValue.Text = _componentPricing.PreviousPriceSentToSales.Value.ToString("C2")
            lblPrevSalesValue.ForeColor = Color.Black
        Else
            lblPrevSalesValue.Text = "N/A"
            lblPrevSalesValue.ForeColor = Color.Gray
        End If

        If _componentPricing.PctChangeSentToSales.HasValue Then
            Dim pctChange = _componentPricing.PctChangeSentToSales.Value
            lblPctChgSalesValue.Text = pctChange.ToString("P1")
            If pctChange > 0.05D Then
                lblPctChgSalesValue.ForeColor = Color.Red
                lblPctChgSalesValue.Font = New Font(lblPctChgSalesValue.Font, FontStyle.Bold)
            ElseIf pctChange < -0.05D Then
                lblPctChgSalesValue.ForeColor = Color.Green
                lblPctChgSalesValue.Font = New Font(lblPctChgSalesValue.Font, FontStyle.Bold)
            ElseIf Math.Abs(pctChange) > 0.02D Then
                lblPctChgSalesValue.ForeColor = Color.DarkOrange
                lblPctChgSalesValue.Font = New Font(lblPctChgSalesValue.Font, FontStyle.Regular)
            Else
                lblPctChgSalesValue.ForeColor = Color.Black
                lblPctChgSalesValue.Font = New Font(lblPctChgSalesValue.Font, FontStyle.Regular)
            End If
        Else
            lblPctChgSalesValue.Text = "N/A"
            lblPctChgSalesValue.ForeColor = Color.Gray
        End If

        If _componentPricing.PreviousPriceSentToBuilder.HasValue Then
            lblPrevBuilderValue.Text = _componentPricing.PreviousPriceSentToBuilder.Value.ToString("C2")
            lblPrevBuilderValue.ForeColor = Color.Black
        Else
            lblPrevBuilderValue.Text = "N/A"
            lblPrevBuilderValue.ForeColor = Color.Gray
        End If

        If _componentPricing.PctChangeSentToBuilder.HasValue Then
            Dim pctChange = _componentPricing.PctChangeSentToBuilder.Value
            lblPctChgBuilderValue.Text = pctChange.ToString("P1")
            If pctChange > 0.05D Then
                lblPctChgBuilderValue.ForeColor = Color.Red
                lblPctChgBuilderValue.Font = New Font(lblPctChgBuilderValue.Font, FontStyle.Bold)
            ElseIf pctChange < -0.05D Then
                lblPctChgBuilderValue.ForeColor = Color.Green
                lblPctChgBuilderValue.Font = New Font(lblPctChgBuilderValue.Font, FontStyle.Bold)
            ElseIf Math.Abs(pctChange) > 0.02D Then
                lblPctChgBuilderValue.ForeColor = Color.DarkOrange
                lblPctChgBuilderValue.Font = New Font(lblPctChgBuilderValue.Font, FontStyle.Regular)
            Else
                lblPctChgBuilderValue.ForeColor = Color.Black
                lblPctChgBuilderValue.Font = New Font(lblPctChgBuilderValue.Font, FontStyle.Regular)
            End If
        Else
            lblPctChgBuilderValue.Text = "N/A"
            lblPctChgBuilderValue.ForeColor = Color.Gray
        End If
    End Sub

    Private Sub SelectComboItem(combo As ComboBox, value As Integer)
        For i = 0 To combo.Items.Count - 1
            Dim item = TryCast(combo.Items(i), ListItem)
            If item IsNot Nothing AndAlso CInt(item.Value) = value Then
                combo.SelectedIndex = i
                Exit For
            End If
        Next
    End Sub

#End Region

#Region "Adder Support"

    Private Sub UpdateAdderControlsVisibility()
        pnlAdderSettings.Visible = chkIsAdder.Checked
    End Sub

    Private Sub LoadReferenceUnits()
        cboReferenceUnit.Items.Clear()
        cboReferenceUnit.Items.Add(New ListItem("(Select reference unit...)", 0))

        If cboPlan.SelectedItem Is Nothing OrElse cboProductType.SelectedItem Is Nothing Then
            cboReferenceUnit.SelectedIndex = 0
            Return
        End If

        Dim planID = CInt(DirectCast(cboPlan.SelectedItem, ListItem).Value)
        Dim productTypeID = CInt(DirectCast(cboProductType.SelectedItem, ListItem).Value)
        Dim excludeID = If(_isNew, 0, _componentPricing.ComponentPricingID)

        _referenceUnits = _dataAccess.GetPotentialReferenceUnits(_priceLock.PriceLockID, planID, productTypeID, excludeID)

        For Each unit In _referenceUnits
            cboReferenceUnit.Items.Add(New ListItem($"{unit.ElevationName} - Cost: {unit.Cost:C2}", unit.ComponentPricingID))
        Next

        If Not _isNew AndAlso _componentPricing.BaseComponentPricingID.HasValue Then
            SelectComboItem(cboReferenceUnit, _componentPricing.BaseComponentPricingID.Value)
        Else
            cboReferenceUnit.SelectedIndex = 0
        End If
    End Sub

    Private Sub UpdateAdderCostDisplay()
        If Not chkIsAdder.Checked Then
            lblAdderCost.Text = ""
            Return
        End If

        Dim selectedRef = TryCast(cboReferenceUnit.SelectedItem, ListItem)
        If selectedRef Is Nothing OrElse CInt(selectedRef.Value) = 0 Then
            lblAdderCost.Text = "(Select a reference unit)"
            lblAdderCost.ForeColor = Color.Gray
            Return
        End If

        Dim refID = CInt(selectedRef.Value)
        Dim refUnit = _referenceUnits?.FirstOrDefault(Function(r) r.ComponentPricingID = refID)

        If refUnit IsNot Nothing AndAlso refUnit.Cost.HasValue Then
            Dim adderCost = nudCost.Value - refUnit.Cost.Value
            If adderCost >= 0 Then
                lblAdderCost.Text = $"{adderCost:C2} (This Cost {nudCost.Value:C2} - Ref Cost {refUnit.Cost.Value:C2})"
                lblAdderCost.ForeColor = Color.DarkGreen
            Else
                lblAdderCost.Text = $"{adderCost:C2} (Warning: Negative adder)"
                lblAdderCost.ForeColor = Color.Red
            End If
        Else
            lblAdderCost.Text = "(Reference unit has no cost)"
            lblAdderCost.ForeColor = Color.Orange
        End If
    End Sub

    Private Function GetEffectiveCostForCalculation() As Decimal
        If Not chkIsAdder.Checked Then
            Return nudCost.Value
        End If

        Dim selectedRef = TryCast(cboReferenceUnit.SelectedItem, ListItem)
        If selectedRef Is Nothing OrElse CInt(selectedRef.Value) = 0 Then
            Return nudCost.Value
        End If

        Dim refID = CInt(selectedRef.Value)
        Dim refUnit = _referenceUnits?.FirstOrDefault(Function(r) r.ComponentPricingID = refID)

        If refUnit IsNot Nothing AndAlso refUnit.Cost.HasValue Then
            Dim adderCost = nudCost.Value - refUnit.Cost.Value
            Return adderCost
        End If

        Return nudCost.Value
    End Function

#End Region

#Region "Calculation"

    Private Sub CalculatePrice()
        Dim cost = GetEffectiveCostForCalculation()
        Dim margin = nudAppliedMargin.Value

        If margin >= 1 Then
            lblCalculatedPrice.Text = "Invalid margin"
            Return
        End If

        Dim calculated = cost / (1 - margin)
        Dim rounded = Math.Ceiling(calculated)

        If chkIsAdder.Checked Then
            lblCalculatedPrice.Text = $"{rounded:C2} (from adder cost {cost:C2})"
        Else
            lblCalculatedPrice.Text = rounded.ToString("C2")
        End If

        _componentPricing.CalculatedPrice = calculated

        If chkIsAdder.Checked Then
            UpdateAdderCostDisplay()
        End If

        If Not _isNew AndAlso _previousCalculatedPrice.HasValue AndAlso _previousCalculatedPrice.Value <> calculated Then
            Dim msg = $"The calculated price has changed from {_previousCalculatedPrice.Value:C2} to {calculated:C2}." & vbCrLf & vbCrLf &
                      "Do you want to update the Final Price and Builder Price to match?"

            Dim result = MessageBox.Show(msg, "Update Prices?", MessageBoxButtons.YesNo, MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                nudFinalPrice.Value = rounded
                nudPriceSentToBuilder.Value = rounded
            End If

            _previousCalculatedPrice = calculated
        ElseIf nudFinalPrice.Value = 0 OrElse _isNew Then
            nudFinalPrice.Value = rounded
            nudPriceSentToBuilder.Value = rounded
        End If
    End Sub

#End Region

#Region "Validation and Save"

    Private Function ValidateInput() As Boolean
        If cboPlan.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a plan.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        If cboElevation.SelectedItem Is Nothing Then
            MessageBox.Show("Please select an elevation.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        If cboProductType.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a product type.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If
        If cboMarginSource.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a margin source.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return False
        End If

        If chkIsAdder.Checked Then
            Dim selectedRef = TryCast(cboReferenceUnit.SelectedItem, ListItem)
            If selectedRef Is Nothing OrElse CInt(selectedRef.Value) = 0 Then
                MessageBox.Show("Please select a reference unit for this adder.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                cboReferenceUnit.Focus()
                Return False
            End If
        End If

        If nudPriceSentToBuilder.Value <> nudFinalPrice.Value AndAlso String.IsNullOrWhiteSpace(txtPriceNote.Text) Then
            MessageBox.Show("Please enter a note explaining why the Builder price differs from the Final price.", "Note Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPriceNote.Focus()
            Return False
        End If

        ' Additional validation when key fields are changed
        If Not _isNew AndAlso _keyFieldsUnlocked Then
            Dim newPlanID = CInt(DirectCast(cboPlan.SelectedItem, ListItem).Value)
            Dim newElevationID = CInt(DirectCast(cboElevation.SelectedItem, ListItem).Value)
            Dim newOptionVal = CInt(DirectCast(cboOption.SelectedItem, ListItem).Value)
            Dim newOptionID As Integer? = If(newOptionVal = 0, Nothing, CType(newOptionVal, Integer?))
            Dim newProductTypeID = CInt(DirectCast(cboProductType.SelectedItem, ListItem).Value)

            Dim keyFieldsChanged = (newPlanID <> _originalPlanID) OrElse
                                   (newElevationID <> _originalElevationID) OrElse
                                   (newOptionID <> _originalOptionID) OrElse
                                   (newProductTypeID <> _originalProductTypeID)

            If keyFieldsChanged Then
                Dim confirmMsg = "You have changed one or more key fields (Plan, Elevation, Option, or Product Type)." & vbCrLf & vbCrLf &
                                 "This will UPDATE the existing record's classification." & vbCrLf & vbCrLf &
                                 "Are you sure you want to save these changes?"

                If MessageBox.Show(confirmMsg, "Confirm Key Field Changes", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) = DialogResult.No Then
                    Return False
                End If
            End If
        End If

        Return True
    End Function

    Private Sub SaveComponentPricing()
        Try
            Cursor = Cursors.WaitCursor

            _componentPricing.PlanID = CInt(DirectCast(cboPlan.SelectedItem, ListItem).Value)
            _componentPricing.ElevationID = CInt(DirectCast(cboElevation.SelectedItem, ListItem).Value)
            Dim optVal = CInt(DirectCast(cboOption.SelectedItem, ListItem).Value)
            _componentPricing.OptionID = If(optVal = 0, Nothing, CType(optVal, Integer?))
            _componentPricing.ProductTypeID = CInt(DirectCast(cboProductType.SelectedItem, ListItem).Value)
            _componentPricing.IsAdder = chkIsAdder.Checked

            If chkIsAdder.Checked Then
                Dim selectedRef = TryCast(cboReferenceUnit.SelectedItem, ListItem)
                If selectedRef IsNot Nothing AndAlso CInt(selectedRef.Value) > 0 Then
                    _componentPricing.BaseComponentPricingID = CInt(selectedRef.Value)
                Else
                    _componentPricing.BaseComponentPricingID = Nothing
                End If
            Else
                _componentPricing.BaseComponentPricingID = Nothing
            End If

            _componentPricing.MgmtSellPrice = nudMgmtSellPrice.Value
            _componentPricing.Cost = nudCost.Value
            _componentPricing.AppliedMargin = nudAppliedMargin.Value
            _componentPricing.FinalPrice = nudFinalPrice.Value
            _componentPricing.PriceSentToSales = nudFinalPrice.Value
            _componentPricing.PriceSentToBuilder = nudPriceSentToBuilder.Value
            _componentPricing.PriceNote = txtPriceNote.Text.Trim()
            _componentPricing.ModifiedBy = Environment.UserName
            _componentPricing.MarginSource = cboMarginSource.SelectedItem.ToString()

            If _isNew Then
                _componentPricing.ComponentPricingID = _dataAccess.InsertComponentPricing(_componentPricing)
            Else
                _dataAccess.UpdateComponentPricing(_componentPricing)
            End If

            ' Reload to get all joined fields
            Dim reloadedComponent = _dataAccess.GetComponentPricingByID(_componentPricing.ComponentPricingID)
            If reloadedComponent IsNot Nothing Then
                _componentPricing.PlanName = reloadedComponent.PlanName
                _componentPricing.ElevationName = reloadedComponent.ElevationName
                _componentPricing.OptionName = reloadedComponent.OptionName
                _componentPricing.ProductTypeName = reloadedComponent.ProductTypeName
                _componentPricing.MarginSource = reloadedComponent.MarginSource
                _componentPricing.CalculatedPrice = reloadedComponent.CalculatedPrice
                _componentPricing.BaseComponentDescription = reloadedComponent.BaseComponentDescription
                _componentPricing.BaseComponentCost = reloadedComponent.BaseComponentCost
            End If

            Me.DialogResult = DialogResult.OK
            Me.Close()
        Catch ex As Exception
            MessageBox.Show($"Error saving: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

#End Region

End Class