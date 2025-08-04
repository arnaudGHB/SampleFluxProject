using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Entity.DocumentP;
using CBS.NLoan.Domain.Context;
using Microsoft.EntityFrameworkCore;

namespace CBS.NLoan.Repository.DocumentP
{

    public class DocumentPackRepository : GenericRepository<DocumentPack, LoanContext>, IDocumentPackRepository
    {
        public DocumentPackRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {
        }
        public DocumentPack GetDocumentPack(string documentPackId)
        {
            return Context.DocumentPacks
                .Include(s => s.DocumentJoins)
                .ThenInclude(sc => sc.Document)
                .FirstOrDefault(s => s.Id == documentPackId);
        }
        public List<DocumentPack> GetAllDocumentPack()
        {
            return Context.DocumentPacks
                .Include(s => s.DocumentJoins)
                .ThenInclude(sc => sc.Document)
                .ToList();
        }

    }
}
