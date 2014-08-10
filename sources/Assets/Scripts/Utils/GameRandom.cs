using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Utils
{
	public static class GameRandom
	{
		public static void Shuffle<T>(this List<T> list)
		{
			T[] array = list.ToArray();
			ShuffleArray(array);
			list.Clear();
			list.AddRange(array);
		}

		public static void ShuffleArray<T>(T[] array)
		{
			T[] source = new T[array.Length];
			Array.Copy(array, source, array.Length);
			for (int i = 1; i < array.Length; i++)
			{
				int indRnd = Random.Range(0, i + 1);
				array[i] = array[indRnd];
				array[indRnd] = source[i];
			}
		}

		public static T NextEnum<T>()
		{
			var values = Enum.GetValues(typeof(T));
			return (T)values.GetValue(Random.Range(0, values.Length));
		}

	    public static float NextDiscrete(float min, float max, int count)
	    {
	        if (count < 2) return min;
	        return min + Random.Range(0, count)*(max - min)/(count - 1);
	    }

        public static int NextWeightedInd(int[] weights)
        {
            int randomInt = Random.Range(1, weights.Sum() + 1);
            int sum = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                sum += weights[i];
                if (randomInt <= sum)
                    return i;
            }
            throw new Exception("Logic error!");
        }

	}
}
