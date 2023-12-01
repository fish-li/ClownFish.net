using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClownFish.Web.Utils;
using Microsoft.AspNetCore.Http;

namespace ClownFish.Web.UnitTest.Utils;
[TestClass]
public class TempFileExtensionsTest
{
    [TestMethod]
    public void Test_error()
    {
        NHttpContext httpContext = null;

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = httpContext.CreateTempFile((byte[])null);
        });
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = httpContext.CreateTempFile((Stream)null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = httpContext.CreateTempFile((IFormFile)null);
        });
    }

    [TestMethod]
    public void Test_error2()
    {
        MyAssert.IsError<ArgumentNullException>(() => {
            _ = NullHttpContext.Instance.CreateTempFile((byte[])null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = NullHttpContext.Instance.CreateTempFile((Stream)null);
        });

        MyAssert.IsError<ArgumentNullException>(() => {
            _ = NullHttpContext.Instance.CreateTempFile((IFormFile)null);
        });
    }


    [TestMethod]
    public void Test_bytes()
    {
        byte[] bb = Guid.NewGuid().ToByteArray();
        string filePath = NullHttpContext.Instance.CreateTempFile(bb);
        Console.WriteLine(filePath);

        Assert.IsNotNull(filePath);
        Assert.IsTrue(File.Exists(filePath));

        NullHttpContext.Instance.DisposeObjects();

        System.Threading.Thread.Sleep(10);
        Assert.IsFalse(File.Exists(filePath));
    }


    [TestMethod]
    public void Test_stream()
    {
        byte[] bb = Guid.NewGuid().ToByteArray();
        using MemoryStream ms = new MemoryStream(bb);

        string filePath = NullHttpContext.Instance.CreateTempFile(ms);
        Console.WriteLine(filePath);

        Assert.IsNotNull(filePath);
        Assert.IsTrue(File.Exists(filePath));

        NullHttpContext.Instance.DisposeObjects();

        System.Threading.Thread.Sleep(10);
        Assert.IsFalse(File.Exists(filePath));
    }


    [TestMethod]
    public void Test_formfile()
    {
        byte[] bb = Guid.NewGuid().ToByteArray();
        using TempFile file = TempFile.CreateFile(bb);

        MockFormFile formfile = new MockFormFile(file.FilePath);

        string filePath = NullHttpContext.Instance.CreateTempFile(formfile);
        Console.WriteLine(filePath);

        Assert.IsNotNull(filePath);
        Assert.IsTrue(File.Exists(filePath));

        NullHttpContext.Instance.DisposeObjects();

        System.Threading.Thread.Sleep(10);
        Assert.IsFalse(File.Exists(filePath));
    }
}


internal sealed class MockFormFile : IFormFile
{
    private readonly string _filePath;

    public MockFormFile(string filePath)
    {
        _filePath = filePath;
    }

    public Stream OpenReadStream()
    {
        return File.OpenRead(_filePath);
    }

    public void CopyTo(Stream target)
    {
        throw new NotImplementedException();
    }

    public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public string ContentType => throw new NotImplementedException();

    public string ContentDisposition => throw new NotImplementedException();

    public IHeaderDictionary Headers => throw new NotImplementedException();

    public long Length => (new FileInfo(_filePath)).Length;

    public string Name => Path.GetFileNameWithoutExtension(_filePath);

    public string FileName => Path.GetFileName(_filePath);
}
