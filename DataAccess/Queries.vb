Namespace DataAccess
    Public Module Queries
        ' Projects (unchanged, targets parent projects)
        Public Const SelectProjects As String = "SELECT p.*, pt.ProjectTypeName AS ProjectType, e.EstimatorName AS Estimator, ca.CustomerName AS ArchitectName, ce.CustomerName AS EngineerName FROM Projects p LEFT JOIN ProjectType pt ON p.ProjectTypeID = pt.ProjectTypeID LEFT JOIN Estimator e ON p.EstimatorID = e.EstimatorID LEFT JOIN Customer ca ON p.ArchitectID = ca.CustomerID AND ca.CustomerType = 2 LEFT JOIN Customer ce ON p.EngineerID = ce.CustomerID AND ce.CustomerType = 3 ORDER BY BidDate DESC"
        Public Const InsertProject As String = "INSERT INTO Projects (JBID, ProjectTypeID, ProjectName, EstimatorID, Address, City, State, Zip, BidDate, ArchPlansDated, EngPlansDated, MilesToJobSite, TotalNetSqft, TotalGrossSqft, ArchitectID, EngineerID, ProjectNotes, LastModifiedDate, CreatedDate) OUTPUT INSERTED.ProjectID VALUES (@JBID, @ProjectTypeID, @ProjectName, @EstimatorID, @Address, @City, @State, @Zip, @BidDate, @ArchPlansDated, @EngPlansDated, @MilesToJobSite, @TotalNetSqft, @TotalGrossSqft, @ArchitectID, @EngineerID, @ProjectNotes, GetDate(), GetDate())"
        Public Const UpdateProject As String = "UPDATE Projects SET JBID = @JBID, ProjectTypeID = @ProjectTypeID, ProjectName = @ProjectName, EstimatorID = @EstimatorID, Address = @Address, City = @City, State = @State, Zip = @Zip, BidDate = @BidDate, ArchPlansDated = @ArchPlansDated, EngPlansDated = @EngPlansDated, MilesToJobSite = @MilesToJobSite, TotalNetSqft = @TotalNetSqft, TotalGrossSqft = @TotalGrossSqft, ArchitectID = @ArchitectID, EngineerID = @EngineerID, ProjectNotes = @ProjectNotes, LastModifiedDate = GetDate() WHERE ProjectID = @ProjectID"
        Public Const SelectProjectByID As String = "SELECT p.*, pt.ProjectTypeName AS ProjectType, e.EstimatorName AS Estimator, ca.CustomerName AS ArchitectName, ce.CustomerName AS EngineerName FROM Projects p LEFT JOIN ProjectType pt ON p.ProjectTypeID = pt.ProjectTypeID LEFT JOIN Estimator e ON p.EstimatorID = e.EstimatorID LEFT JOIN Customer ca ON p.ArchitectID = ca.CustomerID AND ca.CustomerType = 2 LEFT JOIN Customer ce ON p.EngineerID = ce.CustomerID AND ce.CustomerType = 3 WHERE p.ProjectID = @ProjectID"
        ' Buildings (updated to use VersionID)
        Public Const SelectBuildingsByVersionID As String = "SELECT * FROM Buildings WHERE VersionID = @VersionID"
        Public Const InsertBuilding As String = "INSERT INTO Buildings (BuildingName, BuildingType, ResUnits, BldgQty, VersionID, LastModifiedDate) OUTPUT INSERTED.BuildingID VALUES (@BuildingName, @BuildingType, @ResUnits, @BldgQty, @VersionID, GetDate())"
        Public Const UpdateBuilding As String = "UPDATE Buildings SET BuildingName = @BuildingName, BuildingType = @BuildingType, ResUnits = @ResUnits, BldgQty = @BldgQty, LastModifiedDate = GetDate() WHERE BuildingID = @BuildingID"
        Public Const DeleteBuilding As String = "DELETE FROM Buildings WHERE BuildingID = @BuildingID"

        ' Levels (updated to use VersionID)
        Public Const SelectLevelsByBuilding As String = "SELECT l.*, pt.ProductTypeName FROM Levels l LEFT JOIN ProductType pt ON l.ProductTypeID = pt.ProductTypeID WHERE l.BuildingID = @BuildingID"
        Public Const InsertLevel As String = "INSERT INTO Levels (VersionID, BuildingID, ProductTypeID, LevelNumber, LevelName, LastModifiedDate) OUTPUT INSERTED.LevelID VALUES (@VersionID, @BuildingID, @ProductTypeID, @LevelNumber, @LevelName, GetDate())"
        Public Const UpdateLevel As String = "UPDATE Levels SET ProductTypeID = @ProductTypeID, LevelNumber = @LevelNumber, LevelName = @LevelName, LastModifiedDate = GetDate() WHERE LevelID = @LevelID"
        Public Const DeleteLevel As String = "DELETE FROM Levels WHERE LevelID = @LevelID"
        Public Const GetBuildingIDByLevelID As String = "SELECT BuildingID FROM Levels WHERE LevelID = @LevelID"
        Public Const SelectLevelsByVersionID As String = "SELECT LevelID, BuildingID, ProductTypeID, LevelNumber, LevelName FROM Levels WHERE VersionID = @VersionID"
        Public Const SelectLevelByID As String = "SELECT * FROM Levels WHERE LevelID = @LevelID"
        Public Const SelectLevelEstimateWithProductType As String = "
    SELECT 
        l.OverallPrice,
        l.OverallBDFT,
        l.LumberCost,
        l.PlateCost,
        l.LaborCost,
        l.LaborMH,
        l.ItemsCost,
        l.DeliveryCost,
        l.DesignCost,
        l.MGMTCost,
        l.JobSuppliesCost,
        l.OverallCost,
        pt.ProductTypeName
    FROM Levels l
    JOIN ActualToLevelMapping alm ON l.LevelID = alm.LevelID
    JOIN ActualUnits au ON alm.ActualUnitID = au.ActualUnitID
    JOIN ProductType pt ON au.ProductTypeID = pt.ProductTypeID
    WHERE l.LevelID = @LevelID AND l.VersionID = @VersionID
    GROUP BY 
        l.OverallPrice, l.OverallBDFT, l.LumberCost, l.PlateCost, l.LaborCost, l.LaborMH,
        l.ItemsCost, l.DeliveryCost, l.DesignCost, l.MGMTCost, l.JobSuppliesCost,
        l.OverallCost, pt.ProductTypeName"

        ' Product Types (unchanged)
        Public Const SelectProductTypes As String = "SELECT * FROM ProductType"

        ' Project Product Settings (updated to use VersionID)
        Public Const SelectProjectProductSettings As String = "SELECT * FROM ProjectProductSettings WHERE VersionID = @VersionID"
        Public Const InsertProjectProductSetting As String = "INSERT INTO ProjectProductSettings (VersionID, ProductTypeID, MarginPercent, LumberAdder) OUTPUT INSERTED.SettingID VALUES (@VersionID, @ProductTypeID, @MarginPercent, @LumberAdder)"
        Public Const UpdateProjectProductSetting As String = "UPDATE ProjectProductSettings SET MarginPercent = @MarginPercent, LumberAdder = @LumberAdder WHERE SettingID = @SettingID"

        ' Customers (unchanged)
        Public Const SelectCustomers As String = "SELECT c.*, ct.CustomerTypeName FROM Customer c LEFT JOIN CustomerType ct ON c.CustomerType = ct.CustomerTypeID WHERE (@CustomerType IS NULL OR c.CustomerType = @CustomerType) order by CustomerName"
        Public Const InsertCustomer As String = "INSERT INTO Customer (CustomerName, CustomerType) OUTPUT INSERTED.CustomerID VALUES (@CustomerName, @CustomerType)"
        Public Const UpdateCustomer As String = "UPDATE Customer SET CustomerName = @CustomerName, CustomerType = @CustomerType WHERE CustomerID = @CustomerID"
        Public Const SelectCustomerTypes As String = "SELECT CustomerTypeID, CustomerTypeName FROM CustomerType ORDER BY CustomerTypeName"

        ' Sales (unchanged)
        Public Const SelectSales As String = "SELECT * FROM Sales"
        Public Const InsertSales As String = "INSERT INTO Sales (SalesName) OUTPUT INSERTED.SalesID VALUES (@SalesName)"
        Public Const UpdateSales As String = "UPDATE Sales SET SalesName = @SalesName WHERE SalesID = @SalesID"

        ' RawUnits (updated to use VersionID)
        Public Const SelectRawUnitsByVersion As String = "SELECT * FROM RawUnits WHERE VersionID = @VersionID"
        Public Const InsertRawUnit As String = "INSERT INTO RawUnits (RawUnitName, VersionID, ProductTypeID, BF, LF, EWPLF, SqFt, FCArea, LumberCost, PlateCost, ManufLaborCost, DesignLabor, MGMTLabor, JobSuppliesCost, ManHours, ItemCost, OverallCost, DeliveryCost, TotalSellPrice, AvgSPFNo2, SPFNo2BDFT, Avg241800, MSR241800BDFT, Avg242400, MSR242400BDFT, Avg261800, MSR261800BDFT, Avg262400, MSR262400BDFT) OUTPUT INSERTED.RawUnitID VALUES (@RawUnitName, @VersionID, @ProductTypeID, @BF, @LF, @EWPLF, @SqFt, @FCArea, @LumberCost, @PlateCost, @ManufLaborCost, @DesignLabor, @MGMTLabor, @JobSuppliesCost, @ManHours, @ItemCost, @OverallCost, @DeliveryCost, @TotalSellPrice, @AvgSPFNo2, @SPFNo2BDFT, @Avg241800, @MSR241800BDFT, @Avg242400, @MSR242400BDFT, @Avg261800, @MSR261800BDFT, @Avg262400, @MSR262400BDFT)"
        Public Const SelectRawUnitByID As String = "SELECT * FROM RawUnits WHERE RawUnitID = @RawUnitID"
        Public Const UpdateRawUnit As String = "UPDATE RawUnits SET RawUnitName = @RawUnitName, BF = @BF, LF = @LF, EWPLF = @EWPLF, SqFt = @SqFt, FCArea = @FCArea, LumberCost = @LumberCost, PlateCost = @PlateCost, ManufLaborCost = @ManufLaborCost, DesignLabor = @DesignLabor, MGMTLabor = @MGMTLabor, JobSuppliesCost = @JobSuppliesCost, ManHours = @ManHours, ItemCost = @ItemCost, OverallCost = @OverallCost, DeliveryCost = @DeliveryCost, TotalSellPrice = @TotalSellPrice, AvgSPFNo2 = @AvgSPFNo2, SPFNo2BDFT = @SPFNo2BDFT, Avg241800 = @Avg241800, MSR241800BDFT = @MSR241800BDFT, Avg242400 = @Avg242400, MSR242400BDFT = @MSR242400BDFT, Avg261800 = @Avg261800, MSR261800BDFT = @MSR261800BDFT, Avg262400 = @Avg262400, MSR262400BDFT = @MSR262400BDFT WHERE RawUnitID = @RawUnitID"

        ' ActualUnits (updated to use VersionID)
        Public Const InsertActualUnit As String = "INSERT INTO ActualUnits (VersionID, RawUnitID, ProductTypeID, UnitName, PlanSQFT, UnitType, OptionalAdder, ColorCode) OUTPUT INSERTED.ActualUnitID VALUES (@VersionID, @RawUnitID, @ProductTypeID, @UnitName, @PlanSQFT, @UnitType, @OptionalAdder, @ColorCode)"
        Public Const UpdateActualUnit As String = "UPDATE ActualUnits SET UnitName = @UnitName, PlanSQFT = @PlanSQFT, UnitType = @UnitType, OptionalAdder = @OptionalAdder, ColorCode = @ColorCode WHERE ActualUnitID = @ActualUnitID"
        Public Const SelectActualUnitsByVersion As String = "SELECT au.*, ru.RawUnitName FROM ActualUnits au JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID WHERE au.VersionID = @VersionID"
        Public Const DeleteActualUnit As String = "DELETE FROM ActualUnits WHERE ActualUnitID = @ActualUnitID"
        Public Const SelectActualUnitIDsByLevelID As String = "SELECT DISTINCT ActualUnitID FROM ActualToLevelMapping WHERE LevelID = @LevelID"
        Public Const CountMappingsByActualUnitID As String = "SELECT COUNT(*) FROM ActualToLevelMapping WHERE ActualUnitID = @ActualUnitID"
        Public Const SelectActualUnitByID As String = "SELECT au.*, ru.RawUnitName AS ReferencedRawUnitName FROM ActualUnits au JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID WHERE au.ActualUnitID = @ActualUnitID"
        Public Const SelectActualToLevelMappingsByActualUnitID As String = "SELECT * FROM ActualToLevelMapping WHERE ActualUnitID = @ActualUnitID"


        ' ActualToLevelMapping (updated to use VersionID)
        Public Const InsertActualToLevelMapping As String = "INSERT INTO ActualToLevelMapping (VersionID, ActualUnitID, LevelID, Quantity) OUTPUT INSERTED.MappingID VALUES (@VersionID, @ActualUnitID, @LevelID, @Quantity)"
        Public Const UpdateActualToLevelMapping As String = "UPDATE ActualToLevelMapping SET Quantity = @Quantity WHERE MappingID = @MappingID"
        Public Const DeleteActualToLevelMappingByLevelID As String = "DELETE FROM ActualToLevelMapping WHERE LevelID = @LevelID"
        Public Const DeleteActualToLevelMappingByMappingID As String = "DELETE FROM ActualToLevelMapping WHERE MappingID = @MappingID"
        Public Const SelectActualToLevelMappingsByLevelID As String = "SELECT alm.*, au.*, ru.RawUnitName AS ReferencedRawUnitName FROM ActualToLevelMapping alm JOIN ActualUnits au ON alm.ActualUnitID = au.ActualUnitID JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID WHERE alm.LevelID = @LevelID"

        ' CalculatedComponents (updated to use VersionID)
        Public Const DeleteCalculatedComponentsByActualUnitID As String = "DELETE FROM CalculatedComponents WHERE ActualUnitID = @ActualUnitID"
        Public Const InsertCalculatedComponent As String = "INSERT INTO CalculatedComponents (VersionID, ActualUnitID, ComponentType, Value) VALUES (@VersionID, @ActualUnitID, @ComponentType, @Value)"
        Public Const SelectCalculatedComponentsByActualUnitID As String = "SELECT * FROM CalculatedComponents WHERE ActualUnitID = @ActualUnitID"

        ' ProjectType (unchanged)
        Public Const SelectProjectTypes As String = "SELECT * FROM ProjectType ORDER BY ProjectTypeName"
        Public Const InsertProjectType As String = "INSERT INTO ProjectType (ProjectTypeName, Description) OUTPUT INSERTED.ProjectTypeID VALUES (@ProjectTypeName, @Description)"
        Public Const UpdateProjectType As String = "UPDATE ProjectType SET ProjectTypeName = @ProjectTypeName, Description = @Description WHERE ProjectTypeID = @ProjectTypeID"

        ' Estimator (unchanged)
        Public Const SelectEstimators As String = "SELECT * FROM Estimator ORDER BY EstimatorName"
        Public Const InsertEstimator As String = "INSERT INTO Estimator (EstimatorName) OUTPUT INSERTED.EstimatorID VALUES (@EstimatorName)"
        Public Const UpdateEstimator As String = "UPDATE Estimator SET EstimatorName = @EstimatorName WHERE EstimatorID = @EstimatorID"

        ' Configuration (unchanged)
        Public Const SelectConfigValue As String = "SELECT Value FROM Configuration WHERE ConfigKey = @ConfigKey"

        ' Additional Queries for Rollups and Utilities (updated where necessary)
        Public Const SelectMilesToJobSite As String = "SELECT MilesToJobSite FROM Projects WHERE ProjectID = @ProjectID"
        Public Const SelectLumberAdder As String = "SELECT LumberAdder FROM ProjectProductSettings WHERE VersionID = @VersionID AND ProductTypeID = @ProductTypeID"
        Public Const SelectMarginPercent As String = "SELECT MarginPercent FROM ProjectProductSettings WHERE VersionID = @VersionID AND ProductTypeID = @ProductTypeID"

        ' Rollup Calculations for Levels (updated to use VersionID)
        Public Const CalculateOverallSQFT As String = "SELECT SUM(au.PlanSQFT * alm.Quantity) AS OverallSQFT FROM ActualUnits au JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE alm.LevelID = @LevelID"
        Public Const CalculateOverallLF As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS OverallLF FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'LF/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateOverallBDFT As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS OverallBDFT FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'BDFT/SQFT' AND alm.LevelID = @LevelID"
        'Public Const CalculateLumberCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) + COALESCE((l.OverallBDFT / 1000) * pps.LumberAdder, 0) AS LumberCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID JOIN Levels l ON alm.LevelID = l.LevelID JOIN ProjectProductSettings pps ON l.VersionID = pps.VersionID AND au.ProductTypeID = pps.ProductTypeID WHERE cc.ComponentType = 'Lumber/SQFT' AND l.LevelID = @LevelID GROUP BY l.OverallBDFT, pps.LumberAdder"
        Public Const CalculateLumberCost As String = "SELECT SUM(rlh.LumberCost * au.OptionalAdder * alm.Quantity) + COALESCE((l.OverallBDFT / 1000) * pps.LumberAdder, 0) AS LumberCost " &
                                                    "FROM ActualToLevelMapping alm " &
                                                    "JOIN ActualUnits au ON alm.ActualUnitID = au.ActualUnitID " &
                                                    "JOIN (SELECT RawUnitID, LumberCost " &
                                                          "FROM RawUnitLumberHistory rlh1 " &
                                                          "WHERE rlh1.UpdateDate = (SELECT MAX(UpdateDate) FROM RawUnitLumberHistory rlh2 WHERE rlh2.RawUnitID = rlh1.RawUnitID AND rlh2.VersionID = @VersionID)) rlh ON au.RawUnitID = rlh.RawUnitID " &
                                                    "JOIN Levels l ON alm.LevelID = l.LevelID " &
                                                    "JOIN ProjectProductSettings pps ON l.VersionID = pps.VersionID AND au.ProductTypeID = pps.ProductTypeID " &
                                                    "WHERE alm.LevelID = @LevelID GROUP BY l.OverallBDFT, pps.LumberAdder"
        Public Const CalculatePlateCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS PlateCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'Plate/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateLaborCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS LaborCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'ManufLabor/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateLaborMH As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS LaborMH FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'ManHours/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateDesignCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS DesignCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'DesignLabor/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateMGMTCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS MGMTCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'MGMTLabor/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateJobSuppliesCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS JobSuppliesCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'JobSupplies/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateItemsCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS ItemsCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'ItemCost/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateDeliveryCost As String = "SELECT CASE WHEN l.OverallBDFT < 1 THEN 0 ELSE CASE WHEN ROUND(CEILING(l.OverallBDFT / 10000.0) * c.Value * p.MilesToJobSite, 0) < 150 THEN 150 ELSE ROUND(CEILING(l.OverallBDFT / 10000.0) * c.Value * p.MilesToJobSite, 0) END END AS DeliveryCost FROM Levels l JOIN ProjectVersions pv ON l.VersionID = pv.VersionID JOIN Projects p ON pv.ProjectID = p.ProjectID JOIN Configuration c ON c.ConfigKey = 'MileageRate' WHERE l.LevelID = @LevelID"
        Public Const CalculateOverallCost As String = "SELECT SUM(l.LumberCost + l.PlateCost + l.LaborCost + l.DesignCost + l.MGMTCost + l.JobSuppliesCost + l.ItemsCost) AS OverallCost FROM Levels l WHERE l.LevelID = @LevelID GROUP BY l.LevelID"
        Public Const UpdateLevelRollupsSql As String = "UPDATE Levels SET OverallSQFT = @OverallSQFT, OverallLF = @OverallLF, OverallBDFT = @OverallBDFT, LumberCost = @LumberCost, PlateCost = @PlateCost, LaborCost = @LaborCost, LaborMH = @LaborMH, DesignCost = @DesignCost, MGMTCost = @MGMTCost, JobSuppliesCost = @JobSuppliesCost, ItemsCost = @ItemsCost, DeliveryCost = @DeliveryCost, OverallCost = @OverallCost, OverallPrice = @OverallPrice, TotalSQFT = @TotalSQFT, AvgPricePerSQFT = @AvgPricePerSQFT WHERE LevelID = @LevelID"

        ' Rollup Calculations for Buildings (updated to use VersionID)
        Public Const SelectBldgQty As String = "SELECT BldgQty FROM Buildings WHERE BuildingID = @BuildingID"
        Public Const CalculateFloorCostPerBldg As String = "SELECT SUM(l.OverallCost) AS FloorCostPerBldg FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"
        Public Const CalculateFloorDeliveryCost As String = "SELECT SUM(l.DeliveryCost) AS FloorDeliveryCost FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"
        Public Const CalculateRoofCostPerBldg As String = "SELECT SUM(l.OverallCost) AS RoofCostPerBldg FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"
        Public Const CalculateRoofDeliveryCost As String = "SELECT SUM(l.DeliveryCost) AS RoofDeliveryCost FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"
        Public Const UpdateBuildingRollupsSql As String = "UPDATE Buildings SET FloorCostPerBldg = @FloorCostPerBldg, RoofCostPerBldg = @RoofCostPerBldg, WallCostPerBldg = @WallCostPerBldg, ExtendedFloorCost = @ExtendedFloorCost, ExtendedRoofCost = @ExtendedRoofCost, ExtendedWallCost = @ExtendedWallCost, OverallPrice = @OverallPrice, OverallCost = @OverallCost WHERE BuildingID = @BuildingID"
        Public Const CalculateSumUnitSellNoDelivery As String = "SELECT SUM( (cc.Value * au.PlanSQFT * au.OptionalAdder * alm.Quantity) / (1 - ((ru.TotalSellPrice - ru.OverallCost) / ru.TotalSellPrice)) ) FROM ActualToLevelMapping alm JOIN ActualUnits au ON alm.ActualUnitID = au.ActualUnitID JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID JOIN CalculatedComponents cc ON cc.ActualUnitID = au.ActualUnitID WHERE alm.LevelID = @LevelID AND cc.ComponentType = 'OverallCost/SQFT' AND ru.TotalSellPrice > 0 AND ((ru.TotalSellPrice - ru.OverallCost) / ru.TotalSellPrice) BETWEEN 0 AND 0.999"
        'Public Const CalculateFloorPricePerBldg As String = "SELECT SUM(l.OverallPrice) AS FloorPricePerBldg FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"
        'Public Const CalculateRoofPricePerBldg As String = "SELECT SUM(l.OverallPrice) AS RoofPricePerBldg FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"
        'Public Const CalculateFloorBaseCost As String = "SELECT SUM(l.OverallCost) AS FloorBaseCost FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"
        'Public Const CalculateRoofBaseCost As String = "SELECT SUM(l.OverallCost) AS RoofBaseCost FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"
        ' Modified Building Rollup Queries
        Public Const CalculateFloorPricePerBldg As String = "SELECT SUM(l.OverallPrice) AS FloorPricePerBldg FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"
        Public Const CalculateRoofPricePerBldg As String = "SELECT SUM(l.OverallPrice) AS RoofPricePerBldg FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"
        Public Const CalculateFloorBaseCost As String = "SELECT SUM(l.OverallCost) AS FloorBaseCost FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"
        Public Const CalculateRoofBaseCost As String = "SELECT SUM(l.OverallCost) AS RoofBaseCost FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"
        Public Const CalculateWallPricePerBldg As String = "SELECT SUM(l.OverallPrice) AS WallPricePerBldg FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 3"
        Public Const CalculateWallBaseCost As String = "SELECT SUM(l.OverallCost) AS WallBaseCost FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 3"


        ' ProjectVersions Queries
        Public Const SelectProjectVersions As String = "SELECT pv.*, c.CustomerName, s.SalesName FROM ProjectVersions pv LEFT JOIN Customer c ON pv.CustomerID = c.CustomerID AND c.CustomerType = 1 LEFT JOIN Sales s ON pv.SalesID = s.SalesID WHERE ProjectID = @ProjectID ORDER BY VersionDate DESC"
        Public Const InsertProjectVersion As String = "INSERT INTO ProjectVersions (ProjectID, VersionName, VersionDate, Description, LastModifiedDate, CustomerID, SalesID, MondayID) OUTPUT INSERTED.VersionID VALUES (@ProjectID, @VersionName, @VersionDate, @Description, GetDate(), @CustomerID, @SalesID, @MondayID)"
        Public Const UpdateProjectVersion As String = "UPDATE ProjectVersions SET VersionName = @VersionName, LastModifiedDate = GetDate(), CustomerID = @CustomerID, SalesID = @SalesID, MondayID=@MondayID WHERE VersionID = @VersionID"

        ' ProjectVersions Duplication Queries
        Public Const DuplicateBuildings As String = "INSERT INTO Buildings (BuildingName, BuildingType, ResUnits, BldgQty, VersionID, LastModifiedDate) OUTPUT INSERTED.BuildingID SELECT BuildingName, BuildingType, ResUnits, BldgQty, @NewVersionID, GetDate() FROM Buildings WHERE VersionID = @OriginalVersionID"
        Public Const DuplicateLevels As String = "INSERT INTO Levels (VersionID, BuildingID, ProductTypeID, LevelNumber, LevelName, LastModifiedDate) OUTPUT INSERTED.LevelID SELECT @NewVersionID, @NewBuildingID, ProductTypeID, LevelNumber, LevelName, GetDate() FROM Levels WHERE VersionID = @OriginalVersionID AND BuildingID = @OriginalBuildingID"
        Public Const DuplicateRawUnits As String = "INSERT INTO RawUnits (RawUnitName, VersionID, ProductTypeID, BF, LF, EWPLF, SqFt, FCArea, LumberCost, PlateCost, ManufLaborCost, DesignLabor, MGMTLabor, JobSuppliesCost, ManHours, ItemCost, OverallCost, DeliveryCost, TotalSellPrice, AvgSPFNo2, SPFNo2BDFT, Avg241800, MSR241800BDFT, Avg242400, MSR242400BDFT, Avg261800, MSR261800BDFT, Avg262400, MSR262400BDFT) OUTPUT INSERTED.RawUnitID SELECT RawUnitName, @NewVersionID, ProductTypeID, BF, LF, EWPLF, SqFt, FCArea, LumberCost, PlateCost, ManufLaborCost, DesignLabor, MGMTLabor, JobSuppliesCost, ManHours, ItemCost, OverallCost, DeliveryCost, TotalSellPrice, AvgSPFNo2, SPFNo2BDFT, Avg241800, MSR241800BDFT, Avg242400, MSR242400BDFT, Avg261800, MSR261800BDFT, Avg262400, MSR262400BDFT FROM RawUnits WHERE VersionID = @OriginalVersionID"
        Public Const DuplicateActualUnits As String = "INSERT INTO ActualUnits (VersionID, RawUnitID, ProductTypeID, UnitName, PlanSQFT, UnitType, OptionalAdder) OUTPUT INSERTED.ActualUnitID SELECT @NewVersionID, @NewRawUnitID, ProductTypeID, UnitName, PlanSQFT, UnitType, OptionalAdder FROM ActualUnits WHERE VersionID = @OriginalVersionID AND RawUnitID = @OriginalRawUnitID"
        Public Const DuplicateCalculatedComponents As String = "INSERT INTO CalculatedComponents (VersionID, ActualUnitID, ComponentType, Value) OUTPUT INSERTED.ComponentID SELECT @NewVersionID, @NewActualUnitID, ComponentType, Value FROM CalculatedComponents WHERE VersionID = @OriginalVersionID AND ActualUnitID = @OriginalActualUnitID"
        Public Const DuplicateActualToLevelMapping As String = "INSERT INTO ActualToLevelMapping (VersionID, ActualUnitID, LevelID, Quantity) OUTPUT INSERTED.MappingID SELECT @NewVersionID, @NewActualUnitID, @NewLevelID, Quantity FROM ActualToLevelMapping WHERE VersionID = @OriginalVersionID AND ActualUnitID = @OriginalActualUnitID AND LevelID = @OriginalLevelID"
        Public Const DuplicateProjectProductSettings As String = "INSERT INTO ProjectProductSettings (VersionID, ProductTypeID, MarginPercent, LumberAdder) OUTPUT INSERTED.SettingID SELECT @NewVersionID, ProductTypeID, MarginPercent, LumberAdder FROM ProjectProductSettings WHERE VersionID = @OriginalVersionID"

        ' ProjectDesignInfo
        Public Const SelectProjectDesignInfo As String = "SELECT * FROM ProjectDesignInfo WHERE ProjectID = @ProjectID"
        Public Const InsertProjectDesignInfo As String = "INSERT INTO ProjectDesignInfo (ProjectID,  BuildingCode, Importance, ExposureCategory, WindSpeed, SnowLoadType, OccupancyCategory, RoofPitches, FloorDepths, WallHeights, HeelHeights) OUTPUT INSERTED.InfoID VALUES (@ProjectID, @BuildingCode, @Importance, @ExposureCategory, @WindSpeed, @SnowLoadType, @OccupancyCategory, @RoofPitches, @FloorDepths, @WallHeights, @HeelHeights)"
        Public Const UpdateProjectDesignInfo As String = "UPDATE ProjectDesignInfo SET BuildingCode = @BuildingCode, Importance = @Importance, ExposureCategory = @ExposureCategory, WindSpeed = @WindSpeed, SnowLoadType = @SnowLoadType, OccupancyCategory = @OccupancyCategory, RoofPitches = @RoofPitches, FloorDepths = @FloorDepths, WallHeights = @WallHeights, HeelHeights=@HeelHeights WHERE InfoID = @InfoID"

        ' ProjectLoads
        Public Const SelectProjectLoads As String = "SELECT * FROM ProjectLoads WHERE ProjectID = @ProjectID ORDER BY Category desc"
        Public Const DeleteProjectLoads As String = "DELETE FROM ProjectLoads WHERE ProjectID = @ProjectID"
        Public Const InsertProjectLoad As String = "INSERT INTO ProjectLoads (ProjectID,  Category, TCLL, TCDL, BCLL, BCDL, OCSpacing, LiveLoadDeflection, TotalLoadDeflection, Absolute, Deflection) OUTPUT INSERTED.LoadID VALUES (@ProjectID,  @Category, @TCLL, @TCDL, @BCLL, @BCDL, @OCSpacing, @LiveLoadDeflection, @TotalLoadDeflection, @Absolute, @Deflection)"

        ' ProjectBearingStyles
        Public Const SelectProjectBearingStyles As String = "SELECT * FROM ProjectBearingStyles WHERE ProjectID = @ProjectID "
        Public Const InsertProjectBearingStyles As String = "INSERT INTO ProjectBearingStyles (ProjectID,  ExtWallStyle, ExtRimRibbon, IntWallStyle, IntRimRibbon, CorridorWallStyle, CorridorRimRibbon) OUTPUT INSERTED.BearingID VALUES (@ProjectID,  @ExtWallStyle, @ExtRimRibbon, @IntWallStyle, @IntRimRibbon, @CorridorWallStyle, @CorridorRimRibbon)"
        Public Const UpdateProjectBearingStyles As String = "UPDATE ProjectBearingStyles SET ExtWallStyle = @ExtWallStyle, ExtRimRibbon = @ExtRimRibbon, IntWallStyle = @IntWallStyle, IntRimRibbon = @IntRimRibbon, CorridorWallStyle = @CorridorWallStyle, CorridorRimRibbon = @CorridorRimRibbon WHERE BearingID = @BearingID"

        ' ProjectGeneralNotes
        Public Const SelectProjectGeneralNotes As String = "SELECT * FROM ProjectGeneralNotes WHERE ProjectID = @ProjectID"
        Public Const InsertProjectGeneralNotes As String = "INSERT INTO ProjectGeneralNotes (ProjectID,  Notes) OUTPUT INSERTED.NoteID VALUES (@ProjectID,  @Notes)"
        Public Const UpdateProjectGeneralNotes As String = "UPDATE ProjectGeneralNotes SET Notes = @Notes WHERE NoteID = @NoteID"

        ' ProjectItems
        Public Const SelectProjectItems As String = "SELECT ItemID, ProjectID, Section, KN, Description, Status, Note FROM ProjectItems WHERE ProjectID = @ProjectID"
        Public Const DeleteProjectItems As String = "DELETE FROM ProjectItems WHERE ProjectID = @ProjectID "
        Public Const InsertProjectItem As String = "INSERT INTO ProjectItems (ProjectID, Section, KN, Description, Status, Note) OUTPUT INSERTED.ItemID VALUES (@ProjectID, @Section, @KN, @Description, @Status, @Note)"

        Public Const SelectComboOptions As String = "SELECT Value FROM ComboOptions WHERE Category = @Category ORDER BY DisplayOrder, Value"
        Public Const SelectItemOptions As String = "SELECT KN, Description FROM ItemOptions WHERE Section = @Section ORDER BY DisplayOrder"

        ' Deletion Queries for Project Cleanup
        Public Const DeleteActualToLevelMappingsByVersion As String = "DELETE FROM ActualToLevelMapping WHERE VersionID = @VersionID"
        Public Const DeleteCalculatedComponentsByVersion As String = "DELETE FROM CalculatedComponents WHERE VersionID = @VersionID"
        Public Const DeleteActualUnitsByVersion As String = "DELETE FROM ActualUnits WHERE VersionID = @VersionID"
        Public Const DeleteRawUnitsByVersion As String = "DELETE FROM RawUnits WHERE VersionID = @VersionID"
        Public Const DeleteLevelsByVersion As String = "DELETE FROM Levels WHERE VersionID = @VersionID"
        Public Const DeleteBuildingsByVersion As String = "DELETE FROM Buildings WHERE VersionID = @VersionID"
        Public Const DeleteProjectProductSettingsByVersion As String = "DELETE FROM ProjectProductSettings WHERE VersionID = @VersionID"
        Public Const DeleteProjectVersionsByProject As String = "DELETE FROM ProjectVersions WHERE ProjectID = @ProjectID"
        Public Const DeleteProjectDesignInfoByProject As String = "DELETE FROM ProjectDesignInfo WHERE ProjectID = @ProjectID"
        Public Const DeleteProjectLoadsByProject As String = "DELETE FROM ProjectLoads WHERE ProjectID = @ProjectID"
        Public Const DeleteProjectBearingStylesByProject As String = "DELETE FROM ProjectBearingStyles WHERE ProjectID = @ProjectID"
        Public Const DeleteProjectGeneralNotesByProject As String = "DELETE FROM ProjectGeneralNotes WHERE ProjectID = @ProjectID"
        Public Const DeleteProjectItemsByProject As String = "DELETE FROM ProjectItems WHERE ProjectID = @ProjectID"
        Public Const DeleteProjectByID As String = "DELETE FROM Projects WHERE ProjectID = @ProjectID"

        ' LumberType Queries
        Public Const SelectLumberTypes As String = "SELECT LumberTypeID, LumberTypeDesc FROM LumberType ORDER BY LumberTypeDesc"
        Public Const InsertLumberType As String = "INSERT INTO LumberType (LumberTypeDesc) OUTPUT INSERTED.LumberTypeID VALUES (@LumberTypeDesc)"

        ' LumberCostEffective Queries
        Public Const SelectLumberCostEffective As String = "SELECT CostEffectiveID, CosteffectiveDate FROM LumberCostEffective ORDER BY CosteffectiveDate DESC"
        Public Const InsertLumberCostEffective As String = "INSERT INTO LumberCostEffective (CosteffectiveDate) OUTPUT INSERTED.CostEffectiveID VALUES (@CosteffectiveDate)"

        ' LumberCost Queries
        Public Const SelectLumberCostsByEffectiveDate As String = "SELECT lc.LumberCostID, lc.LumberTypeID, lt.LumberTypeDesc, lc.LumberCost, lc.CostEffectiveDateID FROM LumberCost lc JOIN LumberType lt ON lc.LumberTypeID = lt.LumberTypeID WHERE lc.CostEffectiveDateID = @CostEffectiveDateID ORDER BY lt.LumberTypeDesc"
        Public Const InsertLumberCost As String = "INSERT INTO LumberCost (LumberTypeID, LumberCost, CostEffectiveDateID) OUTPUT INSERTED.LumberCostID VALUES (@LumberTypeID, @LumberCost, @CostEffectiveDateID)"
        Public Const UpdateLumberCost As String = "UPDATE LumberCost SET LumberCost = @LumberCost WHERE LumberCostID = @LumberCostID"


        ' RawUnitLumberHistory Queries
        Public Const InsertRawUnitLumberHistory As String = "INSERT INTO RawUnitLumberHistory (RawUnitID, VersionID, CostEffectiveDateID, LumberCost, AvgSPFNo2, Avg241800, Avg242400, Avg261800, Avg262400, UpdateDate, IsActive) OUTPUT INSERTED.HistoryID VALUES (@RawUnitID, @VersionID, @CostEffectiveDateID, @LumberCost, @AvgSPFNo2, @Avg241800, @Avg242400, @Avg261800, @Avg262400, GETDATE(), 1)"
        Public Const SelectLatestLumberHistoryByRawUnit As String = "SELECT TOP 1 * FROM RawUnitLumberHistory WHERE RawUnitID = @RawUnitID AND VersionID = @VersionID ORDER BY UpdateDate DESC"
        Public Const SelectLumberHistoryByVersion As String = "SELECT rlh.*, ru.RawUnitName FROM RawUnitLumberHistory rlh JOIN RawUnits ru ON rlh.RawUnitID = ru.RawUnitID WHERE rlh.VersionID = @VersionID ORDER BY rlh.UpdateDate DESC"
        Public Const SelectLumberTypeIDByDesc As String = "SELECT LumberTypeID FROM LumberType WHERE LumberTypeDesc = @LumberTypeDesc"
        Public Const SelectLumberCostByTypeAndDate As String = "SELECT LumberCost FROM LumberCost WHERE LumberTypeID = @LumberTypeID AND CostEffectiveDateID = @CostEffectiveDateID"
        Public Const SetLumberHistoryActive As String = "UPDATE RawUnitLumberHistory SET IsActive = 0 WHERE RawUnitID = @RawUnitID AND VersionID = @VersionID; UPDATE RawUnitLumberHistory SET IsActive = 1 WHERE HistoryID = @HistoryID"


        ' New query to delete a history record
        Public Const DeleteLumberHistory As String = "DELETE FROM RawUnitLumberHistory WHERE HistoryID = @HistoryID AND VersionID = @VersionID"
        ' New query for distinct CosteffectiveDate values
        Public Const SelectDistinctLumberHistoryDates As String = "SELECT rlh.CostEffectiveDateID, lce.CosteffectiveDate, CASE WHEN EXISTS (SELECT 1 FROM RawUnitLumberHistory rlh2 WHERE rlh2.CostEffectiveDateID = rlh.CostEffectiveDateID AND rlh2.VersionID = @VersionID AND rlh2.IsActive = 1) THEN 1 ELSE 0 END AS IsActive, MAX(rlh.UpdateDate) AS UpdateDate FROM RawUnitLumberHistory rlh JOIN LumberCostEffective lce ON rlh.CostEffectiveDateID = lce.CostEffectiveID WHERE rlh.VersionID = @VersionID GROUP BY rlh.CostEffectiveDateID, lce.CosteffectiveDate ORDER BY lce.CosteffectiveDate DESC"

        ' New query to find CostEffectiveDateID by cost
        Public Const SelectCostEffectiveDateIDByCost As String = "SELECT lc.CostEffectiveDateID FROM LumberCost lc JOIN LumberType lt ON lc.LumberTypeID = lt.LumberTypeID WHERE lt.LumberTypeID = 1 AND lc.LumberCost = @SPFLumberCost"


        '--- Lumber Futures -------------------------------------------------
        Public Const SelectLumberFuturesByVersion As String = "SELECT LumberFutureID, ContractMonth, PriorSettle, PullDate FROM LumberFutures WHERE VersionID = @VersionID ORDER BY PullDate"
        Public Const UpsertLumberFuture As String = "IF EXISTS (SELECT 1 FROM LumberFutures WHERE VersionID = @VersionID AND ContractMonth = @ContractMonth) UPDATE LumberFutures SET PriorSettle = @PriorSettle, PullDate = GETDATE() WHERE VersionID = @VersionID AND ContractMonth = @ContractMonth ELSE INSERT INTO LumberFutures (VersionID, ContractMonth, PriorSettle) VALUES (@VersionID, @ContractMonth, @PriorSettle)"
        Public Const UpdateProjectProductSettingLumberAdder As String = "UPDATE ProjectProductSettings SET LumberAdder = @LumberAdder WHERE VersionID = @VersionID AND ProductTypeID = @ProductTypeID"
        Public Const InsertLumberFuture As String = "INSERT INTO LumberFutures (VersionID, ContractMonth, PriorSettle, PullDate) VALUES (@VersionID, @ContractMonth, @PriorSettle, GETDATE())"

        Public Const UpdateLumberFuture As String = "UPDATE LumberFutures SET PriorSettle = @PriorSettle, PullDate = GETDATE() WHERE VersionID = @VersionID AND ContractMonth = @ContractMonth"

        ' Project summary – includes City, State, Architect, Engineer, plan dates and Salesman
        'Public Const SelectProjectSummary As String = "SELECT p.ProjectName, p.JBID,  pv.VersionName, c.CustomerName, s.SalesName, p.City, p.State, ca.CustomerName As Architect, ce.CustomerName As Engineer, p.ArchPlansDated, p.EngPlansDated, b.BuildingName, b.BldgQty, b.OverallPrice, l.LevelName, l.OverallSQFT, l.OverallPrice As OverallPrice_Level, pt.ProductTypeName, SUM(b.BldgQty) OVER (PARTITION BY p.ProjectID) AS TotalBldgQty FROM Projects p INNER JOIN ProjectVersions pv On p.ProjectID = pv.ProjectID And pv.VersionID = @VersionID LEFT JOIN Customer c On pv.CustomerID = c.CustomerID And c.CustomerType = 1 LEFT JOIN Sales s On pv.SalesID = s.SalesID LEFT JOIN Customer ca On p.ArchitectID = ca.CustomerID And ca.CustomerType = 2 LEFT JOIN Customer ce On p.EngineerID = ce.CustomerID And ce.CustomerType = 3 LEFT JOIN Buildings b On pv.VersionID = b.VersionID LEFT JOIN Levels l On b.BuildingID = l.BuildingID LEFT JOIN ProductType pt On l.ProductTypeID = pt.ProductTypeID  WHERE p.ProjectID = @ProjectID"
        Public Const SelectProjectSummary As String = "
