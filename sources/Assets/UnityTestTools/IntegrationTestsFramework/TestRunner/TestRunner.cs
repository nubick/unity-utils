//#define IMITATE_BATCH_MODE //uncomment if you want to imitate batch mode behaviour in non-batch mode mode run
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityTest.IntegrationTestRunner;

namespace UnityTest
{
	[Serializable]
	public class TestRunner : MonoBehaviour
	{
		static private TestResultRenderer resultRenderer = new TestResultRenderer ();

		public TestComponent currentTest;
		private List<TestResult> resultList = new List<TestResult> ();
		private List<TestComponent> testComponents;
		
		public bool isInitializedByRunner
		{
			get
			{

#if !IMITATE_BATCH_MODE				
				if (Application.isEditor && !isBatchMode ()) 
					return true;
#endif
				return false;
			}
		}

		private double startTime;
		private bool readyToRun;

		private AssertionComponent[] assertionsToCheck = null;
		private string testMessages;
		private string stacktrace;
		private TestState testState = TestState.Running;

		private TestRunnerConfigurator configurator;

		public TestRunnerCallbackList TestRunnerCallback = new TestRunnerCallbackList();
		private IntegrationTestsProvider testsProvider;

		private const string prefix = "IntegrationTest";
		private const string startedMessage = prefix + " Started";
		private const string finishedMessage = prefix + " Finished";
		private const string timeoutMessage = prefix + " Timeout";
		private const string failedMessage = prefix + " Failed";
		private const string failedExceptionMessage = prefix + " Failed with exception";
		private const string ignoredMessage = prefix + " Ignored";
		private const string interruptedMessage = prefix + " Run interrupted";


		public void Awake ()
		{
			configurator = new TestRunnerConfigurator ();
			if (isInitializedByRunner) return;
			TestComponent.DisableAllTests ();
		}

		public void Start ()
		{
			if (isInitializedByRunner) return;

			if (configurator.sendResultsOverNetwork)
			{
				var nrs = configurator.ResolveNetworkConnection ();
				if(nrs!=null)
					TestRunnerCallback.Add (nrs);
			}

			TestComponent.DestroyAllDynamicTests ();
			var dynamicTestTypes = TestComponent.GetTypesWithHelpAttribute (Application.loadedLevelName);
			foreach (var dynamicTestType in dynamicTestTypes)
				TestComponent.CreateDynamicTest (dynamicTestType);

			var tests = TestComponent.FindAllTestsOnScene ();

			InitRunner (tests, dynamicTestTypes.Select (type => type.AssemblyQualifiedName).ToList ());
		}

		public void InitRunner(List<TestComponent> tests, List<string> dynamicTestsToRun)
		{
			currentlyRegisteredLogCallback = GetLogCallbackField ();
			logCallback = LogHandler;
			Application.RegisterLogCallback (logCallback);
			
			//Init dynamic tests
			foreach (var typeName in dynamicTestsToRun)
			{
				var t = Type.GetType (typeName);
				if (t == null) continue;
				var scriptComponents = Resources.FindObjectsOfTypeAll (t) as MonoBehaviour[];
				if (scriptComponents.Length == 0)
				{
					Debug.LogWarning (t + " not found. Skipping.");
					continue;
				}
				if (scriptComponents.Length > 1) Debug.LogWarning ("Multiple GameObjects refer to " + typeName);
				tests.Add (scriptComponents.First().GetComponent<TestComponent> ());
			}
			//create test structure
			testComponents = ParseListForGroups (tests).ToList ();
			//create results for tests
			resultList = testComponents.Select (component => new TestResult (component)).ToList ();
			//init test provider
			testsProvider = new IntegrationTestsProvider (resultList.Select (result => result.TestComponent as ITestComponent));
			readyToRun = true;
		}

		private static IEnumerable<TestComponent> ParseListForGroups ( IEnumerable<TestComponent> tests )
		{
			var results = new HashSet<TestComponent> ();
			foreach (var testResult in tests)
			{
				if (testResult.IsTestGroup ())
				{
					var childrenTestResult = testResult.gameObject.GetComponentsInChildren (typeof (TestComponent), true)
						.Where (t=>t!=testResult)
						.Cast<TestComponent> ()
						.ToArray ();
					foreach (var result in childrenTestResult)
					{
						if(!result.IsTestGroup())
							results.Add (result);
					}
					continue;
				}
				results.Add (testResult);
			}
			return results;
		}

		public void Update ()
		{
			if (readyToRun  && Time.frameCount > 1)
			{
				readyToRun = false;
				StartCoroutine ("StateMachine");
			}
			LogCallbackStillRegistered ();

		}

