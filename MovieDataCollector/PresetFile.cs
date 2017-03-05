using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;

namespace MovieDataCollector
{
    class PresetFile
    {
        public string presetDirectory { get; set; }
        public string presetPath { get; set; }
        public List<Dictionary<string, string>> PresetList { get; set; } //Holds values for conversion form presets.
        Dictionary<string, string> presets;
        string presetString = ""; //Holds file text when it is read in.
        string[,] presetArray = new string[,]
        {
            {"AudioCodecAAC", "AAC (AVC)"},
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

        List<string> keyList = new List<string>()
        {
            "AudioCodecAAC", //AAC (AVC)
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
            presetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector";//Writable folder location for config file.
            presetPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Presets.txt"; //Writable file location for config file.
        }
        public void checkPresetFile()
        {

            //Check Directory and File, Create if they dont' exist.
            try
            {
                new FileInfo(presetPath).Directory.Create();
                if (!File.Exists(presetPath))
                {
                    using (StreamWriter sw = File.CreateText(presetPath)) { }
                }

                //Extract File Text
                using (StreamReader sr = new StreamReader(presetPath))
                {
                    presetString = sr.ReadToEnd();
                    sr.Close();
                }

                //Get presets from file and add to list
                ParsePresets(presetString);

                //Write text file with new text
                using (StreamWriter sw = new StreamWriter(presetPath))
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
        private void ParsePresets(string presetFileString)
        {
            //Split string into different presets and populate list with those splits
            char delim = '\\';
            string[] Tokens = SFD.FileName.Split(delim);
            CF.DefaultSettings["ExportFilePath"] = ""; //Clear Path

            for (int i = 0; i < Tokens.Count() - 1; i++)
            {
                CF.DefaultSettings["ExportFilePath"] += Tokens[i].ToString() + "\\"; //Sets the default directory for exporting text file.
            }


        }
    }


}
