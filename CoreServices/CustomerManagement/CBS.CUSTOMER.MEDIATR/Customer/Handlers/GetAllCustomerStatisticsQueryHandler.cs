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

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the retrieval of all Customers based on the GetAllCustomerStatisticsQuery.
    /// </summary>
    public class GetAllCustomerStatisticsQueryHandler : IRequestHandler<GetAllCustomerStatisticsQuery, ServiceResponse<CustomerStatisticsDto>>
    {
        private readonly ICustomerRepository _CustomerRepository; // Repository for accessing Customers data.
        private readonly IDocumentBaseUrlRepository _DocumentBaseUrlRepository;
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllCustomerStatisticsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMediator _meditr; // AutoMapper for object mapping.

        /// <summary>
        /// Constructor for initializing the GetAllCustomerStatisticsQueryHandler.
        /// </summary>
        /// <param name="CustomerRepository">Repository for Customers data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllCustomerStatisticsQueryHandler(
            ICustomerRepository CustomerRepository,
       IDocumentBaseUrlRepository DocumentBaseUrlRepository,
            IMapper mapper, ILogger<GetAllCustomerStatisticsQueryHandler> logger, IMediator meditr)
        {
            // Assign provided dependencies to local variables.
            _DocumentBaseUrlRepository = DocumentBaseUrlRepository;
            _CustomerRepository = CustomerRepository;
            _mapper = mapper;
            _logger = logger;
            _meditr = meditr;
        }

        /// <summary>
        /// Handles the GetAllCustomerStatisticsQuery to retrieve customer statistics by branch or all customers.
        /// </summary>
        /// <param name="request">The GetAllCustomerStatisticsQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<CustomerStatisticsDto>> Handle(GetAllCustomerStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Base query for active, non-deleted customers with necessary filters
                var query = _CustomerRepository.All.AsNoTracking().Where(c => !c.IsDeleted);

                // Apply branch filter if needed
                if (request.QueryParameter?.ToLower() == "bybranch" && !string.IsNullOrEmpty(request.BranchId))
                {
                    query = query.Where(c => c.BranchId == request.BranchId);
                }

                // Calculate total members and branches directly in the database
                var totalMembers = await query.CountAsync(cancellationToken);
                var totalBranches = await query.Select(c => c.BranchId).Distinct().CountAsync(cancellationToken);

                // Group by branch and calculate statistics based on LegalForm directly in the database
                var customerStatisticsByBranches = await query
                    .GroupBy(c => c.BranchId)
                    .Select(group => new CustomerStatisticsByBranchDto
                    {
                        BranchId = group.Key,
                        NumberOfPhysical = group.Count(c => c.LegalForm == LegalForm.Physical_Person.ToString()),
                        NumberOfMoral = group.Count(c => c.LegalForm == LegalForm.Moral_Person.ToString()),
                        NumberOfMembers = group.Count()
                    })
                    .ToListAsync(cancellationToken);

                // Calculate overall statistics
                var totalNumberOfPhysical = await query.CountAsync(c => c.LegalForm == LegalForm.Physical_Person.ToString(), cancellationToken);
                var totalNumberOfMoral = await query.CountAsync(c => c.LegalForm == LegalForm.Moral_Person.ToString(), cancellationToken);

                // Populate the main CustomerStatisticsDto
                var customerStatisticsDto = new CustomerStatisticsDto
                {
                    TotalNumberOfPhysical = totalNumberOfPhysical,
                    TotalNumberOfMoral = totalNumberOfMoral,
                    TotalBranches = totalBranches,
                    TotalMembers = totalMembers,
                    CustomerStatisticsByBranches = customerStatisticsByBranches
                };

                // Return successful response with calculated statistics
                return ServiceResponse<CustomerStatisticsDto>.ReturnResultWith200(customerStatisticsDto);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get customer statistics: {e.Message}");
                return ServiceResponse<CustomerStatisticsDto>.Return500(e, "Failed to get customer statistics");
            }
        }



        public async Task<ServiceResponse<CustomerStatisticsDto>> Handlexxx(GetAllCustomerStatisticsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Define the base query for active, non-deleted customers
                var query = _CustomerRepository.All.AsNoTracking()
                    .Where(c => !c.IsDeleted);

                // Apply branch filter if QueryParameter is "bybranch" and BranchId is provided
                if (request.QueryParameter?.ToLower() == "bybranch" && !string.IsNullOrEmpty(request.BranchId))
                {
                    query = query.Where(c => c.BranchId == request.BranchId);
                }

                // Execute the query and get the customer list
                var customers = await query.ToListAsync();

                // Calculate total branches and total members
                int totalMembers = customers.Count();
                int totalBranches = customers.Select(c => c.BranchId).Distinct().Count();

                // Group by branch and calculate statistics based on LegalForm
                var customerStatisticsByBranches = customers
                    .GroupBy(c => c.BranchId)
                    .Select(group => new CustomerStatisticsByBranchDto
                    {
                        BranchId = group.Key,
                        NumberOfPhysical = group.Count(c => c.LegalForm == LegalForm.Physical_Person.ToString()), // Physical persons
                        NumberOfMoral = group.Count(c => c.LegalForm == LegalForm.Moral_Person.ToString()), // Moral persons
                        NumberOfMembers = group.Count() // Total members in the branch
                    })
                    .ToList();

                // Populate the main CustomerStatisticsDto
                var customerStatisticsDto = new CustomerStatisticsDto
                {
                    TotalNumberOfPhysical = customers.Count(c => c.LegalForm == LegalForm.Physical_Person.ToString()),
                    TotalNumberOfMoral = customers.Count(c => c.LegalForm == LegalForm.Moral_Person.ToString()),
                    TotalBranches = totalBranches,
                    TotalMembers = totalMembers,
                    CustomerStatisticsByBranches = customerStatisticsByBranches
                };

                // Return successful response with calculated statistics
                return ServiceResponse<CustomerStatisticsDto>.ReturnResultWith200(customerStatisticsDto);
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get customer statistics: {e.Message}");
                return ServiceResponse<CustomerStatisticsDto>.Return500(e, "Failed to get customer statistics");
            }
        }


    }
}
