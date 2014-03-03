using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UnityTest
{
	public class ScriptSelector
	{
		public List<MethodInfo> GetSortedMenuList(Type type, Type firstArgumentType, Type secondArgumentType)
		{
			var list = GetMethodList(type, firstArgumentType, secondArgumentType);
			var result = new List<MethodInfo> ();
				
			result.AddRange(FilterDeclaredPublic(list));
			result.AddRange(FilterDeclaredStatic(list));
			result.AddRange(FilterInheritedPublic(list));
			result.AddRange(FilterInheritedStatic(list));

			return result;
		}

		public List<MethodInfo> GetStaticSortedMenuList(Type type, Type firstArgument, Type secondArgument)
		{
			var list = GetMethodList(type, firstArgument, secondArgument);
			var result = new List<MethodInfo>();
			result.AddRange(FilterDeclaredStatic(list));
			result.AddRange(FilterInheritedStatic(list));
			return result;
		}

		internal List<MethodInfo> GetMethodList(Type classType, Type firstArgumentType, Type secondArgumentType)
		{
			var allPublic = classType.GetMethods(); 
			var allStatic = classType.GetMethods(BindingFlags.Static | BindingFlags.FlattenHierarchy | BindingFlags.Public);
			var result = new List<MethodInfo> ().Union (allPublic).Union (allStatic);
			result = result.Where (info => !info.ContainsGenericParameters);
			result = result.Where (info =>
			{
				var methodParams = info.GetParameters ();
				if (methodParams.Length == 0) return true;
				if (firstArgumentType!=null && methodParams.Length == 1)
				{
					if (methodParams[0].ParameterType.IsAssignableFrom(firstArgumentType) || methodParams[0].ParameterType.IsAssignableFrom(typeof(GameObject)))
						return true;

				}
				if (firstArgumentType != null && secondArgumentType!=null && methodParams.Length == 2)
				{
					if ((methodParams[0].ParameterType.IsAssignableFrom(firstArgumentType)
						&& methodParams[1].ParameterType.IsAssignableFrom(secondArgumentType))
						||
						(methodParams[0].ParameterType.IsAssignableFrom(typeof(GameObject))
						&& methodParams[1].ParameterType.IsAssignableFrom(typeof(GameObject)))
						)
						return true;
				}
				return false;
			});

			return result.ToList ();
		}

		internal IEnumerable<MethodInfo> FilterDeclaredPublic (IEnumerable<MethodInfo> methods)
		{
			return methods.Where(m => m.IsPublic && !m.IsStatic && m.ReflectedType == m.DeclaringType);
		}

		internal IEnumerable<MethodInfo> FilterDeclaredStatic (IEnumerable<MethodInfo> methods)
		{
			return methods.Where(m => m.IsPublic && m.IsStatic && m.ReflectedType == m.DeclaringType);
		}

		internal IEnumerable<MethodInfo> FilterInheritedPublic (IEnumerable<MethodInfo> methods)
		{
			return methods.Where(m => m.IsPublic && !m.IsStatic && m.ReflectedType != m.DeclaringType);
		}

		internal IEnumerable<MethodInfo> FilterInheritedStatic (IEnumerable<MethodInfo> methods)
		{
			return methods.Where(m => m.IsPublic && m.IsStatic && m.ReflectedType != m.DeclaringType);
		}
	}
}
