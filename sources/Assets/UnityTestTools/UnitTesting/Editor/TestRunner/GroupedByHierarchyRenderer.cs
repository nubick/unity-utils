using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
	[Serializable]
	class GroupedByHierarchyRenderer
	{
		private static class Styles
		{
			public static GUIStyle selectedLabel;
			public static GUIStyle label;
			public static GUIStyle selectedFoldout;
			public static GUIStyle foldout;
			public static GUIStyle info;

			private static Color selectedColor = new Color (0.3f,
														0.5f,
														0.85f);
			static Styles ()
			{
				label = new GUIStyle (EditorStyles.label);
				selectedLabel = new GUIStyle (EditorStyles.label);
				selectedLabel.active.textColor = selectedLabel.normal.textColor = selectedLabel.onActive.textColor = selectedColor;

				foldout = new GUIStyle (EditorStyles.foldout);
				selectedFoldout = new GUIStyle (EditorStyles.foldout);
				selectedFoldout.onFocused.textColor = selectedFoldout.focused.textColor =
				selectedFoldout.onActive.textColor = selectedFoldout.active.textColor =
				selectedFoldout.onNormal.textColor = selectedFoldout.normal.textColor = selectedColor;

				info = new GUIStyle (EditorStyles.wordWrappedLabel);
				info.wordWrap = true;
				info.stretchHeight = true;

				info.onFocused.textColor = info.focused.textColor =
				info.onActive.textColor = info.active.textColor =
				info.onNormal.textColor = info.normal.textColor;
			}
		}

		[SerializeField] private List<string> foldMarkers = new List<string> ();
		[SerializeField] private List<UnitTestResult> selectedTests = new List<UnitTestResult>();
		[SerializeField] private List<string> selectedGroups = new List<string>();
		[SerializeField] private List<string> hiddenGroups = new List<string>();
		[SerializeField] private bool showHidden;
		[SerializeField] public bool notifyOnSlowRun;
		[SerializeField] public float slowTestThreshold;
		[SerializeField] public string filterString;

		private Action<string[]> RunTest;
		private bool forceRefresh;
		//custom indents is used because of incosistency 
		private int indentLevel;

		#region GUI Contents
		private GUIContent guiRunSelected = new GUIContent ("Run Selected");
		private GUIContent guiRun = new GUIContent ("Run");
		private GUIContent guiOpenInEditor = new GUIContent ("Open in editor");
		private GUIContent guiHideGroup = new GUIContent("Hide group");
		private GUIContent guiUnhideGroup = new GUIContent ("Unhide group");
		private GUIContent guiStopWatch = new GUIContent(Icons.stopwatchImg, "Test run too long");
		#endregion
		
		public GroupedByHierarchyRenderer ()
		{
			filterString = "";
		}
		public GroupedByHierarchyRenderer (Type filter)
		{
			filterString = filter.FullName;
		}

		public bool RenderTests(IEnumerable<UnitTestResult> tests, Action<string[]> runTests)
		{
			forceRefresh = false;
			RunTest = runTests;
			DrawNamespaceGroups (tests);
			return forceRefresh;
		}

		public void RenderInfo ()
		{
			string printedName = null;
			if (selectedTests.Count == 1)
			{
				printedName = "";
				if(!string.IsNullOrEmpty(selectedTests.Single ().Message))
					printedName += selectedTests.Single().Message.Trim();
				if (!string.IsNullOrEmpty(selectedTests.Single().StackTrace))
				{
					var stackTrace = StackTraceFilter.Filter (selectedTests.Single ().StackTrace).Trim ();
					printedName += "\n\n---EXCEPTION---\n" + stackTrace;
				}
				printedName = printedName.Trim();
			}

			EditorGUILayout.SelectableLabel (
										printedName, Styles.info);
		}

		public void RenderOptions ()
		{
			if(hiddenGroups.Any())
				showHidden = EditorGUILayout.Toggle("Show hidden test groups", showHidden);
		}

		public string[] GetSelectedTests ()
		{
			return selectedGroups.Concat(selectedTests.Select(t => t.Test.FullName)).ToArray();
		}

		private void DrawNamespaceGroups (IEnumerable<UnitTestResult> tests)
		{
			foreach (var classWithTests in tests.GroupBy(s => s.Test.Namespace).OrderBy(s => s.Key))
			{
				var key = classWithTests.Key;
				if (key != "" && PrintFoldoutAndCheckIfCollapsed(key, key, classWithTests)) continue;

				if (key != "")
					indentLevel++;
				DrawClassGroups(classWithTests.Key, classWithTests);
				if (key != "")
					indentLevel--;
			}
		}

		private void DrawClassGroups(string group, IEnumerable<UnitTestResult> classWithTests)
		{
			foreach (var testGroups in classWithTests.GroupBy(s => s.Test.ClassName).OrderBy(s => s.Key))
			{
				string classGroup = (string.IsNullOrEmpty(group) ? "" : group + ".") + testGroups.Key;

				if (PrintFoldoutAndCheckIfCollapsed(classGroup, testGroups.Key,
													testGroups)) continue;

				indentLevel++;
				DrawMethod(classGroup, testGroups);
				indentLevel--;
			}
		}

		private void DrawMethod (string group, IEnumerable<UnitTestResult> testGroups)
		{
			foreach(var testGroup in testGroups.GroupBy (s => s.Test.MethodName).OrderBy ( s => s.Key ))
			{
				var methodGroup = group + '.' + testGroup.Key;
				if (testGroup.Count() > 1)
				{
					DrawTestGroup(methodGroup, testGroup);
				}
				else
				{
					var test = testGroup.Single ();
					PrintTest(test.Test.MethodName, test);
				}
			}
		}

		private void DrawTestGroup (string group, IGrouping<string, UnitTestResult> unitTestResult)
		{
			if (PrintFoldoutAndCheckIfCollapsed(group, unitTestResult.Key, unitTestResult)) return;

			//puting this to the loop fails to order elements
			var tests = unitTestResult.ToArray().OrderBy( t => t.Test.MethodName );
			indentLevel++;
			foreach (var testResult in tests)
			{
				var testName = testResult.Test.MethodName + '(' + testResult.Test.ParamName + ')';
				PrintTest(testName, testResult);
			}
			indentLevel--;
		}

		private bool PrintFoldoutAndCheckIfCollapsed (string fullName, string displayName, IEnumerable<UnitTestResult> unitTestResult)
		{
			var resultState = TestResultState.NotRunnable;
			if (unitTestResult.Any(t => t.ResultState == TestResultState.Failure || t.ResultState == TestResultState.Error))
				resultState = TestResultState.Failure;
			else if (unitTestResult.Any (t => t.ResultState == TestResultState.Success))
				resultState = TestResultState.Success;
			else if( unitTestResult.All (t=>t.IsIgnored))
				resultState = TestResultState.Ignored;

			var isClassFolded = PrintFoldout (fullName,
										displayName, resultState);
			return isClassFolded;

		}

		private bool PrintFoldout (string fullName, string displayName, TestResultState resultState)
		{
			if(hiddenGroups.Contains (fullName))
			{
				if (showHidden)
					displayName += " (hidden)";
				else
					return true;
			}

			EditorGUIUtility.SetIconSize(new Vector2(16,16));
			GUILayout.BeginHorizontal (GUILayout.Height (18));
			Indent ();
			var foldoutGUIContent = new GUIContent(displayName, GuiHelper.GetIconForCategoryResult(resultState), fullName);

			var style = IsGroupSelected (fullName) ? Styles.selectedFoldout : Styles.foldout;
			var rect = GUILayoutUtility.GetRect (foldoutGUIContent, style, GUILayout.MaxHeight (16));

			if (rect.Contains (Event.current.mousePosition))
			{
				if (Event.current.type == EventType.ContextClick)
				{
					PrintFoldoutContextMenu (fullName);
				}
				if (Event.current.type == EventType.mouseDown && Event.current.button == 0 && Event.current.control)
				{
					SelectGroup (fullName);
					Event.current.Use ();
				}
			}

			bool isClassFolded = foldMarkers.Contains (fullName);
			EditorGUI.BeginChangeCheck ();
			isClassFolded = !EditorGUI.Foldout (rect,
												!isClassFolded,
												foldoutGUIContent, true, style);
			if (EditorGUI.EndChangeCheck ())
			{
				if (isClassFolded)
					foldMarkers.Add (fullName);
				else
					foldMarkers.RemoveAll (s => s == fullName);
			}

			GUILayout.EndHorizontal ();
			EditorGUIUtility.SetIconSize(Vector2.zero);

			return isClassFolded;
		}

		private void PrintTest(string printedName, UnitTestResult test)
		{
			EditorGUIUtility.SetIconSize(new Vector2(16, 16));
			GUILayout.BeginHorizontal (GUILayout.Height (18));
			
			var foldoutGUIContent = new GUIContent (printedName,
													test.Executed ? GuiHelper.GetIconForTestResult (test.ResultState) : Icons.unknownImg,
													test.Test.FullName);

			Indent ();
			GUILayout.Space (10);
			var rect = GUILayoutUtility.GetRect(foldoutGUIContent,
												EditorStyles.label, GUILayout.ExpandWidth (true)/*, GUILayout.MaxHeight (18)*/);

			if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseDown && Event.current.button == 0)
			{
				SelectTest (test);
				GUI.FocusControl("");
				if (Event.current.clickCount == 2 && selectedTests.Count == 1)
				{
					OpenFailedTestOnError ();
				}
			}
			else if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick)
			{
				Event.current.Use();
				PrintTestContextMenu(test);
			}

			var style = IsTestSelected (test) ? Styles.selectedLabel : Styles.label;
			EditorGUI.LabelField(rect, foldoutGUIContent, style);

			if (notifyOnSlowRun && test.Duration > slowTestThreshold)
			{
				var labelRect = style.CalcSize (foldoutGUIContent);
				var iconRect = new Rect (rect.xMin + labelRect.x, rect.yMin, 20, 20);
				GUI.Label(iconRect, guiStopWatch);
			}
			
			GUILayout.EndHorizontal();
			EditorGUIUtility.SetIconSize(Vector2.zero);
		}

		private void Indent ()
		{
			GUILayout.Space (12 * indentLevel + 5 );
		}

		private bool IsTestSelected (UnitTestResult test)
		{
			return selectedTests.Contains (test);
		}

		private bool IsGroupSelected(string fullName)
		{
			return selectedGroups.Contains(fullName);
		}
		
		private void SelectTest (UnitTestResult test)
		{
			if (!Event.current.control)
			{
				selectedTests.Clear ();
				selectedGroups.Clear ();
			}
			if (Event.current.control && IsTestSelected(test))
				selectedTests.Remove (test);
			else
				selectedTests.Add (test);
			forceRefresh = true;
		}

		private void SelectGroup(string fullName)
		{
			if (!Event.current.control)
			{
				selectedGroups.Clear();
				selectedTests.Clear();
			}
			if (Event.current.control && IsGroupSelected(fullName))
				selectedGroups.Remove(fullName);
			else
				selectedGroups.Add(fullName);
			forceRefresh = true;
		}

		private void PrintFoldoutContextMenu(string path)
		{
			var m = new GenericMenu();
			if ((selectedTests.Count() + selectedGroups.Count()) > 1)
			{
				m.AddItem(guiRunSelected,
							false,
							data => RunTest(selectedTests.Select (t=>t.Test.FullName)
								.Concat (selectedGroups)
								.ToArray()),
							"");
			}
			if (!string.IsNullOrEmpty(path))
			{
				m.AddItem(guiRun,
							false,
							data => RunTest(new[] { path }),
							"");
			}
			m.AddSeparator("");
			if (hiddenGroups.Contains (path))
			{
				m.AddItem (guiUnhideGroup,
							false,
							data => hiddenGroups.Remove (path),
							"");
			}
			else
			{
				m.AddItem(guiHideGroup,
							false,
							data => hiddenGroups.Add(path),
							"");
			}
			m.ShowAsContext();
		}

		private void PrintTestContextMenu(UnitTestResult test)
		{
			var m = new GenericMenu ();
			if ((selectedTests.Count() + selectedGroups.Count()) > 1)
			{
				m.AddItem (guiRunSelected,
							false,
							data => RunTest (selectedTests.Select (t=>t.Test.FullName)
								.Concat(selectedGroups)
								.ToArray()),
							"");
			}
			if (!string.IsNullOrEmpty(test.Test.FullName))
			{
				m.AddItem (guiRun,
							false,
							data => RunTest(new[] { test.Test.FullName }),
							"");
			}
			m.AddSeparator("");

			m.AddItem(guiOpenInEditor,
						false,
						data => GuiHelper.OpenInEditor (test, false),
						"");
				
			m.ShowAsContext();
		}
		private void OpenFailedTestOnError()
		{
			var test = selectedTests.Single ();
			GuiHelper.OpenInEditor(test, true);
		}
	}
}
