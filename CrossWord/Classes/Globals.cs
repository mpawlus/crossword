// ***************************************************************************************************************************************
// ****** Mehedi Shams Rony: Dec 2018*****************************************************************************************************
// ****** Purpose: Class that contains global variables and common methods. **************************************************************
// ***************************************************************************************************************************************
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Text;
using WordPuzzle.Classes;

namespace CrossWord.Classes
{
    public enum Direction { Down = 1, Right, Left, Up, None };
    static class Globals
    {
        public static int gridCellCount = Convert.ToInt16(ConfigurationManager.AppSettings["GRID_SIZE"]);
        public static long MAX_ATTEMPTS = Convert.ToInt32(ConfigurationManager.AppSettings["MAX_ATTEMPTS"]);
        public static int MAX_WORD_LENGTH = Convert.ToInt16(ConfigurationManager.AppSettings["MAX_WORD_LENGTH"]);
        public static int MAX_UNICODE_WORD_LENGTH = Convert.ToInt16(ConfigurationManager.AppSettings["MAX_UNICODE_WORD_LENGTH"]);
        public static int MAX_NON_OVERLAPPING_WORDS_THRESHOLD = Convert.ToInt16(ConfigurationManager.AppSettings["MAX_NON_OVERLAPPING_WORDS_THRESHOLD"]);
        public enum CurrentWordType { Regular, Unicode };
        public const string SAVE_FILTER = "JSON File (*.json)|*.json|All Files (*.*)|*.*";
        public const string SAVE_TITLE = "Save the words and clues";
        public const string OPEN_TITLE = "Open a JSON file that has words and clues";

        private static readonly int MOUSE_X_CALIBRATION_PIXELS = Convert.ToInt16(ConfigurationManager.AppSettings["MOUSE_X_CALIBRATION_PIXELS"]);  // Calibration adjustment for the X-coordinate of the mouse, and also for placement of letters in the middle of boxes.
        
        public static void ScaleBoardIfNecessary(out WindowScaler scaler, Rectangle bounds, out int scaleFactor, out int calibrationFactor)
        {
            scaler = new WindowScaler(bounds);
            scaler.SetMultiplicationFactor();

            scaleFactor = scaler.GetMetrics(35);
            calibrationFactor = scaler.GetMetrics(MOUSE_X_CALIBRATION_PIXELS);
        }

        /// <summary>
        /// Concatenates the individual pieces of the composite Unicode letter and returns it as a a string.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="CharacterMatrix"></param>
        /// <returns></returns>
        public static string GetCompositeLetterFromTheMatrix(int x, int y, char[,,] CharacterMatrix)
        {
            char[] atomElements = new char[MAX_WORD_LENGTH];
            int z = 0;
            while (CharacterMatrix[x, y, z] != '\0')
                atomElements[z] = CharacterMatrix[x, y, z++];
            return new string(atomElements).Trim('\0');
        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endianness fails.
        /// https://stackoverflow.com/questions/3825390/effective-way-to-find-any-files-encoding
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        public static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; //UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; //UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.ASCII;
        }
    }
}
