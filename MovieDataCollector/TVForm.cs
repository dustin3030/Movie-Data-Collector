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
    public partial class TVForm : Form
    {
        const string APIKey = "8AC38B77755B00A0"; //API Key for the TVDB

        public TVForm()
        {
            InitializeComponent();
        }

        private void getHTML()
        {
            int ID = 0;
            string TitleBox = seriesIDTitleTextbox.Text;

            notificationLabel.Visible = true;
            notificationLabel.Text = "Searching..." + seriesIDTitleTextbox.Text;
            notificationLabel.Invalidate();
            notificationLabel.Update();

            //Scan for Series ID, if no ID found scan for series title and retrieve list of possible series to choose from then parse out the correct id.

            //Create object that looks up possible TV Series based on the text in the Series Title Box
        }
    }
}
