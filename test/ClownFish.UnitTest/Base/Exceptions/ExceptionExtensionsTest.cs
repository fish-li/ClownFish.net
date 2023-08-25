using System;
using System.Collections.Generic;
using System.Text;
using ClownFish.Base;
using ClownFish.Base.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Exceptions
{
    [TestClass]
    public class ExceptionExtensionsTest
    {
        [TestMethod]
        public void Test_GetErrorCode()
        {
            Assert.AreEqual(200, ExceptionExtensions.GetErrorCode(null));

            ClownFish.Base.Exceptions.ValidationException ex1 = new ClownFish.Base.Exceptions.ValidationException("xxxxx");
            Assert.AreEqual(400, ExceptionExtensions.GetErrorCode(ex1));

            System.ComponentModel.DataAnnotations.ValidationException ex2 = new System.ComponentModel.DataAnnotations.ValidationException();
            Assert.AreEqual(400, ExceptionExtensions.GetErrorCode(ex2));

            ForbiddenException ex3 = new ForbiddenException("xxx");
            Assert.AreEqual(403, ExceptionExtensions.GetErrorCode(ex3));

            Exception ex4 = ExceptionHelper.CreateException();
            Assert.AreEqual(500, ExceptionExtensions.GetErrorCode(ex4));

            BusinessLogicException ex5 = new BusinessLogicException("xx");
            Assert.AreEqual(651, ExceptionExtensions.GetErrorCode(ex5));
        }

        [TestMethod]
        public void Test_StatusCode()
        {
            BusinessLogicException ex1 = new BusinessLogicException("xxx");
            Assert.AreEqual(651, ExceptionExtensions.GetErrorCode(ex1));

            BusinessLogicException ex1b = new BusinessLogicException("xxx") { StatusCode = 666 };
            Assert.AreEqual(666, ExceptionExtensions.GetErrorCode(ex1b));


            MessageException ex2 = new MessageException("xxx");
            Assert.AreEqual(500, ExceptionExtensions.GetErrorCode(ex2));

            MessageException ex2b = new MessageException("xxx") { StatusCode = 666 };
            Assert.AreEqual(666, ExceptionExtensions.GetErrorCode(ex2b));



            ClientDataException ex3 = new ClientDataException("xxx");
            Assert.AreEqual(400, ExceptionExtensions.GetErrorCode(ex3));

            ClientDataException ex3b = new ClientDataException("xxx") { StatusCode = 666 };
            Assert.AreEqual(666, ExceptionExtensions.GetErrorCode(ex3b));



            ConfigurationErrorsException ex4 = new ConfigurationErrorsException("xxx");
            Assert.AreEqual(500, ExceptionExtensions.GetErrorCode(ex4));


            DatabaseNotFoundException ex5 = new DatabaseNotFoundException("xxx");
            Assert.AreEqual(500, ExceptionExtensions.GetErrorCode(ex5));

            DatabaseNotFoundException ex5b = new DatabaseNotFoundException("xxx") { StatusCode = 666 };
            Assert.AreEqual(666, ExceptionExtensions.GetErrorCode(ex5b));


            DuplicateInsertException ex6 = new DuplicateInsertException("xxx", ex4);
            Assert.AreEqual(500, ExceptionExtensions.GetErrorCode(ex6));

            DuplicateInsertException ex6b = new DuplicateInsertException("xxx", ex4) { StatusCode = 666 };
            Assert.AreEqual(666, ExceptionExtensions.GetErrorCode(ex6b));


            ForbiddenException ex7 = new ForbiddenException("xxx");
            Assert.AreEqual(403, ExceptionExtensions.GetErrorCode(ex7));

            ForbiddenException ex7b = new ForbiddenException("xxx") { StatusCode = 666 };
            Assert.AreEqual(666, ExceptionExtensions.GetErrorCode(ex7b));

            HttpException ex8 = new HttpException(555, "xxx");
            Assert.AreEqual(555, ExceptionExtensions.GetErrorCode(ex8));

            HttpException ex8b = new HttpException(666, "xxx");
            Assert.AreEqual(666, ExceptionExtensions.GetErrorCode(ex8b));


            InvalidCodeException ex9 = new InvalidCodeException("xxx");
            Assert.AreEqual(500, ExceptionExtensions.GetErrorCode(ex9));


            TenantNotFoundException ex10 = new TenantNotFoundException("xxx");
            Assert.AreEqual(500, ExceptionExtensions.GetErrorCode(ex10));

            TenantNotFoundException ex10b = new TenantNotFoundException("xxx") { StatusCode = 666 };
            Assert.AreEqual(666, ExceptionExtensions.GetErrorCode(ex10b));

            ValidationException ex11 = new ValidationException("xxx");
            Assert.AreEqual(400, ExceptionExtensions.GetErrorCode(ex11));

            ValidationException ex11b = new ValidationException("xxx") { StatusCode = 666 };
            Assert.AreEqual(666, ExceptionExtensions.GetErrorCode(ex11b));
        }

        [TestMethod]
        public void Test_GetAllMessages()
        {
            MyAssert.IsError<ArgumentNullException>(() => {
                _ = ExceptionExtensions.GetAllMessages(null);
            });

            Exception ex = ExceptionHelper.CreateException("x1234567890");
            string message = ex.GetAllMessages();
            //Console.WriteLine(message);

            string[] lines = message.ToLines();
            Assert.AreEqual(2, lines.Length);
            Assert.AreEqual("(System.InvalidOperationException) x1234567890", lines[0]);
            Assert.AreEqual("==>(ClownFish.Base.MessageException) 这是个内部异常", lines[1]);
        }

        //[TestMethod]
        //public void Test_ArgumentNullException()
        //{
        //    MyAssert.IsError<ArgumentNullException>(() => {
        //        _ = ExceptionExtensions.GetAllMessages(null);
        //    });

        //    MyAssert.IsError<ArgumentNullException>(() => {
        //        _ = ExceptionExtensions.GetMessage(null);
        //    });
        //}

        //[TestMethod]
        //public void Test_SqlException()
        //{
        //    SqlException sqlException = ExceptionHelper.CreateSqlException(1212, "test-msg");
        //    string message = ExceptionExtensions.GetMessage(sqlException);
        //    Assert.IsTrue(message.StartsWith("(SqlException) 错误编号"));
        //}
    }
}
