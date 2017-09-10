using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Utils
{
	public static class RandomUtil
	{
		/// <summary>
		/// Return random bool value.
		/// </summary>
		public static bool NextBool()
		{
			return Random.Range(0, 2) == 0;
		}

		/// <summary>
		/// Return random item from item1 and item2 set.
		/// </summary>
		public static T Next<T>(T item1, T item2)
		{
			return NextBool() ? item1 : item2;
		}

		/// <summary>
		/// Return random item from item1, item2 and item3 set.
		/// </summary>
		public static T Next<T>(T item1, T item2, T item3)
		{
			int n = Random.Range(0, 3);
			return n == 0 ? item1 : (n == 1 ? item2 : item3);
		}

		/// <summary>
		/// Return random item from array.
		/// </summary>
		public static T NextItem<T>(T[] array)
		{
			return array[Random.Range(0, array.Length)];
		}

		/// <summary>
		/// Return random item from list.
		/// </summary>
		public static T NextItem<T>(List<T> list)
		{
			return list[Random.Range(0, list.Count)];
		}

        /// <summary>
        /// Return random enum item.
        /// </summary>
		public static T NextEnum<T>()
		{
			var values = Enum.GetValues(typeof(T));
			return (T)values.GetValue(Random.Range(0, values.Length));
		}

        /// <summary>
        /// Return random index of passed array. Index random selection is based on array weights.
        /// </summary>
		public static int NextWeightedInd(int[] weights)
		{
            int randomPoint = Random.Range(0, weights.Sum());
			int sum = 0;
			for (int i = 0; i < weights.Length; i++)
			{
				sum += weights[i];
				if (randomPoint <= sum)
					return i;
			}
			throw new Exception("Logic error!");
		}

        /// <summary>
        /// Return random index of passed array. Index random selection is based on array weights.
        /// </summary>
		public static int NextWeightedInd(float[] weights)
		{
			float randomPoint = Random.Range(0f, weights.Sum());
			float sum = 0f;
			for (int i = 0; i < weights.Length; i++)
			{
				sum += weights[i];
				if (randomPoint <= sum)
					return i;
			}
			throw new Exception("Logic error!");
		}

		/// <summary>
		/// Return sub-list of random items from origin list without repeating.
		/// </summary>
		public static List<T> Take<T>(List<T> list, int count)
		{
			List<T> items = new List<T>();
			List<int> remainedIndexes = Enumerable.Range(0, list.Count).ToList();
			for (int i = 0; i < count; i++)
			{
				int selectedIndex = NextItem(remainedIndexes);
				remainedIndexes.Remove(selectedIndex);
				items.Add(list[selectedIndex]);
			}
			return items;
		}

		/// <summary>
		/// Shuffle list of items.
		/// </summary>
		public static void Shuffle<T>(this List<T> list)
		{
            for (int i = 1; i < list.Count; i++)
			{
				int indRnd = Random.Range(0, i + 1);
				T temp = list[i];
				list[i] = list[indRnd];
				list[indRnd] = temp;
			}
		}

        /// <summary>
        /// Shuffle array of items.
        /// </summary>
		public static void Shuffle<T>(T[] array)
		{
			for (int i = 1; i < array.Length; i++)
			{
				int indRnd = Random.Range(0, i + 1);
                T temp = array[i];
				array[i] = array[indRnd];
				array[indRnd] = temp;
			}
		}
	}
}
