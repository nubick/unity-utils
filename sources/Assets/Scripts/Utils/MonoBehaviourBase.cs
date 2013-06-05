using UnityEngine;

namespace Assets.Scripts.Utils
{
	public abstract class MonoBehaviourBase : MonoBehaviour
	{
		private Transform _transform;
		public Transform Transform
		{
			get
			{
				if (_transform == null)
					_transform = transform;
				return _transform;
			}
		}

		protected T Instantiate<T>(Object obj) where T : Object
		{
			return (T)Instantiate(obj);
		}

	}
}
