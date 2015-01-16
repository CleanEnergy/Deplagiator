using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmCollection
{
    public class StringMatching : Algorithm
    {
        /// <summary>
        /// Takes non-formatted raw texts and compares them word-by-word.
        /// </summary>
        /// <param name="leftContent">The "left" side to compare to.</param>
        /// <param name="rightContent">The "right" side to compare to.</param>
        /// <returns>An AlgorithmResult holding all the matching words and the average percent of match.</returns>
        public static StringMatchingResult Start(string leftContent, string rightContent, char[] separators)
        {
            StringMatchingResult result = new StringMatchingResult();

            // Remove special characters
            leftContent = RemoveSpecialCharacters(leftContent);
            rightContent = RemoveSpecialCharacters(rightContent);

            // Split on each word
            string[] whitespaceSplitLeft = leftContent.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            string[] whitespaceSplitRight = rightContent.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            // Arrange left content into hashtable
            // NOTE: lowercase each word
            Hashtable hashTable = new Hashtable();
            for (int i = 0; i < whitespaceSplitLeft.Length; i++)
            {
                if (!hashTable.ContainsValue(whitespaceSplitLeft[i]))
                {
                    hashTable.Add(i, whitespaceSplitLeft[i].ToLower());
                }
            }

            // For each word in the left content look, if there exists a word in right content. If it does, add to the result.
            // NOTE: lowercase each word when checking
            for (int i = 0; i < whitespaceSplitRight.Length; i++)
            {
                if (hashTable.ContainsValue(whitespaceSplitRight[i].ToLower()))
                {
                    result.MatchingWords.Add(whitespaceSplitRight[i]);
                }
            }

            // Calculate the percent match
            result.PercentMatch = ((float)result.MatchingWords.Count / (float)hashTable.Count) * 100;

            return result;
        }

        #region String matching helpers
        private static string RemoveSpecialCharacters(string content)
        {
            string result = content;

            result = result.Replace(".", string.Empty);
            result = result.Replace(",", string.Empty);
            result = result.Replace("(", string.Empty);
            result = result.Replace(")", string.Empty);

            return result;
        }
        #endregion
    }

    public class StringMatchingResult : AlgorithmResult
    {
        public List<string> MatchingWords { get; set; }

        public float PercentMatch { get; set; }

        public StringMatchingResult()
        {
            MatchingWords = new List<string>();
        }
    }
}