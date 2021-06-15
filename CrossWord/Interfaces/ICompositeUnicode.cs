using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossWord.Interfaces
{
    interface ICompositeUnicode
    {
        List<string> CompositeUnicodeLetters { get; set; }       // For unicode only.
    }
}
