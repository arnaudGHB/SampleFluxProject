using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;


using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Commands;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.DATA.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.MEDIATR.Customer.Queries;

namespace CBS.CUSTOMER.MEDIATR
{

    /// <summary>
    /// Handles the command to update a Customer DateOfBirth based on UpdateCustomerDateOfBirthCommand.
    /// </summary>
    public class UpdateCustomerDateOfBirthCommandHandler : IRequestHandler<UpdateCustomerDateOfBirthCommand, ServiceResponse<UpdateCustomerDateBirthDto>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly ILogger<UpdateCustomerDateOfBirthCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _UserInfoToken;
        private readonly IMediator _mediator;
        /// <summary>
        /// Constructor for initializing the UpdateCustomerDateOfBirthCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCustomerDateOfBirthCommandHandler(
            ICustomerRepository CustomerRepository,
            ILogger<UpdateCustomerDateOfBirthCommandHandler> logger,
            UserInfoToken userInfoToken,
             IMediator mediator,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null
            
            )
        {
            _CustomerRepository = CustomerRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _mediator=mediator;
            _UserInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the UpdateCustomerDateOfBirthCommand to update a Customer.
        /// </summary>
        /// <param name="request">The UpdateCustomerDateOfBirthCommand containing updated Customer DateOfBirth data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UpdateCustomerDateBirthDto>> Handle(UpdateCustomerDateOfBirthCommand request, CancellationToken cancellationToken)
        {
            try
            {
               
                // Retrieve the Customer entity to be updated from the repository
                var existingCustomer = await _CustomerRepository.FindAsync(request.CustomerId);
       
                // Check if the Customer entity exists
                if (existingCustomer != null)
                {
                  
                    existingCustomer.DateOfBirth = request.DateOfBirth;
                    existingCustomer.ModifiedDate =DateTime.Now;


                    var getAllCustomerQuery = new GetAllCustomerAgeCategoriesQuery { };
                    var result = await _mediator.Send(getAllCustomerQuery);

                    if (!result.Success)
                    {

                        // If the All Customer Age Categories failed to be retrieved log the error and return a 404 Not Found response
                       string errorMessage = $"failed to be retrieve All Customer Age Categories.";
                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_UserInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 404, _UserInfoToken.Token);
                        return ServiceResponse<UpdateCustomerDateBirthDto>.Return404();
                    }

                    int ageDifference = DateTime.Now.Year - existingCustomer.DateOfBirth.Year;
                    string getCategoryStatus = "No Status";

                    result.Data.ForEach(x =>
                    {
                        if (x.from <= ageDifference && ageDifference <= x.to)
                        {
                            getCategoryStatus = x.Name;
                        }

                    });

                    existingCustomer.AgeCategoryStatus = getCategoryStatus;

                    _CustomerRepository.Update(existingCustomer);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<UpdateCustomerDateBirthDto>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UpdateCustomerDateBirthDto>.ReturnResultWith200(_mapper.Map<UpdateCustomerDateBirthDto>(existingCustomer));
                    _logger.LogInformation($"Customer {request.CustomerId} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Customer entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.CustomerId} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UpdateCustomerDateBirthDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Customer: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UpdateCustomerDateBirthDto>.Return500(e);
            }
        }
    }

}
