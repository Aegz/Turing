using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Diagnostics
{
    class ReasonMessageLibrary
    {
        private static Dictionary<int, String> Library = new Dictionary<int, string>()
        {
            { 8010, "Node is enforcing uniqueness and a duplicate was added" },
        };

        public static String GetReasonMessage(int xiIndex)
        {
            if (Library.ContainsKey(xiIndex))
            {
                return Library[xiIndex];
            }
            else
            {
                return "No such Reason Code";
            }
        }
    }
}
