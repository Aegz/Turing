using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Lexer
{
    /// <summary>
    /// A simple encapsulated class designed to 
    /// </summary>
    public class SlidingTextWindow
    {
        // Queue up all text
        List<Char> acAllText;


        int iCurrentPosition = 0;

        public SlidingTextWindow(String xsAllText)
        {
            acAllText = new List<char>(xsAllText);
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

        public char PeekCharacter(int xiExtraAmount)
        {
            // If there is actually anything left to peek
            if (iCurrentPosition + xiExtraAmount < acAllText.Count)
            {
                return acAllText[iCurrentPosition + xiExtraAmount];
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

        public String PopCharacter(int xiLength = 999999999)
        {
            // initialise a return variable
            String sReturn = String.Empty;

            // Scan until we reach the given length or the end of the string
            for (int iIndex = 0; iIndex < xiLength && iIndex + iCurrentPosition < acAllText.Count; iIndex++)
            {
                sReturn += PopCharacter();
            }

            // Return the variable
            return sReturn;
        }


        public Boolean HasCharactersLeftToProcess()
        {
            return iCurrentPosition < acAllText.Count;
        }

        public int Count
        {
            get
            {
                return acAllText.Count;
            }
        }

        public int Position
        {
            get
            {
                return iCurrentPosition;
            }
        }

        public void SkipAhead(int xiLength)
        {
            iCurrentPosition += xiLength;
        }
        
    }
}
