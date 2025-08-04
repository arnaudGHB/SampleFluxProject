using AutoMapper;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Data;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Command;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Validations;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.Repository.LoanProductP;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CBS.NLoan.MediatR.LoanProductMediaR.Handlers
{

    /// <summary>
    /// Handles the command to update a Loan based on UpdateLoanCommand.
    /// </summary>
    public class UpdateLoanProductCommandHandler : IRequestHandler<UpdateLoanProductCommand, ServiceResponse<LoanProductDto>>
    {
        private readonly ILoanProductRepository _LoanProductRepository; // Repository for accessing LoanProduct data.
        private readonly ILogger<UpdateLoanProductCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly IMapper _mapper;  // AutoMapper for object mapping.
        private readonly IUnitOfWork<LoanContext> _uow;
        private readonly IMediator _mediator; // Repository for accessing LoanProduct data.

        /// <summary>
        /// Constructor for initializing the UpdateLoanProductCommandHandler.
        /// </summary>
        /// <param name="LoanProductRepository">Repository for LoanProduct data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        public UpdateLoanProductCommandHandler(
            ILoanProductRepository LoanProductRepository,
            ILogger<UpdateLoanProductCommandHandler> logger,
            IMapper mapper,
            IUnitOfWork<LoanContext> uow = null,
            IMediator mediator = null)
        {
            _LoanProductRepository = LoanProductRepository;
            _logger = logger;
            _mapper = mapper;
            _uow = uow;
            _mediator = mediator;
        }

        /// <summary>
        /// Handles the UpdateLoanProductCommand to update a LoanProduct.
        /// </summary>
        /// <param name="request">The UpdateLoanProductCommand containing updated LoanProduct data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>


        public async Task<ServiceResponse<LoanProductDto>> Handle(UpdateLoanProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Retrieve the LoanProduct entity to be updated from the repository
                var existingLoanProduct = await _LoanProductRepository.FindAsync(request.Id);

                // Check if the LoanProduct entity exists
                if (existingLoanProduct != null)
                {


                    if (request.ServiceOption == "repayment")
                    {
                        if (request.RepaymentCycles.Any())
                        {
                            var addLoanProductRepaymentCycle = new AddLoanProductRepaymentCycleCommand { LoanProductId = request.Id, RepaymentCycles = request.RepaymentCycles };
                            var resultCycle = await _mediator.Send(addLoanProductRepaymentCycle, cancellationToken);

                            if (resultCycle.StatusCode != 200)
                            {
                                var errorMessage = $"{resultCycle.Message}";
                                _logger.LogError(errorMessage);
                                return ServiceResponse<LoanProductDto>.Return409(errorMessage);
                            }
                        }
                       /* var repaymentOrderCommand = new AddLoanProductRepaymentOrderCommand { CapitalOrder = request.CapitalOrder, CapitalRate = request.CapitalRate, FineOrder = request.FineOrder, FineRate = request.FineRate, InterestOrder = request.InterestOrder, InterestRate = request.InterestRate, LoanDeliquencyPeriod = LoanDeliquentStatus.NormalLoans.ToString(), RepaymentTypeName = LoanDeliquentStatus.NormalLoans.ToString()};
                        var resultOrder = await _mediator.Send(repaymentOrderCommand, cancellationToken);

                        if (resultOrder.StatusCode != 200)
                        {
                            var errorMessage = $"{resultOrder.Message}";
                            _logger.LogError(errorMessage);
                            return ServiceResponse<LoanProductDto>.Return409(errorMessage);

                        }*/

                    }
                    // Map properties from existingLoanProduct to request

                    _mapper.Map(request, existingLoanProduct);

                    string message = $"Product updated successfully.";

                    if (request.UpdateOption == "assign_account_chart")
                    {
                        var apiRequest = CreateApiRequest(existingLoanProduct);
                        var result = await _mediator.Send(apiRequest);

                        if (result.StatusCode != 200)
                        {

                            message = $"{message}, Error occurred while mapping product with the accounting service: {result.Message}";
                            return ServiceResponse<LoanProductDto>.Return409(message);

                        }
                        else
                        {
                            existingLoanProduct.AccountNumber = result.Data.AccountNumber;
                            existingLoanProduct.ChartOfAccountManagementPositionId = result.Data.ChartOfAccountManagementPositionId;
                            message = $"{request.ProductName} on the chart of account was successfully updated with account number {existingLoanProduct.AccountNumber} & Positionning id {existingLoanProduct.ChartOfAccountManagementPositionId}";
                        }
                    }

                    _LoanProductRepository.Update(existingLoanProduct);

                    await _uow.SaveAsync();

                    // Prepare the response and return a successful response with 200 status code
                    var response = ServiceResponse<LoanProductDto>.ReturnResultWith200(_mapper.Map<LoanProductDto>(existingLoanProduct), $"{message}");
                    _logger.LogInformation($"{message}");
                    return response;
                }
                else
                {
                    // If the LoanProduct entity was not found, return 404 Not Found response with an error message
                    string errorMessage = $"{request.ProductCode} was not found to be updated.";
                    _logger.LogError(errorMessage);
                    return ServiceResponse<LoanProductDto>.Return404(errorMessage);
                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with an error message
                string errorMessage = $"Error occurred while updating LoanProduct: {e.Message}";
                _logger.LogError(errorMessage);
                return ServiceResponse<LoanProductDto>.Return500(e);
            }
        }

        private AddAccountingAccountTypePICallCommand CreateApiRequest(LoanProduct loanProduct)
        {
            var details = new List<ProductAccountBookDetail>
            {
                    new ProductAccountBookDetail { ChartOfAccountId = loanProduct.ChartOfAccountIdForPrincipalAmount, Name = OperationEventRubbriqueName.Loan_Principal_Account.ToString() },
                    new ProductAccountBookDetail { ChartOfAccountId = loanProduct.ChartOfAccountIdForInterestReceived, Name = OperationEventRubbriqueName.Loan_Interest_Recieved_Account.ToString() },
                    new ProductAccountBookDetail { ChartOfAccountId = loanProduct.ChartOfAccountIdForPenalty, Name = OperationEventRubbriqueName.Loan_Penalty_Account.ToString() },
                    new ProductAccountBookDetail { ChartOfAccountId = loanProduct.ChartOfAccountIdForProvisionMoreThanOneYear, Name = OperationEventRubbriqueName.Loan_Provisioning_Account_MoreThanOneYear.ToString() },
                    new ProductAccountBookDetail { ChartOfAccountId = loanProduct.ChartOfAccountIdForTax, Name = OperationEventRubbriqueName.Loan_VAT_Account.ToString() },
                    new ProductAccountBookDetail { ChartOfAccountId = loanProduct.ChartOfAccountIdForWriteOffPrincipal , Name = OperationEventRubbriqueName.Loan_WriteOff_Account.ToString() },
                    new ProductAccountBookDetail { ChartOfAccountId = loanProduct.ChartOfAccountIdForProvisionMoreThanTwoYear , Name = OperationEventRubbriqueName.Loan_Provisioning_Account_MoreThanTwoYear.ToString() },
                    new ProductAccountBookDetail { ChartOfAccountId = loanProduct.ChartOfAccountIdForProvisionMoreThanThreeYear , Name = OperationEventRubbriqueName.Loan_Provisioning_Account_MoreThanThreeYear.ToString() },
                    new ProductAccountBookDetail { ChartOfAccountId = loanProduct.ChartOfAccountIdForProvisionMoreThanFourYear , Name = OperationEventRubbriqueName.Loan_Provisioning_Account_MoreThanFourYear.ToString() },
                    new ProductAccountBookDetail { ChartOfAccountId = loanProduct.ChartOfAccountIdForLoanTransition , Name = OperationEventRubbriqueName.Loan_Transit_Account.ToString() },
            };
            var request = new AddAccountingAccountTypePICallCommand
            {
                ProductAccountBookDetails = details,
                ProductAccountBookId = loanProduct.Id,
                ProductAccountBookName = loanProduct.ProductName,
                ProductAccountBookType = OperationEventRubbriqueName.Loan_Product.ToString()
            };
            return request;
        }
    }

}
