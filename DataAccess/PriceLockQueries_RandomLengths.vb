' =====================================================
' PriceLockQueries_RandomLengths.vb
' SQL Query Constants for Random Lengths Import
' BuildersPSE2 - PriceLock Module Extension
' =====================================================
' Add these to the existing PriceLockQueries.vb file
' =====================================================

Option Strict On
Option Explicit On

Namespace DataAccess

    Partial Public NotInheritable Class PriceLockQueries

#Region "Random Lengths Import"

        ''' <summary>Select all active RL imports ordered by date descending</summary>
        Public Const SelectRandomLengthsImports As String =
            "SELECT rli.RandomLengthsImportID, rli.ReportDate, rli.ReportName, 
                    rli.SourceFileName, rli.ImportMethod, rli.ImportedBy, rli.ImportedDate,
                    rli.Notes, rli.IsActive, rli.CreatedDate, rli.ModifiedDate, rli.ModifiedBy,
                    COUNT(rlp.RandomLengthsPricingID) AS PriceCount
             FROM PL_RandomLengthsImport rli
             LEFT JOIN PL_RandomLengthsPricing rlp ON rli.RandomLengthsImportID = rlp.RandomLengthsImportID
             WHERE rli.IsActive = 1
             GROUP BY rli.RandomLengthsImportID, rli.ReportDate, rli.ReportName, 
                    rli.SourceFileName, rli.ImportMethod, rli.ImportedBy, rli.ImportedDate,
                    rli.Notes, rli.IsActive, rli.CreatedDate, rli.ModifiedDate, rli.ModifiedBy
             ORDER BY rli.ReportDate DESC"

        ''' <summary>Select all RL imports including inactive</summary>
        Public Const SelectAllRandomLengthsImports As String =
            "SELECT rli.RandomLengthsImportID, rli.ReportDate, rli.ReportName, 
                    rli.SourceFileName, rli.ImportMethod, rli.ImportedBy, rli.ImportedDate,
                    rli.Notes, rli.IsActive, rli.CreatedDate, rli.ModifiedDate, rli.ModifiedBy,
                    COUNT(rlp.RandomLengthsPricingID) AS PriceCount
             FROM PL_RandomLengthsImport rli
             LEFT JOIN PL_RandomLengthsPricing rlp ON rli.RandomLengthsImportID = rlp.RandomLengthsImportID
             GROUP BY rli.RandomLengthsImportID, rli.ReportDate, rli.ReportName, 
                    rli.SourceFileName, rli.ImportMethod, rli.ImportedBy, rli.ImportedDate,
                    rli.Notes, rli.IsActive, rli.CreatedDate, rli.ModifiedDate, rli.ModifiedBy
             ORDER BY rli.ReportDate DESC"

        ''' <summary>Select single RL import by ID</summary>
        Public Const SelectRandomLengthsImportByID As String =
            "SELECT rli.RandomLengthsImportID, rli.ReportDate, rli.ReportName, 
                    rli.SourceFileName, rli.ImportMethod, rli.ImportedBy, rli.ImportedDate,
                    rli.Notes, rli.IsActive, rli.CreatedDate, rli.ModifiedDate, rli.ModifiedBy,
                    COUNT(rlp.RandomLengthsPricingID) AS PriceCount
             FROM PL_RandomLengthsImport rli
             LEFT JOIN PL_RandomLengthsPricing rlp ON rli.RandomLengthsImportID = rlp.RandomLengthsImportID
             WHERE rli.RandomLengthsImportID = @RandomLengthsImportID
             GROUP BY rli.RandomLengthsImportID, rli.ReportDate, rli.ReportName, 
                    rli.SourceFileName, rli.ImportMethod, rli.ImportedBy, rli.ImportedDate,
                    rli.Notes, rli.IsActive, rli.CreatedDate, rli.ModifiedDate, rli.ModifiedBy"

        ''' <summary>Select RL imports within a date range</summary>
        Public Const SelectRandomLengthsImportsByDateRange As String =
            "SELECT rli.RandomLengthsImportID, rli.ReportDate, rli.ReportName, 
                    rli.SourceFileName, rli.ImportMethod, rli.ImportedBy, rli.ImportedDate,
                    rli.Notes, rli.IsActive, rli.CreatedDate, rli.ModifiedDate, rli.ModifiedBy,
                    COUNT(rlp.RandomLengthsPricingID) AS PriceCount
             FROM PL_RandomLengthsImport rli
             LEFT JOIN PL_RandomLengthsPricing rlp ON rli.RandomLengthsImportID = rlp.RandomLengthsImportID
             WHERE rli.IsActive = 1
               AND rli.ReportDate >= @StartDate
               AND rli.ReportDate <= @EndDate
             GROUP BY rli.RandomLengthsImportID, rli.ReportDate, rli.ReportName, 
                    rli.SourceFileName, rli.ImportMethod, rli.ImportedBy, rli.ImportedDate,
                    rli.Notes, rli.IsActive, rli.CreatedDate, rli.ModifiedDate, rli.ModifiedBy
             ORDER BY rli.ReportDate DESC"

        ''' <summary>Insert new RL import, returns new ID</summary>
        Public Const InsertRandomLengthsImport As String =
            "INSERT INTO PL_RandomLengthsImport 
                    (ReportDate, ReportName, SourceFileName, ImportMethod, 
                     ImportedBy, ImportedDate, Notes, IsActive, CreatedDate, ModifiedDate, ModifiedBy)
             OUTPUT INSERTED.RandomLengthsImportID
             VALUES (@ReportDate, @ReportName, @SourceFileName, @ImportMethod,
                     @ImportedBy, GETDATE(), @Notes, 1, GETDATE(), GETDATE(), @ModifiedBy)"

        ''' <summary>Update RL import</summary>
        Public Const UpdateRandomLengthsImport As String =
            "UPDATE PL_RandomLengthsImport
             SET ReportDate = @ReportDate,
                 ReportName = @ReportName,
                 SourceFileName = @SourceFileName,
                 Notes = @Notes,
                 ModifiedDate = GETDATE(),
                 ModifiedBy = @ModifiedBy
             WHERE RandomLengthsImportID = @RandomLengthsImportID"

        ''' <summary>Soft delete RL import</summary>
        Public Const DeactivateRandomLengthsImport As String =
            "UPDATE PL_RandomLengthsImport
             SET IsActive = 0, ModifiedDate = GETDATE(), ModifiedBy = @ModifiedBy
             WHERE RandomLengthsImportID = @RandomLengthsImportID"

        ''' <summary>Check if RL import exists for a specific date</summary>
        Public Const CheckRandomLengthsImportExists As String =
            "SELECT COUNT(*) FROM PL_RandomLengthsImport
             WHERE ReportDate = @ReportDate AND IsActive = 1
               AND (@RandomLengthsImportID IS NULL OR RandomLengthsImportID <> @RandomLengthsImportID)"

