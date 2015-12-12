using System;

namespace ClutteredMarkov
{
    public static class MarkovGenerator
    {
        /// <summary>
        /// Creates a Markov Chain sentence based on the text that was fed
        /// </summary>
        public static string Create()
        {
            int chainLength = RandomGenerator.GetRandomNumber(100, 200);
            return Create(chainLength);
        }

        private static string GetNextWord(int wordKey)
        {
            try
            {
                int selection = RandomGenerator.GetRandomNumber(0, Markov.NextWords[wordKey].Count - 1);
                return Markov.NextWords[wordKey][selection];
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
            if (Markov.Words.Count == 0)
            {
                return "";
            }
            int beginning = RandomGenerator.GetRandomNumber(0, Markov.Words.Count - 1);
            string markovSentence = "";
            string nextWord = "";

            do
            {
                if (markovSentence == "")
                {
                    markovSentence = Markov.Words[beginning] + " ";
                    nextWord = GetNextWord(beginning);
                }
                else
                {
                    try
                    {
                        int currentNextWordKey = Markov.FindKey(nextWord);
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
