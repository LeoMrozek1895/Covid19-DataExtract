USE [DNN-Covid19]
GO

/****** Object:  Table [dbo].[Covid19-Activity]    Script Date: 7/16/2020 1:21:36 PM ******/
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

/****** Object:  Table [dbo].[Covid19-Activity-Import]    Script Date: 7/16/2020 1:21:36 PM ******/
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

/****** Object:  Table [dbo].[Covid19-Data]    Script Date: 7/16/2020 1:21:36 PM ******/
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

/****** Object:  Table [dbo].[Covid19-Data-Import]    Script Date: 7/16/2020 1:21:36 PM ******/
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

/****** Object:  Table [dbo].[Covid19-Lookups]    Script Date: 7/16/2020 1:21:36 PM ******/
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

/****** Object:  Table [dbo].[Covid19-Survey]    Script Date: 7/16/2020 1:21:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Covid19-Survey](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[EmailAddress] [nvarchar](255) NOT NULL,
	[ZipCode] [nvarchar](10) NOT NULL,
	[FirstSymptoms] [date] NOT NULL,
	[NumWithSymptoms] [int] NOT NULL,
	[NumWithoutSymptoms] [int] NOT NULL,
	[IntlTravel14Days] [bit] NOT NULL,
	[IntlTravel14DaysWhere] [nvarchar](100) NOT NULL,
	[USTravel14Days] [bit] NOT NULL,
	[USTravel14DaysWhere] [nvarchar](100) NOT NULL,
	[Fever] [bit] NOT NULL,
	[Cough] [bit] NOT NULL,
	[Confusion] [bit] NOT NULL,
	[ChestPressure] [bit] NOT NULL,
	[BluishFaceLips] [bit] NOT NULL,
	[LossOfTaste] [bit] NOT NULL,
	[LossOfSmell] [bit] NOT NULL,
	[DifficultyBreathing] [bit] NOT NULL,
	[NauseaVomiting] [bit] NOT NULL,
	[AchesPainsHeadache] [bit] NOT NULL,
	[DateTimeCreated] [datetime] NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_Covid19-Survey] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

/****** Object:  Table [dbo].[Covid19-Survey-Symptoms]    Script Date: 7/16/2020 1:21:36 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Covid19-Survey-Symptoms](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[SurveyId] [int] NOT NULL,
	[SymptomId] [int] NOT NULL,
 CONSTRAINT [PK_Covid19-Survey-Symptoms] PRIMARY KEY CLUSTERED 
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

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_UserId]  DEFAULT ((-1)) FOR [UserId]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_NumWithSymptoms]  DEFAULT ((0)) FOR [NumWithSymptoms]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_NumWithoutSymptoms]  DEFAULT ((0)) FOR [NumWithoutSymptoms]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_IntlTravel14Days]  DEFAULT ((0)) FOR [IntlTravel14Days]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_IntlTravel14DaysWhere]  DEFAULT (N'') FOR [IntlTravel14DaysWhere]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Table_1_IntlTravel14Days1]  DEFAULT ((0)) FOR [USTravel14Days]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Table_1_IntlTravel14DaysWhere1]  DEFAULT (N'') FOR [USTravel14DaysWhere]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_Fever]  DEFAULT ((0)) FOR [Fever]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_Cough]  DEFAULT ((0)) FOR [Cough]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_Cough1]  DEFAULT ((0)) FOR [Confusion]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_ChestPressure]  DEFAULT ((0)) FOR [ChestPressure]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_Cough1_1]  DEFAULT ((0)) FOR [BluishFaceLips]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_Cough1_2]  DEFAULT ((0)) FOR [LossOfTaste]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_Cough1_3]  DEFAULT ((0)) FOR [LossOfSmell]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_Cough1_4]  DEFAULT ((0)) FOR [DifficultyBreathing]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_Cough1_5]  DEFAULT ((0)) FOR [NauseaVomiting]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_AchesPainsHeadache]  DEFAULT ((0)) FOR [AchesPainsHeadache]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_DateTimeCreated]  DEFAULT (getdate()) FOR [DateTimeCreated]
GO

ALTER TABLE [dbo].[Covid19-Survey] ADD  CONSTRAINT [DF_Covid19-Survey_UserGuid]  DEFAULT (newid()) FOR [UserGuid]
GO

ALTER TABLE [dbo].[Covid19-Survey-Symptoms]  WITH CHECK ADD  CONSTRAINT [FK_Covid19-Survey-Symptoms_Covid19-Lookups] FOREIGN KEY([SymptomId])
REFERENCES [dbo].[Covid19-Lookups] ([Id])
GO

ALTER TABLE [dbo].[Covid19-Survey-Symptoms] CHECK CONSTRAINT [FK_Covid19-Survey-Symptoms_Covid19-Lookups]
GO

ALTER TABLE [dbo].[Covid19-Survey-Symptoms]  WITH CHECK ADD  CONSTRAINT [FK_Covid19-Survey-Symptoms_Covid19-Survey1] FOREIGN KEY([SurveyId])
REFERENCES [dbo].[Covid19-Survey] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[Covid19-Survey-Symptoms] CHECK CONSTRAINT [FK_Covid19-Survey-Symptoms_Covid19-Survey1]
GO

