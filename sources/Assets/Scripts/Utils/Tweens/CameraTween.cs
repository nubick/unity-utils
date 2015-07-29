using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class CameraTween : TweenBase
    {
        private Camera _camera;
        private float _startSize;
        private float _targetSize;

        public static CameraTween ChangeSize(Camera camera, float size, float duration)
        {
            CameraTween tween = Create<CameraTween>(camera.gameObject, duration);
            tween._camera = camera;
            tween._startSize = camera.orthographicSize;
            tween._targetSize = size;
            return tween;
        }

        protected override void UpdateValue(float time)
        {
            _camera.orthographicSize = EaseFunc(_startSize, _targetSize, time);
        }
    }
}
