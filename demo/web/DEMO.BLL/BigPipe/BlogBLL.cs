using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using DEMO.Model.BigPipe;


namespace DEMO.BLL.BigPipe
{
	/// <summary>
	///BlogBLL 的摘要说明
	/// </summary>
	public class BlogBLL
	{
		public List<BlogLink> GetLinks(string linksFilePath)
		{
			System.Threading.Thread.Sleep(2000);

			List<string> lines = (from line in File.ReadAllLines(linksFilePath)
								  where line.Length > 0
								  select line).ToList();
			if( lines.Count % 2 != 0 )
				throw new FormatException(linksFilePath + "的格式不是预期的格式。");

			List<BlogLink> list = new List<BlogLink>();
			for( int i = 0; i < lines.Count; i += 2 )
				list.Add(new BlogLink { Href = lines[i], Text = lines[i + 1] });

			return list;
		}

		public List<Comment> GetComments(string commentFilePath)
		{
			System.Threading.Thread.Sleep(2000);

			string[] separator = new string[] { "----------------------------------------" };
			string[] lines = File.ReadAllText(commentFilePath).Split(separator, StringSplitOptions.RemoveEmptyEntries);


			List<Comment> list = new List<Comment>();
			foreach( string line in lines ) {
				string[] pair = line.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
				if( pair.Length >= 2 ) {
					list.Add(new Comment { Title = pair[0], Text = string.Join("<br />", pair.Skip(1).ToArray()) });
				}
			}
			return list;
		}

		public BlogEntity GetBlog(string blogFilePath)
		{
			System.Threading.Thread.Sleep(1000);

			return new BlogEntity { Text = File.ReadAllText(blogFilePath) };
		}
	}
}
