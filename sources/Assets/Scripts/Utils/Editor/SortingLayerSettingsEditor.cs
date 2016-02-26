using System;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Assets.Scripts.Utils.Editor
{
    [CustomEditor(typeof(SortingLayerSettings))]
    public class SortingLayerSettingsEditor : UnityEditor.Editor
    {
        private Renderer _renderer;
        private string[] _sortingLayerNames;

        public void OnEnable()
        {
            _renderer = (target as SortingLayerSettings).GetComponent<Renderer>();
            _sortingLayerNames = GetSortingLayerNames();
        }

        public override void OnInspectorGUI()
        {
            if (_renderer == null)
            {
                EditorGUILayout.LabelField("Game Object doesn't have renderer.");
            }
            else
            {
                int ind = Mathf.Max(0, Array.IndexOf(_sortingLayerNames, _renderer.sortingLayerName));
                int changedInd = EditorGUILayout.Popup("Sorting Layer", ind, _sortingLayerNames);
                if (changedInd != ind)
                {
                    Undo.RecordObject(_renderer, "Edit Sorting Layer Name");
                    _renderer.sortingLayerName = _sortingLayerNames[changedInd];
                    EditorUtility.SetDirty(_renderer);
                }

                int sortingOrder = _renderer.sortingOrder;
                int changedSortingOrder = EditorGUILayout.IntField("Order in Layer", sortingOrder);
                if (changedSortingOrder != sortingOrder)
                {
                    Undo.RecordObject(_renderer, "Edit Sorting Order");
                    _renderer.sortingOrder = changedSortingOrder;
                    EditorUtility.SetDirty(_renderer);
                }
            }
        }

        public string[] GetSortingLayerNames()
        {
            Type internalEditorUtilityType = typeof(InternalEditorUtility);
            PropertyInfo sortingLayersProperty = internalEditorUtilityType.GetProperty(
                "sortingLayerNames", BindingFlags.Static | BindingFlags.NonPublic);
            return (string[])sortingLayersProperty.GetValue(null, new object[0]);
        }
    }
}
