using System;
using System.Collections.Generic;
using System.Linq;

namespace ClutteredMarkov
{
    public static class MarkovGenerator
    {
        private static Markov PassedChain;
        /// <summary>
        /// Creates a Markov Chain sentence based on the text that was fed
        /// </summary>
        public static string Create(Markov chain)
        {
            int chainLength = RandomGenerator.GetRandomNumber(100, 200);
            return Create(chainLength, chain);
        }

        private static IEnumerable<TValue> RandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict)
        {
            List<TValue> values = Enumerable.ToList(dict.Values);
            int size = dict.Count;
            while (true)
            {
                yield return values[RandomGenerator.GetRandomNumber(0, size)];
            }
        }

        private static string GetNextWord(string word)
        {
            try
            {
                int selection = RandomGenerator.GetRandomNumber(0, PassedChain.ChainState[word].Count - 1);
                return PassedChain.ChainState[word][selection];
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// Creates a Markov Chain sentence of a specified length based on the text that was fed
        /// </summary>
        /// <param name="chainLength">The amount of characters the chain should contain at the maximum</param>
        public static string Create(int chainLength, Markov chain)
        {
            PassedChain = chain;
            if (PassedChain.ChainState.Count == 0)
            {
                return "";
            }
            int beginning = RandomGenerator.GetRandomNumber(0, PassedChain.ChainState.Count - 1);
            string markovSentence = "";
            string nextWord = "";

            do
            {
                if (markovSentence == "")
                {
                    string chainBit = "";
                    foreach (var value in RandomValues(PassedChain.ChainState).Take(1))
                    {
                        chainBit = PassedChain.ChainState.FirstOrDefault(x => x.Value == value).Key;
                    }

                    markovSentence = chainBit + " ";
                    nextWord = GetNextWord(chainBit);
                }
                else
                {
                    try
                    {
                        nextWord = GetNextWord(nextWord);
                    }
                    catch (Exception)
                    {
                        return markovSentence;
                    }
                    if (nextWord == null)
                    {
                        return markovSentence;
                    }
                    if (markovSentence.Length + nextWord.Length <= chainLength)
                    {
                        if (markovSentence.Length + nextWord.Length + 1 <= chainLength)
                        {
                            markovSentence += nextWord + " ";
                        }
                        else
                        {
                            return markovSentence += nextWord;
                        }
                    }
                    else
                    {
                        return markovSentence;
                    }
                }

            } while ((markovSentence + nextWord).Length <= chainLength);
            PassedChain = null;
            return "";
        }
    }
}
