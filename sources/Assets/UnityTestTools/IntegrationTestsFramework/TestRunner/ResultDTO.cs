using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
	[Serializable]
	public class ResultDTO
	{
		public MessageType messageType;
		public int levelCount;
		public int loadedLevel;
		public string loadedLevelName;
		public string testName;
		public float testTimeout;
		public ITestResult testResult;

		private ResultDTO (MessageType messageType)
		{
			this.messageType = messageType;
			this.levelCount = Application.levelCount;
			this.loadedLevel = Application.loadedLevel;
			this.loadedLevelName = Application.loadedLevelName;
		}

		public enum MessageType
		{
			Ping,
			RunStarted,
			RunFinished,
			TestStarted,
			TestFinished,
			RunInterrupted
		}

		public static ResultDTO CreatePing ()
		{
			var dto = new ResultDTO ( MessageType.Ping);
			return dto;
		}

		public static ResultDTO CreateRunStarted ()
		{
			var dto = new ResultDTO (MessageType.RunStarted);
			return dto;
		}

		public static ResultDTO CreateRunFinished (List<TestResult> testResults)
		{
			var dto = new ResultDTO (MessageType.RunFinished);
			return dto;
		}

		public static ResultDTO CreateTestStarted (TestResult test)
		{
			var dto = new ResultDTO (MessageType.TestStarted);
			dto.testName = test.FullName;
			dto.testTimeout = test.TestComponent.timeout;
			return dto;
		}

		public static ResultDTO CreateTestFinished (TestResult test)
		{
			var dto = new ResultDTO (MessageType.TestFinished);
			dto.testName = test.FullName;
			dto.testResult = GetSerializableTestResult(test);
			return dto;
		}

		private static ITestResult GetSerializableTestResult (TestResult test)
		{
			var str = new SerializableTestResult ();

			str.m_resultState = test.ResultState;
			str.m_message = test.messages;
			str.m_executed = test.Executed;
			str.m_name = test.Name;
			str.m_fullName = test.FullName;
			str.m_id = test.id;
			str.m_isSuccess = test.IsSuccess;
			str.m_duration = test.duration;
			str.m_stackTrace = test.stacktrace;

			return str;
		}
	}

	#region SerializableTestResult
	[Serializable]
	internal class SerializableTestResult : ITestResult
	{
		public TestResultState m_resultState;
		public string m_message;
		public bool m_executed;
		public string m_name;
		public string m_fullName;
		public string m_id;
		public bool m_isSuccess;
		public double m_duration;
		public string m_stackTrace;

		public TestResultState ResultState
		{
			get { return m_resultState; }
		}

		public string Message
		{
			get { return m_message; }
		}

		public bool Executed
		{
			get { return m_executed; }
		}

		public string Name
		{
			get { return m_name; }
		}

		public string FullName
		{
			get { return m_fullName; }
		}

		public string Id
		{
			get { return m_id; }
		}

		public bool IsSuccess
		{
			get { return m_isSuccess; }
		}

		public double Duration
		{
			get { return m_duration; }
		}

		public string StackTrace
		{
			get { return m_stackTrace; }
		}
	}
	#endregion
}
