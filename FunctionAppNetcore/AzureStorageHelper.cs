using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;


namespace Function.Helper
{
    public class AzureStorageHelper
    {


        public static async Task<MemoryStream> Download(string connectionString, string containerName, string fullBlobPath)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            if (!await container.ExistsAsync())
            {
                throw new Exception("Container does not exist");
            }


            CloudBlockBlob blockBlob = container.GetBlockBlobReference(fullBlobPath);

            MemoryStream download = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(download);

            return download;

        }

        public static async Task<MemoryStream> DownloadFromUrl(string connectionString, Uri uri)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudBlockBlob blockBlob = new CloudBlockBlob(uri, storageAccount.Credentials);

            MemoryStream download = new MemoryStream();
            await blockBlob.DownloadToStreamAsync(download);

            return download;

        }

        //public static async Task<MemoryStream> Download(String connectionString, String resource, string objectId)
        //{

        //    return await Download(connectionString, resource + "/" + objectId.ToString());

        //}

        //public static async Task<MemoryStream> Download(string connectionString, string containerName, String resource, Guid objectId)
        //{
        //    return await Download(connectionString,containerName, resource, objectId.ToString());

        //}

        /// <summary>
        /// Upload file in azure storage
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static async Task<string> UploadFileAsBlob(string connectionString, string containerName, string resource, string objectName, Stream stream, Dictionary<string, string> Attributes = null)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve a reference to a container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // Create the container if it doesn't already exist.
            await container.CreateIfNotExistsAsync();

            String path = objectName;

            if (!string.IsNullOrWhiteSpace(resource))
            {
                path = resource + "/" + path;
            }

            CloudBlockBlob blockBlob = container.GetBlockBlobReference(path);

            if (Attributes != null)
            {
                foreach (var key in Attributes.Keys)
                {
                    blockBlob.Metadata[key] = Attributes[key];
                }
            }

            await blockBlob.UploadFromStreamAsync(stream);

            stream.Dispose();
            return blockBlob?.Uri.ToString();
        }

        public static async Task<bool> DeleteBlob(string connectionString, string containerName, String ObjectPath)
        {
            // Retrieve storage account from connection string.
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);

            // Create the blob client.
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            // Retrieve reference to a previously created container.
            CloudBlobContainer container = blobClient.GetContainerReference(containerName);

            // Retrieve reference to a blob named "myblob.txt".
            CloudBlockBlob blockBlob = container.GetBlockBlobReference(ObjectPath);

            // Delete the blob.
            await blockBlob.DeleteAsync();

            return true;

        }
    }
}
