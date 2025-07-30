Namespace BuildersPSE.DataAccess
    Public Module Queries
        ' Projects
        Public Const SelectProjects As String = "SELECT p.*, pt.ProjectTypeName AS ProjectType, e.EstimatorName AS Estimator, (SELECT TOP 1 c.CustomerName FROM Customer c JOIN CustomertoProjectMapping m ON c.CustomerID = m.CustomerID WHERE m.ProjectID = p.ProjectID ORDER BY m.CustProjID DESC) AS PrimaryCustomer, (SELECT TOP 1 s.SalesName FROM Sales s JOIN SalestoProjectMapping m ON s.SalesID = m.SalesID WHERE m.ProjectID = p.ProjectID ORDER BY m.SalesProjID DESC) AS PrimarySalesman FROM Projects p LEFT JOIN ProjectType pt ON p.ProjectTypeID = pt.ProjectTypeID LEFT JOIN Estimator e ON p.EstimatorID = e.EstimatorID ORDER BY BidDate DESC"
        Public Const InsertProject As String = "INSERT INTO Projects (JBID, ProjectTypeID, ProjectName, EstimatorID, Address, City, State, Zip, BidDate, ArchPlansDated, EngPlansDated, MilesToJobSite, TotalNetSqft, TotalGrossSqft, ProjectArchitect, ProjectEngineer, ProjectNotes, LastModifiedDate) OUTPUT INSERTED.ProjectID VALUES (@JBID, @ProjectTypeID, @ProjectName, @EstimatorID, @Address, @City, @State, @Zip, @BidDate, @ArchPlansDated, @EngPlansDated, @MilesToJobSite, @TotalNetSqft, @TotalGrossSqft, @ProjectArchitect, @ProjectEngineer, @ProjectNotes, GetDate())"
        Public Const UpdateProject As String = "UPDATE Projects SET JBID = @JBID, ProjectTypeID = @ProjectTypeID, ProjectName = @ProjectName, EstimatorID = @EstimatorID, Address = @Address, City = @City, State = @State, Zip = @Zip, BidDate = @BidDate, ArchPlansDated = @ArchPlansDated, EngPlansDated = @EngPlansDated, MilesToJobSite = @MilesToJobSite, TotalNetSqft = @TotalNetSqft, TotalGrossSqft = @TotalGrossSqft, ProjectArchitect = @ProjectArchitect, ProjectEngineer = @ProjectEngineer, ProjectNotes = @ProjectNotes, LastModifiedDate=GetDate() WHERE ProjectID = @ProjectID"
        Public Const SelectProjectByID As String = "SELECT p.*, pt.ProjectTypeName AS ProjectType, e.EstimatorName AS Estimator FROM Projects p LEFT JOIN ProjectType pt ON p.ProjectTypeID = pt.ProjectTypeID LEFT JOIN Estimator e ON p.EstimatorID = e.EstimatorID WHERE ProjectID = @ProjectID"

        ' Buildings
        Public Const SelectBuildingsByProject As String = "SELECT * FROM Buildings WHERE ProjectID = @ProjectID"
        Public Const InsertBuilding As String = "INSERT INTO Buildings (BuildingName, BuildingType, ResUnits, BldgQty, ProjectID, LastModifiedDate) OUTPUT INSERTED.BuildingID VALUES (@BuildingName, @BuildingType, @ResUnits, @BldgQty, @ProjectID, GetDate())"
        Public Const UpdateBuilding As String = "UPDATE Buildings SET BuildingName = @BuildingName, BuildingType = @BuildingType, ResUnits = @ResUnits, BldgQty = @BldgQty, LastModifiedDate = GetDate() WHERE BuildingID = @BuildingID"
        Public Const DeleteBuilding As String = "DELETE FROM Buildings WHERE BuildingID = @BuildingID"
        Public Const GetBuildingIDByLevelID As String = "SELECT BuildingID FROM Levels WHERE LevelID = @LevelID"

        ' Levels
        Public Const SelectLevelsByBuilding As String = "SELECT l.*, pt.ProductTypeName FROM Levels l LEFT JOIN ProductType pt ON l.ProductTypeID = pt.ProductTypeID WHERE l.BuildingID = @BuildingID"
        Public Const InsertLevel As String = "INSERT INTO Levels (ProjectID, BuildingID, ProductTypeID, LevelNumber, LevelName, LastModifiedDate) OUTPUT INSERTED.LevelID VALUES (@ProjectID, @BuildingID, @ProductTypeID, @LevelNumber, @LevelName, GetDate())"
        Public Const UpdateLevel As String = "UPDATE Levels SET ProductTypeID = @ProductTypeID, LevelNumber = @LevelNumber, LevelName = @LevelName, LastModifiedDate = GetDate() WHERE LevelID = @LevelID"
        Public Const DeleteLevel As String = "DELETE FROM Levels WHERE LevelID = @LevelID"

        ' Product Types
        Public Const SelectProductTypes As String = "SELECT * FROM ProductType"

        ' Project Product Settings
        Public Const SelectProjectProductSettings As String = "SELECT * FROM ProjectProductSettings WHERE ProjectID = @ProjectID"
        Public Const InsertProjectProductSetting As String = "INSERT INTO ProjectProductSettings (ProjectID, ProductTypeID, MarginPercent, LumberAdder) OUTPUT INSERTED.SettingID VALUES (@ProjectID, @ProductTypeID, @MarginPercent, @LumberAdder)"
        Public Const UpdateProjectProductSetting As String = "UPDATE ProjectProductSettings SET MarginPercent = @MarginPercent, LumberAdder = @LumberAdder WHERE SettingID = @SettingID"

        ' Customers
        Public Const SelectCustomers As String = "SELECT * FROM Customer"
        Public Const InsertCustomer As String = "INSERT INTO Customer (CustomerName) OUTPUT INSERTED.CustomerID VALUES (@CustomerName)"
        Public Const UpdateCustomer As String = "UPDATE Customer SET CustomerName = @CustomerName WHERE CustomerID = @CustomerID"
        Public Const SelectCustomersByProject As String = "SELECT c.* FROM Customer c JOIN CustomertoProjectMapping m ON c.CustomerID = m.CustomerID WHERE m.ProjectID = @ProjectID"
        Public Const InsertCustomerToProject As String = "INSERT INTO CustomertoProjectMapping (ProjectID, CustomerID) VALUES (@ProjectID, @CustomerID)"
        Public Const DeleteCustomerToProject As String = "DELETE FROM CustomertoProjectMapping WHERE ProjectID = @ProjectID AND CustomerID = @CustomerID"

        ' Sales
        Public Const SelectSales As String = "SELECT * FROM Sales"
        Public Const InsertSales As String = "INSERT INTO Sales (SalesName) OUTPUT INSERTED.SalesID VALUES (@SalesName)"
        Public Const UpdateSales As String = "UPDATE Sales SET SalesName = @SalesName WHERE SalesID = @SalesID"
        Public Const SelectSalesByProject As String = "SELECT s.* FROM Sales s JOIN SalestoProjectMapping m ON s.SalesID = m.SalesID WHERE m.ProjectID = @ProjectID"
        Public Const InsertSalesToProject As String = "INSERT INTO SalestoProjectMapping (ProjectID, SalesID) VALUES (@ProjectID, @SalesID)"
        Public Const DeleteSalesToProject As String = "DELETE FROM SalestoProjectMapping WHERE ProjectID = @ProjectID AND SalesID = @SalesID"

        ' RawUnits
        Public Const SelectRawUnitsByProject As String = "SELECT * FROM RawUnits WHERE ProjectID = @ProjectID"
        Public Const InsertRawUnit As String = "INSERT INTO RawUnits (RawUnitName, ProjectID, ProductTypeID, BF, LF, EWPLF, SqFt, FCArea, LumberCost, PlateCost, ManufLaborCost, DesignLabor, MGMTLabor, JobSuppliesCost, ManHours, ItemCost, OverallCost, DeliveryCost, TotalSellPrice, AvgSPFNo2) OUTPUT INSERTED.RawUnitID VALUES (@RawUnitName, @ProjectID, @ProductTypeID, @BF, @LF, @EWPLF, @SqFt, @FCArea, @LumberCost, @PlateCost, @ManufLaborCost, @DesignLabor, @MGMTLabor, @JobSuppliesCost, @ManHours, @ItemCost, @OverallCost, @DeliveryCost, @TotalSellPrice, @AvgSPFNo2)"
        Public Const SelectRawUnitByID As String = "SELECT * FROM RawUnits WHERE RawUnitID = @RawUnitID"

        ' ActualUnits
        Public Const InsertActualUnit As String = "INSERT INTO ActualUnits (ProjectID, RawUnitID, ProductTypeID, UnitName, PlanSQFT, UnitType, OptionalAdder) OUTPUT INSERTED.ActualUnitID VALUES (@ProjectID, @RawUnitID, @ProductTypeID, @UnitName, @PlanSQFT, @UnitType, @OptionalAdder)"
        Public Const UpdateActualUnit As String = "UPDATE ActualUnits SET UnitName = @UnitName, PlanSQFT = @PlanSQFT, UnitType = @UnitType, OptionalAdder = @OptionalAdder WHERE ActualUnitID = @ActualUnitID"
        Public Const SelectActualUnitsByProject As String = "SELECT au.*, ru.RawUnitName FROM ActualUnits au JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID WHERE au.ProjectID = @ProjectID"
        Public Const DeleteActualUnit As String = "DELETE FROM ActualUnits WHERE ActualUnitID = @ActualUnitID"
        Public Const SelectActualUnitIDsByLevelID As String = "SELECT DISTINCT ActualUnitID FROM ActualToLevelMapping WHERE LevelID = @LevelID"
        Public Const CountMappingsByActualUnitID As String = "SELECT COUNT(*) FROM ActualToLevelMapping WHERE ActualUnitID = @ActualUnitID"
        Public Const SelectActualUnitByID As String = "SELECT au.*, ru.RawUnitName AS ReferencedRawUnitName FROM ActualUnits au JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID WHERE au.ActualUnitID = @ActualUnitID"

        ' ActualToLevelMapping
        Public Const InsertActualToLevelMapping As String = "INSERT INTO ActualToLevelMapping (ProjectID, ActualUnitID, LevelID, Quantity) OUTPUT INSERTED.MappingID VALUES (@ProjectID, @ActualUnitID, @LevelID, @Quantity)"
        Public Const UpdateActualToLevelMapping As String = "UPDATE ActualToLevelMapping SET Quantity = @Quantity WHERE MappingID = @MappingID"
        Public Const DeleteActualToLevelMappingByLevelID As String = "DELETE FROM ActualToLevelMapping WHERE LevelID = @LevelID"
        Public Const DeleteActualToLevelMappingByMappingID As String = "DELETE FROM ActualToLevelMapping WHERE MappingID = @MappingID"
        Public Const SelectActualToLevelMappingsByLevelID As String = "SELECT alm.*, au.*, ru.RawUnitName AS ReferencedRawUnitName FROM ActualToLevelMapping alm JOIN ActualUnits au ON alm.ActualUnitID = au.ActualUnitID JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID WHERE alm.LevelID = @LevelID"

        ' CalculatedComponents
        Public Const DeleteCalculatedComponentsByActualUnitID As String = "DELETE FROM CalculatedComponents WHERE ActualUnitID = @ActualUnitID"
        Public Const InsertCalculatedComponent As String = "INSERT INTO CalculatedComponents (ProjectID, ActualUnitID, ComponentType, Value) VALUES (@ProjectID, @ActualUnitID, @ComponentType, @Value)"
        Public Const SelectCalculatedComponentsByActualUnitID As String = "SELECT * FROM CalculatedComponents WHERE ActualUnitID = @ActualUnitID"

        ' ProjectType
        Public Const SelectProjectTypes As String = "SELECT * FROM ProjectType ORDER BY ProjectTypeName"
        Public Const InsertProjectType As String = "INSERT INTO ProjectType (ProjectTypeName, Description) OUTPUT INSERTED.ProjectTypeID VALUES (@ProjectTypeName, @Description)"
        Public Const UpdateProjectType As String = "UPDATE ProjectType SET ProjectTypeName = @ProjectTypeName, Description = @Description WHERE ProjectTypeID = @ProjectTypeID"

        ' Estimator
        Public Const SelectEstimators As String = "SELECT * FROM Estimator ORDER BY EstimatorName"
        Public Const InsertEstimator As String = "INSERT INTO Estimator (EstimatorName) OUTPUT INSERTED.EstimatorID VALUES (@EstimatorName)"
        Public Const UpdateEstimator As String = "UPDATE Estimator SET EstimatorName = @EstimatorName WHERE EstimatorID = @EstimatorID"

        ' Configuration
        Public Const SelectConfigValue As String = "SELECT Value FROM Configuration WHERE ConfigKey = @ConfigKey"

        ' Additional Queries for Rollups and Utilities
        Public Const SelectMilesToJobSite As String = "SELECT MilesToJobSite FROM Projects WHERE ProjectID = @ProjectID"
        Public Const SelectLumberAdder As String = "SELECT LumberAdder FROM ProjectProductSettings WHERE ProjectID = @ProjectID AND ProductTypeID = @ProductTypeID"
        Public Const SelectMarginPercent As String = "SELECT MarginPercent FROM ProjectProductSettings WHERE ProjectID = @ProjectID AND ProductTypeID = @ProductTypeID"

        ' Rollup Calculations for Levels
        Public Const CalculateOverallSQFT As String = "SELECT SUM(au.PlanSQFT * alm.Quantity) AS OverallSQFT FROM ActualUnits au JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE alm.LevelID = @LevelID"
        Public Const CalculateOverallLF As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS OverallLF FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'LF/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateOverallBDFT As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS OverallBDFT FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'BDFT/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateLumberCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) + COALESCE((l.OverallBDFT / 1000) * pps.LumberAdder, 0) AS LumberCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID JOIN Levels l ON alm.LevelID = l.LevelID JOIN ProjectProductSettings pps ON l.ProjectID = pps.ProjectID AND au.ProductTypeID = pps.ProductTypeID WHERE cc.ComponentType = 'Lumber/SQFT' AND l.LevelID = @LevelID GROUP BY l.OverallBDFT, pps.LumberAdder"
        Public Const CalculatePlateCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS PlateCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'Plate/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateLaborCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS LaborCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'ManufLabor/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateLaborMH As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS LaborMH FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'ManHours/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateDesignCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS DesignCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'DesignLabor/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateMGMTCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS MGMTCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'MGMTLabor/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateJobSuppliesCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS JobSuppliesCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'JobSupplies/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateItemsCost As String = "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS ItemsCost FROM CalculatedComponents cc JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID WHERE cc.ComponentType = 'ItemCost/SQFT' AND alm.LevelID = @LevelID"
        Public Const CalculateDeliveryCost As String = "SELECT CASE WHEN l.OverallBDFT < 1 THEN 0 ELSE CASE WHEN ROUND(CEILING(l.OverallBDFT / 10000.0) * c.Value * p.MilesToJobSite, 0) < 150 THEN 150 ELSE ROUND(CEILING(l.OverallBDFT / 10000.0) * c.Value * p.MilesToJobSite, 0) END END AS DeliveryCost FROM Levels l JOIN Projects p ON l.ProjectID = p.ProjectID JOIN Configuration c ON c.ConfigKey = 'MileageRate' WHERE l.LevelID = @LevelID"
        Public Const CalculateOverallCost As String = "SELECT SUM(l.LumberCost + l.PlateCost + l.LaborCost + l.DesignCost + l.MGMTCost + l.JobSuppliesCost + l.ItemsCost) AS OverallCost FROM Levels l WHERE l.LevelID = @LevelID GROUP BY l.LevelID"
        Public Const UpdateLevelRollupsSql As String = "UPDATE Levels SET OverallSQFT = @OverallSQFT, OverallLF = @OverallLF, OverallBDFT = @OverallBDFT, LumberCost = @LumberCost, PlateCost = @PlateCost, LaborCost = @LaborCost, LaborMH = @LaborMH, DesignCost = @DesignCost, MGMTCost = @MGMTCost, JobSuppliesCost = @JobSuppliesCost, ItemsCost = @ItemsCost, DeliveryCost = @DeliveryCost, OverallCost = @OverallCost, OverallPrice = @OverallPrice, TotalSQFT = @TotalSQFT, AvgPricePerSQFT = @AvgPricePerSQFT WHERE LevelID = @LevelID"

        ' Rollup Calculations for Buildings
        Public Const SelectBldgQty As String = "SELECT BldgQty FROM Buildings WHERE BuildingID = @BuildingID"
        Public Const CalculateFloorCostPerBldg As String = "SELECT SUM(l.OverallCost) AS FloorCostPerBldg FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"
        Public Const CalculateFloorDeliveryCost As String = "SELECT SUM(l.DeliveryCost) AS FloorDeliveryCost FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"
        Public Const CalculateRoofCostPerBldg As String = "SELECT SUM(l.OverallCost) AS RoofCostPerBldg FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"
        Public Const CalculateRoofDeliveryCost As String = "SELECT SUM(l.DeliveryCost) AS RoofDeliveryCost FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"
        Public Const UpdateBuildingRollupsSql As String = "UPDATE Buildings SET FloorCostPerBldg = @FloorCostPerBldg, RoofCostPerBldg = @RoofCostPerBldg, WallCostPerBldg = @WallCostPerBldg, ExtendedFloorCost = @ExtendedFloorCost, ExtendedRoofCost = @ExtendedRoofCost, ExtendedWallCost = @ExtendedWallCost, OverallPrice = @OverallPrice, OverallCost = @OverallCost WHERE BuildingID = @BuildingID"
    End Module
End Namespace