namespace CrossWord.Forms
{
    partial class FinalCrosswordBoard
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
            this.lblAcross = new System.Windows.Forms.Label();
            this.lblDown = new System.Windows.Forms.Label();
            this.txtAcross = new System.Windows.Forms.TextBox();
            this.txtDown = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveCrosswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblAcross
            // 
            this.lblAcross.AutoSize = true;
            this.lblAcross.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAcross.ForeColor = System.Drawing.Color.Firebrick;
            this.lblAcross.Location = new System.Drawing.Point(982, 23);
            this.lblAcross.Name = "lblAcross";
            this.lblAcross.Size = new System.Drawing.Size(106, 31);
            this.lblAcross.TabIndex = 5;
            this.lblAcross.Text = "Across:";
            // 
            // lblDown
            // 
            this.lblDown.AutoSize = true;
            this.lblDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDown.ForeColor = System.Drawing.Color.Firebrick;
            this.lblDown.Location = new System.Drawing.Point(982, 448);
            this.lblDown.Name = "lblDown";
            this.lblDown.Size = new System.Drawing.Size(92, 31);
            this.lblDown.TabIndex = 7;
            this.lblDown.Text = "Down:";
            // 
            // txtAcross
            // 
            this.txtAcross.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAcross.Location = new System.Drawing.Point(988, 62);
            this.txtAcross.Multiline = true;
            this.txtAcross.Name = "txtAcross";
            this.txtAcross.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtAcross.Size = new System.Drawing.Size(395, 388);
            this.txtAcross.TabIndex = 8;
            this.txtAcross.TabStop = false;
            // 
            // txtDown
            // 
            this.txtDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDown.Location = new System.Drawing.Point(988, 487);
            this.txtDown.Multiline = true;
            this.txtDown.Name = "txtDown";
            this.txtDown.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtDown.Size = new System.Drawing.Size(395, 388);
            this.txtDown.TabIndex = 9;
            this.txtDown.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1414, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveCrosswordToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // saveCrosswordToolStripMenuItem
            // 
            this.saveCrosswordToolStripMenuItem.Name = "saveCrosswordToolStripMenuItem";
            this.saveCrosswordToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.saveCrosswordToolStripMenuItem.Text = "Save &Crossword";
            this.saveCrosswordToolStripMenuItem.Click += new System.EventHandler(this.saveCrosswordToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.closeToolStripMenuItem.Text = "&Close";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // FinalCrosswordBoard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1414, 943);
            this.Controls.Add(this.txtDown);
            this.Controls.Add(this.txtAcross);
            this.Controls.Add(this.lblDown);
            this.Controls.Add(this.lblAcross);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FinalCrosswordBoard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FinalCrosswordBoard";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.FinalCrosswordBoard_Paint);
            this.Resize += new System.EventHandler(this.FinalCrosswordBoard_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblAcross;
        private System.Windows.Forms.Label lblDown;
        private System.Windows.Forms.TextBox txtAcross;
        private System.Windows.Forms.TextBox txtDown;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveCrosswordToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
    }
}