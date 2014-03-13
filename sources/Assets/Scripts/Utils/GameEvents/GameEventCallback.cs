using System;

namespace Assets.Scripts.Utils.GameEvents
{
    public class GameEventCallback<TEventArgs> where TEventArgs : GameEventArgs
    {
        public int SubscriberId { get; private set; }
        public Action<TEventArgs> Callback { get; private set; }

        public GameEventCallback(int subscriberId, Action<TEventArgs> callback)
        {
            SubscriberId = subscriberId;
            Callback = callback;
        }
    }
}
