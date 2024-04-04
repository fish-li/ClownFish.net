using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest;

#if NETCOREAPP

[TestClass]
public class ClownFishOptionsTest
{
    [TestMethod]
    public void Test1()
    {
        int a = ClownFishOptions.MemoryStreamPool_BlockSize;
        int b = ClownFishOptions.MemoryStreamPool_LargeBufferMultiple;
        int c = ClownFishOptions.MemoryStreamPool_MaximumBufferSize;

        int d = ClownFishOptions.StringBuilderPool_InitialCapacity;
        int e = ClownFishOptions.StringBuilderPool_MaximumRetainedCapacity;
        int f = ClownFishOptions.StringBuilderPool_MaximumRetained;

        int g = ClownFishOptions.AsyncBackgroundTask_WaitSeconds1;
        int h = ClownFishOptions.AsyncBackgroundTask_WaitSeconds2;

        bool i = ClownFishOptions.ShowBadHttpRequestException;
        int j = ClownFishOptions.MinMessageLength;
        bool k = ClownFishOptions.JsonSerializer_CreateDefault;
        bool l = ClownFishOptions.JsonSerializer_CamelCase;
        string m = ClownFishOptions.IndexNameTimeFormat;
    }
}

#endif
