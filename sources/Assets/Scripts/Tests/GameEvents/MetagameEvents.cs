using Assets.Scripts.Utils.GameEvents;

namespace Assets.Scripts.Tests.GameEvents
{
    public static class MetagameEvents 
    {
        public static GameEvent NewGameStarted = new GameEvent();
		public static GameEvent<int> CoinsGot = new GameEvent<int>();
		public static GameEvent<GameFinishedArgs> GameFinished = new GameEvent<GameFinishedArgs>();
        public static GameEvent<float, float> StarAppeared = new GameEvent<float, float>();
    }

    public class GameFinishedArgs
    {
		public GameFinishedReason Reason { get; private set; }
		public int Score { get; private set; }

        public GameFinishedArgs(GameFinishedReason reason, int score = 0)
        {
            Reason = reason;
            Score = score;
        }
    }

    public enum GameFinishedReason
    {
        Win,
        Lose,
        Restart
    }
}
