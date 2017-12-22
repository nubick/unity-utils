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
            _subscriberId = GetNextSubscriberId();
        }

        public static int GetNextSubscriberId()
        {
            _counter++;
            return _counter;
        }

        public void Subscribe(GameEvent gameEvent, Action callback)
        {
            gameEvent.Subscribe(_subscriberId, callback);
        }

        public void Subscribe<TArgs>(GameEvent<TArgs> gameEvent, Action<TArgs> callback)
        {
            gameEvent.Subscribe(_subscriberId, callback);
        }

        public void Subscribe<TArgs1, TArgs2>(GameEvent<TArgs1, TArgs2> gameEvent, Action<TArgs1, TArgs2> callback)
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

        public void SubscribeWithDelay<TArgs>(GameEvent<TArgs> gameEvent, Action<TArgs> callback, float delay)
        {
            gameEvent.Subscribe(_subscriberId, args =>
            {
                StartCoroutine(ExecuteAfterDelay(callback, args, delay));
            });
        }

        public void SubscribeWithDelay<TArgs1, TArgs2>(GameEvent<TArgs1, TArgs2> gameEvent, Action<TArgs1, TArgs2> callback, float delay)
        {
            gameEvent.Subscribe(_subscriberId, (args1, args2) =>
            {
                StartCoroutine(ExecuteAfterDelay(callback, args1, args2, delay));
            });
        }

        private IEnumerator ExecuteAfterDelay(Action callback, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback();
        }

        private IEnumerator ExecuteAfterDelay<TArgs>(Action<TArgs> callback, TArgs args, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback(args);
        }

        private IEnumerator ExecuteAfterDelay<Args1, Args2>(Action<Args1, Args2> callback, Args1 args1, Args2 args2, float delay)
        {
            yield return new WaitForSeconds(delay);
            callback(args1, args2);
        }

        public void UnSubscribe(GameEvent gameEvent)
        {
            gameEvent.UnSubscribe(_subscriberId);
        }

        public void UnSubscribe<TArgs>(GameEvent<TArgs> gameEvent)
        {
            gameEvent.UnSubscribe(_subscriberId);
        }

        public void UnSubscribe<TArgs1, TArgs2>(GameEvent<TArgs1, TArgs2> gameEvent)
        {
            gameEvent.UnSubscribe(_subscriberId);
        }
    }
}