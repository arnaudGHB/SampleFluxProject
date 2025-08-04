using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.TellerP.Queries;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Queries;
using CBS.TransactionManagement.MediatR.RemittanceP.Queries;
using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Repository.RemittanceP;
using CBS.TransactionManagement.Helper.Helper.Pagging;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Handlers
{
    public class GetAllRemittanceQueryHandler : IRequestHandler<GetAllRemittanceQuery, ServiceResponse<CustomDataTable>>
    {
        private readonly IRemittanceRepository _remittanceRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetAllRemittanceQueryHandler> _logger;
        private readonly UserInfoToken _userInfoToken;

        public GetAllRemittanceQueryHandler(
            IRemittanceRepository remittanceRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllRemittanceQueryHandler> logger)
        {
            _remittanceRepository = remittanceRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<CustomDataTable>> Handle(GetAllRemittanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var options = request.DataTableOptions;
                var query = _remittanceRepository.All.AsNoTracking().Where(x => !x.IsDeleted);

                // Filter by Date Range
                if (request.ByDateRange)
                {
                    query = query.Where(x => x.CreatedDate >= request.DateFrom && x.CreatedDate <= request.DateTo);
                }

                // Filter by Status
                if (!string.IsNullOrEmpty(request.Status) && request.Status.ToLower() != "all")
                {
                    query = query.Where(x => x.Status.ToLower() == request.Status.ToLower());
                }

                // Apply Branch ID Filter
                if (!string.IsNullOrEmpty(request.BranchId) && request.BranchId.ToLower() != "all")
                {
                    query = query.Where(x => x.SourceBranchId == request.BranchId || x.ReceivingBranchId == request.BranchId);
                }

                // Apply Query Filters
                if (!string.IsNullOrEmpty(request.QueryParameter) && !string.IsNullOrEmpty(request.QueryValue))
                {
                    query = request.QueryParameter.ToLower() switch
                    {
                        "sourcebranchid" => query.Where(x => x.SourceBranchId == request.QueryValue),
                        "receivingbranchid" => query.Where(x => x.ReceivingBranchId == request.QueryValue),
                        "accountnumber" => query.Where(x => x.AccountNumber == request.QueryValue),
                        "sendername" => query.Where(x => x.SenderName.Contains(request.QueryValue)),
                        "receivername" => query.Where(x => x.ReceiverName.Contains(request.QueryValue)),
                        "transactionreference" => query.Where(x => x.TransactionReference.Contains(request.QueryValue)),
                        "externalreference" => query.Where(x => x.ExternalReference.Contains(request.QueryValue)),
                        "sendercni" => query.Where(x => x.SenderCNI.Contains(request.QueryValue)),
                        "receivercni" => query.Where(x => x.ReceiverCNI.Contains(request.QueryValue)),
                        "senderphonenumber" => query.Where(x => x.SenderPhoneNumber.Contains(request.QueryValue)),
                        "receiverphonenumber" => query.Where(x => x.ReceiverPhoneNumber.Contains(request.QueryValue)),
                        "branchid" => query.Where(x => x.SourceBranchId == request.QueryValue || x.ReceivingBranchId == request.QueryValue),
                        _ => query
                    };
                }

                // Global Search
                if (!string.IsNullOrEmpty(options.searchValue))
                {
                    string searchValue = options.searchValue.ToLower();
                    query = query.Where(x => x.SenderName.ToLower().Contains(searchValue) ||
                                             x.ReceiverName.ToLower().Contains(searchValue) ||
                                             x.AccountNumber.Contains(searchValue) ||
                                             x.SourceBranchName.ToLower().Contains(searchValue) ||
                                             x.ReceivingBranchName.ToLower().Contains(searchValue) ||
                                             x.TransactionReference.ToLower().Contains(searchValue) ||
                                             x.ExternalReference.ToLower().Contains(searchValue) ||
                                             x.SenderCNI.ToLower().Contains(searchValue) ||
                                             x.ReceiverCNI.ToLower().Contains(searchValue) ||
                                             x.SenderPhoneNumber.Contains(searchValue) ||
                                             x.ReceiverPhoneNumber.Contains(searchValue));
                }

                int recordsTotal = await query.CountAsync(cancellationToken);

                // Sorting
                if (!string.IsNullOrEmpty(options.sortColumnName) && !string.IsNullOrEmpty(options.sortColumnDirection))
                {
                    query = options.sortColumnDirection.ToLower() == "asc"
                        ? query.OrderBy(x => EF.Property<object>(x, options.sortColumnName))
                        : query.OrderByDescending(x => EF.Property<object>(x, options.sortColumnName));
                }
                else
                {
                    query = query.OrderByDescending(x => x.CreatedDate);
                }

                // Pagination
                var data = await query.Skip(options.start).Take(options.length).ToListAsync(cancellationToken);

                var mappedData = _mapper.Map<List<RemittanceDto>>(data);
                var response = new CustomDataTable(
                    draw: int.Parse(options.draw),
                    recordsTotal: recordsTotal,
                    recordsFiltered: recordsTotal,
                    data: mappedData,
                    dataTableOptions: options);

                return ServiceResponse<CustomDataTable>.ReturnResultWith200(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching remittance records: {ex.Message}", ex);
                return ServiceResponse<CustomDataTable>.Return500(ex, "Failed to retrieve remittance records.");
            }
        }
    }
}
