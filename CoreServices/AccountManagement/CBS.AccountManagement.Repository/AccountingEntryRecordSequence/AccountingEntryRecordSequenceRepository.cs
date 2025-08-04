using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.AccountManagement.Repository
{
    public class AccountingEntryRecordSequenceRepository : GenericRepository<AccountingEntryRecordSequence, POSContext>, IAccountingEntryRecordSequenceRepository
    {
        public AccountingEntryRecordSequenceRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        
        }

        public async Task<string> GetNextSequenceValueAsync(string sequenceName, string branchCode)
        {
            try
            {
                var today = DateTime.UtcNow.Date; // 

                // Try to find an existing sequence for today
                //var sequence = await this.All
                //    .FirstOrDefaultAsync(s => s.SequenceName == sequenceName &&
                //                              s.BranchCode == branchCode &&
                //                              s.SequenceDate == today && s.IsUsed == true);

                //if (sequence == null)
                //{
                //    // Create a new sequence entry for today
                //    var newSequence = new AccountingEntryRecordSequence
                //    {
                //        SequenceName = sequenceName,
                //        BranchCode = branchCode,
                //        SequenceDate = today,
                //        CurrentValue = $"{branchCode}-0001",
                //        IsUsed = false
                //    };

                //    this.Add(newSequence);
                //    await this.SaveChangesAsync();

                //    return newSequence.CurrentValue;
                //}
                //else
                //{
                //    // Increment the numeric part
                //    var prefix = sequence.CurrentValue.Substring(0, sequence.CurrentValue.LastIndexOf('-') + 1);
                //    var numberPart = int.Parse(sequence.CurrentValue.Substring(sequence.CurrentValue.LastIndexOf('-') + 1)) + 1;

                //    sequence.CurrentValue = $"{prefix}{numberPart:D4}";

                //    await this.SaveChangesAsync();

                //    return sequence.CurrentValue;
                //}
                return "";
            }
            catch (Exception ex)
            {

                throw(ex);
            }
        }
    }
}