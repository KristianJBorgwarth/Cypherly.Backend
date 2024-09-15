using FakeItEasy;
using Microsoft.AspNetCore.Http;

namespace Cypherly.UserManagement.Storage.Test.Unit.Helpers;

public static class MockHelper
{
    public static IFormFile CreateFakeIFormFile(string fileName, string contentType, byte[] content)
    {
        var file = A.Fake<IFormFile>();
        var stream = new MemoryStream(content);
        var writer = new StreamWriter(stream);
        writer.Flush();
        stream.Position = 0;

        A.CallTo(() => file.FileName).Returns(fileName);
        A.CallTo(() => file.Length).Returns(stream.Length);
        A.CallTo(() => file.OpenReadStream()).Returns(stream);
        A.CallTo(() => file.ContentDisposition).Returns($"form-data; name=\"file\"; filename=\"{fileName}\"");
        A.CallTo(() => file.ContentType).Returns(contentType);

        return file;
    }
}