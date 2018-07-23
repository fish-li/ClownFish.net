using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Log
{
    internal static class LockObject<T>
    {
        public static readonly object SyncObject = new object();
    }
}
