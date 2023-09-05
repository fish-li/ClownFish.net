using Newtonsoft.Json.Serialization;

namespace ClownFish.Web.Security.Auth;

internal sealed class JwtJsonUserTypesBinder : ISerializationBinder
{
    private static readonly HashSet<Type> s_userInfoTypes  = new HashSet<Type>(32);
    public static readonly JwtJsonUserTypesBinder Instance = new JwtJsonUserTypesBinder();

    static JwtJsonUserTypesBinder()
    {
        s_userInfoTypes.Add(typeof(WebUserInfo));
    }


    [MethodImpl(MethodImplOptions.Synchronized)]
    public static void RegisterUserType<T>() where T : IUserInfo
    {
        s_userInfoTypes.Add(typeof(T));
    }


    public Type BindToType(string assemblyName, string typeName)
    {
        if( assemblyName == null )
            return s_userInfoTypes.SingleOrDefault(t => t.Name == typeName);
        else
            return Type.GetType(typeName + ", " + assemblyName, true);
    }

    public void BindToName(Type serializedType, out string assemblyName, out string typeName)
    {
        if( s_userInfoTypes.Contains(serializedType) ) {
            assemblyName = null;
            typeName = serializedType.Name;
        }
        else {
            assemblyName = serializedType.Assembly.GetName().Name;
            typeName = serializedType.FullName;
        }
    }
}
