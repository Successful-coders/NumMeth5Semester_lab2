using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com_Methods
{
    interface IIterationSolver
    {
        int MaxIterationCount { set; get; }
        double Eps { set; get; }
        int IterrationIndex { set; get; }
    }
}
