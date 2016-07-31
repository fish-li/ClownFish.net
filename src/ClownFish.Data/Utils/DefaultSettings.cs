using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data
{
	internal static class DefaultSettings
	{
		public static readonly int ListSize = GetDefaultListSize();

		private static int GetDefaultListSize()
		{
			string text = ConfigurationManager.AppSettings["ClownFish.Data:DefaultSettings.ListSize"];
			int value = 32;

			if( int.TryParse(text, out value) && value > 10 )
				return value;
			else
				return 32;
		}
	}
}
