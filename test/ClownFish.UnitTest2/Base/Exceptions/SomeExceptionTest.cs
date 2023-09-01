using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Base;
using ClownFish.Base.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ClownFish.UnitTest.Base.Exceptions;
[TestClass]
public class SomeExceptionTest
{
    [TestMethod]
    public void Test_BusinessLogicException()
    {
        BusinessLogicException ex1 = new BusinessLogicException("SomeExceptionTest") { StatusCode = 567 };
        Assert.AreEqual("SomeExceptionTest", ex1.Message);
        Assert.AreEqual(567, ex1.StatusCode);
        Assert.AreEqual(567, (ex1 as IErrorCode).GetErrorCode());

        Exception innerEx = ExceptionHelper.CreateException();
        BusinessLogicException ex2 = new BusinessLogicException("SomeExceptionTest", innerEx) { StatusCode = 567 };
        Assert.IsNotNull(ex2.InnerException);
    }



    [TestMethod]
    public void Test_ClientDataException()
    {
        ClientDataException ex1 = new ClientDataException("SomeExceptionTest") { StatusCode = 567 };
        Assert.AreEqual("SomeExceptionTest", ex1.Message);
        Assert.AreEqual(567, ex1.StatusCode);
        Assert.AreEqual(567, (ex1 as IErrorCode).GetErrorCode());

        Exception innerEx = ExceptionHelper.CreateException();
        ClientDataException ex2 = new ClientDataException("SomeExceptionTest", innerEx) { StatusCode = 567 };
        Assert.IsNotNull(ex2.InnerException);
    }

    [TestMethod]
    public void Test_ConfigurationErrorsException()
    {
        ConfigurationErrorsException ex1 = new ConfigurationErrorsException("SomeExceptionTest");
        Assert.AreEqual("SomeExceptionTest", ex1.Message);
        Assert.AreEqual(500, (ex1 as IErrorCode).GetErrorCode());

        Exception innerEx = ExceptionHelper.CreateException();
        ConfigurationErrorsException ex2 = new ConfigurationErrorsException("SomeExceptionTest", innerEx);
        Assert.IsNotNull(ex2.InnerException);
    }


    [TestMethod]
    public void Test_DatabaseNotFoundException()
    {
        DatabaseNotFoundException ex1 = new DatabaseNotFoundException("SomeExceptionTest") { StatusCode = 567 };
        Assert.AreEqual("SomeExceptionTest", ex1.Message);
        Assert.AreEqual(567, ex1.StatusCode);
        Assert.AreEqual(567, (ex1 as IErrorCode).GetErrorCode());

        Exception innerEx = ExceptionHelper.CreateException();
        DatabaseNotFoundException ex2 = new DatabaseNotFoundException("SomeExceptionTest", innerEx) { StatusCode = 567 };
        Assert.IsNotNull(ex2.InnerException);
    }

    [TestMethod]
    public void Test_DuplicateInsertException()
    {
        Exception innerEx = ExceptionHelper.CreateException();
        DuplicateInsertException ex1 = new DuplicateInsertException("SomeExceptionTest", innerEx) { StatusCode = 567 };
        Assert.AreEqual("SomeExceptionTest", ex1.Message);
        Assert.AreEqual(567, ex1.StatusCode);
        Assert.AreEqual(567, (ex1 as IErrorCode).GetErrorCode());
        Assert.IsNotNull(ex1.InnerException);
    }

    [TestMethod]
    public void Test_ForbiddenException()
    {
        ForbiddenException ex1 = new ForbiddenException("SomeExceptionTest") { StatusCode = 567 };
        Assert.AreEqual("SomeExceptionTest", ex1.Message);
        Assert.AreEqual(567, ex1.StatusCode);
        Assert.AreEqual(567, (ex1 as IErrorCode).GetErrorCode());

        Exception innerEx = ExceptionHelper.CreateException();
        ForbiddenException ex2 = new ForbiddenException("SomeExceptionTest", innerEx) { StatusCode = 567 };
        Assert.IsNotNull(ex2.InnerException);
    }

    [TestMethod]
    public void Test_HttpException()
    {
        HttpException ex1 = new HttpException(567, "SomeExceptionTest");
        Assert.AreEqual("SomeExceptionTest", ex1.Message);
        Assert.AreEqual(567, ex1.StatusCode);
        Assert.AreEqual(567, (ex1 as IErrorCode).GetErrorCode());
    }

    [TestMethod]
    public void Test_InvalidCodeException()
    {
        InvalidCodeException ex1 = new InvalidCodeException("SomeExceptionTest");
        Assert.AreEqual("SomeExceptionTest", ex1.Message);
        Assert.AreEqual(500, (ex1 as IErrorCode).GetErrorCode());
    }


    [TestMethod]
    public void Test_MessageException()
    {
        MessageException ex1 = new MessageException("SomeExceptionTest") { StatusCode = 567 };
        Assert.AreEqual("SomeExceptionTest", ex1.Message);
        Assert.AreEqual(567, ex1.StatusCode);
        Assert.AreEqual(567, (ex1 as IErrorCode).GetErrorCode());

        Exception innerEx = ExceptionHelper.CreateException();
        MessageException ex2 = new MessageException("SomeExceptionTest", innerEx) { StatusCode = 567 };
        Assert.IsNotNull(ex2.InnerException);
    }


    [TestMethod]
    public void Test_TenantNotFoundException()
    {
        TenantNotFoundException ex1 = new TenantNotFoundException("SomeExceptionTest") { StatusCode = 567 };
        Assert.AreEqual("SomeExceptionTest", ex1.Message);
        Assert.AreEqual(567, ex1.StatusCode);
        Assert.AreEqual(567, (ex1 as IErrorCode).GetErrorCode());

        Exception innerEx = ExceptionHelper.CreateException();
        TenantNotFoundException ex2 = new TenantNotFoundException("SomeExceptionTest", innerEx) { StatusCode = 567 };
        Assert.IsNotNull(ex2.InnerException);
    }

    [TestMethod]
    public void Test_ValidationException()
    {
        ValidationException2 ex1 = new ValidationException2("SomeExceptionTest") { StatusCode = 567 };
        Assert.AreEqual("SomeExceptionTest", ex1.Message);
        Assert.AreEqual(567, ex1.StatusCode);
        Assert.AreEqual(567, (ex1 as IErrorCode).GetErrorCode());

        Exception innerEx = ExceptionHelper.CreateException();
        ValidationException2 ex2 = new ValidationException2("SomeExceptionTest", innerEx) { StatusCode = 567 };
        Assert.IsNotNull(ex2.InnerException);
    }
}
