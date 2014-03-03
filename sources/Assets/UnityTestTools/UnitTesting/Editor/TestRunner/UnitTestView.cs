using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityTest.UnitTestRunner;

namespace UnityTest
{
	[Serializable]
	public class UnitTestView : EditorWindow
	{
		private static class Styles
		{
			public static GUIStyle buttonLeft;
			public static GUIStyle buttonMid;
			public static GUIStyle buttonRight;
			static Styles ()
			{
				buttonLeft = GUI.skin.FindStyle (GUI.skin.button.name + "left");
				buttonMid = GUI.skin.FindStyle (GUI.skin.button.name + "mid");
				buttonRight = GUI.skin.FindStyle (GUI.skin.button.name + "right");
			}
		}

		private List<IUnitTestEngine> testEngines = new List<IUnitTestEngine> ();

		[SerializeField]
		private List<UnitTestResult> testList = new List<UnitTestResult> ();

		#region renderer list
		[SerializeField]
		private int selectedRenderer;
		[SerializeField]
		private List<GroupedByHierarchyRenderer> rendererList = new List<GroupedByHierarchyRenderer> ()
		{
			new GroupedByHierarchyRenderer()
		};
		#endregion

		#region runner steering vars
		private bool isCompiling;
		private Vector2 testListScroll, testInfoScroll, toolbarScroll;
		private bool shouldUpdateTestList;
		private string[] testsToRunList;
		private bool readyToRun;
		#endregion
		
		#region runner options vars
		private bool optionsFoldout;
		private bool runOnRecompilation;
		private bool horizontalSplit = true;
		private bool autoSaveSceneBeforeRun;
		private bool runTestOnANewScene = true;
		private bool notifyOnSlowRun;
		private float slowTestThreshold = 1f;
		#endregion

		#region test filter vars
		private bool filtersFoldout;
		private string testFilter = "";
		private bool showFailed = true;
		private bool showIgnored = true;
		private bool showNotRun = true;
		private bool showSucceeded = true;
		private Rect toolbarRect;
		#endregion

		#region GUI Contents
		private readonly GUIContent guiRunSelectedTestsIcon = new GUIContent (Icons.runImg, "Run selected tests");
		private readonly GUIContent guiRunAllTestsIcon = new GUIContent (Icons.runAllImg, "Run all tests");
		private readonly GUIContent guiRerunFailedTestsIcon = new GUIContent (Icons.runFailedImg, "Rerun failed tests");
		private readonly GUIContent guiOptionButton = new GUIContent ("Options", Icons.gearImg);
		private readonly GUIContent guiRunOnRecompile = new GUIContent ("Run on recompile", "Run on recompile");
		private readonly GUIContent guiRunTestsOnNewScene = new GUIContent ("Run tests on a new scene", "Run tests on a new scene");
		private readonly GUIContent guiAutoSaveSceneBeforeRun = new GUIContent ("Autosave scene", "The runner will automaticall save current scene changes before it starts");
		private readonly GUIContent guiShowDetailsBelowTests = new GUIContent ("Show details below tests", "Show run details below test list");
		private readonly GUIContent guiNotifyWhenSlow = new GUIContent ("Notify when test is slow", "When test will run longer that set threshold, it will be marked as slow");
		private readonly GUIContent guiSlowTestThreshold = new GUIContent ("Slow test threshold");
		#endregion


		public UnitTestView ()
		{
			title = "Unit Tests Runner";

			if (EditorPrefs.HasKey ("UTR-runOnRecompilation"))
			{
				runOnRecompilation = EditorPrefs.GetBool ("UTR-runOnRecompilation");
				runTestOnANewScene = EditorPrefs.GetBool ("UTR-runTestOnANewScene");
				autoSaveSceneBeforeRun = EditorPrefs.GetBool ("UTR-autoSaveSceneBeforeRun");
				horizontalSplit = EditorPrefs.GetBool ("UTR-horizontalSplit");
				notifyOnSlowRun = EditorPrefs.GetBool ("UTR-notifyOnSlowRun");
				slowTestThreshold = EditorPrefs.GetFloat ("UTR-slowTestThreshold");
				filtersFoldout = EditorPrefs.GetBool ("UTR-filtersFoldout");
				showFailed = EditorPrefs.GetBool ("UTR-showFailed");
				showIgnored = EditorPrefs.GetBool ("UTR-showIgnored");
				showNotRun = EditorPrefs.GetBool ("UTR-showNotRun");
				showSucceeded = EditorPrefs.GetBool ("UTR-showSucceeded");
			}
			
			InstantiateUnitTestEngines();
		}

