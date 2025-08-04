
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;

using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;
using BusinessServiceLayer.Objects.SmsObject;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class AddCardSignatureSpecimenDetailsCommandHandler : IRequestHandler<AddCardSignatureSpecimenDetailsCommand, ServiceResponse<CreateCardSignatureSpecimenDetails>>
    {
        private readonly ICardSignatureSpecimenDetailRepository _CardSignatureSpecimenDetailsRepository; // Repository for accessing CardSignatureSpecimenDetails data.
        private readonly ICardSignatureSpecimenRepository _CardSignatureSpecimenRepository; // Repository for accessing CardSignatureSpecimen data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;

        /// <summary>
        /// Constructor for initializing the AddCustomerCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCardSignatureSpecimenDetailsCommandHandler(
            ICardSignatureSpecimenDetailRepository CardSignatureSpecimenDetailsRepository,
            IMapper mapper,
            ILogger<AddCustomerCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper
,
            UserInfoToken userInfoToken,
            ICardSignatureSpecimenRepository cardSignatureSpecimenRepository)
        {
            _CardSignatureSpecimenDetailsRepository = CardSignatureSpecimenDetailsRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _PathHelper = pathHelper;
            _UserInfoToken = userInfoToken;
            _CardSignatureSpecimenRepository = cardSignatureSpecimenRepository;
        }

        /// <summary>
        /// Handles the AddCustomerCommand to add a new Customer.
        /// </summary>
        /// <param name="request">The AddCustomerCommand containing Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateCardSignatureSpecimenDetails>> Handle(AddCardSignatureSpecimenDetailsCommand request, CancellationToken cancellationToken)
        {
            try
            {

                // Check if a CardSignatureSpecimen with the same name already exists (case-insensitive)
                var existCardSignatureSpecimen =  _CardSignatureSpecimenRepository.Find(request.CardSignatureSpecimenId);

                // If a CardSignatureSpecimen with the same Phone already exists, return a conflict response
                if (existCardSignatureSpecimen != null)
                {
                    var errorMessage = $"CardSignatureSpecimen With Phone {(request.CardSignatureSpecimenId)} already exists.";
                    _logger.LogError(errorMessage);
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 409, _UserInfoToken.Token);
                    return ServiceResponse<CreateCardSignatureSpecimenDetails>.Return409(errorMessage);
                };


                // Map the AddCustomerCommand to a Customer entity
                var CustomerEntity = _mapper.Map<DATA.Entity.CardSignatureSpecimenDetail>(request);
                //LoginDto login = await APICallHelper.AuthenthicationFromIdentityServer(_pathHelper);

                
                CustomerEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                CustomerEntity.Id = BaseUtilities.GenerateUniqueNumber();
              

                var temporalPin = BaseUtilities.GenerateUniqueNumber(5);

                


           
                // Add the new Customer entity to the repository
                _CardSignatureSpecimenDetailsRepository.Add(CustomerEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, $"Internal Server Error occurred while saving new MembershipNextOfKings, MembershipNextOfKings : {request.Name}", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);
                    return ServiceResponse<CreateCardSignatureSpecimenDetails>.Return500();
                }

              
                  

                




                // Map the Customer entity to CreateCustomer and return it with a success response
                var CreateCustomer = _mapper.Map<CreateCardSignatureSpecimenDetails>(CustomerEntity);
               
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), CreateCustomer, $"Creating New MembershipNextOfKings   {request.Name} Successful  ", LogLevelInfo.Information.ToString(), 200, _UserInfoToken.Token);

                return ServiceResponse<CreateCardSignatureSpecimenDetails>.ReturnResultWith200(CreateCustomer);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Customer: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new MembershipNextOfKings, MembershipNextOfKings : {request.Name}  ", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);

                return ServiceResponse<CreateCardSignatureSpecimenDetails>.Return500(e);
            }
        }

      


    }

}
