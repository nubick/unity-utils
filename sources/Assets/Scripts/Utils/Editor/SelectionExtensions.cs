using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Utils.Editor
{
	public static class SelectionExtensions
	{
		[MenuItem("Window/Utils/Selection/Select child 1 &#1")]
		public static void SelectChildsAt1()
		{
			SelectChilds(0);
		}

		[MenuItem("Window/Utils/Selection/Select child 2 &#2")]
		public static void SelectChildsAt2()
		{
			SelectChilds(1);
		}

		[MenuItem("Window/Utils/Selection/Select child 3 &#3")]
		public static void SelectChildsAt3()
		{
			SelectChilds(2);
		}

		[MenuItem("Window/Utils/Selection/Select child 4 &#4")]
		public static void SelectChildsAt4()
		{
			SelectChilds(3);
		}

		[MenuItem("Window/Utils/Selection/Select child 5 &#5")]
		public static void SelectChildsAt5()
		{
			SelectChilds(4);
		}

		[MenuItem("Window/Utils/Selection/Select ALL child &#0")]
		public static void SelectAllChilds()
		{
			List<GameObject> childs = new List<GameObject>();
			foreach (Transform tr in Selection.transforms)
				for (int i = 0; i < tr.childCount; i++)
					childs.Add(tr.GetChild(i).gameObject);
			Selection.objects = childs.ToArray();
		}

		private static void SelectChilds(int index)
		{
			List<GameObject> childs = new List<GameObject>();
			foreach (Transform tr in Selection.transforms)
				if (tr.childCount - 1 >= index)
					childs.Add(tr.GetChild(index).gameObject);
			Selection.objects = childs.ToArray();
		}
	}
}