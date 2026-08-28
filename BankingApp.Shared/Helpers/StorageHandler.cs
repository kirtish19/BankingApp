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

        public async Task UploadBlobAsync(string connectionString, string containerName, string directoryName, IFormFileCollection formFiles)
        {
            BlobContainerClient container = new BlobContainerClient(connectionString, containerName);

            await container.CreateIfNotExistsAsync();

            if (formFiles == null || !formFiles.Any())
                return;

            foreach (var formFile in formFiles)
            {
                if (formFile == null || formFile.Length == 0)
                    continue;

                string fileName = formFile.FileName;
                BlobClient blobClient = container.GetBlobClient($"{directoryName}/{fileName}");
                using var stream = formFile.OpenReadStream();
                await blobClient.UploadAsync(stream, overwrite: true);
            }
        }
    }
}
