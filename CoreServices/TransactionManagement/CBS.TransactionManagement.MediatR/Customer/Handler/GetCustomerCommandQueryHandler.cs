using AutoMapper;
using CBS.APICaller.Helper.LoginModel.Authenthication;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Queries;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Handler
{
    /// <summary>
    /// Handles the query to retrieve customer details by Customer ID.
    /// </summary>
    public class GetCustomerCommandQueryHandler : IRequestHandler<GetCustomerCommandQuery, ServiceResponse<CustomerDto>>
    {
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly ILogger<GetCustomerCommandQueryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetCustomerCommandQueryHandler"/> class.
        /// </summary>
        public GetCustomerCommandQueryHandler(
            UserInfoToken userInfoToken,
            ILogger<GetCustomerCommandQueryHandler> logger,
            PathHelper pathHelper)
        {
            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
        }

        /// <summary>
        /// Handles the GetCustomerCommandQuery request to fetch customer details by Customer ID.
        /// </summary>
        /// <param name="request">The request containing the Customer ID.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<CustomerDto>> Handle(GetCustomerCommandQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Fetch customer data from the API
                var customerData = await APICallHelper.GetCustomerMapperAsync(
                    _pathHelper.CustomerBaseUrl,
                    _pathHelper.CustomerUrlGet,
                    request.CustomerID,
                    _userInfoToken.Token);

                if (customerData != null)
                {
                    // Map API response to DTO
                    var customerDto = Mapper.MapToDto(customerData);

                    // Log success
                    await BaseUtilities.LogAndAuditAsync(
                        $"Successfully retrieved customer details for Customer ID: {request.CustomerID}",
                        request,
                        HttpStatusCodeEnum.OK,
                        LogAction.GetMemberByMemberReference,
                        LogLevelInfo.Information,
                        request.CustomerID);

                    return ServiceResponse<CustomerDto>.ReturnResultWith200(customerDto);
                }

                // Handle case where no customer data is found
                var notFoundMessage = $"No customer found for Customer ID: {request.CustomerID}";
                await BaseUtilities.LogAndAuditAsync(
                    notFoundMessage,
                    request,
                    HttpStatusCodeEnum.NotFound,
                    LogAction.GetMemberByMemberReference,
                    LogLevelInfo.Warning,
                    request.CustomerID);

                return ServiceResponse<CustomerDto>.Return404(notFoundMessage);
            }
            catch (Exception ex)
            {
                // Construct error message
                var errorMessage = $"Error occurred while retrieving customer for Customer ID: {request.CustomerID}. Error: {ex.Message}";

                // Log and audit the error
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(
                    errorMessage,
                    request,
                    HttpStatusCodeEnum.InternalServerError,
                    LogAction.GetMemberByMemberReference,
                    LogLevelInfo.Error,
                    request.CustomerID);

                return ServiceResponse<CustomerDto>.Return500(ex, errorMessage);
            }
        }
    }
    public static class Mapper
    {
        /// <summary>
        /// Maps a <see cref="CustomerMapper"/> to a <see cref="CustomerDto"/>.
        /// </summary>
        public static CustomerDto MapToDto(CustomerMapper customerMapper)
        {
            return new CustomerDto
            {
                CustomerId = customerMapper.CustomerId,
                FirstName = customerMapper.FirstName,
                LastName = customerMapper.LastName,
                PhotoUrl = customerMapper.PhotoUrl,
                SignatureUrl = customerMapper.SignatureUrl,
                AgeCategoryStatus=customerMapper.AgeCategoryStatus,
                Occupation = customerMapper.Occupation,
                Address = customerMapper.Address,
                IDNumber = customerMapper.IDNumber,
                IDNumberIssueDate = customerMapper.IDNumberIssueDate,
                IDNumberIssueAt = customerMapper.IDNumberIssueAt,
                Language = customerMapper.Language,
                Email = customerMapper.Email,
                Name=customerMapper.FirstName+" "+customerMapper.LastName,
                Matricule=customerMapper.Matricule,
                Phone = customerMapper.Phone,
                Active = customerMapper.Active,
                MembershipApprovalStatus = customerMapper.MembershipApprovalStatus,
                BranchId = customerMapper.BranchId,
                BankId = customerMapper.BankId,
                LegalForm = customerMapper.LegalForm,
                BranchCode = customerMapper.BranchCode
            };
        }
        public static List<CustomerDto> MapToDtoList(IEnumerable<CustomerMapper> customerMappers)
        {
            return customerMappers.Select(MapToDto).ToList();
        }
    }

}

