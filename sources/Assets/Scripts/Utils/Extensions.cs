using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Utils
{
	public static class Extensions
	{		
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

		public static Color SetA(this Color color, float a)
		{
			return new Color(color.r, color.g, color.b, a);
		}

		public static void Rewrite<T>(this ICollection<T> collection, IEnumerable<T> newCollection)
		{
			collection.Clear();
			foreach (T item in newCollection)
				collection.Add(item);
		}
	}

}
