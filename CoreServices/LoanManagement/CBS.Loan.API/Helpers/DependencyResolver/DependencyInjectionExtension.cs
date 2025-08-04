using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.LoanTermP;
using CBS.NLoan.Data.Enums;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Handlers;
using CBS.NLoan.MediatR.LoanCalculatorHelper.DeliquencyCalculations;
using CBS.NLoan.MediatR.LoanCalculatorHelper.InterestCalculationService;
using CBS.NLoan.Repository;
using CBS.NLoan.Repository.AccountingRuleP;
using CBS.NLoan.Repository.AlertProfileP;
using CBS.NLoan.Repository.CollateralP;
using CBS.NLoan.Repository.CommiteeP.LoanCommeteeMemberP;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeGroupP;
using CBS.NLoan.Repository.CommiteeP.LoanCommiteeValidationP;
using CBS.NLoan.Repository.CustomerLoanAccountP;
using CBS.NLoan.Repository.CustomerProfileP;
using CBS.NLoan.Repository.DocumentP;
using CBS.NLoan.Repository.FeeP.FeeP;
using CBS.NLoan.Repository.FeeRangeP;
using CBS.NLoan.Repository.FileDownloadInfoP;
using CBS.NLoan.Repository.FundingLineP;
using CBS.NLoan.Repository.InterestCalculationP;
using CBS.NLoan.Repository.LoanApplicationFeeP;
using CBS.NLoan.Repository.LoanApplicationP;
using CBS.NLoan.Repository.LoanApplicationP.DisburstLoan;
using CBS.NLoan.Repository.LoanApplicationP.LoanAmortizationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanCommentryP;
using CBS.NLoan.Repository.LoanApplicationP.LoanCycleP;
using CBS.NLoan.Repository.LoanApplicationP.LoanDeliquencyConfigurationP;
using CBS.NLoan.Repository.LoanApplicationP.LoanGuarantorP;
using CBS.NLoan.Repository.LoanApplicationP.LoanP;
using CBS.NLoan.Repository.LoanApplicationP.RescheduleLoanP;
using CBS.NLoan.Repository.LoanProductP;
using CBS.NLoan.Repository.LoanPurposeP;
using CBS.NLoan.Repository.LoanTermP;
using CBS.NLoan.Repository.Notification;
using CBS.NLoan.Repository.Notifications;
using CBS.NLoan.Repository.PenaltyP;
using CBS.NLoan.Repository.PeriodP;
using CBS.NLoan.Repository.RefundP;
using CBS.NLoan.Repository.TaxP;
using CBS.NLoan.Repository.WriteOffLoanP;
using CBS.NLoan.Repository.RefundMongoP;
using CBS.NLoan.Common.MongoUnitOfWork;

namespace CBS.NLoan.API.Helpers.DependencyResolver
{
    public static class DependencyInjectionExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<AddLoanApprovalCommandCommandHandler>();
            services.AddScoped(typeof(IUnitOfWork<>), typeof(UnitOfWork<>));
            services.AddScoped<ILoanApplicationRepository, LoanApplicationRepository>().Reverse();
            services.AddScoped<ILoanProductRepository, LoanProductRepository>().Reverse();
            services.AddScoped<ICustomerProfileRepository, CustomerProfileRepository>().Reverse();
            services.AddScoped<ILoanDeliquencyConfigurationRepository, LoanDeliquencyConfigurationRepository>().Reverse();
            services.AddScoped<ILoanRepository, LoanRepository>().Reverse();
            services.AddScoped<IPropertyMappingService, PropertyMappingService>().Reverse();
            services.AddScoped<IDisburstedLoanRepository, DisburstedLoanRepository>().Reverse();
            services.AddScoped<IAccountingRuleRepository, AccountingRuleRepository>().Reverse();
            services.AddScoped<IPeriodRepository, PeriodRepository>().Reverse();
            services.AddScoped<IDocumentRepository, DocumentRepository>().Reverse();
            services.AddScoped<IDocumentPackRepository, DocumentPackRepository>().Reverse();
            services.AddScoped<IDocumentJoinRepository, DocumentJoinRepository>().Reverse();

