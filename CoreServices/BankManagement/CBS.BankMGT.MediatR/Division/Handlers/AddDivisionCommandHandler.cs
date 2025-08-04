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
    /// Handles the command to add a new Division.
    /// </summary>
    public class AddDivisionCommandHandler : IRequestHandler<AddDivisionCommand, ServiceResponse<DivisionDto>>
    {
        private readonly IDivisionRepository _DivisionRepository; // Repository for accessing Division data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddDivisionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddDivisionCommandHandler.
        /// </summary>
        /// <param name="DivisionRepository">Repository for Division data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddDivisionCommandHandler(
            IDivisionRepository DivisionRepository,
            IMapper mapper,
            ILogger<AddDivisionCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _DivisionRepository = DivisionRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddDivisionCommand to add a new Division.
        /// </summary>
        /// <param name="request">The AddDivisionCommand containing Division data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<DivisionDto>> Handle(AddDivisionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Division with the same name already exists (case-insensitive)
                var existingDivision = await _DivisionRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a Division with the same name already exists, return a conflict response
                if (existingDivision!=null)
                {
                    var errorMessage = $"Division {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<DivisionDto>.Return409(errorMessage);
                }
                // Map the AddDivisionCommand to a Division entity
                var DivisionEntity = _mapper.Map<Division>(request);
                // Convert UTC to local time and set it in the entity
                DivisionEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                DivisionEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Add the new Division entity to the repository
                _DivisionRepository.Add(DivisionEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<DivisionDto>.Return500();
                }
                // Map the Division entity to DivisionDto and return it with a success response
                var DivisionDto = _mapper.Map<DivisionDto>(DivisionEntity);
                return ServiceResponse<DivisionDto>.ReturnResultWith200(DivisionDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Division: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<DivisionDto>.Return500(e);
            }
        }
    }

}
