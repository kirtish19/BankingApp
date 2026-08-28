namespace BankingApp.CustomerKycProcessorFunction.Services
{
    public interface IMetaDataProcessorService
    {
        public Task ProcessMetaData(CustomerKYCMessage message);
    }
}
