using BankingApp.Data.DocumentDb.Containers;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BankingApp.Data.DocumentDb.Repository
{
    public class LoanDocumentRepository : ILoanDocumentRepository
    {
        private readonly Container _container;
        private readonly ILogger<LoanDocumentRepository> _logger;

        public LoanDocumentRepository(CosmosClient cosmosClient, IConfiguration configuration, ILogger<LoanDocumentRepository> logger)
        {
            var databaseName = configuration.GetValue<string>("CosmosDbName");
            var containerName = configuration.GetValue<string>("LoanContainerName");
            _container = cosmosClient.GetContainer(databaseName, containerName);
            _logger = logger;
        }
        public async Task AddLoanDocumentRecords(IEnumerable<LoanDocuments> loanDocuments)
        {
            try
            {
                foreach (var loanDocument in loanDocuments)
                {
                    await _container.CreateItemAsync(loanDocument, new PartitionKey(loanDocument.Id.ToString()));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

        }

        public async Task<IEnumerable<LoanDocuments>> GetLoanDocumentsByLoanId(Guid loanApplicationId)
        {
            try
            {
                var iterator = _container
                    .GetItemLinqQueryable<LoanDocuments>(true)
                    .Where(k => k.LoanApplicationId == loanApplicationId)
                    .ToFeedIterator();

                var loanDocuments = new List<LoanDocuments>();
                while (iterator.HasMoreResults)
                {
                    var page = await iterator.ReadNextAsync();
                    loanDocuments.AddRange(page.Resource);
                }

                return loanDocuments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching loan documents for loan application {LoanApplicationId}", loanApplicationId);
                throw;
            }
        }
    }
}
