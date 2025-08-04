
using MediatR;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.Customer.MEDIATR;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Dto.Groups;

namespace CBS.Group.MEDIATR.GroupMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new GroupType.
    /// </summary>
    public class AddGroupTypeCommandHandler : IRequestHandler<AddGroupTypeCommand, ServiceResponse<CreateGroupTypeDto>>
    {
        private readonly IGroupTypeRepository _GroupTypeRepository; // Repository for accessing GroupType data.
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddGroupTypeCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddGroupCommandHandler.
        /// </summary>
        /// <param name="GroupRepository">Repository for GroupType data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddGroupTypeCommandHandler(
            IGroupTypeRepository GroupTypeRepository,
            IMapper mapper,
            ILogger<AddGroupTypeCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            ICustomerRepository customerRepository)
        {
            _GroupTypeRepository = GroupTypeRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _CustomerRepository = customerRepository;
        }

        /// <summary> 
        /// Handles the AddGroupCommand to add a new GroupType.
        /// </summary>
        /// <param name="request">The AddGroupCommand containing GroupType data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateGroupTypeDto>> Handle(AddGroupTypeCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a GroupType with the same name already exists (case-insensitive)
                var existingGroup = await _GroupTypeRepository.FindBy(c => c.GroupTypeName == (request.GroupTypeName) && c.IsDeleted==false).FirstOrDefaultAsync();

                // If a GroupType with the same name already exists, return a conflict response
                if (existingGroup != null)
                {
                    var errorMessage = $"GroupType {(request.GroupTypeName)} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CreateGroupTypeDto>.Return409(errorMessage);
                }

                // Map the AddGroupCommand to a GroupType entity
                var GroupTypeEntity = _mapper.Map<CUSTOMER.DATA.Entity.GroupType>(request);
                GroupTypeEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                GroupTypeEntity.GroupTypeId = BaseUtilities.GenerateUniqueNumber();
                _GroupTypeRepository.Add(GroupTypeEntity);
                await _uow.SaveAsync();
                // Map the GroupType entity to CreateGroup and return it with a success response
                var GroupTypeDto = _mapper.Map<CreateGroupTypeDto>(GroupTypeEntity);
                return ServiceResponse<CreateGroupTypeDto>.ReturnResultWith200(GroupTypeDto, $"{request.GroupTypeName} was successfully created.");
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving GroupType: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateGroupTypeDto>.Return500(e);
            }
        }
    }

}
