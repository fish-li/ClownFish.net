using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest
{
    internal static class ExceptionHelper
    {
        internal static Exception CreateException()
        {
            try {
                throw new MessageException("一个用于测试的异常");
            }
            catch( Exception ex ) {
                return ex;
            }
        }

        internal static Exception CreateException(string message)
        {
            try {
                try {
                    throw new MessageException("这是个内部异常");
                }
                catch( Exception ex ) {
                    throw new InvalidOperationException(message, ex);
                }
            }
            catch( Exception ex2 ) {
                return ex2;
            }
        }


        public static string ExecuteActionReturnErrorMessage(Action action)
        {
            try {
                action();
            }
            catch( Exception ex ) {
                return ex.Message;
            }


            throw new InternalTestFailureException("异常没有出现！");
        }

        public async static Task<string> ExecuteActionReturnErrorMessage(Func<Task> action)
        {
            try {
                await action();
            }
            catch( Exception ex ) {
                return ex.Message;
            }


            throw new InternalTestFailureException("异常没有出现！");
        }





        internal static SqlException CreateSqlException(int number, string message = "xxxx", int @class = 20, int state = 50, string server = "server_xxx", string procedure = "procedure_xxx", int line = 101)
        {
            ConstructorInfo ctor = null;
            SqlError error = null;

#if NETFRAMEWORK
            // private string source = ".Net SqlClient Data Provider";
            // internal SqlError(int infoNumber, byte errorState, byte errorClass, string server, string errorMessage, string procedure, int lineNumber)
            ctor = typeof(SqlError).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                new Type[] { typeof(int), typeof(byte), typeof(byte), typeof(string), typeof(string), typeof(string), typeof(int) }, null);

            error = (SqlError)ctor.Invoke(new object[] { number, (byte)state, (byte)@class, server, message, procedure, line });
#else
            // private string _source = "Core .Net SqlClient Data Provider";
            // internal SqlError(int infoNumber, byte errorState, byte errorClass, string server, string errorMessage, string procedure, int lineNumber, Exception exception = null)
            ctor = typeof(SqlError).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null,
                new Type[] { typeof(int), typeof(byte), typeof(byte), typeof(string), typeof(string), typeof(string), typeof(int), typeof(Exception) }, null);

            error = (SqlError)ctor.Invoke(new object[] { number, (byte)state, (byte)@class, server, message, procedure, line, null });
#endif


            ConstructorInfo ctor2 = typeof(SqlErrorCollection).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, Type.EmptyTypes, null);
            SqlErrorCollection errorCollection = (SqlErrorCollection)ctor2.Invoke(null);

            typeof(SqlErrorCollection).InvokeMember("Add",
                BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.NonPublic,
                null, errorCollection, new object[] { error });

            SqlException ex = (SqlException)typeof(SqlException).InvokeMember("CreateException", BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.NonPublic,
                null, null, new object[] { errorCollection, "10.0.0" });

            return ex;
        }


        internal static MyDbException CreateMyDbException(string message = "xxxx")
        {
            return new MyDbException(message);
        }
    }


    public class MyDbException : DbException
    {
        internal MyDbException(string message) : base(message)
        {
        }
    }
}
