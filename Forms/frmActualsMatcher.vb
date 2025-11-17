Option Strict On
Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Utilities
Imports BuildersPSE2.DataAccess
Imports Microsoft.VisualBasic.FileIO

Public Class frmActualsMatcher
    Private ReadOnly versionID As Integer
    Private ReadOnly stageType As Integer ' 1=Design, 2=Invoice
    Private miTekData As DataTable
    Private projectLevels As DataTable
    Private matches As New Dictionary(Of Integer, Integer)() ' MiTekRowIndex → LevelID

    Public Sub New(versionID As Integer, stageType As Integer, csvPath As String)
        InitializeComponent()
        Me.versionID = versionID
        Me.stageType = stageType
        LoadMiTekData(csvPath)
        LoadProjectLevels()
        Me.Text = If(stageType = 1, "Match MiTek Design Data", "Match BisTrack Invoice Data")
    End Sub

    Private Sub LoadMiTekData(csvPath As String)
        miTekData = New DataTable()

        Using parser As New TextFieldParser(csvPath)
            parser.TextFieldType = FieldType.Delimited
            parser.SetDelimiters(",")

            ' Read header
            Dim headers = parser.ReadFields()
            For Each h In headers
                miTekData.Columns.Add(h)
            Next

            ' Temporary table to hold ALL rows first
            Dim tempTable As New DataTable()
            For Each col In miTekData.Columns
                tempTable.Columns.Add(col.ToString())
            Next

            ' Load ALL rows into temp table
            While Not parser.EndOfData
                Dim fields = parser.ReadFields()
                If fields IsNot Nothing AndAlso fields.Length = headers.Length Then
                    tempTable.Rows.Add(fields)
                End If
            End While

            ' FILTER: Only rows with BOTH WONbr AND SONbr
            Dim validRows = tempTable.AsEnumerable() _
            .Where(Function(r)
                       Dim wo = If(r("WONbr") Is DBNull.Value, String.Empty, r("WONbr").ToString().Trim())
                       Dim so = If(r("SONbr") Is DBNull.Value, String.Empty, r("SONbr").ToString().Trim())
                       Return Not String.IsNullOrEmpty(wo) AndAlso Not String.IsNullOrEmpty(so)
                   End Function)

            ' Copy only valid rows to final table
            For Each row In validRows
                miTekData.ImportRow(row)
            Next
        End Using

        ' Bind to grid
        dgvMitek.DataSource = miTekData
        ColorMiTekRows()

        ' Friendly message if nothing qualified
        If miTekData.Rows.Count = 0 Then
            MessageBox.Show("No rows found with both Work Order (WONbr) and Sales Order (SONbr)." & vbCrLf &
                       "Only rows containing both will be imported.",
                       "No Valid Data", MessageBoxButtons.OK, MessageBoxIcon.Information)
            btnImportMatched.Enabled = False
        Else
            btnImportMatched.Enabled = True
        End If
    End Sub

    Private Sub LoadProjectLevels()
        Dim sql As String = "SELECT l.LevelID, l.LevelName, b.BuildingName + ' (x' + CAST(b.BldgQty AS varchar) + ')' AS Building, p.ProductTypeName 
                             FROM Levels l 
                             JOIN Buildings b ON l.BuildingID = b.BuildingID 
                             JOIN ProductType p ON l.ProductTypeID = p.ProductTypeID 
                             WHERE l.VersionID = @VersionID 
                             ORDER BY b.BuildingName, l.LevelNumber"
        projectLevels = New DataTable()
        Using reader = SqlConnectionManager.Instance.ExecuteReader(sql, {New SqlParameter("@VersionID", versionID)})
            projectLevels.Load(reader)
        End Using
        dgvProject.DataSource = projectLevels
    End Sub

    Private Sub dgvMiTek_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles dgvMitek.CellDoubleClick
        If e.RowIndex < 0 Then Exit Sub
        Dim row = dgvMitek.Rows(e.RowIndex)
        If dgvProject.SelectedRows.Count = 0 Then
            MessageBox.Show("Select a level on the right first.")
            Exit Sub
        End If
        Dim levelID = CInt(dgvProject.SelectedRows(0).Cells("LevelID").Value)
        matches(e.RowIndex) = levelID
        row.DefaultCellStyle.BackColor = Color.LightGreen
        row.Cells(0).ToolTipText = "Matched to: " & dgvProject.SelectedRows(0).Cells("LevelName").Value.ToString()
    End Sub

    Private Sub btnMatchSelected_Click(sender As Object, e As EventArgs) Handles btnMatchSelected.Click
        If dgvMitek.SelectedRows.Count = 0 OrElse dgvProject.SelectedRows.Count = 0 Then
            MessageBox.Show("Select one row from each grid.")
            Exit Sub
        End If
        Dim miTekRow = dgvMitek.SelectedRows(0).Index
        Dim levelID = CInt(dgvProject.SelectedRows(0).Cells("LevelID").Value)
        matches(miTekRow) = levelID
        dgvMitek.Rows(miTekRow).DefaultCellStyle.BackColor = Color.LightGreen
    End Sub

    Private Sub btnAutoMatchByName_Click(sender As Object, e As EventArgs) Handles btnAutoMatchByName.Click
        For Each row As DataGridViewRow In dgvMitek.Rows
            Dim name = row.Cells("LevelName").Value?.ToString().Trim()
            If String.IsNullOrEmpty(name) Then Continue For
            Dim match = projectLevels.AsEnumerable().
                FirstOrDefault(Function(r) r.Field(Of String)("LevelName").Trim() = name)
            If match IsNot Nothing Then
                matches(row.Index) = CInt(match("LevelID"))
                row.DefaultCellStyle.BackColor = Color.LightGreen
            End If
        Next
        MessageBox.Show($"Auto-matched {matches.Count} levels by name.")
    End Sub

    Private Sub btnClearMatches_Click(sender As Object, e As EventArgs) Handles btnClearMatches.Click
        matches.Clear()
        ColorMiTekRows()
    End Sub

    Private Sub ColorMiTekRows()
        For Each row As DataGridViewRow In dgvMitek.Rows
            row.DefaultCellStyle.BackColor = Color.White
        Next
    End Sub

    Private Sub btnImportMatched_Click(sender As Object, e As EventArgs) Handles btnImportMatched.Click
        If matches.Count = 0 Then
            MessageBox.Show("No matches to import.", "Nothing to do", MessageBoxButtons.OK, MessageBoxIcon.Information)
            Exit Sub
        End If

        Dim importedCount As Integer = 0

        Using conn = SqlConnectionManager.Instance.GetConnection()
            Using tran = conn.BeginTransaction()
                Try
                    For Each kvp In matches
                        Dim rowIndex As Integer = kvp.Key
                        Dim row As DataRow = miTekData.Rows(rowIndex)
                        Dim levelID As Integer = kvp.Value

                        ' NEW: SKIP ROWS WITHOUT BOTH WO and SO
                        Dim woNbr As String = If(row("WONbr") Is DBNull.Value OrElse row("WONbr") Is Nothing, String.Empty, row("WONbr").ToString().Trim())
                        Dim soNbr As String = If(row("SONbr") Is DBNull.Value OrElse row("SONbr") Is Nothing, String.Empty, row("SONbr").ToString().Trim())

                        If String.IsNullOrEmpty(woNbr) OrElse String.IsNullOrEmpty(soNbr) Then
                            ' Skip this row — no WO/SO
                            Continue For
                        End If

                        ' Calculate Misc Labor and Margin
                        Dim designLabor As Decimal = If(row("DesignLabor") Is DBNull.Value, 0D, CDec(row("DesignLabor")))
                        Dim mgmtLabor As Decimal = If(row("MGMTLabor") Is DBNull.Value, 0D, CDec(row("MGMTLabor")))
                        Dim jobSupplies As Decimal = If(row("JobSuppliesCost") Is DBNull.Value, 0D, CDec(row("JobSuppliesCost")))
                        Dim actualMiscLaborCost As Decimal = designLabor + mgmtLabor + jobSupplies

                        Dim totalSellPrice As Decimal = If(row("TotalSellPrice") Is DBNull.Value, 0D, CDec(row("TotalSellPrice")))
                        Dim overallCost As Decimal = If(row("OverallCost") Is DBNull.Value, 0D, CDec(row("OverallCost")))
                        Dim actualMarginPercent As Decimal = If(totalSellPrice = 0, 0D, (totalSellPrice - overallCost) / totalSellPrice)

                        Dim params() As SqlParameter = {
                        New SqlParameter("@LevelID", levelID),
                        New SqlParameter("@VersionID", versionID),
                        New SqlParameter("@StageType", stageType),
                        New SqlParameter("@ActualBDFT", If(row("BF") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(row("BF")), Object))),
                        New SqlParameter("@ActualLumberCost", If(row("LumberCost") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(row("LumberCost")), Object))),
                        New SqlParameter("@ActualPlateCost", If(row("PlateCost") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(row("PlateCost")), Object))),
                        New SqlParameter("@ActualManufLaborCost", If(row("ManufLaborCost") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(row("ManufLaborCost")), Object))),
                        New SqlParameter("@ActualManufMH", If(row("ManufMH") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(row("ManufMH")), Object))),
                        New SqlParameter("@ActualItemCost", If(row("ItemCost") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(row("ItemCost")), Object))),
                        New SqlParameter("@ActualDeliveryCost", If(row("DeliveryCost") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(row("DeliveryCost")), Object))),
                        New SqlParameter("@ActualMiscLaborCost", CType(actualMiscLaborCost, Object)),
                        New SqlParameter("@ActualTotalCost", If(row("OverallCost") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(row("OverallCost")), Object))),
                        New SqlParameter("@ActualSoldAmount", If(row("TotalSellPrice") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(row("TotalSellPrice")), Object))),
                        New SqlParameter("@ActualMarginPercent", CType(actualMarginPercent, Object)),
                        New SqlParameter("@AvgSPFNo2Actual", If(row("AvgSPFNo2") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(row("AvgSPFNo2")), Object))),
                        New SqlParameter("@MiTekJobNumber", If(stageType = 1,
                            If(row("JobNumber") Is DBNull.Value OrElse row("JobNumber") Is Nothing,
                                CType(DBNull.Value, Object),
                                CType(row("JobNumber").ToString().Trim(), Object)),
                            CType(DBNull.Value, Object))),
                        New SqlParameter("@BistrackWorksOrder", CType(woNbr, Object)),
                        New SqlParameter("@BisTrackSalesOrder", CType(soNbr, Object)),
                        New SqlParameter("@ImportedBy", Environment.UserName),
                        New SqlParameter("@Notes", $"Matched Import - WO:{woNbr} SO:{soNbr}")
                    }

                        SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.InsertLevelActual, params, conn, tran)
                        importedCount += 1
                    Next

                    tran.Commit()
                    MessageBox.Show($"Successfully imported {importedCount} level(s) with WO/SO numbers!", "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                    Me.DialogResult = DialogResult.OK
                    Me.Close()

                Catch ex As Exception
                    tran.Rollback()
                    MessageBox.Show("Import failed: " & ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Try
            End Using
        End Using
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub
End Class