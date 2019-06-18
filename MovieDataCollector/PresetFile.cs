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
        public string PresetDirectory { get; set; }
        public string PresetPath { get; set; }
        public List<Dictionary<string, string>> PresetList { get; set; } //Holds values for conversion form presets.
        public List<Dictionary<string, string>> PresetListSorter { get; set; } // Holds sorted presets
        Dictionary<string, string> presets;
        public List<string> presetNames = new List<string>();
        
        string presetString = ""; //Holds file text when it is read in.

        public List<string> keyList = new List<string>()
        {
            "AudioCodec", //AAC (FDK)
            "AudioMixdown", //Dolby ProLogic 2
            "AudioSampleRate", //48
            "FilteredAACCheck", //false
            "FilteredAC3Check", //false
            "FilteredEAC3Check", //false
            "FilteredDTSCheck", //false
            "FilteredDTSHDCheck", //false
            "FilteredTrueHDCheck", //false
            "FilteredMP3Check", //false
            "FilteredFLACCheck", //false
            "AudioBitrate", //128
            "AudioGain", //0

            "EnableTrack2", //False
            "AudioCodec2", //E-AC3
            "AudioMixdown2", //5.1
            "AudioSampleRate2", //48
            "FilteredAACCheck2", //false
            "FilteredAC3Check2", //false
            "FilteredEAC3Check2", //false
            "FilteredDTSCheck2", //false
            "FilteredDTSHDCheck2", //false
            "FilteredTrueHDCheck2", //false
            "FilteredMP3Check2", //false
            "FilteredFLACCheck2", //false
            "AudioBitrate2", //128
            "AudioGain2", //0

            "EnableTrack3", //False
            "AudioCodec3", //Filtered Passthru
            "AudioMixdown3", //5.1
            "AudioSampleRate3", //48
            "FilteredAACCheck3", //false
            "FilteredAC3Check3", //false
            "FilteredEAC3Check3", //false
            "FilteredDTSCheck3", //false
            "FilteredDTSHDCheck3", //false
            "FilteredTrueHDCheck3", //false
            "FilteredMP3Check3", //false
            "FilteredFLACCheck3", //false
            "AudioBitrate3", //128
            "AudioGain3", //0

            "Encoder", //x264 (FFMPEG)
            "EncoderSpeed", //Very Fast
            "FrameRateMode", //Peak
            "FrameRate", //Roku Compliant\
            "EncoderTune", //Fast Decode
            "VideoBitrate", //3.5
            "EncoderProfile", //Main
            "EncoderLevel", //4.0
            "Optimize", //true
            "AutoCrop", //true
            "TwoPass", //true
            "TurboFirstPass", //true
            "SubtitleSelection", //None for TV, All For Movie
            "ForcedSubtitleBurnIn" //False for TV, True for Movie
        };

        List<string> TVPresetValueList = new List<string>()
        {
            "AAC (FDK)", //AudioCodecAAC
            "Dolby ProLogic 2", //AudioMixdown,
            "48", //AudioSampleRate
            "false", //FilteredAACCheck
            "false", //FilteredAC3Check
            "false", //FilteredEAC3Check
            "false", //FilteredDTSCheck
            "false", //FilteredDTSHDCheck
            "false", //FilteredTrueHDCheck
            "false", //FilteredMP3Check
            "false", //FilteredFLACCheck
            "128", //AudioBitrate
            "10", //AudioGain

            "true", //EnableTrack2
            "E-AC3", //AudioCodecAAC2
            "5.1 Audio", //AudioMixdown2
            "48", //AudioSampleRate2
            "false", //FilteredAACCheck2
            "false", //FilteredAC3Check2
            "false", //FilteredEAC3Check2
            "false", //FilteredDTSCheck2
            "false", //FilteredDTSHDCheck2
            "false", //FilteredTrueHDCheck2
            "false", //FilteredMP3Check2
            "false", //FilteredFLACCheck2
            "128", //AudioBitrate2
            "0", //AudioGain2

            "false", //EnableTrack3
            "Filtered Passthru", //AudioCodecAAC3
            "5.1 Audio", //AudioMixdown3
            "48", //AudioSampleRate3
            "false", //FilteredAACCheck3
            "false", //FilteredAC3Check3
            "false", //FilteredEAC3Check3
            "false", //FilteredDTSCheck3
            "false", //FilteredDTSHDCheck3
            "false", //FilteredTrueHDCheck3
            "false", //FilteredMP3Check3
            "false", //FilteredFLACCheck3
            "128", //AudioBitrate3
            "0", //AudioGain3

            "x264 (FFMPEG)", //Encoder
            "Very Fast", //EncoderSpeed
            "Peak", //FrameRateMode
            "23.976", //FrameRate
            "Fast Decode", //EncoderTune
            "3.5", //VideoBitrate
            "Main", //EncoderProfile
            "4.0", //EncoderLevel
            "true", //Optimize
            "false", //AutoCrop
            "true", //TwoPass
            "true", //TurboFirstPass
            "English", //SubtitleSelection
            "True" //ForcedSubtitlesBurnIn
        };

        List<string> MoviePresetValueList = new List<string>()
        {
            "AAC (FDK)", //AudioCodecAAC
            "Dolby ProLogic 2", //AudioMixdown,
            "48", //AudioSampleRate
            "false", //FilteredAACCheck
            "false", //FilteredAC3Check
            "false", //FilteredEAC3Check
            "false", //FilteredDTSCheck
            "false", //FilteredDTSHDCheck
            "false", //FilteredTrueHDCheck
            "false", //FilteredMP3Check
            "false", //FilteredFLACCheck
            "128", //AudioBitrate
            "10", //AudioGain

            "true", //EnableTrack2
            "E-AC3", //AudioCodecAAC2
            "5.1 Audio", //AudioMixdown2
            "48", //AudioSampleRate2
            "false", //FilteredAACCheck2
            "false", //FilteredAC3Check2
            "false", //FilteredEAC3Check2
            "false", //FilteredDTSCheck2
            "false", //FilteredDTSHDCheck2
            "false", //FilteredTrueHDCheck2
            "false", //FilteredMP3Check2
            "false", //FilteredFLACCheck2
            "128", //AudioBitrate2
            "0", //AudioGain2

            "false", //EnableTrack3
            "Filtered Passthru", //AudioCodecAAC3
            "5.1 Audio", //AudioMixdown3
            "48", //AudioSampleRate3
            "false", //FilteredAACCheck3
            "true", //FilteredAC3Check3
            "true", //FilteredEAC3Check3
            "true", //FilteredDTSCheck3
            "true", //FilteredDTSHDCheck3
            "true", //FilteredTrueHDCheck3
            "false", //FilteredMP3Check3
            "false", //FilteredFLACCheck3
            "128", //AudioBitrate3
            "0", //AudioGain3

            "x264 (FFMPEG)", //Encoder
            "Medium", //EncoderSpeed
            "Peak", //FrameRateMode
            "23.976", //FrameRate
            "Fast Decode", //EncoderTune
            "5.5", //VideoBitrate
            "Main", //EncoderProfile
            "4.0", //EncoderLevel
            "true", //Optimize
            "true", //AutoCrop
            "true", //TwoPass
            "true", //TurboFirstPass
            "All", //SubtitleSelection
            "True" //ForcedSubtitlesBurnIn
        };

        List<string> TabletPresetValueList = new List<string>()
        {
            "AAC (FDK)", //AudioCodecAAC
            "Dolby ProLogic 2", //AudioMixdown,
            "48", //AudioSampleRate
            "false", //FilteredAACCheck
            "false", //FilteredAC3Check
            "false", //FilteredEAC3Check
            "false", //FilteredDTSCheck
            "false", //FilteredDTSHDCheck
            "false", //FilteredTrueHDCheck
            "false", //FilteredMP3Check
            "false", //FilteredFLACCheck
            "128", //AudioBitrate
            "0", //AudioGain

            "false", //EnableTrack2
            "E-AC3", //AudioCodecAAC2
            "5.1 Audio", //AudioMixdown2
            "48", //AudioSampleRate2
            "false", //FilteredAACCheck2
            "false", //FilteredAC3Check2
            "false", //FilteredEAC3Check2
            "false", //FilteredDTSCheck2
            "false", //FilteredDTSHDCheck2
            "false", //FilteredTrueHDCheck2
            "false", //FilteredMP3Check2
            "false", //FilteredFLACCheck2
            "128", //AudioBitrate2
            "0", //AudioGain2

            "false", //EnableTrack3
            "Filtered Passthru", //AudioCodecAAC3
            "5.1 Audio", //AudioMixdown3
            "48", //AudioSampleRate3
            "false", //FilteredAACCheck3
            "true", //FilteredAC3Check3
            "true", //FilteredEAC3Check3
            "true", //FilteredDTSCheck3
            "true", //FilteredDTSHDCheck3
            "true", //FilteredTrueHDCheck3
            "false", //FilteredMP3Check3
            "false", //FilteredFLACCheck3
            "128", //AudioBitrate3
            "0", //AudioGain3

            "x264 (FFMPEG)", //Encoder
            "Very Fast", //EncoderSpeed
            "Peak", //FrameRateMode
            "23.976", //FrameRate
            "Fast Decode", //EncoderTune
            "3", //VideoBitrate
            "Main", //EncoderProfile
            "4.0", //EncoderLevel
            "false", //Optimize
            "true", //AutoCrop
            "false", //TwoPass
            "false", //TurboFirstPass
            "All", //SubtitleSelection
            "True" //ForcedSubtitlesBurnIn
        };

        List<string> UHDPresetValueList = new List<string>()
        {
            "AAC (FDK)", //AudioCodecAAC
            "Dolby ProLogic 2", //AudioMixdown,
            "48", //AudioSampleRate
            "false", //FilteredAACCheck
            "false", //FilteredAC3Check
            "false", //FilteredEAC3Check
            "false", //FilteredDTSCheck
            "false", //FilteredDTSHDCheck
            "false", //FilteredTrueHDCheck
            "false", //FilteredMP3Check
            "false", //FilteredFLACCheck
            "128", //AudioBitrate
            "10", //AudioGain

            "true", //EnableTrack2
            "E-AC3", //AudioCodecAAC2
            "5.1 Audio", //AudioMixdown2
            "48", //AudioSampleRate2
            "false", //FilteredAACCheck2
            "false", //FilteredAC3Check2
            "false", //FilteredEAC3Check2
            "false", //FilteredDTSCheck2
            "false", //FilteredDTSHDCheck2
            "false", //FilteredTrueHDCheck2
            "false", //FilteredMP3Check2
            "false", //FilteredFLACCheck2
            "128", //AudioBitrate2
            "0", //AudioGain2

            "true", //EnableTrack3
            "Filtered Passthru", //AudioCodecAAC3
            "5.1 Audio", //AudioMixdown3
            "48", //AudioSampleRate3
            "false", //FilteredAACCheck3
            "true", //FilteredAC3Check3
            "true", //FilteredEAC3Check3
            "true", //FilteredDTSCheck3
            "true", //FilteredDTSHDCheck3
            "true", //FilteredTrueHDCheck3
            "false", //FilteredMP3Check3
            "false", //FilteredFLACCheck3
            "128", //AudioBitrate3
            "0", //AudioGain3

            "x265 (FFMPEG)", //Encoder
            "Medium", //EncoderSpeed
            "Peak", //FrameRateMode
            "23.976", //FrameRate
            "Fast Decode", //EncoderTune
            "10", //VideoBitrate
            "High", //EncoderProfile
            "4.1", //EncoderLevel
            "true", //Optimize
            "true", //AutoCrop
            "true", //TwoPass
            "true", //TurboFirstPass
            "All", //SubtitleSelection
            "True" //ForcedSubtitlesBurnIn
        };

        public PresetFile()
        {
            PresetList = new List<Dictionary<string, string>>(); //Instantiate List
            PresetListSorter = new List<Dictionary<string, string>>(); //Instantiate List

            PresetDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector";//Writable folder location for config file.
            PresetPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Presets.txt"; //Writable file location for config file.

            CheckPresetFile();

        }
        /// <summary>
        /// Creates preset file if necessary and populates with one preset. Reads in file contents to list of dictionaries
        /// </summary>
        public void CheckPresetFile()
        {
            //Check Directory and File, Create if they dont' exist.
            try
            {
                new FileInfo(PresetDirectory).Directory.Create();
                if (!File.Exists(PresetPath))
                {
                    using (StreamWriter sw = File.CreateText(PresetPath)) { }

                    //Write default presets to file since it is empty
                    using (StreamWriter sw = new StreamWriter(PresetPath))
                    {
                        sw.WriteLine(CreateDefaultPresetText());
                        sw.Close();
                    }
                    
                }
                else //File does exist, check contents
                {
                    using (StreamReader sr = new StreamReader(PresetPath))
                    {
                        presetString = sr.ReadToEnd();
                        sr.Close();
                    }

                    //Check that string contains all the keys of the dictionary
                    bool valueMissing = false;
                    for (int i = 0; i < keyList.Count(); i++)
                    {
                        if(!valueMissing)
                        {
                            if (string.IsNullOrEmpty(Program.GeneralParser(presetString, "<" + keyList[i] + ">", "</" + keyList[i] + ">")))
                            {
                                valueMissing = true;
                            }
                        }
                    }
                    if(valueMissing) //if a value is missing create new file with defaults.
                    {
                        using (StreamWriter sw = new StreamWriter(PresetPath))
                        {
                            sw.WriteLine(CreateDefaultPresetText());
                            sw.Close();
                        }
                    }
                    
                }

                //Extract File Text
                using (StreamReader sr = new StreamReader(PresetPath))
                {
                    presetString = sr.ReadToEnd();
                    sr.Close();
                }

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
#pragma warning disable IDE0028 // Simplify collection initialization
                    presets = new Dictionary<string, string>();
#pragma warning restore IDE0028 // Simplify collection initialization

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
                        else
                        {
                            presets.Add(keyList[a], ""); //add empty string if value isn't found in the file.
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

            //TV Presets
            presetFileString.Append("<Preset_TV>\r\n");
            for (int i = 0; i < keyList.Count(); i++)
            {
                presetFileString.Append("\t<" + keyList[i] + ">" + TVPresetValueList[i] + "</" + keyList[i] + ">\r\n");
            }
            presetFileString.Append("</Preset_TV>\r\n");

            //Movie Presets
            presetFileString.Append("<Preset_Movie>\r\n");
            for (int i = 0; i < keyList.Count(); i++)
            {
                presetFileString.Append("\t<" + keyList[i] + ">" + MoviePresetValueList[i] + "</" + keyList[i] + ">\r\n");
            }
            presetFileString.Append("</Preset_Movie>\r\n");

            //Tablet Presets
            presetFileString.Append("<Preset_Tablet>\r\n");
            for (int i = 0; i < keyList.Count(); i++)
            {
                presetFileString.Append("\t<" + keyList[i] + ">" + TabletPresetValueList[i] + "</" + keyList[i] + ">\r\n");
            }
            presetFileString.Append("</Preset_Tablet>\r\n");

            //UHD Presets
            presetFileString.Append("<Preset_UHD>\r\n");
            for (int i = 0; i < keyList.Count(); i++)
            {
                presetFileString.Append("\t<" + keyList[i] + ">" + UHDPresetValueList[i] + "</" + keyList[i] + ">\r\n");
            }
            presetFileString.Append("</Preset_UHD>\r\n");


            return presetFileString.ToString();
        }
        public void UpdatePresets()
        {
            StringBuilder presetFileString = new StringBuilder();

            SortPresets();
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
            using (StreamWriter sw = new StreamWriter(PresetPath))
            {
                sw.WriteLine(presetFileString);
                sw.Close();
            }

        }
        public void AddPreset(Dictionary<string,string> NewPreset)
        {
            //Check preset doesn't already exist
            if(!presetNames.Contains(NewPreset["Name"]))
            {
                //Add Preset Name
                presetNames.Add(NewPreset["Name"]);
                PresetList.Add(NewPreset);
                UpdatePresets();
            }
            else
            {
                //Remove the preset first
                RemovePreset(NewPreset["Name"]);

                //Add Preset Name
                presetNames.Add(NewPreset["Name"]);
                PresetList.Add(NewPreset);
                UpdatePresets();
            }
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
        public void SortPresets()
        {
            PresetListSorter.Clear();
            //take name list, sort it
            presetNames.Sort();

            //Loop through sorted name list and match dictionary entry
            for (int i = 0; i < presetNames.Count(); i++)
            {
                for (int a = 0; a < PresetList.Count(); a++)
                {
                    if(PresetList[a]["Name"] == presetNames[i])
                    {
                        //Add matched dictionary to new list containing dictionaries
                        PresetListSorter.Add(PresetList[a]);
                    }
                }
            }

            //Add presets back to list
            PresetList.Clear();

            for (int i = 0; i < PresetListSorter.Count(); i++)
            {
                PresetList.Add(PresetListSorter[i]);
            }
        }
    }
}
