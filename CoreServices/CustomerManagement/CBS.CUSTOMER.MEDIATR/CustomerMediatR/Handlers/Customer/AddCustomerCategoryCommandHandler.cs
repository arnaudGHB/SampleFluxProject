
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
using CBS.CUSTOMER.DATA;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the command to add a new Customer.
    /// </summary>
    public class AddCustomerCategoryCommandHandler : IRequestHandler<AddCustomerCategoryCommand, ServiceResponse<CreateCustomerCategory>>
    {
        private readonly ICustomerCategoryRepository _CustomerCategoryRepository; // Repository for accessing CustomerCategory data.
        private readonly ICustomerSmsConfigurationRepository _CustomerSmsConfigurationRepository; // Repository for accessing CustomerSmsConfiguration data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly PathHelper _PathHelper;

        /// <summary>
        /// Constructor for initializing the AddCustomerCategoryCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddCustomerCategoryCommandHandler(
            ICustomerCategoryRepository CustomerCategoryRepository,
            ICustomerSmsConfigurationRepository CustomerSmsConfigurationRepository,
            IMapper mapper,
            ILogger<AddCustomerCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            PathHelper pathHelper
,
            UserInfoToken userInfoToken)
        {
            _CustomerCategoryRepository = CustomerCategoryRepository;
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
        public async Task<ServiceResponse<CreateCustomerCategory>> Handle(AddCustomerCategoryCommand request, CancellationToken cancellationToken)
        {
            try
            {
               
                // Map the AddCustomerCommand to a Customer entity
                var CustomerEntity = _mapper.Map<DATA.Entity.CustomerCategory>(request);
                //LoginDto login = await APICallHelper.AuthenthicationFromIdentityServer(_pathHelper);

                
                CustomerEntity.CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow);

                //Auto generateKey For Unique Identification
                CustomerEntity.CustomerCategoryId = BaseUtilities.GenerateUniqueNumber();
             
           
                // Add the new Customer entity to the repository
                _CustomerCategoryRepository.Add(CustomerEntity);
                if (await _uow.SaveAsync() <= 0)
                {
                    await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), request, $"Internal Server Error occurred while saving new CustomerCategory, CustomerCategory : {request.CategoryName}", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);
                    return ServiceResponse<CreateCustomerCategory>.Return500();
                }

              
                  

                




                // Map the Customer entity to CreateCustomer and return it with a success response
                var CreateCustomer = _mapper.Map<CreateCustomerCategory>(CustomerEntity);
               
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), CreateCustomer, $"Creating New CustomerCategory   {request.CategoryName} Successful  ", LogLevelInfo.Information.ToString(), 200, _UserInfoToken.Token);

                return ServiceResponse<CreateCustomerCategory>.ReturnResultWith200(CreateCustomer);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving CustomerCategory: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new MembershipNextOfKings, CustomerCategory : {request.CategoryName}  ", LogLevelInfo.Error.ToString(), 500, _UserInfoToken.Token);

                return ServiceResponse<CreateCustomerCategory>.Return500(e);
            }
        }

      


    }

}
