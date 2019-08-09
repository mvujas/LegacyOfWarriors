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
using Remote.InGameObjects;

namespace GameServer.GameServerLogic
{
    // TO-DO: EXAMINE THREAD-SAFETY OF THIS CLASS
    public class GameManager
    {
        private ObjectPool<GameWrapper> m_gameWrapperObjectPool;
        private Random m_random = new Random();
        private LogicExecutionEngine executionEngine = new LogicExecutionEngine();

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

            Utils.ArrayUtils.Shuffle(ref players);

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
                    
                    for(int i = 0; i < players.Length; i++)
                    {
                        identities[i].GameWrapper = gameWrapper;

                        var playerIndex = i;
                        var enemyIndex = players.Length - 1 - i;
                        Config.GameServer.Send(userTokens[playerIndex], new GameFoundNotification
                        {
                            EnemyInfo = UserLogic.GetUserInfo(identities[enemyIndex].LastlyFetchedUser),
                            PlayersHealth = GameConfig.STARTING_HEALTH,
                            PlayersDeckSize = decks[playerIndex].Count,
                            EnemiesHealth = GameConfig.STARTING_HEALTH,
                            EnemiesDeckSize = decks[enemyIndex].Count
                        });
                    }
                }
            }
        }

        public void MarkUserAsReadyForGame(AsyncUserToken userToken)
        {
            var identity = (ServerSideTokenIdentity)userToken.info;
            // BEWARE THAT MATCHMAKING STATUS MAY CHANGE WHILE GAINING LOCK ON GAME WRAPPER
            if (identity.MatchmakingStatus != UserMatchmakingStatus.PREPARING_GAME)
            {
                throw new MatchmakingException("Korisnik se ne priprema za igru");
            }
            var gameWrapper = identity.GameWrapper;
            lock (gameWrapper.@lock)
            {
                if (gameWrapper.IsReady)
                {
                    throw new MatchmakingException("Igra je već spremna");
                }

                bool areAllReady = true;
                for (int i = 0; i < gameWrapper.Tokens.Length; i++)
                {
                    if (gameWrapper.Tokens[i] == userToken)
                    {
                        gameWrapper.PlayersReadyStatus[i] = true;
                    }
                    areAllReady &= gameWrapper.PlayersReadyStatus[i];
                }

                if (areAllReady)
                {
                    FinalizeGamePreparation(gameWrapper);
                    Console.WriteLine("Igra spremna");
                }
            }
        }

        private void FinalizeGamePreparation(GameWrapper gameWrapper)
        {
            lock(gameWrapper.@lock)
            {
                gameWrapper.IsReady = true;
                var game = gameWrapper.Game;
                for (int i = 0; i < gameWrapper.Tokens.Length; i++)
                {
                    executionEngine.DrawStartingHand(gameWrapper.Game, i, GameConfig.INITIAL_HAND_SIZE);
                    var token = gameWrapper.Tokens[i];
                    var tokenIdentity = (ServerSideTokenIdentity)token.info;
                    lock (tokenIdentity.MatchmakingLock)
                    {
                        tokenIdentity.MatchmakingStatus = UserMatchmakingStatus.GAME;
                        if (!game.Players[i].Cards.TryGetValue(PossibleCardPlace.HAND, out LinkedList<CardInGame> startingHand))
                        {
                            Console.WriteLine("No hand in player cards dictionary");
                            startingHand = new LinkedList<CardInGame>();
                        }
                        Config.GameServer.Send(token, new StartingUserGameState
                        {
                            PlayerIndex = i,
                            StartingDeck = startingHand
                        });
                    }
                }
                NextTurn(gameWrapper, true);
            }
        }

        private void NextTurn(GameWrapper gameWrapper, bool isFirstTurn = false)
        {
            lock(gameWrapper.@lock)
            {
                CardDrawingOutcome drawingOutcome = 
                    executionEngine.NewTurn(gameWrapper.Game, out int playerIndex, out CardInGame card, out int mana, out int fatiqueDamage, isFirstTurn);
                for(int i = 0; i < gameWrapper.Tokens.Length; i++)
                {
                    var token = gameWrapper.Tokens[i];
                    Config.GameServer.Send(token, new NewTurnNotification
                    {
                        PlayerIndex = playerIndex,
                        DrawnCard = (i == playerIndex) ? card : null,
                        DrawingOutcome = drawingOutcome,
                        Mana = mana,
                        FatiqueDamage = fatiqueDamage,
                        RemainingHealth = gameWrapper.Game.Players[playerIndex].Health
                    });
                }
            }
        }

        public void EndTurn(AsyncUserToken token)
        {
            // SAME AS METHOD MarkUserAsReadyForGame, TRY TO SOLVE IT LATER
            var identity = (ServerSideTokenIdentity)token.info;
            var gameWrapper = identity.GameWrapper;
            if(gameWrapper == null || identity.MatchmakingStatus != UserMatchmakingStatus.GAME)
            {
                throw new LogicExecutionException("Korisnik nije u igri");
            }
            lock(gameWrapper.@lock)
            {
                var game = gameWrapper.Game;
                if(gameWrapper.Tokens[game.IndexOfPlayerWhoPlayTheTurn] != token)
                {
                    throw new LogicExecutionException("Igrač nije na potezu");
                }
                NextTurn(gameWrapper);
                CheckIfPlayerDied(gameWrapper);
            }
        }

        public void PlayCard(AsyncUserToken userToken, int cardInGameId)
        {
            var identity = (ServerSideTokenIdentity)userToken.info;
            var gameWrapper = identity.GameWrapper;
            if (gameWrapper == null || identity.MatchmakingStatus != UserMatchmakingStatus.GAME)
            {
                throw new LogicExecutionException("Korisnik nije u igri");
            }
            lock (gameWrapper.@lock)
            {
                var game = gameWrapper.Game;
                if (gameWrapper.Tokens[game.IndexOfPlayerWhoPlayTheTurn] != userToken)
                {
                    throw new LogicExecutionException("Igrač nije na potezu");
                }

                var playedCard = executionEngine.PlayCard(game, game.IndexOfPlayerWhoPlayTheTurn, cardInGameId);

                var cardPlayedNotification = new CardPlayedNotification
                {
                    PlayerIndex = game.IndexOfPlayerWhoPlayTheTurn,
                    PlayedCard = playedCard,
                    RemainingMana = game.Players[game.IndexOfPlayerWhoPlayTheTurn].Mana
                };
                for (int i = 0; i < gameWrapper.Tokens.Length; i++)
                {
                    var token = gameWrapper.Tokens[i];
                    Config.GameServer.Send(token, cardPlayedNotification);
                }
            }
        }

        // This works only for 2 player game because only that is implemented in first iteration
        // 2 players can't loose health as cause of single action in current state of the game
        private void CheckIfPlayerDied(GameWrapper gameWrapper)
        {
            if (gameWrapper == null)
            {
                throw new ArgumentNullException(nameof(gameWrapper));
            }

            lock(gameWrapper.@lock)
            {
                var game = gameWrapper.Game;
                if(game.Players.Length != 2)
                {
                    throw new ArgumentException("Meyhod works only for 2 player games");
                }

                int? winner = null;
                for(int i = 0; i < game.Players.Length && winner == null; i++)
                {
                    if(game.Players[i].Health <= 0)
                    {
                        winner = 1 - i;
                    }
                }

                if(winner != null)
                {
                    GameFinishedNotification gameFinishedNotification = new GameFinishedNotification { WinnerPlayerId = (int)winner };

                    foreach(var token in gameWrapper.Tokens)
                    {
                        Config.GameServer.Send(token, gameFinishedNotification);
                    }

                    TerminateGame(gameWrapper);
                }
            }
        }

        private void TerminateGame(GameWrapper gameWrapper)
        {
            if (gameWrapper == null)
            {
                throw new ArgumentNullException(nameof(gameWrapper));
            }

            lock (gameWrapper.@lock)
            {
                foreach(var token in gameWrapper.Tokens)
                {
                    var identity = (ServerSideTokenIdentity)token.info;
                    lock(identity.MatchmakingLock)
                    {
                        identity.MatchmakingStatus = UserMatchmakingStatus.LOBBY;
                        identity.GameWrapper = null;
                    }
                }
                m_gameWrapperObjectPool.ReleaseObject(gameWrapper);
            }
        }
    }
}