            services.AddScoped<IRefundRepository, RefundRepository>().Reverse();
            services.AddScoped<IRefundRepository, RefundRepository>().Reverse();
            services.AddScoped<ILoanAmortizationRepository, LoanAmortizationRepository>().Reverse();

            services.AddScoped<ICustomerLoanAccountRepository, CustomerLoanAccountRepository>().Reverse();

            services.AddScoped<ILoanCommeteeMemberRepository, LoanCommeteeMemberRepository>().Reverse();
            services.AddScoped<ILoanCommiteeGroupRepository, LoanCommiteeGroupRepository>().Reverse();
            services.AddScoped<ILoanCommiteeValidationHistoryRepository, LoanCommiteeValidationHistoryRepository>().Reverse();

            services.AddScoped<IDocumentAttachedToLoanRepository, DocumentAttachedToLoanRepository>().Reverse();
            services.AddScoped<IOTPNotificationRepository, OTPNotificationRepository>().Reverse();
            services.AddScoped<ILoanTermRepository, LoanTermRepository>().Reverse();
            
            services.AddScoped<ITaxRepository, TaxRepository>().Reverse();
            services.AddScoped<ICollateralRepository, CollateralRepository>().Reverse();
            services.AddScoped<ILoanCollateralRepository, LoanCollateralRepository>().Reverse();
            services.AddScoped<ILoanGuarantorRepository, LoanGuarantorRepository>().Reverse();

            services.AddScoped<IFeeRepository, FeeRepository>().Reverse();
            services.AddScoped<ILoanNotificationRepository, LoanNotificationRepository>().Reverse();
            //
            services.AddScoped<ILoanCommentryRepository, LoanCommentryRepository>().Reverse();

            services.AddScoped<IRescheduleLoanRepository, RescheduleLoanRepository>().Reverse();
            services.AddScoped<IRefundDetailRepository, RefundDetailRepository>().Reverse();
            services.AddScoped<IDailyInterestCalculationRepository, DailyInterestCalculationRepository>().Reverse();
            //services.AddScoped<LoanDelinquencyBackgroundService, LoanDelinquencyBackgroundService>().Reverse();
            //
            services.AddScoped<IFundingLineRepository, FundingLineRepository>().Reverse();
            services.AddScoped<IDelinquencyService, DelinquencyService>().Reverse();
            services.AddScoped<IWriteOffLoanRepository, WriteOffLoanRepository>().Reverse();

            services.AddScoped<ILoanPurposeRepository, LoanPurposeRepository>().Reverse();
            services.AddScoped<IAlertProfileRepository, AlertProfileRepository>().Reverse();
            services.AddScoped<ILoanProductCollateralRepository, LoanProductCollateralRepository>();
            services.AddScoped<IPenaltyRepository, PenaltyRepository>().Reverse();
            services.AddScoped<ILoanProductCategoryRepository, LoanProductCategoryRepository>().Reverse();
            services.AddScoped<ILoanProductRepaymentOrderRepository, LoanProductRepaymentOrderRepository>().Reverse();
            services.AddScoped<ILoanProductRepaymentCycleRepository, LoanProductRepaymentCycleRepository>().Reverse();
            services.AddScoped<IFeeRangeRepository, FeeRangeRepository>().Reverse();
            services.AddScoped<ILoanApplicationFeeRepository, LoanApplicationFeeRepository>().Reverse();
            services.AddScoped<IFileDownloadInfoRepository, FileDownloadInfoRepository>().Reverse();
            // Register other services
            services.AddScoped<IInterestForLoanCalculationServices, InterestForLoanCalculationServices>();

            services.AddScoped<IMongoUnitOfWork,MongoUnitOfWork>();
            services.AddScoped<ILoanRepaymentMongoRepository, LoanRepaymentMongoRepository>().Reverse();




        }
    }
}
