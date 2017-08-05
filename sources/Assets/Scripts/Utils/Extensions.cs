using System;
using System.Linq.Expressions;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    public static class Extensions
    {
        #region Invoke

        private static string GetMethodName(Expression<Action> expr)
        {
            return ((MethodCallExpression)expr.Body).Method.Name;
        }

        public static void Invoke(this MonoBehaviour monoBehaviour, Expression<Action> expr, float time)
        {
            monoBehaviour.Invoke(GetMethodName(expr), time);
        }

        public static bool IsInvoking(this MonoBehaviour monoBehaviour, Expression<Action> expr)
        {
            return monoBehaviour.IsInvoking(GetMethodName(expr));
        }

        public static void CancelInvoke(this MonoBehaviour monoBehaviour, Expression<Action> expr)
        {
            monoBehaviour.CancelInvoke(GetMethodName(expr));
        }

        public static void InvokeRepeating(this MonoBehaviour monoBehaviour, Expression<Action> expr, float time, float repeatRate)
        {
            monoBehaviour.InvokeRepeating(GetMethodName(expr), time, repeatRate);
        }

        #endregion

        #region Vector2

        public static Vector2 MakePixelPerfect(this Vector2 position)
        {
            return new Vector2((int)position.x, (int)position.y);
        }

        public static Vector2 Rotate(this Vector2 vector, float angle)
        {
            return Quaternion.AngleAxis(angle, Vector3.forward) * vector;
        }

        public static float Angle(this Vector2 direction)
        {
            return direction.y > 0
                       ? Vector2.Angle(new Vector2(1, 0), direction)
                       : -Vector2.Angle(new Vector2(1, 0), direction);
        }

        #endregion

        #region Color

        public static Color SetA(this Color color, float a)
        {
            return new Color(color.r, color.g, color.b, a);
        }

        #endregion

        #region Transform.position

        public static void SetXYZ(this Transform transform, float x, float y, float z)
        {
            transform.position = new Vector3(x, y, z);
        }

        public static void SetLocalXYZ(this Transform transform, float x, float y, float z)
        {
            transform.localPosition = new Vector3(x, y, z);
        }

        public static void SetXY(this Transform transform, float x, float y)
        {
            transform.position = new Vector3(x, y, transform.position.z);
        }

        public static void SetLocalXY(this Transform transform, float x, float y)
        {
            transform.localPosition = new Vector3(x, y, transform.localPosition.z);
        }

        public static void SetX(this Transform transform, float x)
        {
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        public static void SetLocalX(this Transform transform, float x)
        {
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        }

        public static void SetY(this Transform transform, float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        public static void SetLocalY(this Transform transform, float y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }

        public static void SetZ(this Transform transform, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }

        public static void SetLocalZ(this Transform transform, float z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        }

        public static Vector2 Position2(this Transform transform)
        {
            return transform.position;
        }

        public static void IncLocalX(this Transform transform, float dx)
        {
            transform.localPosition = new Vector3(transform.localPosition.x + dx, transform.localPosition.y,
                                                  transform.localPosition.z);
        }

        public static void IncLocalY(this Transform transform, float dy)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + dy,
                                                  transform.localPosition.z);
        }

        public static void IncLocalZ(this Transform transform, float dz)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y,
                                                  transform.localPosition.z + dz);
        }

        #endregion

        #region Transform.localScale

        public static void SetScaleX(this Transform transform, float scaleX)
        {
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }

        public static void SetScaleY(this Transform transform, float scaleY)
        {
            transform.localScale = new Vector3(transform.localScale.x, scaleY, transform.localScale.z);
        }

        public static void SetScaleZ(this Transform transform, float scaleZ)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, scaleZ);
        }

        #endregion

        #region Transform.rotation

        public static void SetRotation(this Transform transform, float angle)
        {
            transform.rotation = new Quaternion();
            transform.Rotate(Vector3.forward, angle);
        }

        #endregion
    }

}
