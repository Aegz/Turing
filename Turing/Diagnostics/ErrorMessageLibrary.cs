using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Diagnostics
{
    class ErrorMessageLibrary
    {
        private static Dictionary<int, String> Library = new Dictionary<int, string>()
        {
            { 8000, @"Expecting ({0}), Found ({1})" },
            { 8001, @"Unexpected Token Found ({0})." },
            { 8002, @"{0} was not terminated properly." },
            { 8003, @"Missing {0}" },

            { 8010, @"Attempted to add an invalid {1} to {2} due to: {0}." },
        };

        public static StatusItem GetErrorMessage(int xiIndex, params String[] args)
        {
            
            if (Library.ContainsKey(xiIndex))
            {
                return new StatusItem(String.Format(Library[xiIndex], args));
            }
            else
            {
                return new StatusItem("No such Error");
            }
        }

    }
}
