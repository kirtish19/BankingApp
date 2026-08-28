namespace BankingApp.CustomerApi.Services
{
    public class UserService(IUnitOfWork unitOfWork, IStorageHandler storageHandler, IServiceBusHandler serviceBusHandler, IConfiguration configuration) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IStorageHandler _storageHandler = storageHandler;
        private readonly IServiceBusHandler _serviceBusHandler = serviceBusHandler;
        private readonly IConfiguration _configuration = configuration;

        public async Task RegisterUserAsync(PostUserRegisterationRequest request)
        {
            try
            {
                var user = request.ToUser();
                await _unitOfWork.UserRepository.AddAsync(user);

                // Customer is created only for Customer registration
                if (request.UserType == UserType.Customer)
                {
                    var customer = request.ToCustomer(user.Id);
                    await _unitOfWork.CustomerRepository.AddAsync(customer);


                    if (request.KycDocuments is not null && request.KycDocuments.Any())
                    {
                        var storageConnectionString = _configuration.GetValue<string>("StorageAccountConnectionString")!;
                        var containerName = _configuration.GetValue<string>("StorageContainerName")!;
                        await _storageHandler.UploadBlobAsync(storageConnectionString, containerName, user.Customer!.Id.ToString(), request.KycDocuments);
                    }

                    CustomerKYCMessage customerKYCMessage = CreateCustomerKYCMessage(request, customer);

                    var message = customerKYCMessage;
                    var topicName = "DocumentTopic";
                    var serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusWriter")!;
                    var additionalProperties = new Dictionary<string, object>
                    {
                        { nameof(CustomerKYCMessage.DocumentType), customerKYCMessage.DocumentType },
                    };
                    await _serviceBusHandler.SendMessageToQueueOrTopic(message, topicName, serviceBusConnectionString, additionalProperties);
                }

                // Commit everything together
                await _unitOfWork.TransactionManager.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private static CustomerKYCMessage CreateCustomerKYCMessage(PostUserRegisterationRequest request, Customer customer)
        {
            CustomerKYCMessage customerKYCEvent = new CustomerKYCMessage();
            customerKYCEvent.EventId = Guid.NewGuid();
            customerKYCEvent.EventType = "CustomerKYCUploaded";
            customerKYCEvent.EventTime = DateTime.Now;
            customerKYCEvent.DocumentType = "KYC";
            customerKYCEvent.CustomerId = customer.Id;

            foreach (var doc in request.KycDocuments!)
            {
                customerKYCEvent.Documents.Add(
                  new CustomerKYCDocument
                  {
                      DocumentId = Guid.NewGuid(),
                      DocumentName = doc.FileName,
                      BlobUrl = $"https://team1bankingapp.blob.core.windows.net/kyc-documents/{customer.Id}/{doc.FileName}" //TODO - This URL should be generated based on the actual blob storage URL after upload
                  }
                );
            }

            customerKYCEvent.UploadedBy = "Customer";
            customerKYCEvent.SourceSystem = "CustomerService";
            return customerKYCEvent;
        }
    }
}
