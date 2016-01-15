using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ClutteredMarkov
{

    public class Markov
    {
        //internal Dictionary<int, string> Words = new Dictionary<int, string>();
        //internal Dictionary<int, List<string>> NextWords = new Dictionary<int, List<string>>();

        internal Dictionary<string, List<string>> ChainState { get; set; } = new Dictionary<string, List<string>>();

        /// <summary>
        /// Adds a new word to the dictionary
        /// </summary>
        private void AddNewWord(string word)
        {
            if (ChainState.ContainsKey(word))
            {
                return;
            }
            else
            {
                ChainState.Add(word, new List<string>());
                return;
            }
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

        /// <summary>
        /// Saves the Markov chain information to the binary files words.cmc and nextwords.cmc
        /// </summary>
        public void SaveChainState(string name)
        {
            SaveChainState(name, false);
        }

        /// <summary>
        /// Saves the Markov chain information to the binary files words.cmc and nextwords.cmc
        /// </summary>
        /// <param name="ignoreFailsafe">If true, ignores failsafe for writing empty object</param>
        public void SaveChainState(string name, bool ignoreFailsafe)
        {
            byte[] state = ObjectToByteArray(ChainState);
            if (File.Exists(name + ".bin") && ignoreFailsafe == false)
            {
                FileInfo info = new FileInfo(name + ".bin");
                if (info.Length <= state.Length)
                {
                    File.WriteAllBytes(name + ".bin", state);
                    return;
                }
                else
                {
                    return;
                }
            }
            File.WriteAllBytes(name + ".bin", state);
        }

        /// <summary>
        /// Loads the binary information of the Markov chain from the words.cmc and nextwords.cmc files
        /// </summary>
        public void LoadChainState(string name)
        {
            object words = ByteArrayToObject(File.ReadAllBytes(name + ".bin"));
            ChainState = (Dictionary<string, List<string>>)words;
        }

        /// <summary>
        /// Feeds a text line to the markov chain
        /// </summary>
        public void Feed(string line)
        {
            line = line.Replace('\n', ' ');
            line = line.Replace(".", "").Replace(",", "");
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
                if (ChainState.ContainsKey(words[i]))
                {
                    string key = words[i]; /* doing this because I'm too lazy to rewrite too much :P */
                    if (i == words.Length - 1)
                    {
                        /* If it's the last entry, add a null (which is interpreted as an END signal) */
                        ChainState[key].Add(null);
                    }
                    else if (ChainState[key].Count == 0)
                    {
                        ChainState[key].Add(words[i + 1]);
                    }
                    else if (ChainState[key].Contains(words[i + 1]) == false)
                    {
                        ChainState[key].Add(words[i + 1]);
                    }

                }
                else
                {
                    AddNewWord(words[i]);
                    string key = words[i];
                    if (i == words.Length - 1)
                    {
                        /* If it's the last entry, add a null (which is interpreted as an END signal) */
                        ChainState[key].Add(null);
                    }
                    else if (ChainState[key].Contains(words[i + 1]) == false)
                    {
                        ChainState[key].Add(words[i + 1]);
                    }
                }
            }
        }
    }
}
