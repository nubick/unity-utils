
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

    public class GameEventArgs<T1, T2> : GameEventArgs
    {
        public T1 Value1 { get; private set; }
        public T2 Value2 { get; private set; }

        public GameEventArgs(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }
}
