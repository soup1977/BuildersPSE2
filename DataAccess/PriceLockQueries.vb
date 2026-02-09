' =====================================================
' PriceLockQueries.vb
' SQL Query Constants for PriceLock Module
' BuildersPSE2 Extension
' =====================================================
' ALL SQL queries for PriceLock module MUST be defined here
' NEVER use inline SQL in forms or other classes
' =====================================================

Option Strict On
Option Explicit On

Namespace DataAccess

    ''' <summary>
    ''' Contains all SQL query constants for the PriceLock module.
    ''' Organized by entity type with XML documentation.
    ''' </summary>
    Public NotInheritable Class PriceLockQueries

#Region "Builders"

        ''' <summary>Select all active builders ordered by name</summary>
        Public Const SelectBuilders As String =
            "SELECT BuilderID, BuilderName, BuilderCode, IsActive, CreatedDate, ModifiedDate
             FROM PL_Builders
             WHERE IsActive = 1
             ORDER BY BuilderName"

        ''' <summary>Select all builders including inactive</summary>
        Public Const SelectAllBuilders As String =
            "SELECT BuilderID, BuilderName, BuilderCode, IsActive, CreatedDate, ModifiedDate
             FROM PL_Builders
             ORDER BY BuilderName"

        ''' <summary>Select single builder by ID</summary>
        Public Const SelectBuilderByID As String =
            "SELECT BuilderID, BuilderName, BuilderCode, IsActive, CreatedDate, ModifiedDate
             FROM PL_Builders
             WHERE BuilderID = @BuilderID"

        ''' <summary>Insert new builder, returns new BuilderID</summary>
        Public Const InsertBuilder As String =
            "INSERT INTO PL_Builders (BuilderName, BuilderCode, IsActive, CreatedDate, ModifiedDate)
             OUTPUT INSERTED.BuilderID
             VALUES (@BuilderName, @BuilderCode, 1, GETDATE(), GETDATE())"

        ''' <summary>Update existing builder</summary>
        Public Const UpdateBuilder As String =
            "UPDATE PL_Builders
             SET BuilderName = @BuilderName,
                 BuilderCode = @BuilderCode,
                 IsActive = @IsActive,
                 ModifiedDate = GETDATE()
             WHERE BuilderID = @BuilderID"

        ''' <summary>Soft delete builder (set inactive)</summary>
        Public Const DeactivateBuilder As String =
            "UPDATE PL_Builders
             SET IsActive = 0, ModifiedDate = GETDATE()
             WHERE BuilderID = @BuilderID"

#End Region

#Region "Subdivisions"

        ''' <summary>Select all active subdivisions for a builder</summary>
        Public Const SelectSubdivisionsByBuilder As String =
            "SELECT s.SubdivisionID, s.BuilderID, s.SubdivisionName, s.SubdivisionCode,
                    s.IsActive, s.CreatedDate, s.ModifiedDate, b.BuilderName
             FROM PL_Subdivisions s
             INNER JOIN PL_Builders b ON s.BuilderID = b.BuilderID
             WHERE s.BuilderID = @BuilderID AND s.IsActive = 1
             ORDER BY s.SubdivisionName"

        ''' <summary>Select all active subdivisions across all builders</summary>
        Public Const SelectAllActiveSubdivisions As String =
            "SELECT s.SubdivisionID, s.BuilderID, s.SubdivisionName, s.SubdivisionCode,
                    s.IsActive, s.CreatedDate, s.ModifiedDate, b.BuilderName
             FROM PL_Subdivisions s
             INNER JOIN PL_Builders b ON s.BuilderID = b.BuilderID
             WHERE s.IsActive = 1
             ORDER BY b.BuilderName, s.SubdivisionName"

        ''' <summary>Select single subdivision by ID</summary>
        Public Const SelectSubdivisionByID As String =
            "SELECT s.SubdivisionID, s.BuilderID, s.SubdivisionName, s.SubdivisionCode,
                    s.IsActive, s.CreatedDate, s.ModifiedDate, b.BuilderName
             FROM PL_Subdivisions s
             INNER JOIN PL_Builders b ON s.BuilderID = b.BuilderID
             WHERE s.SubdivisionID = @SubdivisionID"

        ''' <summary>Insert new subdivision, returns new SubdivisionID</summary>
        Public Const InsertSubdivision As String =
            "INSERT INTO PL_Subdivisions (BuilderID, SubdivisionName, SubdivisionCode, IsActive, CreatedDate, ModifiedDate)
             OUTPUT INSERTED.SubdivisionID
             VALUES (@BuilderID, @SubdivisionName, @SubdivisionCode, 1, GETDATE(), GETDATE())"

        ''' <summary>Update existing subdivision</summary>
        Public Const UpdateSubdivision As String =
            "UPDATE PL_Subdivisions
             SET SubdivisionName = @SubdivisionName,
                 SubdivisionCode = @SubdivisionCode,
                 BuilderID = @BuilderID,
                 IsActive = @IsActive,
                 ModifiedDate = GETDATE()
             WHERE SubdivisionID = @SubdivisionID"

        ''' <summary>Soft delete subdivision</summary>
        Public Const DeactivateSubdivision As String =
            "UPDATE PL_Subdivisions
             SET IsActive = 0, ModifiedDate = GETDATE()
             WHERE SubdivisionID = @SubdivisionID"

#End Region

