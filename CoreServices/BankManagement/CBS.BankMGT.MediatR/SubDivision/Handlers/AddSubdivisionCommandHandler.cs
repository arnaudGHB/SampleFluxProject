using AutoMapper;
using CBS.BankMGT.Common.UnitOfWork;
using CBS.BankMGT.Data;
using CBS.BankMGT.Domain;
using CBS.BankMGT.Helper;
using CBS.BankMGT.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.BankMGT.MediatR.Commands;
using CBS.BankMGT.Data.Dto;

namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Subdivision.
    /// </summary>
    public class AddSubdivisionCommandHandler : IRequestHandler<AddSubdivisionCommand, ServiceResponse<SubdivisionDto>>
    {
        private readonly ISubdivisionRepository _SubdivisionRepository; // Repository for accessing Subdivision data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddSubdivisionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddSubdivisionCommandHandler.
        /// </summary>
        /// <param name="SubdivisionRepository">Repository for Subdivision data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddSubdivisionCommandHandler(
            ISubdivisionRepository SubdivisionRepository,
            IMapper mapper,
            ILogger<AddSubdivisionCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _SubdivisionRepository = SubdivisionRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddSubdivisionCommand to add a new Subdivision.
        /// </summary>
        /// <param name="request">The AddSubdivisionCommand containing Subdivision data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<SubdivisionDto>> Handle(AddSubdivisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Subdivision with the same name already exists (case-insensitive)
                var existingSubdivision = await _SubdivisionRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a Subdivision with the same name already exists, return a conflict response
                if (existingSubdivision!=null)
                {
                    var errorMessage = $"Subdivision {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<SubdivisionDto>.Return409(errorMessage);
                }
                // Map the AddSubdivisionCommand to a Subdivision entity
                var SubdivisionEntity = _mapper.Map<Subdivision>(request);
                // Convert UTC to local time and set it in the entity
                SubdivisionEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                SubdivisionEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Add the new Subdivision entity to the repository
                _SubdivisionRepository.Add(SubdivisionEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<SubdivisionDto>.Return500();
                }
                // Map the Subdivision entity to SubdivisionDto and return it with a success response
                var SubdivisionDto = _mapper.Map<SubdivisionDto>(SubdivisionEntity);
                return ServiceResponse<SubdivisionDto>.ReturnResultWith200(SubdivisionDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Subdivision: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<SubdivisionDto>.Return500(e);
            }
        }
    }

}
