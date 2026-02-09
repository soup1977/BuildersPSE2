Option Strict On

Namespace DataAccess
    ''' <summary>
    ''' Centralized SQL query constants for all database operations.
    ''' Organized by entity/feature area with XML documentation.
    ''' </summary>
    ''' <remarks>
    ''' All queries use parameterized values (@ParameterName) for SQL injection prevention.
    ''' Queries marked "updated to use VersionID" support multi-version project architecture.
    ''' </remarks>
    Public Module Queries

#Region "Projects"

        ''' <summary>Selects all projects with related type, estimator, architect, and engineer names.</summary>
        Public Const SelectProjects As String =
            "SELECT p.*, pt.ProjectTypeName AS ProjectType, e.EstimatorName AS Estimator, 
             ca.CustomerName AS ArchitectName, ce.CustomerName AS EngineerName 
             FROM Projects p 
             LEFT JOIN ProjectType pt ON p.ProjectTypeID = pt.ProjectTypeID 
             LEFT JOIN Estimator e ON p.EstimatorID = e.EstimatorID 
             LEFT JOIN Customer ca ON p.ArchitectID = ca.CustomerID AND ca.CustomerType = 2 
             LEFT JOIN Customer ce ON p.EngineerID = ce.CustomerID AND ce.CustomerType = 3 
             ORDER BY BidDate DESC"

        ''' <summary>Selects a single project by ID with related names.</summary>
        Public Const SelectProjectByID As String =
            "SELECT p.*, pt.ProjectTypeName AS ProjectType, e.EstimatorName AS Estimator, 
             ca.CustomerName AS ArchitectName, ce.CustomerName AS EngineerName 
             FROM Projects p 
             LEFT JOIN ProjectType pt ON p.ProjectTypeID = pt.ProjectTypeID 
             LEFT JOIN Estimator e ON p.EstimatorID = e.EstimatorID 
             LEFT JOIN Customer ca ON p.ArchitectID = ca.CustomerID AND ca.CustomerType = 2 
             LEFT JOIN Customer ce ON p.EngineerID = ce.CustomerID AND ce.CustomerType = 3 
             WHERE p.ProjectID = @ProjectID"

        ''' <summary>Inserts a new project. Returns the new ProjectID.</summary>
        Public Const InsertProject As String =
            "INSERT INTO Projects (JBID, ProjectTypeID, ProjectName, EstimatorID, Address, City, State, Zip, 
             BidDate, ArchPlansDated, EngPlansDated, MilesToJobSite, TotalNetSqft, TotalGrossSqft, 
             ArchitectID, EngineerID, ProjectNotes, LastModifiedDate, CreatedDate) 
             OUTPUT INSERTED.ProjectID 
             VALUES (@JBID, @ProjectTypeID, @ProjectName, @EstimatorID, @Address, @City, @State, @Zip, 
             @BidDate, @ArchPlansDated, @EngPlansDated, @MilesToJobSite, @TotalNetSqft, @TotalGrossSqft, 
             @ArchitectID, @EngineerID, @ProjectNotes, GetDate(), GetDate())"

        ''' <summary>Updates an existing project by ProjectID.</summary>
        Public Const UpdateProject As String =
            "UPDATE Projects SET JBID = @JBID, ProjectTypeID = @ProjectTypeID, ProjectName = @ProjectName, 
             EstimatorID = @EstimatorID, Address = @Address, City = @City, State = @State, Zip = @Zip, 
             BidDate = @BidDate, ArchPlansDated = @ArchPlansDated, EngPlansDated = @EngPlansDated, 
             MilesToJobSite = @MilesToJobSite, TotalNetSqft = @TotalNetSqft, TotalGrossSqft = @TotalGrossSqft, 
             ArchitectID = @ArchitectID, EngineerID = @EngineerID, ProjectNotes = @ProjectNotes, 
             LastModifiedDate = GetDate() 
             WHERE ProjectID = @ProjectID"

        ''' <summary>Gets MilesToJobSite for delivery cost calculations.</summary>
        Public Const SelectMilesToJobSite As String =
            "SELECT MilesToJobSite FROM Projects WHERE ProjectID = @ProjectID"

#End Region

#Region "Project Versions"

        ''' <summary>Selects all versions for a project with customer and sales info.</summary>
        Public Const SelectProjectVersions As String =
            "SELECT pv.*, c.CustomerName, s.SalesName 
             FROM ProjectVersions pv 
             LEFT JOIN Customer c ON pv.CustomerID = c.CustomerID AND c.CustomerType = 1 
             LEFT JOIN Sales s ON pv.SalesID = s.SalesID 
             WHERE ProjectID = @ProjectID 
             ORDER BY VersionDate DESC"

        ''' <summary>Selects all project versions across all projects with customer and sales info.</summary>
        Public Const SelectAllProjectVersions As String =
            "SELECT pv.*, c.CustomerName, s.SalesName 
             FROM ProjectVersions pv 
             LEFT JOIN Customer c ON pv.CustomerID = c.CustomerID AND c.CustomerType = 1 
             LEFT JOIN Sales s ON pv.SalesID = s.SalesID 
             ORDER BY pv.ProjectID, pv.VersionDate DESC"

        ''' <summary>Inserts a new project version. Returns the new VersionID.</summary>
        Public Const InsertProjectVersion As String =
            "INSERT INTO ProjectVersions (ProjectID, VersionName, VersionDate, Description, LastModifiedDate, 
             CustomerID, SalesID, MondayID, ProjVersionStatusID, FuturesAdderAmt, FuturesAdderProjTotal) 
             OUTPUT INSERTED.VersionID 
             VALUES (@ProjectID, @VersionName, @VersionDate, @Description, GetDate(), 
             @CustomerID, @SalesID, @MondayID, @ProjVersionStatusID, @FuturesAdderAmt, @FuturesAdderProjTotal)"

        ''' <summary>Updates an existing project version.</summary>
        Public Const UpdateProjectVersion As String =
            "UPDATE ProjectVersions SET VersionName = @VersionName, LastModifiedDate = GetDate(), 
             CustomerID = @CustomerID, SalesID = @SalesID, ProjVersionStatusID = @ProjVersionStatusID, 
             MondayID = @MondayID, FuturesAdderAmt = @FuturesAdderAmt, FuturesAdderProjTotal = @FuturesAdderProjTotal 
             WHERE VersionID = @VersionID"

        ''' <summary>Selects all project version statuses for dropdowns.</summary>
        Public Const SelectProjectStatus As String =
            "SELECT * FROM ProjVersionStatus ORDER BY ProjVersionStatusID"

#End Region

