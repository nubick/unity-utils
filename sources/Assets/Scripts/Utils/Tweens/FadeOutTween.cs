using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class FadeOutTween : FadeTweenBase
    {
		public static FadeOutTween Run(GameObject item, float duration)
		{
			return Create<FadeOutTween>(item, duration);
		}

	    protected override void OnStart()
	    {
		    Initialize();
			UpdateValue(0);
	    }

	    protected override void UpdateValue(float time)
        {
			UpdateAlpha(EaseFunc(1f, 0f, time));
        }

	    protected override void OnFinish()
	    {
			gameObject.SetActive(false);
			UpdateAlpha(0);
			base.OnFinish();
	    }
    }
}
