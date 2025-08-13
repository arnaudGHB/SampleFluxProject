using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Data.Entity.ChangeManagement;
using DocumentFormat.OpenXml.Bibliography;

namespace CBS.TransactionManagement.Repository
{

    public class CashChangeHistoryRepository : GenericRepository<CashChangeHistory, TransactionContext>, ICashChangeHistoryRepository
    {
        private readonly ILogger<CashChangeHistoryRepository> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        public CashChangeHistoryRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<CashChangeHistoryRepository> logger = null, UserInfoToken userInfoToken = null) : base(unitOfWork)
        {
            _logger = logger;
            _userInfoToken = userInfoToken;
        }
        public void CreateChangeHistory(CashChangeDataCarrier changeManagement, CashChangeHistory changeHistory)
        {
            // Step 9: Create a `CashChangeHistoryDto` object to log the change operation.
            var cashChangeHistory = new CashChangeHistory
            {
                Id = BaseUtilities.GenerateUniqueNumber(),
                ChangeDate = BaseUtilities.UtcNowToDoualaTime(),
                GivenNote10000 = changeManagement.denominationsGiven.Note10000,
                GivenNote5000 = changeManagement.denominationsGiven.Note5000,
                GivenNote2000 = changeManagement.denominationsGiven.Note2000,
                GivenNote1000 = changeManagement.denominationsGiven.Note1000,
                GivenNote500 = changeManagement.denominationsGiven.Note500,
                GivenCoin500 = changeManagement.denominationsGiven.Coin500,
                GivenCoin100 = changeManagement.denominationsGiven.Coin100,
                GivenCoin50 = changeManagement.denominationsGiven.Coin50,
                GivenCoin25 = changeManagement.denominationsGiven.Coin25,
                GivenCoin10 = changeManagement.denominationsGiven.Coin10,
                GivenCoin5 = changeManagement.denominationsGiven.Coin5,
                GivenCoin1 = changeManagement.denominationsGiven.Coin1,
                ReceivedNote10000 = changeManagement.denominationsReceived.Note10000,
                ReceivedNote5000 = changeManagement.denominationsReceived.Note5000,
                ReceivedNote2000 = changeManagement.denominationsReceived.Note2000,
                ReceivedNote1000 = changeManagement.denominationsReceived.Note1000,
                ReceivedNote500 = changeManagement.denominationsReceived.Note500,
                ReceivedCoin500 = changeManagement.denominationsReceived.Coin500,
                ReceivedCoin100 = changeManagement.denominationsReceived.Coin100,
                ReceivedCoin50 = changeManagement.denominationsReceived.Coin50,
                ReceivedCoin25 = changeManagement.denominationsReceived.Coin25,
                ReceivedCoin10 = changeManagement.denominationsReceived.Coin10,
                ReceivedCoin5 = changeManagement.denominationsReceived.Coin5,
                ReceivedCoin1 = changeManagement.denominationsReceived.Coin1,
                BranchId = _userInfoToken.BranchID,
                BranchCode = _userInfoToken.BranchCode,
                BranchName = _userInfoToken.BranchName,
                ChangedBy = _userInfoToken.FullName,
                Reference=changeHistory.Reference,
                SubTellerId=changeHistory.SubTellerId,
                PrimaryTellerId=changeHistory.PrimaryTellerId,
                VaultId=changeHistory.VaultId,
                AmountGiven=changeHistory.AmountGiven,
                AmountReceive=changeHistory.AmountReceive,
                ChangeReason=changeHistory.ChangeReason,
                ServiceOperationType=changeHistory.ServiceOperationType,
                SystemName=changeManagement.SystemName
            };

            Add(cashChangeHistory);
        }
    }

}
