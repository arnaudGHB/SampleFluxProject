using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;

namespace CBS.GroupCustomer.MEDIATR.GroupCustomerMediatR.Handlers
{

    /// <summary>
    /// Handles the command to update a GroupCustomer based on UpdateGroupCustomerCommand.
    /// </summary>
    public class UpdateGroupCustomerCommandHandler : IRequestHandler<UpdateGroupCustomerCommand, ServiceResponse<UpdateGroupCustomer>>
    {
        private readonly IGroupCustomerRepository _GroupCustomerRepository; // Repository for accessing GroupCustomer data.
        private readonly ILogger<UpdateGroupCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateGroupCustomerCommandHandler.
        /// </summary>
        /// <param name="GroupCustomerRepository">Repository for GroupCustomer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateGroupCustomerCommandHandler(
            IGroupCustomerRepository GroupCustomerRepository,
            ILogger<UpdateGroupCustomerCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _GroupCustomerRepository = GroupCustomerRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateGroupCustomerCommand to update a GroupCustomer.
        /// </summary>
        /// <param name="request">The UpdateGroupCustomerCommand containing updated GroupCustomer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UpdateGroupCustomer>> Handle(UpdateGroupCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the GroupCustomer entity to be updated from the repository
                var existingGroupCustomer = await _GroupCustomerRepository.FindAsync(request.Id);

                // Check if the GroupCustomer entity exists
                if (existingGroupCustomer != null)
                {
                    // Update GroupCustomer entity properties with values from the request
                  /*  existingGroupCustomer.Name = request.Name;
                    existingGroupCustomer.Address = request.Address;
                    existingGroupCustomer.Telephone = request.Telephone;
                    existingGroupCustomer.CreatedBy = request.UserID;*/
                    // Use the repository to update the existing GroupCustomer entity
                    _GroupCustomerRepository.Update(existingGroupCustomer);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<UpdateGroupCustomer>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UpdateGroupCustomer>.ReturnResultWith200(_mapper.Map<UpdateGroupCustomer>(existingGroupCustomer));
                    _logger.LogInformation($"GroupCustomer {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the GroupCustomer entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UpdateGroupCustomer>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating GroupCustomer: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UpdateGroupCustomer>.Return500(e);
            }
        }
    }

}
