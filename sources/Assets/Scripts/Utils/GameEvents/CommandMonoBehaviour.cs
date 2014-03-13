using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Utils.GameEvents
{
	public abstract class CommandMonoBehaviour : MonoBehaviourBase
	{
		private readonly Queue<PendingCommand> _pendingCommands;
	    private readonly int _subscriberId;
	    private static int _counter;

		protected CommandMonoBehaviour()
		{
			_pendingCommands = new Queue<PendingCommand>();
            _subscriberId = ++_counter;
		}

		public virtual void Update()
		{
		    while (_pendingCommands.Any())
		    {
		        _pendingCommands.Dequeue().Execute();
		    }
		}

		protected void Subscribe<TEventArgs>(GameEvent<TEventArgs> gameEvent, Action<TEventArgs> action)
			where TEventArgs : GameEventArgs
		{
		    gameEvent.Subscribe(_subscriberId, args => EnqueuePendingCommand(action, args));
		}

        private void EnqueuePendingCommand<TEventArgs>(Action<TEventArgs> action, TEventArgs args)
            where TEventArgs : GameEventArgs
	    {
            _pendingCommands.Enqueue(new PendingCommand<TEventArgs>(action, args));
	    }

        protected void UnSubscribe<TEventArgs>(GameEvent<TEventArgs> gameEvent)
            where TEventArgs : GameEventArgs
        {
            gameEvent.UnSubscribe(_subscriberId);
        }
	}

}
