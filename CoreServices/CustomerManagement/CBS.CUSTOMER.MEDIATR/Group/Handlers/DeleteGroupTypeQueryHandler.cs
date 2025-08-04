using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.MEDIATR;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;

namespace CBS.Group.MEDIATR
{
    /// <summary>
    /// Handles the request to retrieve a specific Group based on its unique identifier.
    /// </summary>
    public class DeleteGroupTypeQueryHandler : IRequestHandler<DeleteGroupTypeCommand, ServiceResponse<bool>>
    {
        private readonly IGroupTypeRepository _GroupTypeRepository; // Repository for accessing Group type data.
         private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<DeleteGroupCustomerQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the DeleteGroupTypeQueryHandler.
        /// </summary>
        /// <param name="GroupRepository">Repository for Group data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public DeleteGroupTypeQueryHandler(
            IMapper mapper,
            ILogger<DeleteGroupCustomerQueryHandler> logger,
            IUnitOfWork<POSContext> uow,
            IGroupTypeRepository groupTypeRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _GroupTypeRepository = groupTypeRepository;
        }

        /// <summary>
        /// Handles the DeleteGroupTypeCommand to delete specific Group.
        /// </summary>
        /// <param name="request">The DeleteGroupTypeCommand containing Group ID to delete specific group.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<bool>> Handle(DeleteGroupTypeCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {

                var groupType = _GroupTypeRepository.FindBy(x => x.GroupTypeId == request.Id && x.IsDeleted == false).FirstOrDefault();

                if (groupType != null)
                {

                    groupType.IsDeleted = true;
                    groupType.DeletedBy = request.UserId;
                    _GroupTypeRepository.Update(groupType);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<bool>.Return500();
                    }
                    return ServiceResponse<bool>.ReturnResultWith200(true);
                }
                else
                {
                    // If the Group Type entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Group Type CustomerId  not found.");
                    return ServiceResponse<bool>.Return404("Group Type CustomerId  not found.");
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Group: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<bool>.Return500(e);
            }
        }
    }

}
