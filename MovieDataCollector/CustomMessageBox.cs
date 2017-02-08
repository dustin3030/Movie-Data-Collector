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
    public partial class CustomMessageBox : Form
    {
        
        public CustomMessageBox()
        {
            InitializeComponent();
        }

        static CustomMessageBox MsgBox;
        static DialogResult result = DialogResult.No;
        //Optional Arguments for Height and Width and Caption
        public static DialogResult Show(string text, int Height, int Width, string Caption = "Error")
        {
            MsgBox = new CustomMessageBox();
            MsgBox.Size = new Size(Width, Height);
            MsgBox.label1.Text = text;
            MsgBox.Text = Caption;
            result = DialogResult.OK;
            MsgBox.ShowDialog();
            return result;
        }
    }
}
