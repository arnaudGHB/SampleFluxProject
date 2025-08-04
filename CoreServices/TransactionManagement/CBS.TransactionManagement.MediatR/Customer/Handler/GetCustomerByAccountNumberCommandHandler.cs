using AutoMapper;
using AutoMapper.Internal;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Handlers;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Helper.Helper;
using CBS.TransactionManagement.Queries;
using CBS.TransactionManagement.Repository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Handler
{
    public class GetCustomerByAccountNumberCommandHandler : IRequestHandler<GetCustomerByAccountNumberCommand, ServiceResponse<CustomerDto>>
    {
        private readonly IAccountRepository _AccountRepository; // Repository for accessing Account data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        private readonly ILogger<GetCustomerByAccountNumberCommand> _logger; // Logger for logging handler actions and errors.
        /// <summary>
        /// Constructor for initializing the GetTransactionQueryHandler.
        /// </summary>
        /// <param name="AccountRepository">Repository for Account data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetCustomerByAccountNumberCommandHandler(
            IAccountRepository AccountRepository,
            IMapper mapper,
            UserInfoToken UserInfoToken,
            ILogger<GetCustomerByAccountNumberCommand> logger,
            IMediator mediator = null)

        {
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _userInfoToken = UserInfoToken;
            _logger = logger;
            _mediator = mediator;
        }

        public async Task<ServiceResponse<CustomerDto>> Handle(GetCustomerByAccountNumberCommand request, CancellationToken cancellationToken)
        {
            string errorMessage = null;
            try
            {
                // Retrieve the Account entity with the specified ID from the repository
                var entity = await _AccountRepository.FindBy(a => a.AccountNumber == request.AccountNumber).FirstOrDefaultAsync();

                if (entity == null)
                {
                    // If the Account entity was not found, log the error and return a 404 Not Found response
                    _logger.LogError("Account not found.");
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Account not found.", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                    return ServiceResponse<CustomerDto>.Return404();
                }

                using (HttpClient client = new HttpClient())
                {
                    var requestData = new
                    {
                        userName = "afurlongla@gmail.com",
                        password = "123456",
                        remoteIp = "string",
                        latitude = "string",
                        longitude = "string"
                    };

                    string jsonRequestData = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(jsonRequestData, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync("https://identityserver.prodmomokash.com/api/Authentication/Session/Login", content);
                    response.EnsureSuccessStatusCode();

                    string responseData = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(responseData);

                    // Access the bearerToken from the dynamic response
                    string bearerToken = jsonResponse?.bearerToken;

                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + bearerToken);

                    response = await client.GetAsync("https://cbsclient.prodmomokash.com/api/v1/Customer/" + entity.CustomerId);
                    if (response == null)
                    {
                        errorMessage = $"Error occurred while getting Account Holder: No response from Client Service";
                        // Log the error and return a 500 Internal Server Error response with the error message
                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
                        return ServiceResponse<CustomerDto>.Return500();
                    }

                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        errorMessage = $"Error occurred while getting Account Holder: Account holder not found";
                        // Log the error and return a 500 Internal Server Error response with the error message
                        _logger.LogError(errorMessage);
                        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Account holder information not found", LogLevelInfo.Information.ToString(), 404, _userInfoToken.Token);
                        return ServiceResponse<CustomerDto>.Return404();
                    }
                    response.EnsureSuccessStatusCode(); // Ensure a successful response

                    // Deserialize the response content to MyApiResponse
                    responseData = await response.Content.ReadAsStringAsync();
                    ServiceResponse<CustomerDto> apiResponse = JsonConvert.DeserializeObject<ServiceResponse<CustomerDto>>(responseData);

                    // Access the deserialized CustomerDto
                    //CustomerDto customerDto = apiResponse.Data;
                    await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, "Account holder information returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);
                    return apiResponse;

                }
            }
            catch (Exception e)
            {
                errorMessage = $"Error occurred while getting Account: {e.Message}";

                // Log the error and return a 500 Internal Server Error response with the error message
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, errorMessage, LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);
                return ServiceResponse<CustomerDto>.Return500(e);
            }
        }
    
    }
}
