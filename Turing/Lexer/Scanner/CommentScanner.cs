﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Diagnostics;
using Turing.Factories;
using Turing.Syntax;
using Turing.Utilities;

namespace Turing.Lexer.Scanner
{
    class CommentScanner
    {

        public static SyntaxTrivia ScanSingleLineComment(SlidingTextWindow TextWindow)
        {
            // Check next char
            char xcNextChar = TextWindow.PeekCharacter();

            // If it is two hypens together
            if (xcNextChar == '-' && TextWindow.PeekCharacter(1) == '-')
            {
                // Ignore Comment syntax (we just want text)
                TextWindow.SkipAhead(2);
                Boolean bIsCommentTerminated = false;
                String sCommentLine = String.Empty;

                // Iterate indefinitely until we reach the end
                for (int iIndex = 0; TextWindow.Position + iIndex < TextWindow.Count ; iIndex++)
                {
                    // NewLine char (Terminated properly)
                    if (SyntaxUtilities.IsNewLineCharacter(TextWindow.PeekCharacter(iIndex)))
                    {
                        // Break the loop and the comment is terminated properly
                        bIsCommentTerminated = true;
                        break;
                    }

                    // Keep adding characters to our intermediate var
                    sCommentLine += TextWindow.PeekCharacter(iIndex);
                }

                // Just advance the length of the text (we can't fix peoples comments 100%
                // of the time)
                TextWindow.SkipAhead(sCommentLine.Length);

                // If our comment was terminated properly
                if (bIsCommentTerminated)
                {
                    return SyntaxFactory.SingleLineComment(sCommentLine);
                }
                // Not terminated properly.. (hardest case)
                // If we reach the end of the window its either an EOF comment (which is unlikely)
                // or it is an unterminated comment
                else
                {
                    // Intermediate Var (Add all the text as a comment)
                    SyntaxTrivia oCommentNode = SyntaxFactory.SingleLineComment(sCommentLine);

                    // Add the erroraneous message
                    oCommentNode.Comments.Add(ErrorMessageLibrary.GetErrorMessage(8002, "Single Line Comment")); 

                    // return the comment node with everything in comments
                    return oCommentNode;
                }

            }

            // This token should not be here (there is something wrong)

            // Intermediate Var (Add all the text as a comment)
            SyntaxTrivia oErraneousToken = new SyntaxTrivia(
                SyntaxKind.UnknownNode, // Unknown
                Convert.ToString(TextWindow.PeekCharacter())); // Get the character out of the stream

            oErraneousToken.Comments.Add(ErrorMessageLibrary.GetErrorMessage(8001, Convert.ToString(TextWindow.PopCharacter())));
         
            // return the comment node with everything in comments
            return oErraneousToken;
        }

        private static Boolean CheckCommentForKeywords(String xsCommentText)
        {
            String[] asIndividualTokens = xsCommentText.Split();

            for (int iIndex = 0; iIndex < asIndividualTokens.Length; iIndex++)
            {
                String sWord = asIndividualTokens[iIndex];

                if (SyntaxKindUtilities.ContainsKey(sWord))
                {
                    return true;
                }
            }

            return false;
        }

        public static SyntaxTrivia ScanMultiLineComment(SlidingTextWindow TextWindow)
        {
            // Check next char
            char xcNextChar = TextWindow.PeekCharacter();

            // If it is /*
            if (xcNextChar == '/' && TextWindow.PeekCharacter(1) == '*')
            {
                // Ignore comment sections
                TextWindow.SkipAhead(2);
                Boolean bIsCommentTerminated = false;
                String sCommentLine = String.Empty;

                // Iterate indefinitely until we reach the end
                for (int iIndex = 0; TextWindow.Position + iIndex < TextWindow.Count; iIndex++)
                {
                    // Terminating Case (find * and then /)
                    if (TextWindow.PeekCharacter(iIndex) == '*' && TextWindow.PeekCharacter(iIndex + 1) == '/')
                    {
                        bIsCommentTerminated = true;
                        break;
                    }

                    // Keep adding characters to our intermediate var
                    sCommentLine += TextWindow.PeekCharacter(iIndex);
                }
                
                // Just advance the length of the text (we can't fix peoples comments 100%
                // of the time)
                TextWindow.SkipAhead(sCommentLine.Length);

                // If our comment was terminated properly
                if (bIsCommentTerminated)
                {
                    // Ignore the */
                    TextWindow.SkipAhead(2);
                    return SyntaxFactory.MultiLineComment(sCommentLine);
                }
                // Not terminated properly.. (hardest case)
                // If we reach the end of the window its either an EOF comment (which is unlikely)
                // or it is an unterminated comment
                else
                {
                    // Intermediate Var (Add all the text as a comment)
                    SyntaxTrivia oCommentNode = SyntaxFactory.MultiLineComment(sCommentLine);

                    oCommentNode.Comments.Add(ErrorMessageLibrary.GetErrorMessage(8002, "Multi Line Comment"));


                    // return the comment node with everything in comments
                    return oCommentNode;
                }

            }

            // This token should not be here (there is something wrong)

            // Intermediate Var (Add all the text as a comment)
            SyntaxTrivia oErraneousToken = new SyntaxTrivia(
                SyntaxKind.UnknownNode, // Unknown
                Convert.ToString(TextWindow.PeekCharacter())); // Get the character out of the stream

            // 
            oErraneousToken.Comments.Add(ErrorMessageLibrary.GetErrorMessage(8001, Convert.ToString(TextWindow.PopCharacter())));

            // return the comment node with everything in comments
            return oErraneousToken;
        }
    }
}
