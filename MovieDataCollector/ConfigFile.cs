using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace MovieDataCollector
{
    class ConfigFile
    {
        public string ConfigDirectory { get; set; }
        public string ConfigPath { get; set; }
        public Dictionary<string,string> DefaultSettings { get; set; }
        public List<string> FavoritesList { get; set; }
        public List<string> FavoriteTitles { get; set; }
        public List<string> FavoriteIDs { get; set; }

        string configString = ""; //Holds configuration file text from when the file is first read in.
        
        List<string> KeyList = new List<string>()
            {   "MFPath", //C:\\
                "TFPath",//C:\\
                "InputFilePath", //C:\\
                "OutputFilePath", //C:\\
                "ExportFilePath", //C:\\
                "CompatibilitySelector", //Roku
                "DefaultFormat", //Plex
                "TwoPass", //True
                "TurboFirstPass", //True
                "Optimize", //True
                "AutoCrop", //True
                "Encoder", //x264 (FFMPEG)
                "EncoderSpeed", //Medium
                "EncoderTune", //Fast Decode
                "EncoderProfile", //High
                "EncoderLevel", //High
                "VideoBitrateCap", //3.5
                "Framerate", //Roku Compliant
                "FramerateMode", //Constant, Peak, Variable
                "ConversionPreset", //Roku Compliant
                "SMTP_Port", //Left blank as default
                "SMTP_Server", //Left blank as default
                "SMTP_Account", //Left blank as default
                "SMTP_Password", //Left blank as Default
                "NotifyAddress", //Left blank as default
                "SubtitleSelection", //None
                "ForcedSubtitleBurnIn", //False

                "AudioCodec", //AAC (FDK)
                "AAC_Passthru", //False
                "AC3_Passthru", //False
                "EAC3_Passthru", //False
                "DTS_Passthru", //False
                "DTSHD_Passthru", //False
                "TrueHD_Passthru", //False
                "MP3_Passthru", //False
                "FLAC_Passthru", //False

                "Mixdown", //Dolby Prologic 2
                "AudioBitrateCap", //128
                "AudioSampleRate", //48
                "AudioGain", //0

                "EnableTrack2", //false
                "AudioCodec2", //E-AC3
                "AAC_Passthru2", //False
                "AC3_Passthru2", //False
                "EAC3_Passthru2", //False
                "DTS_Passthru2", //False
                "DTSHD_Passthru2", //False
                "TrueHD_Passthru2", //False
                "MP3_Passthru2", //False
                "FLAC_Passthru2", //False

                "Mixdown2", //5.1
                "AudioBitrateCap2", //128
                "AudioSampleRate2", //48
                "AudioGain2", //0

                "EnableTrack3", //false
                "AudioCodec3", //Passthru
                "AAC_Passthru3", //False
                "AC3_Passthru3", //False
                "EAC3_Passthru3", //False
                "DTS_Passthru3", //False
                "DTSHD_Passthru3", //False
                "TrueHD_Passthru3", //False
                "MP3_Passthru3", //False
                "FLAC_Passthru3", //False

                "Mixdown3", //5.1
                "AudioBitrateCap3", //128
                "AudioSampleRate3", //48
                "AudioGain3", //0

            };
        List<string> ValueList = new List<string>()
            {
                "C:\\", //"MFPath"
                "C:\\", //TFPath"
                "C:\\", //InputFilePath
                "C:\\", //OutputFilePath
                "C:\\", //ExportFilePath
                "Roku", //Compatibility Selector
                "Synology", //DefaultFormat
                "True", //TwoPass
                "True", //TurboFirstPass
                "True", //Optimize
                "True", //Autocrop
                "x264 (FFMPEG)", //Encoder
                "Medium", //EncoderSpeed
                "Fast Decode", //EncoderTune
                "High", //EncoderProfile
                "4.0", //EncoderLevel
                "3.5", //VideoBitrateCap
                "Roku Compliant", //Framerate
                "Peak", //FramerateMode
                "", //ConversionPreset is blank by default
                "", //SMTP_Port
                "", //SMTP_Server
                "", //SMTP_Account
                "", //SMTP_Password
                "", //NotifyAddress
                "None", //SubtitleSelection
                "False", //ForcedSubtitleBurnIn

                "AAC (FDK)", //"AudioCodec"
                "False", //"AAC_Passthru"
                "False", //"AC3_Passthru"
                "False", //"EAC3_Passthru"
                "False", //"DTS_Passthru"
                "False", //"DTSHD_Passthru"
                "False", //"TrueHD_Passthru"
                "False", //"MP3_Passthru"
                "False", //"FLAC_Passthru"

                "Dolby Prologic 2", //Mixdown
                "128", //AudioBitrateCap
                "48", //AudioSampleRate
                "0", //AudioGain

                "false", //EnableTrack2
                "E-AC3", //AudioCodec2
                "False", //"AAC_Passthru2"
                "False", //"AC3_Passthru2"
                "False", //"EAC3_Passthru2"
                "False", //"DTS_Passthru2"
                "False", //"DTSHD_Passthru2"
                "False", //"TrueHD_Passthru2"
                "False", //"MP3_Passthru2"
                "False", //"FLAC_Passthru2"

                "5.1 Audio", //Mixdown2
                "128", //AudioBitrateCap2
                "48", //AudioSampleRate2
                "0", //AudioGain2

                "false", //EnableTrack3
                "Filtered Passthru", //AudioCodec3
                "False", //"AAC_Passthru3"
                "True", //"AC3_Passthru3"
                "True", //"EAC3_Passthru3"
                "True", //"DTS_Passthru3"
                "True", //"DTSHD_Passthru3"
                "True", //"TrueHD_Passthru3"
                "False", //"MP3_Passthru3"
                "False", //"FLAC_Passthru3"

                "5.1 Audio", //Mixdown3
                "128", //AudioBitrateCap3
                "48", //AudioSampleRate3
                "0", //AudioGain3
            };

        public ConfigFile()
        {
            ConfigDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector";//Writable folder location for config file.
            ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Config.txt"; //Writable file location for config file.

            DefaultSettings = new Dictionary<string, string>();//Dictionary containing default settings
            FavoritesList = new List<string>(); //List containing favorite TV show information from config file
            FavoriteTitles= new List<string>(); //instantiate list
            FavoriteIDs = new List<string>(); //instantiate list

            CheckConfigFile();
        }
        /// <summary>
        /// Checks that configuration file exists and creates it with defaults if it doesn't.
        /// </summary>
        public void CheckConfigFile()
        {
            string defaults = "";
            string favorites = "";

            //Check Directory and File, Create if they dont' exist.
            try
            {
                //if (!Directory.Exists(ConfigDirectory)) { Directory.CreateDirectory(ConfigDirectory); }
                new FileInfo(ConfigPath).Directory.Create();
                if (!File.Exists(ConfigPath))
                {
                    using (StreamWriter sw = File.CreateText(ConfigPath)) { }
                }

                //Extract File Text
                using (StreamReader sr = new StreamReader(ConfigPath))
                {
                    configString = sr.ReadToEnd();
                    sr.Close();
                }

                //Create new text for config file
                defaults = GenerateDefaultsString();  //Also generates dictionary of defaults
                favorites = GenerateFavoritesString();

                //Write text file with new text
                using (StreamWriter sw = new StreamWriter(ConfigPath))
                {
                    sw.WriteLine(defaults + "\r\n" + favorites);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), 300, 300);
            }

        }
        public string GenerateDefaultsString()
        {
            string Defaults = "";

            DefaultSettings.Clear();
            //Build Dictionary
            for (int i = 0; i < KeyList.Count(); i++)
            {
                DefaultSettings.Add(KeyList[i], ValueList[i]);
            }

            //If File is empty fill with default
            if (string.IsNullOrEmpty(configString))
            {
                string Start = "<Defaults>\r\n";
                string Middle = "";
                string End = "</Defaults>";

                for (int i = 0; i < KeyList.Count; i++)
                {
                    Middle += "\t<" + KeyList[i] + ">" + ValueList[i] + "</" + KeyList[i] + ">\r\n";
                }
                Defaults = Start + Middle + End;
            }
            //Check file for defaults set by user
            else
            {
                string Start = "<Defaults>\r\n";
                string Middle = "";
                string End = "</Defaults>";

                //filter out section of text file to work with
                Defaults = Program.GeneralParser(configString, "<Defaults>", "</Defaults>");

                //If value is found in the file replace dictionary defaults with the file value set by user.
                for (int i = 0; i < KeyList.Count; i++)
                {
                    if (!string.IsNullOrEmpty(Program.GeneralParser(Defaults, "<" + KeyList[i] + ">", "</" + KeyList[i] + ">")))
                    { DefaultSettings[KeyList[i]] = Program.GeneralParser(Defaults, "<" + KeyList[i] + ">", "</" + KeyList[i] + ">"); }
                }

                //Build middle section of string
                for (int i = 0; i < KeyList.Count; i++)
                {
                    Middle += "\t<" + KeyList[i] + ">" + DefaultSettings[KeyList[i]] + "</" + KeyList[i] + ">\r\n";
                }

                //Combine string sections to generate section text
                Defaults = Start + Middle + End;
            }

            return Defaults;
        }
        public void UpdateDefaults()
        {
            string start = "<Defaults>\r\n";
            string middle = "";
            string end = "</Defaults>";
            string defaults = "";
            string favorites = "";

            try
            {
                //Extract File Text
                using (StreamReader sr = new StreamReader(ConfigPath))
                {
                    configString = sr.ReadToEnd();
                    sr.Close();
                }

                favorites = ReturnFavorites();

                //Update configuration file with defaults from dictionary
                for (int i = 0; i < KeyList.Count; i++)
                {
                    middle += "\t<" + KeyList[i] + ">" + DefaultSettings[KeyList[i]] + "</" + KeyList[i] + ">\r\n";
                }

                //Combine string sections to generate section text
                defaults = start + middle + end;

                //Write text file with new text
                using (StreamWriter sw = new StreamWriter(ConfigPath))
                {
                    sw.WriteLine(defaults + "\r\n" + favorites);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), 300, 300);
            }
        }
        public string GenerateFavoritesString()
        {
            string favorites = "";
            string beginning = "<Favorites>\r\n";
            string middle = "";
            string end = "</Favorites>";
            FavoritesList.Clear();
            FavoriteTitles.Clear();
            FavoriteIDs.Clear();

            //If File is empty fill with default
            if (string.IsNullOrEmpty(configString))
            {
                favorites = "<Favorites>\r\n" +
                   "</Favorites>";
            }
            else //Need to add a check to ensure that if there are no favorites everything still works.
            {
                //filter out section of text file to work with
                favorites = Program.GeneralParser(configString, beginning, "\r\n" + end);

                //Sort and create List
                
                FavoritesList = Regex.Split(favorites,"\r\n").ToList();
                FavoritesList.Sort();

                for (int i = 0; i < FavoritesList.Count; i++)
                {
                    middle += FavoritesList[i] + "\r\n";
                    FavoriteTitles.Add(Program.GeneralParser(FavoritesList[i], "<TVShow>", "<ShowID>"));
                    FavoriteIDs.Add(Program.GeneralParser(FavoritesList[i], "<ShowID>", "</TVShow>"));
                }

                //Generate string
                favorites = beginning + middle + end;
            }

            return favorites;
        }
        public void AddFavorite(string title, string id)
        {
            string beginning = "<Favorites>\r\n";
            string end = "</Favorites>";
            string middle = "";


            CheckConfigFile(); //updates variable information

            FavoriteTitles.Clear();
            FavoriteIDs.Clear();
            FavoritesList.Clear();

            

            string defaults = "";
            string favorites = "";

            try
            {

                defaults = ReturnDefaults();
                favorites = ReturnFavorites();
                favorites = favorites.Replace("<Favorites>\r\n", "");
                favorites = favorites.Replace("</Favorites>", "");

                //Create List
                FavoritesList = Regex.Split(favorites, "\r\n").ToList();

                //Check for duplicates and Add new item to list
                if(!FavoritesList.Contains("\t<TVShow>" + title + "<ShowID>" + id + "</TVShow>"))
                {
                    FavoritesList.Add("\t<TVShow>" + title + "<ShowID>" + id + "</TVShow>");
                }

                FavoritesList.Sort();

                //recreate lists
                for (int i = 0; i < FavoritesList.Count; i++)
                {
                    if(!string.IsNullOrEmpty(Program.GeneralParser(FavoritesList[i], "<TVShow>", "<ShowID>")))
                    {
                        middle += FavoritesList[i] + "\r\n";
                        FavoriteTitles.Add(Program.GeneralParser(FavoritesList[i], "<TVShow>", "<ShowID>"));
                        FavoriteIDs.Add(Program.GeneralParser(FavoritesList[i], "<ShowID>", "</TVShow>"));
                    }
                }


                //Rebuild FavoritesList
                FavoritesList.Clear();
                for (int i = 0; i < FavoriteTitles.Count; i++)
                {
                    FavoritesList.Add("\t<TVShow>" + FavoriteTitles[i] + "<ShowID>" + FavoriteIDs[i] + "</TVShow>\r\n");
                }

                //Generate string
                favorites = beginning + middle + end;
                //Write text file with new text
                using (StreamWriter sw = new StreamWriter(ConfigPath))
                {
                    sw.WriteLine(defaults + favorites);
                    sw.Close();
                }
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), 300, 300);
            }

        }
        public void RemoveFavorite(int index)
        {
            string beginning = "<Favorites>\r\n";
            string end = "</Favorites>";
            string middle = "";
            string favorites = "";
            string defaults = ReturnDefaults();

            FavoritesList.RemoveAt(index);
            FavoriteTitles.RemoveAt(index);
            FavoriteIDs.RemoveAt(index);

            for (int i = 0; i < FavoritesList.Count; i++)
            {
                middle += FavoritesList[i] + "\r\n";
            }

            //Generate string
            favorites = beginning + middle + end;
            try
            {
                //Write text file with new text
                using (StreamWriter sw = new StreamWriter(ConfigPath))
                {
                    sw.WriteLine(defaults + favorites);
                    sw.Close();
                }
            } 
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), 300, 300);
            }
        }
        private string ReturnFavorites()
        {
            return "<Favorites>\r\n" + Program.GeneralParser(configString, "<Favorites>\r\n", "</Favorites>") + "</Favorites>";
        }
        private string ReturnDefaults()
        {
            return "<Defaults>\r\n" + Program.GeneralParser(configString, "<Defaults>\r\n", "</Defaults>") + "</Defaults>\r\n";
        }
    }
}
