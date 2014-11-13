using System.Collections.Generic;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Tweens;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class TweensTestingManager : MonoBehaviourBase
    {
        public List<GameObject> GameObjects;
        public GameObject ActiveObject;

        public void Awake()
        {
            SelectActiveObject(0);
        }

        public void OnFadeTweens()
        {
            FadeOutTween.Run(ActiveObject, 2f);
            FadeInTween.Run(ActiveObject, 2f).SetDelay(2.5f);
        }

        public void OnScaleTween()
        {
            ScaleTween.Run(ActiveObject, Vector3.zero, 1f);
            ScaleTween.Run(ActiveObject, Vector3.one, 1f).SetDelay(1.5f);
        }

        public void OnMoveTween()
        {
            MoveToTween.Run(ActiveObject, Random.insideUnitCircle*Screen.height, 1f)
                .SetLocal(true).SetEase(Ease.InBack);
        }

        public void OnNumberRunTween()
        {
            NumberRunTween.Run(ActiveObject, 1, 99, 1f);
        }

        public void OnNext()
        {
            int ind = GameObjects.IndexOf(ActiveObject);
            ind = (ind + 1)%GameObjects.Count;
            SelectActiveObject(ind);
        }

        public void OnPrevious()
        {
            int ind = GameObjects.IndexOf(ActiveObject);
            ind = (ind - 1 + GameObjects.Count)%GameObjects.Count;
            SelectActiveObject(ind);
        }

        private void SelectActiveObject(int ind)
        {
            GameObjects.ForEach(_=>_.SetActive(false));
            ActiveObject = GameObjects[ind];
            ActiveObject.SetActive(true);
        }
    }
}
