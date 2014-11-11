using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class FadeInTween : FadeTweenBase
    {
        public static void Run(GameObject item, float duration)
        {
            Create<FadeInTween>(item).Run(duration);
        }

        public static void Run(GameObject item, float duration, float delay)
        {
            Create<FadeInTween>(item).Run(duration, delay);
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
