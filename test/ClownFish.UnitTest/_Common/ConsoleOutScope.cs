using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest._Common;
public sealed class ConsoleOutScope : IDisposable
{
    public readonly string OutFilePath = Path.Combine(AppContext.BaseDirectory, "temp/_consoleout.txt");

    public ConsoleOutScope()
    {
        Console2.SetOutToFile(OutFilePath, 0);
    }

    public void Dispose()
    {
        Console2.ResetOut();
    }

    public string GetText()
    {
        return RetryFile.ReadAllText(OutFilePath);
    }
}
