' ModelParams.vb
Option Strict On

Imports BuildersPSE2.BuildersPSE.Models

Namespace DataAccess
    Friend Class ModelParams
        Public Shared Function ForProject(model As ProjectModel) As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"@JBID", If(String.IsNullOrEmpty(model.JBID), DBNull.Value, CType(model.JBID, Object))},
                {"@ProjectTypeID", If(model.ProjectType Is Nothing OrElse model.ProjectType.ProjectTypeID = 0, DBNull.Value, CType(model.ProjectType.ProjectTypeID, Object))},
                {"@ProjectName", If(String.IsNullOrEmpty(model.ProjectName), DBNull.Value, CType(model.ProjectName, Object))},
                {"@EstimatorID", If(model.Estimator Is Nothing OrElse model.Estimator.EstimatorID = 0, DBNull.Value, CType(model.Estimator.EstimatorID, Object))},
                {"@Address", If(String.IsNullOrEmpty(model.Address), DBNull.Value, CType(model.Address, Object))},
                {"@City", If(String.IsNullOrEmpty(model.City), DBNull.Value, CType(model.City, Object))},
                {"@State", If(String.IsNullOrEmpty(model.State), DBNull.Value, CType(model.State, Object))},
                {"@Zip", If(String.IsNullOrEmpty(model.Zip), DBNull.Value, CType(model.Zip, Object))},
                {"@BidDate", If(model.BidDate.HasValue, CType(model.BidDate.Value, Object), DBNull.Value)},
                {"@ArchPlansDated", If(model.ArchPlansDated.HasValue, CType(model.ArchPlansDated.Value, Object), DBNull.Value)},
                {"@EngPlansDated", If(model.EngPlansDated.HasValue, CType(model.EngPlansDated.Value, Object), DBNull.Value)},
                {"@MilesToJobSite", If(model.MilesToJobSite = 0, DBNull.Value, CType(model.MilesToJobSite, Object))},  ' Assuming 0 is default/unset
                {"@TotalNetSqft", If(model.TotalNetSqft.HasValue, CType(model.TotalNetSqft.Value, Object), DBNull.Value)},
                {"@TotalGrossSqft", If(model.TotalGrossSqft.HasValue, CType(model.TotalGrossSqft.Value, Object), DBNull.Value)},
                {"@ArchitectID", If(model.ArchitectID.HasValue, CType(model.ArchitectID.Value, Object), DBNull.Value)},
                {"@EngineerID", If(model.EngineerID.HasValue, CType(model.EngineerID.Value, Object), DBNull.Value)},
                {"@ProjectNotes", If(String.IsNullOrEmpty(model.ProjectNotes), DBNull.Value, CType(model.ProjectNotes, Object))}
            }
        End Function

        Public Shared Function ForProjectVersion(model As ProjectVersionModel) As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"@ProjectID", model.ProjectID},
                {"@VersionName", If(String.IsNullOrEmpty(model.VersionName), DBNull.Value, CType(model.VersionName, Object))},
                {"@VersionDate", model.VersionDate},
                {"@Description", If(String.IsNullOrEmpty(model.Description), DBNull.Value, CType(model.Description, Object))},
                {"@CustomerID", If(model.CustomerID.HasValue, CType(model.CustomerID.Value, Object), DBNull.Value)},
                {"@SalesID", If(model.SalesID.HasValue, CType(model.SalesID.Value, Object), DBNull.Value)},
                {"@MondayID", If(String.IsNullOrEmpty(model.MondayID), DBNull.Value, CType(model.MondayID, Object))}
            }
        End Function
        Public Shared Function ForRawUnit(model As RawUnitModel, newVersionID As Integer) As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"@RawUnitName", model.RawUnitName},
                {"@VersionID", newVersionID},
                {"@ProductTypeID", model.ProductTypeID},
                {"@BF", If(model.BF.HasValue, CType(model.BF.Value, Object), DBNull.Value)},
                {"@LF", If(model.LF.HasValue, CType(model.LF.Value, Object), DBNull.Value)},
                {"@EWPLF", If(model.EWPLF.HasValue, CType(model.EWPLF.Value, Object), DBNull.Value)},
                {"@SqFt", If(model.SqFt.HasValue, CType(model.SqFt.Value, Object), DBNull.Value)},
                {"@FCArea", If(model.FCArea.HasValue, CType(model.FCArea.Value, Object), DBNull.Value)},
                {"@LumberCost", If(model.LumberCost.HasValue, CType(model.LumberCost.Value, Object), DBNull.Value)},
                {"@PlateCost", If(model.PlateCost.HasValue, CType(model.PlateCost.Value, Object), DBNull.Value)},
                {"@ManufLaborCost", If(model.ManufLaborCost.HasValue, CType(model.ManufLaborCost.Value, Object), DBNull.Value)},
                {"@DesignLabor", If(model.DesignLabor.HasValue, CType(model.DesignLabor.Value, Object), DBNull.Value)},
                {"@MGMTLabor", If(model.MGMTLabor.HasValue, CType(model.MGMTLabor.Value, Object), DBNull.Value)},
                {"@JobSuppliesCost", If(model.JobSuppliesCost.HasValue, CType(model.JobSuppliesCost.Value, Object), DBNull.Value)},
                {"@ManHours", If(model.ManHours.HasValue, CType(model.ManHours.Value, Object), DBNull.Value)},
                {"@ItemCost", If(model.ItemCost.HasValue, CType(model.ItemCost.Value, Object), DBNull.Value)},
                {"@OverallCost", If(model.OverallCost.HasValue, CType(model.OverallCost.Value, Object), DBNull.Value)},
                {"@DeliveryCost", If(model.DeliveryCost.HasValue, CType(model.DeliveryCost.Value, Object), DBNull.Value)},
                {"@TotalSellPrice", If(model.TotalSellPrice.HasValue, CType(model.TotalSellPrice.Value, Object), DBNull.Value)},
                {"@AvgSPFNo2", If(model.AvgSPFNo2.HasValue, CType(model.AvgSPFNo2.Value, Object), DBNull.Value)},
                {"@SPFNo2BDFT", If(model.SPFNo2BDFT.HasValue, CType(model.SPFNo2BDFT.Value, Object), DBNull.Value)},
                {"@Avg241800", If(model.Avg241800.HasValue, CType(model.Avg241800.Value, Object), DBNull.Value)},
                {"@MSR241800BDFT", If(model.MSR241800BDFT.HasValue, CType(model.MSR241800BDFT.Value, Object), DBNull.Value)},
                {"@Avg242400", If(model.Avg242400.HasValue, CType(model.Avg242400.Value, Object), DBNull.Value)},
                {"@MSR242400BDFT", If(model.MSR242400BDFT.HasValue, CType(model.MSR242400BDFT.Value, Object), DBNull.Value)},
                {"@Avg261800", If(model.Avg261800.HasValue, CType(model.Avg261800.Value, Object), DBNull.Value)},
                {"@MSR261800BDFT", If(model.MSR261800BDFT.HasValue, CType(model.MSR261800BDFT.Value, Object), DBNull.Value)},
                {"@Avg262400", If(model.Avg262400.HasValue, CType(model.Avg262400.Value, Object), DBNull.Value)},
                {"@MSR262400BDFT", If(model.MSR262400BDFT.HasValue, CType(model.MSR262400BDFT.Value, Object), DBNull.Value)}
            }
        End Function

        Public Shared Function ForBuilding(model As BuildingModel, newVersionID As Integer) As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"@BuildingName", If(String.IsNullOrEmpty(model.BuildingName), DBNull.Value, CType(model.BuildingName, Object))},
                {"@BuildingType", If(model.BuildingType.HasValue, CType(model.BuildingType.Value, Object), DBNull.Value)},
                {"@ResUnits", If(model.ResUnits.HasValue, CType(model.ResUnits.Value, Object), DBNull.Value)},
                {"@BldgQty", model.BldgQty},
                {"@VersionID", newVersionID}
            }
        End Function

        Public Shared Function ForActualUnit(model As ActualUnitModel, newVersionID As Integer, newRawUnitID As Integer, colorCode As String) As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
        {"@VersionID", newVersionID},
        {"@RawUnitID", newRawUnitID},
        {"@ProductTypeID", model.ProductTypeID},
        {"@UnitName", model.UnitName},
        {"@PlanSQFT", model.PlanSQFT},
        {"@UnitType", model.UnitType},
        {"@OptionalAdder", model.OptionalAdder},
        {"@ColorCode", If(String.IsNullOrEmpty(colorCode), DBNull.Value, CType(colorCode, Object))}
    }
        End Function

        Public Shared Function ForComponent(model As CalculatedComponentModel, newVersionID As Integer, newActualUnitID As Integer) As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"@VersionID", newVersionID},
                {"@ActualUnitID", newActualUnitID},
                {"@ComponentType", model.ComponentType},
                {"@Value", model.Value}
            }
        End Function

        Public Shared Function ForLevel(model As LevelModel, newVersionID As Integer, newBuildingID As Integer) As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"@VersionID", newVersionID},
                {"@BuildingID", newBuildingID},
                {"@ProductTypeID", model.ProductTypeID},
                {"@LevelNumber", model.LevelNumber},
                {"@LevelName", model.LevelName}
            }
        End Function

        Public Shared Function ForMapping(model As ActualToLevelMappingModel, newVersionID As Integer, newActualUnitID As Integer, newLevelID As Integer) As Dictionary(Of String, Object)
            Return New Dictionary(Of String, Object) From {
                {"@VersionID", newVersionID},
                {"@ActualUnitID", newActualUnitID},
                {"@LevelID", newLevelID},
                {"@Quantity", model.Quantity}
            }
        End Function

    End Class
End Namespace
