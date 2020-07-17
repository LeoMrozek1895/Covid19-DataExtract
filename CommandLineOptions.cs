using CommandLine;

namespace DataExtract
{
	/// <summary>
	/// Commandline options for the applications using the CommandLine Parser from Nuget
	/// </summary>
	public class CommandLineOptions
	{
		[Option('w', "wait", Required = false, HelpText = "Set to whether to wait to continue when done or to close when done.")]
		public bool WaitForExit { get; set; }

		[Option('f', "force", Required = false, HelpText = "Force a reload even if it has aready been done and rowcounts are the same.")]
		public bool ForceReload { get; set; }
	}
}