#End Region

#Region "Random Lengths Pricing Type"

        ''' <summary>Select all active RL pricing types</summary>
        Public Const SelectRLPricingTypes As String =
            "SELECT RLPricingTypeID, TypeCode, TypeName, TypeDescription, 
                    CategoryGroup, DisplayOrder, IsActive
             FROM PL_RandomLengthsPricingType
             WHERE IsActive = 1
             ORDER BY DisplayOrder, CategoryGroup, TypeName"

        ''' <summary>Select all RL pricing types including inactive</summary>
        Public Const SelectAllRLPricingTypes As String =
            "SELECT RLPricingTypeID, TypeCode, TypeName, TypeDescription, 
                    CategoryGroup, DisplayOrder, IsActive
             FROM PL_RandomLengthsPricingType
             ORDER BY DisplayOrder, CategoryGroup, TypeName"

        ''' <summary>Select RL pricing types by category group</summary>
        Public Const SelectRLPricingTypesByGroup As String =
            "SELECT RLPricingTypeID, TypeCode, TypeName, TypeDescription, 
                    CategoryGroup, DisplayOrder, IsActive
             FROM PL_RandomLengthsPricingType
             WHERE CategoryGroup = @CategoryGroup AND IsActive = 1
             ORDER BY DisplayOrder, TypeName"

        ''' <summary>Select single RL pricing type by ID</summary>
        Public Const SelectRLPricingTypeByID As String =
            "SELECT RLPricingTypeID, TypeCode, TypeName, TypeDescription, 
                    CategoryGroup, DisplayOrder, IsActive
             FROM PL_RandomLengthsPricingType
             WHERE RLPricingTypeID = @RLPricingTypeID"

        ''' <summary>Select single RL pricing type by code</summary>
        Public Const SelectRLPricingTypeByCode As String =
            "SELECT RLPricingTypeID, TypeCode, TypeName, TypeDescription, 
                    CategoryGroup, DisplayOrder, IsActive
             FROM PL_RandomLengthsPricingType
             WHERE TypeCode = @TypeCode"

        ''' <summary>Get distinct category groups</summary>
        Public Const SelectRLPricingTypeCategoryGroups As String =
            "SELECT DISTINCT CategoryGroup
             FROM PL_RandomLengthsPricingType
             WHERE IsActive = 1
             ORDER BY CategoryGroup"

