using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Tweens;
using UnityEngine;

namespace Assets.Scripts.Tests
{
    public class TweensTestingManager : MonoBehaviourBase
    {
        public List<GameObject> GameObjects;
        public GameObject ActiveObject;
		public float Duration;
	    public float Delay;

        public void Awake()
        {
            SelectActiveObject(0);
        }

        public void OnFadeTweens()
        {
			FadeOutTween.Run(ActiveObject, Duration);
			FadeInTween.Run(ActiveObject, Duration).SetDelay(2.5f);
        }

        public void OnScaleTween()
        {
			ScaleTween.Run(ActiveObject, Vector3.zero, Duration);
			ScaleTween.Run(ActiveObject, Vector3.one, Duration).SetDelay(Duration);
        }

        public void OnMoveTween()
        {
			MoveToTween.Run(ActiveObject, Random.insideUnitCircle * Screen.height, Duration)
                .SetLocal(true).SetEase(Ease.InBack);
        }

        public void OnNumberRunTween()
        {
			NumberRunTween.Run(ActiveObject, 1, 99, Duration);
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

		public void UpdateDuration(string value)
	    {
			Debug.Log(value);
		    Duration = float.Parse(value, NumberStyles.Float);
	    }
    }
}
