using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.DocumentP
{

    public class DocumentRepository : GenericRepository<Document, LoanContext>, IDocumentRepository
    {
        public DocumentRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
