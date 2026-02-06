USE [BuildersPSE]
GO
/****** Object:  Table [dbo].[PL_Builders]    Script Date: 2/4/2026 11:00:26 AM ******/
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
/****** Object:  Table [dbo].[PL_ComponentPricing]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_Elevations]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_MaterialCategories]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_MaterialCategoryRLMapping]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_MaterialPricing]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_Options]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_Plans]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_PriceChangeHistory]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_PriceLocks]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_ProductTypes]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_RandomLengthsImport]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_RandomLengthsPricing]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_RandomLengthsPricingType]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_SubdivisionPlans]    Script Date: 2/4/2026 11:00:27 AM ******/
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
/****** Object:  Table [dbo].[PL_Subdivisions]    Script Date: 2/4/2026 11:00:27 AM ******/
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
ALTER TABLE [dbo].[PL_PriceLocks]  WITH CHECK ADD  CONSTRAINT [CK_PL_PriceLocks_Status] CHECK  (([Status]='Sent' OR [Status]='Approved' OR [Status]='Pending' OR [Status]='Draft'))
GO
ALTER TABLE [dbo].[PL_PriceLocks] CHECK CONSTRAINT [CK_PL_PriceLocks_Status]
GO
