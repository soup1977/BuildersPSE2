USE [BuildersPSE]
GO
/****** Object:  Table [dbo].[ActualToLevelMapping]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActualToLevelMapping](
	[MappingID] [int] IDENTITY(1,1) NOT NULL,
	[ActualUnitID] [int] NOT NULL,
	[LevelID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[VersionID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MappingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ActualUnits]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ActualUnits](
	[ActualUnitID] [int] IDENTITY(1,1) NOT NULL,
	[RawUnitID] [int] NOT NULL,
	[ProductTypeID] [int] NOT NULL,
	[UnitName] [varchar](100) NOT NULL,
	[PlanSQFT] [decimal](12, 2) NOT NULL,
	[UnitType] [varchar](50) NOT NULL,
	[OptionalAdder] [decimal](12, 2) NULL,
	[MarginPercent] [decimal](5, 2) NULL,
	[VersionID] [int] NOT NULL,
	[ColorCode] [varchar](6) NULL,
	[SortOrder] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ActualUnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[AuditLog]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AuditLog](
	[AuditID] [bigint] IDENTITY(1,1) NOT NULL,
	[AuditDate] [datetime2](3) NOT NULL,
	[WindowsUser] [varchar](100) NOT NULL,
	[ApplicationName] [varchar](100) NOT NULL,
	[TableName] [sysname] NOT NULL,
	[Operation] [char](6) NOT NULL,
	[PrimaryKeyValue] [nvarchar](500) NOT NULL,
	[OldValues] [xml] NULL,
	[NewValues] [xml] NULL,
	[ChangedColumns] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[AuditID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Buildings]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Buildings](
	[BuildingID] [int] IDENTITY(1,1) NOT NULL,
	[BuildingName] [varchar](100) NOT NULL,
	[BuildingType] [int] NULL,
	[BldgQty] [int] NOT NULL,
	[ResUnits] [int] NULL,
	[FloorCostPerBldg] [decimal](12, 2) NULL,
	[RoofCostPerBldg] [decimal](12, 2) NULL,
	[WallCostPerBldg] [decimal](12, 2) NULL,
	[ExtendedFloorCost] [decimal](12, 2) NULL,
	[ExtendedRoofCost] [decimal](12, 2) NULL,
	[ExtendedWallCost] [decimal](12, 2) NULL,
	[OverallPrice] [decimal](12, 2) NULL,
	[OverallCost] [decimal](12, 2) NULL,
	[LastModifiedDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[VersionID] [int] NOT NULL,
 CONSTRAINT [PK__Building__5463CDE4B8D001FA] PRIMARY KEY CLUSTERED 
(
	[BuildingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CalculatedComponents]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CalculatedComponents](
	[ComponentID] [int] IDENTITY(1,1) NOT NULL,
	[ActualUnitID] [int] NOT NULL,
	[ComponentType] [varchar](50) NOT NULL,
	[Value] [decimal](12, 6) NOT NULL,
	[VersionID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ComponentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ComboOptions]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ComboOptions](
	[OptionID] [int] IDENTITY(1,1) NOT NULL,
	[Category] [varchar](50) NOT NULL,
	[Value] [varchar](50) NOT NULL,
	[DisplayOrder] [int] NULL,
 CONSTRAINT [PK_ComboOptions] PRIMARY KEY CLUSTERED 
(
	[OptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Configuration]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Configuration](
	[ConfigID] [int] IDENTITY(1,1) NOT NULL,
	[ConfigKey] [varchar](50) NOT NULL,
	[Value] [decimal](12, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ConfigID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customer]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customer](
	[CustomerID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerName] [varchar](50) NULL,
	[CustomerType] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[CustomerType]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CustomerType](
	[CustomerTypeID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerTypeName] [nvarchar](50) NULL,
 CONSTRAINT [PK_CustomerType] PRIMARY KEY CLUSTERED 
(
	[CustomerTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Estimator]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Estimator](
	[EstimatorID] [int] IDENTITY(1,1) NOT NULL,
	[EstimatorName] [varchar](50) NOT NULL,
	[Active] [bit] NULL,
 CONSTRAINT [PK_Estimator] PRIMARY KEY CLUSTERED 
(
	[EstimatorID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ItemOptions]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemOptions](
	[ItemOptionID] [int] IDENTITY(1,1) NOT NULL,
	[Section] [varchar](50) NOT NULL,
	[KN] [int] NOT NULL,
	[Description] [varchar](100) NOT NULL,
	[DisplayOrder] [int] NULL,
 CONSTRAINT [PK_ItemOptions] PRIMARY KEY CLUSTERED 
(
	[ItemOptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LevelActuals]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LevelActuals](
	[ActualID] [int] IDENTITY(1,1) NOT NULL,
	[BuildingID] [int] NOT NULL,
	[LevelID] [int] NOT NULL,
	[VersionID] [int] NOT NULL,
	[StageType] [tinyint] NOT NULL,
	[ActualBDFT] [decimal](12, 2) NULL,
	[ActualLumberCost] [decimal](12, 2) NULL,
	[ActualPlateCost] [decimal](12, 2) NULL,
	[ActualManufLaborCost] [decimal](12, 2) NULL,
	[ActualManufMH] [decimal](12, 2) NULL,
	[ActualItemCost] [decimal](12, 2) NULL,
	[ActualDeliveryCost] [decimal](12, 2) NULL,
	[ActualMiscLaborCost] [decimal](12, 2) NULL,
	[ActualTotalCost] [decimal](12, 2) NULL,
	[ActualSoldAmount] [decimal](12, 2) NULL,
	[ActualMarginPercent] [decimal](5, 2) NULL,
	[AvgSPFNo2Actual] [decimal](12, 2) NULL,
	[MiTekJobNumber] [varchar](50) NULL,
	[BisTrackSalesOrder] [varchar](50) NULL,
	[BisTrackWorksOrder] [varchar](50) NULL,
	[ImportDate] [datetime] NULL,
	[ImportedBy] [varchar](50) NULL,
	[Notes] [varchar](max) NULL,
 CONSTRAINT [PK__LevelAct__0585DAC997C05C33] PRIMARY KEY CLUSTERED 
(
	[ActualID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LevelActualsBistrack]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LevelActualsBistrack](
	[LevelsActualBistrackID] [int] NOT NULL,
	[SalesOrderNumber] [int] NOT NULL,
	[BranchID] [tinyint] NOT NULL,
	[TotalCostPrice] [float] NULL,
	[TotalSellPrice] [float] NULL,
	[TotalMargin] [float] NULL,
	[TotalWeight] [float] NULL,
	[InvoiceID] [int] NULL,
	[DeliveryDate] [datetime2](7) NULL,
	[InvoiceDate] [datetime2](7) NULL,
	[TaskActHubCost] [float] NULL,
	[TaskActJobSupplies] [float] NULL,
	[TaskActManageCost] [float] NULL,
	[TaskActLabCost] [float] NULL,
	[TaskActDelCost] [float] NULL,
	[WOTaskCostAmt] [float] NULL,
	[TaskActItemCost] [float] NULL,
	[TaskActPltCost] [float] NULL,
	[TaskActLumCost] [float] NULL,
	[SalesOrderMiscCost] [float] NULL,
	[TaskActInvBDFT] [float] NULL,
 CONSTRAINT [PK_LevelsActualBistrack] PRIMARY KEY CLUSTERED 
(
	[LevelsActualBistrackID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Levels]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Levels](
	[LevelID] [int] IDENTITY(1,1) NOT NULL,
	[BuildingID] [int] NOT NULL,
	[ProductTypeID] [int] NOT NULL,
	[LevelNumber] [int] NULL,
	[LevelName] [varchar](100) NOT NULL,
	[OverallPrice] [decimal](12, 2) NULL,
	[OverallCost] [decimal](12, 2) NULL,
	[OverallSQFT] [decimal](12, 2) NULL,
	[OverallLF] [decimal](12, 2) NULL,
	[OverallBDFT] [decimal](12, 2) NULL,
	[LumberCost] [decimal](12, 2) NULL,
	[PlateCost] [decimal](12, 2) NULL,
	[LaborCost] [decimal](12, 2) NULL,
	[LaborMH] [decimal](12, 2) NULL,
	[DesignCost] [decimal](12, 2) NULL,
	[MGMTCost] [decimal](12, 2) NULL,
	[JobSuppliesCost] [decimal](12, 2) NULL,
	[ItemsCost] [decimal](12, 2) NULL,
	[DeliveryCost] [decimal](12, 2) NULL,
	[UnitOverallCost] [decimal](12, 2) NULL,
	[CommonSQFT] [decimal](12, 2) NULL,
	[TotalSQFT] [decimal](12, 2) NULL,
	[AvgPricePerSQFT] [decimal](12, 2) NULL,
	[LastModifiedDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[VersionID] [int] NOT NULL,
 CONSTRAINT [PK__Levels__09F03C06AED8BB38] PRIMARY KEY CLUSTERED 
(
	[LevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LumberCost]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LumberCost](
	[LumberCostID] [int] IDENTITY(1,1) NOT NULL,
	[LumberTypeID] [int] NOT NULL,
	[LumberCost] [decimal](12, 2) NOT NULL,
	[CostEffectiveDateid] [int] NOT NULL,
 CONSTRAINT [PK_LumberCost] PRIMARY KEY CLUSTERED 
(
	[LumberCostID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LumberCostEffective]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LumberCostEffective](
	[CostEffectiveID] [int] IDENTITY(1,1) NOT NULL,
	[CosteffectiveDate] [date] NOT NULL,
 CONSTRAINT [PK_LumberCostEffective] PRIMARY KEY CLUSTERED 
(
	[CostEffectiveID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LumberFutures]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LumberFutures](
	[LumberFutureID] [int] IDENTITY(1,1) NOT NULL,
	[VersionID] [int] NOT NULL,
	[ContractMonth] [varchar](20) NOT NULL,
	[PriorSettle] [decimal](12, 2) NOT NULL,
	[Active] [bit] NULL,
	[PullDate] [datetime] NOT NULL,
 CONSTRAINT [PK__LumberFu__EE3001DFC9C33F61] PRIMARY KEY CLUSTERED 
(
	[LumberFutureID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[LumberType]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[LumberType](
	[LumberTypeID] [int] IDENTITY(1,1) NOT NULL,
	[LumberTypeDesc] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_LumberType] PRIMARY KEY CLUSTERED 
(
	[LumberTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_Builders]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_Builders](
	[BuilderID] [int] IDENTITY(1,1) NOT NULL,
	[BuilderName] [nvarchar](100) NOT NULL,
	[BuilderCode] [nvarchar](20) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BuilderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_Builders_Name] UNIQUE NONCLUSTERED 
(
	[BuilderName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_ComponentPricing]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_ComponentPricing](
	[ComponentPricingID] [int] IDENTITY(1,1) NOT NULL,
	[PriceLockID] [int] NOT NULL,
	[PlanID] [int] NOT NULL,
	[ElevationID] [int] NOT NULL,
	[OptionID] [int] NULL,
	[ProductTypeID] [int] NOT NULL,
	[MgmtSellPrice] [decimal](12, 2) NULL,
	[Cost] [decimal](12, 2) NULL,
	[CalculatedPrice] [decimal](12, 2) NULL,
	[AppliedMargin] [decimal](5, 4) NULL,
	[FinalPrice] [decimal](12, 2) NULL,
	[PriceSentToSales] [decimal](12, 2) NULL,
	[PriceSentToBuilder] [decimal](12, 2) NULL,
	[IsAdder] [bit] NOT NULL,
	[BaseElevationID] [int] NULL,
	[PriceNote] [nvarchar](500) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[MarginSource] [varchar](20) NULL,
	[BaseComponentPricingID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ComponentPricingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_Elevations]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_Elevations](
	[ElevationID] [int] IDENTITY(1,1) NOT NULL,
	[PlanID] [int] NOT NULL,
	[ElevationName] [nvarchar](100) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ElevationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_Elevations_PlanName] UNIQUE NONCLUSTERED 
(
	[PlanID] ASC,
	[ElevationName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_MaterialCategories]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_MaterialCategories](
	[MaterialCategoryID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](50) NOT NULL,
	[CategoryCode] [nvarchar](20) NULL,
	[DisplayOrder] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[MaterialCategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_MaterialCategories_Name] UNIQUE NONCLUSTERED 
(
	[CategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_MaterialCategoryRLMapping]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_MaterialCategoryRLMapping](
	[MappingID] [int] IDENTITY(1,1) NOT NULL,
	[MaterialCategoryID] [int] NOT NULL,
	[RLPricingTypeID] [int] NOT NULL,
	[WeightFactor] [decimal](5, 4) NOT NULL,
	[IsPrimary] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PL_MaterialCategoryRLMapping] PRIMARY KEY CLUSTERED 
(
	[MappingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_MaterialPricing]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_MaterialPricing](
	[MaterialPricingID] [int] IDENTITY(1,1) NOT NULL,
	[PriceLockID] [int] NOT NULL,
	[MaterialCategoryID] [int] NOT NULL,
	[RandomLengthsDate] [date] NULL,
	[RandomLengthsPrice] [decimal](10, 2) NULL,
	[CalculatedPrice] [decimal](10, 4) NULL,
	[PriceSentToSales] [decimal](10, 4) NULL,
	[PriceSentToBuilder] [decimal](10, 4) NULL,
	[PctChangeFromPrevious] [decimal](8, 6) NULL,
	[PriceNote] [nvarchar](500) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
	[BaselineRLPrice] [decimal](10, 2) NULL,
	[CurrentRLPrice] [decimal](10, 2) NULL,
	[OriginalSellPrice] [decimal](12, 2) NULL,
	[RLPercentChange] [decimal](8, 6) NULL,
PRIMARY KEY CLUSTERED 
(
	[MaterialPricingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_MaterialPricing_LockCategory] UNIQUE NONCLUSTERED 
(
	[PriceLockID] ASC,
	[MaterialCategoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_Options]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_Options](
	[OptionID] [int] IDENTITY(1,1) NOT NULL,
	[OptionName] [nvarchar](150) NOT NULL,
	[OptionDescription] [nvarchar](255) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OptionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_Options_Name] UNIQUE NONCLUSTERED 
(
	[OptionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_Plans]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_Plans](
	[PlanID] [int] IDENTITY(1,1) NOT NULL,
	[PlanName] [nvarchar](100) NOT NULL,
	[PlanDescription] [nvarchar](255) NULL,
	[SquareFootage] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[PlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_Plans_Name] UNIQUE NONCLUSTERED 
(
	[PlanName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_PriceChangeHistory]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_PriceChangeHistory](
	[HistoryID] [int] IDENTITY(1,1) NOT NULL,
	[TableName] [nvarchar](50) NOT NULL,
	[RecordID] [int] NOT NULL,
	[FieldName] [nvarchar](50) NOT NULL,
	[OldValue] [nvarchar](100) NULL,
	[NewValue] [nvarchar](100) NULL,
	[ChangedBy] [nvarchar](100) NULL,
	[ChangedDate] [datetime] NOT NULL,
	[ChangeReason] [nvarchar](255) NULL,
PRIMARY KEY CLUSTERED 
(
	[HistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_PriceLocks]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_PriceLocks](
	[PriceLockID] [int] IDENTITY(1,1) NOT NULL,
	[SubdivisionID] [int] NOT NULL,
	[PriceLockDate] [date] NOT NULL,
	[PriceLockName] [nvarchar](100) NULL,
	[BaseMgmtMargin] [decimal](5, 4) NULL,
	[AdjustedMarginBaseModels] [decimal](5, 4) NULL,
	[OptionMargin] [decimal](5, 4) NULL,
	[Status] [nvarchar](20) NOT NULL,
	[CreatedBy] [nvarchar](100) NULL,
	[ApprovedBy] [nvarchar](100) NULL,
	[ApprovedDate] [datetime] NULL,
	[SentToBuilderDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[BaselineRLImportID] [int] NULL,
	[CurrentRLImportID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PriceLockID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_PriceLocks_SubdivisionDate] UNIQUE NONCLUSTERED 
(
	[SubdivisionID] ASC,
	[PriceLockDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_ProductTypes]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_ProductTypes](
	[ProductTypeID] [int] NOT NULL,
	[ProductTypeName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_ProductTypes_Name] UNIQUE NONCLUSTERED 
(
	[ProductTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_RandomLengthsImport]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_RandomLengthsImport](
	[RandomLengthsImportID] [int] IDENTITY(1,1) NOT NULL,
	[ReportDate] [date] NOT NULL,
	[ReportName] [nvarchar](100) NULL,
	[SourceFileName] [nvarchar](255) NULL,
	[ImportMethod] [nvarchar](20) NOT NULL,
	[ImportedBy] [nvarchar](100) NOT NULL,
	[ImportedDate] [datetime] NOT NULL,
	[Notes] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
	[ModifiedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK_PL_RandomLengthsImport] PRIMARY KEY CLUSTERED 
(
	[RandomLengthsImportID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_RandomLengthsPricing]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_RandomLengthsPricing](
	[RandomLengthsPricingID] [int] IDENTITY(1,1) NOT NULL,
	[RandomLengthsImportID] [int] NOT NULL,
	[RLPricingTypeID] [int] NOT NULL,
	[Price] [decimal](10, 2) NOT NULL,
	[PriceSource] [nvarchar](100) NULL,
	[Notes] [nvarchar](255) NULL,
 CONSTRAINT [PK_PL_RandomLengthsPricing] PRIMARY KEY CLUSTERED 
(
	[RandomLengthsPricingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_RandomLengthsPricing_ImportType] UNIQUE NONCLUSTERED 
(
	[RandomLengthsImportID] ASC,
	[RLPricingTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_RandomLengthsPricingType]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_RandomLengthsPricingType](
	[RLPricingTypeID] [int] IDENTITY(1,1) NOT NULL,
	[TypeCode] [nvarchar](50) NOT NULL,
	[TypeName] [nvarchar](100) NOT NULL,
	[TypeDescription] [nvarchar](255) NULL,
	[CategoryGroup] [nvarchar](50) NOT NULL,
	[DisplayOrder] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_PL_RandomLengthsPricingType] PRIMARY KEY CLUSTERED 
(
	[RLPricingTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_RandomLengthsPricingType_TypeCode] UNIQUE NONCLUSTERED 
(
	[TypeCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_SubdivisionPlans]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_SubdivisionPlans](
	[SubdivisionPlanID] [int] IDENTITY(1,1) NOT NULL,
	[SubdivisionID] [int] NOT NULL,
	[PlanID] [int] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SubdivisionPlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_SubdivisionPlans] UNIQUE NONCLUSTERED 
(
	[SubdivisionID] ASC,
	[PlanID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PL_Subdivisions]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PL_Subdivisions](
	[SubdivisionID] [int] IDENTITY(1,1) NOT NULL,
	[BuilderID] [int] NOT NULL,
	[SubdivisionName] [nvarchar](150) NOT NULL,
	[SubdivisionCode] [nvarchar](20) NULL,
	[IsActive] [bit] NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[ModifiedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SubdivisionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [UQ_PL_Subdivisions_BuilderName] UNIQUE NONCLUSTERED 
(
	[BuilderID] ASC,
	[SubdivisionName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductType]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductType](
	[ProductTypeID] [int] IDENTITY(1,1) NOT NULL,
	[ProductTypeName] [varchar](50) NOT NULL,
	[Description] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectBearingStyles]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectBearingStyles](
	[BearingID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NULL,
	[ExtWallStyle] [varchar](50) NULL,
	[ExtRimRibbon] [varchar](50) NULL,
	[IntWallStyle] [varchar](50) NULL,
	[IntRimRibbon] [varchar](50) NULL,
	[CorridorWallStyle] [varchar](50) NULL,
	[CorridorRimRibbon] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[BearingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectDesignInfo]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectDesignInfo](
	[InfoID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NULL,
	[BuildingCode] [varchar](50) NULL,
	[Importance] [varchar](50) NULL,
	[ExposureCategory] [varchar](50) NULL,
	[WindSpeed] [varchar](50) NULL,
	[SnowLoadType] [varchar](50) NULL,
	[OccupancyCategory] [varchar](50) NULL,
	[RoofPitches] [varchar](200) NULL,
	[FloorDepths] [varchar](200) NULL,
	[WallHeights] [varchar](200) NULL,
	[HeelHeights] [varchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[InfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectGeneralNotes]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectGeneralNotes](
	[NoteID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NULL,
	[Notes] [text] NULL,
PRIMARY KEY CLUSTERED 
(
	[NoteID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectItems]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectItems](
	[ItemID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NULL,
	[Section] [varchar](50) NULL,
	[KN] [int] NULL,
	[Description] [varchar](100) NULL,
	[Status] [varchar](1) NULL,
	[Note] [varchar](max) NULL,
 CONSTRAINT [PK__ProjectI__727E83EB07316C52] PRIMARY KEY CLUSTERED 
(
	[ItemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectLoads]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectLoads](
	[LoadID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NULL,
	[Category] [varchar](50) NULL,
	[TCLL] [varchar](50) NULL,
	[TCDL] [varchar](50) NULL,
	[BCLL] [varchar](50) NULL,
	[BCDL] [varchar](50) NULL,
	[OCSpacing] [varchar](50) NULL,
	[LiveLoadDeflection] [varchar](50) NULL,
	[TotalLoadDeflection] [varchar](50) NULL,
	[AbsoluteLL] [varchar](50) NULL,
	[AbsoluteTL] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[LoadID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectProductSettings]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectProductSettings](
	[SettingID] [int] IDENTITY(1,1) NOT NULL,
	[ProductTypeID] [int] NOT NULL,
	[MarginPercent] [decimal](5, 2) NULL,
	[LumberAdder] [decimal](12, 2) NULL,
	[VersionID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SettingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Projects]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Projects](
	[ProjectID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectName] [nvarchar](255) NOT NULL,
	[JBID] [varchar](50) NOT NULL,
	[ProjectTypeID] [int] NULL,
	[EstimatorID] [int] NULL,
	[Address] [varchar](200) NULL,
	[City] [varchar](100) NULL,
	[State] [varchar](50) NULL,
	[Zip] [varchar](20) NULL,
	[BidDate] [date] NULL,
	[ArchPlansDated] [date] NULL,
	[EngPlansDated] [date] NULL,
	[MilesToJobSite] [int] NULL,
	[TotalNetSqft] [int] NULL,
	[TotalGrossSqft] [int] NULL,
	[ArchitectID] [int] NULL,
	[EngineerID] [int] NULL,
	[ProjectNotes] [text] NULL,
	[LastModifiedDate] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
 CONSTRAINT [PK__Projects__761ABED0B8F77E09] PRIMARY KEY CLUSTERED 
(
	[ProjectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectType]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectType](
	[ProjectTypeID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectTypeName] [varchar](50) NOT NULL,
	[Description] [nvarchar](50) NULL,
 CONSTRAINT [PK_ProjectType] PRIMARY KEY CLUSTERED 
(
	[ProjectTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjectVersions]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjectVersions](
	[VersionID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[VersionName] [varchar](50) NOT NULL,
	[VersionDate] [datetime] NOT NULL,
	[Description] [varchar](200) NULL,
	[LastModifiedDate] [datetime] NOT NULL,
	[CustomerID] [int] NULL,
	[SalesID] [int] NULL,
	[MondayID] [nvarchar](50) NULL,
	[ProjVersionStatusID] [int] NULL,
	[FuturesAdderAmt] [decimal](12, 2) NULL,
	[FuturesAdderProjTotal] [decimal](12, 2) NULL,
	[IsLocked] [bit] NOT NULL,
	[LockedDate] [datetime] NULL,
	[LockedBy] [nvarchar](100) NULL,
 CONSTRAINT [PK__ProjectV__16C6402FDD4575C5] PRIMARY KEY CLUSTERED 
(
	[VersionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjVersionPriceHistory]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjVersionPriceHistory](
	[PriceHistoryID] [int] IDENTITY(1,1) NOT NULL,
	[ProjectID] [int] NOT NULL,
	[VersionID] [int] NOT NULL,
	[VersionName] [nvarchar](100) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [nvarchar](100) NOT NULL,
	[ReportType] [nvarchar](50) NOT NULL,
	[JBID] [nvarchar](50) NULL,
	[ProjectName] [nvarchar](200) NULL,
	[Address] [nvarchar](200) NULL,
	[City] [nvarchar](100) NULL,
	[State] [nvarchar](50) NULL,
	[Zip] [nvarchar](20) NULL,
	[BidDate] [date] NULL,
	[MilesToJobSite] [int] NULL,
	[TotalGrossSqftEntered] [int] NULL,
	[TotalNetSqftEntered] [int] NULL,
	[ExtendedSell] [decimal](18, 2) NULL,
	[ExtendedCost] [decimal](18, 2) NULL,
	[ExtendedDelivery] [decimal](18, 2) NULL,
	[ExtendedSqft] [decimal](18, 2) NULL,
	[ExtendedBdft] [decimal](18, 2) NULL,
	[CalculatedGrossSqft] [decimal](18, 2) NULL,
	[Margin] [decimal](8, 4) NULL,
	[MarginWithDelivery] [decimal](8, 4) NULL,
	[PricePerBdft] [decimal](18, 4) NULL,
	[PricePerSqft] [decimal](18, 4) NULL,
	[FuturesAdderAmt] [decimal](18, 4) NULL,
	[FuturesAdderProjTotal] [decimal](18, 2) NULL,
	[FuturesContractMonth] [nvarchar](20) NULL,
	[FuturesPriorSettle] [decimal](18, 4) NULL,
	[FuturesPullDate] [datetime] NULL,
	[LumberFutureID] [int] NULL,
	[ActiveFloorSPFNo2] [decimal](18, 4) NULL,
	[ActiveRoofSPFNo2] [decimal](18, 4) NULL,
	[ActiveCostEffectiveID] [int] NULL,
	[ActiveCostEffectiveDate] [date] NULL,
	[FloorMarginPercent] [decimal](8, 4) NULL,
	[FloorLumberAdder] [decimal](18, 4) NULL,
	[RoofMarginPercent] [decimal](8, 4) NULL,
	[RoofLumberAdder] [decimal](18, 4) NULL,
	[WallMarginPercent] [decimal](8, 4) NULL,
	[WallLumberAdder] [decimal](18, 4) NULL,
	[CustomerID] [int] NULL,
	[CustomerName] [nvarchar](200) NULL,
	[SalesID] [int] NULL,
	[SalesName] [nvarchar](200) NULL,
	[EstimatorID] [int] NULL,
	[EstimatorName] [nvarchar](200) NULL,
	[TotalBuildingCount] [int] NULL,
	[TotalBldgQty] [int] NULL,
	[TotalLevelCount] [int] NULL,
	[TotalActualUnitCount] [int] NULL,
	[Notes] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[PriceHistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjVersionPriceHistoryBuildings]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjVersionPriceHistoryBuildings](
	[PriceHistoryBuildingID] [int] IDENTITY(1,1) NOT NULL,
	[PriceHistoryID] [int] NOT NULL,
	[BuildingID] [int] NOT NULL,
	[BuildingName] [nvarchar](200) NOT NULL,
	[BldgQty] [int] NOT NULL,
	[BaseSell] [decimal](18, 2) NULL,
	[BaseCost] [decimal](18, 2) NULL,
	[BaseDelivery] [decimal](18, 2) NULL,
	[BaseSqft] [decimal](18, 2) NULL,
	[BaseBdft] [decimal](18, 2) NULL,
	[ExtendedSell] [decimal](18, 2) NULL,
	[ExtendedCost] [decimal](18, 2) NULL,
	[ExtendedDelivery] [decimal](18, 2) NULL,
	[ExtendedSqft] [decimal](18, 2) NULL,
	[ExtendedBdft] [decimal](18, 2) NULL,
	[Margin] [decimal](8, 4) NULL,
PRIMARY KEY CLUSTERED 
(
	[PriceHistoryBuildingID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjVersionPriceHistoryLevels]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjVersionPriceHistoryLevels](
	[PriceHistoryLevelID] [int] IDENTITY(1,1) NOT NULL,
	[PriceHistoryBuildingID] [int] NOT NULL,
	[LevelID] [int] NOT NULL,
	[LevelName] [nvarchar](200) NOT NULL,
	[LevelNumber] [int] NOT NULL,
	[ProductTypeID] [int] NOT NULL,
	[ProductTypeName] [nvarchar](100) NOT NULL,
	[OverallSqft] [decimal](18, 2) NULL,
	[OverallLf] [decimal](18, 2) NULL,
	[OverallBdft] [decimal](18, 2) NULL,
	[LumberCost] [decimal](18, 2) NULL,
	[PlateCost] [decimal](18, 2) NULL,
	[LaborCost] [decimal](18, 2) NULL,
	[LaborMH] [decimal](18, 2) NULL,
	[DesignCost] [decimal](18, 2) NULL,
	[MgmtCost] [decimal](18, 2) NULL,
	[JobSuppliesCost] [decimal](18, 2) NULL,
	[ItemsCost] [decimal](18, 2) NULL,
	[DeliveryCost] [decimal](18, 2) NULL,
	[OverallCost] [decimal](18, 2) NULL,
	[OverallPrice] [decimal](18, 2) NULL,
	[Margin] [decimal](8, 4) NULL,
	[ActualUnitCount] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PriceHistoryLevelID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProjVersionStatus]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProjVersionStatus](
	[ProjVersionStatusID] [int] IDENTITY(1,1) NOT NULL,
	[ProjVersionStatus] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ProjVersionStatus] PRIMARY KEY CLUSTERED 
(
	[ProjVersionStatusID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RawUnitLumberHistory]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RawUnitLumberHistory](
	[HistoryID] [int] IDENTITY(1,1) NOT NULL,
	[RawUnitID] [int] NOT NULL,
	[VersionID] [int] NOT NULL,
	[CostEffectiveDateID] [int] NULL,
	[LumberCost] [decimal](12, 2) NOT NULL,
	[AvgSPFNo2] [decimal](12, 2) NULL,
	[Avg241800] [decimal](12, 2) NULL,
	[Avg242400] [decimal](12, 2) NULL,
	[Avg261800] [decimal](12, 2) NULL,
	[Avg262400] [decimal](12, 2) NULL,
	[UpdateDate] [datetime] NOT NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[HistoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RawUnits]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RawUnits](
	[RawUnitID] [int] IDENTITY(1,1) NOT NULL,
	[VersionID] [int] NOT NULL,
	[ProductTypeID] [int] NOT NULL,
	[RawUnitName] [varchar](100) NOT NULL,
	[BF] [decimal](12, 2) NULL,
	[LF] [decimal](12, 2) NULL,
	[EWPLF] [decimal](12, 2) NULL,
	[SqFt] [decimal](12, 2) NULL,
	[FCArea] [decimal](12, 2) NULL,
	[LumberCost] [decimal](12, 2) NULL,
	[PlateCost] [decimal](12, 2) NULL,
	[ManufLaborCost] [decimal](12, 2) NULL,
	[DesignLabor] [decimal](12, 2) NULL,
	[MGMTLabor] [decimal](12, 2) NULL,
	[JobSuppliesCost] [decimal](12, 2) NULL,
	[ManHours] [decimal](12, 2) NULL,
	[ItemCost] [decimal](12, 2) NULL,
	[OverallCost] [decimal](12, 2) NULL,
	[DeliveryCost] [decimal](12, 2) NULL,
	[TotalSellPrice] [decimal](12, 2) NULL,
	[AvgSPFNo2] [decimal](12, 2) NULL,
	[SPFNo2BDFT] [decimal](12, 2) NULL,
	[Avg241800] [decimal](12, 2) NULL,
	[MSR241800BDFT] [decimal](12, 2) NULL,
	[Avg242400] [decimal](12, 2) NULL,
	[MSR242400BDFT] [decimal](12, 2) NULL,
	[Avg261800] [decimal](12, 2) NULL,
	[MSR261800BDFT] [decimal](12, 2) NULL,
	[Avg262400] [decimal](12, 2) NULL,
	[MSR262400BDFT] [decimal](12, 2) NULL,
 CONSTRAINT [PK__RawUnits__7908BCA58256945E] PRIMARY KEY CLUSTERED 
(
	[RawUnitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sales]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sales](
	[SalesID] [int] IDENTITY(1,1) NOT NULL,
	[SalesName] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[SalesID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[WindowsLogon] [varchar](100) NOT NULL,
	[DisplayName] [nvarchar](100) NOT NULL,
	[EstimatorID] [int] NULL,
	[IsActive] [bit] NOT NULL,
	[IsAdmin] [bit] NOT NULL,
	[IsLoggedIn] [bit] NOT NULL,
	[LastLogin] [datetime] NULL,
	[LastLogout] [datetime] NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[WindowsLogon] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[VersionUnlockLog]    Script Date: 2/18/2026 2:05:17 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[VersionUnlockLog](
	[UnlockLogID] [int] IDENTITY(1,1) NOT NULL,
	[VersionID] [int] NOT NULL,
	[UnlockedBy] [nvarchar](100) NOT NULL,
	[UnlockedDate] [datetime] NOT NULL,
	[Reason] [nvarchar](500) NULL,
	[OriginalLockedDate] [datetime] NULL,
	[OriginalLockedBy] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[UnlockLogID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[AuditLog] ADD  DEFAULT (sysdatetime()) FOR [AuditDate]
GO
ALTER TABLE [dbo].[AuditLog] ADD  DEFAULT (suser_sname()) FOR [WindowsUser]
GO
ALTER TABLE [dbo].[AuditLog] ADD  DEFAULT (app_name()) FOR [ApplicationName]
GO
ALTER TABLE [dbo].[Buildings] ADD  CONSTRAINT [DF_Buildings_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[Estimator] ADD  CONSTRAINT [DF_Estimator_Active]  DEFAULT ((1)) FOR [Active]
GO
ALTER TABLE [dbo].[LevelActuals] ADD  CONSTRAINT [DF__LevelActu__Stage__45544755]  DEFAULT ((1)) FOR [StageType]
GO
ALTER TABLE [dbo].[LevelActuals] ADD  CONSTRAINT [DF__LevelActu__Impor__46486B8E]  DEFAULT (getdate()) FOR [ImportDate]
GO
ALTER TABLE [dbo].[Levels] ADD  CONSTRAINT [DF_Levels_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_Builders] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PL_Builders] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_Builders] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[PL_ComponentPricing] ADD  DEFAULT ((0)) FOR [IsAdder]
GO
ALTER TABLE [dbo].[PL_ComponentPricing] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_ComponentPricing] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[PL_ComponentPricing] ADD  CONSTRAINT [DF_PL_ComponentPricing_MarginSource]  DEFAULT ('Adjusted') FOR [MarginSource]
GO
ALTER TABLE [dbo].[PL_Elevations] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PL_Elevations] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_Elevations] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[PL_MaterialCategories] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[PL_MaterialCategories] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PL_MaterialCategories] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_MaterialCategoryRLMapping] ADD  DEFAULT ((1.0)) FOR [WeightFactor]
GO
ALTER TABLE [dbo].[PL_MaterialCategoryRLMapping] ADD  DEFAULT ((1)) FOR [IsPrimary]
GO
ALTER TABLE [dbo].[PL_MaterialCategoryRLMapping] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PL_MaterialPricing] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_MaterialPricing] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[PL_Options] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PL_Options] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_Options] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[PL_Plans] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PL_Plans] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_Plans] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[PL_PriceChangeHistory] ADD  DEFAULT (getdate()) FOR [ChangedDate]
GO
ALTER TABLE [dbo].[PL_PriceLocks] ADD  DEFAULT ('Draft') FOR [Status]
GO
ALTER TABLE [dbo].[PL_PriceLocks] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_PriceLocks] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[PL_RandomLengthsImport] ADD  DEFAULT ('Manual') FOR [ImportMethod]
GO
ALTER TABLE [dbo].[PL_RandomLengthsImport] ADD  DEFAULT (getdate()) FOR [ImportedDate]
GO
ALTER TABLE [dbo].[PL_RandomLengthsImport] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PL_RandomLengthsImport] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_RandomLengthsImport] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[PL_RandomLengthsPricingType] ADD  DEFAULT ((0)) FOR [DisplayOrder]
GO
ALTER TABLE [dbo].[PL_RandomLengthsPricingType] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PL_SubdivisionPlans] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PL_SubdivisionPlans] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_Subdivisions] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[PL_Subdivisions] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[PL_Subdivisions] ADD  DEFAULT (getdate()) FOR [ModifiedDate]
GO
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF__Projects__Estima__46E78A0C]  DEFAULT ('') FOR [EstimatorID]
GO
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF__Projects__BidDat__47DBAE45]  DEFAULT ('1900-01-01') FOR [BidDate]
GO
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF__Projects__ArchPl__48CFD27E]  DEFAULT ('1900-01-01') FOR [ArchPlansDated]
GO
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF__Projects__EngPla__49C3F6B7]  DEFAULT ('1900-01-01') FOR [EngPlansDated]
GO
ALTER TABLE [dbo].[Projects] ADD  CONSTRAINT [DF_Projects_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ProjectVersions] ADD  DEFAULT ((0)) FOR [IsLocked]
GO
ALTER TABLE [dbo].[ProjVersionPriceHistory] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[ProjVersionPriceHistoryBuildings] ADD  DEFAULT ((1)) FOR [BldgQty]
GO
ALTER TABLE [dbo].[RawUnitLumberHistory] ADD  DEFAULT (getdate()) FOR [UpdateDate]
GO
ALTER TABLE [dbo].[RawUnitLumberHistory] ADD  CONSTRAINT [DF_RawUnitLumberHistory_IsActive]  DEFAULT ((0)) FOR [IsActive]
GO
ALTER TABLE [dbo].[RawUnits] ADD  CONSTRAINT [DF_RawUnits_SPFNo2BDFT]  DEFAULT ((0)) FOR [SPFNo2BDFT]
GO
ALTER TABLE [dbo].[RawUnits] ADD  CONSTRAINT [DF_RawUnits_Avg241800]  DEFAULT ((0)) FOR [Avg241800]
GO
ALTER TABLE [dbo].[RawUnits] ADD  CONSTRAINT [DF_RawUnits_MSR241800BDFT]  DEFAULT ((0)) FOR [MSR241800BDFT]
GO
ALTER TABLE [dbo].[RawUnits] ADD  CONSTRAINT [DF_RawUnits_Avg242400]  DEFAULT ((0)) FOR [Avg242400]
GO
ALTER TABLE [dbo].[RawUnits] ADD  CONSTRAINT [DF_RawUnits_MSR242400BDFT]  DEFAULT ((0)) FOR [MSR242400BDFT]
GO
ALTER TABLE [dbo].[RawUnits] ADD  CONSTRAINT [DF_RawUnits_Avg261800]  DEFAULT ((0)) FOR [Avg261800]
GO
ALTER TABLE [dbo].[RawUnits] ADD  CONSTRAINT [DF_RawUnits_MSR261800BDFT]  DEFAULT ((0)) FOR [MSR261800BDFT]
GO
ALTER TABLE [dbo].[RawUnits] ADD  CONSTRAINT [DF_RawUnits_Avg262400]  DEFAULT ((0)) FOR [Avg262400]
GO
ALTER TABLE [dbo].[RawUnits] ADD  CONSTRAINT [DF_RawUnits_MSR262400BDFT]  DEFAULT ((0)) FOR [MSR262400BDFT]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [IsAdmin]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ((0)) FOR [IsLoggedIn]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedDate]
GO
ALTER TABLE [dbo].[VersionUnlockLog] ADD  DEFAULT (getdate()) FOR [UnlockedDate]
GO
ALTER TABLE [dbo].[ActualToLevelMapping]  WITH CHECK ADD FOREIGN KEY([ActualUnitID])
REFERENCES [dbo].[ActualUnits] ([ActualUnitID])
GO
ALTER TABLE [dbo].[ActualToLevelMapping]  WITH CHECK ADD  CONSTRAINT [FK__ActualToL__Level__619B8048] FOREIGN KEY([LevelID])
REFERENCES [dbo].[Levels] ([LevelID])
GO
ALTER TABLE [dbo].[ActualToLevelMapping] CHECK CONSTRAINT [FK__ActualToL__Level__619B8048]
GO
ALTER TABLE [dbo].[ActualToLevelMapping]  WITH CHECK ADD  CONSTRAINT [FK_ActualToLevelMapping_ProjectVersions] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
GO
ALTER TABLE [dbo].[ActualToLevelMapping] CHECK CONSTRAINT [FK_ActualToLevelMapping_ProjectVersions]
GO
ALTER TABLE [dbo].[ActualUnits]  WITH CHECK ADD FOREIGN KEY([ProductTypeID])
REFERENCES [dbo].[ProductType] ([ProductTypeID])
GO
ALTER TABLE [dbo].[ActualUnits]  WITH CHECK ADD  CONSTRAINT [FK__ActualUni__RawUn__5812160E] FOREIGN KEY([RawUnitID])
REFERENCES [dbo].[RawUnits] ([RawUnitID])
GO
ALTER TABLE [dbo].[ActualUnits] CHECK CONSTRAINT [FK__ActualUni__RawUn__5812160E]
GO
ALTER TABLE [dbo].[ActualUnits]  WITH CHECK ADD  CONSTRAINT [FK_ActualUnits_ProjectVersions] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
GO
ALTER TABLE [dbo].[ActualUnits] CHECK CONSTRAINT [FK_ActualUnits_ProjectVersions]
GO
ALTER TABLE [dbo].[Buildings]  WITH CHECK ADD  CONSTRAINT [FK_Buildings_Buildings] FOREIGN KEY([BuildingID])
REFERENCES [dbo].[Buildings] ([BuildingID])
GO
ALTER TABLE [dbo].[Buildings] CHECK CONSTRAINT [FK_Buildings_Buildings]
GO
ALTER TABLE [dbo].[Buildings]  WITH CHECK ADD  CONSTRAINT [FK_Buildings_ProjectVersions] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
GO
ALTER TABLE [dbo].[Buildings] CHECK CONSTRAINT [FK_Buildings_ProjectVersions]
GO
ALTER TABLE [dbo].[CalculatedComponents]  WITH CHECK ADD FOREIGN KEY([ActualUnitID])
REFERENCES [dbo].[ActualUnits] ([ActualUnitID])
GO
ALTER TABLE [dbo].[CalculatedComponents]  WITH CHECK ADD  CONSTRAINT [FK_CalculatedComponents_ProjectVersions] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
GO
ALTER TABLE [dbo].[CalculatedComponents] CHECK CONSTRAINT [FK_CalculatedComponents_ProjectVersions]
GO
ALTER TABLE [dbo].[Customer]  WITH CHECK ADD  CONSTRAINT [FK_Customer_CustomerType] FOREIGN KEY([CustomerType])
REFERENCES [dbo].[CustomerType] ([CustomerTypeID])
GO
ALTER TABLE [dbo].[Customer] CHECK CONSTRAINT [FK_Customer_CustomerType]
GO
ALTER TABLE [dbo].[CustomerType]  WITH CHECK ADD  CONSTRAINT [FK_CustomerType_CustomerType] FOREIGN KEY([CustomerTypeID])
REFERENCES [dbo].[CustomerType] ([CustomerTypeID])
GO
ALTER TABLE [dbo].[CustomerType] CHECK CONSTRAINT [FK_CustomerType_CustomerType]
GO
ALTER TABLE [dbo].[LevelActuals]  WITH CHECK ADD  CONSTRAINT [FK_LevelActuals_Buildings] FOREIGN KEY([BuildingID])
REFERENCES [dbo].[Buildings] ([BuildingID])
GO
ALTER TABLE [dbo].[LevelActuals] CHECK CONSTRAINT [FK_LevelActuals_Buildings]
GO
ALTER TABLE [dbo].[LevelActuals]  WITH CHECK ADD  CONSTRAINT [FK_LevelActuals_Levels] FOREIGN KEY([LevelID])
REFERENCES [dbo].[Levels] ([LevelID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LevelActuals] CHECK CONSTRAINT [FK_LevelActuals_Levels]
GO
ALTER TABLE [dbo].[LevelActuals]  WITH CHECK ADD  CONSTRAINT [FK_LevelActuals_ProjectVersions] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LevelActuals] CHECK CONSTRAINT [FK_LevelActuals_ProjectVersions]
GO
ALTER TABLE [dbo].[Levels]  WITH CHECK ADD  CONSTRAINT [FK__Levels__Building__5070F446] FOREIGN KEY([BuildingID])
REFERENCES [dbo].[Buildings] ([BuildingID])
GO
ALTER TABLE [dbo].[Levels] CHECK CONSTRAINT [FK__Levels__Building__5070F446]
GO
ALTER TABLE [dbo].[Levels]  WITH CHECK ADD  CONSTRAINT [FK_Levels_ProductType] FOREIGN KEY([ProductTypeID])
REFERENCES [dbo].[ProductType] ([ProductTypeID])
GO
ALTER TABLE [dbo].[Levels] CHECK CONSTRAINT [FK_Levels_ProductType]
GO
ALTER TABLE [dbo].[Levels]  WITH CHECK ADD  CONSTRAINT [FK_Levels_ProjectVersions] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
GO
ALTER TABLE [dbo].[Levels] CHECK CONSTRAINT [FK_Levels_ProjectVersions]
GO
ALTER TABLE [dbo].[LumberCost]  WITH CHECK ADD  CONSTRAINT [FK_LumberCost_LumberCostEffective] FOREIGN KEY([CostEffectiveDateid])
REFERENCES [dbo].[LumberCostEffective] ([CostEffectiveID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LumberCost] CHECK CONSTRAINT [FK_LumberCost_LumberCostEffective]
GO
ALTER TABLE [dbo].[LumberCost]  WITH CHECK ADD  CONSTRAINT [FK_LumberCost_LumberType] FOREIGN KEY([LumberTypeID])
REFERENCES [dbo].[LumberType] ([LumberTypeID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LumberCost] CHECK CONSTRAINT [FK_LumberCost_LumberType]
GO
ALTER TABLE [dbo].[LumberFutures]  WITH CHECK ADD  CONSTRAINT [FK_LumberFutures_Version] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[LumberFutures] CHECK CONSTRAINT [FK_LumberFutures_Version]
GO
ALTER TABLE [dbo].[PL_ComponentPricing]  WITH CHECK ADD  CONSTRAINT [FK_ComponentPricing_BaseComponent] FOREIGN KEY([BaseComponentPricingID])
REFERENCES [dbo].[PL_ComponentPricing] ([ComponentPricingID])
GO
ALTER TABLE [dbo].[PL_ComponentPricing] CHECK CONSTRAINT [FK_ComponentPricing_BaseComponent]
GO
ALTER TABLE [dbo].[PL_ComponentPricing]  WITH CHECK ADD  CONSTRAINT [FK_PL_ComponentPricing_BaseElev] FOREIGN KEY([BaseElevationID])
REFERENCES [dbo].[PL_Elevations] ([ElevationID])
GO
ALTER TABLE [dbo].[PL_ComponentPricing] CHECK CONSTRAINT [FK_PL_ComponentPricing_BaseElev]
GO
ALTER TABLE [dbo].[PL_ComponentPricing]  WITH CHECK ADD  CONSTRAINT [FK_PL_ComponentPricing_Elevation] FOREIGN KEY([ElevationID])
REFERENCES [dbo].[PL_Elevations] ([ElevationID])
GO
ALTER TABLE [dbo].[PL_ComponentPricing] CHECK CONSTRAINT [FK_PL_ComponentPricing_Elevation]
GO
ALTER TABLE [dbo].[PL_ComponentPricing]  WITH CHECK ADD  CONSTRAINT [FK_PL_ComponentPricing_Option] FOREIGN KEY([OptionID])
REFERENCES [dbo].[PL_Options] ([OptionID])
GO
ALTER TABLE [dbo].[PL_ComponentPricing] CHECK CONSTRAINT [FK_PL_ComponentPricing_Option]
GO
ALTER TABLE [dbo].[PL_ComponentPricing]  WITH CHECK ADD  CONSTRAINT [FK_PL_ComponentPricing_Plan] FOREIGN KEY([PlanID])
REFERENCES [dbo].[PL_Plans] ([PlanID])
GO
ALTER TABLE [dbo].[PL_ComponentPricing] CHECK CONSTRAINT [FK_PL_ComponentPricing_Plan]
GO
ALTER TABLE [dbo].[PL_ComponentPricing]  WITH CHECK ADD  CONSTRAINT [FK_PL_ComponentPricing_PriceLock] FOREIGN KEY([PriceLockID])
REFERENCES [dbo].[PL_PriceLocks] ([PriceLockID])
GO
ALTER TABLE [dbo].[PL_ComponentPricing] CHECK CONSTRAINT [FK_PL_ComponentPricing_PriceLock]
GO
ALTER TABLE [dbo].[PL_ComponentPricing]  WITH CHECK ADD  CONSTRAINT [FK_PL_ComponentPricing_ProductType] FOREIGN KEY([ProductTypeID])
REFERENCES [dbo].[PL_ProductTypes] ([ProductTypeID])
GO
ALTER TABLE [dbo].[PL_ComponentPricing] CHECK CONSTRAINT [FK_PL_ComponentPricing_ProductType]
GO
ALTER TABLE [dbo].[PL_Elevations]  WITH CHECK ADD  CONSTRAINT [FK_PL_Elevations_Plan] FOREIGN KEY([PlanID])
REFERENCES [dbo].[PL_Plans] ([PlanID])
GO
ALTER TABLE [dbo].[PL_Elevations] CHECK CONSTRAINT [FK_PL_Elevations_Plan]
GO
ALTER TABLE [dbo].[PL_MaterialCategoryRLMapping]  WITH CHECK ADD  CONSTRAINT [FK_PL_MaterialCategoryRLMapping_Category] FOREIGN KEY([MaterialCategoryID])
REFERENCES [dbo].[PL_MaterialCategories] ([MaterialCategoryID])
GO
ALTER TABLE [dbo].[PL_MaterialCategoryRLMapping] CHECK CONSTRAINT [FK_PL_MaterialCategoryRLMapping_Category]
GO
ALTER TABLE [dbo].[PL_MaterialCategoryRLMapping]  WITH CHECK ADD  CONSTRAINT [FK_PL_MaterialCategoryRLMapping_RLType] FOREIGN KEY([RLPricingTypeID])
REFERENCES [dbo].[PL_RandomLengthsPricingType] ([RLPricingTypeID])
GO
ALTER TABLE [dbo].[PL_MaterialCategoryRLMapping] CHECK CONSTRAINT [FK_PL_MaterialCategoryRLMapping_RLType]
GO
ALTER TABLE [dbo].[PL_MaterialPricing]  WITH CHECK ADD  CONSTRAINT [FK_PL_MaterialPricing_Category] FOREIGN KEY([MaterialCategoryID])
REFERENCES [dbo].[PL_MaterialCategories] ([MaterialCategoryID])
GO
ALTER TABLE [dbo].[PL_MaterialPricing] CHECK CONSTRAINT [FK_PL_MaterialPricing_Category]
GO
ALTER TABLE [dbo].[PL_MaterialPricing]  WITH CHECK ADD  CONSTRAINT [FK_PL_MaterialPricing_PriceLock] FOREIGN KEY([PriceLockID])
REFERENCES [dbo].[PL_PriceLocks] ([PriceLockID])
GO
ALTER TABLE [dbo].[PL_MaterialPricing] CHECK CONSTRAINT [FK_PL_MaterialPricing_PriceLock]
GO
ALTER TABLE [dbo].[PL_PriceLocks]  WITH CHECK ADD  CONSTRAINT [FK_PL_PriceLocks_BaselineRL] FOREIGN KEY([BaselineRLImportID])
REFERENCES [dbo].[PL_RandomLengthsImport] ([RandomLengthsImportID])
GO
ALTER TABLE [dbo].[PL_PriceLocks] CHECK CONSTRAINT [FK_PL_PriceLocks_BaselineRL]
GO
ALTER TABLE [dbo].[PL_PriceLocks]  WITH CHECK ADD  CONSTRAINT [FK_PL_PriceLocks_CurrentRL] FOREIGN KEY([CurrentRLImportID])
REFERENCES [dbo].[PL_RandomLengthsImport] ([RandomLengthsImportID])
GO
ALTER TABLE [dbo].[PL_PriceLocks] CHECK CONSTRAINT [FK_PL_PriceLocks_CurrentRL]
GO
ALTER TABLE [dbo].[PL_PriceLocks]  WITH CHECK ADD  CONSTRAINT [FK_PL_PriceLocks_Subdivision] FOREIGN KEY([SubdivisionID])
REFERENCES [dbo].[PL_Subdivisions] ([SubdivisionID])
GO
ALTER TABLE [dbo].[PL_PriceLocks] CHECK CONSTRAINT [FK_PL_PriceLocks_Subdivision]
GO
ALTER TABLE [dbo].[PL_RandomLengthsPricing]  WITH CHECK ADD  CONSTRAINT [FK_PL_RandomLengthsPricing_Import] FOREIGN KEY([RandomLengthsImportID])
REFERENCES [dbo].[PL_RandomLengthsImport] ([RandomLengthsImportID])
GO
ALTER TABLE [dbo].[PL_RandomLengthsPricing] CHECK CONSTRAINT [FK_PL_RandomLengthsPricing_Import]
GO
ALTER TABLE [dbo].[PL_RandomLengthsPricing]  WITH CHECK ADD  CONSTRAINT [FK_PL_RandomLengthsPricing_Type] FOREIGN KEY([RLPricingTypeID])
REFERENCES [dbo].[PL_RandomLengthsPricingType] ([RLPricingTypeID])
GO
ALTER TABLE [dbo].[PL_RandomLengthsPricing] CHECK CONSTRAINT [FK_PL_RandomLengthsPricing_Type]
GO
ALTER TABLE [dbo].[PL_SubdivisionPlans]  WITH CHECK ADD  CONSTRAINT [FK_PL_SubdivisionPlans_Plan] FOREIGN KEY([PlanID])
REFERENCES [dbo].[PL_Plans] ([PlanID])
GO
ALTER TABLE [dbo].[PL_SubdivisionPlans] CHECK CONSTRAINT [FK_PL_SubdivisionPlans_Plan]
GO
ALTER TABLE [dbo].[PL_SubdivisionPlans]  WITH CHECK ADD  CONSTRAINT [FK_PL_SubdivisionPlans_Subdivision] FOREIGN KEY([SubdivisionID])
REFERENCES [dbo].[PL_Subdivisions] ([SubdivisionID])
GO
ALTER TABLE [dbo].[PL_SubdivisionPlans] CHECK CONSTRAINT [FK_PL_SubdivisionPlans_Subdivision]
GO
ALTER TABLE [dbo].[PL_Subdivisions]  WITH CHECK ADD  CONSTRAINT [FK_PL_Subdivisions_Builder] FOREIGN KEY([BuilderID])
REFERENCES [dbo].[PL_Builders] ([BuilderID])
GO
ALTER TABLE [dbo].[PL_Subdivisions] CHECK CONSTRAINT [FK_PL_Subdivisions_Builder]
GO
ALTER TABLE [dbo].[ProjectBearingStyles]  WITH CHECK ADD  CONSTRAINT [FK__ProjectBe__Proje__4E1E9780] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectBearingStyles] CHECK CONSTRAINT [FK__ProjectBe__Proje__4E1E9780]
GO
ALTER TABLE [dbo].[ProjectDesignInfo]  WITH CHECK ADD  CONSTRAINT [FK__ProjectDe__Proje__467D75B8] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectDesignInfo] CHECK CONSTRAINT [FK__ProjectDe__Proje__467D75B8]
GO
ALTER TABLE [dbo].[ProjectGeneralNotes]  WITH CHECK ADD  CONSTRAINT [FK__ProjectGe__Proje__51EF2864] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectGeneralNotes] CHECK CONSTRAINT [FK__ProjectGe__Proje__51EF2864]
GO
ALTER TABLE [dbo].[ProjectItems]  WITH CHECK ADD  CONSTRAINT [FK__ProjectIt__Proje__55BFB948] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectItems] CHECK CONSTRAINT [FK__ProjectIt__Proje__55BFB948]
GO
ALTER TABLE [dbo].[ProjectLoads]  WITH CHECK ADD  CONSTRAINT [FK__ProjectLo__Proje__4A4E069C] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectLoads] CHECK CONSTRAINT [FK__ProjectLo__Proje__4A4E069C]
GO
ALTER TABLE [dbo].[ProjectProductSettings]  WITH CHECK ADD FOREIGN KEY([ProductTypeID])
REFERENCES [dbo].[ProductType] ([ProductTypeID])
GO
ALTER TABLE [dbo].[ProjectProductSettings]  WITH CHECK ADD  CONSTRAINT [FK_ProjectProductSettings_ProjectVersions] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
GO
ALTER TABLE [dbo].[ProjectProductSettings] CHECK CONSTRAINT [FK_ProjectProductSettings_ProjectVersions]
GO
ALTER TABLE [dbo].[Projects]  WITH CHECK ADD  CONSTRAINT [FK_Projects_CustomerArch] FOREIGN KEY([ArchitectID])
REFERENCES [dbo].[Customer] ([CustomerID])
GO
ALTER TABLE [dbo].[Projects] CHECK CONSTRAINT [FK_Projects_CustomerArch]
GO
ALTER TABLE [dbo].[Projects]  WITH CHECK ADD  CONSTRAINT [FK_Projects_CustomerEng] FOREIGN KEY([EngineerID])
REFERENCES [dbo].[Customer] ([CustomerID])
GO
ALTER TABLE [dbo].[Projects] CHECK CONSTRAINT [FK_Projects_CustomerEng]
GO
ALTER TABLE [dbo].[Projects]  WITH CHECK ADD  CONSTRAINT [FK_Projects_Estimator] FOREIGN KEY([EstimatorID])
REFERENCES [dbo].[Estimator] ([EstimatorID])
GO
ALTER TABLE [dbo].[Projects] CHECK CONSTRAINT [FK_Projects_Estimator]
GO
ALTER TABLE [dbo].[Projects]  WITH CHECK ADD  CONSTRAINT [FK_Projects_ProjectType] FOREIGN KEY([ProjectTypeID])
REFERENCES [dbo].[ProjectType] ([ProjectTypeID])
GO
ALTER TABLE [dbo].[Projects] CHECK CONSTRAINT [FK_Projects_ProjectType]
GO
ALTER TABLE [dbo].[ProjectType]  WITH CHECK ADD  CONSTRAINT [FK_ProjectType_ProjectType] FOREIGN KEY([ProjectTypeID])
REFERENCES [dbo].[ProjectType] ([ProjectTypeID])
GO
ALTER TABLE [dbo].[ProjectType] CHECK CONSTRAINT [FK_ProjectType_ProjectType]
GO
ALTER TABLE [dbo].[ProjectVersions]  WITH CHECK ADD  CONSTRAINT [FK_ProjectVersions_Customer] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Customer] ([CustomerID])
GO
ALTER TABLE [dbo].[ProjectVersions] CHECK CONSTRAINT [FK_ProjectVersions_Customer]
GO
ALTER TABLE [dbo].[ProjectVersions]  WITH CHECK ADD  CONSTRAINT [FK_ProjectVersions_Projects] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjectVersions] CHECK CONSTRAINT [FK_ProjectVersions_Projects]
GO
ALTER TABLE [dbo].[ProjectVersions]  WITH CHECK ADD  CONSTRAINT [FK_ProjectVersions_ProjVersionStatus] FOREIGN KEY([ProjVersionStatusID])
REFERENCES [dbo].[ProjVersionStatus] ([ProjVersionStatusID])
GO
ALTER TABLE [dbo].[ProjectVersions] CHECK CONSTRAINT [FK_ProjectVersions_ProjVersionStatus]
GO
ALTER TABLE [dbo].[ProjectVersions]  WITH CHECK ADD  CONSTRAINT [FK_ProjectVersions_Sales] FOREIGN KEY([SalesID])
REFERENCES [dbo].[Sales] ([SalesID])
GO
ALTER TABLE [dbo].[ProjectVersions] CHECK CONSTRAINT [FK_ProjectVersions_Sales]
GO
ALTER TABLE [dbo].[ProjVersionPriceHistory]  WITH CHECK ADD  CONSTRAINT [FK_PriceHistory_Project] FOREIGN KEY([ProjectID])
REFERENCES [dbo].[Projects] ([ProjectID])
GO
ALTER TABLE [dbo].[ProjVersionPriceHistory] CHECK CONSTRAINT [FK_PriceHistory_Project]
GO
ALTER TABLE [dbo].[ProjVersionPriceHistory]  WITH CHECK ADD  CONSTRAINT [FK_PriceHistory_Version] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
GO
ALTER TABLE [dbo].[ProjVersionPriceHistory] CHECK CONSTRAINT [FK_PriceHistory_Version]
GO
ALTER TABLE [dbo].[ProjVersionPriceHistoryBuildings]  WITH CHECK ADD  CONSTRAINT [FK_PriceHistoryBldg_History] FOREIGN KEY([PriceHistoryID])
REFERENCES [dbo].[ProjVersionPriceHistory] ([PriceHistoryID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjVersionPriceHistoryBuildings] CHECK CONSTRAINT [FK_PriceHistoryBldg_History]
GO
ALTER TABLE [dbo].[ProjVersionPriceHistoryLevels]  WITH CHECK ADD  CONSTRAINT [FK_PriceHistoryLevel_Bldg] FOREIGN KEY([PriceHistoryBuildingID])
REFERENCES [dbo].[ProjVersionPriceHistoryBuildings] ([PriceHistoryBuildingID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProjVersionPriceHistoryLevels] CHECK CONSTRAINT [FK_PriceHistoryLevel_Bldg]
GO
ALTER TABLE [dbo].[RawUnitLumberHistory]  WITH CHECK ADD  CONSTRAINT [FK_RawUnitLumberHistory_LumberCostEffective] FOREIGN KEY([CostEffectiveDateID])
REFERENCES [dbo].[LumberCostEffective] ([CostEffectiveID])
GO
ALTER TABLE [dbo].[RawUnitLumberHistory] CHECK CONSTRAINT [FK_RawUnitLumberHistory_LumberCostEffective]
GO
ALTER TABLE [dbo].[RawUnitLumberHistory]  WITH CHECK ADD  CONSTRAINT [FK_RawUnitLumberHistory_RawUnits] FOREIGN KEY([RawUnitID])
REFERENCES [dbo].[RawUnits] ([RawUnitID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RawUnitLumberHistory] CHECK CONSTRAINT [FK_RawUnitLumberHistory_RawUnits]
GO
ALTER TABLE [dbo].[RawUnitLumberHistory]  WITH CHECK ADD  CONSTRAINT [FK_RawUnitLumberHistory_VersionID] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
ON UPDATE CASCADE
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[RawUnitLumberHistory] CHECK CONSTRAINT [FK_RawUnitLumberHistory_VersionID]
GO
ALTER TABLE [dbo].[RawUnits]  WITH CHECK ADD  CONSTRAINT [FK__RawUnits__Produc__5441852A] FOREIGN KEY([ProductTypeID])
REFERENCES [dbo].[ProductType] ([ProductTypeID])
GO
ALTER TABLE [dbo].[RawUnits] CHECK CONSTRAINT [FK__RawUnits__Produc__5441852A]
GO
ALTER TABLE [dbo].[RawUnits]  WITH CHECK ADD  CONSTRAINT [FK_RawUnits_ProjectVersions] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
GO
ALTER TABLE [dbo].[RawUnits] CHECK CONSTRAINT [FK_RawUnits_ProjectVersions]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Estimator] FOREIGN KEY([EstimatorID])
REFERENCES [dbo].[Estimator] ([EstimatorID])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Estimator]
GO
ALTER TABLE [dbo].[VersionUnlockLog]  WITH CHECK ADD  CONSTRAINT [FK_VersionUnlockLog_Version] FOREIGN KEY([VersionID])
REFERENCES [dbo].[ProjectVersions] ([VersionID])
GO
ALTER TABLE [dbo].[VersionUnlockLog] CHECK CONSTRAINT [FK_VersionUnlockLog_Version]
GO
ALTER TABLE [dbo].[AuditLog]  WITH CHECK ADD CHECK  (([Operation]='DELETE' OR [Operation]='UPDATE' OR [Operation]='INSERT'))
GO
ALTER TABLE [dbo].[PL_PriceLocks]  WITH CHECK ADD  CONSTRAINT [CK_PL_PriceLocks_Status] CHECK  (([Status]='Sent' OR [Status]='Approved' OR [Status]='Pending' OR [Status]='Draft'))
GO
ALTER TABLE [dbo].[PL_PriceLocks] CHECK CONSTRAINT [CK_PL_PriceLocks_Status]
GO
