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
        private void getFileNames()
        {
            ReadDefaultFilePath();

            notificationLabel.Visible = true;
            notificationLabel.Text = "Querying Files";
            notificationLabel.Invalidate();
            notificationLabel.Update();

            string fileName = "";
            fileNamesListbox.Items.Clear();

            FolderBrowserDialog FBD = new FolderBrowserDialog(); //creates new instance of the FolderBrowserDialog

            if (!string.IsNullOrEmpty(folderPath)) //if defaultpath contains a path, sets folderBrowserDialog to default to this path
            {
                FBD.SelectedPath = folderPath;
            }

            if (FBD.ShowDialog() == DialogResult.OK) //shows folderbrowserdialog, runs addtional code if not cancelled out
            {
                folderPath = FBD.SelectedPath;

                parentPathLabel.Text = FBD.SelectedPath.ToString() + "\\"; //adds a \ to the end of folderpath, double backslash required to add a single one to a string


                /*"Video Files|*.mpg;*.mpeg;*.vob;*.mod;*.ts;*.m2ts;*.mp4;*.m4v;*.mov;*.avi;*.divx;*.wmv;"
                +"*.asf;*.mkv;*.flv;*.f4v;*.dvr;*.dvr-ms;*.wtv;*.ogv;*.ogm;*.3gp;*.rm;*.rmvb;";*/

                string[] fileNames = Directory
                    .GetFiles(folderPath, "*.*")
                    .Where(file => file.ToLower().EndsWith(".mpg") || file.ToLower().EndsWith(".mpeg") || file.ToLower().EndsWith(".vob") || file.ToLower().EndsWith(".mod") || file.ToLower().EndsWith(".ts") || file.ToLower().EndsWith(".m2ts")
                    || file.ToLower().EndsWith(".mp4") || file.ToLower().EndsWith(".m4v") || file.ToLower().EndsWith(".mov") || file.ToLower().EndsWith("avi") || file.ToLower().EndsWith(".divx")
                    || file.ToLower().EndsWith(".wmv") || file.ToLower().EndsWith(".asf") || file.ToLower().EndsWith(".mkv") || file.ToLower().EndsWith(".flv") || file.ToLower().EndsWith(".f4v")
                    || file.ToLower().EndsWith(".dvr") || file.ToLower().EndsWith(".dvr-ms") || file.ToLower().EndsWith(".wtv") || file.ToLower().EndsWith(".ogv") || file.ToLower().EndsWith(".ogm")
                    || file.ToLower().EndsWith(".3gp") || file.ToLower().EndsWith(".rm") || file.ToLower().EndsWith(".rmvb"))
                    .ToArray();

                foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                {
                    fileName = file.Replace(folderPath + "\\", "");

                    if (!fileName.StartsWith("._"))
                    {
                        fileNamesListbox.Items.Add(fileName);
                    }
                }


                WriteDefaultFilePath();
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


                for (int i = 0; i < fileNamesListbox.Items.Count; i++) //loop through listbox items
                {
                    string[] Tokens = fileNamesListbox.Items[i].ToString().Split(delim);
                    ext = Tokens[Tokens.Count() - 1]; //should be extension
                    season = "";
                    episode = "";
                    string newTitle = "";


                    season = checkSeason(fileNamesListbox.Items[i].ToString().ToUpper(), maxSeason); //tries to parse season info from filename
                    episode = checkEpisode(fileNamesListbox.Items[i].ToString().ToUpper(), maxEpisode); //tries to parse episode info from filename

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
                                    switch (formatCombo.Text)
                                    {
                                        case "Synology":
                                            newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[a]["EpisodeName"] + "." + ext;
                                            break;
                                        case "PLEX":
                                            newTitle = SeriesInfo.series["SeriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.episodes[a]["EpisodeName"] + "." + ext;
                                            break;
                                        case "KODI":
                                            newTitle = SeriesInfo.series["SeriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.episodes[a]["EpisodeName"] + "." + ext;
                                            break;
                                        default:
                                            newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[a]["EpisodeName"] + "." + ext;
                                            break;
                                    }
                                    //newTitle = SeriesInfo.series["SeriesName"] + " " + season + episode + " " + SeriesInfo.episodes[a]["EpisodeName"] + "." + ext;
                                }
                            }
                        }
                    }

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
                                    switch (formatCombo.Text)
                                    {
                                        case "Synology":
                                            newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[e]["EpisodeName"] + "." + ext;
                                            break;
                                        case "PLEX":
                                            newTitle = SeriesInfo.series["SeriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.episodes[e]["EpisodeName"] + "." + ext;
                                            break;
                                        case "KODI":
                                            newTitle = SeriesInfo.series["SeriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.episodes[e]["EpisodeName"] + "." + ext;
                                            break;
                                        default:
                                            newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[e]["EpisodeName"] + "." + ext;
                                            break;
                                    }
                                    //newTitle = SeriesInfo.series["SeriesName"] + " " + season + episode + " " + SeriesInfo.episodes[e]["EpisodeName"] + "." + ext;
                                }
                            }
                        }
                        if (string.IsNullOrEmpty(newTitle) & !string.IsNullOrEmpty(season) & !string.IsNullOrEmpty(episode)) { newTitle = "NO SUCH EPISODE FOUND"; }
                        else if (string.IsNullOrEmpty(newTitle)) { newTitle = "EPISODE COULD NOT BE DETERMINED"; }
                    }
                    newTitle = formatFileName(newTitle); //removes invalid characters from the filename.
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
                    FileName.Contains(i.ToString() + ".E") ||
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
                        switch (formatCombo.Text)
                        {
                            case "Synology":
                                newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeName"] + "." + ext;
                                break;
                            case "PLEX":
                                newTitle = SeriesInfo.series["SeriesName"] + " - " + season.ToLower() + episode.ToLower() + " - " + SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeName"] + "." + ext;
                                break;
                            case "KODI":
                                newTitle = SeriesInfo.series["SeriesName"] + "_" + season.ToLower() + episode.ToLower() + "_" + SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeName"] + "." + ext;
                                break;
                            default:
                                newTitle = SeriesInfo.series["SeriesName"] + "." + season + "." + episode + "." + SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeName"] + "." + ext;
                                break;
                        }
                        return newTitle;
                        //return SeriesInfo.series["SeriesName"] + " " + season + episode + " " + SeriesInfo.episodes[int.Parse(block) - 1]["EpisodeName"] + "." + ext;
                    }
                }
            }
            Array.Clear(characterBlocks, 0, characterBlocks.Length);
            return "";
        }
        private string GeneralParser(string InputString, string start, string end)
        {
            if (string.IsNullOrEmpty(InputString)) { return ""; }
            int startPosition = 0;
            int endPosition = 0;
            try
            {
                if (InputString.Contains(start) & InputString.Length > start.Length)
                {
                    startPosition = InputString.IndexOf(start) + start.Length;
                }
                else { return ""; }

                if (InputString.Contains(end) & InputString.Length > end.Length)
                {
                    endPosition = InputString.IndexOf(end, startPosition);
                }
                else { return ""; }

                if (startPosition == -1 || endPosition == -1) { return ""; }

                if (startPosition >= endPosition) { return ""; }

                if (InputString.Length - startPosition > endPosition - startPosition)
                {
                    return InputString.Substring(startPosition, endPosition - startPosition);
                }
                else { return ""; }
            }
            catch
            {
                return "";
            }
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
                        This would be the case if there were multiple fils with the same name in the chosen
                            directory*/

                        else
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
            if (changedFileNamesListbox.Items.Count > 0)
            {
                int index = changedFileNamesListbox.SelectedIndex;
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
        private void BulkScanTVShows_Load(object sender, EventArgs e)
        {
            string format = "";
            ReadDefaultFilePath();
            Favorites();
            using (StreamReader SR = new StreamReader(configPath))
            {
                //Read configFileText to string
                configFileText = SR.ReadToEnd();
                //<DefaultFormatStart>KODI<DefaultFormatEnd>
                format = GeneralParser(configFileText, "<DefaultFormatStart>", "<DefaultFormatEnd>");
                SR.Close();
            }
            switch (format)
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
        private void ReadDefaultFilePath()
        {
            configFileText = "";
            if (System.IO.File.Exists(configPath))
            {
                //parse default path from config file and set it to folder path
                using (StreamReader SR = new StreamReader(configPath))
                {
                    //Read configFileText to string
                    configFileText = SR.ReadToEnd();
                    folderPath = GeneralParser(configFileText, "<BulkScanTVShowStart>", "<BulkScanTVShowEnd>");
                    defaultFormat = GeneralParser(configFileText, "<DefaultFormat>", "<DefaultFormatEnd>");
                    SR.Close();
                }
                switch (defaultFormat)
                {
                    case "Plex":
                        formatCombo.SelectedIndex = 0;
                        break;
                    case "Kodi":
                        formatCombo.SelectedIndex = 1;
                        break;
                    case "Synology":
                        formatCombo.SelectedIndex = 2;
                        break;
                    default:
                        formatCombo.SelectedIndex = 2;
                        break;
                }
            }
        }
        private void WriteDefaultFilePath()
        {
            string oldText = "";
            string newText = "";

            using (StreamReader SR = new StreamReader(configPath))
            {
                //Read configFileText to string
                configFileText = SR.ReadToEnd();
                defaultPathText = GeneralParser(configFileText, "<BulkScanTVShowStart>", "<BulkScanTVShowEnd>");
                oldText = "<BulkScanTVShowStart>" + defaultPathText + "<BulkScanTVShowEnd>";
                newText = "<BulkScanTVShowStart>" + folderPath + "<BulkScanTVShowEnd>";
                configFileText = configFileText.Replace(oldText, newText);
                SR.Close();
                SR.Dispose();
            }

            using (StreamWriter sw = File.CreateText(configPath))
            {
                sw.WriteLine(configFileText);
                sw.Close();
            }
        }
        private void addFavoriteButton_Click(object sender, EventArgs e)
        {
            string seriesURL = "";
            string seriesName = "";

            if (!string.IsNullOrEmpty(favoritesCombo.Text) & !string.IsNullOrEmpty(seriesIDTitleTextbox.Text))
            {
                seriesURL = seriesIDTitleTextbox.Text;
                seriesName = favoritesCombo.Text;
                favoritesCombo.Items.Add(seriesName);
                FavoriteURLList.Add(seriesURL);

                if (!Directory.Exists(configDirectory))
                {
                    Directory.CreateDirectory(configDirectory);
                }

                if (!File.Exists(configPath))
                {
                    using (StreamWriter FavoritesShows1 = File.CreateText(configPath)) { FavoritesShows1.Close(); }

                }

                using (StreamWriter FavoriteShows2 = File.AppendText(configPath))
                {
                    string newShow = "<TVShow>" + seriesName + "<ShowURL>" + seriesURL + "<End>";
                    FavoriteShows2.WriteLine(newShow);
                    FavoriteShows2.Close();
                }

                FavoriteURLList.Clear();
                favoritesCombo.Items.Clear();

                Favorites();

                seriesIDTitleTextbox.Text = "";
                favoritesCombo.Text = "";
            }
            else
            {
                CustomMessageBox.Show("Saving a favorite requires both a URL and a name in the favorites box", 136, 367);
            }

            DialogResult = DialogResult.None; //Prevents form from closing...
        }
        private void favoritesCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {

            seriesIDTitleTextbox.Text = FavoriteURLList[favoritesCombo.SelectedIndex].ToString(); //Matches the favorite name with the correct URL

            if (!string.IsNullOrEmpty(favoritesCombo.SelectedItem.ToString()))
            {
                getHTML(); //Prevents you from having to click the GetHTML button since that will be your next move anyway
            }

        }
        private void Favorites()
        {
            string showName = "";
            string showURL = "";
            string fileName = configPath;
            string textLine = "";
            string line = "";
            FavoriteListSorter.Clear();
            try
            {
                if (File.Exists(fileName))
                {
                    using (StreamReader SR = new StreamReader(fileName))
                    {
                        //Read configFileText to string
                        configFileText = SR.ReadToEnd();
                        SR.Close();
                        SR.Dispose();
                    }
                    defaultPaths = GeneralParser(configFileText, "--DefaultPathsStart--", "--DefaultPathsEnd--");
                    formatCheckDefaults = GeneralParser(configFileText, "--FormatCheckDefaultsStart--", "--FormatCheckDefaultsEnd--");

                    using (StreamReader SR = new StreamReader(fileName))
                    {
                        while (!SR.EndOfStream) //Loops through file until the end
                        {
                            line = SR.ReadLine();
                            //Parse whole lines from Text File 
                            if (line.Contains("<TVShow>"))
                            {
                                textLine = GeneralParser(line, "<TVShow>", "<End>");
                                FavoriteListSorter.Add(textLine);
                            }
                        }
                        SR.Close();
                        SR.Dispose();
                    }

                    FavoriteListSorter.Sort(); //Sort List by Show Title

                    //Create a new Config.txt file that is sorted
                    using (StreamWriter sw = File.CreateText(fileName))
                    {
                        sw.WriteLine("--DefaultPathsStart--" + defaultPaths + "--DefaultPathsEnd--");
                        sw.WriteLine("");
                        sw.WriteLine("--FavoritesListStart--");

                        for (int i = 0; i < FavoriteListSorter.Count(); i++)
                        {
                            if (!string.IsNullOrEmpty(FavoriteListSorter[i]))
                            {
                                sw.WriteLine("<TVShow>" + FavoriteListSorter[i] + "<End>");
                            }

                        }
                        sw.WriteLine("--FavoritesListEnd--\r\n");
                        sw.WriteLine("--FormatCheckDefaultsStart--" + formatCheckDefaults + "--FormatCheckDefaultsEnd--");
                        sw.Close();
                    }

                    //Add empty space to the top of combobox and List
                    favoritesCombo.Items.Add("");
                    FavoriteURLList.Add("");


                    using (StreamReader sr = new StreamReader(fileName))
                    {
                        while (!sr.EndOfStream) //Loops through file until the end
                        {
                            line = sr.ReadLine();
                            //Parse Line to divide Common and URL names
                            if (line.Contains("<TVShow>"))
                            {

                                showName = GeneralParser(line, "<TVShow>", "<ShowURL>");
                                showURL = GeneralParser(line, "<ShowURL>", "<End>");

                                //Fill lists
                                favoritesCombo.Items.Add(showName);
                                FavoriteURLList.Add(showURL);
                            }
                        }
                        sr.Close();
                        sr.Dispose();
                    }

                }
            }
            catch
            {
                CustomMessageBox.Show("Error while reading file Config.txt", 126, 343);
            }
        }
        private void deleteFavorite()
        {
            string fileName = configPath;
            string line = "";
            string showName = "";
            string showURL = "";
            int deleteLocation = 0;

            configFileText = "";
            if (favoritesCombo.SelectedIndex >= 0 & favoritesCombo.Items.Count > 0)
            {

                FavoriteListSorter.Clear();
                FavoriteListSorter.Add(""); //Adds blank to match other lists and combo box

                using (StreamReader SR = new StreamReader(fileName))
                {
                    //Read configFileText to string
                    configFileText = SR.ReadToEnd();
                    SR.Close();
                    SR.Dispose();

                    defaultPaths = GeneralParser(configFileText, "--DefaultPathsStart--", "--DefaultPathsEnd--");
                    formatCheckDefaults = GeneralParser(configFileText, "--FormatCheckDefaultsStart--", "--FormatCheckDefaultsEnd--");
                }

                using (StreamReader sr = new StreamReader(fileName))
                {
                    while (!sr.EndOfStream) //Loops through file until the end
                    {
                        line = sr.ReadLine();
                        //Parse Line to divide Common and URL names

                        if (line.Contains("<TVShow>"))
                        {
                            showName = GeneralParser(line, "<TVShow>", "<ShowURL>");
                            showURL = GeneralParser(line, "<ShowURL>", "<End>");

                            //Fill lists
                            FavoriteListSorter.Add("<TVShow>" + showName + "<ShowURL>" + showURL + "<End>");
                        }
                    }
                    sr.Close();
                }

                //remove selected item from combo box and List
                deleteLocation = favoritesCombo.SelectedIndex;
                favoritesCombo.Items.RemoveAt(deleteLocation);
                FavoriteURLList.RemoveAt(deleteLocation);
                FavoriteListSorter.RemoveAt(deleteLocation);


                //createfile to writeover txt file
                using (StreamWriter sw = File.CreateText(fileName))
                {
                    sw.WriteLine("--DefaultPathsStart--" + defaultPaths + "--DefaultPathsEnd--");
                    sw.WriteLine("");
                    sw.WriteLine("--FavoritesListStart--");
                    for (int i = 0; i < FavoriteListSorter.Count; i++) //loop through listbox items
                    {
                        if (!string.IsNullOrEmpty(FavoriteListSorter[i]))
                        {
                            sw.WriteLine(FavoriteListSorter[i]);
                        }
                    }
                    sw.WriteLine("--FavoritesListEnd--\r\n");
                    sw.WriteLine("--FormatCheckDefaultsStart--" + formatCheckDefaults + "--FormatCheckDefaultsEnd--");
                    sw.Close();
                }
                favoritesCombo.Text = "";
                seriesIDTitleTextbox.Text = "";
            }
            else
            {
                CustomMessageBox.Show("Nothing selected for deletion", 110, 227);
            }
        }
        private void deleteFavoriteButton_Click(object sender, EventArgs e)
        {
            deleteFavorite();
        }
        private void InvisibleCloseButton_Click(object sender, EventArgs e)
        {
            this.Close(); //Located behind the Make Changes button
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Get File Names
            getFileNames();
        }
        private void formatCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            writeFormatDefault();
            if (fileNamesListbox.Items.Count > 0)
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

        }
        private void writeFormatDefault()
        {
            string defaults = "";
            string favorites = "";
            string formatCheckDefaults = "";
            string defaultFormat = "";
            string movieDataCollector = "";
            string formatCheckInput = "";
            string bulkScanTV = "";
            string formatCheckOutput = "";

            //Read config file
            using (StreamReader SR = new StreamReader(configPath))
            {
                //Read configFileText to string
                configFileText = SR.ReadToEnd();
                defaults = GeneralParser(configFileText, "--DefaultPathsStart--\r\n", "\r\n--DefaultPathsEnd--\r\n");
                SR.Close();
            }
            //Parse Contents
            favorites = "\r\n--FavoritesListStart--\r\n" + GeneralParser(configFileText, "\r\n--FavoritesListStart--\r\n", "\r\n--FavoritesListEnd--\r\n") + "\r\n--FavoritesListEnd--\r\n";
            formatCheckDefaults = "\r\n--FormatCheckDefaultsStart--\r\n" + GeneralParser(configFileText, "\r\n--FormatCheckDefaultsStart--\r\n", "\r\n--FormatCheckDefaultsEnd--\r\n") + "\r\n--FormatCheckDefaultsEnd--\r\n";

            movieDataCollector = "<MovieCollectorDefaultPathStart>" + GeneralParser(defaults, "<MovieCollectorDefaultPathStart>", "<MovieCollectorDefaultPathEnd>") + "<MovieCollectorDefaultPathEnd>\r\n";
            formatCheckInput = "<FormatCheckDefaultPathStart>" + GeneralParser(defaults, "<FormatCheckDefaultPathStart>", "<FormatCheckDefaultPathEnd>") + "<FormatCheckDefaultPathEnd>\r\n";
            formatCheckOutput = "<FormatCheckOutputStart>" + GeneralParser(defaults, "<FormatCheckOutputStart>", "<FormatCheckOutputEnd>") + "<FormatCheckOutputEnd>\r\n";
            bulkScanTV = "<BulkScanTVShowStart>" + GeneralParser(defaults, "<BulkScanTVShowStart>", "<BulkScanTVShowEnd>") + "<BulkScanTVShowEnd>\r\n";

            //Create new defaut format line
            defaultFormat = "<DefaultFormatStart>" + formatCombo.SelectedItem.ToString() + "<DefaultFormatEnd>\r\n";

            //Merge DefaultPaths area
            defaults = "--DefaultPathsStart--\r\n" + movieDataCollector + formatCheckInput + formatCheckOutput + bulkScanTV + defaultFormat + "--DefaultPathsEnd--\r\n";

            //rewrite file including the default format
            using (StreamWriter sw = File.CreateText(configPath))
            {
                sw.WriteLine(defaults + favorites + formatCheckDefaults);
                sw.Close();
            }
        }
        private void theTVDBcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.thetvdb.com");
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
