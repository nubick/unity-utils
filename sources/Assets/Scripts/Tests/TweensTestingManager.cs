using System;
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
		private List<Toggle> _easeToggles = new List<Toggle>();

        public List<GameObject> GameObjects;
        public GameObject ActiveObject;
		public float Duration;
	    public float Delay;
		public Ease Ease;
		public RectTransform EaseTogglesRoot;
		public RectTransform EaseTogglePrefab;

        public void Awake()
        {
            SelectActiveObject(0);
			CreateEaseToggles();
        }

		private void CreateEaseToggles()
		{
			Toggle.ToggleEvent toggleEvent = new Toggle.ToggleEvent();
			toggleEvent.AddListener(new UnityEngine.Events.UnityAction<bool>(OnValueChanged));

			ToggleGroup toggleGroup = EaseTogglesRoot.GetComponent<ToggleGroup>();
			foreach(string easeName in Enum.GetNames(typeof(Ease)))
			{
				RectTransform easeToggle = Instantiate(EaseTogglePrefab);
				easeToggle.SetParent(EaseTogglesRoot);
				easeToggle.name = easeName;
				easeToggle.localScale = Vector3.one;
				easeToggle.GetComponentInChildren<Text>().text = easeName;
				Toggle toggle = easeToggle.GetComponent<Toggle>();
				toggle.group = toggleGroup;
				toggle.onValueChanged = toggleEvent;
				_easeToggles.Add(toggle);
			}
			toggleGroup.SetAllTogglesOff();
			_easeToggles[0].isOn = true;
		}

		private void OnValueChanged(bool isOn)
		{
			foreach(Toggle toggle in _easeToggles)
				if(toggle.isOn)
					Ease = (Ease)Enum.Parse(typeof(Ease), toggle.name);
		}

	    public void OnFadeOutTween()
	    {
		    FadeOutTween.Run(ActiveObject, Duration).SetDelay(Delay).SetEase(Ease);
	    }

	    public void OnFadeInTween()
	    {
		    FadeInTween.Run(ActiveObject, Duration).SetDelay(Delay).SetEase(Ease);
	    }

	    public void OnScaleTween()
	    {
		    TweenSequence.Run2(
			    () => ScaleTween.Run(ActiveObject, Vector3.zero, Duration).SetEase(Ease).SetDelay(Delay),
			    () => ScaleTween.Run(ActiveObject, Vector3.one, Duration).SetEase(Ease));
	    }

        private Vector2 GetRandomScreenPoint()
        {
            return Random.insideUnitCircle*Screen.height*0.75f;
        }

	    public void OnMoveTween()
	    {
		    MoveToTween.Run(ActiveObject, GetRandomScreenPoint(), Duration)
			    .SetLocal(true).SetDelay(Delay).SetEase(Ease);
	    }

		public void OnMoveXTween()
		{
			MoveToTween.RunX(ActiveObject, GetRandomScreenPoint().x, Duration)
				.SetLocal(true).SetDelay(Delay).SetEase(Ease);
		}

        public void OnNumberRunTween()
        {
	        NumberRunTween.Run(ActiveObject, 1, 99, Duration).SetEase(Ease).SetDelay(Delay);
        }

        public void OnShakeTween()
        {
            TweenSequence.Run3(
                () => ShakeTween.Run(ActiveObject, 25, Duration),
                () => ShakeTween.RunVertical(ActiveObject, 25, Duration),
                () => ShakeTween.Run(ActiveObject, 25, Duration).SetDirection(new Vector2(3f, 1f)));
        }

        public void RunKickTween()
        {
            TweenSequence.Run5(
                () => KickTween.RunLeft(ActiveObject, 100f),
                () => KickTween.RunRight(ActiveObject, 100f),
                () => KickTween.RunUp(ActiveObject, 100f),
                () => KickTween.RunDown(ActiveObject, 100f),
                () => KickTween.RunDirection(ActiveObject, 100f, new Vector2(4f, 3f)));
        }

		public void RunCameraTween()
		{
			float startSize = Camera.main.orthographicSize;
			TweenSequence.Run2(
				() => CameraTween.ChangeSize(Camera.main, 200, Duration).SetEase(Ease),
				() => CameraTween.ChangeSize(Camera.main, startSize, Duration).SetEase(Ease).SetDelay(1f));
		}

        public void RunValueTween(bool isIntegerValue)
        {
            Text textComponent = ActiveObject.GetComponent<Text>();
            if (textComponent == null)
            {
                Debug.Log("Test error! Can't test ValueTween. Select text component before.");
            }
            else
            {
                if (isIntegerValue)
                {
                    ValueTween.Run(ActiveObject, 10, 100, Duration,
                        iValue =>
                        {
                            textComponent.text = iValue.ToString();
                        }).SetEase(Ease).SetDelay(Delay);
                }
                else
                {
                    ValueTween.Run(ActiveObject, 100f, 10f, Duration,
                        fValue =>
                        {
                            textComponent.text = fValue.ToString("F");
                        }).SetEase(Ease).SetDelay(Delay);
                }
            }
        }

        public void RunJump2DTween()
        {
            Jump2DTween.Run(ActiveObject, GetRandomScreenPoint(), Duration).SetJumpYOffset(100).SetDelay(Delay);
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
    }
}
