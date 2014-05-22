using Lucene.Net.Documents;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwitterFeedSearch
{
    public class TwitterFeedSearcher
    {
        FSDirectory dir = null;
        Lucene.Net.Analysis.StopAnalyzer an = new Lucene.Net.Analysis.StopAnalyzer();
        IndexSearcher searcher = null;
        public TwitterFeedSearcher(string directoryPath)
        {
            dir = FSDirectory.GetDirectory(directoryPath);
            searcher = new IndexSearcher(dir);
        }

        public List<Document> PerformSearch(string strQuery){
            List<Document> tweets = new List<Document>();
            Query query = new QueryParser("text", an).Parse(strQuery);
            Hits results = searcher.Search(query);
            for (int i = 0; i < results.Length(); i++)
            {
                Document doc = results.Doc(i);
                string text = doc.Get("text");
                Console.WriteLine(text);
                tweets.Add(doc);
            }
            return tweets;
        }
        
    }
}
