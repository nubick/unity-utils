using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityTest.IntegrationTestRunner;

#if !UNITY_METRO
using System.Net;
#endif

#if !UNITY_WEBPLAYER
using System.Net.NetworkInformation;
#endif

namespace UnityTest
{
	public class TestRunnerConfigurator
	{
		public static string integrationTestsNetwork = "networkconfig.txt";
		public static string batchRunFileMarker = "batchrun.txt";

		public bool isBatchRun { get; private set; }

		public bool sendResultsOverNetwork { get; private set; }

#if !UNITY_METRO
		private List<IPEndPoint> ipEndPointList = new List<IPEndPoint> ();
#endif

		public TestRunnerConfigurator ()
		{
			CheckForBatchMode ();
			CheckForSendingResultsOverNetwork ();
		}

		private void CheckForSendingResultsOverNetwork ()
		{
#if !UNITY_METRO
			string text;
			if (Application.isEditor)
				text = GetTextFromTempFile (integrationTestsNetwork);
			else
				text = GetTextFromTextAsset (integrationTestsNetwork);

			if (text == null) return;

			sendResultsOverNetwork = true;

			ipEndPointList.Clear ();

			foreach (var line in text.Split (new []{'\n'}, StringSplitOptions.RemoveEmptyEntries))
			{
				var idx = line.IndexOf (':');
				if (idx == -1) throw new Exception (line);
				var ip = line.Substring (0, idx);
				var port = line.Substring (idx + 1);
				ipEndPointList.Add (new IPEndPoint (IPAddress.Parse (ip), Int32.Parse (port)));
			}
#endif
		}

		private static string GetTextFromTextAsset ( string fileName )
		{
			var nameWithoutExtension = fileName.Substring (0, fileName.LastIndexOf ('.'));
			var resultpathFile = Resources.Load (nameWithoutExtension) as TextAsset;
			return resultpathFile!=null?resultpathFile.text:null;
		}

		private static string GetTextFromTempFile(string fileName)
		{
			string text = null;
			try
			{
#if UNITY_EDITOR
				text = System.IO.File.ReadAllText(System.IO.Path.Combine("Temp", fileName));
#endif
			}
			catch (Exception)
			{
			}
			return text;
		}

		private void CheckForBatchMode ()
		{
#if IMITATE_BATCH_MODE
			isBatchRun = true;
#elif UNITY_EDITOR
			if (Application.isEditor && UnityEditorInternal.InternalEditorUtility.inBatchMode) 
				isBatchRun = true;
#else
			if (GetTextFromTextAsset (batchRunFileMarker) != null) isBatchRun = true;
#endif
		}

		public static List<string> GetAvailableNetworkIPs ()
		{
#if !UNITY_METRO && !UNITY_WEBPLAYER
			if (!NetworkInterface.GetIsNetworkAvailable ()) return null;

			var ipList = new List<UnicastIPAddressInformation> ();

			foreach (var netInterface in NetworkInterface.GetAllNetworkInterfaces ())
			{
				if(netInterface.NetworkInterfaceType != NetworkInterfaceType.Wireless80211 && 
					netInterface.NetworkInterfaceType != NetworkInterfaceType.Ethernet)
					continue;
				if(netInterface.OperationalStatus != OperationalStatus.Up) continue;

				var ipAdresses = netInterface.GetIPProperties ().UnicastAddresses
					.Where (a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork );
				ipList.AddRange (ipAdresses);
			}
			//sort ip list by their masks to predict which ip belongs to lan network
			ipList.Sort (( ip1, ip2 ) =>
			{
				var mask1 = BitConverter.ToInt32 (ip1.IPv4Mask.GetAddressBytes ().Reverse ().ToArray (),0);
				var mask2 = BitConverter.ToInt32 (ip2.IPv4Mask.GetAddressBytes ().Reverse ().ToArray (), 0);
				return mask2.CompareTo (mask1);
			});
			return ipList.Select (i => i.Address.ToString ()).ToList ();
#else
			return new List<string>();
#endif
		}

		public ITestRunnerCallback ResolveNetworkConnection ()
		{
#if !UNITY_METRO
			var nrsList = ipEndPointList.Select (ipEndPoint => new NetworkResultSender (ipEndPoint.Address.ToString (), ipEndPoint.Port)).ToList ();

			var timeout = TimeSpan.FromSeconds (30);
			DateTime startTime = DateTime.Now;
			while ((DateTime.Now - startTime) < timeout)
			{
				foreach (var networkResultSender in nrsList)
				{
					try
					{
						if(!networkResultSender.Ping ()) continue;
					}	
					catch (Exception e)
					{
						Debug.LogException (e);
						sendResultsOverNetwork = false;
						return null;
					}
					return networkResultSender;
				}
				Thread.Sleep (500);
			}
			Debug.LogError ("Couldn't connect to the server: " + string.Join (", ", ipEndPointList.Select (ipep=>ipep.Address + ":" + ipep.Port).ToArray ()));
			sendResultsOverNetwork = false;
#endif
			return null;
		}
	}
}
