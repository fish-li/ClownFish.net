using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Http.Pipleline;

namespace ClownFish.WebApi.Result
{
    /// <summary>
    /// 表示一个二进制的Action执行结果
    /// </summary>
    public sealed class BinaryResult : IActionResult
    {
        // 说明：
        // 这个类的本质就是 StreamResult
        // 只是当开发人员遇到需要返回二进制结果时，通常不会想到 StreamResult
        // 所以为了方便使用，增加了这个类型
        // 所以，可认为 BinaryResult 是 StreamResult 的一个别名


        // 服务端返回二进制结果属于【小众】场景，框架就不做Action执行后的自动识别了
        // 所以要求Action的返回值类型是 BinaryResult
        // 即： return new BinaryResult(buffer);


        private StreamResult _streamResult;

        /// <summary>
		/// 构造函数
		/// </summary>
		/// <param name="buffer">文件内容的字节数组</param>
		public BinaryResult(byte[] buffer)
        {
            if( buffer == null || buffer.Length == 0 )
                throw new ArgumentNullException("buffer");

            _streamResult = new StreamResult(buffer);
        }

        /// <summary>
        /// 实现IActionResult接口，执行输出
        /// </summary>
        /// <param name="context"></param>
        public void Ouput(NHttpContext context)
        {
            _streamResult.Ouput(context);
        }
    }
}
