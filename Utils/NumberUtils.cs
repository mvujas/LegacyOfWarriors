using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public static class NumberUtils
    {
        public static bool FloatEquals(float a, float b, float checkDifference = 1e-6f)
        {
            return Math.Abs(a - b) < checkDifference;
        }
    }
}
