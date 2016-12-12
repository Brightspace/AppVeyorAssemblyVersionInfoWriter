using System;
using System.Collections.Generic;
using System.IO;
using NDesk.Options;

namespace AppVeyorAssemblyVersionInfoWriter {

	internal static class ArgumentsParser {

		internal const string AssemblyFileVersionVariable = "ASSEMBLY_FILE_VERSION";
		internal const string BuildNumberVariable = "APPVEYOR_BUILD_NUMBER";
		internal const string BranchVariable = "APPVEYOR_REPO_BRANCH";
		internal const string TagVariable = "APPVEYOR_REPO_TAG_NAME";

		internal static int TryParse(
				Func<string, string> environment,
				string[] arguments,
				TextWriter errors,
				out Arguments args
			) {

			args = null;

			FileInfo outputPath = null;
			Version assemblyFileVersion = null;

			OptionSet options = new OptionSet() {
				{
					"output=",
					"The output file path",
					v => {
						outputPath = new FileInfo( v );
					}
				},
				{
					"assemblyFileVersion=",
					"The assembly file version in the format MAJOR.MINOR.PATCH",
					v => {
						if( !Version.TryParse( v, out assemblyFileVersion ) ) {
							throw new OptionException( $"Invalid version: { v }", "assemblyFileVersion" );
						}
					}
				}
			};

			List<string> extra;
			try {
				extra = options.Parse( arguments );
				if( extra.Count > 0 ) {

					errors.Write( "Invalid arguments: " );
					errors.WriteLine( String.Join( " ", extra ) );

					WriteUsage( options, errors );
					return 1;
				}

			} catch( OptionException e ) {

				errors.Write( "Invalid arguments: " );
				errors.WriteLine( e.Message );

				WriteUsage( options, errors );
				return 2;
			}

			if( outputPath == null ) {

				errors.WriteLine( "Invalid arguments: output path not specified" );

				WriteUsage( options, errors );
				return 3;
			}

			if( assemblyFileVersion == null ) {

				string afv = environment( AssemblyFileVersionVariable );
				if( string.IsNullOrEmpty( afv ) ) {

					errors.WriteLine( $"{ AssemblyFileVersionVariable } environment variable not set" );
					return 4;
				}

				if( !Version.TryParse( afv, out assemblyFileVersion ) ) {

					errors.WriteLine( $"Invalid { AssemblyFileVersionVariable } value: { afv }" );
					return 5;
				}
			}

			if( assemblyFileVersion.Major == -1
				|| assemblyFileVersion.Minor == -1
				|| assemblyFileVersion.Build == -1
				|| assemblyFileVersion.Revision != -1 ) {

				errors.WriteLine( $"Assembly file version must be in the format MAJOR.MINOR.PATCH" );
				return 6;
			}

			int buildNumber;
			{
				string bn = environment( BuildNumberVariable );
				if( string.IsNullOrEmpty( bn ) ) {

					errors.WriteLine( $"{ BuildNumberVariable } environment variable not set" );
					return 7;
				}

				if( !int.TryParse( bn, out buildNumber ) ) {

					errors.WriteLine( $"Invalid { BuildNumberVariable } value: { bn }" );
					return 8;
				}
			}

			string branch = environment( BranchVariable );
			if( String.IsNullOrEmpty( branch ) ) {

				errors.WriteLine( $"{ BranchVariable } environment variable not set" );
				return 9;
			}

			string tag = environment( TagVariable ) ?? String.Empty;

			args = new Arguments {
				Output = outputPath,
				AssemblyFileVersion = assemblyFileVersion,
				Branch = branch,
				Tag = tag,
				Build = buildNumber
			};

			return 0;
		}

		private static void WriteUsage( OptionSet options, TextWriter output ) {

			output.WriteLine( "Usage:" );
			options.WriteOptionDescriptions( output );
		}
	}
}
