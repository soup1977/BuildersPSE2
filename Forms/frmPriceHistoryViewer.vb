Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Utilities

Public Class frmPriceHistoryViewer

    Private _projectID As Integer
    Private _projectName As String
    Private _historyRecords As List(Of PriceHistoryModel)
    Private _selectedHistoryID As Integer = -1

    Public Sub New(projectID As Integer, projectName As String)
        InitializeComponent()
        _projectID = projectID
        _projectName = projectName
    End Sub

    Private Sub frmPriceHistoryViewer_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = $"Price History Viewer - {_projectName}"
        LoadVersionFilter()
        LoadHistoryRecords()
        SetupGridColumns()
    End Sub

#Region "Data Loading"

    Private Sub LoadVersionFilter()
        Try
            cboVersionFilter.Items.Clear()
            cboVersionFilter.Items.Add(New VersionFilterItem(-1, "(All Versions)"))

            Dim versions = ProjVersionDataAccess.GetProjectVersions(_projectID)
            For Each v In versions
                cboVersionFilter.Items.Add(New VersionFilterItem(v.VersionID, v.VersionName))
            Next

            cboVersionFilter.SelectedIndex = 0
        Catch ex As Exception
            MessageBox.Show($"Error loading versions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadHistoryRecords()
        Try
            Dim selectedVersionID As Integer = -1
            If cboVersionFilter.SelectedItem IsNot Nothing Then
                selectedVersionID = CType(cboVersionFilter.SelectedItem, VersionFilterItem).VersionID
            End If

            _historyRecords = PriceHistoryDataAccess.GetPriceHistoryByProject(_projectID)

            ' Apply version filter
            If selectedVersionID > 0 Then
                _historyRecords = _historyRecords.Where(Function(h) h.VersionID = selectedVersionID).ToList()
            End If

            ' Bind to grid
            dgvHistory.DataSource = Nothing
            dgvHistory.DataSource = _historyRecords

            lblStatus.Text = $"{_historyRecords.Count} snapshot(s) found"

            ' Clear detail grids
            ClearDetailGrids()

            ' Select first row if available
            If dgvHistory.Rows.Count > 0 Then
                dgvHistory.Rows(0).Selected = True
                LoadSelectedHistoryDetail()
            End If

        Catch ex As Exception
            MessageBox.Show($"Error loading history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadSelectedHistoryDetail()
        If dgvHistory.SelectedRows.Count = 0 Then
            ClearDetailGrids()
            Return
        End If

        Try
            Dim selectedRow = dgvHistory.SelectedRows(0)
            _selectedHistoryID = CInt(selectedRow.Cells("PriceHistoryID").Value)

            ' Load summary
            LoadSummaryGrid(_selectedHistoryID)

            ' Load buildings
            LoadBuildingsGrid(_selectedHistoryID)

        Catch ex As Exception
            MessageBox.Show($"Error loading detail: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Sub LoadSummaryGrid(historyID As Integer)
        Dim history = _historyRecords.FirstOrDefault(Function(h) h.PriceHistoryID = historyID)
        If history Is Nothing Then Return

        Dim summaryData As New DataTable()
        summaryData.Columns.Add("Category", GetType(String))
        summaryData.Columns.Add("Value", GetType(String))

        ' Project Info
        summaryData.Rows.Add("--- Project Info ---", "")
        summaryData.Rows.Add("Version", history.VersionName)
        summaryData.Rows.Add("Report Type", history.ReportType)
        summaryData.Rows.Add("Created", history.CreatedDate.ToString("g"))
        summaryData.Rows.Add("Created By", history.CreatedBy)

        ' Rollup Values
        summaryData.Rows.Add("--- Rollup Values ---", "")
        summaryData.Rows.Add("Extended Sell", history.ExtendedSell.ToString("C2"))
        summaryData.Rows.Add("Extended Cost", history.ExtendedCost.ToString("C2"))
        summaryData.Rows.Add("Extended Delivery", history.ExtendedDelivery.ToString("C2"))
        summaryData.Rows.Add("Extended SQFT", history.ExtendedSqft.ToString("N0"))
        summaryData.Rows.Add("Extended BDFT", history.ExtendedBdft.ToString("N0"))
        summaryData.Rows.Add("Margin", history.Margin.ToString("P2"))
        summaryData.Rows.Add("Margin w/ Delivery", history.MarginWithDelivery.ToString("P2"))
        summaryData.Rows.Add("Price/BDFT", history.PricePerBdft.ToString("C4"))
        summaryData.Rows.Add("Price/SQFT", history.PricePerSqft.ToString("C2"))

        ' Futures Adder
        summaryData.Rows.Add("--- Futures Adder ---", "")
        summaryData.Rows.Add("Futures Adder/MBF", If(history.FuturesAdderAmt.HasValue, history.FuturesAdderAmt.Value.ToString("C2"), "—"))
        summaryData.Rows.Add("Futures Adder Total", If(history.FuturesAdderProjTotal.HasValue, history.FuturesAdderProjTotal.Value.ToString("C2"), "—"))
        summaryData.Rows.Add("Futures Contract", If(String.IsNullOrEmpty(history.FuturesContractMonth), "—", history.FuturesContractMonth))
        summaryData.Rows.Add("Futures Settle Price", If(history.FuturesPriorSettle.HasValue, history.FuturesPriorSettle.Value.ToString("N2"), "—"))

        ' Active Lumber
        summaryData.Rows.Add("--- Active Lumber ---", "")
        summaryData.Rows.Add("Floor SPF#2", history.ActiveFloorSPFNo2.ToString("N2"))
        summaryData.Rows.Add("Roof SPF#2", history.ActiveRoofSPFNo2.ToString("N2"))
        summaryData.Rows.Add("Cost Effective Date", If(history.ActiveCostEffectiveDate.HasValue, history.ActiveCostEffectiveDate.Value.ToString("d"), "—"))

        ' Building Summary
        summaryData.Rows.Add("--- Structure ---", "")
        summaryData.Rows.Add("Buildings", history.TotalBuildingCount.ToString())
        summaryData.Rows.Add("Total Bldg Qty", history.TotalBldgQty.ToString())
        summaryData.Rows.Add("Total Levels", history.TotalLevelCount.ToString())
        summaryData.Rows.Add("Total Units", history.TotalActualUnitCount.ToString())

        dgvSummary.DataSource = summaryData

        ' Format section headers
        For Each row As DataGridViewRow In dgvSummary.Rows
            Dim category = row.Cells(0).Value?.ToString()
            If category IsNot Nothing AndAlso category.StartsWith("---") Then
                row.DefaultCellStyle.BackColor = Color.LightGray
                row.DefaultCellStyle.Font = New Font(dgvSummary.Font, FontStyle.Bold)
            End If
        Next
    End Sub

    Private Sub LoadBuildingsGrid(historyID As Integer)
        Dim buildings = PriceHistoryDataAccess.GetBuildingsByHistoryID(historyID)

        dgvBuildings.DataSource = Nothing
        dgvBuildings.DataSource = buildings

        ' Clear levels
        dgvLevels.DataSource = Nothing

        ' Select first building if available
        If dgvBuildings.Rows.Count > 0 Then
            dgvBuildings.Rows(0).Selected = True
            LoadLevelsGrid()
        End If
    End Sub

    Private Sub LoadLevelsGrid()
        If dgvBuildings.SelectedRows.Count = 0 Then
            dgvLevels.DataSource = Nothing
            Return
        End If

        Try
            Dim buildingHistoryID = CInt(dgvBuildings.SelectedRows(0).Cells("PriceHistoryBuildingID").Value)
            Dim levels = PriceHistoryDataAccess.GetLevelsByBuildingHistoryID(buildingHistoryID)
            dgvLevels.DataSource = Nothing
            dgvLevels.DataSource = levels
        Catch ex As Exception
            dgvLevels.DataSource = Nothing
        End Try
    End Sub

    Private Sub ClearDetailGrids()
        dgvSummary.DataSource = Nothing
        dgvBuildings.DataSource = Nothing
        dgvLevels.DataSource = Nothing
        _selectedHistoryID = -1
    End Sub

#End Region

#Region "Grid Setup"

    Private Sub SetupGridColumns()
        ' History grid columns
        dgvHistory.AutoGenerateColumns = False
        dgvHistory.Columns.Clear()

        dgvHistory.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "PriceHistoryID",
        .DataPropertyName = "PriceHistoryID",
        .HeaderText = "ID",
        .Width = 50,
        .Visible = False
    })
        dgvHistory.Columns.Add(New DataGridViewTextBoxColumn() With {
        .DataPropertyName = "CreatedDate",
        .HeaderText = "Date",
        .Width = 130,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "g"}
    })
        dgvHistory.Columns.Add(New DataGridViewTextBoxColumn() With {
        .DataPropertyName = "VersionName",
        .HeaderText = "Version",
        .Width = 80
    })
        dgvHistory.Columns.Add(New DataGridViewTextBoxColumn() With {
        .DataPropertyName = "CreatedBy",
        .HeaderText = "User",
        .Width = 80
    })
        dgvHistory.Columns.Add(New DataGridViewTextBoxColumn() With {
        .DataPropertyName = "ExtendedSell",
        .HeaderText = "Sell",
        .Width = 90,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C0", .Alignment = DataGridViewContentAlignment.MiddleRight}
    })
        dgvHistory.Columns.Add(New DataGridViewTextBoxColumn() With {
        .DataPropertyName = "Margin",
        .HeaderText = "Margin",
        .Width = 70,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "P1", .Alignment = DataGridViewContentAlignment.MiddleRight}
    })

        ' Buildings grid columns
        dgvBuildings.AutoGenerateColumns = False
        dgvBuildings.Columns.Clear()

        dgvBuildings.Columns.Add(New DataGridViewTextBoxColumn() With {
        .Name = "PriceHistoryBuildingID",
        .DataPropertyName = "PriceHistoryBuildingID",
        .Visible = False
    })
        dgvBuildings.Columns.Add(New DataGridViewTextBoxColumn() With {
        .DataPropertyName = "BuildingName",
        .HeaderText = "Building",
        .Width = 150
    })
        dgvBuildings.Columns.Add(New DataGridViewTextBoxColumn() With {
        .DataPropertyName = "BldgQty",
        .HeaderText = "Qty",
        .Width = 50,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Alignment = DataGridViewContentAlignment.MiddleCenter}
    })
        dgvBuildings.Columns.Add(New DataGridViewTextBoxColumn() With {
        .DataPropertyName = "BaseSell",
        .HeaderText = "Base Sell",
        .Width = 100,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2", .Alignment = DataGridViewContentAlignment.MiddleRight}
    })
        dgvBuildings.Columns.Add(New DataGridViewTextBoxColumn() With {
        .DataPropertyName = "ExtendedSell",
        .HeaderText = "Ext Sell",
        .Width = 100,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2", .Alignment = DataGridViewContentAlignment.MiddleRight}
    })
        dgvBuildings.Columns.Add(New DataGridViewTextBoxColumn() With {
        .DataPropertyName = "Margin",
        .HeaderText = "Margin",
        .Width = 70,
        .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "P1", .Alignment = DataGridViewContentAlignment.MiddleRight}
    })

        ' Levels grid columns
        dgvLevels.AutoGenerateColumns = False
        dgvLevels.Columns.Clear()

        dgvLevels.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "LevelName",
            .HeaderText = "Level",
            .Width = 120
        })
        dgvLevels.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "ProductTypeName",
            .HeaderText = "Type",
            .Width = 60
        })
        dgvLevels.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "OverallBdft",
            .HeaderText = "BDFT",
            .Width = 70,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "N0", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })
        dgvLevels.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "OverallCost",
            .HeaderText = "Cost",
            .Width = 90,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })
        dgvLevels.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "OverallPrice",
            .HeaderText = "Price",
            .Width = 90,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })
        dgvLevels.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "Margin",
            .HeaderText = "Margin",
            .Width = 70,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "P1", .Alignment = DataGridViewContentAlignment.MiddleRight}
        })
        dgvLevels.Columns.Add(New DataGridViewTextBoxColumn() With {
            .DataPropertyName = "ActualUnitCount",
            .HeaderText = "Units",
            .Width = 50,
            .DefaultCellStyle = New DataGridViewCellStyle() With {.Alignment = DataGridViewContentAlignment.MiddleCenter}
        })
    End Sub

