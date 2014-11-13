using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class FadeInTween : FadeTweenBase
    {
        public static FadeInTween Run(GameObject item, float duration)
        {
            return Create<FadeInTween>(item, duration);
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
