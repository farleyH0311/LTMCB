using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordUp.Models
{
    public class Player
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public bool Ready { get; set; }
    }

    public class PlayerScore
    {
        public string Name { get; set; }
        public int Score { get; set; }
    }
}

