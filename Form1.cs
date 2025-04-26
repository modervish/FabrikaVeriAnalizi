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


namespace Fabrika
{
    public partial class Form1 : Form
    {
        private DataTable productionData;
        public Form1()
        {
            InitializeComponent();
            LoadData();
            InitializeComboBox();
        }
        private void LoadData()
        {
            string filePath = "D:\\proje\\urunler.txt";
            productionData = ReadCsvFile(filePath);
            dataGridView1.DataSource = productionData;
        }
        private void InitializeComboBox()
        {
            var machines = productionData.AsEnumerable()
                                         .Select(row => row.Field<string>("Makine"))
                                         .Distinct()
                                         .ToList();
            comboBox1.DataSource = machines;
        }
        private DataTable ReadCsvFile(string filePath)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Makine", typeof(string));
            dt.Columns.Add("Tarih", typeof(DateTime));
            dt.Columns.Add("HedefMiktar", typeof(int));
            dt.Columns.Add("UretilenMiktar", typeof(int));

            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string headerLine = sr.ReadLine();

                    while (!sr.EndOfStream)
                    {
                        string line = sr.ReadLine();
                        if (string.IsNullOrWhiteSpace(line))
                        {
                            continue;
                        }

                        string[] values = line.Split(',');

                        if (values.Length != 4)
                        {
                            MessageBox.Show($"Skipping malformed line: {line}");
                            continue;
                        }

                        try
                        {
                            string machine = values[0];
                            DateTime date = DateTime.Parse(values[1]);
                            int target = int.Parse(values[2]);
                            int produced = int.Parse(values[3]);

                            dt.Rows.Add(machine, date, target, produced);
                        }
                        catch (FormatException ex)
                        {
                            MessageBox.Show($"Skipping line due to format error: {line} - {ex.Message}");
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading CSV file: {ex.Message}");
            }

            return dt;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a machine.");
                return;
            }

            string selectedMachine = comboBox1.SelectedItem.ToString();
            DateTime startDate = dateTimePicker1.Value.Date;
            DateTime endDate = dateTimePicker2.Value.Date;

            var filteredData = productionData.AsEnumerable()
                                             .Where(row => row.Field<string>("Makine") == selectedMachine &&
                                                           row.Field<DateTime>("Tarih") >= startDate &&
                                                           row.Field<DateTime>("Tarih") <= endDate);

            if (!filteredData.Any())
            {
                MessageBox.Show("No data found for the selected machine and date range.");
                dataGridView1.DataSource = null;
                chart1.Series.Clear();
                return;
            }



            var filteredDataTable = filteredData.CopyToDataTable();
            dataGridView1.DataSource = filteredDataTable;
            DrawChart(filteredDataTable);
        }
        private void DrawChart(DataTable data)
        {
            chart1.Series.Clear();

            Series targetSeries = new Series("HedefMiktar");
            targetSeries.ChartType = SeriesChartType.Column;
            targetSeries.Color = System.Drawing.Color.Blue;

            Series producedSeries = new Series("UretilenMiktar");
            producedSeries.ChartType = SeriesChartType.Column;
            producedSeries.Color = System.Drawing.Color.Orange;

            foreach (DataRow row in data.Rows)
            {
                DateTime date = row.Field<DateTime>("Tarih");
                int target = row.Field<int>("HedefMiktar");
                int produced = row.Field<int>("UretilenMiktar");

                targetSeries.Points.AddXY(date.ToShortDateString(), target);
                producedSeries.Points.AddXY(date.ToShortDateString(), produced);
            }

            chart1.Series.Add(targetSeries);
            chart1.Series.Add(producedSeries);

            chart1.ChartAreas[0].AxisX.LabelStyle.Angle = -45;
            chart1.ChartAreas[0].RecalculateAxesScale();
        }
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {

        }
        private void chart1_Click(object sender, EventArgs e)
        {

        }
        private void label3_Click(object sender, EventArgs e)
        {

        }
        private void label2_Click(object sender, EventArgs e)
        {

        }
        private void label1_Click(object sender, EventArgs e)
        {

        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}
