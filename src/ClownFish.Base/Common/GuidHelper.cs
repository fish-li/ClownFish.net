using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClownFish.Base
{
    /// <summary>
    /// Guid扩展方法类
    /// </summary>
    public static class GuidHelper
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1060")]
        [DllImport("rpcrt4.dll", SetLastError = true)]
        private static extern int UuidCreateSequential(out Guid guid);


        /// <summary>
        /// 生成SQLSERVER能识别的有序GUID
        /// </summary>
        /// <returns>新生成的有序guid</returns>
        public static Guid NewSeqGuid()
        {
            // 请参见如下网址说明。
            // https://blogs.msdn.microsoft.com/dbrowne/2012/07/03/how-to-generate-sequential-guids-for-sql-server-in-net/
            // http://msdn.microsoft.com/zh-cn/library/ms189786%28v=sql.120%29.aspx
            // http://www.pinvoke.net/default.aspx/rpcrt4/UuidCreateSequential.html

            Guid guid;

            //调用windows API 创建有序GUID
            UuidCreateSequential(out guid);
            var s = guid.ToByteArray();

            //转换为byte数组
            var t = new byte[16];

            //由于sqlserver里有序guid与windows有序guid排序方式不一致，所以这里需要做一次转换
            //0-3作为一组替换
            //4-9作为一组替换
            //10-15作为一组替换

            t[3] = s[0];
            t[2] = s[1];
            t[1] = s[2];
            t[0] = s[3];
            t[5] = s[4];
            t[4] = s[5];
            t[7] = s[6];
            t[6] = s[7];
            t[8] = s[8];
            t[9] = s[9];
            t[10] = s[10];
            t[11] = s[11];
            t[12] = s[12];
            t[13] = s[13];
            t[14] = s[14];
            t[15] = s[15];

            //重新转换为Guid
            return new Guid(t);
        }


        /*
         
        private static void TestMethod()
        {
            string connectionString = @"................................";

            using( SqlConnection connection = new SqlConnection(connectionString) ) {
                connection.Open();

                SqlParameter p1 = new SqlParameter("@RowGuid", SqlDbType.UniqueIdentifier);
                SqlParameter p2 = new SqlParameter("@IntValue", SqlDbType.Int);

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "insert into TestGuid(RowGuid, IntValue) values(@RowGuid, @IntValue);";
                command.Parameters.Add(p1);
                command.Parameters.Add(p2);

                for( int i = 0; i < 1000; i++ ) {
                    p1.Value = GuidHelper.NewSeqGuid();
                    p2.Value = i + 1;
                    command.ExecuteNonQuery();
                }
            }
        }


        CREATE TABLE [dbo].[TestGuid](
            [RowGuid] UniqueIdentifier NOT NULL,
            [IntValue] [int] NOT NULL,
            [RowIndex] [int] IDENTITY(1,1) NOT NULL,
            CONSTRAINT [PK_TestGuid] PRIMARY KEY CLUSTERED 
            (
                [RowGuid] ASC
            )
        ) 

        检查方法： select * from [dbo].[TestGuid]

        */

    }
}
