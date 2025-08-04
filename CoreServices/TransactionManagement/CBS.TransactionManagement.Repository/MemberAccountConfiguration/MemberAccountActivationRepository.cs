using CBS.NLoan.Repository.FeePolicyP;
using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.FeeP;
using CBS.TransactionManagement.Data.Entity.MemberAccountConfiguration;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.FeeP;
using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.MemberAccountConfiguration
{

    public class MemberAccountActivationRepository : GenericRepository<MemberAccountActivation, TransactionContext>, IMemberAccountActivationRepository
    {
        private readonly ILogger<AccountRepository> _logger; // Logger for logging actions and errors.
        private readonly IFeeRepository _feeRepository; // Repository for accessing member account activation policies.
        private readonly IFeePolicyRepository _feePolicyRepository; // Repository for accessing member account activation policies.
        private readonly IUnitOfWork<TransactionContext> _uow;

        /// <summary>
        /// Constructor for initializing the MemberAccountActivationRepository.
        /// </summary>
        /// <param name="unitOfWork">Unit of work for transaction management.</param>
        /// <param name="memberAccountActivationPolicyRepository">Repository for member account activation policies.</param>
        /// <param name="logger">Logger for logging actions and errors (optional).</param>
        public MemberAccountActivationRepository(
            IUnitOfWork<TransactionContext> unitOfWork,
            ILogger<AccountRepository> logger = null
,
            IFeeRepository feeRepository = null,
            IFeePolicyRepository feePolicyRepository = null,
            IUnitOfWork<TransactionContext> uow = null) : base(unitOfWork)
        {
            // Assign provided dependencies to local variables.
            _logger = logger;
            _feeRepository = feeRepository;
            _feePolicyRepository = feePolicyRepository;
            _uow = uow;
        }

        /// <summary>
        /// Retrieves the member subscription information based on the customer details.
        /// </summary>
        /// <param name="customer">Customer details for which the subscription is to be retrieved.</param>
        /// <returns>A task representing the asynchronous operation, containing the MemberActivationPolicyDto.</returns>
        public async Task<MemberFeePolicyDto> GetMemberSubcription(CustomerDto customer)
        {
            var memberFeePolicies = new MemberFeePolicyDto();
            var feePolicies1 = new List<FeePolicy>();
            // Initialize a new MemberActivationPolicyDto object.
            var memberActivation = new MemberActivationPolicyDto();
            bool isMoralPerson = false;
            if (customer.LegalForm == LegalForms.Moral_Person.ToString())
            {
                isMoralPerson = true;
            }
            // Retrieve the member registration fee policy based on the customer's legal form.
            var fees = await _feeRepository.FindBy(x => x.OperationFeeType == OperationFeeType.MemberShip.ToString() && x.IsMoralPerson == isMoralPerson && x.IsDeleted == false).ToListAsync();
            decimal amount = 0;
            // If no policy is found, log an error and throw an exception.
            if (!fees.Any())
            {
                var errorMessage = $"Member's activation policy is not configured in the system. Please contact your system administration.";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }

            // Retrieve the member account activation based on the customer's ID.
            var memberAccounts = await FindBy(x => x.CustomerId == customer.CustomerId && x.BranchId == customer.BranchId && x.IsDeleted == false).ToListAsync();

            // If no account activation is found, use the maximum values from the policy.
            if (!memberAccounts.Any())
            {
                // Step 1: Get Fee Configuration by Branch
                var feePolicies = await _feePolicyRepository.FindBy(x => x.BranchId == customer.BranchId && x.IsDeleted == false &&x.IsCentralised==false && x.EventCode!=null).Include(x=>x.Fee).ToListAsync();

                // Step 2: If no branch-specific configuration, get centralised fee configuration
                if (!feePolicies.Any())
                {
                    feePolicies = await _feePolicyRepository
                        .FindBy(x => x.IsCentralised && !x.IsDeleted && x.Fee.IsMoralPerson == isMoralPerson && x.EventCode!=null)
                        .GroupBy(x => x.FeeId) // Group by FeeId
                        .Select(g => g.FirstOrDefault()) // Select the first record from each group
                        .ToListAsync();

                    // Step 3: If no centralised configuration either, log error and throw exception
                    if (!feePolicies.Any())
                    {
                        var errorMessage = $"Membership policy is not configured for branch [{customer.BranchCode}] [{customer.BranchName}], and no centralised or harmonize policy exists. Please contact your system administrator.";
                        _logger.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }
                }

                // Process Fee Policies
                var all = (from a in feePolicies join b in fees on a.FeeId equals b.Id select a ).ToList();
                amount = all.Sum(x => x.Charge);
                feePolicies1 = all;
                // Create Member Account Activation Entries
                var memberAccountActivations = new List<MemberAccountActivation>();

                foreach (var feePolicy in all)
                {
                    var memberAccount = new MemberAccountActivation
                    {
                        Amount = feePolicy.Charge,
                        AmountPaid = 0,
                        Balance = feePolicy.Charge,
                        BankId = feePolicy.BankId,
                        BranchId = feePolicy.BranchId,
                        CustomerId = customer.CustomerId,
                        FeeId = feePolicy.FeeId,
                        FeeName = fees.Where(x => x.Id == feePolicy.FeeId).FirstOrDefault()?.Name,
                        Status = false,
                        Fee = fees.Where(x => x.Id == feePolicy.FeeId).FirstOrDefault(),
                        Id = BaseUtilities.GenerateUniqueNumber(),
                        CustomeAmount = feePolicy.Charge
                    };
                    memberAccountActivations.Add(memberAccount);
                    memberFeePolicies.Policies.Add(new FeePolicy { EventCode = feePolicy.EventCode, Fee= memberAccount.Fee, FeeId=feePolicy.FeeId,AmountFrom=feePolicy.AmountFrom, AmountTo=feePolicy.AmountTo, BankId=feePolicy.BankId, BranchId=feePolicy.BranchId, Charge=feePolicy.Charge, Id=feePolicy.Id, Value=feePolicy.Value });
                }
                memberFeePolicies.MemberAccountActivations = memberAccountActivations;
                memberFeePolicies.Amount = amount;
                // Add activations to repository and save
                //AddRange(memberAccountActivations);
                //await _uow.SaveAsync();
                return memberFeePolicies;
            }
            else
            {
                // If member accounts already exist, sum the custom amounts
                amount = memberAccounts.Sum(x => x.CustomeAmount);
            }
            memberFeePolicies.Amount = amount;
            memberFeePolicies.Policies = feePolicies1;
            // Return the member activation details.
            return memberFeePolicies;
        }

    }
}
