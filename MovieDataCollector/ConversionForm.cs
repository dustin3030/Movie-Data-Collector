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
using MediaInfoNET; /* http://teejeetech.blogspot.com/2013/01/mediainfo-wrapper-for-net-projects.html Copyright (c) 2013 Tony George (teejee2008@gmail.com)
                      GNU General Public License version 2.0 (GPLv2)
                      Downloaded Wrapper for returning media info from files.
                      Need to have both the wrapper (MediaInfoNet.dll) and the DLL (MediaInfo.dll) saved in the
                      Application folder (Release or Debug) or it will not work, Add MediaInfoNet.dll as a reference through the project menu*/

namespace MovieDataCollector
{
    public partial class ConversionForm : Form
    {
        //string folderPath = ""; //Contains path for parent directory
        List<string> VideoFilesList = new List<string>(); //Contains File Paths for video files 
        StringBuilder incompatible = new StringBuilder();
        string separator = "========================================================================\n";
        string separator2 = "\n.........................................................................................................................\r\n \r\n";
        List<string> IncompatibilityInfo = new List<string>(); //Contains Incompatibility info for each file listed in VideoFilesList

        //string configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector"; //Writable folder location for config file.
        //string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Config.txt"; //Writable file location for config file.

        //Ending audio bitrate string used in encoder and setting video bitrate buffer and maxrate size.

        /*Values available for conversion*/
        List<string> codecList = new List<string>()
        {
            "AAC (AVC)", //Default
            "Filtered Passthru",
            "AC3"
        };

        List<string> mixdownList = new List<string>()
        {
            "Dolby ProLogic 2", //Default
            "5.1 Audio"
        };

        

