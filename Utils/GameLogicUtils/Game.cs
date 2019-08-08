using Remote.InGameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.DataTypes;

namespace Utils.GameLogicUtils
{
    public class Game
    {
        public PlayerInGame[] Players { get; private set; }
        public int IndexOfPlayerWhoPlayTheTurn { get; set; }

        public int AccumulativeTurn { get; set; }

        private int m_currentAvaliableCardGameId;
        private ObjectPool<CardInGame> m_cardInGamePool;
        private int m_startingHealth;
        private int m_deckSize;

        public Game(int numberOfPlayers, int startingHealth, int deckSize)
        {
            if(numberOfPlayers < 1)
            {
                throw new ArgumentException("Number of players must be positive number");
            }
            if (deckSize < 1)
            {
                throw new ArgumentException("Deck size must be positive number");
            }
            if (startingHealth < 1)
            {
                throw new ArgumentException("Starting health must be positive number");
            }
            Players = new PlayerInGame[numberOfPlayers];
            for(int i = 0; i < numberOfPlayers; i++)
            {
                Players[i] = new PlayerInGame();
            }
            m_deckSize = deckSize;
            m_startingHealth = startingHealth;
            m_cardInGamePool = new ObjectPool<CardInGame>(() => new CardInGame(), m_deckSize * 30);
        }

        public void Reset(params LinkedList<Card>[] decks)
        {
            if(decks.Length != Players.Length)
            {
                throw new ArgumentException("There must be as much decks as there are players");
            }
            m_currentAvaliableCardGameId = 0;
            IndexOfPlayerWhoPlayTheTurn = 0;
            AccumulativeTurn = 0;
            for (int i = 0; i < decks.Length; i++)
            {
                var deck = decks[i];
                var player = Players[i];
                if (deck.Count != m_deckSize)
                {
                    throw new ArgumentException("Invalid deck size!");
                }
                player.Reset(m_startingHealth, TransformDeckToInGameDeck(deck));
            }
        }

        private static Random shuffleRandom = new Random();
        private LinkedList<CardInGame> TransformDeckToInGameDeck(LinkedList<Card> deck)
        {
            var result = new LinkedList<CardInGame>();
            foreach (var card in deck)
            {
                var cardInGame = CardToCardInGame(card);
                // Brainless shuffling
                if (shuffleRandom.NextDouble() < 0.5) {
                    result.AddLast(cardInGame);
                }
                else
                {
                    result.AddFirst(cardInGame);
                }
            }
            return result;
        }

        private CardInGame CardToCardInGame(Card card)
        {
            var cardInGame = m_cardInGamePool.GetObject();
            cardInGame.SetCard(card);
            cardInGame.InGameId = m_currentAvaliableCardGameId++;
            return cardInGame;
        }

        public int IndexOfPlayerThatOwnsCard(int cardInGameId)
        {
            if (cardInGameId >= m_deckSize * Players.Length || cardInGameId < 0)
            {
                throw new IndexOutOfRangeException();
            }
            return cardInGameId / m_deckSize;
        }

    }
}
