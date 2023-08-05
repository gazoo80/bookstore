using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using BulkyBook.Utility.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Utility
{
    public class AzureFileStorage : IFileStorage
    {
        private readonly string connectionString;

        public AzureFileStorage(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureStorage");
        }

        public async Task DeleteFile(string path, string container)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var client = new BlobContainerClient(connectionString, container);
            await client.CreateIfNotExistsAsync();
            var file = Path.GetFileName(path);
            var blob = client.GetBlobClient(file);
            await blob.DeleteIfExistsAsync();
        }

        public async Task<string> EditFile(byte[] content, string extension, string container, string path, string type)
        {
            await DeleteFile(path, container);
            return await SaveFile(content, extension, container, type);
        }

        public async Task<string> SaveFile(byte[] content, string extension, string container, string type)
        {
            var client = new BlobContainerClient(connectionString, container);
            await client.CreateIfNotExistsAsync();
            client.SetAccessPolicy(PublicAccessType.Blob);

            var fileName = $"{Guid.NewGuid()}{extension}";
            var blob = client.GetBlobClient(fileName);

            var blobUploadOptions = new BlobUploadOptions();
            var blobHttpHeader = new BlobHttpHeaders();
            blobHttpHeader.ContentType = type;
            blobUploadOptions.HttpHeaders = blobHttpHeader;

            await blob.UploadAsync(new BinaryData(content), blobUploadOptions);
            return blob.Uri.ToString();
        }
    }
}