        List<string> audioBitrateCapList = new List<string>()
        {
            "192", //Default
            "32",
            "40",
            "48",
            "56",
            "64",
            "80",
            "96", //Highest bitrate supported by Roku
            "112",
            "128",
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
            "10"
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
            populatePresets();
            applyPreset(); // Applies the preset corresponding to the text in the preset combobox.
        }
        private void ApplyConfigDefaults() //Sets encode options to values from file
        {
            /*Preset*/
            presetComboBox.Text = CF.DefaultSettings["ConversionPreset"];

            /*Compatibility Selection*/
            if (CF.DefaultSettings["CompatibilitySelector"] == "Roku"){ compatibilityCombo.SelectedIndex = 0; }
            if (CF.DefaultSettings["CompatibilitySelector"] == "Xbox") { compatibilityCombo.SelectedIndex = 1; }

            /*Audio Settings*/
            audioCodecComboBox.Text = CF.DefaultSettings["AudioCodec"];
            if (CF.DefaultSettings["AudioCodec"] == "Filtered Passthru")
            {
                filteredAACCheck.Visible = true;
                filteredAC3Check.Visible = true;
                filteredDTSCheck.Visible = true;
                passthruFilterLabel.Visible = true;
            }
            if (CF.DefaultSettings["AAC_Passthru"] == "True") { filteredAACCheck.Checked = true; } else { filteredAACCheck.Checked = false; }
            if (CF.DefaultSettings["AC3_Passthru"] == "True") { filteredAC3Check.Checked = true; } else { filteredAC3Check.Checked = false; }
            if (CF.DefaultSettings["DTS_Passthru"] == "True") { filteredDTSCheck.Checked = true; } else { filteredDTSCheck.Checked = false; }

            mixdownComboBox.Text = CF.DefaultSettings["Mixdown"];
            audioBitrateCombo.Text = CF.DefaultSettings["AudioBitrateCap"];
            sampleRateCombo.Text = CF.DefaultSettings["AudioSampleRate"];

            /*Video Settings*/
            encoderSpeedCombo.Text = CF.DefaultSettings["EncoderSpeed"];
            framerateCombo.Text = CF.DefaultSettings["Framerate"];
            frameRateModeCombo.Text = CF.DefaultSettings["FramerateMode"]; //Constant, Peak, Variable
            encoderTuneComboBox.Text = CF.DefaultSettings["EncoderTune"];
            avgBitrateCombo.Text = CF.DefaultSettings["VideoBitrateCap"];
            encoderProfileComboBox.Text = CF.DefaultSettings["EncoderProfile"];
            encoderLevelComboBox.Text = CF.DefaultSettings["EncoderLevel"];

            if (CF.DefaultSettings["Optimize"] == "True") { optimizeStreamingCheckBox.Checked = true; } else { optimizeStreamingCheckBox.Checked = false; }
            if (CF.DefaultSettings["TwoPass"] == "True") { twoPassCheckbox.Checked = true; } else { twoPassCheckbox.Checked = false; }
            if (CF.DefaultSettings["TurboFirstPass"] == "True") { turboCheckBox.Checked = true; } else { turboCheckBox.Checked = false; }


            /*Notification Settings*/
            if (!string.IsNullOrEmpty(CF.DefaultSettings["GmailAccount"])) { usernameBox.Text = CF.DefaultSettings["GmailAccount"]; }
            if (!string.IsNullOrEmpty(CF.DefaultSettings["NotifyAddress"])) { sendToBox.Text = CF.DefaultSettings["NotifyAddress"]; }

        }
        private void returnAllVideoFiles()
        {

            int loopcount = 0; //displays iteration number of the loop. Used to display which file is being processed.
            int fileCount = 0; //displays the number of files in the directory to be processed.

            if (Directory.Exists(CF.DefaultSettings["InputFilePath"]))
            {
                string fileName = ""; //Holds value of processing filename
                filesListBox.Items.Clear();
                outPutTextBox.Clear();
                VideoFilesList.Clear();
                IncompatibilityInfo.Clear();

                nLabelUpdate("Checking and filtering directory for video files ", Color.GreenYellow);

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

                        nLabelUpdate("Processing file " + loopcount.ToString() + " of " + fileCount.ToString() + " - " + file, Color.GreenYellow);
                        
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
                    nLabelUpdate("Listing " + filesListBox.Items.Count.ToString() + " Video Files", Color.GreenYellow);
                }
                catch (Exception e){CustomMessageBox.Show(e.ToString(), 131, 280);}
            }
        }
        private void listAllVideosButton_Click(object sender, EventArgs e)
        {
            returnAllVideoFiles();
        }       
        private void selectDirectory()
        {

            FolderBrowserDialog FBD = new FolderBrowserDialog(); //creates new instance of the FolderBrowserDialog

            if (!string.IsNullOrEmpty(CF.DefaultSettings["InputFilePath"])) //if CF.DefaultSettings["InputFilePath"] contains a path, sets folderBrowserDialog to default to this path
            {
                FBD.SelectedPath = CF.DefaultSettings["InputFilePath"];
            }

            if (FBD.ShowDialog() == DialogResult.OK) //shows folderbrowserdialog, runs addtional code if not cancelled out
            {
                CF.DefaultSettings["InputFilePath"] = FBD.SelectedPath;
                CF.updateDefaults();
                filenameTextBox.Text = CF.DefaultSettings["InputFilePath"];
                returnAllVideoFiles();
            }

            DialogResult = DialogResult.None; //Prevents form from closing...
        }
        private void selectDirectoryButton_Click(object sender, EventArgs e)
        {
            selectDirectory();
        }
        private void invisibleCloseButton_Click(object sender, EventArgs e)
        {
            this.Close(); //Located behind the MediaInfo text box, lower right hand corner
        }       
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Select Directory
            selectDirectory();
        }
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close(); //closes form from tool menu
        }
        private void filesListBox_MouseDoubleClick(object sender, MouseEventArgs e)
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
                        outPutTextBox.Text = videoFile.Info_Text; //output info about selected file to the output box
                        outPutTextBox.SelectionStart = 0;
                        outPutTextBox.ScrollToCaret(); // force current position back to top
                        outPutTextBox.Update();
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
                            outPutTextBox.Text = "INCOMPATIBLE FILE FOUND - " +
                                filesListBox.SelectedItem.ToString() + "\r\n\r\n" +
                                "INCOMPATIBLE FILE ATTRIBUTES LISTED BELOW:\r\n\r\n" +
                                IncompatibilityInfo[filesListBox.SelectedIndex] +
                                separator2 +
                                videoFile.Info_Text;
                            outPutTextBox.SelectionStart = 0;
                            outPutTextBox.ScrollToCaret(); // force current position back to top
                            outPutTextBox.Update();
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

                        CF.updateDefaults();

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

                        CF.updateDefaults();

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
        private void saveInfoButton_Click(object sender, EventArgs e)
        {
            string fileLocation = "";
            string outputBoxText = outPutTextBox.Text;
            outputBoxText = outputBoxText.Replace("\n", "\r\n");
            //SaveFileDialog
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.DefaultExt = "txt";
            if (outPutTextBox.Text.Contains(separator))
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
        private void getQuickInfo(string file, string fileName)
        {
            
            StringBuilder VideoInfo = new StringBuilder();
            StringBuilder AudioInfo = new StringBuilder();

            string videoBitrate = "";
            string FPS = "";
            string videoFrameSize = "";
            string videoFormat = "";

            string audioBitrate = "";
            string audioLanguage = "";
            string audioFormat = "";
            string audioChannels = "";

            //Add filename, bitrate, framerate to the Media Info Box
            MediaFile videoFile = new MediaFile(file);

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

            if (string.IsNullOrEmpty(VideoInfo.ToString()) && string.IsNullOrEmpty(AudioInfo.ToString()))
            {
                outPutTextBox.Text = outPutTextBox.Text + fileName + "\n\t" + "Unable to gather info. File may be corrupt." + separator;
                outPutTextBox.SelectionStart = 0;
                //outPutTextBox.ScrollToCaret(); // force current position back to top
                outPutTextBox.Update();
            }
            else
            {
                outPutTextBox.Text = outPutTextBox.Text + fileName + "\n\t"
                + VideoInfo.ToString() + "\n\t"
                + AudioInfo.ToString().TrimEnd('\t') + separator;
                outPutTextBox.SelectionStart = 0;
                //outPutTextBox.ScrollToCaret(); // force current position back to top
                outPutTextBox.Update();
            }


            VideoInfo.Clear();
            AudioInfo.Clear();
        }
        private void QuickInfobutton_Click(object sender, EventArgs e)
        {

            if (VideoFilesList.Count > 0 & filesListBox.Items.Count > 0)
            {
                tabControl1.SelectedIndex = 0; //Selects Media Info Tab
                outPutTextBox.Clear();
                notificationLabel.Visible = true;
                for (int i = 0; i < VideoFilesList.Count(); i++)
                {
                    nLabelUpdate("Processing file " + (i + 1).ToString() + " of " + VideoFilesList.Count().ToString(), Color.GreenYellow);
                    getQuickInfo(VideoFilesList[i], filesListBox.Items[i].ToString());
                }
            }
        }
        private void detailInfoButton_Click(object sender, EventArgs e)
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
                        MediaFile videoFile = new MediaFile(videoFileName); //return info about selected file
                        outPutTextBox.Text = videoFile.Info_Text; //output info about selected file to the output box
                        outPutTextBox.SelectionStart = 0;
                        outPutTextBox.ScrollToCaret(); // force current position back to top
                        outPutTextBox.Update();
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
                            MediaFile videoFile = new MediaFile(videoFileName); //list incompatible file attributes.
                            outPutTextBox.Text = "INCOMPATIBLE FILE FOUND - " +
                                filesListBox.SelectedItem.ToString() + "\r\n\r\n" +
                                "INCOMPATIBLE FILE ATTRIBUTES LISTED BELOW:\r\n\r\n" +
                                IncompatibilityInfo[filesListBox.SelectedIndex] +
                                separator2 +
                                videoFile.Info_Text;
                            outPutTextBox.SelectionStart = 0;
                            outPutTextBox.ScrollToCaret(); // force current position back to top
                            outPutTextBox.Update();
                        }
                    }
                }
            }
        }


        /*The following methods are for converting video files*/
        private void ConvertSelectedButton_Click(object sender, EventArgs e)
        {
            CF.updateDefaults();
            //Check for location of HandbrakeCLI
            string handBrakeCLILocation = CheckForHandbrakeCLI();
            string totalProcessingTime = "";
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            List<string> Errors = new List<string>(); //Stores error information as files process or fail to.
            string errorString = "";
            Errors.Clear();
            int exitCode = 0; //Exit code for HandbrakeCLI

            if (preConversionChecks()) //Several Checks take place prior to conversion
            {
                string handBrakeCLIString;

                if (filesListBox.SelectedIndex >= 0)
                {
                    FolderBrowserDialog FBD = new FolderBrowserDialog(); //creates new instance of the FolderBrowserDialog
                    FBD.Description = "Select Output Folder for Converted Video Files";

                    if (!string.IsNullOrEmpty(CF.DefaultSettings["OutputFilePath"])) //if folderpath contains a path, sets folderBrowserDialog to default to this path
                    {
                        FBD.SelectedPath = CF.DefaultSettings["OutputFilePath"];
                    }

                    if (FBD.ShowDialog() == DialogResult.OK) //shows folderbrowserdialog, runs addtional code if not cancelled out
                    {
                        CF.DefaultSettings["OutputFilePath"] = FBD.SelectedPath;
                        CF.updateDefaults();

                        nLabelUpdate("Converting File ( " + filesListBox.SelectedItem.ToString() + " )", Color.GreenYellow);

                        DialogResult = DialogResult.None; //Prevents form from closing...

                        try
                        {

                            if (System.IO.File.Exists(VideoFilesList[filesListBox.SelectedIndex])) //Skip file if it has been moved or deleted 
                            {
                                handBrakeCLIString = GenerateConversionString(VideoFilesList[filesListBox.SelectedIndex], filesListBox.SelectedItem.ToString(), FBD.SelectedPath);

                                if (string.IsNullOrEmpty(handBrakeCLIString))
                                {
                                    handBrakeCLIString = "-i " + FBD.SelectedPath + "\\" + filesListBox.SelectedItem.ToString() + " -o "; // Set to defaults let handbrake error out if necessary
                                }

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
                                        //StreamReader SError = conversionProcess.StandardError;
                                        //conversionProcess.PriorityClass = ProcessPriorityClass.High; //starts the process with the highest possible priority.
                                        conversionProcess.WaitForExit();
                                        //string standardError = SError.ReadToEnd();
                                        exitCode = conversionProcess.ExitCode;
                                    }
                                }
                                catch
                                {
                                    exitCode = 4; //Unknown Error
                                }

                            }

                            switch (exitCode)
                            {
                                case 0: //Completed Successfully
                                    break;
                                case 1: //Cancelld
                                    Errors.Add("Error Processing File " + filesListBox.SelectedItem.ToString() + "\r\n\t Exited with code = " + exitCode + " - Cancelled");
                                    break;
                                case 2: //Invalid Input
                                    Errors.Add("Error Processing File " + filesListBox.SelectedItem.ToString() + "\r\n\t Exited with code = " + exitCode + " - Invalid Input");
                                    break;
                                case 3: //Initialization Error
                                    Errors.Add("Error Processing File " + filesListBox.SelectedItem.ToString() + "\r\n\t Exited with code = " + exitCode + " - Initialization Error");
                                    break;
                                case 4: //Unknown Error
                                    Errors.Add("Error Processing File " + filesListBox.SelectedItem.ToString() + "\r\n\t Exited with code = " + exitCode + " - Unknown Error");
                                    break;
                                default:
                                    break;
                            }

                            endTime = DateTime.Now;
                            totalProcessingTime = timeDifference(startTime, endTime);

                            if (Errors.Count > 0)
                            {
                                tabControl1.SelectedIndex = 0; //select output text box
                                foreach (var ErrorLine in Errors)
                                {
                                    errorString += ErrorLine + "\r\n"; //write out errors for user to see
                                }
                                outPutTextBox.Text = "File skipped due to error:\r\n" + errorString;

                                nLabelUpdate("Transcoding of \"" + filesListBox.SelectedItem.ToString() + "\" Failed.", Color.GreenYellow);



                                if (notificationCheck.Checked)
                                {
                                    string username = usernameBox.Text;
                                    string password = passwordBox.Text;
                                    string sendTo = sendToBox.Text;

                                    sendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " Failed with exit code " + exitCode.ToString() + ".");
                                }

                            }
                            else //No errors were thrown by handbrake cli
                            {
                                outPutTextBox.Text = ""; //Clears Output Box on successful Encode
                                nLabelUpdate("Transcoding of \"" + filesListBox.SelectedItem.ToString() + "\" completed in " + totalProcessingTime, Color.GreenYellow);

                                if (notificationCheck.Checked)
                                {
                                    string username = usernameBox.Text;
                                    string password = passwordBox.Text;
                                    string sendTo = sendToBox.Text;

                                    //Send notification that transcoding is complete
                                    sendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. \r\n The file was processed in " + totalProcessingTime);
                                }
                            }

                        }
                        catch
                        {
                            nLabelUpdate("Error Gathering File Info for " + filesListBox.SelectedItem.ToString() + " File May Be Corrupt.", Color.GreenYellow);



                            if (notificationCheck.Checked)
                            {
                                string username = usernameBox.Text;
                                string password = passwordBox.Text;
                                string sendTo = sendToBox.Text;

                                //Send notification that the transcoding is complete
                                sendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " failed. \r\n The file may be corrupt");
                            }
                        }
                    }
                }
                else
                {
                    //Clear notification label
                    nLabelUpdate("", Color.GreenYellow);
                }
            }
        }
        private void ConvertAllButton_Click(object sender, EventArgs e)
        {
            CF.updateDefaults();

            //Check for location of HandbrakeCLI
            string handBrakeCLILocation = CheckForHandbrakeCLI();
            DateTime startTime = DateTime.Now;
            DateTime endTime;
            string totalProcessingTime = "";
            List<string> Errors = new List<string>();
            string errorString = "";
            Errors.Clear();
            int exitCode = 0; //HandbrakeCLI Exit Code 0=Exited Normally, 1=Cancelled, 2=Invalid Input, 3=Initalization Error, 4=Unknown Error

            if (preConversionChecks()) //If handbrake is found continue
            {
                string handBrakeCLIString;
                if (filesListBox.Items.Count > 0) //FilesListBox contains files
                {
                    FolderBrowserDialog FBD = new FolderBrowserDialog(); //creates new instance of the FolderBrowserDialog
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
                            //Display which file is being converted
                            nLabelUpdate("Converting File " + (i + 1).ToString() + " of " + VideoFilesList.Count.ToString() + " ( " + filesListBox.Items[i].ToString() + " )", Color.GreenYellow);

                            DialogResult = DialogResult.None; //Prevents form from closing...

                            handBrakeCLIString = GenerateConversionString(VideoFilesList[i], filesListBox.Items[i].ToString(), FBD.SelectedPath);

                            if (string.IsNullOrEmpty(handBrakeCLIString))
                            {
                                handBrakeCLIString = ""; // Set to defaults let handbrake error out if necessary
                            }

                            try //Check for errors and continue processing 
                            {
                                //Launch command line object to pass the commands to
                                if (System.IO.File.Exists(VideoFilesList[i])) //Skip file if it has been moved or deleted 
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
                                            //StreamReader SError = conversionProcess.StandardError;
                                            conversionProcess.PriorityClass = ProcessPriorityClass.High; //starts the process with the highest possible priority.
                                            conversionProcess.WaitForExit();
                                            //string standardError = SError.ReadToEnd();
                                            exitCode = conversionProcess.ExitCode;
                                        }
                                    }
                                    catch
                                    {
                                        exitCode = 4; //Unknown Error
                                    }
                                    
                                    
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

                        endTime = DateTime.Now;
                        totalProcessingTime = timeDifference(startTime, endTime);

                        if (Errors.Count > 0)
                        {
                            tabControl1.SelectedIndex = 0;
                            foreach (var ErrorLine in Errors)
                            {
                                errorString += ErrorLine + "\r\n";
                            }
                            outPutTextBox.Text = "Files skipped due to error:\r\n" + errorString;

                            if (VideoFilesList.Count == 1) { nLabelUpdate("The transcoding que initiated " + startTime.ToString() + " failed. HandbrakeCLI exited with code " + exitCode.ToString(), Color.GreenYellow); }
                            if (VideoFilesList.Count > 1) { nLabelUpdate("The transcoding que initiated " + startTime.ToString() + " is now complete. " + (VideoFilesList.Count() - Errors.Count()).ToString() + " of " + VideoFilesList.Count().ToString() + " files processed successfully in " + totalProcessingTime, Color.GreenYellow); }




                            if (notificationCheck.Checked)
                            {
                                string username = usernameBox.Text;
                                string password = passwordBox.Text;
                                string sendTo = sendToBox.Text;

                                if (VideoFilesList.Count == 1) { sendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " failed. HandbrakeCLI exited with code" + exitCode.ToString()); }
                                if (VideoFilesList.Count > 1) { sendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. " + (VideoFilesList.Count() - Errors.Count()).ToString() + " of " + VideoFilesList.Count().ToString() + " files processed successfully in " + totalProcessingTime); }
                            }
                        }
                        else
                        {
                            outPutTextBox.Text = ""; //Clears Output Box on successful Encode

                            if (VideoFilesList.Count == 1) { nLabelUpdate("The transcoding que initiated " + startTime.ToString() + " is now complete. The file was processed in " + totalProcessingTime, Color.GreenYellow); }
                            if (VideoFilesList.Count > 1) { nLabelUpdate("The transcoding que initiated " + startTime.ToString() + " is now complete. " + VideoFilesList.Count().ToString() + " files were processed in " + totalProcessingTime, Color.GreenYellow); }

                            if (notificationCheck.Checked)
                            {
                                string username = usernameBox.Text;
                                string password = passwordBox.Text;
                                string sendTo = sendToBox.Text;

                                if (VideoFilesList.Count == 1) { sendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. The file was processed in " + totalProcessingTime); }
                                if (VideoFilesList.Count > 1) { sendNotification(username, password, sendTo, "Movie Data Collector Notification", "The transcoding que initiated " + startTime.ToString() + " is now complete. " + VideoFilesList.Count().ToString() + " files were processed in " + totalProcessingTime); }
                            }
                        }
                    }
                }
                else
                {
                    nLabelUpdate("", Color.GreenYellow);
                }
            }
        }
        private bool preConversionChecks()
        {
            bool checksPassed = true;
            nLabelUpdate("Checking for existence of Handbrake CLI", Color.GreenYellow);
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
                        nLabelUpdate(" No Passthru filter Selected!", Color.Red);
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
            audioTrack = int.Parse(Program.GeneralParser(AudioString, "--audio ", " "));

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
            crop = "--crop 0:0:0:0 --auto-anamorphic --modulus 2 "; //Forces 0 crop, Changed from loose-anamorphic to --auto-anamorphic

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
                                        nLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
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
                                        nLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
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
                                        nLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 60 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    default:
                        nLabelUpdate("Framerate ignored, preserving source rate.", Color.Red);
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
                double audioBitrate = 0;

                /*Encopts***********************************************************************************************************************************************************************************************/
                //Need to adjust for number of channels of audio, currently it is the value for mono not stereo which is typical.

                double.TryParse(avgBitrateCombo.Text, out avgBitrateCap);
                avgBitrateCap = avgBitrateCap * 1000; //This changes the value in the dropdown from Mbps to Kbps
                videoBitrate = avgBitrateCap;
                double.TryParse(audioBitrateCombo.Text, out audioBitrate);

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
                                        nLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
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
                                        nLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
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
                                        nLabelUpdate("Variable Framerate Mode is not compatible with Roku players. Changed to Peak Framerate Mode.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
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
                                nLabelUpdate("Variable Framerate Mode ignores framerate setting.", Color.Red);
                                outputFrameRate = " --vfr";
                                break;
                            default: //Peak
                                outputFrameRate = "--rate 60 --pfr "; //Peak Framerate
                                break;
                        }
                        break;
                    default:
                        nLabelUpdate("Framerate ignored, preserving source rate.", Color.Red);
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

            return outputEncoder + outputEncoderSpeed + outputEncoderTune + outputEncopts + outputEncoderProfile + outputEncoderLevel + outputVideoBitrate + outputTwoPass + outputTurbo + outputFrameRate;
        }
        private string AudioConversionString(MediaFile videoFile)
        {
            //Users selected variables
            double userSelectedBitrate = 0; //Bitrate for audio is per channel so stereo audio of 96 bitrate would actually be 96 * 2 = 192

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
                                if ((videoFile.Audio[i].Bitrate / 2) > maxBitrate) // Assume at least stereo
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
                            if ((videoFile.Audio[i].Bitrate / 2) > maxBitrate) // Assume at least stereo
                            {
                                maxBitrate = videoFile.Audio[i].Bitrate / 2; //Get the per channel bitrate
                                audioTrackNumber = i; //Mark the audio track that has the highest bitrate
                            }
                        }
                    }
                }
                outputAudioTrack = "--audio " + (audioTrackNumber + 1).ToString() + " ";

                /*Samplerate***********************************************************************************************************************************************************************************************/
                if (string.IsNullOrEmpty(sampleRateCombo.Text))
                {
                    if (videoFile.Audio[audioTrackNumber].SamplingRate / 1000 >= 48)
                    {
                        outputSampleRate = "--arate 48 "; //Roku Compatible high rate
                    }
                    else if(videoFile.Audio[audioTrackNumber].SamplingRate / 1000 == 44.1)
                    {
                        outputSampleRate = "--arate 44.1 "; //Roku compatible low rate
                    }
                    else
                    {
                        outputSampleRate = "--arate 48"; //Default to 48
                    }
                }
                else //Use user selected samplerate
                {
                    outputSampleRate = "--arate " + sampleRateCombo.Text;
                }

                /*Bitrate***********************************************************************************************************************************************************************************************/

                //Determine per channel Bitrate selected by user
                try { userSelectedBitrate = int.Parse(audioBitrateCombo.Text); } catch { userSelectedBitrate = 96; }//default value is 96

                if (videoFile.Audio[audioTrackNumber].Bitrate != 0)
                {
                    //Determine per channel Bitrate of file
                    bitrateOfFile = videoFile.Audio[audioTrackNumber].Bitrate / videoFile.Audio[audioTrackNumber].Channels;
                }
                else if (videoFile.Audio[audioTrackNumber].Properties.ContainsKey("Bit rate"))
                {
                    try { double.TryParse(videoFile.Audio[audioTrackNumber].Properties["Bit rate"].Replace(" ", "").Replace("Kbps", ""), out bitrateOfFile); }
                    catch { bitrateOfFile = 0; }

                    bitrateOfFile = bitrateOfFile / videoFile.Audio[audioTrackNumber].Channels;
                }


                //If per channel Bitrate of file is < Selected Bitrate use the file's bitrate.
                if ((bitrateOfFile < userSelectedBitrate) && (bitrateOfFile > 0)) { userSelectedBitrate = bitrateOfFile; }

                /*Fallback***********************************************************************************************************************************************************************************************/
                switch (audioCodecComboBox.Text)
                {

                    case "Filtered Passthru":
                        if (videoFile.Audio[audioTrackNumber].Properties.ContainsKey("Format"))
                        {
                            switch (videoFile.Audio[audioTrackNumber].Properties["Format"])
                            {
                                case "DTS":
                                    outputFallBack = "--audio-fallback ac3 "; //No DTS encoder option
                                    break;
                                case "AC-3":
                                    outputFallBack = "--audio-fallback ac3 ";
                                    break;
                                case "AAC":
                                    outputFallBack = "--audio-fallback av_aac ";
                                    break;
                                default:
                                    break;
                            }
                        }
                        else //Default fallback is av_aac
                        {
                            outputFallBack = "--audio-fallback av_aac ";
                        }
                        break;
                    default:
                        outputFallBack = "--audio-fallback av_aac ";
                        break;
                }
                /*Mixdown***********************************************************************************************************************************************************************************************/

                //The options are auto, mono, stereo, dpl1 (Dolby Surround), dpl2 (Dolby ProLogic? 2), or 6ch (5.1).
                switch (audioCodecComboBox.Text)
                {
                    case "Filtered Passthru":
                        switch (videoFile.Audio[audioTrackNumber].Channels)
                        {
                            case 1:
                                outputMixdown = "--mixdown mono ";
                                outputBitrate = "--ab " + (userSelectedBitrate * 1).ToString() + " ";
                                break;
                            case 2:
                                outputMixdown = "--mixdown dpl2  ";
                                outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                                break;
                            case 3:
                                outputMixdown = "--mixdown dpl2  ";
                                outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                                break;
                            case 4:
                                outputMixdown = "--mixdown dpl2  ";
                                outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                                break;
                            case 5:
                                outputMixdown = "--mixdown dpl2  ";
                                outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                                break;
                            case 6:
                                outputMixdown = "--mixdown 6ch  ";
                                outputBitrate = "--ab " + (userSelectedBitrate * 6).ToString() + " ";
                                break;
                            default:
                                outputMixdown = "--mixdown dpl2 ";
                                outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                                break;
                        }
                        break;
                    case "AAC (AVC)": //only good up to Dolby Pro Logic 2 (2 channel)
                        switch (videoFile.Audio[audioTrackNumber].Channels)
                        {
                            case 1:
                                outputMixdown = "--mixdown mono ";
                                outputBitrate = "--ab " + (userSelectedBitrate * 1).ToString() + " ";
                                break;
                            default:
                                outputMixdown = "--mixdown dpl2 ";
                                outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                                break;
                        }
                        break;
                    default:
                        switch (mixdownComboBox.Text)
                        {
                            case "Dolby ProLogic 2":
                                outputMixdown = "--mixdown dpl2 ";
                                outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                                break;
                            default: //5.1 Audio
                                switch (videoFile.Audio[audioTrackNumber].Channels)
                                {
                                    case 1:
                                        outputMixdown = "--mixdown mono ";
                                        outputBitrate = "--ab " + (userSelectedBitrate * 1).ToString() + " ";
                                        break;
                                    case 2:
                                        outputMixdown = "--mixdown dpl2 ";
                                        outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                                        break;
                                    case 3:
                                        outputMixdown = "--mixdown dpl2 ";
                                        outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                                        break;
                                    case 4:
                                        outputMixdown = "--mixdown dpl2 ";
                                        outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                                        break;
                                    case 5:
                                        outputMixdown = "--mixdown dpl2 ";
                                        outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                                        break;
                                    case 6:
                                        outputMixdown = "--mixdown 6ch ";
                                        outputBitrate = "--ab " + (userSelectedBitrate * 6).ToString() + " ";
                                        break;
                                    case 7:
                                        outputMixdown = "--mixdown 6ch ";
                                        outputBitrate = "--ab " + (userSelectedBitrate * 6).ToString() + " ";
                                        break;
                                    case 8:
                                        outputMixdown = "--mixdown 6ch ";
                                        outputBitrate = "--ab " + (userSelectedBitrate * 6).ToString() + " ";
                                        break;
                                    default:
                                        outputMixdown = "--mixdown 6ch ";
                                        outputBitrate = "--ab " + (userSelectedBitrate * 6).ToString() + " ";
                                        break;
                                }
                                break;
                        }
                        break;
                }

            }
            else //Source Unreadable - Set variables derived from file to user selected values
            {
                //bitrateOfFile, audioTrackNumber, maxBitrate
                outputAudioTrack = "--audio 1 "; //Since no track can be read in, select the first audio track.
                try { userSelectedBitrate = int.Parse(audioBitrateCombo.Text); } catch { userSelectedBitrate = 96; }//default value is 96
                maxBitrate = userSelectedBitrate;

                /*Samplerate***********************************************************************************************************************************************************************************************/
                outputSampleRate = "--arate " + sampleRateCombo.Text;

                /*Fallback***********************************************************************************************************************************************************************************************/

                outputFallBack = "--audio-fallback av_aac ";

            }
            /*Mixdown***********************************************************************************************************************************************************************************************/

            //The options are auto, mono, stereo, dpl1 (Dolby Surround), dpl2 (Dolby ProLogic? 2), or 6ch (5.1).
            switch (audioCodecComboBox.Text)
            {
                case "Filtered Passthru":
                    outputMixdown = "";
                    outputBitrate = "" + (userSelectedBitrate * 2).ToString() + " ";
                    break;
                case "AAC (AVC)": //only good up to Dolby Pro Logic 2 (2 channel)
                    outputMixdown = "--mixdown dpl2 ";
                    outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                    break;
                default:
                    switch (mixdownComboBox.Text)
                    {
                        case "Dolby ProLogic 2":
                            outputMixdown = "--mixdown dpl2 ";
                            outputBitrate = "--ab " + (userSelectedBitrate * 2).ToString() + " ";
                            break;
                        default: //5.1 Audio
                            outputMixdown = "--mixdown 6ch ";
                            outputBitrate = "--ab " + (userSelectedBitrate * 6).ToString() + " ";
                            break;
                    }
                    break;
            }
            /*Passthru Mask***********************************************************************************************************************************************************************************************/

            if (audioCodecComboBox.Text == "Filtered Passthru")
            {
                outputAudioPassthruMask = "--audio-copy-mask ";
                if (filteredAACCheck.Checked) { outputAudioPassthruMask += "aac"; }

                if (filteredAC3Check.Checked)
                {
                    if (outputAudioPassthruMask.Contains("aac")) { outputAudioPassthruMask += ",ac3"; }
                    else { outputAudioPassthruMask += "ac3"; }
                }
                if (filteredDTSCheck.Checked)
                {
                    if (outputAudioPassthruMask.Contains("aac") || outputAudioPassthruMask.Contains("ac3")) { outputAudioPassthruMask += ",dts"; }
                    else { outputAudioPassthruMask += "dts"; }
                }
                outputAudioPassthruMask += " ";
            }
            else { outputAudioPassthruMask = ""; }



            /*Encoder***********************************************************************************************************************************************************************************************/
            switch (audioCodecComboBox.Text)
            {
                case "AAC (AVC)":
                    outputEncoder = "--aencoder av_aac ";
                    break;
                case "AC3":
                    outputEncoder = "--aencoder ac3 ";
                    break;
                case "Filtered Passthru":
                    outputEncoder = "--aencoder copy ";
                    break;
                default:
                    outputEncoder = "--aencoder av_aac ";
                    break;
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
            string largeFileSize = "";
            string webOptimization = "";
            /*Input File***********************************************************************************************************************************************************************************************/
            if (optimizeStreamingCheckBox.Checked) { webOptimization = "--optimize "; } //Optimize mp4 files for HTTP streaming ("fast start")

            /*Input File***********************************************************************************************************************************************************************************************/
            string inputFile = "--input \"" + filepath + "\" --title 1 --angle 1 "; //Input file location
            inputFileExt = "." + Tokens[Tokens.Count() - 1]; //should be extension


            /*Output File***********************************************************************************************************************************************************************************************/
            largeFileSize = ""; // "--large-file "; //Create 64-bit mp4 files that can hold more than 4 GB of data. Note: breaks pre-iOS iPod compatibility. Only use if file will be larger than 4GB
            if (outputLargerThan4Gb) { largeFileSize = "--large-file "; } else { largeFileSize = ""; }

            //Check if output filename already exists, if so add a number to the tail end of the file name

            if (!System.IO.File.Exists(outputPath + "\\" + filename.Replace(inputFileExt, outputFileExt)))
            {
                outputFile = "--output \"" + outputPath + "\\" + filename.Replace(inputFileExt, outputFileExt) + "\" ";
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

                outputFile = "--output \"" + newName + "\" " + containerFormat + chapterMarkers + largeFileSize + webOptimization; //Location to output converted file
            }

            return inputFile + outputFile;
        }



        /*The following methods are to ensure user input is valid*/
        //Video Comboboxes Index Change
        private void avgBitrateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["VideoBitrateCap"] = avgBitrateCombo.Text;
        }
        private void framerateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["Framerate"] = framerateCombo.Text;

            switch (framerateCombo.Text)
            {
                case "Same As Source":
                    nLabelUpdate("Framerates > 30 are not ROKU Compliant!", Color.Red);
                    notificationLabel.Visible = true;
                    break;
                case "50":
                    nLabelUpdate("Framerates > 30 are not ROKU Compliant!", Color.Red);
                    notificationLabel.Visible = true;
                    break;
                case "59.94":
                    nLabelUpdate("Framerates > 30 are not ROKU Compliant!", Color.Red);
                    notificationLabel.Visible = true;
                    break;
                case "60":
                    nLabelUpdate("Framerates > 30 are not ROKU Compliant!", Color.Red);
                    notificationLabel.Visible = true;
                    break;
                default:
                    notificationLabel.Visible = true;
                    break;
            }
        }
        private void frameRateModeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["FramerateMode"] = frameRateModeCombo.Text;
        }
        private void encoderLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["EncoderLevel"] = encoderLevelComboBox.Text;
        }
        private void encoderProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["EncoderProfile"] = encoderProfileComboBox.Text;
        }
        private void encoderSpeedCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["EncoderSpeed"] = encoderSpeedCombo.Text;
        }  
        private void encoderTuneComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["EncoderTune"] = encoderTuneComboBox.Text;
        }
        

        //Video Checkboxes
        private void optimizeStreamingCheckBox_CheckedChanged(object sender, EventArgs e)
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
        private void turboCheckBox_CheckedChanged(object sender, EventArgs e)
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
        private void twoPassCheckbox_CheckStateChanged(object sender, EventArgs e)
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
        

        //Video ComboBoxes Leave
        private void avgBitrateCombo_Leave(object sender, EventArgs e)
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
        private void framerateCombo_Leave(object sender, EventArgs e)
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
        private void frameRateModeCombo_Leave(object sender, EventArgs e)
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
        private void encoderLevelComboBox_Leave(object sender, EventArgs e)
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
        private void encoderProfileComboBox_Leave(object sender, EventArgs e)
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
        private void encoderTuneComboBox_Leave(object sender, EventArgs e)
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
        private void encoderSpeedCombo_Leave(object sender, EventArgs e)
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


        //Video ComboBoxes Text Changed
        private void avgBitrateCombo_TextChanged(object sender, EventArgs e)
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
        private void framerateCombo_TextChanged(object sender, EventArgs e)
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
        private void frameRateModeCombo_TextChanged(object sender, EventArgs e)
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
        private void encoderLevelComboBox_TextChanged(object sender, EventArgs e)
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
        private void encoderProfileComboBox_TextChanged(object sender, EventArgs e)
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
        private void encoderTuneComboBox_TextChanged(object sender, EventArgs e)
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
        private void encoderSpeedCombo_TextUpdate(object sender, EventArgs e)
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
                    filteredDTSCheck.Visible = true;
                    passthruFilterLabel.Visible = true;
                    break;
                case "AAC (AVC)":
                    filteredAACCheck.Visible = false;
                    filteredAC3Check.Visible = false;
                    filteredDTSCheck.Visible = false;
                    passthruFilterLabel.Visible = false;
                    filteredAACCheck.Checked = false;
                    filteredAC3Check.Checked = false;
                    filteredDTSCheck.Checked = false;
                    mixdownComboBox.Text = "Dolby ProLogic 2"; //AAC can only mix down to Prologic or Mono
                    break;
                case "AC3":
                    filteredAACCheck.Visible = false;
                    filteredAC3Check.Visible = false;
                    filteredDTSCheck.Visible = false;
                    filteredAACCheck.Checked = false;
                    filteredAC3Check.Checked = false;
                    filteredDTSCheck.Checked = false;
                    passthruFilterLabel.Visible = false;
                    break;
                default:
                    filteredAACCheck.Visible = false;
                    filteredAC3Check.Visible = false;
                    filteredDTSCheck.Visible = false;
                    filteredAACCheck.Checked = false;
                    filteredAC3Check.Checked = false;
                    filteredDTSCheck.Checked = false;
                    passthruFilterLabel.Visible = false;
                    break;
            }
        }
        private void MixdownComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            //set default in dictionary
            CF.DefaultSettings["Mixdown"] = mixdownComboBox.Text;

            if(audioCodecComboBox.Text == "AAC (AVC)" && mixdownComboBox.Text != "Dolby ProLogic 2")
            {
                nLabelUpdate("The AAC (AVC) Codec can only mixdown to \"Dolby ProLogic 2\"", Color.Red);
                mixdownComboBox.Text = "Dolby ProLogic 2";
            }
            
        }
        private void sampleRateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["AudioSampleRate"] = sampleRateCombo.Text;
        }
        private void audioBitrateCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Update default in dictionary
            CF.DefaultSettings["AudioBitrateCap"] = audioBitrateCombo.Text;
        }

        //Audio Combobox Text Changed
        private void audioCodecComboBox_TextChanged(object sender, EventArgs e)
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
        private void mixdownComboBox_TextChanged(object sender, EventArgs e)
        {
            if (!mixdownList.Contains(mixdownComboBox.Text))
            {
                mixdownComboBox.Text = mixdownList[0];

                //Update default in dictionary
                CF.DefaultSettings[""] = mixdownList[0];
            }
            else
            {
                //Update default in dictionary
                CF.DefaultSettings[""] = mixdownComboBox.Text;
            }
        }
        private void audioBitrateCombo_TextChanged(object sender, EventArgs e)
        {
            double ABitrate;

            try { double.TryParse(audioBitrateCombo.Text, out ABitrate); }
            catch { ABitrate = 192; }

            if (ABitrate > 256) { ABitrate = 256; }
            if (ABitrate < 64) { ABitrate = 64; }

            audioBitrateCombo.Text = ABitrate.ToString();

            CF.DefaultSettings["AudioBitrateCap"] = ABitrate.ToString();
        }
        private void sampleRateCombo_TextChanged(object sender, EventArgs e)
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
        private void MixdownComboBox_Leave(object sender, EventArgs e)
        {
            //Need to add code again
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
        private void sampleRateCombo_Leave(object sender, EventArgs e)
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

        //Audio Checkboxes
        private void filteredAACCheck_CheckedChanged(object sender, EventArgs e)
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
        private void filteredAC3Check_CheckedChanged(object sender, EventArgs e)
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
        private void filteredDTSCheck_CheckedChanged(object sender, EventArgs e)
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


        /*These methods are used to send notifications to the user*/
        private void sendNotification(string userName, string password, string sendTo, string subject, string message)
        {
            //If this doesnt work it's because the gmail account needs to allow less secure apps access to send email
            //https://support.google.com/accounts/answer/6010255?hl=en
            if (notificationCheck.Checked) //Ensures that a notification doesn't try to send unless the user intends it.
            {
                if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(sendTo))
                {
                    nLabelUpdate("Attempting to send notification", Color.GreenYellow);

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
                        //CustomMessageBox.Show("Notification Sent!", 120, 230, "Notification Message");
                        nLabelUpdate("Notification sent to : " + sendTo, Color.GreenYellow);


                    }
                    catch (Exception ex)
                    {
                        CustomMessageBox.Show("You may have to enable less secure apps access to gmail. See https://support.google.com/accounts/answer/6010255?hl=en" + "/r/n" + ex.ToString(), 600, 300);
                        nLabelUpdate("Notification failed to send.", Color.Red);
                    }
                }
                else
                {
                    nLabelUpdate("Missing Notification Parameters", Color.Red);
                }
            }
        }
        private void testNotificationButton_Click(object sender, EventArgs e)
        {
            string username = usernameBox.Text;
            string password = passwordBox.Text;
            string sendTo = sendToBox.Text;

            sendNotification(username, password, sendTo, "Test Notification", "Notification Test from Movie Data Collector");

        }
        private void notificationCheck_CheckedChanged(object sender, EventArgs e)
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
        private string timeDifference(DateTime start, DateTime end)
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
        private void nLabelUpdate(string notificationText, Color color)
        {
            notificationLabel.ForeColor = color;
            notificationLabel.Text = notificationText;
            notificationLabel.Invalidate();
            notificationLabel.Update();
            notificationLabel.Visible = true;
        }
        private void usernameBox_Leave(object sender, EventArgs e)
        {
            CF.DefaultSettings["GmailAccount"] = usernameBox.Text;
        }
        private void sendToBox_Leave(object sender, EventArgs e)
        {
            CF.DefaultSettings["NotifyAddress"] = sendToBox.Text;
        }


        /*The following methods are to check that video files are compatible with a certain device*/
        private void listIncompatibleButton_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 0; //Sets the tabcontrol to show data being presented on the page. 
            int loopcount = 0; //for progress bar value
            int fileCount = 0;

            if (compatibilityCombo.SelectedIndex > -1) // Selection rather than a typed input
            {
                if (Directory.Exists(CF.DefaultSettings["InputFilePath"]))
                {
                    nLabelUpdate("Checking and filtering directory for video files ", Color.GreenYellow);

                    string fileName = "";
                    filesListBox.Items.Clear();
                    VideoFilesList.Clear();
                    IncompatibilityInfo.Clear();
                    outPutTextBox.Clear();


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

                            nLabelUpdate("Analyzing file " + loopcount.ToString() + " of " + fileCount.ToString() + " - " + file, Color.GreenYellow);

                            if (compatibilityCombo.SelectedIndex == 1)
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

                                    outPutTextBox.Text = "\t\t\t\tINVALID ATTRIBUTES:\n\n" + outPutText.ToString();
                                }

                                nLabelUpdate("Listing " + filesListBox.Items.Count.ToString() + " Files Incompatible with Xbox360", Color.GreenYellow);
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
                                    outPutTextBox.Text = "\t\t\t\tINVALID ATTRIBUTES:\n\n" + outPutText.ToString();
                                }
                                nLabelUpdate("Listing " + filesListBox.Items.Count.ToString() + " Files Incompatible with Roku", Color.GreenYellow);
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
                nLabelUpdate("Please select a compatibility option", Color.Red);
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

            nLabelUpdate("Checking if " + fileName + " is compatible with Roku players.", Color.GreenYellow);

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

                    audioBitrate = audioBitrate / videoFile.Audio[i].Channels; //Converts to bitrate per channel of audio
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

            nLabelUpdate("Checking if " + fileName + " is compatible with Xbox 360.", Color.GreenYellow);

            switch (videoFile.Extension)
            {
                case ".avi":
                    return xboxCompatibilityforAVIDIVX(videoFile, fileSize);
                case ".divx":
                    return xboxCompatibilityforAVIDIVX(videoFile, fileSize);
                case ".mov":
                    return xboxCompatibilityforMP4MOV(videoFile, fileSize);
                case ".mp4":
                    return xboxCompatibilityforMP4MOV(videoFile, fileSize);
                case ".m4v":
                    return xboxCompatibilityforMP4MOV(videoFile, fileSize);
                case ".mp4v":
                    return xboxCompatibilityforMP4MOV(videoFile, fileSize);
                case ".wmv":
                    return xboxCompatibilityforWMV(videoFile, fileSize);
                default:
                    incompatible.Append("\tExtension " + videoFile.Extension + " must be AVI, DIVX, M4V, MP4, MP4V, MOV or WMV\n");
                    return incompatible.ToString();
            }
        }
        private string xboxCompatibilityforMP4MOV(MediaFile videoFile, decimal fileSize)
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
        private string xboxCompatibilityforAVIDIVX(MediaFile videoFile, decimal fileSize)
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
        private string xboxCompatibilityforWMV(MediaFile videoFile, decimal fileSize)
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
        private void populatePresets()
        {
            presetComboBox.Items.Clear();
            presetComboBox.Items.Add(""); //Add blank row at top

            for (int i = 0; i < PF.presetNames.Count(); i++)
            {
                presetComboBox.Items.Add(PF.presetNames[i]);
            }
        }
        private void presetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            applyPreset();
            CF.DefaultSettings["ConversionPreset"] = presetComboBox.Text;
            CF.updateDefaults();
        }
        private void presetComboBox_Leave(object sender, EventArgs e)
        {
            CF.DefaultSettings["ConversionPreset"] = presetComboBox.Text;
            CF.updateDefaults();
        }
        private void addPresetButton_Click(object sender, EventArgs e)
        {
            //Check that there is a preset name
            if(!string.IsNullOrEmpty(presetComboBox.Text))
            {
                    Dictionary<string, string> NewPreset = new Dictionary<string, string>();
                    NewPreset.Add("Name", presetComboBox.Text);
                    NewPreset.Add("AudioCodec", audioCodecComboBox.Text);
                    NewPreset.Add("AudioMixdown", mixdownComboBox.Text);
                    NewPreset.Add("AudioSampleRate", sampleRateCombo.Text);

                    if (filteredAACCheck.Checked) { NewPreset.Add("FilteredAACCheck", "true"); } else { NewPreset.Add("FilteredAACCheck", "false"); }
                    if (filteredAC3Check.Checked) { NewPreset.Add("FilteredAC3Check", "true"); } else { NewPreset.Add("FilteredAC3Check", "false"); }
                    if (filteredDTSCheck.Checked) { NewPreset.Add("FilteredDTSCheck", "true"); } else { NewPreset.Add("FilteredDTSCheck", "false"); }

                    NewPreset.Add("AudioBitrate", audioBitrateCombo.Text);
                    NewPreset.Add("EncoderSpeed", encoderSpeedCombo.Text);
                    NewPreset.Add("FrameRateMode", frameRateModeCombo.Text);
                    NewPreset.Add("FrameRate", framerateCombo.Text);
                    NewPreset.Add("EncoderTune", encoderTuneComboBox.Text);
                    NewPreset.Add("VideoBitrate", avgBitrateCombo.Text);
                    NewPreset.Add("EncoderProfile", encoderProfileComboBox.Text);
                    NewPreset.Add("EncoderLevel", encoderLevelComboBox.Text);

                    if (optimizeStreamingCheckBox.Checked) { NewPreset.Add("Optimize", "true"); } else { NewPreset.Add("Optimize", "false"); }
                    if (twoPassCheckbox.Checked) { NewPreset.Add("TwoPass", "true"); } else { NewPreset.Add("TwoPass", "false"); }
                    if (turboCheckBox.Checked) { NewPreset.Add("TurboFirstPass", "true"); } else { NewPreset.Add("TurboFirstPass", "false"); }

                    PF.AddPreset(NewPreset);

                    //re-populate presets combobox
                    presetComboBox.Items.Clear();
                    presetComboBox.Items.Add(""); //Add in blank at the top
                    for (int i = 0; i < PF.presetNames.Count(); i++)
                    {
                        presetComboBox.Items.Add(PF.presetNames[i]);
                    }

                    nLabelUpdate("Added Preset: " + presetComboBox.Text, Color.GreenYellow);
            }
            else
            {
                nLabelUpdate("No name present. Preset must have a name.", Color.Red);
            }

        }
        private void removePresetButton_Click(object sender, EventArgs e)
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
                nLabelUpdate("Removed preset: " + presetComboBox.Text,Color.GreenYellow);

                presetComboBox.Text = "";
            }
        }
        private void applyPreset()
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

                    if (PF.PresetList[index]["FilteredAACCheck"] == "true") { filteredAACCheck.Checked = true; } else { filteredAACCheck.Checked = false; }
                    if (PF.PresetList[index]["FilteredAC3Check"] == "true") { filteredAC3Check.Checked = true; } else { filteredAC3Check.Checked = false; }
                    if (PF.PresetList[index]["FilteredDTSCheck"] == "true") { filteredDTSCheck.Checked = true; } else { filteredDTSCheck.Checked = false; }

                    audioBitrateCombo.Text = PF.PresetList[index]["AudioBitrate"];
                    encoderSpeedCombo.Text = PF.PresetList[index]["EncoderSpeed"];
                    frameRateModeCombo.Text = PF.PresetList[index]["FrameRateMode"];
                    framerateCombo.Text = PF.PresetList[index]["FrameRate"];
                    encoderTuneComboBox.Text = PF.PresetList[index]["EncoderTune"];
                    avgBitrateCombo.Text = PF.PresetList[index]["VideoBitrate"];
                    encoderProfileComboBox.Text = PF.PresetList[index]["EncoderProfile"];
                    encoderLevelComboBox.Text = PF.PresetList[index]["EncoderLevel"];

                    if (PF.PresetList[index]["Optimize"] == "true") { optimizeStreamingCheckBox.Checked = true; } else { optimizeStreamingCheckBox.Checked = false; }
                    if (PF.PresetList[index]["TwoPass"] == "true") { twoPassCheckbox.Checked = true; } else { twoPassCheckbox.Checked = false; }
                    if (PF.PresetList[index]["TurboFirstPass"] == "true") { turboCheckBox.Checked = true; } else { turboCheckBox.Checked = false; }
                }
            }
            
        }

        private void compatibilityCombo_SelectedIndexChanged(object sender, EventArgs e)
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
    }
}
