// ***************************************************************************************************************************************
// ****** Mehedi Shams Rony: Dec 2018*****************************************************************************************************
// ****** Purpose: Displays the crossword clues, puts numbers and back colors accordingly, and saves the crossword. **********************
// ***************************************************************************************************************************************
using CrossWord.Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WordPuzzle.Classes;
using System.Linq;
using CrossWord.Interfaces;

namespace CrossWord.Forms
{
    public partial class FinalCrosswordBoard : Form
    {
        WindowScaler scaler;
        int scale;
        int calibration;

        Globals.CurrentWordType regularOrUnicode;
        List<KeyValuePair<string, string>> words;
        List<IDetails> wordDetails, answersAcross, answersDown;

        private const int ADJUSTMENT_PIXELS = 30;

        private char[,] matrix;                                 // This is a word matrix to mimic the board.
        private char[,,] unicodeMatrix;                         // This is a unicode word matrix to mimic the board.

        List<Point> BlankCells = new List<Point>();
        private readonly Color BLANK_CELL_COLOUR = Color.LightGray;
        private readonly int CLUE_INDEX_FONT_SIZE = Convert.ToInt16(ConfigurationManager.AppSettings["CLUE_NUMBER_FONT_SIZE"]);
        private const string CROSSWORD_FOLDER = @"Crosswords";

        /// <summary>
        /// Constructor for regular words.
        /// </summary>
        /// <param name="wordList">List of word and meanings</param>
        /// <param name="wordPositions">List of word-details</param>
        /// <param name="wordMatrix">2D Board matrix for regular words</param>
        public FinalCrosswordBoard(List<KeyValuePair<string, string>> wordList, List<RegularWordDetails> wordPositions, char[,] wordMatrix)
        {
            InitializeComponent();
            regularOrUnicode = Globals.CurrentWordType.Regular;                         // Note down current wording mode - regular or unicode.
            matrix = wordMatrix;
            wordDetails = new List<IDetails>(wordPositions.Select(x => x).ToList());    // Clone the word details.
            SetUp(wordList);
        }

        /// <summary>
        /// Constructor for unicode words.
        /// </summary>
        /// <param name="wordList">List of word and meanings</param>
        /// <param name="wordPositions">List of word-details</param>
        /// <param name="wordMatrix">3D Board matrix for unicode words</param>
        public FinalCrosswordBoard(List<KeyValuePair<string, string>> wordList, List<UnicodeWordDetails> wordPositions, char[,,] wordMatrix)
        {
            InitializeComponent();
            regularOrUnicode = Globals.CurrentWordType.Unicode;                         // Note down current wording mode - regular or unicode.
            unicodeMatrix = wordMatrix;
            wordDetails = new List<IDetails>(wordPositions.Select(x => x).ToList());    // Clone the word details.
            SetUp(wordList);
        }

        /// <summary>
        /// Tasks common to both regular and unicode words.
        /// </summary>
        /// <param name="wordList"></param>
        private void SetUp(List<KeyValuePair<string, string>> wordList)
        {
            Globals.ScaleBoardIfNecessary(out scaler, Screen.PrimaryScreen.Bounds, out scale, out calibration);
            calibration -= 8;                                 // To output the number at the top left of the cell instead of the middle of the cell.
            words = wordList;
            answersAcross = new List<IDetails>();
            answersDown = new List<IDetails>();
            
            CreateClues();
            FillBlankCells();
        }

        /// <summary>
        /// Paints the unused cells in gray or whatever background colour is selected.
        /// https://msdn.microsoft.com/en-us/library/ztxk24yx(v=vs.110).aspx
        /// </summary>        
        private void ColourCells()
        {
            SolidBrush brush = null;
            Graphics canvas = null;
            try
            {
                brush = new SolidBrush(BLANK_CELL_COLOUR);
                canvas = CreateGraphics();
                int scale = this.scale - 3;

                for (int i = 0; i < BlankCells.Count; i++)
                    canvas.FillRectangle(brush, new Rectangle(BlankCells[i], new Size(scale, scale)));
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'ColourCells()' method of 'FinalCrosswordBoard' class. Error msg: {e.Message}");
            }
            finally
            {
                brush.Dispose();
                canvas.Dispose();
            }
        }

