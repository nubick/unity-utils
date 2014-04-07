using System;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Tweens;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class Scene2DUI : MonoBehaviourBase
    {
        public GameObject FlashObject;
        public float FlashDuration;

        public GameObject[] FadeOutObjects;
        public float FadeOutTime;

        public void OnGUI()
        {
            GUILayout.BeginVertical();

            if (GUILayout.Button("Run FlashTween"))
            {
                FlashTween.Run(FlashObject, FlashDuration);
            }

            if (GUILayout.Button("Run Fade Out Tween"))
            {
                foreach (GameObject fadeOutObject in FadeOutObjects)
                {
                    fadeOutObject.SetActive(true);
                    FadeOutTween.Run(fadeOutObject, FadeOutTime);
                }
            }

            GUILayout.EndVertical();
        }
    }
}
