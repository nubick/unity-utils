using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Utils;
using NUnit.Framework;
using UnityEngine;

namespace Assets.Scripts.Tests.Editor
{
	[TestFixture]
	public class RandomUtilTest
	{
		private const int Attempts = 10000;
		
		[Test]
		public void NextBoolTest()
		{
			int counter = 0;
			int iterations = 1000;
			for (int i = 0; i < iterations; i++)
			{
				bool randomBool = RandomUtil.NextBool();
				if (randomBool)
					counter++;
			}
			Console.WriteLine($"NextBool: {counter} from {iterations}");
		}

		#region NextWeightedInd

		[Test]
		public void NextWeightedIndTest1()
		{
			int[] weights = { 10, 0 };
			for (int attempt = 1; attempt <= Attempts; attempt++)
			{
				int ind = RandomUtil.NextWeightedInd(weights);
				Assert.AreEqual(0, ind);
			}
		}

		[Test]
		public void NextWeightedIndTest2()
		{
			int[] weights = { 0, 100 };
			for (int attempt = 1; attempt <= Attempts; attempt++)
			{
				int ind = RandomUtil.NextWeightedInd(weights);
				Assert.AreEqual(1, ind);
			}
		}

		[Test]
		public void NextWeightedIndTest3()
		{
			int[] weights = { 0, 100, 0, 0, 200 };
			for (int attempt = 1; attempt <= Attempts; attempt++)
			{
				int ind = RandomUtil.NextWeightedInd(weights);
				Assert.IsTrue(ind == 1 || ind == 4);
			}
		}

		[Test]
		public void NextWeightedIndTest4()
		{
			float[] weights = { 1f, 0f };
			for (int attempt = 1; attempt <= Attempts; attempt++)
			{
				int ind = RandomUtil.NextWeightedInd(weights);
				Assert.AreEqual(0, ind);
			}
		}

		[Test]
		public void NextWeightedIndTest5()
		{
			float[] weights = { 1f, 0f, 2f, 0f, 3f };
			for (int attempt = 1; attempt <= Attempts; attempt++)
			{
				int ind = RandomUtil.NextWeightedInd(weights);
				Assert.IsTrue(ind == 0 || ind == 2 || ind == 4);
			}
		}

		#endregion

		#region NextPointOnLine

		[Test]
		public void NextPointOnLineTest1()
		{
			Vector2 point1 = new Vector2(0f, 0f);
			Vector2 point2 = new Vector2(10f, 10f);
			for (int attempt = 1; attempt <= Attempts; attempt++)
			{
				Vector2 randomPoint = RandomUtil.NextPointOnLine(point1, point2);
				Assert.AreEqual(randomPoint.x, randomPoint.y);
			}
		}

		[Test]
		public void NextPointOnLineTest2()
		{
			Vector2 point1 = new Vector2(-123f, 43f);
			Vector2 point2 = new Vector2(98f, 22.54f);
			float distance = Vector2.Distance(point1, point2);
			for (int attempt = 1; attempt <= Attempts; attempt++)
			{
				Vector2 randomPoint = RandomUtil.NextPointOnLine(point1, point2);
				float distance1 = Vector2.Distance(point1, randomPoint);
				float distance2 = Vector2.Distance(point2, randomPoint);
				Assert.LessOrEqual(Mathf.Abs(distance - (distance1 + distance2)), 0.001f);
			}
		}

		[Test]
		public void NextPointOnLineTest3()
		{
			Vector3 point1 = new Vector3(UnityEngine.Random.Range(-123f, -200f), UnityEngine.Random.Range(43f, 143f), UnityEngine.Random.Range(-100f, 100f));
			Vector3 point2 = new Vector3(UnityEngine.Random.Range(98f, 198f), UnityEngine.Random.Range(22.54f, 300f), UnityEngine.Random.Range(-200f, -100f));
			float distance = Vector3.Distance(point1, point2);
			for (int attempt = 1; attempt <= Attempts; attempt++)
			{
				Vector3 randomPoint = RandomUtil.NextPointOnLine(point1, point2);
				float distance1 = Vector3.Distance(point1, randomPoint);
				float distance2 = Vector3.Distance(point2, randomPoint);
				Assert.LessOrEqual(Mathf.Abs(distance - (distance1 + distance2)), 0.001f);
			}
		}

		#endregion

