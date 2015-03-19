using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class FadeOutTween : FadeTweenBase
    {
		public static FadeOutTween Run(GameObject item, float duration)
		{
			FadeOutTween tween = Create<FadeOutTween>(item, duration);
			tween.CacheRenderers();
			return tween;
		}

        protected override void UpdateValue(float time)
        {
			UpdateAlpha(EaseFunc(1f, 0f, time));
        }
    }
}
