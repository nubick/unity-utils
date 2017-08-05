using UnityEngine;
using Assets.Scripts.Utils.GameEvents;
using UnityEngine.UI;

namespace Assets.Scripts.Tests.GameEvents
{
    public class EventsNotificator : MonoBehaviour
    {
        public Text NotificationText;
        public float FadeOutTime;

        public void Awake()
        {
            this.Subscribe(MetagameEvents.NewGameStarted, () => ShowText("New Game Started"));
            this.Subscribe(MetagameEvents.CoinsGot, _ => ShowText("Got coins: " + _));
            this.Subscribe(MetagameEvents.GameFinished, _ => ShowText("Game finished, reason: " + _.Reason + ", score: " + _.Score));
        }

        private void ShowText(string text)
        {
            NotificationText.text = text;
            NotificationText.canvasRenderer.SetAlpha(1f);
            NotificationText.CrossFadeAlpha(0f, FadeOutTime, true);
        }
    }
}
