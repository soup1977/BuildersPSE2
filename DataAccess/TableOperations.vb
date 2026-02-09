Option Strict On
Imports System.Data.SqlClient
Imports BuildersPSE2.BuildersPSE.Models
Imports BuildersPSE2.Utilities

Namespace DataAccess
    ''' <summary>
    ''' SINGLE SOURCE OF TRUTH for all database table operations.
    ''' All DataAccess files call these methods - never build parameters elsewhere.
    ''' Schema changes only need updates here.
    ''' </summary>
    Public Module TableOperations

        '===========================================================================
        ' SAFE VALUE HELPERS
        '===========================================================================

        ''' <summary>Converts nullable value to DBNull.Value if Nothing.</summary>
        Public Function ToDb(Of T)(value As T) As Object
            If value Is Nothing Then Return DBNull.Value
            If TypeOf value Is String AndAlso String.IsNullOrEmpty(CStr(CObj(value))) Then Return DBNull.Value
            Return value
        End Function

        ''' <summary>Gets nullable Decimal from DataRow by column name.</summary>
        Public Function GetDecimal(row As DataRow, columnName As String) As Decimal?
            If Not row.Table.Columns.Contains(columnName) Then Return Nothing
            If row(columnName) Is DBNull.Value Then Return Nothing
            Return CDec(row(columnName))
        End Function

        ''' <summary>Gets nullable Decimal from DataRow by column index.</summary>
        Public Function GetDecimal(row As DataRow, columnIndex As Integer) As Decimal?
            If columnIndex < 0 OrElse columnIndex >= row.Table.Columns.Count Then Return Nothing
            If row(columnIndex) Is DBNull.Value Then Return Nothing
            Return CDec(row(columnIndex))
        End Function

        ''' <summary>Gets String from DataRow by column name.</summary>
        Public Function GetString(row As DataRow, columnName As String) As String
            If Not row.Table.Columns.Contains(columnName) Then Return String.Empty
            If row(columnName) Is DBNull.Value Then Return String.Empty
            Return CStr(row(columnName))
        End Function

        ''' <summary>Gets String from DataRow by column index.</summary>
        Public Function GetString(row As DataRow, columnIndex As Integer) As String
            If columnIndex < 0 OrElse columnIndex >= row.Table.Columns.Count Then Return String.Empty
            If row(columnIndex) Is DBNull.Value Then Return String.Empty
            Return CStr(row(columnIndex))
        End Function

        ''' <summary>Gets nullable Integer from DataRow by column index.</summary>
        Public Function GetInteger(row As DataRow, columnIndex As Integer) As Integer?
            If columnIndex < 0 OrElse columnIndex >= row.Table.Columns.Count Then Return Nothing
            If row(columnIndex) Is DBNull.Value Then Return Nothing
            Return CInt(row(columnIndex))
        End Function

        ''' <summary>Parses nullable Decimal from string.</summary>
        Public Function ParseDecimal(value As String) As Decimal?
            Dim result As Decimal
            If String.IsNullOrEmpty(value) Then Return Nothing
            If Decimal.TryParse(value, result) Then Return result
            Return Nothing
        End Function

        ''' <summary>Safely gets CSV field by index.</summary>
        Public Function GetField(fields As String(), index As Integer) As String
            If fields Is Nothing OrElse index < 0 OrElse index >= fields.Length Then Return String.Empty
            Return If(fields(index), String.Empty).Trim()
        End Function

        '===========================================================================
        ' PROJECT TABLE
        '===========================================================================

        Public Function InsertProject(model As ProjectModel, conn As SqlConnection, tran As SqlTransaction) As Integer
            Dim params As New Dictionary(Of String, Object) From {
                {"@JBID", ToDb(model.JBID)},
                {"@ProjectTypeID", If(model.ProjectType IsNot Nothing, model.ProjectType.ProjectTypeID, 1)},
                {"@ProjectName", ToDb(model.ProjectName)},
                {"@EstimatorID", ToDb(If(model.Estimator IsNot Nothing, model.Estimator.EstimatorID, Nothing))},
                {"@Address", ToDb(model.Address)},
                {"@City", ToDb(model.City)},
                {"@State", ToDb(model.State)},
                {"@Zip", ToDb(model.Zip)},
                {"@BidDate", ToDb(model.BidDate)},
                {"@ArchPlansDated", ToDb(model.ArchPlansDated)},
                {"@EngPlansDated", ToDb(model.EngPlansDated)},
                {"@MilesToJobSite", model.MilesToJobSite},
                {"@TotalNetSqft", ToDb(model.TotalNetSqft)},
                {"@TotalGrossSqft", ToDb(model.TotalGrossSqft)},
                {"@ArchitectID", ToDb(model.ArchitectID)},
                {"@EngineerID", ToDb(model.EngineerID)},
                {"@ProjectNotes", ToDb(model.ProjectNotes)},
                {"@LastModifiedDate", Now}
            }
            Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                Queries.InsertProject, HelperDataAccess.BuildParameters(params), conn, tran)
        End Function

        Public Sub UpdateProject(model As ProjectModel, conn As SqlConnection, tran As SqlTransaction)
            Dim params As New Dictionary(Of String, Object) From {
                {"@ProjectID", model.ProjectID},
                {"@JBID", ToDb(model.JBID)},
                {"@ProjectTypeID", If(model.ProjectType IsNot Nothing, model.ProjectType.ProjectTypeID, 1)},
                {"@ProjectName", ToDb(model.ProjectName)},
                {"@EstimatorID", ToDb(If(model.Estimator IsNot Nothing, model.Estimator.EstimatorID, Nothing))},
                {"@Address", ToDb(model.Address)},
                {"@City", ToDb(model.City)},
                {"@State", ToDb(model.State)},
                {"@Zip", ToDb(model.Zip)},
                {"@BidDate", ToDb(model.BidDate)},
                {"@ArchPlansDated", ToDb(model.ArchPlansDated)},
                {"@EngPlansDated", ToDb(model.EngPlansDated)},
                {"@MilesToJobSite", model.MilesToJobSite},
                {"@TotalNetSqft", ToDb(model.TotalNetSqft)},
                {"@TotalGrossSqft", ToDb(model.TotalGrossSqft)},
                {"@ArchitectID", ToDb(model.ArchitectID)},
                {"@EngineerID", ToDb(model.EngineerID)},
                {"@ProjectNotes", ToDb(model.ProjectNotes)},
                {"@LastModifiedDate", Now}
            }
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                Queries.UpdateProject, HelperDataAccess.BuildParameters(params), conn, tran)
        End Sub

        Public Function UpsertProject(model As ProjectModel, conn As SqlConnection, tran As SqlTransaction) As Integer
            If model.ProjectID = 0 Then
                model.ProjectID = InsertProject(model, conn, tran)
            Else
                UpdateProject(model, conn, tran)
            End If
            Return model.ProjectID
        End Function

        '===========================================================================
        ' PROJECT VERSION TABLE
        '===========================================================================

        Public Function InsertProjectVersion(model As ProjectVersionModel, conn As SqlConnection, tran As SqlTransaction) As Integer
            Dim params As New Dictionary(Of String, Object) From {
                {"@ProjectID", model.ProjectID},
                {"@VersionName", ToDb(model.VersionName)},
                {"@VersionDate", If(model.VersionDate = Date.MinValue, Now, model.VersionDate)},
                {"@Description", ToDb(model.Description)},
                {"@LastModifiedDate", Now},
                {"@CustomerID", ToDb(model.CustomerID)},
                {"@SalesID", ToDb(model.SalesID)},
                {"@MondayID", ToDb(model.MondayID)},
                {"@ProjVersionStatusID", ToDb(model.ProjVersionStatusID)},
                {"@FuturesAdderAmt", ToDb(model.FuturesAdderAmt)},
                {"@FuturesAdderProjTotal", ToDb(model.FuturesAdderProjTotal)}
            }
            Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                Queries.InsertProjectVersion, HelperDataAccess.BuildParameters(params), conn, tran)
        End Function

        Public Sub UpdateProjectVersion(model As ProjectVersionModel, conn As SqlConnection, tran As SqlTransaction)
            Dim params As New Dictionary(Of String, Object) From {
                {"@VersionID", model.VersionID},
                {"@VersionName", ToDb(model.VersionName)},
                {"@LastModifiedDate", Now},
                {"@CustomerID", ToDb(model.CustomerID)},
                {"@SalesID", ToDb(model.SalesID)},
                {"@MondayID", ToDb(model.MondayID)},
                {"@ProjVersionStatusID", ToDb(model.ProjVersionStatusID)},
                {"@FuturesAdderAmt", ToDb(model.FuturesAdderAmt)},
                {"@FuturesAdderProjTotal", ToDb(model.FuturesAdderProjTotal)}
            }
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                Queries.UpdateProjectVersion, HelperDataAccess.BuildParameters(params), conn, tran)
        End Sub

        '===========================================================================
        ' BUILDING TABLE
        '===========================================================================

        Public Function InsertBuilding(model As BuildingModel, versionID As Integer, conn As SqlConnection, tran As SqlTransaction) As Integer
            Dim params As New Dictionary(Of String, Object) From {
                {"@BuildingName", ToDb(model.BuildingName)},
                {"@BuildingType", ToDb(model.BuildingType)},
                {"@ResUnits", ToDb(model.ResUnits)},
                {"@BldgQty", model.BldgQty},
                {"@LastModifiedDate", Now},
                {"@VersionID", versionID}
            }
            Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                Queries.InsertBuilding, HelperDataAccess.BuildParameters(params), conn, tran)
        End Function

        Public Sub UpdateBuilding(model As BuildingModel, versionID As Integer, conn As SqlConnection, tran As SqlTransaction)
            Dim params As New Dictionary(Of String, Object) From {
                {"@BuildingID", model.BuildingID},
                {"@BuildingName", ToDb(model.BuildingName)},
                {"@BuildingType", ToDb(model.BuildingType)},
                {"@ResUnits", ToDb(model.ResUnits)},
                {"@BldgQty", model.BldgQty},
                {"@LastModifiedDate", Now},
                {"@VersionID", versionID}
            }
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                Queries.UpdateBuilding, HelperDataAccess.BuildParameters(params), conn, tran)
        End Sub

        Public Function UpsertBuilding(model As BuildingModel, versionID As Integer, conn As SqlConnection, tran As SqlTransaction) As Integer
            If model.BuildingID = 0 Then
                model.BuildingID = InsertBuilding(model, versionID, conn, tran)
            Else
                UpdateBuilding(model, versionID, conn, tran)
            End If
            Return model.BuildingID
        End Function

        '===========================================================================
        ' LEVEL TABLE
        '===========================================================================

        Public Function InsertLevel(model As LevelModel, buildingID As Integer, versionID As Integer, conn As SqlConnection, tran As SqlTransaction) As Integer
            Dim params As New Dictionary(Of String, Object) From {
                {"@VersionID", versionID},
                {"@BuildingID", buildingID},
                {"@ProductTypeID", model.ProductTypeID},
                {"@LevelNumber", model.LevelNumber},
                {"@LevelName", ToDb(model.LevelName)},
                {"@LastModifiedDate", Now}
            }
            Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                Queries.InsertLevel, HelperDataAccess.BuildParameters(params), conn, tran)
        End Function

        Public Sub UpdateLevel(model As LevelModel, buildingID As Integer, versionID As Integer, conn As SqlConnection, tran As SqlTransaction)
            Dim params As New Dictionary(Of String, Object) From {
                {"@LevelID", model.LevelID},
                {"@VersionID", versionID},
                {"@BuildingID", buildingID},
                {"@ProductTypeID", model.ProductTypeID},
                {"@LevelNumber", model.LevelNumber},
                {"@LevelName", ToDb(model.LevelName)},
                {"@LastModifiedDate", Now}
            }
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                Queries.UpdateLevel, HelperDataAccess.BuildParameters(params), conn, tran)
        End Sub

        Public Function UpsertLevel(model As LevelModel, buildingID As Integer, versionID As Integer, conn As SqlConnection, tran As SqlTransaction) As Integer
            If model.LevelID = 0 Then
                model.LevelID = InsertLevel(model, buildingID, versionID, conn, tran)
            Else
                UpdateLevel(model, buildingID, versionID, conn, tran)
            End If
            Return model.LevelID
        End Function

        '===========================================================================
        ' RAW UNIT TABLE
        '===========================================================================

        Public Function InsertRawUnit(model As RawUnitModel, versionID As Integer, conn As SqlConnection, tran As SqlTransaction) As Integer
            Dim params As New Dictionary(Of String, Object) From {
                {"@VersionID", versionID},
                {"@ProductTypeID", model.ProductTypeID},
                {"@RawUnitName", ToDb(model.RawUnitName)},
                {"@BF", ToDb(model.BF)},
                {"@LF", ToDb(model.LF)},
                {"@EWPLF", ToDb(model.EWPLF)},
                {"@SqFt", ToDb(model.SqFt)},
                {"@FCArea", ToDb(model.FCArea)},
                {"@LumberCost", ToDb(model.LumberCost)},
                {"@PlateCost", ToDb(model.PlateCost)},
                {"@ManufLaborCost", ToDb(model.ManufLaborCost)},
                {"@DesignLabor", ToDb(model.DesignLabor)},
                {"@MGMTLabor", ToDb(model.MGMTLabor)},
                {"@JobSuppliesCost", ToDb(model.JobSuppliesCost)},
                {"@ManHours", ToDb(model.ManHours)},
                {"@ItemCost", ToDb(model.ItemCost)},
                {"@OverallCost", ToDb(model.OverallCost)},
                {"@DeliveryCost", ToDb(model.DeliveryCost)},
                {"@TotalSellPrice", ToDb(model.TotalSellPrice)},
                {"@AvgSPFNo2", ToDb(model.AvgSPFNo2)},
                {"@SPFNo2BDFT", ToDb(model.SPFNo2BDFT)},
                {"@Avg241800", ToDb(model.Avg241800)},
                {"@MSR241800BDFT", ToDb(model.MSR241800BDFT)},
                {"@Avg242400", ToDb(model.Avg242400)},
                {"@MSR242400BDFT", ToDb(model.MSR242400BDFT)},
                {"@Avg261800", ToDb(model.Avg261800)},
                {"@MSR261800BDFT", ToDb(model.MSR261800BDFT)},
                {"@Avg262400", ToDb(model.Avg262400)},
                {"@MSR262400BDFT", ToDb(model.MSR262400BDFT)}
            }
            Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                Queries.InsertRawUnit, HelperDataAccess.BuildParameters(params), conn, tran)
        End Function

        Public Function InsertRawUnitWithHistory(model As RawUnitModel, versionID As Integer, conn As SqlConnection, tran As SqlTransaction, Optional costEffectiveID As Object = Nothing) As Integer
            Dim newRawID As Integer = InsertRawUnit(model, versionID, conn, tran)

            ' Insert lumber history if relevant fields present
            If (costEffectiveID IsNot Nothing AndAlso costEffectiveID IsNot DBNull.Value) OrElse
               model.AvgSPFNo2.HasValue OrElse model.Avg241800.HasValue OrElse
               model.Avg242400.HasValue OrElse model.Avg261800.HasValue OrElse
               model.Avg262400.HasValue Then

                InsertRawUnitLumberHistory(newRawID, model, versionID, costEffectiveID, conn, tran)
            End If

            Return newRawID
        End Function

        Public Sub InsertRawUnitLumberHistory(rawUnitID As Integer, model As RawUnitModel, versionID As Integer, costEffectiveID As Object, conn As SqlConnection, tran As SqlTransaction)
            Dim params As New Dictionary(Of String, Object) From {
                {"@RawUnitID", rawUnitID},
                {"@VersionID", versionID},
                {"@CostEffectiveDateID", If(costEffectiveID, DBNull.Value)},
                {"@LumberCost", ToDb(model.LumberCost)},
                {"@AvgSPFNo2", ToDb(model.AvgSPFNo2)},
                {"@Avg241800", ToDb(model.Avg241800)},
                {"@Avg242400", ToDb(model.Avg242400)},
                {"@Avg261800", ToDb(model.Avg261800)},
                {"@Avg262400", ToDb(model.Avg262400)}
            }
            SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                Queries.InsertRawUnitLumberHistory, HelperDataAccess.BuildParameters(params), conn, tran)
        End Sub

        '===========================================================================
        ' ACTUAL UNIT TABLE
        '===========================================================================

        Public Function InsertActualUnit(model As ActualUnitModel, rawUnitID As Integer, versionID As Integer, conn As SqlConnection, tran As SqlTransaction, Optional colorCode As String = Nothing) As Integer
            Dim params As New Dictionary(Of String, Object) From {
                {"@VersionID", versionID},
                {"@RawUnitID", rawUnitID},
                {"@ProductTypeID", model.ProductTypeID},
                {"@UnitName", ToDb(model.UnitName)},
                {"@PlanSQFT", model.PlanSQFT},
                {"@UnitType", ToDb(model.UnitType)},
                {"@OptionalAdder", model.OptionalAdder},
                {"@ColorCode", ToDb(colorCode)},
                {"@MarginPercent", model.MarginPercent},
                {"@SortOrder", model.SortOrder}
            }
            Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                Queries.InsertActualUnit, HelperDataAccess.BuildParameters(params), conn, tran)
        End Function

        Public Sub UpdateActualUnit(model As ActualUnitModel, conn As SqlConnection, tran As SqlTransaction)
            Dim params As New Dictionary(Of String, Object) From {
        {"@ActualUnitID", model.ActualUnitID},
        {"@RawUnitID", model.RawUnitID},
        {"@UnitName", ToDb(model.UnitName)},
        {"@PlanSQFT", model.PlanSQFT},
        {"@UnitType", ToDb(model.UnitType)},
        {"@OptionalAdder", model.OptionalAdder},
        {"@ColorCode", ToDb(model.ColorCode)},
        {"@MarginPercent", model.MarginPercent},
        {"@SortOrder", model.SortOrder}
    }
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
        Queries.UpdateActualUnit, HelperDataAccess.BuildParameters(params), conn, tran)
        End Sub

        '===========================================================================
        ' ACTUAL TO LEVEL MAPPING TABLE
        '===========================================================================

        Public Function InsertActualToLevelMapping(actualUnitID As Integer, levelID As Integer, quantity As Integer, versionID As Integer, conn As SqlConnection, tran As SqlTransaction) As Integer
            Dim params As New Dictionary(Of String, Object) From {
                {"@VersionID", versionID},
                {"@ActualUnitID", actualUnitID},
                {"@LevelID", levelID},
                {"@Quantity", quantity}
            }
            Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                Queries.InsertActualToLevelMapping, HelperDataAccess.BuildParameters(params), conn, tran)
        End Function

        Public Sub UpdateActualToLevelMapping(mappingID As Integer, quantity As Integer, conn As SqlConnection, tran As SqlTransaction)
            Dim params As New Dictionary(Of String, Object) From {
                {"@MappingID", mappingID},
                {"@Quantity", quantity}
            }
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                Queries.UpdateActualToLevelMapping, HelperDataAccess.BuildParameters(params), conn, tran)
        End Sub

        '===========================================================================
        ' CALCULATED COMPONENT TABLE
        '===========================================================================

        Public Sub InsertCalculatedComponent(model As CalculatedComponentModel, actualUnitID As Integer, versionID As Integer, conn As SqlConnection, tran As SqlTransaction)
            Dim params As New Dictionary(Of String, Object) From {
                {"@VersionID", versionID},
                {"@ActualUnitID", actualUnitID},
                {"@ComponentType", model.ComponentType},
                {"@Value", model.Value}
            }
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                Queries.InsertCalculatedComponent, HelperDataAccess.BuildParameters(params), conn, tran)
        End Sub

        Public Sub InsertCalculatedComponentsFromRawUnit(rawUnitData As Dictionary(Of String, Decimal), actualUnitID As Integer, versionID As Integer, conn As SqlConnection, tran As SqlTransaction)
            Dim sqFt As Decimal = rawUnitData("SqFt")
            If sqFt <= 0 Then sqFt = 1D

            Dim components As New List(Of (String, Decimal)) From {
                ("LF/SQFT", rawUnitData("LF") / sqFt),
                ("BDFT/SQFT", rawUnitData("BF") / sqFt),
                ("Lumber/SQFT", rawUnitData("LumberCost") / sqFt),
                ("Plate/SQFT", rawUnitData("PlateCost") / sqFt),
                ("ManufLabor/SQFT", rawUnitData("ManufLaborCost") / sqFt),
                ("DesignLabor/SQFT", rawUnitData("DesignLabor") / sqFt),
                ("MGMTLabor/SQFT", rawUnitData("MGMTLabor") / sqFt),
                ("JobSupplies/SQFT", rawUnitData("JobSuppliesCost") / sqFt),
                ("ManHours/SQFT", rawUnitData("ManHours") / sqFt),
                ("ItemCost/SQFT", rawUnitData("ItemCost") / sqFt),
                ("OverallCost/SQFT", rawUnitData("OverallCost") / sqFt),
                ("SellPrice/SQFT", rawUnitData("TotalSellPrice") / sqFt),
                ("Margin/SQFT", (rawUnitData("TotalSellPrice") - rawUnitData("OverallCost")) / sqFt)
            }

            For Each comp In components
                Dim model As New CalculatedComponentModel With {.ComponentType = comp.Item1, .Value = comp.Item2}
                InsertCalculatedComponent(model, actualUnitID, versionID, conn, tran)
            Next
        End Sub

        '===========================================================================
        ' RAW UNIT LUMBER HISTORY TABLE
        '===========================================================================

        ''' <summary>Inserts a lumber history record from a RawUnitLumberHistoryModel.</summary>
        Public Function InsertLumberHistoryFromModel(model As RawUnitLumberHistoryModel, newRawUnitID As Integer, newVersionID As Integer,
                                                     conn As SqlConnection, tran As SqlTransaction) As Integer
            Dim params As New Dictionary(Of String, Object) From {
                {"@RawUnitID", newRawUnitID},
                {"@VersionID", newVersionID},
                {"@CostEffectiveDateID", ToDb(model.CostEffectiveDateID)},
                {"@LumberCost", model.LumberCost},
                {"@AvgSPFNo2", ToDb(model.AvgSPFNo2)},
                {"@Avg241800", ToDb(model.Avg241800)},
                {"@Avg242400", ToDb(model.Avg242400)},
                {"@Avg261800", ToDb(model.Avg261800)},
                {"@Avg262400", ToDb(model.Avg262400)}
            }
            Return SqlConnectionManager.Instance.ExecuteScalarTransactional(Of Integer)(
                Queries.InsertRawUnitLumberHistory, HelperDataAccess.BuildParameters(params), conn, tran)
        End Function

        ''' <summary>Sets a lumber history record as active (deactivates others for same RawUnit/Version).</summary>
        Public Sub SetLumberHistoryActive(rawUnitID As Integer, versionID As Integer, historyID As Integer,
                                          conn As SqlConnection, tran As SqlTransaction)
            Dim params As New Dictionary(Of String, Object) From {
                {"@RawUnitID", rawUnitID},
                {"@VersionID", versionID},
                {"@HistoryID", historyID}
            }
            SqlConnectionManager.Instance.ExecuteNonQueryTransactional(
                Queries.SetLumberHistoryActive, HelperDataAccess.BuildParameters(params), conn, tran)
        End Sub

        '===========================================================================
        ' MODEL MAPPERS (DataRow/CSV → Model)
        '===========================================================================

        ''' <summary>Maps DataRow from FloorImport/RoofImport to RawUnitModel.</summary>
        Public Function MapRawUnitFromDataRow(row As DataRow, productTypeID As Integer) As RawUnitModel
            Return New RawUnitModel With {
                .RawUnitName = GetString(row, "Elevation"),
                .ProductTypeID = productTypeID,
                .BF = GetDecimal(row, "BF"),
                .LF = GetDecimal(row, "LF"),
                .EWPLF = GetDecimal(row, "EWPLF"),
                .SqFt = GetDecimal(row, "SqFt"),
                .FCArea = GetDecimal(row, "FCArea"),
                .LumberCost = GetDecimal(row, "LumberCost"),
                .PlateCost = GetDecimal(row, "PlateCost"),
                .ManufLaborCost = GetDecimal(row, "ManufLaborCost"),
                .DesignLabor = GetDecimal(row, "DesignLabor"),
                .MGMTLabor = GetDecimal(row, "MGMTLabor"),
                .JobSuppliesCost = GetDecimal(row, "JobSuppliesCost"),
                .ManHours = GetDecimal(row, "ManHours"),
                .ItemCost = GetDecimal(row, "ItemCost"),
                .OverallCost = GetDecimal(row, "OverallCost"),
                .DeliveryCost = GetDecimal(row, "DeliveryCost"),
                .TotalSellPrice = GetDecimal(row, "TotalSellPrice"),
                .AvgSPFNo2 = GetDecimal(row, "AvgSPFNo2"),
                .SPFNo2BDFT = GetDecimal(row, "spfNo2BDFT"),
                .Avg241800 = GetDecimal(row, "Avg241800"),
                .MSR241800BDFT = GetDecimal(row, "MSR241800BDFT"),
                .Avg242400 = GetDecimal(row, "Avg242400"),
                .MSR242400BDFT = GetDecimal(row, "MSR242400BDFT"),
                .Avg261800 = GetDecimal(row, "Avg261800"),
                .MSR261800BDFT = GetDecimal(row, "MSR261800BDFT"),
                .Avg262400 = GetDecimal(row, "Avg262400"),
                .MSR262400BDFT = GetDecimal(row, "MSR262400BDFT")
            }
        End Function

        ''' <summary>Maps CSV fields to RawUnitModel using MGMT export column order.</summary>
        Public Function MapRawUnitFromCsvFields(fields As String(), rawUnitName As String, productTypeID As Integer) As RawUnitModel
            Return New RawUnitModel With {
                .RawUnitName = rawUnitName,
                .ProductTypeID = productTypeID,
                .BF = ParseDecimal(GetField(fields, 8)),
                .LF = ParseDecimal(GetField(fields, 9)),
                .EWPLF = ParseDecimal(GetField(fields, 10)),
                .SqFt = ParseDecimal(GetField(fields, 11)),
                .FCArea = ParseDecimal(GetField(fields, 12)),
                .LumberCost = ParseDecimal(GetField(fields, 13)),
                .PlateCost = ParseDecimal(GetField(fields, 14)),
                .ManufLaborCost = ParseDecimal(GetField(fields, 15)),
                .DesignLabor = ParseDecimal(GetField(fields, 16)),
                .MGMTLabor = ParseDecimal(GetField(fields, 17)),
                .JobSuppliesCost = ParseDecimal(GetField(fields, 18)),
                .ManHours = ParseDecimal(GetField(fields, 19)),
                .ItemCost = ParseDecimal(GetField(fields, 20)),
                .OverallCost = ParseDecimal(GetField(fields, 21)),
                .DeliveryCost = ParseDecimal(GetField(fields, 22)),
                .TotalSellPrice = ParseDecimal(GetField(fields, 23)),
                .AvgSPFNo2 = ParseDecimal(GetField(fields, 24)),
                .SPFNo2BDFT = ParseDecimal(GetField(fields, 25)),
                .Avg241800 = ParseDecimal(GetField(fields, 26)),
                .MSR241800BDFT = ParseDecimal(GetField(fields, 27)),
                .Avg242400 = ParseDecimal(GetField(fields, 28)),
                .MSR242400BDFT = ParseDecimal(GetField(fields, 29)),
                .Avg261800 = ParseDecimal(GetField(fields, 30)),
                .MSR261800BDFT = ParseDecimal(GetField(fields, 31)),
                .Avg262400 = ParseDecimal(GetField(fields, 32)),
                .MSR262400BDFT = ParseDecimal(GetField(fields, 33))
            }
        End Function

        ''' <summary>Applies Wall-specific adjustments (VTP, misc materials).</summary>
        Public Sub ApplyWallAdjustments(model As RawUnitModel, fields As String())
            Dim vtpCost As Decimal = ParseDecimal(GetField(fields, 37)).GetValueOrDefault(0D)
            Dim vtpBdft As Decimal = ParseDecimal(GetField(fields, 38)).GetValueOrDefault(0D)
            Dim miscMaterialCost As Decimal = ParseDecimal(GetField(fields, 39)).GetValueOrDefault(0D)

            model.LumberCost = model.LumberCost.GetValueOrDefault(0D) + vtpCost
            model.BF = model.BF.GetValueOrDefault(0D) + vtpBdft
            model.SPFNo2BDFT = model.SPFNo2BDFT.GetValueOrDefault(0D) + model.BF.GetValueOrDefault(0D) + vtpBdft
            model.ItemCost = miscMaterialCost
        End Sub

    End Module
End Namespace