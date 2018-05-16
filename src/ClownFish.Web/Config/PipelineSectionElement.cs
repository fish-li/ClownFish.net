using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;

namespace ClownFish.Web.Config
{
	/// <summary>
	/// Pipeline相关的配置节
	/// </summary>
	public sealed class PipelineSectionElement : ConfigurationElement
	{
		/// <summary>
		/// 404错误页的模板页面路径
		/// </summary>
		[ConfigurationProperty("http404TemplatePagePath", IsRequired = false, DefaultValue = "/404DiagnoseResult.aspx")]
		public string Http404PagePath
		{
			get { return this["http404TemplatePagePath"].ToString(); }
		}
		/// <summary>
		/// 
		/// </summary>
		protected override void PostDeserialize()
		{
			try {
				string templatePath = UiHelper.AppRoot + this.Http404PagePath.Replace("/", "\\");

				if( RetryFile.Exists(templatePath) == false )
					throw new System.IO.FileNotFoundException();
			}
			catch {
				throw new ConfigurationErrorsException("pipeline.http404TemplatePagePath 配置值无效。");
			}
		}
	}
}
