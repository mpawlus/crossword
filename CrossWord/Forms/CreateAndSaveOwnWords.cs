// ***************************************************************************************************************************************
// ****** Mehedi Shams Rony: Nov 2018*****************************************************************************************************
// ****** Purpose: Main crossword board for showing words in the matrix. *****************************************************************
// ***************************************************************************************************************************************
using CrossWord.Classes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WordPuzzle.Classes;

namespace CrossWord.Forms
{
    public partial class CreateAndSaveOwnWords : Form
    {
        WindowScaler scaler;
        int MAX_WORDS = Convert.ToInt16(ConfigurationManager.AppSettings["MAX_WORDS"]);
        int MAX_WORD_LENGTH = Convert.ToInt16(ConfigurationManager.AppSettings["MAX_WORD_LENGTH"]);
        int MAX_WORD_MEANING_LENGTH = Convert.ToInt16(ConfigurationManager.AppSettings["MAX_WORD_MEANING_LENGTH"]);
        private const int MARGIN = 80;
        private int currentColumn = -1;
        public bool saveSuccess = false;
        public WordTypes encoding;
        public enum WordTypes {Regular, Unicode, Mix, Unknown};
        MainBoard main;

        public CreateAndSaveOwnWords(MainBoard mainBoard)
        {
            InitializeComponent();
            main = mainBoard;
            StartPosition = FormStartPosition.CenterScreen;

            scaler = new WindowScaler(Screen.PrimaryScreen.Bounds);
            scaler.SetMultiplicationFactor();

            SetupPlayersWordGrid();
        }

        /// <summary>
        /// Sets up the columns (width, height, colour) of the grid view where the user enters the words.
        /// </summary>
        private void SetupPlayersWordGrid()
        {
            try
            {
                // http://stackoverflow.com/questions/64041/winforms-datagridview-font-size
                WordsDataGridView.DefaultCellStyle.Font = new Font(FontFamily.GenericSansSerif, scaler.GetMetrics(15), FontStyle.Regular);
                WordsDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                WordsDataGridView.RowTemplate.Height = scaler.GetMetrics(30);
                WordsDataGridView.Columns.Add("", "Word");
                WordsDataGridView.Columns[0].HeaderCell.Style.Font = new Font(FontFamily.GenericSansSerif, scaler.GetMetrics(15), FontStyle.Regular);

                WordsDataGridView.Columns.Add("", "Clue");
                WordsDataGridView.Columns[1].HeaderCell.Style.Font = new Font(FontFamily.GenericSansSerif, scaler.GetMetrics(15), FontStyle.Regular);

                // http://stackoverflow.com/questions/5206203/can-i-set-the-max-number-of-rows-in-unbound-datagridview
                WordsDataGridView.UserAddedRow += WordsDataGridView_RowCountChanged;
                WordsDataGridView.UserDeletedRow += WordsDataGridView_RowCountChanged;
                WordsDataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Raised;
                WordsDataGridView.CellBorderStyle = DataGridViewCellBorderStyle.Single;

                // http://stackoverflow.com/questions/2154154/datagridview-how-to-set-column-width
                WordsDataGridView.Columns[0].FillWeight = scaler.GetMetrics(200);
                DataGridViewColumn column = WordsDataGridView.Columns[0];
                column.Width = scaler.GetMetrics(200);
                column.MinimumWidth = scaler.GetMetrics(200);
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderCell.Style.BackColor = Color.Red;

                WordsDataGridView.Columns[1].FillWeight = scaler.GetMetrics(200);
                column = WordsDataGridView.Columns[1];
                column.Width = scaler.GetMetrics(Screen.PrimaryScreen.WorkingArea.Width - WordsDataGridView.Left - MARGIN - 200 * 2);
                column.MinimumWidth = scaler.GetMetrics(Screen.PrimaryScreen.WorkingArea.Width - WordsDataGridView.Left - MARGIN - 200 * 2);
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderCell.Style.BackColor = Color.Red;
            }
            catch (Exception Ex)
            {
                MessageBox.Show("An error occurred in 'SetupPlayersWordGrid()' method of 'CreateAndSaveOwnWords' form. Error msg: " + Ex.Message);
            }
        }