		public void OnDestroy()
		{
			if (currentTest != null)
			{
				var testResult = resultList.Single (result => result.TestComponent == currentTest);
				testResult.messages += "Test run interrupted (crash?)";
				LogMessage(interruptedMessage);
				FinishTest(TestResult.ResultType.Failed);
			}
			if (currentTest != null || (testsProvider != null && testsProvider.AnyTestsLeft ()))
			{
				var remainingTests = testsProvider.GetRemainingTests ();
				TestRunnerCallback.TestRunInterrupted( remainingTests.ToList () );
			}
			Application.RegisterLogCallback(null);
		}

		private void LogHandler (string condition, string stacktrace, LogType type)
		{
			if (!condition.StartsWith (startedMessage) && !condition.StartsWith (finishedMessage))
			{
				var msg = condition;
				if (msg.StartsWith(prefix)) msg = msg.Substring(prefix.Length+1);
				if (currentTest != null && msg.EndsWith ("(" + currentTest.name + ')')) msg = msg.Substring (0, msg.LastIndexOf ('('));
				testMessages += msg + "\n";
			}
			if (type == LogType.Exception)
			{
				var exceptionType = condition.Substring (0, condition.IndexOf(':'));
				if (currentTest.IsExceptionExpected (exceptionType))
				{
					testMessages += exceptionType + " was expected\n";
					if (currentTest.ShouldSucceedOnException())
					{
						testState = TestState.Success;
					}
				}
				else
				{
					testState = TestState.Exception;
					this.stacktrace = stacktrace;
				}
			}
			else if (type == LogType.Error || type == LogType.Assert)
			{
				testState = TestState.Failure;
				this.stacktrace = stacktrace;
			}
			else if (type == LogType.Log)
			{
				if (testState ==  TestState.Running && condition.StartsWith (IntegrationTest.passMessage))
				{
					testState = TestState.Success;
				}
				if (condition.StartsWith(IntegrationTest.failMessage))
				{
					testState = TestState.Failure;
				}
			}
		}

		public IEnumerator StateMachine ()
		{
			TestRunnerCallback.RunStarted (Application.platform.ToString (), testComponents);
			while (true)
			{
				if (!testsProvider.AnyTestsLeft() && currentTest == null)
				{
					FinishTestRun ();
					yield break;
				}
				if (currentTest == null)
				{
					StartNewTest ();
				}
				if (currentTest != null)
				{
					if (testState == TestState.Running)
					{
						if (assertionsToCheck != null && assertionsToCheck.All (a => a.checksPerformed > 0))
						{
							IntegrationTest.Pass (currentTest.gameObject);
							testState = TestState.Success;
						}
						if (currentTest != null && Time.time > startTime + currentTest.GetTimeout())
						{
							testState = TestState.Timeout;
						}
					}

					switch (testState)
					{
						case TestState.Success:
							LogMessage (finishedMessage);
							FinishTest (TestResult.ResultType.Success);
							break;
						case TestState.Failure:
							LogMessage (failedMessage);
							FinishTest (TestResult.ResultType.Failed);
							break;
						case TestState.Exception:
							LogMessage (failedExceptionMessage);
							FinishTest (TestResult.ResultType.FailedException);
							break;
						case TestState.Timeout:
							LogMessage(timeoutMessage);
							FinishTest(TestResult.ResultType.Timeout);
							break;
						case TestState.Ignored:
							LogMessage (ignoredMessage);
							FinishTest(TestResult.ResultType.Ignored);
							break;
					}
				}
				yield return null;
			}
		}

		private void LogMessage(string message)
		{
			if (currentTest != null)
				Debug.Log (message + " (" + currentTest.Name + ")", currentTest.gameObject);
			else
				Debug.Log (message);
		}

		private void FinishTestRun ()
		{
			PrintResultToLog ();
			TestRunnerCallback.RunFinished (resultList);
			LoadNextLevelOrQuit ();
		}

		private void PrintResultToLog ()
		{
			var resultString = "";
			resultString += "Passed: " + resultList.Count (t => t.IsSuccess);
			if (resultList.Any (result => result.IsFailure))
			{
				resultString += " Failed: " + resultList.Count (t => t.IsFailure);
				Debug.Log ("Failed tests: " + string.Join (", ", resultList.Where (t => t.IsFailure).Select (result => result.Name).ToArray ()));
			}
			if (resultList.Any (result => result.IsIgnored))
			{
				resultString += " Ignored: " + resultList.Count (t => t.IsIgnored);
				Debug.Log ("Ignored tests: " + string.Join (", ",
															resultList.Where (t => t.IsIgnored).Select (result => result.Name).ToArray ()));
			}
			Debug.Log (resultString);
		}

