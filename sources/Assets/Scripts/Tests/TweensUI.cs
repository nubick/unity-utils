using Assets.Scripts.Utils.Tweens;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class TweensUI : MonoBehaviour
    {
        public TextMesh NumberRunText;
        public int NumberRunFrom;
        public int NumberRunTo;
        public float NumberRunDuration;


        public void OnGUI()
        {
            DrawNumberRunTweenUI();
        }


        private void DrawNumberRunTweenUI()
        {
            if (GUILayout.Button("Run"))
            {
                NumberRunTween.Run(NumberRunText.gameObject, NumberRunFrom, NumberRunTo, NumberRunDuration,
                    value => NumberRunText.text = value.ToString());
            }
        }

    }
}
