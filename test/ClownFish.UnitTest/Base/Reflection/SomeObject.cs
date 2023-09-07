namespace ClownFish.UnitTest.Base.Reflection;

public class DataObject
{
    public virtual int InputA { get; set; }

    public int InputB { get; set; }

    public int Result { get; set; }

    public long Value1;

    public static long Value2;
}

public static class StaticObject
{
    public static int Result { get; set; }

    public static int XWrite {
        set => Result = value;
    }
    public static int XRead {
        get => Result;
    }

    public static int Add(int a, int b)
    {
        return a + b;
    }

    public static int Fun0()
    {
        return 1252574;
    }

    public static int Fun1(int a)
    {
        return a + 10;
    }

    public static int Fun2(int a, int b)
    {
        return a + b + 10;
    }

    public static int Fun3(int a, int b, int c)
    {
        return a + b + c + 10;
    }

    public static int Fun4(int a, int b, int c, int d)
    {
        return a + b + c + d + 10;
    }

    public static int Fun5(int a, int b, int c, int d, int e)
    {
        return a + b + c + d + e + 10;
    }

    public static int Fun6(int a, int b, int c, int d, int e, int f)
    {
        return a + b + c + d + e + f + 10;
    }

    public static int Fun7(int a, int b, int c, int d, int e, int f, int g)
    {
        return a + b + c + d + e + f + g + 10;
    }

    public static int Fun8(int a, int b, int c, int d, int e, int f, int g, int h)
    {
        return a + b + c + d + e + f + g + h + 10;
    }

    public static int Fun9(int a, int b, int c, int d, int e, int f, int g, int h, int k)
    {
        return a + b + c + d + e + f + g + h + k + 10;
    }

    public static int Fun10(int a, int b, int c, int d, int e, int f, int g, int h, int k, int m)
    {
        return a + b + c + d + e + f + g + h + k + m + 10;
    }


    public static void Action0()
    {
        Result = 1252574;
    }
    public static void Action1(int a)
    {
        Result = a + 10;
    }
    public static void Action2(int a, int b)
    {
        Result = a + b + 10;
    }
    public static void Action3(int a, int b, int c)
    {
        Result = a + b + c + 10;
    }
    public static void Action4(int a, int b, int c, int d)
    {
        Result = a + b + c + d + 10;
    }
    public static void Action5(int a, int b, int c, int d, int e)
    {
        Result = a + b + c + d + e + 10;
    }
    public static void Action6(int a, int b, int c, int d, int e, int f)
    {
        Result = a + b + c + d + e + f + 10;
    }
    public static void Action7(int a, int b, int c, int d, int e, int f, int g)
    {
        Result = a + b + c + d + e + f + g + 10;
    }
    public static void Action8(int a, int b, int c, int d, int e, int f, int g, int h)
    {
        Result = a + b + c + d + e + f + g + h + 10;
    }

    public static void Action9(int a, int b, int c, int d, int e, int f, int g, int h, int k)
    {
        Result = a + b + c + d + e + f + g + h + k + 10;
    }

    public static void Action10(int a, int b, int c, int d, int e, int f, int g, int h, int k, int m)
    {
        Result = a + b + c + d + e + f + g + h + k + m + 10;
    }
}


public class InstanceObject
{
    public int Result { get; set; }

    public int InputA { get; set; }

    public int XWrite {
        set => this.Result = value;
    }
    public int XRead {
        get => this.Result;
    }

    public int this[int x] {
        get => this.Result;
        set => this.Result = value;
    }

    public InstanceObject(int aa)
    {
        this.InputA = aa;
    }

    public int Fun0()
    {
        return 1252574;
    }

    public int Fun1(int a)
    {
        return a + 10;
    }

    public int Fun2(int a, int b)
    {
        return a + b + 10;
    }

    public int Fun3(int a, int b, int c)
    {
        return a + b + c + 10;
    }

    public int Fun4(int a, int b, int c, int d)
    {
        return a + b + c + d + 10;
    }

    public int Fun5(int a, int b, int c, int d, int e)
    {
        return a + b + c + d + e + 10;
    }

    public int Fun6(int a, int b, int c, int d, int e, int f)
    {
        return a + b + c + d + e + f + 10;
    }

    public int Fun7(int a, int b, int c, int d, int e, int f, int g)
    {
        return a + b + c + d + e + f + g + 10;
    }

    public int Fun8(int a, int b, int c, int d, int e, int f, int g, int h)
    {
        return a + b + c + d + e + f + g + h + 10;
    }

    public int Fun9(int a, int b, int c, int d, int e, int f, int g, int h, int k)
    {
        return a + b + c + d + e + f + g + h + k + 10;
    }

    public int Fun10(int a, int b, int c, int d, int e, int f, int g, int h, int k, int m)
    {
        return a + b + c + d + e + f + g + h + k + m + 10;
    }

    public void Action0()
    {
        Result = 1252574;
    }
    public void Action1(int a)
    {
        Result = a + 10;
    }
    public void Action2(int a, int b)
    {
        Result = a + b + 10;
    }
    public void Action3(int a, int b, int c)
    {
        Result = a + b + c + 10;
    }
    public void Action4(int a, int b, int c, int d)
    {
        Result = a + b + c + d + 10;
    }
    public void Action5(int a, int b, int c, int d, int e)
    {
        Result = a + b + c + d + e + 10;
    }
    public void Action6(int a, int b, int c, int d, int e, int f)
    {
        Result = a + b + c + d + e + f + 10;
    }
    public void Action7(int a, int b, int c, int d, int e, int f, int g)
    {
        Result = a + b + c + d + e + f + g + 10;
    }
    public void Action8(int a, int b, int c, int d, int e, int f, int g, int h)
    {
        Result = a + b + c + d + e + f + g + h + 10;
    }

    public void Action9(int a, int b, int c, int d, int e, int f, int g, int h, int k)
    {
        Result = a + b + c + d + e + f + g + h + k + 10;
    }

    public void Action10(int a, int b, int c, int d, int e, int f, int g, int h, int k, int m)
    {
        Result = a + b + c + d + e + f + g + h + k + m + 10;
    }
}


public class XxxObject
{
    public int Result { get; set; }

    public XxxObject(int x)
    {
        this.Result = x;
    }

    public void M1(int a, ref int b)
    {
        b = a;
    }
}
