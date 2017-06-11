using System.Collections.Generic;

namespace DEMO.Models
{
	public sealed class JsTreeNode
	{
		//public int id { get; set; }
		public string text { get; set; }
		public string state { get; set; }
		public string iconCls { get; set; }
		public List<JsTreeNode> children { get; set; }
		public JsTreeNodeCustAttr attributes { get; set; }
	}


	public sealed class JsTreeNodeCustAttr
	{
		public static readonly string InvalidFilePath = "###";

		public string FilePath { get; set; }
		public string FileType { get; set; }

		public JsTreeNodeCustAttr()
		{
			this.FilePath = InvalidFilePath;
			this.FileType = string.Empty;
		}
		public JsTreeNodeCustAttr(string path, string ext)
		{
			this.FilePath = path;
			this.FileType = ext;
		}
	}
}