#Region "Buildings"

        ''' <summary>Selects all buildings for a version.</summary>
        Public Const SelectBuildingsByVersionID As String =
            "SELECT * FROM Buildings WHERE VersionID = @VersionID"

        ''' <summary>Inserts a new building. Returns the new BuildingID.</summary>
        Public Const InsertBuilding As String =
            "INSERT INTO Buildings (BuildingName, BuildingType, ResUnits, BldgQty, VersionID, LastModifiedDate) 
             OUTPUT INSERTED.BuildingID 
             VALUES (@BuildingName, @BuildingType, @ResUnits, @BldgQty, @VersionID, GetDate())"

        ''' <summary>Updates an existing building.</summary>
        Public Const UpdateBuilding As String =
            "UPDATE Buildings SET BuildingName = @BuildingName, BuildingType = @BuildingType, 
             ResUnits = @ResUnits, BldgQty = @BldgQty, LastModifiedDate = GetDate() 
             WHERE BuildingID = @BuildingID"

        ''' <summary>Deletes a building by ID.</summary>
        Public Const DeleteBuilding As String =
            "DELETE FROM Buildings WHERE BuildingID = @BuildingID"

        ''' <summary>Gets building quantity for rollup calculations.</summary>
        Public Const SelectBldgQty As String =
            "SELECT BldgQty FROM Buildings WHERE BuildingID = @BuildingID"

#End Region

#Region "Levels"

        ''' <summary>Selects all levels for a building with product type name.</summary>
        Public Const SelectLevelsByBuilding As String =
            "SELECT l.*, pt.ProductTypeName 
             FROM Levels l 
             LEFT JOIN ProductType pt ON l.ProductTypeID = pt.ProductTypeID 
             WHERE l.BuildingID = @BuildingID"

        ''' <summary>Selects all levels for a version (used for duplication).</summary>
        Public Const SelectLevelsByVersionID As String =
            "SELECT LevelID, BuildingID, ProductTypeID, LevelNumber, LevelName 
             FROM Levels WHERE VersionID = @VersionID"

        ''' <summary>Selects a single level by ID.</summary>
        Public Const SelectLevelByID As String =
            "SELECT * FROM Levels WHERE LevelID = @LevelID"

        ''' <summary>Gets the BuildingID for a given LevelID.</summary>
        Public Const GetBuildingIDByLevelID As String =
            "SELECT BuildingID FROM Levels WHERE LevelID = @LevelID"

        ''' <summary>Inserts a new level. Returns the new LevelID.</summary>
        Public Const InsertLevel As String =
            "INSERT INTO Levels (VersionID, BuildingID, ProductTypeID, LevelNumber, LevelName, LastModifiedDate) 
             OUTPUT INSERTED.LevelID 
             VALUES (@VersionID, @BuildingID, @ProductTypeID, @LevelNumber, @LevelName, GetDate())"

        ''' <summary>Updates an existing level.</summary>
        Public Const UpdateLevel As String =
            "UPDATE Levels SET ProductTypeID = @ProductTypeID, LevelNumber = @LevelNumber, 
             LevelName = @LevelName, LastModifiedDate = GetDate() 
             WHERE LevelID = @LevelID"

        ''' <summary>Deletes a level by ID.</summary>
        Public Const DeleteLevel As String =
            "DELETE FROM Levels WHERE LevelID = @LevelID"

        ''' <summary>Selects level estimate details with product type for reporting.</summary>
        Public Const SelectLevelEstimateWithProductType As String =
            "SELECT l.OverallPrice, l.OverallBDFT, l.LumberCost, l.PlateCost, l.LaborCost, l.LaborMH,
             l.ItemsCost, l.DeliveryCost, l.DesignCost, l.MGMTCost, l.JobSuppliesCost, l.OverallCost,
             pt.ProductTypeName
             FROM Levels l
             JOIN ActualToLevelMapping alm ON l.LevelID = alm.LevelID
             JOIN ActualUnits au ON alm.ActualUnitID = au.ActualUnitID
             JOIN ProductType pt ON au.ProductTypeID = pt.ProductTypeID
             WHERE l.LevelID = @LevelID AND l.VersionID = @VersionID
             GROUP BY l.OverallPrice, l.OverallBDFT, l.LumberCost, l.PlateCost, l.LaborCost, l.LaborMH,
             l.ItemsCost, l.DeliveryCost, l.DesignCost, l.MGMTCost, l.JobSuppliesCost, l.OverallCost, 
             pt.ProductTypeName"

#End Region

#Region "Raw Units"

        ''' <summary>Selects all raw units for a version.</summary>
        Public Const SelectRawUnitsByVersion As String =
            "SELECT * FROM RawUnits WHERE VersionID = @VersionID"

        ''' <summary>Selects a single raw unit by ID.</summary>
        Public Const SelectRawUnitByID As String =
            "SELECT * FROM RawUnits WHERE RawUnitID = @RawUnitID"

        ''' <summary>Inserts a new raw unit. Returns the new RawUnitID.</summary>
        Public Const InsertRawUnit As String =
            "INSERT INTO RawUnits (RawUnitName, VersionID, ProductTypeID, BF, LF, EWPLF, SqFt, FCArea, 
             LumberCost, PlateCost, ManufLaborCost, DesignLabor, MGMTLabor, JobSuppliesCost, ManHours, 
             ItemCost, OverallCost, DeliveryCost, TotalSellPrice, AvgSPFNo2, SPFNo2BDFT, Avg241800, 
             MSR241800BDFT, Avg242400, MSR242400BDFT, Avg261800, MSR261800BDFT, Avg262400, MSR262400BDFT) 
             OUTPUT INSERTED.RawUnitID 
             VALUES (@RawUnitName, @VersionID, @ProductTypeID, @BF, @LF, @EWPLF, @SqFt, @FCArea, 
             @LumberCost, @PlateCost, @ManufLaborCost, @DesignLabor, @MGMTLabor, @JobSuppliesCost, @ManHours, 
             @ItemCost, @OverallCost, @DeliveryCost, @TotalSellPrice, @AvgSPFNo2, @SPFNo2BDFT, @Avg241800, 
             @MSR241800BDFT, @Avg242400, @MSR242400BDFT, @Avg261800, @MSR261800BDFT, @Avg262400, @MSR262400BDFT)"

        ''' <summary>Updates an existing raw unit.</summary>
        Public Const UpdateRawUnit As String =
            "UPDATE RawUnits SET RawUnitName = @RawUnitName, BF = @BF, LF = @LF, EWPLF = @EWPLF, 
             SqFt = @SqFt, FCArea = @FCArea, LumberCost = @LumberCost, PlateCost = @PlateCost, 
             ManufLaborCost = @ManufLaborCost, DesignLabor = @DesignLabor, MGMTLabor = @MGMTLabor, 
             JobSuppliesCost = @JobSuppliesCost, ManHours = @ManHours, ItemCost = @ItemCost, 
             OverallCost = @OverallCost, DeliveryCost = @DeliveryCost, TotalSellPrice = @TotalSellPrice, 
             AvgSPFNo2 = @AvgSPFNo2, SPFNo2BDFT = @SPFNo2BDFT, Avg241800 = @Avg241800, 
             MSR241800BDFT = @MSR241800BDFT, Avg242400 = @Avg242400, MSR242400BDFT = @MSR242400BDFT, 
             Avg261800 = @Avg261800, MSR261800BDFT = @MSR261800BDFT, Avg262400 = @Avg262400, 
             MSR262400BDFT = @MSR262400BDFT 
             WHERE RawUnitID = @RawUnitID"

#End Region

#Region "Actual Units"

        ''' <summary>Selects all actual units for a version with raw unit names.</summary>
        Public Const SelectActualUnitsByVersion As String =
            "SELECT au.*, ru.RawUnitName 
             FROM ActualUnits au 
             JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID 
             WHERE au.VersionID = @VersionID 
             ORDER BY au.SortOrder, au.ActualUnitID"

        ''' <summary>Selects a single actual unit by ID with raw unit name.</summary>
        Public Const SelectActualUnitByID As String =
            "SELECT au.*, ru.RawUnitName AS ReferencedRawUnitName 
             FROM ActualUnits au 
             JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID 
             WHERE au.ActualUnitID = @ActualUnitID"

        ''' <summary>Selects distinct ActualUnitIDs mapped to a level.</summary>
        Public Const SelectActualUnitIDsByLevelID As String =
            "SELECT DISTINCT ActualUnitID FROM ActualToLevelMapping WHERE LevelID = @LevelID"

        ''' <summary>Inserts a new actual unit. Returns the new ActualUnitID.</summary>
        Public Const InsertActualUnit As String =
            "INSERT INTO ActualUnits (VersionID, RawUnitID, ProductTypeID, UnitName, PlanSQFT, UnitType, 
             OptionalAdder, ColorCode, MarginPercent, SortOrder) 
             OUTPUT INSERTED.ActualUnitID 
             VALUES (@VersionID, @RawUnitID, @ProductTypeID, @UnitName, @PlanSQFT, @UnitType, 
             @OptionalAdder, @ColorCode, @MarginPercent, @SortOrder)"

        ''' <summary>Updates an existing actual unit.</summary>
        Public Const UpdateActualUnit As String =
            "UPDATE ActualUnits SET RawUnitID = @RawUnitID, UnitName = @UnitName, PlanSQFT = @PlanSQFT, 
             UnitType = @UnitType, OptionalAdder = @OptionalAdder, ColorCode = @ColorCode, 
             MarginPercent = @MarginPercent, SortOrder = @SortOrder 
             WHERE ActualUnitID = @ActualUnitID"

        ''' <summary>Updates sort order for a single actual unit.</summary>
        Public Const UpdateActualUnitSortOrder As String =
            "UPDATE ActualUnits SET SortOrder = @SortOrder WHERE ActualUnitID = @ActualUnitID"

        ''' <summary>Updates sort order with version validation.</summary>
        Public Const UpdateActualUnitsSortOrderBatch As String =
            "UPDATE ActualUnits SET SortOrder = @SortOrder 
             WHERE ActualUnitID = @ActualUnitID AND VersionID = @VersionID"

        ''' <summary>Deletes an actual unit by ID.</summary>
        Public Const DeleteActualUnit As String =
            "DELETE FROM ActualUnits WHERE ActualUnitID = @ActualUnitID"

        ''' <summary>Counts mappings referencing an actual unit (for delete validation).</summary>
        Public Const CountMappingsByActualUnitID As String =
            "SELECT COUNT(*) FROM ActualToLevelMapping WHERE ActualUnitID = @ActualUnitID"

#End Region

