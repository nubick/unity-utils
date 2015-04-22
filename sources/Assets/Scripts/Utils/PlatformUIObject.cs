using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class PlatformUIObject : MonoBehaviour
    {
		public List<RuntimePlatform> Platforms;
		public bool IsForceVisible;

		public void OnEnable()
		{
			UpdateVisibility();
		}

		private void UpdateVisibility()
		{
			gameObject.SetActive(IsForceVisible || Platforms.Contains(Application.platform));
		}
    }

}
