using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.CUSTOMER.HELPER.Helper;
using CBS.CUSTOMER.MEDIATR.CustomerMediatR.Queries;
using CBS.CUSTOMER.REPOSITORY;
using CBS.CUSTOMER.DATA.Entity;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using CBS.CUSTOMER.DATA.Dto;
using System.Collections;
using Microsoft.IdentityModel.Tokens;
using CBS.CUSTOMER.MEDIATR.Customer.Queries;
using CBS.CUSTOMER.DOMAIN.Context;
using CBS.CUSTOMER.COMMON.UnitOfWork;
using System.Text.RegularExpressions;

namespace CBS.CUSTOMER.MEDIATR
{
    /// <summary>
    /// Handles the retrieval of members based on their Customer Type.
    /// </summary>
    /// <param name="request">The query containing the Customer Type.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A service response containing a list of members matching the given Customer Type.</returns>
    public class GetMembersByCustomerTypeQueryHandler : IRequestHandler<GetMembersByCustomerTypeQuery, ServiceResponse<List<CustomerDto>>>
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetMembersByCustomerTypeQueryHandler> _logger;
        private readonly UserInfoToken _userInfoToken;

        public GetMembersByCustomerTypeQueryHandler(
            ICustomerRepository customerRepository,
            IMapper mapper,
            ILogger<GetMembersByCustomerTypeQueryHandler> logger,
            UserInfoToken userInfoToken)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
            _logger = logger;
            _userInfoToken = userInfoToken;
        }

        public async Task<ServiceResponse<List<CustomerDto>>> Handle(GetMembersByCustomerTypeQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var members = new List<CBS.CUSTOMER.DATA.Entity.Customer>();
                string customerTypeInfo = string.IsNullOrWhiteSpace(request.CustomerType) ? "all" : request.CustomerType;
                string branchInfo = $"Branch name: {_userInfoToken.BranchName}";

                // Determine if fetching all members or filtering by CustomerType
                if (string.IsNullOrWhiteSpace(request.CustomerType) || request.CustomerType=="all")
                {
                    string fetchAllMessage = $"[INFO] Fetching all collective salary members for {branchInfo}.";
                    _logger.LogInformation(fetchAllMessage);
                    await BaseUtilities.LogAndAuditAsync(fetchAllMessage, request, HttpStatusCodeEnum.OK, LogAction.Read, LogLevelInfo.Information);

                    members = await _customerRepository
                        .FindBy(m => m.BranchId == _userInfoToken.BranchID && !m.IsDeleted && m.CustomerType == "SalaryCollection")
                        .ToListAsync();
                }
                else
                {
                    string fetchFilteredMessage = $"[INFO] Fetching members with CustomerType: '{request.CustomerType}' for {branchInfo}.";
                    _logger.LogInformation(fetchFilteredMessage);
                    await BaseUtilities.LogAndAuditAsync(fetchFilteredMessage, request, HttpStatusCodeEnum.OK, LogAction.Read, LogLevelInfo.Information);

                    members = await _customerRepository
                        .FindBy(m => m.CustomerType == request.CustomerType && m.BranchId == _userInfoToken.BranchID && !m.IsDeleted)
                        .ToListAsync();
                }

                // Handle case where no members are found
                if (members == null || !members.Any())
                {
                    string noRecordsMessage = $"[WARNING] No members found for CustomerType: '{customerTypeInfo}' in {branchInfo}.";
                    _logger.LogWarning(noRecordsMessage);
                    await BaseUtilities.LogAndAuditAsync(noRecordsMessage, request, HttpStatusCodeEnum.NotFound, LogAction.Read, LogLevelInfo.Warning);
                    return ServiceResponse<List<CustomerDto>>.Return404(noRecordsMessage);
                }

                // Map entities to DTOs
                var memberDtos = _mapper.Map<List<CustomerDto>>(members);

                // Log success message with total members retrieved
                string successMessage = $"[SUCCESS] Retrieved {members.Count} members for CustomerType: '{customerTypeInfo}' in {branchInfo}.";
                _logger.LogInformation(successMessage);
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.Read, LogLevelInfo.Information);

                return ServiceResponse<List<CustomerDto>>.ReturnResultWith200(memberDtos);
            }
            catch (Exception ex)
            {
                string errorMessage = $"[ERROR] Failed to retrieve members for CustomerType: '{request.CustomerType ?? "All"}' in Branch ID: '{_userInfoToken.BranchName}'. Error: {ex.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.Read, LogLevelInfo.Error);
                return ServiceResponse<List<CustomerDto>>.Return500(ex);
            }
        }
    }


}
