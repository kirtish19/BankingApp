using Azure.Storage.Sas;

namespace BankingApp.Shared.Helpers
{
    public class StorageHandler : IStorageHandler
    {
        public async Task DownloadBlobAsync(string connectionString, string containerName, string blobName, string downloadPath)
        {
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            BlobClient blobClient = container.GetBlobClient(blobName);

            await blobClient.DownloadToAsync(downloadPath);
        }

        public Task<AsyncPageable<BlobItem>?> ListBlobsAsync(string connectionString, string containerName)
        {
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            var blobItems = container.GetBlobsAsync();
            return Task.FromResult<AsyncPageable<BlobItem>?>(blobItems);
        }

        public async Task<Dictionary<string, string>> UploadBlobAsync(string connectionString, string containerName, string directoryName, IFormFileCollection formFiles)
        {
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            await container.CreateIfNotExistsAsync();

            if (formFiles == null || !formFiles.Any())
                return [];

            var uploadedUrls = new Dictionary<string, string>();

            foreach (var formFile in formFiles)
            {
                if (formFile == null || formFile.Length == 0)
                    continue;

                string fileName = formFile.FileName;
                BlobClient blobClient = container.GetBlobClient($"{directoryName}/{fileName}");
                using var stream = formFile.OpenReadStream();
                await blobClient.UploadAsync(stream, overwrite: true);

                // Record the blob URI where the file was uploaded. This is the canonical URL to the blob.
                uploadedUrls[fileName] = blobClient.Uri.ToString();
            }

            return uploadedUrls;
        }


        public string GenerateBlobSasUrl(string connectionString, string containerName, string blobName)
        {
            BlobContainerClient container =
                new BlobContainerClient(
                    connectionString,
                    containerName);

            BlobClient blobClient =
                container.GetBlobClient(blobName);

            if (!blobClient.CanGenerateSasUri)
            {
                throw new InvalidOperationException(
                    "Storage connection does not have permission to generate SAS.");
            }

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerName,
                BlobName = blobName,
                Resource = "b",

                // SAS will be valid for 30 minutes
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(30)
            };

            // Read-only permission
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            Uri sasUri = blobClient.GenerateSasUri(sasBuilder);

            return sasUri.ToString();
        }

    }
}
