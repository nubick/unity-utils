using System;
using Assets.Scripts.Utils;
using NUnit.Framework;

namespace Assets.Scripts.Tests.Editor
{
	[TestFixture]
	public class RandomUtilTest
	{
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
            Console.WriteLine(string.Format("NextBool: {0} from {1}", counter, iterations));
		}

	    [Test]
	    public void NextWeightedIndTest1()
	    {
	        int[] weights = {10, 0};
	        for (int attempt = 1; attempt <= 100; attempt++)
	        {
	            int ind = RandomUtil.NextWeightedInd(weights);
	            Assert.AreEqual(0, ind);
	        }
	    }

        [Test]
        public void NextWeightedIndTest2()
        {
            int[] weights = { 0, 100 };
            for (int attempt = 1; attempt <= 100; attempt++)
            {
                int ind = RandomUtil.NextWeightedInd(weights);
                Assert.AreEqual(1, ind);
            }
        }

        [Test]
        public void NextWeightedIndTest3()
        {
            int[] weights = {0, 100, 0, 0, 200};
            for (int attempt = 1; attempt <= 100; attempt++)
            {
                int ind = RandomUtil.NextWeightedInd(weights);
                Assert.IsTrue(ind == 1 || ind == 4);
            }
        }

	}
}
