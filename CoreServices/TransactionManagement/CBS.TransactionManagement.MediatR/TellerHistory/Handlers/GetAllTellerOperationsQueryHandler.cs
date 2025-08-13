 using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Queries;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.Repository.ReversalRequestP;
using CBS.TransactionManagement.Data;

namespace CBS.TransactionManagement.Handlers
{
    /// <summary>
    /// Handles the retrieval of all TellerHistory based on the GetAllTellerHistoryQuery.
    /// </summary>
    public class GetAllTellerOperationsQueryHandler : IRequestHandler<GetAllTellerOperationsQuery, ServiceResponse<List<TellerOperationGL>>>
    {
        private readonly ITellerOperationRepository _tellerOperationRepository; // Repository for accessing TellerHistory data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<GetAllTellerOperationsQueryHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly ITransactionRepository _transactionRepository;
        private readonly ITellerRepository _tellerRepository;

        /// <summary>
        /// Constructor for initializing the GetAllTellerHistoryQueryHandler.
        /// </summary>
        /// <param name="TellerHistoryRepository">Repository for TellerHistory data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public GetAllTellerOperationsQueryHandler(
            IPrimaryTellerProvisioningHistoryRepository TellerHistoryRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper, ILogger<GetAllTellerOperationsQueryHandler> logger, ITellerOperationRepository tellerOperationRepository = null, ITransactionRepository transactionRepository = null, ITellerRepository tellerRepository = null)
        {
            // Assign provided dependencies to local variables.
            _userInfoToken = UserInfoToken;
            _mapper = mapper;
            _logger = logger;
            _tellerOperationRepository = tellerOperationRepository;
            _transactionRepository = transactionRepository;
            _tellerRepository = tellerRepository;
        }

        /// <summary>
        /// Handles the GetAllTellerHistoryQuery to retrieve all TellerHistory.
        /// </summary>
        /// <param name="request">The GetAllTellerHistoryQuery containing query parameters.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<TellerOperationGL>>> Handle(GetAllTellerOperationsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Initialize the query to get all TellerOperations based on the request parameters
                var tellerOperationsQuery = _tellerOperationRepository.All.AsQueryable();

                // Filter by Branch if needed
                if (request.IsByBranch)
                {
                    tellerOperationsQuery = tellerOperationsQuery.Where(t => t.BranchId == request.BranchId);
                }

                // Filter by Date if needed
                if (request.IsByDate)
                {
                    DateTime dateFrom = Convert.ToDateTime(request.DateFrom).Date;
                    DateTime dateTo = Convert.ToDateTime(request.DateTo).Date;
                    tellerOperationsQuery = tellerOperationsQuery.Where(t => t.Date.Value.Date >= dateFrom && t.Date.Value.Date <= dateTo);
                }

                // Filter by Teller if needed
                if (request.IsByTeller)
                {
                    //var teller = await _tellerRepository.FindAsync(request.TellerId);
                    //if (teller.TellerType==TellerType.MobileMoneyMTN.ToString())
                    //{
                    //    tellerOperationsQuery = tellerOperationsQuery.Where(t => t.EventName== TellerType.MobileMoneyMTN.ToString());
                    //}

                    //else if (teller.TellerType == TellerType.MobileMoneyORANGE.ToString())
                    //{
                    //    tellerOperationsQuery = tellerOperationsQuery.Where(t => t.EventName == TellerType.MobileMoneyORANGE.ToString());

                    //}
                    //else if (teller.TellerType == TellerType.MomocashCollectionMTN.ToString())
                    //{
                    //    tellerOperationsQuery = tellerOperationsQuery.Where(t =>t.EventName == TellerType.MomocashCollectionMTN.ToString());

                    //}
                    //else if (teller.TellerType == TellerType.MomocashCollectionOrange.ToString())
                    //{
                    //    tellerOperationsQuery = tellerOperationsQuery.Where(t => t.EventName == TellerType.MomocashCollectionOrange.ToString());

                    //}
                    //else
                    //{
                    //    tellerOperationsQuery = tellerOperationsQuery.Where(t => t.TellerID == request.TellerId);
                    //}
                    tellerOperationsQuery = tellerOperationsQuery.Where(t => t.TellerID == request.TellerId);
                }
                if (request.QueryString== "CashOperations")
                {
                    tellerOperationsQuery = tellerOperationsQuery.Where(t => t.IsCashOperation);
                }
                else if (request.QueryString == "NoneCashOperations")
                {
                    tellerOperationsQuery = tellerOperationsQuery.Where(t => !t.IsCashOperation);
                }
                // Filter by Status based on QueryString
                if (!string.IsNullOrEmpty(request.QueryString))
                {
                    switch (request.QueryString.ToLower())
                    {
                        case "deposit":
                            tellerOperationsQuery = tellerOperationsQuery.Where(t => t.EventName.ToLower() == "deposit");
                            break;
                        case "withdrawal":
                            tellerOperationsQuery = tellerOperationsQuery.Where(t => t.EventName.ToLower() == "withdrawal");
                            break;
                        case "transfer":
                            tellerOperationsQuery = tellerOperationsQuery.Where(t => t.EventName.ToLower() == "transfer");
                            break;
                        default:
                            // If QueryString does not match any specific status, include all statuses
                            break;
                    }
                }

