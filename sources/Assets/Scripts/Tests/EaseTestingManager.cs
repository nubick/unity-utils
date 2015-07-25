using System;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Tweens;
using UnityEngine;

namespace Assets.Scripts.Tests
{
	public class EaseTestingManager : MonoBehaviourBase
	{
		public Func<float, float, float, float> EaseFunc;
		public float FromTime;
		public float ToTime;
		public float FromValue;
		public float ToValue;

		public void Awake()
		{
			EaseFunc = Ease.Linear;
		}

		public void OnDrawGizmos()
		{
			if (EaseFunc == null)
				EaseFunc = Ease.Linear;

			Gizmos.color = Color.blue;
			Gizmos.DrawCube(
				new Vector2((ToTime + FromTime)/2, (ToValue + FromValue)/2),
				new Vector2(Mathf.Abs(FromTime - ToTime), Mathf.Abs(FromValue - ToValue)));

			Gizmos.color = Color.white;
			Vector2 previousPoint = new Vector2(FromTime, FromValue);
			for (float time = 0.0f; time <= 1.0f; time += 0.01f)
			{
				float value = EaseFunc(FromValue, ToValue, time);
				Vector2 currentPoint = new Vector2(time*(ToTime - FromTime) + FromTime, value);
				Gizmos.DrawLine(previousPoint, currentPoint);
				previousPoint = currentPoint;
			}
		}

		public void SelectLinear()
		{
			EaseFunc = Ease.Linear;
		}

		
		public void SelectInBack()
		{
			EaseFunc = Ease.InBack;
		}

		public void SelectOutBack()
		{
			EaseFunc = Ease.OutBack;
		}

		public void SelectInOutBack()
		{
			EaseFunc = Ease.InOutBack;
		}



		public void SelectOutCirc()
		{
			EaseFunc = Ease.OutCirc;
		}

		public void SelectInCirc()
		{
			EaseFunc = Ease.InCirc;
		}

		public void SelectInOutCirc()
		{
			EaseFunc = Ease.InOutCirc;
		}


		public void SelectOutCubic()
		{
			EaseFunc = Ease.OutCubic;
		}

		public void SelectInCubic()
		{
			EaseFunc = Ease.InCubic;
		}

		public void SelectInOutCubic()
		{
			EaseFunc = Ease.InOutCubic;
		}
	}
}
