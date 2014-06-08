using System;
using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class NumberRunTween : MonoBehaviourBase
    {
        private int _from;
        private int _to;
        private float _duration;
        private Action<int> _updateDelegate;
        private float _startTime;

        public static void Run(GameObject item, int from, int to, float duration, Action<int> updateDelegate)
        {
            NumberRunTween tween = item.GetComponent<NumberRunTween>();
            if (tween == null)
                tween = item.AddComponent<NumberRunTween>();
            else
                tween.StopAllCoroutines();
            tween.Run(from, to, duration, updateDelegate);
        }

        private void Run(int from, int to, float duration, Action<int> updateDelegate)
        {
            _startTime = Time.time;
            _from = from;
            _to = to;
            _duration = duration;
            _updateDelegate = updateDelegate;
        }

        public void Update()
        {
            float t = (Time.time - _startTime)/_duration;
            if (t > 1.0f)
            {
                _updateDelegate(_to);
                Destroy(this);
            }
            else
            {
                _updateDelegate((int)Mathf.Lerp(_from, _to, t));
            }
        }
    }
}
