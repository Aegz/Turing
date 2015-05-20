using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLValidator.Lexer
{
    /// <summary>
    /// A simple encapsulated class designed to 
    /// </summary>
    class SlidingTextWindow
    {
        public static readonly char NEW_LINE_FLAG = '\n';

        // Queue up all text
        List<Char> acAllText;

        // buffer used to enable rollbacks where necessary
        List<Char> acBuffer;

        int iCurrentPosition = 0;

        public SlidingTextWindow(String xsAllText)
        {
            String sCleanedText = CleanseText(xsAllText);
            acAllText = new List<char>(sCleanedText);
            acBuffer = new List<char>();
        }

        private String CleanseText(String xsUncleanText)
        {
            // Intermediate var
            String NewLineFlag = Convert.ToString(NEW_LINE_FLAG);

            // Replace anything inappropriate or hard to catch
            return xsUncleanText
                // New Line Specific
                .Replace("\n\r", NewLineFlag)
                .Replace("\r", NewLineFlag);
        }

        public char PeekCharacter()
        {
            // If there is actually anything left to peek
            if (iCurrentPosition < acAllText.Count)
            {
                return acAllText[iCurrentPosition];
            }
            else
            {
                throw new Exception("Sliding Window out of bounds");
            }

        }

        public char PopCharacter()
        {
            // Get the character out
            char cTemp = PeekCharacter();
            // Move our index along
            iCurrentPosition++;
            // return the character
            return cTemp;
        }

        public void CacheCharacter()
        {
            acBuffer.Add(PopCharacter());
        }

        public String GetCachedText()
        {
            // Simply return the list as a string
            String sReturn = String.Join("", acBuffer);

            // Flush out the buffer
            acBuffer.Clear();

            return sReturn;
        }

        public Boolean HasCharactersLeftToProcess()
        {
            return iCurrentPosition < acAllText.Count;
        }

        public String ScanAheadCertainLength(int xiLength = 999999999)
        {
            // initialise a return variable
            String sReturn = String.Empty;

            // Scan until we reach the given length or the end of the string
            for (int iIndex = 0; iIndex < xiLength && iIndex + iCurrentPosition < acAllText.Count; iIndex++)
            {
                sReturn += acAllText[iIndex + iCurrentPosition];
            }

            // Return the variable
            return sReturn;
        }

        public String ScanAheadForCharacters(char[] xacCharactersToLookOutFor)
        {
            // Initialise return var
            String sReturn = "";

            // Loop through the remaining text
            for (int iIndex = 0; iIndex < acAllText.Count - iCurrentPosition; iIndex++ )
            {
                // Intermediate var
                char cCurrentItem = acAllText[iCurrentPosition + iIndex];
                // Add the character to the return text
                sReturn += cCurrentItem;
                // If we find the item we want
                if (xacCharactersToLookOutFor.Contains(cCurrentItem))
                {
                    // return immediately
                    return sReturn;
                }
            }

            // Default to empty/invalid
            return String.Empty;
        }

        public void SkipAhead(int xiLength)
        {
            iCurrentPosition += xiLength;
        }

        public void EnqueueInCache(IEnumerable<char> xacGiven)
        {
            acBuffer.AddRange(xacGiven);
        }
    }
}
