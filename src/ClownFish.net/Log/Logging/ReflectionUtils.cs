#if NETCOREAPP
namespace ClownFish.Log.Logging;

internal static class ReflectionUtils
{
    public static T Get<T>(this object data, string propName)
    {
        PropertyInfo p = data.GetType().GetProperty(propName, BindingFlags.Instance | BindingFlags.Public);
        if( p == null )
            throw new ArgumentOutOfRangeException(nameof(propName));

        return (T)p.FastGetValue(data);
    }
}
#endif
