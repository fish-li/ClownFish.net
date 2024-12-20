using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Web.UnitTest.Test;

// 目前在 TxClient 中使用

internal static class TextMatchUtils
{
    static TextMatchUtils()
    {
        Regex.CacheSize = 150;
    }

    private static readonly char[] s_chars = "\\()[]{}<>+?*^$-,!|".ToCharArray();

    internal static bool IsRegex(string pattern)
    {
        if( pattern.IsNullOrEmpty() )
            return false;

        foreach( char c in pattern ) {
            if( s_chars.Contains(c) )
                return true;
        }
        return false;
    }

    public static bool IsMatch(string text, string pattern, bool ignoreCase = true)
    {
        return IsMatch0(text, pattern, ignoreCase) > 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int IsMatch0(string text, string pattern, bool ignoreCase = true)
    {
        if( text.IsNullOrEmpty() || pattern.IsNullOrEmpty() )
            return 0;

        if( IsRegex(pattern) ) {
            RegexOptions options = ignoreCase ? RegexOptions.IgnoreCase : RegexOptions.None;
            return Regex.IsMatch(text, pattern, options) ? 1 : -1;
        }
        else {
            if( ignoreCase ) {
                return text.Contains(pattern, StringComparison.OrdinalIgnoreCase) ? 2 : -2;
            }
            else {
                return text.Contains(pattern, StringComparison.Ordinal) ? 3 : -3;
            }
        }
    }
}


[TestClass]
public class TextMatchUtilsTest
{
    [TestMethod]
    public void Test_IsRegexPattern()
    {
        Assert.IsFalse(TextMatchUtils.IsRegex(""));
        Assert.IsFalse(TextMatchUtils.IsRegex("中华"));
        Assert.IsFalse(TextMatchUtils.IsRegex("cpu"));
        Assert.IsTrue(TextMatchUtils.IsRegex("cpu|mem"));
        Assert.IsTrue(TextMatchUtils.IsRegex("[abc]"));
    }


    [TestMethod]
    public void Test_IsMatch()
    {
        Assert.IsFalse(TextMatchUtils.IsMatch("", "xx"));
        Assert.IsFalse(TextMatchUtils.IsMatch("xx", ""));
               

        Assert.AreEqual(2, TextMatchUtils.IsMatch0("中华文明-5000年", "中华"));
        Assert.AreEqual(3, TextMatchUtils.IsMatch0("中华文明-5000年", "中华", false));

        Assert.AreEqual(2, TextMatchUtils.IsMatch0("CPU使用率超过80%", "cpu"));
        Assert.AreEqual(-3, TextMatchUtils.IsMatch0("CPU使用率超过80%", "cpu", false));

        Assert.AreEqual(1, TextMatchUtils.IsMatch0("CPU使用率超过80%", "cpu|mem"));
        Assert.AreEqual(-1, TextMatchUtils.IsMatch0("CPU使用率超过80%", "cpu|mem", false));

        Assert.IsTrue(TextMatchUtils.IsMatch("中华文明-5000年", "中华"));
        Assert.IsTrue(TextMatchUtils.IsMatch("中华文明-5000年", "中华", false));

        Assert.IsTrue(TextMatchUtils.IsMatch("CPU使用率超过80%", "cpu"));
        Assert.IsFalse(TextMatchUtils.IsMatch("CPU使用率超过80%", "cpu", false));

        Assert.IsTrue(TextMatchUtils.IsMatch("CPU使用率超过80%", "cpu|mem"));
        Assert.IsFalse(TextMatchUtils.IsMatch("CPU使用率超过80%", "cpu|mem", false));
    }
}
