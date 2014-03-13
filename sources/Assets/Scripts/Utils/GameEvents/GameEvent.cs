using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utils.GameEvents
{
	public class GameEvent<TEventArgs> where TEventArgs : GameEventArgs
	{
	    private readonly List<GameEventCallback<TEventArgs>> _callbacks; 

		public GameEvent()
		{
            _callbacks = new List<GameEventCallback<TEventArgs>>();
		}

		public void Subscribe(int subscriberId, Action<TEventArgs> callback)
		{
		    _callbacks.Add(new GameEventCallback<TEventArgs>(subscriberId, callback));
		}

	    public void Publish(TEventArgs eventArgs)
	    {
	        foreach (GameEventCallback<TEventArgs> callback in _callbacks)
	            callback.Callback(eventArgs);
	    }

        public void UnSubscribe(int subscriberId)
        {
            foreach(GameEventCallback<TEventArgs> callback in _callbacks.ToArray())
                if (callback.SubscriberId == subscriberId)
                    _callbacks.Remove(callback);
	    }
    }
}
