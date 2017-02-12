using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.IO; //Allows for file system usage
using System.Net; //Allows for Web Useage
using System.Text.RegularExpressions; //string manipulations
using System.Diagnostics; //Allows for using Process.Start codes lines
using System.Text;

namespace MovieDataCollector
{
    public partial class MovieForm : Form
    {
        const string APIKey = "ff39f5beb591dd0facb7fcf5e5e10425"; //API key for theMovieDB
        string[,] subLanguageCodes = new string[,] //Holds common subtitle language codes
            { {"English","En","Eng"},
              {"French","Fr","Fre"},
              {"French","Fr","Fra"},
              {"German","De","Ger"},
              {"German","De","Deu"},
              {"Spanish","Es","Spa"},
              {"Russian","Ru","Rus"},
              {"Chinese","Zh","Chi"},
              {"Chinese","Zh","Zho"},
              {"Japanese","Ja","Jpn"},
              {"Korean","Ko","Kor"},
              {"Portuguese","Pt","Por"},
              {"Swedish","Sv","Swe"},
              {"Finnish","Fi","Fin"},
              {"Czech","Cs","Cze"},
              {"Czech","Cs","Ces"},
              {"Greek","El","Gre"},
              {"Greek","El","Ell"},
              {"Italian","It","Ita"},

              {"ENGLISH","EN","ENG"},
              {"FRENCH","FR","FRE"},
              {"FRENCH","FR","FRA"},
              {"GERMAN","DE","GER"},
              {"GERMAN","DE","DEU"},
              {"SPANISH","ES","SPA"},
              {"RUSSIAN","RU","RUS"},
              {"CHINESE","ZH","CHI"},
              {"CHINESE","ZH","ZHO"},
              {"JAPANESE","JA","JPN"},
              {"KOREAN","KO","KOR"},
              {"PORTUGUESE","PT","POR"},
              {"SWEDISH","SV","SWE"},
              {"FINNISH","FI","FIN"},
              {"CZECH","CS","CZE"},
              {"CZECH","CS","CES"},
              {"GREEK","EL","GRE"},
              {"GREEK","EL","ELL"},
              {"ITALIAN","IT","ITA"},

              {"english","en","eng"},
              {"french","fr","fre"},
              {"french","fr","fra"},
              {"german","de","ger"},
              {"german","de","deu"},
              {"spanish","es","spa"},
              {"russian","ru","rus"},
              {"chinese","zh","chi"},
              {"chinese","zh","zho"},
              {"japanese","ja","jpn"},
              {"korean","ko","kor"},
              {"portuguese","pt","por"},
              {"swedish","sv","swe"},
              {"finnish","fi","fin"},
              {"czech","cs","cze"},
              {"czech","cs","ces"},
              {"greek","el","gre"},
              {"greek","el","ell"}}; //Language Codes
        string parentPath = ""; //holds string value for video file's directory path
        string videoExtension = ""; //holds the video files extension (.mp4, .mov, etc)
        string videoPath = ""; //Path of the video file
        string videoFileName = ""; //without extension
        string newDirectoryName = ""; //holds name of new directory video and other files get put into.
        string genresNFOString = ""; //hold NFO information for genre
        string studiosNFOString = ""; //holds NFO information for producing studios
        string defaultPathText = "";
        string configFileText = "";
        string configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector";
        string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Config.txt";

        //Filter for opening video files
        const string videoTypeFilter = "Video Files|*.mpg;*.mpeg;*.vob;*.mod;*.ts;*.m2ts;*.mp4;*.m4v;*.mov;*.avi;*.divx;*.wmv;"
                                 + "*.asf;*.mkv;*.flv;*.f4v;*.dvr;*.dvr-ms;*.wtv;*.ogv;*.ogm;*.3gp;*.rm;*.rmvb;|All Files|*.*";

        //holds list of subtitle files in same directory as video file
        List<string> subfilesList = new List<string>();
        List<string> subExtensions = new List<string>();
        MovieInfo Movie; //Movie Object

