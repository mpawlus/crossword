using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CrossWord.Forms
{
    public partial class EditWord : Form
    {
        MainBoard form;
        public EditWord(MainBoard formMain, string word, string meaning)
        {
            InitializeComponent();
            txtWord.Text = word;
            txtMeaning.Text = meaning;
            form = formMain;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            form.selectedWord = txtWord.Text;
            form.selectedWordMeaning = txtMeaning.Text;
            Close();
        }

        /// <summary>
        /// For closing the search window by pressing ESC key.
        /// Reference: https://stackoverflow.com/questions/2290959/escape-button-to-close-windows-forms-form-in-c-sharp
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="keyData"></param>
        /// <returns></returns>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                Close();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
