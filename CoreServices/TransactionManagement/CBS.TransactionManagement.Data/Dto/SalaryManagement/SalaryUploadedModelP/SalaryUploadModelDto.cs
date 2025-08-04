using CBS.TransactionManagement.Data.Entity.SalaryFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CBS.TransactionManagement.Data.Dto.SalaryManagement.SalaryUploadedModelP
{
    public class SalaryUploadModelDto
    {
        public string Id { get; set; }
        public string SalaryCode { get; set; }
        public string Matricule { get; set; }
        public string Surname { get; set; }
        public string? MemberReference { get; set; }
        public string Name { get; set; }
        public string LaisonBankAccountNumber { get; set; }
        public decimal NetSalary { get; set; }
        public string BranchId { get; set; }
        public string BranchName { get; set; }
        public string BranchCode { get; set; }
        public string FileUploadId { get; set; }
        public string Phonumber { get; set; }
        public DateTime Date { get; set; }
        public string UploadedBy { get; set; }
        public string SalaryType { get; set; }

        public FileUpload FileUpload { get; set; }
        public SalaryUploadModelSummaryDto SalaryUploadModelSummaryDto { get; set; }
    }
    public class SalaryUploadModelSummaryDto
    {
      
        public decimal TotalNetSalary { get; set; }
        public int TotalMembers { get; set; }
        
    }
}
