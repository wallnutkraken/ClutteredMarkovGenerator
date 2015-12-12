using System;

namespace ClutteredMarkov
{
    internal static class RandomGenerator
    {
        /* Used for locking the thread. See below. */
        private static object locker = new object();
        /* This Random object has a seed. A seed is used to make the number */
        /* a random number generator produces more random. In this specific scenario, */
        /* we use seeds twice. Here is the first use. The Random object has a overloaded */
        /* constructor that takes an int value as the seed. This specific seed is simply */
        /* how many miliseconds have elapsed since the system was started */
        private static Random seedGenerator = new Random(Environment.TickCount);

        /// <summary>
        /// A more spammable random number generator than Random.Next(). Uses a seed.
        /// </summary>
        /// <param name="minVal">Minimum value of return int</param>
        /// <param name="maxVal">Maximum value of return int</param>
        internal static int GetRandomNumber(int minVal, int maxVal)
        {
            int seed;

            /* The lock keyword locks this block of code from any other thread messing with it. */
            /* Not that we do have any other threads here, but there could be eventually. */
            lock (locker)
            {
                /* Here we use the seed a second time. We use the first random number generator */
                /* (Random object) to generate a number that can lie anywhere in the Int32 range, */
                /* int.MinValue and int.MaxValue. */
                seed = seedGenerator.Next(int.MinValue, int.MaxValue);
            }

            /* Here we use the seed we generated with the first seeded Random object to */
            /* create a new seeded Random object. This should be /pretty/ random. */
            Random random = new Random(seed);

            /* And generate the value based on the min and max values passed to the method */
            return random.Next(minVal, maxVal);
        }
    }
}
