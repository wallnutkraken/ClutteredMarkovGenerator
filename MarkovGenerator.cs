using System;

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
            PassedChain = chain;
            string genChain = Create(chainLength);
            PassedChain = null;
            return genChain;
        }

        private static string GetNextWord(int wordKey)
        {
            try
            {
                int selection = RandomGenerator.GetRandomNumber(0, PassedChain.NextWords[wordKey].Count - 1);
                return PassedChain.NextWords[wordKey][selection];
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
        public static string Create(int chainLength)
        {
            if (PassedChain.Words.Count == 0)
            {
                return "";
            }
            int beginning = RandomGenerator.GetRandomNumber(0, PassedChain.Words.Count - 1);
            string markovSentence = "";
            string nextWord = "";

            do
            {
                if (markovSentence == "")
                {
                    markovSentence = PassedChain.Words[beginning] + " ";
                    nextWord = GetNextWord(beginning);
                }
                else
                {
                    try
                    {
                        int currentNextWordKey = PassedChain.FindKey(nextWord);
                        nextWord = GetNextWord(currentNextWordKey);
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
            return "";
        }
    }
}
