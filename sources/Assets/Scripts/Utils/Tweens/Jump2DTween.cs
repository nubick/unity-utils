using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class Jump2DTween : TweenBase
    {
        private Vector2 _startPosition;
        private Vector2 _targetPosition;
        private bool _isLocal;
        private float _jumpYOffset = 50f;

        private Vector2 _middlePosition;

        public static Jump2DTween Run(GameObject item, Vector2 position, float duration)
        {
            Jump2DTween tween = Create<Jump2DTween>(item, duration);
            tween._targetPosition = position;
            return tween;
        }

        public Jump2DTween SetLocal(bool isLocal)
        {
            _isLocal = isLocal;
            return this;
        }

        public Jump2DTween SetJumpYOffset(float jumpYOffset)
        {
            _jumpYOffset = jumpYOffset;
            return this;
        }

        protected override void OnStart()
        {
            _startPosition = _isLocal ? Transform.localPosition : Transform.position;
            _middlePosition = new Vector2(
                (_targetPosition.x + _startPosition.x)/2f,
                Mathf.Max(_startPosition.y, _targetPosition.y) + _jumpYOffset);
        }

        protected override void UpdateValue(float time)
        {
            Vector2 nextPosition;
            if (time <= 0.5f)
            {
                nextPosition = new Vector2(
                    Mathf.Lerp(_startPosition.x, _middlePosition.x, time*2f),
                    EaseFunctions.OutCirc(_startPosition.y, _middlePosition.y, time*2f));
            }
            else
            {
                nextPosition = new Vector2(
                    Mathf.Lerp(_middlePosition.x, _targetPosition.x, time*2f - 1f),
                    EaseFunctions.InCirc(_middlePosition.y, _targetPosition.y, time*2f - 1f));
            }

            if (_isLocal)
                Transform.localPosition = nextPosition;
            else
                Transform.position = nextPosition;
        }
    }
}