#Region "Plans"

        ''' <summary>Select all active plans</summary>
        Public Const SelectPlans As String =
            "SELECT PlanID, PlanName, PlanDescription, SquareFootage, IsActive, CreatedDate, ModifiedDate
             FROM PL_Plans
             WHERE IsActive = 1
             ORDER BY PlanName"

        ''' <summary>Select plans assigned to a specific subdivision</summary>
        Public Const SelectPlansBySubdivision As String =
            "SELECT p.PlanID, p.PlanName, p.PlanDescription, p.SquareFootage, 
                    p.IsActive, p.CreatedDate, p.ModifiedDate, sp.SubdivisionPlanID
             FROM PL_Plans p
             INNER JOIN PL_SubdivisionPlans sp ON p.PlanID = sp.PlanID
             WHERE sp.SubdivisionID = @SubdivisionID AND sp.IsActive = 1 AND p.IsActive = 1
             ORDER BY p.PlanName"

        ''' <summary>Select single plan by ID</summary>
        Public Const SelectPlanByID As String =
            "SELECT PlanID, PlanName, PlanDescription, SquareFootage, IsActive, CreatedDate, ModifiedDate
             FROM PL_Plans
             WHERE PlanID = @PlanID"

        ''' <summary>Insert new plan, returns new PlanID</summary>
        Public Const InsertPlan As String =
            "INSERT INTO PL_Plans (PlanName, PlanDescription, SquareFootage, IsActive, CreatedDate, ModifiedDate)
             OUTPUT INSERTED.PlanID
             VALUES (@PlanName, @PlanDescription, @SquareFootage, 1, GETDATE(), GETDATE())"

        ''' <summary>Update existing plan</summary>
        Public Const UpdatePlan As String =
            "UPDATE PL_Plans
             SET PlanName = @PlanName,
                 PlanDescription = @PlanDescription,
                 SquareFootage = @SquareFootage,
                 IsActive = @IsActive,
                 ModifiedDate = GETDATE()
             WHERE PlanID = @PlanID"

        ''' <summary>Assign plan to subdivision</summary>
        Public Const InsertSubdivisionPlan As String =
            "INSERT INTO PL_SubdivisionPlans (SubdivisionID, PlanID, IsActive, CreatedDate)
             OUTPUT INSERTED.SubdivisionPlanID
             VALUES (@SubdivisionID, @PlanID, 1, GETDATE())"

        ''' <summary>Remove plan from subdivision</summary>
        Public Const DeactivateSubdivisionPlan As String =
            "UPDATE PL_SubdivisionPlans
             SET IsActive = 0
             WHERE SubdivisionID = @SubdivisionID AND PlanID = @PlanID"

        ''' <summary>Check if plan exists in subdivision</summary>
        Public Const CheckSubdivisionPlanExists As String =
            "SELECT COUNT(*) FROM PL_SubdivisionPlans
             WHERE SubdivisionID = @SubdivisionID AND PlanID = @PlanID AND IsActive = 1"

#End Region

#Region "Elevations"

        ''' <summary>Select all elevations for a plan</summary>
        Public Const SelectElevationsByPlan As String =
            "SELECT ElevationID, PlanID, ElevationName, IsActive, CreatedDate, ModifiedDate
             FROM PL_Elevations
             WHERE PlanID = @PlanID AND IsActive = 1
             ORDER BY ElevationName"

        ''' <summary>Select single elevation by ID</summary>
        Public Const SelectElevationByID As String =
            "SELECT ElevationID, PlanID, ElevationName, IsActive, CreatedDate, ModifiedDate
             FROM PL_Elevations
             WHERE ElevationID = @ElevationID"

        ''' <summary>Insert new elevation, returns new ElevationID</summary>
        Public Const InsertElevation As String =
            "INSERT INTO PL_Elevations (PlanID, ElevationName, IsActive, CreatedDate, ModifiedDate)
             OUTPUT INSERTED.ElevationID
             VALUES (@PlanID, @ElevationName, 1, GETDATE(), GETDATE())"

        ''' <summary>Update existing elevation</summary>
        Public Const UpdateElevation As String =
            "UPDATE PL_Elevations
             SET ElevationName = @ElevationName,
                 IsActive = @IsActive,
                 ModifiedDate = GETDATE()
             WHERE ElevationID = @ElevationID"

#End Region

#Region "Options"

        ''' <summary>Select all active options</summary>
        Public Const SelectOptions As String =
            "SELECT OptionID, OptionName, OptionDescription, IsActive, CreatedDate, ModifiedDate
             FROM PL_Options
             WHERE IsActive = 1
             ORDER BY OptionName"

        ''' <summary>Select single option by ID</summary>
        Public Const SelectOptionByID As String =
            "SELECT OptionID, OptionName, OptionDescription, IsActive, CreatedDate, ModifiedDate
             FROM PL_Options
             WHERE OptionID = @OptionID"

        ''' <summary>Insert new option, returns new OptionID</summary>
        Public Const InsertOption As String =
            "INSERT INTO PL_Options (OptionName, OptionDescription, IsActive, CreatedDate, ModifiedDate)
             OUTPUT INSERTED.OptionID
             VALUES (@OptionName, @OptionDescription, 1, GETDATE(), GETDATE())"

        ''' <summary>Update existing option</summary>
        Public Const UpdateOption As String =
            "UPDATE PL_Options
             SET OptionName = @OptionName,
                 OptionDescription = @OptionDescription,
                 IsActive = @IsActive,
                 ModifiedDate = GETDATE()
             WHERE OptionID = @OptionID"

        ''' <summary>Find or create option by name, returns OptionID</summary>
        Public Const FindOrCreateOption As String =
            "IF NOT EXISTS (SELECT 1 FROM PL_Options WHERE OptionName = @OptionName)
             BEGIN
                 INSERT INTO PL_Options (OptionName, OptionDescription, IsActive, CreatedDate, ModifiedDate)
                 VALUES (@OptionName, @OptionDescription, 1, GETDATE(), GETDATE())
             END
             SELECT OptionID FROM PL_Options WHERE OptionName = @OptionName"

        ''' <summary>Merge duplicate options - reassigns all component pricing from source to target option</summary>
        Public Const MergeOptions As String =
            "-- Reassign all component pricing records from source option to target option
             UPDATE PL_ComponentPricing
             SET OptionID = @TargetOptionID,
                 ModifiedDate = GETDATE(),
                 ModifiedBy = @ModifiedBy
             WHERE OptionID = @SourceOptionID;
             
             -- Deactivate the source option (soft delete)
             UPDATE PL_Options
             SET IsActive = 0,
                 OptionDescription = ISNULL(OptionDescription, '') + ' [MERGED INTO OptionID ' + CAST(@TargetOptionID AS VARCHAR(10)) + ']',
                 ModifiedDate = GETDATE()
             WHERE OptionID = @SourceOptionID;
             
             -- Return count of records updated
             SELECT @@ROWCOUNT AS RecordsUpdated"

        ''' <summary>Get count of component pricing records using an option</summary>
        Public Const GetOptionUsageCount As String =
            "SELECT COUNT(*) FROM PL_ComponentPricing WHERE OptionID = @OptionID"

#End Region

#Region "Product Types"

        ''' <summary>Select all product types</summary>
        Public Const SelectProductTypes As String =
            "SELECT ProductTypeID, ProductTypeName
             FROM PL_ProductTypes
             ORDER BY ProductTypeID"

#End Region

