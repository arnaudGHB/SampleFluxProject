using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using System.Globalization;
using AutoMapper.Internal;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.MediatR.TellerP.Commands;

namespace CBS.TransactionManagement.MediatR.TellerP.Handlers
{
    /// <summary>
    /// Handles the command to add a new Teller.
    /// </summary>
    public class AddTellerCommandHandler : IRequestHandler<AddTellerCommand, ServiceResponse<TellerDto>>
    {
        private readonly IAccountRepository _AccountRepository;
        private readonly ISavingProductRepository _SavingProductRepository;
        private readonly ITellerRepository _TellerRepository; // Repository for accessing Teller data.
        private readonly IMapper _mapper; // AutoMapper for object mapping.
        private readonly ILogger<AddTellerCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly UserInfoToken _userInfoToken;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddTellerCommandHandler.
        /// </summary>
        /// <param name="TellerRepository">Repository for Teller data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public AddTellerCommandHandler(
            ITellerRepository TellerRepository,
ISavingProductRepository savingProductRepository,
            IAccountRepository AccountRepository,
            UserInfoToken UserInfoToken,
            IMapper mapper,
            ILogger<AddTellerCommandHandler> logger,
            IUnitOfWork<TransactionContext> uow,
            IMediator mediator = null)
        {
            _TellerRepository = TellerRepository;
            _SavingProductRepository = savingProductRepository;
            _AccountRepository = AccountRepository;
            _mapper = mapper;
            _userInfoToken = UserInfoToken;
            _logger = logger;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the AddTellerCommand to add a new Teller.
        /// </summary>
        /// <param name="request">The AddTellerCommand containing Teller data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>


        public async Task<ServiceResponse<TellerDto>> Handle(AddTellerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Check if a Teller exists by name or code for BranchId and bankId
                var teller = await _TellerRepository.FindBy(c => (c.Name == request.Name || c.Code == request.Code) && c.BranchId == request.BranchId && !c.IsDeleted)
                                                      .FirstOrDefaultAsync();

                if (teller != null)
                {
                    var errorMessage = $"Teller with name {request.Name} or code {request.Code} already exists in this bank and branch";
                    LogAndAuditError(errorMessage, request, 409);
                    return ServiceResponse<TellerDto>.Return409(errorMessage);
                }

                // Check for primary teller conflict
                var Pteller = await _TellerRepository.FindBy(c => c.IsPrimary && c.BranchId == request.BranchId && !c.IsDeleted).FirstOrDefaultAsync();
                if (request.IsPrimary && Pteller != null)
                {
                    var errorMessage = $"Primary teller is already set for the selected branch.";
                    LogAndAuditError(errorMessage, request, 409);
                    return ServiceResponse<TellerDto>.Return409(errorMessage);
                }

                // Validate that only one teller of specific types exists per branch
                if (await IsSingleTellerTypePerBranch(request.TellerType, request.BranchId))
                {
                    var errorMessage = $"A teller of type {request.TellerType} already exists in this branch.";
                    LogAndAuditError(errorMessage, request, 409);
                    return ServiceResponse<TellerDto>.Return409(errorMessage);
                }

                // Map the AddTellerCommand to a Teller entity
                var TellerEntity = _mapper.Map<Teller>(request);
                TellerEntity.IsPrimary = request.IsPrimary && await IsPrimaryTellerAvailable(request.BankId, request.BranchId);

                // Validate MinAmount and MaxAmount
                ValidateTellerRequest(request);

                // Convert UTC to local time and set it in the entity
                TellerEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
                TellerEntity.Id = BaseUtilities.GenerateUniqueNumber();

                // Add the new Teller entity to the repository
                _TellerRepository.Add(TellerEntity);

                // Create Teller Account
                var tellerAccount = await CreateTellerAccount(TellerEntity);

                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "Teller created successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

                // Map the Teller entity to TellerDto and return it with a success response
                var TellerDto = _mapper.Map<TellerDto>(TellerEntity);

                return ServiceResponse<TellerDto>.ReturnResultWith200(TellerDto, $"{request.Name} was created successfully");

            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Teller: {e.Message}";
                LogAndAuditError(errorMessage, request, 500, e);
                return ServiceResponse<TellerDto>.Return500(e, errorMessage);
            }
        }

