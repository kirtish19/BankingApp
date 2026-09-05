using Microsoft.Identity.Client;

namespace BankingApp.CustomerApi.Services
{
    public class UserService(IUnitOfWork unitOfWork, IStorageHandler storageHandler,
        IServiceBusHandler serviceBusHandler, IConfiguration configuration) : IUserService
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
                      new BankingDocument
                      {
                          DocumentId = Guid.NewGuid(),
                          DocumentName = doc.FileName,
                          BlobUrl = blobUrl
                      }
                    );
                }
            }

            customerKYCEvent.UploadedBy = "CustomerApi";
            customerKYCEvent.SourceSystem = "CustomerService";
            return customerKYCEvent;
        }

        public async Task<LoginResponseDto> LoginUserAsync(PostLoginRequest request)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUserName(request.UserName);
            if (user == null) return new LoginResponseDto { LoginSuccess = false };
            using var hmac = new HMACSHA512(user.LoginPasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));
            for (int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.LoginPasswordHash[i]) return new LoginResponseDto { LoginSuccess = false };
            }
            return new LoginResponseDto { LoginSuccess = true, User = user };
        }

        public async Task<string> GetTokenAsync(User user)
        {
            var tenantId = _configuration.GetValue<string>("AzureEntra:TenantId");
            var clientId = _configuration.GetValue<string>("AzureEntra:Customer:ClientId");
            var clientSecret = _configuration.GetValue<string>("AzureEntra:Customer:ClientSecret");
            var scope = _configuration.GetValue<string>("AzureEntra:Scope");
            var instance = _configuration.GetValue<string>("AzureEntra:Instance");
            var authority = $"{instance}/{tenantId}";

            IConfidentialClientApplication app = ConfidentialClientApplicationBuilder
                .Create(clientId)
                .WithClientSecret(clientSecret)
                .WithAuthority(new Uri(authority))
                .Build();

            var scopes = new[] { scope };

            try
            {
                var result = await app.AcquireTokenForClient(scopes).ExecuteAsync();
                return result.AccessToken;
            }
            catch (MsalServiceException msalEx)
            {
                // bubble up with context or log as needed
                throw new InvalidOperationException("Failed to acquire token from Entra ID.", msalEx);
            }
        }

        public async Task<CustomerDto?> GetCustomerDetailsAsync(Guid customerId)
        {
            var customer = await _unitOfWork.CustomerRepository.GetByIdAsync(customerId);
            return customer?.ToCustomerDto();
        }
    }
}
