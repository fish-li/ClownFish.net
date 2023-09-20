#if NETCOREAPP3_1

using System.ComponentModel;

namespace System.Runtime.CompilerServices
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal class IsExternalInit{}
}

namespace ClownFish.Base
{
    internal static class XXXX
    {
        internal static bool TryReset(this CancellationTokenSource cancellationTokenSource)
        {
            // .netcore 3.1 根本没有这个方法，为了让代码统一，所以增加这样一个 “空方法”
            return false;
        }
    }
}


#endif
