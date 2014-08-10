using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace UnityTest
{
	[Serializable]
	public partial class UnitTestView : EditorWindow
	{
		private static UnitTestView Instance;
		private static IUnitTestEngine testEngine = new NUnitTestEngine ();

		[SerializeField] private List<UnitTestResult> resultList = new List<UnitTestResult> ();
		[SerializeField] private string[] availableCategories = null;
		[SerializeField] private List<string> foldMarkers = new List<string> ();
		[SerializeField] private List<UnitTestRendererLine> selectedLines = new List<UnitTestRendererLine> ();
		UnitTestRendererLine testLines;

		#region runner steering vars
		private Vector2 testListScroll, testInfoScroll;
		private float horizontalSplitBarPosition = 200;
		private float verticalSplitBarPosition = 300;
		#endregion

		private UnitTestsRunnerSettings settings;

		#region GUI Contents
		private readonly GUIContent guiRunSelectedTestsIcon = new GUIContent (Icons.runImg, "Run selected tests");
		private readonly GUIContent guiRunAllTestsIcon = new GUIContent (Icons.runAllImg, "Run all tests");
		private readonly GUIContent guiRerunFailedTestsIcon = new GUIContent (Icons.runFailedImg, "Rerun failed tests");
		private readonly GUIContent guiOptionButton = new GUIContent ("Options", Icons.gearImg);
		private readonly GUIContent guiHideButton = new GUIContent ("Hide", Icons.gearImg);
		private readonly GUIContent guiRunOnRecompile = new GUIContent ("Run on recompile", "Run all tests after recompilation");
		private readonly GUIContent guiShowDetailsBelowTests = new GUIContent ("Show details below tests", "Show run details below test list");
		private readonly GUIContent guiRunTestsOnNewScene = new GUIContent ("Run tests on a new scene", "Run tests on a new scene");
		private readonly GUIContent guiAutoSaveSceneBeforeRun = new GUIContent ("Autosave scene", "The runner will automaticall save current scene changes before it starts");
		private readonly GUIContent guiShowSucceededTests = new GUIContent ("Succeeded", Icons.successImg, "Show tests that succeeded");
		private readonly GUIContent guiShowFailedTests = new GUIContent ("Failed", Icons.failImg, "Show tests that failed");
		private readonly GUIContent guiShowIgnoredTests = new GUIContent ("Ignored", Icons.ignoreImg, "Show tests that are ignored");
		private readonly GUIContent guiShowNotRunTests = new GUIContent ("Not Run", Icons.unknownImg, "Show tests that didn't run");
		#endregion

		public UnitTestView ()
		{
			title = "Unit Tests Runner";
			resultList.Clear ();
		}

		public void OnEnable ()
		{
			Instance = this;
			settings = ProjectSettingsBase.Load<UnitTestsRunnerSettings> ();
			RefreshTests ();
			EnableBackgroundRunner (settings.runOnRecompilation);
		}

		public void OnDestroy ()
		{
			Instance = null;
			EnableBackgroundRunner (false);
		}

		public void Awake ()
		{
			RefreshTests ();
		}
		
		public void OnGUI ()
		{
			GUILayout.Space (10);
			EditorGUILayout.BeginVertical ();

			EditorGUILayout.BeginHorizontal ();

			var layoutOptions = new[] { GUILayout.Width(32), GUILayout.Height(24) };
			if (GUILayout.Button (guiRunAllTestsIcon, Styles.buttonLeft, layoutOptions))
			{
				RunTests();
				GUIUtility.ExitGUI ();
			}
			if (GUILayout.Button (guiRunSelectedTestsIcon, Styles.buttonMid, layoutOptions))
			{
				testLines.RunSelectedTests ();
			}
			if (GUILayout.Button (guiRerunFailedTestsIcon, Styles.buttonRight, layoutOptions))
			{
				testLines.RunTests (resultList.Where(result => result.IsFailure || result.IsError).Select (l => l.FullName).ToArray ());
			}

			GUILayout.FlexibleSpace ();

			if (GUILayout.Button (settings.optionsFoldout ? guiHideButton : guiOptionButton, GUILayout.Height (24), GUILayout.Width (80)))
			{
				settings.optionsFoldout = !settings.optionsFoldout;
			}
			EditorGUILayout.EndHorizontal ();

			if (settings.optionsFoldout) DrawOptions ();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("Filter:", GUILayout.Width(35));
			settings.testFilter = EditorGUILayout.TextField (settings.testFilter, EditorStyles.textField);

			if (availableCategories != null && availableCategories.Length > 1)
				settings.categoriesMask = EditorGUILayout.MaskField (settings.categoriesMask, availableCategories, GUILayout.MaxWidth (90));

			if (GUILayout.Button (settings.filtersFoldout ? "Hide" : "Advanced", GUILayout.Width (80), GUILayout.Height (15)))
				settings.filtersFoldout = !settings.filtersFoldout;
			EditorGUILayout.EndHorizontal ();

			if (settings.filtersFoldout)
				DrawFilters ();

			if (settings.horizontalSplit)
				EditorGUILayout.BeginVertical ();
			else
				EditorGUILayout.BeginHorizontal (GUILayout.ExpandWidth (true));

			RenderTestList ();
			RenderTestInfo ();

			if (settings.horizontalSplit)
				EditorGUILayout.EndVertical ();
			else
				EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
		}

		private string[] GetSelectedCategories ()
		{
			var selectedCategories = new List<string> ();
			foreach (var availableCategory in availableCategories)
			{
				var idx = Array.FindIndex (availableCategories, ( a ) => a == availableCategory);
				var mask = 1 << idx;
				if ((settings.categoriesMask & mask) != 0) selectedCategories.Add (availableCategory);
			}
			return selectedCategories.ToArray ();
		}
		
		private void RenderTestList ()
		{
			EditorGUILayout.BeginVertical (Styles.testList);
			testListScroll = EditorGUILayout.BeginScrollView (testListScroll, 
																GUILayout.ExpandWidth (true),
																GUILayout.MaxWidth (2000));
			if (testLines != null)
			{
				var options = new RenderingOptions ();
				options.showSucceeded = settings.showSucceeded;
				options.showFailed = settings.showFailed;
				options.showIgnored = settings.showIgnored;
				options.showNotRunned = settings.showNotRun;
				options.nameFilter = settings.testFilter;
				options.categories = GetSelectedCategories ();

				if (testLines.Render (options)) Repaint ();
			}
			EditorGUILayout.EndScrollView ();
			EditorGUILayout.EndVertical ();
		}

		private void RenderTestInfo ()
		{
			var ctrlId = EditorGUIUtility.GetControlID (FocusType.Passive);
			var rect = GUILayoutUtility.GetLastRect ();
			if (settings.horizontalSplit)
			{
				rect.y = rect.height + rect.y - 1;
				rect.height = 3;
			}
			else
			{
				rect.x = rect.width + rect.x - 1;
				rect.width = 3;
			}

			EditorGUIUtility.AddCursorRect (rect, settings.horizontalSplit ? MouseCursor.ResizeVertical : MouseCursor.ResizeHorizontal);
			var e = Event.current;
			switch (e.type)
			{
				case EventType.MouseDown:
					if (EditorGUIUtility.hotControl == 0 && rect.Contains (e.mousePosition))
						EditorGUIUtility.hotControl = ctrlId;
					break;
				case EventType.MouseDrag:
					if (EditorGUIUtility.hotControl == ctrlId)
					{
						horizontalSplitBarPosition -= e.delta.y;
						if (horizontalSplitBarPosition < 20) horizontalSplitBarPosition = 20;
						verticalSplitBarPosition -= e.delta.x;
						if (verticalSplitBarPosition < 20) verticalSplitBarPosition = 20;
						Repaint ();
					}
						
				break;
				case EventType.MouseUp:
					if (EditorGUIUtility.hotControl == ctrlId)
						EditorGUIUtility.hotControl = 0;
					break;
			}
			testInfoScroll = EditorGUILayout.BeginScrollView (testInfoScroll, settings.horizontalSplit 
																			? GUILayout.MinHeight (horizontalSplitBarPosition) 
																			: GUILayout.Width (verticalSplitBarPosition));

			var text = "";
			if (selectedLines.Any ())
			{
				text = selectedLines.First ().GetResultText ();
			}
			EditorGUILayout.TextArea (text, Styles.info);

			EditorGUILayout.EndScrollView ();
		}

		private void DrawFilters ()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.BeginHorizontal ();
			settings.showSucceeded = GUILayout.Toggle (settings.showSucceeded, guiShowSucceededTests, GUI.skin.FindStyle (GUI.skin.button.name + "left"), GUILayout.ExpandWidth (true));
			settings.showFailed = GUILayout.Toggle (settings.showFailed, guiShowFailedTests, GUI.skin.FindStyle (GUI.skin.button.name + "mid"));
			settings.showIgnored = GUILayout.Toggle (settings.showIgnored, guiShowIgnoredTests, GUI.skin.FindStyle (GUI.skin.button.name + "mid"));
			settings.showNotRun = GUILayout.Toggle (settings.showNotRun, guiShowNotRunTests, GUI.skin.FindStyle (GUI.skin.button.name + "right"), GUILayout.ExpandWidth (true));
			EditorGUILayout.EndHorizontal ();
			if (EditorGUI.EndChangeCheck()) settings.Save ();
		}

		private void DrawOptions ()
		{
			EditorGUI.BeginChangeCheck ();

			EditorGUI.BeginChangeCheck ();
			settings.runOnRecompilation = EditorGUILayout.Toggle (guiRunOnRecompile, settings.runOnRecompilation);
			if (EditorGUI.EndChangeCheck ()) EnableBackgroundRunner (settings.runOnRecompilation);

			settings.runTestOnANewScene = EditorGUILayout.Toggle (guiRunTestsOnNewScene, settings.runTestOnANewScene);
			EditorGUI.BeginDisabledGroup (!settings.runTestOnANewScene);
			settings.autoSaveSceneBeforeRun = EditorGUILayout.Toggle (guiAutoSaveSceneBeforeRun, settings.autoSaveSceneBeforeRun);
			EditorGUI.EndDisabledGroup ();
			settings.horizontalSplit = EditorGUILayout.Toggle (guiShowDetailsBelowTests, settings.horizontalSplit);

			if (EditorGUI.EndChangeCheck ())
			{
				settings.Save ();
			}
			EditorGUILayout.Space ();
		}

		private void RefreshTests ()
		{
			UnitTestResult[] newResults;
			testLines = testEngine.GetTests (out newResults, out availableCategories);
			
			foreach (var newResult in newResults)
			{
				var result = resultList.Where (t => t.Test == newResult.Test && t.FullName == newResult.FullName).ToArray();
				if (result.Count () != 1) continue;
				newResult.Update(result.Single(), true);
			}

			UnitTestRendererLine.SelectedLines = selectedLines;
			UnitTestRendererLine.RunTest = RunTests;
			GroupLine.FoldMarkers = foldMarkers;
			TestLine.GetUnitTestResult = FindTestResult;

			resultList = new List<UnitTestResult> (newResults);

			Repaint ();
		}
	}
}
