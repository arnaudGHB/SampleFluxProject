using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanPurposeP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.Domain.Context;
using CBS.NLoan.Helper.Helper;
using CBS.NLoan.MediatR.Commands;
using CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Queries;
using CBS.NLoan.MediatR.LoanP.Command;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.DisburstLoan;
using CBS.NLoan.Repository.LoanApplicationP.LoanAmortizationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using CBS.NLoan.Repository.LoanProductP;
using CBS.NLoan.Repository.LoanPurposeP;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using NPOI.XSSF.UserModel;

namespace CBS.NLoan.MediatR.LoanP.Handler
{
    public class LoanUploadCommandHandler : IRequestHandler<LoanUploadCommand, ServiceResponse<List<LoanUpload>>>
    {
        private readonly IDisburstedLoanRepository _disburstedLoanRepository;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanProductRepository _loanProductRepository;
        private readonly ILoanPurposeRepository _loanPurposeRepository;
        private readonly ILoanAmortizationRepository _loanAmortizationRepository;
        private readonly ILogger<LoanUploadCommandHandler> _logger; // Logger for logging handler actions and errors.
        private readonly UserInfoToken _userInfoToken;
        private readonly PathHelper _pathHelper;
        private readonly IHostingEnvironment _webHostEnvironment;
        private readonly IUnitOfWork<LoanContext> _uow;
        public IMediator _mediator { get; set; }
        /// <summary>
        /// Constructor for initializing the AddAccountingEventCommandHandler.
        /// </summary>
        /// <param name="AccountingEventRepository">Repository for AccountingEvent data access.</param>
        /// <param name="mapper">AutoMapper for object mapping.</param>
        /// <param name="claimTypeRepository">Repository for ClaimType data access.</param>
        /// <param name="logger">Logger for logging handler actions and errors.</param>
        public LoanUploadCommandHandler(
            UserInfoToken userInfoToken,
            ILogger<LoanUploadCommandHandler> logger,
            PathHelper pathHelper,
            IMediator mediator,
            IHostingEnvironment webHostEnvironment,
            ILoanRepository loanRepository,
            ILoanProductRepository loanProductRepository,
            IDisburstedLoanRepository disburstedLoanRepository,
            ILoanApplicationRepository loanApplicationRepository,
            IUnitOfWork<LoanContext> uow,
            ILoanPurposeRepository loanPurposeRepository,
            ILoanAmortizationRepository loanAmortizationRepository)
        {

            _userInfoToken = userInfoToken;
            _logger = logger;
            _pathHelper = pathHelper;
            _mediator = mediator;
            _webHostEnvironment = webHostEnvironment;
            _loanRepository = loanRepository;
            _loanProductRepository = loanProductRepository;
            _disburstedLoanRepository = disburstedLoanRepository;
            _loanApplicationRepository = loanApplicationRepository;
            _uow = uow;
            _loanPurposeRepository = loanPurposeRepository;
            _loanAmortizationRepository = loanAmortizationRepository;
        }

