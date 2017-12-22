using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils.GameEvents
{
    public abstract class GameEventBase
    {
        protected string _logName;
        protected bool _isLogEnabled;

        protected readonly List<int> _subscriberIds = new List<int>();

        protected GameEventBase() { }

        protected GameEventBase(string logName)
        {
            _logName = logName;
            _isLogEnabled = true;
        }

        protected void LogPublish()
        {
            if (_isLogEnabled)
                Debug.Log(string.Format("Event published: {0}", _logName));
        }

        protected void UnSubscribe(int subscriberId, IList callbacks)
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                //Subscriber can several times subscribe to the same event.
                if (_subscriberIds[i] == subscriberId)
                {
                    callbacks.RemoveAt(i);
                    _subscriberIds.RemoveAt(i);
                    i--;
                }
            }
        }

        protected void Clear(IList callbacks)
        {
            _subscriberIds.Clear();
            callbacks.Clear();
        }
    }

    public class GameEvent : GameEventBase
    {
        private readonly List<Action> _callbacks = new List<Action>();

        public GameEvent() { }
        public GameEvent(string logName) : base(logName) { }

        public void Subscribe(int subscriberId, Action callback)
        {
            _subscriberIds.Add(subscriberId);
            _callbacks.Add(callback);
        }

        public void Publish()
        {
            LogPublish();
            for (int i = 0; i < _callbacks.Count; i++)
                _callbacks[i]();
        }

        public void UnSubscribe(int subscriberId)
        {
            UnSubscribe(subscriberId, _callbacks);
        }

        public void Clear()
        {
            Clear(_callbacks);
        }
    }

    public class GameEvent<TArgs> : GameEventBase
    {
        private readonly List<Action<TArgs>> _callbacks = new List<Action<TArgs>>();

        public GameEvent() { }
        public GameEvent(string logName) : base(logName) { }

        public void Subscribe(int subscriberId, Action<TArgs> callback)
        {
            _subscriberIds.Add(subscriberId);
            _callbacks.Add(callback);
        }

        public void Publish(TArgs eventArgs)
        {
            LogPublish();
            for (int i = 0; i < _callbacks.Count; i++)
                _callbacks[i](eventArgs);
        }

        public void UnSubscribe(int subscriberId)
        {
            UnSubscribe(subscriberId, _callbacks);
        }

        public void Clear()
        {
            Clear(_callbacks);
        }
    }

    public class GameEvent<TArgs1, TArgs2> : GameEventBase
    {
        private readonly List<Action<TArgs1, TArgs2>> _callbacks = new List<Action<TArgs1, TArgs2>>();

        public GameEvent() { }
        public GameEvent(string logName) : base(logName) { }

        public void Subscribe(int subscriberId, Action<TArgs1, TArgs2> callback)
        {
            _subscriberIds.Add(subscriberId);
            _callbacks.Add(callback);
        }

        public void Publish(TArgs1 args1, TArgs2 args2)
        {
            LogPublish();
            for (int i = 0; i < _callbacks.Count; i++)
                _callbacks[i](args1, args2);
        }

        public void UnSubscribe(int subscriberId)
        {
            UnSubscribe(subscriberId, _callbacks);
        }

        public void Clear()
        {
            Clear(_callbacks);
        }
    }
}
