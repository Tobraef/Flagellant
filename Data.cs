using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant
{
    public enum Severity : int
    {
        Great = -2,
        Good = -1,
        Ok = 0,
        Bad = 1,
        Critical = 2,
        Chaos = 3
    }

    public static class Data
    {
        public const int workToBreakRatio = 5;
    }
}
