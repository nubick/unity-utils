using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class ColorFlashTween : TweenBase
    {
        private SpriteRenderer _spriteRenderer;
        private Color _startColor;
        private Color _targetColor;

        public static ColorFlashTween Run(GameObject item, Color color, float duration)
        {
            ColorFlashTween tween = Create<ColorFlashTween>(item, duration);
            tween._targetColor = color;
            return tween;
        }

        protected override void OnStart()
        {
            _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            _startColor = _spriteRenderer.color;
        }

        protected override void UpdateValue(float time)
        {
            time = EaseFunc(0f, 1f, time);
            if (time <= 0.5f)
                _spriteRenderer.color = Color.Lerp(_startColor, _targetColor, time*2);
            else
                _spriteRenderer.color = Color.Lerp(_targetColor, _startColor, (time - 0.5f)*2);
        }
    }
}
