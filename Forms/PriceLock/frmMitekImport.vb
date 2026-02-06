' =====================================================
' frmMitekImport.vb
' Import component pricing from MiTek CSV export
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Drawing
Imports System.IO
Imports System.Text.RegularExpressions
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models

Partial Public Class frmMitekImport
    Inherits Form

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _priceLock As PLPriceLock
    Private _importData As List(Of MitekImportRow)
    Private _productTypes As List(Of PLProductType)

#End Region

#Region "Constructor"

    Public Sub New(priceLock As PLPriceLock, dataAccess As PriceLockDataAccess)
        _priceLock = priceLock
        _dataAccess = dataAccess
        _importData = New List(Of MitekImportRow)()
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
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "Status", .HeaderText = "Status", .Width = 70})
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "PlanName", .HeaderText = "Plan", .Width = 100})
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "ElevationName", .HeaderText = "Elevation", .Width = 80})
        dgvPreview.Columns.Add(New DataGridViewTextBoxColumn() With {.Name = "OptionName", .HeaderText = "Option", .Width = 150})
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

            ' Validate and display preview
            ValidateImportData()
            DisplayPreview()

            lblRecordCount.Text = $"{_importData.Count} records found"
            lblStatus.Text = "Ready to import"
            lblStatus.ForeColor = Color.Green
            btnImport.Enabled = _importData.Count > 0

        Catch ex As Exception
            lblStatus.Text = $"Error: {ex.Message}"
            lblStatus.ForeColor = Color.Red
            btnImport.Enabled = False
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Function ParseCsvLine(line As String, lineNum As Integer) As MitekImportRow
        ' CSV format: Plan Name, Subdivision, Elevation/Description, Product Type, True Cost, Sell Price
        ' Example: Plan 2094,Tanterra,Elev A,Roof,"$5,610.01","$6,981.45"

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

            ' Parse plan name (keep "Plan " prefix for matching)
            row.PlanName = row.RawPlanName.Trim()

            ' Parse elevation/description (keep "Elev " prefix for matching)
            row.ElevationName = row.RawFullDescription.Trim()

            ' Parse option from full description
            row.OptionName = ExtractOptionName(row.RawFullDescription)

            ' If there's an option, extract the base elevation name
            If Not String.IsNullOrEmpty(row.OptionName) Then
                ' Extract base elevation (e.g., "Elev A" from "Elev A - Opt Outdoor Room")
                Dim dashIndex = row.RawFullDescription.IndexOf(" -", StringComparison.Ordinal)
                If dashIndex > 0 Then
                    row.ElevationName = row.RawFullDescription.Substring(0, dashIndex).Trim()
                End If
            End If

            ' Parse product type
            row.ProductTypeName = row.RawProductType

            ' Parse true cost (remove $, commas, quotes)
            Dim trueCostStr = row.RawTrueCost.Replace("$", "").Replace(",", "").Replace("""", "").Trim()
            If Decimal.TryParse(trueCostStr, row.TrueCost) = False Then
                row.Notes = "Invalid true cost format"
                row.Status = ImportStatus.Error
            End If

            ' Parse sell price (remove $, commas, quotes)
            Dim priceStr = row.RawPrice.Replace("$", "").Replace(",", "").Replace("""", "").Trim()
            If Decimal.TryParse(priceStr, row.Price) = False Then
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
        ' Handle CSV with quoted fields containing commas
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

    Private Function ExtractOptionName(fullDescription As String) As String
        ' Full description might be "Elev A - Opt Outdoor Room", "Elev A - Outdoor Room Opt", or just "Elev A"
        ' We need to extract the option part

        If String.IsNullOrEmpty(fullDescription) Then Return Nothing

        ' Look for " - " pattern (common separator for options)
        Dim dashIndex = fullDescription.IndexOf(" - ", StringComparison.Ordinal)
        If dashIndex > 0 Then
            Dim optPart = fullDescription.Substring(dashIndex + 3).Trim()
            ' Remove common prefixes like "Opt ", "Opt. ", etc.
            optPart = Regex.Replace(optPart, "^Opt\.?\s*", "", RegexOptions.IgnoreCase).Trim()
            If Not String.IsNullOrEmpty(optPart) Then Return optPart
        End If

        ' Look for " Opt" suffix pattern (e.g., "Outdoor Room Opt")
        If fullDescription.EndsWith(" Opt", StringComparison.OrdinalIgnoreCase) Then
            ' Look backwards for the elevation part
            Dim elevPattern = "^Elev\s+[A-Z]\s+"
            Dim match = Regex.Match(fullDescription, elevPattern, RegexOptions.IgnoreCase)
            If match.Success Then
                Dim optPart = fullDescription.Substring(match.Length).Trim()
                ' Remove trailing "Opt"
                If optPart.EndsWith(" Opt", StringComparison.OrdinalIgnoreCase) Then
                    optPart = optPart.Substring(0, optPart.Length - 4).Trim()
                End If
                If Not String.IsNullOrEmpty(optPart) Then Return optPart
            End If
        End If

        Return Nothing ' No option - this is a base elevation
    End Function

    Private Sub ValidateImportData()
        ' Get existing plans for this subdivision
        Dim existingPlans = _dataAccess.GetPlansBySubdivision(_priceLock.SubdivisionID)
        Dim existingPlanDict = existingPlans.ToDictionary(Function(p) p.PlanName.ToUpper(), Function(p) p)

        ' Get existing options
        Dim existingOptions = _dataAccess.GetOptions()
        Dim existingOptionDict = existingOptions.ToDictionary(Function(o) o.OptionName.ToUpper(), Function(o) o)

        ' Get existing pricing for this lock
        Dim existingPricing = _dataAccess.GetComponentPricingByLock(_priceLock.PriceLockID)

        For Each row In _importData
            If row.Status = ImportStatus.Error Then Continue For

            row.Status = ImportStatus.New
            Dim notes As New List(Of String)()

            ' Check Plan (match with full name including "Plan " prefix)
            Dim planKey = row.PlanName.ToUpper()
            If existingPlanDict.ContainsKey(planKey) Then
                row.PlanID = existingPlanDict(planKey).PlanID

                ' Check Elevation (match with full name including "Elev " prefix)
                Dim elevations = _dataAccess.GetElevationsByPlan(row.PlanID.Value)
                Dim elev = elevations.FirstOrDefault(Function(e) e.ElevationName.Equals(row.ElevationName, StringComparison.OrdinalIgnoreCase))
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

            ' Check Option (if applicable)
            If Not String.IsNullOrEmpty(row.OptionName) Then
                Dim optKey = row.OptionName.ToUpper()
                If existingOptionDict.ContainsKey(optKey) Then
                    row.OptionID = existingOptionDict(optKey).OptionID
                Else
                    notes.Add($"Option '{row.OptionName}' will be created")
                    If Not chkCreateMissingOptions.Checked Then row.Status = ImportStatus.Skip
                End If
            End If

            ' Check Product Type
            Dim pt = _productTypes.FirstOrDefault(Function(p) p.ProductTypeName.Equals(row.ProductTypeName, StringComparison.OrdinalIgnoreCase))
            If pt IsNot Nothing Then
                row.ProductTypeID = pt.ProductTypeID
            Else
                notes.Add($"Unknown product type '{row.ProductTypeName}'")
                row.Status = ImportStatus.Error
            End If

            ' Check if pricing already exists
            If row.PlanID.HasValue AndAlso row.ElevationID.HasValue AndAlso row.ProductTypeID.HasValue Then
                Dim existing = existingPricing.FirstOrDefault(Function(cp) _
                    CBool(cp.PlanID = row.PlanID.Value AndAlso
                    cp.ElevationID = row.ElevationID.Value AndAlso
                    cp.ProductTypeID = row.ProductTypeID.Value AndAlso
                    (cp.OptionID Is Nothing AndAlso row.OptionID Is Nothing OrElse cp.OptionID = row.OptionID)))

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
            ' Add values in the correct order matching the column setup
            Dim dgvRow = dgvPreview.Rows.Add()

            dgvPreview.Rows(dgvRow).Cells("Status").Value = statusText
            dgvPreview.Rows(dgvRow).Cells("PlanName").Value = row.PlanName
            dgvPreview.Rows(dgvRow).Cells("ElevationName").Value = row.ElevationName
            dgvPreview.Rows(dgvRow).Cells("OptionName").Value = If(row.OptionName, "")
            dgvPreview.Rows(dgvRow).Cells("ProductType").Value = row.ProductTypeName
            dgvPreview.Rows(dgvRow).Cells("TrueCost").Value = row.TrueCost
            dgvPreview.Rows(dgvRow).Cells("Price").Value = row.Price
            dgvPreview.Rows(dgvRow).Cells("Notes").Value = If(row.Notes, "")

            ' Color code by status
            Select Case row.Status
                Case ImportStatus.New
                    dgvPreview.Rows(dgvRow).DefaultCellStyle.BackColor = Color.LightGreen
                Case ImportStatus.Update
                    dgvPreview.Rows(dgvRow).DefaultCellStyle.BackColor = Color.LightYellow
                Case ImportStatus.Skip
                    dgvPreview.Rows(dgvRow).DefaultCellStyle.BackColor = Color.LightGray
                Case ImportStatus.Error
                    dgvPreview.Rows(dgvRow).DefaultCellStyle.BackColor = Color.LightPink
            End Select
        Next
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
                        ' Assign to subdivision
                        _dataAccess.AssignPlanToSubdivision(_priceLock.SubdivisionID, row.PlanID.Value)
                    End If

                    ' Create missing elevation if needed
                    If row.PlanID.HasValue AndAlso Not row.ElevationID.HasValue AndAlso chkCreateMissingElevations.Checked Then
                        Dim newElev As New PLElevation() With {.PlanID = row.PlanID.Value, .ElevationName = row.ElevationName, .IsActive = True}
                        row.ElevationID = _dataAccess.InsertElevation(newElev)
                    End If

                    ' Create missing option if needed
                    If Not String.IsNullOrEmpty(row.OptionName) AndAlso Not row.OptionID.HasValue AndAlso chkCreateMissingOptions.Checked Then
                        row.OptionID = _dataAccess.FindOrCreateOption(row.OptionName)
                    End If

                    ' Skip if we still don't have required IDs
                    If Not row.PlanID.HasValue OrElse Not row.ElevationID.HasValue OrElse Not row.ProductTypeID.HasValue Then
                        skipped += 1
                        Continue For
                    End If

                    ' Create or update pricing
                    If row.Status = ImportStatus.Update AndAlso row.ExistingPricingID.HasValue Then
                        ' Update existing
                        Dim existing = _dataAccess.GetComponentPricingByID(row.ExistingPricingID.Value)
                        If existing IsNot Nothing Then
                            existing.Cost = row.TrueCost
                            existing.MgmtSellPrice = row.Price
                            existing.ModifiedBy = Environment.UserName
                            _dataAccess.UpdateComponentPricing(existing)
                            updated += 1
                        End If
                    Else
                        ' Insert new
                        Dim pricing As New PLComponentPricing() With {
                            .PriceLockID = _priceLock.PriceLockID,
                            .PlanID = row.PlanID.Value,
                            .ElevationID = row.ElevationID.Value,
                            .OptionID = row.OptionID,
                            .ProductTypeID = row.ProductTypeID.Value,
                            .Cost = row.TrueCost,
                            .MgmtSellPrice = row.Price,
                            .IsAdder = (row.OptionID.HasValue),
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
                $"Errors: {errors}",
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
    [New]       ' Will be inserted
    Update      ' Will update existing
    Skip        ' Will be skipped
    [Error]     ' Has errors
End Enum

#End Region