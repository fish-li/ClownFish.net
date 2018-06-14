using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ClownFish.Data.SqlClient
{
    /// <summary>
    /// 表示一个SQLSERVER数据表字段的描述信息
    /// </summary>
	public sealed class DbField
    {
        /// <summary>
        /// 字段名
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        [XmlAttribute]
        public string DataType { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        [XmlAttribute]
        public int Length { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public int scale { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public int Precision { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public bool Identity { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public bool Nullable { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public bool Computed { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public bool IsPersisted { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string DefaultValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string Formular { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public int SeedValue { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public int IncrementValue { get; set; }


        /// <summary>
        /// 是否为主键。
        /// 注意：此属性不由 ClownFish.Data 填充，仅供应用程序标记使用。
        /// </summary>
        [XmlAttribute]
        public bool IsPK { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Name;
        }
    }




}
