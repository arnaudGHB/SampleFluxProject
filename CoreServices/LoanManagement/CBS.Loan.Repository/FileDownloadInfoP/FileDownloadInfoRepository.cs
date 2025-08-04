using CBS.NLoan.Common.GenericRespository;
using CBS.NLoan.Common.UnitOfWork;
using CBS.NLoan.Data.Dto.FileDownloadInfoP;
using CBS.NLoan.Domain.Context;

namespace CBS.NLoan.Repository.FileDownloadInfoP
{

    public class FileDownloadInfoRepository : GenericRepository<FileDownloadInfo, LoanContext>, IFileDownloadInfoRepository
    {
        public FileDownloadInfoRepository(IUnitOfWork<LoanContext> unitOfWork) : base(unitOfWork)
        {

        }

    }
}
