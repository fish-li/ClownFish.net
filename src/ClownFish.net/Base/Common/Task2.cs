namespace ClownFish.Base;

/// <summary>
/// Task2
/// </summary>
public static class Task2
{
    /// <summary>
    /// Delay without TaskCanceledException
    /// </summary>
    /// <param name="millisecondsDelay"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task Delay(int millisecondsDelay, CancellationToken cancellationToken)
    {
        try {
            await Task.Delay(millisecondsDelay, cancellationToken);
        }
        catch( TaskCanceledException ) { }
    }
}