SELECT 
    p.ProjectName, 
    p.JBID, 
    pv.VersionName, 
    c.CustomerName, 
    s.SalesName,
    p.City, 
    p.State, 
    ca.CustomerName AS Architect, 
    ce.CustomerName AS Engineer, 
    p.ArchPlansDated, 
    p.EngPlansDated,
    b.BuildingName, 
    b.BldgQty,
    b.OverallPrice,
    l.LevelName, 
    l.OverallSQFT, 
    l.OverallPrice AS OverallPrice_Level,
    pt.ProductTypeName,
    -- TOTAL: Sum BldgQty once per Building (from distinct buildings)
    (SELECT SUM(b2.BldgQty) 
     FROM Buildings b2 
     WHERE b2.VersionID = pv.VersionID) AS TotalBldgQty
FROM Projects p
INNER JOIN ProjectVersions pv ON p.ProjectID = pv.ProjectID AND pv.VersionID = @VersionID
LEFT JOIN Customer c ON pv.CustomerID = c.CustomerID AND c.CustomerType = 1
LEFT JOIN Sales s ON pv.SalesID = s.SalesID
LEFT JOIN Customer ca ON p.ArchitectID = ca.CustomerID AND ca.CustomerType = 2
LEFT JOIN Customer ce ON p.EngineerID = ce.CustomerID AND ce.CustomerType = 3
LEFT JOIN Buildings b ON pv.VersionID = b.VersionID
LEFT JOIN Levels l ON b.BuildingID = l.BuildingID
LEFT JOIN ProductType pt ON l.ProductTypeID = pt.ProductTypeID
WHERE p.ProjectID = @ProjectID"

        ' ===============================================================
        ' LEVELACTUALS – FINAL PRODUCTION QUERIES (MULTI-SHIPMENT READY)
        ' ===============================================================

        ' 1. Insert new actuals row (Design or Invoice) – used by both MiTek CSV and BisTrack pull
        Public Const InsertLevelActual As String = "
