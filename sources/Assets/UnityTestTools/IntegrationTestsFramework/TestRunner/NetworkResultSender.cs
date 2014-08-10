using System;
using System.Collections.Generic;

#if !UNITY_METRO
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
#endif

using UnityTest.IntegrationTestRunner;

namespace UnityTest
{
	public class NetworkResultSender : ITestRunnerCallback
	{
		private readonly TimeSpan ConnectionTimeout = TimeSpan.FromSeconds(5);

		private string ip;
		private int port;
		private bool lostConnection;

		public NetworkResultSender ( string ip, int port )
		{
			this.ip = ip;
			this.port = port;
		}

		private bool SendDTO(ResultDTO dto)
		{
			if (lostConnection) return false;
#if !UNITY_METRO
			try
			{
				using (var tcpClient = new TcpClient())
				{
					var result = tcpClient.BeginConnect (ip, port, null, null);
					var success = result.AsyncWaitHandle.WaitOne(ConnectionTimeout);
					if (!success)
					{
						return false;
					}
					try
					{
						tcpClient.EndConnect (result);
					}
					catch (SocketException)
					{
						lostConnection = true;
						return false;
					}

					var bf = new BinaryFormatter();
					bf.Serialize(tcpClient.GetStream(), dto);
					tcpClient.GetStream().Close ();
					tcpClient.Close();
					UnityEngine.Debug.Log ("Sent " + dto.messageType);
				}
			}
			catch (SocketException e)
			{
				UnityEngine.Debug.LogException (e);
				lostConnection = true;
				return false;
			}
#endif
			return true;
		}

		public bool Ping ()
		{
			var result = SendDTO (ResultDTO.CreatePing ());
			lostConnection = false;
			return result;
		}

		public void RunStarted ( string platform, List<TestComponent> testsToRun )
		{
			SendDTO (ResultDTO.CreateRunStarted ());
		}

		public void RunFinished ( List<TestResult> testResults )
		{
			SendDTO(ResultDTO.CreateRunFinished(testResults));
		}

		public void TestStarted ( TestResult test )
		{
			SendDTO(ResultDTO.CreateTestStarted(test));
		}

		public void TestFinished ( TestResult test )
		{
			SendDTO(ResultDTO.CreateTestFinished(test));
		}

		public void TestRunInterrupted ( List<ITestComponent> testsNotRun )
		{
			RunFinished (new List<TestResult> ());
		}
	}
}
