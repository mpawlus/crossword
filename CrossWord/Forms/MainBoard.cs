// ***************************************************************************************************************************************
// ****** Mehedi Shams Rony: Nov 2018*****************************************************************************************************
// ****** Purpose: Main crossword board for showing words in the matrix. *****************************************************************
// ***************************************************************************************************************************************
//http://unicode.org/faq/bengali.html
//http://www.unicode.org/charts/PDF/U0980.pdf
//http://www.unicode.org/versions/Unicode11.0.0/ch12.pdf#G664195
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using WordPuzzle.Classes;
using System.Configuration;
using CrossWord.Classes;
using CrossWord.Interfaces;
using System.Text;

namespace CrossWord.Forms
{
    public partial class MainBoard : Form
    {
        GameEngine engine;
        BanglaUnicodeGameEngine banglaWordsEngine;                
        Globals.CurrentWordType regularOrUnicode;

        WindowScaler scaler;
        int scale;
        int calibration;

        private const int COLUMN_WORD_WIDTH_SCALE_FACTOR = 13;
        private const int COLUMN_WORDMEANING_WIDTH_SCALE_FACTOR = 5;

        private readonly Color LegendBackColorFails = Color.LightBlue;
        private readonly Color LegendBackColorIsolates = Color.LightSeaGreen;
        private readonly Color ListViewBackColorLongClues = Color.LightCoral;

        private readonly int MAX_WORDS = Convert.ToInt16(ConfigurationManager.AppSettings["MAX_WORDS"]);
        private readonly int MAX_WORD_MEANING_LENGTH = Convert.ToInt16(ConfigurationManager.AppSettings["MAX_WORD_MEANING_LENGTH"]);        

        public string JSON_ENGLISH_WORD_DICTIONARY_FILE_NAME = @"..\..\Words\Words.json";
        public string JSON_BANGLA_WORD_DICTIONARY_FILE_NAME = @"..\..\Words\Bangla.json";

        public string selectedWord, selectedWordMeaning;
        public bool wordChanged;

        Dictionary<string, string> wordsAndMeaning;
        private List<KeyValuePair<string, string>> currentWordList;
        
        List<Point> CellsWithColour = new List<Point>();
        Random rnd;
        private enum MapOrErase { Map = 1, Erase }

        /// <summary>
        /// Constructor - initiates the randomizer, loads legends, adjusts column width of the list view and launches the main wording process.
        /// </summary>
        public MainBoard()
        {
            InitializeComponent();
            rnd = new Random();
            LoadLegends();
            AdjustWordListView();

            Globals.ScaleBoardIfNecessary(out scaler, Screen.PrimaryScreen.Bounds, out scale, out calibration);
            LaunchMainWordingProcess();
        }

        /// <summary>
        /// Reads words from file, saves them in local collection, take a snapshot of some words from the collection,
        /// populates the listview with the words, instantiates game engines, loads English words, initiates logic application for matrix,
        /// highlight words that failed and are isolated, updates status, paints the board.
        /// </summary>
        private void LaunchMainWordingProcess()
        {
            try
            {
                ReadWordsFromFile(JSON_ENGLISH_WORD_DICTIONARY_FILE_NAME);                
                currentWordList = new List<KeyValuePair<string, string>>();
                if (!SnapWordsInDictionary()) return;

                PopulateListViewWithCurrentWords();
                engine = new GameEngine();
                banglaWordsEngine = new BanglaUnicodeGameEngine();
                regularOrUnicode = Globals.CurrentWordType.Regular;
                engine.PlaceWordsOnTheBoard(currentWordList);
                HighlightFailedAndIsolatedWords();
                UpdateStatus();
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'LaunchMainWordingProcess()' method of 'MainBoard' form. Error msg: {e.Message}");
            }
        }

        /// <summary>
        /// Highlight the words that have long meanings that are advised to reduce.
        /// </summary>
        private void HighlightWordsWithLongMeaningsThatShouldBeReduced()
        {
            try
            {
                foreach (ListViewItem itm in lvwWords.Items)
                    if (itm.SubItems[2].Text.Length > MAX_WORD_MEANING_LENGTH)
                        itm.BackColor = ListViewBackColorLongClues;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'HighlightWordsWithLongMeaningsThatShouldBeReduced()' method of 'MainBoard' form. Error msg: {e.Message}");
            }
        }

