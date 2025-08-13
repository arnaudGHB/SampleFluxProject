using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.RemittanceP;

namespace CBS.TransactionManagement.Repository.RemittanceP
{
    public interface IRemittanceRepository : IGenericRepository<Remittance>
    {
        string ValidateReceiverIdentity(Remittance remittance, string receiverName, string receiverAddress, string receiverPhoneNumber, string secreteCode);

        string ValidateSenderIdentity(Remittance remittance, string senderName, string senderPhoneNumber, string secreteCode, decimal amount, DateTime transactionDate);
    }
}
