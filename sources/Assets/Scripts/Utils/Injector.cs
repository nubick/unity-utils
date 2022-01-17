using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Injection
{
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	public class Inject : Attribute { }
	
	public class Injector
	{
		private static readonly Dictionary<Type, FieldInfo[]> FieldsCache = new Dictionary<Type, FieldInfo[]>();
		private static readonly Dictionary<Type, PropertyInfo[]> PropertiesCache = new Dictionary<Type, PropertyInfo[]>();
		private readonly Dictionary<Type, object> _objects = new Dictionary<Type, object>();
		private BindingFlags BindingFlags => BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

		public void Bind<T>(T obj)
		{
			Type type = typeof(T);
			
			if (_objects.ContainsKey(type))
				throw new ArgumentException($"Injector contains instance of type '{type.FullName}'.");
			_objects[type] = obj;
		}

		public void CommitBindings()
		{
			foreach (Type type in _objects.Keys)
			{
				if (_objects[type] == null)
				{
					Debug.Log($"Injecting:{type.Name}. Can't InjectTo null object.");
				}
				else
				{
					Debug.Log($"Injecting:{type.Name}");
					InjectTo(_objects[type]);
				}
			}
		}

		public void InjectTo(object obj)
		{
			Type objType = obj.GetType();
			
			foreach (FieldInfo fieldInfo in GetFields(objType))
			{
				object value = Get(fieldInfo.FieldType);
				
				if (value == null)
					Debug.Log($"Injecting:[WARNING]: Can't find type {fieldInfo.FieldType.FullName} for object: {obj}.");

				fieldInfo.SetValue(obj, value);
			}

			foreach (PropertyInfo propertyInfo in GetProperties(objType))
			{
				object value = Get(propertyInfo.PropertyType);
				
				if (value == null)
					Debug.Log($"Injecting:[WARNING]: Can't find type {propertyInfo.PropertyType.FullName} for object: {obj}.");

				propertyInfo.SetValue(obj, value);
			}
		}

		private object Get(Type type)
		{
			if (_objects.ContainsKey(type))
				return _objects[type];
			
			return null;
		}

		public T Get<T>()
		{
			object obj = Get(typeof(T));

			if (obj == null)
				Debug.Log($"Injecting:[WARNING]: Can't find type {typeof(T)}.");

			return (T) obj;
		}

		public object[] GetAll()
		{
			return _objects.Values.ToArray();
		}
		
		private FieldInfo[] GetFields(Type type)
		{
			if (!FieldsCache.ContainsKey(type))
				FieldsCache[type] = GetMemberInfos(type, t => t.GetFields(BindingFlags));
			return FieldsCache[type];
		}

		private PropertyInfo[] GetProperties(Type type)
		{
			if (!PropertiesCache.ContainsKey(type))
				PropertiesCache[type] = GetMemberInfos(type, t => t.GetProperties(BindingFlags));
			return PropertiesCache[type];
		}
		
		private T[] GetMemberInfos<T>(Type type, Func<Type, T[]> getMembersFunc) where T : MemberInfo
		{
			List<T> membersList = new List<T>();
			while (type != typeof(object))
			{
				var fields = getMembersFunc(type).Where(fieldInfo => fieldInfo.IsDefined(typeof(Inject), inherit: false));
				membersList.AddRange(fields);
				type = type.BaseType;
			}
			return membersList.ToArray();
		}
	}
}