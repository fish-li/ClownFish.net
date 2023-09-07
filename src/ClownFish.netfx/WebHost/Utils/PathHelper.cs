namespace ClownFish.WebHost.Utils;

internal static class PathHelper
{
    internal static readonly char DirectorySeparatorChar = '\\';
    internal static readonly char AltDirectorySeparatorChar = '/';
    internal static readonly char VolumeSeparatorChar = ':';


    public static string GetExtension(string path)
    {
        // copy from Path.GetExtension

        if( path == null ) {
            return null;
        }
        

        //CheckInvalidPathChars(path, false);       // 反复运行这段代码真是浪费性能

        int length = path.Length;
        int num = length;
        while( --num >= 0 ) {
            char c = path[num];
            if( c == '.' ) {
                if( num != length - 1 ) {
                    return path.Substring(num, length - num);
                }
                return string.Empty;
            }
            if( c == DirectorySeparatorChar ) {
                break;
            }
            if( c == AltDirectorySeparatorChar ) {
                break;
            }
            if( c == VolumeSeparatorChar ) {
                break;
            }
        }
        return string.Empty;
    }
}
