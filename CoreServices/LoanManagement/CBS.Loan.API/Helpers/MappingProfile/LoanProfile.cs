using AutoMapper;
using CBS.NLoan.Data;
using CBS.NLoan.Data.Dto.AlertProfileP;
using CBS.NLoan.Data.Dto.CollateraP;
using CBS.NLoan.Data.Dto.CommiteeP;
using CBS.NLoan.Data.Dto.CustomerLoanAccountP;
using CBS.NLoan.Data.Dto.DocumentP;
using CBS.NLoan.Data.Dto.FeeP;
using CBS.NLoan.Data.Dto.FileDownloadInfoP;
using CBS.NLoan.Data.Dto.FundingLineP;
using CBS.NLoan.Data.Dto.LoanApplicationP;
using CBS.NLoan.Data.Dto.LoanPurposeP;
using CBS.NLoan.Data.Dto.LoanTermP;
using CBS.NLoan.Data.Dto.Notifications;
using CBS.NLoan.Data.Dto.PenaltyP;
using CBS.NLoan.Data.Dto.PeriodP;
using CBS.NLoan.Data.Dto.RefundP;
using CBS.NLoan.Data.Dto.TaxP;
using CBS.NLoan.Data.Dto.WriteOffLoanP;
using CBS.NLoan.Data.Entity.AlertProfileP;
using CBS.NLoan.Data.Entity.CollateraP;
using CBS.NLoan.Data.Entity.CommiteeP;
using CBS.NLoan.Data.Entity.CustomerLoanAccountP;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Data.Entity.FeeP;
using CBS.NLoan.Data.Entity.FileDownloadInfoP;
using CBS.NLoan.Data.Entity.FundingLineP;
using CBS.NLoan.Data.Entity.LoanApplicationP;
using CBS.NLoan.Data.Entity.LoanPurposeP;
using CBS.NLoan.Data.Entity.LoanTermP;
using CBS.NLoan.Data.Entity.Notifications;
using CBS.NLoan.Data.Entity.PenaltyP;
using CBS.NLoan.Data.Entity.PeriodP;
using CBS.NLoan.Data.Entity.RefundP;
using CBS.NLoan.Data.Entity.TaxP;
using CBS.NLoan.Data.Entity.WriteOffLoanP;
using CBS.NLoan.MediatR.AlertProfileMediaR.Commands;
using CBS.NLoan.MediatR.CollateralMediaR.Commands;
using CBS.NLoan.MediatR.Commands;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommeteeMemberMediaR.Commands;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationCriteriaMediaR.Commands;
using CBS.NLoan.MediatR.CommiteeMediaR.LoanCommiteeValidationMediaR.Commands;
using CBS.NLoan.MediatR.CustomerLoanAccountMediaR.Commands;
using CBS.NLoan.MediatR.CycleNameMediaR.Commands;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentAttachedToLoanP.Commands;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentP.Commands;
using CBS.NLoan.MediatR.DocumentMediaR.DocumentPackP.Commands;
using CBS.NLoan.MediatR.FeeMediaR.FeeP.Commands;
using CBS.NLoan.MediatR.FeeMediaR.FeeRangeP.Commands;
using CBS.NLoan.MediatR.FundingLineMediaR.Commands;
using CBS.NLoan.MediatR.LoanApplicationFeeMediaR.LoanApplicationFeeP.Commands;
using CBS.NLoan.MediatR.LoanApplicationMediaR.Commands;
using CBS.NLoan.MediatR.LoanCollateralMediaR.Commands;
using CBS.NLoan.MediatR.LoanCommentryMediaR.Commands;
using CBS.NLoan.MediatR.LoanDeliquencyConfigurationMediaR.Commands;
using CBS.NLoan.MediatR.LoanGuarantorMediaR.Commands;
using CBS.NLoan.MediatR.LoanMediaR.Commands;
using CBS.NLoan.MediatR.LoanProductCollateralMediaR.Commands;
using CBS.NLoan.MediatR.LoanProductMediaR.Commands;
using CBS.NLoan.MediatR.LoanPurposeMediaR.Commands;
using CBS.NLoan.MediatR.LoanTermP.Commands;
using CBS.NLoan.MediatR.MediatR.FileDownloadInfoP.Commands;
using CBS.NLoan.MediatR.Notifications.Commands;
using CBS.NLoan.MediatR.PenaltyMediaR.Commands;
using CBS.NLoan.MediatR.PeriodMediaR.Commands;
using CBS.NLoan.MediatR.RefundMediaR.Commands;
using CBS.NLoan.MediatR.TaxMediaR.Commands;
using CBS.NLoan.MediatR.WriteOffLoanMediaR.Commands;
using CBS.TransactionManagement.Data.Dto.OTP;

