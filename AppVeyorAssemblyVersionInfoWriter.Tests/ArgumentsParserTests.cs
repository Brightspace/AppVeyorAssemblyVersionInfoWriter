using System;
using System.IO;
using System.Text;
using Moq;
using NUnit.Framework;

namespace AppVeyorAssemblyVersionInfoWriter.Tests {

	[TestFixture]
	internal sealed class ArgumentsParserTests {

		[Test]
		public void AssemblyFileVersionArg() {

			Mock<Func<string, string>> env = new Mock<Func<string, string>>( MockBehavior.Strict );

			env
				.Setup( e => e( ArgumentsParser.BranchVariable ) )
				.Returns( "master" );

			env
				.Setup( e => e( ArgumentsParser.TagVariable ) )
				.Returns( "tagger" );

			env
				.Setup( e => e( ArgumentsParser.BuildNumberVariable ) )
				.Returns( "3433" );

			string[] arguments = new[] {
				"--assemblyFileVersion", "10.6.9",
				"--output", "versions.cs"
			};

			Arguments args = AssertSuccessfulTryParse( env.Object, arguments );
			Assert.AreEqual( "versions.cs", args.Output.Name );
			Assert.AreEqual( new Version( 10, 6, 9 ), args.AssemblyFileVersion );
			Assert.AreEqual( "master", args.Branch );
			Assert.AreEqual( "tagger", args.Tag );
			Assert.AreEqual( 3433, args.Build );
		}

		[Test]
		public void EnvironmentVariables() {

			Mock<Func<string, string>> env = new Mock<Func<string, string>>( MockBehavior.Strict );

			env
				.Setup( e => e( ArgumentsParser.AssemblyFileVersionVariable ) )
				.Returns( "10.6.9" );

			env
				.Setup( e => e( ArgumentsParser.BranchVariable ) )
				.Returns( "master" );

			env
				.Setup( e => e( ArgumentsParser.TagVariable ) )
				.Returns( "tagger" );

			env
				.Setup( e => e( ArgumentsParser.BuildNumberVariable ) )
				.Returns( "3433" );

			string[] arguments = new[] {
				"--output", "versions.cs"
			};

			Arguments args = AssertSuccessfulTryParse( env.Object, arguments );
			Assert.AreEqual( "versions.cs", args.Output.Name );
			Assert.AreEqual( new Version( 10, 6, 9 ), args.AssemblyFileVersion );
			Assert.AreEqual( "master", args.Branch );
			Assert.AreEqual( "tagger", args.Tag );
			Assert.AreEqual( 3433, args.Build );
		}

		private Arguments AssertSuccessfulTryParse(
				Func<string, string> environment,
				string[] arguments
			) {

			StringBuilder errors = new StringBuilder();
			using( StringWriter errorsWriter = new StringWriter( errors ) ) {

				Arguments args;

				int code = ArgumentsParser.TryParse(
						environment,
						arguments,
						errorsWriter,
						out args
					);

				Assert.IsEmpty( errors.ToString(), "No errors should have been output" );
				Assert.AreEqual( 0, code, "0 exit code expected" );

				return args;
			}
		}
	}
}
