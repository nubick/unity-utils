using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils.Tweens
{
    public class ColorFlashTween : TweenBase
    {
        private SpriteRenderer _spriteRenderer;
        private Image _image;
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
            if(_spriteRenderer != null)
                _startColor = _spriteRenderer.color;

            _image = gameObject.GetComponent<Image>();
            if (_image != null)
                _startColor = _image.color;
        }

        protected override void UpdateValue(float time)
        {
            time = EaseFunc(0f, 1f, time);
            Color color = time <= 0.5f
                ? Color.Lerp(_startColor, _targetColor, time*2)
                : Color.Lerp(_targetColor, _startColor, (time - 0.5f)*2);

            if (_spriteRenderer != null)
                _spriteRenderer.color = color;

            if (_image != null)
                _image.color = color;
        }
    }
}
