
Imports System.Data.SqlClient

Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.Utilities

Imports OpenQA.Selenium
Imports OpenQA.Selenium.Edge
Imports OpenQA.Selenium.Support.UI



Namespace DataAccess


    Public Class LumberDataAccess

        ' LumberType Methods
        Public Function GetAllLumberTypes() As DataTable
            Dim dt As New DataTable()
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Dim cmd As New SqlCommand(Queries.SelectLumberTypes, conn)
                                                                           Dim da As New SqlDataAdapter(cmd)
                                                                           da.Fill(dt)
                                                                       End Using
                                                                   End Sub, "Error fetching lumber types")
            Return dt
        End Function

        Public Function CreateLumberType(lumberTypeDesc As String) As Integer
            If String.IsNullOrWhiteSpace(lumberTypeDesc) Then
                Throw New ArgumentException("LumberTypeDesc cannot be null or empty.")
            End If

            Dim newLumberTypeID As Integer
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim params As New Dictionary(Of String, Object) From {
                                                                               {"@LumberTypeDesc", lumberTypeDesc}
                                                                           }
                                                                       Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.InsertLumberType, HelperDataAccess.BuildParameters(params))
                                                                       newLumberTypeID = CInt(newIDObj)
                                                                   End Sub, "Error creating lumber type: " & lumberTypeDesc)
            Return newLumberTypeID
        End Function

        ' LumberCostEffective Methods
        Public Shared Function GetAllLumberCostEffective() As DataTable
            Dim dt As New DataTable()
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Dim cmd As New SqlCommand(Queries.SelectLumberCostEffective, conn)
                                                                           Dim da As New SqlDataAdapter(cmd)
                                                                           da.Fill(dt)
                                                                       End Using
                                                                   End Sub, "Error fetching lumber cost effective dates")
            Return dt
        End Function

        Public Function CreateLumberCostEffective(effectiveDate As Date) As Integer
            Dim newCostEffectiveID As Integer
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using transaction As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   ' Insert new LumberCostEffective record
                                                                                   Dim params As New Dictionary(Of String, Object) From {
                                                                                           {"@CosteffectiveDate", effectiveDate}
                                                                                       }
                                                                                   Dim newIDObj As Object = SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Object)(Queries.InsertLumberCostEffective, HelperDataAccess.BuildParameters(params), conn, transaction)
                                                                                   newCostEffectiveID = CInt(newIDObj)

                                                                                   ' Get all LumberType IDs
                                                                                   Dim lumberTypes As DataTable = GetAllLumberTypes()
                                                                                   For Each row As DataRow In lumberTypes.Rows
                                                                                       Dim lumberTypeID As Integer = CInt(row("LumberTypeID"))
                                                                                       ' Insert LumberCost record for each LumberType with default cost 0.00
                                                                                       params = New Dictionary(Of String, Object) From {
                                                                                               {"@LumberTypeID", lumberTypeID},
                                                                                               {"@LumberCost", 0.00D},
                                                                                               {"@CostEffectiveDateID", newCostEffectiveID}
                                                                                           }
                                                                                       SqlConnectionManager.Instance.ExecuteNonQueryTransactional(Queries.InsertLumberCost, HelperDataAccess.BuildParameters(params), conn, transaction)
                                                                                   Next

                                                                                   transaction.Commit()
                                                                               Catch ex As Exception
                                                                                   transaction.Rollback()
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using
                                                                   End Sub, "Error creating lumber cost effective date: " & effectiveDate.ToString("yyyy-MM-dd"))
            Return newCostEffectiveID
        End Function

        ' LumberCost Methods
        Public Function GetLumberCostsByEffectiveDate(costEffectiveDateID As Integer) As DataTable
            Dim dt As New DataTable()
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Dim cmd As New SqlCommand(Queries.SelectLumberCostsByEffectiveDate, conn)
                                                                           cmd.Parameters.Add(New SqlParameter("@CostEffectiveDateID", costEffectiveDateID))
                                                                           Dim da As New SqlDataAdapter(cmd)
                                                                           da.Fill(dt)
                                                                       End Using
                                                                   End Sub, "Error fetching lumber costs for CostEffectiveDateID: " & costEffectiveDateID)
            Return dt
        End Function

        Public Sub UpdateLumberCost(lumberCostID As Integer, lumberCost As Decimal)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       If lumberCost < 0 Then
                                                                           Throw New ArgumentException("Lumber cost cannot be negative.")
                                                                       End If
                                                                       Dim params As New Dictionary(Of String, Object) From {
                                                                               {"@LumberCostID", lumberCostID},
                                                                               {"@LumberCost", lumberCost}
                                                                           }
                                                                       SqlConnectionManager.Instance.ExecuteNonQuery(Queries.UpdateLumberCost, HelperDataAccess.BuildParameters(params))
                                                                   End Sub, "Error updating lumber cost for LumberCostID: " & lumberCostID)
        End Sub


        ' In LumberDataAccess.vb, within BuildersPSE.DataAccess namespace
        ' Modified UpdateLumberPrices to set IsActive=1 and deactivate other records
        ' Explanation: Updates InsertRawUnitLumberHistory to include IsActive=1, deactivates prior records per RawUnitID.
        ' Minimal change: Adds deactivation step and updates historyParams.
        ' Ensures only latest update is active for rollups (Deming's process control) and auditable data (Servant leadership).

        Public Shared Sub UpdateLumberPrices(versionID As Integer, costEffectiveDateID As Integer)
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       ' Step 1: Fetch all RawUnits for the version
                                                                       Dim rawUnits As List(Of RawUnitModel) = ProjectDataAccess.GetRawUnitsByVersionID(versionID)
                                                                       If Not rawUnits.Any() Then
                                                                           Debug.WriteLine("No RawUnits found for VersionID: " & versionID)
                                                                           Exit Sub
                                                                       End If

                                                                       ' Define lumber types with exact database LumberTypeDesc values
                                                                       Dim lumberTypes As New Dictionary(Of String, Tuple(Of String, String, String)) From {
                                                                            {"SPFNo2", Tuple.Create("2x4 SPF #2", "AvgSPFNo2", "SPFNo2BDFT")},
                                                                            {"MSR241800", Tuple.Create("2x4 DF-N 1800F 1.6E", "Avg241800", "MSR241800BDFT")},
                                                                            {"MSR242400", Tuple.Create("2x4 DF-N 2400F 2.0E", "Avg242400", "MSR242400BDFT")},
                                                                            {"MSR261800", Tuple.Create("2x6 DF-N 1800F 1.6E", "Avg261800", "MSR261800BDFT")},
                                                                            {"MSR262400", Tuple.Create("2x6 DF-N 2400F 2.0E", "Avg262400", "MSR262400BDFT")}
                                                                        }

                                                                       Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                           conn.Open()
                                                                           Using transaction As SqlTransaction = conn.BeginTransaction()
                                                                               Try
                                                                                   For Each rawUnit In rawUnits

                                                                                       Dim totalAdjustment As Decimal = 0D
                                                                                       Dim allocatedBDFT As Decimal = 0D
                                                                                       Dim newAvgPrices As New Dictionary(Of String, Decimal?)

                                                                                       ' Step 2: Process each lumber type
                                                                                       For Each kvp In lumberTypes
                                                                                           Dim typeKey As String = kvp.Key
                                                                                           Dim typeDesc As String = kvp.Value.Item1
                                                                                           Dim avgField As String = kvp.Value.Item2
                                                                                           Dim bdftField As String = kvp.Value.Item3

                                                                                           ' Get LumberTypeID
                                                                                           Dim typeParams As New Dictionary(Of String, Object) From {{"@LumberTypeDesc", typeDesc}}
                                                                                           Dim lumberTypeIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectLumberTypeIDByDesc, HelperDataAccess.BuildParameters(typeParams))
                                                                                           If lumberTypeIDObj Is DBNull.Value Then
                                                                                               Debug.WriteLine($"LumberTypeDesc not found for {typeKey}: '{typeDesc}'")
                                                                                               Continue For
                                                                                           End If
                                                                                           Dim lumberTypeID As Integer = CInt(lumberTypeIDObj)
                                                                                           Debug.WriteLine($"LumberTypeID for {typeKey}: {lumberTypeID}")

                                                                                           ' Get new cost
                                                                                           Dim costParams As New Dictionary(Of String, Object) From {
                                                                                                {"@LumberTypeID", lumberTypeID},
                                                                                                {"@CostEffectiveDateID", costEffectiveDateID}
                                                                                            }
                                                                                           Dim newCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectLumberCostByTypeAndDate, HelperDataAccess.BuildParameters(costParams))
                                                                                           If newCostObj Is DBNull.Value Then
                                                                                               Debug.WriteLine($"No LumberCost found for LumberTypeID {lumberTypeID} and CostEffectiveDateID {costEffectiveDateID} - skipping {typeKey}")
                                                                                               Continue For
                                                                                           End If
                                                                                           Dim newCost As Decimal = CDec(newCostObj)
                                                                                           If newCost <= 0D Then
                                                                                               Debug.WriteLine($"Invalid newCost <= 0 ({newCost}) for {typeKey} - skipping adjustment")
                                                                                               newAvgPrices(avgField) = Nothing
                                                                                               Continue For
                                                                                           End If
                                                                                           newAvgPrices(avgField) = newCost
                                                                                           Debug.WriteLine($"NewCost for {typeKey}: {newCost}")

                                                                                           ' Get old cost and BDFT
                                                                                           Dim oldCost As Decimal? = CType(CallByName(rawUnit, avgField, CallType.Get), Decimal?)
                                                                                           Dim bdft As Decimal? = CType(CallByName(rawUnit, bdftField, CallType.Get), Decimal?)
                                                                                           If Not oldCost.HasValue OrElse oldCost.Value <= 0 OrElse Not bdft.HasValue OrElse bdft.Value <= 0 Then

                                                                                               Continue For
                                                                                           End If

                                                                                           ' Compute adjustment
                                                                                           Dim diffPerThousand As Decimal = newCost - oldCost.Value
                                                                                           Dim adjustment As Decimal = diffPerThousand * (bdft.Value / 1000)
                                                                                           totalAdjustment += adjustment
                                                                                           allocatedBDFT += bdft.Value
                                                                                           Debug.WriteLine($"{typeKey}: oldCost={oldCost.Value}, newCost={newCost}, bdft={bdft.Value}, diffPerThousand={diffPerThousand}, adjustment={adjustment}")
                                                                                       Next

                                                                                       ' Step 3: Handle unallocated BDFT
                                                                                       If rawUnit.BF.HasValue AndAlso rawUnit.BF.Value > allocatedBDFT Then
                                                                                           Dim unallocatedBDFT As Decimal = rawUnit.BF.Value - allocatedBDFT
                                                                                           If unallocatedBDFT > 0 Then
                                                                                               Dim typeParams As New Dictionary(Of String, Object) From {{"@LumberTypeDesc", "2x4 SPF #2"}}
                                                                                               Dim lumberTypeIDObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectLumberTypeIDByDesc, HelperDataAccess.BuildParameters(typeParams))
                                                                                               If lumberTypeIDObj IsNot DBNull.Value Then
                                                                                                   Dim lumberTypeID As Integer = CInt(lumberTypeIDObj)
                                                                                                   Dim costParams As New Dictionary(Of String, Object) From {
                                                                                                        {"@LumberTypeID", lumberTypeID},
                                                                                                        {"@CostEffectiveDateID", costEffectiveDateID}
                                                                                                    }
                                                                                                   Dim newCostObj As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectLumberCostByTypeAndDate, HelperDataAccess.BuildParameters(costParams))
                                                                                                   If newCostObj IsNot DBNull.Value Then
                                                                                                       Dim newCost As Decimal = CDec(newCostObj)
                                                                                                       If newCost > 0D Then
                                                                                                           Dim oldCost As Decimal? = rawUnit.AvgSPFNo2
                                                                                                           If oldCost.HasValue AndAlso oldCost.Value > 0 Then
                                                                                                               Dim diffPerThousand As Decimal = newCost - oldCost.Value
                                                                                                               Dim unallocatedAdjustment As Decimal = diffPerThousand * (unallocatedBDFT / 1000)
                                                                                                               totalAdjustment += unallocatedAdjustment
                                                                                                               Debug.WriteLine($"Unallocated adjustment: bdft={unallocatedBDFT}, diffPerThousand={diffPerThousand}, adjustment={unallocatedAdjustment}")

                                                                                                           End If
                                                                                                       Else
                                                                                                           Debug.WriteLine($"Invalid newCost ({newCost}) for SPFNo2, CostEffectiveDateID: {costEffectiveDateID}")
                                                                                                       End If
                                                                                                   Else
                                                                                                       Debug.WriteLine($"No LumberCost for SPFNo2, CostEffectiveDateID: {costEffectiveDateID}")
                                                                                                   End If
                                                                                               Else
                                                                                                   Debug.WriteLine("SPFNo2 LumberTypeID not found for unallocated")
                                                                                               End If
                                                                                           End If
                                                                                       End If

                                                                                       ' Step 4: Insert RawUnitLumberHistory (only if changes)
                                                                                       If totalAdjustment = 0 AndAlso newAvgPrices.Count = 0 Then
                                                                                           Debug.WriteLine($"No changes for RawUnitID: {rawUnit.RawUnitID} - skipping history insert")
                                                                                           Continue For
                                                                                       End If
                                                                                       Dim baseLumberCost As Decimal = If(rawUnit.LumberCost, 0D)
                                                                                       Dim updatedLumberCost As Decimal = baseLumberCost + totalAdjustment
                                                                                       If updatedLumberCost < 0 Then
                                                                                           Debug.WriteLine($"Warning: Negative LumberCost ({updatedLumberCost}) for RawUnitID: {rawUnit.RawUnitID}, baseLumberCost={baseLumberCost}, totalAdjustment={totalAdjustment}")
                                                                                           updatedLumberCost = baseLumberCost ' Prevent negative costs
                                                                                       End If
                                                                                       ' Deactivate existing records for this RawUnitID
                                                                                       Dim deactivateParams As New Dictionary(Of String, Object) From {
                                                                                                {"@RawUnitID", rawUnit.RawUnitID},
                                                                                                {"@VersionID", versionID}
                                                                                            }
                                                                                       SqlConnectionManager.Instance.ExecuteNonQueryTransactional("UPDATE RawUnitLumberHistory SET IsActive = 0 WHERE RawUnitID = @RawUnitID AND VersionID = @VersionID", HelperDataAccess.BuildParameters(deactivateParams), conn, transaction)

                                                                                       ' Insert new record with IsActive=1
                                                                                       Dim historyParams As New Dictionary(Of String, Object) From {
                                                                                            {"@RawUnitID", rawUnit.RawUnitID},
                                                                                            {"@VersionID", versionID},
                                                                                            {"@CostEffectiveDateID", costEffectiveDateID},
                                                                                            {"@LumberCost", updatedLumberCost},
                                                                                            {"@AvgSPFNo2", If(newAvgPrices.ContainsKey("AvgSPFNo2") AndAlso newAvgPrices("AvgSPFNo2").HasValue, CType(newAvgPrices("AvgSPFNo2").Value, Object), DBNull.Value)},
                                                                                            {"@Avg241800", If(newAvgPrices.ContainsKey("Avg241800") AndAlso newAvgPrices("Avg241800").HasValue, CType(newAvgPrices("Avg241800").Value, Object), DBNull.Value)},
                                                                                            {"@Avg242400", If(newAvgPrices.ContainsKey("Avg242400") AndAlso newAvgPrices("Avg242400").HasValue, CType(newAvgPrices("Avg242400").Value, Object), DBNull.Value)},
                                                                                            {"@Avg261800", If(newAvgPrices.ContainsKey("Avg261800") AndAlso newAvgPrices("Avg261800").HasValue, CType(newAvgPrices("Avg261800").Value, Object), DBNull.Value)},
                                                                                            {"@Avg262400", If(newAvgPrices.ContainsKey("Avg262400") AndAlso newAvgPrices("Avg262400").HasValue, CType(newAvgPrices("Avg262400").Value, Object), DBNull.Value)},
                                                                                            {"@IsActive", 1}
                                                                                          }
                                                                                       Dim cmdHistory As New SqlCommand(Queries.InsertRawUnitLumberHistory, conn, transaction) With {
                                                                                            .CommandTimeout = 0
                                                                                        }
                                                                                       cmdHistory.Parameters.AddRange(HelperDataAccess.BuildParameters(historyParams))
                                                                                       Dim historyID As Integer = CInt(cmdHistory.ExecuteScalar())
                                                                                       Debug.WriteLine($"Inserted HistoryID {historyID} for RawUnitID {rawUnit.RawUnitID}: LumberCost={updatedLumberCost}, IsActive=1")
                                                                                   Next

                                                                                   transaction.Commit()
                                                                                   Debug.WriteLine("Transaction committed for VersionID: " & versionID)
                                                                               Catch ex As Exception
                                                                                   transaction.Rollback()
                                                                                   Debug.WriteLine("Transaction rolled back: " & ex.Message)
                                                                                   Throw
                                                                               End Try
                                                                           End Using
                                                                       End Using

                                                                       ' Step 5: Recalculate rollups
                                                                       RollupDataAccess.RecalculateVersion(versionID)
                                                                   End Sub, "Error updating lumber prices for version " & versionID)
        End Sub
        ' In DataAccess.vb: Add method to get average AvgSPFNo2 for floors and roofs
        Public Shared Function GetAverageSPFNo2ByProductType(versionID As Integer, productTypeName As String) As Decimal
            Dim avgSPFNo2 As Decimal = 0D
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Dim params As New Dictionary(Of String, Object) From {
                                                                           {"@VersionID", versionID},
                                                                           {"@ProductTypeName", productTypeName}
                                                                       }
                                                                       Dim query As String = "SELECT AVG(ru.AvgSPFNo2) FROM RawUnits ru INNER JOIN ProductType pt ON ru.ProductTypeID = pt.ProductTypeID WHERE ru.VersionID = @VersionID AND pt.ProductTypeName = @ProductTypeName AND ru.AvgSPFNo2 IS NOT NULL"
                                                                       Dim result As Object = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(query, HelperDataAccess.BuildParameters(params))
                                                                       If result IsNot DBNull.Value Then
                                                                           avgSPFNo2 = CDec(result)
                                                                       End If
                                                                   End Sub, $"Error calculating average SPFNo2 for {productTypeName} in version {versionID}")
            Return avgSPFNo2
        End Function
        ' Updated: GetLumberAdder (use VersionID)
        Public Shared Function GetLumberAdder(versionID As Integer, productTypeID As Integer) As Decimal
            Dim params As SqlParameter() = {New SqlParameter("@VersionID", versionID), New SqlParameter("@ProductTypeID", productTypeID)}
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(Queries.SelectLumberAdder, params)
                                                                   End Sub, "Error fetching lumber adder for version " & versionID)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function
        Public Shared Function GetLatestLumberHistory(rawUnitID As Integer, versionID As Integer) As RawUnitLumberHistoryModel
            Dim history As RawUnitLumberHistoryModel = Nothing
            Dim params As SqlParameter() = {
        New SqlParameter("@RawUnitID", rawUnitID),
        New SqlParameter("@VersionID", versionID)
    }
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       Using reader As SqlDataReader = SqlConnectionManager.Instance.ExecuteReader("SELECT TOP 1 * FROM RawUnitLumberHistory WHERE RawUnitID = @RawUnitID AND VersionID = @VersionID AND IsActive = 1 ORDER BY UpdateDate DESC", params)
                                                                           If reader.Read() Then
                                                                               history = New RawUnitLumberHistoryModel With {
                                                                                    .HistoryID = reader.GetInt32(reader.GetOrdinal("HistoryID")),
                                                                                    .RawUnitID = reader.GetInt32(reader.GetOrdinal("RawUnitID")),
                                                                                    .VersionID = reader.GetInt32(reader.GetOrdinal("VersionID")),
                                                                                    .CostEffectiveDateID = If(Not reader.IsDBNull(reader.GetOrdinal("CostEffectiveDateID")), reader.GetInt32(reader.GetOrdinal("CostEffectiveDateID")), Nothing),
                                                                                    .LumberCost = reader.GetDecimal(reader.GetOrdinal("LumberCost")),
                                                                                    .AvgSPFNo2 = If(Not reader.IsDBNull(reader.GetOrdinal("AvgSPFNo2")), reader.GetDecimal(reader.GetOrdinal("AvgSPFNo2")), Nothing),
                                                                                    .Avg241800 = If(Not reader.IsDBNull(reader.GetOrdinal("Avg241800")), reader.GetDecimal(reader.GetOrdinal("Avg241800")), Nothing),
                                                                                    .Avg242400 = If(Not reader.IsDBNull(reader.GetOrdinal("Avg242400")), reader.GetDecimal(reader.GetOrdinal("Avg242400")), Nothing),
                                                                                    .Avg261800 = If(Not reader.IsDBNull(reader.GetOrdinal("Avg261800")), reader.GetDecimal(reader.GetOrdinal("Avg261800")), Nothing),
                                                                                    .Avg262400 = If(Not reader.IsDBNull(reader.GetOrdinal("Avg262400")), reader.GetDecimal(reader.GetOrdinal("Avg262400")), Nothing),
                                                                                    .UpdateDate = reader.GetDateTime(reader.GetOrdinal("UpdateDate")),
                                                                                    .IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                                                                                }
                                                                           End If
                                                                       End Using
                                                                   End Sub, "Error fetching latest lumber history for RawUnitID " & rawUnitID & " and VersionID " & versionID)
            Return history
        End Function

        Public Shared Function GetActiveSPFNo2ByProductType(versionID As Integer, productType As String) As Decimal
            Dim params As SqlParameter() = {
                New SqlParameter("@VersionID", versionID),
                New SqlParameter("@ProductTypeName", productType)
            }
            Dim valObj As Object = Nothing
            SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                       valObj = SqlConnectionManager.Instance.ExecuteScalar(Of Object)(
                                                                           "SELECT AVG(rlh.AvgSPFNo2) " &
                                                                           "FROM RawUnitLumberHistory rlh " &
                                                                           "JOIN RawUnits ru ON rlh.RawUnitID = ru.RawUnitID " &
                                                                           "JOIN ProductType pt ON ru.ProductTypeID = pt.ProductTypeID " &
                                                                           "WHERE rlh.VersionID = @VersionID AND pt.ProductTypeName = @ProductTypeName AND rlh.IsActive = 1",
                                                                           params
                                                                       )
                                                                   End Sub, "Error fetching active SPFNo2 price for version " & versionID & " and product type " & productType)
            Return If(valObj Is DBNull.Value OrElse valObj Is Nothing, 0D, CDec(valObj))
        End Function

        ''' <summary>
        ''' Scrapes lumber futures settle prices by month from CME Group (free, no key).
        ''' Returns list of (Month, Settle). Runs headless for daily pulls.
        ''' </summary>https://www.cmegroup.com/markets/agriculture/lumber-and-softs/lumber.settlements.html

        Public Shared Function GetLumberFutures() As List(Of LumberFutures)
            Dim futuresData As New List(Of LumberFutures)

            Dim options As New EdgeOptions()
            options.AddArgument("--no-sandbox")
            options.AddArgument("--disable-dev-shm-usage")
            options.AddArgument("--disable-gpu")
            options.AddArgument("--disable-software-rasterizer")
            options.AddArgument("--window-size=1920,1080")
            'options.AddArgument("--headless=new")

            'Dim driverService = EdgeDriverService.CreateDefaultService()
            'driverService.HideCommandPromptWindow = True

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

                ' Extract rows
                Dim rows = tableElement.FindElements(By.CssSelector("tbody tr"))
                For Each row In rows
                    Dim cells = row.FindElements(By.TagName("td"))
                    If cells.Count >= 7 Then
                        Dim monthText = cells(0).Text.Trim()
                        If String.IsNullOrEmpty(monthText) OrElse monthText.Contains("Month") Then Continue For

                        Dim settleText = cells(6).Text.Trim()
                        Dim settle As Decimal? = Nothing
                        Dim parsed As Decimal = 0D
                        If Decimal.TryParse(settleText, parsed) Then settle = parsed

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
        ''' Saves or updates lumber futures data for a version using the LumberFutures class.
        ''' </summary>
        Public Shared Sub SaveLumberFutures(versionId As Integer, futures As List(Of LumberFutures), conn As SqlConnection, tran As SqlTransaction)
            For Each f As LumberFutures In futures
                Using cmd As New SqlCommand(Queries.InsertLumberFuture, conn, tran)
                    cmd.Parameters.AddWithValue("@VersionID", versionId)
                    cmd.Parameters.AddWithValue("@ContractMonth", f.ContractMonth)
                    cmd.Parameters.AddWithValue("@PriorSettle", If(f.PriorSettle.HasValue, CType(f.PriorSettle.Value, Object), DBNull.Value))
                    ' PullDate is set by GETDATE() in the query → always current
                    cmd.ExecuteNonQuery()
                End Using
            Next
        End Sub
        ''' <summary>
        ''' Returns sorted list of futures for display: (Text, ID, Price)
        ''' </summary>
        Public Shared Function GetFuturesForVersion(versionId As Integer) As List(Of LumberFutures)
            Dim result As New List(Of LumberFutures)
            If versionId <= 0 Then
                Debug.WriteLine("GetFuturesForVersion: Invalid versionId")
                Return result
            End If

            Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                conn.Open()
                Using cmd As New SqlCommand(Queries.SelectLumberFuturesByVersion, conn)
                    cmd.Parameters.AddWithValue("@VersionID", versionId)
                    Using rdr = cmd.ExecuteReader()
                        While rdr.Read()
                            Dim f As New LumberFutures With {
                        .LumberFutureID = rdr.GetInt32(0),           ' Primary key
                        .ContractMonth = rdr.GetString(1).Trim(),
                        .PriorSettle = If(rdr.IsDBNull(2), Nothing, CDec(rdr.GetDecimal(2))),
                        .PullDate = rdr.GetDateTime(3),
                        .Active = If(rdr.IsDBNull(4), Nothing, rdr.GetBoolean(4))
                    }
                            result.Add(f)
                        End While
                    End Using
                End Using
            End Using

            If result.Count = 0 Then
                Debug.WriteLine($"No futures found for VersionID {versionId}")
            End If

            Return result
        End Function

        Public Shared Sub SetActiveLumberFuture(versionId As Integer, lumberFutureId As Integer)
            Using conn = SqlConnectionManager.Instance.GetConnection()
                Using tx = conn.BeginTransaction()
                    Try
                        SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                    Queries.SetActiveLumberFuture,
                    {
                        New SqlParameter("@VersionID", versionId),
                        New SqlParameter("@LumberFutureID", lumberFutureId)
                    },
                    conn, tx)

                        tx.Commit()
                    Catch
                        tx.Rollback()
                        Throw
                    End Try
                End Using
            End Using
        End Sub


        Public Shared Function MonthNameToNumber(name As String) As Integer
            Select Case name
                Case "JAN" : Return 1
                Case "FEB" : Return 2
                Case "MAR" : Return 3
                Case "APR" : Return 4
                Case "MAY" : Return 5
                Case "JUN" : Return 6
                Case "JLY" : Return 7
                Case "AUG" : Return 8
                Case "SEP" : Return 9
                Case "OCT" : Return 10
                Case "NOV" : Return 11
                Case "DEC" : Return 12
                Case Else : Return 0
            End Select
        End Function

    End Class
End Namespace