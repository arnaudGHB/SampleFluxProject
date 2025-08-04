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
    /// Handles the retrieval of all Customers based on the GetCustomersByMatriculesQuery.
    /// </summary>
    public class GetCustomersByMatriculesQueryHandler : IRequestHandler<GetCustomersByMatriculesQuery, ServiceResponse<List<CustomerLightDto>>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customers data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetCustomersByMatriculesQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetCustomersByMatriculesQueryHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customers data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCustomersByMatriculesQueryHandler(
            ICustomerRepository CustomerRepository,
       IDocumentBaseUrlRepository DocumentBaseUrlRepository,
            IMapper mapper, ILogger<GetCustomersByMatriculesQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _CustomerRepository = CustomerRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<CustomerLightDto>>> Handle(GetCustomersByMatriculesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Validate input
                if (request.Matricules == null || !request.Matricules.Any())
                {
                    return ServiceResponse<List<CustomerLightDto>>.Return422("Search list cannot be null or empty.");
                }

                // Determine search criteria based on IsMatricule flag
                IQueryable<DATA.Entity.Customer> query;

                if (request.IsMatricule)
                {
                    // Search by Matricule and exclude deleted records
                    query = _CustomerRepository.FindBy(x => request.Matricules.Contains(x.Matricule) && !x.IsDeleted).AsNoTracking();
                }
                else
                {
                    // Search by CustomerId and exclude deleted records
                    query = _CustomerRepository.FindBy(x => request.Matricules.Contains(x.CustomerId) && !x.IsDeleted).AsNoTracking();
                }

                // Fetch customers and directly map to CustomerLightDto
                var customerDtos = await query
                    .Select(x => new CustomerLightDto
                    {
                        CustomerId = x.CustomerId,
                        FirstName = x.FirstName,
                        LastName = x.LastName,
                        FullName = $"{x.FirstName} {x.LastName}",
                        Matricule = x.Matricule,
                        MobileLoginId = x.MobileLoginId,
                        LegalForm = x.LegalForm,
                        MembershipApprovalStatus = x.MembershipApprovalStatus,
                        Gender = x.Gender ?? GenderType.Male.ToString(),
                        Phone = x.Phone,
                        BranchId = x.BranchId,
                        BankId = x.BankId,
                        Language = x.Language,
                        Active = x.Active
                    })
                    .ToListAsync(cancellationToken);

                return ServiceResponse<List<CustomerLightDto>>.ReturnResultWith200(customerDtos);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get all Customers: {e.Message}");
                return ServiceResponse<List<CustomerLightDto>>.Return500(e, "Failed to get all Customers");
            }
        }

    }
}
