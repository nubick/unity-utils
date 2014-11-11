using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.Utils.Tweens
{
    public abstract class FadeTweenBase : TweenBase
    {
        private SpriteRenderer _spriteRenderer;
        private TextMesh _textMesh;
        private Image _image;
        private Text _text;

        protected override void OnStart()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _textMesh = GetComponent<TextMesh>();
            _image = GetComponent<Image>();
            _text = GetComponent<Text>();
        }

        protected void UpdateAlpha(float alpha)
        {
            if (_spriteRenderer != null)
                _spriteRenderer.color = _spriteRenderer.color.SetA(alpha);

            if (_textMesh != null)
                _textMesh.color = _textMesh.color.SetA(alpha);

            if (_image != null)
                _image.color = _image.color.SetA(alpha);

            if (_text != null)
                _text.color = _text.color.SetA(alpha);
        }
    }
}
