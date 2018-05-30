using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.Log.UnitTest
{
    [TestClass]
    public class DeleteLog
    {
        //[TestMethod]
        public void DeleteEventLog()
        {
            string[] names = { "ClownFish", "ClownFish-SyncData", "ClownFish-LogTest", "LogTest", "ClownFish-Log" };

            foreach(var name in names ) {

                try {
                    EventLog.Delete(name);
                }
                catch( Exception ex1 ) { string xx = ex1.Message; }

                try {
                    EventLog.DeleteEventSource(name);
                }
                catch( Exception ex1 ) { string xx = ex1.Message; }

            }
            
        }

    }
}
