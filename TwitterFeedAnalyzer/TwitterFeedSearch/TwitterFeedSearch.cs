using DragonClassifier;
using Lucene.Net.Documents;
using Lucene.Net.Index;
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
        Classifier classifier = null;
        public TwitterFeedSearcher(string directoryPath)
        {
            dir = FSDirectory.GetDirectory(directoryPath);
            searcher = new IndexSearcher(dir);
            var positiveReviews = new Evidence("Positive", "Repository\\Positive.Evidence.csv");
            var negativeReviews = new Evidence("Negative", "Repository\\Negative.Evidence.csv");
            
            classifier = new Classifier(positiveReviews, negativeReviews);                     
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

        public void PerformAmountSearch(string strQuery, out string[] datesArray, out int[] amountsArray)
        {
            List<Document> tweets = new List<Document>();
            List<string> dates = new List<string>();
            List<int> amounts = new List<int>();
            int amount = 0;
            Query query = new QueryParser("text", an).Parse(strQuery);
            Hits results = searcher.Search(query,Sort.INDEXORDER);
            for (int i = 0; i < results.Length(); i++)
            {
                Document doc = results.Doc(i);                
                string date = DateTime.Parse(doc.Get("created")).Date.ToString();
                if (!dates.Contains(date))
                {
                    if (dates.Count > 0)
                    {
                        amounts.Add(amount);
                    }
                    amount = 0;
                    dates.Add(date);
                }
                amount++;
            }
            amounts.Add(amount);
            datesArray = dates.ToArray();
            amountsArray = amounts.ToArray();            
        }

        public void PerformAmountSearch(string strQuery, out string[] datesArray, out int[] amountsArray, out int[] positiveArray, out int[] negativeArray)
        {
            List<Document> tweets = new List<Document>();
            List<string> dates = new List<string>();
            List<int> amounts = new List<int>();
            List<int> positiveAmounts = new List<int>();
            List<int> negativeAmounts = new List<int>();
            int amount = 0;
            int positiveAmount = 0;
            int negativeAmount = 0;
            Query query = new QueryParser("text", an).Parse(strQuery);
            Hits results = searcher.Search(query, Sort.INDEXORDER);
            for (int i = 0; i < results.Length(); i++)
            {
                Document doc = results.Doc(i);
                string date = DateTime.Parse(doc.Get("created")).Date.ToString();
                if (!dates.Contains(date))
                {
                    if (dates.Count > 0)
                    {
                        amounts.Add(amount);
                        positiveAmounts.Add(positiveAmount);
                        negativeAmounts.Add(negativeAmount);
                    }
                    amount = 0;
                    positiveAmount = 0;
                    negativeAmount = 0;
                    dates.Add(date);
                }
                amount++;
                var scores = classifier.Classify(doc.Get("text"), DragonHelper.DragonHelper.ExcludeList);
                if (scores["Positive"] > scores["Negative"])
                {
                    positiveAmount++;
                }
                else
                {
                    negativeAmount++;
                }
            }
            amounts.Add(amount);
            positiveAmounts.Add(positiveAmount);
            negativeAmounts.Add(negativeAmount);
            datesArray = dates.ToArray();
            amountsArray = amounts.ToArray();
            positiveArray = positiveAmounts.ToArray();
            negativeArray = negativeAmounts.ToArray();
        }

        public void GetDocuments(out List<Document> documents,out Dictionary<string, int> positiveTerms, out Dictionary<string, int> negativeTerms)
        {
            positiveTerms = new Dictionary<string, int>();
            negativeTerms = new Dictionary<string, int>();
            documents = new List<Document>();
            IndexReader reader = IndexReader.Open(dir);            
            

            for (int i = 0; i < 10000; i++ )
            {
                if (reader.IsDeleted(i))
                    continue;
                TermFreqVector[] vectors = reader.GetTermFreqVectors(i);
                if (vectors == null)
                {
                    continue;
                }
                foreach (TermFreqVector vector in vectors)
                {
                    foreach (string term in vector.GetTerms())
                    {
                        if (!positiveTerms.ContainsKey(term))
                        {
                            positiveTerms.Add(term, 0);
                            negativeTerms.Add(term, 0);
                        }
                    }
                }
                documents.Add(reader.Document(i));
            }

        }
        
    }
}
