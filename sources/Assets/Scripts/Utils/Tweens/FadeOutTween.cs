using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class FadeOutTween : FadeTweenBase
    {
        public static void Run(GameObject item, float duration)
        {
            Create<FadeOutTween>(item).Run(duration);
        }

        public static void Run(GameObject item, float duration, float delay)
        {
            Create<FadeOutTween>(item).Run(duration, delay);
        }

        protected override void UpdateValue(float time)
        {
            UpdateAlpha(Mathf.Lerp(1f, 0f, time));
        }
    }
}
