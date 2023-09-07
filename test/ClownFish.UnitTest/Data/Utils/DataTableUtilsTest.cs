using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data.Utils;

internal static class XDataRowUtils
{
    public static DataRow CreateRow()
    {
        DataTable table = new DataTable();
        DataColumn col = new DataColumn("xname", typeof(object));
        table.Columns.Add(col);

        table.Rows.Add("xx");
        return table.Rows[0];
    }


    public static void SetValue(this DataRow row, object value)
    {
        row[0] = value;
    }
}


[TestClass]
public class DataTableUtilsTest
{
    [TestMethod]
    public void Test_ToString()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue("");
        Assert.AreEqual("", DataTableUtils.GetValue(reader, 0));
        Assert.AreEqual("", DataTableUtils.ToString(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("abc");
        Assert.AreEqual("abc", DataTableUtils.GetValue(reader, 0));
        Assert.AreEqual("abc", DataTableUtils.ToString(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(null);
        Assert.AreEqual(null, DataTableUtils.ToString(reader, 0, typeof(Customer), "xxx"));
        Assert.AreEqual(DBNull.Value, DataTableUtils.GetValue(reader, 0));

        reader.SetValue('a');
        Assert.AreEqual("a", DataTableUtils.ToString(reader, 0, typeof(Customer), "xxx"));
    }


    [TestMethod]
    public void Test_ToBool()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(true);
        Assert.AreEqual(true, DataTableUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(false);
        Assert.AreEqual(false, DataTableUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(1);
        Assert.AreEqual(true, DataTableUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(11);
        Assert.AreEqual(true, DataTableUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(0);
        Assert.AreEqual(false, DataTableUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("1");
        Assert.AreEqual(true, DataTableUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("11");
        Assert.AreEqual(true, DataTableUtils.ToBool(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("0");
        Assert.AreEqual(false, DataTableUtils.ToBool(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToBool(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToBool(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToBoolNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(null);
        Assert.IsNull(DataTableUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(true);
        Assert.AreEqual(true, DataTableUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(false);
        Assert.AreEqual(false, DataTableUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(1);
        Assert.AreEqual(true, DataTableUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(11);
        Assert.AreEqual(true, DataTableUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(0);
        Assert.AreEqual(false, DataTableUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("1");
        Assert.AreEqual(true, DataTableUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("11");
        Assert.AreEqual(true, DataTableUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("0");
        Assert.AreEqual(false, DataTableUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToBoolNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToBoolNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToByte()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue((byte)3);
        Assert.AreEqual((byte)3, DataTableUtils.ToByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)33);
        Assert.AreEqual((byte)33, DataTableUtils.ToByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((byte)3, DataTableUtils.ToByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((byte)3, DataTableUtils.ToByte(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToByte(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToByte(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToByteNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)3);
        Assert.AreEqual((byte)3, DataTableUtils.ToByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)33);
        Assert.AreEqual((byte)33, DataTableUtils.ToByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((byte)3, DataTableUtils.ToByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((byte)3, DataTableUtils.ToByteNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToByteNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToByteNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToSByte()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue((byte)3);
        Assert.AreEqual((sbyte)3, DataTableUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)33);
        Assert.AreEqual((sbyte)33, DataTableUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((sbyte)3, DataTableUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((sbyte)3, DataTableUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((sbyte)-3);
        Assert.AreEqual((sbyte)-3, DataTableUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((sbyte)-33);
        Assert.AreEqual((sbyte)-33, DataTableUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)-3);
        Assert.AreEqual((sbyte)-3, DataTableUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)-3);
        Assert.AreEqual((sbyte)-3, DataTableUtils.ToSByte(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToSByte(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToSByte(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToSByteNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)3);
        Assert.AreEqual((sbyte)3, DataTableUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((byte)33);
        Assert.AreEqual((sbyte)33, DataTableUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((sbyte)3, DataTableUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((sbyte)3, DataTableUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((sbyte)-3);
        Assert.AreEqual((sbyte)-3, DataTableUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((sbyte)-33);
        Assert.AreEqual((sbyte)-33, DataTableUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)-3);
        Assert.AreEqual((sbyte)-3, DataTableUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)-3);
        Assert.AreEqual((sbyte)-3, DataTableUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToSByteNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToSByteNull(reader, 99, typeof(Customer), "xxx");
        });
    }



    [TestMethod]
    public void Test_ToByteArray()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        byte[] bb = Guid.NewGuid().ToByteArray();

        reader.SetValue(bb);
        Assert.IsTrue(bb.IsEqual(DataTableUtils.ToByteArray(reader, 0, typeof(Customer), "xxx")));

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToByteArray(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToByteArray(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToByteArray(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToChar()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue("");
        Assert.AreEqual(' ', DataTableUtils.ToChar(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(" ");
        Assert.AreEqual(' ', DataTableUtils.ToChar(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(' ');
        Assert.AreEqual(' ', DataTableUtils.ToChar(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue('b');
        Assert.AreEqual('b', DataTableUtils.ToChar(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("abc");
        Assert.AreEqual('a', DataTableUtils.ToChar(reader, 0, typeof(Customer), "xxx"));

        //MyAssert.IsError<InvalidCastException>(() => {
        //    reader.SetValue(DateTime.Now);
        //    var x = DataTableUtils.ToChar(reader, 0, typeof(Customer), "xxx");
        //});

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToChar(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToCharNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("");
        Assert.AreEqual(' ', DataTableUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(" ");
        Assert.AreEqual(' ', DataTableUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(' ');
        Assert.AreEqual(' ', DataTableUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue('b');
        Assert.AreEqual('b', DataTableUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue("abc");
        Assert.AreEqual('a', DataTableUtils.ToCharNull(reader, 0, typeof(Customer), "xxx"));

        //MyAssert.IsError<InvalidCastException>(() => {
        //    reader.SetValue(DateTime.Now);
        //    var x = DataTableUtils.ToChar(reader, 0, typeof(Customer), "xxx");
        //});

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToCharNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToShort()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue((ushort)3);
        Assert.AreEqual((short)3, DataTableUtils.ToShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((short)-33);
        Assert.AreEqual((short)-33, DataTableUtils.ToShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((short)3, DataTableUtils.ToShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((short)3, DataTableUtils.ToShort(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToShort(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToShort(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToShortNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((ushort)3);
        Assert.AreEqual((short)3, DataTableUtils.ToShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((short)-33);
        Assert.AreEqual((short)-33, DataTableUtils.ToShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((short)3, DataTableUtils.ToShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((short)3, DataTableUtils.ToShortNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToShortNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToShortNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToUShort()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue((short)3);
        Assert.AreEqual((ushort)3, DataTableUtils.ToUShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((short)33);
        Assert.AreEqual((ushort)33, DataTableUtils.ToUShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((ushort)3, DataTableUtils.ToUShort(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((ushort)3, DataTableUtils.ToUShort(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToUShort(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToUShort(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToUShortNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((short)3);
        Assert.AreEqual((ushort)3, DataTableUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((short)33);
        Assert.AreEqual((ushort)33, DataTableUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((ushort)3, DataTableUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((ushort)3, DataTableUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToUShortNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToUShortNull(reader, 99, typeof(Customer), "xxx");
        });
    }



    [TestMethod]
    public void Test_ToInt()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue((int)3);
        Assert.AreEqual((int)3, DataTableUtils.ToInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)33);
        Assert.AreEqual((int)33, DataTableUtils.ToInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((int)3, DataTableUtils.ToInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((int)3, DataTableUtils.ToInt(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToInt(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToInt(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToIntNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((int)3, DataTableUtils.ToIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)33);
        Assert.AreEqual((int)33, DataTableUtils.ToIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((int)3, DataTableUtils.ToIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((int)3, DataTableUtils.ToIntNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToIntNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToIntNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToUInt()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue((int)3);
        Assert.AreEqual((uint)3, DataTableUtils.ToUInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((uint)33, DataTableUtils.ToUInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((uint)3, DataTableUtils.ToUInt(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((uint)3, DataTableUtils.ToUInt(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToUInt(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToUInt(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToUIntNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((uint)3, DataTableUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)33);
        Assert.AreEqual((uint)33, DataTableUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((uint)3, DataTableUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((uint)3, DataTableUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToUIntNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToUIntNull(reader, 99, typeof(Customer), "xxx");
        });
    }



    [TestMethod]
    public void Test_ToLong()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue((int)3);
        Assert.AreEqual((long)3, DataTableUtils.ToLong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)33);
        Assert.AreEqual((long)33, DataTableUtils.ToLong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((long)3, DataTableUtils.ToLong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((long)3, DataTableUtils.ToLong(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToLong(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToLong(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToLongNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToLongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((long)3, DataTableUtils.ToLongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)33);
        Assert.AreEqual((long)33, DataTableUtils.ToLongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((long)3, DataTableUtils.ToLongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((long)3, DataTableUtils.ToLongNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToLongNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToLongNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToULong()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue((uint)3);
        Assert.AreEqual((ulong)3, DataTableUtils.ToULong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((ulong)33, DataTableUtils.ToULong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((ulong)3, DataTableUtils.ToULong(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((ulong)3, DataTableUtils.ToULong(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToULong(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToULong(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToULongNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToULongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((uint)3);
        Assert.AreEqual((ulong)3, DataTableUtils.ToULongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)33);
        Assert.AreEqual((ulong)33, DataTableUtils.ToULongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)3);
        Assert.AreEqual((ulong)3, DataTableUtils.ToULongNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((ulong)3, DataTableUtils.ToULongNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToULongNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToULongNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToFloat()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue((int)3);
        Assert.AreEqual((float)3, DataTableUtils.ToFloat(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)33);
        Assert.AreEqual((float)33, DataTableUtils.ToFloat(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((float)3, DataTableUtils.ToFloat(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((float)3, DataTableUtils.ToFloat(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToFloat(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToFloat(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToFloatNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((float)3, DataTableUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)33);
        Assert.AreEqual((float)33, DataTableUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((float)3, DataTableUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((float)3, DataTableUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToFloatNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToFloatNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToDouble()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue((int)3);
        Assert.AreEqual((double)3, DataTableUtils.ToDouble(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((double)33, DataTableUtils.ToDouble(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((double)3, DataTableUtils.ToDouble(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((double)3, DataTableUtils.ToDouble(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToDouble(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToDouble(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToDoubleNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((double)3, DataTableUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((double)33, DataTableUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((decimal)3);
        Assert.AreEqual((double)3, DataTableUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((double)3, DataTableUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToDoubleNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToDoubleNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToDecimal()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue((int)3);
        Assert.AreEqual((decimal)3, DataTableUtils.ToDecimal(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((decimal)33, DataTableUtils.ToDecimal(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((decimal)3, DataTableUtils.ToDecimal(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((decimal)3, DataTableUtils.ToDecimal(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToDecimal(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToDecimal(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToDecimalNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((int)3);
        Assert.AreEqual((decimal)3, DataTableUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((float)33);
        Assert.AreEqual((decimal)33, DataTableUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((double)3);
        Assert.AreEqual((decimal)3, DataTableUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue((long)3);
        Assert.AreEqual((decimal)3, DataTableUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx"));


        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToDecimalNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToDecimalNull(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToDateTime()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        DateTime t1 = new DateTime(2000, 1, 1);

        reader.SetValue(t1);
        Assert.AreEqual(t1, DataTableUtils.ToDateTime(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(DateTime.MinValue);
        Assert.AreEqual(DateTime.MinValue, DataTableUtils.ToDateTime(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(DateTime.MaxValue);
        Assert.AreEqual(DateTime.MaxValue, DataTableUtils.ToDateTime(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(33);
            var x = DataTableUtils.ToDateTime(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToDateTime(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToDateTimeNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToDateTimeNull(reader, 0, typeof(Customer), "xxx"));

        DateTime t1 = new DateTime(2000, 1, 1);

        reader.SetValue(t1);
        Assert.AreEqual(t1, DataTableUtils.ToDateTimeNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(DateTime.MinValue);
        Assert.AreEqual(DateTime.MinValue, DataTableUtils.ToDateTimeNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(DateTime.MaxValue);
        Assert.AreEqual(DateTime.MaxValue, DataTableUtils.ToDateTimeNull(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(33);
            var x = DataTableUtils.ToDateTimeNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToDateTimeNull(reader, 99, typeof(Customer), "xxx");
        });
    }



    [TestMethod]
    public void Test_ToTimeSpan()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        TimeSpan t1 = new TimeSpan(1, 1, 1);

        reader.SetValue(t1.Ticks);
        Assert.AreEqual(t1, DataTableUtils.ToTimeSpan(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(TimeSpan.MinValue.Ticks);
        Assert.AreEqual(TimeSpan.MinValue, DataTableUtils.ToTimeSpan(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(TimeSpan.MaxValue.Ticks);
        Assert.AreEqual(TimeSpan.MaxValue, DataTableUtils.ToTimeSpan(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(Guid.NewGuid());
            var x = DataTableUtils.ToTimeSpan(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToTimeSpan(reader, 99, typeof(Customer), "xxx");
        });
    }



    [TestMethod]
    public void Test_ToTimeSpanNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToTimeSpanNull(reader, 0, typeof(Customer), "xxx"));

        TimeSpan t1 = new TimeSpan(1, 1, 1);

        reader.SetValue(t1.Ticks);
        Assert.AreEqual(t1, DataTableUtils.ToTimeSpanNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(TimeSpan.MinValue.Ticks);
        Assert.AreEqual(TimeSpan.MinValue, DataTableUtils.ToTimeSpanNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(TimeSpan.MaxValue.Ticks);
        Assert.AreEqual(TimeSpan.MaxValue, DataTableUtils.ToTimeSpanNull(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(Guid.NewGuid());
            var x = DataTableUtils.ToTimeSpanNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToTimeSpanNull(reader, 99, typeof(Customer), "xxx");
        });
    }


    [TestMethod]
    public void Test_ToGuid()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        Guid g1 = Guid.NewGuid();

        reader.SetValue(g1);
        Assert.AreEqual(g1, DataTableUtils.ToGuid(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(Guid.Empty);
        Assert.AreEqual(Guid.Empty, DataTableUtils.ToGuid(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToGuid(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToGuid(reader, 99, typeof(Customer), "xxx");
        });
    }

    [TestMethod]
    public void Test_ToGuidNull()
    {
        DataRow reader = XDataRowUtils.CreateRow();

        reader.SetValue(DBNull.Value);
        Assert.IsNull(DataTableUtils.ToGuidNull(reader, 0, typeof(Customer), "xxx"));

        Guid g1 = Guid.NewGuid();

        reader.SetValue(g1);
        Assert.AreEqual(g1, DataTableUtils.ToGuidNull(reader, 0, typeof(Customer), "xxx"));

        reader.SetValue(Guid.Empty);
        Assert.AreEqual(Guid.Empty, DataTableUtils.ToGuidNull(reader, 0, typeof(Customer), "xxx"));

        MyAssert.IsError<InvalidCastException>(() => {
            reader.SetValue(DateTime.Now);
            var x = DataTableUtils.ToGuidNull(reader, 0, typeof(Customer), "xxx");
        });

        MyAssert.IsError<InvalidOperationException>(() => {
            reader.SetValue("xxxxxxxxxx");
            var x = DataTableUtils.ToGuidNull(reader, 99, typeof(Customer), "xxx");
        });
    }











}
