using UnityEngine;

namespace Assets.Scripts.Utils.UI
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class ButtonClickScale : MonoBehaviourBase
    {
        private Vector3 _originalScale;
        private bool _isScaled;
        public float Scale = 0.95f;

        public void OnMouseDown()
        {
            _originalScale = Transform.localScale;
            Transform.localScale = _originalScale * Scale;
            _isScaled = true;
        }

        public void OnMouseUpAsButton()
        {
            ResetScale();
        }

        public void OnMouseExit()
        {
            ResetScale();
        }

        public void OnDisable()
        {
            ResetScale();
        }

        private void ResetScale()
        {
            if (_isScaled)
                Transform.localScale = _originalScale;
        }
    }
}
