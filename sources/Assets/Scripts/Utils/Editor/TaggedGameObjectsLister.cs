using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils.Editor
{
	public class TaggedGameObjectsLister: EditorWindow
	{
		private readonly List<GameObject> _components;
		private string _tag;
		private Vector2 _scrollPosition;

		public TaggedGameObjectsLister()
		{
			_components = new List<GameObject>();
			_tag = string.Empty;
			_scrollPosition = Vector2.zero;
		}

		[MenuItem("Window/Utils/Find game objects by tag")]
        public static void Launch()
		{
			EditorWindow window = GetWindow(typeof(TaggedGameObjectsLister));
			window.Show();
		}

		public void OnGUI()
		{
			DrawSearchPanel();
			DrawComponents();
		}

		private void DrawSearchPanel()
		{
			GUILayout.BeginHorizontal(GUI.skin.GetStyle("Box"));
			GUILayout.Label("Input tag:");
			GUILayout.FlexibleSpace();
			_tag = GUILayout.TextField(_tag, GUILayout.Width(200));
			if (GUILayout.Button("Refresh") && !string.IsNullOrEmpty(_tag))
			{
				UpdateList(_tag);
			}
			GUILayout.EndHorizontal();
		}

		private void DrawComponents()
		{
			_scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
			foreach (GameObject component in _components)
			{
				if (GUILayout.Button(component.name))
				{
					Selection.activeObject = component;
				}
			}
			GUILayout.EndScrollView();
		}

		public void UpdateList(string tag)
		{
			_components.Clear();
			Object[] components = Resources.FindObjectsOfTypeAll(typeof(GameObject));
			foreach (GameObject component in components)
			{
				if (component.tag.ToLower() == tag.ToLower())
					_components.Add(component);
			}
		}
	}
}