#Region "Actual To Level Mapping"

        ''' <summary>Selects all mappings for a level with actual unit and raw unit details.</summary>
        Public Const SelectActualToLevelMappingsByLevelID As String =
            "SELECT alm.*, au.*, ru.RawUnitName AS ReferencedRawUnitName 
             FROM ActualToLevelMapping alm 
             JOIN ActualUnits au ON alm.ActualUnitID = au.ActualUnitID 
             JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID 
             WHERE alm.LevelID = @LevelID"

        ''' <summary>Selects all mappings for an actual unit.</summary>
        Public Const SelectActualToLevelMappingsByActualUnitID As String =
            "SELECT * FROM ActualToLevelMapping WHERE ActualUnitID = @ActualUnitID"

        ''' <summary>Inserts a new mapping. Returns the new MappingID.</summary>
        Public Const InsertActualToLevelMapping As String =
            "INSERT INTO ActualToLevelMapping (VersionID, ActualUnitID, LevelID, Quantity) 
             OUTPUT INSERTED.MappingID 
             VALUES (@VersionID, @ActualUnitID, @LevelID, @Quantity)"

        ''' <summary>Updates mapping quantity.</summary>
        Public Const UpdateActualToLevelMapping As String =
            "UPDATE ActualToLevelMapping SET Quantity = @Quantity WHERE MappingID = @MappingID"

        ''' <summary>Deletes all mappings for a level.</summary>
        Public Const DeleteActualToLevelMappingByLevelID As String =
            "DELETE FROM ActualToLevelMapping WHERE LevelID = @LevelID"

        ''' <summary>Deletes a specific mapping by ID.</summary>
        Public Const DeleteActualToLevelMappingByMappingID As String =
            "DELETE FROM ActualToLevelMapping WHERE MappingID = @MappingID"

#End Region

#Region "Calculated Components"

        ''' <summary>Selects all calculated components for an actual unit.</summary>
        Public Const SelectCalculatedComponentsByActualUnitID As String =
            "SELECT * FROM CalculatedComponents WHERE ActualUnitID = @ActualUnitID"

        ''' <summary>Inserts a new calculated component.</summary>
        Public Const InsertCalculatedComponent As String =
            "INSERT INTO CalculatedComponents (VersionID, ActualUnitID, ComponentType, Value) 
             VALUES (@VersionID, @ActualUnitID, @ComponentType, @Value)"

        ''' <summary>Deletes all calculated components for an actual unit.</summary>
        Public Const DeleteCalculatedComponentsByActualUnitID As String =
            "DELETE FROM CalculatedComponents WHERE ActualUnitID = @ActualUnitID"

#End Region

#Region "Product Types"

        ''' <summary>Selects all product types (Floor=1, Roof=2, Wall=3).</summary>
        Public Const SelectProductTypes As String =
            "SELECT * FROM ProductType"

#End Region

#Region "Project Product Settings"

        ''' <summary>Selects all product settings for a version.</summary>
        Public Const SelectProjectProductSettings As String =
            "SELECT * FROM ProjectProductSettings WHERE VersionID = @VersionID"

        ''' <summary>Inserts a new product setting. Returns the new SettingID.</summary>
        Public Const InsertProjectProductSetting As String =
            "INSERT INTO ProjectProductSettings (VersionID, ProductTypeID, MarginPercent, LumberAdder) 
             OUTPUT INSERTED.SettingID 
             VALUES (@VersionID, @ProductTypeID, @MarginPercent, @LumberAdder)"

        ''' <summary>Updates margin and lumber adder for a setting.</summary>
        Public Const UpdateProjectProductSetting As String =
            "UPDATE ProjectProductSettings SET MarginPercent = @MarginPercent, LumberAdder = @LumberAdder 
             WHERE SettingID = @SettingID"

        ''' <summary>Updates only the lumber adder for a product type.</summary>
        Public Const UpdateProjectProductSettingLumberAdder As String =
            "UPDATE ProjectProductSettings SET LumberAdder = @LumberAdder 
             WHERE VersionID = @VersionID AND ProductTypeID = @ProductTypeID"

        ''' <summary>Gets lumber adder for rollup calculations.</summary>
        Public Const SelectLumberAdder As String =
            "SELECT LumberAdder FROM ProjectProductSettings 
             WHERE VersionID = @VersionID AND ProductTypeID = @ProductTypeID"

        ''' <summary>Gets margin percent for pricing calculations.</summary>
        Public Const SelectMarginPercent As String =
            "SELECT MarginPercent FROM ProjectProductSettings 
             WHERE VersionID = @VersionID AND ProductTypeID = @ProductTypeID"

#End Region

#Region "Customers"

        ''' <summary>Selects customers filtered by type (1=GC, 2=Architect, 3=Engineer).</summary>
        Public Const SelectCustomers As String =
            "SELECT c.*, ct.CustomerTypeName 
             FROM Customer c 
             LEFT JOIN CustomerType ct ON c.CustomerType = ct.CustomerTypeID 
             WHERE (@CustomerType IS NULL OR c.CustomerType = @CustomerType) 
             ORDER BY CustomerName"

        ''' <summary>Inserts a new customer. Returns the new CustomerID.</summary>
        Public Const InsertCustomer As String =
            "INSERT INTO Customer (CustomerName, CustomerType) 
             OUTPUT INSERTED.CustomerID 
             VALUES (@CustomerName, @CustomerType)"

        ''' <summary>Updates an existing customer.</summary>
        Public Const UpdateCustomer As String =
            "UPDATE Customer SET CustomerName = @CustomerName, CustomerType = @CustomerType 
             WHERE CustomerID = @CustomerID"

        ''' <summary>Selects all customer types for dropdowns.</summary>
        Public Const SelectCustomerTypes As String =
            "SELECT CustomerTypeID, CustomerTypeName FROM CustomerType ORDER BY CustomerTypeName"

        ''' <summary>Gets CustomerType for validation.</summary>
        Public Const SelectCustomerTypeByID As String =
            "SELECT CustomerType FROM Customer WHERE CustomerID = @CustomerID"

        ''' <summary>Finds CustomerID by name and type (for GetOrInsert).</summary>
        Public Const SelectCustomerIDByNameAndType As String =
            "SELECT CustomerID FROM Customer WHERE CustomerName = @CustomerName AND CustomerType = @CustomerType ORDER BY CustomerName"

#End Region

#Region "Project Types"

        ''' <summary>Selects all project types for dropdowns.</summary>
        Public Const SelectProjectTypes As String =
            "SELECT * FROM ProjectType ORDER BY ProjectTypeName"

        ''' <summary>Inserts a new project type. Returns the new ProjectTypeID.</summary>
        Public Const InsertProjectType As String =
            "INSERT INTO ProjectType (ProjectTypeName, Description) 
             OUTPUT INSERTED.ProjectTypeID 
             VALUES (@ProjectTypeName, @Description)"

        ''' <summary>Updates an existing project type.</summary>
        Public Const UpdateProjectType As String =
            "UPDATE ProjectType SET ProjectTypeName = @ProjectTypeName, Description = @Description 
             WHERE ProjectTypeID = @ProjectTypeID"

#End Region

#Region "Estimators"

        ''' <summary>Selects all estimators for dropdowns.</summary>
        Public Const SelectEstimators As String =
            "SELECT * FROM Estimator ORDER BY EstimatorName"

        ''' <summary>Inserts a new estimator. Returns the new EstimatorID.</summary>
        Public Const InsertEstimator As String =
            "INSERT INTO Estimator (EstimatorName) 
             OUTPUT INSERTED.EstimatorID 
             VALUES (@EstimatorName)"

        ''' <summary>Updates an existing estimator.</summary>
        Public Const UpdateEstimator As String =
            "UPDATE Estimator SET EstimatorName = @EstimatorName WHERE EstimatorID = @EstimatorID"

        ''' <summary>Finds EstimatorID by name (for GetOrInsert).</summary>
        Public Const SelectEstimatorIDByName As String =
            "SELECT EstimatorID FROM Estimator WHERE EstimatorName = @EstimatorName"

        ''' <summary>Finds SalesID by name (for GetOrInsert).</summary>
        Public Const SelectSalesIDByName As String =
            "SELECT SalesID FROM Sales WHERE SalesName = @SalesName"


#End Region

#Region "Sales"

        ''' <summary>Selects all sales representatives.</summary>
        Public Const SelectSales As String =
            "SELECT * FROM Sales"

        ''' <summary>Inserts a new sales rep. Returns the new SalesID.</summary>
        Public Const InsertSales As String =
            "INSERT INTO Sales (SalesName) OUTPUT INSERTED.SalesID VALUES (@SalesName)"

        ''' <summary>Updates an existing sales rep.</summary>
        Public Const UpdateSales As String =
            "UPDATE Sales SET SalesName = @SalesName WHERE SalesID = @SalesID"

#End Region

#Region "Configuration"

        ''' <summary>Gets a configuration value by key (e.g., 'MileageRate').</summary>
        Public Const SelectConfigValue As String =
            "SELECT Value FROM Configuration WHERE ConfigKey = @ConfigKey"

