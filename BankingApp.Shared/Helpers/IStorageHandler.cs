namespace BankingApp.Shared.Helpers
{
    public interface IStorageHandler
    {
        public Task UploadBlobAsync(string connectionString, string containerName, string directoryName, IFormFileCollection formFiles);

        public Task DownloadBlobAsync(string connectionString, string containerName, string blobName, string downloadPath);

        public Task<AsyncPageable<BlobItem>?> ListBlobsAsync(string connectionString, string containerName);
    }
}