		private void InstantiateUnitTestEngines()
		{
			var type = typeof(IUnitTestEngine);
			var types =
				AppDomain.CurrentDomain.GetAssemblies()
						 .SelectMany(a => a.GetTypes())
						 .Where(type.IsAssignableFrom)
						 .Where(t => !t.IsInterface);
			IEnumerable<IUnitTestEngine> instances = types.Select(t => Activator.CreateInstance(t)).Cast<IUnitTestEngine>();
			testEngines.AddRange(instances);
		}

		public void SaveOptions()
		{
			EditorPrefs.SetBool("UTR-runOnRecompilation", runOnRecompilation);
			EditorPrefs.SetBool("UTR-runTestOnANewScene", runTestOnANewScene);
			EditorPrefs.SetBool("UTR-autoSaveSceneBeforeRun", autoSaveSceneBeforeRun);
			EditorPrefs.SetBool("UTR-horizontalSplit", horizontalSplit);
			EditorPrefs.SetBool("UTR-notifyOnSlowRun", notifyOnSlowRun);
			EditorPrefs.SetFloat("UTR-slowTestThreshold", slowTestThreshold);
			EditorPrefs.GetBool("UTR-filtersFoldout", filtersFoldout);
			EditorPrefs.SetBool("UTR-showFailed", showFailed);
			EditorPrefs.SetBool("UTR-showIgnored", showIgnored);
			EditorPrefs.SetBool("UTR-showNotRun", showNotRun);
			EditorPrefs.SetBool("UTR-showSucceeded", showSucceeded);
		}

		public void OnEnable ()
		{
			RefreshTests ();
			shouldUpdateTestList = true;
		}
		
		public void OnGUI ()
		{
			GUILayout.Space (10);
			EditorGUILayout.BeginVertical ();

			EditorGUILayout.BeginHorizontal ();

			var layoutOptions = new[] {
								GUILayout.Width(32),
								GUILayout.Height(24)
								};
			if (GUILayout.Button (guiRunAllTestsIcon, Styles.buttonLeft, layoutOptions))
			{
				RunTests (GetAllVisibleTests ());
			}
			if (GUILayout.Button (guiRunSelectedTestsIcon, Styles.buttonMid, layoutOptions))
			{
				RunTests(GetAllSelectedTests());
			}
			if (GUILayout.Button (guiRerunFailedTestsIcon, Styles.buttonRight, layoutOptions))
			{
				RunTests(GetAllFailedTests());
			}

			GUILayout.FlexibleSpace ();

			if (GUILayout.Button (guiOptionButton, GUILayout.Height(24), GUILayout.Width(80)))
			{
				optionsFoldout = !optionsFoldout;
			}
			EditorGUILayout.EndHorizontal ();
			
			if (optionsFoldout) DrawOptions();

			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.LabelField("Filter:", GUILayout.Width(33));
			testFilter = EditorGUILayout.TextField(testFilter, EditorStyles.textField);
			if (GUILayout.Button(filtersFoldout ? "Hide" : "Advanced", GUILayout.Width(80)))
				filtersFoldout = !filtersFoldout;
			EditorGUILayout.EndHorizontal ();
			
			if (filtersFoldout)
				DrawFilters ();
			
			GUILayout.Box ("", new [] {GUILayout.ExpandWidth (true), GUILayout.Height (1)});

			GetRenderer ().RenderOptions ();

			if (horizontalSplit)
				EditorGUILayout.BeginVertical ();
			else
				EditorGUILayout.BeginHorizontal ();

			RenderToolbar ();
			RenderTestList ();

			if (horizontalSplit)
				GUILayout.Box ("", new[] {GUILayout.ExpandWidth (true), GUILayout.Height (1)});
			else
				GUILayout.Box ("", new[] {GUILayout.ExpandHeight (true), GUILayout.Width (1)});

			RenderTestInfo ();

			if (horizontalSplit)
				EditorGUILayout.EndVertical ();
			else
				EditorGUILayout.EndHorizontal ();

			EditorGUILayout.EndVertical ();
		}

