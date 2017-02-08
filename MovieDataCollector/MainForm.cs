using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieDataCollector
{
    public partial class MainForm : Form
    {
        string configDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector"; //Writable folder location for config file.
        string configPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Movie Data Collector\\Config.txt"; //Writable file location for config file.
        string configString = ""; //Holds configuration file text from when the file is first read in.

        public MainForm()
        {
            InitializeComponent();
            Activate(); //Opens form on top of others but doesn't force topmost always
        }

        private void invisibleCloseButton_Click(object sender, EventArgs e)
        {
            this.Close(); //Located behind the bottom button
        }

        private void tvButton_Click(object sender, EventArgs e)
        {
            // calls the form’s constructor
            TVForm TV = new TVForm();
            //  shows the form as a dialog
            this.Hide();
            TV.ShowDialog();
            this.Show();
        }
    }
}
