using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.DocumentP
{

    public class DocumentAttachedToLoanRepository : GenericRepository<DocumentAttachedToLoan, LoanContext>, IDocumentAttachedToLoanRepository
    {
        public DocumentAttachedToLoanRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
