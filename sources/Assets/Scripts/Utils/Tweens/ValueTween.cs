using System;
using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class ValueTween : TweenBase
    {
        private float _from;
        private float _to;
        private Action<float> _updateAction;

        public static ValueTween Run(GameObject item, float from, float to, float duration, Action<float> updateAction)
        {
            ValueTween tween = Create<ValueTween>(item, duration);
            tween._from = from;
            tween._to = to;
            tween._updateAction = updateAction;
            return tween;
        }

        protected override void OnStart()
        {
            
        }

        protected override void UpdateValue(float time)
        {
            _updateAction(((int) Mathf.Lerp(_from, _to, time)));
        }
    }
}