namespace CBS.NLoan.API.Helpers.MappingProfile
{
    public class LoanProfile : Profile
    {
        public LoanProfile()
        {
            CreateMap<LoanApplication, LoanApplicationDto>().ReverseMap();
            CreateMap<AddLoanApplicationCommand, LoanApplication>().ReverseMap();
            CreateMap<UpdateLoanApplicationCommand, LoanApplication>().ReverseMap();
            CreateMap<UpdateLoanApplicationCommand, LoanProductDto>().ReverseMap();
            CreateMap<LoanApplication, AddLoanApprovalCommand>().ReverseMap();
            CreateMap<LoanApplicationDto, AddLoanApprovalCommand>().ReverseMap();
            CreateMap<AddLoanApplicationCommand, SimulateLoanInstallementQuery>().ReverseMap();
            CreateMap<LoanApplication, SimulateLoanInstallementQuery>().ReverseMap();

            CreateMap<LoanDeliquencyConfiguration, LoanDeliquencyConfigurationDto>().ReverseMap();
            CreateMap<LoanDeliquencyConfiguration, AddLoanDeliquencyConfigurationCommand>().ReverseMap();
            CreateMap<LoanDeliquencyConfiguration, UpdateLoanDeliquencyConfigurationCommand>().ReverseMap();

            CreateMap<LoanProduct, LoanProductDto>().ReverseMap();
            CreateMap<LoanProduct, LoanProductConfigurationAgregatesDto>().ReverseMap();
            CreateMap<AddLoanProductCommand, LoanProduct>().ReverseMap();
            CreateMap<UpdateLoanProductCommand, LoanProduct>().ReverseMap();
            CreateMap<DeleteLoanProductCommand, LoanProduct>().ReverseMap();

            CreateMap<CustomerProfile, CustomerProfileDto>().ReverseMap();
            CreateMap<CustomerProfile, CreateClientGroupDto>().ReverseMap();

            CreateMap<LoanAmortization, LoanAmortizationDto>().ReverseMap();

            CreateMap<Refund, RefundDto>().ReverseMap();

            CreateMap<Document, DocumentDto>().ReverseMap();
            CreateMap<Document, AddDocumentCommand>().ReverseMap();
            CreateMap<Document, UpdateDocumentCommand>().ReverseMap();

            CreateMap<OTPNotification, OTPNotificationDto>().ReverseMap();
            CreateMap<OTPNotification, AddOTPNotificationCommand>().ReverseMap();
            CreateMap<OTPNotification, UpdateOTPNotificationCommand>().ReverseMap();

            CreateMap<LoanApplicationFee, LoanApplicationFeeDto>().ReverseMap();
            CreateMap<LoanApplicationFee, AddLoanApplicationFeeCommand>().ReverseMap();
            CreateMap<LoanApplicationFee, UpdateLoanApplicationFeeCommand>().ReverseMap();

            CreateMap<FeeRange, FeeRangeDto>().ReverseMap();
            CreateMap<FeeRange, AddFeeRangeCommand>().ReverseMap();
            CreateMap<FeeRange, UpdateFeeRangeCommand>().ReverseMap();

            CreateMap<LoanTerm, LoanTermDto>().ReverseMap();
            CreateMap<LoanTerm, AddLoanTermCommand>().ReverseMap();
            CreateMap<LoanTerm, UpdateLoanTermCommand>().ReverseMap();

            CreateMap<LoanAmortization, LoanAmortizationDto>().ReverseMap();
            CreateMap<LoanAmortization, AddLoanAmortizationCommand>().ReverseMap();
            //CreateMap<LoanAmortization, UpdateLoanAmortizationCommand>().ReverseMap();

            CreateMap<DocumentPack, DocumentPackDto>().ReverseMap();
            CreateMap<DocumentPack, AddDocumentPackCommand>().ReverseMap();
            CreateMap<DocumentPack, UpdateDocumentPackCommand>().ReverseMap();
            CreateMap<DocumentPack, AddDocumentToPackCommand>().ReverseMap();

            CreateMap<DocumentJoin, AddDocumentToPackCommand>().ReverseMap();
            CreateMap<DocumentJoin, DocumentJoinDto>().ReverseMap();

            CreateMap<Period, PeriodDto>().ReverseMap();
            CreateMap<Period, AddPeriodCommand>().ReverseMap();
            CreateMap<Period, UpdatePeriodCommand>().ReverseMap();

            CreateMap<LoanProductCollateral, LoanProductCollateralDto>().ReverseMap();
            CreateMap<LoanProductCollateral, AddLoanProductCollateralCommand>().ReverseMap();
            CreateMap<LoanProductCollateral, UpdateLoanProductCollateralCommand>().ReverseMap();

            CreateMap<Refund, RefundDto>().ReverseMap();
            CreateMap<Refund, AddRefundCommand>().ReverseMap();


            CreateMap<CustomerLoanAccount, CustomerLoanAccountDto>().ReverseMap();
            CreateMap<CustomerLoanAccount, AddCustomerLoanAccountCommand>().ReverseMap();
            CreateMap<CustomerLoanAccount, UpdateCustomerLoanAccountCommand>().ReverseMap();

            CreateMap<LoanCommiteeMember, LoanCommiteeMemberDto>().ReverseMap();
            CreateMap<LoanCommiteeMember, AddLoanCommeteeMemberCommand>().ReverseMap();
            CreateMap<LoanCommiteeMember, UpdateLoanCommeteeMemberCommand>().ReverseMap();

            CreateMap<LoanCommiteeGroup, LoanCommiteeGroupDto>().ReverseMap();
            CreateMap<LoanCommiteeGroup, AddLoanCommiteeGroupCommand>().ReverseMap();
            CreateMap<LoanCommiteeGroup, UpdateLoanCommiteeGroupCommand>().ReverseMap();

            CreateMap<LoanCommiteeValidationHistory, LoanCommiteeValidationHistoryDto>().ReverseMap();
            CreateMap<LoanCommiteeValidationHistory, AddLoanCommiteeValidationHistoryCommand>().ReverseMap();
            CreateMap<LoanCommiteeValidationHistory, UpdateLoanCommiteeValidationHistoryCommand>().ReverseMap();

            CreateMap<DocumentAttachedToLoan, DocumentAttachedToLoanDto>().ReverseMap();
            CreateMap<DocumentAttachedToLoan, DocumentAttachedToLoanCommand>().ReverseMap();
            CreateMap<DocumentAttachedToLoan, UpdateDocumentAttachedToLoanCommand>().ReverseMap();

            CreateMap<Collateral, CollateralDto>().ReverseMap();
            CreateMap<Collateral, AddCollateralCommand>().ReverseMap();
            CreateMap<Collateral, UpdateCollateralCommand>().ReverseMap();

            CreateMap<LoanApplicationCollateral, LoanApplicationCollateralDto>().ReverseMap();
            CreateMap<LoanApplicationCollateral, AddLoanCollateralCommand>().ReverseMap();
            CreateMap<LoanApplicationCollateral, UpdateLoanCollateralCommand>().ReverseMap();

            CreateMap<LoanGuarantor, LoanGuarantorDto>().ReverseMap();
            CreateMap<LoanGuarantor, AddLoanGuarantorCommand>().ReverseMap();
            CreateMap<LoanGuarantor, UpdateLoanGuarantorCommand>().ReverseMap();

            CreateMap<LoanParameters, LoanAmortizationDto>().ReverseMap();
            CreateMap<LoanParameters, SimulateLoanInstallementQuery>().ReverseMap();



            CreateMap<FileDownloadInfo, AddFileDownloadInfoCommand>().ReverseMap();
            CreateMap<FileDownloadInfo, FileDownloadInfoDto>().ReverseMap();



            CreateMap<Tax, TaxDto>().ReverseMap();
            CreateMap<Tax, AddTaxCommand>().ReverseMap();
            CreateMap<Tax, UpdateTaxCommand>().ReverseMap();

            CreateMap<Fee, FeeDto>().ReverseMap();
            CreateMap<Fee, AddFeeCommand>().ReverseMap();
            CreateMap<Fee, UpdateFeeCommand>().ReverseMap();


            CreateMap<LoanCommentry, LoanCommentryDto>().ReverseMap();
            CreateMap<LoanCommentry, AddLoanCommentryCommand>().ReverseMap();
            CreateMap<LoanCommentry, UpdateLoanCommentryCommand>().ReverseMap();

            CreateMap<FundingLine, FundingLineDto>().ReverseMap();
            CreateMap<FundingLine, AddFundingLineCommand>().ReverseMap();
            CreateMap<FundingLine, UpdateFundingLineCommand>().ReverseMap();

            CreateMap<WriteOffLoan, WriteOffLoanDto>().ReverseMap();
            CreateMap<WriteOffLoan, AddWriteOffLoanCommand>().ReverseMap();
            CreateMap<WriteOffLoan, UpdateWriteOffLoanCommand>().ReverseMap();

            CreateMap<LoanPurpose, LoanPurposeDto>().ReverseMap();
            CreateMap<LoanPurpose, AddLoanPurposeCommand>().ReverseMap();
            CreateMap<LoanPurpose, UpdateLoanPurposeCommand>().ReverseMap();

            CreateMap<AlertProfile, AlertProfileDto>().ReverseMap();
            CreateMap<AlertProfile, AddAlertProfileCommand>().ReverseMap();
            CreateMap<AlertProfile, UpdateAlertProfileCommand>().ReverseMap();

            CreateMap<Penalty, PenaltyDto>().ReverseMap();
            CreateMap<Penalty, AddPenaltyCommand>().ReverseMap();
            CreateMap<Penalty, UpdatePenaltyCommand>().ReverseMap();

            
            CreateMap<Loan, LoanDto>().ReverseMap();
            CreateMap<Loan, LightLoanDto>().ReverseMap();
            CreateMap<Loan, AddLoanCommand>().ReverseMap();
            //CreateMap<Loan, UpdateLoanCommand>().ReverseMap();

            CreateMap<LoanProductCategory, LoanProductCategoryDto>().ReverseMap();
            CreateMap<LoanProductCategory, AddLoanProductCategoryCommand>().ReverseMap();
            CreateMap<LoanProductCategory, UpdateLoanProductCategoryCommand>().ReverseMap();

            CreateMap<LoanProductRepaymentOrder, LoanProductRepaymentOrderDto>().ReverseMap();
            CreateMap<LoanProductRepaymentOrder, AddLoanProductRepaymentOrderCommand>().ReverseMap();
            CreateMap<LoanProductRepaymentOrder, UpdateLoanProductRepaymentOrderCommand>().ReverseMap();

            CreateMap<LoanProductRepaymentCycle, LoanProductRepaymentCycleDto>().ReverseMap();
            CreateMap<LoanProductRepaymentCycle, AddLoanProductRepaymentCycleCommand>().ReverseMap();
            CreateMap<LoanProductRepaymentCycle, UpdateLoanProductRepaymentCycleCommand>().ReverseMap();
        }
    }
}
