using Remote.InGameObjects;
using Utils.GameLogicUtils;
using System;
using Utils.Net;

namespace GameServer.GameServerLogic
{
    public class GameWrapper
    {
        private AsyncUserToken[] m_tokens;

        public GameWrapper(Game game)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
        }

        public void Reset()
        {
            IsReady = false;
            Game.Reset();
            m_tokens = null;
        }

        public bool IsReady { get; set; } = false;
        public Game Game { get; private set; }
        public AsyncUserToken[] Tokens
        {
            get => m_tokens;
            set
            {
                if(value.Length != Game.Players.Length)
                {
                    throw new ArgumentException("Invalid user tokens array size");
                }
                m_tokens = value;
            }
        }
    }
}