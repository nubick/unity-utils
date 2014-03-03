using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityTest
{
	public class TestComponent : MonoBehaviour
	{
		public float timeout = 5;
		public bool ignored = false;
		public bool succeedAfterAllAssertionsAreExecuted = false;
		public bool expectException = false;
		public string expectedExceptionList = "";
		public bool succeedWhenExceptionIsThrown = false;
		public IncludedPlatforms includedPlatforms = (IncludedPlatforms)~0L;
		
		public bool IsExludedOnThisPlatform ()
		{
			try
			{
				var ipv = (IncludedPlatforms) Enum.Parse (typeof (IncludedPlatforms),
														Application.platform.ToString ());
				return (includedPlatforms & ipv) == 0;
			}
			catch
			{
				Debug.LogWarning ("Current platform is not supported");
				return true;
			}
		}

		public bool IsExceptionExpected (string exception)
		{
			if (!expectException) return false;
			exception = exception.Trim ();
			foreach (var expectedException in expectedExceptionList.Split (',').Select (e=>e.Trim()))
			{
				if (exception == expectedException) return true;
				var exceptionType = Type.GetType (exception) ?? GetTypeByName(exception);
				var expectedExceptionType = Type.GetType (expectedException) ?? GetTypeByName (expectedException);
				if (exceptionType != null && expectedExceptionType != null && expectedExceptionType.IsAssignableFrom (exceptionType))
					return true;
			}
			return false;
		}

		private Type GetTypeByName(string className)
		{
			return AppDomain.CurrentDomain.GetAssemblies ().SelectMany (a => a.GetTypes ()).FirstOrDefault (type => type.Name == className);
		}

		public void OnValidate ()
		{
			if (timeout < 0.01f) timeout = 0.01f;
		}

		[Flags]
		public enum IncludedPlatforms
		{
			WindowsEditor		= 1 << 0,
			OSXEditor			= 1 << 1,
			WindowsPlayer		= 1 << 2,
			OSXPlayer			= 1 << 3,
			LinuxPlayer			= 1 << 4,
			//MetroPlayerX86	= 1 << 5,
			//MetroPlayerX64	= 1 << 6,	
			//MetroPlayerARM	= 1 << 7,
			WindowsWebPlayer	= 1 << 8,
			//OSXWebPlayer		= 1 << 9,
			Android				= 1 << 10,
			//IPhonePlayer		= 1 << 11,
			//TizenPlayer		= 1 << 12,
			//WP8Player			= 1 << 13,
			//BB10Player		= 1 << 14,	
			//NaCl				= 1 << 15,
		}
	}
}
