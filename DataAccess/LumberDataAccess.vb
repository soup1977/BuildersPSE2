Option Strict On

Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.Services
Imports BuildersPSE2.Utilities
Imports OpenQA.Selenium
Imports OpenQA.Selenium.Edge
Imports OpenQA.Selenium.Support.UI

Namespace DataAccess
    ''' <summary>
    ''' Data access layer for lumber pricing, cost history, and futures operations.
    ''' Manages LumberType, LumberCostEffective, LumberCost, RawUnitLumberHistory, and LumberFutures tables.
    ''' Uses TableOperations.ToDb() for consistent null handling.
    ''' </summary>
    Public Class LumberDataAccess

#Region "Constants"

        ''' <summary>Lumber type mappings: Key -> (DatabaseDesc, AvgField, BDFTField)</summary>
        Private Shared ReadOnly LumberTypeMappings As New Dictionary(Of String, Tuple(Of String, String, String)) From {
            {"SPFNo2", Tuple.Create("2x4 SPF #2", "AvgSPFNo2", "SPFNo2BDFT")},
            {"MSR241800", Tuple.Create("2x4 DF-N 1800F 1.6E", "Avg241800", "MSR241800BDFT")},
            {"MSR242400", Tuple.Create("2x4 DF-N 2400F 2.0E", "Avg242400", "MSR242400BDFT")},
            {"MSR261800", Tuple.Create("2x6 DF-N 1800F 1.6E", "Avg261800", "MSR261800BDFT")},
            {"MSR262400", Tuple.Create("2x6 DF-N 2400F 2.0E", "Avg262400", "MSR262400BDFT")}
        }

#End Region

#Region "LumberType Operations"

        ''' <summary>
        ''' Gets all lumber types for dropdowns and lookups.
        ''' </summary>
        ''' <returns>DataTable with LumberTypeID and LumberTypeDesc columns.</returns>
        Public Function GetAllLumberTypes() As DataTable
            Dim dt As New DataTable()
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using da As New SqlDataAdapter(Queries.SelectLumberTypes, conn)
                                                                               da.Fill(dt)
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error fetching lumber types")
            Return dt
        End Function

        ''' <summary>
        ''' Creates a new lumber type.
        ''' </summary>
        ''' <param name="lumberTypeDesc">Description (e.g., "2x4 SPF #2").</param>
        ''' <returns>The new LumberTypeID.</returns>
        ''' <exception cref="ArgumentException">Thrown if description is null or empty.</exception>
        Public Function CreateLumberType(lumberTypeDesc As String) As Integer
            If String.IsNullOrWhiteSpace(lumberTypeDesc) Then
                Throw New ArgumentException("LumberTypeDesc cannot be null or empty.", NameOf(lumberTypeDesc))
            End If

            Dim newID As Integer = 0
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim params As New Dictionary(Of String, Object) From {
                                                                           {"@LumberTypeDesc", lumberTypeDesc}
                                                                       }
                                                                       newID = CInt(SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                                                                           Queries.InsertLumberType, HelperDataAccess.BuildParameters(params)))
                                                                   End Sub, "Error creating lumber type: " & lumberTypeDesc)
            Return newID
        End Function

#End Region

