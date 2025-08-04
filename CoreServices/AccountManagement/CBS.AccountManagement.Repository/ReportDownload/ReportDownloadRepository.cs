using CBS.AccountManagement.Common;

using CBS.AccountManagement.Data;
using CBS.AccountManagement.Data.Entity;
using CBS.AccountManagement.Domain;

namespace CBS.AccountManagement.Repository
{
    public class ReportDownloadRepository : GenericRepository<ReportDownload, POSContext>, IReportDownloadRepository
    {
        public ReportDownloadRepository(IUnitOfWork<POSContext> unitOfWork) : base(unitOfWork)
        {
        }
    }
}