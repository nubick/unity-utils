using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utils
{
	public abstract class CommandMonoBehaviour : MonoBehaviourBase
	{
		private readonly Queue<PendingCommand> _pendingCommands;

		protected CommandMonoBehaviour()
		{
			_pendingCommands = new Queue<PendingCommand>();
		}

		public virtual void Update()
		{
			for (int i = 0; i < _pendingCommands.Count; i++)
			{
				PendingCommand command = _pendingCommands.Dequeue();
				if (command.IsTime)
					command.Execute();
				else
					_pendingCommands.Enqueue(command);
			}
		}

		protected void Subscribe<TEventArgs>(GameEvent<TEventArgs> gameEvent, Action<TEventArgs> action, bool isPrior = false)
			where TEventArgs : GameEventArgs
		{
			if (isPrior)
			{
				gameEvent.Subscribe(args =>
				{
					PendingCommand<TEventArgs> command = new PendingCommand<TEventArgs>(action, args);
					_pendingCommands.Enqueue(command);
					return command;
				});
			}
			else
			{
				gameEvent.Subscribe(args =>
				{
					PendingCommand<TEventArgs> command = new PendingCommand<TEventArgs>(action, args);
					_pendingCommands.Enqueue(command);
				});
			}
		}

		protected void Subscribe<TEventArgs>(GameEvent<TEventArgs> gameEvent, Action<TEventArgs> action, float delay)
			where TEventArgs : GameEventArgs
		{
			gameEvent.Subscribe(args =>
			{
				PendingCommand<TEventArgs> command = new PendingCommand<TEventArgs>(action, args, delay);
				_pendingCommands.Enqueue(command);
			});
		}

	}

}
