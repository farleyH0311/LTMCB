using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordUp.Models
{
    public class Flashcard
    {
        public Front front { get; set; }
        public Back back { get; set; }
    }

    public class Front
    {
        public string Word { get; set; }
        public string Phonetic { get; set; }
        public string Definition { get; set; }
    }

    public class Back
    {
        public string Meaning { get; set; }
    }

}

