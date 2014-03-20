using GDataDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RealTimeMorpion
{
    public class Score
    {
        public Score()
        { }
        public Score(int p1, int p2)
        {
            Player1 = p1;
            Player2 = p2;
        }


        public int Player1 { get; set; }

        public int Player2 { get; set; }
    }

    public class ScoreService : IScoreService
    {
        public void GetScore(out int player1, out int player2)
        {
            player1 = 2;
            player2 = 42;

            try
            {
                var client = new DatabaseClient("PUT_YOUR_GDRIVE_ACCOUNT", "PUT_YOUR_GDRIVE_PASSWORD");
                const string dbName = "RealtimeMorpion";
                var db = client.GetDatabase(dbName) ?? client.CreateDatabase(dbName);
                const string tableName = "Results";
                var t = db.GetTable<Score>(tableName) ?? db.CreateTable<Score>(tableName);

                var all = t.FindAll();

                var r = all.Count > 0 ? all[0] : t.Add(new Score(0, 0)); ;

                player1 = r.Element.Player1;
                player2 = r.Element.Player2;

            }
            catch
            {

            }

        }

        public void Score(bool player1, bool player2)
        {
            try
            {
                var client = new DatabaseClient("garett.chicago@gmail.com", "Pass@word01");
                const string dbName = "RealtimeMorpion";
                var db = client.GetDatabase(dbName) ?? client.CreateDatabase(dbName);
                const string tableName = "Results";
                var t = db.GetTable<Score>(tableName) ?? db.CreateTable<Score>(tableName);

                var all = t.FindAll();

                var r = all.Count > 0 ? all[0] : t.Add(new Score(0, 0)); ;

                if (player1)
                {
                    r.Element.Player1++;
                }
                if (player2)
                {
                    r.Element.Player2++;
                }



                r.Update();
            }
            catch
            {

            }

        }
    }
    public interface IScoreService
    {
        void Score(bool player1, bool player2);
        void GetScore(out int player1, out int player2);
    }
}