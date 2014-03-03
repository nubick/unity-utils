using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;

namespace UnityTest
{
	[TestFixture]
	public class NSubstituteDemo
	{
		[Test]
		public void RegisteredEventListenersGetEvents ()
		{
			var sink = new GameEventSink ();
			var listener = Substitute.For<IGameEventListener> ();
			sink.RegisterListener (listener);
			sink.ReceiveEvent (Substitute.For<IGameEvent> ());
			listener.Received ().ReceiveEvent (Arg.Any<IGameEvent> ());
		}
	}

	public class GameEventSink : IGameEventListener
	{
		private List<IGameEventListener> listeners = new List<IGameEventListener> ();

		public void RegisterListener (IGameEventListener listener)
		{
			listeners.Add (listener);
		}

		public void ReceiveEvent (IGameEvent gameEvent)
		{
			foreach (var gameEventListener in listeners)
			{
				gameEventListener.ReceiveEvent (gameEvent);
			}
		}
	}

	public interface IGameEvent
	{
	}

	public interface IGameEventListener
	{
		void ReceiveEvent (IGameEvent gameEvent);
	}
}
