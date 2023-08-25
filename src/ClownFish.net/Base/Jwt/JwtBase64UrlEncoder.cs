namespace ClownFish.Base.Jwt;

internal static class NbJwtBase64UrlEncoder
{
    public static string Encode(byte[] input)
    {
        if( input == null || input.Length == 0 ) {
            throw new ArgumentNullException(nameof(input));
        }

        //return Convert.ToBase64String(input).FirstSegment('=').Replace('+', '-').Replace('/', '_');

        StringBuilder sb = StringBuilderPool.Get();
        try {
            foreach( char c in Convert.ToBase64String(input) ) {
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


    //public static string FirstSegment(this string input, char separator)
    //{
    //    int num = input.IndexOf(separator);
    //    if( num == -1 ) {
    //        return input;
    //    }
    //    return input.Substring(0, num);
    //}


    //public static byte[] Decode(string input)
    //{
    //    if( string.IsNullOrWhiteSpace(input) ) {
    //        throw new ArgumentException(nameof(input));
    //    }
    //    string text = input;
    //    text = text.Replace('-', '+');
    //    text = text.Replace('_', '/');
    //    switch( text.Length % 4 ) {
    //        case 2:
    //            text += "==";
    //            break;
    //        case 3:
    //            text += "=";
    //            break;
    //        default:
    //            throw new FormatException("Illegal base64url string.");
    //        case 0:
    //            break;
    //    }

    //    return Convert.FromBase64String(text);
    //}


    public static byte[] Decode(string input)
    {
        if( string.IsNullOrEmpty(input) ) {
            throw new ArgumentNullException(nameof(input));
        }

        // 解密TOKEN的调用次数较多，所以优化下性能

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
                    throw new FormatException("Illegal base64url string.");
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
