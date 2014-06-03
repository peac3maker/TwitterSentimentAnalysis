using Iveonik.Stemmers;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Store;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwitterFeedLogger;

namespace TweetIndexer
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionString = "mongodb://10.0.0.17/test";
            //var connectionString = "mongodb://localhost/test";

            MongoClient mongoClient = new MongoClient(connectionString);
            MongoServer mongoServer = mongoClient.GetServer();
            MongoDatabase db = mongoServer.GetDatabase("test");
            var collection = db.GetCollection<TweetItem>("TweetItems");
            DateTime dtmFirst = new DateTime(2014, 05, 17,0,0,0);
            DateTime dtmLast = new DateTime(2014, 05, 17, 23, 59, 59);
            FSDirectory dir = FSDirectory.GetDirectory(Environment.CurrentDirectory + "\\LuceneIndex");
            //Lucene.Net.Store.RAMDirectory dir = new RAMDirectory();
            Lucene.Net.Analysis.StopAnalyzer an = new Lucene.Net.Analysis.StopAnalyzer();            
            IndexWriter wr = new IndexWriter(dir, an, true);
            IStemmer stemmer = new EnglishStemmer();
            while (dtmFirst.Date <= DateTime.Now.Date)
            {
                var query = Query<TweetItem>.Where(t => t.CreationDate >= dtmFirst && t.CreationDate <= dtmLast);
                List<TweetItem> value = collection.Find(query).ToList();                
                //DirectoryInfo diMain = new DirectoryInfo(dia.SelectedPath);
                foreach (TweetItem tweet in value)
                {
                    Document doc = new Document();
                    doc.Add(new Field("id", tweet._id.ToString(), Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("created", tweet.CreationDate.ToString(), Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("user", tweet.User, Field.Store.YES, Field.Index.NO));
                    doc.Add(new Field("text", PerformStemming(stemmer,NLPToolkit.Tokenizer.TokenizeNow(tweet.Text).ToArray()), Field.Store.YES, Field.Index.TOKENIZED, Field.TermVector.YES));
                    wr.AddDocument(doc);
                }
                dtmFirst = dtmFirst.AddDays(1);
                dtmLast = dtmLast.AddDays(1);
            }
            wr.Optimize();
            wr.Flush();
            wr.Close();
            dir.Close();
        }

        public static string PerformStemming(IStemmer stemmer, string[] words)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string word in words)
            {
                if (builder.ToString().Length == 0)
                {
                    builder.Append(stemmer.Stem(word));
                }
                else
                {
                    builder.AppendFormat(" {0}", stemmer.Stem(word));
                }
            }
            return builder.ToString();
        }
    }
}
