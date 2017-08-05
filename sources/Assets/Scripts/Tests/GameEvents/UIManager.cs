using Assets.Scripts.Utils.GameEvents;
using UnityEngine;

namespace Assets.Scripts.Tests.GameEvents
{
    public class UIManager : MonoBehaviour
    {
        public void RaiseNewGameStarted()
        {
            MetagameEvents.NewGameStarted.Publish();
        }

        public void RaiseCoinsGotEvent()
        {
            MetagameEvents.CoinsGot.Publish(Random.Range(10, 100));
        }

        public void RaiseGameWinEvent()
        {
            MetagameEvents.GameFinished.Publish(new GameFinishedArgs(GameFinishedReason.Win, Random.Range(1, 10)));
        }

        public void RaiseGameLoseEvent()
        {
            MetagameEvents.GameFinished.Publish(new GameFinishedArgs(GameFinishedReason.Lose));
        }

        public void RaiseGameRestartEvent()
        {
            MetagameEvents.GameFinished.Publish(new GameFinishedArgs(GameFinishedReason.Restart));
        }
    }
}