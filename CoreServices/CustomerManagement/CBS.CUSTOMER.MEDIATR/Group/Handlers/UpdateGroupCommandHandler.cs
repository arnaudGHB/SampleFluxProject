using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using System;

namespace CBS.Group.MEDIATR.GroupMediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a Group based on UpdateGroupCommand.
    /// </summary>
    public class UpdateGroupCommandHandler : IRequestHandler<UpdateGroupCommand, ServiceResponse<UpdateGroup>>
    {
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Group data.
        private readonly ILogger<UpdateGroupCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly IMediator _mediator;
        private readonly ICustomerRepository _customerRepository;
        private readonly UserInfoToken _userInfoToken;

        /// <summary>
        /// Constructor for initializing the UpdateGroupCommandHandler.
        /// </summary>
        /// <param name="GroupRepository">Repository for Group data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateGroupCommandHandler(
            IGroupRepository GroupRepository,
            ILogger<UpdateGroupCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null,
            IMediator mediator = null,
            UserInfoToken userInfoToken = null,
            ICustomerRepository customerRepository = null)
        {
            _GroupRepository = GroupRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _mediator = mediator;
            _userInfoToken = userInfoToken;
            _customerRepository = customerRepository;
        }

        /// <summary>
        /// Handles the UpdateGroupCommand to update a Group.
        /// </summary>
        /// <param name="request">The UpdateGroupCommand containing updated Group data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UpdateGroup>> Handle(UpdateGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the Group entity to be updated from the repository
                var existingGroup = await _GroupRepository.FindAsync(request.GroupId);
                var customer = await _customerRepository.FindAsync(existingGroup.GroupLeaderId);
                //UpdateCustomerMoralPersonCommand
                var updateCustomerMoralPerson = new UpdateCustomerMoralPersonCommand
                {
                    Active = request.Active,
                    CustomerId = existingGroup.GroupLeaderId,
                    FirstName = request.GroupName,
                    ActiveStatus = request.Active ? customer.ActiveStatus : ActiveStatus.InActive.ToString(),
                    RegistrationNumber = request.RegistrationNumber,
                    TaxIdentificationNumber = request.TaxPayerNumber
                };
                // Add the customer
                var result = await _mediator.Send(updateCustomerMoralPerson, cancellationToken);
                // If adding the customer fails, log the error and return a 500 Internal Server Error response
                if (result.StatusCode != 200)
                {
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, result.Message, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                    return ServiceResponse<UpdateGroup>.Return500(result.Message);
                }
                // Check if the Group entity exists
                if (existingGroup != null)
                {
                    _mapper.Map(request, existingGroup);
                    _GroupRepository.Update(existingGroup);
                    await _uow.SaveAsync();
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UpdateGroup>.ReturnResultWith200(_mapper.Map<UpdateGroup>(existingGroup));
                    _logger.LogInformation($"Group {request.GroupId} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Group entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.GroupId} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UpdateGroup>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Group: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UpdateGroup>.Return500(e);
            }
        }
    }

}
