// ***************************************************************************************************************************************
// ****** Mehedi Shams Rony: Dec 2018*****************************************************************************************************
// ****** Purpose: Main crossword logic for arranging unicode letters in the matrix and storing the word poistions. **********************
// ***************************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WordPuzzle.Classes;

namespace CrossWord.Classes
{
    class BanglaUnicodeGameEngine
    {
        public List<UnicodeWordDetails> wordDetails = new List<UnicodeWordDetails>();
        private Random rnd;
        public char[,,] matrix;                          // This is a word matrix to mimic the board.

        List<Direction> directions = new List<Direction>();

        public BanglaUnicodeGameEngine()
        {
            directions.Add(Direction.Down);
            directions.Add(Direction.Right);
            directions.Add(Direction.None);
        }

        /// <summary>
        /// Loops through all the words on the list and tries to find a placement for them.
        /// </summary>
        /// <param name="Words"></param>
        public void PlaceWordsOnTheBoard(List<KeyValuePair<string, string>> Words)
        {
            try
            {
                wordDetails.Clear();
                matrix = new char[Globals.gridCellCount, Globals.gridCellCount, Globals.gridCellCount];
                rnd = new Random(DateTime.Now.Millisecond);
                Direction direction;
                int x, y;
                long attempts;
                bool success;
                string word;
                int indexOfLetterInCompositeLetters;
                List<string> compositeUnicodeLetters = new List<string>();
                BanglaUnicodeParser parser = new BanglaUnicodeParser();

                for (int i = 0; i < Words.Count; i++)   // For all the words in the list, try to place them on the board.
                {
                    attempts = 0;
                    success = false;
                    word = Words[i].Key;
                    compositeUnicodeLetters = parser.ParseWord(word);

                    // *****************************************************************************************************************************
                    // Eneavour 1: Loop through individual composite letters and try to find if it can be placed on an existing letter on the board.
                    // *****************************************************************************************************************************
                    foreach (string compositeLetter in compositeUnicodeLetters)
                    {
                        indexOfLetterInCompositeLetters = compositeUnicodeLetters.IndexOf(compositeLetter);
                        success = TryCrossingThroughExistingWords(Words[i], compositeLetter, indexOfLetterInCompositeLetters, compositeUnicodeLetters, i, ref attempts);
                        if (success) break;
                    }
                    if (success) continue;  // If placement succeeded, then continue with the next word.

                    // *****************************************************************************************************************************
                    // Eneavour 2: Try random axes selection and see if the word can be placed there.
                    // *****************************************************************************************************************************                    
                    do
                    {
                        direction = GetDirection(rnd, 3);   // From randomizer, orientation of the string to put should be either of the eight orientations.

                        x = GetRanddomAxis(rnd, Globals.gridCellCount);    // Get the X-coordinate for the string to place.
                        y = GetRanddomAxis(rnd, Globals.gridCellCount);    // Get the Y-coordinate for the string to place.

                        success = PlaceTheWord(direction, x, y, word, Words[i].Value, compositeUnicodeLetters, i, ref attempts);
                        if (attempts > Globals.MAX_ATTEMPTS)
                        {
                            SaveWordDetailsInCollection(word, Words[i].Value, -1, -1, Direction.None, attempts, true);
                            success = true;
                            break;
                        }
                        if (success) break;
                    }
                    while (!success);
                }
                foreach (UnicodeWordDetails wrd in wordDetails)             // Flag the words that are completely isolated.
                    CheckIfTheWordIsIsolatedAndFlagAccordingly(wrd);
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred in 'PlaceWordsOnTheBoard()' method of the 'BanglaUnicodeGameEngine' class. Error msg: " + e.Message);
            }
        }

