//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.IO;
//using System.Web.Hosting;

//namespace ClownFish.MockAspnetRuntime
//{
//    internal class MockWorkerRequest : SimpleWorkerRequest
//    {
		
//        public MockWorkerRequest()
//            : base("~/", "c:\\inetpub\\wwwroot\\webapp\\", "default.aspx", "", new StringWriter())
//        // public SimpleWorkerRequest(string appVirtualDir, string appPhysicalDir, string page, string query, TextWriter output);
//        { }

//        public override string GetAppPathTranslated()
//        {
//            return base.GetAppPathTranslated();
//        }

//        public override string GetRemoteAddress()
//        {
//            return base.GetRemoteAddress();
//        }

//        public override string GetKnownRequestHeader(int index)
//        {
//            if( index == 0x27 )// UserAgent
//                return "Test";

//            return base.GetKnownRequestHeader(index);
//        }

//    }
//}
