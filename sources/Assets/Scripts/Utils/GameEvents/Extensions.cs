using System;
using UnityEngine;

namespace Assets.Scripts.Utils.GameEvents
{
	public static class Extensions
	{
		public static void Subscribe(this MonoBehaviour monoBehaviour, GameEvent gameEvent, Action action)
		{
			EventsListener eventsListener = monoBehaviour.GetComponent<EventsListener>() ??
											monoBehaviour.gameObject.AddComponent<EventsListener>();
			eventsListener.Subscribe(gameEvent, action);
		}

		public static void Subscribe<TEventArgs>(this MonoBehaviour monoBehaviour, GameEvent<TEventArgs> gameEvent, Action<TEventArgs> action)
		{
			EventsListener eventsListener = monoBehaviour.GetComponent<EventsListener>() ??
											monoBehaviour.gameObject.AddComponent<EventsListener>();
			eventsListener.Subscribe(gameEvent, action);
		}

		public static void SubscribeWithDelay(this MonoBehaviour monoBehaviour, GameEvent gameEvent, Action action, float delay)
		{
			EventsListener eventsListener = monoBehaviour.GetComponent<EventsListener>() ??
											monoBehaviour.gameObject.AddComponent<EventsListener>();
			eventsListener.SubscribeWithDelay(gameEvent, action, delay);
		}

		public static void SubscribeWithDelay<TEventArgs>(this MonoBehaviour monoBehaviour, GameEvent<TEventArgs> gameEvent, Action<TEventArgs> action, float delay)
		{
			EventsListener eventsListener = monoBehaviour.GetComponent<EventsListener>() ??
											monoBehaviour.gameObject.AddComponent<EventsListener>();
			eventsListener.SubscribeWithDelay(gameEvent, action, delay);
		}
	}
}
