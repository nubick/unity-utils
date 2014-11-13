using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class FadeOutTween : FadeTweenBase
    {
        public static FadeOutTween Run(GameObject item, float duration)
        {
            return Create<FadeOutTween>(item, duration);
        }

        protected override void UpdateValue(float time)
        {
            UpdateAlpha(Mathf.Lerp(1f, 0f, time));
        }
    }
}