INSERT INTO LevelActuals
    (LevelID, VersionID, StageType,
     ActualBDFT, ActualLumberCost, ActualPlateCost, ActualManufLaborCost, ActualManufMH,
     ActualItemCost, ActualDeliveryCost, ActualMiscLaborCost, ActualTotalCost,
     ActualSoldAmount, ActualMarginPercent, AvgSPFNo2Actual,
     MiTekJobNumber, BistrackWorksOrder, BisTrackSalesOrder,
     BuildingID, ImportedBy, Notes)
OUTPUT INSERTED.ActualID
VALUES
    (@LevelID, @VersionID, @StageType,
     @ActualBDFT, @ActualLumberCost, @ActualPlateCost, @ActualManufLaborCost, @ActualManufMH,
     @ActualItemCost, @ActualDeliveryCost, @ActualMiscLaborCost, @ActualTotalCost,
     @ActualSoldAmount, @ActualMarginPercent, @AvgSPFNo2Actual,
     @MiTekJobNumber, @BistrackWorksOrder, @BisTrackSalesOrder,
     @BuildingID, @ImportedBy, @Notes)"

        ' 2. Get ALL actuals for a specific Level (shows every shipment)
        Public Const SelectAllActualsForLevel As String = "
    SELECT
        ActualID, StageType, ImportDate, MiTekJobNumber, BistrackWorksOrder, BisTrackSalesOrder,
        ActualBDFT, ActualLumberCost, ActualPlateCost, ActualManufLaborCost,
        ActualItemCost, ActualDeliveryCost, ActualMiscLaborCost, ActualTotalCost,
        ActualSoldAmount, ActualMarginPercent, AvgSPFNo2Actual,ActualManufMH, Notes
    FROM LevelActuals
    WHERE LevelID = @LevelID AND VersionID = @VersionID
    ORDER BY ImportDate DESC"

        ' 3. HOLY GRAIL: Multi-shipment variance grid (Estimate vs. every actual shipment)
        Public Const SelectMultiShipmentVariance As String = "
    SELECT 
        l.LevelID,
        l.LevelName,
        b.BldgQty,
        la.BisTrackSalesOrder AS [Shipment SO],
        la.ImportDate AS [Ship Date],
        l.OverallBDFT AS [Est BDFT per Bldg],
        la.ActualBDFT AS [Actual BDFT],
        ISNULL(la.ActualBDFT, 0) - l.OverallBDFT AS [BDFT Var],
        l.LumberCost AS [Est Lumber per Bldg],
        la.ActualLumberCost AS [Actual Lumber],
        ISNULL(la.ActualLumberCost, 0) - l.LumberCost AS [Lumber Var],
        l.OverallCost AS [Est Cost per Bldg],
        la.ActualTotalCost AS [Actual Cost],
        ISNULL(la.ActualTotalCost, 0) - l.OverallCost AS [Cost Var],
        l.OverallPrice AS [Est Sold per Bldg],
        la.ActualSoldAmount AS [Actual Sold],
        ISNULL(la.ActualSoldAmount, 0) - l.OverallPrice AS [Sold Var],
        la.ActualMarginPercent AS [Actual Margin %],
        l.AvgSPFNo2 AS [Est SPF#2],
        la.AvgSPFNo2Actual AS [Actual SPF#2]
    FROM Levels l
    JOIN Buildings b ON l.BuildingID = b.BuildingID
    LEFT JOIN LevelActuals la ON l.LevelID = la.LevelID 
                             AND l.VersionID = la.VersionID
                             AND la.StageType = 1   -- 2 = Invoice (change to 1 for Design)
    WHERE l.VersionID = @VersionID
    ORDER BY l.LevelName, la.ImportDate DESC"

        ' 4. Project-level summary across ALL shipments
        Public Const SelectProjectShipmentSummary As String = "
    SELECT 
        COUNT(DISTINCT la.BisTrackSalesOrder) AS TotalShipments,
        SUM(l.OverallBDFT * b.BldgQty) AS TotalEstBDFT,
        SUM(la.ActualBDFT) AS TotalActualBDFT,
        SUM(la.ActualBDFT) - SUM(l.OverallBDFT * b.BldgQty) AS NetBDFTVar,
        SUM(l.OverallCost * b.BldgQty) AS TotalEstCost,
        SUM(la.ActualTotalCost) AS TotalActualCost,
        SUM(la.ActualTotalCost) - SUM(l.OverallCost * b.BldgQty) AS NetCostVar,
        SUM(l.OverallPrice * b.BldgQty) AS TotalEstSold,
        SUM(la.ActualSoldAmount) AS TotalActualSold,
        SUM(la.ActualSoldAmount) - SUM(l.OverallPrice * b.BldgQty) AS NetSoldVar
    FROM Levels l
    JOIN Buildings b ON l.BuildingID = b.BuildingID
    LEFT JOIN LevelActuals la ON l.LevelID = la.LevelID 
                             AND l.VersionID = la.VersionID
                             AND la.StageType = 1
    WHERE l.VersionID = @VersionID"

        ' 5. Design-stage only variance (MiTek actuals vs. Estimate)
        Public Const SelectDesignVariance As String = "
    SELECT 
        l.LevelName,
        l.OverallBDFT AS EstBDFT,
        la.ActualBDFT AS DesignBDFT,
        la.ActualBDFT - l.OverallBDFT AS DesignBDFTVar,
        la.ActualTotalCost - l.OverallCost AS DesignCostVar
    FROM Levels l
    LEFT JOIN LevelActuals la ON l.LevelID = la.LevelID 
                             AND l.VersionID = la.VersionID
                             AND la.StageType = 1
    WHERE l.VersionID = @VersionID"

        ' 6. Check if a specific shipment already exists (prevents duplicates on re-import)
        Public Const SelectExistingShipment As String = "
    SELECT ActualID 
    FROM LevelActuals 
    WHERE LevelID = @LevelID 
      AND VersionID = @VersionID 
      AND BisTrackSalesOrder = @BisTrackSalesOrder 
      AND StageType = 2"

        ' 7. Delete a specific shipment row (for corrections)
        Public Const DeleteShipmentActual As String = "
    DELETE FROM LevelActuals 
    WHERE ActualID = @ActualID"

        ' Users (for login and auto-add)
        Public Const SelectUserByWindowsLogon As String = "SELECT * FROM Users WHERE WindowsLogon = @WindowsLogon"
        Public Const InsertUser As String = "INSERT INTO Users (WindowsLogon, DisplayName, EstimatorID, IsActive, IsAdmin, IsLoggedIn, LastLogin) OUTPUT INSERTED.UserID VALUES (@WindowsLogon, @DisplayName, @EstimatorID, 1, 0, 1, GETDATE())"
        Public Const UpdateUserLoginStatus As String = "UPDATE Users SET IsLoggedIn = @IsLoggedIn, LastLogin = COALESCE(@LastLogin, LastLogin), LastLogout = COALESCE(@LastLogout, LastLogout) WHERE UserID = @UserID"


    End Module
End Namespace
