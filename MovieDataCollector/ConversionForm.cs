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
                      Application folder (Release or Debug) or it will not work*/

namespace MovieDataCollector
{
    public partial class ConversionForm : Form
    {
        public ConversionForm()
        {
            InitializeComponent();
        }
    }
}
