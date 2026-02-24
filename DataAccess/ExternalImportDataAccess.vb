Option Strict On
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.Services
Imports BuildersPSE2.Utilities
Imports Microsoft.VisualBasic.FileIO


Namespace DataAccess
    '=====================================================================
    ' Result class for Wall Import
    '=====================================================================
    Public Class ImportWallSummary
        Public Property VersionID As Integer
        Public Property Success As Boolean = False
        Public Property WasCancelled As Boolean = False
        Public Property StartTime As Date
        Public Property EndTime As Date
        Public Property TotalWallRows As Integer = 0
        Public Property SkippedRows As Integer = 0
        Public Property BuildingsCreated As Integer = 0
        Public Property LevelsCreated As Integer = 0
        Public Property RawUnitsImported As Integer = 0
        Public Property ActualUnitsImported As Integer = 0
        Public Property MappingsCreated As Integer = 0
        Public Property BuildingMappingsApplied As Integer = 0
        Public Property Log As New Text.StringBuilder()

        Public Function GetSummaryText() As String
            Dim sb As New Text.StringBuilder()
            sb.AppendLine("=== Wall Import Summary ===")
            sb.AppendLine($"Version ID: {VersionID}")
            sb.AppendLine($"Status     : {(If(Success, "SUCCESS", "FAILED"))}")
            If WasCancelled Then sb.AppendLine("CANCELLED by user")
            sb.AppendLine($"Duration   : {(If(EndTime > StartTime, (EndTime - StartTime).TotalSeconds, 0)):F1} seconds")
            sb.AppendLine($"Wall rows  : {TotalWallRows} (skipped {SkippedRows})")
            sb.AppendLine($"Buildings created : {BuildingsCreated}")
            sb.AppendLine($"Wall levels created: {LevelsCreated}")
            sb.AppendLine($"Units imported     : {ActualUnitsImported}")
            sb.AppendLine($"Mappings created   : {MappingsCreated}")
            sb.AppendLine($"Building mappings  : {BuildingMappingsApplied}")
            sb.AppendLine()
            sb.AppendLine(Log.ToString())
            Return sb.ToString()
        End Function
    End Class

    Public Class ExternalImportDataAccess

        '=====================================================================
        ' IMPORT PSE SPREADSHEET AS NEW PROJECT
        '=====================================================================
        Public Shared Sub ImportSpreadsheetAsNewProject(filePath As String)
            UIHelper.ShowBusy(frmMain, "Importing spreadsheet as new project...")
            Dim spreadsheetData As Dictionary(Of String, DataTable) = SpreadsheetParser.ParseSpreadsheet(filePath)
            Dim importLog As New StringBuilder()
            Try
                SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                           Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                               conn.Open()
                                                                               Using transaction As SqlTransaction = conn.BeginTransaction()
                                                                                   Try
                                                                                       ' Step 1: Extract project details from Summary sheet
                                                                                       ' NOTE: Summary sheet uses positional column indexes, not named columns
                                                                                       Dim summaryDt As DataTable = spreadsheetData("Summary")
                                                                                       Dim jbid As String = TableOperations.GetString(summaryDt.Rows(0), 1)
                                                                                       Dim projectName As String = TableOperations.GetString(summaryDt.Rows(1), 1)
                                                                                       Dim address As String = TableOperations.GetString(summaryDt.Rows(2), 1)
                                                                                       Dim city As String = TableOperations.GetString(summaryDt.Rows(3), 1)
                                                                                       Dim state As String = TableOperations.GetString(summaryDt.Rows(3), 3)
                                                                                       Dim zip As String = TableOperations.GetString(summaryDt.Rows(3), 4)
                                                                                       Dim bidDate As Date? = ParseDate(TableOperations.GetString(summaryDt.Rows(2), 6))
                                                                                       Dim archPlansDated As Date? = ParseDate(TableOperations.GetString(summaryDt.Rows(3), 6))
                                                                                       Dim engPlansDated As Date? = ParseDate(TableOperations.GetString(summaryDt.Rows(4), 6))
                                                                                       Dim milesToJobSite As Integer? = TableOperations.GetInteger(summaryDt.Rows(5), 6)
                                                                                       Dim totalNetSqft As Integer? = TableOperations.GetInteger(summaryDt.Rows(6), 1)
                                                                                       Dim totalGrossSqft As Integer? = TableOperations.GetInteger(summaryDt.Rows(7), 1)
                                                                                       Dim architectName As String = TableOperations.GetString(summaryDt.Rows(9), 1)
                                                                                       Dim engineerName As String = TableOperations.GetString(summaryDt.Rows(8), 1)
                                                                                       Dim customerName As String = TableOperations.GetString(summaryDt.Rows(5), 1)
                                                                                       Dim estimatorName As String = TableOperations.GetString(summaryDt.Rows(1), 6)
                                                                                       Dim salesName As String = TableOperations.GetString(summaryDt.Rows(0), 6)

                                                                                       ' Step 2: Create Project and Version using TableOperations
                                                                                       Dim ids = CreateProjectAndVersion(conn, transaction, jbid, projectName,
                                                                                           "Imported PSE " & Date.Now.ToString("yyyyMMdd"), "Imported from PSE spreadsheet",
                                                                                           address, city, state, zip, bidDate, archPlansDated, engPlansDated,
                                                                                           milesToJobSite, totalNetSqft, totalGrossSqft, Nothing,
                                                                                           customerName, Nothing, architectName, engineerName,
                                                                                           estimatorName, Nothing, salesName, Nothing, Nothing, 1, True)
                                                                                       Dim projectID As Integer = ids.Item1
                                                                                       Dim newVersionID As Integer = ids.Item2
                                                                                       UIHelper.Add($"Project '{projectName}' created with ID {projectID}, Version ID {newVersionID}.")

                                                                                       ' Step 3: Import RawUnits using TableOperations mapper
                                                                                       Dim rawUnitMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                                                                                       Dim rawUnitCount As Integer = 0
                                                                                       For Each sheet In {"FloorImport", "RoofImport"}
                                                                                           If spreadsheetData.ContainsKey(sheet) Then
                                                                                               Dim dt As DataTable = spreadsheetData(sheet)

                                                                                               ' Check if DataTable is empty or missing required columns
                                                                                               If dt.Rows.Count = 0 OrElse Not dt.Columns.Contains("JobNumber") OrElse Not dt.Columns.Contains("Elevation") Then
                                                                                                   UIHelper.Add($"Skipping {sheet} - no data or missing required columns.")
                                                                                                   Continue For
                                                                                               End If

                                                                                               Dim productTypeID As Integer = If(sheet = "FloorImport", 1, 2)
                                                                                               For Each row As DataRow In dt.Rows
                                                                                                   If row("JobNumber") Is DBNull.Value Then Continue For
                                                                                                   Dim planName As String = TableOperations.GetString(row, "Elevation").Trim()
                                                                                                   Dim jobNumber As String = CStr(row("JobNumber")).Trim()
                                                                                                   Dim uniqueKey As String = If(String.IsNullOrEmpty(planName), "Unknown_" & jobNumber, planName & "_" & jobNumber)

                                                                                                   ' Use TableOperations mapper instead of inline 30+ line With block
                                                                                                   Dim rawModel As RawUnitModel = TableOperations.MapRawUnitFromDataRow(row, productTypeID)
                                                                                                   rawModel.RawUnitName = planName

                                                                                                   ' Lookup cost effective date
                                                                                                   Dim costEffectiveID As Object = DBNull.Value
                                                                                                   If rawModel.AvgSPFNo2.HasValue Then
                                                                                                       Dim fetchParams As SqlParameter() = {New SqlParameter("@SPFLumberCost", rawModel.AvgSPFNo2.Value)}
                                                                                                       costEffectiveID = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(
                                                                                                       Queries.SelectCostEffectiveDateIDByCost, fetchParams, conn, transaction)
                                                                                                   End If

                                                                                                   ' Use TableOperations for insert
                                                                                                   Dim newRawID As Integer = TableOperations.InsertRawUnitWithHistory(rawModel, newVersionID, conn, transaction, costEffectiveID)
                                                                                                   rawUnitMap.Add(uniqueKey, newRawID)
                                                                                                   rawUnitCount += 1
                                                                                               Next
                                                                                           End If
                                                                                       Next
                                                                                       UIHelper.Add($"Imported {rawUnitCount} raw units.")
                                                                                       ' Step 4: Import ActualUnits
                                                                                       Dim actualUnitMap As New Dictionary(Of String, Integer)
                                                                                       Dim actualUnitSet As New HashSet(Of String)
                                                                                       Dim actualUnitCount As Integer = 0
                                                                                       Dim currentSortOrder As Integer = 1
                                                                                       For Each sheet In {"Floor Unit Data", "Roof Unit Data"}
                                                                                           If Not spreadsheetData.ContainsKey(sheet) Then Continue For
                                                                                           Dim dt As DataTable = spreadsheetData(sheet)
                                                                                           Dim productTypeID As Integer = If(sheet.Contains("Floor"), 1, 2)
                                                                                           Dim importSheetName As String = If(productTypeID = 1, "FloorImport", "RoofImport")

                                                                                           ' Pre-build elevation to jobNumber lookup (case-insensitive)
                                                                                           Dim elevationToJobNumber As New Dictionary(Of String, String)(StringComparer.OrdinalIgnoreCase)
                                                                                           If spreadsheetData.ContainsKey(importSheetName) Then
                                                                                               Dim importDt As DataTable = spreadsheetData(importSheetName)
                                                                                               If importDt.Rows.Count > 0 AndAlso importDt.Columns.Contains("Elevation") AndAlso importDt.Columns.Contains("JobNumber") Then
                                                                                                   For Each r As DataRow In importDt.Rows
                                                                                                       If r("Elevation") IsNot DBNull.Value AndAlso r("JobNumber") IsNot DBNull.Value Then
                                                                                                           Dim elev As String = CStr(r("Elevation")).Trim()
                                                                                                           Dim jn As String = CStr(r("JobNumber")).Trim()
                                                                                                           If Not String.IsNullOrEmpty(elev) AndAlso Not elevationToJobNumber.ContainsKey(elev) Then
                                                                                                               elevationToJobNumber(elev) = jn
                                                                                                           End If
                                                                                                       End If
                                                                                                   Next
                                                                                               End If
                                                                                           End If

                                                                                           For col As Integer = 46 To dt.Columns.Count - 1
                                                                                               Dim rawName As String = dt.Columns(col).ColumnName.Trim()
                                                                                               Dim unitName As String = If(dt.Rows(0)(col) Is DBNull.Value, String.Empty, CStr(dt.Rows(0)(col)).Trim())
                                                                                               If String.IsNullOrEmpty(unitName) OrElse unitName = "<select>" Then Continue For

                                                                                               ' FIX: Changed regex to match both "Model-" and "Models-" prefixes
                                                                                               Dim planName As String = Regex.Replace(rawName, "^Models?-?", "", RegexOptions.IgnoreCase).Trim()
                                                                                               planName = Regex.Replace(planName, "[_]\d+$", "").Trim()

                                                                                               ' Look up jobNumber from pre-built dictionary
                                                                                               Dim jobNumber As String = String.Empty
                                                                                               If elevationToJobNumber.ContainsKey(planName) Then
                                                                                                   jobNumber = elevationToJobNumber(planName)
                                                                                               End If

                                                                                               Dim lookupKey As String = If(String.IsNullOrEmpty(planName), "Unknown_" & jobNumber, planName & "_" & jobNumber)
                                                                                               Dim rawID As Integer = If(rawUnitMap.ContainsKey(lookupKey), rawUnitMap(lookupKey), 0)
                                                                                               If rawID = 0 Then Continue For

                                                                                               Dim planSqftObj As Object = dt.Rows(1)(col)
                                                                                               Dim planSqft As Decimal = 0D
                                                                                               If planSqftObj IsNot DBNull.Value Then Decimal.TryParse(CStr(planSqftObj), planSqft)
                                                                                               Dim unitType As String = "Res"
                                                                                               If dt.Rows(3)(col) IsNot DBNull.Value Then
                                                                                                   Dim s As String = CStr(dt.Rows(3)(col)).Trim()
                                                                                                   If s = "Res" OrElse s = "Nonres" Then unitType = s
                                                                                               End If

                                                                                               Dim uniqueCombo As String = unitName & "_" & planSqft.ToString() & "_" & rawID
                                                                                               If actualUnitSet.Contains(uniqueCombo) Then Continue For
                                                                                               Dim cleanUnitName As String = Regex.Replace(unitName.Trim(), "actual$", "", RegexOptions.IgnoreCase).Trim()
                                                                                               Dim uniqueKey As String = $"{cleanUnitName}_{planSqft}_{col}"

                                                                                               Dim actualModel As New ActualUnitModel With {
                                                                                                     .ProductTypeID = productTypeID,
                                                                                                     .UnitName = unitName,
                                                                                                     .PlanSQFT = planSqft,
                                                                                                     .UnitType = unitType,
                                                                                                     .MarginPercent = 0,
                                                                                                     .OptionalAdder = 1D,
                                                                                                     .SortOrder = currentSortOrder
                                                                                                 }
                                                                                               Dim newActualID As Integer = TableOperations.InsertActualUnit(actualModel, rawID, newVersionID, conn, transaction)
                                                                                               actualUnitMap(uniqueKey) = newActualID
                                                                                               actualUnitSet.Add(uniqueCombo)
                                                                                               actualUnitCount += 1
                                                                                               currentSortOrder += 1
                                                                                           Next
                                                                                       Next
                                                                                       UIHelper.Add($"Imported {actualUnitCount} actual units.")
                                                                                       ' Step 5: Import CalculatedComponents using TableOperations
                                                                                       Dim componentCount As Integer = 0
                                                                                       Dim rawUnitDataCache As New Dictionary(Of Integer, Dictionary(Of String, Decimal))
                                                                                       For Each actualKey As String In actualUnitMap.Keys
                                                                                           Dim actualUnitID As Integer = actualUnitMap(actualKey)
                                                                                           Dim rawUnitIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(
                                                                                               "SELECT RawUnitID FROM ActualUnits WHERE ActualUnitID = @ActualUnitID AND VersionID = @VersionID",
                                                                                               {New SqlParameter("@ActualUnitID", actualUnitID), New SqlParameter("@VersionID", newVersionID)}, conn, transaction)
                                                                                           If rawUnitIDObj Is DBNull.Value Then Continue For
                                                                                           Dim rawUnitID As Integer = CInt(rawUnitIDObj)

                                                                                           If Not rawUnitDataCache.ContainsKey(rawUnitID) Then
                                                                                               Dim data = LoadRawUnitData(rawUnitID, newVersionID, conn, transaction)
                                                                                               If data Is Nothing Then Continue For
                                                                                               rawUnitDataCache(rawUnitID) = data
                                                                                           End If

                                                                                           Dim sqFt As Decimal = rawUnitDataCache(rawUnitID)("SqFt")
                                                                                           If sqFt <= 0 Then Continue For

                                                                                           ' Use TableOperations for component insert
                                                                                           TableOperations.InsertCalculatedComponentsFromRawUnit(rawUnitDataCache(rawUnitID), actualUnitID, newVersionID, conn, transaction)
                                                                                           componentCount += 13 ' 13 components per unit
                                                                                       Next
                                                                                       UIHelper.Add($"Imported {componentCount} calculated components.")

                                                                                       ' Step 6: Import Buildings
                                                                                       Dim buildingMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                                                                                       Dim buildingCount As Integer = 0
                                                                                       If spreadsheetData.ContainsKey("Buildings") Then
                                                                                           Dim dt As DataTable = spreadsheetData("Buildings")
                                                                                           For i As Integer = 3 To dt.Rows.Count - 1
                                                                                               Dim buildingName As String = If(dt.Rows(i)(1) Is DBNull.Value, String.Empty, CStr(dt.Rows(i)(1))).Trim()
                                                                                               If String.IsNullOrEmpty(buildingName) Then Continue For
                                                                                               Dim resUnits As Integer? = If(dt.Rows(i)(2) Is DBNull.Value, Nothing, CInt(dt.Rows(i)(2)))
                                                                                               Dim bldgQty As Integer = If(dt.Rows(i)(3) Is DBNull.Value, 0, CInt(dt.Rows(i)(3)))
                                                                                               Dim newBldgID As Integer = InsertBuildingIfNew(buildingName, newVersionID, conn, transaction, buildingMap, resUnits, bldgQty)
                                                                                               If newBldgID > 0 Then buildingCount += 1
                                                                                           Next
                                                                                       End If
                                                                                       UIHelper.Add($"Imported {buildingCount} buildings.")

                                                                                       ' Step 7: Import Levels
                                                                                       Dim levelMap As New Dictionary(Of Tuple(Of Integer, Integer, String), Integer)(New TupleComparer(StringComparer.OrdinalIgnoreCase))
                                                                                       Dim levelCount As Integer = 0
                                                                                       For Each sheet In {"Floor Unit Data", "Roof Unit Data"}
                                                                                           If spreadsheetData.ContainsKey(sheet) Then
                                                                                               Dim dt As DataTable = spreadsheetData(sheet)
                                                                                               Dim productTypeID As Integer = If(sheet.Contains("Floor"), 1, 2)
                                                                                               For rowIdx As Integer = 33 To Math.Min(dt.Rows.Count - 1, 68)
                                                                                                   Dim buildingName As String = If(dt.Rows(rowIdx)(0) Is DBNull.Value, String.Empty, CStr(dt.Rows(rowIdx)(0)).Trim())
                                                                                                   If String.IsNullOrEmpty(buildingName) OrElse Not buildingMap.ContainsKey(buildingName) Then Continue For
                                                                                                   Dim levelName As String = If(dt.Rows(rowIdx)(1) Is DBNull.Value OrElse String.IsNullOrEmpty(CStr(dt.Rows(rowIdx)(1)).Trim()),
                                                                                                       If(productTypeID = 2, "roof", String.Empty), CStr(dt.Rows(rowIdx)(1)).Trim())
                                                                                                   If String.IsNullOrEmpty(levelName) Then Continue For
                                                                                                   Dim buildingID As Integer = buildingMap(buildingName)
                                                                                                   Dim newLevelID As Integer = GetOrCreateLevel(buildingID, productTypeID, levelName, newVersionID, conn, transaction, levelMap, rowIdx - 32)
                                                                                                   If newLevelID > 0 Then levelCount += 1
                                                                                               Next
                                                                                           End If
                                                                                       Next
                                                                                       UIHelper.Add($"Imported {levelCount} levels.")

                                                                                       ' Step 8: Import ActualToLevelMappings
                                                                                       Dim mappingCount As Integer = 0
                                                                                       For Each sheet In {"Floor Unit Data", "Roof Unit Data"}
                                                                                           If spreadsheetData.ContainsKey(sheet) Then
                                                                                               Dim dt As DataTable = spreadsheetData(sheet)
                                                                                               Dim productTypeID As Integer = If(sheet.Contains("Floor"), 1, 2)
                                                                                               For rowIdx As Integer = 33 To dt.Rows.Count - 1
                                                                                                   Dim buildingName As String = If(dt.Rows(rowIdx)(0) Is DBNull.Value, String.Empty, CStr(dt.Rows(rowIdx)(0)).Trim())
                                                                                                   If String.IsNullOrEmpty(buildingName) OrElse Not buildingMap.ContainsKey(buildingName) Then Continue For
                                                                                                   Dim levelName As String = If(dt.Rows(rowIdx)(1) Is DBNull.Value OrElse String.IsNullOrEmpty(CStr(dt.Rows(rowIdx)(1)).Trim()),
                                                                                                       If(productTypeID = 2, "roof", String.Empty), CStr(dt.Rows(rowIdx)(1)).Trim())
                                                                                                   If String.IsNullOrEmpty(levelName) Then Continue For
                                                                                                   Dim buildingID As Integer = buildingMap(buildingName)
                                                                                                   Dim levelKey = Tuple.Create(buildingID, productTypeID, levelName)
                                                                                                   Dim levelID As Integer = If(levelMap.ContainsKey(levelKey), levelMap(levelKey), 0)
                                                                                                   If levelID = 0 Then Continue For

                                                                                                   For colIdx As Integer = 46 To dt.Columns.Count - 1
                                                                                                       Dim qtyObj As Object = dt.Rows(rowIdx)(colIdx)
                                                                                                       If qtyObj Is DBNull.Value OrElse String.IsNullOrWhiteSpace(CStr(qtyObj)) OrElse (IsNumeric(qtyObj) AndAlso CInt(qtyObj) <= 0) Then Continue For
                                                                                                       Dim rawUnitName As String = If(dt.Rows(0)(colIdx) Is DBNull.Value, String.Empty, CStr(dt.Rows(0)(colIdx))).Trim()
                                                                                                       Dim unitName As String = Regex.Replace(rawUnitName, "actual$", "", RegexOptions.IgnoreCase).Trim()
                                                                                                       If String.IsNullOrEmpty(unitName) Then Continue For
                                                                                                       Dim planSqft As String = If(dt.Rows(1)(colIdx) Is DBNull.Value, "0", CStr(dt.Rows(1)(colIdx)))
                                                                                                       Dim actualUnitID As Integer = 0
                                                                                                       For Each key As String In actualUnitMap.Keys
                                                                                                           If key.StartsWith(unitName & "_" & planSqft & "_") Then
                                                                                                               actualUnitID = actualUnitMap(key)
                                                                                                               Exit For
                                                                                                           End If
                                                                                                       Next
                                                                                                       If actualUnitID = 0 Then Continue For

                                                                                                       ' Use TableOperations for mapping insert
                                                                                                       TableOperations.InsertActualToLevelMapping(actualUnitID, levelID, CInt(qtyObj), newVersionID, conn, transaction)
                                                                                                       mappingCount += 1
                                                                                                   Next
                                                                                               Next
                                                                                           End If
                                                                                       Next
                                                                                       UIHelper.Add($"Imported {mappingCount} mappings.")

                                                                                       FinalizeImport(newVersionID, importLog, transaction)
                                                                                   Catch ex As Exception
                                                                                       transaction.Rollback()
                                                                                       Throw
                                                                                   End Try
                                                                               End Using
                                                                           End Using
                                                                       End Sub, "Error importing spreadsheet as new project")
            Finally
                UIHelper.HideBusy(frmMain)
            End Try
        End Sub

        '=====================================================================
        ' IMPORT PROJECT FROM CSV (MGMT Export)
        '=====================================================================
        Public Shared Function ImportProjectFromCSV(csvFilePath As String, projectName As String, customerName As String,
                                           Optional estimatorID As Integer? = Nothing, Optional salesID As Integer? = Nothing,
                                           Optional address As String = Nothing, Optional city As String = Nothing,
                                           Optional state As String = Nothing, Optional zip As String = Nothing,
                                           Optional biddate As Date? = Nothing, Optional archdate As Date? = Nothing,
                                           Optional engdate As Date? = Nothing, Optional miles As Integer? = Nothing) As Integer

            Dim newProjectID As Integer = 0
            Dim importLog As New StringBuilder()

            UIHelper.ShowBusy(frmMain, "Importing project from CSV...")
            Try
                Using conn = SqlConnectionManager.Instance.GetConnection()
                    Using transaction As SqlTransaction = conn.BeginTransaction()
                        Try
                            Dim fileName = System.IO.Path.GetFileNameWithoutExtension(csvFilePath)
                            Dim projectNumber = fileName.Split("-"c)(0).Trim()

                            ' Create Project and Version
                            Dim ids = CreateProjectAndVersion(conn, transaction, projectNumber, projectName,
                                "Imported from MGMT " & Now.ToString("yyyyMMdd"), "Imported from MGMT full model Estimate",
                                address, city, state, zip, biddate, archdate, engdate,
                                miles, 0, 0, Nothing, customerName, Nothing, Nothing, Nothing,
                                Nothing, estimatorID, Nothing, salesID, Nothing, 1, True)
                            newProjectID = ids.Item1
                            Dim versionID As Integer = ids.Item2
                            UIHelper.Add($"Project '{projectName}' created with ID {newProjectID}, Version ID {versionID}.")

                            ' Parse CSV
                            Dim buildingKeys As New HashSet(Of String)
                            Dim rows As New List(Of String())
                            Using parser As New TextFieldParser(csvFilePath)
                                parser.TextFieldType = FieldType.Delimited
                                parser.SetDelimiters(",")
                                parser.ReadLine()
                                While Not parser.EndOfData
                                    Dim fields = parser.ReadFields()
                                    If fields.Length >= 26 Then
                                        rows.Add(fields)
                                        Dim prefix = fields(0).Trim().Split("-"c)(0).Trim()
                                        Dim plan = fields(6).Trim()
                                        buildingKeys.Add(prefix & "_" & plan)
                                    End If
                                End While
                            End Using

                            ' Insert Buildings
                            Dim buildingIdMap As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                            For Each key In buildingKeys
                                Dim planName = key.Split("_"c)(1)
                                InsertBuildingIfNew(planName, versionID, conn, transaction, buildingIdMap, Nothing, 1, 1, key)
                            Next

                            ' Process Rows
                            Dim levelMap As New Dictionary(Of Tuple(Of Integer, Integer, String), Integer)
                            Dim wallSortOrder, floorSortOrder, roofSortOrder As Integer
                            wallSortOrder = 1 : floorSortOrder = 1 : roofSortOrder = 1

                            For Each fields In rows
                                Dim product = fields(1).Trim()
                                Dim productTypeID = If(product = "Floor", 1, If(product = "Roof", 2, If(product = "Wall", 3, 0)))
                                If productTypeID = 0 Then Continue For

                                Dim prefix = fields(0).Trim().Split("-"c)(0).Trim()
                                Dim plan = fields(6).Trim()
                                Dim buildingKey = (prefix & "_" & plan).ToLower
                                Dim elevation = fields(7).Trim()
                                Dim rawUnitName = fields(0).Trim() & " " & product
                                Dim buildingID As Integer = buildingIdMap(buildingKey)

                                Dim currentSort As Integer
                                Select Case productTypeID
                                    Case 1 : currentSort = floorSortOrder : floorSortOrder += 1
                                    Case 2 : currentSort = roofSortOrder : roofSortOrder += 1
                                    Case 3 : currentSort = wallSortOrder : wallSortOrder += 1
                                    Case Else : currentSort = 1
                                End Select

                                ' Use TableOperations mapper for RawUnit
                                Dim rawModel As RawUnitModel = TableOperations.MapRawUnitFromCsvFields(fields, rawUnitName, productTypeID)
                                If productTypeID = 3 Then
                                    TableOperations.ApplyWallAdjustments(rawModel, fields)
                                End If

                                ' Ensure SqFt is valid
                                If Not rawModel.SqFt.HasValue OrElse rawModel.SqFt.Value <= 0 Then rawModel.SqFt = 1D
                                Dim sqFt As Decimal = rawModel.SqFt.Value

                                ' Get or Create Level
                                Dim cleanElevation As String = If(String.IsNullOrEmpty(elevation), "default", elevation.ToLower())
                                Dim levelID As Integer = GetOrCreateLevel(buildingID, productTypeID, cleanElevation, versionID, conn, transaction, levelMap)

                                ' Insert RawUnit
                                Dim newRawUnitID As Integer = TableOperations.InsertRawUnitWithHistory(rawModel, versionID, conn, transaction)

                                ' Insert ActualUnit and Mapping
                                Dim actualModel As New ActualUnitModel With {
                                    .ProductTypeID = productTypeID,
                                    .UnitName = elevation,
                                    .PlanSQFT = sqFt,
                                    .UnitType = "Res",
                                    .OptionalAdder = 1D,
                                    .SortOrder = currentSort
                                }
                                Dim newActualUnitID As Integer = TableOperations.InsertActualUnit(actualModel, newRawUnitID, versionID, conn, transaction)
                                TableOperations.InsertActualToLevelMapping(newActualUnitID, levelID, 1, versionID, conn, transaction)
                            Next

                            FinalizeImport(versionID, importLog, transaction)
                        Catch ex As Exception
                            transaction.Rollback()
                            Throw
                        End Try
                    End Using
                End Using
            Finally
                UIHelper.HideBusy(frmMain)
            End Try

            Return newProjectID
        End Function

        '=====================================================================
        ' IMPORT WALLS INTERACTIVE
        '=====================================================================
        Public Shared Function ImportWallsInteractive(csvFilePath As String, versionID As Integer) As ImportWallSummary
            Dim summary As New ImportWallSummary With {.VersionID = versionID, .StartTime = Date.Now}
            SqlConnectionManager.CloseAllDataReaders()
            UIHelper.ShowBusy(frmMain, "Importing wall data from CSV...")
            Try
                SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                           Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                               conn.Open()
                                                                               Using transaction As SqlTransaction = conn.BeginTransaction()
                                                                                   Try
                                                                                       ' Parse CSV - Wall rows only
                                                                                       Dim wallRows As New List(Of String())
                                                                                       Dim buildingKeysFromCsv As New HashSet(Of String)
                                                                                       Dim skippedRows As Integer = 0
                                                                                       Using parser As New TextFieldParser(csvFilePath)
                                                                                           parser.TextFieldType = FieldType.Delimited
                                                                                           parser.SetDelimiters(",")
                                                                                           parser.ReadLine()
                                                                                           While Not parser.EndOfData
                                                                                               Dim fields As String() = parser.ReadFields()
                                                                                               If fields.Length < 26 Then skippedRows += 1 : Continue While
                                                                                               If fields(0).Trim().Equals("JobNumber", StringComparison.OrdinalIgnoreCase) OrElse
                                                                                                  fields(1).Trim().Equals("Product", StringComparison.OrdinalIgnoreCase) Then Continue While
                                                                                               If Not String.Equals(fields(1).Trim(), "Wall", StringComparison.OrdinalIgnoreCase) Then Continue While
                                                                                               Dim fullJobNumber = fields(0).Trim()
                                                                                               Dim jobNumberPrefix = fullJobNumber.Split("-"c)(0).Trim()
                                                                                               Dim plan = fields(6).Trim()
                                                                                               buildingKeysFromCsv.Add(jobNumberPrefix & "_" & plan)
                                                                                               wallRows.Add(fields)
                                                                                           End While
                                                                                       End Using
                                                                                       If wallRows.Count = 0 Then
                                                                                           summary.Log.AppendLine("No Wall rows found in CSV.")
                                                                                           MessageBox.Show("No Wall data found to import.", "Wall Import", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                                                           Return
                                                                                       End If
                                                                                       summary.TotalWallRows = wallRows.Count
                                                                                       summary.SkippedRows = skippedRows

                                                                                       ' Load existing buildings
                                                                                       Dim existingBuildings As New Dictionary(Of String, Integer)(StringComparer.OrdinalIgnoreCase)
                                                                                       Dim buildingList As New List(Of (Integer, String))
                                                                                       Using cmd As New SqlCommand(Queries.SelectBuildingsByVersionID, conn, transaction)
                                                                                           cmd.Parameters.AddWithValue("@VersionID", versionID)
                                                                                           Using reader = cmd.ExecuteReader()
                                                                                               While reader.Read()
                                                                                                   Dim id = CInt(reader("BuildingID"))
                                                                                                   Dim name = reader("BuildingName").ToString().Trim()
                                                                                                   buildingList.Add((id, name))
                                                                                                   existingBuildings(name) = id
                                                                                               End While
                                                                                           End Using
                                                                                       End Using

                                                                                       ' Interactive mapping
                                                                                       Dim mappingForm As New WallImportMappingForm(buildingKeysFromCsv, buildingList, existingBuildings, wallRows.Count)
                                                                                       If mappingForm.ShowDialog() = DialogResult.Cancel Then
                                                                                           summary.WasCancelled = True
                                                                                           Return
                                                                                       End If
                                                                                       Dim buildingIdMap As Dictionary(Of String, Integer) = mappingForm.FinalMapping
                                                                                       Dim createMissingLevels As Boolean = mappingForm.CreateMissingLevels
                                                                                       summary.BuildingMappingsApplied = buildingIdMap.Count

                                                                                       ' Process each wall row
                                                                                       Dim levelMap As New Dictionary(Of Tuple(Of Integer, Integer, String), Integer)
                                                                                       Dim levelCount, rawUnitCount, actualUnitCount, mappingCount, currentSort As Integer
                                                                                       currentSort = 1
                                                                                       Dim processedLevelUnits As New HashSet(Of String)(StringComparer.OrdinalIgnoreCase)

                                                                                       For Each fields In wallRows
                                                                                           Dim fullJobNumber = fields(0).Trim()
                                                                                           Dim jobNumberPrefix = fullJobNumber.Split("-"c)(0).Trim()
                                                                                           Dim plan = fields(6).Trim()
                                                                                           Dim buildingKey = jobNumberPrefix & "_" & plan
                                                                                           Dim elevation = fields(7).Trim()
                                                                                           Dim rawUnitName = fullJobNumber & " Wall"

                                                                                           Dim buildingID As Integer
                                                                                           If Not buildingIdMap.TryGetValue(buildingKey, buildingID) Then
                                                                                               summary.SkippedRows += 1 : Continue For
                                                                                           End If
                                                                                           If buildingID = -1 Then
                                                                                               buildingID = InsertBuildingIfNew(plan, versionID, conn, transaction, buildingIdMap, 0, 1, 1, buildingKey)
                                                                                               summary.BuildingsCreated += 1
                                                                                           End If

                                                                                           ' Duplicate check
                                                                                           Dim levelUnitKey As String = $"{buildingID}|{elevation.ToLower()}"
                                                                                           If processedLevelUnits.Contains(levelUnitKey) Then
                                                                                               summary.SkippedRows += 1
                                                                                               summary.Log.AppendLine($"Skipped duplicate: {elevation} for BuildingID {buildingID}")
                                                                                               Continue For
                                                                                           End If
                                                                                           processedLevelUnits.Add(levelUnitKey)

                                                                                           Dim levelID As Integer = 0
                                                                                           If createMissingLevels Then
                                                                                               levelID = GetOrCreateLevel(buildingID, 3, elevation, versionID, conn, transaction, levelMap, 1)
                                                                                               If levelID > 0 Then levelCount += 1
                                                                                           End If
                                                                                           If levelID = 0 Then Continue For

                                                                                           ' Use TableOperations mapper
                                                                                           Dim rawModel As RawUnitModel = TableOperations.MapRawUnitFromCsvFields(fields, rawUnitName, 3)
                                                                                           TableOperations.ApplyWallAdjustments(rawModel, fields)
                                                                                           If Not rawModel.SqFt.HasValue OrElse rawModel.SqFt.Value <= 0 Then rawModel.SqFt = 1D

                                                                                           ' Lookup cost effective date
                                                                                           Dim costEffectiveID As Object = DBNull.Value
                                                                                           If rawModel.AvgSPFNo2.HasValue Then
                                                                                               Using fetchCmd As New SqlCommand(Queries.SelectCostEffectiveDateIDByCost, conn, transaction)
                                                                                                   fetchCmd.Parameters.AddWithValue("@SPFLumberCost", rawModel.AvgSPFNo2.Value)
                                                                                                   Dim result = fetchCmd.ExecuteScalar()
                                                                                                   If result IsNot Nothing AndAlso result IsNot DBNull.Value Then costEffectiveID = result
                                                                                               End Using
                                                                                           End If

                                                                                           Dim newRawUnitID As Integer = TableOperations.InsertRawUnitWithHistory(rawModel, versionID, conn, transaction, costEffectiveID)
                                                                                           rawUnitCount += 1

                                                                                           Dim actualModel As New ActualUnitModel With {
                                                                                               .ProductTypeID = 3,
                                                                                               .UnitName = elevation,
                                                                                               .PlanSQFT = rawModel.SqFt.Value,
                                                                                               .UnitType = "Res",
                                                                                               .OptionalAdder = 1D,
                                                                                               .SortOrder = currentSort
                                                                                           }
                                                                                           Dim newActualUnitID = TableOperations.InsertActualUnit(actualModel, newRawUnitID, versionID, conn, transaction)
                                                                                           TableOperations.InsertActualToLevelMapping(newActualUnitID, levelID, 1, versionID, conn, transaction)
                                                                                           actualUnitCount += 1
                                                                                           mappingCount += 1
                                                                                           currentSort += 1

                                                                                           ' Insert calculated components
                                                                                           Dim rawData As New Dictionary(Of String, Decimal) From {
                                                                                               {"SqFt", rawModel.SqFt.GetValueOrDefault(1D)},
                                                                                               {"LF", rawModel.LF.GetValueOrDefault()},
                                                                                               {"BF", rawModel.BF.GetValueOrDefault()},
                                                                                               {"LumberCost", rawModel.LumberCost.GetValueOrDefault()},
                                                                                               {"PlateCost", rawModel.PlateCost.GetValueOrDefault()},
                                                                                               {"ManufLaborCost", rawModel.ManufLaborCost.GetValueOrDefault()},
                                                                                               {"DesignLabor", rawModel.DesignLabor.GetValueOrDefault()},
                                                                                               {"MGMTLabor", rawModel.MGMTLabor.GetValueOrDefault()},
                                                                                               {"JobSuppliesCost", rawModel.JobSuppliesCost.GetValueOrDefault()},
                                                                                               {"ManHours", rawModel.ManHours.GetValueOrDefault()},
                                                                                               {"ItemCost", rawModel.ItemCost.GetValueOrDefault()},
                                                                                               {"OverallCost", rawModel.OverallCost.GetValueOrDefault()},
                                                                                               {"TotalSellPrice", rawModel.TotalSellPrice.GetValueOrDefault()}
                                                                                           }
                                                                                           TableOperations.InsertCalculatedComponentsFromRawUnit(rawData, newActualUnitID, versionID, conn, transaction)
                                                                                       Next

                                                                                       summary.LevelsCreated = levelCount
                                                                                       summary.RawUnitsImported = rawUnitCount
                                                                                       summary.ActualUnitsImported = actualUnitCount
                                                                                       summary.MappingsCreated = mappingCount
                                                                                       transaction.Commit()
                                                                                       summary.Success = True
                                                                                   Catch ex As Exception
                                                                                       If transaction.Connection IsNot Nothing Then transaction.Rollback()
                                                                                       summary.Success = False
                                                                                       summary.Log.AppendLine("ERROR: " & ex.Message & vbCrLf & ex.StackTrace)
                                                                                       Throw
                                                                                   End Try
                                                                               End Using
                                                                           End Using
                                                                           RollupCalculationService.RecalculateVersion(versionID)
                                                                           summary.EndTime = Date.Now
                                                                           MessageBox.Show(summary.GetSummaryText(), "Wall Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                                       End Sub, "Error importing walls from CSV")
            Finally
                UIHelper.HideBusy(frmMain)
            End Try
            Return summary
        End Function

        '=====================================================================
        ' HELPER METHODS
        '=====================================================================

        Public Shared Function ParseDate(value As String) As Date?
            If String.IsNullOrEmpty(value) Then Return Nothing
            value = value.Trim()
            Dim result As Date
            If Date.TryParseExact(value, {"M/d/yy", "M.d.yy", "M-d-yy", "M/d/yyyy", "M/d/yyyy h:mm:ss tt"}, CultureInfo.InvariantCulture, DateTimeStyles.None, result) Then
                Return result
            End If
            Return Nothing
        End Function

        Private Shared Function LoadRawUnitData(rawUnitID As Integer, versionID As Integer, conn As SqlConnection, tran As SqlTransaction) As Dictionary(Of String, Decimal)
            Dim rawReader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReaderTransactional(
                "SELECT SqFt, LF, BF, LumberCost, PlateCost, ManufLaborCost, DesignLabor, MGMTLabor, JobSuppliesCost, ManHours, ItemCost, OverallCost, TotalSellPrice FROM RawUnits WHERE RawUnitID = @RawUnitID AND VersionID = @VersionID",
                {New SqlParameter("@RawUnitID", rawUnitID), New SqlParameter("@VersionID", versionID)}, conn, tran)
            If Not rawReader.Read() Then
                rawReader.Close()
                Return Nothing
            End If
            Dim data As New Dictionary(Of String, Decimal) From {
                {"SqFt", If(rawReader("SqFt") Is DBNull.Value, 0D, CDec(rawReader("SqFt")))},
                {"LF", If(rawReader("LF") Is DBNull.Value, 0D, CDec(rawReader("LF")))},
                {"BF", If(rawReader("BF") Is DBNull.Value, 0D, CDec(rawReader("BF")))},
                {"LumberCost", If(rawReader("LumberCost") Is DBNull.Value, 0D, CDec(rawReader("LumberCost")))},
                {"PlateCost", If(rawReader("PlateCost") Is DBNull.Value, 0D, CDec(rawReader("PlateCost")))},
                {"ManufLaborCost", If(rawReader("ManufLaborCost") Is DBNull.Value, 0D, CDec(rawReader("ManufLaborCost")))},
                {"DesignLabor", If(rawReader("DesignLabor") Is DBNull.Value, 0D, CDec(rawReader("DesignLabor")))},
                {"MGMTLabor", If(rawReader("MGMTLabor") Is DBNull.Value, 0D, CDec(rawReader("MGMTLabor")))},
                {"JobSuppliesCost", If(rawReader("JobSuppliesCost") Is DBNull.Value, 0D, CDec(rawReader("JobSuppliesCost")))},
                {"ManHours", If(rawReader("ManHours") Is DBNull.Value, 0D, CDec(rawReader("ManHours")))},
                {"ItemCost", If(rawReader("ItemCost") Is DBNull.Value, 0D, CDec(rawReader("ItemCost")))},
                {"OverallCost", If(rawReader("OverallCost") Is DBNull.Value, 0D, CDec(rawReader("OverallCost")))},
                {"TotalSellPrice", If(rawReader("TotalSellPrice") Is DBNull.Value, 0D, CDec(rawReader("TotalSellPrice")))}
            }
            rawReader.Close()
            Return data
        End Function

        Private Shared Function CreateProjectAndVersion(
            conn As SqlConnection, transaction As SqlTransaction,
            jbid As String, projectName As String, versionName As String, description As String,
            Optional address As String = Nothing, Optional city As String = Nothing,
            Optional state As String = Nothing, Optional zip As String = Nothing,
            Optional bidDate As Date? = Nothing, Optional archPlansDated As Date? = Nothing,
            Optional engPlansDated As Date? = Nothing, Optional milesToJobSite As Integer? = Nothing,
            Optional totalNetSqft As Integer? = Nothing, Optional totalGrossSqft As Integer? = Nothing,
            Optional projectNotes As String = Nothing, Optional customerName As String = Nothing,
            Optional customerID As Integer? = Nothing, Optional architectName As String = Nothing,
            Optional engineerName As String = Nothing, Optional estimatorName As String = Nothing,
            Optional estimatorID As Integer? = Nothing, Optional salesName As String = Nothing,
            Optional salesID As Integer? = Nothing, Optional MondayID As String = Nothing,
            Optional ProjVersionStatusID As Integer? = Nothing, Optional insertDefaultSettings As Boolean = True
        ) As Tuple(Of Integer, Integer)

            ' Resolve IDs from names
            Dim finalCustomerID As Integer? = customerID
            If Not finalCustomerID.HasValue AndAlso Not String.IsNullOrEmpty(customerName) Then
                finalCustomerID = HelperDataAccess.GetOrInsertCustomer(customerName, 1, conn, transaction)
            End If
            Dim finalArchitectID As Integer? = If(Not String.IsNullOrEmpty(architectName), HelperDataAccess.GetOrInsertCustomer(architectName, 2, conn, transaction), Nothing)
            Dim finalEngineerID As Integer? = If(Not String.IsNullOrEmpty(engineerName), HelperDataAccess.GetOrInsertCustomer(engineerName, 3, conn, transaction), Nothing)
            Dim finalEstimatorID As Integer? = estimatorID
            If Not finalEstimatorID.HasValue AndAlso Not String.IsNullOrEmpty(estimatorName) Then
                finalEstimatorID = HelperDataAccess.GetOrInsertEstimator(estimatorName, conn, transaction)
            End If
            Dim finalSalesID As Integer? = salesID
            If Not finalSalesID.HasValue AndAlso Not String.IsNullOrEmpty(salesName) Then
                finalSalesID = HelperDataAccess.GetOrInsertSales(salesName, conn, transaction)
            End If

            ' Create Project using TableOperations
            Dim projectModel As New ProjectModel With {
                .JBID = jbid, .ProjectType = New ProjectTypeModel With {.ProjectTypeID = 1},
                .ProjectName = projectName, .Estimator = If(finalEstimatorID.HasValue, New EstimatorModel With {.EstimatorID = finalEstimatorID.Value}, Nothing),
                .Address = address, .City = city, .State = state, .Zip = zip,
                .BidDate = bidDate, .ArchPlansDated = archPlansDated, .EngPlansDated = engPlansDated,
                .MilesToJobSite = If(milesToJobSite, 0), .TotalNetSqft = totalNetSqft, .TotalGrossSqft = totalGrossSqft,
                .ArchitectID = finalArchitectID, .EngineerID = finalEngineerID, .ProjectNotes = projectNotes
            }
            Dim projectID As Integer = TableOperations.InsertProject(projectModel, conn, transaction)

            ' Create Version using TableOperations
            Dim versionModel As New ProjectVersionModel With {
                .ProjectID = projectID, .VersionName = versionName, .VersionDate = Date.Now,
                .Description = description, .CustomerID = finalCustomerID, .SalesID = finalSalesID,
                .MondayID = MondayID, .ProjVersionStatusID = ProjVersionStatusID
            }
            Dim versionID As Integer = TableOperations.InsertProjectVersion(versionModel, conn, transaction)

            ' Insert default settings
            If insertDefaultSettings Then
                Dim da As New ProjectDataAccess()
                For Each pt In da.GetProductTypes()
                    Dim settingParams As New Dictionary(Of String, Object) From {
                        {"@VersionID", versionID}, {"@ProductTypeID", pt.ProductTypeID},
                        {"@MarginPercent", 0D}, {"@LumberAdder", 0D}
                    }
                    SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                        Queries.InsertProjectProductSetting, HelperDataAccess.BuildParameters(settingParams), conn, transaction)
                Next
            End If

            Return Tuple.Create(projectID, versionID)
        End Function

        Private Shared Function InsertBuildingIfNew(buildingName As String, versionID As Integer, conn As SqlConnection, trans As SqlTransaction,
            ByRef buildingMap As Dictionary(Of String, Integer), Optional resUnits As Integer? = Nothing, Optional bldgQty As Integer = 1,
            Optional buildingType As Integer = 1, Optional mapKey As String = Nothing) As Integer

            Dim key As String = If(Not String.IsNullOrEmpty(mapKey), mapKey.Trim().ToLower(), buildingName.Trim().ToLower())
            If buildingMap.ContainsKey(key) Then Return buildingMap(key)

            Dim buildingModel As New BuildingModel With {
                .BuildingName = buildingName, .BuildingType = buildingType, .ResUnits = resUnits, .BldgQty = bldgQty
            }
            Dim newBldgID As Integer = TableOperations.InsertBuilding(buildingModel, versionID, conn, trans)
            buildingMap.Add(key, newBldgID)
            Return newBldgID
        End Function

        Private Shared Function GetOrCreateLevel(buildingID As Integer, productTypeID As Integer, levelName As String, versionID As Integer,
            conn As SqlConnection, trans As SqlTransaction, ByRef levelMap As Dictionary(Of Tuple(Of Integer, Integer, String), Integer),
            Optional levelNumber As Integer? = Nothing) As Integer

            Dim cleanLevelName As String = levelName.Trim().ToLower()
            Dim key = Tuple.Create(buildingID, productTypeID, cleanLevelName)
            If levelMap.ContainsKey(key) Then Return levelMap(key)

            Dim levelModel As New LevelModel With {
                .ProductTypeID = productTypeID, .LevelNumber = If(levelNumber, 1), .LevelName = levelName
            }
            Dim newLevelID As Integer = TableOperations.InsertLevel(levelModel, buildingID, versionID, conn, trans)
            levelMap.Add(key, newLevelID)
            Return newLevelID
        End Function

        Private Shared Sub FinalizeImport(versionID As Integer, importLog As StringBuilder, trans As SqlTransaction,
            Optional successMessage As String = "Import completed successfully.")
            trans.Commit()
            RollupCalculationService.RecalculateVersion(versionID)
            MessageBox.Show(If(importLog.Length > 0, importLog.ToString(), successMessage), "Import Summary", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        Public Class TupleComparer
            Implements IEqualityComparer(Of Tuple(Of Integer, Integer, String))
            Private ReadOnly _stringComparer As StringComparer
            Public Sub New(stringComparer As StringComparer)
                _stringComparer = stringComparer
            End Sub
            Public Overloads Function Equals(x As Tuple(Of Integer, Integer, String), y As Tuple(Of Integer, Integer, String)) As Boolean Implements IEqualityComparer(Of Tuple(Of Integer, Integer, String)).Equals
                If x Is Nothing AndAlso y Is Nothing Then Return True
                If x Is Nothing OrElse y Is Nothing Then Return False
                Return x.Item1 = y.Item1 AndAlso x.Item2 = y.Item2 AndAlso _stringComparer.Equals(x.Item3, y.Item3)
            End Function
            Public Overloads Function GetHashCode(obj As Tuple(Of Integer, Integer, String)) As Integer Implements IEqualityComparer(Of Tuple(Of Integer, Integer, String)).GetHashCode
                If obj Is Nothing Then Return 0
                Return obj.Item1.GetHashCode() Xor obj.Item2.GetHashCode() Xor If(obj.Item3 Is Nothing, 0, _stringComparer.GetHashCode(obj.Item3))
            End Function
        End Class

    End Class
End Namespace