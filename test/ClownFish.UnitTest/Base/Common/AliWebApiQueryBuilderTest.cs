using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.UnitTest.Base.Common;
[TestClass]
public class AliWebApiQueryBuilderTest
{
    [TestMethod]
    public void Test1()
    {
        string query = new AliWebApiQueryBuilder("AccessKeyId_xxxxx", "AccessKeySecret_xxxxxxxx")
                                        .AddParam("Action", "RecognizeGeneral")
                                        .AddParam("Version", "2021-07-07")
                                        .GetQueryString("POST");

        //byte[] fileBody = File.ReadAllBytes(@"D:\test-ocr-img.png");
        byte[] fileBody = Guid.NewGuid().ToByteArray();

        HttpOption httpOption = new HttpOption {
            Method = "POST",
            Url = "http://ocr-api.cn-hangzhou.aliyuncs.com/?" + query,
            Format = SerializeFormat.Form,
            Data = new {
                body = new HttpFile { FileName = "image1.png", FileBody = fileBody }
            },
        };

        string text = httpOption.ToAllText();

        Console.WriteLine(text);
    }


    [TestMethod]
    public void Test2()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = new AliWebApiQueryBuilder(null, "xxx");
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = new AliWebApiQueryBuilder("xxx", null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            var x = new AliWebApiQueryBuilder("AccessKeyId_xxxxx", "AccessKeySecret_xxxxxxxx");
            x.AddParam(null, "xxx");
        });
        
    }
}