#End Region

#Region "Event Handlers"

    Private Sub dgvHistory_SelectionChanged(sender As Object, e As EventArgs) Handles dgvHistory.SelectionChanged
        LoadSelectedHistoryDetail()
    End Sub

    Private Sub dgvBuildings_SelectionChanged(sender As Object, e As EventArgs) Handles dgvBuildings.SelectionChanged
        LoadLevelsGrid()
    End Sub

    Private Sub cboVersionFilter_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cboVersionFilter.SelectedIndexChanged
        LoadHistoryRecords()
    End Sub

    Private Sub btnRefresh_Click(sender As Object, e As EventArgs) Handles btnRefresh.Click
        LoadHistoryRecords()
    End Sub

    Private Sub btnDelete_Click(sender As Object, e As EventArgs) Handles btnDelete.Click
        If _selectedHistoryID <= 0 Then
            MessageBox.Show("Please select a history record to delete.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        If Not CurrentUser.IsAdmin Then
            MessageBox.Show("Admin access required to delete history records.", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Dim result = MessageBox.Show("Are you sure you want to delete this price history snapshot?" & vbCrLf & vbCrLf &
                                      "This action cannot be undone.",
                                      "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)

        If result = DialogResult.Yes Then
            Try
                PriceHistoryDataAccess.DeletePriceHistory(_selectedHistoryID)
                MessageBox.Show("Price history deleted.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information)
                LoadHistoryRecords()
            Catch ex As Exception
                MessageBox.Show($"Error deleting history: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End If
    End Sub

    Private Sub btnCompare_Click(sender As Object, e As EventArgs) Handles btnCompare.Click
        ' Require at least 2 snapshots to compare
        If _historyRecords Is Nothing OrElse _historyRecords.Count < 2 Then
            MessageBox.Show("At least 2 price history snapshots are required for comparison.",
                        "Cannot Compare", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Return
        End If

        ' Open a dialog to select the two snapshots to compare
        Using dlg As New frmSelectSnapshotsToCompare(_historyRecords)
            If dlg.ShowDialog() = DialogResult.OK Then
                Dim snapshot1 = dlg.SelectedSnapshot1
                Dim snapshot2 = dlg.SelectedSnapshot2

                If snapshot1 IsNot Nothing AndAlso snapshot2 IsNot Nothing Then
                    Using compareForm As New frmPriceHistoryCompare(snapshot1, snapshot2)
                        compareForm.ShowDialog()
                    End Using
                End If
            End If
        End Using
    End Sub

    Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
        MessageBox.Show("Export feature coming soon - will export selected snapshot to Excel.",
                        "Coming Soon", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
        Me.Close()
    End Sub

#End Region

#Region "Helper Classes"

    Private Class VersionFilterItem
        Public Property VersionID As Integer
        Public Property VersionName As String

        Public Sub New(id As Integer, name As String)
            VersionID = id
            VersionName = name
        End Sub

        Public Overrides Function ToString() As String
            Return VersionName
        End Function
    End Class

#End Region

End Class