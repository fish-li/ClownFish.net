using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base.Http
{
    /// <summary>
    /// 常用的请求内容类型
    /// </summary>
    public static class RequestContentType
    {
        /// <summary>
        /// 表示以FORM方式提交的数据格式
        /// </summary>
        public static readonly string Form = "application/x-www-form-urlencoded";
        
        /// <summary>
		/// 表示包含上传文件的提交格式
		/// </summary>
		public static readonly string Multipart = "multipart/form-data";

        /// <summary>
        /// 表示以JSON形式提交数据
        /// </summary>
        public static readonly string Json = "application/json";

        /// <summary>
        /// 表示以XML形式提交数据
        /// </summary>
        public static readonly string Xml = "application/xml";


        /// <summary>
        /// 根据 Content-Type 请求头字符串，转换成SerializeFormat枚举
        /// </summary>
        /// <param name="contentType"></param>
        /// <returns></returns>
        public static SerializeFormat GetFormat(string contentType)
        {
            if( string.IsNullOrEmpty(contentType) )
                return SerializeFormat.None;

            if( contentType.IndexOfIgnoreCase(RequestContentType.Form) >= 0
                || contentType.IndexOfIgnoreCase(RequestContentType.Multipart) >= 0 )
                return SerializeFormat.Form;

            if( contentType.IndexOfIgnoreCase(RequestContentType.Json) >= 0 )
                return SerializeFormat.Json;

            if( contentType.IndexOfIgnoreCase(RequestContentType.Xml) >= 0 )
                return SerializeFormat.Xml;

            return SerializeFormat.Unknown;
        }


        /// <summary>
        /// 根据SerializeFormat枚举转换成 Content-Type 请求头字符串，
        /// 对于无效的枚举，返回空字符串“”
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public static string GetByFormat(SerializeFormat format)
        {
            switch( format ) {
                case SerializeFormat.Form:
                    return RequestContentType.Form;

                case SerializeFormat.Json:
                    return RequestContentType.Json;

                case SerializeFormat.Xml:
                    return RequestContentType.Xml;

                default:
                    return string.Empty;
            }
        }



    }
}
