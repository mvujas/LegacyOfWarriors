using Remote.InGameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.GameLogicUtils
{
    public enum PossibleCardPlace
    {
        FIELD,
        HAND,
        DECK,
        GRAVEYARD
    }

    public class PlayerInGame
    {
        public Dictionary<PossibleCardPlace, LinkedList<CardInGame>> Cards { get; private set; } = 
            new Dictionary<PossibleCardPlace, LinkedList<CardInGame>>();
        private Dictionary<int, PossibleCardPlace> m_cardGameIdToPlaceMapping;
        private Dictionary<int, CardInGame> m_cardGameIdToCardInGameMapping = new Dictionary<int, CardInGame>();
        public int Health { get; set; }
        public int Mana { get; set; }

        public int CurrentFatiqueDamage { get; set; }

        private int m_maxHandSize = ProjectLevelConfig.GameConfig.MAX_HAND_SIZE;

        public PossibleCardPlace[] places = null;

        public PlayerInGame()
        {
            places = Enum.GetValues(typeof(PossibleCardPlace)).Cast<PossibleCardPlace>().ToArray(); ;
            foreach (var place in places)
            {
                Cards[place] = null;
            }
        }

        public void Reset(int startingHealth, LinkedList<CardInGame> startingDeck)
        {
            Health = startingHealth;
            ResetCards();
            Cards[PossibleCardPlace.DECK] = startingDeck;
            m_cardGameIdToPlaceMapping = new Dictionary<int, PossibleCardPlace>();
            foreach (var cardInGame in startingDeck)
            {
                m_cardGameIdToCardInGameMapping[cardInGame.InGameId] = cardInGame;
                m_cardGameIdToPlaceMapping[cardInGame.InGameId] = PossibleCardPlace.HAND;
            }
        }

        private void ResetCards()
        {
            m_cardGameIdToCardInGameMapping.Clear();
            foreach(var place in places)
            {
                if(place != PossibleCardPlace.DECK)
                {
                    Cards[place] = new LinkedList<CardInGame>();
                }
            }
        }

        /// <param name="cardGameId">CardInGame Identifier</param>
        /// <param name="place">The place where the card is if the player owns it</param>
        /// <returns>Whether the players owns the card</returns>
        public bool DoesPlayerOwnCard(int cardGameId, out PossibleCardPlace place)
        {
            return m_cardGameIdToPlaceMapping.TryGetValue(cardGameId, out place);
        }

        public bool MoveCard(int cardGameId, PossibleCardPlace newPlace)
        {
            try
            {
                PossibleCardPlace oldPlace = m_cardGameIdToPlaceMapping[cardGameId];
                CardInGame cardInGame = m_cardGameIdToCardInGameMapping[cardGameId];
                Cards[oldPlace].Remove(cardInGame);

                m_cardGameIdToPlaceMapping[cardGameId] = newPlace;
                Cards[newPlace].AddLast(cardInGame);
                return true;
            }
            catch(Exception)
            {
                return false;
            }
        }

        public bool GetCard(int cardGameId, out CardInGame card)
        {
            return m_cardGameIdToCardInGameMapping.TryGetValue(cardGameId, out card);
        }

        public CardDrawingOutcome MoveFirstFromDeckToHand(out CardInGame card)
        {
            card = null;
            var deck = Cards[PossibleCardPlace.DECK];
            var hand = Cards[PossibleCardPlace.HAND];
            CardInGame topOfTheDeck = deck.FirstOrDefault();
            if (topOfTheDeck == default)
            {
                return CardDrawingOutcome.EMPTY_DECK;
            }
            card = topOfTheDeck;
            deck.RemoveFirst();
            if (hand.Count == m_maxHandSize)
            {
                m_cardGameIdToPlaceMapping[card.InGameId] = PossibleCardPlace.GRAVEYARD;
                return CardDrawingOutcome.FULL_HAND;
            }

            m_cardGameIdToPlaceMapping[card.InGameId] = PossibleCardPlace.HAND;
            Cards[PossibleCardPlace.HAND].AddLast(card);
            return CardDrawingOutcome.SUCCESSFUL;
        }
    }
}
