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
			UpdateEase(0);
		}

		public void OnDrawGizmos()
		{
			if (EaseFunc == null)
				UpdateEase(0);

			Gizmos.color = Color.blue;
			Gizmos.DrawCube(
				new Vector2((ToTime + FromTime)/2, (ToValue + FromValue)/2),
				new Vector2(Mathf.Abs(FromTime - ToTime), Mathf.Abs(FromValue - ToValue)));

			Gizmos.color = Color.white;
			Vector2 previousPoint = new Vector2(FromTime, FromValue);
			for (float time = 0.0f; time <= 1.0f; time += 0.005f)
			{
				float value = EaseFunc(FromValue, ToValue, time);
				Vector2 currentPoint = new Vector2(time*(ToTime - FromTime) + FromTime, value);
				Gizmos.DrawLine(previousPoint, currentPoint);
				previousPoint = currentPoint;
			}
		}

		public void UpdateEase(int easeInt)
		{
			EaseFunc = EaseFunctions.Get((Ease)easeInt);
		}
	}
}
