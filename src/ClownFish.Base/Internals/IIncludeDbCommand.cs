using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base.Internals
{
    internal interface IIncludeDbCommand
    {
        DbCommand Command { get; }
    }
}
