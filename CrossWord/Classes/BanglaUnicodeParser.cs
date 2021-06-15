// ***************************************************************************************************************************************
// ****** Mehedi Shams Rony: Dec 2018*****************************************************************************************************
// ****** Purpose: Main crossword logic for parsing unicode letters and return them as a combined composite letter. **********************
// ***************************************************************************************************************************************
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace CrossWord.Classes
{
    class BanglaUnicodeParser
    {
        public List<string> ParseWord(string word)
        {
            char[] letters = word.ToCharArray();
            List<string> individualLetters = new List<string>();
            StringBuilder builder = new StringBuilder();
            try
            {
                for (int i = 0; i < letters.Length;)
                {
                    do
                        builder.Append(letters[i]);
                    while (ThisIsPartOfACompositeLetter(letters[i], i++, letters));

                    individualLetters.Add(builder.ToString());
                    builder.Clear();
                }
                return individualLetters;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'ParseWord()' method of the 'BanglaUnicodeParser' class. Error msg: {e.Message}");
                return null;
            }
            finally
            {
                builder = null;
                individualLetters = null;
            }
        }

        private bool ThisIsPartOfACompositeLetter(char v, int currentIndex, char[] letters)
        {
            try
            {
                if (currentIndex == letters.Length - 1) return false;       // End of the word; so this is not part of the current composite letter.

                if (ThisIsOnusshor(v)) return false;                        // 'Onusshor' is a separate element of its own, hence trigger a termination of the loop to continue from next on.
                if (ThisIsBishorgho(v)) return false;                       // 'Bishorgho' is a separate element of its own, hence trigger a termination of the loop to continue from next on.
                if (ThisIsChondroBindu(v)) return false;                    // 'Chondro bindu' must be the last element of a composite, hence trigger a termination.
                if (ThisIsKhondoTo(v)) return false;                        // 'Khondo to' is a separate element of its own, hence trigger a termination of the loop to continue from next on.

                if (IndividualVowel(v)) return false;                   // If this is a vowel, then this is of its own, hence not part of a composite letter. E.g.: Hrossho i.
                if (VowelConnector(v, letters[currentIndex + 1])) return false;

                if (NextElementIsAConnector(v, letters[currentIndex + 1])) return true;
                if (NextElementIsConnected(v, currentIndex, letters)) return true;
                if (ConsecutiveConsonants(v, currentIndex, letters)) return false;              // If the next unicode is another consonant, then also this is a consonent of its own, hence it is not part of a composite letter. E.g.: 'স', 'প' in "সপ্তর্ষি"
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show($"An error occurred in 'ThisIsPartOfACompositeLetter()' method of the 'BanglaUnicodeParser' class. Error msg: {e.Message}");
                return false;
            }
        }

        private bool ThisIsKhondoTo(char v)
        {
            return v == 0x09CE;
        }

        private bool ThisIsBishorgho(char v)
        {
            return v == 0x0983;
        }

        private bool ThisIsOnusshor(char v)
        {
            return v == 0x0982;
        }

        private bool ThisIsChondroBindu(char v)
        {
            return v == 0x0981;
        }

        private bool VowelConnector(char v, char nextCharacter)
        {
            if (v >= 0x09BE && v <= 0x09CC)
            {
                if (ThisIsChondroBindu(nextCharacter)) return false;
                else return true;
            }
            return false;
        }

        private bool NextElementIsConnected(char v, int currentIndex, char[] letters)
        {
            char nextElement = letters[currentIndex + 1];
            if (v == 2509)
                if ((nextElement >= 0x0995 && nextElement <= 0x09B9) || (nextElement >= 0x09DC && nextElement <= 0x09DF))
                    return true;
            return false;
        }

        private bool NextElementIsAConnector(char v1, char v2)
        {
            return v2 == 2509;
        }

        private bool IndividualVowel(char v)
        {
            return v >= 0x0985 && v <= 0x0994;
        }

        private bool ConsecutiveConsonants(char v, int currentIndex, char[] letters)
        {
            char nextElement = letters[currentIndex + 1];
            if ((v >= 0x0995 && v <= 0x09B9) || (v >= 0x09DC && v <= 0x09DF))
                if ((nextElement >= 0x0995 && nextElement <= 0x09B9) || (nextElement >= 0x09DC && nextElement <= 0x09DF))
                    return true;
            return false;
        }
    }
}