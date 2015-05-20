using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Turing.Syntax.Constructs.Keywords;

namespace Turing.Syntax
{
    /// <summary>
    /// Use this class to retrieve the appropriate compatibility
    /// matrices for each type. This should be done through cloning
    /// As each Kind will be removed from the list when it is consumed
    /// </summary>
    class SyntaxCompatibilityMatrix
    {
        #region Compatibility Matrix

        private Dictionary<SyntaxKind, List<SyntaxKind>> CompatabilityMatrixKind = new Dictionary<SyntaxKind, List<SyntaxKind>>
        {
            #region Statements

            // Create [TEMP] Table IDN AS [(] SELECT [)] Distribute ON Identifier
            { SyntaxKind.CreateKeyword, new List<SyntaxKind>
                {
                    { SyntaxKind.TempKeyword },
                    { SyntaxKind.TableKeyword },
                    { SyntaxKind.ViewKeyword },

                    { SyntaxKind.IdentifierToken },

                    // Bracket or Select
                    { SyntaxKind.OpenParenthesisToken },
                    { SyntaxKind.SelectKeyword },

                    { SyntaxKind.DistributeKeyword },
                    { SyntaxKind.OnKeyword },

                    { SyntaxKind.IdentifierToken },
                }
            },

            { SyntaxKind.SelectKeyword, new List<SyntaxKind>
                {
                    { SyntaxKind.FromKeyword },
                    { SyntaxKind.WhereKeyword },
                    { SyntaxKind.GroupByKeyword },
                    { SyntaxKind.OrderByKeyword },
                    { SyntaxKind.LimitKeyword },
                }
            },

            #endregion
        };

        #endregion

        };
    }
}
