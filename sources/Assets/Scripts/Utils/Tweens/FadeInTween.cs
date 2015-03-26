using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class FadeInTween : FadeTweenBase
    {
		public static FadeInTween Run(GameObject item, float duration)
		{
			FadeInTween fadeInTween = Create<FadeInTween>(item, duration);
			fadeInTween.Initialize();
			fadeInTween.UpdateValue(0);
			item.SetActive(true);
			return fadeInTween;
		}

        protected override void UpdateValue(float time)
        {
            UpdateAlpha(EaseFunc(0f, 1f, time));
        }
    }
}
