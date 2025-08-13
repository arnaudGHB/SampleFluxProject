using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Command;
using CBS.TransactionManagement.Data;
using Microsoft.EntityFrameworkCore;
using CBS.TransactionManagement.MediatR.Accounting.Queries;
using AutoMapper.Internal;

namespace CBS.TransactionManagement.Handlers
{

    /// <summary>
    /// Handles the command to update a   based on UpdateSavingProductCommand.
    /// </summary>
    public class UpdateSavingProductCommandHandler : IRequestHandler<UpdateSavingProductCommand, ServiceResponse<SavingProductDto>>
    {
        private readonly ISavingProductRepository _SavingProductRepository; // Repository for accessing SavingProduct data.
        private readonly ILogger<UpdateSavingProductCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<TransactionContext> _uow;
        public IMediator _mediator { get; set; }
        private readonly IAccountRepository _accountRepository; // Repository for accessing SavingProduct data.

        /// <summary>
        /// Constructor for initializing the UpdateSavingProductCommandHandler.
        /// </summary>
        /// <param name="SavingProductRepository">Repository for SavingProduct data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateSavingProductCommandHandler(
            ISavingProductRepository SavingProductRepository,
            ILogger<UpdateSavingProductCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<TransactionContext> uow = null,
            IMediator mediator = null,
            IAccountRepository accountRepository = null)
        {
            _SavingProductRepository = SavingProductRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _mediator = mediator;
            _accountRepository = accountRepository;
        }
        /// <summary>
        /// Handles the update of a SavingProduct entity and optionally maps the product with the accounting service.
        /// Optimized to update only the account number when required.
        /// </summary>
        public async Task<ServiceResponse<SavingProductDto>> Handle(UpdateSavingProductCommand request, CancellationToken cancellationToken)
        {
            string logReference = BaseUtilities.GenerateUniqueNumber();  // Generate unique log reference for tracking
            string logDetails = string.Empty;  // Log details for success or failure

            try
            {
                // 1. Retrieve the SavingProduct entity to be updated from the repository
                var existingSavingProduct = await _SavingProductRepository.FindAsync(request.Id);

                // 2. Check if the SavingProduct entity exists
                if (existingSavingProduct == null)
                {
                    string errorMessage = $"SavingProduct with ID '{request.Id}' was not found.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.NotFound, LogAction.UpdateSavingProduct, LogLevelInfo.Error, logReference);
                    return ServiceResponse<SavingProductDto>.Return404(errorMessage);
                }

                // 3. Map updated values from the request to the existing entity
                _mapper.Map(request, existingSavingProduct);

                // 4. If the request specifies updating the account chart, handle that logic
                if (request.UpdateOption == "assign_account_chart")
                {
                    // Prepare API request for mapping with the accounting service
                    var apiRequest = CreateApiRequest(existingSavingProduct);
                    var result = await _mediator.Send(apiRequest);

                    string accountFailed = string.Empty;
                    var accountResponse = result.Data;

                    // 5. Handle any accounts that failed during mapping
                    if (accountResponse.NotUpdateds.Any())
                    {
                        accountFailed = SerializeNotUpdatedsToString(accountResponse.NotUpdateds);
                    }

                    // 6. Update SavingProduct entity with accounting service details
                    existingSavingProduct.AccountNuber = accountResponse.AccountNumber;
                    existingSavingProduct.AccountManagementPositionId = accountResponse.ChartOfAccountManagementPositionId;

                    // 7. Update and save the entity
                    _SavingProductRepository.Update(existingSavingProduct);
                    await _uow.SaveAsync();

                    // 8. Log and audit success message with account details
                    string successMessage = $"SavingProduct '{existingSavingProduct.Name}' was updated successfully. " +
                                            $"Account mapping successful with Account Number '{accountResponse.AccountNumber}'. " +
                                            (string.IsNullOrEmpty(accountFailed) ? "No issues during mapping." : $"However, failed for: {accountFailed}");

                    _logger.LogInformation(successMessage);
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.UpdateSavingProduct, LogLevelInfo.Information, logReference);

                    return ServiceResponse<SavingProductDto>.ReturnResultWith200(_mapper.Map<SavingProductDto>(existingSavingProduct), successMessage);
                }
                else
                {
                    // 9. If no account mapping, update the SavingProduct entity and save changes
                    _SavingProductRepository.Update(existingSavingProduct);
                    await _uow.SaveAsync();

                    // 10. Log and audit success message for simple updates
                    string successMessage = $"SavingProduct '{existingSavingProduct.Name}' was updated successfully without account mapping.";
                    _logger.LogInformation(successMessage);
                    await BaseUtilities.LogAndAuditAsync(successMessage, request, HttpStatusCodeEnum.OK, LogAction.UpdateSavingProduct, LogLevelInfo.Information, logReference);

                    return ServiceResponse<SavingProductDto>.ReturnResultWith200(_mapper.Map<SavingProductDto>(existingSavingProduct), successMessage);
                }
            }
            catch (Exception e)
            {
                // 11. Handle exceptions and log error details
                string errorMessage = $"Error occurred while updating SavingProduct with ID '{request.Id}': {e.Message}";
                _logger.LogError(errorMessage);
                await BaseUtilities.LogAndAuditAsync(errorMessage, request, HttpStatusCodeEnum.InternalServerError, LogAction.UpdateSavingProduct, LogLevelInfo.Error, logReference);
                return ServiceResponse<SavingProductDto>.Return500(e);
            }
        }
        // Method to serialize NotUpdateds to a string
        public string SerializeNotUpdatedsToString(List<ProductAccountNotUpdated> NotUpdateds)
        {
            if (NotUpdateds == null || !NotUpdateds.Any())
            {
                return "Empty";
            }

            return string.Join(", ", NotUpdateds.Select(n => $"AccountId: {n.AccountId}, AccountName: {n.AccountName}"));
        }
        /// <summary>
        /// Creates an API request for accounting account type PI call, filtering out null values.
        /// </summary>
        private AddAccountingAccountTypePICallCommand CreateApiRequest(SavingProduct savingProductEntity)
        {
            var details = new List<ProductAccountBookDetail>
    {
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdInterestAccount, Name = OperationEventRubbriqueName.Saving_Interest_Account.ToString() },
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdPricipalAccount, Name = OperationEventRubbriqueName.Principal_Saving_Account.ToString() },
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdInterestExpenseAccount, Name = OperationEventRubbriqueName.Saving_Interest_Expense_Account.ToString() },
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdSavingFee , Name = OperationEventRubbriqueName.Saving_Fee_Account.ToString() },
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdLiassonAccount , Name = OperationEventRubbriqueName.Liasson_Account.ToString() },
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdCashInCommission , Name = OperationEventRubbriqueName.CashIn_Commission_Account.ToString() },
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdTransferFee , Name = OperationEventRubbriqueName.Transfer_Fee_Account.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdCashOutCommission , Name = OperationEventRubbriqueName.CashOut_Commission_Account.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdHeadOfficeShareCashInCommission , Name = OperationEventRubbriqueName.HeadOfficeShareCashInCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdHeadOfficeShareCashOutCommission , Name = OperationEventRubbriqueName.HeadOfficeShareCashOutCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdCamCCULShareCashInCommission , Name = OperationEventRubbriqueName.CamCCULShareCashInCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdCamCCULShareCashOutCommission , Name = OperationEventRubbriqueName.CamCCULShareCashOutCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdFluxAndPTMShareCashInCommission , Name = OperationEventRubbriqueName.FluxAndPTMShareCashInCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdFluxAndPTMShareCashOutCommission , Name = OperationEventRubbriqueName.FluxAndPTMShareCashOutCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdCamCCULShareTransferCommission , Name = OperationEventRubbriqueName.CamCCULShareTransferCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdHeadOfficeShareTransferCommission , Name = OperationEventRubbriqueName.HeadOfficeShareTransferCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdFluxAndPTMShareTransferCommission , Name = OperationEventRubbriqueName.FluxAndPTMShareTransferCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdCamCCULShareCMoneyTransferCommission , Name = OperationEventRubbriqueName.CamCCULShareCMoneyTransferCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdDestinationCMoneyTransferCommission , Name = OperationEventRubbriqueName.ChartOfAccountIdDestinationCMoneyTransferCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdFluxAndPTMShareCMoneyTransferCommission , Name = OperationEventRubbriqueName.FluxAndPTMShareCMoneyTransferCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdHeadOfficeShareCMoneyTransferCommission , Name = OperationEventRubbriqueName.HeadOfficeShareCMoneyTransferCommission.ToString()},
        new ProductAccountBookDetail { ChartOfAccountId = savingProductEntity.ChartOfAccountIdSourceCMoneyTransferCommission , Name = OperationEventRubbriqueName.SourceCMoneyTransferCommission.ToString()},
    };

            // Filter out details with null ChartOfAccountId
            details = details.Where(detail => detail.ChartOfAccountId != null).ToList();

            // Create the API request
            var request = new AddAccountingAccountTypePICallCommand
            {
                ProductAccountBookDetails = details,
                ProductAccountBookId = savingProductEntity.Id,
                ProductAccountBookName = savingProductEntity.Name,
                ProductAccountBookType = OperationAccountType.Saving_Product.ToString()
            };
            return request;
        }
    }

}
