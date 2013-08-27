using Assets.Scripts.Utils;
using UnityEngine;

namespace Assets.Scripts.Tests
{
	public class TimeEventTest : MonoBehaviourBase
	{
		private bool _isRunning;
		private TimeEvent _timeEvent;

		public void Update () 
		{
			if (_isRunning)
			{
				if (_timeEvent.IsOccurred())
				{
					//Write text passed event
				}
			}
		}

		public void OnGUI()
		{
			if (GUI.Button(new Rect(0, 0, 100, 80),  "Test 'TimeEvent'"))
			{
				_timeEvent = new TimeEvent(3.0f);
			}
		}
	}
}
