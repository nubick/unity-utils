using UnityEngine;

namespace Assets.Scripts.Utils.Views
{
	public class ViewsController : MonoBehaviour
	{
		public ViewBase StartView;

		public void Start()
		{
			foreach (ViewBase viewBase in GetComponentsInChildren<ViewBase>())
				viewBase.Content.SetActive(false);

			StartView.Show();
		}
	}
}
