
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RealTimeMorpion
{
    public class Game
    {
        public int[,] Board { get; set; }
        public int CurrrentPlayer { get; set; }

        private IScoreService scoreService;

        public Game(IScoreService service)
        {
            Board = getNewBoard();
            CurrrentPlayer = 0;
            this.scoreService = service;
        }

        public int Play(int x, int y)
        {
           
            Board[x, y] = CurrrentPlayer;
            int result = GetWinner();
            if (result != -1)
            {
                Task.Run(() => this.scoreService.Score((result == 0) || (result == -2), (result == 1) || (result == -2))); 
            }
         
            CurrrentPlayer = getNextPlayer();
            return result;
        }

        private int[,] getNewBoard()
        {
            int[,] result = new int[,] { 
            { -1, -1, -1 }, {-1, -1, -1 } , { -1, -1, -1 }
            };
            return result;
        }


        internal Game Reset()
        {
            Board = getNewBoard();
            CurrrentPlayer = 0;
            return this;
        }

        private int getNextPlayer()
        {
            return (CurrrentPlayer + 1) % 2;
        }


        private int GetWinner()
        {

            if (checkHorizontal() || checkVertival() || checkDiagonal() || checkAntiDiag())
            {
                return CurrrentPlayer;
            }

            if (checkForTie())
            {
                return -2;
            }


            return -1;

        }

        private bool checkForTie()
        {
            bool result = true;
            foreach (int square in Board)
            {
                result &= square != -1;
            }
            return result;
        }

        private bool checkAntiDiag()
        {

            bool result = true;
            int player1 = Board[1, 1];
            for (int delta = -1; delta < 2; delta++)
            {
                result &= (player1 == Board[1 + delta, 1 - delta] && player1 != -1);
            }
            return result;
        }

        private bool checkDiagonal()
        {


            bool result = true;
            int player1 = Board[1, 1];
            for (int delta = -1; delta < 2; delta++)
            {
                result &= (player1 == Board[1 + delta, 1 + delta] && player1 != -1);
            }
            return result;
        }

        private bool checkVertival()
        {
            bool result = true;

            for (int j = 0; j < 3; j++)
            {
                result = true;
                int player1 = Board[0, j];
                for (int i = 0; i < 3; i++)
                {
                    result &= (player1 != -1 && player1 == Board[i, j]);
                }

                if (result)
                {
                    break;
                }
            }
            return result;
        }

        private bool checkHorizontal()
        {
            bool result = true;

            for (int i = 0; i < 3; i++)
            {
                result = true;
                int player1 = Board[i, 0];
                for (int j = 0; j < 3; j++)
                {
                    result &= (player1 != -1 && player1 == Board[i, j]);
                }

                if (result)
                {
                    break;
                }
            }
            return result;
        }

        internal void GetScore(out int score1, out int score2)
        {
            this.scoreService.GetScore(out score1, out score2);
        }
    }
}