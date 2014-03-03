using System;
using System.Linq;

namespace UnityTest
{
	public static class AssertionComponentExt
	{
		public static Type GetFirstArgumentType (this AssertionComponent assertion)
		{
			return assertion.Action.GetParameterType();
		}
		public static Type GetSecondArgumentType(this AssertionComponent assertion)
		{
			Type secondArgument = null;
			if (assertion.Action is ComparerBase)
			{
				secondArgument = (assertion.Action as ComparerBase).GetSecondParameterType();
			}
			return secondArgument;
		}
	}
}
