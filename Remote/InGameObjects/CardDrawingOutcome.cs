using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Remote.InGameObjects
{
    [Serializable]
    public enum CardDrawingOutcome
    {
        SUCCESSFUL,
        FULL_HAND,
        EMPTY_DECK
    }
}
