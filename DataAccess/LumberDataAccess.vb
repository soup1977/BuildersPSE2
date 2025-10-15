Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Utilities
Imports BuildersPSE2.BuildersPSE.Models

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
                                                                                       Debug.WriteLine($"Processing RawUnitID: {rawUnit.RawUnitID}, Name: {rawUnit.RawUnitName}, BF: {If(rawUnit.BF.HasValue, rawUnit.BF.Value, "NULL")}, LumberCost: {If(rawUnit.LumberCost.HasValue, rawUnit.LumberCost.Value, "NULL")}")
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
                                                                                               Debug.WriteLine($"Invalid oldCost or bdft for {typeKey}: oldCost={If(oldCost.HasValue, oldCost.Value, "NULL")}, bdft={If(bdft.HasValue, bdft.Value, "NULL")} - skipping")
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
                                                                                                           Else
                                                                                                               Debug.WriteLine($"No valid AvgSPFNo2 ({If(oldCost.HasValue, oldCost.Value, "NULL")}) for unallocated BDFT in RawUnitID: {rawUnit.RawUnitID}")
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
                                                                                       Dim baseLumberCost As Decimal = If(rawUnit.LumberCost.HasValue, rawUnit.LumberCost.Value, 0D)
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

    End Class
End Namespace