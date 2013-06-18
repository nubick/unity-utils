using UnityEngine;

namespace Assets.Scripts.Utils
{
	public class TimeEvent
	{
		private float _duration;
		private float _lastTimeOccured;
		private bool _isActive;

		public float LifeTime{ get { return Time.time - _lastTimeOccured; } }
		public float LifeTimePercent{ get { return Mathf.Clamp(LifeTime*100/_duration, 0, 100); } }

		private TimeEvent()
		{
			_isActive = false;
		}

		public TimeEvent(float duration)
		{
			Init(duration);
		}

		private void Init(float duration)
		{
			_isActive = true;
			_duration = duration;
			_lastTimeOccured = Time.time;			
		}

		public static TimeEvent GetDeferred()
		{
			return new TimeEvent();
		}

		public bool PopIsOccurred(bool restart = true)
		{
			bool isOccurred = IsOccured();
			if (isOccurred)
			{
				if (restart)
					_lastTimeOccured = Time.time;
				else
					_isActive = false;
			}
			return isOccurred;
		}

		public bool IsOccured()
		{
			return _isActive && _lastTimeOccured + _duration <= Time.time;
		}

		public void Start(float duration)
		{
			Init(duration);
		}

		public float GetProgress()
		{
			return (Time.time - _lastTimeOccured)/_duration;
		}
	}
}
