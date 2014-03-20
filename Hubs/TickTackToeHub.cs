using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Diagnostics;


namespace RealTimeMorpion.Hubs
{
    public class TickTackToeHub : Hub
    {

        public static Game currentGame = new Game(new ScoreService());
        /// <summary>
        /// The count of users connected.
        /// </summary>
        public static List<string> Users = new List<string>();


        //SC = Server Call
        //SC_Play(string coords, int player) / SC_Reset()

        //CC = Client Call
        //cc_refresh(int[,]) / cc_reset() / cc_updateUsersOnlineCount(int clientCount) / cc_initialize(int[,])

        #region Server methods
        public void sC_Play(string coords, int player)
        {
            string[] coordarray = coords.Split(',');
            int x, y = -1;
            if (coordarray.Length == 2
                && int.TryParse(coordarray[0], out x)
                && int.TryParse(coordarray[1], out y)
                )
            {
              
                int isItWon = currentGame.Play(x, y);
              
                if (isItWon == -1)
                {
                    Clients.All.cc_refresh(currentGame.Board, currentGame.CurrrentPlayer,false);

                }
                else
                {
                    
                    Clients.All.cc_gameEnd(currentGame.Board, isItWon);
                }

            }
        }

        public void sC_Reset()
        {
            currentGame = currentGame.Reset();
            Clients.All.cc_reset();
        }
        #endregion Client -> Server

        #region SignalR events

        /// <summary>
        /// The OnConnected event.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override System.Threading.Tasks.Task OnConnected()
        {
            string clientId = GetClientId();

            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);
            }

            int score1, score2 = 0;
            currentGame.GetScore(out score1,out score2);
            Clients.Caller.cc_join(currentGame.Board, currentGame.CurrrentPlayer,score1,score2 );
            // Send the current count of users
            SendClientCount(Users.Count);

            return base.OnConnected();
        }

        /// <summary>
        /// The OnReconnected event.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override System.Threading.Tasks.Task OnReconnected()
        {
            string clientId = GetClientId();
            if (Users.IndexOf(clientId) == -1)
            {
                Users.Add(clientId);
            }
            Clients.Caller.cc_refresh(currentGame.Board,true);
            // Send the current count of users
            SendClientCount(Users.Count);

            return base.OnReconnected();
        }

        /// <summary>
        /// The OnDisconnected event.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public override System.Threading.Tasks.Task OnDisconnected()
        {

            string clientId = GetClientId();

            if (Users.IndexOf(clientId) > -1)
            {
                Users.Remove(clientId);
            }

            // Send the current count of users
            SendClientCount(Users.Count);

            return base.OnDisconnected();
        }

        #endregion 

        #region utilities
        private string GetClientId()
        {
            string clientId = "";
            if (Context.QueryString["clientId"] != null)
            {
                // clientId passed from application 
                clientId = this.Context.QueryString["clientId"];
            }

            if (string.IsNullOrEmpty(clientId.Trim()))
            {
                clientId = Context.ConnectionId;
            }

            return clientId;
        }

        public void SendClientCount(int clientCount)
        {
            Clients.All.cc_updateUsersOnlineCount(clientCount);
        }
        #endregion 

    }
}