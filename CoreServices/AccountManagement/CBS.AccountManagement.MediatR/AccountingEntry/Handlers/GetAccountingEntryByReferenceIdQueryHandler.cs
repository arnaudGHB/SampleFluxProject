using AutoMapper;
using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Enum;
using CBS.AccountManagement.Helper;
using CBS.AccountManagement.MediatR.Queries;
using CBS.AccountManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Design;
using CBS.AccountManagement.MediatR.Commands;

namespace CBS.AccountManagement.MediatR.Handlers
{
    /// <summary>
    /// Handles the retrieval of all Accounts based on the GetAllAccountQuery.
    /// </summary>
    public class GetAccountingEntryByReferenceIdQueryHandler : 
        IRequestHandler<GetAccountingEntryByReferenceIdQuery, ServiceResponse<List<AccountingEntryDto>>>
    {
        private readonly IAccountingEntryRepository _accountingEntryRepository; // Repository for accessing Accounts data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAccountingEntryByReferenceIdQueryHandler> _logger; // Logger for logging handler actions and errors.

        /// <summary>
        /// Constructor for initializing the GetAllAccountQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Accounts data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAccountingEntryByReferenceIdQueryHandler(
            IAccountingEntryRepository accountingEntryRepository,
            IMapper mapper, ILogger<GetAccountingEntryByReferenceIdQueryHandler> logger)
        {
            // Assign provided dependencies to local variables.
            _accountingEntryRepository = accountingEntryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Handles the query for retrieving accounting entries based on specific query options. 
        /// </summary>
        /// <param name="request">The GetAllAccountQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<AccountingEntryDto>>> Handle(GetAccountingEntryByReferenceIdQuery request, CancellationToken cancellationToken)
        {
            string errorMessage = null;

            try
            {
                // Retrieve accounting entries based on the given ReferenceId
                var entities = await GetEntriesAsync(request.ReferenceId);

                // Check if entries exist
                if (entities == null || !entities.Any())
                {
                    errorMessage = $"No accounting entries found for ReferenceId: {request.ReferenceId}.";
                    _logger.LogWarning(errorMessage);

                    // Log and Audit
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.GetAccountingEntryByReferenceIdQuery, LogLevelInfo.Warning);

                    return ServiceResponse<List<AccountingEntryDto>>.Return404();
                }

                // Map entities to DTOs
                var resultDto = _mapper.Map<List<AccountingEntryDto>>(entities);

                errorMessage = $"Successfully retrieved {resultDto.Count} accounting entries for ReferenceId: {request.ReferenceId}.";
                _logger.LogInformation(errorMessage);

                // Log and Audit Success
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.OK, LogAction.GetAccountingEntryByReferenceIdQuery, LogLevelInfo.Information);

                return ServiceResponse<List<AccountingEntryDto>>.ReturnResultWith200(resultDto);
            }
            catch (Exception ex)
            {
                errorMessage = $"Error occurred while retrieving accounting entries for ReferenceId: {request.ReferenceId}. Error: {ex.Message}";
                _logger.LogError(ex, errorMessage);

                // Log and Audit Error
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.GetAccountingEntryByReferenceIdQuery, LogLevelInfo.Error);

                return ServiceResponse<List<AccountingEntryDto>>.Return500(ex, "Failed to retrieve accounting entries.");
            }
        }


        private async Task<List<Data.AccountingEntry>> GetEntriesAsync(string referenceId)
        {
           
                return await _accountingEntryRepository.All.Where(x=>x.ReferenceID.Equals(referenceId)).ToListAsync();
           
        }

  

    }
}