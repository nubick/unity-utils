using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
	[Serializable]
	public class TestManager
	{
		[SerializeField]
		private static bool reloadTestList = true;
		private static DateTime nextIvalidateTime = DateTime.Now;
	
		[SerializeField]
		private List<TestResult> testList = new List<TestResult>();

		public IList<TestResult> GetAllTestsResults ()
		{
			TryToReload ();
			return testList.ToList();
		}

		private void TryToReload ()
		{
			if (reloadTestList && nextIvalidateTime <= DateTime.Now)
			{
				var foundTestList = GetAllTestGameObjects ();

				var newTestList = new List<TestResult> ();
				foreach (var gameObject in foundTestList)
				{
					var result = testList.Find (t => t.go == gameObject);
					if (result != null)
					{
						result.name = result.go.name;
						newTestList.Add (result);
					}
					else
						newTestList.Add (new TestResult (gameObject));
				}
				testList = newTestList;

				SortTestList ();
				reloadTestList = false;
				nextIvalidateTime = DateTime.Now.AddSeconds (1);
			}
		}

		public TestResult AddTest ()
		{
			var go = new GameObject ();
			go.name = "New Test";
			go.AddComponent<TestComponent>();
			ShowTestInHierarchy (go, true);

			var testResult = new TestResult (go);
			testList.Add(testResult);
			SortTestList ();	

			return testResult;
		}

		private void SortTestList ()
		{
			testList.Sort((t1, t2) =>
			{
				var result = t1.go.name.CompareTo (t2.go.name);
				if(result == 0)
					result = t1.go.GetInstanceID ().CompareTo(t2.go.GetInstanceID ());
				return result;
			});
		}

		public void ClearTestList ()
		{
			testList.Clear ();
			InvalidateTestList ();
		}

		public void DeleteTest(List<TestResult> tests)
		{
			foreach (var test in tests)
			{
				GameObject.DestroyImmediate(test.go);
				testList.Remove (test);
			}
		}

		public static void InvalidateTestList ()
		{
			nextIvalidateTime = DateTime.Now;
			reloadTestList = true;
		}

		public TestResult GetResultFor (GameObject testInfo)
		{
			if(reloadTestList) TryToReload ();

			try{
				return testList.Single (result => result.go == testInfo);
			}catch(Exception)
			{
				InvalidateTestList();
				TryToReload();
				return testList.SingleOrDefault(result => result.go == testInfo);
			}
		}

		public void UpdateResults (List<TestResult> tests)
		{
			foreach (var testResult in tests)
			{
				var idx = testList.FindIndex (result => result.id == testResult.id);
				testList[idx] = testResult;
			}
		}

		#region Static methods

		public static void ShowOrHideTestInHierarchy (bool hideTestsInHierarchy)
		{
			foreach (var t in GetAllTestGameObjects ())
			{
				var a = t.activeInHierarchy;
				t.SetActive (true);
				ShowTestInHierarchy(t.gameObject, !hideTestsInHierarchy);
				t.SetActive (a);
			}
		}

		public static GameObject FindTopGameObject (GameObject go)
		{
			while (go.transform.parent != null)
				go = go.transform.parent.gameObject;
			return go;
		}

		public static bool AnyTestsOnScene ()
		{
			return GetAllTestGameObjects ().Any ();
		}

		public static void SelectInHierarchy (GameObject test, bool hideTestsInHierarchy)
		{
			foreach (var t in GetAllTestGameObjects ())
			{
				t.gameObject.SetActive(t == test);
				if (hideTestsInHierarchy)
					ShowTestInHierarchy(t.gameObject, t == test);
			}
		}

		public static void DisableAllTests ()
		{
			foreach (var t in GetAllTestGameObjects ())
			{
				t.gameObject.SetActive (true);
				ShowTestInHierarchy (t.gameObject, true);
				t.gameObject.SetActive (false);
			}
		}

		public static void ShowTestInHierarchy (GameObject gameObject, bool show)
		{
			if (show)
				gameObject.hideFlags &= ~HideFlags.HideInHierarchy;
			else
				gameObject.hideFlags |= HideFlags.HideInHierarchy;

			gameObject.hideFlags |= HideFlags.NotEditable;
			gameObject.transform.hideFlags |= HideFlags.HideInInspector;
			var c = gameObject.GetComponent<TestComponent>();
			if(c!=null) c.hideFlags = 0;
			EditorUtility.SetDirty(gameObject);
		}

		public static List<GameObject> GetAllTestGameObjects ()
		{
			var resultArray = Resources.FindObjectsOfTypeAll (typeof (TestComponent)) as TestComponent[];
			var foundTestList = new List<GameObject> (resultArray.Select (component => component.gameObject));
			return foundTestList;
		}

		#endregion

		public IEnumerable<TestResult> GetTestsToSelect (List<TestResult> selectedTests, TestResult testToSelect)
		{
			TestResult start = null;
			TestResult end = null;

			for (int i = testList.Count-1; i >=0 ; i--)
			{
				var testResult = testList[i];
				if (start==null)
				{
					if (testResult == testToSelect)
						start = testToSelect;
					else if (selectedTests.Contains (testResult))
						start = testResult;
				}else if(testResult == testToSelect)
				{
					end = testToSelect;
					break;
				}
				if(start!=null)
				{
					if (testResult == testToSelect)
						end = testToSelect;
					else if (selectedTests.Contains(testResult))
						end = testResult;

				}
			}
			var startIdx = testList.IndexOf (start);
			var endIdx = testList.IndexOf (end);
			return testList.GetRange(endIdx, startIdx-endIdx+1);
		}
	}
}