		private void RenderToolbar ()
		{
			if (rendererList.Count > 1)
			{
				toolbarScroll = EditorGUILayout.BeginScrollView(toolbarScroll, GUILayout.ExpandHeight(false));

				EditorGUILayout.BeginHorizontal ();
				var toolbarList = rendererList.Select(hierarchyRenderer =>
				{
					var label = hierarchyRenderer.filterString;
					if (string.IsNullOrEmpty (label)) label = "All tests";
					return new GUIContent (label, label);
				}).ToArray();

				if (toolbarRect.Contains (Event.current.mousePosition) 
					&& Event.current.type == EventType.MouseDown 
					&& Event.current.button == 1)
				{
					var tabWidth = toolbarRect.width / rendererList.Count;
					var tabNo = (int)(Event.current.mousePosition.x / tabWidth);
					if(tabNo != 0)
					{
						var menu = new GenericMenu ();
						menu.AddItem (new GUIContent ("Remove"), false, ()=>RemoveSelectedTab(tabNo));
						menu.ShowAsContext ();
					}
					Event.current.Use ();
				}
				
				selectedRenderer = GUILayout.Toolbar(selectedRenderer, toolbarList);
				if (Event.current.type == EventType.Repaint)
					toolbarRect = GUILayoutUtility.GetLastRect ();

				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.EndScrollView();
			}
		}

		private void RemoveSelectedTab (int idx)
		{
			rendererList.RemoveAt (idx);
			selectedRenderer--;
		}

		private GroupedByHierarchyRenderer GetRenderer ()
		{
			var r = rendererList.ElementAtOrDefault (selectedRenderer);
			if (r == null)
			{
				selectedRenderer = 0;
				r = rendererList[selectedRenderer];
			}
			return r;
		}

		private void RenderTestList ()
		{
			testListScroll = EditorGUILayout.BeginScrollView (testListScroll,
																GUILayout.ExpandHeight (true),
																GUILayout.ExpandWidth (true),
																horizontalSplit ? GUILayout.MinHeight (0) : GUILayout.MaxWidth (500),
																horizontalSplit ? GUILayout.MinWidth (0) : GUILayout.MinWidth (200));

			var filteredResults = FilterResults (testList);
			GetRenderer().notifyOnSlowRun = notifyOnSlowRun;
			GetRenderer().slowTestThreshold = slowTestThreshold;
			if (rendererList.ElementAtOrDefault(selectedRenderer) == null)
				selectedRenderer = 0;
			shouldUpdateTestList = rendererList[selectedRenderer].RenderTests (filteredResults, RunTests);
			EditorGUILayout.EndScrollView ();
		}

		private void RenderTestInfo ()
		{
			testInfoScroll = EditorGUILayout.BeginScrollView (testInfoScroll,
																GUILayout.ExpandHeight (true),
																GUILayout.ExpandWidth (true),
																horizontalSplit ? GUILayout.MaxHeight (200) : GUILayout.MinWidth (0));

			GetRenderer().RenderInfo();
			EditorGUILayout.EndScrollView ();
		}

		private void DrawFilters ()
		{
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.BeginVertical ();
			showSucceeded = EditorGUILayout.Toggle("Show succeeded", showSucceeded, GUILayout.MinWidth (120));
			showFailed = EditorGUILayout.Toggle("Show failed", showFailed, GUILayout.MinWidth (120));
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical ();
			showIgnored = EditorGUILayout.Toggle("Show ignored", showIgnored);
			showNotRun = EditorGUILayout.Toggle("Show not runned", showNotRun);
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
			if (EditorGUI.EndChangeCheck())
				SaveOptions();
		}

