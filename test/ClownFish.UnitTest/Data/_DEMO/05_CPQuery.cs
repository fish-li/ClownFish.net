using ClownFish.UnitTest.Data.Models;

namespace ClownFish.UnitTest.Data._DEMO;

class DEMO_5_CPQuery
{
    // CPQuery 是什么？ 有什么作用？
    // =================================================

    // 具体解释可参考：http://www.cnblogs.com/fish-li/archive/2012/09/10/CPQuery.html

    // 建议
    // 1、用 CPQuery 来代替【拼接SQL字符串】，防止SQL注入的安全漏洞。
    // 2、优先应该选择 XmlCommand


    // =================================================
    // 更多示例调用可参考：https://github.com/fish-li/ClownFish.net/blob/master/test/ClownFish.UnitTest.Data/CPQueryTest.cs



    public void 典型用法()
    {
        // 准备一条【参数化】SQL
        string sql = "select * from table1 where [time] >= @start and [time] < @end ";

        // 执行命令，获取实体列表
        var args = new { start = DateTime.Now.AddDays(-1), end = DateTime.Now };
        var list = CPQuery.Create(sql, args).ToList<Product>();
    }


    private void 根据界面输入条件执行动态SQL()
    {
        Product p = null;       //  来自于界面输入结果

        // 构造动态查询
        CPQuery query = BuildDynamicQuery(p);

        // 执行查询
        var list = query.ToList<Product>();
    }


    static CPQuery BuildDynamicQuery(Product p)
    {
        // 下面二行代码是等价的，可根据喜好选择。
        var query = "select ProductID, ProductName from Products where (1=1) ".AsCPQuery();
        //var query = CPQuery.New() + "select ProductID, ProductName from Products where (1=1) ";

        // 注意：下面的拼接代码中不能写成: query += .....

        if( p.ProductID > 0 )
            query = query + " and ProductID = " + p.ProductID;    // 整数参数。

        if( string.IsNullOrEmpty(p.ProductName) == false )
            // 给查询添加一个字符串参数。
            query = query + " and ProductName like " + p.ProductName.AsQueryParameter();

        if( p.CategoryID > 0 )
            query = query + " and CategoryID = " + p.CategoryID;    // 整数参数。

        if( string.IsNullOrEmpty(p.Unit) == false )
            query = query + " and Unit = " + (QueryParameter)p.Unit;    // 字符串参数

        if( p.UnitPrice > 0 )
            query = query + " and UnitPrice >= " + p.UnitPrice;    // decimal参数。

        if( p.Quantity > 0 )
            query = query + " and Quantity >= " + p.Quantity;    // 整数参数。

        return query;
    }



}
