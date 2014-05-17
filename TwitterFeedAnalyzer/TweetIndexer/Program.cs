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
            DateTime dtmFirst = DateTime.Now;
            dtmFirst = dtmFirst.AddHours(-dtmFirst.Hour);
            dtmFirst = dtmFirst.AddMinutes(-dtmFirst.Minute);
            DateTime dtmLast = DateTime.Now.AddDays(1);
            dtmLast = dtmLast.AddHours(-dtmLast.Hour);
            dtmLast = dtmLast.AddMinutes(-dtmLast.Minute);
            var query = Query<TweetItem>.Where(t => t.CreationDate >= dtmFirst && t.CreationDate < dtmLast);
            List<TweetItem> value = collection.Find(query).ToList();
            FSDirectory dir = FSDirectory.GetDirectory(Environment.CurrentDirectory + "\\LuceneIndex");
            //Lucene.Net.Store.RAMDirectory dir = new RAMDirectory();
            Lucene.Net.Analysis.StopAnalyzer an = new Lucene.Net.Analysis.StopAnalyzer();
            IndexWriter wr = new IndexWriter(dir, an, true);
            //DirectoryInfo diMain = new DirectoryInfo(dia.SelectedPath);
            foreach (TweetItem tweet in value)
            {
                Document doc = new Document();
                doc.Add(new Field("id", tweet._id.ToString(), Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("created", tweet.CreationDate.ToString(), Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("user", tweet.User, Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("text", tweet.Text, Field.Store.YES, Field.Index.TOKENIZED, Field.TermVector.YES));
                wr.AddDocument(doc);
            }
            wr.Optimize();
            wr.Flush();
            wr.Close();
            dir.Close();
        }
    }
}
