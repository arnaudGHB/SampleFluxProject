using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;

namespace CBS.AccountManagement.Repository
{
    public interface IDocumentRepository : IGenericRepository<Document>
    {
    }
    public interface IDocumentTypeRepository : IGenericRepository<DocumentType>
    {
    }
    public interface IDocumentReferenceCodeRepository : IGenericRepository<DocumentReferenceCode>
    {
    }
    
          public interface IBSAccountRepository : IGenericRepository<BalanceSheetAccount>
    {
    }
    public interface ITrialBalanceRepository : IGenericRepository<TrialBalance>
    {
    }
    public interface ICorrespondingMappingExceptionRepository : IGenericRepository<CorrespondingMappingException>
    {
    }
    public interface ICorrespondingMappingRepository : IGenericRepository<CorrespondingMapping>
    {
    }
    //

    public interface IConditionalAccountReferenceFinancialReportRepository : IGenericRepository<ConditionalAccountReferenceFinancialReport>
    {
    }
}
