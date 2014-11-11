using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Tweens;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class FadeTweensUI : MonoBehaviourBase
    {
        public GameObject Sprite1;
        public GameObject Sprite2;

        public void OnGUI()
        {
            GUILayout.BeginVertical();

            if(GUILayout.Button("Run FadeOut and FadeIn tweens"))
            {
                FadeOutTween.Run(Sprite1, 2f);
                FadeInTween.Run(Sprite1, 2f, 2.5f);
            }

            if (GUILayout.Button("Run scale tween"))
            {
                ScaleTween.Run(Sprite1, Vector3.zero, 1f);
                ScaleTween.Run(Sprite1, Vector3.one, 1f, 1.5f);
            }

            GUILayout.EndVertical();
        }
    }
}
