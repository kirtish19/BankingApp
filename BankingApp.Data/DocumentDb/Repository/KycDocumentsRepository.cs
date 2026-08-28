using BankingApp.Data.DocumentDb.Containers;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace BankingApp.Data.DocumentDb.Repository
{
    public class KycDocumentsRepository : IKycDocumentsRepository
    {

        //Keeping this code for reference - now switching to SDK based approach

        //private readonly DocumentDbContext _documentDbContext;


        //public KycDocumentsRepository(DocumentDbContext documentDbContext)
        //{
        //    _documentDbContext = documentDbContext;
        //}
        //public async Task AddKycRecords(IEnumerable<KycDocument> kycDocuments)
        //{
        //    await _documentDbContext.KycDocuments.AddRangeAsync(kycDocuments);
        //    await _documentDbContext.SaveChangesAsync();
        //}

        private readonly Container _container;
        public KycDocumentsRepository(CosmosClient cosmosClient, IConfiguration configuration)
        {
            var databaseName = configuration.GetValue<string>("CosmosDbName");
            var containerName = configuration.GetValue<string>("KycContainerName");
            _container = cosmosClient.GetContainer(databaseName, containerName);
        }
        public async Task AddKycRecords(IEnumerable<KycDocument> kycDocuments)
        {
            foreach (var kycDocument in kycDocuments)
            {
                await _container.CreateItemAsync(kycDocument, new PartitionKey(kycDocument.CustomerId.ToString()));
            }
        }
    }
}
