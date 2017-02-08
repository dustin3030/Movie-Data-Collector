using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace MovieDataCollector
{
    public partial class TVSeriesSelection : Form
    {
        List<Dictionary<string, string>> Series_List;
        public string SelectedID { get; set; }

        public TVSeriesSelection(List<Dictionary<string, string>> SeriesList)
        {
            InitializeComponent();
            Series_List = SeriesList;
            GenerateList();
        }
        private void seriesNameListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Series_List[seriesNameListBox.SelectedIndex].ContainsKey("banner")) { seriesPosterPictureBox.ImageLocation = "http://thetvdb.com/banners/" + Series_List[seriesNameListBox.SelectedIndex]["banner"]; }
            else { seriesPosterPictureBox.ImageLocation = ""; };

            if (Series_List[seriesNameListBox.SelectedIndex].ContainsKey("Overview")) { overviewTextBox.Text = Series_List[seriesNameListBox.SelectedIndex]["Overview"]; }
            else { overviewTextBox.Text = "No Overview provided"; }
        }
        private void AcceptBtn_Click(object sender, EventArgs e)
        {
            SelectedID = Series_List[seriesNameListBox.SelectedIndex]["seriesid"];
            DialogResult = DialogResult.OK;
            this.Close();
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void GenerateList()
        {
            seriesNameListBox.Items.Clear();

            if (yearRbtn.Checked)
            {
                Series_List = (from x in Series_List
                               orderby x["FirstAired"] descending
                               select x).ToList();
            }
            else
            {
                Series_List = (from x in Series_List
                               orderby x["SeriesName"] ascending
                               select x).ToList();
            }

            for (int i = 0; i < Series_List.Count(); i++)
            {

                if (Series_List[i].ContainsKey("SeriesName") & Series_List[i].ContainsKey("seriesid"))
                {
                    if (Series_List[i].ContainsKey("FirstAired") & !string.IsNullOrEmpty(Series_List[i]["FirstAired"]))
                    {
                        seriesNameListBox.Items.Add(Series_List[i]["SeriesName"] + " - (" + (Series_List[i]["FirstAired"].Remove(Series_List[i]["FirstAired"].Length - 6, 6)) + ")");
                    }
                    else { seriesNameListBox.Items.Add(Series_List[i]["SeriesName"]); }
                }
            }
            if (seriesNameListBox.Items.Count > 0)
            {
                seriesNameListBox.SelectedIndex = 0;

                if (Series_List[0].ContainsKey("banner")) { seriesPosterPictureBox.ImageLocation = "http://thetvdb.com/banners/" + Series_List[0]["banner"]; }
                else { /*Provide default picture incase no banner is found*/ };

                if (Series_List[0].ContainsKey("Overview")) { overviewTextBox.Text = Series_List[0]["Overview"]; }
                else { overviewTextBox.Text = "No Overview provided"; }
            }
        }
        private void yearRbtn_Click(object sender, EventArgs e)
        {

            GenerateList();
        }
        private void defaultRbtn_Click(object sender, EventArgs e)
        {
            GenerateList();
        }
    }
}
