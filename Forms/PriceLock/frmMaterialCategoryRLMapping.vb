' =====================================================
' frmMaterialCategoryRLMapping.vb
' Manage Material Category → RL Pricing Type mappings
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.Linq
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Public Class frmMaterialCategoryRLMapping

#Region "Fields"
    Private _dataAccess As PriceLockDataAccess
    Private _categories As List(Of PLMaterialCategory)
    Private _pricingTypes As List(Of PLRandomLengthsPricingType)
    Private _mappings As List(Of PLMaterialCategoryRLMapping)
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

#Region "Form Load"
    Private Sub frmMaterialCategoryRLMapping_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupGrid()
        LoadCategories()
        LoadPricingTypes()
        LoadMappings()
    End Sub

    Private Sub SetupGrid()
        dgvMappings.AutoGenerateColumns = False
        dgvMappings.Columns.Clear()
        dgvMappings.SelectionMode = DataGridViewSelectionMode.FullRowSelect
        dgvMappings.MultiSelect = False
        dgvMappings.ReadOnly = True
        dgvMappings.AllowUserToAddRows = False

        dgvMappings.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "MappingID",
            .DataPropertyName = "MappingID",
            .HeaderText = "ID",
            .Width = 50,
            .Visible = False
        })

        dgvMappings.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "CategoryName",
            .DataPropertyName = "CategoryName",
            .HeaderText = "Material Category",
            .Width = 150
        })

        dgvMappings.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "RLGroup",
            .HeaderText = "RL Group",
            .Width = 120
        })

        dgvMappings.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "RLTypeName",
            .DataPropertyName = "RLTypeName",
            .HeaderText = "RL Pricing Type",
            .Width = 300
        })

        dgvMappings.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "IsPrimary",
            .HeaderText = "Primary",
            .Width = 60
        })

        dgvMappings.Columns.Add(New DataGridViewTextBoxColumn() With {
            .Name = "IsActive",
            .HeaderText = "Active",
            .Width = 60
        })
    End Sub

    Private Sub LoadCategories()
        Try
            _categories = _dataAccess.GetMaterialCategories()

            cboMaterialCategory.Items.Clear()
            For Each cat In _categories.Where(Function(c) c.IsActive).OrderBy(Function(c) c.DisplayOrder)
                cboMaterialCategory.Items.Add(New ListItem(cat.CategoryName, cat.MaterialCategoryID))
            Next

            If cboMaterialCategory.Items.Count > 0 Then
                cboMaterialCategory.SelectedIndex = 0
            End If
        Catch ex As Exception
            MessageBox.Show($"Error loading categories: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadPricingTypes()
        Try
            _pricingTypes = _dataAccess.GetRLPricingTypes()

            cboRLPricingType.Items.Clear()
            For Each pType In _pricingTypes.Where(Function(t) t.IsActive).OrderBy(Function(t) t.CategoryGroup).ThenBy(Function(t) t.DisplayOrder)
                Dim displayText = $"{pType.CategoryGroup} - {pType.TypeName}"
                cboRLPricingType.Items.Add(New ListItem(displayText, pType.RLPricingTypeID))
            Next

            If cboRLPricingType.Items.Count > 0 Then
                cboRLPricingType.SelectedIndex = 0
            End If
        Catch ex As Exception
            MessageBox.Show($"Error loading pricing types: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadMappings()
        Try
            _mappings = _dataAccess.GetMaterialCategoryRLMappings()

            dgvMappings.Rows.Clear()
            For Each mapping In _mappings.OrderBy(Function(m) m.CategoryName).ThenByDescending(Function(m) m.IsPrimary)
                Dim pType = _pricingTypes.FirstOrDefault(Function(t) t.RLPricingTypeID = mapping.RLPricingTypeID)

                dgvMappings.Rows.Add(
                    mapping.MappingID,
                    mapping.CategoryName,
                    If(pType IsNot Nothing, pType.CategoryGroup, "--"),
                    mapping.RLTypeName,
                    If(mapping.IsPrimary, "Yes", "No"),
                    If(mapping.IsActive, "Yes", "No")
                )

                ' Color primary mappings
                If mapping.IsPrimary Then
                    Dim row = dgvMappings.Rows(dgvMappings.Rows.Count - 1)
                    row.DefaultCellStyle.BackColor = Color.LightGreen
                    row.DefaultCellStyle.Font = New Font(dgvMappings.Font, FontStyle.Bold)
                End If
            Next
        Catch ex As Exception
            MessageBox.Show($"Error loading mappings: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
#End Region

#Region "Button Events"
    Private Sub btnAdd_Click(sender As Object, e As EventArgs) Handles btnAdd.Click
        If cboMaterialCategory.SelectedItem Is Nothing OrElse cboRLPricingType.SelectedItem Is Nothing Then
            MessageBox.Show("Please select both a Material Category and RL Pricing Type.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim categoryID = CInt(DirectCast(cboMaterialCategory.SelectedItem, ListItem).Value)
        Dim rlTypeID = CInt(DirectCast(cboRLPricingType.SelectedItem, ListItem).Value)
        Dim isPrimary = chkSetAsPrimary.Checked

        ' Check if mapping already exists
        If _mappings.Any(Function(m) m.MaterialCategoryID = categoryID AndAlso m.RLPricingTypeID = rlTypeID) Then
            MessageBox.Show("This mapping already exists.", "Duplicate", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        ' Check if primary already exists for this category
        If isPrimary Then
            Dim existingPrimary = _mappings.FirstOrDefault(Function(m) m.MaterialCategoryID = categoryID AndAlso m.IsPrimary)
            If existingPrimary IsNot Nothing Then
                Dim result = MessageBox.Show(
                    $"Material Category already has a primary RL mapping ({existingPrimary.RLTypeName})." & vbCrLf & vbCrLf &
                    "Do you want to replace it with this one?",
                    "Primary Exists",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question)

                If result = DialogResult.Yes Then
                    ' Update existing primary to non-primary
                    existingPrimary.IsPrimary = False
                    _dataAccess.UpdateMaterialCategoryRLMapping(existingPrimary)
                Else
                    Return
                End If
            End If
        End If

        ' Create new mapping
        Try
            Dim mapping As New PLMaterialCategoryRLMapping() With {
                .MaterialCategoryID = categoryID,
                .RLPricingTypeID = rlTypeID,
                .WeightFactor = 1D,
                .IsPrimary = isPrimary,
                .IsActive = True
            }

            mapping.MappingID = _dataAccess.InsertMaterialCategoryRLMapping(mapping)
            LoadMappings()

            MessageBox.Show("Mapping added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show($"Error adding mapping: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If dgvMappings.SelectedRows.Count = 0 Then
            MessageBox.Show("Please select a mapping to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim mappingID = CInt(dgvMappings.SelectedRows(0).Cells("MappingID").Value)
        Dim mapping = _mappings.FirstOrDefault(Function(m) m.MappingID = mappingID)

        If mapping IsNot Nothing Then
            If mapping.IsPrimary Then
                MessageBox.Show("Cannot delete primary mapping. Please set another mapping as primary first.", "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                Return
            End If

            Dim result = MessageBox.Show(
                $"Delete mapping for {mapping.CategoryName} → {mapping.RLTypeName}?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question)

            If result = DialogResult.Yes Then
                Try
                    _dataAccess.DeleteMaterialCategoryRLMapping(mappingID)
                    LoadMappings()
                    MessageBox.Show("Mapping deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Catch ex As Exception
                    MessageBox.Show($"Error deleting mapping: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End If
        End If
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub
#End Region

End Class