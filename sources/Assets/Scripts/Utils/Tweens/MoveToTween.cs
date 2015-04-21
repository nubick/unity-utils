using UnityEngine;

namespace Assets.Scripts.Utils.Tweens
{
    public class MoveToTween : TweenBase
    {
        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private bool _isLocal;
	    private float? _targetX;
	    private float? _targetY;
	    private float? _targetZ;

        public static MoveToTween Run(GameObject item, Vector3 position, float duration)
        {
            MoveToTween tween = Create<MoveToTween>(item, duration);
            tween._targetPosition = position;
            return tween;
        }

		public static MoveToTween RunX(GameObject item, float x, float duration)
		{
			MoveToTween tween = Create<MoveToTween>(item, duration);
			tween._targetX = x;
			return tween;
		}

		public static MoveToTween RunY(GameObject item, float y, float duration)
		{
			MoveToTween tween = Create<MoveToTween>(item, duration);
			tween._targetY = y;
			return tween;
		}

		public static MoveToTween RunZ(GameObject item, float z, float duration)
		{
			MoveToTween tween = Create<MoveToTween>(item, duration);
			tween._targetZ = z;
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

		    if (_targetX.HasValue)
			    _targetPosition = new Vector3(_targetX.Value, _startPosition.y, _startPosition.z);
		    else if (_targetY.HasValue)
			    _targetPosition = new Vector3(_startPosition.x, _targetY.Value, _startPosition.z);
		    else if (_targetZ.HasValue)
			    _targetPosition = new Vector3(_startPosition.x, _startPosition.y, _targetZ.Value);
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
    }
}
