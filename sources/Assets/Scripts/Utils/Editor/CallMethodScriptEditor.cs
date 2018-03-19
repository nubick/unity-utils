using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Assets.Scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
{
    [CustomEditor(typeof(CallMethodScript))]
    public class CallMethodScriptEditor : UnityEditor.Editor
    {
        private CallMethodScript Target { get { return target as CallMethodScript; } }

        public override void OnInspectorGUI()
        {
            GUILayout.BeginVertical();

            Component component = DrawSelectComponent();

            bool isShowPrivateNew = EditorGUILayout.Toggle("Include Private Methods", Target.IsShowPrivate);
            if (isShowPrivateNew != Target.IsShowPrivate)
            {
                Target.IsShowPrivate = isShowPrivateNew;
                EditorUtility.SetDirty(Target);
            }

            MethodInfo methodInfo = DrawSelectMethod(component);
            if (methodInfo == null)
                return;

            DrawInputParameter(methodInfo);

            if (GUILayout.Button("Execute"))
            {
                methodInfo.Invoke(component, Target.ParamterValues);
            }

            GUILayout.EndVertical();
        }

        private Component DrawSelectComponent()
        {
            List<Component> components = Target.gameObject.GetComponents<Component>().Where(_ => !(_ is CallMethodScript) && !(_ is Transform)).ToList();
            string[] componentNames = components.Select(_ => _.GetType().Name).ToArray();
            int newComponentIndex = EditorGUILayout.Popup("Component", Target.SelectedComponentIndex, componentNames);
            if (newComponentIndex != Target.SelectedComponentIndex)
            {
                Target.SelectedComponentIndex = newComponentIndex;
                EditorUtility.SetDirty(Target);
            }
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
                {
                    Target.SelectedMethodIndex = newMethodIndex;
                    EditorUtility.SetDirty(Target);
                }
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
                foreach(ParameterInfo parameterInfo in parameterInfos)
                {
                    if (parameterInfo.ParameterType == typeof(string))
                        typeStrings.Add("str");
                    else if (parameterInfo.ParameterType == typeof(int))
                        typeStrings.Add("int");
                    else if (parameterInfo.ParameterType == typeof(float))
                        typeStrings.Add("float");
                    else if (parameterInfo.ParameterType.IsSubclassOf(typeof(Object)))
                        typeStrings.Add("uobj");
                    else
                        typeStrings.Add("not-supported");                    
                }
                string aggregatedTypes = typeStrings.Any() ? typeStrings.Aggregate((s1, s2) => s1 + "," + s2) : string.Empty;
                methodNames[i] = string.Format("{0}({1})", methodInfos[i].Name, aggregatedTypes);
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

            if (Target.ParamterValues == null || Target.ParamterValues.Length != parameterInfos.Length)
                Target.ParamterValues = new object[parameterInfos.Length];

            for (int i = 0; i < parameterInfos.Length; i++)
            {
                if (parameterInfos[i].ParameterType == typeof(string))
                {
                    string oldValue = Target.ParamterValues[i] is string ? (string)Target.ParamterValues[i] : string.Empty;
                    string newValue = EditorGUILayout.TextField("String parameter", oldValue);
                    if (newValue != oldValue)
                    {
                        Target.ParamterValues[i] = newValue;
                        EditorUtility.SetDirty(Target);
                    }
                }
                else if (parameterInfos[i].ParameterType == typeof(int))
                {
                    int oldValue = Target.ParamterValues[i] is int ? (int)Target.ParamterValues[i] : 0;
                    int newValue = EditorGUILayout.IntField("Integer parameter", oldValue);
                    if (newValue != oldValue)
                    {
                        Target.ParamterValues[i] = newValue;
                        EditorUtility.SetDirty(Target);
                    }
                }
                else if (parameterInfos[i].ParameterType == typeof(float))
                {
                    float oldValue = Target.ParamterValues[i] is float ? (float)Target.ParamterValues[i] : 0f;
                    float newValue = EditorGUILayout.FloatField("Float parameter", oldValue);
                    if (!Mathf.Approximately(oldValue, newValue))
                    {
                        Target.ParamterValues[i] = newValue;
                        EditorUtility.SetDirty(Target);
                    }
                }
                else if (parameterInfos[i].ParameterType.IsSubclassOf(typeof(Object)))
                {
                    System.Type parameterType = parameterInfos[i].ParameterType;

                    Object oldValue = null;
                    if (Target.ParamterValues[i] != null && Target.ParamterValues[i].GetType() == parameterType)
                        oldValue = Target.ParamterValues[i] as Object;

                    Object newValue = EditorGUILayout.ObjectField("Object parameter", oldValue, parameterType, true);
                    if (newValue != oldValue)
                    {
                        Target.ParamterValues[i] = newValue;
                        EditorUtility.SetDirty(Target);
                    }
                }
            }
        }
    }
}