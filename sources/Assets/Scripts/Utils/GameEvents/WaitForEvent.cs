using UnityEngine;

namespace Assets.Scripts.Utils.GameEvents
{
    public class WaitForEvent : CustomYieldInstruction
    {
        private readonly int _subscriberId;
        private GameEvent _gameEvent;
        private bool _isPublished;

        public override bool keepWaiting { get { return !_isPublished; } }

        public WaitForEvent(GameEvent gameEvent)
        {
            _subscriberId = EventsListener.GetNextSubscriberId();
            _gameEvent = gameEvent;
            _gameEvent.Subscribe(EventsListener.GetNextSubscriberId(), OnPublish);
        }

        private void OnPublish()
        {
            _isPublished = true;
            _gameEvent.UnSubscribe(_subscriberId);
        }
    }

    public class WaitForEvent<TArgs> : CustomYieldInstruction
    {
        private readonly int _subscriberId;
        private GameEvent<TArgs> _gameEvent;
        private bool _isPublished;

        public override bool keepWaiting { get { return !_isPublished; } }

        public WaitForEvent(GameEvent<TArgs> gameEvent)
        {
            _subscriberId = EventsListener.GetNextSubscriberId();
            _gameEvent = gameEvent;
            _gameEvent.Subscribe(EventsListener.GetNextSubscriberId(), OnPublish);
        }

        private void OnPublish(TArgs eventArgs)
        {
            _isPublished = true;
            _gameEvent.UnSubscribe(_subscriberId);
        }
    }

    public class WaitForEvent<TArgs1, TArgs2> : CustomYieldInstruction
    {
        private readonly int _subscriberId;
        private GameEvent<TArgs1, TArgs2> _gameEvent;
        private bool _isPublished;

        public override bool keepWaiting { get { return !_isPublished; } }

        public WaitForEvent(GameEvent<TArgs1, TArgs2> gameEvent)
        {
            _subscriberId = EventsListener.GetNextSubscriberId();
            _gameEvent = gameEvent;
            _gameEvent.Subscribe(EventsListener.GetNextSubscriberId(), OnPublish);
        }

        private void OnPublish(TArgs1 args1, TArgs2 args2)
        {
            _isPublished = true;
            _gameEvent.UnSubscribe(_subscriberId);
        }
    }
}