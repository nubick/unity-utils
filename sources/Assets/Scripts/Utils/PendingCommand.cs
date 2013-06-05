using System;
using UnityEngine;

namespace Assets.Scripts.Utils
{
	public abstract class PendingCommand
	{
		protected float _delay;
		protected float _createTime;
		public bool IsTime { get { return _createTime + _delay <= Time.time; } }

		public abstract void Execute();
	}

	public class PendingCommand<TEventArgs> : PendingCommand
		where TEventArgs : GameEventArgs
	{
		private readonly Action<TEventArgs> _callback;
		private readonly TEventArgs _args;

		public PendingCommand(Action<TEventArgs> callback, TEventArgs args)
		{
			_callback = callback;
			_args = args;
		}

		public PendingCommand(Action<TEventArgs> callback, TEventArgs args, float delay)
		{
			_callback = callback;
			_args = args;
			_delay = delay;
			_createTime = Time.time;
		}

		public override void Execute()
		{
			_callback(_args);
		}
	}
}
