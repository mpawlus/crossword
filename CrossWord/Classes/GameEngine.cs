// ***************************************************************************************************************************************
// ****** Mehedi Shams Rony: Dec 2018*****************************************************************************************************
// ****** Purpose: Main crossword logic for arranging letters in the matrix and storing the word poistions. ******************************
// ***************************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using CrossWord.Classes;

namespace WordPuzzle.Classes
{
    public class GameEngine
    {
        public List<RegularWordDetails> wordDetails = new List<RegularWordDetails>();
        private Random rnd;
        public char[,] matrix;                          // This is a word matrix to mimic the board.        
        List<Direction> directions = new List<Direction>();

        public GameEngine()
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
                matrix = new char[Globals.gridCellCount, Globals.gridCellCount];
                rnd = new Random(DateTime.Now.Millisecond);
                Direction direction;
                int x, y;
                long attempts;
                bool success;
                string word;

                for (int i = 0; i < Words.Count; i++)                       // For all the words in the list, try to place them on the board.
                {
                    attempts = 0;
                    success = false;
                    word = Words[i].Key;
                    do
                    {
                        direction = GetDirection(rnd, 3);                   // From randomizer, orientation of the string to put should be either of the eight orientations.
                        x = GetRandomAxis(rnd, Globals.gridCellCount);      // Get the X-coordinate for the string to place.
                        y = GetRandomAxis(rnd, Globals.gridCellCount);      // Get the Y-coordinate for the string to place.
                        success = PlaceTheWord(direction, x, y, Words[i].Key, Words[i].Value, i, ref attempts);
                        if (attempts > Globals.MAX_ATTEMPTS)
                        {
                            SaveWordDetailsInCollection(word, Words[i].Value, -1, -1, Direction.None, attempts, true);
                            break;
                        }
                    }
                    while (!success);
                }
                foreach (RegularWordDetails wrd in wordDetails)             // Flag the words that are completely isolated.
                    CheckIfTheWordIsIsolatedAndFlagAccordingly(wrd);
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'PlaceWordsOnTheBoard()' method of the 'GameEngine' class. Error msg: {e.Message}");
            }
        }

        /// <summary>
        /// Flags the words that are completely isolated.
        /// </summary>
        /// <param name="wrd"></param>
        private void CheckIfTheWordIsIsolatedAndFlagAccordingly(RegularWordDetails wrd)
        {
            try
            {
                if (wrd.WordDirection == Direction.Down)
                {
                    if (wrd.X > 0)                                                                  // If there is a column of cells to the left of the down-directed word.
                        for (int x = wrd.X - 1, y = wrd.Y, i = 0; i < wrd.Word.Length; y++, i++)    // Walk downwards along the left column of the word.
                            if (matrix[x, y] != '\0')                                               // And see if there is any character to any cell of that column.
                            {                                                                       // Which would mean another word passed through; hence this is not isolated.
                                wrd.Isolated = false;
                                return;
                            }
                    if (wrd.X < Globals.gridCellCount - 1)                                          // If there is a column of cells to the left of the down-directed word.
                        for (int x = wrd.X + 1, y = wrd.Y, i = 0; i < wrd.Word.Length; y++, i++)    // Walk downwards along the right column of the word.
                            if (matrix[x, y] != '\0')                                               // And see if there is any character to any cell of that column.
                            {                                                                       // Which would mean another word passed through; hence this is not isolated.
                                wrd.Isolated = false;
                                return;
                            }
                }
                else if (wrd.WordDirection == Direction.Right)
                {
                    if (wrd.Y > 0)                                                                  // If there is a row of cells to the top of the right-directed word.
                        for (int x = wrd.X, y = wrd.Y - 1, i = 0; i < wrd.Word.Length; x++, i++)    // Walk righwards along the top row of the word.
                            if (matrix[x, y] != '\0')                                               // And see if there is any character to any cell of that row.
                            {                                                                       // Which would mean another word passed through; hence this is not isolated.
                                wrd.Isolated = false;
                                return;
                            }
                    if (wrd.Y < Globals.gridCellCount - 1)                                          // If there is a row of cells to the bottom of the right-directed word.
                        for (int x = wrd.X, y = wrd.Y + 1, i = 0; i < wrd.Word.Length; x++, i++)    // Walk righwards along the bottom row of the word.
                            if (matrix[x, y] != '\0')                                               // And see if there is any character to any cell of that row.
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
                    for (int i = 0, x = wrd.X, y = wrd.Y; i < wrd.Word.Length && i < Globals.gridCellCount; i++, y++)
                        matrix[x, y] = '\0';
                else if (wrd.WordDirection == Direction.Right)
                    for (int i = 0, x = wrd.X, y = wrd.Y; i < wrd.Word.Length && i < Globals.gridCellCount; i++, x++)
                        matrix[x, y] = '\0';
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'CheckIfTheWordIsIsolatedAndMarkAccordingly()' method of the" +
                                $"'GameEngine' class. for {Environment.NewLine}word:'{wrd.Word}', x:{wrd.X}, y:{wrd.Y}, direction:{wrd.WordDirection}." +
                                $"{Environment.NewLine}{Environment.NewLine}Error msg: " + e.Message);
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
        private int GetRandomAxis(Random Rnd, int Max)
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
        bool LegitimateOverlapOfAnExistingWord(int x, int y, string word, Direction direction)
        {
            char[] chars = new char[Globals.MAX_WORD_LENGTH];
            int originalX = x, originalY = y;
            try
            {
                switch (direction)
                {
                    case Direction.Left:
                        while (--x >= 0)
                            if (matrix[x, y] == '\0') break;                                // First walk towards the left until you reach the beginning of the word that is already on the board.
                        ++x;

                        for (int i = 0; x < Globals.gridCellCount && i < Globals.MAX_WORD_LENGTH; x++, i++) // Now walk towards right until you reach the end of the word that is already on the board.
                        {
                            if (matrix[x, y] == '\0') break;
                            chars[i] = matrix[x, y];                            
                        }

                        string str = new string(chars);
                        str = str.Trim('\0');
                        RegularWordDetails wordOnBoard = (RegularWordDetails)wordDetails.Find(a => a.Word == str);  // See if the characters form a valid word that is already on the board.
                        if (wordOnBoard == null) return false;                              // If this is not a word on the board, then this must be some random characters, hence not a legitimate word, hence this is a wrong placement.
                        if (wordOnBoard.WordDirection == Direction.Down) return false;      // If the word on the board is in parallel to the word on to be placed, then also this is a wrong placement as two words cannot be placed side by side in the same direction.
                        if (wordOnBoard.X + wordOnBoard.Word.Length == originalX) return false; // The word on the board ended just before the x-cordinate for the current word to place. Hence illegitimate.
                        return true;                                                        // Else, passed all validation checks for a legitimate overlap, hence return true.
                    case Direction.Right:
                        while (--x >= 0)
                            if (matrix[x, y] == '\0') break;                                // First walk towards the left until you reach the beginning of the word that is already on the board.
                        ++x;

                        for (int i = 0; x < Globals.gridCellCount && i < Globals.MAX_WORD_LENGTH; x++, i++) // Now walk towards right until you reach the end of the word that is already on the board.
                        {
                            if (matrix[x, y] == '\0') break;
                            chars[i] = matrix[x, y];
                        }

                        str = new string(chars);
                        str = str.Trim('\0');
                        wordOnBoard = (RegularWordDetails)wordDetails.Find(a => a.Word == str);     // See if the characters form a valid word that is already on the board.
                        if (wordOnBoard == null) return false;                                      // If this is not a word on the board, then this must be some random characters, hence not a legitimate word, hence this is a wrong placement.
                        if (wordOnBoard.WordDirection == Direction.Down) return false;              // If the word on the board is in parallel to the word on to be placed, then also this is a wrong placement as two words cannot be placed side by side in the same direction.
                        if (wordOnBoard.X == originalX + 1) return false;                           // The word on the board starts right after the x-cordinate for the current word to place. Hence illegitimate.
                        return true;                                                                // Else, passed all validation checks for a legitimate overlap, hence return true.
                    case Direction.Up:
                        while (--y >= 0)
                            if (matrix[x, y] == '\0') break;                                        // First walk upwards until you reach the beginning of the word that is already on the board.
                        ++y;

                        for (int i = 0; y < Globals.gridCellCount && i < Globals.MAX_WORD_LENGTH; y++, i++) // Now walk downwards until you reach the end of the word that is already on the board.
                        {
                            if (matrix[x, y] == '\0') break;
                            chars[i] = matrix[x, y];
                        }

                        str = new string(chars);
                        str = str.Trim('\0');
                        wordOnBoard = (RegularWordDetails)wordDetails.Find(a => a.Word == str);     // See if the characters form a valid word that is already on the board.
                        if (wordOnBoard == null) return false;                                      // If this is not a word on the board, then this must be some random characters, hence not a legitimate word, hence this is a wrong placement.
                        if (wordOnBoard.WordDirection == Direction.Right) return false;             // If the word on the board is in parallel to the word on to be placed, then also this is a wrong placement as two words cannot be placed side by side in the same direction.
                        if (wordOnBoard.Y + wordOnBoard.Word.Length == originalY) return false;     // The word on the board starts right below the y-cordinate for the current word to place. Hence illegitimate.
                        return true;                                                                // Else, passed all validation checks for a legitimate overlap, hence return true.
                    case Direction.Down:
                        while (--y >= 0)
                            if (matrix[x, y] == '\0') break;                                        // First walk upwards until you reach the beginning of the word that is already on the board.
                        ++y;

                        for (int i = 0; y < Globals.gridCellCount && i < Globals.MAX_WORD_LENGTH; y++, i++) // Now walk downwards until you reach the end of the word that is already on the board.
                        {
                            if (matrix[x, y] == '\0') break;
                            chars[i] = matrix[x, y];
                        }

                        str = new string(chars);
                        str = str.Trim('\0');
                        wordOnBoard = (RegularWordDetails)wordDetails.Find(a => a.Word == str);     // See if the characters form a valid word that is already on the board.
                        if (wordOnBoard == null) return false;                                      // If this is not a word on the board, then this must be some random characters, hence not a legitimate word, hence this is a wrong placement.
                        if (wordOnBoard.WordDirection == Direction.Right) return false;             // If the word on the board is in parallel to the word on to be placed, then also this is a wrong placement as two words cannot be placed side by side in the same direction.
                        if (wordOnBoard.Y == originalY + 1) return false;                           // The word on the board starts right after the x-cordinate for the current word to place. Hence illegitimate.
                        return true;                                                                // Else, passed all validation checks for a legitimate overlap, hence return true.
                }
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in the LegitimateOverlapOfAnExistingWord() method of the 'GameEngine' class.\n\n" +
                                $"Original x = {originalX}, Original y = {originalY}, x = {x}, y = {y}, word: {word}, Direction: {direction}." +
                                $"\n\nError msg: {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// This method checks if any left-side characters of the word to be placed is a valid overlap from an existing ACROSS word on the board.
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
        bool LeftCellFreeForDownDirectedWord(int x, int y, string word)
        {
            try
            {
                if (x == 0) return true;
                bool isValid = true;
                if (x > 0)
                {
                    for (int i = 0; i < word.Length; y++, i++)
                    {
                        if (matrix[x - 1, y] != '\0')
                            isValid = LegitimateOverlapOfAnExistingWord(x, y, word, Direction.Left);
                        if (!isValid) break;
                    }
                }
                return isValid;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in the LeftCellFreeForDownDirectedWord() method of the 'GameEngine' class. x = {x}, y = {y}, word: {word}.\n\nError msg: {e.Message}");
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
        bool LeftCellFreeForRightDirectedWord(int x, int y, string word)
        {
            try
            {
                if (x == 0) return true;
                if (x - 1 >= 0)
                    return matrix[x - 1, y] == '\0';
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in the LeftCellFreeForRightDirectedWord() method of the 'GameEngine' class. x = {x}, y = {y}, word: {word}.\n\nError msg: {e.Message}");
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
        bool RightCellFreeForDownDirectedWord(int x, int y, string word)
        {
            try
            {
                if (x == Globals.gridCellCount) return true;
                bool isValid = true;
                if (x + 1 < Globals.gridCellCount)
                {
                    for (int i = 0; i < word.Length; y++, i++)
                    {
                        if (matrix[x + 1, y] != '\0')
                            isValid = LegitimateOverlapOfAnExistingWord(x, y, word, Direction.Right);
                        if (!isValid) break;
                    }
                }
                return isValid;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in the RightCellFreeForDownwardWord() method of the 'GameEngine' class. x = {x}, y = {y}, word: {word}.\n\nError msg: {e.Message}");
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
        bool RightMostCellFreeForRightDirectedWord(int x, int y, string word)
        {
            try
            {
                if (x + word.Length == Globals.gridCellCount) return true;
                if (x + word.Length < Globals.gridCellCount)
                    return matrix[x + word.Length, y] == '\0';
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in the RightMostCellFreeForRightDirectedWord() method of the 'GameEngine' class. x = {x}, y = {y}, word: {word}.\n\nError msg: {e.Message}");
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
        bool TopCellFreeForDownDirectedWord(int x, int y, string word)
        {
            try
            {
                if (y == 0) return true;
                if (y - 1 >= 0)
                    return matrix[x, y - 1] == '\0';
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in the TopCellFreeForDownDirectedWord() method of the 'GameEngine' class. x = {x}, y = {y}, word: {word}.\n\nError msg: {e.Message}");
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
        bool TopCellFreeForRightDirectedWord(int x, int y, string word)
        {
            try
            {
                if (y == 0) return true;
                bool isValid = true;
                if (y - 1 >= 0)
                {
                    for (int i = 0; i < word.Length; x++, i++)
                    {
                        if (matrix[x, y - 1] != '\0')
                            isValid = LegitimateOverlapOfAnExistingWord(x, y, word, Direction.Up);
                        if (!isValid) break;
                    }
                }
                return isValid;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in the TopCellFreeForRightDirectedWord() method of the 'GameEngine' class. x = {x}, y = {y}, word: {word}.\n\nError msg: {e.Message}");
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
        bool BottomCellFreeForRightDirectedWord(int x, int y, string word)
        {
            try
            {
                if (y == Globals.gridCellCount) return true;
                bool isValid = true;
                if (y + 1 < Globals.gridCellCount)
                {
                    for (int i = 0; i < word.Length; x++, i++)
                    {
                        if (matrix[x, y + 1] != '\0')
                            isValid = LegitimateOverlapOfAnExistingWord(x, y, word, Direction.Down);
                        if (!isValid) break;
                    }
                }
                return isValid;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in the BottomCellFreeForRightDirectedWord() method of the 'GameEngine' class. x = {x}, y = {y}, word: {word}.\n\nError msg: {e.Message}");
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
        bool BottomMostBottomCellFreeForDownDirectedWord(int x, int y, string word)
        {
            try
            {
                if (y + word.Length == Globals.gridCellCount) return true;
                if (y + word.Length < Globals.gridCellCount)
                    return matrix[x, y + word.Length] == '\0';
                return false;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in the BottomMostBottomCellFreeForDownDirectedWord() method of the 'GameEngine' class. x = {x}, y = {y}, word: {word}.\n\nError msg: {e.Message}");
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
        private bool PlaceTheWord(Direction direction, int x, int y, string word, string wordMeaning, int currentWordCount, ref long attempts)
        {
            try
            {
                attempts++;
                bool placeAvailable = true, overlapped = false;
                switch (direction)
                {
                    case Direction.Right:
                        for (int i = 0, xx = x; i < word.Length; i++, xx++) // First we check if the word can be placed in the array. For this it needs blanks there or the same letter (of another word) in the cell.
                        {
                            if (xx >= Globals.gridCellCount) return false;  // Falling outside the grid. Hence placement unavailable.
                            if (matrix[xx, y] != '\0')
                            {
                                if (matrix[xx, y] != word[i])   // If there is an overlap, then we see if the characters match. If matches, then it can still go there.
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
                        bool leftFree = LeftCellFreeForRightDirectedWord(x, y, word);

                        // If it is a right-direction, then check if it can cross through another word. E.g.: if CAT is the current word,
                        // then check if there is a valid crossing through existing words on the board - VICINITY, STICK:
                        // V
                        // I   S
                        // C A T
                        // I   I
                        // N   C
                        // I   K
                        // T
                        // Y
                        bool topFree = TopCellFreeForRightDirectedWord(x, y, word);

                        // If it is a right-direction, then check if it can cross through another word. E.g.: if CAT is the current word,
                        // then check if there is a valid crossing through existing words on the board - VICINITY, STICK:
                        // V
                        // I   S
                        // C A T
                        // I   I
                        // N   C
                        // I   K
                        // T
                        // Y
                        bool bottomFree = BottomCellFreeForRightDirectedWord(x, y, word);

                        // If it is a right-direction, then there should not be any character in the adjacent cell to the right of the ending of this word.
                        // E.g.: If CAT is to be placed, then it cannot be placed with another word BUS as C A T B U S
                        bool rightMostFree = RightMostCellFreeForRightDirectedWord(x, y, word);

                        // If cells that need to be free are not free, then this word cannot be placed there.
                        if (!leftFree || !topFree || !bottomFree || !rightMostFree) return false;

                        // If all the cells are blank, or a non-conflicting overlap is available, then this word can be placed there. So place it.
                        for (int i = 0, j = x; i < word.Length; i++, j++)
                            matrix[j, y] = word[i];
                        SaveWordDetailsInCollection(word, wordMeaning, x, y, direction, attempts, false);
                        return true;
                    case Direction.Down:
                        for (int i = 0, yy = y; i < word.Length; i++, yy++)     // First we check if the word can be placed in the array. For this it needs blanks there or the same letter (of another word) in the cell.
                        {
                            if (yy >= Globals.gridCellCount) return false;      // Falling outside the grid. Hence placement unavailable.
                            if (matrix[x, yy] != '\0')
                            {
                                if (matrix[x, yy] != word[i])                   // If there is an overlap, then we see if the characters match. If matches, then it can still go there.
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
                        leftFree = LeftCellFreeForDownDirectedWord(x, y, word);

                        // If it is a down-direction, then there should not be any character in the adjacent cell to the right of the beginning of this word.
                        // E.g.: If CAT is to be placed downwards, then it cannot be placed with another word CLASS as
                        // C C L A S S
                        // A
                        // T
                        bool rightFree = RightCellFreeForDownDirectedWord(x, y, word);

                        // If it is a down-direction, then there should not be any character in the adjacent cell to the top of the beginning of this word.
                        // E.g.: If CAT is to be placed downwards, then it cannot be placed with another word CLASS as
                        // C L A S S
                        // C
                        // A
                        // T
                        topFree = TopCellFreeForDownDirectedWord(x, y, word);

                        // If it is a down-direction, then there should not be any character in the adjacent cell to the bottom of the end of this word.
                        // E.g.: If CAT is to be placed downwards, then it cannot be placed with another word CLASS as                        
                        // C
                        // A
                        // T
                        // C L A S S
                        bool bottomMostBottomFree = BottomMostBottomCellFreeForDownDirectedWord(x, y, word);

                        // If cells that need to be free are not free, then this word cannot be placed there.
                        if (!leftFree || !rightFree || !topFree || !bottomMostBottomFree) return false;

                        // If all the cells are blank, or a non-conflicting overlap is available, then this word can be placed there. So place it.
                        for (int i = 0, j = y; i < word.Length; i++, j++)
                            matrix[x, j] = word[i];
                        SaveWordDetailsInCollection(word, wordMeaning, x, y, direction, attempts, false);
                        return true;
                }
                return false;   // Otherwise continue with a different place and index.
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'PlaceTheWords()' method of the 'GameEngine' class. Error msg: {e.Message}");
                return false;   // Otherwise continue with a different place and index.
            }
        }

        /// <summary>
        /// Keeps the word and details in the collection.
        /// </summary>
        /// <param name="word"></param>
        /// <param name="wordMeaning"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="direction"></param>
        /// <param name="attempts"></param>
        /// <param name="failedMaxAttempts"></param>
        private void SaveWordDetailsInCollection(string word, string wordMeaning, int x, int y, Direction direction, long attempts, bool failedMaxAttempts)
        {
            try
            {
                RegularWordDetails details = new RegularWordDetails();
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
                MessageBox.Show($"An error occurred in 'SaveWordDetailsInCollection()' method of the 'GameEngine' class. Error msg: {e.Message}");
            }
        }
    }
}