#End Region

#Region "Random Lengths Pricing"

        ''' <summary>Select all pricing for a specific RL import</summary>
        Public Const SelectRandomLengthsPricingByImport As String =
            "SELECT rlp.RandomLengthsPricingID, rlp.RandomLengthsImportID, rlp.RLPricingTypeID,
                    rlp.Price, rlp.PriceSource, rlp.Notes,
                    rlt.TypeCode, rlt.TypeName, rlt.CategoryGroup, rlt.DisplayOrder
             FROM PL_RandomLengthsPricing rlp
             INNER JOIN PL_RandomLengthsPricingType rlt ON rlp.RLPricingTypeID = rlt.RLPricingTypeID
             WHERE rlp.RandomLengthsImportID = @RandomLengthsImportID
             ORDER BY rlt.DisplayOrder, rlt.CategoryGroup, rlt.TypeName"

        ''' <summary>Select single pricing record by ID</summary>
        Public Const SelectRandomLengthsPricingByID As String =
            "SELECT rlp.RandomLengthsPricingID, rlp.RandomLengthsImportID, rlp.RLPricingTypeID,
                    rlp.Price, rlp.PriceSource, rlp.Notes,
                    rlt.TypeCode, rlt.TypeName, rlt.CategoryGroup, rlt.DisplayOrder
             FROM PL_RandomLengthsPricing rlp
             INNER JOIN PL_RandomLengthsPricingType rlt ON rlp.RLPricingTypeID = rlt.RLPricingTypeID
             WHERE rlp.RandomLengthsPricingID = @RandomLengthsPricingID"

        ''' <summary>Select pricing for a specific type across an import</summary>
        Public Const SelectRandomLengthsPricingByImportAndType As String =
            "SELECT rlp.RandomLengthsPricingID, rlp.RandomLengthsImportID, rlp.RLPricingTypeID,
                    rlp.Price, rlp.PriceSource, rlp.Notes,
                    rlt.TypeCode, rlt.TypeName, rlt.CategoryGroup, rlt.DisplayOrder
             FROM PL_RandomLengthsPricing rlp
             INNER JOIN PL_RandomLengthsPricingType rlt ON rlp.RLPricingTypeID = rlt.RLPricingTypeID
             WHERE rlp.RandomLengthsImportID = @RandomLengthsImportID
               AND rlp.RLPricingTypeID = @RLPricingTypeID"

        ''' <summary>Insert new RL pricing record</summary>
        Public Const InsertRandomLengthsPricing As String =
            "INSERT INTO PL_RandomLengthsPricing 
                    (RandomLengthsImportID, RLPricingTypeID, Price, PriceSource, Notes)
             OUTPUT INSERTED.RandomLengthsPricingID
             VALUES (@RandomLengthsImportID, @RLPricingTypeID, @Price, @PriceSource, @Notes)"

        ''' <summary>Update RL pricing record</summary>
        Public Const UpdateRandomLengthsPricing As String =
            "UPDATE PL_RandomLengthsPricing
             SET Price = @Price,
                 PriceSource = @PriceSource,
                 Notes = @Notes
             WHERE RandomLengthsPricingID = @RandomLengthsPricingID"

        ''' <summary>Upsert RL pricing (insert or update if exists)</summary>
        Public Const UpsertRandomLengthsPricing As String =
            "IF EXISTS (SELECT 1 FROM PL_RandomLengthsPricing 
                        WHERE RandomLengthsImportID = @RandomLengthsImportID 
                          AND RLPricingTypeID = @RLPricingTypeID)
             BEGIN
                 UPDATE PL_RandomLengthsPricing
                 SET Price = @Price, PriceSource = @PriceSource, Notes = @Notes
                 WHERE RandomLengthsImportID = @RandomLengthsImportID 
                   AND RLPricingTypeID = @RLPricingTypeID
                 
                 SELECT RandomLengthsPricingID FROM PL_RandomLengthsPricing
                 WHERE RandomLengthsImportID = @RandomLengthsImportID 
                   AND RLPricingTypeID = @RLPricingTypeID
             END
             ELSE
             BEGIN
                 INSERT INTO PL_RandomLengthsPricing 
                        (RandomLengthsImportID, RLPricingTypeID, Price, PriceSource, Notes)
                 OUTPUT INSERTED.RandomLengthsPricingID
                 VALUES (@RandomLengthsImportID, @RLPricingTypeID, @Price, @PriceSource, @Notes)
             END"

        ''' <summary>Delete RL pricing record</summary>
        Public Const DeleteRandomLengthsPricing As String =
            "DELETE FROM PL_RandomLengthsPricing
             WHERE RandomLengthsPricingID = @RandomLengthsPricingID"

        ''' <summary>Delete all pricing for an import</summary>
        Public Const DeleteRandomLengthsPricingByImport As String =
            "DELETE FROM PL_RandomLengthsPricing
             WHERE RandomLengthsImportID = @RandomLengthsImportID"