        public MovieForm()
        {
            //Initiallizes components of form
            InitializeComponent();

            posterNumberLabel.Text = "of 1"; //Sets label to 1 representing the number of poster images available to select from
            backdropNumberLabel.Text = "of 1"; //Sets the label to 1 representing the number of backdrop images to select from
            formatPicturebox.Visible = true; //displays the Universal image since it is the default for that control
            formatComboBox.SelectedIndex = 2; //displays Universal in the dropdown as being the default for that control
        }
        /// <summary>
        /// Calls the openVideoFile Method
        /// Also used for the ToolStrip menu "Open" control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void openVideoFileButton_Click(object sender, EventArgs e)
        {
            openVideoFile();
        }
        private void getInitialDirectory()
        {
            if (System.IO.File.Exists(configPath))
            {
                //parse default path from config file and set it to folder path
                using (StreamReader SR = new StreamReader(configPath))
                {
                    //Read configFileText to string
                    configFileText = SR.ReadToEnd();
                    defaultPathText = GeneralParser(configFileText, "<MovieCollectorDefaultPathStart>", "<MovieCollectorDefaultPathEnd>");
                    SR.Close();
                }
            }
        }
        private void WriteDefaultFilePath()
        {
            string oldText;
            string newText;

            try
            {
                using (StreamReader SR = new StreamReader(configPath))
                {
                    //Read configFileText to string
                    configFileText = SR.ReadToEnd();
                    defaultPathText = GeneralParser(configFileText, "<MovieCollectorDefaultPathStart>", "<MovieCollectorDefaultPathEnd>");
                    oldText = "<MovieCollectorDefaultPathStart>" + defaultPathText + "<MovieCollectorDefaultPathEnd>";
                    newText = "<MovieCollectorDefaultPathStart>" + parentPath + "<MovieCollectorDefaultPathEnd>";
                    configFileText = configFileText.Replace(oldText, newText);
                    SR.Close();
                }

                using (StreamWriter sw = File.CreateText(configPath))
                {
                    sw.WriteLine(configFileText);
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                CustomMessageBox.Show(e.ToString(), 300, 300);
            }
        }
        private void openVideoFile()
        {
            getInitialDirectory();

            OpenFileDialog OFD = new OpenFileDialog();

            if (Directory.Exists(defaultPathText))
            {
                OFD.InitialDirectory = defaultPathText;
            }
            try //Try block incase of file error
            {
                OFD.Filter = videoTypeFilter;
                if (OFD.ShowDialog() == DialogResult.OK) //If user clicks ok...
                {
                    parentPath = (Directory.GetParent(OFD.FileName)).ToString();
                    parentPath += "\\";
                    videoPath = OFD.FileName;
                    videoPathTextBox.Text = videoPath;

                    char[] delim = { '.' }; //using '.' as the delimiter splits the extension from the filepath
                    string[] Tokens = OFD.FileName.Split(delim);

                    //Token[Tokens.Count()-1] - File extension without the . 
                    videoExtension = "." + Tokens[Tokens.Count() - 1]; //Last Token in the array - the file extension
                    videoFileName = videoPath.Replace(parentPath, "");
                    videoFileName = videoFileName.Replace(Tokens[Tokens.Count() - 1], "");
                    WriteDefaultFilePath();
                }
            }
            catch //displays messagebox if error occurs while opening file
            {
                CustomMessageBox.Show("Failed to retrieve file information", 125, 236);
            }
        }
        private void findSubtitlesInFile()
        {
            subfilesList.Clear();
            try //Generate a list of all subtitle files in folder
            {
                subfilesList = Directory
                        .GetFiles(parentPath, "*.*")
                        .Where(file => file.ToLower().EndsWith(".srt") ||
                            file.ToLower().EndsWith(".sub") ||
                            file.ToLower().EndsWith(".idx") ||
                            file.ToLower().EndsWith(".ssa") ||
                            file.ToLower().EndsWith(".ass") ||
                            file.ToLower().EndsWith(".smi") ||
                            file.ToLower().EndsWith(".vtt"))
                        .ToList();
            }
            catch (Exception e)
            {
                CustomMessageBox.Show("Error retrieveing subtitle file" + e.ToString(), 117, 212);
            }

            //Build list of extensions for subtitles.
            foreach (string s in subfilesList)
            {
                char[] delim = { '.' }; //using '.' as the delimiter splits the extension from the filepath
                string[] Tokens = s.Split(delim);
                //Token[Tokens.Count()-1] - File extension without the . 
                subExtensions.Add("." + Tokens[Tokens.Count() - 1]); //Last Token in the array - the file extension
            }

            for (int i = 0; i < subfilesList.Count(); i++) //filter out all but the filename and extension of each subtitle file
            {
                subfilesList[i] = subfilesList[i].Replace(parentPath, "");
            }

            //Switch to accomodate format selection
            switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
            {
                case 0: //Plex
                    /*[MovieTitle].[2/3 Char language Code].[ext]
                        */

                    //check for subtitles for current movie
                    for (int i = 0; i < subfilesList.Count(); i++)
                    {

                        notificationLabel.Text = "Searching for subtitle matches ";

                        if (subfilesList[i].Contains(videoFileName)) //If the subtitle file contains the movie filename
                        {
                            //remove movie title from subtitle file
                            subfilesList[i].Replace(videoFileName, "");

                            for (int b = 0; b < (subLanguageCodes.Length / 3); b++) //Check for language codes in the filename
                            {
                                if (subfilesList[i].Contains("." + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 0] + ")") ||

                                    subfilesList[i].Contains("." + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 1] + ")") ||

                                    subfilesList[i].Contains("." + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 2] + ")"))
                                {
                                    try //if a match is found move the file to the new folder named movietitle.languagecode.ext
                                    {
                                        notificationLabel.Text = "Match Discovered: " + subfilesList[i].ToString();

                                        //check to see if subfile contains forced and a language code
                                        if (subfilesList[i].ToLower().Contains("forced"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".FORCED" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".FORCED" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".FORCED" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].ToLower().Contains("sdh")) //subtitles for the deaf and hard of hearing
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(SDH)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(SDH)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(SDH)" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].ToLower().Contains("cc"))//CC - Closed Captioning
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(CC)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(CC)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(CC)" + subExtensions[i]);
                                            }
                                        }

                                        else
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        CustomMessageBox.Show("Error moving file " + parentPath.ToString() + subfilesList[i].ToString(), 200, 380);
                                    }
                                }
                            }
                            if (File.Exists(parentPath + subfilesList[i])) //If language code wasn't found get file anyway and name it as a standard movietitle.srt
                            {
                                try
                                {
                                    if (subfilesList[i].Contains("forced") ||
                                        subfilesList[i].Contains("Forced") ||
                                        subfilesList[i].Contains("FORCED"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-2" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-3" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                            subfilesList[i].Contains("sdh") ||
                                            subfilesList[i].Contains("Sdh"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-2" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-3" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("CC") || //CC-Closed Captioning
                                            subfilesList[i].Contains("cc"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-2" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-3" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(2)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(3)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(3)" + subExtensions[i]); //move file to new folder
                                        }

                                    }
                                }
                                catch
                                {
                                    CustomMessageBox.Show("Error moving file " + parentPath.ToString() + subfilesList[i].ToString(), 200, 380);
                                }
                            }
                        }

                    }
                    break;

                case 1: //Synology
                    /*Subtitles for Synology should be formatted like:
                        * [Movie Title].[SubtitleName].[ext]
                        * Note anything in the [SubitleName] section will display
                        * so if you inpute Avatar(2010).EngForced.srt or Avatar(2010).Eng_Forced.srt it will display correctly
                        * but if you input Avatar(2010).Eng.Forced.srt or Avatar(2010).Eng Forced.srt  it will not display the Eng part in the list.
                        */

