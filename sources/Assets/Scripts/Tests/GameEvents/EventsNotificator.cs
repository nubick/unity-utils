using UnityEngine;
using Assets.Scripts.Utils.GameEvents;
using UnityEngine.UI;

namespace Assets.Scripts.Tests.GameEvents
{
    public class EventsNotificator : MonoBehaviour
    {
        private int CoinsAmount;
        private int ScoreAmount;

        public Text NotificationText;
        public float FadeOutTime;
        public Image StarImage;
        public Text CoinsAmountComponent;
        public Text ScoreAmountComponent;

        public void Awake()
        {
            this.Subscribe(MetagameEvents.NewGameStarted, () => ShowText("New Game Started"));
            this.Subscribe(MetagameEvents.CoinsGot, OnCoinsGot);
            this.Subscribe(MetagameEvents.GameFinished, OnGameFinished);
            this.Subscribe(MetagameEvents.StarAppeared, (posX, posY) => ShowText(string.Format("Star appeared at ({0}, {1}) after delay.", posX, posY)));
            this.SubscribeWithDelay(MetagameEvents.StarAppeared, OnStarAppeared, 1f);
            RefreshCoinsAmountText();
            RefreshScoreAmountText();
		}

        public void UnSubscribeAll()
        {
            this.UnSubscribe(MetagameEvents.NewGameStarted);
            this.UnSubscribe(MetagameEvents.CoinsGot);
            this.UnSubscribe(MetagameEvents.GameFinished);
            this.UnSubscribe(MetagameEvents.StarAppeared);
        }

        private void ShowText(string text)
        {
            NotificationText.text = text;
            NotificationText.canvasRenderer.SetAlpha(1f);
            NotificationText.CrossFadeAlpha(0f, FadeOutTime, true);
        }

        private void OnStarAppeared(float posX, float posY)
        {
            StarImage.transform.localPosition = new Vector2(posX, posY);
            StarImage.canvasRenderer.SetAlpha(1f);
            StarImage.CrossFadeAlpha(0f, FadeOutTime, true);
        }

        private void OnCoinsGot(int amount)
        {
            CoinsAmount += amount;
            RefreshCoinsAmountText();
            ShowText("Got coins: " + amount);
        }

        private void RefreshCoinsAmountText()
        {
			CoinsAmountComponent.text = "Coins: " + CoinsAmount;
		}

        private void OnGameFinished(GameFinishedArgs args)
        {
            ScoreAmount += args.Score;
            RefreshScoreAmountText();
            ShowText("Game finished, reason: " + args.Reason + ", score: " + args.Score);
        }

        private void RefreshScoreAmountText()
        {
            ScoreAmountComponent.text = "Score: " + ScoreAmount;
        }
    }
}