		private void LoadNextLevelOrQuit ()
		{
			if (isInitializedByRunner) return;
			
			if (Application.loadedLevel < Application.levelCount - 1)
				Application.LoadLevel (Application.loadedLevel + 1);
			else
			{
				resultRenderer.ShowResults();
				if (configurator.isBatchRun && configurator.sendResultsOverNetwork)
					Application.Quit ();
			}
		}

		public void OnGUI ()
		{
			resultRenderer.Draw ();
		}

		private void StartNewTest ()
		{
			this.testMessages = "";
			this.stacktrace = "";
			testState = TestState.Running;
			assertionsToCheck = null;

			startTime = Time.time;
			currentTest = testsProvider.GetNextTest () as TestComponent;

			var testResult = resultList.Single (result => result.TestComponent == currentTest);
			
			if (currentTest.ShouldSucceedOnAssertions ())
			{
				var assertionList = currentTest.gameObject.GetComponentsInChildren<AssertionComponent> ().Where (a => a.enabled);
				if(assertionList.Any())
					assertionsToCheck = assertionList.ToArray();
			}

			if (currentTest.IsExludedOnThisPlatform ())
			{
				testState = TestState.Ignored;
				Debug.Log(currentTest.gameObject.name + " is excluded on this platform");
			}

			//don't ignore test if user initiated it from the runner and it's the only test that is being run
			if (currentTest.IsIgnored () && !(isInitializedByRunner && resultList.Count == 1)) testState = TestState.Ignored;

			LogMessage(startedMessage);
			TestRunnerCallback.TestStarted (testResult);
		}

		private void FinishTest(TestResult.ResultType result)
		{
			testsProvider.FinishTest (currentTest);
			var testResult = resultList.Single (t => t.GameObject == currentTest.gameObject);
			testResult.resultType = result;
			testResult.duration = Time.time - startTime;
			testResult.messages = testMessages;
			testResult.stacktrace = stacktrace;
			TestRunnerCallback.TestFinished (testResult);
			currentTest = null;
			if (!testResult.IsSuccess 
				&& testResult.Executed
				&& !testResult.IsIgnored) resultRenderer.AddResults (Application.loadedLevelName, testResult);
		}

		#region Test Runner Helpers
		
		public static TestRunner GetTestRunner ()
		{
			TestRunner testRunnerComponent = null;
			var testRunnerComponents = Resources.FindObjectsOfTypeAll(typeof(TestRunner));

			if (testRunnerComponents.Count () > 1)
				foreach (var t in testRunnerComponents) DestroyImmediate((t as TestRunner).gameObject);
			else if (!testRunnerComponents.Any())
				testRunnerComponent = Create().GetComponent<TestRunner>();
			else
				testRunnerComponent = testRunnerComponents.Single() as TestRunner;

			return testRunnerComponent;
		}

		private static GameObject Create()
		{
			var runner = new GameObject ("TestRunner");
			var component = runner.AddComponent<TestRunner> ();
			component.hideFlags = HideFlags.NotEditable;
			Debug.Log ("Created Test Runner");
			return runner;
		}

		private static bool isBatchMode ()
		{
#if !UNITY_METRO
			var InternalEditorUtilityClassName = "UnityEditorInternal.InternalEditorUtility, UnityEditor, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";

			var t = Type.GetType(InternalEditorUtilityClassName, false);
			if (t == null) return false;

			var inBatchModeProperty = "inBatchMode";
			var prop = t.GetProperty (inBatchModeProperty);
			return (bool) prop.GetValue (null, null);
#else
			return false;
#endif
		}

		#endregion

		#region LogCallback check
		private Application.LogCallback logCallback;
		private FieldInfo currentlyRegisteredLogCallback;

		public void LogCallbackStillRegistered()
		{
			if (Application.platform == RuntimePlatform.OSXWebPlayer
				|| Application.platform == RuntimePlatform.WindowsWebPlayer)
				return;
			if (currentlyRegisteredLogCallback == null) return;
			var v = (Application.LogCallback)currentlyRegisteredLogCallback.GetValue(null);
			if (v == logCallback) return;
			Debug.LogError("Log callback got changed. This may be caused by other tools using RegisterLogCallback.");
			Application.RegisterLogCallback(logCallback);
		}

		private FieldInfo GetLogCallbackField()
		{
#if !UNITY_METRO
			var type = typeof(Application);
			var f = type.GetFields(BindingFlags.Static | BindingFlags.NonPublic).Where(p => p.Name == "s_LogCallback");
			if (f.Count() != 1) return null;
			return f.Single();
#else
			return null;
#endif
		}
		#endregion

		enum TestState
		{
			Running,
			Success,
			Failure,
			Exception,
			Timeout,
			Ignored
		}
	}
}
