using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class DateTimeExtensionsTest
    {
        [TestMethod]
        public void Test_ToTimeSpan()
        {
            DateTime dateTime = new DateTime(2023, 2, 15, 17, 33, 59, 222);
            TimeSpan time = dateTime.ToTimeSpan();
            Assert.AreEqual(17, time.Hours);
            Assert.AreEqual(33, time.Minutes);
            Assert.AreEqual(59, time.Seconds);
            Assert.AreEqual(0, time.Milliseconds);
            Assert.AreEqual(0, time.Days);
        }


        [TestMethod]
        public void Test_ToTime8String()
        {
            DateTime dateTime = new DateTime(2023, 2, 15, 5, 33, 59, 222);
            TimeSpan time = dateTime.ToTimeSpan();

            string text = time.ToTime8String();
            Assert.AreEqual("05:33:59", text);
        }


        [TestMethod]
        public void Test_日期转字符串_长格式()
        {
            DateTime dt = new DateTime(2016, 1, 2, 11, 22, 33);
            Assert.AreEqual("2016-01-02 11:22:33", dt.ToTimeString());

            string text = dt.ToTime27String();
            Assert.IsTrue(text.StartsWith("2016-01-02 11:22:33"));
            Assert.AreEqual(27, text.Length);
        }

        [TestMethod]
        public void Test_日期转字符串_短格式()
        {
            DateTime dt = new DateTime(2016, 1, 2, 11, 22, 33);
            Assert.AreEqual("2016-01-02", dt.ToDateString());
        }

        [TestMethod]
        public void Test_ToDateTime()
        {
            var time1 = DateTimeExtensions.ToDateTime("2021-03-02");
            Assert.AreEqual(2021, time1.Year);
            Assert.AreEqual(3, time1.Month);
            Assert.AreEqual(2, time1.Day);

            var time2 = DateTimeExtensions.ToDateTime("2021-03-02 11:22:33");
            Assert.AreEqual(time1.Date, time2.Date);
            Assert.AreEqual(11, time2.Hour);
            Assert.AreEqual(22, time2.Minute);
            Assert.AreEqual(33, time2.Second);


            var time3 = DateTimeExtensions.ToDateTime("2021-03-02 11:22:33.2222");
            Assert.AreEqual(time1.Date, time3.Date);
            Assert.AreEqual(11, time3.Hour);
            Assert.AreEqual(22, time3.Minute);
            Assert.AreEqual(33, time3.Second);


            var time4 = DateTimeExtensions.ToDateTime("20210302112233");
            Assert.AreEqual(time2, time4);

            var time5 = DateTimeExtensions.ToDateTime(DateTime.Now.Ticks.ToString());
            Assert.IsTrue((DateTime.Now - time5).TotalSeconds < 3);

            var time6 = DateTimeExtensions.ToDateTime("20210302");
            Assert.AreEqual(time1, time6);
        }

        [TestMethod]
        public void Test_ToDateTime2()
        {
            DateTime testTime = new DateTime(2000, 5, 3, 8, 11, 19);

            Assert.AreEqual(testTime, testTime.ToTimeString().ToDateTime());
            Assert.AreEqual(testTime, testTime.ToTime14().ToDateTime());
            Assert.AreEqual(testTime, testTime.Ticks.ToString().ToDateTime());
            Assert.AreEqual(testTime.Date, testTime.ToDateString().ToDateTime());
        }

        [TestMethod]
        public void Test_TryToDateTime()
        {
            var time1 = DateTimeExtensions.TryToDateTime("2021-03-02");
            Assert.AreEqual(2021, time1.Year);
            Assert.AreEqual(3, time1.Month);
            Assert.AreEqual(2, time1.Day);

            var time2 = DateTimeExtensions.TryToDateTime("2021-03-02 11:22:33");
            Assert.AreEqual(time1.Date, time2.Date);
            Assert.AreEqual(11, time2.Hour);
            Assert.AreEqual(22, time2.Minute);
            Assert.AreEqual(33, time2.Second);


            var time3 = DateTimeExtensions.TryToDateTime("2021-03-02 11:22:33.2222");
            Assert.AreEqual(time1.Date, time3.Date);
            Assert.AreEqual(11, time3.Hour);
            Assert.AreEqual(22, time3.Minute);
            Assert.AreEqual(33, time3.Second);


            var time4 = DateTimeExtensions.TryToDateTime("20210302112233");
            Assert.AreEqual(time2, time4);

            var time5 = DateTimeExtensions.TryToDateTime(DateTime.Now.Ticks.ToString());
            Assert.IsTrue((DateTime.Now - time5).TotalSeconds < 3);

            var time6 = DateTimeExtensions.TryToDateTime("20210302");
            Assert.AreEqual(time1, time6);
        }


        [TestMethod]
        public void Test_TryToDateTime_Error()
        {
            DateTime minTime = DateTime.MinValue;
            DateTime today = DateTime.Today;

            var time1 = DateTimeExtensions.TryToDateTime("20213302");
            Assert.AreEqual(minTime, time1);

            var time2 = DateTimeExtensions.TryToDateTime("2021-03-02 55:22:33.2222");
            Assert.AreEqual(minTime, time2);

            var time3 = DateTimeExtensions.TryToDateTime("20213302", today);
            Assert.AreEqual(today, time3);

            var time4 = DateTimeExtensions.TryToDateTime("2021-03-02 55:22:33.2222", today);
            Assert.AreEqual(today, time4);

            bool flag1 = DateTimeExtensions.TryToDateTime("2021-03-02 55:22:33.2222", out _);
            Assert.IsFalse(flag1);

            bool flag2 = DateTimeExtensions.TryToDateTime("20213302", out _);
            Assert.IsFalse(flag2);

            bool flag3 = DateTimeExtensions.TryToDateTime("2021-03-02 15:22:33", out _);
            Assert.IsTrue(flag3);
        }


        [TestMethod]
        public void Test_Error()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                var time = DateTimeExtensions.ToDateTime("");
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                var time = DateTimeExtensions.ToDateTime(null);
            });

            MyAssert.IsError<ArgumentException>(() => {
                var time = DateTimeExtensions.ToDateTime("xxx");
            });

            MyAssert.IsError<ArgumentException>(() => {
                var time = DateTimeExtensions.ToDateTime("2021-33-02");
            });

            MyAssert.IsError<ArgumentException>(() => {
                var time = DateTimeExtensions.ToDateTime("2021-33-02 11:11");
            });

            MyAssert.IsError<ArgumentException>(() => {
                var time = DateTimeExtensions.ToDateTime("2021-33-02 11:11:11");
            });

            MyAssert.IsError<ArgumentException>(() => {
                var time = DateTimeExtensions.ToDateTime("20213302111111");
            });

            MyAssert.IsError<ArgumentException>(() => {
                var time = DateTimeExtensions.ToDateTime("-324");
            });

            MyAssert.IsError<ArgumentException>(() => {
                var time = DateTimeExtensions.ToDateTime("63653sfsf3096875234446");
            });
        }


        [TestMethod]
        public void Test_ToTimeString()
        {
            DateTime testTime = new DateTime(2000, 5, 3, 8, 11, 19);

            Assert.AreEqual("2000-05-03 08:11:19", testTime.ToTimeString());
            Assert.AreEqual("2000-05-03", testTime.ToDateString());

            Assert.AreEqual("20000503081119", testTime.ToTime14());
            Assert.AreEqual("20000503", testTime.ToDate8());
        }


        [TestMethod]
        public void Test_AsDateTime()
        {
            DateTime time1 = "2022-02-06 14:12:59".TryToDateTime();
            Assert.AreEqual(time1, time1.Ticks.AsDateTime());

            DateTime time2 = DateTime.Now;
            Assert.AreEqual(time2, time2.Ticks.AsDateTime());

            Console.WriteLine("MinValue.Ticks   : " + DateTime.MinValue.Ticks.ToString());
            Console.WriteLine("1-01-01.Ticks    : " + (new DateTime(1, 1, 1, 0, 0, 1).Ticks.ToString()));
            Console.WriteLine("100-01-01.Ticks  : " + (new DateTime(100, 1, 1).Ticks.ToString()));
            Console.WriteLine("1000-01-01.Ticks : " + (new DateTime(1000, 1, 1).Ticks.ToString()));
            Console.WriteLine("2000-01-01.Ticks : " + (new DateTime(2000, 1, 1).Ticks.ToString()));
            Console.WriteLine("2023-01-01.Ticks : " + (new DateTime(2023, 1, 1).Ticks.ToString()));
            Console.WriteLine("MinValue.ToNumber: " + DateTime.MinValue.ToNumber().ToString());
        }

        [TestMethod]
        public void Test_NumberToDateTime()
        {
            DateTime time1 = "2022-02-06 14:12:59".TryToDateTime();
            Assert.AreEqual(time1, time1.ToNumber().ToDateTime());

            DateTime time2 = DateTime.Now.SetMillisecondToZero();
            Assert.AreEqual(time2, time2.ToNumber().ToDateTime());

            MyAssert.IsError<ArgumentOutOfRangeException>(() => {
                long ticks = 99991231235959L + 1;
                _ = ticks.ToDateTime();
            });

            Assert.AreEqual(DateTime.MinValue, (10101000000L - 1).ToDateTime());
        }

        [TestMethod]
        public void Test_ToNumber()
        {
            Assert.AreEqual(20220206141259L, "20220206141259".TryToDateTime().ToNumber());
            Assert.AreEqual(20220206141259L, "2022-02-06 14:12:59".TryToDateTime().ToNumber());

            Console.WriteLine("DateTime.MinValue: " + DateTime.MinValue.ToNumber().ToString());
            Console.WriteLine("DateTime.MinValue: " + DateTime.MinValue.Ticks.ToString());
            Console.WriteLine("0001-1-1 00:00:01: " + (new DateTime(1, 1, 1, 0, 0, 1)).Ticks.ToString());
            Console.WriteLine("0001-1-1 00:00:01: " + (new DateTime(1, 1, 1, 0, 0, 1)).ToNumber().ToString());
        }


    }
}
