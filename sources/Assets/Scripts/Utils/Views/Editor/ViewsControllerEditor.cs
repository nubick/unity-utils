using UnityEngine;
using UnityEditor;

namespace Assets.Scripts.Utils.Views.Editor
{
	[CustomEditor(typeof(ViewsController))]
	public class ViewsControllerEditor : UnityEditor.Editor
	{
		private ViewBase[] _views;

		private ViewsController ViewsController { get { return target as ViewsController; } }

		public void OnEnable()
		{
			_views = ViewsController.GetComponentsInChildren<ViewBase>();
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			GUILayout.BeginVertical();
			foreach (ViewBase view in _views)
			{
				bool newValue = GUILayout.Toggle(view.Content.activeSelf, view.name, "Button");
				if (newValue != view.Content.activeSelf)
				{
					bool wasActive = view.Content.activeSelf;
					UnSelectAll();
					if (!wasActive)
						view.Content.SetActive(true);
				}
			}
			GUILayout.EndVertical();
		}

		private void UnSelectAll()
		{
			foreach (ViewBase view in _views)
				view.Content.SetActive(false);
		}
	}
}