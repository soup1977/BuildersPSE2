' =====================================================
' frmMaterialCategoryManagement.vb
' Manage Material Categories (STUDS, OSB, EWP, etc.)
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Partial Public Class frmMaterialCategoryManagement
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _categories As List(Of PLMaterialCategory)

#End Region

#Region "Constructor"

    Public Sub New()
        _dataAccess = New PriceLockDataAccess()
        InitializeComponent()
    End Sub

    Public Sub New(dataAccess As PriceLockDataAccess)
        _dataAccess = dataAccess
        InitializeComponent()
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmMaterialCategoryManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()
        LoadCategories()
        ClearEditFields()
    End Sub

    Private Sub dgvCategories_SelectionChanged(sender As Object, e As EventArgs) Handles dgvCategories.SelectionChanged
        LoadSelectedCategory()
    End Sub

    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        AddCategory()
    End Sub

    Private Sub btnUpdate_Click(sender As Object, e As EventArgs) Handles btnUpdate.Click
        UpdateCategory()
    End Sub

    Private Sub btnMoveUp_Click(sender As Object, e As EventArgs) Handles btnMoveUp.Click
        MoveCategory(-1)
    End Sub

    Private Sub btnMoveDown_Click(sender As Object, e As EventArgs) Handles btnMoveDown.Click
        MoveCategory(1)
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

#End Region

#Region "Grid Setup"

    Private Sub SetupGrid()
        dgvCategories.Columns.Clear()
        dgvCategories.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "MaterialCategoryID", .DataPropertyName = "MaterialCategoryID", .Visible = False})
        dgvCategories.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "DisplayOrder", .HeaderText = "#", .DataPropertyName = "DisplayOrder", .Width = 40})
        dgvCategories.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "CategoryName", .HeaderText = "Category Name", .DataPropertyName = "CategoryName", .Width = 150})
        dgvCategories.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "CategoryCode", .HeaderText = "Code", .DataPropertyName = "CategoryCode", .Width = 60})
        dgvCategories.Columns.Add(New DataGridViewCheckBoxColumn() With {.Name = "IsActive", .HeaderText = "Active", .DataPropertyName = "IsActive", .Width = 50})
    End Sub

#End Region

#Region "Data Operations"

    Private Sub LoadCategories()
        Try
            _categories = _dataAccess.GetMaterialCategories()
            dgvCategories.DataSource = Nothing
            dgvCategories.DataSource = _categories
        Catch ex As Exception
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadSelectedCategory()
        If dgvCategories.SelectedRows.Count = 0 OrElse dgvCategories.SelectedRows(0).Index >= _categories.Count Then
            ClearEditFields()
            Return
        End If

        Dim category = _categories(dgvCategories.SelectedRows(0).Index)
        txtCategoryName.Text = category.CategoryName
        txtCategoryCode.Text = category.CategoryCode
        nudDisplayOrder.Value = category.DisplayOrder
        chkIsActive.Checked = category.IsActive
    End Sub

    Private Sub ClearEditFields()
        txtCategoryName.Text = ""
        txtCategoryCode.Text = ""
        nudDisplayOrder.Value = If(_categories?.Count > 0, _categories.Max(Function(c) c.DisplayOrder) + 1, 1)
        chkIsActive.Checked = True
        lblStatus.Text = ""
    End Sub

    Private Sub AddCategory()
        If String.IsNullOrWhiteSpace(txtCategoryName.Text) Then
            MessageBox.Show("Please enter a category name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCategoryName.Focus()
            Return
        End If

        Try
            Dim category As New PLMaterialCategory() With {
                .CategoryName = txtCategoryName.Text.Trim().ToUpper(),
                .CategoryCode = txtCategoryCode.Text.Trim().ToUpper(),
                .DisplayOrder = CInt(nudDisplayOrder.Value),
                .IsActive = chkIsActive.Checked
            }
            category.MaterialCategoryID = _dataAccess.InsertMaterialCategory(category)

            LoadCategories()
            SelectCategoryInGrid(category.MaterialCategoryID)
            lblStatus.Text = "Category added."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error adding category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub UpdateCategory()
        If dgvCategories.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a category to update.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If String.IsNullOrWhiteSpace(txtCategoryName.Text) Then
            MessageBox.Show("Please enter a category name.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            txtCategoryName.Focus()
            Return
        End If

        Try
            Dim category = _categories(dgvCategories.SelectedRows(0).Index)
            category.CategoryName = txtCategoryName.Text.Trim().ToUpper()
            category.CategoryCode = txtCategoryCode.Text.Trim().ToUpper()
            category.DisplayOrder = CInt(nudDisplayOrder.Value)
            category.IsActive = chkIsActive.Checked

            _dataAccess.UpdateMaterialCategory(category)

            LoadCategories()
            SelectCategoryInGrid(category.MaterialCategoryID)
            lblStatus.Text = "Category updated."
            lblStatus.ForeColor = Color.Green
        Catch ex As Exception
            MessageBox.Show($"Error updating category: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub MoveCategory(direction As Integer)
        If dgvCategories.SelectedRows.Count = 0 Then Return

        Dim index = dgvCategories.SelectedRows(0).Index
        Dim newIndex = index + direction

        If newIndex < 0 OrElse newIndex >= _categories.Count Then Return

        Try
            ' Swap display orders
            Dim currentCategory = _categories(index)
            Dim swapCategory = _categories(newIndex)

            Dim tempOrder = currentCategory.DisplayOrder
            currentCategory.DisplayOrder = swapCategory.DisplayOrder
            swapCategory.DisplayOrder = tempOrder

            _dataAccess.UpdateMaterialCategory(currentCategory)
            _dataAccess.UpdateMaterialCategory(swapCategory)

            LoadCategories()
            SelectCategoryInGrid(currentCategory.MaterialCategoryID)
        Catch ex As Exception
            MessageBox.Show($"Error reordering: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub SelectCategoryInGrid(categoryID As Integer)
        For i = 0 To dgvCategories.Rows.Count - 1
            If CInt(dgvCategories.Rows(i).Cells("MaterialCategoryID").Value) = categoryID Then
                dgvCategories.Rows(i).Selected = True
                dgvCategories.FirstDisplayedScrollingRowIndex = i
                Exit For
            End If
        Next
    End Sub

#End Region

End Class