        /// <summary>
        /// Draws the board, colour the blank cells, and place the numbers.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinalCrosswordBoard_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = null;
            try
            {
                MatrixToBoard();
                ColourCells();
                pen = new Pen(Color.FromArgb(255, 0, 0, 0));

                // Draw horizontal lines.
                for (int i = 0; i <= Globals.gridCellCount; i++)
                    e.Graphics.DrawLine(pen, scale, (i + 1) * scale, Globals.gridCellCount * scale + scale, (i + 1) * scale);

                // Draw vertical lines.
                for (int i = 0; i <= Globals.gridCellCount; i++)
                    e.Graphics.DrawLine(pen, (i + 1) * scale, scale, (i + 1) * scale, Globals.gridCellCount * scale + scale);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in 'FinalCrosswordBoard_Paint()' method of 'FinalCrosswordBoard' form. Error msg: {ex.Message}");
            }
            finally
            {
                pen.Dispose();
            }
        }

        /// <summary>
        /// Creates the clues (word-meanings) and places them in the text boxes.
        /// </summary>
        private void CreateClues()
        {
            StringBuilder across = new StringBuilder(), down = new StringBuilder();
            List<IDetails> detailsCopy = null;
            try
            {                
                int i = 1;
                detailsCopy = new List<IDetails>(wordDetails.Select(x => x).ToList()); // Clone the existing word details list.
                wordDetails.Clear();    // Clear the existing word details.

                // Get all the words that have the same starting axes. E.g.: 17, 3.
                // It is likely that any two words might start at the same index - one heading right, the other heading bottom.
                var wordsStartingAtSameAxes = from j in detailsCopy
                                              group j by new { j.X, j.Y } into d
                                              where d.Count() > 1
                                              select (d).ToList();

                // if at all such word-pairs are found, then 'wordsStartingAtSameAxes' will be a list of a pair of word details.
                foreach (var word in wordsStartingAtSameAxes)
                {
                    word[0].OutputSequence = word[1].OutputSequence = i;    // Both across and down should have the same sequence.

                    // If the first of the pair is a DOWN word, then put the first word-meaning in the textbox for DOWN words.
                    // The second word should obviously be ACROSS; hence put the second word-meaning in the textbox for ACROSS words.
                    if (word[0].WordDirection == Direction.Down)
                    {                        
                        down.Append(i + ") " + words.Find(x => x.Key == word[0].Word).Value + Environment.NewLine);
                        answersDown.Add(word[0]);
                        across.Append(i + ") " + words.Find(x => x.Key == word[1].Word).Value + Environment.NewLine);
                        answersAcross.Add(word[1]);
                    }

                    // If the first of the pair is an ACROSS word, then put the first word-meaning in the textbox for ACROSS words.
                    // The second word should obviously be a DOWN word; hence put the second word-meaning in the textbox for DOWN words.
                    else if (word[0].WordDirection == Direction.Right)
                    {
                        across.Append(i + ") " + words.Find(x => x.Key == word[0].Word).Value + Environment.NewLine);
                        answersAcross.Add(word[0]);
                        down.Append(i + ") " + words.Find(x => x.Key == word[1].Word).Value + Environment.NewLine);
                        answersDown.Add(word[1]);
                    }
                    wordDetails.Add(word[0]);           // The main copy of the word details should have any one of the index; it just needs the indices now for the paint method.
                    detailsCopy.Remove(word[0]);        // Remove the word pairs from the collection, so that they don't get picked up in the next iteration.
                    detailsCopy.Remove(word[1]);
                    i++;
                }

                // After the words starting at the same axes are taken care of, then proceed on to add the remaining word-meanings at the respective boxes.
                foreach (IDetails d in detailsCopy)
                {
                    d.OutputSequence = i;
                    if (d.WordDirection == Direction.Down)
                    {
                        down.Append(i + ") " + words.Find(x => x.Key == d.Word).Value + Environment.NewLine);
                        answersDown.Add(d);
                    }
                    else if (d.WordDirection == Direction.Right)
                    {
                        across.Append(i + ") " + words.Find(x => x.Key == d.Word).Value + Environment.NewLine);
                        answersAcross.Add(d);
                    }

                    wordDetails.Add(d);
                    i++;
                }
                txtAcross.Text = across.ToString();
                txtDown.Text = down.ToString();
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'CreateClues()' method of 'FinalCrosswordBoard' form. Error msg: {e.Message}");
            }
            finally
            {
                across.Clear();
                down.Clear();
                across = down = null;

                detailsCopy.Clear();
                detailsCopy = null;
            }
        }

