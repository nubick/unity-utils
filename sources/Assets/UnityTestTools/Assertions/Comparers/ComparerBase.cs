using System;
using UnityEngine;
using Object = System.Object;

namespace UnityTest
{
	public abstract class ComparerBase : ActionBase
	{
		public enum CompareToType
		{
			CompareToObject,
			CompareToConstantValue,
			CompareToNull
		}

		public CompareToType compareToType = CompareToType.CompareToObject;

		public GameObject other;
		public string otherPropertyPath = "";
		
		protected abstract bool Compare (object a, object b);

		protected override bool Compare(object objVal)
		{
			object objOtherVal;
			if (compareToType == CompareToType.CompareToConstantValue)
			{
				objOtherVal = ConstValue;
			}
			else if (compareToType == CompareToType.CompareToNull)
			{
				objOtherVal = null; 
			}
			else
			{
				if (other == null)
					objOtherVal = null;
				else
					objOtherVal = GetPropertyValue (other,
													otherPropertyPath,
													GetAccepatbleTypesForB ());
			}

			return Compare (objVal,
							objOtherVal);
		}

		public virtual Type[] GetAccepatbleTypesForB()
		{
			return null;
		}

		#region Const value

		public virtual object ConstValue { get; set; }
		public virtual object GetDefaultConstValue()
		{
			throw new NotImplementedException();
		}

		#endregion

		public virtual Type GetSecondParameterType() { return typeof(object); }

		public object GetOtherPropertyValue ()
		{
			switch (compareToType)
			{
				case CompareToType.CompareToObject:
					return PropertyResolver.GetPropertyValueFromString(other,
														otherPropertyPath);
				case CompareToType.CompareToConstantValue:
					return ConstValue;
				case CompareToType.CompareToNull:
				default:
					return null;
			}
		}

		public override string GetFailureMessage ()
		{
			var message = name + " assertion failed.\n(" + go + ")." + thisPropertyPath + " " + compareToType;

			switch (compareToType)
			{
				case ComparerBase.CompareToType.CompareToObject:
					message += " (" + other + ")." + otherPropertyPath + " failed.";
					break;
				case ComparerBase.CompareToType.CompareToConstantValue:
					message += ConstValue + " failed.";
					break;
				case ComparerBase.CompareToType.CompareToNull:
					message += " failed.";
					break;
			}
			return message;
		}
	}

	[Serializable]
	public abstract class ComparerBaseGeneric<T> : ComparerBaseGeneric<T,T>
	{
	}

	[Serializable]
	public abstract class ComparerBaseGeneric<T1, T2> : ComparerBase
	{
		public T2 constantValueGeneric = default(T2);

		public override Object  ConstValue
		{
			get 
			{
				return constantValueGeneric;
			}
			set 
			{
				constantValueGeneric = (T2) value;
			}
		}

		public override Object GetDefaultConstValue()
		{
			return default(T2);
		}

		protected override bool Compare(object a, object b)
		{
			var type = typeof(T2);
			if (b == null && type.IsValueType)
			{
				throw new ArgumentException("Null was passed to a value-type argument");
			}
			return Compare((T1)a, (T2)b);
		}

		protected abstract bool Compare(T1 a, T2 b);

		public override Type[] GetAccepatbleTypesForA()
		{
			return new[] { typeof(T1) };
		}

		public override Type[] GetAccepatbleTypesForB ()
		{
			return new[] {typeof (T2)};
		}

		public override Type GetParameterType()
		{
			return typeof(T1);
		}

		public override Type GetSecondParameterType()
		{
			return typeof(T2);
		}
	}
}
