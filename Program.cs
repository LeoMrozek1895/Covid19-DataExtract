using System;
using System.Data;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.FileIO;

using Newtonsoft.Json;

using static System.IO.Path;

namespace DataExtract
{
	public class Program
	{

		public enum TableInstances
		{
			OrigData = 0,
			ActivityData = 1
		};
		/// <summary>
		/// Entry to application.
		/// </summary>
		/// <param name="args">Only parameter is "wait=0" (do not prompt to continue) or "wait=1" (prompt to continue)</param>
		private static void Main(string[] args)
		{
			Console.WriteLine($"Started at: {DateTime.Now:MM/dd/yyyy HH:mm:ss}");
			Console.WriteLine();

			var url = new string[2] { "https://query.data.world/s/tfkx62rinlvkpr7gyji3ktdo3dha5j", "https://query.data.world/s/omp3uuql6d4u2cugslj7kdtbhs4q4k" };
			var fileName = new string[2] { "Covid-19-cases.csv", "Covid-19-Activity.csv" };

			// Old data (ended June 2020)
			//  const string url = "https://query.data.world/s/tfkx62rinlvkpr7gyji3ktdo3dha5j"  
			//  const string fileName = "Covid-19-cases.csv";

			//  New Data Source to beginnning
			//  const string url = "https://query.data.world/s/omp3uuql6d4u2cugslj7kdtbhs4q4k" 
			// const string fileName = "Covid-19-Activity.csv";

			var tableInstance = TableInstances.OrigData;
			var tableId = (int)tableInstance;

			var wait = false;

			if (args.Length > 0)
			{
				var t = args[0].Split(new[] { '=' })[1];
				wait = t != "0";

				var ti = args[1].Split(new[] { '=' })[1];
				tableInstance = ti == "0" ? TableInstances.OrigData : TableInstances.ActivityData ;
				tableId = Convert.ToInt32(ti);
			}

			var drRowData = Dal.RowData().Tables[0].Rows[0];


			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Downloading CSV File");
			using (var client = new WebClient())
			{
				client.DownloadFile(url[tableId], fileName[tableId]);
			}

			var csvFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\{fileName[tableId]}";

			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Loading CSV File to DataTable");


			// This works with Any CSV File
			var dt = GetDataTableFromCsvFile(csvFile);

			var json = JsonConvert.SerializeObject(dt);
			var jsonFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Extract.json";
			using (var sw = new StreamWriter(jsonFile))
			{
				sw.Write(json);
				sw.Close();
				sw.Dispose();
			}


			var lastDate = string.Empty;
			var rowCounts = (int)drRowData["RowCount"];

			if(tableInstance == TableInstances.OrigData){
				lastDate = drRowData["LastDataDate"].ToString();
			}
			
			var csvCounts = dt.Rows.Count;

			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: **** Start Row Data ****");
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}:     Row Count: {rowCounts}");
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}:     Last Date: {lastDate}");
			Console.WriteLine();
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: **** CSV Row Data ****");
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}:     Row Count: {csvCounts}");
			Console.WriteLine();

			if (rowCounts == csvCounts)
			{
				Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: ******** NO CHANGE IN DATA, ABORTING LOAD ********");
				if (wait) return;

				Console.WriteLine();
				Console.WriteLine($"Finished at: {DateTime.Now:MM/dd/yyyy HH:mm:ss}");
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(false);
				return;
			}

			// Truncate Working Table
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Truncating Working Table");
			Dal.TruncateWorkingTable(tableId);

			// Bulk Load Data
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Loading Data to Working Table");
			Dal.BulkLoadData(dt, tableInstance);

			// Truncating Production Table
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Truncating Data in Production Table");
			Dal.TruncateDataTable(tableId);

			// Load Prod Table
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Loading Production Table");


			var ds = Dal.LoadCovidData(tableInstance);

			// Old Code replaced by above
			//	var ds = Dal.LoadCovid19DataTable();

			var dt2 = ds?.Tables[0];
			if (dt2 != null)
			{
				var dr = dt2.Rows[0];
				Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Rows imported from Data World: {dr["ImportCount"]}");
				Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Rows copied to production Table: {dr["CopyCount"]}");
			}

			// Clean up CSV File
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Cleaning up downloaded file");
			if (File.Exists(csvFile)) { File.Delete(csvFile); }

			if (wait) return;

			Console.WriteLine();
			Console.WriteLine($"Finished at: {DateTime.Now:MM/dd/yyyy HH:mm:ss}");
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey(false);
		}

		/// <summary>
		/// Converts CSV File to data table
		/// </summary>
		/// <param name="csvFilePath"></param>
		/// <returns>DataTable of CSV values</returns>
		private static DataTable GetDataTableFromCsvFile(string csvFilePath)
		{
			var csvData = new DataTable();
			try
			{
				using (var csvReader = new TextFieldParser(csvFilePath))
				{
					csvReader.SetDelimiters(new[] { "," });
					csvReader.HasFieldsEnclosedInQuotes = true;
					var colFields = csvReader.ReadFields();
					if (colFields != null)
						foreach (var column in colFields)
						{
							var dateColumn = new DataColumn(column) { AllowDBNull = true };
							csvData.Columns.Add(dateColumn);
						}

					while (!csvReader.EndOfData)
					{
						var fieldData = csvReader.ReadFields();
						//Making empty value as null
						if (fieldData != null)
							for (var i = 0; i < fieldData.Length; i++)
							{
								if ((string)fieldData[i] == "")
								{
									fieldData[i] = null;
								}
							}

						if (fieldData != null) csvData.Rows.Add(fieldData);
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(@"ERROR IN function: GetDataTableFromCsvFile");
				Console.WriteLine($@"Exception: {ex}");
				Console.WriteLine($@"Inner Exception: {Convert.ToString(ex.InnerException)}");
				Console.WriteLine($@"Message: {Convert.ToString(ex.Message)}");
				Console.WriteLine($@"Source: { Convert.ToString(ex.Source)}");
				Console.WriteLine($@"Stack Trace: {Convert.ToString(ex.StackTrace)}");
			}

			return csvData;
		}
	}
}
