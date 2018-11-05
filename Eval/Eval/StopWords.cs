using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eval
{
    class StopWords
    {
        private List<string> StopWordList;
        public StopWords()
        {
            StopWordList = new List<string>();
            StopWordList.Add("I");
            StopWordList.Add("a");
            StopWordList.Add("about");
            StopWordList.Add("an");
            StopWordList.Add("and");
            StopWordList.Add("are");
            StopWordList.Add("as");
            StopWordList.Add("at");
            StopWordList.Add("be");
            StopWordList.Add("by");
            StopWordList.Add("for");
            StopWordList.Add("from");
            StopWordList.Add("how");
            StopWordList.Add("in");
            StopWordList.Add("is");
            StopWordList.Add("it");
            StopWordList.Add("of");
            StopWordList.Add("on");
            StopWordList.Add("or");
            StopWordList.Add("that");
            StopWordList.Add("the");
            StopWordList.Add("this");
            StopWordList.Add("to");
            StopWordList.Add("was");
            StopWordList.Add("what");
            StopWordList.Add("when");
            StopWordList.Add("where");
            StopWordList.Add("who");
            StopWordList.Add("will");
            StopWordList.Add("with");
            StopWordList.Add("the");
        }
        public bool StopMatching(string Term)
        {
            return StopWordList.Contains(Term);
        }
    }
}
