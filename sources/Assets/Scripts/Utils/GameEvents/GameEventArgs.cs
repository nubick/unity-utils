
namespace Assets.Scripts.Utils.GameEvents
{
	public class GameEventArgs
	{
		private static GameEventArgs _empty;
		public static GameEventArgs Empty
		{
			get
			{
				if (_empty == null)
					_empty = new GameEventArgs();
				return _empty;
			}
		}
	}

	public class GameEventArgs<T> : GameEventArgs
	{
		public T Value { get; private set; }

		public GameEventArgs(T value)
		{
			Value = value;
		}
	}
}
