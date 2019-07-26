using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils.GameLogicUtils
{
    public interface IGameExecutionEngine
    {
        void AttackCard(Game game, int playerIndex, int cardThatAttacks, int cardToBeAttacked);
        void PlayCard(Game game, int playerIndex, int cardToBePlayed);
        void Die(Game game, int cardThatDies);
    }
}