#End Region

#Region "Material Category RL Mapping"

        ''' <summary>Select all active mappings</summary>
        Public Const SelectMaterialCategoryRLMappings As String =
            "SELECT m.MappingID, m.MaterialCategoryID, m.RLPricingTypeID,
                    m.WeightFactor, m.IsPrimary, m.IsActive,
                    mc.CategoryName, rlt.TypeCode, rlt.TypeName
             FROM PL_MaterialCategoryRLMapping m
             INNER JOIN PL_MaterialCategories mc ON m.MaterialCategoryID = mc.MaterialCategoryID
             INNER JOIN PL_RandomLengthsPricingType rlt ON m.RLPricingTypeID = rlt.RLPricingTypeID
             WHERE m.IsActive = 1
             ORDER BY mc.DisplayOrder, mc.CategoryName, m.IsPrimary DESC"

        ''' <summary>Select mappings for a specific material category</summary>
        Public Const SelectMaterialCategoryRLMappingsByCategory As String =
            "SELECT m.MappingID, m.MaterialCategoryID, m.RLPricingTypeID,
                    m.WeightFactor, m.IsPrimary, m.IsActive,
                    mc.CategoryName, rlt.TypeCode, rlt.TypeName
             FROM PL_MaterialCategoryRLMapping m
             INNER JOIN PL_MaterialCategories mc ON m.MaterialCategoryID = mc.MaterialCategoryID
             INNER JOIN PL_RandomLengthsPricingType rlt ON m.RLPricingTypeID = rlt.RLPricingTypeID
             WHERE m.MaterialCategoryID = @MaterialCategoryID AND m.IsActive = 1
             ORDER BY m.IsPrimary DESC, rlt.TypeName"

        ''' <summary>Get weighted RL price for a material category from a specific import</summary>
        Public Const SelectMaterialCategoryWeightedRLPrice As String =
            "SELECT SUM(rlp.Price * m.WeightFactor) / NULLIF(SUM(m.WeightFactor), 0) AS WeightedPrice
             FROM PL_MaterialCategoryRLMapping m
             INNER JOIN PL_RandomLengthsPricing rlp 
                 ON m.RLPricingTypeID = rlp.RLPricingTypeID
                 AND rlp.RandomLengthsImportID = @RandomLengthsImportID
             WHERE m.MaterialCategoryID = @MaterialCategoryID
               AND m.IsActive = 1"

        ' ADD to the Material Category RL Mapping queries region:

        Public Const InsertMaterialCategoryRLMapping As String = "
    INSERT INTO PL_MaterialCategoryRLMapping (MaterialCategoryID, RLPricingTypeID, WeightFactor, IsPrimary, IsActive)
    VALUES (@MaterialCategoryID, @RLPricingTypeID, @WeightFactor, @IsPrimary, @IsActive);
    SELECT CAST(SCOPE_IDENTITY() AS INT);"

        Public Const UpdateMaterialCategoryRLMapping As String = "
    UPDATE PL_MaterialCategoryRLMapping 
    SET IsPrimary = @IsPrimary, 
        IsActive = @IsActive,
        WeightFactor = @WeightFactor
    WHERE MappingID = @MappingID"

        Public Const DeleteMaterialCategoryRLMapping As String = "
    DELETE FROM PL_MaterialCategoryRLMapping WHERE MappingID = @MappingID"

#End Region

#Region "Price Lock RL References"

        ''' <summary>Update price lock baseline RL import</summary>
        Public Const UpdatePriceLockBaselineRL As String =
            "UPDATE PL_PriceLocks
             SET BaselineRLImportID = @BaselineRLImportID,
                 ModifiedDate = GETDATE()
             WHERE PriceLockID = @PriceLockID"

        ''' <summary>Update price lock current RL import</summary>
        Public Const UpdatePriceLockCurrentRL As String =
            "UPDATE PL_PriceLocks
             SET CurrentRLImportID = @CurrentRLImportID,
                 ModifiedDate = GETDATE()
             WHERE PriceLockID = @PriceLockID"

        ''' <summary>Update both price lock RL imports</summary>
        Public Const UpdatePriceLockRLImports As String =
            "UPDATE PL_PriceLocks
             SET BaselineRLImportID = @BaselineRLImportID,
                 CurrentRLImportID = @CurrentRLImportID,
                 ModifiedDate = GETDATE()
             WHERE PriceLockID = @PriceLockID"

        ''' <summary>Select price lock with RL import details</summary>
        Public Const SelectPriceLockWithRLImports As String =
            "SELECT pl.PriceLockID, pl.SubdivisionID, pl.PriceLockDate, pl.PriceLockName,
                    pl.BaseMgmtMargin, pl.AdjustedMarginBaseModels, pl.OptionMargin,
                    pl.Status, pl.CreatedBy, pl.ApprovedBy, pl.ApprovedDate, pl.SentToBuilderDate,
                    pl.CreatedDate, pl.ModifiedDate,
                    pl.BaselineRLImportID, pl.CurrentRLImportID,
                    s.SubdivisionName, s.SubdivisionCode, b.BuilderID, b.BuilderName,
                    brl.ReportDate AS BaselineRLDate, brl.ReportName AS BaselineRLName,
                    crl.ReportDate AS CurrentRLDate, crl.ReportName AS CurrentRLName
             FROM PL_PriceLocks pl
             INNER JOIN PL_Subdivisions s ON pl.SubdivisionID = s.SubdivisionID
             INNER JOIN PL_Builders b ON s.BuilderID = b.BuilderID
             LEFT JOIN PL_RandomLengthsImport brl ON pl.BaselineRLImportID = brl.RandomLengthsImportID
             LEFT JOIN PL_RandomLengthsImport crl ON pl.CurrentRLImportID = crl.RandomLengthsImportID
             WHERE pl.PriceLockID = @PriceLockID"

#End Region

#Region "Material Pricing RL Calculations"

        ''' <summary>Update material pricing with RL calculation results</summary>
        Public Const UpdateMaterialPricingRLCalculation As String =
            "UPDATE PL_MaterialPricing
             SET BaselineRLPrice = @BaselineRLPrice,
                 CurrentRLPrice = @CurrentRLPrice,
                 RLPercentChange = @RLPercentChange,
                 CalculatedPrice = @CalculatedPrice,
                 PriceSentToSales = @CalculatedPrice,
                 ModifiedDate = GETDATE(),
                 ModifiedBy = @ModifiedBy
             WHERE MaterialPricingID = @MaterialPricingID"

        ''' <summary>Set original sell price for material pricing</summary>
        Public Const UpdateMaterialPricingOriginalSellPrice As String =
            "UPDATE PL_MaterialPricing
             SET OriginalSellPrice = @OriginalSellPrice,
                 ModifiedDate = GETDATE(),
                 ModifiedBy = @ModifiedBy
             WHERE MaterialPricingID = @MaterialPricingID"

        ''' <summary>Select material pricing with extended RL fields</summary>
        Public Const SelectMaterialPricingWithRL As String =
            "SELECT mp.MaterialPricingID, mp.PriceLockID, mp.MaterialCategoryID,
                    mp.RandomLengthsDate, mp.RandomLengthsPrice,
                    mp.BaselineRLPrice, mp.CurrentRLPrice, mp.OriginalSellPrice, mp.RLPercentChange,
                    mp.CalculatedPrice, mp.PriceSentToSales, mp.PriceSentToBuilder,
                    mp.PctChangeFromPrevious, mp.PriceNote,
                    mp.CreatedDate, mp.ModifiedDate, mp.ModifiedBy,
                    mc.CategoryName, mc.CategoryCode, mc.DisplayOrder
             FROM PL_MaterialPricing mp
             INNER JOIN PL_MaterialCategories mc ON mp.MaterialCategoryID = mc.MaterialCategoryID
             WHERE mp.PriceLockID = @PriceLockID
             ORDER BY mc.DisplayOrder, mc.CategoryName"

        ''' <summary>
        ''' Calculate percent change for all material categories between two RL imports.
        ''' Returns one row per material category with calculated values.
        ''' </summary>
        Public Const SelectRLPercentChangeAllCategories As String =
            "WITH BaselinePrices AS (
                SELECT m.MaterialCategoryID, 
                       SUM(rlp.Price * m.WeightFactor) / NULLIF(SUM(m.WeightFactor), 0) AS Price
                FROM PL_MaterialCategoryRLMapping m
                INNER JOIN PL_RandomLengthsPricing rlp 
                    ON m.RLPricingTypeID = rlp.RLPricingTypeID
                    AND rlp.RandomLengthsImportID = @BaselineRLImportID
                WHERE m.IsActive = 1
                GROUP BY m.MaterialCategoryID
            ),
            CurrentPrices AS (
                SELECT m.MaterialCategoryID, 
                       SUM(rlp.Price * m.WeightFactor) / NULLIF(SUM(m.WeightFactor), 0) AS Price
                FROM PL_MaterialCategoryRLMapping m
                INNER JOIN PL_RandomLengthsPricing rlp 
                    ON m.RLPricingTypeID = rlp.RLPricingTypeID
                    AND rlp.RandomLengthsImportID = @CurrentRLImportID
                WHERE m.IsActive = 1
                GROUP BY m.MaterialCategoryID
            )
            SELECT mc.MaterialCategoryID, mc.CategoryName, mc.DisplayOrder,
                   bp.Price AS BaselineRLPrice,
                   cp.Price AS CurrentRLPrice,
                   CASE WHEN bp.Price > 0 THEN (cp.Price - bp.Price) / bp.Price ELSE 0 END AS PercentChange
            FROM PL_MaterialCategories mc
            LEFT JOIN BaselinePrices bp ON mc.MaterialCategoryID = bp.MaterialCategoryID
            LEFT JOIN CurrentPrices cp ON mc.MaterialCategoryID = cp.MaterialCategoryID
            WHERE mc.IsActive = 1
            ORDER BY mc.DisplayOrder, mc.CategoryName"

