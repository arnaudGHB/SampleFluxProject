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
            IUnitOfWork<POSContext> uow = null)
        {
            _GroupRepository = GroupRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
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
                var existingGroup = await _GroupRepository.FindAsync(request.Id);

                // Check if the Group entity exists
                if (existingGroup != null)
                {
                    // Update Group entity properties with values from the request
                  /*  existingGroup.Name = request.Name;
                    existingGroup.Address = request.Address;
                    existingGroup.Telephone = request.Telephone;
                    existingGroup.CreatedBy = request.UserID;*/
                    // Use the repository to update the existing Group entity
                    _GroupRepository.Update(existingGroup);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<UpdateGroup>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UpdateGroup>.ReturnResultWith200(_mapper.Map<UpdateGroup>(existingGroup));
                    _logger.LogInformation($"Group {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Group entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
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
