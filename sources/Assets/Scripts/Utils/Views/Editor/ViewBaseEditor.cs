using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils.Views.Editor
{
    [CustomEditor(typeof(ViewBase), true)]
    public class ViewBaseEditor : UnityEditor.Editor
    {
        private ViewBase View { get { return target as ViewBase; } }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (EditorApplication.isPlaying)
                DrawRuntimeEditorActions();
        }

        private void DrawRuntimeEditorActions()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Show"))
                View.Show();

            if (GUILayout.Button("Hide"))
                View.Hide();

            GUILayout.EndHorizontal();
        }
    }
}