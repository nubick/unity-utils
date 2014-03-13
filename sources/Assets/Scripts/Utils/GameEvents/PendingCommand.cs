using System;

namespace Assets.Scripts.Utils.GameEvents
{
	public abstract class PendingCommand
	{
		public abstract void Execute();
	}

	public class PendingCommand<TEventArgs> : PendingCommand
		where TEventArgs : GameEventArgs
	{
		private readonly Action<TEventArgs> _callback;

		public TEventArgs Args { get; private set; }

		public PendingCommand(Action<TEventArgs> callback, TEventArgs args)
		{
			_callback = callback;
			Args = args;
		}

		public override void Execute()
		{
			_callback(Args);
		}
	}
}
