using AutoMapper;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Queries;
using CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.CUSTOMER.MEDIATR.CMoney.MembersActivation.Handlers
{
    /// <summary>
    /// Handles the query to retrieve a C-MONEY Member Activation by ID.
    /// </summary>
    public class GetCMoneyMemberActivationByIdQueryHandler : IRequestHandler<GetCMoneyMemberActivationByIdQuery, ServiceResponse<CMoneyMembersActivationAccountDto>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _CMoneyMembersActivationAccountRepository; // Repository for accessing C-MONEY activation data.
        private readonly IMapper _mapper; // AutoMapper for mapping entities to DTOs.
        private readonly ILogger<GetCMoneyMemberActivationByIdQueryHandler> _logger; // Logger for logging actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCMoneyMemberActivationByIdQueryHandler.
        /// </summary>
        /// <param name="CMoneyMembersActivationAccountRepository">Repository for C-MONEY activation account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging actions and errors.</param>
        public GetCMoneyMemberActivationByIdQueryHandler(
            ICMoneyMembersActivationAccountRepository CMoneyMembersActivationAccountRepository,
            IMapper mapper,
            ILogger<GetCMoneyMemberActivationByIdQueryHandler> logger)
        {
            _CMoneyMembersActivationAccountRepository = CMoneyMembersActivationAccountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetCMoneyMemberActivationByIdQuery to retrieve member activation details by ID.
        /// </summary>
        /// <param name="request">The query containing the ID of the activation record.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response containing the activation details or an error message.</returns>
        public async Task<ServiceResponse<CMoneyMembersActivationAccountDto>> Handle(GetCMoneyMemberActivationByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the member's activation account by ID
                var memberActivation = await _CMoneyMembersActivationAccountRepository.FindAsync(request.Id);
                if (memberActivation == null)
                {
                    var message = $"C-MONEY member activation not found for ID {request.Id}.";
                    _logger.LogInformation(message);
                    return ServiceResponse<CMoneyMembersActivationAccountDto>.Return404(message);
                }

                // Map the entity to a DTO
                var activationDto = _mapper.Map<CMoneyMembersActivationAccountDto>(memberActivation);

                return ServiceResponse<CMoneyMembersActivationAccountDto>.ReturnResultWith200(activationDto);
            }
            catch (Exception e)
            {
                // Log error and return an error response
                var errorMessage = $"Error occurred while retrieving activation for ID {request.Id}: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CMoneyMembersActivationAccountDto>.Return500(errorMessage);
            }
        }
    }

   

}
