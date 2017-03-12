using System;
using System.Windows.Forms;
using System.Diagnostics; //Allows for using Process.Start codes lines

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
            // calls the form’s constructor
            ConversionForm CForm = new ConversionForm();
            //  shows the form as a dialog
            this.Hide();
            CForm.ShowDialog();
            this.Show();
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

        private void iMDBcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.imdb.com");
        }

        private void theTVDBcomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.thetvdb.com");
        }

        private void theMovieDBorgToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start("http://www.themoviedb.org");
        }
    }
}
