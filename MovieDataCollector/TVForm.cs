using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieDataCollector
{
    public partial class TVForm : Form
    {
        const string APIKey = "8AC38B77755B00A0"; //API Key for the TVDB

        //string seriesURL = ""; //contains the URL from TVDB.com for the all season link for series your are using
        string[] separators = { " ", "." }; //Delimination charactors for filenames in order to separate out numbers
        string[] characterBlocks;
        public string SeriesID { get; set; }

        /*Used to filter out file name information containing numbers
         * that hinders scraping season and episode information*/
        string[] lineStringFilter = { "X264","X.264","H264","H.264","MP4","M4V","MT2S","3GP",
                                    "MPEG2","MPEG-2","MPEG4","MPEG-4","RV40","VP8","VP9",
                                    "1080P","720P","480P"};

        string episode = ""; //contains the episode number  (E + episodeNumber)
        string season = ""; //contains season number (S + seasonNumber
        string ext = ""; //contains the extenstion of files in the filenameslistbox

        string folderPath = ""; //contains path for the selected folder containing video files
        string configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector"; //Direcory to store configuration files on host
        string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Config.txt"; //configuration file location on host
        string configFileText = "";
        string defaultPathText = "";
        string defaultFormat = "";

        string defaultPaths = "";
        string formatCheckDefaults = "";

        List<string> FavoriteURLList = new List<string>(); //Contains URLs for Favorite TV Shows
        List<string> FavoriteListSorter = new List<string>(); //Used to sort Favorites alphabetically when new favorites are added
        TVSeriesInfo SeriesInfo;
        List<string> ListOfEpisodeNames = new List<string>();

        public TVForm()
        {
            InitializeComponent();
        }

        private void getHTML()
        {
            int ID = 0;
            string TitleBox = seriesIDTitleTextbox.Text;

            notificationLabel.Visible = true;
            notificationLabel.Text = "Searching..." + seriesIDTitleTextbox.Text;
            notificationLabel.Invalidate();
            notificationLabel.Update();

            //Scan for Series ID, if no ID found scan for series title and retrieve list of possible series to choose from then parse out the correct id.
            if (int.TryParse(TitleBox, out ID) && ID != 0)
            {
                notificationLabel.Text = "ID found, gathering series info.";
                notificationLabel.Invalidate();
                notificationLabel.Update();
                TVSeriesInfo T = new TVSeriesInfo(APIKey, ID.ToString());
                seriesImagePicturebox.ImageLocation = "http://thetvdb.com/banners/" + T.series["banner"];
                SeriesInfo = T;
                if (T.series.ContainsKey("SeriesName")) { favoritesCombo.Text = T.series["SeriesName"]; }
                seriesIDTitleTextbox.Text = T.Series_ID;
            }
            //Create object that looks up possible TV Series based on the text in the Series Title Box
            else if (!string.IsNullOrEmpty(TitleBox))
            {
                notificationLabel.Text = "Searching..." + seriesIDTitleTextbox.Text;
                notificationLabel.Invalidate();
                notificationLabel.Update();

                //Create object that looks up possible TV Series based on text in SeriesURLBox
                TVSeriesSearch S = new TVSeriesSearch(TitleBox);
                //Create form object to hold results from search
                if (S.SeriesList.Count > 1)
                {
                    notificationLabel.Text = "Multiple Series Identified";
                    notificationLabel.Invalidate();
                    notificationLabel.Update();
                    TVSeriesSelection M = new TVSeriesSelection(S.SeriesList);
                    //Show form as dialog to prevent further code from running until option selected.
                    M.ShowDialog();

                    if (M.DialogResult == DialogResult.OK)
                    {
                        notificationLabel.Text = "Selection Accepted, gathering series info";
                        notificationLabel.Invalidate();
                        notificationLabel.Update();

                        //Once show is selected, use selected shows ID to gather episode information
                        TVSeriesInfo T = new TVSeriesInfo(APIKey, M.SelectedID);
                        //display series banner 
                        seriesImagePicturebox.ImageLocation = "http://thetvdb.com/banners/" + T.series["banner"];
                        SeriesInfo = T;
                        if (T.series.ContainsKey("SeriesName")) { favoritesCombo.Text = T.series["SeriesName"]; }
                        seriesIDTitleTextbox.Text = T.Series_ID;
                    }
                    else if (M.DialogResult == DialogResult.Abort || M.DialogResult == DialogResult.Cancel) { return; }
                }
                else if (S.SeriesList.Count == 1)
                {
                    notificationLabel.Text = "Series Identified, gathering series info";
                    notificationLabel.Invalidate();
                    notificationLabel.Update();

                    TVSeriesInfo T = new TVSeriesInfo(APIKey, S.SeriesList[0]["seriesid"]);
                    seriesImagePicturebox.ImageLocation = "http://thetvdb.com/banners/" + T.series["banner"];
                    SeriesInfo = T;
                    if (T.series.ContainsKey("SeriesName")) { favoritesCombo.Text = T.series["SeriesName"]; }
                    seriesIDTitleTextbox.Text = T.Series_ID;
                }
                else { CustomMessageBox.Show("No such show found", 170, 310); return; }
            }
            else { CustomMessageBox.Show("Please enter Series ID, or name, or URL", 182, 317); return; }

            for (int i = 0; i < SeriesInfo.episodes.Count; i++)
            {
                if (SeriesInfo.episodes[i].ContainsKey("EpisodeName")
                    & SeriesInfo.episodes[i].ContainsKey("EpisodeNumber")
                    & SeriesInfo.episodes[i].ContainsKey("SeasonNumber"))
                {
                    string Snum = "";
                    string Enum = "";
                    if (int.Parse(SeriesInfo.episodes[i]["SeasonNumber"]) < 10)
                    {
                        Snum = "S0" + SeriesInfo.episodes[i]["SeasonNumber"];
                    }
                    else
                    {
                        Snum = "S" + SeriesInfo.episodes[i]["SeasonNumber"];
                    }
                    if (int.Parse(SeriesInfo.episodes[i]["EpisodeNumber"]) < 10)
                    {
                        Enum = "E0" + SeriesInfo.episodes[i]["EpisodeNumber"];
                    }
                    else
                    {
                        Enum = "E" + SeriesInfo.episodes[i]["EpisodeNumber"];
                    }

                    switch (formatCombo.SelectedIndex)
                    {
                        case 0: //Synology
                            ListOfEpisodeNames.Add(SeriesInfo.series["SeriesName"] + "." + Snum + "." + Enum + "." + SeriesInfo.episodes[i]["EpisodeName"] + "." + ext);
                            break;
                        case 1: //PLEX
                            ListOfEpisodeNames.Add(SeriesInfo.series["SeriesName"] + " - " + Snum.ToLower() + Enum.ToLower() + " - " + SeriesInfo.episodes[i]["EpisodeName"] + "." + ext);
                            break;
                        case 2: //KODI
                            ListOfEpisodeNames.Add(SeriesInfo.series["SeriesName"] + "_" + Snum.ToLower() + Enum.ToLower() + "_" + SeriesInfo.episodes[i]["EpisodeName"] + "." + ext);
                            break;
                        default:
                            ListOfEpisodeNames.Add(SeriesInfo.series["SeriesName"] + "." + Snum + "." + Enum + "." + SeriesInfo.episodes[i]["EpisodeName"] + "." + ext);
                            break;
                    }
                    //ListOfEpisodeNames.Add(SeriesInfo.series["SeriesName"] + " " + Snum + Enum + " " + SeriesInfo.episodes[i]["EpisodeName"]);
                }
            }

            notificationLabel.Text = "";
            notificationLabel.Visible = false;
            notificationLabel.Invalidate();
            notificationLabel.Update();
        }

        private void getHTMLButton_Click(object sender, EventArgs e) { getHTML(); }
    }
}
