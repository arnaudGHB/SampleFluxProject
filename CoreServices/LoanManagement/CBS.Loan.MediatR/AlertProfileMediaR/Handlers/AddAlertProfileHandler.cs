using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.AlertProfileP;
using CBS.NLoan.Data.Entity.AlertProfileP;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.AlertProfileMediaR.Commands;
using CBS.NLoan.Repository.AlertProfileP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.AlertProfileMediaR.Handlers
{
    /// <summary>
    /// Handles the command to add a new Loan.
    /// </summary>
    public class AddAlertProfileHandler : IRequestHandler<AddAlertProfileCommand, ServiceResponse<AlertProfileDto>>
    {
        private readonly IAlertProfileRepository _AlertProfileRepository; // Repository for accessing AlertProfile data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddAlertProfileHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<LoanContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddAlertProfileCommandHandler.
        /// </summary>
        /// <param name="AlertProfileRepository">Repository for AlertProfile data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddAlertProfileHandler(
            IAlertProfileRepository AlertProfileRepository,
            IMapper mapper,
            ILogger<AddAlertProfileHandler> logger,
            IUnitOfWork<LoanContext> uow)
        {
            _AlertProfileRepository = AlertProfileRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddAlertProfileCommand to add a new AlertProfile.
        /// </summary>
        /// <param name="request">The AddAlertProfileCommand containing AlertProfile data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<AlertProfileDto>> Handle(AddAlertProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a AlertProfile with the same name already exists (case-insensitive)
                //var existingAlertProfile = await _AlertProfileRepository.FindBy(c => c.Id == request.Id).FirstOrDefaultAsync();

                // If a AlertProfile with the same name already exists, return a conflict response
                //if (existingAlertProfile != null)
                //{
                //    var errorMessage = $"AlertProfile {request.Id} already exists.";
                //    _logger.LogError(errorMessage);
                //    return ServiceResponse<AlertProfileDto>.Return409(errorMessage);
                //}


                // Map the AddAlertProfileCommand to a AlertProfile entity
                var AlertProfileEntity = _mapper.Map<AlertProfile>(request);
                // Convert UTC to local time and set it in the entity
                AlertProfileEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                AlertProfileEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new AlertProfile entity to the repository
                _AlertProfileRepository.Add(AlertProfileEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<AlertProfileDto>.Return500();
                }

                // Map the AlertProfile entity to AlertProfileDto and return it with a success response
                var AlertProfileDto = _mapper.Map<AlertProfileDto>(AlertProfileEntity);
                return ServiceResponse<AlertProfileDto>.ReturnResultWith200(AlertProfileDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving AlertProfile: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<AlertProfileDto>.Return500(e);
            }
        }
    }

}
