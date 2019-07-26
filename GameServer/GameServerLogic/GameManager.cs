using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.GameServerLogic
{
    public class GameManager
    {
        private GameManager() { }
        private static GameManager instance = new GameManager();
        public static GameManager GetInstance()
        {
            return instance;
        }

        public void CreateGame(UserQueueWrapper player1, UserQueueWrapper player2)
        {
            throw new NotImplementedException();
        }
    }
}
