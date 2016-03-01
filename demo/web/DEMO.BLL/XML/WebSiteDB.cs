using System;
using System.Text;
using ClownFish;
using DEMO.Model;


namespace DEMO.BLL
{
	public static class WebSiteDB
	{
		private static MyNorthwind s_db;

		public static MyNorthwind MyNorthwind
		{
			get
			{
				if( s_db == null )
					throw new InvalidOperationException("DataBase is null.");

				return s_db;
			}
		}

		public static void LoadDbFromXml(string xmlPath)
		{
			if( System.IO.File.Exists(xmlPath) == false )
				throw new ArgumentException("指定的文件不存在：" + xmlPath);

			s_db = XmlHelper.XmlDeserializeFromFile<MyNorthwind>(xmlPath, Encoding.UTF8);
		}

		public static void SaveDbToXml(string xmlPath)
		{
			if( string.IsNullOrEmpty(xmlPath) )
				throw new ArgumentNullException("xmlPath");

			if( s_db != null ) {
				try {
					XmlHelper.XmlSerializeToFile(s_db, xmlPath, Encoding.UTF8);
				}
				catch { }
			}
		}

	}
}