#End Region

#Region "Level Rollup Calculations"

        ''' <summary>Calculates total SQFT for a level from mapped units.</summary>
        Public Const CalculateOverallSQFT As String =
            "SELECT SUM(au.PlanSQFT * alm.Quantity) AS OverallSQFT 
             FROM ActualUnits au 
             JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID 
             WHERE alm.LevelID = @LevelID"

        ''' <summary>Calculates total linear feet for a level.</summary>
        Public Const CalculateOverallLF As String =
            "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS OverallLF 
             FROM CalculatedComponents cc 
             JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID 
             JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID 
             WHERE cc.ComponentType = 'LF/SQFT' AND alm.LevelID = @LevelID"

        ''' <summary>Calculates total board feet for a level.</summary>
        Public Const CalculateOverallBDFT As String =
            "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS OverallBDFT 
             FROM CalculatedComponents cc 
             JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID 
             JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID 
             WHERE cc.ComponentType = 'BDFT/SQFT' AND alm.LevelID = @LevelID"

        ''' <summary>Calculates lumber cost using history table with adder.</summary>
        Public Const CalculateLumberCost As String =
            "SELECT SUM(rlh.LumberCost * au.OptionalAdder * alm.Quantity) + 
             COALESCE((l.OverallBDFT / 1000) * pps.LumberAdder, 0) AS LumberCost 
             FROM ActualToLevelMapping alm 
             JOIN ActualUnits au ON alm.ActualUnitID = au.ActualUnitID 
             JOIN (SELECT RawUnitID, LumberCost FROM RawUnitLumberHistory rlh1 
                   WHERE rlh1.UpdateDate = (SELECT MAX(UpdateDate) FROM RawUnitLumberHistory rlh2 
                   WHERE rlh2.RawUnitID = rlh1.RawUnitID AND rlh2.VersionID = @VersionID)) rlh 
             ON au.RawUnitID = rlh.RawUnitID 
             JOIN Levels l ON alm.LevelID = l.LevelID 
             JOIN ProjectProductSettings pps ON l.VersionID = pps.VersionID AND au.ProductTypeID = pps.ProductTypeID 
             WHERE alm.LevelID = @LevelID 
             GROUP BY l.OverallBDFT, pps.LumberAdder"

        ''' <summary>Calculates plate cost for a level.</summary>
        Public Const CalculatePlateCost As String =
            "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS PlateCost 
             FROM CalculatedComponents cc 
             JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID 
             JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID 
             WHERE cc.ComponentType = 'Plate/SQFT' AND alm.LevelID = @LevelID"

        ''' <summary>Calculates labor cost for a level.</summary>
        Public Const CalculateLaborCost As String =
            "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS LaborCost 
             FROM CalculatedComponents cc 
             JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID 
             JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID 
             WHERE cc.ComponentType = 'ManufLabor/SQFT' AND alm.LevelID = @LevelID"

        ''' <summary>Calculates labor man-hours for a level.</summary>
        Public Const CalculateLaborMH As String =
            "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS LaborMH 
             FROM CalculatedComponents cc 
             JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID 
             JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID 
             WHERE cc.ComponentType = 'ManHours/SQFT' AND alm.LevelID = @LevelID"

        ''' <summary>Calculates design cost for a level.</summary>
        Public Const CalculateDesignCost As String =
            "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS DesignCost 
             FROM CalculatedComponents cc 
             JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID 
             JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID 
             WHERE cc.ComponentType = 'DesignLabor/SQFT' AND alm.LevelID = @LevelID"

        ''' <summary>Calculates management cost for a level.</summary>
        Public Const CalculateMGMTCost As String =
            "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS MGMTCost 
             FROM CalculatedComponents cc 
             JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID 
             JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID 
             WHERE cc.ComponentType = 'MGMTLabor/SQFT' AND alm.LevelID = @LevelID"

        ''' <summary>Calculates job supplies cost for a level.</summary>
        Public Const CalculateJobSuppliesCost As String =
            "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS JobSuppliesCost 
             FROM CalculatedComponents cc 
             JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID 
             JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID 
             WHERE cc.ComponentType = 'JobSupplies/SQFT' AND alm.LevelID = @LevelID"

        ''' <summary>Calculates items cost for a level.</summary>
        Public Const CalculateItemsCost As String =
            "SELECT SUM(cc.Value * au.OptionalAdder * alm.Quantity * au.PlanSQFT) AS ItemsCost 
             FROM CalculatedComponents cc 
             JOIN ActualUnits au ON cc.ActualUnitID = au.ActualUnitID 
             JOIN ActualToLevelMapping alm ON au.ActualUnitID = alm.ActualUnitID 
             WHERE cc.ComponentType = 'ItemCost/SQFT' AND alm.LevelID = @LevelID"

        ''' <summary>Calculates delivery cost based on BDFT, mileage rate, and distance.</summary>
        Public Const CalculateDeliveryCost As String =
            "SELECT CASE WHEN l.OverallBDFT < 1 THEN 0 
             ELSE CASE WHEN ROUND(CEILING(l.OverallBDFT / 10000.0) * c.Value * p.MilesToJobSite, 0) < 150 
             THEN 150 ELSE ROUND(CEILING(l.OverallBDFT / 10000.0) * c.Value * p.MilesToJobSite, 0) END END AS DeliveryCost 
             FROM Levels l 
             JOIN ProjectVersions pv ON l.VersionID = pv.VersionID 
             JOIN Projects p ON pv.ProjectID = p.ProjectID 
             JOIN Configuration c ON c.ConfigKey = 'MileageRate' 
             WHERE l.LevelID = @LevelID"

        ''' <summary>Calculates overall cost (sum of all cost components).</summary>
        Public Const CalculateOverallCost As String =
            "SELECT SUM(l.LumberCost + l.PlateCost + l.LaborCost + l.DesignCost + l.MGMTCost + 
             l.JobSuppliesCost + l.ItemsCost) AS OverallCost 
             FROM Levels l WHERE l.LevelID = @LevelID GROUP BY l.LevelID"

        ''' <summary>Calculates unit sell price without delivery for margin calculations.</summary>
        Public Const CalculateSumUnitSellNoDelivery As String =
            "SELECT SUM((cc.Value * au.PlanSQFT * au.OptionalAdder * alm.Quantity) / 
             (1 - ((ru.TotalSellPrice - ru.OverallCost) / ru.TotalSellPrice))) 
             FROM ActualToLevelMapping alm 
             JOIN ActualUnits au ON alm.ActualUnitID = au.ActualUnitID 
             JOIN RawUnits ru ON au.RawUnitID = ru.RawUnitID 
             JOIN CalculatedComponents cc ON cc.ActualUnitID = au.ActualUnitID 
             WHERE alm.LevelID = @LevelID AND cc.ComponentType = 'OverallCost/SQFT' 
             AND ru.TotalSellPrice > 0 AND ((ru.TotalSellPrice - ru.OverallCost) / ru.TotalSellPrice) BETWEEN 0 AND 0.999"

        ''' <summary>Updates all rollup fields for a level.</summary>
        Public Const UpdateLevelRollupsSql As String =
            "UPDATE Levels SET OverallSQFT = @OverallSQFT, OverallLF = @OverallLF, OverallBDFT = @OverallBDFT, 
             LumberCost = @LumberCost, PlateCost = @PlateCost, LaborCost = @LaborCost, LaborMH = @LaborMH, 
             DesignCost = @DesignCost, MGMTCost = @MGMTCost, JobSuppliesCost = @JobSuppliesCost, 
             ItemsCost = @ItemsCost, DeliveryCost = @DeliveryCost, OverallCost = @OverallCost, 
             OverallPrice = @OverallPrice, TotalSQFT = @TotalSQFT, AvgPricePerSQFT = @AvgPricePerSQFT 
             WHERE LevelID = @LevelID"

#End Region

#Region "Building Rollup Calculations"

        ''' <summary>Calculates floor cost per building.</summary>
        Public Const CalculateFloorCostPerBldg As String =
            "SELECT SUM(l.OverallCost) AS FloorCostPerBldg 
             FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"

        ''' <summary>Calculates floor delivery cost.</summary>
        Public Const CalculateFloorDeliveryCost As String =
            "SELECT SUM(l.DeliveryCost) AS FloorDeliveryCost 
             FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"

        ''' <summary>Calculates floor price per building.</summary>
        Public Const CalculateFloorPricePerBldg As String =
            "SELECT SUM(l.OverallPrice) AS FloorPricePerBldg 
             FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"

        ''' <summary>Calculates floor base cost.</summary>
        Public Const CalculateFloorBaseCost As String =
            "SELECT SUM(l.OverallCost) AS FloorBaseCost 
             FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 1"

        ''' <summary>Calculates roof cost per building.</summary>
        Public Const CalculateRoofCostPerBldg As String =
            "SELECT SUM(l.OverallCost) AS RoofCostPerBldg 
             FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"

        ''' <summary>Calculates roof delivery cost.</summary>
        Public Const CalculateRoofDeliveryCost As String =
            "SELECT SUM(l.DeliveryCost) AS RoofDeliveryCost 
             FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"

        ''' <summary>Calculates roof price per building.</summary>
        Public Const CalculateRoofPricePerBldg As String =
            "SELECT SUM(l.OverallPrice) AS RoofPricePerBldg 
             FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"

        ''' <summary>Calculates roof base cost.</summary>
        Public Const CalculateRoofBaseCost As String =
            "SELECT SUM(l.OverallCost) AS RoofBaseCost 
             FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 2"

        ''' <summary>Calculates wall price per building.</summary>
        Public Const CalculateWallPricePerBldg As String =
            "SELECT SUM(l.OverallPrice) AS WallPricePerBldg 
             FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 3"

        ''' <summary>Calculates wall base cost.</summary>
        Public Const CalculateWallBaseCost As String =
            "SELECT SUM(l.OverallCost) AS WallBaseCost 
             FROM Levels l WHERE l.BuildingID = @BuildingID AND l.ProductTypeID = 3"

        ''' <summary>Updates all rollup fields for a building.</summary>
        Public Const UpdateBuildingRollupsSql As String =
            "UPDATE Buildings SET FloorCostPerBldg = @FloorCostPerBldg, RoofCostPerBldg = @RoofCostPerBldg, 
             WallCostPerBldg = @WallCostPerBldg, ExtendedFloorCost = @ExtendedFloorCost, 
             ExtendedRoofCost = @ExtendedRoofCost, ExtendedWallCost = @ExtendedWallCost, 
             OverallPrice = @OverallPrice, OverallCost = @OverallCost 
             WHERE BuildingID = @BuildingID"

