using AutoMapper;
using CBS.TransactionManagement.CashCeilingMovement.Commands;
using CBS.TransactionManagement.CashOutThirdPartyP.Commands;
using CBS.TransactionManagement.Commands;
using CBS.TransactionManagement.Commands.ChargesWaivedP;
using CBS.TransactionManagement.DailyTellerP.Commands;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.ChargesWaivedP;
using CBS.TransactionManagement.Data.Dto;
using CBS.TransactionManagement.Data.Dto.AccountingDayOpening;
using CBS.TransactionManagement.Data.Dto.CashCeilingMovement;
using CBS.TransactionManagement.Data.Dto.CashOutThirdPartyP;
using CBS.TransactionManagement.Data.Dto.ChargesWaivedP;
using CBS.TransactionManagement.Data.Dto.DailyStatisticBoard;
using CBS.TransactionManagement.Data.Dto.FileDownloadInfoP;
using CBS.TransactionManagement.Data.Dto.HolyDayP;
using CBS.TransactionManagement.Data.Dto.HolyDayRecurringP;
using CBS.TransactionManagement.Data.Dto.MemberNoneCashOperationP;
using CBS.TransactionManagement.Data.Dto.MobileMoney;
using CBS.TransactionManagement.Data.Dto.OldLoanConfiguration;
using CBS.TransactionManagement.Data.Dto.OtherCashInP;
using CBS.TransactionManagement.Data.Dto.Receipts.Details;
using CBS.TransactionManagement.Data.Dto.Receipts.Payments;
using CBS.TransactionManagement.Data.Dto.RemittanceP;
using CBS.TransactionManagement.Data.Dto.ReversalRequestP;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Data.Dto.TransactionTrackerAccountingData;
using CBS.TransactionManagement.Data.Dto.VaultP;
using CBS.TransactionManagement.Data.Entity.AccountingDayOpening;
using CBS.TransactionManagement.Data.Entity.CashCeilingMovement;
using CBS.TransactionManagement.Data.Entity.CashOutThirdPartyP;
using CBS.TransactionManagement.Data.Entity.CashVaultP;
using CBS.TransactionManagement.Data.Entity.ChangeManagement;
using CBS.TransactionManagement.Data.Entity.DailyStatisticBoard;
using CBS.TransactionManagement.Data.Entity.FileDownloadInfoP;
using CBS.TransactionManagement.Data.Entity.HolyDayP;
using CBS.TransactionManagement.Data.Entity.HolyDayRecurringP;
using CBS.TransactionManagement.Data.Entity.MemberNoneCashOperationP;
using CBS.TransactionManagement.Data.Entity.MobileMoney;
using CBS.TransactionManagement.Data.Entity.MongoDBObjects;
using CBS.TransactionManagement.Data.Entity.OldLoanConfiguration;
using CBS.TransactionManagement.Data.Entity.OtherCashInP;
using CBS.TransactionManagement.Data.Entity.Receipts.Details;
using CBS.TransactionManagement.Data.Entity.Receipts.Payments;
using CBS.TransactionManagement.Data.Entity.RemittanceP;
using CBS.TransactionManagement.Data.Entity.ReversalRequestP;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Data.Entity.SalaryFilesDto;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.StandingOrderP;
using CBS.TransactionManagement.Data.Entity.ThirtPartyPayment;
using CBS.TransactionManagement.Data.SalaryFilesDto.SalaryFiles;
using CBS.TransactionManagement.Dto;
using CBS.TransactionManagement.MediatR.CashReplenishmentPrimaryTellerP.Commands;
using CBS.TransactionManagement.MediatR.Commands.ReversalRequestP;
using CBS.TransactionManagement.MediatR.FileDownloadInfoP.Commands;
using CBS.TransactionManagement.MediatR.HolyDayMediaR.HolyDayP.Commands;
using CBS.TransactionManagement.MediatR.HolyDayP.Commands;
using CBS.TransactionManagement.MediatR.HolyDayRecurringP.Commands;
using CBS.TransactionManagement.MediatR.MemberNoneCashOperationP.Commands;
using CBS.TransactionManagement.MediatR.MobileMoney.Commands;
using CBS.TransactionManagement.MediatR.PrimaryTellerProvisioningP.Commands;
using CBS.TransactionManagement.MediatR.RecurringRecurringP.Commands;
using CBS.TransactionManagement.MediatR.RemittanceP.Commands;
using CBS.TransactionManagement.MediatR.SalaryManagement.StandingOrderP.Commands;
using CBS.TransactionManagement.MediatR.ThirtPartyPayment.Commands;
using CBS.TransactionManagement.MediatR.TransactionTrackerAccountingData.Commands;
using CBS.TransactionManagement.MediatR.VaultP;
using CBS.TransactionManagement.OldLoanConfiguration.Commands;
using CBS.TransactionManagement.otherCashIn.Commands;
using CBS.TransactionManagement.Repository;
using CBS.TransactionManagement.Repository.MobileMoney;

