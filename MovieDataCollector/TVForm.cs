using System;
using System.Collections.Generic;
using System.Data;
using System.IO; //allows for file manipulation
using System.Net; //Allows for WebClient usage
using System.Diagnostics; //Allows for using Process.Start codes lines
using System.Linq;
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

        string configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector"; //Direcory to store configuration files on host
        string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Config.txt"; //configuration file location on host

        TVSeriesInfo SeriesInfo;
        List<string> ListOfEpisodeNames = new List<string>();
        List<string> KODIEpisodeNames = new List<string>();
        List<string> SynologyEpisodeName = new List<string>();
        List<string> PLEXEpisodeNames = new List<string>();
        ConfigFile cf = new ConfigFile();

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

            //Keeps episode names list clear in cases of searching many series in one session.
            KODIEpisodeNames.Clear();
            PLEXEpisodeNames.Clear();
            SynologyEpisodeName.Clear();

            //Scan for Series ID, if no ID found scan for series title and retrieve list of possible series to choose from then parse out the correct id.
            if (int.TryParse(TitleBox, out ID) && ID != 0)
            {
                try
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
                catch //ID Search returned error, attempt search as a series title instead.
                {
                    notificationLabel.Text = "ID search failed, searching " + TitleBox + "as text";
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
                        if (T.series.ContainsKey("banner"))
                        {
                            seriesImagePicturebox.ImageLocation = "http://thetvdb.com/banners/" + T.series["banner"];
                        }
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
                    if (T.series.ContainsKey("banner"))
                    {
                        seriesImagePicturebox.ImageLocation = "http://thetvdb.com/banners/" + T.series["banner"];
                    }
                    
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
                    //ext not populated yet
                    PLEXEpisodeNames.Add(SeriesInfo.series["SeriesName"] + " - " + Snum.ToLower() + Enum.ToLower() + " - " + SeriesInfo.episodes[i]["EpisodeName"]);
                    KODIEpisodeNames.Add(SeriesInfo.series["SeriesName"] + "_" + Snum.ToLower() + Enum.ToLower() + "_" + SeriesInfo.episodes[i]["EpisodeName"]);
                    SynologyEpisodeName.Add(SeriesInfo.series["SeriesName"] + "." + Snum + "." + Enum + "." + SeriesInfo.episodes[i]["EpisodeName"]);
                }
            }

            notificationLabel.Text = "";
            notificationLabel.Visible = false;
            notificationLabel.Invalidate();
            notificationLabel.Update();
        }
        private void getHTMLButton_Click(object sender, EventArgs e) { getHTML(); }
        private void getFileNames()
        {

            notificationLabel.Visible = true;
            notificationLabel.Text = "Querying Files";
            notificationLabel.Invalidate();
            notificationLabel.Update();

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
                cf.updateDefaults(); //updates default settings

                parentPathLabel.Text = FBD.SelectedPath.ToString() + "\\"; //adds a \ to the end of folderpath, double backslash required to add a single one to a string


                /*"Video Files|*.mpg;*.mpeg;*.vob;*.mod;*.ts;*.m2ts;*.mp4;*.m4v;*.mov;*.avi;*.divx;*.wmv;"
                +"*.asf;*.mkv;*.flv;*.f4v;*.dvr;*.dvr-ms;*.wtv;*.ogv;*.ogm;*.3gp;*.rm;*.rmvb;";*/

                string[] fileNames = Directory
                    .GetFiles(cf.DefaultSettings["TFPath"], "*.*")
                    .Where(file => file.ToLower().EndsWith(".mpg") || file.ToLower().EndsWith(".mpeg") || file.ToLower().EndsWith(".vob") || file.ToLower().EndsWith(".mod") || file.ToLower().EndsWith(".ts") || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4") || file.ToLower().EndsWith(".m4v") || file.ToLower().EndsWith(".mov") || file.ToLower().EndsWith("avi") || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv") || file.ToLower().EndsWith(".asf") || file.ToLower().EndsWith(".mkv") || file.ToLower().EndsWith(".flv") || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr") || file.ToLower().EndsWith(".dvr-ms") || file.ToLower().EndsWith(".wtv") || file.ToLower().EndsWith(".ogv") || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp") || file.ToLower().EndsWith(".rm") || file.ToLower().EndsWith(".rmvb"))
                    .ToArray();

                foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                {
                    fileName = file.Replace(cf.DefaultSettings["TFPath"] + "\\", "");

                    if (!fileName.StartsWith("._"))
                    {
                        fileNamesListbox.Items.Add(fileName);
                    }
                }
            }
            notificationLabel.Visible = false;
        }
        private void getFileNamesButton_Click(object sender, EventArgs e)
        {
            getFileNames();
        }
        private void previewChanges()
        {
            notificationLabel.Visible = true;
            notificationLabel.Text = "";
            notificationLabel.Invalidate();
            notificationLabel.Update();

            if (fileNamesListbox.Items.Count > 0 && SeriesInfo != null)
            {
                changedFileNamesListbox.Items.Clear(); //clears listbox

                notificationLabel.Text = "Determining Episode Numbers from Filename";
                notificationLabel.Invalidate();
                notificationLabel.Update();

                determineEpisodeFromFileName(); //sets season and episode from filename
            }
            else
            {
                CustomMessageBox.Show("Files must be selected and HTML must be created", 125, 277);
            }
            notificationLabel.Visible = false;
        }
        private void previewChangesButton_Click(object sender, EventArgs e)
        {
            previewChanges();
            if (changedFileNamesListbox.Items.Count > 0 & fileNamesListbox.Items.Count > 0)
            {
                if (fileNamesListbox.SelectedIndex > -1)
                {
                    changedFileNamesListbox.SelectedIndex = fileNamesListbox.SelectedIndex;
                }
            }
        }
        private void determineEpisodeFromFileName()
        {
            season = "";
            episode = "";
            char[] delim = { '.' };
            List<string> episodeNames = new List<string>();

            int maxSeason = 0; //Used to restrict season loops
            int maxEpisode = 0; //used to restrict episode loops

            if (fileNamesListbox.Items.Count > 0) //checks that fileNamesListbox is not empty
            {
                
                //look for highest season number, highest episode nmber and build list of all episodes of the show.
                for (int i = 0; i < SeriesInfo.episodes.Count(); i++)
                {
                    if (SeriesInfo.episodes[i].ContainsKey("SeasonNumber") && int.Parse(SeriesInfo.episodes[i]["SeasonNumber"]) > maxSeason)
                    {
                        maxSeason = int.Parse(SeriesInfo.episodes[i]["SeasonNumber"]);
                    }

                    //Look for largest episode number in all seasons of the show.
                    if (SeriesInfo.episodes[i].ContainsKey("EpisodeNumber") && int.Parse(SeriesInfo.episodes[i]["EpisodeNumber"]) > maxEpisode)
                    {
                        maxEpisode = int.Parse(SeriesInfo.episodes[i]["EpisodeNumber"]);
                    }

                    if (SeriesInfo.episodes[i].ContainsKey("EpisodeName"))
                    {
                        episodeNames.Add(SeriesInfo.episodes[i]["EpisodeName"]);
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

                    //loop through selected indecies to ensure item is selected or not
                    if(fileNamesListbox.SelectedIndices.Count > 0)
                    {
                        for (int a = 0; a < fileNamesListbox.SelectedIndices.Count; a++)
                        {
                            //If its found to exist in the selected indices
                            if (fileNamesListbox.SelectedIndices[a] == i)
                            {
                                isSelected = true;
                            }
                        }
                        if(isSelected)
                        {
                            season = checkSeason(fileNamesListbox.Items[i].ToString().ToUpper(), maxSeason); //tries to parse season info from filename
                            episode = checkEpisode(fileNamesListbox.Items[i].ToString().ToUpper(), maxEpisode); //tries to parse episode info from filename
                        }
                        else
                        {
                            season = "-1";
                            episode = "-1";
                        }
                    }
                    else //None selected, perform check on the entire list.
                    {
                        season = checkSeason(fileNamesListbox.Items[i].ToString().ToUpper(), maxSeason); //tries to parse season info from filename
                        episode = checkEpisode(fileNamesListbox.Items[i].ToString().ToUpper(), maxEpisode); //tries to parse episode info from filename
                    }
                    

                    

                    if ((string.IsNullOrEmpty(season) | string.IsNullOrEmpty(episode)) & titleCb.Checked)
                    {
                        //Add loop for each episode to see if an episode title in the SeriesInfo object matches a filename. If so return the episode and season from that.
                        for (int a = 0; a < SeriesInfo.episodes.Count; a++)
                        {
                            if (SeriesInfo.episodes[a].ContainsKey("EpisodeName"))
                            {
                                if ((fileNamesListbox.Items[i].ToString().ToUpper()).Contains(SeriesInfo.episodes[a]["EpisodeName"].ToUpper()))
                                {

                                    if (int.Parse(SeriesInfo.episodes[a]["SeasonNumber"]) < 10)
                                    {
                                        season = "S0" + SeriesInfo.episodes[a]["SeasonNumber"];
                                    }
                                    else
                                    {
                                        season = "S" + SeriesInfo.episodes[a]["SeasonNumber"];
                                    }

                                    if (int.Parse(SeriesInfo.episodes[a]["EpisodeNumber"]) < 10)
                                    {
                                        episode = "E0" + SeriesInfo.episodes[a]["EpisodeNumber"];
                                    }
                                    else
                                    {
                                        episode = "E" + SeriesInfo.episodes[a]["EpisodeNumber"];
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
                                            newTitle = SeriesInfo.series["SeriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.episodes[a]["EpisodeName"] + "." + ext;
                                            break;
                                        case 1: //KODI
                                            newTitle = SeriesInfo.series["SeriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.episodes[a]["EpisodeName"] + "." + ext;
                                            break;
                                        case 2: //Synology
                                            newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[a]["EpisodeName"] + "." + ext;
                                            break;
                                        default: //Synology
                                            newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[a]["EpisodeName"] + "." + ext;
                                            break;
                                    }
                                    //newTitle = SeriesInfo.series["SeriesName"] + " " + season + episode + " " + SeriesInfo.episodes[a]["EpisodeName"] + "." + ext;
                                }
                            }
                        }
                    }
                    //Selections made, other items are marked with -1 and then skipped
                    if (season == "-1") { newTitle = "SKIPPED"; }

                    if ((string.IsNullOrEmpty(season) | string.IsNullOrEmpty(episode)) & absoluteCb.Checked)
                    {
                        // Add filter for absolute episode numbers here
                        newTitle = checkAbsolutNumber(fileNamesListbox.Items[i].ToString());
                        if (string.IsNullOrEmpty(newTitle)) { newTitle = "EPISODE COULD NOT BE DETERMINED"; }
                    }

                    if (string.IsNullOrEmpty(newTitle))//Check for matching episode and season entry
                    {
                        for (int e = 0; e < SeriesInfo.episodes.Count(); e++)
                        {
                            if (SeriesInfo.episodes[e].ContainsKey("EpisodeNumber")
                                & SeriesInfo.episodes[e]["EpisodeNumber"] == episode)
                            {
                                if (SeriesInfo.episodes[e].ContainsKey("SeasonNumber")
                                    & SeriesInfo.episodes[e]["SeasonNumber"] == season)
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
                                            newTitle = SeriesInfo.series["SeriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.episodes[e]["EpisodeName"] + "." + ext;
                                            break;
                                        case 1: //KODI
                                            newTitle = SeriesInfo.series["SeriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.episodes[e]["EpisodeName"] + "." + ext;
                                            break;
                                        case 2: //Synology
                                            newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[e]["EpisodeName"] + "." + ext;
                                            break;
                                        default: //Synology
                                            newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[e]["EpisodeName"] + "." + ext;
                                            break;
                                    }
                                    //newTitle = SeriesInfo.series["SeriesName"] + " " + season + episode + " " + SeriesInfo.episodes[e]["EpisodeName"] + "." + ext;
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
                        newTitle = formatFileName(newTitle); //removes invalid characters from the filename.
                    }
                    changedFileNamesListbox.Items.Add(newTitle);
                }
            }
            episodeNames.Clear();
        }
        private string checkSeason(string FileName, int maxSeason)
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
                    seasonNumber = i; //Loops through all episodes check for season number, that way it will check for season 1 as well as 10 and not return only season 1.
                }
            }
            if (seasonNumber != -1) { return seasonNumber.ToString(); }*/

            //Check for season info in filename e.g. S0 + the loop number like 9, etc
            //Loop only goes as high as the number of seasons in the series (Checked by thetvdb.com)
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
                    seasonNumber = i; //Loops through all episodes check for season number, that way it will check for season 1 as well as 10 and not return only season 1.
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
        private string checkEpisode(string FileName, int maxEpisode)
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
        private string checkAbsolutNumber(string FileName)
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
                    if (int.Parse(block) <= SeriesInfo.episodes.Count() + 1)
                    {
                        string newTitle = "";

                        if (int.Parse(SeriesInfo.episodes[int.Parse(block) - 1]["SeasonNumber"]) >= 10)
                        {
                            season = "S" + SeriesInfo.episodes[int.Parse(block) - 1]["SeasonNumber"];
                        }
                        else { season = "S0" + SeriesInfo.episodes[int.Parse(block) - 1]["SeasonNumber"]; }

                        if (int.Parse(SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeNumber"]) >= 10)
                        {
                            episode = "E" + SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeNumber"];
                        }
                        else { episode = "E0" + SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeNumber"]; }

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
                                newTitle = SeriesInfo.series["SeriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeName"] + "." + ext;
                                break;
                            case 1: //KODI
                                newTitle = SeriesInfo.series["SeriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeName"] + "." + ext;
                                break;
                            case 2: //Synology
                                newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeName"] + "." + ext;
                                break;
                            default: //Synology
                                newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeName"] + "." + ext;
                                break;
                        }
                        return newTitle;
                    }
                }
            }
            Array.Clear(characterBlocks, 0, characterBlocks.Length);
            return "";
        }
        private void fileNamesListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (changedFileNamesListbox.Items.Count > 0 & fileNamesListbox.Items.Count > 0)
                {
                    //ensures when selecting an item from the listbox that both listboxes highlight the same index
                    changedFileNamesListbox.SelectedIndex = fileNamesListbox.SelectedIndex;
                }
            }
            catch
            {

            }

        }
        private void changedFileNamesListbox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (fileNamesListbox.Items.Count > 0 & changedFileNamesListbox.Items.Count > 0)
                {
                    //ensures when selecting an item from the listbox that both listboxes highlight the same index
                    fileNamesListbox.SelectedIndex = changedFileNamesListbox.SelectedIndex;
                }
            }
            catch
            {

            }
        }
        private string formatFileName(string fileName)
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
        private void changeFileNamesButton_Click(object sender, EventArgs e)
        {
            notificationLabel.Visible = true;
            notificationLabel.Text = "Processing File Names";
            notificationLabel.Invalidate();
            notificationLabel.Update();

            string errorList = "";
            if (!string.IsNullOrEmpty(parentPathLabel.Text))
            {
                //re-names files to the correct format base on thetvdb.com all seasons url
                string folderPath = parentPathLabel.Text; //current directory of the files
                string fileName = "";

                for (int i = 0; i < fileNamesListbox.Items.Count; i++)
                {
                    //If the names are the same, don't bother doing anything else.
                    if (fileNamesListbox.Items[i].ToString() != changedFileNamesListbox.Items[i].ToString())
                    {
                        notificationLabel.Text = "Re-Naming File - " + fileNamesListbox.Items[i].ToString();
                        notificationLabel.Invalidate();
                        notificationLabel.Update();

                        if (changedFileNamesListbox.Items[i].ToString().Contains("EPISODE COULD NOT BE DETERMINED") ||
                            changedFileNamesListbox.Items[i].ToString().Contains("NO SUCH EPISODE FOUND"))
                        {
                            errorList += fileNamesListbox.Items[i].ToString() + " - " + changedFileNamesListbox.Items[i].ToString() + "\n";
                        }
                        /*Don't attempt to change the filename if a file with the new name already exists.
                        This would be the case if there were multiple files with the same name in the chosen
                            directory*/

                        else if(changedFileNamesListbox.Items[i].ToString() !="")
                        {
                            if (!File.Exists(parentPathLabel.Text + changedFileNamesListbox.Items[i].ToString()))
                            {
                                //Using File.Move to change the fileNames.
                                System.IO.File.Move(parentPathLabel.Text + fileNamesListbox.Items[i].ToString(), parentPathLabel.Text + changedFileNamesListbox.Items[i].ToString());
                            }
                            else if (File.Exists(parentPathLabel.Text + changedFileNamesListbox.Items[i].ToString()))
                            {
                                errorList += fileNamesListbox.Items[i].ToString() + " - Duplicate Episode\n";
                            }
                        }
                    }

                }

                if (!string.IsNullOrEmpty(errorList))
                {
                    CustomMessageBox.Show("The following files had errors: \n" + errorList, 470, 530);
                }

                fileNamesListbox.Items.Clear(); //clears the fileNamesListbox so it can be refreshed with current data


                notificationLabel.Text = "Re-Querying Files ";
                notificationLabel.Invalidate();
                notificationLabel.Update();

                /*pulls in the video files located in the folderPath directory
                 These files are the newly renamed files*/
                string[] fileNames = Directory
                        .GetFiles(folderPath, "*.*")
                        .Where(file => file.ToLower().EndsWith(".mpg") || file.ToLower().EndsWith(".mpeg") || file.ToLower().EndsWith(".vob") || file.ToLower().EndsWith(".mod") || file.ToLower().EndsWith(".ts") || file.ToLower().EndsWith(".m2ts")
                        || file.ToLower().EndsWith(".mp4") || file.ToLower().EndsWith(".m4v") || file.ToLower().EndsWith(".mov") || file.ToLower().EndsWith(".avi") || file.ToLower().EndsWith(".divx")
                        || file.ToLower().EndsWith(".wmv") || file.ToLower().EndsWith(".asf") || file.ToLower().EndsWith(".mkv") || file.ToLower().EndsWith(".flv") || file.ToLower().EndsWith(".f4v")
                        || file.ToLower().EndsWith(".dvr") || file.ToLower().EndsWith(".dvr-ms") || file.ToLower().EndsWith(".wtv") || file.ToLower().EndsWith(".ogv") || file.ToLower().EndsWith(".ogm")
                        || file.ToLower().EndsWith(".3gp") || file.ToLower().EndsWith(".rm") || file.ToLower().EndsWith(".rmvb"))
                        .ToArray();
                foreach (string file in fileNames)
                {
                    fileName = file.Replace(folderPath, ""); //ensures you only return the filename and not the rest of the path
                    fileNamesListbox.Items.Add(fileName); //adds file names to the listbox
                }

                changedFileNamesListbox.Items.Clear();
            }
            else
            {
                CustomMessageBox.Show("No directory path", 115, 195);
            }
            notificationLabel.Visible = false;
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            clearAll();
            DialogResult = DialogResult.None; //prevent form from closing
        }
        private void clearAll()
        {
            seriesIDTitleTextbox.Clear();
            favoritesCombo.Text = "";
            seriesImagePicturebox.ImageLocation = "";
            parentPathLabel.Text = "";
            fileNamesListbox.Items.Clear();
            changedFileNamesListbox.Items.Clear();

        }
        private void fileNamesListbox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int value = 0;

            //Keeps both listboxex in sync by deleting items from both boxes at once
            if (fileNamesListbox.Items.Count > 0 & fileNamesListbox.SelectedIndex >= 0 & changedFileNamesListbox.Items.Count > 0)
            {
                value = fileNamesListbox.SelectedIndex;
                fileNamesListbox.Items.RemoveAt(value);
                changedFileNamesListbox.Items.RemoveAt(value);
            }

            //Allows you to delete items from fileNamesListbox if changedFileNamesListbox isn't populated
            if (fileNamesListbox.Items.Count > 0 & fileNamesListbox.SelectedIndex >= 0 & changedFileNamesListbox.Items.Count == 0)
            {
                value = fileNamesListbox.SelectedIndex;
                fileNamesListbox.Items.RemoveAt(value);
            }

        }
        private void changedFileNamesListbox_MouseDoubleClick(object sender, MouseEventArgs e)
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
                    changedFileNamesListbox.Items.Insert(index, R.changedFileName);
                }
                R.Dispose();
            }
        }
        private void TVForm_Load(object sender, EventArgs e)
        {
            cf.checkConfigFile();

            //Add items to Favorites Combo
            for (int i = 0; i < cf.favoriteTitles.Count; i++)
            {
                favoritesCombo.Items.Add(cf.favoriteTitles[i]);
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

            switch (cf.DefaultSettings["TVTitleInFilenameCheck"])
            {
                case "True":
                    titleCb.Checked = true;
                    break;
                case "False":
                    titleCb.Checked = false;
                    break;
                default:
                    titleCb.Checked = false;
                    break;
            }

            switch (cf.DefaultSettings["TVAbsoluteNumbersCheck"])
            {
                case "True":
                    absoluteCb.Checked = true;
                    break;
                case "False":
                    absoluteCb.Checked = false;
                    break;
                default:
                    absoluteCb.Checked = false;
                    break;
            }
        }
        private void addFavoriteButton_Click(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(favoritesCombo.Text) & !string.IsNullOrEmpty(seriesIDTitleTextbox.Text))
            {

                cf.addFavorite(favoritesCombo.Text, seriesIDTitleTextbox.Text);

                seriesIDTitleTextbox.Text = "";
                favoritesCombo.Text = "";
                favoritesCombo.Items.Clear();

                for (int i = 0; i < cf.favoriteTitles.Count; i++)
                {
                    favoritesCombo.Items.Add(cf.favoriteTitles[i]);
                }
                

            }
            else
            {
                CustomMessageBox.Show("Saving a favorite requires both a URL and a name in the favorites box", 136, 367);
            }

            DialogResult = DialogResult.None; //Prevents form from closing...
        }
        private void favoritesCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {

            seriesIDTitleTextbox.Text = cf.favoriteIDs[favoritesCombo.SelectedIndex]; //Matches the favorite name with the correct URL

            if (!string.IsNullOrEmpty(favoritesCombo.SelectedItem.ToString()))
            {
                getHTML(); //Prevents you from having to click the GetHTML button since that will be your next move anyway
            }

        }
        private void deleteFavoriteButton_Click(object sender, EventArgs e)
        {
            cf.removeFavorite(favoritesCombo.SelectedIndex);
            favoritesCombo.Text = "";
            seriesIDTitleTextbox.Text = "";
            seriesImagePicturebox.ImageLocation = "";

            favoritesCombo.Items.Clear();
            for (int i = 0; i < cf.favoriteTitles.Count; i++)
            {
                favoritesCombo.Items.Add(cf.favoriteTitles[i]);
            }
      
        }
        private void InvisibleCloseButton_Click(object sender, EventArgs e)
        {
            this.Close(); //Located behind the Make Changes button
        }
        private void formatCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(fileNamesListbox.Items.Count > 0) //ensures that this doesnt fire when there is nothing for it to do but error
            {
                previewChanges(); //updates titles for seletion

                if (changedFileNamesListbox.Items.Count > 0 & fileNamesListbox.Items.Count > 0)
                {
                    if (fileNamesListbox.SelectedIndex > -1)
                    {
                        changedFileNamesListbox.SelectedIndex = fileNamesListbox.SelectedIndex;
                    }
                }
            }
            cf.DefaultSettings["DefaultFormat"] = formatCombo.SelectedItem.ToString();
            cf.updateDefaults();
        }
        private void theTVDBcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.thetvdb.com");
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get File Names
            getFileNames();
        }
        private void absoluteCb_CheckedChanged(object sender, EventArgs e)
        {
            switch(absoluteCb.Checked)
            {
                case true:
                    cf.DefaultSettings["TVAbsoluteNumbersCheck"] = "True";
                    cf.updateDefaults();
                    break;
                case false:
                    cf.DefaultSettings["TVAbsoluteNumbersCheck"] = "False";
                    cf.updateDefaults();
                    break;
                default:
                    cf.DefaultSettings["TVAbsoluteNumbersCheck"] = "False";
                    cf.updateDefaults();
                    break;
            }
        }

        private void titleCb_CheckedChanged(object sender, EventArgs e)
        {
            switch(titleCb.Checked)
            {
                case true:
                    cf.DefaultSettings["TVTitleInFilenameCheck"] = "True";
                    cf.updateDefaults();
                    break;
                case false:
                    cf.DefaultSettings["TVTitleInFilenameCheck"] = "False";
                    cf.updateDefaults();
                    break;
                default:
                    cf.DefaultSettings["TVTitleInFilenameCheck"] = "False";
                    cf.updateDefaults();
                    break;
            }
        }
    }
}
