using System.Drawing;
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
#pragma warning disable IDE0017 // Simplify object initialization
            MsgBox = new CustomMessageBox();
#pragma warning restore IDE0017 // Simplify object initialization
            MsgBox.Size = new Size(Width, Height);
            MsgBox.label1.Text = text;
            MsgBox.Text = Caption;
            result = DialogResult.OK;
            MsgBox.ShowDialog();
            return result;
        }
    }
}
