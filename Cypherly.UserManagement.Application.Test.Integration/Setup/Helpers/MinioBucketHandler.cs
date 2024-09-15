using Minio;
using Minio.DataModel.Args;

namespace Cypherly.UserManagement.Application.Test.Integration.Setup.Helpers;

internal class MinioBucketHandler(string user, string password)
{
    public async Task CreateBucketAsync(string bucketName)
    {
        var minioClient = new MinioClient()
            .WithEndpoint("localhost", 9023)
            .WithCredentials(user, password)
            .Build();
        bool bucketExits = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName));

        switch (bucketExits)
        {
            case true:
                return;

            case false:
                await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName));
                break;
        }
    }
}