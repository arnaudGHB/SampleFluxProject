
using MediatR;
using CBS.CUSTOMER.HELPER.Helper;
using AutoMapper;

using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.DATA.Dto;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class AddMembershipNextOfKingsCommandHandler : IRequestHandler<AddMembershipNextOfKingsCommand, ServiceResponse<CreateMembershipNextOfKings>>
    {
        private readonly IMembershipNextOfKingRepository _MembershipNextOfKingsRepository; // Repository for accessing MembershipNextOfKings data.
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationRepository; // Repository for accessing CustomerSmsConfiguration data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;

        /// <summary>
        /// Constructor for initializing the AddMembershipNextOfKingsCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddMembershipNextOfKingsCommandHandler(
            IMembershipNextOfKingRepository MembershipNextOfKingsRepository,
            ICustomerSmsConfigurationRepository CustomerSmsConfigurationRepository,
            IMapper mapper,
            ILogger<AddCustomerCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper
,
            UserInfoToken userInfoToken)
        {
            _MembershipNextOfKingsRepository = MembershipNextOfKingsRepository;
            _CustomerSmsConfigurationRepository = CustomerSmsConfigurationRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _PathHelper = pathHelper;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddCustomerCommand to add a new Customer.
        /// </summary>
        /// <param name="request">The AddCustomerCommand containing Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateMembershipNextOfKings>> Handle(AddMembershipNextOfKingsCommand request, CancellationToken cancellationToken)
        {
            try
            {
               
                // Map the AddCustomerCommand to a Customer entity
                var CustomerEntity = _mapper.Map<DATA.Entity.MembershipNextOfKing>(request);
                //LoginDto login = await APICallHelper.AuthenthicationFromIdentityServer(_pathHelper);

                
                CustomerEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                CustomerEntity.Id = BaseUtilities.GenerateUniqueNumber();
              

                var temporalPin = BaseUtilities.GenerateUniqueNumber(5);

                


           
                // Add the new Customer entity to the repository
                _MembershipNextOfKingsRepository.Add(CustomerEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, $"Internal Server Error occurred while saving new MembershipNextOfKings, MembershipNextOfKings : {request.Name}", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);
                    return ServiceResponse<CreateMembershipNextOfKings>.Return500();
                }

              
                  

                




                // Map the Customer entity to CreateCustomer and return it with a success response
                var CreateCustomer = _mapper.Map<CreateMembershipNextOfKings>(CustomerEntity);
               
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), CreateCustomer, $"Creating New MembershipNextOfKings   {request.Name} Successful  ", LogLevelInfo.Information.ToString(), 200, _UserInfoToken.Token);

                return ServiceResponse<CreateMembershipNextOfKings>.ReturnResultWith200(CreateCustomer);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving MembershipNextOfKings: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new MembershipNextOfKings, MembershipNextOfKings : {request.Name}  ", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);

                return ServiceResponse<CreateMembershipNextOfKings>.Return500(e);
            }
        }

      


    }

}
