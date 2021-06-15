using CrossWord.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossWord.Interfaces
{
    interface IDetails
    {
        string Word { get; set; }
        string WordMeaning { get; set; }
        int X { get; set; }
        int Y { get; set; }
        Direction WordDirection { get; set; }
        long AttemptsCount { get; set; }
        bool FailedMaxAttempts { get; set; }
        bool Isolated { get; set; }
        int OutputSequence { get; set; }                         // For output only.
    }
}
