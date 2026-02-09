' =====================================================
' frmRandomLengthsImportEdit.vb
' Dialog for creating/editing Random Lengths import header
' BuildersPSE2 - PriceLock Module
' =====================================================

Option Strict On
Option Explicit On

Imports System.Drawing
Imports System.IO
Imports System.Windows.Forms
Imports BuildersPSE2.DataAccess
Imports BuildersPSE2.Models
Imports ClosedXML.Excel
Imports ClosedXML.Graphics

Public Class frmRandomLengthsImportEdit

#Region "Fields"

    Private _dataAccess As PriceLockDataAccess
    Private _import As PLRandomLengthsImport
    Private _isNew As Boolean
    Private _presetExcelPath As String = Nothing  ' ← ADD THIS

    Public Property SavedImportID As Integer

#End Region

#Region "Constructor"

    Public Sub New(rlImport As PLRandomLengthsImport, dataAccess As PriceLockDataAccess)
        _dataAccess = dataAccess
        _import = rlImport
        _isNew = (rlImport Is Nothing)

        InitializeComponent()

        If _isNew Then
            Me.Text = "New Random Lengths Import"
            _import = New PLRandomLengthsImport()
        Else
            Me.Text = "Edit Random Lengths Import"
        End If
    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Sets up the form for Excel import. Must be called BEFORE ShowDialog.
    ''' </summary>
    Public Sub SetupForExcelImport(excelFilePath As String)
        _presetExcelPath = excelFilePath
    End Sub

#End Region

#Region "Form Events"

    Private Sub frmRandomLengthsImportEdit_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        LoadData()

        ' Apply preset Excel path if set (from ImportFromExcelDirect)
        If Not String.IsNullOrEmpty(_presetExcelPath) Then
            txtExcelPath.Text = _presetExcelPath
            btnImportExcel.Enabled = True

            ' Auto-populate report name if empty
            If String.IsNullOrEmpty(txtReportName.Text) Then
                txtReportName.Text = Path.GetFileNameWithoutExtension(_presetExcelPath)
            End If

            ' Set import method to Excel
            cboImportMethod.SelectedItem = "Excel"
        End If
    End Sub

    Private Sub btnOK_Click(sender As Object, e As EventArgs) Handles btnOK.Click
        If ValidateInput() Then
            SaveImport()
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        Me.DialogResult = DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub btnBrowseExcel_Click(sender As Object, e As EventArgs) Handles btnBrowseExcel.Click


        BrowseForExcelFile()
    End Sub

    Private Sub btnImportExcel_Click(sender As Object, e As EventArgs) Handles btnImportExcel.Click
        ImportFromExcel()
    End Sub

#End Region

#Region "Data Loading"

    Private Sub LoadData()
        ' Populate import method combo
        cboImportMethod.Items.Clear()
        cboImportMethod.Items.AddRange(New String() {"Manual", "Excel", "PDF"})

        If _isNew Then
            dtpReportDate.Value = Date.Today
            txtReportName.Text = ""
            cboImportMethod.SelectedIndex = 0  ' Manual
            txtNotes.Text = ""
            txtExcelPath.Text = ""
            btnImportExcel.Enabled = False
        Else
            dtpReportDate.Value = _import.ReportDate
            txtReportName.Text = _import.ReportName
            cboImportMethod.Text = _import.ImportMethod
            txtNotes.Text = _import.Notes
            txtExcelPath.Text = ""

            If Not String.IsNullOrEmpty(_import.SourceFileName) Then
                lblSourceFile.Text = $"Source: {_import.SourceFileName}"
                lblSourceFile.Visible = True
            End If
        End If
    End Sub

#End Region

#Region "Excel Import"

    Private Sub BrowseForExcelFile()
        Using ofd As New OpenFileDialog()
            ofd.Filter = "Excel Files|*.xlsx;*.xls|All Files|*.*"
            ofd.Title = "Select Random Lengths Pricing Excel File"
            ofd.CheckFileExists = True

            If ofd.ShowDialog() = DialogResult.OK Then
                txtExcelPath.Text = ofd.FileName
                btnImportExcel.Enabled = True

                ' Try to auto-populate report name from filename if empty
                If String.IsNullOrEmpty(txtReportName.Text) Then
                    txtReportName.Text = Path.GetFileNameWithoutExtension(ofd.FileName)
                End If
            End If
        End Using
    End Sub

    Private Sub ImportFromExcel()
        If String.IsNullOrEmpty(txtExcelPath.Text) OrElse Not File.Exists(txtExcelPath.Text) Then
            MessageBox.Show("Please select a valid Excel file first.", "Invalid File",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning)
            Return
        End If

        Try
            Cursor = Cursors.WaitCursor

            ' First, validate the input
            If Not ValidateInput() Then
                Return
            End If

            ' Prepare the import record
            _import.ReportDate = dtpReportDate.Value.Date
            _import.ReportName = txtReportName.Text.Trim()
            _import.ImportMethod = "Excel"
            _import.SourceFileName = Path.GetFileName(txtExcelPath.Text)
            _import.Notes = txtNotes.Text.Trim()
            _import.ModifiedBy = Environment.UserName

            ' Save the import header FIRST to get a valid ID
            If _isNew Then
                _import.ImportedBy = Environment.UserName
                SavedImportID = _dataAccess.InsertRandomLengthsImport(_import)
                Debug.WriteLine($"DEBUG: Inserted import with ID: {SavedImportID}")
                _import.RandomLengthsImportID = SavedImportID
                _isNew = False
            Else
                _dataAccess.UpdateRandomLengthsImport(_import)
                SavedImportID = _import.RandomLengthsImportID
            End If

            ' NOW import pricing from Excel using the saved import ID
            Dim importedCount = ImportPricingFromExcel(txtExcelPath.Text, SavedImportID)

            MessageBox.Show($"Successfully imported {importedCount} price(s) from Excel." & vbCrLf & vbCrLf &
                      "The import has been saved.",
                      "Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show($"Error importing from Excel: {ex.Message}", "Import Error",
                      MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

    Private Function ImportPricingFromExcel(excelPath As String, importID As Integer) As Integer
        Dim importedCount = 0
        Dim tempFile As String = Nothing

        Try
            ' Create a temp copy of the file
            tempFile = Path.Combine(Path.GetTempPath(), $"RLImport_{Guid.NewGuid()}.xlsx")
            File.Copy(excelPath, tempFile, True)

            ' Open the temp file and remove all sheets except "Costs"
            Using workbook = New XLWorkbook(tempFile)
                ' Get list of sheets to delete (everything except "Costs")
                Dim sheetsToDelete = workbook.Worksheets _
                .Where(Function(ws) Not ws.Name.Equals("Costs", StringComparison.OrdinalIgnoreCase)) _
                .ToList()

                ' Delete all non-Costs sheets
                For Each sheet In sheetsToDelete
                    sheet.Delete()
                Next

                ' Save the cleaned workbook
                workbook.Save()
            End Using

            ' Now reload the cleaned file (only has "Costs" sheet, no images)
            Using workbook = New XLWorkbook(tempFile)
                If Not workbook.Worksheets.Contains("Costs") Then
                    Throw New Exception("Excel file does not contain a 'Costs' worksheet.")
                End If

                Dim worksheet = workbook.Worksheet("Costs")
                Dim pricingTypes = _dataAccess.GetRLPricingTypes()
                Dim cellMappings = GetCellToTypeCodeMappings()

                ' Import each mapped cell
                For Each mapping In cellMappings
                    Try
                        Dim cell = worksheet.Cell(mapping.CellAddress)
                        If cell.IsEmpty() Then Continue For

                        Dim price As Decimal = 0

                        Try
                            ' In ClosedXML 0.105.0, use GetNumber() instead of TryGetNumber()
                            Dim cellValue = cell.Value

                            ' Check if the value is a number type
                            If cellValue.IsNumber Then
                                price = CDec(cellValue.GetNumber())

                                If price > 0 Then
                                    Dim pricingType = pricingTypes.FirstOrDefault(Function(pt) pt.TypeCode = mapping.TypeCode)
                                    If pricingType Is Nothing Then
                                        Debug.WriteLine($"TypeCode not found: {mapping.TypeCode}")
                                        Continue For
                                    End If

                                    Dim pricing As New PLRandomLengthsPricing() With {
                        .RandomLengthsImportID = importID,
                        .RLPricingTypeID = pricingType.RLPricingTypeID,
                        .Price = price,
                        .PriceSource = $"Excel Cell {mapping.CellAddress}",
                        .Notes = Nothing
                    }

                                    _dataAccess.UpsertRandomLengthsPricing(pricing)
                                    importedCount += 1
                                    Debug.WriteLine($"Successfully imported {mapping.CellAddress}: {price}")
                                Else
                                    Debug.WriteLine($"Skipping {mapping.CellAddress}: Price is zero or negative")
                                End If
                            Else
                                Debug.WriteLine($"Skipping {mapping.CellAddress}: Not a number (Type: {cellValue.Type})")
                            End If

                        Catch ex As NotImplementedException
                            Debug.WriteLine($"Skipping {mapping.CellAddress}: External reference not supported")
                            Continue For
                        End Try

                    Catch ex As Exception
                        Debug.WriteLine($"Error importing cell {mapping.CellAddress}: {ex.Message}")
                    End Try
                Next
            End Using

        Finally
            ' Clean up temp file
            If tempFile IsNot Nothing AndAlso File.Exists(tempFile) Then
                Try
                    File.Delete(tempFile)
                Catch
                    ' Ignore cleanup errors
                End Try
            End If
        End Try

        Return importedCount
    End Function



    Private Function GetCellToTypeCodeMappings() As List(Of ExcelCellMapping)
        ' =====================================================
        ' Excel Cell to TypeCode Mappings for Random Lengths Import
        ' =====================================================
        ' MAINTENANCE NOTE: 
        ' - These mappings are for the standard Random Lengths "Costs" worksheet
        ' - Cell locations are based on the 2024 RL report format
        ' - Only update if Random Lengths changes their report structure
        ' - Last updated: 2025-02-04 by System
        ' 
        ' STRUCTURE:
        ' - Columns B/C = Denver/Grand Island pricing (adjust as needed)
        ' - Rows organized by material category (Studs, SPF, OSB, etc.)
        ' =====================================================

        Dim mappings As New List(Of ExcelCellMapping)

        ' STUDS Category - Hem Fir
        mappings.Add(New ExcelCellMapping("B18", "STUD_2X4_92_HF"))     ' 2x4 92-5/8 Stud Hem Fir
        mappings.Add(New ExcelCellMapping("B19", "STUD_2X4_104_HF"))    ' 2x4 104-5/8 Stud Hem Fir
        mappings.Add(New ExcelCellMapping("B20", "STUD_2X4_116_HF"))    ' 2x4 116-5/8 Stud Hem Fir
        mappings.Add(New ExcelCellMapping("B21", "STUD_2X6_92_HF"))     ' 2x6 92-5/8 Stud Hem Fir
        mappings.Add(New ExcelCellMapping("B22", "STUD_2X6_104_HF"))    ' 2x6 104-5/8 Stud Hem Fir
        mappings.Add(New ExcelCellMapping("B23", "STUD_2X6_116_HF"))    ' 2x6 116-5/8 Stud Hem Fir

        ' STUDS Category - SPF Denver
        mappings.Add(New ExcelCellMapping("D18", "STUD_2X4_92_DF"))    ' 2x4 92-5/8 Stud SPF Denver
        mappings.Add(New ExcelCellMapping("D19", "STUD_2X4_104_DF"))   ' 2x4 104-5/8 Stud SPF Denver
        mappings.Add(New ExcelCellMapping("D20", "STUD_2X4_116_DF"))   ' 2x4 116-5/8 Stud SPF Denver
        mappings.Add(New ExcelCellMapping("D21", "STUD_2X6_92_DF"))    ' 2x6 92-5/8 Stud SPF Denver
        mappings.Add(New ExcelCellMapping("D22", "STUD_2X6_104_DF"))   ' 2x6 104-5/8 Stud SPF Denver
        mappings.Add(New ExcelCellMapping("D23", "STUD_2X6_116_DF"))   ' 2x6 116-5/8 Stud SPF Denver

        ' FRAMING LUMBER - Coastal Hem Fir
        mappings.Add(New ExcelCellMapping("B5", "DIM_2X4_HF"))         ' 2x4 #2 Coastal Hem Fir
        mappings.Add(New ExcelCellMapping("B6", "DIM_2X6_HF"))         ' 2x6 #2 Coastal Hem Fir
        mappings.Add(New ExcelCellMapping("B7", "DIM_2X8_HF"))        ' 2x8 #2 Hem Fir
        mappings.Add(New ExcelCellMapping("B8", "DIM_2X10_HF"))       ' 2x10 #2 Hem Fir
        mappings.Add(New ExcelCellMapping("B9", "DIM_2X12_HF"))       ' 2x12 #2 Hem Fir

        ' FRAMING LUMBER - Inland Fir
        mappings.Add(New ExcelCellMapping("C5", "DIM_2X4_IF"))         ' 2x4 #2 Inland Fir
        mappings.Add(New ExcelCellMapping("C6", "DIM_2X6_IF"))         ' 2x6 #2 Inland Fir

        ' FRAMING LUMBER - Douglas Fir
        mappings.Add(New ExcelCellMapping("D5", "DIM_2X4_DF"))         ' 2x4 #2 Doug Fir
        mappings.Add(New ExcelCellMapping("D6", "DIM_2X6_DF"))         ' 2x6 #2 Doug Fir

        ' SPF - Denver
        mappings.Add(New ExcelCellMapping("F5", "SPF_2X4_DEN"))       ' 2x4 #2 SPF Denver
        mappings.Add(New ExcelCellMapping("F6", "SPF_2X6_DEN"))       ' 2x6 #2 SPF Denver

        ' SPF - Grand Island
        mappings.Add(New ExcelCellMapping("I5", "SPF_2X4_GI"))        ' 2x4 #2 SPF Grand Island
        mappings.Add(New ExcelCellMapping("I6", "SPF_2X6_GI"))        ' 2x6 #2 SPF Grand Island

        ' SPF - MSR
        mappings.Add(New ExcelCellMapping("J14", "SPF_1650_2X4"))      ' 2x4 SPF 1650f MSR


        ' OSB - Denver
        mappings.Add(New ExcelCellMapping("B26", "OSB_7_16_DEN"))      ' 7/16 OSB Denver
        mappings.Add(New ExcelCellMapping("C26", "OSB_15_32_DEN"))     ' 15/32 OSB Denver
        mappings.Add(New ExcelCellMapping("D26", "OSB_15_32_S1_DEN"))  ' 15/32 Struc 1 OSB Denver
        mappings.Add(New ExcelCellMapping("E26", "OSB_19_32_DEN"))     ' 19/32 OSB Denver
        mappings.Add(New ExcelCellMapping("F26", "OSB_3_4_TG_DEN"))    ' 3/4 T&G OSB Denver

        ' OSB - Grand Island
        mappings.Add(New ExcelCellMapping("B31", "OSB_7_16_GI"))       ' 7/16 OSB Grand Island
        mappings.Add(New ExcelCellMapping("C31", "OSB_15_32_GI"))      ' 15/32 OSB Grand Island

        ' MSR - Douglas Fir
        mappings.Add(New ExcelCellMapping("E14", "MSR_DF_1800_2X4"))      ' 2x4 DF 1800f MSR
        mappings.Add(New ExcelCellMapping("E15", "MSR_DF_1800_2X6"))      ' 2x6 DF 1800f MSR
        mappings.Add(New ExcelCellMapping("F14", "MSR_DF_2400_2X4"))      ' 2x4 DF 2400f MSR
        mappings.Add(New ExcelCellMapping("F15", "MSR_DF_2400_2X6"))      ' 2x6 DF 2400f MSR
        mappings.Add(New ExcelCellMapping("I14", "MSR_DF_1950_2X8"))      ' 2x8 DF 1950f MSR
        mappings.Add(New ExcelCellMapping("I15", "MSR_DF_1950_2X10"))      ' 2x10 DF 1950f MSR

        '' TREATED - Borate
        'mappings.Add(New ExcelCellMapping("B32", "TRT_BORATE_2X4"))    ' Borate Treated 2x4
        'mappings.Add(New ExcelCellMapping("B33", "TRT_BORATE_2X6"))    ' Borate Treated 2x6

        '' TREATED - Cedartone MCA
        'mappings.Add(New ExcelCellMapping("C32", "TRT_CEDAR_2X4"))     ' Cedartone Treated 2x4
        'mappings.Add(New ExcelCellMapping("C33", "TRT_CEDAR_2X6"))     ' Cedartone Treated 2x6
        'mappings.Add(New ExcelCellMapping("C34", "TRT_CEDAR_2X8"))     ' Cedartone Treated 2x8
        'mappings.Add(New ExcelCellMapping("C35", "TRT_CEDAR_2X10"))    ' Cedartone Treated 2x10
        'mappings.Add(New ExcelCellMapping("C36", "TRT_CEDAR_2X12"))    ' Cedartone Treated 2x12

        '' EWP (Engineered Wood Products)
        'mappings.Add(New ExcelCellMapping("B39", "EWP_LVL_1_75"))      ' LVL 1-3/4"
        'mappings.Add(New ExcelCellMapping("B40", "EWP_IJOIST_9_5"))    ' I-Joist 9-1/2"
        'mappings.Add(New ExcelCellMapping("B41", "EWP_IJOIST_11_875")) ' I-Joist 11-7/8"
        'mappings.Add(New ExcelCellMapping("B42", "EWP_IJOIST_14"))     ' I-Joist 14"
        'mappings.Add(New ExcelCellMapping("B43", "EWP_IJOIST_16"))     ' I-Joist 16"

        ' WIDES - Douglas Fir
        mappings.Add(New ExcelCellMapping("D8", "WIDE_2X10_DF"))      ' 2x10 Doug Fir
        mappings.Add(New ExcelCellMapping("D9", "WIDE_2X12_DF"))      ' 2x12 Doug Fir

        ' WIDES - Hem Fir
        mappings.Add(New ExcelCellMapping("C8", "WIDE_2X10_HF"))      ' 2x10 Hem Fir
        mappings.Add(New ExcelCellMapping("C9", "WIDE_2X12_HF"))      ' 2x12 Hem Fir

        Return mappings
    End Function

    ''' <summary>Helper class to map Excel cells to pricing type codes</summary>
    Private Class ExcelCellMapping
        Public Property CellAddress As String
        Public Property TypeCode As String

        Public Sub New(cellAddr As String, typeCode As String)
            Me.CellAddress = cellAddr
            Me.TypeCode = typeCode
        End Sub
    End Class

