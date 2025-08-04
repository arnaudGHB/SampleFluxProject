
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;

using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.DATA;
using CBS.CUSTOMER.DATA.Entity;
using Microsoft.EntityFrameworkCore;

namespace CBS.CardSignatureSpecimen.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new CardSignatureSpecimen.
    /// </summary>
    public class AddCardSignatureSpecimenCommandHandler : IRequestHandler<AddCardSignatureSpecimenCommand, ServiceResponse<CreateCardSignatureSpecimen>>
    {
        private readonly ICardSignatureSpecimenRepository _CardSignatureSpecimenRepository; // Repository for accessing CardSignatureSpecimen data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCardSignatureSpecimenCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;

        /// <summary>
        /// Constructor for initializing the AddCardSignatureSpecimenCommandHandler.
        /// </summary>
        /// <param name="CardSignatureSpecimenRepository">Repository for CardSignatureSpecimen data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCardSignatureSpecimenCommandHandler(
            ICardSignatureSpecimenRepository CardSignatureSpecimenRepository,
            IMapper mapper,
            ILogger<AddCardSignatureSpecimenCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper
,
            UserInfoToken userInfoToken)
        {
            _CardSignatureSpecimenRepository = CardSignatureSpecimenRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _PathHelper = pathHelper;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddCardSignatureSpecimenCommand to add a new CardSignatureSpecimen.
        /// </summary>
        /// <param name="request">The AddCardSignatureSpecimenCommand containing CardSignatureSpecimen data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateCardSignatureSpecimen>> Handle(AddCardSignatureSpecimenCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var exist = await _CardSignatureSpecimenRepository.FindBy(x => x.Name == request.Name && x.IsDeleted == false).FirstOrDefaultAsync();
                if (exist!=null)
                {
                    var errorMessage = $"{request.Name} already exist.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), exist, errorMessage , LogLevelInfo.Error.ToString(), 409, _UserInfoToken.Token);
                    return ServiceResponse<CreateCardSignatureSpecimen>.Return409(errorMessage);
                }
                // Map the AddCardSignatureSpecimenCommand to a CardSignatureSpecimen entity
                var CardSignatureSpecimenEntity = _mapper.Map<CUSTOMER.DATA.Entity.AccountSignature>(request);
                //LoginDto login = await APICallHelper.AuthenthicationFromIdentityServer(_pathHelper);
                CardSignatureSpecimenEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);
                //Auto generateKey For Unique Identification
                CardSignatureSpecimenEntity.Id = BaseUtilities.GenerateUniqueNumber();
                var temporalPin = BaseUtilities.GenerateUniqueNumber(5);

                // Add the new CardSignatureSpecimen entity to the repository
                _CardSignatureSpecimenRepository.Add(CardSignatureSpecimenEntity);
                await _uow.SaveAsync();

                // Map the CardSignatureSpecimen entity to CreateCardSignatureSpecimen and return it with a success response
                var CreateCardSignatureSpecimen = _mapper.Map<CreateCardSignatureSpecimen>(CardSignatureSpecimenEntity);

                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), CreateCardSignatureSpecimen, $"Creating New CardSignatureSpecimen   {request.CustomerId} Successful  ", LogLevelInfo.Information.ToString(), 200, _UserInfoToken.Token);

                return ServiceResponse<CreateCardSignatureSpecimen>.ReturnResultWith200(CreateCardSignatureSpecimen);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving CardSignatureSpecimen: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new CardSignatureSpecimen, CardSignatureSpecimen : {request.CustomerId}  ", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);

                return ServiceResponse<CreateCardSignatureSpecimen>.Return500(e);
            }
        }
    }

}
