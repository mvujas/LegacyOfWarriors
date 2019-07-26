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
            int attackerOwner = game.IndexOfPlayerThatOwnsCard(cardThatAttacks);
            int attackedOwner = game.IndexOfPlayerThatOwnsCard(cardToBeAttacked);
            if(playerIndex != attackerOwner || attackerOwner == playerIndex)
            {
                // Baci izuzetak
            }


        }

        public virtual void Die(Game game, int cardThatDies)
        {

        }

        public virtual void PlayCard(Game game, int playerIndex, int cardToBePlayed)
        {

        }
    }
}
