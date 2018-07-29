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
    public partial class TVManualRename : Form
    {
        public string ChangedFileName { get; set; }
        List<string> listOfEpisodeNames;
        string ext = "";

        public TVManualRename(string file1, string fileForEdit, List<string> listOfEpisodes)
        {
            InitializeComponent();
            listOfEpisodeNames = listOfEpisodes;
            ChangedFileName = fileForEdit;
            originalTB.Text = file1;
            renameCB.Text = fileForEdit;
            string[] Tokens = file1.Split('.');
            ext = Tokens[Tokens.Length - 1].ToString(); //should be extension

            PopulateComboBox(listOfEpisodeNames);
        }
        private string CheckFileName(string filename)
        {
            StringBuilder SB = new StringBuilder();

            // Replaces invalid characters /\:*?<>|
            string[] separators = { "/", "\\", ":", "*", "?", "<", ">", "|" };
            foreach (string s in separators)
            {
                if (filename.Contains(s))
                {
                    filename = filename.Replace(s, "");
                    SB.Append(s + ", ");
                }

            }

            if (!string.IsNullOrEmpty(SB.ToString()))
            {
                CustomMessageBox.Show("Invalid Characters found in Title: " + SB.ToString() + "\n\n Characters were removed.", 211, 332);
                SB.Clear();
            }
            return filename;
        }
        private void PopulateComboBox(List<string> inputList)
        {
            renameCB.Items.Clear();
            foreach (string S in inputList)
            {
                renameCB.Items.Add(S + "." + ext);
            }
        }
        private void FilterComboBox()
        {
            if (!string.IsNullOrEmpty(renameCB.Text))
            {
                //Filter List Contents to what is typed
                List<string> filteredList = new List<string>();
                for (int i = 0; i < listOfEpisodeNames.Count; i++)
                {
                    if (listOfEpisodeNames[i].ToUpper().Contains(renameCB.Text.ToUpper()))
                    {
                        filteredList.Add(listOfEpisodeNames[i]);
                    }
                }
                PopulateComboBox(filteredList);
            }
            else
            {
                PopulateComboBox(listOfEpisodeNames);
            }
        }
        private void CancelBtn_Click_1(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            this.Close();
        }
        private void FilterBtn_Click_1(object sender, EventArgs e)
        {
            FilterComboBox();
            if (renameCB.Items.Count == 1)
            {
                renameCB.Text = renameCB.Items[0].ToString();
            }
        }
        private void AcceptBtn_Click_1(object sender, EventArgs e)
        {
            if (renameCB.Text.Contains("/") ||
                          renameCB.Text.Contains("\\") ||
                          renameCB.Text.Contains(":") ||
                          renameCB.Text.Contains("*") ||
                          renameCB.Text.Contains("?") ||
                          renameCB.Text.Contains("<") ||
                          renameCB.Text.Contains(">") ||
                          renameCB.Text.Contains("|"))
            {
                ChangedFileName = CheckFileName(renameCB.Text);
                renameCB.Text = ChangedFileName;
                DialogResult = DialogResult.None;
                return;
            }
            else
            {
                DialogResult = DialogResult.OK;
                ChangedFileName = renameCB.Text;
                this.Close();
            }
        }
    }
}
