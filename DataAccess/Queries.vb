Namespace BuildersPSE.DataAccess
    Public Module Queries
        ' Projects

        Public Const SelectProjects As String = "SELECT p.*, pt.ProjectTypeName AS ProjectType, e.EstimatorName AS Estimator FROM Projects p LEFT JOIN ProjectType pt ON p.ProjectTypeID = pt.ProjectTypeID LEFT JOIN Estimator e ON p.EstimatorID = e.EstimatorID ORDER BY BidDate DESC"  ' Fixed typo: p., -> p.*
        Public Const InsertProject As String = "INSERT INTO Projects (JBID, ProjectTypeID, ProjectName, EstimatorID, Address, City, State, Zip, BidDate, ArchPlansDated, EngPlansDated, MilesToJobSite, TotalNetSqft, TotalGrossSqft, ProjectArchitect, ProjectEngineer, ProjectNotes, LastModifiedDate) OUTPUT INSERTED.ProjectID VALUES (@JBID, @ProjectTypeID, @ProjectName, @EstimatorID, @Address, @City, @State, @Zip, @BidDate, @ArchPlansDated, @EngPlansDated, @MilesToJobSite, @TotalNetSqft, @TotalGrossSqft, @ProjectArchitect, @ProjectEngineer, @ProjectNotes, GetDate())"
        Public Const UpdateProject As String = "UPDATE Projects SET JBID = @JBID, ProjectTypeID = @ProjectTypeID, ProjectName = @ProjectName, EstimatorID = @EstimatorID, Address = @Address, City = @City, State = @State, Zip = @Zip, BidDate = @BidDate, ArchPlansDated = @ArchPlansDated, EngPlansDated = @EngPlansDated, MilesToJobSite = @MilesToJobSite, TotalNetSqft = @TotalNetSqft, TotalGrossSqft = @TotalGrossSqft, ProjectArchitect = @ProjectArchitect, ProjectEngineer = @ProjectEngineer, ProjectNotes = @ProjectNotes, LastModifiedDate=GetDate() WHERE ProjectID = @ProjectID"
        Public Const SelectProjectList As String = "SELECT p.*, pt.ProjectTypeName AS ProjectType, e.EstimatorName AS Estimator FROM Projects p LEFT JOIN ProjectType pt ON p.ProjectTypeID = pt.ProjectTypeID LEFT JOIN Estimator e ON p.EstimatorID = e.EstimatorID ORDER BY BidDate DESC"  ' Fixed typo: p., -> p.*
        Public Const SelectProjectByID As String = "SELECT p.*, pt.ProjectTypeName AS ProjectType, e.EstimatorName AS Estimator FROM Projects p LEFT JOIN ProjectType pt ON p.ProjectTypeID = pt.ProjectTypeID LEFT JOIN Estimator e ON p.EstimatorID = e.EstimatorID WHERE ProjectID = @ProjectID"

        ' Buildings

        Public Const SelectBuildingsByProject As String = "SELECT * FROM Buildings WHERE ProjectID = @ProjectID"
        Public Const InsertBuilding As String = "INSERT INTO Buildings (BuildingName, BuildingType, ResUnits, BldgQty, ProjectID, LastModifiedDate) OUTPUT INSERTED.BuildingID VALUES (@BuildingName, @BuildingType, @ResUnits, @BldgQty, @ProjectID, GetDate())"
        Public Const UpdateBuilding As String = "UPDATE Buildings SET BuildingName = @BuildingName, BuildingType = @BuildingType, ResUnits = @ResUnits, BldgQty = @BldgQty, LastModifiedDate = GetDate() WHERE BuildingID = @BuildingID"
        Public Const DeleteBuilding As String = "DELETE FROM Buildings WHERE BuildingID = @BuildingID"
        Public Const GetBuildingIDByLevelID As String = "SELECT BuildingID FROM Levels WHERE LevelID = @LevelID"

        ' Levels

        Public Const SelectLevelsByBuilding As String = "SELECT l.*, pt.ProductTypeName FROM Levels l LEFT JOIN ProductType pt ON l.ProductTypeID = pt.ProductTypeID WHERE l.BuildingID = @BuildingID"
        Public Const InsertLevel As String = "INSERT INTO Levels (ProjectID, BuildingID, ProductTypeID, LevelNumber, LevelName, LastModifiedDate, LastModifiedDate) OUTPUT INSERTED.LevelID VALUES (@ProjectID, @BuildingID, @ProductTypeID, @LevelNumber, @LevelName,  GetDate())"
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

        ' Sales
        Public Const SelectSales As String = "SELECT * FROM Sales"
        Public Const InsertSales As String = "INSERT INTO Sales (SalesName) OUTPUT INSERTED.SalesID VALUES (@SalesName)"
        Public Const UpdateSales As String = "UPDATE Sales SET SalesName = @SalesName WHERE SalesID = @SalesID"

        ' Customer to Project Mapping
        Public Const SelectCustomersByProject As String = "SELECT c.* FROM Customer c JOIN CustomertoProjectMapping m ON c.CustomerID = m.CustomerID WHERE m.ProjectID = @ProjectID"
        Public Const InsertCustomerToProject As String = "INSERT INTO CustomertoProjectMapping (ProjectID, CustomerID) VALUES (@ProjectID, @CustomerID)"
        Public Const DeleteCustomerToProject As String = "DELETE FROM CustomertoProjectMapping WHERE ProjectID = @ProjectID AND CustomerID = @CustomerID"

        ' Sales to Project Mapping
        Public Const SelectSalesByProject As String = "SELECT s.* FROM Sales s JOIN SalestoProjectMapping m ON s.SalesID = m.SalesID WHERE m.ProjectID = @ProjectID"
        Public Const InsertSalesToProject As String = "INSERT INTO SalestoProjectMapping (ProjectID, SalesID) VALUES (@ProjectID, @SalesID)"
        Public Const DeleteSalesToProject As String = "DELETE FROM SalestoProjectMapping WHERE ProjectID = @ProjectID AND SalesID = @SalesID"

        ' RawUnits
        Public Const SelectRawUnitsByProject As String = "SELECT * FROM RawUnits WHERE ProjectID = @ProjectID"
        Public Const InsertRawUnit As String = "INSERT INTO RawUnits (RawUnitName, ProjectID, ProductTypeID, BF, LF, EWPLF, SqFt, FCArea, LumberCost, PlateCost, ManufLaborCost, DesignLabor, MGMTLabor, JobSuppliesCost, ManHours, ItemCost, OverallCost, DeliveryCost, TotalSellPrice, AvgSPFNo2) OUTPUT INSERTED.RawUnitID VALUES (@RawUnitName, @ProjectID, @ProductTypeID, @BF, @LF, @EWPLF, @SqFt, @FCArea, @LumberCost, @PlateCost, @ManufLaborCost, @DesignLabor, @MGMTLabor, @JobSuppliesCost, @ManHours, @ItemCost, @OverallCost, @DeliveryCost, @TotalSellPrice, @AvgSPFNo2)"
        Public Const SelectRawUnitByID = "SELECT * FROM RawUnits WHERE RawUnitID = @RawUnitID"

        ' ActualUnits
        Public Const InsertActualUnit As String = "INSERT INTO ActualUnits (ProjectID, RawUnitID, ProductTypeID, UnitName, PlanSQFT, UnitType, OptionalAdder) OUTPUT INSERTED.ActualUnitID VALUES (@ProjectID, @RawUnitID, @ProductTypeID, @UnitName, @PlanSQFT, @UnitType, @OptionalAdder)"
        Public Const UpdateActualUnit As String = "UPDATE ActualUnits SET UnitName = @UnitName, PlanSQFT = @PlanSQFT, UnitType = @UnitType, OptionalAdder = @OptionalAdder WHERE ActualUnitID = @ActualUnitID"
        Public Const SelectActualUnitsByProject As String = "SELECT au.*, ru.RawUnitName FROM ActualUnits au JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID WHERE au.ProjectID = @ProjectID"
        Public Const DeleteActualUnit As String = "DELETE FROM ActualUnits WHERE ActualUnitID = @ActualUnitID"
        Public Const SelectActualUnitIDsByLevelID As String = "SELECT DISTINCT ActualUnitID FROM ActualToLevelMapping WHERE LevelID = @LevelID"
        Public Const CountMappingsByActualUnitID As String = "SELECT COUNT(*) FROM ActualToLevelMapping WHERE ActualUnitID = @ActualUnitID"

        ' ActualToLevelMapping
        Public Const InsertActualToLevelMapping As String = "INSERT INTO ActualToLevelMapping (ProjectID, ActualUnitID, LevelID, Quantity) OUTPUT INSERTED.MappingID VALUES (@ProjectID, @ActualUnitID, @LevelID, @Quantity)"
        Public Const UpdateActualToLevelMapping As String = "UPDATE ActualToLevelMapping SET Quantity = @Quantity WHERE MappingID = @MappingID"
        Public Const DeleteActualToLevelMappingByLevelID As String = "DELETE FROM ActualToLevelMapping WHERE LevelID = @LevelID"
        Public Const DeleteActualToLevelMappingByMappingID As String = "DELETE FROM ActualToLevelMapping WHERE MappingID = @MappingID"

        ' CalculatedComponents
        Public Const DeleteCalculatedComponentsByActualUnitID As String = "DELETE FROM CalculatedComponents WHERE ActualUnitID = @ActualUnitID"
        Public Const InsertCalculatedComponent As String = "INSERT INTO CalculatedComponents (ProjectID, ActualUnitID, ComponentType, Value) VALUES (@ProjectID, @ActualUnitID, @ComponentType, @Value)"

        'ProjectType
        Public Const SelectProjectTypes As String = "SELECT * FROM ProjectType ORDER BY ProjectTypeName"
        Public Const InsertProjectType As String = "INSERT INTO ProjectType (ProjectTypeName, Description) OUTPUT INSERTED.ProjectTypeID VALUES (@ProjectTypeName, @Description)"
        Public Const UpdateProjectType As String = "UPDATE ProjectType SET ProjectTypeName = @ProjectTypeName, Description = @Description WHERE ProjectTypeID = @ProjectTypeID"

        ' Estimator
        Public Const SelectEstimators As String = "SELECT * FROM Estimator ORDER BY EstimatorName"
        Public Const InsertEstimator As String = "INSERT INTO Estimator (EstimatorName) OUTPUT INSERTED.EstimatorID VALUES (@EstimatorName)"
        Public Const UpdateEstimator As String = "UPDATE Estimator SET EstimatorName = @EstimatorName WHERE EstimatorID = @EstimatorID"


        ' Update CalculateLumberCost for LumberAdder logic
        Public Const CalculateLumberCost As String = "
            SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) + 
            COALESCE((l.OverallBDFT / 1000) * pps.LumberAdder, 0) AS LumberCost
            FROM CalculatedComponents cc
            JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID
            JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID
            JOIN Levels l ON alm.LevelID = l.LevelID
            JOIN ProjectProductSettings pps ON l.ProjectID = pps.ProjectID AND au.ProductTypeID = pps.ProductTypeID
            WHERE cc.ComponentType = 'Lumber/SQFT' AND l.LevelID = @LevelID
            GROUP BY l.OverallBDFT, pps.LumberAdder"

        ' Similar for other calcs like DeliveryCost
        Public Const CalculateDeliveryCost As String = "
            SELECT CASE WHEN l.OverallBDFT < 1 THEN 0
            ELSE CASE WHEN ROUND(CEILING(l.OverallBDFT / 10000.0) * c.Value * p.MilesToJobSite, 0) < 150 THEN 150
            ELSE ROUND(CEILING(l.OverallBDFT / 10000.0) * c.Value * p.MilesToJobSite, 0) END END AS DeliveryCost
            FROM Levels l
            JOIN Projects p ON l.ProjectID = p.ProjectID
            JOIN Configuration c ON c.ConfigKey = 'MileageRate'
            WHERE l.LevelID = @LevelID"

        ' Add more as needed for OverallPrice, etc.
        ' ActualToLevelMapping
        Public Const SelectActualToLevelMappingsByLevelID As String = "
            SELECT alm.*, au.*, ru.RawUnitName AS ReferencedRawUnitName
            FROM ActualToLevelMapping alm
            JOIN ActualUnits au ON alm.ActualUnitID = au.ActualUnitID
            JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID
            WHERE alm.LevelID = @LevelID"

        ' ActualUnits (for single fetch)
        Public Const SelectActualUnitByID As String = "
            SELECT au.*, ru.RawUnitName AS ReferencedRawUnitName
            FROM ActualUnits au
            JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID
            WHERE au.ActualUnitID = @ActualUnitID"


    End Module
End Namespace