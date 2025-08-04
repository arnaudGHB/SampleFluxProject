using AutoMapper;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Queries;
using CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Handlers
{
    /// <summary>
    /// Handles the query to retrieve C-MONEY Member Activations based on various filters.
    /// </summary>
    public class GetCMoneyMemberActivationsQueryHandler : IRequestHandler<GetCMoneyMemberActivationsQuery, ServiceResponse<List<CMoneyMembersActivationAccountDto>>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _CMoneyMembersActivationAccountRepository; // Repository for accessing C-MONEY activation data.
        private readonly IMapper _mapper; // AutoMapper for mapping entities to DTOs.
        private readonly ILogger<GetCMoneyMemberActivationsQueryHandler> _logger; // Logger for logging actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCMoneyMemberActivationsQueryHandler.
        /// </summary>
        /// <param name="CMoneyMembersActivationAccountRepository">Repository for C-MONEY activation account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        public GetCMoneyMemberActivationsQueryHandler(
            ICMoneyMembersActivationAccountRepository CMoneyMembersActivationAccountRepository,
            IMapper mapper,
            ILogger<GetCMoneyMemberActivationsQueryHandler> logger)
        {
            _CMoneyMembersActivationAccountRepository = CMoneyMembersActivationAccountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetCMoneyMemberActivationsQuery to retrieve activations based on filters.
        /// </summary>
        /// <param name="request">The query containing filters like Branch, Date range, User, Active/Deactivated status, etc.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response containing the list of activation details or an error message.</returns>
        public async Task<ServiceResponse<List<CMoneyMembersActivationAccountDto>>> Handle(GetCMoneyMemberActivationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Start building the query as IQueryable for optimization
                var query = _CMoneyMembersActivationAccountRepository.All.AsNoTracking();

                // Apply filters based on request parameters
                if (request.ByBranch && !string.IsNullOrEmpty(request.ParameterString))
                {
                    query = query.Where(x => x.BranchCode == request.ParameterString);
                }

                if (request.ByUser && !string.IsNullOrEmpty(request.ParameterString))
                {
                    query = query.Where(x => x.CreatedBy == request.ParameterString);
                }

                if (request.ByDate && request.StartDate.HasValue && request.EndDate.HasValue)
                {
                    query = query.Where(x => x.ActivationDate >= request.StartDate && x.ActivationDate <= request.EndDate);
                }

                if (request.IsActive)
                {
                    query = query.Where(x => x.IsActive);
                }

                if (request.IsDeactivated)
                {
                    query = query.Where(x => !x.IsActive);
                }

                // Execute the query and fetch results
                var activations = await query.ToListAsync(cancellationToken);

                // Map entities to DTOs
                var activationDtos = _mapper.Map<List<CMoneyMembersActivationAccountDto>>(activations);

                return ServiceResponse<List<CMoneyMembersActivationAccountDto>>.ReturnResultWith200(activationDtos);
            }
            catch (Exception e)
            {
                // Log error and return an error response
                var errorMessage = $"Error occurred while retrieving activations: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<List<CMoneyMembersActivationAccountDto>>.Return500(errorMessage);
            }
        }
    }




}
