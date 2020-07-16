using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace DataExtract
{
	/// <summary>
	/// Collection class that holds a collection of parameters passed to a Stored procedure in the DAL
	/// </summary>
	public class DalParamsCollection : CollectionBase
	{
		/// <summary>
		/// Add parameter to collection
		/// </summary>
		/// <param name="parm"></param>
		public void Add(DalParms parm)
		{
			List.Add(parm);
		}

		/// <summary>
		/// Get parameter from collection
		/// </summary>
		/// <param name="index"></param>
		/// <returns>Returns the parameter at the index requested</returns>
		public DalParms Item(int index)
		{
			return (DalParms)List[index];
		}

		/// <summary>
		/// Removes a parameter from collection
		/// </summary>
		/// <param name="index"></param>
		public void Remove(int index)
		{
			if (index > Count - 1 | index < 0)
				return;
			List.RemoveAt(index);
		}
	}

	/// <summary>
	/// SQL Server parameter that is to be added to a collection
	/// </summary>
	public class DalParms
	{
		/// <summary>
		/// Parameter to be added to a procedure call
		/// </summary>
		/// <param name="parmName">Name of the parameter (e.g. @id)</param>
		/// <param name="dbType">Type of parameter (e.g. SqlDbType.NVarChar)</param>
		/// <param name="length">Length of parameter (e.g. NVarchar field of 50, int's and other numbers get a value of -1)</param>
		/// <param name="value">Value of the paramber</param>
		public DalParms(string parmName, SqlDbType dbType, int? length, object value)
		{
			ParmName = parmName;
			SqlDbType = dbType;
			Length = length;
			Value = value;
		}

		public int? Length { get; set; }
		public string ParmName { get; set; }
		public SqlDbType SqlDbType { get; set; }
		public object Value { get; set; }
	}

	internal static class DalHelper
	{
		/// <summary>
		/// Executes and returns a DataReader to the calling procedure.
		/// </summary>
		/// <param name="connString"></param>
		/// <param name="strSql"></param>
		/// <param name="parms"></param>
		/// <param name="commandType"></param>
		/// <returns></returns>
		public static SqlDataReader ExecuteDataReader(String connString, string strSql, ref DalParamsCollection parms, CommandType commandType = CommandType.Text)
		{
			try
			{
				using (var cn = new SqlConnection(connString))
				{
					cn.Open();
					do
					{
						Thread.Sleep(100);
					} while (cn.State != ConnectionState.Open);

					using (var cmd = new SqlCommand(strSql, cn) { CommandType = commandType })
					{
						// var dependency = new SqlDependency(cmd);
						//dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);

						foreach (DalParms itm in parms)
						{
							var parm = new SqlParameter(itm.ParmName, itm.SqlDbType);
							if (itm.Length.HasValue)
							{
								parm.Size = Convert.ToInt32(itm.Length);
							}

							parm.Value = itm.Value;
							cmd.Parameters.Add(parm);
						}

						var dr = cmd.ExecuteReader();
						return dr;
					}
				}
			}
			catch (Exception ex)
			{
				WriteToConsole("ExecuteDataReader", strSql, parms, ex);
			}

			return null;
		}

		/// <summary>
		/// Executes a query against a database that has no return. Return whether the query succeeded or not
		/// </summary>
		/// <param name="connString">Connection string to database</param>
		/// <param name="strSql">SQL of the command (e.g. name of stored procedure or SQL command)</param>
		/// <param name="parms">Collection of parameters of null if none</param>
		/// <param name="cmdType">CommandType (e.g. StoredProcedure or Text)</param>
		/// <param name="timeOut">Timeout in seconds</param>
		/// <returns></returns>
		public static bool ExecuteNonQuery(String connString, string strSql, ref DalParamsCollection parms, CommandType cmdType, int timeOut = 600)
		{
			if (timeOut <= 0) throw new ArgumentOutOfRangeException(nameof(timeOut));

			var blnRet = false;
			try
			{
				using (var cn = new SqlConnection(connString))
				{
					try
					{
						var cnt = 0;
						cn.Open();
						do
						{
							Thread.Sleep(100);
							cnt += 1;
							if (cnt == 100)
							{
								return false;
							} // give it 10 seconds and give up
						} while (cn.State != ConnectionState.Open);

						using (var cmd = new SqlCommand(strSql, cn) { CommandType = cmdType })
						{
							foreach (DalParms itm in parms)
							{
								var parm = new SqlParameter(itm.ParmName, itm.SqlDbType);
								if (itm.Length.HasValue)
								{
									parm.Size = Convert.ToInt32(itm.Length);
								}

								parm.Value = itm.Value;
								cmd.Parameters.Add(parm);
							}

							blnRet = Convert.ToBoolean(cmd.ExecuteNonQuery());
						}
					}
					finally
					{
						if (cn.State != ConnectionState.Closed)
							cn.Close();
					}
				}
			}
			catch (Exception ex)
			{
				WriteToConsole("ExecuteNonQuery", strSql, parms, ex);
			}
			return blnRet;
		}

		/// <summary>
		/// Executes a query against a database that returns a 1 row/1 column value
		/// </summary>
		/// <param name="connString">Connection string to database</param>
		/// <param name="strSql">SQL of the command (e.g. name of stored procedure or SQL command)</param>
		/// <param name="parms">Collection of parameters of null if none</param>
		/// <param name="cmdType">CommandType (e.g. StoredProcedure or Text)</param>
		/// <returns></returns>
		public static object ExecuteScalar(String connString, string strSql, ref DalParamsCollection parms, CommandType cmdType)
		{
			object ret = null;
			try
			{
				using (var cn = new SqlConnection(connString))
				{
					try
					{
						cn.Open();
						do { Thread.Sleep(100); } while (cn.State != ConnectionState.Open);

						using (var cmd = new SqlCommand(strSql, cn) { CommandType = cmdType })
						{
							// var dependency = new SqlDependency(cmd);
							//dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);

							foreach (DalParms itm in parms)
							{
								var parm = new SqlParameter(itm.ParmName, itm.SqlDbType);
								if (itm.Length.HasValue)
								{
									parm.Size = Convert.ToInt32(itm.Length);
								}

								parm.Value = itm.Value;
								cmd.Parameters.Add(parm);
							}

							ret = cmd.ExecuteScalar();
						}
					}
					finally
					{
						if (cn.State != ConnectionState.Closed)
							cn.Close();
					}
				}
			}
			catch (Exception ex)
			{
				WriteToConsole("ExecuteScalar", strSql, parms, ex);
			}
			return ret;
		}

		/// <summary>
		/// Returns a .NET DataSet
		/// </summary>
		/// <param name="connString">Connection string to database</param>
		/// <param name="strSql">SQL of the command (e.g. name of stored procedure or SQL command)</param>
		/// <param name="parms">Collection of parameters of null if none</param>
		/// <param name="cmdType">CommandType (e.g. StoredProcedure or Text)</param>
		/// <returns>Dataset from database</returns>
		public static DataSet GetDataSet(String connString, string strSql, ref DalParamsCollection parms, CommandType cmdType)
		{
			try
			{
				using (var cn = new SqlConnection(connString))
				{
					try
					{
						cn.Open();

						do
						{
							Thread.Sleep(100);
						} while (cn.State != ConnectionState.Open);

						using (var cmd = new SqlCommand(strSql, cn) { CommandType = cmdType })
						{
							// var dependency = new SqlDependency(cmd);
							//dependency.OnChange += new OnChangeEventHandler(OnDependencyChange);

							foreach (DalParms itm in parms)
							{
								var parm = new SqlParameter(itm.ParmName, itm.SqlDbType);
								if (itm.Length.HasValue)
								{
									parm.Size = Convert.ToInt32(itm.Length);
								}

								parm.Value = itm.Value;
								cmd.Parameters.Add(parm);
							}


							var da = new SqlDataAdapter(cmd);
							var ds = new DataSet();
							da.Fill(ds);
							return ds;
						}
					}
					finally
					{
						if (cn.State != ConnectionState.Closed)
							cn.Close();
					}
				}
			}
			catch (Exception ex)
			{
				WriteToConsole("GetDataSet", strSql, parms, ex);
				return null;
			}
		}

		/// <summary>
		/// Outputs any errors to the Console window
		/// </summary>
		/// <param name="methodName"></param>
		/// <param name="strSql"></param>
		/// <param name="parms"></param>
		/// <param name="ex"></param>
		private static void WriteToConsole(string methodName, string strSql, IEnumerable parms, Exception ex)
		{
			Console.WriteLine(@"ERROR IN function: " + methodName);
			Console.WriteLine(@"Exception: " + Convert.ToString(ex));
			Console.WriteLine(@"Inner Exception: " + Convert.ToString(ex.InnerException));
			Console.WriteLine(@"Message: " + Convert.ToString(ex.Message));
			Console.WriteLine(@"Source: " + Convert.ToString(ex.Source));
			Console.WriteLine(@"Stack Trace: " + Convert.ToString(ex.StackTrace));
			Console.WriteLine(@"SQL: " + strSql);
			Console.WriteLine(@"Parameters:");

			foreach (var itm in parms.Cast<DalParms>().Where(itm => itm.Length.HasValue))
			{
				Console.WriteLine("Name: " + itm.ParmName + " - Value: " + Convert.ToString(itm.Value));
			}
		}
	}
}