#Region "Material Categories"

        ''' <summary>Select all active material categories</summary>
        Public Const SelectMaterialCategories As String =
            "SELECT MaterialCategoryID, CategoryName, CategoryCode, DisplayOrder, IsActive, CreatedDate
             FROM PL_MaterialCategories
             WHERE IsActive = 1
             ORDER BY DisplayOrder, CategoryName"

        ''' <summary>Select all material categories including inactive</summary>
        Public Const SelectAllMaterialCategories As String =
            "SELECT MaterialCategoryID, CategoryName, CategoryCode, DisplayOrder, IsActive, CreatedDate
             FROM PL_MaterialCategories
             ORDER BY DisplayOrder, CategoryName"

        ''' <summary>Insert new material category</summary>
        Public Const InsertMaterialCategory As String =
            "INSERT INTO PL_MaterialCategories (CategoryName, CategoryCode, DisplayOrder, IsActive, CreatedDate)
             OUTPUT INSERTED.MaterialCategoryID
             VALUES (@CategoryName, @CategoryCode, @DisplayOrder, 1, GETDATE())"

        ''' <summary>Update material category</summary>
        Public Const UpdateMaterialCategory As String =
            "UPDATE PL_MaterialCategories
             SET CategoryName = @CategoryName,
                 CategoryCode = @CategoryCode,
                 DisplayOrder = @DisplayOrder,
                 IsActive = @IsActive
             WHERE MaterialCategoryID = @MaterialCategoryID"

#End Region

#Region "Price Locks"

        ''' <summary>Select all price locks for a subdivision ordered by date descending</summary>
        Public Const SelectPriceLocksBySubdivision As String =
    "SELECT pl.PriceLockID, pl.SubdivisionID, pl.PriceLockDate, pl.PriceLockName,
            pl.BaseMgmtMargin, pl.AdjustedMarginBaseModels, pl.OptionMargin,
            pl.Status, pl.CreatedBy, pl.ApprovedBy, pl.ApprovedDate, pl.SentToBuilderDate,
            pl.BaselineRLImportID, pl.CurrentRLImportID,
            pl.CreatedDate, pl.ModifiedDate,
            s.SubdivisionName, s.SubdivisionCode, b.BuilderID, b.BuilderName
     FROM PL_PriceLocks pl
     INNER JOIN PL_Subdivisions s ON pl.SubdivisionID = s.SubdivisionID
     INNER JOIN PL_Builders b ON s.BuilderID = b.BuilderID
     WHERE pl.SubdivisionID = @SubdivisionID
     ORDER BY pl.PriceLockDate DESC"

        ''' <summary>Select all price locks with filtering options</summary>
        Public Const SelectPriceLocksFiltered As String =
    "SELECT pl.PriceLockID, pl.SubdivisionID, pl.PriceLockDate, pl.PriceLockName,
            pl.BaseMgmtMargin, pl.AdjustedMarginBaseModels, pl.OptionMargin,
            pl.Status, pl.CreatedBy, pl.ApprovedBy, pl.ApprovedDate, pl.SentToBuilderDate,
            pl.BaselineRLImportID, pl.CurrentRLImportID,
            pl.CreatedDate, pl.ModifiedDate,
            s.SubdivisionName, s.SubdivisionCode, b.BuilderID, b.BuilderName
     FROM PL_PriceLocks pl
     INNER JOIN PL_Subdivisions s ON pl.SubdivisionID = s.SubdivisionID
     INNER JOIN PL_Builders b ON s.BuilderID = b.BuilderID
     WHERE (@BuilderID IS NULL OR b.BuilderID = @BuilderID)
       AND (@SubdivisionID IS NULL OR pl.SubdivisionID = @SubdivisionID)
       AND (@Status IS NULL OR pl.Status = @Status)
       AND (@StartDate IS NULL OR pl.PriceLockDate >= @StartDate)
       AND (@EndDate IS NULL OR pl.PriceLockDate <= @EndDate)
     ORDER BY pl.PriceLockDate DESC, b.BuilderName, s.SubdivisionName"

        ''' <summary>Select single price lock by ID with full details</summary>
        Public Const SelectPriceLockByID As String =
    "SELECT pl.PriceLockID, pl.SubdivisionID, pl.PriceLockDate, pl.PriceLockName,
            pl.BaseMgmtMargin, pl.AdjustedMarginBaseModels, pl.OptionMargin,
            pl.Status, pl.CreatedBy, pl.ApprovedBy, pl.ApprovedDate, pl.SentToBuilderDate,
            pl.BaselineRLImportID, pl.CurrentRLImportID,
            pl.CreatedDate, pl.ModifiedDate,
            s.SubdivisionName, s.SubdivisionCode, b.BuilderID, b.BuilderName
     FROM PL_PriceLocks pl
     INNER JOIN PL_Subdivisions s ON pl.SubdivisionID = s.SubdivisionID
     INNER JOIN PL_Builders b ON s.BuilderID = b.BuilderID
     WHERE pl.PriceLockID = @PriceLockID"

        ''' <summary>Select most recent price lock for a subdivision</summary>
        Public Const SelectLatestPriceLock As String =
    "SELECT TOP 1 pl.PriceLockID, pl.SubdivisionID, pl.PriceLockDate, pl.PriceLockName,
            pl.BaseMgmtMargin, pl.AdjustedMarginBaseModels, pl.OptionMargin,
            pl.Status, pl.CreatedBy, pl.ApprovedBy, pl.ApprovedDate, pl.SentToBuilderDate,
            pl.BaselineRLImportID, pl.CurrentRLImportID,
            pl.CreatedDate, pl.ModifiedDate,
            s.SubdivisionName, s.SubdivisionCode, b.BuilderID, b.BuilderName
     FROM PL_PriceLocks pl
     INNER JOIN PL_Subdivisions s ON pl.SubdivisionID = s.SubdivisionID
     INNER JOIN PL_Builders b ON s.BuilderID = b.BuilderID
     WHERE pl.SubdivisionID = @SubdivisionID
     ORDER BY pl.PriceLockDate DESC"

        ''' <summary>Select previous price lock (for comparison)</summary>
        Public Const SelectPreviousPriceLock As String =
            "SELECT TOP 1 pl.PriceLockID, pl.SubdivisionID, pl.PriceLockDate, pl.PriceLockName,
                    pl.BaseMgmtMargin, pl.AdjustedMarginBaseModels, pl.OptionMargin,
                    pl.Status
             FROM PL_PriceLocks pl
             WHERE pl.SubdivisionID = @SubdivisionID
               AND pl.PriceLockDate < @CurrentLockDate
             ORDER BY pl.PriceLockDate DESC"

        ''' <summary>Insert new price lock, returns new PriceLockID</summary>
        Public Const InsertPriceLock As String =
            "INSERT INTO PL_PriceLocks (SubdivisionID, PriceLockDate, PriceLockName,
                    BaseMgmtMargin, AdjustedMarginBaseModels, OptionMargin,
                    Status, CreatedBy, CreatedDate, ModifiedDate)
             OUTPUT INSERTED.PriceLockID
             VALUES (@SubdivisionID, @PriceLockDate, @PriceLockName,
                    @BaseMgmtMargin, @AdjustedMarginBaseModels, @OptionMargin,
                    @Status, @CreatedBy, GETDATE(), GETDATE())"

        ''' <summary>Update price lock header</summary>
        Public Const UpdatePriceLock As String =
    "UPDATE PL_PriceLocks
     SET PriceLockDate = @PriceLockDate,
         PriceLockName = @PriceLockName,
         BaseMgmtMargin = @BaseMgmtMargin,
         AdjustedMarginBaseModels = @AdjustedMarginBaseModels,
         OptionMargin = @OptionMargin,
         Status = @Status,
         BaselineRLImportID = @BaselineRLImportID,
         CurrentRLImportID = @CurrentRLImportID,
         ModifiedDate = GETDATE()
     WHERE PriceLockID = @PriceLockID"

        ''' <summary>Update price lock status</summary>
        Public Const UpdatePriceLockStatus As String =
            "UPDATE PL_PriceLocks
             SET Status = @Status,
                 ApprovedBy = CASE WHEN @Status = 'Approved' THEN @ApprovedBy ELSE ApprovedBy END,
                 ApprovedDate = CASE WHEN @Status = 'Approved' THEN GETDATE() ELSE ApprovedDate END,
                 SentToBuilderDate = CASE WHEN @Status = 'Sent' THEN GETDATE() ELSE SentToBuilderDate END,
                 ModifiedDate = GETDATE()
             WHERE PriceLockID = @PriceLockID"

        ''' <summary>Delete price lock (only if Draft status)</summary>
        Public Const DeletePriceLock As String =
            "DELETE FROM PL_PriceLocks
             WHERE PriceLockID = @PriceLockID AND Status = 'Draft'"

        ''' <summary>Check if price lock exists for subdivision/date combination</summary>
        Public Const CheckPriceLockExists As String =
            "SELECT COUNT(*) FROM PL_PriceLocks
             WHERE SubdivisionID = @SubdivisionID AND PriceLockDate = @PriceLockDate
               AND (@PriceLockID IS NULL OR PriceLockID <> @PriceLockID)"

