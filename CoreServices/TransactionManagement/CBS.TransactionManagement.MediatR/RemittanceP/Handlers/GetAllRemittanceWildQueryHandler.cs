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
using CBS.TransactionManagement.Data.Entity.RemittanceP;
using CBS.TransactionManagement.Repository.RemittanceP;

namespace CBS.TransactionManagement.MediatR.RemittanceP.Handlers
{
    public class GetAllRemittanceWildQueryHandler : IRequestHandler<GetAllRemittanceWildQuery, ServiceResponse<List<RemittanceDto>>>
    {
        private readonly IRemittanceRepository _remittanceRepository; // Repository for accessing remittance data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllRemittanceWildQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;

        public GetAllRemittanceWildQueryHandler(
            IRemittanceRepository remittanceRepository,
            UserInfoToken userInfoToken,
            IMapper mapper,
            ILogger<GetAllRemittanceWildQueryHandler> logger)
        {
            _remittanceRepository = remittanceRepository;
            _userInfoToken = userInfoToken;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResponse<List<RemittanceDto>>> Handle(GetAllRemittanceWildQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Start building the query based on the QueryString provided.
                IQueryable<Remittance> query = _remittanceRepository.All.AsNoTracking();

                // Convert QueryString to lower case to ensure case-insensitive comparison.
                var queryString = string.IsNullOrEmpty(request.QueryString) ? string.Empty : request.QueryString.ToLower();
                if (request.Approved)
                {
                    query.Where(x => x.Status.ToLower() == Status.Approved.ToString().ToLower() && x.IsDeleted == false);
                }
                // Determine the search criteria based on the query string.
                if (!string.IsNullOrEmpty(queryString))
                {
                    switch (queryString)
                    {
                        case "cni":
                            // Filter by CNI
                            query = query.Where(x => x.ReceiverCNI.ToLower() == request.QueryValue.ToLower());
                            break;

                        case "senderphonenumber":
                            // Filter by SenderPhoneNumber
                            query = query.Where(x => x.SenderPhoneNumber.ToLower() == request.QueryValue.ToLower());
                            break;

                        case "receiverphonenumber":
                            // Filter by ReceiverPhoneNumber
                            query = query.Where(x => x.ReceiverPhoneNumber.ToLower() == request.QueryValue.ToLower());
                            break;

                        case "sendername":
                            // Filter by SenderName
                            query = query.Where(x => x.SenderName.ToLower().Contains(request.QueryValue.ToLower()));
                            break;

                        case "receivername":
                            // Filter by ReceiverName
                            query = query.Where(x => x.ReceiverName.ToLower().Contains(request.QueryValue.ToLower()));
                            break;

                        case "referencenumber":
                            // Filter by ReferenceNumber
                            query = query.Where(x => x.TransactionReference.ToLower() == request.QueryValue.ToLower());
                            break;

                        default:
                            // If an unknown query string is provided, return an empty result or throw an error.
                            return ServiceResponse<List<RemittanceDto>>.Return400("Invalid query parameter.");
                    }
                }

                // Retrieve the filtered results from the repository
                var remittances = await query.Where(x => x.IsDeleted == false).ToListAsync(cancellationToken);

                // Map the result to a list of DTOs and return the response
                return ServiceResponse<List<RemittanceDto>>.ReturnResultWith200(_mapper.Map<List<RemittanceDto>>(remittances));
            }
            catch (Exception e)
            {
                // Log error and return a 500 Internal Server Error response with error message
                _logger.LogError($"Failed to get remittance records: {e.Message}");
                await BaseUtilities.LogAndAuditAsync(
                    $"Failed to get remittance records: {e.Message}",
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.Remittance,
                    LogLevelInfo.Error);
                return ServiceResponse<List<RemittanceDto>>.Return500($"Failed to get remittance records: {e.Message}");
            }
        }
    }
}
