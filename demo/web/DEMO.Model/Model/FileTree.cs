using System.Collections.Generic;

namespace DEMO.Model
{
	public sealed class JsTreeNode
	{
		//public int id;
		public string text;
		public string state;
		public string iconCls;
		public List<JsTreeNode> children;
		public JsTreeNodeCustAttr attributes;
	}


	public sealed class JsTreeNodeCustAttr
	{
		public static readonly string InvalidFilePath = "###";

		public string FilePath;
		public string FileType;

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