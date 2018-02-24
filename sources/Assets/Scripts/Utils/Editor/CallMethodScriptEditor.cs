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

            bool isShowPrivateNew = EditorGUILayout.Toggle("Private Methods", Target.IsShowPrivate);
            if (isShowPrivateNew != Target.IsShowPrivate)
            {
                Target.IsShowPrivate = isShowPrivateNew;
                EditorUtility.SetDirty(Target);
            }

            DrawSelectMethod(component);

            if (GUILayout.Button("Execute"))
            {
                MethodInfo methodInfo = component.GetType().GetMethods(GetFilter())[Target.SelectedMethodIndex];
                methodInfo.Invoke(component, null);
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

        private void DrawSelectMethod(Component component)
        {
            MethodInfo[] methodInfos = component.GetType().GetMethods(GetFilter());
            string[] methodNames = methodInfos.Select(_ => _.Name).ToArray();
            int newMethodIndex = EditorGUILayout.Popup("Method", Target.SelectedMethodIndex, methodNames);
            if (newMethodIndex != Target.SelectedMethodIndex)
            {
                Target.SelectedMethodIndex = newMethodIndex;
                EditorUtility.SetDirty(Target);
            }
        }

        private BindingFlags GetFilter()
        {
            BindingFlags filter = BindingFlags.Instance | BindingFlags.DeclaredOnly;
            filter |= Target.IsShowPrivate ? BindingFlags.NonPublic : BindingFlags.Public;
            return filter;
        }
    }
}