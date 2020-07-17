using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using CommandLine;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;


namespace DataExtract
{
	public class Program
	{
		// Flag as to whether to wait or not
		private static bool WaitForExit { get; set; } = false;
		private static bool ForceReload { get; set; } = false;
		
		/// <summary>
		/// Entry to application.
		/// </summary>
		/// <param name="args">Only parameter is "--wait" (to prompt to continue) </param>
		private static void Main(string[] args)
		{
			Console.WriteLine($"Started at: {DateTime.Now:MM/dd/yyyy HH:mm:ss}");
			Console.WriteLine();

			// Link to the CSV file that contains the data on world.data
			const string url = "https://query.data.world/s/omp3uuql6d4u2cugslj7kdtbhs4q4k";
			
			// Spreadsheet name on world.data
			const string fileName = "Covid-19-Activity.csv";

			// Set the path to the CSV File
			var csvFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + $@"\{fileName}";

			// Reads the command line options and sets the WaitForExit flag
			Parser.Default.ParseArguments<CommandLineOptions>(args)
				.WithParsed<CommandLineOptions>(o =>
				{
					Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: ******** Command Line Arguments passed to application ********");
					Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Wait for Exit. Current Arguments: -w or --wait = {o.WaitForExit}");
					WaitForExit = o.WaitForExit;

					Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Force Reload. Current Arguments: -f or --force = {o.ForceReload}");
					ForceReload = o.ForceReload;

					Console.WriteLine();
				});

			// Gets the number of rows and latest date from of data from the Production datatable
			var drRowData = Dal.RowData().Tables[0].Rows[0];
			var rowCounts = (int)drRowData["RowCount"];
			var lastDate = drRowData["LastDataDate"].ToString();

			// Download the CSV file from data.world
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Downloading CSV File");
			using (var client = new WebClient())
			{
				client.DownloadFile(url, fileName);
			}

			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Loading CSV File to DataTable");
			// This works with Any CSV File to load into a datatable
			var dt = GetDataTableFromCsvFile(csvFile);

			// Get the rowcounts from the CSV Datatable
			var csvCounts = dt.Rows.Count;

			// THis is temp code to extract the data set as JSON. The file is about 244MB so it is only run from Visual Studio for when needed.
			//var json = JsonConvert.SerializeObject(dt);
			//var jsonFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Extract.json";
			//using (var sw = new StreamWriter(jsonFile))
			//{
			//	sw.Write(json);
			//	sw.Close();
			//	sw.Dispose();
			//}

			// Display stats from current data and new data
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: **** Start Row Data ****");
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}:     Row Count: {rowCounts}");
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}:     Last Date: {lastDate}");
			Console.WriteLine();
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: **** CSV Row Data ****");
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}:     Row Count: {csvCounts}");
			Console.WriteLine();

			// If there is no rowcount change, there is no new data
			if (rowCounts == csvCounts && !ForceReload)
			{
				Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: ******** NO CHANGE IN DATA, ABORTING LOAD ********");
				if (!WaitForExit) return;

				Console.WriteLine();
				Console.WriteLine($"Finished at: {DateTime.Now:MM/dd/yyyy HH:mm:ss}");
				Console.WriteLine("Press any key to continue...");
				Console.ReadKey(false);
				Environment.Exit(0);
			}
			else if (rowCounts == csvCounts && ForceReload)
			{
				Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}:******** FORCE RELOADING DATA, ROW COUNT SAME  ********");
			}

			// Truncate working data table of all data
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Truncating Working Table");
			Dal.TruncateData(Dal.DataTables.Working);

			// Bulk Load Data into working datatable
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Loading Data to Working Table");
			Dal.BulkLoadData(dt);

			// Truncating Production data table of all data
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Truncating Data in Production Table");
			Dal.TruncateData(Dal.DataTables.Production);

			// Load production data table from the staging table
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Loading Production Table");
			var ds = Dal.LoadCovidData();

			// Using returned data, compare import count with copy count to verify all data was copied
			var dt2 = ds?.Tables[0];
			if (dt2 != null)
			{
				var dr = dt2.Rows[0];
				Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Rows imported from Data World: {dr["ImportCount"]}");
				Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Rows copied to production Table: {dr["CopyCount"]}");

				if ((int) dr["ImportCount"] != (int) dr["CopyCount"])
				{
					Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Data copied from staging to production table does not equal rows imported. Re-Run when completed or investigate why mismatch.");
				}
			}

			// Clean up CSV File
			Console.WriteLine($"{DateTime.Now:MM/dd/yyyy HH:mm:ss}: Cleaning up downloaded file");
			if (File.Exists(csvFile)) { File.Delete(csvFile); }

			// REturn if not waiting for exit
			if (!WaitForExit) return;

			// Wait for exit
			Console.WriteLine();
			Console.WriteLine($"Finished at: {DateTime.Now:MM/dd/yyyy HH:mm:ss}");
			Console.WriteLine("Press any key to continue...");
			Console.ReadKey(false);
			Environment.Exit(0);
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
