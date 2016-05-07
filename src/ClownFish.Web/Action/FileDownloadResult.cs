using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ClownFish.Web
{
	/// <summary>
	/// 文件下载的执行结果
	/// </summary>
	public sealed class FileDownloadResult : IActionResult
	{
		private string _filePath;
		private string _downLoadFileName;

		/// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="filePath">让浏览器下载的文件路径</param>
		/// <param name="downLoadFileName">下载对话框显示的文件名</param>
		public FileDownloadResult(string filePath, string downLoadFileName)
		{
			if( string.IsNullOrEmpty(filePath) )
				throw new ArgumentNullException("filePath");

			_filePath = filePath;
			_downLoadFileName = downLoadFileName;
		}

		/// <summary>
		/// 实现IActionResult接口，执行输出
		/// </summary>
		/// <param name="context"></param>
		public void Ouput(HttpContext context)
		{
			// 设置当前响应的文档类型
			context.Response.ContentType = StreamResult.DefaultContentType;

			// 设置浏览器下载对话框中的保存文件名称
			StreamResult.SetDownloadFileName(context, _downLoadFileName);

			// 下载文件
			context.Response.TransmitFile(_filePath);
		}
	}
}
