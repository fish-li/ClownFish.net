using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Common
{
    [TestClass]
    public class FormatUtilsTest
    {
        [TestMethod]
        public void TestToWString()
        {
            Assert.AreEqual("1", FormatUtils.ToWString(1));
            Assert.AreEqual("10", FormatUtils.ToWString(10));
            Assert.AreEqual("100", FormatUtils.ToWString(100));
            Assert.AreEqual("1000", FormatUtils.ToWString(1000));
            Assert.AreEqual("1'0000", FormatUtils.ToWString(10000));
            Assert.AreEqual("10'0000", FormatUtils.ToWString(100000));
            Assert.AreEqual("100'0000", FormatUtils.ToWString(1000000));
            Assert.AreEqual("1000'0000", FormatUtils.ToWString(10000000));
            Assert.AreEqual("1'0000'0000", FormatUtils.ToWString(100000000));
            Assert.AreEqual("10'0000'0000", FormatUtils.ToWString(1000000000));

            Assert.AreEqual("999", FormatUtils.ToWString(999));
            Assert.AreEqual("9999", FormatUtils.ToWString(9999));
            Assert.AreEqual("9'9999", FormatUtils.ToWString(99999));
            Assert.AreEqual("99'9999", FormatUtils.ToWString(999999));
            Assert.AreEqual("999'9999", FormatUtils.ToWString(9999999));
            Assert.AreEqual("9999'9999", FormatUtils.ToWString(99999999));
            Assert.AreEqual("9'9999'9999", FormatUtils.ToWString(999999999));

            Assert.AreEqual("6", FormatUtils.ToWString(6));
            Assert.AreEqual("56", FormatUtils.ToWString(56));
            Assert.AreEqual("456", FormatUtils.ToWString(456));
            Assert.AreEqual("3456", FormatUtils.ToWString(3456));
            Assert.AreEqual("12'3456", FormatUtils.ToWString(123456));
            Assert.AreEqual("120'3456", FormatUtils.ToWString(1203456));
            Assert.AreEqual("1200'3456", FormatUtils.ToWString(12003456));
            Assert.AreEqual("2'1200'3456", FormatUtils.ToWString(212003456));
            Assert.AreEqual("32'1200'3456", FormatUtils.ToWString(3212003456));
            Assert.AreEqual("432'1200'3456", FormatUtils.ToWString(43212003456));
            Assert.AreEqual("5432'1200'3456", FormatUtils.ToWString(543212003456));
            Assert.AreEqual("6'5432'1200'3456", FormatUtils.ToWString(6543212003456));
            Assert.AreEqual("76'5432'1200'3456", FormatUtils.ToWString(76543212003456));
            Assert.AreEqual("876'5432'1200'3456", FormatUtils.ToWString(876543212003456));
            Assert.AreEqual("9876'5432'1200'3456", FormatUtils.ToWString(9876543212003456));
            Assert.AreEqual("1'9876'5432'1200'3456", FormatUtils.ToWString(19876543212003456));
            Assert.AreEqual("21'9876'5432'1200'3456", FormatUtils.ToWString(219876543212003456));


            Assert.AreEqual("-1", FormatUtils.ToWString(-1));
            Assert.AreEqual("-10", FormatUtils.ToWString(-10));
            Assert.AreEqual("-100", FormatUtils.ToWString(-100));
            Assert.AreEqual("-1000", FormatUtils.ToWString(-1000));
            Assert.AreEqual("-1'0000", FormatUtils.ToWString(-10000));
            Assert.AreEqual("-10'0000", FormatUtils.ToWString(-100000));
            Assert.AreEqual("-100'0000", FormatUtils.ToWString(-1000000));
            Assert.AreEqual("-1000'0000", FormatUtils.ToWString(-10000000));
            Assert.AreEqual("-1'0000'0000", FormatUtils.ToWString(-100000000));
            Assert.AreEqual("-10'0000'0000", FormatUtils.ToWString(-1000000000));

            Assert.AreEqual("-999", FormatUtils.ToWString(-999));
            Assert.AreEqual("-9999", FormatUtils.ToWString(-9999));
            Assert.AreEqual("-9'9999", FormatUtils.ToWString(-99999));
            Assert.AreEqual("-99'9999", FormatUtils.ToWString(-999999));
            Assert.AreEqual("-999'9999", FormatUtils.ToWString(-9999999));
            Assert.AreEqual("-9999'9999", FormatUtils.ToWString(-99999999));
            Assert.AreEqual("-9'9999'9999", FormatUtils.ToWString(-999999999));


            Assert.AreEqual("-6", FormatUtils.ToWString(-6));
            Assert.AreEqual("-56", FormatUtils.ToWString(-56));
            Assert.AreEqual("-456", FormatUtils.ToWString(-456));
            Assert.AreEqual("-3456", FormatUtils.ToWString(-3456));
            Assert.AreEqual("-12'3456", FormatUtils.ToWString(-123456));
            Assert.AreEqual("-120'3456", FormatUtils.ToWString(-1203456));
            Assert.AreEqual("-1200'3456", FormatUtils.ToWString(-12003456));
            Assert.AreEqual("-2'1200'3456", FormatUtils.ToWString(-212003456));
            Assert.AreEqual("-32'1200'3456", FormatUtils.ToWString(-3212003456));
            Assert.AreEqual("-432'1200'3456", FormatUtils.ToWString(-43212003456));
            Assert.AreEqual("-5432'1200'3456", FormatUtils.ToWString(-543212003456));
            Assert.AreEqual("-6'5432'1200'3456", FormatUtils.ToWString(-6543212003456));
            Assert.AreEqual("-76'5432'1200'3456", FormatUtils.ToWString(-76543212003456));
            Assert.AreEqual("-876'5432'1200'3456", FormatUtils.ToWString(-876543212003456));
            Assert.AreEqual("-9876'5432'1200'3456", FormatUtils.ToWString(-9876543212003456));
            Assert.AreEqual("-1'9876'5432'1200'3456", FormatUtils.ToWString(-19876543212003456));
            Assert.AreEqual("-21'9876'5432'1200'3456", FormatUtils.ToWString(-219876543212003456));
        }

        [TestMethod]
        public void TestToWString2()
        {
            char separator = '_';

            Assert.AreEqual("1", FormatUtils.ToWString(1, separator));
            Assert.AreEqual("10", FormatUtils.ToWString(10, separator));
            Assert.AreEqual("100", FormatUtils.ToWString(100, separator));
            Assert.AreEqual("1000", FormatUtils.ToWString(1000, separator));
            Assert.AreEqual("1_0000", FormatUtils.ToWString(10000, separator));
            Assert.AreEqual("10_0000", FormatUtils.ToWString(100000, separator));
            Assert.AreEqual("100_0000", FormatUtils.ToWString(1000000, separator));
            Assert.AreEqual("1000_0000", FormatUtils.ToWString(10000000, separator));
            Assert.AreEqual("1_0000_0000", FormatUtils.ToWString(100000000, separator));
            Assert.AreEqual("10_0000_0000", FormatUtils.ToWString(1000000000, separator));

            Assert.AreEqual("999", FormatUtils.ToWString(999, separator));
            Assert.AreEqual("9999", FormatUtils.ToWString(9999, separator));
            Assert.AreEqual("9_9999", FormatUtils.ToWString(99999, separator));
            Assert.AreEqual("99_9999", FormatUtils.ToWString(999999, separator));
            Assert.AreEqual("999_9999", FormatUtils.ToWString(9999999, separator));
            Assert.AreEqual("9999_9999", FormatUtils.ToWString(99999999, separator));
            Assert.AreEqual("9_9999_9999", FormatUtils.ToWString(999999999, separator));

            Assert.AreEqual("6", FormatUtils.ToWString(6, separator));
            Assert.AreEqual("56", FormatUtils.ToWString(56, separator));
            Assert.AreEqual("456", FormatUtils.ToWString(456, separator));
            Assert.AreEqual("3456", FormatUtils.ToWString(3456, separator));
            Assert.AreEqual("12_3456", FormatUtils.ToWString(123456, separator));
            Assert.AreEqual("120_3456", FormatUtils.ToWString(1203456, separator));
            Assert.AreEqual("1200_3456", FormatUtils.ToWString(12003456, separator));
            Assert.AreEqual("2_1200_3456", FormatUtils.ToWString(212003456, separator));
            Assert.AreEqual("32_1200_3456", FormatUtils.ToWString(3212003456, separator));
            Assert.AreEqual("432_1200_3456", FormatUtils.ToWString(43212003456, separator));
            Assert.AreEqual("5432_1200_3456", FormatUtils.ToWString(543212003456, separator));
            Assert.AreEqual("6_5432_1200_3456", FormatUtils.ToWString(6543212003456, separator));
            Assert.AreEqual("76_5432_1200_3456", FormatUtils.ToWString(76543212003456, separator));
            Assert.AreEqual("876_5432_1200_3456", FormatUtils.ToWString(876543212003456, separator));
            Assert.AreEqual("9876_5432_1200_3456", FormatUtils.ToWString(9876543212003456, separator));
            Assert.AreEqual("1_9876_5432_1200_3456", FormatUtils.ToWString(19876543212003456, separator));
            Assert.AreEqual("21_9876_5432_1200_3456", FormatUtils.ToWString(219876543212003456, separator));


            Assert.AreEqual("-1", FormatUtils.ToWString(-1, separator));
            Assert.AreEqual("-10", FormatUtils.ToWString(-10, separator));
            Assert.AreEqual("-100", FormatUtils.ToWString(-100, separator));
            Assert.AreEqual("-1000", FormatUtils.ToWString(-1000, separator));
            Assert.AreEqual("-1_0000", FormatUtils.ToWString(-10000, separator));
            Assert.AreEqual("-10_0000", FormatUtils.ToWString(-100000, separator));
            Assert.AreEqual("-100_0000", FormatUtils.ToWString(-1000000, separator));
            Assert.AreEqual("-1000_0000", FormatUtils.ToWString(-10000000, separator));
            Assert.AreEqual("-1_0000_0000", FormatUtils.ToWString(-100000000, separator));
            Assert.AreEqual("-10_0000_0000", FormatUtils.ToWString(-1000000000, separator));

            Assert.AreEqual("-999", FormatUtils.ToWString(-999, separator));
            Assert.AreEqual("-9999", FormatUtils.ToWString(-9999, separator));
            Assert.AreEqual("-9_9999", FormatUtils.ToWString(-99999, separator));
            Assert.AreEqual("-99_9999", FormatUtils.ToWString(-999999, separator));
            Assert.AreEqual("-999_9999", FormatUtils.ToWString(-9999999, separator));
            Assert.AreEqual("-9999_9999", FormatUtils.ToWString(-99999999, separator));
            Assert.AreEqual("-9_9999_9999", FormatUtils.ToWString(-999999999, separator));


            Assert.AreEqual("-6", FormatUtils.ToWString(-6, separator));
            Assert.AreEqual("-56", FormatUtils.ToWString(-56, separator));
            Assert.AreEqual("-456", FormatUtils.ToWString(-456, separator));
            Assert.AreEqual("-3456", FormatUtils.ToWString(-3456, separator));
            Assert.AreEqual("-12_3456", FormatUtils.ToWString(-123456, separator));
            Assert.AreEqual("-120_3456", FormatUtils.ToWString(-1203456, separator));
            Assert.AreEqual("-1200_3456", FormatUtils.ToWString(-12003456, separator));
            Assert.AreEqual("-2_1200_3456", FormatUtils.ToWString(-212003456, separator));
            Assert.AreEqual("-32_1200_3456", FormatUtils.ToWString(-3212003456, separator));
            Assert.AreEqual("-432_1200_3456", FormatUtils.ToWString(-43212003456, separator));
            Assert.AreEqual("-5432_1200_3456", FormatUtils.ToWString(-543212003456, separator));
            Assert.AreEqual("-6_5432_1200_3456", FormatUtils.ToWString(-6543212003456, separator));
            Assert.AreEqual("-76_5432_1200_3456", FormatUtils.ToWString(-76543212003456, separator));
            Assert.AreEqual("-876_5432_1200_3456", FormatUtils.ToWString(-876543212003456, separator));
            Assert.AreEqual("-9876_5432_1200_3456", FormatUtils.ToWString(-9876543212003456, separator));
            Assert.AreEqual("-1_9876_5432_1200_3456", FormatUtils.ToWString(-19876543212003456, separator));
            Assert.AreEqual("-21_9876_5432_1200_3456", FormatUtils.ToWString(-219876543212003456, separator));
        }


        [TestMethod]
        public void TestToWString3()
        {
            Assert.AreEqual("1.3330", FormatUtils.ToWString(1.333m));
            Assert.AreEqual("10.3330", FormatUtils.ToWString(10.333m));
            Assert.AreEqual("100.3330", FormatUtils.ToWString(100.333m));
            Assert.AreEqual("1000.3330", FormatUtils.ToWString(1000.333m));
            Assert.AreEqual("1'0000.3330", FormatUtils.ToWString(10000.333m));
            Assert.AreEqual("10'0000.3330", FormatUtils.ToWString(100000.333m));
            Assert.AreEqual("100'0000.3330", FormatUtils.ToWString(1000000.333m));
            Assert.AreEqual("1000'0000.3330", FormatUtils.ToWString(10000000.333m));
            Assert.AreEqual("1'0000'0000.3330", FormatUtils.ToWString(100000000.333m));
            Assert.AreEqual("10'0000'0000.3330", FormatUtils.ToWString(1000000000.333m));

            Assert.AreEqual("999.3330", FormatUtils.ToWString(999.333m));
            Assert.AreEqual("9999.3330", FormatUtils.ToWString(9999.333m));
            Assert.AreEqual("9'9999.3330", FormatUtils.ToWString(99999.333m));
            Assert.AreEqual("99'9999.3330", FormatUtils.ToWString(999999.333m));
            Assert.AreEqual("999'9999.3330", FormatUtils.ToWString(9999999.333m));
            Assert.AreEqual("9999'9999.3330", FormatUtils.ToWString(99999999.333m));
            Assert.AreEqual("9'9999'9999.3330", FormatUtils.ToWString(999999999.333m));

            Assert.AreEqual("6.3330", FormatUtils.ToWString(6.333m));
            Assert.AreEqual("56.3330", FormatUtils.ToWString(56.333m));
            Assert.AreEqual("456.3330", FormatUtils.ToWString(456.333m));
            Assert.AreEqual("3456.3330", FormatUtils.ToWString(3456.333m));
            Assert.AreEqual("12'3456.3330", FormatUtils.ToWString(123456.333m));
            Assert.AreEqual("120'3456.3330", FormatUtils.ToWString(1203456.333m));
            Assert.AreEqual("1200'3456.3330", FormatUtils.ToWString(12003456.333m));
            Assert.AreEqual("2'1200'3456.3330", FormatUtils.ToWString(212003456.333m));
            Assert.AreEqual("32'1200'3456.3330", FormatUtils.ToWString(3212003456.333m));
            Assert.AreEqual("432'1200'3456.3330", FormatUtils.ToWString(43212003456.333m));
            Assert.AreEqual("5432'1200'3456.3330", FormatUtils.ToWString(543212003456.333m));
            Assert.AreEqual("6'5432'1200'3456.3330", FormatUtils.ToWString(6543212003456.333m));
            Assert.AreEqual("76'5432'1200'3456.3330", FormatUtils.ToWString(76543212003456.333m));
            Assert.AreEqual("876'5432'1200'3456.3330", FormatUtils.ToWString(876543212003456.333m));
            Assert.AreEqual("9876'5432'1200'3456.3330", FormatUtils.ToWString(9876543212003456.333m));
            Assert.AreEqual("1'9876'5432'1200'3456.3330", FormatUtils.ToWString(19876543212003456.333m));
            Assert.AreEqual("21'9876'5432'1200'3456.3330", FormatUtils.ToWString(219876543212003456.333m));


            Assert.AreEqual("-1.3330", FormatUtils.ToWString(-1.333m));
            Assert.AreEqual("-10.3330", FormatUtils.ToWString(-10.333m));
            Assert.AreEqual("-100.3330", FormatUtils.ToWString(-100.333m));
            Assert.AreEqual("-1000.3330", FormatUtils.ToWString(-1000.333m));
            Assert.AreEqual("-1'0000.3330", FormatUtils.ToWString(-10000.333m));
            Assert.AreEqual("-10'0000.3330", FormatUtils.ToWString(-100000.333m));
            Assert.AreEqual("-100'0000.3330", FormatUtils.ToWString(-1000000.333m));
            Assert.AreEqual("-1000'0000.3330", FormatUtils.ToWString(-10000000.333m));
            Assert.AreEqual("-1'0000'0000.3330", FormatUtils.ToWString(-100000000.333m));
            Assert.AreEqual("-10'0000'0000.3330", FormatUtils.ToWString(-1000000000.333m));

            Assert.AreEqual("-999.3330", FormatUtils.ToWString(-999.333m));
            Assert.AreEqual("-9999.3330", FormatUtils.ToWString(-9999.333m));
            Assert.AreEqual("-9'9999.3330", FormatUtils.ToWString(-99999.333m));
            Assert.AreEqual("-99'9999.3330", FormatUtils.ToWString(-999999.333m));
            Assert.AreEqual("-999'9999.3330", FormatUtils.ToWString(-9999999.333m));
            Assert.AreEqual("-9999'9999.3330", FormatUtils.ToWString(-99999999.333m));
            Assert.AreEqual("-9'9999'9999.3330", FormatUtils.ToWString(-999999999.333m));


            Assert.AreEqual("-6.3330", FormatUtils.ToWString(-6.333m));
            Assert.AreEqual("-56.3330", FormatUtils.ToWString(-56.333m));
            Assert.AreEqual("-456.3330", FormatUtils.ToWString(-456.333m));
            Assert.AreEqual("-3456.3330", FormatUtils.ToWString(-3456.333m));
            Assert.AreEqual("-12'3456.3330", FormatUtils.ToWString(-123456.333m));
            Assert.AreEqual("-120'3456.3330", FormatUtils.ToWString(-1203456.333m));
            Assert.AreEqual("-1200'3456.3330", FormatUtils.ToWString(-12003456.333m));
            Assert.AreEqual("-2'1200'3456.3330", FormatUtils.ToWString(-212003456.333m));
            Assert.AreEqual("-32'1200'3456.3330", FormatUtils.ToWString(-3212003456.333m));
            Assert.AreEqual("-432'1200'3456.3330", FormatUtils.ToWString(-43212003456.333m));
            Assert.AreEqual("-5432'1200'3456.3330", FormatUtils.ToWString(-543212003456.333m));
            Assert.AreEqual("-6'5432'1200'3456.3330", FormatUtils.ToWString(-6543212003456.333m));
            Assert.AreEqual("-76'5432'1200'3456.3330", FormatUtils.ToWString(-76543212003456.333m));
            Assert.AreEqual("-876'5432'1200'3456.3330", FormatUtils.ToWString(-876543212003456.333m));
            Assert.AreEqual("-9876'5432'1200'3456.3330", FormatUtils.ToWString(-9876543212003456.333m));
            Assert.AreEqual("-1'9876'5432'1200'3456.3330", FormatUtils.ToWString(-19876543212003456.333m));
            Assert.AreEqual("-21'9876'5432'1200'3456.3330", FormatUtils.ToWString(-219876543212003456.333m));
        }


        [TestMethod]
        public void TestToWString4()
        {
            char separator = '_';

            Assert.AreEqual("1.3330", FormatUtils.ToWString(1.333m, separator));
            Assert.AreEqual("10.3330", FormatUtils.ToWString(10.333m, separator));
            Assert.AreEqual("100.3330", FormatUtils.ToWString(100.333m, separator));
            Assert.AreEqual("1000.3330", FormatUtils.ToWString(1000.333m, separator));
            Assert.AreEqual("1_0000.3330", FormatUtils.ToWString(10000.333m, separator));
            Assert.AreEqual("10_0000.3330", FormatUtils.ToWString(100000.333m, separator));
            Assert.AreEqual("100_0000.3330", FormatUtils.ToWString(1000000.333m, separator));
            Assert.AreEqual("1000_0000.3330", FormatUtils.ToWString(10000000.333m, separator));
            Assert.AreEqual("1_0000_0000.3330", FormatUtils.ToWString(100000000.333m, separator));
            Assert.AreEqual("10_0000_0000.3330", FormatUtils.ToWString(1000000000.333m, separator));

            Assert.AreEqual("999.3330", FormatUtils.ToWString(999.333m, separator));
            Assert.AreEqual("9999.3330", FormatUtils.ToWString(9999.333m, separator));
            Assert.AreEqual("9_9999.3330", FormatUtils.ToWString(99999.333m, separator));
            Assert.AreEqual("99_9999.3330", FormatUtils.ToWString(999999.333m, separator));
            Assert.AreEqual("999_9999.3330", FormatUtils.ToWString(9999999.333m, separator));
            Assert.AreEqual("9999_9999.3330", FormatUtils.ToWString(99999999.333m, separator));
            Assert.AreEqual("9_9999_9999.3330", FormatUtils.ToWString(999999999.333m, separator));

            Assert.AreEqual("6.3330", FormatUtils.ToWString(6.333m, separator));
            Assert.AreEqual("56.3330", FormatUtils.ToWString(56.333m, separator));
            Assert.AreEqual("456.3330", FormatUtils.ToWString(456.333m, separator));
            Assert.AreEqual("3456.3330", FormatUtils.ToWString(3456.333m, separator));
            Assert.AreEqual("12_3456.3330", FormatUtils.ToWString(123456.333m, separator));
            Assert.AreEqual("120_3456.3330", FormatUtils.ToWString(1203456.333m, separator));
            Assert.AreEqual("1200_3456.3330", FormatUtils.ToWString(12003456.333m, separator));
            Assert.AreEqual("2_1200_3456.3330", FormatUtils.ToWString(212003456.333m, separator));
            Assert.AreEqual("32_1200_3456.3330", FormatUtils.ToWString(3212003456.333m, separator));
            Assert.AreEqual("432_1200_3456.3330", FormatUtils.ToWString(43212003456.333m, separator));
            Assert.AreEqual("5432_1200_3456.3330", FormatUtils.ToWString(543212003456.333m, separator));
            Assert.AreEqual("6_5432_1200_3456.3330", FormatUtils.ToWString(6543212003456.333m, separator));
            Assert.AreEqual("76_5432_1200_3456.3330", FormatUtils.ToWString(76543212003456.333m, separator));
            Assert.AreEqual("876_5432_1200_3456.3330", FormatUtils.ToWString(876543212003456.333m, separator));
            Assert.AreEqual("9876_5432_1200_3456.3330", FormatUtils.ToWString(9876543212003456.333m, separator));
            Assert.AreEqual("1_9876_5432_1200_3456.3330", FormatUtils.ToWString(19876543212003456.333m, separator));
            Assert.AreEqual("21_9876_5432_1200_3456.3330", FormatUtils.ToWString(219876543212003456.333m, separator));


            Assert.AreEqual("-1.3330", FormatUtils.ToWString(-1.333m, separator));
            Assert.AreEqual("-10.3330", FormatUtils.ToWString(-10.333m, separator));
            Assert.AreEqual("-100.3330", FormatUtils.ToWString(-100.333m, separator));
            Assert.AreEqual("-1000.3330", FormatUtils.ToWString(-1000.333m, separator));
            Assert.AreEqual("-1_0000.3330", FormatUtils.ToWString(-10000.333m, separator));
            Assert.AreEqual("-10_0000.3330", FormatUtils.ToWString(-100000.333m, separator));
            Assert.AreEqual("-100_0000.3330", FormatUtils.ToWString(-1000000.333m, separator));
            Assert.AreEqual("-1000_0000.3330", FormatUtils.ToWString(-10000000.333m, separator));
            Assert.AreEqual("-1_0000_0000.3330", FormatUtils.ToWString(-100000000.333m, separator));
            Assert.AreEqual("-10_0000_0000.3330", FormatUtils.ToWString(-1000000000.333m, separator));

            Assert.AreEqual("-999.3330", FormatUtils.ToWString(-999.333m, separator));
            Assert.AreEqual("-9999.3330", FormatUtils.ToWString(-9999.333m, separator));
            Assert.AreEqual("-9_9999.3330", FormatUtils.ToWString(-99999.333m, separator));
            Assert.AreEqual("-99_9999.3330", FormatUtils.ToWString(-999999.333m, separator));
            Assert.AreEqual("-999_9999.3330", FormatUtils.ToWString(-9999999.333m, separator));
            Assert.AreEqual("-9999_9999.3330", FormatUtils.ToWString(-99999999.333m, separator));
            Assert.AreEqual("-9_9999_9999.3330", FormatUtils.ToWString(-999999999.333m, separator));


            Assert.AreEqual("-6.3330", FormatUtils.ToWString(-6.333m, separator));
            Assert.AreEqual("-56.3330", FormatUtils.ToWString(-56.333m, separator));
            Assert.AreEqual("-456.3330", FormatUtils.ToWString(-456.333m, separator));
            Assert.AreEqual("-3456.3330", FormatUtils.ToWString(-3456.333m, separator));
            Assert.AreEqual("-12_3456.3330", FormatUtils.ToWString(-123456.333m, separator));
            Assert.AreEqual("-120_3456.3330", FormatUtils.ToWString(-1203456.333m, separator));
            Assert.AreEqual("-1200_3456.3330", FormatUtils.ToWString(-12003456.333m, separator));
            Assert.AreEqual("-2_1200_3456.3330", FormatUtils.ToWString(-212003456.333m, separator));
            Assert.AreEqual("-32_1200_3456.3330", FormatUtils.ToWString(-3212003456.333m, separator));
            Assert.AreEqual("-432_1200_3456.3330", FormatUtils.ToWString(-43212003456.333m, separator));
            Assert.AreEqual("-5432_1200_3456.3330", FormatUtils.ToWString(-543212003456.333m, separator));
            Assert.AreEqual("-6_5432_1200_3456.3330", FormatUtils.ToWString(-6543212003456.333m, separator));
            Assert.AreEqual("-76_5432_1200_3456.3330", FormatUtils.ToWString(-76543212003456.333m, separator));
            Assert.AreEqual("-876_5432_1200_3456.3330", FormatUtils.ToWString(-876543212003456.333m, separator));
            Assert.AreEqual("-9876_5432_1200_3456.3330", FormatUtils.ToWString(-9876543212003456.333m, separator));
            Assert.AreEqual("-1_9876_5432_1200_3456.3330", FormatUtils.ToWString(-19876543212003456.333m, separator));
            Assert.AreEqual("-21_9876_5432_1200_3456.3330", FormatUtils.ToWString(-219876543212003456.333m, separator));
        }

        //[TestMethod]
        //public void TestToWString2()
        //{
        //    int count = 10 * 1000;

        //    string x = null;
        //    Stopwatch watch = Stopwatch.StartNew();

        //    for( int i = 0; i < count; i++ ) {
        //        x = FormatUtils.ToWString(219876543212003456);
        //        x = FormatUtils.ToWString(-219876543212003456);
        //    }

        //    watch.Stop();
        //    Console.WriteLine(watch.Elapsed.ToString());





        //    watch = Stopwatch.StartNew();

        //    for( int i = 0; i < count; i++ ) {
        //        x = FormatUtils.ToWString2(219876543212003456);
        //        x = FormatUtils.ToWString2(-219876543212003456);
        //    }

        //    watch.Stop();
        //    Console.WriteLine(watch.Elapsed.ToString());
        //}


        [TestMethod]
        public void Test_long_ToKString()
        {
            Assert.AreEqual("6,543,212,003,456", FormatUtils.ToKString(6543212003456L));
            Assert.AreEqual("6,543,212,003,456.00", FormatUtils.ToKString(6543212003456L, 2));
            Assert.AreEqual("6,543,212,003,456.00000", FormatUtils.ToKString(6543212003456L, 5));
        }

        [TestMethod]
        public void Test_int_ToKString()
        {
            Assert.AreEqual("12,003,456", FormatUtils.ToKString(12003456));
            Assert.AreEqual("12,003,456.00", FormatUtils.ToKString(12003456, 2));
            Assert.AreEqual("12,003,456.00000", FormatUtils.ToKString(12003456, 5));
        }
    }
}
