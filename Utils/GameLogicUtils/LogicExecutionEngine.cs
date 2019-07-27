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
        public virtual void AttackCard(Game game, int playerIndex, int cardThatAttacks, int cardToBeAttacked)
        {
            if(game.IndexOfPlayerWhoPlayTheTurn != playerIndex)
            {
                // Baci izuzetak
            }

            int attackerOwner = game.IndexOfPlayerThatOwnsCard(cardThatAttacks);
            int attackedOwner = game.IndexOfPlayerThatOwnsCard(cardToBeAttacked);
            if(playerIndex != attackerOwner || attackerOwner == playerIndex)
            {
                // Baci izuzetak
            }
            var attackerOwnerPlayer = game.Players[attackerOwner];
            var attackedOwnerPlayer = game.Players[attackedOwner];
            attackerOwnerPlayer.DoesPlayerOwnCard(cardThatAttacks, out PossibleCardPlace attackerPlace);
            attackedOwnerPlayer.DoesPlayerOwnCard(cardToBeAttacked, out PossibleCardPlace attackedPlace);
            if(attackerPlace != PossibleCardPlace.FIELD || attackedPlace != PossibleCardPlace.FIELD)
            {
                // Baci izuzetak
            }
            attackerOwnerPlayer.GetCard(cardThatAttacks, out CardInGame attacker);
            attackedOwnerPlayer.GetCard(cardThatAttacks, out CardInGame attacked);

            attacked.Health -= attacker.Attack;
            attacker.Health -= attacked.Attack;
            if(attacked.Health <= 0)
            {
                Die(attackedOwnerPlayer, attacked);
            }
            if (attacker.Health <= 0)
            {
                Die(attackerOwnerPlayer, attacker);
            }
        }

        /// <returns>Whether attacked player dies</returns>
        public bool AttackPlayer(Game game, int playerIndex, int cardThatAttacks, int playerIndexToBeAttacked)
        {
            if(game.IndexOfPlayerWhoPlayTheTurn != playerIndex)
            {
                // Baci izuzetak
            }
            if(playerIndex == playerIndexToBeAttacked)
            {
                // Baci izuzetak
            }
            PlayerInGame attacker = game.Players[playerIndex];
            PlayerInGame attacked = game.Players[playerIndexToBeAttacked];
            if(!attacker.DoesPlayerOwnCard(cardThatAttacks, out PossibleCardPlace place) ||
                place != PossibleCardPlace.FIELD)
            {
                // Baci izuzetak
            }

            attacker.GetCard(cardThatAttacks, out CardInGame card);
            attacked.Health -= card.Attack;
            return attacked.Health <= 0;
        }

        private void Die(PlayerInGame owner, CardInGame card)
        {
            card.Health = card.Card.Health;
            card.Cost = card.Card.Cost;
            card.Attack = card.Card.Attack;
            owner.MoveCard(card.InGameId, PossibleCardPlace.GRAVEYARD);
        }

        public virtual void PlayCard(Game game, int playerIndex, int cardToBePlayed)
        {
            if(game.IndexOfPlayerWhoPlayTheTurn != playerIndex)
            {
                // Baci izuzetak
            }
            PlayerInGame player = game.Players[playerIndex];
            if (!player.DoesPlayerOwnCard(cardToBePlayed, out PossibleCardPlace place) || place != PossibleCardPlace.HAND)
            {
                // Baci izuzetak
            }
            player.MoveCard(cardToBePlayed, PossibleCardPlace.FIELD);
        }

        public virtual CardDrawingOutcome NewTurn(Game game, out int nextPlayerIndex, out CardInGame card)
        {
            nextPlayerIndex = ++game.IndexOfPlayerWhoPlayTheTurn;
            var player = game.Players[nextPlayerIndex];
            return player.MoveFirstFromDeckToHand(out card); 
        }
    }
}
