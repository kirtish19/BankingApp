using BankingApp.CustomerApi.Extensions.Mappings;
using BankingApp.Data.Tables;
using BankingApp.Shared.Helpers;
using Microsoft.Azure.Amqp.Framing;

namespace BankingApp.CustomerApi.Services
{
    public class UserService(IUnitOfWork unitOfWork,IStorageHandler storageHandler,IServiceBusHandler<User> serviceBusHandler) : IUserService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IStorageHandler _storageHandler = storageHandler;
        private readonly IServiceBusHandler<User> _serviceBusHandler = serviceBusHandler;

        public async Task RegisterUserAsync(PostUserRegisterationRequest request)
        {
            //TODO - call customer repo only when user type is customer but user repo is always called.
            // upload the documents to blog storage
            // push a message to ASB topic.
            // User is always created
            var user = request.ToUser();

            await _unitOfWork.Users.AddAsync(user);

            // Customer is created only for Customer registration
            if (request.UserType == UserType.Customer)
            {
                var customer = request.ToCustomer(user.Id);
                await _unitOfWork.Customers.AddAsync(customer);
            }

            if (request.KycDocuments is not null && request.KycDocuments.Any())
            {
                var storageConnectionString = Environment.GetEnvironmentVariable("STORAGE_CONNECTION_STRING");
                var containerName = Environment.GetEnvironmentVariable("STORAGE_CONTAINER_NAME");
                await _storageHandler.UploadBlobAsync(storageConnectionString, containerName, request.KycDocuments);
            }
                // Commit everything together
            await _unitOfWork.TransactionManager.SaveChangesAsync();

            var message = user;
            var topicName = Environment.GetEnvironmentVariable("SERVICE_BUS_TOPIC_NAME");
            var serviceBusConnectionString = Environment.GetEnvironmentVariable("SERVICE_BUS_CONNECTION_STRING");
            await _serviceBusHandler.SendMessageToQueueOrTopic(message, topicName, serviceBusConnectionString);

        }
    }
}
