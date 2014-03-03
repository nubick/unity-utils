using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
	[Serializable]
	public class IntegrationTestRunnerRenderer
	{
		private static class Styles 
		{
			public static readonly GUIStyle selectedTestStyle;
			public static readonly GUIStyle testStyle;
			public static readonly GUIStyle iconStyle;
			public static GUIStyle buttonLeft; 
			public static GUIStyle buttonMid;
			public static GUIStyle buttonRight;

			static Styles ()
			{
				testStyle = new GUIStyle (EditorStyles.label);
				selectedTestStyle = new GUIStyle (EditorStyles.label);
				selectedTestStyle.active.textColor = selectedTestStyle.normal.textColor = selectedTestStyle.onActive.textColor = new Color (0.3f, 0.5f, 0.85f);
				iconStyle = new GUIStyle(EditorStyles.label);
				iconStyle.fixedWidth = 24;
				buttonLeft = GUI.skin.FindStyle (GUI.skin.button.name + "left");
				buttonMid = GUI.skin.FindStyle (GUI.skin.button.name + "mid");
				buttonRight = GUI.skin.FindStyle (GUI.skin.button.name + "right");
			}
		}

		private Action<IList<GameObject>> RunTest;
		private TestManager testManager;
		private bool showDetails;

		#region runner options vars
		[SerializeField] private bool showOptions;
		[SerializeField] private bool showTestRunnerObjectOnScene = true;
		[SerializeField] private bool addNewGameObjectUnderSelectedTest = false;
		[SerializeField] private bool focusOnSelection = true;
		[SerializeField] private bool hideTestsInHierarchy;
		[SerializeField] private bool keepTestComponentObjectsOnTop = true;
		[SerializeField] internal bool blockUIWhenRunning = true;
		#endregion

		#region filter vars
		[SerializeField] private bool showAdvancedFilter;
		[SerializeField] private string filterString = "";
		[SerializeField] private bool showSucceededTest = true;
		[SerializeField] internal bool showFailedTest = true;
		[SerializeField] private bool showNotRunnedTest = true;
		[SerializeField] private bool showIgnoredTest = true;
		#endregion

		#region runner steering vars
		[SerializeField] private Vector2 testListScroll;
		[SerializeField] public bool forceRepaint;
		private List<TestResult> selectedTests = new List<TestResult>();
		#endregion

		#region GUI Contents
		private readonly GUIContent guiOptionsHideLabel = new GUIContent ("Hide", Icons.gearImg);
		private readonly GUIContent guiOptionsShowLabel = new GUIContent ("Options", Icons.gearImg);
		private readonly GUIContent guiCreateNewTest = new GUIContent (Icons.plusImg, "Create new test");
		private readonly GUIContent guiRunSelectedTests = new GUIContent (Icons.runImg, "Run selected test(s)");
		private readonly GUIContent guiRunAllTests = new GUIContent (Icons.runAllImg, "Run all tests");
		private readonly GUIContent guiAdvancedFilterShow = new GUIContent ("Advanced");
		private readonly GUIContent guiAdvancedFilterHide = new GUIContent ("Hide");
		private readonly GUIContent guiTimeoutIcon = new GUIContent (Icons.stopwatchImg, "Timeout");
		private readonly GUIContent guiRunSelected = new GUIContent ("Run selected");
		private readonly GUIContent guiRun = new GUIContent ("Run");
		private readonly GUIContent guiRunAll = new GUIContent ("Run All");
		private readonly GUIContent guiRunAllIncludingIgnored = new GUIContent ("Run All (include ignored)");
		private readonly GUIContent guiDelete = new GUIContent ("Delete");
		private readonly GUIContent guiAddGOUderTest = new GUIContent ("Add GOs under test", "Add new GameObject under selected test");
		private readonly GUIContent guiBlockUI = new GUIContent ("Block UI when running", "Block UI when running tests");
		private readonly GUIContent guiHideTestInHierarchy = new GUIContent ("Hide tests in hierarchy", "Hide tests in hierarchy");
		private readonly GUIContent guiHideTestRunner = new GUIContent ("Hide Test Runner", "Hides Test Runner object in hierarchy");
		#endregion

		public IntegrationTestRunnerRenderer (Action<IList<GameObject>> RunTest)
		{
			testManager = new TestManager ();
			this.RunTest = RunTest;

			if (EditorPrefs.HasKey ("ITR-addNewGameObjectUnderSelectedTest"))
			{
				addNewGameObjectUnderSelectedTest = EditorPrefs.GetBool ("ITR-addNewGameObjectUnderSelectedTest");
				focusOnSelection = EditorPrefs.GetBool ("ITR-focusOnSelection");
				hideTestsInHierarchy = EditorPrefs.GetBool ("ITR-hideTestsInHierarchy");
				keepTestComponentObjectsOnTop = EditorPrefs.GetBool ("ITR-keepTestComponentObjectsOnTop");
				showOptions = EditorPrefs.GetBool ("ITR-showOptions");
				showTestRunnerObjectOnScene = EditorPrefs.GetBool ("ITR-showTestRunnerObjectOnScene");
				blockUIWhenRunning = EditorPrefs.GetBool ("ITR-blockUIWhenRunning");
				showAdvancedFilter = EditorPrefs.GetBool ("ITR-showAdvancedFilter");
				filterString = EditorPrefs.GetString ("ITR-filterString");
				showSucceededTest = EditorPrefs.GetBool ("ITR-showSucceededTest");
				showFailedTest = EditorPrefs.GetBool ("ITR-showFailedTest");
				showIgnoredTest = EditorPrefs.GetBool ("ITR-showIgnoredTest");
				showNotRunnedTest = EditorPrefs.GetBool ("ITR-showNotRunnedTest");
			}
		}

		private  void SaveSettings()
		{
			EditorPrefs.SetBool("ITR-addNewGameObjectUnderSelectedTest", addNewGameObjectUnderSelectedTest);
			EditorPrefs.SetBool("ITR-focusOnSelection", focusOnSelection);
			EditorPrefs.SetBool("ITR-hideTestsInHierarchy", hideTestsInHierarchy);
			EditorPrefs.SetBool("ITR-keepTestComponentObjectsOnTop", keepTestComponentObjectsOnTop);
			EditorPrefs.SetBool("ITR-showOptions", showOptions);
			EditorPrefs.SetBool("ITR-showTestRunnerObjectOnScene", showTestRunnerObjectOnScene);
			EditorPrefs.SetBool("ITR-blockUIWhenRunning", blockUIWhenRunning);
			EditorPrefs.SetBool ("ITR-showAdvancedFilter", showAdvancedFilter);
			EditorPrefs.SetString ("ITR-filterString", filterString);
			EditorPrefs.SetBool ("ITR-showSucceededTest", showSucceededTest);
			EditorPrefs.SetBool ("ITR-showFailedTest", showFailedTest);
			EditorPrefs.SetBool ("ITR-showIgnoredTest", showIgnoredTest);
			EditorPrefs.SetBool ("ITR-showNotRunnedTest", showNotRunnedTest);
		}

		private void DrawTest (TestResult testInfo)
		{
			EditorGUIUtility.SetIconSize (new Vector2 (16, 16));
			Color tempColor = GUI.color;
			if (testInfo.isRunning)
			{
				var frame = Mathf.Abs(Mathf.Cos (Time.realtimeSinceStartup*4)) * 0.6f + 0.4f;
				GUI.color = new Color (1, 1, 1, frame);
			}

			var label = new GUIContent (testInfo.name, GetIconBasedOnResultType (testInfo).image);
			var labelRect = GUILayoutUtility.GetRect (label, EditorStyles.label, GUILayout.ExpandWidth (true));

			if (labelRect.Contains (Event.current.mousePosition)
				&& Event.current.type == EventType.MouseDown
				&& Event.current.button == 0)
			{
				SelectTest (testInfo);
			}
			else if (labelRect.Contains (Event.current.mousePosition)
					&& Event.current.type == EventType.ContextClick)
			{
				Event.current.Use ();
				DrawContextTestMenu (testInfo);
			}
			EditorGUI.LabelField (labelRect,
								label,
								selectedTests.Contains (testInfo) ? Styles.selectedTestStyle : Styles.testStyle);

			if (testInfo.isRunning) GUI.color = tempColor;
			EditorGUIUtility.SetIconSize (Vector2.zero);
		}

		private void SelectTest (TestResult testToSelect)
		{
			if (!Event.current.control && !Event.current.shift)
				selectedTests.Clear();
			if (Event.current.control && selectedTests.Contains (testToSelect))
				selectedTests.Remove (testToSelect);
			else if (Event.current.shift && selectedTests.Any ())
			{
				var tests = testManager.GetTestsToSelect(selectedTests, testToSelect);
				selectedTests.Clear ();
				selectedTests.AddRange (tests);
			}
			else
				selectedTests.Add (testToSelect);
			if (!EditorApplication.isPlayingOrWillChangePlaymode && selectedTests.Count == 1)
			{
				var selectedTest = selectedTests.Single ();
				TestManager.SelectInHierarchy(selectedTest.go, hideTestsInHierarchy);
				EditorApplication.RepaintHierarchyWindow ();
			}
			Selection.objects = selectedTests.Select(result => result.go).ToArray();
			forceRepaint = true;
			GUI.FocusControl("");
		}

		private GUIContent GetIconBasedOnResultType (TestResult result)
		{
			if (result == null) 
				return Icons.guiUnknownImg;
			if (result.isRunning)
				return Icons.guiUnknownImg;

			if (result.resultType == TestResult.ResultType.NotRun
				&& result.TestComponent.ignored)
				return Icons.guiIgnoreImg;

			switch (result.resultType)
			{
				case TestResult.ResultType.Success:
					return Icons.guiSuccessImg;
				case TestResult.ResultType.Timeout:
				case TestResult.ResultType.Failed:
				case TestResult.ResultType.FailedException:
					return Icons.guiFailImg;
				case TestResult.ResultType.Ignored:
					return Icons.guiIgnoreImg;
				case TestResult.ResultType.NotRun:
				default:
					return Icons.guiUnknownImg;
			}
		}

		public void PrintHeadPanel (bool isRunning)
		{
			var sceneName = "";
			if (!string.IsNullOrEmpty (EditorApplication.currentScene))
			{
				sceneName = EditorApplication.currentScene.Substring (EditorApplication.currentScene.LastIndexOf ('/') + 1);
				sceneName = sceneName.Substring (0,
												sceneName.LastIndexOf ('.'));
			}
			GUILayout.Label ("Integration Tests (" + sceneName + ")",
							EditorStyles.boldLabel);
			
			EditorGUILayout.BeginHorizontal ();

			var layoutOptions = new [] {
								GUILayout.Height(24),
								GUILayout.Width(32),
								};
			if (GUILayout.Button (guiRunAllTests,
								Styles.buttonLeft,
								layoutOptions
								) && !isRunning)
			{
				RunTest (GetVisibleNotIgnoredTests ());
			}
			if (GUILayout.Button(guiRunSelectedTests,
								Styles.buttonMid,
								layoutOptions
								) && !isRunning)
			{
				RunTest(selectedTests.Select (t=>t.go).ToList ());
			}
			if (GUILayout.Button (guiCreateNewTest,
								Styles.buttonRight,
								layoutOptions
								) && !isRunning)
			{
				SelectTest (testManager.AddTest ());
			}
			GUILayout.FlexibleSpace ();


			if (GUILayout.Button (showOptions ? guiOptionsHideLabel : guiOptionsShowLabel, GUILayout.Height (24), GUILayout.Width (80)))
			{
				showOptions = !showOptions;
				SaveSettings ();
			}
			EditorGUILayout.EndHorizontal ();

			if(showOptions)
				PrintOptions();

			EditorGUILayout.BeginHorizontal ();

			EditorGUILayout.LabelField ("Filter:", GUILayout.Width (35));

			EditorGUI.BeginChangeCheck ();
			filterString = EditorGUILayout.TextField (filterString);
			if(EditorGUI.EndChangeCheck ())
				SaveSettings ();

			if (GUILayout.Button (showAdvancedFilter ? guiAdvancedFilterHide : guiAdvancedFilterShow, GUILayout.Width (80)))
			{
				showAdvancedFilter = !showAdvancedFilter;
				SaveSettings ();
			}
			EditorGUILayout.EndHorizontal ();
			if (showAdvancedFilter)
				PrintAdvancedFilter ();

			GUILayout.Space (5);
		}

		private void PrintAdvancedFilter ()
		{
			EditorGUILayout.BeginHorizontal ();
			EditorGUILayout.BeginVertical ();
			EditorGUI.BeginChangeCheck ();
			showSucceededTest = EditorGUILayout.Toggle ("Show succeeded",
									showSucceededTest);
			showFailedTest = EditorGUILayout.Toggle ("Show failed",
									showFailedTest);
			
			EditorGUILayout.EndVertical ();
			EditorGUILayout.BeginVertical ();

			showIgnoredTest = EditorGUILayout.Toggle ("Show ignored",
									showIgnoredTest);
			showNotRunnedTest = EditorGUILayout.Toggle ("Show not runed",
									showNotRunnedTest);

			if(EditorGUI.EndChangeCheck ())
				SaveSettings ();
			EditorGUILayout.EndVertical ();
			EditorGUILayout.EndHorizontal ();
		}

		public void PrintTestList ()
		{
			GUILayout.Box ("",
							new[] {GUILayout.ExpandWidth (true), GUILayout.Height (1)});
			GUILayout.Space (5);

			testListScroll = EditorGUILayout.BeginScrollView (testListScroll,
															new[] {GUILayout.ExpandHeight (true)});

			foreach (var testInfo in GetFilteredTestsResults ())
			{
				EditorGUILayout.BeginHorizontal();

				DrawTest (testInfo);
				
				if (testInfo.resultType == TestResult.ResultType.Timeout)
				{
					GUILayout.Label(guiTimeoutIcon,
									GUILayout.Width(24)
									);
					GUILayout.FlexibleSpace();
				}
				
				EditorGUILayout.EndHorizontal ();
			}
			EditorGUILayout.EndScrollView ();
		}

		private List<TestResult> GetFilteredTestsResults ()
		{
			return testManager.GetAllTestsResults ().Where (IsNotFiltered).ToList ();
		}

		public void PrintSelectedTestDetails ()
		{
			if (Event.current.type == EventType.Layout)
			{
				if (showDetails != selectedTests.Any ())
					showDetails = !showDetails;
			}
			if (!showDetails) return;

			GUILayout.Box ("",
							new[] { GUILayout.ExpandWidth (true), GUILayout.Height (1) });

			EditorGUILayout.LabelField ("Test details");
			
			string messages = "", stacktrace = "";
			if (selectedTests.Count == 1)
			{
				var test = selectedTests.Single();
				if (test != null)
				{
					messages = test.messages;
					stacktrace = test.stacktrace;
				}
			}

			EditorGUILayout.SelectableLabel (messages,
									EditorStyles.miniLabel,
									GUILayout.MaxHeight(50));

			EditorGUILayout.SelectableLabel(stacktrace,
									EditorStyles.miniLabel,
									GUILayout.MaxHeight(50));
			
		}

		private void DrawContextTestMenu (TestResult test)
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode) return;

			var m = new GenericMenu ();
			var localTest = test;
			if(selectedTests.Count > 1)
				m.AddItem(guiRunSelected,
						false,
						data => RunTest(selectedTests.Select (t=>t.go).ToList ()),
						"");
			m.AddItem (guiRun,
						false,
						data => RunTest(new List<GameObject> { localTest.go}),
						"");
			m.AddItem (guiRunAll,
						false,
						data => RunTest (GetVisibleNotIgnoredTests ()),
						"");
			m.AddItem (guiRunAllIncludingIgnored,
						false,
						data => RunTest (GetVisibleTestsIncludingIgnored ()),
						"");
			m.AddSeparator ("");
			m.AddItem (guiDelete,
						false,
						data => RemoveTest (localTest),
						"");

			m.ShowAsContext ();
		}

		private void RemoveTest (TestResult test)
		{
			var testsToDelete = new List<TestResult> { test };
			if (selectedTests.Count > 1)
				testsToDelete = selectedTests;

			if (EditorUtility.DisplayDialog ("",
											"Are you sure you want to delete " + 
											((testsToDelete.Count > 1) ? (testsToDelete.Count + " tests?"):(testsToDelete.Single().name + "?")),
											"Delete",
											"Cancel"))
			{
				foreach (var t in testsToDelete)
				{	
#if !UNITY_4_0 && !UNITY_4_0_1 && !UNITY_4_1 && !UNITY_4_2
					Undo.DestroyObjectImmediate ((t as TestResult).go);
#else
					Undo.RegisterSceneUndo ("Destroy Objects");
					GameObject.DestroyImmediate (t.go);
#endif
				}

				testManager.DeleteTest(testsToDelete);
				selectedTests.Clear ();
				forceRepaint = true;
			}
		}

		public void OnHierarchyWindowItemOnGui (int id, Rect rect)
		{
			var o = EditorUtility.InstanceIDToObject (id);
			if (o is GameObject)
			{
				var go = o as GameObject;
				if (TestManager.AnyTestsOnScene() && go.GetComponent<TestComponent>() != null)
				{
					if (!EditorApplication.isPlayingOrWillChangePlaymode
						&& rect.Contains (Event.current.mousePosition)
						&& Event.current.type == EventType.MouseDown
						&& Event.current.button == 1)
					{
						DrawContextTestMenu (testManager.GetResultFor(go));
					}

					EditorGUIUtility.SetIconSize (new Vector2 (15, 15));
					EditorGUI.LabelField (new Rect (rect.xMax - 18,
													rect.yMin - 2,
													rect.width,
													rect.height),
										GetIconBasedOnResultType (testManager.GetResultFor(go)));
					EditorGUIUtility.SetIconSize (Vector2.zero);
				}
			}
		}

		public void PrintOptions ()
		{
			EditorGUILayout.BeginVertical ();
			EditorGUI.BeginChangeCheck();

			var style = EditorStyles.toggle;

			//Temporary disabled
			//focusOnSelection = EditorGUILayoutExt.ToggleLeft((new GUIContent("Focus on selected test", "Focus on selected test")),
			//												focusOnSelection,
			//												style);

			addNewGameObjectUnderSelectedTest = EditorGUILayout.Toggle(guiAddGOUderTest,
																		addNewGameObjectUnderSelectedTest,
																		style);

			blockUIWhenRunning = EditorGUILayout.Toggle(guiBlockUI,
														blockUIWhenRunning,
														style);
			
			EditorGUI.BeginChangeCheck ();
			hideTestsInHierarchy = EditorGUILayout.Toggle(guiHideTestInHierarchy,
															hideTestsInHierarchy,
															style);
			if (EditorGUI.EndChangeCheck ())
			{
				TestManager.ShowOrHideTestInHierarchy(hideTestsInHierarchy);
				if (selectedTests.Count == 1)
					TestManager.SelectInHierarchy(selectedTests.Single().go, hideTestsInHierarchy);
				EditorApplication.RepaintHierarchyWindow();
			}
			EditorGUI.BeginChangeCheck();
			showTestRunnerObjectOnScene = !EditorGUILayout.Toggle(guiHideTestRunner,
																	!showTestRunnerObjectOnScene,
																	style);
			if (EditorGUI.EndChangeCheck ())
				ShowTestRunnerObjectOnScene(showTestRunnerObjectOnScene);

			if (EditorGUI.EndChangeCheck ())
				SaveSettings ();

			EditorGUILayout.EndVertical ();
		}

		private void ShowTestRunnerObjectOnScene (bool show)
		{
			var tr = TestRunner.GetTestRunner ();
			if (show)
				tr.gameObject.hideFlags = 0;
			else
				tr.gameObject.hideFlags |= HideFlags.HideInHierarchy;
			tr.hideFlags = HideFlags.NotEditable;
			EditorUtility.SetDirty(tr.gameObject);
		}

		public void SelectInHierarchy (IEnumerable<GameObject> go)
		{
			selectedTests.Clear();
			selectedTests.AddRange(go.Select (o=>testManager.GetResultFor (o)));
			if (selectedTests.Count () == 1)
				TestManager.SelectInHierarchy (selectedTests.Single ().go,
												hideTestsInHierarchy);

			if (focusOnSelection && SceneView.currentDrawingSceneView != null)
			{
				SceneView.lastActiveSceneView.FrameSelected ();
			}
		}

		public void InvalidateTestList ()
		{
			selectedTests.Clear ();
			testManager.ClearTestList ();
		}

		public void UpdateResults (List<TestResult> testToRun)
		{
			testManager.UpdateResults (testToRun);
		}

		public void OnHierarchyChange(bool isRunning)
		{
			if (isRunning
				|| EditorApplication.isPlayingOrWillChangePlaymode)
				return;

			if (addNewGameObjectUnderSelectedTest
				&& Selection.activeGameObject != null)
			{
				var go = Selection.activeGameObject;
				if (selectedTests.Count == 1
					&& go.transform.parent == null
					&& go.GetComponent<TestComponent>() == null
					&& go.GetComponent<TestRunner>() == null)
				{
					go.transform.parent = selectedTests.Single().go.transform;
				}
			}

			if (keepTestComponentObjectsOnTop)
			{
				foreach (var test in TestManager.GetAllTestGameObjects())
				{
					if (test.transform.parent != null)
					{
						test.transform.parent = null;
						Debug.LogWarning("Tests need to be on top of hierarchy.");
					}
				}
			}

			if (Selection.gameObjects.Count() > 1 
				&& Selection.gameObjects.All(o => o is GameObject && o.GetComponent<TestComponent>()))
			{
				selectedTests.Clear ();
				selectedTests.AddRange (Selection.gameObjects.Select (go=>testManager.GetResultFor (go)));
				forceRepaint = true;
			}
		}

		public void OnTestRunFinished ()
		{
			if(selectedTests.Count==1)
				TestManager.SelectInHierarchy(selectedTests.Single().go, hideTestsInHierarchy);
		}

		public List<TestResult> GetTestResultsForGO (IList<GameObject> tests)
		{
			return testManager.GetAllTestsResults ().Where (t => tests.Contains (t.go)).ToList ();
		}

		private bool IsNotFiltered (TestResult testInfo)
		{
			if (!testInfo.name.ToLower ().Contains (filterString.Trim ().ToLower ())) return false;
			if (!showSucceededTest && testInfo.resultType == TestResult.ResultType.Success) return false;
			if (!showFailedTest && (testInfo.resultType == TestResult.ResultType.Failed
				|| testInfo.resultType == TestResult.ResultType.FailedException
				|| testInfo.resultType == TestResult.ResultType.Timeout)) return false;
			if (!showIgnoredTest && (testInfo.resultType == TestResult.ResultType.Ignored || testInfo.TestComponent.ignored)) return false;
			if (!showNotRunnedTest && testInfo.resultType == TestResult.ResultType.NotRun) return false;
			return true;
		}

		public List<GameObject> GetVisibleNotIgnoredTests ()
		{
			return testManager.GetAllTestsResults ().Where (tr => tr.TestComponent.ignored != true).Where (IsNotFiltered).Select (result => result.go).ToList ();
		}

		public List<GameObject> GetVisibleTestsIncludingIgnored ()
		{
			return testManager.GetAllTestsResults ().Where (IsNotFiltered).Select (result => result.go).ToList ();
		}
	}
}