#End Region

#Region "Project Design Info"

        ''' <summary>Selects design info for a project.</summary>
        Public Const SelectProjectDesignInfo As String =
            "SELECT * FROM ProjectDesignInfo WHERE ProjectID = @ProjectID"

        ''' <summary>Inserts new project design info. Returns the new InfoID.</summary>
        Public Const InsertProjectDesignInfo As String =
            "INSERT INTO ProjectDesignInfo (ProjectID, BuildingCode, Importance, ExposureCategory, 
             WindSpeed, SnowLoadType, OccupancyCategory, RoofPitches, FloorDepths, WallHeights, HeelHeights) 
             OUTPUT INSERTED.InfoID 
             VALUES (@ProjectID, @BuildingCode, @Importance, @ExposureCategory, @WindSpeed, 
             @SnowLoadType, @OccupancyCategory, @RoofPitches, @FloorDepths, @WallHeights, @HeelHeights)"

        ''' <summary>Updates existing project design info.</summary>
        Public Const UpdateProjectDesignInfo As String =
            "UPDATE ProjectDesignInfo SET BuildingCode = @BuildingCode, Importance = @Importance, 
             ExposureCategory = @ExposureCategory, WindSpeed = @WindSpeed, SnowLoadType = @SnowLoadType, 
             OccupancyCategory = @OccupancyCategory, RoofPitches = @RoofPitches, FloorDepths = @FloorDepths, 
             WallHeights = @WallHeights, HeelHeights = @HeelHeights 
             WHERE InfoID = @InfoID"

#End Region

#Region "Project Loads"

        ''' <summary>Selects all loads for a project ordered by category.</summary>
        Public Const SelectProjectLoads As String =
            "SELECT * FROM ProjectLoads WHERE ProjectID = @ProjectID ORDER BY Category DESC"

        ''' <summary>Deletes all loads for a project (before re-inserting).</summary>
        Public Const DeleteProjectLoads As String =
            "DELETE FROM ProjectLoads WHERE ProjectID = @ProjectID"

        ''' <summary>Inserts a new project load. Returns the new LoadID.</summary>
        Public Const InsertProjectLoad As String =
            "INSERT INTO ProjectLoads (ProjectID, Category, TCLL, TCDL, BCLL, BCDL, OCSpacing, 
             LiveLoadDeflection, TotalLoadDeflection, AbsoluteLL, AbsoluteTL) 
             OUTPUT INSERTED.LoadID 
             VALUES (@ProjectID, @Category, @TCLL, @TCDL, @BCLL, @BCDL, @OCSpacing, 
             @LiveLoadDeflection, @TotalLoadDeflection, @AbsoluteLL, @AbsoluteTL)"

#End Region

#Region "Project Bearing Styles"

        ''' <summary>Selects bearing styles for a project.</summary>
        Public Const SelectProjectBearingStyles As String =
            "SELECT * FROM ProjectBearingStyles WHERE ProjectID = @ProjectID"

        ''' <summary>Inserts new bearing styles. Returns the new BearingID.</summary>
        Public Const InsertProjectBearingStyles As String =
            "INSERT INTO ProjectBearingStyles (ProjectID, ExtWallStyle, ExtRimRibbon, IntWallStyle, 
             IntRimRibbon, CorridorWallStyle, CorridorRimRibbon) 
             OUTPUT INSERTED.BearingID 
             VALUES (@ProjectID, @ExtWallStyle, @ExtRimRibbon, @IntWallStyle, @IntRimRibbon, 
             @CorridorWallStyle, @CorridorRimRibbon)"

        ''' <summary>Updates existing bearing styles.</summary>
        Public Const UpdateProjectBearingStyles As String =
            "UPDATE ProjectBearingStyles SET ExtWallStyle = @ExtWallStyle, ExtRimRibbon = @ExtRimRibbon, 
             IntWallStyle = @IntWallStyle, IntRimRibbon = @IntRimRibbon, CorridorWallStyle = @CorridorWallStyle, 
             CorridorRimRibbon = @CorridorRimRibbon 
             WHERE BearingID = @BearingID"

#End Region

#Region "Project General Notes"

        ''' <summary>Selects general notes for a project.</summary>
        Public Const SelectProjectGeneralNotes As String =
            "SELECT * FROM ProjectGeneralNotes WHERE ProjectID = @ProjectID"

        ''' <summary>Inserts new general notes. Returns the new NoteID.</summary>
        Public Const InsertProjectGeneralNotes As String =
            "INSERT INTO ProjectGeneralNotes (ProjectID, Notes) 
             OUTPUT INSERTED.NoteID 
             VALUES (@ProjectID, @Notes)"

        ''' <summary>Updates existing general notes.</summary>
        Public Const UpdateProjectGeneralNotes As String =
            "UPDATE ProjectGeneralNotes SET Notes = @Notes WHERE NoteID = @NoteID"

#End Region

#Region "Project Items"

        ''' <summary>Selects all items for a project.</summary>
        Public Const SelectProjectItems As String =
            "SELECT ItemID, ProjectID, Section, KN, Description, Status, Note 
             FROM ProjectItems WHERE ProjectID = @ProjectID"

        ''' <summary>Deletes all items for a project.</summary>
        Public Const DeleteProjectItems As String =
            "DELETE FROM ProjectItems WHERE ProjectID = @ProjectID"

        ''' <summary>Inserts a new project item. Returns the new ItemID.</summary>
        Public Const InsertProjectItem As String =
            "INSERT INTO ProjectItems (ProjectID, Section, KN, Description, Status, Note) 
             OUTPUT INSERTED.ItemID 
             VALUES (@ProjectID, @Section, @KN, @Description, @Status, @Note)"

#End Region

#Region "Combo and Item Options"

        ''' <summary>Selects combo options by category for dropdowns.</summary>
        Public Const SelectComboOptions As String =
            "SELECT Value FROM ComboOptions WHERE Category = @Category ORDER BY DisplayOrder, Value"

        ''' <summary>Selects item options by section for project items.</summary>
        Public Const SelectItemOptions As String =
            "SELECT KN, Description FROM ItemOptions WHERE Section = @Section ORDER BY DisplayOrder"

#End Region

#Region "Lumber Types"

        ''' <summary>Selects all lumber types for dropdowns.</summary>
        Public Const SelectLumberTypes As String =
            "SELECT LumberTypeID, LumberTypeDesc FROM LumberType ORDER BY LumberTypeDesc"

        ''' <summary>Inserts a new lumber type. Returns the new LumberTypeID.</summary>
        Public Const InsertLumberType As String =
            "INSERT INTO LumberType (LumberTypeDesc) OUTPUT INSERTED.LumberTypeID VALUES (@LumberTypeDesc)"

        ''' <summary>Gets LumberTypeID by description.</summary>
        Public Const SelectLumberTypeIDByDesc As String =
            "SELECT LumberTypeID FROM LumberType WHERE LumberTypeDesc = @LumberTypeDesc"

