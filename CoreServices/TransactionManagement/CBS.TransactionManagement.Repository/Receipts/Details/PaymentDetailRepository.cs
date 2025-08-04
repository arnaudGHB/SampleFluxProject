using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.Receipts.Details;
using CBS.TransactionManagement.Domain;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Dto;

namespace CBS.TransactionManagement.Repository.Receipts.Details
{

    public class PaymentDetailRepository : GenericRepository<PaymentDetail, TransactionContext>, IPaymentDetailRepository
    {
        private readonly ILogger<TellerRepository> _logger;
        private readonly UserInfoToken _userInfoToken;

        public PaymentDetailRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<TellerRepository> logger, UserInfoToken userInfoToken) : base(unitOfWork)
        {
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

    }
}