#End Region

#Region "Comparison Between RL Imports"

        ''' <summary>Compare pricing between two RL imports</summary>
        Public Const CompareRandomLengthsImports As String =
            "SELECT rlt.RLPricingTypeID, rlt.TypeCode, rlt.TypeName, rlt.CategoryGroup,
                    base.Price AS BaselinePrice,
                    curr.Price AS CurrentPrice,
                    curr.Price - ISNULL(base.Price, 0) AS PriceDiff,
                    CASE WHEN base.Price IS NULL OR base.Price = 0 THEN NULL
                         ELSE (curr.Price - base.Price) / base.Price END AS PctChange
             FROM PL_RandomLengthsPricingType rlt
             LEFT JOIN PL_RandomLengthsPricing base 
                 ON rlt.RLPricingTypeID = base.RLPricingTypeID 
                 AND base.RandomLengthsImportID = @BaselineRLImportID
             LEFT JOIN PL_RandomLengthsPricing curr 
                 ON rlt.RLPricingTypeID = curr.RLPricingTypeID 
                 AND curr.RandomLengthsImportID = @CurrentRLImportID
             WHERE rlt.IsActive = 1
               AND (base.Price IS NOT NULL OR curr.Price IS NOT NULL)
             ORDER BY rlt.DisplayOrder, rlt.CategoryGroup, rlt.TypeName"

#End Region

    End Class

End Namespace