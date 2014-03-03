namespace UnityTest
{
	public interface IUnitTestEngine
	{
		UnitTestResult[] GetTests (bool reload);
		UnitTestResult[] RunTests(string[] tests, UnitTestRunner.ITestRunnerCallback testRunnerEventListener);
	}
}
