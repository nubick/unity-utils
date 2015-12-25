using System;
using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class ValueTween : TweenBase
    {
        private bool _isIntegerValue;

        private int _fromInt;
        private int _toInt;
        private Action<int> _updateActionInt;

        private float _fromFloat;
        private float _toFloat;
        private Action<float> _updateActionFloat;

        public static ValueTween Run(GameObject item, float from, float to, float duration, Action<float> updateAction)
        {
            ValueTween tween = Create<ValueTween>(item, duration);
            tween._fromFloat = from;
            tween._toFloat = to;
            tween._updateActionFloat = updateAction;
            tween._isIntegerValue = false;
            return tween;
        }

        public static ValueTween Run(GameObject item, int from, int to, float duration, Action<int> updateAction)
        {
            ValueTween tween = Create<ValueTween>(item, duration);
            tween._fromInt = from;
            tween._toInt = to;
            tween._updateActionInt = updateAction;
            tween._isIntegerValue = true;
            return tween;
        }

        protected override void UpdateValue(float time)
        {
            if (_isIntegerValue)
            {
                _updateActionInt((int)EaseFunc(_fromInt, _toInt, time));
            }
            else
            {
                _updateActionFloat(EaseFunc(_fromFloat, _toFloat, time));
            }
        }
    }
}
