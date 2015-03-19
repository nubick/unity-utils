using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class FadeInTween : FadeTweenBase
    {
		public static FadeInTween Run(GameObject item, float duration)
		{
			FadeInTween tween = Create<FadeInTween>(item, duration);
			tween.CacheRenderers();
			tween.UpdateValue(0);
			item.SetActive(true);
			return tween;
		}

        protected override void OnStart()
        {
            base.OnStart();
            gameObject.SetActive(true);
        }

        protected override void UpdateValue(float time)
        {
            UpdateAlpha(EaseFunc(0f, 1f, time));
        }
    }
}