        /// <summary>
        /// Paints the words on the board.
        /// https://msdn.microsoft.com/en-us/library/9why95hd%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396
        /// </summary>
        private void MatrixToBoard()
        {
            Graphics canvas = null;
            Font font = null;
            SolidBrush brush = null;

            try
            {
                canvas = CreateGraphics();
                font = new Font("Arial", scaler.GetMetrics(CLUE_INDEX_FONT_SIZE));
                brush = new SolidBrush(Color.Black);
                for (int i = 0; i < wordDetails.Count; i++)
                    canvas.DrawString((i + 1).ToString(), font, brush, (wordDetails[i].X + 1) * scale + calibration, (wordDetails[i].Y + 1) * scale + calibration);
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'MatrixToBoard()' method of 'FinalCrosswordBoard' form. Error msg: {e.Message}");
            }
            finally
            {
                font.Dispose();
                brush.Dispose();
                canvas.Dispose();
            }
        }

        /// <summary>
        /// Fills in the blank cells (where the cells in the matrix are NULL).
        /// </summary>
        private void FillBlankCells()
        {
            try
            {
                if (regularOrUnicode == Globals.CurrentWordType.Regular)
                {
                    for (int i = 0; i < Globals.gridCellCount; i++)
                        for (int j = 0; j < Globals.gridCellCount; j++)
                            if (matrix[i, j] == '\0')
                                BlankCells.Add(new Point((i + 1) * scale + 2, (j + 1) * scale + 2));
                }
                else if (regularOrUnicode == Globals.CurrentWordType.Unicode)
                {
                    for (int i = 0; i < Globals.gridCellCount; i++)
                        for (int j = 0; j < Globals.gridCellCount; j++)
                            if (unicodeMatrix[i, j, 0] == '\0')
                                BlankCells.Add(new Point((i + 1) * scale + 2, (j + 1) * scale + 2));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'PopulateBlankCells()' method of 'FinalCrosswordBoard' form. Error msg: {e.Message}");
            }
        }

        /// <summary>
        /// Saves the crossword as an image (suffixed with current date-time stamp) in the specified folder.
        /// Saves the answers and the clues  (suffixed with current date-time stamp) as separate text files in the specified folder.
        /// https://stackoverflow.com/questions/5049122/capture-the-screen-shot-using-net
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void saveCrosswordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (Bitmap bmpScreenCapture = new Bitmap(Globals.gridCellCount * scale + scale, Globals.gridCellCount * scale + scale))
            using (Graphics g = Graphics.FromImage(bmpScreenCapture))
            {
                // *******************************************************************************************************************
                // ******************** Output the crossword as an image in the game folder. *****************************************
                // *******************************************************************************************************************
                g.CopyFromScreen(Left + ADJUSTMENT_PIXELS,
                                 Top + ADJUSTMENT_PIXELS * 2,
                                 0, 0,
                                 bmpScreenCapture.Size,
                                 CopyPixelOperation.SourceCopy);
                string dateTimeStamp = DateTime.Today.ToShortDateString().Replace("/", string.Empty) + "_" + DateTime.Now.ToString("HHmmss");
                string crosswordFileName = "Crossword_Clues_" + dateTimeStamp + ".bmp";
                if (!Directory.Exists(Path.GetDirectoryName(Application.ExecutablePath) + @"\Crosswords"))
                    Directory.CreateDirectory(Path.GetDirectoryName(Application.ExecutablePath) + @"\Crosswords");
                bmpScreenCapture.Save(Path.GetDirectoryName(Application.ExecutablePath) + @"\Crosswords\" + crosswordFileName);

                // *******************************************************************************************************************
                // ******************** Output the answers as a text file in the game folder. ****************************************
                // *******************************************************************************************************************
                string answerFileName = "Crossword_Answers_" + dateTimeStamp + ".txt";
                using (StreamWriter writer = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + @"\Crosswords\" + answerFileName))
                {
                    writer.WriteLine($"Across:{Environment.NewLine}===========================================");
                    foreach (IDetails d in answersAcross)
                        writer.WriteLine(d.OutputSequence + ")" + d.Word);

                    writer.WriteLine($"Down:{Environment.NewLine}===========================================");
                    foreach (IDetails d in answersDown)
                        writer.WriteLine(d.OutputSequence + ")" + d.Word);
                }

                // *******************************************************************************************************************
                // ******************** Output the clues as a text file in the game folder. ******************************************
                // *******************************************************************************************************************
                string clueFileName = "Crossword_Clues_" + dateTimeStamp + ".txt";
                using (StreamWriter writer = new StreamWriter(Path.GetDirectoryName(Application.ExecutablePath) + @"\Crosswords\" + clueFileName))
                {
                    writer.WriteLine($"Across:{Environment.NewLine}===========================================");
                    foreach(IDetails d in answersAcross)
                        writer.WriteLine(d.OutputSequence + ")" + d.WordMeaning);

                    writer.WriteLine($"Down:{Environment.NewLine}===========================================");
                    foreach (IDetails d in answersDown)
                        writer.WriteLine(d.OutputSequence + ")" + d.WordMeaning);
                }

                MessageBox.Show("The crossword was saved as:" + Environment.NewLine +
                    Path.GetDirectoryName(Application.ExecutablePath) + CROSSWORD_FOLDER + crosswordFileName +
                    Environment.NewLine + Environment.NewLine +
                    "Answers are saved as:" + Environment.NewLine +
                    Path.GetDirectoryName(Application.ExecutablePath) + CROSSWORD_FOLDER + answerFileName +
                    Environment.NewLine + Environment.NewLine +
                    "Clues are saved as:" + Environment.NewLine +
                    Path.GetDirectoryName(Application.ExecutablePath) + CROSSWORD_FOLDER + clueFileName);
            }
            Close();    // Close the window and return to the main board after saving.
        }

        /// <summary>
        /// Scale the board if necessary.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FinalCrosswordBoard_Resize(object sender, EventArgs e)
        {
            try
            {
                Width = scaler.GetMetrics(Width, "Width");                                  // Form width and height set up.
                Height = scaler.GetMetrics(Height, "Height");
                Left = Screen.GetBounds(this).Width / 2 - Width / 2;                        // Form centering.
                Top = Screen.GetBounds(this).Height / 2 - Height / 2 - ADJUSTMENT_PIXELS;   // Calibration factor.

                foreach (Control ctl in Controls)
                {
                    ctl.Font = new Font(FontFamily.GenericSansSerif, scaler.GetMetrics((int)ctl.Font.Size), FontStyle.Regular);
                    ctl.Width = scaler.GetMetrics(ctl.Width, "Width");
                    ctl.Height = scaler.GetMetrics(ctl.Height, "Height");
                    ctl.Top = scaler.GetMetrics(ctl.Top, "Top");
                    ctl.Left = scaler.GetMetrics(ctl.Left, "Left");
                }

                // Override the textbox width to extend near the border.
                txtAcross.Width = txtDown.Width = Width - txtAcross.Left - ADJUSTMENT_PIXELS * 2;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred in 'FinalCrosswordBoard_Resize()' method of the 'FinalCrosswordBoard' class. Error msg: {ex.Message}");
            }
        }

        /// <summary>
        /// Closes the form and returns to the main board.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}