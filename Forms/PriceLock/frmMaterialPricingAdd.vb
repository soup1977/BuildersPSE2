' =====================================================
' frmMaterialPricingAdd.vb
' Dialog for adding new material pricing records
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Linq
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Public Class frmMaterialPricingAdd

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _priceLock As PLPriceLock
    Private _existingCategoryIDs As HashSet(Of Integer)

#End Region

#Region "Constructor"

    Public Sub New(priceLock As PLPriceLock, dataAccess As PriceLockDataAccess)
        _priceLock = priceLock
        _dataAccess = dataAccess

        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmMaterialPricingAdd_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadCategories()

        ' Wire up selection change event
        AddHandler cboCategory.SelectedIndexChanged, AddressOf cboCategory_SelectedIndexChanged
    End Sub

    Private Sub nudPriceSentToBuilder_ValueChanged(sender As Object, e As EventArgs) Handles nudPriceSentToBuilder.ValueChanged
        lblPriceDiffWarning.Visible = (nudPriceSentToBuilder.Value <> nudCalculatedPrice.Value)
    End Sub

    Private Sub btnAddCategory_Click(sender As Object, e As EventArgs) Handles btnAddCategory.Click
        Dim categoryName = InputBox("Enter new Material Category name:", "Add Category")
        If String.IsNullOrWhiteSpace(categoryName) Then Return

        Try
            Dim newCat As New PLMaterialCategory() With {
                .CategoryName = categoryName.Trim().ToUpper(),
                .IsActive = True,
                .DisplayOrder = 999
            }
            newCat.MaterialCategoryID = _dataAccess.InsertMaterialCategory(newCat)

            LoadCategories()
            SelectComboItem(cboCategory, newCat.MaterialCategoryID)

            MessageBox.Show($"Category '{categoryName.ToUpper()}' added.", "Category Added", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"Error adding category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If ValidateInput() Then SaveMaterialPricing()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadCategories()
        ' Get existing material pricing for this lock to exclude already-added categories
        Dim existingPricing = _dataAccess.GetMaterialPricingByLock(_priceLock.PriceLockID)
        _existingCategoryIDs = New HashSet(Of Integer)(existingPricing.Select(Function(m) m.MaterialCategoryID))

        ' Load all categories, filtering out ones already in this price lock
        cboCategory.Items.Clear()
        For Each cat In _dataAccess.GetMaterialCategories()
            If Not _existingCategoryIDs.Contains(cat.MaterialCategoryID) Then
                cboCategory.Items.Add(New ListItem(cat.CategoryName, cat.MaterialCategoryID))
            End If
        Next

        If cboCategory.Items.Count > 0 Then
            cboCategory.SelectedIndex = 0
        End If
    End Sub

    Private Sub cboCategory_SelectedIndexChanged(sender As Object, e As EventArgs)
        LoadPreviousApprovedPrice()
    End Sub
    Private Sub LoadPreviousApprovedPrice()
        If cboCategory.SelectedItem Is Nothing Then
            lblPreviousApproved.Text = "--"
            lblPreviousApproved.ForeColor = Color.Gray
            Return
        End If

        Dim selectedCategory = DirectCast(cboCategory.SelectedItem, ListItem)
        Dim categoryID = CInt(selectedCategory.Value)

        Try
            ' Get previous lock for this subdivision
            Dim previousLock = _dataAccess.GetPreviousPriceLock(_priceLock.SubdivisionID, _priceLock.PriceLockDate)
            If previousLock Is Nothing Then
                lblPreviousApproved.Text = "(First price lock)"
                lblPreviousApproved.ForeColor = Color.Blue
                Return
            End If

            ' Get previous approved price (PriceSentToSales or PriceSentToBuilder)
            Dim previousPrice = _dataAccess.GetPreviousMaterialPrice(
            _priceLock.SubdivisionID,
            categoryID,
            _priceLock.PriceLockDate)

            If previousPrice.HasValue Then
                lblPreviousApproved.Text = previousPrice.Value.ToString("C2")
                lblPreviousApproved.ForeColor = Color.Green

                ' OPTIONAL: Auto-populate calculated price with previous price as starting point
                If nudCalculatedPrice.Value = 0 Then
                    nudCalculatedPrice.Value = previousPrice.Value
                End If
            Else
                lblPreviousApproved.Text = "(Not in previous lock)"
                lblPreviousApproved.ForeColor = Color.Orange
            End If

        Catch ex As Exception
            lblPreviousApproved.Text = "(Error loading)"
            lblPreviousApproved.ForeColor = Color.Red
            System.Diagnostics.Debug.WriteLine($"Error loading previous price: {ex.Message}")
        End Try
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

#Region "Validation and Save"

    Private Function ValidateInput() As Boolean
        If cboCategory.SelectedItem Is Nothing Then
            MessageBox.Show("Please select a material category.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            cboCategory.Focus()
            Return False
        End If

        ' Require note if prices differ
        If nudPriceSentToBuilder.Value <> nudCalculatedPrice.Value AndAlso String.IsNullOrWhiteSpace(txtPriceNote.Text) Then
            MessageBox.Show("Please enter a note explaining why the Builder price differs from the Calculated price.",
                "Note Required", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtPriceNote.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub SaveMaterialPricing()
        Try
            Cursor = Cursors.WaitCursor

            Dim selectedCategory = DirectCast(cboCategory.SelectedItem, ListItem)

            Dim pricing As New PLMaterialPricing() With {
                .PriceLockID = _priceLock.PriceLockID,
                .MaterialCategoryID = CInt(selectedCategory.Value),
                .RandomLengthsDate = dtpRandomLengthsDate.Value.Date,
                .RandomLengthsPrice = nudRandomLengthsPrice.Value,
                .CalculatedPrice = nudCalculatedPrice.Value,
                .PriceSentToSales = nudCalculatedPrice.Value,
                .PriceSentToBuilder = nudPriceSentToBuilder.Value,
                .PriceNote = txtPriceNote.Text.Trim(),
                .ModifiedBy = Environment.UserName
            }

            _dataAccess.InsertMaterialPricing(pricing)

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