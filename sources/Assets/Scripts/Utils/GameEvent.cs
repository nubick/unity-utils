using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utils
{
	public class GameEvent<TEventArgs> where TEventArgs : GameEventArgs
	{
		private readonly List<Action<TEventArgs>> _callbacks;

		public GameEvent()
		{
			_callbacks = new List<Action<TEventArgs>>();
		}

		public void Subscribe(Action<TEventArgs> callback)
		{
			_callbacks.Add(callback);
		}

		public void Publish(TEventArgs eventArgs)
		{
			foreach (Action<TEventArgs> callback in _callbacks)
				callback(eventArgs);
		}
	}
}
