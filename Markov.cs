using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace ClutteredMarkov
{
    public class Markov
    {
        internal Dictionary<int, string> Words = new Dictionary<int, string>();
        internal Dictionary<int, List<string>> NextWords = new Dictionary<int, List<string>>();

        internal int FindKey(string word)
        {
            if (Words.ContainsValue(word))
            {
                return Words.FirstOrDefault(x => x.Value == word).Key;
            }
            else
            {
                return 0;
            }
        }
        /// <summary>
        /// Adds a new word to the dictionary and returns it's key
        /// </summary>
        private int AddNewWord(string word)
        {
            if (Words.ContainsValue(word))
            {
                return Words.FirstOrDefault(x => x.Value == word).Key;
            }
            if (Words.Count == 0)
            {
                Words.Add(0, word);
                return 0;
            }
            int i = 0;
            while (Words.ContainsKey(i))
            {
                i++;
            }
            Words.Add(i, word);
            return i;
        }

        private static object ByteArrayToObject(byte[] readArray)
        {
            using (var memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(readArray, 0, readArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                Object obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        private static byte[] ObjectToByteArray(Object obj)
        {
            BinaryFormatter bin = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bin.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        [Obsolete("Please only use this to load old files into memory. Save them using SaveChainState from now on.")]
        public void Load()
        {
            object words = ByteArrayToObject(File.ReadAllBytes("words.cmc"));
            Words = (Dictionary<int, string>)words;
            object nextWords = ByteArrayToObject(File.ReadAllBytes("nextwords.cmc"));
            NextWords = (Dictionary<int, List<string>>)nextWords;
        }

        /// <summary>
        /// Saves the Markov chain information to the binary files words.cmc and nextwords.cmc
        /// </summary>
        public void SaveChainState(string name)
        {
            byte[] words = ObjectToByteArray(Words);
            byte[] nextWords = ObjectToByteArray(NextWords);

            File.WriteAllBytes(name + "_words.cmc", words);
            File.WriteAllBytes(name + "_nextwords.cmc", nextWords);
        }

        /// <summary>
        /// Loads the binary information of the Markov chain from the words.cmc and nextwords.cmc files
        /// </summary>
        public void LoadChainState(string name)
        {
            object words = ByteArrayToObject(File.ReadAllBytes(name + "_words.cmc"));
            Words = (Dictionary<int, string>)words;
            object nextWords = ByteArrayToObject(File.ReadAllBytes(name + "_nextwords.cmc"));
            NextWords = (Dictionary<int, List<string>>)nextWords;
        }

        /// <summary>
        /// Feeds a text line to the markov chain
        /// </summary>
        public void Feed(string line)
        {
            line = line.Replace('\n', ' ');
            /* Removes spaces that occur more than once */
            try
            {
                line = string.Join(" ", line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries));
            }
            catch (Exception) { }

            char[] splitters = { ' ', '/', '\\' };
            string[] words = line.Split(splitters);

            if (words.Length == 1)
            {
                AddNewWord(line.Trim(' '));
                return;
            }

            for (int i = 0; i < words.Length; i++)
            {
                if (Words.ContainsValue(words[i]))
                {
                    int key = Words.FirstOrDefault(x => x.Value == words[i]).Key;
                    if (i == words.Length - 1)
                    {
                        /* If it's the last entry, add a null (which is interpreted as an END signal) */
                        if (NextWords.ContainsKey(key) == false)
                        {
                            List<string> nextWords = new List<string>();
                            nextWords.Add(null);
                            NextWords.Add(key, nextWords);
                        }
                        NextWords[key].Add(null);
                    }
                    else if (NextWords.ContainsKey(key) == false)
                    {
                        List<string> nextWords = new List<string>();
                        nextWords.Add(words[i + 1]);
                        NextWords.Add(key, nextWords);
                    }
                    else if (NextWords[key].Contains(words[i + 1]) == false)
                    {
                        NextWords[key].Add(words[i + 1]);
                    }

                }
                else
                {
                    AddNewWord(words[i]);
                    int key = Words.FirstOrDefault(x => x.Value == words[i]).Key;
                    if (NextWords.ContainsKey(key) == false)
                    {
                        NextWords.Add(key, new List<string>());
                    }
                    if (i == words.Length - 1)
                    {
                        /* If it's the last entry, add a null (which is interpreted as an END signal) */
                        NextWords[key].Add(null);
                    }
                    else if (NextWords[key].Contains(words[i + 1]) == false)
                    {
                        NextWords[key].Add(words[i + 1]);
                    }
                }
            }
        }
    }
}
