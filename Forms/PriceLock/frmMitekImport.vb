' =====================================================
' frmMitekImport.vb
' Import component pricing from MiTek CSV export
' ENHANCED: Smart option resolution with fuzzy matching
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows.Forms
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Forms.PriceLock
Imports BuildersPSE2.Models

Partial Public Class frmMitekImport
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _priceLock As PLPriceLock
    Private _importData As List(Of MitekImportRow)
    Private _productTypes As List(Of PLProductType)
    Private _optionMappings As Dictionary(Of String, OptionMapping)

#End Region

#Region "Constructor"

    Public Sub New(priceLock As PLPriceLock, dataAccess As PriceLockDataAccess)
        _priceLock = priceLock
        _dataAccess = dataAccess
        _importData = New List(Of MitekImportRow)()
        _optionMappings = New Dictionary(Of String, OptionMapping)(StringComparer.OrdinalIgnoreCase)
        InitializeComponent()
        Me.Text = $"Import MiTek CSV - {_priceLock.SubdivisionName}"
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmMitekImport_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        SetupPreviewGrid()
        LoadProductTypes()
    End Sub

    Private Sub btnBrowse_Click(sender As Object, e As EventArgs) Handles btnBrowse.Click
        Using ofd As New OpenFileDialog()
            ofd.Title = "Select MiTek CSV Export"
            ofd.Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*"
            ofd.FilterIndex = 1

            If ofd.ShowDialog() = DialogResult.OK Then
                txtFilePath.Text = ofd.FileName
                LoadAndPreviewFile(ofd.FileName)
            End If
        End Using
    End Sub

    Private Sub btnResolveOptions_Click(sender As Object, e As EventArgs) Handles btnResolveOptions.Click
        ResolveOptions()
    End Sub

    Private Sub btnImport_Click(sender As Object, e As EventArgs) Handles btnImport.Click
        PerformImport()
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

#End Region

