namespace MovieDataCollector
{
    partial class MovieForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovieForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.instructionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iMDBcomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.backdropNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.backdropNumberLabel = new System.Windows.Forms.Label();
            this.openVideoFileButton = new System.Windows.Forms.Button();
            this.videoPathLabel = new System.Windows.Forms.Label();
            this.videoPathTextBox = new System.Windows.Forms.TextBox();
            this.getHTMLButton = new System.Windows.Forms.Button();
            this.searchLabel = new System.Windows.Forms.Label();
            this.imdbIDTextBox = new System.Windows.Forms.TextBox();
            this.formatLabel = new System.Windows.Forms.Label();
            this.formatComboBox = new System.Windows.Forms.ComboBox();
            this.formatPictureBox = new System.Windows.Forms.PictureBox();
            this.imdbPictureBox = new System.Windows.Forms.PictureBox();
            this.tmdbPictureBox = new System.Windows.Forms.PictureBox();
            this.backDropPictureBox = new System.Windows.Forms.PictureBox();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backdropNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formatPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.imdbPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tmdbPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.backDropPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.GreenYellow;
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.goToToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(690, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.instructionsToolStripMenuItem,
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // goToToolStripMenuItem
            // 
            this.goToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iMDBcomToolStripMenuItem});
            this.goToToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goToToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.goToToolStripMenuItem.Name = "goToToolStripMenuItem";
            this.goToToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.goToToolStripMenuItem.Text = "Go To";
            // 
            // instructionsToolStripMenuItem
            // 
            this.instructionsToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.instructionsToolStripMenuItem.Name = "instructionsToolStripMenuItem";
            this.instructionsToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.instructionsToolStripMenuItem.Text = "Instructions";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.openToolStripMenuItem.Text = "Open";
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // iMDBcomToolStripMenuItem
            // 
            this.iMDBcomToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.iMDBcomToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.iMDBcomToolStripMenuItem.Name = "iMDBcomToolStripMenuItem";
            this.iMDBcomToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.iMDBcomToolStripMenuItem.Text = "IMDB.com";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.Controls.Add(this.backdropNumberLabel);
            this.panel1.Controls.Add(this.backdropNumericUpDown);
            this.panel1.Location = new System.Drawing.Point(300, 226);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(108, 23);
            this.panel1.TabIndex = 2;
            // 
            // backdropNumericUpDown
            // 
            this.backdropNumericUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.backdropNumericUpDown.ForeColor = System.Drawing.Color.GreenYellow;
            this.backdropNumericUpDown.Location = new System.Drawing.Point(3, 3);
            this.backdropNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.backdropNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.backdropNumericUpDown.Name = "backdropNumericUpDown";
            this.backdropNumericUpDown.Size = new System.Drawing.Size(48, 20);
            this.backdropNumericUpDown.TabIndex = 3;
            this.backdropNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.backdropNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // backdropNumberLabel
            // 
            this.backdropNumberLabel.AutoSize = true;
            this.backdropNumberLabel.BackColor = System.Drawing.Color.Transparent;
            this.backdropNumberLabel.Location = new System.Drawing.Point(57, 5);
            this.backdropNumberLabel.Name = "backdropNumberLabel";
            this.backdropNumberLabel.Size = new System.Drawing.Size(43, 13);
            this.backdropNumberLabel.TabIndex = 4;
            this.backdropNumberLabel.Text = "of 200";
            // 
            // openVideoFileButton
            // 
            this.openVideoFileButton.BackColor = System.Drawing.Color.GreenYellow;
            this.openVideoFileButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.openVideoFileButton.Location = new System.Drawing.Point(12, 255);
            this.openVideoFileButton.Name = "openVideoFileButton";
            this.openVideoFileButton.Size = new System.Drawing.Size(97, 23);
            this.openVideoFileButton.TabIndex = 4;
            this.openVideoFileButton.Text = "&Open Video File";
            this.openVideoFileButton.UseVisualStyleBackColor = false;
            // 
            // videoPathLabel
            // 
            this.videoPathLabel.AutoSize = true;
            this.videoPathLabel.Location = new System.Drawing.Point(115, 261);
            this.videoPathLabel.Name = "videoPathLabel";
            this.videoPathLabel.Size = new System.Drawing.Size(73, 13);
            this.videoPathLabel.TabIndex = 5;
            this.videoPathLabel.Text = "Video Path:";
            // 
            // videoPathTextBox
            // 
            this.videoPathTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.videoPathTextBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.videoPathTextBox.Location = new System.Drawing.Point(194, 258);
            this.videoPathTextBox.Name = "videoPathTextBox";
            this.videoPathTextBox.Size = new System.Drawing.Size(484, 20);
            this.videoPathTextBox.TabIndex = 6;
            // 
            // getHTMLButton
            // 
            this.getHTMLButton.BackColor = System.Drawing.Color.GreenYellow;
            this.getHTMLButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.getHTMLButton.Location = new System.Drawing.Point(12, 294);
            this.getHTMLButton.Name = "getHTMLButton";
            this.getHTMLButton.Size = new System.Drawing.Size(75, 23);
            this.getHTMLButton.TabIndex = 7;
            this.getHTMLButton.Text = "Get &HTML";
            this.getHTMLButton.UseVisualStyleBackColor = false;
            // 
            // searchLabel
            // 
            this.searchLabel.AutoSize = true;
            this.searchLabel.Location = new System.Drawing.Point(166, 280);
            this.searchLabel.Name = "searchLabel";
            this.searchLabel.Size = new System.Drawing.Size(147, 13);
            this.searchLabel.TabIndex = 9;
            this.searchLabel.Text = "Search: IMDB ID or Title";
            // 
            // imdbIDTextBox
            // 
            this.imdbIDTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.imdbIDTextBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.imdbIDTextBox.Location = new System.Drawing.Point(166, 296);
            this.imdbIDTextBox.Name = "imdbIDTextBox";
            this.imdbIDTextBox.Size = new System.Drawing.Size(222, 20);
            this.imdbIDTextBox.TabIndex = 10;
            // 
            // formatLabel
            // 
            this.formatLabel.AutoSize = true;
            this.formatLabel.Location = new System.Drawing.Point(439, 294);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(49, 13);
            this.formatLabel.TabIndex = 11;
            this.formatLabel.Text = "Format:";
            // 
            // formatComboBox
            // 
            this.formatComboBox.FormattingEnabled = true;
            this.formatComboBox.Items.AddRange(new object[] {
            "PLEX",
            "SYNOLOGY",
            "UNIVERSAL",
            "KODI"});
            this.formatComboBox.Location = new System.Drawing.Point(494, 291);
            this.formatComboBox.Name = "formatComboBox";
            this.formatComboBox.Size = new System.Drawing.Size(100, 21);
            this.formatComboBox.TabIndex = 12;
            // 
            // formatPictureBox
            // 
            this.formatPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.formatPictureBox.Image = global::MovieDataCollector.Properties.Resources.Universal;
            this.formatPictureBox.InitialImage = global::MovieDataCollector.Properties.Resources.Universal;
            this.formatPictureBox.Location = new System.Drawing.Point(600, 280);
            this.formatPictureBox.Name = "formatPictureBox";
            this.formatPictureBox.Size = new System.Drawing.Size(78, 46);
            this.formatPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.formatPictureBox.TabIndex = 13;
            this.formatPictureBox.TabStop = false;
            // 
            // imdbPictureBox
            // 
            this.imdbPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.imdbPictureBox.Image = global::MovieDataCollector.Properties.Resources.IMDB_Logo;
            this.imdbPictureBox.Location = new System.Drawing.Point(98, 289);
            this.imdbPictureBox.Name = "imdbPictureBox";
            this.imdbPictureBox.Size = new System.Drawing.Size(62, 32);
            this.imdbPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.imdbPictureBox.TabIndex = 8;
            this.imdbPictureBox.TabStop = false;
            // 
            // tmdbPictureBox
            // 
            this.tmdbPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.tmdbPictureBox.Image = global::MovieDataCollector.Properties.Resources.MovieDBLogo;
            this.tmdbPictureBox.Location = new System.Drawing.Point(453, 43);
            this.tmdbPictureBox.Name = "tmdbPictureBox";
            this.tmdbPictureBox.Size = new System.Drawing.Size(197, 191);
            this.tmdbPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.tmdbPictureBox.TabIndex = 3;
            this.tmdbPictureBox.TabStop = false;
            // 
            // backDropPictureBox
            // 
            this.backDropPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.backDropPictureBox.Image = global::MovieDataCollector.Properties.Resources.highlight_reel;
            this.backDropPictureBox.InitialImage = null;
            this.backDropPictureBox.Location = new System.Drawing.Point(0, 25);
            this.backDropPictureBox.Name = "backDropPictureBox";
            this.backDropPictureBox.Size = new System.Drawing.Size(408, 224);
            this.backDropPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.backDropPictureBox.TabIndex = 1;
            this.backDropPictureBox.TabStop = false;
            // 
            // MovieForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.ClientSize = new System.Drawing.Size(690, 688);
            this.Controls.Add(this.formatPictureBox);
            this.Controls.Add(this.formatComboBox);
            this.Controls.Add(this.formatLabel);
            this.Controls.Add(this.imdbIDTextBox);
            this.Controls.Add(this.searchLabel);
            this.Controls.Add(this.imdbPictureBox);
            this.Controls.Add(this.getHTMLButton);
            this.Controls.Add(this.videoPathTextBox);
            this.Controls.Add(this.videoPathLabel);
            this.Controls.Add(this.openVideoFileButton);
            this.Controls.Add(this.tmdbPictureBox);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.backDropPictureBox);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.GreenYellow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(453, 43);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MovieForm";
            this.Text = "Movie Data Collector";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backdropNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formatPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imdbPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tmdbPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backDropPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem instructionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem iMDBcomToolStripMenuItem;
        private System.Windows.Forms.PictureBox backDropPictureBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label backdropNumberLabel;
        private System.Windows.Forms.NumericUpDown backdropNumericUpDown;
        private System.Windows.Forms.PictureBox tmdbPictureBox;
        private System.Windows.Forms.Button openVideoFileButton;
        private System.Windows.Forms.Label videoPathLabel;
        private System.Windows.Forms.TextBox videoPathTextBox;
        private System.Windows.Forms.Button getHTMLButton;
        private System.Windows.Forms.PictureBox imdbPictureBox;
        private System.Windows.Forms.Label searchLabel;
        private System.Windows.Forms.TextBox imdbIDTextBox;
        private System.Windows.Forms.Label formatLabel;
        private System.Windows.Forms.ComboBox formatComboBox;
        private System.Windows.Forms.PictureBox formatPictureBox;
    }
}