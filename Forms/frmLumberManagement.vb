Imports System.Windows.Forms
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Utilities
Imports System.Data

Public Class frmLumberManagement
    Private ReadOnly _dataAccess As New LumberDataAccess()
    Private _lumberCostsDataTable As DataTable

    Private Sub frmLumberManagement_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeControls()
        PopulateLumberTypes()
        PopulateCostEffectiveDates()
        ConfigureDataGridView()
        If lstCostEffective.Items.Count > 0 Then
            lstCostEffective.SelectedIndex = 0 ' Select the most recent date
        End If
    End Sub

    Private Sub InitializeControls()
        ' Set up ListBox display and value members
        lstLumberType.DisplayMember = "LumberTypeDesc"
        lstLumberType.ValueMember = "LumberTypeID"
        lstCostEffective.DisplayMember = "CosteffectiveDate"
        lstCostEffective.ValueMember = "CostEffectiveID"
    End Sub

    Private Sub ConfigureDataGridView()
        DataGridView1.AutoGenerateColumns = False
        DataGridView1.Columns.Clear()

        ' Hidden columns for IDs
        Dim colLumberCostID As New DataGridViewTextBoxColumn With {
            .DataPropertyName = "LumberCostID",
            .Name = "LumberCostID",
            .Visible = False
        }
        DataGridView1.Columns.Add(colLumberCostID)

        Dim colLumberTypeID As New DataGridViewTextBoxColumn With {
            .DataPropertyName = "LumberTypeID",
            .Name = "LumberTypeID",
            .Visible = False
        }
        DataGridView1.Columns.Add(colLumberTypeID)

        Dim colCostEffectiveDateID As New DataGridViewTextBoxColumn With {
            .DataPropertyName = "CostEffectiveDateID",
            .Name = "CostEffectiveDateID",
            .Visible = False
        }
        DataGridView1.Columns.Add(colCostEffectiveDateID)

        ' Visible columns
        Dim colLumberTypeDesc As New DataGridViewTextBoxColumn With {
            .DataPropertyName = "LumberTypeDesc",
            .Name = "LumberTypeDesc",
            .HeaderText = "Lumber Type",
            .ReadOnly = True,
            .Width = 200
        }
        DataGridView1.Columns.Add(colLumberTypeDesc)

        Dim colLumberCost As New DataGridViewTextBoxColumn With {
            .DataPropertyName = "LumberCost",
            .Name = "LumberCost",
            .HeaderText = "Cost ($)",
            .Width = 100
        }
        DataGridView1.Columns.Add(colLumberCost)

        DataGridView1.AllowUserToAddRows = False
        DataGridView1.AllowUserToDeleteRows = False
    End Sub

    Private Sub PopulateLumberTypes()
        Try
            Dim dt As DataTable = _dataAccess.GetAllLumberTypes()
            lstLumberType.DataSource = dt
        Catch ex As Exception
            MessageBox.Show("Error loading lumber types: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PopulateCostEffectiveDates()
        Try
            Dim dt As DataTable = LumberDataAccess.GetAllLumberCostEffective()
            lstCostEffective.DataSource = dt
        Catch ex As Exception
            MessageBox.Show("Error loading cost effective dates: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub PopulateLumberCosts(costEffectiveDateID As Integer)
        Try
            _lumberCostsDataTable = _dataAccess.GetLumberCostsByEffectiveDate(costEffectiveDateID)
            DataGridView1.DataSource = _lumberCostsDataTable
        Catch ex As Exception
            MessageBox.Show("Error loading lumber costs: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub lstCostEffective_SelectedIndexChanged(sender As Object, e As EventArgs) Handles lstCostEffective.SelectedIndexChanged
        If lstCostEffective.SelectedValue IsNot Nothing AndAlso lstCostEffective.SelectedValue IsNot DBNull.Value Then
            Dim costEffectiveDateID As Integer
            If Integer.TryParse(lstCostEffective.SelectedValue.ToString(), costEffectiveDateID) Then
                PopulateLumberCosts(costEffectiveDateID)
            End If
        Else
            DataGridView1.DataSource = Nothing
        End If
    End Sub

    Private Sub btnAddLumber_Click(sender As Object, e As EventArgs) Handles btnAddLumber.Click
        Dim lumberTypeDesc As String = InputBox("Enter new lumber type description:", "Add Lumber Type")
        If Not String.IsNullOrWhiteSpace(lumberTypeDesc) Then
            Try
                _dataAccess.CreateLumberType(lumberTypeDesc)
                PopulateLumberTypes()
                MessageBox.Show("Lumber type added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Error adding lumber type: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        ElseIf lumberTypeDesc <> "" Then ' InputBox returns empty string if cancelled
            MessageBox.Show("Lumber type description cannot be empty.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub btnAddCostEffective_Click(sender As Object, e As EventArgs) Handles btnAddCostEffective.Click
        Dim input As String = InputBox("Enter new effective date (yyyy-MM-dd):", "Add Cost Effective Date")
        Dim effectiveDate As Date
        If Date.TryParse(input, effectiveDate) Then
            Try
                _dataAccess.CreateLumberCostEffective(effectiveDate)
                PopulateCostEffectiveDates()
                If lstCostEffective.Items.Count > 0 Then
                    lstCostEffective.SelectedIndex = 0 ' Select the newest date
                End If
                MessageBox.Show("Cost effective date added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Catch ex As Exception
                MessageBox.Show("Error adding cost effective date: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        ElseIf input <> "" Then
            MessageBox.Show("Invalid date format. Use yyyy-MM-dd.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
        End If
    End Sub

    Private Sub btnSaveLumberCosts_Click(sender As Object, e As EventArgs) Handles btnSaveLumberCosts.Click
        If _lumberCostsDataTable Is Nothing Then
            MessageBox.Show("No lumber costs loaded to save.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Exit Sub
        End If

        Try
            For Each row As DataRow In _lumberCostsDataTable.Rows
                If row.RowState = DataRowState.Modified Then
                    Dim lumberCostID As Integer = CInt(row("LumberCostID"))
                    Dim lumberCost As Decimal
                    If Decimal.TryParse(row("LumberCost").ToString(), lumberCost) Then
                        _dataAccess.UpdateLumberCost(lumberCostID, lumberCost)
                    Else
                        MessageBox.Show("Invalid cost value for " & row("LumberTypeDesc").ToString() & ".", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                        Exit Sub
                    End If
                End If
            Next
            _lumberCostsDataTable.AcceptChanges()
            MessageBox.Show("Lumber costs saved successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
        Catch ex As Exception
            MessageBox.Show("Error saving lumber costs: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub btnExitForm_Click(sender As Object, e As EventArgs) Handles btnExitForm.Click
        Try
            Dim tagValue As String = Me.Tag?.ToString()
            If String.IsNullOrEmpty(tagValue) Then
                Throw New Exception("Tab tag not found.")
            End If
            RemoveTabFromTabControl(tagValue)
            StatusLogger.Add($"Closed tab {tagValue} at {DateTime.Now:HH:mm:ss}")
        Catch ex As Exception
            StatusLogger.Add($"Error closing tab: {ex.Message} at {DateTime.Now:HH:mm:ss}")
            MessageBox.Show($"Error closing tab: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

End Class