using ProjectLevelConfig;
using Remote.InGameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.GameLogicUtils
{
    public class LogicExecutionEngine
    {
        public virtual void AttackCard(Game game, int playerIndex, int cardThatAttacks, int attackedPlayer, int cardToBeAttacked, 
            out int attackerRemainingHealth, out int targetRemainingHealth)
        {
            if(playerIndex > game.Players.Length || attackedPlayer > game.Players.Length)
            {
                throw new LogicExecutionException("Bar jedan od igraca je nevalidan");
            }

            int attackerOwner = game.IndexOfPlayerThatOwnsCard(cardThatAttacks);
            int attackedOwner = game.IndexOfPlayerThatOwnsCard(cardToBeAttacked);
            if(playerIndex != attackerOwner)
            {
                throw new LogicExecutionException("Igrac ne poseduje datu kartu");
            }
            if (attackedPlayer != attackedOwner)
            {
                throw new LogicExecutionException("Napadnuti igrac ne poseduje datu kartu");
            }
            if (attackedOwner == playerIndex)
            {
                throw new LogicExecutionException("Igrac ne moze napasti svoju kartu");
            }
            var attackerOwnerPlayer = game.Players[attackerOwner];
            var attackedOwnerPlayer = game.Players[attackedOwner];
            attackerOwnerPlayer.DoesPlayerOwnCard(cardThatAttacks, out PossibleCardPlace attackerPlace);
            attackedOwnerPlayer.DoesPlayerOwnCard(cardToBeAttacked, out PossibleCardPlace attackedPlace);
            if(attackerPlace != PossibleCardPlace.FIELD || attackedPlace != PossibleCardPlace.FIELD)
            {
                throw new LogicExecutionException("Neka od karata nije na terenu");
            }
            attackerOwnerPlayer.GetCard(cardThatAttacks, out CardInGame attacker);
            attackedOwnerPlayer.GetCard(cardToBeAttacked, out CardInGame attacked);

            attacked.Health -= attacker.Attack;
            attacker.Health -= attacked.Attack;

            attackerRemainingHealth = Math.Max(0, attacker.Health);
            targetRemainingHealth = Math.Max(0, attacked.Health);

            if (attacked.Health <= 0)
            {
                Die(attackedOwnerPlayer, attacked);
            }
            if (attacker.Health <= 0)
            {
                Die(attackerOwnerPlayer, attacker);
            }
        }

        /// <returns>Whether attacked player dies</returns>
        public bool AttackPlayer(Game game, int playerIndex, int cardThatAttacks, int playerIndexToBeAttacked, 
            out int attackerRemainingHealth, out int targetRemainingHealth)
        {
            if (playerIndex > game.Players.Length || playerIndexToBeAttacked > game.Players.Length)
            {
                throw new LogicExecutionException("Bar jedan od igraca je nevalidan");
            }

            if (playerIndex == playerIndexToBeAttacked)
            {
                throw new LogicExecutionException("Igrac ne moze napasti samog sebe");
            }
            PlayerInGame attacker = game.Players[playerIndex];
            PlayerInGame attacked = game.Players[playerIndexToBeAttacked];
            if(!attacker.DoesPlayerOwnCard(cardThatAttacks, out PossibleCardPlace place) ||
                place != PossibleCardPlace.FIELD)
            {
                throw new LogicExecutionException("Karta nije na igracevom terenu");
            }

            attacker.GetCard(cardThatAttacks, out CardInGame card);
            attacked.Health -= card.Attack;

            attackerRemainingHealth = card.Health;
            targetRemainingHealth = Math.Max(0, attacked.Health);

            return attacked.Health <= 0;
        }

        private void Die(PlayerInGame owner, CardInGame card)
        {
            card.Health = card.Card.Health;
            card.Cost = card.Card.Cost;
            card.Attack = card.Card.Attack;
            owner.MoveCard(card.InGameId, PossibleCardPlace.GRAVEYARD);
        }

        public virtual CardInGame PlayCard(Game game, int playerIndex, int cardToBePlayed)
        {
            if(game.IndexOfPlayerWhoPlayTheTurn != playerIndex)
            {
                throw new LogicExecutionException("Igrač nije na potezu");
            }
            PlayerInGame player = game.Players[playerIndex];
            if (!player.DoesPlayerOwnCard(cardToBePlayed, out PossibleCardPlace place) || place != PossibleCardPlace.HAND)
            {
                throw new LogicExecutionException("Data karta nije u igračevoj ruci");
            }
            if(!player.Cards.TryGetValue(PossibleCardPlace.FIELD, out LinkedList<CardInGame> field) || field.Count >= GameConfig.FIELD_SIZE)
            {
                throw new LogicExecutionException("Nema više mesta na terenu");
            }
            player.GetCard(cardToBePlayed, out CardInGame cardInGame);
            if(player.Mana < cardInGame.Cost)
            {
                throw new LogicExecutionException("Igrač nema dovoljno mane da bi igrao kartu");
            }
            player.Mana -= cardInGame.Cost;
            player.MoveCard(cardToBePlayed, PossibleCardPlace.FIELD);
            return cardInGame;
        }

        public virtual CardDrawingOutcome NewTurn(Game game, out int nextPlayerIndex, out CardInGame card, out int mana, out int fatiqueDamage, bool isFirstTurn = false)
        {
            if(isFirstTurn)
            {
                game.IndexOfPlayerWhoPlayTheTurn = 0;
                game.AccumulativeTurn = 0;
            }
            else
            {
                game.IndexOfPlayerWhoPlayTheTurn = (game.IndexOfPlayerWhoPlayTheTurn + 1) % game.Players.Length;
                game.AccumulativeTurn++;
            }
            nextPlayerIndex = game.IndexOfPlayerWhoPlayTheTurn;
            var player = game.Players[nextPlayerIndex];
            player.Mana = Math.Min((game.AccumulativeTurn / game.Players.Length) + 1, GameConfig.MAX_MANA);
            mana = player.Mana;
            var outcome =  player.MoveFirstFromDeckToHand(out card);
            fatiqueDamage = 0;
            if(outcome == CardDrawingOutcome.EMPTY_DECK)
            {
                player.CurrentFatiqueDamage++;
                player.Health -= player.CurrentFatiqueDamage;
                fatiqueDamage = player.CurrentFatiqueDamage;
            }
            return outcome;
        }

        public void DrawStartingHand(Game game, int playerIndex, int initialHandSize)
        {
            var player = game.Players[playerIndex];
            for(int i = 0; i < initialHandSize; i++)
            {
                player.MoveFirstFromDeckToHand(out _);
            }
        }
    }
}
