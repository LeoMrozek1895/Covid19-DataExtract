using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;

namespace DataExtract
{
	/// <summary>
	/// Basic functions used in application/>
	/// </summary>
	//internal static class Functions
	//{
	//	/// <summary>
	//	/// COnverts IList to datatable
	//	/// </summary>
	//	/// <typeparam name="T"></typeparam>
	//	/// <param name="data"></param>
	//	/// <returns></returns>
	//	public static DataTable ToDataTable<T>(this IList<T> data)
	//	{
	//		var props = TypeDescriptor.GetProperties(typeof(T));
	//		var table = new DataTable();
	//		for (var i = 0; i < props.Count; i++)
	//		{
	//			var prop = props[i];
	//			table.Columns.Add(prop.Name, prop.PropertyType);
	//		}

	//		var values = new object[props.Count];
	//		foreach (var item in data)
	//		{
	//			for (var i = 0; i < values.Length; i++)
	//			{
	//				values[i] = props[i].GetValue(item);
	//			}

	//			table.Rows.Add(values);
	//		}

	//		return table;
	//	}
	//}

	/// <summary>
	/// Pulbic class of fiddler functions for retrieving data from Data.World
	/// </summary>
	public class FiddlerExt
	{
		/// <summary>
		/// Calls request functions sequentially.
		/// </summary>
		/// <param name="startRow">Start row for number of rows to return (i.e. 1 for start, 100 for row 100, etc)</param>
		/// <param name="endRow">End row for number of rows to return (i.e. 100 for first 100 rows, 200 to include up to row 200, etc)</param>
		/// <returns>Head, metadata and results from the requested response</returns>
		public Covid19Data MakeRequests(int startRow, int endRow)
		{
			string responseText = null;

			if (Request_query_data_world(startRow, endRow, out var response))
			{
				//Success, possibly use response.
				responseText = ReadResponse(response);
				response.Close();
			}
			else
			{
				//Failure, cannot use response.
			}

			return !string.IsNullOrEmpty(responseText) ? Covid19Data.FromJson(responseText) : null;
		}

		/// <summary>
		/// Returns the text contained in the response.  For example, the page HTML.  Only handles the most common HTTP encodings.
		/// </summary>
		/// <param name="response"></param>
		/// <returns>A string of the data returned in the request</returns>
		private static string ReadResponse(HttpWebResponse response)
		{
			using (var responseStream = response.GetResponseStream())
			{
				var streamToRead = responseStream;
				if (response.ContentEncoding.ToLower().Contains("gzip"))
				{
					streamToRead = new GZipStream(streamToRead ?? throw new InvalidOperationException(), CompressionMode.Decompress);
				}
				else if (response.ContentEncoding.ToLower().Contains("deflate"))
				{
					streamToRead = new DeflateStream(streamToRead ?? throw new InvalidOperationException(), CompressionMode.Decompress);
				}

				using (var streamReader = new StreamReader(streamToRead ?? throw new InvalidOperationException(), Encoding.UTF8))
				{
					return streamReader.ReadToEnd();
				}
			}
		}

		/// <summary>
		/// Tries to request the URL: https://query.data.world/table_view_window/covid-19-data-resource-hub/covid-19-case-counts/covid_19_cases?startRow=0&amp;endRow=200&amp;ascending=true
		/// </summary>
		/// <param name="response">After the function has finished, will possibly contain the response to the request.</param>
		/// <param name="startRow">Request group start row num (0 based)</param>
		/// <param name="endRow">End Row number (200 brings back 200 rows</param>
		/// <returns>True if the request was successful; false otherwise.</returns>
		private bool Request_query_data_world(int startRow, int endRow, out HttpWebResponse response)
		{
			response = null;

			try
			{
				//Create request to URL.
				var request = (HttpWebRequest)WebRequest.Create(
					$"https://query.data.world/table_view_window/covid-19-data-resource-hub/covid-19-case-counts/covid_19_cases?startRow={startRow}&endRow={endRow}&ascending=true");

				//Set request headers.
				request.KeepAlive = true;
				request.Headers.Set(HttpRequestHeader.Pragma, "no-cache");
				request.Headers.Set(HttpRequestHeader.CacheControl, "no-cache");
				request.Accept = "application/sparql-results+json";
				request.Headers.Add("Sec-Fetch-Dest", @"empty");
				request.Headers.Set(HttpRequestHeader.Authorization,
					"Bearer eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiJwcm9kLXVzZXItY2xpZW50Omxlb21yb3plazE4OTUiLCJpc3MiOiJkdzphZ2VudDpsZW9tcm96ZWsxODk1OjpiMWFmYTVjZi0zZmEwLTRhZjYtYTc2OS1mY2U3NjdmMTFlMzQiLCJpYXQiOjE1ODU4MzI4NTYsInJvbGUiOlsidXNlciIsInVzZXJfYXBpX2FkbWluIiwidXNlcl9hcGlfcmVhZCIsInVzZXJfYXBpX3dyaXRlIl0sImV4cCI6MTU5MzYwODg1NiwiZ2VuZXJhbC1wdXJwb3NlIjp0cnVlLCJhdXRob3JpdHlpZHMiOlsiZGF0YWRvdHdvcmxkIl19.tjC-9FIxazaSEM0yfg05njznQ36p3tcbmmSkNEYVWrfibwpGioUc0zp_uuXIlk-qXKMh9KMLQ5jrpvqhGZYW2A");
				request.UserAgent =
					"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";
				request.Headers.Add("DNT", @"1");
				request.Headers.Add("Origin", @"https://data.world");
				request.Headers.Add("Sec-Fetch-Site", @"same-site");
				request.Headers.Add("Sec-Fetch-Mode", @"cors");
				request.Referer = "https://data.world/";
				request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate, br");
				request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.9");

				//Get response to request.
				response = (HttpWebResponse)request.GetResponse();
			}
			catch (WebException e)
			{
				//ProtocolError indicates a valid HTTP response, but with a non-200 status code (e.g. 304 Not Modified, 404 Not Found)
				if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
				else return false;
			}
			catch (Exception)
			{
				if (response != null) response.Close();
				return false;
			}

			return true;
		}
	}
}