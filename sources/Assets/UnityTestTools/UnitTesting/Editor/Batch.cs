using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityTest.UnitTestRunner;

namespace UnityTest
{
	public static partial class Batch
	{
		private static string resultFilePathParam = "-resultFilePath=";
		private static string testFilterParam = "-filter=";
		private static string categoryParam = "-categories=";
		private static string defaultResultFileName = "UnitTestResults.xml";

		public static int RETURN_CODE_TESTS_OK = 0;
		public static int RETURN_CODE_TESTS_FAILED = 2;
		public static int RETURN_CODE_RUN_ERROR = 3;

		public static void RunUnitTests ()
		{
			var filter = GetTestFilter ();
			var resultFilePath = GetParameterArgument (resultFilePathParam) ?? Directory.GetCurrentDirectory ();
			if (Directory.Exists (resultFilePath))
				resultFilePath = Path.Combine (resultFilePath, defaultResultFileName);
			EditorApplication.NewScene ();
			var engine = new NUnitTestEngine ();
			UnitTestResult[] results;
			string[] categories;
			engine.GetTests (out results, out categories);
			engine.RunTests (filter, new TestRunnerEventListener (resultFilePath,results.ToList()));
		}

		private static TestFilter GetTestFilter ()
		{
			var testFilterArg = GetParameterArgumentArray (testFilterParam);
			var testCategoryArg = GetParameterArgumentArray (categoryParam);
			var filter = new TestFilter ()
			{
				names = testFilterArg,
				categories = testCategoryArg
			};
			return filter;
		}

		private static string[] GetParameterArgumentArray (string parameterName)
		{
			var arg = GetParameterArgument(parameterName);
			if (string.IsNullOrEmpty(arg)) return null;
			return arg.Split(',').Select(s => s.Trim()).ToArray();
		}

		private static string GetParameterArgument ( string parameterName )
		{
			foreach (var arg in Environment.GetCommandLineArgs ())
			{
				if (arg.ToLower ().StartsWith (parameterName.ToLower ()))
				{
					return arg.Substring (parameterName.Length);
				}
			}
			return null;
		}

		private class TestRunnerEventListener : ITestRunnerCallback
		{
			private string resultFilePath;
			private List<UnitTestResult> results;

			public TestRunnerEventListener ( string resultFilePath, List<UnitTestResult> resultList )
			{
				this.resultFilePath = resultFilePath;
				this.results = resultList;
			}

			public void TestFinished (ITestResult test)
			{
				results.Single( r=>r.Id == test.Id).Update(test, false);
			}

			public void RunFinished ()
			{
				var resultDestiantion = Application.dataPath;
				if (!string.IsNullOrEmpty (resultFilePath))
					resultDestiantion = resultFilePath;
				var fileName = Path.GetFileName (resultDestiantion);
				if (!string.IsNullOrEmpty (fileName))
					resultDestiantion = resultDestiantion.Substring (0, resultDestiantion.Length - fileName.Length);
				else
					fileName = "UnitTestResults.xml";
#if !UNITY_METRO
				var resultWriter = new XmlResultWriter ("Unit Tests", results.ToArray ());
				resultWriter.WriteToFile (resultDestiantion, fileName);
#endif
				var executed = results.Where( result => result.Executed );
				if (!executed.Any ())
				{
					EditorApplication.Exit(RETURN_CODE_RUN_ERROR);
					return;
				}
				var failed = executed.Where (result => !result.IsSuccess);
				EditorApplication.Exit(failed.Any() ? RETURN_CODE_TESTS_FAILED : RETURN_CODE_TESTS_OK);
			}

			public void TestStarted (string fullName)
			{
			}

			public void RunStarted (string suiteName, int testCount)
			{
			}

			public void RunFinishedException (Exception exception)
			{
				EditorApplication.Exit(RETURN_CODE_RUN_ERROR);
				throw exception;
			}
		}
	}
}