		[Test]
		public void GetChanceTest1()
		{
			int oneChanceAmount = 0;
			int halfChanceAmount = 0;
			for (int attempt = 1; attempt <= Attempts; attempt++)
			{
				if (RandomUtil.GetChance(0))
					Assert.Fail("GetChance(0) is true");

				if (RandomUtil.GetChance(-23))
					Assert.Fail("GetChance(-23) is true.");

				if (!RandomUtil.GetChance(100))
					Assert.Fail("GetChance(100) is false.");

				if (!RandomUtil.GetChance(123))
					Assert.Fail("GetChance(123) is false.");

				if (RandomUtil.GetChance(1))
					oneChanceAmount++;

				if (RandomUtil.GetChance(50))
					halfChanceAmount++;
			}

			Assert.IsTrue(Mathf.Abs(oneChanceAmount - Attempts / 100) < Attempts / 100, string.Format("oneChanceAmount:{0}", oneChanceAmount));
			Assert.IsTrue(Mathf.Abs(halfChanceAmount - Attempts / 2) < Attempts / 100, "halfChanceAmount:" + halfChanceAmount);
		}

		[Test]
		public void GetChanceTest2()
		{
			int oneChanceAmount = 0;
			int halfChanceAmount = 0;
			for (int attempt = 1; attempt <= Attempts; attempt++)
			{
				if (RandomUtil.GetChance(0.0f))
					Assert.Fail("GetChance(0) is true");

				if (RandomUtil.GetChance(-0.23f))
					Assert.Fail("GetChance(-23) is true.");

				if (!RandomUtil.GetChance(1.0f))
					Assert.Fail("GetChance(100) is false.");

				if (!RandomUtil.GetChance(1.23f))
					Assert.Fail("GetChance(123) is false.");

				if (RandomUtil.GetChance(0.01f))
					oneChanceAmount++;

				if (RandomUtil.GetChance(0.5f))
					halfChanceAmount++;
			}

			Assert.IsTrue(Mathf.Abs(oneChanceAmount - Attempts / 100) < Attempts / 100, string.Format("oneChanceAmount:{0}", oneChanceAmount));
			Assert.IsTrue(Mathf.Abs(halfChanceAmount - Attempts / 2) < Attempts / 100, "halfChanceAmount:" + halfChanceAmount);
		}

		[Test]
		public void NextPointOnRectTest()
		{
			Func<float> randomFloat = () => UnityEngine.Random.Range(-1000f, 1000f);

			int rectIterations = 100;
			for (int j = 0; j < rectIterations; j++)
			{
				Rect rect = new Rect(randomFloat(), randomFloat(), randomFloat() + 1000f, randomFloat() + 1000f);

				int iterations = 1000;
				for (int i = 0; i < iterations; i++)
				{
					Vector2 point = RandomUtil.NextPointOnRectBorder(rect);

					bool leftOrRight = point.x == rect.xMin || point.x == rect.xMax;
					bool topOrBottom = point.y == rect.yMin || point.y == rect.yMax;

					bool onLeftOrRightBorder = leftOrRight && point.y <= rect.yMax && point.y >= rect.yMin;
					bool onTopOrBottomBorder = topOrBottom && point.x <= rect.xMax && point.x >= rect.xMin;

					bool isOk = onLeftOrRightBorder || onTopOrBottomBorder;

					if (!isOk)
						Debug.Log("rect: " + rect + ", point: " + point);

					Assert.IsTrue(onLeftOrRightBorder || onTopOrBottomBorder);
				}
			}
		}
		
		[Test]
		public void TakeTest()
		{
			int[] deck = Enumerable.Range(1, 52).ToArray();
			int[] frequency = new int[52];
			Debug.Log(string.Join(",", deck));
			int count = 23;
			int iterations = 100000;
			for (int i = 0; i < iterations; i++)
			{
				List<int> hand = RandomUtil.Take(deck, count);
				hand.ForEach(_ => frequency[_ - 1]++);
				Assert.AreEqual(count, hand.Count);
				Assert.AreEqual(hand.Count, hand.Distinct().Count());
			}
			int average = frequency.Sum() / frequency.Length;
			float[] variance = frequency.Select(_ => Mathf.Abs((_ - average)*1f/average)).ToArray();
			Debug.Log(string.Join(",", frequency));
			Debug.Log(string.Join(",", variance));
		}
	}
}
