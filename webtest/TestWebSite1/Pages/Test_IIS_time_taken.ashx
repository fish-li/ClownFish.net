<%@ WebHandler Language="C#" Class="Test_IIS_time_taken" %>

using System;
using System.Web;

public class Test_IIS_time_taken : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";

		System.Threading.Thread.Sleep(1000 * 2);
		
		context.Response.Write(string.Format("{0}, {1}\r\n", "Start", DateTime.Now));
		context.Response.Flush();
		
		System.Threading.Thread.Sleep(1000 * 2);

		for( int i = 0; i < 20; i++ ) {
			context.Response.Write(string.Format("{0}, {1}\r\n", i, DateTime.Now));
			context.Response.Flush();
			System.Threading.Thread.Sleep(1000 * 1);
		}

		context.Response.Write("End");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}