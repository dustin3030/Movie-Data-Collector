using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace MovieDataCollector
{
    class ConfigFile
    {
        public string configDirectory { get; set; }
        public string configPath { get; set; }
        public Dictionary<string,string> DefaultSettings { get; set; }
        public List<string> favoritesList { get; set; }
        public List<string> favoriteTitles { get; set; }
        public List<string> favoriteIDs { get; set; }

        string configString = ""; //Holds configuration file text from when the file is first read in.
        List<string> KeyList = new List<string>()
            {   "MFPath", //C:\\
                "TFPath",//C:\\
                "InputFilePath", //C:\\
                "OutputFilePath", //C:\\
                "DefaultFormat", //Synology
                "AudioCodec", //AAC (FDK)
                "AAC_Passthru", //False
                "AC3_Passthru", //False
                "DTS_Passthru", //False
                "TwoPass", //True
                "TurboFirstPass", //True
                "Optimize", //True
                "Mixdown", //Dolby Prologic 2
                "AudioBitrateCap", //96
                "EncoderSpeed", //Medium
                "EncoderTune", //Fast Decode
                "EncoderProfile", //High
                "EncoderLevel", //High
                "VideoBitrateCap", //3.5
                "Framerate", //Roku Compliant
            };
        List<string> ValueList = new List<string>()
            {
                "C:\\", //"MFPath"
                "C:\\", //TFPath"
                "C:\\", //InputFilePath
                "C:\\", //OutputFilePath
                "Synology", //DefaultFormat
                "AAC (FDK)", //AudioCodec
                "False", //AAC_Passthru
                "False", //AC3_Passthru
                "False", //DTS_Passthru
                "True", //TwoPass
                "True", //TurboFirstPass
                "True", //Optimize
                "Dolby Prologic 2", //Mixdown
                "96", //AudioBitrateCap
                "Medium", //EncoderSpeed
                "Fast Decode", //EncoderTune
                "High", //EncoderProfile
                "4.0", //EncoderLevel
                "3.5", //VideoBitrateCap
                "Roku Compliant" //Framerate
            };

        
        public ConfigFile()
        {
            configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector";//Writable folder location for config file.
            configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Config.txt"; //Writable file location for config file.

            DefaultSettings = new Dictionary<string, string>();//Dictionary containing default settings
            favoritesList = new List<string>(); //List containing favorite TV show information from config file


            checkConfigFile();
        }
        
        /// <summary>
        /// Checks that configuration file exists and creates it with defaults if it doesn't.
        /// </summary>
        public void checkConfigFile()
        {
            string defaults = "";
            string favorites = "";

            //Check Directory and File, Create if they dont' exist.
            try
            {
                //if (!Directory.Exists(configDirectory)) { Directory.CreateDirectory(configDirectory); }
                new FileInfo(configPath).Directory.Create();
                if (!File.Exists(configPath))
                {
                    using (StreamWriter sw = File.CreateText(configPath)) { }
                }

                //Extract File Text
                using (StreamReader sr = new StreamReader(configPath))
                {
                    configString = sr.ReadToEnd();
                    sr.Close();
                }

                //Create new text for config file
                defaults = GenerateDefaultsString();  //Also generates dictionary of defaults
                favorites = GenerateFavoritesString();

                //Write text file with new text
                using (StreamWriter sw = new StreamWriter(configPath))
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
        public void updateDefaults()
        {
            string start = "<Defaults>\r\n";
            string middle = "";
            string end = "</Defaults>";
            string defaults = "";
            string favorites = "";

            try
            {
                //Extract File Text
                using (StreamReader sr = new StreamReader(configPath))
                {
                    configString = sr.ReadToEnd();
                    sr.Close();
                }

                favorites = "<Favorites>\r\n" + Program.GeneralParser(configString, "<Favorites>", "</Favorites>\r\n");

                //Update configuration file with defaults from dictionary
                for (int i = 0; i < KeyList.Count; i++)
                {
                    middle += "\t<" + KeyList[i] + ">" + DefaultSettings[KeyList[i]] + "</" + KeyList[i] + ">\r\n";
                }

                //Combine string sections to generate section text
                defaults = start + middle + end;

                //Write text file with new text
                using (StreamWriter sw = new StreamWriter(configPath))
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
            string beginning = "<Favorites>";
            string middle = "";
            string end = "</Favorites>";
            favoritesList.Clear();

            //If File is empty fill with default
            if (string.IsNullOrEmpty(configString))
            {
                favorites = "<Favorites>\r\n" +
                   "</Favorites>";
            }
            else //Need to add a check to ensure that if there are no favorites everything still works.
            {
                //filter out section of text file to work with
                favorites = Program.GeneralParser(configString, beginning, end);

                //Sort and create List
                
                favoritesList = Regex.Split(favorites,"\r\n").ToList();
                favoritesList.Sort();

                for (int i = 0; i < favoritesList.Count; i++)
                {
                    middle += favoritesList[i];
                    favoriteTitles.Add(Program.GeneralParser(favoritesList[i], "<TVShow>", "<ShowID>"));
                    favoriteIDs.Add(Program.GeneralParser(favoritesList[i], "<ShowID>", "</TVShow>"));
                }

                //Generate string
                favorites = beginning + middle + end;
            }

            return favorites;
        }
        public void addFavorite(string title, string id)
        {
            string beginning = "<Favorites>\r\n";
            string end = "</Favorites>";
            string middle = "";

            favoriteTitles.Clear();
            favoriteIDs.Clear();
            favoritesList.Clear();

            string defaults = "";
            string favorites = "";

            try
            {
                using (StreamReader sr = new StreamReader(configPath))
                {
                    configString = sr.ReadToEnd();
                    sr.Close();
                }

                defaults = Program.GeneralParser(configString, "<Defaults>", "</Defaults>");
                favorites = Program.GeneralParser(configString, "<Favorites>", "</Favorites>");

                //Create List
                favoritesList = Regex.Split(favorites, "\r\n").ToList();

                //Add new item to list
                favoritesList.Add("\t<TVShow>" + title + "<ShowID>" + id + "</TVShow>\r\n");
                favoritesList.Sort();

                //recreate lists
                for (int i = 0; i < favoritesList.Count; i++)
                {
                    middle += favoritesList[i];
                    favoriteTitles.Add(Program.GeneralParser(favoritesList[i], "<TVShow>", "<ShowID>"));
                    favoriteIDs.Add(Program.GeneralParser(favoritesList[i], "<ShowID>", "</TVShow>"));
                }

                //Generate string
                favorites = beginning + middle + end;

                //Write text file with new text
                using (StreamWriter sw = new StreamWriter(configPath))
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
        public void removeFavorite(string title)
        {
            string beginning = "<Favorites>\r\n";
            string end = "</Favorites>";
            string middle = "";

            favoriteTitles.Clear();
            favoriteIDs.Clear();
            favoritesList.Clear();

            string defaults = "";
            string favorites = "";

            try
            {
                using (StreamReader sr = new StreamReader(configPath))
                {
                    configString = sr.ReadToEnd();
                    sr.Close();
                }

                defaults = Program.GeneralParser(configString, "<Defaults>", "</Defaults>");
                favorites = Program.GeneralParser(configString, "<Favorites>", "</Favorites>");

                //Create List
                favoritesList = Regex.Split(favorites, "\r\n").ToList();
                favoritesList.Sort();

                //recreate lists
                for (int i = 0; i < favoritesList.Count; i++)
                {
                    if(!favoritesList[i].Contains(title)) //Will skip adding title to list thus removing it
                    {
                        middle += favoritesList[i];
                        favoriteTitles.Add(Program.GeneralParser(favoritesList[i], "<TVShow>", "<ShowID>"));
                        favoriteIDs.Add(Program.GeneralParser(favoritesList[i], "<ShowID>", "</TVShow>"));
                    }
                    
                }
                //Generate string
                favorites = beginning + middle + end;

                //Write text file with new text
                using (StreamWriter sw = new StreamWriter(configPath))
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
    }
}
