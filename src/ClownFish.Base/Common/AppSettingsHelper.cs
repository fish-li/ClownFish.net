using System;
using System.Collections.Generic;
using System.Configuration;


namespace ClownFish.Base
{
    /// <summary>
    /// 读取AppSettings的工具类
    /// </summary>
    public static class AppSettingsHelper
    {
        /// <summary>
        /// 从 AppSettings 中获取一个整数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ReadInt(string name, int defaultValue)
        {
            if( string.IsNullOrEmpty(name) )
                throw new ArgumentNullException(nameof(name));


            string value = ConfigurationManager.AppSettings[name];

            // 允许不指定，就用默认值返回
            if( string.IsNullOrEmpty(value) )
                return defaultValue;

            int result = 0;
            if( int.TryParse(value, out result) )
                return result;

            // 如果有指定设置，就必须是正确的！
            throw new ConfigurationErrorsException($"指定名称 {name} 对应的配置值 {value} 无效");
        }


        /// <summary>
        /// 从 AppSettings 中获取一个【正整数】
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static int ReadUInt(string name, int defaultValue)
        {
            int value = ReadInt(name, defaultValue);
            if( value > 0 )
                return value;
            
            throw new ConfigurationErrorsException($"指定名称 {name} 对应的配置值 {value} 无效");
        }

    }
}
