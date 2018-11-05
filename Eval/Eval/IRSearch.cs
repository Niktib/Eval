using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eval
{
    class IRSearch
    {

        static PorterStemming ps;
        searchingData dataSearcher = new searchingData();
        public List<string> allStringsFound;


        public  IRSearch(string query, bool StopWord, bool Stemming)
        {
            ps = new PorterStemming();
            StopWords sw = new StopWords();
            List<string> termFixing = new List<string>();
            string[] StringArray = query.ToLower().Replace("*", "").Replace("+", "").Replace(".", "").Replace(",", "").Replace("!", "").Replace("?", "").Replace("(", "").Replace(")", "").Replace("=", "").Replace('\n', ' ').Split(' ');
            foreach (string SingleTerms in StringArray)
            {
                if (!StopWord || (StopWord && !sw.StopMatching(SingleTerms)))
                {
                    if (Stemming)
                    {
                        termFixing.Add(ps.StemWord(SingleTerms));
                    }
                    else if (SingleTerms != "")
                    {
                        termFixing.Add(SingleTerms);
                    }
                }
            }
            string terms = " " + String.Join(" ", termFixing.ToArray()) + " ";
            List<int> QueryVector = new List<int>();
            List<string> QueryTerms = new List<string>();
            termFixing.Sort();
            while (termFixing.Count > 0)
            {
                QueryVector.Add(Regex.Matches(terms, String.Format(" {0} ", termFixing[0])).Count);
                QueryTerms.Add(termFixing[0]);
                for (int i = 0; i < QueryVector.LastOrDefault(); i++) { termFixing.Remove(termFixing[0]); }
            }

            SortedDictionary<double, List<int>> FinalNumbers = dataSearcher.FindFinalVectors(QueryVector.ToArray(), QueryTerms.ToArray());
            terms = "";
            allStringsFound = new List<string>();
            var items = from pair in FinalNumbers
                        orderby pair.Key descending
                        select pair;
            foreach (KeyValuePair<double, List<int>> pair in items)
            {
                for (int k = 0; k < pair.Value.Count; k++)
                    allStringsFound.Add(String.Format("{0}\t{1}\n", pair.Value[k], pair.Key));
            }
        }
    }
}
