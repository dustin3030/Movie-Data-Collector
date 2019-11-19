using System;
using System.Collections.Generic;
using System.Data;
using System.IO; //allows for file manipulation
using System.Net; //Allows for WebClient usage
using System.Diagnostics; //Allows for using Process.Start codes lines
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MovieDataCollector
{
    public partial class TVForm : Form
    {
        const string API_Key = "8AC38B77755B00A0"; //API Key for the TheTVDB.com
        const string User_Key = "04DD483577DACF8E"; //User Key for the TheTVDB.com
        const string User_Name = "dustin.oldroyd@gmail.com"; //UserName for TheTVDB.com Access
        string Authorization_Token = ""; //Authorization Token used for querying theTVDB.com databases.

        //string SeriesURL = ""; //contains the URL from TVDB.com for the all season link for Series your are using
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

        string ConfigDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector"; //Direcory to store configuration files on host
        string ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Config.txt"; //configuration file location on host


        TVSeriesInfo SeriesInfo;
        List<string> EpisodePathList= new List<string>(); //Stores File Location for episode list, Necessary for recursive search function


        List<string> ListOfEpisodeNames = new List<string>(); //Used in Manual Rename Form, Populated with items from ONE of the following 3 lists (KODIEpisodeNames, SynologyEpisodeNames, PLEXEpisodeNames)
        List<string> KODIEpisodeNames = new List<string>(); //Used for manual rename form - Populates ListOfEpisodeNames based on format selection
        List<string> SynologyEpisodeName = new List<string>(); //Used for manual rename form - Populates ListOfEpisodeNames based on format selection
        List<string> PLEXEpisodeNames = new List<string>(); //Used for manual rename form - Populates ListOfEpisodeNames based on format selection
        ConfigFile cf = new ConfigFile();

        bool filenameListboxFlag = false;
        bool changedFileNamesListboxFlag = false;

        int filenameListboxLastIndex = 0;
        public TVForm()
        {
            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls11 | System.Net.SecurityProtocolType.Tls12;

            InitializeComponent();
            AuthenticateWithTVDB(User_Name, User_Key, API_Key); //Writes Authorization token to global variable...

            this.fileNamesListbox.DragDrop += new System.Windows.Forms.DragEventHandler(this.fileNamesListbox_DragDrop);
            this.fileNamesListbox.DragEnter += new System.Windows.Forms.DragEventHandler(this.fileNamesListbox_DragEnter);

            this.parentPathLabel.DragDrop += new System.Windows.Forms.DragEventHandler(this.parentPathLabel_DragDrop);
            this.parentPathLabel.DragEnter += new System.Windows.Forms.DragEventHandler(this.parentPathLabel_DragEnter);
        }
        private async void GETRequest(string AuthToken, string URL)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.thetvdb.com/search/series?name=Titans"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");
                    request.Headers.TryAddWithoutValidation("Authorization", "Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE1NzQxNzA2MzgsImlkIjoiT2xkcm95ZCBNZWRpYSBGaWxlIFJlbmFtZXIiLCJvcmlnX2lhdCI6MTU3NDA4NDIzOCwidXNlcmlkIjoxMTU5NTksInVzZXJuYW1lIjoiRHVzdGluLk9sZHJveWRAZ21haWwuY29tIn0.2ZHj400dcPxG9FEfi-LF0hWEPG6V0L-rbqp4eNt_I7xmkxPU8H6fVI_BVtRfI7gGE31mBqvTPkOhR9nz1KL-lrZcz2JCtNkQeqwfY0eTGHFlbxt3YO-2NPTgT0Va2YhQp4Jxa9yMXaAXgyal52DMfPPKlRpyff7PYedshTuRR0TVl6uDIUyi4fKM0cMi3YvunbXpB2JklC4zf1Fwr02X12cwj03OQ-3peTQMH4swXwJtmE3duyYSTWWfbrF80Widz5-kwQX7lkjOPUO-DtsuUBUuJou1ZDYDlqcDEG-PXWdX0VItI7nGF7_YjL8e9HhzwsHSmdzmNDiEPpX2_s5veg");

                    var response = await httpClient.SendAsync(request);
                    var result = response.Content.ReadAsStringAsync();
                    dynamic jsonObject = JObject.Parse(result.Result);
                    dynamic jsonObejct2 = JsonConvert.DeserializeObject(result.Result.ToString());
                }
            }
        }
        private async void AuthenticateWithTVDB(string UserName, string UserKey, string APIKey)
        {
            using (var httpClient = new HttpClient())
            {
                using (var request = new HttpRequestMessage(new HttpMethod("POST"), "https://api.thetvdb.com/login"))
                {
                    request.Headers.TryAddWithoutValidation("Accept", "application/json");

                    request.Content = new StringContent("{\n  \"apikey\": \"" + APIKey + "\",\n  \"userkey\": \"" + UserKey + "\",\n  \"username\": \"" + UserName + "\"\n}");
                    request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                    HttpResponseMessage response = await httpClient.SendAsync(request);

                    var result = response.Content.ReadAsStringAsync();

                    Authorization_Token = Program.GeneralParser(result.Result.ToString(), "\":\"", "\"}");

                }
            }
        }



        private void fileNamesListbox_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void fileNamesListbox_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string fileName = "";

            if (s.Count() == 1 && Directory.Exists(s[0].ToString())) //Single Directory Dropped
            {
                EpisodePathList.Clear();

                NLabelUpdate("Querying Files",Color.GreenYellow);

                fileNamesListbox.Items.Clear();

                cf.DefaultSettings["TFPath"] = s[0].ToString();
                cf.UpdateDefaults();

                parentPathLabel.Text = s[0].ToString() + "\\"; //adds a \ to the end of folderpath, double backslash required to add a single one to a string

                if (recursiveCB.Checked)
                {
                    string[] fileNames = Directory
                    .GetFiles(cf.DefaultSettings["TFPath"], "*.*", SearchOption.AllDirectories)
                    .Where(file => file.ToLower().EndsWith(".mpg")
                    || file.ToLower().EndsWith(".mpeg")
                    || file.ToLower().EndsWith(".vob")
                    || file.ToLower().EndsWith(".mod")
                    || file.ToLower().EndsWith(".ts")
                    || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4")
                    || file.ToLower().EndsWith(".m4v")
                    || file.ToLower().EndsWith(".mov")
                    || file.ToLower().EndsWith("avi")
                    || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv")
                    || file.ToLower().EndsWith(".asf")
                    || file.ToLower().EndsWith(".mkv")
                    || file.ToLower().EndsWith(".flv")
                    || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr")
                    || file.ToLower().EndsWith(".dvr-ms")
                    || file.ToLower().EndsWith(".wtv")
                    || file.ToLower().EndsWith(".ogv")
                    || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp")
                    || file.ToLower().EndsWith(".rm")
                    || file.ToLower().EndsWith(".rmvb")
                    || file.ToLower().EndsWith(".srt")  //Add Subtitle File Extensions also
                    || file.ToLower().EndsWith(".sub")
                    || file.ToLower().EndsWith(".idx")
                    || file.ToLower().EndsWith(".ssa")
                    || file.ToLower().EndsWith(".ass")
                    || file.ToLower().EndsWith(".smi")
                    || file.ToLower().EndsWith(".vtt")).ToArray();

                    foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                    {
                        EpisodePathList.Add(file);

                        fileName = file.Replace(cf.DefaultSettings["TFPath"] + "\\", "");

                        if (!fileName.StartsWith("._"))
                        {
                            fileNamesListbox.Items.Add(fileName);
                        }
                    }
                }
                else
                {
                    string[] fileNames = Directory
                    .GetFiles(cf.DefaultSettings["TFPath"], "*.*")
                    .Where(file => file.ToLower().EndsWith(".mpg")
                    || file.ToLower().EndsWith(".mpeg")
                    || file.ToLower().EndsWith(".vob")
                    || file.ToLower().EndsWith(".mod")
                    || file.ToLower().EndsWith(".ts")
                    || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4")
                    || file.ToLower().EndsWith(".m4v")
                    || file.ToLower().EndsWith(".mov")
                    || file.ToLower().EndsWith("avi")
                    || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv")
                    || file.ToLower().EndsWith(".asf")
                    || file.ToLower().EndsWith(".mkv")
                    || file.ToLower().EndsWith(".flv")
                    || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr")
                    || file.ToLower().EndsWith(".dvr-ms")
                    || file.ToLower().EndsWith(".wtv")
                    || file.ToLower().EndsWith(".ogv")
                    || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp")
                    || file.ToLower().EndsWith(".rm")
                    || file.ToLower().EndsWith(".rmvb")
                    || file.ToLower().EndsWith(".srt")  //Add Subtitle File Extensions also
                    || file.ToLower().EndsWith(".sub")
                    || file.ToLower().EndsWith(".idx")
                    || file.ToLower().EndsWith(".ssa")
                    || file.ToLower().EndsWith(".ass")
                    || file.ToLower().EndsWith(".smi")
                    || file.ToLower().EndsWith(".vtt")).ToArray();

                    foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                    {
                        EpisodePathList.Add(file);
                        fileName = file.Replace(cf.DefaultSettings["TFPath"] + "\\", "");

                        if (!fileName.StartsWith("._"))
                        {
                            fileNamesListbox.Items.Add(fileName);
                        }
                    }
                }

                notificationLabel.Visible = false;

            }
            else if(s.Count() > 1)
            {
                NLabelUpdate("Only one item can be dropped at a time", Color.Red);
            }
            else if(!Directory.Exists(s[0].ToString())) //Not a directory
            {
                NLabelUpdate("Only directories can be dropped on form",Color.Red);
            }
        }


        private void parentPathLabel_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void parentPathLabel_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            string fileName = "";

            if (s.Count() == 1 && Directory.Exists(s[0].ToString())) //Single Directory Dropped
            {
                EpisodePathList.Clear();

                NLabelUpdate("Querying Files", Color.GreenYellow);

                fileNamesListbox.Items.Clear();

                cf.DefaultSettings["TFPath"] = s[0].ToString();
                cf.UpdateDefaults();

                parentPathLabel.Text = s[0].ToString() + "\\"; //adds a \ to the end of folderpath, double backslash required to add a single one to a string

                if (recursiveCB.Checked)
                {
                    string[] fileNames = Directory
                    .GetFiles(cf.DefaultSettings["TFPath"], "*.*", SearchOption.AllDirectories)
                    .Where(file => file.ToLower().EndsWith(".mpg")
                    || file.ToLower().EndsWith(".mpeg")
                    || file.ToLower().EndsWith(".vob")
                    || file.ToLower().EndsWith(".mod")
                    || file.ToLower().EndsWith(".ts")
                    || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4")
                    || file.ToLower().EndsWith(".m4v")
                    || file.ToLower().EndsWith(".mov")
                    || file.ToLower().EndsWith("avi")
                    || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv")
                    || file.ToLower().EndsWith(".asf")
                    || file.ToLower().EndsWith(".mkv")
                    || file.ToLower().EndsWith(".flv")
                    || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr")
                    || file.ToLower().EndsWith(".dvr-ms")
                    || file.ToLower().EndsWith(".wtv")
                    || file.ToLower().EndsWith(".ogv")
                    || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp")
                    || file.ToLower().EndsWith(".rm")
                    || file.ToLower().EndsWith(".rmvb")
                    || file.ToLower().EndsWith(".srt")  //Add Subtitle File Extensions also
                    || file.ToLower().EndsWith(".sub")
                    || file.ToLower().EndsWith(".idx")
                    || file.ToLower().EndsWith(".ssa")
                    || file.ToLower().EndsWith(".ass")
                    || file.ToLower().EndsWith(".smi")
                    || file.ToLower().EndsWith(".vtt")).ToArray();

                    foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                    {
                        EpisodePathList.Add(file);

                        fileName = file.Replace(cf.DefaultSettings["TFPath"] + "\\", "");

                        if (!fileName.StartsWith("._"))
                        {
                            fileNamesListbox.Items.Add(fileName);
                        }
                    }
                }
                else
                {
                    string[] fileNames = Directory
                    .GetFiles(cf.DefaultSettings["TFPath"], "*.*")
                    .Where(file => file.ToLower().EndsWith(".mpg")
                    || file.ToLower().EndsWith(".mpeg")
                    || file.ToLower().EndsWith(".vob")
                    || file.ToLower().EndsWith(".mod")
                    || file.ToLower().EndsWith(".ts")
                    || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4")
                    || file.ToLower().EndsWith(".m4v")
                    || file.ToLower().EndsWith(".mov")
                    || file.ToLower().EndsWith("avi")
                    || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv")
                    || file.ToLower().EndsWith(".asf")
                    || file.ToLower().EndsWith(".mkv")
                    || file.ToLower().EndsWith(".flv")
                    || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr")
                    || file.ToLower().EndsWith(".dvr-ms")
                    || file.ToLower().EndsWith(".wtv")
                    || file.ToLower().EndsWith(".ogv")
                    || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp")
                    || file.ToLower().EndsWith(".rm")
                    || file.ToLower().EndsWith(".rmvb")
                    || file.ToLower().EndsWith(".srt")  //Add Subtitle File Extensions also
                    || file.ToLower().EndsWith(".sub")
                    || file.ToLower().EndsWith(".idx")
                    || file.ToLower().EndsWith(".ssa")
                    || file.ToLower().EndsWith(".ass")
                    || file.ToLower().EndsWith(".smi")
                    || file.ToLower().EndsWith(".vtt")).ToArray();

                    foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                    {
                        EpisodePathList.Add(file);
                        fileName = file.Replace(cf.DefaultSettings["TFPath"] + "\\", "");

                        if (!fileName.StartsWith("._"))
                        {
                            fileNamesListbox.Items.Add(fileName);
                        }
                    }
                }

                notificationLabel.Visible = false;

            }
            else if (s.Count() > 1)
            {
                NLabelUpdate("Only one item can be dropped at a time", Color.Red);
            }
            else if (!Directory.Exists(s[0].ToString())) //Not a directory
            {
                NLabelUpdate("Only directories can be dropped on form", Color.Red);
            }
        }


        private void NLabelUpdate(string notificationText, Color color)
        {
            notificationLabel.Visible = true;
            notificationLabel.ForeColor = color;
            notificationLabel.Text = notificationText;
            notificationLabel.Invalidate();
            notificationLabel.Update();
        }

        private void GetHTML()
        {

            string TitleBox = SeriesIDTitleTextbox.Text;

            NLabelUpdate("Searching..." + SeriesIDTitleTextbox.Text, Color.GreenYellow);


            //Keeps episode names list clear in cases of searching many Series in one session.
            KODIEpisodeNames.Clear();
            PLEXEpisodeNames.Clear();
            SynologyEpisodeName.Clear();

            //Scan for Series ID, if no ID found scan for Series title and retrieve list of possible Series to choose from then parse out the correct id.
            if (int.TryParse(TitleBox, out int ID) && ID != 0)
            {
                try
                { 
                    NLabelUpdate("ID found, gathering Series info.",Color.GreenYellow);

                    TVSeriesInfo T = new TVSeriesInfo(Authorization_Token, ID.ToString());

                    SeriesImagePicturebox.ImageLocation = "https://thetvdb.com/" + T.series["banner"];
                    SeriesInfo = T;
                    if (T.series.ContainsKey("seriesName")) { favoritesCombo.Text = T.series["seriesName"]; }
                    SeriesIDTitleTextbox.Text = T.Series_ID;
                }
                catch //ID Search returned error, attempt search as a Series title instead.
                {
                    NLabelUpdate("ID search failed, searching " + TitleBox + "as text", Color.Red);

                    //Create object that looks up possible TV Series based on text in SeriesURLBox
                    TVSeriesSearch S = new TVSeriesSearch(TitleBox, Authorization_Token);
                    //Create form object to hold results from search
                    if (S.SeriesList.Count > 1)
                    {
                        NLabelUpdate("Multiple Series Identified", Color.GreenYellow);

                        TVSeriesSelection M = new TVSeriesSelection(S.SeriesList);
                        //Show form as dialog to prevent further code from running until option selected.
                        M.ShowDialog();

                        if (M.DialogResult == DialogResult.OK)
                        {
                            NLabelUpdate("Selection Accepted, gathering Series info", Color.GreenYellow);


                            //Once show is selected, use selected shows ID to gather episode information
                            TVSeriesInfo T = new TVSeriesInfo(Authorization_Token, M.SelectedID);

                            //display Series banner 
                            SeriesImagePicturebox.ImageLocation = "https://thetvdb.com/" + T.series["banner"];
                            SeriesInfo = T;

                            if (T.series.ContainsKey("seriesName")) { favoritesCombo.Text = T.series["seriesName"]; }
                            SeriesIDTitleTextbox.Text = T.Series_ID;
                        }
                        else if (M.DialogResult == DialogResult.Abort || M.DialogResult == DialogResult.Cancel) { return; }
                    }
                    else if (S.SeriesList.Count == 1)
                    {
                        NLabelUpdate("Series Identified, gathering Series info", Color.GreenYellow);


                        TVSeriesInfo T = new TVSeriesInfo(Authorization_Token, S.SeriesList[0]["seriesid"]);
                        SeriesImagePicturebox.ImageLocation = "https://thetvdb.com/" + T.series["banner"];
                        SeriesInfo = T;
                        if (T.series.ContainsKey("seriesName")) { favoritesCombo.Text = T.series["seriesName"]; }
                        SeriesIDTitleTextbox.Text = T.Series_ID;
                    }
                    else { CustomMessageBox.Show("No such show found", 170, 310); return; }
                }
            }
            //Create object that looks up possible TV Series based on the text in the Series Title Box
            else if (!string.IsNullOrEmpty(TitleBox))
            {
                NLabelUpdate("Searching..." + SeriesIDTitleTextbox.Text, Color.GreenYellow);

                //Create object that looks up possible TV Series based on text in SeriesURLBox
                TVSeriesSearch S = new TVSeriesSearch(TitleBox, Authorization_Token);
                //Create form object to hold results from search
                if (S.SeriesList.Count > 1)
                {
                    NLabelUpdate("Multiple Series Identified", Color.GreenYellow);

                    TVSeriesSelection M = new TVSeriesSelection(S.SeriesList);
                    //Show form as dialog to prevent further code from running until option selected.
                    M.ShowDialog();

                    if (M.DialogResult == DialogResult.OK)
                    {
                        NLabelUpdate("Selection Accepted, gathering Series info", Color.GreenYellow);

                        //Once show is selected, use selected shows ID to gather episode information
                        TVSeriesInfo T = new TVSeriesInfo(Authorization_Token, M.SelectedID);
                        //display Series banner 
                        if (T.series.ContainsKey("banner"))
                        {
                            SeriesImagePicturebox.ImageLocation = "https://thetvdb.com/" + T.series["banner"];
                        }
                        SeriesInfo = T;
                        if (T.series.ContainsKey("seriesName")) { favoritesCombo.Text = T.series["seriesName"]; }
                        SeriesIDTitleTextbox.Text = T.Series_ID;
                    }
                    else if (M.DialogResult == DialogResult.Abort || M.DialogResult == DialogResult.Cancel) { return; }
                }
                else if (S.SeriesList.Count == 1)
                {
                    NLabelUpdate("Series Identified, gathering Series info", Color.GreenYellow);

                    TVSeriesInfo T = new TVSeriesInfo(Authorization_Token, S.SeriesList[0]["seriesid"]);
                    if (T.series.ContainsKey("banner"))
                    {
                        SeriesImagePicturebox.ImageLocation = "https://thetvdb.com/" + T.series["banner"];
                    }
                    
                    SeriesInfo = T;
                    if (T.series.ContainsKey("seriesName")) { favoritesCombo.Text = T.series["seriesName"]; }
                    SeriesIDTitleTextbox.Text = T.Series_ID;
                }
                else { CustomMessageBox.Show("No such show found", 170, 310); return; }
            }
            else { CustomMessageBox.Show("Please enter Series ID, or name, or URL", 182, 317); return; }

            for (int i = 0; i < SeriesInfo.EpisodeList.Count; i++)
            {
                if (SeriesInfo.EpisodeList[i].ContainsKey("episodeName")
                    & SeriesInfo.EpisodeList[i].ContainsKey("airedEpisodeNumber")
                    & SeriesInfo.EpisodeList[i].ContainsKey("airedSeason"))
                {
                    string Snum = "";
                    string Enum = "";
                    if (int.Parse(SeriesInfo.EpisodeList[i]["airedSeason"]) < 10)
                    {
                        Snum = "S0" + SeriesInfo.EpisodeList[i]["airedSeason"];
                    }
                    else
                    {
                        Snum = "S" + SeriesInfo.EpisodeList[i]["airedSeason"];
                    }
                    if (int.Parse(SeriesInfo.EpisodeList[i]["airedEpisodeNumber"]) < 10)
                    {
                        Enum = "E0" + SeriesInfo.EpisodeList[i]["airedEpisodeNumber"];
                    }
                    else
                    {
                        Enum = "E" + SeriesInfo.EpisodeList[i]["airedEpisodeNumber"];
                    }
                    //ext not populated yet
                    PLEXEpisodeNames.Add(SeriesInfo.series["seriesName"] + " - " + Snum.ToLower() + Enum.ToLower() + " - " + SeriesInfo.EpisodeList[i]["episodeName"]);
                    KODIEpisodeNames.Add(SeriesInfo.series["seriesName"] + "_" + Snum.ToLower() + Enum.ToLower() + "_" + SeriesInfo.EpisodeList[i]["episodeName"]);
                    SynologyEpisodeName.Add(SeriesInfo.series["seriesName"] + "." + Snum + "." + Enum + "." + SeriesInfo.EpisodeList[i]["episodeName"]);
                }
            }

            NLabelUpdate("", Color.GreenYellow);
        }
        private void GetHTMLButton_Click(object sender, EventArgs e) { GetHTML(); }
        private void GetFileNames()
        {
            EpisodePathList.Clear();

            NLabelUpdate("Querying Files", Color.GreenYellow);

            string fileName = "";
            fileNamesListbox.Items.Clear();

            FolderBrowserDialog FBD = new FolderBrowserDialog(); //creates new instance of the FolderBrowserDialog

            if (!string.IsNullOrEmpty(cf.DefaultSettings["TFPath"])) //if defaultpath contains a path, sets folderBrowserDialog to default to this path
            {
                FBD.SelectedPath = cf.DefaultSettings["TFPath"];
            }

            if (FBD.ShowDialog() == DialogResult.OK) //shows folderbrowserdialog, runs addtional code if not cancelled out
            {
                cf.DefaultSettings["TFPath"] = FBD.SelectedPath;
                cf.UpdateDefaults(); //updates default settings

                parentPathLabel.Text = FBD.SelectedPath.ToString() + "\\"; //adds a \ to the end of folderpath, double backslash required to add a single one to a string


                /*"Video Files|*.mpg;*.mpeg;*.vob;*.mod;*.ts;*.m2ts;*.mp4;*.m4v;*.mov;*.avi;*.divx;*.wmv;"
                +"*.asf;*.mkv;*.flv;*.f4v;*.dvr;*.dvr-ms;*.wtv;*.ogv;*.ogm;*.3gp;*.rm;*.rmvb;";*/

                /*Subtitle FIles| .srt, .sub, .idx, .ssa, .ass, .smi, .vtt*/

                if(recursiveCB.Checked)
                {
                    string[]fileNames = Directory
                    .GetFiles(cf.DefaultSettings["TFPath"], "*.*", SearchOption.AllDirectories)
                    .Where(file => file.ToLower().EndsWith(".mpg")
                    || file.ToLower().EndsWith(".mpeg")
                    || file.ToLower().EndsWith(".vob")
                    || file.ToLower().EndsWith(".mod")
                    || file.ToLower().EndsWith(".ts")
                    || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4")
                    || file.ToLower().EndsWith(".m4v")
                    || file.ToLower().EndsWith(".mov")
                    || file.ToLower().EndsWith("avi")
                    || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv")
                    || file.ToLower().EndsWith(".asf")
                    || file.ToLower().EndsWith(".mkv")
                    || file.ToLower().EndsWith(".flv")
                    || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr")
                    || file.ToLower().EndsWith(".dvr-ms")
                    || file.ToLower().EndsWith(".wtv")
                    || file.ToLower().EndsWith(".ogv")
                    || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp")
                    || file.ToLower().EndsWith(".rm")
                    || file.ToLower().EndsWith(".rmvb")
                    || file.ToLower().EndsWith(".srt")  //Add Subtitle File Extensions also
                    || file.ToLower().EndsWith(".sub")
                    || file.ToLower().EndsWith(".idx")
                    || file.ToLower().EndsWith(".ssa")
                    || file.ToLower().EndsWith(".ass")
                    || file.ToLower().EndsWith(".smi")
                    || file.ToLower().EndsWith(".vtt")).ToArray();

                    foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                    {
                        EpisodePathList.Add(file);

                        fileName = file.Replace(cf.DefaultSettings["TFPath"] + "\\", "");

                        if (!fileName.StartsWith("._"))
                        {
                            fileNamesListbox.Items.Add(fileName);
                        }
                    }
                }
                else
                {
                    string[]fileNames = Directory
                    .GetFiles(cf.DefaultSettings["TFPath"], "*.*")
                    .Where(file => file.ToLower().EndsWith(".mpg")
                    || file.ToLower().EndsWith(".mpeg")
                    || file.ToLower().EndsWith(".vob")
                    || file.ToLower().EndsWith(".mod")
                    || file.ToLower().EndsWith(".ts")
                    || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4")
                    || file.ToLower().EndsWith(".m4v")
                    || file.ToLower().EndsWith(".mov")
                    || file.ToLower().EndsWith("avi")
                    || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv")
                    || file.ToLower().EndsWith(".asf")
                    || file.ToLower().EndsWith(".mkv")
                    || file.ToLower().EndsWith(".flv")
                    || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr")
                    || file.ToLower().EndsWith(".dvr-ms")
                    || file.ToLower().EndsWith(".wtv")
                    || file.ToLower().EndsWith(".ogv")
                    || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp")
                    || file.ToLower().EndsWith(".rm")
                    || file.ToLower().EndsWith(".rmvb")
                    || file.ToLower().EndsWith(".srt")  //Add Subtitle File Extensions also
                    || file.ToLower().EndsWith(".sub")
                    || file.ToLower().EndsWith(".idx")
                    || file.ToLower().EndsWith(".ssa")
                    || file.ToLower().EndsWith(".ass")
                    || file.ToLower().EndsWith(".smi")
                    || file.ToLower().EndsWith(".vtt")).ToArray();

                    foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                    {
                        EpisodePathList.Add(file);
                        fileName = file.Replace(cf.DefaultSettings["TFPath"] + "\\", "");

                        if (!fileName.StartsWith("._"))
                        {
                            fileNamesListbox.Items.Add(fileName);
                        }
                    }
                }
                
            }
            notificationLabel.Visible = false;
        }
        private void GetFileNamesButton_Click(object sender, EventArgs e)
        {
            GetFileNames();
        }
        private void PreviewChanges()
        {
            NLabelUpdate("", Color.GreenYellow);


            if (fileNamesListbox.Items.Count > 0 && SeriesInfo != null)
            {
                changedFileNamesListbox.Items.Clear(); //clears listbox

                NLabelUpdate("Determining Episode Numbers from Filename", Color.GreenYellow);

                DetermineEpisodeFromFileName(); //sets season and episode from filename
                for (int i = 0; i < fileNamesListbox.SelectedIndices.Count; i++)
                {
                    changedFileNamesListbox.SelectedIndices.Add(fileNamesListbox.SelectedIndices[i]);
                }
            }
            else
            {
                CustomMessageBox.Show("Files must be selected and HTML must be created", 125, 277);
            }
            notificationLabel.Visible = false;
        }
        private void PreviewChangesButton_Click(object sender, EventArgs e)
        {
            PreviewChanges();
        }

        private string cleanString(string input)
        {
            string output = input;
            List<string> spaceReplace = new List<string>
            {
                ".",
                "_"
            };

            List<string> reservedCharacters = new List<string>
            {
                "<",
                ">",
                ":",
                "\"",
                "/",
                "\\",
                "|",
                "?",
                "*"
            };

            for (int i = 0; i < spaceReplace.Count(); i++)
            {
                output = output.Replace(spaceReplace[i], " "); 
            }

            for (int i = 0; i < reservedCharacters.Count(); i++)
            {
                output = output.Replace(reservedCharacters[i], "");
            }

            return output;
        }
        private void DetermineEpisodeFromFileName()
        {
            season = "";
            episode = "";
            char[] delim = { '.' };
            List<string> episodeNames = new List<string>();

            int maxSeason = 0; //Used to restrict season loops
            int maxEpisode = 0; //used to restrict episode loops

            if (fileNamesListbox.Items.Count > 0) //checks that fileNamesListbox is not empty
            {
                
                //look for highest season number, highest episode nmber and build list of all Episodes of the show.
                for (int i = 0; i < SeriesInfo.EpisodeList.Count(); i++)
                {
                    if (SeriesInfo.EpisodeList[i].ContainsKey("airedSeason") && int.Parse(SeriesInfo.EpisodeList[i]["airedSeason"]) > maxSeason)
                    {
                        maxSeason = int.Parse(SeriesInfo.EpisodeList[i]["airedSeason"]);
                    }

                    //Look for largest episode number in all seasons of the show.
                    if (SeriesInfo.EpisodeList[i].ContainsKey("airedEpisodeNumber") && int.Parse(SeriesInfo.EpisodeList[i]["airedEpisodeNumber"]) > maxEpisode)
                    {
                        maxEpisode = int.Parse(SeriesInfo.EpisodeList[i]["airedEpisodeNumber"]);
                    }

                    if (SeriesInfo.EpisodeList[i].ContainsKey("episodeName"))
                    {
                        episodeNames.Add(SeriesInfo.EpisodeList[i]["episodeName"]);
                    }
                }

                //Check for selected items, if there are items selected then for the non selected items set the season and episode numbers as -1 to effectively disable them
                for (int i = 0; i < fileNamesListbox.Items.Count; i++) //loop through listbox items
                {
                    string[] Tokens = fileNamesListbox.Items[i].ToString().Split(delim);
                    ext = Tokens[Tokens.Count() - 1]; //should be extension
                    season = "";
                    episode = "";
                    string newTitle = "";
                    bool isSelected = false;

                    //Check for episode title in filename first
                    
                    if (fileNamesListbox.SelectedIndices.Count > 0)
                    {

                        for (int a = 0; a < fileNamesListbox.SelectedIndices.Count; a++)
                        {
                            //If its found to exist in the selected indices
                            if (fileNamesListbox.SelectedIndices[a] == i)
                            {
                                isSelected = true;
                            }
                        }
                        if (isSelected)
                        {
                            //Add loop for each episode to see if an episode title in the SeriesInfo object matches a filename. If so return the episode and season from that.
                            for (int a = 0; a < SeriesInfo.EpisodeList.Count; a++)
                            {
                                if (SeriesInfo.EpisodeList[a].ContainsKey("episodeName"))
                                {

                                    if ((cleanString(fileNamesListbox.Items[i].ToString()).ToUpper()).Replace(SeriesInfo.series["seriesName"], "").Contains(cleanString(SeriesInfo.EpisodeList[a]["episodeName"]).ToUpper()))
                                    {

                                        if (int.Parse(SeriesInfo.EpisodeList[a]["airedSeason"]) < 10)
                                        {
                                            season = "S0" + SeriesInfo.EpisodeList[a]["airedSeason"];
                                        }
                                        else
                                        {
                                            season = "S" + SeriesInfo.EpisodeList[a]["airedSeason"];
                                        }

                                        if (int.Parse(SeriesInfo.EpisodeList[a]["airedEpisodeNumber"]) < 10)
                                        {
                                            episode = "E0" + SeriesInfo.EpisodeList[a]["airedEpisodeNumber"];
                                        }
                                        else
                                        {
                                            episode = "E" + SeriesInfo.EpisodeList[a]["airedEpisodeNumber"];
                                        }

                                        //add episode and season info to the newTitle variable;

                                        /*TitleFormats
                                            Plex: ShowName - sXXeYY - Optional_info.ext
                                                Grey's Anatomy - S01e02 - The First Cut is the Deepest.avi

                                            Synology: ShowName.sXXeYY.ext
                                                Grey's Anatomy.S01.E02.avi
                                                Grey's Anatomy.S01.E02.The First Cut is the Deepest.avi

                                            Kodi/XBMC: ShowName_sXXeYY.ext
                                                Grey's Anatomy_S01E02.avi
                                                Grey's Anatomy_S01E02_The First Cut is the Deepest.avi
                                            */
                                        switch (formatCombo.SelectedIndex)
                                        {

                                            case 0: //PLEX
                                                newTitle = SeriesInfo.series["seriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                                break;
                                            case 1: //KODI
                                                newTitle = SeriesInfo.series["seriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                                break;
                                            case 2: //Synology
                                                newTitle = SeriesInfo.series["seriesName"] + "." + season + "." + episode + "." + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                                break;
                                            default: //Plex
                                                newTitle = SeriesInfo.series["seriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                                break;
                                        }
                                        //newTitle = SeriesInfo.series["seriesName"] + " " + season + episode + " " + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                    }
                                }
                            }
                        }
                        else
                        {
                            season = "-1";
                            episode = "-1";
                        }

                    }
                    else
                    {
                        //Add loop for each episode to see if an episode title in the SeriesInfo object matches a filename. If so return the episode and season from that.
                        for (int a = 0; a < SeriesInfo.EpisodeList.Count; a++)
                        {
                            if (SeriesInfo.EpisodeList[a].ContainsKey("episodeName"))
                            {
                                //Cleans filename of Series Name, common space replacing characters before checking that episode title is in filename. 
                                //This ensures if an epsisode is named the same as the series name, only that episode with fail and every file won't be matched to that episode.
                                if ((cleanString(fileNamesListbox.Items[i].ToString()).ToUpper()).Replace(SeriesInfo.series["seriesName"],"").Contains(cleanString(SeriesInfo.EpisodeList[a]["episodeName"]).ToUpper()))
                                {

                                    if (int.Parse(SeriesInfo.EpisodeList[a]["airedSeason"]) < 10)
                                    {
                                        season = "S0" + SeriesInfo.EpisodeList[a]["airedSeason"];
                                    }
                                    else
                                    {
                                        season = "S" + SeriesInfo.EpisodeList[a]["airedSeason"];
                                    }

                                    if (int.Parse(SeriesInfo.EpisodeList[a]["airedEpisodeNumber"]) < 10)
                                    {
                                        episode = "E0" + SeriesInfo.EpisodeList[a]["airedEpisodeNumber"];
                                    }
                                    else
                                    {
                                        episode = "E" + SeriesInfo.EpisodeList[a]["airedEpisodeNumber"];
                                    }

                                    //add episode and season info to the newTitle variable;

                                    /*TitleFormats
                                        Plex: ShowName - sXXeYY - Optional_info.ext
                                            Grey's Anatomy - S01e02 - The First Cut is the Deepest.avi

                                        Synology: ShowName.sXXeYY.ext
                                            Grey's Anatomy.S01.E02.avi
                                            Grey's Anatomy.S01.E02.The First Cut is the Deepest.avi

                                        Kodi/XBMC: ShowName_sXXeYY.ext
                                            Grey's Anatomy_S01E02.avi
                                            Grey's Anatomy_S01E02_The First Cut is the Deepest.avi
                                        */
                                    switch (formatCombo.SelectedIndex)
                                    {

                                        case 0: //PLEX
                                            newTitle = SeriesInfo.series["seriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                            break;
                                        case 1: //KODI
                                            newTitle = SeriesInfo.series["seriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                            break;
                                        case 2: //Synology
                                            newTitle = SeriesInfo.series["seriesName"] + "." + season + "." + episode + "." + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                            break;
                                        default: //Plex
                                            newTitle = SeriesInfo.series["seriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                            break;
                                    }
                                    //newTitle = SeriesInfo.series["seriesName"] + " " + season + episode + " " + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                }
                            }
                        }

                    }



                    if ((string.IsNullOrEmpty(season) | season == "-1" | string.IsNullOrEmpty(episode) | episode == "-1"))
                    {
                        //loop through selected indecies to ensure item is selected or not
                        if (fileNamesListbox.SelectedIndices.Count > 0)
                        {
                            for (int a = 0; a < fileNamesListbox.SelectedIndices.Count; a++)
                            {
                                //If its found to exist in the selected indices
                                if (fileNamesListbox.SelectedIndices[a] == i)
                                {
                                    isSelected = true;
                                }
                            }
                            if (isSelected)
                            {
                                season = CheckSeason(fileNamesListbox.Items[i].ToString().ToUpper(), maxSeason); //tries to parse season info from filename
                                episode = CheckEpisode(fileNamesListbox.Items[i].ToString().ToUpper(), maxEpisode); //tries to parse episode info from filename
                            }
                            else
                            {
                                season = "-1";
                                episode = "-1";
                            }
                        }
                        else //None selected, perform check on the entire list.
                        {
                            season = CheckSeason(fileNamesListbox.Items[i].ToString().ToUpper(), maxSeason); //tries to parse season info from filename
                            episode = CheckEpisode(fileNamesListbox.Items[i].ToString().ToUpper(), maxEpisode); //tries to parse episode info from filename
                        }

                    }

                    //Selections made, other items are marked with -1 and then skipped
                    if (season == "-1") { newTitle = "SKIPPED"; }

                    if ((string.IsNullOrEmpty(season) | string.IsNullOrEmpty(episode)))
                    {
                        // Add filter for absolute episode numbers here
                        newTitle = CheckAbsoluteNumber(fileNamesListbox.Items[i].ToString());
                        if (string.IsNullOrEmpty(newTitle)) { newTitle = "EPISODE COULD NOT BE DETERMINED"; }
                    }

                    if (string.IsNullOrEmpty(newTitle))//Check for matching episode and season entry
                    {
                        for (int e = 0; e < SeriesInfo.EpisodeList.Count(); e++)
                        {
                            if (SeriesInfo.EpisodeList[e].ContainsKey("airedEpisodeNumber")
                                & SeriesInfo.EpisodeList[e]["airedEpisodeNumber"] == episode)
                            {
                                if (SeriesInfo.EpisodeList[e].ContainsKey("airedSeason")
                                    & SeriesInfo.EpisodeList[e]["airedSeason"] == season)
                                {
                                    if (int.Parse(season) < 10) { season = "S0" + season; }
                                    else { season = "S" + season; }
                                    if (int.Parse(episode) < 10) { episode = "E0" + episode; }
                                    else { episode = "E" + episode; }

                                    //add episode and season info to the newTitle variable;

                                    /*TitleFormats
                                        Plex: ShowName - sXXeYY - Optional_info.ext
                                            Grey's Anatomy - S01e02 - The First Cut is the Deepest.avi
                                        
                                        Synology: ShowName.sXXeYY.ext
                                            Grey's Anatomy.S01.E02.avi
                                            Grey's Anatomy.S01.E02.The First Cut is the Deepest.avi

                                        Kodi/XBMC: ShowName_sXXeYY.ext
                                            Grey's Anatomy_S01E02.avi
                                            Grey's Anatomy_S01E02_The First Cut is the Deepest.avi
                                     */
                                    switch (formatCombo.SelectedIndex)
                                    {
                                        
                                        case 0: //PLEX
                                            newTitle = SeriesInfo.series["seriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.EpisodeList[e]["episodeName"] + "." + ext;
                                            break;
                                        case 1: //KODI
                                            newTitle = SeriesInfo.series["seriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.EpisodeList[e]["episodeName"] + "." + ext;
                                            break;
                                        case 2: //Synology
                                            newTitle = SeriesInfo.series["seriesName"] + "." + season + "." + episode + "." + SeriesInfo.EpisodeList[e]["episodeName"] + "." + ext;
                                            break;
                                        default: //Synology
                                            newTitle = SeriesInfo.series["seriesName"] + "." + season + "." + episode + "." + SeriesInfo.EpisodeList[e]["episodeName"] + "." + ext;
                                            break;
                                    }
                                    //newTitle = SeriesInfo.series["seriesName"] + " " + season + episode + " " + SeriesInfo.EpisodeList[e]["episodeName"] + "." + ext;
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(newTitle) & !string.IsNullOrEmpty(season) & !string.IsNullOrEmpty(episode)) { newTitle = "NO SUCH EPISODE FOUND"; }
                        else if (string.IsNullOrEmpty(newTitle)) { newTitle = "EPISODE COULD NOT BE DETERMINED"; }
                        else if (newTitle == "SKIPPED") { newTitle =""; MessageBox.Show("title change"); } //items marked as SKIPPED are items that were not selected and thus not evaluated.
                    }
                    if (newTitle == "SKIPPED") { newTitle = ""; }
                    else
                    {
                        newTitle = FormatFileName(newTitle); //removes invalid characters from the filename.
                    }
                    changedFileNamesListbox.Items.Add(newTitle);
                }
            }
            episodeNames.Clear();
        }
        private string CheckSeason(string FileName, int maxSeason)
        {
            int seasonNumber = -1;

            /*Remove items from string that might confuse the program.
            List is stored in string array lineStringFilter*/
            for (int i = 0; i < lineStringFilter.Length; i++)
            {
                if (FileName.Contains(lineStringFilter[i]))
                {
                    FileName = FileName.Replace(lineStringFilter[i], "");
                }
            }
            /*Check for season # in parent folder
            char[] delim = { '\\' };
            string[] Tokens = folderPath.ToUpper().Split(delim);

            for (int i = 0; i < (maxSeason + 1); i++) //loop through season number
            {
                //Attempt to parse season info from filename
                if (Tokens[Tokens.Count() -1].Contains("S0" + i.ToString()) ||
                    Tokens[Tokens.Count() - 1].Contains("S" + i.ToString()) ||
                    Tokens[Tokens.Count() - 1].Contains("SEASON_" + i.ToString()) ||
                    Tokens[Tokens.Count() - 1].Contains("SEASON " + i.ToString()) ||
                    Tokens[Tokens.Count() - 1].Contains("SEASON 0" + i.ToString()) ||
                    Tokens[Tokens.Count() - 1].Contains("SEASON0" + i.ToString()) ||
                    Tokens[Tokens.Count() - 1].Contains("SEASON" + i.ToString()) ||
                    Tokens[Tokens.Count() - 1].Contains("SEASON." + i.ToString()))
                {
                    seasonNumber = i; //Loops through all Episodes check for season number, that way it will check for season 1 as well as 10 and not return only season 1.
                }
            }
            if (seasonNumber != -1) { return seasonNumber.ToString(); }*/

            //Check for season info in filename e.g. S0 + the loop number like 9, etc
            //Loop only goes as high as the number of seasons in the Series (Checked by thetvdb.com)
            for (int i = 0; i < (maxSeason + 1); i++) //loop through season number
            {
                //Attempt to parse season info from filename
                if (FileName.Contains("S0" + i.ToString()) ||
                    FileName.Contains("S" + i.ToString()) ||
                    FileName.Contains("S" + i.ToString() + "E") ||
                    FileName.Contains("S0" + i.ToString() + "E") ||
                    FileName.Contains(i.ToString() + "X") ||
                    //FileName.Contains(i.ToString() + ".E") ||
                    FileName.Contains("SEASON_" + i.ToString()) ||
                    FileName.Contains("SEASON " + i.ToString()) ||
                    FileName.Contains("SEASON 0" + i.ToString()) ||
                    FileName.Contains("SEASON0" + i.ToString()) ||
                    FileName.Contains("SEASON" + i.ToString()) ||
                    FileName.Contains("SEASON." + i.ToString()))
                {
                    seasonNumber = i; //Loops through all Episodes check for season number, that way it will check for season 1 as well as 10 and not return only season 1.
                }
            }
            if (seasonNumber != -1) { return seasonNumber.ToString(); }

            else if (seasonNumber == -1) //check for season numbers using 3 or 4 digits such as 101 = S01E01
            {
                characterBlocks = FileName.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (string block in characterBlocks)
                {
                    int DigitCounter = 0;
                    foreach (char C in block)
                    {
                        if (char.IsDigit(C)) { DigitCounter = DigitCounter + 1; }
                    }

                    if (DigitCounter >= 3 & DigitCounter <= 4)
                    {
                        switch (block.Length)
                        {
                            case 3:
                                return block.Remove(1, 2);
                            case 4:
                                return block.Remove(2, 2);
                            default:
                                break;
                        }
                    }
                    DigitCounter = 0;
                }
                Array.Clear(characterBlocks, 0, characterBlocks.Length);
                return "";
            }


            return "";

        }
        private string CheckEpisode(string FileName, int maxEpisode)
        {
            /*Remove items from string that might confuse the program.
             List is stored in string array lineStringFilter*/
            for (int i = 0; i < lineStringFilter.Length; i++)
            {
                if (FileName.Contains(lineStringFilter[i]))
                {
                    FileName = FileName.Replace(lineStringFilter[i], "");
                }
            }

            for (int i = 0; i < (maxEpisode + 1); i++) //loop through episode number
            {
                //attempt to determine the episode number from the filename
                //Plex Format - ShowName – sXXeYY – Optional_Info.ext
                //Synology Format - ShowName.SXX.EYY.ext
                /*Kodi Formats - ShowName S01E02.ext
                 ShowName S1E2.ext
                 ShowName S01.E02.ext
                 ShowName S01_E02.ext
                 ShowName S01xE02.ext
                 ShowName 1x02.ext
                 ShowName 102.ext*/
                /*Othe Formats - anything_s01e02.ext
                anything_s1e2.ext
                anything_s01.e02.ext
                anything_s01_e02.ext
                anything_1x02.ext
                anything_102.ext
                anything_1x02.ext
                02 Episode Name.avi
                s01e02.avi
                1x02.avi*/
                if (FileName.Contains("E0" + i.ToString() + " ") ||
                    FileName.Contains("E0" + i.ToString() + "_") ||
                    FileName.Contains("E0" + i.ToString() + ".") ||
                    FileName.Contains("E0" + i.ToString() + "-") ||
                    FileName.Contains("E0" + i.ToString() + "]") ||
                    FileName.Contains("E0" + i.ToString() + "}") ||
                    FileName.Contains("E0" + i.ToString() + ")") ||

                    FileName.Contains("E" + i.ToString() + " ") ||
                    FileName.Contains("E" + i.ToString() + "_") ||
                    FileName.Contains("E" + i.ToString() + "-") ||
                    FileName.Contains("E" + i.ToString() + ".") ||
                    FileName.Contains("E" + i.ToString() + "]") ||
                    FileName.Contains("E" + i.ToString() + "}") ||
                    FileName.Contains("E" + i.ToString() + ")") ||

                    FileName.Contains(".E" + i.ToString() + " ") ||
                    FileName.Contains(".E" + i.ToString() + "_") ||
                    FileName.Contains(".E" + i.ToString() + "-") ||
                    FileName.Contains(".E" + i.ToString() + ".") ||
                    FileName.Contains(".E" + i.ToString() + "]") ||
                    FileName.Contains(".E" + i.ToString() + "}") ||
                    FileName.Contains(".E" + i.ToString() + ")") ||

                    FileName.Contains("EP" + i.ToString() + " ") ||
                    FileName.Contains("EP" + i.ToString() + "_") ||
                    FileName.Contains("EP" + i.ToString() + "-") ||
                    FileName.Contains("EP" + i.ToString() + ".") ||
                    FileName.Contains("EP" + i.ToString() + "]") ||
                    FileName.Contains("EP" + i.ToString() + "}") ||
                    FileName.Contains("EP" + i.ToString() + ")") ||

                    FileName.Contains("X" + i.ToString() + " ") ||
                    FileName.Contains("X" + i.ToString() + "_") ||
                    FileName.Contains("X" + i.ToString() + "-") ||
                    FileName.Contains("X" + i.ToString() + ".") ||
                    FileName.Contains("X" + i.ToString() + "]") ||
                    FileName.Contains("X" + i.ToString() + "}") ||
                    FileName.Contains("X" + i.ToString() + ")") ||

                    FileName.Contains("X0" + i.ToString() + " ") ||
                    FileName.Contains("X0" + i.ToString() + "_") ||
                    FileName.Contains("X0" + i.ToString() + "-") ||
                    FileName.Contains("X0" + i.ToString() + ".") ||
                    FileName.Contains("X0" + i.ToString() + "]") ||
                    FileName.Contains("X0" + i.ToString() + "}") ||
                    FileName.Contains("X0" + i.ToString() + ")") ||

                    FileName.Contains("-" + i.ToString() + " ") ||
                    FileName.Contains("-" + i.ToString() + "_") ||
                    FileName.Contains("-" + i.ToString() + "-") ||
                    FileName.Contains("-" + i.ToString() + ".") ||
                    FileName.Contains("-" + i.ToString() + "]") ||
                    FileName.Contains("-" + i.ToString() + "}") ||
                    FileName.Contains("-" + i.ToString() + ")") ||

                    FileName.Contains("_" + i.ToString() + " ") ||
                    FileName.Contains("_" + i.ToString() + "_") ||
                    FileName.Contains("_" + i.ToString() + "-") ||
                    FileName.Contains("_" + i.ToString() + ".") ||
                    FileName.Contains("_" + i.ToString() + "]") ||
                    FileName.Contains("_" + i.ToString() + "}") ||
                    FileName.Contains("_" + i.ToString() + ")"))
                {
                    return i.ToString();
                }

            }

            characterBlocks = FileName.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string block in characterBlocks)
            {
                int DigitCounter = 0;
                foreach (char C in block)
                {
                    if (char.IsDigit(C))
                    {
                        DigitCounter = DigitCounter + 1;
                    }
                }

                if (DigitCounter == 3 || DigitCounter == 4)
                {
                    switch (block.Length)
                    {
                        case 3:
                            if (int.Parse(block.Remove(0, 1)) < 10)
                            { return int.Parse(block.Remove(0, 1)).ToString(); }
                            else { return block.Remove(0, 1); }

                        case 4:
                            int.Parse(block.Remove(0, 2));
                            return block.Remove(0, 2);
                        default:
                            break;
                    }
                }
                DigitCounter = 0;
            }
            Array.Clear(characterBlocks, 0, characterBlocks.Length);
            return "";
        }
        private string CheckAbsoluteNumber(string FileName)
        {
            characterBlocks = FileName.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            foreach (string block in characterBlocks)
            {
                bool blockOfDigits = true;
                foreach (char C in block)
                {
                    if (!char.IsDigit(C)) { blockOfDigits = false; }
                }
                if (blockOfDigits)
                {
                    if (int.Parse(block) <= SeriesInfo.EpisodeList.Count() + 1 && int.Parse(block) > 0)
                    {
                        string newTitle = "";

                        if (int.Parse(SeriesInfo.EpisodeList[int.Parse(block) - 1]["airedSeason"]) >= 10)
                        {
                            season = "S" + SeriesInfo.EpisodeList[int.Parse(block) - 1]["airedSeason"];
                        }
                        else { season = "S0" + SeriesInfo.EpisodeList[int.Parse(block) - 1]["airedSeason"]; }

                        if (int.Parse(SeriesInfo.EpisodeList[int.Parse(block) - 1]["airedEpisodeNumber"]) >= 10)
                        {
                            episode = "E" + SeriesInfo.EpisodeList[int.Parse(block) - 1]["airedEpisodeNumber"];
                        }
                        else { episode = "E0" + SeriesInfo.EpisodeList[int.Parse(block) - 1]["airedEpisodeNumber"]; }

                        //add episode and season info to the newTitle variable;

                        /*TitleFormats
                            Plex: ShowName - sXXeYY - Optional_info.ext
                                Grey's Anatomy - S01e02 - The First Cut is the Deepest.avi

                            Synology: ShowName.sXXeYY.ext
                                Grey's Anatomy.S01.E02.avi
                                Grey's Anatomy.S01.E02.The First Cut is the Deepest.avi

                            Kodi/XBMC: ShowName_sXXeYY.ext
                                Grey's Anatomy_S01E02.avi
                                Grey's Anatomy_S01E02_The First Cut is the Deepest.avi
                         */
                        switch (formatCombo.SelectedIndex)
                        {
                            
                            case 0: //PLEX
                                newTitle = SeriesInfo.series["seriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.EpisodeList[int.Parse(block) - 1]["episodeName"] + "." + ext;
                                break;
                            case 1: //KODI
                                newTitle = SeriesInfo.series["seriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.EpisodeList[int.Parse(block) - 1]["episodeName"] + "." + ext;
                                break;
                            case 2: //Synology
                                newTitle = SeriesInfo.series["seriesName"] + "." + season + "." + episode + "." + SeriesInfo.EpisodeList[int.Parse(block) - 1]["episodeName"] + "." + ext;
                                break;
                            default: //Synology
                                newTitle = SeriesInfo.series["seriesName"] + "." + season + "." + episode + "." + SeriesInfo.EpisodeList[int.Parse(block) - 1]["episodeName"] + "." + ext;
                                break;
                        }
                        return newTitle;
                    }
                }
            }
            Array.Clear(characterBlocks, 0, characterBlocks.Length);
            return "";
        }
        private void FileNamesListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(filenameListboxFlag) //items in box and something was clicked
            {
                changedFileNamesListboxFlag = false;

                if(fileNamesListbox.SelectedIndex != -1)
                {
                    if (fileNamesListbox.Items.Count > 0 && changedFileNamesListbox.Items.Count > 0)
                    {
                        //Match changedfilenamelistbox to match
                        changedFileNamesListbox.SelectedIndices.Clear();

                        for (int i = 0; i < fileNamesListbox.SelectedIndices.Count; i++)
                        {
                            changedFileNamesListbox.SelectedIndices.Add(fileNamesListbox.SelectedIndices[i]);
                        }

                        filenameListboxFlag = false;
                    }
                }
                else
                {
                    changedFileNamesListbox.SelectedIndices.Clear();
                }

                    
            }
            else if(fileNamesListbox.SelectedIndices.Count == 1 && fileNamesListbox.Focused)
            {
                changedFileNamesListbox.SelectedIndices.Clear();
                changedFileNamesListbox.SelectedIndex = fileNamesListbox.SelectedIndex;
            }

        }
        private void ChangedFileNamesListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (changedFileNamesListboxFlag) //items in the box and something was clicked
            {
                filenameListboxFlag = false;

                if(changedFileNamesListbox.SelectedIndex != -1)
                {
                    if (fileNamesListbox.Items.Count > 0 && changedFileNamesListbox.Items.Count > 0)
                    {
                        //Match changedfilenamelistbox to match
                        fileNamesListbox.SelectedIndices.Clear();

                        for (int i = 0; i < changedFileNamesListbox.SelectedIndices.Count; i++)
                        {
                            fileNamesListbox.SelectedIndices.Add(changedFileNamesListbox.SelectedIndices[i]);
                        }

                        changedFileNamesListboxFlag = false;
                    }
                }
                else
                {
                    fileNamesListbox.SelectedIndices.Clear();
                }
                

            }
            else if (changedFileNamesListbox.SelectedIndices.Count == 1 && changedFileNamesListbox.Focused)
            {
                fileNamesListbox.SelectedIndices.Clear();
                fileNamesListbox.SelectedIndex = changedFileNamesListbox.SelectedIndex;
            }
        }
        private string FormatFileName(string fileName)
        {
            // Replaces invalid characters /\:*?<>|
            if (fileName.Contains("/"))
            {
                fileName = fileName.Replace("/", "");
            }

            if (fileName.Contains("\\"))
            {
                fileName = fileName.Replace("\\", "");
            }

            if (fileName.Contains(":"))
            {
                fileName = fileName.Replace(":", "");
            }

            if (fileName.Contains("*"))
            {
                fileName = fileName.Replace("*", "");
            }

            if (fileName.Contains("?"))
            {
                fileName = fileName.Replace("?", "");
            }

            if (fileName.Contains("<"))
            {
                fileName = fileName.Replace("<", "");
            }

            if (fileName.Contains(">"))
            {
                fileName = fileName.Replace(">", "");
            }

            if (fileName.Contains("|"))
            {
                fileName = fileName.Replace("|", "");
            }

            return fileName;
        }
        private void ChangeFileNamesButton_Click(object sender, EventArgs e)
        {
            NLabelUpdate("Processing File Names", Color.GreenYellow);

            string errorList = "";
            if (!string.IsNullOrEmpty(parentPathLabel.Text))
            {
                //re-names files to the correct format base on thetvdb.com all seasons url
                string folderPath = parentPathLabel.Text; //current directory of the files
                string fileName = "";

                for (int i = 0; i < fileNamesListbox.Items.Count; i++)
                {
                    if(fileNamesListbox.SelectedIndices.Count > 0)
                    {
                        for (int a = 0; a < fileNamesListbox.SelectedIndices.Count; a++)
                        {
                            //If its found to exist in the selected indices
                            if (fileNamesListbox.SelectedIndices[a] == i)
                            {
                                //Get Extension
                                char[] delim = { '.' };
                                string[] Tokens = EpisodePathList[i].Split(delim);
                                ext = "." + Tokens[Tokens.Count() - 1]; //should be extension

                                //Get Parent Path
                                string s = EpisodePathList[i];
                                string EpisodeParentPath = "";
                                int c = s.LastIndexOf('\\');

                                if (c != -1)
                                {
                                    EpisodeParentPath = s.Substring(c + 1).Replace(ext, "");
                                    EpisodeParentPath = EpisodePathList[i].Replace(EpisodeParentPath, "").Replace(ext, "");
                                }

                                //If the names are the same, don't bother doing anything else.
                                if (EpisodePathList[i].ToString().Replace(EpisodeParentPath, "") != changedFileNamesListbox.Items[i].ToString())
                                {
                                    NLabelUpdate("Re-Naming File - " + fileNamesListbox.Items[i].ToString(), Color.GreenYellow);

                                    if (changedFileNamesListbox.Items[i].ToString().Contains("EPISODE COULD NOT BE DETERMINED") ||
                                        changedFileNamesListbox.Items[i].ToString().Contains("NO SUCH EPISODE FOUND"))
                                    {
                                        errorList += fileNamesListbox.Items[i].ToString() + " - " + changedFileNamesListbox.Items[i].ToString() + "\n";
                                    }
                                    /*Don't attempt to change the filename if a file with the new name already exists.
                                    This would be the case if there were multiple files with the same name in the chosen
                                        directory*/

                                    else if (changedFileNamesListbox.Items[i].ToString() != "")
                                    {
                                        /*if (!File.Exists(parentPathLabel.Text + changedFileNamesListbox.Items[i].ToString()))
                                        {
                                            //Using File.Move to change the fileNames.
                                            System.IO.File.Move(parentPathLabel.Text + fileNamesListbox.Items[i].ToString(), parentPathLabel.Text + changedFileNamesListbox.Items[i].ToString());
                                        }
                                        else if (File.Exists(parentPathLabel.Text + changedFileNamesListbox.Items[i].ToString()))
                                        {
                                            errorList += fileNamesListbox.Items[i].ToString() + " - Duplicate Episode\n";
                                        }*/

                                        if (!File.Exists(EpisodeParentPath + changedFileNamesListbox.Items[i].ToString()))
                                        {
                                            //Using File.Move to change the fileNames.
                                            System.IO.File.Move(EpisodePathList[i].ToString(), EpisodeParentPath + changedFileNamesListbox.Items[i].ToString());
                                        }
                                        else if (File.Exists(EpisodeParentPath + changedFileNamesListbox.Items[i].ToString()))
                                        {
                                            errorList += fileNamesListbox.Items[i].ToString() + " - Duplicate Episode\n";
                                        }
                                    }
                                }

                            }
                        }
                    }
                    else
                    {
                        //Get Extension
                        char[] delim = { '.' };
                        string[] Tokens = EpisodePathList[i].Split(delim);
                        ext = "." + Tokens[Tokens.Count() - 1]; //should be extension

                        //Get Parent Path
                        string s = EpisodePathList[i];
                        string EpisodeParentPath = "";
                        int c = s.LastIndexOf('\\');

                        if (c != -1)
                        {
                            EpisodeParentPath = s.Substring(c + 1).Replace(ext, "");
                            EpisodeParentPath = EpisodePathList[i].Replace(EpisodeParentPath, "").Replace(ext, "");
                        }

                        //If the names are the same, don't bother doing anything else.
                        if (EpisodePathList[i].ToString().Replace(EpisodeParentPath, "") != changedFileNamesListbox.Items[i].ToString())
                        {
                            NLabelUpdate("Re-Naming File - " + fileNamesListbox.Items[i].ToString(), Color.GreenYellow);

                            if (changedFileNamesListbox.Items[i].ToString().Contains("EPISODE COULD NOT BE DETERMINED") ||
                                changedFileNamesListbox.Items[i].ToString().Contains("NO SUCH EPISODE FOUND"))
                            {
                                errorList += fileNamesListbox.Items[i].ToString() + " - " + changedFileNamesListbox.Items[i].ToString() + "\n";
                            }
                            /*Don't attempt to change the filename if a file with the new name already exists.
                            This would be the case if there were multiple files with the same name in the chosen
                                directory*/

                            else if (changedFileNamesListbox.Items[i].ToString() != "")
                            {
                                /*if (!File.Exists(parentPathLabel.Text + changedFileNamesListbox.Items[i].ToString()))
                                {
                                    //Using File.Move to change the fileNames.
                                    System.IO.File.Move(parentPathLabel.Text + fileNamesListbox.Items[i].ToString(), parentPathLabel.Text + changedFileNamesListbox.Items[i].ToString());
                                }
                                else if (File.Exists(parentPathLabel.Text + changedFileNamesListbox.Items[i].ToString()))
                                {
                                    errorList += fileNamesListbox.Items[i].ToString() + " - Duplicate Episode\n";
                                }*/

                                if (!File.Exists(EpisodeParentPath + changedFileNamesListbox.Items[i].ToString()))
                                {
                                    //Using File.Move to change the fileNames.
                                    System.IO.File.Move(EpisodePathList[i].ToString(), EpisodeParentPath + changedFileNamesListbox.Items[i].ToString());
                                }
                                else if (File.Exists(EpisodeParentPath + changedFileNamesListbox.Items[i].ToString()))
                                {
                                    errorList += fileNamesListbox.Items[i].ToString() + " - Duplicate Episode\n";
                                }
                            }
                        }
                    }
                    
                }

                if (!string.IsNullOrEmpty(errorList))
                {
                    CustomMessageBox.Show("The following files had errors: \n" + errorList, 470, 530);
                }

                fileNamesListbox.Items.Clear(); //clears the fileNamesListbox so it can be refreshed with current data


                NLabelUpdate("Re-Querying Files ", Color.GreenYellow);

                /*pulls in the video files located in the folderPath directory
                 These files are the newly renamed files*/

                EpisodePathList.Clear();

                if (recursiveCB.Checked)
                {
                    string[] fileNames = Directory
                    .GetFiles(cf.DefaultSettings["TFPath"], "*.*", SearchOption.AllDirectories)
                    .Where(file => file.ToLower().EndsWith(".mpg")
                    || file.ToLower().EndsWith(".mpeg")
                    || file.ToLower().EndsWith(".vob")
                    || file.ToLower().EndsWith(".mod")
                    || file.ToLower().EndsWith(".ts")
                    || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4")
                    || file.ToLower().EndsWith(".m4v")
                    || file.ToLower().EndsWith(".mov")
                    || file.ToLower().EndsWith("avi")
                    || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv")
                    || file.ToLower().EndsWith(".asf")
                    || file.ToLower().EndsWith(".mkv")
                    || file.ToLower().EndsWith(".flv")
                    || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr")
                    || file.ToLower().EndsWith(".dvr-ms")
                    || file.ToLower().EndsWith(".wtv")
                    || file.ToLower().EndsWith(".ogv")
                    || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp")
                    || file.ToLower().EndsWith(".rm")
                    || file.ToLower().EndsWith(".rmvb")
                    || file.ToLower().EndsWith(".srt")  //Add Subtitle File Extensions also
                    || file.ToLower().EndsWith(".sub")
                    || file.ToLower().EndsWith(".idx")
                    || file.ToLower().EndsWith(".ssa")
                    || file.ToLower().EndsWith(".ass")
                    || file.ToLower().EndsWith(".smi")
                    || file.ToLower().EndsWith(".vtt")).ToArray();

                    foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                    {
                        EpisodePathList.Add(file);

                        fileName = file.Replace(cf.DefaultSettings["TFPath"] + "\\", "");

                        if (!fileName.StartsWith("._"))
                        {
                            fileNamesListbox.Items.Add(fileName);
                        }
                    }
                }
                else
                {
                    string[] fileNames = Directory
                    .GetFiles(cf.DefaultSettings["TFPath"], "*.*")
                    .Where(file => file.ToLower().EndsWith(".mpg")
                    || file.ToLower().EndsWith(".mpeg")
                    || file.ToLower().EndsWith(".vob")
                    || file.ToLower().EndsWith(".mod")
                    || file.ToLower().EndsWith(".ts")
                    || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4")
                    || file.ToLower().EndsWith(".m4v")
                    || file.ToLower().EndsWith(".mov")
                    || file.ToLower().EndsWith("avi")
                    || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv")
                    || file.ToLower().EndsWith(".asf")
                    || file.ToLower().EndsWith(".mkv")
                    || file.ToLower().EndsWith(".flv")
                    || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr")
                    || file.ToLower().EndsWith(".dvr-ms")
                    || file.ToLower().EndsWith(".wtv")
                    || file.ToLower().EndsWith(".ogv")
                    || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp")
                    || file.ToLower().EndsWith(".rm")
                    || file.ToLower().EndsWith(".rmvb")
                    || file.ToLower().EndsWith(".srt")  //Add Subtitle File Extensions also
                    || file.ToLower().EndsWith(".sub")
                    || file.ToLower().EndsWith(".idx")
                    || file.ToLower().EndsWith(".ssa")
                    || file.ToLower().EndsWith(".ass")
                    || file.ToLower().EndsWith(".smi")
                    || file.ToLower().EndsWith(".vtt")).ToArray();

                    foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                    {
                        EpisodePathList.Add(file);
                        fileName = file.Replace(cf.DefaultSettings["TFPath"] + "\\", "");

                        if (!fileName.StartsWith("._"))
                        {
                            fileNamesListbox.Items.Add(fileName);
                        }
                    }
                }

                changedFileNamesListbox.Items.Clear();
            }
            else
            {
                CustomMessageBox.Show("No directory path", 115, 195);
            }
            notificationLabel.Visible = false;
        }
        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();
            DialogResult = DialogResult.None; //prevent form from closing
        }
        private void ClearAll()
        {
            SeriesIDTitleTextbox.Clear();
            favoritesCombo.Text = "";
            SeriesImagePicturebox.ImageLocation = "";
            parentPathLabel.Text = "";
            fileNamesListbox.Items.Clear();
            changedFileNamesListbox.Items.Clear();

        }
        private void FileNamesListbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int value = 0;

            //Keeps both listboxex in sync by deleting items from both boxes at once
            if (fileNamesListbox.Items.Count > 0 & fileNamesListbox.SelectedIndex >= 0 & changedFileNamesListbox.Items.Count > 0)
            {
                value = fileNamesListbox.SelectedIndex;
                fileNamesListbox.Items.RemoveAt(value);
                changedFileNamesListbox.Items.RemoveAt(value);
                EpisodePathList.RemoveAt(value);

            }

            //Allows you to delete items from fileNamesListbox if changedFileNamesListbox isn't populated
            if (fileNamesListbox.Items.Count > 0 & fileNamesListbox.SelectedIndex >= 0 & changedFileNamesListbox.Items.Count == 0)
            {
                value = fileNamesListbox.SelectedIndex;
                fileNamesListbox.Items.RemoveAt(value);
            }

        }
        private void ChangedFileNamesListbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListOfEpisodeNames.Clear();
            if (changedFileNamesListbox.Items.Count > 0)
            {
                int index = changedFileNamesListbox.SelectedIndex;

                switch(formatCombo.SelectedIndex)
                {
                    case 0: //PLEX
                        for (int i = 0; i < PLEXEpisodeNames.Count; i++)
                        {
                            ListOfEpisodeNames.Add(PLEXEpisodeNames[i]);
                        }
                        break;
                    case 1: //KODI
                        for (int i = 0; i < KODIEpisodeNames.Count; i++)
                        {
                            ListOfEpisodeNames.Add(KODIEpisodeNames[i]);
                        }
                        break;
                    case 2: //Synology
                        for (int i = 0; i < SynologyEpisodeName.Count; i++)
                        {
                            ListOfEpisodeNames.Add(SynologyEpisodeName[i]);
                        }
                        break;
                    default:
                        for (int i = 0; i < SynologyEpisodeName.Count; i++)
                        {
                            ListOfEpisodeNames.Add(SynologyEpisodeName[i]);
                        }
                        break;
                }


                TVManualRename R = new TVManualRename(fileNamesListbox.SelectedItem.ToString(), changedFileNamesListbox.SelectedItem.ToString(), ListOfEpisodeNames);
                R.ShowDialog();

                if (R.DialogResult == DialogResult.OK)
                {
                    changedFileNamesListbox.Items.RemoveAt(index);
                    changedFileNamesListbox.Items.Insert(index, R.ChangedFileName);
                }
                R.Dispose();
            }
        }
        private void TVForm_Load(object sender, EventArgs e)
        {
            cf.CheckConfigFile();

            //Add items to Favorites Combo
            for (int i = 0; i < cf.FavoriteTitles.Count; i++)
            {
                favoritesCombo.Items.Add(cf.FavoriteTitles[i]);
            }

            
            switch (cf.DefaultSettings["DefaultFormat"])
            {
                case "PLEX":
                    formatCombo.SelectedIndex = 0;
                    break;
                case "KODI":
                    formatCombo.SelectedIndex = 1;
                    break;
                case "Synology":
                    formatCombo.SelectedIndex = 2;
                    break;
                default: //Default to Synology
                    formatCombo.SelectedIndex = 2;
                    break;
            }
        }
        private void AddFavoriteButton_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(favoritesCombo.Text) & !string.IsNullOrEmpty(SeriesIDTitleTextbox.Text))
            {

                cf.AddFavorite(favoritesCombo.Text, SeriesIDTitleTextbox.Text);

                SeriesIDTitleTextbox.Text = "";
                favoritesCombo.Text = "";
                favoritesCombo.Items.Clear();

                for (int i = 0; i < cf.FavoriteTitles.Count; i++)
                {
                    favoritesCombo.Items.Add(cf.FavoriteTitles[i]);
                }
                

            }
            else
            {
                CustomMessageBox.Show("Saving a favorite requires both a URL and a name in the favorites box", 136, 367);
            }

            DialogResult = DialogResult.None; //Prevents form from closing...
        }
        private void FavoritesCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {

            SeriesIDTitleTextbox.Text = cf.FavoriteIDs[favoritesCombo.SelectedIndex]; //Matches the favorite name with the correct URL

            if (!string.IsNullOrEmpty(favoritesCombo.SelectedItem.ToString()))
            {
                GetHTML(); //Prevents you from having to click the GetHTML button since that will be your next move anyway
            }

        }
        private void DeleteFavoriteButton_Click(object sender, EventArgs e)
        {
            cf.RemoveFavorite(favoritesCombo.SelectedIndex);
            favoritesCombo.Text = "";
            SeriesIDTitleTextbox.Text = "";
            SeriesImagePicturebox.ImageLocation = "";

            favoritesCombo.Items.Clear();
            for (int i = 0; i < cf.FavoriteTitles.Count; i++)
            {
                favoritesCombo.Items.Add(cf.FavoriteTitles[i]);
            }
      
        }
        private void InvisibleCloseButton_Click(object sender, EventArgs e)
        {
            this.Close(); //Located behind the Make Changes button
        }
        private void FormatCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(fileNamesListbox.Items.Count > 0) //ensures that this doesnt fire when there is nothing for it to do but error
            {
                PreviewChanges(); //updates titles for seletion

                if (changedFileNamesListbox.Items.Count > 0 & fileNamesListbox.Items.Count > 0)
                {
                    if (fileNamesListbox.SelectedIndex > -1)
                    {
                        changedFileNamesListbox.SelectedIndex = fileNamesListbox.SelectedIndex;
                    }
                }
            }
            cf.DefaultSettings["DefaultFormat"] = formatCombo.SelectedItem.ToString();
            cf.UpdateDefaults();
        }
        private void TheTVDBcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("https://www.thetvdb.com");
        }
        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get File Names
            GetFileNames();
        }

        private string AutoDetermineEpisodeFromFileName(string fileNameToCheck)
        {
            season = "";
            episode = "";
            char[] delim = { '.' };
            List<string> episodeNames = new List<string>();
            string newTitle = "";

            int maxSeason = 0; //Used to restrict season loops
            int maxEpisode = 0; //used to restrict episode loops



            //look for highest season number, highest episode number and build list of all Episodes of the show.
            for (int i = 0; i < SeriesInfo.EpisodeList.Count(); i++)
            {
                if (SeriesInfo.EpisodeList[i].ContainsKey("airedSeason") && int.Parse(SeriesInfo.EpisodeList[i]["airedSeason"]) > maxSeason)
                {
                    maxSeason = int.Parse(SeriesInfo.EpisodeList[i]["airedSeason"]);
                }

                //Look for largest episode number in all seasons of the show.
                if (SeriesInfo.EpisodeList[i].ContainsKey("airedEpisodeNumber") && int.Parse(SeriesInfo.EpisodeList[i]["airedEpisodeNumber"]) > maxEpisode)
                {
                    maxEpisode = int.Parse(SeriesInfo.EpisodeList[i]["airedEpisodeNumber"]);
                }

                if (SeriesInfo.EpisodeList[i].ContainsKey("episodeName"))
                {
                    episodeNames.Add(SeriesInfo.EpisodeList[i]["episodeName"]);
                }
            }

            //look for highest season number, highest episode nmber and build list of all EpisodeList of the show.
            for (int i = 0; i < SeriesInfo.EpisodeList.Count(); i++)
            {
                if (SeriesInfo.EpisodeList[i].ContainsKey("airedSeason") && int.Parse(SeriesInfo.EpisodeList[i]["airedSeason"]) > maxSeason)
                {
                    maxSeason = int.Parse(SeriesInfo.EpisodeList[i]["airedSeason"]);
                }

                //Look for largest episode number in all seasons of the show.
                if (SeriesInfo.EpisodeList[i].ContainsKey("airedEpisodeNumber") && int.Parse(SeriesInfo.EpisodeList[i]["airedEpisodeNumber"]) > maxEpisode)
                {
                    maxEpisode = int.Parse(SeriesInfo.EpisodeList[i]["airedEpisodeNumber"]);
                }

                if (SeriesInfo.EpisodeList[i].ContainsKey("episodeName"))
                {
                    episodeNames.Add(SeriesInfo.EpisodeList[i]["episodeName"]);
                }
            }

            //Check for selected items, if there are items selected then for the non selected items set the season and episode numbers as -1 to effectively disable them

            string[] Tokens = fileNameToCheck.Split(delim);
            ext = Tokens[Tokens.Count() - 1]; //should be extension
            season = "";
            episode = "";

            //Check for episode title in filename first
            //Add loop for each episode to see if an episode title in the SeriesInfo object matches a filename. If so return the episode and season from that.
            for (int a = 0; a < SeriesInfo.EpisodeList.Count; a++)
            {
                if (SeriesInfo.EpisodeList[a].ContainsKey("episodeName"))
                {
                    if ((cleanString(fileNameToCheck).ToUpper()).Contains(" " + cleanString(SeriesInfo.EpisodeList[a]["episodeName"]).ToUpper() + " "))
                    {

                        if (int.Parse(SeriesInfo.EpisodeList[a]["airedSeason"]) < 10)
                        {
                            season = "S0" + SeriesInfo.EpisodeList[a]["airedSeason"];
                        }
                        else
                        {
                            season = "S" + SeriesInfo.EpisodeList[a]["airedSeason"];
                        }

                        if (int.Parse(SeriesInfo.EpisodeList[a]["airedEpisodeNumber"]) < 10)
                        {
                            episode = "E0" + SeriesInfo.EpisodeList[a]["airedEpisodeNumber"];
                        }
                        else
                        {
                            episode = "E" + SeriesInfo.EpisodeList[a]["airedEpisodeNumber"];
                        }

                        //add episode and season info to the newTitle variable;

                        /*TitleFormats
                            Plex: ShowName - sXXeYY - Optional_info.ext
                                Grey's Anatomy - S01e02 - The First Cut is the Deepest.avi

                            Synology: ShowName.sXXeYY.ext
                                Grey's Anatomy.S01.E02.avi
                                Grey's Anatomy.S01.E02.The First Cut is the Deepest.avi

                            Kodi/XBMC: ShowName_sXXeYY.ext
                                Grey's Anatomy_S01E02.avi
                                Grey's Anatomy_S01E02_The First Cut is the Deepest.avi
                            */
                        switch (formatCombo.SelectedIndex)
                        {

                            case 0: //PLEX
                                newTitle = SeriesInfo.series["seriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                break;
                            case 1: //KODI
                                newTitle = SeriesInfo.series["seriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                break;
                            case 2: //Synology
                                newTitle = SeriesInfo.series["seriesName"] + "." + season + "." + episode + "." + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                break;
                            default: //Plex
                                newTitle = SeriesInfo.series["seriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.EpisodeList[a]["episodeName"] + "." + ext;
                                break;
                        }
                    }
                }
            }

            if ((string.IsNullOrEmpty(season) | season == "-1" | string.IsNullOrEmpty(episode) | episode == "-1"))
            {

                season = CheckSeason(fileNameToCheck.ToUpper(), maxSeason); //tries to parse season info from filename
                episode = CheckEpisode(fileNameToCheck.ToUpper(), maxEpisode); //tries to parse episode info from filename

            }

            //Selections made, other items are marked with -1 and then skipped
            if (season == "-1") { newTitle = "SKIPPED"; }

            if ((string.IsNullOrEmpty(season) | string.IsNullOrEmpty(episode)))
            {
                // Add filter for absolute episode numbers here
                newTitle = CheckAbsoluteNumber(fileNameToCheck);
                if (string.IsNullOrEmpty(newTitle)) { newTitle = "EPISODE COULD NOT BE DETERMINED"; }
            }

            if (string.IsNullOrEmpty(newTitle))//Check for matching episode and season entry
            {
                for (int e = 0; e < SeriesInfo.EpisodeList.Count(); e++)
                {
                    if (SeriesInfo.EpisodeList[e].ContainsKey("airedEpisodeNumber")
                        & SeriesInfo.EpisodeList[e]["airedEpisodeNumber"] == episode)
                    {
                        if (SeriesInfo.EpisodeList[e].ContainsKey("airedSeason")
                            & SeriesInfo.EpisodeList[e]["airedSeason"] == season)
                        {
                            if (int.Parse(season) < 10) { season = "S0" + season; }
                            else { season = "S" + season; }
                            if (int.Parse(episode) < 10) { episode = "E0" + episode; }
                            else { episode = "E" + episode; }

                            //add episode and season info to the newTitle variable;

                            /*TitleFormats
                                Plex: ShowName - sXXeYY - Optional_info.ext
                                    Grey's Anatomy - S01e02 - The First Cut is the Deepest.avi

                                Synology: ShowName.sXXeYY.ext
                                    Grey's Anatomy.S01.E02.avi
                                    Grey's Anatomy.S01.E02.The First Cut is the Deepest.avi

                                Kodi/XBMC: ShowName_sXXeYY.ext
                                    Grey's Anatomy_S01E02.avi
                                    Grey's Anatomy_S01E02_The First Cut is the Deepest.avi
                                */
                            switch (formatCombo.SelectedIndex)
                            {

                                case 0: //PLEX
                                    newTitle = SeriesInfo.series["seriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.EpisodeList[e]["episodeName"] + "." + ext;
                                    break;
                                case 1: //KODI
                                    newTitle = SeriesInfo.series["seriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.EpisodeList[e]["episodeName"] + "." + ext;
                                    break;
                                case 2: //Synology
                                    newTitle = SeriesInfo.series["seriesName"] + "." + season + "." + episode + "." + SeriesInfo.EpisodeList[e]["episodeName"] + "." + ext;
                                    break;
                                default: //Synology
                                    newTitle = SeriesInfo.series["seriesName"] + "." + season + "." + episode + "." + SeriesInfo.EpisodeList[e]["episodeName"] + "." + ext;
                                    break;
                            }
                            //newTitle = SeriesInfo.series["seriesName"] + " " + season + episode + " " + SeriesInfo.EpisodeList[e]["episodeName"] + "." + ext;
                        }
                    }
                }
                if (string.IsNullOrEmpty(newTitle) & !string.IsNullOrEmpty(season) & !string.IsNullOrEmpty(episode)) { newTitle = "NO SUCH EPISODE FOUND"; }
                else if (string.IsNullOrEmpty(newTitle)) { newTitle = "EPISODE COULD NOT BE DETERMINED"; }
                else if (newTitle == "SKIPPED") { newTitle = ""; MessageBox.Show("title change"); } //items marked as SKIPPED are items that were not selected and thus not evaluated.
            }
            if (newTitle == "SKIPPED") { newTitle = ""; }
            else
            {
                newTitle = FormatFileName(newTitle); //removes invalid characters from the filename.
            }

            episodeNames.Clear();
            return (newTitle);
        }
        private void AutoBtn_Click(object sender, EventArgs e)
        {
            List<String> Titles = new List<string>();
            bool favoriteMatchFound = false;
            string fileNameString = "";
            string testString = "";
            string title = "";

            //Loop through each file in list and attempt to auto detect the episode based on previously added favorites.

            //Make sure there are files populated in the list else error out.
            if((fileNamesListbox.Items.Count > 0) & (cf.FavoriteTitles.Count > 0)) //Check that file list is populated, and the Favorites list is populated
            {
                NLabelUpdate("Matching files to favorites list.", Color.GreenYellow);

                changedFileNamesListbox.Items.Clear(); //clear out the preview listbox.
                //Start Loop
                if(fileNamesListbox.SelectedIndices.Count > 0)
                {
                    // Run for all files
                    for (int i = 0; i < fileNamesListbox.Items.Count; i++) //Loop through filenamesListBox
                    {
                        Titles.Insert(i, ""); //increases list size as fileNameString increases

                        bool isSelected = false;
                        for (int a = 0; a < fileNamesListbox.SelectedIndices.Count; a++)
                        {
                            //If its found to exist in the selected indices
                            if (fileNamesListbox.SelectedIndices[a] == i)
                            {
                                isSelected = true;
                            }
                        }
                        if (isSelected)
                        {
                            fileNameString = fileNamesListbox.Items[i].ToString(); //Store filename from listbox, make uppercase to eliminate possibilities
                            

                            for (int b = 0; b < cf.FavoriteTitles.Count; b++) //Loop through each name in listbox to see if it matches the filename in the listbox.
                            {
                                testString = cf.FavoriteTitles[b]; //make uppercase to eliminate possibilities

                                if (fileNameString.ToUpper().Contains(testString.ToUpper())) //Exact Match
                                {
                                    NLabelUpdate("Match found, " + cf.FavoriteTitles[b].ToString() + ".", Color.GreenYellow);

                                    favoriteMatchFound = true;
                                }

                                if (!favoriteMatchFound) //Remove spaces and apostrophes
                                {
                                    testString = FormatFileName(cf.FavoriteTitles[b]);
                                    testString = testString.Replace(" ", ".");
                                    testString = testString.Replace("'", "");

                                    if (fileNameString.ToUpper().Contains(testString.ToUpper()))
                                    {
                                        NLabelUpdate("Match found, " + cf.FavoriteTitles[b].ToString() + ".", Color.GreenYellow);

                                        favoriteMatchFound = true;
                                    }

                                    testString = testString.Replace("&", "and");

                                    if (fileNameString.ToUpper().Contains(testString.ToUpper()))
                                    {
                                        NLabelUpdate("Match found, " + cf.FavoriteTitles[b].ToString() + ".", Color.GreenYellow);

                                        favoriteMatchFound = true;
                                    }

                                    testString = testString.Replace("AND", "&");

                                    if (fileNameString.ToUpper().Contains(testString.ToUpper()))
                                    {
                                        NLabelUpdate("Match found, " + cf.FavoriteTitles[b].ToString() + ".", Color.GreenYellow);

                                        favoriteMatchFound = true;
                                    }

                                }

                                if (!favoriteMatchFound) //Change and to & and vice versa
                                {
                                    testString = FormatFileName(cf.FavoriteTitles[b]);
                                    testString = testString.Replace(" ", ".");
                                    testString = testString.Replace("'", "");

                                    if (fileNameString.ToUpper().Contains(testString.ToUpper()))
                                    {
                                        NLabelUpdate("Match found, " + cf.FavoriteTitles[b].ToString() + ".", Color.GreenYellow);

                                        favoriteMatchFound = true;
                                    }
                                }

                                if (favoriteMatchFound) //Match found, proceed looking up info and populating form with suggested name
                                {
                                    NLabelUpdate("Match found! Identifyting episode.", Color.GreenYellow);

                                    TVSeriesInfo SI = new TVSeriesInfo(Authorization_Token, cf.FavoriteIDs[b].ToString());
                                    SeriesInfo = SI; //Makes the seriesinfo global
                                    title = AutoDetermineEpisodeFromFileName(fileNameString);
                                    //Scrub incompatible characters from file name
                                    title = FormatFileName(title);

                                    NLabelUpdate("Match found, " + title, Color.GreenYellow);

                                    if (!string.IsNullOrEmpty(title))
                                    {
                                        NLabelUpdate("Match Found! " + title, Color.GreenYellow);

                                        Titles.Insert(i, title); //insert the title in the appropriate spot in the list
                                    }
                                }

                                favoriteMatchFound = false;
                            }
                        }

                    }
                }
                else
                {
                    // Run for all files
                    for (int i = 0; i < fileNamesListbox.Items.Count; i++) //Loop through filenamesListBox
                    {
                        fileNameString = fileNamesListbox.Items[i].ToString(); //Store filename from listbox, make uppercase to eliminate possibilities
                        Titles.Insert(i, ""); //increases list size as fileNameString increases

                        for (int b = 0; b < cf.FavoriteTitles.Count; b++) //Loop through each name in listbox to see if it matches the filename in the listbox.
                        {
                            testString = cf.FavoriteTitles[b]; //make uppercase to eliminate possibilities

                            if (fileNameString.ToUpper().Contains(testString.ToUpper())) //Exact Match
                            {
                                NLabelUpdate("Match found, " + cf.FavoriteTitles[b].ToString() + ".", Color.GreenYellow);

                                favoriteMatchFound = true;
                            }

                            if (!favoriteMatchFound) //Remove spaces and apostrophes
                            {
                                testString = FormatFileName(cf.FavoriteTitles[b]);
                                testString = testString.Replace(" ", ".");
                                testString = testString.Replace("'", "");

                                if (fileNameString.ToUpper().Contains(testString.ToUpper()))
                                {
                                    NLabelUpdate("Match found, " + cf.FavoriteTitles[b].ToString() + ".", Color.GreenYellow);

                                    favoriteMatchFound = true;
                                }

                                testString = testString.Replace("&", "and");

                                if (fileNameString.ToUpper().Contains(testString.ToUpper()))
                                {
                                    NLabelUpdate("Match found, " + cf.FavoriteTitles[b].ToString() + ".", Color.GreenYellow);

                                    favoriteMatchFound = true;
                                }

                                testString = testString.Replace("AND", "&");

                                if (fileNameString.ToUpper().Contains(testString.ToUpper()))
                                {
                                    NLabelUpdate("Match found, " + cf.FavoriteTitles[b].ToString() + ".", Color.GreenYellow);

                                    favoriteMatchFound = true;
                                }

                            }

                            if (!favoriteMatchFound) //Change and to & and vice versa
                            {
                                testString = FormatFileName(cf.FavoriteTitles[b]);
                                testString = testString.Replace(" ", ".");
                                testString = testString.Replace("'", "");

                                if (fileNameString.ToUpper().Contains(testString.ToUpper()))
                                {
                                    NLabelUpdate("Match found, " + cf.FavoriteTitles[b].ToString() + ".", Color.GreenYellow);

                                    favoriteMatchFound = true;
                                }
                            }

                            if (favoriteMatchFound) //Match found, proceed looking up info and populating form with suggested name
                            {
                                NLabelUpdate("Match found! Identifyting episode.", Color.GreenYellow);

                                TVSeriesInfo SI = new TVSeriesInfo(Authorization_Token, cf.FavoriteIDs[b].ToString());
                                SeriesInfo = SI; //Makes the seriesinfo global
                                title = AutoDetermineEpisodeFromFileName(fileNameString);
                                //Scrub incompatible characters from file name
                                title = FormatFileName(title);

                                NLabelUpdate("Match found, " + title, Color.GreenYellow);

                                if (!string.IsNullOrEmpty(title))
                                {
                                    NLabelUpdate("Match Found!" + title, Color.GreenYellow);

                                    Titles.Insert(i, title); //insert the title in the appropriate spot in the list
                                }
                            }

                            favoriteMatchFound = false;
                        }

                    }
                }

                for (int i = 0; i < fileNamesListbox.Items.Count; i++)
                {
                    changedFileNamesListbox.Items.Add(Titles[i].ToString());
                }

                title = "";
                Titles.Clear();
                notificationLabel.Visible = false;

            }
            else if (fileNamesListbox.Items.Count > 0) { NLabelUpdate("Auto Feature dependent on \"Favorites List\". No \"Favorites\" saved to check against.",Color.Red); }
            else if(cf.FavoriteTitles.Count > 0) { NLabelUpdate("No files in list",Color.Red); }
        }

        private void recursiveCB_CheckedChanged(object sender, EventArgs e)
        {
            string fileName = "";

            if(fileNamesListbox.Items.Count > 0)
            {
                fileNamesListbox.Items.Clear();
                EpisodePathList.Clear();
            }

            if (recursiveCB.Checked)
            {
                if (!string.IsNullOrEmpty(parentPathLabel.Text))
                {
                    //Perform Search
                    string[] fileNames = Directory
                    .GetFiles(cf.DefaultSettings["TFPath"], "*.*", SearchOption.AllDirectories)
                    .Where(file => file.ToLower().EndsWith(".mpg")
                    || file.ToLower().EndsWith(".mpeg")
                    || file.ToLower().EndsWith(".vob")
                    || file.ToLower().EndsWith(".mod")
                    || file.ToLower().EndsWith(".ts")
                    || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4")
                    || file.ToLower().EndsWith(".m4v")
                    || file.ToLower().EndsWith(".mov")
                    || file.ToLower().EndsWith("avi")
                    || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv")
                    || file.ToLower().EndsWith(".asf")
                    || file.ToLower().EndsWith(".mkv")
                    || file.ToLower().EndsWith(".flv")
                    || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr")
                    || file.ToLower().EndsWith(".dvr-ms")
                    || file.ToLower().EndsWith(".wtv")
                    || file.ToLower().EndsWith(".ogv")
                    || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp")
                    || file.ToLower().EndsWith(".rm")
                    || file.ToLower().EndsWith(".rmvb")
                    || file.ToLower().EndsWith(".srt")  //Add Subtitle File Extensions also
                    || file.ToLower().EndsWith(".sub")
                    || file.ToLower().EndsWith(".idx")
                    || file.ToLower().EndsWith(".ssa")
                    || file.ToLower().EndsWith(".ass")
                    || file.ToLower().EndsWith(".smi")
                    || file.ToLower().EndsWith(".vtt")).ToArray();

                    foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                    {
                        EpisodePathList.Add(file);
                        fileName = file.Replace(cf.DefaultSettings["TFPath"] + "\\", "");

                        if (!fileName.StartsWith("._"))
                        {
                            fileNamesListbox.Items.Add(fileName);
                        }
                    }
                }
                recursiveCB.BackColor = Color.FromName("YellowGreen");
            }
            else
            {
                if (!string.IsNullOrEmpty(parentPathLabel.Text))
                {
                    //Perform Search
                    string[] fileNames = Directory
                    .GetFiles(cf.DefaultSettings["TFPath"], "*.*")
                    .Where(file => file.ToLower().EndsWith(".mpg")
                    || file.ToLower().EndsWith(".mpeg")
                    || file.ToLower().EndsWith(".vob")
                    || file.ToLower().EndsWith(".mod")
                    || file.ToLower().EndsWith(".ts")
                    || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4")
                    || file.ToLower().EndsWith(".m4v")
                    || file.ToLower().EndsWith(".mov")
                    || file.ToLower().EndsWith("avi")
                    || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv")
                    || file.ToLower().EndsWith(".asf")
                    || file.ToLower().EndsWith(".mkv")
                    || file.ToLower().EndsWith(".flv")
                    || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr")
                    || file.ToLower().EndsWith(".dvr-ms")
                    || file.ToLower().EndsWith(".wtv")
                    || file.ToLower().EndsWith(".ogv")
                    || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp")
                    || file.ToLower().EndsWith(".rm")
                    || file.ToLower().EndsWith(".rmvb")
                    || file.ToLower().EndsWith(".srt")  //Add Subtitle File Extensions also
                    || file.ToLower().EndsWith(".sub")
                    || file.ToLower().EndsWith(".idx")
                    || file.ToLower().EndsWith(".ssa")
                    || file.ToLower().EndsWith(".ass")
                    || file.ToLower().EndsWith(".smi")
                    || file.ToLower().EndsWith(".vtt")).ToArray();

                    foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                    {
                        EpisodePathList.Add(file);
                        fileName = file.Replace(cf.DefaultSettings["TFPath"] + "\\", "");

                        if (!fileName.StartsWith("._"))
                        {
                            fileNamesListbox.Items.Add(fileName);
                        }
                    }
                }

                recursiveCB.BackColor = Color.FromName("GreenYellow");
            }
            
        }

        private void changedFileNamesListbox_MouseClick(object sender, MouseEventArgs e)
        {
            //Sets flag if listbox contains items and was clicked
            //Used to control what items are selected
            if (changedFileNamesListbox.Items.Count > 0)
            {
                changedFileNamesListboxFlag = true;
            }
            
        }

        private void fileNamesListbox_MouseClick(object sender, MouseEventArgs e)
        {
            //Sets flag if listbox contains items and was clicked
            //Used to control what items are selected
            if (fileNamesListbox.Items.Count > 0)
            {
                filenameListboxFlag = true;
            }
            filenameListboxLastIndex = fileNamesListbox.SelectedIndex;
        }
    }
}
