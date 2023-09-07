namespace ClownFish.UnitTest.WebClient;

[TestClass]
public class HttpFileTest
{
    [TestMethod]
    public void Test()
    {
        HttpFile file = new HttpFile();
        file.FileName = @"d:\aa.txt";
        //file.Key = "file1";
        file.FileBody = Guid.NewGuid().ToByteArray();
        //file.ContentLength = file.FileBody.Length;
        //file.ContentType = RequestContentType.Text;

        Assert.AreEqual(@"d:\aa.txt", file.FileName);
        //Assert.AreEqual("file1", file.Key);
        Assert.IsNotNull(file.FileBody);
        //Assert.AreEqual(16, file.ContentLength);
        //Assert.AreEqual(RequestContentType.Text, file.ContentType);
    }

    [TestMethod]
    public void Test_CreateFromFileInfo()
    {
        string filePath = Path.Combine(AppDomain.CurrentDomain.GetTempPath(), "Test_CreateFromFileInfo.dat");
        byte[] data = Guid.NewGuid().ToByteArray();
        RetryFile.WriteAllBytes(filePath, data);


        HttpFile file = HttpFile.CreateFromFileInfo(new FileInfo(filePath));
        Assert.AreEqual(filePath, file.FileName);
        Assert.IsNull(file.FileBody);
        Assert.IsNull(file.BodyStream);
        Assert.IsNotNull(file.FileInfo);
        Assert.AreEqual(data.Length, file.FileInfo.Length);
    }
}
