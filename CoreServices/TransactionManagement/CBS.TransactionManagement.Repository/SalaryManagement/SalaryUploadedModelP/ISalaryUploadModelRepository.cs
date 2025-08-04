using CBS.TransactionManagement.Common.GenericRespository;
using CBS.TransactionManagement.Data;
using CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using CBS.TransactionManagement.Data.Entity.SalaryManagement.SalaryUploadedModelP;
using CBS.TransactionManagement.Dto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Repository.SalaryManagement.SalaryUploadedModelP
{
    public interface ISalaryUploadModelRepository : IGenericRepository<SalaryUploadModel>
    {
        Task<SalaryUploadModelSummaryDto> ExtractExcelDataToDatabase(IFormFile file, string SalaryType, List<CustomerDto> customers, List<BranchDto> branches);
        Task RemoveSalaryUploadModelDataForFile(string fileUploadId);


    }
}
