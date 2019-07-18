using System;
using System.Collections.Generic;
using System.Text;

namespace Remote
{
    [Serializable]
    public class Objekat
    {
        public String ime;
        public int godiste;

        public override string ToString()
        {
            return $"Objekat {ime}, godiste {godiste}";
        }
    }
}
