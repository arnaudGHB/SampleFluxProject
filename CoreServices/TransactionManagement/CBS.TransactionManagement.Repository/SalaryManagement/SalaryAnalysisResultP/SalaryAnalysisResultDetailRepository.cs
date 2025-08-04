using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.LoanRepayment;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Domain;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.Helper;
using CBS.TransactionManagement.Repository.FileUploadP;
using CBS.TransactionManagement.Repository.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Repository.SalaryManagement.StandingOrderP;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CBS.TransactionManagement.Repository.SalaryManagement.SalaryAnalysisResultP
{

    public class SalaryAnalysisResultDetailRepository : GenericRepository<SalaryAnalysisResultDetail, TransactionContext>, ISalaryAnalysisResultDetailRepository
    {
        private readonly IUnitOfWork<TransactionContext> _uow;
        private readonly IStandingOrderRepository _standingOrderRepository;
        private readonly IFileUploadRepository _fileUploadRepository;
        private readonly ISalaryAnalysisResultRepository _salaryAnalysisResultRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly ISavingProductRepository _savingProductRepository;
        private readonly UserInfoToken _userInfoToken;
        private readonly ILogger<SalaryAnalysisResultDetailRepository> _logger;
        public SalaryAnalysisResultDetailRepository(
            IUnitOfWork<TransactionContext> unitOfWork,
            ILogger<SalaryAnalysisResultDetailRepository> logger,
            UserInfoToken userInfoToken,
            IStandingOrderRepository standingOrderRepository,
            IFileUploadRepository fileUploadRepository,
            ISalaryAnalysisResultRepository salaryAnalysisResultRepository,
            IAccountRepository accountRepository,
            ISavingProductRepository savingProductRepository) : base(unitOfWork)
        {
            _uow = unitOfWork;
            _logger = logger;
            _userInfoToken = userInfoToken;
            _standingOrderRepository = standingOrderRepository;
            _fileUploadRepository = fileUploadRepository;
            _salaryAnalysisResultRepository = salaryAnalysisResultRepository;
            _accountRepository=accountRepository;
            _savingProductRepository=savingProductRepository;
        }


        public async Task<SalaryAnalysisResultDto> AnalyzeSalaryFileAsync(
            string fileUploadId,
            LoanRepaymentOrderDto loanRepaymentOrder,
            BranchDto branch,
            List<SalaryUploadModel> salaryUploadModels,
            List<LightLoanDto> loans,
            List<CustomerDto> members)
        {
            // Fetch the uploaded file details
            var fileUploaded = await _fileUploadRepository.FindAsync(fileUploadId);
            var savingProduct = await _savingProductRepository.FindBy(x => x.AccountType==AccountType.Salary.ToString()).FirstOrDefaultAsync();
            try
            {
                if (savingProduct == null)
                {
                    string errorMessage = "Salary product not found.";
                    _logger.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Validate the uploaded file
                if (fileUploaded == null)
                {
                    string errorMessage = $"File with ID {fileUploadId} not found.";
                    _logger.LogError(errorMessage);
                    throw new InvalidOperationException(errorMessage);
                }

                // Ensure no duplicate analysis exists for the file in the branch
                var existingResults = await _salaryAnalysisResultRepository
                    .FindBy(x => x.FileUploadId == fileUploadId && x.BranchId == branch.id)
                    .ToListAsync();

                if (existingResults.Any())
                {
                    _salaryAnalysisResultRepository.RemoveRange(existingResults);
                    await _uow.SaveAsync();
                    string errorMessage = $"Salary analysis for file {fileUploaded.FileName} in branch: {branch.name} has already been performed. Deleted, then execution continue.";
                    _logger.LogError(errorMessage);
                    await BaseUtilities.LogAndAuditAsync(errorMessage, fileUploaded, HttpStatusCodeEnum.OK, LogAction.SalaryAnalysis, LogLevelInfo.Information);

                }

                // Fetch active standing orders for members
                var memberIds = members.Select(x => x.CustomerId).ToList();
                var standingOrders = await _standingOrderRepository
                    .All
                    .Where(so => memberIds.Contains(so.MemberId) && so.IsActive)
                    .OrderBy(so => so.Priority)
                    .ToListAsync();

                // Initialize the salary analysis result object
                var salaryAnalysisResult = new SalaryAnalysisResult
                {
                    Id = BaseUtilities.GenerateUniqueNumber(),
                    FileUploadId = fileUploadId,
                    TotalMembers = salaryUploadModels.Count.ToString(),
                    TotalBranches = 1,
                    BranchCode = branch.branchCode,
                    BranchId = branch.id,
                    BranchName = branch.name,
                    Date = BaseUtilities.UtcNowToDoualaTime(),
                    ExecutedBy = _userInfoToken.FullName,
                    TotalNetSalary = 0,
                    TotalLoanCapital = 0,
                    TotalLoanInterest = 0,
                    TotalVAT = 0,
                    TotalLoanRepayment = 0,
                    TotalDeposit = 0,
                    TotalSavings = 0,
                    TotalCharges = 0,
                    TotalShares = 0,
                    TotalPreferenceShares = 0,
                    TotalRemainingSalary = 0,
                    salaryAnalysisResultDetails = new List<SalaryAnalysisResultDetail>()
                };

                // Process each salary record in the file
                foreach (var salaryUploadModel in salaryUploadModels)
                {
                    var member = members.FirstOrDefault(x => x.Matricule == salaryUploadModel.Matricule);
                    decimal remainingSalary = salaryUploadModel.NetSalary;

                    // Initialize allocation variables
                    decimal loanCapital = 0, loanInterest = 0, vat = 0, penalty = 0, totalLoanRepayment = 0;
                    decimal deposit = 0, savings = 0, shares = 0, preferenceShares = 0, charges = 0;
                    string loanid = "n/a"; string loanType = "n/a"; decimal standingOrderAmount = 0; string standingOrderStatement = "n/a"; string loanProductId = "n/a"; string loanProductName = "n/a";
                    bool isOldLoan=false;
                    if (member != null)
                    {
                        // Process loan standing orders
                        var loanOrders = standingOrders
                            .Where(so => so.MemberId == member.CustomerId && so.DestinationAccountType == "Loan")
                            .OrderBy(so => so.Priority)
                            .ToList();
                        charges = await CalculateCharges(salaryUploadModel.NetSalary, savingProduct.Id, FeeOperationType.CivilServants, false);
                        remainingSalary-=charges;

                        foreach (var loanOrder in loanOrders)
                        {
                            if (remainingSalary <= 0) break;

                            decimal allocation = Math.Min(loanOrder.Amount, remainingSalary);
                            standingOrderAmount=loanOrder.Amount;
                            standingOrderStatement=loanOrder.Purpose;
                            // Find the associated loan for the member
                            var activeLoan = loans.FirstOrDefault(l => l.CustomerId == member.CustomerId);
                            if (activeLoan != null)
                            {
                                if (salaryUploadModel.Matricule=="0684686O")
                                {

                                }
                                loanid=activeLoan.Id;
                                loanType=activeLoan.LoanCategory;
                                loanProductId=activeLoan.LoanApplication.LoanProduct.Id;
                                loanProductName=$"[{activeLoan.LoanApplication.LoanProduct.ProductCode}] [{activeLoan.LoanApplication.LoanProduct.ProductName}] -[{activeLoan.LoanApplication.LoanProduct.TargetType}]";
                                isOldLoan=activeLoan.IsUpload;
                                // Calculate interest and penalty
                                loanInterest = activeLoan.Balance * activeLoan.InterestRate / 100;
                                if (activeLoan.AccrualInterest<loanInterest)
                                {
                                    loanInterest=activeLoan.AccrualInterest;
                                }
                    
                                // Apply VAT if loan balance exceeds threshold
                                vat = loanInterest * 0.1925m; // 19.25% VAT on interest

                                // Initialize payment tracking variables
                                var remainingAmount = allocation;
                                var interestPaid = 0m;
                                var taxPaid = 0m;
                                var principalPaid = 0m;


                                // Process repayment based on repayment order
                                foreach (var order in GetRepaymentOrder(loanRepaymentOrder))
                                {
                                    switch (order)
                                    {
                                        case "interest":
                                            (interestPaid, taxPaid, remainingAmount) = ProcessInterestRepayment(activeLoan, loanRepaymentOrder, loanInterest, remainingAmount);
                                            vat=taxPaid;
                                            loanInterest=interestPaid;
                                            break;
                                        case "principal":
                                            principalPaid = ProcessPrincipalRepayment(activeLoan, activeLoan.Balance, remainingAmount);
                                            remainingAmount -= principalPaid;
                                            break;

                                    }

                                    if (remainingAmount <= 0) break;
                                }
                                if (remainingAmount > 0)
                                {
                                    savings += remainingAmount;
                                }

                                // Add loan-related deductions
                                loanCapital += principalPaid;
                                totalLoanRepayment += principalPaid + interestPaid + taxPaid + penalty;

                                remainingSalary -= allocation;
                                remainingSalary += remainingAmount;
                                //remainingSalary -= vat;
                                remainingSalary -= savings;
                            }

                            else
                            {
                                if (allocation > 0)
                                {
                                    savings += allocation;
                                    remainingSalary-=allocation;
                                    allocation=0;

                                }
                            }
                        }

                        // Process non-loan standing orders
                        var otherOrders = standingOrders
                            .Where(so => so.MemberId == member.CustomerId && so.DestinationAccountType != "Loan")
                            .OrderBy(so => so.Priority)
                            .ToList();

                        foreach (var order in otherOrders)
                        {
                            standingOrderAmount=order.Amount;
                            standingOrderStatement=order.Purpose;
                            if (remainingSalary <= 0) break;

                            decimal allocation = Math.Min(order.Amount, remainingSalary);

                            switch (order.DestinationAccountType)
                            {
                                case "Deposit":
                                    deposit += allocation;
                                    break;
                                case "Savings":
                                    savings += (allocation);
                                    break;
                                case "OrdinaryShares":
                                    shares += allocation;
                                    break;
                                case "PreferenceShares":
                                    preferenceShares += allocation;
                                    break;
                                default:
                                    throw new InvalidOperationException($"Unknown DestinationAccountType: {order.DestinationAccountType}");
                            }

                            remainingSalary -= allocation;
                        }

                    }

                    // Add detail to the salary analysis result
                    salaryAnalysisResult.salaryAnalysisResultDetails.Add(new SalaryAnalysisResultDetail
                    {
                        Matricule = salaryUploadModel.Matricule,
                        MemberName = $"{salaryUploadModel.Surname} {salaryUploadModel.Name}",
                        NetSalary = salaryUploadModel.NetSalary,
                        LoanCapital = loanCapital,
                        LoanInterest = loanInterest,
                        LoanId=loanid,
                        LoanType=loanType,
                        LoanProductId=loanProductId,
                        LoanProductName=loanProductName, IsOnldLoan=isOldLoan,
                        VAT = vat,
                        TotalLoanRepayment = totalLoanRepayment,
                        StandingOrderAmount=standingOrderAmount,
                        StandingOrderStatement=standingOrderStatement,
                        Deposit = deposit,
                        CustomerId = salaryUploadModel.CustomerId,
                        Savings = savings,
                        Shares = shares,
                        PreferenceShares = preferenceShares,
                        RemainingSalary = remainingSalary,
                        Charges = charges,
                        Status = remainingSalary >= 0 ? "Analyzed" : "Insufficient Salary",
                        BranchId = branch.id,
                        BranchCode = branch.branchCode,
                        BranchName = branch.name,
                        Id = BaseUtilities.GenerateUniqueNumber(),
                        SalaryAnalysisResultId = salaryAnalysisResult.Id
                    });

                    // Update aggregate totals
                    salaryAnalysisResult.TotalNetSalary += salaryUploadModel.NetSalary;
                    salaryAnalysisResult.TotalLoanCapital += loanCapital;
                    salaryAnalysisResult.TotalLoanInterest += loanInterest;
                    salaryAnalysisResult.TotalVAT += vat;
                    salaryAnalysisResult.TotalLoanRepayment += totalLoanRepayment;
                    salaryAnalysisResult.TotalDeposit += deposit;
                    salaryAnalysisResult.TotalSavings += savings;
                    salaryAnalysisResult.TotalShares += shares;
                    salaryAnalysisResult.TotalPreferenceShares += preferenceShares;
                    salaryAnalysisResult.TotalCharges += charges;
                    salaryAnalysisResult.TotalRemainingSalary += remainingSalary;
                }

                // Save result and details to the database
                _salaryAnalysisResultRepository.Add(salaryAnalysisResult);
                await _uow.SaveAsync();

                // Log and audit success
                string successMessage = $"Salary analysis for file {fileUploaded.FileName} completed successfully.";
                await BaseUtilities.LogAndAuditAsync(successMessage, fileUploaded, HttpStatusCodeEnum.OK, LogAction.SalaryAnalysis, LogLevelInfo.Information);
                _logger.LogInformation(successMessage);

                return MapSalaryAnalysisResultToDto(salaryAnalysisResult);
            }
            catch (Exception ex)
            {
                string errorMessage = $"Salary analysis failed for file {fileUploaded?.FileName}. Error: {ex.Message}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage, ex);
            }
        }

        /// <summary>
        /// Determines the repayment order based on priority values defined in the LoanRepaymentOrderDto.
        /// Orders repayment components (interest, principal, charges) from highest to lowest priority.
        /// </summary>
        /// <param name="repaymentOrder">The repayment order DTO containing priority values.</param>
        /// <returns>An ordered list of repayment categories based on their assigned priority.</returns>
        private IEnumerable<string> GetRepaymentOrder(LoanRepaymentOrderDto repaymentOrder)
        {
            // Define a list of repayment components and their assigned priority.
            var orderList = new List<KeyValuePair<string, int>>
    {
        new KeyValuePair<string, int>("interest", repaymentOrder.InterestOrder),
        new KeyValuePair<string, int>("principal", repaymentOrder.CapitalOrder),
        new KeyValuePair<string, int>("charges", repaymentOrder.FineOrder)
    };

            // Order the repayment components by their priority and return only the component names.
            return orderList.OrderBy(x => x.Value).Select(x => x.Key);
        }

        /// <summary>
        /// Processes the repayment of accrued interest on a loan.
        /// Deducts the interest payment and calculates the applicable tax (VAT).
        /// </summary>
        /// <param name="loan">The loan for which the interest repayment is being processed.</param>
        /// <param name="loanRepaymentOrder">The repayment order configuration containing interest rate details.</param>
        /// <param name="interestAmount">The total interest amount intended to be paid.</param>
        /// <param name="standingOrderRemainingAmount">The remaining amount available for repayment.</param>
        /// <returns>
        /// A tuple containing:
        /// - interestPaid: The actual interest amount paid.
        /// - taxPaid: The tax (VAT) paid on the interest.
        /// - standingOrderRemainingAmount: The remaining balance after interest and tax deductions.
        /// </returns>
        private (decimal interestPaid, decimal taxPaid, decimal remainingAmount) ProcessInterestRepayment(
            LightLoanDto loan,
            LoanRepaymentOrderDto loanRepaymentOrder,
            decimal interestAmount,
            decimal standingOrderRemainingAmount)
        {
            decimal interestPaid = 0m;
            decimal taxPaid = 0m;

            // Ensure there is an outstanding accrued interest and available repayment funds
            if (standingOrderRemainingAmount > 0 && loan.AccrualInterest > 0)
            {
                // Determine the amount that can be paid towards interest
                decimal interestToPay = Math.Min(interestAmount, loan.AccrualInterest);

                // Adjust the interest payment based on the interest rate in the repayment order
                if (standingOrderRemainingAmount <= interestToPay)
                {
                    interestToPay = Math.Min(interestAmount, standingOrderRemainingAmount);
                    interestToPay = (interestToPay * loanRepaymentOrder.InterestRate) / 100;
                }


                // Deduct the paid interest from the loan's accrued interest
                loan.AccrualInterest -= interestToPay;
                interestPaid = interestToPay;
                standingOrderRemainingAmount -= interestToPay;

                // Calculate VAT on the paid interest (19.25%)
                decimal vat = BaseUtilities.RoundUpToOne(interestPaid * 0.1925m);
                // Ensure VAT is deducted if applicable
                if (vat > 0 && loan.Tax > 0)
                {
                    decimal taxToPay = Math.Min(vat, loan.Tax);
                    taxToPay=Math.Min(taxToPay, standingOrderRemainingAmount);
                    loan.Tax -= taxToPay;
                    taxPaid = taxToPay;
                    standingOrderRemainingAmount -= taxToPay;
                }
            }

            return (interestPaid, taxPaid, standingOrderRemainingAmount);
        }

        /// <summary>
        /// Processes the repayment of the principal amount on a loan.
        /// Deducts the principal payment from the loan balance while ensuring valid repayment amounts.
        /// </summary>
        /// <param name="loan">The loan for which the principal repayment is being processed.</param>
        /// <param name="principalAmount">The amount intended to be paid towards the principal.</param>
        /// <param name="standingOrderRemainingAmount">The available amount for repayment.</param>
        /// <returns>
        /// The actual principal amount paid.
        /// </returns>
        private decimal ProcessPrincipalRepayment(LightLoanDto loan, decimal principalAmount, decimal standingOrderRemainingAmount)
        {
            // Round down the remaining amount to the nearest 1,000 for valid repayment
            decimal adjustedRemainingAmount = Math.Floor(standingOrderRemainingAmount / 1000) * 1000;

            // Ensure there is a valid remaining amount and outstanding loan balance
            if (adjustedRemainingAmount > 0 && loan.DueAmount > 0)
            {
                // If the rounded remaining amount exceeds the intended principal payment, adjust it
                if (adjustedRemainingAmount <= principalAmount)
                {
                    principalAmount = adjustedRemainingAmount;
                }

                // Determine the actual amount that can be paid toward the principal
                decimal principalToPay = Math.Min(principalAmount, loan.DueAmount);

                // Deduct the paid principal from the loan balance
                loan.DueAmount -= principalToPay;
                loan.Balance -= principalToPay;
                loan.Principal -= principalToPay;

                // Track the total principal paid
                loan.TotalPrincipalPaid += principalToPay;

                return principalToPay; // Return the actual principal amount paid
            }

            return 0m; // Return 0 if no payment was made
        }


        private SalaryAnalysisResultDto MapSalaryAnalysisResultToDto(SalaryAnalysisResult salaryAnalysisResult)
        {
            return new SalaryAnalysisResultDto
            {
                Id = salaryAnalysisResult.Id,
                FileUploadId = salaryAnalysisResult.FileUploadId,
                TotalMembers = salaryAnalysisResult.TotalMembers,
                TotalBranches = salaryAnalysisResult.TotalBranches,
                TotalNetSalary = salaryAnalysisResult.TotalNetSalary,
                TotalLoanCapital = salaryAnalysisResult.TotalLoanCapital,
                TotalLoanInterest = salaryAnalysisResult.TotalLoanInterest,
                TotalVAT = salaryAnalysisResult.TotalVAT,
                TotalLoanRepayment = salaryAnalysisResult.TotalLoanRepayment,
                TotalDeposit = salaryAnalysisResult.TotalDeposit,
                TotalSavings = salaryAnalysisResult.TotalSavings,
                TotalShares = salaryAnalysisResult.TotalShares,
                TotalPreferenceShares = salaryAnalysisResult.TotalPreferenceShares,
                TotalCharges = salaryAnalysisResult.TotalCharges,
                TotalRemainingSalary = salaryAnalysisResult.TotalRemainingSalary,
                salaryAnalysisResultDetails = salaryAnalysisResult.salaryAnalysisResultDetails
                    .Select(detail => new SalaryAnalysisResultDetailDto
                    {
                        Matricule = detail.Matricule,
                        MemberName = detail.MemberName,
                        NetSalary = detail.NetSalary,
                        LoanCapital = detail.LoanCapital,
                        LoanInterest = detail.LoanInterest,
                        CustomerId = detail.CustomerId,
                        VAT = detail.VAT,
                        TotalLoanRepayment = detail.TotalLoanRepayment,
                        Deposit = detail.Deposit,
                        Savings = detail.Savings,
                        Shares = detail.Shares,
                        PreferenceShares = detail.PreferenceShares,
                        RemainingSalary = detail.RemainingSalary,
                        Charges = detail.Charges,
                        Status = detail.Status,
                        BranchId = detail.BranchId,
                        BranchCode = detail.BranchCode,
                        BranchName = detail.BranchName,
                        Id = detail.Id,
                        SalaryAnalysisResultId = detail.SalaryAnalysisResultId
                    }).ToList()
            };
        }





        /// <summary>
        /// Calculates charges for a given standing order based on its type and allocated amount.
        /// </summary>
        /// <param name="order">The standing order for which charges are calculated.</param>
        /// <param name="allocationAmount">The amount allocated to the standing order.</param>
        /// <returns>The calculated charge amount.</returns>
        private async Task<decimal> CalculateCharges(decimal allocationAmount, string productid, FeeOperationType feeOperationType, bool isMinorAccount = false)
        {
            TransferCharges transferCharges = await _accountRepository.CalculateTransferCharges(allocationAmount, productid, feeOperationType, _userInfoToken.BranchID, "Local", false, false, 0, isMinorAccount);
            return transferCharges.ServiceCharge;

        }

    }



}