#End Region

#Region "Lumber Cost Effective Dates"

        ''' <summary>Selects all cost effective dates ordered by date descending.</summary>
        Public Const SelectLumberCostEffective As String =
            "SELECT CostEffectiveID, CosteffectiveDate FROM LumberCostEffective ORDER BY CosteffectiveDate DESC"

        ''' <summary>Inserts a new cost effective date. Returns the new CostEffectiveID.</summary>
        Public Const InsertLumberCostEffective As String =
            "INSERT INTO LumberCostEffective (CosteffectiveDate) 
             OUTPUT INSERTED.CostEffectiveID 
             VALUES (@CosteffectiveDate)"

#End Region

#Region "Lumber Costs"

        ''' <summary>Selects all lumber costs for an effective date with type descriptions.</summary>
        Public Const SelectLumberCostsByEffectiveDate As String =
            "SELECT lc.LumberCostID, lc.LumberTypeID, lt.LumberTypeDesc, lc.LumberCost, lc.CostEffectiveDateID 
             FROM LumberCost lc 
             JOIN LumberType lt ON lc.LumberTypeID = lt.LumberTypeID 
             WHERE lc.CostEffectiveDateID = @CostEffectiveDateID 
             ORDER BY lt.LumberTypeDesc"

        ''' <summary>Inserts a new lumber cost. Returns the new LumberCostID.</summary>
        Public Const InsertLumberCost As String =
            "INSERT INTO LumberCost (LumberTypeID, LumberCost, CostEffectiveDateID) 
             OUTPUT INSERTED.LumberCostID 
             VALUES (@LumberTypeID, @LumberCost, @CostEffectiveDateID)"

        ''' <summary>Updates an existing lumber cost.</summary>
        Public Const UpdateLumberCost As String =
            "UPDATE LumberCost SET LumberCost = @LumberCost WHERE LumberCostID = @LumberCostID"

        ''' <summary>Gets lumber cost by type and effective date.</summary>
        Public Const SelectLumberCostByTypeAndDate As String =
            "SELECT LumberCost FROM LumberCost 
             WHERE LumberTypeID = @LumberTypeID AND CostEffectiveDateID = @CostEffectiveDateID"

        ''' <summary>Finds CostEffectiveDateID by SPF lumber cost value.</summary>
        Public Const SelectCostEffectiveDateIDByCost As String =
            "SELECT lc.CostEffectiveDateID FROM LumberCost lc 
             JOIN LumberType lt ON lc.LumberTypeID = lt.LumberTypeID 
             WHERE lt.LumberTypeID = 1 AND lc.LumberCost = @SPFLumberCost"

#End Region

#Region "Raw Unit Lumber History"

        ''' <summary>Inserts a new lumber history record. Returns the new HistoryID.</summary>
        Public Const InsertRawUnitLumberHistory As String =
            "INSERT INTO RawUnitLumberHistory (RawUnitID, VersionID, CostEffectiveDateID, LumberCost, 
             AvgSPFNo2, Avg241800, Avg242400, Avg261800, Avg262400, UpdateDate, IsActive) 
             OUTPUT INSERTED.HistoryID 
             VALUES (@RawUnitID, @VersionID, @CostEffectiveDateID, @LumberCost, @AvgSPFNo2, 
             @Avg241800, @Avg242400, @Avg261800, @Avg262400, GETDATE(), 1)"

        ''' <summary>Gets the latest lumber history for a raw unit (any active state).</summary>
        Public Const SelectLatestLumberHistoryByRawUnit As String =
            "SELECT TOP 1 * FROM RawUnitLumberHistory 
             WHERE RawUnitID = @RawUnitID AND VersionID = @VersionID 
             ORDER BY UpdateDate DESC"

        ''' <summary>Gets the latest ACTIVE lumber history for a raw unit.</summary>
        Public Const SelectLatestActiveLumberHistory As String =
            "SELECT TOP 1 * FROM RawUnitLumberHistory 
             WHERE RawUnitID = @RawUnitID AND VersionID = @VersionID AND IsActive = 1 
             ORDER BY UpdateDate DESC"

        ''' <summary>Gets all lumber history for a version.</summary>
        Public Const SelectLumberHistoryByVersion As String =
            "SELECT rlh.* FROM RawUnitLumberHistory rlh 
             WHERE rlh.VersionID = @VersionID 
             ORDER BY rlh.UpdateDate DESC 
             OPTION (Recompile)"

        ''' <summary>Gets distinct cost effective dates with active status for a version.</summary>
        Public Const SelectDistinctLumberHistoryDates As String =
            "SELECT rlh.CostEffectiveDateID, lce.CosteffectiveDate, 
             CASE WHEN EXISTS (SELECT 1 FROM RawUnitLumberHistory rlh2 
             WHERE rlh2.CostEffectiveDateID = rlh.CostEffectiveDateID AND rlh2.VersionID = @VersionID 
             AND rlh2.IsActive = 1) THEN 1 ELSE 0 END AS IsActive, 
             MAX(rlh.UpdateDate) AS UpdateDate 
             FROM RawUnitLumberHistory rlh 
             JOIN LumberCostEffective lce ON rlh.CostEffectiveDateID = lce.CostEffectiveID 
             WHERE rlh.VersionID = @VersionID 
             GROUP BY rlh.CostEffectiveDateID, lce.CosteffectiveDate 
             ORDER BY lce.CosteffectiveDate DESC"

        ''' <summary>Sets a specific history record as active (deactivates others first).</summary>
        Public Const SetLumberHistoryActive As String =
            "UPDATE RawUnitLumberHistory SET IsActive = 0 WHERE RawUnitID = @RawUnitID AND VersionID = @VersionID; 
             UPDATE RawUnitLumberHistory SET IsActive = 1 WHERE HistoryID = @HistoryID"

        ''' <summary>Deactivates all lumber history for a raw unit/version.</summary>
        Public Const DeactivateLumberHistory As String =
            "UPDATE RawUnitLumberHistory SET IsActive = 0 WHERE RawUnitID = @RawUnitID AND VersionID = @VersionID"

        ''' <summary>Deletes a specific lumber history record.</summary>
        Public Const DeleteLumberHistory As String =
            "DELETE FROM RawUnitLumberHistory WHERE HistoryID = @HistoryID AND VersionID = @VersionID"

        ''' <summary>Gets average SPFNo2 from active lumber history by product type.</summary>
        Public Const SelectActiveSPFNo2ByProductType As String =
            "SELECT AVG(rlh.AvgSPFNo2) FROM RawUnitLumberHistory rlh 
             JOIN RawUnits ru ON rlh.RawUnitID = ru.RawUnitID 
             JOIN ProductType pt ON ru.ProductTypeID = pt.ProductTypeID 
             WHERE rlh.VersionID = @VersionID AND pt.ProductTypeName = @ProductTypeName AND rlh.IsActive = 1"

        ''' <summary>Gets average SPFNo2 from RawUnits (not history) by product type.</summary>
        Public Const SelectAverageSPFNo2ByProductType As String =
            "SELECT AVG(ru.AvgSPFNo2) FROM RawUnits ru 
             INNER JOIN ProductType pt ON ru.ProductTypeID = pt.ProductTypeID 
             WHERE ru.VersionID = @VersionID AND pt.ProductTypeName = @ProductTypeName AND ru.AvgSPFNo2 IS NOT NULL"

#End Region

#Region "Lumber Futures"

        ''' <summary>Selects all futures for a version ordered by pull date.</summary>
        Public Const SelectLumberFuturesByVersion As String =
            "SELECT LumberFutureID, ContractMonth, PriorSettle, PullDate, Active 
             FROM LumberFutures WHERE VersionID = @VersionID ORDER BY PullDate"

        ''' <summary>Inserts a new lumber future record.</summary>
        Public Const InsertLumberFuture As String =
            "INSERT INTO LumberFutures (VersionID, ContractMonth, PriorSettle, PullDate) 
             VALUES (@VersionID, @ContractMonth, @PriorSettle, GETDATE())"

        ''' <summary>Updates an existing lumber future by contract month.</summary>
        Public Const UpdateLumberFuture As String =
            "UPDATE LumberFutures SET PriorSettle = @PriorSettle, PullDate = GETDATE() 
             WHERE VersionID = @VersionID AND ContractMonth = @ContractMonth"

        ''' <summary>Upserts a lumber future (inserts if not exists, updates if exists).</summary>
        Public Const UpsertLumberFuture As String =
            "IF EXISTS (SELECT 1 FROM LumberFutures WHERE VersionID = @VersionID AND ContractMonth = @ContractMonth) 
             UPDATE LumberFutures SET PriorSettle = @PriorSettle, PullDate = GETDATE() 
             WHERE VersionID = @VersionID AND ContractMonth = @ContractMonth 
             ELSE INSERT INTO LumberFutures (VersionID, ContractMonth, PriorSettle) 
             VALUES (@VersionID, @ContractMonth, @PriorSettle)"

        ''' <summary>Sets a lumber future as active (deactivates others first).</summary>
        Public Const SetActiveLumberFuture As String =
            "UPDATE LumberFutures SET Active = 0 WHERE VersionID = @VersionID;
             UPDATE LumberFutures SET Active = 1 WHERE LumberFutureID = @LumberFutureID AND VersionID = @VersionID;"

        ''' <summary>Alias for SetActiveLumberFuture (same functionality).</summary>
        Public Const SetLumberFutureActive As String =
            "UPDATE LumberFutures SET Active = 0 WHERE VersionID = @VersionID;
             UPDATE LumberFutures SET Active = 1 WHERE LumberFutureID = @LumberFutureID AND VersionID = @VersionID;"

#End Region

#Region "Level Actuals (Multi-Shipment)"

        ''' <summary>Inserts a new actuals row (Design or Invoice). Returns the new ActualID.</summary>
        ''' <remarks>StageType: 1=Design, 2=Invoice</remarks>
        Public Const InsertLevelActual As String =
            "INSERT INTO LevelActuals (LevelID, VersionID, StageType, ActualBDFT, ActualLumberCost, 
             ActualPlateCost, ActualManufLaborCost, ActualManufMH, ActualItemCost, ActualDeliveryCost, 
             ActualMiscLaborCost, ActualTotalCost, ActualSoldAmount, ActualMarginPercent, AvgSPFNo2Actual,
             MiTekJobNumber, BistrackWorksOrder, BisTrackSalesOrder, BuildingID, ImportedBy, Notes)
             OUTPUT INSERTED.ActualID
             VALUES (@LevelID, @VersionID, @StageType, @ActualBDFT, @ActualLumberCost, @ActualPlateCost, 
             @ActualManufLaborCost, @ActualManufMH, @ActualItemCost, @ActualDeliveryCost, @ActualMiscLaborCost, 
             @ActualTotalCost, @ActualSoldAmount, @ActualMarginPercent, @AvgSPFNo2Actual, @MiTekJobNumber, 
             @BistrackWorksOrder, @BisTrackSalesOrder, @BuildingID, @ImportedBy, @Notes)"

        ''' <summary>Gets all actuals for a level (shows every shipment).</summary>
        Public Const SelectAllActualsForLevel As String =
            "SELECT ActualID, StageType, ImportDate, MiTekJobNumber, BistrackWorksOrder, BisTrackSalesOrder,
             ActualBDFT, ActualLumberCost, ActualPlateCost, ActualManufLaborCost, ActualItemCost, 
             ActualDeliveryCost, ActualMiscLaborCost, ActualTotalCost, ActualSoldAmount, ActualMarginPercent, 
             AvgSPFNo2Actual, ActualManufMH, Notes
             FROM LevelActuals
             WHERE LevelID = @LevelID AND VersionID = @VersionID
             ORDER BY ImportDate DESC"

        ''' <summary>Multi-shipment variance grid comparing estimate vs. actual shipments.</summary>
        Public Const SelectMultiShipmentVariance As String =
            "SELECT l.LevelID, l.LevelName, b.BldgQty,
             la.BisTrackSalesOrder AS [Shipment SO], la.ImportDate AS [Ship Date],
             l.OverallBDFT AS [Est BDFT per Bldg], la.ActualBDFT AS [Actual BDFT],
             ISNULL(la.ActualBDFT, 0) - l.OverallBDFT AS [BDFT Var],
             l.LumberCost AS [Est Lumber per Bldg], la.ActualLumberCost AS [Actual Lumber],
             ISNULL(la.ActualLumberCost, 0) - l.LumberCost AS [Lumber Var],
             l.OverallCost AS [Est Cost per Bldg], la.ActualTotalCost AS [Actual Cost],
             ISNULL(la.ActualTotalCost, 0) - l.OverallCost AS [Cost Var],
             l.OverallPrice AS [Est Sold per Bldg], la.ActualSoldAmount AS [Actual Sold],
             ISNULL(la.ActualSoldAmount, 0) - l.OverallPrice AS [Sold Var],
             la.ActualMarginPercent AS [Actual Margin %],
             l.AvgSPFNo2 AS [Est SPF#2], la.AvgSPFNo2Actual AS [Actual SPF#2]
             FROM Levels l
             JOIN Buildings b ON l.BuildingID = b.BuildingID
             LEFT JOIN LevelActuals la ON l.LevelID = la.LevelID AND l.VersionID = la.VersionID AND la.StageType = 1
             WHERE l.VersionID = @VersionID
             ORDER BY l.LevelName, la.ImportDate DESC"

        ''' <summary>Project-level summary across all shipments.</summary>
        Public Const SelectProjectShipmentSummary As String =
            "SELECT COUNT(DISTINCT la.BisTrackSalesOrder) AS TotalShipments,
             SUM(l.OverallBDFT * b.BldgQty) AS TotalEstBDFT, SUM(la.ActualBDFT) AS TotalActualBDFT,
             SUM(la.ActualBDFT) - SUM(l.OverallBDFT * b.BldgQty) AS NetBDFTVar,
             SUM(l.OverallCost * b.BldgQty) AS TotalEstCost, SUM(la.ActualTotalCost) AS TotalActualCost,
             SUM(la.ActualTotalCost) - SUM(l.OverallCost * b.BldgQty) AS NetCostVar,
             SUM(l.OverallPrice * b.BldgQty) AS TotalEstSold, SUM(la.ActualSoldAmount) AS TotalActualSold,
             SUM(la.ActualSoldAmount) - SUM(l.OverallPrice * b.BldgQty) AS NetSoldVar
             FROM Levels l
             JOIN Buildings b ON l.BuildingID = b.BuildingID
             LEFT JOIN LevelActuals la ON l.LevelID = la.LevelID AND l.VersionID = la.VersionID AND la.StageType = 1
             WHERE l.VersionID = @VersionID"

        ''' <summary>Design-stage variance (MiTek actuals vs. Estimate).</summary>
        Public Const SelectDesignVariance As String =
            "SELECT l.LevelName, l.OverallBDFT AS EstBDFT, la.ActualBDFT AS DesignBDFT,
             la.ActualBDFT - l.OverallBDFT AS DesignBDFTVar,
             la.ActualTotalCost - l.OverallCost AS DesignCostVar
             FROM Levels l
             LEFT JOIN LevelActuals la ON l.LevelID = la.LevelID AND l.VersionID = la.VersionID AND la.StageType = 1
             WHERE l.VersionID = @VersionID"

        ''' <summary>Checks if a specific shipment already exists (prevents duplicates).</summary>
        Public Const SelectExistingShipment As String =
            "SELECT ActualID FROM LevelActuals 
             WHERE LevelID = @LevelID AND VersionID = @VersionID 
             AND BisTrackSalesOrder = @BisTrackSalesOrder AND StageType = 2"

        ''' <summary>Deletes a specific shipment row.</summary>
        Public Const DeleteShipmentActual As String =
            "DELETE FROM LevelActuals WHERE ActualID = @ActualID"

#End Region

#Region "Project Summary Report"

        ''' <summary>Gets comprehensive project summary for reports.</summary>
        Public Const SelectProjectSummary As String =
            "SELECT p.ProjectName, p.JBID, pv.VersionName, c.CustomerName, s.SalesName,
             p.City, p.State, ca.CustomerName AS Architect, ce.CustomerName AS Engineer, 
             p.ArchPlansDated, p.EngPlansDated, b.BuildingName, b.BldgQty, b.OverallPrice,
             l.LevelName, l.OverallSQFT, l.OverallPrice AS OverallPrice_Level, pt.ProductTypeName,
             (SELECT SUM(b2.BldgQty) FROM Buildings b2 WHERE b2.VersionID = pv.VersionID) AS TotalBldgQty
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

