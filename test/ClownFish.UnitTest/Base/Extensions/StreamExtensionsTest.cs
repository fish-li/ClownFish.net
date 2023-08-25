using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Extensions
{
    [TestClass]
    public class StreamExtensionsTest
    {
        [TestMethod]
        public void Test_ToMemoryStream()
        {
            byte[] bb = Guid.NewGuid().ToByteArray();
            using( MemoryStream  ms = new MemoryStream(bb, false) ) {

                using( MemoryStream m2 = MemoryStreamPool.GetStream() ) {
                    ms.CopyToMemoryStream(m2);

                    // 原始流已到尾部
                    Assert.AreEqual(bb.Length, ms.Position);

                    // 结果流指针已定位到开头
                    Assert.AreEqual(0, m2.Position);
                    Assert.AreEqual(bb.Length, m2.Length);
                }
            }
        }

        [TestMethod]
        public async Task Test_ToMemoryStreamAsync()
        {
            byte[] bb = Guid.NewGuid().ToByteArray();
            using( MemoryStream ms = new MemoryStream(bb, false) ) {

                using( MemoryStream m2 = MemoryStreamPool.GetStream() ) {
                    await ms.CopyToMemoryStreamAsync(m2);

                    // 原始流已到尾部
                    Assert.AreEqual(bb.Length, ms.Position);

                    // 结果流指针已定位到开头
                    Assert.AreEqual(0, m2.Position);
                    Assert.AreEqual(bb.Length, m2.Length);
                }
            }
        }


        [TestMethod]
        public void Test_ToArray()
        {
            byte[] bb = Guid.NewGuid().ToByteArray();
            using( MemoryStream ms = new MemoryStream(bb, false) ) {

                byte[] b2 = StreamExtensions.ToArray(ms);

                Assert.IsTrue(bb.IsEqual(b2));
            }
        }


        [TestMethod]
        public async Task Test_ToArrayAsync()
        {
            byte[] bb = Guid.NewGuid().ToByteArray();
            using( MemoryStream ms = new MemoryStream(bb, false) ) {

                byte[] b2 = await StreamExtensions.ToArrayAsync(ms);

                Assert.IsTrue(bb.IsEqual(b2));
            }
        }


        [TestMethod]
        public void Test_Error()
        {
            MemoryStream msObject = new MemoryStream();

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = StreamExtensions.ToArray(null);
            });


            MyAssert.IsError<ArgumentNullException>(() => {
                StreamExtensions.CopyToMemoryStream(null, msObject);
            });


            byte[] bb = Encoding.UTF8.GetBytes("xxxxxxxxxxxxxx");
            using( MemoryStream ms = MemoryStreamPool.GetStream() ) {
                ms.Write(bb, 0, bb.Length);

                MemoryStream ms2 = MemoryStreamPool.GetStream();
                ms.CopyToMemoryStream(ms2);

                byte[] bb2 = ms2.ToArray();

                Assert.IsTrue(bb.IsEqual(bb2));
            }

            MyAssert.IsError<InvalidOperationException>(() => {
                MyTestStream stream = new MyTestStream();
                StreamExtensions.CopyToMemoryStream(stream, msObject);
            });

            MyAssert.IsError<InvalidOperationException>(() => {
                MyTestStream stream = new MyTestStream();
                _ = stream.ToArray();
            });
        }





        public class MyTestStream : Stream
        {
            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => throw new NotImplementedException();

            public override long Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public override void Flush()
            {
                throw new NotImplementedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotImplementedException();
            }

            public override void SetLength(long value)
            {
                throw new NotImplementedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotImplementedException();
            }
        }
    }
}
