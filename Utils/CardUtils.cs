using Remote.InGameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public class CardUtilsException : Exception
    {
        public CardUtilsException(string message) : base(message)
        {
        }
    }

    public static class CardUtils
    {
        // ako je neka karta null baca NullReferenceException, ali ce se pretpostaviti da se ovo nece dogoditi sem ako programer ne pogresi
        public static int[] DeckToCardIds(List<Card> deck)
        {
            if(deck == null)
            {
                throw new CardUtilsException("Dek ne može biti null");
            }
            return deck.Select(card => card.Id).ToArray();
        }

        private static IEnumerable<Card> CardIdsToEnumerable(CardList cardList, int[] cardIds)
        {
            if(cardList == null)
            {
                throw new CardUtilsException("Neispravna lista karata");
            }
            if(cardIds == null)
            {
                throw new CardUtilsException("Lista id-ova karata ne može biti null");
            }
            return cardIds.Select(id =>
                                {
                                    Card card = cardList.GetCardById(id);
                                    if (card == null)
                                    {
                                        throw new CardUtilsException("U deku postoji karta koja ne postoji u listi karata");
                                    }
                                    return card;
                                });
        }

        public static List<Card> CardIdsToList(CardList cardList, int[] cardIds)
        {
            return CardIdsToEnumerable(cardList, cardIds).ToList();
        }

        public static LinkedList<Card> CardIdsToLinkedList(CardList cardList, int[] cardIds)
        {
            LinkedList<Card> result = new LinkedList<Card>();
            foreach(var card in CardIdsToEnumerable(cardList, cardIds))
            {
                result.AddLast(card);
            }
            return result;
        }
    }
}
