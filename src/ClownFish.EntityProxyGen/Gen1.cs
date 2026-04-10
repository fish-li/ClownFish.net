using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace ClownFish.EntityProxyGen;

// 参考链接： https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md

[Generator]
public class Gen1 : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        IncrementalValueProvider<bool> xmlRootConfig = context.AnalyzerConfigOptionsProvider.Select((options, ct) => {
            // 从 MSBuild 属性中读取
            options.GlobalOptions.TryGetValue("build_property.ClownFishEntityProxyGenEnableXmlRootAttribute", out var value);
            return bool.TryParse(value, out var result) && result;
        });

        // Add a single header with assembly attributes and using directives
        context.RegisterPostInitializationOutput(ctx => {
            string header = @"// ClownFish.Data dynamically generated code (source generator)
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
";
            ctx.AddSource("ClownFish_Data_Generator_Header.g.cs", SourceText.From(header, Encoding.UTF8));
        });

        var provider = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, ct) => IsCandidate(node),
                transform: (ctx, ct) => GetSymbol(ctx))
            .Where(symbol => symbol != null)
            .Combine(xmlRootConfig)
            .Collect();

        context.RegisterSourceOutput(provider, (spc, list) => Execute(spc, list));
    }

    private static bool IsCandidate(SyntaxNode node)
    {
        if( node is ClassDeclarationSyntax cds ) {
            if( cds.AttributeLists.Count > 0 )
                return true;
            if( cds.BaseList != null && cds.BaseList.Types.Count > 0 )
                return true;
        }
        return false;
    }

    private static INamedTypeSymbol? GetSymbol(GeneratorSyntaxContext ctx)
    {
        ClassDeclarationSyntax cds = (ClassDeclarationSyntax)ctx.Node;
        SemanticModel model = ctx.SemanticModel;
        INamedTypeSymbol? symbol = model.GetDeclaredSymbol(cds) as INamedTypeSymbol;
        if( symbol == null )
            return null;

        // Recognize entity types only by [DbEntity] attribute
        bool hasDbEntity = symbol.GetAttributes().Any(a => a.AttributeClass?.Name == "DbEntityAttribute" || a.AttributeClass?.Name == "DbEntity");

        if( hasDbEntity )
            return symbol;

        return null;
    }

    private static void Execute(SourceProductionContext spc, ImmutableArray<(INamedTypeSymbol? Symbol, bool EnableXmlRoot)> list)
    {
        if( list.IsDefaultOrEmpty )
            return;

#pragma warning disable RS1024 // 正确比较符号
        HashSet<ISymbol> seen = new HashSet<ISymbol>(SymbolEqualityComparer.Default);
#pragma warning restore RS1024 // 正确比较符号

        foreach( var item in list ) {

            INamedTypeSymbol? s = item.Symbol;
            if( s == null )
                continue;

            if( false == seen.Add(s) )
                continue;

            INamedTypeSymbol sym = (INamedTypeSymbol)s;

            try {
                string src = GenerateForEntity(sym, item.EnableXmlRoot);
                string hintName = GetHintName(sym);
                spc.AddSource(hintName, SourceText.From(src, Encoding.UTF8));
            }
            catch( Exception ex ) {
                var diag = Diagnostic.Create(new DiagnosticDescriptor(
                    id: "CFEPGEN001",
                    title: "Generator error",
                    messageFormat: "Exception while generating for '{0}': {1}",
                    category: "ClownFish.ProxyEntityGen",
                    DiagnosticSeverity.Warning,
                    isEnabledByDefault: true),
                    Location.None, sym?.ToDisplayString(), ex.Message);
                spc.ReportDiagnostic(diag);
            }
        }
    }

    private static string GetHintName(INamedTypeSymbol sym)
    {
        string full = sym.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        string md5 = GetMd5(full);
        return sym.Name + "_" + md5 + ".g.cs";
    }

    private static string GetMd5(string input)
    {
        using( MD5 md5 = MD5.Create() ) {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = md5.ComputeHash(bytes);
            //return Convert.ToHexString(hash);
            return BitConverter.ToString(hash).Replace("-", "");
        }
    }

    private sealed class DbColumnInfo
    {
        private bool _hasData = false;
        public bool HasData => _hasData;

        public string? Alias {
            get => field;
            set { _hasData = true; field = value; }
        }
        public bool PrimaryKey {
            get => field;
            set { _hasData = true; field = value; }
        }
        public bool Identity {
            get => field;
            set { _hasData = true; field = value; }
        }
        public bool Ignore { // support [DbColumn(Ignore=true)]
            get => field;
            set { _hasData = true; field = value; }
        }
    }
    private sealed class PropColumnKV
    {
#pragma warning disable CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。
        public /*required*/ int Index;
        public /*required*/ IPropertySymbol Property;
        public DbColumnInfo? ColInfo;
        public /*required*/ string DbName;
#pragma warning restore CS8618 // 在退出构造函数时，不可为 null 的字段必须包含非 null 值。请考虑添加 "required" 修饰符或声明为可为 null。

        public bool IsPrimary => this.ColInfo?.PrimaryKey ?? false;
    }


    private static DbColumnInfo? ParseDbColumnAttribute(AttributeData attr)
    {
        if( attr == null )
            return null;

        DbColumnInfo info = new DbColumnInfo();

        foreach( var na in attr.NamedArguments ) {
            string key = na.Key;
            TypedConstant val = na.Value;

            if( key == "Alias" && val.Value is string s && false == string.IsNullOrEmpty(s) )
                info.Alias = s;

            if( key == "PrimaryKey" && val.Value is bool b && b )
                info.PrimaryKey = true;

            if( key == "Identity" && val.Value is bool b2 && b2 )
                info.Identity = true;

            if( key == "Ignore" && val.Value is bool bi && bi )
                info.Ignore = true;

        }

        if( info.HasData == false )
            return null;

        // if alias not found in named args, check constructor args (positional)
        //if (string.IsNullOrEmpty(info.Alias) && attr.ConstructorArguments.Length > 0)
        //{
        //    var first = attr.ConstructorArguments[0];
        //    if (first.Kind == TypedConstantKind.Primitive && first.Value is string s && !string.IsNullOrEmpty(s))
        //        info.Alias = s;
        //}

        return info;
    }

    private static DbColumnInfo? ParseDbColumnAttributeFromSyntax(IPropertySymbol prop)
    {
        DbColumnInfo info = new DbColumnInfo();

        foreach( SyntaxReference r in prop.DeclaringSyntaxReferences ) {
            SyntaxNode node = r.GetSyntax();
            if( node is PropertyDeclarationSyntax pds ) {
                foreach( AttributeListSyntax al in pds.AttributeLists ) {
                    foreach( AttributeSyntax a in al.Attributes ) {
                        var name = a.Name.ToString();
                        if( name.EndsWith("DbColumn") || name.EndsWith("DbColumnAttribute") ) {
                            // inspect arguments
                            if( a.ArgumentList != null ) {
                                foreach( AttributeArgumentSyntax arg in a.ArgumentList.Arguments ) {
                                    if( arg.NameEquals != null ) {
                                        string id = arg.NameEquals.Name.Identifier.Text;
                                        string expr = arg.Expression.ToString().Trim();
                                        // strip quotes
                                        if( expr.Length >= 2 && expr[0] == '"' && expr[expr.Length-1] == '"' )
                                            expr = expr.Substring(1, expr.Length - 2);

                                        if( id == "Alias" )
                                            info.Alias = expr;
                                        if( id == "PrimaryKey" && bool.TryParse(expr, out var pb) && pb )
                                            info.PrimaryKey = true;
                                        if( id == "Identity" && bool.TryParse(expr, out var ib) && ib )
                                            info.Identity = true;
                                        if( id == "Ignore" && bool.TryParse(expr, out var ig) && ig )
                                            info.Ignore = true;
                                    }
                                    else {
                                        // positional argument, maybe alias
                                        string expr = arg.Expression.ToString().Trim();
                                        if( expr.Length >= 2 && expr[0] == '"' && expr[expr.Length-1] == '"' )
                                            expr = expr.Substring(1, expr.Length - 2);
                                        if( string.IsNullOrEmpty(info.Alias) )
                                            info.Alias = expr;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        if( info.HasData == false )
            return null;

        return info;
    }

    private static bool IsSupportedType(ITypeSymbol type)
    {
        if( type == null )
            return false;

        // Unwrap nullable
        if( type is INamedTypeSymbol ns && ns.IsGenericType && ns.ConstructedFrom?.ToDisplayString() == "System.Nullable<T>" )
            return IsSupportedType(ns.TypeArguments[0]);

        // byte[] special case
        if( type is IArrayTypeSymbol arr && arr.ElementType.SpecialType == SpecialType.System_Byte )
            return true;

        // primitive map (order chosen by user)
        switch( type.SpecialType ) {
            case SpecialType.System_Char:
            case SpecialType.System_Boolean:
            case SpecialType.System_Byte:
            case SpecialType.System_SByte:
            case SpecialType.System_Int16:
            case SpecialType.System_Int32:
            case SpecialType.System_Int64:
            case SpecialType.System_UInt16:
            case SpecialType.System_UInt32:
            case SpecialType.System_UInt64:
            case SpecialType.System_Single:
            case SpecialType.System_Double:
            case SpecialType.System_Decimal:
            case SpecialType.System_DateTime:
            case SpecialType.System_String:
                return true;
        }

        string fullname = type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
        if( fullname.Contains("System.Guid") || fullname.Contains("System.TimeSpan") )
            return true;

        // generic collections, arrays (except byte[]) and custom reference types are not supported unless DbColumn attribute present
        return false;
    }

    // 此阶段实体类型还没有产生，所以无法使用 CodeDom 生成代码
    //    private static string? GenerateByCodeDom(INamedTypeSymbol sym)
    //    {
    //        // Try to delegate to CodeDom generator when the runtime Type is available.
    //        try {
    //            string runtimeFullName = sym.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
    //            // find runtime Type in loaded assemblies
    //            Type? runtimeType = AppDomain.CurrentDomain.GetAssemblies()
    //                .Where(a => false == a.IsDynamic)
    //                .SelectMany(a => {
    //                    try { return a.GetTypes(); }
    //                    catch { return Array.Empty<Type>(); }
    //                })
    //                .FirstOrDefault(t => t.FullName == runtimeFullName);

    //            if( runtimeType != null ) {
    //                // find EntityGenerator type
    //                Type? genType = AppDomain.CurrentDomain.GetAssemblies()
    //                    .Select(a => {
    //                        try { return a.GetType("ClownFish.Data.CodeDom.EntityGenerator", false); }
    //                        catch { return null; }
    //                    })
    //                    .FirstOrDefault(t => t != null);

    //                if( genType != null ) {
    //                    object? genInstance = Activator.CreateInstance(genType);
    //                    if( genInstance != null ) {
    //                        // find generic method GetCode<T>()
    //                        var method = genType.GetMethods().FirstOrDefault(m => m.Name == "GetCode" && m.IsGenericMethodDefinition && m.GetParameters().Length == 0);
    //                        if( method != null ) {
    //                            var closed = method.MakeGenericMethod(runtimeType);
    //                            var result = closed.Invoke(genInstance, null) as string;
    //                            if( result != null ) {
    //                                // prepend the same header used by GenerateExpectedFilesByCodeDom
    //                                string codeHeader = @"// <auto-generated />
    //using System;
    //using System.Collections.Generic;
    //using System.Data;
    //using System.Data.Common;
    //using ClownFish.Data;
    //using ClownFish.Data.Internals;

    //";
    //                                return codeHeader + result;
    //                            }
    //                        }
    //                    }
    //                }
    //            }
    //        }
    //        catch {
    //            // ignore and fallback to existing generation
    //        }

    //        return null;
    //    }


    public static string GenerateForEntity(INamedTypeSymbol sym, bool enableXmlRoot)
    {
        string ns = "ClownFish_Data_GeneratorCode";
        string fullName = sym.ToDisplayString();
        string md5 = GetMd5(sym.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat));
        string proxyName = sym.Name + "_" + md5 + "_Proxy";
        string loaderName = sym.Name + "_" + md5 + "_Loader";

        // Collect properties that are readable and writable and not indexers
        IPropertySymbol[] props = sym.GetMembers().OfType<IPropertySymbol>()
            .Where(p => false == p.IsStatic && p.DeclaredAccessibility == Accessibility.Public)
            //  && p.GetMethod != null && p.SetMethod != null && p.Parameters.Length == 0)
            .ToArray();

        int propCount = props.Length;

        List<PropColumnKV> list = new List<PropColumnKV>(props.Length);

        int index = -1;
        // For each property determine db name and whether it's primary key or identity
        foreach( IPropertySymbol p in props ) {
            index++;

            if( p.GetMethod == null || p.SetMethod == null || p.Parameters.Length > 0 ) {
                // skip non-readwrite or indexer properties
                continue;
            }

            DbColumnInfo? info = null;

            // try semantic attribute data first
            AttributeData? dbAttr = p.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "DbColumnAttribute" || a.AttributeClass?.Name == "DbColumn");
            if( dbAttr != null ) {
                info = ParseDbColumnAttribute(dbAttr);
            }

            // fallback to syntax parsing if alias not found or pk/identity not found
            if( info == null && p.DeclaringSyntaxReferences.Length > 0 ) {
                info = ParseDbColumnAttributeFromSyntax(p);
            }

            if( info != null && info.Ignore ) {
                // skip ignored properties
                continue;
            }

            // exclude unsupported types when no DbColumn attribute
            if( dbAttr == null && false == IsSupportedType(p.Type) ) {
                continue;
            }

            string dbName = p.Name;
            if( info != null && false == string.IsNullOrEmpty(info.Alias) ) {
                dbName = info.Alias!;
            }

            PropColumnKV kv = new PropColumnKV {
                Index = index,
                Property = p,
                ColInfo = info,
                DbName = dbName,
            };
            list.Add(kv);
        }

        // Determine table name from [DbEntity(Alias=...)] or fallback to type name
        string tableName = sym.Name;
        AttributeData? entAttr = sym.GetAttributes().FirstOrDefault(a => a.AttributeClass?.Name == "DbEntityAttribute" || a.AttributeClass?.Name == "DbEntity");
        if( entAttr != null ) {
            foreach( var na in entAttr.NamedArguments ) {
                if( na.Key == "Alias" && na.Value.Value is string s && !string.IsNullOrEmpty(s) ) {
                    tableName = s;
                    break;
                }
            }

            // fallback syntax
            if( tableName == sym.Name && sym.DeclaringSyntaxReferences.Length > 0 ) {
                SyntaxNode node = sym.DeclaringSyntaxReferences[0].GetSyntax();
                if( node is ClassDeclarationSyntax cds ) {
                    foreach( AttributeListSyntax al in cds.AttributeLists )
                        foreach( AttributeSyntax a in al.Attributes ) {
                            var name = a.Name.ToString();
                            if( name.EndsWith("DbEntity") || name.EndsWith("DbEntityAttribute") ) {
                                if( a.ArgumentList != null ) {
                                    foreach( AttributeArgumentSyntax arg in a.ArgumentList.Arguments ) {
                                        if( arg.NameEquals != null && arg.NameEquals.Name.Identifier.Text == "Alias" ) {
                                            string expr = arg.Expression.ToString().Trim();
                                            if( expr.Length >= 2 && expr[0] == '"' && expr[expr.Length - 1] == '"' )
                                                expr = expr.Substring(1, expr.Length - 2);
                                            if( !string.IsNullOrEmpty(expr) )
                                                tableName = expr;
                                        }
                                    }
                                }
                            }
                        }
                }
            }
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("// <auto-generated />");
        sb.AppendLine("using System;");
        sb.AppendLine("using System.Collections.Generic;");
        sb.AppendLine("using System.Data;");
        sb.AppendLine("using System.Data.Common;");
        sb.AppendLine("using ClownFish.Data;");
        sb.AppendLine("using ClownFish.Data.Internals;");
        sb.AppendLine();
        sb.AppendLine($"namespace {ns} {{");

        // Proxy
        if( enableXmlRoot ) {
            sb.AppendLine($"    [XmlRoot(\"{sym.Name}\")]");
        }
        sb.AppendLine($"    public sealed class {proxyName} : {fullName}, IEntityProxy");
        sb.AppendLine("    {");

        sb.AppendLine($"        private bool[] _x_changeFlags = new bool[{propCount}];");
        sb.AppendLine($"        private {fullName} _x_realEntity;");
        sb.AppendLine("        private DbContext _context;\r\n");

        // IEntityProxy.Init
        sb.AppendLine("        void IEntityProxy.Init(DbContext dbContext, Entity entity)");
        sb.AppendLine("        {");
        sb.AppendLine("            _context = dbContext;");
        sb.AppendLine($"            _x_realEntity = ({fullName})entity;");
        sb.AppendLine("        }");
        sb.AppendLine();

        // DbContext and InnerEntity
        sb.AppendLine("        DbContext IEntityProxy.DbContext { get { return _context; } }");
        sb.AppendLine("        Entity IEntityProxy.InnerEntity { get { return _x_realEntity; } }");
        sb.AppendLine();

        // ClearChangeFlags
        sb.AppendLine("        void IEntityProxy.ClearChangeFlags()");
        sb.AppendLine("        {");
        sb.AppendLine("            for (int i = 0; i < _x_changeFlags.Length; i++) ");
        sb.AppendLine("                 _x_changeFlags[i] = false;");
        sb.AppendLine("        }");
        sb.AppendLine();

        // Properties: override if virtual
        sb.AppendLine("        #region Properties");
        foreach( var c in list ) {
            IPropertySymbol p = c.Property;
            bool isVirtual = p.IsVirtual || p.IsOverride;

            if( isVirtual ) {
                string typeName = p.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                sb.AppendLine();
                sb.AppendLine($"        public override {typeName} {p.Name} {{");
                sb.AppendLine($"            get {{ return _x_realEntity.{p.Name}; }}");
                sb.AppendLine($"            set {{ _x_changeFlags[{c.Index}] = true; _x_realEntity.{p.Name} = value; }}");
                sb.AppendLine("        }");
            }
            else {
                sb.AppendLine($"        // ignore {p.Name}");
            }
        }
        sb.AppendLine();
        sb.AppendLine("        #endregion");
        sb.AppendLine();

        // GetChangeNames
        sb.AppendLine("        IReadOnlyList<string> IEntityProxy.GetChangeNames()");
        sb.AppendLine("        {");
        sb.AppendLine($"            List<string> list = new List<string>({propCount});");
        foreach( var c in list ) {
            if( c.Property.IsVirtual || c.Property.IsOverride )
                sb.AppendLine($"            if (_x_changeFlags[{c.Index}]) list.Add(\"{c.DbName}\");");
        }
        sb.AppendLine("            return list;");
        sb.AppendLine("        }");
        sb.AppendLine();

        // GetChangeValues
        sb.AppendLine("        IReadOnlyList<object> IEntityProxy.GetChangeValues()");
        sb.AppendLine("        {");
        sb.AppendLine($"            List<object> list = new List<object>({propCount});");
        foreach( var c in list ) {
            IPropertySymbol p = c.Property;

            if( p.IsVirtual || p.IsOverride ) {
                // Handle nullable value types specially
                if( IsNullable(p.Type) ) {
                    // use HasValue/Value access pattern
                    sb.AppendLine($"     if (_x_changeFlags[{c.Index}]) {{  ");
                    sb.AppendLine($"          if (this.{p.Name}.HasValue) list.Add(this.{p.Name}.Value);");
                    sb.AppendLine($"          else list.Add(null);");
                    sb.AppendLine("      }");
                }
                else {
                    sb.AppendLine($"            if (_x_changeFlags[{c.Index}]) list.Add(this.{p.Name});");
                }
            }
        }
        sb.AppendLine("            return list;");
        sb.AppendLine("        }");
        sb.AppendLine();

        // GetRowKey
        sb.AppendLine("        FieldNvObject IEntityProxy.GetRowKey() { ");
        // Determine primary key column if any
        var pk = list.FirstOrDefault(c => c.IsPrimary);
        if( pk == null ) {
            sb.AppendLine("    throw new InvalidOperationException(\"实体没有属性被指定为 主键（ [DbColumn(PrimaryKey=true)] ），不能执行Update操作\");");
        }
        else {
            sb.AppendLine($"   return new FieldNvObject(\"{pk.DbName}\", this.{pk.Property.Name});");
        }
        sb.AppendLine("    }");

        // class end
        sb.AppendLine("    }");
        sb.AppendLine();

        // Loader class
        sb.AppendLine($"    [EntityAddition(ProxyType=typeof({proxyName}))]");
        sb.AppendLine($"    public sealed class {loaderName} : BaseDataLoader<{fullName}>, IDataLoader<{fullName}> ");
        sb.AppendLine("    {");
        sb.AppendLine($"        private static readonly Type s_entityType = typeof({fullName});");
        sb.AppendLine();

        // Create s_map
        sb.AppendLine("        private static readonly DataFieldMapKV[] s_map = new DataFieldMapKV[] { ");
        foreach( var c in list ) {
            sb.AppendLine($"            new DataFieldMapKV({c.Index}, \"{c.DbName}\"),");
        }
        sb.AppendLine("        };\r\n");

        // CreateIndex
        sb.AppendLine("        public override int[] CreateIndex(object dataSource) ");
        sb.AppendLine("{");
        sb.AppendLine($"             return DataReaderUtils.CreateNameMapIndex(dataSource, {propCount}, s_map);");
        sb.AppendLine("}");

        sb.AppendLine();

        // LoadFromDataReader
        sb.AppendLine($"        public override void LoadFromDataReader(DbDataReader reader, int[] cols, {fullName} m)");
        sb.AppendLine("        {");
        foreach( var c in list ) {
            IPropertySymbol p = c.Property;
            bool isNullable = IsNullable(p.Type);

            sb.AppendLine($"            if (cols[{c.Index}] >= 0) {{");

            if( p.Type.TypeKind == TypeKind.Enum ) {
                sb.AppendLine($"    m.{p.Name} = ({p.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)})DataReaderUtils.ToInt(reader, cols[{c.Index}], s_entityType, \"{p.Name}\");");
            }
            else if( isNullable && GetUnderlyingType(p.Type).TypeKind == TypeKind.Enum ) {
                string underlying = GetUnderlyingType(p.Type).ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                sb.AppendLine($"    int? val = DataReaderUtils.ToIntNull(reader, cols[{c.Index}], s_entityType, \"{p.Name}\");");
                sb.AppendLine($"   if( val.HasValue )  m.{p.Name} = ({underlying})(val.Value);");
            }
            else if( TryGetReaderMethod(p.Type, out string methodNonNull, out string methodNullable, out string useCastType) ) {
                if( isNullable ) {
                    sb.AppendLine($"                m.{p.Name} = {methodNullable}(reader, cols[{c.Index}], s_entityType, \"{p.Name}\");");
                }
                else {
                    sb.AppendLine($"                m.{p.Name} = {methodNonNull}(reader, cols[{c.Index}], s_entityType, \"{p.Name}\");");
                }
            }
            else {
                // fallback to DataFieldTypeHandlerFactory
                string tname = p.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                if( isNullable ) {
                    string underlying = GetUnderlyingType(p.Type).ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                    sb.AppendLine($"                m.{p.Name} = ({underlying})DataFieldTypeHandlerFactory.Get(typeof({underlying})).GetValue(reader, cols[{c.Index}], s_entityType, \"{p.Name}\");");
                }
                else {
                    sb.AppendLine($"                m.{p.Name} = ({tname})DataFieldTypeHandlerFactory.Get(typeof({tname})).GetValue(reader, cols[{c.Index}], s_entityType, \"{p.Name}\");");
                }
            }

            sb.AppendLine("            }");
        }
        sb.AppendLine("        }");
        sb.AppendLine();

        // LoadFromDataRow - similar but using DataTableUtils
        sb.AppendLine($"        public override void LoadFromDataRow(DataRow row, int[] cols, {fullName} m)");
        sb.AppendLine("        {");
        foreach( var c in list ) {
            IPropertySymbol p = c.Property;
            bool isNullable = IsNullable(p.Type);

            sb.AppendLine($"            if (cols[{c.Index}] >= 0) {{");

            if( p.Type.TypeKind == TypeKind.Enum ) {
                sb.AppendLine($"                m.{p.Name} = ({p.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat)})DataTableUtils.ToInt(row, cols[{c.Index}], s_entityType, \"{p.Name}\");");
            }
            else if( isNullable && GetUnderlyingType(p.Type).TypeKind == TypeKind.Enum ) {
                string underlying = GetUnderlyingType(p.Type).ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                sb.AppendLine($"  int? val = DataTableUtils.ToIntNull(row, cols[{c.Index}], s_entityType, \"{p.Name}\");");
                sb.AppendLine($"  if( val.HasValue )  m.{p.Name} = ({underlying})(val.Value);");
            }
            else if( TryGetRowMethod(p.Type, out string rMethodNonNull, out string rMethodNullable, out string rCastType) ) {
                if( isNullable ) {
                    sb.AppendLine($"                m.{p.Name} = {rMethodNullable}(row, cols[{c.Index}], s_entityType, \"{p.Name}\");");
                }
                else {
                    sb.AppendLine($"                m.{p.Name} = {rMethodNonNull}(row, cols[{c.Index}], s_entityType, \"{p.Name}\");");
                }
            }
            else {
                string tname = p.Type.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                if( isNullable ) {
                    string underlying = GetUnderlyingType(p.Type).ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                    sb.AppendLine($"                m.{p.Name} = ({underlying})DataFieldTypeHandlerFactory.Get(typeof({underlying})).GetValue(row, cols[{c.Index}], s_entityType, \"{p.Name}\");");
                }
                else {
                    sb.AppendLine($"                m.{p.Name} = ({tname})DataFieldTypeHandlerFactory.Get(typeof({tname})).GetValue(row, cols[{c.Index}], s_entityType, \"{p.Name}\");");
                }
            }

            sb.AppendLine("            }");
        }
        sb.AppendLine("        }");

        // end loader
        sb.AppendLine("    }");
        sb.AppendLine();

        sb.AppendLine("}");

        return sb.ToString();
    }

    // Try map common CLR types to DataReaderUtils methods. Return full invocation (e.g. DataReaderUtils.ToInt)
    private static bool TryGetReaderMethod(ITypeSymbol type, out string methodNonNull, out string methodNullable, out string castType)
    {
        methodNonNull = null!;
        methodNullable = null!;
        castType = null!;

        ITypeSymbol under = type;
        if( type is INamedTypeSymbol ns && ns.IsGenericType && ns.ConstructedFrom?.ToDisplayString() == "System.Nullable<T>" ) {
            under = ns.TypeArguments[0];
        }

        castType = under.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

        switch( under.SpecialType ) {
            case SpecialType.System_Char:
                methodNonNull = "DataReaderUtils.ToChar";
                methodNullable = "DataReaderUtils.ToCharNull";
                return true;
            case SpecialType.System_Boolean:
                methodNonNull = "DataReaderUtils.ToBool";
                methodNullable = "DataReaderUtils.ToBoolNull";
                return true;
            case SpecialType.System_Byte:
                methodNonNull = "DataReaderUtils.ToByte";
                methodNullable = "DataReaderUtils.ToByteNull";
                return true;
            case SpecialType.System_SByte:
                methodNonNull = "DataReaderUtils.ToSByte";
                methodNullable = "DataReaderUtils.ToSByteNull";
                return true;
            case SpecialType.System_Int16:
                methodNonNull = "DataReaderUtils.ToShort";
                methodNullable = "DataReaderUtils.ToShortNull";
                return true;
            case SpecialType.System_Int32:
                methodNonNull = "DataReaderUtils.ToInt";
                methodNullable = "DataReaderUtils.ToIntNull";
                return true;
            case SpecialType.System_Int64:
                methodNonNull = "DataReaderUtils.ToLong";
                methodNullable = "DataReaderUtils.ToLongNull";
                return true;
            case SpecialType.System_UInt16:
                methodNonNull = "DataReaderUtils.ToUShort";
                methodNullable = "DataReaderUtils.ToUShortNull";
                return true;
            case SpecialType.System_UInt32:
                methodNonNull = "DataReaderUtils.ToUInt";
                methodNullable = "DataReaderUtils.ToUIntNull";
                return true;
            case SpecialType.System_UInt64:
                methodNonNull = "DataReaderUtils.ToULong";
                methodNullable = "DataReaderUtils.ToULongNull";
                return true;
            case SpecialType.System_Single:
                methodNonNull = "DataReaderUtils.ToFloat";
                methodNullable = "DataReaderUtils.ToFloatNull";
                return true;
            case SpecialType.System_Double:
                methodNonNull = "DataReaderUtils.ToDouble";
                methodNullable = "DataReaderUtils.ToDoubleNull";
                return true;
            case SpecialType.System_Decimal:
                methodNonNull = "DataReaderUtils.ToDecimal";
                methodNullable = "DataReaderUtils.ToDecimalNull";
                return true;
            case SpecialType.System_DateTime:
                methodNonNull = "DataReaderUtils.ToDateTime";
                methodNullable = "DataReaderUtils.ToDateTimeNull";
                return true;
            case SpecialType.System_String:
                methodNonNull = "DataReaderUtils.ToString";
                methodNullable = "DataReaderUtils.ToString"; // string can be null
                return true;
            default:
                // handle types not in SpecialType (Guid, TimeSpan) by name
                string fullname = under.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                if( fullname.Contains("System.Guid") ) {
                    methodNonNull = "DataReaderUtils.ToGuid";
                    methodNullable = "DataReaderUtils.ToGuidNull";
                    return true;
                }
                if( fullname.Contains("System.TimeSpan") ) {
                    methodNonNull = "DataReaderUtils.ToTimeSpan";
                    methodNullable = "DataReaderUtils.ToTimeSpanNull";
                    return true;
                }
                // check for byte[]
                if( under is IArrayTypeSymbol arr && arr.ElementType.SpecialType == SpecialType.System_Byte ) {
                    methodNonNull = "DataReaderUtils.ToByteArray";
                    methodNullable = "DataReaderUtils.ToByteArray";
                    return true; // handle as special case
                }
                return false;
        }
    }

    private static bool TryGetRowMethod(ITypeSymbol type, out string methodNonNull, out string methodNullable, out string castType)
    {
        // Map to DataTableUtils methods similar to DataReaderUtils mapping
        methodNonNull = null!;
        methodNullable = null!;
        castType = null!;

        ITypeSymbol under = type;
        if( type is INamedTypeSymbol ns && ns.IsGenericType && ns.ConstructedFrom?.ToDisplayString() == "System.Nullable<T>" ) {
            under = ns.TypeArguments[0];
        }

        castType = under.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);

        switch( under.SpecialType ) {
            case SpecialType.System_Char:
                methodNonNull = "DataTableUtils.ToChar";
                methodNullable = "DataTableUtils.ToCharNull";
                return true;
            case SpecialType.System_Boolean:
                methodNonNull = "DataTableUtils.ToBool";
                methodNullable = "DataTableUtils.ToBoolNull";
                return true;
            case SpecialType.System_Byte:
                methodNonNull = "DataTableUtils.ToByte";
                methodNullable = "DataTableUtils.ToByteNull";
                return true;
            case SpecialType.System_SByte:
                methodNonNull = "DataTableUtils.ToSByte";
                methodNullable = "DataTableUtils.ToSByteNull";
                return true;
            case SpecialType.System_Int16:
                methodNonNull = "DataTableUtils.ToShort";
                methodNullable = "DataTableUtils.ToShortNull";
                return true;
            case SpecialType.System_Int32:
                methodNonNull = "DataTableUtils.ToInt";
                methodNullable = "DataTableUtils.ToIntNull";
                return true;
            case SpecialType.System_Int64:
                methodNonNull = "DataTableUtils.ToLong";
                methodNullable = "DataTableUtils.ToLongNull";
                return true;
            case SpecialType.System_UInt16:
                methodNonNull = "DataTableUtils.ToUShort";
                methodNullable = "DataTableUtils.ToUShortNull";
                return true;
            case SpecialType.System_UInt32:
                methodNonNull = "DataTableUtils.ToUInt";
                methodNullable = "DataTableUtils.ToUIntNull";
                return true;
            case SpecialType.System_UInt64:
                methodNonNull = "DataTableUtils.ToULong";
                methodNullable = "DataTableUtils.ToULongNull";
                return true;
            case SpecialType.System_Single:
                methodNonNull = "DataTableUtils.ToFloat";
                methodNullable = "DataTableUtils.ToFloatNull";
                return true;
            case SpecialType.System_Double:
                methodNonNull = "DataTableUtils.ToDouble";
                methodNullable = "DataTableUtils.ToDoubleNull";
                return true;
            case SpecialType.System_Decimal:
                methodNonNull = "DataTableUtils.ToDecimal";
                methodNullable = "DataTableUtils.ToDecimalNull";
                return true;
            case SpecialType.System_DateTime:
                methodNonNull = "DataTableUtils.ToDateTime";
                methodNullable = "DataTableUtils.ToDateTimeNull";
                return true;
            case SpecialType.System_String:
                methodNonNull = "DataTableUtils.ToString";
                methodNullable = "DataTableUtils.ToString";
                return true;
            default:
                string fullname = under.ToDisplayString(SymbolDisplayFormat.CSharpErrorMessageFormat);
                if( fullname.Contains("System.Guid") ) {
                    methodNonNull = "DataTableUtils.ToGuid";
                    methodNullable = "DataTableUtils.ToGuidNull";
                    return true;
                }
                if( fullname.Contains("System.TimeSpan") ) {
                    methodNonNull = "DataTableUtils.ToTimeSpan";
                    methodNullable = "DataTableUtils.ToTimeSpanNull";
                    return true;
                }
                if( under is IArrayTypeSymbol arr && arr.ElementType.SpecialType == SpecialType.System_Byte ) {
                    methodNonNull = "DataTableUtils.ToByteArray";
                    methodNullable = "DataTableUtils.ToByteArray";
                    return true;
                }
                return false;
        }
    }

    private static bool IsNullable(ITypeSymbol type)
    {
        return type is INamedTypeSymbol ns && ns.IsGenericType && ns.ConstructedFrom?.ToDisplayString() == "System.Nullable<T>";
    }

    private static ITypeSymbol GetUnderlyingType(ITypeSymbol type)
    {
        if( type is INamedTypeSymbol ns && ns.IsGenericType && ns.TypeArguments.Length == 1 )
            return ns.TypeArguments[0];
        return type;
    }
}


