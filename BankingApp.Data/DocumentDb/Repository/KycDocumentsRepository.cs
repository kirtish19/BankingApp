using BankingApp.Data.DocumentDb.Containers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

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
        private readonly ILogger<KycDocumentsRepository> _logger;

        public KycDocumentsRepository(CosmosClient cosmosClient, IConfiguration configuration, ILogger<KycDocumentsRepository> logger)
        {
            var databaseName = configuration.GetValue<string>("CosmosDbName");
            var containerName = configuration.GetValue<string>("KycContainerName");
            _container = cosmosClient.GetContainer(databaseName, containerName);
            _logger = logger;
        }
        public async Task AddKycRecords(IEnumerable<KycDocument> kycDocuments)
        {
            try
            {
                foreach (var kycDocument in kycDocuments)
                {
                    await _container.CreateItemAsync(kycDocument, new PartitionKey(kycDocument.CustomerId.ToString()));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                throw;
            }

        }

        //public async Task<IEnumerable<KycDocument>> GetKycDocumentsByCustomerId(Guid customerId)
        //{
        //    try
        //    {
        //        var query = new QueryDefinition("SELECT * FROM c WHERE c.CustomerId = @customerId")
        //            .WithParameter("@customerId", customerId);

        //        var iterator = _container.GetItemQueryIterator<KycDocument>(query, requestOptions: new QueryRequestOptions { PartitionKey = new PartitionKey(customerId.ToString()) });

        //        var kycDocuments = new List<KycDocument>();
        //        while (iterator.HasMoreResults)
        //        {
        //            var feed = await iterator.ReadNextAsync();
        //            kycDocuments.AddRange(feed.Resource);
        //        }

        //        return kycDocuments;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error fetching KYC documents for customer {CustomerId}", customerId);
        //        throw;
        //    }
        //}

        public async Task<IEnumerable<KycDocument>> GetKycDocumentsByCustomerId(Guid customerId)
        {
            try
            {
                var iterator = _container
                    .GetItemLinqQueryable<KycDocument>(true)
                    .Where(k => k.CustomerId == customerId)
                    .ToFeedIterator();

                var kycDocuments = new List<KycDocument>();
                while (iterator.HasMoreResults)
                {
                    var page = await iterator.ReadNextAsync();
                    kycDocuments.AddRange(page.Resource);
                }

                return kycDocuments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching KYC documents for customer {CustomerId}", customerId);
                throw;
            }
        }
    }
}
