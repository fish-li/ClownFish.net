using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Log.Writers;

namespace AotTestConsoleApp1.Utils;
public sealed class ConsoleWriter : ILogWriter
{
    public void Init(LogConfiguration config, WriterConfig section)
    {
    }

    void ILogWriter.WriteList<T>(List<T> list)
    {
        foreach( T item in list ) {
            Console2.Info($"ConsoleWriter: {typeof(T).FullName}: {item.ToJson()}");
        }
    }
}
