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
            IUnitOfWork<POSContext> uow = null
            
            )
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
                // Retrieve the Customer entity to be updated from the repository
                var existingCustomer = await _CustomerRepository.FindAsync(request.CustomerId);
       
                // Check if the Customer entity exists
                if (existingCustomer != null)
                {
                  

                     existingCustomer.Email = request.Email==null?existingCustomer.Email:request.Email;
                    existingCustomer.Phone = request.Phone == null ? existingCustomer.Phone : request.Phone;
                    existingCustomer.WorkingStatus = request.WorkingStatus == null ? existingCustomer.WorkingStatus : request.WorkingStatus;
                    existingCustomer.EmployerName = request.EmployerName == null ? existingCustomer.EmployerName : request.EmployerName;
                    existingCustomer.EmployerAddress = request.EmployerAddress == null ? existingCustomer.EmployerAddress : request.EmployerAddress;
                    existingCustomer.LegalForm = request.LegalForm == null ? existingCustomer.LegalForm : request.LegalForm;
                    existingCustomer.FormalOrInformalSector = request.FormalOrInformalSector == null ? existingCustomer.FormalOrInformalSector : request.FormalOrInformalSector;
                    existingCustomer.BankingRelationship = request.BankingRelationship == null ? existingCustomer.BankingRelationship : request.BankingRelationship;

                    existingCustomer.IDNumber = request.IdNumber == null ? existingCustomer.IDNumber : request.IdNumber;
                    existingCustomer.Address = request.Address == null ? existingCustomer.Address : request.Address;
                    existingCustomer.FirstName = request.FirstName == null ? existingCustomer.FirstName : request.FirstName;
                    existingCustomer.LastName = request.LastName == null ? existingCustomer.LastName : request.LastName;
                    existingCustomer.BankId = request.BankId == null ? existingCustomer.BankId : request.BankId;
                    existingCustomer.BranchId = request.BranchId == null ? existingCustomer.BranchId : request.BranchId;
                    existingCustomer.DivisionId = request.DivisionId == null ? existingCustomer.DivisionId : request.DivisionId;
                    existingCustomer.SubDivisionId = request.SubDivisionId == null ? existingCustomer.SubDivisionId : request.SubDivisionId;
                    existingCustomer.OrganizationId = request.OrganizationId == null ? existingCustomer.OrganizationId : request.OrganizationId;
                    existingCustomer.TaxIdentificationNumber = request.TaxIdentificationNumber == null ? existingCustomer.TaxIdentificationNumber : request.TaxIdentificationNumber;
                    existingCustomer.CustomerPackageId=request.CustomerPackageId == null ? existingCustomer.CustomerPackageId : request.CustomerPackageId;
                    existingCustomer.EconomicActivitiesId = request.EconomicActivitiesId == null ? existingCustomer.EconomicActivitiesId : request.EconomicActivitiesId;
                    existingCustomer.POBox = request.PoBox == null ? existingCustomer.POBox : request.PoBox;
                    existingCustomer.Gender = request.Gender == null ? existingCustomer.Gender : request.Gender;
                    existingCustomer.Language = request.Language == null ? existingCustomer.Language : request.Language;
                    existingCustomer.MembershipApplicantDate = request.MembershipApplicantDate == null ? existingCustomer.MembershipApplicantDate : request.MembershipApplicantDate;
                   
                    existingCustomer.MembershipApplicantProposedByReferral1 = request.MembershipApplicantProposedByReferral1 == null ? existingCustomer.MembershipApplicantProposedByReferral1 : request.MembershipApplicantProposedByReferral1;

                    existingCustomer.MembershipApplicantProposedByReferral2 = request.MembershipApplicantProposedByReferral2 == null ? existingCustomer.MembershipApplicantProposedByReferral2 : request.MembershipApplicantProposedByReferral2;
                    existingCustomer.Matricule = request.Matricule;
                    existingCustomer.VillageOfOrigin = request.VillageOfOrigin;
                    existingCustomer.MembershipApprovalBy = request.MembershipApprovalBy == null ? existingCustomer.MembershipApprovalBy : request.MembershipApprovalBy;

                    existingCustomer.MembershipApprovalStatus = request.MembershipApprovalStatus == null ? existingCustomer.MembershipApprovalStatus : request.MembershipApprovalStatus;

                    existingCustomer.MembershipApprovedDate = request.MembershipApprovedDate == null ? existingCustomer.MembershipApprovedDate : request.MembershipApprovedDate;

                    existingCustomer.MembershipAllocatedNumber = request.MembershipAllocatedNumber == null ? existingCustomer.MembershipAllocatedNumber : request.MembershipAllocatedNumber;    
                    
                    existingCustomer.Fax = request.Fax == null ? existingCustomer.Fax : request.Fax;
                    existingCustomer.CountryId = request.CountryId == null ? existingCustomer.CountryId : request.CountryId;
                    existingCustomer.TownId = request.TownId == null ? existingCustomer.TownId : request.TownId;
                    existingCustomer.DivisionId = request.DivisionId == null ? existingCustomer.DivisionId : request.DivisionId;

                    existingCustomer.RegionId = request.RegionId == null ? existingCustomer.RegionId : request.RegionId;

                    existingCustomer.MobileOrOnLineBankingLoginState = request.MobileOrOnLineBankingLoginState == null ? existingCustomer.MobileOrOnLineBankingLoginState : request.MobileOrOnLineBankingLoginState;

                    existingCustomer.IsUseOnLineMobileBanking = request.IsUseOnLineMobileBanking;
                    existingCustomer.Income = request.Income;
                    existingCustomer.NumberOfKids = request.NumberOfKids;

                    existingCustomer.MaritalStatus = request.MaritalStatus == null ? existingCustomer.MaritalStatus : request.MaritalStatus;

                    existingCustomer.SpouseName = request.SpouseName == null ? existingCustomer.SpouseName : request.SpouseName;
                    existingCustomer.SpouseAddress = request.SpouseAddress == null ? existingCustomer.SpouseAddress : request.SpouseAddress;

                    existingCustomer.SpouseOccupation = request.SpouseOccupation == null ? existingCustomer.SpouseOccupation : request.SpouseOccupation;
                    existingCustomer.SpouseContactNumber = request.SpouseContactNumber == null ? existingCustomer.SpouseContactNumber : request.SpouseContactNumber;
                    existingCustomer.CustomerCategoryId = request.CustomerCategoryId == null ? existingCustomer.CustomerCategoryId : request.CustomerCategoryId;
                    existingCustomer.ActiveStatus = request.ActiveStatus == null ? existingCustomer.ActiveStatus : request.ActiveStatus;

                    existingCustomer.ModifiedDate =DateTime.Now;
                  

                    _CustomerRepository.Update(existingCustomer);
                    if (await _uow.SaveAsync() <= 0)
                    {
                        return ServiceResponse<UpdateCustomer>.Return500();
                    }
                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<UpdateCustomer>.ReturnResultWith200(_mapper.Map<UpdateCustomer>(existingCustomer));
                    _logger.LogInformation($"Customer {request.CustomerId} was successfully updated.");
                    return response;
                }
                else
                {
                    // If the Customer entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.CustomerId} was not found to be updated.";
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
