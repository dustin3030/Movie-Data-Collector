﻿namespace MovieDataCollector
{
    partial class TVSeriesSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TVSeriesSelection));
            this.label1 = new System.Windows.Forms.Label();
            this.SeriesPosterPictureBox = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.yearRbtn = new System.Windows.Forms.RadioButton();
            this.titleRbtn = new System.Windows.Forms.RadioButton();
            this.label2 = new System.Windows.Forms.Label();
            this.SeriesNameListBox = new System.Windows.Forms.ListBox();
            this.overviewTextBox = new System.Windows.Forms.TextBox();
            this.exitButton = new System.Windows.Forms.Button();
            this.AcceptBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.SeriesPosterPictureBox)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(346, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Please Select Correct Series From the List Below";
            // 
            // SeriesPosterPictureBox
            // 
            this.SeriesPosterPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.SeriesPosterPictureBox.ImageLocation = "";
            this.SeriesPosterPictureBox.Location = new System.Drawing.Point(424, 12);
            this.SeriesPosterPictureBox.Name = "SeriesPosterPictureBox";
            this.SeriesPosterPictureBox.Size = new System.Drawing.Size(216, 311);
            this.SeriesPosterPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.SeriesPosterPictureBox.TabIndex = 1;
            this.SeriesPosterPictureBox.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.groupBox1.Controls.Add(this.yearRbtn);
            this.groupBox1.Controls.Add(this.titleRbtn);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.groupBox1.Location = new System.Drawing.Point(12, 48);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(168, 28);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // yearRbtn
            // 
            this.yearRbtn.AutoSize = true;
            this.yearRbtn.ForeColor = System.Drawing.Color.GreenYellow;
            this.yearRbtn.Location = new System.Drawing.Point(112, 8);
            this.yearRbtn.Name = "yearRbtn";
            this.yearRbtn.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.yearRbtn.Size = new System.Drawing.Size(51, 17);
            this.yearRbtn.TabIndex = 3;
            this.yearRbtn.Text = "Year";
            this.yearRbtn.UseVisualStyleBackColor = true;
            this.yearRbtn.Click += new System.EventHandler(this.YearRbtn_Click);
            // 
            // titleRbtn
            // 
            this.titleRbtn.AutoSize = true;
            this.titleRbtn.Checked = true;
            this.titleRbtn.ForeColor = System.Drawing.Color.GreenYellow;
            this.titleRbtn.Location = new System.Drawing.Point(59, 8);
            this.titleRbtn.Name = "titleRbtn";
            this.titleRbtn.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.titleRbtn.Size = new System.Drawing.Size(50, 17);
            this.titleRbtn.TabIndex = 3;
            this.titleRbtn.TabStop = true;
            this.titleRbtn.Text = "Title";
            this.titleRbtn.UseVisualStyleBackColor = true;
            this.titleRbtn.Click += new System.EventHandler(this.DefaultRbtn_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.GreenYellow;
            this.label2.Location = new System.Drawing.Point(6, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(52, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Sort By:";
            // 
            // SeriesNameListBox
            // 
            this.SeriesNameListBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.SeriesNameListBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.SeriesNameListBox.FormattingEnabled = true;
            this.SeriesNameListBox.Location = new System.Drawing.Point(12, 82);
            this.SeriesNameListBox.Name = "SeriesNameListBox";
            this.SeriesNameListBox.Size = new System.Drawing.Size(336, 407);
            this.SeriesNameListBox.TabIndex = 3;
            this.SeriesNameListBox.SelectedIndexChanged += new System.EventHandler(this.SeriesNameListBox_SelectedIndexChanged);
            // 
            // overviewTextBox
            // 
            this.overviewTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.overviewTextBox.ForeColor = System.Drawing.Color.GreenYellow;
            this.overviewTextBox.Location = new System.Drawing.Point(354, 329);
            this.overviewTextBox.Multiline = true;
            this.overviewTextBox.Name = "overviewTextBox";
            this.overviewTextBox.Size = new System.Drawing.Size(357, 160);
            this.overviewTextBox.TabIndex = 4;
            this.overviewTextBox.Text = "Overview";
            // 
            // exitButton
            // 
            this.exitButton.BackColor = System.Drawing.Color.GreenYellow;
            this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.exitButton.Location = new System.Drawing.Point(12, 495);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(207, 42);
            this.exitButton.TabIndex = 5;
            this.exitButton.Text = "&Exit";
            this.exitButton.UseVisualStyleBackColor = false;
            this.exitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // AcceptBtn
            // 
            this.AcceptBtn.BackColor = System.Drawing.Color.GreenYellow;
            this.AcceptBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.AcceptBtn.Location = new System.Drawing.Point(466, 495);
            this.AcceptBtn.Name = "AcceptBtn";
            this.AcceptBtn.Size = new System.Drawing.Size(207, 42);
            this.AcceptBtn.TabIndex = 6;
            this.AcceptBtn.Text = "&Accept";
            this.AcceptBtn.UseVisualStyleBackColor = false;
            this.AcceptBtn.Click += new System.EventHandler(this.AcceptBtn_Click);
            // 
            // TVSeriesSelection
            // 
            this.AcceptButton = this.AcceptBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.CancelButton = this.exitButton;
            this.ClientSize = new System.Drawing.Size(723, 549);
            this.Controls.Add(this.AcceptBtn);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.overviewTextBox);
            this.Controls.Add(this.SeriesNameListBox);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.SeriesPosterPictureBox);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.GreenYellow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TVSeriesSelection";
            this.Text = "TV Series Selection";
            ((System.ComponentModel.ISupportInitialize)(this.SeriesPosterPictureBox)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.PictureBox SeriesPosterPictureBox;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton yearRbtn;
        private System.Windows.Forms.RadioButton titleRbtn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ListBox SeriesNameListBox;
        private System.Windows.Forms.TextBox overviewTextBox;
        private System.Windows.Forms.Button exitButton;
        private System.Windows.Forms.Button AcceptBtn;
    }
}