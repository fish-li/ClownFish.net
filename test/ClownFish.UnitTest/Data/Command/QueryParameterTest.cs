namespace ClownFish.UnitTest.Data.Command;

[TestClass]
public class QueryParameterTest
{
    [TestMethod]
    public void Test_All()
    {
        QueryParameter p1 = (QueryParameter)"xxxx";
        Assert.AreEqual(typeof(string), p1.Value.GetType());
        Assert.AreEqual("xxxx", (string)p1.Value);

        QueryParameter p2 = DBNull.Value;
        Assert.AreEqual(typeof(DBNull), p2.Value.GetType());
        Assert.AreEqual(DBNull.Value, (DBNull)p2.Value);

        QueryParameter p3 = true;
        Assert.AreEqual(typeof(bool), p3.Value.GetType());
        Assert.AreEqual(true, (bool)p3.Value);

        QueryParameter p4 = (byte)5;
        Assert.AreEqual(typeof(byte), p4.Value.GetType());
        Assert.AreEqual((byte)5, (byte)p4.Value);

        QueryParameter p5 = (sbyte)6;
        Assert.AreEqual(typeof(sbyte), p5.Value.GetType());
        Assert.AreEqual((sbyte)6, (sbyte)p5.Value);

        byte[] bb = Guid.NewGuid().ToByteArray();
        QueryParameter p6 = bb;
        Assert.AreEqual(typeof(byte[]), p6.Value.GetType());
        Assert.IsTrue(bb.IsEqual((byte[])p6.Value));

        QueryParameter p7 = 'c';
        Assert.AreEqual(typeof(char), p7.Value.GetType());
        Assert.AreEqual('c', (char)p7.Value);

        QueryParameter p8 = (short)88;
        Assert.AreEqual(typeof(short), p8.Value.GetType());
        Assert.AreEqual((short)88, (short)p8.Value);

        QueryParameter p9 = (ushort)88;
        Assert.AreEqual(typeof(ushort), p9.Value.GetType());
        Assert.AreEqual((ushort)88, (ushort)p9.Value);

        QueryParameter p10 = (int)8;
        Assert.AreEqual(typeof(int), p10.Value.GetType());
        Assert.AreEqual((int)8, (int)p10.Value);

        QueryParameter p11 = (uint)88;
        Assert.AreEqual(typeof(uint), p11.Value.GetType());
        Assert.AreEqual((uint)88, (uint)p11.Value);


        QueryParameter p12 = (long)8;
        Assert.AreEqual(typeof(long), p12.Value.GetType());
        Assert.AreEqual((long)8, (long)p12.Value);

        QueryParameter p13 = (ulong)88;
        Assert.AreEqual(typeof(ulong), p13.Value.GetType());
        Assert.AreEqual((ulong)88, (ulong)p13.Value);


        QueryParameter p14 = (float)8;
        Assert.AreEqual(typeof(float), p14.Value.GetType());
        Assert.AreEqual((float)8, (float)p14.Value);

        QueryParameter p15 = (double)88;
        Assert.AreEqual(typeof(double), p15.Value.GetType());
        Assert.AreEqual((double)88, (double)p15.Value);

        QueryParameter p16 = (decimal)8;
        Assert.AreEqual(typeof(decimal), p16.Value.GetType());
        Assert.AreEqual((decimal)8, (decimal)p16.Value);


        QueryParameter p17 = new DateTime(2020, 2, 3);
        Assert.AreEqual(typeof(DateTime), p17.Value.GetType());
        Assert.AreEqual(new DateTime(2020, 2, 3), (DateTime)p17.Value);

        QueryParameter p18 = TimeSpan.FromSeconds(100);
        Assert.AreEqual(typeof(TimeSpan), p18.Value.GetType());
        Assert.AreEqual(TimeSpan.FromSeconds(100), (TimeSpan)p18.Value);

        Guid g = Guid.NewGuid();
        QueryParameter p19 = g;
        Assert.AreEqual(typeof(Guid), p19.Value.GetType());
        Assert.AreEqual(g, (Guid)p19.Value);

        int[] intArray = new int[] { 1, 2, 3 };
        QueryParameter p20 = intArray;
        Assert.AreEqual(typeof(int[]), p20.Value.GetType());
        MyAssert.AreEqual(intArray, (int[])p20.Value);

        List<int> intList = intArray.ToList();
        QueryParameter p21 = intList;
        Assert.AreEqual(typeof(List<int>), p21.Value.GetType());
        MyAssert.AreEqual(intList, (List<int>)p21.Value);

        long[] longArray = new long[] { 1, 2, 3 };
        QueryParameter p22 = longArray;
        Assert.AreEqual(typeof(long[]), p22.Value.GetType());
        MyAssert.AreEqual(longArray, (long[])p22.Value);

        List<long> longList = longArray.ToList();
        QueryParameter p23 = longList;
        Assert.AreEqual(typeof(List<long>), p23.Value.GetType());
        MyAssert.AreEqual(longList, (List<long>)p23.Value);


        Guid[] guidArray = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
        QueryParameter p24 = guidArray;
        Assert.AreEqual(typeof(Guid[]), p24.Value.GetType());
        MyAssert.AreEqual(guidArray, (Guid[])p24.Value);

        List<Guid> guidList = guidArray.ToList();
        QueryParameter p25 = guidList;
        Assert.AreEqual(typeof(List<Guid>), p25.Value.GetType());
        MyAssert.AreEqual(guidList, (List<Guid>)p25.Value);


        string[] stringArray = new string[] { "1", "2", "3" };
        QueryParameter p26 = stringArray;
        Assert.AreEqual(typeof(string[]), p26.Value.GetType());
        MyAssert.AreEqual(stringArray, (string[])p26.Value);

        List<string> stringList = stringArray.ToList();
        QueryParameter p27 = stringList;
        Assert.AreEqual(typeof(List<string>), p27.Value.GetType());
        MyAssert.AreEqual(stringList, (List<string>)p27.Value);
    }
}
