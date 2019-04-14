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
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.iMDBcomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.theMovieDBorgToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.backdropNumberLabel = new System.Windows.Forms.Label();
            this.backdropNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.openVideoFileButton = new System.Windows.Forms.Button();
            this.videoPathLabel = new System.Windows.Forms.Label();
            this.videoPathTextBox = new System.Windows.Forms.TextBox();
            this.getHTMLButton = new System.Windows.Forms.Button();
            this.searchLabel = new System.Windows.Forms.Label();
            this.imdbIDTextBox = new System.Windows.Forms.TextBox();
            this.formatLabel = new System.Windows.Forms.Label();
            this.formatComboBox = new System.Windows.Forms.ComboBox();
            this.createFilesButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.posterNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.panel2 = new System.Windows.Forms.Panel();
            this.posterNumberLabel = new System.Windows.Forms.Label();
            this.titleComboBox = new System.Windows.Forms.ComboBox();
            this.setTextBox = new System.Windows.Forms.TextBox();
            this.yearTextBox = new System.Windows.Forms.TextBox();
            this.runTimeTextBox = new System.Windows.Forms.TextBox();
            this.mpaaTextBox = new System.Windows.Forms.TextBox();
            this.genresTextBox = new System.Windows.Forms.TextBox();
            this.plotTextBox = new System.Windows.Forms.TextBox();
            this.notificationLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.setLabel = new System.Windows.Forms.Label();
            this.yearLabel = new System.Windows.Forms.Label();
            this.runTimeLabel = new System.Windows.Forms.Label();
            this.mpaaLabel = new System.Windows.Forms.Label();
            this.genresLabel = new System.Windows.Forms.Label();
            this.moviePosterPictureBox = new System.Windows.Forms.PictureBox();
            this.formatPicturebox = new System.Windows.Forms.PictureBox();
            this.imdbPictureBox = new System.Windows.Forms.PictureBox();
            this.tmdbPictureBox = new System.Windows.Forms.PictureBox();
            this.backDropPictureBox = new System.Windows.Forms.PictureBox();
            this.InvisibleCloseButton = new System.Windows.Forms.Button();
            this.metadataCb = new System.Windows.Forms.CheckBox();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backdropNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posterNumericUpDown)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.moviePosterPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.formatPicturebox)).BeginInit();
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
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenVideoFileButton_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(104, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.InvisibleCloseButton_Click);
            // 
            // goToToolStripMenuItem
            // 
            this.goToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.iMDBcomToolStripMenuItem,
            this.theMovieDBorgToolStripMenuItem});
            this.goToToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.goToToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.goToToolStripMenuItem.Name = "goToToolStripMenuItem";
            this.goToToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.goToToolStripMenuItem.Text = "Go To";
            // 
            // iMDBcomToolStripMenuItem
            // 
            this.iMDBcomToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.iMDBcomToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.iMDBcomToolStripMenuItem.Name = "iMDBcomToolStripMenuItem";
            this.iMDBcomToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.iMDBcomToolStripMenuItem.Text = "IMDB.com";
            this.iMDBcomToolStripMenuItem.Click += new System.EventHandler(this.IMDBcomToolStripMenuItem_Click);
            // 
            // theMovieDBorgToolStripMenuItem
            // 
            this.theMovieDBorgToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.theMovieDBorgToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.theMovieDBorgToolStripMenuItem.Name = "theMovieDBorgToolStripMenuItem";
            this.theMovieDBorgToolStripMenuItem.Size = new System.Drawing.Size(169, 22);
            this.theMovieDBorgToolStripMenuItem.Text = "TheMovieDB.org";
            this.theMovieDBorgToolStripMenuItem.Click += new System.EventHandler(this.TheMovieDBorgToolStripMenuItem_Click);
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
            this.backdropNumericUpDown.ValueChanged += new System.EventHandler(this.BackdropNumericUpDown_ValueChanged);
            // 
            // openVideoFileButton
            // 
            this.openVideoFileButton.BackColor = System.Drawing.Color.GreenYellow;
            this.openVideoFileButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.openVideoFileButton.Location = new System.Drawing.Point(12, 255);
            this.openVideoFileButton.Name = "openVideoFileButton";
            this.openVideoFileButton.Size = new System.Drawing.Size(97, 23);
            this.openVideoFileButton.TabIndex = 4;
            this.openVideoFileButton.Text = "Select &File";
            this.openVideoFileButton.UseVisualStyleBackColor = false;
            this.openVideoFileButton.Click += new System.EventHandler(this.OpenVideoFileButton_Click);
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
            this.getHTMLButton.Text = "&Search";
            this.getHTMLButton.UseVisualStyleBackColor = false;
            this.getHTMLButton.Click += new System.EventHandler(this.GetHTMLButton_Click);
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
            this.formatLabel.Location = new System.Drawing.Point(439, 285);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(49, 13);
            this.formatLabel.TabIndex = 11;
            this.formatLabel.Text = "Format:";
            // 
            // formatComboBox
            // 
            this.formatComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.formatComboBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.formatComboBox.FormattingEnabled = true;
            this.formatComboBox.Items.AddRange(new object[] {
            "PLEX",
            "KODI",
            "Synology"});
            this.formatComboBox.Location = new System.Drawing.Point(494, 282);
            this.formatComboBox.Name = "formatComboBox";
            this.formatComboBox.Size = new System.Drawing.Size(100, 21);
            this.formatComboBox.TabIndex = 12;
            this.formatComboBox.SelectedIndexChanged += new System.EventHandler(this.FormatComboBox_SelectedIndexChanged);
            // 
            // createFilesButton
            // 
            this.createFilesButton.BackColor = System.Drawing.Color.GreenYellow;
            this.createFilesButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.createFilesButton.Location = new System.Drawing.Point(442, 331);
            this.createFilesButton.Name = "createFilesButton";
            this.createFilesButton.Size = new System.Drawing.Size(152, 30);
            this.createFilesButton.TabIndex = 14;
            this.createFilesButton.Text = "&Create / Rename Files";
            this.createFilesButton.UseVisualStyleBackColor = false;
            this.createFilesButton.Click += new System.EventHandler(this.CreateFilesButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.BackColor = System.Drawing.Color.GreenYellow;
            this.clearButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.clearButton.Location = new System.Drawing.Point(603, 331);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(75, 30);
            this.clearButton.TabIndex = 15;
            this.clearButton.Text = "C&lear All";
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // posterNumericUpDown
            // 
            this.posterNumericUpDown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.posterNumericUpDown.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.posterNumericUpDown.ForeColor = System.Drawing.Color.GreenYellow;
            this.posterNumericUpDown.Location = new System.Drawing.Point(3, 5);
            this.posterNumericUpDown.Maximum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.posterNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.posterNumericUpDown.Name = "posterNumericUpDown";
            this.posterNumericUpDown.Size = new System.Drawing.Size(48, 16);
            this.posterNumericUpDown.TabIndex = 17;
            this.posterNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.posterNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.posterNumericUpDown.ValueChanged += new System.EventHandler(this.PosterNumericUpDown_ValueChanged);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.Controls.Add(this.posterNumberLabel);
            this.panel2.Controls.Add(this.posterNumericUpDown);
            this.panel2.Location = new System.Drawing.Point(579, 656);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(99, 26);
            this.panel2.TabIndex = 18;
            // 
            // posterNumberLabel
            // 
            this.posterNumberLabel.AutoSize = true;
            this.posterNumberLabel.Location = new System.Drawing.Point(52, 6);
            this.posterNumberLabel.Name = "posterNumberLabel";
            this.posterNumberLabel.Size = new System.Drawing.Size(43, 13);
            this.posterNumberLabel.TabIndex = 18;
            this.posterNumberLabel.Text = "of 200";
            // 
            // titleComboBox
            // 
            this.titleComboBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.titleComboBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.titleComboBox.FormattingEnabled = true;
            this.titleComboBox.Location = new System.Drawing.Point(98, 327);
            this.titleComboBox.Name = "titleComboBox";
            this.titleComboBox.Size = new System.Drawing.Size(290, 21);
            this.titleComboBox.Sorted = true;
            this.titleComboBox.TabIndex = 19;
            // 
            // setTextBox
            // 
            this.setTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.setTextBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.setTextBox.Location = new System.Drawing.Point(98, 351);
            this.setTextBox.Name = "setTextBox";
            this.setTextBox.Size = new System.Drawing.Size(290, 20);
            this.setTextBox.TabIndex = 20;
            // 
            // yearTextBox
            // 
            this.yearTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.yearTextBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.yearTextBox.Location = new System.Drawing.Point(98, 374);
            this.yearTextBox.Name = "yearTextBox";
            this.yearTextBox.Size = new System.Drawing.Size(41, 20);
            this.yearTextBox.TabIndex = 21;
            // 
            // runTimeTextBox
            // 
            this.runTimeTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.runTimeTextBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.runTimeTextBox.Location = new System.Drawing.Point(302, 374);
            this.runTimeTextBox.Name = "runTimeTextBox";
            this.runTimeTextBox.Size = new System.Drawing.Size(86, 20);
            this.runTimeTextBox.TabIndex = 22;
            // 
            // mpaaTextBox
            // 
            this.mpaaTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.mpaaTextBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.mpaaTextBox.Location = new System.Drawing.Point(98, 397);
            this.mpaaTextBox.Name = "mpaaTextBox";
            this.mpaaTextBox.Size = new System.Drawing.Size(290, 20);
            this.mpaaTextBox.TabIndex = 23;
            // 
            // genresTextBox
            // 
            this.genresTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.genresTextBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.genresTextBox.Location = new System.Drawing.Point(98, 420);
            this.genresTextBox.Name = "genresTextBox";
            this.genresTextBox.Size = new System.Drawing.Size(290, 20);
            this.genresTextBox.TabIndex = 24;
            // 
            // plotTextBox
            // 
            this.plotTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.plotTextBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.plotTextBox.Location = new System.Drawing.Point(14, 446);
            this.plotTextBox.Multiline = true;
            this.plotTextBox.Name = "plotTextBox";
            this.plotTextBox.Size = new System.Drawing.Size(374, 211);
            this.plotTextBox.TabIndex = 25;
            // 
            // notificationLabel
            // 
            this.notificationLabel.AutoSize = true;
            this.notificationLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this.notificationLabel.Location = new System.Drawing.Point(12, 660);
            this.notificationLabel.Name = "notificationLabel";
            this.notificationLabel.Size = new System.Drawing.Size(34, 13);
            this.notificationLabel.TabIndex = 26;
            this.notificationLabel.Text = "Start";
            this.notificationLabel.Visible = false;
            // 
            // titleLabel
            // 
            this.titleLabel.AutoSize = true;
            this.titleLabel.Location = new System.Drawing.Point(56, 331);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(36, 13);
            this.titleLabel.TabIndex = 27;
            this.titleLabel.Text = "Title:";
            // 
            // setLabel
            // 
            this.setLabel.AutoSize = true;
            this.setLabel.Location = new System.Drawing.Point(62, 354);
            this.setLabel.Name = "setLabel";
            this.setLabel.Size = new System.Drawing.Size(30, 13);
            this.setLabel.TabIndex = 28;
            this.setLabel.Text = "Set:";
            // 
            // yearLabel
            // 
            this.yearLabel.AutoSize = true;
            this.yearLabel.Location = new System.Drawing.Point(55, 377);
            this.yearLabel.Name = "yearLabel";
            this.yearLabel.Size = new System.Drawing.Size(37, 13);
            this.yearLabel.TabIndex = 29;
            this.yearLabel.Text = "Year:";
            // 
            // runTimeLabel
            // 
            this.runTimeLabel.AutoSize = true;
            this.runTimeLabel.Location = new System.Drawing.Point(231, 377);
            this.runTimeLabel.Name = "runTimeLabel";
            this.runTimeLabel.Size = new System.Drawing.Size(65, 13);
            this.runTimeLabel.TabIndex = 30;
            this.runTimeLabel.Text = "Run Time:";
            // 
            // mpaaLabel
            // 
            this.mpaaLabel.AutoSize = true;
            this.mpaaLabel.Location = new System.Drawing.Point(6, 400);
            this.mpaaLabel.Name = "mpaaLabel";
            this.mpaaLabel.Size = new System.Drawing.Size(86, 13);
            this.mpaaLabel.TabIndex = 31;
            this.mpaaLabel.Text = "MPAA Rating:";
            // 
            // genresLabel
            // 
            this.genresLabel.AutoSize = true;
            this.genresLabel.Location = new System.Drawing.Point(41, 423);
            this.genresLabel.Name = "genresLabel";
            this.genresLabel.Size = new System.Drawing.Size(51, 13);
            this.genresLabel.TabIndex = 32;
            this.genresLabel.Text = "Genres:";
            // 
            // moviePosterPictureBox
            // 
            this.moviePosterPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.moviePosterPictureBox.Image = global::MovieDataCollector.Properties.Resources.film_reel__Small_;
            this.moviePosterPictureBox.InitialImage = null;
            this.moviePosterPictureBox.Location = new System.Drawing.Point(442, 365);
            this.moviePosterPictureBox.Name = "moviePosterPictureBox";
            this.moviePosterPictureBox.Size = new System.Drawing.Size(236, 317);
            this.moviePosterPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.moviePosterPictureBox.TabIndex = 16;
            this.moviePosterPictureBox.TabStop = false;
            // 
            // formatPicturebox
            // 
            this.formatPicturebox.BackColor = System.Drawing.Color.Transparent;
            this.formatPicturebox.Image = global::MovieDataCollector.Properties.Resources.Synology;
            this.formatPicturebox.InitialImage = global::MovieDataCollector.Properties.Resources.Universal;
            this.formatPicturebox.Location = new System.Drawing.Point(600, 280);
            this.formatPicturebox.Name = "formatPicturebox";
            this.formatPicturebox.Size = new System.Drawing.Size(78, 46);
            this.formatPicturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.formatPicturebox.TabIndex = 13;
            this.formatPicturebox.TabStop = false;
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
            // InvisibleCloseButton
            // 
            this.InvisibleCloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.InvisibleCloseButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.InvisibleCloseButton.Location = new System.Drawing.Point(378, 647);
            this.InvisibleCloseButton.Name = "InvisibleCloseButton";
            this.InvisibleCloseButton.Size = new System.Drawing.Size(10, 10);
            this.InvisibleCloseButton.TabIndex = 33;
            this.InvisibleCloseButton.Text = "button1";
            this.InvisibleCloseButton.UseVisualStyleBackColor = true;
            this.InvisibleCloseButton.Click += new System.EventHandler(this.InvisibleCloseButton_Click);
            // 
            // metadataCb
            // 
            this.metadataCb.BackColor = System.Drawing.Color.Transparent;
            this.metadataCb.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.metadataCb.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.metadataCb.Checked = true;
            this.metadataCb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.metadataCb.ForeColor = System.Drawing.Color.GreenYellow;
            this.metadataCb.Location = new System.Drawing.Point(442, 309);
            this.metadataCb.Name = "metadataCb";
            this.metadataCb.Size = new System.Drawing.Size(152, 17);
            this.metadataCb.TabIndex = 34;
            this.metadataCb.Text = "Add &Metadata To File";
            this.metadataCb.UseVisualStyleBackColor = false;
            // 
            // MovieForm
            // 
            this.AcceptButton = this.getHTMLButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.CancelButton = this.InvisibleCloseButton;
            this.ClientSize = new System.Drawing.Size(690, 688);
            this.Controls.Add(this.metadataCb);
            this.Controls.Add(this.genresLabel);
            this.Controls.Add(this.mpaaLabel);
            this.Controls.Add(this.runTimeLabel);
            this.Controls.Add(this.yearLabel);
            this.Controls.Add(this.setLabel);
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.notificationLabel);
            this.Controls.Add(this.genresTextBox);
            this.Controls.Add(this.mpaaTextBox);
            this.Controls.Add(this.runTimeTextBox);
            this.Controls.Add(this.yearTextBox);
            this.Controls.Add(this.setTextBox);
            this.Controls.Add(this.titleComboBox);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.moviePosterPictureBox);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.createFilesButton);
            this.Controls.Add(this.formatPicturebox);
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
            this.Controls.Add(this.plotTextBox);
            this.Controls.Add(this.InvisibleCloseButton);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.GreenYellow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(453, 43);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MovieForm";
            this.Text = "Movie Naming & Packaging";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backdropNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posterNumericUpDown)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.moviePosterPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.formatPicturebox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.imdbPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tmdbPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.backDropPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
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
        private System.Windows.Forms.PictureBox formatPicturebox;
        private System.Windows.Forms.Button createFilesButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.PictureBox moviePosterPictureBox;
        private System.Windows.Forms.NumericUpDown posterNumericUpDown;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label posterNumberLabel;
        private System.Windows.Forms.ComboBox titleComboBox;
        private System.Windows.Forms.TextBox setTextBox;
        private System.Windows.Forms.TextBox yearTextBox;
        private System.Windows.Forms.TextBox runTimeTextBox;
        private System.Windows.Forms.TextBox mpaaTextBox;
        private System.Windows.Forms.TextBox genresTextBox;
        private System.Windows.Forms.TextBox plotTextBox;
        private System.Windows.Forms.Label notificationLabel;
        private System.Windows.Forms.Label titleLabel;
        private System.Windows.Forms.Label setLabel;
        private System.Windows.Forms.Label yearLabel;
        private System.Windows.Forms.Label runTimeLabel;
        private System.Windows.Forms.Label mpaaLabel;
        private System.Windows.Forms.Label genresLabel;
        private System.Windows.Forms.Button InvisibleCloseButton;
        private System.Windows.Forms.ToolStripMenuItem theMovieDBorgToolStripMenuItem;
        private System.Windows.Forms.CheckBox metadataCb;
    }
}