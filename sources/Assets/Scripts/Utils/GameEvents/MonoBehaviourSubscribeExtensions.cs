using System;
using UnityEngine;

namespace Assets.Scripts.Utils.GameEvents
{
    public static class MonoBehaviourSubscribeExtensions
    {
        private static EventsListener CreateEventsListener(MonoBehaviour monoBehaviour)
        {
            return monoBehaviour.GetComponent<EventsListener>() ?? monoBehaviour.gameObject.AddComponent<EventsListener>();
        }

        public static void Subscribe(this MonoBehaviour monoBehaviour, GameEvent gameEvent, Action action)
        {
            EventsListener eventsListener = CreateEventsListener(monoBehaviour);
            eventsListener.Subscribe(gameEvent, action);
        }

        public static void Subscribe<TArgs>(this MonoBehaviour monoBehaviour, GameEvent<TArgs> gameEvent, Action<TArgs> action)
        {
            EventsListener eventsListener = CreateEventsListener(monoBehaviour);
            eventsListener.Subscribe(gameEvent, action);
        }

        public static void Subscribe<TArgs1, TArgs2>(this MonoBehaviour monoBehaviour, GameEvent<TArgs1, TArgs2> gameEvent, Action<TArgs1, TArgs2> action)
        {
            EventsListener eventsListener = CreateEventsListener(monoBehaviour);
            eventsListener.Subscribe(gameEvent, action);
        }

        public static void SubscribeWithDelay(this MonoBehaviour monoBehaviour, GameEvent gameEvent, Action action, float delay)
        {
            EventsListener eventsListener = CreateEventsListener(monoBehaviour);
            eventsListener.SubscribeWithDelay(gameEvent, action, delay);
        }

        public static void SubscribeWithDelay<TArgs>(this MonoBehaviour monoBehaviour, GameEvent<TArgs> gameEvent, Action<TArgs> action, float delay)
        {
            EventsListener eventsListener = CreateEventsListener(monoBehaviour);
            eventsListener.SubscribeWithDelay(gameEvent, action, delay);
        }

        public static void SubscribeWithDelay<TArgs1, TArgs2>(this MonoBehaviour monoBehaviour, GameEvent<TArgs1, TArgs2> gameEvent, Action<TArgs1, TArgs2> action, float delay)
        {
            EventsListener eventsListener = CreateEventsListener(monoBehaviour);
            eventsListener.SubscribeWithDelay(gameEvent, action, delay);
        }

        public static void UnSubscribe(this MonoBehaviour monoBehaviour, GameEvent gameEvent)
        {
            EventsListener eventsListener = CreateEventsListener(monoBehaviour);
            eventsListener.UnSubscribe(gameEvent);
        }

        public static void UnSubscribe<TArgs>(this MonoBehaviour monoBehaviour, GameEvent<TArgs> gameEvent)
        {
            EventsListener eventsListener = CreateEventsListener(monoBehaviour);
            eventsListener.UnSubscribe(gameEvent);
        }

        public static void UnSubscribe<TArgs1, TArgs2>(this MonoBehaviour monoBehaviour, GameEvent<TArgs1, TArgs2> gameEvent)
        {
            EventsListener eventsListener = CreateEventsListener(monoBehaviour);
            eventsListener.UnSubscribe(gameEvent);
        }
    }
}