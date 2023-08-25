using System;
using System.Data.Common;
using System.Text;
using ClownFish.Data;

namespace ClownFish.UnitTest.Data.Events
{
	public static class ClownFishDataEventSubscriber
	{
		internal static string LastQuery { get; private set; }
		internal static string LastExecuteSQL { get; private set; }

		public static void SubscribeEvent()
		{
            DbContextEvent.OnBeforeExecute += DbContextEvent_OnBeforeExecute;
            DbContextEvent.OnAfterExecute += CommandAfterExecute;
		}

        private static void DbContextEvent_OnBeforeExecute(object sender, ExecuteCommandEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void CommandAfterExecute(object sender, ExecuteCommandEventArgs e)
		{
			LastQuery = e.Command.Command.CommandText;

			StringBuilder sb = new StringBuilder();

			DbCommand command = e.Command.Command;
			sb.AppendLine().AppendLine(command.CommandText);
			
			if( command.Parameters != null )
				foreach( DbParameter p in command.Parameters )
					sb.AppendFormat("{0}: ({1}), {2}\r\n", 
								p.ParameterName,
                                GetParameterType(p), 
								((p.Value == null || p.Value == DBNull.Value) ? "NULL" :  p.Value.ToString()));

			LastExecuteSQL = sb.ToString();

			//Console.WriteLine(LastExecuteSQL);
		}

        private static string GetParameterType(DbParameter p)
        {
            try {
                return p.DbType.ToString();
            }
            catch {
                return "Unknow";
            }
        }
	}
}
