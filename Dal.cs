using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace DataExtract
{
	/// <summary>
	/// Data Access Layer for the application.
	/// </summary>
	public static class Dal
	{
		public enum DataTables
		{
			Working = 0,
			Production = 1
		}

		/// <summary>
		/// Brings back the latest date in the data and the number of rows in the complete dataset
		/// </summary>
		/// <returns></returns>
		public static DataSet RowData()
		{
			const string sql = "dbo.usp_Covid19_RowData";
			var dataSource = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString();
			var col = new DalParamsCollection();
			return DalHelper.GetDataSet(dataSource, sql, ref col, CommandType.StoredProcedure);
		}

		//DataTables
		//[usp_Covid19_TruncateData]
		public static bool TruncateData(DataTables dtEnum)
		{
			const string sql = "dbo.usp_Covid19_TruncateData";
			var dataSource = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString();
			var col = new DalParamsCollection
			{
				new DalParms("@Id", SqlDbType.Int, null, (int)dtEnum),

			};
			return DalHelper.ExecuteNonQuery(dataSource, sql, ref col, CommandType.StoredProcedure);
		}

		/// <summary>
		/// Bulk Loads the data into the Activity-Import Table
		/// </summary>
		/// <param name="dt"></param>
		public static void BulkLoadData(DataTable dt)
		{
			const string schema = "dbo";
			const string tableName = "Covid19-Activity-Import";
			var connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString();
			using (var connection = new SqlConnection(connectionString))

			using (var bulkCopy = new SqlBulkCopy(connection))
			{
				connection.Open();
				bulkCopy.DestinationTableName = $"{schema}.[{tableName}]";

				bulkCopy.ColumnMappings.Add("PEOPLE_POSITIVE_CASES_COUNT", "PEOPLE_POSITIVE_CASES_COUNT");
				bulkCopy.ColumnMappings.Add("COUNTY_NAME", "COUNTY_NAME");
				bulkCopy.ColumnMappings.Add("REPORT_DATE", "REPORT_DATE");
				bulkCopy.ColumnMappings.Add("PROVINCE_STATE_NAME", "PROVINCE_STATE_NAME");
				bulkCopy.ColumnMappings.Add("CONTINENT_NAME", "CONTINENT_NAME");
				bulkCopy.ColumnMappings.Add("DATA_SOURCE_NAME", "DATA_SOURCE_NAME");
				bulkCopy.ColumnMappings.Add("PEOPLE_DEATH_NEW_COUNT", "PEOPLE_DEATH_NEW_COUNT");
				bulkCopy.ColumnMappings.Add("COUNTY_FIPS_NUMBER", "COUNTY_FIPS_NUMBER");
				bulkCopy.ColumnMappings.Add("COUNTRY_ALPHA_3_CODE", "COUNTRY_ALPHA_3_CODE");
				bulkCopy.ColumnMappings.Add("COUNTRY_SHORT_NAME", "COUNTRY_SHORT_NAME");
				bulkCopy.ColumnMappings.Add("COUNTRY_ALPHA_2_CODE", "COUNTRY_ALPHA_2_CODE");
				bulkCopy.ColumnMappings.Add("PEOPLE_POSITIVE_NEW_CASES_COUNT", "PEOPLE_POSITIVE_NEW_CASES_COUNT");
				bulkCopy.ColumnMappings.Add("PEOPLE_DEATH_COUNT", "PEOPLE_DEATH_COUNT");
				try
				{
					bulkCopy.WriteToServer(dt);
				}
				catch (Exception e)
				{
					Console.Write(e.Message);
				}
			}
		}

		/// <summary>
		/// Copies the data from the working table to the production table
		/// </summary>
		/// <returns></returns>
		public static DataSet LoadCovidData()
		{
			const string sql = "dbo.usp_LoadCovid19ActivityTable";
			var dataSource = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString();
			var col = new DalParamsCollection();

			return DalHelper.GetDataSet(dataSource, sql, ref col, CommandType.StoredProcedure);
		}
	}
}
