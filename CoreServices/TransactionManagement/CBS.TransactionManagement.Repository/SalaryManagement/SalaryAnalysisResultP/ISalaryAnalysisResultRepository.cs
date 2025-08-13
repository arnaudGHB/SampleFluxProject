using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryAnalysisResultP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.StandingOrderP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.SalaryManagement.SalaryAnalysisResultP
{
    public interface ISalaryAnalysisResultRepository : IGenericRepository<SalaryAnalysisResult>
    {
    }
}
