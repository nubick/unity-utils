using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
	public abstract class ProjectSettingsBase : ScriptableObject
	{
		private static string settingsPath = Path.Combine ("UnityTestTools", "Common");
		private static string settingsFolder = "Settings";

		public void Save ()
		{
			EditorUtility.SetDirty (this);
		}

		public static T Load<T> () where T : ProjectSettingsBase, new ()
		{
			var pathsInProject = Directory.GetDirectories ("Assets", "*", SearchOption.AllDirectories)
				.Where (s => s.Contains (settingsPath));

			if (pathsInProject.Count () == 0) Debug.LogError ("Can't find settings path: " + settingsPath);
			
			string pathInProject = Path.Combine (pathsInProject.First (), settingsFolder);
			var assetPath = Path.Combine (pathInProject, typeof (T).Name) + ".asset";
			var settings = AssetDatabase.LoadAssetAtPath (assetPath, typeof (T)) as T;

			if (settings != null) return settings;

			settings = CreateInstance<T> ();
			Directory.CreateDirectory(pathInProject);
			AssetDatabase.CreateAsset (settings, assetPath);
			return settings;
		}
	}
}
