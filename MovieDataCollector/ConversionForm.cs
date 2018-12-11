﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics; //Allows for using Process.Start codes lines
using System.Drawing;
using System.IO; //allows for file manipulation
using System.Text;
using System.Net.Mail; // For Notification
using MediaInfoNET; /* http://teejeetech.blogspot.com/2013/01/mediainfo-wrapper-for-net-projects.html Copyright (c) 2013 Tony George (teejee2008@gmail.com)
                      GNU General Public License version 2.0 (GPLv2)
                      Downloaded Wrapper for returning media info from files.
                      Need to have both the wrapper (MediaInfoNet.dll) and the DLL (MediaInfo.dll) saved in the
                      Application folder (Release or Debug) or it will not work, Add MediaInfoNet.dll as a reference through the project menu*/
using System.Threading.Tasks;

namespace MovieDataCollector
{
    public partial class ConversionForm : Form
    {
        //string folderPath = ""; //Contains path for parent directory
        List<string> VideoFilesList = new List<string>(); //Contains File Paths for video files 
        StringBuilder incompatible = new StringBuilder();
        string separator = "========================================================================\n"; //When the scroll bar in a few characters get squished onto the next line. Don't modify unless you change the size of the control.
        string separator2 = "\n.........................................................................................................................\r\n \r\n";
        List<string> IncompatibilityInfo = new List<string>(); //Contains Incompatibility info for each file listed in VideoFilesList

        //string ConfigDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector"; //Writable folder location for config file.
        //string ConfigPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Config.txt"; //Writable file location for config file.

        //Ending audio bitrate string used in encoder and setting video bitrate buffer and maxrate size.
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

        List<string> subtitleComboList = new List<string>()
        {
            "None",
            "All",
            "Default",
            "First",
            "Chinese",
            "Czech",
            "English",
            "Finnish",
            "French",
            "German",
            "Greek",
            "Japanese",
            "Korean",
            "Portuguese",
            "Russian",
            "Spanish",
            "Swedish"
        };

        /*Values available for conversion*/
        List<string> codecList = new List<string>()
        {
            "AAC (FDK)", //Default
            "Filtered Passthru",
            "AC3",
            "E-AC3"
        };

        List<string> mixdownList = new List<string>()
        {
            "Dolby ProLogic 2", //Default
            "5.1 Audio"
        };

        List<string> audioBitrateCapList = new List<string>()
        {
            "128", //Default
            "32",
            "40",
            "48",
            "56",
            "64",
            "80",
            "96", 
            "112",
            "128", //Highest bitrate supported by Roku
            "160",
            "224",
            "256"
        };

        List<string> encoderSpeedList = new List<string>()
        {
            "Very Fast", //Default
            "Ultra Fast",
            "Super Fast",
            "Faster",
            "Fast",
            "Medium", 
            "Slow",
            "Slower",
            "Very Slow",
            "Placebo"
        };

        List<string> encoderTuneList = new List<string>()
        {
            "Fast Decode", //Default
            "Animation",
            "Film",
            "Grain",
            "None",
            "Still Image",
            "Zero Latency"
        };

        List<string> encoderProfileList = new List<string>()
        {
            "High", //Default
            "Baseline",
            "Main"
        };

        List<string> encoderLevelList = new List<string>()
        {
            "4.1", //Default
            "Auto",
            "1.0",
            "1b",
            "1.1",
            "1.2",
            "1.3",
            "2.0",
            "2.1",
            "2.2",
            "3.0",
            "3.1",
            "3.2",
            "4.0",
            "4.2",
            "5.0",
            "5.1",
            "5.2"
        };

        List<string> videoBitrateCapList = new List<string>()
        {
            "3.5", //Default
            ".5",
            "1",
            "1.5",
            "2",
            "2.5",
            "3",
            "4",
            "4.5",
            "5",
            "5.5",
            "6",
            "6.5",
            "7",
            "7.5",
            "8",
            "8.5",
            "9",
            "9.5",
            "10" //bitrates over 10mbps are not supported by Roku Devices.
        };

        List<string> frameRateModeList = new List<string>()
        {
            "Constant",
            "Peak",
            "Variable"
        };