        private async Task<bool> IsSingleTellerTypePerBranch(string tellerType, string branchId)
        {
            // Define the restricted teller types
            var tellerTypesToCheck = new[]
            {
        //"MobileMoneyMTN",
        "MomocashCollectionMTN",
        //"MobileMoneyORANGE",
        "MomocashCollectionOrange",
        "NoneCashTeller"
    };

            // Check if the tellerType is one of the restricted types
            if (tellerTypesToCheck.Contains(tellerType))
            {
                // Query the repository to see if a teller of this type already exists in the branch
                var existingTellerWithType = await _TellerRepository
                    .FindBy(c => c.TellerType == tellerType && c.BranchId == branchId && !c.IsDeleted)
                    .FirstOrDefaultAsync();

                // Return true if an existing teller of this type is found
                return existingTellerWithType != null;
            }

            return false;
        }


        //public async Task<ServiceResponse<TellerDto>> Handle(AddTellerCommand request, CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        // Check if a Teller exists by name or code for BranchId and bankId
        //        var teller = await _TellerRepository.FindBy(c => (c.Name == request.Name || c.Code == request.Code) && c.BranchId == request.BranchId && !c.IsDeleted)
        //                                              .FirstOrDefaultAsync();

        //        if (teller != null)
        //        {
        //            var errorMessage = $"Teller with name {request.Name} or code {request.Code} already exists in this bank and branch";
        //            LogAndAuditError(errorMessage, request, 409);
        //            return ServiceResponse<TellerDto>.Return409(errorMessage);
        //        }

        //        // Check for primary teller conflict
        //        var Pteller = await _TellerRepository.FindBy(c => c.IsPrimary && c.BranchId == request.BranchId && !c.IsDeleted).FirstOrDefaultAsync();
        //        if (request.IsPrimary && Pteller != null)
        //        {
        //            var errorMessage = $"Primary teller is already set for the selected branch.";
        //            LogAndAuditError(errorMessage, request, 409);
        //            return ServiceResponse<TellerDto>.Return409(errorMessage);
        //        }

        //        // Constrain to ensure only one teller of specific types per branch
        //        var tellerTypesToCheck = new[]
        //        {
        //    "MobileMoneyMTN",
        //    "MomocashCollectionMTN",
        //    "MobileMoneyORANGE",
        //    "MomocashCollectionOrange",
        //    "NoneCashTeller"
        //};

        //        if (tellerTypesToCheck.Contains(request.TellerType))
        //        {
        //            var existingTellerWithType = await _TellerRepository.FindBy(c => c.TellerType == request.TellerType && c.BranchId == request.BranchId && !c.IsDeleted)
        //                                                                .FirstOrDefaultAsync();
        //            if (existingTellerWithType != null)
        //            {
        //                var errorMessage = $"A teller of type {request.TellerType} already exists in this branch.";
        //                LogAndAuditError(errorMessage, request, 409);
        //                return ServiceResponse<TellerDto>.Return409(errorMessage);
        //            }
        //        }

        //        // Map the AddTellerCommand to a Teller entity
        //        var TellerEntity = _mapper.Map<Teller>(request);
        //        TellerEntity.IsPrimary = request.IsPrimary && await IsPrimaryTellerAvailable(request.BankId, request.BranchId);

        //        // Validate MinAmount and MaxAmount
        //        ValidateTellerRequest(request);

        //        // Convert UTC to local time and set it in the entity
        //        TellerEntity.CreatedDate = BaseUtilities.UtcToDoualaTime(DateTime.UtcNow);
        //        TellerEntity.Id = BaseUtilities.GenerateUniqueNumber();

        //        // Add the new Teller entity to the repository
        //        _TellerRepository.Add(TellerEntity);

        //        // Create Teller Account
        //        var tellerAccount = await CreateTellerAccount(TellerEntity);

        //        await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, "Teller created successfully", LogLevelInfo.Information.ToString(), 200, _userInfoToken.Token);

        //        // Map the Teller entity to TellerDto and return it with a success response
        //        var TellerDto = _mapper.Map<TellerDto>(TellerEntity);

        //        return ServiceResponse<TellerDto>.ReturnResultWith200(TellerDto, $"{request.Name} was created successfully");

        //    }
        //    catch (Exception e)
        //    {
        //        // Log error and return 500 Internal Server Error response with error message
        //        var errorMessage = $"Error occurred while saving Teller: {e.Message}";
        //        LogAndAuditError(errorMessage, request, 500, e);
        //        return ServiceResponse<TellerDto>.Return500(e, errorMessage);
        //    }
        //}

