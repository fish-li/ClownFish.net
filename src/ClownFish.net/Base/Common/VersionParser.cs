namespace ClownFish.Base;

#if NETCOREAPP

/// <summary>
/// 解析版本号的工具类
/// </summary>
public static class VersionParser
{
    /// <summary>
    /// 采用“宽松”的方式解析版本号
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static Version Parse(string input)
    {
        if( string.IsNullOrEmpty(input) )
            return new Version(0, 0);

        ReadOnlySpan<char> span = input.AsSpan();

        // 解析主版本号
        if( false == TryParseVersionPart(span, out int major, out int nextIndex) )
            return new Version(0, 0);

        // 如果没有更多部分，返回主版本号+0
        if( nextIndex >= span.Length )
            return new Version(major, 0);

        // 解析次版本号
        if( false == TryParseVersionPart(span.Slice(nextIndex), out int minor, out int nextIndex2) )
            return new Version(major, 0);

        int currentIndex = nextIndex + nextIndex2;

        // 如果没有更多部分，返回主次版本号
        if( currentIndex >= span.Length )
            return new Version(major, minor);

        // 解析构建版本号
        if( false == TryParseVersionPart(span.Slice(currentIndex), out int build, out int nextIndex3) )
            return new Version(major, minor);

        currentIndex += nextIndex3;

        // 如果没有更多部分，返回主次构建版本号
        if( currentIndex >= span.Length )
            return new Version(major, minor, build);

        // 解析修订版本号
        if( false == TryParseVersionPart(span.Slice(currentIndex), out int revision, out _) )
            return new Version(major, minor, build);

        return new Version(major, minor, build, revision);
    }

    private static bool TryParseVersionPart(ReadOnlySpan<char> span, out int value, out int nextIndex)
    {
        value = 0;
        nextIndex = 0;

        // 跳过开头的分隔符（除了第一个部分）
        int start = 0;
        if( nextIndex == 0 && span.Length > 0 && span[0] == '.' ) {
            start = 1;
        }

        // 查找下一个分隔符或结尾
        int end = start;
        while( end < span.Length && span[end] != '.' ) {
            end++;
        }

        // 计算下一部分的起始位置
        nextIndex = end < span.Length ? end + 1 : end;

        // 提取数字部分
        ReadOnlySpan<char> numberSpan = span.Slice(start, end - start);

        // 如果为空，视为无效
        if( numberSpan.IsEmpty )
            return false;

        // 尝试解析数字
        return TryParseNonNegativeInteger(numberSpan, out value);
    }

    private static bool TryParseNonNegativeInteger(ReadOnlySpan<char> span, out int value)
    {
        value = 0;

        // 如果是负数，返回失败（但我们允许继续解析其他部分）
        if( span.Length > 0 && span[0] == '-' )
            return false;

        // 解析数字
        for( int i = 0; i < span.Length; i++ ) {
            char c = span[i];
            if( c < '0' || c > '9' )
                return false;

            value = value * 10 + (c - '0');

            // 防止溢出，但版本号通常不会太大
            if( value < 0 )
                return false;
        }

        return true;
    }
}


#else

/// <summary>
/// 解析版本号的工具类
/// </summary>
public static class VersionParser
{
    /// <summary>
    /// 采用“宽松”的方式解析版本号
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static Version Parse(string input)
    {
        if( string.IsNullOrEmpty(input) )
            return new Version(0, 0);

        // 使用 split 分割版本号
        string[] parts = input.Split('.');

        if( parts.Length == 0 )
            return new Version(0, 0);

        // 解析主版本号
        if( false == TryParseNonNegativeInteger(parts[0], out int major) )
            return new Version(0, 0);

        // 如果只有一个部分，返回主版本号+0
        if( parts.Length < 2 )
            return new Version(major, 0);

        // 解析次版本号
        if( false == TryParseNonNegativeInteger(parts[1], out int minor) )
            return new Version(major, 0);

        // 如果只有两个部分，返回主次版本号
        if( parts.Length < 3 )
            return new Version(major, minor);

        // 解析构建版本号
        if( false == TryParseNonNegativeInteger(parts[2], out int build) )
            return new Version(major, minor);

        // 如果只有三个部分，返回主次构建版本号
        if( parts.Length < 4 )
            return new Version(major, minor, build);

        // 解析修订版本号
        if( false == TryParseNonNegativeInteger(parts[3], out int revision) )
            return new Version(major, minor, build);

        return new Version(major, minor, build, revision);
    }

    private static bool TryParseNonNegativeInteger(string input, out int value)
    {
        value = 0;

        if( string.IsNullOrEmpty(input) )
            return false;

        // 如果是负数，返回失败
        if( input[0] == '-' )
            return false;

        // 解析数字
        foreach( char c in input ) {
            if( c < '0' || c > '9' )
                return false;

            value = value * 10 + (c - '0');

            // 防止溢出
            if( value < 0 )
                return false;
        }

        return true;
    }
}

#endif
