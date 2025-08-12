Option Strict On
' Add this import at the top of SpreadsheetImportData.vb (if not already present)
Imports System.Windows.Forms
Imports System.Collections.Generic
Imports System.Data
Imports System.Data.SqlClient
Imports System.Globalization
Imports BuildersPSE2.BuildersPSE.Models ' Adjust if your models are in a different namespace
Imports BuildersPSE2.BuildersPSE.Utilities
Imports System.Text


Namespace BuildersPSE.DataAccess
    Public Class SpreadsheetImportData
        ' Helper to build parameters (from existing DataAccess)
        Private Function BuildParameters(params As IDictionary(Of String, Object)) As SqlParameter()
            Dim sqlParams As New List(Of SqlParameter)
            For Each kvp As KeyValuePair(Of String, Object) In params
                sqlParams.Add(New SqlParameter(kvp.Key, If(kvp.Value, DBNull.Value)))
            Next
            Return sqlParams.ToArray()
        End Function

        ' Helper to get or insert CustomerID
        Private Function GetOrInsertCustomer(customerName As String, customerType As Integer, conn As SqlConnection, transaction As SqlTransaction) As Integer?
            If String.IsNullOrEmpty(customerName) Then Return Nothing
            Dim params As New Dictionary(Of String, Object) From {
                {"@CustomerName", customerName},
                {"@CustomerType", customerType}
            }
            Dim customerIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)("SELECT CustomerID FROM Customer WHERE CustomerName = @CustomerName AND CustomerType = @CustomerType", BuildParameters(params), conn, transaction)
            If customerIDObj Is DBNull.Value OrElse customerIDObj Is Nothing Then
                params = New Dictionary(Of String, Object) From {
                    {"@CustomerName", customerName},
                    {"@CustomerType", customerType}
                }
                Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertCustomer, BuildParameters(params), conn, transaction)
            End If
            Return CInt(customerIDObj)
        End Function

        ' Helper to get or insert EstimatorID
        Private Function GetOrInsertEstimator(estimatorName As String, conn As SqlConnection, transaction As SqlTransaction) As Integer?
            If String.IsNullOrEmpty(estimatorName) Then Return Nothing
            Dim params As New Dictionary(Of String, Object) From {{"@EstimatorName", estimatorName}}
            Dim estimatorIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)("SELECT EstimatorID FROM Estimator WHERE EstimatorName = @EstimatorName", BuildParameters(params), conn, transaction)
            If estimatorIDObj Is DBNull.Value OrElse estimatorIDObj Is Nothing Then
                params = New Dictionary(Of String, Object) From {{"@EstimatorName", estimatorName}}
                Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertEstimator, BuildParameters(params), conn, transaction)
            End If
            Return CInt(estimatorIDObj)
        End Function

        ' Helper to get or insert SalesID
        Private Function GetOrInsertSales(salesName As String, conn As SqlConnection, transaction As SqlTransaction) As Integer?
            If String.IsNullOrEmpty(salesName) Then Return Nothing
            Dim params As New Dictionary(Of String, Object) From {{"@SalesName", salesName}}
            Dim salesIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)("SELECT SalesID FROM Sales WHERE SalesName = @SalesName", BuildParameters(params), conn, transaction)
            If salesIDObj Is DBNull.Value OrElse salesIDObj Is Nothing Then
                params = New Dictionary(Of String, Object) From {{"@SalesName", salesName}}
                Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertSales, BuildParameters(params), conn, transaction)
            End If
            Return CInt(salesIDObj)
        End Function
        Private Function ParseDate(value As String) As Date?
            If String.IsNullOrEmpty(value) Then Return Nothing
            value = value.Trim()
            Dim result As Date
            If Date.TryParseExact(value, New String() {"M/d/yy", "M.d.yy", "M-d-yy", "M/d/yyyy", "M/d/yyyy h:mm:ss tt"}, CultureInfo.InvariantCulture, DateTimeStyles.None, result) Then
                Return result
            End If
            Return Nothing
        End Function

        ' Inside SpreadsheetImportData class
        Public Sub ImportSpreadsheetAsNewProject(filePath As String)
            Dim spreadsheetData As Dictionary(Of String, DataTable) = SpreadsheetParser.ParseSpreadsheet(filePath)
            Dim importLog As New StringBuilder()
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using transaction As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   ' Step 1: Extract project details from Summary sheet
                                                                                   Dim summaryDt As DataTable = spreadsheetData("Summary")
                                                                                   Dim jbid As String = If(summaryDt.Rows(0)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(0)(1))) ' Row 2, col 2: P209824
                                                                                   Dim projectName As String = If(summaryDt.Rows(1)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(1)(1))) ' Row 3, col 2: Ponderosa Pines
                                                                                   Dim address As String = If(summaryDt.Rows(2)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(2)(1))) ' Row 4, col 2
                                                                                   Dim city As String = If(summaryDt.Rows(3)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(3)(1))) ' Row 5, col 2: Parker
                                                                                   Dim state As String = If(summaryDt.Rows(3)(3) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(3)(3))) ' Row 5, col 3: CO
                                                                                   Dim zip As String = If(summaryDt.Rows(3)(4) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(3)(4))) ' Row 5, col 4
                                                                                   Dim bidDate As Date? = If(summaryDt.Rows(2)(6) Is DBNull.Value, Nothing, ParseDate(CStr(summaryDt.Rows(2)(6))))
                                                                                   Dim archPlansDated As Date? = If(summaryDt.Rows(3)(6) Is DBNull.Value, Nothing, ParseDate(CStr(summaryDt.Rows(3)(6))))
                                                                                   Dim engPlansDated As Date? = If(summaryDt.Rows(4)(6) Is DBNull.Value, Nothing, ParseDate(CStr(summaryDt.Rows(4)(6))))
                                                                                   Dim milesToJobSite As Integer? = If(summaryDt.Rows(5)(6) Is DBNull.Value, Nothing, CInt(summaryDt.Rows(5)(6))) ' Row 7, col 6: 28
                                                                                   Dim totalNetSqft As Integer? = If(summaryDt.Rows(6)(1) Is DBNull.Value, Nothing, CInt(summaryDt.Rows(6)(1))) ' Row 8, col 2: 219802
                                                                                   Dim totalGrossSqft As Integer? = If(summaryDt.Rows(7)(1) Is DBNull.Value, Nothing, CInt(summaryDt.Rows(7)(1))) ' Row 9, col 2: 238614
                                                                                   Dim architectName As String = If(summaryDt.Rows(9)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(9)(1))) ' Row 11, col 2: KTGY
                                                                                   Dim engineerName As String = If(summaryDt.Rows(8)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(8)(1))) ' Row 10, col 2: Integrity
                                                                                   Dim customerName As String = If(summaryDt.Rows(5)(1) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(5)(1))) ' Row 7, col 2: Shaw
                                                                                   Dim estimatorName As String = If(summaryDt.Rows(1)(6) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(1)(6))) ' Row 3, col 6: Tim Teruel
                                                                                   Dim salesName As String = If(summaryDt.Rows(0)(6) Is DBNull.Value, String.Empty, CStr(summaryDt.Rows(0)(6))) ' Row 2, col 6: Doug Sehr

                                                                                   ' Step 2: Insert new project
                                                                                   Dim architectID As Integer? = GetOrInsertCustomer(architectName, 2, conn, transaction) ' CustomerType=2 (Architect)
                                                                                   Dim engineerID As Integer? = GetOrInsertCustomer(engineerName, 3, conn, transaction) ' CustomerType=3 (Engineer)
                                                                                   Dim customerID As Integer? = GetOrInsertCustomer(customerName, 1, conn, transaction) ' CustomerType=1 (Customer)
                                                                                   Dim estimatorID As Integer? = GetOrInsertEstimator(estimatorName, conn, transaction)
                                                                                   Dim salesID As Integer? = GetOrInsertSales(salesName, conn, transaction)

                                                                                   Dim projectParams As New Dictionary(Of String, Object) From {
                        {"@JBID", If(String.IsNullOrEmpty(jbid), DBNull.Value, CType(jbid, Object))},
                        {"@ProjectTypeID", 1}, ' Hardcode to 1 for MultiFamily For Rent
                        {"@ProjectName", If(String.IsNullOrEmpty(projectName), DBNull.Value, CType(projectName, Object))},
                        {"@EstimatorID", If(estimatorID.HasValue, CType(estimatorID.Value, Object), DBNull.Value)},
                        {"@Address", If(String.IsNullOrEmpty(address), DBNull.Value, CType(address, Object))},
                        {"@City", If(String.IsNullOrEmpty(city), DBNull.Value, CType(city, Object))},
                        {"@State", If(String.IsNullOrEmpty(state), DBNull.Value, CType(state, Object))},
                        {"@Zip", If(String.IsNullOrEmpty(zip), DBNull.Value, CType(zip, Object))},
                        {"@BidDate", If(bidDate.HasValue, CType(bidDate.Value, Object), DBNull.Value)},
                        {"@ArchPlansDated", If(archPlansDated.HasValue, CType(archPlansDated.Value, Object), DBNull.Value)},
                        {"@EngPlansDated", If(engPlansDated.HasValue, CType(engPlansDated.Value, Object), DBNull.Value)},
                        {"@MilesToJobSite", If(milesToJobSite.HasValue, CType(milesToJobSite.Value, Object), DBNull.Value)},
                        {"@TotalNetSqft", If(totalNetSqft.HasValue, CType(totalNetSqft.Value, Object), DBNull.Value)},
                        {"@TotalGrossSqft", If(totalGrossSqft.HasValue, CType(totalGrossSqft.Value, Object), DBNull.Value)},
                        {"@ArchitectID", If(architectID.HasValue, CType(architectID.Value, Object), DBNull.Value)},
                        {"@EngineerID", If(engineerID.HasValue, CType(engineerID.Value, Object), DBNull.Value)},
                        {"@ProjectNotes", DBNull.Value}
                    }
                                                                                   Dim projectID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertProject, BuildParameters(projectParams), conn, transaction)
                                                                                   importLog.AppendLine($"Project '{projectName}' created successfully with ID {projectID}.")

                                                                                   ' Step 3: Call existing ImportFromSpreadsheet
                                                                                   Dim rawUnitCount As Integer = 0
                                                                                   Dim actualUnitCount As Integer = 0
                                                                                   Dim buildingCount As Integer = 0
                                                                                   Dim levelCount As Integer = 0
                                                                                   Dim mappingCount As Integer = 0

                                                                                   ' Step 3.1: Create new ProjectVersion
                                                                                   Dim versionParams As New Dictionary(Of String, Object) From {
                                                                                                {"@ProjectID", projectID},
                                                                                                {"@VersionName", "Imported_" & Date.Now.ToString("yyyyMMdd")},
                                                                                                {"@VersionDate", Date.Now},
                                                                                                {"@Description", "Imported from spreadsheet"},
                                                                                                {"@CustomerID", If(customerID.HasValue, CType(customerID.Value, Object), DBNull.Value)},
                                                                                                {"@SalesID", If(salesID.HasValue, CType(salesID.Value, Object), DBNull.Value)}
                                                                                            }
                                                                                   Dim newVersionID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertProjectVersion, BuildParameters(versionParams), conn, transaction)
                                                                                   importLog.AppendLine($"Project version created with ID {newVersionID}.")

                                                                                   ' Insert default ProjectProductSettings for the new version
                                                                                   Dim da As New DataAccess()
                                                                                   Dim productTypes As List(Of ProductTypeModel) = da.GetProductTypes()
                                                                                   For Each pt In productTypes
                                                                                       Dim settingParams As New Dictionary(Of String, Object) From {
                                                                                            {"@VersionID", newVersionID},
                                                                                            {"@ProductTypeID", pt.ProductTypeID},
                                                                                            {"@MarginPercent", 0D},
                                                                                            {"@LumberAdder", 0D}
                                                                                        }
                                                                                       SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertProjectProductSetting, BuildParameters(settingParams), conn, transaction)
                                                                                   Next
                                                                                   importLog.AppendLine($"Inserted default project product settings for {productTypes.Count} product types.")

                                                                                   'Step 3.2: Import RawUnits from FloorImport and RoofImport sheets
                                                                                   Dim rawUnitMap As New Dictionary(Of String, Integer) ' Key: Plan_JobNumber, Value: New RawUnitID
                                                                                   For Each sheet In {"FloorImport", "RoofImport"}
                                                                                       If spreadsheetData.ContainsKey(sheet) Then
                                                                                           Dim dt As DataTable = spreadsheetData(sheet)
                                                                                           For Each row As DataRow In dt.Rows
                                                                                               If row("JobNumber") Is DBNull.Value Then Continue For ' Skip empty
                                                                                               Dim productTypeID As Integer = If(sheet = "FloorImport", 1, 2) ' Floor=1, Roof=2
                                                                                               Dim planName As String = If(row("Elevation") Is DBNull.Value, String.Empty, CStr(row("Elevation")))
                                                                                               Dim jobNumber As String = CStr(row("JobNumber"))
                                                                                               Dim uniqueKey As String = If(String.IsNullOrEmpty(planName), "Unknown_" & jobNumber, planName & "_" & jobNumber)
                                                                                               Dim rawParams As New Dictionary(Of String, Object) From {
                                                                                                    {"@RawUnitName", planName},
                                                                                                    {"@VersionID", newVersionID},
                                                                                                    {"@ProductTypeID", productTypeID},
                                                                                                    {"@BF", If(row("BF") Is DBNull.Value, DBNull.Value, CType(row("BF"), Object))},
                                                                                                    {"@LF", If(row("LF") Is DBNull.Value, DBNull.Value, CType(row("LF"), Object))},
                                                                                                    {"@EWPLF", If(row("EWPLF") Is DBNull.Value, DBNull.Value, CType(row("EWPLF"), Object))},
                                                                                                    {"@SqFt", If(row("SqFt") Is DBNull.Value, DBNull.Value, CType(row("SqFt"), Object))},
                                                                                                    {"@FCArea", If(row("FCArea") Is DBNull.Value, DBNull.Value, CType(row("FCArea"), Object))},
                                                                                                    {"@LumberCost", If(row("LumberCost") Is DBNull.Value, DBNull.Value, CType(row("LumberCost"), Object))},
                                                                                                    {"@PlateCost", If(row("PlateCost") Is DBNull.Value, DBNull.Value, CType(row("PlateCost"), Object))},
                                                                                                    {"@ManufLaborCost", If(row("ManufLaborCost") Is DBNull.Value, DBNull.Value, CType(row("ManufLaborCost"), Object))},
                                                                                                    {"@DesignLabor", If(row("DesignLabor") Is DBNull.Value, DBNull.Value, CType(row("DesignLabor"), Object))},
                                                                                                    {"@MGMTLabor", If(row("MGMTLabor") Is DBNull.Value, DBNull.Value, CType(row("MGMTLabor"), Object))},
                                                                                                    {"@JobSuppliesCost", If(row("JobSuppliesCost") Is DBNull.Value, DBNull.Value, CType(row("JobSuppliesCost"), Object))},
                                                                                                    {"@ManHours", If(row("ManHours") Is DBNull.Value, DBNull.Value, CType(row("ManHours"), Object))},
                                                                                                    {"@ItemCost", If(row("ItemCost") Is DBNull.Value, DBNull.Value, CType(row("ItemCost"), Object))},
                                                                                                    {"@OverallCost", If(row("OverallCost") Is DBNull.Value, DBNull.Value, CType(row("OverallCost"), Object))},
                                                                                                    {"@DeliveryCost", If(row("DeliveryCost") Is DBNull.Value, DBNull.Value, CType(row("DeliveryCost"), Object))},
                                                                                                    {"@TotalSellPrice", If(row("TotalSellPrice") Is DBNull.Value, DBNull.Value, CType(row("TotalSellPrice"), Object))},
                                                                                                    {"@AvgSPFNo2", If(row("AvgSPFNo2") Is DBNull.Value, DBNull.Value, CType(row("AvgSPFNo2"), Object))}
                                                                                                }
                                                                                               Dim newRawID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertRawUnit, BuildParameters(rawParams), conn, transaction)
                                                                                               rawUnitMap.Add(uniqueKey, newRawID)
                                                                                               rawUnitCount += 1
                                                                                           Next
                                                                                       End If
                                                                                   Next
                                                                                   importLog.AppendLine($"Imported {rawUnitCount} raw units from FloorImport and RoofImport.")
                                                                                   ' Step 3.3: Import ActualUnits
                                                                                   Dim actualUnitMap As New Dictionary(Of String, Integer) ' Key: uniqueKey (unitName_planSqft_col), Value: New ActualUnitID
                                                                                   Dim actualUnitSet As New HashSet(Of String) ' Track unique unitName_planSqft_rawID
                                                                                   For Each sheet In {"Floor Unit Data", "Roof Unit Data"}
                                                                                       If spreadsheetData.ContainsKey(sheet) Then
                                                                                           Dim dt As DataTable = spreadsheetData(sheet)
                                                                                           For col As Integer = 46 To dt.Columns.Count - 1
                                                                                               Dim unitName As String = If(dt.Rows(0)(col) Is DBNull.Value, String.Empty, CStr(dt.Rows(0)(col)))
                                                                                               If String.IsNullOrEmpty(unitName) OrElse unitName = "<select>" Then Continue For
                                                                                               Dim productTypeID As Integer = If(sheet.Contains("Floor"), 1, 2)
                                                                                               Dim rawName As String = dt.Columns(col).ColumnName.Replace("Models-", "").Split("_"c)(0) ' Strip Models- prefix and _x suffix
                                                                                               Dim rawID As Integer = 0
                                                                                               For Each key As String In rawUnitMap.Keys
                                                                                                   If key.StartsWith(rawName & "_") Then
                                                                                                       rawID = rawUnitMap(key)
                                                                                                       Exit For
                                                                                                   End If
                                                                                               Next
                                                                                               If rawID = 0 Then Continue For
                                                                                               Dim planSqftObj As Object = dt.Rows(1)(col)
                                                                                               Dim planSqft As Decimal? = If(planSqftObj Is DBNull.Value, Nothing, CDec(planSqftObj))
                                                                                               Dim planSqftStr As String = If(planSqft.HasValue, planSqft.Value.ToString(), "0")
                                                                                               Dim uniqueCombo As String = unitName & "_" & planSqftStr & "_" & rawID
                                                                                               If actualUnitSet.Contains(uniqueCombo) Then
                                                                                                   Debug.WriteLine($"Skipped duplicate actual unit: {unitName}, PlanSQFT={planSqftStr}, RawID={rawID}")
                                                                                                   Continue For ' Skip as duplicate
                                                                                               End If
                                                                                               Dim uniqueKey As String = If(String.IsNullOrEmpty(unitName), "Unknown_" & planSqftStr & "_" & col, unitName & "_" & planSqftStr & "_" & col)
                                                                                               Dim actualParams As New Dictionary(Of String, Object) From {
                {"@VersionID", newVersionID},
                {"@RawUnitID", rawID},
                {"@ProductTypeID", productTypeID},
                {"@UnitName", unitName},
                {"@PlanSQFT", If(planSqft.HasValue, CType(planSqft.Value, Object), DBNull.Value)},
                {"@UnitType", "Standard"},
                {"@OptionalAdder", 1D}
            }
                                                                                               Dim newActualID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertActualUnit, BuildParameters(actualParams), conn, transaction)
                                                                                               actualUnitMap.Add(uniqueKey, newActualID)
                                                                                               actualUnitSet.Add(uniqueCombo) ' Track unique combo
                                                                                               actualUnitCount += 1
                                                                                           Next
                                                                                       End If
                                                                                   Next
                                                                                   importLog.AppendLine($"Imported {actualUnitCount} actual units from Floor and Roof Unit Data.")


                                                                                   ' Step 3.4: Import CalculatedComponents
                                                                                   Dim componentCount As Integer = 0
                                                                                   Dim dataAccess As New DataAccess()
                                                                                   ' Collect RawUnit data first to avoid open DataReader during inserts
                                                                                   Dim rawUnitData As New Dictionary(Of Integer, Dictionary(Of String, Decimal))
                                                                                   For Each actualKey As String In actualUnitMap.Keys
                                                                                       Dim actualUnitID As Integer = actualUnitMap(actualKey)
                                                                                       ' Extract unitName, planSqft, and col from actualKey (unitName_planSqft_col)
                                                                                       Dim keyParts As String() = actualKey.Split("_"c)
                                                                                       Dim unitName As String = keyParts(0)
                                                                                       Dim planSqftStr As String = If(keyParts.Length > 1, keyParts(1), "0")
                                                                                       Dim planSqft As Decimal? = If(planSqftStr = "0", Nothing, CDec(planSqftStr))
                                                                                       ' Get RawUnitID from ActualUnits import
                                                                                       Dim actualParams As New Dictionary(Of String, Object) From {
        {"@ActualUnitID", actualUnitID},
        {"@VersionID", newVersionID}
    }
                                                                                       Dim rawUnitIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)("SELECT RawUnitID FROM ActualUnits WHERE ActualUnitID = @ActualUnitID AND VersionID = @VersionID", BuildParameters(actualParams), conn, transaction)
                                                                                       If rawUnitIDObj Is DBNull.Value Then Continue For
                                                                                       Dim rawUnitID As Integer = CInt(rawUnitIDObj)
                                                                                       ' Skip if RawUnitID already processed
                                                                                       If rawUnitData.ContainsKey(rawUnitID) Then Continue For
                                                                                       ' Get RawUnit data
                                                                                       Dim rawParams As New Dictionary(Of String, Object) From {
        {"@RawUnitID", rawUnitID},
        {"@VersionID", newVersionID}
    }
                                                                                       Dim rawReader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReaderTransactional("SELECT SqFt, LF, BF, LumberCost, PlateCost, ManufLaborCost, DesignLabor, MGMTLabor, JobSuppliesCost, ManHours, ItemCost, OverallCost, TotalSellPrice FROM RawUnits WHERE RawUnitID = @RawUnitID AND VersionID = @VersionID", BuildParameters(rawParams), conn, transaction)
                                                                                       If Not rawReader.Read() Then
                                                                                           rawReader.Close()
                                                                                           Debug.WriteLine($"Skipped CalculatedComponents for ActualUnitID {actualUnitID}: RawUnitID {rawUnitID} not found")
                                                                                           Continue For
                                                                                       End If
                                                                                       Dim sqFt As Decimal = If(rawReader("SqFt") Is DBNull.Value, 0D, CDec(rawReader("SqFt")))
                                                                                       Dim lf As Decimal = If(rawReader("LF") Is DBNull.Value, 0D, CDec(rawReader("LF")))
                                                                                       Dim bf As Decimal = If(rawReader("BF") Is DBNull.Value, 0D, CDec(rawReader("BF")))
                                                                                       Dim lumberCost As Decimal = If(rawReader("LumberCost") Is DBNull.Value, 0D, CDec(rawReader("LumberCost")))
                                                                                       Dim plateCost As Decimal = If(rawReader("PlateCost") Is DBNull.Value, 0D, CDec(rawReader("PlateCost")))
                                                                                       Dim manufLaborCost As Decimal = If(rawReader("ManufLaborCost") Is DBNull.Value, 0D, CDec(rawReader("ManufLaborCost")))
                                                                                       Dim designLabor As Decimal = If(rawReader("DesignLabor") Is DBNull.Value, 0D, CDec(rawReader("DesignLabor")))
                                                                                       Dim mgmtLabor As Decimal = If(rawReader("MGMTLabor") Is DBNull.Value, 0D, CDec(rawReader("MGMTLabor")))
                                                                                       Dim jobSuppliesCost As Decimal = If(rawReader("JobSuppliesCost") Is DBNull.Value, 0D, CDec(rawReader("JobSuppliesCost")))
                                                                                       Dim manHours As Decimal = If(rawReader("ManHours") Is DBNull.Value, 0D, CDec(rawReader("ManHours")))
                                                                                       Dim itemCost As Decimal = If(rawReader("ItemCost") Is DBNull.Value, 0D, CDec(rawReader("ItemCost")))
                                                                                       Dim OverallCost As Decimal = If(rawReader("OverallCost") Is DBNull.Value, 0D, CDec(rawReader("OverallCost")))
                                                                                       Dim TotalSellPrice As Decimal = If(rawReader("TotalSellPrice") Is DBNull.Value, 0D, CDec(rawReader("TotalSellPrice")))
                                                                                       rawReader.Close()
                                                                                       If sqFt <= 0 Then
                                                                                           Debug.WriteLine($"Skipped CalculatedComponents for ActualUnitID {actualUnitID}: SqFt is {sqFt}")
                                                                                           Continue For
                                                                                       End If
                                                                                       ' Calculate components
                                                                                       Dim productTypeID As Integer = If(actualKey.Contains("Floor"), 1, 2)
                                                                                       Dim lfPerSqft As Decimal = lf / sqFt
                                                                                       Dim bdftPerSqft As Decimal = bf / sqFt
                                                                                       Dim lumberPerSqft As Decimal = lumberCost / sqFt
                                                                                       Dim platePerSqft As Decimal = plateCost / sqFt
                                                                                       Dim manufLaborPerSqft As Decimal = manufLaborCost / sqFt
                                                                                       Dim designLaborPerSqft As Decimal = designLabor / sqFt
                                                                                       Dim mgmtLaborPerSqft As Decimal = mgmtLabor / sqFt
                                                                                       Dim jobSuppliesPerSqft As Decimal = jobSuppliesCost / sqFt
                                                                                       Dim manHoursPerSqft As Decimal = manHours / sqFt
                                                                                       Dim itemCostPerSqft As Decimal = itemCost / sqFt
                                                                                       Dim overallCostPerSqft As Decimal = OverallCost / sqFt
                                                                                       Dim sellPerSqft As Decimal = TotalSellPrice / sqFt
                                                                                       Dim marginPerSqft As Decimal = sellPerSqft - overallCostPerSqft
                                                                                       ' Insert components
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
                                                                                           Dim componentParams As New Dictionary(Of String, Object) From {
            {"@VersionID", newVersionID},
            {"@ActualUnitID", actualUnitID},
            {"@ComponentType", comp.Item1},
            {"@Value", comp.Item2}
        }
                                                                                           SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertCalculatedComponent, BuildParameters(componentParams), conn, transaction)
                                                                                           componentCount += 1
                                                                                       Next
                                                                                   Next
                                                                                   importLog.AppendLine($"Imported {componentCount} calculated components.")
                                                                                   ' Step 3.4: Import Buildings
                                                                                   Dim buildingMap As New Dictionary(Of String, Integer)
                                                                                   If spreadsheetData.ContainsKey("Buildings") Then
                                                                                       Dim dt As DataTable = spreadsheetData("Buildings")
                                                                                       For i As Integer = 3 To dt.Rows.Count - 1
                                                                                           Dim buildingNameObj As Object = dt.Rows(i)(1)
                                                                                           Dim buildingName As String = If(buildingNameObj Is DBNull.Value, String.Empty, CStr(buildingNameObj)).Trim()
                                                                                           If String.IsNullOrEmpty(buildingName) Then Continue For
                                                                                           Dim resUnitsObj As Object = dt.Rows(i)(2)
                                                                                           Dim resUnits As Integer? = If(resUnitsObj Is DBNull.Value, Nothing, CInt(resUnitsObj))
                                                                                           Dim bldgQty As Integer = If(dt.Rows(i)(3) Is DBNull.Value, 0, CInt(dt.Rows(i)(3)))
                                                                                           Dim bldgParams As New Dictionary(Of String, Object) From {
                                {"@BuildingName", buildingName},
                                {"@BuildingType", 1},
                                {"@ResUnits", If(resUnits.HasValue, CType(resUnits.Value, Object), DBNull.Value)},
                                {"@BldgQty", bldgQty},
                                {"@VersionID", newVersionID}
                            }
                                                                                           Dim newBldgID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertBuilding, BuildParameters(bldgParams), conn, transaction)
                                                                                           buildingMap.Add(buildingName, newBldgID)
                                                                                           buildingCount += 1
                                                                                       Next
                                                                                   End If
                                                                                   importLog.AppendLine($"Imported {buildingCount} buildings from Buildings sheet.")

                                                                                   ' Step 3.5: Import Levels
                                                                                   Dim levelMap As New Dictionary(Of Tuple(Of Integer, Integer, String), Integer) ' Key: (BuildingID, ProductTypeID, LevelName), Value: LevelID
                                                                                   For Each sheet In {"Floor Unit Data", "Roof Unit Data"}
                                                                                       If spreadsheetData.ContainsKey(sheet) Then
                                                                                           Dim dt As DataTable = spreadsheetData(sheet)
                                                                                           Dim productTypeID As Integer = If(sheet.Contains("Floor"), 1, 2)
                                                                                           For rowIdx As Integer = 33 To Math.Min(dt.Rows.Count - 1, 68) ' Rows 39-69 (0-based 38-68)
                                                                                               Dim buildingName As String = If(dt.Rows(rowIdx)(0) Is DBNull.Value, String.Empty, CStr(dt.Rows(rowIdx)(0)).Trim())
                                                                                               If String.IsNullOrEmpty(buildingName) OrElse Not buildingMap.ContainsKey(buildingName) Then
                                                                                                   Debug.WriteLine($"Skipped row {rowIdx + 1} in {sheet}: buildingName '{buildingName}' not in buildingMap")
                                                                                                   Continue For
                                                                                               End If
                                                                                               Dim levelName As String = If(dt.Rows(rowIdx)(1) Is DBNull.Value OrElse String.IsNullOrEmpty(CStr(dt.Rows(rowIdx)(1)).Trim()), If(productTypeID = 2, "roof", String.Empty), CStr(dt.Rows(rowIdx)(1)).Trim())
                                                                                               If String.IsNullOrEmpty(levelName) Then
                                                                                                   Debug.WriteLine($"Skipped row {rowIdx + 1} in {sheet}: levelName is empty")
                                                                                                   Continue For
                                                                                               End If
                                                                                               Dim buildingID As Integer = buildingMap(buildingName)
                                                                                               Dim uniqueKey As Tuple(Of Integer, Integer, String) = Tuple.Create(buildingID, productTypeID, levelName)
                                                                                               If levelMap.ContainsKey(uniqueKey) Then Continue For ' Skip duplicates
                                                                                               Dim levelParams As New Dictionary(Of String, Object) From {
                {"@VersionID", newVersionID},
                {"@BuildingID", buildingID},
                {"@ProductTypeID", productTypeID},
                {"@LevelNumber", rowIdx - 33}, ' Incremental number starting from 1 (row 39 = 1)
                {"@LevelName", levelName}
            }
                                                                                               Dim newLevelID As Integer = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(Queries.InsertLevel, BuildParameters(levelParams), conn, transaction)
                                                                                               levelMap.Add(uniqueKey, newLevelID)
                                                                                               levelCount += 1
                                                                                           Next
                                                                                       End If
                                                                                   Next
                                                                                   importLog.AppendLine($"Imported {levelCount} levels for buildings.")

                                                                                   ' Step 3.6: Import ActualToLevelMappings

                                                                                   For Each sheet In {"Floor Unit Data", "Roof Unit Data"}
                                                                                       If spreadsheetData.ContainsKey(sheet) Then
                                                                                           Dim dt As DataTable = spreadsheetData(sheet)
                                                                                           Dim productTypeID As Integer = If(sheet.Contains("Floor"), 1, 2)

                                                                                           For rowIdx As Integer = 33 To dt.Rows.Count - 1 ' Rows 39-69 (0-based 38-68)
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
                                                                                                   If qtyObj Is DBNull.Value OrElse CInt(qtyObj) <= 0 Then Continue For
                                                                                                   Dim unitName As String = If(dt.Rows(0)(colIdx) Is DBNull.Value, String.Empty, CStr(dt.Rows(0)(colIdx)))
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
                                                                                                   If actualUnitID = 0 Then
                                                                                                       Debug.WriteLine($"Skipped col {colIdx + 1} in {sheet}: unitName '{unitName}' with planSqft '{planSqft}' not in actualUnitMap")
                                                                                                       Continue For
                                                                                                   End If
                                                                                                   Dim mappingParams As New Dictionary(Of String, Object) From {
                                                                                                        {"@VersionID", newVersionID},
                                                                                                        {"@ActualUnitID", actualUnitID},
                                                                                                        {"@LevelID", levelID},
                                                                                                        {"@Quantity", CInt(qtyObj)}
                                                                                                    }
                                                                                                   SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.InsertActualToLevelMapping, BuildParameters(mappingParams), conn, transaction)
                                                                                                   mappingCount += 1
                                                                                               Next
                                                                                           Next
                                                                                       End If
                                                                                   Next
                                                                                   importLog.AppendLine($"Imported {mappingCount} actual-to-level mappings.")

                                                                                   ' Step 3.7: Commit and recalculate
                                                                                   transaction.Commit()

                                                                                   da.RecalculateVersion(newVersionID)
                                                                                   importLog.AppendLine("All data imported and rollups recalculated.")
                                                                                   MessageBox.Show(importLog.ToString(), "Import Summary", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                                               Catch ex As Exception
                                                                                   transaction.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error importing spreadsheet as new project")
        End Sub
    End Class
End Namespace