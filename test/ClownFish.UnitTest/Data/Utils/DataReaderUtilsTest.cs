using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.Utils;

[TestClass]
public class DataReaderUtilsTest
{
    [TestMethod]
    public void Test_ToBool()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(true);
        Assert.AreEqual(true, DataReaderUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(false);
        Assert.AreEqual(false, DataReaderUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(1);
        Assert.AreEqual(true, DataReaderUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(11);
        Assert.AreEqual(true, DataReaderUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(0);
        Assert.AreEqual(false, DataReaderUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("1");
        Assert.AreEqual(true, DataReaderUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("11");
        Assert.AreEqual(true, DataReaderUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("0");
        Assert.AreEqual(false, DataReaderUtils.ToBool(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToBool(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToBool(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToBoolNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(true);
        Assert.AreEqual(true, DataReaderUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(false);
        Assert.AreEqual(false, DataReaderUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(1);
        Assert.AreEqual(true, DataReaderUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(11);
        Assert.AreEqual(true, DataReaderUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(0);
        Assert.AreEqual(false, DataReaderUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("1");
        Assert.AreEqual(true, DataReaderUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("11");
        Assert.AreEqual(true, DataReaderUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("0");
        Assert.AreEqual(false, DataReaderUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToBoolNull(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToByte()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue((byte)3);
        Assert.AreEqual((byte)3, DataReaderUtils.ToByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)33);
        Assert.AreEqual((byte)33, DataReaderUtils.ToByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((byte)3, DataReaderUtils.ToByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((byte)3, DataReaderUtils.ToByte(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToByte(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToByte(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToByteNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)3);
        Assert.AreEqual((byte)3, DataReaderUtils.ToByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)33);
        Assert.AreEqual((byte)33, DataReaderUtils.ToByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((byte)3, DataReaderUtils.ToByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((byte)3, DataReaderUtils.ToByteNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToByteNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToByteNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToSByte()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue((byte)3);
        Assert.AreEqual((sbyte)3, DataReaderUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)33);
        Assert.AreEqual((sbyte)33, DataReaderUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((sbyte)3, DataReaderUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((sbyte)3, DataReaderUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((sbyte)-3);
        Assert.AreEqual((sbyte)-3, DataReaderUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((sbyte)-33);
        Assert.AreEqual((sbyte)-33, DataReaderUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)-3);
        Assert.AreEqual((sbyte)-3, DataReaderUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)-3);
        Assert.AreEqual((sbyte)-3, DataReaderUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToSByte(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToSByte(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToSByteNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)3);
        Assert.AreEqual((sbyte)3, DataReaderUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)33);
        Assert.AreEqual((sbyte)33, DataReaderUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((sbyte)3, DataReaderUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((sbyte)3, DataReaderUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((sbyte)-3);
        Assert.AreEqual((sbyte)-3, DataReaderUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((sbyte)-33);
        Assert.AreEqual((sbyte)-33, DataReaderUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)-3);
        Assert.AreEqual((sbyte)-3, DataReaderUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)-3);
        Assert.AreEqual((sbyte)-3, DataReaderUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToSByteNull(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToByteArray()
    {
        XDbDataReader reader = new XDbDataReader();

        byte[] bb = Guid.NewGuid().ToByteArray();

        reader.SetValue(bb);
        Assert.IsTrue(bb.IsEqual(DataReaderUtils.ToByteArray(reader, 0, typeof(Customer), "xxx")));

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToByteArray(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToByteArray(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToByteArray(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToChar()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue("");
        Assert.AreEqual(' ', DataReaderUtils.ToChar(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(" ");
        Assert.AreEqual(' ', DataReaderUtils.ToChar(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(' ');
        Assert.AreEqual(' ', DataReaderUtils.ToChar(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue('b');
        Assert.AreEqual('b', DataReaderUtils.ToChar(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("abc");
        Assert.AreEqual('a', DataReaderUtils.ToChar(reader, 0, typeof(Customer), "xxx"));


        //MyAssert.IsError<InvalidCastException>(() => {
        //    reader.SetValue(DateTime.Now);
        //    var x = DataReaderUtils.ToChar(reader, 0, typeof(Customer), "xxx");
        //});

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToChar(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToCharNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("");
        Assert.AreEqual(' ', DataReaderUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(" ");
        Assert.AreEqual(' ', DataReaderUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(' ');
        Assert.AreEqual(' ', DataReaderUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue('b');
        Assert.AreEqual('b', DataReaderUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("abc");
        Assert.AreEqual('a', DataReaderUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));


        //MyAssert.IsError<InvalidCastException>(() => {
        //    reader.SetValue(DateTime.Now);
        //    var x = DataReaderUtils.ToCharNull(reader, 0, typeof(Customer), "xxx");
        //});

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToCharNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToShort()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue((ushort)3);
        Assert.AreEqual((short)3, DataReaderUtils.ToShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((short)-33);
        Assert.AreEqual((short)-33, DataReaderUtils.ToShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((short)3, DataReaderUtils.ToShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((short)3, DataReaderUtils.ToShort(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToShort(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToShort(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToShortNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((ushort)3);
        Assert.AreEqual((short)3, DataReaderUtils.ToShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((short)-33);
        Assert.AreEqual((short)-33, DataReaderUtils.ToShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((short)3, DataReaderUtils.ToShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((short)3, DataReaderUtils.ToShortNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToShortNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToShortNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToUShort()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue((short)3);
        Assert.AreEqual((ushort)3, DataReaderUtils.ToUShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((short)33);
        Assert.AreEqual((ushort)33, DataReaderUtils.ToUShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((ushort)3, DataReaderUtils.ToUShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((ushort)3, DataReaderUtils.ToUShort(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToUShort(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToUShort(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToUShortNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((short)3);
        Assert.AreEqual((ushort)3, DataReaderUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((short)33);
        Assert.AreEqual((ushort)33, DataReaderUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((ushort)3, DataReaderUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((ushort)3, DataReaderUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToUShortNull(reader, 99, typeof(Customer), "xxx");
        });
    }



    [TestMethod]
    public void Test_ToInt()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue((int)3);
        Assert.AreEqual((int)3, DataReaderUtils.ToInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)33);
        Assert.AreEqual((int)33, DataReaderUtils.ToInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((int)3, DataReaderUtils.ToInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((int)3, DataReaderUtils.ToInt(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToInt(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToInt(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToIntNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((int)3, DataReaderUtils.ToIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)33);
        Assert.AreEqual((int)33, DataReaderUtils.ToIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((int)3, DataReaderUtils.ToIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((int)3, DataReaderUtils.ToIntNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToIntNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToIntNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToUInt()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue((int)3);
        Assert.AreEqual((uint)3, DataReaderUtils.ToUInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((uint)33, DataReaderUtils.ToUInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((uint)3, DataReaderUtils.ToUInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((uint)3, DataReaderUtils.ToUInt(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToUInt(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToUInt(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToUIntNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((uint)3, DataReaderUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)33);
        Assert.AreEqual((uint)33, DataReaderUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((uint)3, DataReaderUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((uint)3, DataReaderUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToUIntNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToLong()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue((int)3);
        Assert.AreEqual((long)3, DataReaderUtils.ToLong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)33);
        Assert.AreEqual((long)33, DataReaderUtils.ToLong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((long)3, DataReaderUtils.ToLong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((long)3, DataReaderUtils.ToLong(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToLong(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToLong(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToLongNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToLongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((long)3, DataReaderUtils.ToLongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)33);
        Assert.AreEqual((long)33, DataReaderUtils.ToLongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((long)3, DataReaderUtils.ToLongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((long)3, DataReaderUtils.ToLongNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToLongNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToLongNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToULong()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue((uint)3);
        Assert.AreEqual((ulong)3, DataReaderUtils.ToULong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((ulong)33, DataReaderUtils.ToULong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((ulong)3, DataReaderUtils.ToULong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((ulong)3, DataReaderUtils.ToULong(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToULong(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToULong(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToULongNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToULongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((uint)3);
        Assert.AreEqual((ulong)3, DataReaderUtils.ToULongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)33);
        Assert.AreEqual((ulong)33, DataReaderUtils.ToULongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)3);
        Assert.AreEqual((ulong)3, DataReaderUtils.ToULongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((ulong)3, DataReaderUtils.ToULongNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToULongNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToULongNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToFloat()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue((int)3);
        Assert.AreEqual((float)3, DataReaderUtils.ToFloat(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)33);
        Assert.AreEqual((float)33, DataReaderUtils.ToFloat(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((float)3, DataReaderUtils.ToFloat(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((float)3, DataReaderUtils.ToFloat(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToFloat(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToFloat(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToFloatNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((float)3, DataReaderUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)33);
        Assert.AreEqual((float)33, DataReaderUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((float)3, DataReaderUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((float)3, DataReaderUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToFloatNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToDouble()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue((int)3);
        Assert.AreEqual((double)3, DataReaderUtils.ToDouble(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((double)33, DataReaderUtils.ToDouble(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((double)3, DataReaderUtils.ToDouble(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((double)3, DataReaderUtils.ToDouble(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToDouble(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToDouble(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToDoubleNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((double)3, DataReaderUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((double)33, DataReaderUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((double)3, DataReaderUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((double)3, DataReaderUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToDoubleNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToDecimal()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue((int)3);
        Assert.AreEqual((decimal)3, DataReaderUtils.ToDecimal(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((decimal)33, DataReaderUtils.ToDecimal(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((decimal)3, DataReaderUtils.ToDecimal(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((decimal)3, DataReaderUtils.ToDecimal(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToDecimal(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToDecimal(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToDecimalNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((decimal)3, DataReaderUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((decimal)33, DataReaderUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((decimal)3, DataReaderUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((decimal)3, DataReaderUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToDecimalNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToDateTime()
    {
        XDbDataReader reader = new XDbDataReader();

        DateTime t1 = new DateTime(2000, 1, 1);

        reader.SetValue(t1);
        Assert.AreEqual(t1, DataReaderUtils.ToDateTime(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(DateTime.MinValue);
        Assert.AreEqual(DateTime.MinValue, DataReaderUtils.ToDateTime(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(DateTime.MaxValue);
        Assert.AreEqual(DateTime.MaxValue, DataReaderUtils.ToDateTime(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(33);
            var x = DataReaderUtils.ToDateTime(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToDateTime(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToDateTimeNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToDateTimeNull(reader, 0, typeof(Customer), "xxx"));

        DateTime t1 = new DateTime(2000, 1, 1);

        reader.SetValue(t1);
        Assert.AreEqual(t1, DataReaderUtils.ToDateTimeNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(DateTime.MinValue);
        Assert.AreEqual(DateTime.MinValue, DataReaderUtils.ToDateTimeNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(DateTime.MaxValue);
        Assert.AreEqual(DateTime.MaxValue, DataReaderUtils.ToDateTimeNull(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(33);
            var x = DataReaderUtils.ToDateTimeNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToDateTimeNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToTimeSpan()
    {
        XDbDataReader reader = new XDbDataReader();

        TimeSpan t1 = new TimeSpan(1, 1, 1);

        reader.SetValue(t1.Ticks);
        Assert.AreEqual(t1, DataReaderUtils.ToTimeSpan(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(TimeSpan.MinValue.Ticks);
        Assert.AreEqual(TimeSpan.MinValue, DataReaderUtils.ToTimeSpan(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(TimeSpan.MaxValue.Ticks);
        Assert.AreEqual(TimeSpan.MaxValue, DataReaderUtils.ToTimeSpan(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(Guid.NewGuid());
            var x = DataReaderUtils.ToTimeSpan(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToTimeSpan(reader, 99, typeof(Customer), "xxx");
        });
    }



    [TestMethod]
    public void Test_ToTimeSpanNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToTimeSpanNull(reader, 0, typeof(Customer), "xxx"));

        TimeSpan t1 = new TimeSpan(1, 1, 1);

        reader.SetValue(t1.Ticks);
        Assert.AreEqual(t1, DataReaderUtils.ToTimeSpanNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(TimeSpan.MinValue.Ticks);
        Assert.AreEqual(TimeSpan.MinValue, DataReaderUtils.ToTimeSpanNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(TimeSpan.MaxValue.Ticks);
        Assert.AreEqual(TimeSpan.MaxValue, DataReaderUtils.ToTimeSpanNull(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(Guid.NewGuid());
            var x = DataReaderUtils.ToTimeSpanNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToTimeSpanNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToGuid()
    {
        XDbDataReader reader = new XDbDataReader();

        Guid g1 = Guid.NewGuid();

        reader.SetValue(g1);
        Assert.AreEqual(g1, DataReaderUtils.ToGuid(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(Guid.Empty);
        Assert.AreEqual(Guid.Empty, DataReaderUtils.ToGuid(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToGuid(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToGuid(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToGuidNull()
    {
        XDbDataReader reader = new XDbDataReader();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataReaderUtils.ToGuidNull(reader, 0, typeof(Customer), "xxx"));

        Guid g1 = Guid.NewGuid();

        reader.SetValue(g1);
        Assert.AreEqual(g1, DataReaderUtils.ToGuidNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(Guid.Empty);
        Assert.AreEqual(Guid.Empty, DataReaderUtils.ToGuidNull(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataReaderUtils.ToGuidNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataReaderUtils.ToGuidNull(reader, 99, typeof(Customer), "xxx");
        });
    }


}



public class XDbDataReader : DbDataReader
{
    private object _value;

    public void SetValue(object value)
    {
        _value = value;
    }

    public override object GetValue(int ordinal)
    {
        if( ordinal >= 99 )
            throw new ArgumentOutOfRangeException(nameof(ordinal));

        return _value;
    }





    public override object this[int ordinal] => throw new NotImplementedException();

    public override object this[string name] => throw new NotImplementedException();

    public override int Depth => throw new NotImplementedException();

    public override int FieldCount => throw new NotImplementedException();

    public override bool HasRows => throw new NotImplementedException();

    public override bool IsClosed => throw new NotImplementedException();

    public override int RecordsAffected => throw new NotImplementedException();

    public override bool GetBoolean(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override byte GetByte(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
    {
        throw new NotImplementedException();
    }

    public override char GetChar(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
    {
        throw new NotImplementedException();
    }

    public override string GetDataTypeName(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override DateTime GetDateTime(int ordinal)
    {
        return Convert.ToDateTime(_value);
    }

    public override decimal GetDecimal(int ordinal)
    {
        return Convert.ToDecimal(_value);
    }

    public override double GetDouble(int ordinal)
    {
        return Convert.ToDouble(_value);
    }

    public override IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public override Type GetFieldType(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override float GetFloat(int ordinal)
    {
        return Convert.ToSingle(_value);
    }

    public override Guid GetGuid(int ordinal)
    {
        if( ordinal >= 99 )
            throw new ArgumentOutOfRangeException(nameof(ordinal));

        return (Guid)_value;
    }

    public override short GetInt16(int ordinal)
    {
        if( ordinal >= 99 )
            throw new ArgumentOutOfRangeException(nameof(ordinal));

        return Convert.ToInt16(_value);
    }

    public override int GetInt32(int ordinal)
    {
        if( ordinal >= 99 )
            throw new ArgumentOutOfRangeException(nameof(ordinal));

        return Convert.ToInt32(_value);
    }

    public override long GetInt64(int ordinal)
    {
        if( ordinal >= 99 )
            throw new ArgumentOutOfRangeException(nameof(ordinal));

        return Convert.ToInt64(_value);
    }

    public override string GetName(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override int GetOrdinal(string name)
    {
        throw new NotImplementedException();
    }

    public override string GetString(int ordinal)
    {
        throw new NotImplementedException();
    }



    public override int GetValues(object[] values)
    {
        throw new NotImplementedException();
    }

    public override bool IsDBNull(int ordinal)
    {
        throw new NotImplementedException();
    }

    public override bool NextResult()
    {
        throw new NotImplementedException();
    }

    public override bool Read()
    {
        throw new NotImplementedException();
    }
}
