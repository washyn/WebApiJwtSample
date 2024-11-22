using System;
using System.IO;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WindowsAzureStorage
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // "AzureStorageCredentials": {
            //     "StorageName": "akdemic",
            //     "AccessKey": "b3tcfXtiByUi2Yy4UK7o+oC1rm9F434YZUM4TcK1GwzJKORD1PxqujSGS3oIKp01iDt02YdYFXvVsC7KxhmaHQ=="
            // },
            Console.WriteLine("Hello World!");
            var setting = new CloudStorageCredentials()
            {
                AccessKey = "b3tcfXtiByUi2Yy4UK7o+oC1rm9F434YZUM4TcK1GwzJKORD1PxqujSGS3oIKp01iDt02YdYFXvVsC7KxhmaHQ==",
                StorageName = "akdemic",
            };
            var instance = new CloudStorageService(setting);
            
            
            var storageAccount = new CloudStorageAccount(
                new StorageCredentials(
                    setting.StorageName,
                    setting.AccessKey), true);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("eva");

            if (await container.CreateIfNotExistsAsync())
            {
                await container.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    }
                );
            }


            var localFileName = @"D:\temp_storage\Normas Revista Investigaciones EPG. (agosto 2023).pdf";
            var stream = File.OpenRead(localFileName);
            
            
            
            var fileName = Guid.NewGuid().ToString();
            var blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromStreamAsync(stream);
            
            var uri = blockBlob.Uri.ToString();
            
            Console.WriteLine(uri);
            Console.WriteLine(fileName);
            Console.ReadLine();
        }
    }
    
    public class CloudStorageCredentials
    {
        public string StorageName { get; set; }
        public string AccessKey { get; set; }
    }
    
    public class CloudStorageService
    {
        private readonly CloudBlobClient _blobClient;
        private readonly CloudStorageCredentials _settings;

        public CloudStorageService(CloudStorageCredentials settings)
        {
            _settings = settings;

            var storageAccount = new CloudStorageAccount(
                new StorageCredentials(
                    _settings.StorageName,
                    _settings.AccessKey), true);

            _blobClient = storageAccount.CreateCloudBlobClient();
        }

        private async Task<string> Upload(Stream stream, string cloudStorageContainer, string fileName)
        {
            var container = _blobClient.GetContainerReference(cloudStorageContainer);

            if (await container.CreateIfNotExistsAsync())
            {
                await container.SetPermissionsAsync(
                    new BlobContainerPermissions
                    {
                        PublicAccess = BlobContainerPublicAccessType.Blob
                    }
                );
            }

            var blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.UploadFromStreamAsync(stream);

            return blockBlob.Uri.ToString();
        }

        public async Task<string> UploadFile(Stream stream, string container, string fileName, string extension)
        {
            return await Upload(stream, container, $"{fileName}{extension}");
        }

        public async Task<string> UploadFile(Stream stream, String container, String extension)
        {
            return await Upload(stream, container, $"{Guid.NewGuid().ToString()}{extension}");
        }

        public async Task<string> UploadProductBinary(Stream stream, string container)
        {
            return await Upload(stream, container, Guid.NewGuid().ToString());
        }

        private async Task<bool> Delete(string fileName, string cloudStorageContainer)
        {
            var container = _blobClient.GetContainerReference(cloudStorageContainer);
            var blockBlob = container.GetBlockBlobReference(fileName);

            return await blockBlob.DeleteIfExistsAsync();
        }

        public async Task<bool> TryDelete(string fileName, string cloudStorageContainer)
        {
            return await Delete(fileName, cloudStorageContainer);
        }

        public async Task<bool> TryDeleteProductBinary(string fileName)
        {
            return await Delete(fileName, "binaries");
        }

        public async Task<bool> TryDeleteProductImage(string fileName)
        {
            return await Delete(fileName, "applications-images");
        }

        private async Task<Stream> Download(Stream stream, string cloudStorageContainer, string fileName)
        {
            var container = _blobClient.GetContainerReference(cloudStorageContainer);
            var blockBlob = container.GetBlockBlobReference(fileName);
            await blockBlob.DownloadToStreamAsync(stream);
            return stream;
        }

        public async Task<Stream> TryDownload(Stream stream, string cloudStorageContainer, string fileName)
        {
            return await Download(stream, cloudStorageContainer, fileName);
        }

        public async Task<Stream> TryDownloadProductBinary(Stream stream, string fileName)
        {
            return await Download(stream, "binaries", fileName);
        }

        public async Task<Stream> TryDownloadProductImage(Stream stream, string fileName)
        {
            return await Download(stream, "applications-images", fileName);
        }
    }
}