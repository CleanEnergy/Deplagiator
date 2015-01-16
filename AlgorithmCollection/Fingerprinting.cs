using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace AlgorithmCollection
{
    public class Fingerprinting : Algorithm
    {
        /// <summary>
        /// Compares the left print to the right print.
        /// </summary>
        /// <param name="leftPrint">The prints to be checked.</param>
        /// <param name="rightPrint">The prints to be compared to.</param>
        /// <returns>A FingerprintingAlgorithmResult containing the matched prints and the percent match of the right print to the left print.</returns>
        public static FingerprintingResult Compare(string[] leftPrint, string[] rightPrint)
        {
            FingerprintingResult result = new FingerprintingResult();

            int counter = leftPrint.Length < rightPrint.Length ? leftPrint.Length : rightPrint.Length;
            for (int i = 0; i < counter; i++)
            {
                if (leftPrint[i] == rightPrint[i])
                {
                    result.MatchedPrints.Add(leftPrint[i]);
                }
            }

            // The left prints are compared to the right prints -> (number of matched prints / number of left prints) * 100
            result.PercentMatch = ((float)result.MatchedPrints.Count / (float)leftPrint.Length) * 100;

            return result;
        }

        /// <summary>
        /// Splits the text into nGrams, computes a hash for each nGram and stores them into an array.
        /// </summary>
        /// <param name="nGram">The size of the nGram.</param>
        /// <param name="text">The raw text to operate on.</param>
        /// <param name="separatorCharacters">The characters to split the text on.</param>
        /// <returns>An array of hashes that represents the print of the text.</returns>
        public static string[] ComputePrint(int nGram, string text, params char[] separatorCharacters)
        {
            List<string> print = new List<string>();

            string[] split = text.Split(separatorCharacters, StringSplitOptions.RemoveEmptyEntries);

            int nGramCounter = 0;
            List<string> tempPrint = new List<string>();
            for (int i = 0; i < split.Length; i = i + nGram)
            {
                for (int j = 0; j < nGram; j++, nGramCounter++)
                {
                    if (nGramCounter < split.Length)
                    {
                        tempPrint.Add(split[nGramCounter]);
                    }
                    else
                    {
                        break;
                    }
                }

                string hash = ComputeHash(tempPrint);
                print.Add(hash);
                tempPrint.Clear();
            }

            return print.ToArray();
        }

        private static string ComputeHash(List<string> tempPrint)
        {
            string nGram = string.Empty;
            for (int i = 0; i < tempPrint.Count(); i++)
            {
                nGram += tempPrint[i];
            }

            Hash h = Hash.CreateSHA256(GetBytes(nGram));
            return BitConverter.ToString(h.SHA256);
        }

        #region Helpers
        private static byte[] GetBytes(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        #endregion
    }

    public class FingerprintingResult : AlgorithmResult
    {
        public List<string> MatchedPrints { get; set; }

        public float PercentMatch { get; set; }

        public FingerprintingResult()
        {
            MatchedPrints = new List<string>();
        }
    }


}
