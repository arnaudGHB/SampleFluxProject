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
using System.Threading;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository
{
   
    public class TellerOperationRepository : GenericRepository<TellerOperation, TransactionContext>, ITellerOperationRepository
    {
        private readonly ILogger<AccountRepository> _logger; // Logger for logging handler actions and errors.

        public TellerOperationRepository(IUnitOfWork<TransactionContext> unitOfWork, ILogger<AccountRepository> logger = null) : base(unitOfWork)
        {
            _logger = logger;
        }
        public async Task<List<TellerOperationGL>> GetTellerLastOperations(string tellerId, DateTime accountingDate)
        {
            try
            {
                // Find the latest date of operations for the given teller
                //var latestDate = await FindBy(x => x.TellerID == tellerId && x.Date.Value.Year==DateTime.Now.Year && x.OpenOfDayReference==openOfDayReference).OrderByDescending(x => x.Date).Select(x => x.Date).FirstOrDefaultAsync();
                //if (latestDate == null)
                //{
                //    return new List<TellerOperationGL>();
                //}

                // Retrieve operations for the latest date
                var query = FindBy(x => x.TellerID == tellerId && x.Date.Value.Date== accountingDate.Date).AsQueryable();
                var tellerOperations = await query.OrderBy(x => x.CreatedDate).ToListAsync();

                // Initialize variables for processing
                List<TellerOperationGL> glList = new List<TellerOperationGL>();
                DateTime currentDay = DateTime.MinValue;
                decimal runningBalance = 0;

                // Process each TellerOperation
                foreach (var tellerOperation in tellerOperations)
                {
                    if (tellerOperation.Date.HasValue)
                    {
                        // Check if we are in a new day
                        if (tellerOperation.Date.Value.Date != currentDay.Date)
                        {
                            currentDay = tellerOperation.Date.Value.Date;
                            runningBalance = tellerOperation.PreviousBalance;
                        }
                        // Create a new TellerOperationGL object
                        TellerOperationGL gl = new TellerOperationGL
                        {
                            Amount = tellerOperation.Amount,
                            AccountNumber = tellerOperation.AccountNumber,
                            TransactionType = tellerOperation.TransactionType,
                            TransactionRef = tellerOperation.TransactionReference,
                            BalanceBF = runningBalance,
                            Debit = tellerOperation.OperationType.ToLower() == "debit" ? Math.Abs(tellerOperation.Amount) : 0,
                            Credit = tellerOperation.OperationType.ToLower() == "credit" ? Math.Abs(tellerOperation.Amount) : 0,
                            Naration = $"{tellerOperation.TransactionType}, {tellerOperation.CustomerId}, {tellerOperation.MemberName}, {tellerOperation.TransactionReference}",
                            TellerID = tellerOperation.TellerID,
                            Date = tellerOperation.Date.Value,
                            Description = tellerOperation.Description,
                            MemberAccountNumber = tellerOperation.MemberAccountNumber,
                            MemberId = tellerOperation.CustomerId,
                            BranchId = tellerOperation.BranchId,
                            DailyReferences = tellerOperation.ReferenceId,
                            MemberName = tellerOperation.MemberName,
                        };
                        // Update running balance
                        runningBalance += gl.Credit - gl.Debit;
                        decimal balance = tellerOperation.Amount + tellerOperation.PreviousBalance;
                        //gl.Balance = balance;
                        gl.Balance = BaseUtilities.RoundUpToOne(runningBalance);
                        // Add the new object to the list
                        glList.Add(gl);
                    }
                }


                //List<TellerOperationGL> glList = new List<TellerOperationGL>();
                //DateTime currentDay = DateTime.MinValue;
                //decimal runningBalance = 0;

                //var groupedEntities = entities.GroupBy(e => e.Date.Value.Date).OrderBy(g => g.Key);

                //foreach (var group in groupedEntities)
                //{
                //    foreach (var entity in group)
                //    {
                //        if (entity.Date.HasValue)
                //        {
                //            // Check if we are in a new day
                //            if (entity.Date.Value.Date != currentDay.Date)
                //            {
                //                currentDay = entity.Date.Value.Date;
                //                runningBalance = entity.PreviousBalance;
                //            }

                //            TellerOperationGL gl = new TellerOperationGL
                //            {
                //                Amount = entity.Amount,
                //                AccountNumber = entity.AccountNumber,
                //                TransactionType = entity.TransactionType,
                //                TransactionRef = entity.TransactionReference,
                //                BalanceBF = runningBalance,
                //                Debit = entity.OperationType.ToLower() == "debit" ? Math.Abs(entity.Amount) : 0,
                //                Credit = entity.OperationType.ToLower() == "credit" ? Math.Abs(entity.Amount) : 0,
                //                TellerID = entity.TellerID,
                //                Date = entity.Date.Value,
                //                BranchId = entity.BranchId
                //            };

                //            runningBalance += gl.Credit - gl.Debit;
                //            gl.Balance = runningBalance;

                //            glList.Add(gl);
                //        }
                //    }
                //}

                return glList;
            }
            catch (Exception e)
            {
                var errorMessage = $"Failed getting teller operations. Error: {e.Message}";
                _logger.LogError(errorMessage);
                // Log an audit trail for the error
                throw new InvalidOperationException(errorMessage);
            }
        }

    }
}