        /// <summary>
        /// Handles the AddAccountingEventCommand to add a new AccountingEvent.
        /// </summary>
        /// <param name="request">The AddAccountingEventCommand containing AccountingEvent data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <summary>
        /// Handles the AddCustomerCommand to add a new Customer.
        /// </summary>
        /// <param name="request">The AddCustomerCommand containing Customer data.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        public async Task<ServiceResponse<List<LoanUpload>>> Handle(LoanUploadCommand request, CancellationToken cancellationToken)
        {
            try
            {
                List<LoanUpload> loanUploadList = new List<LoanUpload>();
                List<LoanUpload> loanUploadListLeft = new List<LoanUpload>();
                var filePath = await FileData.SaveFileAsync(request.formFile, _pathHelper.UploadPath, _webHostEnvironment);
                // Get the file extension
                string fileExtension = Path.GetExtension(filePath)?.ToLower();
                // Check if the file extension is either XLSX or XLS
                if (fileExtension == ".xlsx" || fileExtension == ".xls")
                {
                    //677550429
                    XSSFWorkbook xssfwb;
                    using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        xssfwb = new XSSFWorkbook(file);
                    }

                    var sheet = xssfwb.GetSheetAt(0); // Change this to the worksheet you want to import.
                    var rows = sheet.GetRowEnumerator();
                    rows.MoveNext();
                    while (rows.MoveNext())
                    {
                        var row = (XSSFRow)rows.Current;

                        var data = new LoanUpload()
                        {
                            Folder = row.GetCell(0)?.ToString(),
                            MemberNumber = row.GetCell(1)?.ToString(),
                            MemberName = row.GetCell(2)?.ToString(),
                            SubmitDate = DateTime.Parse(row.GetCell(3)?.ToString()),
                            ValidationDate = DateTime.Parse(row.GetCell(4)?.ToString()),
                            LoanProduct = row.GetCell(5)?.ToString(),
                            Label = row.GetCell(27)?.ToString(),
                            LoanAmount = Converter.toDecimal(row.GetCell(7)?.ToString()),
                            LoanBalance = Converter.toDecimal(row.GetCell(8)?.ToString()),
                            DelayInterest = Converter.toDecimal(row.GetCell(9)?.ToString()),
                            Reason = row.GetCell(10)?.ToString(),
                            LoanAccount = row.GetCell(11)?.ToString(),
                            Periodic = row.GetCell(12)?.ToString(),
                            NBRECHE = Converter.toInt(row.GetCell(13)?.ToString()),
                            CalculationType = row.GetCell(14)?.ToString(),
                            IntRate = Converter.toDecimal(row.GetCell(15)?.ToString()),
                            PremierEcheance = row.GetCell(16)?.ToString(),
                            BranchCode = _userInfoToken.BranchCode,
                            LastRepayment = DateTime.Parse(row.GetCell(17)?.ToString()),
                            EndDate = row.GetCell(18)?.ToString(),
                            ANNEE = row.GetCell(19)?.ToString(),
                            Amount = Converter.toDecimal(row.GetCell(20)?.ToString()),
                            Balance = Converter.toDecimal(row.GetCell(21)?.ToString()),
                            Interest = Converter.toDecimal(row.GetCell(22)?.ToString()),
                            DeliquentDays = Converter.toInt(row.GetCell(23)?.ToString()),
                            DeliquentStatus = row.GetCell(27)?.ToString(),
                            AdvanceDays = Converter.toInt(row.GetCell(24)?.ToString()),
                            DeliquentAmount = Converter.toDecimal(row.GetCell(25)?.ToString()),
                            AdvanceAmount = Converter.toDecimal(row.GetCell(26)?.ToString()),
                            LoanType = row.GetCell(28)?.ToString(),
                            Description = ""
                        };
                        loanUploadList.Add(data);
                    }

                    // Check if a Loanpurse with the same name already exists (case-insensitive)
                    LoanPurpose loanPurpose = null;

                    //LoanPurpose loanPurpose = await _loanPurposeRepository.AllIncluding().Where(c => c.PurposeName == "UploadLoan").FirstOrDefaultAsync();

                    // If a LoanProduct with the same name already exists, return a conflict response
                    //if (loanPurpose == null)
                    //{
                    //    var errorMessage = $"loanPurpose UploadLoan does not exists.";
                    //    _logger.LogError(errorMessage);
                    //    return ServiceResponse<List<LoanUpload>>.Return404(errorMessage);
                    //}

                    LoanProduct loanProduct = null;

                    //if (loanUploadList.Count > 0)
                    //{
                    //    var upLoan = loanUploadList[0];

                    //}
                    //add data

                    // Call the bank for economic activitie API
                    var GetEconomicActivitiesResponse = await APICallHelper.GetEconomicActivities<ServiceResponse<List<EconomicActivityDto>>>(_pathHelper, _userInfoToken.Token);

                    List<EconomicActivityDto> economicActivities = null;
                    List<AddLoanAmortizationCommand> loanAmortizationCommands = new List<AddLoanAmortizationCommand>();

                    if (GetEconomicActivitiesResponse != null && GetEconomicActivitiesResponse.StatusCode == 200)
                    {
                        economicActivities = GetEconomicActivitiesResponse.Data;
                    }
                    else
                    {
                        var errorMessage = $"Failed getting Economic Activities .";
                        _logger.LogError(errorMessage);
                        return ServiceResponse<List<LoanUpload>>.Return404(errorMessage);
                    }

                    foreach (var upLoan in loanUploadList)
                    {
                        string id = "";
                        id = upLoan.BranchCode.Substring(upLoan.BranchCode.Length - 2) + "" + upLoan.Folder;

                        // Check if a Loanpurse with the same name already exists (case-insensitive)
                        var existLoanApplication = await _loanApplicationRepository.AllIncluding().Where(c => c.Id == id).FirstOrDefaultAsync();

                        if (existLoanApplication != null)
                        {
                            var errorMessage = $"LoanApplication id {id} already exists.";
                            _logger.LogError(errorMessage);
                            upLoan.Description = errorMessage;
                            loanUploadListLeft.Add(upLoan);
                            continue;
                            //return ServiceResponse<List<LoanUpload>>.Return404(errorMessage);
                        }

                        var existLoan = await _loanRepository.AllIncluding().Where(c => c.Id == id).FirstOrDefaultAsync();

                        if (existLoan != null)
                        {
                            var errorMessage = $"Loan id {id} already exists.";
                            _logger.LogError(errorMessage);
                            upLoan.Description = errorMessage;
                            loanUploadListLeft.Add(upLoan);
                            continue;
                        }

                        EconomicActivityDto economicActivityDto = extractEconomicActivityByName(upLoan.Reason, economicActivities);

                        if (economicActivityDto == null)
                        {
                            var errorMessage = $"economic activity {upLoan.Reason} does not exists.";
                            _logger.LogError(errorMessage);
                            upLoan.Description = errorMessage;
                            loanUploadListLeft.Add(upLoan);
                            continue;
                        }

                        // Check if a Loanpurse with the same name already exists (case-insensitive)
                        loanPurpose = await _loanPurposeRepository.AllIncluding().Where(c => c.PurposeName == upLoan.Reason).FirstOrDefaultAsync();

                        // If a LoanProduct with the same name already exists, return a conflict response
                        if (loanPurpose == null)
                        {
                            var errorMessage = $"loanPurpose {upLoan.Reason} does not exists.";
                            _logger.LogError(errorMessage);
                            upLoan.Description = errorMessage;
                            loanUploadListLeft.Add(upLoan);
                            continue;
                            //return ServiceResponse<List<LoanUpload>>.Return404(errorMessage);
                        }

                        // Check if a LoanProduct with the same name already exists (case-insensitive)
                        loanProduct = await _loanProductRepository.AllIncluding().Where(c => c.ProductCode == upLoan.LoanProduct).FirstOrDefaultAsync();

                        // If a LoanProduct with the same name already exists, return a conflict response
                        if (loanProduct == null)
                        {
                            var errorMessage = $"LoanProduct {upLoan.LoanProduct} does not exists.";
                            _logger.LogError(errorMessage);
                            upLoan.Description = errorMessage;
                            loanUploadListLeft.Add(upLoan);
                            continue;
                            //return ServiceResponse<List<LoanUpload>>.Return404(errorMessage);
                        }


                        LoanApplication loanApplication = createLoanApplication(upLoan, loanProduct.Id, loanPurpose.Id, economicActivityDto.Id);
                        loanApplication.Id = id;

                        _loanApplicationRepository.Add(loanApplication);

                        Loan loan = createLoan(upLoan, loanApplication.Id);

                        var deliquencyRange = await getDeliquencyRange(loan.DeliquentDays);
                        loan.LoanDeliquencyConfigurationId = deliquencyRange != null ? deliquencyRange.Id : "";
                        loan.Id = id;
                        _loanRepository.Add(loan);

                        DisburstedLoan disburstedLoan = createDisburse(upLoan, loan.Id);
                        disburstedLoan.Id = id + "1";
                        _disburstedLoanRepository.Add(disburstedLoan);

                        // Check if a LoanProduct with the same name already exists (case-insensitive)
                        LoanAmortization loanAmortization = await _loanAmortizationRepository.All.Where(c => c.Id == id).FirstOrDefaultAsync();
                        //var loanAmortization = _loanAmortizationRepository.All.ToList();

                        // If a LoanProduct with the same name already exists, return a conflict response
                        if (loanAmortization == null)
                        {
                            var errorMessage = $"LoanAmortization {id} does not exists.";
                            _logger.LogInformation(errorMessage);
                            AddLoanAmortizationCommand addLoanAmortizationCommand = new AddLoanAmortizationCommand();
                            addLoanAmortizationCommand.LoanId = id;
                            addLoanAmortizationCommand.LoanApplicationId = id;
                            loanAmortizationCommands.Add(addLoanAmortizationCommand);
                            //    _mediator.Send(addLoanAmortizationCommand, cancellationToken);
                            //return ServiceResponse<List<LoanUpload>>.Return404(errorMessage);
                        }

                    }

                    await _uow.SaveAsync();

                    //foreach (var addLoanAmortizationCommand in loanAmortizationCommands)
                    //{
                    //    _mediator.Send(addLoanAmortizationCommand, cancellationToken);
                    //}

                    return ServiceResponse<List<LoanUpload>>.ReturnResultWith200(loanUploadListLeft);

                }
                else
                {

                    return ServiceResponse<List<LoanUpload>>.Return500("The file is not an XLS or XLSX file.");

                }
            }
            catch (Exception e)
            {
                // Log error and return 500 Internal Server Error response with error message
                var errorMessage = $"Error occurred while saving Customer: {e.Message}";
                _logger.LogError(errorMessage);
                await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), e.Message, $"Internal Server Error occurred while saving new Customer ", LogLevelInfo.Error.ToString(), 500, _userInfoToken.Token);

