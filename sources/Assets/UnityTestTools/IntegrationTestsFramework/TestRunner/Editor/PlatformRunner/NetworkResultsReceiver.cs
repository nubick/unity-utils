using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
	[Serializable]
	public class NetworkResultsReceiver : EditorWindow
	{
		public static NetworkResultsReceiver Instance = null;

		private string statusLabel;
		private TcpListener listener;

		[SerializeField]
		private PlatformRunnerConfiguration configuration;

		private List<ITestResult> testResults = new List<ITestResult> ();

		#region steering variables
		private bool runFinished;
		private bool repaint;

		private TimeSpan testTimeout = TimeSpan.Zero;
		private DateTime lastMessageReceived;
		private bool running;

		public TimeSpan ReceiveMessageTimeout = TimeSpan.FromSeconds(30);
		private TimeSpan InitialConnectionTimeout = TimeSpan.FromSeconds(300);
		private bool testFailed;
		#endregion

		private void AcceptCallback ( TcpClient client )
		{
			repaint = true;
			ResultDTO dto = null;
			try
			{
				lastMessageReceived = DateTime.Now;
				using (var stream = client.GetStream ())
				{
					var bf = new BinaryFormatter ();
					dto = (ResultDTO) bf.Deserialize (stream);
					stream.Close ();
				}
				client.Close ();
			}
			catch (ObjectDisposedException e)
			{
				Debug.LogException (e);
				statusLabel = "Got disconnected";
				return;
			}
			catch (Exception e)
			{
				Debug.LogException (e);
				return;
			}

			switch (dto.messageType)
			{
				case ResultDTO.MessageType.TestStarted:
					statusLabel = dto.testName;
					testTimeout = TimeSpan.FromSeconds(dto.testTimeout);
					break;
				case ResultDTO.MessageType.TestFinished:
					testResults.Add (dto.testResult);
					testTimeout = TimeSpan.Zero;
					if(dto.testResult.Executed && !dto.testResult.IsSuccess)
						testFailed = true;
					break;
				case ResultDTO.MessageType.RunStarted:
					testResults = new List<ITestResult> ();
					statusLabel = "Run started: " + dto.loadedLevelName;
					break;
				case ResultDTO.MessageType.RunFinished:
					WriteResultsToLog(dto, testResults);
					if (!string.IsNullOrEmpty (configuration.resultsDir))
					{
						var resultWriter = new XmlResultWriter (dto.loadedLevelName, testResults.ToArray ());
						try
						{
							var filePath = Path.Combine (configuration.resultsDir, dto.loadedLevelName + ".xml");
							File.WriteAllText (filePath, resultWriter.GetTestResult ());
						}
						catch (Exception e)
						{
							Debug.LogException (e);
						}
					}
					if (dto.levelCount - dto.loadedLevel == 1)
					{
						running = false;
						runFinished = true;
					}
					break;
				case ResultDTO.MessageType.Ping:
					break;
			}
		}

		private void WriteResultsToLog (ResultDTO dto, List<ITestResult> list)
		{
			string result = "Run finished for: " + dto.loadedLevelName;
			var failCount = list.Count (t => t.Executed && !t.IsSuccess);
			if (failCount == 0)
				result += "\nAll tests passed";
			else
				result += "\n" + failCount + " tests failed";

			if(failCount == 0)
				Debug.Log (result);
			else
				Debug.LogWarning (result);
		}

		public void Update ()
		{
			if (EditorApplication.isCompiling
			    && listener != null)
			{
				running = false;
				listener.Stop ();
				return;
			}

			if (running)
			{
				try
				{
					if (listener.Pending ())
					{
						using (var client = listener.AcceptTcpClient ())
						{
							AcceptCallback (client);
							client.Close ();
						}
					}
				}
				catch (InvalidOperationException e)
				{
					statusLabel = "Exception happened: " + e.Message;
					Repaint();
					Debug.LogException(e);
				}
			}

			if (running)
			{
				var adjustedtestTimeout =  testTimeout.Add (testTimeout);
				var timeout = ReceiveMessageTimeout > adjustedtestTimeout ? ReceiveMessageTimeout : adjustedtestTimeout;
				if((DateTime.Now - lastMessageReceived) > timeout )
				{
					Debug.LogError ("Timeout when waiting for test results");
					runFinished = true;
				}
			}
			if (runFinished)
			{
				if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
					EditorApplication.Exit(testFailed ? Batch.RETURN_CODE_TESTS_FAILED : Batch.RETURN_CODE_TESTS_OK);
				Close ();
			}
			if (repaint) Repaint ();
		}

		public void OnEnable ()
		{
			minSize = new Vector2(300, 100);
			position = new Rect(position.xMin, position.yMin, 300, 100);
			title = "Test run monitor";
			Instance = this;
			statusLabel = "Initializing...";
			if (EditorApplication.isCompiling) return;
			EnableServer();
		}

		private void EnableServer ()
		{
			var ipAddress = IPAddress.Any;
			if (configuration != null && configuration.ipList != null && configuration.ipList.Count == 1)
				ipAddress = IPAddress.Parse (configuration.ipList.Single ());

			var ipAddStr = ipAddress == IPAddress.Any ? "[All interfaces]" : ipAddress.ToString ();
			listener = new TcpListener (ipAddress, configuration.port);
			statusLabel = "Waiting for connection on: " + ipAddStr + ":" + configuration.port;

			try
			{
				listener.Start (100);
			}
			catch (SocketException e)
			{
				statusLabel = "Exception happened: " + e.Message;
				Repaint ();
				Debug.LogException (e);
			}
			running = true;
			lastMessageReceived = DateTime.Now + InitialConnectionTimeout;
		}

		public void OnDisable()
		{
			Instance = null;
			if (listener != null)
				listener.Stop ();
		}

		private void OnGUI ()
		{
			EditorGUILayout.LabelField("Status:", EditorStyles.boldLabel);
			EditorGUILayout.LabelField(statusLabel);
			GUILayout.FlexibleSpace ();
			if (GUILayout.Button ("Stop"))
			{
				StopReceiver ();
				if(UnityEditorInternal.InternalEditorUtility.inBatchMode)
					EditorApplication.Exit (Batch.RETURN_CODE_RUN_ERROR);
			}
		}

		public static void StartReceiver (PlatformRunnerConfiguration configuration)
		{
			var w = (NetworkResultsReceiver) GetWindow (typeof (NetworkResultsReceiver), false);
			w.SetConfiguration (configuration);
			w.Show(true);

		}

		private void SetConfiguration (PlatformRunnerConfiguration configuration)
		{
			this.configuration = configuration;
		}

		public static void StopReceiver ()
		{
			if (Instance == null) return;
			Instance.Close ();
		}
	}
}
