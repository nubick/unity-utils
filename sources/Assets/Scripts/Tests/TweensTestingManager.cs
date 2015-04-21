﻿using System;
using System.Collections.Generic;
using System.Globalization;
using Assets.Scripts.Utils;
using Assets.Scripts.Utils.Tweens;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Tests
{
    public class TweensTestingManager : MonoBehaviourBase
    {
        public List<GameObject> GameObjects;
        public GameObject ActiveObject;
		public float Duration;
	    public float Delay;

	    public Toggle[] EaseToggles;

	    private readonly Func<float, float, float, float>[] EaseFunctions =
	    {
		    Ease.Linear,
		    Ease.OutBack,
		    Ease.InBack,
			Ease.InOutBack,
			Ease.OutCirc,
			Ease.InCirc,
			Ease.InOutCirc
	    };//Sequence must be same as in EaseToggles binding.

        public void Awake()
        {
            SelectActiveObject(0);
        }

	    public void OnFadeOutTween()
	    {
		    FadeOutTween.Run(ActiveObject, Duration).SetDelay(Delay).SetEase(GetSelectedEase());
	    }

	    public void OnFadeInTween()
	    {
		    FadeInTween.Run(ActiveObject, Duration).SetDelay(Delay).SetEase(GetSelectedEase());
	    }

	    public void OnScaleTween()
	    {
		    TweenSequence.Run(
			    () => ScaleTween.Run(ActiveObject, Vector3.zero, Duration).SetEase(GetSelectedEase()).SetDelay(Delay),
			    () => ScaleTween.Run(ActiveObject, Vector3.one, Duration).SetEase(GetSelectedEase()));
	    }

	    public void OnMoveTween()
	    {
		    MoveToTween.Run(ActiveObject, Random.insideUnitCircle*Screen.height*0.75f, Duration)
			    .SetLocal(true).SetDelay(Delay).SetEase(GetSelectedEase());
	    }

		public void OnMoveXTween()
		{
			MoveToTween.RunX(ActiveObject, (Random.insideUnitCircle*Screen.height*0.75f).x, Duration)
				.SetLocal(true).SetDelay(Delay).SetEase(GetSelectedEase());
		}

        public void OnNumberRunTween()
        {
	        NumberRunTween.Run(ActiveObject, 1, 99, Duration).SetEase(GetSelectedEase()).SetDelay(Delay);
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
	        ActiveObject.transform.localPosition = Vector3.zero;
            ActiveObject.SetActive(true);
        }

		public void UpdateDuration(string value)
	    {
		    Duration = float.Parse(value, NumberStyles.Float);
	    }

		public void UpdateDelay(string value)
		{
			Delay = float.Parse(value, NumberStyles.Float);
		}

	    private Func<float, float, float, float> GetSelectedEase()
	    {
		    for (int i = 0; i < EaseToggles.Length; i++)
		    {
			    if (EaseToggles[i].isOn)
				    return EaseFunctions[i];
		    }
			throw new Exception("No any ease selected.");
	    }
    }
}