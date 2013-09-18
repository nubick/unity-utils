using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Utils
{
	public class GameEvent<TEventArgs> where TEventArgs : GameEventArgs
	{
		private readonly List<Func<TEventArgs, PendingCommand<TEventArgs>>> _priorCallbacks;
		private readonly List<Action<TEventArgs>> _callbacks;
		private readonly Dictionary<int, List<PendingCommand<TEventArgs>>> _publishMap;
		private int _publishToken;

		public GameEvent()
		{
			_callbacks = new List<Action<TEventArgs>>();
			_priorCallbacks = new List<Func<TEventArgs, PendingCommand<TEventArgs>>>();
			_publishMap = new Dictionary<int, List<PendingCommand<TEventArgs>>>();
		}

		public void Subscribe(Func<TEventArgs, PendingCommand<TEventArgs>> priorCallback)
		{
			_priorCallbacks.Add(priorCallback);
		}

		public void Subscribe(Action<TEventArgs> callback)
		{
			_callbacks.Add(callback);
		}

		public void Publish(TEventArgs eventArgs)
		{
			if (_priorCallbacks.Any())
			{
				_publishToken++;
				_publishMap.Add(_publishToken, new List<PendingCommand<TEventArgs>>());
				foreach (Func<TEventArgs, PendingCommand<TEventArgs>> priorCallback in _priorCallbacks)
				{
					PendingCommand<TEventArgs> command = priorCallback(eventArgs);
					_publishMap[_publishToken].Add(command);
					command.Finished += OnPriorCommandFinished;
				}
			}
			else
			{
				PublishNotPrior(eventArgs);
			}
		}

		void OnPriorCommandFinished(object sender, EventArgs e)
		{
			PendingCommand<TEventArgs> command = sender as PendingCommand<TEventArgs>;
			command.Finished -= OnPriorCommandFinished;
			foreach (int publishToken in _publishMap.Keys)
			{
				if (_publishMap[publishToken].Contains(command))
				{
					_publishMap[publishToken].Remove(command);
					if (!_publishMap[publishToken].Any())
					{
						_publishMap.Remove(publishToken);
						PublishNotPrior(command.Args);
					}
					break;
				}
			}
		}

		private void PublishNotPrior(TEventArgs eventArgs)
		{
			foreach (Action<TEventArgs> callback in _callbacks)
				callback(eventArgs);
		}
	}
}
