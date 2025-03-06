﻿using Amazon.S3;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Bucket.Services;
using Cypherly.UserManagement.Bucket.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Cypherly.UserManagement.Bucket.Configuration;

public static class StorageConfiguration
{
    public static IServiceCollection AddStorage(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IAmazonS3>(sp =>

        {
            var minioSettings = sp.GetRequiredService<IOptions<MinioSettings>>().Value;
            var credentials = new Amazon.Runtime.BasicAWSCredentials(minioSettings.User, minioSettings.Password);
            return new AmazonS3Client(credentials, new AmazonS3Config
            {
                ServiceURL = minioSettings.Host,
                ForcePathStyle = true,
            });
        });

        services.AddHttpClient<IMinioProxyClient, MinioProxyClient>((sp, client) =>
        {
            client.BaseAddress = new Uri(sp.GetRequiredService<IOptions<MinioSettings>>().Value.Host);
        });

        services.AddScoped<IProfilePictureService, ProfilePictureService>();
        services.AddScoped<IFileValidator, FileValidator>();

        return services;
    }
}