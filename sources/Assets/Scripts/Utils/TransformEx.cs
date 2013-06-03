using UnityEngine;

namespace Assets.Scripts.Utils
{
	public static class TransformEx
	{
		public static void SetPosition(this Transform transform, float x, float y, float z)
		{
			transform.position = new Vector3(x, y, z);
		}

		public static void SetPosition(this Transform transform, float x, float y)
		{
			transform.position = new Vector3(x, y, transform.position.z);
		}

		public static void SetPosition(this Transform transform, Vector2 point, float z)
		{
			transform.position = new Vector3(point.x, point.y, z);
		}

		public static void SetPosition(this Transform transform, Vector2 point)
		{
			transform.position = new Vector3(point.x, point.y, transform.position.z);
		}

		public static Vector2 Position2(this Transform transform)
		{
			return transform.position;
		}

		public static void SetX(this Transform transform, float x)
		{
			transform.position = new Vector3(x, transform.position.y, transform.position.z);
		}

		public static void SetY(this Transform transform, float y)
		{
			transform.position = new Vector3(transform.position.x, y, transform.position.z);
		}

		public static void SetZ(this Transform transform, float z)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, z);
		}

		public static void SetLocalX(this Transform transform, float x)
		{
			transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
		}

		public static void SetLocalY(this Transform transform, float y)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
		}

		public static void SetLocalZ(this Transform transform, float z)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
		}

		public static void IncLocalX(this Transform transform, float dx)
		{
			transform.localPosition = new Vector3(transform.localPosition.x + dx, transform.localPosition.y, transform.localPosition.z);
		}

		public static void IncLocalY(this Transform transform, float dy)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + dy, transform.localPosition.z);
		}

		public static void IncLocalZ(this Transform transform, float dz)
		{
			transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + dz);
		}




		public static void SetRotation(this Transform transform, float angle)
		{
			transform.rotation = new Quaternion();
			transform.Rotate(Vector3.forward, angle);
		}

	}
}
