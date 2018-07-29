namespace MovieDataCollector
{
    partial class TVManualRename
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TVManualRename));
            this.originalLB = new System.Windows.Forms.Label();
            this.newLB = new System.Windows.Forms.Label();
            this.originalTB = new System.Windows.Forms.TextBox();
            this.renameCB = new System.Windows.Forms.ComboBox();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.filterBtn = new System.Windows.Forms.Button();
            this.acceptBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // originalLB
            // 
            this.originalLB.AutoSize = true;
            this.originalLB.Location = new System.Drawing.Point(11, 9);
            this.originalLB.Name = "originalLB";
            this.originalLB.Size = new System.Drawing.Size(104, 13);
            this.originalLB.TabIndex = 0;
            this.originalLB.Text = "Original Filename";
            // 
            // newLB
            // 
            this.newLB.AutoSize = true;
            this.newLB.Location = new System.Drawing.Point(12, 60);
            this.newLB.Name = "newLB";
            this.newLB.Size = new System.Drawing.Size(86, 13);
            this.newLB.TabIndex = 1;
            this.newLB.Text = "New Filename";
            // 
            // originalTB
            // 
            this.originalTB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.originalTB.ForeColor = System.Drawing.Color.GreenYellow;
            this.originalTB.Location = new System.Drawing.Point(14, 25);
            this.originalTB.Name = "originalTB";
            this.originalTB.Size = new System.Drawing.Size(816, 20);
            this.originalTB.TabIndex = 2;
            // 
            // renameCB
            // 
            this.renameCB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.renameCB.ForeColor = System.Drawing.Color.GreenYellow;
            this.renameCB.FormattingEnabled = true;
            this.renameCB.Location = new System.Drawing.Point(13, 76);
            this.renameCB.Name = "renameCB";
            this.renameCB.Size = new System.Drawing.Size(734, 21);
            this.renameCB.TabIndex = 3;
            // 
            // cancelBtn
            // 
            this.cancelBtn.BackColor = System.Drawing.Color.GreenYellow;
            this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.cancelBtn.Location = new System.Drawing.Point(12, 103);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(160, 33);
            this.cancelBtn.TabIndex = 4;
            this.cancelBtn.Text = "&Exit";
            this.cancelBtn.UseVisualStyleBackColor = false;
            this.cancelBtn.Click += new System.EventHandler(this.CancelBtn_Click_1);
            // 
            // filterBtn
            // 
            this.filterBtn.BackColor = System.Drawing.Color.GreenYellow;
            this.filterBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.filterBtn.Location = new System.Drawing.Point(756, 74);
            this.filterBtn.Name = "filterBtn";
            this.filterBtn.Size = new System.Drawing.Size(75, 23);
            this.filterBtn.TabIndex = 5;
            this.filterBtn.Text = "Filter";
            this.filterBtn.UseVisualStyleBackColor = false;
            this.filterBtn.Click += new System.EventHandler(this.FilterBtn_Click_1);
            // 
            // acceptBtn
            // 
            this.acceptBtn.BackColor = System.Drawing.Color.GreenYellow;
            this.acceptBtn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.acceptBtn.Location = new System.Drawing.Point(670, 103);
            this.acceptBtn.Name = "acceptBtn";
            this.acceptBtn.Size = new System.Drawing.Size(160, 33);
            this.acceptBtn.TabIndex = 6;
            this.acceptBtn.Text = "&Accept";
            this.acceptBtn.UseVisualStyleBackColor = false;
            this.acceptBtn.Click += new System.EventHandler(this.AcceptBtn_Click_1);
            // 
            // TVManualRename
            // 
            this.AcceptButton = this.acceptBtn;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.CancelButton = this.cancelBtn;
            this.ClientSize = new System.Drawing.Size(843, 145);
            this.Controls.Add(this.acceptBtn);
            this.Controls.Add(this.filterBtn);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.renameCB);
            this.Controls.Add(this.originalTB);
            this.Controls.Add(this.newLB);
            this.Controls.Add(this.originalLB);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.GreenYellow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TVManualRename";
            this.Text = "Manual Filename Entry";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label originalLB;
        private System.Windows.Forms.Label newLB;
        private System.Windows.Forms.TextBox originalTB;
        private System.Windows.Forms.ComboBox renameCB;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.Button filterBtn;
        private System.Windows.Forms.Button acceptBtn;
    }
}