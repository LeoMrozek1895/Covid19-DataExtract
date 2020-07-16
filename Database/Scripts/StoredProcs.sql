USE [DNN-Covid19]
GO

/****** Object:  Table [dbo].[Covid19-Activity]    Script Date: 7/16/2020 1:22:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Covid19-Activity](
	[PEOPLE_POSITIVE_CASES_COUNT] [bigint] NULL,
	[COUNTY_NAME] [nvarchar](33) NULL,
	[REPORT_DATE] [date] NULL,
	[PROVINCE_STATE_NAME] [nvarchar](25) NULL,
	[CONTINENT_NAME] [nvarchar](14) NULL,
	[DATA_SOURCE_NAME] [nvarchar](50) NULL,
	[PEOPLE_DEATH_NEW_COUNT] [bigint] NULL,
	[COUNTY_FIPS_NUMBER] [nvarchar](18) NULL,
	[COUNTRY_ALPHA_3_CODE] [nvarchar](20) NULL,
	[COUNTRY_SHORT_NAME] [nvarchar](33) NULL,
	[COUNTRY_ALPHA_2_CODE] [nvarchar](20) NULL,
	[PEOPLE_POSITIVE_NEW_CASES_COUNT] [bigint] NULL,
	[PEOPLE_DEATH_COUNT] [bigint] NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Covid19-Activity-Import]    Script Date: 7/16/2020 1:22:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Covid19-Activity-Import](
	[PEOPLE_POSITIVE_CASES_COUNT] [nvarchar](50) NULL,
	[COUNTY_NAME] [nvarchar](33) NULL,
	[REPORT_DATE] [nvarchar](50) NULL,
	[PROVINCE_STATE_NAME] [nvarchar](25) NULL,
	[CONTINENT_NAME] [nvarchar](14) NULL,
	[DATA_SOURCE_NAME] [nvarchar](50) NULL,
	[PEOPLE_DEATH_NEW_COUNT] [nvarchar](50) NULL,
	[COUNTY_FIPS_NUMBER] [nvarchar](18) NULL,
	[COUNTRY_ALPHA_3_CODE] [nvarchar](20) NULL,
	[COUNTRY_SHORT_NAME] [nvarchar](33) NULL,
	[COUNTRY_ALPHA_2_CODE] [nvarchar](20) NULL,
	[PEOPLE_POSITIVE_NEW_CASES_COUNT] [nvarchar](50) NULL,
	[PEOPLE_DEATH_COUNT] [nvarchar](50) NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Covid19-Data]    Script Date: 7/16/2020 1:22:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Covid19-Data](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Case_Type] [nvarchar](10) NULL,
	[Cases] [int] NULL,
	[Difference] [int] NULL,
	[Date] [date] NULL,
	[Country_Region] [nvarchar](40) NULL,
	[Province_State] [nvarchar](40) NULL,
	[Admin2] [nvarchar](100) NULL,
	[ISO2] [nvarchar](100) NULL,
	[ISO3] [nvarchar](100) NULL,
	[FIPS] [nvarchar](10) NULL,
	[Lat] [decimal](11, 8) NULL,
	[Long] [decimal](11, 8) NULL,
	[Population_Count] [int] NULL,
	[Data_Source] [nvarchar](100) NULL,
	[Prep_Flow_Runtime] [datetime] NULL,
	[CreatedDateTime] [datetime] NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Covid19-Data-Import]    Script Date: 7/16/2020 1:22:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Covid19-Data-Import](
	[Case_Type] [nvarchar](20) NULL,
	[Cases] [int] NULL,
	[Difference] [int] NULL,
	[Date] [date] NULL,
	[Country_Region] [nvarchar](40) NULL,
	[Province_State] [nvarchar](40) NULL,
	[Admin2] [nvarchar](100) NULL,
	[ISO2] [nvarchar](100) NULL,
	[ISO3] [nvarchar](100) NULL,
	[FIPS] [nvarchar](100) NULL,
	[Lat] [decimal](11, 8) NULL,
	[Long] [decimal](11, 8) NULL,
	[Population_Count] [int] NULL,
	[Data_Source] [nvarchar](100) NULL,
	[Prep_Flow_Runtime] [nvarchar](25) NULL
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Covid19-Lookups]    Script Date: 7/16/2020 1:22:24 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Covid19-Lookups](
	[Id] [int] IDENTITY(0,1) NOT NULL,
	[ParentId] [int] NOT NULL,
	[Description] [nvarchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_Covid19-Lookups] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Covid19-Data] ADD  CONSTRAINT [DF__Covid19-D__Creat__5E2AE217]  DEFAULT (getdate()) FOR [CreatedDateTime]
GO

ALTER TABLE [dbo].[Covid19-Lookups] ADD  CONSTRAINT [DF_Covid19-Lookups_ParentId]  DEFAULT ((0)) FOR [ParentId]
GO

ALTER TABLE [dbo].[Covid19-Lookups] ADD  CONSTRAINT [DF_Covid19-Lookups_Description]  DEFAULT (N'') FOR [Description]
GO

ALTER TABLE [dbo].[Covid19-Lookups] ADD  CONSTRAINT [DF_Covid19-Lookups_SortOrder]  DEFAULT ((0)) FOR [SortOrder]
GO