#End Region

#Region "Component Pricing"

        ''' <summary>Select all component pricing for a price lock</summary>
        Public Const SelectComponentPricingByLock As String =
    "SELECT cp.ComponentPricingID, cp.PriceLockID, cp.PlanID, cp.ElevationID, cp.OptionID,
            cp.ProductTypeID, cp.Cost, cp.MgmtSellPrice, cp.CalculatedPrice, cp.AppliedMargin,
            cp.FinalPrice, cp.PriceSentToSales, cp.PriceSentToBuilder,
            cp.IsAdder, cp.BaseElevationID, cp.BaseComponentPricingID, cp.PriceNote, cp.MarginSource,
            cp.CreatedDate, cp.ModifiedDate, cp.ModifiedBy,
            p.PlanName, e.ElevationName, o.OptionName, pt.ProductTypeName,
            be.ElevationName AS BaseElevationName,
            basecp.Cost AS BaseComponentCost,
            basep.PlanName + ' ' + basee.ElevationName + ' (' + basept.ProductTypeName + ')' AS BaseComponentDescription
     FROM PL_ComponentPricing cp
     INNER JOIN PL_Plans p ON cp.PlanID = p.PlanID
     INNER JOIN PL_Elevations e ON cp.ElevationID = e.ElevationID
     INNER JOIN PL_ProductTypes pt ON cp.ProductTypeID = pt.ProductTypeID
     LEFT JOIN PL_Options o ON cp.OptionID = o.OptionID
     LEFT JOIN PL_Elevations be ON cp.BaseElevationID = be.ElevationID
     LEFT JOIN PL_ComponentPricing basecp ON cp.BaseComponentPricingID = basecp.ComponentPricingID
     LEFT JOIN PL_Plans basep ON basecp.PlanID = basep.PlanID
     LEFT JOIN PL_Elevations basee ON basecp.ElevationID = basee.ElevationID
     LEFT JOIN PL_ProductTypes basept ON basecp.ProductTypeID = basept.ProductTypeID
     WHERE cp.PriceLockID = @PriceLockID
     ORDER BY p.PlanName, e.ElevationName, o.OptionName, pt.ProductTypeName"

        ' =====================================================
        ' ADD THIS QUERY after SelectComponentPricingByLock (around line 340)
        ' =====================================================

        ''' <summary>Select all component pricing for a price lock WITH previous price lock comparison data</summary>
        Public Const SelectComponentPricingByLockWithPrevious As String =
    "-- First, get the previous price lock ID for this subdivision
     ;WITH PrevLock AS (
         SELECT TOP 1 prev.PriceLockID AS PrevPriceLockID
         FROM PL_PriceLocks curr
         INNER JOIN PL_PriceLocks prev 
             ON prev.SubdivisionID = curr.SubdivisionID
             AND prev.PriceLockDate < curr.PriceLockDate
         WHERE curr.PriceLockID = @PriceLockID
         ORDER BY prev.PriceLockDate DESC
     )
     SELECT cp.ComponentPricingID, cp.PriceLockID, cp.PlanID, cp.ElevationID, cp.OptionID,
            cp.ProductTypeID, cp.Cost, cp.MgmtSellPrice, cp.CalculatedPrice, cp.AppliedMargin,
            cp.FinalPrice, cp.PriceSentToSales, cp.PriceSentToBuilder,
            cp.IsAdder, cp.BaseElevationID, cp.BaseComponentPricingID, cp.PriceNote, cp.MarginSource,
            cp.CreatedDate, cp.ModifiedDate, cp.ModifiedBy,
            p.PlanName, e.ElevationName, o.OptionName, pt.ProductTypeName,
            be.ElevationName AS BaseElevationName,
            basecp.Cost AS BaseComponentCost,
            basep.PlanName + ' ' + basee.ElevationName + ' (' + basept.ProductTypeName + ')' AS BaseComponentDescription,
            -- Previous price lock comparison fields
            prevcp.PriceSentToSales AS PreviousPriceSentToSales,
            prevcp.PriceSentToBuilder AS PreviousPriceSentToBuilder,
            -- Calculate percent changes
            CASE 
                WHEN prevcp.PriceSentToSales IS NULL OR prevcp.PriceSentToSales = 0 THEN NULL
                ELSE (cp.PriceSentToSales - prevcp.PriceSentToSales) / prevcp.PriceSentToSales 
            END AS PctChangeSentToSales,
            CASE 
                WHEN prevcp.PriceSentToBuilder IS NULL OR prevcp.PriceSentToBuilder = 0 THEN NULL
                ELSE (cp.PriceSentToBuilder - prevcp.PriceSentToBuilder) / prevcp.PriceSentToBuilder 
            END AS PctChangeSentToBuilder
     FROM PL_ComponentPricing cp
     INNER JOIN PL_Plans p ON cp.PlanID = p.PlanID
     INNER JOIN PL_Elevations e ON cp.ElevationID = e.ElevationID
     INNER JOIN PL_ProductTypes pt ON cp.ProductTypeID = pt.ProductTypeID
     LEFT JOIN PL_Options o ON cp.OptionID = o.OptionID
     LEFT JOIN PL_Elevations be ON cp.BaseElevationID = be.ElevationID
     LEFT JOIN PL_ComponentPricing basecp ON cp.BaseComponentPricingID = basecp.ComponentPricingID
     LEFT JOIN PL_Plans basep ON basecp.PlanID = basep.PlanID
     LEFT JOIN PL_Elevations basee ON basecp.ElevationID = basee.ElevationID
     LEFT JOIN PL_ProductTypes basept ON basecp.ProductTypeID = basept.ProductTypeID
     -- Join to previous price lock's component pricing (matching by business key)
     LEFT JOIN PrevLock ON 1=1
     LEFT JOIN PL_ComponentPricing prevcp 
         ON prevcp.PriceLockID = PrevLock.PrevPriceLockID
         AND prevcp.PlanID = cp.PlanID
         AND prevcp.ElevationID = cp.ElevationID
         AND ISNULL(prevcp.OptionID, 0) = ISNULL(cp.OptionID, 0)
         AND prevcp.ProductTypeID = cp.ProductTypeID
     WHERE cp.PriceLockID = @PriceLockID
     ORDER BY p.PlanName, e.ElevationName, o.OptionName, pt.ProductTypeName"


        ''' <summary>Select component pricing for a specific plan within a price lock</summary>
        Public Const SelectComponentPricingByPlan As String =
    "SELECT cp.ComponentPricingID, cp.PriceLockID, cp.PlanID, cp.ElevationID, cp.OptionID,
            cp.ProductTypeID, cp.Cost, cp.MgmtSellPrice, cp.CalculatedPrice, cp.AppliedMargin,
            cp.FinalPrice, cp.PriceSentToSales, cp.PriceSentToBuilder,
            cp.IsAdder, cp.BaseElevationID, cp.PriceNote, cp.MarginSource,
            cp.CreatedDate, cp.ModifiedDate, cp.ModifiedBy,
            p.PlanName, e.ElevationName, o.OptionName, pt.ProductTypeName
     FROM PL_ComponentPricing cp
     INNER JOIN PL_Plans p ON cp.PlanID = p.PlanID
     INNER JOIN PL_Elevations e ON cp.ElevationID = e.ElevationID
     INNER JOIN PL_ProductTypes pt ON cp.ProductTypeID = pt.ProductTypeID
     LEFT JOIN PL_Options o ON cp.OptionID = o.OptionID
     WHERE cp.PriceLockID = @PriceLockID AND cp.PlanID = @PlanID
     ORDER BY e.ElevationName, o.OptionName, pt.ProductTypeName"

        ''' <summary>Select single component pricing record</summary>
        Public Const SelectComponentPricingByID As String =
    "SELECT cp.ComponentPricingID, cp.PriceLockID, cp.PlanID, cp.ElevationID, cp.OptionID,
            cp.ProductTypeID, cp.Cost, cp.MgmtSellPrice, cp.CalculatedPrice, cp.AppliedMargin,
            cp.FinalPrice, cp.PriceSentToSales, cp.PriceSentToBuilder,
            cp.IsAdder, cp.BaseElevationID, cp.BaseComponentPricingID, cp.PriceNote, cp.MarginSource,
            cp.CreatedDate, cp.ModifiedDate, cp.ModifiedBy,
            p.PlanName, e.ElevationName, o.OptionName, pt.ProductTypeName,
            basecp.Cost AS BaseComponentCost,
            basep.PlanName + ' ' + basee.ElevationName + ' (' + basept.ProductTypeName + ')' AS BaseComponentDescription
     FROM PL_ComponentPricing cp
     INNER JOIN PL_Plans p ON cp.PlanID = p.PlanID
     INNER JOIN PL_Elevations e ON cp.ElevationID = e.ElevationID
     INNER JOIN PL_ProductTypes pt ON cp.ProductTypeID = pt.ProductTypeID
     LEFT JOIN PL_Options o ON cp.OptionID = o.OptionID
     LEFT JOIN PL_ComponentPricing basecp ON cp.BaseComponentPricingID = basecp.ComponentPricingID
     LEFT JOIN PL_Plans basep ON basecp.PlanID = basep.PlanID
     LEFT JOIN PL_Elevations basee ON basecp.ElevationID = basee.ElevationID
     LEFT JOIN PL_ProductTypes basept ON basecp.ProductTypeID = basept.ProductTypeID
     WHERE cp.ComponentPricingID = @ComponentPricingID"

        ''' <summary>Insert new component pricing record</summary>
        Public Const InsertComponentPricing As String =
    "INSERT INTO PL_ComponentPricing (PriceLockID, PlanID, ElevationID, OptionID, ProductTypeID,
            Cost, MgmtSellPrice, CalculatedPrice, AppliedMargin,
            FinalPrice, PriceSentToSales, PriceSentToBuilder,
            IsAdder, BaseElevationID, BaseComponentPricingID, PriceNote, MarginSource, CreatedDate, ModifiedDate, ModifiedBy)
     OUTPUT INSERTED.ComponentPricingID
     VALUES (@PriceLockID, @PlanID, @ElevationID, @OptionID, @ProductTypeID,
            @Cost, @MgmtSellPrice, @CalculatedPrice, @AppliedMargin,
            @FinalPrice, @PriceSentToSales, @PriceSentToBuilder,
            @IsAdder, @BaseElevationID, @BaseComponentPricingID, @PriceNote, @MarginSource, GETDATE(), GETDATE(), @ModifiedBy)"

        ''' <summary>Update component pricing record</summary>
        Public Const UpdateComponentPricing As String =
    "UPDATE PL_ComponentPricing
     SET Cost = @Cost,
         MgmtSellPrice = @MgmtSellPrice,
         CalculatedPrice = @CalculatedPrice,
         AppliedMargin = @AppliedMargin,
         FinalPrice = @FinalPrice,
         PriceSentToSales = @PriceSentToSales,
         PriceSentToBuilder = @PriceSentToBuilder,
         IsAdder = @IsAdder,
         BaseElevationID = @BaseElevationID,
         BaseComponentPricingID = @BaseComponentPricingID,
         PriceNote = @PriceNote,
         MarginSource = @MarginSource,
         ModifiedDate = GETDATE(),
         ModifiedBy = @ModifiedBy
     WHERE ComponentPricingID = @ComponentPricingID"

        ''' <summary>Update only the pricing fields (for quick edits)</summary>
        Public Const UpdateComponentPricingPrices As String =
            "UPDATE PL_ComponentPricing
             SET FinalPrice = @FinalPrice,
                 PriceSentToSales = @FinalPrice,
                 PriceSentToBuilder = @PriceSentToBuilder,
                 PriceNote = @PriceNote,
                 ModifiedDate = GETDATE(),
                 ModifiedBy = @ModifiedBy
             WHERE ComponentPricingID = @ComponentPricingID"

        ''' <summary>Delete component pricing record</summary>
        Public Const DeleteComponentPricing As String =
            "DELETE FROM PL_ComponentPricing
             WHERE ComponentPricingID = @ComponentPricingID"

        ''' <summary>Delete all component pricing for a price lock</summary>
        Public Const DeleteComponentPricingByLock As String =
            "DELETE FROM PL_ComponentPricing
             WHERE PriceLockID = @PriceLockID"

        ''' <summary>Copy component pricing from one price lock to another</summary>
        Public Const CopyComponentPricing As String =
    "INSERT INTO PL_ComponentPricing (PriceLockID, PlanID, ElevationID, OptionID, ProductTypeID,
            Cost, MgmtSellPrice, CalculatedPrice, AppliedMargin,
            FinalPrice, PriceSentToSales, PriceSentToBuilder,
            IsAdder, BaseElevationID, BaseComponentPricingID, PriceNote, MarginSource, 
            CreatedDate, ModifiedDate, ModifiedBy)
     SELECT @NewPriceLockID, PlanID, ElevationID, OptionID, ProductTypeID,
            Cost, MgmtSellPrice, CalculatedPrice, AppliedMargin,
            FinalPrice, PriceSentToSales, PriceSentToBuilder,
            IsAdder, BaseElevationID, BaseComponentPricingID, PriceNote, MarginSource, 
            GETDATE(), GETDATE(), @ModifiedBy
     FROM PL_ComponentPricing
     WHERE PriceLockID = @SourcePriceLockID"

        ' Update component pricing when margin changes
        Public Const UpdateComponentPricingForMarginChange As String = "
    UPDATE cp
    SET 
        cp.AppliedMargin = @NewMargin,
        -- For NON-ADDERS: Standard margin calculation
        cp.CalculatedPrice = CASE 
            WHEN cp.IsAdder = 0 AND cp.Cost IS NOT NULL AND cp.Cost > 0 
            THEN cp.Cost / (1 - @NewMargin)
            WHEN cp.IsAdder = 0 
            THEN cp.CalculatedPrice
            -- For ADDERS: Use incremental cost (AdderCost - BaseCost)
            WHEN cp.IsAdder = 1 AND cp.Cost IS NOT NULL AND basecp.Cost IS NOT NULL
            THEN (cp.Cost - basecp.Cost) / (1 - @NewMargin)
            ELSE cp.CalculatedPrice
        END,
        cp.FinalPrice = CASE 
            WHEN cp.IsAdder = 0 AND cp.Cost IS NOT NULL AND cp.Cost > 0 
            THEN cp.Cost / (1 - @NewMargin)
            WHEN cp.IsAdder = 0 
            THEN cp.FinalPrice
            -- For ADDERS: Use incremental cost
            WHEN cp.IsAdder = 1 AND cp.Cost IS NOT NULL AND basecp.Cost IS NOT NULL
            THEN (cp.Cost - basecp.Cost) / (1 - @NewMargin)
            ELSE cp.FinalPrice
        END,
        cp.PriceSentToBuilder = CASE 
            WHEN cp.IsAdder = 0 AND cp.Cost IS NOT NULL AND cp.Cost > 0 
            THEN cp.Cost / (1 - @NewMargin)
            WHEN cp.IsAdder = 0 
            THEN cp.PriceSentToBuilder
            -- For ADDERS: Use incremental cost
            WHEN cp.IsAdder = 1 AND cp.Cost IS NOT NULL AND basecp.Cost IS NOT NULL
            THEN (cp.Cost - basecp.Cost) / (1 - @NewMargin)
            ELSE cp.PriceSentToBuilder
        END,
        cp.ModifiedBy = @ModifiedBy,
        cp.ModifiedDate = GETDATE()
    FROM PL_ComponentPricing cp
    LEFT JOIN PL_ComponentPricing basecp ON cp.BaseComponentPricingID = basecp.ComponentPricingID
    WHERE cp.PriceLockID = @PriceLockID
      AND (
            (@MarginType = 'Adjusted' AND (cp.MarginSource = 'Adjusted' OR cp.MarginSource IS NULL) AND (cp.OptionID IS NULL OR cp.OptionID = 0))
            OR
            (@MarginType = 'Option' AND (cp.MarginSource = 'Option' OR (cp.OptionID IS NOT NULL AND cp.OptionID > 0)))
          )
