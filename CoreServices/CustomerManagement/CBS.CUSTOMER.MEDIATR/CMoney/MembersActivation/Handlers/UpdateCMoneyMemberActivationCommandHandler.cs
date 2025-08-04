using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DATA.Dto.CMoney;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY.CMoney.MembersActivation;
using CBS.CustomerSmsConfigurations.MEDIAT.CMoney.MembersActivation;
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
    /// Handles the command to update a C-MONEY Member Activation.
    /// </summary>
    public class UpdateCMoneyMemberActivationCommandHandler : IRequestHandler<UpdateCMoneyMemberActivationCommand, ServiceResponse<bool>>
    {
        private readonly ICMoneyMembersActivationAccountRepository _CMoneyMembersActivationAccountRepository; // Repository for accessing C-MONEY activation data.
        private readonly ILogger<UpdateCMoneyMemberActivationCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IMapper _mapper; // AutoMapper for object mapping.

        /// <summary>
        /// Constructor for initializing the UpdateCMoneyMemberActivationCommandHandler.
        /// </summary>
        /// <param name="CMoneyMembersActivationAccountRepository">Repository for C-MONEY activation account data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="uow">Unit of Work for transaction management.</param>
        public UpdateCMoneyMemberActivationCommandHandler(
            ICMoneyMembersActivationAccountRepository CMoneyMembersActivationAccountRepository,
            ILogger<UpdateCMoneyMemberActivationCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            IMapper mapper)
        {
            _CMoneyMembersActivationAccountRepository = CMoneyMembersActivationAccountRepository;
            _logger = logger;
            _uow = uow;
            _mapper = mapper;
        }

        /// <summary>
        /// Handles the UpdateCMoneyMemberActivationCommand to update C-MONEY activation details for a member.
        /// </summary>
        /// <param name="request">The command containing updated activation data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A service response indicating success or failure.</returns>
        public async Task<ServiceResponse<bool>> Handle(UpdateCMoneyMemberActivationCommand request, CancellationToken cancellationToken)
        {
            string message = string.Empty;
            try
            {
                _logger.LogInformation($"Starting update process for member activation ID {request.Id}.");

                // Retrieve the member's activation account
                var memberActivation = await _CMoneyMembersActivationAccountRepository.FindAsync(request.Id);
                if (memberActivation == null)
                {
                    message = $"C-MONEY member activation not found for ID {request.Id}.";
                    _logger.LogWarning(message);
                    await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.NotFound, LogAction.CMoneyUpdateActivation, LogLevelInfo.Warning);
                    return ServiceResponse<bool>.Return404(message);
                }

                _logger.LogInformation($"Member activation found for ID {request.Id}. Updating details.");

                // Update the member's activation details
                memberActivation.PhoneNumber = request.PhoneNumber;
                memberActivation.IsActive = request.IsActive;
                memberActivation.IsSubcribed = request.IsSubcribed;

                // Update the repository
                _CMoneyMembersActivationAccountRepository.Update(memberActivation);

                // Save changes using Unit of Work
                await _uow.SaveAsync();

                // Map the updated entity to a DTO and return a success response
                message = "C-MONEY member profile successfully updated.";
                _logger.LogInformation(message);
                await BaseUtilities.LogAndAuditAsync(message, request, HttpStatusCodeEnum.OK, LogAction.CMoneyUpdateActivation, LogLevelInfo.Information);
                return ServiceResponse<bool>.ReturnResultWith200(true, message);
            }
            catch (Exception e)
            {
                // Log error and return an error response
                var errorMessage = $"Error occurred while updating activation for member ID {request.Id}: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.CMoneyUpdateActivation, LogLevelInfo.Error);
                return ServiceResponse<bool>.Return500(errorMessage);
            }
        }
    }


}
