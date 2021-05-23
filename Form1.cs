using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text;

namespace PageRankHM
{
    public partial class Form1 : Form
    {
        private double dampFactor = 0.85;
        private double convergenceValue = 0.0001;
        private string fileName = "";
        private ArrayList graphLinksMatrix = null;
        private bool graphLoaded = false;
        private ArrayList pageRankReputation; //ArrayList of double[]
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.InitialDirectory = Directory.GetCurrentDirectory();
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                fileName = openFileDialog1.FileName;
                graphLinksMatrix = new ArrayList();
                string[] text = System.IO.File.ReadAllLines(fileName);
                int numberOfNodes = Int32.Parse(text[0]);
                text = text.Skip(1).ToArray();
                for (int i = 0; i < numberOfNodes; i++)
                {
                    graphLinksMatrix.Add(new List<int>());
                }
                foreach (string line in text)
                {
                    string[] lineTrimmed = line.Split(' ');
                    ((List<int>)graphLinksMatrix[Int32.Parse(lineTrimmed[0]) - 1]).Add(Int32.Parse(lineTrimmed[1]));
                }
                graphLoaded = true;
                label1.Text = "Graph Loaded!";
            }
        }

        private void openFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(!graphLoaded)
            {
                label1.Text += "!";
                return;
            }
            richTextBox1.Text = "Calculating PageRank...";
            PageRank pr = new PageRank(graphLinksMatrix, dampFactor, convergenceValue, 1);
            pageRankReputation = pr.ComputePageRank();
            printResult();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dampFactor = Double.Parse(textBox1.Text);
            
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            convergenceValue = Double.Parse(textBox2.Text);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void printResult()
        {
            richTextBox1.Text = "Converged PageRank\n\n\n";

            for(int i = 0; i < pageRankReputation.Count; i++)
            {
                richTextBox1.Text += "Iteration " + (i + 1) + ":\n\n";
                for(int j = 0; j < ((double[])pageRankReputation[i]).Length; j++)
                {
                    richTextBox1.Text += "Node " + (j + 1) + ":" + ((double[])pageRankReputation[i])[j] + "\n";
                }

                richTextBox1.Text += "\n\n";
            }
            
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!graphLoaded)
            {
                label1.Text += "!";
                return;
            }
            //a (beta - iterations needed)
            //b (3D plot (beta - results)
            string folderPath = Directory.GetCurrentDirectory() + "\\Results ex 5";
            System.IO.Directory.CreateDirectory(folderPath);

            string iterationsNeeded = folderPath + "\\iterations.csv";
            string pageRanks = folderPath + "\\pageRanks.csv";

            var iterationscsv = new StringBuilder();
            var pageRankscsv = new StringBuilder();
            for (double beta = 0; beta <= 1; beta += 0.05)
            {
                PageRank pr = new PageRank(graphLinksMatrix, beta, convergenceValue, 1);
                pageRankReputation = pr.ComputePageRank();

                iterationscsv.AppendLine(beta + "," + (pageRankReputation.Count + 1));
                for(int i = 0; i < ((double[])pageRankReputation[pageRankReputation.Count - 1]).Length; i++)
                {
                    pageRankscsv.AppendLine(beta + "," + i + "," + ((double[])pageRankReputation[pageRankReputation.Count - 1])[i]);
                }
            }

            File.WriteAllText(iterationsNeeded, iterationscsv.ToString());
            File.WriteAllText(pageRanks, pageRankscsv.ToString());


        }
    }
}