                    //check for subtitles for current movie
                    for (int i = 0; i < subfilesList.Count(); i++)
                    {

                        notificationLabel.Text = "Searching for subtitle matches ";

                        if (subfilesList[i].Contains(videoFileName)) //If the subtitle file contains the movie filename
                        {
                            //remove movie title from subtitle file
                            subfilesList[i].Replace(videoFileName, "");

                            for (int b = 0; b < (subLanguageCodes.Length / 3); b++) //Check for language codes in the filename
                            {
                                if (subfilesList[i].Contains("." + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 0] + ")") ||

                                    subfilesList[i].Contains("." + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 1] + ")") ||

                                    subfilesList[i].Contains("." + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 2] + ")"))
                                {
                                    try //if a match is found move the file to the new folder named movietitle.languagecode.ext
                                    {
                                        notificationLabel.Text = "Match Discovered: " + subfilesList[i].ToString();

                                        //check to see if subfile contains forced and a language code
                                        if (subfilesList[i].Contains("forced") ||
                                            subfilesList[i].Contains("Forced") ||
                                            subfilesList[i].Contains("FORCED"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(FORCED)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(FORCED)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(FORCED)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(FORCED)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(FORCED)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(FORCED)" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                                subfilesList[i].Contains("sdh") ||
                                                subfilesList[i].Contains("Sdh"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(SDH)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(SDH)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(SDH)" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].Contains("CC") || //CC - Closed Captioning
                                                subfilesList[i].Contains("cc"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(CC)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(CC)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(CC)" + subExtensions[i]);
                                            }
                                        }

                                        else
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        CustomMessageBox.Show("Error moving file " + parentPath.ToString() + subfilesList[i].ToString(), 200, 380);
                                    }
                                }
                            }
                            if (File.Exists(parentPath + subfilesList[i])) //If language code wasn't found get file anyway and name it as a standard movietitle.srt
                            {
                                try
                                {
                                    if (subfilesList[i].Contains("forced") ||
                                        subfilesList[i].Contains("Forced") ||
                                        subfilesList[i].Contains("FORCED"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(FORCED)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(FORCED)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(FORCED-2)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(FORCED-2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(FORCED-3)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(FORCED-3)" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                            subfilesList[i].Contains("sdh") ||
                                            subfilesList[i].Contains("Sdh"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(SDH)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(SDH)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(SDH-2)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(SDH-2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(SDH-3)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(SDH-3)" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("CC") || //CC-Closed Captioning
                                            subfilesList[i].Contains("cc"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(CC)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(CC)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(CC-2)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(CC-2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(CC-3)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(CC-3)" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(2)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(3)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG(3)" + subExtensions[i]); //move file to new folder
                                        }

                                    }
                                }
                                catch
                                {
                                    CustomMessageBox.Show("Error moving file " + parentPath.ToString() + subfilesList[i].ToString(), 200, 380);
                                }
                            }
                        }

                    }
                    break;

                case 2: //Universal
                    /*
                        The Universal setting is not complete. I've set it to work the 
                        * same as the kodi setting
                        */
                    //check for subtitles for current movie
                    for (int i = 0; i < subfilesList.Count(); i++)
                    {

                        notificationLabel.Text = "Searching for subtitle matches ";

                        if (subfilesList[i].Contains(videoFileName)) //If the subtitle file contains the movie filename
                        {
                            //remove movie title from subtitle file
                            subfilesList[i].Replace(videoFileName, "");

                            for (int b = 0; b < (subLanguageCodes.Length / 3); b++) //Check for language codes in the filename
                            {
                                if (subfilesList[i].Contains("." + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 0] + ")") ||

                                    subfilesList[i].Contains("." + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 1] + ")") ||

                                    subfilesList[i].Contains("." + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 2] + ")"))
                                {
                                    try //if a match is found move the file to the new folder named movietitle.languagecode.ext
                                    {
                                        notificationLabel.Text = "Match Discovered: " + subfilesList[i].ToString();

                                        //check to see if subfile contains forced and a language code
                                        if (subfilesList[i].Contains("forced") ||
                                            subfilesList[i].Contains("Forced") ||
                                            subfilesList[i].Contains("FORCED"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".FORCED" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".FORCED" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".FORCED" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                                subfilesList[i].Contains("sdh") ||
                                                subfilesList[i].Contains("Sdh"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(SDH)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(SDH)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(SDH)" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].Contains("CC") || //CC - Closed Captioning
                                                subfilesList[i].Contains("cc"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(CC)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(CC)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(CC)" + subExtensions[i]);
                                            }
                                        }

                                        else
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        CustomMessageBox.Show("Error moving file " + parentPath.ToString() + subfilesList[i].ToString(), 200, 380);
                                    }
                                }
                            }
                            if (File.Exists(parentPath + subfilesList[i])) //If language code wasn't found get file anyway and name it as a standard movietitle.srt
                            {
                                try
                                {
                                    if (subfilesList[i].Contains("forced") ||
                                        subfilesList[i].Contains("Forced") ||
                                        subfilesList[i].Contains("FORCED"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-2" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-3" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                            subfilesList[i].Contains("sdh") ||
                                            subfilesList[i].Contains("Sdh"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-2" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-3" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("CC") || //CC-Closed Captioning
                                            subfilesList[i].Contains("cc"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-2" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-3" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(2)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(3)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(3)" + subExtensions[i]); //move file to new folder
                                        }

                                    }
                                }
                                catch
                                {
                                    CustomMessageBox.Show("Error moving file " + parentPath.ToString() + subfilesList[i].ToString(), 200, 380);
                                }
                            }
                        }

                    }
                    break;

                case 3: //kodi
                    /*[MovieTitle].[LanguageCode].[ext} or 
                        *[MovieTitle].[LanguageCode].[Forced or CC or SDH].[ext]
                    */
                    //check for subtitles for current movie
                    for (int i = 0; i < subfilesList.Count(); i++)
                    {

                        notificationLabel.Text = "Searching for subtitle matches ";

                        if (subfilesList[i].Contains(videoFileName)) //If the subtitle file contains the movie filename
                        {
                            //remove movie title from subtitle file
                            subfilesList[i].Replace(videoFileName, "");

                            for (int b = 0; b < (subLanguageCodes.Length / 3); b++) //Check for language codes in the filename
                            {
                                if (subfilesList[i].Contains("." + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 0]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 0] + ")") ||

                                    subfilesList[i].Contains("." + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 1]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 1] + ")") ||

                                    subfilesList[i].Contains("." + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("_" + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains(" " + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("-" + subLanguageCodes[b, 2]) ||
                                    subfilesList[i].Contains("(" + subLanguageCodes[b, 2] + ")"))
                                {
                                    try //if a match is found move the file to the new folder named movietitle.languagecode.ext
                                    {
                                        notificationLabel.Text = "Match Discovered: " + subfilesList[i].ToString();

                                        //check to see if subfile contains forced and a language code
                                        if (subfilesList[i].Contains("forced") ||
                                            subfilesList[i].Contains("Forced") ||
                                            subfilesList[i].Contains("FORCED"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".FORCED" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".FORCED" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".FORCED" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                                subfilesList[i].Contains("sdh") ||
                                                subfilesList[i].Contains("Sdh"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(SDH)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(SDH)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(SDH)" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].Contains("CC") || //CC - Closed Captioning
                                                subfilesList[i].Contains("cc"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(CC)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(CC)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(CC)" + subExtensions[i]);
                                            }
                                        }

                                        else
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        CustomMessageBox.Show("Error moving file " + parentPath.ToString() + subfilesList[i].ToString(), 200, 380);
                                    }
                                }
                            }
                            if (File.Exists(parentPath + subfilesList[i])) //If language code wasn't found get file anyway and name it as a standard movietitle.srt
                            {
                                try
                                {
                                    if (subfilesList[i].Contains("forced") ||
                                        subfilesList[i].Contains("Forced") ||
                                        subfilesList[i].Contains("FORCED"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-2" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-3" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.FORCED-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                            subfilesList[i].Contains("sdh") ||
                                            subfilesList[i].Contains("Sdh"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-2" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-3" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.SDH-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("CC") || //CC-Closed Captioning
                                            subfilesList[i].Contains("cc"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-2" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-3" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.CC-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(2)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(3)" + subExtensions[i]))
                                        {
                                            File.Move(parentPath + subfilesList[i], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".ENG.(3)" + subExtensions[i]); //move file to new folder
                                        }

                                    }
                                }
                                catch
                                {
                                    CustomMessageBox.Show("Error moving file " + parentPath.ToString() + subfilesList[i].ToString(), 200, 380);
                                }
                            }
                        }

                    }
                    break;
            }
        }
        private void GetHTMLButton_Click(object sender, EventArgs e)
        {
            notificationLabel.Visible = true;
            nLabelUpdate("Performing Search");

            if (!string.IsNullOrEmpty(imdbIDTextBox.Text))
            {
                //Check for IMDBID
                if (imdbIDTextBox.Text.Length < 10 & imdbIDTextBox.Text.Length > 8 & imdbIDTextBox.Text.Contains("tt"))
                {
                    nLabelUpdate("Building Movie Objects");
                    Movie = new MovieInfo(imdbIDTextBox.Text, APIKey); //Create Movie Object using IMDBID number from textbox

                }
                else
                {
                    nLabelUpdate("Performing Search");
                    MovieSelection M = new MovieSelection(APIKey, imdbIDTextBox.Text);

                    if (M.movieList.Count > 1)
                    {
                        M.ShowDialog();
                        if (M.DialogResult == DialogResult.OK)
                        {
                            nLabelUpdate("Building Movie Objects");
                            Movie = new MovieInfo(M.selectedID, APIKey); //Create Movie Object using IMDBID number from textbox
                        }
                        if (M.DialogResult == DialogResult.Cancel) { clearAll(); notificationLabel.Visible = false; return; }
                        if (M.DialogResult == DialogResult.Abort) { clearAll(); notificationLabel.Visible = false; return; }
                    }
                    else if (M.movieList.Count == 1)
                    {
                        nLabelUpdate("Building Movie Objects");
                        Movie = new MovieInfo(M.movieList[0]["ID"], APIKey); //Create Movie Object using IMDBID number from textbox
                    }
                    else
                    {

                        CustomMessageBox.Show(imdbIDTextBox.Text + " is not a valid search term or IMDB ID", 189, 287, "Invalid IMDB ID Number");
                        clearAll();
                        return;
                    }
                }
            }
            else
            {
                CustomMessageBox.Show(imdbIDTextBox.Text + " is not a valid IMDB movie ID #", 189, 287, "Invalid IMDB ID Number");
                clearAll();
                return;
            }

            string videoFilePath = videoPathTextBox.Text; //Saves video file path
            clearAll(); //Clears form contents
            videoPathTextBox.Text = videoFilePath; //fills in the video file path text box with previous info


            /*Call public methods of the movie object one by in order to show progress bar
            Could be all called with the constructor but since it takes a while this will 
            make it so we can use the progress bar. The order doesn't matter as these
            methods for the object don't need to be called in order*/

            nLabelUpdate("Gathering Film Credits");

            Movie.getCredits();

            nLabelUpdate("Retrieving Film MPAA Rating");

            Movie.getRating(); //Returns the MPAA Rating 

            nLabelUpdate("Gathering Plot, Title, Genre, etc");

            Movie.getBasicInfo(); //Returns basic film info
            imdbIDTextBox.Text = Movie.staticProperties["IMDB_ID"]; //adds the IMDBID back to the form

            nLabelUpdate("Retrieving Alternate US Titles");

            Movie.GetUSTitles(); //Builds list of US versions of the film title

            nLabelUpdate("Gathering US and Non-Region Coded Film Images");
            Movie.getFilmImages(); //Gathers film image URLs

            nLabelUpdate("Adding Alternate Titles");

            //Loop to add movie titles from movie object to the combo box
            for (int i = 0; i < Movie.listProperties["USTitles"].Count; i++)
            {
                titleComboBox.Items.Add(Movie.listProperties["USTitles"][i]);
            }

            if (!titleComboBox.Items.Contains(Movie.staticProperties["Title"]) && !string.IsNullOrEmpty(Movie.staticProperties["Title"]))
            {
                titleComboBox.Items.Add(Movie.staticProperties["Title"]);
            }

            if (!titleComboBox.Items.Contains(Movie.staticProperties["OriginalTitle"]) && !string.IsNullOrEmpty(Movie.staticProperties["OriginalTitle"]))
            {
                titleComboBox.Items.Add(Movie.staticProperties["OriginalTitle"]);
            }
            setTitleComboBoxWidth();

            nLabelUpdate("Filling in Form Data");
            titleComboBox.Text = Movie.staticProperties["Title"]; //uses the first title in the list as the default title;
            setTextBox.Text = Movie.staticProperties["Collection"]; //uses the Collection information from the movie object to fill in the collection information
            yearTextBox.Text = Movie.staticProperties["ReleaseYear"]; //uses the release year information from the movie object to fill in the year
            runTimeTextBox.Text = Movie.staticProperties["RunTime"];
            mpaaTextBox.Text = Movie.staticProperties["MPAA_Rating"];
            genresTextBox.Text = Movie.staticProperties["Genres"];

            if (string.IsNullOrEmpty(Movie.staticProperties["Tag_Line"])) { plotTextBox.Text = Movie.staticProperties["Plot"]; }
            else { plotTextBox.Text = Movie.staticProperties["Tag_Line"] + "\r\r" + Movie.staticProperties["Plot"]; }


            nLabelUpdate("Setting up images");

            posterNumericUpDown.Maximum = Movie.listProperties["Posters"].Count() + 1;
            posterNumericUpDown.Minimum = 0;
            backdropNumericUpDown.Maximum = Movie.listProperties["Backdrops"].Count() + 1;
            backdropNumericUpDown.Minimum = 0;

            posterNumberLabel.Text = "of " + Movie.listProperties["Posters"].Count().ToString();
            backdropNumberLabel.Text = "of " + Movie.listProperties["Backdrops"].Count().ToString();
            posterNumericUpDown.Value = 1;
            backdropNumericUpDown.Value = 1;

            backDropPictureBox.ImageLocation = Movie.staticProperties["BackDropPath"];
            moviePosterPictureBox.ImageLocation = Movie.staticProperties["PosterPath"];

            nLabelUpdate("Gathering Film Credits");
            notificationLabel.Visible = false;

            //Determine if errors were encountered

            if (Movie.listProperties["Errors"].Count > 0)
            {
                StringBuilder Errors = new StringBuilder();
                foreach (string s in Movie.listProperties["Errors"])
                {
                    if (string.IsNullOrEmpty(Errors.ToString())) { Errors.Append(s); }
                    else { Errors.Append("\r\r" + s); }
                }
                clearAll();
                CustomMessageBox.Show(Errors.ToString(), 300, 400);
            }

        }
        private void createFilesButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor; //sets cursor to waitcursor
            notificationLabel.Visible = true;
            notificationLabel.Text = "Start";
            notificationLabel.Invalidate();
            notificationLabel.Update();


            if (!string.IsNullOrEmpty(videoPathTextBox.Text) && (!string.IsNullOrEmpty(titleComboBox.Text)))
            {
                notificationLabel.Text = "Checking Format Selection";
                notificationLabel.Invalidate();
                notificationLabel.Update();

                switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                {
                    case 0: //Plex
                        Movie.staticProperties["FormattedTitle"] = validTitle(titleComboBox.Text + " (" + yearTextBox.Text + ")");
                        break;
                    case 1: //Synology
                        Movie.staticProperties["FormattedTitle"] = validTitle(titleComboBox.Text + " (" + yearTextBox.Text + ")");
                        break;
                    case 2: //Universal
                        Movie.staticProperties["FormattedTitle"] = validTitle(titleComboBox.Text + " (" + yearTextBox.Text + ")");
                        break;
                    case 3: //kodi
                        Movie.staticProperties["FormattedTitle"] = validTitle(titleComboBox.Text + " (" + yearTextBox.Text + ")");
                        break;
                }

                notificationLabel.Text = "Setting Video Path";
                notificationLabel.Invalidate();
                notificationLabel.Update();

                videoPath = parentPath + Movie.staticProperties["FormattedTitle"] + videoExtension;
                newDirectoryName = parentPath + Movie.staticProperties["FormattedTitle"];

                notificationLabel.Text = "Checking for existing directory";
                notificationLabel.Invalidate();
                notificationLabel.Update();

                //add code to check for directory prior to doing anything
                if (!Directory.Exists(newDirectoryName))
                {
                    notificationLabel.Text = "Creating Directory";
                    notificationLabel.Invalidate();
                    notificationLabel.Update();

                    DirectoryInfo di = Directory.CreateDirectory(newDirectoryName);

                    notificationLabel.Text = "Checking for existing video file";
                    notificationLabel.Invalidate();
                    notificationLabel.Update();

                    if (File.Exists(videoPathTextBox.Text))
                    {
                        notificationLabel.Text = "Renaming video file";
                        notificationLabel.Invalidate();
                        notificationLabel.Update();

                        string moveToPath = parentPath + Movie.staticProperties["FormattedTitle"] + videoExtension; //uses validTitle method to remove illegal characters
                        File.Move(videoPathTextBox.Text, moveToPath); //renames file if in the same directory
                    }
                    else
                    {
                        CustomMessageBox.Show("File: " + videoPathTextBox.Text + "\n Does not exist.", 200, 380);
                    }


                    notificationLabel.Text = "Checking for existing file in folder";
                    notificationLabel.Invalidate();
                    notificationLabel.Update();

                    if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + videoExtension)) //Check that file isn't already in folder
                    {
                        notificationLabel.Text = "Moving file to folder";
                        notificationLabel.Invalidate();
                        notificationLabel.Update();

                        File.Move(videoPath, newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + videoExtension); //move file to new folder
                    }
                    else
                    {
                        CustomMessageBox.Show("File: " + newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + videoExtension + ".\n Already exists!", 200, 380);
                    }

                    notificationLabel.Text = "Checking for subtitle files";
                    notificationLabel.Invalidate();
                    notificationLabel.Update();

                    findSubtitlesInFile(); ;
                    //subTitleNamer(formatComboBox.SelectedIndex);

                    notificationLabel.Text = "Formatting artwork URLs";
                    notificationLabel.Invalidate();
                    notificationLabel.Update();

                    //add images
                    if (Movie.listProperties["Posters"].Count > 0)
                    {
                        Movie.staticProperties["PosterPath"] = Movie.listProperties["Posters"][(int)posterNumericUpDown.Value - 1];
                        Movie.staticProperties["PosterPath"] = Movie.staticProperties["PosterPath"].Replace("/w154/", "/original/");
                    }

                    if (Movie.listProperties["Backdrops"].Count > 0)
                    {
                        Movie.staticProperties["BackDropPath"] = Movie.listProperties["Backdrops"][(int)backdropNumericUpDown.Value - 1];
                        Movie.staticProperties["BackDropPath"] = Movie.staticProperties["BackDropPath"].Replace("/w300/", "/original/");
                    }

                    if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".jpg")
                        & !File.Exists(newDirectoryName + "\\" + "poster.jpg")
                        & !File.Exists(newDirectoryName + "\\" + "Folder.jpg")
                        & !string.IsNullOrEmpty(Movie.staticProperties["PosterPath"]))
                    {
                        notificationLabel.Text = "Downloading Poster";
                        notificationLabel.Invalidate();
                        notificationLabel.Update();


                        using (WebClient wc = new WebClient())
                        {

                            try //Attempt to download original file
                            {
                                wc.Encoding = System.Text.Encoding.UTF8; //Sets Encoding for text output

                                switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                {
                                    case 0: //Plex
                                        wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".jpg");
                                        break;
                                    case 1: //Synology
                                        wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "Folder.jpg");
                                        break;
                                    case 2: //Universal creates two identical poster files with different names to have max compatibility
                                        wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");
                                        File.Copy(newDirectoryName + "\\" + "poster.jpg", newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".jpg");
                                        break;
                                    case 3: //kodi
                                        wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");
                                        break;
                                }

                            }
                            catch //If original file doesn't exist try w1000
                            {
                                Movie.staticProperties["PosterPath"] = Movie.staticProperties["PosterPath"].Replace("/original/", "/w1000/");
                                try //Attempt to download w1000 size
                                {
                                    switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                    {
                                        case 0: //Plex
                                            wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".jpg");
                                            break;
                                        case 1: //Synology
                                            wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "Folder.jpg");
                                            break;
                                        case 2: //Universal creates two identical poster files with different names to have max compatibility
                                            wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");
                                            File.Copy(newDirectoryName + "\\" + "poster.jpg", newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".jpg");
                                            break;
                                        case 3: //kodi
                                            wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");
                                            break;
                                    }
                                }
                                catch //If w1000 doesn't exist try w500
                                {
                                    Movie.staticProperties["PosterPath"] = Movie.staticProperties["PosterPath"].Replace("/w1000/", "/w500/");
                                    try //Attempt to download w500 size
                                    {
                                        switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                        {
                                            case 0: //Plex
                                                wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".jpg");
                                                break;
                                            case 1: //Synology
                                                wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "Folder.jpg");
                                                break;
                                            case 2: //Universal creates two identical poster files with different names to have max compatibility
                                                wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");
                                                File.Copy(newDirectoryName + "\\" + "poster.jpg", newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".jpg");
                                                break;
                                            case 3: //kodi
                                                wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");
                                                break;
                                        }
                                    }
                                    catch //if w500 doesn't exist try w300
                                    {
                                        Movie.staticProperties["PosterPath"] = Movie.staticProperties["PosterPath"].Replace("/w500/", "/w300/");
                                        try //Attempt to download w300 size
                                        {
                                            switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                            {
                                                case 0: //Plex
                                                    wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".jpg");
                                                    break;
                                                case 1: //Synology
                                                    wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "Folder.jpg");
                                                    break;
                                                case 2: //Universal creates two identical poster files with different names to have max compatibility
                                                    wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");
                                                    File.Copy(newDirectoryName + "\\" + "poster.jpg", newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".jpg");
                                                    break;
                                                case 3: //kodi
                                                    wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");
                                                    break;
                                            }
                                        }
                                        catch //if w300 doesn't exist
                                        {
                                            CustomMessageBox.Show("Failed to download Poster Image", 122, 258);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    if (!File.Exists(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "-fanart.jpg")
                        & !File.Exists(newDirectoryName + "\\" + "fanart.jpg")
                        & !string.IsNullOrEmpty(Movie.staticProperties["BackDropPath"]))
                    {
                        notificationLabel.Text = "Downloading Backdrop/Fanart";
                        notificationLabel.Invalidate();
                        notificationLabel.Update();

                        using (WebClient wc = new WebClient())
                        {
                            try //Attempt to download original file
                            {
                                wc.Encoding = System.Text.Encoding.UTF8; //Sets Encoding for text output
                                switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                {
                                    case 0: //Plex
                                        wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "-fanart.jpg");
                                        break;
                                    case 1: //Synology
                                        break;
                                    case 2: //Universal
                                        wc.DownloadFile(Movie.staticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                        break;
                                    case 3: //kodi
                                        wc.DownloadFile(Movie.staticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                        break;
                                }

                            }
                            catch
                            {

                                Movie.staticProperties["BackDropPath"].Replace("/original/", "/w1000/");
                                try //Attempt to download w1000 file
                                {
                                    switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                    {
                                        case 0: //Plex
                                            wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "-fanart.jpg");
                                            break;
                                        case 1: //Synology
                                            break;
                                        case 2: //Universal
                                            wc.DownloadFile(Movie.staticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                            break;
                                        case 3: //kodi
                                            wc.DownloadFile(Movie.staticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                            break;
                                    }
                                }
                                catch
                                {
                                    Movie.staticProperties["BackDropPath"].Replace("/w1000/", "/w500/");
                                    try //Attempt to download w500 file
                                    {
                                        switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                        {
                                            case 0: //Plex
                                                wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "-fanart.jpg");
                                                break;
                                            case 1: //Synology
                                                break;
                                            case 2: //Universal
                                                wc.DownloadFile(Movie.staticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                                break;
                                            case 3: //kodi
                                                wc.DownloadFile(Movie.staticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                                break;
                                        }
                                    }
                                    catch
                                    {
                                        Movie.staticProperties["BackDropPath"].Replace("/w500/", "/w300/");
                                        try //Attempt to download w300 file
                                        {
                                            switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                            {
                                                case 0: //Plex
                                                    wc.DownloadFile(Movie.staticProperties["PosterPath"], newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + "-fanart.jpg");
                                                    break;
                                                case 1: //Synology
                                                    break;
                                                case 2: //Universal
                                                    wc.DownloadFile(Movie.staticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                                    break;
                                                case 3: //kodi
                                                    wc.DownloadFile(Movie.staticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                                    break;
                                            }
                                        }
                                        catch
                                        {
                                            CustomMessageBox.Show("Failed to download fan art", 119, 202);
                                        }

                                    }
                                }
                            }
                        }
                    }
                    try
                    {
                        notificationLabel.Text = "Creating NFO Metadata file";
                        notificationLabel.Invalidate();
                        notificationLabel.Update();

                        switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                        {
                            case 0: //Plex
                                break;
                            case 1: //Synology
                                break;
                            case 2: //Universal
                                using (StreamWriter sw = File.CreateText(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".nfo"))
                                {
                                    sw.Write(CreateNFO());
                                }
                                break;
                            case 3: //kodi
                                using (StreamWriter sw = File.CreateText(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".nfo"))
                                {
                                    sw.Write(CreateNFO());
                                }
                                break;
                            default:
                                using (StreamWriter sw = File.CreateText(newDirectoryName + "\\" + Movie.staticProperties["FormattedTitle"] + ".nfo"))
                                {
                                    sw.Write(CreateNFO());
                                }
                                break;
                        }


                    }
                    catch
                    {
                        CustomMessageBox.Show("Faile to create NFO file", 119, 202);
                    }
                    clearAll();

                }
                else
                {
                    CustomMessageBox.Show("Directory: " + newDirectoryName + "\n Already exists", 200, 380);
                }
            }
            else if (string.IsNullOrEmpty(videoPathTextBox.Text))
            {
                CustomMessageBox.Show("No Video Path Present", 133, 270);
            }
            else if (string.IsNullOrEmpty(titleComboBox.Text))
            {
                CustomMessageBox.Show("Video information not yet gathered", 133, 270);
            }

            Cursor.Current = Cursors.WaitCursor; //sets cursor to waitcursor
            notificationLabel.Text = "Finished";
            notificationLabel.Invalidate();
            notificationLabel.Update();

            notificationLabel.Visible = false;

        }
        private void clearAll()
        {
            videoPathTextBox.Clear();
            imdbIDTextBox.Clear();
            yearTextBox.Clear();
            setTextBox.Clear();
            mpaaTextBox.Clear();
            runTimeTextBox.Clear();
            genresTextBox.Clear();
            plotTextBox.Clear();
            backDropPictureBox.Image = MovieDataCollector.Properties.Resources.highlightreel;
            moviePosterPictureBox.Image = MovieDataCollector.Properties.Resources.filmreel;
            subfilesList.Clear();
            backdropNumberLabel.Text = "";
            posterNumberLabel.Text = " of 1";
            backdropNumberLabel.Text = " of 1";
            titleComboBox.Items.Clear();
            titleComboBox.Text = "";

        }
        private string validTitle(string unformattedTitle)
        {
            string charsToRemove = @"\/:*?<>|"; //\ / : * ? " < > |
            unformattedTitle = unformattedTitle.Replace("\"", "");
            string pattern = string.Format("[{0}]", Regex.Escape(charsToRemove));

            return Regex.Replace(unformattedTitle, pattern, "");
        }
        private void iMDBcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.imdb.com");
        }
        private void clearButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            clearAll();
        }
        private void posterNumericUpDown_ValueChanged(object sender, EventArgs e)
        {

            if (Movie == null || Movie.listProperties["Posters"].Count <= 0) { moviePosterPictureBox.Image = MovieDataCollector.Properties.Resources.filmreel; return; }

            else if (posterNumericUpDown.Value <= (Movie.listProperties["Posters"].Count) && posterNumericUpDown.Value >= 1)
            {
                moviePosterPictureBox.ImageLocation = Movie.listProperties["Posters"][(int)posterNumericUpDown.Value - 1];
            }
            else if (posterNumericUpDown.Value >= (Movie.listProperties["Posters"].Count))
            {
                posterNumericUpDown.Value = 1;
                moviePosterPictureBox.ImageLocation = Movie.listProperties["Posters"][(int)posterNumericUpDown.Value - 1];
            }
            else if (posterNumericUpDown.Value < 1)
            {
                posterNumericUpDown.Value = Movie.listProperties["Posters"].Count;
                moviePosterPictureBox.ImageLocation = Movie.listProperties["Posters"][(int)posterNumericUpDown.Value - 1];
            }
        }
        private void backdropNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Movie == null || Movie.listProperties["Backdrops"].Count <= 0) { backDropPictureBox.Image = MovieDataCollector.Properties.Resources.highlightreel; return; }

            else if (backdropNumericUpDown.Value <= (Movie.listProperties["Backdrops"].Count) && backdropNumericUpDown.Value >= 1)
            {
                backDropPictureBox.ImageLocation = Movie.listProperties["Backdrops"][(int)backdropNumericUpDown.Value - 1];
            }
            else if (backdropNumericUpDown.Value >= (Movie.listProperties["Backdrops"].Count))
            {
                backdropNumericUpDown.Value = 1;
                backDropPictureBox.ImageLocation = Movie.listProperties["Backdrops"][(int)backdropNumericUpDown.Value - 1];
            }
            else if (backdropNumericUpDown.Value < 1)
            {
                backdropNumericUpDown.Value = Movie.listProperties["Backdrops"].Count;
                backDropPictureBox.ImageLocation = Movie.listProperties["Backdrops"][(int)backdropNumericUpDown.Value - 1];
            }
        }
        private string CreateNFO()
        {
            string space = "    ";
            string doubleSpace = space + space;
            string tripleSpace = space + space + space;
            string quadrupleSpace = space + space + space + space;
            string nfoOutPut = "";

            string postersNFOString = "";
            string backdropsNFOString = "";

            string actorsNFOString = "";

            for (int i = 0; i < Movie.listProperties["Posters"].Count; i++)
            {
                postersNFOString += space + "<thumb aspect=\"poster\" preview=\"" + Movie.listProperties["Posters"][i].Replace("w154", "w500") + "\">" + Movie.listProperties["Posters"][i].Replace("w154", "original") + "</thumb>";
            }

            for (int i = 0; i < Movie.listProperties["Backdrops"].Count; i++)
            {
                backdropsNFOString += doubleSpace + "<thumb preview=\"" + Movie.listProperties["Backdrops"][i].Replace("w300", "w780") + "\">" + Movie.listProperties["Backdrops"][i].Replace("w300", "original") + "</thumb>";

            }

            for (int i = 0; i < Movie.ActorNames.Count; i++)
            {
                actorsNFOString += space + "<actor>" + doubleSpace + "<name>" + Movie.ActorNames[i] + "</name>"
                    + doubleSpace + "<role>" + Movie.ActorRoles[i] + "</role>"
                    + doubleSpace + "<thumb>" + Movie.ActorImages[i] + "</thumb>"
                    + space + "</actor>";
            }

            for (int i = 0; i < Movie.GenreList.Count; i++)
            {
                genresNFOString += space + "<genre>" + Movie.GenreList[i] + "</genre>";
            }

            for (int i = 0; i < Movie.StudioList.Count; i++)
            {
                studiosNFOString += space + "<studio>" + Movie.StudioList[i] + "</studio>";
            }

            //2014-04-03 17:15:33
            string datetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");



            nfoOutPut =
              "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>"
            + "<movie>"
            + space + "<title>" + titleComboBox.Text + "</title>"
            + space + "<originaltitle>" + Movie.staticProperties["OriginalTitle"] + "</originaltitle>"
            + space + "<rating>" + Movie.staticProperties["VoteAverage"] + "</rating>"
            + space + "<epbookmark>0.000000</epbookmark>" //leave at zeroes
            + space + "<year>" + yearTextBox.Text + "</year>"
            + space + "<top250>0</top250>" //leave at zero
            + space + "<votes>" + Movie.staticProperties["VoteCount"] + "</votes>"
            + space + "<outline></outline>" //leave empty
            + space + "<plot>" + Movie.staticProperties["Plot"] + "</plot>"
            + space + "<Tag_Line>" + Movie.staticProperties["Tag_Line"] + "</Tag_Line>"
            + space + "<RunTime>" + runTimeTextBox.Text + "</RunTime>"
            //Add loop for movie posters
            + postersNFOString
            + space + "<fanart>"
            //Add loop for movie backdrops
            + backdropsNFOString
            + space + "</fanart>"
            + space + "<mpaa>" + mpaaTextBox.Text + "</mpaa>"
            + space + "<playcount>0</playcount>" //leave at 0
            + space + "<lastplayed>1601-01-01</lastplayed>" //leave like this
            + space + "<id>" + Movie.staticProperties["IMDB_ID"] + "</id>"
            //add loop for genres
            + genresNFOString
            + space + "<country>" + Movie.staticProperties["ProductionCountry"] + "</country>" //leave as USA
            + space + "<set>" + setTextBox.Text + "</set>"
            + space + "<credits></credits>" //leave blank
            + space + "<director>" + Movie.staticProperties["Director"] + "</director>"
            + space + "<premiered>1601-01-01</premiered>" //leave
            + space + "<status></status>" //leave
            + space + "<code></code>" //leave
            + space + "<aired>1601-01-01</aired>" //leave
            + studiosNFOString //studios involed in producing the film
            + space + "<trailer></trailer>" //API is not working for this
            + space + "<fileinfo>"
            + doubleSpace + "<streamdetails>"
            + tripleSpace + "<video>"
            + quadrupleSpace + "<codec></codec>"
            + quadrupleSpace + "<aspect></aspect>"
            + quadrupleSpace + "<width></width>"
            + quadrupleSpace + "<height></height>"
            + quadrupleSpace + "<durationinseconds></durationinseconds>"
            + tripleSpace + "</video>"
            + tripleSpace + "<audio>"
            + quadrupleSpace + "<codec></codec>"
            + quadrupleSpace + "<language></language>"
            + quadrupleSpace + "<channels></channels>"
            + tripleSpace + "</audio>"
            + doubleSpace + "</streamdetails>"
            + space + "</fileinfo>"

            //add loop for actors
            + actorsNFOString
            + space + "<resume>"
            + doubleSpace + "<position>0.000000</position>" //leave
            + doubleSpace + "<total>0.000000</total>" //leve
            + space + "</resume>"
            //get current date
            + space + "<dateadded>" + datetime + "</dateadded>"
            + "</movie>";

            return nfoOutPut;

        }
        private void InvisibleCloseButton_Click(object sender, EventArgs e)
        {
            this.Close(); //Located behind the clear all button.
        }
        /// <summary>
        ///Changes picture based on selection in formatComboBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void formatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (formatComboBox.SelectedIndex)
            {
                case 0:
                    formatPicturebox.Image = MovieDataCollector.Properties.Resources.PlexLogo;
                    break;
                case 1:
                    formatPicturebox.Image = MovieDataCollector.Properties.Resources.Synology;
                    break;
                case 2:
                    formatPicturebox.Image = MovieDataCollector.Properties.Resources.Universal;
                    break;
                case 3:
                    formatPicturebox.Image = MovieDataCollector.Properties.Resources.kodi;
                    break;
                default:
                    formatPicturebox.Image = Properties.Resources.kodi;
                    break;
            }
        }
        private void nLabelUpdate(string Msg)
        {
            notificationLabel.Text = Msg;
            notificationLabel.Invalidate();
            notificationLabel.Update();
        }
        private void setTitleComboBoxWidth()
        {
            int max = 0;
            int temp = 0;

            if (titleComboBox.Items.Count > 0)
            {
                foreach (string s in titleComboBox.Items)
                {
                    temp = TextRenderer.MeasureText(s, titleComboBox.Font).Width;
                    if (temp > max) { max = temp; }
                }
                titleComboBox.DropDownWidth = max;
            }
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
    }
}
