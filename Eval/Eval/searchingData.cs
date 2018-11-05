using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eval
{
    class searchingData
    {
        static private string[] docArray;
        static private string[] dictionaryArray;
        static private string[] postingArray;

        private int termID { get; set; }

        public searchingData()
        {
            docArray = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\titlesAndAbstracts.txt").ToArray();
            dictionaryArray = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\dictionary.txt").ToArray();
            postingArray = File.ReadAllLines(AppDomain.CurrentDomain.BaseDirectory + "\\postings.txt").ToArray();

        }
        public SortedDictionary<double, List<int>> FindFinalVectors(int[] QueryVector, string[] QueryTerms)
        {
            //So the first int is document ID, and the second is an array for each term and it's frequency
            SortedDictionary<int, double[]> DocVectors = new SortedDictionary<int, double[]>();
            SortedDictionary<string, int> dictionaryMap = new SortedDictionary<string, int>();
            List<string> test = new List<string>();

            string CurrTerm;
            int CurrFreq;
            //Had to format it like this, otherwise I'd have giant for loops in nested for loops
            for (int i = 0; i < dictionaryArray.Length; i++)
            {
                CurrTerm = dictionaryArray[i].Split(' ')[0];
                CurrFreq = Int32.Parse(dictionaryArray[i].Split(' ')[1]);
                dictionaryMap.Add(CurrTerm, CurrFreq);
            }
            //Find the query term, if it exists we add the relevant documents to the DocVectors with their Weights calculated
            for (int a = 0; a < QueryTerms.Length; a++)
            {
                int i;
                if (dictionaryMap.ContainsKey(QueryTerms[a]))
                {
                    i = dictionaryMap.Keys.ToList().IndexOf(QueryTerms[a]);
                    string[] IndividualPostings = postingArray[i].Split('|');
                    for (int j = 0; j < IndividualPostings.Length - 1; j++)
                    {
                        string[] OnePosting = IndividualPostings[j].Split('\t');
                        int key = Int32.Parse(OnePosting[0]);
                        double[] tempArr = new double[QueryTerms.Length];

                        if (!DocVectors.ContainsKey(key))
                        {
                            DocVectors.Add(key, tempArr);
                        }
                        tempArr = DocVectors[key];

                        tempArr[a] = VectorTermMath(Int32.Parse(OnePosting[1]), Int32.Parse(dictionaryArray[i].Split(' ')[1]), docArray.Length);
                        DocVectors[key] = tempArr;
                    }
                }
            }



            return CosSim(QueryVector, DocVectors);
        }



        //This function returns the Cosine Simularities
        private SortedDictionary<double, List<int>> CosSim(int[] QueryVector, SortedDictionary<int, double[]> DocVectors)
        {
            SortedDictionary<double, List<int>> FinalValues = new SortedDictionary<double, List<int>>();
            double QueryNormWeight, DocNormWeight, CosineSimilarity;
            QueryNormWeight = WeightNormalization(QueryVector);
            foreach (KeyValuePair<int, double[]> entry in DocVectors)
            {
                if (entry.Key == 1315 || entry.Key == 585)
                {
                    CosineSimilarity = 0;
                }
                DocNormWeight = WeightNormalization(entry.Value);
                double NormalizedWeight = DocNormWeight * QueryNormWeight;
                CosineSimilarity = 0;
                for (int i = 0; i < QueryVector.Length; i++)
                {
                    CosineSimilarity = CosineSimilarity + (entry.Value[i] * Convert.ToDouble(QueryVector[i])) / NormalizedWeight;
                }
                if (!FinalValues.ContainsKey(CosineSimilarity))
                {
                    FinalValues.Add(CosineSimilarity, new List<int>());
                }
                FinalValues[CosineSimilarity].Add(entry.Key);
            }
            return FinalValues;
        }

        private double WeightNormalization(int[] Weights)
        {
            double SumOfWeights = 0;
            foreach (int Weight in Weights)
            {
                SumOfWeights = SumOfWeights + Math.Pow(Convert.ToDouble(Weight), 2);
            }
            return Math.Sqrt(SumOfWeights);
        }

        private double WeightNormalization(double[] Weights)
        {
            double SumOfWeights = 0;
            foreach (double Weight in Weights)
            {
                SumOfWeights = SumOfWeights + Math.Pow(Weight, 2);
            }
            return Math.Sqrt(SumOfWeights);
        }
        //This gets Weight for a single term
        private double VectorTermMath(int frequency, int documentFrequency, int numOfDocuments)
        {
            double termFrequency = 0, inverseDocumentFrequency;
            if (frequency > 0) { termFrequency = 1 + Math.Log(frequency); }
            inverseDocumentFrequency = Math.Log10(Convert.ToDouble(numOfDocuments) / Convert.ToDouble(documentFrequency));

            return termFrequency * inverseDocumentFrequency;
        }
    }
}
