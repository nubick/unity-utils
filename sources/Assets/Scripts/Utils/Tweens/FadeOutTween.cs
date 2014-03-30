using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class FadeOutTween : MonoBehaviourBase
    {
        private float _starTime;
        private float _duration;
        private SpriteRenderer _spriteRenderer;

        public static void Run(GameObject item, float duration)
        {
            FadeOutTween tween = item.GetComponent<FadeOutTween>();
            if (tween == null)
                tween = item.gameObject.AddComponent<FadeOutTween>();
            else
                tween.StopAllCoroutines();
            tween.Run(duration);
        }

        private void Run(float duration)
        {
            _duration = duration;
            _starTime = Time.time;
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Update()
        {
            float normalizedTime = (Time.time - _starTime) / _duration;
            if (normalizedTime > 1f)
            {
                gameObject.SetActive(false);
                Destroy(this);
                _spriteRenderer.color = _spriteRenderer.color.SetA(1f);
            }
            else
            {
                float alpha = Mathf.Lerp(1f, 0f, normalizedTime);
                _spriteRenderer.color = _spriteRenderer.color.SetA(alpha);
            }
        }
    }


}