#Region "LumberCostEffective Operations"

        ''' <summary>
        ''' Gets all cost effective dates, ordered by date descending.
        ''' </summary>
        ''' <returns>DataTable with CostEffectiveID and CosteffectiveDate columns.</returns>
        Public Shared Function GetAllLumberCostEffective() As DataTable
            Dim dt As New DataTable()
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using da As New SqlDataAdapter(Queries.SelectLumberCostEffective, conn)
                                                                               da.Fill(dt)
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error fetching lumber cost effective dates")
            Return dt
        End Function

        ''' <summary>
        ''' Creates a new cost effective date and initializes all lumber types with $0.00 cost.
        ''' </summary>
        ''' <param name="effectiveDate">The effective date for the new pricing period.</param>
        ''' <returns>The new CostEffectiveID.</returns>
        Public Function CreateLumberCostEffective(effectiveDate As Date) As Integer
            Dim newID As Integer = 0

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   ' Insert cost effective date
                                                                                   Dim dateParams As New Dictionary(Of String, Object) From {
                                                                                       {"@CosteffectiveDate", effectiveDate}
                                                                                   }
                                                                                   newID = CInt(SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(
                                                                                       Queries.InsertLumberCostEffective, HelperDataAccess.BuildParameters(dateParams), conn, tran))

                                                                                   ' Initialize all lumber types with $0.00
                                                                                   Dim lumberTypes As DataTable = GetAllLumberTypes()
                                                                                   For Each row As DataRow In lumberTypes.Rows
                                                                                       Dim costParams As New Dictionary(Of String, Object) From {
                                                                                           {"@LumberTypeID", CInt(row("LumberTypeID"))},
                                                                                           {"@LumberCost", 0.00D},
                                                                                           {"@CostEffectiveDateID", newID}
                                                                                       }
                                                                                       SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                                                                           Queries.InsertLumberCost, HelperDataAccess.BuildParameters(costParams), conn, tran)
                                                                                   Next

                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error creating lumber cost effective date: " & effectiveDate.ToString("yyyy-MM-dd"))

            Return newID
        End Function

#End Region

#Region "LumberCost Operations"

        ''' <summary>
        ''' Gets all lumber costs for a specific effective date.
        ''' </summary>
        ''' <param name="costEffectiveDateID">The cost effective date ID to query.</param>
        ''' <returns>DataTable with LumberCostID, LumberTypeID, LumberTypeDesc, LumberCost columns.</returns>
        Public Function GetLumberCostsByEffectiveDate(costEffectiveDateID As Integer) As DataTable
            Dim dt As New DataTable()
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using cmd As New SqlCommand(Queries.SelectLumberCostsByEffectiveDate, conn)
                                                                               cmd.Parameters.AddWithValue("@CostEffectiveDateID", costEffectiveDateID)
                                                                               Using da As New SqlDataAdapter(cmd)
                                                                                   da.Fill(dt)
                                                                               End Using
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error fetching lumber costs for CostEffectiveDateID: " & costEffectiveDateID)
            Return dt
        End Function

        ''' <summary>
        ''' Updates the cost for a specific lumber cost record.
        ''' </summary>
        ''' <param name="lumberCostID">The lumber cost record ID.</param>
        ''' <param name="lumberCost">The new cost per MBF.</param>
        ''' <exception cref="ArgumentException">Thrown if cost is negative.</exception>
        Public Sub UpdateLumberCost(lumberCostID As Integer, lumberCost As Decimal)
            If lumberCost < 0 Then
                Throw New ArgumentException("Lumber cost cannot be negative.", NameOf(lumberCost))
            End If

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim params As New Dictionary(Of String, Object) From {
                                                                           {"@LumberCostID", lumberCostID},
                                                                           {"@LumberCost", lumberCost}
                                                                       }
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLumberCost, HelperDataAccess.BuildParameters(params))
                                                                   End Sub, "Error updating lumber cost for LumberCostID: " & lumberCostID)
        End Sub

#End Region

#Region "Lumber Adder Operations"

        ''' <summary>
        ''' Gets the lumber adder for a specific version and product type.
        ''' </summary>
        ''' <param name="versionID">The project version ID.</param>
        ''' <param name="productTypeID">The product type ID (1=Floor, 2=Roof, 3=Wall).</param>
        ''' <returns>Lumber adder amount per MBF, or 0 if not set.</returns>
        Public Shared Function GetLumberAdder(versionID As Integer, productTypeID As Integer) As Decimal
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectLumberAdder,
                                                                           {New SqlParameter("@VersionID", versionID), New SqlParameter("@ProductTypeID", productTypeID)})
                                                                   End Sub, "Error fetching lumber adder for version " & versionID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

