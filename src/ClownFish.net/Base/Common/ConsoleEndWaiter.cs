using System.Runtime.InteropServices;

namespace ClownFish.Base;
// 参考：Microsoft.Extensions.Hosting.Internal.ConsoleLifetime
//       https://github.com/dotnet/runtime/issues/50527

#if NET6_0_OR_GREATER

/// <summary>
/// 控制台等待事件包装类
/// </summary>
public sealed class ConsoleEndWaiter : IDisposable
{
    private PosixSignalRegistration _sigIntRegistration;

    private PosixSignalRegistration _sigQuitRegistration;

    private PosixSignalRegistration _sigTermRegistration;

    private readonly ManualResetEvent _exitEvent = new ManualResetEvent(false);

    /// <summary>
    /// 等待退出事件
    /// </summary>
    public void Wait()
    {
        RegisterShutdownHandlers();
        _exitEvent.WaitOne();
    }

    private void RegisterShutdownHandlers()
    {
        Action<PosixSignalContext> handler = HandlePosixSignal;
        _sigIntRegistration = PosixSignalRegistration.Create(PosixSignal.SIGINT, handler);
        _sigQuitRegistration = PosixSignalRegistration.Create(PosixSignal.SIGQUIT, handler);
        _sigTermRegistration = PosixSignalRegistration.Create(PosixSignal.SIGTERM, handler);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public void Dispose()
    {
        UnregisterShutdownHandlers();
    }

    private void UnregisterShutdownHandlers()
    {
        _sigIntRegistration?.Dispose();
        _sigQuitRegistration?.Dispose();
        _sigTermRegistration?.Dispose();
    }

    internal void HandlePosixSignal(PosixSignalContext context)
    {
        Console2.Info($"##### PosixSignal: {context.Signal}");
        context.Cancel = true;
        _exitEvent.Set();
    }
}
#endif
