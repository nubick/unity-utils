using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace UnityTest.IntegrationTests
{
	[Serializable]
	public class PlatformRunnerSettingsWindow : EditorWindow
	{
		private BuildTarget buildTarget;
		private List<string> sceneList;
		private Vector2 scrollPosition;
		private List<string> interfaces = new List<string> ();
		private List<string> selectedScenes = new List<string> ();
		private int selectedInterface;
		[SerializeField]
		private bool advancedNetworkingSettings;

		private PlatformRunnerSettings settings;

		GUIContent label = new GUIContent ("Results target directory", "Directory where the results will be saved. If no value is specified, the results will be generated in project's data folder.");
		

		public PlatformRunnerSettingsWindow ()
		{
			title = "Platform runner";
			buildTarget = PlatformRunner.defaultBuildTarget;
			position.Set (position.xMin, position.yMin, 200, position.height);
			sceneList = Directory.GetFiles (Directory.GetCurrentDirectory (), "*.unity", SearchOption.AllDirectories).ToList ();
			sceneList.Sort();
			var currentScene = (Directory.GetCurrentDirectory () + EditorApplication.currentScene).Replace ("\\", "").Replace ("/", "");
			var currentScenePath = sceneList.Where (s => s.Replace ("\\", "").Replace ("/", "") == currentScene);
			selectedScenes.AddRange (currentScenePath);

			interfaces.Add ("(Any)");
			interfaces.AddRange (TestRunnerConfigurator.GetAvailableNetworkIPs());
			interfaces.Add ("127.0.0.1");
		}

		public void OnEnable ()
		{
			settings = ProjectSettingsBase.Load<PlatformRunnerSettings> ();
		}

		private void OnGUI ()
		{
			EditorGUILayout.BeginVertical ();

			EditorGUILayout.LabelField ("List of scenes to build:", EditorStyles.boldLabel);
			scrollPosition = EditorGUILayout.BeginScrollView (scrollPosition, Styles.testList);
			EditorGUI.indentLevel++;
			foreach (var scenePath in sceneList)
			{
				var path = Path.GetFileNameWithoutExtension (scenePath);
				var guiContent = new GUIContent (path, scenePath);
				var rect = GUILayoutUtility.GetRect (guiContent, EditorStyles.label);
				if (rect.Contains (Event.current.mousePosition))
				{
					if (Event.current.type == EventType.mouseDown && Event.current.button == 0)
					{
						if (!Event.current.control)
							selectedScenes.Clear ();
						if (!selectedScenes.Contains (scenePath))
							selectedScenes.Add (scenePath);
						else
							selectedScenes.Remove (scenePath);
						Event.current.Use ();
					}
				}
				var style = new GUIStyle(EditorStyles.label);
				if (selectedScenes.Contains (scenePath))
					style.normal.textColor = new Color (0.3f, 0.5f, 0.85f);
				EditorGUI.LabelField (rect, guiContent, style);
			}
			EditorGUI.indentLevel--;
			EditorGUILayout.EndScrollView ();

			GUILayout.Space (3);

			buildTarget = (BuildTarget) EditorGUILayout.EnumPopup ("Build tests for", buildTarget);

			if (PlatformRunner.defaultBuildTarget != buildTarget)
			{
				if (GUILayout.Button ("Make default target platform"))
				{
					PlatformRunner.defaultBuildTarget = buildTarget;
				}
			}
			DrawSetting ();
			var build = GUILayout.Button ("Build and run tests");
			EditorGUILayout.EndVertical ();

			if (!build) return;

			var config = new PlatformRunnerConfiguration ()
			{
				buildTarget = buildTarget,
				scenes = selectedScenes.ToArray (),
				projectName = selectedScenes.Count > 1 ? "IntegrationTests" : Path.GetFileNameWithoutExtension (EditorApplication.currentScene),
				resultsDir = settings.resultsPath,
				sendResultsOverNetwork = settings.sendResultsOverNetwork,
				ipList = interfaces.Skip (1).ToList (),
				port = settings.port
			};

			if (selectedInterface > 0)
				config.ipList = new List<string> {interfaces.ElementAt(selectedInterface)};

			PlatformRunner.BuildAndRunInPlayer (config);
			Close ();
		}

		private void DrawSetting ()
		{
			EditorGUI.BeginChangeCheck ();

			EditorGUILayout.BeginHorizontal ();
			settings.resultsPath = EditorGUILayout.TextField (label, settings.resultsPath);
			if (GUILayout.Button ("search", EditorStyles.miniButton, GUILayout.Width (50)))
			{
				var selectedPath = EditorUtility.SaveFolderPanel ("Result files destination", settings.resultsPath, "");
				if (!string.IsNullOrEmpty (selectedPath))
					settings.resultsPath = Path.GetFullPath(selectedPath);
			}
			EditorGUILayout.EndHorizontal ();

			if (!string.IsNullOrEmpty (settings.resultsPath))
			{
				Uri uri;
				if (!Uri.TryCreate (settings.resultsPath, UriKind.Absolute, out uri) || !uri.IsFile || uri.IsWellFormedOriginalString ())
				{
					EditorGUILayout.HelpBox ("Invalid URI path", MessageType.Warning);
				}
			}

			settings.sendResultsOverNetwork = EditorGUILayout.Toggle ("Send results to editor", settings.sendResultsOverNetwork);
			EditorGUI.BeginDisabledGroup (!settings.sendResultsOverNetwork);
			advancedNetworkingSettings = EditorGUILayout.Foldout(advancedNetworkingSettings, "Advanced network settings");
			if (advancedNetworkingSettings)
			{
				selectedInterface = EditorGUILayout.Popup ("Network interface", selectedInterface, interfaces.ToArray ());
				EditorGUI.BeginChangeCheck ();
				settings.port = EditorGUILayout.IntField ("Network port", settings.port);
				if (EditorGUI.EndChangeCheck ())
				{
					if (settings.port > IPEndPoint.MaxPort)
						settings.port = IPEndPoint.MaxPort;
					else if (settings.port < IPEndPoint.MinPort)
						settings.port = IPEndPoint.MinPort;
				}
				EditorGUI.EndDisabledGroup ();
			}
			if (EditorGUI.EndChangeCheck ())
			{
				settings.Save ();
			}
		}
	}
}
