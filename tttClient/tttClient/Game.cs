using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tttClient
{
    public class Game
    {
        public int Id { get; set; }
        public int[] Players { get; set; }
        public int[] Gameboard { get; set; }

        public int Hsize { get; set; }
        public int Vsize { get; set; }
        public int Winner { get; set; }

        public Game()
        {
            
        }

        public Game(int id, int[] players, int[] gameboard, int hsize, int vsize, int winner)
        {
            Id = id;
            Players = players;
            Gameboard = gameboard;
            Hsize = hsize;
            Vsize = vsize;
            Winner = winner;
        }
    }
}
