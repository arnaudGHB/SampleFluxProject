using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;
using CBS.CUSTOMER.DATA.Dto;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the retrieval of Members by BranchId where Matricule is not null or empty.
    /// </summary>
    public class GetCustomersWithMatriculeByBranchQueryHandler : IRequestHandler<GetCustomersWithMatriculeByBranchQuery, ServiceResponse<List<CustomerLightDto>>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCustomersWithMatriculeByBranchQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCustomersWithMatriculeByBranchQueryHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customers data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCustomersWithMatriculeByBranchQueryHandler(
            ICustomerRepository CustomerRepository,
            IMapper mapper, ILogger<GetCustomersWithMatriculeByBranchQueryHandler> logger)
        {
            _CustomerRepository = CustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetCustomersWithMatriculeByBranchQuery to retrieve Members by BranchId and Matricule.
        /// </summary>
        /// <param name="request">The GetCustomersWithMatriculeByBranchQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<CustomerLightDto>>> Handle(GetCustomersWithMatriculeByBranchQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Optimized query to filter by BranchId and Matricule not null or empty
                var query = _CustomerRepository.All
                    .Where(x => x.BranchId == request.BranchId
                                && x.IsDeleted == false
                                && !string.IsNullOrEmpty(x.Matricule))
                    .Select(x => new CustomerLightDto
                    {
                        CustomerId = x.CustomerId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        FullName = string.Join(" ", x.FirstName, x.LastName).Trim(),
                        Matricule = x.Matricule,
                        MobileLoginId = x.MobileLoginId,
                        LegalForm = x.LegalForm,
                        MembershipApprovalStatus = x.MembershipApprovalStatus,
                        Gender = x.Gender,
                        Phone = x.Phone,
                        BranchId = x.BranchId,
                        BankId = x.BankId,
                        Language = x.Language,
                        Active = x.Active,
                        AccountConfirmationNumber = x.AccountConfirmationNumber
                    });

                // Materialize the query to list asynchronously
                var result = await query.ToListAsync(cancellationToken);

                // Return the result with a 200 status code
                return ServiceResponse<List<CustomerLightDto>>.ReturnResultWith200(result);
            }
            catch (Exception e)
            {
                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError($"Failed to get Customers with Matricule: {e.Message}");
                return ServiceResponse<List<CustomerLightDto>>.Return500(e, "Failed to get Customers with Matricule");
            }
        }

    }
}