#End Region

#Region "Users"

        ''' <summary>Finds a user by Windows logon name.</summary>
        Public Const SelectUserByWindowsLogon As String =
            "SELECT * FROM Users WHERE WindowsLogon = @WindowsLogon"

        ''' <summary>Inserts a new user. Returns the new UserID.</summary>
        Public Const InsertUser As String =
            "INSERT INTO Users (WindowsLogon, DisplayName, EstimatorID, IsActive, IsAdmin, IsLoggedIn, LastLogin) 
             OUTPUT INSERTED.UserID 
             VALUES (@WindowsLogon, @DisplayName, @EstimatorID, 1, 0, 1, GETDATE())"

        ''' <summary>Updates user login status and timestamps.</summary>
        Public Const UpdateUserLoginStatus As String =
            "UPDATE Users SET IsLoggedIn = @IsLoggedIn, 
             LastLogin = COALESCE(@LastLogin, LastLogin), 
             LastLogout = COALESCE(@LastLogout, LastLogout) 
             WHERE UserID = @UserID"

#End Region

#Region "Version Duplication"

        ''' <summary>Duplicates buildings from one version to another. Returns new BuildingIDs.</summary>
        Public Const DuplicateBuildings As String =
            "INSERT INTO Buildings (BuildingName, BuildingType, ResUnits, BldgQty, VersionID, LastModifiedDate) 
             OUTPUT INSERTED.BuildingID 
             SELECT BuildingName, BuildingType, ResUnits, BldgQty, @NewVersionID, GetDate() 
             FROM Buildings WHERE VersionID = @OriginalVersionID"

        ''' <summary>Duplicates levels from one building to another. Returns new LevelIDs.</summary>
        Public Const DuplicateLevels As String =
            "INSERT INTO Levels (VersionID, BuildingID, ProductTypeID, LevelNumber, LevelName, LastModifiedDate) 
             OUTPUT INSERTED.LevelID 
             SELECT @NewVersionID, @NewBuildingID, ProductTypeID, LevelNumber, LevelName, GetDate() 
             FROM Levels WHERE VersionID = @OriginalVersionID AND BuildingID = @OriginalBuildingID"

        ''' <summary>Duplicates raw units. Returns new RawUnitIDs.</summary>
        Public Const DuplicateRawUnits As String =
            "INSERT INTO RawUnits (RawUnitName, VersionID, ProductTypeID, BF, LF, EWPLF, SqFt, FCArea, 
             LumberCost, PlateCost, ManufLaborCost, DesignLabor, MGMTLabor, JobSuppliesCost, ManHours, 
             ItemCost, OverallCost, DeliveryCost, TotalSellPrice, AvgSPFNo2, SPFNo2BDFT, Avg241800, 
             MSR241800BDFT, Avg242400, MSR242400BDFT, Avg261800, MSR261800BDFT, Avg262400, MSR262400BDFT) 
             OUTPUT INSERTED.RawUnitID 
             SELECT RawUnitName, @NewVersionID, ProductTypeID, BF, LF, EWPLF, SqFt, FCArea, LumberCost, 
             PlateCost, ManufLaborCost, DesignLabor, MGMTLabor, JobSuppliesCost, ManHours, ItemCost, 
             OverallCost, DeliveryCost, TotalSellPrice, AvgSPFNo2, SPFNo2BDFT, Avg241800, MSR241800BDFT, 
             Avg242400, MSR242400BDFT, Avg261800, MSR261800BDFT, Avg262400, MSR262400BDFT 
             FROM RawUnits WHERE VersionID = @OriginalVersionID"

        ''' <summary>Duplicates actual units. Returns new ActualUnitIDs.</summary>
        Public Const DuplicateActualUnits As String =
            "INSERT INTO ActualUnits (VersionID, RawUnitID, ProductTypeID, UnitName, PlanSQFT, UnitType, OptionalAdder) 
             OUTPUT INSERTED.ActualUnitID 
             SELECT @NewVersionID, @NewRawUnitID, ProductTypeID, UnitName, PlanSQFT, UnitType, OptionalAdder 
             FROM ActualUnits WHERE VersionID = @OriginalVersionID AND RawUnitID = @OriginalRawUnitID"

        ''' <summary>Duplicates calculated components. Returns new ComponentIDs.</summary>
        Public Const DuplicateCalculatedComponents As String =
            "INSERT INTO CalculatedComponents (VersionID, ActualUnitID, ComponentType, Value) 
             OUTPUT INSERTED.ComponentID 
             SELECT @NewVersionID, @NewActualUnitID, ComponentType, Value 
             FROM CalculatedComponents WHERE VersionID = @OriginalVersionID AND ActualUnitID = @OriginalActualUnitID"

        ''' <summary>Duplicates actual to level mappings. Returns new MappingIDs.</summary>
        Public Const DuplicateActualToLevelMapping As String =
            "INSERT INTO ActualToLevelMapping (VersionID, ActualUnitID, LevelID, Quantity) 
             OUTPUT INSERTED.MappingID 
             SELECT @NewVersionID, @NewActualUnitID, @NewLevelID, Quantity 
             FROM ActualToLevelMapping 
             WHERE VersionID = @OriginalVersionID AND ActualUnitID = @OriginalActualUnitID AND LevelID = @OriginalLevelID"

        ''' <summary>Duplicates project product settings. Returns new SettingIDs.</summary>
        Public Const DuplicateProjectProductSettings As String =
            "INSERT INTO ProjectProductSettings (VersionID, ProductTypeID, MarginPercent, LumberAdder) 
             OUTPUT INSERTED.SettingID 
             SELECT @NewVersionID, ProductTypeID, MarginPercent, LumberAdder 
             FROM ProjectProductSettings WHERE VersionID = @OriginalVersionID"

