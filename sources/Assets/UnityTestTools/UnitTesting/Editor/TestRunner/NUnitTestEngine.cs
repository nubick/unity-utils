using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NUnit.Core;
using NUnit.Core.Filters;
using System.Linq;

namespace UnityTest
{
	public class NUnitTestEngine : IUnitTestEngine
	{
		private List<UnitTestResult> testList = new List<UnitTestResult>();
		private string[] assembliesWithTests = new[]
			{
				"Assembly-CSharp", "Assembly-CSharp-Editor", 
				"Assembly-Boo", "Assembly-Boo-Editor", 
				"Assembly-UnityScript", "Assembly-UnityScript-Editor"
			};

		public UnitTestResult[] GetTests(bool reload)
		{
			if (reload) ReloadTestList();
			return testList.ToArray();
		}

		public UnitTestResult[] RunTests(string[] tests, UnitTestRunner.ITestRunnerCallback testRunnerEventListener)
		{
			List<String> assemblies = GetAssemblies();
			TestSuite suite = PrepareTestSuite(assemblies);

			ITestFilter filter = TestFilter.Empty;
			if (tests != null && tests.Any())
				filter = new SimpleNameFilter(tests);

			testRunnerEventListener.RunStarted(suite.TestName.FullName, suite.TestCount);

			NUnit.Core.TestResult result = ExecuteTestSuite(suite,
												testRunnerEventListener,
												filter);
			UpdateTestResults(result);

			testRunnerEventListener.RunFinished();

			return testList.ToArray();
		}

		private void ReloadTestList()
		{
			List<String> assemblies = GetAssemblies();
			TestSuite suite = PrepareTestSuite(assemblies);
			UpdateTestResults(suite);
		}

		private void UpdateTestResults(NUnit.Core.TestSuite suite)
		{
			ToUnitTestResult(suite);
		}

		private void UpdateTestResults(NUnit.Core.TestResult result)
		{
			ToUnitTestResult(result);
		}

		private List<String> GetAssemblies()
		{
			var assemblyList = new List<String>();

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				var assemblyName = assembly.GetName().Name;
				if (assembliesWithTests.Contains(assemblyName)) assemblyList.Add (assembly.Location);
			}

			return assemblyList;
		}

		private TestSuite PrepareTestSuite(List<String> assemblyList)
		{
			CoreExtensions.Host.InitializeService();
			var testPackage = new TestPackage("Unity",
												assemblyList);
			var builder = new TestSuiteBuilder();
			TestExecutionContext.CurrentContext.TestPackage = testPackage;
			TestSuite suite = builder.Build(testPackage);


			return suite;
		}

		private NUnit.Core.TestResult ExecuteTestSuite(TestSuite suite, UnitTestRunner.ITestRunnerCallback testRunnerEventListener, ITestFilter filter)
		{
			var result = suite.Run(new TestRunnerEventListener(testRunnerEventListener),
									filter);
			return result;
		}

		private class TestRunnerEventListener : EventListener
		{
			private UnitTestRunner.ITestRunnerCallback testRunnerEventListener;

			public TestRunnerEventListener(UnitTestRunner.ITestRunnerCallback testRunnerEventListener)
			{
				this.testRunnerEventListener = testRunnerEventListener;
			}

			public void RunStarted(string name, int testCount)
			{
				testRunnerEventListener.RunStarted(name, testCount);
			}

			public void RunFinished(NUnit.Core.TestResult result)
			{
				testRunnerEventListener.RunFinished();
			}

			public void RunFinished(Exception exception)
			{
				testRunnerEventListener.RunFinishedException(exception);
			}

			public void TestStarted(NUnit.Core.TestName testName)
			{
				testRunnerEventListener.TestStarted(testName.FullName);
			}

			public void TestFinished(NUnit.Core.TestResult result)
			{
				testRunnerEventListener.TestFinished(result.UnitTestResult());
			}

			public void SuiteStarted(NUnit.Core.TestName testName)
			{
			}

			public void SuiteFinished(NUnit.Core.TestResult result)
			{
			}

			public void UnhandledException(Exception exception)
			{
			}

			public void TestOutput(NUnit.Core.TestOutput testOutput)
			{
			}
		}

		#region Test and TestResult to UnitTestTestResult

		private void ToUnitTestResult(NUnit.Core.Test test)
		{
			if (test.IsSuite)
				ToUnitTestResult(test.Tests);
			else
			{
				if(test is TestMethod)
				{
					var tm = test as TestMethod;
					if( tm.Method.DeclaringType != tm.Method.ReflectedType )
						return;

				}
				UpdateTest(test.UnitTestResult());
			}
		}

		private void ToUnitTestResult(IList list)
		{
			foreach (var obj in list)
			{
				if (obj is TestAssembly && File.Exists((obj as TestAssembly).TestName.FullName))
					(obj as TestResult).SetCurrentAssembly((obj as TestAssembly).TestName.FullName);

				if (obj is NUnit.Core.Test) ToUnitTestResult(obj as NUnit.Core.Test);
				else if (obj is TestResult) ToUnitTestResult(obj as NUnit.Core.TestResult);
			}
		}

		private void ToUnitTestResult(NUnit.Core.TestResult result)
		{
			if (result.HasResults)
			{
				ToUnitTestResult(result.Results);
			}
			else
			{
				UpdateTest(result.UnitTestResult());
			}

		}

		private void UpdateTest(UnitTestResult unitTestResult)
		{
			var result = testList.FirstOrDefault(m => m.Test.Equals(unitTestResult.Test));
			if (result != null)
			{
				result.Update(unitTestResult);
			}
			else
			{
				testList.Add(unitTestResult);
			}
		}

		#endregion
	}
}