namespace CBS.TransactionManagement.API.Helpers.MappingProfile
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            CreateMap<Account, AccountDto>();
            //.ForMember(dest => dest.Balance, opt => opt.MapFrom(src => BaseUtilities.FormatCurrency(src.Balance, "XAF", 2)))
            //.ForMember(dest => dest.PreviousBalance, opt => opt.MapFrom(src => BaseUtilities.FormatCurrency(src.PreviousBalance, "XAF", 2))).ReverseMap();
            CreateMap<Account, AccountShortDto>().ReverseMap();
            CreateMap<Account, AddAccountCommand>().ReverseMap();
            CreateMap<Account, AccountBalanceDto>().ReverseMap();
            CreateMap<Account, AccountBalanceThirdPartyDto>().ReverseMap();
            //AccountBalanceThirdPartyDto
            // .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.Product.Name))
            //.ForMember(dest => dest.Balance, opt => opt.MapFrom(src => BaseUtilities.FormatCurrency(src.Balance, "XAF", 2))).ReverseMap();

            CreateMap<AddChargesWaivedCommand, ChargesWaived>().ReverseMap();
            CreateMap<UpdateChargesWaivedCommand, ChargesWaived>().ReverseMap();
            CreateMap<ChargesWaivedDto, ChargesWaived>().ReverseMap();

            CreateMap<AddLoanAccountCommand, Account>().ReverseMap();


            CreateMap<MobileMoneyCashTopup, AddMobileMoneyCashTopupCommand>().ReverseMap();
            CreateMap<MobileMoneyCashTopup, MobileMoneyCashTopupDto>().ReverseMap();
            CreateMap<MobileMoneyCashTopup, ValidateMobileMoneyCashTopupCommand>().ReverseMap();

            CreateMap<OtherTransaction, AddOtherTransactionCommand>().ReverseMap();
            CreateMap<OtherTransaction, OtherTransactionDto>().ReverseMap();
            CreateMap<OtherTransaction, AddOtherTransactionMobileMoneyCommand>().ReverseMap();

            //CreateMap<BlockedAccount, BlockedAccount>();

            CreateMap<CashOutThirdParty, AddCashOutThirdPartyCommand>().ReverseMap();
            CreateMap<CashOutThirdParty, CashOutThirdPartyDto>().ReverseMap();


            CreateMap<FileDownloadInfo, AddFileDownloadInfoCommand>().ReverseMap();
            CreateMap<FileDownloadInfo, FileDownloadInfoDto>().ReverseMap();


            CreateMap<Account, MemberAccountsThirdPartyDto>();

            CreateMap<CashReplenishmentPrimaryTeller, CashReplenishmentPrimaryTellerDto>().ReverseMap();
            CreateMap<CashReplenishmentPrimaryTeller, ValidationCashReplenishmentPrimaryTellerCommand>().ReverseMap();
            CreateMap<CashReplenishmentPrimaryTeller, AddCashReplenishmentPrimaryTellerCommand>().ReverseMap();

            CreateMap<PrimaryTellerProvisioningHistory, PrimaryTellerProvisioningHistoryDto>().ReverseMap();
            CreateMap<PrimaryTellerProvisioningHistory, TillOpenAndCloseOfDayDto>().ReverseMap();

            CreateMap<SubTellerProvioningHistory, SubTellerProvioningHistoryDto>().ReverseMap();
            CreateMap<SubTellerProvioningHistory, TillOpenAndCloseOfDayDto>().ReverseMap();


            CreateMap<AccountingDay, AccountingDayDto>().ReverseMap();

            CreateMap<DailyTeller, AddDailyTellerCommand>().ReverseMap();
            CreateMap<DailyTeller, UpdateDailyTellerCommand>().ReverseMap();
            CreateMap<DailyTeller, DailyTellerDto>().ReverseMap();

            CreateMap<HolyDay, AddHolyDayCommand>().ReverseMap();
            CreateMap<HolyDay, UpdateHolyDayCommand>().ReverseMap();
            CreateMap<HolyDay, HolyDayDto>().ReverseMap();

            CreateMap<HolyDayRecurring, AddHolyDayRecurringCommand>().ReverseMap();
            CreateMap<HolyDayRecurring, UpdateHolyDayRecurringCommand>().ReverseMap();
            CreateMap<HolyDayRecurring, HolyDayRecurringDto>().ReverseMap();


            CreateMap<PaymentReceipt, PaymentReceiptDto>().ReverseMap();
            CreateMap<PaymentDetail, PaymentDetailDto>().ReverseMap();

            CreateMap<ReversalRequest, AddReversalRequestCommand>().ReverseMap();
            CreateMap<ReversalRequest, ApprovedReversalRequestCommand>().ReverseMap();
            CreateMap<ReversalRequest, ValidationReversalRequestCommand>().ReverseMap();
            CreateMap<ReversalRequest, CashCompletionOfReversalCommand>().ReverseMap();
            CreateMap<ReversalRequest, ReversalRequestDto>().ReverseMap();


            CreateMap<AddCashCeilingRequestCommand, CashCeilingRequest>().ReverseMap();
            CreateMap<CashCeilingRequest, CashCeilingRequestDto>().ReverseMap();

            CreateMap<GimacPayment, GimacPaymentCommand>().ReverseMap();

            CreateMap<OldLoanAccountingMaping, AddOldLoanAccountingMapingCommand>().ReverseMap();
            CreateMap<OldLoanAccountingMaping, UpdateOldLoanAccountingMapingCommand>().ReverseMap();
            CreateMap<OldLoanAccountingMaping, OldLoanAccountingMapingDto>().ReverseMap();

            CreateMap<GeneralDailyDashboard, GeneralDailyDashboardDto>().ReverseMap();


            CreateMap<Remittance, AddRemittanceCommand>().ReverseMap();
            CreateMap<Remittance, ValidationOfRemittanceCommand>().ReverseMap();
            CreateMap<Remittance, RemittanceDto>().ReverseMap();


            CreateMap<TransactionTrackerAccounting, TransactionTrackerAccountingDto>().ReverseMap();
            CreateMap<TransactionTrackerAccounting, AddTransactionTrackerAccountingCommand>().ReverseMap();
            CreateMap<TransactionTrackerAccounting, UpdateTransactionTrackerAccountingCommand>().ReverseMap();


            CreateMap<Vault, AddVaultCommand>().ReverseMap();
            CreateMap<Vault, UpdateVaultCommand>().ReverseMap();
            CreateMap<Vault, VaultDto>().ReverseMap();

            CreateMap<CashChangeHistory, CashChangeHistoryDto>().ReverseMap();

            CreateMap<CashCeilingRequest, AddCashCeilingRequestCommand>().ReverseMap();
            CreateMap<CashCeilingRequest, ValidationCashCeilingRequestCommand>().ReverseMap();
            CreateMap<CashCeilingRequest, CashCeilingRequestDto>().ReverseMap();

            CreateMap<StandingOrder, AddStandingOrderCommand>().ReverseMap();
            CreateMap<StandingOrder, UpdateStandingOrderCommand>().ReverseMap();
            CreateMap<StandingOrder, StandingOrderDto>().ReverseMap();

            CreateMap<FileUpload, FileUploadDto>().ReverseMap();
            CreateMap<SalaryUploadModel, SalaryUploadModelDto>().ReverseMap();


            CreateMap<SalaryExtract, SalaryExtractDto>().ReverseMap();

            CreateMap<SalaryAnalysisResult, SalaryAnalysisResultDto>().ReverseMap();
            CreateMap<SalaryAnalysisResultDetail, SalaryAnalysisResultDetailDto>().ReverseMap();

            CreateMap<MemberNoneCashOperation, AddMemberNoneCashOperationCommand>().ReverseMap();
            CreateMap<MemberNoneCashOperation, ValidateMemberNoneCashOperationCommand>().ReverseMap();
            CreateMap<MemberNoneCashOperation, MemberNoneCashOperationDto>().ReverseMap();

        }
    }
}//
 //MemberNoneCashOperationDto