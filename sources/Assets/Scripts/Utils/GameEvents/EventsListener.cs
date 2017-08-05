using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts.Utils.GameEvents
{
	public class EventsListener : MonoBehaviour
	{
		private readonly int _subscriberId;
		private static int _counter;

		public EventsListener()
		{
			_subscriberId = ++_counter;
		}

		public void Subscribe(GameEvent gameEvent, Action callback)
		{
			gameEvent.Subscribe(_subscriberId, callback);
		}

		public void Subscribe<TEventArgs>(GameEvent<TEventArgs> gameEvent, Action<TEventArgs> callback)			
		{
			gameEvent.Subscribe(_subscriberId, callback);
		}

		public void SubscribeWithDelay(GameEvent gameEvent, Action callback, float delay)
		{
            gameEvent.Subscribe(_subscriberId, () =>
            {
                StartCoroutine(ExecuteAfterDelay(callback, delay));
            });
		}

		public void SubscribeWithDelay<TEventArgs>(GameEvent<TEventArgs> gameEvent, Action<TEventArgs> callback, float delay)			
		{
			gameEvent.Subscribe(_subscriberId, eventArgs =>
			{
				StartCoroutine(ExecuteAfterDelay(callback, eventArgs, delay));
			});
		}

		private IEnumerator ExecuteAfterDelay(Action callback, float delay)
		{
			yield return new WaitForSeconds(delay);
			callback();
		}

		private IEnumerator ExecuteAfterDelay<TEventArgs>(Action<TEventArgs> callback, TEventArgs eventArgs, float delay)			
		{
			yield return new WaitForSeconds(delay);
			callback(eventArgs);
		}

		protected void UnSubscribe<TEventArgs>(GameEvent<TEventArgs> gameEvent)
		{
			gameEvent.UnSubscribe(_subscriberId);
		}

		protected void UnSubscribe(GameEvent gameEvent)
		{
			gameEvent.UnSubscribe(_subscriberId);
		}
	}

}
