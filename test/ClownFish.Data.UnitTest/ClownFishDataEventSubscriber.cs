using System;
using System.Data.Common;
using System.Text;
using ClownFish.Base.TypeExtend;

namespace ClownFish.Data.UnitTest
{
	public sealed class ClownFishDataEventSubscriber : EventSubscriber<EventManager>
	{
		internal static string LastExecuteSQL;

		public override void SubscribeEvent(EventManager instance)
		{
			instance.BeforeExecute += Instance_BeforeExecute;
		}

		private void Instance_BeforeExecute(object sender, CommandEventArgs e)
		{
			StringBuilder sb = new StringBuilder();

			DbCommand command = e.Command.Command;
			sb.AppendLine().AppendLine(command.CommandText);
			
			if( command.Parameters != null )
				foreach( DbParameter p in command.Parameters )
					sb.AppendFormat("{0}: ({1}), {2}\r\n", 
								p.ParameterName, 
								p.DbType.ToString(), 
								(p.Value == DBNull.Value ? "NULL" :  p.Value.ToString()));

			LastExecuteSQL = sb.ToString();
			Console.WriteLine(LastExecuteSQL);

		}
	}
}
