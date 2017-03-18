namespace CubeCam
{
    partial class MainForm
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
      this.videoDisplay = new System.Windows.Forms.PictureBox();
      this.tbResults = new System.Windows.Forms.RichTextBox();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.importScramblesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.saveVideoStreamToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.btnWriteResults = new System.Windows.Forms.Button();
      this.enterScramblesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      ((System.ComponentModel.ISupportInitialize)(this.videoDisplay)).BeginInit();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // videoDisplay
      // 
      this.videoDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.videoDisplay.Location = new System.Drawing.Point(12, 27);
      this.videoDisplay.Name = "videoDisplay";
      this.videoDisplay.Size = new System.Drawing.Size(640, 480);
      this.videoDisplay.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.videoDisplay.TabIndex = 0;
      this.videoDisplay.TabStop = false;
      // 
      // tbResults
      // 
      this.tbResults.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tbResults.BackColor = System.Drawing.SystemColors.Control;
      this.tbResults.Location = new System.Drawing.Point(658, 26);
      this.tbResults.Name = "tbResults";
      this.tbResults.ReadOnly = true;
      this.tbResults.ShowSelectionMargin = true;
      this.tbResults.Size = new System.Drawing.Size(150, 455);
      this.tbResults.TabIndex = 1;
      this.tbResults.Text = "";
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(820, 24);
      this.menuStrip1.TabIndex = 0;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fileToolStripMenuItem
      // 
      this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importScramblesToolStripMenuItem,
            this.enterScramblesToolStripMenuItem,
            this.saveVideoStreamToolStripMenuItem});
      this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
      this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fileToolStripMenuItem.Text = "&File";
      // 
      // importScramblesToolStripMenuItem
      // 
      this.importScramblesToolStripMenuItem.Name = "importScramblesToolStripMenuItem";
      this.importScramblesToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
      this.importScramblesToolStripMenuItem.Text = "&Import Scrambles...";
      this.importScramblesToolStripMenuItem.Click += new System.EventHandler(this.importScramblesToolStripMenuItem_Click);
      // 
      // saveVideoStreamToolStripMenuItem
      // 
      this.saveVideoStreamToolStripMenuItem.Name = "saveVideoStreamToolStripMenuItem";
      this.saveVideoStreamToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
      this.saveVideoStreamToolStripMenuItem.Text = "&Save Video Stream...";
      this.saveVideoStreamToolStripMenuItem.Click += new System.EventHandler(this.saveVideoStreamToolStripMenuItem_Click);
      // 
      // btnWriteResults
      // 
      this.btnWriteResults.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btnWriteResults.Location = new System.Drawing.Point(658, 483);
      this.btnWriteResults.Name = "btnWriteResults";
      this.btnWriteResults.Size = new System.Drawing.Size(150, 23);
      this.btnWriteResults.TabIndex = 2;
      this.btnWriteResults.Text = "Write results to video";
      this.btnWriteResults.UseVisualStyleBackColor = true;
      this.btnWriteResults.Click += new System.EventHandler(this.btnWriteResults_Click);
      this.btnWriteResults.KeyDown += new System.Windows.Forms.KeyEventHandler(this.btnWriteResults_KeyDown);
      // 
      // enterScramblesToolStripMenuItem
      // 
      this.enterScramblesToolStripMenuItem.Name = "enterScramblesToolStripMenuItem";
      this.enterScramblesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
      this.enterScramblesToolStripMenuItem.Text = "&Enter Scrambles...";
      this.enterScramblesToolStripMenuItem.Click += new System.EventHandler(this.enterScramblesToolStripMenuItem_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(820, 535);
      this.Controls.Add(this.btnWriteResults);
      this.Controls.Add(this.tbResults);
      this.Controls.Add(this.videoDisplay);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "MainForm";
      this.Text = "CubeCam";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      ((System.ComponentModel.ISupportInitialize)(this.videoDisplay)).EndInit();
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox videoDisplay;
        private System.Windows.Forms.RichTextBox tbResults;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importScramblesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveVideoStreamToolStripMenuItem;
        private System.Windows.Forms.Button btnWriteResults;
    private System.Windows.Forms.ToolStripMenuItem enterScramblesToolStripMenuItem;
  }
}

