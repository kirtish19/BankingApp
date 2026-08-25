using BankingApp.CustomerApi.Extensions.Mappings;
using BankingApp.Data.Tables;
using BankingApp.Shared.Helpers;
using Microsoft.Azure.Amqp.Framing;

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

                await _unitOfWork.Users.AddAsync(user);

                // Customer is created only for Customer registration
                if (request.UserType == UserType.Customer)
                {
                    var customer = request.ToCustomer(user.Id);
                    await _unitOfWork.Customers.AddAsync(customer);


                    if (request.KycDocuments is not null && request.KycDocuments.Any())
                    {
                        var storageConnectionString = _configuration.GetValue<string>("StorageAccountConnectionString")!;
                        var containerName = _configuration.GetValue<string>("StorageContainerName")!;
                        await _storageHandler.UploadBlobAsync(storageConnectionString, containerName, user.Customer!.Id.ToString(), request.KycDocuments);
                    }
                }
                //TODO - Message will contain - 
                /*
                 * {
                      "eventId": "3b4d9eb1-8f5c-4f18-a35b-f99111a1c001",
                      "eventType": "CustomerKYCUploaded",
                      "eventTime": "2026-08-25T10:15:00Z",
                      "documentType": "KYC",
                      "customerId": 1001,

                      "documents": [
                        {
                          "documentId": "DOC001",
                          "documentName": "PAN.pdf",
                          "blobUrl": "https://bankstorage.blob.core.windows.net/kyc-documents/1001/PAN.pdf"
                        },
                        {
                          "documentId": "DOC002",
                          "documentName": "AADHAAR.pdf",
                          "blobUrl": "https://bankstorage.blob.core.windows.net/kyc-documents/1001/AADHAAR.pdf"
                        }
                      ],

                      "uploadedBy": "Customer",
                      "sourceSystem": "CustomerService"
                    }
                 */

                var message = user;
                var topicName = "DocumentTopic";
                var serviceBusConnectionString = _configuration.GetValue<string>("ServiceBusWriter")!;
                await _serviceBusHandler.SendMessageToQueueOrTopic(message, topicName, serviceBusConnectionString);
                // Commit everything together
                await _unitOfWork.TransactionManager.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }


        }
    }
}
