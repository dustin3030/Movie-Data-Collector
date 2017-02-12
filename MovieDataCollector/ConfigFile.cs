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
            favoriteTitles= new List<string>(); //instantiate list
            favoriteIDs = new List<string>(); //instantiate list

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

                favorites = returnFavorites();

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
            string beginning = "<Favorites>\r\n";
            string middle = "";
            string end = "</Favorites>";
            favoritesList.Clear();
            favoriteTitles.Clear();
            favoriteIDs.Clear();

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
                
                favoritesList = Regex.Split(favorites,"\r\n").ToList();
                favoritesList.Sort();

                for (int i = 0; i < favoritesList.Count; i++)
                {
                    middle += favoritesList[i] + "\r\n";
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


            checkConfigFile(); //updates variable information

            favoriteTitles.Clear();
            favoriteIDs.Clear();
            favoritesList.Clear();

            

            string defaults = "";
            string favorites = "";

            try
            {

                defaults = returnDefaults();
                favorites = returnFavorites();
                favorites = favorites.Replace("<Favorites>\r\n", "");
                favorites = favorites.Replace("</Favorites>", "");

                //Create List
                favoritesList = Regex.Split(favorites, "\r\n").ToList();

                //Check for duplicates and Add new item to list
                if(!favoritesList.Contains("\t<TVShow>" + title + "<ShowID>" + id + "</TVShow>"))
                {
                    favoritesList.Add("\t<TVShow>" + title + "<ShowID>" + id + "</TVShow>");
                }

                favoritesList.Sort();

                //recreate lists
                for (int i = 0; i < favoritesList.Count; i++)
                {
                    if(!string.IsNullOrEmpty(Program.GeneralParser(favoritesList[i], "<TVShow>", "<ShowID>")))
                    {
                        middle += favoritesList[i] + "\r\n";
                        favoriteTitles.Add(Program.GeneralParser(favoritesList[i], "<TVShow>", "<ShowID>"));
                        favoriteIDs.Add(Program.GeneralParser(favoritesList[i], "<ShowID>", "</TVShow>"));
                    }
                }


                //Rebuild FavoritesList
                favoritesList.Clear();
                for (int i = 0; i < favoriteTitles.Count; i++)
                {
                    favoritesList.Add("\t<TVShow>" + favoriteTitles[i] + "<ShowID>" + favoriteIDs[i] + "</TVShow>\r\n");
                }

                //Generate string
                favorites = beginning + middle + end;
                //Write text file with new text
                using (StreamWriter sw = new StreamWriter(configPath))
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
        public void removeFavorite(int index)
        {
            string beginning = "<Favorites>\r\n";
            string end = "</Favorites>";
            string middle = "";
            string favorites = "";
            string defaults = returnDefaults();

            favoritesList.RemoveAt(index);
            favoriteTitles.RemoveAt(index);
            favoriteIDs.RemoveAt(index);

            for (int i = 0; i < favoritesList.Count; i++)
            {
                middle += favoritesList[i] + "\r\n";
            }

            //Generate string
            favorites = beginning + middle + end;
            try
            {
                //Write text file with new text
                using (StreamWriter sw = new StreamWriter(configPath))
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
        private string returnFavorites()
        {
            return "<Favorites>\r\n" + Program.GeneralParser(configString, "<Favorites>\r\n", "</Favorites>") + "</Favorites>";
        }
        private string returnDefaults()
        {
            return "<Defaults>\r\n" + Program.GeneralParser(configString, "<Defaults>\r\n", "</Defaults>") + "</Defaults>\r\n";
        }
    }
}