                // Execute the query and get the TellerOperation entities
                var tellerOperations = await tellerOperationsQuery.OrderBy(t => t.CreatedDate).ToListAsync(cancellationToken);

                // Initialize variables for processing
                List<TellerOperationGL> glList = new List<TellerOperationGL>();
                DateTime currentDay = DateTime.MinValue;
                decimal runningBalance = 0;

                // Process each TellerOperation
                foreach (var tellerOperation in tellerOperations)
                {
                    if (tellerOperation.Date.HasValue)
                    {
                        // Check if we are in a new day
                        if (tellerOperation.Date.Value.Date != currentDay.Date)
                        {
                            currentDay = tellerOperation.Date.Value.Date;
                            runningBalance = tellerOperation.PreviousBalance;
                        }
                        // Create a new TellerOperationGL object
                        TellerOperationGL gl = new TellerOperationGL
                        {
                            Amount = tellerOperation.Amount,
                            AccountNumber = tellerOperation.AccountNumber,
                            TransactionType = tellerOperation.TransactionType,
                            TransactionRef = tellerOperation.TransactionReference,
                            BalanceBF = runningBalance,
                            Debit = tellerOperation.OperationType.ToLower() == "debit" ? Math.Abs(tellerOperation.Amount) : 0,
                            Credit = tellerOperation.OperationType.ToLower() == "credit" ? Math.Abs(tellerOperation.Amount) : 0,
                            Naration = $"{tellerOperation.TransactionType}, {tellerOperation.CustomerId}, {tellerOperation.MemberName}, {tellerOperation.TransactionReference}",
                            TellerID = tellerOperation.TellerID,
                            Date = tellerOperation.Date.Value,  
                            Description = tellerOperation.Description,
                            MemberAccountNumber = tellerOperation.MemberAccountNumber,
                            MemberId = tellerOperation.CustomerId,
                            BranchId = tellerOperation.BranchId, EntryDate=tellerOperation.CreatedDate,
                            DailyReferences = tellerOperation.ReferenceId,
                            MemberName = tellerOperation.MemberName,
                        };

                        // Update running balance
                        runningBalance += gl.Credit - gl.Debit;
                        decimal balance = tellerOperation.Amount + tellerOperation.PreviousBalance;
                        //gl.Balance = balance;
                        gl.Balance = runningBalance;
                        // Add the new object to the list
                        glList.Add(gl);
                    }
                }

                // Log the successful retrieval
                await APICallHelper.AuditLogger(_userInfoToken.FullName, LogAction.Read.ToString(), request, "Till statement (Till F5) returned successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Return the result
                return ServiceResponse<List<TellerOperationGL>>.ReturnResultWith200(glList);
            }
            catch (Exception e)
            {
                // Log the error
                _logger.LogError($"Failed to get all ReversalRequest: {e.Message}");
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Read.ToString(), request, $"Failed to get all ReversalRequest: {e.Message}", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                // Return the error response
                return ServiceResponse<List<TellerOperationGL>>.Return500(e, "Failed to get all ReversalRequest");
            }
        }

    }
}