#End Region

#Region "RawUnitLumberHistory Operations"

        ''' <summary>
        ''' Gets the latest active lumber history for a raw unit.
        ''' </summary>
        ''' <param name="rawUnitID">The raw unit ID.</param>
        ''' <param name="versionID">The project version ID.</param>
        ''' <returns>RawUnitLumberHistoryModel or Nothing if no history exists.</returns>
        Public Shared Function GetLatestLumberHistory(rawUnitID As Integer, versionID As Integer) As RawUnitLumberHistoryModel
            Dim history As RawUnitLumberHistoryModel = Nothing

            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader(
                                                                           Queries.SelectLatestActiveLumberHistory,
                                                                           {New SqlParameter("@RawUnitID", rawUnitID), New SqlParameter("@VersionID", versionID)})

                                                                           If reader.Read() Then
                                                                               history = MapLumberHistoryFromReader(reader)
                                                                           End If
                                                                       End Using
                                                                   End Sub, $"Error fetching latest lumber history for RawUnitID {rawUnitID}")

            Return history
        End Function

        ''' <summary>
        ''' Gets the average SPFNo2 price from active lumber history by product type.
        ''' </summary>
        ''' <param name="versionID">The project version ID.</param>
        ''' <param name="productTypeName">Product type name (e.g., "Floor", "Roof").</param>
        ''' <returns>Average SPFNo2 price or 0 if none found.</returns>
        Public Shared Function GetActiveSPFNo2ByProductType(versionID As Integer, productTypeName As String) As Decimal
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectActiveSPFNo2ByProductType,
                                                                           {New SqlParameter("@VersionID", versionID), New SqlParameter("@ProductTypeName", productTypeName)})
                                                                   End Sub, $"Error fetching active SPFNo2 for version {versionID}")
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        ''' <summary>
        ''' Gets the average SPFNo2 from RawUnits (not history) by product type.
        ''' </summary>
        ''' <param name="versionID">The project version ID.</param>
        ''' <param name="productTypeName">Product type name.</param>
        ''' <returns>Average SPFNo2 price or 0 if none found.</returns>
        Public Shared Function GetAverageSPFNo2ByProductType(versionID As Integer, productTypeName As String) As Decimal
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectAverageSPFNo2ByProductType,
                                                                           {New SqlParameter("@VersionID", versionID), New SqlParameter("@ProductTypeName", productTypeName)})
                                                                   End Sub, $"Error calculating average SPFNo2 for {productTypeName} in version {versionID}")
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        ''' <summary>Maps a SqlDataReader to RawUnitLumberHistoryModel.</summary>
        Private Shared Function MapLumberHistoryFromReader(reader As SqlDataReader) As RawUnitLumberHistoryModel
            Return New RawUnitLumberHistoryModel With {
                .HistoryID = reader.GetInt32(reader.GetOrdinal("HistoryID")),
                .RawUnitID = reader.GetInt32(reader.GetOrdinal("RawUnitID")),
                .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID")),
                .CostEffectiveDateID = If(Not reader.IsDBNull(reader.GetOrdinal("CostEffectiveDateID")),
                    reader.GetInt32(reader.GetOrdinal("CostEffectiveDateID")), Nothing),
                .LumberCost = reader.GetDecimal(reader.GetOrdinal("LumberCost")),
                .AvgSPFNo2 = If(Not reader.IsDBNull(reader.GetOrdinal("AvgSPFNo2")),
                    reader.GetDecimal(reader.GetOrdinal("AvgSPFNo2")), Nothing),
                .Avg241800 = If(Not reader.IsDBNull(reader.GetOrdinal("Avg241800")),
                    reader.GetDecimal(reader.GetOrdinal("Avg241800")), Nothing),
                .Avg242400 = If(Not reader.IsDBNull(reader.GetOrdinal("Avg242400")),
                    reader.GetDecimal(reader.GetOrdinal("Avg242400")), Nothing),
                .Avg261800 = If(Not reader.IsDBNull(reader.GetOrdinal("Avg261800")),
                    reader.GetDecimal(reader.GetOrdinal("Avg261800")), Nothing),
                .Avg262400 = If(Not reader.IsDBNull(reader.GetOrdinal("Avg262400")),
                    reader.GetDecimal(reader.GetOrdinal("Avg262400")), Nothing),
                .UpdateDate = reader.GetDateTime(reader.GetOrdinal("UpdateDate")),
                .IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
            }
        End Function

#End Region

#Region "Lumber Price Update Operations"

        ''' <summary>
        ''' Updates lumber prices for all raw units in a version based on new cost effective pricing.
        ''' Deactivates prior history records and creates new active records with adjusted costs.
        ''' Triggers version rollup recalculation after completion.
        ''' </summary>
        ''' <param name="versionID">The project version ID to update.</param>
        ''' <param name="costEffectiveDateID">The cost effective date ID with new pricing.</param>
        Public Shared Sub UpdateLumberPrices(versionID As Integer, costEffectiveDateID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim rawUnits As List(Of RawUnitModel) = ProjectDataAccess.GetRawUnitsByVersionID(versionID)
                                                                       If Not rawUnits.Any() Then
                                                                           Debug.WriteLine($"No RawUnits found for VersionID: {versionID}")
                                                                           Exit Sub
                                                                       End If

                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using tran As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   For Each rawUnit In rawUnits
                                                                                       ProcessRawUnitLumberUpdate(rawUnit, versionID, costEffectiveDateID, conn, tran)
                                                                                   Next
                                                                                   tran.Commit()
                                                                               Catch
                                                                                   tran.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using

                                                                       RollupCalculationService.RecalculateVersion(versionID)
                                                                   End Sub, $"Error updating lumber prices for version {versionID}")
        End Sub

        ''' <summary>Processes lumber cost update for a single raw unit.</summary>
        Private Shared Sub ProcessRawUnitLumberUpdate(rawUnit As RawUnitModel, versionID As Integer,
                                                      costEffectiveDateID As Integer, conn As SqlConnection, tran As SqlTransaction)
            Dim totalAdjustment As Decimal = 0D
            Dim allocatedBDFT As Decimal = 0D
            Dim newAvgPrices As New Dictionary(Of String, Decimal?)

            ' Process each lumber type
            For Each kvp In LumberTypeMappings
                Dim typeDesc As String = kvp.Value.Item1
                Dim avgField As String = kvp.Value.Item2
                Dim bdftField As String = kvp.Value.Item3

                ' Get LumberTypeID
                Dim typeParams As New Dictionary(Of String, Object) From {{"@LumberTypeDesc", typeDesc}}
                Dim lumberTypeIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                    Queries.SelectLumberTypeIDByDesc, HelperDataAccess.BuildParameters(typeParams))
                If lumberTypeIDObj Is DBNull.Value Then Continue For

                Dim lumberTypeID As Integer = CInt(lumberTypeIDObj)

                ' Get new cost
                Dim costParams As New Dictionary(Of String, Object) From {
                    {"@LumberTypeID", lumberTypeID},
                    {"@CostEffectiveDateID", costEffectiveDateID}
                }
                Dim newCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                    Queries.SelectLumberCostByTypeAndDate, HelperDataAccess.BuildParameters(costParams))
                If newCostObj Is DBNull.Value Then Continue For

                Dim newCost As Decimal = CDec(newCostObj)
                If newCost <= 0D Then
                    newAvgPrices(avgField) = Nothing
                    Continue For
                End If
                newAvgPrices(avgField) = newCost

                ' Get old cost and BDFT using reflection
                Dim oldCost As Decimal? = CType(CallByName(rawUnit, avgField, CallType.Get), Decimal?)
                Dim bdft As Decimal? = CType(CallByName(rawUnit, bdftField, CallType.Get), Decimal?)

                If Not oldCost.HasValue OrElse oldCost.Value <= 0 OrElse Not bdft.HasValue OrElse bdft.Value <= 0 Then
                    Continue For
                End If

                ' Compute adjustment
                Dim diffPerThousand As Decimal = newCost - oldCost.Value
                totalAdjustment += diffPerThousand * (bdft.Value / 1000D)
                allocatedBDFT += bdft.Value
            Next

            ' Handle unallocated BDFT
            totalAdjustment += CalculateUnallocatedAdjustment(rawUnit, allocatedBDFT, costEffectiveDateID)

            ' Skip if no changes
            If totalAdjustment = 0D AndAlso newAvgPrices.Count = 0 Then Return

            ' Calculate updated lumber cost (prevent negative)
            Dim baseLumberCost As Decimal = If(rawUnit.LumberCost, 0D)
            Dim updatedLumberCost As Decimal = Math.Max(baseLumberCost + totalAdjustment, baseLumberCost)

            ' Deactivate existing records
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.DeactivateLumberHistory,
                HelperDataAccess.BuildParameters(New Dictionary(Of String, Object) From {
                    {"@RawUnitID", rawUnit.RawUnitID},
                    {"@VersionID", versionID}
                }), conn, tran)

            ' Insert new active record
            InsertLumberHistoryRecord(rawUnit.RawUnitID, versionID, costEffectiveDateID, updatedLumberCost, newAvgPrices, conn, tran)
        End Sub

        ''' <summary>Calculates adjustment for unallocated BDFT using SPF#2 pricing.</summary>
        Private Shared Function CalculateUnallocatedAdjustment(rawUnit As RawUnitModel, allocatedBDFT As Decimal,
                                                                costEffectiveDateID As Integer) As Decimal
            If Not rawUnit.BF.HasValue OrElse rawUnit.BF.Value <= allocatedBDFT Then Return 0D

            Dim unallocatedBDFT As Decimal = rawUnit.BF.Value - allocatedBDFT
            If unallocatedBDFT <= 0 Then Return 0D

            Dim typeParams As New Dictionary(Of String, Object) From {{"@LumberTypeDesc", "2x4 SPF #2"}}
            Dim lumberTypeIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                Queries.SelectLumberTypeIDByDesc, HelperDataAccess.BuildParameters(typeParams))
            If lumberTypeIDObj Is DBNull.Value Then Return 0D

            Dim costParams As New Dictionary(Of String, Object) From {
                {"@LumberTypeID", CInt(lumberTypeIDObj)},
                {"@CostEffectiveDateID", costEffectiveDateID}
            }
            Dim newCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                Queries.SelectLumberCostByTypeAndDate, HelperDataAccess.BuildParameters(costParams))
            If newCostObj Is DBNull.Value Then Return 0D

            Dim newCost As Decimal = CDec(newCostObj)
            If newCost <= 0D OrElse Not rawUnit.AvgSPFNo2.HasValue OrElse rawUnit.AvgSPFNo2.Value <= 0D Then Return 0D

            Return (newCost - rawUnit.AvgSPFNo2.Value) * (unallocatedBDFT / 1000D)
        End Function

        ''' <summary>Inserts a new lumber history record with IsActive = 1.</summary>
        Private Shared Sub InsertLumberHistoryRecord(rawUnitID As Integer, versionID As Integer,
                                                     costEffectiveDateID As Integer, lumberCost As Decimal,
                                                     avgPrices As Dictionary(Of String, Decimal?),
                                                     conn As SqlConnection, tran As SqlTransaction)
            Dim params As New Dictionary(Of String, Object) From {
                {"@RawUnitID", rawUnitID},
                {"@VersionID", versionID},
                {"@CostEffectiveDateID", costEffectiveDateID},
                {"@LumberCost", lumberCost},
                {"@AvgSPFNo2", TableOperations.ToDb(If(avgPrices.ContainsKey("AvgSPFNo2"), avgPrices("AvgSPFNo2"), Nothing))},
                {"@Avg241800", TableOperations.ToDb(If(avgPrices.ContainsKey("Avg241800"), avgPrices("Avg241800"), Nothing))},
                {"@Avg242400", TableOperations.ToDb(If(avgPrices.ContainsKey("Avg242400"), avgPrices("Avg242400"), Nothing))},
                {"@Avg261800", TableOperations.ToDb(If(avgPrices.ContainsKey("Avg261800"), avgPrices("Avg261800"), Nothing))},
                {"@Avg262400", TableOperations.ToDb(If(avgPrices.ContainsKey("Avg262400"), avgPrices("Avg262400"), Nothing))},
                {"@IsActive", 1}
            }

            Using cmd As New SqlCommand(Queries.InsertRawUnitLumberHistory, conn, tran)
                cmd.CommandTimeout = 0
                cmd.Parameters.AddRange(HelperDataAccess.BuildParameters(params))
                cmd.ExecuteScalar()
            End Using
        End Sub

#End Region

#Region "Lumber Futures Operations"

        ''' <summary>
        ''' Scrapes lumber futures settle prices from CME Group website.
        ''' Uses Selenium WebDriver to navigate and parse the settlements table.
        ''' </summary>
        ''' <returns>List of LumberFutures with ContractMonth and PriorSettle values.</returns>
        ''' <remarks>Requires Microsoft Edge and WebDriver to be installed.</remarks>
        Public Shared Function GetLumberFutures() As List(Of LumberFutures)
            Dim futuresData As New List(Of LumberFutures)

            Dim options As New EdgeOptions()
            options.AddArgument("--no-sandbox")
            options.AddArgument("--disable-dev-shm-usage")
            options.AddArgument("--disable-gpu")
            options.AddArgument("--disable-software-rasterizer")
            options.AddArgument("--window-size=1920,1080")

            Using driver As IWebDriver = New EdgeDriver(options)
                driver.Navigate().GoToUrl("https://www.cmegroup.com/markets/agriculture/lumber-and-softs/lumber.settlements.html")

                Dim wait As New WebDriverWait(driver, TimeSpan.FromSeconds(7))
                Dim tableElement As IWebElement = Nothing

                Try
                    tableElement = wait.Until(Function(d)
                                                  Dim tables = d.FindElements(By.CssSelector("div.table-container table, table"))
                                                  Return If(tables.Count > 0, tables(0), Nothing)
                                              End Function)
                Catch
                    tableElement = wait.Until(Function(d)
                                                  Dim els = d.FindElements(By.XPath("//table[.//th[contains(text(), 'Settle') or contains(text(), 'Change')]]"))
                                                  Return If(els.Count > 0, els(0), Nothing)
                                              End Function)
                End Try

                If tableElement Is Nothing Then Return futuresData

                For Each row In tableElement.FindElements(By.CssSelector("tbody tr"))
                    Dim cells = row.FindElements(By.TagName("td"))
                    If cells.Count >= 7 Then
                        Dim monthText = cells(0).Text.Trim()
                        If String.IsNullOrEmpty(monthText) OrElse monthText.Contains("Month") Then Continue For

                        Dim settle As Decimal? = Nothing
                        Dim parsed As Decimal = 0D
                        If Decimal.TryParse(cells(6).Text.Trim(), parsed) Then settle = parsed

                        futuresData.Add(New LumberFutures With {
                            .ContractMonth = monthText,
                            .PriorSettle = settle
                        })
                    End If
                Next
            End Using

            Return futuresData
        End Function

        ''' <summary>
        ''' Saves lumber futures data for a version.
        ''' </summary>
        ''' <param name="versionId">The project version ID.</param>
        ''' <param name="futures">List of futures to save.</param>
        ''' <param name="conn">Open SQL connection.</param>
        ''' <param name="tran">Active transaction.</param>
        Public Shared Sub SaveLumberFutures(versionId As Integer, futures As List(Of LumberFutures),
                                            conn As SqlConnection, tran As SqlTransaction)
            For Each f As LumberFutures In futures
                Dim params As New Dictionary(Of String, Object) From {
                    {"@VersionID", versionId},
                    {"@ContractMonth", f.ContractMonth},
                    {"@PriorSettle", TableOperations.ToDb(f.PriorSettle)}
                }
                Using cmd As New SqlCommand(Queries.InsertLumberFuture, conn, tran)
                    cmd.Parameters.AddRange(HelperDataAccess.BuildParameters(params))
                    cmd.ExecuteNonQuery()
                End Using
            Next
        End Sub

        ''' <summary>
        ''' Gets all futures for a version.
        ''' </summary>
        ''' <param name="versionId">The project version ID.</param>
        ''' <returns>List of LumberFutures ordered by PullDate.</returns>
        Public Shared Function GetFuturesForVersion(versionId As Integer) As List(Of LumberFutures)
            Dim result As New List(Of LumberFutures)
            If versionId <= 0 Then Return result

            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using cmd As New SqlCommand(Queries.SelectLumberFuturesByVersion, conn)
                    cmd.Parameters.AddWithValue("@VersionID", versionId)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            result.Add(New LumberFutures With {
                                .LumberFutureID = rdr.GetInt32(0),
                                .ContractMonth = rdr.GetString(1).Trim(),
                                .PriorSettle = If(rdr.IsDBNull(2), Nothing, CDec(rdr.GetDecimal(2))),
                                .PullDate = rdr.GetDateTime(3),
                                .Active = Not rdr.IsDBNull(4) AndAlso rdr.GetBoolean(4)
                            })
                        End While
                    End Using
                End Using
            End Using

            Return result
        End Function

        ''' <summary>
        ''' Sets a specific lumber future as active, deactivating all others for the version.
        ''' </summary>
        ''' <param name="versionId">The project version ID.</param>
        ''' <param name="lumberFutureId">The lumber future ID to activate.</param>
        Public Shared Sub SetActiveLumberFuture(versionId As Integer, lumberFutureId As Integer)
            Using conn = SqlConnectionManager.Instance.GetConnection()
                Using tran = conn.BeginTransaction()
                    Try
                        SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.SetActiveLumberFuture,
                            {New SqlParameter("@VersionID", versionId), New SqlParameter("@LumberFutureID", lumberFutureId)},
                            conn, tran)
                        tran.Commit()
                    Catch
                        tran.Rollback()
                        Throw
                    End Try
                End Using
            End Using
        End Sub

#End Region

#Region "Utility Methods"

        ''' <summary>
        ''' Converts three-letter month abbreviation to month number.
        ''' </summary>
        ''' <param name="name">Month abbreviation (e.g., "JAN", "FEB").</param>
        ''' <returns>Month number (1-12) or 0 if not recognized.</returns>
        Public Shared Function MonthNameToNumber(name As String) As Integer
            Select Case name.ToUpperInvariant()
                Case "JAN" : Return 1
                Case "FEB" : Return 2
                Case "MAR" : Return 3
                Case "APR" : Return 4
                Case "MAY" : Return 5
                Case "JUN" : Return 6
                Case "JLY", "JUL" : Return 7
                Case "AUG" : Return 8
                Case "SEP" : Return 9
                Case "OCT" : Return 10
                Case "NOV" : Return 11
                Case "DEC" : Return 12
                Case Else : Return 0
            End Select
        End Function

#End Region

    End Class
End Namespace