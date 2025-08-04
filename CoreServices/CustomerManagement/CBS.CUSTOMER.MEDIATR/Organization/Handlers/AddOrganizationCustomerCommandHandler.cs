
using MediatR;
using AutoMapper;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.DOMAIN.Context;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using CBS.Customer.MEDIATR.CustomerMediatR;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.REPOSITORY.OrganisationRepo;
using CBS.CUSTOMER.DATA.Entity;

namespace CBS.OrganisationCustomer.MEDIATR.OrganisationCustomerMediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new OrganizationCustomer.
    /// </summary>
    public class AddOrganizationCustomerCommandHandler : IRequestHandler<AddOrganizationCustomerCommand, ServiceResponse<CreateOrganizationCustomer>>
    {
        private readonly IOrganizationCustomerRepository _OrganizationCustomerRepository; // Repository for accessing OrganizationCustomer data.
        private readonly IOrganizationRepository _OrganizationRepository; // Repository for accessing Organization data.
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddOrganizationCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        /// <summary>
        /// Constructor for initializing the AddOrganizationCustomerCommandHandler.
        /// </summary>
        /// <param name="OrganizationCustomerRepository">Repository for OrganizationCustomer data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddOrganizationCustomerCommandHandler(
            IOrganizationCustomerRepository OrganizationCustomerRepository,
            IMapper mapper,
            ILogger<AddOrganizationCustomerCommandHandler> logger,
            IUnitOfWork<POSContext> uow,
            ICustomerRepository customerRepository,
            IOrganizationRepository OrganizationRepository)
        {
            _OrganizationCustomerRepository = OrganizationCustomerRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _CustomerRepository = customerRepository;
            _OrganizationRepository = OrganizationRepository;
        }

        /// <summary>
        /// Handles the AddOrganizationCustomerCommand to add a new OrganizationCustomer.
        /// </summary>
        /// <param name="request">The AddOrganizationCustomerCommand containing OrganizationCustomer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CreateOrganizationCustomer>> Handle(AddOrganizationCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var OrganizationId = request.OrganizationId;

                var verfiyIfOrganizationExist = _OrganizationRepository.Find(OrganizationId);

                if (verfiyIfOrganizationExist == null)
                {
                    return ServiceResponse<CreateOrganizationCustomer>.Return404("Organization Record with CustomerId :  " + OrganizationId + " Does Not Exist");
                }


                var verfiyIfCustomerExist = _CustomerRepository.Find(request.CustomerId);

               

                if (verfiyIfCustomerExist==null)
                {
                    return ServiceResponse<CreateOrganizationCustomer>.Return404("Customer with CustomerId : "+request.CustomerId+" Does not Exist");
                }


                List<CUSTOMER.DATA.Entity.OrganizationCustomer> OrganizationCustomersResult = new List<CUSTOMER.DATA.Entity.OrganizationCustomer>();


              
                    // Map the AddOrganizationCustomerCommand to a OrganizationCustomer entity
                    var OrganizationCustomerEntity = new CUSTOMER.DATA.Entity.OrganizationCustomer()
                    {
                        OrganizationCustomerId = BaseUtilities.GenerateUniqueNumber(),
                        CustomerId = request.CustomerId,
                        Position = request.Position,
                        OrganizationId = OrganizationId,
                        CreatedDate = BaseUtilities.UtcToLocal(DateTime.UtcNow)
                    };

                    // Add the new OrganizationCustomer entity to the repository
                    _OrganizationCustomerRepository.Add(OrganizationCustomerEntity);



     
             

                if (await _uow.SaveAsync() <= 0)
                {
                    return ServiceResponse<CreateOrganizationCustomer>.Return500();
                }

                var CreateOrganizationCustomer = _mapper.Map<CreateOrganizationCustomer>(OrganizationCustomerEntity);
                return ServiceResponse<CreateOrganizationCustomer>.ReturnResultWith200(CreateOrganizationCustomer);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving OrganizationCustomer: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<CreateOrganizationCustomer>.Return500(e);
            }
        }
    }


}
