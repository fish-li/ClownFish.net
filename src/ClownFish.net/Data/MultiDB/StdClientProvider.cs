using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Data;

internal static class StdClientProvider
{
    public static CPQuery SetPagedQuery(CPQuery query, int skip, int take)
    {
        return query + $"\r\nLIMIT {take} OFFSET {skip}";
    }

    public static Page2Query GetPagedCommand(BaseCommand query, PagingInfo pagingInfo)
    {
        /* MySQL 的分页包装方式
        原 SQL :
        select * from products
        where CategoryID = 1 and UnitPrice < 500
        order by ProductId

        变成二个新 SQL：

        select count(*) as totalrows from (
            select * from products
            where CategoryID = 1 and UnitPrice < 500
            order by ProductId
        ) as txx


        select * from products
        where CategoryID = 1 and UnitPrice < 500
        order by ProductId
        LIMIT rows OFFSET offset
        */

        // 说明：以上实现是一个简单偷懒的做法，
        // 核心想法是：在【不修改】调用者提供的SQL前提下，对原SQL添加分页必需的操作来实现分页查询。
        // 为什么是【不修改】原SQL ?   因为：有些SQL可能非常复杂，请继续看完……

        // 上面的示例中，你会发现在计算【总数】时，把 order by 也包含进去了，
        // 看起来确实是个【不需要的操作】，也可能会对性能产生不确定的影响（尤其是在没有索引的时候）
        // 但是在SQLSERVER的实现中又做了特殊处理，把 order by “删除” 了，具体原因在那边代码的注释中有解释，可以自己去看 ！

        // 想 “完美干净” 地去掉 order by，其实也不简单，可以自己去了解下各个数据库对SELECT语句的定义，
        // 直接用 “关键字定位” 的方法去除，有一定的风险，因为以后数据库再添加语法放在 order by 后面呢？

        // 再说说性能影响，如果原SQL在 SELECT 子名中包含了复杂的“计算列”，甚至包含了“子查询”，那么生成的计算总数SQL肯定是 “低效”的，即使去掉orderby
        // 所以，如果 ToPageList 这些方法如果因为这些问题导致性能低下，请不要使用这些方法，自己动手实现2个SQL !

        // 科普下：分页其实是由2个SQL查询构成的：1，查列表--LIST查询，2，计算总数--COUNT查询
        // 在性能要求较高时，可以自己提供性能较好的 COUNT 查询，例如，简化SELECT，不指定orderby

        string srcSql = query.Command.CommandText;

        string listSql = $"{srcSql} \r\nLIMIT {pagingInfo.PageSize} OFFSET {pagingInfo.PageIndex * pagingInfo.PageSize}";

        string countSql = pagingInfo.NeedCount
                                ? $"SELECT COUNT(*) AS totalrows FROM ( \r\n {srcSql} \r\n ) as txx"
                                : null;

        return CreatePageQuery(query, pagingInfo, listSql, countSql);
    }


    /// <summary>
    /// 根据指定的参数生成 Page2Query 结构
    /// </summary>
    /// <param name="query"></param>
    /// <param name="pagingInfo"></param>
    /// <param name="listSql"></param>
    /// <param name="countSql"></param>
    /// <returns></returns>
    internal static Page2Query CreatePageQuery(BaseCommand query, PagingInfo pagingInfo, string listSql, string countSql)
    {
        if( query == null )
            throw new ArgumentNullException(nameof(query));
        if( pagingInfo == null )
            throw new ArgumentNullException(nameof(pagingInfo));

        DbContext dbContext = query.Context;
        DbCommand command = query.Command;


        // 获取当前命令的参数集合，给第一个CPQuery使用
        DbParameter[] parameters1 = command.Parameters.Cast<DbParameter>().ToArray();

        // 断开参数对象与原命令的关联。
        // 如果不断开，直接将参数添加到其它命令时，会引发异常。
        command.Parameters.Clear();

        // 创建新的 “列表查询” 
        CPQuery listQuery = dbContext.CPQuery.Create(listSql, parameters1);


        if( pagingInfo.NeedCount ) {

            // 克隆参数数组，因为参数对象只能属于一个命令对象。
            DbParameter[] parameters2 = listQuery.Command.CloneParameters();

            // 创建新的 “Count查询”
            CPQuery countQuery = dbContext.CPQuery.Create(countSql, parameters2);
            return new Page2Query(listQuery, countQuery);
        }
        else {
            return new Page2Query(listQuery, null);
        }
    }
}