#End Region

#Region "Validation and Save"

    Private Function ValidateInput() As Boolean
        ' Check for duplicate date (only when creating new or changing date)
        If _dataAccess.CheckRandomLengthsImportExists(dtpReportDate.Value.Date,
            If(_isNew, CType(Nothing, Integer?), _import.RandomLengthsImportID)) Then

            MessageBox.Show($"An import already exists for {dtpReportDate.Value:MM/dd/yyyy}. Please choose a different date.",
                "Duplicate Date", MessageBoxButtons.OK, MessageBoxIcon.Warning)
            dtpReportDate.Focus()
            Return False
        End If

        Return True
    End Function

    Private Sub SaveImport()
        Try
            Cursor = Cursors.WaitCursor

            _import.ReportDate = dtpReportDate.Value.Date
            _import.ReportName = txtReportName.Text.Trim()
            _import.ImportMethod = cboImportMethod.Text
            _import.Notes = txtNotes.Text.Trim()
            _import.ModifiedBy = Environment.UserName

            If _isNew Then
                _import.ImportedBy = Environment.UserName
                SavedImportID = _dataAccess.InsertRandomLengthsImport(_import)
            Else
                _dataAccess.UpdateRandomLengthsImport(_import)
                SavedImportID = _import.RandomLengthsImportID
            End If

            Me.DialogResult = DialogResult.OK
            Me.Close()

        Catch ex As Exception
            MessageBox.Show($"Error saving import: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        Finally
            Cursor = Cursors.Default
        End Try
    End Sub

#End Region

End Class
