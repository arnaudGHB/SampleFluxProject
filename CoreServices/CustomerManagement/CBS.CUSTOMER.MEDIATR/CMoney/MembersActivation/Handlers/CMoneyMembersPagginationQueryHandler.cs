using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using CBS.CUSTOMER.REPOSITORY.CustomerRepo;
using CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Queries;
using CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Customers based on the CMoneyMembersPagginationQuery.
    /// </summary>
    public class CMoneyMembersPagginationQueryHandler : IRequestHandler<CMoneyMembersPagginationQuery, ServiceResponse<CMoneyMembersActivationAccountsList>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _cMoneyMembersActivationAccountRepository; // Repository for accessing Customers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<CMoneyMembersPagginationQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the CMoneyMembersPagginationQueryHandler.
        /// </summary>
        /// <param name="cMoneyMembersActivationAccountRepository">Repository for C-MONEY member activation account data.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public CMoneyMembersPagginationQueryHandler(
            ICMoneyMembersActivationAccountRepository cMoneyMembersActivationAccountRepository,
            IMapper mapper,
            ILogger<CMoneyMembersPagginationQueryHandler> logger)
        {
            _cMoneyMembersActivationAccountRepository = cMoneyMembersActivationAccountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the CMoneyMembersPagginationQuery to retrieve paginated C-MONEY members.
        /// </summary>
        /// <param name="request">The CMoneyMembersPagginationQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CMoneyMembersActivationAccountsList>> Handle(CMoneyMembersPagginationQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Fetching paginated C-MONEY members for query: {request.CustomerResource}");

                // Fetch paginated members from the repository based on the criteria.
                CMoneyMembersActivationAccountsList membersList;

                if (request.CustomerResource.IsByBranch)
                {
                    _logger.LogInformation($"Fetching members filtered by branch: {request.CustomerResource.BranchId}");
                    membersList = await _cMoneyMembersActivationAccountRepository.GetCustomersAsyncByBranch(request.CustomerResource);
                }
                else
                {
                    _logger.LogInformation("Fetching members without branch filtering.");
                    membersList = await _cMoneyMembersActivationAccountRepository.GetCustomersAsync(request.CustomerResource);
                }

                _logger.LogInformation($"Successfully fetched {membersList.TotalCount} members.");
                return ServiceResponse<CMoneyMembersActivationAccountsList>.ReturnResultWith200(membersList);
            }
            catch (Exception e)
            {
                _logger.LogError($"Failed to get paginated C-MONEY members: {e.Message}");
                return ServiceResponse<CMoneyMembersActivationAccountsList>.Return500(e, "Failed to get paginated C-MONEY members");
            }
        }
    }

}
