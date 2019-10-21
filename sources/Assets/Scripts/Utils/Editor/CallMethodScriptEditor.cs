using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Tests;
using Assets.Scripts.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Editor
{
	[CustomEditor(typeof(CallMethodScript))]
	[CanEditMultipleObjects]
	public class CallMethodScriptEditor : UnityEditor.Editor
	{
		private CallMethodScript Target => target as CallMethodScript;

		public override void OnInspectorGUI()
		{
			GUILayout.BeginVertical();

			Component component = DrawSelectComponent();

			bool isShowPrivateNew = EditorGUILayout.Toggle("Include Private Methods", Target.IsShowPrivate);
			if (isShowPrivateNew != Target.IsShowPrivate)
				UpdateIsShowPrivate(isShowPrivateNew);

			MethodInfo methodInfo = DrawSelectMethod(component);
			if (methodInfo == null)
				return;

			DrawInputParameter(methodInfo);
			DrawExecuteButtons(methodInfo, component);

			GUILayout.EndVertical();
		}

		private void DrawExecuteButtons(MethodInfo methodInfo, Component component)
		{
			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Execute"))
			{
				if (targets.Length > 1)
					ExecuteMultiple();
				else
					methodInfo.Invoke(component, Target.ParameterValues);

			}

			if (targets.Length == 1 && typeof(IEnumerator).IsAssignableFrom(methodInfo.ReturnType))
			{
				if (!EditorApplication.isPlaying)
					GUI.enabled = false;
				
				if (GUILayout.Button("Start Coroutine"))
				{
					IEnumerator routine = (IEnumerator)methodInfo.Invoke(component, Target.ParameterValues);
					if (routine == null)
						Debug.LogError("Routine is null!");
					else
						(component as MonoBehaviour).StartCoroutine(routine);
				}

				if(GUILayout.Button("Stop All Coroutines"))
				{
					(component as MonoBehaviour).StopAllCoroutines();
				}

				GUI.enabled = true;
			}

			GUILayout.EndHorizontal();
		}

		private List<Component> GetComponents(GameObject go)
		{
			return go.GetComponents<Component>().Where(_ => !(_ is CallMethodScript) && !(_ is Transform)).ToList();
		}

		private Component DrawSelectComponent()
		{
			List<Component> components = GetComponents(Target.gameObject);
			string[] componentNames = components.Select(_ => _.GetType().Name).ToArray();
			int newComponentIndex = EditorGUILayout.Popup("Component", Target.SelectedComponentIndex, componentNames);
			if (newComponentIndex != Target.SelectedComponentIndex)
				UpdateSelectedComponentIndex(newComponentIndex);
			return components[Target.SelectedComponentIndex];
		}

		private MethodInfo DrawSelectMethod(Component component)
		{
			MethodInfo selectedMethodInfo = null;
			MethodInfo[] methodInfos = component.GetType().GetMethods(GetFilter());
			if (methodInfos.Any())
			{
				string[] methodNames = BuildMethodNames(methodInfos);
				int oldMethodIndex = Target.SelectedMethodIndex > methodInfos.Length - 1 ? 0 : Target.SelectedMethodIndex;
				int newMethodIndex = EditorGUILayout.Popup("Method", oldMethodIndex, methodNames);
				if (newMethodIndex != Target.SelectedMethodIndex)
					UpdateSelectedMethodIndex(newMethodIndex);
				selectedMethodInfo = methodInfos[Target.SelectedMethodIndex];
			}
			return selectedMethodInfo;
		}

		private string[] BuildMethodNames(MethodInfo[] methodInfos)
		{
			string[] methodNames = new string[methodInfos.Length];
			for (int i = 0; i < methodInfos.Length; i++)
			{
				MethodInfo methodInfo = methodInfos[i];
				ParameterInfo[] parameterInfos = methodInfo.GetParameters();
				List<string> typeStrings = new List<string>();
				foreach (ParameterInfo parameterInfo in parameterInfos)
				{
					if (parameterInfo.ParameterType == typeof(string))
						typeStrings.Add("str");
					else if (parameterInfo.ParameterType == typeof(int))
						typeStrings.Add("int");
					else if (parameterInfo.ParameterType == typeof(float))
						typeStrings.Add("float");
					else if (parameterInfo.ParameterType == typeof(bool))
						typeStrings.Add("bool");
					else if (parameterInfo.ParameterType.IsEnum)
						typeStrings.Add("enum");
					else if (parameterInfo.ParameterType.IsSubclassOf(typeof(Object)))
						typeStrings.Add("uobj");
					else
						typeStrings.Add("not-supported");
				}
				string aggregatedTypes = typeStrings.Any() ? typeStrings.Aggregate((s1, s2) => s1 + "," + s2) : string.Empty;
				methodNames[i] = $"{methodInfos[i].Name}({aggregatedTypes})";
			}
			return methodNames;
		}

		private BindingFlags GetFilter()
		{
			BindingFlags filter = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.Public;
			if (Target.IsShowPrivate)
				filter |= BindingFlags.NonPublic;
			return filter;
		}

		private void DrawInputParameter(MethodInfo methodInfo)
		{
			ParameterInfo[] parameterInfos = methodInfo.GetParameters();

			UpdateParameterValuesArraySize(parameterInfos.Length);

			for (int i = 0; i < parameterInfos.Length; i++)
			{
				if (parameterInfos[i].ParameterType == typeof(string))
				{
					string oldValue = Target.ParameterValues[i] is string ? (string)Target.ParameterValues[i] : string.Empty;
					string newValue = EditorGUILayout.TextField(parameterInfos[i].Name + " (str)", oldValue);
					if (newValue != oldValue)
						UpdateParameterValue(i, newValue);
				}
				else if (parameterInfos[i].ParameterType == typeof(int))
				{
					int oldValue = Target.ParameterValues[i] is int ? (int)Target.ParameterValues[i] : 0;
					int newValue = EditorGUILayout.IntField(parameterInfos[i].Name + " (int)", oldValue);
					if (newValue != oldValue)
						UpdateParameterValue(i, newValue);
				}
				else if (parameterInfos[i].ParameterType == typeof(float))
				{
					float oldValue = Target.ParameterValues[i] is float ? (float)Target.ParameterValues[i] : 0f;
					float newValue = EditorGUILayout.FloatField(parameterInfos[i].Name + " (float)", oldValue);
					if (!Mathf.Approximately(oldValue, newValue))
						UpdateParameterValue(i, newValue);
				}
				else if (parameterInfos[i].ParameterType == typeof(bool))
				{
					bool oldValue = Target.ParameterValues[i] is bool ? (bool) Target.ParameterValues[i] : false;
					bool newValue = EditorGUILayout.Toggle(parameterInfos[i].Name + " (bool)", oldValue);
					if (newValue != oldValue)
						UpdateParameterValue(i, newValue);
				}
				else if (parameterInfos[i].ParameterType.IsEnum)
				{
					Enum oldValue;
					if (Target.ParameterValues[i] != null && Target.ParameterValues[i].GetType() == parameterInfos[i].ParameterType)
						oldValue = (Enum) Target.ParameterValues[i];
					else
						oldValue = (Enum) Enum.GetValues(parameterInfos[i].ParameterType).GetValue(0);						
					
					Enum newValue = EditorGUILayout.EnumPopup(parameterInfos[i].Name + " (enum)", oldValue);
					if(newValue != oldValue)
						UpdateParameterValue(i, newValue);
				}
				else if (parameterInfos[i].ParameterType.IsSubclassOf(typeof(Object)))
				{
					System.Type parameterType = parameterInfos[i].ParameterType;

					Object oldValue = null;
					if (Target.ParameterValues[i] != null && Target.ParameterValues[i].GetType() == parameterType)
						oldValue = Target.ParameterValues[i] as Object;

					Object newValue = EditorGUILayout.ObjectField("Object parameter", oldValue, parameterType, true);
					if (newValue != oldValue)
						UpdateParameterValue(i, newValue);
				}
			}
		}

		#region Multiple Objects Updates

		private void UpdateParameterValuesArraySize(int length)
		{
			foreach(CallMethodScript t in targets)
			{
				if (t.ParameterValues == null || t.ParameterValues.Length != length)
				{
					t.ParameterValues = new object[length];
					EditorUtility.SetDirty(t);
				}					
			}
		}

		private void UpdateSelectedComponentIndex(int newComponentIndex)
		{
			foreach (CallMethodScript t in targets)
			{
				t.SelectedComponentIndex = newComponentIndex;
				EditorUtility.SetDirty(t);
			}
		}

		private void UpdateSelectedMethodIndex(int newMethodIndex)
		{
			foreach (CallMethodScript t in targets)
			{
				t.SelectedMethodIndex = newMethodIndex;
				EditorUtility.SetDirty(t);
			}
		}

		private void UpdateIsShowPrivate(bool isShowPrivateNew)
		{
			foreach (CallMethodScript t in targets)
			{
				t.IsShowPrivate = isShowPrivateNew;
				EditorUtility.SetDirty(t);
			}
		}

		private void UpdateParameterValue(int index, object newValue)
		{
			foreach (CallMethodScript t in targets)
			{
				t.ParameterValues[index] = newValue;
				EditorUtility.SetDirty(t);
			}
		}

		private void ExecuteMultiple()
		{
			foreach (CallMethodScript t in targets)
			{
				Component component = GetComponents(t.gameObject)[t.SelectedComponentIndex];
				MethodInfo methodInfo = component.GetType().GetMethods(GetFilter())[t.SelectedMethodIndex];
				methodInfo.Invoke(component, t.ParameterValues);
			}
		}

		#endregion
	}
}