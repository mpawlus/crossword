namespace CrossWord.Forms
{
    partial class CreateAndSaveOwnWords
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
            this.WordsDataGridView = new System.Windows.Forms.DataGridView();
            this.CreateOwnSetLabel = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.saveAndCreateCrosswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.WordsDataGridView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // WordsDataGridView
            // 
            this.WordsDataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.WordsDataGridView.Location = new System.Drawing.Point(51, 66);
            this.WordsDataGridView.Name = "WordsDataGridView";
            this.WordsDataGridView.RowTemplate.Height = 30;
            this.WordsDataGridView.Size = new System.Drawing.Size(1148, 642);
            this.WordsDataGridView.TabIndex = 6;
            this.WordsDataGridView.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.WordsDataGridView_EditingControlShowing);
            // 
            // CreateOwnSetLabel
            // 
            this.CreateOwnSetLabel.AutoSize = true;
            this.CreateOwnSetLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateOwnSetLabel.ForeColor = System.Drawing.Color.Firebrick;
            this.CreateOwnSetLabel.Location = new System.Drawing.Point(45, 32);
            this.CreateOwnSetLabel.Name = "CreateOwnSetLabel";
            this.CreateOwnSetLabel.Size = new System.Drawing.Size(424, 31);
            this.CreateOwnSetLabel.TabIndex = 7;
            this.CreateOwnSetLabel.Text = "Create your own words and clues:";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveAndCreateCrosswordToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1360, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // saveAndCreateCrosswordToolStripMenuItem
            // 
            this.saveAndCreateCrosswordToolStripMenuItem.Name = "saveAndCreateCrosswordToolStripMenuItem";
            this.saveAndCreateCrosswordToolStripMenuItem.Size = new System.Drawing.Size(162, 20);
            this.saveAndCreateCrosswordToolStripMenuItem.Text = "&Save and Create Crossword";
            this.saveAndCreateCrosswordToolStripMenuItem.Click += new System.EventHandler(this.saveAndCreateCrosswordToolStripMenuItem_Click);
            // 
            // CreateAndSaveOwnWords
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1360, 975);
            this.Controls.Add(this.CreateOwnSetLabel);
            this.Controls.Add(this.WordsDataGridView);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateAndSaveOwnWords";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Create and Save Your Own Words";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Resize += new System.EventHandler(this.CreateAndSaveOwnWords_Resize);
            ((System.ComponentModel.ISupportInitialize)(this.WordsDataGridView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView WordsDataGridView;
        private System.Windows.Forms.Label CreateOwnSetLabel;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem saveAndCreateCrosswordToolStripMenuItem;
    }
}