namespace ClownFish.Jwt;

internal static class NbJwtBase64UrlEncoder
{
    public static string Encode(byte[] input)
    {
        if( input == null || input.Length == 0 )
            return string.Empty;

        return ReplaceChars(Convert.ToBase64String(input));
    }

    private static string ReplaceChars(string base64String)
    {
        StringBuilder sb = StringBuilderPool.Get();
        try {
            foreach( char c in base64String ) {
                if( c == '+' )
                    sb.Append('-');
                else if( c == '/' )
                    sb.Append('_');
                else if( c != '=' )
                    sb.Append(c);
            }
            return sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }
    }

#if NETCOREAPP
    public static string Encode(ReadOnlySpan<byte> input)
    {
        if( input == null || input.Length == 0 )
            return string.Empty;

        return ReplaceChars(Convert.ToBase64String(input));
    }
#endif


    public static byte[] Decode(string input)
    {
        if( string.IsNullOrEmpty(input) ) 
            return Empty.Array<byte>();

        string text = null;
        StringBuilder sb = StringBuilderPool.Get();
        try {
            sb.Append(input);
            switch( input.Length % 4 ) {
                case 0:
                    break;
                case 2:
                    sb.Append("==");
                    break;
                case 3:
                    sb.Append('=');
                    break;
                default:
                    throw new FormatException("Illegal base64 string.");
            }

            sb.Replace('-', '+').Replace('_', '/');
            text = sb.ToString();
        }
        finally {
            StringBuilderPool.Return(sb);
        }

        return Convert.FromBase64String(text);
    }
}
