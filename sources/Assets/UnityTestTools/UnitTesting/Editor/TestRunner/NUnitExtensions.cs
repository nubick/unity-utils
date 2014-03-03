using NUnit.Core;

namespace UnityTest
{
	public static class NUnitExtensions
	{
		private static string currentAssemblyPath = "";

		public static UnitTestInfo UnitTestInfo(this ITest test)
		{
			return new UnitTestInfo(test.TestName.FullName);
		}

		public static UnitTestResult UnitTestResult(this NUnit.Core.TestResult result)
		{
			return new UnitTestResult()
			{
				Executed = true,
				ResultState = (TestResultState)result.ResultState,
				Message = result.Message,
				StackTrace = result.StackTrace,
				Duration = result.Time,
				Test = result.Test.UnitTestInfo(),
				AssemblyPath = currentAssemblyPath
			};
		}

		public static void SetCurrentAssembly(this TestResult result, string assemblyPath)
		{
			currentAssemblyPath = assemblyPath;
		}


		public static UnitTestResult UnitTestResult(this NUnit.Core.Test test)
		{
			return new UnitTestResult()
			{
				Message = "",
				StackTrace = "",
				Duration = 0,
				Test = test.UnitTestInfo(),
				AssemblyPath = currentAssemblyPath
			};
		}
	}
}
