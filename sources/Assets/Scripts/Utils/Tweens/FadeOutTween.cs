using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class FadeOutTween : MonoBehaviourBase
    {
        private float _starTime;
        private float _duration;
        private SpriteRenderer _spriteRenderer;
        private TextMesh _textMesh;

        public static void Run(GameObject item, float duration)
        {
            FadeOutTween tween = item.GetComponent<FadeOutTween>();
            if (tween == null)
                tween = item.AddComponent<FadeOutTween>();
            else
                tween.StopAllCoroutines();
            tween.Run(duration);
        }

        private void Run(float duration)
        {
            _duration = duration;
            _starTime = Time.time;
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _textMesh = GetComponent<TextMesh>();
        }

        public void Update()
        {
            float normalizedTime = (Time.time - _starTime) / _duration;
            if (normalizedTime > 1f)
            {
                gameObject.SetActive(false);
                Destroy(this);
                UpdateAlpha(1f);
            }
            else
            {
                UpdateAlpha(Mathf.Lerp(1f, 0f, normalizedTime));
            }
        }

        private void UpdateAlpha(float alpha)
        {
            if (_spriteRenderer != null)
                _spriteRenderer.color = _spriteRenderer.color.SetA(alpha);
            if (_textMesh != null)
                _textMesh.color = _textMesh.color.SetA(alpha);
        }

    }


}
