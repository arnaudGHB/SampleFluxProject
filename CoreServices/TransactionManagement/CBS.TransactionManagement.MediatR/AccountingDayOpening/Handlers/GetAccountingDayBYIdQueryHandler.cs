using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Queries.AccountingDayOpening;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.Command;

namespace CBS.TransactionManagement.Handlers.AccountingDayOpening
{
    public class GetAccountingDayBYIdQueryHandler : IRequestHandler<GetAccountingDayBYIdQuery, ServiceResponse<AccountingDayDto>>
    {
        private readonly IAccountingDayRepository _accountingDayRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAccountingDayBYIdQueryHandler> _logger;
        private readonly UserInfoToken _userInfoToken;
        private readonly IMediator _mediator;

        public GetAccountingDayBYIdQueryHandler(
            IAccountingDayRepository accountingDayRepository,
            IMapper mapper,
            ILogger<GetAccountingDayBYIdQueryHandler> logger,
            UserInfoToken userInfoToken,
            IMediator mediator)
        {
            _accountingDayRepository = accountingDayRepository ?? throw new ArgumentNullException(nameof(accountingDayRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userInfoToken = userInfoToken ?? throw new ArgumentNullException(nameof(userInfoToken));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Handles the retrieval of an accounting day by the specified ID.
        /// </summary>
        /// <param name="request">The query containing the accounting day ID.</param>
        /// <param name="cancellationToken">Cancellation token for the async operation.</param>
        /// <returns>A service response containing the accounting day details or an error message.</returns>
        public async Task<ServiceResponse<AccountingDayDto>> Handle(GetAccountingDayBYIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Step 1: Retrieve the accounting day by the specified ID.
                var accountingDay = await _accountingDayRepository.FindAsync(request.Id);

                if (accountingDay != null)
                {
                    // Step 2: Map the retrieved entity to the DTO.
                    var accountingDayDto = _mapper.Map<AccountingDayDto>(accountingDay);

                    // Step 3: Retrieve branch details using the mediator.
                    var branchByIdCommand = new GetBranchByIdCommand { BranchId = accountingDay.BranchId };
                    var serviceResponse = await _mediator.Send(branchByIdCommand);

                    // Step 4: Set branch information based on the response.
                    if (serviceResponse.StatusCode == 200)
                    {
                        var branch = serviceResponse.Data;
                        accountingDayDto.BranchName = branch.name;
                        accountingDayDto.BranchCode = branch.branchCode;
                        accountingDayDto.BranchId = branch.id;
                    }
                    else
                    {
                        // Default values if branch information is unavailable.
                        accountingDayDto.BranchName = "Centralised System";
                        accountingDayDto.BranchCode = "CS";
                        accountingDayDto.BranchId = "N/A";
                    }

                    // Step 5: Set default values for optional fields if null.
                    accountingDayDto.ReOpenedDate = accountingDayDto.ReOpenedDate ?? DateTime.MinValue;
                    accountingDayDto.Note = accountingDayDto.Note ?? $"Opening of accounting day {accountingDayDto.Date}";

                    // Step 6: Log success and audit the retrieval.
                    var successMessage = $"Accounting day retrieved for branch {accountingDayDto.BranchName} on {DateTime.UtcNow}.";
                    _logger.LogInformation(successMessage);
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.AccountingDayRetrieved, LogLevelInfo.Information);

                    // Return the successful response.
                    return ServiceResponse<AccountingDayDto>.ReturnResultWith200(accountingDayDto);
                }
                else
                {
                    // Step 7: Handle case where the accounting day is not found.
                    var notFoundMessage = $"Accounting day not found for the ID {request.Id}.";
                    _logger.LogWarning(notFoundMessage);
                    await BaseUtilities.LogAndAuditAsync(notFoundMessage, request, HttpStatusCodeEnum.NotFound, LogAction.AccountingDayRetrieved, LogLevelInfo.Warning);

                    return ServiceResponse<AccountingDayDto>.Return404(notFoundMessage);
                }
            }
            catch (Exception e)
            {
                // Step 8: Handle exceptions and log the error.
                var errorMessage = $"Error occurred while retrieving the accounting day for ID {request.Id}: {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.AccountingDayRetrieved, LogLevelInfo.Error);

                return ServiceResponse<AccountingDayDto>.Return500(errorMessage);
            }
        }
    }

}
