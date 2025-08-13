using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using System.Linq;
using CBS.TransactionManagement.Repository.AccountingDayOpening;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Queries.AccountingDayOpening;
using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;

namespace CBS.TransactionManagement.Handlers.AccountingDayOpening
{
    public class GetAllAccountingDayQueryHandler : IRequestHandler<GetAccountingDayQuery, ServiceResponse<List<AccountingDayDto>>>
    {
        private readonly IAccountingDayRepository _accountingDayRepository; // Repository for accessing AccountingDay data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllAccountingDayQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMediator _mediator; // Mediator for sending commands.

        /// <summary>
        /// Constructor for initializing the GetAllAccountingDayQueryHandler.
        /// </summary>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="accountingDayRepository">Repository for AccountingDay data access.</param>
        /// <param name="mediator">Mediator for sending commands.</param>
        public GetAllAccountingDayQueryHandler(
            IMapper mapper,
            ILogger<GetAllAccountingDayQueryHandler> logger,
            IAccountingDayRepository accountingDayRepository,
            IMediator mediator)
        {
            _mapper = mapper;
            _logger = logger;
            _accountingDayRepository = accountingDayRepository;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the GetAccountingDayQuery to retrieve all AccountingDays.
        /// </summary>
        /// <param name="request">The GetAccountingDayQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>

        public async Task<ServiceResponse<List<AccountingDayDto>>> Handle(GetAccountingDayQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Initialize query with the base condition and date filters
                IQueryable<AccountingDay> query = _accountingDayRepository
                    .FindBy(x => !x.IsDeleted &&
                                (request.DateFrom.Date == default(DateTime) || x.Date.Date >= request.DateFrom.Date) &&
                                (request.DateTo.Date == default(DateTime) || x.Date.Date <= request.DateTo.Date))
                    .AsNoTracking(); // No tracking for read-only query

                // Further filter by branch if necessary
                if (request.ByBranch)
                {
                    query = query.Where(a => a.BranchId == request.BranchId);
                }

                // Execute the query and map results to DTOs
                var entities = await query.ToListAsync(cancellationToken);
                var accountingDayDtos = _mapper.Map<List<AccountingDayDto>>(entities);

                // Ensure all nullable date properties are properly handled in the DTOs
                foreach (var dto in accountingDayDtos)
                {
                    dto.OpenedAt = dto.OpenedAt ?? DateTime.MinValue;
                    dto.ClosedAt = dto.ClosedAt ?? DateTime.MinValue;
                    dto.ReOpenedDate = dto.ReOpenedDate ?? DateTime.MinValue;
                    dto.IsClosed = dto.IsClosed;
                    dto.IsCentralized = dto.IsCentralized;
                    dto.ClosedBy = dto.ClosedBy ?? "N/A";
                    dto.OpenedBy = dto.OpenedBy ?? "N/A";
                    dto.Note = dto.Note ?? "No notes available.";
                    dto.Id = dto.Id;
                    dto.BranchId = dto.BranchId;
                    dto.Date = dto.Date;
                }
                // Return successful response with the data
                return ServiceResponse<List<AccountingDayDto>>.ReturnResultWith200(accountingDayDtos);
            }
            catch (Exception ex)
            {
                // Log the exception details
                _logger.LogError($"Failed to get all accounting days: {ex.Message}");
                if (ex.InnerException != null)
                {
                    _logger.LogError($"Inner exception: {ex.InnerException.Message}");
                }

                // Return error response
                return ServiceResponse<List<AccountingDayDto>>.Return500(ex, "Failed to get all accounting days");
            }
        }


        //public async Task<ServiceResponse<List<AccountingDayDto>>> Handle(GetAccountingDayQuery request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        // Retrieve branches information



        //        if (!request.ByBranch)
        //        {
        //            var getBranchesCommand = new GetBranchesCommand();
        //            var branchResponse = await _mediator.Send(getBranchesCommand);
        //            if (branchResponse.StatusCode == 200)
        //            {
        //                // Retrieve accounting days for the given date
        //                var entities = await _accountingDayRepository.GetAccountingDays(request.Date, branchResponse.Data, request.QueryParameter, request.ByBranch);

        //                // Map entities to DTOs and return successful response
        //                var accountingDayDtos = _mapper.Map<List<AccountingDayDto>>(entities);
        //                return ServiceResponse<List<AccountingDayDto>>.ReturnResultWith200(accountingDayDtos);
        //            }
        //            else
        //            {
        //                // Return error response if branch retrieval failed
        //                _logger.LogError($"Failed to retrieve branches with status code {branchResponse.StatusCode}");
        //                return ServiceResponse<List<AccountingDayDto>>.Return404("Branches not found");
        //            }
        //        }
        //        else
        //        {

        //            var entities = await _accountingDayRepository.GetAccountingDays(request.Date, null, request.QueryParameter, request.ByBranch);

        //            // Map entities to DTOs and return successful response
        //            var accountingDayDtos = _mapper.Map<List<AccountingDayDto>>(entities);
        //            return ServiceResponse<List<AccountingDayDto>>.ReturnResultWith200(accountingDayDtos);



        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception details
        //        _logger.LogError($"Failed to get all accounting days: {ex.Message}");
        //        if (ex.InnerException != null)
        //        {
        //            _logger.LogError($"Inner exception: {ex.InnerException.Message}");
        //        }

        //        // Return error response
        //        return ServiceResponse<List<AccountingDayDto>>.Return500(ex, "Failed to get all accounting days");
        //    }
        //}



    }
}
