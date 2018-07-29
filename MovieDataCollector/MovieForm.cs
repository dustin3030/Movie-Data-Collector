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

        Dictionary<string, string> NFODictionary = new Dictionary<string, string>();

        string videoExtension = ""; //holds the video files extension (.mp4, .mov, etc)
        string videoPath = ""; //Path of the video file
        string videoFileName = ""; //without extension
        string newDirectoryName = ""; //holds name of new directory video and other files get put into.

        //string configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector";
        //string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Config.txt";

        //Filter for opening video files
        const string videoTypeFilter = "Video Files|*.mpg;*.mpeg;*.vob;*.mod;*.ts;*.m2ts;*.mp4;*.m4v;*.mov;*.avi;*.divx;*.wmv;"
                                 + "*.asf;*.mkv;*.flv;*.f4v;*.dvr;*.dvr-ms;*.wtv;*.ogv;*.ogm;*.3gp;*.rm;*.rmvb;|All Files|*.*";

        //holds list of subtitle files in same directory as video file
        List<string> subfilesList = new List<string>();
        List<string> subExtensions = new List<string>();
        MovieInfo Movie; //Movie Object
        ConfigFile CF = new ConfigFile();
        public MovieForm()
        {
            //Initiallizes components of form
            InitializeComponent();

            posterNumberLabel.Text = "of 1"; //Sets label to 1 representing the number of poster images available to select from
            backdropNumberLabel.Text = "of 1"; //Sets the label to 1 representing the number of backdrop images to select from
            formatPicturebox.Visible = true; //displays the Universal image since it is the default for that control

            switch(CF.DefaultSettings["DefaultFormat"])
            {
                case "PLEX":
                    formatComboBox.SelectedIndex = 0;
                    break;
                case "KODI":
                    formatComboBox.SelectedIndex = 1;
                    break;
                case "Synology":
                    formatComboBox.SelectedIndex = 2;
                    break;
                default:
                    formatComboBox.SelectedIndex = 2;
                    break;
            }

        }
        /// <summary>
        /// Calls the openVideoFile Method
        /// Also used for the ToolStrip menu "Open" control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenVideoFileButton_Click(object sender, EventArgs e)
        {
            OpenVideoFile();
        }
        private void OpenVideoFile()
        {
            OpenFileDialog OFD = new OpenFileDialog();

            if (Directory.Exists(CF.DefaultSettings["MFPath"]))
            {
                OFD.InitialDirectory = CF.DefaultSettings["MFPath"];
            }
            try //Try block incase of file error
            {
                OFD.Filter = videoTypeFilter;
                if (OFD.ShowDialog() == DialogResult.OK) //If user clicks ok...
                {
                    CF.DefaultSettings["MFPath"] = (Directory.GetParent(OFD.FileName)).ToString();
                    CF.DefaultSettings["MFPath"] += "\\";
                    videoPath = OFD.FileName;
                    videoPathTextBox.Text = videoPath;

                    char[] delim = { '.' }; //using '.' as the delimiter splits the extension from the filepath
                    string[] Tokens = OFD.FileName.Split(delim);

                    //Token[Tokens.Count()-1] - File extension without the . 
                    videoExtension = "." + Tokens[Tokens.Count() - 1]; //Last Token in the array - the file extension
                    videoFileName = videoPath.Replace(CF.DefaultSettings["MFPath"], "");
                    videoFileName = videoFileName.Replace(Tokens[Tokens.Count() - 1], "");
                    CF.updateDefaults();
                }
            }
            catch //displays messagebox if error occurs while opening file
            {
                CustomMessageBox.Show("Failed to retrieve file information", 125, 236);
            }
        }
        private void FindSubtitlesInFile()
        {
            subfilesList.Clear();
            try //Generate a list of all subtitle files in folder
            {
                subfilesList = Directory
                        .GetFiles(CF.DefaultSettings["MFPath"], "*.*")
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

            //Build list of extensions for subtitle files found in the working directory.
            foreach (string s in subfilesList)
            {
                char[] delim = { '.' }; //using '.' as the delimiter splits the extension from the filepath
                string[] Tokens = s.Split(delim);
                //Token[Tokens.Count()-1] - File extension without the . 
                subExtensions.Add("." + Tokens[Tokens.Count() - 1]); //Last Token in the array - the file extension
            }

            for (int i = 0; i < subfilesList.Count(); i++) //filter out all but the filename and extension of each subtitle file
            {
                subfilesList[i] = subfilesList[i].Replace(CF.DefaultSettings["MFPath"], "");
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

                        NLabelUpdate("Searching for subtitle matches ");

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
                                        NLabelUpdate("Match Discovered: " + subfilesList[i].ToString());

                                        //check to see if subfile contains forced and a language code
                                        if (subfilesList[i].ToLower().Contains("forced"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".FORCED" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".FORCED" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".FORCED" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].ToLower().Contains("sdh")) //subtitles for the deaf and hard of hearing
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(SDH)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(SDH)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(SDH)" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].ToLower().Contains("cc"))//CC - Closed Captioning
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(CC)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(CC)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(CC)" + subExtensions[i]);
                                            }
                                        }

                                        else
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        CustomMessageBox.Show("Error moving file " + CF.DefaultSettings["MFPath"].ToString() + subfilesList[i].ToString(), 200, 380);
                                    }
                                }
                            }
                            if (File.Exists(CF.DefaultSettings["MFPath"] + subfilesList[i])) //If language code wasn't found get file anyway and name it as a standard movietitle.srt
                            {
                                try
                                {
                                    if (subfilesList[i].Contains("forced") ||
                                        subfilesList[i].Contains("Forced") ||
                                        subfilesList[i].Contains("FORCED"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED-2" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED-3" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                            subfilesList[i].Contains("sdh") ||
                                            subfilesList[i].Contains("Sdh"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH-2" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH-3" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("CC") || //CC-Closed Captioning
                                            subfilesList[i].Contains("cc"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC-2" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC-3" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.(2)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.(2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.(3)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.(3)" + subExtensions[i]); //move file to new folder
                                        }

                                    }
                                }
                                catch
                                {
                                    CustomMessageBox.Show("Error moving file " + CF.DefaultSettings["MFPath"].ToString() + subfilesList[i].ToString(), 200, 380);
                                }
                            }
                        }

                    }
                    break;

                case 1: //KODI [Movie Title].[2/3].[ext]  The Terminator.eng.srt
                    //check for subtitles for current movie
                    for (int i = 0; i < subfilesList.Count(); i++)
                    {

                        NLabelUpdate("Searching for subtitle matches ");

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
                                        NLabelUpdate("Match Discovered: " + subfilesList[i].ToString());

                                        //check to see if subfile contains forced and a language code
                                        if (subfilesList[i].ToLower().Contains("forced"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".FORCED" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".FORCED" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".FORCED" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".FORCED" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].ToLower().Contains("sdh")) //subtitles for the deaf and hard of hearing
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(SDH)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(SDH)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(SDH)" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].ToLower().Contains("cc"))//CC - Closed Captioning
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + ".(CC)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + ".(CC)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + ".(CC)" + subExtensions[i]);
                                            }
                                        }

                                        else
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        CustomMessageBox.Show("Error moving file " + CF.DefaultSettings["MFPath"].ToString() + subfilesList[i].ToString(), 200, 380);
                                    }
                                }
                            }
                            if (File.Exists(CF.DefaultSettings["MFPath"] + subfilesList[i])) //If language code wasn't found get file anyway and name it as a standard movietitle.srt
                            {
                                try
                                {
                                    if (subfilesList[i].Contains("forced") ||
                                        subfilesList[i].Contains("Forced") ||
                                        subfilesList[i].Contains("FORCED"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED-2" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED-3" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.FORCED-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                            subfilesList[i].Contains("sdh") ||
                                            subfilesList[i].Contains("Sdh"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH-2" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH-3" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.SDH-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("CC") || //CC-Closed Captioning
                                            subfilesList[i].Contains("cc"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC-2" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC-2" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC-3" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.CC-3" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.(2)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.(2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.(3)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG.(3)" + subExtensions[i]); //move file to new folder
                                        }

                                    }
                                }
                                catch
                                {
                                    CustomMessageBox.Show("Error moving file " + CF.DefaultSettings["MFPath"].ToString() + subfilesList[i].ToString(), 200, 380);
                                }
                            }
                        }

                    }
                    break;
                case 2: //Synology
                    //Synology
                    /*Subtitles for Synology should be formatted like:
                        * [Movie Title].[SubtitleName].[ext]
                        * Note anything in the [SubitleName] section will display
                        * so if you input Avatar(2010).EngForced.srt or Avatar(2010).Eng_Forced.srt it will display correctly
                        * but if you input Avatar(2010).Eng.Forced.srt or Avatar(2010).Eng Forced.srt  it will not display the Eng part in the list.
                        */

                    //check for subtitles for current movie
                    for (int i = 0; i < subfilesList.Count(); i++)
                    {

                        NLabelUpdate("Searching for subtitle matches ");

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
                                        NLabelUpdate("Match Discovered: " + subfilesList[i].ToString());

                                        //check to see if subfile contains forced and a language code
                                        if (subfilesList[i].Contains("forced") ||
                                            subfilesList[i].Contains("Forced") ||
                                            subfilesList[i].Contains("FORCED"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(FORCED)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(FORCED)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(FORCED)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(FORCED)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(FORCED)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(FORCED)" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                                subfilesList[i].Contains("sdh") ||
                                                subfilesList[i].Contains("Sdh"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(SDH)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(SDH)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(SDH)" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].Contains("CC") || //CC - Closed Captioning
                                                subfilesList[i].Contains("cc"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(CC)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(CC)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(CC)" + subExtensions[i]);
                                            }
                                        }

                                        else
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        CustomMessageBox.Show("Error moving file " + CF.DefaultSettings["MFPath"].ToString() + subfilesList[i].ToString(), 200, 380);
                                    }
                                }
                            }
                            if (File.Exists(CF.DefaultSettings["MFPath"] + subfilesList[i])) //If language code wasn't found get file anyway and name it as a standard movietitle.srt
                            {
                                try
                                {
                                    if (subfilesList[i].Contains("forced") ||
                                        subfilesList[i].Contains("Forced") ||
                                        subfilesList[i].Contains("FORCED"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED-2)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED-2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED-3)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED-3)" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                            subfilesList[i].Contains("sdh") ||
                                            subfilesList[i].Contains("Sdh"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH-2)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH-2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH-3)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH-3)" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("CC") || //CC-Closed Captioning
                                            subfilesList[i].Contains("cc"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC-2)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC-2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC-3)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC-3)" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(2)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(3)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(3)" + subExtensions[i]); //move file to new folder
                                        }

                                    }
                                }
                                catch
                                {
                                    CustomMessageBox.Show("Error moving file " + CF.DefaultSettings["MFPath"].ToString() + subfilesList[i].ToString(), 200, 380);
                                }
                            }
                        }

                    }
                    break;
                default: //Synology
                    //Synology
                    /*Subtitles for Synology should be formatted like:
                        * [Movie Title].[SubtitleName].[ext]
                        * Note anything in the [SubitleName] section will display
                        * so if you inpute Avatar(2010).EngForced.srt or Avatar(2010).Eng_Forced.srt it will display correctly
                        * but if you input Avatar(2010).Eng.Forced.srt or Avatar(2010).Eng Forced.srt  it will not display the Eng part in the list.
                        */

                    //check for subtitles for current movie
                    for (int i = 0; i < subfilesList.Count(); i++)
                    {

                        NLabelUpdate("Searching for subtitle matches ");

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
                                        NLabelUpdate("Match Discovered: " + subfilesList[i].ToString());

                                        //check to see if subfile contains forced and a language code
                                        if (subfilesList[i].Contains("forced") ||
                                            subfilesList[i].Contains("Forced") ||
                                            subfilesList[i].Contains("FORCED"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(FORCED)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(FORCED)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(FORCED)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(FORCED)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(FORCED)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(FORCED)" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                                subfilesList[i].Contains("sdh") ||
                                                subfilesList[i].Contains("Sdh"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(SDH)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(SDH)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(SDH)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(SDH)" + subExtensions[i]);
                                            }
                                        }

                                        else if (subfilesList[i].Contains("CC") || //CC - Closed Captioning
                                                subfilesList[i].Contains("cc"))
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + "(CC)" + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + "(CC)" + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(CC)" + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + "(CC)" + subExtensions[i]);
                                            }
                                        }

                                        else
                                        {
                                            //Check to see if file exists using 3 character sub code before writing
                                            if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 2].ToUpper() + subExtensions[i]); //move file to new folder
                                            }
                                            //Check to see if file exists using 2 character sub code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 1].ToUpper() + subExtensions[i]);
                                            }
                                            //Check to see if file exists using long language code before writing
                                            else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]))
                                            {
                                                File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "." + subLanguageCodes[b, 0].ToUpper() + subExtensions[i]);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        CustomMessageBox.Show("Error moving file " + CF.DefaultSettings["MFPath"].ToString() + subfilesList[i].ToString(), 200, 380);
                                    }
                                }
                            }
                            if (File.Exists(CF.DefaultSettings["MFPath"] + subfilesList[i])) //If language code wasn't found get file anyway and name it as a standard movietitle.srt
                            {
                                try
                                {
                                    if (subfilesList[i].Contains("forced") ||
                                        subfilesList[i].Contains("Forced") ||
                                        subfilesList[i].Contains("FORCED"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED-2)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED-2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED-3)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(FORCED-3)" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("SDH") || //SDH - Subtitles for the Deaf and Hard of Hearing
                                            subfilesList[i].Contains("sdh") ||
                                            subfilesList[i].Contains("Sdh"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH-2)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH-2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH-3)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(SDH-3)" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else if (subfilesList[i].Contains("CC") || //CC-Closed Captioning
                                            subfilesList[i].Contains("cc"))
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC-2)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC-2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC-3)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(CC-3)" + subExtensions[i]); //move file to new folder
                                        }
                                    }

                                    else
                                    {
                                        //check to see if file exists before writing.
                                        if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(2)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(2)" + subExtensions[i]); //move file to new folder
                                        }
                                        else if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(3)" + subExtensions[i]))
                                        {
                                            File.Move(CF.DefaultSettings["MFPath"] + subfilesList[i], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".ENG(3)" + subExtensions[i]); //move file to new folder
                                        }

                                    }
                                }
                                catch
                                {
                                    CustomMessageBox.Show("Error moving file " + CF.DefaultSettings["MFPath"].ToString() + subfilesList[i].ToString(), 200, 380);
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
            NLabelUpdate("Performing Search");

            if (!string.IsNullOrEmpty(imdbIDTextBox.Text))
            {
                //Check for IMDBID
                if (imdbIDTextBox.Text.Length < 10 & imdbIDTextBox.Text.Length > 8 & imdbIDTextBox.Text.Contains("tt"))
                {
                    NLabelUpdate("Building Movie Objects");
                    Movie = new MovieInfo(imdbIDTextBox.Text, APIKey); //Create Movie Object using IMDBID number from textbox

                }
                else
                {
                    NLabelUpdate("Performing Search");
                    MovieSelection M = new MovieSelection(APIKey, imdbIDTextBox.Text);

                    if (M.movieList.Count > 1)
                    {
                        M.ShowDialog();
                        if (M.DialogResult == DialogResult.OK)
                        {
                            NLabelUpdate("Building Movie Objects");
                            Movie = new MovieInfo(M.selectedID, APIKey); //Create Movie Object using IMDBID number from textbox
                        }
                        if (M.DialogResult == DialogResult.Cancel) { ClearAll(); notificationLabel.Visible = false; return; }
                        if (M.DialogResult == DialogResult.Abort) { ClearAll(); notificationLabel.Visible = false; return; }
                    }
                    else if (M.movieList.Count == 1)
                    {
                        NLabelUpdate("Building Movie Objects");
                        Movie = new MovieInfo(M.movieList[0]["ID"], APIKey); //Create Movie Object using IMDBID number from textbox
                    }
                    else
                    {

                        CustomMessageBox.Show(imdbIDTextBox.Text + " is not a valid search term or IMDB ID", 189, 287, "Invalid IMDB ID Number");
                        ClearAll();
                        return;
                    }
                }
            }
            else
            {
                CustomMessageBox.Show(imdbIDTextBox.Text + " is not a valid IMDB movie ID #", 189, 287, "Invalid IMDB ID Number");
                ClearAll();
                return;
            }

            string videoFilePath = videoPathTextBox.Text; //Saves video file path
            ClearAll(); //Clears form contents
            videoPathTextBox.Text = videoFilePath; //fills in the video file path text box with previous info


            /*Call public methods of the movie object one by in order to show progress bar
            Could be all called with the constructor but since it takes a while this will 
            make it so we can use the progress bar. The order doesn't matter as these
            methods for the object don't need to be called in order*/

            NLabelUpdate("Gathering Film Credits");

            Movie.GetCredits();

            NLabelUpdate("Retrieving Film MPAA Rating");

            Movie.GetRating(); //Returns the MPAA Rating 

            NLabelUpdate("Retrieving Alternate US Titles");

            Movie.GetUSTitles(); //Builds list of US versions of the film title

            NLabelUpdate("Gathering US and Non-Region Coded Film Images");
            Movie.GetFilmImages(); //Gathers film image URLs

            NLabelUpdate("Gathering Plot, Title, Genre, etc");

            Movie.GetBasicInfo(); //Returns basic film info
            imdbIDTextBox.Text = Movie.StaticProperties["IMDB_ID"]; //adds the IMDBID back to the form


            NLabelUpdate("Adding Alternate Titles");

            //Loop to add movie titles from movie object to the combo box
            for (int i = 0; i < Movie.ListProperties["USTitles"].Count; i++)
            {
                titleComboBox.Items.Add(Movie.ListProperties["USTitles"][i]);
            }

            if (!titleComboBox.Items.Contains(Movie.StaticProperties["Title"]) && !string.IsNullOrEmpty(Movie.StaticProperties["Title"]))
            {
                titleComboBox.Items.Add(Movie.StaticProperties["Title"]);
            }

            if (!titleComboBox.Items.Contains(Movie.StaticProperties["OriginalTitle"]) && !string.IsNullOrEmpty(Movie.StaticProperties["OriginalTitle"]))
            {
                titleComboBox.Items.Add(Movie.StaticProperties["OriginalTitle"]);
            }
            SetTitleComboBoxWidth();

            NLabelUpdate("Filling in Form Data");
            titleComboBox.Text = Movie.StaticProperties["Title"]; //uses the first title in the list as the default title;
            setTextBox.Text = Movie.StaticProperties["Collection"]; //uses the Collection information from the movie object to fill in the collection information
            yearTextBox.Text = Movie.StaticProperties["ReleaseYear"]; //uses the release year information from the movie object to fill in the year
            runTimeTextBox.Text = Movie.StaticProperties["RunTime"];
            mpaaTextBox.Text = Movie.StaticProperties["MPAA_Rating"];
            genresTextBox.Text = Movie.StaticProperties["Genres"];

            if (string.IsNullOrEmpty(Movie.StaticProperties["Tag_Line"])) { plotTextBox.Text = Movie.StaticProperties["Plot"]; }
            else { plotTextBox.Text = Movie.StaticProperties["Tag_Line"] + "..\r\n\r\n" + Movie.StaticProperties["Plot"]; }


            NLabelUpdate("Setting up images");

            posterNumericUpDown.Maximum = Movie.ListProperties["Posters"].Count() + 1;
            posterNumericUpDown.Minimum = 0;
            backdropNumericUpDown.Maximum = Movie.ListProperties["Backdrops"].Count() + 1;
            backdropNumericUpDown.Minimum = 0;

            posterNumberLabel.Text = "of " + Movie.ListProperties["Posters"].Count().ToString();
            backdropNumberLabel.Text = "of " + Movie.ListProperties["Backdrops"].Count().ToString();
            posterNumericUpDown.Value = 1;
            backdropNumericUpDown.Value = 1;

            backDropPictureBox.ImageLocation = Movie.StaticProperties["BackDropPath"];
            moviePosterPictureBox.ImageLocation = Movie.StaticProperties["PosterPath"];

            NLabelUpdate("Gathering Film Credits");
            notificationLabel.Visible = false;

            //Determine if errors were encountered

            if (Movie.ListProperties["Errors"].Count > 0)
            {
                StringBuilder Errors = new StringBuilder();
                foreach (string s in Movie.ListProperties["Errors"])
                {
                    if (string.IsNullOrEmpty(Errors.ToString())) { Errors.Append(s); }
                    else { Errors.Append("\r\r" + s); }
                }
                ClearAll();
                CustomMessageBox.Show(Errors.ToString(), 300, 400);
            }

        }
        private void CreateFilesButton_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor; //sets cursor to waitcursor
            notificationLabel.Visible = true;
            NLabelUpdate("Start");



            if (!string.IsNullOrEmpty(videoPathTextBox.Text) && (!string.IsNullOrEmpty(titleComboBox.Text)))
            {
                NLabelUpdate("Checking Format Selection");

                switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                {
                    case 0: //Plex
                        Movie.StaticProperties["FormattedTitle"] = ValidTitle(titleComboBox.Text + " (" + yearTextBox.Text + ")");
                        break;
                    case 1: //KODI
                        Movie.StaticProperties["FormattedTitle"] = ValidTitle(titleComboBox.Text + " (" + yearTextBox.Text + ")");
                        
                        break;
                    case 2: //Synology
                        Movie.StaticProperties["FormattedTitle"] = ValidTitle(titleComboBox.Text + " (" + yearTextBox.Text + ")");
                        break;
                    default: 
                        Movie.StaticProperties["FormattedTitle"] = ValidTitle(titleComboBox.Text + " (" + yearTextBox.Text + ")");
                        break;
                }

                NLabelUpdate("Setting Video Path");

                videoPath = CF.DefaultSettings["MFPath"] + Movie.StaticProperties["FormattedTitle"] + videoExtension;
                newDirectoryName = CF.DefaultSettings["MFPath"] + Movie.StaticProperties["FormattedTitle"];

                NLabelUpdate("Checking for existing directory");
                
                
                //add code to check for directory prior to doing anything
                if (!Directory.Exists(newDirectoryName))
                {
                    NLabelUpdate("Creating Directory");
                    DirectoryInfo di = Directory.CreateDirectory(newDirectoryName);
                    NLabelUpdate("Checking for existing video file");

                    if (File.Exists(videoPathTextBox.Text))
                    {
                        NLabelUpdate("Renaming video file");
                        string moveToPath = CF.DefaultSettings["MFPath"] + Movie.StaticProperties["FormattedTitle"] + videoExtension; //uses validTitle method to remove illegal characters
                        File.Move(videoPathTextBox.Text, moveToPath); //renames file if in the same directory
                    }
                    else
                    {
                        CustomMessageBox.Show("File: " + videoPathTextBox.Text + "\n Does not exist.", 200, 380);
                    }

                    NLabelUpdate("Checking for existing file in folder");

                    if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + videoExtension)) //Check that file isn't already in folder
                    {
                        NLabelUpdate("Moving file to folder");
                        
                        File.Move(videoPath, newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + videoExtension); //move file to new folder
                    }
                    else
                    {
                        CustomMessageBox.Show("File: " + newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + videoExtension + ".\n Already exists!", 200, 380);
                    }

                    NLabelUpdate("Checking for subtitle files");

                    FindSubtitlesInFile();
                    //subTitleNamer(formatComboBox.SelectedIndex);

                    NLabelUpdate("Formatting artwork URLs");
                    
                    //add images
                    if (Movie.ListProperties["Posters"].Count > 0)
                    {
                        Movie.StaticProperties["PosterPath"] = Movie.ListProperties["Posters"][(int)posterNumericUpDown.Value - 1];
                        Movie.StaticProperties["PosterPath"] = Movie.StaticProperties["PosterPath"].Replace("/w154/", "/original/");
                    }

                    if (Movie.ListProperties["Backdrops"].Count > 0)
                    {
                        Movie.StaticProperties["BackDropPath"] = Movie.ListProperties["Backdrops"][(int)backdropNumericUpDown.Value - 1];
                        Movie.StaticProperties["BackDropPath"] = Movie.StaticProperties["BackDropPath"].Replace("/w300/", "/original/");
                    }

                    if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".jpg")
                        & !File.Exists(newDirectoryName + "\\" + "poster.jpg")
                        & !File.Exists(newDirectoryName + "\\" + "Folder.jpg")
                        & !string.IsNullOrEmpty(Movie.StaticProperties["PosterPath"]))
                    {
                        NLabelUpdate("Downloading Poster");
                        
                        using (WebClient wc = new WebClient())
                        {

                            try //Attempt to download original file
                            {
                                wc.Encoding = System.Text.Encoding.UTF8; //Sets Encoding for text output

                                switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                {
                                    case 0: //Plex
                                        wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".jpg");
                                        break;
                                    case 1: //KODI
                                        wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");                                 
                                        break;
                                    case 2: // Synology
                                        wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".jpg");
                                        break;
                                    default:
                                        wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".jpg");
                                        break;
                                }

                            }
                            catch //If original file doesn't exist try w1000
                            {
                                Movie.StaticProperties["PosterPath"] = Movie.StaticProperties["PosterPath"].Replace("/original/", "/w1000/");
                                try //Attempt to download w1000 size
                                {
                                    switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                    {
                                        case 0: //Plex
                                            wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".jpg");
                                            break;
                                        case 1: //KODI
                                            wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");
                                            break;
                                        case 2: //Synology
                                            wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + "Folder.jpg");
                                            break;
                                        default:
                                            wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + "Folder.jpg");
                                            break;
                                    }
                                }
                                catch //If w1000 doesn't exist try w500
                                {
                                    Movie.StaticProperties["PosterPath"] = Movie.StaticProperties["PosterPath"].Replace("/w1000/", "/w500/");
                                    try //Attempt to download w500 size
                                    {
                                        switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                        {
                                            case 0: //PLEX
                                                wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".jpg");
                                                break;
                                            case 1: //KODI
                                                wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");
                                                break;
                                            case 2: //Synology
                                                wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + "Folder.jpg");
                                                break;
                                            default: 
                                                wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + "Folder.jpg");
                                                break;
                                        }
                                    }
                                    catch //if w500 doesn't exist try w300
                                    {
                                        Movie.StaticProperties["PosterPath"] = Movie.StaticProperties["PosterPath"].Replace("/w500/", "/w300/");
                                        try //Attempt to download w300 size
                                        {
                                            switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                            {
                                                case 0: //PLEX
                                                    wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".jpg");
                                                    break;
                                                case 1: //KODI
                                                    wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + "poster.jpg");
                                                    break;
                                                case 2: //Synology
                                                    wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + "Folder.jpg");
                                                    break;
                                                default:
                                                    wc.DownloadFile(Movie.StaticProperties["PosterPath"], newDirectoryName + "\\" + "Folder.jpg");
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
                    if (!File.Exists(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "-fanart.jpg")
                        & !File.Exists(newDirectoryName + "\\" + "fanart.jpg")
                        & !string.IsNullOrEmpty(Movie.StaticProperties["BackDropPath"]))
                    {
                        NLabelUpdate("Downloading Backdrop/Fanart");
                        
                        using (WebClient wc = new WebClient())
                        {
                            try //Attempt to download original file
                            {
                                wc.Encoding = System.Text.Encoding.UTF8; //Sets Encoding for text output
                                switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                {
                                    case 0: //PLEX
                                        wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "-fanart.jpg");
                                        break;
                                    case 1: //KODI
                                        wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                        break;
                                    case 2: //Synology
                                        wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                        break;
                                    default:
                                        wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                        break;
                                }

                            }
                            catch
                            {

                                Movie.StaticProperties["BackDropPath"].Replace("/original/", "/w1000/");
                                try //Attempt to download w1000 file
                                {
                                    switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                    {
                                        case 0: //PLEX
                                            wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "-fanart.jpg");
                                            break;
                                        case 1: //KODI
                                            wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                            break;
                                        case 2: //Synology
                                            wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                            break;
                                        default:
                                            wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                            break;
                                    }
                                }
                                catch
                                {
                                    Movie.StaticProperties["BackDropPath"].Replace("/w1000/", "/w500/");
                                    try //Attempt to download w500 file
                                    {
                                        switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                        {
                                            case 0: //Plex
                                                wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "-fanart.jpg");
                                                break;
                                            case 1: //KODI
                                                wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                                break;
                                            case 2: //Synology
                                                wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                                break;
                                            default:
                                                wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                                break;
                                        }
                                    }
                                    catch
                                    {
                                        Movie.StaticProperties["BackDropPath"].Replace("/w500/", "/w300/");
                                        try //Attempt to download w300 file
                                        {
                                            switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                                            {
                                                case 0: //PLEX
                                                    wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + "-fanart.jpg");
                                                    break;
                                                case 1: //KODI
                                                    wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                                    break;
                                                case 2: //Synology
                                                    wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
                                                    break;
                                                default:
                                                    wc.DownloadFile(Movie.StaticProperties["BackDropPath"], newDirectoryName + "\\" + "fanart.jpg");
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
                        NLabelUpdate("Creating NFO Metadata file");
                        
                        

                        switch (formatComboBox.SelectedIndex) //Formats title to plex, kodi, or synology spec
                        {
                            case 0: //Plex
                                using (StreamWriter sw = File.CreateText(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".nfo"))
                                {
                                    sw.Write(CreateNFO());
                                }
                                break;
                            case 1: //KODI
                                using (StreamWriter sw = File.CreateText(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".nfo"))
                                {
                                    sw.Write(CreateNFO());
                                }
                                break;
                            case 2: //Synology
                                using (StreamWriter sw = File.CreateText(newDirectoryName + "\\" + Movie.StaticProperties["FormattedTitle"] + ".nfo"))
                                {
                                    sw.Write(CreateNFO());
                                }
                                break;
                            default:
                                break;
                        }


                    }
                    catch
                    {
                        CustomMessageBox.Show("Faile to create NFO file", 119, 202);
                    }
                    

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

            NLabelUpdate("Finished");
            ClearAll();
            notificationLabel.Visible = false;

        }
        private void ClearAll()
        {
            videoPathTextBox.Clear();
            imdbIDTextBox.Clear();
            yearTextBox.Clear();
            setTextBox.Clear();
            mpaaTextBox.Clear();
            runTimeTextBox.Clear();
            genresTextBox.Clear();
            plotTextBox.Clear();
            subfilesList.Clear();
            backdropNumberLabel.Text = "";
            posterNumberLabel.Text = " of 1";
            backdropNumberLabel.Text = " of 1";
            posterNumericUpDown.Maximum = 1;
            posterNumericUpDown.Minimum = 1;
            backdropNumericUpDown.Maximum = 1;
            backdropNumericUpDown.Minimum = 1;
            titleComboBox.Items.Clear();
            titleComboBox.Text = "";

            if(Movie != null && Movie.ListProperties.ContainsKey("Backdrops") && Movie.ListProperties["Backdrops"].Count() > 0) { Movie.ListProperties["Backdrops"].Clear(); }
            if (Movie != null && Movie.ListProperties.ContainsKey("Posters") && Movie.ListProperties["Posters"].Count() > 0) { Movie.ListProperties["Posters"].Clear(); }

            backDropPictureBox.ImageLocation = "";
            backDropPictureBox.Image = MovieDataCollector.Properties.Resources.highlight_reel;
            moviePosterPictureBox.ImageLocation = "";
            moviePosterPictureBox.Image = MovieDataCollector.Properties.Resources.film_reel__Small_;

        }
        private string ValidTitle(string unformattedTitle)
        {
            string charsToRemove = @"\/:*?<>|"; //\ / : * ? " < > |
            unformattedTitle = unformattedTitle.Replace("\"", "");
            string pattern = string.Format("[{0}]", Regex.Escape(charsToRemove));

            return Regex.Replace(unformattedTitle, pattern, "");
        }
        private void IMDBcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.imdb.com");
        }
        private void ClearButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;
            ClearAll();
        }
        private void PosterNumericUpDown_ValueChanged(object sender, EventArgs e)
        {

            if (Movie == null || Movie.ListProperties["Posters"].Count <= 0) { moviePosterPictureBox.Image = MovieDataCollector.Properties.Resources.film_reel__Small_; return; }

            else if (posterNumericUpDown.Value <= (Movie.ListProperties["Posters"].Count) && posterNumericUpDown.Value >= 1)
            {
                moviePosterPictureBox.ImageLocation = Movie.ListProperties["Posters"][(int)posterNumericUpDown.Value - 1];
            }
            else if (posterNumericUpDown.Value >= (Movie.ListProperties["Posters"].Count))
            {
                posterNumericUpDown.Value = 1;
                moviePosterPictureBox.ImageLocation = Movie.ListProperties["Posters"][(int)posterNumericUpDown.Value - 1];
            }
            else if (posterNumericUpDown.Value < 1)
            {
                posterNumericUpDown.Value = Movie.ListProperties["Posters"].Count;
                moviePosterPictureBox.ImageLocation = Movie.ListProperties["Posters"][(int)posterNumericUpDown.Value - 1];
            }
        }
        private void BackdropNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            if (Movie == null || Movie.ListProperties["Backdrops"].Count <= 0) { backDropPictureBox.Image = MovieDataCollector.Properties.Resources.highlight_reel; return; }

            else if (backdropNumericUpDown.Value <= (Movie.ListProperties["Backdrops"].Count) && backdropNumericUpDown.Value >= 1)
            {
                backDropPictureBox.ImageLocation = Movie.ListProperties["Backdrops"][(int)backdropNumericUpDown.Value - 1];
            }
            else if (backdropNumericUpDown.Value >= (Movie.ListProperties["Backdrops"].Count))
            {
                backdropNumericUpDown.Value = 1;
                backDropPictureBox.ImageLocation = Movie.ListProperties["Backdrops"][(int)backdropNumericUpDown.Value - 1];
            }
            else if (backdropNumericUpDown.Value < 1)
            {
                backdropNumericUpDown.Value = Movie.ListProperties["Backdrops"].Count;
                backDropPictureBox.ImageLocation = Movie.ListProperties["Backdrops"][(int)backdropNumericUpDown.Value - 1];
            }
        }
        private string CreateNFO()
        {
            string nfoOutPut = "";
            string genresNFOString = "";
            string studiosNFOString = "";
            string actorsNFOString = "";

            for (int i = 0; i < Movie.ListProperties["ActorNames"].Count; i++)
            {
                actorsNFOString += "\t" + "<actor>\r\n" + "\t\t" + "<name>" + Movie.ListProperties["ActorNames"][i] + "</name>\r\n"
                    + "\t\t" + "<role>" + Movie.ListProperties["ActorRoles"][i] + "</role>\r\n"
                    + "\t\t" + "<thumb>" + Movie.ListProperties["ActorImages"][i] + "</thumb>\r\n"
                    + "\t" + "</actor>\r\n";
            }

            for (int i = 0; i < Movie.ListProperties["GenreList"].Count; i++)
            {
                genresNFOString += "\t" + "<genre>" + Movie.ListProperties["GenreList"][i] + "</genre>\r\n";
            }

            for (int i = 0; i < Movie.ListProperties["StudioList"].Count; i++)
            {
                studiosNFOString += "\t" + "<studio>" + Movie.ListProperties["StudioList"][i] + "</studio>\r\n";
            }

            //KODI NFO File
            nfoOutPut =
                    "<movie>\r\n" +
                        "\t<title>" + titleComboBox.Text + "</title>\r\n" +
                        "\t<set>" + setTextBox.Text + "</set>\r\n" +
                        "\t<sorttitle>" + "</sorttitle>\r\n" +
                        "\t<rating>" + Movie.StaticProperties["VoteAverage"] + "</rating>\r\n" +
                        "\t<year>" + yearTextBox.Text + "</year>\r\n" +
                        "\t<top250>0</top250>\r\n" +
                        "\t<votes>" + Movie.StaticProperties["VoteCount"] + "</votes>\r\n" +
                        "\t<outline>" + Movie.StaticProperties["Tag_Line"] + "</outline>\r\n" +
                        "\t<plot>" + Movie.StaticProperties["Plot"] + "</plot>\r\n" +
                        "\t<tagline>" + Movie.StaticProperties["Tag_Line"] + "</tagline>\r\n" +
                        "\t<runtime>" + runTimeTextBox.Text + "</runtime>\r\n" +
                        "\t<thumb>" + moviePosterPictureBox.ImageLocation.ToString() + "</thumb>\r\n" +
                        "\t<mpaa>" + mpaaTextBox.Text + "</mpaa>\r\n" +
                        "\t<playcount>0</playcount>" +
                        "\t<id>" + Movie.StaticProperties["IMDB_ID"] + "</id>\r\n" +
                        "\t<trailer></trailer>\r\n" +
                         genresNFOString +
                         "\t<credits></credits>\r\n" +
                         studiosNFOString +
                         "\t<director>" + Movie.StaticProperties["Director"] + "</director>\r\n" +
                         actorsNFOString +
                         "</movie>";

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
        private void FormatComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (formatComboBox.SelectedIndex)
            {
                case 0: //PLEX
                    formatPicturebox.Image = MovieDataCollector.Properties.Resources.PlexLogo;
                    CF.DefaultSettings["DefaultFormat"] = "PLEX";
                    CF.updateDefaults();
                    break;
                case 1: //KODI
                    formatPicturebox.Image = MovieDataCollector.Properties.Resources.kodi;
                    CF.DefaultSettings["DefaultFormat"] = "KODI";
                    CF.updateDefaults();
                    break;
                case 2: //Synology
                    formatPicturebox.Image = MovieDataCollector.Properties.Resources.Synology;
                    CF.DefaultSettings["DefaultFormat"] = "Synology";
                    CF.updateDefaults();
                    break;
                default:
                    formatPicturebox.Image = Properties.Resources.Synology;
                    CF.DefaultSettings["DefaultFormat"] = "Synology";
                    CF.updateDefaults();
                    break;
            }
        }
        private void NLabelUpdate(string Msg)
        {
            notificationLabel.Text = Msg;
            notificationLabel.Invalidate();
            notificationLabel.Update();
        }
        private void SetTitleComboBoxWidth()
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

        private void TheMovieDBorgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.TheMovieDB.org");
        }
    }
}
