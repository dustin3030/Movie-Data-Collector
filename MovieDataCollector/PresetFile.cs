using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace MovieDataCollector
{
    class PresetFile
    {
        public string presetDirectory { get; set; }
        public string presetPath { get; set; }
        public List<Dictionary<string, string>> PresetList { get; set; } //Holds values for conversion form presets.
        Dictionary<string, string> presets;
        List<string> presetNames = new List<string>();
        
        string presetString = ""; //Holds file text when it is read in.
        string[,] presetArray = new string[,]
        {
            {"AudioCodec", "AAC (AVC)"},
            {"AudioMixdown", "Dolby ProLogic 2"},
            {"AudioSampleRate","48"},
            {"FilteredAACCheck","false"},
            {"FilteredAC3Check","false"},
            {"FilteredDTSCheck","false"},
            {"AudioBitrate","96"},
            {"EncoderSpeed","Very Fast"},
            {"FrameRateMode","Peak"},
            {"FrameRate","Roku Compliant"},
            {"EncoderTune","Fast Decode"},
            {"VideoBitrate","3.5"},
            {"EncoderProfile","High"},
            {"EncoderLevel","4.0"},
            {"Optimize","true"},
            {"TwoPass","true"},
            {"TurboFirstPass","true"}
        };

        public List<string> keyList = new List<string>()
        {
            "AudioCodec", //AAC (AVC)
            "AudioMixdown", //Dolby ProLogic 2
            "AudioSampleRate", //48
            "FilteredAACCheck", //false
            "FilteredAC3Check", //false
            "FilteredDTSCheck", //false
            "AudioBitrate", //96
            "EncoderSpeed", //Very Fast
            "FrameRateMode", //Peak
            "FrameRate", //Roku Compliant\
            "EncoderTune", //Fast Decode
            "VideoBitrate", //3.5
            "EncoderProfile", //High
            "EncoderLevel", //4.0
            "Optimize", //true
            "TwoPass", //true
            "TurboFirstPass" //true
        };

        List<string> valueList = new List<string>()
        {
            "AAC (AVC)", //AudioCodecAAC
            "Dolby ProLogic 2", //AudioMixdown,
            "48", //AudioSampleRate
            "false", //FilteredAACCheck
            "false", //FilteredAC3Check
            "false", //FilteredDTSCheck
            "96", //AudioBitrate
            "Very Fast", //EncoderSpeed
            "Peak", //FrameRateMode
            "Roku Compliant", //FrameRate
            "Fast Decode", //EncoderTune
            "3.5", //VideoBitrate
            "High", //EncoderProfile
            "4.0", //EncoderLevel
            "true", //Optimize
            "true", //TwoPass
            "true", //TurboFirstPass
        };

        public PresetFile()
        {
            PresetList = new List<Dictionary<string, string>>(); //Instantiate List

            presetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector";//Writable folder location for config file.
            presetPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Presets.txt"; //Writable file location for config file.

            checkPresetFile();
        }
        /// <summary>
        /// Creates preset file if necessary and populates with one preset. Reads in file contents to list of dictionaries
        /// </summary>
        public void checkPresetFile()
        {
            //Check Directory and File, Create if they dont' exist.
            try
            {
                new FileInfo(presetDirectory).Directory.Create();
                if (!File.Exists(presetPath))
                {
                    using (StreamWriter sw = File.CreateText(presetPath)) { }

                    //Write default presets to file since it is empty
                    using (StreamWriter sw = new StreamWriter(presetPath))
                    {
                        sw.WriteLine(CreateDefaultPresetText());
                        sw.Close();
                    }
                    
                }

                //Extract File Text
                using (StreamReader sr = new StreamReader(presetPath))
                {
                    presetString = sr.ReadToEnd();
                    sr.Close();
                }

                //Check that

                //Get presets from file and add to list
                ParsePresets(presetString);

                
            }
            catch (Exception ex)
            {
                CustomMessageBox.Show(ex.ToString(), 300, 300);
            }

        }
        private void ParsePresets(string presetFileString)
        {
            //Split string into different presets and populate list with those splits
            string[] delim = new string[] { "<Preset" };
            string[] Tokens = presetString.Split(delim,StringSplitOptions.None);
            

            //Create Dictionary for each split if it has the values

            for (int i = 0; i < Tokens.Count(); i++)
            {
                //Check information
                if(Tokens[i].Contains("<AudioCodec>"))
                {
                    //Parse for Preset Name
                    string Pname = Program.GeneralParser(Tokens[i], "_", ">");
                    presetNames.Add(Pname);

                    //Create Dictionary
                    presets = new Dictionary<string, string>();

                    //Add Preset Name to dictionary
                    presets.Add("Name", Pname);

                    //Parse Dictionary Items
                    for (int a = 0; a < keyList.Count(); a++)
                    {
                        string parsedValue = Program.GeneralParser(Tokens[i], "<" + keyList[a] + ">", "</" + keyList[a] + ">");
                        if (!string.IsNullOrEmpty(parsedValue))
                        {
                            presets.Add(keyList[a], parsedValue);
                        }
                    }
                    //Add Dictionary to List
                    PresetList.Add(presets);
                }
            }
        }
        private string CreateDefaultPresetText()
        {
            StringBuilder presetFileString = new StringBuilder();

            presetFileString.Append("<Preset_RokuCompliant>\r\n");
            for (int i = 0; i < keyList.Count(); i++)
            {
                presetFileString.Append("\t<" + keyList[i] + ">" + valueList[i] + "</" + keyList[i] + ">\r\n");
            }
            presetFileString.Append("</Preset_RokuCompliant>\r\n");

            return presetFileString.ToString();
        }
        public void UpdatePresets()
        {
            StringBuilder presetFileString = new StringBuilder();
            //Create String to write to file
            for (int i = 0; i < PresetList.Count(); i++)
            {
                presetFileString.Append("<Preset_" + presetNames[i] + ">\r\n");
                for (int a = 0; a < keyList.Count(); a++)
                {
                    presetFileString.Append("\t<" + keyList[a] + ">" + PresetList[i][keyList[a]] + "</" + keyList[a] + ">\r\n");
                }
                presetFileString.Append("</Preset_" + presetNames[i] + ">\r\n");
            }

            //Write text file with new text
            using (StreamWriter sw = new StreamWriter(presetPath))
            {
                sw.WriteLine(presetFileString);
                sw.Close();
            }

        }
        public void AddPreset(Dictionary<string,string> NewPreset)
        {
            //Add Preset Name
            presetNames.Add(NewPreset["Name"]);
            PresetList.Add(NewPreset);
            UpdatePresets();
        }
        public void RemovePreset(string PName)
        {
            int itemToRemove = -1;
            for (int i = 0; i < PresetList.Count(); i++)
            {
                if(PresetList[i]["Name"] == PName)
                {
                    itemToRemove = i;
                }
            }
            if(itemToRemove != -1)
            {
                PresetList.RemoveAt(itemToRemove);
                presetNames.RemoveAt(itemToRemove);
            }

            UpdatePresets();

        }

    }
}
