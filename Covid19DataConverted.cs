using System;

namespace DataExtract
{
	class Covid19DataConverted
	{
		public string CaseType { get; set; }
		public string Cases { get; set; }
		public int Difference { get; set; }
		public DateTime Date { get; set; }
		public string CountryRegion { get; set; }
		public string ProvinceState { get; set; }
		public decimal Lat { get; set; }
		public decimal Long { get; set; }
		public DateTime PrepFlowRuntime { get; set; }
		public string TableNames { get; set; }
	}
}