#Region "Grid Setup"

    Private Sub SetupPreviewGrid()
        dgvPreview.Columns.Clear()
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "Status", .HeaderText = "Status", .Width = 80})
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "PlanName", .HeaderText = "Plan", .Width = 100})
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ElevationName", .HeaderText = "Elevation", .Width = 80})
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "OptionName", .HeaderText = "Option (CSV)", .Width = 150})
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ResolvedOption", .HeaderText = "Option (Resolved)", .Width = 150})
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ProductType", .HeaderText = "Type", .Width = 60})
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "TrueCost", .HeaderText = "True Cost", .Width = 80, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2", .Alignment = DataGridViewContentAlignment.MiddleRight}})
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "Price", .HeaderText = "Sell Price", .Width = 80, .DefaultCellStyle = New DataGridViewCellStyle() With {.Format = "C2", .Alignment = DataGridViewContentAlignment.MiddleRight}})
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "Notes", .HeaderText = "Notes", .Width = 200})
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadProductTypes()
        _productTypes = _dataAccess.GetProductTypes()
    End Sub

    Private Sub LoadAndPreviewFile(filePath As String)
        Try
            Cursor = Cursors.WaitCursor
            lblStatus.Text = "Reading file..."
            Application.DoEvents()

            _importData.Clear()
            _optionMappings.Clear()
            dgvPreview.Rows.Clear()

            ' Read and parse CSV
            Dim lines = File.ReadAllLines(filePath)
            Dim lineNum = 0

            For Each line In lines
                lineNum += 1
                If String.IsNullOrWhiteSpace(line) Then Continue For

                Dim row = ParseCsvLine(line, lineNum)
                If row IsNot Nothing Then
                    _importData.Add(row)
                End If
            Next

            ' Collect unique option names for resolution
            CollectUniqueOptions()

            ' Initial validation (without option resolution)
            ValidateImportData()
            DisplayPreview()

            Dim optionCount = _optionMappings.Count
            lblRecordCount.Text = $"{_importData.Count} records found, {optionCount} unique option(s)"

            ' Enable resolve button if there are options
            btnResolveOptions.Enabled = optionCount > 0
            btnResolveOptions.Text = $"Resolve {optionCount} Option(s)..."

            ' Require option resolution before import if there are options
            If optionCount > 0 Then
                lblStatus.Text = "Click 'Resolve Options' to review option mappings"
                lblStatus.ForeColor = System.Drawing.Color.Orange
                btnImport.Enabled = False
            Else
                lblStatus.Text = "Ready to import (no options to resolve)"
                lblStatus.ForeColor = System.Drawing.Color.Green
                btnImport.Enabled = _importData.Count > 0
            End If

        Catch ex As Exception
            lblStatus.Text = $"Error: {ex.Message}"
            lblStatus.ForeColor = System.Drawing.Color.Red
            btnImport.Enabled = False
            btnResolveOptions.Enabled = False
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Function ParseCsvLine(line As String, lineNum As Integer) As MitekImportRow
        Try
            Dim parts = ParseCsvFields(line)
            If parts.Count < 6 Then Return Nothing

            Dim row As New MitekImportRow() With {
                .LineNumber = lineNum,
                .RawPlanName = parts(0).Trim(),
                .RawSubdivision = parts(1).Trim(),
                .RawFullDescription = parts(2).Trim(),
                .RawProductType = parts(3).Trim(),
                .RawTrueCost = parts(4).Trim(),
                .RawPrice = parts(5).Trim()
            }

            ' Normalize the full description (fix whitespace and dash issues)
            Dim normalizedDesc = TextNormalizer.NormalizeElevationName(row.RawFullDescription)

            ' Parse plan name
            row.PlanName = row.RawPlanName.Trim()

            ' Extract base elevation (normalized)
            row.ElevationName = TextNormalizer.ExtractBaseElevation(normalizedDesc)

            ' Extract option (keep original formatting for user review)
            row.OptionName = TextNormalizer.ExtractOptionPart(normalizedDesc)

            ' Parse product type
            row.ProductTypeName = row.RawProductType

            ' Parse true cost
            Dim trueCostStr = row.RawTrueCost.Replace("$", "").Replace(",", "").Replace("""", "").Trim()
            If Not Decimal.TryParse(trueCostStr, row.TrueCost) Then
                row.Notes = "Invalid true cost format"
                row.Status = ImportStatus.Error
            End If

            ' Parse sell price
            Dim priceStr = row.RawPrice.Replace("$", "").Replace(",", "").Replace("""", "").Trim()
            If Not Decimal.TryParse(priceStr, row.Price) Then
                row.Notes = "Invalid price format"
                row.Status = ImportStatus.Error
            End If

            Return row

        Catch ex As Exception
            Return New MitekImportRow() With {
                .LineNumber = lineNum,
                .Status = ImportStatus.Error,
                .Notes = $"Parse error: {ex.Message}"
            }
        End Try
    End Function

    Private Function ParseCsvFields(line As String) As List(Of String)
        Dim fields As New List(Of String)()
        Dim inQuotes = False
        Dim currentField As New System.Text.StringBuilder()

        For Each c In line
            If c = """"c Then
                inQuotes = Not inQuotes
            ElseIf c = ","c AndAlso Not inQuotes Then
                fields.Add(currentField.ToString())
                currentField.Clear()
            Else
                currentField.Append(c)
            End If
        Next

        fields.Add(currentField.ToString())
        Return fields
    End Function

    Private Sub CollectUniqueOptions()
        _optionMappings.Clear()

        For Each row In _importData
            If String.IsNullOrEmpty(row.OptionName) Then Continue For

            If Not _optionMappings.ContainsKey(row.OptionName) Then
                _optionMappings(row.OptionName) = New OptionMapping() With {
                    .CsvOptionName = row.OptionName,
                    .ResolvedOptionName = row.OptionName,
                    .IsNewOption = True,
                    .UserConfirmed = False,
                    .OccurrenceCount = 1
                }
            Else
                _optionMappings(row.OptionName).OccurrenceCount += 1
            End If
        Next
    End Sub

    Private Sub ValidateImportData()
        ' Get existing plans for this subdivision
        Dim existingPlans = _dataAccess.GetPlansBySubdivision(_priceLock.SubdivisionID)
        Dim existingPlanDict = existingPlans.ToDictionary(Function(p) p.PlanName.ToUpper(), Function(p) p)

        ' Get existing pricing for this lock
        Dim existingPricing = _dataAccess.GetComponentPricingByLock(_priceLock.PriceLockID)

        For Each row In _importData
            If row.Status = ImportStatus.Error Then Continue For

            row.Status = ImportStatus.New
            Dim notes As New List(Of String)()

            ' Check Plan
            Dim planKey = row.PlanName.ToUpper()
            If existingPlanDict.ContainsKey(planKey) Then
                row.PlanID = existingPlanDict(planKey).PlanID

                ' Check Elevation
                Dim elevations = _dataAccess.GetElevationsByPlan(row.PlanID.Value)
                Dim elev = elevations.FirstOrDefault(Function(el) el.ElevationName.Equals(row.ElevationName, StringComparison.OrdinalIgnoreCase))
                If elev IsNot Nothing Then
                    row.ElevationID = elev.ElevationID
                Else
                    notes.Add($"Elevation '{row.ElevationName}' not found")
                    If Not chkCreateMissingElevations.Checked Then row.Status = ImportStatus.Skip
                End If
            Else
                notes.Add($"Plan '{row.PlanName}' not found")
                If Not chkCreateMissingPlans.Checked Then row.Status = ImportStatus.Skip
            End If

            ' Check Product Type
            Dim pt = _productTypes.FirstOrDefault(Function(p) p.ProductTypeName.Equals(row.ProductTypeName, StringComparison.OrdinalIgnoreCase))
            If pt IsNot Nothing Then
                row.ProductTypeID = pt.ProductTypeID
            Else
                notes.Add($"Unknown product type '{row.ProductTypeName}'")
                row.Status = ImportStatus.Error
            End If

            ' Option will be resolved separately via the resolver dialog
            If Not String.IsNullOrEmpty(row.OptionName) Then
                If _optionMappings.ContainsKey(row.OptionName) Then
                    Dim mapping = _optionMappings(row.OptionName)
                    row.ResolvedOptionName = mapping.ResolvedOptionName
                    row.OptionID = mapping.ResolvedOptionID
                End If
            End If

            ' Check if pricing already exists (only if we have all IDs)
            If row.PlanID.HasValue AndAlso row.ElevationID.HasValue AndAlso row.ProductTypeID.HasValue Then
                Dim existingOptionID = row.OptionID
                Dim existing = existingPricing.FirstOrDefault(Function(cp) _
                    CBool(cp.PlanID = row.PlanID.Value AndAlso
                    cp.ElevationID = row.ElevationID.Value AndAlso
                    cp.ProductTypeID = row.ProductTypeID.Value AndAlso
                    ((cp.OptionID Is Nothing OrElse cp.OptionID = 0) AndAlso existingOptionID Is Nothing) OrElse
                    (cp.OptionID IsNot Nothing AndAlso existingOptionID IsNot Nothing AndAlso cp.OptionID = existingOptionID)))

                If existing IsNot Nothing Then
                    row.ExistingPricingID = existing.ComponentPricingID
                    row.Status = ImportStatus.Update
                    If Not chkUpdateExisting.Checked Then row.Status = ImportStatus.Skip
                End If
            End If

            row.Notes = String.Join("; ", notes)
        Next
    End Sub

    Private Sub DisplayPreview()
        dgvPreview.Rows.Clear()

        For Each row In _importData
            Dim statusText = row.Status.ToString()
            Dim dgvRow = dgvPreview.Rows.Add()

            dgvPreview.Rows(dgvRow).Cells("Status").Value = statusText
            dgvPreview.Rows(dgvRow).Cells("PlanName").Value = row.PlanName
            dgvPreview.Rows(dgvRow).Cells("ElevationName").Value = row.ElevationName
            dgvPreview.Rows(dgvRow).Cells("OptionName").Value = If(row.OptionName, "")
            dgvPreview.Rows(dgvRow).Cells("ResolvedOption").Value = If(row.ResolvedOptionName, If(row.OptionName, ""))
            dgvPreview.Rows(dgvRow).Cells("ProductType").Value = row.ProductTypeName
            dgvPreview.Rows(dgvRow).Cells("TrueCost").Value = row.TrueCost
            dgvPreview.Rows(dgvRow).Cells("Price").Value = row.Price
            dgvPreview.Rows(dgvRow).Cells("Notes").Value = If(row.Notes, "")

            ' Color code by status
            Select Case row.Status
                Case ImportStatus.New
                    dgvPreview.Rows(dgvRow).DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen
                Case ImportStatus.Update
                    dgvPreview.Rows(dgvRow).DefaultCellStyle.BackColor = System.Drawing.Color.LightYellow
                Case ImportStatus.Skip
                    dgvPreview.Rows(dgvRow).DefaultCellStyle.BackColor = System.Drawing.Color.LightGray
                Case ImportStatus.Error
                    dgvPreview.Rows(dgvRow).DefaultCellStyle.BackColor = System.Drawing.Color.LightPink
            End Select

            ' Highlight if option name differs from resolved
            If Not String.IsNullOrEmpty(row.OptionName) AndAlso
               Not String.IsNullOrEmpty(row.ResolvedOptionName) AndAlso
               Not row.OptionName.Equals(row.ResolvedOptionName, StringComparison.OrdinalIgnoreCase) Then
                dgvPreview.Rows(dgvRow).Cells("ResolvedOption").Style.ForeColor = System.Drawing.Color.Blue
                dgvPreview.Rows(dgvRow).Cells("ResolvedOption").Style.Font = New System.Drawing.Font(dgvPreview.Font, System.Drawing.FontStyle.Bold)
            End If
        Next
    End Sub

#End Region

#Region "Option Resolution"

    Private Sub ResolveOptions()
        Dim optionNames = _optionMappings.Keys.ToList()

        Using resolver As New frmOptionResolver(optionNames, _dataAccess)
            If resolver.ShowDialog(Me) = DialogResult.OK Then
                ' Update our mappings with the resolved values
                For Each resolved In resolver.ResolvedMappings
                    If _optionMappings.ContainsKey(resolved.CsvOptionName) Then
                        _optionMappings(resolved.CsvOptionName) = resolved
                    End If
                Next

                ' Update the import data with resolved options
                For Each row In _importData
                    If Not String.IsNullOrEmpty(row.OptionName) AndAlso _optionMappings.ContainsKey(row.OptionName) Then
                        Dim mapping = _optionMappings(row.OptionName)
                        row.ResolvedOptionName = mapping.ResolvedOptionName
                        row.OptionID = mapping.ResolvedOptionID
                    End If
                Next

                ' Re-validate and refresh display
                ValidateImportData()
                DisplayPreview()

                lblStatus.Text = "Options resolved. Ready to import."
                lblStatus.ForeColor = System.Drawing.Color.Green
                btnImport.Enabled = _importData.Count > 0
            End If
        End Using
    End Sub

#End Region

#Region "Import"

    Private Sub PerformImport()
        Try
            Cursor = Cursors.WaitCursor
            btnImport.Enabled = False
            prgImport.Visible = True
            prgImport.Value = 0
            prgImport.Maximum = _importData.Count

            Dim imported = 0
            Dim updated = 0
            Dim skipped = 0
            Dim errors = 0

            ' Get margin values from the price lock
            Dim adjustedMargin As Decimal = If(_priceLock.AdjustedMarginBaseModels, 0.15D)
            Dim optionMargin As Decimal = If(_priceLock.OptionMargin, adjustedMargin)

            ' First, create any new options that were confirmed
            Dim createdOptions As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
            For Each kvp In _optionMappings
                Dim mapping = kvp.Value
                If mapping.IsNewOption AndAlso mapping.UserConfirmed AndAlso Not mapping.ResolvedOptionID.HasValue Then
                    Dim newOptionID = _dataAccess.FindOrCreateOption(mapping.ResolvedOptionName)
                    mapping.ResolvedOptionID = newOptionID
                    createdOptions(mapping.CsvOptionName) = newOptionID
                End If
            Next

            ' Update import rows with newly created option IDs
            For Each row In _importData
                If Not String.IsNullOrEmpty(row.OptionName) AndAlso createdOptions.ContainsKey(row.OptionName) Then
                    row.OptionID = createdOptions(row.OptionName)
                End If
            Next

            For i = 0 To _importData.Count - 1
                Dim row = _importData(i)
                prgImport.Value = i + 1
                Application.DoEvents()

                If row.Status = ImportStatus.Skip OrElse row.Status = ImportStatus.Error Then
                    If row.Status = ImportStatus.Error Then errors += 1 Else skipped += 1
                    Continue For
                End If

                Try
                    ' Create missing plan if needed
                    If Not row.PlanID.HasValue AndAlso chkCreateMissingPlans.Checked Then
                        Dim newPlan As New PLPlan() With {.PlanName = row.PlanName, .IsActive = True}
                        row.PlanID = _dataAccess.InsertPlan(newPlan)
                        _dataAccess.AssignPlanToSubdivision(_priceLock.SubdivisionID, row.PlanID.Value)
                    End If

                    ' Create missing elevation if needed
                    If row.PlanID.HasValue AndAlso Not row.ElevationID.HasValue AndAlso chkCreateMissingElevations.Checked Then
                        Dim newElev As New PLElevation() With {.PlanID = row.PlanID.Value, .ElevationName = row.ElevationName, .IsActive = True}
                        row.ElevationID = _dataAccess.InsertElevation(newElev)
                    End If

                    ' Get option ID from mapping if not already set
                    If Not String.IsNullOrEmpty(row.OptionName) AndAlso Not row.OptionID.HasValue Then
                        If _optionMappings.ContainsKey(row.OptionName) Then
                            row.OptionID = _optionMappings(row.OptionName).ResolvedOptionID
                        End If
                    End If

                    ' Skip if we still don't have required IDs
                    If Not row.PlanID.HasValue OrElse Not row.ElevationID.HasValue OrElse Not row.ProductTypeID.HasValue Then
                        skipped += 1
                        Continue For
                    End If

                    ' Determine which margin to use based on whether this is an option
                    Dim isOption As Boolean = row.OptionID.HasValue AndAlso row.OptionID.Value > 0
                    Dim marginToApply As Decimal = If(isOption, optionMargin, adjustedMargin)
                    Dim marginSource As String = If(isOption, "Option", "Adjusted")

                    ' Calculate prices using margin formula: Price = Cost / (1 - Margin)
                    Dim calculatedPrice As Decimal = 0D
                    If row.TrueCost > 0 AndAlso marginToApply < 1 Then
                        calculatedPrice = row.TrueCost / (1 - marginToApply)
                    End If

                    ' Create or update pricing
                    If row.Status = ImportStatus.Update AndAlso row.ExistingPricingID.HasValue Then
                        Dim existing = _dataAccess.GetComponentPricingByID(row.ExistingPricingID.Value)
                        If existing IsNot Nothing Then
                            existing.Cost = row.TrueCost
                            existing.MgmtSellPrice = row.Price
                            existing.AppliedMargin = marginToApply
                            existing.CalculatedPrice = calculatedPrice
                            existing.FinalPrice = calculatedPrice
                            existing.PriceSentToSales = calculatedPrice
                            existing.PriceSentToBuilder = calculatedPrice
                            existing.MarginSource = marginSource
                            existing.ModifiedBy = Environment.UserName
                            _dataAccess.UpdateComponentPricing(existing)
                            updated += 1
                        End If
                    Else
                        Dim pricing As New PLComponentPricing() With {
                            .PriceLockID = _priceLock.PriceLockID,
                            .PlanID = row.PlanID.Value,
                            .ElevationID = row.ElevationID.Value,
                            .OptionID = row.OptionID,
                            .ProductTypeID = row.ProductTypeID.Value,
                            .Cost = row.TrueCost,
                            .MgmtSellPrice = row.Price,
                            .AppliedMargin = marginToApply,
                            .CalculatedPrice = calculatedPrice,
                            .FinalPrice = calculatedPrice,
                            .PriceSentToSales = calculatedPrice,
                            .PriceSentToBuilder = calculatedPrice,
                            .MarginSource = marginSource,
                            .IsAdder = isOption,
                            .ModifiedBy = Environment.UserName
                        }
                        _dataAccess.InsertComponentPricing(pricing)
                        imported += 1
                    End If

                Catch ex As Exception
                    row.Notes = $"Import error: {ex.Message}"
                    errors += 1
                End Try
            Next

            MessageBox.Show($"Import complete!" & vbCrLf & vbCrLf &
                $"New records: {imported}" & vbCrLf &
                $"Updated: {updated}" & vbCrLf &
                $"Skipped: {skipped}" & vbCrLf &
                $"Errors: {errors}" & vbCrLf & vbCrLf &
                $"Margins applied:" & vbCrLf &
                $"  Adjusted: {adjustedMargin:P1}" & vbCrLf &
                $"  Option: {optionMargin:P1}",
                "Import Results", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show($"Import failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
            btnImport.Enabled = True
            prgImport.Visible = False
        End Try
    End Sub

#End Region

End Class

#Region "Helper Classes"

''' <summary>
''' Represents a single row from the MiTek CSV import
''' </summary>
Public Class MitekImportRow
    ' Raw values from CSV
    Public Property LineNumber As Integer
    Public Property RawPlanName As String
    Public Property RawSubdivision As String
    Public Property RawFullDescription As String
    Public Property RawProductType As String
    Public Property RawTrueCost As String
    Public Property RawPrice As String

    ' Parsed values
    Public Property PlanName As String
    Public Property ElevationName As String
    Public Property OptionName As String
    Public Property ResolvedOptionName As String
    Public Property ProductTypeName As String
    Public Property TrueCost As Decimal
    Public Property Price As Decimal

    ' Matched IDs
    Public Property PlanID As Integer?
    Public Property ElevationID As Integer?
    Public Property OptionID As Integer?
    Public Property ProductTypeID As Integer?
    Public Property ExistingPricingID As Integer?

    ' Status
    Public Property Status As ImportStatus = ImportStatus.New
    Public Property Notes As String
End Class

Public Enum ImportStatus
    [New]
    Update
    Skip
    [Error]
End Enum

#End Region