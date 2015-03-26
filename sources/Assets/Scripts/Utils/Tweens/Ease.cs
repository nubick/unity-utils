using System;
using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class Ease
    {
	    public static Func<float, float, float, float> Linear = Mathf.Lerp;
	    public static Func<float, float, float, float> OutBack = OutBackFunc;
		public static Func<float, float, float, float> InBack = InBackFunc;

        private static float OutBackFunc(float start, float end, float time)
        {
            const float s = 1.70158f;
            end -= start;
	        time = time - 1f;
            return end * (time * time * ((s + 1f) * time + s) + 1f) + start;
        }

		private static float InBackFunc(float start, float end, float time)
        {
            end -= start;
            time /= 1f;
            const float s = 1.70158f;
            return end * time * time * ((s + 1f) * time - s) + start;
        }
    }
}
