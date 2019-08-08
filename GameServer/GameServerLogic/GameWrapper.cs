using Remote.InGameObjects;
using Utils.GameLogicUtils;
using System;
using Utils.Net;
using System.Collections.Generic;

namespace GameServer.GameServerLogic
{
    public class GameWrapper
    {
        private AsyncUserToken[] m_tokens;

        public GameWrapper(Game game)
        {
            Game = game ?? throw new ArgumentNullException(nameof(game));
            PlayersReadyStatus = new bool[Game.Players.Length];
        }

        public void Reset(params LinkedList<Card>[] decks)
        {
            IsReady = false;
            for(int i = 0; i < PlayersReadyStatus.Length; i++)
            {
                PlayersReadyStatus[i] = false;
            }
            Game.Reset(decks);
            m_tokens = null;
        }

        public object @lock { get; private set; } = new object();
        public bool IsReady { get; set; } = false;
        public bool[] PlayersReadyStatus { get; private set; }
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