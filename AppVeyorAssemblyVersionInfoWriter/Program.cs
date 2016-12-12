using System;
using System.IO;
using System.Text;

namespace AppVeyorAssemblyVersionInfoWriter {

	internal static class Program {

		internal static int Main( string[] arguments ) {

			Arguments args;
			int exitCode = ArgumentsParser.TryParse( Environment.GetEnvironmentVariable, arguments, Console.Error, out args );
			if( exitCode != 0 ) {
				return exitCode;
			}

			args.Output.Directory.Create();

			using( FileStream fs = new FileStream( args.Output.FullName, FileMode.Create, FileAccess.Write, FileShare.Read ) )
			using( StreamWriter sw = new StreamWriter( fs, Encoding.UTF8 ) ) {

				AssemblyInfoVersionWriter.Write(
						output: sw,
						assemblyFileVersion: args.AssemblyFileVersion,
						branch: args.Branch,
						tag: args.Tag,
						build: args.Build
					);
			}

			return 0;
		}
	}
}
