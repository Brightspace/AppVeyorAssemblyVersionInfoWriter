using System;
using System.IO;

namespace AppVeyorAssemblyVersionInfoWriter {

	public static class AssemblyInfoVersionWriter {

		public static void Write(
				TextWriter output,
				Version assemblyFileVersion,
				string branch,
				string tag,
				int build
			) {

			int major = assemblyFileVersion.Major;
			int minor = assemblyFileVersion.Minor;
			int patch = assemblyFileVersion.Build;

			string package;
			if( tag.Equals( $"v{ major }.{ minor }.{ patch }" ) ) {
				package = String.Empty;

			} else if( branch.Equals( "master" ) ) {
				package = $"-rc{ build }";

			} else {
				package = $"-alpha{ build }";
			}

			output.WriteLine( $@"using System.Reflection;

[assembly: AssemblyVersion( ""{ major }.{ minor }"" )]
[assembly: AssemblyFileVersion( ""{ major }.{ minor }.{ patch }.{ build }"" )]
[assembly: AssemblyInformationalVersion( ""{ major }.{ minor }.{ patch }{ package }"" )]" );
		}
	}
}
