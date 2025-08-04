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
    /// Handles the command to add a new Town.
    /// </summary>
    public class AddTownCommandHandler : IRequestHandler<AddTownCommand, ServiceResponse<TownDto>>
    {
        private readonly ITownRepository _TownRepository; // Repository for accessing Town data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddTownCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddTownCommandHandler.
        /// </summary>
        /// <param name="TownRepository">Repository for Town data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddTownCommandHandler(
            ITownRepository TownRepository,
            IMapper mapper,
            ILogger<AddTownCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _TownRepository = TownRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddTownCommand to add a new Town.
        /// </summary>
        /// <param name="request">The AddTownCommand containing Town data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<TownDto>> Handle(AddTownCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Town with the same name already exists (case-insensitive)
                var existingTown = await _TownRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a Town with the same name already exists, return a conflict response
                if (existingTown!=null)
                {
                    var errorMessage = $"Town {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<TownDto>.Return409(errorMessage);
                }
                // Map the AddTownCommand to a Town entity
                var TownEntity = _mapper.Map<Town>(request);
                // Convert UTC to local time and set it in the entity
                TownEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                TownEntity.Id = BaseUtilities.GenerateUniqueNumber();
                // Add the new Town entity to the repository
                _TownRepository.Add(TownEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<TownDto>.Return500();
                }
                // Map the Town entity to TownDto and return it with a success response
                var TownDto = _mapper.Map<TownDto>(TownEntity);
                return ServiceResponse<TownDto>.ReturnResultWith200(TownDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Town: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<TownDto>.Return500(e);
            }
        }
    }

}
