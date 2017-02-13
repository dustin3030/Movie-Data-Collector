using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovieDataCollector
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            Activate(); //Opens form on top of others but doesn't force topmost always
            ConfigFile cf = new ConfigFile();
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
        private void CompatibilityCheckerButton_Click(object sender, EventArgs e)
        {
            
        }

        private void movieButton_Click(object sender, EventArgs e)
        {
            // calls the form’s constructor
            MovieForm M = new MovieForm();
            //  shows the form as a dialog
            this.Hide();
            M.ShowDialog();
            this.Show();
        }
    }
}
