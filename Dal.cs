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
		//dbo.usp_Covid19_RowData
		public static DataSet RowData()
		{
			const string sql = "dbo.usp_Covid19_RowData";
			var dataSource = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString();
			var col = new DalParamsCollection();
			return DalHelper.GetDataSet(dataSource, sql, ref col, CommandType.StoredProcedure);
		}

		/// <summary>
		/// Truncates the Working table in the database. 
		/// </summary>
		/// <param name="id">ID = 1 = Old Data, ID = 2 = Activity Data</param>
		/// <returns>True/False of success</returns>
		public static bool TruncateWorkingTable(int id)
		{
			const string sql = "dbo.usp_Covid19_TruncateWorkingTable";
			var dataSource = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString();
			var col = new DalParamsCollection
			{
				new DalParms("@ID", SqlDbType.Int, -1,id)
			};
			return DalHelper.ExecuteNonQuery(dataSource, sql, ref col, CommandType.StoredProcedure);
		}

		/// <summary>
		/// Truncates the Actual datatable. This is because the load takes significant time (over 1 minute) that would cause errors if the page was requested during load.
		/// This runs in a few seconds to prevent that potential error
		/// </summary>
		/// <returns>True/False of success</returns>
		public static bool TruncateDataTable(int tableId)
		{
			const string sql = "dbo.usp_Covid19_TruncateDataTable";
			
			var dataSource = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString();
			var col = new DalParamsCollection
			{
				new DalParms("@ID", SqlDbType.Int, -1, tableId)
			};

			return DalHelper.ExecuteNonQuery(dataSource, sql, ref col, CommandType.StoredProcedure);
		}

		public static void BulkLoadData(DataTable dt, Program.TableInstances ti)
		{
			if (ti == Program.TableInstances.OrigData)
			{
				BulkLoadDataOldData(dt);
			}
			else
			{
				BulkLoadDataActivityData(dt);
			}
		}

		public static void BulkLoadDataActivityData(DataTable dt)
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
		/// Bulk loads the Working table in the database with data from the CSV extract
		/// </summary>
		/// <param name="dt"></param>
		public static void BulkLoadDataOldData(DataTable dt)
		{
			const string schema = "dbo";
			const string tableName = "Covid19-Data-Import";
			var connectionString = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString();
			using (var connection = new SqlConnection(connectionString))

			using (var bulkCopy = new SqlBulkCopy(connection))
			{
				connection.Open();
				bulkCopy.DestinationTableName = $"{schema}.[{tableName}]";

				bulkCopy.ColumnMappings.Add("Case_Type", "Case_Type");
				bulkCopy.ColumnMappings.Add("Cases", "Cases");
				bulkCopy.ColumnMappings.Add("Difference", "Difference");
				bulkCopy.ColumnMappings.Add("Date", "Date");
				bulkCopy.ColumnMappings.Add("Country_Region", "Country_Region");
				bulkCopy.ColumnMappings.Add("Province_State", "Province_State");
				bulkCopy.ColumnMappings.Add("Admin2", "Admin2");
				bulkCopy.ColumnMappings.Add("ISO2", "ISO2");
				bulkCopy.ColumnMappings.Add("ISO3", "ISO3");
				bulkCopy.ColumnMappings.Add("Lat", "Lat");
				bulkCopy.ColumnMappings.Add("Long", "Long");
				bulkCopy.ColumnMappings.Add("Population_Count", "Population_Count");
				bulkCopy.ColumnMappings.Add("Data_Source", "Data_Source");
				bulkCopy.ColumnMappings.Add("Prep_Flow_Runtime", "Prep_Flow_Runtime");

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


		public static DataSet LoadCovidData(Program.TableInstances ti)
		{
			switch (ti)
			{
				case Program.TableInstances.ActivityData:
					return LoadCovid19ActivityTable();

				case Program.TableInstances.OrigData:
					return LoadCovid19DataTable();
			}
			return null;
		}

		/// <summary>
		/// Copies the data from the working table to the production table
		/// </summary>
		/// <returns></returns>
		public static DataSet LoadCovid19DataTable()
		{
			const string sql = "dbo.usp_LoadCovid19DataTable";
			var dataSource = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString();
			var col = new DalParamsCollection();

			return DalHelper.GetDataSet(dataSource, sql, ref col, CommandType.StoredProcedure);
		}

		public static DataSet LoadCovid19ActivityTable()
		{
			const string sql = "dbo.usp_LoadCovid19ActivityTable";
			var dataSource = ConfigurationManager.ConnectionStrings["SiteSqlServer"].ToString();
			var col = new DalParamsCollection();

			return DalHelper.GetDataSet(dataSource, sql, ref col, CommandType.StoredProcedure);
		}
	}
}