        /// <summary>
        /// Event handler that is triggered when a new row is about to start.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordsDataGridView_RowCountChanged(object sender, EventArgs e)
        {
            CheckRowCount();
        }

        /// <summary>
        /// If rows reach a maximum count, then deny any further row insertion.
        /// </summary>
        private void CheckRowCount()
        {
            if (WordsDataGridView.Rows != null && WordsDataGridView.Rows.Count > Convert.ToInt16(ConfigurationManager.AppSettings["MAX_WORDS"]))
                WordsDataGridView.AllowUserToAddRows = false;
            else if (!WordsDataGridView.AllowUserToAddRows)
                WordsDataGridView.AllowUserToAddRows = true;
        }

        /// <summary>
        /// Custom handler for key press events. Denies any space in the word column, checks and restricts lengths for both words and clues.
        /// https://social.msdn.microsoft.com/Forums/en-US/ecf3dec5-9cba-410b-bef5-8b950d04e6bf/change-the-character-to-upper-case-when-i-keying?forum=csharpgeneral
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>        
        private void WordsDataGridView_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                currentColumn = (sender as DataGridView).CurrentCell.ColumnIndex;
                e.Control.KeyPress -= new KeyPressEventHandler(Control_KeyPress);
                e.Control.KeyPress += new KeyPressEventHandler(Control_KeyPress);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred in 'WordsDataGridView_EditingControlShowing' method of 'CreateAndSaveOwnWords' form. Error msg: " + ex.Message);
            }
        }

        /// <summary>
        /// Custom handler bubbled from WordsDataGridView_EditingControlShowing event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Control_KeyPress(object sender, KeyPressEventArgs e)
        {
            KeyPressValidity(sender, e);
        }

        /// <summary>
        /// Checks the validity of the key that is pressed. Checks space and length.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void KeyPressValidity(object sender, KeyPressEventArgs e)
        {
            try
            {
                TextBox tb = sender as TextBox;
                //if (e.KeyChar == (char)Keys.Enter)
                //{
                //    if (tb.Text.Length <= MIN_LENGTH)   // Checking length
                //    {
                //        MessageBox.Show("Words should be at least " + MAX_LENGTH + " characters long.");
                //        e.Handled = true;
                //        return;
                //    }
                //}
                if (currentColumn == 0)
                {
                    if (tb.Text.Length >= MAX_WORD_LENGTH)   // Checking max length
                    {
                        MessageBox.Show($"Word length cannot be more than {MAX_WORD_LENGTH}.");
                        e.Handled = true;
                        return;
                    }
                    if (e.KeyChar.Equals(' '))  // Checking space; no space allowed. Other invalid characters check can be put here instead of the final check on save button click.
                    {
                        MessageBox.Show("No space, please.");
                        e.Handled = true;
                        return;
                    }
                    e.KeyChar = char.ToUpper(e.KeyChar);
                }
                else if (currentColumn == 1)
                {
                    if (tb.Text.Length >= MAX_WORD_MEANING_LENGTH)   // Checking max length
                    {
                        MessageBox.Show($"Clue length cannot be more than {MAX_WORD_MEANING_LENGTH}.");
                        e.Handled = true;
                        return;
                    }
                }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("An error occurred in 'KeyPressValidity' method of 'CreateAndSaveOwnWords' form. Error msg: " + Ex.Message);
            }
        }

        /// <summary>
        /// Automatic resizing of the grid view.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CreateAndSaveOwnWords_Resize(object sender, EventArgs e)
        {
            WordsDataGridView.Width = Screen.PrimaryScreen.WorkingArea.Width - WordsDataGridView.Left - MARGIN;
            WordsDataGridView.Height = Screen.PrimaryScreen.WorkingArea.Height - WordsDataGridView.Top - MARGIN;
        }

        /// <summary>
        /// Saves the words in a file, makes sure either of regular or unicode is used.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveAndCreateCrosswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (WordsDataGridView.Rows.Count < int.Parse(ConfigurationManager.AppSettings["MAX_NON_OVERLAPPING_WORDS_THRESHOLD"]) * 2)
            {
                MessageBox.Show($"Not enough words. Must be at least {int.Parse(ConfigurationManager.AppSettings["MAX_NON_OVERLAPPING_WORDS_THRESHOLD"]) * 2} words.");
                return;
            }

            // https://stackoverflow.com/questions/1365617/how-to-force-refresh-the-datagridviews-content
            // This is needed as the last cell where the cursor is blinking will not automatically refresh until the cursor is moved to the next row.
            // The EndEdit just forces a refresh, effectively the same as moving the cursor in the next row.
            WordsDataGridView.EndEdit();
            Dictionary<string, string> wordAndClue = CollectWordsFromGrid();
            encoding = GetEncoding(wordAndClue);
            if (encoding == WordTypes.Mix)
            {
                MessageBox.Show("Either regular or unicode is allowed. You can't mix both. Please correct before proceeding.");
                return;
            }

            //https://www.newtonsoft.com/json/help/html/SerializingCollections.htm
            string json = JsonConvert.SerializeObject(wordAndClue, Formatting.Indented);

            SaveFileDialog save = new SaveFileDialog();
            save.Filter = Globals.SAVE_FILTER;
            save.Title = Globals.SAVE_TITLE;
            DialogResult res = save.ShowDialog(this);
            if (res == DialogResult.Cancel) return;

            if (encoding == WordTypes.Regular)
            {
                using (StreamWriter file = new StreamWriter(save.FileName, false))
                {
                    file.Write(json);
                    main.JSON_ENGLISH_WORD_DICTIONARY_FILE_NAME = save.FileName;
                }
            }
            else if (encoding == WordTypes.Unicode)
            {
                using (StreamWriter file = new StreamWriter(save.FileName, false, Encoding.Unicode))
                {
                    file.Write(json);
                    main.JSON_BANGLA_WORD_DICTIONARY_FILE_NAME = save.FileName;
                }
            }
            saveSuccess = true;
            Close();
        }

        /// <summary>
        /// Checks the encoding of the words to make sure they are either regular or unicode.
        /// </summary>
        /// <param name="wordAndClue"></param>
        /// <returns></returns>
        private WordTypes GetEncoding(Dictionary<string, string> wordAndClue)
        {
            WordTypes type = WordTypes.Unknown;
            WordTypes prevType = WordTypes.Unknown;
            foreach (KeyValuePair<string, string> kvp in wordAndClue)
            {
                char[] ch = kvp.Key.ToCharArray();
                if (ch[0] >= 65 && ch[0] <= 255)
                    prevType = WordTypes.Regular;
                else if (ch[0] >= 0x0980 && ch[0] <= 0x09fe)        // Refer to Bangla Unicode chart: http://www.unicode.org/charts/PDF/U0980.pdf, modify the code range for other unicode letters.
                    prevType = WordTypes.Unicode;

                for (int i = 1; i < ch.Length; i++)
                {
                    if (ch[i] >= 65 && ch[i] <= 255)
                        type = WordTypes.Regular;
                    else if (ch[i] >= 0x0980 && ch[i] <= 0x09fe)    // Refer to Bangla Unicode chart: http://www.unicode.org/charts/PDF/U0980.pdf, modify the code range for other unicode letters.
                        prevType = WordTypes.Unicode;

                    if (type != prevType) return WordTypes.Mix;
                    prevType = type;
                }
            }
            return type;
        }

        /// <summary>
        /// Collects the words and clues from the grid view and puts them in a dictionary.
        /// </summary>
        /// <returns>A dictionary object containing words and clues as key-value pairs</returns>
        private Dictionary<string, string> CollectWordsFromGrid()
        {
            Dictionary<string, string> words = new Dictionary<string, string>();
            foreach (DataGridViewRow row in WordsDataGridView.Rows)
            {
                if (row.Cells[0].Value == null || row.Cells[1].Value == null) continue;
                words.Add(row.Cells[0].Value.ToString(), row.Cells[1].Value.ToString());
            }
            return words;
        }
    }
}