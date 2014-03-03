using System;
using UnityEngine;

namespace UnityTest
{
	[Serializable]
	public class TestResult : ITestResult
	{
		public GameObject go;
		public string name;
		public ResultType resultType;
		public double duration;
		public string messages;
		public string stacktrace;
		public bool isRunning;
		public string id { get; private set; }

		public TestComponent TestComponent
		{
			get { return go.GetComponent<TestComponent> (); }
		}

		public TestResult ( GameObject gameObject )
		{
			id =  Guid.NewGuid().ToString("N");
			resultType = ResultType.NotRun;
			this.go = gameObject;
			if(gameObject!=null)
				name = gameObject.name;
		}

		public enum ResultType
		{
			Success,
			Failed,
			Timeout,
			NotRun,
			FailedException,
			Ignored
		}

		public void Reset ()
		{
			resultType = ResultType.NotRun;
			duration = 0f;
			messages = "";
			stacktrace = "";
			isRunning = false;
		}

		#region ITestResult implementation
		public TestResultState ResultState { get
		{
			switch (resultType)
			{
				case ResultType.Success: return TestResultState.Success;
				case ResultType.Failed: return TestResultState.Failure;
				case ResultType.FailedException: return TestResultState.Error;
				case ResultType.Ignored: return TestResultState.Ignored;
				case ResultType.NotRun: return TestResultState.Skipped;
				case ResultType.Timeout: return TestResultState.Cancelled;
				default: throw new Exception();
			}
		}}
		public string Message { get { return messages; } }
		public bool Executed { get { return resultType != ResultType.NotRun; } }
		public string Name { get { return name; } }
		public string FullName { get { return Name; } }
		public bool IsSuccess { get { return resultType == ResultType.Success; } }
		public double Duration { get { return duration; } }
		public string StackTrace { get { return stacktrace; } }

		public bool IsIgnored { get { return resultType == ResultType.Ignored; } }
		public bool IsFailure 
		{ 
			get 
			{ 
				return resultType == ResultType.Failed 
					|| resultType == ResultType.FailedException 
					|| resultType == ResultType.Timeout; 
			}
		}

		

		#endregion
	}
}