"

        ' Update margin source for a component
        Public Const UpdateComponentMarginSource As String = "
            UPDATE PL_ComponentPricing
            SET 
                MarginSource = @MarginSource,
                ModifiedBy = @ModifiedBy,
                ModifiedDate = GETDATE()
            WHERE ComponentPricingID = @ComponentPricingID
        "
        ''' <summary>Select potential reference units for an adder (same plan, same product type, no option)</summary>
        Public Const SelectPotentialReferenceUnits As String =
            "SELECT cp.ComponentPricingID, cp.PlanID, cp.ElevationID, cp.ProductTypeID,
                    cp.Cost, cp.FinalPrice,
                    p.PlanName, e.ElevationName, pt.ProductTypeName,
                    p.PlanName + ' ' + e.ElevationName + ' (' + pt.ProductTypeName + ')' AS Description
             FROM PL_ComponentPricing cp
             INNER JOIN PL_Plans p ON cp.PlanID = p.PlanID
             INNER JOIN PL_Elevations e ON cp.ElevationID = e.ElevationID
             INNER JOIN PL_ProductTypes pt ON cp.ProductTypeID = pt.ProductTypeID
             WHERE cp.PriceLockID = @PriceLockID
               AND cp.PlanID = @PlanID
               AND cp.ProductTypeID = @ProductTypeID
               AND (cp.OptionID IS NULL OR cp.OptionID = 0)
               AND cp.IsAdder = 0
               AND cp.ComponentPricingID <> ISNULL(@ExcludeComponentPricingID, 0)
             ORDER BY e.ElevationName"

        ''' <summary>Get the base component cost for an adder</summary>
        Public Const SelectBaseComponentCost As String =
            "SELECT Cost FROM PL_ComponentPricing 
             WHERE ComponentPricingID = @BaseComponentPricingID"

        ''' <summary>Update the base component reference for an adder</summary>
        Public Const UpdateAdderBaseComponent As String =
            "UPDATE PL_ComponentPricing
             SET BaseComponentPricingID = @BaseComponentPricingID,
                 ModifiedBy = @ModifiedBy,
                 ModifiedDate = GETDATE()
             WHERE ComponentPricingID = @ComponentPricingID"

#End Region

#Region "Material Pricing"

        ''' <summary>Select all material pricing for a price lock</summary>
        Public Const SelectMaterialPricingByLock As String =
            "SELECT mp.MaterialPricingID, mp.PriceLockID, mp.MaterialCategoryID,
                    mp.RandomLengthsDate, mp.RandomLengthsPrice,
                    mp.CalculatedPrice, mp.PriceSentToSales, mp.PriceSentToBuilder,
                    mp.PctChangeFromPrevious, mp.PriceNote,
                    mp.CreatedDate, mp.ModifiedDate, mp.ModifiedBy,
                    mc.CategoryName, mc.CategoryCode, mc.DisplayOrder
             FROM PL_MaterialPricing mp
             INNER JOIN PL_MaterialCategories mc ON mp.MaterialCategoryID = mc.MaterialCategoryID
             WHERE mp.PriceLockID = @PriceLockID
             ORDER BY mc.DisplayOrder, mc.CategoryName"

        ''' <summary>Select single material pricing record</summary>
        Public Const SelectMaterialPricingByID As String =
            "SELECT mp.MaterialPricingID, mp.PriceLockID, mp.MaterialCategoryID,
                    mp.RandomLengthsDate, mp.RandomLengthsPrice,
                    mp.CalculatedPrice, mp.PriceSentToSales, mp.PriceSentToBuilder,
                    mp.PctChangeFromPrevious, mp.PriceNote,
                    mp.CreatedDate, mp.ModifiedDate, mp.ModifiedBy,
                    mc.CategoryName, mc.CategoryCode
             FROM PL_MaterialPricing mp
             INNER JOIN PL_MaterialCategories mc ON mp.MaterialCategoryID = mc.MaterialCategoryID
             WHERE mp.MaterialPricingID = @MaterialPricingID"

        ''' <summary>Insert new material pricing record</summary>
        Public Const InsertMaterialPricing As String =
            "INSERT INTO PL_MaterialPricing (PriceLockID, MaterialCategoryID,
                    RandomLengthsDate, RandomLengthsPrice,
                    CalculatedPrice, PriceSentToSales, PriceSentToBuilder,
                    PctChangeFromPrevious, PriceNote, CreatedDate, ModifiedDate, ModifiedBy)
             OUTPUT INSERTED.MaterialPricingID
             VALUES (@PriceLockID, @MaterialCategoryID,
                    @RandomLengthsDate, @RandomLengthsPrice,
                    @CalculatedPrice, @PriceSentToSales, @PriceSentToBuilder,
                    @PctChangeFromPrevious, @PriceNote, GETDATE(), GETDATE(), @ModifiedBy)"

        ''' <summary>Update material pricing record</summary>
        Public Const UpdateMaterialPricing As String =
            "UPDATE PL_MaterialPricing
             SET RandomLengthsDate = @RandomLengthsDate,
                 RandomLengthsPrice = @RandomLengthsPrice,
                 CalculatedPrice = @CalculatedPrice,
                 PriceSentToSales = @PriceSentToSales,
                 PriceSentToBuilder = @PriceSentToBuilder,
                 PctChangeFromPrevious = @PctChangeFromPrevious,
                 PriceNote = @PriceNote,
                 ModifiedDate = GETDATE(),
                 ModifiedBy = @ModifiedBy
             WHERE MaterialPricingID = @MaterialPricingID"

        ''' <summary>Update only the pricing fields (for quick edits)</summary>
        Public Const UpdateMaterialPricingPrices As String =
            "UPDATE PL_MaterialPricing
             SET CalculatedPrice = @CalculatedPrice,
                 PriceSentToSales = @CalculatedPrice,
                 PriceSentToBuilder = @PriceSentToBuilder,
                 PriceNote = @PriceNote,
                 ModifiedDate = GETDATE(),
                 ModifiedBy = @ModifiedBy
             WHERE MaterialPricingID = @MaterialPricingID"

        ''' <summary>Delete material pricing record</summary>
        Public Const DeleteMaterialPricing As String =
            "DELETE FROM PL_MaterialPricing
             WHERE MaterialPricingID = @MaterialPricingID"

        ''' <summary>Delete all material pricing for a price lock</summary>
        Public Const DeleteMaterialPricingByLock As String =
            "DELETE FROM PL_MaterialPricing
             WHERE PriceLockID = @PriceLockID"

        ''' <summary>Copy material pricing from one price lock to another</summary>
        Public Const CopyMaterialPricing As String =
    "INSERT INTO PL_MaterialPricing (PriceLockID, MaterialCategoryID,
            RandomLengthsDate, RandomLengthsPrice,
            CalculatedPrice, PriceSentToSales, PriceSentToBuilder,
            PctChangeFromPrevious, PriceNote,
            BaselineRLPrice, CurrentRLPrice, OriginalSellPrice, RLPercentChange,
            CreatedDate, ModifiedDate, ModifiedBy)
     SELECT @NewPriceLockID, MaterialCategoryID,
            RandomLengthsDate, RandomLengthsPrice,
            CalculatedPrice, PriceSentToSales, PriceSentToBuilder,
            PctChangeFromPrevious, PriceNote,
            BaselineRLPrice, CurrentRLPrice, OriginalSellPrice, RLPercentChange,
            GETDATE(), GETDATE(), @ModifiedBy
     FROM PL_MaterialPricing
     WHERE PriceLockID = @SourcePriceLockID"

        ''' <summary>Get previous material price for calculating percent change</summary>
        Public Const SelectPreviousMaterialPrice As String =
            "SELECT TOP 1 mp.CalculatedPrice
             FROM PL_MaterialPricing mp
             INNER JOIN PL_PriceLocks pl ON mp.PriceLockID = pl.PriceLockID
             WHERE pl.SubdivisionID = @SubdivisionID
               AND mp.MaterialCategoryID = @MaterialCategoryID
               AND pl.PriceLockDate < @CurrentLockDate
             ORDER BY pl.PriceLockDate DESC"

