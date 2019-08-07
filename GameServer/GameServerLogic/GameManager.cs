using GameServer.Logic;
using ProjectLevelConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.DataTypes;
using Utils.Delegates;
using Utils.GameLogicUtils;
using Utils.Net;
using Remote.Implementation;

namespace GameServer.GameServerLogic
{
    public class GameManager
    {
        private ObjectPool<GameWrapper> m_gameWrapperObjectPool;

        private GameManager()
        {
            Supplier<GameWrapper> gameWrapperSupplier = () =>
                new GameWrapper(new Game(GameConfig.NUMBER_OF_PLAYERS, GameConfig.STARTING_HEALTH, GameConfig.DECK_SIZE));
            m_gameWrapperObjectPool = new ObjectPool<GameWrapper>(gameWrapperSupplier, 10, growable: true);
        }
        private static GameManager instance = new GameManager();
        public static GameManager GetInstance()
        {
            return instance;
        }

        public void PrepareGame(params UserQueueWrapper[] players)
        {
            if(players.Length != 2)
            {
                throw new ArgumentException("Invalid number of players in game!");
            }
            AsyncUserToken[] userTokens = players.Select(player => player.Token).ToArray();
            var decks = players.Select(player => player.Deck).ToArray();
            ServerSideTokenIdentity[] identities = userTokens.Select(token => (ServerSideTokenIdentity)token.info).ToArray();
            lock (identities[0].MatchmakingLock)
            {
                lock (identities[1].MatchmakingLock)
                {
                    var gameWrapper = m_gameWrapperObjectPool.GetObject();
                    gameWrapper.Reset(decks);
                    gameWrapper.Tokens = userTokens;
                    
                    Config.GameServer.Send(userTokens[0], new GameFoundNotification
                    {
                        EnemyInfo = UserLogic.GetUserInfo(identities[1].LastlyFetchedUser)
                    });
                    Config.GameServer.Send(userTokens[1], new GameFoundNotification
                    {
                        EnemyInfo = UserLogic.GetUserInfo(identities[0].LastlyFetchedUser)
                    });
                    
                }
            }
        }
    }
}
