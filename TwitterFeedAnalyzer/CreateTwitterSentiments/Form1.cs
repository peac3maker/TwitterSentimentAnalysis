using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TwitterFeedSearch;
using NLPToolkit;
using System.IO;

namespace CreateTwitterSentiments
{
    public partial class Form1 : Form
    {
        Dictionary<string, int> positiveTerms = new Dictionary<string, int>();
        Dictionary<string, int> negativeTerms = new Dictionary<string, int>();
        int docNumber = 0;
        List<Document> documents;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {            
            TwitterFeedSearcher searcher = new TwitterFeedSearcher(@"C:\temp\LuceneIndex");
            searcher.GetDocuments(out documents, out positiveTerms, out negativeTerms);
            textBox1.Text = documents[0].Get("text");
            docNumber++;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Document doc = documents[docNumber];
            List<string> words = NLPToolkit.Tokenizer.TokenizeNow(textBox1.Text).ToList();
            foreach (string word in words)
            {
                if(positiveTerms.ContainsKey(word))
                positiveTerms[word] = ++positiveTerms[word];
            }
            textBox1.Text = documents[docNumber].Get("text");
            docNumber++;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Document doc = documents[docNumber];
            List<string> words = NLPToolkit.Tokenizer.TokenizeNow(textBox1.Text).ToList();
            foreach (string word in words)
            {
                if (negativeTerms.ContainsKey(word))
                negativeTerms[word] = ++negativeTerms[word];
            }
            textBox1.Text = documents[docNumber].Get("text");
            docNumber++;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (StreamWriter file = File.CreateText("positive.csv"))
            {
                foreach (KeyValuePair<string, int> val in positiveTerms)
                {
                    file.WriteLine(val.Key + "," + val.Value);
                }
            }
            using (StreamWriter file = File.CreateText("negative.csv"))
            {
                foreach (KeyValuePair<string, int> val in negativeTerms)
                 {
                    file.WriteLine(val.Key + "," + val.Value);
                }
            }
            using (StreamWriter file = File.CreateText("number"))
            {
                file.WriteLine(docNumber);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            TwitterFeedSearcher searcher = new TwitterFeedSearcher(@"C:\temp\LuceneIndex");
            searcher.GetDocuments(out documents, out positiveTerms, out negativeTerms);
            positiveTerms = new Dictionary<string, int>();
            string[] positives = File.ReadAllLines("positive.csv");
            foreach(string positive in positives){
                string[] split = positive.Split(',');
                string term = split[0];
                int amount = Convert.ToInt32(split[1]);
                positiveTerms.Add(term,amount);
            }
            string[] negatives = File.ReadAllLines("negative.csv");
            negativeTerms = new Dictionary<string, int>();
            foreach (string negative in negatives)
            {
                string[] split = negative.Split(',');
                string term = split[0];
                int amount = Convert.ToInt32(split[1]);
                negativeTerms.Add(term, amount);
            }
            docNumber = Convert.ToInt32(File.ReadAllText("number"));
            textBox1.Text = documents[docNumber].Get("text");
            docNumber++;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (StreamWriter file = File.CreateText("Positive.Evidence.csv"))
            {
                foreach (KeyValuePair<string, int> val in positiveTerms)
                {
                    if(val.Value > 0)
                    file.WriteLine(val.Key + "," + val.Value);
                }
            }
            using (StreamWriter file = File.CreateText("Negative.Evidence.csv"))
            {
                foreach (KeyValuePair<string, int> val in negativeTerms)
                 {
                    if(val.Value > 0)
                    file.WriteLine(val.Key + "," + val.Value);
                }
            }        
        }
    }
}
