namespace BankingApp.LoanApi.Services
{
    public class LoanService(IUnitOfWork unitOfWork, IStorageHandler storageHandler, IConfiguration configuration, IServiceBusHandler serviceBusHandler) : ILoanService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IStorageHandler _storageHandler = storageHandler;
        private readonly IConfiguration _configuration = configuration;
        private readonly IServiceBusHandler _serviceBusHandler = serviceBusHandler;

        public async Task LoanApplicationSubmitAsync(PostLoanApplicationRequest request)
        {
            try
            {
                var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(request.CustomerId)
                                ?? throw new InvalidDataException($"Customer with id {request.CustomerId} does not exist.");

                if (customer.Status != CustomerStatus.Active)
                    throw new InvalidDataException($"Customer with id {request.CustomerId} is not active.");

                var loanApplicationDto = request.ToLoanApplicationsDto();

                await _unitOfWork.TransactionManager.BeginTransactionAsync();
                await _unitOfWork.LoanApplicationRepository.AddAsync(loanApplicationDto.ToLoanApplication());


                Dictionary<string, string>? uploadedBlobUrls = null;
                FormFileCollection loanDocuments = [
                    request.SalarySlip,
                request.BankStatement,
                request.EmploymentLetter
                    ];
                var storageConnectionString = _configuration.GetValue<string>("StorageAccountConnectionString")!;
                var containerName = _configuration.GetValue<string>("StorageContainerNameLoan")!;
                uploadedBlobUrls = await _storageHandler.UploadBlobAsync(storageConnectionString, containerName, loanApplicationDto.Id.ToString(), loanDocuments);

                var loanApplicationMessage = CreateLoanApplicationMessage(request, loanApplicationDto, loanDocuments, uploadedBlobUrls);

                var message = loanApplicationMessage;
                var queueName = _configuration.GetValue<string>("LoanQueueName")!;
                var serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusWriter")!;
                var additionalProperties = new Dictionary<string, object>
                    {
                        { nameof(LoanApplicationMessage.DocumentType), loanApplicationMessage.DocumentType },
                    };
                await _serviceBusHandler.SendMessageToQueueOrTopic(message, queueName, serviceBusConnectionString, additionalProperties);
                await _unitOfWork.TransactionManager.SaveChangesAsync();
                await _unitOfWork.TransactionManager.CommitAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.TransactionManager.RollbackAsync();
                throw;
            }

        }

        private static LoanApplicationMessage CreateLoanApplicationMessage(PostLoanApplicationRequest request, LoanApplicationsDto loanApplicationDto, IFormFileCollection loanDocuments, Dictionary<string, string>? uploadedBlobUrls = null)
        {
            LoanApplicationMessage loanApplicationEvent = new()
            {
                EventId = Guid.NewGuid(),
                EventType = "LoanApplicationSubmitted",
                EventTime = DateTime.Now,
                DocumentType = "LOAN",
                CustomerId = loanApplicationDto.CustomerId,
                LoanApplicationId = loanApplicationDto.Id,
            };

            if (loanDocuments is not null && uploadedBlobUrls is not null)
            {
                foreach (var doc in loanDocuments)
                {
                    uploadedBlobUrls.TryGetValue(doc.FileName, out var url);

                    loanApplicationEvent.Documents.Add(
                      new BankingDocument
                      {
                          DocumentId = Guid.NewGuid(),
                          DocumentName = doc.FileName,
                          BlobUrl = url!
                      }
                    );
                }
            }
            loanApplicationEvent.UploadedBy = "LoanApi";
            loanApplicationEvent.SourceSystem = "LoanService";
            return loanApplicationEvent;
        }
    }
}