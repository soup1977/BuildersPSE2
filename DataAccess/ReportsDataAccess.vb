Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Utilities

Public Class ReportsDataAccess
    ' In DataAccess.vb, within BuildersPSE.DataAccess namespace
    Public Shared Function GetProjectSummaryData(projectID As Integer, versionID As Integer) As DataTable
        Dim dt As New DataTable()
        SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                   Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                       conn.Open()
                                                                       Dim cmd As New SqlCommand("SELECT p.ProjectName, pv.VersionName, c.CustomerName, " &
                                     "b.BuildingName, b.BldgQty, b.OverallPrice, " &
                                     "l.LevelName, l.OverallSQFT, l.OverallPrice AS OverallPrice_Level, " &
                                     "pt.ProductTypeName " &
                                     "FROM Projects p " &
                                     "INNER JOIN ProjectVersions pv ON p.ProjectID = pv.ProjectID AND pv.VersionID = @VersionID " &
                                     "LEFT JOIN Customer c ON pv.CustomerID = c.CustomerID " &
                                     "LEFT JOIN Buildings b ON pv.VersionID = b.VersionID " &
                                     "LEFT JOIN Levels l ON b.BuildingID = l.BuildingID " &
                                     "LEFT JOIN ProductType pt ON l.ProductTypeID = pt.ProductTypeID " &
                                     "WHERE p.ProjectID = @ProjectID", conn)
                                                                       cmd.Parameters.Add(New SqlParameter("@ProjectID", projectID))
                                                                       cmd.Parameters.Add(New SqlParameter("@VersionID", versionID))
                                                                       Dim da As New SqlDataAdapter(cmd)
                                                                       da.Fill(dt)
                                                                       Debug.WriteLine($"Rows returned for GetProjectSummaryData ProjectID {projectID}, VersionID {versionID}: {dt.Rows.Count}")
                                                                   End Using
                                                               End Sub, "Error fetching project summary for ProjectID " & projectID & ", VersionID " & versionID)
        Return dt
    End Function
    Public Shared Function GetProjectHeaderData(projectID As Integer, versionID As Integer) As DataTable
        Dim dt As New DataTable()
        SqlConnectionManager.Instance.ExecuteWithErrorHandling(Sub()
                                                                   Using conn As New SqlConnection(SqlConnectionManager.Instance.ConnectionString)
                                                                       conn.Open()
                                                                       Dim cmd As New SqlCommand("SELECT p.ProjectName, pv.VersionName, c.CustomerName AS CustomerName, s.SalesName, " &
                                     "p.ArchPlansDated, p.EngPlansDated, ca.CustomerName AS ArchitectName, ce.CustomerName AS EngineerName, " &
                                     "pdi.BuildingCode, pdi.Importance, pdi.ExposureCategory, pdi.WindSpeed, pdi.SnowLoadType, " &
                                     "pdi.OccupancyCategory, pdi.RoofPitches, pdi.FloorDepths, pdi.WallHeights, " &
                                     "pgn.Notes " &
                                     "FROM Projects p " &
                                     "INNER JOIN ProjectVersions pv ON p.ProjectID = pv.ProjectID AND pv.VersionID = @VersionID " &
                                     "LEFT JOIN Customer c ON pv.CustomerID = c.CustomerID AND c.CustomerType = 1 " &
                                     "LEFT JOIN Sales s ON pv.SalesID = s.SalesID " &
                                     "LEFT JOIN Customer ca ON p.ArchitectID = ca.CustomerID AND ca.CustomerType = 2 " &
                                     "LEFT JOIN Customer ce ON p.EngineerID = ce.CustomerID AND ce.CustomerType = 3 " &
                                     "LEFT JOIN ProjectDesignInfo pdi ON p.ProjectID = pdi.ProjectID " &
                                     "LEFT JOIN ProjectGeneralNotes pgn ON p.ProjectID = pdi.ProjectID " &
                                     "WHERE p.ProjectID = @ProjectID", conn)
                                                                       cmd.Parameters.Add(New SqlParameter("@ProjectID", projectID))
                                                                       cmd.Parameters.Add(New SqlParameter("@VersionID", versionID))
                                                                       Dim da As New SqlDataAdapter(cmd)
                                                                       da.Fill(dt)
                                                                       Debug.WriteLine($"Rows returned for GetProjectHeaderData ProjectID {projectID}, VersionID {versionID}: {dt.Rows.Count}")
                                                                   End Using
                                                               End Sub, "Error fetching project header data for ProjectID " & projectID & ", VersionID " & versionID)
        Return dt
    End Function

End Class
