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
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;
            InitializeComponent();
            Series_List = SeriesList;
            GenerateList();
        }
        private void SeriesNameListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //https://artworks.thetvdb.com/banners/posters/255316-2.jpg
            ///banners/posters/5d353f9f602ec.jpg
            if (Series_List[SeriesNameListBox.SelectedIndex].ContainsKey("banner")) { SeriesPosterPictureBox.ImageLocation = "https://artworks.thetvdb.com" + Series_List[SeriesNameListBox.SelectedIndex]["banner"]; }
            else { SeriesPosterPictureBox.ImageLocation = ""; };

            if (Series_List[SeriesNameListBox.SelectedIndex].ContainsKey("overview")) { overviewTextBox.Text = Series_List[SeriesNameListBox.SelectedIndex]["overview"]; }
            else { overviewTextBox.Text = "No Overview provided"; }
        }
        private void AcceptBtn_Click(object sender, EventArgs e)
        {
            SelectedID = Series_List[SeriesNameListBox.SelectedIndex]["id"];
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
            SeriesNameListBox.Items.Clear();

            if (yearRbtn.Checked)
            {
                Series_List = (from x in Series_List
                               orderby x["firstAired"] descending
                               select x).ToList();
            }
            else
            {
                Series_List = (from x in Series_List
                               orderby x["seriesName"] ascending
                               select x).ToList();
            }

            for (int i = 0; i < Series_List.Count(); i++)
            {

                if (Series_List[i].ContainsKey("seriesName") & Series_List[i].ContainsKey("id"))
                {
                    if (Series_List[i].ContainsKey("firstAired") & !string.IsNullOrEmpty(Series_List[i]["firstAired"]) & !Series_List[i]["firstAired"].Contains("null"))
                    {
                        SeriesNameListBox.Items.Add(Series_List[i]["seriesName"] + " - (" + (Series_List[i]["firstAired"].Remove(Series_List[i]["firstAired"].Length - 6, 5)) + ")");
                    }
                    else { SeriesNameListBox.Items.Add(Series_List[i]["seriesName"]); }
                }
            }
            if (SeriesNameListBox.Items.Count > 0)
            {
                SeriesNameListBox.SelectedIndex = 0;

                if (Series_List[0].ContainsKey("banner")) { SeriesPosterPictureBox.ImageLocation = "https://artworks.thetvdb.com" + Series_List[0]["banner"]; }
                else { /*Provide default picture incase no banner is found*/ };

                if (Series_List[0].ContainsKey("overview")) { overviewTextBox.Text = Series_List[0]["overview"]; }
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
