using System;
using System.Linq.Expressions;
using UnityEngine;
using Object = UnityEngine.Object;

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
			return (T) Instantiate(obj);
		}

		protected T Instantiate<T>(Object obj, Vector3 position) where T : Object
		{
			return (T) Instantiate(obj, position, new Quaternion());
		}

		protected T Instantiate<T>(Object obj, Vector3 position, Quaternion rotation) where T : Object
		{
			return (T) Instantiate(obj, position, rotation);
		}

        #region Invoke

	    private string GetMethodName(Expression<Action> expr)
	    {
	        return ((MethodCallExpression) expr.Body).Method.Name;
	    }

        protected void Invoke(Expression<Action> expr, float time)
	    {
	        Invoke(GetMethodName(expr), time);
        }

	    protected bool IsInvoking(Expression<Action> expr)
	    {
	        return IsInvoking(GetMethodName(expr));
	    }

	    protected void CancelInvoke(Expression<Action> expr)
	    {
	        CancelInvoke(GetMethodName(expr));
	    }

	    protected void InvokeRepeating(Expression<Action> expr, float time, float repeatRate)
	    {
	        InvokeRepeating(GetMethodName(expr), time, repeatRate);
	    }

	    #endregion
    }
}
