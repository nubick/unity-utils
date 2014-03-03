using System;

namespace UnityTest
{
	public class AssertionException : Exception
	{
		public AssertionException (AssertionComponent assertion) : base(assertion.Action.GetFailureMessage ())
		{
		}
	}
}