        /// <summary>
        /// This method tries to find a similar composite letter in any existing word on the board.
        /// If no word is found or there is nothing on the board, then -1 is assigned to both x and y
        /// that indicates the calling function that no space was found and the calling method then
        /// generates random axes for the matrix.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="compositeLetter"></param>
        private bool TryCrossingThroughExistingWords(KeyValuePair<string, string> word, string compositeLetter, int indexOfLetterInCompositeLetters, List<string> compositeUnicodeLetters, int i, ref long attempts)
        {
            int x = -1, y = -1;
            List<UnicodeWordDetails> wordOnBoard = null;
            Direction dir;
            try
            {
                wordOnBoard = wordDetails.Where(z => z.CompositeUnicodeLetters.Contains(compositeLetter)).ToList();
                foreach (UnicodeWordDetails wrd in wordOnBoard)
                {
                    if (wrd.WordDirection == Direction.Right)
                    {
                        int pos = wrd.CompositeUnicodeLetters.IndexOf(compositeLetter);
                        x = wrd.X + pos;
                        y = wrd.Y - indexOfLetterInCompositeLetters;
                    }
                    else if (wrd.WordDirection == Direction.Down)
                    {
                        int pos = wrd.CompositeUnicodeLetters.IndexOf(compositeLetter);
                        x = wrd.X - indexOfLetterInCompositeLetters;
                        y = wrd.Y + pos;
                    }

                    if (x < 0 || y < 0) continue;

                    dir = wrd.WordDirection == Direction.Down ? Direction.Right : Direction.Down;   // Direction should be opposite to what is on the board.
                    bool success = PlaceTheWord(dir, x, y, word.Key, word.Value, compositeUnicodeLetters, i, ref attempts); // Try placing the word.
                    if (success) return true;
                    if (attempts > Globals.MAX_ATTEMPTS)
                    {
                        SaveWordDetailsInCollection(wrd.Word, wrd.WordMeaning, -1, -1, Direction.None, attempts, true);
                        return false;
                    }
                }
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred in 'TryCrossingThroughExistingWords()' method of the 'BanglaUnicodeGameEngine' class. Error msg: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Flags the words that are completely isolated.
        /// </summary>
        /// <param name="wrd"></param>
        private void CheckIfTheWordIsIsolatedAndFlagAccordingly(UnicodeWordDetails wrd)
        {
            try
            {
                if (wrd.WordDirection == Direction.Down)
                {
                    if (wrd.X > 0)                                                                  // If there is a column of cells to the left of the down-directed word.
                        for (int x = wrd.X - 1, y = wrd.Y, i = 0; i < wrd.CompositeUnicodeLetters.Count; y++, i++)    // Walk downwards along the left column of the word.
                            if (matrix[x, y, 0] != '\0')                                            // And see if there is any character to any cell of that column. Checking the first cell in the third dimension is okay as that dimension contains a combination of elements for the unicode.
                            {                                                                       // Which would mean another word passed through; hence this is not isolated.
                                wrd.Isolated = false;
                                return;
                            }
                    if (wrd.X < Globals.gridCellCount - 1)                                          // If there is a column of cells to the left of the down-directed word.
                        for (int x = wrd.X + 1, y = wrd.Y, i = 0; i < wrd.CompositeUnicodeLetters.Count; y++, i++)    // Walk downwards along the right column of the word.
                            if (matrix[x, y, 0] != '\0')                                            // And see if there is any character to any cell of that column. Checking the first cell in the third dimension is okay as that dimension contains a combination of elements for the unicode.
                            {                                                                       // Which would mean another word passed through; hence this is not isolated.
                                wrd.Isolated = false;
                                return;
                            }
                }
                else if (wrd.WordDirection == Direction.Right)
                {
                    if (wrd.Y > 0)                                                                  // If there is a row of cells to the top of the right-directed word.
                        for (int x = wrd.X, y = wrd.Y - 1, i = 0; i < wrd.CompositeUnicodeLetters.Count; x++, i++)    // Walk righwards along the top row of the word.
                            if (matrix[x, y, 0] != '\0')                                            // And see if there is any character to any cell of that column. Checking the first cell in the third dimension is okay as that dimension contains a combination of elements for the unicode.
                            {                                                                       // Which would mean another word passed through; hence this is not isolated.
                                wrd.Isolated = false;
                                return;
                            }
                    if (wrd.Y < Globals.gridCellCount - 1)                                          // If there is a row of cells to the bottom of the right-directed word.
                        for (int x = wrd.X, y = wrd.Y + 1, i = 0; i < wrd.CompositeUnicodeLetters.Count; x++, i++)    // Walk righwards along the bottom row of the word.
                            if (matrix[x, y, 0] != '\0')                                            // And see if there is any character to any cell of that column. Checking the first cell in the third dimension is okay as that dimension contains a combination of elements for the unicode.
                            {                                                                       // Which would mean another word passed through; hence this is not isolated.
                                wrd.Isolated = false;
                                return;
                            }
                }

                // Reaching this point would mean that the word didn't cross through another word.
                // Hence flag it as isolated and clear the corresponding cells in the character matrix.
                // However, if that is already flaged as failed, then no need to flag it again as isolated.

                if (!wrd.FailedMaxAttempts)
                    wrd.Isolated = true;

                if (wrd.WordDirection == Direction.Down)
                    for (int i = 0, x = wrd.X, y = wrd.Y; i < wrd.CompositeUnicodeLetters.Count; i++, y++)
                        for (int j = 0; j < wrd.CompositeUnicodeLetters[i].Length; j++)
                            matrix[x, y, j] = '\0';
                else if (wrd.WordDirection == Direction.Right)
                    for (int i = 0, x = wrd.X, y = wrd.Y; i < wrd.CompositeUnicodeLetters.Count; i++, x++)
                        for (int j = 0; j < wrd.CompositeUnicodeLetters[i].Length; j++)
                            matrix[x, y, j] = '\0';
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'CheckIfTheWordIsIsolatedAndMarkAccordingly()' method of the" +
                                $"'BanglaUnicodeGameEngine' class. for the word {wrd} Error msg: " + e.InnerException.Message);
            }
        }

        /// <summary>
        /// Randomly generate direction - ACROSS (RIGHT), or DOWN.
        /// </summary>
        /// <param name="Rnd"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        private Direction GetDirection(Random Rnd, int Max)
        {
            switch (Rnd.Next(1, Max))   // Generate a random number between 1 and Max - 1; So if Max = 9, it will generate a random direction between 1 and 8.
            {
                case 1: if (directions.Find(p => p.Equals(Direction.Down)) == Direction.Down) return Direction.Down; break;
                case 2: if (directions.Find(p => p.Equals(Direction.Right)) == Direction.Right) return Direction.Right; break;
                default: return Direction.None;
            }
            return Direction.None;
        }

        /// <summary>
        /// Generates random X or Y axis.
        /// </summary>
        /// <param name="Rnd"></param>
        /// <param name="Max"></param>
        /// <returns></returns>
        private int GetRanddomAxis(Random Rnd, int Max)
        {
            return Rnd.Next(Max);   // Generates a number from 0 up to the grid size.
        }

        /// <summary>
        /// This function checks if a word that is already to the left of the word rightfully passes through the word to be placed.
        /// </summary>
        /// <param name="x">Intended x-position of the word to be placed</param>
        /// <param name="y">Intended y-position of the word to be placed</param>
        /// <param name="direction">The direction to search for from the (x, y)</param>
        /// <returns></returns>
        bool LegitimateOverlapOfAnExistingWord(int x, int y, List<string> word, Direction direction)
        {
            char[] chars = new char[Globals.MAX_UNICODE_WORD_LENGTH];
            int originalX = x, originalY = y;
            try
            {
                switch (direction)
                {
                    case Direction.Left:
                        while (--x >= 0)
                            if (matrix[x, y, 0] == '\0') break;                        // First walk towards the left until you reach the beginning of the word that is already on the board.
                        ++x;

                        for (int i = 0, j = 0; x < Globals.gridCellCount && i < word.Count; x++) // Now walk towards right until you reach the end of the word that is already on the board.
                        {
                            if (matrix[x, y, 0] == '\0') break;
                            for (int z = 0; matrix[x, y, z] != '\0'; z++)
                                chars[j++] = matrix[x, y, z];
                        }

                        string str = new string(chars);
                        str = str.Trim('\0');
                        UnicodeWordDetails wordOnBoard = wordDetails.Find(a => a.Word == str);  // See if the characters form a valid word that is already on the board.
                        if (wordOnBoard == null) return false;                                  // If this is not a word on the board, then this must be some random characters, hence not a legitimate word, hence this is a wrong placement.
                        if (wordOnBoard.WordDirection == Direction.Down) return false;          // If the word on the board is in parallel to the word on to be placed, then also this is a wrong placement as two words cannot be placed side by side in the same direction.
                        if (wordOnBoard.X + wordOnBoard.CompositeUnicodeLetters.Count == originalX) return false; // The word on the board ended just before the x-cordinate for the current word to place. Hence illegitimate.
                        return true;                                                            // Else, passed all validation checks for a legitimate overlap, hence return true.
                    case Direction.Right:
                        while (--x >= 0)
                            if (matrix[x, y, 0] == '\0') break;                                 // First walk towards the left until you reach the beginning of the word that is already on the board.
                        ++x;

                        for (int i = 0, j = 0; x < Globals.gridCellCount && i < Globals.MAX_WORD_LENGTH; x++) // Now walk towards right until you reach the end of the word that is already on the board.
                        {
                            if (matrix[x, y, 0] == '\0') break;
                            for (int z = 0; matrix[x, y, z] != '\0'; z++)
                                chars[j++] = matrix[x, y, z];
                        }

                        str = new string(chars);
                        str = str.Trim('\0');
                        wordOnBoard = wordDetails.Find(a => a.Word == str);                 // See if the characters form a valid word that is already on the board.
                        if (wordOnBoard == null) return false;                              // If this is not a word on the board, then this must be some random characters, hence not a legitimate word, hence this is a wrong placement.
                        if (wordOnBoard.WordDirection == Direction.Down) return false;      // If the word on the board is in parallel to the word on to be placed, then also this is a wrong placement as two words cannot be placed side by side in the same direction.
                        if (wordOnBoard.X == originalX + 1) return false;                   // The word on the board starts right after the x-cordinate for the current word to place. Hence illegitimate.
                        return true;                                                        // Else, passed all validation checks for a legitimate overlap, hence return true.
                    case Direction.Up:
                        while (--y >= 0)
                            if (matrix[x, y, 0] == '\0') break;                             // First walk upwards until you reach the beginning of the word that is already on the board.
                        ++y;

                        for (int i = 0, j = 0; y < Globals.gridCellCount && i < Globals.MAX_WORD_LENGTH; y++) // Now walk downwards until you reach the end of the word that is already on the board.
                        {
                            if (matrix[x, y, 0] == '\0') break;
                            for (int z = 0; matrix[x, y, z] != '\0'; z++)
                                chars[j++] = matrix[x, y, z];
                        }

                        str = new string(chars);
                        str = str.Trim('\0');
                        wordOnBoard = wordDetails.Find(a => a.Word == str);                 // See if the characters form a valid word that is already on the board.
                        if (wordOnBoard == null) return false;                              // If this is not a word on the board, then this must be some random characters, hence not a legitimate word, hence this is a wrong placement.
                        if (wordOnBoard.WordDirection == Direction.Right) return false;     // If the word on the board is in parallel to the word on to be placed, then also this is a wrong placement as two words cannot be placed side by side in the same direction.
                        if (wordOnBoard.Y + wordOnBoard.CompositeUnicodeLetters.Count == originalY) return false;     // The word on the board starts right below the y-cordinate for the current word to place. Hence illegitimate.
                        return true;                                                        // Else, passed all validation checks for a legitimate overlap, hence return true.
                    case Direction.Down:
                        while (--y >= 0)
                            if (matrix[x, y, 0] == '\0') break;                             // First walk upwards until you reach the beginning of the word that is already on the board.
                        ++y;

                        for (int i = 0, j = 0; y < Globals.gridCellCount && i < Globals.MAX_WORD_LENGTH; y++) // Now walk downwards until you reach the end of the word that is already on the board.
                        {
                            if (matrix[x, y, 0] == '\0') break;
                            for (int z = 0; matrix[x, y, z] != '\0'; z++)
                                chars[j++] = matrix[x, y, z];
                        }

                        str = new string(chars);
                        str = str.Trim('\0');
                        wordOnBoard = wordDetails.Find(a => a.Word == str);                 // See if the characters form a valid word that is already on the board.
                        if (wordOnBoard == null) return false;                              // If this is not a word on the board, then this must be some random characters, hence not a legitimate word, hence this is a wrong placement.
                        if (wordOnBoard.WordDirection == Direction.Right) return false;     // If the word on the board is in parallel to the word on to be placed, then also this is a wrong placement as two words cannot be placed side by side in the same direction.
                        if (wordOnBoard.Y == originalY + 1) return false;                   // The word on the board starts right after the x-cordinate for the current word to place. Hence illegitimate.
                        return true;                                                        // Else, passed all validation checks for a legitimate overlap, hence return true.
                }
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in the LegitimateOverlapOfAnExistingWord() method of the 'BanglaUnicodeGameEngine' class.\n\n" +
                                $"Original x = {originalX}, Original y = {originalY}, x = {x}, y = {y}, word: {word}, Direction: {direction}." +
                                $"\n\nError msg: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// This method checks if any left-side characters of the word to be placed is a valid overlap from an existing right-directed word on the board.
        /// E.g.: CART is to be placed downwards. It checks valid overlaps with existing right-directed words - ARC, PARTY.
        ///     A R C
        ///         A
        ///     P A R T Y
        ///         T
        /// </summary>
        /// <param name="x">Intended x-position of the word to be placed</param>
        /// <param name="y">Intended y-position of the word to be placed</param>
        /// <param name="word">The word to be placed. E.g.: CART</param>
        /// <returns></returns>
        bool LeftCellFreeForDownDirectedWord(int x, int y, List<string> word)
        {
            try
            {
                if (x == 0) return true;
                bool isValid = true;
                if (x > 0)
                {
                    for (int i = 0; i < word.Count; y++, i++)
                    {
                        if (matrix[x - 1, y, 0] != '\0')
                            isValid = LegitimateOverlapOfAnExistingWord(x, y, word, Direction.Left);
                        if (!isValid) break;
                    }
                }
                return isValid;
            }
            catch (Exception e)
            {
                string wrd = string.Join("", word);
                MessageBox.Show($"An error occurred in the LeftCellFreeForDownDirectedWord() method of the 'BanglaUnicodeGameEngine' class. x = {x}, y = {y}, word: {wrd}.\n\nError msg: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// This method checks if any there is any character to the left cell of the current word to place.
        /// E.g.: CAT is to be placed ACROSS, then there cannot be a letter to the left of CAT.
        /// ****************   P |__|
        /// **************** |__|  C   A   T
        /// ****************   A |__|
        /// </summary>
        /// <param name="x">Intended x-position of the word to be placed</param>
        /// <param name="y">Intended y-position of the word to be placed</param>
        /// <param name="word">The word to be placed. E.g.: CART</param>
        /// <returns></returns>
        bool LeftCellFreeForRightDirectedWord(int x, int y, List<string> word)
        {
            try
            {
                if (x == 0) return true;
                if (x - 1 >= 0)
                    return matrix[x - 1, y, 0] == '\0';
                return false;
            }
            catch (Exception e)
            {
                string wrd = string.Join("", word);
                MessageBox.Show($"An error occurred in the LeftCellFreeForRightDirectedWord() method of the 'BanglaUnicodeGameEngine' class. x = {x}, y = {y}, word: {wrd}.\n\nError msg: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// This method checks if any right-side characters of the word to be placed is a valid overlap from an existing ACROSS word on the board.
        /// E.g.: CART is to be placed downwards. It checks valid overlaps with existing right-directed words - ARC, PARTY.
        ///     A R C
        ///         A
        ///     P A R T Y
        ///         T
        /// </summary>
        /// <param name="x">Intended x-position of the word to be placed</param>
        /// <param name="y">Intended y-position of the word to be placed</param>
        /// <param name="word">The word to be placed. E.g.: CART</param>
        /// <returns></returns>
        bool RightCellFreeForDownDirectedWord(int x, int y, List<string> word)
        {
            try
            {
                if (x == Globals.gridCellCount) return true;
                bool isValid = true;
                if (x + 1 < Globals.gridCellCount)
                {
                    for (int i = 0; i < word.Count; y++, i++)
                    {
                        if (matrix[x + 1, y, 0] != '\0')
                            isValid = LegitimateOverlapOfAnExistingWord(x, y, word, Direction.Right);
                        if (!isValid) break;
                    }
                }
                return isValid;
            }
            catch (Exception e)
            {
                string wrd = string.Join("", word);
                MessageBox.Show($"An error occurred in the RightCellFreeForDownDirectedWord() method of the 'BanglaUnicodeGameEngine' class. x = {x}, y = {y}, word: {wrd}.\n\nError msg: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// This method checks if any there is any character to the left cell of the current word to place.
        /// E.g.: CAT is to be placed ACROSS, then there cannot be a letter to the right of CAT.
        /// ****************       |__|  P
        /// **************** C   A   T  |__|
        /// ****************       |__|
        /// </summary>
        /// <param name="x">Intended x-position of the word to be placed</param>
        /// <param name="y">Intended y-position of the word to be placed</param>
        /// <param name="word">The word to be placed. E.g.: CART</param>
        /// <returns></returns>
        bool RightMostCellFreeForRightDirectedWord(int x, int y, List<string> word)
        {
            try
            {
                if (x + word.Count == Globals.gridCellCount) return true;
                if (x + word.Count < Globals.gridCellCount)
                    return matrix[x + word.Count, y, 0] == '\0';
                return false;
            }
            catch (Exception e)
            {
                string wrd = string.Join("", word);
                MessageBox.Show($"An error occurred in the RightMostCellFreeForRightDirectedWord() method of the 'BanglaUnicodeGameEngine' class. x = {x}, y = {y}, word: {wrd}.\n\nError msg: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// If it is a DWON word, then there should not be any character in the adjacent cell to the top of the beginning of this word.
        /// E.g.: If CAT is to be placed downwards, then the top cell should be free:
        /// |___| C L A S S
        ///   C
        ///   A
        ///   T
        /// </summary>
        /// <param name="x">Intended x-position of the word to be placed</param>
        /// <param name="y">Intended y-position of the word to be placed</param>
        /// <param name="word">The word to be placed. E.g.: CART</param>
        /// <returns></returns>
        bool TopCellFreeForDownDirectedWord(int x, int y, List<string> word)
        {
            try
            {
                if (y == 0) return true;
                if (y - 1 >= 0)
                    return matrix[x, y - 1, 0] == '\0';
                return false;
            }
            catch (Exception e)
            {
                string wrd = string.Join("", word);
                MessageBox.Show($"An error occurred in the TopCellFreeForDownDirectedWord() method of the 'BanglaUnicodeGameEngine' class. x = {x}, y = {y}, word: {wrd}.\n\nError msg: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// This method checks if any top-side characters of the word to be placed is a valid overlap from an existing DOWB word on the board.
        /// E.g.: CART is to be placed ACROSS. It checks valid overlaps with existing DOWN words - ARC, PARTY.
        ///       A   P
        ///       R   A
        ///       C A R T
        ///           T
        ///           Y
        /// </summary>
        /// <param name="x">Intended x-position of the word to be placed</param>
        /// <param name="y">Intended y-position of the word to be placed</param>
        /// <param name="word">The word to be placed. E.g.: CART</param>
        /// <returns></returns>
        bool TopCellFreeForRightDirectedWord(int x, int y, List<string> word)
        {
            try
            {
                if (y == 0) return true;
                bool isValid = true;
                if (y - 1 >= 0)
                {
                    for (int i = 0; i < word.Count; x++, i++)
                    {
                        if (matrix[x, y - 1, 0] != '\0')
                            isValid = LegitimateOverlapOfAnExistingWord(x, y, word, Direction.Up);
                        if (!isValid) break;
                    }
                }
                return isValid;
            }
            catch (Exception e)
            {
                string wrd = string.Join("", word);
                MessageBox.Show($"An error occurred in the TopCellFreeForRightDirectedWord() method of the 'BanglaUnicodeGameEngine' class. x = {x}, y = {y}, word: {wrd}.\n\nError msg: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// This method checks if any top-side characters of the word to be placed is a valid overlap from an existing DOWB word on the board.
        /// E.g.: CART is to be placed ACROSS. It checks valid overlaps with existing DOWN words - SCOOP, PARTY.
        ///           P
        ///       S   A
        ///       C A R T
        ///       O   T
        ///       O   Y
        ///       P
        /// </summary>
        /// <param name="x">Intended x-position of the word to be placed</param>
        /// <param name="y">Intended y-position of the word to be placed</param>
        /// <param name="word">The word to be placed. E.g.: CART</param>
        /// <returns></returns>
        bool BottomCellFreeForRightDirectedWord(int x, int y, List<string> word)
        {
            try
            {
                if (y == Globals.gridCellCount) return true;
                bool isValid = true;
                if (y + 1 < Globals.gridCellCount)
                {
                    for (int i = 0; i < word.Count; x++, i++)
                    {
                        if (matrix[x, y + 1, 0] != '\0')
                            isValid = LegitimateOverlapOfAnExistingWord(x, y, word, Direction.Down);
                        if (!isValid) break;
                    }
                }
                return isValid;
            }
            catch (Exception e)
            {
                string wrd = string.Join("", word);
                MessageBox.Show($"An error occurred in the BottomCellFreeForRightDirectedWord() method of the 'BanglaUnicodeGameEngine' class. x = {x}, y = {y}, word: {wrd}.\n\nError msg: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// If it is a DWON word, then there should not be any character in the adjacent cell to the bottom of the end of this word.
        /// E.g.: If CAT is to be placed downwards, then the bottom cell should be free:        
        ///   C
        ///   A
        ///   T
        /// |___| C L A S S
        /// </summary>
        /// <param name="x">Intended x-position of the word to be placed</param>
        /// <param name="y">Intended y-position of the word to be placed</param>
        /// <param name="word">The word to be placed. E.g.: CART</param>
        bool BottomMostBottomCellFreeForDownDirectedWord(int x, int y, List<string> word)
        {
            try
            {
                if (y + word.Count == Globals.gridCellCount) return true;
                if (y + word.Count < Globals.gridCellCount)
                    return matrix[x, y + word.Count, 0] == '\0';
                return false;
            }
            catch (Exception e)
            {
                string wrd = string.Join("", word);
                MessageBox.Show($"An error occurred in the BottomMostBottomCellFreeForDownDirectedWord() method of the 'BanglaUnicodeGameEngine' class. x = {x}, y = {y}, word: {wrd}.\n\nError msg: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// This method first checks if there is a valid overlap with any existing letter in the same cell of the matrix.
        /// It also makes sure the first few words don't overlap to make them sparse around the matrix.
        /// After the first few scattered words, it makes sure all the subsequent letters are overlapped with existing words.
        /// Then it checks legitimate cross-through with an existing word.
        /// If all passes, then it places the word in the collection.
        /// </summary>
        /// <param name="direction"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="word"></param>
        /// <param name="wordMeaning"></param>
        /// <param name="currentWordCount"></param>
        /// <param name="attempts"></param>
        /// <returns></returns>
        private bool PlaceTheWord(Direction direction, int x, int y, string word, string wordMeaning, List<string> unicodeLetters, int currentWordCount, ref long attempts)
        {
            try
            {
                attempts++;
                bool placeAvailable = true, overlapped = false;
                switch (direction)
                {
                    case Direction.Right:
                        for (int i = 0, xx = x; i < unicodeLetters.Count; i++, xx++)    // First we check if the word can be placed in the array. For this it needs blanks there or the same letter (of another word) in the cell.
                        {
                            if (xx >= Globals.gridCellCount) return false;              // Falling outside the grid. Hence placement unavailable.
                            if (matrix[xx, y, 0] != '\0')
                            {
                                string compositeUnicodeLetter = Globals.GetCompositeLetterFromTheMatrix(xx, y, matrix);
                                if (compositeUnicodeLetter != unicodeLetters[i])   // If there is an overlap, then we see if the characters match. If matches, then it can still go there.
                                {
                                    placeAvailable = false;
                                    break;
                                }
                                else overlapped = true;
                            }
                        }

                        if (!placeAvailable)
                            return false;

                        // The first few words should be placed without overlapping.
                        if (currentWordCount < Globals.MAX_NON_OVERLAPPING_WORDS_THRESHOLD && overlapped)
                            return false;

                        // If overlapping didn't occur after the maximum allowed non-overlapping words for the first few runs (e.g. first 5 words)
                        // then trigger a re-position attempt (by returning false to the calling method which will in turn trigger another search until
                        // an overlapping position is found.)
                        else if (currentWordCount >= Globals.MAX_NON_OVERLAPPING_WORDS_THRESHOLD && !overlapped)
                            return false;

                        // If it is a right-direction, then there should not be any character in the adjacent cell to the left of the beginning of this word.
                        // E.g.: If CAT is to be placed, then it cannot be placed with another word CLASS as C L A S S C A T
                        bool leftFree = LeftCellFreeForRightDirectedWord(x, y, unicodeLetters);

                        // If it is a right-direction, then check if it can cross through another word:
                        // V
                        // I   S
                        // C A T
                        // I   I
                        // N   C
                        // I   K
                        // T
                        // Y
                        bool topFree = TopCellFreeForRightDirectedWord(x, y, unicodeLetters);

                        // If it is a right-direction, then check if it can cross through another word:
                        // V
                        // I   S
                        // C A T
                        // I   I
                        // N   C
                        // I   K
                        // T
                        // Y
                        bool bottomFree = BottomCellFreeForRightDirectedWord(x, y, unicodeLetters);

                        // If it is a right-direction, then there should not be any character in the adjacent cell to the right of the ending of this word.
                        // E.g.: If CAT is to be placed, then it cannot be placed with another word BUS as C A T B U S
                        bool rightMostFree = RightMostCellFreeForRightDirectedWord(x, y, unicodeLetters);

                        // If cells that need to be free are not free, then this word cannot be placed there.
                        if (!leftFree || !topFree || !bottomFree || !rightMostFree) return false;

                        // If all the cells are blank, or a non-conflicting overlap is available, then this word can be placed there. So place it.
                        SaveWordDetailsInCollection(word, wordMeaning, x, y, direction, attempts, false);                        
                        for (int i = 0; i < unicodeLetters.Count; i++, x++)
                        {
                            char[] atomElements = unicodeLetters[i].ToArray();
                            int z = 0;
                            foreach (char c in atomElements)
                                matrix[x, y, z++] = c;
                        }                        
                        return true;                        
                    case Direction.Down:
                        for (int i = 0, yy = y; i < unicodeLetters.Count; i++, yy++)    // First we check if the word can be placed in the array. For this it needs blanks there or the same letter (of another word) in the cell.
                        {
                            if (yy >= Globals.gridCellCount) return false;              // Falling outside the grid. Hence placement unavailable.
                            if (matrix[x, yy, 0] != '\0')
                            {
                                string compositeUnicodeLetter = Globals.GetCompositeLetterFromTheMatrix(x, yy, matrix);
                                if (compositeUnicodeLetter != unicodeLetters[i])   // If there is an overlap, then we see if the characters match. If matches, then it can still go there.
                                {
                                    placeAvailable = false;
                                    break;
                                }
                                else overlapped = true;
                            }
                        }

                        if (!placeAvailable)
                            return false;

                        // The first few words should be placed without overlapping.
                        if (currentWordCount < Globals.MAX_NON_OVERLAPPING_WORDS_THRESHOLD && overlapped)
                            return false;

                        // If overlapping didn't occur after the maximum allowed non-overlapping words for the first few runs (e.g. first 5 words)
                        // then trigger a re-position attempt (by returning false to the calling method which will in turn trigger another search until
                        // an overlapping position is found.)
                        else if (currentWordCount >= Globals.MAX_NON_OVERLAPPING_WORDS_THRESHOLD && !overlapped)
                            return false;

                        // If it is a right-direction, then check if it can cross through another word. E.g.: If STICK the current word, see if there
                        // is a valid crossing through existing words on the board - CAT, SKID:
                        //     S
                        // C A T
                        //     I
                        //     C
                        //   S K I D
                        leftFree = LeftCellFreeForDownDirectedWord(x, y, unicodeLetters);

                        // If it is a right-direction, then check if it can cross through another word. E.g.: If STICK the current word, see if there
                        // is a valid crossing through existing words on the board - CAT, SKID:
                        //     S
                        // C A T
                        //     I
                        //     C
                        //   S K I D
                        bool rightFree = RightCellFreeForDownDirectedWord(x, y, unicodeLetters);

                        // If it is a down-direction, then there should not be any character in the adjacent cell to the top of the beginning of this word.
                        // E.g.: If CAT is to be placed downwards, then it cannot be placed with another word CLASS as
                        // C L A S S
                        // C
                        // A
                        // T
                        topFree = TopCellFreeForDownDirectedWord(x, y, unicodeLetters);

                        // If it is a down-direction, then there should not be any character in the adjacent cell to the bottom of the end of this word.
                        // E.g.: If CAT is to be placed downwards, then it cannot be placed with another word CLASS as                        
                        // C
                        // A
                        // T
                        // C L A S S
                        bool bottomMostBottomFree = BottomMostBottomCellFreeForDownDirectedWord(x, y, unicodeLetters);

                        // If cells that need to be free are not free, then this word cannot be placed there.
                        if (!leftFree || !rightFree || !topFree || !bottomMostBottomFree) return false;

                        // If all the cells are blank, or a non-conflicting overlap is available, then this word can be placed there. So place it.
                        SaveWordDetailsInCollection(word, wordMeaning, x, y, direction, attempts, false);
                        for (int i = 0; i < unicodeLetters.Count; i++, y++)
                        {
                            char[] atomElements = unicodeLetters[i].ToArray();
                            int z = 0;
                            foreach (char c in atomElements)
                                matrix[x, y, z++] = c;
                        }
                        return true;
                }
                return false;   // Otherwise continue with a different place and index.
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'PlaceTheWords' method of the 'BanglaUnicodeGameEngine' class. Error msg: {e.Message}");
                return false;   // Otherwise continue with a different place and index.
            }
        }

        private void SaveWordDetailsInCollection(string word, string wordMeaning, int x, int y, Direction direction, long attempts, bool failedMaxAttempts)
        {
            try
            {
                UnicodeWordDetails details = new UnicodeWordDetails();
                BanglaUnicodeParser parser = new BanglaUnicodeParser();
                details.CompositeUnicodeLetters = parser.ParseWord(word.ToString());

                details.Word = word;
                details.WordMeaning = wordMeaning;

                details.X = x;
                details.Y = y;
                details.WordDirection = direction;
                details.AttemptsCount = attempts;
                details.FailedMaxAttempts = failedMaxAttempts;
                wordDetails.Add(details);
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'SaveWordDetailsInCollection()' method of the 'BanglaUnicodeGameEngine' class. Error msg: {e.Message}");
            }
        }
    }
}