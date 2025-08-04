using CBS.AccountManagement.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.AccountManagement.MediatR.ChartOfAccount.Queries;
using Microsoft.EntityFrameworkCore;

namespace CBS.AccountManagement.MediatR.Handlers
{
    internal class GetChartOfAccountByAccountNumberQueryHandler : IRequestHandler<GetChartOfAccountByAccountNumberQuery, ServiceResponse<ChartOfAccountDto>>
    {
        private readonly IChartOfAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetChartOfAccountQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetChartOfAccountByAccountNumberQueryHandler(
            IChartOfAccountRepository AccountRepository,
            IMapper mapper,
            ILogger<GetChartOfAccountQueryHandler> logger)
        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the GetChartOfAccountQuery to retrieve a specific ChartOfAccount.
        /// </summary>
        /// <param name="request">The GetChartOfAccountQuery containing Accountnumber to be retrieved.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<ChartOfAccountDto>> Handle(GetChartOfAccountByAccountNumberQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the ChartOfAccount entity with the specified ID from the repository
                var entity =   _AccountRepository.FindBy(p => p.AccountNumber.Equals(request.AccountNumber)).FirstOrDefault();
                if (entity != null)
                {
                    if (entity.IsDeleted)
                    {
                        string message = "ChartOfAccount has been deleted.";
                        // If the AccountCategory entity was not found, log the error and return a 404 Not Found response
                        _logger.LogError(message);
                        return ServiceResponse<ChartOfAccountDto>.Return404(message);
                    }
                    else
                    {
                        // Map the ChartOfAccount entity to ChartOfAccountDto and return it with a success response
                        var AccountDto = _mapper.Map<ChartOfAccountDto>(entity);
                        return ServiceResponse<ChartOfAccountDto>.ReturnResultWith200(AccountDto);
                    }


                }
                else
                {
                    // If the ChartOfAccount entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Account not found.");
                    return ServiceResponse<ChartOfAccountDto>.Return404();
                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                return ServiceResponse<ChartOfAccountDto>.Return500(e);
            }
        }
    }
}
