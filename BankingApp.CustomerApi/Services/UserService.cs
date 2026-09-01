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
                var userDto = request.ToUserDto();
                var user = userDto.ToUser();
                await _unitOfWork.UserRepository.AddAsync(user);

                // Customer is created only for Customer registration
                if (request.UserType == UserType.Customer)
                {
                    var customerDto = request.ToCustomerDto();
                    var customer = customerDto.ToCustomer(userDto.Id);
                    await _unitOfWork.CustomerRepository.AddAsync(customer);


                    Dictionary<string, string>? uploadedBlobUrls = null;
                    if (request.KycDocuments is not null && request.KycDocuments.Any())
                    {
                        var storageConnectionString = _configuration.GetValue<string>("StorageAccountConnectionString")!;
                        var containerName = _configuration.GetValue<string>("StorageContainerName")!;
                        uploadedBlobUrls = await _storageHandler.UploadBlobAsync(storageConnectionString, containerName, customerDto.Id.ToString(), request.KycDocuments);
                    }

                    CustomerKYCMessage customerKYCMessage = CreateCustomerKYCMessage(request, customerDto, uploadedBlobUrls);

                    var message = customerKYCMessage;
                    var topicName = _configuration.GetValue<string>("KycTopicName")!;
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

        private static CustomerKYCMessage CreateCustomerKYCMessage(PostUserRegisterationRequest request, CustomerDto customer, Dictionary<string, string>? uploadedBlobUrls = null)
        {
            CustomerKYCMessage customerKYCEvent = new CustomerKYCMessage
            {
                EventId = Guid.NewGuid(),
                EventType = "CustomerKYCUploaded",
                EventTime = DateTime.Now,
                DocumentType = "KYC",
                CustomerId = customer.Id
            };

            if (request.KycDocuments is not null)
            {
                foreach (var doc in request.KycDocuments)
                {
                    var blobUrl = (uploadedBlobUrls != null && uploadedBlobUrls.TryGetValue(doc.FileName, out var url))
                        ? url
                        : $"https://team1bankingapp.blob.core.windows.net/kyc-documents/{customer.Id}/{doc.FileName}"; // fallback to previously used pattern if upload did not return URL

                    customerKYCEvent.Documents.Add(
                      new CustomerKYCDocument
                      {
                          DocumentId = Guid.NewGuid(),
                          DocumentName = doc.FileName,
                          BlobUrl = blobUrl
                      }
                    );
                }
            }

            customerKYCEvent.UploadedBy = "Customer";
            customerKYCEvent.SourceSystem = "CustomerService";
            return customerKYCEvent;
        }

        public Task<bool> LoginUserAsync(PostLoginRequest request)
        {
            //validate user id and password and also ensure the logged in user is active if not reject.
            throw new NotImplementedException();
        }
    }
}
