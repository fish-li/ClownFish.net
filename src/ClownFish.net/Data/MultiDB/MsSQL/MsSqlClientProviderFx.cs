#if NETFRAMEWORK

namespace ClownFish.Data.MultiDB.MsSQL;

internal class MsSqlClientProvider : BaseMsSqlClientProvider
{
    public static readonly BaseClientProvider Instance = new MsSqlClientProvider();

    public override DbProviderFactory ProviderFactory => System.Data.SqlClient.SqlClientFactory.Instance;


    public override bool IsDuplicateInsertException(Exception ex)
    {
        if( ex is System.Data.SqlClient.SqlException sqlException ) {

            return sqlException.Number == 2601 || sqlException.Number == 2627;

            // select * from master.dbo.sysmessages where error = 2601
            // error	severity	dlevel	description	                                                        msglangid
            // 2601	    14	        0	    不能在具有唯一索引“%2!”的对象“%1!”中插入重复键的行。重复键值为 %3!。	2052
            // For example: 不能在具有唯一索引“IX_Table1_IntValue”的对象“dbo.Table1”中插入重复键的行。重复键值为 (31)。


            // select * from master.dbo.sysmessages where error = 2627
            // error	severity	dlevel	description	                                                        msglangid
            // 2601	    14	        0	    违反了 %1! 约束“%2!”。不能在对象“%3!”中插入重复键。重复键值为 %4!。 	2052
            // For example: 违反了 PRIMARY KEY 约束“PK_HtTypeGUID”。不能在对象“dbo.cb_HtType”中插入重复键。重复键值为 (c4e4facd-e4e3-4b04-9755-00cdbdc653e5)。
        }

        return false;
    }
}

#endif