        /// <summary>
        /// Sets up the column widths of the list view.
        /// </summary>
        private void AdjustWordListView()
        {
            try
            {
                lvwWords.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.None);             // The number sequence - no scaling.
                lvwWords.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent);    // The word - scale accordingly to the content.
                lvwWords.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);    // The meaning - scale accordingly to the content.
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'AdjustWordListView()' method of 'MainBoard' form. Error msg: {e.Message}");
            }         
        }

        /// <summary>
        /// Initializes the legends with respective messages.
        /// </summary>
        private void LoadLegends()
        {
            try
            {
                lblLegendFails.Text = $"Words that could not be placed after {string.Format("{0:#,#}", Globals.MAX_ATTEMPTS)} attempts.";
                lblTooLongMeaning.Text = $"Words that have lengthy meaning (clues) ({MAX_WORD_MEANING_LENGTH} chars), and a reduction is advised.";
                lblLegendIsolates.Text = $"Words that didn't CROSS another WORD during the shuffle; hence will be removed from the final board.";
                pictureBoxLegendFails.Width = pictureBoxLegendFails.Height = pictureBoxLegendIsolates.Width = pictureBoxLegendIsolates.Height =
                    pictureBoxLegendTooLongMeaning.Width = pictureBoxLegendTooLongMeaning.Height = 20;

                pictureBoxLegendFails.BackColor = LegendBackColorFails;
                pictureBoxLegendIsolates.BackColor = LegendBackColorIsolates;
                pictureBoxLegendTooLongMeaning.BackColor = ListViewBackColorLongClues;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'LoadLegends()' method of 'MainBoard' form. Error msg: {e.Message}");
            }
        }

        /// <summary>
        /// Populates the listview with current words in the list collection.
        /// </summary>
        private void PopulateListViewWithCurrentWords()
        {
            ListViewItem lvwItem;
            ListViewItem.ListViewSubItem lvwSubItem;
            
            int maxLengthWord = 0, maxLengthWordMeaning = 0;
            try
            {
                foreach (KeyValuePair<string, string> word in currentWordList)
                {
                    if (word.Key.Length > maxLengthWord)
                        maxLengthWord = word.Key.Length;

                    if (word.Value.Length > maxLengthWordMeaning)
                        maxLengthWordMeaning = word.Value.Length;
                }

                lvwWords.Columns[1].Width = maxLengthWord * COLUMN_WORD_WIDTH_SCALE_FACTOR;
                lvwWords.Columns[2].Width = maxLengthWordMeaning * COLUMN_WORDMEANING_WIDTH_SCALE_FACTOR;

                int i = 1;
                foreach (KeyValuePair<string, string> word in currentWordList)
                {
                    lvwItem = lvwWords.Items.Add(word.Key, (i++).ToString(), -1);   // No overload available to add text and key only; need to supply a dummy value for image index.
                    lvwSubItem = lvwItem.SubItems.Add(word.Key);
                    lvwSubItem = lvwItem.SubItems.Add(word.Value);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'PopulateListViewWithCurrentWords()' method of 'MainBoard' form. Error msg: {e.Message}");
            }
        }

        private bool SnapWordsInDictionary()
        {
            if (MAX_WORDS > wordsAndMeaning.Count)
            {
                MessageBox.Show($"There are less words in the dictionary ({wordsAndMeaning.Count}) than the maximum words ({MAX_WORDS}). Please reduce maximum words.");
                return false;
            }
            KeyValuePair<string, string> wordAndMeaning;
            string key;
            try
            {
                for (int i = 0; i < MAX_WORDS; i++)
                {
                    wordAndMeaning = wordsAndMeaning.ElementAt(rnd.Next(0, wordsAndMeaning.Count));        // https://stackoverflow.com/questions/1028136/random-entry-from-dictionary
                    if (regularOrUnicode == Globals.CurrentWordType.Regular)
                    {
                        if (wordAndMeaning.Key.Length > Globals.MAX_WORD_LENGTH) { i--; continue; }
                    }
                    else if (wordAndMeaning.Key.Length > Globals.MAX_UNICODE_WORD_LENGTH) { i--; continue; }

                    // We want to remove any hyphen or space from the key. However we have to change the key before insertion
                    // as after insertion in the list the key becomes read-only.
                    key = wordAndMeaning.Key.Replace("-", string.Empty).Replace(" ", string.Empty);

                    if (currentWordList.FirstOrDefault(x => x.Key == key).Key != null)    // This is a duplicate key (WORD); abort this one and pick another one.
                    {
                        i--;
                        continue;
                    }
                    currentWordList.Add(new KeyValuePair<string, string>(key, wordAndMeaning.Value));
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'SnapWordsInDictionary()' method of 'MainBoard' form. Error msg: {e.Message}");
                return false;
            }
        }

        private void ReadWordsFromFile(string fileName)
        {
            string jsonWords = "";
            wordChanged = false;

            try
            {
                using (StreamReader reader = new StreamReader(fileName))
                    jsonWords = reader.ReadToEnd();
                JObject obj = (JObject)JsonConvert.DeserializeObject(jsonWords);
                wordsAndMeaning = obj.ToObject<Dictionary<string, string>>();
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'ReadWordsFromFile()' method of 'MainBoard' form. Error msg: {e.Message}");
            }
        }
        
        /// <summary>
        /// The paint event of the board. Draws the grid, places any colour if needed, places the words in respective places.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainBoard_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = null;
            try
            {
                pen = new Pen(Color.FromArgb(255, 0, 0, 0));
                ColourCells(CellsWithColour, LegendBackColorFails);
                
                // Draw horizontal lines.
                for (int i = 0; i <= Globals.gridCellCount; i++)
                    e.Graphics.DrawLine(pen, scale, (i + 1) * scale, Globals.gridCellCount * scale + scale, (i + 1) * scale);

                // Draw vertical lines.
                for (int i = 0; i <= Globals.gridCellCount; i++)
                    e.Graphics.DrawLine(pen, (i + 1) * scale, scale, (i + 1) * scale, Globals.gridCellCount * scale + scale);

                if (regularOrUnicode == Globals.CurrentWordType.Regular)
                    RegularMatrixToBoard();
                else UnicodeMatrixToBoard();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in 'MainBoard_Paint()' method of 'MainBoard' form. Error msg: {ex.Message}");
            }
            finally
            {
                pen.Dispose();
            }
        }

        /// <summary>
        /// Paints the regular words on the board.
        /// https://msdn.microsoft.com/en-us/library/9why95hd%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
        /// </summary>
        private void RegularMatrixToBoard()
        {            
            Graphics canvas = null;
            Font font = null;
            SolidBrush brush = null;
            string alphabetToMap;

            try
            {
                canvas = CreateGraphics();
                font = new Font("Arial", scaler.GetMetrics(16));
                brush = new SolidBrush(Color.Black);

                for (int i = 0; i < Globals.gridCellCount; i++)
                    for (int j = 0; j < Globals.gridCellCount; j++)
                    {
                        if (engine.matrix[i, j] != '\0')
                        {
                            alphabetToMap = "" + engine.matrix[i, j]; // "" is needed as a means for conversion of character to string.
                            canvas.DrawString(alphabetToMap, font, brush, (i + 1) * scale + calibration, (j + 1) * scale + calibration);
                        }
                    }
            }
            catch (Exception Ex)
            {
                MessageBox.Show("An error occurred in 'RegularMatrixToBoard()' method of 'GameBoard' form. Error msg: " + Ex.Message);
            }
            finally
            {
                font.Dispose();
                brush.Dispose();
                canvas.Dispose();
            }
        }

        /// <summary>
        /// Paints the unicode words on the board.
        /// https://msdn.microsoft.com/en-us/library/9why95hd%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
        /// </summary>
        private void UnicodeMatrixToBoard()
        {
            Graphics canvas = null;
            Font font = null;
            SolidBrush brush = null;
            string alphabetToMap;

            try
            {
                canvas = CreateGraphics();
                font = new Font("Arial", scaler.GetMetrics(16));
                brush = new SolidBrush(Color.Black);

                for (int i = 0; i < Globals.gridCellCount; i++)
                    for (int j = 0; j < Globals.gridCellCount; j++)
                    {
                        if (banglaWordsEngine.matrix[i, j, 0] != '\0')
                        {
                            alphabetToMap = Globals.GetCompositeLetterFromTheMatrix(i, j, banglaWordsEngine.matrix);
                            canvas.DrawString(alphabetToMap, font, brush, (i + 1) * scale + calibration, (j + 1) * scale + calibration);
                        }
                    }
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'UnicodeMatrixToBoard()' method of 'MainBoard' form. Error msg: {e.Message}");
            }
            finally
            {
                font.Dispose();
                brush.Dispose();
                canvas.Dispose();
            }
        }

        /// <summary>
        /// If a user selects a word, then find the cells of the word in the board and colour the cells.
        /// https://msdn.microsoft.com/en-us/library/ztxk24yx(v=vs.110).aspx
        /// </summary>
        /// <param name="Rect"></param>
        /// <param name="RectColor"></param>        
        private void ColourCells(List<Point> Rect, Color RectColor)
        {
            SolidBrush brush = null;
            Graphics canvas = null;
            try
            {
                brush = new SolidBrush(RectColor);
                canvas = CreateGraphics();
                canvas = CreateGraphics();

                for (int i = 0; i < Rect.Count; i++)
                    canvas.FillRectangle(brush, new Rectangle(Rect[i], new Size(scale, scale)));

                brush.Dispose();
                canvas.Dispose();
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'ColourCells()' method of 'MainBoard' form. Error msg: {e.Message}");
            }
            finally
            {
                brush.Dispose();
                canvas.Dispose();
            }
        }
         
        /// <summary>
        /// User opts for a different assembly, hence shuffle it again.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void reshuffleBoardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Graphics canvas = CreateGraphics();
                CellsWithColour.Clear();
                canvas.Clear(Color.White);
                wordChanged = false;

                if (regularOrUnicode == Globals.CurrentWordType.Regular)
                    engine.PlaceWordsOnTheBoard(currentWordList);
                else banglaWordsEngine.PlaceWordsOnTheBoard(currentWordList);

                Invalidate();
                if (lvwWords.SelectedItems.Count > 0)
                    lvwWords.SelectedItems[0].Selected = false;
                HighlightFailedAndIsolatedWords();
                UpdateStatus();                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in 'reshuffleBoardToolStripMenuItem_Click()' method of the 'MainBoard' class. Error msg: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads a new set of regular words.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadRegularWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                regularOrUnicode = Globals.CurrentWordType.Regular;
                Graphics canvas = CreateGraphics();
                canvas.Clear(Color.White);
                Array.Clear(engine.matrix, 0, engine.matrix.Length);
                CellsWithColour.Clear();

                currentWordList.Clear();
                lvwWords.Items.Clear();
                wordsAndMeaning.Clear();

                ReadWordsFromFile(JSON_ENGLISH_WORD_DICTIONARY_FILE_NAME);
                if (!SnapWordsInDictionary()) return;
                PopulateListViewWithCurrentWords();
                engine.PlaceWordsOnTheBoard(currentWordList);

                Invalidate();
                HighlightFailedAndIsolatedWords();
                UpdateStatus();
                if (lvwWords.SelectedItems.Count > 0)
                    lvwWords.SelectedItems[0].Selected = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in 'reshuffleWordsToolStripMenuItem_Click()' method of the 'MainBoard' class. Error msg: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates status.
        /// </summary>
        private void UpdateStatus()
        {
            try
            {
                int failedCount = 0, isolationCount = 0;
                if (regularOrUnicode == Globals.CurrentWordType.Regular)
                {
                    failedCount = engine.wordDetails.Where(x => x.FailedMaxAttempts == true).Count();
                    isolationCount = engine.wordDetails.Where(x => x.Isolated == true).Count();
                }
                else if (regularOrUnicode == Globals.CurrentWordType.Unicode)
                {
                    failedCount = banglaWordsEngine.wordDetails.Where(x => x.FailedMaxAttempts == true).Count();
                    isolationCount = banglaWordsEngine.wordDetails.Where(x => x.Isolated == true).Count();
                }
                int longMeaningCount = 0;
                foreach (ListViewItem itm in lvwWords.Items)
                    if (itm.SubItems[2].Text.Length > MAX_WORD_MEANING_LENGTH)
                        longMeaningCount++;

                lblTooLongMeaning.Text = $"{longMeaningCount} words have lengthy clues (over than {MAX_WORD_MEANING_LENGTH} chars), and a reduction is advised.";
                lblStatus.Text = $"Placement complete. {failedCount} failed case(s), {isolationCount} isolated case(s);" +
                                 $"remaining {currentWordList.Count - failedCount - isolationCount} words will be on the crossword.";
                lblStatus.Refresh();
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'UpdateStatus()' method of the 'MainBoard' class. Error msg: {e.Message}");
            }
        }

        /// <summary>
        /// Highlights words that failed and are isolated.
        /// </summary>
        private void HighlightFailedAndIsolatedWords()
        {
            try
            {
                // First clear any existing failed back color if any.
                foreach (ListViewItem item in lvwWords.Items)
                    item.BackColor = Color.White;

                // Highlight any word that has longer length than the specified threshold (MAX_WORD_MEANING_LENGTH).
                HighlightWordsWithLongMeaningsThatShouldBeReduced();

                if (regularOrUnicode == Globals.CurrentWordType.Regular)
                {
                    // Now set the back color for isolated words only.
                    foreach (IDetails failedWord in engine.wordDetails.Where(x => x.Isolated == true))
                        lvwWords.Items.Find(failedWord.Word, true)[0].BackColor = LegendBackColorIsolates;

                    // Now set the back color for failed words only.
                    foreach (IDetails failedWord in engine.wordDetails.Where(x => x.FailedMaxAttempts == true))
                        lvwWords.Items.Find(failedWord.Word, true)[0].BackColor = LegendBackColorFails;
                }
                else if (regularOrUnicode == Globals.CurrentWordType.Unicode)
                {
                    // Now set the back color for isolated words only.
                    foreach (IDetails failedWord in banglaWordsEngine.wordDetails.Where(x => x.Isolated == true))
                        lvwWords.Items.Find(failedWord.Word, true)[0].BackColor = LegendBackColorIsolates;

                    // Now set the back color for failed words only.
                    foreach (IDetails failedWord in banglaWordsEngine.wordDetails.Where(x => x.FailedMaxAttempts == true))
                        lvwWords.Items.Find(failedWord.Word, true)[0].BackColor = LegendBackColorFails;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'HighlightFailedAndIsolatedWords()' method of the 'MainBoard' class. Error msg: {e.Message}");
            }
        }

        /// <summary>
        /// Fetch a new word for the current selected word on the listview.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordsListView_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                // https://stackoverflow.com/questions/1028136/random-entry-from-dictionary
                KeyValuePair<string, string> val = wordsAndMeaning.ElementAt(rnd.Next(0, wordsAndMeaning.Count));

                // Make the change in the list view as well as in the word list.
                KeyValuePair<string, string> kvp = currentWordList.Find(x => x.Key == lvwWords.SelectedItems[0].SubItems[1].Text);
                int index = currentWordList.IndexOf(kvp);
                currentWordList.Remove(kvp);
                currentWordList.Insert(index, val);

                lvwWords.SelectedItems[0].SubItems[1].Text = lvwWords.SelectedItems[0].Name = val.Key;
                lvwWords.SelectedItems[0].SubItems[2].Text = val.Value;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in 'WordsListView_DoubleClick()' method of the 'MainBoard' class. Error msg: {ex.Message}");
            }            
        }

        /// <summary>
        /// Scale the board if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainBoard_Load(object sender, EventArgs e)
        {
            try
            {
                Width = scaler.GetMetrics(Width, "Width");                  // Form width and height set up.
                Height = scaler.GetMetrics(Height, "Height");
                Left = Screen.GetBounds(this).Width / 2 - Width / 2;        // Form centering.
                Top = Screen.GetBounds(this).Height / 2 - Height / 2 - 30;  // 30 is a calibration factor.

                foreach (Control ctl in Controls)
                {
                    ctl.Font = new Font(FontFamily.GenericSansSerif, scaler.GetMetrics((int)ctl.Font.Size), FontStyle.Regular);
                    ctl.Width = scaler.GetMetrics(ctl.Width, "Width");
                    ctl.Height = scaler.GetMetrics(ctl.Height, "Height");
                    ctl.Top = scaler.GetMetrics(ctl.Top, "Top");
                    ctl.Left = scaler.GetMetrics(ctl.Left, "Left");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in 'MainBoard_Load()' method of the 'MainBoard' class. Error msg: {ex.Message}");
            }
        }

        /// <summary>
        /// If Populate the 'CellsWithColour' list to obtain the cells where the word belongs.
        /// The invalidate method will repaint the board with the coloured cells.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwWords_Click(object sender, EventArgs e)
        {
            try
            {
                CellsWithColour.Clear();
                if (regularOrUnicode == Globals.CurrentWordType.Regular)
                {
                    RegularWordDetails pos = null;
                    pos = engine.wordDetails.Find(p => p.Word.Equals((sender as ListView).SelectedItems[0].Name));
                    if (pos != null)
                    {
                        if (pos.WordDirection == Direction.Right)                // Across.
                            for (int i = pos.X + 1, j = pos.Y + 1, k = 0; k < pos.Word.Length; i++, k++)
                                CellsWithColour.Add(new Point(i * scale, j * scale));
                        else if (pos.WordDirection == Direction.Down)            // Down.
                            for (int i = pos.X + 1, j = pos.Y + 1, k = 0; k < pos.Word.Length; j++, k++)
                                CellsWithColour.Add(new Point(i * scale, j * scale));
                    }
                }
                else if (regularOrUnicode == Globals.CurrentWordType.Unicode)
                {
                    UnicodeWordDetails pos = null;
                    pos = banglaWordsEngine.wordDetails.Find(p => p.Word.Equals((sender as ListView).SelectedItems[0].Name));
                    if (pos != null)
                    {
                        if (pos.WordDirection == Direction.Right)                // Across.
                            for (int i = pos.X + 1, j = pos.Y + 1, k = 0; k < pos.CompositeUnicodeLetters.Count; i++, k++)
                                CellsWithColour.Add(new Point(i * scale, j * scale));
                        else if (pos.WordDirection == Direction.Down)            // Down.
                            for (int i = pos.X + 1, j = pos.Y + 1, k = 0; k < pos.CompositeUnicodeLetters.Count; j++, k++)
                                CellsWithColour.Add(new Point(i * scale, j * scale));
                    }
                }
                Invalidate();
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in 'lvwWords_Click()' method of the 'MainBoard' class. Error msg: {ex.Message}");
            }
        }

        /// <summary>
        /// Creates the crossword. Summons the crossword form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void createCrosswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (lvwWords.Items.Count == 0) return;
            FinalCrosswordBoard board = null;

            if (wordChanged)
            {
                MessageBox.Show("Word(s) were changed, so a re-shuffle is needed before the crossword is created. Please re-shuffle.");
                return;
            }

            try
            {
                if (regularOrUnicode == Globals.CurrentWordType.Regular)
                {
                    // Now remove all the words that could not be placed on the board or are isolated (didn't CROSS through another WORD).
                    List<RegularWordDetails> failedWords = engine.wordDetails.Where(x => x.FailedMaxAttempts == true || x.Isolated == true).ToList();
                    List<KeyValuePair<string, string>> currentWordListClone = currentWordList.Select(x => x).ToList();
                    List<RegularWordDetails> wordDetailsClone = engine.wordDetails.Select(x => x).ToList();

                    foreach (RegularWordDetails word in failedWords)
                    {
                        currentWordListClone.Remove(currentWordListClone.Find(z => z.Key == word.Word));      // Remove the word from the current word list.
                        wordDetailsClone.Remove(word);                                            // Remove the word position from the position list.
                    }

                    if (failedWords.Count > 0)
                        MessageBox.Show($"{failedWords.Count} words (isolated + failed a placement) will be discarded in the final board.");

                    board = new FinalCrosswordBoard(currentWordListClone, wordDetailsClone, engine.matrix);
                }
                else
                {
                    // Now remove all the words that could not be placed on the board or are isolated (didn't CROSS through another WORD).
                    List<UnicodeWordDetails> failedWords = banglaWordsEngine.wordDetails.Where(x => x.FailedMaxAttempts == true || x.Isolated == true).ToList();
                    List<KeyValuePair<string, string>> currentWordListClone = currentWordList.Select(x => x).ToList();
                    List<UnicodeWordDetails> banglaWordDetailsClone = banglaWordsEngine.wordDetails.Select(x => x).ToList();
                    foreach (UnicodeWordDetails word in failedWords)
                    {
                        currentWordList.Remove(currentWordList.Find(z => z.Key == word.Word));      // Remove the word from the current word list.
                        banglaWordDetailsClone.Remove(word);                                 // Remove the word position from the position list.
                    }

                    if (failedWords.Count > 0)
                        MessageBox.Show($"{failedWords.Count} words (isolated + failed a placement) will be discarded in the final board.");

                    board = new FinalCrosswordBoard(currentWordListClone, banglaWordDetailsClone, banglaWordsEngine.matrix);
                }
                board.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in 'createCrosswordToolStripMenuItem_Click()' method of the 'MainBoard' class. Error msg: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads the Bangla Unicode words.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loadBanglaWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Graphics canvas = CreateGraphics();
                canvas.Clear(Color.White);
                Array.Clear(engine.matrix, 0, engine.matrix.Length);
                CellsWithColour.Clear();

                currentWordList.Clear();
                lvwWords.Items.Clear();
                wordsAndMeaning.Clear();

                ReadWordsFromFile(JSON_BANGLA_WORD_DICTIONARY_FILE_NAME);
                if (!SnapWordsInDictionary()) return;
                PopulateListViewWithCurrentWords();
                banglaWordsEngine.PlaceWordsOnTheBoard(currentWordList);
                regularOrUnicode = Globals.CurrentWordType.Unicode;

                Invalidate();
                HighlightFailedAndIsolatedWords();
                UpdateStatus();
                if (lvwWords.SelectedItems.Count > 0)
                    lvwWords.SelectedItems[0].Selected = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in 'loadBanglaWordsToolStripMenuItem_Click()' method of the 'MainBoard' class. Error msg: {ex.Message}");
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.ShowDialog();
        }

        private void createOwnWordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateAndSaveOwnWords create = new CreateAndSaveOwnWords(this);
            create.ShowDialog();
            if (create.saveSuccess)
            {
                if (create.encoding == CreateAndSaveOwnWords.WordTypes.Regular)
                    loadRegularWordsToolStripMenuItem_Click(sender, e);
                else if (create.encoding == CreateAndSaveOwnWords.WordTypes.Unicode)
                    loadBanglaWordsToolStripMenuItem_Click(sender, e);
            }
        }

        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = Globals.SAVE_FILTER;
            ofd.Title = Globals.OPEN_TITLE;
            DialogResult res = ofd.ShowDialog(this);
            if (res == DialogResult.Cancel) return;

            if (!FileIsValidAndHasEnoughWords(ofd.FileName)) return;

            Encoding fileEncoding = Globals.GetEncoding(ofd.FileName);
            if (fileEncoding == Encoding.ASCII)
            {
                JSON_ENGLISH_WORD_DICTIONARY_FILE_NAME = ofd.FileName;
                loadRegularWordsToolStripMenuItem_Click(sender, e);
            }
            else if (fileEncoding == Encoding.Unicode || fileEncoding == Encoding.UTF8 || fileEncoding == Encoding.UTF7)
            {
                JSON_BANGLA_WORD_DICTIONARY_FILE_NAME = ofd.FileName;
                loadBanglaWordsToolStripMenuItem_Click(sender, e);
            }
        }

        private bool FileIsValidAndHasEnoughWords(string fileName)
        {
            try
            {
                string jsonWords = "";
                using (StreamReader reader = new StreamReader(fileName))
                    jsonWords = reader.ReadToEnd();
                JObject obj = (JObject)JsonConvert.DeserializeObject(jsonWords);
                wordsAndMeaning = obj.ToObject<Dictionary<string, string>>();

                if (MAX_WORDS > wordsAndMeaning.Count)
                {
                    MessageBox.Show($"There are less words in the dictionary ({wordsAndMeaning.Count}) than the maximum words ({MAX_WORDS})." +
                        $"Please add more words and clues in the file.");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Not a valid JSON file. Error msg: {ex.Message}");
                return false;
            }            
        }

        /// <summary>
        /// Change the word and/or meaning of the current selected word in the listview that was double-clicked.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WordsListView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                EditWord edit = new EditWord(this, lvwWords.SelectedItems[0].SubItems[1].Text, lvwWords.SelectedItems[0].SubItems[2].Text);
                edit.ShowDialog();
                if (!string.IsNullOrEmpty(selectedWord) && !string.IsNullOrEmpty(selectedWordMeaning))
                {
                    // If the word is changed in the edit box, then flag it as a change, so the user can't proceed to create the crossword without re-shuffling it.
                    wordChanged = selectedWord != lvwWords.SelectedItems[0].SubItems[1].Text;

                    // Now remove the entry from the current word list.
                    KeyValuePair <string, string> keyToRemove = currentWordList.Find(x => x.Key == lvwWords.SelectedItems[0].SubItems[1].Text);
                    currentWordList.Remove(keyToRemove);

                    // And add the changed key-value pair in the list.
                    currentWordList.Add(new KeyValuePair<string, string>(selectedWord, selectedWordMeaning));

                    lvwWords.SelectedItems[0].SubItems[1].Text = selectedWord;
                    lvwWords.SelectedItems[0].SubItems[2].Text = selectedWordMeaning;                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in 'WordsListView_DoubleClick()' method of the 'MainBoard' class. Error msg: {ex.Message}");
            }
        }
    }
}