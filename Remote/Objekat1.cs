using System;
using System.Collections.Generic;
using System.Text;

namespace Remote
{
    [Serializable]
    public class Objekat1: Objekat
    {
        public int ocene = 5;
        public override string ToString()
        {
            return base.ToString() + ", ocene " + ocene;
        }
    }
}
