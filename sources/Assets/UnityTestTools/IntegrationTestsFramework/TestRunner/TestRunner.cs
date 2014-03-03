using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTest.IntegrationTestRunner;

namespace UnityTest
{
	[Serializable]
	public class TestRunner : MonoBehaviour
	{
		private bool imitateBatchMode = false; //set true if you want to imitate batch mode behaviour in non-batch mode mode run

		private TestResult currentTest;

		public List<TestResult> testToRun = new List<TestResult>();
		private Queue<TestResult> testToRunQueue = new Queue<TestResult> ();

		public bool isInitializedByRunner
		{
			get
			{
#if UNITY_EDITOR
				if (!UnityEditorInternal.InternalEditorUtility.inBatchMode && !imitateBatchMode) return true;
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

		public TestRunnerCallbackList TestRunnerCallback = new TestRunnerCallbackList();

		private const string startedMessage = "IntegrationTest started";
		private const string finishedMessage = "IntegrationTest finished";
		private const string timeoutMessage = "IntegrationTest timeout";
		private const string failedMessage = "IntegrationTest failed";
		private const string failedExceptionMessage = "IntegrationTest failed with exception";
		private const string ignoredMessage = "IntegrationTest ignored";
		private const string interruptedMessage = "IntegrationTest Run interrupted";

		public void Awake ()
		{
#if UNITY_EDITOR
			if (!UnityEditorInternal.InternalEditorUtility.inBatchMode && !imitateBatchMode) return;
#endif
			DisableAllTests ();
		}

		private void DisableAllTests()
		{
			foreach (var t in FindAllTests ())
			{
				t.go.hideFlags = 0;
				t.go.SetActive(false);
			}
		}

		public void Start()
		{
#if UNITY_EDITOR
			if (!UnityEditorInternal.InternalEditorUtility.inBatchMode && !imitateBatchMode) return;
#endif
			var tests = FindAllTests();
			InitRunner(tests);
		}

		private List<TestResult> FindAllTests ()
		{
			var resultArray = Resources.FindObjectsOfTypeAll(typeof(TestComponent)) as TestComponent[];
			var foundTestList = new List<TestResult>(resultArray.Select(component => new TestResult(component.gameObject)));
			return foundTestList;
		}

		public void InitRunner(List<TestResult> tests)
		{
			Application.RegisterLogCallback(LogHandler);
			if (isInitializedByRunner)
				tests.Sort((o1,o2) =>o1.go.name.CompareTo (o2.go.name));
			testToRun = tests;
			testToRunQueue = new Queue<TestResult>(testToRun);
			readyToRun = true;
		}

		public void Update ()
		{
			if (readyToRun  && Time.frameCount > 1)
			{
				readyToRun = false;
				StartCoroutine ("StateMachine");
			}
		}

		public void OnDestroy()
		{
			var test = currentTest;
			if (currentTest != null)
			{
				test.messages += "Test run interrupted (crash?)";
				LogMessage(interruptedMessage);
				FinishTest(TestResult.ResultType.Failed);
			}
			if (test != null || testToRunQueue.Any())
				TestRunnerCallback.TestRunInterrupted(testToRunQueue.ToList ());
			Application.RegisterLogCallback(null);
		}

		private void LogHandler (string condition, string stacktrace, LogType type)
		{
			testMessages += condition + "\n";
			if (type == LogType.Exception)
			{
				var exceptionType = condition.Substring (0, condition.IndexOf(':'));
				if (currentTest.TestComponent.IsExceptionExpected (exceptionType))
				{
					testMessages += exceptionType + " was expected\n";
					if (currentTest.TestComponent.succeedWhenExceptionIsThrown)
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
			else if (type == LogType.Log)
			{
				if (condition.StartsWith (IntegrationTest.passMessage))
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
			TestRunnerCallback.RunStarted(Application.platform.ToString(), testToRun);
			while (true)
			{
				if (testToRunQueue.Count == 0 && currentTest == null)
				{
					FinishTestRun ();
					break;
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
							IntegrationTest.Pass (currentTest.go);
							testState = TestState.Success;
						}
						if (currentTest != null && Time.time > startTime + currentTest.TestComponent.timeout)
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
				Debug.Log (message + " (" + currentTest.name + ")",
							currentTest.go);
			else
				Debug.Log (message);
		}

		private void FinishTestRun ()
		{
			PrintResultToLog ();
#if UNITY_EDITOR || UNITY_STANDALONE
			WriteResultsToFile();
#endif
			TestRunnerCallback.RunFinished (testToRun);
			LoadNextLevelOrQuit ();
		}

		private void PrintResultToLog ()
		{
			var resultString = "";
			resultString += "Passed: " + testToRun.Count (t => t.IsSuccess);
			if (testToRun.Any (result => result.IsFailure))
			{
				resultString += " Failed: " + testToRun.Count (t => t.IsFailure);
				Debug.Log ("Failed tests: " + string.Join (", ", testToRun.Where (t => t.IsFailure).Select (result => result.name).ToArray ()));
			}
			if (testToRun.Any (result => result.IsIgnored))
			{
				resultString += " Ignored: " + testToRun.Count (t => t.IsIgnored);
				Debug.Log ("Ignored tests: " + string.Join (", ",
															testToRun.Where (t => t.IsIgnored).Select (result => result.name).ToArray ()));
			}
			Debug.Log (resultString);
		}

		private void LoadNextLevelOrQuit ()
		{
#if UNITY_EDITOR
			if (UnityEditorInternal.InternalEditorUtility.inBatchMode || imitateBatchMode)
			{
				if (Application.loadedLevel < Application.levelCount - 1)
					Application.LoadLevel (Application.loadedLevel + 1);
				else
					UnityEditor.EditorApplication.Exit (0);
			}
#else
			if(Application.loadedLevel < Application.levelCount - 1)
				Application.LoadLevel (Application.loadedLevel + 1);
			else
				Application.Quit ();
#endif
		}

		private void WriteResultsToFile()
		{
			var path = System.IO.Path.Combine (Application.dataPath,
												Application.loadedLevelName + "-TestResults.xml");
			Debug.Log ("Saving results in " + path);
			var resultWriter = new XmlResultWriter(path);
			resultWriter.SaveTestResult (Application.loadedLevelName, testToRun.ToArray ());
		}

		private void StartNewTest ()
		{
			this.testMessages = "";
			this.stacktrace = "";
			testState = TestState.Running;
			assertionsToCheck = null;

			startTime = Time.time;
			currentTest = testToRunQueue.Dequeue ();
			currentTest.isRunning = true;
			currentTest.go.SetActive (true);
			
			if (currentTest.TestComponent.succeedAfterAllAssertionsAreExecuted)
			{
				var assertionList = currentTest.go.GetComponentsInChildren<AssertionComponent> ().Where (a => a.enabled);
				if(assertionList.Any())
					assertionsToCheck = assertionList.ToArray();
			}

			if (currentTest.TestComponent.IsExludedOnThisPlatform ())
			{
				testState = TestState.Ignored;
				Debug.Log(currentTest.name + " is excluded on this platform");
			}

			//do not run ignored tests only when it's batch mode
			//test runner in the editor will not pass ignored tests to run, unless is expected to
			if (!isInitializedByRunner && currentTest.TestComponent.ignored)
				testState = TestState.Ignored;
			LogMessage(startedMessage);
			TestRunnerCallback.TestStarted(currentTest);
		}

		private void FinishTest(TestResult.ResultType result)
		{
			testToRun.Find(results => results == currentTest).resultType = result;
			if (currentTest.go!=null)
				currentTest.go.gameObject.SetActive(false);
			currentTest.isRunning = false;
			currentTest.duration = Time.time - startTime;
			currentTest.messages = testMessages;
			currentTest.stacktrace = stacktrace;
			TestRunnerCallback.TestFinished (currentTest);
			currentTest = null;
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

		public bool HasResultFor(GameObject testInfo)
		{
			return testToRun.Any(result => result.go == testInfo);
		}

		public TestResult GetResultFor(GameObject testInfo)
		{
			return testToRun.Find(result => result.go == testInfo);
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
