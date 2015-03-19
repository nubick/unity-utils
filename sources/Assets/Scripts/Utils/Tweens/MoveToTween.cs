using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class MoveToTween : TweenBase
    {
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private bool _isLocal;

        public static MoveToTween Run(GameObject item, Vector3 position, float duration)
        {
            MoveToTween tween = Create<MoveToTween>(item, duration);
            tween._targetPosition = position;
            return tween;
        }

        public MoveToTween SetLocal(bool isLocal)
        {
            _isLocal = isLocal;
            return this;
        }

        protected override void OnStart()
        {
            _startPosition = _isLocal ? Transform.localPosition : Transform.position;
        }

        protected override void UpdateValue(float time)
        {
            if (_isLocal)
            {
                Transform.localPosition = new Vector3(
                    EaseFunc(_startPosition.x, _targetPosition.x, time),
                    EaseFunc(_startPosition.y, _targetPosition.y, time),
                    EaseFunc(_startPosition.z, _targetPosition.z, time));
            }
            else
            {
                Transform.position = new Vector3(
                    EaseFunc(_startPosition.x, _targetPosition.x, time),
                    EaseFunc(_startPosition.y, _targetPosition.y, time),
                    EaseFunc(_startPosition.z, _targetPosition.z, time));
            }
        }

		//private Vector3 _startPosition;
		//private Vector3 _targetPosition;
		//private bool _isLocal;

		//public static MoveToTween Run(GameObject item, Vector3 position, float duration)
		//{
		//	MoveToTween tween = Create<MoveToTween>(item, duration);
		//	tween._targetPosition = position;
		//	return tween;
		//}

		//public static MoveToTween RunX(GameObject item, float x, float duration)
		//{
		//	Vector3 position = item.transform.localPosition;
		//	position.x = x;
		//	return Run(item, position, duration);
		//}

		//public MoveToTween SetLocal(bool isLocal)
		//{
		//	_isLocal = isLocal;
		//	return this;
		//}

		//protected override void OnStart()
		//{
		//	_startPosition = _isLocal ? Transform.localPosition : Transform.position;
		//}

		//protected override void UpdateValue(float time)
		//{
		//	if (_isLocal)
		//	{
		//		Transform.localPosition = new Vector3(
		//			EaseFunc(_startPosition.x, _targetPosition.x, time),
		//			EaseFunc(_startPosition.y, _targetPosition.y, time),
		//			EaseFunc(_startPosition.z, _targetPosition.z, time));
		//	}
		//	else
		//	{
		//		Transform.position = new Vector3(
		//			EaseFunc(_startPosition.x, _targetPosition.x, time),
		//			EaseFunc(_startPosition.y, _targetPosition.y, time),
		//			EaseFunc(_startPosition.z, _targetPosition.z, time));
		//	}
		//}

    }
}
