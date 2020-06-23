using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flagellant
{
    public interface IProcess
    {
        void Kill();
        bool PopWindow();
        string Name { get; }
        string PostFix { get; }
        bool IsAlive { get; }
    }
}
