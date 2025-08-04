using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Domain;
using Microsoft.EntityFrameworkCore;

namespace CBS.AccountManagement.Repository
{
    

    public class BSAccountRepository : GenericRepository<BalanceSheetAccount, POSContext>, IBSAccountRepository
    {
        public BSAccountRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
    public class TrialBalanceRepository : GenericRepository<TrialBalance, POSContext>, ITrialBalanceRepository
    {
        public TrialBalanceRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
    public class DocumentRepository : GenericRepository<Document, POSContext>, IDocumentRepository
    {
        public DocumentRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }

    public class DocumentTypeRepository : GenericRepository<DocumentType, POSContext>, IDocumentTypeRepository
    {
        public DocumentTypeRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }

    public class DocumentReferenceCodeRepository : GenericRepository<DocumentReferenceCode, POSContext>, IDocumentReferenceCodeRepository
    {
        public DocumentReferenceCodeRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
    public class CorrespondingMappingRepository : GenericRepository<CorrespondingMapping, POSContext>, ICorrespondingMappingRepository
    {
        public CorrespondingMappingRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
    public class CorrespondingMappingExceptionRepository : GenericRepository<CorrespondingMappingException, POSContext>, ICorrespondingMappingExceptionRepository
    {
        public CorrespondingMappingExceptionRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }

    public class ConditionalAccountReferenceFinancialReportRepository : GenericRepository<ConditionalAccountReferenceFinancialReport, POSContext>, IConditionalAccountReferenceFinancialReportRepository
    {
        public ConditionalAccountReferenceFinancialReportRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}