#End Region

#Region "Cascade Delete Operations"

        ''' <summary>Deletes all actual to level mappings for a version.</summary>
        Public Const DeleteActualToLevelMappingsByVersion As String =
            "DELETE FROM ActualToLevelMapping WHERE VersionID = @VersionID"

        ''' <summary>Deletes all calculated components for a version.</summary>
        Public Const DeleteCalculatedComponentsByVersion As String =
            "DELETE FROM CalculatedComponents WHERE VersionID = @VersionID"

        ''' <summary>Deletes all actual units for a version.</summary>
        Public Const DeleteActualUnitsByVersion As String =
            "DELETE FROM ActualUnits WHERE VersionID = @VersionID"

        ''' <summary>Deletes all raw units for a version.</summary>
        Public Const DeleteRawUnitsByVersion As String =
            "DELETE FROM RawUnits WHERE VersionID = @VersionID"

        ''' <summary>Deletes all levels for a version.</summary>
        Public Const DeleteLevelsByVersion As String =
            "DELETE FROM Levels WHERE VersionID = @VersionID"

        ''' <summary>Deletes all buildings for a version.</summary>
        Public Const DeleteBuildingsByVersion As String =
            "DELETE FROM Buildings WHERE VersionID = @VersionID"

        ''' <summary>Deletes all product settings for a version.</summary>
        Public Const DeleteProjectProductSettingsByVersion As String =
            "DELETE FROM ProjectProductSettings WHERE VersionID = @VersionID"

        ''' <summary>Deletes all versions for a project.</summary>
        Public Const DeleteProjectVersionsByProject As String =
            "DELETE FROM ProjectVersions WHERE ProjectID = @ProjectID"

        ''' <summary>Deletes design info for a project.</summary>
        Public Const DeleteProjectDesignInfoByProject As String =
            "DELETE FROM ProjectDesignInfo WHERE ProjectID = @ProjectID"

        ''' <summary>Deletes all loads for a project.</summary>
        Public Const DeleteProjectLoadsByProject As String =
            "DELETE FROM ProjectLoads WHERE ProjectID = @ProjectID"

        ''' <summary>Deletes bearing styles for a project.</summary>
        Public Const DeleteProjectBearingStylesByProject As String =
            "DELETE FROM ProjectBearingStyles WHERE ProjectID = @ProjectID"

        ''' <summary>Deletes general notes for a project.</summary>
        Public Const DeleteProjectGeneralNotesByProject As String =
            "DELETE FROM ProjectGeneralNotes WHERE ProjectID = @ProjectID"

        ''' <summary>Deletes all items for a project.</summary>
        Public Const DeleteProjectItemsByProject As String =
            "DELETE FROM ProjectItems WHERE ProjectID = @ProjectID"

        ''' <summary>Deletes a project by ID (call after all child records are deleted).</summary>
        Public Const DeleteProjectByID As String =
            "DELETE FROM Projects WHERE ProjectID = @ProjectID"

#End Region

    End Module
End Namespace