		private void DrawOptions ()
		{
			EditorGUI.BeginChangeCheck ();
			runOnRecompilation = EditorGUILayout.Toggle (guiRunOnRecompile, runOnRecompilation);
			runTestOnANewScene = EditorGUILayout.Toggle (guiRunTestsOnNewScene, runTestOnANewScene);
			if (runTestOnANewScene)
				autoSaveSceneBeforeRun = EditorGUILayout.Toggle (guiAutoSaveSceneBeforeRun, autoSaveSceneBeforeRun);
			horizontalSplit = EditorGUILayout.Toggle (guiShowDetailsBelowTests, horizontalSplit);
			notifyOnSlowRun = EditorGUILayout.Toggle(guiNotifyWhenSlow, notifyOnSlowRun);
			if (notifyOnSlowRun)
				slowTestThreshold = EditorGUILayout.FloatField(guiSlowTestThreshold, slowTestThreshold);
			if (EditorGUI.EndChangeCheck())
				SaveOptions();
			EditorGUILayout.Space ();
		}
		
		private IEnumerable<UnitTestResult> FilterResults (IEnumerable<UnitTestResult> mTestResults)
		{
			var results = mTestResults;

			if (!string.IsNullOrEmpty(GetRenderer ().filterString))
				results = results.Where(result => result.Test.FullClassName == GetRenderer().filterString);
			
			results = results.Where(r => r.Test.FullName.ToLower().Contains(testFilter.ToLower()));

			if (!showIgnored)
				results = results.Where (r => !r.IsIgnored);
			if (!showFailed)
				results = results.Where (r => !(r.IsFailure || r.IsError || r.IsInconclusive));
			if (!showNotRun)
				results = results.Where (r => r.Executed);
			if (!showSucceeded)
				results = results.Where (r => !r.IsSuccess);

			return results;
		}

		private string[] GetAllVisibleTests ()
		{
			return FilterResults (testList).Select (result => result.Test.FullName).ToArray ();
		}

		private string[] GetAllSelectedTests()
		{
			return GetRenderer().GetSelectedTests();
		}

		private string[] GetAllFailedTests ()
		{
			return FilterResults (testList).Where (result => result.IsError || result.IsFailure).Select (result => result.Test.FullName).ToArray ();
		}

		private void RefreshTests ()
		{
			var newTestResults = new List<UnitTestResult> ();
			var allTests = new List<UnitTestResult> ();
			foreach (var unitTestEngine in testEngines)
			{
				allTests.AddRange (unitTestEngine.GetTests (true));
			}

			foreach (var result in testList)
			{
				var test = allTests.SingleOrDefault (testResult => testResult.Test == result.Test);
				if (test != null)
				{
					newTestResults.Add (result);
					allTests.Remove(test);
				}
			}
			newTestResults.AddRange(allTests);
			testList = newTestResults;
		}

		private void UpdateTestInfo(ITestResult result)
		{
			FindTestResultByName(result.FullName).Update(result);
		}

		private UnitTestResult FindTestResultByName (string name)
		{
			var idx = testList.FindIndex(testResult => testResult.Test.FullName == name);
			return testList.ElementAt (idx);
		}

		public void Update ()
		{
			if (readyToRun)
			{
				readyToRun = false;
				StartTestRun();
			}

			if (shouldUpdateTestList)
			{
				shouldUpdateTestList = false;
				Repaint ();
			}
			if (EditorApplication.isCompiling && !isCompiling)
			{
				isCompiling = true;
			}
			if (isCompiling && !EditorApplication.isCompiling)
			{
				isCompiling = false;
				OnRecompile ();
			}
		}

		public void OnRecompile ()
		{
			RefreshTests ();
			if (runOnRecompilation && IsCompilationCompleted())
				RunTests (GetAllVisibleTests ());
		}

		private bool IsCompilationCompleted ()
		{
			return File.Exists (Path.GetFullPath ("Library/ScriptAssemblies/CompilationCompleted.txt"));
		}

