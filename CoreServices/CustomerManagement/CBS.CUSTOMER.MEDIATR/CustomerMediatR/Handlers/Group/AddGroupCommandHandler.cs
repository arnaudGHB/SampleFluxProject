
using MediatR;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.REPOSITORY.GroupRepo;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.Customer.MEDIATR;

namespace CBS.Group.MEDIATR.GroupMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Group.
    /// </summary>
    public class AddGroupCommandHandler : IRequestHandler<AddGroupCommand, ServiceResponse<CreateGroup>>
    {
        private readonly IGroupRepository _GroupRepository; // Repository for accessing Group data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddGroupCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddGroupCommandHandler.
        /// </summary>
        /// <param name="GroupRepository">Repository for Group data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddGroupCommandHandler(
            IGroupRepository GroupRepository,
            IMapper mapper,
            ILogger<AddGroupCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _GroupRepository = GroupRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddGroupCommand to add a new Group.
        /// </summary>
        /// <param name="request">The AddGroupCommand containing Group data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateGroup>> Handle(AddGroupCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Group with the same name already exists (case-insensitive)
                var existingGroup = await _GroupRepository.FindBy(c => c.GroupName == (request.GroupName)).FirstOrDefaultAsync();

                // If a Group with the same name already exists, return a conflict response
                if (existingGroup != null)
                {
                    var errorMessage = $"Group {(request.GroupName)} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CreateGroup>.Return409(errorMessage);
                }

                // Map the AddGroupCommand to a Group entity
                var GroupEntity = _mapper.Map<CUSTOMER.DATA.Entity.Group>(request);

               

                GroupEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                GroupEntity.GroupId = BaseUtilities.GenerateUniqueNumber();
 
                // Add the new Group entity to the repository
                _GroupRepository.Add(GroupEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CreateGroup>.Return500();
                }
                // Map the Group entity to CreateGroup and return it with a success response
                var CreateGroup = _mapper.Map<CreateGroup>(GroupEntity);
                return ServiceResponse<CreateGroup>.ReturnResultWith200(CreateGroup);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Group: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateGroup>.Return500(e);
            }
        }
    }

}
