//using ClownFish.Email;

//namespace ClownFish.UnitTest.Email;
//[TestClass]
//public class MailClientTest
//{
//    [TestMethod]
//    public void Test1()
//    {
//        string configValue = EnvironmentVariables.Get("mail_config");
//        if( configValue.IsNullOrEmpty() ) {
//            Console.WriteLine("环境变量 mail_config 没有设置。");
//            Console.WriteLine("mail_config的格式：Host=smtp.exmail.qq.com;Port=587;UserName=xx@abc.com;Password=xxxxx;IsSSL=1");
//            return;
//        }
//        else {
//            Console.WriteLine("mail_config: " + configValue);
//        }

//        SmtpConfig config = configValue.ToObject<SmtpConfig>();


//        MailClient client = new MailClient(config);
//        client.LogFilePath = "MimeKit.log";

//        client
//            .SetReceivers("liqf01@mingyuanyun.com", "yangmc@mingyuanyun.com", "lvj01@mingyuanyun.com", "zhouh09@mingyuanyun.com", "412537239@qq.com")

//            //.SetReceivers("李奇峰1 <liqf01@mingyuanyun.com>", "方武2 <fangw@mingyuanyun.com>",
//            //              "杨敏超3 <yangmc@mingyuanyun.com>", "杨敏超4 <412537239@qq.com>")

//            //.SetReceivers( new NameValue("李奇峰1", "liqf01@mingyuanyun.com"), new NameValue("方武2", "fangw@mingyuanyun.com"), 
//            //               new NameValue("杨敏超3", "yangmc@mingyuanyun.com"), new NameValue("杨敏超4", "412537239@qq.com") )

//            .SetCC("liqf01@mingyuanyun.com")
//            .SetSubject("Test MimeKit" + DateTime.Now.ToTimeString())
//            .SetBody("aaaaaaaaaaaaaaaaaa")
//            .Send();

//        Console.WriteLine("Send mail OK !");
//    }
//}
