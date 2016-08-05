using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base.Reflection;
using ClownFish.Data;

namespace ClownFish.Data.CodeDom
{
	/// <summary>
	/// 实体代理类型的代码生成器
	/// </summary>
	public sealed class EntityGenerator
	{
        /// <summary>
        /// 默认的代码文件头上的 using 语句块
        /// </summary>
        public static readonly string UsingCodeBlock = $@"
// ClownFish.Data dynamically generate code
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Xml.Serialization;
using System.Linq;
using System.Text;
using ClownFish.Data;

[assembly: ClownFish.Data.EntityProxyAssembly]
[assembly: System.Reflection.AssemblyTitle(""ClownFish.Data.ProxyGen"")]
[assembly: System.Reflection.AssemblyDescription("""")]
[assembly: System.Reflection.AssemblyConfiguration("""")]
[assembly: System.Reflection.AssemblyCompany("""")]
[assembly: System.Reflection.AssemblyProduct(""ClownFish.Data.ProxyGen"")]
[assembly: System.Reflection.AssemblyCopyright(""Copyright © ClownFish 2016"")]
[assembly: System.Reflection.AssemblyTrademark("""")]
[assembly: System.Reflection.AssemblyCulture("""")]
[assembly: System.Runtime.InteropServices.ComVisible(false)]
[assembly: System.Runtime.InteropServices.Guid(""{Guid.NewGuid().ToString()}"")]
[assembly: System.Reflection.AssemblyVersion(""1.0.0.0"")]
[assembly: System.Reflection.AssemblyFileVersion(""1.{DateTime.Now.ToString("yyyy")}.{DateTime.Now.ToString("Mdd")}.{DateTime.Now.ToString("Hmm")}"")]

";
        /// <summary>
        /// 所有动态生成的实体类型的命名空间
        /// </summary>
		public readonly string NameSpace = "ClownFish.Data.GeneratorCode";

        
        /// <summary>
        /// 当前处理的实体类型
        /// </summary>
        private Type _entityType;
		/// <summary>
		/// 实体的描述信息
		/// </summary>
		private EntityDescription _typeInfo;

		/// <summary>
		/// 实体主键的数据库字段名
		/// </summary>
        private string _PrimaryKeyDbName;
		/// <summary>
		/// 实体主键的.NET属性名
		/// </summary>
        private string _PrimaryKeyNetName;


        /// <summary>
        /// 代理类的名称
        /// </summary>
        public string ProxyClassName { get; private set; }

		/// <summary>
		/// 数据加载类型的名称
		/// </summary>
		public string DataLoaderClassName { get; private set; }

		/// <summary>
		/// 用于拼接C#代码的StringBuilder实例
		/// </summary>
		private StringBuilder _code = new StringBuilder(2048);

		private string GetMd5(string input)
		{
			byte[] bb = (new MD5CryptoServiceProvider()).ComputeHash(Encoding.UTF8.GetBytes(input));
			return BitConverter.ToString(bb).Replace("-", "");
		}

		private void Init()
        {
            _typeInfo = EntityDescriptionCache.Create(_entityType, false);
            _typeInfo.Attr = _entityType.GetMyAttribute<DbEntityAttribute>();

            // 为了解决类型在不同时间，不同机器生成的代理类型名称一致，生成规则：原类名 + md5(原类型全名) + "Proxy"
            // 为了防止类型名称重复，新的类名：原类名（不含命名空间） + 全局的实体类型累加数量 + "Proxy"
            string md5 = "_" + GetMd5(_entityType.FullName);

			ProxyClassName = _entityType.Name + md5 + "_Proxy";
			DataLoaderClassName = _entityType.Name + md5 + "_Loader";


			var column = _typeInfo.MemberDict
									.Where(x => x.Value.Attr != null && x.Value.Attr.PrimaryKey)
									.SingleOrDefault().Value;

            _PrimaryKeyNetName = column?.PropertyInfo.Name;
            _PrimaryKeyDbName = column?.DbName;
        }


        /// <summary>
        /// 生成实体的代理类型代码，将用于编译成代理类型
        /// </summary>
        /// <returns></returns>
		public string GetCode<T>() where T : Entity, new()
        {
            return GetCode(typeof(T));
        }
        
        internal string GetCode(Type t)
        {
            _entityType = t;
            Init();

            WriteHeader();

			WriteProxyHeader();
			WriteProxyFields();
			WriteProperties();
            

			WriteGetChangeNames();
			WriteGetChangeValues();			
			WriteGetRowKey();
			WriteChangeFlags();
			WriteClassEnd();



			WriteLoaderHeader();
            WriteCreateIndex();
            WriteLoadFromDataReader();
            WriteLoadFromDataRow();

			WriteClassEnd();

			WriteEnd();

            return _code.ToString();
        }

		private void WriteHeader()
		{
			_code.Append("namespace ").Append(NameSpace).Append("{\r\n");
		}

		private void WriteProxyHeader()
		{
			if( _entityType.IsSerializable )
				_code.AppendLine("[Serializable]");

			_code
				.AppendFormat("public sealed class {0} : {1}, IEntityProxy\r\n", ProxyClassName, _entityType.FullName)
				.Append("{\r\n");
		}

		private void WriteLoaderHeader()
		{
			_code.AppendFormat("[EntityAddition(ProxyType=typeof({0}))]\r\n", ProxyClassName)
				.AppendFormat("public sealed class {0} : BaseDataLoader<{1}>, IDataLoader<{1}>\r\n", DataLoaderClassName, _entityType.FullName)
				.Append("{\r\n");
		}

		private void WriteProxyFields()
		{
            _code.AppendFormat(@"
private bool[] _x_changeFlags = new bool[{0}];
private {1} _x_realEntity;

Entity IEntityProxy.RealEntity {{ get {{ return _x_realEntity; }}  }}
private {1} _X__Entity {{
	get {{
		if( _x_realEntity == null ) _x_realEntity = new {1}();
		return _x_realEntity;
	}}
}}

void IEntityProxy.Init(Entity entity) 
{{
	_x_realEntity = ({1})entity;
}}

void IEntityProxy.ClearChangeFlags()
{{
	for( int i = 0; i < _x_changeFlags.Length; i++ )
		_x_changeFlags[i] = false;
}}
", _typeInfo.PropertyCount, _entityType.FullName);
        }


		private void WriteProperties()
		{
            _code.AppendLine("#region Properties");

            // 输出每个【重写】属性的代码
            foreach(var kvp in _typeInfo.MemberDict) {
                ColumnInfo column = kvp.Value;
                if(column.PropertyInfo.IsVirtual()) {
                    _code.AppendFormat(@"
public override {0} {1} {{
get {{return _x_realEntity.{1};}}
set {{_x_changeFlags[{2}] = true; _X__Entity.{1} = value;}}
}}", column.PropertyInfo.PropertyType.ToTypeString(), column.PropertyInfo.Name, column.Index);
                }
                else {
                    _code.AppendFormat("\r\n// ignore {0}", column.PropertyInfo.Name);
                }
            }

            _code.AppendLine("\r\n#endregion");
        }



		private void WriteGetChangeNames()
		{
            _code.AppendFormat(@"
string[] IEntityProxy.GetChangeNames()
{{
List<string> list = new List<string>( {0} );
", _typeInfo.PropertyCount);

            foreach (var kvp in _typeInfo.MemberDict) {
                ColumnInfo column = kvp.Value;                
                if(column.PropertyInfo.IsVirtual()) {
					_code.AppendFormat(@"if( _x_changeFlags[{0}] ) list.Add(""{1}"");
", column.Index, column.DbName);
                }
            }

            _code.Append(@"return list.ToArray();
}");
        }

		private void WriteGetChangeValues()
		{
			_code.AppendFormat(@"
object[] IEntityProxy.GetChangeValues()
{{
List<object> list = new List<object>( {0} );
"
, _typeInfo.PropertyCount);


			foreach (var kvp in _typeInfo.MemberDict) {
                ColumnInfo column = kvp.Value;
                if (column.PropertyInfo.IsVirtual()) {

                    // 这里的泛型参数就认为是可空类型，因为不允许其它的泛型数据类型
                    if (column.PropertyInfo.PropertyType.IsGenericType) {
                        _code.AppendFormat(@"
if( _x_changeFlags[{0}] ) {{
	if( this.{1}.HasValue )	list.Add(this.{1}.Value);
	else	list.Add(null);
}}
", column.Index, column.PropertyInfo.Name);
                    }
                    else {
						// 虚属性（有状态标记），非泛型
						_code.AppendFormat(@"if( _x_changeFlags[{0}] ) list.Add(this.{1});
", column.Index, column.PropertyInfo.Name);
                    }
                }
            }

            _code.Append(@"return list.ToArray();
}");
        }
		

		private void WriteGetRowKey()
		{
			if( _PrimaryKeyNetName == null )
				_code.Append(@"
Tuple<string, object> IEntityProxy.GetRowKey(){
	throw new InvalidOperationException(""实体没有属性被指定为【唯一】主键（ [DbColumn(PrimaryKey=true)] ），不能执行Update操作"");
}
");

			else

			_code.AppendFormat(@"
Tuple<string, object> IEntityProxy.GetRowKey(){{
	return new Tuple<string, object>(""{0}"", this.{1});
}}
"
, _PrimaryKeyDbName, _PrimaryKeyNetName);
		}

		private void WriteChangeFlags()
		{
			// 这个属性要放在最后申明，用于保证序列化出现在最后面，
			// 可以避免反序列化过程中属性赋值把这个数组的值给修改了。
			_code.Append(@"
[XmlAttribute]
public bool[] XEntityDataChangeFlags { get { return _x_changeFlags; }  set { _x_changeFlags = value; } }
");
		}


		private void WriteCreateIndex()
        {
            _code.AppendFormat(@"
public override int[] CreateIndex(object dataSource)
{{return DataLoaderHelper.CreateNameMapIndex(dataSource, {0}
", _typeInfo.PropertyCount);

			foreach( var kvp in _typeInfo.MemberDict ) {
				ColumnInfo column = kvp.Value;
				_code.AppendFormat(",new KeyValuePair<int, string>({0}, \"{1}\")\r\n"
							, column.Index, column.DbName);
			}
			_code.AppendLine(");}");
		}

        private string GetReaderMethodName(Type t)
        {
            if (t == TypeList._int)
                return "reader.GetInt32";
            if (t == TypeList._string)
                return "reader.GetString";
            if (t == TypeList._DateTime)
                return "reader.GetDateTime";
            if (t == TypeList._decimal)
                return "reader.GetDecimal";
            if (t == TypeList._bool)
                return "reader.GetBoolean";
            if (t == TypeList._long)
                return "reader.GetInt64";
            if (t == TypeList._short)
                return "reader.GetInt16";
            if (t == TypeList._float)
                return "reader.GetFloat";
            if (t == TypeList._double)
                return "reader.GetDouble";
            if (t == TypeList._Guid)
                return "reader.GetGuid";
            if (t == TypeList._byte)
                return "reader.GetByte";
            //if( t == TypeList._char )
            //    return "reader.GetChar";

            return string.Format("({0})reader.GetValue", t.ToTypeString());
        }

        private void WriteLoadFromDataReader()
        {
            _code.AppendFormat("public override void LoadFromDataReader(DbDataReader reader, int[] colIndex, {0} m)\r\n", _entityType.FullName);
            _code.AppendLine("{");

            foreach (var kvp in _typeInfo.MemberDict) {
                ColumnInfo column = kvp.Value;
                _code.AppendFormat("if (colIndex[{0}] >= 0){{\r\n", column.Index);

                if( column.PropertyInfo.PropertyType == TypeList._char) {
                    _code.AppendFormat("m.{0} = reader.GetString(colIndex[{1}])[0];\r\n",
                        column.PropertyInfo.Name,
                        column.Index);
                }
                else if(column.PropertyInfo.PropertyType == TypeList._char_null) {
                    _code.AppendFormat(@"
object val = reader.GetValue(colIndex[{0}]);
if (val != null && DBNull.Value.Equals(val) == false) {{
	string str = val.ToString();
	if (str.Length > 0)
		m.{1} = str[0];
}}
", column.Index, column.PropertyInfo.Name);
                }
                else if( column.PropertyInfo.PropertyType.IsValueType 
                    && column.PropertyInfo.PropertyType.IsGenericType == false) {
                    _code.AppendFormat("m.{0} = {1}(colIndex[{2}]);\r\n",
                        column.PropertyInfo.Name, 
                        GetReaderMethodName(column.PropertyInfo.PropertyType),
                        column.Index
                        );
                }
                else {
                    _code.AppendFormat(@"
object val = reader.GetValue(colIndex[{0}]);
if (val != null && DBNull.Value.Equals(val) == false)
	m.{1} = ({2})(val);
", column.Index, column.PropertyInfo.Name, column.DataType.ToTypeString());
                }
                
                _code.AppendLine("}");
            }

            _code.AppendLine("}");
        }

        private void WriteLoadFromDataRow()
        {
            _code.AppendFormat("public override void LoadFromDataRow(DataRow row, int[] colIndex, {0} m)\r\n", _entityType.FullName);
            _code.AppendLine("{");

            foreach (var kvp in _typeInfo.MemberDict) {
                ColumnInfo column = kvp.Value;
                _code.AppendFormat("if (colIndex[{0}] >= 0){{\r\n", column.Index);

                if (column.PropertyInfo.PropertyType.IsValueType
                    && column.PropertyInfo.PropertyType.IsGenericType == false) {
                    _code.AppendFormat("m.{0} = ({1})(row[colIndex[{2}]]);\r\n",
                        column.PropertyInfo.Name,
                        column.DataType.ToTypeString(),
                        column.Index
                        );
                }
                else {
                    _code.AppendFormat(@"
object val = row[colIndex[{0}]];
if (val != null && DBNull.Value.Equals(val) == false)
	m.{1} = ({2})(val);
", column.Index, column.PropertyInfo.Name, column.DataType.ToTypeString());
                }

                _code.AppendLine("}");
            }

            _code.AppendLine("}");
        }
		

		private void WriteClassEnd()
		{
            _code.Append(@"}
");
		}

		private void WriteEnd()
		{
			_code.Append("}");
		}
	}
}
