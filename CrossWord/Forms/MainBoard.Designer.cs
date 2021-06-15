namespace CrossWord.Forms
{
    partial class MainBoard
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
            this.components = new System.ComponentModel.Container();
            this.lvwWords = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.WordsListLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.loadRegularWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadBanglaWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reshuffleBoardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createCrosswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createOwnWordsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTipForFailedWords = new System.Windows.Forms.ToolTip(this.components);
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblLegendFails = new System.Windows.Forms.Label();
            this.lblLegendIsolates = new System.Windows.Forms.Label();
            this.pictureBoxLegendFails = new System.Windows.Forms.PictureBox();
            this.pictureBoxLegendIsolates = new System.Windows.Forms.PictureBox();
            this.pictureBoxLegendTooLongMeaning = new System.Windows.Forms.PictureBox();
            this.lblTooLongMeaning = new System.Windows.Forms.Label();
            this.loadFromFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLegendFails)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLegendIsolates)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLegendTooLongMeaning)).BeginInit();
            this.SuspendLayout();
            // 
            // lvwWords
            // 
            this.lvwWords.BackColor = System.Drawing.Color.White;
            this.lvwWords.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvwWords.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader1,
            this.columnHeader2});
            this.lvwWords.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvwWords.ForeColor = System.Drawing.Color.DarkGreen;
            this.lvwWords.FullRowSelect = true;
            this.lvwWords.Location = new System.Drawing.Point(966, 60);
            this.lvwWords.Name = "lvwWords";
            this.lvwWords.Size = new System.Drawing.Size(582, 772);
            this.lvwWords.TabIndex = 3;
            this.lvwWords.UseCompatibleStateImageBehavior = false;
            this.lvwWords.View = System.Windows.Forms.View.Details;
            this.lvwWords.Click += new System.EventHandler(this.lvwWords_Click);
            this.lvwWords.DoubleClick += new System.EventHandler(this.WordsListView_DoubleClick);
            this.lvwWords.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.WordsListView_KeyPress);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Idx";
            this.columnHeader3.Width = 30;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Word";
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Meaning";
            this.columnHeader2.Width = 200;
            // 
            // WordsListLabel
            // 
            this.WordsListLabel.AutoSize = true;
            this.WordsListLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.WordsListLabel.ForeColor = System.Drawing.Color.Firebrick;
            this.WordsListLabel.Location = new System.Drawing.Point(960, 24);
            this.WordsListLabel.Name = "WordsListLabel";
            this.WordsListLabel.Size = new System.Drawing.Size(150, 31);
            this.WordsListLabel.TabIndex = 4;
            this.WordsListLabel.Text = "Words List:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadRegularWordsToolStripMenuItem,
            this.loadBanglaWordsToolStripMenuItem,
            this.reshuffleBoardToolStripMenuItem,
            this.createCrosswordToolStripMenuItem,
            this.createOwnWordsToolStripMenuItem,
            this.loadFromFileToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1360, 24);
            this.menuStrip1.TabIndex = 5;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // loadRegularWordsToolStripMenuItem
            // 
            this.loadRegularWordsToolStripMenuItem.Name = "loadRegularWordsToolStripMenuItem";
            this.loadRegularWordsToolStripMenuItem.Size = new System.Drawing.Size(125, 20);
            this.loadRegularWordsToolStripMenuItem.Text = "Load &Regular Words";
            this.loadRegularWordsToolStripMenuItem.Click += new System.EventHandler(this.loadRegularWordsToolStripMenuItem_Click);
            // 
            // loadBanglaWordsToolStripMenuItem
            // 
            this.loadBanglaWordsToolStripMenuItem.Name = "loadBanglaWordsToolStripMenuItem";
            this.loadBanglaWordsToolStripMenuItem.Size = new System.Drawing.Size(131, 20);
            this.loadBanglaWordsToolStripMenuItem.Text = "Load &Bangla Unicode";
            this.loadBanglaWordsToolStripMenuItem.Click += new System.EventHandler(this.loadBanglaWordsToolStripMenuItem_Click);
            // 
            // reshuffleBoardToolStripMenuItem
            // 
            this.reshuffleBoardToolStripMenuItem.Name = "reshuffleBoardToolStripMenuItem";
            this.reshuffleBoardToolStripMenuItem.Size = new System.Drawing.Size(102, 20);
            this.reshuffleBoardToolStripMenuItem.Text = "&Reshuffle Board";
            this.reshuffleBoardToolStripMenuItem.Click += new System.EventHandler(this.reshuffleBoardToolStripMenuItem_Click);
            // 
            // createCrosswordToolStripMenuItem
            // 
            this.createCrosswordToolStripMenuItem.Name = "createCrosswordToolStripMenuItem";
            this.createCrosswordToolStripMenuItem.Size = new System.Drawing.Size(112, 20);
            this.createCrosswordToolStripMenuItem.Text = "&Create Crossword";
            this.createCrosswordToolStripMenuItem.Click += new System.EventHandler(this.createCrosswordToolStripMenuItem_Click);
            // 
            // createOwnWordsToolStripMenuItem
            // 
            this.createOwnWordsToolStripMenuItem.Name = "createOwnWordsToolStripMenuItem";
            this.createOwnWordsToolStripMenuItem.Size = new System.Drawing.Size(118, 20);
            this.createOwnWordsToolStripMenuItem.Text = "Create &Own Words";
            this.createOwnWordsToolStripMenuItem.Click += new System.EventHandler(this.createOwnWordsToolStripMenuItem_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "&About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // toolTipForFailedWords
            // 
            this.toolTipForFailedWords.ShowAlways = true;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.Location = new System.Drawing.Point(963, 847);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(71, 17);
            this.lblStatus.TabIndex = 6;
            this.lblStatus.Text = "lblStatus";
            // 
            // lblLegendFails
            // 
            this.lblLegendFails.AutoSize = true;
            this.lblLegendFails.Location = new System.Drawing.Point(991, 877);
            this.lblLegendFails.Name = "lblLegendFails";
            this.lblLegendFails.Size = new System.Drawing.Size(74, 13);
            this.lblLegendFails.TabIndex = 7;
            this.lblLegendFails.Text = "lblLegendFails";
            // 
            // lblLegendIsolates
            // 
            this.lblLegendIsolates.AutoSize = true;
            this.lblLegendIsolates.Location = new System.Drawing.Point(991, 899);
            this.lblLegendIsolates.Name = "lblLegendIsolates";
            this.lblLegendIsolates.Size = new System.Drawing.Size(89, 13);
            this.lblLegendIsolates.TabIndex = 8;
            this.lblLegendIsolates.Text = "lblLegendIsolates";
            // 
            // pictureBoxLegendFails
            // 
            this.pictureBoxLegendFails.Location = new System.Drawing.Point(966, 877);
            this.pictureBoxLegendFails.Name = "pictureBoxLegendFails";
            this.pictureBoxLegendFails.Size = new System.Drawing.Size(19, 17);
            this.pictureBoxLegendFails.TabIndex = 9;
            this.pictureBoxLegendFails.TabStop = false;
            // 
            // pictureBoxLegendIsolates
            // 
            this.pictureBoxLegendIsolates.Location = new System.Drawing.Point(966, 896);
            this.pictureBoxLegendIsolates.Name = "pictureBoxLegendIsolates";
            this.pictureBoxLegendIsolates.Size = new System.Drawing.Size(19, 17);
            this.pictureBoxLegendIsolates.TabIndex = 10;
            this.pictureBoxLegendIsolates.TabStop = false;
            // 
            // pictureBoxLegendTooLongMeaning
            // 
            this.pictureBoxLegendTooLongMeaning.Location = new System.Drawing.Point(966, 916);
            this.pictureBoxLegendTooLongMeaning.Name = "pictureBoxLegendTooLongMeaning";
            this.pictureBoxLegendTooLongMeaning.Size = new System.Drawing.Size(19, 17);
            this.pictureBoxLegendTooLongMeaning.TabIndex = 12;
            this.pictureBoxLegendTooLongMeaning.TabStop = false;
            // 
            // lblTooLongMeaning
            // 
            this.lblTooLongMeaning.AutoSize = true;
            this.lblTooLongMeaning.Location = new System.Drawing.Point(991, 919);
            this.lblTooLongMeaning.Name = "lblTooLongMeaning";
            this.lblTooLongMeaning.Size = new System.Drawing.Size(101, 13);
            this.lblTooLongMeaning.TabIndex = 11;
            this.lblTooLongMeaning.Text = "lblTooLongMeaning";
            // 
            // loadFromFileToolStripMenuItem
            // 
            this.loadFromFileToolStripMenuItem.Name = "loadFromFileToolStripMenuItem";
            this.loadFromFileToolStripMenuItem.Size = new System.Drawing.Size(95, 20);
            this.loadFromFileToolStripMenuItem.Text = "Load from &File";
            this.loadFromFileToolStripMenuItem.Click += new System.EventHandler(this.loadFromFileToolStripMenuItem_Click);
            // 
            // MainBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1360, 975);
            this.Controls.Add(this.pictureBoxLegendTooLongMeaning);
            this.Controls.Add(this.lblTooLongMeaning);
            this.Controls.Add(this.pictureBoxLegendIsolates);
            this.Controls.Add(this.pictureBoxLegendFails);
            this.Controls.Add(this.lblLegendIsolates);
            this.Controls.Add(this.lblLegendFails);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.WordsListLabel);
            this.Controls.Add(this.lvwWords);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainBoard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Crossword Puzzle Creator";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainBoard_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MainBoard_Paint);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLegendFails)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLegendIsolates)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBoxLegendTooLongMeaning)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ListView lvwWords;
        private System.Windows.Forms.Label WordsListLabel;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolTip toolTipForFailedWords;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblLegendFails;
        private System.Windows.Forms.Label lblLegendIsolates;
        private System.Windows.Forms.PictureBox pictureBoxLegendFails;
        private System.Windows.Forms.PictureBox pictureBoxLegendIsolates;
        private System.Windows.Forms.ToolStripMenuItem reshuffleBoardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadRegularWordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createCrosswordToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBoxLegendTooLongMeaning;
        private System.Windows.Forms.Label lblTooLongMeaning;
        private System.Windows.Forms.ToolStripMenuItem loadBanglaWordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem createOwnWordsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadFromFileToolStripMenuItem;
    }
}