        private async Task<bool> IsPrimaryTellerAvailable(string bankId, string branchId)
        {
            try
            {
                var primaryTeller = await _TellerRepository.FindBy(c => c.IsPrimary && c.BankId == bankId && c.BranchId == branchId)
                                                            .FirstOrDefaultAsync();

                return primaryTeller == null;
            }
            catch (Exception e)
            {
                LogAndAuditError($"Error occurred while checking primary teller availability: {e.Message}", null, 500, e);
                throw; // Rethrow the exception
            }
        }

        private bool IsValidAmountRange(decimal minAmount, decimal maxAmount)
        {
            if (minAmount < 0 || minAmount > maxAmount)
            {
                var errorMessage = "MinAlertBalance cannot be greater than MaxAlertBalance and less than 0";
                LogAndAuditError(errorMessage, null, 409);
                throw new InvalidOperationException(errorMessage);
            }

            return true;
        }

        private async Task<Account> CreateTellerAccount(Teller tellerEntity)
        {
            try
            {
                //find a saving product by its name property
                var savingProduct = await _SavingProductRepository.FindBy(c => c.IsUsedForTellerProvisioning).FirstOrDefaultAsync();

                if (savingProduct == null)
                {
                    var errorMessage = $"Sorry you need to create {DefaultProducts.DAILYOPERATIONS.ToString()} product to manage teller's accounts";
                    LogAndAuditError(errorMessage, tellerEntity, 409);
                    throw new InvalidOperationException(errorMessage);
                }
                if (tellerEntity.MaximumAmountToManage > savingProduct.MaxAmount)
                {
                    var errorMessage = $"Sorry, the maximum amount you can manage is {savingProduct.MaxAmount}";
                    LogAndAuditError(errorMessage, tellerEntity, 409);
                    throw new InvalidOperationException(errorMessage);
                }
                var tellerAccount = new Account
                {
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    AccountNumber = BaseUtilities.GenerateUniqueNumber(),
                    CreatedDate = tellerEntity.CreatedDate,
                    TellerId = tellerEntity.Id,
                    ProductId = savingProduct.Id,
                    BranchId = tellerEntity.BranchId,
                    BankId = tellerEntity.BankId,
                    AccountType = AccountType.Teller.ToString(),
                    IsTellerAccount = true,
                    Status = AccountStatus.Open.ToString(),
                    AccountName = tellerEntity.Name,
                    CustomerId = savingProduct.Id, CustomerName= tellerEntity.Name, BranchCode=_userInfoToken.BranchCode, 
                };

                _AccountRepository.Add(tellerAccount);
                await _uow.SaveAsync();
                return tellerAccount;
            }
            catch (Exception e)
            {
                var errorMessage = $"Error occurred while creating Teller account: {e.Message}";
                LogAndAuditError(errorMessage, tellerEntity, 500, e);
                throw new InvalidOperationException(errorMessage);
            }
        }

        private void LogAndAuditError(string errorMessage, object request, int statusCode, Exception exception = null)
        {
            _logger.LogError(errorMessage);
            APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), request, errorMessage, LogLevelInfo.Information.ToString(), statusCode, _userInfoToken.Token);
        }

        private bool ValidateTellerRequest(AddTellerCommand request)
        {
            if (request.MinimumAmountToManage >= request.MaximumAmountToManage || request.MinimumAmountToManage < 0)
            {
                throw new ArgumentException("The minimum amount to manage must be less than the maximum amount and cannot be negative.");
            }
            if (request.MinimumDepositAmount >= request.MaximumDepositAmount || request.MinimumDepositAmount < 0)
            {
                throw new ArgumentException("The minimum deposit amount must be less than the maximum deposit amount and cannot be negative.");
            }
            if (request.MinimumWithdrawalAmount >= request.MaximumWithdrawalAmount || request.MinimumWithdrawalAmount < 0)
            {
                throw new ArgumentException("The minimum withdrawal amount must be less than the maximum withdrawal amount and cannot be negative.");
            }
            if (request.MinimumTransferAmount >= request.MaximumTransferAmount || request.MinimumTransferAmount < 0)
            {
                throw new ArgumentException("The minimum transfer amount must be less than the maximum transfer amount and cannot be negative.");
            }

            // Return true if all validations pass
            return true;
        }




    }

}
