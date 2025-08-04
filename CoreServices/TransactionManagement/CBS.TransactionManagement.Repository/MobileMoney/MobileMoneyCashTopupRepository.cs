using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Data.Entity.MobileMoney;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.MobileMoney
{

    public class MobileMoneyCashTopupRepository : GenericRepository<MobileMoneyCashTopup, TransactionContext>, IMobileMoneyCashTopupRepository
    {

        private readonly ILogger<AccountRepository> _logger;
        private readonly UserInfoToken _userInfoToken;


        public MobileMoneyCashTopupRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<AccountRepository> logger, UserInfoToken userInfoToken) : base(unitOfWork)
        {
            _logger = logger;
            _userInfoToken = userInfoToken;
        }
        public async Task<List<MobileMoneyCashTopupDto>> GetMobileMoneyCash(string QuerParamter, bool ByBranch, string BranchId)
        {
            // Fetching data from the repository (assuming this is a base query to start with)
            var query = FindBy(t => !t.IsDeleted); // Filter out deleted entries first

            // Apply branch filter if needed
            if (ByBranch && !string.IsNullOrEmpty(BranchId))
            {
                query = query.Where(t => t.BranchId == BranchId);
            }

            // Apply status filter based on QuerParamter
            if (!string.IsNullOrEmpty(QuerParamter))
            {
                switch (QuerParamter.ToUpper())
                {
                    case "PENDING":
                        query = query.Where(t => t.RequestApprovalStatus == "Pending");
                        break;
                    case "APPROVED":
                        query = query.Where(t => t.RequestApprovalStatus == "Approved");
                        break;
                    case "REJECTED":
                        query = query.Where(t => t.RequestApprovalStatus == "Rejected");
                        break;
                    case "ALL":
                        // No filtering needed, return everything (already handled by initial query)
                        break;
                    default:
                        throw new ArgumentException("Invalid QuerParamter value");
                }
            }

            // Execute the query and return the result as a list
            var result = await query.ToListAsync();

            // If no records are found, log and return an empty list (or handle based on business logic)
            if (result == null || result.Count == 0)
            {
                var message = "No mobile money top-up records found.";
                _logger.LogInformation(message); // Log the info.
                return new List<MobileMoneyCashTopupDto>(); // Return empty list
            }

            // Map the entity to DTO and return the result
            var resultDto = result.Select(t => new MobileMoneyCashTopupDto
            {
                Id = t.Id,
                Amount = t.Amount,
                OperatorType = t.OperatorType,
                SourceType = t.SourceType,
                RequestDate = t.RequestDate,
                BranchId = t.BranchId,
                BankId = t.BankId,
                RequestNote = t.RequestNote,
                RequestInitiatedBy = t.RequestInitiatedBy,
                RequestApprovalDate = t.RequestApprovalDate,
                RequestApprovedBy = t.RequestApprovedBy,
                RequestApprovalStatus = t.RequestApprovalStatus,
                RequestApprovalNote = t.RequestApprovalNote,
                RequestReference = t.RequestReference,
                MobileMoneyTransactionId = t.MobileMoneyTransactionId,
                AccountNumber = t.AccountNumber,
                MobileMoneyMemberReference = t.MobileMoneyMemberReference,
                PhoneNumber = t.PhoneNumber
            }).ToList();

            return resultDto;
        }

    }
}
