using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Lucene;
using Lucene.Net.Index;
using Lucene.Net.Store;
using Lucene.Net.Documents;
using System.IO;
using Lucene.Net.Search;
using Iveonik.Stemmers;

namespace SearchKeyWordExtractor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dia = new FolderBrowserDialog();
            DialogResult res = dia.ShowDialog();
            if (res != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }

            FSDirectory dir = FSDirectory.GetDirectory(Environment.CurrentDirectory + "\\LuceneIndex");
            //Lucene.Net.Store.RAMDirectory dir = new RAMDirectory();
            Lucene.Net.Analysis.StopAnalyzer an = new Lucene.Net.Analysis.StopAnalyzer();
            IndexWriter wr = new IndexWriter(dir, an,true);
            IStemmer stemmer = new EnglishStemmer();
            DirectoryInfo diMain = new DirectoryInfo(dia.SelectedPath);
            foreach(FileInfo fi in diMain.GetFiles()){
                Document doc = new Document();
                doc.Add(new Field("title", fi.Name,Field.Store.YES, Field.Index.NO));
                //doc.Add(new Field("text", File.ReadAllText(fi.FullName),Field.Store.YES, Field.Index.TOKENIZED,Field.TermVector.YES));
                doc.Add(new Field("text", PerformStemming(stemmer,NLPToolkit.Tokenizer.TokenizeNow(File.ReadAllText(fi.FullName)).ToArray()), Field.Store.YES, Field.Index.TOKENIZED, Field.TermVector.YES));
                wr.AddDocument(doc);
            }
            wr.Optimize();
            wr.Flush();
            wr.Close();
            dir.Close();

            IndexReader reader = IndexReader.Open(dir);
            for (int i = 0; i < reader.MaxDoc(); i++)
            {
                if (reader.IsDeleted(i))
                    continue;

                Document doc = reader.Document(i);
                String docId = doc.Get("docId");
                foreach (TermFreqVector vector in reader.GetTermFreqVectors(i))
                {
                    foreach(string term in vector.GetTerms()){
                        Console.WriteLine(term);
                    }
                }
                // do something with docId here...
            }
            //IndexSearcher search = new IndexSearcher(wr.GetReader());

            //MoreLikeThis mlt = new MoreLikeThis(wr.GetReader());
            //FileInfo fitarget = new FileInfo(@"C:\Users\peacemaker\Desktop\TestNoBitcoin\test.txt");
            //Query query = mlt.Like(fitarget);

            //var hits = search.Search(query, int.MaxValue);
            //foreach (ScoreDoc doc in hits.ScoreDocs)
            //{
            //    textBox1.Text += doc.Score + Environment.NewLine;
            //}                                                            
            
        }
        public string PerformStemming(IStemmer stemmer, string[] words)
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
