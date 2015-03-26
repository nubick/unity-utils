using System;
using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public abstract class TweenBase : MonoBehaviourBase
    {
        private bool _isStarted;
        protected float _startTime;
        protected float _duration;
        protected Func<float, float, float, float> EaseFunc;

		public Action FinishAction { get; private set; }


        public static T Create<T>(GameObject item, float duration) where T:TweenBase
        {
            //T tween = item.GetComponent<T>();
            //if (tween == null)
            //    tween = item.gameObject.AddComponent<T>();
            //else
            //    tween.StopAllCoroutines();

            T tween = item.gameObject.AddComponent<T>();
            tween.EaseFunc = Mathf.Lerp;
            tween._duration = duration;
            tween._startTime = Time.time;
            return tween;
        }

		public TweenBase SetEase(Func<float, float, float, float> easeFunc)
        {
            EaseFunc = easeFunc;
			return this;
        }

		public TweenBase SetDelay(float delay)
		{
			_startTime = Time.time + delay;
			return this;
		}

        public void Update()
        {
            if(_isStarted)
            {
                float time = (Time.time - _startTime)/_duration;
                if (time > 1f)
                {
					Finish();
                }
                else
                {
                    UpdateValue(time);
                }
            }
            else if (Time.time >= _startTime)
            {
                _isStarted = true;
                OnStart();
            }
        }

		protected virtual void OnStart() { }

		protected abstract void UpdateValue(float time);

		public void Finish()
		{
			UpdateValue(1f);
			OnFinish();
			Destroy(this);
		}

		protected virtual void OnFinish()
		{
			if (FinishAction != null)
				FinishAction();
		}

		public TweenBase OnFinish(Action finishAction)
		{
			FinishAction = finishAction;
			return this;
		}
    }
}
