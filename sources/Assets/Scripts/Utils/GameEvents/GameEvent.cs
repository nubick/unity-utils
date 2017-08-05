using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utils.GameEvents
{
	public class GameEvent
	{
		private readonly List<int> _subscriberIds = new List<int>();
		private readonly List<Action> _callbacks = new List<Action>();

		public void Subscribe(int subscriberId, Action callback)
		{
			_subscriberIds.Add(subscriberId);
			_callbacks.Add(callback);
		}

		public void Publish()
		{
			for (int i = 0; i < _callbacks.Count; i++)
				_callbacks[i]();
		}

		public void UnSubscribe(int subscriberId)
		{
			for (int i = 0; i < _callbacks.Count; i++)
			{
				if (_subscriberIds[i] == subscriberId)
				{
					_callbacks.RemoveAt(i);
					_subscriberIds.RemoveAt(i);
					i--;
				}
			}
		}
	}

	public class GameEvent<TEventArgs>
	{
        private readonly List<int> _subscriberIds = new List<int>();
        private readonly List<Action<TEventArgs>> _callbacks = new List<Action<TEventArgs>>();

		public void Subscribe(int subscriberId, Action<TEventArgs> callback)
		{
            _subscriberIds.Add(subscriberId);
            _callbacks.Add(callback);
		}

        public void Publish(TEventArgs eventArgs)
        {
            for (int i = 0; i < _callbacks.Count; i++)
                _callbacks[i](eventArgs);
        }

		public void UnSubscribe(int subscriberId)
		{
            for (int i = 0; i < _callbacks.Count; i++)
            {
                if(_subscriberIds[i] == subscriberId)
                {
                    _callbacks.RemoveAt(i);
                    _subscriberIds.RemoveAt(i);
                    i--;
                }
            }
		}
	}
}
