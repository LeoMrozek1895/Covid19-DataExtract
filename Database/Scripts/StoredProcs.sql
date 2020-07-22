USE [DNN-Covid19]
GO

/****** Object:  StoredProcedure [dbo].[usp_LoadCovid19ActivityTable]    Script Date: 7/22/2020 10:12:24 AM ******/
DROP PROCEDURE [dbo].[usp_LoadCovid19ActivityTable]
GO

/****** Object:  StoredProcedure [dbo].[usp_Covid19_TruncateData]    Script Date: 7/22/2020 10:12:24 AM ******/
DROP PROCEDURE [dbo].[usp_Covid19_TruncateData]
GO

/****** Object:  StoredProcedure [dbo].[usp_Covid19_RowData]    Script Date: 7/22/2020 10:12:24 AM ******/
DROP PROCEDURE [dbo].[usp_Covid19_RowData]
GO

/****** Object:  StoredProcedure [dbo].[usp_Covid19_RowData]    Script Date: 7/22/2020 10:12:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_Covid19_RowData]
AS
SELECT COUNT(*) as "RowCount", MAX([REPORT_DATE]) as LastDataDate 
FROM [dbo].[Covid19-Activity]
GO

/****** Object:  StoredProcedure [dbo].[usp_Covid19_TruncateData]    Script Date: 7/22/2020 10:12:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[usp_Covid19_TruncateData]
	@Id		int = 0
AS
IF @ID = 0 BEGIN
	TRUNCATE TABLE [dbo].[Covid19-Activity-Import]
END 

IF @ID = 1 BEGIN
	TRUNCATE TABLE [dbo].[Covid19-Activity]
END 

GO

/****** Object:  StoredProcedure [dbo].[usp_LoadCovid19ActivityTable]    Script Date: 7/22/2020 10:12:24 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[usp_LoadCovid19ActivityTable]
AS

--TRUNCATE TABLE [dbo].[Covid19-Activity-Import]
INSERT INTO [dbo].[Covid19-Activity] ([PEOPLE_POSITIVE_CASES_COUNT], [COUNTY_NAME], [REPORT_DATE], [PROVINCE_STATE_NAME], [CONTINENT_NAME], [DATA_SOURCE_NAME], [PEOPLE_DEATH_NEW_COUNT], [COUNTY_FIPS_NUMBER], [COUNTRY_ALPHA_3_CODE], [COUNTRY_SHORT_NAME], [COUNTRY_ALPHA_2_CODE], [PEOPLE_POSITIVE_NEW_CASES_COUNT], [PEOPLE_DEATH_COUNT])
SELECT 
	  CONVERT(bigint, CONVERT(numeric, [PEOPLE_POSITIVE_CASES_COUNT]))AS [PEOPLE_POSITIVE_CASES_COUNT]
	, [COUNTY_NAME]
	, CONVERT(date, [REPORT_DATE])
	, [PROVINCE_STATE_NAME]
	, [CONTINENT_NAME]
	, [DATA_SOURCE_NAME]
	, CONVERT(bigint, CONVERT(numeric, [PEOPLE_DEATH_NEW_COUNT]))AS [PEOPLE_DEATH_NEW_COUNT]
	, [COUNTY_FIPS_NUMBER]
	, [COUNTRY_ALPHA_3_CODE]
	, [COUNTRY_SHORT_NAME]
	, [COUNTRY_ALPHA_2_CODE]
	, CONVERT(bigint, CONVERT(numeric, [PEOPLE_POSITIVE_NEW_CASES_COUNT]))AS [PEOPLE_POSITIVE_NEW_CASES_COUNT]
	, CONVERT(bigint, CONVERT(numeric, [PEOPLE_DEATH_COUNT]))AS [PEOPLE_DEATH_COUNT]
FROM [dbo].[Covid19-Activity-Import]

DECLARE @ImportCount int
DECLARE @CopyCount int

SELECT @ImportCount = COUNT(*) FROM [dbo].[Covid19-Activity-Import] 
SELECT @CopyCount = COUNT(*) FROM [dbo].[Covid19-Activity] 

SELECT @ImportCount as ImportCount, @CopyCount as CopyCount

ALTER INDEX ALL ON [Covid19-Activity] REBUILD

GO

