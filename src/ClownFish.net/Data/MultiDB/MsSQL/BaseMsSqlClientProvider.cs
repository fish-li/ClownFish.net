using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data.MultiDB.MsSQL;
internal abstract class BaseMsSqlClientProvider : BaseClientProvider
{
    public override DatabaseType DatabaseType => DatabaseType.SQLSERVER;

 
    /// <summary>
    /// 用于匹配SQL中的 order by 关键字
    /// </summary>
    private static readonly Regex s_regexOrderBy = new Regex(@"\border\s+by\b",
                                        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

    public override string GetObjectFullName(string symbol)
    {
        return "[" + symbol + "]";
    }


    public override CPQuery SetPagedQuery(CPQuery query, int skip, int take)
    {
        return query + $"\r\nOFFSET {skip} ROWS FETCH NEXT {take} ROWS ONLY";
    }

    /* SQLSERVER 的分页包装方式，只适用于 SQLSERVER2012 及后续版本
    原 SQL :
    select * from products
    where CategoryID = 1 and UnitPrice < 500
    order by ProductId

    变成二个新 SQL：

    select count(*) as totalrows from (
        select * from products
        where CategoryID = 1 and UnitPrice < 500
        --order by ProductId  -- 要把排序删除，否则会出现错误
    ) as txx

    //在SQLSERVER的分页处理中，做COUNT时，不能简单的套个 select count(*) 就完事，
    //SQLSERVER不允许在 count 语句中排序，因为这样的排序没有意义，所以有点麻烦。
    //所以，只好将 order by 后面的所有部分全部忽略

    select * from products
    where CategoryID = 1 and UnitPrice < 500
    order by ProductId
    OFFSET 0 ROWS FETCH NEXT 5 ROWS ONLY

    // 注意：当使用 SQLSERVER2012 的 OFFSET/FETCH 分页时，必须指定 ORDER BY，因为这二个关键字就是属于ORDER BY 子句的！
    */


    public override Page2Query GetPagedCommand(BaseCommand query, PagingInfo pagingInfo)
    {
        string srcSql = query.Command.CommandText;

        string listSql = $"{srcSql} \r\nOFFSET {pagingInfo.PageIndex * pagingInfo.PageSize} ROWS FETCH NEXT {pagingInfo.PageSize} ROWS ONLY";
        string countSql = null;

        if( pagingInfo.NeedCount ) {
            Match m = s_regexOrderBy.Match(srcSql);
            if( m.Success == false )
                throw new InvalidOperationException("SQL查询不符合要求，没有找到 'ORDER BY' 子句，请确保查询语句中包含 'ORDER BY'。");

            // 去掉 ORDER BY 子句
            // ORDER BY 通常来说，只也能放在SQL的后面，最后面还有可能会包含 OPTION 子句，这里不管了，删除！
            string srcSql2 = srcSql.Substring(0, m.Index);

            countSql = $"SELECT COUNT(*) AS totalrows FROM ( \r\n {srcSql2} \r\n ) as txx";
        }

        return StdClientProvider.CreatePageQuery(query, pagingInfo, listSql, countSql);
    }



    public override CPQuery GetNewIdQuery(CPQuery query, object entity)
    {
        return query + "; SELECT SCOPE_IDENTITY();";
    }


    public override Guid NewSeqGuid()
    {
        return GuidHelper.CreateSeqGuid(SequentialGuidType.AtEnd);
    }
}
