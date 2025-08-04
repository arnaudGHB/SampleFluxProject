using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository
{
   
    public class TellerRepository : GenericRepository<Teller, TransactionContext>, ITellerRepository
    {
        private readonly ILogger<TellerRepository> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        public TellerRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<TellerRepository> logger = null, UserInfoToken userInfoToken = null) : base(unitOfWork)
        {
            _logger = logger;
            _userInfoToken = userInfoToken;
        }
        // Method to retrieve teller information based on the provided TellerProvisioningStatus
        public async Task<Teller> RetrieveTeller(DailyTeller tellerProvision)
        {
            // Find the teller with the specified ID that is not primary
            var teller = await FindBy(t => t.Id == tellerProvision.TellerId).FirstOrDefaultAsync();
            // Check if teller exists
            if (teller == null)
            {
                // If the teller does not exist, log an error and throw an exception
                var errorMessage = $"Teller with ID '{tellerProvision.TellerId}' does not exist or is not active.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Return the retrieved teller
            return teller;
        }
      
        // Method to retrieve teller information based on the access code
        public async Task<Teller> GetTellerByCode(string applicationCode, string branchCode,string branchName)
        {
            string tppCode = $"{applicationCode.ToUpper()}-{branchCode}";
            // Find the teller with the provided access code that is active and not primary
            var teller = await FindBy(x => x.Code.ToUpper() == tppCode && x.ActiveStatus && !x.IsPrimary).FirstOrDefaultAsync();
            // Check if teller exists
            if (teller == null)
            {
                // If the teller does not exist, log an error and throw an exception
                var errorMessage = $"TPP with application code: {applicationCode} does not exist for branch {branchName}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            if (!teller.ActiveStatus)
            {
                var errorMessage = $"TPP with application code: {applicationCode} status is inactive for branch {branchName}";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.

            }
            // Return the retrieved teller
            return teller;
        }
        private async Task<Teller> GetTellerByCode(string applicationCode)
        {
            // Find the teller with the provided access code that is active and not primary
            var teller = await FindBy(x => x.Code == applicationCode && x.ActiveStatus && !x.IsPrimary).FirstOrDefaultAsync();
            // Check if teller exists
            if (teller == null)
            {
                // If the teller does not exist, log an error and throw an exception
                var errorMessage = $"TPP with application code: {applicationCode} does not exist.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Return the retrieved teller
            return teller;
        }
        public async Task<Teller> GetTellerByOperationType(string SourceType)
        {
            // Find the teller with the provided access code that is active and not primary
            var teller = await FindBy(x => x.OperationType == SourceType && x.ActiveStatus && !x.IsPrimary && x.BranchId==_userInfoToken.BranchID).FirstOrDefaultAsync();
            // Check if teller exists
            if (teller == null)
            {
                // If the teller does not exist, log an error and throw an exception
                var errorMessage = $"Teller operation type: {SourceType} does not exist.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Return the retrieved teller
            return teller;
        }
  
        public async Task<Teller> GetTellerByOperationType(string SourceType,string branchid)
        {
            // Find the teller with the provided access code that is active and not primary
            var teller = await FindBy(x => x.OperationType == SourceType && x.ActiveStatus && !x.IsPrimary && x.BranchId==branchid).FirstOrDefaultAsync();
            // Check if teller exists
            if (teller == null)
            {
                // If the teller does not exist, log an error and throw an exception
                var errorMessage = $"Teller operation type: {SourceType} does not exist.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Return the retrieved teller
            return teller;
        }
        public async Task<Teller> GetTellerByType(string tellerType, string branchid)
        {
            // Find the teller with the provided access code that is active and not primary
            var teller = await FindBy(x => x.TellerType == tellerType && x.ActiveStatus && !x.IsPrimary && x.BranchId == branchid).FirstOrDefaultAsync();
            // Check if teller exists
            if (teller == null)
            {
                // If the teller does not exist, log an error and throw an exception
                var errorMessage = $"Teller type: {tellerType} does not exist.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Return the retrieved teller
            return teller;
        }
        public async Task<Teller> GetTellerByType(string tellerType)
        {
            // Find the teller with the provided access code that is active and not primary
            var teller = await FindBy(x => x.TellerType == tellerType && x.ActiveStatus && !x.IsPrimary && x.BranchId == _userInfoToken.BranchID).FirstOrDefaultAsync();
            // Check if teller exists
            if (teller == null)
            {
                // If the teller does not exist, log an error and throw an exception
                var errorMessage = $"Teller type: {tellerType} does not exist.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Return the retrieved teller
            return teller;
        }
        public async Task<bool> ValidateTellerLimites(decimal requestedAmount, decimal totalCashDeposited, Teller teller)
        {
            if (requestedAmount != totalCashDeposited)
            {
                var errorMessage = $"Amount enter is not equal to denomination total.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            if (requestedAmount > teller.MaximumAmountToManage)
            {
                var errorMessage = $"Amount enter is greater than teller maximum limit.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
            if (requestedAmount < teller.MinimumAmountToManage)
            {
                var errorMessage = $"Amount enter is lesser than teller minimum limit";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            return true;
        }

        public async Task<Teller> GetPrimaryTeller(string BranchId)
        {
            // Find the teller with the provided access code that is active and not primary
            var teller = await FindBy(x => x.BranchId == BranchId && x.IsPrimary).FirstOrDefaultAsync();
            // Check if teller exists
            if (teller == null)
            {
                // If the teller does not exist, log an error and throw an exception
                var errorMessage = $"Teller does not exist.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            if (!teller.ActiveStatus)
            {
                // If the teller does not exist, log an error and throw an exception
                var errorMessage = $"Primary teller {teller.Name} is inactive.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Return the retrieved teller
            return teller;
        }
        public async Task<Teller> GetTeller(string tellerId)
        {
            // Find teller
            var teller = await FindAsync(tellerId);
            // Check if teller exists
            if (teller == null)
            {
                // If the teller does not exist, log an error and throw an exception
                var errorMessage = $"Teller does not exist.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            if (!teller.ActiveStatus)
            {
                // If the teller does not exist, log an error and throw an exception
                var errorMessage = $"Teller {teller.Name} is inactive.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // Return the retrieved teller
            return teller;
        }

        // Method to check if a transfer operation is within the teller's scope based on access code and transfer amount
        public async Task<bool> CheckIfTransferIsWithinTellerScope(string accessCode, decimal amount)
        {
            // Retrieve the teller information based on the access code
            var teller = await GetTellerByCode(accessCode);
            // Check if the transfer amount exceeds the teller's maximum transfer amount
            if (teller.MaximumTransferAmount < amount)
            {
                // If the transfer amount exceeds the maximum, log an error and throw an exception
                var errorMessage = $"Teller {teller.Name} does not have sufficient rights to transfer this amount. Maximum transfer amount is {BaseUtilities.FormatCurrency(teller.MaximumTransferAmount)}.";
                _logger.LogError(errorMessage); // Log error message.
                throw new InvalidOperationException(errorMessage); // Throw exception.
            }
            // If the transfer is within the teller's scope, return true
            return true;
        }
       
        public async Task<bool> CheckTellerOperationalRights(Teller teller, string operationType, bool isCashOperation)
        {
            // Validate teller's rights for deposit operations
            if (operationType == OperationType.Deposit.ToString())
            {
                if (!teller.PerformCashIn)
                {
                    var errorMessage = $"Teller {teller.Name} is not allowed to perform deposits. Please contact the system administrator.";
                    _logger.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw exception
                }
            }
            // Validate teller's rights for withdrawal operations
            else if (operationType == OperationType.Withdrawal.ToString())
            {
                if (!teller.PerformCashOut)
                {
                    var errorMessage = $"Teller {teller.Name} is not allowed to perform withdrawals. Please contact the system administrator.";
                    _logger.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw exception
                }
            }
            // Validate teller's rights for transfer operations
            else if (operationType == OperationType.Transfer.ToString())
            {
                if (!teller.PerformTransfer)
                {
                    var errorMessage = $"Teller {teller.Name} is not allowed to perform transfers. Please contact the system administrator.";
                    _logger.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw exception
                }
            }

            // Validate rights for cash operations
            if (isCashOperation)
            {
                if (teller.OperationType == OperationType.NoneCash.ToString())
                {
                    var errorMessage = $"Teller {teller.Name} is not allowed to perform cash operations. Please contact the system administrator.";
                    _logger.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw exception
                }
            }
            // Validate rights for non-cash operations
            else
            {
                if (teller.OperationType == OperationType.Cash.ToString())
                {
                    var errorMessage = $"Teller {teller.Name} is not allowed to perform non-cash operations. Please contact the system administrator.";
                    _logger.LogError(errorMessage); // Log error message
                    throw new InvalidOperationException(errorMessage); // Throw exception
                }
            }

            // If all validations pass, return true
            return true;
        }
    }

}
