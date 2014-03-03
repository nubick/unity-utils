using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityTest
{
	public abstract class ActionBase : ScriptableObject
	{
		public GameObject go;

		public string thisPropertyPath = "";
		public virtual Type[] GetAccepatbleTypesForA()
		{
			return null;
		}
		public virtual int GetDepthOfSearch() { return 2; }
		
		public virtual string[] GetExcludedFieldNames()
		{
			return new string[] { };
		}

		public bool Compare ()
		{
			var objVal = GetPropertyValue(go,
											thisPropertyPath, GetAccepatbleTypesForA());

			return Compare(objVal);
		}

		protected abstract bool Compare (object objVal);

		protected object GetPropertyValue(GameObject gameObject, string path, Type[] acceptableTypes)
		{
			var objVal = PropertyResolver.GetPropertyValueFromString(gameObject,
												path);

			if (acceptableTypes != null && !acceptableTypes.Contains(objVal.GetType(), new IsTypeComparer()))
				Debug.LogWarning(gameObject.GetType() + "." + thisPropertyPath + " is not acceptable type for the comparer");

			return objVal;
		}

		public object GetPropertyValue()
		{
			return PropertyResolver.GetPropertyValueFromString(go,
												thisPropertyPath);
		}

		private class IsTypeComparer : IEqualityComparer<Type>
		{
			public bool Equals(Type x, Type y)
			{
				return x.IsAssignableFrom(y);
			}

			public int GetHashCode(Type obj)
			{
				return obj.GetHashCode();
			}
		}

		public virtual Type GetParameterType() { return typeof(object); }

		public virtual string GetConfigurationDescription ()
		{
			string result = "";
			foreach (var prop in GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
				.Where (info => info.FieldType.IsSerializable))
			{
				var value = prop.GetValue (this);
				if (value is double)
					value = ((double)value).ToString("0.########");
				if (value is float)
					value = ((float)value).ToString("0.########");
				result += value + " ";
			}
			return result;
		}

		public ActionBase CreateCopy (GameObject oldGameObject, GameObject newGameObject)
		{
			var newObj = CreateInstance (GetType ()) as ActionBase;
			var fields = GetType ().GetFields (BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in fields)
			{
				var value = field.GetValue (this);
				if (value is GameObject)
				{
					if (value as GameObject == oldGameObject)
						value = newGameObject;
				}
				field.SetValue (newObj, value);
			}
			return newObj;
		}

		public virtual string GetFailureMessage ()
		{
			return name + " assertion failed.\n(" + go + ")." + thisPropertyPath + " failed.";
		}
	}

	public abstract class ActionBaseGeneric<T> : ActionBase
	{
		protected override bool Compare(object objVal)
		{
			return Compare ((T) objVal);
		}
		protected abstract bool Compare(T objVal);

		public override Type[] GetAccepatbleTypesForA()
		{
			return new[] { typeof(T) };
		}

		public override Type GetParameterType ()
		{
			return typeof(T);
		}
	}
}
