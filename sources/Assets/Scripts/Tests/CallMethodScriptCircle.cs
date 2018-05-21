using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Tests
{
	public class CallMethodScriptCircle : MonoBehaviour
	{
		public void Rotate(float seconds, float speed)
		{
			StartCoroutine(RotateCoroutine(seconds, speed));
		}

		private IEnumerator RotateCoroutine(float seconds, float speed)
		{
			float finishTime = Time.time + seconds;

			while (Time.time < finishTime)
			{
				transform.Rotate(0f, 0f, Time.deltaTime * speed);
				yield return null;
			}
		}
	}
}