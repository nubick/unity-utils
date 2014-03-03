using System.Linq;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using UnityEditorInternal;
using UnityEngine;

namespace UnityTest
{
	public static class GuiHelper
	{
		public static Texture GetIconForTestResult (TestResultState resultState)
		{
			switch (resultState)
			{
				case TestResultState.Failure:
				case TestResultState.Error:
					return Icons.failImg;
				case TestResultState.Success:
					return Icons.successImg;
				case TestResultState.Ignored:
				case TestResultState.Skipped:
					return Icons.ignoreImg;
				case TestResultState.Inconclusive:
					return Icons.inconclusiveImg;
				case TestResultState.Cancelled:
				default:
					return Icons.unknownImg;
			}
		}

		public static Texture GetIconForCategoryResult (TestResultState resultState)
		{
			switch (resultState)
			{
				case TestResultState.Failure:
				case TestResultState.Error:
					return Icons.failImg;
				case TestResultState.Success:
					return Icons.successImg;
				case TestResultState.Cancelled:
				case TestResultState.Inconclusive:
				case TestResultState.NotRunnable:
				case TestResultState.Skipped:
					return Icons.unknownImg;
				case TestResultState.Ignored:
					return Icons.ignoreImg;
				default:
					return null;
			}
		}

		private static int ExtractSourceFileLine(string stackTrace)
		{
			int line = 0;
			if (!string.IsNullOrEmpty(stackTrace))
			{
				var regEx = new Regex(@".* in (?'path'.*):(?'line'\d+)");
				var matches = regEx.Matches(stackTrace);
				for (int i = 0; i < matches.Count; i++)
				{
					line = int.Parse(matches[i].Groups["line"].Value);
					if (line != 0)
						break;
				}
			}
			return line;
		}

		private static string ExtractSourceFilePath(string stackTrace)
		{
			string path = "";
			if (!string.IsNullOrEmpty(stackTrace))
			{
				var regEx = new Regex(@".* in (?'path'.*):(?'line'\d+)");
				var matches = regEx.Matches(stackTrace);
				for (int i = 0; i < matches.Count; i++)
				{
					path = matches[i].Groups["path"].Value;
					if (path != "<filename unknown>")
						break;
				}
			}
			return path;
		}

		public static void OpenInEditor(UnitTestResult test, bool openError)
		{

			var sourceFilePath = ExtractSourceFilePath(test.StackTrace);
			var sourceFileLine = ExtractSourceFileLine(test.StackTrace);

			if (!openError || sourceFileLine == 0 || string.IsNullOrEmpty (sourceFilePath))
			{
				var sp = GetSequencePointOfTest(test);
				sourceFileLine = sp.StartLine;
				sourceFilePath = sp.Document.Url;
			}

			OpenInEditorInternal(sourceFilePath, sourceFileLine);
		}

		public static SequencePoint GetSequencePointOfTest(UnitTestResult test)
		{
			var readerParameters = new ReaderParameters
				{
					ReadSymbols = true,
					SymbolReaderProvider = new MdbReaderProvider (),
					ReadingMode = ReadingMode.Immediate
				};

			var assemblyDefinition = AssemblyDefinition.ReadAssembly (test.AssemblyPath,
																	readerParameters);

			var classModule = assemblyDefinition.MainModule.Types.Single (t => t.FullName == test.Test.FullClassName);
			var method = classModule.Methods.Single (t => t.Name == test.Test.MethodName);
			var sp = method.Body.Instructions.First (i => i.SequencePoint != null).SequencePoint;

			return sp;
		}

		private static void OpenInEditorInternal (string filename, int line)
		{
			InternalEditorUtility.OpenFileAtLineExternal (filename, line);
		}
	}
}
