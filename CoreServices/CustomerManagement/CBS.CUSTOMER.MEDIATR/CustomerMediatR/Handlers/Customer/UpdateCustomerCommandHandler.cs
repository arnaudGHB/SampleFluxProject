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

namespace CBS.CUSTOMER.MEDIATR
{

    /// <summary>
    /// Handles the command to update a Customer based on UpdateCustomerCommand.
    /// </summary>
    public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, ServiceResponse<UpdateCustomer>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customer data.
        private readonly ILogger<UpdateCustomerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<POSContext> _uow;

        /// <summary>
        /// Constructor for initializing the UpdateCustomerCommandHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customer data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateCustomerCommandHandler(
            ICustomerRepository CustomerRepository,
            ILogger<UpdateCustomerCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<POSContext> uow = null)
        {
            _CustomerRepository = CustomerRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
        }

        /// <summary>
        /// Handles the UpdateCustomerCommand to update a Customer.
        /// </summary>
        /// <param name="request">The UpdateCustomerCommand containing updated Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<UpdateCustomer>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                request.Phone = BaseUtilities.Add237Prefix(request.Phone);
                request.HomePhone = BaseUtilities.Add237Prefix(request.HomePhone);
                request.PersonalPhone = BaseUtilities.Add237Prefix(request.PersonalPhone);
                // Retrieve the Customer entity to be updated from the repository
                var existingCustomer = await _CustomerRepository.FindAsync(request.Id);
       
                // Check if the Customer entity exists
                if (existingCustomer != null)
                {
                   /* // Check if a Customer with the same name already exists (case-insensitive)
                    var existingPhoneCustomer = await _CustomerRepository.FindBy(c => c.Phone == request.Phone && c.IsDeleted == false).FirstOrDefaultAsync();

                    // If a Customer with the same Phone already exists, return a conflict response
                    if (existingPhoneCustomer != null && existingCustomer.Email !=request.Email)
                    {
                        var errorMessage = $"Customer With Phone {(request.Phone)} already exists.";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<UpdateCustomer>.Return409(errorMessage);
                    };*/

                  /*  // Check if a Customer with the same name already exists (case-insensitive)
                    var existingEmail = await _CustomerRepository.FindBy(c => c.Email == request.Email && c.IsDeleted == false).FirstOrDefaultAsync();

                    // If a Customer with the same Email already exists, return a conflict response
                    if (existingEmail != null && existingEmail.Email != request.Email && existingEmail.Phone!=ex)
                    {
                        var errorMessage = $"Customer With Email {(request.Email)} already exists. Try Another Email";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<UpdateCustomer>.Return409(errorMessage);
                    };*/

                     existingCustomer.Email = request.Email==null?existingCustomer.Email:request.Email;
                    existingCustomer.Phone = request.Phone == null ? existingCustomer.Phone : request.Phone;
                
                    existingCustomer.IDNumber = request.IDNumber == null ? existingCustomer.IDNumber : request.IDNumber;
                    existingCustomer.Address = request.Address == null ? existingCustomer.Address : request.Address;
                    existingCustomer.FirstName = request.FirstName == null ? existingCustomer.FirstName : request.FirstName;
                    existingCustomer.LastName = request.LastName == null ? existingCustomer.LastName : request.LastName;
                    existingCustomer.BankId = request.BankId == null ? existingCustomer.BankId : request.BankId;
                    existingCustomer.BranchId = request.BranchId == null ? existingCustomer.BranchId : request.BranchId;
                    existingCustomer.DivisionId = request.DivisionId == null ? existingCustomer.DivisionId : request.DivisionId;
                    existingCustomer.SubDivisionId = request.SubdivisionId == null ? existingCustomer.Email : request.Email;
                    existingCustomer.OrganizationId = request.OrganisationId == null ? existingCustomer.OrganizationId : request.OrganisationId;
                    existingCustomer.TaxIdentificationNumber = request.TaxIdentificationNumber == null ? existingCustomer.TaxIdentificationNumber : request.TaxIdentificationNumber;
                    existingCustomer.CustomerPackageId=request.PackageId == null ? existingCustomer.CustomerPackageId : request.PackageId;
                    existingCustomer.EconomicActivitiesId = request.EconomicActivitesId == null ? existingCustomer.EconomicActivitiesId : request.EconomicActivitesId;
                    existingCustomer.POBox = request.ZipCode == null ? existingCustomer.POBox : request.ZipCode;
                    existingCustomer.Gender = request.Gender == null ? existingCustomer.Gender : request.Gender;
                    existingCustomer.Language = request.Language == null ? existingCustomer.Language : request.Language;
                    existingCustomer.ModifiedDate =DateTime.Now;
                   // existingCustomer.Pin = BCrypt.Net.BCrypt.HashPassword(request.Pin);

                    _CustomerRepository.Update(existingCustomer);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<UpdateCustomer>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UpdateCustomer>.ReturnResultWith200(_mapper.Map<UpdateCustomer>(existingCustomer));
                    _logger.LogInformation($"Customer {request.Id} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Customer entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.Id} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<UpdateCustomer>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating Customer: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<UpdateCustomer>.Return500(e);
            }
        }
    }

}
