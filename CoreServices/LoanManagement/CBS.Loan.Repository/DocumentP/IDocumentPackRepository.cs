using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Data.Entity.DocumentP;

namespace CBS.NLoan.Repository.DocumentP
{
    public interface IDocumentPackRepository : IGenericRepository<DocumentPack>
    {
        public DocumentPack GetDocumentPack(string documentPackId);

        public List<DocumentPack> GetAllDocumentPack();
    }

}
