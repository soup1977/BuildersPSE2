' ===============================================================
' ActualsImporter.vb — FINAL, Option Strict On SAFE
' ===============================================================
Option Strict On

Imports System.Data.SqlClient

Imports BuildersPSE2.DataAccess
Imports Microsoft.VisualBasic.FileIO

Namespace Utilities

    Public Class ActualsImporter

        ' MIKEK CSV → DESIGN ACTUALS (StageType = 1)
        Public Shared Sub ImportMiTekDesignActuals(
            csvPath As String,
            versionID As Integer,
            importedBy As String,
            miTekJobNumber As String)

            Using conn = SqlConnectionManager.Instance.GetConnection()
                Using tran = conn.BeginTransaction()
                    Try
                        Using parser As New TextFieldParser(csvPath)
                            parser.TextFieldType = FieldType.Delimited
                            parser.SetDelimiters(",")
                            parser.ReadLine() ' Skip header

                            While Not parser.EndOfData
                                Dim fields() As String = parser.ReadFields()
                                If fields.Length >= 13 Then
                                    Dim levelID As Integer = CInt(fields(0))

                                    Dim params() As SqlParameter = {
                                        New SqlParameter("@LevelID", levelID),
                                        New SqlParameter("@VersionID", versionID),
                                        New SqlParameter("@StageType", 1),
                                        New SqlParameter("@ActualBDFT", If(String.IsNullOrWhiteSpace(fields(1)), CType(DBNull.Value, Object), CType(CDec(fields(1)), Object))),
                                        New SqlParameter("@ActualLumberCost", If(String.IsNullOrWhiteSpace(fields(2)), CType(DBNull.Value, Object), CType(CDec(fields(2)), Object))),
                                        New SqlParameter("@ActualPlateCost", If(String.IsNullOrWhiteSpace(fields(3)), CType(DBNull.Value, Object), CType(CDec(fields(3)), Object))),
                                        New SqlParameter("@ActualManufLaborCost", If(String.IsNullOrWhiteSpace(fields(4)), CType(DBNull.Value, Object), CType(CDec(fields(4)), Object))),
                                        New SqlParameter("@ActualManufMH", If(String.IsNullOrWhiteSpace(fields(5)), CType(DBNull.Value, Object), CType(CDec(fields(5)), Object))),
                                        New SqlParameter("@ActualItemCost", If(String.IsNullOrWhiteSpace(fields(6)), CType(DBNull.Value, Object), CType(CDec(fields(6)), Object))),
                                        New SqlParameter("@ActualDeliveryCost", If(String.IsNullOrWhiteSpace(fields(7)), CType(DBNull.Value, Object), CType(CDec(fields(7)), Object))),
                                        New SqlParameter("@ActualMiscLaborCost", If(String.IsNullOrWhiteSpace(fields(8)), CType(DBNull.Value, Object), CType(CDec(fields(8)), Object))),
                                        New SqlParameter("@ActualTotalCost", If(String.IsNullOrWhiteSpace(fields(9)), CType(DBNull.Value, Object), CType(CDec(fields(9)), Object))),
                                        New SqlParameter("@ActualSoldAmount", If(String.IsNullOrWhiteSpace(fields(10)), CType(DBNull.Value, Object), CType(CDec(fields(10)), Object))),
                                        New SqlParameter("@ActualMarginPercent", If(String.IsNullOrWhiteSpace(fields(11)), CType(DBNull.Value, Object), CType(CDec(fields(11)), Object))),
                                        New SqlParameter("@AvgSPFNo2Actual", If(String.IsNullOrWhiteSpace(fields(12)), CType(DBNull.Value, Object), CType(CDec(fields(12)), Object))),
                                        New SqlParameter("@MiTekJobNumber", miTekJobNumber),
                                        New SqlParameter("@BisTrackSalesOrder", DBNull.Value),
                                        New SqlParameter("@ImportedBy", importedBy),
                                        New SqlParameter("@Notes", "MiTek Design Import")
                                    }

                                    SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                        Queries.InsertLevelActual, params, conn, tran)
                                End If
                            End While
                        End Using

                        tran.Commit()
                    Catch ex As Exception
                        tran.Rollback()
                        Throw New ApplicationException("MiTek import failed: " & ex.Message)
                    End Try
                End Using
            End Using
        End Sub

        ' BISTRACK → INVOICE ACTUALS (StageType = 2)
        Public Shared Sub ImportBisTrackInvoiceActuals(
            salesOrder As String,
            versionID As Integer,
            importedBy As String)

            Dim sql As String = "
                SELECT 
                    l.LevelID,
                    SUM(id.Qty * id.UnitPrice) AS ActualSoldAmount,
                    SUM(id.ExtendedCost) AS ActualTotalCost,
                    SUM(id.BDFT) AS ActualBDFT
                FROM BisTrack_InvoiceDetail id
                JOIN YourLevelMappingTable m ON id.JobNumber = m.MiTekJobNumber
                JOIN Levels l ON m.LevelID = l.LevelID
                WHERE id.SalesOrder = @SalesOrder
                  AND l.VersionID = @VersionID
                GROUP BY l.LevelID"

            Using conn = SqlConnectionManager.Instance.GetConnection()
                Using tran = conn.BeginTransaction()
                    Try
                        Using cmd As New SqlCommand(sql, conn, tran)
                            cmd.Parameters.AddWithValue("@SalesOrder", salesOrder)
                            cmd.Parameters.AddWithValue("@VersionID", versionID)
                            Using reader = cmd.ExecuteReader()
                                While reader.Read()
                                    Dim levelID As Integer = CInt(reader("LevelID"))

                                    Dim params() As SqlParameter = {
                                        New SqlParameter("@LevelID", levelID),
                                        New SqlParameter("@VersionID", versionID),
                                        New SqlParameter("@StageType", 2),
                                        New SqlParameter("@ActualBDFT", If(reader("ActualBDFT") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(reader("ActualBDFT")), Object))),
                                        New SqlParameter("@ActualTotalCost", If(reader("ActualTotalCost") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(reader("ActualTotalCost")), Object))),
                                        New SqlParameter("@ActualSoldAmount", If(reader("ActualSoldAmount") Is DBNull.Value, CType(DBNull.Value, Object), CType(CDec(reader("ActualSoldAmount")), Object))),
                                        New SqlParameter("@BisTrackSalesOrder", salesOrder),
                                        New SqlParameter("@MiTekJobNumber", DBNull.Value),
                                        New SqlParameter("@ImportedBy", importedBy),
                                        New SqlParameter("@Notes", "BisTrack Invoice Import")
                                    }

                                    SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                                        Queries.InsertLevelActual, params, conn, tran)
                                End While
                            End Using
                        End Using

                        tran.Commit()
                    Catch ex As Exception
                        tran.Rollback()
                        Throw New ApplicationException("BisTrack import failed: " & ex.Message)
                    End Try
                End Using
            End Using
        End Sub

    End Class
End Namespace