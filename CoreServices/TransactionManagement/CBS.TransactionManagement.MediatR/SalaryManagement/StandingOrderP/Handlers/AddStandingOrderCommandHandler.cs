using AutoMapper;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Repository.SalaryManagement.StandingOrderP;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
namespace CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Handlers
{


    public class AddStandingOrderCommandHandler : IRequestHandler<AddStandingOrderCommand, ServiceResponse<StandingOrderDto>>
    {
        private readonly IStandingOrderRepository _standingOrderRepository;
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly ILogger<AddStandingOrderCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly UserInfoToken _userInfoToken;
        private readonly IAccountRepository _accountRepository;

        /// <summary>
        /// Constructor for the AddStandingOrderCommandHandler.
        /// </summary>
        /// <param name="standingOrderRepository">Repository for managing standing orders.</param>
        /// <param name="uow">Unit of work for managing transactions.</param>
        /// <param name="logger">Logger instance for logging operations.</param>
        /// <param name="mapper">Mapper instance for converting entities to DTOs.</param>
        public AddStandingOrderCommandHandler(
            IStandingOrderRepository standingOrderRepository,
            IUnitOfWork<TransactionContext> uow,
            ILogger<AddStandingOrderCommandHandler> logger,
            IMapper mapper,
            UserInfoToken userInfoToken,
            IAccountRepository accountRepository)
        {
            _standingOrderRepository = standingOrderRepository;
            _uow = uow;
            _logger = logger;
            _mapper = mapper;
            _userInfoToken=userInfoToken;
            _accountRepository=accountRepository;
        }

        /// <summary>
        /// Handles the AddStandingOrderCommand to create new standing orders for a member.
        /// </summary>
        /// <param name="request">Command containing standing order details.</param>
        /// <param name="cancellationToken">Cancellation token for the operation.</param>
        /// <returns>ServiceResponse containing the list of newly created standing orders.</returns>
        public async Task<ServiceResponse<StandingOrderDto>> Handle(AddStandingOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var account = new Account();
                if (request.ExternalAccount)
                {
                    account=await _accountRepository.GetAccountByAccountNumber(request.ExternalAccountNumber);
                    request.ExternalAccountHolderName=account.CustomerName;
                    request.DestinationAccountType="None";
                }
                // Validate input
                if (string.IsNullOrEmpty(request.MemberId))
                {
                    string errorMessage = "MemberId is required to create a standing order. MemberId cannot be null or empty.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.StandingOrder, LogLevelInfo.Error, null);
                    return ServiceResponse<StandingOrderDto>.Return422(errorMessage);
                }

                if (request.Amount <= 0)
                {
                    string errorMessage = $"Invalid Amount: {request.Amount}. Amount must be greater than zero.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Forbidden, LogAction.StandingOrder, LogLevelInfo.Error, null);
                    return ServiceResponse<StandingOrderDto>.Return422(errorMessage);
                }

                // Check for duplicate standing order
                var existingOrder = await _standingOrderRepository
                    .FindBy(x => x.MemberId == request.MemberId &&
                                 x.SourceAccountType == request.SourceAccountType &&
                                 x.DestinationAccountType == request.DestinationAccountType &&
                                 x.Purpose == request.Purpose &&
                                 x.IsActive && x.IsDeleted==false)
                    .FirstOrDefaultAsync();

                if (existingOrder != null)
                {
                    string errorMessage = "A similar active standing order already exists for this member.";
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.Conflict, LogAction.StandingOrder, LogLevelInfo.Warning, null);
                    return ServiceResponse<StandingOrderDto>.Return409(errorMessage);
                }

        // Map the request to the StandingOrder entity using AutoMapper
        var standingOrder = _mapper.Map<StandingOrder>(request);

                // Set system fields
                standingOrder.Id = BaseUtilities.GenerateUniqueNumber();
                standingOrder.BranchCode=_userInfoToken.BranchCode;
                standingOrder.BranchId=_userInfoToken.BranchID;
                standingOrder.BranchName=_userInfoToken.BranchName;
                standingOrder.UserName=_userInfoToken.FullName;
                // Add the standing order to the repository
                _standingOrderRepository.Add(standingOrder);

                // Commit the transaction
                await _uow.SaveAsync();

                // Log success
                string successMessage = $"Standing order created successfully for Member: {request.MemberName}.";
                await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.StandingOrder, LogLevelInfo.Information);

                _logger.LogInformation(successMessage);

                // Map the created entity to DTO using AutoMapper
                var standingOrderDto = _mapper.Map<StandingOrderDto>(standingOrder);

                return ServiceResponse<StandingOrderDto>.ReturnResultWith200(standingOrderDto, successMessage);
            }
            catch (Exception ex)
            {
                // Log and audit the error
                string errorMessage = $"Error occurred while creating standing order for Member: {request.MemberName}. Error: {ex.Message}";
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.StandingOrder, LogLevelInfo.Error);

                _logger.LogError(errorMessage);

                return ServiceResponse<StandingOrderDto>.Return500(errorMessage);
            }
        }
    }
}
