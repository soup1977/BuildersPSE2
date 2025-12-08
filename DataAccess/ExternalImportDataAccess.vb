Option Strict On
Imports System.Data.SqlClient
Imports System.Globalization
Imports System.Text
Imports System.Text.RegularExpressions
Imports BuildersPSE2.BuildersPSE.Models ' Adjust if your models are in a different namespace
Imports BuildersPSE2.Utilities
Imports Microsoft.VisualBasic.FileIO


Namespace DataAccess
    '=====================================================================
    ' Result class for Wall Import – place this at the TOP of the file
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

        ' Refactored Import PSE Spreadsheet as New Project
        Public Shared Sub ImportSpreadsheetAsNewProject(filePath As String)
            UIHelper.ShowBusy(frmMain, "Importing spreadsheet as new project...")
            Dim spreadsheetData As Dictionary(Of String, DataTable) = SpreadsheetParser.ParseSpreadsheet(filePath)
            Dim importLog As New StringBuilder()
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using transaction As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   ' Step 1: Extract project details from Summary sheet (keep extractions, but no manual lookups/inserts)
                                                                                   Dim summaryDt As DataTable = spreadsheetData("Summary")
                                                                                   Dim jbid As String = If(summaryDt.Rows(0)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(0)(1)))
                                                                                   Dim projectName As String = If(summaryDt.Rows(1)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(1)(1)))
                                                                                   Dim address As String = If(summaryDt.Rows(2)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(2)(1)))
                                                                                   Dim city As String = If(summaryDt.Rows(3)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(3)(1)))
                                                                                   Dim state As String = If(summaryDt.Rows(3)(3) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(3)(3)))
                                                                                   Dim zip As String = If(summaryDt.Rows(3)(4) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(3)(4)))
                                                                                   Dim bidDate As Date? = If(summaryDt.Rows(2)(6) Is DBNull.Value, Nothing, ParseDate(CStr(summaryDt.Rows(2)(6))))
                                                                                   Dim archPlansDated As Date? = If(summaryDt.Rows(3)(6) Is DBNull.Value, Nothing, ParseDate(CStr(summaryDt.Rows(3)(6))))
                                                                                   Dim engPlansDated As Date? = If(summaryDt.Rows(4)(6) Is DBNull.Value, Nothing, ParseDate(CStr(summaryDt.Rows(4)(6))))
                                                                                   Dim milesToJobSite As Integer? = If(summaryDt.Rows(5)(6) Is DBNull.Value, Nothing, CInt(summaryDt.Rows(5)(6)))
                                                                                   Dim totalNetSqft As Integer? = If(summaryDt.Rows(6)(1) Is DBNull.Value, Nothing, CInt(summaryDt.Rows(6)(1)))
                                                                                   Dim totalGrossSqft As Integer? = If(summaryDt.Rows(7)(1) Is DBNull.Value, Nothing, CInt(summaryDt.Rows(7)(1)))
                                                                                   Dim architectName As String = If(summaryDt.Rows(9)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(9)(1)))
                                                                                   Dim engineerName As String = If(summaryDt.Rows(8)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(8)(1)))
                                                                                   Dim customerName As String = If(summaryDt.Rows(5)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(5)(1)))
                                                                                   Dim estimatorName As String = If(summaryDt.Rows(1)(6) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(1)(6)))
                                                                                   Dim salesName As String = If(summaryDt.Rows(0)(6) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(0)(6)))

                                                                                   ' New Step: Call shared function for Project + Version insert
                                                                                   Dim versionName As String = "Imported PSE " & Date.Now.ToString("yyyyMMdd")
                                                                                   Dim versionDesc As String = "Imported from PSE spreadsheet"
                                                                                   Dim ids As Tuple(Of Integer, Integer) = CreateProjectAndVersion(
                                                                                        conn, transaction,
                                                                                        jbid, projectName, versionName, versionDesc,
                                                                                        address, city, state, zip, bidDate, archPlansDated, engPlansDated,
                                                                                        milesToJobSite, totalNetSqft, totalGrossSqft, Nothing,
                                                                                        customerName, Nothing, architectName, engineerName,
                                                                                        estimatorName, Nothing, salesName, Nothing, Nothing,
                                                                                        True
                                                                                    )
                                                                                   Dim projectID As Integer = ids.Item1
                                                                                   Dim newVersionID As Integer = ids.Item2
                                                                                   UIHelper.Add($"Project '{projectName}' created successfully with ID {projectID}.")
                                                                                   UIHelper.Add($"Project version created with ID {newVersionID}.")
                                                                                   UIHelper.Add("Inserted default project product settings.")  ' Simplified log


                                                                                   ' Step 3.2: Import RawUnits (refactored)
                                                                                   Dim rawUnitMap As New Dictionary(Of String, Integer)
                                                                                   Dim rawUnitCount As Integer = 0
                                                                                   For Each sheet In {"FloorImport", "RoofImport"}
                                                                                       If spreadsheetData.ContainsKey(sheet) Then
                                                                                           Dim dt As DataTable = spreadsheetData(sheet)
                                                                                           For Each row As DataRow In dt.Rows
                                                                                               If row("JobNumber") Is DBNull.Value Then Continue For
                                                                                               Dim productTypeID As Integer = If(sheet = "FloorImport", 1, 2)
                                                                                               Dim planName As String = If(row("Elevation") Is DBNull.Value, String.Empty, CStr(row("Elevation")))
                                                                                               Dim jobNumber As String = CStr(row("JobNumber"))
                                                                                               Dim uniqueKey As String = If(String.IsNullOrEmpty(planName), "Unknown_" & jobNumber, planName & "_" & jobNumber)

                                                                                               Dim rawModel As New RawUnitModel With {
                                                                                                    .RawUnitName = planName,
                                                                                                    .ProductTypeID = productTypeID,
                                                                                                    .BF = If(row("BF") Is DBNull.Value, Nothing, CDec(row("BF"))),
                                                                                                    .LF = If(row("LF") Is DBNull.Value, Nothing, CDec(row("LF"))),
                                                                                                    .EWPLF = If(row("EWPLF") Is DBNull.Value, Nothing, CDec(row("EWPLF"))),
                                                                                                    .SqFt = If(row("SqFt") Is DBNull.Value, Nothing, CDec(row("SqFt"))),
                                                                                                    .FCArea = If(row("FCArea") Is DBNull.Value, Nothing, CDec(row("FCArea"))),
                                                                                                    .LumberCost = If(row("LumberCost") Is DBNull.Value, Nothing, CDec(row("LumberCost"))),
                                                                                                    .PlateCost = If(row("PlateCost") Is DBNull.Value, Nothing, CDec(row("PlateCost"))),
                                                                                                    .ManufLaborCost = If(row("ManufLaborCost") Is DBNull.Value, Nothing, CDec(row("ManufLaborCost"))),
                                                                                                    .DesignLabor = If(row("DesignLabor") Is DBNull.Value, Nothing, CDec(row("DesignLabor"))),
                                                                                                    .MGMTLabor = If(row("MGMTLabor") Is DBNull.Value, Nothing, CDec(row("MGMTLabor"))),
                                                                                                    .JobSuppliesCost = If(row("JobSuppliesCost") Is DBNull.Value, Nothing, CDec(row("JobSuppliesCost"))),
                                                                                                    .ManHours = If(row("ManHours") Is DBNull.Value, Nothing, CDec(row("ManHours"))),
                                                                                                    .ItemCost = If(row("ItemCost") Is DBNull.Value, Nothing, CDec(row("ItemCost"))),
                                                                                                    .OverallCost = If(row("OverallCost") Is DBNull.Value, Nothing, CDec(row("OverallCost"))),
                                                                                                    .DeliveryCost = If(row("DeliveryCost") Is DBNull.Value, Nothing, CDec(row("DeliveryCost"))),
                                                                                                    .TotalSellPrice = If(row("TotalSellPrice") Is DBNull.Value, Nothing, CDec(row("TotalSellPrice"))),
                                                                                                    .AvgSPFNo2 = If(dt.Columns.Contains("AvgSPFNo2") AndAlso row("AvgSPFNo2") IsNot DBNull.Value, CDec(row("AvgSPFNo2")), Nothing),
                                                                                                    .SPFNo2BDFT = If(dt.Columns.Contains("spfNo2BDFT") AndAlso row("spfNo2BDFT") IsNot DBNull.Value, CDec(row("spfNo2BDFT")), Nothing),
                                                                                                    .Avg241800 = If(dt.Columns.Contains("Avg241800") AndAlso row("Avg241800") IsNot DBNull.Value, CDec(row("Avg241800")), Nothing),
                                                                                                    .MSR241800BDFT = If(dt.Columns.Contains("MSR241800BDFT") AndAlso row("MSR241800BDFT") IsNot DBNull.Value, CDec(row("MSR241800BDFT")), Nothing),
                                                                                                    .Avg242400 = If(dt.Columns.Contains("Avg242400") AndAlso row("Avg242400") IsNot DBNull.Value, CDec(row("Avg242400")), Nothing),
                                                                                                    .MSR242400BDFT = If(dt.Columns.Contains("MSR242400BDFT") AndAlso row("MSR242400BDFT") IsNot DBNull.Value, CDec(row("MSR242400BDFT")), Nothing),
                                                                                                    .Avg261800 = If(dt.Columns.Contains("Avg261800") AndAlso row("Avg261800") IsNot DBNull.Value, CDec(row("Avg261800")), Nothing),
                                                                                                    .MSR261800BDFT = If(dt.Columns.Contains("MSR261800BDFT") AndAlso row("MSR261800BDFT") IsNot DBNull.Value, CDec(row("MSR261800BDFT")), Nothing),
                                                                                                    .Avg262400 = If(dt.Columns.Contains("Avg262400") AndAlso row("Avg262400") IsNot DBNull.Value, CDec(row("Avg262400")), Nothing),
                                                                                                    .MSR262400BDFT = If(dt.Columns.Contains("MSR262400BDFT") AndAlso row("MSR262400BDFT") IsNot DBNull.Value, CDec(row("MSR262400BDFT")), Nothing)
                                                                                                }
                                                                                               Dim costEffectiveID As Object = DBNull.Value
                                                                                               If dt.Columns.Contains("AvgSPFNo2") AndAlso row("AvgSPFNo2") IsNot DBNull.Value Then
                                                                                                   Dim fetchParams As SqlParameter() = {New SqlParameter("@SPFLumberCost", row("AvgSPFNo2"))}
                                                                                                   costEffectiveID = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(Queries.SelectCostEffectiveDateIDByCost, fetchParams, conn, transaction)
                                                                                               End If
                                                                                               Dim newRawID As Integer = InsertRawUnitWithHistory(rawModel, newVersionID, conn, transaction, costEffectiveID)
                                                                                               rawUnitMap.Add(uniqueKey, newRawID)
                                                                                               rawUnitCount += 1
                                                                                           Next
                                                                                       End If
                                                                                   Next
                                                                                   UIHelper.Add($"Imported {rawUnitCount} raw units from FloorImport and RoofImport.")

                                                                                   ' Step 3.3: Import ActualUnits (refactored)
                                                                                   Dim actualUnitMap As New Dictionary(Of String, Integer)
                                                                                   Dim actualUnitSet As New HashSet(Of String)
                                                                                   Dim actualUnitCount As Integer = 0
                                                                                   For Each sheet In {"Floor Unit Data", "Roof Unit Data"}
                                                                                       If Not spreadsheetData.ContainsKey(sheet) Then Continue For
                                                                                       Dim dt As DataTable = spreadsheetData(sheet)
                                                                                       Dim productTypeID As Integer = If(sheet.Contains("Floor"), 1, 2)
                                                                                       For col As Integer = 46 To dt.Columns.Count - 1
                                                                                           Dim rawName As String = dt.Columns(col).ColumnName.Trim()
                                                                                           Dim unitName As String = If(dt.Rows(0)(col) Is DBNull.Value, String.Empty, CStr(dt.Rows(0)(col)).Trim())
                                                                                           If String.IsNullOrEmpty(unitName) OrElse unitName = "<select>" Then Continue For

                                                                                           Dim planName As String = Regex.Replace(rawName, "^Models-?", "", RegexOptions.IgnoreCase).Trim()
                                                                                           planName = Regex.Replace(planName, "[_]\d+$", "").Trim()
                                                                                           Dim importSheetName As String = If(productTypeID = 1, "FloorImport", "RoofImport")
                                                                                           Dim jobNumber As String = String.Empty
                                                                                           If spreadsheetData.ContainsKey(importSheetName) Then
                                                                                               Dim importDt As DataTable = spreadsheetData(importSheetName)
                                                                                               For Each r As DataRow In importDt.Rows
                                                                                                   If r("Elevation") Is DBNull.Value Then Continue For
                                                                                                   If CStr(r("Elevation")).Trim() = planName Then
                                                                                                       jobNumber = CStr(r("JobNumber"))
                                                                                                       Exit For
                                                                                                   End If
                                                                                               Next
                                                                                           End If
                                                                                           Dim lookupKey As String = If(String.IsNullOrEmpty(planName), "Unknown_" & jobNumber, planName & "_" & jobNumber)
                                                                                           Dim rawID As Integer = If(rawUnitMap.ContainsKey(lookupKey), rawUnitMap(lookupKey), 0)
                                                                                           If rawID = 0 Then Continue For

                                                                                           Dim planSqftObj As Object = dt.Rows(1)(col)
                                                                                           Dim planSqft As Decimal = If(planSqftObj Is DBNull.Value OrElse Not Decimal.TryParse(CStr(planSqftObj), planSqft), 0D, CDec(planSqftObj))
                                                                                           Dim unitType As String = "Res"
                                                                                           Dim unitTypeObj As Object = dt.Rows(3)(col)
                                                                                           If unitTypeObj IsNot DBNull.Value Then
                                                                                               Dim s As String = CStr(unitTypeObj).Trim()
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
                                                                                                .OptionalAdder = 1D
                                                                                            }
                                                                                           Dim newActualID As Integer = InsertActualUnitOnly(actualModel, rawID, newVersionID, conn, transaction)
                                                                                           actualUnitMap(uniqueKey) = newActualID   ' ← Store for later mapping
                                                                                           actualUnitSet.Add(uniqueCombo)
                                                                                           actualUnitCount += 1
                                                                                       Next
                                                                                   Next
                                                                                   UIHelper.Add($"Imported {actualUnitCount} actual units from Floor and Roof Unit Data.")

                                                                                   ' Step 3.4: Import CalculatedComponents (refactored – now uses ModelParams and NonQuery)
                                                                                   Dim componentCount As Integer = 0
                                                                                   Dim rawUnitData As New Dictionary(Of Integer, Dictionary(Of String, Decimal))
                                                                                   For Each actualKey As String In actualUnitMap.Keys
                                                                                       Dim actualUnitID As Integer = actualUnitMap(actualKey)
                                                                                       Dim rawUnitIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)("SELECT RawUnitID FROM ActualUnits WHERE ActualUnitID = @ActualUnitID AND VersionID = @VersionID",
                                                                                   {New SqlParameter("@ActualUnitID", actualUnitID), New SqlParameter("@VersionID", newVersionID)}.ToArray(), conn, transaction)
                                                                                       If rawUnitIDObj Is DBNull.Value Then Continue For
                                                                                       Dim rawUnitID As Integer = CInt(rawUnitIDObj)

                                                                                       If Not rawUnitData.ContainsKey(rawUnitID) Then
                                                                                           Dim rawReader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReaderTransactional("SELECT SqFt, LF, BF, LumberCost, PlateCost, ManufLaborCost, DesignLabor, MGMTLabor, JobSuppliesCost, ManHours, ItemCost, OverallCost, TotalSellPrice FROM RawUnits WHERE RawUnitID = @RawUnitID AND VersionID = @VersionID",
                                                                                       {New SqlParameter("@RawUnitID", rawUnitID), New SqlParameter("@VersionID", newVersionID)}.ToArray(), conn, transaction)
                                                                                           If Not rawReader.Read() Then
                                                                                               rawReader.Close()
                                                                                               Continue For
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
                                                                                           rawUnitData(rawUnitID) = data
                                                                                       End If

                                                                                       Dim sqFt As Decimal = rawUnitData(rawUnitID)("SqFt")
                                                                                       If sqFt <= 0 Then Continue For

                                                                                       Dim lfPerSqft As Decimal = rawUnitData(rawUnitID)("LF") / sqFt
                                                                                       Dim bdftPerSqft As Decimal = rawUnitData(rawUnitID)("BF") / sqFt
                                                                                       Dim lumberPerSqft As Decimal = rawUnitData(rawUnitID)("LumberCost") / sqFt
                                                                                       Dim platePerSqft As Decimal = rawUnitData(rawUnitID)("PlateCost") / sqFt
                                                                                       Dim manufLaborPerSqft As Decimal = rawUnitData(rawUnitID)("ManufLaborCost") / sqFt
                                                                                       Dim designLaborPerSqft As Decimal = rawUnitData(rawUnitID)("DesignLabor") / sqFt
                                                                                       Dim mgmtLaborPerSqft As Decimal = rawUnitData(rawUnitID)("MGMTLabor") / sqFt
                                                                                       Dim jobSuppliesPerSqft As Decimal = rawUnitData(rawUnitID)("JobSuppliesCost") / sqFt
                                                                                       Dim manHoursPerSqft As Decimal = rawUnitData(rawUnitID)("ManHours") / sqFt
                                                                                       Dim itemCostPerSqft As Decimal = rawUnitData(rawUnitID)("ItemCost") / sqFt
                                                                                       Dim overallCostPerSqft As Decimal = rawUnitData(rawUnitID)("OverallCost") / sqFt
                                                                                       Dim sellPerSqft As Decimal = rawUnitData(rawUnitID)("TotalSellPrice") / sqFt
                                                                                       Dim marginPerSqft As Decimal = sellPerSqft - overallCostPerSqft

                                                                                       Dim components As New List(Of Tuple(Of String, Decimal)) From {
                                                                                   Tuple.Create("LF/SQFT", lfPerSqft),
                                                                                   Tuple.Create("BDFT/SQFT", bdftPerSqft),
                                                                                   Tuple.Create("Lumber/SQFT", lumberPerSqft),
                                                                                   Tuple.Create("Plate/SQFT", platePerSqft),
                                                                                   Tuple.Create("ManufLabor/SQFT", manufLaborPerSqft),
                                                                                   Tuple.Create("DesignLabor/SQFT", designLaborPerSqft),
                                                                                   Tuple.Create("MGMTLabor/SQFT", mgmtLaborPerSqft),
                                                                                   Tuple.Create("JobSupplies/SQFT", jobSuppliesPerSqft),
                                                                                   Tuple.Create("ManHours/SQFT", manHoursPerSqft),
                                                                                   Tuple.Create("ItemCost/SQFT", itemCostPerSqft),
                                                                                   Tuple.Create("OverallCost/SQFT", overallCostPerSqft),
                                                                                   Tuple.Create("SellPrice/SQFT", sellPerSqft),
                                                                                   Tuple.Create("Margin/SQFT", marginPerSqft)
                                                                               }

                                                                                       For Each comp In components
                                                                                           Dim compModel As New CalculatedComponentModel With {
                                                                                       .ComponentType = comp.Item1,
                                                                                       .Value = comp.Item2
                                                                                   }
                                                                                           Dim compParams = ModelParams.ForComponent(compModel, newVersionID, actualUnitID)
                                                                                           SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.InsertCalculatedComponent, HelperDataAccess.BuildParameters(compParams), conn, transaction)
                                                                                           componentCount += 1
                                                                                       Next
                                                                                   Next
                                                                                   UIHelper.Add($"Imported {componentCount} calculated components.")

                                                                                   ' Step 3.5: Import Buildings using shared function
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
                                                                                           If newBldgID > 0 Then buildingCount += 1 ' Only count new inserts
                                                                                       Next
                                                                                   End If
                                                                                   UIHelper.Add($"Imported {buildingCount} buildings from Buildings sheet.")

                                                                                   ' Step 3.6: Import Levels using shared function
                                                                                   Dim levelMap As New Dictionary(Of Tuple(Of Integer, Integer, String), Integer)(
    New TupleComparer(StringComparer.OrdinalIgnoreCase))
                                                                                   Dim levelCount As Integer = 0
                                                                                   For Each sheet In {"Floor Unit Data", "Roof Unit Data"}
                                                                                       If spreadsheetData.ContainsKey(sheet) Then
                                                                                           Dim dt As DataTable = spreadsheetData(sheet)
                                                                                           Dim productTypeID As Integer = If(sheet.Contains("Floor"), 1, 2)
                                                                                           For rowIdx As Integer = 33 To Math.Min(dt.Rows.Count - 1, 68)
                                                                                               Dim buildingName As String = If(dt.Rows(rowIdx)(0) Is DBNull.Value, String.Empty, CStr(dt.Rows(rowIdx)(0)).Trim())
                                                                                               If String.IsNullOrEmpty(buildingName) OrElse Not buildingMap.ContainsKey(buildingName) Then Continue For
                                                                                               Dim levelName As String = If(dt.Rows(rowIdx)(1) Is DBNull.Value OrElse String.IsNullOrEmpty(CStr(dt.Rows(rowIdx)(1)).Trim()), If(productTypeID = 2, "roof", String.Empty), CStr(dt.Rows(rowIdx)(1)).Trim())
                                                                                               If String.IsNullOrEmpty(levelName) Then Continue For
                                                                                               Dim buildingID As Integer = buildingMap(buildingName)
                                                                                               Dim newLevelID As Integer = GetOrCreateLevel(buildingID, productTypeID, levelName, newVersionID, conn, transaction, levelMap, rowIdx - 32)
                                                                                               If newLevelID > 0 Then levelCount += 1 ' Only count new inserts
                                                                                           Next
                                                                                       End If
                                                                                   Next
                                                                                   UIHelper.Add($"Imported {levelCount} levels for buildings.")

                                                                                   ' Step 3.7: Import ActualToLevelMappings (refactored)
                                                                                   Dim mappingCount As Integer = 0
                                                                                   For Each sheet In {"Floor Unit Data", "Roof Unit Data"}
                                                                                       If spreadsheetData.ContainsKey(sheet) Then
                                                                                           Dim dt As DataTable = spreadsheetData(sheet)
                                                                                           Dim productTypeID As Integer = If(sheet.Contains("Floor"), 1, 2)
                                                                                           For rowIdx As Integer = 33 To dt.Rows.Count - 1
                                                                                               Dim buildingName As String = If(dt.Rows(rowIdx)(0) Is DBNull.Value, String.Empty, CStr(dt.Rows(rowIdx)(0)).Trim())
                                                                                               If String.IsNullOrEmpty(buildingName) OrElse Not buildingMap.ContainsKey(buildingName) Then Continue For
                                                                                               Dim levelName As String = If(dt.Rows(rowIdx)(1) Is DBNull.Value OrElse String.IsNullOrEmpty(CStr(dt.Rows(rowIdx)(1)).Trim()), If(productTypeID = 2, "roof", String.Empty), CStr(dt.Rows(rowIdx)(1)).Trim())
                                                                                               If String.IsNullOrEmpty(levelName) Then Continue For
                                                                                               Dim buildingID As Integer = buildingMap(buildingName)
                                                                                               Dim levelKey As Tuple(Of Integer, Integer, String) = Tuple.Create(buildingID, productTypeID, levelName)
                                                                                               Dim levelID As Integer = If(levelMap.ContainsKey(levelKey), levelMap(levelKey), 0)
                                                                                               If levelID = 0 Then Continue For

                                                                                               For colIdx As Integer = 46 To dt.Columns.Count - 1
                                                                                                   Dim qtyObj As Object = dt.Rows(rowIdx)(colIdx)
                                                                                                   If qtyObj Is DBNull.Value OrElse String.IsNullOrWhiteSpace(CStr(qtyObj)) OrElse (IsNumeric(qtyObj) AndAlso CInt(qtyObj) <= 0) Then Continue For
                                                                                                   Dim rawUnitName As String = If(dt.Rows(0)(colIdx) Is DBNull.Value, String.Empty, CStr(dt.Rows(0)(colIdx))).Trim()
                                                                                                   Dim unitName As String = Regex.Replace(rawUnitName, "actual$", "", RegexOptions.IgnoreCase).Trim()
                                                                                                   If String.IsNullOrEmpty(unitName) Then Continue For
                                                                                                   Dim planSqftObj As Object = dt.Rows(1)(colIdx)
                                                                                                   Dim planSqft As String = If(planSqftObj Is DBNull.Value, "0", CStr(planSqftObj))
                                                                                                   Dim actualUnitID As Integer = 0
                                                                                                   For Each key As String In actualUnitMap.Keys
                                                                                                       If key.StartsWith(unitName & "_" & planSqft & "_") Then
                                                                                                           actualUnitID = actualUnitMap(key)
                                                                                                           Exit For
                                                                                                       End If
                                                                                                   Next
                                                                                                   If actualUnitID = 0 Then Continue For

                                                                                                   InsertActualToLevelMapping(
                                                                                                        actualUnitID:=actualUnitID,
                                                                                                        levelID:=levelID,
                                                                                                        quantity:=CInt(qtyObj),
                                                                                                        versionID:=newVersionID,
                                                                                                        conn:=conn,
                                                                                                        trans:=transaction
                                                                                                    )
                                                                                                   mappingCount += 1
                                                                                               Next
                                                                                           Next
                                                                                       End If
                                                                                   Next
                                                                                   UIHelper.Add($"Imported {mappingCount} actual-to-level mappings.")

                                                                                   ' Commit and recalculate
                                                                                   FinalizeImport(newVersionID, importLog, transaction)
                                                                               Catch ex As Exception
                                                                                   transaction.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error importing spreadsheet as new project")
        End Sub

        ' Refactored Imports a project from a CSV file exported from MGMT software.
        Public Shared Function ImportProjectFromCSV(csvFilePath As String, projectName As String, customerName As String,
                                           Optional estimatorID As Integer? = Nothing, Optional salesID As Integer? = Nothing,
                                           Optional address As String = Nothing, Optional city As String = Nothing,
                                           Optional state As String = Nothing, Optional zip As String = Nothing,
                                           Optional biddate As Date? = Nothing, Optional archdate As Date? = Nothing,
                                           Optional engdate As Date? = Nothing, Optional miles As Integer? = Nothing) As Integer

            Dim newProjectID As Integer = 0
            Dim importLog As New StringBuilder()

            '   SqlConnectionManager.Instance.ExecuteWithErrorHandling(
            '  Sub()
            Using conn = SqlConnectionManager.Instance.GetConnection()
                Using transaction As SqlTransaction = conn.BeginTransaction()
                    Try
                        Dim fileName = System.IO.Path.GetFileNameWithoutExtension(csvFilePath)
                        Dim projectNumber = fileName.Split("-"c)(0).Trim()

                        ' New Step: Call shared function for Project + Version insert
                        Dim versionName As String = "Imported from MGMT " & Now.ToString("yyyyMMdd")
                        Dim versionDesc As String = "Imported from MGMT full model Estimate"
                        Dim ids As Tuple(Of Integer, Integer) = CreateProjectAndVersion(
                                conn, transaction,
                                projectNumber, projectName, versionName, versionDesc,
                                address, city, state, zip, biddate, archdate, engdate,
                                miles, 0, 0, Nothing,
                                customerName, Nothing, Nothing, Nothing,
                                Nothing, estimatorID, Nothing, salesID, Nothing,
                                False
                            )
                        newProjectID = ids.Item1
                        Dim versionID As Integer = ids.Item2
                        ' Optional: Add logging for consistency
                        UIHelper.Add($"Project '{projectName}' created successfully with ID {newProjectID}.")
                        UIHelper.Add($"Project version created with ID {versionID}.")

                        ' === Parse CSV ===
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

                        ' === Insert Buildings using shared function ===
                        Dim buildingIdMap As New Dictionary(Of String, Integer)
                        For Each key In buildingKeys
                            Dim planName = key.Split("_"c)(1)
                            Dim buildingID As Integer = InsertBuildingIfNew(planName, versionID, conn, transaction, buildingIdMap,,,, mapKey:=key)
                        Next

                        ' === Process Rows ===
                        Dim levelMap As New Dictionary(Of Tuple(Of Integer, Integer, String), Integer) ' Rename from levelIdMap for consistency

                        For Each fields In rows
                            Dim product = fields(1).Trim()
                            Dim productTypeID = If(product = "Floor", 1, If(product = "Roof", 2, If(product = "Wall", 3, 0)))
                            If productTypeID = 0 Then Continue For

                            Dim prefix = fields(0).Trim().Split("-"c)(0).Trim()
                            Dim plan = fields(6).Trim()
                            Dim buildingKey = (prefix & "_" & plan).ToLower
                            Dim elevation = fields(7).Trim()
                            Dim rawUnitName = fields(0).Trim() & " " & product
                            Dim newRawUnitID As Integer
                            Dim buildingID As Integer = buildingIdMap(buildingKey)

                            Dim sqFt As Decimal = If(Decimal.TryParse(fields(11), sqFt), sqFt, 1D)
                            If sqFt <= 0 Then sqFt = 1D

                            ' Parse all nullable decimals safely
                            Dim bf As Decimal? = ParseDecimal(fields(8))
                            Dim lf As Decimal? = ParseDecimal(fields(9))
                            Dim ewplf As Decimal? = ParseDecimal(fields(10))
                            Dim fcArea As Decimal? = ParseDecimal(fields(12))
                            Dim lumberCost As Decimal? = ParseDecimal(fields(13))
                            Dim plateCost As Decimal? = ParseDecimal(fields(14))
                            Dim manufLaborCost As Decimal? = ParseDecimal(fields(15))
                            Dim designLabor As Decimal? = ParseDecimal(fields(16))
                            Dim mgmtLabor As Decimal? = ParseDecimal(fields(17))
                            Dim jobSuppliesCost As Decimal? = ParseDecimal(fields(18))
                            Dim manHours As Decimal? = ParseDecimal(fields(19))
                            Dim itemCost As Decimal? = ParseDecimal(fields(20))
                            Dim overallCost As Decimal? = ParseDecimal(fields(21))
                            Dim deliveryCost As Decimal? = ParseDecimal(fields(22))
                            Dim totalSellPrice As Decimal? = ParseDecimal(fields(23))
                            Dim avgSPFNo2 As Decimal? = ParseDecimal(fields(24))
                            Dim spfNo2BDFT As Decimal? = ParseDecimal(fields(25))
                            Dim avg241800 As Decimal? = ParseDecimal(fields(26))
                            Dim msr241800BDFT As Decimal? = ParseDecimal(fields(27))
                            Dim avg242400 As Decimal? = ParseDecimal(fields(28))
                            Dim msr242400BDFT As Decimal? = ParseDecimal(fields(29))
                            Dim avg261800 As Decimal? = ParseDecimal(fields(30))
                            Dim msr261800BDFT As Decimal? = ParseDecimal(fields(31))
                            Dim avg262400 As Decimal? = ParseDecimal(fields(32))
                            Dim msr262400BDFT As Decimal? = ParseDecimal(fields(33))
                            Dim vtpCost As Decimal? = If(fields.Length > 37, ParseDecimal(fields(37)), 0D)
                            Dim vtpBdft As Decimal? = If(fields.Length > 38, ParseDecimal(fields(38)), 0D)
                            Dim miscMaterialCost As Decimal? = If(fields.Length > 39, ParseDecimal(fields(39)), 0D)


                            ' === Wall-Panel-specific adjustments (only touch these two fields) ===
                            If fields.Length > 1 AndAlso String.Equals(fields(1).Trim(), "Wall", StringComparison.OrdinalIgnoreCase) Then
                                lumberCost = lumberCost.GetValueOrDefault(0D) + vtpCost.GetValueOrDefault(0D)
                                bf = bf.GetValueOrDefault(0D) + vtpBdft.GetValueOrDefault(0D)
                                spfNo2BDFT = spfNo2BDFT.GetValueOrDefault(0D) + bf.GetValueOrDefault(0D) + vtpBdft.GetValueOrDefault(0D)
                                itemCost = miscMaterialCost.GetValueOrDefault(0D)
                            End If


                            ' === Level ===
                            ' === Level using shared function ===
                            Dim cleanElevation As String = If(String.IsNullOrEmpty(elevation), "default", elevation.ToLower())

                            Dim levelID As Integer = GetOrCreateLevel(buildingID, productTypeID, cleanElevation, versionID, conn, transaction, levelMap)



                            ' === RawUnit – now 100% complete via ModelParams ===
                            Dim rawModel As New RawUnitModel() With {
                                .RawUnitName = rawUnitName,
                                .ProductTypeID = productTypeID,
                                .BF = bf, .LF = lf, .EWPLF = ewplf, .SqFt = sqFt, .FCArea = fcArea,
                                .LumberCost = lumberCost, .PlateCost = plateCost,
                                .ManufLaborCost = manufLaborCost, .DesignLabor = designLabor,
                                .MGMTLabor = mgmtLabor, .JobSuppliesCost = jobSuppliesCost,
                                .ManHours = manHours, .ItemCost = itemCost,
                                .OverallCost = overallCost, .DeliveryCost = deliveryCost,
                                .TotalSellPrice = totalSellPrice,
                                .AvgSPFNo2 = avgSPFNo2,
                                .SPFNo2BDFT = spfNo2BDFT,
                                .Avg241800 = avg241800, .MSR241800BDFT = msr241800BDFT,
                                .Avg242400 = avg242400, .MSR242400BDFT = msr242400BDFT,
                                .Avg261800 = avg261800, .MSR261800BDFT = msr261800BDFT,
                                .Avg262400 = avg262400, .MSR262400BDFT = msr262400BDFT
                            }
                            newRawUnitID = InsertRawUnitWithHistory(rawModel, versionID, conn, transaction)

                            ' === ActualUnit with ColorCode fix ===
                            Dim newActualUnitID As Integer

                            Dim actualModel As New ActualUnitModel With {
                                .ProductTypeID = productTypeID,
                                .UnitName = elevation,
                                .PlanSQFT = sqFt,
                                .UnitType = "Res",
                                .OptionalAdder = 1D
                            }
                            newActualUnitID = InsertActualUnitAndMapping(actualModel, newRawUnitID, levelID, 1, versionID, conn, transaction, "")
                        Next

                        ' === Finalize ===
                        FinalizeImport(versionID, importLog, transaction)
                    Catch ex As Exception
                        transaction.Rollback()
                        Throw
                    End Try
                End Using
            End Using
            '  End Sub,
            '  "Error importing project from CSV")

            Return newProjectID
        End Function

        ' Refactored imports of Wall Panel data from CSV file exported from MGMT software
        Public Shared Function ImportWallsInteractive(csvFilePath As String, versionID As Integer) As ImportWallSummary
            Dim summary As New ImportWallSummary With {
        .VersionID = versionID,
        .StartTime = Date.Now
    }
            SqlConnectionManager.CloseAllDataReaders()
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using transaction As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   ' === 1. Parse CSV - Wall rows only ===
                                                                                   Dim wallRows As New List(Of String())
                                                                                   Dim buildingKeysFromCsv As New HashSet(Of String)
                                                                                   Dim skippedRows As Integer = 0
                                                                                   Using parser As New TextFieldParser(csvFilePath)
                                                                                       parser.TextFieldType = FieldType.Delimited
                                                                                       parser.SetDelimiters(",")
                                                                                       parser.ReadLine() ' Skip header
                                                                                       While Not parser.EndOfData
                                                                                           Dim fields As String() = parser.ReadFields()
                                                                                           If fields.Length < 26 Then
                                                                                               skippedRows += 1 : Continue While
                                                                                           End If
                                                                                           ' Skip any accidental header repeats
                                                                                           If fields(0).Trim().Equals("JobNumber", StringComparison.OrdinalIgnoreCase) OrElse
                                                                                       fields(1).Trim().Equals("Product", StringComparison.OrdinalIgnoreCase) Then
                                                                                               Continue While
                                                                                           End If
                                                                                           If Not String.Equals(fields(1).Trim(), "Wall", StringComparison.OrdinalIgnoreCase) Then
                                                                                               Continue While
                                                                                           End If
                                                                                           Dim fullJobNumber = fields(0).Trim()
                                                                                           Dim jobNumberPrefix = fullJobNumber.Split("-"c)(0).Trim()
                                                                                           Dim plan = fields(6).Trim()
                                                                                           Dim buildingKey = jobNumberPrefix & "_" & plan
                                                                                           buildingKeysFromCsv.Add(buildingKey)
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
                                                                                   ' === 2. Load existing buildings (unchanged) ===
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
                                                                                   ' === 3. Interactive mapping ===
                                                                                   Dim mappingForm As New WallImportMappingForm(buildingKeysFromCsv, buildingList, existingBuildings, wallRows.Count)
                                                                                   If mappingForm.ShowDialog() = DialogResult.Cancel Then
                                                                                       summary.WasCancelled = True
                                                                                       Return
                                                                                   End If
                                                                                   Dim buildingIdMap As Dictionary(Of String, Integer) = mappingForm.FinalMapping
                                                                                   Dim createMissingLevels As Boolean = mappingForm.CreateMissingLevels
                                                                                   summary.BuildingMappingsApplied = buildingIdMap.Count
                                                                                   ' === 4. Process each wall row ===
                                                                                   Dim levelMap As New Dictionary(Of Tuple(Of Integer, Integer, String), Integer)
                                                                                   Dim levelCount As Integer = 0
                                                                                   Dim rawUnitCount As Integer = 0
                                                                                   Dim actualUnitCount As Integer = 0
                                                                                   Dim mappingCount As Integer = 0
                                                                                   For Each fields In wallRows
                                                                                       Dim fullJobNumber = fields(0).Trim()
                                                                                       Dim jobNumberPrefix = fullJobNumber.Split("-"c)(0).Trim()
                                                                                       Dim plan = fields(6).Trim()
                                                                                       Dim buildingKey = jobNumberPrefix & "_" & plan
                                                                                       Dim elevation = fields(7).Trim()
                                                                                       Dim rawUnitName = fullJobNumber & " Wall"
                                                                                       ' Resolve building
                                                                                       Dim buildingID As Integer
                                                                                       If Not buildingIdMap.TryGetValue(buildingKey, buildingID) Then
                                                                                           summary.SkippedRows += 1 : Continue For
                                                                                       End If
                                                                                       ' Create new building if requested (-1)
                                                                                       If buildingID = -1 Then
                                                                                           buildingID = InsertBuildingIfNew(plan, versionID, conn, transaction, buildingIdMap, 0, 1,, mapKey:=buildingKey)
                                                                                           summary.BuildingsCreated += 1
                                                                                       End If
                                                                                       ' Level: BuildingID + Elevation + ProductTypeID=3 (Wall)
                                                                                       Dim levelID As Integer = 0
                                                                                       If createMissingLevels Then
                                                                                           levelID = GetOrCreateLevel(buildingID, 3, elevation, versionID, conn, transaction, levelMap, 1)
                                                                                           If levelID > 0 Then levelCount += 1
                                                                                       End If
                                                                                       If levelID = 0 Then Continue For ' Skip if no level created and not existing
                                                                                       ' Safe SqFt
                                                                                       Dim sqFt As Decimal = If(Decimal.TryParse(fields(11), sqFt), sqFt, 1D)
                                                                                       If sqFt <= 0 Then sqFt = 1D

                                                                                       ' === NEW: Parse the two extra columns we need for Walls only ===
                                                                                       Dim vtpCost As Decimal = 0D
                                                                                       Dim vtpBdft As Decimal = 0D
                                                                                       Dim miscMaterialCost As Decimal = 0D

                                                                                       If fields.Length > 37 AndAlso Decimal.TryParse(fields(37), miscMaterialCost) Then
                                                                                           ' parsed successfully → keep value
                                                                                       Else
                                                                                           vtpCost = 0D
                                                                                       End If

                                                                                       If fields.Length > 38 AndAlso Decimal.TryParse(fields(38), vtpBdft) Then ' Column AL = VTPBDFT
                                                                                           ' parsed successfully → keep value
                                                                                       Else
                                                                                           vtpBdft = 0D
                                                                                       End If

                                                                                       If fields.Length > 39 AndAlso Decimal.TryParse(fields(39), miscMaterialCost) Then ' Column AM = MiscMaterialCost
                                                                                           ' parsed successfully → keep value
                                                                                       Else
                                                                                           miscMaterialCost = 0D
                                                                                       End If


                                                                                       ' === FULLY POPULATED RawUnitModel using exact CSV columns ===
                                                                                       Dim rawModel As New RawUnitModel() With {
                                                                                            .RawUnitName = rawUnitName,
                                                                                            .ProductTypeID = 3, ' Wall
                                                                                            .SqFt = sqFt
                                                                                        }
                                                                                       ' Parse all cost/quantity fields from CSV (standard MiTek wall export order)
                                                                                       Dim temp As Decimal
                                                                                       If Decimal.TryParse(fields(8), temp) Then rawModel.BF = temp
                                                                                       If Decimal.TryParse(fields(9), temp) Then rawModel.LF = temp
                                                                                       If Decimal.TryParse(fields(10), temp) Then rawModel.EWPLF = temp
                                                                                       If Decimal.TryParse(fields(11), temp) Then rawModel.SqFt = temp
                                                                                       If Decimal.TryParse(fields(12), temp) Then rawModel.FCArea = temp
                                                                                       If Decimal.TryParse(fields(13), temp) Then rawModel.LumberCost = temp
                                                                                       If Decimal.TryParse(fields(14), temp) Then rawModel.PlateCost = temp
                                                                                       If Decimal.TryParse(fields(15), temp) Then rawModel.ManufLaborCost = temp
                                                                                       If Decimal.TryParse(fields(16), temp) Then rawModel.DesignLabor = temp
                                                                                       If Decimal.TryParse(fields(17), temp) Then rawModel.MGMTLabor = temp
                                                                                       If Decimal.TryParse(fields(18), temp) Then rawModel.JobSuppliesCost = temp
                                                                                       If Decimal.TryParse(fields(19), temp) Then rawModel.ManHours = temp
                                                                                       If Decimal.TryParse(fields(20), temp) Then rawModel.ItemCost = temp
                                                                                       If Decimal.TryParse(fields(21), temp) Then rawModel.OverallCost = temp
                                                                                       If Decimal.TryParse(fields(22), temp) Then rawModel.DeliveryCost = temp
                                                                                       If Decimal.TryParse(fields(23), temp) Then rawModel.TotalSellPrice = temp
                                                                                       If Decimal.TryParse(fields(24), temp) Then rawModel.AvgSPFNo2 = temp
                                                                                       If Decimal.TryParse(fields(25), temp) Then rawModel.SPFNo2BDFT = temp
                                                                                       If Decimal.TryParse(fields(26), temp) Then rawModel.Avg241800 = temp
                                                                                       If Decimal.TryParse(fields(27), temp) Then rawModel.MSR241800BDFT = temp
                                                                                       If Decimal.TryParse(fields(28), temp) Then rawModel.Avg242400 = temp
                                                                                       If Decimal.TryParse(fields(29), temp) Then rawModel.MSR242400BDFT = temp
                                                                                       If Decimal.TryParse(fields(30), temp) Then rawModel.Avg261800 = temp
                                                                                       If Decimal.TryParse(fields(31), temp) Then rawModel.MSR261800BDFT = temp
                                                                                       If Decimal.TryParse(fields(32), temp) Then rawModel.Avg262400 = temp
                                                                                       If Decimal.TryParse(fields(33), temp) Then rawModel.MSR262400BDFT = temp

                                                                                       ' === WALL-ONLY ADJUSTMENTS (same business rule as main import) ===
                                                                                       rawModel.LumberCost = rawModel.LumberCost.GetValueOrDefault(0D) + vtpCost
                                                                                       rawModel.BF = rawModel.BF.GetValueOrDefault(0D) + vtpBdft
                                                                                       rawModel.SPFNo2BDFT = rawModel.SPFNo2BDFT.GetValueOrDefault(0D) + rawModel.BF.GetValueOrDefault(0D) + vtpBdft
                                                                                       rawModel.ItemCost = miscMaterialCost


                                                                                       ' Insert RawUnit with history
                                                                                       Dim costEffectiveID As Object = DBNull.Value
                                                                                       Using fetchCmd As New SqlCommand(Queries.SelectCostEffectiveDateIDByCost, conn, transaction)
                                                                                           fetchCmd.Parameters.AddWithValue("@SPFLumberCost", rawModel.LumberCost.GetValueOrDefault())
                                                                                           Dim result = fetchCmd.ExecuteScalar()
                                                                                           If result IsNot Nothing Then costEffectiveID = result
                                                                                       End Using
                                                                                       Dim newRawUnitID As Integer = InsertRawUnitWithHistory(rawModel, versionID, conn, transaction, costEffectiveID)
                                                                                       rawUnitCount += 1


                                                                                       ' === ActualUnit ===
                                                                                       Dim actualModel As New ActualUnitModel With {
                                                                                            .ProductTypeID = 3,
                                                                                            .UnitName = elevation,
                                                                                            .PlanSQFT = sqFt,
                                                                                            .UnitType = "Res",
                                                                                            .OptionalAdder = 1D
                                                                                        }
                                                                                       Dim newActualUnitID As Integer = InsertActualUnitAndMapping(actualModel, newRawUnitID, levelID, 1, versionID, conn, transaction, Nothing)
                                                                                       actualUnitCount += 1
                                                                                       ' === CalculatedComponents per SQFT ===
                                                                                       Dim sqFtRaw As Decimal = rawModel.SqFt.GetValueOrDefault(1D)
                                                                                       If sqFtRaw > 0 Then
                                                                                           Dim values As Decimal() = {
                                                                                                rawModel.LF.GetValueOrDefault(),
                                                                                                rawModel.BF.GetValueOrDefault(),
                                                                                                rawModel.LumberCost.GetValueOrDefault(),
                                                                                                rawModel.PlateCost.GetValueOrDefault(),
                                                                                                rawModel.ManufLaborCost.GetValueOrDefault(),
                                                                                                rawModel.DesignLabor.GetValueOrDefault(),
                                                                                                rawModel.MGMTLabor.GetValueOrDefault(),
                                                                                                rawModel.JobSuppliesCost.GetValueOrDefault(),
                                                                                                rawModel.ManHours.GetValueOrDefault(),
                                                                                                rawModel.ItemCost.GetValueOrDefault(),
                                                                                                rawModel.OverallCost.GetValueOrDefault(),
                                                                                                rawModel.TotalSellPrice.GetValueOrDefault()
                                                                                            }
                                                                                           Dim types As String() = {
                                                                                                "LF/SQFT", "BDFT/SQFT", "Lumber/SQFT", "Plate/SQFT",
                                                                                                "ManufLabor/SQFT", "DesignLabor/SQFT", "MGMTLabor/SQFT", "JobSupplies/SQFT",
                                                                                                "ManHours/SQFT", "ItemCost/SQFT", "OverallCost/SQFT", "SellPrice/SQFT"
                                                                                            }
                                                                                           For i = 0 To types.Length - 1
                                                                                               Dim compModel As New CalculatedComponentModel With {
                                                                                                    .ComponentType = types(i),
                                                                                                    .Value = values(i) / sqFtRaw
                                                                                                }
                                                                                               Dim compParams = ModelParams.ForComponent(compModel, versionID, newActualUnitID)
                                                                                               SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.InsertCalculatedComponent, HelperDataAccess.BuildParameters(compParams), conn, transaction)
                                                                                           Next
                                                                                           Dim margin As Decimal = If(values(11) > 0, (values(11) - values(10)) / sqFtRaw, 0D)
                                                                                           Dim marginModel As New CalculatedComponentModel With {
                                                                                                .ComponentType = "Margin/SQFT",
                                                                                                .Value = margin
                                                                                            }
                                                                                           Dim marginParams = ModelParams.ForComponent(marginModel, versionID, newActualUnitID)
                                                                                           SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.InsertCalculatedComponent, HelperDataAccess.BuildParameters(marginParams), conn, transaction)
                                                                                       End If
                                                                                   Next
                                                                                   summary.LevelsCreated = levelCount
                                                                                   summary.RawUnitsImported = rawUnitCount
                                                                                   summary.ActualUnitsImported = actualUnitCount
                                                                                   summary.MappingsCreated = mappingCount
                                                                                   transaction.Commit()
                                                                                   RollupDataAccess.RecalculateVersion(versionID)
                                                                                   summary.Success = True
                                                                                   summary.EndTime = Date.Now
                                                                               Catch ex As Exception
                                                                                   transaction.Rollback()
                                                                                   summary.Success = False
                                                                                   summary.Log.AppendLine("ERROR: " & ex.Message & vbCrLf & ex.StackTrace)
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                       MessageBox.Show(summary.GetSummaryText(), "Wall Import Complete", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                                   End Sub, "Error importing walls from CSV")
            Return summary
        End Function


        Public Shared Function ParseDate(value As String) As Date?
            If String.IsNullOrEmpty(value) Then Return Nothing
            value = value.Trim()
            Dim result As Date
            If Date.TryParseExact(value, New String() {"M/d/yy", "M.d.yy", "M-d-yy", "M/d/yyyy", "M/d/yyyy h:mm:ss tt"}, CultureInfo.InvariantCulture, DateTimeStyles.None, result) Then
                Return result
            End If
            Return Nothing
        End Function

        ' Helper to avoid repeating TryParse
        Private Shared Function ParseDecimal(s As String) As Decimal?
            Dim val As Decimal
            If Decimal.TryParse(s, val) Then Return val
            Return Nothing
        End Function


        Private Shared Function CreateProjectAndVersion(
    ByVal conn As SqlConnection,
    ByVal transaction As SqlTransaction,
    ByVal jbid As String,
    ByVal projectName As String,
    ByVal versionName As String,
    ByVal description As String,
    Optional ByVal address As String = Nothing,
    Optional ByVal city As String = Nothing,
    Optional ByVal state As String = Nothing,
    Optional ByVal zip As String = Nothing,
    Optional ByVal bidDate As Date? = Nothing,
    Optional ByVal archPlansDated As Date? = Nothing,
    Optional ByVal engPlansDated As Date? = Nothing,
    Optional ByVal milesToJobSite As Integer? = Nothing,
    Optional ByVal totalNetSqft As Integer? = Nothing,
    Optional ByVal totalGrossSqft As Integer? = Nothing,
    Optional ByVal projectNotes As String = Nothing,
    Optional ByVal customerName As String = Nothing,
    Optional ByVal customerID As Integer? = Nothing,
    Optional ByVal architectName As String = Nothing,
    Optional ByVal engineerName As String = Nothing,
    Optional ByVal estimatorName As String = Nothing,
    Optional ByVal estimatorID As Integer? = Nothing,
    Optional ByVal salesName As String = Nothing,
    Optional ByVal salesID As Integer? = Nothing,
    Optional ByVal MondayID As String = Nothing,
    Optional ByVal insertDefaultSettings As Boolean = True
) As Tuple(Of Integer, Integer)

            ' Resolve IDs from names if not provided
            Dim finalCustomerID As Integer? = customerID
            If Not finalCustomerID.HasValue AndAlso Not String.IsNullOrEmpty(customerName) Then
                finalCustomerID = HelperDataAccess.GetOrInsertCustomer(customerName, 1, conn, transaction)
            End If

            Dim finalArchitectID As Integer? = Nothing
            If Not String.IsNullOrEmpty(architectName) Then
                finalArchitectID = HelperDataAccess.GetOrInsertCustomer(architectName, 2, conn, transaction)
            End If

            Dim finalEngineerID As Integer? = Nothing
            If Not String.IsNullOrEmpty(engineerName) Then
                finalEngineerID = HelperDataAccess.GetOrInsertCustomer(engineerName, 3, conn, transaction)
            End If

            Dim finalEstimatorID As Integer? = estimatorID
            If Not finalEstimatorID.HasValue AndAlso Not String.IsNullOrEmpty(estimatorName) Then
                finalEstimatorID = HelperDataAccess.GetOrInsertEstimator(estimatorName, conn, transaction)
            End If

            Dim finalSalesID As Integer? = salesID
            If Not finalSalesID.HasValue AndAlso Not String.IsNullOrEmpty(salesName) Then
                finalSalesID = HelperDataAccess.GetOrInsertSales(salesName, conn, transaction)
            End If

            ' Create and insert Project
            Dim projectModel As New ProjectModel With {
        .JBID = jbid,
        .ProjectType = New ProjectTypeModel With {.ProjectTypeID = 1},
        .ProjectName = projectName,
        .Estimator = If(finalEstimatorID.HasValue, New EstimatorModel With {.EstimatorID = finalEstimatorID.Value}, Nothing),
        .Address = address,
        .City = city,
        .State = state,
        .Zip = zip,
        .BidDate = bidDate,
        .ArchPlansDated = archPlansDated,
        .EngPlansDated = engPlansDated,
        .MilesToJobSite = If(milesToJobSite, 0),
        .TotalNetSqft = totalNetSqft,
        .TotalGrossSqft = totalGrossSqft,
        .ArchitectID = finalArchitectID,
        .EngineerID = finalEngineerID,
        .ProjectNotes = projectNotes
    }
            Dim projectParams = ModelParams.ForProject(projectModel)
            Dim projectID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertProject, HelperDataAccess.BuildParameters(projectParams), conn, transaction)

            ' Create and insert ProjectVersion
            Dim versionModel As New ProjectVersionModel With {
        .ProjectID = projectID,
        .VersionName = versionName,
        .VersionDate = Date.Now,
        .Description = description,
        .CustomerID = finalCustomerID,
        .SalesID = finalSalesID,
        .MondayID = MondayID
    }
            Dim versionParams = ModelParams.ForProjectVersion(versionModel)
            Dim versionID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertProjectVersion, HelperDataAccess.BuildParameters(versionParams), conn, transaction)

            ' Optionally insert default ProjectProductSettings
            If insertDefaultSettings Then
                Dim da As New ProjectDataAccess()
                Dim productTypes As List(Of ProductTypeModel) = da.GetProductTypes()
                For Each pt In productTypes
                    Dim settingParams As New Dictionary(Of String, Object) From {
                {"@VersionID", versionID},
                {"@ProductTypeID", pt.ProductTypeID},
                {"@MarginPercent", 0D},
                {"@LumberAdder", 0D}
            }
                    SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertProjectProductSetting, HelperDataAccess.BuildParameters(settingParams), conn, transaction)
                Next
            End If

            Return New Tuple(Of Integer, Integer)(projectID, versionID)
        End Function

        Private Shared Function InsertBuildingIfNew(
    buildingName As String,
    versionID As Integer,
    conn As SqlConnection,
    trans As SqlTransaction,
    ByRef buildingMap As Dictionary(Of String, Integer),
    Optional resUnits As Integer? = Nothing,
    Optional bldgQty As Integer = 1,
    Optional buildingType As Integer = 1,
    Optional mapKey As String = Nothing
) As Integer
            Dim key As String = If(Not String.IsNullOrEmpty(mapKey), mapKey.Trim().ToLower(), buildingName.Trim().ToLower()) ' Use mapKey if provided
            If buildingMap.ContainsKey(key) Then
                Return buildingMap(key)
            End If

            Dim buildingModel As New BuildingModel With {
        .BuildingName = buildingName,
        .BuildingType = buildingType,
        .ResUnits = resUnits,
        .BldgQty = bldgQty
    }
            Dim bldgParams = ModelParams.ForBuilding(buildingModel, versionID)
            Dim newBldgID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertBuilding, HelperDataAccess.BuildParameters(bldgParams), conn, trans)
            buildingMap.Add(key, newBldgID)
            Return newBldgID
        End Function

        Private Shared Function GetOrCreateLevel(
    buildingID As Integer,
    productTypeID As Integer,
    levelName As String,
    versionID As Integer,
    conn As SqlConnection,
    trans As SqlTransaction,
    ByRef levelMap As Dictionary(Of Tuple(Of Integer, Integer, String), Integer),
    Optional levelNumber As Integer? = Nothing
) As Integer
            Dim cleanLevelName As String = levelName.Trim().ToLower() ' Normalize for case-insensitive matching
            Dim key As Tuple(Of Integer, Integer, String) = Tuple.Create(buildingID, productTypeID, cleanLevelName)
            If levelMap.ContainsKey(key) Then
                Return levelMap(key)
            End If

            Dim levelModel As New LevelModel With {
        .ProductTypeID = productTypeID,
        .LevelNumber = If(levelNumber, 1),
        .LevelName = levelName
    }
            Dim levelParams = ModelParams.ForLevel(levelModel, versionID, buildingID)
            Dim newLevelID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertLevel, HelperDataAccess.BuildParameters(levelParams), conn, trans)
            levelMap.Add(key, newLevelID)
            Return newLevelID
        End Function

        Private Shared Function InsertRawUnitWithHistory(
    rawModel As RawUnitModel,
    versionID As Integer,
    conn As SqlConnection,
    trans As SqlTransaction,
    Optional costEffectiveID As Object = Nothing
) As Integer
            Dim rawParams = ModelParams.ForRawUnit(rawModel, versionID)
            Dim newRawID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertRawUnit, HelperDataAccess.BuildParameters(rawParams), conn, trans)

            ' Optionally insert lumber history if costEffectiveID provided or relevant fields present
            If costEffectiveID IsNot DBNull.Value OrElse
       rawModel.AvgSPFNo2.HasValue OrElse rawModel.Avg241800.HasValue OrElse
       rawModel.Avg242400.HasValue OrElse rawModel.Avg261800.HasValue OrElse
       rawModel.Avg262400.HasValue Then

                Dim historyParams As New Dictionary(Of String, Object) From {
            {"@RawUnitID", newRawID},
            {"@VersionID", versionID},
            {"@CostEffectiveDateID", costEffectiveID},
            {"@LumberCost", If(rawModel.LumberCost.HasValue, CType(rawModel.LumberCost.Value, Object), DBNull.Value)},
            {"@AvgSPFNo2", If(rawModel.AvgSPFNo2.HasValue, CType(rawModel.AvgSPFNo2.Value, Object), DBNull.Value)},
            {"@Avg241800", If(rawModel.Avg241800.HasValue, CType(rawModel.Avg241800.Value, Object), DBNull.Value)},
            {"@Avg242400", If(rawModel.Avg242400.HasValue, CType(rawModel.Avg242400.Value, Object), DBNull.Value)},
            {"@Avg261800", If(rawModel.Avg261800.HasValue, CType(rawModel.Avg261800.Value, Object), DBNull.Value)},
            {"@Avg262400", If(rawModel.Avg262400.HasValue, CType(rawModel.Avg262400.Value, Object), DBNull.Value)}
        }
                SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertRawUnitLumberHistory, HelperDataAccess.BuildParameters(historyParams), conn, trans)
            End If

            Return newRawID
        End Function

        ' 1. Insert only the ActualUnit (used by ImportSpreadsheetAsNewProject)
        Public Shared Function InsertActualUnitOnly(
    actualModel As ActualUnitModel,
    rawUnitID As Integer,
    versionID As Integer,
    conn As SqlConnection,
    trans As SqlTransaction,
    Optional colorCode As String = Nothing
) As Integer

            Dim actualParams = ModelParams.ForActualUnit(actualModel, versionID, rawUnitID, colorCode)
            Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
        Queries.InsertActualUnit,
        HelperDataAccess.BuildParameters(actualParams),
        conn, trans)
        End Function

        ' 2. Insert only the Mapping (called later, after Level exists)
        Public Shared Sub InsertActualToLevelMapping(
    actualUnitID As Integer,
    levelID As Integer,
    quantity As Integer,
    versionID As Integer,
    conn As SqlConnection,
    trans As SqlTransaction
)
            Dim mappingModel As New ActualToLevelMappingModel With {.Quantity = quantity}
            Dim mappingParams = ModelParams.ForMapping(mappingModel, versionID, actualUnitID, levelID)

            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
        Queries.InsertActualToLevelMapping,
        HelperDataAccess.BuildParameters(mappingParams),
        conn, trans)
        End Sub

        Private Shared Function InsertActualUnitAndMapping(
    actualModel As ActualUnitModel,
    rawUnitID As Integer,
    levelID As Integer,
    quantity As Integer,
    versionID As Integer,
    conn As SqlConnection,
    trans As SqlTransaction,
    Optional colorCode As String = Nothing
) As Integer

            Dim colorObj As Object = If(String.IsNullOrEmpty(colorCode), DBNull.Value, CType(colorCode, Object))

            Dim actualParams = ModelParams.ForActualUnit(actualModel, versionID, rawUnitID, If(colorObj Is DBNull.Value OrElse colorObj Is Nothing, Nothing, CStr(colorObj)))

            Dim newActualID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertActualUnit, HelperDataAccess.BuildParameters(actualParams), conn, trans)

            Dim mappingModel As New ActualToLevelMappingModel With {.Quantity = quantity}

            Dim mappingParams = ModelParams.ForMapping(mappingModel, versionID, newActualID, levelID)

            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.InsertActualToLevelMapping, HelperDataAccess.BuildParameters(mappingParams), conn, trans)

            Return newActualID
        End Function


        Private Shared Sub FinalizeImport(
    versionID As Integer,
    importLog As StringBuilder,
    trans As SqlTransaction,
    Optional successMessage As String = "Import completed successfully."
)
            trans.Commit()
            RollupDataAccess.RecalculateVersion(versionID)

            UIHelper.Add("All data imported and rollups recalculated.") ' Common append
            UIHelper.HideBusy(frmMain)
            MessageBox.Show(If(importLog.Length > 0, importLog.ToString(), successMessage), "Import Summary", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Sub

        ' Case-insensitive comparer for Tuple(Of Integer, Integer, String)
        Public Class TupleComparer
            Implements IEqualityComparer(Of Tuple(Of Integer, Integer, String))

            Private ReadOnly _stringComparer As StringComparer

            Public Sub New(stringComparer As StringComparer)
                _stringComparer = stringComparer
            End Sub

            Public Overloads Function Equals(x As Tuple(Of Integer, Integer, String), y As Tuple(Of Integer, Integer, String)) As Boolean Implements IEqualityComparer(Of Tuple(Of Integer, Integer, String)).Equals
                If x Is Nothing AndAlso y Is Nothing Then Return True
                If x Is Nothing OrElse y Is Nothing Then Return False
                Return x.Item1 = y.Item1 AndAlso
                       x.Item2 = y.Item2 AndAlso
                       _stringComparer.Equals(x.Item3, y.Item3)
            End Function

            Public Overloads Function GetHashCode(obj As Tuple(Of Integer, Integer, String)) As Integer Implements IEqualityComparer(Of Tuple(Of Integer, Integer, String)).GetHashCode
                If obj Is Nothing Then Return 0
                Dim hash As Integer = 17
                hash = hash * 31 + obj.Item1.GetHashCode()
                hash = hash * 31 + obj.Item2.GetHashCode()
                hash = hash * 31 + If(obj.Item3 Is Nothing, 0, _stringComparer.GetHashCode(obj.Item3))
                Return hash
            End Function
        End Class

    End Class
End Namespace