#End Region

#Region "Price Change History (Audit)"

        ''' <summary>Insert audit record for price change</summary>
        Public Const InsertPriceChangeHistory As String =
            "INSERT INTO PL_PriceChangeHistory (TableName, RecordID, FieldName, OldValue, NewValue, ChangedBy, ChangedDate, ChangeReason)
             VALUES (@TableName, @RecordID, @FieldName, @OldValue, @NewValue, @ChangedBy, GETDATE(), @ChangeReason)"

        ''' <summary>Select price change history for a record</summary>
        Public Const SelectPriceChangeHistory As String =
            "SELECT HistoryID, TableName, RecordID, FieldName, OldValue, NewValue, ChangedBy, ChangedDate, ChangeReason
             FROM PL_PriceChangeHistory
             WHERE TableName = @TableName AND RecordID = @RecordID
             ORDER BY ChangedDate DESC"

#End Region

#Region "Comparison Reports"

        ''' <summary>Compare component pricing between two price locks</summary>
        Public Const CompareComponentPricing As String =
            "SELECT 
                 p.PlanName, e.ElevationName, o.OptionName, pt.ProductTypeName,
                 curr.FinalPrice AS CurrentFinalPrice,
                 curr.PriceSentToBuilder AS CurrentBuilderPrice,
                 prev.FinalPrice AS PreviousFinalPrice,
                 prev.PriceSentToBuilder AS PreviousBuilderPrice,
                 curr.FinalPrice - ISNULL(prev.FinalPrice, 0) AS FinalPriceDiff,
                 CASE WHEN prev.FinalPrice IS NULL OR prev.FinalPrice = 0 THEN NULL
                      ELSE (curr.FinalPrice - prev.FinalPrice) / prev.FinalPrice END AS FinalPricePctChange,
                 curr.PriceNote
             FROM PL_ComponentPricing curr
             INNER JOIN PL_Plans p ON curr.PlanID = p.PlanID
             INNER JOIN PL_Elevations e ON curr.ElevationID = e.ElevationID
             INNER JOIN PL_ProductTypes pt ON curr.ProductTypeID = pt.ProductTypeID
             LEFT JOIN PL_Options o ON curr.OptionID = o.OptionID
             LEFT JOIN PL_ComponentPricing prev ON prev.PriceLockID = @PreviousPriceLockID
                 AND prev.PlanID = curr.PlanID
                 AND prev.ElevationID = curr.ElevationID
                 AND ISNULL(prev.OptionID, 0) = ISNULL(curr.OptionID, 0)
                 AND prev.ProductTypeID = curr.ProductTypeID
             WHERE curr.PriceLockID = @CurrentPriceLockID
             ORDER BY p.PlanName, e.ElevationName, o.OptionName, pt.ProductTypeName"

        ''' <summary>Compare material pricing between two price locks</summary>
        Public Const CompareMaterialPricing As String =
            "SELECT 
                 mc.CategoryName, mc.CategoryCode,
                 curr.CalculatedPrice AS CurrentPrice,
                 curr.PriceSentToBuilder AS CurrentBuilderPrice,
                 prev.CalculatedPrice AS PreviousPrice,
                 prev.PriceSentToBuilder AS PreviousBuilderPrice,
                 curr.CalculatedPrice - ISNULL(prev.CalculatedPrice, 0) AS PriceDiff,
                 curr.PctChangeFromPrevious,
                 curr.PriceNote
             FROM PL_MaterialPricing curr
             INNER JOIN PL_MaterialCategories mc ON curr.MaterialCategoryID = mc.MaterialCategoryID
             LEFT JOIN PL_MaterialPricing prev ON prev.PriceLockID = @PreviousPriceLockID
                 AND prev.MaterialCategoryID = curr.MaterialCategoryID
             WHERE curr.PriceLockID = @CurrentPriceLockID
             ORDER BY mc.DisplayOrder, mc.CategoryName"

        ''' <summary>Get subdivisions needing price lock renewal (days since last lock)</summary>
        Public Const SelectSubdivisionsNeedingRenewal As String =
            "SELECT s.SubdivisionID, s.SubdivisionName, s.SubdivisionCode,
                    b.BuilderID, b.BuilderName,
                    pl.PriceLockID, pl.PriceLockDate,
                    DATEDIFF(DAY, pl.PriceLockDate, GETDATE()) AS DaysSinceLock
             FROM PL_Subdivisions s
             INNER JOIN PL_Builders b ON s.BuilderID = b.BuilderID
             LEFT JOIN PL_PriceLocks pl ON s.SubdivisionID = pl.SubdivisionID
                 AND pl.PriceLockDate = (SELECT MAX(PriceLockDate) FROM PL_PriceLocks WHERE SubdivisionID = s.SubdivisionID)
             WHERE s.IsActive = 1
             ORDER BY DaysSinceLock DESC, b.BuilderName, s.SubdivisionName"

#End Region

    End Class

End Namespace