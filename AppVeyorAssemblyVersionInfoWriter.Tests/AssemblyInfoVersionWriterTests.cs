using System;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace AppVeyorAssemblyVersionInfoWriter.Tests {

	[TestFixture]
	public class AssemblyInfoVersionWriterTests {

		[Test]
		public void FeatureBranch() {

			AssertWrite(
					assemblyFileVersion: new Version( 10, 6, 8 ),
					branch: "feature/test",
					tag: String.Empty,
					build: 93,
					expectedCs: @"using System.Reflection;

[assembly: AssemblyVersion( ""10.6"" )]
[assembly: AssemblyFileVersion( ""10.6.8.93"" )]
[assembly: AssemblyInformationalVersion( ""10.6.8-alpha93"" )]
" );
		}

		[Test]
		public void MasterBranch() {

			AssertWrite(
					assemblyFileVersion: new Version( 10, 6, 8 ),
					branch: "master",
					tag: String.Empty,
					build: 93,
					expectedCs: @"using System.Reflection;

[assembly: AssemblyVersion( ""10.6"" )]
[assembly: AssemblyFileVersion( ""10.6.8.93"" )]
[assembly: AssemblyInformationalVersion( ""10.6.8-rc93"" )]
" );
		}

		[Test]
		public void ReleaseTag() {

			AssertWrite(
					assemblyFileVersion: new Version( 10, 6, 8 ),
					branch: "master",
					tag: "v10.6.8",
					build: 93,
					expectedCs: @"using System.Reflection;

[assembly: AssemblyVersion( ""10.6"" )]
[assembly: AssemblyFileVersion( ""10.6.8.93"" )]
[assembly: AssemblyInformationalVersion( ""10.6.8"" )]
" );
		}

		private static void AssertWrite(
				Version assemblyFileVersion,
				string branch,
				string tag,
				int build,
				string expectedCs
			) {

			StringBuilder sb = new StringBuilder();

			using( StringWriter strW = new StringWriter( sb ) ) {

				AssemblyInfoVersionWriter.Write(
						output: strW,
						assemblyFileVersion: assemblyFileVersion,
						branch: branch,
						tag: tag,
						build: build
					);
			}

			string cs = sb.ToString();
			Assert.AreEqual( expectedCs, cs );
		}
	}
}
