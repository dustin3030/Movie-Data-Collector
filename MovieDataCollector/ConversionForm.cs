using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics; //Allows for using Process.Start codes lines
using System.Drawing;
using System.IO; //allows for file manipulation
using System.Text;
using System.Net.Mail; // For Notification
using MediaInfoNET; /* https://teejeetech.blogspot.com/2013/01/mediainfo-wrapper-for-net-projects.html Copyright (c) 2013 Tony George (teejee2008@gmail.com)
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
        List<string> ParentDirectoryList = new List<string>();

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

        List<string> PassthruList = new List<string>(); //List used to store user selected audio passthru codecs.

        List<int> stereoTracks = new List<int>();
        List<int> surroundTracks = new List<int>();
        List<int> surroundBitrates = new List<int>();
        List<int> stereoBitrates = new List<int>();

        int selectedTrack1 = -1;
        int selectedTrack2 = -1;
        int selectedTrack3 = -1;


        //Create object containing Configuration information
        ConfigFile CF = new ConfigFile();
        //Create object containing preset information
        PresetFile PF = new PresetFile();
        public ConversionForm()
        {
            InitializeComponent(); //Initializes components.
            this.filesListBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.filesListBox_DragDrop);
            this.filesListBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.filesListBox_DragEnter);

            this.filenameTextBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.filenameTextBox_DragDrop);
            this.filenameTextBox.DragEnter += new System.Windows.Forms.DragEventHandler(this.filenameTextBox_DragEnter);

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

            /*Email Settings*/
            SMTPPortTB.Text = CF.DefaultSettings["SMTP_Port"];
            SMTPSeverTB.Text = CF.DefaultSettings["SMTP_Server"];
            usernameBox.Text = CF.DefaultSettings["SMTP_Account"];
            passwordBox.Text = CF.DefaultSettings["SMTP_Password"];
            sendToBox.Text = CF.DefaultSettings["NotifyAddress"];

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
            if(!string.IsNullOrEmpty(MediaInfoTB.Text)) //only run if there is something in the textbox to save
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
            else
            {
                NLabelUpdate("No information to save", Color.Red);
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
        private void QuickInfobutton_Click(object sender, EventArgs e)
        {

            if (VideoFilesList.Count > 0 & filesListBox.Items.Count > 0)
            {
                tabControl1.SelectedIndex = 0; //Selects Media Info Tab
                MediaInfoTB.Clear();
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

        //Drag and Drop functionality
        private void filesListBox_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }  
        }
        private void filesListBox_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            int loopcount = 0;
            string fileName = "";

            filesListBox.Items.Clear();
            filesListBox.Update();
            MediaInfoTB.Clear();
            VideoFilesList.Clear();
            IncompatibilityInfo.Clear();

            if (s.Count() == 1 && Directory.Exists(s[0].ToString()))
            {
                fileName = s[0].ToString();

                CF.DefaultSettings["InputFilePath"] = s[0].ToString();
                CF.UpdateDefaults();
                filenameTextBox.Text = CF.DefaultSettings["InputFilePath"];
                ReturnAllVideoFiles();

            }
            else
            {

                foreach (string file in s) //loops through files, pulls out file names and adds them to filenameslistbox
                {
                    loopcount = loopcount + 1;
                    NLabelUpdate("Processing file " + loopcount.ToString() + " of " + s.Count().ToString() + " - " + file, Color.GreenYellow);


                    if (!Directory.Exists(file))
                    {
                        fileName = file;
                        while (fileName.Contains(@"\"))
                        {
                            fileName = fileName.Remove(0, 1);
                        }


                        CF.DefaultSettings["InputFilePath"] = file.Replace(fileName, "");
                        CF.UpdateDefaults();

                        filesListBox.Items.Add(fileName);
                        filesListBox.Update();
                        VideoFilesList.Add(file);
                        ParentDirectoryList.Add(file);

                        filenameTextBox.Text = CF.DefaultSettings["InputFilePath"];
                    }

                }
            }

            NLabelUpdate("Listing " + filesListBox.Items.Count.ToString() + " Video Files", Color.GreenYellow);

        }

        private void filenameTextBox_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.All;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }
        private void filenameTextBox_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            string fileName = "";
            string[] s = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (s.Count() == 1 && Directory.Exists(s[0].ToString()))
            {
                fileName = s[0].ToString();

                CF.DefaultSettings["InputFilePath"] = s[0].ToString();
                CF.UpdateDefaults();
                filenameTextBox.Text = CF.DefaultSettings["InputFilePath"];
                ReturnAllVideoFiles();

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
                                Int32 port = Int32.Parse(SMTPPortTB.Text);
                                string server = SMTPSeverTB.Text;
                                string username = usernameBox.Text;
                                string password = passwordBox.Text;
                                string sendTo = sendToBox.Text;

                                if (VideoFilesList.Count == 1)
                                {
                                    await Task.Run(() =>
                                   {
                                       SendNotification(server, port, username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " failed. HandbrakeCLI exited with code" + exitCode.ToString());
                                   });
                                    
                                }
                                if (VideoFilesList.Count > 1)
                                {
                                    {
                                        SendNotification(server, port, username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. " + (VideoFilesList.Count() - Errors.Count()).ToString() + " of " + VideoFilesList.Count().ToString() + " files processed successfully in " + totalProcessingTime);
                                    }
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
                                Int32 port = Int32.Parse(SMTPPortTB.Text);
                                string server = SMTPSeverTB.Text;
                                string username = usernameBox.Text;
                                string password = passwordBox.Text;
                                string sendTo = sendToBox.Text;

                                if (filesListBox.SelectedIndices.Count == 1) { SendNotification(server, port, username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. The file was processed in " + totalProcessingTime); }
                                if (filesListBox.SelectedIndices.Count > 1) { SendNotification(server, port, username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. " + filesListBox.SelectedIndices.Count.ToString() + " files were processed in " + totalProcessingTime); }
                            }
                        }
                        else
                        {
                            MediaInfoTB.Text = ""; //Clears Output Box on successful Encode

                            if (VideoFilesList.Count == 1) { NLabelUpdate("The transcoding que initiated " + startTime.ToString() + " is now complete. The file was processed in " + totalProcessingTime, Color.GreenYellow); }
                            if (VideoFilesList.Count > 1) { NLabelUpdate("The transcoding que initiated " + startTime.ToString() + " is now complete. " + VideoFilesList.Count().ToString() + " files were processed in " + totalProcessingTime, Color.GreenYellow); }

                            if (notificationCheck.Checked)
                            {
                                Int32 port = Int32.Parse(SMTPPortTB.Text);
                                string server = SMTPSeverTB.Text;
                                string username = usernameBox.Text;
                                string password = passwordBox.Text;
                                string sendTo = sendToBox.Text;

                                if (VideoFilesList.Count == 1) { SendNotification(server, port, username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. The file was processed in " + totalProcessingTime); }
                                if (VideoFilesList.Count > 1) { SendNotification(server, port, username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. " + VideoFilesList.Count().ToString() + " files were processed in " + totalProcessingTime); }
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
            //int audioTrack = 0;
            string VideoString = "";
            string sourceOptions = "";
            string crop = "";

            MediaFile videoFile = new MediaFile(filepath);
            AudioString = AudioConversionString2(videoFile);

            VideoString = VideoConversionString(videoFile);

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
        private string VideoConversionString(MediaFile videoFile)
        {
            //Output Variables
            string outputEncoder = BuildEncoderString(videoFile); //Encoder, level, tune, 
            string outputEncopts = BuildEncoptsString(videoFile); //Encopts & Video Bitrate
            string outputFrameRate = BuildFramerateString(videoFile); //Video Framerate
            string subtitleString = BuildSubString(videoFile);

            string outputTwoPass = "";
            string outputTurbo = "";

            
            /*TwoPass & Turbo First***********************************************************************************************************************************************************************************************/
            if (twoPassCheckbox.Checked) { outputTwoPass = "--two-pass "; }
            if (turboCheckBox.Checked) { outputTurbo = "--turbo "; }

            return outputEncoder + subtitleString + outputEncopts + outputTwoPass + outputTurbo + outputFrameRate ;
        }

        //Framrate string
        private string BuildFramerateString(MediaFile videoFile)
        {

            string outputFrameRate = "";


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
            }
            else
            {
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
            return outputFrameRate;
        }

        //Encopts String & Video Bitrate
        private string BuildEncoptsString(MediaFile videoFile)
        {
            double avgBitrateCap = 0.0;
            string MaxBitrate = "";
            string BufferSize = "";
            double BitrateMultiplier = 1.5; //Size of the Maximum bitrate for the video portion of the file
            double BufferMultiplier = 2; //Size of the buffer
            string outputEncopts = "";
            double videoBitrate = 0.0;
            string outputVideoBitrate = "";

            if (videoFile.Video.Count > 0) //video stream found
            {
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
                    videoBitrate = videoFile.General.Bitrate; //- videoFile.Audio[audioTrack - 1].Bitrate; - Audio Track info changed in previous version. This breaks the code. use gneral bitrate if no other is foudn.
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

                /*https://www.chaneru.com/Roku/HLS/X264_Settings.htm#vbv-maxrate
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
            }
            else
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
                outputEncopts = "--encopts level=" + encoderLevelComboBox.Text + ":vbv-bufsize=" + BufferSize + ":vbv-maxrate=" + MaxBitrate + " --verbose=1 --encoder-level=\"" + encoderLevelComboBox.Text + "\" --encoder-profile=" + encoderProfileComboBox.Text.ToLower() + " --verbose=1 ";

            }

            //These settings set the buffer size and maximum video bitrate, also setting the encoder level
            outputEncopts = "--encopts level=" + encoderLevelComboBox.Text + ":vbv-bufsize=" + BufferSize + ":vbv-maxrate=" + MaxBitrate + " --verbose=1 --encoder-level=\"" + encoderLevelComboBox.Text + "\" --encoder-profile=" + encoderProfileComboBox.Text.ToLower() + " --verbose=1 ";
            return outputEncopts + outputVideoBitrate;
        }
        private string BuildEncoderString(MediaFile vidoeFile)
        {
            string outputEncoder = ""; //encoder and speed preset
            string outputEncoderSpeed = "";
            string outputEncoderTune = ""; //Encoder tune
            string outputEncoderProfile = "";
            string outputEncoderLevel = "";

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

            return outputEncoder + outputEncoderLevel + outputEncoderProfile + outputEncoderSpeed + outputEncoderTune;
        }
        
        //The following methods are for creating the command string for subtitle Selection
        private string BuildSubString(MediaFile videoFile)
        {
            int forcedStreamIndex = IdenfityForcedSubIndex(videoFile); //This checks for the burn in forced subs checkbox also. Will return -1 if that isn't check or if no subs are found
            int HCLISublistIndex = -1;
            string subString = "";
            List<int> nonPGSIndexes = new List<int> { };
            List<int> nonPGSLanguageMatchIndexes = new List<int> { };
            bool ForcedIndexFound = false;
            bool forcedSRTExists = false;

            char delim = '.';
            string[] Tokens = videoFile.File.Split(delim);
            //This looks weird because handbrake doesn't properly handle the \ escape character in the input srt string. I had to add extra so the output would have double \\ instead.
            string forcedSRTFileName = videoFile.File.Replace("\\", "\\\\").Replace("." + Tokens[Tokens.Count() - 1].ToString(), "-Forced.srt");


            //Check if external srt exists
            //Check for external subtitle with the same name as the video file except -Forced.
            if (System.IO.File.Exists(forcedSRTFileName))
            {
                forcedSRTExists = true;
            }


            //Build list of nonPGSIndexes
            for (int i = 0; i < videoFile.Text.Count(); i++)
            {
                if (videoFile.Text[i].Properties.ContainsKey("Format"))
                {
                    if (videoFile.Text[i].Properties["Format"] != "PGS")
                    {
                        nonPGSIndexes.Add(i);
                    }
                }
                if (i == forcedStreamIndex) { ForcedIndexFound = true; }
            }

            /*None,  All, Default, First, Chinese, Czech, English, Finnish, French, German, Greek, Japanese, Korean, Portuguese, Russian, Spanish, Swedish */

            if (forcedStreamIndex != -1) //Forced English subtitles found and burn forced subs checkbox selected
            {
                
                switch (subtitleCombo.Text)
                {
                    case "None": //Don't add any subtitles, but burn in forced track.

                        //Check for external subtitle with the same name as the video file except -Forced.
                        if (forcedSRTExists)
                        {
                            subString = "--srt-file \"" + forcedSRTFileName + "\" --srt-burn " + "--srt-codeset UTF-8 ";
                        }
                        else if(ForcedIndexFound)
                        {
                            subString = "--subtitle \"" + (forcedStreamIndex + 1).ToString() + "\" --subtitle-burned=1 "; //Only add the forced track
                        }

                    break;
                    case "All": //Include all non PGS tracks, PGS Tracks can't be added to MP4 unless they are burned in.

                        if (nonPGSIndexes.Count > 0)
                        {

                            for (int i = 0; i < nonPGSIndexes.Count(); i++)
                            {
                                if(nonPGSIndexes[i] == forcedStreamIndex) { HCLISublistIndex = i+1; }
                                if (string.IsNullOrEmpty(subString))
                                {
                                    subString = "--subtitle \"" + (nonPGSIndexes[i] + 1).ToString();
                                }
                                else
                                {
                                    subString += ", " + (nonPGSIndexes[i] + 1).ToString();
                                }
                            }
                            //Check for external subtitle with the same name as the video file except -Forced.
                            if (forcedSRTExists)
                            {
                                subString += "\" --srt-file \"" + forcedSRTFileName + "\" --srt-burn " + "--srt-codeset UTF-8 " + "--srt-lang eng";
                            }
                            else if(ForcedIndexFound)
                            {

                                subString += "\" --subtitle-burned=" + (HCLISublistIndex).ToString() + " ";
                            }
                            else //Selected forced subtitle index not found in list, must be added.
                            {
                                if (string.IsNullOrEmpty(subString))
                                {
                                    subString = "--subtitle \"" + (forcedStreamIndex + 1).ToString() + "\" --subtitle-burned=1 ";
                                }
                                else
                                {
                                    subString += ", " + (forcedStreamIndex + 1).ToString() + "\" --subtitle-burned=" + (nonPGSIndexes.Count() + 1).ToString();
                                }
                            }

                        }
                        else //No subtitles streams can be added, they are either all PGS or don't exist.
                        {
                            //Check for external subtitle with the same name as the video file except -Forced.
                            if (forcedSRTExists)
                            {
                                subString = "--srt-file \"" + forcedSRTFileName + "\" --srt-burn " + "--srt-codeset UTF-8 ";
                            }
                            else
                            {
                                subString = "--subtitle \"" + (forcedStreamIndex + 1).ToString() + "\" --subtitle-burned=1"; //since no non-pgs subtitles were added to the list, there is one 1 added, command list index of 1.
                            }
                        }

                        break;
                    case "First":

                        //Check for external subtitle with the same name as the video file except -Forced.
                        if (forcedSRTExists)
                        {
                            subString = "--subtitle \"1\" --srt-file \"" + forcedSRTFileName + "\" --srt-burn " + "--srt-codeset UTF-8 ";
                        }
                        else if (forcedStreamIndex == 0)
                        {
                            subString = "--subtitle \"1\" --subtitle-burned=1 ";
                        }
                        else
                        {
                            subString = "--subtitle \"1, " + (forcedStreamIndex + 1).ToString() + "\" --subtitle-burned=2"; //The forced stream is added second making it index 2 in the command string list.
                        }
                        break;
                    default:

                        //Find non pgs subs that match user selected language and add them to the list
                        for (int i = 0; i < nonPGSIndexes.Count(); i++)
                        {
                            
                            if (videoFile.Text[nonPGSIndexes[i]].Properties.ContainsKey("Language"))
                            {
                                if (videoFile.Text[nonPGSIndexes[i]].Properties["Language"] == subtitleCombo.Text)
                                {
                                    //Non PGS Subtitle Found with matching language
                                    nonPGSLanguageMatchIndexes.Add(nonPGSIndexes[i]);

                                    if (nonPGSIndexes[i] == forcedStreamIndex) { ForcedIndexFound = true; }
                                }
                            }
                        }


                        if (nonPGSLanguageMatchIndexes.Count() > 0) //non pgs subtitles that match user selected language found
                        {
                            for (int i = 0; i < nonPGSLanguageMatchIndexes.Count(); i++)
                            {
                                if (nonPGSLanguageMatchIndexes[i] == forcedStreamIndex) { HCLISublistIndex = i + 1; } //stores the index in the nonPGSIndexes list where the forced subtitle is stored. The +1 makes it the correct format for later
                                if (string.IsNullOrEmpty(subString))
                                {
                                    subString = "--subtitle \"" + (nonPGSLanguageMatchIndexes[i] + 1).ToString();
                                }
                                else
                                {
                                    subString += ", " + (nonPGSLanguageMatchIndexes[i] + 1).ToString();
                                }

                            }


                            if (string.IsNullOrEmpty(subString))
                            {
                                //Check for external subtitle with the same name as the video file except -Forced.
                                if (forcedSRTExists)
                                {
                                    subString = "--srt-file \"" + forcedSRTFileName + "\" --srt-burn " + "--srt-codeset UTF-8 ";
                                }
                                else
                                {
                                    subString = "--subtitle \"" + (forcedStreamIndex + 1).ToString() + "\" --subtitle-burned=1"; //nothing in command string list, burned string is first / 1
                                }
                                
                            }
                            else
                            {
                                //Check for external subtitle with the same name as the video file except -Forced.
                                if (forcedSRTExists)
                                {
                                    subString += "\" --srt-file \"" + forcedSRTFileName + "\" --srt-burn " + "--srt-codeset UTF-8 ";
                                }
                                else if (ForcedIndexFound)
                                {
                                    subString += "\" --subtitle-burned=" + (HCLISublistIndex).ToString() + " ";
                                }
                                else
                                {
                                    subString += ", " + (forcedStreamIndex + 1).ToString() + "\" --subtitle-burned=" + (HCLISublistIndex).ToString() + " ";
                                }
                            }

                        }
                        else //No non pgs subs found that match user language selection
                        {
                            //Check for external subtitle with the same name as the video file except -Forced.
                            if (forcedSRTExists)
                            {
                                subString = "--srt-file \"" + forcedSRTFileName + "\" --srt-burn " + "--srt-codeset UTF-8 ";
                            }
                            else //No overriding external subtitle found
                            {
                                subString = "--subtitle \"" + (forcedStreamIndex + 1).ToString() + "\" --subtitle-burned=" + (HCLISublistIndex).ToString() + " ";
                            }
                            
                        }
                        break;
                }

            }
            else //No Forced English Subtitles Found, or burn forced subs not selected.
            {
                switch (subtitleCombo.Text)
                {
                    case "None": //Don't add any subtitles or burn in any tracks
                        subString = "";
                        break;
                    case "All":
                        if(nonPGSIndexes.Count > 0)
                        {
                            for (int i = 0; i < nonPGSIndexes.Count(); i++)
                            {
                                if (string.IsNullOrEmpty(subString))
                                {
                                    subString = "--subtitle \"" + (nonPGSIndexes[i] + 1).ToString();
                                }
                                else
                                {
                                    subString += ", " + (nonPGSIndexes[i] + 1).ToString();
                                }
                            }
                            subString += "\" ";
                        }
                        else //Nothing to add
                        {
                            subString = "";
                        }
                        break;
                    case "First":
                        subString = "--first-subtitle ";
                        break;
                    default:
                        for (int i = 0; i < nonPGSIndexes.Count(); i++)
                        {
                            if(videoFile.Text[nonPGSIndexes[i]].Properties.ContainsKey("Language"))
                            {
                                if(videoFile.Text[nonPGSIndexes[i]].Properties["Language"] == subtitleCombo.Text)
                                {
                                    if (string.IsNullOrEmpty(subString))
                                    {
                                        subString = "--subtitle \"" + (nonPGSIndexes[i] + 1).ToString();
                                    }
                                    else
                                    {
                                        subString += ", " + (nonPGSIndexes[i] + 1).ToString();
                                    }
                                }
                            }
                            
                        }
                        subString += "\" ";
                        break;
                }
            }

            nonPGSIndexes.Clear();
            nonPGSLanguageMatchIndexes.Clear();

            return subString;
        }
        private int IdenfityForcedSubIndex(MediaFile videoFile)
        {
            int subStreamIndex = -1;
           
            //User selected to burn in forced subtitles.
            if (burnInSubtitlesCheck.Checked)
            {
                //Check that forced subs exist
                for (int i = 0; i < videoFile.Text.Count(); i++)
                {
                    if(subStreamIndex == -1)
                    {
                        if (videoFile.Text[i].Properties.ContainsKey("Language"))
                        {
                            if (videoFile.Text[i].Properties["Language"] == "English")
                            {
                                if (videoFile.Text[i].Properties.ContainsKey("Forced"))
                                {
                                    if (videoFile.Text[i].Properties["Forced"] == "Yes")
                                    {
                                        subStreamIndex = i;
                                    }
                                    else if (subStreamIndex == -1)
                                    {
                                        if (videoFile.Text[i].Properties.ContainsKey("Title"))
                                        {
                                            if (videoFile.Text[i].Properties["Title"].ToUpper().Contains("FORCED"))
                                            {
                                                subStreamIndex = i;
                                            }
                                        }
                                    }
                                    else if (subStreamIndex == -1)
                                    {
                                        if (videoFile.Text[i].Properties.ContainsKey("Title"))
                                        {
                                            if (videoFile.Text[i].Properties["Title"].ToUpper().Contains("FOREIGN")
                                                && videoFile.Text[i].Properties["Title"].ToUpper().Contains("ONLY"))
                                            {
                                                subStreamIndex = i;
                                            }
                                        }
                                    }
                                    else if (subStreamIndex == -1)
                                    {
                                        if (videoFile.Text[i].Properties.ContainsKey("Title"))
                                        {
                                            if (videoFile.Text[i].Properties["Title"].ToUpper().Contains("PARTS")
                                                && videoFile.Text[i].Properties["Title"].ToUpper().Contains("ONLY"))
                                            {
                                                subStreamIndex = i;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }

            }
            else
            {
                return subStreamIndex;
            }


            return subStreamIndex;
        }

        /*The following methods are for creating the audio conversion string*/
        /// <summary>
        /// Build string using the other audio methods.
        /// the goal is to form each part of the command string within the method for that part without requiring additional methods.
        /// </summary>
        /// <param name="videoFile"></param>
        /// <returns></returns>
        private string AudioConversionString2(MediaFile videoFile)
        {
            string audioOutput = "";
            //Generate audioPassthru list
            GeneratePassthruList();

            //Track & bitrate
            string audioTrack = AudioTrack(videoFile);
            string trackNames = AudioTrackNames(videoFile, selectedTrack1, selectedTrack2, selectedTrack3, PassthruList);
            string audioEncoder = AudioEncoder();
            string copyMask = AudioCopyMask();
            string fallback = AudioFallback();
            string sampleRate = AudioSampleRate(videoFile);
            string mixdown = AudioMixdown();
            string drc = AudioDynamicRangeCompression();
            string gain = AudioGain();

            audioOutput = audioTrack + trackNames + audioEncoder + copyMask + fallback + sampleRate + mixdown + drc + gain;

            return CheckforDuplicatedTracks(videoFile, audioTrack, trackNames, audioEncoder, copyMask, fallback, sampleRate, mixdown, drc, gain); //Removes tracks that will result in duplicated audio increasing file size but not gaining anything. (For instance two dolby pro logic tracks of the same bitrate)

            //return audioOutput;
        }
        private string CheckforDuplicatedTracks(MediaFile videofile, string audioTrack, string trackNames, string audioEncoder, string copyMask, string fallback, string sampleRate, string mixdown, string drc, string gain)
        {
            List<string> audioOutputList = new List<string>
            { audioTrack,
                trackNames,
                audioEncoder,
                copyMask,
                fallback,
                sampleRate,
                mixdown,
                drc,
                gain
            };
            string rString = "";
            string track1Type = "";
            string track2Type = "";
            string track3Type = "";

            for (int i = 0; i < surroundTracks.Count; i++)
            {
                if (selectedTrack1 == surroundTracks[i]) { track1Type = "surround"; }
                if (selectedTrack2 == surroundTracks[i]) { track2Type = "surround"; }
                if (selectedTrack3 == surroundTracks[i]) { track3Type = "surround"; }
            }

            for (int i = 0; i < stereoTracks.Count; i++)
            {
                if (selectedTrack1 == stereoTracks[i]) { track1Type = "stereo"; }
                if (selectedTrack2 == stereoTracks[i]) { track2Type = "stereo"; }
                if (selectedTrack3 == stereoTracks[i]) { track3Type = "stereo"; }
            }

            if (string.IsNullOrEmpty(track1Type)) { track1Type = "unknown"; }
            if (string.IsNullOrEmpty(track2Type)) { track2Type = "unknown"; }
            if (string.IsNullOrEmpty(track3Type)) { track3Type = "unknown"; }

            //If only 1 stereo track exists, eliminated tracks 2 & 3. There is no reason to keep multiple stereo tracks
            if (surroundTracks.Count <=0 && (!disableCheckStream2.Checked || !disableCheckStream3.Checked)) //No surround tracks exist and a second track is enabled either track 2 or 3
            {
                for (int i = 0; i < audioOutputList.Count; i++)
                {
                    switch (i)
                        {
                        case 0: //Audio Track
                            audioOutputList[i] = "--audio " + Program.GeneralParser(audioTrack, "--audio ", ",");
                            audioOutputList[i] += " --ab " + Program.GeneralParser(audioTrack, "--ab ", ",");
                            break;
                        case 1: //Track Names
                            audioOutputList[i] = " -A " + Program.GeneralParser(trackNames, "-A ", ",");
                            break;
                        case 2: //Audio Encoder
                            audioOutputList[i] = " --aencoder " + Program.GeneralParser(audioEncoder, "--aencoder ", ",");
                            break;
                        case 3: //Copy Mask
                            if(audioCodecComboBox.Text == "Filtered Passthru")
                            {
                                audioOutputList[i] = copyMask;
                            }
                            else
                            {
                                audioOutputList[i] = "";
                            }
                            break;
                        case 4: //Fallback Encoder
                            if (track1Type == "surround")
                            {
                                audioOutputList[i] = " --audio-fallback eac3 ";
                            }
                            else
                            {
                                audioOutputList[i] = " --audio-fallback fdk_aac ";
                            }
                            break;
                        case 5: //Sample Rate
                            audioOutputList[i] = " --arate " + Program.GeneralParser(sampleRate, "--arate ", ",");
                            break;
                        case 6: //Mixdown
                            audioOutputList[i] = " --mixdown " + Program.GeneralParser(mixdown, "--mixdown ", ",");
                            break;
                        case 7: //DRC
                            audioOutputList[i] = " --drc " + Program.GeneralParser(drc, "--drc ", ",");
                            break;
                        case 8: //Gain
                            audioOutputList[i] = " --gain " + Program.GeneralParser(gain, "--gain ", ",") + " ";
                            break;
                        default:
                            break;

                    }
                }
                
            }
            for (int a = 0; a < audioOutputList.Count; a++)
            {
                rString += audioOutputList[a];
            }

            return rString;
        }
        private void GeneratePassthruList()
        {
            if ((audioCodecComboBox.Text == "Filtered Passthru" && filteredAACCheck.Checked) ||
                (audioCodecComboBox2.Text == "Filtered Passthru" && filteredAACCheck2.Checked) ||
                (audioCodecComboBox3.Text == "Filtered Passthru" && filteredAACCheck3.Checked)) { PassthruList.Add("aac"); }

            if ((audioCodecComboBox.Text == "Filtered Passthru" && filteredAC3Check.Checked) ||
                (audioCodecComboBox2.Text == "Filtered Passthru" && filteredAC3Check2.Checked) ||
                (audioCodecComboBox3.Text == "Filtered Passthru" && filteredAC3Check3.Checked)) { PassthruList.Add("ac3"); }

            if ((audioCodecComboBox.Text == "Filtered Passthru" && filteredEAC3Check.Checked) ||
                (audioCodecComboBox2.Text == "Filtered Passthru" && filteredEAC3Check2.Checked) ||
                (audioCodecComboBox3.Text == "Filtered Passthru" && filteredEAC3Check3.Checked)) { PassthruList.Add("eac3"); }

            if ((audioCodecComboBox.Text == "Filtered Passthru" && filteredDTSCheck.Checked) ||
                (audioCodecComboBox2.Text == "Filtered Passthru" && filteredDTSCheck2.Checked) ||
                (audioCodecComboBox3.Text == "Filtered Passthru" && filteredDTSCheck3.Checked)) { PassthruList.Add("dts"); }

            if ((audioCodecComboBox.Text == "Filtered Passthru" && filteredDTSHDCheck.Checked) ||
                (audioCodecComboBox2.Text == "Filtered Passthru" && filteredDTSHDCheck2.Checked) ||
                (audioCodecComboBox3.Text == "Filtered Passthru" && filteredDTSHDCheck3.Checked)) { PassthruList.Add("dtshd"); }

            if ((audioCodecComboBox.Text == "Filtered Passthru" && filteredTrueHDCheck.Checked) ||
                (audioCodecComboBox2.Text == "Filtered Passthru" && filteredTrueHDCheck2.Checked) ||
                (audioCodecComboBox3.Text == "Filtered Passthru" && filteredTrueHDCheck3.Checked)) { PassthruList.Add("truehd"); }

            if ((audioCodecComboBox.Text == "Filtered Passthru" && filteredMP3Check.Checked) ||
                (audioCodecComboBox2.Text == "Filtered Passthru" && filteredMP3Check2.Checked) ||
                (audioCodecComboBox3.Text == "Filtered Passthru" && filteredMP3Check3.Checked)) { PassthruList.Add("mp3"); }

            if ((audioCodecComboBox.Text == "Filtered Passthru" && filteredFLACCheck.Checked) ||
                (audioCodecComboBox2.Text == "Filtered Passthru" && filteredFLACCheck2.Checked) ||
                (audioCodecComboBox3.Text == "Filtered Passthru" && filteredFLACCheck3.Checked)) { PassthruList.Add("flac16"); }

            if ((audioCodecComboBox.Text == "Filtered Passthru" && filteredFLACCheck.Checked) ||
                (audioCodecComboBox2.Text == "Filtered Passthru" && filteredFLACCheck2.Checked) ||
                (audioCodecComboBox3.Text == "Filtered Passthru" && filteredFLACCheck3.Checked)) { PassthruList.Add("flac24"); }

        }
        /*--audio "1,1,1" -A "Track1,Track2,Track3" --aencoder "fdk_aac,eac3,copy:dtshd" --audio-fallback "eac3" --ab 256,768,1509 --arate 48,48,48 --mixdown "dpl2,5point1,5point1" --drc 0,0,0 --gain 0,0,0*/
        /// <summary>
        /// The audio track selected from the input file should be selected as follows
        ///     -Must be and English Track (some tracks have no language code associated)
        ///     -Must be a surround track (unless mixdown is set to 2 channel output in which case if there is a stereo track that has a high enough bitrate use that one)
        ///     -Track should be the highest bitrate per channel track available meeting the above criteria
        ///     -Can list up to 3 tracks, must check user input to determine what tracks for each user selection.
        ///   
        /// </summary>
        /// <param name="videofile"></param>
        /// <returns></returns>
        private string AudioTrack(MediaFile videofile) //Maximum Bitrate for AC3 audio is 640 total. Handbrake will sanitize the audio to 640.
        {
            stereoTracks.Clear();
            surroundTracks.Clear();
            surroundBitrates.Clear();
            stereoBitrates.Clear();
            PassthruList.Clear();

            int maxSurroundBitrate = -1;
            int maxStereoBitrate = -1;
            int bestStereoTrack = -1;
            int bestSurroundTrack = -1;

            //Output Track command string
            string outputTrack1 = "";
            string outputTrack2 = "";
            string outputTrack3 = "";

            //Output Bitrate Command String
            string outputBitrate1 = "";
            string outputBitrate2 = "";
            string outputBitrate3 = "";

            int userSelectedBitrate1 = -1;
            int userSelectedBitrate2 = -1;
            int userSelectedBitrate3 = -1;

            int.TryParse(audioBitrateCombo.Text, out userSelectedBitrate1);
            int.TryParse(audioBitrateCombo2.Text, out userSelectedBitrate2);
            int.TryParse(audioBitrateCombo3.Text, out userSelectedBitrate3);

            for (int i = 0; i < videofile.Audio.Count; i++)
            {
                if (videofile.Audio[i].Properties.ContainsKey("Language"))
                {
                    if (videofile.Audio[i].Properties["Language"].ToUpper() == "ENGLISH" ||
                        videofile.Audio[i].Properties["Language"].ToUpper() == "ENG" ||
                        videofile.Audio[i].Properties["Language"].ToUpper() == "EN")
                    {
                        switch (videofile.Audio[i].Channels)
                        {
                            case 0: //Possibly Atmos Track or Unrecognized
                                if (videofile.Audio[i].Description.ToUpper().Contains("ATMOS")) //ATMOS 7.1 Track Identified
                                {
                                    //add track to Surround List
                                    surroundTracks.Add(i);
                                    surroundBitrates.Add(AudioBitrate(videofile, i) / 8);
                                }
                                else if (videofile.Audio[i].Description.ToUpper().Contains("TRUEHD")) //ATMOS 7.1 Track Identified
                                {
                                    //add track to Surround List
                                    surroundTracks.Add(i);
                                    surroundBitrates.Add(AudioBitrate(videofile, i) / 8);
                                }
                                else if (videofile.Audio[i].Properties.ContainsKey("Channel(s)"))
                                {
                                    if (videofile.Audio[i].Properties["Channel(s)"].Contains("Object Based")) //Dolby HD, Dolby TrueHD
                                    {
                                        //add track to Surround List
                                        surroundTracks.Add(i);
                                        surroundBitrates.Add(AudioBitrate(videofile, i) / 8);
                                    }
                                }
                                else if (videofile.Audio[i].Properties.ContainsKey("Channel(s)_Original"))
                                {
                                    //add track to Surround List
                                    surroundTracks.Add(i);
                                    if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("8")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 8); }
                                    else if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("7")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 7); }
                                    else if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("6")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 6); }
                                    else { surroundBitrates.Add(AudioBitrate(videofile, i) / 5); }
                                }
                                break;
                            case 1: //Mono
                                    //add track to Stereo List
                                stereoTracks.Add(i);
                                stereoBitrates.Add(AudioBitrate(videofile, i));
                                break;
                            case 2: // L,R
                                    //add track to Stereo List
                                if (videofile.Audio[i].Properties.ContainsKey("Channel(s)_Original"))
                                {
                                    surroundTracks.Add(i);
                                    if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("8")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 8); }
                                    else if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("7")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 7); }
                                    else if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("6")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 6); }
                                    else { surroundBitrates.Add(AudioBitrate(videofile, i) / 5); }

                                }
                                else
                                {
                                    stereoTracks.Add(i);
                                    stereoBitrates.Add(AudioBitrate(videofile, i) / 2);
                                }
                                break;
                            case 3: //2.1 L,R + Subwoofer
                                    //add track to Stereo List
                                stereoTracks.Add(i);
                                stereoBitrates.Add(AudioBitrate(videofile, i) / 3);
                                break;
                            case 4: //3.1 L,R,C + Subwoofer
                                    //add track to Stereo List
                                stereoTracks.Add(i);
                                stereoBitrates.Add(AudioBitrate(videofile, i) / 4);
                                break;
                            case 5: //4.1 L,R,SL,SR + Subwoofer
                                    //add track to Surround List
                                surroundTracks.Add(i);
                                surroundBitrates.Add(AudioBitrate(videofile, i) / 5);
                                break;
                            case 6: //5.1 L,R,C,SL,SR + Subwoofer
                                    //add track to Surround List
                                surroundTracks.Add(i);
                                surroundBitrates.Add(AudioBitrate(videofile, i) / 6);
                                break;
                            case 7: //6.1 L,R,C,SL,SR,SC + Subwoofer
                                    //add track to Surround List
                                surroundTracks.Add(i);
                                surroundBitrates.Add(AudioBitrate(videofile, i) / 7);
                                break;
                            case 8: //7.1 L,R,C L-Side, R-Side, SR, SL + Subwoofer
                                    //add track to Surround List
                                surroundTracks.Add(i);
                                surroundBitrates.Add(AudioBitrate(videofile, i) / 8);
                                break;
                            default: //other
                                     //add track to Stereo List
                                stereoTracks.Add(i);
                                stereoBitrates.Add(AudioBitrate(videofile, i) / 2);
                                break;
                        }

                    }
                }
                else //No Language Key Found
                {
                    switch (videofile.Audio[i].Channels)
                    {
                        case 0: //Possibly Atmos Track or Unrecognized
                            if (videofile.Audio[i].Description.ToUpper().Contains("ATMOS")) //ATMOS 7.1 Track Identified
                            {
                                //add track to Surround List
                                surroundTracks.Add(i);
                                surroundBitrates.Add(AudioBitrate(videofile, i) / 8);
                            }
                            else if (videofile.Audio[i].Description.ToUpper().Contains("TRUEHD")) //ATMOS 7.1 Track Identified
                            {
                                //add track to Surround List
                                surroundTracks.Add(i);
                                surroundBitrates.Add(AudioBitrate(videofile, i) / 8);
                            }
                            else if (videofile.Audio[i].Properties.ContainsKey("Channel(s)"))
                            {
                                if (videofile.Audio[i].Properties["Channel(s)"].Contains("Object Based")) //Dolby HD, Dolby TrueHD
                                {
                                    //add track to Surround List
                                    surroundTracks.Add(i);
                                    surroundBitrates.Add(AudioBitrate(videofile, i) / 8);
                                }
                            }
                            else if (videofile.Audio[i].Properties.ContainsKey("Channel(s)_Original"))
                            {
                                //add track to Surround List
                                surroundTracks.Add(i);
                                if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("8")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 8); }
                                else if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("7")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 7); }
                                else if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("6")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 6); }
                                else { surroundBitrates.Add(AudioBitrate(videofile, i) / 5); }
                            }
                            break;
                        case 1: //Mono
                            //add track to Stereo List
                            stereoTracks.Add(i);
                            stereoBitrates.Add(AudioBitrate(videofile, i));
                            break;
                        case 2: // L,R
                                //add track to Stereo List
                            if (videofile.Audio[i].Properties.ContainsKey("Channel(s)_Original"))
                            {
                                surroundTracks.Add(i);
                                if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("8")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 8); }
                                else if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("7")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 7); }
                                else if (videofile.Audio[i].Properties["Channel(s)_Original"].Contains("6")) { surroundBitrates.Add(AudioBitrate(videofile, i) / 6); }
                                else { surroundBitrates.Add(AudioBitrate(videofile, i) / 5); }
                            }
                            else
                            {
                                stereoTracks.Add(i);
                                stereoBitrates.Add(AudioBitrate(videofile, i) / 2);
                            }
                            break;
                        case 3: //2.1 L,R + Subwoofer
                            //add track to Stereo List
                            stereoTracks.Add(i);
                            stereoBitrates.Add(AudioBitrate(videofile, i) / 3);
                            break;
                        case 4: //3.1 L,R,C + Subwoofer
                            //add track to Stereo List
                            stereoTracks.Add(i);
                            stereoBitrates.Add(AudioBitrate(videofile, i) / 4);
                            break;
                        case 5: //4.1 L,R,SL,SR + Subwoofer
                            //add track to Surround List
                            surroundTracks.Add(i);
                            surroundBitrates.Add(AudioBitrate(videofile, i) / 5);
                            break;
                        case 6: //5.1 L,R,C,SL,SR + Subwoofer
                            //add track to Surround List
                            surroundTracks.Add(i);
                            surroundBitrates.Add(AudioBitrate(videofile, i) / 6);
                            break;
                        case 7: //6.1 L,R,C,SL,SR,SC + Subwoofer
                            //add track to Surround List
                            surroundTracks.Add(i);
                            surroundBitrates.Add(AudioBitrate(videofile, i) / 7);
                            break;
                        case 8: //7.1 L,R,C L-Side, R-Side, SR, SL + Subwoofer
                            //add track to Surround List
                            surroundTracks.Add(i);
                            surroundBitrates.Add(AudioBitrate(videofile, i) / 8);
                            break;
                        default: //other
                            //add track to Stereo List
                            stereoTracks.Add(i);
                            stereoBitrates.Add(AudioBitrate(videofile, i) / 2);
                            break;
                    }
                }

            }
            //Check max bitrate for surround tracks
            for (int i = 0; i < surroundTracks.Count; i++)
            {
                if(surroundBitrates[i] > maxSurroundBitrate)
                {
                    maxSurroundBitrate = surroundBitrates[i];
                    bestSurroundTrack = i;
                }
            }

            //Check max bitrate for stereo tracks
            for (int i = 0; i < stereoTracks.Count; i++)
            {
                if(stereoBitrates[i] > maxStereoBitrate)
                {
                    maxStereoBitrate = stereoBitrates[i];
                    bestStereoTrack = i;
                }
            }

            //Track 1 user selections
            switch(mixdownComboBox.Text)
            {
                case "Dolby ProLogic 2":
                    if (stereoBitrates.Count > 0 && surroundBitrates.Count > 0)
                    {
                        if (stereoBitrates[bestStereoTrack] > surroundBitrates[bestSurroundTrack])
                        {
                            outputTrack1 = (stereoTracks[bestStereoTrack] + 1).ToString();
                            //This should be the track index not the track number
                            selectedTrack1 = stereoTracks[bestStereoTrack];

                            //check/compare bitrate to that selected by user.
                            if (userSelectedBitrate1 <= stereoBitrates[bestStereoTrack])
                            {
                                outputBitrate1 = (userSelectedBitrate1 * 2).ToString();
                            }
                            else
                            {
                                outputBitrate1 = (stereoBitrates[bestStereoTrack] * 2).ToString();
                            }
                        }
                        else
                        {
                            outputTrack1 = (surroundTracks[bestSurroundTrack] + 1).ToString();
                            //This should be the track index not the track number
                            selectedTrack1 = surroundTracks[bestSurroundTrack];
                            //check/compare bitrate to that selected by user.
                            if (userSelectedBitrate1 <= surroundBitrates[bestSurroundTrack])
                            {
                                outputBitrate1 = (userSelectedBitrate1 * 2).ToString();
                            }
                            else
                            {
                                outputBitrate1 = (surroundBitrates[bestSurroundTrack] * 2).ToString();
                            }
                        }
                    }
                    //Either no surround exists or no stereo, try surround first
                    else if (surroundBitrates.Count > 0)
                    {
                        outputTrack1 = (surroundTracks[bestSurroundTrack] + 1).ToString();
                        //This should be the track index not the track number
                        selectedTrack1 = surroundTracks[bestSurroundTrack];
                        //check/compare bitrate to that selected by user.
                        if (userSelectedBitrate1 <= surroundBitrates[bestSurroundTrack])
                        {
                            outputBitrate1 = (userSelectedBitrate1 * 2).ToString();
                        }
                        else
                        {
                            outputBitrate1 = (surroundBitrates[bestSurroundTrack] * 2).ToString();
                        }
                    }
                    //No surround track exists, try stereo
                    else if (stereoBitrates.Count > 0)
                    {
                        outputTrack1 = (stereoTracks[bestStereoTrack]+1).ToString();
                        //This should be the track index not the track number
                        selectedTrack1 = stereoTracks[bestStereoTrack];
                        //check/compare bitrate to that selected by user.
                        if (userSelectedBitrate1 <= stereoBitrates[bestStereoTrack])
                        {
                            outputBitrate1 = (userSelectedBitrate1 * 2).ToString();
                        }
                        else
                        {
                            outputBitrate1 = (stereoBitrates[bestStereoTrack] * 2).ToString();
                        }
                    }
                    else //use first audio track, let handbrake error out
                    {
                        outputTrack1 = "1";
                        outputBitrate1 = (userSelectedBitrate1 * 2).ToString();
                        //This should be the track index not the track number
                        selectedTrack1 = 0;
                    }
                    break;
                case "5.1 Audio":
                    if(surroundBitrates.Count > 0)
                    {
                        outputTrack1 = (surroundTracks[bestSurroundTrack]+1).ToString();
                        //This should be the track index not the track number
                        selectedTrack1 = surroundTracks[bestSurroundTrack];
                        //check/compare bitrate to that selected by user.
                        if (userSelectedBitrate1 <= (surroundBitrates[bestSurroundTrack] * 6))
                        {
                            outputBitrate1 = (userSelectedBitrate1 * 6).ToString();
                        }
                        else
                        {
                            outputBitrate1 = (surroundBitrates[bestSurroundTrack] * 6).ToString();
                        }
                    }
                    else if(stereoBitrates.Count > 0)
                    {
                        outputTrack1 = (stereoTracks[bestStereoTrack]+1).ToString();
                        //This should be the track index not the track number
                        selectedTrack1 = stereoTracks[bestStereoTrack];
                        //check/compare bitrate to that selected by user.
                        if (userSelectedBitrate1 <= stereoBitrates[bestStereoTrack])
                        {
                            outputBitrate1 = (userSelectedBitrate1 * 2).ToString();
                        }
                        else
                        {
                            outputBitrate1 = (stereoBitrates[bestStereoTrack] * 2).ToString();
                        }
                    }
                    else //use first audio track, let handbrake error out
                    {
                        outputTrack1 = "1";
                        outputBitrate1 = (userSelectedBitrate1 * 6).ToString();
                        //This should be the track index not the track number
                        selectedTrack1 = 0;
                    }
                    break;
                default: //No mixdown selected, should be very rare
                    outputTrack1 = "1";
                    outputBitrate1 = (userSelectedBitrate1 * 2).ToString();
                    //This should be the track index not the track number
                    selectedTrack1 = 0;
                    break;
            }              

            //Track 2 user selections
            if(!disableCheckStream2.Checked) //Stream 2 enabled
            {
                switch (mixdownComboBox2.Text)
                {
 
                    case "Dolby ProLogic 2": //This may result in the same track twice...
                        
                        if (stereoBitrates.Count > 0 && surroundBitrates.Count > 0)
                        {
                            if (stereoBitrates[bestStereoTrack] > surroundBitrates[bestSurroundTrack])
                            {
                                outputTrack2 = "," + (stereoTracks[bestStereoTrack]+1).ToString();
                                //This should be the track index not the track number
                                selectedTrack2 = stereoTracks[bestStereoTrack];
                                //check/compare bitrate to that selected by user.
                                if (userSelectedBitrate2 <= stereoBitrates[bestStereoTrack])
                                {
                                    outputBitrate2 = "," + (userSelectedBitrate2 * 2).ToString();
                                }
                                else
                                {
                                    outputBitrate2 = "," + (stereoBitrates[bestStereoTrack] * 2).ToString();
                                }
                            }
                            else
                            {
                                outputTrack2 = "," + (surroundTracks[bestSurroundTrack]+1).ToString();
                                //This should be the track index not the track number
                                selectedTrack2 = surroundTracks[bestSurroundTrack];
                                //check/compare bitrate to that selected by user.
                                if (userSelectedBitrate2 <= surroundBitrates[bestSurroundTrack])
                                {
                                    outputBitrate2 = "," + (userSelectedBitrate2 * 2).ToString();
                                }
                                else
                                {
                                    outputBitrate2 = "," + (surroundBitrates[bestSurroundTrack] * 2).ToString();
                                }
                            }
                        }
                        //Either no surround exists or no stereo, try stereo first
                        else if (surroundBitrates.Count > 0)
                        {
                            outputTrack2 = "," + (surroundTracks[bestSurroundTrack]+1).ToString();
                            //This should be the track index not the track number
                            selectedTrack2 = surroundTracks[bestSurroundTrack];
                            //check/compare bitrate to that selected by user.
                            if (userSelectedBitrate2 <= surroundBitrates[bestSurroundTrack])
                            {
                                outputBitrate2 = "," + (userSelectedBitrate2 * 2).ToString();
                            }
                            else
                            {
                                outputBitrate2 = "," + (surroundBitrates[bestSurroundTrack] * 2).ToString();
                            }
                        }
                        //No surround track exists, try stereo
                        else if (stereoBitrates.Count > 0)
                        {
                            outputTrack2 = "," + (stereoTracks[bestStereoTrack]+1).ToString();
                            //This should be the track index not the track number
                            selectedTrack2 = stereoTracks[bestStereoTrack];
                            //check/compare bitrate to that selected by user.
                            if (userSelectedBitrate2 <= stereoBitrates[bestStereoTrack])
                            {
                                outputBitrate2 = "," + (userSelectedBitrate2 * 2).ToString();
                            }
                            else
                            {
                                outputBitrate2 = "," + (stereoBitrates[bestStereoTrack] * 2).ToString();
                            }
                        }
                        else //use first audio track, let handbrake error out
                        {
                            outputTrack2 = ",1";
                            outputBitrate2 = "," + (userSelectedBitrate2 * 2).ToString();
                            //This should be the track index not the track number
                            selectedTrack2 = 0;
                        }
                        break;
                    case "5.1 Audio":
                        if (surroundBitrates.Count > 0)
                        {
                            outputTrack2 = "," + (surroundTracks[bestSurroundTrack]+1).ToString();
                            //This should be the track index not the track number
                            selectedTrack2 = surroundTracks[bestSurroundTrack];
                            //check/compare bitrate to that selected by user.
                            if (userSelectedBitrate2 <= surroundBitrates[bestSurroundTrack])
                            {
                                outputBitrate2 = "," + (userSelectedBitrate2 * 6).ToString();
                            }
                            else
                            {
                                outputBitrate2 = "," + (surroundBitrates[bestSurroundTrack] * 6).ToString();
                            }
                        }
                        else if (stereoBitrates.Count > 0)
                        {
                            outputTrack2 = "," + (stereoTracks[bestStereoTrack]+1).ToString();
                            //This should be the track index not the track number
                            selectedTrack2 = stereoTracks[bestStereoTrack];
                            //check/compare bitrate to that selected by user.
                            if (userSelectedBitrate2 <= stereoBitrates[bestStereoTrack])
                            {
                                outputBitrate2 = "," + (userSelectedBitrate2 * 2).ToString();
                            }
                            else
                            {
                                outputBitrate2 = "," + (stereoBitrates[bestStereoTrack] * 2).ToString();
                            }
                        }
                        else //use first audio track, let handbrake error out
                        {
                            outputTrack2 = ",1";
                            outputBitrate2 = "," + (userSelectedBitrate1 * 6).ToString();
                            //This should be the track index not the track number
                            selectedTrack2 = 0;
                        }
                        break;
                    default:
                        outputTrack2 = ",1";
                        outputBitrate2 = "," + (userSelectedBitrate2 * 2).ToString();
                        //This should be the track index not the track number
                        selectedTrack2 = 0;
                        break;
                }
            }

            //Track 3 user selections
            if (!disableCheckStream3.Checked) //Stream 3 enabled
            {
                switch (mixdownComboBox3.Text)
                {
                    case "Dolby ProLogic 3":
                        if (stereoBitrates.Count > 0 && surroundBitrates.Count > 0)
                        {
                            if (stereoBitrates[bestStereoTrack] > surroundBitrates[bestSurroundTrack])
                            {
                                outputTrack3 = "," + (stereoTracks[bestStereoTrack] + 1).ToString();
                                //This should be the track index not the track number
                                selectedTrack3 = stereoTracks[bestStereoTrack];
                                //check/compare bitrate to that selected by user.
                                if (userSelectedBitrate3 <= stereoBitrates[bestStereoTrack])
                                {
                                    outputBitrate3 = "," + (userSelectedBitrate3 * 2).ToString();
                                }
                                else
                                {
                                    outputBitrate3 = "," + (stereoBitrates[bestStereoTrack] * 2).ToString();
                                }
                            }
                            else
                            {
                                outputTrack3 = "," + (surroundTracks[bestSurroundTrack] + 1).ToString();
                                //This should be the track index not the track number
                                selectedTrack3 = surroundTracks[bestSurroundTrack];
                                //check/compare bitrate to that selected by user.
                                if (userSelectedBitrate3 <= surroundBitrates[bestSurroundTrack])
                                {
                                    outputBitrate3 = "," + (userSelectedBitrate3 * 2).ToString();
                                }
                                else
                                {
                                    outputBitrate3 = "," + (surroundBitrates[bestSurroundTrack] * 2).ToString();
                                }
                            }
                        }
                        //Either no surround exists or no stereo, try stereo first
                        else if (surroundBitrates.Count > 0)
                        {
                            outputTrack3 = "," + (surroundTracks[bestSurroundTrack] + 1).ToString();
                            //This should be the track index not the track number
                            selectedTrack3 = surroundTracks[bestSurroundTrack];
                            //check/compare bitrate to that selected by user.
                            if (userSelectedBitrate3 <= surroundBitrates[bestSurroundTrack])
                            {
                                outputBitrate3 = "," + (userSelectedBitrate3 * 2).ToString();
                            }
                            else
                            {
                                outputBitrate3 = "," + (surroundBitrates[bestSurroundTrack] * 2).ToString();
                            }
                        }
                        //No surround track exists, try stereo
                        else if (stereoBitrates.Count > 0)
                        {
                            outputTrack3 = "," + (stereoTracks[bestStereoTrack] + 1).ToString();
                            //This should be the track index not the track number
                            selectedTrack3 = stereoTracks[bestStereoTrack];
                            //check/compare bitrate to that selected by user.
                            if (userSelectedBitrate3 <= stereoBitrates[bestStereoTrack])
                            {
                                outputBitrate3 = "," + (userSelectedBitrate3 * 2).ToString();
                            }
                            else
                            {
                                outputBitrate3 = "," + (stereoBitrates[bestStereoTrack] * 2).ToString();
                            }
                        }
                        else //use first audio track, let handbrake error out
                        {
                            outputTrack3 = ",1";
                            outputBitrate3 = "," + (userSelectedBitrate3 * 2).ToString();
                            //This should be the track index not the track number
                            selectedTrack3 = 0;
                        }
                        break;
                    case "5.1 Audio":
                        if (surroundBitrates.Count > 0)
                        {
                            outputTrack3 = "," + (surroundTracks[bestSurroundTrack] + 1).ToString();
                            //This should be the track index not the track number
                            selectedTrack3 = surroundTracks[bestSurroundTrack];
                            //check/compare bitrate to that selected by user.
                            if (userSelectedBitrate3 <= surroundBitrates[bestSurroundTrack])
                            {
                                outputBitrate3 = "," + (userSelectedBitrate3 * 6).ToString();
                            }
                            else
                            {
                                outputBitrate3 = "," + (surroundBitrates[bestSurroundTrack] * 6).ToString();
                            }
                        }
                        else if (stereoBitrates.Count > 0)
                        {
                            outputTrack3 = "," + (stereoTracks[bestStereoTrack] + 1).ToString();
                            //This should be the track index not the track number
                            selectedTrack3 = stereoTracks[bestStereoTrack];
                            //check/compare bitrate to that selected by user.
                            if (userSelectedBitrate3 <= stereoBitrates[bestStereoTrack])
                            {
                                outputBitrate3 = "," + (userSelectedBitrate3 * 2).ToString();
                            }
                            else
                            {
                                outputBitrate3 = "," + (stereoBitrates[bestStereoTrack] * 2).ToString();
                            }
                        }
                        else //use first audio track, let handbrake error out
                        {
                            outputTrack3 = ",1";
                            outputBitrate3 = "," + (userSelectedBitrate1 * 6).ToString();
                            //This should be the track index not the track number
                            selectedTrack3 = 0;
                        }
                        break;
                    default:
                        outputTrack3 = ",1";
                        outputBitrate3 = "," + (userSelectedBitrate3 * 2).ToString();
                        //This should be the track index not the track number
                        selectedTrack3 = 0;
                        break;
                }
            }


            return "--audio " + outputTrack1 + outputTrack2 + outputTrack3 + " --ab " + outputBitrate1 + outputBitrate2 + outputBitrate3 + " ";
        }
        /// <summary>
        /// Output audio bitrate should be capped by the user selection so as not to bloat the file.
        ///     -Some tracks show 0 bitrate, interpret those tracks and use the user selection
        ///     -Some tracks show multiple bitrates, assume the highest bitrate for calculations
        ///     -Bitrate calculations should be on a per channel basis, not as an entire stream
        /// </summary>
        /// <param name="AudioTracks"></param>
        /// <returns></returns>
        private int AudioBitrate(MediaFile videofile, int trackNumber)
        {

            int bitrate = 0;
            bitrate = videofile.Audio[trackNumber].Bitrate;

            if (bitrate == 0)
            {
                if (videofile.Audio[trackNumber].Properties.ContainsKey("Bit rate"))
                {
                    int.TryParse(videofile.Audio[trackNumber].Properties["Bit rate"].ToUpper().Replace(" ", "").Replace("UNKNOWN", "").Replace("KBPS", "").Replace("/", ""), out bitrate);
                }
            }

            return bitrate;
        }
        /// <summary>
        /// Track names should be pretty easy, there is no user input for this variable
        ///     -2 channel output tracks should be listed as Stereo
        ///     ->=6 channel output tracks should be labeled as Surround
        /// </summary>
        /// <param name="videofile"></param>
        /// <returns></returns>
        private string AudioTrackNames(MediaFile videofile, int selectedTrack1, int selectedTrack2, int selectedTrack3, List<string> PassthruList)
        {
            string trackName1 = "";
            string trackName2 = "";
            string trackName3 = "";

            //stores whether track is surround or stereo
            string track1Type = "";
            string track2Type = "";
            string track3Type = "";

            for (int i = 0; i < surroundTracks.Count; i++)
            {
                if(selectedTrack1 == surroundTracks[i]) { track1Type = "surround"; }
                if (selectedTrack2 == surroundTracks[i]) { track2Type = "surround"; }
                if (selectedTrack3 == surroundTracks[i]) { track3Type = "surround"; }
            }

            for (int i = 0; i < stereoTracks.Count; i++)
            {
                if (selectedTrack1 == stereoTracks[i]) { track1Type = "stereo"; }
                if (selectedTrack2 == stereoTracks[i]) { track2Type = "stereo"; }
                if (selectedTrack3 == stereoTracks[i]) { track3Type = "stereo"; }
            }

            if (string.IsNullOrEmpty(track1Type)) { track1Type = "unknown"; }
            if (string.IsNullOrEmpty(track2Type)) { track2Type = "unknown"; }
            if (string.IsNullOrEmpty(track3Type)) { track3Type = "unknown"; }

            //Track1
            if (selectedTrack1>=0) //Track is valid and selected by user (Not -1)
            {
                switch (audioCodecComboBox.Text)
                {
                    case "AAC (FDK)":
                        trackName1 = "\"Dolby Pro Logic 2 - AAC\"";
                        break;
                    case "Filtered Passthru":
                        bool passthru = false;
                        //Check to see if selected track codec is in passthru list
                        for (int i = 0; i < PassthruList.Count; i++)
                        {
                            if(!passthru) //only process if a match hasn't been found
                            {
                                if (videofile.Audio[selectedTrack1].Format.ToUpper().Contains(PassthruList[i].ToUpper()))
                                {
                                    passthru = true;

                                    if(track1Type == "surround") { trackName1 = "\"Surround - " + PassthruList[i] + "\""; }
                                    else if(track1Type == "stereo") { trackName1 = "\"Stero - " + PassthruList[i] + "\""; }
                                }
                            }
                        }
                        //If not in passthru list then use fallback encoder
                        if(!passthru)
                        {
                            if(track1Type == "stereo") { trackName1 = "\"Unknown\""; }
                        }

                        trackName1 = "Dolby Pro Logic 2 - AAC";
                        break;
                    case "AC3":
                        if(track1Type == "surround") { trackName1 = "\"Surround - AC3\""; }
                        else if(track1Type == "stereo") { trackName1 = "\"Stereo - AC3\""; }
                        else { trackName1 = "\"Unknown - AC3\""; }
                        break;
                    case "E-AC3":
                        if (track1Type == "surround") { trackName1 = "\"Surround - E-AC3\""; }
                        else if (track1Type == "stereo") { trackName1 = "\"Stereo - E-AC3\""; }
                        else { trackName1 = "\"Unknown - E-AC3\""; }
                        break;
                    default:
                        break;
                }
            }

            //Track 2
            if (selectedTrack2 >= 0) //Track is valid and selected by user (Not -1)
            {
                switch (audioCodecComboBox2.Text)
                {
                    case "AAC (FDK)":
                        trackName2 = ",\"Dolby Pro Logic 2 - AAC\"";
                        break;
                    case "Filtered Passthru":
                        bool passthru = false;
                        //Check to see if selected track codec is in passthru list
                        for (int i = 0; i < PassthruList.Count; i++)
                        {
                            if (!passthru) //only process if a match hasn't been found
                            {
                                if (videofile.Audio[selectedTrack2].Format.ToUpper().Contains(PassthruList[i].ToUpper()))
                                {
                                    passthru = true;

                                    if (track2Type == "surround") { trackName2 = ",\"Surround - " + PassthruList[i] + "\""; }
                                    else if (track2Type == "stereo") { trackName2 = ",\"Stereo - " + PassthruList[i] + "\""; }
                                }
                            }
                        }
                        //If not in passthru list then use fallback encoder
                        if (!passthru)
                        {
                            if (track2Type == "stereo") { trackName2 = ",\"Unknown\""; }
                        }

                        trackName2 = ",\"Dolby Pro Logic 2 - AAC\"";
                        break;
                    case "AC3":
                        if (track2Type == "surround") { trackName2 = ",\"Surround - AC3\""; }
                        else if (track2Type == "stereo") { trackName2 = ",\"Stereo - AC3\""; }
                        else { trackName2 = ",\"Unknown - AC3\""; }
                        break;
                    case "E-AC3":
                        if (track2Type == "surround") { trackName2 = ",\"Surround - E-AC3\""; }
                        else if (track2Type == "stereo") { trackName2 = ",\"Stereo - E-AC3\""; }
                        else { trackName2 = ",\"Unknown - E-AC3\""; }
                        break;
                    default:
                        break;
                }
            }

            //Track 3
            if (selectedTrack3 >= 0) //Track is valid and selected by user (Not -1)
            {
                switch (audioCodecComboBox3.Text)
                {
                    case "AAC (FDK)":
                        trackName3 = ",\"Dolby Pro Logic 2 - AAC\"";
                        break;
                    case "Filtered Passthru":
                        bool passthru = false;
                        //Check to see if selected track codec is in passthru list
                        for (int i = 0; i < PassthruList.Count; i++)
                        {
                            if (!passthru) //only process if a match hasn't been found
                            {
                                if (videofile.Audio[selectedTrack3].Format.ToUpper().Contains(PassthruList[i].ToUpper()))
                                {
                                    passthru = true;

                                    if (track3Type == "surround") { trackName3 = ",\"Surround - " + PassthruList[i] + "\""; }
                                    else if (track3Type == "stereo") { trackName3 = ",\"Stereo - " + PassthruList[i] + "\""; }
                                }
                            }
                        }
                        //If not in passthru list then use fallback encoder
                        if (!passthru)
                        {
                            if (track3Type == "stereo") { trackName3 = ",\"Unknown\""; }
                        }

                        trackName3 = ",\"Dolby Pro Logic 2 - AAC\"";
                        break;
                    case "AC3":
                        if (track3Type == "surround") { trackName3 = ",\"Surround - AC3\""; }
                        else if (track3Type == "stereo") { trackName3 = ",\"Stereo - AC3\""; }
                        else { trackName3 = ",\"Unknown - AC3\""; }
                        break;
                    case "E-AC3":
                        if (track3Type == "surround") { trackName3 = ",\"Surround - E-AC3\""; }
                        else if (track3Type == "stereo") { trackName3 = ",\"Stereo - E-AC3\""; }
                        else { trackName3 = ",\"Unknown - E-AC3\""; }
                        break;
                    default:
                        break;
                }
            }

            return "-A " + trackName1 + trackName2 + trackName3 + " " ;
        }
        /// <summary>
        /// Encoder should be based solely on user selection. Other options should be changed upon selection of encoder 
        /// such as selecting AAC then a mixdown of 5.1 should not be available.
        /// Check to see if the selected track & mixdown combination will output a stereo or surround track
        /// </summary>
        /// <param name="videofile"></param>
        /// <returns></returns>
        private string AudioEncoder()
        {
            string encoder1 = "";
            string encoder2 = "";
            string encoder3 = "";

            //encoder 1
            switch(audioCodecComboBox.Text)
            {
                case "AAC (FDK)":
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
                    break;
            }

            //encoder 2
            if (!disableCheckStream2.Checked) //enabled
            {
                switch (audioCodecComboBox2.Text)
                {
                    case "AAC (FDK)":
                        encoder2 = ",fdk_aac";
                        break;
                    case "AC3":
                        encoder2 = ",ac3";
                        break;
                    case "E-AC3":
                        encoder2 = ",eac3";
                        break;
                    case "Filtered Passthru":
                        encoder2 = ",copy";
                        break;
                    default:
                        break;
                }
            }

            //encoder 3
            if (!disableCheckStream3.Checked) //enabled
            {
                switch (audioCodecComboBox3.Text)
                {
                    case "AAC (FDK)":
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
                        break;
                }
            }



            return "--aencoder " + encoder1 + encoder2 + encoder3 + " " ;

        }
        /// <summary>
        /// If the selected encoder is passthru, the audio copymask will determine what gets passedthru based on the form selections.
        /// </summary>
        /// <returns></returns>
        private string AudioCopyMask()
        {
            string passthruOptions = "";
            for (int i = 0; i < PassthruList.Count; i++)
            {
                if (string.IsNullOrEmpty(passthruOptions))
                {
                    passthruOptions = PassthruList[i];
                }
                else
                {
                    passthruOptions += "," + PassthruList[i];
                }
            }

            if (string.IsNullOrEmpty(passthruOptions)) { return ""; } else { return "--audio-copy-mask " + passthruOptions + " "; }
            
        }
        /// <summary>
        /// Set audio codec to use when it is not possible to copy an audio track without re-encoding.
        ///     -Fallback for surround encodings should be EAC3
        ///     -Fallback for stereo encodings should be AAC
        ///     -When multiple tracks are selected for the output file then always use EAC3 as it is a more capable codec and can handle stereo and surround
        /// </summary>
        /// <returns></returns>
        private string AudioFallback()
        {
            string fallback = "--audio-fallback ";
            //stores whether track is surround or stereo
            string track1Type = "";
            string track2Type = "";
            string track3Type = "";

            for (int i = 0; i < surroundTracks.Count; i++)
            {
                if (selectedTrack1 == surroundTracks[i]) { track1Type = "surround"; }
                if (selectedTrack2 == surroundTracks[i]) { track2Type = "surround"; }
                if (selectedTrack3 == surroundTracks[i]) { track3Type = "surround"; }
            }

            for (int i = 0; i < stereoTracks.Count; i++)
            {
                if (selectedTrack1 == stereoTracks[i]) { track1Type = "stereo"; }
                if (selectedTrack2 == stereoTracks[i]) { track2Type = "stereo"; }
                if (selectedTrack3 == stereoTracks[i]) { track3Type = "stereo"; }
            }

            if (string.IsNullOrEmpty(track1Type)) { track1Type = "unknown"; }
            if (string.IsNullOrEmpty(track2Type)) { track2Type = "unknown"; }
            if (string.IsNullOrEmpty(track3Type)) { track3Type = "unknown"; }

            if(track1Type == "surround" || track2Type == "surround" || track3Type == "surround") { fallback += "eac3 "; } else { fallback += "fdk_aac "; }

            return fallback;

        }  
        /// <summary>
        /// Sample rate should be based on user selection 44.1 or 48
        ///     -If samplerate is higher than selection, drop to selection
        ///     -If samplerate is lower than selection use file bitrate only if it matches either 44.1 or 48
        ///     -When samplerate is unreadable use 48 as it is most common
        /// </summary>
        /// <param name="videofile"></param>
        /// <returns></returns>
        private string AudioSampleRate(MediaFile videofile)
        {
            string samplerate1 = "";
            string samplerate2 = "";
            string samplerate3 = "";

            //If parsing fails set to 48 the standard sample rate.
            double userSelectedRate1 = 48;
            double userSelectedRate2 = 48;
            double userSelectedRate3 = 48;

            double.TryParse(sampleRateCombo.Text, out userSelectedRate1);
            double.TryParse(sampleRateCombo2.Text, out userSelectedRate2);
            double.TryParse(sampleRateCombo3.Text, out userSelectedRate3);

            //Stream 1
            switch(videofile.Audio[selectedTrack1].SamplingRate)
            {
                case 48000:
                    samplerate1 = "48";
                    break;
                case 44100:
                    samplerate1 = "44.1";
                    break;
                default:
                    if (videofile.Audio[selectedTrack1].SamplingRate >= 48000)
                    {
                        samplerate1 = "48";
                    }
                    else if(videofile.Audio[selectedTrack1].SamplingRate < 48000)
                    {
                        samplerate1 = "44.1";
                    }
                    else
                    {
                        samplerate1 = "48";
                    }
                    break;
            }
               
            //Stream 2 Enabled
            if (!disableCheckStream2.Checked)
            {
                switch (videofile.Audio[selectedTrack2].SamplingRate)
                {
                    case 48000:
                        samplerate2 = ",48";
                        break;
                    case 44100:
                        samplerate2 = ",44.1";
                        break;
                    default:
                        if (videofile.Audio[selectedTrack2].SamplingRate >= 48000)
                        {
                            samplerate2 = ",48";
                        }
                        else if (videofile.Audio[selectedTrack2].SamplingRate < 48000)
                        {
                            samplerate2 = ",44.1";
                        }
                        else
                        {
                            samplerate2 = ",48";
                        }
                        break;
                }
            }

            //Stream 3 Enabled
            if (!disableCheckStream3.Checked)
            {
                switch (videofile.Audio[selectedTrack3].SamplingRate)
                {
                    case 48000:
                        samplerate3 = ",48";
                        break;
                    case 44100:
                        samplerate3 = ",44.1";
                        break;
                    default:
                        if (videofile.Audio[selectedTrack3].SamplingRate >= 48000)
                        {
                            samplerate3 = ",48";
                        }
                        else if (videofile.Audio[selectedTrack3].SamplingRate < 48000)
                        {
                            samplerate3 = ",44.1";
                        }
                        else
                        {
                            samplerate3 = ",48";
                        }
                        break;
                }
            }

            return "--arate " + samplerate1 + samplerate2 + samplerate3 + " " ;
        }
        /// <summary>
        /// Based on user selection, if user selects 5.1 but track is a stereo mixdown must change to Dolby Pro Logic 2
        /// IF user selection is Dolby Pro Logic 2, and track is 5.1, mixdown to user selection
        /// Mixdown surround tracks to 5.1
        /// Mixdown Stero Tracks to Dolby Pro Logic 2
        /// 
        /*--mixdown <string>  Format(s) for audio downmixing/upmixing:
            mono
            left_only
            right_only
            stereo
            dpl1
            dpl2
            5point1
            6point1
            7point1
            5_2_lfe
            Separate tracks by commas.
            Defaults:
            none        up to dpl2
            ca_aac      up to dpl2
            ca_haac     up to dpl2
            ac3         up to 5point1
            eac3        up to 5point1
            mp3         up to dpl2
            vorbis      up to dpl2
            flac16      up to 7point1
            flac24      up to 7point1
            opus        up to 7point1*/
        /// </summary>
        /// <param name="videofile"></param>
        /// <returns></returns>
        private string AudioMixdown()
        {
            string mixdown1 = "";
            string mixdown2 = "";
            string mixdown3 = "";

            //stores whether track is surround or stereo
            string track1Type = "";
            string track2Type = "";
            string track3Type = "";

            for (int i = 0; i < surroundTracks.Count; i++)
            {
                if (selectedTrack1 == surroundTracks[i]) { track1Type = "surround"; }
                if (selectedTrack2 == surroundTracks[i]) { track2Type = "surround"; }
                if (selectedTrack3 == surroundTracks[i]) { track3Type = "surround"; }
            }

            for (int i = 0; i < stereoTracks.Count; i++)
            {
                if (selectedTrack1 == stereoTracks[i]) { track1Type = "stereo"; }
                if (selectedTrack2 == stereoTracks[i]) { track2Type = "stereo"; }
                if (selectedTrack3 == stereoTracks[i]) { track3Type = "stereo"; }
            }

            if (string.IsNullOrEmpty(track1Type)) { track1Type = "unknown"; }
            if (string.IsNullOrEmpty(track2Type)) { track2Type = "unknown"; }
            if (string.IsNullOrEmpty(track3Type)) { track3Type = "unknown"; }

            
            //Mixdown1
            switch (mixdownComboBox.Text)
            {
                case "Dolby ProLogic 2":
                    mixdown1 = "dpl2";
                    break;
                case "5.1 Audio":
                    if (track1Type == "surround" || track1Type == "unknown")
                    {
                        mixdown1 = "5point1";
                    }
                    else
                    {
                        mixdown1 = "dpl2";
                    }
                    break;
                default:
                    mixdown1 = "dpl2";
                    break;
            }

            //Mixdown2
            if(!disableCheckStream2.Checked) //enabled
            {
                switch (mixdownComboBox2.Text)
                {
                    case "Dolby ProLogic 2":
                        mixdown2 = ",dpl2";
                        break;
                    case "5.1 Audio":
                        if (track2Type == "surround" || track2Type == "unknown")
                        {
                            mixdown2 = ",5point1";
                        }
                        else
                        {
                            mixdown2 = ",dpl2";
                        }
                        break;
                    default:
                        mixdown2 = ",dpl2";
                        break;
                }
            }

            //Mixdown3
            if (!disableCheckStream3.Checked) //enabled
            {
                switch (mixdownComboBox3.Text)
                {
                    case "Dolby ProLogic 2":
                        mixdown3 = ",dpl2";
                        break;
                    case "5.1 Audio":
                        if (track3Type == "surround" || track3Type == "unknown")
                        {
                            mixdown3 = ",5point1";
                        }
                        else
                        {
                            mixdown3 = ",dpl2";
                        }
                        break;
                    default:
                        mixdown3 = ",dpl2";
                        break;
                }
            }

            return "--mixdown " + mixdown1 + mixdown2 + mixdown3 + " ";
        }
        /// <summary>
        /// Use the default Dynamic Range Compression of 0
        /*Apply extra dynamic range compression to the audio, making soft sounds louder.Range is 1.0
            to 4.0 (too loud), with 1.5 - 2.5 being a useful range. Separate tracks by commas.*/
        /// </summary>
        /// <param name="videofile"></param>
        /// <returns></returns>
        private string AudioDynamicRangeCompression()
        {
            string drc2 = "";
            string drc3 = "";

            //Audio Stream 2 Enabled
            if (!disableCheckStream2.Checked) { drc2 = ",0"; }

            //Audio Stream 3 Enabled
            if (!disableCheckStream3.Checked) { drc3 = ",0"; }

            return "--drc 0" + drc2 + drc3 + " ";
        }
        /// <summary>
        /// Set at default of 0
        /*Amplify or attenuate audio before encoding.  Does NOT work with audio passthru(copy). 
            Values are in dB.Negative values attenuate, positive
            values amplify.A 1 dB difference is barely audible.*/
        /// </summary>
        /// <param name="videofile"></param>
        /// <returns></returns>
        private string AudioGain()
        {
            string gain2 = "";
            string gain3 = "";

            //Audio Stream 2 Enabled
            if (!disableCheckStream2.Checked) { gain2 = ",0"; }

            //Audio Stream 3 Enabled
            if (!disableCheckStream3.Checked) { gain3 = ",0"; }

            return "--gain 0" + gain2 + gain3 + " ";
        }

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
                    break;
                case "50":
                    NLabelUpdate("Framerates > 30 are not ROKU Compliant!", Color.Red);
                    break;
                case "59.94":
                    NLabelUpdate("Framerates > 30 are not ROKU Compliant!", Color.Red);
                    break;
                case "60":
                    NLabelUpdate("Framerates > 30 are not ROKU Compliant!", Color.Red);
                    break;
                default:
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
        /*private void SendNotification(string server, string userName, string password, string sendTo, string subject, string message)
        {
            //If this doesnt work it's because the gmail account needs to allow less secure apps access to send email
            //https://support.google.com/accounts/answer/6010255?hl=en
            if (notificationCheck.Checked) //Ensures that a notification doesn't try to send unless the user intends it.
            {
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(sendTo) && !string.IsNullOrEmpty(server))
                {
                    //NLabelUpdate("Attempting to send notification", Color.GreenYellow);

                    //smtp.gmail.com
                    if (SMTPSeverTB.Text.ToUpper().Contains("GMAIL"))
                    {
                        if (!userName.ToUpper().Contains("@GMAIL.COM")) { userName += "@gmail.com"; }
                        server = "smtp.gmail.com";
                    }

                    //smtp.mail.yahoo.com
                    if (SMTPSeverTB.Text.ToUpper().Contains("YAHOO"))
                    {
                        if (!userName.ToUpper().Contains("@YAHOO.COM")) { userName += "@yahoo.com"; }
                        server = "smtp.mail.yahoo.com";
                    }

                    //smtp-mail.outlook.com
                    if (SMTPSeverTB.Text.ToUpper().Contains("HOTMAIL"))
                    {
                        if (!userName.ToUpper().Contains("@HOTMAIL.COM")) { userName += "@hotmail.com"; }
                        server = "smtp-mail.outlook.com";
                    }

                    //smtp.live.com
                    if (SMTPSeverTB.Text.ToUpper().Contains("MSN"))
                    {
                        if (!userName.ToUpper().Contains("@MSN.COM")) { userName += "@msn.com"; }
                        server = "smtp.live.com";
                    }


                    try
                    {
                        MailMessage mail = new MailMessage();
                        SmtpClient SmtpServer = new SmtpClient(server);

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
                        SmtpServer.UseDefaultCredentials = false;


                        SmtpServer.Send(mail);
                        CustomMessageBox.Show("Notification Sent!", 120, 230, "Notification Message");
                        //NLabelUpdate("Notification sent to : " + sendTo, Color.GreenYellow);
                        CF.UpdateDefaults(); //Stores info in config file

                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show(ex.ToString(), 600, 300);
                        //NLabelUpdate("Notification failed to send.", Color.Red);
                    }
                }
                else
                {
                    CustomMessageBox.Show("Missing Notifiction Parameters", 600, 300);
                    //NLabelUpdate("Missing Notification Parameters", Color.Red);
                }
            }
        }*/

        private void SendNotification(string server, Int32 port, string userName, string password, string sendTo, string subject, string message)
        {
            NLabelUpdate("Sending Notification to " + sendTo, Color.GreenYellow);

            //Separate Domain from server address
            char delim = '.';
            string[] Tokens = server.Split(delim);
            string domain = Tokens[Tokens.Count()-2] + "." + Tokens[Tokens.Count()-1];

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
                subject = "";
            }


            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(server);


                mail.From = new MailAddress(userName + "@" + domain);
                mail.To.Add(sendTo);
                mail.Subject = subject;
                mail.Body = message;

                SmtpServer.Port = port;
                SmtpServer.Credentials = new System.Net.NetworkCredential(userName, password);
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);

                NLabelUpdate("Notification Sent Successfully",Color.GreenYellow);
                CF.UpdateDefaults(); //Stores info in config file

            }
            catch (Exception ex)
            {
                NLabelUpdate(ex.Message.ToString(), Color.Red);
                //CustomMessageBox.Show(ex.Message.ToString(), 300, 600, "Error on SMTP Connection");
            }
        }
        private void TestNotificationButton_Click(object sender, EventArgs e)
        {
            //Check that all boxes are filled
            bool portValue = false;
            bool serverValue = false;
            bool userNameValue = false;
            bool passwordValue = false;
            bool sendToValue = false;

            Int32 portNum = -1;

            //Verify Port number
            Int32.TryParse(SMTPPortTB.Text, out portNum);
            if(portNum != -1) { portValue = true; }
            if(portNum != 587) { NLabelUpdate("Port 587 is generally used for Secure SMTP transmissions, not port " + portNum.ToString() + ".", Color.Red); }

            //Verify Server
            if (!string.IsNullOrEmpty(SMTPSeverTB.Text)) { serverValue = true; }

            //Verify User Name
            if (!string.IsNullOrEmpty(usernameBox.Text)) { userNameValue = true; }

            //Verify Password
            if (!string.IsNullOrEmpty(passwordBox.Text)) { passwordValue = true; }

            //Verify Recipient / Send To
            if (!string.IsNullOrEmpty(sendToBox.Text) && sendToBox.Text.Contains("@") && sendToBox.Text.Contains(".")) { sendToValue = true; }

        

            if (portValue && serverValue && userNameValue && passwordValue & sendToValue)
            {
                Int32 port = Int32.Parse(SMTPPortTB.Text);
                string server = SMTPSeverTB.Text;
                string username = usernameBox.Text;
                string password = passwordBox.Text;
                string sendTo = sendToBox.Text;

                SendNotification(server, port, username, password, sendTo, "Test Notification", "Notification Test from Movie Data Collector");
            }
            else
            {
                string invalidValues = "";

                if (string.IsNullOrEmpty(invalidValues) && !portValue) { invalidValues += "Port"; } else if(!string.IsNullOrEmpty(invalidValues) && !portValue) { invalidValues += ", Port"; }
                if (string.IsNullOrEmpty(invalidValues) && !serverValue) { invalidValues += "Server"; } else if (!string.IsNullOrEmpty(invalidValues) && !serverValue) { invalidValues += ", Server"; }
                if (string.IsNullOrEmpty(invalidValues) && !userNameValue) { invalidValues += "Username"; } else if (!string.IsNullOrEmpty(invalidValues) && !userNameValue) { invalidValues += ", Username"; }
                if (string.IsNullOrEmpty(invalidValues) && !passwordValue) { invalidValues += "Password"; } else if (!string.IsNullOrEmpty(invalidValues) && !passwordValue) { invalidValues += ", Password"; }
                if (string.IsNullOrEmpty(invalidValues) && !sendToValue) { invalidValues += "Send To"; } else if (!string.IsNullOrEmpty(invalidValues) && !sendToValue) { invalidValues += ", Send To"; }

                if(!string.IsNullOrEmpty(invalidValues))
                {
                    NLabelUpdate("Some information is either missing or invalid: " + invalidValues + ".", Color.Red);
                }
            }

        }
        private void NotificationCheck_CheckedChanged(object sender, EventArgs e)
        {
            if(notificationCheck.Checked)
            {
                SMTPPortLbl.Visible = true;
                SMTPPortTB.Visible = true;
                SMTPServerLbl.Visible = true;
                SMTPSeverTB.Visible = true;
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
                SMTPPortLbl.Visible = false;
                SMTPPortTB.Visible = false;
                SMTPServerLbl.Visible = false;
                SMTPSeverTB.Visible = false;
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
            notificationLabel.Visible = true;
            notificationLabel.ForeColor = color;
            notificationLabel.Text = notificationText;
            notificationLabel.Invalidate();
            notificationLabel.Update();
        }
        private void SMTPPortTB_Leave(object sender, EventArgs e)
        {
            Int32 result = -1;
            Int32.TryParse(SMTPSeverTB.Text, out result);

            if(result != -1)
            {
                CF.DefaultSettings["SMTP_Port"] = SMTPPortTB.Text;
            }
            else
            {
                SMTPPortTB.Text = "";
                NLabelUpdate("SMTP Port must be a valid port number.", Color.Red);
            }
        }
        private void SMTPSeverTB_Leave(object sender, EventArgs e)
        {

            CF.DefaultSettings["SMTP_Server"] = SMTPSeverTB.Text;
        }
        private void UsernameBox_Leave(object sender, EventArgs e)
        {
            CF.DefaultSettings["SMTP_Account"] = usernameBox.Text;
        }
        private void PasswordBox_Leave(object sender, EventArgs e)
        {
            CF.DefaultSettings["SMTP_Password"] = passwordBox.Text;
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
            //Compatible Values
            List<string> FileExtention = new List<string>()
            {
                ".MP4",
                ".MOV",
                ".M4V",
                ".MKV",
            };

            List<string> VideoCodecs = new List<string>()
            {
                "H264",
                "X264",
                "H.264",
                "AVC",
                "HEVC",
                "H265",
                "H.265",
                "X265",
            };

            List<string> AudioCodecs = new List<string>()
            {
                "AAC",
                "HE-AAC",
                "AAC-LC",
                "MP3",
                "WMA",
                "WAV",
                "PCM",
                "AIFF",
                "FLAC",
                "ALAC",
                "AC3",
                "AC-3",
                "E-AC3",
                "DTS"
            };

            List<double> Framerates = new List<double>()
            {
                23.976,
                24,
                25,
                29.97,
                30,
                50,
                60
            };

            List<string> H264_Profiles = new List<string>()
            {
                "MAIN@L1",
                "MAIN@L1.0",
                "MAIN@L1.B",
                "MAINT@L1.1",
                "MAIN@L1.2",
                "MAINT@L1.3",
                "MAINT@L2",
                "MAIN@L2.2",
                "MAIN@L3",
                "MAIN@L3.1",
                "MAIN@L3.2",
                "MAIN@L4",
                "MAIN@L4.0",
                "MAIN@L4.1",
                "MAIN@4.2",
                "HIGH@L1.0",
                "HIGH@L1.B",
                "HIGHT@L1.1",
                "HIGH@L1.2",
                "HIGHT@L1.3",
                "HIGHT@L2",
                "HIGH@L2.2",
                "HIGH@L3",
                "HIGH@L3.1",
                "HIGH@L3.2",
                "HIGH@L4",
                "HIGH@L4.0",
                "HIGH@L4.1",
                "HIGH@4.2",
            };

            List<string> H265_Profiles = new List<string>()
            {
                "MAIN",
                "MAINT 10"
            };

            int maxCompatibleH264StreamingBitrate = 10; //in Mbps

            int maxCompatibleH265StreamingBitrate = 40; //in Mbps

            double peakBitrateMultiplier = 1.5; //must be less than 1.5 * Average bitrate

            //File Info
            double maxBitrate = 0;
            double bitrate = 0;
            double audioChannels = 0;
            double audioBitrate = 0;
            double audioMaxBitrate = 0;

            //Incompatibility Info
            StringBuilder incompatible = new StringBuilder(); //Stores string of why file is incompatile with Roku

            NLabelUpdate("Checking if " + fileName + " is compatible with Roku players.", Color.GreenYellow);

            MediaFile videoFile = new MediaFile(fileName);

            /*Roku officially supports the following media formats:
            Video file types: MP4, MOV, M4V, MKV, WebM
            Video codecs: H.264/AVC, HEVC/H.265, VP9
            Audio file types: AAC, MP3, WMA, WAV (PCM), AIFF, FLAC, ALAC, AC3, E-AC3
            Streaming protocols: HLS, Smooth, DASH
            */



            //Video Stream Check**********************************************************************************************************************************************************************************************
            for (int i = 0; i < videoFile.Video.Count; i++)
            {
                bool extentionMatch = false;
                //Container Check*********************************************************************************************************************************************************************************************
                for (int a = 0; a < FileExtention.Count; a++)
                {
                    if(videoFile.Extension.ToUpper() == FileExtention[a]) { extentionMatch = true; }
                }
                //add to incompatible text
                if (!extentionMatch) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": Invalid Container: " + videoFile.Extension + "\n"); }

                //Format Check************************************************************************************************************************************************************************************************
                bool formatMatch = false;

                //loop through format list and check each one
                for (int a = 0; a < VideoCodecs.Count; a++)
                {
                    if(videoFile.Video[i].FormatID.ToUpper().Contains(VideoCodecs[a]) )
                    {
                        formatMatch = true;
                    }
                }
                //add to incompatible text
                if (!formatMatch) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": Invalid Format: " + videoFile.Video[i].FormatID + "\n"); }

                //Framerate Check*********************************************************************************************************************************************************************************************
                bool frameratecheck = false;
                for (int a = 0; a < Framerates.Count; a++)
                {
                    if(videoFile.Video[i].FrameRate == Framerates[a]) { frameratecheck = true; }
                }
                if (!frameratecheck) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": Invalid Framerate: " + videoFile.Video[i].FrameRate.ToString() + "\n"); }

                //Bitrate Check***********************************************************************************************************************************************************************************************
                //H264 / AVC
                if (videoFile.Video[i].Format.ToUpper() =="AVC" || videoFile.Video[i].Format.ToUpper() == "H264" || videoFile.Video[i].Format.ToUpper() == "X264" || videoFile.Video[i].Format.ToUpper() == "H.264")
                {
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

                        if (bitrate > (maxCompatibleH264StreamingBitrate * 1000)) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": bitrate " + bitrate + ", Must be < " + (maxCompatibleH264StreamingBitrate * 1000).ToString() + " Mbps\n"); }
                        if (maxBitrate > (bitrate * peakBitrateMultiplier)) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": Peak bitrate (" + maxBitrate + ") > 1.5 * Average bitrate (" + bitrate + ")\n"); }
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

                        if (bitrate > (maxCompatibleH264StreamingBitrate * 1000)) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": bitrate " + bitrate + ", Must be < " + (maxCompatibleH264StreamingBitrate * 1000).ToString() + " kbps\n"); }
                    }

                    //Check H264 Compatible Profiles
                    bool compatibleProfileCheck = false;
                    for (int a = 0; a < H264_Profiles.Count; a++)
                    {
                        if(videoFile.Video[i].Properties.ContainsKey("Format profile"))
                        {
                            if(videoFile.Video[i].Properties["Format profile"].ToUpper() == H264_Profiles[a]) { compatibleProfileCheck = true; }
                        }
                    }
                    if (!compatibleProfileCheck) { incompatible.Append("\tVideo" + (((i + 1).ToString())).Replace("0", "") + ":  Profile Invalid: " + videoFile.Video[i].Properties["Format profile"] + "\n"); }
                }

                //H265 / HEVC
                if (videoFile.Video[i].Format.ToUpper() == "HEVC" || videoFile.Video[i].Format.ToUpper() == "H265" || videoFile.Video[i].Format.ToUpper() == "X265" || videoFile.Video[i].Format.ToUpper() == "H.265")
                {
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

                        if (bitrate > (maxCompatibleH265StreamingBitrate * 1000)) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": bitrate " + bitrate + ", Must be < " + maxCompatibleH265StreamingBitrate.ToString() + " Mbps\n"); }
                        if (maxBitrate > (bitrate * peakBitrateMultiplier)) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": Peak bitrate (" + maxBitrate + ") > 1.5 * Average bitrate (" + bitrate + ")\n"); }
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

                        if (bitrate > (maxCompatibleH265StreamingBitrate * 1000)) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": bitrate " + bitrate + ", Must be < " + maxCompatibleH265StreamingBitrate + " Mbps\n"); }
                    }

                    //Check H265 Compatible Profiles
                    bool compatibleProfileCheck = false;
                    for (int a = 0; a < H265_Profiles.Count; a++)
                    {
                        if (videoFile.Video[i].Properties.ContainsKey("Format profile"))
                        {
                            if (videoFile.Video[i].Properties["Format profile"] == H265_Profiles[a]) { compatibleProfileCheck = true; }
                        }
                    }

                    if (!compatibleProfileCheck) { incompatible.Append("\tVideo" + (((i + 1).ToString())).Replace("0", "") + ":  Profile Invalid: " + videoFile.Video[i].Properties["Format profile"] + "\n"); }
                }

                //Codec Profile***********************************************************************************************************************************************************************************************
                

                if (videoFile.Video[i].IsInterlaced) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": File must be Deinterlaced\n"); }

                if (videoFile.Video[i].Width > 1920 | videoFile.Video[i].Height > 1080) { incompatible.Append("\tVideo" + ((i + 1).ToString()).Replace("0", "") + ": Frame size (" + videoFile.Video[i].Width.ToString() + " x " + videoFile.Video[i].Height.ToString() + "), Must be < 1920 x 1080\n"); }
            }

            //Audio Stream Check**********************************************************************************************************************************************************************************************
            
            for (int i = 0; i < videoFile.Audio.Count; i++)
            {
                //Audio Codec*************************************************************************************************************************************************************************************************
                if (videoFile.Audio[i].Properties.ContainsKey("Format"))
                {
                    bool audioCodecCheck = false;

                    for (int a = 0; a < AudioCodecs.Count; a++)
                    {
                        if(videoFile.Audio[i].Properties["Format"].Contains(AudioCodecs[a]))
                        {
                            audioCodecCheck = true;
                        }
                    }
                    if (!audioCodecCheck) { incompatible.Append("\tAudio" + ((i + 1).ToString()).Replace("0", "") + ": format invalid: " + videoFile.Audio[i].Properties["Format"] + "\n"); }
                }

                //Channels Check**********************************************************************************************************************************************************************************************

                //AAC
                if(videoFile.Audio[i].CodecID.Contains("AAC"))
                {
                    if (double.TryParse(videoFile.Audio[i].Properties["Channel(s)"].Replace(" ", "").Replace("channels", "").Replace("channel", ""), out audioChannels) && audioChannels > 2)
                    {
                        incompatible.Append("\tAudio" + ((i + 1).ToString()).Replace("0", "") + ": has " + videoFile.Audio[i].Properties["Channel(s)"] + " must be <= 2\n");
                    }

                    //Audio Bitrate******************************************************************************************************************************
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
                        if (videoFile.Audio[i].Description.Contains("ATMOS")) //ATMOS is at least 8 channels but shows 0 because it's object oriented.
                        {
                            audioBitrate = audioBitrate / 8;
                        }
                        else
                        {
                            audioBitrate = audioBitrate / videoFile.Audio[i].Channels; //Converts to bitrate per channel of audio
                        }

                        if (audioBitrate != 0 & audioBitrate < 32 | audioBitrate > 256)
                        {
                            incompatible.Append("\tAudio" + ((i + 1).ToString()).Replace("0", "") + ": bitrate " + audioBitrate.ToString() + ", AAC tracks must be between 32 & 256 kbps\n");
                        }
                    }
                }
                //AC3
                if (videoFile.Audio[i].CodecID.Contains("AC3") || videoFile.Audio[i].CodecID.Contains("AC-3"))
                {
                    double.TryParse(videoFile.Audio[i].Properties["Channel(s)"].Replace(" ", "").Replace("channels", "").Replace("channel", ""), out audioChannels);
                    if(audioChannels != 2 && audioChannels != 6 && audioChannels != 8 )
                    {
                        incompatible.Append("\tAudio" + ((i + 1).ToString()).Replace("0", "") + ": has " + videoFile.Audio[i].Properties["Channel(s)"] + " must be 2, 6 (5.1), or 8 (7.1)\n");
                    }
                    //Audio Bitrate******************************************************************************************************************************
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
                        if (videoFile.Audio[i].Description.Contains("ATMOS")) //ATMOS is at least 8 channels but shows 0 because it's object oriented.
                        {
                            audioBitrate = audioBitrate / 8;
                        }
                        else
                        {
                            audioBitrate = audioBitrate / videoFile.Audio[i].Channels; //Converts to bitrate per channel of audio
                        }

                        if (audioBitrate != 0 & audioBitrate < 96 | audioBitrate > 768)
                        {
                            incompatible.Append("\tAudio" + ((i + 1).ToString()).Replace("0", "") + ": bitrate " + audioBitrate.ToString() + ", AC3/E-AC3 tracks must be between 96 & 768 kbps\n");
                        }
                    }
                }

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
