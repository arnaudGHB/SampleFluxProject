using AutoMapper;
using CBS.AccountManagement.Common;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Commands;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the command to add a new BudgetPeriod.
    /// </summary>
    public class AddBudgetPeriodCommandHandler : IRequestHandler<AddBudgetPeriodCommand, ServiceResponse<List<BudgetPeriodDto>>>
    {
        private readonly IBudgetPeriodRepository _budgetPeriodRepository; // Repository for accessing BudgetPeriod data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddBudgetPeriodCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<POSContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        /// <summary>
        /// Constructor for initializing the AddBudgetPeriodCommandHandler.
        /// </summary>
        /// <param name="BudgetPeriodRepository">Repository for BudgetPeriod data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddBudgetPeriodCommandHandler(
            IBudgetPeriodRepository budgetPeriodRepository,
            IMapper mapper,
            ILogger<AddBudgetPeriodCommandHandler> logger,
            IUnitOfWork<POSContext> uow, UserInfoToken userInfoToken)
        {
            _budgetPeriodRepository = budgetPeriodRepository;
            _mapper = mapper;
            _logger = logger;
            _uow = uow;
            _userInfoToken = userInfoToken;
        }

        /// <summary>
        /// Handles the AddBudgetPeriodCommand to add a new BudgetPeriod.
        /// </summary>
        /// <param name="request">The AddBudgetPeriodCommand containing BudgetPeriod data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<BudgetPeriodDto>>> Handle(AddBudgetPeriodCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = string.Empty;
            List<BudgetPeriodDto> budgetPeriods = new List<BudgetPeriodDto>();
            try
            {
                var existingBudgetPeriod = await _budgetPeriodRepository.FindAsync(request.Year);

                // Check if the AccountFeature entity exists
                if (existingBudgetPeriod != null)
                {
                    errorMessage = $"BudgetPeriod {request.Year} has already been set  successfully.";


                    _logger.LogInformation(errorMessage);
                    return ServiceResponse<List<BudgetPeriodDto>>.Return404(errorMessage);
                }
                int Year = Convert.ToInt32(request.Year);
                    // Check if a BudgetPeriod with the same name already exists (case-insensitive)
                var BudgetPeriods = _budgetPeriodRepository.GenerateBudgetPeriods(Year, Year);
                // If a BudgetPeriod with the same name already exists, return a conflict response

                _budgetPeriodRepository.AddRange(BudgetPeriods);
                await _uow.SaveAsync();
             budgetPeriods = _mapper.Map(BudgetPeriods, budgetPeriods);  


                return ServiceResponse<List<BudgetPeriodDto>>.ReturnResultWith200(budgetPeriods);
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                  errorMessage = $"Error occurred while saving BudgetPeriod: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<List<BudgetPeriodDto>>.Return500(e);
            }
        }
    }
}