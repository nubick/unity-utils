using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils.Editor
{
	public static class SelectionExtensions
	{
		[MenuItem("Window/Utils/Selection/Select children 1 &#1")]
		public static void SelectChildrenAt1()
		{
			SelectChildren(0);
		}

		[MenuItem("Window/Utils/Selection/Select children 2 &#2")]
		public static void SelectChildrenAt2()
		{
			SelectChildren(1);
		}

		[MenuItem("Window/Utils/Selection/Select children 3 &#3")]
		public static void SelectChildrenAt3()
		{
			SelectChildren(2);
		}

		[MenuItem("Window/Utils/Selection/Select children 4 &#4")]
		public static void SelectChildrenAt4()
		{
			SelectChildren(3);
		}

		[MenuItem("Window/Utils/Selection/Select children 5 &#5")]
		public static void SelectChildrenAt5()
		{
			SelectChildren(4);
		}

		[MenuItem("Window/Utils/Selection/Select ALL children &#0")]
		public static void SelectAllChildren()
		{
			List<GameObject> children = new List<GameObject>();
			foreach (Transform tr in Selection.transforms)
				for (int i = 0; i < tr.childCount; i++)
					children.Add(tr.GetChild(i).gameObject);
			Selection.objects = children.ToArray();
		}

		private static void SelectChildren(int index)
		{
			List<GameObject> children = new List<GameObject>();
			foreach (Transform tr in Selection.transforms)
				if (tr.childCount - 1 >= index)
					children.Add(tr.GetChild(index).gameObject);
			Selection.objects = children.ToArray();
		}

		[MenuItem("Window/Utils/Selection/Select ALL parents &#-")]
		public static void SelectAllParents()
		{
			List<GameObject> parents = new List<GameObject>();
			foreach(Transform tr in Selection.transforms)
				if(tr.parent != null)
					parents.Add(tr.parent.gameObject);
			Selection.objects = parents.ToArray();
		}
	}
}