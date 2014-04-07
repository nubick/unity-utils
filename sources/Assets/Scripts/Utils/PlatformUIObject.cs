using UnityEngine;

namespace Assets.Scripts.Utils
{
    public class PlatformUIObject : MonoBehaviour
    {
        public RuntimePlatform Platform;
        public bool IsForceVisible;

        public void OnEnable()
        {
            UpdateVisibility();
        }

        private void UpdateVisibility()
        {
            gameObject.SetActive(IsForceVisible || Application.platform == Platform);
        }
    }

}
