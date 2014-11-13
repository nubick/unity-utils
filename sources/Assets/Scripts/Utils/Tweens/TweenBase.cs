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

        protected static T Create<T>(GameObject item, float duration) where T:TweenBase
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

        public void SetEase(Func<float, float, float, float> easeFunc)
        {
            EaseFunc = easeFunc;
        }

        public void SetDelay(float delay)
        {
            _startTime = Time.time + delay;
        }

        protected abstract void OnStart();

        public void Update()
        {
            if(_isStarted)
            {
                float time = (Time.time - _startTime)/_duration;
                if (time > 1f)
                {
                    UpdateValue(1f);
                    Destroy(this);
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

        protected abstract void UpdateValue(float time);
    }
}
