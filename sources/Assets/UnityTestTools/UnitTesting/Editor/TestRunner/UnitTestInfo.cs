using System;
using Object = System.Object;

namespace UnityTest
{
	[Serializable]
	public class UnitTestInfo
	{
		public UnitTestInfo (string methodPath)
		{
			if(string.IsNullOrEmpty(methodPath))
				throw new ArgumentException();

			FullName = methodPath;

			var idx = methodPath.IndexOf ('(');
			if (idx > 0)
			{
				ParamName = methodPath.Substring (idx + 1,
												methodPath.Length - idx - 2);
				methodPath = methodPath.Substring (0,
													methodPath.IndexOf ('('));
			}
			else
			{
				ParamName = "";
			}

			MethodName = methodPath.Substring (methodPath.LastIndexOf ('.') + 1);

			methodPath = methodPath.Substring (0,
												methodPath.LastIndexOf ('.'));

			if (methodPath.LastIndexOf ('.') > -1)
			{
				FullClassName = methodPath;
				ClassName = methodPath.Substring (methodPath.LastIndexOf ('.') + 1);

				methodPath = methodPath.Substring (0,
													methodPath.LastIndexOf ('.'));
				Namespace = methodPath;
			}
			else
			{
				ClassName = methodPath;
				FullClassName = methodPath;
				Namespace = "";
			}
		}

		public string ParamName { get; private set; }
		public string MethodName { get; private set; }
		public string ClassName { get; private set; }
		public string FullClassName { get; private set; }
		public string Namespace { get; private set; }
		public string FullName { get; private set; }

		public override bool Equals (Object obj)
		{
			if (!(obj is UnitTestInfo)) return false;

			var testInfo = (UnitTestInfo) obj;
			return FullName == testInfo.FullName;
		}

		public override int GetHashCode ()
		{
			return FullName.GetHashCode ();
		}

		public static bool operator == (UnitTestInfo a, UnitTestInfo b)
		{
			// If both are null, or both are same instance, return true.
			if (ReferenceEquals (a,
								b))
			{
				return true;
			}

			// If one is null, but not both, return false.
			if (((object) a == null) || ((object) b == null))
			{
				return false;
			}

			// Return true if the fields match:
			return a.Equals (b);
		}

		public static bool operator != (UnitTestInfo a, UnitTestInfo b)
		{
			return !(a == b);
		}
	}
}
