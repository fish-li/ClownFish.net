namespace ClownFish.Data.CodeDom;

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
using ClownFish.Data.Internals;

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
[assembly: System.Runtime.InteropServices.Guid(""92d5cd01-460f-4002-9ab0-2533d7a24153"")]
[assembly: System.Reflection.AssemblyVersion(""1.0.0.0"")]
[assembly: System.Reflection.AssemblyFileVersion(""1.{DateTime.Now.ToString("yyyy")}.{DateTime.Now.ToString("Mdd")}.{DateTime.Now.ToString("Hmm")}"")]

";

    /// <summary>
    /// 加载程序，确保 UsingCodeBlock 字段中引用的命令空间都是有效的
    /// </summary>
    internal static List<Type> LoadAssembly()
    {
        List<Type> list = new List<Type>();
        list.Add(typeof(System.Collections.Generic.List<string>));
        list.Add(typeof(System.Data.DataSet));
        list.Add(typeof(System.Data.Common.DbCommand));
        list.Add(typeof(System.Xml.Serialization.XmlAttributeAttribute));
        list.Add(typeof(System.Linq.EnumerableQuery));

        return list;
    }

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
    private string _primaryKeyDbName;
    /// <summary>
    /// 实体主键的.NET属性名
    /// </summary>
    private string _primaryKeyNetName;


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
    private readonly StringBuilder _code = new StringBuilder(1024 * 2);

    private string GetMd5(string input)
    {
        return HashHelper.Md5(input);
    }

    private void Init()
    {
        _typeInfo = EntityDescriptionCache.Get(_entityType);

        // 为了解决类型在不同时间，不同机器生成的代理类型名称一致，生成规则：原类名 + md5(原类型全名) + "Proxy"
        // 为了防止类型名称重复，新的类名：原类名（不含命名空间） + 全局的实体类型累加数量 + "Proxy"
        string md5 = "_" + GetMd5(_entityType.FullName);

        ProxyClassName = _entityType.Name + md5 + "_Proxy";
        DataLoaderClassName = _entityType.Name + md5 + "_Loader";

        try {
            var column = _typeInfo.MemberDict
                                    .Where(x => x.Value.Attr != null && x.Value.Attr.PrimaryKey)
                                    .SingleOrDefault().Value;

            _primaryKeyNetName = column?.PropertyInfo.Name;
            _primaryKeyDbName = column?.DbName;
        }
        catch( InvalidOperationException ex ) {
            string message = $"实体类型 {_entityType.FullName} 定义了多个 [DbColumn(PrimaryKey=true)]";
            throw new NotSupportedException(message, ex);
        }
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
        //WriteChangeFlags();
        WriteClassEnd();



        WriteLoaderHeader();
        WriteCreateIndex();

        WriteLoadFromDataX("LoadFromDataReader", "DbDataReader", "reader", "DataReaderUtils");
        WriteLoadFromDataX("LoadFromDataRow", "DataRow", "row", "DataTableUtils");

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
        //if( _entityType.IsSerializable )   // 不再支持二进制序列化
        //    _code.AppendLineRN("[Serializable]");

        _code
            .Append($"[XmlRoot(\"{_entityType.Name}\")]\r\n")
            .AppendFormat("public sealed class {0} : {1}, IEntityProxy\r\n", ProxyClassName, _entityType.FullName)
            .Append("{\r\n");
    }

    private void WriteLoaderHeader()
    {
        _code.AppendFormat("[EntityAddition(ProxyType=typeof({0}))]\r\n", ProxyClassName)            
            .AppendFormat("public sealed class {0} : BaseDataLoader<{1}>, IDataLoader<{1}>\r\n", DataLoaderClassName, _entityType.FullName)
            .Append("{\r\n")
            .Append($"private static readonly Type s_entityType = typeof({_entityType.FullName});\r\n");
    }

    private void WriteProxyFields()
    {
        _code.AppendFormat(@"
private bool[] _x_changeFlags = new bool[{0}];
private {1} _x_realEntity;
private DbContext _context;

void IEntityProxy.Init(DbContext dbContext, Entity entity) 
{{
    _context = dbContext;
	_x_realEntity = ({1})entity;
}}

DbContext IEntityProxy.DbContext {{ get {{ return _context; }} }}
Entity IEntityProxy.InnerEntity {{ get {{ return _x_realEntity; }}  }}

void IEntityProxy.ClearChangeFlags()
{{
	for( int i = 0; i < _x_changeFlags.Length; i++ )
		_x_changeFlags[i] = false;
}}
", _typeInfo.PropertyCount, _entityType.FullName);
    }


    private void WriteProperties()
    {
        _code.AppendLineRN("#region Properties");

        // 输出每个【重写】属性的代码
        foreach( var kvp in _typeInfo.MemberDict ) {
            ColumnInfo column = kvp.Value;
            if( column.PropertyInfo.IsVirtual() ) {
                _code.AppendFormat(@"
public override {0} {1} {{
get {{return _x_realEntity.{1};}}
set {{_x_changeFlags[{2}] = true; _x_realEntity.{1} = value;}}
}}", column.PropertyInfo.PropertyType.ToTypeString(), column.PropertyInfo.Name, column.Index);
            }
            else {
                _code.AppendFormat("\r\n// ignore {0}", column.PropertyInfo.Name);
            }
        }

        _code.AppendLineRN("\r\n#endregion");
    }



    private void WriteGetChangeNames()
    {
        _code.AppendFormat(@"
IReadOnlyList<string> IEntityProxy.GetChangeNames()
{{
List<string> list = new List<string>( {0} );
", _typeInfo.PropertyCount);

        foreach( var kvp in _typeInfo.MemberDict ) {
            ColumnInfo column = kvp.Value;
            if( column.PropertyInfo.IsVirtual() ) {
                _code.AppendFormat(@"if( _x_changeFlags[{0}] ) list.Add(""{1}"");
", column.Index, column.DbName);
            }
        }

        _code.Append(@"return list;
}");
    }

    private void WriteGetChangeValues()
    {
        _code.AppendFormat(@"
IReadOnlyList<object> IEntityProxy.GetChangeValues()
{{
List<object> list = new List<object>( {0} );
"
, _typeInfo.PropertyCount);


        foreach( var kvp in _typeInfo.MemberDict ) {
            ColumnInfo column = kvp.Value;
            if( column.PropertyInfo.IsVirtual() ) {

                // 判断是不是可空类型，例如：int?  long?
                if( column.PropertyInfo.PropertyType.IsNullableType() ) {
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

        _code.Append(@"return list;
}");
    }


    private void WriteGetRowKey()
    {
        if( _primaryKeyNetName == null )
            _code.Append(@"
FieldNvObject IEntityProxy.GetRowKey(){
	throw new InvalidOperationException(""实体没有属性被指定为 主键（ [DbColumn(PrimaryKey=true)] ），不能执行Update操作"");
}
");

        else

            _code.AppendFormat(@"
FieldNvObject IEntityProxy.GetRowKey(){{
	return new FieldNvObject(""{0}"", this.{1});
}}
"
, _primaryKeyDbName, _primaryKeyNetName);
    }

//    private void WriteChangeFlags()
//    {
//        // 这个属性要放在最后申明，用于保证序列化出现在最后面，
//        // 可以避免反序列化过程中属性赋值把这个数组的值给修改了。
//        _code.Append(@"
//[XmlAttribute]
//public bool[] XEntityDataChangeFlags { get { return _x_changeFlags; }  set { _x_changeFlags = value; } }
//");
//    }


    //		private void WriteCreateIndex()
    //        {
    //            _code.AppendLineRN("private static readonly (int, string)[] s_map = new (int, string)[] {");

    //            foreach( var kvp in _typeInfo.MemberDict ) {
    //                ColumnInfo column = kvp.Value;
    //                _code.AppendLineRN($"({column.Index}, \"{column.DbName}\"),");
    //            }
    //            _code.AppendLineRN("};");

    //            _code.AppendLineRN($@"
    //public override int[] CreateIndex(object dataSource)
    //{{
    //return DataReaderUtils.CreateNameMapIndex(dataSource, {_typeInfo.PropertyCount}, s_map);
    //}}");
    //		}

    private void WriteCreateIndex()
    {
        _code.AppendLineRN("private static readonly DataFieldMapKV[] s_map = new DataFieldMapKV[] {");

        foreach( var kvp in _typeInfo.MemberDict ) {
            ColumnInfo column = kvp.Value;
            _code.AppendLineRN($"new DataFieldMapKV({column.Index}, \"{column.DbName}\"),");
        }
        _code.AppendLineRN("};");

        _code.AppendLineRN($@"
public override int[] CreateIndex(object dataSource)
{{
return DataReaderUtils.CreateNameMapIndex(dataSource, {_typeInfo.PropertyCount}, s_map);
}}");
    }


    private void WriteLoadFromDataX(string method, string varType, string var, string utils)
    {
        // 样例1：public override void LoadFromDataReader(DbDataReader reader, int[] cols, ClownFish.UnitTest.Data.PostgreSQL.YkfKbase m)
        // 样例2：public override void LoadFromDataRow(DataRow row, int[] cols, ClownFish.UnitTest.Data.PostgreSQL.YkfKbase m)

        _code.AppendLineRN($"public override void {method}({varType} {var}, int[] cols, {_entityType.FullName} m)");
        _code.AppendLineRN("{");

        foreach( var kvp in _typeInfo.MemberDict ) {
            ColumnInfo column = kvp.Value;
            PropertyInfo p = column.PropertyInfo;

            _code.AppendLineRN($"if (cols[{column.Index}] >= 0){{");

            if( p.PropertyType.IsEnum ) {
                _code.AppendLineRN($"m.{p.Name} = ({column.DataType.ToTypeString()}){utils}.ToInt({var}, cols[{column.Index}], s_entityType, \"{p.Name}\");");
            }
            else if( p.PropertyType.IsNullableEnum() ) {
                _code.AppendLineRN($@"int? val = {utils}.ToIntNull({var}, cols[{column.Index}], s_entityType, ""{p.Name}"");
if( val.HasValue )  m.{p.Name} = ({column.DataType.ToTypeString()})(val.Value);");
            }
            else {
                string typeName = TypeList.GetTypeName(p.PropertyType);
                if( typeName.HasValue() ) {
                    string methodName = TypeList.GetMethodName(typeName);
                    _code.AppendLineRN($"m.{p.Name} = {utils}.{methodName}({var}, cols[{column.Index}], s_entityType, \"{p.Name}\");");
                }
                else {
                    // 很有可能是自定义的数据类型
                    _code.AppendLineRN($"m.{p.Name} = ({p.PropertyType.ToTypeString()})DataFieldTypeHandlerFactory.Get(typeof({p.PropertyType.ToTypeString()})).GetValue({var}, cols[{column.Index}], s_entityType, \"{p.Name}\");");
                }
            }

            _code.AppendLineRN("}");
        }

        _code.AppendLineRN("}");
    }


    private void WriteClassEnd()
    {
        _code.AppendLineRN("}");
    }

    private void WriteEnd()
    {
        _code.AppendLineRN("}");
    }
}