                return ServiceResponse<List<LoanUpload>>.Return500(e);
            }
        }



        private async Task LogAuditError(LoanUploadCommand request, string errorMessage)
        {
            // Serialize the request data
            string serializedRequest = JsonConvert.SerializeObject(request);

            // Log the audit error
            await APICallHelper.AuditLogger(_userInfoToken.Email, LogAction.Create.ToString(), serializedRequest, errorMessage, LogLevelInfo.Information.ToString(), 500, _userInfoToken.Token);
        }

        private LoanApplication createLoanApplication(LoanUpload loanUpload, string LoanProductId, string loanPurposeId, string economicActivityId)
        {
            var customerId = loanUpload.BranchCode.Substring(loanUpload.BranchCode.Length - 3) + "" + loanUpload.MemberNumber;
            return new LoanApplication(
            loanProductId: LoanProductId,
            amount: loanUpload.LoanAmount,
            interestRate: loanUpload.IntRate,
            numberOfRepayment: 10,
            loanDuration: loanUpload.NBRECHE,
            firstInstallmentDate: loanUpload.ValidationDate,
            applicationDate: loanUpload.ValidationDate,
            approvalDate: loanUpload.ValidationDate,
            disbursementDate: loanUpload.ValidationDate,
            customerId: customerId,
            loanPurposeId: loanPurposeId,
            organizationId: "1",
            branchId: _userInfoToken.BranchID,
            bankId: _userInfoToken.BankID,
            economicActivityId: economicActivityId,
            customerName: loanUpload.MemberName,
            branchName: _userInfoToken.BranchName,
            branchCode: _userInfoToken.BranchCode
            );
        }

        private DateTime CalculateNextInstallement(string period,DateTime validationDate)
        {
            switch (period)
            {
                case "D":
                    return validationDate.AddDays(1);
                case "M":
                    return validationDate.AddMonths(1);
                case "Y":
                    return validationDate.AddYears(1);
                default:
                    break;
            }

            return validationDate.AddMonths(1);
        }

        private Loan createLoan(LoanUpload loanUpload, string loanApplicationId)
        {
            var amountWithInterest = Converter.roundUp(loanUpload.Amount * (1 + (loanUpload.IntRate / 100)));
            //var interest = Converter.roundUp(amountWithInterest - loanUpload.LoanAmount);
            var interest = Converter.roundUp(amountWithInterest - loanUpload.Amount);
            var paid = loanUpload.Amount - loanUpload.Balance;
            var paidCapital = paid / (1 + (loanUpload.IntRate / 100));
            //var paidInterest = paid - paidCapital;
            var debt = loanUpload.Balance;
            //var debt = amountWithInterest - paid;
            //var balance = debt;
            if (loanUpload.Interest > 0)
            {
                debt = loanUpload.Interest + debt;
                interest = loanUpload.Interest;
            }
            var loanStatus = debt == 0 ? LoanStatus.Closed.ToString() : LoanStatus.Open.ToString();
            var isCurrent = !(debt == 0);
            var branchCode = loanUpload.BranchCode.Substring(loanUpload.BranchCode.Length - 3);
            //var branchCode = loanUpload.BranchCode.Substring(loanUpload.BranchCode.Length - 2);
            var customerId = branchCode + "" + loanUpload.MemberNumber;
            var loanAccount = loanUpload.LoanAccount;//.Substring(0, 6);

            DateTime firstInstallmentDate = CalculateNextInstallement(loanUpload.Periodic, loanUpload.ValidationDate);
         
            int advancedPaymentDays = loanUpload.AdvanceDays;
            int deliquentDays = loanUpload.DeliquentDays;
            decimal advancedPaymentAmount = loanUpload.DeliquentAmount;
            decimal deliquentAmount = loanUpload.AdvanceAmount;
            string loanType = loanUpload.LoanType == null ? LoanType.Main_Loan.ToString() : loanUpload.LoanType;
            string delinquentStatus = loanUpload.DeliquentStatus == "Delinquent_Loan" ? LoanDeliquentStatus.Delinquent.ToString() : LoanDeliquentStatus.Current.ToString();

            decimal deliquentInterest = loanUpload.DelayInterest * deliquentDays;


            return new Loan(
                loanApplicationId: loanApplicationId,
                principal: BaseUtilities.RoundUpValue(loanUpload.Balance),
                //principal: (decimal)loanUpload.LoanAmount,
                loanAmount: loanUpload.Amount,
                interestForcasted: BaseUtilities.RoundUpValue(interest),
                interestRate: loanUpload.IntRate,
                paid: BaseUtilities.RoundUpValue(paid),
                balance: BaseUtilities.RoundUpValue(loanUpload.Balance),
                dueAmount: BaseUtilities.RoundUpValue(debt),
                //dueAmount: (decimal)amountWithInterest,
                disbursementDate: loanUpload.ValidationDate,
                firstInstallmentDate: firstInstallmentDate,
                nextInstallmentDate: loanUpload.ValidationDate,
                loanDate: loanUpload.ValidationDate,
                customerId: customerId,
                maturityDate: loanUpload.ValidationDate.AddMonths(loanUpload.NBRECHE),
                organizationId: "1",
                branchId: _userInfoToken.BranchID,
                bankId: _userInfoToken.BankID,
                loanStatus: loanStatus,
                disbursmentStatus: DisbursmentStatus.Disbursed.ToString(),
                isCurrentLoan: isCurrent,
                lastRefundDate: loanUpload.LastRepayment,
                memberName: loanUpload.MemberName,
                branchcode: branchCode,
                loanDuration: loanUpload.NBRECHE,
                loanType: loanType,
                accrualInterest: BaseUtilities.RoundUpValue(loanUpload.Interest),
                //accrualInterest: (decimal)loanUpload.Interest,
                accrualInterestPaid:BaseUtilities.RoundUpValue( paidCapital),
                totalPrincipalPaid: BaseUtilities.RoundUpValue(paid),
                accountNumber: loanAccount,
                deliquentInterest: BaseUtilities.RoundUpValue(deliquentInterest),
                advancedPaymentDays: advancedPaymentDays,
                deliquentDays: deliquentDays,
                advancedPaymentAmount: BaseUtilities.RoundUpValue(advancedPaymentAmount),
                deliquentAmount: BaseUtilities.RoundUpValue(deliquentAmount),
                delinquentStatus: delinquentStatus
               
            //dueInterest: (decimal)interest
            );
        }

        private async Task<LoanDeliquencyConfigurationDto> getDeliquencyRange(int delinquencyDays)
        {
            GetLoanDeliquencyConfigurationWithDaysCountQuery request = new()
            {
                days = delinquencyDays
            };

            var response = await _mediator.Send(request);
            if (response.Success && response.Data != null)
            {
                return response.Data;
            }
            return null;
        }
        //string memberName, string branchcode,string loanDuration,string loanType
        private DisburstedLoan createDisburse(LoanUpload loanUpload, string loanId)
        {
            return new DisburstedLoan()
            {
                LoanId = loanId,
                DisbursmentDate = loanUpload.ValidationDate,
                DisbursementStatus = DisbursmentStatus.Disbursed.ToString(),
                DisbursedBy = _userInfoToken.FullName,
                Comment = "Upload Loan flow"
            };
        }
        private EconomicActivityDto extractEconomicActivityByName(string name, List<EconomicActivityDto> activities)
        {
            return activities.FirstOrDefault(activity => activity.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

    }
}