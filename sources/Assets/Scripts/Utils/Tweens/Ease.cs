using System;
using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class Ease
    {
	    public static Func<float, float, float, float> Linear = Mathf.Lerp;

		#region Back

		private const float BackS = 1.70158f;
	    private const float BackS2 = 2.5949095f;
		public static Func<float, float, float, float> InBack = InBackFunc;
		public static Func<float, float, float, float> OutBack = OutBackFunc;
		public static Func<float, float, float, float> InOutBack = InOutBackFunc;

		private static float InBackFunc(float start, float end, float time)
		{
			return (end - start)*time*time*((BackS + 1f)*time - BackS) + start;
		}

	    private static float OutBackFunc(float start, float end, float time)
	    {
		    time -= 1f;
		    return (end - start)*(time*time*((BackS + 1f)*time + BackS) + 1f) + start;
	    }

	    private static float InOutBackFunc(float start, float end, float time)
		{
			time *= 2f;
			if (time < 1f)
				return (end - start)/2f*(time*time*((BackS2 + 1)*time - BackS2)) + start;

			time -= 2f;
			return (end - start)/2f*(time*time*((BackS2 + 1)*time + BackS2) + 2) + start;
		}

		#endregion

		#region Circ

		public static Func<float, float, float, float> OutCirc = OutCircFunc;
		public static Func<float, float, float, float> InCirc = InCircFunc;
	    public static Func<float, float, float, float> InOutCirc = InOutCircFunc;

		public static float OutCircFunc(float start, float end, float time)
		{
			time -= 1f;
			return (end - start)*Mathf.Sqrt(1f - time*time) + start;
		}

		private static float InCircFunc(float start, float end, float time)
		{
			return -(end - start)*(Mathf.Sqrt(1f - time*time) - 1f) + start;
		}

		private static float InOutCircFunc(float start, float end, float time)
		{
			time *= 2f;
			if (time < 1)
				return -(end - start)/2*(Mathf.Sqrt(1 - time*time) - 1) + start;

			time -= 2;
			return (end - start)/2*(Mathf.Sqrt(1 - time*time) + 1) + start;
		}

		#endregion


		
    }
}
