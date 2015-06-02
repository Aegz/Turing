using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turing.Diagnostics
{
    public enum StatusCategory
    {
        Severe, // Missing, Exception etc.
        Medium, // Idn incorrect, table wrong etc.
        Minor, // Cosmetic
    }

    public class StatusItem
    {

        public String Message { get; set; }

        public StatusItem(String xsMessage)
        {
            Message = xsMessage;
        }
    }
}
