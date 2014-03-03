using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTest
{
	public class GroupByTestsRenderer : AssertionListRenderer<GameObject>
	{
		protected override IEnumerable<IGrouping<GameObject, AssertionComponent>> GroupResult (IEnumerable<AssertionComponent> assertionComponents)
		{
			return assertionComponents.GroupBy (c =>
			{
				var topGO = TestManager.FindTopGameObject (c.gameObject);
				if (topGO.GetComponents<TestComponent> () != null)
					return topGO;
				return topGO;
			});
		}

		protected override string GetFoldoutDisplayName (GameObject key)
		{
			return key.name;
		}
	}
}
