using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Eval
{
    class Program
    {
        static void Main(string[] args)
        {
            bool stopWords = false, stemming = false;
            foreach (var item in args)
            {
                if (item.ToString() == "-stop") stopWords = true;
                if (item.ToString() == "-stem") stemming = true;
            }
            ReadingQueries rq = new ReadingQueries(stopWords, stemming);

            Console.WriteLine("Done");
        }
    }
    class ReadingQueries
    {
        public ReadingQueries(bool StopWord, bool Stemming)
        {
            string fileLocation = AppDomain.CurrentDomain.BaseDirectory + "\\query.text";
            string[] lines = File.ReadAllLines(fileLocation).ToArray();
            List<List<string>> Results = new List<List<string>>();
            int QueryNum = 0;
            string currentFlag = "";
            string abstractText = "";
            IRSearch search;

            for (int i = 0; i < lines.Count(); i++)
            {
                if (FlagCheck(lines[i])) { currentFlag = lines[i].Substring(0, 2); }
                switch (currentFlag)
                {
                    case ".I":
                        QueryNum++;
                        abstractText = "";
                        break;
                    case ".W":
                        abstractText = abstractText + " " + lines[i];
                        break;
                    case ".N":
                        if (abstractText.Length > 3) { abstractText = abstractText.Substring(3); }
                        search = new IRSearch(abstractText, StopWord, Stemming);
                        Results.Add(search.allStringsFound);
                        currentFlag = "";
                        break;
                    default:
                        break;
                }
            }
            AverageMAPandRPrecision(Results);
        }
        private double[] AverageMAPandRPrecision(List<List<string>> Results)
        {
            double RPrecision = 0;
            double MAP = 0;
            double SingleDocumentPrecision, AveragePrecisionPerQuery;
            int counter;
            int QueryCount = 0;
            int RelevantDocumentsFound;
            SortedDictionary<int, List<int>> RelevantQueryList = GetRelevantQueryList();
            //Every Query with its list
            foreach (List<string> FinalResult in Results)
            {
                QueryCount++;
                counter = 1;
                RelevantDocumentsFound = 0;
                SingleDocumentPrecision = 0;
                AveragePrecisionPerQuery = 0;
                //Each individual Queries Retrieved document list
                for (int i = 0; i < FinalResult.Count; i++)
                {
                    int DocumentNum = Convert.ToInt32(FinalResult[i].Split('\t')[0]);
                    if (RelevantQueryList.ContainsKey(QueryCount))
                    {
                        if (RelevantQueryList[QueryCount].Contains(DocumentNum))
                        {
                            RelevantDocumentsFound++;
                            SingleDocumentPrecision = RelevantDocumentsFound / counter;
                            AveragePrecisionPerQuery = AveragePrecisionPerQuery + SingleDocumentPrecision;
                        }
                    }
                    counter++;
                }
                RPrecision = RPrecision + SingleDocumentPrecision;
                if (RelevantQueryList.ContainsKey(QueryCount))
                {
                    MAP = MAP + AveragePrecisionPerQuery / RelevantQueryList[QueryCount].Count;
                }
            }

            RPrecision = RPrecision / RelevantQueryList.Count;
            MAP = MAP / RelevantQueryList.Count;

            double[] FinalInfo = { MAP, RPrecision };
            Console.WriteLine(String.Format("The Final Precision for my IR system is {0} and the MAP is {1}", RPrecision, MAP));
            Console.ReadKey();
            return FinalInfo;
        }
        private SortedDictionary<int, List<int>> GetRelevantQueryList()
        {
            SortedDictionary<int, List<int>> Relevant = new SortedDictionary<int, List<int>>();
            string fileLocation = AppDomain.CurrentDomain.BaseDirectory + "\\qrels.text";
            string[] lines = File.ReadAllLines(fileLocation).ToArray();
            foreach (string line in lines)
            {
                string[] information = line.Split(' ');
                int key = Convert.ToInt32(information[0]);
                int value = Convert.ToInt32(information[1]);
                if (!Relevant.ContainsKey(key))
                {
                    Relevant[key] = new List<int>();
                }
                List<int> Documents = Relevant[key];
                Documents.Add(value);
                Relevant[key] = Documents;
            }

            return Relevant;
        }
        private bool FlagCheck(string sentence)
        {
            Regex r = new Regex("^\\.[A-Z]$|^\\.[I]");
            return r.IsMatch(sentence);
        }
    }
}
