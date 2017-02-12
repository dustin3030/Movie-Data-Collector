namespace MovieDataCollector
{
    partial class MovieSelection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MovieSelection));
            this.moviesLB = new System.Windows.Forms.ListBox();
            this.overviewTB = new System.Windows.Forms.RichTextBox();
            this.exitBtn = new System.Windows.Forms.Button();
            this.acceptBtn = new System.Windows.Forms.Button();
            this.infoLabel = new System.Windows.Forms.Label();
            this.backdropPB = new System.Windows.Forms.PictureBox();
            this.posterPB = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.PopularityRBtn = new System.Windows.Forms.RadioButton();
            this.releaseYearRBtn = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this.backdropPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.posterPB)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // moviesLB
            // 
            this.moviesLB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.moviesLB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.moviesLB.ForeColor = System.Drawing.Color.GreenYellow;
            this.moviesLB.FormattingEnabled = true;
            this.moviesLB.Location = new System.Drawing.Point(12, 316);
            this.moviesLB.Name = "moviesLB";
            this.moviesLB.Size = new System.Drawing.Size(369, 225);
            this.moviesLB.TabIndex = 3;
            this.moviesLB.SelectedIndexChanged += new System.EventHandler(this.moviesLB_SelectedIndexChanged);
            // 
            // overviewTB
            // 
            this.overviewTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.overviewTB.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.overviewTB.ForeColor = System.Drawing.Color.GreenYellow;
            this.overviewTB.Location = new System.Drawing.Point(397, 317);
            this.overviewTB.Name = "overviewTB";
            this.overviewTB.Size = new System.Drawing.Size(270, 223);
            this.overviewTB.TabIndex = 3;
            this.overviewTB.Text = "overviewTB";
            // 
            // exitBtn
            // 
            this.exitBtn.BackColor = System.Drawing.Color.GreenYellow;
            this.exitBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.exitBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.exitBtn.Location = new System.Drawing.Point(12, 547);
            this.exitBtn.Name = "exitBtn";
            this.exitBtn.Size = new System.Drawing.Size(270, 62);
            this.exitBtn.TabIndex = 4;
            this.exitBtn.Text = "Exit";
            this.exitBtn.UseVisualStyleBackColor = false;
            this.exitBtn.Click += new System.EventHandler(this.exitBtn_Click);
            // 
            // acceptBtn
            // 
            this.acceptBtn.BackColor = System.Drawing.Color.GreenYellow;
            this.acceptBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.acceptBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.acceptBtn.Location = new System.Drawing.Point(397, 546);
            this.acceptBtn.Name = "acceptBtn";
            this.acceptBtn.Size = new System.Drawing.Size(270, 62);
            this.acceptBtn.TabIndex = 5;
            this.acceptBtn.Text = "Accept";
            this.acceptBtn.UseVisualStyleBackColor = false;
            this.acceptBtn.Click += new System.EventHandler(this.acceptBtn_Click);
            // 
            // infoLabel
            // 
            this.infoLabel.AutoSize = true;
            this.infoLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.infoLabel.ForeColor = System.Drawing.Color.GreenYellow;
            this.infoLabel.Location = new System.Drawing.Point(12, 11);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(255, 13);
            this.infoLabel.TabIndex = 0;
            this.infoLabel.Text = "Please select your movie from the list below";
            // 
            // backdropPB
            // 
            this.backdropPB.BackColor = System.Drawing.Color.Transparent;
            this.backdropPB.Location = new System.Drawing.Point(180, 40);
            this.backdropPB.Name = "backdropPB";
            this.backdropPB.Size = new System.Drawing.Size(474, 231);
            this.backdropPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.backdropPB.TabIndex = 1;
            this.backdropPB.TabStop = false;
            // 
            // posterPB
            // 
            this.posterPB.BackColor = System.Drawing.Color.Transparent;
            this.posterPB.Location = new System.Drawing.Point(12, 39);
            this.posterPB.Name = "posterPB";
            this.posterPB.Size = new System.Drawing.Size(162, 232);
            this.posterPB.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.posterPB.TabIndex = 0;
            this.posterPB.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(394, 300);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Overview";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(9, 300);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Movies";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Sort List By:";
            // 
            // PopularityRBtn
            // 
            this.PopularityRBtn.AutoSize = true;
            this.PopularityRBtn.Checked = true;
            this.PopularityRBtn.Location = new System.Drawing.Point(75, 14);
            this.PopularityRBtn.Name = "PopularityRBtn";
            this.PopularityRBtn.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.PopularityRBtn.Size = new System.Drawing.Size(71, 17);
            this.PopularityRBtn.TabIndex = 1;
            this.PopularityRBtn.TabStop = true;
            this.PopularityRBtn.Text = "Popularity";
            this.PopularityRBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.PopularityRBtn.UseVisualStyleBackColor = true;
            this.PopularityRBtn.Click += new System.EventHandler(this.PopularityRBtn_Click);
            // 
            // releaseYearRBtn
            // 
            this.releaseYearRBtn.AutoSize = true;
            this.releaseYearRBtn.Location = new System.Drawing.Point(152, 14);
            this.releaseYearRBtn.Name = "releaseYearRBtn";
            this.releaseYearRBtn.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.releaseYearRBtn.Size = new System.Drawing.Size(89, 17);
            this.releaseYearRBtn.TabIndex = 2;
            this.releaseYearRBtn.Text = "Release Year";
            this.releaseYearRBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.releaseYearRBtn.UseVisualStyleBackColor = true;
            this.releaseYearRBtn.Click += new System.EventHandler(this.releaseYearRBtn_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.releaseYearRBtn);
            this.groupBox1.Controls.Add(this.PopularityRBtn);
            this.groupBox1.Location = new System.Drawing.Point(105, 277);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(276, 36);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            // 
            // MovieSelection
            // 
            this.AcceptButton = this.acceptBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.CancelButton = this.exitBtn;
            this.ClientSize = new System.Drawing.Size(678, 641);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.infoLabel);
            this.Controls.Add(this.acceptBtn);
            this.Controls.Add(this.exitBtn);
            this.Controls.Add(this.overviewTB);
            this.Controls.Add(this.moviesLB);
            this.Controls.Add(this.backdropPB);
            this.Controls.Add(this.posterPB);
            this.ForeColor = System.Drawing.Color.GreenYellow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MovieSelection";
            this.Text = "MovieSelection";
            ((System.ComponentModel.ISupportInitialize)(this.backdropPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.posterPB)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox posterPB;
        private System.Windows.Forms.PictureBox backdropPB;
        private System.Windows.Forms.ListBox moviesLB;
        private System.Windows.Forms.RichTextBox overviewTB;
        private System.Windows.Forms.Button exitBtn;
        private System.Windows.Forms.Button acceptBtn;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton PopularityRBtn;
        private System.Windows.Forms.RadioButton releaseYearRBtn;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}