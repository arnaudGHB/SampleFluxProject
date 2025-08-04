using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;

namespace CBS.GroupType.MEDIATR.GroupCustomerMediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a GroupType based on UpdateGroupTypeCommand.
    /// </summary>
    public class UpdateGroupTypeCommandHandler : IRequestHandler<UpdateGroupTypeCommand, ServiceResponse<UpdateGroupTypeDto>>
    {
        private readonly IGroupTypeRepository _GroupTypeRepository; // Repository for accessing GroupType data.
        private readonly ILogger<UpdateGroupTypeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateGroupTypeCommandHandler.
        /// </summary>
        /// <param name="GroupCustomerRepository">Repository for GroupType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateGroupTypeCommandHandler(
            IGroupTypeRepository GroupTypeRepository,
            ILogger<UpdateGroupTypeCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _GroupTypeRepository = GroupTypeRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateGroupTypeCommand to update a GroupType.
        /// </summary>
        /// <param name="request">The UpdateGroupTypeCommand containing updated GroupType data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UpdateGroupTypeDto>> Handle(UpdateGroupTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the GroupType entity to be updated from the repository
                var existingGroupType = await _GroupTypeRepository.FindAsync(request.Id);

                // Check if the GroupType entity exists
                if (existingGroupType != null)
                {
                    existingGroupType.Description=request.Description;
                    
                    _GroupTypeRepository.Update(existingGroupType);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<UpdateGroupTypeDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UpdateGroupTypeDto>.ReturnResultWith200(_mapper.Map<UpdateGroupTypeDto>(existingGroupType));
                    _logger.LogInformation($"GroupType {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the GroupType entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UpdateGroupTypeDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating GroupType: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UpdateGroupTypeDto>.Return500(e);
            }
        }
    }

}
