using System;
using UnityEngine;

namespace Assets.Scripts.Utils
{
	public abstract class PendingCommand
	{
		protected float _delay;
		protected float _createTime;
		public bool IsTime { get { return _createTime + _delay <= Time.time; } }

		public event EventHandler<EventArgs> Finished;

		public abstract void Execute();

		protected void RaiseFinished()
		{
			if (Finished != null)
				Finished(this, EventArgs.Empty);
		}
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

		public PendingCommand(Action<TEventArgs> callback, TEventArgs args, float delay)
		{
			_callback = callback;
			Args = args;
			_delay = delay;
			_createTime = Time.time;
		}

		public override void Execute()
		{
			_callback(Args);
			RaiseFinished();
		}
	}
}
