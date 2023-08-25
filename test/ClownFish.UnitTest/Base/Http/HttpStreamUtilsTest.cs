using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;

#if NETCOREAPP
namespace ClownFish.UnitTest.Base.Http
{
    [TestClass]
    public class HttpStreamUtilsTest
    {
        private static readonly string s_inputText = "namespace ClownFish.UnitTest.Base.Http，中文汉字, !@$^WE^%$&!@$!@$&*(";

        [TestMethod]
        public async Task Test_normal()
        {
            using MemoryStream ms1 = new MemoryStream();

            HttpStreamWriter writer1 = new HttpStreamWriter(ms1);
            await writer1.WriteAsync(s_inputText);



            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StreamContent(ms1);

            Assert.AreEqual(s_inputText, response.ReadBodyAsText());
        }


        [TestMethod]
        public async Task Test_normalAsync()
        {
            using MemoryStream ms1 = new MemoryStream();

            HttpStreamWriter writer1 = new HttpStreamWriter(ms1);
            await writer1.WriteAsync(s_inputText);


            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StreamContent(ms1);

            Assert.AreEqual(s_inputText, await response.ReadBodyAsTextAsync());
        }


        [TestMethod]
        public async Task Test_gzip()
        {
            using MemoryStream ms1 = new MemoryStream();

            HttpStreamWriter writer1 = new HttpStreamWriter(ms1, "gzip");
            await writer1.WriteAsync(s_inputText);


            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StreamContent(ms1);
            response.Content.Headers.TryAddWithoutValidation("Content-Encoding", "gzip");

            Assert.AreEqual(s_inputText, response.ReadBodyAsText());
        }

        [TestMethod]
        public async Task Test_gzipAsync()
        {
            using MemoryStream ms1 = new MemoryStream();

            HttpStreamWriter writer1 = new HttpStreamWriter(ms1, "gzip");
            await writer1.WriteAsync(s_inputText);


            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StreamContent(ms1);
            response.Content.Headers.TryAddWithoutValidation("Content-Encoding", "gzip");

            Assert.AreEqual(s_inputText, await response.ReadBodyAsTextAsync());
        }

        [TestMethod]
        public async Task Test_deflate()
        {
            using MemoryStream ms1 = new MemoryStream();

            HttpStreamWriter writer1 = new HttpStreamWriter(ms1, "deflate");
            await writer1.WriteAsync(s_inputText);



            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StreamContent(ms1);
            response.Content.Headers.TryAddWithoutValidation("Content-Encoding", "deflate");

            Assert.AreEqual(s_inputText, response.ReadBodyAsText());
        }


        [TestMethod]
        public async Task Test_deflateAsync()
        {
            using MemoryStream ms1 = new MemoryStream();

            HttpStreamWriter writer1 = new HttpStreamWriter(ms1, "deflate");
            await writer1.WriteAsync(s_inputText);


            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StreamContent(ms1);
            response.Content.Headers.TryAddWithoutValidation("Content-Encoding", "deflate");

            Assert.AreEqual(s_inputText, await response.ReadBodyAsTextAsync());
        }

        [TestMethod]
        public async Task Test_br()
        {
            using MemoryStream ms1 = new MemoryStream();

            HttpStreamWriter writer1 = new HttpStreamWriter(ms1, "br");
            await writer1.WriteAsync(s_inputText);



            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StreamContent(ms1);
            response.Content.Headers.TryAddWithoutValidation("Content-Encoding", "br");

            Assert.AreEqual(s_inputText, response.ReadBodyAsText());
        }


        [TestMethod]
        public async Task Test_brAsync()
        {
            using MemoryStream ms1 = new MemoryStream();

            HttpStreamWriter writer1 = new HttpStreamWriter(ms1, "br");
            await writer1.WriteAsync(s_inputText);


            HttpResponseMessage response = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            response.Content = new StreamContent(ms1);
            response.Content.Headers.TryAddWithoutValidation("Content-Encoding", "br");

            Assert.AreEqual(s_inputText, await response.ReadBodyAsTextAsync());
        }



        [TestMethod]
        public async Task Test_Error()
        {
            MemoryStream ms = new MemoryStream();
            HttpResponseMessage response = null;

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = response.ReadBodyAsText();
            });

            await MyAssert.IsErrorAsync<ArgumentNullException>(async () => {
                _ = await response.ReadBodyAsTextAsync();
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = new HttpStreamReader(null);
            });

            MyAssert.IsError<ArgumentNullException>(() => {
                _ = new HttpStreamWriter(null);
            });


            MyAssert.IsError<NotSupportedException>(() => {
                _ = new HttpStreamReader(ms, "xxzip");
            });

            MyAssert.IsError<NotSupportedException>(() => {
                _ = new HttpStreamWriter(ms, "xxzip");
            });

        }

    }
}
#endif