        List<string> framerateList = new List<string>()
        {
            "Roku Compliant", //Default
            "Same As Source",
            "5",
            "10",
            "12",
            "15",
            "23.976", //Roku Compatible
            "24",
            "25",
            "29.97", //Roku Compatible
            "30",
            "50",
            "59.94",
            "60"
        };
        //Create object containing Configuration information
        ConfigFile CF = new ConfigFile();
        //Create object containing preset information
        PresetFile PF = new PresetFile();
        public ConversionForm()
        {
            InitializeComponent(); //Initializes components.
            ApplyConfigDefaults();
            PopulatePresets();
            ApplyPreset(); // Applies the preset corresponding to the text in the preset combobox.
        }
        private void ApplyConfigDefaults() //Sets encode options to values from file
        {
            /*Preset*/
            presetComboBox.Text = CF.DefaultSettings["ConversionPreset"];

            /*Compatibility Selection*/
            if (CF.DefaultSettings["CompatibilitySelector"] == "Roku"){ compatibilityCombo.SelectedIndex = 0; }
            if (CF.DefaultSettings["CompatibilitySelector"] == "Xbox") { compatibilityCombo.SelectedIndex = 1; }

            /*Audio Settings*/
            /*Track 1*******************************************************************************************************************************************************/
            audioCodecComboBox.Text = CF.DefaultSettings["AudioCodec"];
            mixdownComboBox.Text = CF.DefaultSettings["Mixdown"];
            audioBitrateCombo.Text = CF.DefaultSettings["AudioBitrateCap"];
            sampleRateCombo.Text = CF.DefaultSettings["AudioSampleRate"];

            if (CF.DefaultSettings["AudioCodec"] == "Filtered Passthru")
            {
                filteredAACCheck.Visible = true;
                filteredAC3Check.Visible = true;
                filteredEAC3Check.Visible = true;
                filteredDTSCheck.Visible = true;
                filteredDTSHDCheck.Visible = true;
                filteredTrueHDCheck.Visible = true;
                filteredMP3Check.Visible = true;
                filteredFLACCheck.Visible = true;
                passthruFilterLabel.Visible = true;
            }
            else
            {
                filteredAACCheck.Visible = false;
                filteredAC3Check.Visible = false;
                filteredEAC3Check.Visible = false;
                filteredDTSCheck.Visible = false;
                filteredDTSHDCheck.Visible = false;
                filteredTrueHDCheck.Visible = false;
                filteredMP3Check.Visible = false;
                filteredFLACCheck.Visible = false;
                passthruFilterLabel.Visible = false;
            }

            if (CF.DefaultSettings["AAC_Passthru"] == "True") { filteredAACCheck.Checked = true; } else { filteredAACCheck.Checked = false; }
            if (CF.DefaultSettings["AC3_Passthru"] == "True") { filteredAC3Check.Checked = true; } else { filteredAC3Check.Checked = false; }
            if (CF.DefaultSettings["EAC3_Passthru"] == "True") { filteredEAC3Check.Checked = true; } else { filteredEAC3Check.Checked = false; }
            if (CF.DefaultSettings["DTS_Passthru"] == "True") { filteredDTSCheck.Checked = true; } else { filteredDTSCheck.Checked = false; }
            if (CF.DefaultSettings["DTSHD_Passthru"] == "True") { filteredDTSHDCheck.Checked = true; } else { filteredDTSHDCheck.Checked = false; }
            if (CF.DefaultSettings["TrueHD_Passthru"] == "True") { filteredTrueHDCheck.Checked = true; } else { filteredTrueHDCheck.Checked = false; }
            if (CF.DefaultSettings["MP3_Passthru"] == "True") { filteredMP3Check.Checked = true; } else { filteredMP3Check.Checked = false; }
            if (CF.DefaultSettings["FLAC_Passthru"] == "True") { filteredFLACCheck.Checked = true; } else { filteredFLACCheck.Checked = false; }

            /*Track 2*******************************************************************************************************************************************************/
            audioCodecComboBox2.Text = CF.DefaultSettings["AudioCodec2"];
            mixdownComboBox2.Text = CF.DefaultSettings["Mixdown2"];
            audioBitrateCombo2.Text = CF.DefaultSettings["AudioBitrateCap2"];
            sampleRateCombo2.Text = CF.DefaultSettings["AudioSampleRate2"];

            

            if (CF.DefaultSettings["AAC_Passthru2"] == "True") { filteredAACCheck2.Checked = true; } else { filteredAACCheck2.Checked = false; }
            if (CF.DefaultSettings["AC3_Passthru2"] == "True") { filteredAC3Check2.Checked = true; } else { filteredAC3Check2.Checked = false; }
            if (CF.DefaultSettings["EAC3_Passthru2"] == "True") { filteredEAC3Check2.Checked = true; } else { filteredEAC3Check2.Checked = false; }
            if (CF.DefaultSettings["DTS_Passthru2"] == "True") { filteredDTSCheck2.Checked = true; } else { filteredDTSCheck2.Checked = false; }
            if (CF.DefaultSettings["DTSHD_Passthru2"] == "True") { filteredDTSHDCheck2.Checked = true; } else { filteredDTSHDCheck2.Checked = false; }
            if (CF.DefaultSettings["TrueHD_Passthru2"] == "True") { filteredTrueHDCheck2.Checked = true; } else { filteredTrueHDCheck2.Checked = false; }
            if (CF.DefaultSettings["MP3_Passthru2"] == "True") { filteredMP3Check2.Checked = true; } else { filteredMP3Check2.Checked = false; }
            if (CF.DefaultSettings["FLAC_Passthru2"] == "True") { filteredFLACCheck2.Checked = true; } else { filteredFLACCheck2.Checked = false; }

            if (CF.DefaultSettings["EnableTrack2"] == "True")
            {
                disableCheckStream2.Checked = false;
                audioCodecComboBox2.Enabled = true;
                mixdownComboBox2.Enabled = true;
                sampleRateCombo2.Enabled = true;
                audioBitrateCombo2.Enabled = true;

                if (CF.DefaultSettings["AudioCodec2"] == "Filtered Passthru")
                {
                    filteredAACCheck2.Visible = true;
                    filteredAC3Check2.Visible = true;
                    filteredEAC3Check2.Visible = true;
                    filteredDTSCheck2.Visible = true;
                    filteredDTSHDCheck2.Visible = true;
                    filteredTrueHDCheck2.Visible = true;
                    filteredMP3Check2.Visible = true;
                    filteredFLACCheck2.Visible = true;
                    passthruFilterLabel2.Visible = true;
                }
                else
                {
                    filteredAACCheck2.Visible = false;
                    filteredAC3Check2.Visible = false;
                    filteredEAC3Check2.Visible = false;
                    filteredDTSCheck2.Visible = false;
                    filteredDTSHDCheck2.Visible = false;
                    filteredTrueHDCheck2.Visible = false;
                    filteredMP3Check2.Visible = false;
                    filteredFLACCheck2.Visible = false;
                    passthruFilterLabel2.Visible = false;
                }
            }
            else
            {
                disableCheckStream2.Checked = true;
                audioCodecComboBox2.Enabled = false;
                mixdownComboBox2.Enabled = false;
                sampleRateCombo2.Enabled = false;
                audioBitrateCombo2.Enabled = false;

                filteredAACCheck2.Visible = false;
                filteredAC3Check2.Visible = false;
                filteredEAC3Check2.Visible = false;
                filteredDTSCheck2.Visible = false;
                filteredDTSHDCheck2.Visible = false;
                filteredTrueHDCheck2.Visible = false;
                filteredMP3Check2.Visible = false;
                filteredFLACCheck2.Visible = false;
                passthruFilterLabel2.Visible = false;

            }


            /*Track 3*******************************************************************************************************************************************************/
            audioCodecComboBox3.Text = CF.DefaultSettings["AudioCodec3"];
            mixdownComboBox3.Text = CF.DefaultSettings["Mixdown3"];
            audioBitrateCombo3.Text = CF.DefaultSettings["AudioBitrateCap3"];
            sampleRateCombo3.Text = CF.DefaultSettings["AudioSampleRate3"];



            if (CF.DefaultSettings["AAC_Passthru3"] == "True") { filteredAACCheck3.Checked = true; } else { filteredAACCheck3.Checked = false; }
            if (CF.DefaultSettings["AC3_Passthru3"] == "True") { filteredAC3Check3.Checked = true; } else { filteredAC3Check3.Checked = false; }
            if (CF.DefaultSettings["EAC3_Passthru3"] == "True") { filteredEAC3Check3.Checked = true; } else { filteredEAC3Check3.Checked = false; }
            if (CF.DefaultSettings["DTS_Passthru3"] == "True") { filteredDTSCheck3.Checked = true; } else { filteredDTSCheck3.Checked = false; }
            if (CF.DefaultSettings["DTSHD_Passthru3"] == "True") { filteredDTSHDCheck3.Checked = true; } else { filteredDTSHDCheck3.Checked = false; }
            if (CF.DefaultSettings["TrueHD_Passthru3"] == "True") { filteredTrueHDCheck3.Checked = true; } else { filteredTrueHDCheck3.Checked = false; }
            if (CF.DefaultSettings["MP3_Passthru3"] == "True") { filteredMP3Check3.Checked = true; } else { filteredMP3Check3.Checked = false; }
            if (CF.DefaultSettings["FLAC_Passthru3"] == "True") { filteredFLACCheck3.Checked = true; } else { filteredFLACCheck3.Checked = false; }

            if (CF.DefaultSettings["EnableTrack3"] == "True")
            {
                disableCheckStream3.Checked = false;
                audioCodecComboBox3.Enabled = true;
                mixdownComboBox3.Enabled = true;
                sampleRateCombo3.Enabled = true;
                audioBitrateCombo3.Enabled = true;

                if (CF.DefaultSettings["AudioCodec3"] == "Filtered Passthru")
                {
                    filteredAACCheck3.Visible = true;
                    filteredAC3Check3.Visible = true;
                    filteredEAC3Check3.Visible = true;
                    filteredDTSCheck3.Visible = true;
                    filteredDTSHDCheck3.Visible = true;
                    filteredTrueHDCheck3.Visible = true;
                    filteredMP3Check3.Visible = true;
                    filteredFLACCheck3.Visible = true;
                    passthruFilterLabel3.Visible = true;
                }
                else
                {
                    filteredAACCheck3.Visible = false;
                    filteredAC3Check3.Visible = false;
                    filteredEAC3Check3.Visible = false;
                    filteredDTSCheck3.Visible = false;
                    filteredDTSHDCheck3.Visible = false;
                    filteredTrueHDCheck3.Visible = false;
                    filteredMP3Check3.Visible = false;
                    filteredFLACCheck3.Visible = false;
                    passthruFilterLabel3.Visible = false;
                }
            }
            else
            {
                disableCheckStream3.Checked = true;
                audioCodecComboBox3.Enabled = false;
                mixdownComboBox3.Enabled = false;
                sampleRateCombo3.Enabled = false;
                audioBitrateCombo3.Enabled = false;

                filteredAACCheck3.Visible = false;
                filteredAC3Check3.Visible = false;
                filteredEAC3Check3.Visible = false;
                filteredDTSCheck3.Visible = false;
                filteredDTSHDCheck3.Visible = false;
                filteredTrueHDCheck3.Visible = false;
                filteredMP3Check3.Visible = false;
                filteredFLACCheck3.Visible = false;
                passthruFilterLabel3.Visible = false;

            }

        }
        private void ReturnAllVideoFiles()
        {

            int loopcount = 0; //displays iteration number of the loop. Used to display which file is being processed.
            int fileCount = 0; //displays the number of files in the directory to be processed.

            if (Directory.Exists(CF.DefaultSettings["InputFilePath"]))
            {
                string fileName = ""; //Holds value of processing filename
                filesListBox.Items.Clear();
                MediaInfoTB.Clear();
                VideoFilesList.Clear();
                IncompatibilityInfo.Clear();

                NLabelUpdate("Checking and filtering directory for video files ", Color.GreenYellow);

                try
                {
                    string[] fileNames = Directory
                            .GetFiles(CF.DefaultSettings["InputFilePath"], "*", SearchOption.AllDirectories)
                            .Where(file => file.ToLower().EndsWith(".mpg") || file.ToLower().EndsWith(".mpeg") || file.ToLower().EndsWith(".vob") || file.ToLower().EndsWith(".mod") || file.ToLower().EndsWith(".ts") || file.ToLower().EndsWith(".m2ts")
                            || file.ToLower().EndsWith(".mp4") || file.ToLower().EndsWith(".m4v") || file.ToLower().EndsWith(".mov") || file.ToLower().EndsWith("avi") || file.ToLower().EndsWith(".divx")
                            || file.ToLower().EndsWith(".wmv") || file.ToLower().EndsWith(".asf") || file.ToLower().EndsWith(".mkv") || file.ToLower().EndsWith(".flv") || file.ToLower().EndsWith(".f4v")
                            || file.ToLower().EndsWith(".dvr") || file.ToLower().EndsWith(".dvr-ms") || file.ToLower().EndsWith(".wtv") || file.ToLower().EndsWith(".ogv") || file.ToLower().EndsWith(".ogm")
                            || file.ToLower().EndsWith(".3gp") || file.ToLower().EndsWith(".rm") || file.ToLower().EndsWith(".rmvb"))
                            .ToArray();

                    Array.Sort(fileNames);
                    fileCount = fileNames.Count();

                    foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                    {
                        loopcount = loopcount + 1;

                        NLabelUpdate("Processing file " + loopcount.ToString() + " of " + fileCount.ToString() + " - " + file, Color.GreenYellow);
                        
                        fileName = file;
                        while (fileName.Contains(@"\"))
                        {
                            fileName = fileName.Remove(0, 1);
                        }

                        CF.DefaultSettings["InputFilePath"] = file.Replace(fileName, "");

                        filesListBox.Items.Add(fileName);
                        filesListBox.Update();
                        VideoFilesList.Add(file);

                    }
                    NLabelUpdate("Listing " + filesListBox.Items.Count.ToString() + " Video Files", Color.GreenYellow);
                }
                catch (Exception e){CustomMessageBox.Show(e.ToString(), 131, 280);}
            }
        }
        private void ListAllVideosButton_Click(object sender, EventArgs e)
        {
            ReturnAllVideoFiles();
        }       
        private void SelectDirectory()
        {

            FolderBrowserDialog FBD = new FolderBrowserDialog(); //creates new instance of the FolderBrowserDialog

            if (!string.IsNullOrEmpty(CF.DefaultSettings["InputFilePath"])) //if CF.DefaultSettings["InputFilePath"] contains a path, sets folderBrowserDialog to default to this path
            {
                FBD.SelectedPath = CF.DefaultSettings["InputFilePath"];
            }

            if (FBD.ShowDialog() == DialogResult.OK) //shows folderbrowserdialog, runs addtional code if not cancelled out
            {
                CF.DefaultSettings["InputFilePath"] = FBD.SelectedPath;
                CF.UpdateDefaults();
                filenameTextBox.Text = CF.DefaultSettings["InputFilePath"];
                ReturnAllVideoFiles();
            }

            DialogResult = DialogResult.None; //Prevents form from closing...
        }
        private void SelectDirectoryButton_Click(object sender, EventArgs e)
        {
            SelectDirectory();
        }
        private void InvisibleCloseButton_Click(object sender, EventArgs e)
        {
            this.Close(); //Located behind the MediaInfo text box, lower right hand corner
        }       
        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Select Directory
            SelectDirectory();
        }
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); //closes form from tool menu
        }
        private void FilesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //Double click to get media information about the file.
            tabControl1.SelectedIndex = 0; //Changes tab to media info tab.
            if (IncompatibilityInfo.Count <= 0) //Incompatible List not available
            {
                if (filesListBox.Items.Count > 0) //ensures that the files listbox isn't emtpy
                {
                    string videoFileName = VideoFilesList[filesListBox.SelectedIndex]; //returns currently selected items file path

                    if (!string.IsNullOrEmpty(CF.DefaultSettings["InputFilePath"])) //As long as the file path isn't empty or null, get info about file
                    {
                        MediaFile videoFile = new MediaFile(videoFileName); //return info about selected file
                        MediaInfoTB.Text = videoFile.Info_Text; //output info about selected file to the output box
                        MediaInfoTB.SelectionStart = 0;
                        MediaInfoTB.ScrollToCaret(); // force current position back to top
                        MediaInfoTB.Update();
                    }
                }
            }
            else //Incompatibility information available, skip detail info give incompatible info.
            {
                //checks that there is incompatibility info, and that the counts are correct.
                //The IncompatibilityInfo count should be equal to the filesListBox count.
                if (IncompatibilityInfo.Count > 0 & !(filesListBox.Items.Count > IncompatibilityInfo.Count))
                {
                    if (filesListBox.Items.Count > 0) //Ensure there is something in the listbox
                    {
                        string videoFileName = VideoFilesList[filesListBox.SelectedIndex]; //pulls filepath (videoFileName) from listbox

                        if (!string.IsNullOrEmpty(CF.DefaultSettings["InputFilePath"])) //check that folderpath isn't empty
                        {
                            MediaFile videoFile = new MediaFile(videoFileName); //list incompatible file attributes.
                            MediaInfoTB.Text = "INCOMPATIBLE FILE FOUND - " +
                                filesListBox.SelectedItem.ToString() + "\r\n\r\n" +
                                "INCOMPATIBLE FILE ATTRIBUTES LISTED BELOW:\r\n\r\n" +
                                IncompatibilityInfo[filesListBox.SelectedIndex] +
                                separator2 +
                                videoFile.Info_Text;
                            MediaInfoTB.SelectionStart = 0;
                            MediaInfoTB.ScrollToCaret(); // force current position back to top
                            MediaInfoTB.Update();
                        }
                    }
                }
            }

        }
        private void ExportButton_Click(object sender, EventArgs e)
        {
            string fileLocation = "";
            //Check to see if data exist in fileslistbox
            if (VideoFilesList.Count > 0)
            {
                //check to see if data exists in IncompatibilityInfo List
                if (IncompatibilityInfo.Count > 0)
                {
                    //SaveFileDialog
                    SaveFileDialog SFD = new SaveFileDialog();
                    SFD.DefaultExt = "txt";
                    SFD.FileName = "Incompatible Video Files.txt";
                    SFD.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    SFD.InitialDirectory = CF.DefaultSettings["ExportFilePath"];

                    if (SFD.ShowDialog() == DialogResult.OK)
                    {
                        fileLocation = SFD.FileName;

                        char delim = '\\';
                        string[] Tokens = SFD.FileName.Split(delim);
                        CF.DefaultSettings["ExportFilePath"] = ""; //Clear Path

                        for (int i = 0; i < Tokens.Count() -1; i++)
                        {
                            CF.DefaultSettings["ExportFilePath"] += Tokens[i].ToString() + "\\"; //Sets the default directory for exporting text file.
                        }

                        CF.UpdateDefaults();

                        //Create text file
                        using (StreamWriter sw = System.IO.File.CreateText(fileLocation))
                        {
                            //Loop Through List
                            //Write text file with Incompatibility Info
                            for (int i = 0; i < VideoFilesList.Count; i++)
                            {
                                sw.WriteLine("File: " + filesListBox.Items[i]);
                                sw.WriteLine(VideoFilesList[i]);
                                sw.WriteLine("Incompatible Attributes:");
                                sw.WriteLine(IncompatibilityInfo[i] + "\r");
                            }
                            sw.Close();
                        }

                        //Open Text file
                        Process.Start(fileLocation);
                    }

                }
                else
                {
                    //SaveFileDialog
                    SaveFileDialog SFD = new SaveFileDialog();
                    SFD.FileName = "Video Files.txt";
                    SFD.DefaultExt = "txt";
                    SFD.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                    SFD.InitialDirectory = CF.DefaultSettings["ExportFilePath"];

                    if (SFD.ShowDialog() == DialogResult.OK)
                    {
                        fileLocation = SFD.FileName;

                        char delim = '\\';
                        string[] Tokens = SFD.FileName.Split(delim);
                        CF.DefaultSettings["ExportFilePath"] = ""; //Clear Path

                        for (int i = 0; i < Tokens.Count() - 1; i++)
                        {
                            CF.DefaultSettings["ExportFilePath"] += Tokens[i].ToString() + "\\"; //Sets the default directory for exporting text file.
                        }

                        CF.UpdateDefaults();

                        //Create text file
                        using (StreamWriter sw = System.IO.File.CreateText(fileLocation))
                        {
                            //Loop Through List
                            //Write text file with Incompatibility Info
                            for (int i = 0; i < VideoFilesList.Count; i++)
                            {
                                sw.WriteLine("File: " + filesListBox.Items[i]);
                                sw.WriteLine(VideoFilesList[i] + "\r");
                            }
                            sw.Close();
                        }

                        //Open Text file
                        Process.Start(fileLocation);
                    }

                }

            }
            else
            {
                CustomMessageBox.Show("There is no data to save.", 129, 235, "Save to file error");
            }
        }
        private void SaveInfoButton_Click(object sender, EventArgs e)
        {
            string fileLocation = "";
            string outputBoxText = MediaInfoTB.Text;
            outputBoxText = outputBoxText.Replace("\n", "\r\n");
            //SaveFileDialog
#pragma warning disable IDE0017 // Simplify object initialization
            SaveFileDialog SFD = new SaveFileDialog();
#pragma warning restore IDE0017 // Simplify object initialization
            SFD.DefaultExt = "txt";
            if (MediaInfoTB.Text.Contains(separator))
            {
                SFD.FileName = "Quick_Media_Info.txt";
            }
            else
            {
                SFD.FileName = "Detail Info For " + filesListBox.SelectedItem.ToString() + ".txt";
            }

            SFD.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

            if (SFD.ShowDialog() == DialogResult.OK)
            {
                fileLocation = SFD.FileName;
                //Create text file


                using (StreamWriter sw = System.IO.File.CreateText(fileLocation))
                {

                    sw.WriteLine("File: " + filesListBox.SelectedItem.ToString());
                    sw.WriteLine("");
                    sw.WriteLine(outputBoxText);
                    sw.Close();
                }

                //Open Text file
                Process.Start(fileLocation);
            }
        }
        private void GetQuickInfo(string file, string fileName)
        {
            
            StringBuilder VideoInfo = new StringBuilder();
            StringBuilder AudioInfo = new StringBuilder();
            StringBuilder SubtitleInfo = new StringBuilder();

            string videoBitrate = "";
            string FPS = "";
            string videoFrameSize = "";
            string videoFormat = "";

            string audioBitrate = "";
            string audioLanguage = "";
            string audioFormat = "";
            string audioChannels = "";

            string subtitleFormat = "";
            string subtitleTitle = "";
            string subtitleLanguage = "";
            string subtitleForcedFlag = "";
            string subtitleDefaultFlag = "";



            //Add filename, bitrate, framerate to the Media Info Box
            MediaFile videoFile = new MediaFile(file);

            //Video Info
            for (int a = 0; a < videoFile.Video.Count; a++)
            {
                if (videoFile.Video[a].Properties.ContainsKey("Bit rate") && videoFile.Video[a].Properties["Bit rate"] != "0")
                {
                    if (videoFile.Video[a].Properties["Bit rate"].Contains("Mbps")) //If file bitrate is in Mbps
                    {
                        videoBitrate = videoFile.Video[a].Properties["Bit rate"].Replace(" ", "").Replace("Mbps", "");
                        videoBitrate = ", " + (double.Parse(videoBitrate) * 1000).ToString() + " Kbps";//accounts for reading in Mbps rather than Kbps
                    }
                    else if (videoFile.Video[a].Properties["Bit rate"].Contains("Kbps")) //If file bitrate is in Kbps
                    {
                        videoBitrate = ", " + videoFile.Video[a].Properties["Bit rate"].ToUpper().Replace(" ", "").Replace("KBPS", "") + " kbps";
                    }
                }
                else if (videoFile.General.Properties.ContainsKey("Overall bit rate") && videoFile.General.Properties["Overall bit rate"] != "0")
                {
                    if (videoFile.General.Properties["Overall bit rate"].Contains("Mbps")) //if bitrate is measured in Mbps
                    {
                        videoBitrate = videoFile.General.Properties["Overall bit rate"].ToUpper().Replace(" ", "").Replace("MBPS", "");
                        videoBitrate = ", Overall Bitrate: " + (double.Parse(videoBitrate) * 1000).ToString() + " kbps";
                    }
                }
                else
                {
                    videoBitrate = ", Overall Bitrate: " + videoFile.General.Bitrate.ToString() + " kbps";
                }

                if (videoFile.Video[a].Properties.ContainsKey("Frame rate") && videoFile.Video[a].Properties["Frame rate"] != "0")
                {
                    FPS = ", " + videoFile.Video[a].Properties["Frame rate"];
                }

                if (videoFile.Video[a].Properties.ContainsKey("Width") && videoFile.Video[a].Properties.ContainsKey("Height"))
                {
                    videoFrameSize = ", " + videoFile.Video[a].Properties["Width"].Replace(" ", "").Replace("pixels", "") + " x " + videoFile.Video[a].Properties["Height"].Replace(" ", "").Replace("pixels", "");
                }

                if (videoFile.Video[a].Properties.ContainsKey("Format"))
                {
                    videoFormat = videoFile.Video[a].Properties["Format"];
                }

                VideoInfo.Append(" Video" + (a + 1).ToString() + ": " + videoFormat + videoBitrate + FPS + videoFrameSize);

                videoBitrate = null;
                FPS = null;
                videoFrameSize = null;
                videoFormat = null;
            }

            //Audio Info
            for (int i = 0; i < videoFile.Audio.Count(); i++)
            {
                if (videoFile.Audio[i].Properties.ContainsKey("Language"))
                {
                    audioLanguage = videoFile.Audio[i].Properties["Language"] + ", ";
                }

                if (videoFile.Audio[i].Properties.ContainsKey("Format"))
                {
                    audioFormat = videoFile.Audio[i].Properties["Format"] + ", ";
                }

                if (videoFile.Audio[i].Properties.ContainsKey("Channel(s)"))
                {
                    audioChannels = videoFile.Audio[i].Properties["Channel(s)"] + ", ";
                }
                if (videoFile.Audio[i].Properties.ContainsKey("Format"))
                {
                    if(videoFile.Audio[i].Properties["Format"] == "E-AC-3")
                    {
                        audioChannels = videoFile.Audio[i].Properties["Channel(s)"] + "???, ";
                    }
                }
                if(videoFile.Audio[i].Properties.ContainsKey("Channel(s)_Original"))
                {
                    //Channel(s)_Original : 8 channels / 6 channels
                    audioChannels = videoFile.Audio[i].Properties["Channel(s)_Original"] + ", ";
                }


                if (videoFile.Audio[i].Properties.ContainsKey("Bit rate"))
                {
                    if (videoFile.Audio[i].Properties["Bit rate"].Contains("Mbps")) //If file bitrate is in Mbps
                    {
                        audioBitrate = videoFile.Audio[i].Properties["Bit rate"].Replace(" ", "").Replace("Mbps", "");
                        audioBitrate = (double.Parse(audioBitrate) * 1000).ToString() + " Kbps";//accounts for reading in Mbps rather than Kbps
                    }
                    else if (videoFile.Audio[i].Properties["Bit rate"].Contains("Kbps")) //If file bitrate is in Kbps
                    {
                        audioBitrate = videoFile.Audio[i].Properties["Bit rate"].Replace(" ", "").Replace("Kbps", "") + " Kbps";
                    }

                }

                AudioInfo.Append(" Audio" + (i + 1).ToString() + ": " + audioLanguage + audioFormat + audioChannels + audioBitrate + "\n\t");
                audioBitrate = null;
                audioLanguage = null;
                audioFormat = null;
                audioChannels = null;
            }

            //Subtitle Info
            /*string subtitleInfo = "";
            string subtitleFormat = "";
            string subtitleTitle = "";
            string subtitleLanguage = "";
            bool subtitleForcedFlag = false;
            bool subtitleDefaulFlag = false;*/

            for (int i = 0; i < videoFile.Text.Count; i++)
            {
                //Title
                if(videoFile.Text[i].Properties.ContainsKey("Title")){ subtitleTitle = videoFile.Text[i].Properties["Title"] + ", "; }

                //Language
                if (videoFile.Text[i].Properties.ContainsKey("Language")){ subtitleLanguage = videoFile.Text[i].Properties["Language"] + ", "; }

                //Flags
                if (videoFile.Text[i].Properties.ContainsKey("Forced"))
                {
                    if(videoFile.Text[i].Properties["Forced"] == "Yes")
                    {
                        subtitleForcedFlag = "Forced";
                    }
                }
                if (videoFile.Text[i].Properties.ContainsKey("Default"))
                {

                    if (videoFile.Text[i].Properties["Default"] == "Yes")
                    {
                        if(string.IsNullOrEmpty(subtitleForcedFlag))
                        {
                            subtitleDefaultFlag = "Default";
                        }
                        else
                        {
                            subtitleDefaultFlag = ", Default ";
                        }
                        
                    }
                }

                //Format
                if (videoFile.Text[i].Properties.ContainsKey("Format")) { subtitleFormat = videoFile.Text[i].Properties["Format"] + ", "; }
                string substring = (" Subtitle" + (i + 1).ToString() + ": " + subtitleTitle + subtitleFormat + subtitleLanguage + subtitleForcedFlag + subtitleDefaultFlag);
                substring = substring.TrimEnd(' ',',');
                SubtitleInfo.Append(substring + "\n\t");
                subtitleFormat = "";
                subtitleTitle = "";
                subtitleLanguage = "";
                subtitleForcedFlag = "";
                subtitleDefaultFlag = "";
            }

            if (string.IsNullOrEmpty(VideoInfo.ToString()) && string.IsNullOrEmpty(AudioInfo.ToString()) && string.IsNullOrEmpty(SubtitleInfo.ToString()))
            {
                MediaInfoTB.Text = MediaInfoTB.Text + fileName + "\n\t" + "Unable to gather info. File may be corrupt." + separator;
                MediaInfoTB.SelectionStart = 0;
                //outPutTextBox.ScrollToCaret(); // force current position back to top
                MediaInfoTB.Update();
            }
            else
            {
                MediaInfoTB.Text = MediaInfoTB.Text + fileName + "\n\t"
                + VideoInfo.ToString() + "\n\t";
                if (!string.IsNullOrEmpty(AudioInfo.ToString())) { MediaInfoTB.Text += AudioInfo.ToString(); }
                if (!string.IsNullOrEmpty(SubtitleInfo.ToString())) { MediaInfoTB.Text += SubtitleInfo.ToString(); }

                //Add Separator
                MediaInfoTB.Text = MediaInfoTB.Text.TrimEnd('\t') + separator;

                MediaInfoTB.SelectionStart = 0;
                //outPutTextBox.ScrollToCaret(); // force current position back to top
                MediaInfoTB.Update();
            }


            VideoInfo.Clear();
            AudioInfo.Clear();
            SubtitleInfo.Clear();
        }
        private async void QuickInfobutton_Click(object sender, EventArgs e)
        {

            if (VideoFilesList.Count > 0 & filesListBox.Items.Count > 0)
            {
                tabControl1.SelectedIndex = 0; //Selects Media Info Tab
                MediaInfoTB.Clear();
                notificationLabel.Visible = true;
                if(filesListBox.SelectedIndices.Count > 0) //selected items
                {
                    for (int i = 0; i < filesListBox.SelectedIndices.Count; i++)
                    {
                        NLabelUpdate("Processing file " + (i + 1).ToString() + " of " + filesListBox.SelectedIndices.Count.ToString(), Color.GreenYellow);
                        GetQuickInfo(VideoFilesList[filesListBox.SelectedIndices[i]], filesListBox.Items[filesListBox.SelectedIndices[i]].ToString());
                    }
                }
                else //no items selected, perform on all items in listbox.
                {
                    for (int i = 0; i < VideoFilesList.Count(); i++)
                    {
                        NLabelUpdate("Processing file " + (i + 1).ToString() + " of " + VideoFilesList.Count().ToString(), Color.GreenYellow);

                        GetQuickInfo(VideoFilesList[i], filesListBox.Items[i].ToString());
                    }
                }
                
                NLabelUpdate("Processing complete.", Color.GreenYellow);
            }
        }
        private void DetailInfoButton_Click(object sender, EventArgs e)
        {
            if(filesListBox.SelectedIndices.Count == 1 || filesListBox.SelectedIndices.Count == 0) //Only 0 or 1 item selected
            {
                if (filesListBox.SelectedIndex == -1) { filesListBox.SelectedIndex = 0; } //Sets index selection to top of the list
                tabControl1.SelectedIndex = 0; //Changes tab to media info tab.
                if (IncompatibilityInfo.Count <= 0) //Incompatible List not available
                {
                    if (filesListBox.Items.Count > 0) //ensures that the files listbox isn't emtpy
                    {
                        string videoFileName = VideoFilesList[filesListBox.SelectedIndex]; //returns currently selected items file path


                        if (!string.IsNullOrEmpty(CF.DefaultSettings["InputFilePath"])) //As long as the file path isn't empty or null, get info about file
                        {
                            NLabelUpdate("Processing detailed info for " + filesListBox.Items[filesListBox.SelectedIndex].ToString() + ".", Color.GreenYellow);
                            MediaFile videoFile = new MediaFile(videoFileName); //return info about selected file
                            MediaInfoTB.Text = videoFile.Info_Text; //output info about selected file to the output box
                            MediaInfoTB.SelectionStart = 0;
                            MediaInfoTB.ScrollToCaret(); // force current position back to top
                            MediaInfoTB.Update();
                            NLabelUpdate("Processing detailed info for " + filesListBox.Items[filesListBox.SelectedIndex].ToString() + " is now complete.", Color.GreenYellow);
                        }
                    }
                }
                else //List available for Incompatible files
                {
                    //checks that there is incompatibility info, and that the counts are correct.
                    //The IncompatibilityInfo count should be equal to the filesListBox count.
                    if (IncompatibilityInfo.Count > 0 & !(filesListBox.Items.Count > IncompatibilityInfo.Count))
                    {
                        if (filesListBox.Items.Count > 0) //Ensure there is something in the listbox
                        {
                            string videoFileName = VideoFilesList[filesListBox.SelectedIndex]; //pulls filepath (videoFileName) from listbox

                            if (!string.IsNullOrEmpty(CF.DefaultSettings["InputFilePath"])) //check that folderpath isn't empty
                            {
                                NLabelUpdate("Processing detailed info for " + filesListBox.Items[filesListBox.SelectedIndex].ToString() + ".", Color.GreenYellow);
                                MediaFile videoFile = new MediaFile(videoFileName); //list incompatible file attributes.
                                MediaInfoTB.Text = "INCOMPATIBLE FILE FOUND - " +
                                    filesListBox.SelectedItem.ToString() + "\r\n\r\n" +
                                    "INCOMPATIBLE FILE ATTRIBUTES LISTED BELOW:\r\n\r\n" +
                                    IncompatibilityInfo[filesListBox.SelectedIndex] +
                                    separator2 +
                                    videoFile.Info_Text;
                                MediaInfoTB.SelectionStart = 0;
                                MediaInfoTB.ScrollToCaret(); // force current position back to top
                                MediaInfoTB.Update();
                                NLabelUpdate("Processing detailed info for " + filesListBox.Items[filesListBox.SelectedIndex].ToString() + " is now complete.", Color.GreenYellow);
                            }
                        }
                    }
                }
            }
            else //Multiple Selected
            {
                CustomMessageBox.Show("Detailed info can only be provided for 1 item at a time.", 150, 300, "Error");
            }
            
        }


        /*The following methods are for converting video files*/
        private async void ConvertAllButton_Click(object sender, EventArgs e)
        {
            CF.UpdateDefaults();
            CommandOutPutTextBox.Text = ""; //Clear output text
            

            //Check for location of HandbrakeCLI
            string handBrakeCLILocation = CheckForHandbrakeCLI();
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            string totalProcessingTime = "";
            List<string> Errors = new List<string>();
            string errorString = "";
            Errors.Clear();
            int exitCode = 0; //HandbrakeCLI Exit Code 0=Exited Normally, 1=Cancelled, 2=Invalid Input, 3=Initalization Error, 4=Unknown Error
            int selectCounter = 0; //Counter to measure number of selected items vs processes ones.


            if (PreConversionChecks()) //If handbrake is found continue
            {
                string handBrakeCLIString;
                if (filesListBox.Items.Count > 0) //FilesListBox contains files
                {
#pragma warning disable IDE0017 // Simplify object initialization
                    FolderBrowserDialog FBD = new FolderBrowserDialog(); //creates new instance of the FolderBrowserDialog
#pragma warning restore IDE0017 // Simplify object initialization
                    FBD.Description = "Select Output Folder for Converted Video Files";

                    if (!string.IsNullOrEmpty(CF.DefaultSettings["OutputFilePath"])) //if folderpath contains a path, sets folderBrowserDialog to default to this path
                    {
                        FBD.SelectedPath = CF.DefaultSettings["OutputFilePath"];
                    }

                    if (FBD.ShowDialog() == DialogResult.OK) //shows folderbrowserdialog, runs addtional code if not cancelled out
                    {
                        CF.DefaultSettings["OutputFilePath"] = FBD.SelectedPath;

                        for (int i = 0; i < VideoFilesList.Count; i++)
                        {
                            if(filesListBox.SelectedIndices.Count > 0)
                            {
                                
                                for (int a = 0; a < filesListBox.SelectedIndices.Count; a++)
                                {
                                    //Selected item to process
                                    if(filesListBox.SelectedIndices[a] == i )
                                    {
                                        selectCounter += 1;
                                        //Display which file is being converted
                                        NLabelUpdate("Converting File " + (selectCounter).ToString() + " of " + filesListBox.SelectedIndices.Count.ToString() + " ( " + filesListBox.Items[i].ToString() + " )", Color.GreenYellow);

                                        DialogResult = DialogResult.None; //Prevents form from closing...

                                        handBrakeCLIString = GenerateConversionString(VideoFilesList[i], filesListBox.Items[i].ToString(), FBD.SelectedPath);

                                        if (string.IsNullOrEmpty(handBrakeCLIString))
                                        {
                                            handBrakeCLIString = ""; // Set to defaults let handbrake error out if necessary
                                        }

                                        //Send commands used in handbrake cli to the command output textbox
                                        string commandOutputString = "Handbrake Command for encoding task " + selectCounter.ToString() + " \"" + filesListBox.Items[i].ToString() + "\" \r\n\r\n" + handBrakeCLIString;
                                        if (string.IsNullOrEmpty(CommandOutPutTextBox.Text))
                                        {
                                            CommandOutPutTextBox.Text = commandOutputString;
                                        }
                                        else
                                        {
                                            CommandOutPutTextBox.Text += "\r\n\r\n" + commandOutputString;
                                        }
                                        
                                        try //Check for errors and continue processing 
                                        {
                                            //Launch command line object to pass the commands to
                                            if (System.IO.File.Exists(VideoFilesList[i])) //Skip file if it has been moved or deleted 
                                            {

                                                await Task.Run(()=>
                                                {
                                                    try
                                                    {
                                                        using (Process conversionProcess = new Process())
                                                        {
                                                            conversionProcess.StartInfo.FileName = handBrakeCLILocation + @"\HandBrakeCLI.exe";
                                                            conversionProcess.StartInfo.Arguments = "/c " + handBrakeCLIString; //Sets commandline arguments
                                                            conversionProcess.StartInfo.UseShellExecute = false; //Must be set to false to redirect standard error.
                                                                                                                    //conversionProcess.StartInfo.RedirectStandardError = true; //Allows for redirect of the standard error for the process.
                                                            conversionProcess.EnableRaisingEvents = true; //Raises process exited event on close

                                                            conversionProcess.Start();
                                                            conversionProcess.WaitForExit();

                                                            exitCode = conversionProcess.ExitCode;
                                                        }
                                                    }
                                                    catch
                                                    {
                                                        exitCode = 4; //Unknown Error
                                                    }
                                                        
                                                });
                                                    
                                            }

                                            switch (exitCode)
                                            {
                                                case 0: //Completed Successfully
                                                    break;
                                                case 1: //Cancelld
                                                    Errors.Add("Error Processing File " + filesListBox.Items[i] + "\r\n\t Exited with code = " + exitCode + " - Cancelled");
                                                    break;
                                                case 2: //Invalid Input
                                                    Errors.Add("Error Processing File " + filesListBox.Items[i] + "\r\n\t Exited with code = " + exitCode + " - Invalid Input");
                                                    break;
                                                case 3: //Initialization Error
                                                    Errors.Add("Error Processing File " + filesListBox.Items[i] + "\r\n\t Exited with code = " + exitCode + " - Initialization Error");
                                                    break;
                                                case 4: //Unknown Error
                                                    Errors.Add("Error Processing File " + filesListBox.Items[i] + "\r\n\t Exited with code = " + exitCode + " - Unknown Error");
                                                    break;
                                                default:
                                                    break;
                                            }
                                        }
                                        catch
                                        {
                                            Errors.Add("Error Processing File " + filesListBox.Items[i] + " . File May Be Corrupt.");
                                        }
                                    }
                                    //No item selected
                                    else
                                    {

                                    }
                                }
                            }
                            else //No items selected, process all items as normal.
                            {
                                //Display which file is being converted
                                NLabelUpdate("Converting File " + (i + 1).ToString() + " of " + VideoFilesList.Count.ToString() + " ( " + filesListBox.Items[i].ToString() + " )", Color.GreenYellow);

                                DialogResult = DialogResult.None; //Prevents form from closing...

                                handBrakeCLIString = GenerateConversionString(VideoFilesList[i], filesListBox.Items[i].ToString(), FBD.SelectedPath);

                                if (string.IsNullOrEmpty(handBrakeCLIString))
                                {
                                    handBrakeCLIString = ""; // Set to defaults let handbrake error out if necessary
                                }

                                //Send commands used in handbrake cli to the command output textbox
                                string commandOutputString = "Handbrake Command for encoding task " + i.ToString() + " \"" + filesListBox.Items[i].ToString() + "\" \r\n\r\n" + handBrakeCLIString;
                                if (string.IsNullOrEmpty(CommandOutPutTextBox.Text))
                                {
                                    CommandOutPutTextBox.Text = commandOutputString;
                                }
                                else
                                {
                                    CommandOutPutTextBox.Text += "\r\n\r\n" + commandOutputString;
                                }

                                try //Check for errors and continue processing 
                                {
                                    //Launch command line object to pass the commands to
                                    if (System.IO.File.Exists(VideoFilesList[i])) //Skip file if it has been moved or deleted 
                                    {
                                        await Task.Run(() =>
                                        {
                                            try
                                            {
                                                using (Process conversionProcess = new Process())
                                                {
                                                    conversionProcess.StartInfo.FileName = handBrakeCLILocation + @"\HandBrakeCLI.exe";
                                                    conversionProcess.StartInfo.Arguments = "/c " + handBrakeCLIString; //Sets commandline arguments
                                                    conversionProcess.StartInfo.UseShellExecute = false; //Must be set to false to redirect standard error.
                                                                                                         //conversionProcess.StartInfo.RedirectStandardError = true; //Allows for redirect of the standard error for the process.
                                                    conversionProcess.EnableRaisingEvents = true; //Raises process exited event on close

                                                    conversionProcess.Start();
                                                    conversionProcess.WaitForExit();

                                                    exitCode = conversionProcess.ExitCode;
                                                }
                                            }
                                            catch
                                            {
                                                exitCode = 4; //Unknown Error
                                            }

                                        });


                                    }

                                    switch (exitCode)
                                    {
                                        case 0: //Completed Successfully
                                            break;
                                        case 1: //Cancelld
                                            Errors.Add("Error Processing File " + filesListBox.Items[i] + "\r\n\t Exited with code = " + exitCode + " - Cancelled");
                                            break;
                                        case 2: //Invalid Input
                                            Errors.Add("Error Processing File " + filesListBox.Items[i] + "\r\n\t Exited with code = " + exitCode + " - Invalid Input");
                                            break;
                                        case 3: //Initialization Error
                                            Errors.Add("Error Processing File " + filesListBox.Items[i] + "\r\n\t Exited with code = " + exitCode + " - Initialization Error");
                                            break;
                                        case 4: //Unknown Error
                                            Errors.Add("Error Processing File " + filesListBox.Items[i] + "\r\n\t Exited with code = " + exitCode + " - Unknown Error");
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                catch
                                {
                                    Errors.Add("Error Processing File " + filesListBox.Items[i] + " . File May Be Corrupt.");
                                }
                            }
                            

                        }

                        endTime = DateTime.Now;
                        totalProcessingTime = TimeDifference(startTime, endTime);

                        if (Errors.Count > 0)
                        {
                            tabControl1.SelectedIndex = 0;
                            foreach (var ErrorLine in Errors)
                            {
                                errorString += ErrorLine + "\r\n";
                            }
                            MediaInfoTB.Text = "Files skipped due to error:\r\n" + errorString;

                            if (VideoFilesList.Count == 1) { NLabelUpdate("The transcoding que initiated " + startTime.ToString() + " failed. HandbrakeCLI exited with code " + exitCode.ToString(), Color.GreenYellow); }
                            if (VideoFilesList.Count > 1) { NLabelUpdate("The transcoding que initiated " + startTime.ToString() + " is now complete. " + (VideoFilesList.Count() - Errors.Count()).ToString() + " of " + VideoFilesList.Count().ToString() + " files processed successfully in " + totalProcessingTime, Color.GreenYellow); }




                            if (notificationCheck.Checked)
                            {
                                string username = usernameBox.Text;
                                string password = passwordBox.Text;
                                string sendTo = sendToBox.Text;

                                if (VideoFilesList.Count == 1)
                                {
                                    await Task.Run(() =>
                                   {
                                       SendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " failed. HandbrakeCLI exited with code" + exitCode.ToString());
                                   });
                                    
                                }
                                if (VideoFilesList.Count > 1)
                                {
                                    await Task.Run(() =>
                                    {
                                        SendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. " + (VideoFilesList.Count() - Errors.Count()).ToString() + " of " + VideoFilesList.Count().ToString() + " files processed successfully in " + totalProcessingTime);
                                    });
                                }
                            }
                        }
                        else if (selectCounter > 0) //Items selected from list
                        {
                            MediaInfoTB.Text = ""; //Clears Output Box on successful Encode

                            if (filesListBox.SelectedIndices.Count == 1) { NLabelUpdate("The transcoding que initiated " + startTime.ToString() + " is now complete. The file was processed in " + totalProcessingTime, Color.GreenYellow); }
                            if (filesListBox.SelectedIndices.Count > 1) { NLabelUpdate("The transcoding que initiated " + startTime.ToString() + " is now complete. " + filesListBox.SelectedIndices.Count.ToString() + " files were processed in " + totalProcessingTime, Color.GreenYellow); }

                            if (notificationCheck.Checked)
                            {
                                string username = usernameBox.Text;
                                string password = passwordBox.Text;
                                string sendTo = sendToBox.Text;

                                if (filesListBox.SelectedIndices.Count == 1) { SendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. The file was processed in " + totalProcessingTime); }
                                if (filesListBox.SelectedIndices.Count > 1) { SendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. " + filesListBox.SelectedIndices.Count.ToString() + " files were processed in " + totalProcessingTime); }
                            }
                        }
                        else
                        {
                            MediaInfoTB.Text = ""; //Clears Output Box on successful Encode

                            if (VideoFilesList.Count == 1) { NLabelUpdate("The transcoding que initiated " + startTime.ToString() + " is now complete. The file was processed in " + totalProcessingTime, Color.GreenYellow); }
                            if (VideoFilesList.Count > 1) { NLabelUpdate("The transcoding que initiated " + startTime.ToString() + " is now complete. " + VideoFilesList.Count().ToString() + " files were processed in " + totalProcessingTime, Color.GreenYellow); }

                            if (notificationCheck.Checked)
                            {
                                string username = usernameBox.Text;
                                string password = passwordBox.Text;
                                string sendTo = sendToBox.Text;

                                if (VideoFilesList.Count == 1) { SendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. The file was processed in " + totalProcessingTime); }
                                if (VideoFilesList.Count > 1) { SendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. " + VideoFilesList.Count().ToString() + " files were processed in " + totalProcessingTime); }
                            }
                        }
                    }
                }
                else
                {
                    NLabelUpdate("", Color.GreenYellow);
                }
            }
        }
        private bool PreConversionChecks()
        {
            bool checksPassed = true;
            NLabelUpdate("Checking for existence of Handbrake CLI", Color.GreenYellow);
            string handBrakeCLILocation = CheckForHandbrakeCLI();

            //Ensure HandbrakeCLI is found
            if (string.IsNullOrEmpty(handBrakeCLILocation)) { checksPassed = false; }

            //If Filtered Passthru is selected ensure a filter is also chosen
            switch (audioCodecComboBox.Text)
            {
                case "Filtered Passthru":
                    if (!filteredAACCheck.Checked && !filteredAC3Check.Checked && !filteredDTSCheck.Checked)
                    {
                        checksPassed = false;
                        NLabelUpdate(" No Passthru filter Selected!", Color.Red);
                    }
                    break;
                default:
                    break;
            }

            notificationLabel.Visible = true;
            return checksPassed;
        }
        private string CheckForHandbrakeCLI()
        {
            if (System.IO.File.Exists((Directory.GetCurrentDirectory() + "\\HandBrakeCLI\\HandBrakeCLI.exe"))) //Check Hancbrake CLI is in current directory
            {
                return Directory.GetCurrentDirectory() + "\\HandBrakeCLI";
            }
            else
            { 
                CustomMessageBox.Show("HandBrakeCLI Not Found in " + Directory.GetCurrentDirectory() + "\\HandBrakeCLI", 200, 400, "File Not Found \"HandBrakeCLI.exe\"");
            }
            return "";
        }
        private string GenerateConversionString(string filepath, string filename, string outputPath)
        {
            double audioConversionBitrate = 0;
            double videoConversionBitrate = 0;
            double videoTotalTime = 0;
            double totalBitrate = 0;
            bool outputLargerThan4GB = false;
            string AudioString = "";
            int audioTrack = 0;
            string VideoString = "";
            string sourceOptions = "";
            string crop = "";

            MediaFile videoFile = new MediaFile(filepath);
            AudioString = AudioConversionString(videoFile);
            if(!disableCheckStream2.Checked)
            {
                audioTrack = int.Parse(Program.GeneralParser(AudioString, "--audio ", ","));

            }
            else
            {
                audioTrack = int.Parse(Program.GeneralParser(AudioString, "--audio ", " "));
            }
            

            VideoString = VideoConversionString(videoFile, audioTrack);

            if (videoFile.Audio.Count > 0 && videoFile.Video.Count > 0) //Only use if both video and auto streams are visible
            {
                //Estimate final file size
                audioConversionBitrate = double.Parse(Program.GeneralParser(AudioString, "--ab ", " "));
                videoConversionBitrate = double.Parse(Program.GeneralParser(VideoString, "--vb ", " "));
                totalBitrate = audioConversionBitrate + videoConversionBitrate;
                videoTotalTime = videoFile.General.DurationMillis; //
                videoTotalTime = videoTotalTime / 1000;

                if ((videoTotalTime * totalBitrate) > 4000000000) //Check if file is estimated to be larger than 4GB is so Mark as Large File
                {
                    outputLargerThan4GB = true;
                }
            }


            sourceOptions = SourceDestinationOptionsString(filepath, filename, outputPath, outputLargerThan4GB);
            //Stick with source dimensions - no crop, auto anamorphic, modulus 2 (used to be an issue but the code has been fixed so 2 is the best option for preserving source)
            
            if(autoCropCB.Checked)
            {
                crop = "--auto-anamorphic --modulus 2 "; //Auto Removes Black Bars, Changed from loose-anamorphic to --auto-anamorphic
            }
            else
            {
                crop = "--crop 0:0:0:0 --auto-anamorphic --modulus 2 "; //Forces 0 crop, Changed from loose-anamorphic to --auto-anamorphic
            }


            return "--verbose 1 " + sourceOptions + VideoString + AudioString + crop;

        }
        private string VideoConversionString(MediaFile videoFile, int audioTrack)
        {
            //Calculation Variables
            double videoBitrate = 0.0;
            string MaxBitrate = "";
            string BufferSize = "";
            double BitrateMultiplier = 1.5; //Size of the Maximum bitrate for the video portion of the file
            double BufferMultiplier = 2; //Size of the buffer

            //Variables from Form
            double avgBitrateCap = 0.0;
            bool burnForcedSubs = false;
            string subsToInclude = subtitleCombo.Text;
            if (burnInSubtitlesCheck.Checked) { burnForcedSubs = true; }

            //Output Variables
            string outputEncoder = ""; //encoder and speed preset
            string outputEncoderSpeed = "";
            string outputEncoderTune = ""; //Encoder tune
            string outputEncoderProfile = "";
            string outputEncoderLevel = "";
            string outputEncopts = ""; //advanced encoder settings
            string outputVideoBitrate = "";
            string outputTwoPass = "";
            string outputTurbo = "";
            string outputFrameRate = "";
            string subtitleString = "";
            string subtitleBurnString = "";
            //string subtitleSubString = "";
            


            if (videoFile.Video.Count > 0) //video stream found
            {

                /*Framerate**********************************************************************************************************************************************************************************************/
                /*Roku Compliant
                    Same As Source
                    5
                    10
                    12
                    15
                    23.976
                    24,
                    25
                    29.97
                    30
                    50
                    59.94
                    60*/
                switch (framerateCombo.Text)
                {
                    case "Roku Compliant": //Roku Compliant Force Framerates of 23.976 fps or 29.97 fps and CFR or PFR only
                        if (videoFile.Video[0].FrameRate > 0)
                        {
                            if (videoFile.Video[0].FrameRate == 23.976 || videoFile.Video[0].FrameRate == 29.97)
                            {
                                switch (frameRateModeCombo.Text)
                                {
                                    case "Constant": //Constant
                                        outputFrameRate = "--rate  " + videoFile.Video[0].FrameRate.ToString() + " --cfr "; //Constant Framerate
                                        break;
                                    case "Peak": //Peak
                                        outputFrameRate = "--rate  " + videoFile.Video[0].FrameRate.ToString() + " --pfr "; //Peak Framerate
                                        break;
                                    case "Variable": //Variable
                                        NLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
                                        outputFrameRate = "--rate  " + videoFile.Video[0].FrameRate.ToString() + " --pfr "; //Peak Framerate
                                        break;
                                    default: //Peak
                                        outputFrameRate = "--rate  " + videoFile.Video[0].FrameRate.ToString() + " --pfr "; //Peak Framerate
                                        break;
                                }
                            }
                            else if (videoFile.Video[0].FrameRate >= 29) //Force to 29.97
                            {
                                switch (frameRateModeCombo.Text)
                                {
                                    case "Constant": //Constant
                                        outputFrameRate = "--rate 29.97 --cfr "; //Constant Framerate
                                        break;
                                    case "Peak": //Peak
                                        outputFrameRate = "--rate 29.97 --pfr "; //Peak Framerate
                                        break;
                                    case "Variable": //Variable
                                        NLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
                                        outputFrameRate = "--rate 29.97 --pfr "; //Peak Framerate
                                        break;
                                    default: //Peak
                                        outputFrameRate = "--rate 29.97 --pfr "; //Peak Framerate
                                        break;
                                }
                            }
                            else //Force down to 23.976
                            {
                                switch (frameRateModeCombo.Text)
                                {
                                    case "Constant": //Constant
                                        outputFrameRate = "--rate 23.976 --cfr "; //Constant Framerate
                                        break;
                                    case "Peak": //Peak
                                        outputFrameRate = "--rate 23.976 --pfr "; //Peak Framerate
                                        break;
                                    case "Variable": //Variable
                                        NLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
                                        outputFrameRate = "--rate 23.976 --pfr "; //Peak Framerate
                                        break;
                                    default: //Peak
                                        outputFrameRate = "--rate 23.976 --pfr "; //Peak Framerate
                                        break;
                                }
                            }

                        }
                        break;
                    case "Same As Source": //Same As Source 
                        outputFrameRate = "--vfr"; //preserves the source timing.
                        break;
                    case "5": //5
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 5 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 5 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 5 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "10": //10
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 10 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 10 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 10 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "12": //12
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 12 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 12 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 12 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "15": //15
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 15 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 15 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 15 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "23.976": //23.976
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 23.976 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 23.976 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 23.976 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "24": //24
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 24 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 24 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 24 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "25": //25
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 25 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 25 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 25 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "29.97": //29.97
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 29.97 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 29.97 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 29.97 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "30": //30
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 30 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 30 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 30 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "50": //50
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 50 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 50 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 50 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "59.94": //59.94
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 59.94 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 59.94 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 59.94 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "60": //60
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 60 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 60 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 60 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    default:
                        NLabelUpdate("Framerate ignored, preserving source rate.", Color.Red);
                        outputFrameRate = "--vfr"; ////preserves the source timing.
                        break;
                }

                /*Encopts***********************************************************************************************************************************************************************************************/
                if (videoFile.Video[0].Properties.ContainsKey("Bit rate"))
                {
                    if (videoFile.Video[0].Properties["Bit rate"].Contains("Mbps")) //If file bitrate is in Mbps
                    {
                        double.TryParse(videoFile.Video[0].Properties["Bit rate"].Replace(" ", "").Replace("Mbps", ""), out videoBitrate);
                        videoBitrate = videoBitrate * 1000; //accounts for reading in Mbps rather than Kbps
                    }
                    else if (videoFile.Video[0].Properties["Bit rate"].Contains("Kbps")) //If file bitrate is in Mbps
                    {
                        double.TryParse(videoFile.Video[0].Properties["Bit rate"].Replace(" ", "").Replace("Kbps", ""), out videoBitrate);
                    }
                }

                if (videoBitrate == 0 && videoFile.General.Bitrate > 0)
                {
                    //check for audio bitrate and subtract from general bitrate
                    videoBitrate = videoFile.General.Bitrate - videoFile.Audio[audioTrack - 1].Bitrate;
                }

                if (videoBitrate == 0 && videoFile.General.Properties.ContainsKey("Overall bit rate"))
                {
                    if (videoFile.General.Properties["Overall bit rate"].Contains("Mbps"))
                    {
                        double.TryParse(videoFile.General.Properties["Overall bit rate"].Replace(" ", "").Replace("Mbps", ""), out videoBitrate);
                        videoBitrate = videoBitrate * 1000; //accounts for reading in Mbps rather than Kbps
                    }
                    else if (videoFile.General.Properties["Overall bit rate"].Contains("Kbps"))
                    {
                        double.TryParse(videoFile.General.Properties["Overall bit rate"].Replace(" ", "").Replace("Kbps", ""), out videoBitrate);
                    }
                }

                double.TryParse(avgBitrateCombo.Text, out avgBitrateCap);
                avgBitrateCap = avgBitrateCap * 1000; //This changes the value in the dropdown from Mbps to Kbps

                /*http://www.chaneru.com/Roku/HLS/X264_Settings.htm#vbv-maxrate
                  vbv-maxrate
                  Default: 0
                  Sets the maximum rate the VBV buffer should be assumed to refill at.
                  VBV reduces quality, so you should only use this if you're encoding for a playback scenario that requires it.
                  See also: --vbv-bufsize, --vbv-init, VBV Encoding Suggestions

                  vbv-bufsize
                  Default: 0
                  Sets the size of the VBV buffer in kilobits.
                  VBV reduces quality, so you should only use this if you're encoding for a playback scenario that requires it.
                  See also: --vbv-maxsize, --vbv-init, VBV Encoding Suggestions

                  vbv-init
                  Default: 0.9
                  Sets how full the VBV Buffer must be before playback starts.
                  If it is less than 1, the the initial fill is: vbv-init * vbv-bufsize. Otherwise it is interpreted as the initial fill in kbits.
                  See also: --vbv-maxsize, --vbv-bufsize, VBV Encoding Suggestions*/

                //Abide by avgBitrateCap value                   
                if (videoBitrate > avgBitrateCap)
                {
                    outputVideoBitrate = "--vb " + avgBitrateCap + " ";
                    if (Math.Floor(avgBitrateCap * BitrateMultiplier) < 10000)
                    {
                        MaxBitrate = Math.Floor(avgBitrateCap * BitrateMultiplier).ToString();
                        BufferSize = Math.Floor((avgBitrateCap * BitrateMultiplier) * BufferMultiplier).ToString(); //Buffer of 2 seconds
                    }
                    else
                    {
                        MaxBitrate = "10000"; //Max value for Roku Players
                        BufferSize = "20000"; // buffer of 2 seconds
                    }
                }
                else
                {
                    outputVideoBitrate = "--vb " + videoBitrate.ToString() + " ";

                    if (Math.Floor(videoBitrate * BitrateMultiplier) < 10000)
                    {
                        MaxBitrate = Math.Floor(videoBitrate * BitrateMultiplier).ToString();
                        BufferSize = Math.Floor((videoBitrate * BitrateMultiplier) * BufferMultiplier).ToString();
                    }
                    else
                    {
                        MaxBitrate = "10000";
                        BufferSize = "20000";
                    }

                }

                //These settings set the buffer size and maximum video bitrate, also setting the encoder level
                outputEncopts = "--encopts level=" + encoderLevelComboBox.Text + ":vbv-bufsize=" + BufferSize + ":vbv-maxrate=" + MaxBitrate + " --verbose=1 --encoder-level=\"" + encoderLevelComboBox.Text + "\" --encoder-profile=" + encoderProfileComboBox.Text.ToLower() + "--verbose=1 ";
            }
            else //Video stream is unreadable - set user selected values or defaults
            {

                /*Encopts***********************************************************************************************************************************************************************************************/
                //Need to adjust for number of channels of audio, currently it is the value for mono not stereo which is typical.

                double.TryParse(avgBitrateCombo.Text, out avgBitrateCap);
                avgBitrateCap = avgBitrateCap * 1000; //This changes the value in the dropdown from Mbps to Kbps
                videoBitrate = avgBitrateCap;
                double.TryParse(audioBitrateCombo.Text, out double audioBitrate);

                outputVideoBitrate = "--vb " + videoBitrate.ToString() + " ";

                if (Math.Floor(videoBitrate * BitrateMultiplier) < 10000) //Ensures max bitrate doesn't go over 10 which is the limit for Roku compatibility
                {
                    MaxBitrate = Math.Floor(videoBitrate * BitrateMultiplier).ToString();
                    BufferSize = Math.Floor((videoBitrate * BitrateMultiplier) * BufferMultiplier).ToString();
                }
                else
                {
                    MaxBitrate = "10000"; //Max for Roku Devices
                    BufferSize = "20000"; //2 times the MaxBitrate
                }

                //These settings set the buffer size and maximum video bitrate, also setting the encoder level
                outputEncopts = "--encopts level=" + encoderLevelComboBox.Text + ":vbv-bufsize=" + BufferSize + ":vbv-maxrate=" + MaxBitrate + " --verbose=1 --encoder-level=\"" + encoderLevelComboBox.Text + "\" --encoder-profile=" + encoderProfileComboBox.Text.ToLower() + "--verbose=1 ";

                /*Framerate***********************************************************************************************************************************************************************************************/
                /*Framerate***********************************************************************************************************************************************************************************************/
                /*Roku Compliant
                    Same As Source
                    5
                    10
                    12
                    15
                    23.976
                    24,
                    25
                    29.97
                    30
                    50
                    59.94
                    60*/
                switch (framerateCombo.Text)
                {
                    case "Roku Compliant": //Roku Compliant Force Framerates of 23.976 fps or 29.97 fps and CFR or PFR only
                        if (videoFile.Video[0].FrameRate > 0)
                        {
                            if (videoFile.Video[0].FrameRate == 23.976 || videoFile.Video[0].FrameRate == 29.97)
                            {
                                switch (frameRateModeCombo.Text)
                                {
                                    case "Constant": //Constant
                                        outputFrameRate = "--rate  " + videoFile.Video[0].FrameRate.ToString() + " --cfr "; //Constant Framerate
                                        break;
                                    case "Peak": //Peak
                                        outputFrameRate = "--rate  " + videoFile.Video[0].FrameRate.ToString() + " --pfr "; //Peak Framerate
                                        break;
                                    case "Variable": //Variable
                                        NLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
                                        outputFrameRate = "--rate  " + videoFile.Video[0].FrameRate.ToString() + " --pfr "; //Peak Framerate
                                        break;
                                    default: //Peak
                                        outputFrameRate = "--rate  " + videoFile.Video[0].FrameRate.ToString() + " --pfr "; //Peak Framerate
                                        break;
                                }
                            }
                            else if (videoFile.Video[0].FrameRate >= 29) //Force to 29.97
                            {
                                switch (frameRateModeCombo.Text)
                                {
                                    case "Constant": //Constant
                                        outputFrameRate = "--rate 29.97 --cfr "; //Constant Framerate
                                        break;
                                    case "Peak": //Peak
                                        outputFrameRate = "--rate 29.97 --pfr "; //Peak Framerate
                                        break;
                                    case "Variable": //Variable
                                        NLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
                                        outputFrameRate = "--rate 29.97 --pfr "; //Peak Framerate
                                        break;
                                    default: //Peak
                                        outputFrameRate = "--rate 29.97 --pfr "; //Peak Framerate
                                        break;
                                }
                            }
                            else //Force down to 23.976
                            {
                                switch (frameRateModeCombo.Text)
                                {
                                    case "Constant": //Constant
                                        outputFrameRate = "--rate 23.976 --cfr "; //Constant Framerate
                                        break;
                                    case "Peak": //Peak
                                        outputFrameRate = "--rate 23.976 --pfr "; //Peak Framerate
                                        break;
                                    case "Variable": //Variable
                                        NLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
                                        outputFrameRate = "--rate 23.976 --pfr "; //Peak Framerate
                                        break;
                                    default: //Peak
                                        outputFrameRate = "--rate 23.976 --pfr "; //Peak Framerate
                                        break;
                                }
                            }

                        }
                        break;
                    case "Same As Source": //Same As Source 
                        outputFrameRate = "--vfr"; //preserves the source timing.
                        break;
                    case "5": //5
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 5 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 5 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 5 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "10": //10
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 10 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 10 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 10 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "12": //12
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 12 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 12 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 12 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "15": //15
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 15 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 15 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 15 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "23.976": //23.976
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 23.976 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 23.976 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 23.976 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "24": //24
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 24 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 24 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 24 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "25": //25
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 25 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 25 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 25 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "29.97": //29.97
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 29.97 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 29.97 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 29.97 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "30": //30
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 30 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 30 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 30 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "50": //50
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 50 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 50 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 50 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "59.94": //59.94
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 59.94 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 59.94 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 59.94 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    case "60": //60
                        switch (frameRateModeCombo.Text)
                        {
                            case "Constant": //Constant
                                outputFrameRate = "--rate 60 --cfr "; //Constant Framerate
                                break;
                            case "Peak": //Peak
                                outputFrameRate = "--rate 60 --pfr "; //Peak Framerate
                                break;
                            case "Variable": //Variable
                                NLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 60 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    default:
                        NLabelUpdate("Framerate ignored, preserving source rate.", Color.Red);
                        outputFrameRate = "--vfr"; ////preserves the source timing.
                        break;
                }
            }
            /*TwoPass & Turbo First***********************************************************************************************************************************************************************************************/
            if (twoPassCheckbox.Checked) { outputTwoPass = "--two-pass "; }
            if (turboCheckBox.Checked) { outputTurbo = "--turbo "; }

            /*Encoder * **********************************************************************************************************************************************************************************************/
            outputEncoder = "--encoder x264 ";
            /*Encoder Speed***********************************************************************************************************************************************************************************************/
            switch (encoderSpeedCombo.Text)
            {
                case "Ultra Fast":
                    outputEncoderSpeed = "--x264-preset ultrafast ";
                    break;
                case "Super Fast":
                    outputEncoderSpeed = "--x264-preset superfast ";
                    break;
                case "Very Fast":
                    outputEncoderSpeed = "--x264-preset veryfast ";
                    break;
                case "Faster":
                    outputEncoderSpeed = "--x264-preset faster ";
                    break;
                case "Fast":
                    outputEncoderSpeed = "--x264-preset fast ";
                    break;
                case "Medium":
                    outputEncoderSpeed = "--x264-preset medium ";
                    break;
                case "Slow":
                    outputEncoderSpeed = "--x264-preset slow ";
                    break;
                case "Slower":
                    outputEncoderSpeed = "--x264-preset slower ";
                    break;
                case "Very Slow":
                    outputEncoderSpeed = "--x264-preset veryslow ";
                    break;
                case "Placebo":
                    outputEncoderSpeed = "--x264-preset placebo ";
                    break;
                default:
                    outputEncoderSpeed = "--x264-preset medium ";
                    break;
            }
            /*Encoder Tune***********************************************************************************************************************************************************************************************/

            switch (encoderTuneComboBox.Text)
            {
                case "Animation":
                    outputEncoderTune = "--x264-tune animation ";
                    break;
                case "Fast Decode":
                    outputEncoderTune = "--x264-tune fastdecode ";
                    break;
                case "Film":
                    outputEncoderTune = "--x264-tune film ";
                    break;
                case "Grain":
                    outputEncoderTune = "--x264-tune grain ";
                    break;
                case "None":
                    outputEncoderTune = "";
                    break;
                case "Still Image":
                    outputEncoderTune = "--x264-tune stillimage ";
                    break;
                case "Zero Latency":
                    outputEncoderTune = "--x264-tune zerolatency ";
                    break;
                default:
                    outputEncoderTune = "";
                    break;
            }

            /*Encoder Profile***********************************************************************************************************************************************************************************************/

            switch (encoderProfileComboBox.Text)
            {
                case "Baseline":
                    outputEncoderProfile = "--x264-profile baseline ";
                    break;
                case "Main":
                    outputEncoderProfile = "--x264-profile main ";
                    break;
                case "High":
                    outputEncoderProfile = "--x264-profile high ";
                    break;
                default:
                    outputEncoderProfile = "--x264-profile high ";
                    break;
            }
            /*Encoder Level***********************************************************************************************************************************************************************************************/
            switch (encoderLevelComboBox.Text)
            {
                case "1.0":
                    outputEncoderLevel = "--encoder-level 1.0 ";
                    break;
                case "1b":
                    outputEncoderLevel = "--encoder-level 1b ";
                    break;
                case "1.1":
                    outputEncoderLevel = "--encoder-level 1.1 ";
                    break;
                case "1.2":
                    outputEncoderLevel = "--encoder-level 1.2 ";
                    break;
                case "1.3":
                    outputEncoderLevel = "--encoder-level 1.3 ";
                    break;
                case "2.0":
                    outputEncoderLevel = "--encoder-level 2.0 ";
                    break;
                case "2.1":
                    outputEncoderLevel = "--encoder-level 2.1 ";
                    break;
                case "2.2":
                    outputEncoderLevel = "--encoder-level 2.2 ";
                    break;
                case "3.0":
                    outputEncoderLevel = "--encoder-level 3.0 ";
                    break;
                case "3.1":
                    outputEncoderLevel = "--encoder-level 3.1 ";
                    break;
                case "3.2":
                    outputEncoderLevel = "--encoder-level 3.2 ";
                    break;
                case "4.0":
                    outputEncoderLevel = "--encoder-level 4.0 ";
                    break;
                case "4.1":
                    outputEncoderLevel = "--encoder-level 4.1 ";
                    break;
                default:
                    outputEncoderLevel = "--encoder-level 4.0 ";
                    break;
            }

            //Subtitle Options
            //burnForcedSubs
            //subsToInclude
            //Determine number of subtitle streams

            /*None
            All
            Default
            First
            Chinese
            Czech
            English
            Finnish
            French
            German
            Greek
            Japanese
            Korean
            Portuguese
            Russian
            Spanish
            Swedish*/

            //PGS can only be burned into video, not passed through. Ignore PGS here.
            if (videoFile.Text.Count() > 0)
            {

                switch(subtitleCombo.Text)
                {
                    case "None":
                        subtitleString = "";
                        break;
                    case "All":
                        for (int i = 0; i < videoFile.Text.Count(); i++)
                        {
                            //Check for PGS subtitles and ignore them, they cannot be passed through only burned.
                            if(videoFile.Text[i].Properties.ContainsKey("Format"))
                            {
                                if(!videoFile.Text[i].Properties["Format"].Contains("PGS"))
                                {
                                    if (string.IsNullOrEmpty(subtitleString)) { subtitleString = "--subtitle \"" + (i + 1).ToString(); }
                                    else { subtitleString += "," + (i + 1).ToString(); }
                                }
                            }  
                        }
                        if (!string.IsNullOrEmpty(subtitleString))
                        {
                            subtitleString += "\" "; //adds the last quote and space 
                        }
                        break;
                    case "Default": //Adds first track with default flag, there should only be one and tags it as default.
                        for (int i = 0; i < videoFile.Text.Count(); i++)
                        {
                            //Check for PGS subtitles and ignore them, they cannot be passed through only burned.
                            if (videoFile.Text[i].Properties.ContainsKey("Format"))
                            {
                                if (!videoFile.Text[i].Properties["Format"].Contains("PGS"))
                                {
                                    if (string.IsNullOrEmpty(subtitleString))
                                    {
                                        if (videoFile.Text[i].Properties.ContainsKey("Default"))
                                        {
                                            subtitleString = "--subtitle-default " + (i + 1).ToString() + " ";
                                        }
                                    }
                                }
                            }
                            
                        }
                        break;
                    case "First": //Adds the first subtitle track regarless of what it is.
                                  //Check for PGS subtitles and ignore them, they cannot be passed through only burned.
                        if (videoFile.Text[0].Properties.ContainsKey("Format"))
                        {
                            if (!videoFile.Text[0].Properties["Format"].Contains("PGS"))
                            {
                                subtitleString = "--subtitle \"1\"";
                            }
                        }
                        
                        break;
                    default: //All other language codes
                        for (int i = 0; i < videoFile.Text.Count(); i++)
                        {
                            //Check for PGS subtitles and ignore them, they cannot be passed through only burned.
                            if (videoFile.Text[i].Properties.ContainsKey("Format"))
                            {
                                if (!videoFile.Text[i].Properties["Format"].Contains("PGS"))
                                {
                                    if (videoFile.Text[i].Properties.ContainsKey("Language"))
                                    {
                                        if (videoFile.Text[i].Properties["Language"] == subsToInclude)
                                        {
                                            if (string.IsNullOrEmpty(subtitleString))
                                            {
                                                subtitleString = "--subtitle \"" + (i + 1).ToString();
                                            }
                                            else
                                            {
                                                subtitleString += "," + (i + 1).ToString();
                                            }
                                        }
                                    }
                                }
                            }
                            
                        }
                        if (!string.IsNullOrEmpty(subtitleString))
                        {
                            subtitleString += "\" "; //adds the last quote and space 
                        } 
                        break;
                }

                //Burn Forced Subtitles Checked
                if(burnForcedSubs)
                {
                    //Check for forced flag, and English
                    for (int i = 0; i < videoFile.Text.Count(); i++)
                    {
                        if (string.IsNullOrEmpty(subtitleBurnString))
                        {
                            if (videoFile.Text[i].Properties.ContainsKey("Forced"))
                            {
                                if (videoFile.Text[i].Properties["Forced"] == "Yes")
                                {
                                    if (videoFile.Text[i].Properties.ContainsKey("Language"))
                                    {
                                        if (videoFile.Text[i].Properties["Language"] == "English")
                                        {
                                            subtitleBurnString = "--subtitle-burned=" + (i + 1).ToString() + " ";
                                        }
                                    }
                                }
                            }
                        }
                        
                    }
                    if(string.IsNullOrEmpty(subtitleBurnString)) //Search for srt file in the same folder named the same as the file with a -Forced on the end.
                    {
                        char delim = '.';
                        string[] Tokens = videoFile.File.Split(delim);

                        //This looks weird because handbrake doesn't properly handle the \ escape character in the input srt string. I had to add extra so the output would have double \\ instead.
                        string forcedSRTFileName = videoFile.File.Replace("\\","\\\\").Replace("." + Tokens[Tokens.Count() - 1].ToString(), "-Forced.srt");

                        if(System.IO.File.Exists(forcedSRTFileName))
                        {
                            subtitleBurnString = "--srt-file \"" + forcedSRTFileName + "\" --srt-burn " + "--srt-codeset UTF-8 ";
                        }
                    }
                }

            }

            return outputEncoder + outputEncoderSpeed + outputEncoderTune + subtitleString + subtitleBurnString + outputEncopts + outputEncoderProfile + outputEncoderLevel + outputVideoBitrate + outputTwoPass + outputTurbo + outputFrameRate ;
        }
        private string AudioConversionString(MediaFile videoFile)
        {
            /*12/9/18 - addiing support for secondary 5.1 audio track if file contains a 5.1 or above track. Note* at this time Handbrake is only capable of encoding up to 5.1 in E-AC3 format
            and ROKU can only decode AAC, AC3, and E-AC3 formats while it can passthrough DTS. Thus to keep files as universal as possible to prevent transcoding on the server
            we must maintain 5.1 audio as the highest level audio possible in the second stream.*/

            //Users selected variables
            double userSelectedBitrate = 0; //Bitrate for audio is per channel so stereo audio of 96 bitrate would actually be 96 * 2 = 192
            double userSelectedBitrate2 = 0; //Bitrate for audio is per channel so stereo audio of 96 bitrate would actually be 96 * 2 = 192
            double userSelectedBitrate3 = 0; //Bitrate for audio is per channel so stereo audio of 96 bitrate would actually be 96 * 2 = 192

            //Variables derived from file
            double bitrateOfFile = 0; //used to determine the bitrate of the actual audio.
            int audioTrackNumber = 0; //Highest bitrate audio track in English
            double maxBitrate = 0;

            //Variables for output string
            string outputAudioPassthruMask = "";
            string outputAudioTrack = "";
            string outputEncoder = "";
            string outputFallBack = "";
            string outputMixdown = "";
            string outputSampleRate = "--arate 48 "; //Auto is no longer listd as an option in handbrake cli documentation 2/2017.
            string outputDynamicRange = "--drc 0 --gain 0 ";

            string outputBitrate = "";
            string outputBitrate2 = "";
            string outputBitrate3 = "";

            string mixdown1 = "";
            string mixdown2 = "";
            string mixdown3 = "";
            /*****************************************************************************************************************************************************************************************************************************/

            if (videoFile.Audio.Count > 0) //Source Readable
            {
                /*Select Audio Track to Use***********************************************************************************************************************************************************************************************/
                //Select Audio Track to Use - Highest Bitrate Audio in English
                for (int i = 0; i < videoFile.Audio.Count; i++)
                {
                    //Check for English Language Track
                    if (videoFile.Audio[i].Properties.ContainsKey("Language"))
                    {
                        if (videoFile.Audio[i].Properties["Language"].ToUpper() == "ENGLISH" ||
                            videoFile.Audio[i].Properties["Language"].ToUpper() == "ENG" ||
                            videoFile.Audio[i].Properties["Language"].ToUpper() == "EN")
                        {
                            //Check for Max per channel Bitrate English Track
                            if(videoFile.Audio[i].Channels > 0)
                            {
                                if ((videoFile.Audio[i].Bitrate / videoFile.Audio[i].Channels) > maxBitrate)
                                {
                                    maxBitrate = videoFile.Audio[i].Bitrate / videoFile.Audio[i].Channels; //Get the per channel bitrate
                                    audioTrackNumber = i; //Mark the audio track that has the highest bitrate
                                }
                            }
                            else
                            {
                                //ATMOS tracks are object oriented and show 0 channels
                                if (videoFile.Audio[i].Description.Contains("ATMOS"))
                                {
                                    maxBitrate = videoFile.Audio[i].Bitrate / 8; //7.1 audio has 8 channels, ATMOS is at least 7.1 always.
                                    audioTrackNumber = i; //Mark the audio track that has the highest bitrate
                                }
                                //Dolby TrueHD tracks are object based and show 0 channels
                                else if (videoFile.Audio[audioTrackNumber].Properties.ContainsKey("Channel(s)"))
                                {
                                    if (videoFile.Audio[audioTrackNumber].Properties["Channel(s)"].Contains("Object Based"))
                                    {
                                        maxBitrate = videoFile.Audio[i].Bitrate / 8; //7.1 audio has 8 channels, ATMOS is at least 7.1 always.
                                        audioTrackNumber = i; //Mark the audio track that has the highest bitrate
                                    }
                                }
                                else if ((videoFile.Audio[i].Bitrate / 2) > maxBitrate) // Assume at least stereo
                                {
                                    maxBitrate = videoFile.Audio[i].Bitrate / 2; //Get the per channel bitrate
                                    audioTrackNumber = i; //Mark the audio track that has the highest bitrate
                                }
                            }
                            
                        }
                    }
                    else //No Language code
                    {
                        //Check for Max Bitrate Track
                        if (videoFile.Audio[i].Channels > 0)
                        {
                            if ((videoFile.Audio[i].Bitrate / videoFile.Audio[i].Channels) > maxBitrate)
                            {
                                maxBitrate = videoFile.Audio[i].Bitrate / videoFile.Audio[i].Channels; //Get the per channel bitrate
                                audioTrackNumber = i; //Mark the audio track that has the highest bitrate
                            }
                        }
                        else
                        {
                            //ATOMS tracks are object oriented and show 0 channels
                            if(videoFile.Audio[i].Description.Contains("ATMOS"))
                            {
                                //Identified as ATMOS 8 channels
                                if ((videoFile.Audio[i].Bitrate / 8) > maxBitrate)
                                {
                                    maxBitrate = videoFile.Audio[i].Bitrate / 8; //7.1 audio has 8 channels, ATMOS is at least 7.1 always.
                                    audioTrackNumber = i; //Mark the audio track that has the highest bitrate
                                }
                                   
                            }
                            else if (videoFile.Audio[audioTrackNumber].Properties.ContainsKey("Channel(s)_Original"))
                            {
                                //Identified as DolbyHD 8 channels
                                if ((videoFile.Audio[i].Bitrate / 8) > maxBitrate)
                                {
                                    maxBitrate = videoFile.Audio[i].Bitrate / 8; //7.1 audio has 8 channels, DolbyHD is at least 7.1 always.
                                    audioTrackNumber = i; //Mark the audio track that has the highest bitrate
                                }
                            }
                            //Dolby TrueHD tracks are object based and show 0 channels
                            else if (videoFile.Audio[audioTrackNumber].Properties.ContainsKey("Channel(s)"))
                            {
                                if (videoFile.Audio[audioTrackNumber].Properties["Channel(s)"].Contains("Object Based"))
                                {
                                    //Identified as TrueHD 8 channels
                                    if ((videoFile.Audio[i].Bitrate / 8) > maxBitrate)
                                    {
                                        maxBitrate = videoFile.Audio[i].Bitrate / 8; //7.1 audio has 8 channels, ATMOS is at least 7.1 always.
                                        audioTrackNumber = i; //Mark the audio track that has the highest bitrate
                                    }
                                }
                            }
                            else if ((videoFile.Audio[i].Bitrate / 2) > maxBitrate) // Assume at least stereo
                            {
                                maxBitrate = videoFile.Audio[i].Bitrate / 2; //Get the per channel bitrate
                                audioTrackNumber = i; //Mark the audio track that has the highest bitrate
                            }
                        }
                    }
                }

                if(!disableCheckStream2.Checked) //Track 2 Enabled
                {

                    if (!disableCheckStream3.Checked) //Track 3 Enabled
                    {
                        outputAudioTrack = "--audio " + (audioTrackNumber + 1).ToString() + "," + (audioTrackNumber + 1).ToString() + "," + (audioTrackNumber + 1).ToString() + " ";
                    }
                    else //Track 3 Disabled
                    {
                        outputAudioTrack = "--audio " + (audioTrackNumber + 1).ToString() + "," + (audioTrackNumber + 1).ToString() + " ";
                    }
                }
                else //Track 2 disabled (Assume Track 3 is disabled also)
                {
                    outputAudioTrack = "--audio " + (audioTrackNumber + 1).ToString() + " ";
                }


                /*Samplerate***********************************************************************************************************************************************************************************************/
                string samplerate1 = "";
                string samplerate2 = "";
                string samplerate3 = "";

                //Samplerate1
                if (string.IsNullOrEmpty(sampleRateCombo.Text))
                {
                    if (videoFile.Audio[audioTrackNumber].SamplingRate / 1000 >= 48)
                    {
                        samplerate1 = "48"; //Roku Compatible high rate
                    }
                    else if(videoFile.Audio[audioTrackNumber].SamplingRate / 1000 == 44.1)
                    {
                        samplerate1 = "44.1"; //Roku compatible low rate
                    }
                    else
                    {
                        samplerate1 = "48"; //Default to 48
                    }
                }
                else //Use user selected samplerate
                {
                    samplerate1 = sampleRateCombo.Text;
                }
                
                //Samplerate 2
                if(!disableCheckStream2.Checked) //Stream 2 enabled
                {
                    if (string.IsNullOrEmpty(sampleRateCombo2.Text))
                    {

                        if (videoFile.Audio[audioTrackNumber].SamplingRate / 1000 >= 48)
                        {
                            samplerate2 = ",48"; //Roku Compatible high rate
                        }
                        else if (videoFile.Audio[audioTrackNumber].SamplingRate / 1000 == 44.1)
                        {
                            samplerate2 = ",44.1"; //Roku compatible low rate
                        }
                        else
                        {
                            samplerate2 = ",48"; //Default to 48
                        }
                    }
                    else //Use user selected samplerate
                    {
                        samplerate2 = "," + sampleRateCombo2.Text;
                    }
                }

                //Samplerate 3
                if (!disableCheckStream3.Checked) //Stream 3 enabled
                {
                    if (string.IsNullOrEmpty(sampleRateCombo3.Text))
                    {

                        if (videoFile.Audio[audioTrackNumber].SamplingRate / 1000 >= 48)
                        {
                            samplerate3 = ",48"; //Roku Compatible high rate
                        }
                        else if (videoFile.Audio[audioTrackNumber].SamplingRate / 1000 == 44.1)
                        {
                            samplerate3 = ",44.1"; //Roku compatible low rate
                        }
                        else
                        {
                            samplerate3 = ",48"; //Default to 48
                        }
                    }
                    else //Use user selected samplerate
                    {
                        samplerate3 = "," + sampleRateCombo3.Text;
                    }
                }

                outputSampleRate = "--arate " + samplerate1 + samplerate2 + samplerate3 + " ";

                /*Bitrate***********************************************************************************************************************************************************************************************/

                //Determine per channel Bitrate selected by user
                try { userSelectedBitrate = int.Parse(audioBitrateCombo.Text); } catch { userSelectedBitrate = 96; }//default value is 96
                try { userSelectedBitrate2 = int.Parse(audioBitrateCombo2.Text); } catch { userSelectedBitrate2 = 96; }//default value is 96
                try { userSelectedBitrate3 = int.Parse(audioBitrateCombo3.Text); } catch { userSelectedBitrate3 = 96; }//default value is 96

                //Determine bitrate of file
                if (videoFile.Audio[audioTrackNumber].Bitrate != 0)
                {
                    
                    if(videoFile.Audio[audioTrackNumber].Channels == 0)
                    {
                        //ATOMS tracks are object based and show 0 channels
                        if (videoFile.Audio[audioTrackNumber].Description.Contains("ATMOS"))
                        {
                            bitrateOfFile = videoFile.Audio[audioTrackNumber].Bitrate / 8; //7.1 audio has 8 channels, ATMOS is at least 7.1 always.
                        }
                        //Dolby TrueHD tracks are object based and show 0 channels
                        else if (videoFile.Audio[audioTrackNumber].Properties.ContainsKey("Channel(s)"))
                        {
                            if (videoFile.Audio[audioTrackNumber].Properties["Channel(s)"].Contains("Object Based"))
                            {
                                bitrateOfFile = videoFile.Audio[audioTrackNumber].Bitrate / 8; //The Dolby TrueHD  detected, 7.1 audio has 8 channels.
                            }
                        }
                        else
                        {
                            bitrateOfFile = videoFile.Audio[audioTrackNumber].Bitrate / 2; //assume at least stereo.
                        }

                    }
                    //Valid Bitrate Identified, Valid Channel count identified
                    else
                    {
                        //Determine per channel Bitrate of file
                        bitrateOfFile = videoFile.Audio[audioTrackNumber].Bitrate / videoFile.Audio[audioTrackNumber].Channels;
                    }
                    
                }
                else if (videoFile.Audio[audioTrackNumber].Properties.ContainsKey("Bit rate"))
                {
                    try { double.TryParse(videoFile.Audio[audioTrackNumber].Properties["Bit rate"].Replace(" ", "").Replace("Unknown/","").Replace("Kbps", ""), out bitrateOfFile); }
                    catch { bitrateOfFile = 0; }

                    if(videoFile.Audio[audioTrackNumber].Description.Contains("ATMOS"))
                    {
                        bitrateOfFile = bitrateOfFile / 8; //ATMOS tracks are object based and are 7.1 (8) channel audio
                    }
                    //Dolby TrueHD tracks are object based and show 0 channels
                    else if (videoFile.Audio[audioTrackNumber].Properties.ContainsKey("Channel(s)"))
                    {
                        if (videoFile.Audio[audioTrackNumber].Properties["Channel(s)"].Contains("Object Based"))
                        {
                            bitrateOfFile = bitrateOfFile / 8; //The Dolby TrueHD detected, 7.1 audio has 8 channels.
                        }
                    }
                    else if (videoFile.Audio[audioTrackNumber].Properties.ContainsKey("Channel(s)_Original"))
                    {
                        if (videoFile.Audio[audioTrackNumber].Properties["Channel(s)"].Contains("Object Based"))
                        {
                            bitrateOfFile = bitrateOfFile / 8; //The Dolby Audio detected, 7.1 audio has 8 channels.
                        }
                    }
                    else
                    {
                        bitrateOfFile = bitrateOfFile / videoFile.Audio[audioTrackNumber].Channels;
                    }
                    
                }


                //If per channel Bitrate of file is < Selected Bitrate use the file's bitrate.
                if ((bitrateOfFile < userSelectedBitrate) && (bitrateOfFile > 0)) { userSelectedBitrate = bitrateOfFile; }
                if ((bitrateOfFile < userSelectedBitrate2) && (bitrateOfFile > 0)) { userSelectedBitrate2 = bitrateOfFile; }
                if ((bitrateOfFile < userSelectedBitrate3) && (bitrateOfFile > 0)) { userSelectedBitrate3 = bitrateOfFile; }


                /*Fallback***********************************************************************************************************************************************************************************************/
                /*Set audio codec to use when it is not possible to copy an audio track without re-encoding.*/

                if(audioCodecComboBox.Text == "Filtered Passthru" || audioCodecComboBox2.Text == "Filtered Passthru" || audioCodecComboBox3.Text == "Filtered Passthru")
                {
                    if(!disableCheckStream2.Checked) //2nd Stream Enabled
                    {
                        outputFallBack = "--audio-fallback eac3 ";
                    }
                    else //First stream should always be aac as it's universal.
                    {
                        outputFallBack = "--audio-fallback fdk_aac ";
                    }
                    
                }

                /*Mixdown***********************************************************************************************************************************************************************************************/

                //The options are mono, stereo, dpl1 (Dolby Surround), dpl2 (Dolby ProLogic? 2), or 5point1 (5.1).
                if(videoFile.Audio[audioTrackNumber].Properties.ContainsKey("Channel(s)_Original")) //Assume 8 Channels
                {
                    switch (audioCodecComboBox.Text)
                    {
                        case "Filtered Passthru":
                            mixdown1 = "5point1";
                            userSelectedBitrate = userSelectedBitrate * 8;
                            break;
                        case "AAC (FDK)": //only good up to Dolby Pro Logic 2 (2 channel)
                            mixdown1 = "dpl2";
                            userSelectedBitrate = userSelectedBitrate * 2;
                            break;
                        default:
                            switch (mixdownComboBox.Text)
                            {
                                case "Dolby ProLogic 2":
                                    mixdown1 = "dpl2";
                                    userSelectedBitrate = userSelectedBitrate * 2;
                                    break;
                                default: //5.1 Audio
                                    mixdown1 = "5point1";
                                    userSelectedBitrate = userSelectedBitrate * 6;
                                    break;
                            }
                            break;
                    }

                    if (!disableCheckStream2.Checked) //Enabled
                    {
                        switch (audioCodecComboBox2.Text)
                        {
                            case "Filtered Passthru":
                                mixdown2 = ",5point1";
                                userSelectedBitrate2 = userSelectedBitrate2 * 8;
                                break;
                            case "AAC (FDK)": //only good up to Dolby Pro Logic 2 (2 channel)
                                mixdown2 = ",dpl2";
                                userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                break;
                            default:
                                switch (mixdownComboBox2.Text)
                                {
                                    case "Dolby ProLogic 2":
                                        mixdown2 = ",dpl2";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                        break;
                                    default: //5.1 Audio
                                        mixdown2 = ",5point1";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 6;
                                        break;
                                }
                                break;
                        }

                        if (!disableCheckStream3.Checked) //Enabled
                        {
                            switch (audioCodecComboBox3.Text)
                            {
                                case "Filtered Passthru":
                                    mixdown3 = ",5point1";
                                    userSelectedBitrate3 = userSelectedBitrate3 * 8;
                                    break;
                                case "AAC (FDK)": //only good up to Dolby Pro Logic 2 (2 channel)
                                    mixdown3 = ",dpl2";
                                    userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                    break;
                                default:
                                    switch (mixdownComboBox3.Text)
                                    {
                                        case "Dolby ProLogic 2":
                                            mixdown3 = ",dpl2";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                            break;
                                        default: //5.1 Audio
                                            mixdown3 = ",5point1";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 6;
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    switch (audioCodecComboBox.Text)
                    {
                        case "Filtered Passthru":
                            switch (videoFile.Audio[audioTrackNumber].Channels)
                            {
                                case 1:
                                    mixdown1 = "mono";
                                    userSelectedBitrate = userSelectedBitrate * 1;
                                    break;
                                case 2:
                                    mixdown1 = "dpl2";
                                    userSelectedBitrate = userSelectedBitrate * 2;
                                    break;
                                case 3:
                                    mixdown1 = "dpl2";
                                    userSelectedBitrate = userSelectedBitrate * 2;
                                    break;
                                case 4:
                                    mixdown1 = "dpl2";
                                    userSelectedBitrate = userSelectedBitrate * 2;
                                    break;
                                case 5:
                                    mixdown1 = "dpl2";
                                    userSelectedBitrate = userSelectedBitrate * 2;
                                    break;
                                case 6:
                                    mixdown1 = "5point1";
                                    userSelectedBitrate = userSelectedBitrate * 6;
                                    break;
                                case 7:
                                    mixdown1 = "5point1";
                                    userSelectedBitrate = userSelectedBitrate * 6;
                                    break;
                                case 8:
                                    mixdown1 = "5point1";
                                    userSelectedBitrate = userSelectedBitrate * 6;
                                    break;
                                default:
                                    mixdown1 = "dpl2";
                                    userSelectedBitrate = userSelectedBitrate * 2;
                                    break;
                            }
                            break;
                        case "AAC (FDK)": //only good up to Dolby Pro Logic 2 (2 channel)
                            switch (videoFile.Audio[audioTrackNumber].Channels)
                            {
                                case 1:
                                    mixdown1 = "mono";
                                    userSelectedBitrate = userSelectedBitrate * 1; ;
                                    break;
                                default:
                                    mixdown1 = "dpl2";
                                    userSelectedBitrate = userSelectedBitrate * 2;
                                    break;
                            }
                            break;
                        default:
                            switch (mixdownComboBox.Text)
                            {
                                case "Dolby ProLogic 2":
                                    mixdown1 = "dpl2";
                                    userSelectedBitrate = userSelectedBitrate * 2;
                                    break;
                                default: //5.1 Audio
                                    switch (videoFile.Audio[audioTrackNumber].Channels)
                                    {
                                        case 1:
                                            mixdown1 = "mono";
                                            userSelectedBitrate = userSelectedBitrate * 1;
                                            break;
                                        case 2:
                                            mixdown1 = "dpl2";
                                            userSelectedBitrate = userSelectedBitrate * 2;
                                            break;
                                        case 3:
                                            mixdown1 = "dpl2";
                                            userSelectedBitrate = userSelectedBitrate * 2;
                                            break;
                                        case 4:
                                            mixdown1 = "dpl2";
                                            userSelectedBitrate = userSelectedBitrate * 2;
                                            break;
                                        case 5:
                                            mixdown1 = "dpl2";
                                            userSelectedBitrate = userSelectedBitrate * 2;
                                            break;
                                        case 6:
                                            mixdown1 = "5point1";
                                            userSelectedBitrate = userSelectedBitrate * 2;
                                            break;
                                        case 7:
                                            mixdown1 = "5point1";
                                            userSelectedBitrate = userSelectedBitrate * 6;
                                            break;
                                        case 8:
                                            mixdown1 = "5point1";
                                            userSelectedBitrate = userSelectedBitrate * 6;
                                            break;
                                        default:
                                            mixdown1 = "5point1";
                                            userSelectedBitrate = userSelectedBitrate * 6;
                                            break;
                                    }
                                    break;
                            }
                            break;
                    }

                    if (!disableCheckStream2.Checked) //Enabled
                    {
                        switch (audioCodecComboBox2.Text)
                        {
                            case "Filtered Passthru":
                                switch (videoFile.Audio[audioTrackNumber].Channels)
                                {
                                    case 1:
                                        mixdown2 = ",mono";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 1;
                                        break;
                                    case 2:
                                        mixdown2 = ",dpl2";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                        break;
                                    case 3:
                                        mixdown2 = ",dpl2";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                        break;
                                    case 4:
                                        mixdown2 = ",dpl2";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                        break;
                                    case 5:
                                        mixdown2 = ",dpl2";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                        break;
                                    case 6:
                                        mixdown2 = ",5point1";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 6;
                                        break;
                                    case 7:
                                        mixdown2 = ",5point1";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 6;
                                        break;
                                    case 8:
                                        mixdown2 = ",5point1";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 6;
                                        break;
                                    default:
                                        mixdown2 = ",dpl2";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                        break;
                                }
                                break;
                            case "AAC (FDK)": //only good up to Dolby Pro Logic 2 (2 channel)
                                switch (videoFile.Audio[audioTrackNumber].Channels)
                                {
                                    case 1:
                                        mixdown2 = ",mono";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 1; ;
                                        break;
                                    default:
                                        mixdown2 = ",dpl2";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                        break;
                                }
                                break;
                            default:
                                switch (mixdownComboBox.Text)
                                {
                                    case "Dolby ProLogic 2":
                                        mixdown2 = ",dpl2";
                                        userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                        break;
                                    default: //5.1 Audio
                                        switch (videoFile.Audio[audioTrackNumber].Channels)
                                        {
                                            case 1:
                                                mixdown2 = ",mono";
                                                userSelectedBitrate2 = userSelectedBitrate2 * 1;
                                                break;
                                            case 2:
                                                mixdown2 = ",dpl2";
                                                userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                                break;
                                            case 3:
                                                mixdown2 = ",dpl2";
                                                userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                                break;
                                            case 4:
                                                mixdown2 = ",dpl2";
                                                userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                                break;
                                            case 5:
                                                mixdown2 = ",dpl2";
                                                userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                                break;
                                            case 6:
                                                mixdown2 = ",5point1";
                                                userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                                break;
                                            case 7:
                                                mixdown2 = ",5point1";
                                                userSelectedBitrate2 = userSelectedBitrate2 * 6;
                                                break;
                                            case 8:
                                                mixdown2 = ",5point1";
                                                userSelectedBitrate2 = userSelectedBitrate2 * 6;
                                                break;
                                            default:
                                                mixdown2 = ",5point1";
                                                userSelectedBitrate2 = userSelectedBitrate2 * 6;
                                                break;
                                        }
                                        break;
                                }
                                break;
                        }

                        if (!disableCheckStream3.Checked) //Enabled
                        {
                            switch (audioCodecComboBox3.Text)
                            {
                                case "Filtered Passthru":
                                    switch (videoFile.Audio[audioTrackNumber].Channels)
                                    {
                                        case 1:
                                            mixdown3 = ",mono";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 1;
                                            break;
                                        case 2:
                                            mixdown3 = ",dpl2";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                            break;
                                        case 3:
                                            mixdown3 = ",dpl2";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                            break;
                                        case 4:
                                            mixdown3 = ",dpl2";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                            break;
                                        case 5:
                                            mixdown3 = ",dpl2";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                            break;
                                        case 6:
                                            mixdown3 = ",5point1";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 6;
                                            break;
                                        case 7:
                                            mixdown3 = ",5point1";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 6;
                                            break;
                                        case 8:
                                            mixdown3 = ",5point1";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 6;
                                            break;
                                        default:
                                            mixdown3 = ",dpl2";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                            break;
                                    }
                                    break;
                                case "AAC (FDK)": //only good up to Dolby Pro Logic 2 (2 channel)
                                    switch (videoFile.Audio[audioTrackNumber].Channels)
                                    {
                                        case 1:
                                            mixdown3 = ",mono";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 1; ;
                                            break;
                                        default:
                                            mixdown3 = ",dpl2";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                            break;
                                    }
                                    break;
                                default:
                                    switch (mixdownComboBox.Text)
                                    {
                                        case "Dolby ProLogic 2":
                                            mixdown3 = ",dpl2";
                                            userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                            break;
                                        default: //5.1 Audio
                                            switch (videoFile.Audio[audioTrackNumber].Channels)
                                            {
                                                case 1:
                                                    mixdown3 = ",mono";
                                                    userSelectedBitrate3 = userSelectedBitrate3 * 1;
                                                    break;
                                                case 2:
                                                    mixdown3 = ",dpl2";
                                                    userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                                    break;
                                                case 3:
                                                    mixdown3 = ",dpl2";
                                                    userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                                    break;
                                                case 4:
                                                    mixdown3 = ",dpl2";
                                                    userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                                    break;
                                                case 5:
                                                    mixdown3 = ",dpl2";
                                                    userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                                    break;
                                                case 6:
                                                    mixdown3 = ",5point1";
                                                    userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                                    break;
                                                case 7:
                                                    mixdown3 = ",5point1";
                                                    userSelectedBitrate3 = userSelectedBitrate3 * 6;
                                                    break;
                                                case 8:
                                                    mixdown3 = ",5point1";
                                                    userSelectedBitrate3 = userSelectedBitrate3 * 6;
                                                    break;
                                                default:
                                                    mixdown3 = ",5point1";
                                                    userSelectedBitrate3 = userSelectedBitrate3 * 6;
                                                    break;
                                            }
                                            break;
                                    }
                                    break;
                            }
                        }
                    }
                }

                

                outputMixdown = "--mixdown " + mixdown1 + mixdown2 + mixdown3 + " ";

                if(!disableCheckStream2.Checked) //Stream 2 Enabled
                {
                    if(!disableCheckStream3.Checked)
                    {
                        outputBitrate = "--ab " + userSelectedBitrate.ToString() + "," + userSelectedBitrate2.ToString() + "," + userSelectedBitrate3.ToString() + " ";
                    }
                    else
                    {
                        outputBitrate = "--ab " + userSelectedBitrate.ToString() + "," + userSelectedBitrate2.ToString() +  " ";
                    }
                }
                else //Stream 2 Disabled
                {
                    outputBitrate = "--ab " + userSelectedBitrate.ToString() + " ";
                }
                
            }
            else //Source Unreadable - Set variables derived from file to user selected values
            {
                if(!disableCheckStream2.Checked) //Stream 2 enabled
                {
                    if(!disableCheckStream3.Checked) //Stream 3 enabled
                    {
                        outputAudioTrack = "--audio 1,1,1 ";//Since no track can be read in, select the first audio track.
                        try { userSelectedBitrate = int.Parse(audioBitrateCombo.Text); } catch { userSelectedBitrate = 96; }//default value is 96
                        try { userSelectedBitrate2 = int.Parse(audioBitrateCombo2.Text); } catch { userSelectedBitrate2 = 96; }//default value is 96
                        try { userSelectedBitrate3 = int.Parse(audioBitrateCombo3.Text); } catch { userSelectedBitrate3 = 96; }//default value is 96

                        maxBitrate = userSelectedBitrate;
                        outputSampleRate = "--arate " + sampleRateCombo.Text + "," + sampleRateCombo2 + "," + sampleRateCombo3 + " ";

                        if (audioCodecComboBox.Text =="Filtered Passthru" || audioCodecComboBox2.Text == "Filtered Passthru" || audioCodecComboBox3.Text == "Filtered Passthru")
                        {
                            if (!disableCheckStream2.Checked) //2nd Stream Enabled
                            {
                                outputFallBack = "--audio-fallback eac3 ";
                            }
                            else //First stream should always be aac as it's universal.
                            {
                                outputFallBack = "--audio-fallback fdk_aac ";
                            }

                        }
                    }
                    else //Stream 3 disabled
                    {
                        outputAudioTrack = "--audio 1,1 ";//Since no track can be read in, select the first audio track.
                        try { userSelectedBitrate = int.Parse(audioBitrateCombo.Text); } catch { userSelectedBitrate = 96; }//default value is 96
                        try { userSelectedBitrate2 = int.Parse(audioBitrateCombo2.Text); } catch { userSelectedBitrate2 = 96; }//default value is 96
                        try { userSelectedBitrate3 = int.Parse(audioBitrateCombo3.Text); } catch { userSelectedBitrate3 = 96; }//default value is 96

                        maxBitrate = userSelectedBitrate;
                        outputSampleRate = "--arate " + sampleRateCombo.Text + "," + sampleRateCombo2 + " ";

                        if (audioCodecComboBox.Text == "Filtered Passthru" || audioCodecComboBox2.Text == "Filtered Passthru" || audioCodecComboBox3.Text == "Filtered Passthru")
                        {
                            if (!disableCheckStream2.Checked) //2nd Stream Enabled
                            {
                                outputFallBack = "--audio-fallback eac3 ";
                            }
                            else //First stream should always be aac as it's universal.
                            {
                                outputFallBack = "--audio-fallback fdk_aac ";
                            }

                        }
                    }
                }
                else
                {
                    //bitrateOfFile, audioTrackNumber, maxBitrate
                    outputAudioTrack = "--audio 1 "; //Since no track can be read in, select the first audio track.
                    try { userSelectedBitrate = int.Parse(audioBitrateCombo.Text); } catch { userSelectedBitrate = 96; }//default value is 96
                    maxBitrate = userSelectedBitrate;

                    /*Samplerate***********************************************************************************************************************************************************************************************/
                    outputSampleRate = "--arate " + sampleRateCombo.Text + " ";

                    /*Fallback***********************************************************************************************************************************************************************************************/
                    //outputFallBack = "--audio-fallback av_aac "; - Use the FDK encoder it's much better, requires handbrake to be compiled separateley.
                    outputFallBack = "--audio-fallback fdk_aac ";
                }

                /*Mixdown***********************************************************************************************************************************************************************************************/
                //The options are auto, mono, stereo, dpl1 (Dolby Surround), dpl2 (Dolby ProLogic? 2), or 5point1 (5.1).
                switch (audioCodecComboBox.Text)
                {
                    case "Filtered Passthru":
                        mixdown1 = mixdownComboBox.Text;
                        userSelectedBitrate = userSelectedBitrate * 6;
                        break;
                    case "AAC (FDK)": //only good up to Dolby Pro Logic 2 (2 channel)
                        mixdown1 = "dpl2";
                        userSelectedBitrate = userSelectedBitrate * 2;
                        break;
                    default:
                        switch (mixdownComboBox.Text)
                        {
                            case "Dolby ProLogic 2":
                                mixdown1 = "dpl2";
                                userSelectedBitrate = userSelectedBitrate * 2;
                                break;
                            default: //5.1 Audio
                                mixdown1 = "5point1";
                                userSelectedBitrate = userSelectedBitrate * 6;
                                break;
                        }
                        break;
                }
                if (!disableCheckStream2.Checked)
                {
                    switch (audioCodecComboBox2.Text)
                    {
                        case "Filtered Passthru":
                            mixdown2 = mixdownComboBox2.Text;
                            userSelectedBitrate2 = userSelectedBitrate2 * 6;
                            break;
                        case "AAC (FDK)": //only good up to Dolby Pro Logic 2 (2 channel)
                            mixdown2 = ",dpl2";
                            userSelectedBitrate2 = userSelectedBitrate2 * 2;
                            break;
                        default:
                            switch (mixdownComboBox2.Text)
                            {
                                case "Dolby ProLogic 2":
                                    mixdown2 = ",dpl2";
                                    userSelectedBitrate2 = userSelectedBitrate2 * 2;
                                    break;
                                default: //5.1 Audio
                                    mixdown2 = ",5point1";
                                    userSelectedBitrate2 = userSelectedBitrate2 * 6;
                                    break;
                            }
                            break;
                    }
                    if (!disableCheckStream3.Checked) //Enabled
                    {
                        switch (audioCodecComboBox3.Text)
                        {
                            case "Filtered Passthru":
                                mixdown3 = mixdownComboBox3.Text;
                                userSelectedBitrate3 = userSelectedBitrate3 * 6;
                                break;
                            case "AAC (FDK)": //only good up to Dolby Pro Logic 2 (2 channel)
                                mixdown3 = "--mixdown dpl2 ";
                                userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                break;
                            default:
                                switch (mixdownComboBox3.Text)
                                {
                                    case "Dolby ProLogic 3":
                                        mixdown3 = "--mixdown dpl2 ";
                                        userSelectedBitrate3 = userSelectedBitrate3 * 2;
                                        break;
                                    default: //5.1 Audio
                                        mixdown3 = "--mixdown 5point1 ";
                                        userSelectedBitrate3 = userSelectedBitrate3 * 6;
                                        break;
                                }
                                break;
                        }
                    }
                }



                outputMixdown = "--mixdown " + mixdown1 + mixdown2 + mixdown3 + " ";

                if (!disableCheckStream2.Checked) //Stream 2 Enabled
                {
                    if (!disableCheckStream3.Checked)
                    {
                        outputBitrate = "--ab " + userSelectedBitrate.ToString() + "," + userSelectedBitrate2.ToString() + "," + userSelectedBitrate3.ToString() + " ";
                    }
                    else
                    {
                        outputBitrate = "--ab " + userSelectedBitrate.ToString() + "," + userSelectedBitrate2.ToString() + " ";
                    }
                }
                else //Stream 2 Disabled
                {
                    outputBitrate = "--ab " + userSelectedBitrate.ToString() + " ";
                }
            }
            
            /*Passthru Mask***********************************************************************************************************************************************************************************************/

            if(audioCodecComboBox.Text =="Filtered Passthru" || audioCodecComboBox2.Text == "Filtered Passthru" || audioCodecComboBox3.Text == "Filtered Passthru")
            {
                //Check which passthru options are selected

                if (filteredAACCheck.Checked || filteredAACCheck2.Checked || filteredAACCheck3.Checked)
                {
                    if (string.IsNullOrEmpty(outputAudioPassthruMask))
                    {
                        outputAudioPassthruMask += "aac";
                    }
                    else
                    {
                        outputAudioPassthruMask += ",aac";
                    }
                    
                }
                if (filteredAC3Check.Checked || filteredAC3Check2.Checked || filteredAC3Check3.Checked)
                {
                    if (string.IsNullOrEmpty(outputAudioPassthruMask))
                    {
                        outputAudioPassthruMask += "ac3";
                    }
                    else
                    {
                        outputAudioPassthruMask += ",ac3";
                    }
                }
                if (filteredEAC3Check.Checked || filteredEAC3Check2.Checked || filteredEAC3Check3.Checked)
                {
                    if (string.IsNullOrEmpty(outputAudioPassthruMask))
                    {
                        outputAudioPassthruMask += "eac3";
                    }
                    else
                    {
                        outputAudioPassthruMask += ",eac3";
                    }
                }
                if (filteredDTSCheck.Checked || filteredDTSCheck2.Checked || filteredDTSCheck3.Checked)
                {
                    if (string.IsNullOrEmpty(outputAudioPassthruMask))
                    {
                        outputAudioPassthruMask += "dts";
                    }
                    else
                    {
                        outputAudioPassthruMask += ",dts";
                    }
                }
                if (filteredDTSHDCheck.Checked || filteredDTSHDCheck2.Checked || filteredDTSHDCheck3.Checked)
                {
                    if (string.IsNullOrEmpty(outputAudioPassthruMask))
                    {
                        outputAudioPassthruMask += "dtshd";
                    }
                    else
                    {
                        outputAudioPassthruMask += ",dtshd";
                    }
                }
                if (filteredTrueHDCheck.Checked || filteredTrueHDCheck2.Checked || filteredTrueHDCheck3.Checked)
                {
                    if (string.IsNullOrEmpty(outputAudioPassthruMask))
                    {
                        outputAudioPassthruMask += "truehd";
                    }
                    else
                    {
                        outputAudioPassthruMask += ",truehd";
                    }
                }
                if (filteredMP3Check.Checked || filteredMP3Check2.Checked || filteredMP3Check3.Checked)
                {
                    if (string.IsNullOrEmpty(outputAudioPassthruMask))
                    {
                        outputAudioPassthruMask += "mp3";
                    }
                    else
                    {
                        outputAudioPassthruMask += ",mp3";
                    }
                }
                if (filteredFLACCheck.Checked || filteredFLACCheck2.Checked || filteredFLACCheck3.Checked)
                {
                    if (string.IsNullOrEmpty(outputAudioPassthruMask))
                    {
                        outputAudioPassthruMask += "flac16,flac24";
                    }
                    else
                    {
                        outputAudioPassthruMask += ",flac16,flac24";
                    }
                }
                if(!string.IsNullOrEmpty(outputAudioPassthruMask))
                {
                    outputAudioPassthruMask = "--audio-copy-mask " + outputAudioPassthruMask + " ";
                }
                
            }
            else
            {
                outputAudioPassthruMask = "";
            }

            /*Encoder***********************************************************************************************************************************************************************************************/
            string encoder1 = "";
            string encoder2 = "";
            string encoder3 = "";

            switch (audioCodecComboBox.Text)
            {
                case "AAC (FDK)":
                    //outputEncoder = "--audio-fallback av_aac "; - Use the FDK encoder it's much better, requires handbrake to be compiled separateley.
                    encoder1 = "fdk_aac";
                    break;
                case "AC3":
                    encoder1 = "ac3";
                    break;
                case "E-AC3":
                    encoder1 = "eac3";
                    break;
                case "Filtered Passthru":
                    encoder1 = "copy";
                    break;
                default:
                    //outputEncoder = "--audio-fallback av_aac "; - Use the FDK encoder it's much better, requires handbrake to be compiled separateley.
                    encoder1 = "fdk_aac";
                    break;
            }
            if(!disableCheckStream2.Checked) //Enabled
            {
                switch (audioCodecComboBox2.Text)
                {
                    case "AAC (FDK)":
                        //outputEncoder = "--audio-fallback av_aac "; - Use the FDK encoder it's much better, requires handbrake to be compiled separateley.
                        encoder2 = ",fdk_aac";
                        break;
                    case "AC3":
                        encoder2 = ",AC3";
                        break;
                    case "E-AC3":
                        encoder2 = ",eac3";
                        break;
                    case "Filtered Passthru":
                        encoder2 = ",copy";
                        break;
                    default:
                        //outputEncoder = "--audio-fallback av_aac "; - Use the FDK encoder it's much better, requires handbrake to be compiled separateley.
                        encoder2 = ",fdk_aac";
                        break;
                }
                if(!disableCheckStream3.Checked)
                {
                    switch (audioCodecComboBox3.Text)
                    {
                        case "AAC (FDK)":
                            //outputEncoder = "--audio-fallback av_aac "; - Use the FDK encoder it's much better, requires handbrake to be compiled separateley.
                            encoder3 = ",fdk_aac";
                            break;
                        case "AC3":
                            encoder3 = ",ac3";
                            break;
                        case "E-AC3":
                            encoder3 = ",eac3";
                            break;
                        case "Filtered Passthru":
                            encoder3 = ",copy";
                            break;
                        default:
                            //outputEncoder = "--audio-fallback av_aac "; - Use the FDK encoder it's much better, requires handbrake to be compiled separateley.
                            encoder3 = ",fdk_aac";
                            break;
                    }
                }
            }

            outputEncoder = "--aencoder " + encoder1 + encoder2 + encoder3 + " ";

            if(!disableCheckStream2.Checked) //Enabled
            {
                outputDynamicRange = "--drc 0,0 --gain 0,0 ";
                if(!disableCheckStream3.Checked) //Enabled
                {
                    outputDynamicRange = "--drc 0,0,0 --gain 0,0,0 ";
                }
            }

            return outputAudioTrack + outputEncoder + outputAudioPassthruMask + outputFallBack + outputBitrate + outputSampleRate + outputMixdown + outputDynamicRange;
        } //Researching 2 track audio playability on Roku and Xbox.
        private string SourceDestinationOptionsString(string filepath, string filename, string outputPath, bool outputLargerThan4Gb)
        {
            string inputFileExt = "";
            char delim = '.';
            string[] Tokens = filename.Split(delim);

            string outputFile = "";
            string outputFileExt = ".mp4";
            string containerFormat = "--format mp4 ";
            string chapterMarkers = "--markers "; //Add chapter markers
            string webOptimization = "";
            /*Input File***********************************************************************************************************************************************************************************************/
            if (optimizeStreamingCheckBox.Checked) { webOptimization = "--optimize "; } //Optimize mp4 files for HTTP streaming ("fast start")

            /*Input File***********************************************************************************************************************************************************************************************/
            string inputFile = "--input \"" + filepath + "\" --title 1 --angle 1 "; //Input file location
            inputFileExt = "." + Tokens[Tokens.Count() - 1]; //should be extension


            /*Output File***********************************************************************************************************************************************************************************************/

            //Check if output filename already exists, if so add a number to the tail end of the file name

            if (!System.IO.File.Exists(outputPath + "\\" + filename.Replace(inputFileExt, outputFileExt)))
            {
                //outputFile = "--output \"" + outputPath + "\\" + filename.Replace(inputFileExt, outputFileExt) + "\" ";
                outputFile = "--output \"" + outputPath + "\\" + filename.Replace(inputFileExt, outputFileExt) + "\" " + containerFormat + chapterMarkers + webOptimization;
            }
            else
            {
                string newName = outputPath + "\\" + filename.Replace(inputFileExt, outputFileExt);
                int counter = 0;
                while (System.IO.File.Exists(newName))
                {
                    counter++;
                    newName = outputPath + "\\" + filename.Replace(inputFileExt, "") + "-" + counter.ToString() + outputFileExt;
                }

                outputFile = "--output \"" + newName + "\" " + containerFormat + chapterMarkers + webOptimization; //Location to output converted file
            }

            return inputFile + outputFile;
        }



        /*The following methods are to ensure user input is valid*/
        //Video Comboboxes Index Change
        private void AvgBitrateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["VideoBitrateCap"] = avgBitrateCombo.Text;
        }
        private void FramerateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["Framerate"] = framerateCombo.Text;

            switch (framerateCombo.Text)
            {
                case "Same As Source":
                    NLabelUpdate("Framerates > 30 are not ROKU Compliant!", Color.Red);
                    notificationLabel.Visible = true;
                    break;
                case "50":
                    NLabelUpdate("Framerates > 30 are not ROKU Compliant!", Color.Red);
                    notificationLabel.Visible = true;
                    break;
                case "59.94":
                    NLabelUpdate("Framerates > 30 are not ROKU Compliant!", Color.Red);
                    notificationLabel.Visible = true;
                    break;
                case "60":
                    NLabelUpdate("Framerates > 30 are not ROKU Compliant!", Color.Red);
                    notificationLabel.Visible = true;
                    break;
                default:
                    notificationLabel.Visible = true;
                    break;
            }
        }
        private void FrameRateModeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["FramerateMode"] = frameRateModeCombo.Text;
        }
        private void EncoderLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["EncoderLevel"] = encoderLevelComboBox.Text;
        }
        private void EncoderProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["EncoderProfile"] = encoderProfileComboBox.Text;
        }
        private void EncoderSpeedCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["EncoderSpeed"] = encoderSpeedCombo.Text;
        }  
        private void EncoderTuneComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["EncoderTune"] = encoderTuneComboBox.Text;
        }
        private void SubtitleCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["SubtitleSelection"] = subtitleCombo.Text;
        }


        //Video Checkboxes
        private void OptimizeStreamingCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (optimizeStreamingCheckBox.Checked)
            {
                //Update default in dictionary
                CF.DefaultSettings["Optimize"] = "True";
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["Optimize"] = "False";
            }

        }
        private void autoCropCB_CheckedChanged_1(object sender, EventArgs e)
        {
            if (autoCropCB.Checked)
            {
                //Update default in dictionary
                CF.DefaultSettings["AutoCrop"] = "True";
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["AutoCrop"] = "False";
            }
        }
        private void TurboCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (turboCheckBox.Checked)
            {
                //Update default in dictionary
                CF.DefaultSettings["TurboFirstPass"] = "True";
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["TurboFirstPass"] = "False";
            }
        }
        private void TwoPassCheckbox_CheckStateChanged(object sender, EventArgs e)
        {
            if (twoPassCheckbox.Checked)
            {
                //Update default in dictionary
                CF.DefaultSettings["TwoPass"] = "True";
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["TwoPass"] = "False";
            }
        }
        private void BurnInSubtitlesCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (burnInSubtitlesCheck.Checked)
            {
                //Update default in dictionary
                CF.DefaultSettings["ForcedSubtitleBurnIn"] = "True";
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["ForcedSubtitleBurnIn"] = "False";
            }
        }


        //Video ComboBoxes Leave
        private void AvgBitrateCombo_Leave(object sender, EventArgs e)
        {
            if (!videoBitrateCapList.Contains(avgBitrateCombo.Text))
            {
                avgBitrateCombo.Text = videoBitrateCapList[0];

                //Update default in dictionary
                CF.DefaultSettings["VideoBitrateCap"] = videoBitrateCapList[0];
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["VideoBitrateCap"] = avgBitrateCombo.Text;
            }
        }
        private void FramerateCombo_Leave(object sender, EventArgs e)
        {
            if (!framerateList.Contains(framerateCombo.Text))
            {
                framerateCombo.Text = framerateList[0];

                //Update Default in Dictionary
                CF.DefaultSettings["Framerate"] = framerateList[0];
            }
            else
            {
                CF.DefaultSettings["Framerate"] = framerateCombo.Text;
            }
        }
        private void FrameRateModeCombo_Leave(object sender, EventArgs e)
        {
            if(!frameRateModeList.Contains(frameRateModeCombo.Text))
            {
                frameRateModeCombo.Text = frameRateModeList[0];

                //Update default in dictionary
                CF.DefaultSettings["FramerateMode"] = frameRateModeList[0];
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["FramerateMode"] = frameRateModeCombo.Text;
            }
        }
        private void EncoderLevelComboBox_Leave(object sender, EventArgs e)
        {
            if (!encoderLevelList.Contains(encoderLevelComboBox.Text))
            {
                encoderLevelComboBox.Text = encoderLevelList[0];

                //Update Dictionary default
                CF.DefaultSettings["EncoderLevel"] = encoderLevelList[0];
            }
            else
            {
                //Update Dictionary default
                CF.DefaultSettings["EncoderLevel"] = encoderLevelComboBox.Text;
            }
        }
        private void EncoderProfileComboBox_Leave(object sender, EventArgs e)
        {
            if (!encoderProfileList.Contains(encoderProfileComboBox.Text))
            {
                encoderProfileComboBox.Text = encoderProfileList[0];

                //Update Default in Dictionary
                CF.DefaultSettings["EncoderProfile"] = encoderProfileList[0];
            }
            else
            {
                //Update Default in Dictionary
                CF.DefaultSettings["EncoderProfile"] = encoderProfileComboBox.Text;
            }
        }
        private void EncoderTuneComboBox_Leave(object sender, EventArgs e)
        {
            if (!encoderTuneList.Contains(encoderTuneComboBox.Text))
            {
                encoderTuneComboBox.Text = encoderTuneList[0];

                //Update default in dictionary
                CF.DefaultSettings["EncoderTune"] = encoderTuneList[0];
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["EncoderTune"] = encoderTuneComboBox.Text;
            }
        }
        private void EncoderSpeedCombo_Leave(object sender, EventArgs e)
        {
            //Verify text is in list
            if (!encoderSpeedList.Contains(encoderSpeedCombo.Text))
            {
                encoderSpeedCombo.Text = encoderSpeedList[0];

                //Update default in Dictionary
                CF.DefaultSettings["EncoderSpeed"] = encoderSpeedList[0];
            }
            else
            {
                CF.DefaultSettings["EncoderSpeed"] = encoderSpeedCombo.Text;
            }
        }
        private void SubtitleCombo_Leave_1(object sender, EventArgs e)
        {
            //Verify text is in list
            if (!subtitleComboList.Contains(subtitleCombo.Text))
            {
                subtitleCombo.Text = subtitleComboList[0];

                //Update default in Dictionary
                CF.DefaultSettings["SubtitleSelection"] = subtitleComboList[0];
            }
            else
            {
                CF.DefaultSettings["SubtitleSelection"] = subtitleCombo.Text;
            }
        }


        //Video ComboBoxes Text Changed
        private void AvgBitrateCombo_TextChanged(object sender, EventArgs e)
        {
            if (!videoBitrateCapList.Contains(avgBitrateCombo.Text))
            {
                avgBitrateCombo.Text = videoBitrateCapList[0];

                //Update default in dictionary
                CF.DefaultSettings["VideoBitrateCap"] = videoBitrateCapList[0];
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["VideoBitrateCap"] = avgBitrateCombo.Text;
            }
        }
        private void FramerateCombo_TextChanged(object sender, EventArgs e)
        {
            if (!framerateList.Contains(framerateCombo.Text))
            {
                framerateCombo.Text = framerateList[0];

                //Update Default in Dictionary
                CF.DefaultSettings["Framerate"] = framerateList[0];
            }
            else
            {
                CF.DefaultSettings["Framerate"] = framerateCombo.Text;
            }
        }
        private void FrameRateModeCombo_TextChanged(object sender, EventArgs e)
        {
            if (!frameRateModeList.Contains(frameRateModeCombo.Text))
            {
                frameRateModeCombo.Text = frameRateModeList[0];

                //Update default in dictionary
                CF.DefaultSettings["FramerateMode"] = frameRateModeList[0];
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["FramerateMode"] = frameRateModeCombo.Text;
            }
        }
        private void EncoderLevelComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!encoderLevelList.Contains(encoderLevelComboBox.Text))
            {
                encoderLevelComboBox.Text = encoderLevelList[0];

                //Update Dictionary default
                CF.DefaultSettings["EncoderLevel"] = encoderLevelList[0];
            }
            else
            {
                //Update Dictionary default
                CF.DefaultSettings["EncoderLevel"] = encoderLevelComboBox.Text;
            }
        }
        private void EncoderProfileComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!encoderProfileList.Contains(encoderProfileComboBox.Text))
            {
                encoderProfileComboBox.Text = encoderProfileList[0];

                //Update Default in Dictionary
                CF.DefaultSettings["EncoderProfile"] = encoderProfileList[0];
            }
            else
            {
                //Update Default in Dictionary
                CF.DefaultSettings["EncoderProfile"] = encoderProfileComboBox.Text;
            }
        }
        private void EncoderTuneComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!encoderTuneList.Contains(encoderTuneComboBox.Text))
            {
                encoderTuneComboBox.Text = encoderTuneList[0];

                //Update default in dictionary
                CF.DefaultSettings["EncoderTune"] = encoderTuneList[0];
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["EncoderTune"] = encoderTuneComboBox.Text;
            }
        }
        private void EncoderSpeedCombo_TextUpdate(object sender, EventArgs e)
        {
            //Verify text is in list
            if (!encoderSpeedList.Contains(encoderSpeedCombo.Text))
            {
                encoderSpeedCombo.Text = encoderSpeedList[0];

                //Update default in Dictionary
                CF.DefaultSettings["EncoderSpeed"] = encoderSpeedList[0];
            }
            else
            {
                CF.DefaultSettings["EncoderSpeed"] = encoderSpeedCombo.Text;
            }
        }
        private void SubtitleCombo_TextChanged(object sender, EventArgs e)
        {
            //Verify text is in list
            if (!subtitleComboList.Contains(subtitleCombo.Text))
            {
                subtitleCombo.Text = subtitleComboList[0];

                //Update default in Dictionary
                CF.DefaultSettings["SubtitleSelection"] = subtitleComboList[0];
            }
            else
            {
                CF.DefaultSettings["SubtitleSelection"] = subtitleCombo.Text;
            }
        }


        //Audio
        //Audio combobox index change
        private void AudioCodecComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set default value in dictionary.
            CF.DefaultSettings["AudioCodec"] = audioCodecComboBox.Text;

            //Only show filter options if Filtered Passthru is selected.
            switch (audioCodecComboBox.Text)
            {
                case "Filtered Passthru":
                    filteredAACCheck.Visible = true;
                    filteredAC3Check.Visible = true;
                    filteredEAC3Check.Visible = true;
                    filteredDTSCheck.Visible = true;
                    filteredDTSHDCheck.Visible = true;
                    filteredTrueHDCheck.Visible = true;
                    filteredMP3Check.Visible = true;
                    filteredFLACCheck.Visible = true;
                    passthruFilterLabel.Visible = true;
                    break;
                case "AAC (FDK)":
                    filteredAACCheck.Visible = false;
                    filteredAC3Check.Visible = false;
                    filteredEAC3Check.Visible = false;
                    filteredDTSCheck.Visible = false;
                    filteredDTSHDCheck.Visible = false;
                    filteredTrueHDCheck.Visible = false;
                    filteredMP3Check.Visible = false;
                    filteredFLACCheck.Visible = false;
                    passthruFilterLabel.Visible = false;
                    mixdownComboBox.Text = "Dolby ProLogic 2"; //AAC can only mix down to Prologic or Mono
                    break;
                default:
                    filteredAACCheck.Visible = false;
                    filteredAC3Check.Visible = false;
                    filteredEAC3Check.Visible = false;
                    filteredDTSCheck.Visible = false;
                    filteredDTSHDCheck.Visible = false;
                    filteredTrueHDCheck.Visible = false;
                    filteredMP3Check.Visible = false;
                    filteredFLACCheck.Visible = false;
                    passthruFilterLabel.Visible = false;
                    break;
            }
        }
        private void audioCodecComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set default value in dictionary.
            CF.DefaultSettings["AudioCodec2"] = audioCodecComboBox2.Text;

            //Only show filter options if Filtered Passthru is selected.
            switch (audioCodecComboBox2.Text)
            {
                case "Filtered Passthru":
                    filteredAACCheck2.Visible = true;
                    filteredAC3Check2.Visible = true;
                    filteredEAC3Check2.Visible = true;
                    filteredDTSCheck2.Visible = true;
                    filteredDTSHDCheck2.Visible = true;
                    filteredTrueHDCheck2.Visible = true;
                    filteredMP3Check2.Visible = true;
                    filteredFLACCheck2.Visible = true;
                    passthruFilterLabel2.Visible = true;
                    break;
                case "AAC (FDK)":
                    filteredAACCheck2.Visible = false;
                    filteredAC3Check2.Visible = false;
                    filteredEAC3Check2.Visible = false;
                    filteredDTSCheck2.Visible = false;
                    filteredDTSHDCheck2.Visible = false;
                    filteredTrueHDCheck2.Visible = false;
                    filteredMP3Check2.Visible = false;
                    filteredFLACCheck2.Visible = false;
                    passthruFilterLabel2.Visible = false;
                    mixdownComboBox2.Text = "Dolby ProLogic 2"; //AAC can only mix down to Prologic or Mono
                    break;
                default:
                    filteredAACCheck2.Visible = false;
                    filteredAC3Check2.Visible = false;
                    filteredEAC3Check2.Visible = false;
                    filteredDTSCheck2.Visible = false;
                    filteredDTSHDCheck2.Visible = false;
                    filteredTrueHDCheck2.Visible = false;
                    filteredMP3Check2.Visible = false;
                    filteredFLACCheck2.Visible = false;
                    passthruFilterLabel2.Visible = false;
                    break;
            }
        }
        private void audioCodecComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set default value in dictionary.
            CF.DefaultSettings["AudioCodec3"] = audioCodecComboBox3.Text;

            //Only show filter options if Filtered Passthru is selected.
            switch (audioCodecComboBox3.Text)
            {
                case "Filtered Passthru":
                    filteredAACCheck3.Visible = true;
                    filteredAC3Check3.Visible = true;
                    filteredEAC3Check3.Visible = true;
                    filteredDTSCheck3.Visible = true;
                    filteredDTSHDCheck3.Visible = true;
                    filteredTrueHDCheck3.Visible = true;
                    filteredMP3Check3.Visible = true;
                    filteredFLACCheck3.Visible = true;
                    passthruFilterLabel3.Visible = true;
                    break;
                case "AAC (FDK)":
                    filteredAACCheck3.Visible = false;
                    filteredAC3Check3.Visible = false;
                    filteredEAC3Check3.Visible = false;
                    filteredDTSCheck3.Visible = false;
                    filteredDTSHDCheck3.Visible = false;
                    filteredTrueHDCheck3.Visible = false;
                    filteredMP3Check3.Visible = false;
                    filteredFLACCheck3.Visible = false;
                    passthruFilterLabel3.Visible = false;
                    mixdownComboBox3.Text = "Dolby ProLogic 2"; //AAC can only mix down to Prologic or Mono
                    break;
                default:
                    filteredAACCheck3.Visible = false;
                    filteredAC3Check3.Visible = false;
                    filteredEAC3Check3.Visible = false;
                    filteredDTSCheck3.Visible = false;
                    filteredDTSHDCheck3.Visible = false;
                    filteredTrueHDCheck3.Visible = false;
                    filteredMP3Check3.Visible = false;
                    filteredFLACCheck3.Visible = false;
                    passthruFilterLabel3.Visible = false;
                    break;
            }
        }

        private void MixdownComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set default in dictionary
            CF.DefaultSettings["Mixdown"] = mixdownComboBox.Text;

            if(audioCodecComboBox.Text == "AAC (FDK)" && mixdownComboBox.Text != "Dolby ProLogic 2")
            {
                NLabelUpdate("The AAC (FDK) Codec can only mixdown to \"Dolby ProLogic 2\"", Color.Red);
                mixdownComboBox.Text = "Dolby ProLogic 2";
            } 
        }
        private void MixdownComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set default in dictionary
            CF.DefaultSettings["Mixdown2"] = mixdownComboBox2.Text;

            if (audioCodecComboBox2.Text == "AAC (FDK)" && mixdownComboBox2.Text != "Dolby ProLogic 2")
            {
                NLabelUpdate("The AAC (FDK) Codec can only mixdown to \"Dolby ProLogic 2\"", Color.Red);
                mixdownComboBox2.Text = "Dolby ProLogic 2";
            }
        }
        private void MixdownComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set default in dictionary
            CF.DefaultSettings["Mixdown3"] = mixdownComboBox3.Text;

            if (audioCodecComboBox3.Text == "AAC (FDK)" && mixdownComboBox3.Text != "Dolby ProLogic 2")
            {
                NLabelUpdate("The AAC (FDK) Codec can only mixdown to \"Dolby ProLogic 2\"", Color.Red);
                mixdownComboBox3.Text = "Dolby ProLogic 2";
            }
        }

        private void SampleRateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["AudioSampleRate"] = sampleRateCombo.Text;
        }
        private void SampleRateCombo2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["AudioSampleRate2"] = sampleRateCombo2.Text;
        }
        private void SampleRateCombo3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["AudioSampleRate3"] = sampleRateCombo3.Text;
        }

        private void AudioBitrateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["AudioBitrateCap"] = audioBitrateCombo.Text;
        }
        private void AudioBitrateCombo2_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["AudioBitrateCap2"] = audioBitrateCombo2.Text;
        }
        private void AudioBitrateCombo3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["AudioBitrateCap3"] = audioBitrateCombo3.Text;
        }

        //Audio Combobox Text Changed
        private void AudioCodecComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!codecList.Contains(audioCodecComboBox.Text))
            {
                audioCodecComboBox.Text = codecList[0];

                //Update default in dictionary
                CF.DefaultSettings["AudioCodec"] = codecList[0];
            }
            else
            {
                CF.DefaultSettings["AudioCodec"] = audioCodecComboBox.Text;
            }
        }
        private void AudioCodecComboBox2_TextChanged(object sender, EventArgs e)
        {
            if (!codecList.Contains(audioCodecComboBox2.Text))
            {
                audioCodecComboBox2.Text = codecList[0];

                //Update default in dictionary
                CF.DefaultSettings["AudioCodec2"] = codecList[0];
            }
            else
            {
                CF.DefaultSettings["AudioCodec2"] = audioCodecComboBox2.Text;
            }
        }
        private void AudioCodecComboBox3_TextChanged(object sender, EventArgs e)
        {
            if (!codecList.Contains(audioCodecComboBox3.Text))
            {
                audioCodecComboBox3.Text = codecList[0];

                //Update default in dictionary
                CF.DefaultSettings["AudioCodec3"] = codecList[0];
            }
            else
            {
                CF.DefaultSettings["AudioCodec3"] = audioCodecComboBox.Text;
            }
        }

        private void MixdownComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!mixdownList.Contains(mixdownComboBox.Text))
            {
                mixdownComboBox.Text = mixdownList[0];

                //Update default in dictionary
                CF.DefaultSettings["Mixdown"] = mixdownList[0];
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["Mixdown"] = mixdownComboBox.Text;
            }
        }
        private void MixdownComboBox2_TextChanged(object sender, EventArgs e)
        {
            if (!mixdownList.Contains(mixdownComboBox2.Text))
            {
                mixdownComboBox2.Text = mixdownList[0];

                //Update default in dictionary
                CF.DefaultSettings["Mixdown2"] = mixdownList[0];
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["Mixdown2"] = mixdownComboBox2.Text;
            }
        }
        private void MixdownComboBox3_TextChanged(object sender, EventArgs e)
        {
            if (!mixdownList.Contains(mixdownComboBox3.Text))
            {
                mixdownComboBox3.Text = mixdownList[0];

                //Update default in dictionary
                CF.DefaultSettings["Mixdown3"] = mixdownList[0];
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings["Mixdown3"] = mixdownComboBox3.Text;
            }
        }

        private void AudioBitrateCombo_TextChanged(object sender, EventArgs e)
        {
            double ABitrate;

            try { double.TryParse(audioBitrateCombo.Text, out ABitrate); }
            catch { ABitrate = 192; }

            if (ABitrate > 256) { ABitrate = 256; }
            if (ABitrate < 64) { ABitrate = 64; }

            audioBitrateCombo.Text = ABitrate.ToString();

            CF.DefaultSettings["AudioBitrateCap"] = ABitrate.ToString();
        }
        private void AudioBitrateCombo2_TextChanged(object sender, EventArgs e)
        {
            double ABitrate;

            try { double.TryParse(audioBitrateCombo2.Text, out ABitrate); }
            catch { ABitrate = 192; }

            if (ABitrate > 256) { ABitrate = 256; }
            if (ABitrate < 64) { ABitrate = 64; }

            audioBitrateCombo2.Text = ABitrate.ToString();

            CF.DefaultSettings["AudioBitrateCap2"] = ABitrate.ToString();
        }
        private void AudioBitrateCombo3_TextChanged(object sender, EventArgs e)
        {
            double ABitrate;

            try { double.TryParse(audioBitrateCombo3.Text, out ABitrate); }
            catch { ABitrate = 192; }

            if (ABitrate > 256) { ABitrate = 256; }
            if (ABitrate < 64) { ABitrate = 64; }

            audioBitrateCombo3.Text = ABitrate.ToString();

            CF.DefaultSettings["AudioBitrateCap3"] = ABitrate.ToString();
        }

        private void SampleRateCombo_TextChanged(object sender, EventArgs e)
        {
            double SRate;
            try { double.TryParse(sampleRateCombo.Text, out SRate); }
            catch { SRate = 48; } //Default to 48

            if (SRate > 48) { sampleRateCombo.Text = "48"; }
            else if (SRate < 48 && SRate > 44.1)
            {
                sampleRateCombo.Text = "48";
            }
            else if (SRate <= 44.1)
            {
                sampleRateCombo.Text = "44.1";
            }
            CF.DefaultSettings["AudioSampleRate"] = sampleRateCombo.Text;
        }
        private void SampleRateCombo2_TextChanged(object sender, EventArgs e)
        {
            double SRate;
            try { double.TryParse(sampleRateCombo2.Text, out SRate); }
            catch { SRate = 48; } //Default to 48

            if (SRate > 48) { sampleRateCombo2.Text = "48"; }
            else if (SRate < 48 && SRate > 44.1)
            {
                sampleRateCombo2.Text = "48";
            }
            else if (SRate <= 44.1)
            {
                sampleRateCombo2.Text = "44.1";
            }
            CF.DefaultSettings["AudioSampleRate2"] = sampleRateCombo2.Text;
        }
        private void SampleRateCombo3_TextChanged(object sender, EventArgs e)
        {
            double SRate;
            try { double.TryParse(sampleRateCombo3.Text, out SRate); }
            catch { SRate = 48; } //Default to 48

            if (SRate > 48) { sampleRateCombo3.Text = "48"; }
            else if (SRate < 48 && SRate > 44.1)
            {
                sampleRateCombo3.Text = "48";
            }
            else if (SRate <= 44.1)
            {
                sampleRateCombo3.Text = "44.1";
            }
            CF.DefaultSettings["AudioSampleRate3"] = sampleRateCombo3.Text;
        }

        //Audio combobox leave
        private void AudioCodecComboBox_Leave(object sender, EventArgs e)
        {
            if (!codecList.Contains(audioCodecComboBox.Text))
            {
                audioCodecComboBox.Text = codecList[0];

                //Update default in dictionary
                CF.DefaultSettings["AudioCodec"] = codecList[0];
            }
            else
            {
                CF.DefaultSettings["AudioCodec"] = audioCodecComboBox.Text;
            }
        }
        private void AudioCodecComboBox2_Leave(object sender, EventArgs e)
        {
            if (!codecList.Contains(audioCodecComboBox2.Text))
            {
                audioCodecComboBox2.Text = codecList[0];

                //Update default in dictionary
                CF.DefaultSettings["AudioCodec2"] = codecList[0];
            }
            else
            {
                CF.DefaultSettings["AudioCodec2"] = audioCodecComboBox2.Text;
            }
        }
        private void AudioCodecComboBox3_Leave(object sender, EventArgs e)
        {
            if (!codecList.Contains(audioCodecComboBox3.Text))
            {
                audioCodecComboBox3.Text = codecList[0];

                //Update default in dictionary
                CF.DefaultSettings["AudioCodec3"] = codecList[0];
            }
            else
            {
                CF.DefaultSettings["AudioCodec3"] = audioCodecComboBox3.Text;
            }
        }

        private void MixdownComboBox_Leave(object sender, EventArgs e)
        {
            if(!mixdownList.Contains(mixdownComboBox.Text))
            {
                mixdownComboBox.Text = mixdownList[0];

                CF.DefaultSettings["Mixdown"] = mixdownList[0];
            }
            else
            {
                CF.DefaultSettings["Mixdown"] = mixdownComboBox.Text;
            }
        }
        private void MixdownComboBox2_Leave(object sender, EventArgs e)
        {
            if(!mixdownList.Contains(mixdownComboBox2.Text))
            {
                mixdownComboBox2.Text = mixdownList[0];

                CF.DefaultSettings["Mixdown2"] = mixdownList[0];
            }
            else
            {
                CF.DefaultSettings["Mixdown2"] = mixdownComboBox2.Text;
            }
        }
        private void MixdownComboBox3_Leave(object sender, EventArgs e)
        {
            if (!mixdownList.Contains(mixdownComboBox3.Text))
            {
                mixdownComboBox3.Text = mixdownList[0];

                CF.DefaultSettings["Mixdown3"] = mixdownList[0];
            }
            else
            {
                CF.DefaultSettings["Mixdown3"] = mixdownComboBox3.Text;
            }
        }

        private void AudioBitrateCombo_Leave(object sender, EventArgs e)
        {
            double ABitrate;

            try { double.TryParse(audioBitrateCombo.Text, out ABitrate); }
            catch { ABitrate = 192; }

            if (ABitrate > 256) { ABitrate = 256; }
            if (ABitrate < 64) { ABitrate = 64; }

            audioBitrateCombo.Text = ABitrate.ToString();

            CF.DefaultSettings["AudioBitrateCap"] = ABitrate.ToString();
        }
        private void AudioBitrateCombo2_Leave(object sender, EventArgs e)
        {
            double ABitrate;

            try { double.TryParse(audioBitrateCombo2.Text, out ABitrate); }
            catch { ABitrate = 192; }

            if (ABitrate > 256) { ABitrate = 256; }
            if (ABitrate < 64) { ABitrate = 64; }

            audioBitrateCombo2.Text = ABitrate.ToString();

            CF.DefaultSettings["AudioBitrateCap2"] = ABitrate.ToString();
        }
        private void AudioBitrateCombo3_Leave(object sender, EventArgs e)
        {
            double ABitrate;

            try { double.TryParse(audioBitrateCombo3.Text, out ABitrate); }
            catch { ABitrate = 192; }

            if (ABitrate > 256) { ABitrate = 256; }
            if (ABitrate < 64) { ABitrate = 64; }

            audioBitrateCombo3.Text = ABitrate.ToString();

            CF.DefaultSettings["AudioBitrateCap3"] = ABitrate.ToString();
        }

        private void SampleRateCombo_Leave(object sender, EventArgs e)
        {
            double SRate;
            try { double.TryParse(sampleRateCombo.Text, out SRate); }
            catch { SRate = 48; } //Default to 48

            if (SRate > 48) { sampleRateCombo.Text = "48"; }
            else if(SRate < 48 && SRate > 44.1)
            {
                sampleRateCombo.Text = "48";
            }
            else if (SRate <= 44.1)
            {
                sampleRateCombo.Text = "44.1";
            }
            CF.DefaultSettings["AudioSampleRate"] = sampleRateCombo.Text;
        }
        private void SampleRateCombo2_Leave(object sender, EventArgs e)
        {
            double SRate;
            try { double.TryParse(sampleRateCombo2.Text, out SRate); }
            catch { SRate = 48; } //Default to 48

            if (SRate > 48) { sampleRateCombo2.Text = "48"; }
            else if (SRate < 48 && SRate > 44.1)
            {
                sampleRateCombo2.Text = "48";
            }
            else if (SRate <= 44.1)
            {
                sampleRateCombo2.Text = "44.1";
            }
            CF.DefaultSettings["AudioSampleRate2"] = sampleRateCombo2.Text;
        }
        private void SampleRateCombo3_Leave(object sender, EventArgs e)
        {
            double SRate;
            try { double.TryParse(sampleRateCombo3.Text, out SRate); }
            catch { SRate = 48; } //Default to 48

            if (SRate > 48) { sampleRateCombo3.Text = "48"; }
            else if (SRate < 48 && SRate > 44.1)
            {
                sampleRateCombo3.Text = "48";
            }
            else if (SRate <= 44.1)
            {
                sampleRateCombo3.Text = "44.1";
            }
            CF.DefaultSettings["AudioSampleRate3"] = sampleRateCombo3.Text;
        }

        //Audio Checkboxes
        private void FilteredAACCheck_CheckedChanged(object sender, EventArgs e)
        {
            if(filteredAACCheck.Checked)
            {
                CF.DefaultSettings["AAC_Passthru"] = "True";
            }
            else
            {
                CF.DefaultSettings["AAC_Passthru"] = "False";
            }
            
        }
        private void FilteredAACCheck2_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredAACCheck2.Checked)
            {
                CF.DefaultSettings["AAC_Passthru2"] = "True";
            }
            else
            {
                CF.DefaultSettings["AAC_Passthru2"] = "False";
            }

        }
        private void FilteredAACCheck3_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredAACCheck3.Checked)
            {
                CF.DefaultSettings["AAC_Passthru3"] = "True";
            }
            else
            {
                CF.DefaultSettings["AAC_Passthru3"] = "False";
            }

        }

        private void FilteredAC3Check_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredAC3Check.Checked)
            {
                CF.DefaultSettings["AC3_Passthru"] = "True";
            }
            else
            {
                CF.DefaultSettings["AC3_Passthru"] = "False";
            }
        }
        private void FilteredAC3Check2_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredAC3Check2.Checked)
            {
                CF.DefaultSettings["AC3_Passthru2"] = "True";
            }
            else
            {
                CF.DefaultSettings["AC3_Passthru2"] = "False";
            }
        }
        private void FilteredAC3Check3_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredAC3Check3.Checked)
            {
                CF.DefaultSettings["AC3_Passthru3"] = "True";
            }
            else
            {
                CF.DefaultSettings["AC3_Passthru3"] = "False";
            }
        }

        private void FilteredEAC3Check_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredEAC3Check.Checked)
            {
                CF.DefaultSettings["EAC3_Passthru"] = "True";
            }
            else
            {
                CF.DefaultSettings["EAC3_Passthru"] = "False";
            }
        }
        private void FilteredEAC3Check2_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredEAC3Check2.Checked)
            {
                CF.DefaultSettings["EAC3_Passthru2"] = "True";
            }
            else
            {
                CF.DefaultSettings["EAC3_Passthru2"] = "False";
            }
        }
        private void FilteredEAC3Check3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void FilteredDTSCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredDTSCheck.Checked)
            {
                CF.DefaultSettings["DTS_Passthru"] = "True";
            }
            else
            {
                CF.DefaultSettings["DTS_Passthru"] = "False";
            }
        }
        private void FilteredDTSCheck2_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredDTSCheck2.Checked)
            {
                CF.DefaultSettings["DTS_Passthru2"] = "True";
            }
            else
            {
                CF.DefaultSettings["DTS_Passthru2"] = "False";
            }
        }
        private void FilteredDTSCheck3_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredDTSCheck3.Checked)
            {
                CF.DefaultSettings["DTS_Passthru3"] = "True";
            }
            else
            {
                CF.DefaultSettings["DTS_Passthru3"] = "False";
            }
        }

        private void FilteredDTSHDCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredDTSHDCheck.Checked)
            {
                CF.DefaultSettings["DTSHD_Passthru"] = "True";
            }
            else
            {
                CF.DefaultSettings["DTSHD_Passthru"] = "False";
            }
        }
        private void FilteredDTSHDCheck2_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredDTSHDCheck2.Checked)
            {
                CF.DefaultSettings["DTSHD_Passthru2"] = "True";
            }
            else
            {
                CF.DefaultSettings["DTSHD_Passthru2"] = "False";
            }
        }
        private void FilteredDTSHDCheck3_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredDTSHDCheck3.Checked)
            {
                CF.DefaultSettings["DTSHD_Passthru3"] = "True";
            }
            else
            {
                CF.DefaultSettings["DTSHD_Passthru3"] = "False";
            }
        }

        private void FilteredTrueHDCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredTrueHDCheck.Checked)
            {
                CF.DefaultSettings["TrueHD_Passthru"] = "True";
            }
            else
            {
                CF.DefaultSettings["TrueHD_Passthru"] = "False";
            }
        }
        private void FilteredTrueHDCheck2_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredTrueHDCheck2.Checked)
            {
                CF.DefaultSettings["TrueHD_Passthru2"] = "True";
            }
            else
            {
                CF.DefaultSettings["TrueHD_Passthru2"] = "False";
            }
        }
        private void FilteredTrueHDCheck3_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredTrueHDCheck3.Checked)
            {
                CF.DefaultSettings["TrueHD_Passthru3"] = "True";
            }
            else
            {
                CF.DefaultSettings["TrueHD_Passthru3"] = "False";
            }
        }

        private void FilteredMP3Check_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredMP3Check.Checked)
            {
                CF.DefaultSettings["MP3_Passthru"] = "True";
            }
            else
            {
                CF.DefaultSettings["MP3_Passthru"] = "False";
            }
        }
        private void FilteredMP3Check2_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredMP3Check2.Checked)
            {
                CF.DefaultSettings["MP3_Passthru2"] = "True";
            }
            else
            {
                CF.DefaultSettings["MP3_Passthru2"] = "False";
            }
        }
        private void FilteredMP3Check3_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredMP3Check3.Checked)
            {
                CF.DefaultSettings["MP3_Passthru3"] = "True";
            }
            else
            {
                CF.DefaultSettings["MP3_Passthru3"] = "False";
            }
        }

        private void FilteredFLACCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredFLACCheck.Checked)
            {
                CF.DefaultSettings["FLAC_Passthru"] = "True";
            }
            else
            {
                CF.DefaultSettings["FLAC_Passthru"] = "False";
            }
        }
        private void FilteredFLACCheck2_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredFLACCheck2.Checked)
            {
                CF.DefaultSettings["FLAC_Passthru2"] = "True";
            }
            else
            {
                CF.DefaultSettings["FLAC_Passthru2"] = "False";
            }
        }
        private void FilteredFLACCheck3_CheckedChanged(object sender, EventArgs e)
        {
            if (filteredFLACCheck3.Checked)
            {
                CF.DefaultSettings["FLAC_Passthru3"] = "True";
            }
            else
            {
                CF.DefaultSettings["FLAC_Passthru3"] = "False";
            }
        }


        /*These methods are used to send notifications to the user*/
        private void SendNotification(string userName, string password, string sendTo, string subject, string message)
        {
            //If this doesnt work it's because the gmail account needs to allow less secure apps access to send email
            //https://support.google.com/accounts/answer/6010255?hl=en
            if (notificationCheck.Checked) //Ensures that a notification doesn't try to send unless the user intends it.
            {
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(sendTo))
                {
                    //NLabelUpdate("Attempting to send notification", Color.GreenYellow);

                    if (!userName.ToUpper().Contains("@GMAIL.COM")) { userName += "@gmail.com"; }

                    try
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                        mail.From = new MailAddress(userName);
                        mail.To.Add(sendTo);
                        mail.Subject = subject;

                        //Check if recipient is a cellphone, if so remove the subject
                        if (sendTo.ToUpper().Contains("@VTEXT.COM") ||
                            sendTo.ToUpper().Contains("@MMS.ATT.NET") ||
                            sendTo.ToUpper().Contains("@TXT.ATT.NET") ||
                            sendTo.ToUpper().Contains("@TMOMAIL.NET") ||
                            sendTo.ToUpper().Contains("@SPRINTPCS.COM") ||
                            sendTo.ToUpper().Contains("@PM.SPRINT.COM") ||
                            sendTo.ToUpper().Contains("@VMOBL.COM") ||
                            sendTo.ToUpper().Contains("@MMST5.TRACFONE.COM") ||
                            sendTo.ToUpper().Contains("@MYMETROPCS.COM") ||
                            sendTo.ToUpper().Contains("@MYBOOSTMOBILE.COM") ||
                            sendTo.ToUpper().Contains("@SMS.MYCRICKET.COM") ||
                            sendTo.ToUpper().Contains("@PTEL.COM") ||
                            sendTo.ToUpper().Contains("@TEXT.REPUBLICWIRELESS.COM") ||
                            sendTo.ToUpper().Contains("@TMS.SUNCOM.COM") ||
                            sendTo.ToUpper().Contains("@MESSAGE.TING.COM") ||
                            sendTo.ToUpper().Contains("@EMAIL.USCC.NET") ||
                            sendTo.ToUpper().Contains("@CINGULARME.COM") ||
                            sendTo.ToUpper().Contains("@CSPIRE1.COM"))
                        {
                            mail.Subject = "";
                        }

                        mail.Body = message;

                        SmtpServer.Port = 587;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(userName, password);
                        SmtpServer.EnableSsl = true;

                        SmtpServer.Send(mail);
                        CustomMessageBox.Show("Notification Sent!", 120, 230, "Notification Message");
                        //NLabelUpdate("Notification sent to : " + sendTo, Color.GreenYellow);
                        CF.UpdateDefaults(); //Stores info in config file

                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show("You may have to enable less secure apps access to gmail. See https://support.google.com/accounts/answer/6010255?hl=en" + "/r/n" + ex.ToString(), 600, 300);
                        //NLabelUpdate("Notification failed to send.", Color.Red);
                    }
                }
                else
                {
                    CustomMessageBox.Show("Missing Notifiction Parameters", 600, 300);
                    //NLabelUpdate("Missing Notification Parameters", Color.Red);
                }
            }
        }
        private async void TestNotificationButton_Click(object sender, EventArgs e)
        {
            string username = usernameBox.Text;
            string password = passwordBox.Text;
            string sendTo = sendToBox.Text;

            await Task.Run(() =>
            {
                SendNotification(username, password, sendTo, "Test Notification", "Notification Test from Movie Data Collector");
            });

        }
        private void NotificationCheck_CheckedChanged(object sender, EventArgs e)
        {
            if(notificationCheck.Checked)
            {
                userNameLabel.Visible = true;
                usernameBox.Visible = true;
                passwordLabel.Visible = true;
                passwordBox.Visible = true;
                sendToLabel.Visible = true;
                sendToBox.Visible = true;
                testNotificationButton.Visible = true;
            }
            else
            {
                userNameLabel.Visible = false;
                usernameBox.Visible = false;
                passwordLabel.Visible = false;
                passwordBox.Visible = false;
                sendToLabel.Visible = false;
                sendToBox.Visible = false;
                testNotificationButton.Visible = false;

            }
        }
        private string TimeDifference(DateTime start, DateTime end)
        {
            int hours = 0;
            int minutes = 0;
            int seconds = 0;
            int days = 0;

            string totalTime = "";

            hours = (end - start).Hours;
            minutes = (end - start).Minutes;
            seconds = (end - start).Seconds;
            days = (end - start).Days;


            if (days == 1) { totalTime = days.ToString() + " day"; }
            if (days > 1) { totalTime = days.ToString() + " days"; }

            if (!string.IsNullOrEmpty(totalTime) && hours == 1) { totalTime += ", " + hours.ToString() + " hour"; }
            if (!string.IsNullOrEmpty(totalTime) && hours > 1) { totalTime += ", " + hours.ToString() + " hours"; }
            if (string.IsNullOrEmpty(totalTime) && hours == 1) { totalTime += hours.ToString() + " hour"; }
            if (string.IsNullOrEmpty(totalTime) && hours > 1) { totalTime += hours.ToString() + " hours"; }

            if (!string.IsNullOrEmpty(totalTime) && minutes == 1) { totalTime += ", " + minutes.ToString() + " minute"; }
            if (!string.IsNullOrEmpty(totalTime) && minutes > 1) { totalTime += ", " + minutes.ToString() + " minutes"; }
            if (string.IsNullOrEmpty(totalTime) && minutes == 1) { totalTime += minutes.ToString() + " minute"; }
            if (string.IsNullOrEmpty(totalTime) && minutes > 1) { totalTime += minutes.ToString() + " minutes"; }

            if (!string.IsNullOrEmpty(totalTime) && seconds == 1) { totalTime += ", " + seconds.ToString() + " second"; }
            if (!string.IsNullOrEmpty(totalTime) && seconds > 1) { totalTime += ", " + seconds.ToString() + " seconds"; }
            if (string.IsNullOrEmpty(totalTime) && seconds == 1) { totalTime += seconds.ToString() + " second"; }
            if (string.IsNullOrEmpty(totalTime) && seconds > 1) { totalTime += seconds.ToString() + " seconds"; }

            return totalTime;

        }
        private void NLabelUpdate(string notificationText, Color color)
        {
            notificationLabel.ForeColor = color;
            notificationLabel.Text = notificationText;
            notificationLabel.Invalidate();
            notificationLabel.Update();
            notificationLabel.Visible = true;
        }
        private void UsernameBox_Leave(object sender, EventArgs e)
        {
            CF.DefaultSettings["GmailAccount"] = usernameBox.Text;
        }
        private void PasswordBox_Leave(object sender, EventArgs e)
        {
            CF.DefaultSettings["Password"] = passwordBox.Text;
        }
        private void SendToBox_Leave(object sender, EventArgs e)
        {
            CF.DefaultSettings["NotifyAddress"] = sendToBox.Text;
        }


        /*The following methods are to check that video files are compatible with a certain device*/
        private void ListIncompatibleButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0; //Sets the tabcontrol to show data being presented on the page. 
            int loopcount = 0; //for progress bar value
            int fileCount = 0;

            if (compatibilityCombo.SelectedIndex > -1) // Selection rather than a typed input
            {
                if (Directory.Exists(CF.DefaultSettings["InputFilePath"]))
                {
                    NLabelUpdate("Checking and filtering directory for video files ", Color.GreenYellow);

                    string fileName = "";
                    filesListBox.Items.Clear();
                    VideoFilesList.Clear();
                    IncompatibilityInfo.Clear();
                    MediaInfoTB.Clear();


                    try
                    {
                        string[] fileNames = Directory
                                .GetFiles(CF.DefaultSettings["InputFilePath"], "*", SearchOption.AllDirectories)
                                .Where(file => file.ToLower().EndsWith(".mpg") || file.ToLower().EndsWith(".mpeg") || file.ToLower().EndsWith(".vob") || file.ToLower().EndsWith(".mod") || file.ToLower().EndsWith(".ts") || file.ToLower().EndsWith(".m2ts")
                                || file.ToLower().EndsWith(".mp4") || file.ToLower().EndsWith(".m4v") || file.ToLower().EndsWith(".mov") || file.ToLower().EndsWith("avi") || file.ToLower().EndsWith(".divx")
                                || file.ToLower().EndsWith(".wmv") || file.ToLower().EndsWith(".asf") || file.ToLower().EndsWith(".mkv") || file.ToLower().EndsWith(".flv") || file.ToLower().EndsWith(".f4v")
                                || file.ToLower().EndsWith(".dvr") || file.ToLower().EndsWith(".dvr-ms") || file.ToLower().EndsWith(".wtv") || file.ToLower().EndsWith(".ogv") || file.ToLower().EndsWith(".ogm")
                                || file.ToLower().EndsWith(".3gp") || file.ToLower().EndsWith(".rm") || file.ToLower().EndsWith(".rmvb"))
                                .ToArray();

                        Array.Sort(fileNames);

                        fileCount = fileNames.Count();


                        foreach (string file in fileNames) //loops through files, pulls out file names and adds them to filenameslistbox
                        {
                            StringBuilder outPutText = new StringBuilder();

                            loopcount = loopcount + 1;

                            NLabelUpdate("Analyzing file " + loopcount.ToString() + " of " + fileCount.ToString() + " - " + file, Color.GreenYellow);

                            if (compatibilityCombo.SelectedIndex == 1) //Xbox
                            {
                                if (!string.IsNullOrEmpty(XboxCompatibilityCheck(file)))
                                {
                                    fileName = file;
                                    while (fileName.Contains(@"\"))
                                    {
                                        fileName = fileName.Remove(0, 1);
                                    }
                                    CF.DefaultSettings["InputFilePath"] = file;
                                    CF.DefaultSettings["InputFilePath"] = file.Replace(fileName, "");

                                    filesListBox.Items.Add(fileName);
                                    filesListBox.Update();

                                    VideoFilesList.Add(file);
                                    IncompatibilityInfo.Add(XboxCompatibilityCheck(file));

                                    //Add filename, bitrate, framerate to the Media Info Box
                                    //getQuickInfo(file, fileName);

                                    for (int i = 0; i < IncompatibilityInfo.Count; i++)
                                    {
                                        outPutText.Append("\n" + filesListBox.Items[i] + "\n" + IncompatibilityInfo[i] + separator);
                                    }

                                    MediaInfoTB.Text = "\t\t\t\tINVALID ATTRIBUTES:\n\n" + outPutText.ToString();
                                }

                                NLabelUpdate("Listing " + filesListBox.Items.Count.ToString() + " Files Incompatible with Xbox360", Color.GreenYellow);
                            }
                            if (compatibilityCombo.SelectedIndex == 0) //Roku is selected
                            {
                                if (!string.IsNullOrEmpty(RokuCompatibilityCheck(file)))
                                {
                                    fileName = file;
                                    while (fileName.Contains(@"\"))
                                    {
                                        fileName = fileName.Remove(0, 1);
                                    }
                                    CF.DefaultSettings["InputFilePath"] = file;
                                    CF.DefaultSettings["InputFilePath"] = file.Replace(fileName, "");

                                    filesListBox.Items.Add(fileName);
                                    filesListBox.Update();

                                    VideoFilesList.Add(file);
                                    IncompatibilityInfo.Add(RokuCompatibilityCheck(file));

                                    //Add filename, bitrate, framerate to the Media Info Box
                                    for (int i = 0; i < IncompatibilityInfo.Count; i++)
                                    {
                                        outPutText.Append("\n" + filesListBox.Items[i] + "\n" + IncompatibilityInfo[i] + separator);
                                    }
                                    MediaInfoTB.Text = "\t\t\t\tINVALID ATTRIBUTES:\n\n" + outPutText.ToString();
                                }
                                NLabelUpdate("Listing " + filesListBox.Items.Count.ToString() + " Files Incompatible with Roku", Color.GreenYellow);
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show(ex.ToString(), 131, 280);
                    }
                }
            }
            else
            {
                NLabelUpdate("Please select a compatibility option", Color.Red);
            }
        }
        private string RokuCompatibilityCheck(string fileName)
        {

            //File Info
            decimal fileSize = 0;
            double maxBitrate = 0;
            double bitrate = 0;
            double audioChannels = 0;
            double audioBitrate = 0;
            double audioMaxBitrate = 0;
            //Incompatibility Info
            StringBuilder incompatible = new StringBuilder(); //Stores string of why file is incompatile with Roku

            NLabelUpdate("Checking if " + fileName + " is compatible with Roku players.", Color.GreenYellow);

            FileInfo fInfo = new FileInfo(fileName);
            fileSize = Convert.ToDecimal(fInfo.Length);
            fileSize = decimal.Round(((fileSize / 1024) / 1024), 2); //Bytes Converted to Megabytes

            MediaFile videoFile = new MediaFile(fileName);

            /*Video — MKV (H.264), MP4 (H.264), MOV (H.264), WMV (VC-1, firmware 3.1 only)
            * Audio: AAC, MP3
            * Must be Deinterlaced video
            * Max 2channel audio except for passthrough
            * Framerate must be <= 30 fps
            * Audio Bitrate 32-256Kbps per channel
            * AAC LC (CBR), AC3 Passthrough
            * Max video bitrate 1.5x average
            * Average video bitrate 1.0Mbps – 10Mbps
            * h.264 Main or High profile up to 4.1
            * Video Codec H.264/AVC
            * Extensions - .mp4, .mov, .m4v
            * Up to 1920 X 1080 pixel size
            */
            for (int i = 0; i < videoFile.Video.Count; i++)
            {
                if (videoFile.Video[i].FormatID != "H264" && videoFile.Video[i].FormatID != "x264" && videoFile.Video[i].FormatID != "h.264")
                {
                    incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": Format " + videoFile.Video[i].FormatID + ", must be H264 or X264\n");
                }

                if (videoFile.Video[i].FrameRate > 30)
                {
                    incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": Framerate " + videoFile.Video[i].FrameRate.ToString() + ", must be <= 30 using h264 codec\n");
                }

                if (videoFile.Video[i].Properties.ContainsKey("Maximum bit rate") && videoFile.Video[i].Properties.ContainsKey("Bit rate"))
                {
                    if (double.TryParse(videoFile.Video[i].Properties["Maximum bit rate"].Replace(" ", "").Replace("Kbps", ""), out maxBitrate)) { } else { maxBitrate = 0; }

                    if (videoFile.Video[i].Properties["Bit rate"].Contains("Mbps")) //If file bitrate is in Mbps
                    {
                        double.TryParse(videoFile.Video[0].Properties["Bit rate"].Replace(" ", "").Replace("Mbps", ""), out bitrate);
                        bitrate = bitrate * 1000; //accounts for reading in Mbps rather than Kbps
                    }
                    else if (videoFile.Video[i].Properties["Bit rate"].Contains("Kbps")) //If file bitrate is in Mbps
                    {
                        double.TryParse(videoFile.Video[0].Properties["Bit rate"].Replace(" ", "").Replace("Kbps", ""), out bitrate);
                    }
                    else
                    {
                        bitrate = 0;
                    }

                    if (bitrate > 10000) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": bitrate " + bitrate + ", Must be < 10000 kbps\n"); }
                    if (maxBitrate > (bitrate * 1.5)) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": Max bitrate (" + maxBitrate + ") > 1.5 * Average bitrate (" + bitrate + ")\n"); }
                }

                if (videoFile.Video[i].Properties.ContainsKey("Bit rate") && !videoFile.Video[i].Properties.ContainsKey("Maximum bit rate"))
                {
                    if (videoFile.Video[i].Properties["Bit rate"].Contains("Mbps")) //If file bitrate is in Mbps
                    {
                        double.TryParse(videoFile.Video[0].Properties["Bit rate"].Replace(" ", "").Replace("Mbps", ""), out bitrate);
                        bitrate = bitrate * 1000; //accounts for reading in Mbps rather than Kbps
                    }
                    else if (videoFile.Video[i].Properties["Bit rate"].Contains("Kbps")) //If file bitrate is in Mbps
                    {
                        double.TryParse(videoFile.Video[0].Properties["Bit rate"].Replace(" ", "").Replace("Kbps", ""), out bitrate);
                    }
                    else
                    {
                        bitrate = 0;
                    }

                    if (bitrate > 10000) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": bitrate " + bitrate + ", Must be < 10000 kbps\n"); }
                }

                if (videoFile.Video[i].Properties.ContainsKey("Format profile"))
                {
                    if (!videoFile.Video[i].Properties["Format profile"].Contains("High")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("Main")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("Baseline")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L1")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L1.0")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L1.B")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L1")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L1.1")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L1.2")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L1.3")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L2")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L2.0")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L2.1")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L2.2")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L3")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L3.0")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L3.1")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L3.2")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L4")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L4.0")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("L4.1"))
                    {
                        incompatible.Append("\tVideo" + (((i + 1).ToString())).Replace("0", "") + ":  Profile " + videoFile.Video[i].Properties["Format profile"] + ", Must be High, Main, or Baseline L1-4.1\n");
                    }
                }

                if (videoFile.Video[i].IsInterlaced) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": File must be Deinterlaced\n"); }

                if (videoFile.Video[i].Width > 1920 | videoFile.Video[i].Height > 1080) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": Frame size (" + videoFile.Video[i].Width.ToString() + " x " + videoFile.Video[i].Height.ToString() + "), Must be < 1920 x 1080\n"); }
            }

            for (int i = 0; i < videoFile.Audio.Count; i++)
            {
                if (videoFile.Audio[i].Properties.ContainsKey("Format"))
                {
                    if (!videoFile.Audio[i].Properties["Format"].Contains("AAC")
                        & !videoFile.Audio[i].Properties["Format"].Contains("MP3")
                        & !videoFile.Audio[i].Properties["Format"].Contains("MPEG"))
                    {
                        incompatible.Append("\tAudio" + ((i + 1).ToString()).Replace("0", "") + ": format - " + videoFile.Audio[i].Properties["Format"] + ", must be AAC or MP3\n");
                    }
                }

                if (videoFile.Audio[i].Properties.ContainsKey("Channel(s)"))
                {
                    if (double.TryParse(videoFile.Audio[i].Properties["Channel(s)"].Replace(" ", "").Replace("channels", "").Replace("channel", ""), out audioChannels) && audioChannels > 2)
                    {
                        incompatible.Append("\tAudio" + ((i + 1).ToString()).Replace("0", "") + ": has " + videoFile.Audio[i].Properties["Channel(s)"] + " must be <= 2\n");
                    }
                }

                if (videoFile.Audio[i].Properties.ContainsKey("Bit rate"))
                {
                    if (videoFile.Audio[i].Properties["Bit rate"].Contains("Mbps")) //If file bitrate is in Mbps
                    {
                        double.TryParse(videoFile.Audio[i].Properties["Bit rate"].Replace(" ", "").Replace("Mbps", ""), out audioBitrate);
                        audioBitrate = audioBitrate * 1000; //accounts for reading in Mbps rather than Kbps
                    }
                    else if (videoFile.Audio[i].Properties["Bit rate"].Contains("Kbps")) //If file bitrate is in Kbps
                    {
                        double.TryParse(videoFile.Audio[i].Properties["Bit rate"].Replace(" ", "").Replace("Kbps", ""), out audioBitrate);
                    }

                    if (videoFile.Audio[i].Properties.ContainsKey("Maximum bit rate"))
                    {
                        if (videoFile.Audio[i].Properties["Maximum bit rate"].Contains("Mbps")) //If file bitrate is in Mbps
                        {
                            double.TryParse(videoFile.Audio[i].Properties["Maximum bit rate"].Replace(" ", "").Replace("Mbps", ""), out audioMaxBitrate);
                            audioMaxBitrate = audioMaxBitrate * 1000; //accounts for reading in Mbps rather than Kbps
                        }
                        else if (videoFile.Audio[i].Properties["Maximum bit rate"].Contains("Kbps")) //If file bitrate is in Kbps
                        {
                            double.TryParse(videoFile.Audio[i].Properties["Maximum bit rate"].Replace(" ", "").Replace("Kbps", ""), out audioMaxBitrate);
                        }

                    }
                    if(videoFile.Audio[i].Description.Contains("ATMOS")) //ATMOS is at least 8 channels but shows 0 because it's object oriented.
                    {
                        audioBitrate = audioBitrate / 8;
                    }
                    else
                    {
                        audioBitrate = audioBitrate / videoFile.Audio[i].Channels; //Converts to bitrate per channel of audio
                    }
                    
                    if (audioBitrate != 0 & audioBitrate < 32 | audioBitrate > 256)
                    {
                        incompatible.Append("\tAudio" + ((i + 1).ToString()).Replace("0", "") + ": bitrate " + audioBitrate.ToString() + ", must be between 32 & 256 kbps\n");
                    }
                }
            }

            if (!videoFile.Extension.Contains("mp4")
                & !videoFile.Extension.Contains("m4v")
                & !videoFile.Extension.Contains("mov"))
            {
                incompatible.Append("\tContainer " + videoFile.Extension + ", conatiner must be mp4, m4v, or mov\n");
            }


            return incompatible.ToString();

        }
        private string XboxCompatibilityCheck(string fileName)
        {

            /*Xbox 360 Supported Video Formats
             * Except for WMV extension, files cannot exceed 4GB in size.
             * 
             * MPEG-4 Part 2 video, up to 5Mbps, 1280 x 720 pixels, 30 frames per second, 
             * Simple/Advanced Simple Profile in AVI containers with Dolby Digital 2 channel and 5.1 channel,
             * MP3 in .avi and .divx file formats;
             * 
             * H.264 video, up to 10 Mbps, 1920 x 1080 pixels, 30 frames per second,
             * Baseline/Main/High (up to level 4.1) Profiles in MPEG-4/QuickTime containers
             * with two-channel AAC low complexity (LC) in .mp4, .m4v, mp4v, .mov file formats;
             * 
             * MPEG-4 Part 2 video, up to 5Mbps, 1280 x 720 pixels, 30 frames per second,
             * Simple/Advanced Simple Profile in MPEG-4/QuickTime containers with two-channel
             * AAC low complexity (LC) in .avi and .divx file formats;
             * 
             * WMV (VC-1) video, up to 15Mbps, 1920 x 1080 pixels, 30 frames per second,
             * WMV7 (WMV1), WMV8 (WMV2), WMV9 (WMV3), VC-1 (WVC1 or WMVA) in Simple/Main/Advanced
             * up to level 3 in asf containers with WMA7/8, WMA 9 Pro (stereo and 5.1),
             * WMA lossless in .wmv file formats;
             */

            //File Info
            decimal fileSize = 0;

            //Incompatibility Info
            StringBuilder incompatible = new StringBuilder();

            FileInfo fInfo = new FileInfo(fileName);
            fileSize = Convert.ToDecimal(fInfo.Length);
            fileSize = decimal.Round(((fileSize / 1024) / 1024), 2); //Bytes Converted to Megabytes

            MediaFile videoFile = new MediaFile(fileName);

            NLabelUpdate("Checking if " + fileName + " is compatible with Xbox 360.", Color.GreenYellow);

            switch (videoFile.Extension)
            {
                case ".avi":
                    return XboxCompatibilityforAVIDIVX(videoFile, fileSize);
                case ".divx":
                    return XboxCompatibilityforAVIDIVX(videoFile, fileSize);
                case ".mov":
                    return XboxCompatibilityforMP4MOV(videoFile, fileSize);
                case ".mp4":
                    return XboxCompatibilityforMP4MOV(videoFile, fileSize);
                case ".m4v":
                    return XboxCompatibilityforMP4MOV(videoFile, fileSize);
                case ".mp4v":
                    return XboxCompatibilityforMP4MOV(videoFile, fileSize);
                case ".wmv":
                    return XboxCompatibilityforWMV(videoFile, fileSize);
                default:
                    incompatible.Append("\tExtension " + videoFile.Extension + " must be AVI, DIVX, M4V, MP4, MP4V, MOV or WMV\n");
                    return incompatible.ToString();
            }
        }
        private string XboxCompatibilityforMP4MOV(MediaFile videoFile, decimal fileSize)
        {
            /* H.264 video, up to 10 Mbps, 1920 x 1080 pixels, 30 frames per second,
            *  Baseline/Main/High (up to level 4.1) Profiles in MPEG-4/QuickTime containers
            *  with two-channel AAC low complexity (LC) in .mp4, .m4v, mp4v, .mov file formats;*/
            StringBuilder incompatible = new StringBuilder();

            if (videoFile.General.Bitrate > 10000)
            {
                incompatible.Append("\tOverall bitrate (" + videoFile.General.Bitrate + ") cannot be > 10000 kbps for " + videoFile.Extension + " files\n");
            }
            if (fileSize > 4096)
            {
                incompatible.Append("\tFile Size (" + fileSize + " MB) " + " must be <= 4096 MB (4GB) for " + videoFile.Extension + "\n");
            }

            for (int i = 0; i < videoFile.Video.Count; i++)
            {
                if (videoFile.Video[i].Width > 1920)
                {
                    incompatible.Append("\tVideo" + (i + 1).ToString() + ": frame width " + videoFile.Video[i].Width.ToString() + ", must be <=1920 for " + videoFile.Extension + "\n");
                }
                if (videoFile.Video[i].Height > 1080)
                {
                    incompatible.Append("\tVideo" + (i + 1).ToString() + ": frame height " + videoFile.Video[i].Height.ToString() + ", must be <=1080 for DIVX\n");
                }
                if (videoFile.Video[i].FrameRate > 30)
                {
                    incompatible.Append("\tVideo" + (i + 1).ToString() + ": FPS " + videoFile.Video[i].FrameRate.ToString().Replace(" ", "").Replace("Fps", "") + ", must be <=30 for " + videoFile.Extension + "\n");
                }

                if (videoFile.Video[i].Properties.ContainsKey("Format profile"))
                {
                    if (!videoFile.Video[i].Properties["Format profile"].Contains("Simple")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("Baseline")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("Main")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("High"))
                    {
                        if (!videoFile.Video[i].Properties["Format profile"].Contains("L1") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L1.0") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L1.B") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L1.1") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L1.2") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L1.3") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L2") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L2.0") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L2.1") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L2.2") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L3") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L3.0") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L3.1") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L3.2") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L4") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L4.0") &
                            !videoFile.Video[i].Properties["Format profile"].Contains("L4.1"))
                        {
                            incompatible.Append("\tVideo" + (i + 1).ToString() + ": Format " + videoFile.Video[i].Properties["Format profile"] + ", must be Simple, Baseline, Main, or High 1-4.1\n");
                        }
                    }
                }
            }
            for (int i = 0; i < videoFile.Audio.Count; i++)
            {
                if (videoFile.Audio[i].Properties.ContainsKey("Channel(s)"))
                {
                    if ((videoFile.Audio[i].Properties["Channel(s)"].Replace(" ", "").Replace("channels", "")) != "1"
                        & videoFile.Audio[i].Properties["Channel(s)"].Replace(" ", "").Replace("channels", "") != "2")
                    {
                        incompatible.Append("\tAudio " + (i + 1).ToString() + ": channel amount " + (videoFile.Audio[i].Properties["Channel(s)"].Replace(" ", "").Replace("channels", "")) + ", Must be <= 2 for " + videoFile.Extension + " containers\n");
                    }
                }

                if (!videoFile.Audio[i].Format.Contains("AAC"))
                {
                    incompatible.Append("\tAudio " + (i + 1).ToString() + ": format " + videoFile.Audio[i].Format + ", must be AAC for " + videoFile.Extension + " containers\n");
                }

                if (videoFile.Audio[i].Properties.ContainsKey("Format profile")
                    && !videoFile.Audio[i].Properties["Format profile"].Contains("LC")
                    && videoFile.Audio[i].Format.Contains("AAC"))
                {
                    incompatible.Append("\tAudio " + (i + 1).ToString() + ": profile " + videoFile.Audio[i].Properties["Format profile"] + ", must be LC for AAC audio in " + videoFile.Extension + " Containers\n");
                }
            }

            return incompatible.ToString();
        }
        private string XboxCompatibilityforAVIDIVX(MediaFile videoFile, decimal fileSize)
        {
            /*MPEG - 4 Part 2 video, up to 5Mbps, 1280 x 720 pixels, 30 frames per second, 
            Simple / Advanced Simple Profile in AVI containers with Dolby Digital 2 channel and 5.1 channel,
            MP3 in .avi and .divx file formats*/
            StringBuilder incompatible = new StringBuilder();

            if (videoFile.General.Bitrate > 5000)
            {
                incompatible.Append("\tOverall bitrate (" + videoFile.General.Bitrate + ") must be > 5000 kbps for " + videoFile.Extension + " files\n");
            }
            if (fileSize > 4096)
            {
                incompatible.Append("\tFile Size (" + fileSize + " MB) " + " must be <= 4096 MB (4GB) for " + videoFile.Extension + "\n");
            }

            for (int i = 0; i < videoFile.Video.Count; i++)
            {
                if (videoFile.Video[i].Width > 1280)
                {
                    incompatible.Append("\tVideo" + (i + 1).ToString() + ": frame width " + videoFile.Video[i].Width.ToString() + ", must be <=1280 for " + videoFile.Extension + "\n");
                }
                if (videoFile.Video[i].Height > 720)
                {
                    incompatible.Append("\tVideo" + (i + 1).ToString() + ": frame height " + videoFile.Video[i].Height.ToString() + ", must be <=720 for " + videoFile.Extension + "\n");
                }
                if (videoFile.Video[i].FrameRate > 30)
                {
                    incompatible.Append("\tVideo" + (i + 1).ToString() + ": FPS " + videoFile.Video[i].FrameRate + ", must be <=30 for " + videoFile.Extension + "\n");
                }

                if (videoFile.Video[i].Properties.ContainsKey("Format profile"))
                {
                    if (!videoFile.Video[i].Properties["Format profile"].Contains("Simple")
                        & !videoFile.Video[i].Properties["Format profile"].Contains("Baseline"))
                    {
                        incompatible.Append("\tVideo" + (i + 1).ToString() + ": format " + videoFile.Video[i].Properties["Format profile"] + ", must be Simple or Baseline\n");
                    }
                }
                if (!videoFile.Video[i].FormatID.Contains("MPEG"))
                {
                    incompatible.Append("\tVideo" + (i + 1).ToString() + ": format " + videoFile.Video[i].FormatID + " invalid for use with " + videoFile.Extension + " containers. Must be MPEG-4");
                }
            }
            for (int i = 0; i < videoFile.Audio.Count; i++)
            {
                if (videoFile.Audio[i].Properties.ContainsKey("Channel(s)"))
                {
                    if ((videoFile.Audio[i].Properties["Channel(s)"].Replace(" ", "").Replace("channels", "")) != "1"
                        & videoFile.Audio[i].Properties["Channel(s)"].Replace(" ", "").Replace("channels", "") != "2"
                        & videoFile.Audio[i].Properties["Channel(s)"].Replace(" ", "").Replace("channels", "") != "5.1")
                    {
                        incompatible.Append("\tAudio " + (i + 1).ToString() + ": channel amount " + (videoFile.Audio[i].Properties["Channel(s)"].Replace(" ", "").Replace("channels", "")) + ", must be <= 5.1\n");
                    }
                }
                if (!videoFile.Audio[i].Format.Contains("MP3")
                    & !videoFile.Audio[i].Format.Contains("AAC")
                    & !videoFile.Audio[i].Format.Contains("MPEG"))
                {
                    incompatible.Append("\tAudio " + (i + 1).ToString() + ": format " + videoFile.Audio[i].Format + ", must be MP3 or AAC\n");
                }

                if (videoFile.Audio[i].Properties.ContainsKey("Format profile")
                    && !videoFile.Audio[i].Properties["Format profile"].Contains("LC")
                    && videoFile.Audio[i].Format.Contains("AAC"))
                {
                    incompatible.Append("\tAudio " + (i + 1).ToString() + ": profile " + videoFile.Audio[i].Properties["Format profile"] + ", must be LC for AAC audio\n");
                }
            }
            return incompatible.ToString();
        }
        private string XboxCompatibilityforWMV(MediaFile videoFile, decimal fileSize)
        {
            /* WMV (VC-1) video, up to 15Mbps, 1920 x 1080 pixels, 30 frames per second,
             * WMV7 (WMV1), WMV8 (WMV2), WMV9 (WMV3), VC-1 (WVC1 or WMVA) in Simple/Main/Advanced
             * up to level 3 in asf containers with WMA7/8, WMA 9 Pro (stereo and 5.1),
             * WMA lossless in .wmv file formats;*/
            StringBuilder incompatible = new StringBuilder();

            if (videoFile.General.Bitrate > 15000)
            {
                incompatible.Append("\tOverall bitrate (" + videoFile.General.Bitrate + ") cannot be > 15000 kbps for " + videoFile.Extension + " files\n");
            }

            for (int i = 0; i < videoFile.Video.Count; i++)
            {
                if (videoFile.Video[i].Width > 1920)
                {
                    incompatible.Append("\tVideo" + (i + 1).ToString() + ": frame width of " + videoFile.Video[i].Width.ToString() + ", must be <= 1920\n");
                }

                if (videoFile.Video[i].Height > 1080)
                {
                    incompatible.Append("\tVideo" + (i + 1).ToString() + ": frame height of " + videoFile.Video[i].Height.ToString() + ", must be <= 1080\n");
                }

                if (videoFile.Video[i].FrameRate > 30)
                {
                    incompatible.Append("\tVideo" + (i + 1).ToString() + ": FPS " + videoFile.Video[i].FrameRate.ToString().Replace(" ", "").Replace("Fps", "") + ", must be <=30 for " + videoFile.Extension + " video\n");
                }
                if (videoFile.Video[i].Properties.ContainsKey("Format profile"))
                {
                    if (!videoFile.Video[i].Properties["Format profile"].Contains("VC-1") &
                    !videoFile.Video[i].Properties["Format profile"].Contains("WMV1") &
                    !videoFile.Video[i].Properties["Format profile"].Contains("WMV2") &
                    !videoFile.Video[i].Properties["Format profile"].Contains("WMV3") &
                    !videoFile.Video[i].Properties["Format profile"].Contains("WVC1") &
                    !videoFile.Video[i].Properties["Format profile"].Contains("WMVA"))
                    {
                        incompatible.Append("\tVideo" + (i + 1).ToString() + ": format " + videoFile.Video[i].Properties["Format profile"] + ", must be VC-1, WMV1-3, WVC1, OR WMVA\n");
                    }
                }
            }

            return incompatible.ToString();
        }

        //Conversion Presets
        private void PopulatePresets()
        {
            presetComboBox.Items.Clear();
            presetComboBox.Items.Add(""); //Add blank row at top

            for (int i = 0; i < PF.presetNames.Count(); i++)
            {
                presetComboBox.Items.Add(PF.presetNames[i]);
            }
        }
        private void PresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyPreset();
            CF.DefaultSettings["ConversionPreset"] = presetComboBox.Text;
            CF.UpdateDefaults();
        }
        private void PresetComboBox_Leave(object sender, EventArgs e)
        {
            CF.DefaultSettings["ConversionPreset"] = presetComboBox.Text;
            CF.UpdateDefaults();
        }
        private void AddPresetButton_Click(object sender, EventArgs e)
        {
            //Check that there is a preset name
            if(!string.IsNullOrEmpty(presetComboBox.Text))
            {
#pragma warning disable IDE0028 // Simplify collection initialization
                Dictionary<string, string> NewPreset = new Dictionary<string, string>();
#pragma warning restore IDE0028 // Simplify collection initialization
                NewPreset.Add("Name", presetComboBox.Text);
/*Audio Stream 1***********************************************************************************************************************************************************/
                NewPreset.Add("AudioCodec", audioCodecComboBox.Text);
                NewPreset.Add("AudioMixdown", mixdownComboBox.Text);
                NewPreset.Add("AudioSampleRate", sampleRateCombo.Text);
                if (filteredAACCheck.Checked) { NewPreset.Add("FilteredAACCheck", "true"); } else { NewPreset.Add("FilteredAACCheck", "false"); }
                if (filteredAC3Check.Checked) { NewPreset.Add("FilteredAC3Check", "true"); } else { NewPreset.Add("FilteredAC3Check", "false"); }
                if (filteredEAC3Check.Checked) { NewPreset.Add("FilteredEAC3Check", "true"); } else { NewPreset.Add("FilteredEAC3Check", "false"); }
                if (filteredDTSCheck.Checked) { NewPreset.Add("FilteredDTSCheck", "true"); } else { NewPreset.Add("FilteredDTSCheck", "false"); }
                if (filteredDTSHDCheck.Checked) { NewPreset.Add("FilteredDTSHDCheck", "true"); } else { NewPreset.Add("FilteredDTSHDCheck", "false"); }
                if (filteredTrueHDCheck.Checked) { NewPreset.Add("FilteredTrueHDCheck", "true"); } else { NewPreset.Add("FilteredTrueHDCheck", "false"); }
                if (filteredMP3Check.Checked) { NewPreset.Add("FilteredMP3Check", "true"); } else { NewPreset.Add("FilteredMP3Check", "false"); }
                if (filteredFLACCheck.Checked) { NewPreset.Add("FilteredFLACCheck", "true"); } else { NewPreset.Add("FilteredFLACCheck", "false"); }
                NewPreset.Add("AudioBitrate", audioBitrateCombo.Text);

/*Audio Stream 2***********************************************************************************************************************************************************/
                NewPreset.Add("AudioCodec2", audioCodecComboBox2.Text);
                NewPreset.Add("AudioMixdown2", mixdownComboBox2.Text);
                NewPreset.Add("AudioSampleRate2", sampleRateCombo2.Text);
                if (disableCheckStream2.Checked) { NewPreset.Add("EnableTrack2", "false"); } else { NewPreset.Add("EnableTrack2", "true"); }
                if (filteredAACCheck2.Checked) { NewPreset.Add("FilteredAACCheck2", "true"); } else { NewPreset.Add("FilteredAACCheck2", "false"); }
                if (filteredAC3Check2.Checked) { NewPreset.Add("FilteredAC3Check2", "true"); } else { NewPreset.Add("FilteredAC3Check2", "false"); }
                if (filteredEAC3Check2.Checked) { NewPreset.Add("FilteredEAC3Check2", "true"); } else { NewPreset.Add("FilteredEAC3Check2", "false"); }
                if (filteredDTSCheck2.Checked) { NewPreset.Add("FilteredDTSCheck2", "true"); } else { NewPreset.Add("FilteredDTSCheck2", "false"); }
                if (filteredDTSHDCheck2.Checked) { NewPreset.Add("FilteredDTSHDCheck2", "true"); } else { NewPreset.Add("FilteredDTSHDCheck2", "false"); }
                if (filteredTrueHDCheck2.Checked) { NewPreset.Add("FilteredTrueHDCheck2", "true"); } else { NewPreset.Add("FilteredTrueHDCheck2", "false"); }
                if (filteredMP3Check2.Checked) { NewPreset.Add("FilteredMP3Check2", "true"); } else { NewPreset.Add("FilteredMP3Check2", "false"); }
                if (filteredFLACCheck2.Checked) { NewPreset.Add("FilteredFLACCheck2", "true"); } else { NewPreset.Add("FilteredFLACCheck2", "false"); }
                NewPreset.Add("AudioBitrate2", audioBitrateCombo2.Text);

/*Audio Stream 3***********************************************************************************************************************************************************/
                NewPreset.Add("AudioCodec3", audioCodecComboBox3.Text);
                NewPreset.Add("AudioMixdown3", mixdownComboBox3.Text);
                NewPreset.Add("AudioSampleRate3", sampleRateCombo3.Text);
                if (disableCheckStream3.Checked) { NewPreset.Add("EnableTrack3", "false"); } else { NewPreset.Add("EnableTrack3", "true"); }
                if (filteredAACCheck3.Checked) { NewPreset.Add("FilteredAACCheck3", "true"); } else { NewPreset.Add("FilteredAACCheck3", "false"); }
                if (filteredAC3Check3.Checked) { NewPreset.Add("FilteredAC3Check3", "true"); } else { NewPreset.Add("FilteredAC3Check3", "false"); }
                if (filteredEAC3Check3.Checked) { NewPreset.Add("FilteredEAC3Check3", "true"); } else { NewPreset.Add("FilteredEAC3Check3", "false"); }
                if (filteredDTSCheck3.Checked) { NewPreset.Add("FilteredDTSCheck3", "true"); } else { NewPreset.Add("FilteredDTSCheck3", "false"); }
                if (filteredDTSHDCheck3.Checked) { NewPreset.Add("FilteredDTSHDCheck3", "true"); } else { NewPreset.Add("FilteredDTSHDCheck3", "false"); }
                if (filteredTrueHDCheck3.Checked) { NewPreset.Add("FilteredTrueHDCheck3", "true"); } else { NewPreset.Add("FilteredTrueHDCheck3", "false"); }
                if (filteredMP3Check3.Checked) { NewPreset.Add("FilteredMP3Check3", "true"); } else { NewPreset.Add("FilteredMP3Check3", "false"); }
                if (filteredFLACCheck3.Checked) { NewPreset.Add("FilteredFLACCheck3", "true"); } else { NewPreset.Add("FilteredFLACCheck3", "false"); }
                NewPreset.Add("AudioBitrate3", audioBitrateCombo3.Text);
/*Other Options************************************************************************************************************************************************************/
                NewPreset.Add("EncoderSpeed", encoderSpeedCombo.Text);
                NewPreset.Add("FrameRateMode", frameRateModeCombo.Text);
                NewPreset.Add("FrameRate", framerateCombo.Text);
                NewPreset.Add("EncoderTune", encoderTuneComboBox.Text);
                NewPreset.Add("VideoBitrate", avgBitrateCombo.Text);
                NewPreset.Add("EncoderProfile", encoderProfileComboBox.Text);
                NewPreset.Add("EncoderLevel", encoderLevelComboBox.Text);

                if (optimizeStreamingCheckBox.Checked) { NewPreset.Add("Optimize", "true"); } else { NewPreset.Add("Optimize", "false"); }
                if (autoCropCB.Checked) { NewPreset.Add("AutoCrop", "true"); } else { NewPreset.Add("AutoCrop", "false"); }
                if (twoPassCheckbox.Checked) { NewPreset.Add("TwoPass", "true"); } else { NewPreset.Add("TwoPass", "false"); }
                if (turboCheckBox.Checked) { NewPreset.Add("TurboFirstPass", "true"); } else { NewPreset.Add("TurboFirstPass", "false"); }


                //Subtitles
                NewPreset.Add("SubtitleSelection", subtitleCombo.Text);
                if (burnInSubtitlesCheck.Checked)
                {
                    NewPreset.Add("ForcedSubtitleBurnIn", "True");
                }
                else
                {
                    NewPreset.Add("ForcedSubtitleBurnIn", "False");
                }

                    PF.AddPreset(NewPreset);

                    //re-populate presets combobox
                    presetComboBox.Items.Clear();
                    presetComboBox.Items.Add(""); //Add in blank at the top
                    for (int i = 0; i < PF.presetNames.Count(); i++)
                    {
                        presetComboBox.Items.Add(PF.presetNames[i]);
                    }

                    NLabelUpdate("Added Preset: " + presetComboBox.Text, Color.GreenYellow);
            }
            else
            {
                NLabelUpdate("No name present. Preset must have a name.", Color.Red);
            }

        }
        private void RemovePresetButton_Click(object sender, EventArgs e)
        {
            if(presetComboBox.SelectedIndex != -1)
            {
                PF.RemovePreset(presetComboBox.Text);

                //re-populate presets combobox
                presetComboBox.Items.Clear();
                presetComboBox.Items.Add(""); //adds blank at the top

                for (int i = 0; i < PF.presetNames.Count(); i++)
                {
                    presetComboBox.Items.Add(PF.presetNames[i]);
                }
                NLabelUpdate("Removed preset: " + presetComboBox.Text,Color.GreenYellow);

                presetComboBox.Text = "";
            }
        }
        private void ApplyPreset()
        {
            int index = -1;

            if (!string.IsNullOrEmpty(presetComboBox.Text))
            {
                //Find index
                for (int i = 0; i < presetComboBox.Items.Count; i++)
                {
                    if(presetComboBox.Text == presetComboBox.Items[i].ToString())
                    {
                        index = i - 1; //the -1 is to account for the blank that I added in.
                    } 
                }


                if (PF.presetNames.Contains(presetComboBox.Text))
                {
                    //Pull values from preset dictionaries in the PF object.
                    audioCodecComboBox.Text = PF.PresetList[index]["AudioCodec"];
                    mixdownComboBox.Text = PF.PresetList[index]["AudioMixdown"];
                    sampleRateCombo.Text = PF.PresetList[index]["AudioSampleRate"];
                    audioBitrateCombo.Text = PF.PresetList[index]["AudioBitrate"];

                    if (PF.PresetList[index]["FilteredAACCheck"] == "true") { filteredAACCheck.Checked = true; } else { filteredAACCheck.Checked = false; }
                    if (PF.PresetList[index]["FilteredAC3Check"] == "true") { filteredAC3Check.Checked = true; } else { filteredAC3Check.Checked = false; }
                    if (PF.PresetList[index]["FilteredEAC3Check"] == "true") { filteredEAC3Check.Checked = true; } else { filteredEAC3Check.Checked = false; }
                    if (PF.PresetList[index]["FilteredDTSCheck"] == "true") { filteredDTSCheck.Checked = true; } else { filteredDTSCheck.Checked = false; }
                    if (PF.PresetList[index]["FilteredDTSHDCheck"] == "true") { filteredDTSHDCheck.Checked = true; } else { filteredDTSHDCheck.Checked = false; }
                    if (PF.PresetList[index]["FilteredTrueHDCheck"] == "true") { filteredTrueHDCheck.Checked = true; } else { filteredTrueHDCheck.Checked = false; }
                    if (PF.PresetList[index]["FilteredMP3Check"] == "true") { filteredMP3Check.Checked = true; } else { filteredMP3Check.Checked = false; }
                    if (PF.PresetList[index]["FilteredFLACCheck"] == "true") { filteredFLACCheck.Checked = true; } else { filteredFLACCheck.Checked = false; }

/*Audio Stream 2************************************************************************************************************************************************************************************/

                    audioCodecComboBox2.Text = PF.PresetList[index]["AudioCodec2"];
                    mixdownComboBox2.Text = PF.PresetList[index]["AudioMixdown2"];
                    sampleRateCombo2.Text = PF.PresetList[index]["AudioSampleRate2"];
                    audioBitrateCombo2.Text = PF.PresetList[index]["AudioBitrate2"];

                    if (PF.PresetList[index]["FilteredAACCheck2"] == "true") { filteredAACCheck2.Checked = true; } else { filteredAACCheck2.Checked = false; }
                    if (PF.PresetList[index]["FilteredAC3Check2"] == "true") { filteredAC3Check2.Checked = true; } else { filteredAC3Check2.Checked = false; }
                    if (PF.PresetList[index]["FilteredEAC3Check2"] == "true") { filteredEAC3Check2.Checked = true; } else { filteredEAC3Check2.Checked = false; }
                    if (PF.PresetList[index]["FilteredDTSCheck2"] == "true") { filteredDTSCheck2.Checked = true; } else { filteredDTSCheck2.Checked = false; }
                    if (PF.PresetList[index]["FilteredDTSHDCheck2"] == "true") { filteredDTSHDCheck2.Checked = true; } else { filteredDTSHDCheck2.Checked = false; }
                    if (PF.PresetList[index]["FilteredTrueHDCheck2"] == "true") { filteredTrueHDCheck2.Checked = true; } else { filteredTrueHDCheck2.Checked = false; }
                    if (PF.PresetList[index]["FilteredMP3Check2"] == "true") { filteredMP3Check2.Checked = true; } else { filteredMP3Check2.Checked = false; }
                    if (PF.PresetList[index]["FilteredFLACCheck2"] == "true") { filteredFLACCheck2.Checked = true; } else { filteredFLACCheck2.Checked = false; }

                    //This needs to be last as it triggers an event
                    if (PF.PresetList[index]["EnableTrack2"] == "true")//Track Enabled
                    {
                        disableCheckStream2.Checked = false; 
                        if(audioCodecComboBox2.Text == "Filtered Passthru")
                        {
                            filteredAACCheck2.Visible = true;
                            filteredAC3Check2.Visible = true;
                            filteredEAC3Check2.Visible = true;
                            filteredDTSCheck2.Visible = true;
                            filteredDTSHDCheck2.Visible = true;
                            filteredTrueHDCheck2.Visible = true;
                            filteredFLACCheck2.Visible = true;
                            filteredMP3Check2.Visible = true;
                        }
                        else
                        {
                            filteredAACCheck2.Visible = false;
                            filteredAC3Check2.Visible = false;
                            filteredEAC3Check2.Visible = false;
                            filteredDTSCheck2.Visible = false;
                            filteredDTSHDCheck2.Visible = false;
                            filteredTrueHDCheck2.Visible = false;
                            filteredFLACCheck2.Visible = false;
                            filteredMP3Check2.Visible = false;
                        }

                        audioCodecComboBox2.Enabled = true;
                        mixdownComboBox2.Enabled = true;
                        sampleRateCombo2.Enabled = true;
                        audioBitrateCombo2.Enabled = true;
                    }
                    else//Track Disabled
                    {
                        disableCheckStream2.Checked = true; 
                        filteredAACCheck2.Visible = false;
                        filteredAC3Check2.Visible = false;
                        filteredEAC3Check2.Visible = false;
                        filteredDTSCheck2.Visible = false;
                        filteredDTSHDCheck2.Visible = false;
                        filteredTrueHDCheck2.Visible = false;
                        filteredFLACCheck2.Visible = false;
                        filteredMP3Check2.Visible = false;

                        audioCodecComboBox2.Enabled = false;
                        mixdownComboBox2.Enabled = false;
                        sampleRateCombo2.Enabled = false;
                        audioBitrateCombo2.Enabled = false;
                    }


                    /*Audio Stream 3************************************************************************************************************************************************************************************/
                    audioCodecComboBox3.Text = PF.PresetList[index]["AudioCodec3"];
                    mixdownComboBox3.Text = PF.PresetList[index]["AudioMixdown3"];
                    sampleRateCombo3.Text = PF.PresetList[index]["AudioSampleRate3"];
                    audioBitrateCombo3.Text = PF.PresetList[index]["AudioBitrate3"];

                    if (PF.PresetList[index]["FilteredAACCheck3"] == "true") { filteredAACCheck3.Checked = true; } else { filteredAACCheck3.Checked = false; }
                    if (PF.PresetList[index]["FilteredAC3Check3"] == "true") { filteredAC3Check3.Checked = true; } else { filteredAC3Check3.Checked = false; }
                    if (PF.PresetList[index]["FilteredEAC3Check3"] == "true") { filteredEAC3Check3.Checked = true; } else { filteredEAC3Check3.Checked = false; }
                    if (PF.PresetList[index]["FilteredDTSCheck3"] == "true") { filteredDTSCheck3.Checked = true; } else { filteredDTSCheck3.Checked = false; }
                    if (PF.PresetList[index]["FilteredDTSHDCheck3"] == "true") { filteredDTSHDCheck3.Checked = true; } else { filteredDTSHDCheck3.Checked = false; }
                    if (PF.PresetList[index]["FilteredTrueHDCheck3"] == "true") { filteredTrueHDCheck3.Checked = true; } else { filteredTrueHDCheck3.Checked = false; }
                    if (PF.PresetList[index]["FilteredMP3Check3"] == "true") { filteredMP3Check3.Checked = true; } else { filteredMP3Check3.Checked = false; }
                    if (PF.PresetList[index]["FilteredFLACCheck3"] == "true") { filteredFLACCheck3.Checked = true; } else { filteredFLACCheck3.Checked = false; }

                    //This needs to be last as it triggers an event
                    if (PF.PresetList[index]["EnableTrack3"] == "true")//Track Enabled
                    {
                        disableCheckStream3.Checked = false;
                        if (audioCodecComboBox3.Text == "Filtered Passthru")
                        {
                            filteredAACCheck3.Visible = true;
                            filteredAC3Check3.Visible = true;
                            filteredEAC3Check3.Visible = true;
                            filteredDTSCheck3.Visible = true;
                            filteredDTSHDCheck3.Visible = true;
                            filteredTrueHDCheck3.Visible = true;
                            filteredFLACCheck3.Visible = true;
                            filteredMP3Check3.Visible = true;
                        }
                        else
                        {
                            filteredAACCheck3.Visible = false;
                            filteredAC3Check3.Visible = false;
                            filteredEAC3Check3.Visible = false;
                            filteredDTSCheck3.Visible = false;
                            filteredDTSHDCheck3.Visible = false;
                            filteredTrueHDCheck3.Visible = false;
                            filteredFLACCheck3.Visible = false;
                            filteredMP3Check3.Visible = false;
                        }

                        audioCodecComboBox3.Enabled = true;
                        mixdownComboBox3.Enabled = true;
                        sampleRateCombo3.Enabled = true;
                        audioBitrateCombo3.Enabled = true;
                    }
                    else//Track Disabled
                    {
                        disableCheckStream3.Checked = true; 
                        filteredAACCheck3.Visible = false;
                        filteredAC3Check3.Visible = false;
                        filteredEAC3Check3.Visible = false;
                        filteredDTSCheck3.Visible = false;
                        filteredDTSHDCheck3.Visible = false;
                        filteredTrueHDCheck3.Visible = false;
                        filteredFLACCheck3.Visible = false;
                        filteredMP3Check3.Visible = false;

                        audioCodecComboBox3.Enabled = false;
                        mixdownComboBox3.Enabled = false;
                        sampleRateCombo3.Enabled = false;
                        audioBitrateCombo3.Enabled = false;
                    }

                    /*Other Options******************************************************************************************************************************************************************************/
                    encoderSpeedCombo.Text = PF.PresetList[index]["EncoderSpeed"];
                    frameRateModeCombo.Text = PF.PresetList[index]["FrameRateMode"];
                    framerateCombo.Text = PF.PresetList[index]["FrameRate"];
                    encoderTuneComboBox.Text = PF.PresetList[index]["EncoderTune"];
                    avgBitrateCombo.Text = PF.PresetList[index]["VideoBitrate"];
                    encoderProfileComboBox.Text = PF.PresetList[index]["EncoderProfile"];
                    encoderLevelComboBox.Text = PF.PresetList[index]["EncoderLevel"];


                    if (PF.PresetList[index]["Optimize"] == "true") { optimizeStreamingCheckBox.Checked = true; } else { optimizeStreamingCheckBox.Checked = false; }
                    if (PF.PresetList[index]["AutoCrop"] == "true") { autoCropCB.Checked = true; } else { autoCropCB.Checked = false; }
                    if (PF.PresetList[index]["TwoPass"] == "true") { twoPassCheckbox.Checked = true; } else { twoPassCheckbox.Checked = false; }
                    if (PF.PresetList[index]["TurboFirstPass"] == "true") { turboCheckBox.Checked = true; } else { turboCheckBox.Checked = false; }

                    //subtitles
                    subtitleCombo.Text = PF.PresetList[index]["SubtitleSelection"];
                    if (PF.PresetList[index]["ForcedSubtitleBurnIn"] == "True") { burnInSubtitlesCheck.Checked = true; } else { burnInSubtitlesCheck.Checked = false; }
                }
            }
            
        }



        private void CompatibilityCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (compatibilityCombo.SelectedIndex)
            {
                case 0:
                    selectionPicturebox.Image = MovieDataCollector.Properties.Resources.Roku;
                    break;
                case -1:
                    selectionPicturebox.Image = MovieDataCollector.Properties.Resources.Roku;
                    break;
                case 1:
                    selectionPicturebox.Image = MovieDataCollector.Properties.Resources.xbox;
                    break;
                default:
                    selectionPicturebox.Image = MovieDataCollector.Properties.Resources.Roku;
                    break;
            }
        }
        private void disableCheckStream2_CheckedChanged(object sender, EventArgs e)
        {
            if (disableCheckStream2.Checked == false) //Track Enabled
            {
                audioCodecComboBox2.Enabled = true;
                mixdownComboBox2.Enabled = true;
                sampleRateCombo2.Enabled = true;
                audioBitrateCombo2.Enabled = true;

                if(audioCodecComboBox2.Text == "Filtered Passthru")
                {
                    filteredAACCheck2.Visible = true;
                    filteredAC3Check2.Visible = true;
                    filteredDTSCheck2.Visible = true;
                    filteredEAC3Check2.Visible = true;
                    filteredDTSHDCheck2.Visible = true;
                    filteredTrueHDCheck2.Visible = true;
                    filteredMP3Check2.Visible = true;
                    filteredFLACCheck2.Visible = true;
                    passthruFilterLabel2.Visible = true;
                }
                else
                {
                    filteredAACCheck2.Visible = false;
                    filteredAC3Check2.Visible = false;
                    filteredDTSCheck2.Visible = false;
                    filteredEAC3Check2.Visible = false;
                    filteredDTSHDCheck2.Visible = false;
                    filteredTrueHDCheck2.Visible = false;
                    filteredMP3Check2.Visible = false;
                    filteredFLACCheck2.Visible = false;
                    passthruFilterLabel2.Visible = false;

                }


            }
            else //Track Disabled
            {
                audioCodecComboBox2.Enabled = false;
                mixdownComboBox2.Enabled = false;
                sampleRateCombo2.Enabled = false;
                audioBitrateCombo2.Enabled = false;

                filteredAACCheck2.Visible = false;
                filteredAC3Check2.Visible = false;
                filteredDTSCheck2.Visible = false;
                filteredEAC3Check2.Visible = false;
                filteredDTSHDCheck2.Visible = false;
                filteredTrueHDCheck2.Visible = false;
                filteredMP3Check2.Visible = false;
                filteredFLACCheck2.Visible = false;
                passthruFilterLabel2.Visible = false;
            }
        }
        private void disableCheckStream3_CheckedChanged(object sender, EventArgs e)
        {
            if (disableCheckStream3.Checked == false) //Track Enabled
            {
                audioCodecComboBox3.Enabled = true;
                mixdownComboBox3.Enabled = true;
                sampleRateCombo3.Enabled = true;
                audioBitrateCombo3.Enabled = true;

                if (audioCodecComboBox3.Text == "Filtered Passthru")
                {
                    filteredAACCheck3.Visible = true;
                    filteredAC3Check3.Visible = true;
                    filteredDTSCheck3.Visible = true;
                    filteredEAC3Check3.Visible = true;
                    filteredDTSHDCheck3.Visible = true;
                    filteredTrueHDCheck3.Visible = true;
                    filteredMP3Check3.Visible = true;
                    filteredFLACCheck3.Visible = true;
                    passthruFilterLabel3.Visible = true;
                }
                else
                {
                    filteredAACCheck3.Visible = false;
                    filteredAC3Check3.Visible = false;
                    filteredDTSCheck3.Visible = false;
                    filteredEAC3Check3.Visible = false;
                    filteredDTSHDCheck3.Visible = false;
                    filteredTrueHDCheck3.Visible = false;
                    filteredMP3Check3.Visible = false;
                    filteredFLACCheck3.Visible = false;
                    passthruFilterLabel3.Visible = false;
                }


            }
            else //Track Disabled
            {
                audioCodecComboBox3.Enabled = false;
                mixdownComboBox3.Enabled = false;
                sampleRateCombo3.Enabled = false;
                audioBitrateCombo3.Enabled = false;

                filteredAACCheck3.Visible = false;
                filteredAC3Check3.Visible = false;
                filteredDTSCheck3.Visible = false;
                filteredEAC3Check3.Visible = false;
                filteredDTSHDCheck3.Visible = false;
                filteredTrueHDCheck3.Visible = false;
                filteredMP3Check3.Visible = false;
                filteredFLACCheck3.Visible = false;
                passthruFilterLabel3.Visible = false;
            }
        }

   
    }
}