		private void RunTests (string[] tests)
		{
			if (readyToRun)
			{
				Debug.LogWarning ("Tests are already running");
				return;
			}
			testsToRunList = tests;
			readyToRun = true;
		}

		private void StartTestRun ()
		{
			var okToRun = true;
			if (runTestOnANewScene && !UnityEditorInternal.InternalEditorUtility.inBatchMode)
			{
				if (autoSaveSceneBeforeRun)
					EditorApplication.SaveScene ();
				okToRun = EditorApplication.SaveCurrentSceneIfUserWantsTo ();
			}
			if (okToRun)
			{
				var currentScene = EditorApplication.currentScene;
				if (runTestOnANewScene || UnityEditorInternal.InternalEditorUtility.inBatchMode)
					EditorApplication.NewScene ();
				var callbackList = new TestRunnerCallbackList ();
				callbackList.Add (new TestRunnerEventListener (this));
				try
				{
					foreach (var unitTestEngine in testEngines)
					{
						unitTestEngine.RunTests (testsToRunList,
												callbackList);
					}
				}
				catch (Exception e)
				{
					Debug.LogException (e);
					callbackList.RunFinishedException (e);
				}
				finally
				{
					EditorUtility.ClearProgressBar();
					if (runTestOnANewScene && !UnityEditorInternal.InternalEditorUtility.inBatchMode)
						EditorApplication.OpenScene (currentScene);
					if (UnityEditorInternal.InternalEditorUtility.inBatchMode)
						EditorApplication.Exit(0);
					shouldUpdateTestList = true;
				}
			}
		}
		
		[MenuItem ("Unity Test Tools/Unit Test Runner %#&u")]
		public static void ShowWindow ()
		{
			GetWindow (typeof (UnitTestView)).Show ();
		}

		[MenuItem("Unity Test Tools/Run all unit tests")]
		public static void RunAllTestsBatch()
		{
			var window = GetWindow(typeof(UnitTestView)) as UnitTestView;
			window.RefreshTests ();
			window.RunTests (new string[0]);
		}

		[MenuItem("Assets/Unity Test Tools/Load tests from this file")]
		static void LoadTestsFromFile(MenuCommand command)
		{
			if (!ValidateLoadTestsFromFile() && Selection.objects.Any())
			{
				Debug.Log ("Not all selected files are script files");
			}
			var window = GetWindow(typeof(UnitTestView)) as UnitTestView;
			foreach (var o in Selection.objects)
			{
				window.selectedRenderer = window.AddNewRenderer((o as MonoScript).GetClass());
			}
			window.toolbarScroll = new Vector2(float.MaxValue, 0);
		}

		private int AddNewRenderer (Type classFilter)
		{
			var elem = rendererList.SingleOrDefault (hierarchyRenderer => hierarchyRenderer.filterString == classFilter.FullName);
			if ( elem == null)
			{
				elem = new GroupedByHierarchyRenderer (classFilter);
				rendererList.Add(elem);
			}
			return rendererList.IndexOf (elem);
		}

		[MenuItem("Assets/Unity Test Tools/Load tests from this file", true)]
		static bool ValidateLoadTestsFromFile()
		{
			return Selection.objects.All (o => o is MonoScript);
		}

		private class TestRunnerEventListener : ITestRunnerCallback
		{
			private UnitTestView unitTestView;

			public TestRunnerEventListener(UnitTestView unitTestView)
			{
				this.unitTestView = unitTestView;
			}

			public void TestStarted (string fullName)
			{
				EditorUtility.DisplayProgressBar("Unit Tests Runner",
													fullName,
													1);
			}

			public void TestFinished(ITestResult result)
			{
				unitTestView.UpdateTestInfo(result);
			}

			public void RunStarted (string suiteName, int testCount)
			{
			}

			public void RunFinished ()
			{
				var resultWriter = new XmlResultWriter("UnitTestResults.xml");
				resultWriter.SaveTestResult ("Unit Tests", unitTestView.testList.ToArray ());
				EditorUtility.ClearProgressBar();
			}

			public void RunFinishedException (Exception exception)
			{
				RunFinished ();
			}
		}
	}
}
