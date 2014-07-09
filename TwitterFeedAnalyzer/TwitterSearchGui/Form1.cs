using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
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
        BackgroundWorker worker = new BackgroundWorker();
        List<BitCoinData> bitcoinData = new List<BitCoinData>();
        public Form1()
        {
            InitializeComponent();
            worker.DoWork += new DoWorkEventHandler(back_DoWork);
            
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
            int[] positiveArray;
            int[] negativeArray;
            search.PerformAmountSearch(textBox1.Text, out datesArray, out amountsArray, out positiveArray, out negativeArray);
            // Add series.
            this.chart.Series.Clear();
            this.chart.ChartAreas[0].AxisY.Maximum = amountsArray.Max()+20;            

            this.chart1.Series.Clear();
            //this.chart1.ChartAreas[0].AxisY.Maximum = positiveArray.Max() + 20;
            //this.chart1.ChartAreas[0].AxisY.Minimum = -negativeArray.Max() - 20;
            int lowest = 0, highest = 0;
            for (int i = 0; i < datesArray.Length; i++)
            {
                // Add series.
                Series series = this.chart.Series.Add(datesArray[i]);
                //series.ChartType = SeriesChartType.Spline;

                // Add point.
                series.Points.Add(amountsArray[i]);

                series.Points.Add(positiveArray[i]);
                series.Points.Add(negativeArray[i]);

                Series series2 = this.chart1.Series.Add(datesArray[i]);
                //series2.ChartType = SeriesChartType.SplineArea;
                int diff = positiveArray[i] - negativeArray[i];
                if (diff > highest)
                {
                    highest = diff;
                }
                else if (diff < lowest)
                {
                    lowest = diff;
                }
                series2.Points.Add(diff);
                //series2.Points.Add((positiveArray[i] - negativeArray[i]));
            }
            this.chart1.ChartAreas[0].AxisY.Maximum = highest + 20;
            this.chart1.ChartAreas[0].AxisY.Minimum = lowest - 20;            
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

        private void button1_Click(object sender, EventArgs e)
        {
            //worker.RunWorkerAsync();
            if (search == null)
            {
                MessageBox.Show("Please set the Lucene Directory by clicking the button \"choose directory\"");
                return;
            }
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please enter some search keywords");
                return;
            }
            string[] datesArray;
            int[] amountsArray;
            int[] positiveArray;
            int[] negativeArray;
            search.PerformAmountSearchIntraDay(textBox1.Text, out datesArray, out amountsArray, out positiveArray, out negativeArray, dateTimePicker1.Value);
            // Add series.
            this.chart.Series.Clear();
            this.chart.ChartAreas[0].AxisY.Maximum = amountsArray.Max() + 20;
            this.chart1.Series.Clear();
            //this.chart1.ChartAreas[0].AxisY.Maximum = positiveArray.Max() + 20;
            //this.chart1.ChartAreas[0].AxisY.Minimum = -negativeArray.Max() - 20;
            int lowest = 0, highest = 0;
            for (int i = 0; i < datesArray.Length; i++)
            {
                // Add series.
                Series series = this.chart.Series.Add(datesArray[i]);
                //series.ChartType = SeriesChartType.Spline;

                // Add point.
                series.Points.Add(amountsArray[i]);

                series.Points.Add(positiveArray[i]);
                series.Points.Add(negativeArray[i]);

                Series series2 = this.chart1.Series.Add(datesArray[i]);
                //series2.ChartType = SeriesChartType.SplineArea;
                int diff = positiveArray[i] - negativeArray[i];
                if (diff > highest)
                {
                    highest = diff;
                }
                else if (diff < lowest)
                {
                    lowest = diff;
                }
                series2.Points.Add(diff);
            }
            this.chart1.ChartAreas[0].AxisY.Maximum = highest + 20;
            this.chart1.ChartAreas[0].AxisY.Minimum = lowest - 20;
            
        }

        private void back_DoWork(object sender, DoWorkEventArgs e)
        {
            if (search == null)
            {
                MessageBox.Show("Please set the Lucene Directory by clicking the button \"choose directory\"");
                return;
            }
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please enter some search keywords");
                return;
            }
            string[] datesArray;
            int[] amountsArray;
            int[] positiveArray;
            int[] negativeArray;
            search.PerformAmountSearchIntraDay(textBox1.Text, out datesArray, out amountsArray, out positiveArray, out negativeArray, dateTimePicker1.Value);
            // Add series.
            this.chart.Series.Clear();
            this.chart.ChartAreas[0].AxisY.Maximum = amountsArray.Max() + 20;
            this.chart1.Series.Clear();
            //this.chart1.ChartAreas[0].AxisY.Maximum = positiveArray.Max() + 20;
            //this.chart1.ChartAreas[0].AxisY.Minimum = -negativeArray.Max() - 20;
            int lowest = 0, highest = 0;
            for (int i = 0; i < datesArray.Length; i++)
            {
                // Add series.
                Series series = this.chart.Series.Add(datesArray[i]);
                //series.ChartType = SeriesChartType.Spline;

                // Add point.
                series.Points.Add(amountsArray[i]);

                series.Points.Add(positiveArray[i]);
                series.Points.Add(negativeArray[i]);

                Series series2 = this.chart1.Series.Add(datesArray[i]);
                //series2.ChartType = SeriesChartType.SplineArea;
                int diff = positiveArray[i] - negativeArray[i];
                if (diff > highest)
                {
                    highest = diff;
                }
                else if (diff < lowest)
                {
                    lowest = diff;
                }
                series2.Points.Add(diff);
            }
            this.chart1.ChartAreas[0].AxisY.Maximum = highest + 20;
            this.chart1.ChartAreas[0].AxisY.Minimum = lowest - 20;
        }

        private void btnBitcoinData_Click(object sender, EventArgs e)
        {
            bitcoinData.Clear();
           StreamReader reader = File.OpenText("Bitcoindata.csv");
            string str = string.Empty;
            
           while(!reader.EndOfStream){
               str = reader.ReadLine();
               string[] split = str.Split(',');
               BitCoinData data = new BitCoinData()
               {
                   Date = DateTime.Parse(split[0]),
                   Open = float.Parse(split[1]),
                   High = float.Parse(split[2]),
                   Low = float.Parse(split[3]),
                   Close = float.Parse(split[4]),
                   VolumeBTC = double.Parse(split[5]),
                   VolumeCurrency = double.Parse(split[6]),
                   WeighedPrice = float.Parse(split[7])
               };
               Series series2 = null;
               if (this.chart1.Series.IndexOf(data.Date.ToString()) != -1)
               {
                   series2 = this.chart1.Series[data.Date.ToString()];

                   //series2.ChartType = SeriesChartType.SplineArea;
                   series2.Points.Add(data.Close- data.Open);
                   bitcoinData.Add(data);
               }
           }           
        }
    }
}
