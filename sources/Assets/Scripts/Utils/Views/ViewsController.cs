using UnityEngine;

namespace Assets.Scripts.Utils.Views
{
	public class ViewsController : MonoBehaviour
	{
		private ViewBase[] _views;

		public ViewBase StartView;

		public void Start()
		{
			_views = GetComponentsInChildren<ViewBase>();
			foreach (ViewBase viewBase in _views)
				viewBase.Content.SetActive(false);

			StartView.Show();
		}

#if UNITY_ANDROID || UNITY_EDITOR
		public void Update()
		{
			if (Input.GetKeyUp(KeyCode.Escape))
			{
				foreach (ViewBase view in _views)
				{
					if (view.IsActive)
					{
						view.OnBackKey();
						break;
					}
				}
			}
		}
#endif
	}
}
