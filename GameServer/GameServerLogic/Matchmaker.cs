﻿using Remote.InGameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Net;

namespace GameServer.GameServerLogic
{
    public class UserQueueWrapper
    {
        public AsyncUserToken Token { get; set; }
        public LinkedList<Card> Deck { get; set; }
    }

    public class Matchmaker
    {
        private Matchmaker() { }
        private static Matchmaker instance = new Matchmaker();
        public static Matchmaker GetInstance()
        {
            return instance;
        }

        private UserQueueWrapper userWaitingForMatch = null;
        private object matchingLock = new object();
        private static GameManager GAME_MANAGER = GameManager.GetInstance();

        public void AddUserToQueue(UserQueueWrapper queueWrapper)
        {
            AsyncUserToken userToken = queueWrapper.Token;
            ServerSideTokenIdentity identity = userToken.info as ServerSideTokenIdentity;
            lock(identity.MatchmakingLock)
            {
                if(identity.MatchmakingStatus != UserMatchmakingStatus.LOBBY)
                {
                    throw new MatchmakingException("Korisnik nije u lobiju ili je u redu");
                }
                AddOrMatch(queueWrapper);
            }
        }

        private void AddOrMatch(UserQueueWrapper queueWrapper)
        {
            UserQueueWrapper otherPlayer = null;
            lock (matchingLock)
            {
                AsyncUserToken userToken = queueWrapper.Token;
                if (userWaitingForMatch == null)
                {
                    userWaitingForMatch = queueWrapper;
                    ((ServerSideTokenIdentity)userToken.info).MatchmakingStatus = UserMatchmakingStatus.QUEUE;
                }
                else
                {
                    otherPlayer = userWaitingForMatch;
                    userWaitingForMatch = null;
                }
            }
            if(otherPlayer != null)
            {
                GAME_MANAGER.CreateGame(queueWrapper, otherPlayer);
            }
        }

        public void ExitQueue(AsyncUserToken token)
        {
            ServerSideTokenIdentity identity = (ServerSideTokenIdentity)token.info;
            lock(matchingLock)
            {
                lock(identity.MatchmakingLock)
                {
                    if(userWaitingForMatch == null || userWaitingForMatch.Token != token)
                    {
                        throw new MatchmakingException("Korisnik ne ceka u redu");
                    }
                    identity.MatchmakingStatus = UserMatchmakingStatus.LOBBY;
                    userWaitingForMatch = null;
                }
            }
        }
    }
}
