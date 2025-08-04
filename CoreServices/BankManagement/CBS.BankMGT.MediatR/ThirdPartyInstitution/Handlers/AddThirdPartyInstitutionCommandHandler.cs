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
using CBS.BankMGT.Data.Entity;
 
namespace CBS.BankMGT.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new ThirdPartyInstitution.
    /// </summary>
    public class AddThirdPartyInstitutionCommandHandler : IRequestHandler<AddThirdPartyInstitutionCommand, ServiceResponse<CorrespondingBankDto>>
    {
        private readonly IThirdPartyInstitutionRepository _thirdPartyInstitutionRepository; // Repository for accessing ThirdPartyInstitution data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddThirdPartyInstitutionCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddBankCommandHandler.
        /// </summary>
        /// <param name="BankRepository">Repository for Bank data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddThirdPartyInstitutionCommandHandler(
            IThirdPartyInstitutionRepository thirdPartyInstitutionRepository,
            IMapper mapper,
            ILogger<AddThirdPartyInstitutionCommandHandler> logger,
            IUnitOfWork<POSContext> uow)
        {
            _thirdPartyInstitutionRepository = thirdPartyInstitutionRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
        }

        /// <summary>
        /// Handles the AddThirdPartyInstitutionCommand to add a new ThirdPartyInstitution.
        /// </summary>
        /// <param name="request">The AddThirdPartyInstitutionCommand containing ThirdPartyInstitution data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CorrespondingBankDto>> Handle(AddThirdPartyInstitutionCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a ThirdPartyInstitution with the same name already exists (case-insensitive)
                var existingBank = await _thirdPartyInstitutionRepository.FindBy(c => c.Name == request.Name).FirstOrDefaultAsync();

                // If a ThirdPartyInstitution with the same name already exists, return a conflict response
                if (existingBank != null)
                {
                    var errorMessage = $"Bank {request.Name} already exists.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<CorrespondingBankDto>.Return409(errorMessage);
                }

                // Map the AddBankCommand to a Bank entity
                var BankEntity = _mapper.Map<ThirdPartyInstitution>(request);

                BankEntity.Email = "info@flux.com";
                BankEntity.HeadOfficeLocation = "location";
                BankEntity.HeadOfficeTelephone = "2377xxx698";
                BankEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new Bank entity to the repository
                _thirdPartyInstitutionRepository.Add(BankEntity);
                await _uow.SaveAsync();

                // Map the Bank entity to BankDto and return it with a success response
                var BankDto = _mapper.Map<CorrespondingBankDto>(BankEntity);
                return ServiceResponse<CorrespondingBankDto>.ReturnResultWith200(BankDto);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Bank: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CorrespondingBankDto>.Return500(e, errorMessage);
            }
        }
    }

}
