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
using System.Windows.Forms.DataVisualization.Charting;
using TwitterFeedSearch;

namespace TwitterSearchGui
{
    public partial class Form1 : Form
    {
        TwitterFeedSearcher search = null;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (search == null)
            {
                MessageBox.Show("Please set the Lucene Directory by clicking the button \"choose directory\"");
                return;
            }
            if(string.IsNullOrEmpty(textBox1.Text)){
                MessageBox.Show("Please enter some search keywords");
                return;
            }

            //List<Document> results = search.PerformSearch(textBox1.Text);
            //List<string> dates = new List<string>();
            //List<int> amounts = new List<int>();
            //int amount = 0;
            //foreach (Document doc in results.OrderBy(d => DateTime.Parse(d.Get("created")).Date))
            //{                
            //    string date = DateTime.Parse(doc.Get("created")).Date.ToString();
            //    if (!dates.Contains(date))
            //    {
            //        if (dates.Count > 0)
            //        {
            //            amounts.Add(amount);
            //        }
            //        dates.Add(date);
            //    }
            //    amount++;
            //}
            //amounts.Add(amount);
            //string[] datesArray = dates.ToArray();
            //int[] amountsArray = amounts.ToArray();
            string[] datesArray;
            int[] amountsArray;
            search.PerformAmountSearch(textBox1.Text, out datesArray, out amountsArray);
            // Add series.
            this.chart.Series.Clear();
            this.chart.ChartAreas[0].AxisY.Maximum = amountsArray.Max()+50;
            for (int i = 0; i < datesArray.Length; i++)
            {
                // Add series.
                Series series = this.chart.Series.Add(datesArray[i]);

                // Add point.
                series.Points.Add(amountsArray[i]);
            }
            
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dia = new FolderBrowserDialog();
            dia.SelectedPath = @"C:\temp\LuceneIndex";
            DialogResult res = dia.ShowDialog();
            if (res != System.Windows.Forms.DialogResult.OK)
            {
                return;
            }
            search = new TwitterFeedSearcher(dia.SelectedPath);
        }
    }
}
