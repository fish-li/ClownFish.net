using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.CodeDom
{
	/// <summary>
	/// 表示编译异常
	/// </summary>
	[Serializable]
	public sealed class CompileException : System.Exception
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		public CompileException()
		{
		}

		/// <summary>
		/// 构造函数
		/// </summary>
		public CompileException(string code, CompilerResults compilerResult)
		{
			this.Code = code;
			this.CompilerResult = compilerResult;
		}

		/// <summary>
		/// 需要编译的代码
		/// </summary>
		public string Code	{ get; internal set; }

		/// <summary>
		/// 编译结果
		/// </summary>
		public CompilerResults CompilerResult { get; internal set; }

        /// <summary>
        /// 异常消息的描述信息
        /// </summary>
		public override string Message
		{
			get
			{
				return CompilerHelper.GetCompileErrorMessage(this.CompilerResult);
			}
		}


        /// <summary>
        /// 当在派生类中重写时，用于异常的信息设置
        /// </summary>
        /// <param name="info">它存有有关所引发异常的序列化的对象数据。</param>
        /// <param name="context">它包含有关源或目标的上下文信息。</param>
        public CompileException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Code = info.GetString("Code");
			this.CompilerResult = (CompilerResults)info.GetValue("CompilerResult", typeof(CompilerResults));
		}


		/// <summary>
		/// 当在派生类中重写时，用于异常的信息设置
		/// </summary>
		/// <param name="info">它存有有关所引发异常的序列化的对象数据。</param>
		/// <param name="context">它包含有关源或目标的上下文信息。</param>
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Code", this.Code);
			info.AddValue("CompilerResult", this.CompilerResult, typeof(CompilerResults));
		}
	}
}
