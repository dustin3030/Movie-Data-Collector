﻿namespace MovieDataCollector
{
    partial class TVForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TVForm));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.theTVDBcomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.favoritesCombo = new System.Windows.Forms.ComboBox();
            this.favoritesLabel = new System.Windows.Forms.Label();
            this.seriesIDTitleTextbox = new System.Windows.Forms.TextBox();
            this.seriesIDTitleLabel = new System.Windows.Forms.Label();
            this.getHTMLButton = new System.Windows.Forms.Button();
            this.addFavoriteButton = new System.Windows.Forms.Button();
            this.deleteFavoriteButton = new System.Windows.Forms.Button();
            this.clearButton = new System.Windows.Forms.Button();
            this.getFileNamesButton = new System.Windows.Forms.Button();
            this.parentPathLabel = new System.Windows.Forms.Label();
            this.formatLabel = new System.Windows.Forms.Label();
            this.formatCombo = new System.Windows.Forms.ComboBox();
            this.currentFileNamesLabel = new System.Windows.Forms.Label();
            this.proposedFileNamesLabel = new System.Windows.Forms.Label();
            this.fileNamesListbox = new System.Windows.Forms.ListBox();
            this.changedFileNamesListbox = new System.Windows.Forms.ListBox();
            this.removeItemLabel = new System.Windows.Forms.Label();
            this.manualRenameLabel = new System.Windows.Forms.Label();
            this.previewChangesButton = new System.Windows.Forms.Button();
            this.absoluteCb = new System.Windows.Forms.CheckBox();
            this.titleCb = new System.Windows.Forms.CheckBox();
            this.notificationLabel = new System.Windows.Forms.Label();
            this.seriesImagePicturebox = new System.Windows.Forms.PictureBox();
            this.changeFileNamesButton = new System.Windows.Forms.Button();
            this.InvisibleCloseButton = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seriesImagePicturebox)).BeginInit();
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
            this.menuStrip1.Size = new System.Drawing.Size(832, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.openToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.closeToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.closeToolStripMenuItem.Text = "Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // goToToolStripMenuItem
            // 
            this.goToToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.theTVDBcomToolStripMenuItem});
            this.goToToolStripMenuItem.Name = "goToToolStripMenuItem";
            this.goToToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
            this.goToToolStripMenuItem.Text = "Go To";
            // 
            // theTVDBcomToolStripMenuItem
            // 
            this.theTVDBcomToolStripMenuItem.BackColor = System.Drawing.Color.GreenYellow;
            this.theTVDBcomToolStripMenuItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.theTVDBcomToolStripMenuItem.Name = "theTVDBcomToolStripMenuItem";
            this.theTVDBcomToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.theTVDBcomToolStripMenuItem.Text = "TheTVDB.com";
            this.theTVDBcomToolStripMenuItem.Click += new System.EventHandler(this.theTVDBcomToolStripMenuItem_Click);
            // 
            // favoritesCombo
            // 
            this.favoritesCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.favoritesCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.favoritesCombo.ForeColor = System.Drawing.Color.GreenYellow;
            this.favoritesCombo.FormattingEnabled = true;
            this.favoritesCombo.Location = new System.Drawing.Point(12, 48);
            this.favoritesCombo.Name = "favoritesCombo";
            this.favoritesCombo.Size = new System.Drawing.Size(279, 21);
            this.favoritesCombo.TabIndex = 1;
            this.favoritesCombo.SelectedValueChanged += new System.EventHandler(this.favoritesCombo_SelectionChangeCommitted);
            // 
            // favoritesLabel
            // 
            this.favoritesLabel.AutoSize = true;
            this.favoritesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.favoritesLabel.ForeColor = System.Drawing.Color.GreenYellow;
            this.favoritesLabel.Location = new System.Drawing.Point(9, 31);
            this.favoritesLabel.Name = "favoritesLabel";
            this.favoritesLabel.Size = new System.Drawing.Size(59, 13);
            this.favoritesLabel.TabIndex = 2;
            this.favoritesLabel.Text = "Favorites";
            // 
            // seriesIDTitleTextbox
            // 
            this.seriesIDTitleTextbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.seriesIDTitleTextbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.seriesIDTitleTextbox.ForeColor = System.Drawing.Color.GreenYellow;
            this.seriesIDTitleTextbox.Location = new System.Drawing.Point(302, 48);
            this.seriesIDTitleTextbox.Name = "seriesIDTitleTextbox";
            this.seriesIDTitleTextbox.Size = new System.Drawing.Size(270, 20);
            this.seriesIDTitleTextbox.TabIndex = 3;
            // 
            // seriesIDTitleLabel
            // 
            this.seriesIDTitleLabel.AutoSize = true;
            this.seriesIDTitleLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.seriesIDTitleLabel.ForeColor = System.Drawing.Color.GreenYellow;
            this.seriesIDTitleLabel.Location = new System.Drawing.Point(299, 31);
            this.seriesIDTitleLabel.Name = "seriesIDTitleLabel";
            this.seriesIDTitleLabel.Size = new System.Drawing.Size(107, 13);
            this.seriesIDTitleLabel.TabIndex = 4;
            this.seriesIDTitleLabel.Text = " Series ID or Title";
            // 
            // getHTMLButton
            // 
            this.getHTMLButton.BackColor = System.Drawing.Color.GreenYellow;
            this.getHTMLButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.getHTMLButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.getHTMLButton.Location = new System.Drawing.Point(587, 48);
            this.getHTMLButton.Name = "getHTMLButton";
            this.getHTMLButton.Size = new System.Drawing.Size(77, 21);
            this.getHTMLButton.TabIndex = 6;
            this.getHTMLButton.Text = "Get &HTML";
            this.getHTMLButton.UseVisualStyleBackColor = false;
            this.getHTMLButton.Click += new System.EventHandler(this.getHTMLButton_Click);
            // 
            // addFavoriteButton
            // 
            this.addFavoriteButton.BackColor = System.Drawing.Color.GreenYellow;
            this.addFavoriteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addFavoriteButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.addFavoriteButton.Location = new System.Drawing.Point(670, 48);
            this.addFavoriteButton.Name = "addFavoriteButton";
            this.addFavoriteButton.Size = new System.Drawing.Size(66, 21);
            this.addFavoriteButton.TabIndex = 7;
            this.addFavoriteButton.Text = "&Add Fav";
            this.addFavoriteButton.UseVisualStyleBackColor = false;
            this.addFavoriteButton.Click += new System.EventHandler(this.addFavoriteButton_Click);
            // 
            // deleteFavoriteButton
            // 
            this.deleteFavoriteButton.BackColor = System.Drawing.Color.GreenYellow;
            this.deleteFavoriteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.deleteFavoriteButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.deleteFavoriteButton.Location = new System.Drawing.Point(742, 48);
            this.deleteFavoriteButton.Name = "deleteFavoriteButton";
            this.deleteFavoriteButton.Size = new System.Drawing.Size(78, 21);
            this.deleteFavoriteButton.TabIndex = 8;
            this.deleteFavoriteButton.Text = "&Delete Fav";
            this.deleteFavoriteButton.UseVisualStyleBackColor = false;
            this.deleteFavoriteButton.Click += new System.EventHandler(this.deleteFavoriteButton_Click);
            // 
            // clearButton
            // 
            this.clearButton.BackColor = System.Drawing.Color.GreenYellow;
            this.clearButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.clearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clearButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.clearButton.Location = new System.Drawing.Point(755, 187);
            this.clearButton.Name = "clearButton";
            this.clearButton.Size = new System.Drawing.Size(65, 25);
            this.clearButton.TabIndex = 9;
            this.clearButton.Text = "&Clear All";
            this.clearButton.UseVisualStyleBackColor = false;
            this.clearButton.Click += new System.EventHandler(this.clearButton_Click);
            // 
            // getFileNamesButton
            // 
            this.getFileNamesButton.BackColor = System.Drawing.Color.GreenYellow;
            this.getFileNamesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.getFileNamesButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.getFileNamesButton.Location = new System.Drawing.Point(12, 187);
            this.getFileNamesButton.Name = "getFileNamesButton";
            this.getFileNamesButton.Size = new System.Drawing.Size(106, 25);
            this.getFileNamesButton.TabIndex = 10;
            this.getFileNamesButton.Text = "&Get File Names";
            this.getFileNamesButton.UseVisualStyleBackColor = false;
            this.getFileNamesButton.Click += new System.EventHandler(this.getFileNamesButton_Click);
            // 
            // parentPathLabel
            // 
            this.parentPathLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.parentPathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.parentPathLabel.ForeColor = System.Drawing.Color.GreenYellow;
            this.parentPathLabel.Location = new System.Drawing.Point(126, 187);
            this.parentPathLabel.Name = "parentPathLabel";
            this.parentPathLabel.Size = new System.Drawing.Size(623, 20);
            this.parentPathLabel.TabIndex = 11;
            this.parentPathLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // formatLabel
            // 
            this.formatLabel.AutoSize = true;
            this.formatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formatLabel.ForeColor = System.Drawing.Color.GreenYellow;
            this.formatLabel.Location = new System.Drawing.Point(691, 221);
            this.formatLabel.Name = "formatLabel";
            this.formatLabel.Size = new System.Drawing.Size(45, 13);
            this.formatLabel.TabIndex = 12;
            this.formatLabel.Text = "Format";
            // 
            // formatCombo
            // 
            this.formatCombo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.formatCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.formatCombo.ForeColor = System.Drawing.Color.GreenYellow;
            this.formatCombo.FormattingEnabled = true;
            this.formatCombo.Items.AddRange(new object[] {
            "PLEX",
            "KODI",
            "Synology"});
            this.formatCombo.Location = new System.Drawing.Point(742, 218);
            this.formatCombo.Name = "formatCombo";
            this.formatCombo.Size = new System.Drawing.Size(78, 21);
            this.formatCombo.TabIndex = 13;
            this.formatCombo.Text = "Synology";
            this.formatCombo.SelectedIndexChanged += new System.EventHandler(this.formatCombo_SelectedIndexChanged);
            // 
            // currentFileNamesLabel
            // 
            this.currentFileNamesLabel.AutoSize = true;
            this.currentFileNamesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.currentFileNamesLabel.ForeColor = System.Drawing.Color.GreenYellow;
            this.currentFileNamesLabel.Location = new System.Drawing.Point(12, 228);
            this.currentFileNamesLabel.Name = "currentFileNamesLabel";
            this.currentFileNamesLabel.Size = new System.Drawing.Size(114, 13);
            this.currentFileNamesLabel.TabIndex = 14;
            this.currentFileNamesLabel.Text = "Current File Names";
            // 
            // proposedFileNamesLabel
            // 
            this.proposedFileNamesLabel.AutoSize = true;
            this.proposedFileNamesLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.proposedFileNamesLabel.ForeColor = System.Drawing.Color.GreenYellow;
            this.proposedFileNamesLabel.Location = new System.Drawing.Point(416, 228);
            this.proposedFileNamesLabel.Name = "proposedFileNamesLabel";
            this.proposedFileNamesLabel.Size = new System.Drawing.Size(126, 13);
            this.proposedFileNamesLabel.TabIndex = 15;
            this.proposedFileNamesLabel.Text = "Proposed File Names";
            // 
            // fileNamesListbox
            // 
            this.fileNamesListbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.fileNamesListbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.fileNamesListbox.ForeColor = System.Drawing.Color.GreenYellow;
            this.fileNamesListbox.FormattingEnabled = true;
            this.fileNamesListbox.Location = new System.Drawing.Point(12, 244);
            this.fileNamesListbox.Name = "fileNamesListbox";
            this.fileNamesListbox.Size = new System.Drawing.Size(401, 199);
            this.fileNamesListbox.TabIndex = 16;
            this.fileNamesListbox.SelectedIndexChanged += new System.EventHandler(this.fileNamesListbox_SelectedIndexChanged);
            this.fileNamesListbox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.fileNamesListbox_MouseDoubleClick);
            // 
            // changedFileNamesListbox
            // 
            this.changedFileNamesListbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.changedFileNamesListbox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changedFileNamesListbox.ForeColor = System.Drawing.Color.GreenYellow;
            this.changedFileNamesListbox.FormattingEnabled = true;
            this.changedFileNamesListbox.Location = new System.Drawing.Point(419, 244);
            this.changedFileNamesListbox.Name = "changedFileNamesListbox";
            this.changedFileNamesListbox.Size = new System.Drawing.Size(401, 199);
            this.changedFileNamesListbox.TabIndex = 17;
            this.changedFileNamesListbox.SelectedIndexChanged += new System.EventHandler(this.changedFileNamesListbox_SelectedIndexChanged);
            this.changedFileNamesListbox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.changedFileNamesListbox_MouseDoubleClick);
            // 
            // removeItemLabel
            // 
            this.removeItemLabel.AutoSize = true;
            this.removeItemLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.removeItemLabel.ForeColor = System.Drawing.Color.GreenYellow;
            this.removeItemLabel.Location = new System.Drawing.Point(12, 446);
            this.removeItemLabel.Name = "removeItemLabel";
            this.removeItemLabel.Size = new System.Drawing.Size(231, 13);
            this.removeItemLabel.TabIndex = 18;
            this.removeItemLabel.Text = "Double Click Item To Remove From List";
            // 
            // manualRenameLabel
            // 
            this.manualRenameLabel.AutoSize = true;
            this.manualRenameLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.manualRenameLabel.ForeColor = System.Drawing.Color.GreenYellow;
            this.manualRenameLabel.Location = new System.Drawing.Point(416, 446);
            this.manualRenameLabel.Name = "manualRenameLabel";
            this.manualRenameLabel.Size = new System.Drawing.Size(230, 13);
            this.manualRenameLabel.TabIndex = 19;
            this.manualRenameLabel.Text = "Double Click Item to Manually Rename ";
            // 
            // previewChangesButton
            // 
            this.previewChangesButton.BackColor = System.Drawing.Color.GreenYellow;
            this.previewChangesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.previewChangesButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.previewChangesButton.Location = new System.Drawing.Point(12, 467);
            this.previewChangesButton.Name = "previewChangesButton";
            this.previewChangesButton.Size = new System.Drawing.Size(131, 25);
            this.previewChangesButton.TabIndex = 20;
            this.previewChangesButton.Text = "Preview Changes";
            this.previewChangesButton.UseVisualStyleBackColor = false;
            this.previewChangesButton.Click += new System.EventHandler(this.previewChangesButton_Click);
            // 
            // absoluteCb
            // 
            this.absoluteCb.AutoSize = true;
            this.absoluteCb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.absoluteCb.ForeColor = System.Drawing.Color.GreenYellow;
            this.absoluteCb.Location = new System.Drawing.Point(154, 462);
            this.absoluteCb.Name = "absoluteCb";
            this.absoluteCb.Size = new System.Drawing.Size(177, 17);
            this.absoluteCb.TabIndex = 21;
            this.absoluteCb.Text = "Absolute Episode Numbers";
            this.absoluteCb.UseVisualStyleBackColor = true;
            this.absoluteCb.CheckedChanged += new System.EventHandler(this.absoluteCb_CheckedChanged);
            // 
            // titleCb
            // 
            this.titleCb.AutoSize = true;
            this.titleCb.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleCb.ForeColor = System.Drawing.Color.GreenYellow;
            this.titleCb.Location = new System.Drawing.Point(154, 481);
            this.titleCb.Name = "titleCb";
            this.titleCb.Size = new System.Drawing.Size(119, 17);
            this.titleCb.TabIndex = 22;
            this.titleCb.Text = "Title in Filename";
            this.titleCb.UseVisualStyleBackColor = true;
            this.titleCb.CheckedChanged += new System.EventHandler(this.titleCb_CheckedChanged);
            // 
            // notificationLabel
            // 
            this.notificationLabel.AutoSize = true;
            this.notificationLabel.ForeColor = System.Drawing.Color.GreenYellow;
            this.notificationLabel.Location = new System.Drawing.Point(12, 501);
            this.notificationLabel.Name = "notificationLabel";
            this.notificationLabel.Size = new System.Drawing.Size(84, 13);
            this.notificationLabel.TabIndex = 23;
            this.notificationLabel.Text = "notificationLabel";
            // 
            // seriesImagePicturebox
            // 
            this.seriesImagePicturebox.BackColor = System.Drawing.Color.Transparent;
            this.seriesImagePicturebox.Location = new System.Drawing.Point(82, 75);
            this.seriesImagePicturebox.Name = "seriesImagePicturebox";
            this.seriesImagePicturebox.Size = new System.Drawing.Size(668, 109);
            this.seriesImagePicturebox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.seriesImagePicturebox.TabIndex = 5;
            this.seriesImagePicturebox.TabStop = false;
            // 
            // changeFileNamesButton
            // 
            this.changeFileNamesButton.BackColor = System.Drawing.Color.GreenYellow;
            this.changeFileNamesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.changeFileNamesButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.changeFileNamesButton.Location = new System.Drawing.Point(658, 467);
            this.changeFileNamesButton.Name = "changeFileNamesButton";
            this.changeFileNamesButton.Size = new System.Drawing.Size(162, 25);
            this.changeFileNamesButton.TabIndex = 24;
            this.changeFileNamesButton.Text = "&Make Changes";
            this.changeFileNamesButton.UseVisualStyleBackColor = false;
            this.changeFileNamesButton.Click += new System.EventHandler(this.changeFileNamesButton_Click);
            // 
            // InvisibleCloseButton
            // 
            this.InvisibleCloseButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.InvisibleCloseButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.InvisibleCloseButton.Location = new System.Drawing.Point(810, 481);
            this.InvisibleCloseButton.Name = "InvisibleCloseButton";
            this.InvisibleCloseButton.Size = new System.Drawing.Size(10, 10);
            this.InvisibleCloseButton.TabIndex = 25;
            this.InvisibleCloseButton.UseVisualStyleBackColor = true;
            this.InvisibleCloseButton.Click += new System.EventHandler(this.InvisibleCloseButton_Click);
            // 
            // TVForm
            // 
            this.AcceptButton = this.getHTMLButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.CancelButton = this.InvisibleCloseButton;
            this.ClientSize = new System.Drawing.Size(832, 519);
            this.Controls.Add(this.changeFileNamesButton);
            this.Controls.Add(this.notificationLabel);
            this.Controls.Add(this.titleCb);
            this.Controls.Add(this.absoluteCb);
            this.Controls.Add(this.previewChangesButton);
            this.Controls.Add(this.manualRenameLabel);
            this.Controls.Add(this.removeItemLabel);
            this.Controls.Add(this.changedFileNamesListbox);
            this.Controls.Add(this.fileNamesListbox);
            this.Controls.Add(this.proposedFileNamesLabel);
            this.Controls.Add(this.currentFileNamesLabel);
            this.Controls.Add(this.formatCombo);
            this.Controls.Add(this.formatLabel);
            this.Controls.Add(this.parentPathLabel);
            this.Controls.Add(this.getFileNamesButton);
            this.Controls.Add(this.clearButton);
            this.Controls.Add(this.deleteFavoriteButton);
            this.Controls.Add(this.addFavoriteButton);
            this.Controls.Add(this.getHTMLButton);
            this.Controls.Add(this.seriesImagePicturebox);
            this.Controls.Add(this.seriesIDTitleLabel);
            this.Controls.Add(this.seriesIDTitleTextbox);
            this.Controls.Add(this.favoritesLabel);
            this.Controls.Add(this.favoritesCombo);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.InvisibleCloseButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TVForm";
            this.Text = "TVForm";
            this.Load += new System.EventHandler(this.TVForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.seriesImagePicturebox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem theTVDBcomToolStripMenuItem;
        private System.Windows.Forms.ComboBox favoritesCombo;
        private System.Windows.Forms.Label favoritesLabel;
        private System.Windows.Forms.TextBox seriesIDTitleTextbox;
        private System.Windows.Forms.Label seriesIDTitleLabel;
        private System.Windows.Forms.PictureBox seriesImagePicturebox;
        private System.Windows.Forms.Button getHTMLButton;
        private System.Windows.Forms.Button addFavoriteButton;
        private System.Windows.Forms.Button deleteFavoriteButton;
        private System.Windows.Forms.Button clearButton;
        private System.Windows.Forms.Button getFileNamesButton;
        private System.Windows.Forms.Label parentPathLabel;
        private System.Windows.Forms.Label formatLabel;
        private System.Windows.Forms.ComboBox formatCombo;
        private System.Windows.Forms.Label currentFileNamesLabel;
        private System.Windows.Forms.Label proposedFileNamesLabel;
        private System.Windows.Forms.ListBox fileNamesListbox;
        private System.Windows.Forms.ListBox changedFileNamesListbox;
        private System.Windows.Forms.Label removeItemLabel;
        private System.Windows.Forms.Label manualRenameLabel;
        private System.Windows.Forms.Button previewChangesButton;
        private System.Windows.Forms.CheckBox absoluteCb;
        private System.Windows.Forms.CheckBox titleCb;
        private System.Windows.Forms.Label notificationLabel;
        private System.Windows.Forms.Button changeFileNamesButton;
        private System.Windows.Forms.Button InvisibleCloseButton;
    }
}