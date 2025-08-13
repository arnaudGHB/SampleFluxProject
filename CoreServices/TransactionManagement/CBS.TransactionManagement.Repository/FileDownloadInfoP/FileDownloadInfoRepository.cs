using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Common.UnitOfWork;
using CBS.TransactionManagement.Data.Entity.FileDownloadInfoP;
using CBS.TransactionManagement.Domain;

namespace CBS.TransactionManagement.Repository.FileDownloadInfoP
{

    public class FileDownloadInfoRepository : GenericRepository<FileDownloadInfo, TransactionContext>, IFileDownloadInfoRepository
    {
        public FileDownloadInfoRepository(IUnitOfWork<TransactionContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
