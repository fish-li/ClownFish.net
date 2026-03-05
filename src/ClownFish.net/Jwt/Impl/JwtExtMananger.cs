namespace ClownFish.Jwt.Impl;

internal static class JwtExtMananger
{
    private static readonly TSafeDictionary<string, JwtBase> s_dict = new TSafeDictionary<string, JwtBase>();

    private static bool s_initialized = false;
    private static readonly object s_lock = new object();

    internal static void Init()
    {
        if( s_initialized == false ) {
            lock( s_lock ) {
                if( s_initialized == false ) {
                    Init0();
                    s_initialized = true;
                }
            }
        }
    }


    [UnconditionalSuppressMessage("Trimming", "IL2065: type.GetField")]
    [UnconditionalSuppressMessage("Trimming", "IL2075: type.GetField")]
    private static void Init0()
    {
        Type[] types = (from t in typeof(JwtExtMananger).Assembly.GetPublicTypes()
                        where t.IsClass && t.IsAbstract == false && t.IsSubclassOf(typeof(JwtBase))
                        select t).ToArray();

        foreach( Type type in types ) {
            FieldInfo field = type.GetField("Instance", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if( field != null ) {
                JwtBase instance = (JwtBase)field.GetValue(null);
                RegisterJwtmImpl(instance);
            }
        }
    }


    public static void RegisterJwtmImpl(JwtBase instance)
    {
        if( instance == null )
            throw new ArgumentNullException(nameof(instance));

        string name = instance.Name;
        if( name.IsNullOrEmpty() )
            throw new ValidationException2("instance.Name is null");

        string header = instance.GetHeader();
        if( header.IsNullOrEmpty() )
            throw new ValidationException2("instance.GetHeader() return null");

        s_dict[instance.Name] = instance;
    }

    public static void RegisterAlgorithmImpl(IJwtAlgorithm2 jwtAlgorithm)
    {
        if( jwtAlgorithm == null )
            throw new ArgumentNullException(nameof(jwtAlgorithm));

        string name = jwtAlgorithm.Name;
        if( name.IsNullOrEmpty() )
            throw new ValidationException2("jwtAlgorithm.Name is null");

        string header = jwtAlgorithm.GetHeaderJson();
        if( header.IsNullOrEmpty() )
            throw new ValidationException2("jwtAlgorithm.GetHeaderJson() return null");

        s_dict[name] = new JwtExtImpl(jwtAlgorithm);
    }

    public static JwtBase GetImpl(string name)
    {
        return s_dict.TryGet(name);
    }
}